Option Strict On
Option Explicit On

Imports System.Text
Imports System.Data.OracleClient
Imports CASTCommon
Imports System.Collections.Specialized

Public Class KFSMAST040

#Region "変数宣言"
    '共通イベントコントロール
    Private CAST As New CASTCommon.Events

    '数値を許可する
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    'ソートオーダーフラグ
    Dim SortOrderFlag As Boolean = True

    'クリックした列の番号
    Dim ClickedColumn As Integer

    Private MyOwnerForm As Form

    'マルチ先件数追加
    Private MultiCnt As Integer

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFSMAST040", "スケジュール管理画面")
    Private Const msgTitle As String = "スケジュール管理画面(KFSMAST040)"

    Private BAITAI_CODE_LIST As StringDictionary
    Private SYUBETU_LIST As StringDictionary

    '2017/04/26 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ START
    Private SORT_KEY As String = ""
    '2017/04/26 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ END
    '2017/04/26 タスク）西野 ADD 標準版修正（From-ToのINI化）------------------------------------ START
    '自振にあわせる
    Private FmtComm As New CAstFormat.CFormat
    '2017/04/26 タスク）西野 ADD 標準版修正（From-ToのINI化）------------------------------------ END

#End Region

    '画面起動イベント処理
    Private Sub KFSMAST040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            MyOwnerForm = GOwnerForm
            GOwnerForm = Me

            Me.CmdBack.DialogResult = DialogResult.None

            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            '2017/04/26 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ START
            SORT_KEY = GCom.GetObjectParam("KFSMAST040", "SORT", "0")
            '2017/04/26 タスク）西野 ADD 標準版修正（ソート順INI化対応）------------------------------------ END

            '項目名称定義
            With ListView1
                .Clear()
                .Columns.Add("項番", 40, HorizontalAlignment.Right)                 '0
                .Columns.Add("委託者名", 100, HorizontalAlignment.Left)             '1
                .Columns.Add("取引先コード", 90, HorizontalAlignment.Left)          '2
                .Columns.Add("委託者コード", 90, HorizontalAlignment.Center)        '3
                .Columns.Add("媒体", 50, HorizontalAlignment.Left)                  '4
                .Columns.Add("振込日", 80, HorizontalAlignment.Center)              '5
                .Columns.Add("持込回数", 40, HorizontalAlignment.Right)             '6
                .Columns.Add("登録", 40, HorizontalAlignment.Center)                '7
                .Columns.Add("確保", 40, HorizontalAlignment.Center)                '8
                .Columns.Add("発信", 40, HorizontalAlignment.Center)                '9
                .Columns.Add("決済", 40, HorizontalAlignment.Center)                '10
                .Columns.Add("種別", 60, HorizontalAlignment.Left)                  '11
                .Columns.Add("依頼件数", 70, HorizontalAlignment.Right)             '12
                .Columns.Add("依頼金額", 120, HorizontalAlignment.Right)            '13
                .Columns.Add("媒体コード", 0, HorizontalAlignment.Right)            '14
                .Columns.Add("コード区分", 0, HorizontalAlignment.Right)            '15
                .Columns.Add("フォーマット区分", 0, HorizontalAlignment.Right)      '16
                .Columns.Add("ラベル区分", 0, HorizontalAlignment.Right)            '17
                .Columns.Add("持込SEQ", 0, HorizontalAlignment.Right)               '18
                .Columns.Add("ファイルSEQ", 0, HorizontalAlignment.Right)           '19
                .Columns.Add("代表委託者コード", 0, HorizontalAlignment.Right)      '20
                .Columns.Add("受付", 0, HorizontalAlignment.Right)                  '21
            End With
            If ListView1.Items.Count > 0 Then
                ListView1.Items(0).Selected = True
            End If

            '標準日付を算出する
            Dim StartDate As Date
            Dim FinalDate As Date
            '2017/04/26 タスク）西野 CHG 標準版修正（From-ToのINI化）------------------------------------ START
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
            Call GCom.GetFromTo("KFSMAST040", intFrom, intTo, -15, 15)

            For Index As Integer = 0 To 1 Step 1
                Select Case Index
                    Case 0 : StartDate = CASTCommon.GetEigyobi(Date.Now, intFrom, FmtComm.HolidayList)
                    Case 1 : FinalDate = CASTCommon.GetEigyobi(Date.Now, intTo, FmtComm.HolidayList)
                End Select
            Next
            'Dim Ret As Boolean = GCom.CheckDateModule(Nothing, 1)
            'For Index As Integer = 0 To 1 Step 1
            '    Dim Sign As Double
            '    Select Case Index
            '        Case 0 : Sign = -15
            '        Case 1 : Sign = 15
            '    End Select
            '    Dim Temp As String = String.Format("{0:yyyyMMdd}", Date.Now.AddDays(Sign))
            '    Dim Temp2 As String = ""
            '    Ret = GCom.CheckDateModule(Temp, Temp2, Index)
            '    Select Case Index
            '        Case 0 : StartDate = GCom.SET_DATE(Temp2)
            '        Case 1 : FinalDate = GCom.SET_DATE(Temp2)
            '    End Select
            'Next Index
            '2017/04/26 タスク）西野 CHG 標準版修正（From-ToのINI化）------------------------------------ END

            '１ヶ月間＋１日で標準表示する
            With Me
                .TxtYear1.Text = StartDate.Year.ToString.PadLeft(4, "0"c)
                .TxtYear2.Text = FinalDate.Year.ToString.PadLeft(4, "0"c)
                .TxtMonth1.Text = StartDate.Month.ToString.PadLeft(2, "0"c)
                .TxtMonth2.Text = FinalDate.Month.ToString.PadLeft(2, "0"c)
                .TxtDay1.Text = StartDate.Day.ToString.PadLeft(2, "0"c)
                .TxtDay2.Text = FinalDate.Day.ToString.PadLeft(2, "0"c)
            End With

            BAITAI_CODE_LIST = GetStringDictionary("Common_総振_媒体コード.TXT")
            SYUBETU_LIST = GetStringDictionary("KFSMAST010_種別.TXT")

            Me.TxtYear1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    '画面終了時処理
    Private Sub KFSMAST040_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        Try
            GOwnerForm = MyOwnerForm
            GOwnerForm.Visible = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        End Try
    End Sub

    '参照ボタン処理
    Private Sub CmdSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdSelect.Click

        Dim MSG As String
        Dim FURI_DATE(1) As Date
        Dim TORIS_CODE As String = ""
        Dim TORIF_CODE As String = ""
        Dim BAITAI_CODE As String = ""
        Dim SYUBETU As String = ""
        Dim REC As OracleDataReader = Nothing

        Dim SQL As New StringBuilder(128)
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

            SQL.AppendLine("SELECT")
            SQL.AppendLine("  TORIS_CODE_S")
            SQL.AppendLine(", TORIF_CODE_S")
            SQL.AppendLine(", FURI_DATE_S")
            SQL.AppendLine(", MOTIKOMI_SEQ_S")
            SQL.AppendLine(", ITAKU_CODE_S")
            SQL.AppendLine(", BAITAI_CODE_S")
            SQL.AppendLine(", SYORI_KEN_S")
            SQL.AppendLine(", SYORI_KIN_S")
            SQL.AppendLine(", UKETUKE_FLG_S")
            SQL.AppendLine(", TOUROKU_FLG_S")
            SQL.AppendLine(", KAKUHO_FLG_S")
            SQL.AppendLine(", HASSIN_FLG_S")
            SQL.AppendLine(", SOUSIN_FLG_S")
            SQL.AppendLine(", KESSAI_FLG_S")
            SQL.AppendLine(", TYUUDAN_FLG_S")
            SQL.AppendLine(", FILE_SEQ_S")
            SQL.AppendLine(", SYUBETU_S")
            SQL.AppendLine(", ERROR_INF_S")

            SQL.AppendLine(", ITAKU_KANRI_CODE_T")
            SQL.AppendLine(", ITAKU_NNAME_T")
            SQL.AppendLine(", CODE_KBN_T")
            SQL.AppendLine(", FMT_KBN_T")
            SQL.AppendLine(", LABEL_KBN_T")
            SQL.AppendLine(", KESSAI_KBN_T")
            SQL.AppendLine(", KESSAI_PATN_T")

            SQL.AppendLine("  FROM S_SCHMAST, S_TORIMAST")
            SQL.AppendLine(" WHERE FSYORI_KBN_S = FSYORI_KBN_T")
            SQL.AppendLine(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S = TORIF_CODE_T")

            '振込日範囲(必須)
            SQL.AppendLine(" AND FURI_DATE_S BETWEEN '" & String.Format("{0:yyyyMMdd}", FURI_DATE(0)) & "'")
            SQL.AppendLine(" AND '" & String.Format("{0:yyyyMMdd}", FURI_DATE(1)) & "'")
            '2017/04/26 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ START
            If SORT_KEY = "" Then
                SQL.AppendLine(" ORDER BY TORIS_CODE_S ASC")
                SQL.AppendLine(", TORIF_CODE_S ASC")
                SQL.AppendLine(", FURI_DATE_S ASC")
                SQL.AppendLine(", MOTIKOMI_SEQ_S ASC")
            Else
                SQL.Append(" ORDER BY " & SORT_KEY)
            End If
            'SQL.AppendLine(" ORDER BY TORIS_CODE_S ASC")
            'SQL.AppendLine(", TORIF_CODE_S ASC")
            'SQL.AppendLine(", FURI_DATE_S ASC")
            'SQL.AppendLine(", MOTIKOMI_SEQ_S ASC")
            '2017/04/26 タスク）西野 CHG 標準版修正（ソート順INI化対応）------------------------------------ END

            If GCom.SetDynaset(SQL.ToString, REC) Then

                Dim ROW As Integer = 0

                Do While REC.Read

                    Dim Data(21) As String
                    
                    Data(0) = (ROW + 1).ToString("0000")
                    Data(1) = GCom.NzStr(REC.Item("ITAKU_NNAME_T")).Trim        '委託者名
                    Data(2) = GCom.NzDec(REC.Item("TORIS_CODE_S"), "") & "-" & GCom.NzDec(REC.Item("TORIF_CODE_S"), "")   '取引先主コード
                    Data(3) = GCom.NzDec(REC.Item("ITAKU_CODE_S"), "")          '委託者コード
                    
                    SYUBETU = GCom.NzDec(REC.Item("SYUBETU_S"), "00")
                    BAITAI_CODE = GCom.NzDec(REC.Item("BAITAI_CODE_S"), "00")
                    If BAITAI_CODE_LIST.ContainsKey(BAITAI_CODE) Then
                        Data(4) = BAITAI_CODE_LIST.Item(BAITAI_CODE)
                    Else
                        Data(4) = "？？"
                    End If

                    With GCom.NzDec(REC.Item("FURI_DATE_S"), "")
                        Data(5) = .Substring(0, 4) & "/" & .Substring(4, 2) & "/" & .Substring(6, 2)           '振込日
                    End With

                    Data(6) = GCom.NzInt(REC.Item("MOTIKOMI_SEQ_S"), 0).ToString

                    '落込
                    If GCom.NzInt(REC.Item("TOUROKU_FLG_S"), 0) = 1 Then
                        Data(7) = "○"
                    Else
                        Data(7) = "×"
                    End If

                    '確保
                    Select Case GCom.NzDec(REC.Item("KESSAI_PATN_T"), 0)
                        Case 0
                            If GCom.NzInt(REC.Item("KAKUHO_FLG_S"), 0) = 1 Then
                                Data(8) = "○"
                            Else
                                Data(8) = "×"
                            End If
                        Case Else
                            Data(8) = "-"
                    End Select

                    '発信
                    If GCom.NzInt(REC.Item("HASSIN_FLG_S"), 0) = 1 Then
                        Data(9) = "○"
                    Else
                        Data(9) = "×"
                    End If

                    '決済
                    Select Case GCom.NzDec(REC.Item("KESSAI_KBN_T"), 0)
                        Case 99
                            Data(10) = "-"
                        Case Else
                            If GCom.NzInt(REC.Item("KESSAI_FLG_S"), 0) = 1 Then
                                Data(10) = "○"
                            Else
                                Data(10) = "×"
                            End If
                    End Select

                    '種別
                    If SYUBETU_LIST.ContainsKey(SYUBETU) Then
                        Data(11) = SYUBETU_LIST.Item(SYUBETU)
                    Else
                        Data(11) = "？？"
                    End If

                    Data(12) = CLng(REC.Item("SYORI_KEN_S")).ToString("#,0")          '依頼件数
                    Data(13) = CLng(REC.Item("SYORI_KIN_S")).ToString("#,0")          '依頼金額
                    Data(14) = BAITAI_CODE                                            '媒体コード
                    Data(15) = GCom.NzDec(REC.Item("CODE_KBN_T"), "")                 'コード区分
                    Data(16) = GCom.NzDec(REC.Item("FMT_KBN_T"), "")                  'フォーマット区分
                    Data(17) = GCom.NzDec(REC.Item("LABEL_KBN_T"), "")                'ラベル区分
                    Data(18) = GCom.NzDec(REC.Item("MOTIKOMI_SEQ_S"), "")             '持込SEQ
                    Data(19) = GCom.NzDec(REC.Item("FILE_SEQ_S"), "")                 'ファイルＳＥＱ
                    Data(20) = GCom.NzDec(REC.Item("ITAKU_KANRI_CODE_T"), "")         '代表委託者コード
                    Data(21) = GCom.NzDec(REC.Item("UKETUKE_FLG_S"), "")              '受付フラグ

                    Dim LineColor As Color

                    If ROW Mod 2 = 0 Then
                        LineColor = Color.White
                    Else
                        LineColor = Color.WhiteSmoke
                    End If

                    Dim FontColor As Color
                    If GCom.NzStr(REC.Item("ERROR_INF_S")).Trim = "020" Then
                        FontColor = Color.Red
                    Else
                        FontColor = Color.Black
                    End If

                    Dim vLstItem As New ListViewItem(Data, -1, FontColor, LineColor, Nothing)
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
        Dim CreateCSV As New KFSPxxx("KFSP021")
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

                CreateCSV.OutputCsvData(item.SubItems(0).Text, True)                                    '項番
                CreateCSV.OutputCsvData(SyoriDate, True)                                                '処理日
                CreateCSV.OutputCsvData(SyoriTime, True)                                                'タイムスタンプ
                CreateCSV.OutputCsvData(startFuriDate, True)                                            '開始振込日
                CreateCSV.OutputCsvData(endFuriDate, True)                                              '終了振込日
                CreateCSV.OutputCsvData(item.SubItems(2).Text.Replace("-", "").Substring(0, 10), True)  '取引先主コード
                CreateCSV.OutputCsvData(item.SubItems(2).Text.Replace("-", "").Substring(10, 2), True)  '取引先副コード
                CreateCSV.OutputCsvData(item.SubItems(1).Text, True)                                    '取引先名（漢字）
                CreateCSV.OutputCsvData(item.SubItems(3).Text, True)                                    '委託者コード
                CreateCSV.OutputCsvData(item.SubItems(4).Text, True)                                    '媒体
                CreateCSV.OutputCsvData(item.SubItems(5).Text.Replace("/", ""), True)                   '振込日
                CreateCSV.OutputCsvData(item.SubItems(6).Text.Replace(",", ""), True)                   '持込回数
                CreateCSV.OutputCsvData(item.SubItems(7).Text.Replace("/", ""), True)                   '登録
                CreateCSV.OutputCsvData(item.SubItems(8).Text.Replace("/", ""), True)                   '確保
                CreateCSV.OutputCsvData(item.SubItems(9).Text.Replace("/", ""), True)                   '発信
                CreateCSV.OutputCsvData(item.SubItems(10).Text.Replace("/", ""), True)                  '決済
                CreateCSV.OutputCsvData(item.SubItems(12).Text.Replace(",", ""))                        '処理件数
                CreateCSV.OutputCsvData(item.SubItems(13).Text.Replace(",", ""), False, True)           '処理金額

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
            iRet = ExeRepo.ExecReport("KFSP021.EXE", param)

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
    Private Sub CmdUkeDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdUkeDelete.Click
        Dim Ret As Integer
        Dim SQLCode As Integer
        Dim TransStatus As Boolean = False
        Dim CreateCSV As New KFSPxxx("KFSP020")
        Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")
        Dim SyoriTime As String = System.DateTime.Now.ToString("HHmmss")

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(落込取消)開始", "成功", "")

            If ListView1.Items.Count <= 0 Then
                Call MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    Call MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            If GCom.NzStr(GCom.SelectedItem(ListView1, 7)) <> "○" Then
                Call MessageBox.Show(MSG0508W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim memFURI_DATE As String = GCom.NzDec(GCom.SelectedItem(ListView1, 5), "")
            Dim memMOTIKOMI_SEQ As String = GCom.NzDec(GCom.SelectedItem(ListView1, 6), "")
            Dim memITAKU_KANRI_CODE As String = GCom.NzDec(GCom.SelectedItem(ListView1, 20), "")
            '種別を条件に追加
            Dim memSyubetu As String = GCom.SelectedItem(ListView1, 11).ToString
            '2010/10/20 取引先コード単位で落し込み取消しを行う（マルチ非対応）-----------------------------------------------
            Dim memTORIS_CODE As String = GCom.NzDec(GCom.SelectedItem(ListView1, 2), "").Replace("-", "").Substring(0, 10)
            Dim memTORIF_CODE As String = GCom.NzDec(GCom.SelectedItem(ListView1, 2), "").Replace("-", "").Substring(10, 2)


            ''マルチの明細内に取り消しできないものがあるか否かチェックする
            'If CheckMeisaiDelete(memFURI_DATE, memMOTIKOMI_SEQ, memITAKU_KANRI_CODE, memSyubetu) = DialogResult.Cancel Then
            '    Call MessageBox.Show(MSG0508W, msgTitle, _
            '                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Return
            'End If
            '明細内に取り消しできないものがあるか否かチェックする
            If CheckMeisaiDelete(memFURI_DATE, memMOTIKOMI_SEQ, memTORIS_CODE, memTORIF_CODE) = DialogResult.Cancel Then
                Call MessageBox.Show(MSG0508W, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim StrData(8) As String
            If Not CheckMessage(MSG0502I, StrData, MessageBoxDefaultButton.Button2) = DialogResult.OK Then
                Return
            End If
            '----------------------------------------------------------------------------------------------------------------

            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName

            For Each Item As ListViewItem In ListView1.Items

                Dim FURI_DATE As String = GCom.NzDec(Item.SubItems(5), "")
                Dim MOTIKOMI_SEQ As String = GCom.NzDec(Item.SubItems(6), "")
                Dim ITAKU_KANRI_CODE As String = GCom.NzDec(Item.SubItems(20), "")
                Dim Syubetu As String = Item.SubItems(11).Text

                '2010/10/20 取引先コード単位で落し込み取消しを行う（マルチ非対応）
                Dim TORIS_CODE As String = GCom.NzDec(Item.SubItems(2).Text.Replace("-", "").Substring(0, 10), "")
                Dim TORIF_CODE As String = GCom.NzDec(Item.SubItems(2).Text.Replace("-", "").Substring(10, 2), "")

                If FURI_DATE = memFURI_DATE AndAlso MOTIKOMI_SEQ = memMOTIKOMI_SEQ AndAlso _
                                TORIS_CODE = memTORIS_CODE AndAlso TORIF_CODE = memTORIF_CODE AndAlso Item.SubItems(7).Text = "○" Then


                    If TransStatus = False Then
                        TransStatus = True
                        Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Begin)
                    End If

                    '2010/10/20 取引先コード単位で落し込み取消しを行う（マルチ非対応）------------------------------------
                    'Dim TORIS_CODE As String = GCom.NzDec(Item.SubItems(2).Text.Replace("-", "").Substring(0, 10), "")
                    'Dim TORIF_CODE As String = GCom.NzDec(Item.SubItems(2).Text.Replace("-", "").Substring(10, 2), "")
                    '-----------------------------------------------------------------------------------------------------
                    Dim FILE_SEQ As String = GCom.NzDec(Item.SubItems(19), "")
                    Dim SYUBETU_CODE As String = GCom.NzStr(Item.SubItems(11).Text)

                    LW.ToriCode = TORIS_CODE & "-" & TORIF_CODE
                    LW.FuriDate = FURI_DATE

                    '###処理結果確認表（落込取消）の印刷
                    CreateCSV.OutputCsvData(SyoriDate, True)                                      '処理日
                    CreateCSV.OutputCsvData(SyoriTime, True)                                      'タイムスタンプ
                    CreateCSV.OutputCsvData("3", True)                                            '振込処理区分
                    CreateCSV.OutputCsvData(TORIS_CODE, True)                                     '取引先主コード
                    CreateCSV.OutputCsvData(TORIF_CODE, True)                                     '取引先副コード
                    CreateCSV.OutputCsvData(Item.SubItems(1).Text, True)                          '取引先名（漢字）
                    CreateCSV.OutputCsvData(Item.SubItems(3).Text, True)                          '委託者コード
                    CreateCSV.OutputCsvData(FURI_DATE, True)                                      '振込日
                    CreateCSV.OutputCsvData(Item.SubItems(12).Text.Replace(",", ""))              '処理件数
                    CreateCSV.OutputCsvData(Item.SubItems(13).Text.Replace(",", ""))              '処理金額
                    CreateCSV.OutputCsvData(MOTIKOMI_SEQ, True)                                   '持込ＳＥＱ
                    CreateCSV.OutputCsvData(SYUBETU_CODE, True, True)                             '種別

                    Dim SQL As StringBuilder = New StringBuilder(128)

                    SQL.AppendLine("UPDATE S_SCHMAST")
                    SQL.AppendLine(" SET UKETUKE_FLG_S ='0'")
                    SQL.AppendLine(", TOUROKU_FLG_S = '0'")
                    SQL.AppendLine(", SYORI_KEN_S = 0")
                    SQL.AppendLine(", SYORI_KIN_S = 0")
                    SQL.AppendLine(", ERR_KEN_S = 0")
                    SQL.AppendLine(", ERR_KIN_S = 0")
                    SQL.AppendLine(", TESUU_KIN_S = 0")
                    SQL.AppendLine(", TESUU_KIN1_S = 0")
                    SQL.AppendLine(", TESUU_KIN2_S = 0")
                    SQL.AppendLine(", TESUU_KIN3_S = 0")
                    SQL.AppendLine(", FURI_KEN_S = 0")
                    SQL.AppendLine(", FURI_KIN_S = 0")
                    SQL.AppendLine(", FUNOU_KEN_S = 0")
                    SQL.AppendLine(", FUNOU_KIN_S = 0")
                    SQL.AppendLine(", UKETUKE_DATE_S = '" & New String("0"c, 8) & "'")
                    SQL.AppendLine(", TOUROKU_DATE_S = '" & New String("0"c, 8) & "'")
                    SQL.AppendLine(", UFILE_NAME_S = NULL")
                    SQL.AppendLine(", ERROR_INF_S = NULL")
                    SQL.AppendLine(" WHERE TORIS_CODE_S = '" & TORIS_CODE & "'")
                    SQL.AppendLine(" AND TORIF_CODE_S = '" & TORIF_CODE & "'")
                    SQL.AppendLine(" AND FURI_DATE_S = '" & FURI_DATE & "'")
                    SQL.AppendLine(" AND MOTIKOMI_SEQ_S = " & MOTIKOMI_SEQ)
                    SQL.AppendLine(" AND FILE_SEQ_S = " & FILE_SEQ)

                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode)
                    If Ret = 1 AndAlso SQLCode = 0 Then

                        SQL.Length = 0
                        SQL.AppendLine("DELETE FROM S_MEIMAST")
                        SQL.AppendLine(" WHERE TORIS_CODE_K = '" & TORIS_CODE & "'")
                        SQL.AppendLine(" AND TORIF_CODE_K = '" & TORIF_CODE & "'")
                        SQL.AppendLine(" AND FURI_DATE_K = '" & FURI_DATE & "'")
                        SQL.AppendLine(" AND MOTIKOMI_SEQ_K = " & MOTIKOMI_SEQ)

                        Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode)
                        If SQLCode = 0 Then
                            Item.SubItems(7).Text = "×"
                            Item.SubItems(12).Text = "0"
                            Item.SubItems(13).Text = "0"
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
            iRet = ExeRepo.ExecReport("KFSP020.EXE", param)

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

                Call MessageBox.Show(MSG0025I.Replace("{0}", "落し込み"), msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            Else

                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)

                Call MessageBox.Show(String.Format(MSG0027E, "落し込み", "取消"), msgTitle, _
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

    '
    ' 機　能 : 該当処理画面への遷移判定
    '
    ' 戻り値 : DialogResult.OK = 遷移する, DialogResult.Cancel = 遷移しない
    '
    ' 引き数 : ARG1 - 独自メッセージ
    ' 　　　   ARG2 - 記憶配列(値)
    '          ARG3 - デフォルトボタン選択値(省略化)
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

            Dim StrName() As String = {"振込処理区分", "取引先主コード", "取引先副コード", "種別", "委託者コード", "委託者名", _
                                       "振込日", "依頼件数", "依頼金額", "持込回数", "ファイルSEQ"}

            Dim onDate As Date = GCom.SET_DATE(GCom.NzDec(GCom.SelectedItem(ListView1, 5), ""))
            Dim Data() As String = {"3", _
                        GCom.NzDec(GCom.SelectedItem(ListView1, 2), "").Replace("-", "").Substring(0, 10), _
                        GCom.NzDec(GCom.SelectedItem(ListView1, 2), "").Replace("-", "").Substring(10, 2), _
                        GCom.SelectedItem(ListView1, 11).ToString, _
                        GCom.NzDec(GCom.SelectedItem(ListView1, 3), ""), _
                        GCom.NzStr(GCom.SelectedItem(ListView1, 1)), _
                        String.Format("{0:yyyy/MM/dd}", onDate), _
                        String.Format("{0:#,##0}", GCom.NzDec(GCom.SelectedItem(ListView1, 12), 0)), _
                        String.Format("{0:#,##0}", GCom.NzDec(GCom.SelectedItem(ListView1, 13), 0)), _
                        GCom.NzDec(GCom.SelectedItem(ListView1, 6), ""), _
                        GCom.NzDec(GCom.SelectedItem(ListView1, 19), "")}
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
            '依頼取消の場合、マルチ先があれば表示する
            If button = MessageBoxDefaultButton.Button2 AndAlso MultiCnt > 1 Then
                MSG &= "マルチ先数　" & ControlChars.Tab & MultiCnt & Space(8) & ControlChars.Cr
            End If
           
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
    ' 引き数 : ARG1 - 振込日
    ' 　　　   ARG2 - 持込SEQ
    '
    ' 備　考 : 共通化
    '
    ' 2010/10/20 取引先コード単位で落し込み取消しを行う（マルチ非対応）
    '
    Private Function CheckMeisaiDelete(ByVal FURI_DATE As String, ByVal MOTIKOMI_SEQ As String, ByVal TORIS_CODE As String, ByVal TORIF_CODE As String) As DialogResult
        MultiCnt = 0

        Try
            For Each Item As ListViewItem In ListView1.Items

                Dim CurrFURI_DATE As String = GCom.NzDec(Item.SubItems(5).Text, "")
                Dim CurrMOTIKOMI_SEQ As String = GCom.NzDec(Item.SubItems(6).Text, "")
                Dim CurrTORIS_CODE As String = GCom.NzDec(Item.SubItems(2).Text.Replace("-", "").Substring(0, 10), "")
                Dim CurrTORIF_CODE As String = GCom.NzDec(Item.SubItems(2).Text.Replace("-", "").Substring(10, 2), "")

                If CurrFURI_DATE = FURI_DATE AndAlso CurrMOTIKOMI_SEQ = MOTIKOMI_SEQ AndAlso CurrTORIS_CODE = TORIS_CODE AndAlso CurrTORIF_CODE = TORIF_CODE Then
                    If Item.SubItems(7).Text.Trim = "○" Then
                        MultiCnt += 1
                    End If

                    If Item.SubItems(9).Text.Trim = "○" Then
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
        Dim iret As Integer
        Dim MainDB As CASTCommon.MyOracle = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(落込)開始", "成功", "")

            MainDB = New CASTCommon.MyOracle

            If ListView1.Items.Count <= 0 Then
                Call MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If GCom.GetListViewHasRow(Me.ListView1) <= 0 Then
                    Call MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            If GCom.NzStr(GCom.SelectedItem(ListView1, 7)) = "○" Then
                Call MessageBox.Show(MSG0061W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            '### 落し込みに必要なパラメータの取得
            '取引先コード（１２）2、振込日5、コード区分14、フォーマット区分15、媒体コード13、ラベル区分16
            '持込区分17
            Dim paraToriCode As String = GCom.NzDec(GCom.SelectedItem(ListView1, 2).ToString.Replace("-", ""), "")
            Dim paraFuriDate As String = GCom.NzDec(GCom.SelectedItem(ListView1, 5).ToString.Replace("/", ""), "")
            Dim paraCodeKbn As String = GCom.NzDec(GCom.SelectedItem(ListView1, 15), "")
            Dim ParaFmtKbn As String = GCom.NzDec(GCom.SelectedItem(ListView1, 16), "")
            Dim ParaBaitaiCode As String = GCom.NzDec(GCom.SelectedItem(ListView1, 14), "")
            Dim paraLabelKbn As String = GCom.NzDec(GCom.SelectedItem(ListView1, 17), "")

            '###メッセージボックスの表示に必要な情報の取得
            Dim toriName As String = GCom.NzStr(GCom.SelectedItem(ListView1, 1)).Trim
            Dim ItakuCode As String = GCom.NzDec(GCom.SelectedItem(ListView1, 3), "")
            
            'ログ用変数の設定
            LW.ToriCode = GCom.SelectedItem(ListView1, 2).ToString
            LW.FuriDate = paraFuriDate

            '依頼書･伝票の場合、受付フラグをチェックする
            Dim UKETUKE As String = GCom.NzDec(GCom.SelectedItem(ListView1, 21), "")
            If (ParaFmtKbn = "04" OrElse ParaFmtKbn = "05") AndAlso UKETUKE <> "1" Then
                MessageBox.Show(MSG0342W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            JobID = "S010"
            Param = paraToriCode & "," & paraFuriDate & "," & paraCodeKbn & "," & ParaFmtKbn & "," & ParaBaitaiCode & "," & paraLabelKbn

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
            If MainLOG.InsertJOBMAST(JobID, gstrUSER_ID, Param, MainDB) = False Then
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

End Class