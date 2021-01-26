Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports CASTCommon

Public Class KFSMAIN010

#Region "変数宣言"

    Private BatchLog As New CASTCommon.BatchLOG("KFSMAIN010", "振込依頼データ落込画面")

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private Const msgTitle As String = "振込依頼データ落込画面(KFSMAIN010)"

    Private TORI_CODE As String = ""
    Private FURI_DATE As String = ""
    Private CODE_KBN As String = ""
    Private FMT_KBN As String = ""
    Private BAITAI_CODE As String = ""
    Private LABEL_KBN As String = ""
    Private ITAKU_KANRI_CODE_T As String = ""

#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFSMAIN010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)開始", "成功", "")

            '------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------
            GCom.GetUserID = gstrUSER_ID
            GCom.GetSysDate = Date.Now
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------
            '委託者名リストボックスの設定
            '------------------------------------------
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "3") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)", "失敗", ex.Message)
            Me.Close()

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "ボタン"

    '登録ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Dim strMessage As String = ""
        Dim TOUROKU_FLG As String = ""                 '登録フラグ、中断フラグ
        Dim UKETUKE_FLG As String = ""                 '受付フラグ
        Dim LogTori As String = "0000000000-00" 'ログ書込用取引先
        Dim LogFuri As String = "00000000"      'ログ書込用振込日

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
            TORI_CODE = txtTorisCode.Text & txtTorifCode.Text
            FURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            BAITAI_CODE = "00"
            TOUROKU_FLG = "0"
            UKETUKE_FLG = "0"
            LogTori = txtTorisCode.Text & "-" & txtTorifCode.Text
            LogFuri = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'スケジュール検索
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Select Case TORI_CODE
                Case "111111111111"
                    '----------------------------------------
                    '個別落込 － 依頼書ベース全件
                    '----------------------------------------
                    BAITAI_CODE = "04"                     '媒体コード
                    TOUROKU_FLG = "0"                      '登録フラグ、中断フラグ
                    UKETUKE_FLG = "1"                      '受付フラグ
                    If fn_getEntriAllMain(TOUROKU_FLG, UKETUKE_FLG, strMessage) = False Then
                        Exit Sub
                    End If

                Case "222222222222"
                    '----------------------------------------
                    '個別落込 － 伝票ベース全件
                    '----------------------------------------
                    BAITAI_CODE = "09"                     '媒体コード
                    TOUROKU_FLG = "0"                      '登録フラグ、中断フラグ
                    UKETUKE_FLG = "1"                      '受付フラグ
                    If fn_getEntriAllMain(TOUROKU_FLG, UKETUKE_FLG, strMessage) = False Then
                        Exit Sub
                    End If

                Case Else
                    '----------------------------------------
                    '個別落込 － 取引先指定
                    '----------------------------------------
                    If fn_getKobetuMain(TOUROKU_FLG, strMessage) = False Then
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
                Select Case TORI_CODE
                    Case "111111111111", "222222222222"
                        If fn_getEntriAllSetJob(TOUROKU_FLG, UKETUKE_FLG) = False Then
                            Exit Sub
                        End If
                    Case Else
                        'バッチ実行
                        If fn_SetJob() = False Then
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
    Handles txtTorisCode.Validating, txtTorifCode.Validating, txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating

        Try

            Call GCom.NzNumberString(CType(sender, TextBox), True)

            'サイクル連携に対応
            If rbRenkei.Checked = True AndAlso _
                (CType(sender, TextBox).Name = "txtTorisCode" OrElse _
                 CType(sender, TextBox).Name = "txtTorifCode") Then
                Call IsExistToriMast(txtTorisCode.Text, txtTorifCode.Text)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(テキスト制御)", "失敗", ex.Message)

        Finally

        End Try
    End Sub

    '何故か制御出来ないので・・・
    Private Sub txtFuriDateD_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtFuriDateD.KeyPress
        Try

            Select Case e.KeyChar
                Case Microsoft.VisualBasic.ChrW(Keys.Enter)
                    ' Enterキーで，実行ボタンにフォーカスを設定する
                    e.Handled = False
                    btnAction.Select()
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(テキスト制御)", "失敗", ex.Message)

        Finally

        End Try

    End Sub

#End Region

#Region "ラジオボタン"

    'モード キープレス
    Private Sub RadMode_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles rbKobetu.KeyPress, rbRenkei.KeyPress
        Try

            Select Case e.KeyChar
                Case Microsoft.VisualBasic.ChrW(Keys.Enter)
                    ' Enterキーで，実行ボタンにフォーカスを設定する
                    e.Handled = False
                    txtTorisCode.Select()
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ラジオボタン制御)", "失敗", ex.Message)
        Finally

        End Try
    End Sub

    Private Sub rbKobetu_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbKobetu.CheckedChanged, rbRenkei.CheckedChanged
        Try
            Me.SuspendLayout()
            txtFuriDateY.Text = ""
            txtFuriDateM.Text = ""
            txtFuriDateD.Text = ""
            cmbCycle.Items.Clear()

            If rbKobetu.Checked = True Then
                lblCycle.Visible = False
                cmbCycle.Visible = False
                pFuriDate.Visible = True
                Label12.Visible = True
                Label13.Visible = True
            End If

            If rbRenkei.Checked = True Then
                lblCycle.Visible = True
                cmbCycle.Visible = True
                pFuriDate.Visible = False
                Label12.Visible = False
                Label13.Visible = False

                Call IsExistToriMast(txtTorisCode.Text, txtTorifCode.Text)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ラジオボタン切替)", "失敗", ex.Message)
        Finally
            Me.ResumeLayout()
        End Try
    End Sub

#End Region

#Region "コンボボックス"

    '委託者名リストボックスの設定
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '選択カナで始まる委託者名を取得
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "3") = -1 Then
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

                If rbRenkei.Checked = True Then
                    Call IsExistToriMast(txtTorisCode.Text, txtTorifCode.Text)
                Else
                    txtFuriDateY.Focus()
                End If

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取引先コード設定)", "失敗", ex.Message)

        Finally

        End Try
    End Sub

    'サイクルコンボＥｎｔｅｒ押下
    Private Sub cmbCycle_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cmbCycle.KeyPress
        Try

            Select Case e.KeyChar
                Case Microsoft.VisualBasic.ChrW(Keys.Enter)
                    ' Enterキーで，実行ボタンにフォーカスを設定する
                    e.Handled = False
                    btnAction.Select()
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(サイクルコンボ制御)", "失敗", ex.Message)

        Finally

        End Try

    End Sub

#End Region

#Region "関数"

    Function fn_check_text() As Boolean
        Dim CheckFuriDateY As TextBox = Nothing
        Dim CheckFuriDateM As TextBox = Nothing
        Dim CheckFuriDateD As TextBox = Nothing
        Dim CheckToriCode As String

        Try
            If Me.txtTorisCode.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorisCode.Focus()
                Return False
            End If

            If Me.txtTorifCode.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorifCode.Focus()
                Return False
            End If

            CheckFuriDateY = txtFuriDateY
            CheckFuriDateM = txtFuriDateM
            CheckFuriDateD = txtFuriDateD
            CheckToriCode = txtTorisCode.Text & txtTorisCode.Text

            '--------------------------------------
            '共通日付入力チェック
            '--------------------------------------
            If rbKobetu.Checked = True Then
                If Me.txtFuriDateY.Text.Trim = String.Empty Then
                    MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtFuriDateY.Focus()
                    Return False
                End If

                If Me.txtFuriDateM.Text.Trim = String.Empty Then
                    MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtFuriDateM.Focus()
                    Return False
                Else
                    If CInt(txtFuriDateM.Text) < 1 OrElse CInt(txtFuriDateM.Text) > 12 Then
                        MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtFuriDateM.Focus()
                        Return False
                    End If
                End If

                If Me.txtFuriDateD.Text.Trim = String.Empty Then
                    MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtFuriDateD.Focus()
                    Return False
                Else
                    If CInt(txtFuriDateD.Text) < 1 OrElse CInt(txtFuriDateD.Text) > 31 Then
                        MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtFuriDateD.Focus()
                        Return False
                    End If
                End If
            Else
                'サイクル連携時の入力チェック
                FURI_DATE = "00000000"

                '一括処理は不可
                Select Case CheckToriCode
                    Case "111111111111", "222222222222"
                        MessageBox.Show(MSG0500W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtTorisCode.Focus()
                        Return False
                End Select

                If cmbCycle.Text.Trim = "" Then
                    MessageBox.Show(MSG0501W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    cmbCycle.Focus()
                    Return False
                End If
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(入力チェック処理)", "失敗", ex.Message)
            Return False

        Finally

        End Try
    End Function

    Function fn_getEntriAllMain(ByVal TOUROKU_FLG As String, ByVal UKETUKE_FLG As String, ByRef strMessage As String) As Boolean
        Dim lngRecordCount As Long = 0

        Dim SQL As StringBuilder = New StringBuilder(128)
        Dim MainDB As MyOracle = Nothing
        Dim OraReader As MyOracleReader = Nothing

        Try

            '-----------------------------------------
            'フォーマット区分設定
            '-----------------------------------------
            Select Case BAITAI_CODE
                Case "04" : FMT_KBN = "04"
                Case "09" : FMT_KBN = "05"
                Case Else : FMT_KBN = "00"
            End Select

            '-----------------------------------------
            'レコードの件数取得
            '-----------------------------------------
            SQL.AppendLine("SELECT COUNT(*) AS RECORDCOUNT")
            SQL.AppendLine(" FROM S_SCHMAST, S_TORIMAST")
            SQL.AppendLine(" WHERE FURI_DATE_S = '" & FURI_DATE & "'")
            SQL.AppendLine(" AND BAITAI_CODE_S = '" & BAITAI_CODE & "'")
            SQL.AppendLine(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.AppendLine(" AND TOUROKU_FLG_S = '" & TOUROKU_FLG & "'")
            SQL.AppendLine(" AND TYUUDAN_FLG_S = '" & TOUROKU_FLG & "'")
            SQL.AppendLine(" AND UKETUKE_FLG_S = '" & UKETUKE_FLG & "'")
            SQL.AppendLine(" ORDER BY TORIS_CODE_S, TORIF_CODE_S")

            MainDB = New MyOracle
            OraReader = New MyOracleReader(MainDB)

            '-----------------------------------------
            'レコードの件数取得
            '-----------------------------------------
            If OraReader.DataReader(SQL) = True Then
                lngRecordCount = OraReader.GetInt64("RECORDCOUNT")

                If lngRecordCount = 0 Then
                    MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateY.Focus()
                    Return False
                Else
                    strMessage = "登録処理を続行しますか？" & vbCrLf & vbCrLf & _
                                    "振込日　：　" & FURI_DATE.Substring(0, 4) & "年 " & FURI_DATE.Substring(4, 2) & "月 " & FURI_DATE.Substring(6, 2) & "日" & vbCrLf & _
                                    "件　数　：　" & lngRecordCount
                End If
            Else
                MessageBox.Show(String.Format(MSG0027E, "スケジュールマスタ", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(対象スケジュール件数取得)", "終了", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function

    Function fn_getKobetuMain(ByVal TOUROKU_FLG As String, ByRef strMessage As String) As Boolean
        Dim lngRECORD_COUNT As Long = 0
        Dim strITAKU_NNAME As String = ""
        Dim strSYUBETU As String = ""
        Dim strITAKU_CODE As String = ""
        Dim strUKETUKE_DATE As String = ""
        Dim strTOUROKU_DATE As String = ""
        Dim lngSYORI_KEN As Long = 0
        Dim lngSYORI_KIN As Long = 0
        Dim strUKETUKE_FLG As String = ""

        Dim strCYCLE As String = ""
        Dim strKIJITU_KANRI As String = ""

        Dim SQL As StringBuilder = New StringBuilder(128)
        Dim MainDB As MyOracle = Nothing
        Dim OraReader As MyOracleReader = Nothing

        Try
            '総振用マスタ設定取得
            SQL.AppendLine("SELECT * FROM S_TORIMAST")
            SQL.AppendLine(" WHERE TORIS_CODE_T = '" & TORI_CODE.Substring(0, 10) & "'")
            SQL.AppendLine(" AND TORIF_CODE_T = '" & TORI_CODE.Substring(10, 2) & "'")

            MainDB = New MyOracle
            OraReader = New MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) = True Then
                '取引先がある場合は設定取得
                BAITAI_CODE = OraReader.GetString("BAITAI_CODE_T")
                FMT_KBN = OraReader.GetString("FMT_KBN_T")
                CODE_KBN = OraReader.GetString("CODE_KBN_T")
                LABEL_KBN = OraReader.GetString("LABEL_KBN_T")
                strITAKU_NNAME = OraReader.GetString("ITAKU_NNAME_T")
                strSYUBETU = OraReader.GetString("SYUBETU_T")
                strITAKU_CODE = OraReader.GetString("ITAKU_CODE_T")
                ITAKU_KANRI_CODE_T = OraReader.GetString("ITAKU_KANRI_CODE_T")

                strCYCLE = OraReader.GetString("CYCLE_T")
                strKIJITU_KANRI = OraReader.GetString("KIJITU_KANRI_T")

            Else
                '取引先が無い場合
                OraReader.Close()

                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                If rbKobetu.Checked Then
                    txtFuriDateY.Focus()
                End If

                Return False
            End If

            OraReader.Close()

            'サイクル連携の場合
            If rbRenkei.Checked = True Then
                strMessage = String.Format("登録処理を実行しますか？(ID:MSG0050I)" & vbCrLf & _
                                            "取引先名　　：{0}" & vbCrLf & _
                                            "振込日　　　：{1}" & vbCrLf & _
                                            "種別　　　　：{2}" & vbCrLf & _
                                            "委託者コード：{3}" & vbCrLf & _
                                            "受付日　　　：{4}" & vbCrLf & _
                                            "登録日　　　：{5}", strITAKU_NNAME, _
                                           "0000年 00月 00日 ", _
                                           strSYUBETU, strITAKU_CODE, _
                                           "0000年 00月 00日 ", _
                                           "0000年 00月 00日 ")
                Return True
            End If

            ' 依頼書・伝票でない場合、複数回持込あり かつ 期日管理しない場合はスケジュールの存在チェックを行わない
            If BAITAI_CODE <> "04" AndAlso BAITAI_CODE <> "09" Then
                If strCYCLE = "1" AndAlso strKIJITU_KANRI = "0" Then
                    strMessage = String.Format("登録処理を実行しますか？(ID:MSG0050I)" & vbCrLf & _
                                                "取引先名　　：{0}" & vbCrLf & _
                                                "振込日　　　：{1}" & vbCrLf & _
                                                "種別　　　　：{2}" & vbCrLf & _
                                                "委託者コード：{3}" & vbCrLf & _
                                                "受付日　　　：{4}" & vbCrLf & _
                                                "登録日　　　：{5}", strITAKU_NNAME, _
                                               FURI_DATE.Substring(0, 4) & "年 " & FURI_DATE.Substring(4, 2) & "月 " & FURI_DATE.Substring(6, 2) & "日 ", _
                                               strSYUBETU, strITAKU_CODE, _
                                               "0000年 00月 00日 ", _
                                               "0000年 00月 00日 ")
                    Return True
                End If
            End If

            'スケジュールの有無のチェック
            SQL.Length = 0
            SQL.AppendLine("SELECT * FROM S_SCHMAST")
            SQL.AppendLine(" WHERE TORIS_CODE_S = '" & TORI_CODE.Substring(0, 10) & "'")
            SQL.AppendLine(" AND TORIF_CODE_S = '" & TORI_CODE.Substring(10, 2) & "'")
            SQL.AppendLine(" AND FURI_DATE_S = '" & FURI_DATE & "'")
            SQL.AppendLine(" ORDER BY MOTIKOMI_SEQ_S")

            If OraReader.DataReader(SQL) = False Then
                '依頼書・伝票の場合は合計処理をする
                If BAITAI_CODE = "04" OrElse BAITAI_CODE = "09" Then
                    MessageBox.Show(MSG0328W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If

                ' 複数回持込なし かつ 期日管理しない場合
                If strCYCLE = "0" AndAlso strKIJITU_KANRI = "0" Then
                    strMessage = String.Format("登録処理を実行しますか？(ID:MSG0050I)" & vbCrLf & _
                                              "取引先名　　：{0}" & vbCrLf & _
                                              "振込日　　　：{1}" & vbCrLf & _
                                              "種別　　　　：{2}" & vbCrLf & _
                                              "委託者コード：{3}" & vbCrLf & _
                                              "受付日　　　：{4}" & vbCrLf & _
                                              "登録日　　　：{5}", strITAKU_NNAME, _
                                             FURI_DATE.Substring(0, 4) & "年 " & FURI_DATE.Substring(4, 2) & "月 " & FURI_DATE.Substring(6, 2) & "日 ", _
                                             strSYUBETU, strITAKU_CODE, _
                                             "0000年 00月 00日 ", _
                                             "0000年 00月 00日 ")
                    Return True

                Else
                    MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    If rbKobetu.Checked Then
                        txtFuriDateY.Focus()
                    End If

                    Return False
                End If

                'スケジュールが存在する場合
            Else
                If strCYCLE = "0" AndAlso strKIJITU_KANRI = "0" Then
                    ' 期日管理なしの場合で，複数回持ち込みもない場合，
                    ' 既存スケジュールが存在する場合エラー
                    Do While OraReader.EOF = False
                        If OraReader.GetString("TOUROKU_FLG_S") <> "0" AndAlso OraReader.GetString("TYUUDAN_FLG_S") = "0" Then
                            strUKETUKE_DATE = OraReader.GetString("UKETUKE_DATE_S")
                            strTOUROKU_DATE = OraReader.GetString("TOUROKU_DATE_S")
                            lngSYORI_KEN = OraReader.GetInt64("SYORI_KEN_S")
                            lngSYORI_KIN = OraReader.GetInt64("SYORI_KIN_S")

                            MessageBox.Show(String.Format("登録済みです。(ID:MSG0049I)" & vbCrLf & _
                                                            "取引先名　　：{0}" & vbCrLf & _
                                                            "振込日　　　：{1}" & vbCrLf & _
                                                            "種別　　　　：{2}" & vbCrLf & _
                                                            "委託者コード：{3}" & vbCrLf & _
                                                            "受付日　　　：{4}" & vbCrLf & _
                                                            "登録日　　　：{5}" & vbCrLf & _
                                                            "処理件数　　：{6}" & vbCrLf & _
                                                            "処理金額　　：{7}", strITAKU_NNAME, FURI_DATE.Substring(0, 4) & "年 " & FURI_DATE.Substring(4, 2) & "月 " & FURI_DATE.Substring(6, 2) & "日 ", _
                                                            strSYUBETU, strITAKU_CODE, _
                                                            strUKETUKE_DATE.Substring(0, 4) & "年 " & strUKETUKE_DATE.Substring(4, 2) & "月 " & strUKETUKE_DATE.Substring(6, 2) & "日 ", _
                                                            strTOUROKU_DATE.Substring(0, 4) & "年 " & strTOUROKU_DATE.Substring(4, 2) & "月 " & strTOUROKU_DATE.Substring(6, 2) & "日 ", _
                                                            lngSYORI_KEN.ToString("#,##0"), lngSYORI_KIN.ToString("#,##0")), _
                                                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Return False
                        End If

                        OraReader.NextRead()
                    Loop

                    strMessage = String.Format("登録処理を実行しますか？(ID:MSG0050I)" & vbCrLf & _
                                        "取引先名　　：{0}" & vbCrLf & _
                                        "振込日　　　：{1}" & vbCrLf & _
                                        "種別　　　　：{2}" & vbCrLf & _
                                        "委託者コード：{3}" & vbCrLf & _
                                        "受付日　　　：{4}" & vbCrLf & _
                                        "登録日　　　：{5}", strITAKU_NNAME, _
                                       FURI_DATE.Substring(0, 4) & "年 " & FURI_DATE.Substring(4, 2) & "月 " & FURI_DATE.Substring(6, 2) & "日 ", _
                                       strSYUBETU, strITAKU_CODE, _
                                       "0000年 00月 00日 ", _
                                       "0000年 00月 00日 ")
                    Return True
                End If
            End If

            '中断済みの場合
            If OraReader.GetString("TYUUDAN_FLG_S") <> "0" Then
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                If rbKobetu.Checked Then
                    txtFuriDateY.Focus()
                End If

                Return False
            End If

            '登録済みの場合
            If OraReader.GetString("TOUROKU_FLG_S") <> "0" Then
                strUKETUKE_DATE = OraReader.GetString("UKETUKE_DATE_S")
                strTOUROKU_DATE = OraReader.GetString("TOUROKU_DATE_S")
                lngSYORI_KEN = OraReader.GetInt64("SYORI_KEN_S")
                lngSYORI_KIN = OraReader.GetInt64("SYORI_KIN_S")

                MessageBox.Show(String.Format("登録済みです。(ID:MSG0049I)" & vbCrLf & _
                                                "取引先名　　：{0}" & vbCrLf & _
                                                "振込日　　　：{1}" & vbCrLf & _
                                                "種別　　　　：{2}" & vbCrLf & _
                                                "委託者コード：{3}" & vbCrLf & _
                                                "受付日　　　：{4}" & vbCrLf & _
                                                "登録日　　　：{5}" & vbCrLf & _
                                                "処理件数　　：{6}" & vbCrLf & _
                                                "処理金額　　：{7}", strITAKU_NNAME, FURI_DATE.Substring(0, 4) & "年 " & FURI_DATE.Substring(4, 2) & "月 " & FURI_DATE.Substring(6, 2) & "日 ", _
                                                strSYUBETU, strITAKU_CODE, _
                                                strUKETUKE_DATE.Substring(0, 4) & "年 " & strUKETUKE_DATE.Substring(4, 2) & "月 " & strUKETUKE_DATE.Substring(6, 2) & "日 ", _
                                                strTOUROKU_DATE.Substring(0, 4) & "年 " & strTOUROKU_DATE.Substring(4, 2) & "月 " & strTOUROKU_DATE.Substring(6, 2) & "日 ", _
                                                lngSYORI_KEN.ToString("#,##0"), lngSYORI_KIN.ToString("#,##0")), _
                                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Return False
            Else
                strMessage = String.Format("登録処理を実行しますか？(ID:MSG0050I)" & vbCrLf & _
                           "取引先名　　：{0}" & vbCrLf & _
                           "振込日　　　：{1}" & vbCrLf & _
                           "種別　　　　：{2}" & vbCrLf & _
                           "委託者コード：{3}" & vbCrLf & _
                           "受付日　　　：{4}" & vbCrLf & _
                           "登録日　　　：{5}", strITAKU_NNAME, _
                          FURI_DATE.Substring(0, 4) & "年 " & FURI_DATE.Substring(4, 2) & "月 " & FURI_DATE.Substring(6, 2) & "日 ", _
                          strSYUBETU, strITAKU_CODE, _
                          "0000年 00月 00日 ", _
                          "0000年 00月 00日 ")
                Return True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(対象スケジュール件数取得)終了", "成功", "")
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function

    Function fn_SetJob() As Boolean
        '------------------------------------------------
        'ジョブマスタに登録
        '------------------------------------------------
        Dim MainDB As CASTCommon.MyOracle = Nothing
        Try
            Dim JOB_ID As String = ""
            Dim PARAMETA As String = ""

            'サイクル連携に対応
            If rbRenkei.Checked = True Then
                '他システム連携
                JOB_ID = "KFT010"
                'ファイル名はD代表委託者コード+サイクル(2桁).DAT
                Dim FileName As String = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "DEN"), "D" & ITAKU_KANRI_CODE_T & cmbCycle.Text & ".DAT")

                If Not File.Exists(FileName) Then
                    MessageBox.Show(MSG0502W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If

                Dim ProcInfo As New ProcessStartInfo
                ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "EXE"), JOB_ID)
                ProcInfo.Arguments = FileName

                ProcInfo.UseShellExecute = False
                ProcInfo.CreateNoWindow = True

                Dim Proc As Process = Process.Start(ProcInfo)

                Proc.WaitForExit()

                If Proc.ExitCode <> 0 Then
                    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Else
                    MessageBox.Show(MSG0021I.Replace("{0}", "落込"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return True
                End If

            Else
                '個別落込
                JOB_ID = "S010"
                PARAMETA = TORI_CODE & "," & FURI_DATE & "," & CODE_KBN & "," & FMT_KBN _
                                & "," & BAITAI_CODE & "," & LABEL_KBN

                MainDB = New CASTCommon.MyOracle
                MainDB.BeginTrans()
                Dim iRet As Integer = BatchLog.SearchJOBMAST(JOB_ID, PARAMETA, MainDB)
                If iRet = 1 Then
                    MainDB.Rollback()
                    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                ElseIf iRet = -1 Then
                    MainDB.Rollback()
                    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If

                If BatchLog.InsertJOBMAST(JOB_ID, gstrUSER_ID, PARAMETA, MainDB) = False Then
                    MainDB.Rollback()
                    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Else
                    MainDB.Commit()
                    MessageBox.Show(MSG0021I.Replace("{0}", "落込"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return True
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ジョブ登録)", "失敗", ex.Message)
            Return False
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try

    End Function

    Private Function fn_getEntriAllSetJob(ByVal TOUROKU_FLG As String, ByVal UKETUKE_FLG As String) As Boolean
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim MainDB As CASTCommon.MyOracle = Nothing
        Try
            '-----------------------------------------
            'パラメータ値設定
            '-----------------------------------------
            Dim JOB_ID As String = "S010"
            Dim PARAMETA As String = ""

            If BAITAI_CODE = "04" Then
                '依頼書
                BAITAI_CODE = "04" '媒体コード
                FMT_KBN = "04"     'フォーマット区分
            Else
                '伝票
                BAITAI_CODE = "09" '媒体コード
                FMT_KBN = "05"     'フォーマット区分
            End If

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            '-----------------------------------------
            'レコードの件数取得
            '-----------------------------------------
            Dim SQL As New StringBuilder(128)
            SQL.AppendLine("SELECT")
            SQL.AppendLine(" TORIS_CODE_T, TORIF_CODE_T, CODE_KBN_T, LABEL_KBN_T")
            SQL.AppendLine(" FROM S_SCHMAST, S_TORIMAST")
            SQL.AppendLine(" WHERE FURI_DATE_S = '" & FURI_DATE & "'")
            SQL.AppendLine(" AND BAITAI_CODE_S = '" & BAITAI_CODE & "'")
            SQL.AppendLine(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.AppendLine(" AND TOUROKU_FLG_S = '" & TOUROKU_FLG & "'")
            SQL.AppendLine(" AND TYUUDAN_FLG_S = '" & TOUROKU_FLG & "'")
            SQL.AppendLine(" AND UKETUKE_FLG_S = '" & UKETUKE_FLG & "'")
            SQL.AppendLine(" ORDER BY TORIS_CODE_S, TORIF_CODE_S, MOTIKOMI_SEQ_S")

            MainDB.BeginTrans()
            If OraReader.DataReader(SQL) = True Then
                Do
                    TORI_CODE = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")
                    CODE_KBN = OraReader.GetString("CODE_KBN_T")
                    LABEL_KBN = OraReader.GetString("LABEL_KBN_T")
                    
                    PARAMETA = TORI_CODE & "," & FURI_DATE & "," & CODE_KBN & "," & FMT_KBN _
                                    & "," & BAITAI_CODE & "," & LABEL_KBN
                    Dim iRet As Integer = BatchLog.SearchJOBMAST(JOB_ID, PARAMETA, MainDB)
                    If iRet = 1 Then
                        MainDB.Rollback()
                        MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    ElseIf iRet = -1 Then
                        MainDB.Rollback()
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                    End If

                    If BatchLog.InsertJOBMAST(JOB_ID, gstrUSER_ID, PARAMETA, MainDB) = False Then
                        MainDB.Rollback()
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                    End If

                    OraReader.NextRead()
                Loop Until OraReader.EOF

                MainDB.Commit()

            End If

            MainDB.Close()
            MainDB = Nothing

            MessageBox.Show(MSG0021I.Replace("{0}", "落込"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ジョブ登録)", "失敗", ex.Message)
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
        End Try

    End Function

    Private Sub IsExistToriMast(ByVal TorisCode As String, ByVal TorifCode As String)
        If TorisCode.Trim.Length <> 10 OrElse TorifCode.Trim.Length <> 2 Then
            Exit Sub
        End If

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader

            Dim SQL As New StringBuilder(128)

            SQL.Append(" SELECT BAITAI_CODE_T,ITAKU_KANRI_CODE_T FROM S_TORIMAST ")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(TorisCode))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(TorifCode))
           
            If OraReader.DataReader(SQL) Then
                cmbCycle.Items.Clear()

                Dim DenDir As String = CASTCommon.GetFSKJIni("COMMON", "DEN")
                Dim DirInfo() As String = Directory.GetFiles(DenDir, "D" & OraReader.GetItem("ITAKU_KANRI_CODE_T") & "*" & ".DAT")

                For cnt As Integer = 0 To DirInfo.Length - 1
                    Dim filename As String = Path.GetFileNameWithoutExtension(DirInfo(cnt))
                    Dim Cycle As String = filename.Substring(filename.Length - 2)
                    cmbCycle.Items.Add(Cycle)
                Next

                If cmbCycle.Items.Count > 0 Then cmbCycle.SelectedIndex = 0
                cmbCycle.Focus()
            End If

        Catch
            Throw
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

    End Sub

#End Region

End Class
