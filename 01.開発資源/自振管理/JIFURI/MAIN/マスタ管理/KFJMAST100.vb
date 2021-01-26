Imports System.Data.OracleClient
Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon

Public Class KFJMAST100
    Inherits System.Windows.Forms.Form

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAST100", "手数料徴求フラグ一括更新画面")
    Private Const msgTitle As String = "手数料徴求フラグ一括更新(KFJMAST100)"
    Private Const errMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

#Region " ロード"

    Private Sub KFJMAST100_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '------------------------------------------------
            'ログの書込に必要な情報の取得
            '------------------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '------------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Me.Label2, Me.Label3, Me.lbluser, Me.lblDate)

            '------------------------------------------------
            '取引先コンボボックス設定
            '------------------------------------------------
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            Application.DoEvents()
            Me.txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
            Me.Close()
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "表示"

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim startFuriDate As String = String.Concat(New String() {txtStartFuriDateY.Text, txtStartFuriDateM.Text, txtStartFuriDateD.Text})
        Dim endFuriDate As String = String.Concat(New String() {txtEndFuriDateY.Text, txtEndFuriDateM.Text, txtEndFuriDateD.Text})

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(表示)開始", "成功", "")
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If

            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            Dim KFJMAST101 As New KFJMAST101

            KFJMAST101.startFuriDate = startFuriDate
            KFJMAST101.endFuriDate = endFuriDate
            KFJMAST101.TorisCode = txtTorisCode.Text
            KFJMAST101.TorifCode = txtTorifCode.Text

            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST101, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(表示)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(表示)終了", "成功", "")
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

    Private Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :表示ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :
        'Update         :
        '============================================================================

        Try
            '-------------------------------------------------------
            ' 取引先主コード(開始)必須チェック
            ' ※主コードのみを許可するため副のチェックは行わない
            '-------------------------------------------------------
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If

            '-------------------------------------------------------
            ' 振替日（開始）のチェック
            '-------------------------------------------------------
            '年必須チェック
            If txtStartFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtStartFuriDateY.Focus()
                Return False
            End If

            '月必須チェック
            If txtStartFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtStartFuriDateM.Focus()
                Return False
            End If

            '月範囲チェック
            If GCom.NzInt(txtStartFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtStartFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtStartFuriDateM.Focus()
                Return False
            End If

            '日付必須チェック
            If txtStartFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtStartFuriDateD.Focus()
                Return False
            End If

            '日付範囲チェック
            If GCom.NzInt(txtStartFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtStartFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtStartFuriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtStartFuriDateY.Text & "/" & txtStartFuriDateM.Text & "/" & txtStartFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtStartFuriDateY.Focus()
                Return False
            End If

            '-------------------------------------------------------
            ' 振替日（開始）のチェック
            '-------------------------------------------------------
            '年必須チェック
            If txtEndFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtEndFuriDateY.Focus()
                Return False
            End If

            '月必須チェック
            If txtEndFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtEndFuriDateM.Focus()
                Return False
            End If

            '月範囲チェック
            If GCom.NzInt(txtEndFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtEndFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtEndFuriDateM.Focus()
                Return False
            End If

            '日付必須チェック
            If txtEndFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtEndFuriDateD.Focus()
                Return False
            End If

            '日付範囲チェック
            If GCom.NzInt(txtEndFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtEndFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtEndFuriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            WORK_DATE = txtEndFuriDateY.Text & "/" & txtEndFuriDateM.Text & "/" & txtEndFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtEndFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try

    End Function

    Private Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :表示ボタンを押下時にマスタ相関チェック
        'Return         :True=OK,False=NG
        'Create         :
        'Update         :
        '============================================================================
        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Dim startFuriDate As String = String.Concat(New String() {txtStartFuriDateY.Text, txtStartFuriDateM.Text, txtStartFuriDateD.Text})
        Dim endFuriDate As String = String.Concat(New String() {txtEndFuriDateY.Text, txtEndFuriDateM.Text, txtEndFuriDateD.Text})

        Try
            Dim lngDataCNT As Long = 0

            SQL.Append("SELECT ")
            SQL.Append("     COUNT(*) COUNTER")
            SQL.Append(" FROM ")
            SQL.Append("     TORIMAST,SCHMAST")
            SQL.Append(" WHERE ")
            SQL.Append("     TORIS_CODE_T       = TORIS_CODE_S ")
            SQL.Append(" AND TORIF_CODE_T       = TORIF_CODE_S ")
            SQL.Append(" AND TORIS_CODE_T       =" & SQ(txtTorisCode.Text))
            If txtTorifCode.Text.Trim.Length = 2 Then
                SQL.Append(" AND TORIF_CODE_T   =" & SQ(txtTorifCode.Text))
            End If
            SQL.Append(" AND FURI_DATE_S       >= " & SQ(startFuriDate))
            SQL.Append(" AND FURI_DATE_S       <= " & SQ(endFuriDate))
            SQL.Append(" AND TYUUDAN_FLG_S      = '0'")
            SQL.Append(" AND KESSAI_FLG_S       = '1'")
            SQL.Append(" AND TESUUTYO_FLG_S     = '0'")
            SQL.Append(" AND KESSAI_KBN_T      <> '99'")

            If OraReader.DataReader(SQL) = True Then
                lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function

#End Region

#Region " イベント"

    '------------------------------------------------
    ' ゼロパディング
    '------------------------------------------------
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
             Handles txtTorisCode.Validating, txtTorifCode.Validating, txtStartFuriDateY.Validating, txtStartFuriDateM.Validating, txtStartFuriDateD.Validating _
             , txtEndFuriDateY.Validating, txtEndFuriDateM.Validating, txtEndFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception

        End Try
    End Sub

#End Region

    'カナコンボボックス設定変更時再読み込み
    Private Sub cmbKana_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            '取引先コードがReadOnlyの場合は処理終了
            If txtTorisCode.ReadOnly Then
                Exit Sub
            End If
            '取引先コンボボックス設定
            If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Catch ex As Exception

        End Try
    End Sub
    '取引先コード設定
    Private Sub cmbToriName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbToriName.SelectedIndexChanged
        Try
            '取引先コードがReadOnlyの場合は処理終了
            If txtTorisCode.ReadOnly Then
                Exit Sub
            End If
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
            End If
        Catch ex As Exception

        End Try

    End Sub
End Class