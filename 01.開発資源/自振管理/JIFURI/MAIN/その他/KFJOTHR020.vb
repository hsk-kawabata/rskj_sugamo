Option Explicit On

Imports System
Imports System.Data.OracleClient
Imports CASTCommon

Public Class KFJOTHR020
#Region "宣言"
    Inherits System.Windows.Forms.Form

    Private BatchLog As New CASTCommon.BatchLOG("KFJOTHR020", "振替結果変更画面")
    Private Const ErrMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"


    Private CAST As New CASTCommon.Events
    Private CASTx1 As New CASTCommon.Events(" ", CASTCommon.Events.KeyMode.BAD)
    Private CASTx2 As New CASTCommon.Events("", CASTCommon.Events.KeyMode.BAD)
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private FURIKETU_CODE As String
    Private TORIS_CODE As String
    Private TORIF_CODE As String
    Private FURI_DATE As String
    Private KEIYAKU_KIN As String
    Private KEIYAKU_SIT As String
    Private KEIYAKU_KAMOKU As String
    Private KEIYAKU_KOUZA As String
    Private FURIKIN As String
    Private KIGYO_SEQ As String
    Private KEIYAKU_KNAME As String
    Private RECORD_NO As String
    Private JYUYOUKA_NO As String
    '*** 修正 mitsu 2008/05/26 フォーマット区分追加 ***
    Private FMT_KBN As String
    '**************************************************

    '*** 修正 mitsu 2008/08/14 画面制御用フラグ ***
    Private UpdateFlg As Boolean = False '更新フラグ
    '**********************************************

    '振替結果変更履歴のため、旧振替結果コードを退避
    Private OldFURIKETU_CODE As Integer
    '==============================================

    Private Const msgTitle As String = "振替結果変更(KFJOTHR020)"

    Private LoadAction As Boolean = True

    '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- START
    Private STR_KAMOKU_TXT As String = "KFJOTHR020_科目.TXT"
    Private STR_FURIKETU_TXT As String = "KFJOTHR020_振替結果コード.TXT"
    '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- END

#End Region

    Private Sub KFJOTHR020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)開始", "成功", "")

            GCom.GetSysDate = Date.Now
            GCom.GetUserID = gstrUSER_ID

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------
            '委託者名リストボックスの設定
            '------------------------------------------
            If GCom.SelectItakuName("", cmbToriName, TORIS_CODE_K, TORIF_CODE_K, "1") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- START
            '------------------------------------------
            '科目リストボックスの設定
            '------------------------------------------
            Select Case GCom.SetComboBox(KEIYAKU_KAMOKU_K, STR_KAMOKU_TXT, True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "科目", STR_KAMOKU_TXT), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(コンボボックス設定)", "失敗", "科目設定ファイルなし。ファイル:" & STR_KAMOKU_TXT)
                    Exit Sub
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "科目"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(コンボボックス設定)", "失敗", "科目設定異常。ファイル:" & STR_KAMOKU_TXT)
                    Exit Sub
            End Select
            '------------------------------------------
            '振替結果コードリストボックスの設定
            '------------------------------------------
            Select Case GCom.SetComboBox(FURIKETU_CODE_K, STR_FURIKETU_TXT, True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "振替結果コード", STR_FURIKETU_TXT), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(コンボボックス設定)", "失敗", "振替結果コード設定ファイルなし。ファイル:" & STR_FURIKETU_TXT)
                    Exit Sub
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "振替結果コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(コンボボックス設定)", "失敗", "振替結果コード設定異常。ファイル:" & STR_FURIKETU_TXT)
                    Exit Sub
            End Select
            '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- END

            '休日マスタ取り込み 2008.02.14 Update By Astar
            Dim SubSQL As String = " WHERE SUBSTR(YASUMI_DATE_Y, 1, 6)"
            SubSQL &= " IN ('" & String.Format("{0:yyyyMM}", Date.Now.AddMonths(-1)) & "'"
            SubSQL &= ", '" & String.Format("{0:yyyyMM}", Date.Now) & "'"
            SubSQL &= ", '" & String.Format("{0:yyyyMM}", Date.Now.AddMonths(1)) & "')"
            Dim BRet As Boolean = GCom.CheckDateModule(Nothing, CType(1, Short), SubSQL)

            Me.btnEraser.PerformClick()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)", "失敗", ex.Message)
            Me.Close()

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)終了", "成功", "")
        End Try

    End Sub

    'オプションボタン・クリック
    Private Sub OPTION_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbKouza.CheckedChanged, rdbKigyoSeq.CheckedChanged

        Select Case True
            Case CType(sender, RadioButton).Name.ToUpper = "rdbKigyoSeq".ToUpper

                KIGYO_SEQ_K.Visible = True

                KEIYAKU_KIN_K.Enabled = False
                KEIYAKU_KIN_K.BackColor = System.Drawing.SystemColors.Control
                KEIYAKU_SIT_K.Enabled = False
                KEIYAKU_SIT_K.BackColor = System.Drawing.SystemColors.Control
                KEIYAKU_KAMOKU_K.Enabled = False
                KEIYAKU_KAMOKU_K.BackColor = System.Drawing.SystemColors.Control
                KEIYAKU_KOUZA_K.Enabled = False
                KEIYAKU_KOUZA_K.BackColor = System.Drawing.SystemColors.Control
                FURIKIN_K.Enabled = False
                FURIKIN_K.BackColor = System.Drawing.SystemColors.Control

                Me.GroupBox1.Enabled = False

                FURIKETU_CODE_K.Enabled = True
                FURIKETU_CODE_K.BackColor = System.Drawing.SystemColors.Window

                Me.btnAction.Enabled = False

                Me.KIGYO_SEQ_K.Focus()

            Case CType(sender, RadioButton).Name.ToUpper = "rdbKouza".ToUpper

                KIGYO_SEQ_K.Visible = False

                Me.GroupBox1.Enabled = True

                KEIYAKU_KIN_K.Enabled = True
                KEIYAKU_KIN_K.BackColor = System.Drawing.SystemColors.Window
                KEIYAKU_SIT_K.Enabled = True
                KEIYAKU_SIT_K.BackColor = System.Drawing.SystemColors.Window
                KEIYAKU_KAMOKU_K.Enabled = True
                KEIYAKU_KAMOKU_K.BackColor = System.Drawing.SystemColors.Window
                KEIYAKU_KOUZA_K.Enabled = True
                KEIYAKU_KOUZA_K.BackColor = System.Drawing.SystemColors.Window
                FURIKIN_K.Enabled = True
                FURIKIN_K.BackColor = System.Drawing.SystemColors.Window

                FURIKETU_CODE_K.Enabled = True
                FURIKETU_CODE_K.BackColor = System.Drawing.SystemColors.Window

                Me.btnAction.Enabled = False

                Me.KEIYAKU_KIN_K.Focus()

        End Select

    End Sub

    'ゼロ埋め入力 2008.02.14 By Astar
    Private Sub TextBoxZeroFill_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TORIS_CODE_K.Validating, TORIF_CODE_K.Validating, FURI_DATE_K.Validating, FURI_DATE_K1.Validating, FURI_DATE_K2.Validating, KEIYAKU_KIN_K.Validating, KEIYAKU_SIT_K.Validating, KEIYAKU_KOUZA_K.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

    '数値評価(金額フォーマット) 2008.02.14 By Astar
    Private Sub TextBoxOther_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles KIGYO_SEQ_K.Validating, FURIKIN_K.Validating

        With CType(sender, TextBox)
            Select Case .Name.ToUpper
                Case "FURIKIN_K".ToUpper
                    .Text = GCom.GetLimitString(String.Format("{0:#,##0}", GCom.NzDec(.Text, 0)), 13)
                Case Else
                    .Text = GCom.NzDec(.Text, "")
            End Select
        End With
    End Sub

    '明細マスタの更新処理(ボタン)
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(更新)開始", "成功", "")
            If CheckEntryValue(2, TORIS_CODE_K) Then
                If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    Exit Sub
                End If
                Call UPDATE_MEIMAST_FURIKETU_CODE()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(更新)", "失敗", ex.Message)
            Me.Close()
        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(更新)終了", "成功", "")
        End Try
    End Sub

    '参照ボタン
    Private Sub btnSansyou_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSansyou.Click

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(参照)開始", "成功", "")

            Select Case True
                Case rdbKigyoSeq.Checked
                    '企業シーケンス

                    '入力値のチェック 2008.02.14 Update By Astar
                    If CheckEntryValue(1, FURIKETU_CODE_K) Then

                        '明細マスタ存在チェック
                        If FN_SELECT_MEIMAST_SEQ() Then

                            '検索にヒットしたら
                            Me.GroupBox1.Enabled = True

                            KEIYAKU_KIN_K.ReadOnly = True
                            KEIYAKU_SIT_K.ReadOnly = True
                            KEIYAKU_KAMOKU_K.Enabled = False
                            KEIYAKU_KOUZA_K.ReadOnly = True
                            FURIKIN_K.ReadOnly = True

                            FURIKETU_CODE_K.Focus()
                            btnAction.Enabled = True

                            Return
                        End If
                    Else
                        Return
                    End If
                Case rdbKouza.Checked
                    '口座番号指定

                    '入力値のチェック
                    If CheckEntryValue(2, FURIKETU_CODE_K) Then

                        '明細マスタ存在チェック
                        If FN_SELECT_MEIMAST() Then

                            KEIYAKU_KIN_K.ReadOnly = False
                            KEIYAKU_SIT_K.ReadOnly = False
                            KEIYAKU_KAMOKU_K.Enabled = True
                            KEIYAKU_KOUZA_K.ReadOnly = False
                            FURIKIN_K.ReadOnly = False

                            FURIKETU_CODE_K.Focus()
                            btnAction.Enabled = True
                            Return
                        End If
                    Else
                        Return
                    End If

            End Select

            MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(参照)", "失敗", ex.Message)
            Me.Close()
        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(参照)終了", "成功", "")
        End Try
    End Sub

    '取消ボタン
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消)開始", "成功", "")
            For Each CTL As Control In Me.Controls
                '*** 修正 mitsu 2008/08/14 更新フラグがtrueのときは委託者情報をクリアしない ***
                If UpdateFlg = False OrElse ( _
                    CTL.Name <> "TORI_NAME_L" AndAlso _
                    CTL.Name <> "TORIS_CODE_K" AndAlso _
                    CTL.Name <> "TORIF_CODE_K" AndAlso _
                    CTL.Name <> "FURI_DATE_K" AndAlso _
                    CTL.Name <> "FURI_DATE_K1" AndAlso _
                    CTL.Name <> "FURI_DATE_K2") Then

                    If TypeOf CTL Is TextBox Then

                        CType(CTL, TextBox).Clear()

                    ElseIf TypeOf CTL Is Label Then

                        With CType(CTL, Label)
                            If GCom.NzInt(.Tag) = 1 Then
                                .Text = ""
                            End If
                        End With

                    ElseIf TypeOf CTL Is GroupBox Then

                        For Each OBJ As Control In CTL.Controls

                            If TypeOf OBJ Is TextBox Then

                                CType(OBJ, TextBox).Clear()

                            ElseIf TypeOf OBJ Is Label Then

                                With CType(OBJ, Label)
                                    If GCom.NzInt(.Tag) = 1 Then
                                        .Text = ""
                                    End If
                                End With

                            ElseIf TypeOf OBJ Is ComboBox Then

                                CType(OBJ, ComboBox).SelectedIndex = 0

                            End If
                        Next OBJ
                    End If
                End If
                '******************************************************************************
            Next CTL

            '*** 修正 mitsu 2008/08/14 クリア後フラグを戻す ***
            UpdateFlg = False
            '**************************************************

            Me.rdbKouza.Checked = True
            Application.DoEvents()
            Me.rdbKigyoSeq.Checked = True

            Me.btnAction.Enabled = False

            KEIYAKU_KIN_K.ReadOnly = False
            KEIYAKU_SIT_K.ReadOnly = False
            KEIYAKU_KAMOKU_K.Enabled = True
            KEIYAKU_KOUZA_K.ReadOnly = False
            FURIKIN_K.ReadOnly = False

            FURIKETU_CODE_K.Focus()
            '*** 修正 mitsu 2008/08/14 不要 ***
            'CmdUpdate.Enabled = True
            '**********************************


            TORIS_CODE_K.Text = ""
            TORIF_CODE_K.Text = ""

            '表示委託先の検索（スケジュールの検索）
            Dim SetupDate As String = ""
            Dim BRet As Boolean = GCom.CheckDateModule(String.Format("{0:yyyyMMdd}", Date.Now), SetupDate, 1, 1)

            Me.FURI_DATE_K.Text = SetupDate.Substring(0, 4)
            Me.FURI_DATE_K1.Text = SetupDate.Substring(4, 2)
            Me.FURI_DATE_K2.Text = SetupDate.Substring(6)

            Me.rdbKigyoSeq.Checked = True

            If Not LoadAction Then
                Me.TORIS_CODE_K.Focus()
            End If
            LoadAction = False

            cmbKana.SelectedIndex = 0
            cmbToriName.SelectedIndex = 0

            '取引先主コードにフォーカスをあてる
            TORIS_CODE_K.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消)", "失敗", ex.Message)
            Me.Close()
        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消)終了", "成功", "")
        End Try
    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了)", "失敗", ex.Message)
            Me.Close()
        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了)終了", "成功", "")

        End Try

    End Sub

    '
    ' 機能　 ： 科目変換関数
    '
    ' 引数　 ： ARG1 - 選択識別
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： 共通化 2008.02.14 By Astar
    '
    Private Sub GET_SET_KAMOKU_CODE()

        Select Case GCom.NzDec(KEIYAKU_KAMOKU, "").Length = 0
            Case True
                '科目コードが入っていない場合にはコンボボックスから変数値を設定する
                '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- START
                KEIYAKU_KAMOKU = GCom.GetComboBox(KEIYAKU_KAMOKU_K).ToString.PadLeft(2, "0"c)
                'Select Case CStr(KEIYAKU_KAMOKU_K.SelectedItem)
                '    Case "普通"
                '        KEIYAKU_KAMOKU = "02"
                '    Case "当座"
                '        KEIYAKU_KAMOKU = "01"
                '    Case "職員"
                '        KEIYAKU_KAMOKU = "37"
                '    Case "納税"
                '        KEIYAKU_KAMOKU = "05"
                '    Case "その他"
                '        KEIYAKU_KAMOKU = "09"
                '    Case Else
                '        KEIYAKU_KAMOKU = ""
                'End Select
                '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- END
            Case Else
                '科目コードが入っている場合にはコンボボックスを設定する
                '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- START
                Dim Cnt As Integer = 0
                For Cnt = 0 To KEIYAKU_KAMOKU_K.Items.Count - 1 Step 1
                    KEIYAKU_KAMOKU_K.SelectedIndex = Cnt
                    If GCom.GetComboBox(KEIYAKU_KAMOKU_K) = KEIYAKU_KAMOKU Then
                        Exit For
                    End If
                Next Cnt

                If Cnt >= KEIYAKU_KAMOKU_K.Items.Count AndAlso KEIYAKU_KAMOKU_K.Items.Count > 0 Then
                    KEIYAKU_KAMOKU_K.SelectedIndex = -1
                End If
                'Select Case KEIYAKU_KAMOKU
                '    Case "02"
                '        KEIYAKU_KAMOKU_K.SelectedItem = "普通"
                '    Case "01"
                '        KEIYAKU_KAMOKU_K.SelectedItem = "当座"
                '    Case "37"
                '        KEIYAKU_KAMOKU_K.SelectedItem = "職員"
                '    Case "05"
                '        KEIYAKU_KAMOKU_K.SelectedItem = "納税"
                '    Case "09"
                '        KEIYAKU_KAMOKU_K.SelectedItem = "その他"
                'End Select
                '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- END
        End Select
    End Sub

    '
    ' 機能　 ： 振替結果変換関数
    '
    ' 引数　 ： ARG1 - なし
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： 共通化 2008.02.14 By Astar
    '
    Private Sub GET_SET_FURIKETU_CODE()

        Select Case GCom.NzDec(FURIKETU_CODE, "").Length = 0
            Case True
                '振替結果コードが入っていない場合にはコンボボックスから変数値を設定する
                '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- START
                FURIKETU_CODE = GCom.GetComboBox(FURIKETU_CODE_K)
                'Select Case CStr(FURIKETU_CODE_K.SelectedItem)
                '    Case "０：振替済み"
                '        FURIKETU_CODE = "0"
                '    Case "１：資金不足"
                '        FURIKETU_CODE = "1"
                '    Case "２：取引なし"
                '        FURIKETU_CODE = "2"
                '    Case "３：預金者都合"
                '        FURIKETU_CODE = "3"
                '    Case "４：依頼書なし"
                '        FURIKETU_CODE = "4"
                '    Case "８：委託者都合"
                '        FURIKETU_CODE = "8"
                '    Case "９：その他"
                '        FURIKETU_CODE = "9"
                '    Case Else
                '        FURIKETU_CODE = ""
                'End Select
                '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- END
            Case Else
                '振替結果コードが入っている場合にはコンボボックスを設定する
                '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- START
                Dim Cnt As Integer = 0
                For Cnt = 0 To FURIKETU_CODE_K.Items.Count - 1 Step 1
                    FURIKETU_CODE_K.SelectedIndex = Cnt
                    If GCom.GetComboBox(FURIKETU_CODE_K) = FURIKETU_CODE Then
                        Exit For
                    End If
                Next Cnt

                If Cnt >= FURIKETU_CODE_K.Items.Count AndAlso FURIKETU_CODE_K.Items.Count > 0 Then
                    FURIKETU_CODE_K.SelectedIndex = -1
                End If
                'Select Case FURIKETU_CODE
                '    Case "0"
                '        FURIKETU_CODE_K.SelectedItem = "０：振替済み"
                '    Case "1"
                '        FURIKETU_CODE_K.SelectedItem = "１：資金不足"
                '    Case "2"
                '        FURIKETU_CODE_K.SelectedItem = "２：取引なし"
                '    Case "3"
                '        FURIKETU_CODE_K.SelectedItem = "３：預金者都合"
                '    Case "4"
                '        FURIKETU_CODE_K.SelectedItem = "４：依頼書なし"
                '    Case "8"
                '        FURIKETU_CODE_K.SelectedItem = "８：委託者都合"
                '    Case "9"
                '        FURIKETU_CODE_K.SelectedItem = "９：その他"
                'End Select
                '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- END
        End Select
    End Sub

    '
    ' 機能　 ： 入力値の検証関数
    '
    ' 引数　 ： ARG1 - 選択識別
    ' 　　　 　 ARG2 - 終了時のフォーカス設定先
    '
    ' 戻り値 ： OK = True, NG = False
    '
    ' 備考　 ： 共通化 2008.02.14 By Astar
    '
    Private Function CheckEntryValue(ByVal SEL As Integer, ByVal ARG1 As Control) As Boolean

        Dim Temp As String
        Dim CTL As Control = ARG1
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim REC As OracleDataReader = Nothing
        'Dim REC As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        Try

            Temp = GCom.NzDec(TORIS_CODE_K.Text.Trim, "")
            If Temp.Length = 10 Then
                TORIS_CODE = Temp
            Else
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                CTL = TORIS_CODE_K
                Return False
            End If

            Temp = GCom.NzDec(TORIF_CODE_K.Text.Trim, "")
            If Temp.Length = 2 Then
                TORIF_CODE = Temp
            Else
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                CTL = TORIF_CODE_K
                Return False
            End If

            Dim SQL As String = "SELECT ITAKU_NNAME_T"
            '*** 修正 mitsu 2008/05/26 フォーマット区分取得 ***
            SQL &= ", FMT_KBN_T"
            '**************************************************
            SQL &= " FROM TORIMAST"
            SQL &= " WHERE TORIS_CODE_T = '" & TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_T = '" & TORIF_CODE & "'"

            If GCom.SetDynaset(SQL, REC) AndAlso REC.Read Then

                '*** 修正 mitsu 2008/05/26 フォーマット区分取得 ***
                FMT_KBN = GCom.NzStr(REC.Item("FMT_KBN_T"))
                '**************************************************

                If Not REC Is Nothing Then
                    REC.Close()
                    REC.Dispose()
                End If
            Else
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                CTL = TORIS_CODE_K
                Return False
            End If


            '年必須チェック
            If FURI_DATE_K.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_DATE_K.Focus()
                Return False
            End If
            '月必須チェック
            If FURI_DATE_K1.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_DATE_K1.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(FURI_DATE_K1.Text.Trim) < 1 OrElse GCom.NzInt(FURI_DATE_K1.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_DATE_K1.Focus()
                Return False
            End If

            '日付必須チェック
            If FURI_DATE_K2.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_DATE_K2.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(FURI_DATE_K2.Text.Trim) < 1 OrElse GCom.NzInt(FURI_DATE_K2.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_DATE_K2.Focus()
                Return False
            End If

            Dim onDate As Date
            Dim onText(2) As Integer
            onText(0) = GCom.NzInt(FURI_DATE_K.Text, 0)
            onText(1) = GCom.NzInt(FURI_DATE_K1.Text, 0)
            onText(2) = GCom.NzInt(FURI_DATE_K2.Text, 0)
            Dim Ret As Integer = GCom.SET_DATE(onDate, onText)
            Select Case Ret
                Case Is = -1
                    FURI_DATE = String.Format("{0:yyyyMMdd}", onDate)
                Case Is = 1
                    MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    CTL = FURI_DATE_K1
                    Return False
                Case Is = 2
                    MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    CTL = FURI_DATE_K2
                    Return False
                Case Else
                    MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    CTL = FURI_DATE_K
                    Return False
            End Select

            Select Case SEL
                Case 1
                    Temp = GCom.NzDec(KIGYO_SEQ_K.Text.Trim, "")
                    If Temp.Length > 0 Then
                        KIGYO_SEQ = Temp
                    Else
                        MessageBox.Show(MSG0142W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        CTL = KIGYO_SEQ_K
                        Return False
                    End If
                Case 2
                    Temp = GCom.NzDec(KEIYAKU_KIN_K.Text.Trim, "")
                    If Temp.Length = 4 Then
                        KEIYAKU_KIN = Temp
                    Else
                        MessageBox.Show(MSG0032W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        CTL = KEIYAKU_KIN_K
                        Return False
                    End If

                    Temp = GCom.NzDec(KEIYAKU_SIT_K.Text.Trim, "")
                    If Temp.Length = 3 Then
                        KEIYAKU_SIT = Temp
                    Else
                        MessageBox.Show(MSG0035W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        CTL = KEIYAKU_SIT_K
                        Return False
                    End If

                    KEIYAKU_KAMOKU = ""
                    Call GET_SET_KAMOKU_CODE()

                    Temp = GCom.NzDec(KEIYAKU_KOUZA_K.Text.Trim, "")
                    If Temp.Length = 7 Then
                        KEIYAKU_KOUZA = Temp
                    Else
                        MessageBox.Show(MSG0123W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        CTL = KEIYAKU_KOUZA_K
                        Return False
                    End If

                    Temp = GCom.NzDec(FURIKIN_K.Text.Trim, "")
                    If Temp.Length > 0 Then
                        FURIKIN = Temp
                    Else
                        MessageBox.Show(MSG0140W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        CTL = FURIKIN_K
                        Return False
                    End If
                Case 3
                    Temp = GCom.NzDec(KEIYAKU_KIN_K.Text.Trim, "")
                    If Temp.Length = 4 Then
                        KEIYAKU_KIN = Temp
                    Else
                        MessageBox.Show(MSG0032W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        CTL = KEIYAKU_KIN_K
                        Return False
                    End If

                    Temp = GCom.NzDec(KEIYAKU_SIT_K.Text.Trim, "")
                    If Temp.Length = 3 Then
                        KEIYAKU_SIT = Temp
                    Else
                        KEIYAKU_SIT = ""
                    End If
            End Select

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(入力値検証)", "失敗", ex.Message)
            Me.Close()
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not CTL Is Nothing Then
                CTL.Focus()
            End If
        End Try

        Return False
    End Function

    '=====================================================================================
    'NAME           :FN_SELECT_MEIMAST
    'Description    :明細マスタ検索
    'Return         :True=OK(検索ヒット),False=NG（検索失敗）
    'Create         :2004/08/30
    'Update         :2008.02.14 Update By Astar
    '=====================================================================================
    Private Function FN_SELECT_MEIMAST() As Boolean

        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim REC As OracleDataReader = Nothing
        'Dim REC As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        Try

            Call GET_SET_KAMOKU_CODE()

            Dim SQL As String = "SELECT * FROM MEIMAST"
            SQL &= " WHERE TORIS_CODE_K = '" & TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_K = '" & TORIF_CODE & "'"
            '*** 修正 mitsu 2008/05/26 国税対応 ***
            If FMT_KBN = "02" Then
                SQL &= " AND DATA_KBN_K = '3'"
            Else
                SQL &= " AND DATA_KBN_K = '2'"
            End If
            '**************************************
            SQL &= " AND FURI_DATE_K = '" & FURI_DATE & "'"
            SQL &= " AND KEIYAKU_KIN_K = '" & KEIYAKU_KIN & "'"
            SQL &= " AND KEIYAKU_SIT_K = '" & KEIYAKU_SIT & "'"

            '2008.02.14 Update By Astar
            SQL &= " AND KEIYAKU_KAMOKU_K IN ('" & KEIYAKU_KAMOKU & "',"
            SQL &= " '" & GCom.NzInt(KEIYAKU_KAMOKU).ToString & "')"

            SQL &= " AND KEIYAKU_KOUZA_K = '" & KEIYAKU_KOUZA & "'"
            SQL &= " AND FURIKIN_K = '" & FURIKIN & "'"

            Dim RecordCounter As Integer = 0

            If GCom.SetDynaset(SQL, REC) Then
                Do While REC.Read

                    RecordCounter += 1

                    If RecordCounter > 1 Then

                        Dim DRet As DialogResult = MessageBox.Show(String.Format(MSG0026I, KEIYAKU_KIN, KEIYAKU_SIT, KEIYAKU_KAMOKU_K.Text, KEIYAKU_KOUZA, FURIKIN, KEIYAKU_KNAME, RECORD_NO, KIGYO_SEQ), msgTitle, _
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

                        If DRet = DialogResult.Yes Then
                            Exit Do
                        End If
                    End If

                    KEIYAKU_KNAME = GCom.NzStr(REC.Item("KEIYAKU_KNAME_K")).Trim
                    RECORD_NO = GCom.NzDec(REC.Item("RECORD_NO_K"), "")
                    FURIKETU_CODE = GCom.NzDec(REC.Item("FURIKETU_CODE_K"), "")
                    '2013/08/15 saitou 標準修正 ADD -------------------------------------------------->>>>
                    '参照時の振替結果コードを退避
                    OldFURIKETU_CODE = FURIKETU_CODE
                    '2013/08/15 saitou 標準修正 ADD --------------------------------------------------<<<<
                    KIGYO_SEQ = GCom.NzDec(REC.Item("KIGYO_SEQ_K"), "")
                    JYUYOUKA_NO = GCom.NzDec(REC.Item("JYUYOUKA_NO_K"), "")
                Loop

                If RecordCounter > 0 Then

                    KIGYO_SEQ_K.Text = KIGYO_SEQ

                    KEIYAKU_KNAME_K.Text = KEIYAKU_KNAME
                    RECORD_NO_K.Text = String.Format("{0:000000}", GCom.NzInt(RECORD_NO, 0))

                    Call GET_SET_FURIKETU_CODE()
                    Return True
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(明細マスタ検索)", "失敗", ex.Message)
            Me.Close()
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return False
    End Function

    '=====================================================================================
    'NAME           :FN_SELECT_MEIMAST_SEQ
    'Description    :明細マスタ検索
    'Return         :True=OK(検索ヒット),False=NG（検索失敗）
    'Create         :2004/08/30
    'Update         :2008.02.14 Update By Astar
    '=====================================================================================
    Private Function FN_SELECT_MEIMAST_SEQ() As Boolean
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim REC As OracleDataReader = Nothing
        'Dim REC As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        Try

            Dim SQL As String = "SELECT * FROM MEIMAST"
            SQL &= " WHERE TORIS_CODE_K = '" & TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_K = '" & TORIF_CODE & "'"
            SQL &= " AND FURI_DATE_K = '" & FURI_DATE & "'"
            SQL &= " AND KIGYO_SEQ_K = '" & KIGYO_SEQ & "'"
            '*** 修正 mitsu 2008/05/26 国税対応 ***
            If FMT_KBN = "02" Then
                SQL &= " AND DATA_KBN_K = '3'"
            Else
                SQL &= " AND DATA_KBN_K = '2'"
            End If
            '**************************************

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)

            Dim RecordCounter As Integer = 0

            If BRet Then
                Do While REC.Read

                    RecordCounter += 1

                    If RecordCounter > 1 Then

                        Dim DRet As DialogResult = MessageBox.Show(String.Format(MSG0026I, KEIYAKU_KIN, KEIYAKU_SIT, KEIYAKU_KAMOKU_K.Text, KEIYAKU_KOUZA, FURIKIN, KEIYAKU_KNAME, RECORD_NO, KIGYO_SEQ), msgTitle, _
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

                        If DRet = DialogResult.Yes Then
                            Exit Do
                        End If
                    End If

                    KEIYAKU_KIN = GCom.NzDec(REC.Item("KEIYAKU_KIN_K"), "")
                    KEIYAKU_SIT = GCom.NzDec(REC.Item("KEIYAKU_SIT_K"), "")
                    KEIYAKU_KAMOKU = GCom.NzDec(REC.Item("KEIYAKU_KAMOKU_K"), "")
                    KEIYAKU_KOUZA = GCom.NzDec(REC.Item("KEIYAKU_KOUZA_K"), "")
                    FURIKIN = GCom.NzDec(REC.Item("FURIKIN_K"), "")
                    KEIYAKU_KNAME = GCom.NzStr(REC.Item("KEIYAKU_KNAME_K")).Trim
                    RECORD_NO = GCom.NzDec(REC.Item("RECORD_NO_K"), "")
                    FURIKETU_CODE = GCom.NzDec(REC.Item("FURIKETU_CODE_K"), "")
                    OldFURIKETU_CODE = FURIKETU_CODE '参照時点での振替結果コードを退避
                    JYUYOUKA_NO = GCom.NzDec(REC.Item("JYUYOUKA_NO_K"), "")
                Loop

                If RecordCounter > 0 Then

                    '各値を代入
                    KEIYAKU_KIN_K.Text = KEIYAKU_KIN
                    KEIYAKU_SIT_K.Text = KEIYAKU_SIT

                    Call GET_SET_KAMOKU_CODE()

                    KEIYAKU_KOUZA_K.Text = KEIYAKU_KOUZA
                    FURIKIN_K.Text = String.Format("{0:#,##0}", GCom.NzDec(FURIKIN, ""))
                    KEIYAKU_KNAME_K.Text = KEIYAKU_KNAME
                    RECORD_NO_K.Text = String.Format("{0:000000}", GCom.NzInt(RECORD_NO, 0))

                    Call GET_SET_FURIKETU_CODE()
                    Return True
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(明細マスタ検索)", "失敗", ex.Message)
            Return False
        Finally

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

        End Try

        Return False

    End Function

    '=====================================================================================
    'NAME           :UPDATE_MEIMAST_FURIKETU_CODE
    'Description    :明細マスタ更新
    'Return         :Nothing
    'Create         :2004/08/30
    'Update         :2008.02.14 Uodate By Astar
    '=====================================================================================
    Private Sub UPDATE_MEIMAST_FURIKETU_CODE()

        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        'Dim MSG As String
        'Dim DRet As DialogResult
        Dim REC As OracleDataReader = Nothing
        'Dim REC As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Try

            FURIKETU_CODE = ""
            Call GET_SET_FURIKETU_CODE()

            Dim SQL As String = "UPDATE MEIMAST"
            SQL &= " SET FURIKETU_CODE_K = '" & FURIKETU_CODE & "'"
            SQL &= " WHERE TORIS_CODE_K = '" & TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_K = '" & TORIF_CODE & "'"
            SQL &= " AND FURI_DATE_K = '" & FURI_DATE & "'"
            SQL &= " AND KEIYAKU_KIN_K = '" & KEIYAKU_KIN & "'"
            SQL &= " AND KEIYAKU_SIT_K = '" & KEIYAKU_SIT & "'"
            SQL &= " AND KEIYAKU_KAMOKU_K = '" & KEIYAKU_KAMOKU & "'"
            SQL &= " AND KEIYAKU_KOUZA_K = '" & KEIYAKU_KOUZA & "'"
            SQL &= " AND FURIKIN_K = '" & FURIKIN & "'"
            SQL &= " AND RECORD_NO_K = '" & RECORD_NO & "'"

            Dim SQLCode As Integer
            '2012/09/04 saitou 警告解除 MODIFY -------------------------------------------------->>>>
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode, True)
            'Dim Ret As Integer = GCom.DBExecuteProcess(GCom.enDB.DB_Execute, SQL, SQLCode, True)
            '2012/09/04 saitou 警告解除 MODIFY --------------------------------------------------<<<<
            If Ret = 1 AndAlso SQLCode = 0 Then
                '振替結果変更履歴マスタに登録する==================================
                If FURIKETU_CODE <> OldFURIKETU_CODE Then
                    Dim HenkouCnt As Integer
                    SQL = "SELECT NVL(MAX(HENKO_NO_HK) +1,0) COUNTER"
                    SQL &= " FROM KEKKA_RIREKIMAST"
                    SQL &= " WHERE  TORIS_CODE_HK = '" & TORIS_CODE & "'"
                    SQL &= " AND TORIF_CODE_HK = '" & TORIF_CODE & "'"
                    SQL &= " AND FURI_DATE_HK = '" & FURI_DATE & "'"
                    SQL &= " AND RECORD_NO_HK = " & RECORD_NO
                    SQL &= " AND KOUSIN_DATE_HK = '" & GCom.GetSysDate.ToString("yyyyMMdd") & "'"

                    Dim KRet As Boolean = GCom.SetDynaset(SQL, REC)
                    If KRet AndAlso REC.Read Then
                        HenkouCnt = GCom.NzDec(REC.Item("COUNTER"), "")
                    End If
                    If Not REC Is Nothing Then
                        REC.Close()
                        REC.Dispose()
                    End If
                    If Not HenkouCnt > 99 Then
                        SQL = "INSERT INTO KEKKA_RIREKIMAST VALUES("
                        SQL &= "'" & TORIS_CODE & "',"
                        SQL &= "'" & TORIF_CODE & "',"
                        SQL &= "'" & FURI_DATE & "',"
                        SQL &= RECORD_NO & ","
                        SQL &= "'" & GCom.GetSysDate.ToString("yyyyMMdd") & "',"
                        SQL &= OldFURIKETU_CODE & ","
                        SQL &= FURIKETU_CODE & ","
                        SQL &= HenkouCnt
                        SQL &= ")"

                        '2012/09/04 saitou 警告解除 MODIFY -------------------------------------------------->>>>
                        Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode, True)
                        'Ret = GCom.DBExecuteProcess(GCom.enDB.DB_Execute, SQL, SQLCode, True)
                        '2012/09/04 saitou 警告解除 MODIFY --------------------------------------------------<<<<
                        If Ret = 1 AndAlso SQLCode = 0 Then
                            '変更履歴マスタ登録完了
                        Else
                            '変更履歴登録失敗
                            MessageBox.Show(MSG0002E.Replace("{0}", "登録"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Else
                        '変更の限度数を超えた場合
                        MessageBox.Show(MSG0246W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
                '================================
                Dim BAITAI_CODE As String = ""

                '媒体コード確認(学校自振か否か)
                SQL = "SELECT BAITAI_CODE_T"
                SQL &= " FROM TORIMAST"
                SQL &= " WHERE FSYORI_KBN_T = '1'"
                SQL &= " AND TORIS_CODE_T = '" & TORIS_CODE & "'"
                SQL &= " AND TORIF_CODE_T = '" & TORIF_CODE & "'"
                Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
                If BRet AndAlso REC.Read Then
                    BAITAI_CODE = GCom.NzDec(REC.Item("BAITAI_CODE_T"), "")
                End If
                If Not REC Is Nothing Then
                    REC.Close()
                    REC.Dispose()
                End If
                If BAITAI_CODE = "07" Then
                    SQL = "UPDATE G_MEIMAST"
                    SQL &= " SET FURIKETU_CODE_M = '" & FURIKETU_CODE & "'"
                    SQL &= " WHERE GAKKOU_CODE_M = '" & TORIS_CODE & "'"
                    SQL &= " AND FURI_DATE_M = '" & FURI_DATE & "'"
                    SQL &= " AND TKIN_NO_M = '" & KEIYAKU_KIN & "'"
                    SQL &= " AND TSIT_NO_M = '" & KEIYAKU_SIT & "'"
                    SQL &= " AND TKAMOKU_M = '" & KEIYAKU_KAMOKU & "'"
                    SQL &= " AND TKOUZA_M = '" & KEIYAKU_KOUZA & "'"

                    '2008.03.26 By Astar 梶(FJH)指示
                    'SQL &= " AND RECORD_NO_M = '" & RECORD_NO & "'"

                    SQL &= " AND JUYOUKA_NO_M = '" & JYUYOUKA_NO & "'"
                    '2012/09/04 saitou 警告解除 MODIFY -------------------------------------------------->>>>
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode, True)
                    'Ret = GCom.DBExecuteProcess(GCom.enDB.DB_Execute, SQL, SQLCode, True)
                    '2012/09/04 saitou 警告解除 MODIFY --------------------------------------------------<<<<
                    If Ret = 1 AndAlso SQLCode = 0 Then
                        '*** 修正 mitsu 2008/05/26 振替結果変更をスケジュールに反映する ***
                        'SCHMAST,G_SCHMASTを更新する
                        Ret = GCom.ReCalcSchmastTotal("1", TORIS_CODE, TORIF_CODE, FURI_DATE)

                        '他行の場合はTAKOSCHMAST,G_TAKOUSCHMASTを更新する
                        If Ret > -1 AndAlso KEIYAKU_KIN <> GCom.JIKINKOCD Then
                            Ret = GCom.ReCalcTakoSchmastTotal("1", TORIS_CODE, TORIF_CODE, FURI_DATE, KEIYAKU_KIN)
                        End If

                        '更新失敗時
                        If Ret = -1 Then
                            MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                        End If
                        '******************************************************************

                        MessageBox.Show(MSG0027I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                        Me.btnAction.Enabled = False
                        '*** 修正 mitsu 2008/08/14 更新フラグ追加 ***
                        UpdateFlg = True
                        '********************************************
                        'Me.btnEraser.PerformClick()
                        '
                        Return
                    Else
                        MessageBox.Show(MSG0247W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                Else
                    '*** 修正 mitsu 2008/05/26 振替結果変更をスケジュールに反映する ***
                    'SCHMAST,G_SCHMASTを更新する
                    Ret = GCom.ReCalcSchmastTotal("1", TORIS_CODE, TORIF_CODE, FURI_DATE)

                    '他行の場合はTAKOSCHMAST,G_TAKOUSCHMASTを更新する
                    If Ret > -1 AndAlso KEIYAKU_KIN <> GCom.JIKINKOCD Then
                        Ret = GCom.ReCalcTakoSchmastTotal("1", TORIS_CODE, TORIF_CODE, FURI_DATE, KEIYAKU_KIN)
                    End If

                    '更新失敗時
                    If Ret = -1 Then
                        MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End If
                    '******************************************************************

                    MessageBox.Show(MSG0027I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    Me.btnAction.Enabled = False
                    '*** 修正 mitsu 2008/08/14 更新フラグ追加 ***
                    UpdateFlg = True
                    '********************************************

                    '2011/06/16 標準版修正 更新処理後の画面制御変更 ------------------START
                    Me.btnAction.Enabled = False
                    '取引先主コードにフォーカスをあてる
                    TORIS_CODE_K.Focus()
                    'Me.btnEraser.PerformClick()
                    Return
                    '2011/06/16 標準版修正 更新処理後の画面制御変更 ------------------END

                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(更新)", "失敗", ex.Message)
            Me.Close()
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try
    End Sub

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, TORIS_CODE_K, TORIF_CODE_K, "1") = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
            cmbToriName.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(リストボックス設定)", "失敗", ex.Message)
        Finally

        End Try
    End Sub

    '取引先コードテキストボックスに取引先コード設定
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        Try

            ''=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            ''取引先コードを取得
            ''=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, TORIS_CODE_K, TORIF_CODE_K)
                '2016/12/08 saitou RSV2 ADD メンテナンス ---------------------------------------- START
                'フォーカス移動追加
                Me.FURI_DATE_K.Focus()
                '2016/12/08 saitou RSV2 ADD ----------------------------------------------------- END
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(リストボックス設定)", "失敗", ex.Message)

        Finally

        End Try
    End Sub

End Class