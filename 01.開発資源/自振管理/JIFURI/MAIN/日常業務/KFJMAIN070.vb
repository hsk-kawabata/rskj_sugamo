Option Strict On
Option Explicit On

Imports CASTCommon

Public Class KFJMAIN070
    'ログ初期化
    Private BLOG As New CASTCommon.BatchLOG("KFJMAIN070", "手数料計算画面")
    Private Const msgTitle As String = "手数料計算画面(KFJMAIN070)"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Dim strTORI_CODE As String
    Dim strFURI_DATE As String

#Region "テキストボックスの動き"

    '振替年
    Private Sub txtFuriDateY_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtFuriDateY.Validating
        Dim TxtObj As TextBox = CType(sender, TextBox)
        If TxtObj.Text.Trim.Length > 0 Then
            TxtObj.Text = TxtObj.Text.Trim.PadLeft(4, "0"c)
        End If
    End Sub
    '振替月
    Private Sub txtFuriDateM_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtFuriDateM.Validating
        Dim TxtObj As TextBox = CType(sender, TextBox)
        If TxtObj.Text.Trim.Length > 0 Then
            TxtObj.Text = TxtObj.Text.Trim.PadLeft(2, "0"c)
            'If CInt(TxtObj.Text) < 0 Or CInt(TxtObj.Text) > 12 Then
            '    MessageBox.Show("1から12の範囲で入力してください", _
            '                    gstrSYORI_R, _
            '                    MessageBoxButtons.OK, _
            '                    MessageBoxIcon.Warning)
            '    e.Cancel = True
            'End If
        End If
    End Sub

    '振替日
    Private Sub txtFuriDateD_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtFuriDateD.Validating
        Dim TxtObj As TextBox = CType(sender, TextBox)
        If TxtObj.Text.Trim.Length > 0 Then
            TxtObj.Text = TxtObj.Text.Trim.PadLeft(2, "0"c)
            'If CInt(TxtObj.Text) < 0 Or CInt(TxtObj.Text) > 31 Then
            '    MessageBox.Show("1から31の範囲で入力してください", _
            '                    gstrSYORI_R, _
            '                    MessageBoxButtons.OK, _
            '                    MessageBoxIcon.Warning)
            '    e.Cancel = True
            'End If
        End If
    End Sub
#End Region
#Region "画面のＬＯＡＤ"
    Private Sub KFJMAIN070_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Visible = False
        '#####################################
        'ログの書込に必要な情報の取得
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '休日マスタ取り込み
            '2013/03/19 saitou 標準修正 UPD -------------------------------------------------->>>>
            '共通関数一元化
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)終了", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時に振替日に前営業日を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strSysDate As String
            Dim strGetdate As String = ""

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            bRet = GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)

            txtFuriDateY.Text = strGetdate.Substring(0, 4)
            txtFuriDateM.Text = strGetdate.Substring(4, 2)
            txtFuriDateD.Text = strGetdate.Substring(6, 2)

            'If fn_select_YASUMIMAST() = False Then
            '    BLOG.Write(gstrUSER_ID, "", "", "休日マスタの取得", "失敗", Err.Description)

            '    MessageBox.Show(String.Format(MSG0027E, "休日マスタ", "取得"), _
            '                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If

            ''=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            ''画面表示時に振替日に前営業日を表示する
            ''=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'Dim strDATE_Y As String, strDATE_M As String, strDATE_D As String
            'Dim strOUT_Y As String, strOUT_M As String, strOUT_D As String
            'Dim strKYU_CD As Integer

            'strDATE_Y = Format(System.DateTime.Today, "yyyy")
            'strDATE_M = Format(System.DateTime.Today, "MM")
            'strDATE_D = Format(System.DateTime.Today, "dd")
            'strOUT_Y = ""
            'strOUT_M = ""
            'strOUT_D = ""
            'strKYU_CD = 1

            'If fn_set_EIGYOBI(strDATE_Y, strDATE_M, strDATE_D, strKYU_CD, gintSYORI_KBN.BEFORE, strOUT_Y, strOUT_M, strOUT_D) = False Then
            '    Exit Sub
            'End If
            'txtFuriDateY.Text = strOUT_Y
            'txtFuriDateM.Text = strOUT_M
            'txtFuriDateD.Text = strOUT_D
            '2013/03/19 saitou 標準修正 UPD --------------------------------------------------<<<<

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
            txtFuriDateY.Focus()
        End Try
    End Sub
#End Region
#Region "画面のShown"
    Private Sub KFJMAIN070_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        txtFuriDateY.Focus()
    End Sub
#End Region
#Region "ボタン"
    '終了ボタン
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

    '計算ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '============================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :計算ボタンを押下時の処理
        'Return         :
        'Create         :2004/08/25
        'Update         :
        '============================================================================
        Try
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(計算)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面入力値のチェック
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_check_text() = False Then
                Exit Sub
            End If

            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            LW.FuriDate = strFURI_DATE

            If MessageBox.Show(String.Format(MSG0023I, "手数料計算処理"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Exit Sub
            End If

            ' ジョブマスタ パラメータ設定 振替日、手数料計算フラグ 0:初期 1:再計算
            '2013/10/24 saitou 標準修正 ADD -------------------------------------------------->>>>
            Dim jobid As String = String.Empty
            Dim para As String = String.Empty
            '2013/10/24 saitou 標準修正 ADD --------------------------------------------------<<<<
            para = strFURI_DATE & ",0"

            jobid = "J070"

            ' ジョブ監視マスタ　登録チェック
            If clsFUSION.fn_JOBMAST_TOUROKU_CHECK(jobid, gstrUSER_ID, para) = False Then
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' ジョブ監視マスタに登録する
            If clsFUSION.fn_INSERT_JOBMAST(jobid, gstrUSER_ID, para) = False Then
                BLOG.Write(gstrUSER_ID, "", strFURI_DATE, "起動パラメータ登録", "失敗", "起動パラメータ登録に失敗しました")

                MessageBox.Show(String.Format(MSG0027E, "起動パラメータ", "登録"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                BLOG.Write(gstrUSER_ID, "", strFURI_DATE, "起動パラメータ登録", "成功", "起動パラメータを登録しました")

                MessageBox.Show(String.Format(MSG0021I, "手数料計算処理"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(計算)終了", "成功", "")
            LW.FuriDate = "00000000"
        End Try

    End Sub

    '再計算ボタン
    Private Sub btnReAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReAction.Click
        '============================================================================
        'NAME           :btnReAction_Click
        'Parameter      :
        'Description    :再計算ボタンを押下時の処理
        'Return         :
        'Create         :2004/08/26
        'Update         :
        '============================================================================
        Try
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(再計算)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面入力値のチェック
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_check_text() = False Then
                Exit Sub
            End If

            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            LW.FuriDate = strFURI_DATE
            ''SCHMASTに済・不能の件数金額再更新　2007/09/19
            'fn_SCHMAST_KOUSIN()


            If MessageBox.Show(String.Format(MSG0023I, "手数料計算(再計算)処理"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Exit Sub
            End If

            ' ジョブマスタ パラメータ設定
            '2013/10/24 saitou 標準修正 ADD -------------------------------------------------->>>>
            Dim jobid As String = String.Empty
            Dim para As String = String.Empty
            '2013/10/24 saitou 標準修正 ADD --------------------------------------------------<<<<
            para = strFURI_DATE & ",1"

            jobid = "J070"

            ' ジョブ監視マスタ　登録チェック
            If clsFUSION.fn_JOBMAST_TOUROKU_CHECK(jobid, gstrUSER_ID, para) = False Then
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            ' ジョブ監視マスタに登録する
            If clsFUSION.fn_INSERT_JOBMAST(jobid, gstrUSER_ID, para) = False Then
                BLOG.Write(gstrUSER_ID, "", strFURI_DATE, "起動パラメータ登録", "失敗", "起動パラメータ登録に失敗しました")

                MessageBox.Show(String.Format(MSG0027E, "起動パラメータ", "登録"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                BLOG.Write(gstrUSER_ID, "", strFURI_DATE, "起動パラメータ登録", "成功", "起動パラメータを登録しました")

                MessageBox.Show(String.Format(MSG0021I, "手数料計算処理"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            BLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(再計算)終了", "成功", "")
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region
#Region "関数"
    '入力チェック
    Public Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :実行ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/08/25
        'Update         :
        '============================================================================
        fn_check_text = False
        If fn_CHECK_NUM_MSG(txtFuriDateY.Text, "振替年", msgTitle) = False Then
            txtFuriDateY.Focus()
            Exit Function
        End If
        If fn_CHECK_NUM_MSG(txtFuriDateM.Text, "振替月", msgTitle) = False Then
            txtFuriDateM.Focus()
            Exit Function
        Else
            If CInt(txtFuriDateM.Text) < 1 Or CInt(txtFuriDateM.Text) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Exit Function
            End If
        End If
        If fn_CHECK_NUM_MSG(txtFuriDateD.Text, "振替日", msgTitle) = False Then
            txtFuriDateD.Focus()
            Exit Function
        Else
            If CInt(txtFuriDateD.Text) < 1 Or CInt(txtFuriDateD.Text) > 31 Then

                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Exit Function
            End If
        End If
        '日付整合性チェック
        Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
        If Information.IsDate(WORK_DATE) = False Then
            MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY.Focus()
            Return False
        End If

        fn_check_text = True
    End Function

    Function fn_CHECK_NUM_MSG(ByVal objOBJ As String, ByVal strJNAME As String, ByVal gstrTITLE As String) As Boolean
        '============================================================================
        'NAME           :fn_CHECK_NUM_MSG
        'Parameter      :objOBJ：チェック対象オブジェクト／strJNAME：オブジェクト名称
        '               :gstrTITLE：タイトル
        'Description    :数値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/05/28
        'Update         :
        '============================================================================
        fn_CHECK_NUM_MSG = False
        If Trim(objOBJ).Length = 0 Then

            MessageBox.Show(String.Format(MSG0285W, strJNAME), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            fn_CHECK_NUM_MSG = False
            Exit Function
        End If
        fn_CHECK_NUM_MSG = True
    End Function
#End Region
End Class