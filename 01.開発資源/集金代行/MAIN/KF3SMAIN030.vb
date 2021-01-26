Imports System.Text
Imports CASTCommon

''' <summary>
''' 他行分センター送信データ作成取消画面
''' </summary>
''' <remarks></remarks>
Public Class KF3SMAIN030

#Region "クラス定数"
    Private MainLOG As New CASTCommon.BatchLOG("KF3SMAIN030", "他行分センター送信データ作成取消画面")
    Private Const msgTitle As String = "他行分センター送信データ作成取消画面(KF3SMAIN030)"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
#End Region

#Region "クラス変数"
    Private Structure LogWrite
        Dim UserID As String        'ユーザID
        Dim ToriCode As String      '取引先コード
        Dim FuriDate As String      '振替日
    End Structure
    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle

    Private KinkoCd As String

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KF3SMAIN030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '------------------------------------------------
            'ログの書込に必要な情報の取得
            '------------------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '------------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Me.Label1, Me.Label2, Me.lblUser, Me.lblDate)

            '------------------------------------------------
            '処理日にシステム日付を表示
            '------------------------------------------------
            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            Me.txtFuriDateY.Text = strSysDate.Substring(0, 4)
            Me.txtFuriDateM.Text = strSysDate.Substring(4, 2)
            Me.txtFuriDateD.Text = strSysDate.Substring(6, 2)

            '項目入力制御
            Me.txtFuriDateY.Enabled = True
            Me.txtFuriDateM.Enabled = True
            Me.txtFuriDateD.Enabled = True
            Me.btnSearch.Enabled = True
            Me.btnAction.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
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

        MainDB = New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)開始", "成功", "")

            '------------------------------------------------
            '画面入力値のチェック
            '------------------------------------------------
            If fn_check_text() = False Then
                Return
            End If
            Dim strFuriDate As String = Me.txtFuriDateY.Text & Me.txtFuriDateM.Text & Me.txtFuriDateD.Text
            LW.FuriDate = strFuriDate

            '自金庫コード取得
            Me.KinkoCd = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If Me.KinkoCd.Equals("err") = True OrElse Me.KinkoCd.Trim = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                Return
            End If

            '------------------------------------------------
            'スケジュール検索
            '------------------------------------------------
            Me.cmbTimeStamp.Items.Clear()
            Me.cmbTimeStamp.Text = String.Empty

            Dim strTIME_STAMP_START As String = strFuriDate & "000000"
            Dim strTIME_STAMP_END As String = strFuriDate & "999999"
            Dim intCOUNT As Integer
            Dim dtDate As New DateTime

            Dim SQL As New StringBuilder
            With SQL
                .Append("select distinct YOBI1_S")
                .Append(" from SCHMAST")
                .Append(" where YOBI1_S between " & SQ(strTIME_STAMP_START) & " and " & SQ(strTIME_STAMP_END))
                .Append(" and TAKOU_FLG_S = '1'")
                .Append(" and HENKAN_FLG_S = '0'")
                .Append(" and TYUUDAN_FLG_S = '0'")
            End With

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            intCOUNT = 0
            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    intCOUNT += 1
                    dtDate = GCom.SET_DATE(oraReader.GetString("YOBI1_S"))
                    Me.cmbTimeStamp.Items.Add(dtDate.ToString("yyyy/MM/dd HH:mm:ss"))
                    oraReader.NextRead()
                End While
            Else
                MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return
            End If

            oraReader.Close()


            If intCOUNT = 0 Then
                MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return
            Else
                If Me.cmbTimeStamp.Items.Count > 0 Then
                    Me.cmbTimeStamp.SelectedIndex = 0
                End If

                '項目入力制御
                Me.txtFuriDateY.Enabled = False
                Me.txtFuriDateM.Enabled = False
                Me.txtFuriDateD.Enabled = False
                Me.btnSearch.Enabled = False
                Me.btnAction.Enabled = True
            End If

        Catch ex As Exception
            MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)", "失敗", ex.Message)
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
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

        MainDB = New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            '------------------------------------------------
            '画面入力値のチェック
            '------------------------------------------------
            If fn_check_text() = False Then
                Return
            End If

            If MessageBox.Show(String.Format(MSG0015I, "他行分センター送信データ作成取消"), _
                               msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '------------------------------------------------
            'スケジュール検索＆更新
            '------------------------------------------------
            Dim SQL As New StringBuilder
            With SQL
                .Append("select TORIS_CODE_S, TORIF_CODE_S, FURI_DATE_S")
                .Append(" from SCHMAST")
                .Append(" where TAKOU_FLG_S = '1'")
                .Append(" and HENKAN_FLG_S = '0'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and YOBI1_S = " & SQ(GCom.NzDec(Me.cmbTimeStamp.SelectedItem, "")))
            End With

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False

                    'スケジュールマスタ更新
                    With SQL
                        .Length = 0
                        .Append("update SCHMAST set")
                        .Append(" TAKOU_FLG_S = '0'")
                        .Append(" where TORIS_CODE_S = " & SQ(oraReader.GetString("TORIS_CODE_S")))
                        .Append(" and TORIF_CODE_S = " & SQ(oraReader.GetString("TORIF_CODE_S")))
                        .Append(" and FURI_DATE_S = " & SQ(oraReader.GetString("FURI_DATE_S")))
                        .Append(" and YOBI1_S = " & SQ(GCom.NzDec(Me.cmbTimeStamp.SelectedItem, "")))
                        .Append(" and HENKAN_FLG_S = '0'")
                        .Append(" and TAKOU_FLG_S = '1'")
                        .Append(" and TYUUDAN_FLG_S = '0'")
                    End With

                    Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
                    If nRet < 1 Then
                        MessageBox.Show(String.Format(MSG0002E, "更新"), _
                                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainDB.Rollback()
                        Return
                    End If

                    '他行スケジュールマスタ削除
                    With SQL
                        .Length = 0
                        .Append("delete from TAKOSCHMAST")
                        .Append(" where TORIS_CODE_U = " & SQ(oraReader.GetString("TORIS_CODE_S")))
                        .Append(" and TORIF_CODE_U = " & SQ(oraReader.GetString("TORIF_CODE_S")))
                        .Append(" and FURI_DATE_U = " & SQ(oraReader.GetString("FURI_DATE_S")))
                        .Append(" and TKIN_NO_U = " & SQ(Me.KinkoCd))
                    End With

                    nRet = MainDB.ExecuteNonQuery(SQL)
                    If nRet < 1 Then
                        MessageBox.Show(String.Format(MSG0002E, "削除"), _
                                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainDB.Rollback()
                        Return
                    End If

                    oraReader.NextRead()
                End While
            End If

            oraReader.Close()
            MainDB.Commit()

            Me.cmbTimeStamp.Items.Clear()
            Me.cmbTimeStamp.Text = String.Empty

            '項目入力制御
            Me.txtFuriDateY.Enabled = True
            Me.txtFuriDateM.Enabled = True
            Me.txtFuriDateD.Enabled = True
            Me.btnSearch.Enabled = True
            Me.btnAction.Enabled = False

            MessageBox.Show(String.Format(MSG0025I, "他行分センター送信データ作成"), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.Message)
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
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
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")

            '処理日にシステム日付を設定
            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            Me.txtFuriDateY.Text = strSysDate.Substring(0, 4)
            Me.txtFuriDateM.Text = strSysDate.Substring(4, 2)
            Me.txtFuriDateD.Text = strSysDate.Substring(6, 2)

            'コンボボックス設定
            Me.cmbTimeStamp.Items.Clear()
            Me.cmbTimeStamp.Text = String.Empty

            '項目入力制御
            Me.txtFuriDateY.Enabled = True
            Me.txtFuriDateM.Enabled = True
            Me.txtFuriDateD.Enabled = True
            Me.btnSearch.Enabled = True
            Me.btnAction.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.Message)
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
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
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
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
         Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' テキストボックスの入力チェックを行います。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_check_text() As Boolean
        Try
            '振替日(年)必須チェック
            If Me.txtFuriDateY.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '振替日(月)必須チェック
            If Me.txtFuriDateM.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateM.Focus()
                Return False
            End If

            '振替日(月)範囲チェック
            If CInt(Me.txtFuriDateM.Text) < 1 OrElse CInt(Me.txtFuriDateM.Text) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateM.Focus()
                Return False
            End If

            '振替日(日)必須チェック
            If Me.txtFuriDateD.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateD.Focus()
                Return False
            End If

            '振替日(日)範囲チェック
            If CInt(Me.txtFuriDateD.Text) < 1 OrElse CInt(Me.txtFuriDateD.Text) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateD.Focus()
                Return False
            End If

            '振替日整合性チェック
            Dim WORK_DATE As String = Me.txtFuriDateY.Text & "/" & Me.txtFuriDateM.Text & "/" & Me.txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.Message)
            Return False
        End Try
    End Function

#End Region

End Class