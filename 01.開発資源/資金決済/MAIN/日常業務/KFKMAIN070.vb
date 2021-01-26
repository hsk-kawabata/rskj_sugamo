Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKMAIN070

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKMAIN070", "資金決済データ結果更新画面")
    Private Const msgtitle As String = "資金決済データ結果更新画面(KFKMAIN070)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    '画面LOAD時
    Private Sub KFKMAIN070_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            cmbTimeStamp.Enabled = False
            btnReset.Enabled = False
            '------------------------------------------------
            '実行ボタンを非表示設定
            '------------------------------------------------
            btnAction.Enabled = False

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

            If MessageBox.Show(String.Format(MSG0015I, "資金決済データ結果更新"), _
                                   msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '金バッチ結果更新処理
            If Me.UpdateKekkaKinBatch(strTIME_STAMP_W, MainDB) = False Then
                Return
            End If

            MessageBox.Show(String.Format(MSG0016I, "資金決済データ結果更新"), _
                            msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            
            cmbTimeStamp.Items.Clear()

            btnSearch.Enabled = True
            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True
            btnReset.Enabled = False
            cmbTimeStamp.Enabled = False
            btnAction.Enabled = False
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")

            btnSearch.Enabled = True
            btnAction.Enabled = False
            btnReset.Enabled = False
            cmbTimeStamp.Enabled = False

            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True

            'システム日付を再表示
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            cmbTimeStamp.Items.Clear()
            txtSyoriDateY.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub

#Region "KFKMAIN070用関数"

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

            '桁数チェック
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
                .Append("  AND KESSAI_FLG_S = '1'")
                .Append("  AND SYORI_STS_KR = 9)")
                .Append(" UNION")
                .Append(" (SELECT TESUU_TIME_STAMP_S AS TIME_STAMP")
                .Append("  FROM SCHMAST, K_I_RENKEIMAST")
                .Append("  WHERE TESUU_TIME_STAMP_S = SAKUSEI_DATE_KR || SAKUSEI_TIME_KR")
                .Append("  AND TESUU_TIME_STAMP_S BETWEEN " & SQ(strJOKEN1) & " AND " & SQ(strJOKEN2))
                .Append("  AND TESUUTYO_FLG_S = '1'")
                .Append("  AND SYORI_STS_KR = 9)")
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
                    MessageBox.Show(MSG0055W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                '実行ボタンを非表示
                btnAction.Enabled = False
                txtSyoriDateY.Focus()
            Else
                '実行ボタンを表示
                btnAction.Enabled = True

                btnSearch.Enabled = False
                txtSyoriDateD.Enabled = False
                txtSyoriDateM.Enabled = False
                txtSyoriDateY.Enabled = False
                cmbTimeStamp.Enabled = True
                btnReset.Enabled = True
                cmbTimeStamp.SelectedIndex = 0
                cmbTimeStamp.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュールマスタ検索)", "失敗", ex.Message)

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

    ''' <summary>
    ''' 資金決済データ結果更新(金バッチ)処理を行います。
    ''' </summary>
    ''' <param name="TimeStamp">タイムスタンプ</param>
    ''' <param name="db">オラクル</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2013/09/17 大垣信金 金バッチ対応</remarks>
    Private Function UpdateKekkaKinBatch(ByVal TimeStamp As String, _
                                         ByVal db As CASTCommon.MyOracle) As Boolean

        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim List As KFKP003
        Dim CsvFileName As String = String.Empty
        Dim strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
        Dim strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

        Try
            '資金決済連携データテーブルから対象のレコードを取得する
            Dim strSakuseiDate As String = TimeStamp.Substring(0, 8)
            Dim strSakuseiTime As String = TimeStamp.Substring(8, 6)

            With SQL
                .Length = 0
                .Append("SELECT")
                .Append(" TORIS_CODE_KR")
                .Append(",TORIF_CODE_KR")
                .Append(",ITAKU_NNAME_KR")
                .Append(",FURI_DATE_KR")
                .Append(",KAMOKU_KR")
                .Append(",OPE_KR")
                .Append(",ERR_MESSAGE_CODE_KR")
                .Append(",ERR_MESSAGE_KR")
                .Append(" FROM K_I_RENKEIMAST")
                .Append(" WHERE SAKUSEI_DATE_KR = " & SQ(strSakuseiDate))
                .Append(" AND SAKUSEI_TIME_KR = " & SQ(strSakuseiTime))
                .Append(" AND SYORI_STS_KR = 9")
                .Append(" ORDER BY KAIJI_KR, RECORD_KR")
            End With

            oraReader = New CASTCommon.MyOracleReader(db)

            If oraReader.DataReader(SQL) = True Then
                '更新対象あり
                List = New KFKP003
                CsvFileName = List.CreateCsvFile

                While oraReader.EOF = False
                    List.CSVObject.Output(strDate, True)            '処理日
                    List.CSVObject.Output(strTime, True)            '処理時間
                    List.CSVObject.Output(oraReader.GetString("TORIS_CODE_KR"), True)   '取引先主コード
                    List.CSVObject.Output(oraReader.GetString("TORIF_CODE_KR"), True)   '取引先副コード
                    List.CSVObject.Output(oraReader.GetString("ITAKU_NNAME_KR"), True)  '委託者名
                    List.CSVObject.Output(oraReader.GetString("FURI_DATE_KR"), True)    '振替日
                    List.CSVObject.Output(oraReader.GetString("KAMOKU_KR"), True)       '科目コード
                    List.CSVObject.Output(oraReader.GetString("OPE_KR"), True)          'オペコード
                    List.CSVObject.Output(oraReader.GetString("ERR_MESSAGE_CODE_KR"), True) 'エラーメッセージコード
                    List.CSVObject.Output(oraReader.GetString("ERR_MESSAGE_KR"), True, True) 'エラーメッセージ

                    oraReader.NextRead()

                End While
            Else
                '更新対象なし
                oraReader.Close()
                oraReader = Nothing
                MessageBox.Show(MSG0055W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '処理結果確認表の印刷処理
            oraReader.Close()
            oraReader = Nothing

            List.CloseCsv()
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String
            Dim nRet As Integer

            ExeRepo.SetOwner = Me
            param = GCom.GetUserID & "," & CsvFileName & "," & "1"
            nRet = ExeRepo.ExecReport("KFKP003.EXE", param)

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(String.Format(MSG0226W, "処理結果確認表(資金決済データ結果更新)"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "処理結果確認表(資金決済データ結果更新)"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
            End Select

            Return True

        Catch ex As Exception
            MainLOG.Write("資金決済データ結果更新", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False

        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try

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
