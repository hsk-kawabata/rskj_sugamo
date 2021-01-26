'========================================================================
'KFJPRNT160
'預金口座振替変更通知書印刷画面
'
'作成日：2017/03/09
'
'備考：
'========================================================================
Imports System.Windows.Forms
Imports CASTCommon
Imports System.Text

Public Class KFJPRNT160

#Region " 共通宣言"

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

#End Region

#Region " クラス変数宣言"

    Private MainLOG As New CASTCommon.BatchLOG("KFJPRNT160", "預金口座振替変更通知書印刷画面")
    Private Const msgTitle As String = "預金口座振替変更通知書印刷画面(KFJPRNT160)"

    Private strJyoken As String = ""
    Private strPrintName As String = "預金口座振替変更通知書"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure
    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

#End Region

#Region "ロード"

    '=======================================================================
    'KFJPRNT160_Load
    '
    '＜概要＞
    '　預金口座振替変更通知書印刷画面ロードイベントハンドラ
    '
    '＜パラメータ＞
    '　sender：
    '　e：
    '
    '＜戻り値＞
    '　なし
    '=======================================================================
    Private Sub KFJPRNT160_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '------------------------------------------
            ' ログ情報設定
            '------------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "開始", "")

            '------------------------------------
            ' システム日付とユーザ名を表示
            '------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)終了", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '------------------------------------------
            '委託者名リストボックスの設定
            '------------------------------------------
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", strJyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            '------------------------------------
            ' 振替日に前営業日を表示
            '------------------------------------
            Dim strSysDate As String
            Dim strGetdate As String = ""

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            bRet = GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)
            txtDateY.Text = strGetdate.Substring(0, 4)
            txtDateM.Text = strGetdate.Substring(4, 2)
            txtDateD.Text = strGetdate.Substring(6, 2)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "終了", "")
        End Try
    End Sub

#End Region

#Region " ボタン"

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "開始", "")

            '--------------------------------
            ' 必須チェック
            '--------------------------------
            If CheckIsInputRequiredControl() = False Then
                Return
            End If

            '--------------------------------
            ' 印刷前確認メッセージ
            '--------------------------------
            If MessageBox.Show(String.Format(MSG0013I, strPrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "キャンセル", "")
                Return
            End If

            '--------------------------------
            ' パラメータ作成
            '--------------------------------
            Dim prmUserId As String = GCom.GetUserID                                    'ユーザＩＤ
            Dim prmToriSCd As String = Me.txtTorisCode.Text.Trim                        '取引先主コード
            Dim prmToriFCd As String = Me.txtTorifCode.Text.Trim                        '取引先副コード
            Dim prmDate As String = txtDateY.Text & txtDateM.Text & txtDateD.Text       '振替日

            '------------------------------------------
            ' レポエージェント印刷
            '------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            Dim strParam As String = String.Format("{0},{1},{2},{3}", prmUserId, prmToriSCd, prmToriFCd, prmDate)

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "開始", "パラメータ:" & strParam)
            Dim iRetValue As Integer = ExeRepo.ExecReport("KFJP063", strParam)
            Select Case iRetValue
                Case 0
                    '正常
                    MessageBox.Show(String.Format(MSG0014I, strPrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "成功", "")
                Case -1
                    '出力データなし
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "成功", "印刷対象なし")
                Case Else
                    '実行エラー
                    MessageBox.Show(String.Format(MSG0004E, strPrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "失敗", "戻り値 = " & iRetValue.ToString)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷", "終了", "")
        End Try

    End Sub

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "開始", "")
            Me.Close()
            Me.Dispose()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "終了", "")
        End Try
    End Sub

#End Region

#Region " 関数"

    ' 入力チェック
    Private Function CheckIsInputRequiredControl() As Boolean

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "開始", "")

            '取引先主コード
            Dim strToriSCd As String = Me.txtTorisCode.Text.Trim()
            If strToriSCd = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "取引先主コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '取引先副コード
            Dim strToriFCd As String = Me.txtTorifCode.Text.Trim()
            If strToriFCd = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "取引先副コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '振替日(年)
            Dim FURIKAE_DATE As String
            If fn_CHECK_NUM_MSG(txtDateY.Text, "振替日(年)", msgTitle) = False Then
                txtDateY.Focus()
                Return False
            End If

            '振替日(月)
            If fn_CHECK_NUM_MSG(txtDateM.Text, "振替日(月)", msgTitle) = False Then
                txtDateM.Focus()
                Return False
            Else
                If txtDateM.Text < 1 Or txtDateM.Text > 12 Then
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtDateM.Focus()
                    Return False
                End If
            End If

            '振替日(日)
            If fn_CHECK_NUM_MSG(txtDateD.Text, "振替日(日)", msgTitle) = False Then
                txtDateD.Focus()
                Return False
            Else
                If txtDateD.Text < 1 Or txtDateD.Text > 31 Then
                    MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtDateD.Focus()
                    Return False
                End If
            End If

            '振替日妥当性チェック
            If txtDateY.Text.Length <> 0 Or txtDateM.Text.Length <> 0 Or txtDateD.Text.Length <> 0 Then
                FURIKAE_DATE = txtDateY.Text & "/" & txtDateM.Text & "/" & txtDateD.Text
                If Not IsDate(FURIKAE_DATE) Then
                    MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtDateY.Focus()
                    txtDateY.SelectionStart = 0
                    txtDateY.SelectionLength = txtDateY.Text.Length
                    Return False
                End If
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "成功", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "終了", "")
        End Try

    End Function

    Private Function fn_CHECK_NUM_MSG(ByVal objOBJ As String, ByVal strJNAME As String, ByVal gstrTITLE As String) As Boolean
        '============================================================================
        'NAME           :fn_CHECK_NUM_MSG
        'Parameter      :objOBJ：チェック対象オブジェクト／strJNAME：オブジェクト名称
        '               :gstrTITLE：タイトル
        'Description    :数値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/05/28
        'Update         :
        '============================================================================

        Try

            If Trim(objOBJ).Length = 0 Then
                MessageBox.Show(String.Format(MSG0285W, strJNAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            For i As Integer = 0 To objOBJ.Length - 1 Step 1       '小数点/符号ﾁｪｯｸ
                If Char.IsDigit(objOBJ.Chars(i)) = False Then
                    MessageBox.Show(String.Format(MSG0344W, strJNAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            Next i

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function

#End Region

#Region " イベント"

    'テキストボックスゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtTorisCode.Validating, _
                txtTorifCode.Validating, _
                txtDateY.Validating, _
                txtDateM.Validating, _
                txtDateD.Validating


        Call GCom.NzNumberString(CType(sender, TextBox), True)

    End Sub

    '取引先情報設定(取引先名・振替日)
    Private Sub ToriCode_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles txtTorisCode.Validated, _
                txtTorifCode.Validated

        lblToriName.Text = ""
        If (txtTorisCode.Text.Trim & txtTorifCode.Text.Trim).Length <> 12 Then
            Return
        End If

        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder()

        Try
            MainDB = New MyOracle
            OraReader = New MyOracleReader(MainDB)

            SQL.Append(" SELECT * FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(txtTorisCode.Text))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(txtTorifCode.Text))


            If OraReader.DataReader(SQL) = True Then
                lblToriName.Text = OraReader.GetString("ITAKU_NNAME_T")
            Else
                lblToriName.Text = ""
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(取引先情報設定)", "失敗", ex.ToString)
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '委託者名リストボックスの設定
        '--------------------------------
        If GCom.SelectItakuName(cmbKana.SelectedItem, cmbToriName, txtTorisCode, txtTorifCode, "1", strJyoken) = -1 Then
            MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        '-------------------------------------------
        '取引先コードテキストボックスに取引先コード設定
        '-------------------------------------------
        If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
            GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
            ToriCode_Validated(sender, e)
        End If
    End Sub

#End Region

End Class
