Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT020

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT020", "資金決済企業一覧表印刷画面")
    Private Const msgTitle As String = "資金決済企業一覧表印刷画面(KFKPRNT020)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    '2017/05/16 タスク）西野 ADD 標準版修正（決済予定日での出力対応）----------------- START
    Private PRINT_MODE As String = CASTCommon.GetRSKJIni("PRINT", "KFKP005_MODE")   '0:予定帳票、0以外:結果帳票
    '2017/05/16 タスク）西野 ADD 標準版修正（決済予定日での出力対応）----------------- END

    Private Sub KFKPRNT020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            '画面表示時、決済日にシステム日付を表示
            '------------------------------------------------
            Dim strSysDate As String

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtKessaiDateY.Text = strSysDate.Substring(0, 4)
            txtKessaiDateM.Text = strSysDate.Substring(4, 2)
            txtKessaiDateD.Text = strSysDate.Substring(6, 2)

            ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            ''------------------------------------------------
            ''印刷区分を通常に設定
            ''------------------------------------------------
            'cmbPrintKbn.SelectedIndex = 0
            'コンボボックス設定値をテキストに設定する
            Select Case GCom.SetComboBox(Me.cmbPrintKbn, "KFKPRNT020_印刷区分.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "印刷区分", "KFKPRNT020_印刷区分.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", "KFKPRNT020_印刷区分.TXTなし")
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "印刷区分"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", "印刷区分コンボボックス設定異常")
            End Select
            ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    '印刷ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            '------------------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------------------
            If fn_check_TEXT() = False Then Exit Sub

            '決済日の取得
            Dim strKESSAI_DATE As String
            strKESSAI_DATE = txtKessaiDateY.Text & txtKessaiDateM.Text & txtKessaiDateD.Text

            '------------------------------------------------
            '印刷対象先の件数チェック
            '------------------------------------------------
            Dim lngDataCNT As Long = 0
            Dim strKbn As String = ""

            ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            'Select Case cmbPrintKbn.Text
            '    Case "通常"
            '        strKbn = "1"
            '    Case "しんきん中金分"
            '        strKbn = "2"
            '    Case "しんきん中金分以外"
            '        strKbn = "3"
            '    Case Else
            '        MessageBox.Show(MSG0232W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtKessaiDateY.Focus()
            '        Exit Sub
            'End Select
            'コンボボックス設定値をテキストに設定する
            strKbn = GCom.GetComboBox(Me.cmbPrintKbn)
            ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

            If fn_Select_Data(strKESSAI_DATE, strKbn, lngDataCNT) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtKessaiDateY.Focus()
                Exit Sub
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(MSG0013I.Replace("{0}", "資金決済企業一覧表"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            '------------------------------------------------
            ' 資金決済企業一覧表印刷バッチの呼び出し
            ' 引数：決済日
            ' 戻り値:1:正常　-1:件数0件　以外:エラー
            '------------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            Dim Param As String = GCom.GetUserID & "," & strKESSAI_DATE & "," & strKbn
            Dim nRet As Integer = ExeRepo.ExecReport("KFKP005.EXE", Param)

            If nRet = 0 Then
                '正常
                MessageBox.Show(MSG0014I.Replace("{0}", "資金決済企業一覧表"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf nRet = -1 Then
                '対象0件
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '異常　レポエージェントエラーコードを受けたい
                MessageBox.Show(MSG0004E.Replace("{0}", "資金決済企業一覧表"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.Message)
        Finally
            'btnEnd.Focus()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try

    End Sub

    '終了ボタン
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

#Region "KFKMAIN020用関数"

    Private Function fn_check_TEXT() As Boolean
        '============================================================================
        'NAME           :fn_check_TEXT
        'Parameter      :
        'Description    :印刷ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :
        'Update         :
        '============================================================================

        Try

            '------------------------------------------------
            '決済年チェック
            '------------------------------------------------
            '必須チェック
            If txtKessaiDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '決済月チェック
            '------------------------------------------------
            '必須チェック
            If txtKessaiDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtKessaiDateM.Text >= 1 And txtKessaiDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '決済日チェック
            '------------------------------------------------
            '必須チェック
            If txtKessaiDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtKessaiDateD.Text >= 1 And txtKessaiDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = txtKessaiDateY.Text & "/" & txtKessaiDateM.Text & "/" & txtKessaiDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
        End Try

    End Function

    Public Function fn_Select_Data(ByVal astrKESSAI_DATE As String, ByVal astrMode As String, ByRef alngDataCNT As Long) As Boolean
        '    '============================================================================
        '    'NAME           :fn_Select_Data
        '    'Parameter      :
        '    'Description    :印刷対象先があるかどうか
        '    'Return         :True=OK(対象あり),False=NG(対象なし)
        '    'Create         :
        '    'Update         :
        '    '============================================================================

        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try

            alngDataCNT = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(",TORIMAST")
            SQL.Append(" WHERE")
            SQL.Append(" SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
            SQL.Append(" AND SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
            '2009/09/23 資金決済済みデータを抽出するよう修正
            'SQL.Append(" AND (TESUU_DATE_S = '" & astrKESSAI_DATE & "'")
            'SQL.Append("  OR  KESSAI_DATE_S =  '" & astrKESSAI_DATE & "'")
            'SQL.Append("  OR  (TESUUTYO_FLG_S = '0'")
            'SQL.Append("   AND TESUU_YDATE_S <= '" & astrKESSAI_DATE & "'))")
            'SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
            'SQL.Append(" AND SCHMAST.FUNOU_FLG_S = '1'")
            '2017/05/16 タスク）西野 CHG 標準版修正（決済予定日での出力対応）----------------- START
            If PRINT_MODE = "0" Then
                '予定帳票
                SQL.Append(" AND SCHMAST.KESSAI_YDATE_S = '" & astrKESSAI_DATE & "'")
                SQL.Append(" AND SCHMAST.TESUUKEI_FLG_S = '1'")
            Else
                '結果帳票
                SQL.Append(" AND SCHMAST.KESSAI_DATE_S = '" & astrKESSAI_DATE & "'")
                SQL.Append(" AND SCHMAST.KESSAI_FLG_S = '1'")
            End If
            '2017/05/16 タスク）西野 CHG 標準版修正（決済予定日での出力対応）----------------- END
            SQL.Append(" AND SCHMAST.TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND SCHMAST.SYORI_KIN_S > 0")

            Select Case astrMode
                Case "1"    '通常→決済対象外以外
                    SQL.Append(" AND KESSAI_KBN_T <> '99'")
                Case "2"    '信金中金分
                    SQL.Append(" AND KESSAI_KBN_T = '00'")
                Case "3"    'しんきん中金以外
                    SQL.Append(" AND KESSAI_KBN_T NOT IN('00','99')")
            End Select

            If OraReader.DataReader(SQL) = True Then
                alngDataCNT = OraReader.GetInt64("COUNTER")
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(データ検索)", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function

    'ゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKessaiDateY.Validating, txtKessaiDateM.Validating, txtKessaiDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)

    End Sub

#End Region

End Class