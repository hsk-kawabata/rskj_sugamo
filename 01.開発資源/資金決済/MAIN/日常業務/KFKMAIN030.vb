Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKMAIN030

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKMAIN030", "資金決済リエンタ結果更新画面")
    Private Const msgtitle As String = "資金決済リエンタ結果更新画面(KFKMAIN030)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    '2018/01/23 saitou 広島信金(RSV2標準) ADD 資金決済リエンタ結果更新対応 -------------------- START
    Private Structure strcIni
        Dim RIENTA_PATH As String
        Dim RIENTA_FILENAME As String
        Dim RSV2_EDITION As String
        Dim COMMON_BAITAIREAD As String
    End Structure
    Private ini_info As strcIni
    '2018/01/23 saitou 広島信金(RSV2標準) ADD ------------------------------------------------- END

    '画面LOAD時
    Private Sub KFKMAIN030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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

            '2018/01/23 saitou 広島信金(RSV2標準) ADD 資金決済リエンタ結果更新対応 -------------------- START
            '------------------------------------------------
            '設定ファイル取得
            '------------------------------------------------
            If Me.SetIniFIle() = False Then
                Return
            End If
            '2018/01/23 saitou 広島信金(RSV2標準) ADD ------------------------------------------------- END

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

            If MessageBox.Show(MSG0023I.Replace("{0}", "資金決済リエンタ結果更新"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '2018/01/23 saitou 広島信金(RSV2標準) ADD 資金決済リエンタ結果更新対応 -------------------- START
            Dim ret As Integer = 0
            If ini_info.RSV2_EDITION = "2" Then
                '大規模版はバッチ登録前に媒体からリエンタをコピーする
                Do
                    Try
                        Dim DirInfo As New DirectoryInfo(ini_info.RIENTA_PATH)
                        Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories

                        ret = 0
                        Exit Do

                    Catch ex As Exception
                        If MessageBox.Show(String.Format(MSG0066I, Path.GetPathRoot(ini_info.RIENTA_PATH)), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> Windows.Forms.DialogResult.OK Then
                            ret = -1
                            Exit Do
                        End If
                    End Try
                Loop

                If ret = 0 Then
                    File.Copy(Path.Combine(ini_info.RIENTA_PATH, ini_info.RIENTA_FILENAME), Path.Combine(ini_info.COMMON_BAITAIREAD, ini_info.RIENTA_FILENAME), True)
                End If
            End If

            If ret <> 0 Then
                Return
            End If
            '2018/01/23 saitou 広島信金(RSV2標準) ADD ------------------------------------------------- END

            MainDB.BeginTrans()     'トランザクション開始

            Dim jobid As String
            Dim para As String

            'ジョブマスタに登録
            jobid = "K020"                      '..\Batch\資金決済データ作成\
            para = strTIME_STAMP_W                  '決済日をパラメタとして設定

            '#########################
            'job検索
            '#########################
            Dim iRet As Integer
            iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            If iRet = 1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf iRet = -1 Then
                Throw New Exception(MSG0002E.Replace("{0}", "検索"))
            End If

            '#########################
            'job登録
            '#########################
            If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then
                Throw New Exception(MSG0005E)
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0021I.Replace("{0}", "資金決済リエンタ結果更新"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

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

#Region "KFKMAIN030用関数"

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
            SQL.Append("SELECT")
            SQL.Append(" TIME_STUMP")
            SQL.Append(" FROM")
            SQL.Append(" ((SELECT")
            SQL.Append("   KESSAI_TIME_STAMP_S AS TIME_STUMP")
            SQL.Append("  FROM")
            SQL.Append("   SCHMAST")
            SQL.Append("  ,KESSAIMAST")
            SQL.Append("  WHERE")
            SQL.Append("       KESSAI_TIME_STAMP_S = TIME_STAMP_KR")
            SQL.Append("   AND KESSAI_TIME_STAMP_S BETWEEN '" & strJOKEN1 & "' AND '" & strJOKEN2 & "'")
            SQL.Append("   AND KESSAI_FLG_S = '1')")
            SQL.Append("  UNION ")
            SQL.Append(" (SELECT")
            SQL.Append("   TESUU_TIME_STAMP_S AS TIME_STUMP")
            SQL.Append("  FROM")
            SQL.Append("   SCHMAST")
            SQL.Append("  ,KESSAIMAST")
            SQL.Append("  WHERE")
            SQL.Append("       TESUU_TIME_STAMP_S = TIME_STAMP_KR")
            SQL.Append("   AND TESUU_TIME_STAMP_S BETWEEN '" & strJOKEN1 & "' AND '" & strJOKEN2 & "'")
            SQL.Append("   AND TESUUTYO_FLG_S = '1'))")
            SQL.Append(" GROUP BY TIME_STUMP")
            SQL.Append(" ORDER BY TIME_STUMP")

            'コンボボックスの初期化
            cmbTimeStamp.Items.Clear()

            '再描画しないようにする
            cmbTimeStamp.BeginUpdate()

            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF
                    '------------------------------------------------
                    'コンボボックスにリストを追加する
                    '------------------------------------------------
                    strTIME_STAMP = GCom.NzStr(OraReader.GetString("TIME_STUMP"))

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

    '2018/01/23 saitou 広島信金(RSV2標準) DEL 資金決済リエンタ結果更新対応 -------------------- START
    '不要関数
    'Private Function fn_update_SCHMAST(ByVal strTIME_STAMP_W As String, ByRef lngSCH_KES_CNT As Long, ByRef lngSCH_TES_CNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
    '    '============================================================================
    '    'NAME           :fn_update_SCHMAST
    '    'Parameter      :
    '    'Description    :スケジュールマスタ更新
    '    'Return         :True=OK,False=NG
    '    'Create         :
    '    'Update         :
    '    '============================================================================
    '    Dim SQL As String = ""

    '    Try

    '        lngSCH_KES_CNT = 0
    '        '------------------------------------------------
    '        'スケジュールマスタ更新[決済]
    '        '------------------------------------------------
    '        SQL = ""
    '        SQL = "UPDATE SCHMAST SET"
    '        SQL &= "  KESSAI_DATE_S = '" & "".PadLeft(8, "0"c) & "'"
    '        SQL &= " ,KESSAI_TIME_STAMP_S = '" & "".PadLeft(14, "0"c) & "'"
    '        SQL &= " ,KESSAI_FLG_S = '0'"
    '        SQL &= " WHERE KESSAI_TIME_STAMP_S = '" & strTIME_STAMP_W & "'"
    '        SQL &= "   AND KESSAI_FLG_S = '1'"
    '        SQL &= "   AND EXISTS"
    '        SQL &= "       (SELECT * FROM KESSAIMAST"
    '        SQL &= "         WHERE KESSAI_TIME_STAMP_S = TIME_STAMP_KR)"
    '        'SQL &= "           AND DATA_CRT_KBN_K = '01')"          '01:リエンタFD作成済

    '        Dim nRet As Integer = rDB.ExecuteNonQuery(SQL)
    '        If nRet >= 0 Then
    '            lngSCH_KES_CNT = nRet
    '        ElseIf nRet < 0 Then
    '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(fn_update_SCHMAST)", "失敗", "予期せぬエラー")
    '            Return False
    '        End If

    '        lngSCH_TES_CNT = 0
    '        '------------------------------------------------
    '        'スケジュールマスタ更新[手数料徴求]
    '        '------------------------------------------------
    '        SQL = ""
    '        SQL = "UPDATE SCHMAST SET"
    '        SQL &= "  TESUU_DATE_S = '" & "".PadLeft(8, "0"c) & "'"
    '        SQL &= " ,TESUU_TIME_STAMP_S = '" & "".PadLeft(14, "0"c) & "'"
    '        SQL &= " ,TESUUTYO_FLG_S = '0'"
    '        SQL &= " WHERE TESUU_TIME_STAMP_S = '" & strTIME_STAMP_W & "'"
    '        SQL &= "   AND TESUUTYO_FLG_S = '1'"
    '        SQL &= "   AND EXISTS"
    '        SQL &= "       (SELECT * FROM KESSAIMAST"
    '        SQL &= "         WHERE TESUU_TIME_STAMP_S = TIME_STAMP_KR)"
    '        ' SQL &= "           AND DATA_CRT_KBN_K = '01')"          '01:リエンタFD作成済

    '        nRet = rDB.ExecuteNonQuery(SQL)
    '        If nRet >= 0 Then
    '            lngSCH_TES_CNT = nRet
    '        ElseIf nRet < 0 Then
    '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュールマスタ更新)", "失敗", "予期せぬエラー")
    '            Return False
    '        End If

    '        Return True

    '    Catch ex As Exception
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュールマスタ更新)", "失敗", ex.Message)
    '        Return False
    '    Finally
    '    End Try

    'End Function

    'Private Function fn_delete_KESSAIMAST(ByVal strTIME_STAMP_W As String, ByRef lngKES_CNT As Long, ByVal rDB As CASTCommon.MyOracle) As Boolean
    '    '============================================================================
    '    'NAME           :fn_delete_KESSAIMAST
    '    'Parameter      :
    '    'Description    :資金決済マスタ削除
    '    'Return         :True=OK,False=NG
    '    'Create         :
    '    'Update         :
    '    '============================================================================
    '    Dim SQL As String = ""
    '    lngKES_CNT = 0
    '    Dim nRet As Integer
    '    Try

    '        '------------------------------------------------
    '        '資金決済マスタ削除
    '        '------------------------------------------------
    '        SQL = "DELETE FROM KESSAIMAST"
    '        SQL &= " WHERE TIME_STAMP_KR = '" & strTIME_STAMP_W & "'"

    '        nRet = rDB.ExecuteNonQuery(SQL)
    '        If nRet = 0 Then
    '            lngKES_CNT = nRet
    '        ElseIf nRet < 0 Then
    '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(資金決済マスタ削除)", "失敗", "予期せぬエラー")
    '            Return False
    '        End If

    '    Catch ex As Exception
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(資金決済マスタ削除)", "失敗", ex.Message)
    '        Return False
    '    Finally

    '    End Try

    '    Return True
    'End Function
    '2018/01/23 saitou 広島信金(RSV2標準) DEL ------------------------------------------------- END

    'ゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtSyoriDateY.Validating, _
            txtSyoriDateM.Validating, _
            txtSyoriDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

    '2018/01/23 saitou 広島信金(RSV2標準) ADD 資金決済リエンタ結果更新対応 -------------------- START
    Private Function SetIniFIle() As Boolean
        ini_info.RIENTA_PATH = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")        'リエンタファイル作成先
        If ini_info.RIENTA_PATH = "err" OrElse ini_info.RIENTA_PATH = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:リエンタファイル作成フォルダ 分類:COMMON 項目:RIENTADR")
            MessageBox.Show(String.Format(MSG0001E, "リエンタファイル作成フォルダ", "COMMON", "RIENTADR"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ini_info.RIENTA_FILENAME = CASTCommon.GetFSKJIni("KESSAI", "RIENTANAME")       'リエンタファイル名
        If ini_info.RIENTA_FILENAME = "err" OrElse ini_info.RIENTA_FILENAME = "" OrElse ini_info.RIENTA_FILENAME.Length > 12 Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:リエンタファイル名 分類:KESSAI 項目:RIENTANAME")
            MessageBox.Show(String.Format(MSG0001E, "リエンタファイル名", "COMMON", "RIENTANAME"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ini_info.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
        If ini_info.RSV2_EDITION = "err" OrElse ini_info.RSV2_EDITION = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:RSV2機能設定 分類:RSV2_V1.0.0 項目:EDITION")
            MessageBox.Show(String.Format(MSG0001E, "RSV2機能設定", "RSV2_V1.0.0", "EDITION"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ini_info.COMMON_BAITAIREAD = CASTCommon.GetFSKJIni("COMMON", "BAITAIREAD")
        If ini_info.COMMON_BAITAIREAD = "err" OrElse ini_info.COMMON_BAITAIREAD = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:媒体読込用フォルダ 分類:COMMON 項目:BAITAIREAD")
            MessageBox.Show(String.Format(MSG0001E, "媒体読込用フォルダ", "COMMON", "BAITAIREAD"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    '2018/01/23 saitou 広島信金(RSV2標準) ADD ------------------------------------------------- END

#End Region


End Class
