Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJPRNT120
    Private MainLOG As New CASTCommon.BatchLOG("KFJPRNT120", "振替不能事由別集計表印刷画面")
    Private Const msgTitle As String = "振替不能事由別集計表印刷画面(KFJPRNT120)"
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

#Region "宣言"
    Dim strTORIS_CODE As String
    Dim strTORIF_CODE As String
    Dim strFURI_DATE As String
#End Region

    Private Sub KFJPRNT120_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '--------------------------------
            '委託者名リストボックスの設定
            '--------------------------------
            '取引先コンボボックス設定
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

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

#Region " 印刷"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :印刷ボタン
        'Return         :
        'Create         :2009/09/19
        'Update         :
        '=====================================================================================
        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Dim lngDataCNT As Long = 0

        Dim PrintID As String = ""
        Dim PrintName As String = ""
        Dim param As String
        Dim nRet As Integer
        Dim EndCode As Integer

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            PrintID = "KFJP052"
            PrintName = "振替不能事由別集計表"

            '2014/03/19 saitou 奈良信金 標準修正 MODIFY ----------------------------------------------->>>>
            'メッセージ移動
            'If MessageBox.Show(String.Format(MSG0013I, PrintName), _
            '                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            '    Return
            'End If
            '2014/03/19 saitou 奈良信金 標準修正 MODIFY -----------------------------------------------<<<<

            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Return
            End If

            strTORIS_CODE = txtTorisCode.Text
            strTORIF_CODE = txtTorifCode.Text
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            LW.ToriCode = strTORIS_CODE + strTORIF_CODE
            LW.FuriDate = strFURI_DATE

            '------------------------------------------
            '取引先マスタ存在チェック
            '------------------------------------------
            If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                SQL.Append("SELECT ")
                SQL.Append(" COUNT(*) COUNTER")
                SQL.Append(" FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))

                If OraReader.DataReader(SQL) = True Then
                    lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                    MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    txtTorisCode.Focus()
                    Exit Sub
                End If

                If lngDataCNT = 0 Then
                    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    OraReader.Close()
                    txtTorisCode.Focus()
                    Exit Sub
                End If
            End If

            '------------------------------------------
            '対象データ取得
            '------------------------------------------
            '2018/06/26 saitou 広島信金(RSV2) ADD 明細マスタ存在チェック追加 ---------------------------------------- START
            'スケジュールが存在して、明細マスタが存在しない場合（大阪地区の日報など）に、
            '印刷処理が異常終了するのを異常終了しないように考慮する。
            With SQL
                .Length = 0

                .Append("select")
                .Append("     TORIS_CODE_T")
                .Append("    ,TORIF_CODE_T")
                .Append(" from")
                .Append("     SCHMAST")
                .Append("    ,TORIMAST")
                .Append("    ,MEIMAST")
                .Append(" where")
                .Append("     FURI_DATE_S = '" & strFURI_DATE & "'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and FUNOU_FLG_S = '1'")
                .Append(" and TORIS_CODE_S = TORIS_CODE_T")
                .Append(" and TORIF_CODE_S = TORIF_CODE_T")
                .Append(" and TORIS_CODE_S = TORIS_CODE_K")
                .Append(" and TORIF_CODE_S = TORIF_CODE_K")
                .Append(" and FURI_DATE_S = FURI_DATE_K")

                If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                    .Append(" and TORIS_CODE_S = '" & strTORIS_CODE & "'")
                    .Append(" and TORIF_CODE_S = '" & strTORIF_CODE & "'")
                End If

                .Append(" group by")
                .Append("     TORIS_CODE_T")
                .Append("    ,TORIF_CODE_T")
                .Append(" having")
                .Append("     count(*) > 0")
                .Append(" order by")
                .Append("     TORIS_CODE_T")
                .Append("    ,TORIF_CODE_T")
            End With

            'SQL = New StringBuilder(128)

            'SQL.Append(" SELECT")
            'SQL.Append(" TORIS_CODE_T,TORIF_CODE_T")
            'SQL.Append(" FROM SCHMAST,TORIMAST")
            'SQL.Append(" WHERE ")
            'SQL.Append(" FURI_DATE_S = " & SQ(strFURI_DATE))
            'SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            'SQL.Append(" AND FUNOU_FLG_S = '1'")
            'SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            'SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")

            'If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
            '    SQL.Append(" AND TORIS_CODE_S = " & SQ(strTORIS_CODE))
            '    SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            'End If
            '2018/06/26 saitou 広島信金(RSV2) ADD ------------------------------------------------------------------- END

            If OraReader.DataReader(SQL) = True Then

                '2014/03/19 saitou 奈良信金 標準修正 MODIFY ----------------------------------------------->>>>
                'メッセージ移動
                If MessageBox.Show(String.Format(MSG0013I, PrintName),
                                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If
                '2014/03/19 saitou 奈良信金 標準修正 MODIFY -----------------------------------------------<<<<

                While OraReader.EOF = False
                    strTORIS_CODE = OraReader.GetString("TORIS_CODE_T")
                    strTORIF_CODE = OraReader.GetString("TORIF_CODE_T")

                    '------------------------------------------
                    '印刷処理実行
                    '------------------------------------------

                    '印刷バッチ呼び出し
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me

                    param = strTORIS_CODE & strTORIF_CODE & "," & strFURI_DATE
                    nRet = ExeRepo.ExecReport(PrintID & ".EXE", param)

                    If nRet <> 0 Then
                        EndCode = 2
                    End If

                    OraReader.NextRead()
                End While

            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                '2014/03/19 saitou 奈良信金 標準修正 MODIFY ----------------------------------------------->>>>
                'メッセージ変更
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                '2014/03/19 saitou 奈良信金 標準修正 MODIFY -----------------------------------------------<<<<
                txtFuriDateY.Focus()
                Exit Sub
            End If

            '戻り値に対応したメッセージを表示する
            Select Case EndCode
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

            txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Sub

#End Region

#Region "関数"

    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :印刷ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/19
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
            '2011/06/16 標準版修正 チェック順を入れ替え ------------------START

            '日付必須チェック
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '月範囲チェック
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            ''日付必須チェック
            'If txtFuriDateD.Text.Trim = "" Then
            '    MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    txtFuriDateD.Focus()
            '    Return False
            'End If
            '2011/06/16 標準版修正 チェック順を入れ替え ------------------END
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

#End Region

#Region " イベント"

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '委託者名リストボックスの設定
        '--------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
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
            '取引先コンボボックス設定
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コード取得)", "失敗", ex.ToString)
        End Try

    End Sub
    'ゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtTorisCode.Validating, txtTorifCode.Validating, txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub
#End Region

End Class
