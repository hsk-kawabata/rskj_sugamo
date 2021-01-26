Imports System.Text
Imports CASTCommon

Public Class KFJNENK030

#Region "宣言"

    Private BatchLog As New CASTCommon.BatchLOG("KFJNENK030", "年金振込支店コード変更画面")
    Private Const MsgTitle As String = "年金振込支店コード変更(KFJNENK030)"
    Private Const ErrMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private LogToriCode As String = "0000000000-00"
    Private LogFuriDate As String = "00000000"

#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFJNENK030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            BatchLog.Write(LogToriCode, LogFuriDate, "(ロード)開始", "成功")

            '------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------
            GCom.GetUserID = gstrUSER_ID
            GCom.GetSysDate = String.Format("{0:yyyy年MM月dd日}", Date.Now)
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------
            '委託者名リストボックスの設定
            '------------------------------------------
            Dim Jyoken As String = " AND FMT_KBN_T = '03'"   '年金フォーマット
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            '------------------------------------------
            '画面初期表示
            '------------------------------------------
            If fn_GamenClear() = False Then
                Exit Sub
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(ロード)", "失敗", ex.Message)

        Finally
            BatchLog.Write(LogToriCode, LogFuriDate, "(ロード)終了", "成功")

        End Try
    End Sub

#End Region

#Region "ボタン"

    '更新ボタン
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        Try
            BatchLog.Write(LogToriCode, LogFuriDate, "(更新)開始", "成功")

            '------------------------------------------
            '入力チェック
            '------------------------------------------
            If txtSitCode.Text = "" Then
                MessageBox.Show(MSG0035W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '金融機関マスタチェック
            Dim Ret As Boolean
            Ret = GCom.CheckBankBranch(txtKinCode.Text, txtSitCode.Text)
            Select Case Ret
                Case 1, 2
                    '銀行無し
                    '支店無し
                    MessageBox.Show(String.Format(MSG0248W, txtKinCode.Text, txtSitCode.Text), MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
            End Select

            '------------------------------------------
            '確認メッセージ表示
            '------------------------------------------
            If MessageBox.Show(MSG0005I, MsgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            '------------------------------------------
            '年金マスタ更新
            '------------------------------------------
            If fn_upNENKINMAST() = False Then
                Exit Sub
            Else
                MessageBox.Show(MSG0006I, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            '------------------------------------------
            '画面初期表示
            '------------------------------------------
            If fn_GamenClear() = False Then
                Exit Sub
            End If

            cmbToriName.SelectedIndex = 0
            txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(更新)", "失敗", ex.Message)
        Finally
            BatchLog.Write(LogToriCode, LogFuriDate, "(更新)終了", "成功")
        End Try

    End Sub

    '参照ボタン
    Private Sub btnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelect.Click
        Try
            BatchLog.Write(LogToriCode, LogFuriDate, "(参照)開始", "成功")

            '------------------------------------------
            '入力チェック
            '------------------------------------------
            If fn_SelectCheckText() = False Then
                Exit Sub
            End If

            '------------------------------------------
            'ログ書込用変数を設定
            '------------------------------------------
            LogToriCode = txtTorisCode.Text & "-" & txtTorifCode.Text
            LogFuriDate = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            '------------------------------------------
            '取引先マスタチェック
            '------------------------------------------
            If fn_CheckTORIMAST() = False Then
                Exit Sub
            End If

            '------------------------------------------
            'スケジュールマスタチェック
            '------------------------------------------
            If fn_CheckSCHMAST() = False Then
                Exit Sub
            End If

            '------------------------------------------
            '年金マスタ取得／画面設定
            '------------------------------------------
            If fn_getNENKINMAST() = False Then
                Exit Sub
            End If

            '------------------------------------------
            '画面制御
            '------------------------------------------
            cmbKana.Enabled = False         'カナコンボボックス
            cmbToriName.Enabled = False     '取引先指定コンボボックス
            txtTorisCode.Enabled = False    '取引先主コード
            txtTorifCode.Enabled = False    '取引先副コード 
            txtFuriDateY.Enabled = False    '振替日(年)
            txtFuriDateM.Enabled = False    '振替日(月)
            txtFuriDateD.Enabled = False    '振替日(日)
            txtKinCode.Enabled = False      '金融機関コード
            cmbKamoku.Enabled = False       '科目
            txtKouzaBan.Enabled = False     '口座番号
            txtKingaku.Enabled = False      '振替金額
            txtSitCode.Enabled = True       '支店コード
            rdbSyousyoNo.Enabled = False    '年金証書番号指定ラジオボタン
            rdbKouza.Enabled = False        '口座番号指定ラジオボタン
            txtSyousyoNo.Enabled = False    '年金証書番号
            btnUpdate.Enabled = True        '更新ボタン
            btnSelect.Enabled = False       '参照ボタン

            '口座情報を表示する
            If rdbSyousyoNo.Checked = True Then
                txtKinCode.Visible = True
                txtSitCode.Visible = True
                cmbKamoku.Visible = True
                txtKouzaBan.Visible = True
                txtKingaku.Visible = True
                txtKeiyaku.Visible = True
                txtRecordNo.Visible = True
                txtSitKName.Visible = True
                Label11.Visible = True
                Label12.Visible = True
                Label13.Visible = True
                Label14.Visible = True
                Label16.Visible = True
                Label17.Visible = True
                Label15.Visible = True
                Label18.Visible = True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(参照)", "失敗", ex.Message)
        Finally
            BatchLog.Write(LogToriCode, LogFuriDate, "(参照)終了", "成功")

        End Try
    End Sub

    '取消ボタン
    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Try
            '------------------------------------------
            '確認メッセージ表示
            '------------------------------------------
            If MessageBox.Show(MSG0009I, MsgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            '------------------------------------------
            '画面初期化処理
            '------------------------------------------
            Call fn_GamenClear()

            cmbToriName.SelectedIndex = 0

            txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(取消)", "失敗", ex.Message)
        End Try
    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            BatchLog.Write(LogToriCode, LogFuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            BatchLog.Write(LogToriCode, LogFuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "ラジオボタン"

    '年金証書番号指定ラジオボタン
    Private Sub rdbSyousyoNo_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbSyousyoNo.CheckedChanged
        Try
            txtSyousyoNo.Visible = True
            txtKinCode.Visible = False
            cmbKamoku.Visible = False
            txtKouzaBan.Visible = False
            txtKingaku.Visible = False
            txtKeiyaku.Visible = False
            txtRecordNo.Visible = False
            txtSitCode.Visible = False
            txtSitKName.Visible = False
            Label11.Visible = False
            Label12.Visible = False
            Label13.Visible = False
            Label14.Visible = False
            Label16.Visible = False
            Label17.Visible = False
            Label15.Visible = False
            Label18.Visible = False
            btnUpdate.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(年金証書番号指定選択)", "失敗", ex.Message)
        End Try
    End Sub

    '口座番号指定ラジオボタン
    Private Sub rdbKouza_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbKouza.CheckedChanged
        Try
            txtSyousyoNo.Visible = False
            txtKinCode.Visible = True
            txtSitCode.Visible = True
            cmbKamoku.Visible = True
            txtKouzaBan.Visible = True
            txtKingaku.Visible = True
            txtKeiyaku.Visible = True
            txtRecordNo.Visible = True
            txtSitKName.Visible = True
            Label11.Visible = True
            Label12.Visible = True
            Label13.Visible = True
            Label14.Visible = True
            Label16.Visible = True
            Label17.Visible = True
            Label15.Visible = True
            Label18.Visible = True
            txtSitCode.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(口座番号指定選択)", "失敗", ex.Message)
        End Try

    End Sub

#End Region

#Region "テキスト"

    'テキストボックスゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtTorisCode.Validating, txtTorifCode.Validating, _
            txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating, _
            txtKinCode.Validating, txtKouzaBan.Validating, _
           txtRecordNo.Validating, txtSitCode.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(テキスト制御)", "失敗", ex.Message)
        End Try
    End Sub

#End Region

#Region "コンボボックス"

    '委託者名コンボボックスの設定
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            '------------------------------------------
            '選択カナで始まる委託者名を取得
            '------------------------------------------
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定
                Dim Jyoken As String = "  AND FMT_KBN_T = '03'"   '年金フォーマット
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                cmbToriName.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(委託者名リストボックス設定)", "失敗", ex.Message)

        End Try
    End Sub

    '取引先コードテキストボックスに取引先コード設定
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        Try
            '------------------------------------------
            '取引先コードを取得
            '------------------------------------------
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(取引先コード設定)", "失敗", ex.Message)

        End Try
    End Sub

#End Region

#Region "関数"

    Function fn_GamenClear() As Boolean
        '=====================================================================================
        'NAME           :fn_GamenClear
        'Parameter      :なし
        'Description    :各入力項目を初期化、ボタンの制御初期化
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/10/01
        'Update         :
        '=====================================================================================
        Try
            '------------------------------------------------
            'ログ書込用変数初期化
            '------------------------------------------------
            LogToriCode = "0000000000-00"   'ログ取引先コード
            LogFuriDate = "00000000"        'ログ振替日

            '------------------------------------------------
            '共通入力部分
            '------------------------------------------------
            cmbKana.SelectedIndex = -1      'カナコンボボックス
            txtTorisCode.Text = ""          '取引先主コード
            txtTorifCode.Text = ""          '取引先副コード
            txtFuriDateY.Text = ""          '振替日(年)
            txtFuriDateM.Text = ""          '振替日(月)
            txtFuriDateD.Text = ""          '振替日(日)
            rdbSyousyoNo.Checked = True     '年金証書番号指定にフォーカスを当てる

            txtSyousyoNo.Visible = True
            txtKinCode.Visible = False
            cmbKamoku.Visible = False
            txtKouzaBan.Visible = False
            txtKingaku.Visible = False
            txtKeiyaku.Visible = False
            txtRecordNo.Visible = False
            txtSitCode.Visible = False
            txtSitKName.Visible = False
            Label11.Visible = False
            Label12.Visible = False
            Label13.Visible = False
            Label14.Visible = False
            Label16.Visible = False
            Label17.Visible = False
            Label15.Visible = False
            Label18.Visible = False
            btnUpdate.Enabled = False

            '------------------------------------------------
            '年金証書番号指定部分
            '------------------------------------------------
            txtSyousyoNo.Text = ""          '年金証書番号

            '------------------------------------------------
            '口座番号指定区分
            '------------------------------------------------
            cmbKamoku.SelectedIndex = -1    '科目
            txtKinCode.Text = ""            '金融機関コード
            txtKouzaBan.Text = ""           '口座番号
            txtKingaku.Text = ""            '振替金額
            txtKeiyaku.Text = ""            '契約者名
            txtRecordNo.Text = ""           'レコード番号
            txtSitCode.Text = ""            '支店コード
            txtSitKName.Text = ""           '支店名カナ

            '------------------------------------------------
            '入力制御
            '------------------------------------------------
            cmbKana.Enabled = True         'カナコンボボックス
            cmbToriName.Enabled = True     '取引先指定コンボボックス
            txtTorisCode.Enabled = True    '取引先主コード
            txtTorifCode.Enabled = True    '取引先副コード 
            txtFuriDateY.Enabled = True    '振替日(年)
            txtFuriDateM.Enabled = True    '振替日(月)
            txtFuriDateD.Enabled = True    '振替日(日)
            txtKinCode.Enabled = True      '金融機関コード
            cmbKamoku.Enabled = True       '科目
            txtKouzaBan.Enabled = True     '口座番号
            txtKingaku.Enabled = True      '振替金額
            txtSitCode.Enabled = False       '支店コード
            rdbSyousyoNo.Enabled = True    '年金証書番号指定ラジオボタン
            rdbKouza.Enabled = True        '口座番号指定ラジオボタン
            txtSyousyoNo.Enabled = True    '年金証書番号

            '------------------------------------------------
            'ボタン制御
            '------------------------------------------------
            btnUpdate.Enabled = False       '更新ボタン
            btnSelect.Enabled = True        '参照ボタン
            btnClear.Enabled = True         '取消ボタン
            btnEnd.Enabled = True           '終了ボタン

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, MsgTitle, "画面初期化", "失敗", ex.Message)
            Return False
        End Try
    End Function

    Function fn_SelectCheckText() As Boolean
        '=====================================================================================
        'NAME           :fn_SelectCheckText
        'Parameter      :なし
        'Description    :入力項目のチェック処理(参照処理時)
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/10/01
        'Update         :
        '=====================================================================================
        Try
            '------------------------------------------
            '共通チェック項目
            '------------------------------------------
            '取引先主コード
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If

            '取引先副コード
            If txtTorifCode.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorifCode.Focus()
                Return False
            End If

            '振替日(年)
            If txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            '振替日(月)
            If txtFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            ElseIf CInt(txtFuriDateM.Text) < 1 Or CInt(txtFuriDateM.Text) > 12 Then
                MessageBox.Show(MSG0022W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            '振替日(日)
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            ElseIf CInt(txtFuriDateD.Text) < 1 Or CInt(txtFuriDateD.Text) > 31 Then
                MessageBox.Show(MSG0025W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '------------------------------------------
            '年金証書番号指定選択時
            '------------------------------------------
            If rdbSyousyoNo.Checked = True Then
                '年金証書番号
                If txtSyousyoNo.Text.Trim = "" Then
                    MessageBox.Show(MSG0145W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSyousyoNo.Focus()
                    Return False
                End If

                '年金証書番号
                If txtSyousyoNo.Text.Length < 14 Then
                    MessageBox.Show(MSG0357W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSyousyoNo.Focus()
                    Return False
                End If
            End If

            '------------------------------------------
            '口座番号指定選択時
            '------------------------------------------
            If rdbKouza.Checked = True Then
                '金融機関コード
                If txtKinCode.Text.Trim = "" Then
                    MessageBox.Show(MSG0032W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKinCode.Focus()
                    Return False
                End If

                '科目
                If cmbKamoku.SelectedIndex = -1 Then
                    MessageBox.Show(MSG0136W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    cmbKamoku.Focus()
                    Return False
                End If

                '口座番号
                If txtKouzaBan.Text.Trim = "" Then
                    MessageBox.Show(MSG0138W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKouzaBan.Focus()
                    Return False
                End If

                '振替金額
                If txtKingaku.Text.Trim = "" Then
                    MessageBox.Show(MSG0140W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKingaku.Focus()
                    Return False
                End If

            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(参照)", "失敗", ex.Message)
            Return False
        End Try
    End Function

    Function fn_CheckTORIMAST() As Boolean
        '=====================================================================================
        'NAME           :fn_CheckTORIMAST
        'Parameter      :なし
        'Description    :取引先のチェック処理
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/10/01
        'Update         :
        '=====================================================================================
        Dim SQL As New StringBuilder(128)

        Try
            SQL.Append("SELECT FMT_KBN_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = '" & txtTorisCode.Text & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & txtTorifCode.Text & "'")

            gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
            gdbcCONNECT.Open()

            gdbCOMMAND = New OracleClient.OracleCommand
            gdbCOMMAND.CommandText = SQL.ToString
            gdbCOMMAND.Connection = gdbcCONNECT

            gdbrREADER = gdbCOMMAND.ExecuteReader

            '------------------------------------------
            'フォーマット区分チェック
            '------------------------------------------
            If gdbrREADER.Read = False Then
                MessageBox.Show(MSG0063W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            Else
                If gdbrREADER.Item(0) <> "03" Then
                    MessageBox.Show(MSG0270W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorisCode.Focus()
                    Return False
                End If
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(取引先チェック)", "失敗", ex.Message)
            Return False

        Finally
            If gdbcCONNECT IsNot Nothing Then
                gdbcCONNECT.Close()
            End If
        End Try
    End Function

    Function fn_CheckSCHMAST() As Boolean
        '=====================================================================================
        'NAME           :fn_CheckSCHMAST
        'Parameter      :なし
        'Description    :取引先のチェック処理
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/10/01
        'Update         :
        '=====================================================================================
        Dim SQL As New StringBuilder(128)

        Try
            SQL.Append("SELECT COUNT(*) AS COUNT")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_S = '" & txtTorisCode.Text & "'")
            SQL.Append(" AND TORIF_CODE_S = '" & txtTorifCode.Text & "'")
            SQL.Append(" AND FURI_DATE_S = '" & txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text & "'")

            gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
            gdbcCONNECT.Open()

            gdbCOMMAND = New OracleClient.OracleCommand
            gdbCOMMAND.CommandText = SQL.ToString
            gdbCOMMAND.Connection = gdbcCONNECT

            gdbrREADER = gdbCOMMAND.ExecuteReader

            '------------------------------------------
            '登録チェック
            '------------------------------------------
            If gdbrREADER.Read = False OrElse gdbrREADER.Item(0) < 1 Then
                MessageBox.Show(MSG0095W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(スケジュールチェック)", "失敗", ex.Message)
            Return False

        Finally
            If gdbcCONNECT IsNot Nothing Then
                gdbcCONNECT.Close()
            End If
        End Try
    End Function

    Function fn_getNENKINMAST() As Boolean
        '=====================================================================================
        'NAME           :fn_getNENKINMAST
        'Parameter      :なし
        'Description    :年金マスタの参照／画面セット
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/10/01
        'Update         :
        '=====================================================================================
        Dim SQL As New StringBuilder(128)
        Dim strKamoku As String = "00"

        Try
            '------------------------------------------
            '科目コード取得
            '------------------------------------------
            strKamoku = fn_KamokuYomikae(CStr(cmbKamoku.SelectedIndex))

            '------------------------------------------
            '年金マスタ取得
            '------------------------------------------
            SQL.Append("SELECT KEIYAKU_KNAME_K")
            SQL.Append(", KEIYAKU_KIN_K")
            SQL.Append(", KEIYAKU_SIT_K")
            SQL.Append(", KEIYAKU_KAMOKU_K")
            SQL.Append(", KEIYAKU_KOUZA_K")
            SQL.Append(", FURIKIN_K")
            SQL.Append(", YOBI1_K")
            SQL.Append(", RECORD_NO_K")
            SQL.Append(" FROM NENKINMAST")
            SQL.Append(" WHERE TORIS_CODE_K = '" & txtTorisCode.Text & "'")
            SQL.Append(" AND TORIF_CODE_K = '" & txtTorifCode.Text & "'")
            SQL.Append(" AND FURI_DATE_K = '" & txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text & "'")

            '年金証書番号指定時
            If rdbSyousyoNo.Checked = True Then
                SQL.Append(" AND YOBI3_K = '" & txtSyousyoNo.Text & "'")
            End If

            '口座番号指定時
            If rdbKouza.Checked = True Then
                SQL.Append(" AND KEIYAKU_KIN_K = '" & txtKinCode.Text & "'")
                SQL.Append(" AND KEIYAKU_KAMOKU_K = '" & strKamoku & "'")
                SQL.Append(" AND KEIYAKU_KOUZA_K = '" & txtKouzaBan.Text & "'")
                SQL.Append(" AND FURIKIN_K = '" & txtKingaku.Text & "'")
            End If

            gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
            gdbcCONNECT.Open()

            gdbCOMMAND = New OracleClient.OracleCommand
            gdbCOMMAND.CommandText = SQL.ToString
            gdbCOMMAND.Connection = gdbcCONNECT

            gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

            '------------------------------------------
            '登録チェック
            '------------------------------------------
            If gdbrREADER.Read = False Then
                MessageBox.Show(MSG0273W, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                If rdbSyousyoNo.Checked = True Then
                    txtSyousyoNo.Focus()
                Else
                    txtKinCode.Focus()
                End If
                Return False
            End If

            '------------------------------------------
            'データ設定
            '------------------------------------------
            txtKeiyaku.Text = gdbrREADER.Item("KEIYAKU_KNAME_K")
            txtRecordNo.Text = gdbrREADER.Item("RECORD_NO_K")
            txtSitCode.Text = gdbrREADER.Item("KEIYAKU_SIT_K")
            txtSitKName.Text = gdbrREADER.Item("YOBI1_K")

            '年金証書番号指定時
            If rdbSyousyoNo.Checked = True Then
                txtKinCode.Text = gdbrREADER.Item("KEIYAKU_KIN_K")
                cmbKamoku.SelectedIndex = CInt(fn_KamokuYomikae(gdbrREADER.Item("KEIYAKU_KAMOKU_K")))
                txtKouzaBan.Text = gdbrREADER.Item("KEIYAKU_KOUZA_K")
                txtKingaku.Text = gdbrREADER.Item("FURIKIN_K")
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(スケジュールチェック)", "失敗", ex.Message)
            Return False

        Finally
            If gdbcCONNECT IsNot Nothing Then
                gdbcCONNECT.Close()
            End If
        End Try
    End Function

    Function fn_upNENKINMAST() As Boolean
        '=====================================================================================
        'NAME           :fn_upNENKINMAST
        'Parameter      :なし
        'Description    :年金マスタの更新
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/10/01
        'Update         :
        '=====================================================================================
        Dim SQL As New StringBuilder(128)
        Dim SQLCode As Integer
        Dim Ret As Integer
        Dim strKamoku As String

        Try
            strKamoku = fn_KamokuYomikae(CStr(cmbKamoku.SelectedIndex))

            SQL.Append("UPDATE NENKINMAST SET")
            SQL.Append(" KEIYAKU_SIT_K = '" & txtSitCode.Text & "'")
            SQL.Append(" ,FURIKETU_CODE_K = 0")
            SQL.Append(" ,FURIKETU_CENTERCODE_K = 0")
            SQL.Append(" WHERE TORIS_CODE_K = '" & txtTorisCode.Text & "'")
            SQL.Append(" AND TORIF_CODE_K = '" & txtTorifCode.Text & "'")
            SQL.Append(" AND FURI_DATE_K = '" & txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text & "'")
            SQL.Append(" AND KEIYAKU_KIN_K = '" & txtKinCode.Text & "'")
            SQL.Append(" AND KEIYAKU_KAMOKU_K = '" & strKamoku & "'")
            SQL.Append(" AND KEIYAKU_KOUZA_K = '" & txtKouzaBan.Text & "'")
            SQL.Append(" AND FURIKIN_K = '" & txtKingaku.Text & "'")
            SQL.Append(" AND KEIYAKU_KNAME_K = '" & txtKeiyaku.Text & "'")
            SQL.Append(" AND RECORD_NO_K = '" & txtRecordNo.Text & "'")
            SQL.Append(" AND YOBI1_K = '" & txtSitKName.Text & "'")

            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode, True)

            If Ret = 1 AndAlso SQLCode = 0 Then

                SQL = New StringBuilder(128)
                SQL.Append("UPDATE MEIMAST SET")
                SQL.Append(" KEIYAKU_SIT_K = '" & txtSitCode.Text & "'")
                SQL.Append(" ,FURIKETU_CODE_K = 0")
                SQL.Append(" ,FURIKETU_CENTERCODE_K = 0")
                SQL.Append(" WHERE TORIS_CODE_K = '" & txtTorisCode.Text & "'")
                SQL.Append(" AND TORIF_CODE_K = '" & txtTorifCode.Text & "'")
                SQL.Append(" AND FURI_DATE_K = '" & txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text & "'")
                SQL.Append(" AND KEIYAKU_KIN_K = '" & txtKinCode.Text & "'")
                SQL.Append(" AND KEIYAKU_KAMOKU_K = '" & strKamoku & "'")
                SQL.Append(" AND KEIYAKU_KOUZA_K = '" & txtKouzaBan.Text & "'")
                SQL.Append(" AND FURIKIN_K = '" & txtKingaku.Text & "'")
                SQL.Append(" AND KEIYAKU_KNAME_K = '" & txtKeiyaku.Text & "'")
                SQL.Append(" AND RECORD_NO_K = '" & txtRecordNo.Text & "'")
                SQL.Append(" AND YOBI1_K = '" & txtSitKName.Text & "'")

                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL.ToString, SQLCode, True)

                If Ret = 1 AndAlso SQLCode = 0 Then

                    Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, "COMMIT WORK", SQLCode, False)
                    Return True

                Else

                    MessageBox.Show(MSG0002E.Replace("{0}", "更新"), MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False

                End If

            Else
                MessageBox.Show(MSG0002E.Replace("{0}", "更新"), MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(年金マスタ更新)", "失敗", ex.Message)
            Return False

        Finally
            If gdbcCONNECT IsNot Nothing Then
                gdbcCONNECT.Close()
            End If
        End Try
    End Function

    Function fn_KamokuYomikae(ByVal Kamoku As String) As String
        '=====================================================================================
        'NAME           :fn_KamokuYomikae
        'Parameter      :科目コード／科目コンボボックスのインデックス
        'Description    :科目コード⇔インデックスの読替
        'Return         :読替後科目コード・インデックス
        'Create         :2009/10/01
        'Update         :
        '=====================================================================================
        Try
            Select Case Kamoku.Length
                Case 1
                    'インデックス⇒科目コード
                    Select Case cmbKamoku.SelectedText
                        Case "普通"     '普通
                            Return "02"
                        Case "当座"     '当座
                            Return "01"
                        Case "納税"     '納税
                            Return "05"
                        Case "職員"     '職員
                            Return "37"
                        Case Else       'その他
                            Return "02"
                    End Select

                Case 2
                    '科目コード⇒インデックス
                    Select Case Kamoku
                        Case "02"       '普通
                            Return "0"
                        Case "01"       '当座
                            Return "1"
                        Case "05"       '納税
                            Return "2"
                        Case "37"       '職員
                            Return "3"
                        Case Else       'その他
                            Return "4"
                    End Select

                Case Else
                    MessageBox.Show(MSG0272W.Replace("{0}", Kamoku), MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return ""
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, MsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(LogToriCode, LogFuriDate, "(科目設定)", "失敗", ex.Message)
            Return False
        End Try
    End Function
#End Region

End Class
