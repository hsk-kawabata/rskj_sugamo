Imports System.Text
Imports CASTCommon

''' <summary>
''' 他行分センター送信データ作成画面
''' </summary>
''' <remarks></remarks>
Public Class KF3SMAIN010

#Region "クラス定数"
    Private MainLOG As New CASTCommon.BatchLOG("KF3SMAIN010", "他行分センター送信データ作成画面")
    Private Const msgTitle As String = "他行分センター送信データ作成画面(KF3SMAIN010)"
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

    Private FURI_DATE_5_AFTER As String       '振替日の5営業日後

    Private Structure strcIniInfo
        Dim HAISIN_SSS_1 As String
        Dim SSS_BORDER_TIME As String
    End Structure
    Dim IniInfo As strcIniInfo

    Dim MainDB As CASTCommon.MyOracle

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KF3SMAIN010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        MainDB = New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader = Nothing

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
            '休日マスタ取り込み
            '------------------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)", "失敗")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            '------------------------------------------------
            '設定ファイル読み込み
            '------------------------------------------------
            If GetIniInfo() = False Then
                Return
            End If

            '------------------------------------------------
            'システム日付の5営業日後を算出
            '------------------------------------------------
            Dim strSysDate As String
            Dim strSysTime As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            strSysTime = String.Format("{0:HHmm}", GCom.GetSysDate)
            If strSysTime >= IniInfo.SSS_BORDER_TIME Then
                'ボーダー(13:00)を過ぎていると5営業日後の振替日は表示しない
                bRet = GCom.CheckDateModule(strSysDate, FURI_DATE_5_AFTER, GCom.NzInt(IniInfo.HAISIN_SSS_1) + 1, 0)
            Else
                bRet = GCom.CheckDateModule(strSysDate, FURI_DATE_5_AFTER, GCom.NzInt(IniInfo.HAISIN_SSS_1), 0)
            End If

            '------------------------------------------------
            '画面表示時に振替日検索リストに対象振替日を表示する
            '------------------------------------------------
            Me.cmbFuridate.Items.Clear()
            Dim SQL As New StringBuilder
            With SQL
                .Append("select distinct FURI_DATE_S")
                .Append(" from SCHMAST, TORIMAST")
                .Append(" where FURI_DATE_S >= " & SQ(FURI_DATE_5_AFTER))
                .Append(" and FMT_KBN_T in ('20', '21')")
                .Append(" and TOUROKU_FLG_S = '1'")
                .Append(" and TAKOU_FLG_S = '0'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and FSYORI_KBN_T = '1'")
                .Append(" and TORIS_CODE_S = TORIS_CODE_T")
                .Append(" and TORIF_CODE_S = TORIF_CODE_T")
                .Append(" order by FURI_DATE_S")
            End With

            oraReader = New CASTCommon.MyOracleReader(MainDB)
            Dim strFuriDate As String = String.Empty

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    strFuriDate = oraReader.GetString("FURI_DATE_S")
                    Me.cmbFuridate.Items.Add(strFuriDate.Substring(0, 4) & "年" & strFuriDate.Substring(4, 2) & "月" & strFuriDate.Substring(6, 2) & "日")
                    oraReader.NextRead()
                End While
            End If

            oraReader.Close()

            If Me.cmbFuridate.Items.Count <> 0 Then
                Me.cmbFuridate.SelectedIndex = 0
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 作成ボタン押下イベント
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
            '画面の入力値チェック
            '------------------------------------------------
            If fn_check_text() = False Then
                Return
            End If
            Dim strFuriDate As String = Me.txtFuriDateY.Text & Me.txtFuriDateM.Text & Me.txtFuriDateD.Text
            LW.FuriDate = strFuriDate

            '------------------------------------------------
            '入力振替日が５営業日以降であることを確認
            '------------------------------------------------
            If CInt(strFuriDate) < CInt(FURI_DATE_5_AFTER) Then
                MessageBox.Show(MSG0375W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return
            End If

            '------------------------------------------------
            'スケジュールのチェック
            '------------------------------------------------
            Dim SQL As New StringBuilder
            With SQL
                .Append("select * from SCHMAST, TORIMAST")
                .Append(" where FURI_DATE_S = " & SQ(strFuriDate))
                .Append(" and FMT_KBN_T in ('20', '21')")
                .Append(" and TOUROKU_FLG_S = '1'")
                .Append(" and TAKOU_FLG_S = '0'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and TORIS_CODE_S = TORIS_CODE_T")
                .Append(" and TORIF_CODE_S = TORIF_CODE_T")
            End With

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(SQL) = False Then
                MessageBox.Show(MSG0064W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return
            End If

            oraReader.Close()


            If MessageBox.Show(String.Format(MSG0023I, "他行データ作成"), _
                               msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '------------------------------------------------
            'スケジュールマスタの更新
            '------------------------------------------------
            With SQL
                .Length = 0
                .Append("update SCHMAST set TAKOU_FLG_S = '2'")
                .Append(" where TOUROKU_FLG_S = '1'")
                .Append(" and TAKOU_FLG_S = '0'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and FURI_DATE_S = " & SQ(strFuriDate))
                .Append(" and exists ( ")
                .Append("   select TORIS_CODE_T from TORIMAST")
                .Append("   where TORIS_CODE_T = TORIS_CODE_S")
                .Append("   and TORIF_CODE_T = TORIF_CODE_S")
                .Append("   and FMT_KBN_T in ('20', '21')")
                .Append(" )")
            End With

            Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If nRet = 0 Then
                MainDB.Rollback()
                Return
            ElseIf nRet < 0 Then
                Throw New Exception(String.Format(MSG0027E, "スケジュールマスタ", "更新"))
            End If

            '------------------------------------------------
            'ジョブマスタに登録
            '------------------------------------------------
            Dim JOBID As String = "J020"
            '2017/01/17 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
            'パラメータ変更　スリーエスだと分かるように区分追加
            Dim PARAM As String = "9999999999" & "77" & "," & strFuriDate & ",SSS"
            'Dim PARAM As String = "9999999999" & "77" & "," & strFuriDate
            '2017/01/17 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
            nRet = MainLOG.SearchJOBMAST(JOBID, PARAM, MainDB)
            If nRet = 1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf nRet = -1 Then
                Throw New Exception(String.Format(MSG0002E, "検索"))
            End If

            If MainLOG.InsertJOBMAST(JOBID, LW.UserID, PARAM, MainDB) = False Then
                Throw New Exception(MSG0005E)
            End If

            MainDB.Commit()

            MessageBox.Show(String.Format(MSG0021I, "他行データ作成"), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.Message)
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
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

    ''' <summary>
    ''' 振替日コンボボックスチェンジイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbFuridate_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbFuridate.SelectedIndexChanged
        Try
            If Me.cmbFuridate.Items.Count <> 0 Then
                Dim strFuriDate() As String = Me.cmbFuridate.SelectedItem.ToString.Replace("年", "/").Replace("月", "/").Replace("日", "").Split("/"c)
                Me.txtFuriDateY.Text = strFuriDate(0)
                Me.txtFuriDateM.Text = strFuriDate(1)
                Me.txtFuriDateD.Text = strFuriDate(2)
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックスチェンジイベント)", "失敗", ex.ToString)
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

    ''' <summary>
    ''' 設定ファイルを読み込みます。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function GetIniInfo() As Boolean
        Try
            IniInfo.HAISIN_SSS_1 = GetFSKJIni("JIFURI", "HAISIN_SSS_1")
            If IniInfo.HAISIN_SSS_1.Equals("err") OrElse IniInfo.HAISIN_SSS_1 = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "SSS提携内配信期限", "JIFURI", "HAISIN_SSS_1"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:SSS提携内配信期限 分類:JIFURI項目:HAISIN_SSS_1")
                Return False
            End If

            IniInfo.SSS_BORDER_TIME = GetRSKJIni("RSV2_V1.0.0", "SSS_BORDER_TIME")
            If IniInfo.SSS_BORDER_TIME.Equals("err") OrElse IniInfo.SSS_BORDER_TIME = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "SSS配信境界時間", "RSV2_V1.0.0", "SSS_BORDER_TIME"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:SSS配信境界時間 分類:RSV2_V1.0.0 項目:SSS_BORDER_TIME")
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(設定ファイル取得)", "失敗", ex.Message)
            Return False
        End Try
    End Function

#End Region

End Class
