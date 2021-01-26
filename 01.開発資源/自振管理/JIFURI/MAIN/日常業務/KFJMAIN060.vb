Option Strict On
Option Explicit On

Imports System.Text
Imports System.Collections
Imports System.IO
Imports CASTCommon

Public Class KFJMAIN060

    'ソートオーダーフラグ
    Dim SortOrderFlag As Boolean = True

    'クリックした列の番号
    Dim ClickedColumn As Integer

    Private Const MaxCounter As Integer = 60

    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private BLOG As New CASTCommon.BatchLOG("KFJMAIN060", "返還データ作成画面")
    Private Const msgTitle As String = "返還データ作成画面(KFJMAIN060)"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
#Region "宣言"

    'Dim intSCH_COUNT As Integer

    'Public intMAXCNT As Integer
    'Public intMAXLINE As Integer
    'Public intPAGE_CNT As Integer
    'Public P As Integer
    'Public L As Integer

    Public strTORIS_CODE As String
    Public strTORIF_CODE As String
    Public strFURIDATE As String
    Public strTAKOU_KBN As String
    Public strFUNOU_FLG As String
    Public strHENKAN_FLG As String
    Public strMOTIKOMI_SEQ As String
    Public strITAKU_NAME As String
    Public strITAKU_CODE As String
    Public strKIGYO As String
    Public strFURI As String
    Private strRet As String
    Public strKEKKA_HENKYAKU_KBN As String

    'Public intP_PAGE As Integer

    '帳票印刷用
    'Private intGYO As Integer
    'Private intMOJI_SIZE As Integer = 5
    'Private intP As Integer
    'Private intL As Integer

    '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
    'ソース改善
    Private MainDB As CASTCommon.MyOracle
    'Private MainDB As New CASTCommon.MyOracle
    '2012/11/15 saitou 大垣信金 MODIFY --------------------------------------------------<<<<

    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-12(RSV2対応) -------------------- START
    '--------------------------------
    ' INI関連項目
    '--------------------------------
    Friend Structure INI_INFO
        Dim COMMON_DEN As String                ' DENフォルダ
        Dim RSV2_MAKECHK As String              ' 返還データ既存ファイルチェック区分
        '2017/04/28 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）---------------------- START
        Dim WEB_DENSO As String                 'WEB伝送利用区分
        '2017/04/28 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）---------------------- END
    End Structure
    Private IniInfo As INI_INFO
    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-12(RSV2対応) -------------------- END
    '2017/04/26 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ START
    Private SORT_KEY As String = ""
    '2017/04/26 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ END

#End Region

#Region "画面のＬＯＡＤ"
    Private Sub KFJMAIN060_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Visible = False
        '#####################################
        'ログの書込に必要な情報の取得
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            'システム日付
            GCom.GetSysDate = Date.Now
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-12(RSV2対応) -------------------- START
            '--------------------------------
            ' INI情報取得
            '--------------------------------
            If SetIniFIle() = False Then
                Exit Try
            End If
            ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-12(RSV2対応) -------------------- END
            '2017/04/26 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ START
            SORT_KEY = GCom.GetObjectParam("KFJMAIN060", "SORT", "0")
            '2017/04/26 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ END
            '2017/04/28 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）---------------------- START
            If IniInfo.WEB_DENSO = "1" Then
                'WEB伝送利用する
                Label12.Text = "ALL1:依頼書　ALL2:伝票　ALL3:伝送　ALL4:WEB伝送"
            Else
                'WEB伝送利用しない
                Label12.Text = "ALL1:依頼書　ALL2:伝票　ALL3:伝送"
            End If
            '2017/04/28 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）---------------------- END

            '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
            'データベースオープン
            MainDB = New CASTCommon.MyOracle
            '2012/11/15 saitou 大垣信金 MODIFY --------------------------------------------------<<<<
            Dim SQL As New StringBuilder(128)
            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim strSystemDate As String
            strSystemDate = CASTCommon.Calendar.Now.ToString("yyyyMMdd")

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '表示委託先の検索（スケジュールの検索）
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            SQL = New StringBuilder(128)
            SQL.Append("SELECT ITAKU_NNAME_T")
            SQL.Append(", TORIS_CODE_S")
            SQL.Append(", TORIF_CODE_S")
            SQL.Append(", FURI_DATE_S")
            SQL.Append(", FURI_CODE_S")
            SQL.Append(", KIGYO_CODE_S")
            SQL.Append(", BAITAI_CODE_T")
            SQL.Append(", HENKAN_FLG_S")
            SQL.Append(", KEKKA_HENKYAKU_KBN_T")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(", TORIMAST")
            SQL.Append(" WHERE SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
            SQL.Append(" AND SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
            SQL.Append(" AND SCHMAST.HENKAN_YDATE_S = '" & strSystemDate & "'")
            SQL.Append(" AND MOTIKOMI_KBN_T <> '1'")
            SQL.Append(" AND BAITAI_CODE_T <> '07'")    '2010/03/25 追加 学校以外
            SQL.Append(" AND FUNOU_FLG_S = '1'")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            '2017/04/26 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ START
            If SORT_KEY = "" Then
                SQL.Append(" ORDER BY FURI_DATE_S ASC")
                SQL.Append(", SOUSIN_KBN_S ASC")
                SQL.Append(", TORIS_CODE_S ASC")
                SQL.Append(", TORIF_CODE_S ASC")
            Else
                SQL.Append(" ORDER BY " & SORT_KEY)
            End If
            'SQL.Append(" ORDER BY FURI_DATE_S ASC")
            'SQL.Append(", SOUSIN_KBN_S ASC")
            'SQL.Append(", TORIS_CODE_S ASC")
            'SQL.Append(", TORIF_CODE_S ASC")
            '2017/04/26 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ END

            Dim bSQL As Boolean
            bSQL = OraReader.DataReader(SQL)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面の表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim LineColor As Color
            Dim ROW As Integer = 1

            With Me.ListView1
                .Clear()
                .Columns.Add("", 0, HorizontalAlignment.Left)
                .Columns.Add("項番", 45, HorizontalAlignment.Right)
                .Columns.Add("取引先名", 220, HorizontalAlignment.Left)
                .Columns.Add("取引先コード", 115, HorizontalAlignment.Center)
                .Columns.Add("振替日", 95, HorizontalAlignment.Center)
                .Columns.Add("振替ｺｰﾄﾞ", 75, HorizontalAlignment.Center)
                .Columns.Add("企業ｺｰﾄﾞ", 75, HorizontalAlignment.Center)
                .Columns.Add("媒体", 65, HorizontalAlignment.Center)
                .Columns.Add("処理済", 70, HorizontalAlignment.Center)
            End With

            If bSQL = True Then
                Do
                    Dim Data(8) As String
                    Data(1) = CStr(ROW)                                     ' 項番
                    Data(2) = OraReader.GetString("ITAKU_NNAME_T").Trim     ' 取引先名
                    Data(3) = OraReader.GetString("TORIS_CODE_S") & "-" _
                            & OraReader.GetString("TORIF_CODE_S")           ' 取引先コード
                    Data(4) = OraReader.GetString("FURI_DATE_S")            ' 振替日
                    If Data(4).Length >= 8 Then
                        Data(4) = Data(4).Substring(0, 4) & "/" _
                                & Data(4).Substring(4, 2) & "/" _
                                & Data(4).Substring(6, 2)
                    End If
                    Data(5) = OraReader.GetString("FURI_CODE_S")            ' 振替コード
                    Data(6) = OraReader.GetString("KIGYO_CODE_S")           ' 企業コード
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    Data(7) = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"), _
                                                            OraReader.GetString("BAITAI_CODE_T"))
                    'Select Case OraReader.GetString("BAITAI_CODE_T")        ' 媒体コード
                    '    Case "00"
                    '        Data(7) = "伝送"        ' 0, 伝送
                    '    Case "01", "02"
                    '        Data(7) = "3.5FD"       ' 1, ＦＤ３．５
                    '    Case "04"
                    '        Data(7) = "依頼書"      ' 4, 依頼書
                    '    Case "05"
                    '        Data(7) = "MT"          ' 5, ＭＴ
                    '    Case "06"
                    '        Data(7) = "CMT"         ' 6, ＣＭＴ
                    '    Case "07"
                    '        Data(7) = "学校"        ' 7, 学校自振
                    '    Case "09"
                    '        Data(7) = "伝票"        ' 9, 伝票
                    '        '2012/06/30 標準版　WEB伝送対応
                    '    Case "10"
                    '        Data(7) = "WEB伝送"     '10, WEB伝送
                    '    Case "11"
                    '        Data(7) = "DVD-RAM"     '11, DVD
                    '    Case "12"
                    '        Data(7) = "その他"      '12, その他
                    '    Case "13"
                    '        Data(7) = "その他"      '13, その他
                    '    Case "14"
                    '        Data(7) = "その他"      '14, その他
                    '    Case "15"
                    '        Data(7) = "その他"      '15, その他
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END

                    Dim ForeColoe As Color = Color.Black
                    If OraReader.GetString("HENKAN_FLG_S") = "1" Then
                        Data(8) = "済"              ' 返還済み
                    Else
                        If OraReader.GetString("KEKKA_HENKYAKU_KBN_T") = "0" Then
                            Data(8) = "返却不要"
                        Else
                            Data(8) = ""
                        End If
                    End If

                    If ROW Mod 2 = 0 Then
                        LineColor = Color.White
                    Else
                        LineColor = Color.WhiteSmoke
                    End If

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    ROW += 1
                    OraReader.NextRead()
                Loop Until OraReader.EOF
            Else
                '対象スケジュールがなくても次画面遷移
                '    Sql.Length = 0
                '    Sql.Append ("SELECT COUNT(*) AS CNT")
                '    Sql.Append (" FROM TORIMAST")
                '    Sql.Append (", SCHMAST")
                '    Sql.Append (" WHERE TORIMAST.TORIS_CODE_T = SCHMAST.TORIS_CODE_S")
                '    Sql.Append (" AND TORIMAST.TORIF_CODE_T = SCHMAST.TORIF_CODE_S")
                '    Sql.Append (" AND HENKAN_YDATE_S = '" & strSystemDate & "'")
                '    Sql.Append (" AND MOTIKOMI_KBN_T <> '1'")
                '    Sql.Append (" AND FUNOU_FLG_S = '1'")
                '    Sql.Append (" AND TYUUDAN_FLG_S = '0'")
                '
                '    bSQL = OraReader.DataReader(Sql)
                '
                '    If bSQL = True Then
                '        If OraReader.GetInt("CNT") = 0 Then
                '
                '            BLOG.Write(gstrUSER_ID, "", strSystemDate, "スケジュールマスタ検索（画面LOAD）", "失敗", "対象なし０件")
                '            '   返還データ作成対象のスケジュールが存在しません。(ID:MSG0086W)"
                '            MessageBox.Show(MSG0086W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '        End If
                '    Else
                '        BLOG.Write(gstrUSER_ID, "", strSystemDate, "スケジュールマスタ検索（画面LOAD）", "失敗", "マスタ検索失敗")
                '        '   {0}の{1}処理に失敗しました。(ID:MSG0027E)"
                '        MessageBox.Show(String.Format(MSG0027E, "スケジュールマスタ", "検索"), _
                '                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '    End If
                '    If Not OraReader Is Nothing Then
                '        OraReader.Close()
                '    End If
                '    Exit Sub
            End If
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
            'データベースクローズ
            If Not MainDB Is Nothing Then MainDB.Close()
            '2012/11/15 saitou 大垣信金 MODIFY --------------------------------------------------<<<<
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

        Me.Visible = True
    End Sub
#End Region
#Region "関数"
    'Private Function fn_BAITAI_YOMIKAE(ByVal astrBAITAI_CODE As String) As String
    '    '=====================================================================================
    '    'NAME           :fn_BAITAI_YOMIKAE
    '    'Parameter      :
    '    'Description    :媒体コードより媒体名取得
    '    'Return         :媒体名
    '    'Create         :2004/09/02
    '    'Update         :
    '    '=====================================================================================
    '    fn_BAITAI_YOMIKAE = ""
    '    Select Case astrBAITAI_CODE
    '        Case "0"
    '            fn_BAITAI_YOMIKAE = "伝送"
    '        Case "1", "2" '2007/04/10　ＦＤ２枚の表示を追加
    '            fn_BAITAI_YOMIKAE = "3.5FD"
    '        Case "4"
    '            fn_BAITAI_YOMIKAE = "依頼書"
    '        Case "5"
    '            fn_BAITAI_YOMIKAE = "ＭＴ"
    '        Case "6"
    '            fn_BAITAI_YOMIKAE = "ＣＭＴ"
    '        Case "7"
    '            fn_BAITAI_YOMIKAE = "学校"
    '        Case "9"
    '            fn_BAITAI_YOMIKAE = "伝票"
    '    End Select
    'End Function
    'Private Function fn_FLG_YOMIKAE(ByVal astrFLG As String) As String
    '    fn_FLG_YOMIKAE = ""
    '    Select Case astrFLG
    '        Case "0"
    '            fn_FLG_YOMIKAE = ""
    '        Case "1"
    '            fn_FLG_YOMIKAE = "済"
    '    End Select
    'End Function
    Function fn_check_text(ByVal chkKbn As Integer) As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :実行ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/07/14
        'Update         :
        '============================================================================
        fn_check_text = False

        '実行ボタン押下時のみ
        If chkKbn = 1 Then
            '2013/12/24 saitou 標準版 性能向上 UPD -------------------------------------------------->>>>
            '取引先主コード必須チェック
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If
            '取引先副コード必須チェック
            If txtTorifCode.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorifCode.Focus()
                Return False
            End If
            'If fn_CHECK_NUM_MSG(txtTorisCode.Text, "取引先主コード", msgTitle) = False Then
            '    txtTorisCode.Focus()
            '    Exit Function
            'End If
            'If fn_CHECK_NUM_MSG(txtTorifCode.Text, "取引先副コード", msgTitle) = False Then
            '    txtTorifCode.Focus()
            '    Exit Function
            'End If
            '2013/12/24 saitou 標準版 性能向上 UPD --------------------------------------------------<<<<
        End If

        If txtTorisCode.Text = "1111111111" AndAlso txtTorifCode.Text = "11" Then
        ElseIf txtTorisCode.Text = "2222222222" AndAlso txtTorifCode.Text = "22" Then
            '2017/04/26 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ START
        ElseIf txtTorisCode.Text = "3333333333" AndAlso txtTorifCode.Text = "33" Then
        ElseIf txtTorisCode.Text = "4444444444" AndAlso txtTorifCode.Text = "44" AndAlso IniInfo.WEB_DENSO = "1" Then
            '2017/04/26 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ END
        Else
            '2013/12/24 saitou 標準版 性能向上 UPD -------------------------------------------------->>>>
            '年必須チェック
            If txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
            '月必須チェック
            If txtFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '日付必須チェック
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            'If fn_CHECK_NUM_MSG(txtFuriDateY.Text, "振替年", msgTitle) = False Then
            '    txtFuriDateY.Focus()
            '    Exit Function
            'Else
            '    '処理年チェック
            '    If txtFuriDateY.Text.Length <> 0 Then
            '        If txtFuriDateY.Text.Length <> 4 Then

            '            With txtFuriDateY
            '                Dim Temp As String = New String("0"c, .MaxLength)
            '                .Text = String.Format("{0:" & Temp & "}", GCom.NzDec(.Text))
            '            End With
            '        End If
            '    Else
            '        MessageBox.Show(String.Format(MSG0281W, "振替日（年）"), _
            '                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtFuriDateY.Focus()
            '        txtFuriDateY.SelectionStart = 0
            '        txtFuriDateY.SelectionLength = txtFuriDateY.Text.Length
            '        Exit Function
            '    End If
            'End If
            'If fn_CHECK_NUM_MSG(txtFuriDateM.Text, "振替月", msgTitle) = False Then
            '    txtFuriDateM.Focus()
            '    Exit Function
            'Else
            '    If CDbl(txtFuriDateM.Text) < 1 Or CDbl(txtFuriDateM.Text) > 12 Then
            '        MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtFuriDateM.Focus()
            '        Exit Function
            '    End If
            'End If
            'If fn_CHECK_NUM_MSG(txtFuriDateD.Text, "振替日", msgTitle) = False Then
            '    txtFuriDateD.Focus()
            '    Exit Function
            'Else
            '    If CDbl(txtFuriDateD.Text) < 1 Or CDbl(txtFuriDateD.Text) > 31 Then
            '        MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtFuriDateD.Focus()
            '        Exit Function
            '    End If
            'End If

            'Dim onDate As Date
            'Dim onText(2) As Integer

            'onText(0) = GCom.NzInt(Me.txtFuriDateY.Text, 0)
            'onText(1) = GCom.NzInt(Me.txtFuriDateM.Text, 0)
            'onText(2) = GCom.NzInt(Me.txtFuriDateD.Text, 0)

            'Select Case GCom.SET_DATE(onDate, onText)
            '    Case Is = -1
            '        Return True
            '    Case Is = 1
            '        MessageBox.Show(String.Format(MSG0281W, "振替日（月）"), _
            '                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtFuriDateM.Focus()
            '        Exit Function
            '    Case Is = 2
            '        MessageBox.Show(String.Format(MSG0281W, "振替日（日）"), _
            '                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtFuriDateD.Focus()
            '        Exit Function
            '    Case Else
            '        MessageBox.Show(String.Format(MSG0281W, "振替日（年）"), _
            '                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtFuriDateY.Focus()
            '        Exit Function
            'End Select
            '2013/12/24 saitou 標準版 性能向上 UPD --------------------------------------------------<<<<
        End If

        fn_check_text = True
    End Function

    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-12(RSV2対応) -------------------- START
    '================================
    ' INI情報取得
    '================================
    Private Function SetIniFIle() As Boolean

        Try
            BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "開始", "")

            IniInfo.COMMON_DEN = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If IniInfo.COMMON_DEN.ToUpper = "ERR" OrElse IniInfo.COMMON_DEN = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "DENフォルダ", "COMMON", "DEN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:DENフォルダ 分類:COMMON 項目:DEN")
                Return False
            End If

            IniInfo.RSV2_MAKECHK = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MAKECHK")
            If IniInfo.RSV2_MAKECHK.ToUpper = "ERR" OrElse IniInfo.RSV2_MAKECHK = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "既存データチェック区分", "RSV2_V1.0.0", "MAKECHK"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:既存データチェック区分 分類:RSV2_V1.0.0 項目:MAKECHK")
                Return False
            End If

            '2017/04/28 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）---------------------- START
            IniInfo.WEB_DENSO = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_DENSO")
            If IniInfo.WEB_DENSO.ToUpper = "ERR" OrElse IniInfo.WEB_DENSO = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "WEB伝送利用区分", "WEB_DEN", "WEB_DENSO"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:WEB伝送利用区分 分類:WEB_DEN 項目:WEB_DENSO")
                Return False
            End If
            '2017/04/28 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）---------------------- END

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write_Err(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "", ex)
            Return False
        Finally
            BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "終了", "")
        End Try
    End Function


    '================================
    ' 既存ファイルチェック
    '================================
    Private Function CheckHenkanData(ByVal MainDB As MyOracle, ByRef CheckHenkanDataFlg As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "既存ファイルチェック", "開始", "")

            SQL = New StringBuilder(128)

            '------------------------------------------------
            ' 取引先マスタ取得
            '------------------------------------------------
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T = '" & strTORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & strTORIF_CODE & "'")
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                If GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T")) = "00" Then
                    Dim FileName As String = ""
                    If GCom.NzStr(OraReader.Reader.Item("MULTI_KBN_T")) = "0" Then
                        FileName = "O" & strTORIS_CODE & strTORIF_CODE & ".dat"
                    Else
                        FileName = "O" & GCom.NzStr(OraReader.Reader.Item("ITAKU_KANRI_CODE_T")) & ".dat"
                    End If

                    If File.Exists(Path.Combine(IniInfo.COMMON_DEN, FileName)) = True Then
                        CheckHenkanDataFlg = "1"
                        BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "既存ファイルチェック", "成功", "既存ファイルあり")
                        Return True
                    Else
                        BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "既存ファイルチェック", "成功", "既存ファイルなし")
                        CheckHenkanDataFlg = "0"
                        Return True
                    End If
                Else
                    CheckHenkanDataFlg = "0"
                    BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "既存ファイルチェック", "成功", "伝送以外")
                    Return True
                End If
            Else
                BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "既存ファイルチェック", "失敗", "取引先マスタ該当なし")
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write_Err(LW.UserID, LW.ToriCode, LW.FuriDate, "既存ファイルチェック", "", ex)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "既存ファイルチェック", "終了", "")
        End Try
    End Function
    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-12(RSV2対応) -------------------- END

#End Region
#Region "終了ボタン"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region "印刷"
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        '============================================================================
        'NAME           :btnPrint_Click
        'Parameter      :
        'Description    :印刷ボタン押下時の処理
        'Return         :
        'Create         :2009/09/09
        'Update         :
        '============================================================================

        Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items
        Dim CreateCSV As New KFJP016

        Dim strSystemDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
        Dim strTimeStamp As String = CASTCommon.Calendar.Now.ToString("HHmmss")

        Try
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            If ListView1.Items.Count > 0 Then
                If MessageBox.Show(String.Format(MSG0013I, "返還データ作成一覧"), _
                                   msgTitle, _
                                   MessageBoxButtons.YesNo, _
                                   MessageBoxIcon.Question) = DialogResult.No Then
                    Exit Sub
                End If
            Else
                MessageBox.Show(MSG0106W, _
                                msgTitle, _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Warning)
                Exit Sub
            End If

            '印刷データの作成
            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName
            For Each item As ListViewItem In nItems

                CreateCSV.OutputCsvData(strSystemDate, True)                           ' 処理日（システム日付）
                CreateCSV.OutputCsvData(strTimeStamp, True)                            ' タイムスタンプ
                CreateCSV.OutputCsvData(item.SubItems(2).Text, True)    ' 取引先名
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Replace("-", "").Substring(0, 10), True)     ' 取引先主コード
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Replace("-", "").Substring(10, 2), True)     ' 取引先副コード
                CreateCSV.OutputCsvData(item.SubItems(4).Text.Replace("/", ""), True)      ' 振替日
                CreateCSV.OutputCsvData(item.SubItems(5).Text, True)      ' 振替コード
                CreateCSV.OutputCsvData(item.SubItems(6).Text, True)     ' 企業コード
                CreateCSV.OutputCsvData(item.SubItems(7).Text, True)     ' 媒体コード
                CreateCSV.OutputCsvData(item.SubItems(8).Text, False, True)           ' 返還フラグ

            Next
            CreateCSV.CloseCsv()

            'レポエージェント印刷
            Dim ExeRepo As New CAstReports.ClsExecute

            ExeRepo.SetOwner = Me
            Dim Param As String = ""
            Param = GCom.GetUserID & "," & strCSVFileName
            '返還データ作成一覧印刷
            Dim nRet As Integer = ExeRepo.ExecReport("KFJP016.exe", Param)
            If nRet <> 0 Then
                MessageBox.Show(String.Format(MSG0004E, "返還データ作成一覧"), _
                                msgTitle, _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Error)
                BLOG.Write(gstrUSER_ID, "", "", "返還データ作成一覧印刷", "失敗", "印刷パラメータ:" & Param & " ,Err:" & Err.Description)
                Return
            End If
            MessageBox.Show(String.Format(MSG0014I, "返還データ作成一覧"), _
                            msgTitle, _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try

    End Sub
#End Region
#Region "作成ボタン"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :作成ボタン押下時の処理
        'Return         :
        'Create         :2004/09/02
        'Update         :
        '=====================================================================================

        '2012/11/15 saitou 大垣信金 ADD -------------------------------------------------->>>>
        'ソース改善
        MainDB = New MyOracle
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        '2012/11/15 saitou 大垣信金 ADD --------------------------------------------------<<<<
        '2017/04/26 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ START
        Dim oraReader_TAKOUSCH As CASTCommon.MyOracleReader
        Dim TAKOUSCH_SQL As StringBuilder
        '2017/04/26 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ END

        Try
           BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '必須項目の入力値チェック
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '必須項目：取引先コード、振替日

            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text(1) = False Then
                Exit Sub
            End If

            strTORIS_CODE = txtTorisCode.Text
            strTORIF_CODE = txtTorifCode.Text
            strFURIDATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            '--------------------------------
            '取引先マスタ存在チェック
            '--------------------------------
            '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
            'ソース改善(clsFUSIONの関数は使用しない)
            Dim SQL As New StringBuilder
            '2017/04/26 タスク）西野 CHG 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ START
            If String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("111111111111") = False AndAlso _
               String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("222222222222") = False AndAlso _
               String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("333333333333") = False AndAlso _
               (String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("444444444444") = False OrElse _
               (String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("444444444444") = True AndAlso IniInfo.WEB_DENSO <> "1")) Then
                'If String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("111111111111") = False And _
                '      String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("222222222222") = False Then
                '2017/04/26 タスク）西野 CHG 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ END
                SQL.Length = 0
                SQL.Append("select * from TORIMAST")
                SQL.Append(" where TORIS_CODE_T = " & SQ(strTORIS_CODE))
                SQL.Append(" and TORIF_CODE_T = " & SQ(strTORIF_CODE))

                If oraReader.DataReader(SQL) = True Then
                    oraReader.Close()
                Else
                    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtTorisCode.Focus()
                    Return
                End If
            End If
            'If strTORIS_CODE & strTORIF_CODE <> "111111111111" And strTORIS_CODE & strTORIF_CODE <> "222222222222" Then
            '    If clsFUSION.fn_Select_TORIMAST(strTORIS_CODE, strTORIF_CODE, gastrITAKU_KNAME_T, _
            '                          gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
            '                           gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then  '検索にヒットしたら
            '    Else
            '        MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtTorisCode.Focus()
            '        Exit Sub
            '    End If
            'End If
            '2012/11/15 saitou 大垣信金 MODIFY --------------------------------------------------<<<<
            '2017/04/26 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ START
            If String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("333333333333") = False AndAlso _
               (String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("444444444444") = False OrElse _
               (String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("444444444444") = True AndAlso IniInfo.WEB_DENSO <> "1")) Then
                '2017/04/26 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ END
                '--------------------------------
                'スケジュールチェック
                '--------------------------------
                '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
                'データベース統一
                SQL.Length = 0
                SQL.Append("select * from SCHMAST, TORIMAST")
                SQL.Append(" where TORIS_CODE_S = TORIS_CODE_T")
                SQL.Append(" and TORIF_CODE_S = TORIF_CODE_T")
                SQL.Append(" and FURI_DATE_S = " & SQ(strFURIDATE))
                SQL.Append(" and FUNOU_FLG_S = '1'")
                SQL.Append(" and TYUUDAN_FLG_S = '0'")
                Select Case strTORIS_CODE & strTORIF_CODE
                    Case "111111111111"             '依頼書ベース全件指定時
                        SQL.Append(" and BAITAI_CODE_S = '04'")
                        SQL.Append(" and MOTIKOMI_KBN_T = '0'")     'センター直接持込をはずす
                    Case "222222222222"             '伝票ベース全件指定時
                        SQL.Append(" and BAITAI_CODE_S = '09'")
                        SQL.Append(" and MOTIKOMI_KBN_T = '0'")     'センター直接持込をはずす
                    Case Else
                        SQL.Append(" and TORIS_CODE_S = " & SQ(strTORIS_CODE))
                        SQL.Append(" and TORIF_CODE_S = " & SQ(strTORIF_CODE))
                End Select

                If oraReader.DataReader(SQL) = True Then
                    strTAKOU_KBN = oraReader.GetString("TAKO_KBN_T")
                    strHENKAN_FLG = oraReader.GetString("HENKAN_FLG_S")
                    strMOTIKOMI_SEQ = oraReader.GetString("MOTIKOMI_SEQ_S")
                    strITAKU_NAME = oraReader.GetString("ITAKU_NNAME_T")
                    strKIGYO = oraReader.GetString("KIGYO_CODE_T")
                    strFURI = oraReader.GetString("FURI_CODE_T")
                    strITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                    strKEKKA_HENKYAKU_KBN = oraReader.GetString("KEKKA_HENKYAKU_KBN_T")

                    If strKEKKA_HENKYAKU_KBN = "0" Then
                        MessageBox.Show(MSG0316W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Me.txtTorisCode.Focus()
                        Return
                    End If
                Else
                    MessageBox.Show(MSG0086W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Me.txtTorisCode.Focus()
                    Return
                End If
                oraReader.Close()

                'gstrSSQL = "SELECT * FROM SCHMAST,TORIMAST WHERE "
                'gstrSSQL = gstrSSQL & "TORIS_CODE_S = TORIS_CODE_T AND "
                'gstrSSQL = gstrSSQL & "TORIF_CODE_S = TORIF_CODE_T AND "
                'gstrSSQL = gstrSSQL & "FURI_DATE_S = '" & Trim(strFURIDATE) & "' AND "
                'gstrSSQL = gstrSSQL & "FUNOU_FLG_S = '1' AND "
                'gstrSSQL = gstrSSQL & "TYUUDAN_FLG_S = '0' AND "
                'Select Case strTORIS_CODE & strTORIF_CODE    '依頼書ベース全件指定時
                '    Case "111111111111"
                '        gstrSSQL = gstrSSQL & "BAITAI_CODE_S = '04' AND "
                '        gstrSSQL = gstrSSQL & "MOTIKOMI_KBN_T = '0'"    'センター直接持込をはずす
                '    Case "222222222222"                         '伝票ベース全件指定時
                '        gstrSSQL = gstrSSQL & "BAITAI_CODE_S = '09' AND "
                '        gstrSSQL = gstrSSQL & "MOTIKOMI_KBN_T = '0'"    'センター直接持込をはずす
                '    Case Else
                '        gstrSSQL = gstrSSQL & "TORIS_CODE_S = '" & Trim(strTORIS_CODE) & "' AND "
                '        gstrSSQL = gstrSSQL & "TORIF_CODE_S = '" & Trim(strTORIF_CODE) & "'"
                'End Select
                'gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
                'gdbcCONNECT.Open()

                'gdbCOMMAND = New OracleClient.OracleCommand
                'gdbCOMMAND.CommandText = gstrSSQL
                'gdbCOMMAND.Connection = gdbcCONNECT

                'gdbrREADER = gdbCOMMAND.ExecuteReader
                ''読込のみ
                'If gdbrREADER.Read = False Then
                '    MessageBox.Show(MSG0086W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '    gdbcCONNECT.Close()
                '    txtTorisCode.Focus()
                '    Exit Sub
                'Else
                '    strTAKOU_KBN = CStr(gdbrREADER.Item("TAKO_KBN_T"))
                '    strHENKAN_FLG = CStr(gdbrREADER.Item("HENKAN_FLG_S"))
                '    strMOTIKOMI_SEQ = CStr(gdbrREADER.Item("MOTIKOMI_SEQ_S"))
                '    strITAKU_NAME = CStr(gdbrREADER.Item("ITAKU_NNAME_T"))
                '    strKIGYO = CStr(gdbrREADER.Item("KIGYO_CODE_T"))
                '    strFURI = CStr(gdbrREADER.Item("FURI_CODE_T"))
                '    strITAKU_CODE = CStr(gdbrREADER.Item("ITAKU_CODE_T"))
                '    strKEKKA_HENKYAKU_KBN = CStr(gdbrREADER.Item("KEKKA_HENKYAKU_KBN_T"))
                '    If strKEKKA_HENKYAKU_KBN = "0" Then
                '        MessageBox.Show(MSG0316W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                '        gdbcCONNECT.Close()
                '        txtTorisCode.Focus()
                '        Exit Sub
                '    End If
                'End If
                'gdbcCONNECT.Close()
                '2012/11/15 saitou 大垣信金 MODIFY --------------------------------------------------<<<<

                '--------------------------------
                '他行不能チェック
                '--------------------------------
                '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
                'データベース統一
                If String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("111111111111") = False And String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("222222222222") = False Then
                    If strTAKOU_KBN.Equals("1") = True Then
                        SQL.Length = 0
                        SQL.Append("select * from TAKOSCHMAST")
                        SQL.Append(" where TORIS_CODE_U = " & SQ(strTORIS_CODE))
                        SQL.Append(" and TORIF_CODE_U = " & SQ(strTORIF_CODE))
                        SQL.Append(" and FURI_DATE_U = " & SQ(strFURIDATE))

                        If oraReader.DataReader(SQL) = True Then
                            While oraReader.EOF = False
                                If oraReader.GetString("FUNOU_FLG_U") = "0" Then
                                    MessageBox.Show(String.Format(MSG0335W, oraReader.GetString("TKIN_NO_U"), _
                                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning))
                                    oraReader.Close()
                                    Me.txtTorisCode.Focus()
                                    Return
                                End If
                                oraReader.NextRead()
                            End While
                        End If
                    End If
                End If
                'If strTORIS_CODE & strTORIF_CODE <> "111111111111" And strTORIS_CODE & strTORIF_CODE <> "222222222222" Then
                '    If strTAKOU_KBN = "1" Then
                '        gstrSSQL = "SELECT * FROM TAKOSCHMAST WHERE "
                '        gstrSSQL = gstrSSQL & "TORIS_CODE_U = '" & Trim(strTORIS_CODE) & "' AND "
                '        gstrSSQL = gstrSSQL & "TORIF_CODE_U = '" & Trim(strTORIF_CODE) & "' AND "
                '        gstrSSQL = gstrSSQL & "FURI_DATE_U = '" & Trim(strFURIDATE) & "'"

                '        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
                '        gdbcCONNECT.Open()

                '        gdbCOMMAND = New OracleClient.OracleCommand
                '        gdbCOMMAND.CommandText = gstrSSQL
                '        gdbCOMMAND.Connection = gdbcCONNECT

                '        gdbrREADER = gdbCOMMAND.ExecuteReader

                '        While (gdbrREADER.Read)
                '            If GCom.NzStr(gdbrREADER.Item("FUNOU_FLG_U")) = "0" Then
                '                MessageBox.Show(String.Format(MSG0335W, GCom.NzStr(gdbrREADER.Item("TKIN_NO_U")), _
                '                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning))
                '                gdbcCONNECT.Close()
                '                txtTorisCode.Focus()
                '                Exit Sub
                '            End If
                '        End While
                '    End If
                'End If
                '2012/11/15 saitou 大垣信金 MODIFY --------------------------------------------------<<<<

                ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-12(RSV2対応) -------------------- START
                Dim CheckHenkanDataFlg As String = "0"
                '2016/11/28 saitou RSV2 UPD メンテナンス ---------------------------------------- START
                '依頼書全件、伝票全件指定時は、既存ファイルチェックを行わない
                If String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("111111111111") = False AndAlso String.Concat(strTORIS_CODE, strTORIF_CODE).Equals("222222222222") = False Then

                    If CheckHenkanData(MainDB, CheckHenkanDataFlg) = False Then
                        MessageBox.Show(String.Format(MSG0370W, "返還データ既存ファイルチェック"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "失敗", "返還データ既存ファイルチェック" & IniInfo.RSV2_MAKECHK)
                        Exit Try
                    Else
                        If CheckHenkanDataFlg = "1" Then
                            Select Case IniInfo.RSV2_MAKECHK
                                Case "1"
                                    If MessageBox.Show(MSG0081I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                                        MessageBox.Show(MSG0082I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                        BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "中断", "既存ファイルあり 区分" & IniInfo.RSV2_MAKECHK)
                                        Exit Try
                                    End If
                                Case "9"
                                    MessageBox.Show(MSG0374W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "中断", "既存ファイルあり 区分:" & IniInfo.RSV2_MAKECHK)
                                    Exit Try
                            End Select
                        End If
                    End If
                End If

                'If CheckHenkanData(MainDB, CheckHenkanDataFlg) = False Then
                '    MessageBox.Show(String.Format(MSG0370W, "返還データ既存ファイルチェック"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '    BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "失敗", "返還データ既存ファイルチェック" & IniInfo.RSV2_MAKECHK)
                '    Exit Try
                'Else
                '    If CheckHenkanDataFlg = "1" Then
                '        Select Case IniInfo.RSV2_MAKECHK
                '            Case "1"
                '                If MessageBox.Show(MSG0081I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                '                    MessageBox.Show(MSG0082I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                '                    BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "中断", "既存ファイルあり 区分" & IniInfo.RSV2_MAKECHK)
                '                    Exit Try
                '                End If
                '            Case "9"
                '                MessageBox.Show(MSG0374W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '                BLOG.Write_LEVEL1(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "中断", "既存ファイルあり 区分:" & IniInfo.RSV2_MAKECHK)
                '                Exit Try
                '        End Select
                '    End If
                'End If
                '2016/11/28 saitou RSV2 UPD ----------------------------------------------------- END
                ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-12(RSV2対応) -------------------- END

                '------------------------------
                '確認メッセージ
                '------------------------------
                If strTORIS_CODE & strTORIF_CODE <> "111111111111" And strTORIS_CODE & strTORIF_CODE <> "222222222222" And strHENKAN_FLG = "1" Then
                    If MessageBox.Show(MSG0046I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                        '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
                        'データベース統一
                        'gdbcCONNECT.Close()
                        '2012/11/15 saitou 大垣信金 MODIFY --------------------------------------------------<<<<
                        txtTorisCode.Focus()
                        Exit Sub
                    End If
                Else
                    If MessageBox.Show(String.Format(MSG0053I, _
                                                     strITAKU_NAME, _
                                                     strFURIDATE.Substring(0, 4) & "年" & strFURIDATE.Substring(4, 2) & "月" & strFURIDATE.Substring(6, 2) & "日", _
                                                     strKIGYO, _
                                                     strFURI, _
                                                     strITAKU_CODE, _
                                                     Format(System.DateTime.Today, "yyyy年MM月dd日")), _
                                                     msgTitle, _
                                                     MessageBoxButtons.YesNo, _
                                                     MessageBoxIcon.Question) = DialogResult.No Then

                        '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
                        'データベース統一
                        'gdbcCONNECT.Close()
                        '2012/11/15 saitou 大垣信金 MODIFY --------------------------------------------------<<<<
                        txtTorisCode.Focus()
                        Exit Sub
                    End If
                End If

                '------------------------------
                'JOBMASTに登録
                '------------------------------
                Dim strJOBID As String, strPARAMETA As String

                strJOBID = "J060"
                strPARAMETA = strTORIS_CODE & strTORIF_CODE & "," & strFURIDATE & "," & strMOTIKOMI_SEQ.PadLeft(2, "0"c)

                ' ジョブマスタ既存登録チェック関数を呼び出す
                '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
                'ソース改善(clsFUSIONの関数は使用しない)
                Dim iRet As Integer
                MainDB.BeginTrans()
                iRet = BLOG.SearchJOBMAST(strJOBID, strPARAMETA, MainDB)
                If iRet = 1 Then
                    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainDB.Rollback()
                    Return
                ElseIf iRet = -1 Then
                    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainDB.Rollback()
                    Return
                End If

                If BLOG.InsertJOBMAST(strJOBID, gstrUSER_ID, strPARAMETA, MainDB) = False Then
                    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BLOG.Write(strTORIS_CODE & strTORIF_CODE, strFURIDATE, "起動パラメータ登録", "失敗", "起動パラメータ登録に失敗しました")
                    MainDB.Rollback()
                    Return
                Else
                    MainDB.Commit()

                    MessageBox.Show(String.Format(MSG0021I, "返還データ作成"), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    BLOG.Write(strTORIS_CODE & strTORIF_CODE, strFURIDATE, "起動パラメータ登録", "成功", "起動パラメータを登録しました")
                End If

                'If clsFUSION.fn_JOBMAST_TOUROKU_CHECK(strJOBID, gstrUSER_ID, strPARAMETA) = False Then
                '    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '    gdbcCONNECT.Close()
                '    Exit Sub
                'End If

                'If clsFUSION.fn_INSERT_JOBMAST(strJOBID, gstrUSER_ID, strPARAMETA) = False Then
                '    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                '    BLOG.Write(strTORIS_CODE & strTORIF_CODE, strFURIDATE, "起動パラメータ登録", "失敗", "起動パラメータ登録に失敗しました")
                'Else
                '    MessageBox.Show(String.Format(MSG0021I, "返還データ作成処理"), _
                '                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                '    BLOG.Write(strTORIS_CODE & strTORIF_CODE, strFURIDATE, "起動パラメータ登録", "成功", "起動パラメータを登録しました")
                'End If
                'gdbcCONNECT.Close()
                '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>

                '2017/04/26 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ START
            Else
                '================================
                ' 伝送、WEB伝送全件処理
                '================================
                '--------------------------------
                'スケジュールチェック
                '--------------------------------
                SQL.Length = 0
                SQL.Append("select * from SCHMAST, TORIMAST")
                SQL.Append(" where TORIS_CODE_S = TORIS_CODE_T")
                SQL.Append(" and TORIF_CODE_S = TORIF_CODE_T")
                SQL.Append(" and FURI_DATE_S = " & SQ(strFURIDATE))
                SQL.Append(" and FUNOU_FLG_S = '1'")
                SQL.Append(" and TYUUDAN_FLG_S = '0'")
                SQL.Append(" and MOTIKOMI_KBN_T = '0'")     'センター直接持込をはずす
                Select Case strTORIS_CODE & strTORIF_CODE
                    Case "333333333333"             '伝送ベース全件指定時
                        SQL.Append(" and BAITAI_CODE_S = '00'")
                        '20170803 FJH)小嶋 ADD 標準版修正（結果返却要否：返却しないを除外）------start
                        SQL.Append(" and KEKKA_HENKYAKU_KBN_T <> '0'")
                        '20170803 FJH)小嶋 ADD 標準版修正（結果返却要否：返却しないを除外）------end
                    Case "444444444444"             'WEB伝送ベース全件指定時
                        SQL.Append(" and BAITAI_CODE_S = '10'")
                        '20170803 FJH)小嶋 ADD 標準版修正（結果返却要否：返却しないを除外）------start
                        SQL.Append(" and KEKKA_HENKYAKU_KBN_T <> '0'")
                        '20170803 FJH)小嶋 ADD 標準版修正（結果返却要否：返却しないを除外）------end
                End Select

                If oraReader.DataReader(SQL) = True Then
                    While Not oraReader.EOF
                        strTAKOU_KBN = oraReader.GetString("TAKO_KBN_T")
                        strHENKAN_FLG = oraReader.GetString("HENKAN_FLG_S")
                        strMOTIKOMI_SEQ = oraReader.GetString("MOTIKOMI_SEQ_S")
                        strITAKU_NAME = oraReader.GetString("ITAKU_NNAME_T")
                        strKIGYO = oraReader.GetString("KIGYO_CODE_T")
                        strFURI = oraReader.GetString("FURI_CODE_T")
                        strITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                        strKEKKA_HENKYAKU_KBN = oraReader.GetString("KEKKA_HENKYAKU_KBN_T")

                        '20170803 FJH)小嶋 DEL 標準版修正（結果返却要否：返却しないを除外）------start
                        'If strKEKKA_HENKYAKU_KBN = "0" Then
                        '    MessageBox.Show(MSG0316W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        '    oraReader.Close()
                        '    Me.txtTorisCode.Focus()
                        '    Return
                        'End If
                        '20170803 FJH)小嶋 DEL 標準版修正（結果返却要否：返却しないを除外）------end

                        '--------------------------------
                        '他行不能チェック
                        '--------------------------------
                        If strTAKOU_KBN = "1" Then

                            Dim TAKOU_TORIS_CODE As String = oraReader.GetString("TORIS_CODE_T")
                            Dim TAKOU_TORIF_CODE As String = oraReader.GetString("TORIF_CODE_T")

                            oraReader_TAKOUSCH = New MyOracleReader(MainDB)
                            Try
                                TAKOUSCH_SQL = New StringBuilder
                                TAKOUSCH_SQL.Append("select * from TAKOSCHMAST")
                                TAKOUSCH_SQL.Append(" where TORIS_CODE_U = " & SQ(TAKOU_TORIS_CODE))
                                TAKOUSCH_SQL.Append(" and TORIF_CODE_U = " & SQ(TAKOU_TORIF_CODE))
                                TAKOUSCH_SQL.Append(" and FURI_DATE_U = " & SQ(strFURIDATE))

                                If oraReader_TAKOUSCH.DataReader(TAKOUSCH_SQL) = True Then
                                    While oraReader_TAKOUSCH.EOF = False
                                        If oraReader_TAKOUSCH.GetString("FUNOU_FLG_U") = "0" Then
                                            MessageBox.Show(String.Format(MSG0379W, TAKOU_TORIS_CODE & "-" & TAKOU_TORIF_CODE, oraReader_TAKOUSCH.GetString("TKIN_NO_U")), _
                                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                            Me.txtTorisCode.Focus()
                                            Return
                                        End If
                                        oraReader_TAKOUSCH.NextRead()
                                    End While
                                End If
                            Catch ex As Exception
                                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
                                Return
                            Finally
                                If Not oraReader_TAKOUSCH Is Nothing Then
                                    oraReader_TAKOUSCH.Close()
                                    oraReader_TAKOUSCH = Nothing
                                End If
                            End Try
                        End If

                        oraReader.NextRead()
                    End While
                Else
                    MessageBox.Show(MSG0086W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Me.txtTorisCode.Focus()
                    Return
                End If
                oraReader.Close()

                '------------------------------
                '確認メッセージ
                '------------------------------
                Dim SYORI As String = ""
                Select Case strTORIS_CODE & strTORIF_CODE
                    Case "333333333333"             '伝送ベース全件指定時
                        SYORI = "伝送"
                    Case "444444444444"             'WEB伝送ベース全件指定時
                        SYORI = "WEB伝送"
                End Select

                If MessageBox.Show(String.Format(MSG0089I, SYORI, _
                                             strFURIDATE.Substring(0, 4) & "年" & strFURIDATE.Substring(4, 2) & "月" & strFURIDATE.Substring(6, 2) & "日", _
                                             Format(System.DateTime.Today, "yyyy年MM月dd日")), _
                                             msgTitle, _
                                             MessageBoxButtons.YesNo, _
                                             MessageBoxIcon.Question) = DialogResult.No Then

                    txtTorisCode.Focus()
                    Exit Sub
                End If

                '------------------------------
                'JOBMASTに登録
                '------------------------------
                Dim strITAKU_KANRI_CODE_bef As String = ""
                Dim strITAKU_KANRI_CODE_now As String = ""
                Dim strMULTI_KBN As String = ""

                Dim strJOBID As String = "J060"
                Dim strPARAMETA As String = ""

                Try
                    SQL.Length = 0
                    SQL.Append("SELECT * FROM SCHMAST,TORIMAST WHERE ")
                    SQL.Append("TORIS_CODE_S = TORIS_CODE_T AND ")
                    SQL.Append("TORIF_CODE_S = TORIF_CODE_T AND ")
                    SQL.Append("FURI_DATE_S = " & SQ(strFURIDATE) & " AND ")
                    SQL.Append("FUNOU_FLG_S = '1' AND ")
                    SQL.Append("TYUUDAN_FLG_S = '0' AND ")
                    SQL.Append("MOTIKOMI_KBN_T = '0' AND ")    'センター直接持込をはずす
                    Select Case strTORIS_CODE & strTORIF_CODE
                        Case "333333333333"             '伝送ベース全件指定時
                            SQL.Append("BAITAI_CODE_S = '00' ")
                            '20170803 FJH)小嶋 ADD 標準版修正（結果返却要否：返却しないを除外）------start
                            SQL.Append("AND KEKKA_HENKYAKU_KBN_T <> '0' ")
                            '20170803 FJH)小嶋 ADD 標準版修正（結果返却要否：返却しないを除外）------end
                        Case "444444444444"             'WEB伝送ベース全件指定時
                            SQL.Append("BAITAI_CODE_S = '10' ")
                            '20170803 FJH)小嶋 ADD 標準版修正（結果返却要否：返却しないを除外）------start
                            SQL.Append("AND KEKKA_HENKYAKU_KBN_T <> '0' ")
                            '20170803 FJH)小嶋 ADD 標準版修正（結果返却要否：返却しないを除外）------end
                    End Select
                    SQL.Append("ORDER BY ITAKU_KANRI_CODE_T,TORIS_CODE_S,TORIF_CODE_S")

                    '読込のみ
                    If oraReader.DataReader(SQL) Then

                        While Not oraReader.EOF
                            strTORIS_CODE = oraReader.GetString("TORIS_CODE_S")
                            strTORIF_CODE = oraReader.GetString("TORIF_CODE_S")
                            strMOTIKOMI_SEQ = oraReader.GetString("MOTIKOMI_SEQ_S")
                            strITAKU_KANRI_CODE_now = oraReader.GetString("ITAKU_KANRI_CODE_T")
                            strMULTI_KBN = oraReader.GetString("MULTI_KBN_T")

                            If strITAKU_KANRI_CODE_now <> strITAKU_KANRI_CODE_bef Or _
                               strMULTI_KBN = "0" Then
                                strPARAMETA = strTORIS_CODE & strTORIF_CODE & "," & strFURIDATE & "," & strMOTIKOMI_SEQ.PadLeft(2, "0"c)

                                ' ジョブマスタ既存登録チェック関数を呼び出す
                                Dim iRet As Integer
                                MainDB.BeginTrans()
                                iRet = BLOG.SearchJOBMAST(strJOBID, strPARAMETA, MainDB)
                                If iRet = 1 Then
                                    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    MainDB.Rollback()
                                    Return
                                ElseIf iRet = -1 Then
                                    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    MainDB.Rollback()
                                    Return
                                End If

                                If BLOG.InsertJOBMAST(strJOBID, gstrUSER_ID, strPARAMETA, MainDB) = False Then
                                    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    BLOG.Write(strTORIS_CODE & strTORIF_CODE, strFURIDATE, "起動パラメータ登録", "失敗", "起動パラメータ登録に失敗しました")
                                    MainDB.Rollback()
                                    Return
                                Else
                                    MainDB.Commit()
                                    BLOG.Write(strTORIS_CODE & strTORIF_CODE, strFURIDATE, "起動パラメータ登録", "成功", "起動パラメータを登録しました")
                                End If

                                strITAKU_KANRI_CODE_bef = oraReader.GetString("ITAKU_KANRI_CODE_T")

                            End If
                            oraReader.NextRead()
                        End While

                        MessageBox.Show(String.Format(MSG0021I, "返還データ作成処理"), _
                                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        BLOG.Write(strTORIS_CODE & strTORIF_CODE, strFURIDATE, "起動パラメータ登録", "成功", "起動パラメータを登録しました")
                    Else
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        BLOG.Write(strTORIS_CODE & strTORIF_CODE, strFURIDATE, "ジョブマスタ登録", "失敗", "ジョブマスタの登録に失敗しました")
                        Return
                    End If

                Catch ex As Exception
                    MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
                    Return
                End Try
            End If
            '2017/04/26 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)終了", "成功", "")
            '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
            'データベース統一
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            '2012/11/15 saitou 大垣信金 MODIFY --------------------------------------------------<<<<
        End Try

    End Sub
#End Region
#Region "参照ボタン"
    Private Sub btnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelect.Click

        Dim SQL As StringBuilder
        '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
        'データベース統一
        MainDB = New MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        'Dim OraReader As New CASTCommon.MyOracleReader
        '2012/11/15 saitou 大垣信金 MODIFY --------------------------------------------------<<<<
        Dim bSQL As Boolean = False
        Dim strFURI_DATE As String

        Try
            'テキストボックスの入力チェック
            If fn_check_text(0) = False Then
                Exit Sub
            End If

            If txtFuriDateY.Text <> "" AndAlso _
                           txtFuriDateM.Text <> "" AndAlso _
                           txtFuriDateD.Text <> "" Then
                strFURI_DATE = txtFuriDateY.Text & _
                               txtFuriDateM.Text & _
                               txtFuriDateD.Text
            Else
                strFURI_DATE = ""
            End If

            SQL = New StringBuilder(128)
            SQL.Append("SELECT ITAKU_NNAME_T")
            SQL.Append(", TORIS_CODE_S")
            SQL.Append(", TORIF_CODE_S")
            SQL.Append(", FURI_DATE_S")
            SQL.Append(", FURI_CODE_S")
            SQL.Append(", KIGYO_CODE_S")
            SQL.Append(", BAITAI_CODE_T")
            SQL.Append(", HENKAN_FLG_S")
            SQL.Append(", KEKKA_HENKYAKU_KBN_T")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(", TORIMAST")
            SQL.Append(" WHERE SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
            SQL.Append(" AND SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
            SQL.Append(" AND BAITAI_CODE_T <> '07'")    '2010/03/25 追加 学校以外
            If strFURI_DATE <> "" Then
                SQL.Append(" AND SCHMAST.FURI_DATE_S = '" & strFURI_DATE & "'")
            End If
            If txtTorisCode.Text <> "" Then
                If txtTorisCode.Text = "1111111111" AndAlso txtTorifCode.Text = "11" Then
                    SQL.Append(" AND TORIMAST.BAITAI_CODE_T = '04'")
                ElseIf txtTorisCode.Text = "2222222222" AndAlso txtTorifCode.Text = "22" Then
                    SQL.Append(" AND TORIMAST.BAITAI_CODE_T = '09'")
                    '2017/04/26 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ START
                ElseIf txtTorisCode.Text = "3333333333" AndAlso txtTorifCode.Text = "33" Then
                    SQL.Append(" AND TORIMAST.BAITAI_CODE_T = '00'")
                ElseIf txtTorisCode.Text = "4444444444" AndAlso txtTorifCode.Text = "44" AndAlso IniInfo.WEB_DENSO = "1" Then
                    SQL.Append(" AND TORIMAST.BAITAI_CODE_T = '10'")
                    '2017/04/26 タスク）西野 ADD 標準版修正（伝送、WEB伝送一括返却対応）------------------------------------ END
                Else
                    SQL.Append(" AND SCHMAST.TORIS_CODE_S = '" & txtTorisCode.Text & "'")
                    If txtTorifCode.Text <> "" Then
                        SQL.Append(" AND SCHMAST.TORIF_CODE_S = '" & txtTorifCode.Text & "'")
                    End If
                End If
            End If
            SQL.Append(" AND MOTIKOMI_KBN_T <> '1'")
            SQL.Append(" AND FUNOU_FLG_S = '1'")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            '2017/04/26 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ START
            If SORT_KEY = "" Then
                SQL.Append(" ORDER BY FURI_DATE_S ASC")
                SQL.Append(", SOUSIN_KBN_S ASC")
                SQL.Append(", TORIS_CODE_S ASC")
                SQL.Append(", TORIF_CODE_S ASC")
            Else
                SQL.Append(" ORDER BY " & SORT_KEY)
            End If
            'SQL.Append(" ORDER BY FURI_DATE_S ASC")
            'SQL.Append(", SOUSIN_KBN_S ASC")
            'SQL.Append(", TORIS_CODE_S ASC")
            'SQL.Append(", TORIF_CODE_S ASC")
            '2017/04/26 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ END

            bSQL = OraReader.DataReader(SQL)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面の表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim LineColor As Color
            Dim ROW As Integer = 1

            With Me.ListView1
                .Clear()
                .Columns.Add("", 0, HorizontalAlignment.Left)
                .Columns.Add("項番", 45, HorizontalAlignment.Right)
                .Columns.Add("取引先名", 220, HorizontalAlignment.Left)
                .Columns.Add("取引先コード", 115, HorizontalAlignment.Center)
                .Columns.Add("振替日", 95, HorizontalAlignment.Center)
                .Columns.Add("振替ｺｰﾄﾞ", 75, HorizontalAlignment.Center)
                .Columns.Add("企業ｺｰﾄﾞ", 75, HorizontalAlignment.Center)
                .Columns.Add("媒体", 65, HorizontalAlignment.Center)
                .Columns.Add("処理済", 70, HorizontalAlignment.Center)
            End With

            If bSQL = True Then
                Do
                    Dim Data(8) As String
                    Data(1) = CStr(ROW)                                     ' 項番
                    Data(2) = OraReader.GetString("ITAKU_NNAME_T").Trim     ' 取引先名
                    Data(3) = OraReader.GetString("TORIS_CODE_S") & "-" _
                            & OraReader.GetString("TORIF_CODE_S")           ' 取引先コード
                    Data(4) = OraReader.GetString("FURI_DATE_S")            ' 振替日
                    If Data(4).Length >= 8 Then
                        Data(4) = Data(4).Substring(0, 4) & "/" _
                                & Data(4).Substring(4, 2) & "/" _
                                & Data(4).Substring(6, 2)
                    End If
                    Data(5) = OraReader.GetString("FURI_CODE_S")            ' 振替コード
                    Data(6) = OraReader.GetString("KIGYO_CODE_S")           ' 企業コード
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    Data(7) = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"), _
                                                            OraReader.GetString("BAITAI_CODE_T"))
                    'Select Case OraReader.GetString("BAITAI_CODE_T")        ' 媒体コード
                    '    Case "00"
                    '        Data(7) = "伝送"        ' 0, 伝送
                    '    Case "01", "02"
                    '        Data(7) = "3.5FD"       ' 1, ＦＤ３．５
                    '    Case "04"
                    '        Data(7) = "依頼書"      ' 4, 依頼書
                    '    Case "05"
                    '        Data(7) = "MT"          ' 5, ＭＴ
                    '    Case "06"
                    '        Data(7) = "CMT"         ' 6, ＣＭＴ
                    '    Case "07"
                    '        Data(7) = "学校"        ' 7, 学校自振
                    '    Case "09"
                    '        Data(7) = "伝票"        ' 9, 伝票
                    '        '2012/06/30 標準版　WEB伝送対応
                    '    Case "10"
                    '        Data(7) = "WEB伝送"     '10, WEB伝送
                    '        '******20120710 mubuchi DVD追加対応*********
                    '    Case "11"
                    '        Data(7) = "DVD-RAM"     ' 11, DVD
                    '        '******20120710 mubuchi DVD追加対応*********
                    '    Case "12"
                    '        Data(7) = "その他"      ' 12, その他
                    '    Case "13"
                    '        Data(7) = "その他"      ' 13, その他
                    '    Case "14"
                    '        Data(7) = "その他"      ' 14, その他
                    '    Case "15"
                    '        Data(7) = "その他"      ' 15, その他
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END

                    Dim ForeColoe As Color = Color.Black
                    If OraReader.GetString("HENKAN_FLG_S") = "1" Then
                        Data(8) = "済"              ' 返還済み
                    Else
                        If OraReader.GetString("KEKKA_HENKYAKU_KBN_T") = "0" Then
                            Data(8) = "返却不要"
                        Else
                            Data(8) = ""
                        End If
                    End If

                    If ROW Mod 2 = 0 Then
                        LineColor = Color.White
                    Else
                        LineColor = Color.WhiteSmoke
                    End If

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    ROW += 1
                    OraReader.NextRead()
                Loop Until OraReader.EOF

            Else
                MessageBox.Show(MSG0086W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)

        Finally
            '2012/11/15 saitou 大垣信金 MODIFY -------------------------------------------------->>>>
            'データベース統一
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            '2012/11/15 saitou 大垣信金 MODIFY --------------------------------------------------<<<<
        End Try
    End Sub
#End Region
#Region "2008.02.14 追加機能 By Astar"
    '検証(非表示の項目も確認できるため)当該画面では今のところ非表示項目はない
    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Call GCom.MonitorCsvFile(CType(sender, ListView))
    End Sub

    '一覧表示領域のソート
    Private Sub ListView1_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick

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

    '取引先コードと振替日をテキストに設定
    Private Sub ListView1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.Click

        Dim Temp3 As String = GCom.SelectedItem(ListView1, 3).ToString.Trim
        Dim Temp4 As String = GCom.SelectedItem(ListView1, 4).ToString.Trim

        If Temp3.Length = 13 AndAlso Temp4.Length = 10 Then

            strTORIS_CODE = Temp3.Substring(0, 10)
            strTORIF_CODE = Temp3.Substring(11, 2)
            strFURIDATE = Temp4.Replace("/"c, "")

            With Me
                .txtTorisCode.Text = strTORIS_CODE
                .txtTorifCode.Text = strTORIF_CODE
                .txtFuriDateY.Text = strFURIDATE.Substring(0, 4)
                .txtFuriDateM.Text = strFURIDATE.Substring(4, 2)
                .txtFuriDateD.Text = strFURIDATE.Substring(6, 2)

                '.btnAction.Focus()
            End With
        End If
    End Sub

    'ZERO埋め
    Private Sub TEXTBOX_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtTorisCode.Validating, _
            txtTorifCode.Validating, _
            txtFuriDateY.Validating, _
            txtFuriDateM.Validating, _
            txtFuriDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub
#End Region

    '2013/12/24 saitou 標準版 性能向上 DEL -------------------------------------------------->>>>
    'チェック方法統一のためコメントアウト
    'Function fn_CHECK_NUM_MSG(ByVal objOBJ As String, ByVal strJNAME As String, ByVal gstrTITLE As String) As Boolean
    '    '============================================================================
    '    'NAME           :fn_CHECK_NUM_MSG
    '    'Parameter      :objOBJ：チェック対象オブジェクト／strJNAME：オブジェクト名称
    '    '               :gstrTITLE：タイトル
    '    'Description    :数値チェック
    '    'Return         :True=OK,False=NG
    '    'Create         :2004/05/28
    '    'Update         :
    '    '============================================================================
    '    fn_CHECK_NUM_MSG = False
    '    If Trim(objOBJ).Length = 0 Then
    '        MessageBox.Show(String.Format(MSG0285W, strJNAME), _
    '                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
    '        fn_CHECK_NUM_MSG = False
    '        Exit Function
    '    End If
    '    fn_CHECK_NUM_MSG = True
    'End Function
    '2013/12/24 saitou 標準版 性能向上 DEL --------------------------------------------------<<<<

End Class