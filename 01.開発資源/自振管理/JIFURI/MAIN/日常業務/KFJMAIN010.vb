'2013/12/24 saitou 標準版 性能向上 DEL -------------------------------------------------->>>>
'Imports CASTCommon.BatchLOG
'Imports clsFUSION.clsMain
'Imports System.Windows.Forms.Button
'Imports System.Data.OracleClient
'Imports System.Data.SqlTypes.SqlMoney
'2013/12/24 saitou 標準版 性能向上 DEL --------------------------------------------------<<<<
Imports CASTCommon
Imports System.Text

Public Class KFJMAIN010

#Region "宣言"

    '2013/12/24 saitou 標準版 性能向上 DEL -------------------------------------------------->>>>
    'Public clsFUSION As New clsFUSION.clsMain
    'Public GCom As New MenteCommon.clsCommon
    '2013/12/24 saitou 標準版 性能向上 DEL --------------------------------------------------<<<<

    Private BatchLog As New CASTCommon.BatchLOG("KFJMAIN010", "口振依頼データ落込画面")

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private Const msgTitle As String = "口振依頼データ落込画面(KFJMAIN010)"

    Private MainDB As CASTCommon.MyOracle

    '2013/12/24 saitou 標準修正 ADD -------------------------------------------------->>>>
    Private TORI_CODE As String = ""
    Private FURI_DATE As String = ""
    Private CODE_KBN As String = ""
    Private FMT_KBN As String = ""
    Private BAITAI_CODE As String = ""
    Private LABEL_KBN As String = ""
    '2013/12/24 saitou 標準修正 ADD --------------------------------------------------<<<<

#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFJMAIN010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)開始", "成功", "")

            '------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------
            GCom.GetUserID = gstrUSER_ID
            GCom.GetSysDate = String.Format("{0:yyyy年MM月dd日}", Date.Now)
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------
            '委託者名リストボックスの設定
            '------------------------------------------
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            '------------------------------------------
            '媒体区分リストボックスの設定
            '------------------------------------------
            ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            'Call sub_SetBaitaiKBN()
            'cmbBaitai.SelectedIndex = 0
            Select Case GCom.SetComboBox(Me.cmbBaitai, "KFJMAIN010_媒体区分.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "媒体区分", "KFJMAIN010_媒体区分.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(コンボボックス設定)", "失敗", "媒体区分設定ファイルなし。ファイル:KFJMAIN010_媒体区分.TXT")
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "媒体区分"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(コンボボックス設定)", "失敗", "媒体区分設定異常。ファイル:KFJMAIN010_媒体区分.TXT")
            End Select
            ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

            ' 2017/05/09 タスク）綾部 CHG 【OM】(RSV2対応 機能追加) -------------------- START
            ''2010/12/24 信組対応 
            'If GetFSKJIni("COMMON", "CENTER") = "0" Then
            '    '信組の場合は持込区分選択非表示
            '    Label19.Visible = False
            '    rbKinko.Visible = False
            '    rbCenter.Visible = False
            'End If
            '------------------------------------------
            ' CENTER=4(東海)以外の場合は持込区分非表示
            '------------------------------------------
            If GetFSKJIni("COMMON", "CENTER") <> "4" Then
                Label19.Visible = False
                rbKinko.Visible = False
                rbCenter.Visible = False
            End If
            ' 2017/05/09 タスク）綾部 CHG 【OM】(RSV2対応 機能追加) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)", "失敗", ex.Message)

            Me.Close()

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)終了", "成功", "")

        End Try
    End Sub

    ''画面クローズ
    'Private Sub Form1_FormClosing(ByVal sender As Object, _
    '    ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing

    '    Try

    '        If e.CloseReason = CloseReason.UserClosing Then
    '            Me.Owner.Show()
    '        End If
    '    Catch ex As Exception
    '        BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(クローズ)", "失敗", ex.Message)
    '    Finally

    '    End Try
    'End Sub

#End Region

#Region "ボタン"

    '登録ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Dim strMessage As String = ""
        Dim KEY1 As String = ""                 '取引先コード
        Dim KEY2 As String = ""                 '振替日
        Dim KEY3 As String = ""                 '媒体コード
        Dim KEY4 As String = ""                 '登録フラグ、中断フラグ
        Dim KEY5 As String = ""                 '受付フラグ
        Dim LogTori As String = "0000000000-00" 'ログ書込用取引先
        Dim LogFuri As String = "00000000"      'ログ書込用振替日

        Try
            BatchLog.Write(gstrUSER_ID, LogTori, LogFuri, "(登録)開始", "成功", "")
            Cursor.Current = Cursors.WaitCursor()

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '必須項目の入力値チェック
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_check_text() = False Then
                Exit Sub
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'キー項目取得
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Select Case rbKinko.Checked
                Case True
                    '----------------------------------------
                    '金庫持込
                    '----------------------------------------
                    KEY1 = txtTorisCode.Text & txtTorifCode.Text
                    KEY2 = txtKinkoFuriDateY.Text & txtKinkoFuriDateM.Text & txtKinkoFuriDateD.Text
                    KEY3 = 0
                    KEY4 = 0
                    KEY5 = 0
                    TORI_CODE = KEY1
                    FURI_DATE = KEY2
                    LogTori = txtTorisCode.Text & "-" & txtTorifCode.Text
                    LogFuri = txtKinkoFuriDateY.Text & txtKinkoFuriDateM.Text & txtKinkoFuriDateD.Text

                Case Else
                    '----------------------------------------
                    'センター直接持込
                    '----------------------------------------
                    KEY1 = ""
                    KEY2 = txtCenterFuriDateY.Text & txtCenterFuriDateM.Text & txtCenterFuriDateD.Text
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'Select Case cmbBaitai.SelectedItem.ToString
                    '    Case "伝送"
                    '        KEY3 = "00"
                    '    Case "ＣＭＴ"
                    '        KEY3 = "06"
                    'End Select
                    KEY3 = GCom.GetComboBox(Me.cmbBaitai).ToString.PadLeft(2, "0"c)
                    ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                    KEY4 = 0
                    KEY5 = 0
                    LogFuri = txtCenterFuriDateY.Text & txtCenterFuriDateM.Text & txtCenterFuriDateD.Text

            End Select

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'スケジュール検索
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Select Case True
                Case rbCenter.Checked = True
                    '----------------------------------------
                    'センター直接持込
                    '----------------------------------------
                    strMessage = "登録処理を続行しますか？(ID:MSG0050I)" & vbCrLf & vbCrLf & _
                                    "登録日　：　" & KEY2.Substring(0, 4) & "年 " & KEY2.Substring(4, 2) & "月 " & KEY2.Substring(6, 2) & "日" & vbCrLf & _
                                    "媒　体　：　" & cmbBaitai.SelectedItem.ToString

                    FURI_DATE = KEY2
                    BAITAI_CODE = KEY3

                Case rbKinko.Checked = True And KEY1 = "111111111111"
                    '----------------------------------------
                    '金庫持込 － 依頼書ベース全件
                    '----------------------------------------
                    KEY3 = "04"                      '媒体コード
                    KEY4 = "0"                      '登録フラグ、中断フラグ
                    KEY5 = "1"                      '受付フラグ
                    If fn_getEntriAllMain(KEY2, KEY3, KEY4, KEY5, strMessage) = False Then
                        Exit Sub
                    End If

                Case rbKinko.Checked = True And KEY1 = "222222222222"
                    '----------------------------------------
                    '金庫持込 － 伝票ベース全件
                    '----------------------------------------
                    KEY3 = "09"                      '媒体コード
                    KEY4 = "0"                      '登録フラグ、中断フラグ
                    KEY5 = "1"                      '受付フラグ
                    If fn_getEntriAllMain(KEY2, KEY3, KEY4, KEY5, strMessage) = False Then
                        Exit Sub
                    End If

                Case Else
                    '----------------------------------------
                    '金庫持込 － 取引先指定
                    '----------------------------------------
                    If fn_getKobetuMain(KEY1, KEY2, KEY4, strMessage) = False Then
                        Exit Sub
                    End If

            End Select

            '----------------------------------------
            '確認メッセージを表示
            '----------------------------------------
            If MessageBox.Show(strMessage, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                'キャンセル
                Exit Sub
            Else
                Select Case KEY1
                    Case "111111111111", "222222222222"
                        If fn_getEntriAllSetJob(KEY2, KEY3, KEY4, KEY5) = False Then
                            Exit Sub
                        End If
                    Case Else
                        'バッチ実行
                        If fn_setJob() = False Then
                            Exit Sub
                        End If
                End Select

            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, LogTori, LogFuri, "(登録)", "失敗", ex.Message)

        Finally
            BatchLog.Write(gstrUSER_ID, LogTori, LogFuri, "(登録)終了", "成功", "")
        End Try
    End Sub

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("(クローズ)", "失敗", ex.ToString)
        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(クローズ)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "テキスト"

    'テキストボックスゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtTorisCode.Validating, txtTorifCode.Validating, _
            txtKinkoFuriDateY.Validating, txtKinkoFuriDateM.Validating, txtKinkoFuriDateD.Validating, _
            txtCenterFuriDateY.Validating, txtCenterFuriDateM.Validating, txtCenterFuriDateD.Validating

        Try

            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(テキスト制御)", "失敗", ex.Message)

        Finally

        End Try
    End Sub

#End Region

#Region "ラジオボタン"

    'モード キープレス
    Private Sub RadMode_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles rbKinko.KeyPress, rbCenter.KeyPress
        Try

            Select Case e.KeyChar
                Case Microsoft.VisualBasic.ChrW(Keys.Enter)
                    ' Enterキーで，実行ボタンにフォーカスを設定する
                    e.Handled = False
                    If rbCenter.Checked = True Then
                        'センター直接持込
                        txtCenterFuriDateY.Select()
                    Else
                        '金庫持込
                        cmbKana.Select()
                    End If
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ラジオボタン制御)", "失敗", ex.Message)
        Finally

        End Try
    End Sub

    'センター直接持込時
    Private Sub rbCenter_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbCenter.CheckedChanged
        Try
            pKinko.Visible = False
            pCenter.Visible = True

            '20121115 maeda 初期値設定
            With Me
                .txtCenterFuriDateY.Text = Now.ToString("yyyy")
                .txtCenterFuriDateM.Text = Now.ToString("MM")
                .txtCenterFuriDateD.Text = Now.ToString("dd")
                ' 2016/04/22 タスク）綾部 DEL 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                '.cmbBaitai.SelectedItem = "ＣＭＴ"
                ' 2016/04/22 タスク）綾部 DEL 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
            End With
            '20121115 maeda 初期値設定

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(センター直接持込表示)", "失敗", ex.Message)
        Finally

        End Try
    End Sub

    '金庫持込選択時
    Private Sub rbKinko_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbKinko.CheckedChanged
        Try

            pKinko.Visible = True
            pCenter.Visible = False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(金庫持込表示)", "失敗", ex.Message)
        Finally

        End Try
    End Sub

#End Region

#Region "コンボボックス"

    '媒体コンボＥｎｔｅｒ押下
    Private Sub cmbBaitai_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cmbBaitai.KeyPress
        Try

            Select Case e.KeyChar
                Case Microsoft.VisualBasic.ChrW(Keys.Enter)
                    ' Enterキーで，実行ボタンにフォーカスを設定する
                    e.Handled = False
                    btnAction.Select()
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(媒体コンボ制御)", "失敗", ex.Message)
        Finally


        End Try

    End Sub

    '委託者名リストボックスの設定
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '選択カナで始まる委託者名を取得
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
            cmbToriName.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(委託者名リストボックス設定)", "失敗", ex.Message)

        Finally

        End Try
    End Sub

    '取引先コードテキストボックスに取引先コード設定
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        Try


            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '取引先コードを取得
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If Not cmbToriName.SelectedItem.ToString.Trim = Nothing Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtKinkoFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取引先コード設定)", "失敗", ex.Message)

        Finally

        End Try
    End Sub

#End Region

#Region "関数"

    Private Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :実行ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/08/25
        'Update         :
        '============================================================================
        '2013/12/24 saitou 標準版 性能向上 DEL -------------------------------------------------->>>>
        'Dim CheckFuriDateY As TextBox
        'Dim CheckFuriDateM As TextBox
        'Dim CheckFuriDateD As TextBox
        'Dim CheckToriCode As String
        '2013/12/24 saitou 標準版 性能向上 DEL --------------------------------------------------<<<<

        Try

            Select Case rbKinko.Checked
                Case True
                    '--------------------------------------
                    '金庫持込チェック
                    '--------------------------------------
                    '2013/12/24 saitou 標準版 性能向上 UPD -------------------------------------------------->>>>
                    '取引先主コード必須チェック
                    If Me.txtTorisCode.Text.Trim = String.Empty Then
                        MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtTorisCode.Focus()
                        Return False
                    End If

                    '取引先副コード必須チェック
                    If Me.txtTorifCode.Text.Trim = String.Empty Then
                        MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtTorifCode.Focus()
                        Return False
                    End If

                    '振替日(年)必須チェック
                    If Me.txtKinkoFuriDateY.Text.Trim = String.Empty Then
                        MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtKinkoFuriDateY.Focus()
                        Return False
                    End If

                    '振替日(月)必須チェック
                    If Me.txtKinkoFuriDateM.Text.Trim = String.Empty Then
                        MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtKinkoFuriDateM.Focus()
                        Return False
                    End If

                    '振替日(月)範囲チェック
                    If GCom.NzInt(Me.txtKinkoFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtKinkoFuriDateM.Text.Trim) > 12 Then
                        MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtKinkoFuriDateM.Focus()
                        Return False
                    End If

                    '振替日(日)必須チェック
                    If Me.txtKinkoFuriDateD.Text.Trim = String.Empty Then
                        MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtKinkoFuriDateD.Focus()
                        Return False
                    End If

                    '振替日(日)範囲チェック
                    If GCom.NzInt(Me.txtKinkoFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtKinkoFuriDateD.Text.Trim) > 31 Then
                        MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtKinkoFuriDateD.Focus()
                        Return False
                    End If

                    '日付整合性チェック
                    Dim WORK_DATE As String = Me.txtKinkoFuriDateY.Text & "/" & Me.txtKinkoFuriDateM.Text & "/" & Me.txtKinkoFuriDateD.Text
                    If Information.IsDate(WORK_DATE) = False Then
                        MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtKinkoFuriDateY.Focus()
                        Return False
                    End If

                    'If clsFUSION.fn_CHECK_NUM_MSG(txtTorisCode.Text, "取引先主コード", msgTitle) = False Then
                    '    txtTorisCode.Focus()
                    '    Return False
                    'End If

                    'If clsFUSION.fn_CHECK_NUM_MSG(txtTorifCode.Text, "取引先副コード", msgTitle) = False Then
                    '    txtTorifCode.Focus()
                    '    Return False
                    'End If

                    'CheckFuriDateY = txtKinkoFuriDateY
                    'CheckFuriDateM = txtKinkoFuriDateM
                    'CheckFuriDateD = txtKinkoFuriDateD
                    'CheckToriCode = txtTorisCode.Text & txtTorisCode.Text
                    '2013/12/24 saitou 標準版 性能向上 UPD --------------------------------------------------<<<<

                Case Else
                    '--------------------------------------
                    'センター直接持込チェック
                    '--------------------------------------
                    '2013/12/24 saitou 標準版 性能向上 UPD -------------------------------------------------->>>>
                    '振替日(年)必須チェック
                    If Me.txtCenterFuriDateY.Text.Trim = String.Empty Then
                        MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtCenterFuriDateY.Focus()
                        Return False
                    End If

                    '振替日(月)必須チェック
                    If Me.txtCenterFuriDateM.Text.Trim = String.Empty Then
                        MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtCenterFuriDateM.Focus()
                        Return False
                    End If

                    '振替日(月)範囲チェック
                    If GCom.NzInt(Me.txtCenterFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtCenterFuriDateM.Text.Trim) > 12 Then
                        MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtCenterFuriDateM.Focus()
                        Return False
                    End If

                    '振替日(日)必須チェック
                    If Me.txtCenterFuriDateD.Text.Trim = String.Empty Then
                        MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtCenterFuriDateD.Focus()
                        Return False
                    End If

                    '振替日(日)範囲チェック
                    If GCom.NzInt(Me.txtCenterFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtCenterFuriDateD.Text.Trim) > 31 Then
                        MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtCenterFuriDateD.Focus()
                        Return False
                    End If

                    '日付整合性チェック
                    Dim WORK_DATE As String = Me.txtCenterFuriDateY.Text & "/" & Me.txtCenterFuriDateM.Text & "/" & Me.txtCenterFuriDateD.Text
                    If Information.IsDate(WORK_DATE) = False Then
                        MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtCenterFuriDateY.Focus()
                        Return False
                    End If

                    'CheckFuriDateY = txtCenterFuriDateY
                    'CheckFuriDateM = txtCenterFuriDateM
                    'CheckFuriDateD = txtCenterFuriDateD
                    'CheckToriCode = ""
                    '2013/12/24 saitou 標準版 性能向上 UPD --------------------------------------------------<<<<

            End Select

            '2013/12/24 saitou 標準版 性能向上 DEL -------------------------------------------------->>>>
            ''--------------------------------------
            ''共通日付入力チェック
            ''--------------------------------------
            'If clsFUSION.fn_CHECK_NUM_MSG(CheckFuriDateY.Text, "年", msgTitle) = False Then
            '    CheckFuriDateY.Focus()
            '    Return False
            'End If

            'If clsFUSION.fn_CHECK_NUM_MSG(CheckFuriDateM.Text, "月", msgTitle) = False Then
            '    CheckFuriDateM.Focus()
            '    Return False
            'Else
            '    If CheckFuriDateM.Text < 1 Or CheckFuriDateM.Text > 12 Then
            '        MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        CheckFuriDateM.Focus()
            '        Return False
            '    End If
            'End If

            'If clsFUSION.fn_CHECK_NUM_MSG(CheckFuriDateD.Text, "日", msgTitle) = False Then
            '    CheckFuriDateD.Focus()
            '    Return False
            'Else
            '    If CheckFuriDateD.Text < 1 Or CheckFuriDateD.Text > 31 Then
            '        MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        CheckFuriDateD.Focus()
            '        Return False
            '    End If
            'End If
            '2013/12/24 saitou 標準版 性能向上 DEL --------------------------------------------------<<<<

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(入力チェック処理)", "失敗", ex.Message)
            Return False

        Finally

        End Try
    End Function

    ' 2016/04/22 タスク）綾部 DEL 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
    'Sub sub_SetBaitaiKBN()
    '    '=====================================================================================
    '    'NAME           :sub_SetBaitaiKBN
    '    'Parameter      :なし
    '    'Description    :選択されたカナ文字で始まる委託者カナ氏名を抽出し、コンボボックスに表示する
    '    'Create         :2009/10/10
    '    'Update         :
    '    '=====================================================================================
    '    Try
    '        cmbBaitai.Items.Add("伝送")
    '        cmbBaitai.Items.Add("ＣＭＴ")
    '    Catch ex As Exception
    '        MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "媒体コードリスト設定", "失敗", ex.Message)
    '    End Try

    'End Sub
    ' 2016/04/22 タスク）綾部 DEL 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

    Function fn_getEntriAllMain(ByVal Key2 As String, ByVal Key3 As String, ByVal Key4 As String, ByVal Key5 As String, ByRef strMessage As String) As Boolean
        '============================================================================
        'NAME           :fn_getKensuu
        'Parameter      :Key2 - 振替日 / Key3 - 媒体コード / Key4 - 登録フラグ / Key5 - 受付フラグ　
        '               :RecordCount - 検出件数
        'Description    :対象スケジュールの件数を取得
        'Return         :True=OK,False=NG
        'Create         :2009/08/25
        'Update         :
        '============================================================================
        '2013/12/24 saitou 標準版 性能向上 UPD -------------------------------------------------->>>>
        '作り直し
        Dim intRecordCount As Integer
        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try
            '-----------------------------------------
            'パラメータ値設定
            '-----------------------------------------
            If Key3 = "04" Then
                '依頼書
                BAITAI_CODE = "04" '媒体コード
                FMT_KBN = "04"     'フォーマット区分
            Else
                '伝票
                BAITAI_CODE = "09" '媒体コード
                FMT_KBN = "05"     'フォーマット区分
            End If
            CODE_KBN = "0"      'コード区分
            LABEL_KBN = "0"     'ラベル区分

            '-----------------------------------------
            'レコードの件数取得
            '-----------------------------------------
            Dim SQL As New StringBuilder
            With SQL
                .Append("SELECT COUNT(*) AS RECORDCOUNT")
                .Append(" FROM SCHMAST,TORIMAST")
                .Append(" WHERE FURI_DATE_S = " & SQ(Key2))
                .Append(" AND BAITAI_CODE_S = " & SQ(Key3))
                .Append(" AND TORIS_CODE_S = TORIS_CODE_T")
                .Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                .Append(" AND TOUROKU_FLG_S = " & SQ(Key4))
                .Append(" AND UKETUKE_FLG_S = " & SQ(Key5))
                .Append(" AND TYUUDAN_FLG_S = '0'")
            End With

            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            '-----------------------------------------
            'レコードの件数取得
            '-----------------------------------------
            If oraReader.DataReader(SQL) = True Then
                intRecordCount = oraReader.GetInt("RECORDCOUNT")
                If intRecordCount = 0 Then
                    MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    If rbKinko.Checked Then
                        txtKinkoFuriDateY.Focus()
                    Else
                        txtCenterFuriDateY.Focus()
                    End If
                    Return False

                Else
                    strMessage = "登録処理を実行しますか？(ID:MSG0050I)" & vbCrLf & _
                                    "振替日：" & Key2.Substring(0, 4) & "年 " & Key2.Substring(4, 2) & "月 " & Key2.Substring(6, 2) & "日" & vbCrLf & _
                                    "件数　：" & intRecordCount
                End If
            Else
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(対象スケジュール件数取得)", "終了", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try

        'Dim intRecordCount As Integer

        'Try

        '    '-----------------------------------------
        '    'パラメータ値設定
        '    '-----------------------------------------
        '    If Key3 = "04" Then
        '        '依頼書
        '        BAITAI_CODE = "04" '媒体コード
        '        FMT_KBN = "04"     'フォーマット区分
        '    Else
        '        '伝票
        '        BAITAI_CODE = "09" '媒体コード
        '        FMT_KBN = "05"     'フォーマット区分
        '    End If
        '    CODE_KBN = "0"      'コード区分
        '    LABEL_KBN = "0"     'ラベル区分

        '    '-----------------------------------------
        '    'レコードの件数取得
        '    '-----------------------------------------
        '    gstrSSQL = ""
        '    gstrSSQL &= "SELECT COUNT(*) AS RECORDCOUNT"
        '    gstrSSQL &= " FROM SCHMAST,TORIMAST"
        '    gstrSSQL &= " WHERE FURI_DATE_S = '" & Key2 & "'"
        '    gstrSSQL &= " AND BAITAI_CODE_S = '" & Key3 & "'"
        '    gstrSSQL &= " AND TORIS_CODE_S = TORIS_CODE_T"
        '    gstrSSQL &= " AND TORIF_CODE_S = TORIF_CODE_T"
        '    gstrSSQL &= " AND TOUROKU_FLG_S = '" & Key4 & "'"
        '    gstrSSQL &= " AND TYUUDAN_FLG_S = '" & Key4 & "'"
        '    gstrSSQL &= " AND UKETUKE_FLG_S = '" & Key5 & "'"
        '    gstrSSQL &= " ORDER BY TORIS_CODE_S,TORIF_CODE_S"

        '    gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        '    gdbcCONNECT.Open()

        '    gdbCOMMAND = New OracleClient.OracleCommand
        '    gdbCOMMAND.CommandText = gstrSSQL
        '    gdbCOMMAND.Connection = gdbcCONNECT

        '    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

        '    '-----------------------------------------
        '    'レコードの件数取得
        '    '-----------------------------------------
        '    If gdbrREADER.Read() = True Then
        '        intRecordCount = gdbrREADER.Item("RECORDCOUNT")
        '        If intRecordCount = 0 Then
        '            If rbKinko.Checked Then
        '                txtCenterFuriDateY.Focus()
        '            Else
        '                txtCenterFuriDateY.Focus()
        '            End If
        '            MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '            If rbKinko.Checked Then
        '                txtKinkoFuriDateY.Focus()
        '            Else
        '                txtCenterFuriDateY.Focus()
        '            End If
        '            Return False
        '        Else
        '            strMessage = "登録処理を続行しますか？" & vbCrLf & vbCrLf & _
        '                            "振替日　：　" & Key2.Substring(0, 4) & "年 " & Key2.Substring(4, 2) & "月 " & Key2.Substring(6, 2) & "日" & vbCrLf & _
        '                            "件　数　：　" & intRecordCount
        '        End If
        '    Else
        '        MessageBox.Show(String.Format(MSG0027E, "スケジュールマスタ", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '        Return False
        '    End If

        '    Return True

        'Catch ex As Exception
        '    MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(対象スケジュール件数取得)", "終了", ex.Message)
        '    Return False
        'Finally
        '    gdbcCONNECT.Close()
        'End Try
        '2013/12/24 saitou 標準版 性能向上 UPD --------------------------------------------------<<<<

    End Function

    Function fn_getKobetuMain(ByVal Key1 As String, ByVal Key2 As String, ByVal Key4 As String, ByRef strMessage As String) As Boolean
        '============================================================================
        'NAME           :fn_getKobetuMain
        'Parameter      :Key1 - 取引先コード / Key2 - 振替日 / Key4 - 登録フラグ 
        '　　　　　　　 :strMessage - 出力メッセージ
        'Description    :対象スケジュールを取得
        'Return         :True=OK,False=NG
        'Create         :2009/08/25
        'Update         :
        '============================================================================
        Dim intRECORD_COUNT As Long
        Dim strITAKU_NNAME As String = ""
        Dim strKIGYO_CODE As String = ""
        Dim strFURI_CODE As String = ""
        Dim strITAKU_CODE As String = ""
        Dim strUKETUKE_DATE As String = ""
        Dim strTOUROKU_DATE As String = ""
        Dim dblSYORI_KEN As Double
        Dim dblSYORI_KIN As Double
        Dim strUKETUKE_FLG As String = ""

        '2013/12/24 saitou 標準版 性能向上 UPD -------------------------------------------------->>>>
        '作り直し
        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try
            Dim SQL As New StringBuilder
            With SQL
                .Append("SELECT * FROM SCHMAST, TORIMAST")
                .Append(" WHERE TORIS_CODE_S = " & SQ(Key1.Substring(0, 10)))
                .Append(" AND TORIF_CODE_S = " & SQ(Key1.Substring(10, 2)))
                .Append(" AND FURI_DATE_S = " & SQ(Key2))
                .Append(" AND TOUROKU_FLG_S = " & SQ(Key4))
                .Append(" AND TYUUDAN_FLG_S = '0'")
                .Append(" AND TORIS_CODE_S = TORIS_CODE_T")
                .Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            End With

            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            '-----------------------------------------
            'レコードの件数取得　&　マスタ設定値の取得
            '-----------------------------------------
            intRECORD_COUNT = 0
            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    intRECORD_COUNT = intRECORD_COUNT + 1
                    BAITAI_CODE = oraReader.GetString("BAITAI_CODE_T")
                    FMT_KBN = oraReader.GetString("FMT_KBN_T")
                    CODE_KBN = oraReader.GetString("CODE_KBN_T")
                    LABEL_KBN = oraReader.GetString("LABEL_KBN_T")
                    strITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_T")
                    strKIGYO_CODE = oraReader.GetString("KIGYO_CODE_T")
                    strFURI_CODE = oraReader.GetString("FURI_CODE_T")
                    strITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                    strUKETUKE_DATE = oraReader.GetString("UKETUKE_DATE_S")
                    strTOUROKU_DATE = oraReader.GetString("TOUROKU_DATE_S")
                    strUKETUKE_FLG = oraReader.GetString("UKETUKE_FLG_S")

                    oraReader.NextRead()
                End While
            End If

            oraReader.Close()

            '-------------------------------------------------
            'レコード件数が0件の時、登録済みであるか確認
            '-------------------------------------------------
            If intRECORD_COUNT = 0 Then
                Key1 = TORI_CODE      '取引先コード
                Key2 = FURI_DATE      '振替日
                Key4 = "1"                          '登録フラグ、中断フラグ

                With SQL
                    .Length = 0
                    .Append("SELECT * FROM SCHMAST, TORIMAST")
                    .Append(" WHERE TORIS_CODE_S = " & SQ(Key1.Substring(0, 10)))
                    .Append(" AND TORIF_CODE_S = " & SQ(Key1.Substring(10, 2)))
                    .Append(" AND FURI_DATE_S = " & SQ(Key2))
                    .Append(" AND TOUROKU_FLG_S = " & SQ(Key4))
                    .Append(" AND TYUUDAN_FLG_S = '0'")
                    .Append(" AND TORIS_CODE_S = TORIS_CODE_T")
                    .Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                End With

                intRECORD_COUNT = 0
                If oraReader.DataReader(SQL) = True Then
                    While oraReader.EOF = False
                        intRECORD_COUNT = intRECORD_COUNT + 1
                        BAITAI_CODE = oraReader.GetString("BAITAI_CODE_S")
                        FMT_KBN = oraReader.GetString("FMT_KBN_T")
                        CODE_KBN = oraReader.GetString("CODE_KBN_T")
                        LABEL_KBN = oraReader.GetString("LABEL_KBN_T")
                        strITAKU_NNAME = oraReader.GetString("ITAKU_NNAME_T")
                        strKIGYO_CODE = oraReader.GetString("KIGYO_CODE_T")
                        strFURI_CODE = oraReader.GetString("FURI_CODE_T")
                        strITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
                        strUKETUKE_DATE = oraReader.GetString("UKETUKE_DATE_S")
                        strTOUROKU_DATE = oraReader.GetString("TOUROKU_DATE_S")
                        dblSYORI_KEN = oraReader.GetInt64("SYORI_KEN_S")
                        dblSYORI_KIN = oraReader.GetInt64("SYORI_KIN_S")

                        oraReader.NextRead()
                    End While

                    If intRECORD_COUNT > 0 Then
                        '-------------------------------------------------
                        '登録済み確認画面の表示
                        '-------------------------------------------------
                        MessageBox.Show(String.Format(MSG0049I, strITAKU_NNAME, FURI_DATE.Substring(0, 4) & "年 " & FURI_DATE.Substring(4, 2) & "月 " & FURI_DATE.Substring(6, 2) & "日 ", _
                                      strKIGYO_CODE, strFURI_CODE, strITAKU_CODE, _
                                      strUKETUKE_DATE.Substring(0, 4) & "年 " & strUKETUKE_DATE.Substring(4, 2) & "月 " & strUKETUKE_DATE.Substring(6, 2) & "日 ", _
                                      strTOUROKU_DATE.Substring(0, 4) & "年 " & strTOUROKU_DATE.Substring(4, 2) & "月 " & strTOUROKU_DATE.Substring(6, 2) & "日 ", _
                                      Format(dblSYORI_KEN, "#,##0"), Format(dblSYORI_KIN, "#,##0")), _
                                      msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If

                Else
                    MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    If rbKinko.Checked Then
                        txtKinkoFuriDateY.Focus()
                    Else
                        txtCenterFuriDateY.Focus()
                    End If

                    Return False

                End If

            Else
                '-----------------------------------------------
                'レコードか0件でなかった時、確認画面を表示する
                '-----------------------------------------------
                If BAITAI_CODE = "04" Or BAITAI_CODE = "09" Then
                    If strUKETUKE_FLG = "0" Then
                        MessageBox.Show(MSG0328W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If
                End If

                strMessage = String.Format(MSG0050I, strITAKU_NNAME, _
                                           FURI_DATE.Substring(0, 4) & "年 " & FURI_DATE.Substring(4, 2) & "月 " & FURI_DATE.Substring(6, 2) & "日 ", _
                                           strKIGYO_CODE, strFURI_CODE, strITAKU_CODE, _
                                           strUKETUKE_DATE.Substring(0, 4) & "年 " & strUKETUKE_DATE.Substring(4, 2) & "月 " & strUKETUKE_DATE.Substring(6, 2) & "日 ", _
                                           strTOUROKU_DATE.Substring(0, 4) & "年 " & strTOUROKU_DATE.Substring(4, 2) & "月 " & strTOUROKU_DATE.Substring(6, 2) & "日 ")
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(対象スケジュール件数取得)終了", "成功", "")
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try

        'Try

        '    gstrSSQL = ""
        '    gstrSSQL = "SELECT * FROM SCHMAST,TORIMAST WHERE TORIS_CODE_S = '" & Key1.Substring(0, 10) & "' AND TORIF_CODE_S = '" & Key1.Substring(10, 2) & "'"
        '    gstrSSQL = gstrSSQL & " AND FURI_DATE_S = '" & Key2 & "' AND TOUROKU_FLG_S = '" & Key4 & "'"
        '    gstrSSQL = gstrSSQL & " AND TYUUDAN_FLG_S = '" & Key4 & "'"
        '    gstrSSQL = gstrSSQL & " AND TORIS_CODE_S = TORIS_CODE_T AND TORIF_CODE_S = TORIF_CODE_T"

        '    gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        '    gdbcCONNECT.Open()

        '    gdbCOMMAND = New OracleClient.OracleCommand
        '    gdbCOMMAND.CommandText = gstrSSQL
        '    gdbCOMMAND.Connection = gdbcCONNECT

        '    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

        '    '-----------------------------------------
        '    'レコードの件数取得　&　マスタ設定値の取得
        '    '-----------------------------------------
        '    intRECORD_COUNT = 0
        '    While (gdbrREADER.Read)
        '        intRECORD_COUNT = intRECORD_COUNT + 1
        '        BAITAI_CODE = clsFUSION.fn_chenge_null(gdbrREADER.Item("BAITAI_CODE_T"), "00")
        '        FMT_KBN = clsFUSION.fn_chenge_null(gdbrREADER.Item("FMT_KBN_T"), "00")
        '        CODE_KBN = clsFUSION.fn_chenge_null(gdbrREADER.Item("CODE_KBN_T"), "0")
        '        LABEL_KBN = clsFUSION.fn_chenge_null(gdbrREADER.Item("LABEL_KBN_T"), "0")
        '        strITAKU_NNAME = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("ITAKU_NNAME_T"))
        '        strKIGYO_CODE = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("KIGYO_CODE_T"))
        '        strFURI_CODE = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("FURI_CODE_T"))
        '        strITAKU_CODE = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("ITAKU_CODE_T"))
        '        strUKETUKE_DATE = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("UKETUKE_DATE_S"))
        '        strTOUROKU_DATE = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("TOUROKU_DATE_S"))
        '        strUKETUKE_FLG = clsFUSION.fn_chenge_null(gdbrREADER.Item("UKETUKE_FLG_S"), "0")
        '    End While

        '    '-------------------------------------------------
        '    'レコード件数が0件の時、登録済みであるか確認
        '    '-------------------------------------------------
        '    If intRECORD_COUNT = 0 Then
        '        Key1 = TORI_CODE      '取引先コード
        '        Key2 = FURI_DATE      '振替日
        '        Key4 = "1"                          '登録フラグ、中断フラグ

        '        gstrSSQL = "SELECT * FROM SCHMAST,TORIMAST WHERE TORIS_CODE_S = '" & Key1.Substring(0, 10) & "' AND TORIF_CODE_S = '" & Key1.Substring(10, 2) & "'"
        '        gstrSSQL = gstrSSQL & " AND FURI_DATE_S = '" & Key2 & "' AND TOUROKU_FLG_S = '" & Key4 & "'"
        '        gstrSSQL = gstrSSQL & " AND TORIS_CODE_S = TORIS_CODE_T AND TORIF_CODE_S = TORIF_CODE_T"

        '        intRECORD_COUNT = 0
        '        gdbCOMMAND = New OracleClient.OracleCommand
        '        gdbCOMMAND.CommandText = gstrSSQL
        '        gdbCOMMAND.Connection = gdbcCONNECT

        '        gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ
        '        While (gdbrREADER.Read)
        '            intRECORD_COUNT = intRECORD_COUNT + 1
        '            BAITAI_CODE = clsFUSION.fn_chenge_null(gdbrREADER.Item("BAITAI_CODE_S"), "00")
        '            FMT_KBN = clsFUSION.fn_chenge_null(gdbrREADER.Item("FMT_KBN_T"), "00")
        '            CODE_KBN = clsFUSION.fn_chenge_null(gdbrREADER.Item("CODE_KBN_T"), "0")
        '            LABEL_KBN = clsFUSION.fn_chenge_null(gdbrREADER.Item("LABEL_KBN_T"), "0")
        '            strITAKU_NNAME = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("ITAKU_NNAME_T"))
        '            strKIGYO_CODE = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("KIGYO_CODE_T"))
        '            strFURI_CODE = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("FURI_CODE_T"))
        '            strITAKU_CODE = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("ITAKU_CODE_T"))
        '            strUKETUKE_DATE = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("UKETUKE_DATE_S"))
        '            strTOUROKU_DATE = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("TOUROKU_DATE_S"))
        '            dblSYORI_KEN = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("SYORI_KEN_S"))
        '            dblSYORI_KIN = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("SYORI_KIN_S"))
        '        End While

        '        If intRECORD_COUNT > 0 Then
        '            '-------------------------------------------------
        '            '登録済み確認画面の表示
        '            '-------------------------------------------------

        '            MessageBox.Show(String.Format(MSG0049I, strITAKU_NNAME, FURI_DATE.Substring(0, 4) & "年 " & FURI_DATE.Substring(4, 2) & "月 " & FURI_DATE.Substring(6, 2) & "日 ", _
        '                          strKIGYO_CODE, strFURI_CODE, strITAKU_CODE, _
        '                          strUKETUKE_DATE.Substring(0, 4) & "年 " & strUKETUKE_DATE.Substring(4, 2) & "月 " & strUKETUKE_DATE.Substring(6, 2) & "日 ", _
        '                          strTOUROKU_DATE.Substring(0, 4) & "年 " & strTOUROKU_DATE.Substring(4, 2) & "月 " & strTOUROKU_DATE.Substring(6, 2) & "日 ", _
        '                          Format(dblSYORI_KEN, "#,##0"), Format(dblSYORI_KIN, "#,##0")), _
        '                          msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

        '            gdbcCONNECT.Close()
        '            Return False
        '        End If
        '        MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '        If rbKinko.Checked Then
        '            txtKinkoFuriDateY.Focus()
        '        Else
        '            txtCenterFuriDateY.Focus()
        '        End If
        '        gdbcCONNECT.Close()
        '        Return False
        '    Else
        '        '-----------------------------------------------
        '        'レコードか0件でなかった時、確認画面を表示する
        '        '-----------------------------------------------
        '        If BAITAI_CODE = "04" Or BAITAI_CODE = "09" Then
        '            If strUKETUKE_FLG = "0" Then
        '                MessageBox.Show(MSG0328W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '                gdbcCONNECT.Close()
        '                Return False
        '            End If
        '        End If

        '        strMessage = String.Format(MSG0050I, strITAKU_NNAME, _
        '                                   FURI_DATE.Substring(0, 4) & "年 " & FURI_DATE.Substring(4, 2) & "月 " & FURI_DATE.Substring(6, 2) & "日 ", _
        '                                   strKIGYO_CODE, strFURI_CODE, strITAKU_CODE, _
        '                                   strUKETUKE_DATE.Substring(0, 4) & "年 " & strUKETUKE_DATE.Substring(4, 2) & "月 " & strUKETUKE_DATE.Substring(6, 2) & "日 ", _
        '                                   strTOUROKU_DATE.Substring(0, 4) & "年 " & strTOUROKU_DATE.Substring(4, 2) & "月 " & strTOUROKU_DATE.Substring(6, 2) & "日 ")
        '        'strMessage = "登録処理を実行しますか？" & vbCrLf
        '        'strMessage &= "取引先名　　：" & strITAKU_NNAME & vbCrLf
        '        'strMessage &= "振替日　　　：" & gastrFURI_DATE_MAIN0100.Substring(0, 4) & "年 " & gastrFURI_DATE_MAIN0100.Substring(4, 2) & "月 " & gastrFURI_DATE_MAIN0100.Substring(6, 2) & "日 " & vbCrLf
        '        'strMessage &= "企業コード　：" & strKIGYO_CODE & vbCrLf
        '        'strMessage &= "振替コード　：" & strFURI_CODE & vbCrLf
        '        'strMessage &= "委託者コード：" & strITAKU_CODE & vbCrLf
        '        'strMessage &= "受付日　　　：" & strUKETUKE_DATE.Substring(0, 4) & "年 " & strUKETUKE_DATE.Substring(4, 2) & "月 " & strUKETUKE_DATE.Substring(6, 2) & "日 " & vbCrLf
        '        'strMessage &= "登録日　　　：" & strTOUROKU_DATE.Substring(0, 4) & "年 " & strTOUROKU_DATE.Substring(4, 2) & "月 " & strTOUROKU_DATE.Substring(6, 2) & "日 "
        '    End If
        '    gdbcCONNECT.Close()
        '    Return True

        'Catch ex As Exception
        '    MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(対象スケジュール件数取得)終了", "成功", "")
        '    gdbcCONNECT.Close()
        '    Return False
        'Finally
        '    'gdbcCONNECT.Close()
        'End Try
        '2013/12/24 saitou 標準版 性能向上 UPD --------------------------------------------------<<<<

    End Function

    Function fn_setJob() As Boolean
        '============================================================================
        'NAME           :fn_setJob
        'Parameter      :
        'Description    :ジョブマスタに登録
        'Return         :True=OK,False=NG
        'Create         :2009/08/25
        'Update         :
        '============================================================================
        '------------------------------------------------
        'ジョブマスタに登録
        '------------------------------------------------
        Try
            '2013/12/24 saitou 標準版 性能向上 ADD -------------------------------------------------->>>>
            Dim jobid As String = String.Empty
            Dim para As String = String.Empty
            '2013/12/24 saitou 標準版 性能向上 ADD --------------------------------------------------<<<<

            Select Case rbKinko.Checked
                Case True
                    '金庫持込
                    jobid = "J010"
                    para = TORI_CODE & "," & FURI_DATE & "," & CODE_KBN & "," & FMT_KBN _
                                    & "," & BAITAI_CODE & "," & LABEL_KBN
                Case False
                    'センター直接持込
                    jobid = "J011"
                    para = FURI_DATE & "," & BAITAI_CODE
            End Select

            '2013/12/24 saitou 標準版 性能向上 UPD -------------------------------------------------->>>>
            MainDB = New CASTCommon.MyOracle
            MainDB.BeginTrans()
            Dim iRet As Integer = BatchLog.SearchJOBMAST(jobid, para, MainDB)
            If iRet = 1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            ElseIf iRet = -1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            If BatchLog.InsertJOBMAST(jobid, GCom.GetUserID, para, MainDB) = False Then
                MainDB.Rollback()
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            MainDB.Commit()
            MainDB.Close()
            MainDB = Nothing

            MessageBox.Show(String.Format(MSG0021I, "落込"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            'If clsFUSION.fn_JOBMAST_TOUROKU_CHECK(jobid, gstrUSER_ID, para) = False Then
            '    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    gdbcCONNECT.Close()
            '    Return True
            'End If

            'If clsFUSION.fn_INSERT_JOBMAST(jobid, gstrUSER_ID, para) = False Then
            '    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    Return False
            'Else
            '    MessageBox.Show(MSG0021I.Replace("{0}", "落込"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            '    Return False
            'End If
            '2013/12/24 saitou 標準版 性能向上 UPD --------------------------------------------------<<<<

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ジョブ登録)", "失敗", ex.Message)
            Return False
        Finally
            '2013/12/24 saitou 標準版 性能向上 UPD -------------------------------------------------->>>>
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            '2013/12/24 saitou 標準版 性能向上 UPD --------------------------------------------------<<<<
        End Try

    End Function
    Function fn_getEntriAllSetJob(ByVal Key2 As String, ByVal Key3 As String, ByVal Key4 As String, ByVal Key5 As String) As Boolean
        '============================================================================
        'NAME           :fn_getEntriAllSetJob
        'Parameter      :Key2 - 振替日 / Key3 - 媒体コード / Key4 - 登録フラグ / Key5 - 受付フラグ　
        'Description    :依頼書・伝票のジョブ登録
        'Return         :True=OK,False=NG
        'Create         :2010/01/19 (なくなっていたので再追加)
        'Update         :
        '============================================================================
        '2013/12/24 saitou 標準版 性能向上 UPD -------------------------------------------------->>>>
        '作り直し
        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try
            Dim jobid As String = String.Empty
            Dim para As String = String.Empty

            '-----------------------------------------
            'パラメータ値設定
            '-----------------------------------------
            jobid = "J010"
            If Key3 = "04" Then
                '依頼書
                BAITAI_CODE = "04" '媒体コード
                FMT_KBN = "04"     'フォーマット区分
            Else
                '伝票
                BAITAI_CODE = "09" '媒体コード
                FMT_KBN = "05"     'フォーマット区分
            End If
            FURI_DATE = Key2
            '-----------------------------------------
            'レコードの件数取得
            '-----------------------------------------
            Dim SQL As New StringBuilder
            With SQL
                .Append("SELECT *")
                .Append(" FROM SCHMAST,TORIMAST")
                .Append(" WHERE FURI_DATE_S = " & SQ(Key2))
                .Append(" AND BAITAI_CODE_S = " & SQ(Key3))
                .Append(" AND TOUROKU_FLG_S = " & SQ(Key4))
                .Append(" AND UKETUKE_FLG_S = " & SQ(Key5))
                .Append(" AND TYUUDAN_FLG_S = '0'")
                .Append(" AND TORIS_CODE_S = TORIS_CODE_T")
                .Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                .Append(" ORDER BY TORIS_CODE_S,TORIF_CODE_S")
            End With

            MainDB = New CASTCommon.MyOracle
            MainDB.BeginTrans()
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    TORI_CODE = oraReader.GetItem("TORIS_CODE_T") & oraReader.GetItem("TORIF_CODE_T")
                    para = TORI_CODE & "," & FURI_DATE & "," & CODE_KBN & "," & FMT_KBN _
                            & "," & BAITAI_CODE & "," & LABEL_KBN

                    Dim iRet As Integer = BatchLog.SearchJOBMAST(jobid, para, MainDB)
                    If iRet = 1 Then
                        MainDB.Rollback()
                        MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    ElseIf iRet = -1 Then
                        MainDB.Rollback()
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                    End If

                    If BatchLog.InsertJOBMAST(jobid, GCom.GetUserID, para, MainDB) = False Then
                        MainDB.Rollback()
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                    End If

                    oraReader.NextRead()

                End While

                MainDB.Commit()

            End If

            MainDB.Close()
            MainDB = Nothing

            MessageBox.Show(String.Format(MSG0021I, "落込"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ジョブ登録)", "失敗", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If

            '最後まで残っていたらロールバック
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try

        'Try
        '    '2013/10/24 saitou 標準修正 ADD -------------------------------------------------->>>>
        '    Dim jobid As String = String.Empty
        '    Dim para As String = String.Empty
        '    '2013/10/24 saitou 標準修正 ADD --------------------------------------------------<<<<

        '    '-----------------------------------------
        '    'パラメータ値設定
        '    '-----------------------------------------
        '    jobid = "J010"
        '    If Key3 = "04" Then
        '        '依頼書
        '        BAITAI_CODE = "04" '媒体コード
        '        FMT_KBN = "04"     'フォーマット区分
        '    Else
        '        '伝票
        '        BAITAI_CODE = "09" '媒体コード
        '        FMT_KBN = "05"     'フォーマット区分
        '    End If
        '    FURI_DATE = Key2
        '    '-----------------------------------------
        '    'レコードの件数取得
        '    '-----------------------------------------
        '    gstrSSQL = ""
        '    gstrSSQL &= "SELECT *"
        '    gstrSSQL &= " FROM SCHMAST,TORIMAST"
        '    gstrSSQL &= " WHERE FURI_DATE_S = '" & Key2 & "'"
        '    gstrSSQL &= " AND BAITAI_CODE_S = '" & Key3 & "'"
        '    gstrSSQL &= " AND TORIS_CODE_S = TORIS_CODE_T"
        '    gstrSSQL &= " AND TORIF_CODE_S = TORIF_CODE_T"
        '    gstrSSQL &= " AND TOUROKU_FLG_S = '" & Key4 & "'"
        '    gstrSSQL &= " AND TYUUDAN_FLG_S = '" & Key4 & "'"
        '    gstrSSQL &= " AND UKETUKE_FLG_S = '" & Key5 & "'"
        '    gstrSSQL &= " ORDER BY TORIS_CODE_S,TORIF_CODE_S"

        '    gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        '    gdbcCONNECT.Open()

        '    gdbCOMMAND = New OracleClient.OracleCommand
        '    gdbCOMMAND.CommandText = gstrSSQL
        '    gdbCOMMAND.Connection = gdbcCONNECT

        '    gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

        '    '-----------------------------------------
        '    'レコードの件数取得
        '    '-----------------------------------------
        '    While gdbrREADER.Read() = True
        '        TORI_CODE = gdbrREADER.Item("TORIS_CODE_T") & gdbrREADER.Item("TORIF_CODE_T")

        '        para = TORI_CODE & "," & FURI_DATE & "," & CODE_KBN & "," & FMT_KBN _
        '                        & "," & BAITAI_CODE & "," & LABEL_KBN


        '        If clsFUSION.fn_JOBMAST_TOUROKU_CHECK(jobid, gstrUSER_ID, para) = False Then
        '            MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '            gdbcCONNECT.Close()
        '            Return True
        '        End If

        '        If clsFUSION.fn_INSERT_JOBMAST(jobid, gstrUSER_ID, para) = False Then
        '            MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '            Return False
        '        End If
        '    End While

        '    MessageBox.Show(MSG0021I.Replace("{0}", "落込"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        '    Return True

        'Catch ex As Exception
        '    MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ジョブ登録)", "失敗", ex.Message)
        '    Return False
        'Finally
        '    gdbcCONNECT.Close()
        'End Try
        '2013/12/24 saitou 標準版 性能向上 UPD --------------------------------------------------<<<<
    End Function
#End Region

End Class
