Imports System.Text
Imports CASTCommon

Public Class KFJKOZA040

    '共通イベントコントロール
    Private CAST As New CASTCommon.Events

    '' 許可文字指定

    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MyOwnerForm As Form

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFJKOZA040", "自振契約リエンタ結果更新")
    Private Const msgTitle As String = "自振契約リエンタ結果更新(KFJKOZA040)"

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

        Dim MainDB As New CASTCommon.MyOracle
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim sql As New StringBuilder(128)

        Dim strSyoridate As String
        Dim strTimeStamp As String

        Try

            strSyoridate = txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text

            If check_input() = False Then Return

            sql.Append("SELECT TIME_STAMP_JR FROM JIKEIMAST WHERE SYORI_DATE_JR = '" & strSyoridate & "' GROUP BY TIME_STAMP_JR,KAIJI_JR ORDER BY KAIJI_JR")

            'コンボボックスの初期化
            cmbTimeStamp.Items.Clear()

            '再描画しないようにする
            cmbTimeStamp.BeginUpdate()

            If oraReader.DataReader(sql) = True Then
                Do Until oraReader.EOF
                    '------------------------------------------------
                    'コンボボックスにリストを追加する
                    '------------------------------------------------
                    strTimeStamp = GCom.NzStr(oraReader.GetString("TIME_STAMP_JR"))

                    strTimeStamp = strTimeStamp.Substring(0, 4) & "/" & strTimeStamp.Substring(4, 2) & "/" & strTimeStamp.Substring(6, 2) & Space(1) _
                                    & strTimeStamp.Substring(8, 2) & ":" & strTimeStamp.Substring(10, 2) & ":" & strTimeStamp.Substring(12, 2)

                    cmbTimeStamp.Items.Add(strTimeStamp)
                    oraReader.NextRead()
                Loop

                '実行ボタンを表示
                btnAction.Enabled = True

                btnSearch.Enabled = False
                txtSyoriDateD.Enabled = False
                txtSyoriDateM.Enabled = False
                txtSyoriDateY.Enabled = False

                cmbTimeStamp.SelectedIndex = 0
                cmbTimeStamp.Enabled = True
                cmbTimeStamp.Focus()

            Else
                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If


            '再描画するようにする
            cmbTimeStamp.EndUpdate()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)例外エラー", "失敗", ex.Message)
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()

        End Try

    End Sub


    Private Function check_input() As Boolean

        '年必須チェック
        If txtSyoriDateY.Text.Trim = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSyoriDateY.Focus()
            Return False
        End If
        '月必須チェック
        If txtSyoriDateM.Text.Trim = "" Then
            MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSyoriDateM.Focus()
            Return False
        End If
        '月範囲チェック
        If GCom.NzInt(txtSyoriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtSyoriDateM.Text.Trim) > 12 Then
            MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSyoriDateM.Focus()
            Return False
        End If

        '日付必須チェック
        If txtSyoriDateD.Text.Trim = "" Then
            MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSyoriDateD.Focus()
            Return False
        End If
        '日付範囲チェック
        If GCom.NzInt(txtSyoriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtSyoriDateD.Text.Trim) > 31 Then
            MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSyoriDateD.Focus()
            Return False
        End If
        '日付整合性チェック
        Dim WORK_DATE As String = txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text
        If Information.IsDate(WORK_DATE) = False Then
            MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSyoriDateY.Focus()
            Return False
        End If

        Return True

    End Function


    Private Sub KFJKOZA040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            btnAction.Enabled = False
            cmbTimeStamp.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Dim MainDB As New CASTCommon.MyOracle

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")
            '------------------------------------------------
            'タイムスタンプの取得
            '------------------------------------------------
            Dim strTIME_STAMP_W As String
            strTIME_STAMP_W = cmbTimeStamp.SelectedItem
            strTIME_STAMP_W = strTIME_STAMP_W.Substring(0, 4) & strTIME_STAMP_W.Substring(5, 2) & strTIME_STAMP_W.Substring(8, 2) _
                            & strTIME_STAMP_W.Substring(11, 2) & strTIME_STAMP_W.Substring(14, 2) & strTIME_STAMP_W.Substring(17, 2)

            If MessageBox.Show(MSG0023I.Replace("{0}", "自振契約リエンタ結果更新"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            MainDB.BeginTrans()     'トランザクション開始

            Dim jobid As String
            Dim para As String

            'ジョブマスタに登録
            jobid = "J100"
            para = strTIME_STAMP_W

            '#########################
            'job検索
            '#########################
            Dim iRet As Integer
            iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            If iRet = 1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf iRet = -1 Then
                Throw New Exception("(JOBMAST)検索処理失敗")
            End If

            '#########################
            'job登録
            '#########################
            If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then
                Throw New Exception("(JOBMAST)更新処理失敗。ROLLBACKします。")
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0021I.Replace("{0}", "自振契約リエンタ結果更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            btnSearch.Enabled = True
            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True
            cmbTimeStamp.Enabled = False
            cmbTimeStamp.Items.Clear()

            Return

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)例外エラー", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try

    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")

            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            btnSearch.Enabled = True
            btnAction.Enabled = False

            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True
            cmbTimeStamp.Enabled = False

            cmbTimeStamp.Items.Clear()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub

    Private Sub FURI_DATE_S_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtSyoriDateY.Validating, txtSyoriDateM.Validating, txtSyoriDateD.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力後処置)例外エラー", "失敗", ex.Message)
        End Try


    End Sub
End Class
