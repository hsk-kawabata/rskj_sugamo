Imports System
Imports System.Data
Imports System.Windows.Forms
Imports System.Text
Imports CASTCommon
Imports System.Runtime.InteropServices
Imports System.DateTime

Public Class KFUMAST030

#Region "宣言"

    Inherits System.Windows.Forms.Form

    Private MainLog As New CASTCommon.BatchLOG("KFUMAST030", "休日情報マスタメンテナンス")
    Private Const ErrMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"

    'パラメータ用
    Private strTUUBAN As String

    'SQLキー項目用
    Private strKEY1 As String
    Private strKEY2 As String
    Private strKEY3 As String
    Private strKEY4 As String
    Private strKEY5 As String
    '祝日リストボックス用配列
    Private strSYUKU_NM() As String
    Private strSYUKU_DATE() As String

    '共通イベントコントロール
    Private CAST As New CASTCommon.Events
    Private CASTx2 As New CASTCommon.Events("", CASTCommon.Events.KeyMode.BAD)
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MyOwnerForm As Form
    Const msgTitle As String = "休日情報マスタメンテナンス(KFUMAST030)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle

#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFUMAST030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim CmdLines As String() = Environment.GetCommandLineArgs

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            MyOwnerForm = GOwnerForm
            GOwnerForm = Me
            Me.btnEnd.DialogResult = DialogResult.None

            'ユーザID／システム日付表示
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)


            If fn_clear_GAMEN() = False Then
                Exit Sub
            End If
            txtNendo.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")

        End Try
    End Sub

    '画面クローズ
    Private Sub Form1_FormClosing(ByVal sender As Object, _
        ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing

        Try
            MainLog.Write("0000000000-00", "00000000", "(クローズ)開始", "成功", "")
            'If e.CloseReason = CloseReason.UserClosing Then
            '    Me.Owner.Show()
            'End If
        Catch ex As Exception
            MainLog.Write("0000000000-00", "00000000", "(クローズ)", "失敗", ex.Message)
        Finally
            MainLog.Write("0000000000-00", "00000000", "(クローズ)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "ボタン"

    '登録ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            Dim YasumiKen As Integer
            MainLog.Write("0000000000-00", "00000000", "(登録)開始", "成功", "")

            strKEY1 = txtNendo.Text.PadLeft(4, "0")
            strKEY2 = txtNendo.Text.PadLeft(4, "0") & txtTuki.Text.PadLeft(2, "0") & txtHi.Text.PadLeft(2, "0")
            strKEY3 = txtKyujitu_NM.Text
            strKEY4 = Format(Now, "yyyyMMdd")
            strKEY5 = "00000000"

            Cursor.Current = Cursors.WaitCursor()

            '------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------
            If fn_check_text() = False Then

                Exit Sub
            End If

            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = Windows.Forms.DialogResult.No Then
                Return
            End If

            MainDB = New CASTCommon.MyOracle

            '----------------------------------------
            '休日マスタ(YASUMIMAST)に既に登録済みか検索
            '----------------------------------------
            If fn_KENSAKU_YASUMIMAST(strKEY2) = True Then
                MessageBox.Show(MSG0029W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLog.Write("0000000000-00", "00000000", "(登録)終了", "成功", "")
                Exit Sub
            End If

            If fn_insert_YASUMIMAST() = True Then
                If fn_select_YASUMIMAST(YasumiKen) = True Then
                    MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLog.Write("0000000000-00", "00000000", "(登録)終了", "成功", "")
                    Exit Sub
                End If
            Else
                MessageBox.Show(MSG0002E.Replace("{0}", "登録"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLog.Write("0000000000-00", "00000000", "(登録)終了", "成功", "")
                Exit Sub
            End If

            If fn_select_YASUMIMAST(YasumiKen) = True Then
                '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------START
                'btnKousin.Enabled = True
                'btnDelete.Enabled = True
                'btnSansyo.Enabled = False
                'btnKousin.Focus()
                '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------END
            Else
                txtNendo.Focus()
                txtNendo.SelectionStart = 0
                txtNendo.SelectionLength = 4
            End If
            '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------START
            'Me.btnAction.Enabled = False
            '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------END
            'MainLog.Write("0000000000-00", "00000000", "(登録)終了", "成功")

            MainDB.Close()
            MainDB = Nothing

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(登録)", "失敗", ex.Message)

        Finally
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLog.Write("0000000000-00", "00000000", "(登録)終了", "成功", "")
        End Try

    End Sub

    '参照ボタン
    Private Sub btnSansyo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSansyo.Click

        Try
            Dim YasumiKen As Integer
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            strKEY1 = txtNendo.Text.PadLeft(4, "0")
            Cursor.Current = Cursors.WaitCursor()

            '------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------
            If txtNendo.TextLength = 0 Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtNendo.Focus()
                Exit Sub
            End If

            MainDB = New CASTCommon.MyOracle

            If fn_select_YASUMIMAST(YasumiKen) = True Then
                If YasumiKen = 0 Then
                    MessageBox.Show(MSG0028W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtNendo.Focus()
                    Exit Sub
                Else
                    btnKousin.Enabled = True
                    btnDelete.Enabled = True

                    '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------START
                    'btnAction.Enabled = False
                    '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------END

                    btnSansyo.Enabled = False
                    txtNendo.Enabled = False
                    txtTuki.Focus()
                End If
            Else
                txtNendo.Focus()
                txtNendo.SelectionStart = 0
                txtNendo.SelectionLength = 4
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)", "失敗", ex.Message)

        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")

        End Try
    End Sub

    '更新ボタン
    Private Sub btnKousin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKousin.Click

        Try
            Dim YasumiKen As Integer
            MainLog.Write("0000000000-00", "00000000", "(更新)開始", "成功", "")

            strKEY1 = txtNendo.Text.PadLeft(4, "0")
            strKEY2 = txtNendo.Text.PadLeft(4, "0") & txtTuki.Text.PadLeft(2, "0") & txtHi.Text.PadLeft(2, "0")
            strKEY3 = txtKyujitu_NM.Text
            strKEY5 = Format(Now, "yyyyMMdd")

            Cursor.Current = Cursors.WaitCursor()

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '必須項目の入力確認（必須項目：取引先コード、契約者コード）
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If

            MainDB = New CASTCommon.MyOracle

            '----------------------------------------
            '休日マスタ(YASUMIMAST)に既に登録済みか検索
            '----------------------------------------
            If fn_KENSAKU_YASUMIMAST(strKEY2) = False Then
                MessageBox.Show(MSG0030W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '確認メッセージを表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            If fn_update_YASUMIMAST() = True Then
                If fn_select_YASUMIMAST(YasumiKen) = True Then
                    MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            Else
                MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            MainDB.Close()
            MainDB = Nothing

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(更新)", "失敗", ex.Message)

        Finally
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLog.Write("0000000000-00", "00000000", "(更新)終了", "成功", "")

        End Try
    End Sub

    '削除ボタン
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        Try
            Dim YasumiKen As Integer
            MainLog.Write("0000000000-00", "00000000", "(削除)開始", "成功", "")

            strKEY1 = txtNendo.Text.PadLeft(4, "0")
            strKEY2 = txtNendo.Text.PadLeft(4, "0") & txtTuki.Text.PadLeft(2, "0") & txtHi.Text.PadLeft(2, "0")
            Cursor.Current = Cursors.WaitCursor()

            '------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------
            If fn_check_text(True) = False Then
                Exit Sub
            End If

            MainDB = New CASTCommon.MyOracle

            '----------------------------------------
            '休日マスタ(YASUMIMAST)に既に登録済みか検索
            '----------------------------------------
            If fn_KENSAKU_YASUMIMAST(strKEY2) = False Then
                MessageBox.Show(MSG0030W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '確認メッセージを表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            If fn_delete_YASUMIMAST() = True Then
                MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                If fn_select_YASUMIMAST(YasumiKen) = False Then
                    MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------START
                'Me.txtNendo.Text = ""
                'Me.txtTuki.Text = ""
                'Me.txtHi.Text = ""
                'Me.txtKyujitu_NM.Text = ""
                'Me.btnAction.Enabled = True
                'Me.btnDelete.Enabled = False
                'Me.btnKousin.Enabled = False
                '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------END
            Else
                MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            MainDB.Close()
            MainDB = Nothing

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(削除)", "失敗", ex.Message)

        Finally
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLog.Write("0000000000-00", "00000000", "(削除)終了", "成功", "")
        End Try
    End Sub

    '取消ボタン
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        Try
            '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------START
            'If MessageBox.Show(MSG0009I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            '    Exit Sub
            'End If
            '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------END
            MainLog.Write("0000000000-00", "00000000", "(取消)開始", "成功", "")
            If fn_clear_GAMEN() = False Then
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(取消)", "失敗", ex.Message)

        Finally
            MainLog.Write("0000000000-00", "00000000", "(取消)終了", "成功", "")

        End Try
    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Me.Close()
    End Sub

    '2017/05/17 タスク）西野 ADD 標準版修正（休日情報一覧表印刷機能追加）----------------- START
    ''' <summary>
    ''' 印刷ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(印刷)開始", "成功", "")


            '----------------------------------------
            '入力チェック
            '----------------------------------------
            If Me.txtNendo.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "基準年"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtNendo.Focus()
                Return
            End If

            '----------------------------------------
            '登録チェック
            '----------------------------------------
            Dim SQL As New StringBuilder
            Dim strNendo As String = txtNendo.Text.PadLeft(4, "0"c)
            With SQL
                .Append("select")
                .Append("     count(*) as COUNTER")
                .Append(" from")
                .Append("     YASUMIMAST")
                .Append(" WHERE ")
                .Append("     SUBSTR(YASUMI_DATE_Y,1,4) = " & SQ(strNendo))
            End With

            Me.MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)
            If OraReader.DataReader(SQL) = True Then
                If OraReader.GetInt("COUNTER") = 0 Then
                    '休日情報無し
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            Else
                '検索失敗
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            '----------------------------------------
            '印刷処理
            '----------------------------------------
            If MessageBox.Show(String.Format(MSG0013I, "休日情報一覧表"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If

            '終了ボタンを非活性化
            btnEnd.Enabled = False

            Dim ExeRepo As New CAstReports.ClsExecute
            Dim nRet As Integer
            Dim CmdArg As String
            ExeRepo.SetOwner = Me

            'パラメータ：ユーザID
            CmdArg = String.Format("{0},{1}", gstrUSER_ID, strNendo)
            nRet = ExeRepo.ExecReport("KFUP004.EXE", CmdArg)
            Select Case nRet
                Case 0
                    '印刷完了
                    MessageBox.Show(String.Format(MSG0014I, "休日情報一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象無し
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case Else
                    '印刷失敗
                    MessageBox.Show(String.Format(MSG0004E, "休日情報一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "印刷", "失敗", "戻り値:" & nRet)
            End Select

        Catch ex As Exception
            MainLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(印刷)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If

            '終了ボタンを活性化
            btnEnd.Enabled = True

            MainLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(印刷)終了", "成功", "")
        End Try
    End Sub
    '2017/05/17 タスク）西野 ADD 標準版修正（休日情報一覧表印刷機能追加）----------------- END

#End Region

#Region "リストボックス"

    'リストボックス選択時
    Private Sub KyujituList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KyujituList.SelectedIndexChanged
        Try
            '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------START
            If KyujituList.SelectedIndex >= 0 Then
                txtNendo.Text = strSYUKU_DATE(KyujituList.SelectedIndex).Substring(0, 4)
                txtTuki.Text = strSYUKU_DATE(KyujituList.SelectedIndex).Substring(4, 2)
                txtHi.Text = strSYUKU_DATE(KyujituList.SelectedIndex).Substring(6, 2)
                txtKyujitu_NM.Text = strSYUKU_NM(KyujituList.SelectedIndex).Trim()

                Application.DoEvents()
                If Me.txtNendo.Visible AndAlso Me.txtNendo.Enabled Then
                    Me.txtNendo.Focus()
                End If
            End If
            '2011/06/23 標準版修正 休日マスタメンテナンス処理見直し ------------------END
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(リストボックス選択)", "失敗", ex.Message)
        End Try
    End Sub

#End Region

#Region "関数"

    Private Function fn_clear_GAMEN() As Boolean
        Try
            fn_clear_GAMEN = False

            btnAction.Enabled = True
            btnKousin.Enabled = False
            btnDelete.Enabled = False
            btnSansyo.Enabled = True
            txtNendo.Enabled = True

            txtNendo.Text = ""
            txtTuki.Text = ""
            txtHi.Text = ""
            txtKyujitu_NM.Text = ""
            KyujituList.Items.Clear()

            txtNendo.Focus()

            If Err.Number = 0 Then
                fn_clear_GAMEN = True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(画面クリア関数)終了", "失敗", ex.Message)
        End Try
    End Function

    Private Function fn_insert_YASUMIMAST() As Boolean
        Try

            fn_insert_YASUMIMAST = False

            Dim SQL As New StringBuilder
            SQL.Append("INSERT INTO YASUMIMAST")
            SQL.Append(" VALUES ('" & strKEY2 & "','" & strKEY3 & "','")
            SQL.Append(strKEY4 & "','" & strKEY5 & "')")

            Try
                MainDB.ExecuteNonQuery(SQL)
                MainDB.Commit()
            Catch ex As Exception
                MainDB.Rollback()
                fn_insert_YASUMIMAST = False
                Exit Function
            End Try

            If Err.Number = 0 Then
                fn_insert_YASUMIMAST = True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(休日マスタ登録関数)終了", "失敗", ex.Message)
        End Try
    End Function

    Private Function fn_select_YASUMIMAST(ByRef RecordCount As Integer) As Boolean
        Dim intRECORD_COUNT As Integer
        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try

            fn_select_YASUMIMAST = False
            Dim SQL As New StringBuilder
            SQL.Append("SELECT * FROM YASUMIMAST ")
            SQL.Append(" WHERE SUBSTR(YASUMI_DATE_Y,1,4) = '" & strKEY1 & "'")
            SQL.Append(" ORDER BY YASUMI_DATE_Y ASC")

            '-----------------------------------------
            'レコードの件数取得
            '-----------------------------------------
            intRECORD_COUNT = 0
            'リストボックスクリア
            KyujituList.Items.Clear()
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    ReDim Preserve strSYUKU_NM(intRECORD_COUNT)
                    ReDim Preserve strSYUKU_DATE(intRECORD_COUNT)
                    strSYUKU_NM(intRECORD_COUNT) = oraReader.GetItem("YASUMI_NAME_Y")
                    strSYUKU_DATE(intRECORD_COUNT) = oraReader.GetItem("YASUMI_DATE_Y")
                    KyujituList.Items.Add(Space(5) & _
                                          strSYUKU_DATE(intRECORD_COUNT).Substring(4, 2) & "月" & _
                                          strSYUKU_DATE(intRECORD_COUNT).Substring(6, 2) & "日" & _
                                          Space(5) & oraReader.GetItem("YASUMI_NAME_Y"))
                    intRECORD_COUNT = intRECORD_COUNT + 1
                    oraReader.NextRead()
                End While
            End If
            RecordCount = intRECORD_COUNT

            If Err.Number = 0 Then
                fn_select_YASUMIMAST = True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(休日情報マスタ参照関数)終了", "失敗", ex.Message)
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    Private Function fn_update_YASUMIMAST() As Boolean

        Try
            MainLog.Write("0000000000-00", "00000000", "(休日マスタ更新関数)開始", "成功")

            fn_update_YASUMIMAST = False

            Dim SQL As New StringBuilder
            SQL.Append("UPDATE YASUMIMAST SET ")
            SQL.Append(" YASUMI_NAME_Y = '" & strKEY3 & "',")
            SQL.Append(" KOUSIN_DATE_Y = '" & strKEY5 & "'")
            SQL.Append(" WHERE YASUMI_DATE_Y = '" & strKEY2 & "'")

            Try
                MainDB.ExecuteNonQuery(SQL)
                MainDB.Commit()
            Catch ex As Exception
                MainDB.Rollback()
                fn_update_YASUMIMAST = False
                Exit Function
            End Try

            If Err.Number = 0 Then
                fn_update_YASUMIMAST = True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(休日マスタ更新関数)終了", "失敗", ex.Message)
        End Try
    End Function

    Private Function fn_delete_YASUMIMAST() As Boolean
        Try

            fn_delete_YASUMIMAST = False

            Dim SQL As New StringBuilder
            SQL.Append("DELETE YASUMIMAST ")
            SQL.Append(" WHERE SUBSTR(YASUMI_DATE_Y,1,4) = '" & strKEY1 & "'")
            If strKEY2.Trim.Length <> 0 Then
                SQL.Append(" AND YASUMI_DATE_Y = '" & strKEY2 & "'")
            End If

            Try
                MainDB.ExecuteNonQuery(SQL)
                MainDB.Commit()
            Catch ex As Exception
                MainDB.Rollback()
                fn_delete_YASUMIMAST = False
                Exit Function
            End Try

            If Err.Number = 0 Then
                fn_delete_YASUMIMAST = True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(休日マスタ削除関数)終了", "失敗", ex.Message)
        End Try
    End Function

    Private Function fn_KENSAKU_YASUMIMAST(ByVal astrYASUMI_DATE) As Boolean
        '============================================================================
        'NAME           :fn_KENSAKU_YASUMIMAST
        'Parameter      :astrYASUMI_DATE:祝日
        'Description    :astrYASUMI_DATEがYASUMIMASTに登録済みか検索
        'Return         :True=登録済み,False=未登録
        'Create         :2004/10/07
        'Update         :
        '============================================================================
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Try
            MainLog.Write("0000000000-00", "00000000", "(休日マスタ検索関数)開始", "成功")

            fn_KENSAKU_YASUMIMAST = False

            Dim SQL As New StringBuilder
            SQL.Append("SELECT * FROM YASUMIMAST ")
            SQL.Append(" WHERE YASUMI_DATE_Y = '" & astrYASUMI_DATE & "'")

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            fn_KENSAKU_YASUMIMAST = oraReader.DataReader(Sql)

            MainLog.Write("0000000000-00", "00000000", "(休日マスタ検索関数)終了", "成功")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(休日マスタ検索関数)終了", "失敗", ex.Message)
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function
    Function fn_check_text(Optional ByVal DEL_FLG As Boolean = False) As Boolean
        ''============================================================================
        'NAME           :fn_check_text
        'Parameter      :DEL_FLG － 削除判定
        'Description    :必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/10/07
        'Update         :2008/06/17
        '============================================================================
        Try

            fn_check_text = False

            If Me.txtNendo.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtNendo.Focus()
                Return False
            End If

            If Me.txtTuki.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTuki.Focus()
                Return False
            Else
                If CInt(txtTuki.Text) < 1 OrElse CInt(txtTuki.Text) > 12 Then
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTuki.Focus()
                    Return False
                End If
            End If

            If Me.txtHi.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtHi.Focus()
                Return False
            Else
                If CInt(txtHi.Text) < 1 OrElse CInt(txtHi.Text) > 31 Then
                    MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtHi.Focus()
                    Return False
                End If
            End If

            If txtNendo.Text.Length <> 0 And txtTuki.Text.Length <> 0 And txtHi.Text.Length <> 0 Then
                Dim KYUUJITU_DATE As String
                KYUUJITU_DATE = txtNendo.Text & "/" & txtTuki.Text & "/" & txtHi.Text
                If Not IsDate(KYUUJITU_DATE) Then
                    MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtNendo.Focus()
                    txtNendo.SelectionStart = 0
                    txtNendo.SelectionLength = txtNendo.Text.Length
                    Exit Function
                End If
            End If

            If DEL_FLG = False Then
                If (txtKyujitu_NM.Text.Trim).Length = 0 Then
                    MessageBox.Show(MSG0031W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKyujitu_NM.Focus()
                    Exit Function
                End If
            End If

            If Err.Number = 0 Then
                fn_check_text = True
            End If


        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(入力チェック関数)終了", "失敗", ex.Message)
        End Try

    End Function

    '基準日（月）LostFocus
    Private Sub txtTuki_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles txtTuki.LostFocus

        Try

            If FN_CHK_DATE("MONTH") = False Then
                Return
            End If

        Catch ex As Exception
            MainLog.Write("登録日(月)フォーカス失効", "失敗", ex.Message)
        End Try

    End Sub

    '基準日（日）LostFocus
    Private Sub txtHi_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles txtHi.LostFocus

        Try

            If FN_CHK_DATE("DAY") = False Then
                Return
            End If

        Catch ex As Exception
            MainLog.Write("登録日(日)フォーカス失効", "失敗", ex.Message)
        End Try

    End Sub

    Private Function FN_CHK_DATE(ByVal TextType As String) As Boolean

        Try
            Dim MSG As String
            Dim DRet As DialogResult

            Select Case TextType
                Case "MONTH"
                    If Me.txtTuki.Text <> "" Then

                        MSG = MSG0022W
                        Select Case CInt(Me.txtTuki.Text)
                            Case 0, Is >= 13
                                DRet = MessageBox.Show(MSG, msgTitle, _
                                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Me.txtTuki.Focus()
                                Return False
                        End Select
                    End If

                Case "DAY"
                    If Me.txtHi.Text <> "" Then

                        MSG = MSG0025W
                        Select Case CInt(Me.txtHi.Text)
                            Case 0, Is >= 32
                                DRet = MessageBox.Show(MSG, msgTitle, _
                                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Me.txtHi.Focus()
                                Return False
                        End Select
                    End If

            End Select

            Return True
        Catch ex As Exception
            MainLOG.Write("日付チェック", "失敗", ex.Message)

        End Try
    End Function


#End Region

#Region "テキストボックス"

    '基準年／登録月日
    Private Sub DateTextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtNendo.Validating, _
            txtTuki.Validating, _
            txtHi.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MainLog.Write("0000000000-00", "00000000", "(日付テキストボックス)終了", "失敗", ex.Message)
        End Try
    End Sub

    '休日名称
    Private Sub NameTextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtKyujitu_NM.Validating
        Try

            With CType(sender, TextBox)
                .Text = GCom.GetLimitString(.Text, 40)
            End With

        Catch ex As Exception
            MainLog.Write("0000000000-00", "00000000", "(休日名テキストボックス)終了", "失敗", ex.Message)
        End Try
    End Sub

#End Region

End Class
