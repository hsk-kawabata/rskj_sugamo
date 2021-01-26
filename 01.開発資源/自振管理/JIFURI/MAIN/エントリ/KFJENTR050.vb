Imports clsFUSION.clsMain
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Imports System.Data.OracleClient
Public Class KFJENTR050
    Private clsFUSION As New clsFUSION.clsMain()

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private strBAITAI_CODE As String

    Private MainLOG As New CASTCommon.BatchLOG("KFJENTR050", "口座振替入力チェックリスト印刷画面")
    Private Const msgTitle As String = "口座振替入力チェックリスト印刷画面(KFJENTR050)"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Const ThisModuleName As String = "KFJENTR050.vb"
    Private strFuriDate As String
#Region " ロード"
    Private Sub KFJENTR050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            Call GCom.SetMonitorTopArea(Label2, Label4, lblUser, lblDate)
            '--------------------------------
            '委託者名リストボックスの設定
            '--------------------------------
            '取引先コンボボックス設定
            Dim Jyoken As String = " AND BAITAI_CODE_T IN('04','09') "   '媒体が依頼書
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
#Region " 印刷"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            Dim strTORIS_CODE As String = txtTorisCode.Text
            Dim strTORIF_CODE As String = txtTorifCode.Text
            strFuriDate = txtFuriDateY.Text + txtFuriDateM.Text + txtFuriDateD.Text
            LW.ToriCode = strTORIS_CODE + strTORIF_CODE
            LW.FuriDate = strFuriDate
            '--------------------------------
            'マスタチェック
            '--------------------------------
            '取引先コードチェック
            If fn_check_ToriMast() = False Then
                Exit Sub
            End If

            'データ存在チェック
            Select Case strBAITAI_CODE
                Case "04"   '依頼書
                    If fn_check_Iraisyo() = False Then
                        Exit Sub
                    End If
                Case "09"   '伝票
                    If fn_check_Denpyo() = False Then
                        Exit Sub
                    End If
            End Select

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "口座振替入力チェックリスト"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            Dim param As String
            Dim nRet As Integer

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：ログイン名、取引先主副コード,振替日,媒体コード,ソート順
            If rdbSort1.Checked = True Then
                'ソートなし
                param = GCom.GetUserID & "," & strTORIS_CODE & strTORIF_CODE & "," & strFuriDate & "," & strBAITAI_CODE & "," & "1"
            Else
                '支店ソート
                param = GCom.GetUserID & "," & strTORIS_CODE & strTORIF_CODE & "," & strFuriDate & "," & strBAITAI_CODE & "," & "2"
            End If


            nRet = ExeRepo.ExecReport("KFJP029.EXE", param)

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, "口座振替入力チェックリスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(String.Format(MSG0106W, "口座振替入力チェックリスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "口座振替入力チェックリスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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
    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :印刷ボタンを押下時に必須項目のテキストボックスの入力値チェック
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
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then
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
    Function fn_check_ToriMast() As Boolean
        '============================================================================
        'NAME           :fn_check_ToriMast
        'Parameter      :
        'Description    :印刷ボタンを押下時に取引先マスタの相関チェックを実行
        'Return         :True=OK,False=NG
        'Create         :2009/09/10
        'Update         :
        '============================================================================
        fn_check_ToriMast = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            If txtTorisCode.Text + txtTorifCode.Text <> "111111111111" AndAlso _
                txtTorisCode.Text + txtTorifCode.Text <> "222222222222" Then
                '取引先情報取得
                SQL.Append("SELECT * FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
                If OraReader.DataReader(SQL) = True Then
                    strBAITAI_CODE = GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T"))
                    OraReader.Close()
                Else
                    '取引先なし
                    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    OraReader.Close()
                    txtTorisCode.Focus()
                    Return False
                End If

                '媒体コードチェック
                If strBAITAI_CODE <> "04" AndAlso strBAITAI_CODE <> "09" Then '依頼書
                    MessageBox.Show(MSG0134W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorisCode.Focus()
                    Return False
                End If
            ElseIf txtTorisCode.Text + txtTorifCode.Text = "111111111111" Then
                strBAITAI_CODE = "04"
            ElseIf txtTorisCode.Text + txtTorifCode.Text = "222222222222" Then
                strBAITAI_CODE = "09"
            End If


        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先マスタチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_check_ToriMast = True
    End Function
    Function fn_check_Iraisyo() As Boolean
        '============================================================================
        'NAME           :fn_check_Iraisyo
        'Parameter      :
        'Description    :印刷ボタンを押下時に依頼書の相関チェックを実行
        'Return         :True=OK,False=NG
        'Create         :2009/09/10
        'Update         :
        '============================================================================
        fn_check_Iraisyo = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            'スケジュールマスタに印刷対象のスケジュールが存在するかチェックする
            SQL = New StringBuilder(128)
            SQL.Append("SELECT COUNT(*) COUNTER ")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='1'")
            If txtTorisCode.Text + txtTorifCode.Text = "111111111111" Then
                SQL.Append(" AND BAITAI_CODE_T = '04'")
            Else
                SQL.Append(" AND TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
            End If
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            SQL.Append(" AND FURI_DATE_S = " & SQ(strFuriDate))
            SQL.Append(" AND TYUUDAN_FLG_S = '0' ")
            'SQL.Append(" AND UKETUKE_FLG_S = '1' ") '受付済
            Dim Count As Integer
            If OraReader.DataReader(SQL) = True Then
                Count = GCom.NzInt(OraReader.Reader.Item("COUNTER"))
                OraReader.Close()
            Else
                '検索失敗(MSG0002E)
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                txtFuriDateY.Focus()
                Return False
            End If

            If Count = 0 Then
                '対象なし
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            '依頼書マスタに印刷対象となるデータが存在するかチェックする
            SQL = New StringBuilder(128)
            SQL.Append("SELECT COUNT(*) COUNTER ")
            SQL.Append(" FROM TORIMAST,ENTMAST")
            SQL.Append(" WHERE FSYORI_KBN_E ='1'")
            If txtTorisCode.Text + txtTorifCode.Text <> "111111111111" Then
                SQL.Append(" AND TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
            End If
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_E ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_E ")
            SQL.Append(" AND FURI_DATE_E = " & SQ(strFuriDate))
            SQL.Append(" AND KAIYAKU_E = '0' ") '解約済みでないこと
            SQL.Append(" AND FURIKIN_E > 0 ")
            SQL.Append(" AND KAISI_DATE_E <= " & SQ(strFuriDate))

            If OraReader.DataReader(SQL) = True Then
                Count = GCom.NzInt(OraReader.Reader.Item("COUNTER"))
                OraReader.Close()
            Else
                '検索失敗
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                txtTorisCode.Focus()
                Return False
            End If
            If Count = 0 Then
                '対象なし
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_check_Iraisyo = True
    End Function
    Function fn_check_Denpyo() As Boolean
        '============================================================================
        'NAME           :fn_check_Denpyo
        'Parameter      :
        'Description    :印刷ボタンを押下時に伝票の相関チェックを実行
        'Return         :True=OK,False=NG
        'Create         :2009/09/10
        'Update         :
        '============================================================================
        fn_check_Denpyo = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            'スケジュールマスタに印刷対象のスケジュールが存在するかチェックする
            SQL = New StringBuilder(128)
            SQL.Append("SELECT COUNT(*) COUNTER ")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='1'")
            If txtTorisCode.Text + txtTorifCode.Text = "222222222222" Then
                SQL.Append(" AND BAITAI_CODE_T = '09'")
            Else
                SQL.Append(" AND TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
            End If
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            SQL.Append(" AND FURI_DATE_S = " & SQ(strFuriDate))
            SQL.Append(" AND TYUUDAN_FLG_S = '0' ")
            'SQL.Append(" AND UKETUKE_FLG_S = '1' ") '受付済
            Dim Count As Integer
            If OraReader.DataReader(SQL) = True Then
                Count = GCom.NzInt(OraReader.Reader.Item("COUNTER"))
                OraReader.Close()
            Else
                '検索失敗(MSG0002E)
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                Return False
            End If

            If Count = 0 Then
                '対象なし
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '伝票マスタに印刷対象となるデータが存在するかチェックする
            SQL = New StringBuilder(128)
            SQL.Append("SELECT COUNT(*) COUNTER ")
            SQL.Append(" FROM TORIMAST,DENPYOMAST")
            SQL.Append(" WHERE FSYORI_KBN_E ='1'")
            If txtTorisCode.Text + txtTorifCode.Text <> "222222222222" Then
                SQL.Append(" AND TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
            End If
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_E ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_E ")
            SQL.Append(" AND FURI_DATE_E = " & SQ(strFuriDate))
            SQL.Append(" AND KAIYAKU_E = '0' ") '解約済みでないこと
            SQL.Append(" AND FURIKIN_E > 0 ")

            If OraReader.DataReader(SQL) = True Then
                Count = GCom.NzInt(OraReader.Reader.Item("COUNTER"))
                OraReader.Close()
            Else
                '検索失敗(MSG0002E)
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                Return False
            End If
            If Count = 0 Then
                '対象なし
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_check_Denpyo = True
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
                Dim Jyoken As String = " AND BAITAI_CODE_T IN('04','09') "   '媒体が依頼書
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
            cmbToriName.Focus()
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