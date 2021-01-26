Option Strict On
Option Explicit On

Imports System
Imports System.Data.OracleClient
Imports CASTCommon

Public Class KFSOTHR040

#Region "宣言"

    Private BatchLog As New CASTCommon.BatchLOG("KFSOTHR040", "為替明細発信停止画面")


    Private CAST As New CASTCommon.Events
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
    Private MOTIKOMI_SEQ As String
    Private KEIYAKU_KNAME As String
    Private RECORD_NO As String
    Private JYUYOUKA_NO As String
    Private FMT_KBN As String

    Private UpdateFlg As Boolean = False '更新フラグ

    Private OldFURIKETU_CODE As Integer

    Private Const msgTitle As String = "為替明細発信停止(KFSOTHR040)"

    Private LoadAction As Boolean = True

    '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- START
    Private STR_KAMOKU_TXT As String = "KFSOTHR040_科目.TXT"
    Private STR_KAWASECODE_TXT As String = "KFSOTHR040_為替発信コード.TXT"
    '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- END

#End Region

    Private Sub KFSOTHR040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)開始", "成功", "")

            GCom.GetSysDate = Date.Now
            GCom.GetUserID = gstrUSER_ID

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------
            '委託者名リストボックスの設定
            '------------------------------------------
            If GCom.SelectItakuName("", cmbToriName, TORIS_CODE_K, TORIF_CODE_K, "3") = -1 Then
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
            '為替発信コードリストボックスの設定
            '------------------------------------------
            Select Case GCom.SetComboBox(FURIKETU_CODE_K, STR_KAWASECODE_TXT, True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "為替発信コード", STR_KAWASECODE_TXT), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(コンボボックス設定)", "失敗", "為替発信コード設定ファイルなし。ファイル:" & STR_KAWASECODE_TXT)
                    Exit Sub
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "為替発信コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(コンボボックス設定)", "失敗", "為替発信コード設定異常。ファイル:" & STR_KAWASECODE_TXT)
                    Exit Sub
            End Select
            '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- END

            '休日マスタ取り込み
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

    'ゼロ埋め入力
    Private Sub TextBoxZeroFill_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TORIS_CODE_K.Validating, TORIF_CODE_K.Validating, FURI_DATE_K.Validating, FURI_DATE_K1.Validating, FURI_DATE_K2.Validating, KEIYAKU_KIN_K.Validating, KEIYAKU_SIT_K.Validating, KEIYAKU_KOUZA_K.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

    '数値評価(金額フォーマット)
    Private Sub TextBoxOther_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles FURIKIN_K.Validating

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
                '更新フラグがtrueのときは委託者情報をクリアしない
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
            Next CTL

            'クリア後フラグを戻す
            UpdateFlg = False

            Application.DoEvents()

            Me.btnAction.Enabled = False

            KEIYAKU_KIN_K.ReadOnly = False
            KEIYAKU_SIT_K.ReadOnly = False
            KEIYAKU_KAMOKU_K.Enabled = True
            KEIYAKU_KOUZA_K.ReadOnly = False
            FURIKIN_K.ReadOnly = False

            FURIKETU_CODE_K.Focus()

            TORIS_CODE_K.Text = ""
            TORIF_CODE_K.Text = ""

            '表示委託先の検索（スケジュールの検索）
            Dim SetupDate As String = ""
            '2011/06/16 標準版修正 初期値表示なし ------------------START
            'Dim BRet As Boolean = GCom.CheckDateModule(String.Format("{0:yyyyMMdd}", Date.Now), SetupDate, 1, 1)
            'Me.FURI_DATE_K.Text = SetupDate.Substring(0, 4)
            'Me.FURI_DATE_K1.Text = SetupDate.Substring(4, 2)
            'Me.FURI_DATE_K2.Text = SetupDate.Substring(6)
            '2011/06/16 標準版修正 初期値表示なし ------------------END

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
                '振替結果コードが入っていない場合にはコンボボックスから変数値を設定する
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
                    If GCom.GetComboBox(KEIYAKU_KAMOKU_K) = CInt(KEIYAKU_KAMOKU) Then
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
    ' 機能　 ： 為替発信変換関数
    '
    ' 引数　 ： ARG1 - なし
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ：
    '
    Private Sub GET_SET_FURIKETU_CODE()

        Select Case GCom.NzDec(FURIKETU_CODE, "").Length = 0
            Case True
                '為替発信コードが入っていない場合にはコンボボックスから変数値を設定する
                '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- START
                FURIKETU_CODE = GCom.GetComboBox(FURIKETU_CODE_K).ToString
                'Select Case CStr(FURIKETU_CODE_K.SelectedItem)
                '    Case "０：為替発信対象"
                '        FURIKETU_CODE = "0"
                '    Case "１：為替停止"
                '        FURIKETU_CODE = "9"
                '    Case Else
                '        FURIKETU_CODE = ""
                'End Select
                '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- END
            Case Else
                '為替発信コードが入っている場合にはコンボボックスを設定する
                '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- START
                Dim Cnt As Integer = 0
                For Cnt = 0 To FURIKETU_CODE_K.Items.Count - 1 Step 1
                    FURIKETU_CODE_K.SelectedIndex = Cnt
                    If GCom.GetComboBox(FURIKETU_CODE_K) = CInt(FURIKETU_CODE) Then
                        Exit For
                    End If
                Next Cnt

                If Cnt >= FURIKETU_CODE_K.Items.Count AndAlso FURIKETU_CODE_K.Items.Count > 0 Then
                    FURIKETU_CODE_K.SelectedIndex = -1
                End If
                'Select Case FURIKETU_CODE
                '    Case "0"
                '        FURIKETU_CODE_K.SelectedItem = "０：為替発信対象"
                '    Case Else
                '        FURIKETU_CODE_K.SelectedItem = "１：為替停止"
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
    ' 備考　 ：
    '
    Private Function CheckEntryValue(ByVal SEL As Integer, ByVal ARG1 As Control) As Boolean

        Dim Temp As String
        Dim CTL As Control = ARG1
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Dim REC As OracleDataReader = Nothing

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
            SQL &= ", FMT_KBN_T"
            SQL &= " FROM S_TORIMAST"
            SQL &= " WHERE TORIS_CODE_T = '" & TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_T = '" & TORIF_CODE & "'"

            If GCom.SetDynaset(SQL, REC) AndAlso REC.Read Then

                FMT_KBN = GCom.NzStr(REC.Item("FMT_KBN_T"))

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

        Dim REC As OracleDataReader = Nothing

        Try

            Call GET_SET_KAMOKU_CODE()

            Dim SQL As String = "SELECT * FROM S_MEIMAST"
            SQL &= " WHERE TORIS_CODE_K = '" & TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_K = '" & TORIF_CODE & "'"
            SQL &= " AND DATA_KBN_K = '2'"
            SQL &= " AND FURI_DATE_K = '" & FURI_DATE & "'"
            SQL &= " AND KEIYAKU_KIN_K = '" & KEIYAKU_KIN & "'"
            SQL &= " AND KEIYAKU_SIT_K = '" & KEIYAKU_SIT & "'"

            SQL &= " AND KEIYAKU_KAMOKU_K IN ('" & KEIYAKU_KAMOKU & "',"
            SQL &= " '" & GCom.NzInt(KEIYAKU_KAMOKU).ToString & "')"

            SQL &= " AND KEIYAKU_KOUZA_K = '" & KEIYAKU_KOUZA & "'"
            SQL &= " AND FURIKIN_K = '" & FURIKIN & "'"

            Dim RecordCounter As Integer = 0

            If GCom.SetDynaset(SQL, REC) Then
                Do While REC.Read

                    RecordCounter += 1

                    If RecordCounter > 1 Then

                        Dim DRet As DialogResult = MessageBox.Show(String.Format(MSG0026I, KEIYAKU_KIN, KEIYAKU_SIT, KEIYAKU_KAMOKU_K.Text, KEIYAKU_KOUZA, FURIKIN, KEIYAKU_KNAME, RECORD_NO, MOTIKOMI_SEQ).Replace("企業シーケンス", "持込回数"), msgTitle, _
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)

                        If DRet = DialogResult.Yes Then
                            Exit Do
                        End If
                    End If

                    KEIYAKU_KNAME = GCom.NzStr(REC.Item("KEIYAKU_KNAME_K")).Trim
                    RECORD_NO = GCom.NzDec(REC.Item("RECORD_NO_K"), "")
                    FURIKETU_CODE = GCom.NzDec(REC.Item("FURIKETU_CODE_K"), "")
                    JYUYOUKA_NO = GCom.NzDec(REC.Item("JYUYOUKA_NO_K"), "")
                    MOTIKOMI_SEQ = GCom.NzDec(REC.Item("MOTIKOMI_SEQ_K"), "")
                Loop

                If RecordCounter > 0 Then

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
    'NAME           :UPDATE_MEIMAST_FURIKETU_CODE
    'Description    :明細マスタ更新
    'Return         :Nothing
    'Create         :2004/08/30
    'Update         :2008.02.14 Uodate By Astar 20130322 maeda
    '=====================================================================================
    Private Sub UPDATE_MEIMAST_FURIKETU_CODE()

        Dim MainDB As MyOracle = Nothing

        Try
            MainDB = New MyOracle

            MainDB.BeginTrans()

            FURIKETU_CODE = ""
            Call GET_SET_FURIKETU_CODE()

            Dim SQL As String = "UPDATE S_MEIMAST"
            SQL &= " SET FURIKETU_CODE_K = '" & FURIKETU_CODE & "'"
            SQL &= ",KOUSIN_DATE_K = '" & DateTime.Today.ToString("yyyyMMdd") & "'"
            SQL &= " WHERE TORIS_CODE_K = '" & TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_K = '" & TORIF_CODE & "'"
            SQL &= " AND FURI_DATE_K = '" & FURI_DATE & "'"
            SQL &= " AND MOTIKOMI_SEQ_K = " & MOTIKOMI_SEQ & ""
            SQL &= " AND KEIYAKU_KIN_K = '" & KEIYAKU_KIN & "'"
            SQL &= " AND KEIYAKU_SIT_K = '" & KEIYAKU_SIT & "'"
            SQL &= " AND KEIYAKU_KAMOKU_K = '" & KEIYAKU_KAMOKU & "'"
            SQL &= " AND KEIYAKU_KOUZA_K = '" & KEIYAKU_KOUZA & "'"
            SQL &= " AND FURIKIN_K = '" & FURIKIN & "'"
            SQL &= " AND RECORD_NO_K = '" & RECORD_NO & "'"

            Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)

            If Ret = 1 Then

                '変更をスケジュールに反映する
                'S_SCHMASTを更新する
                If Not ReCalcS_SchmastTotal(TORIS_CODE, TORIF_CODE, FURI_DATE, CInt(MOTIKOMI_SEQ), MainDB) Then
                    MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                    MainDB.Rollback()
                    MainDB.Close()
                    MainDB = Nothing
                    Exit Try
                Else
                    MainDB.Commit()
                    MainDB.Close()
                    MainDB = Nothing

                    MessageBox.Show(MSG0501I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                Me.btnAction.Enabled = False

                UpdateFlg = True

                '2011/06/23 標準版修正 更新処理後の画面制御変更(総振) ------------------START
                '取引先主コードにフォーカスをあてる
                TORIS_CODE_K.Focus()
                'Me.btnEraser.PerformClick()
                '2011/06/23 標準版修正 更新処理後の画面制御変更(総振) ------------------END

                Return
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(更新)", "失敗", ex.Message)
            Me.Close()
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try
    End Sub

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, TORIS_CODE_K, TORIF_CODE_K, "3") = -1 Then
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

    '20130322 maeda
    Private Function ReCalcS_SchmastTotal(ByVal TORIS_CODE As String, ByVal TORIF_CODE As String, ByVal FURI_DATE As String, ByVal MOTIKOMI_SEQ As Integer, ByVal MainDB As MyOracle) As Boolean

        Dim ret As Boolean = False

        Dim SQL As New System.Text.StringBuilder(128)
        Dim oraToriReader As MyOracleReader = Nothing
        Dim oraMeiReader As MyOracleReader = Nothing

        Try
            Dim TesuuKin1 As Decimal = 0                    '手数料金額１
            Dim TesuuKin2 As Decimal = 0                    '手数料金額２
            Dim TesuuKin3 As Decimal = 0                    '手数料金額３

            SQL.Length = 0
            With SQL
                .Append(" SELECT ")
                .Append("  TESUUTYO_KBN_T")
                .Append(" ,SOURYO_T")
                .Append(" ,KOTEI_TESUU1_T")
                .Append(" ,KOTEI_TESUU2_T ")
                .Append(" FROM S_TORIMAST")
                .Append(" WHERE TORIS_CODE_T = " & SQ(TORIS_CODE))
                .Append(" AND   TORIF_CODE_T = " & SQ(TORIF_CODE))
            End With

            oraToriReader = New MyOracleReader(MainDB)

            If oraToriReader.DataReader(SQL) Then
                '＊＊＊＊＊手数料情報取得＊＊＊＊＊
                If oraToriReader.GetString("TESUUTYO_KBN_T").Equals("2") = False Then
                    '手数料徴求区分が特別免除以外の場合
                    TesuuKin1 = 0                                        '引落手数料
                    'TesuuKin1 = oraToriReader.GetInt64("TESUU_KIN1")     '引落手数料
                    '送料＋固定手数料１＋固定手数料２
                    TesuuKin2 = oraToriReader.GetInt64("SOURYO_T") _
                                + oraToriReader.GetInt64("KOTEI_TESUU1_T") _
                                + oraToriReader.GetInt64("KOTEI_TESUU2_T")
                    TesuuKin3 = 0

                    '明細マスタより振込手数料を取得する
                    SQL.Length = 0
                    With SQL
                        .Append(" select sum(TESUU_KIN_K) as TOTAL_TESUU")
                        .Append(" from S_MEIMAST")
                        .Append(" where TORIS_CODE_K = " & SQ(TORIS_CODE))
                        .Append(" and TORIF_CODE_K = " & SQ(TORIF_CODE))
                        .Append(" and FURI_DATE_K = " & SQ(FURI_DATE))
                        .Append(" and MOTIKOMI_SEQ_K = " & MOTIKOMI_SEQ.ToString)
                        .Append(" and DATA_KBN_K = '2'")
                        .Append(" and FURIKETU_CODE_K = 0")
                    End With

                    oraMeiReader = New MyOracleReader(MainDB)

                    If oraMeiReader.DataReader(SQL) = True Then
                        TesuuKin3 = oraMeiReader.GetInt64("TOTAL_TESUU")
                    Else
                        TesuuKin3 = 0
                    End If

                    oraMeiReader.Close()
                    oraMeiReader = Nothing

                Else
                    '手数料徴求区分が特別免除の場合
                    TesuuKin1 = 0
                    TesuuKin2 = 0
                    TesuuKin3 = 0
                End If
                '＊＊＊＊＊手数料情報取得＊＊＊＊＊

                '振込件数、振込金額、エラー件数、エラー金額の算出
                Dim FuriKen As Long = 0
                Dim FuriKin As Long = 0
                Dim ErrKen As Long = 0
                Dim ErrKin As Long = 0

                '----------------------
                '振込件数、振込金額
                '----------------------
                SQL.Length = 0
                SQL.Append("SELECT ")
                SQL.Append(" COUNT(FURIKIN_K) KEN")
                SQL.Append(",SUM(FURIKIN_K)   KIN")
                SQL.Append(" from S_MEIMAST")
                SQL.Append(" where TORIS_CODE_K = " & SQ(TORIS_CODE))
                SQL.Append(" and TORIF_CODE_K = " & SQ(TORIF_CODE))
                SQL.Append(" and FURI_DATE_K = " & SQ(FURI_DATE))
                SQL.Append(" and MOTIKOMI_SEQ_K = " & MOTIKOMI_SEQ.ToString)
                SQL.Append(" and DATA_KBN_K = '2'")
                SQL.Append(" and FURIKETU_CODE_K = 0 ")

                oraMeiReader = New MyOracleReader(MainDB)

                If oraMeiReader.DataReader(SQL) = True Then
                    FuriKen = oraMeiReader.GetInt64("KEN")
                    FuriKin = oraMeiReader.GetInt64("KIN")
                Else
                    'エラー
                    Exit Try
                End If
                oraMeiReader.Close()
                oraMeiReader = Nothing

                '----------------------
                'エラー件数、エラー金額
                '----------------------
                SQL.Length = 0
                SQL.Append("SELECT ")
                SQL.Append(" COUNT(FURIKIN_K) KEN")
                SQL.Append(",SUM(FURIKIN_K)   KIN")
                SQL.Append(" from S_MEIMAST")
                SQL.Append(" where TORIS_CODE_K = " & SQ(TORIS_CODE))
                SQL.Append(" and TORIF_CODE_K = " & SQ(TORIF_CODE))
                SQL.Append(" and FURI_DATE_K = " & SQ(FURI_DATE))
                SQL.Append(" and MOTIKOMI_SEQ_K = " & MOTIKOMI_SEQ.ToString)
                SQL.Append(" and DATA_KBN_K = '2'")
                SQL.Append(" and FURIKETU_CODE_K <> 0 ")

                oraMeiReader = New MyOracleReader(MainDB)

                If oraMeiReader.DataReader(SQL) = True Then
                    ErrKen = oraMeiReader.GetInt64("KEN")
                    ErrKin = oraMeiReader.GetInt64("KIN")
                Else
                    'エラー
                    Exit Try
                End If
                oraMeiReader.Close()
                oraMeiReader = Nothing

                'スケジュールマスタ更新
                SQL.Length = 0
                With SQL
                    .Append("update S_SCHMAST set")
                    .Append(" FURI_KEN_S = " & FuriKen.ToString)
                    .Append(",FURI_KIN_S = " & FuriKin.ToString)
                    .Append(",ERR_KEN_S = " & ErrKen.ToString)
                    .Append(",ERR_KIN_S = " & ErrKin.ToString)
                    .Append(",TESUU_KIN1_S = " & TesuuKin1.ToString)
                    .Append(",TESUU_KIN2_S = " & TesuuKin2.ToString)
                    .Append(",TESUU_KIN3_S = " & TesuuKin3.ToString)
                    .Append(",TESUU_KIN_S = " & (TesuuKin1 + TesuuKin2 + TesuuKin3).ToString)
                    .Append(",TESUUKEI_FLG_S = '1'")
                    .Append(" where TORIS_CODE_S = " & SQ(TORIS_CODE))
                    .Append(" and TORIF_CODE_S = " & SQ(TORIF_CODE))
                    .Append(" and FURI_DATE_S = " & SQ(FURI_DATE))
                    .Append(" and MOTIKOMI_SEQ_S = " & MOTIKOMI_SEQ.ToString)
                End With

                If MainDB.ExecuteNonQuery(SQL) <> 1 Then
                    BatchLog.Write("スケジュール更新処理", "失敗", "")
                    Exit Try
                Else
                    BatchLog.Write(TORIS_CODE & TORIF_CODE, FURI_DATE, "スケジュール更新処理", "成功", "")
                End If

                ret = True
            Else
                BatchLog.Write("スケジュール更新処理", "失敗", "対象無し")
            End If

        Catch ex As Exception
            BatchLog.Write("スケジュール更新処理", "失敗", ex.Message)
        Finally
            If Not oraToriReader Is Nothing Then oraToriReader.Close()
            If Not oraMeiReader Is Nothing Then oraMeiReader.Close()
        End Try

        Return ret

    End Function
    '20130322 maeda

End Class