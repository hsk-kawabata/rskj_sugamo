Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Imports System.Data.OracleClient
Public Class KFGPRNT140

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private strBAITAI_CODE As String

    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT140", "口座振替依頼書印刷画面")
    Private Const msgTitle As String = "口座振替依頼書印刷画面(KFGPRNT140)"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Const ThisModuleName As String = "KFGPRNT140.vb"
    Private strFuriDate As String

    '2010/10/08.Sakon　対象学年チェックボックス格納
    Private chkTaisyoGakunen(9) As CheckBox
    Private SiyouGakunen As Integer
    Private i As Integer

#Region " ロード"
    Private Sub KFGPRNT140_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '--------------------------------
            '委託者名コンボボックスの設定
            '--------------------------------
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '--------------------------------
            '入出金コンボボックスの設定
            '--------------------------------
            '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- START
            'テキストファイルからコンボボックス設定
            Dim MSG As String = ""
            Dim NS_TXT As String = "KFGPRNT140_入出金区分.TXT"
            Select Case GCom.SetComboBox(cmbKbn, NS_TXT, True)
                Case 1  'ファイルなし
                    MSG = String.Format(MSG0025E, "入出金区分", NS_TXT)
                    MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MSG = "入出金区分" & "設定ファイルなし。ファイル:" & NS_TXT
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", MSG)
                    Return
                Case 2  '異常
                    MSG = String.Format(MSG0026E, "入出金区分")
                    MessageBox.Show(MSG.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MSG = "コンボボックス設定失敗 コンボボックス名:" & "入出金区分"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", MSG)
                    Return
            End Select
            '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- END
            cmbKbn.SelectedIndex = 0

            '--------------------------------
            ''2010/10/08.Sakon　
            '対象学年チェックボックス格納
            '--------------------------------
            Call SetCheckBox()

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
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If

            '-------------------------------------------------------
            '2010/10/08.Sakon　印刷対象学年を指定できるように変更
            '対象学年チェック
            '-------------------------------------------------------
            If fn_check_chkBox() = False Then
                Exit Sub
            End If

            Dim strGAKKOU_CODE As String = txtGAKKOU_CODE.Text
            LW.ToriCode = strGAKKOU_CODE
            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "口座振替依頼書"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            Dim param As String
            Dim nRet As Integer

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            '2010/11/06 入出金区分設定誤り修正
            ''2010/10/12.Sakon　対象学年指定対応 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            ''パラメータ設定：ログイン名、取引先主副コード、振替日、依頼書種別、対象学年
            'param = GCom.GetUserID & "," & strGAKKOU_CODE & "," & GCom.GetComboBox(cmbKbn) & "," & fn_GetTaisyouGakunen()
            ' ''パラメータ設定：ログイン名、取引先主副コード、振替日、依頼書種別
            ''param = GCom.GetUserID & "," & strGAKKOU_CODE & "," & GCom.GetComboBox(cmbKbn)
            ''+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- START
            param = GCom.GetUserID & "," & strGAKKOU_CODE & "," & GCom.GetComboBox(cmbKbn) & "," & fn_GetTaisyouGakunen()
            'param = GCom.GetUserID & "," & strGAKKOU_CODE & "," & cmbKbn.SelectedIndex & "," & fn_GetTaisyouGakunen()
            '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- END
            nRet = ExeRepo.ExecReport("KFGP032.EXE", param)

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, "口座振替依頼書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(String.Format(MSG0106W, "口座振替依頼書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "口座振替依頼書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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
            '取引先主コード必須チェック(MSG0057W)
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

            fn_check_text = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try

    End Function

    Function fn_check_chkBox() As Boolean
        '============================================================================
        'NAME           :fn_check_chkBox
        'Parameter      :
        'Description    :対象学年チェックボックスの入力チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/09
        'Update         :
        '============================================================================
        fn_check_chkBox = False
        Dim i As Integer
        Try
            '対象学年が１つでもチェックされていることを確認
            For i = 0 To chkTaisyoGakunen.Length - 1
                If chkTaisyoGakunen(i).Checked = True Then
                    fn_check_chkBox = True
                    Exit Function
                End If
            Next

            MessageBox.Show("対象学年が選択されていません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            chkTaisyoGakunen(1).Focus()

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
        'Description    :印刷ボタンを押下時にマスタの相関チェックを実行
        'Return         :True=OK,False=NG
        'Create         :2009/09/10
        'Update         :
        '============================================================================
        fn_check_Table = False
        MainDB = New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try
            '学校情報取得
            SQL.Append("SELECT * FROM GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(Trim(txtGAKKOU_CODE.Text)))
            If OraReader.DataReader(SQL) = True Then
                OraReader.Close()
            Else
                '取引先なし
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                OraReader.Close()
                txtGAKKOU_CODE.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If MainDB IsNot Nothing Then MainDB.Close()
        End Try
        fn_check_Table = True
    End Function

    Private Sub SetCheckBox()
        '============================================================================
        'NAME           :SetCheckBox
        'Parameter      :
        'Description    :学年チェックボックスを配列に格納
        'Return         :なし
        'Create         :2010/10/12
        'Update         :
        '============================================================================
        Try
            chkTaisyoGakunen(0) = chk対象_全学年
            chkTaisyoGakunen(1) = chk対象_１学年
            chkTaisyoGakunen(2) = chk対象_２学年
            chkTaisyoGakunen(3) = chk対象_３学年
            chkTaisyoGakunen(4) = chk対象_４学年
            chkTaisyoGakunen(5) = chk対象_５学年
            chkTaisyoGakunen(6) = chk対象_６学年
            chkTaisyoGakunen(7) = chk対象_７学年
            chkTaisyoGakunen(8) = chk対象_８学年
            chkTaisyoGakunen(9) = chk対象_９学年

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(チェックボックス設定)", "失敗", ex.ToString)
            Exit Sub
        End Try
    End Sub

    Private Function fn_GetTaisyouGakunen() As String
        '============================================================================
        'NAME           :fn_GetTaisyouGakunen
        'Parameter      :
        'Description    :チェックされている対象学年を文字列でかえす
        'Return         :なし
        'Create         :2010/10/12
        'Update         :
        '============================================================================
        If chkTaisyoGakunen(0).Checked = True Then
            '全学年指定
            Return "123456789"
        Else
            Dim TaisyouGakunen As String = ""
            For i = 1 To 9
                If chkTaisyoGakunen(i).Checked = True Then
                    TaisyouGakunen &= CStr(i)
                End If
            Next
            Return TaisyouGakunen
        End If
    End Function

#End Region
#Region " イベント"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '学校カナ絞込みコンボ
        '********************************************
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校名コンボボックス設定
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = True Then
            cmbGakkouName.Focus()
        End If

    End Sub
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '学校検索後の学校コード設定
        '2010/10/12.Sakon　設定順序を変更 ++++++++++++++++++++++++++++++++++
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
        lblGAKKOU_NAME.Text = cmbGakkouName.Text.Trim
        'lblGAKKOU_NAME.Text = cmbGakkouName.Text.Trim
        'txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
             Handles txtGAKKOU_CODE.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '学校名検索
            STR_SQL = "SELECT GAKKOU_NNAME_G FROM GAKMAST1 "
            STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            OBJ_DATAREADER.Read()
            If OBJ_DATAREADER.HasRows = True Then
                lblGAKKOU_NAME.Text = CStr(OBJ_DATAREADER.Item("GAKKOU_NNAME_G"))
            Else
                lblGAKKOU_NAME.Text = ""
            End If

            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If
        End If

    End Sub
    '全学年チェック時の制御
    Private Sub chk対象_全学年_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk対象_全学年.CheckedChanged
        Select Case chk対象_全学年.Checked
            Case True
                For i = 1 To 9
                    chkTaisyoGakunen(i).Checked = False
                    chkTaisyoGakunen(i).Enabled = False
                Next
            Case False
                For i = 1 To 9
                    '2010/11/06 学校が未選択の場合、全学年指定可能に戻す
                    'If i <= SiyouGakunen Then
                    '    chkTaisyoGakunen(i).Enabled = True
                    'End If
                    If txtGAKKOU_CODE.Text <> "" Then
                        If i <= SiyouGakunen Then
                            chkTaisyoGakunen(i).Enabled = True
                        End If
                    Else
                        chkTaisyoGakunen(i).Enabled = True
                    End If
                Next
        End Select
    End Sub
    '学校名表示時に対象学年を制御する
    Private Sub lblGAKKOU_NAME_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblGAKKOU_NAME.TextChanged
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            STR_SQL = "SELECT SIYOU_GAKUNEN_T FROM GAKMAST2 "
            STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            OBJ_DATAREADER.Read()

            If OBJ_DATAREADER.HasRows = True Then
                SiyouGakunen = CInt(OBJ_DATAREADER.Item("SIYOU_GAKUNEN_T"))
                chkTaisyoGakunen(0).Enabled = True
                For i = 1 To 9
                    If i <= SiyouGakunen Then
                        chkTaisyoGakunen(i).Enabled = True
                    Else
                        chkTaisyoGakunen(i).Enabled = False
                    End If
                Next

                chk対象_全学年.Checked = False      '2010/11/06 全学年チェックは学校が変わったらはずす
            End If

            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If
        End If


    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

End Class