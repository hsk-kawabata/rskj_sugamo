Imports System.Text
Imports CASTCommon

''' <summary>
''' 企業別結果表印刷画面
''' </summary>
''' <remarks></remarks>
Public Class KF3SPRNT040

#Region "クラス定数"
    Private MainLOG As New CASTCommon.BatchLOG("KF3SPRNT040", "企業別結果表印刷画面")
    Private Const msgTitle As String = "企業別結果表印刷画面(KF3SPRNT040)"
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

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KF3SPRNT040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            '取引先コンボボックスの設定
            '------------------------------------------------
            Dim Jyoken As String = " AND FMT_KBN_T in ('20', '21')"   'フォーマット区分が集金代行
            If GCom.SelectItakuName("", Me.cmbToriName, Me.txtTorisCode, Me.txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            Me.txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 印刷ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            '------------------------------------------------
            '画面入力値のチェック
            '------------------------------------------------
            If fn_check_text() = False Then
                Return
            End If

            '------------------------------------------------
            'テーブルのチェック
            '------------------------------------------------
            If fn_check_Table() = False Then
                Return
            End If

            Dim strFuriDate As String = Me.txtFuriDateY.Text & Me.txtFuriDateM.Text & Me.txtFuriDateD.Text
            Dim strToriCode As String = Me.txtTorisCode.Text & Me.txtTorifCode.Text
            LW.ToriCode = strToriCode
            LW.FuriDate = strFuriDate

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "企業別結果表"), _
                               msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '------------------------------------------------
            '印刷バッチ呼び出し
            '------------------------------------------------
            Dim param As String
            Dim nRet As Integer
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            param = GCom.GetUserID & "," & strToriCode & "," & strFuriDate
            nRet = ExeRepo.ExecReport("KF3SP004.EXE", param)

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, "企業別結果表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "企業別結果表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
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
         Handles txtTorisCode.Validating, txtTorifCode.Validating, _
                txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' 取引先カナ検索コンボボックスチェンジイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '選択カナで始まる委託者名を取得
        '--------------------------------
        Try
            If Not Me.cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(Me.cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定
                Dim Jyoken As String = " AND FMT_KBN_T in ('20', '21')"   'フォーマット区分が集金代行
                If GCom.SelectItakuName(Me.cmbKana.SelectedItem.ToString, Me.cmbToriName, Me.txtTorisCode, Me.txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
            Me.cmbToriName.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コンボボックス設定)", "失敗", ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' 取引先検索コンボボックスチェンジイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        '-------------------------------------------
        '取引先コードテキストボックスに取引先コード設定
        '-------------------------------------------
        Try
            If Not Me.cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(Me.cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(Me.cmbToriName, Me.txtTorisCode, Me.txtTorifCode)
                Me.txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コード取得)", "失敗", ex.ToString)
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

    ''' <summary>
    ''' テーブルのチェックを行います。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_check_Table() As Boolean
        MainDB = New CASTCommon.MyOracle
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder
        Dim strFmtKbn As String = String.Empty

        Try
            If String.Concat(Me.txtTorisCode.Text, Me.txtTorifCode.Text).Equals("999999999999") = False Then
                '------------------------------------------------
                '取引先情報取得
                '------------------------------------------------
                With SQL
                    .Append("select * from TORIMAST")
                    .Append(" where TORIS_CODE_T = " & SQ(Me.txtTorisCode.Text))
                    .Append(" and TORIF_CODE_T = " & SQ(Me.txtTorifCode.Text))
                End With
                If oraReader.DataReader(SQL) = True Then
                    strFmtKbn = oraReader.GetString("FMT_KBN_T")
                    oraReader.Close()
                Else
                    '取引先なし
                    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Return False
                End If

                'フォーマット区分チェック
                If strFmtKbn.Equals("20") = False AndAlso strFmtKbn.Equals("21") = False Then
                    MessageBox.Show(MSG0376W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If

            '------------------------------------------------
            'スケジュールマスタ参照
            '------------------------------------------------
            With SQL
                .Length = 0
                .Append("select count(*) as COUNTER")
                .Append(" from TORIMAST, SCHMAST")
                .Append(" where FSYORI_KBN_S = '1'")
                If String.Concat(Me.txtTorisCode.Text, Me.txtTorifCode.Text).Equals("999999999999") = False Then
                    .Append(" and TORIS_CODE_T = " & SQ(Me.txtTorisCode.Text))
                    .Append(" and TORIF_CODE_T = " & SQ(Me.txtTorifCode.Text))
                Else
                    .Append(" and FMT_KBN_T in ('20', '21')")
                End If
                .Append(" and TORIS_CODE_T = TORIS_CODE_S")
                .Append(" and TORIF_CODE_T = TORIF_CODE_S")
                .Append(" and FURI_DATE_S = " & SQ(Me.txtFuriDateY.Text & Me.txtFuriDateM.Text & Me.txtFuriDateD.Text))
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and TOUROKU_FLG_S = '1'")
                .Append(" and HAISIN_FLG_S = '1'")
                .Append(" and FUNOU_FLG_S = '1'")
                .Append(" and TAKOU_FLG_S = '1'")
            End With

            Dim Count As Integer
            If oraReader.DataReader(SQL) = True Then
                Count = oraReader.GetInt("COUNTER")
                oraReader.Close()
            Else
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                oraReader.Close()
                Me.txtFuriDateY.Focus()
                Return False
            End If

            If Count = 0 Then
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If oraReader IsNot Nothing Then oraReader.Close()
            If MainDB IsNot Nothing Then MainDB.Close()
        End Try
    End Function

#End Region

End Class