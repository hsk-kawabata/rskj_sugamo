Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFJMAIN041

    Public FURI_DATE As String
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private strSOUSIN_KBN As String

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN041", "不能結果更新(企業持込)画面")
    Private Const msgTitle As String = "不能結果更新(企業持込)画面(KFJMAIN041)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle
    'パブリックデーターベース

    Private Const ThisModuleName As String = "KFKMAST041.vb"

#Region " ロード"
    Private Sub KFJMAIN041_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '--------------------------------
            '委託者名リストボックスの設定
            '--------------------------------
            '取引先コンボボックス設定
            Dim Jyoken As String = " AND SOUSIN_KBN_T = '1'"   '送信区分が全銀方式
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 更新"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :不能結果更新実行ボタン(企業持込)
        'Return         :
        'Create         :2009/09/10
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            FURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            Dim strTORIS_CODE As String = txtTorisCode.Text
            Dim strTORIF_CODE As String = txtTorifCode.Text

            LW.ToriCode = strTORIS_CODE & strTORIF_CODE
            LW.FuriDate = FURI_DATE

            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            '更新前確認メッセージ
            If MessageBox.Show(String.Format(MSG0023I, "不能結果更新(企業持込)"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            MainDB = New CASTCommon.MyOracle
            MainDB.BeginTrans()

            Dim jobid As String
            Dim para As String

            ' 2016/04/22 タスク）綾部 CHG 【PG】UI_B-99-99(RSV2対応) -------------------- START
            ''ジョブマスタに登録
            'jobid = "J040"                      '..\Batch\結果更新\
            ''パラメータ(振替日,持込区分(企業持込),更新キー項目,取引先コード)
            'para = FURI_DATE + ",1,0," + strTORIS_CODE & strTORIF_CODE

            ''#########################
            ''job検索
            ''#########################
            'Dim iRet As Integer
            'iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            'If iRet = 1 Then    'ジョブ登録済
            '    MainDB.Rollback()
            '    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Return
            'ElseIf iRet = -1 Then 'ジョブ検索失敗
            '    MainDB.Rollback()
            '    MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブマスタ検索失敗 パラメータ:" & para)
            '    Return
            'End If

            ''#########################
            ''job登録
            ''#########################
            'If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then 'ジョブ登録失敗
            '    MainDB.Rollback()
            '    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブ登録失敗 パラメータ:" & para)
            '    Return
            'End If
            '--------------------------------
            ' JOB登録（ALL8指定）
            '--------------------------------
            If Trim(txtTorisCode.Text) & Trim(txtTorifCode.Text) = "888888888888" Then
                Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
                Dim OraReader2 As New CASTCommon.MyOracleReader(MainDB)
                Dim SQL As New StringBuilder(128)
                Dim SQL2 As New StringBuilder(128)

                SQL = New StringBuilder(128)
                SQL.Append("SELECT")
                SQL.Append("      ITAKU_KANRI_CODE_T")
                SQL.Append(" FROM")
                SQL.Append("      TORIMAST")
                SQL.Append("    , SCHMAST")
                SQL.Append(" WHERE ")
                SQL.Append("      TORIS_CODE_T   = TORIS_CODE_S")
                SQL.Append("  AND TORIF_CODE_T   = TORIF_CODE_S")
                SQL.Append("  AND FURI_DATE_S    = " & SQ(FURI_DATE))
                SQL.Append("  AND SOUSIN_KBN_S   = '1'")
                SQL.Append("  AND HAISIN_FLG_S   = '1'")
                SQL.Append("  AND FUNOU_FLG_S    = '0'")
                SQL.Append("  AND TYUUDAN_FLG_S  = '0'")
                SQL.Append(" GROUP BY")
                SQL.Append("      ITAKU_KANRI_CODE_T")

                If OraReader.DataReader(SQL) = True Then
                    While OraReader.EOF = False
                        Dim tmp_ITAKU_KANRI_CODE_T As String = ""
                        '2017/03/28 FJH)森 CHG 標準版修正（潜在バグ修正）------------------------------------ START
                        tmp_ITAKU_KANRI_CODE_T = OraReader.Reader.Item("ITAKU_KANRI_CODE_T")
                        'tmp_ITAKU_KANRI_CODE_T = GCom.NzInt(OraReader.Reader.Item("ITAKU_KANRI_CODE_T"))
                        '2017/03/28 FJH)森 CHG 標準版修正（潜在バグ修正）------------------------------------ END

                        SQL2 = New StringBuilder(128)
                        SQL2.Append("SELECT")
                        SQL2.Append("      TORIS_CODE_T")
                        SQL2.Append("    , TORIF_CODE_T")
                        SQL2.Append(" FROM")
                        SQL2.Append("      TORIMAST")
                        SQL2.Append("    , SCHMAST")
                        SQL2.Append(" WHERE ")
                        SQL2.Append("      TORIS_CODE_T       = TORIS_CODE_S")
                        SQL2.Append("  AND TORIF_CODE_T       = TORIF_CODE_S")
                        SQL2.Append("  AND FURI_DATE_S        = " & SQ(FURI_DATE))
                        SQL2.Append("  AND SOUSIN_KBN_S       = '1'")
                        SQL2.Append("  AND HAISIN_FLG_S       = '1'")
                        SQL2.Append("  AND FUNOU_FLG_S        = '0'")
                        SQL2.Append("  AND TYUUDAN_FLG_S      = '0'")
                        '2017/03/28 FJH)森 CHG 標準版修正（潜在バグ修正）------------------------------------ START
                        SQL2.Append("  AND ITAKU_KANRI_CODE_T = " & SQ(tmp_ITAKU_KANRI_CODE_T))
                        'SQL2.Append("  AND ITAKU_KANRI_CODE_T = " & tmp_ITAKU_KANRI_CODE_T)
                        '2017/03/28 FJH)森 CHG 標準版修正（潜在バグ修正）------------------------------------ END
                        SQL2.Append(" ORDER BY")
                        SQL2.Append("      TORIS_CODE_T")
                        SQL2.Append("    , TORIF_CODE_T")

                        Dim tmp_TORIS_CODE_T As String = ""
                        Dim tmp_TORIF_CODE_T As String = ""

                        If OraReader2.DataReader(SQL2) = True Then
                            tmp_TORIS_CODE_T = OraReader2.Reader.Item("TORIS_CODE_T")
                            tmp_TORIF_CODE_T = OraReader2.Reader.Item("TORIF_CODE_T")
                        End If

                        'パラメータ(振替日,持込区分(企業持込),更新キー項目,取引先コード)
                        jobid = "J040"                      '..\Batch\結果更新\
                        para = FURI_DATE + ",1,0," + tmp_TORIS_CODE_T & tmp_TORIF_CODE_T

                        '--------------------------------
                        ' 同一JOB検索
                        '--------------------------------
                        Dim iRet As Integer
                        iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                        If iRet = 1 Then    'ジョブ登録済
                            MainDB.Rollback()
                            MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        ElseIf iRet = -1 Then 'ジョブ検索失敗
                            MainDB.Rollback()
                            MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブマスタ検索失敗 パラメータ:" & para)
                            Return
                        End If

                        '--------------------------------
                        ' JOB登録
                        '--------------------------------
                        If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then 'ジョブ登録失敗
                            MainDB.Rollback()
                            MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブ登録失敗 パラメータ:" & para)
                            Return
                        End If

                        OraReader.NextRead()
                    End While
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                    'MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MessageBox.Show("不能結果更新対象がが存在しません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    txtFuriDateY.Focus()
                    Exit Sub
                End If
            Else
                '--------------------------------
                ' JOB登録（取引先コード指定）
                '--------------------------------
                'パラメータ(振替日,持込区分(企業持込),更新キー項目,取引先コード)
                jobid = "J040"
                para = FURI_DATE + ",1,0," + strTORIS_CODE & strTORIF_CODE

                '--------------------------------
                ' 同一JOB検索
                '--------------------------------
                Dim iRet As Integer
                iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                If iRet = 1 Then    'ジョブ登録済
                    MainDB.Rollback()
                    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                ElseIf iRet = -1 Then 'ジョブ検索失敗
                    MainDB.Rollback()
                    MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブマスタ検索失敗 パラメータ:" & para)
                    Return
                End If

                '--------------------------------
                ' JOB登録
                '--------------------------------
                If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then 'ジョブ登録失敗
                    MainDB.Rollback()
                    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブ登録失敗 パラメータ:" & para)
                    Return
                End If
            End If
            ' 2016/04/22 タスク）綾部 CHG 【PG】UI_B-99-99(RSV2対応) -------------------- END

            MainDB.Commit()

            MessageBox.Show(String.Format(MSG0021I, "不能結果更新(企業持込)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            If MainDB IsNot Nothing Then
                MainDB.Rollback()
            End If
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try

    End Sub
#End Region
#Region " 終了"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 関数"
    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :更新ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/09
        'Update         :
        '============================================================================
        fn_check_text = False
        Try
            '取引先主コード必須チェック
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If
            '取引先副コード必須チェック
            If txtTorifCode.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorifCode.Focus()
                Return False
            End If
            '年必須チェック
            If txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
            '月必須チェック
            If txtFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then '(MSG0022W)
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '日付必須チェック
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
            fn_check_text = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try

    End Function
    Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_Table
        'Parameter      :
        'Description    :更新ボタンを押下時にマスタの相関チェックを実行
        'Return         :True=OK,False=NG
        'Create         :2009/09/10
        'Update         :
        '============================================================================
        fn_check_Table = False
        MainDB = New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try
            ' 2016/04/22 タスク）綾部 CHG 【PG】UI_B-99-99(RSV2対応) -------------------- START
            ''取引先情報取得
            'SQL.Append("SELECT * FROM TORIMAST")
            'SQL.Append(" WHERE TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
            'SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
            'If OraReader.DataReader(SQL) = True Then
            '    strSOUSIN_KBN = GCom.NzStr(OraReader.Reader.Item("SOUSIN_KBN_T"))
            '    OraReader.Close()
            'Else
            '    '取引先なし
            '    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    OraReader.Close()
            '    txtTorisCode.Focus()
            '    Return False
            'End If

            ''送信区分チェック
            'If strSOUSIN_KBN <> "1" Then '全銀方式
            '    MessageBox.Show(MSG0336W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    txtTorisCode.Focus()
            '    Return False
            'End If
            '--------------------------------
            ' 取引先情報取得
            '--------------------------------
            If Trim(txtTorisCode.Text) & Trim(txtTorifCode.Text) <> "888888888888" Then
                '取引先情報取得
                SQL.Append("SELECT * FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
                If OraReader.DataReader(SQL) = True Then
                    strSOUSIN_KBN = GCom.NzStr(OraReader.Reader.Item("SOUSIN_KBN_T"))
                    OraReader.Close()
                Else
                    '取引先なし
                    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    OraReader.Close()
                    txtTorisCode.Focus()
                    Return False
                End If

                '送信区分チェック
                If strSOUSIN_KBN <> "1" Then '全銀方式
                    MessageBox.Show(MSG0336W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorisCode.Focus()
                    Return False
                End If
            End If
            ' 2016/04/22 タスク）綾部 CHG 【PG】UI_B-99-99(RSV2対応) -------------------- END

            ' 2016/04/22 タスク）綾部 CHG 【PG】UI_B-99-99(RSV2対応) -------------------- START
            ''スケジュール情報取得
            'SQL = New StringBuilder(128)
            'SQL.Append("SELECT COUNT(*) COUNTER FROM TORIMAST,SCHMAST")
            'SQL.Append(" WHERE TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
            'SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
            'SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S ")
            'SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            'SQL.Append(" AND FURI_DATE_S = " & SQ(FURI_DATE))
            'SQL.Append(" AND SOUSIN_KBN_S = '1'")
            'SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            'SQL.Append(" AND HAISIN_FLG_S = '1'")
            '--------------------------------
            ' スケジュール情報取得
            '--------------------------------
            SQL = New StringBuilder(128)
            If Trim(txtTorisCode.Text) & Trim(txtTorifCode.Text) <> "888888888888" Then
                SQL.Append("SELECT")
                SQL.Append("      COUNT(*) COUNTER")
                SQL.Append(" FROM")
                SQL.Append("      TORIMAST")
                SQL.Append("    , SCHMAST")
                SQL.Append(" WHERE")
                SQL.Append("      TORIS_CODE_T  = " & SQ(Trim(txtTorisCode.Text)))
                SQL.Append("  AND TORIF_CODE_T  = " & SQ(Trim(txtTorifCode.Text)))
                SQL.Append("  AND TORIS_CODE_T  = TORIS_CODE_S ")
                SQL.Append("  AND TORIF_CODE_T  = TORIF_CODE_S ")
                SQL.Append("  AND FURI_DATE_S   = " & SQ(FURI_DATE))
                SQL.Append("  AND SOUSIN_KBN_S  = '1'")
                SQL.Append("  AND HAISIN_FLG_S  = '1'")
                SQL.Append("  AND TYUUDAN_FLG_S = '0'")
            Else
                SQL.Append("SELECT")
                SQL.Append("      COUNT(*) COUNTER")
                SQL.Append(" FROM")
                SQL.Append("      TORIMAST")
                SQL.Append("    , SCHMAST")
                SQL.Append(" WHERE")
                SQL.Append("      TORIS_CODE_T  = TORIS_CODE_S ")
                SQL.Append("  AND TORIF_CODE_T  = TORIF_CODE_S ")
                SQL.Append("  AND FURI_DATE_S   = " & SQ(FURI_DATE))
                SQL.Append("  AND SOUSIN_KBN_S  = '1'")
                SQL.Append("  AND HAISIN_FLG_S  = '1'")
                SQL.Append("  AND TYUUDAN_FLG_S = '0'")
            End If
            ' 2016/04/22 タスク）綾部 CHG 【PG】UI_B-99-99(RSV2対応) -------------------- END

            Dim Count As Integer
            If OraReader.DataReader(SQL) = True Then
                Count = GCom.NzInt(OraReader.Reader.Item("COUNTER"))
                OraReader.Close()
            Else
                '検索失敗(MSG0002E)
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                txtTorisCode.Focus()
                Return False
            End If

            If Count = 0 Then
                'スケジュールなし(MSG0068W)
                MessageBox.Show(MSG0068W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            txtTorisCode.Focus()
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If MainDB IsNot Nothing Then MainDB.Close()
        End Try
        fn_check_Table = True
    End Function
#End Region
#Region " イベント"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '選択カナで始まる委託者名を取得
        '--------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定
                Dim Jyoken As String = " AND SOUSIN_KBN_T = '1'"   '送信区分が全銀方式
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                cmbToriName.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コンボボックス設定)", "失敗", ex.ToString)
        End Try
    End Sub

    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        '-------------------------------------------
        '取引先コードテキストボックスに取引先コード設定
        '-------------------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コード取得)", "失敗", ex.ToString)
        End Try

    End Sub
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
             Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating, txtTorisCode.Validating, txtTorifCode.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub
#End Region

    '2010/12/24 信組対応 処理結果確認表印刷ボタン追加
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            If txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "999999999999" AndAlso txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "" Then
                '取引先主コード必須チェック
                If txtTorisCode.Text.Trim = "" Then
                    MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorisCode.Focus()
                    Exit Sub
                End If
                '取引先副コード必須チェック
                If txtTorifCode.Text.Trim = "" Then
                    MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorifCode.Focus()
                    Exit Sub
                End If
            End If
            '年必須チェック
            If txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Exit Sub
            End If
            '月必須チェック
            If txtFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Exit Sub
            End If
            '月範囲チェック
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then '(MSG0022W)
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Exit Sub
            End If
            '日付必須チェック
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Exit Sub
            End If
            '日付範囲チェック
            If GCom.NzInt(txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Exit Sub
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Exit Sub
            End If

            Dim strTORIS_CODE As String = txtTorisCode.Text
            Dim strTORIF_CODE As String = txtTorifCode.Text
            FURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            LW.ToriCode = strTORIS_CODE + strTORIF_CODE
            LW.FuriDate = FURI_DATE

            '--------------------------------
            'マスタチェック
            '--------------------------------
            MainDB = New CASTCommon.MyOracle
            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As New StringBuilder(128)

            Try
                If txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "999999999999" AndAlso txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "" Then
                    '取引先情報取得
                    SQL.Append("SELECT SOUSIN_KBN_T FROM TORIMAST")
                    SQL.Append(" WHERE TORIS_CODE_T = " & SQ(txtTorisCode.Text.Trim))
                    SQL.Append(" AND TORIF_CODE_T = " & SQ(txtTorifCode.Text.Trim))
                    If OraReader.DataReader(SQL) = True Then
                        strSOUSIN_KBN = GCom.NzStr(OraReader.Reader.Item("SOUSIN_KBN_T"))
                        OraReader.Close()
                    Else
                        '取引先なし
                        MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        OraReader.Close()
                        txtTorisCode.Focus()
                        Exit Sub
                    End If

                    '送信区分チェック
                    If strSOUSIN_KBN <> "1" Then '全銀方式
                        MessageBox.Show(MSG0336W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtTorisCode.Focus()
                        Exit Sub
                    End If
                End If

                'スケジュール情報取得
                SQL.Length = 0
                SQL.Append("SELECT COUNT(*) COUNTER FROM TORIMAST,SCHMAST")
                If txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "999999999999" AndAlso txtTorisCode.Text.Trim & txtTorifCode.Text.Trim <> "" Then
                    SQL.Append(" WHERE TORIS_CODE_T = " & SQ(txtTorisCode.Text.Trim))
                    SQL.Append(" AND TORIF_CODE_T = " & SQ(txtTorifCode.Text.Trim))
                    SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S ")
                    SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
                Else
                    SQL.Append(" WHERE TORIS_CODE_T = TORIS_CODE_S ")
                    SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
                End If
                SQL.Append(" AND FURI_DATE_S = " & SQ(FURI_DATE))
                SQL.Append(" AND SOUSIN_KBN_S = '1'")
                SQL.Append(" AND HAISIN_FLG_S = '1'")
                SQL.Append(" AND FUNOU_FLG_S = '1'")
                SQL.Append(" AND TYUUDAN_FLG_S = '0'")

                Dim Count As Integer
                If OraReader.DataReader(SQL) = True Then
                    Count = GCom.NzInt(OraReader.Reader.Item("COUNTER"))
                    OraReader.Close()
                Else
                    '検索失敗(MSG0002E)
                    MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    OraReader.Close()
                    txtTorisCode.Focus()
                    Exit Sub
                End If

                If Count = 0 Then
                    'スケジュールなし(MSG0056W)
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateY.Focus()
                    Exit Sub
                End If
            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
                txtTorisCode.Focus()
                Exit Sub
            Finally
                If OraReader IsNot Nothing Then OraReader.Close()
                If MainDB IsNot Nothing Then MainDB.Close()
            End Try

            Dim PrintName As String = "処理結果確認表(不能結果更新・企業持込)"

            If MessageBox.Show(String.Format(MSG0013I, PrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim param As String
            Dim nRet As Integer
            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            param = GCom.GetUserID & "," & strTORIS_CODE & "," & strTORIF_CODE & "," & FURI_DATE
            nRet = ExeRepo.ExecReport("KFJP013_2.EXE", param)

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

        End Try

    End Sub
End Class
