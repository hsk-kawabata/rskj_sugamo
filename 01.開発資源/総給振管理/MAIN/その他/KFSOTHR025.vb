Imports CASTCommon
Imports System.Text

''' <summary>
''' 振込発信データ作成取消画面クラス
''' </summary>
''' <remarks>金バッチ対応</remarks>
Public Class KFSOTHR025

#Region "クラス定数"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFSOTHR025", "振込発信データ作成取消")
    Private Const msgtitle As String = "振込発信データ作成取消画面(KFSOTHR025)"

#End Region

#Region "クラス変数"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure
    Private LW As LogWrite

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFSOTHR025_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '------------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Me.Label1, Me.Label2, Me.lblUser, Me.lblDate)

            '------------------------------------------------
            '画面表示時、処理日にシステム日付を表示
            '------------------------------------------------
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            Me.txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            Me.txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            Me.txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            '------------------------------------------------
            '実行ボタンを非表示設定
            '------------------------------------------------
            Me.btnAction.Enabled = False
            Me.cmbTimeStamp.Enabled = False
            Me.btnReset.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    ''' <summary>
    ''' 検索ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)開始", "成功", "")

            '------------------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------------------
            If fn_check_TEXT() = False Then Exit Sub

            '------------------------------------------------
            'スケジュールマスタ検索処理
            '------------------------------------------------
            If fn_select_SCHMAST(0) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 実行ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim lngSCH_CNT As Long
        Dim lngKAKUHO_CNT As Long
        Dim MainDB As CASTCommon.MyOracle = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")
            '------------------------------------------------
            'タイムスタンプの取得
            '------------------------------------------------
            Dim strTIME_STAMP_W As String
            strTIME_STAMP_W = Me.cmbTimeStamp.SelectedItem.ToString
            strTIME_STAMP_W = strTIME_STAMP_W.Substring(0, 4) & strTIME_STAMP_W.Substring(5, 2) & strTIME_STAMP_W.Substring(8, 2) _
                            & strTIME_STAMP_W.Substring(11, 2) & strTIME_STAMP_W.Substring(14, 2) & strTIME_STAMP_W.Substring(17, 2)

            If MessageBox.Show(MSG0015I.Replace("{0}", "振込発信データ作成取消"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            MainDB = New MyOracle
            MainDB.BeginTrans()     'トランザクション開始

            '------------------------------------------------
            'スケジュールマスタ更新処理
            '------------------------------------------------
            If fn_update_SCHMAST(strTIME_STAMP_W, lngSCH_CNT, MainDB) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            Else
                '更新件数がゼロ件の場合、エラーメッセージ出力
                If lngSCH_CNT = 0 Then
                    MessageBox.Show(MSG0054W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainDB.Rollback()
                    Return
                End If
            End If

            '------------------------------------------------
            '総給振依頼人連携データテーブル削除処理
            '------------------------------------------------
            If fn_delete_S_I_RENKEIMAST(strTIME_STAMP_W, lngKAKUHO_CNT, MainDB) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            '------------------------------------------------
            '総給振明細連携データテーブル削除処理
            '------------------------------------------------
            If fn_delete_S_M_RENKEIMAST(strTIME_STAMP_W, lngKAKUHO_CNT, MainDB) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0016I.Replace("{0}", "振込発信データ作成取消"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '------------------------------------------------
            '画面再表示処理
            '------------------------------------------------
            If fn_select_SCHMAST(1) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Me.cmbTimeStamp.Enabled = False
            Me.btnSearch.Enabled = True
            Me.txtSyoriDateD.Enabled = True
            Me.txtSyoriDateM.Enabled = True
            Me.txtSyoriDateY.Enabled = True
            Me.btnReset.Enabled = False
            Me.txtSyoriDateY.Focus()
            Return

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try

    End Sub

    ''' <summary>
    ''' 取消ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")

            Me.btnSearch.Enabled = True
            Me.btnAction.Enabled = False
            Me.cmbTimeStamp.Enabled = False

            Me.txtSyoriDateD.Enabled = True
            Me.txtSyoriDateM.Enabled = True
            Me.txtSyoriDateY.Enabled = True

            Me.txtSyoriDateY.Focus()

            Me.cmbTimeStamp.Items.Clear()
            Me.btnReset.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 終了ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' テキストボックスゼロ埋めイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtSyoriDateY.Validating, txtSyoriDateM.Validating, txtSyoriDateD.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' テキストボックスの入力チェックを行います。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_check_TEXT() As Boolean
        Try
            '------------------------------------------------
            '処理年チェック
            '------------------------------------------------
            '必須チェック
            If Me.txtSyoriDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '処理月チェック
            '------------------------------------------------
            '必須チェック
            If Me.txtSyoriDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateM.Focus()
                Return False
            End If

            '範囲チェック
            If Not (CInt(Me.txtSyoriDateM.Text) >= 1 And CInt(Me.txtSyoriDateM.Text) <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '処理日チェック
            '------------------------------------------------
            '必須チェック
            If Me.txtSyoriDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateD.Focus()
                Return False
            End If

            '範囲チェック
            If Not (CInt(Me.txtSyoriDateD.Text) >= 1 And CInt(Me.txtSyoriDateD.Text) <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = Me.txtSyoriDateY.Text & "/" & Me.txtSyoriDateM.Text & "/" & Me.txtSyoriDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyoriDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' スケジュールマスタの検索を行います。
    ''' </summary>
    ''' <param name="aintFLG"></param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_select_SCHMAST(ByVal aintFLG As Integer) As Boolean
        Dim strSyoriDate As String
        Dim strJOKEN1 As String '抽出条件1
        Dim strJOKEN2 As String '抽出条件2
        Dim strTIME_STAMP As String

        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As MyOracleReader = Nothing
        Dim OraRenReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)

        Try
            MainDB = New MyOracle
            OraReader = New MyOracleReader(MainDB)
            OraRenReader = New MyOracleReader(MainDB)

            '処理日の取得
            strSyoriDate = Me.txtSyoriDateY.Text & Me.txtSyoriDateM.Text & Me.txtSyoriDateD.Text
            strJOKEN1 = strSyoriDate & "000000"
            strJOKEN2 = strSyoriDate & "999999"

            '------------------------------------------------
            'スケジュールマスタの検索処理
            '------------------------------------------------
            With SQL
                .Length = 0
                .Append("SELECT DISTINCT HASSIN_TIME_STAMP_S")
                .Append(" FROM S_SCHMAST, S_I_RENKEIMAST")
                .Append(" WHERE HASSIN_TIME_STAMP_S BETWEEN " & SQ(strJOKEN1) & " AND " & SQ(strJOKEN2))
                .Append(" AND HASSIN_FLG_S = '1'")
                .Append(" AND TYUUDAN_FLG_S = '0'")
                .Append(" AND HASSIN_TIME_STAMP_S = SAKUSEI_DATE_SR || SAKUSEI_TIME_SR")
                .Append(" ORDER BY HASSIN_TIME_STAMP_S")
            End With

            'コンボボックスの初期化
            Me.cmbTimeStamp.Items.Clear()

            '再描画しないようにする
            Me.cmbTimeStamp.BeginUpdate()

            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF

                    '------------------------------------------------
                    '総給振依頼人データテーブルの検索
                    '------------------------------------------------
                    With SQL
                        .Length = 0
                        .Append("SELECT KIN_STS_SR FROM S_I_RENKEIMAST")
                        .Append(" WHERE SAKUSEI_DATE_SR || SAKUSEI_TIME_SR = " & SQ(OraReader.GetString("HASSIN_TIME_STAMP_S")))
                        .Append(" ORDER BY KIN_STS_SR DESC")
                    End With

                    Dim bAddFlg As Boolean = False
                    If OraRenReader.DataReader(SQL) = True Then
                        While OraRenReader.EOF = False
                            '要検討　金バッチ側取り消し条件
                            '降順にしたので、完了があれば取り消し不可
                            If OraRenReader.GetInt("KIN_STS_SR") = 9 Then
                                bAddFlg = False
                                Exit While
                            ElseIf OraRenReader.GetInt("KIN_STS_SR") = 1 Then
                                Dim strMsg As String = "金バッチ取込済です。続行しますか？" & vbCrLf
                                strMsg &= "作成タイムスタンプ：" & OraReader.GetString("HASSIN_TIME_STAMP_S")
                                If MessageBox.Show(strMsg, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                                    bAddFlg = False
                                    Exit While
                                Else
                                    MessageBox.Show("取消後は必ず金バッチ側でも取消処理を実行してください。", msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                    bAddFlg = True
                                    Exit While
                                End If
                            Else
                                bAddFlg = True
                                Exit While
                            End If

                            OraRenReader.NextRead()
                        End While
                    End If

                    OraRenReader.Close()

                    If bAddFlg = True Then
                        '------------------------------------------------
                        'コンボボックスにリストを追加する
                        '------------------------------------------------
                        strTIME_STAMP = GCom.NzStr(OraReader.GetString("HASSIN_TIME_STAMP_S"))
                        strTIME_STAMP = strTIME_STAMP.Substring(0, 4) & "/" & strTIME_STAMP.Substring(4, 2) & "/" & strTIME_STAMP.Substring(6, 2) & Space(1) _
                                        & strTIME_STAMP.Substring(8, 2) & ":" & strTIME_STAMP.Substring(10, 2) & ":" & strTIME_STAMP.Substring(12, 2)

                        Me.cmbTimeStamp.Items.Add(strTIME_STAMP)

                    End If

                    OraReader.NextRead()
                Loop
            End If

            '再描画するようにする
            Me.cmbTimeStamp.EndUpdate()

            '該当件数がゼロ件の場合
            If Me.cmbTimeStamp.Items.Count = 0 Then
                '検索ボタン押下時のみメッセージ出力
                If aintFLG = 0 Then
                    MessageBox.Show(MSG0054W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                '実行ボタンを非表示
                Me.btnAction.Enabled = False

                Me.txtSyoriDateY.Focus()
            Else
                '実行ボタンを表示
                Me.btnAction.Enabled = True
                Me.cmbTimeStamp.Enabled = True
                Me.btnSearch.Enabled = False
                Me.txtSyoriDateD.Enabled = False
                Me.txtSyoriDateM.Enabled = False
                Me.txtSyoriDateY.Enabled = False
                Me.btnReset.Enabled = True
                Me.cmbTimeStamp.SelectedIndex = 0
                Me.cmbTimeStamp.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール検索)", "失敗", ex.ToString)

            '再描画するようにする
            Me.cmbTimeStamp.EndUpdate()

            '実行ボタンを非表示
            Me.btnAction.Enabled = False
            Me.txtSyoriDateY.Focus()

            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

        Return True
    End Function

    ''' <summary>
    ''' スケジュールマスタの更新を行います。
    ''' </summary>
    ''' <param name="strTIME_STAMP_W">タイムスタンプ</param>
    ''' <param name="lngSCH_CNT">更新件数(参照渡し)</param>
    ''' <param name="rDB">オラクル</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_update_SCHMAST(ByVal strTIME_STAMP_W As String, ByRef lngSCH_CNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
        Dim SQL As StringBuilder = New StringBuilder(128)

        Try

            lngSCH_CNT = 0

            '------------------------------------------------
            'スケジュールマスタ更新
            '------------------------------------------------
            With SQL
                .Append("UPDATE S_SCHMAST SET ")
                .Append(" KAKUHO_FLG_S = '0'")
                .Append(",KAKUHO_DATE_S = '00000000'")
                .Append(",KAKUHO_TIME_STAMP_S = '00000000000000'")
                .Append(",HASSIN_FLG_S = '0'")
                .Append(",HASSIN_DATE_S = '00000000'")
                .Append(",HASSIN_TIME_STAMP_S = '00000000000000'")
                .Append(",TESUUTYO_FLG_S = '0'")
                .Append(",TESUU_DATE_S = '00000000'")
                .Append(",TESUU_TIME_STAMP_S = '00000000000000'")
                .Append(" WHERE HASSIN_TIME_STAMP_S = " & SQ(strTIME_STAMP_W))
                .Append(" AND HASSIN_FLG_S = '1'")
                .Append(" AND TYUUDAN_FLG_S = '0'")
                .Append(" AND EXISTS (")
                .Append("   SELECT * FROM S_I_RENKEIMAST ")
                .Append("   WHERE HASSIN_TIME_STAMP_S = SAKUSEI_DATE_SR || SAKUSEI_TIME_SR)")
            End With

            Dim nRet As Integer = rDB.ExecuteNonQuery(SQL)
            If nRet >= 0 Then
                lngSCH_CNT = nRet
            ElseIf nRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール更新)", "失敗", "予期せぬエラー")
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール更新)", "失敗", ex.ToString)
            Return False
        Finally
        End Try

    End Function

    ''' <summary>
    ''' 総給振依頼人連携データテーブルを削除します。
    ''' </summary>
    ''' <param name="strTIME_STAMP_W"></param>
    ''' <param name="lngCNT"></param>
    ''' <param name="rDB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function fn_delete_S_I_RENKEIMAST(ByVal strTIME_STAMP_W As String, ByRef lngCNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
        Dim SQL As New StringBuilder
        lngCNT = 0

        Try
            '------------------------------------------------
            '総給振依頼人連携データテーブル削除
            '------------------------------------------------
            With SQL
                .Append("DELETE FROM S_I_RENKEIMAST")
                .Append(" WHERE SAKUSEI_DATE_SR || SAKUSEI_TIME_SR = " & SQ(strTIME_STAMP_W))
            End With

            Dim nRet As Integer = rDB.ExecuteNonQuery(SQL)
            If nRet = 0 Then
                lngCNT = nRet
            ElseIf nRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(総給振依頼人連携データテーブル削除)", "失敗", "予期せぬエラー")
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(総給振依頼人連携データテーブル削除)", "失敗", ex.ToString)
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' 総給振明細連携データテーブルを削除します。
    ''' </summary>
    ''' <param name="strTIME_STAMP_W"></param>
    ''' <param name="lngCNT"></param>
    ''' <param name="rDB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function fn_delete_S_M_RENKEIMAST(ByVal strTIME_STAMP_W As String, ByRef lngCNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
        Dim SQL As New StringBuilder
        lngCNT = 0

        Try
            '------------------------------------------------
            '総給振明細連携データテーブル削除
            '------------------------------------------------
            With SQL
                .Append("DELETE FROM S_M_RENKEIMAST")
                .Append(" WHERE SAKUSEI_DATE_SR || SAKUSEI_TIME_SR = " & SQ(strTIME_STAMP_W))
            End With

            Dim nRet As Integer = rDB.ExecuteNonQuery(SQL)
            If nRet = 0 Then
                lngCNT = nRet
            ElseIf nRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(総給振明細連携データテーブル削除)", "失敗", "予期せぬエラー")
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(総給振明細連携データテーブル削除)", "失敗", ex.ToString)
            Return False
        End Try

        Return True
    End Function

#End Region

End Class
