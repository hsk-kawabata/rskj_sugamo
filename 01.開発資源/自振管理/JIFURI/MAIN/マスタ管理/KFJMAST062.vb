Imports System.Text
Imports System.Data.OracleClient
Imports CASTCommon


Public Class KFJMAST062

    '共通イベントコントロール
    Private CAST As New CASTCommon.Events

    '数値を許可する
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    '' 空白を許可しない
    Private CASTx1 As New CASTCommon.Events(" ", CASTCommon.Events.KeyMode.BAD)

    'ソートオーダーフラグ
    Dim SortOrderFlag As Boolean = True

    'クリックした列の番号
    Dim ClickedColumn As Integer

    Private MyOwnerForm As Form

    Private Const ThisFormName As String = "スケジュール管理"

    Private Const ThisModuleName As String = "KFJMAST062.vb"

    '*** 修正 mitsu 2008/06/02 マルチ先件数追加 ***
    Private MultiCnt As Integer
    '**********************************************

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAST062", "スケジュール管理画面")
    Private Const msgTitle As String = "スケジュール管理画面(KFJMAST062)"


    Private FmtComm As New CAstFormat.CFormat       '2011/05/20 追加
    Private SORT_KEY As String = ""

    '配列のインデックスを毎回採番し直すのが面倒なので列挙する
    Private Enum LstIdx As Integer
        No = 0
        ItakuName
        ToriCode
        ItakuCode
        BaitaiName
        FuriDate
        Uketuke
        Touroku
        Syougou
        Haisin
        Funou
        Henkan
        Kessai
        Tyuudan
        SyoriKen
        SyoriKin
        MotikomiSeq
        BaitaiCode
        CodeKbn
        FmtKbn
        LabelKbn
        MotikomiKbn
        KekkaHenkyakuKbn
        FileSeq
        Syubetu
        FSyoriKbn
        ItakuKanriCode
        FuriCode
        KigyoCode
        ErrorInf
        UketukeTimeStamp
        SyougouKbn
    End Enum

    '画面起動イベント処理
    Private Sub KFJMAST062_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        Try

            '-------------------------------------
            ' ログ情報設定
            '-------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            MyOwnerForm = GOwnerForm
            GOwnerForm = Me

            Me.CmdBack.DialogResult = DialogResult.None

            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            SORT_KEY = GCom.GetObjectParam("KFJMAST061", "SORT", "0")

            '項目名称定義
            With ListView1
                .Clear()
                .Columns.Add("項番", 40, HorizontalAlignment.Left)                  '0
                .Columns.Add("委託者名", 100, HorizontalAlignment.Left)             '1
                .Columns.Add("取引先コード", 90, HorizontalAlignment.Left)          '2
                .Columns.Add("委託者コード", 85, HorizontalAlignment.Left)          '3
                .Columns.Add("媒体", 40, HorizontalAlignment.Left)                  '4
                .Columns.Add("振替日", 80, HorizontalAlignment.Left)                '5
                .Columns.Add("受付", 40, HorizontalAlignment.Center)                '6
                .Columns.Add("落込", 40, HorizontalAlignment.Center)                '7
                .Columns.Add("照合", 40, HorizontalAlignment.Center)                '8
                .Columns.Add("配信", 40, HorizontalAlignment.Center)                '9
                .Columns.Add("不能", 40, HorizontalAlignment.Center)                '10
                .Columns.Add("返還", 40, HorizontalAlignment.Center)                '11
                .Columns.Add("決済", 40, HorizontalAlignment.Center)                '12
                .Columns.Add("中断", 40, HorizontalAlignment.Center)                '13
                .Columns.Add("依頼件数", 70, HorizontalAlignment.Right)             '14
                .Columns.Add("依頼金額", 120, HorizontalAlignment.Right)            '15
                .Columns.Add("持込SEQ", 0, HorizontalAlignment.Right)               '16
                .Columns.Add("媒体コード", 0, HorizontalAlignment.Right)            '17
                .Columns.Add("コード区分", 0, HorizontalAlignment.Right)            '18
                .Columns.Add("フォーマット区分", 0, HorizontalAlignment.Right)      '19
                .Columns.Add("ラベル区分", 0, HorizontalAlignment.Right)            '20
                .Columns.Add("持込区分", 0, HorizontalAlignment.Right)              '21
                .Columns.Add("結果返却要否区分", 0, HorizontalAlignment.Right)      '22
                .Columns.Add("ファイルSEQ", 0, HorizontalAlignment.Right)           '23
                .Columns.Add("種別", 0, HorizontalAlignment.Right)                  '24
                .Columns.Add("振替処理区分", 0, HorizontalAlignment.Right)          '25
                .Columns.Add("代表委託者コード", 0, HorizontalAlignment.Right)      '26
                .Columns.Add("振替コード", 0, HorizontalAlignment.Right)            '27
                .Columns.Add("企業コード", 0, HorizontalAlignment.Right)            '28
                .Columns.Add("エラーコード", 0, HorizontalAlignment.Right)          '29
                .Columns.Add("受付日時", 140, HorizontalAlignment.Center)           '30
                .Columns.Add("照合区分", 0, HorizontalAlignment.Right)              '31
            End With

            If ListView1.Items.Count > 0 Then
                ListView1.Items(0).Selected = True
            End If

            '標準日付を算出する
            Dim StartDate As Date
            Dim FinalDate As Date
            'オラクルコネクションをフォーマットクラスに渡して休日リストの作成を行う
            Dim MainDB As CASTCommon.MyOracle = Nothing
            Try
                MainDB = New CASTCommon.MyOracle
                Me.FmtComm.Oracle = MainDB

            Catch ex As Exception

            Finally
                If Not MainDB Is Nothing Then
                    MainDB.Close()
                    MainDB = Nothing
                End If
            End Try

            Dim intFrom As Integer = 0
            Dim intTo As Integer = 0
            Call GCom.GetFromTo("KFJMAST061", intFrom, intTo, 0, 3)

            For Index As Integer = 0 To 1 Step 1
                Select Case Index
                    Case 0 : StartDate = CASTCommon.GetEigyobi(Date.Now, intFrom, FmtComm.HolidayList)
                    Case 1 : FinalDate = CASTCommon.GetEigyobi(Date.Now, intTo, FmtComm.HolidayList)
                End Select
            Next

            With Me
                .TxtYear1.Text = StartDate.Year.ToString.PadLeft(4, "0"c)
                .TxtYear2.Text = FinalDate.Year.ToString.PadLeft(4, "0"c)
                .TxtMonth1.Text = StartDate.Month.ToString.PadLeft(2, "0"c)
                .TxtMonth2.Text = FinalDate.Month.ToString.PadLeft(2, "0"c)
                .TxtDay1.Text = StartDate.Day.ToString.PadLeft(2, "0"c)
                .TxtDay2.Text = FinalDate.Day.ToString.PadLeft(2, "0"c)
            End With

            Me.TxtYear1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    '画面表示(再表示)イベント処理
    Private Sub KFJMAST062_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        GCom.GLog.Job1 = ThisFormName
    End Sub

    '画面終了時処理
    Private Sub KFJMAST062_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        Try
            GOwnerForm = MyOwnerForm
            GOwnerForm.Visible = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        End Try
    End Sub

    '参照ボタン処理
    Private Sub CmdSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CmdSelect.Click

        Dim MSG As String
        Dim FURI_DATE(1) As Date
        Dim TORIS_CODE As String = ""
        Dim TORIF_CODE As String = ""
        Dim BAITAI_CODE As String = ""
        Dim REC As OracleDataReader = Nothing

        Dim sql As New StringBuilder(128)
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            '最初に表示領域をクリアー
            ListView1.Items.Clear()

            '事前チェック


            '日付情報チェック
            Dim TextObject As TextBox
            TextObject = CheckDateOfFromTo(FURI_DATE)
            If Not TextObject Is Nothing Then

                MSG = MSG0281W.Replace("{0}", "対象日")
                Call MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TextObject.Focus()
                Return
            End If

            Me.SuspendLayout()

            sql.Append("SELECT FSYORI_KBN_S")
            sql.Append(", TORIS_CODE_S")
            sql.Append(", TORIF_CODE_S")
            sql.Append(", ITAKU_KANRI_CODE_T")
            sql.Append(", ITAKU_CODE_S")
            sql.Append(", ITAKU_NNAME_T")
            sql.Append(", BAITAI_CODE_S")
            sql.Append(", FURI_DATE_S")
            sql.Append(", SYORI_KEN_S")
            sql.Append(", SYORI_KIN_S")
            sql.Append(", TOUROKU_FLG_S")
            sql.Append(", TAKOU_FLG_S")
            sql.Append(", HAISIN_FLG_S")
            sql.Append(", FUNOU_FLG_S")
            sql.Append(", HENKAN_FLG_S")
            sql.Append(", KESSAI_FLG_S")
            sql.Append(", TYUUDAN_FLG_S")
            sql.Append(", MOTIKOMI_SEQ_S")
            sql.Append(", CODE_KBN_T")
            sql.Append(", FMT_KBN_T")
            sql.Append(", LABEL_KBN_T")
            sql.Append(", MOTIKOMI_KBN_T")
            sql.Append(", KEKKA_HENKYAKU_KBN_T")
            sql.Append(", FILE_SEQ_S")
            sql.Append(", SYUBETU_T")
            sql.Append(", ERROR_INF_S")
            sql.Append(", UKETUKE_FLG_S")
            sql.Append(", FURI_CODE_T")
            sql.Append(", KIGYO_CODE_T")
            sql.Append(", SYOUGOU_KBN_T")
            sql.Append(", SYOUGOU_FLG_S")
            sql.Append(", UKETUKE_TIME_STAMP_S")
            sql.Append("  FROM SCHMAST")
            sql.Append(", SCHMAST_SUB")
            sql.Append(", (SELECT FSYORI_KBN_T")
            sql.Append(", TORIS_CODE_T")
            sql.Append(", TORIF_CODE_T")
            sql.Append(", ITAKU_KANRI_CODE_T")
            sql.Append(", ITAKU_NNAME_T")
            sql.Append(", CODE_KBN_T")
            sql.Append(", FMT_KBN_T")
            sql.Append(", LABEL_KBN_T")
            sql.Append(", KEKKA_HENKYAKU_KBN_T")
            sql.Append(", MOTIKOMI_KBN_T")
            sql.Append(", SYUBETU_T")
            sql.Append(", FURI_CODE_T")
            sql.Append(", KIGYO_CODE_T")
            sql.Append(", SYOUGOU_KBN_T")
            sql.Append(" FROM TORIMAST_VIEW")
            sql.Append(" WHERE FSYORI_KBN_T = '1'")
            sql.Append(")")
            sql.Append(" WHERE FSYORI_KBN_S = FSYORI_KBN_T")
            sql.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            sql.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            sql.Append(" AND FSYORI_KBN_S = FSYORI_KBN_SSUB")
            sql.Append(" AND TORIS_CODE_S = TORIS_CODE_SSUB")
            sql.Append(" AND TORIF_CODE_S = TORIF_CODE_SSUB")
            sql.Append(" AND FURI_DATE_S  = FURI_DATE_SSUB")

            '振替日範囲(必須)
            sql.Append(" AND FURI_DATE_S BETWEEN '" & String.Format("{0:yyyyMMdd}", FURI_DATE(0)) & "'")
            sql.Append(" AND '" & String.Format("{0:yyyyMMdd}", FURI_DATE(1)) & "'")
            If SORT_KEY = "" Then
                sql.Append(" ORDER BY TORIS_CODE_S ASC")
                sql.Append(", TORIF_CODE_S ASC")
                sql.Append(", FURI_DATE_S ASC")
            Else
                sql.Append(" ORDER BY " & SORT_KEY)
            End If

            If GCom.SetDynaset(sql.ToString, REC) Then

                Dim ROW As Integer = 0

                Do While REC.Read

                    '列挙体の項目数から配列数を決める
                    Dim i As Integer = System.Enum.GetNames(GetType(LstIdx)).Length
                    Dim Data(i) As String

                    Data(LstIdx.No) = CType(ROW + 1, String).PadLeft(4, "0"c)
                    Data(LstIdx.ItakuName) = GCom.NzStr(REC.Item("ITAKU_NNAME_T")).Trim        '委託者名
                    Data(LstIdx.ToriCode) = GCom.NzDec(REC.Item("TORIS_CODE_S"), "") & "-" & GCom.NzDec(REC.Item("TORIF_CODE_S"), "")   '取引先コード
                    Data(LstIdx.ItakuCode) = GCom.NzDec(REC.Item("ITAKU_CODE_S"), "")          '委託者コード

                    '媒体名をテキストから取得する
                    Data(LstIdx.BaitaiName) = GetText_CodeToName(IO.Path.Combine(GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"),
                                                            GCom.NzDec(REC.Item("BAITAI_CODE_S"), ""))

                    With GCom.NzDec(REC.Item("FURI_DATE_S"), "")
                        Data(LstIdx.FuriDate) = .Substring(0, 4) & "/" & .Substring(4, 2) & "/" & .Substring(6, 2)           '振替日
                    End With

                    '受付
                    If GCom.NzInt(REC.Item("UKETUKE_FLG_S"), 0) = 1 Then
                        Data(LstIdx.Uketuke) = "○"
                    Else
                        Data(LstIdx.Uketuke) = "×"
                    End If

                    '落込
                    If GCom.NzInt(REC.Item("TOUROKU_FLG_S"), 0) = 1 Then
                        Data(LstIdx.Touroku) = "○"
                    Else
                        Data(LstIdx.Touroku) = "×"
                    End If

                    '照合
                    If GCom.NzInt(REC.Item("SYOUGOU_FLG_S"), 0) = 1 Then
                        Data(LstIdx.Syougou) = "○"
                    Else
                        If GCom.NzInt(REC.Item("SYOUGOU_KBN_T"), 0) = 1 Then
                            Data(LstIdx.Syougou) = "×"
                        Else
                            Data(LstIdx.Syougou) = "-"
                        End If
                    End If

                    '配信
                    If GCom.NzInt(REC.Item("HAISIN_FLG_S"), 0) = 1 Then
                        Data(LstIdx.Haisin) = "○"
                    Else
                        Data(LstIdx.Haisin) = "×"
                    End If

                    '更新
                    If GCom.NzInt(REC.Item("FUNOU_FLG_S"), 0) = 1 Then
                        Data(LstIdx.Funou) = "○"
                    Else
                        Data(LstIdx.Funou) = "×"
                    End If

                    '返還
                    If GCom.NzInt(REC.Item("HENKAN_FLG_S"), 0) = 1 Then
                        Data(LstIdx.Henkan) = "○"
                    Else
                        Data(LstIdx.Henkan) = "×"
                    End If

                    '決済
                    If GCom.NzInt(REC.Item("KESSAI_FLG_S"), 0) = 1 Then
                        Data(LstIdx.Kessai) = "○"
                    Else
                        Data(LstIdx.Kessai) = "×"
                    End If

                    '中断
                    If GCom.NzInt(REC.Item("TYUUDAN_FLG_S"), 0) = 1 Then
                        Data(LstIdx.Tyuudan) = "○"
                    Else
                        Data(LstIdx.Tyuudan) = "×"
                    End If

                    Data(LstIdx.SyoriKen) = GCom.NzLong(REC.Item("SYORI_KEN_S")).ToString("#,###")      '依頼件数
                    Data(LstIdx.SyoriKin) = GCom.NzLong(REC.Item("SYORI_KIN_S")).ToString("#,###")      '依頼金額
                    Data(LstIdx.MotikomiSeq) = GCom.NzDec(REC.Item("MOTIKOMI_SEQ_S"), "")               '持込ＳＥＱ
                    Data(LstIdx.BaitaiCode) = GCom.NzDec(REC.Item("BAITAI_CODE_S"), "")                 '媒体コード
                    Data(LstIdx.CodeKbn) = GCom.NzDec(REC.Item("CODE_KBN_T"), "")                       'コード区分
                    Data(LstIdx.FmtKbn) = GCom.NzDec(REC.Item("FMT_KBN_T"), "")                         'フォーマット区分
                    Data(LstIdx.LabelKbn) = GCom.NzDec(REC.Item("LABEL_KBN_T"), "")                     'ラベル区分
                    Data(LstIdx.MotikomiKbn) = GCom.NzDec(REC.Item("MOTIKOMI_KBN_T"), "")               '持込区分
                    Data(LstIdx.KekkaHenkyakuKbn) = GCom.NzDec(REC.Item("KEKKA_HENKYAKU_KBN_T"), "")    '結果返却要否
                    Data(LstIdx.FileSeq) = GCom.NzDec(REC.Item("FILE_SEQ_S"), "")                       'ファイルＳＥＱ
                    Data(LstIdx.Syubetu) = GCom.NzDec(REC.Item("SYUBETU_T"), "")                        '種別
                    Data(LstIdx.FSyoriKbn) = GCom.NzDec(REC.Item("FSYORI_KBN_S"), "")                   '振替処理区分
                    Data(LstIdx.ItakuKanriCode) = GCom.NzDec(REC.Item("ITAKU_KANRI_CODE_T"), "")        '代表委託者コード
                    Data(LstIdx.FuriCode) = GCom.NzDec(REC.Item("FURI_CODE_T"), "")                     '振替コード
                    Data(LstIdx.KigyoCode) = GCom.NzDec(REC.Item("KIGYO_CODE_T"), "")                   '企業コード
                    Data(LstIdx.ErrorInf) = GCom.NzDec(REC.Item("ERROR_INF_S"), "")                     'エラーコード

                    '受付タイムスタンプ
                    With GCom.NzDec(REC.Item("UKETUKE_TIME_STAMP_S"), "")
                        Data(LstIdx.UketukeTimeStamp) = String.Format("{0}/{1}/{2} {3}:{4}:{5}", .Substring(0, 4), .Substring(4, 2), .Substring(6, 2), .Substring(8, 2), .Substring(10, 2), .Substring(12, 2))
                    End With
                    Data(LstIdx.SyougouKbn) = GCom.NzDec(REC.Item("SYOUGOU_KBN_T"), "")                 '照合区分

                    Dim LineColor As Color

                    If ROW Mod 2 = 0 Then
                        LineColor = Color.White
                    Else
                        LineColor = Color.WhiteSmoke
                    End If

                    Dim FontColor As Color
                    If Data(LstIdx.ErrorInf) = "020" Then
                        FontColor = Color.Red
                    Else
                        FontColor = Color.Black
                    End If

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    ROW += 1
                Loop
            Else
                'スケジュールなし
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TxtYear1.Focus()
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)
        Finally
            If ListView1.Items.Count > 0 Then
                ListView1.Items(0).Selected = True
            End If
            Me.ResumeLayout()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
        End Try
    End Sub


    '
    ' 機　能 : 日付評価
    '
    ' 戻り値 : 評価OK = Nothing
    ' 　　　   評価NG = TextBox Control Object
    '
    ' 引き数 : ARG1 - なし
    '
    ' 備　考 : なし
    '    
    Private Function CheckDateOfFromTo(ByRef onDate() As Date) As TextBox
        Dim Ret As Integer
        Dim onText(2) As Integer
        Dim ObjReturn As TextBox = Nothing
        Try
            '期間開始日チェック
            onText(0) = GCom.NzInt(TxtYear1.Text)
            onText(1) = GCom.NzInt(TxtMonth1.Text)
            onText(2) = GCom.NzInt(TxtDay1.Text)

            Ret = GCom.SET_DATE(onDate(0), onText)
            If Not Ret = -1 Then
                Select Case Ret
                    Case 1 : ObjReturn = TxtMonth1
                    Case 2 : ObjReturn = TxtDay1
                    Case Else : ObjReturn = TxtYear1
                End Select
                Exit Try
            End If

            '期間終了日チェック
            onText(0) = GCom.NzInt(TxtYear2.Text)
            onText(1) = GCom.NzInt(TxtMonth2.Text)
            onText(2) = GCom.NzInt(TxtDay2.Text)

            Ret = GCom.SET_DATE(onDate(1), onText)
            If Not Ret = -1 Then
                Select Case Ret
                    Case 1 : ObjReturn = TxtMonth2
                    Case 2 : ObjReturn = TxtDay2
                    Case Else : ObjReturn = TxtYear2
                End Select
                Exit Try
            End If

            '相関チェック(逆転エラーの場合には入れ替える)
            If onDate(0) > onDate(1) Then
                TxtYear1.Text = String.Format("{0:0000}", onDate(1).Year)
                TxtYear2.Text = String.Format("{0:0000}", onDate(0).Year)
                TxtMonth1.Text = String.Format("{0:00}", onDate(1).Month)
                TxtMonth2.Text = String.Format("{0:00}", onDate(0).Month)
                TxtDay1.Text = String.Format("{0:00}", onDate(1).Day)
                TxtDay2.Text = String.Format("{0:00}", onDate(0).Day)
                onText(0) = GCom.NzInt(TxtYear1.Text)
                onText(1) = GCom.NzInt(TxtMonth1.Text)
                onText(2) = GCom.NzInt(TxtDay1.Text)
                GCom.SET_DATE(onDate(0), onText)
                onText(0) = GCom.NzInt(TxtYear2.Text)
                onText(1) = GCom.NzInt(TxtMonth2.Text)
                onText(2) = GCom.NzInt(TxtDay2.Text)
                GCom.SET_DATE(onDate(1), onText)
            Else
                TxtYear1.Text = String.Format("{0:0000}", onDate(0).Year)
                TxtYear2.Text = String.Format("{0:0000}", onDate(1).Year)
                TxtMonth1.Text = String.Format("{0:00}", onDate(0).Month)
                TxtMonth2.Text = String.Format("{0:00}", onDate(1).Month)
                TxtDay1.Text = String.Format("{0:00}", onDate(0).Day)
                TxtDay2.Text = String.Format("{0:00}", onDate(1).Day)
            End If
        Catch ex As Exception
            ObjReturn = TxtMonth1
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)
        End Try

        Return ObjReturn

    End Function

    '戻るボタン処理
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CmdBack.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")

            Me.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try

    End Sub

    '検証用CSV出力&表示
    Private Sub LetMonitor_ListView(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles ListView1.DoubleClick

        Call GCom.MonitorCsvFile(ListView1)
    End Sub

    '一覧表示領域のソート
    Private Sub LetSort_ListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
    Handles ListView1.ColumnClick

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
            Throw
        End Try

    End Sub

    '日付指定領域のチェック処理
    Private Sub TxtDate_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles TxtYear1.Validating, TxtYear2.Validating, _
            TxtMonth1.Validating, TxtMonth2.Validating, _
            TxtDay1.Validating, TxtDay2.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            Throw
        End Try

    End Sub

    'スケジュール進捗管理表印刷ボタン処理
    Private Sub CmdSchPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdSchPrint.Click

        Dim MSG As String
        Dim DRet As DialogResult
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items
        Dim CreateCSV As New KFJP027
        Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")
        Dim SyoriTime As String = System.DateTime.Now.ToString("HHmmss")

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            Dim startFuriDate As String = TxtYear1.Text & TxtMonth1.Text & TxtDay1.Text
            Dim endFuriDate As String = TxtYear2.Text & TxtMonth2.Text & TxtDay2.Text

            If ListView1.Items.Count <= 0 Then

                MSG = MSG0106W
                Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            DRet = MessageBox.Show(MSG0013I.Replace("{0}", "スケジュール進捗管理表"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)

            If DRet <> DialogResult.OK Then Return

            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName
            For Each item As ListViewItem In nItems

                CreateCSV.OutputCsvData(item.SubItems(LstIdx.No).Text, True)                                            '項番
                CreateCSV.OutputCsvData(SyoriDate, True)                                                                '処理日
                CreateCSV.OutputCsvData(SyoriTime, True)                                                                'タイムスタンプ
                CreateCSV.OutputCsvData(startFuriDate, True)                                                            '開始振替日
                CreateCSV.OutputCsvData(endFuriDate, True)                                                              '終了振替日
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.ToriCode).Text.Replace("-", "").Substring(0, 10), True)    '取引先主コード
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.ToriCode).Text.Replace("-", "").Substring(10, 2), True)    '取引先副コード
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.ItakuName).Text, True)                                     '取引先名（漢字）
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.ItakuCode).Text, True)                                     '委託者コード
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.BaitaiName).Text, True)                                    '媒体
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.FuriDate).Text.Replace("/", ""), True)                     '振替日
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.Touroku).Text.Replace("/", ""), True)                      '登録
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.Haisin).Text.Replace("/", ""), True)                       '配信
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.Funou).Text.Replace("/", ""), True)                        '不能
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.Henkan).Text.Replace("/", ""), True)                       '返還
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.Kessai).Text.Replace("/", ""), True)                       '決済
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.SyoriKen).Text.Replace(",", ""))                           '処理件数
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.SyoriKin).Text.Replace(",", ""))                           '処理金額
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.FuriCode).Text, True)                                      '振替コード
                CreateCSV.OutputCsvData(item.SubItems(LstIdx.KigyoCode).Text, True, True)                               '企業コード

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
            iRet = ExeRepo.ExecReport("KFJP027.EXE", param)

            If iRet <> 0 Then
                '印刷失敗：戻り値に対応したエラーメッセージを表示する
                Select Case iRet
                    Case -1
                        MessageBox.Show(MSG0226W.Replace("{0}", "スケジュール進捗管理表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Case Else
                        MessageBox.Show(MSG0004E.Replace("{0}", "スケジュール進捗管理表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select


                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュール進捗管理表印刷", "失敗", "エラーコード：" & iRet)
                Return
            Else
                MessageBox.Show(String.Format(MSG0014I, "スケジュール進捗管理表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try
    End Sub

    '依頼データ取消ボタン処理
    Private Sub CmdUkeDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CmdUkeDelete.Click
        Dim Ret As Integer
        Dim MSG As String
        Dim SQLCode As Integer
        Dim TransStatus As Boolean = False
        Dim CreateCSV As New KFJP026
        Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")
        Dim SyoriTime As String = System.DateTime.Now.ToString("HHmmss")

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(落込取消)開始", "成功", "")

            If ListView1.Items.Count <= 0 Then
                MSG = MSG0224W
                Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    MSG = MSG0100W
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            Dim BAITAI_CODE As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.BaitaiCode), "")
            If GCom.NzInt(BAITAI_CODE) = 7 Then
                MSG = MSG0315W
                Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            '2018/05/25 タスク）西野 CHG 標準版修正：広島信金対応（未照合分の落込取消対応）------------------------ START
            '照合要否区分で照合要の場合、落込済でも取消ができないのを修正。
            '依頼書、伝票の場合は金額入力した時点で受付フラグが○になるので、この2つの媒体は
            '登録フラグをチェックするように処理を修正。
            Select Case BAITAI_CODE
                Case "04", "09"
                    If GCom.NzStr(GCom.SelectedItem(ListView1, LstIdx.Touroku)) <> "○" Then
                        Call MessageBox.Show(MSG0101W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                Case Else
                    '照合要否区分が照合否でも、金融機関名相違等のエラーで、
                    '受付フラグは○だが、登録フラグは×の状態になってしまい、取消できなくなってしまうので、
                    '照合要否区分に依らず、受付フラグをチェックするように処理を変更。
                    If GCom.NzStr(GCom.SelectedItem(ListView1, LstIdx.Uketuke)) <> "○" Then
                        Call MessageBox.Show(MSG0101W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
            End Select
            'If GCom.NzStr(GCom.SelectedItem(ListView1, 6)) <> "○" Then
            '    MSG = MSG0101W
            '    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Return
            'End If
            '2018/05/25 タスク）西野 CHG 標準版修正：広島信金対応（未照合分の落込取消対応）------------------------ END

            Dim memFURI_DATE As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.FuriDate), "")
            Dim memMOTIKOMI_SEQ As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.MotikomiSeq), "")
            Dim memITAKU_KANRI_CODE As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.ItakuKanriCode), "")

            'マルチの明細内に取り消しできないものがあるか否かチェックする
            If CheckMeisaiDelete(memFURI_DATE, memMOTIKOMI_SEQ) = DialogResult.Cancel Then
                MSG = MSG0101W
                Call MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            MSG = MSG0045I
            Dim StrData(8) As String
            If Not CheckMessage(MSG, StrData, MessageBoxDefaultButton.Button2) = DialogResult.OK Then
                Return
            End If

            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName

            For Each Item As ListViewItem In ListView1.Items

                Dim FURI_DATE As String = GCom.NzDec(Item.SubItems(LstIdx.FuriDate), "")
                Dim MOTIKOMI_SEQ As String = GCom.NzDec(Item.SubItems(LstIdx.MotikomiSeq), "")
                Dim ITAKU_KANRI_CODE As String = GCom.NzDec(Item.SubItems(LstIdx.ItakuKanriCode), "")

                '2018/05/25 タスク）西野 ADD 標準版修正：広島信金対応（未照合分の落込取消対応）------------------------ START
                Dim CheckFlg As String = String.Empty
                Select Case Item.SubItems(LstIdx.BaitaiCode).Text.Trim
                    Case "04", "09"
                        CheckFlg = Item.SubItems(LstIdx.Touroku).Text.Trim
                    Case Else
                        CheckFlg = Item.SubItems(LstIdx.Uketuke).Text.Trim
                End Select
                '2018/05/25 タスク）西野 ADD 標準版修正：広島信金対応（未照合分の落込取消対応）------------------------ END

                '2018/05/25 タスク）西野 CHG 標準版修正：広島信金対応（未照合分の落込取消対応）------------------------ START
                If FURI_DATE = memFURI_DATE AndAlso MOTIKOMI_SEQ = memMOTIKOMI_SEQ AndAlso ITAKU_KANRI_CODE = memITAKU_KANRI_CODE AndAlso CheckFlg = "○" Then
                    'If FURI_DATE = memFURI_DATE AndAlso MOTIKOMI_SEQ = memMOTIKOMI_SEQ AndAlso ITAKU_KANRI_CODE = memITAKU_KANRI_CODE AndAlso Item.SubItems(6).Text = "○" Then
                    '2018/05/25 タスク）西野 CHG 標準版修正：広島信金対応（未照合分の落込取消対応）------------------------ END

                    If TransStatus = False Then
                        TransStatus = True
                        Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Begin)
                    End If

                    Dim FSYORI_KBN As String = GCom.NzDec(Item.SubItems(LstIdx.FSyoriKbn), "")
                    Dim TORIS_CODE As String = GCom.NzDec(Item.SubItems(LstIdx.ToriCode).Text.Replace("-", "").Substring(0, 10), "")
                    Dim TORIF_CODE As String = GCom.NzDec(Item.SubItems(LstIdx.ToriCode).Text.Replace("-", "").Substring(10, 2), "")
                    Dim FILE_SEQ As String = GCom.NzDec(Item.SubItems(LstIdx.FileSeq), "")
                    Dim SYUBETU_CODE As String = GCom.NzDec(Item.SubItems(LstIdx.Syubetu), "")
                    Dim KEN As String = GCom.NzDec(Item.SubItems(LstIdx.SyoriKen), "")
                    Dim KIN As String = GCom.NzDec(Item.SubItems(LstIdx.SyoriKin), "")

                    LW.ToriCode = TORIS_CODE & "-" & TORIF_CODE
                    LW.FuriDate = FURI_DATE


                    '###処理結果確認表（落込取消）の印刷
                    CreateCSV.OutputCsvData(SyoriDate, True)                                        '処理日
                    CreateCSV.OutputCsvData(SyoriTime, True)                                        'タイムスタンプ
                    CreateCSV.OutputCsvData(FSYORI_KBN, True)                                       '振替処理区分
                    CreateCSV.OutputCsvData(TORIS_CODE, True)                                       '取引先主コード
                    CreateCSV.OutputCsvData(TORIF_CODE, True)                                       '取引先副コード
                    CreateCSV.OutputCsvData(Item.SubItems(LstIdx.ItakuName).Text, True)             '取引先名（漢字）
                    CreateCSV.OutputCsvData(Item.SubItems(LstIdx.ItakuCode).Text, True)             '委託者コード
                    CreateCSV.OutputCsvData(FURI_DATE, True)                                        '振替日
                    CreateCSV.OutputCsvData(Item.SubItems(LstIdx.SyoriKen).Text.Replace(",", ""))   '処理件数
                    CreateCSV.OutputCsvData(Item.SubItems(LstIdx.SyoriKin).Text.Replace(",", ""))   '処理金額
                    CreateCSV.OutputCsvData(MOTIKOMI_SEQ, True)                                     '持込ＳＥＱ
                    CreateCSV.OutputCsvData(SYUBETU_CODE, True)                                     '種別
                    CreateCSV.OutputCsvData(Item.SubItems(LstIdx.FuriCode).Text, True)              '振替コード
                    CreateCSV.OutputCsvData(Item.SubItems(LstIdx.KigyoCode).Text, True, True)       '企業コード

                    Dim SQL As String

                    SQL = "UPDATE SCHMAST"
                    SQL &= " SET UKETUKE_FLG_S = '0'"
                    SQL &= ", TOUROKU_FLG_S = '0'"
                    SQL &= ", SYORI_KEN_S = 0"
                    SQL &= ", SYORI_KIN_S = 0"
                    SQL &= ", ERR_KEN_S = 0"
                    SQL &= ", ERR_KIN_S = 0"
                    SQL &= ", TESUU_KIN_S = 0"
                    SQL &= ", TESUU_KIN1_S = 0"
                    SQL &= ", TESUU_KIN2_S = 0"
                    SQL &= ", TESUU_KIN3_S = 0"
                    SQL &= ", FURI_KEN_S = 0"
                    SQL &= ", FURI_KIN_S = 0"
                    SQL &= ", FUNOU_KEN_S = 0"
                    SQL &= ", FUNOU_KIN_S = 0"
                    SQL &= ", UKETUKE_DATE_S = '" & New String("0"c, 8) & "'"
                    SQL &= ", TOUROKU_DATE_S = '" & New String("0"c, 8) & "'"
                    SQL &= ", UFILE_NAME_S = NULL"
                    SQL &= ", ERROR_INF_S = NULL"
                    SQL &= " WHERE FSYORI_KBN_S = '" & FSYORI_KBN & "'"
                    SQL &= " AND TORIS_CODE_S = '" & TORIS_CODE & "'"
                    SQL &= " AND TORIF_CODE_S = '" & TORIF_CODE & "'"
                    SQL &= " AND FURI_DATE_S = '" & FURI_DATE & "'"
                    SQL &= " AND MOTIKOMI_SEQ_S = '" & MOTIKOMI_SEQ & "'"
                    SQL &= " AND FILE_SEQ_S = '" & FILE_SEQ & "'"

                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
                    If Ret = 1 AndAlso SQLCode = 0 Then

                        SQL = "UPDATE SCHMAST_SUB"
                        SQL &= " SET SYOUGOU_FLG_S = '0'"
                        SQL &= ", SYOUGOU_DATE_S = '" & New String("0"c, 8) & "'"
                        SQL &= ", UKETUKE_TIME_STAMP_S = '" & New String("0"c, 14) & "'"
                        SQL &= " WHERE FSYORI_KBN_SSUB = '" & FSYORI_KBN & "'"
                        SQL &= " AND TORIS_CODE_SSUB = '" & TORIS_CODE & "'"
                        SQL &= " AND TORIF_CODE_SSUB = '" & TORIF_CODE & "'"
                        SQL &= " AND FURI_DATE_SSUB = '" & FURI_DATE & "'"

                        Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
                        If Ret = 1 AndAlso SQLCode = 0 Then

                            SQL = "UPDATE MEDIA_ENTRY_TBL"
                            SQL &= " SET CHECK_FLG_ME = '0'"
                            SQL &= ", UPDATE_OP_ME = '" & GCom.GetUserID & "'"
                            SQL &= ", UPDATE_DATE_ME = TO_CHAR(SYSDATE, 'yyyymmddHH24MIss')"
                            SQL &= " WHERE FSYORI_KBN_ME = '" & FSYORI_KBN & "'"
                            SQL &= " AND TORIS_CODE_ME = '" & TORIS_CODE & "'"
                            SQL &= " AND TORIF_CODE_ME = '" & TORIF_CODE & "'"
                            SQL &= " AND FURI_DATE_ME = '" & FURI_DATE & "'"
                            SQL &= " AND NOT NVL(CHECK_FLG_ME, '0') = '0'"

                            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
                            If SQLCode = 0 Then

                                SQL = "DELETE FROM MEIMAST"
                                SQL &= " WHERE FSYORI_KBN_K = '" & FSYORI_KBN & "'"
                                SQL &= " AND TORIS_CODE_K = '" & TORIS_CODE & "'"
                                SQL &= " AND TORIF_CODE_K = '" & TORIF_CODE & "'"
                                SQL &= " AND FURI_DATE_K = '" & FURI_DATE & "'"

                                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
                                If SQLCode = 0 Then

                                    SQL = "UPDATE MEDIA_ENTRY_TBL"
                                    SQL &= " SET CHECK_FLG_ME = '0'"
                                    SQL &= ", UPDATE_OP_ME = '" & GCom.GetUserID & "'"
                                    SQL &= ", UPDATE_DATE_ME = TO_CHAR(SYSDATE, 'yyyymmddHH24MIss')"
                                    SQL &= " WHERE FSYORI_KBN_ME = '" & FSYORI_KBN & "'"
                                    SQL &= " AND TORIS_CODE_ME = '" & TORIS_CODE & "'"
                                    SQL &= " AND TORIF_CODE_ME = '" & TORIF_CODE & "'"
                                    SQL &= " AND FURI_DATE_ME = '" & FURI_DATE & "'"
                                    SQL &= " AND SYORI_KEN_ME = " & KEN
                                    SQL &= " AND SYORI_KIN_ME = " & KIN
                                    SQL &= " AND SYUBETU_CODE_ME = '" & SYUBETU_CODE & "'"
                                    SQL &= " AND BAITAI_CODE_ME = '" & BAITAI_CODE & "'"

                                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
                                    If SQLCode = 0 Then

                                        SQL = "DELETE FROM MATCHING_ERR_TBL"
                                        SQL &= " WHERE FSYORI_KBN_SE = '" & FSYORI_KBN & "'"
                                        SQL &= " AND TORIS_CODE_SE = '" & TORIS_CODE & "'"
                                        SQL &= " AND TORIF_CODE_SE = '" & TORIF_CODE & "'"
                                        SQL &= " AND FURI_DATE_SE = '" & FURI_DATE & "'"
                                        SQL &= " AND CYCLE_NO_SE = " & MOTIKOMI_SEQ

                                        Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
                                        If SQLCode = 0 Then

                                            SQL = "DELETE FROM NENKINMAST"
                                            SQL &= " WHERE FSYORI_KBN_K = '" & FSYORI_KBN & "'"
                                            SQL &= " AND TORIS_CODE_K = '" & TORIS_CODE & "'"
                                            SQL &= " AND TORIF_CODE_K = '" & TORIF_CODE & "'"
                                            SQL &= " AND FURI_DATE_K = '" & FURI_DATE & "'"

                                            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
                                            If SQLCode = 0 Then
                                                If GCom.NzInt(BAITAI_CODE) = 10 Then

                                                    SQL = "UPDATE WEB_RIREKIMAST"
                                                    SQL &= " SET STATUS_KBN_W = '0'"
                                                    SQL &= " WHERE FSYORI_KBN_W = '" & FSYORI_KBN & "'"
                                                    SQL &= " AND TORIS_CODE_W = '" & TORIS_CODE & "'"
                                                    SQL &= " AND TORIF_CODE_W = '" & TORIF_CODE & "'"
                                                    SQL &= " AND FURI_DATE_W = '" & FURI_DATE & "'"
                                                    SQL &= " AND STATUS_KBN_W = '1'"

                                                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
                                                    If SQLCode = 0 Then
                                                        Item.SubItems(LstIdx.Uketuke).Text = "×"
                                                        Item.SubItems(LstIdx.Touroku).Text = "×"
                                                        Item.SubItems(LstIdx.SyoriKen).Text = "0"
                                                        Item.SubItems(LstIdx.SyoriKin).Text = "0"

                                                        If Item.SubItems(LstIdx.SyougouKbn).Text = "1" Then
                                                            Item.SubItems(LstIdx.Syougou).Text = "×"
                                                        Else
                                                            Item.SubItems(LstIdx.Syougou).Text = "-"
                                                        End If
                                                        '受付タイムスタンプ
                                                        Item.SubItems(LstIdx.UketukeTimeStamp).Text = "0000/00/00 00:00:00"
                                                        'エラーコード
                                                        Item.SubItems(LstIdx.ErrorInf).Text = ""
                                                    Else
                                                        Exit Try
                                                    End If
                                                Else
                                                    Item.SubItems(LstIdx.Uketuke).Text = "×"
                                                    Item.SubItems(LstIdx.Touroku).Text = "×"
                                                    Item.SubItems(LstIdx.SyoriKen).Text = "0"
                                                    Item.SubItems(LstIdx.SyoriKin).Text = "0"

                                                    If Item.SubItems(LstIdx.SyougouKbn).Text = "1" Then
                                                        Item.SubItems(LstIdx.Syougou).Text = "×"
                                                    Else
                                                        Item.SubItems(LstIdx.Syougou).Text = "-"
                                                    End If
                                                    '受付タイムスタンプ
                                                    Item.SubItems(LstIdx.UketukeTimeStamp).Text = "0000/00/00 00:00:00"
                                                    'エラーコード
                                                    Item.SubItems(LstIdx.ErrorInf).Text = ""
                                                End If
                                            Else
                                                Exit Try
                                            End If
                                        Else
                                            Exit Try
                                        End If
                                    Else
                                        Exit Try
                                    End If
                                Else
                                    Exit Try
                                End If
                            Else
                                Exit Try
                            End If
                        Else
                            Exit Try
                        End If
                    Else
                        Exit Try
                    End If

                End If
            Next Item

            CreateCSV.CloseCsv()

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String
            ExeRepo.SetOwner = Me
            Dim iRet As Integer
            Dim errMsg As String = ""

            'パラメータ設定：ログイン名、ＣＳＶファイル名、取引先コード
            param = GCom.GetUserID & "," & strCSVFileName
            iRet = ExeRepo.ExecReport("KFJP026.EXE", param)

            If iRet <> 0 Then
                '印刷失敗：戻り値に対応したエラーメッセージを表示する
                Select Case iRet
                    Case -1
                        MessageBox.Show(MSG0226W.Replace("{0}", "処理結果確認表（落込取消）"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Case Else
                        MessageBox.Show(MSG0004E.Replace("{0}", "処理結果確認表（落込取消）"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Select

                TransStatus = False


                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "処理結果確認表（落込取消）", "失敗", "エラーコード：" & iRet)

            End If

            If TransStatus Then

                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)

                MSG = MSG0025I.Replace("{0}", "落し込み")
                Call MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            Else

                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)

                MSG = String.Format(MSG0027E, "落し込み", "取消")
                Call MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(落込取消)終了", "成功", "")
        End Try

    End Sub

    '返却データ再作成ボタン処理
    Private Sub CmdKekkaCreate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CmdKekkaCreate.Click

        Dim jobID As String = "J060"
        Dim Param As String = ""
        Dim MainDB As New CASTCommon.MyOracle
        Dim iRet As Integer
        Dim MSG As String

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(返還)開始", "成功", "")

            If ListView1.Items.Count <= 0 Then

                MSG = MSG0224W
                Call MessageBox.Show(MSG, GCom.GLog.Job1, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then

                    MSG = MSG0100W
                    Call MessageBox.Show(MSG, GCom.GLog.Job1, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            '### 返還データ作成に必要なパラメータの取得
            Dim paramToriCode As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.ToriCode).ToString.Replace("-", ""), "")
            Dim paramFuriDate As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.FuriDate).ToString.Replace("/", ""), "")
            Dim paramMotikomiSEQ As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.MotikomiSeq), "")

            '###メッセージボックスの表示に必要な情報の取得
            Dim toriName As String = GCom.NzStr(GCom.SelectedItem(ListView1, LstIdx.ItakuName)).Trim

            '### 判定に必要な情報の取得
            Dim HenkanFlg As String = GCom.NzStr(GCom.SelectedItem(ListView1, LstIdx.Henkan)).Trim
            Dim FunouFlg As String = GCom.NzStr(GCom.SelectedItem(ListView1, LstIdx.Funou)).Trim
            Dim BaitaiCode As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.BaitaiCode), "")
            Dim KekkaHenkyakuKbn As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.KekkaHenkyakuKbn), "")


            If Not FunouFlg = "○" Then
                MSG = MSG0322W
                MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf GCom.NzInt(KekkaHenkyakuKbn, 0) = 0 Then
                MSG = MSG0316W
                MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf GCom.NzInt(BaitaiCode, 0) = 7 Then
                MSG = MSG0317W
                MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            If HenkanFlg = "○" Then
                MSG = MSG0046I
                If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then Return
            End If

            If MessageBox.Show(MSG0023I.Replace("{0}", "返還データ作成"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then Return

            MainDB.BeginTrans()

            Param = paramToriCode & "," & paramFuriDate & "," & paramMotikomiSEQ.PadLeft(2, "0")

            '### ジョブマスタに登録されていないか確認
            iRet = MainLOG.SearchJOBMAST(jobID, Param, MainDB)
            If iRet = 1 Then
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return
            ElseIf iRet = -1 Then
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            '### ジョブマスタに登録
            If MainLOG.InsertJOBMAST(jobID, LW.UserID, Param, MainDB) = False Then
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0021I.Replace("{0}", "返還データ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(返還)例外エラー", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(返還)終了", "成功", "")
        End Try

    End Sub

    '
    ' 機　能 : 該当処理画面への遷移判定
    '
    ' 戻り値 : DialogResult.OK = 遷移する, DialogResult.Cancel = 遷移しない
    '
    ' 引き数 : ARG1 - 独自メッセージ
    ' 　　　   ARG2 - 記憶配列(値)
    '*** 修正 mitsu 2008/05/28 デフォルトボタン追加 ***
    '          ARG3 - デフォルトボタン選択値(省略化)
    '**************************************************
    '
    ' 備　考 : 共通化
    '
    Private Function CheckMessage(ByVal avMSG As String, ByRef StrData() As String, _
       Optional ByVal button As MessageBoxDefaultButton = MessageBoxDefaultButton.Button1) As DialogResult

        Try
            Dim MSG As String = ""

            If ListView1.Items.Count <= 0 Then
                MSG = MSG0224W
                Call MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return DialogResult.Cancel
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then

                    MSG = MSG0100W
                    Call MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return DialogResult.Cancel
                End If
            End If

            Dim StrName() As String = {"振替処理区分", "取引先主コード", "取引先副コード", "種別", "委託者コード", "委託者名", _
                                       "振替日", "依頼件数", "依頼金額", "持込SEQ", "ファイルSEQ"}

            Dim onDate As Date = GCom.SET_DATE(GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.FuriDate), ""))

            Dim Data() As String = {GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.FSyoriKbn)),
                        GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.ToriCode), "").Replace("-", "").Substring(0, 10),
                        GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.ToriCode), "").Replace("-", "").Substring(10, 2),
                        GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.Syubetu), ""),
                        GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.ItakuCode), ""),
                        GCom.NzStr(GCom.SelectedItem(ListView1, LstIdx.ItakuName)),
                        String.Format("{0:yyyy/MM/dd}", onDate),
                        String.Format("{0:#,##0}", GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.SyoriKen), 0)),
                        String.Format("{0:#,##0}", GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.SyoriKin), 0)),
                        GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.MotikomiSeq), ""),
                        GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.FileSeq), "")}
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
            '*** 修正 mitsu 2008/06/02 依頼取消の場合、マルチ先があれば表示する ***
            If button = MessageBoxDefaultButton.Button2 AndAlso MultiCnt > 1 Then
                MSG &= "マルチ先数　" & ControlChars.Tab & MultiCnt & Space(8) & ControlChars.Cr
            End If
            '**********************************************************************

            MSG &= ControlChars.Cr & avMSG & Space(8)

            Return MessageBox.Show(MSG, msgTitle, _
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)

        Catch ex As Exception
            Throw
        End Try
    End Function

    '
    ' 機　能 : 依頼データ取消が可能か否かの判定関数
    '
    ' 戻り値 : DialogResult.OK = 取消可能, DialogResult.Cancel = 取消不可
    '
    ' 引き数 : ARG1 - 振替日
    ' 　　　   ARG2 - 持込SEQ
    ' 　　　   ARG3 - 連携区分
    '*** 修正 mitsu 2008/05/28 全銀ファイルではなく伝送ファイル ***
    ' 　　　   ARG4 - 伝送ファイル名
    '**************************************************************
    '
    ' 備　考 : 共通化
    '
    Private Function CheckMeisaiDelete(ByVal FURI_DATE As String, ByVal MOTIKOMI_SEQ As String) As DialogResult

        MultiCnt = 0

        Try
            For Each Item As ListViewItem In ListView1.Items

                Dim CurrFURI_DATE As String = GCom.NzDec(Item.SubItems(LstIdx.FuriDate).Text, "")
                Dim CurrMOTIKOMI_SEQ As String = GCom.NzDec(Item.SubItems(LstIdx.MotikomiSeq).Text, "")
                '2018/05/25 タスク）西野 CHG 標準版修正：広島信金対応（未照合分の落込取消対応）------------------------ START
                Dim CurrBAITAI_CODE As String = GCom.NzDec(Item.SubItems(LstIdx.BaitaiCode).Text, "")
                '2018/05/25 タスク）西野 CHG 標準版修正：広島信金対応（未照合分の落込取消対応）------------------------ END

                If CurrFURI_DATE = FURI_DATE AndAlso CurrMOTIKOMI_SEQ = MOTIKOMI_SEQ Then

                    '2018/05/25 タスク）西野 CHG 標準版修正：広島信金対応（未照合分の落込取消対応）------------------------ START
                    Select Case CurrBAITAI_CODE
                        Case "04", "09"
                            If Item.SubItems(LstIdx.Touroku).Text.Trim = "○" Then
                                MultiCnt += 1
                            End If
                        Case Else
                            If Item.SubItems(LstIdx.Uketuke).Text.Trim = "○" Then
                                MultiCnt += 1
                            End If
                    End Select
                    'If Item.SubItems(6).Text.Trim = "○" Then
                    '    MultiCnt += 1
                    'End If
                    '2018/05/25 タスク）西野 CHG 標準版修正：広島信金対応（未照合分の落込取消対応）------------------------ END

                    Dim CurrHAISIN_FLG As String = Item.SubItems(LstIdx.Haisin).Text.Trim

                    If CurrHAISIN_FLG = "○" Then

                        Exit Try

                    End If
                End If
            Next Item

            Return DialogResult.OK
        Catch ex As Exception
            Throw
        End Try

        Return DialogResult.Cancel
    End Function

    Private Sub cmdOtosi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOtosi.Click

        Dim JobID As String = ""
        Dim Param As String = ""
        Dim MSG As String
        Dim iret As Integer
        Dim MainDB As New CASTCommon.MyOracle

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(落込)開始", "成功", "")

            If ListView1.Items.Count <= 0 Then
                MSG = MSG0224W
                Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    MSG = MSG0100W
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            Dim BAITAI_CODE As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.BaitaiCode), "")
            If GCom.NzInt(BAITAI_CODE) = 7 Then
                MSG = MSG0100W
                Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If GCom.NzStr(GCom.SelectedItem(ListView1, LstIdx.Touroku)) = "○" Then
                MSG = MSG0061W
                Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            '### 落し込みに必要なパラメータの取得
            '取引先コード（１２）2、振替日5、コード区分15、フォーマット区分16、媒体コード14、ラベル区分17
            '持込区分18
            Dim paraToriCode As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.ToriCode).ToString.Replace("-", ""), "")
            Dim paraFuriDate As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.FuriDate).ToString.Replace("/", ""), "")
            Dim paraCodeKbn As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.CodeKbn), "")
            Dim ParaFmtKbn As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.FmtKbn), "")
            Dim ParaBaitaiCode As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.BaitaiCode), "")
            Dim paraLabelKbn As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.LabelKbn), "")

            '###メッセージボックスの表示に必要な情報の取得
            Dim toriName As String = GCom.NzStr(GCom.SelectedItem(ListView1, LstIdx.ItakuName)).Trim
            Dim ItakuCOde As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.ItakuCode), "")

            'ログ用変数の設定
            LW.ToriCode = paraToriCode
            LW.FuriDate = paraFuriDate

            '### 判定に必要な情報の取得
            Dim MotikomiKbn As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.MotikomiKbn), "")

            '依頼書･伝票の場合、受付フラグをチェックする
            Dim UKETUKE As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.Uketuke), "")
            If (ParaFmtKbn = "04" OrElse ParaFmtKbn = "05") AndAlso UKETUKE <> "1" Then
                MessageBox.Show(MSG0342W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            '### いいかどうかは置いといて、選択されたのがセンター直接ならセンター直接で登録
            Select Case MotikomiKbn
                Case "0"
                    '金庫持込
                    JobID = "J010"
                    Param = paraToriCode & "," & paraFuriDate & "," & paraCodeKbn & "," & ParaFmtKbn & "," & ParaBaitaiCode & "," & paraLabelKbn
                Case "1"
                    'センター直接持込
                    JobID = "J011"
                    Param = paraFuriDate & "," & ParaBaitaiCode
            End Select

            If MessageBox.Show(MSG0023I.Replace("{0}", "落し込み"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            MainDB.BeginTrans()

            '### ジョブマスタに登録されていないか確認
            iret = MainLOG.SearchJOBMAST(JobID, Param, MainDB)
            If iret = 1 Then
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return
            ElseIf iret = -1 Then
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return
            End If

            '### ジョブマスタに登録
            If MainLOG.InsertJOBMAST(JobID, LW.UserID, Param, MainDB) = False Then
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0021I.Replace("{0}", "落し込み"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(落込)例外エラー", "失敗", ex.Message)
            Return
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(落込)終了", "成功", "")
        End Try
    End Sub

    Private Sub CmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdCancel.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)開始", "成功", "")

            Me.ListView1.Items.Clear()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)終了", "成功", "")
        End Try
    End Sub

    Private Sub CmdInputRePrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdInputRePrint.Click

        Dim MSG As String

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力帳票再印刷", "開始", "")

            If ListView1.Items.Count <= 0 Then
                MSG = MSG0224W
                Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    MSG = MSG0100W
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            If GCom.NzStr(GCom.SelectedItem(ListView1, LstIdx.Touroku)) <> "○" Then
                MSG = MSG0085I
                Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim PrintFileDir As String = GetFSKJIni("COMMON", "HOST-PRT")
            Dim PrintID As String = String.Empty
            Dim PrintRPD As String = String.Empty
            Dim PrintFileName As String = String.Empty
            Dim PrintCount As Integer = 0
            Dim ToriCode As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.ToriCode).ToString.Replace("-", ""), "")
            Dim FuriDate As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.FuriDate).ToString.Replace("/", ""), "")
            Dim MotikomiSeq As String = "0000"

            '---------------------------------------------------
            ' 受付明細表再印刷
            '---------------------------------------------------
            PrintID = "KFJP003"
            PrintRPD = "KFJP003_受付明細表(プリンタ用).rpd"
            PrintFileName = PrintID & "_" & ToriCode & FuriDate & MotikomiSeq & ".CSV"
            If System.IO.File.Exists(System.IO.Path.Combine(PrintFileDir, PrintFileName)) = True Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力帳票再印刷", "開始", "受付明細表再印刷 [" & System.IO.Path.Combine(PrintFileDir, PrintFileName) & "]")

                Dim ListPrint As New CAstExternal.Print(PrintID, PrintRPD, System.IO.Path.Combine(PrintFileDir, PrintFileName))
                If ListPrint.ReportExecute() = False Then
                    MessageBox.Show(MSG0004E.Replace("{0}", "受付明細表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力帳票再印刷", "失敗", "受付明細表再印刷")
                    Return
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力帳票再印刷", "成功", "受付明細表再印刷")
                End If

                PrintCount += 1
            End If

            '---------------------------------------------------
            ' インプットエラーリスト再印刷
            '---------------------------------------------------
            PrintID = "KFJP061"
            PrintRPD = "KFJP061_インプットエラーリスト.rpd"
            PrintFileName = PrintID & "_" & ToriCode & FuriDate & MotikomiSeq & ".CSV"
            If System.IO.File.Exists(System.IO.Path.Combine(PrintFileDir, PrintFileName)) = True Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力帳票再印刷", "開始", "インプットエラーリスト再印刷 [" & System.IO.Path.Combine(PrintFileDir, PrintFileName) & "]")

                Dim ListPrint As New CAstExternal.Print(PrintID, PrintRPD, System.IO.Path.Combine(PrintFileDir, PrintFileName))
                If ListPrint.ReportExecute() = False Then
                    MessageBox.Show(MSG0004E.Replace("{0}", "インプットエラーリスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力帳票再印刷", "失敗", "インプットエラーリスト再印刷")
                    Return
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力帳票再印刷", "成功", "インプットエラーリスト再印刷")
                End If

                PrintCount += 1
            End If

            If PrintCount = 0 Then
                MessageBox.Show(String.Format(MSG0086I, "振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力帳票再印刷", "成功", "帳票なし")
            Else
                MessageBox.Show(String.Format(MSG0014I, "入力帳票"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力帳票再印刷", "成功", "")
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力帳票再印刷", "失敗", ex.Message)
            Return
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力帳票再印刷", "終了", "")
        End Try

    End Sub

    Private Sub CmdKekkaRePrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdKekkaRePrint.Click

        Dim MSG As String

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "結果帳票印刷", "開始", "")

            If ListView1.Items.Count <= 0 Then
                MSG = MSG0224W
                Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    MSG = MSG0100W
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            Dim BAITAI_CODE As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.BaitaiCode), "")
            Select Case GCom.NzInt(BAITAI_CODE)
                Case 7
                    MSG = String.Format(MSG0039I, "印刷")
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
            End Select

            If GCom.NzStr(GCom.SelectedItem(ListView1, LstIdx.Funou)) <> "○" Then
                MSG = MSG0087I
                Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            '---------------------------------------------------
            ' 振替結果・振替不能明細表印刷
            '---------------------------------------------------
            Dim ToriCode As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.ToriCode).ToString.Replace("-", ""), "")
            Dim FuriDate As String = GCom.NzDec(GCom.SelectedItem(ListView1, LstIdx.FuriDate).ToString.Replace("/", ""), "")
            Dim PrintID As String = ""
            Dim PrintName As String = ""

            '--------------------------------
            'マスタチェック
            '--------------------------------
            Dim KekkaPrintKbn As String = String.Empty
            If fn_check_Table(ToriCode, KekkaPrintKbn) = False Then
                Exit Sub
            End If

            Dim Funou As String = "0"
            Select Case KekkaPrintKbn
                Case "1", "6", "8"
                    PrintID = "KFJP018"
                    PrintName = "振替結果明細表"
                    Funou = "0"
                Case "0", "5", "7"
                    PrintID = "KFJP017"
                    PrintName = "不能結果明細表"
                    Funou = "1"
                Case Else
                    MessageBox.Show(MSG0088I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "結果帳票印刷", "成功", "振替結果帳票出力区分:" & KekkaPrintKbn)
                    Return
            End Select


            Dim param As String
            Dim nRet As Integer
            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            param = ToriCode & "," & FuriDate & "," & Funou
            nRet = ExeRepo.ExecReport(PrintID & ".EXE", param)

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "結果帳票印刷", "成功", "")
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "結果帳票印刷", "成功", "印刷対象なし")
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "結果帳票印刷", "成功", "")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "結果帳票印刷", "失敗", ex.Message)
            Return
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "結果帳票印刷", "終了", "")
        End Try

    End Sub

    Private Function fn_check_Table(ByVal ToriCode As String, ByRef KekkaPrintKbn As String) As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :印刷ボタンを押下時にマスタ相関チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/19
        'Update         :
        '============================================================================
        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try
            '------------------------------------------
            '取引先マスタ存在チェック
            '------------------------------------------
            '取引先情報取得
            SQL.Append("SELECT ")
            SQL.Append(" FUNOU_MEISAI_KBN_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(ToriCode.Substring(0, 10)))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(ToriCode.Substring(10, 2)))
            If OraReader.DataReader(SQL) = True Then
                KekkaPrintKbn = OraReader.GetString("FUNOU_MEISAI_KBN_T")
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function

End Class