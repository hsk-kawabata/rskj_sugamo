Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJPRNT020
    Private MainLOG As New CASTCommon.BatchLOG("KFJPRNT020", "口座振替明細表印刷画面")
    Private Const msgTitle As String = "口座振替明細表印刷画面(KFJPRNT020)"
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
    Private strFURI_DATE As String
#Region " ロード"
    Private Sub KFJPRNT020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '#####################################
        'ログの書込に必要な情報の取得
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label2, Label4, lblUser, lblDate)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時に処理日にシステム日付を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtFuriDateY.Text = strSysDate.Substring(0, 4)
            txtFuriDateM.Text = strSysDate.Substring(4, 2)
            txtFuriDateD.Text = strSysDate.Substring(6, 2)

            'コントロール制御
            btnAction.Enabled = False
            btnCancel.Enabled = False

        Catch ex As Exception
            '   MessageBox.Show(ex.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '例外が発生しました。ログを確認のうえ、保守要員に連絡してください。(ID:MSG0006E)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 検索"
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        '=====================================================================================
        'NAME           :btnSearch_Click
        'Parameter      :
        'Description    :検索ボタン
        'Return         :
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Try
            Dim SQL As New StringBuilder(128)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)開始", "成功", "")
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            'コントロール制御
            cmbTimeStamp.SelectedIndex = 0
            txtFuriDateY.Enabled = False
            txtFuriDateM.Enabled = False
            txtFuriDateD.Enabled = False
            txtTORIS.Enabled = False
            txtTORIF.Enabled = False
            btnSearch.Enabled = False
            btnAction.Enabled = True
            btnCancel.Enabled = True
            cmbTimeStamp.Focus()
        Catch ex As Exception
            '   MessageBox.Show(errMsg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '例外が発生しました。ログを確認のうえ、保守要員に連絡してください。(ID:MSG0006E)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 取消"
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        '=====================================================================================
        'NAME           :btnSearch_Click
        'Parameter      :
        'Description    :取消ボタン
        'Return         :
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")
            'コントロール制御
            cmbTimeStamp.Items.Clear()  'タイムスタンプ初期化
            txtFuriDateY.Enabled = True
            txtFuriDateM.Enabled = True
            txtFuriDateD.Enabled = True
            txtTORIS.Enabled = True
            txtTORIF.Enabled = True
            btnSearch.Enabled = True
            btnAction.Enabled = False
            btnCancel.Enabled = False
            txtFuriDateY.Focus()
        Catch ex As Exception
            '   MessageBox.Show(errMsg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '例外が発生しました。ログを確認のうえ、保守要員に連絡してください。(ID:MSG0006E)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 印刷"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :印刷ボタン
        'Return         :
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            Dim strTIME_STAMP As String
            strTIME_STAMP = cmbTimeStamp.SelectedItem
            strTIME_STAMP = strTIME_STAMP.Substring(0, 4) & strTIME_STAMP.Substring(5, 2) & strTIME_STAMP.Substring(8, 2) _
                          & strTIME_STAMP.Substring(11, 2) & strTIME_STAMP.Substring(14, 2) & strTIME_STAMP.Substring(17, 2)

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "口座振替明細表"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            Dim param As String
            Dim nRet As Integer

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：ログイン名、ＣＳＶファイル名
            If txtTORIS.Text.Trim + txtTORIF.Text.Trim <> "" Then
                param = GCom.GetUserID & "," & txtTORIS.Text.Trim + txtTORIF.Text.Trim & "," & strTIME_STAMP
            Else
                param = GCom.GetUserID & ",000000000000," & strTIME_STAMP
            End If


            nRet = ExeRepo.ExecReport("KFJP010.EXE", param)

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, "口座振替明細表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "口座振替明細表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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
    Public Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        Try
            fn_check_text = False
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

            '取引先主コード入力チェック
            If txtTORIF.Text.Trim <> "" AndAlso txtTORIS.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTORIS.Focus()
                Return False
            End If

            '取引先副コード入力チェック
            If txtTORIS.Text.Trim <> "" AndAlso txtTORIF.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTORIF.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        Finally

        End Try
        fn_check_text = True
    End Function
    Private Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :印刷ボタンを押下時にマスタ相関チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        fn_check_Table = False
        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        fn_check_Table = False
        Try

            '取引先コードチェック
            If txtTORIS.Text.Trim + txtTORIF.Text.Trim <> "" Then
                Dim SOUSIN_KBN As String
                SQL.Append("SELECT *")
                SQL.Append(" FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T =" & SQ(txtTORIS.Text))
                SQL.Append(" AND TORIF_CODE_T =" & SQ(txtTORIF.Text))
                If OraReader.DataReader(SQL) = True Then
                    SOUSIN_KBN = GCom.NzLong(OraReader.GetString("SOUSIN_KBN_T"))
                    OraReader.Close()
                Else
                    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTORIS.Focus()
                    Return False
                End If
                If SOUSIN_KBN <> 0 Then
                    MessageBox.Show(MSG0250W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTORIS.Focus()
                    Return False
                End If
            End If


            'スケジュールチェック+コンボボックス設定
            Dim strTIME_STAMP_START As String
            Dim strTIME_STAMP_END As String
            Dim strTIME_STAMP As String
            Dim intCOUNT As Integer

            cmbTimeStamp.Items.Clear()
            cmbTimeStamp.Text = ""

            strTIME_STAMP_START = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text & "000000"
            strTIME_STAMP_END = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text & "999999"
            SQL = New StringBuilder(128)
            SQL.Append(" SELECT DISTINCT JIFURI_TIME_STAMP_S")
            SQL.Append(" FROM SCHMAST,TORIMAST")
            SQL.Append(" WHERE JIFURI_TIME_STAMP_S BETWEEN" & SQ(strTIME_STAMP_START) & "")
            SQL.Append(" AND" & SQ(strTIME_STAMP_END))
            SQL.Append(" AND HAISIN_FLG_S = '1'")
            SQL.Append(" AND SOUSIN_KBN_S = '0'")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            If txtTORIS.Text.Trim <> "" OrElse txtTORIF.Text.Trim <> "" Then
                SQL.Append(" AND TORIS_CODE_S = " & SQ(txtTORIS.Text))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(txtTORIF.Text))
            End If

            intCOUNT = 0
            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    strTIME_STAMP = OraReader.GetString("JIFURI_TIME_STAMP_S")
                    cmbTimeStamp.Items.Add(strTIME_STAMP.Substring(0, 4) & "/" & strTIME_STAMP.Substring(4, 2) & "/" & _
                                           strTIME_STAMP.Substring(6, 2) & " " & strTIME_STAMP.Substring(8, 2) & ":" & _
                                           strTIME_STAMP.Substring(10, 2) & ":" & strTIME_STAMP.Substring(12, 2))
                    OraReader.NextRead()
                    intCOUNT += 1
                End While
            End If

            If intCOUNT = 0 Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
            fn_check_Table = True

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
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
              Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating, txtTORIS.Validating, txtTORIF.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ゼロパディング", "失敗", ex.ToString)
        End Try
    End Sub
#End Region
End Class