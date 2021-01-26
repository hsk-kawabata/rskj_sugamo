Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKMAIN060

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKMAIN060", "資金決済データ作成取消画面")
    Private Const msgtitle As String = "資金決済データ作成取消画面(KFKMAIN060)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private Jikinko As String           '自金庫コード

    '画面LOAD時
    Private Sub KFKMAIN060_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '------------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------------
            '画面表示時、処理日にシステム日付を表示
            '------------------------------------------------
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            '------------------------------------
            'INIファイルの読み込み
            '------------------------------------
            If fn_INI_Read() = False Then
                Return
            End If

            '------------------------------------------------
            '実行ボタンを非表示設定
            '------------------------------------------------
            btnAction.Enabled = False
            cmbTimeStamp.Enabled = False
            btnReset.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    '検索ボタン押下時
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)終了", "成功", "")
        End Try

    End Sub

    '実行ボタン押下時
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim lngSCH_KES_CNT As Long
        Dim lngSCH_TES_CNT As Long
        Dim lngKES_CNT As Long

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

            If MessageBox.Show(String.Format(MSG0015I, "資金決済データ作成取消"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            MainDB.BeginTrans()     'トランザクション開始

            '------------------------------------------------
            'スケジュールマスタ更新処理
            '------------------------------------------------
            If Me.fn_update_SCHMAST_KinBatch(strTIME_STAMP_W, lngSCH_KES_CNT, lngSCH_TES_CNT, MainDB) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            Else
                '更新件数がゼロ件の場合、エラーメッセージ出力
                If lngSCH_KES_CNT + lngSCH_TES_CNT = 0 Then
                    MessageBox.Show(MSG0054W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainDB.Rollback()
                    Return
                End If
            End If

            '------------------------------------------------
            '資金決済連携データテーブル削除処理
            '------------------------------------------------
            If Me.fn_delete_K_I_RENKEIMAST(strTIME_STAMP_W, lngKES_CNT, MainDB) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            MainDB.Commit()

            MessageBox.Show(String.Format(MSG0016I, "資金決済データ作成取消"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '------------------------------------------------
            '画面再表示処理
            '------------------------------------------------
            If fn_select_SCHMAST(1) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            cmbTimeStamp.Enabled = False
            btnSearch.Enabled = True
            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True
            btnReset.Enabled = False
            txtSyoriDateY.Focus()
            Return

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try

    End Sub

    '終了ボタン押下時
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")

            btnSearch.Enabled = True
            btnAction.Enabled = False
            cmbTimeStamp.Enabled = False

            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True

            txtSyoriDateY.Focus()

            cmbTimeStamp.Items.Clear()
            btnReset.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub

#Region "KFKMAIN060用関数"

    Private Function fn_check_TEXT() As Boolean
        '============================================================================
        'NAME           :fn_check_TEXT
        'Parameter      :
        'Description    :検索ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/09/27
        'Update         :
        '============================================================================
        Try

            '------------------------------------------------
            '処理年チェック
            '------------------------------------------------
            '必須チェック
            If txtSyoriDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '処理月チェック
            '------------------------------------------------
            '必須チェック
            If txtSyoriDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtSyoriDateM.Text >= 1 And txtSyoriDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '処理日チェック
            '------------------------------------------------
            '必須チェック
            If txtSyoriDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtSyoriDateD.Text >= 1 And txtSyoriDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.Message)
            Return False

        Finally
        End Try

        Return True
    End Function

    Private Function fn_select_SCHMAST(ByVal aintFLG As Integer) As Boolean
        '============================================================================
        'NAME           :fn_select_SCHMAST
        'Parameter      :aintFLG=0:検索ボタン押下時　1:画面再表示時
        'Description    :SCHMASTから決済タイムスタンプ取得
        'Return         :True=OK,False=NG
        'Create         :2004/09/27
        'Update         :
        '============================================================================
        Dim strSyoriDate As String
        Dim strJOKEN1 As String '抽出条件1
        Dim strJOKEN2 As String '抽出条件2
        Dim strTIME_STAMP As String

        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try

            '処理日の取得
            strSyoriDate = txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text
            strJOKEN1 = strSyoriDate & "000000"
            strJOKEN2 = strSyoriDate & "999999"

            '------------------------------------------------
            'スケジュールマスタの検索処理
            '------------------------------------------------
            With SQL
                .Length = 0
                .Append("SELECT TIME_STAMP FROM (")
                .Append(" (SELECT KESSAI_TIME_STAMP_S AS TIME_STAMP")
                .Append("  FROM SCHMAST, K_I_RENKEIMAST")
                .Append("  WHERE KESSAI_TIME_STAMP_S = SAKUSEI_DATE_KR || SAKUSEI_TIME_KR")
                .Append("  AND KESSAI_TIME_STAMP_S BETWEEN " & SQ(strJOKEN1) & " AND " & SQ(strJOKEN2))
                .Append("  AND KESSAI_FLG_S = '1')")
                .Append(" UNION")
                .Append(" (SELECT TESUU_TIME_STAMP_S AS TIME_STAMP")
                .Append("  FROM SCHMAST, K_I_RENKEIMAST")
                .Append("  WHERE TESUU_TIME_STAMP_S = SAKUSEI_DATE_KR || SAKUSEI_TIME_KR")
                .Append("  AND TESUU_TIME_STAMP_S BETWEEN " & SQ(strJOKEN1) & " AND " & SQ(strJOKEN2))
                .Append("  AND TESUUTYO_FLG_S = '1')")
                .Append(" )")
                .Append(" GROUP BY TIME_STAMP")
                .Append(" ORDER BY TIME_STAMP")
            End With

            'コンボボックスの初期化
            cmbTimeStamp.Items.Clear()

            '再描画しないようにする
            cmbTimeStamp.BeginUpdate()

            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF
                    '------------------------------------------------
                    'コンボボックスにリストを追加する
                    '------------------------------------------------
                    strTIME_STAMP = GCom.NzStr(OraReader.GetString("TIME_STAMP"))
                    strTIME_STAMP = strTIME_STAMP.Substring(0, 4) & "/" & strTIME_STAMP.Substring(4, 2) & "/" & strTIME_STAMP.Substring(6, 2) & Space(1) _
                                    & strTIME_STAMP.Substring(8, 2) & ":" & strTIME_STAMP.Substring(10, 2) & ":" & strTIME_STAMP.Substring(12, 2)

                    cmbTimeStamp.Items.Add(strTIME_STAMP)
                    OraReader.NextRead()
                Loop
            End If

            '再描画するようにする
            cmbTimeStamp.EndUpdate()

            '該当件数がゼロ件の場合
            If cmbTimeStamp.Items.Count = 0 Then
                '検索ボタン押下時のみメッセージ出力
                If aintFLG = 0 Then
                    MessageBox.Show(MSG0054W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                '実行ボタンを非表示
                btnAction.Enabled = False

                txtSyoriDateY.Focus()
            Else
                '実行ボタンを表示
                btnAction.Enabled = True
                cmbTimeStamp.Enabled = True
                btnSearch.Enabled = False
                txtSyoriDateD.Enabled = False
                txtSyoriDateM.Enabled = False
                txtSyoriDateY.Enabled = False
                btnReset.Enabled = True
                cmbTimeStamp.SelectedIndex = 0
                cmbTimeStamp.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール検索)", "失敗", ex.Message)

            '再描画するようにする
            cmbTimeStamp.EndUpdate()

            '実行ボタンを非表示
            btnAction.Enabled = False
            txtSyoriDateY.Focus()

            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

        Return True
    End Function

    Public Function fn_INI_Read() As Boolean
        '============================================================================
        'NAME           :fn_INI_Read
        'Parameter      :
        'Description    :FSKJ.INIファイルの読み込み
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        fn_INI_Read = False
        '自金庫
        Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
        If Jikinko.ToUpper = "ERR" Or Jikinko = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), _
                            msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Return False
        End If
        fn_INI_Read = True
    End Function

    ''' <summary>
    ''' スケジュールマスタを更新します（金バッチ）
    ''' </summary>
    ''' <param name="strTIME_STAMP_W">タイムスタンプ</param>
    ''' <param name="lngSCH_KES_CNT">決済更新レコード数</param>
    ''' <param name="lngSCH_TES_CNT">手数料更新レコード数</param>
    ''' <param name="rDB">オラクル</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/09/17 大垣信金 金バッチ対応</remarks>
    Private Function fn_update_SCHMAST_KinBatch(ByVal strTIME_STAMP_W As String, ByRef lngSCH_KES_CNT As Long, ByRef lngSCH_TES_CNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
        Dim SQL As New StringBuilder
        Dim nRet As Integer

        Try
            '------------------------------------------------
            'スケジュールマスタ更新[決済]
            '------------------------------------------------
            lngSCH_KES_CNT = 0
            With SQL
                .Length = 0
                .Append("UPDATE SCHMAST SET")
                .Append(" KESSAI_DATE_S = '00000000'")
                .Append(",KESSAI_TIME_STAMP_S = '00000000000000'")
                .Append(",KESSAI_FLG_S = '0'")
                .Append(" WHERE KESSAI_TIME_STAMP_S = " & SQ(strTIME_STAMP_W))
                .Append(" AND KESSAI_FLG_S = '1'")
                .Append(" AND EXISTS (")
                .Append("   SELECT * FROM K_I_RENKEIMAST")
                .Append("   WHERE KESSAI_TIME_STAMP_S = SAKUSEI_DATE_KR || SAKUSEI_TIME_KR)")
            End With

            nRet = rDB.ExecuteNonQuery(SQL)
            If nRet >= 0 Then
                lngSCH_KES_CNT = nRet
            ElseIf nRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール更新)", "失敗", "予期せぬエラー")
                Return False
            End If

            '------------------------------------------------
            'スケジュールマスタ更新[手数料徴求]
            '------------------------------------------------
            lngSCH_TES_CNT = 0
            With SQL
                .Length = 0
                .Append("UPDATE SCHMAST SET")
                .Append(" TESUU_DATE_S = '00000000'")
                .Append(",TESUU_TIME_STAMP_S = '00000000000000'")
                .Append(",TESUUTYO_FLG_S = '0'")
                .Append(" WHERE TESUU_TIME_STAMP_S = " & SQ(strTIME_STAMP_W))
                .Append(" AND TESUUTYO_FLG_S = '1'")
                .Append(" AND EXISTS (")
                .Append("   SELECT * FROM K_I_RENKEIMAST")
                .Append("   WHERE TESUU_TIME_STAMP_S = SAKUSEI_DATE_KR || SAKUSEI_TIME_KR)")
            End With

            nRet = rDB.ExecuteNonQuery(SQL)
            If nRet >= 0 Then
                lngSCH_TES_CNT = nRet
            ElseIf nRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール更新)", "失敗", "予期せぬエラー")
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール更新)", "失敗", ex.Message)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' 資金決済連携データテーブルを削除します（金バッチ）
    ''' </summary>
    ''' <param name="strTIME_STAMP_W">タイムスタンプ</param>
    ''' <param name="lngKES_CNT">削除レコード件数</param>
    ''' <param name="rDB">オラクル</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/09/17 大垣信金 金バッチ対応</remarks>
    Private Function fn_delete_K_I_RENKEIMAST(ByVal strTIME_STAMP_W As String, ByRef lngKES_CNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
        Dim SQL As New StringBuilder
        Dim nRet As Integer

        Try
            '------------------------------------------------
            '資金決済連携データテーブル削除
            '------------------------------------------------
            lngKES_CNT = 0
            With SQL
                .Append("DELETE FROM K_I_RENKEIMAST")
                .Append(" WHERE SAKUSEI_DATE_KR || SAKUSEI_TIME_KR = " & SQ(strTIME_STAMP_W))
            End With

            nRet = rDB.ExecuteNonQuery(SQL)
            If nRet = 0 Then
                lngKES_CNT = nRet
            ElseIf nRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(資金決済連携データテーブル削除)", "失敗", "予期せぬエラー")
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(資金決済連携データテーブル削除)", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function

    'ゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtSyoriDateY.Validating, _
            txtSyoriDateM.Validating, _
            txtSyoriDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub
#End Region

End Class
