Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT130
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 口座振替結果帳票印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Dim Str_Report_Path As String
    Dim STR学校コード As String
    Dim STR処理名 As String
    Dim STR帳票ソート順 As String
    Dim STR振替区分 As String
    Dim STR初振日 As String
    Dim STR対象年月 As String
    Private STR_REPORT_KBN(5) As String
    '2006/10/20
    Dim blnPRINT_FLG As Boolean
    Dim strFURI_DATE_Y As String
    Dim strFURI_DATE_M As String
    Dim strFURI_DATE_D As String

    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT130", "随時処理結果帳票印刷画面")
    Private Const msgTitle As String = "随時処理結果帳票印刷画面(KFGPRNT130)"
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub GFJPRNT0300G_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '入力ボタン制御
            btnPrnt.Enabled = True
            btnEnd.Enabled = True


        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Try
            '印刷ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            MainDB = New MyOracle

            blnPRINT_FLG = False

            '入力チェック
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            '出力帳票選択チェック
            If chk振替結果一覧.Checked = False And chk振替不能明細.Checked = False And chk振替店別集計表.Checked = False Then
                MessageBox.Show("帳票が選択されていません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "結果帳票"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            If chk振替結果一覧.Checked = True Then
                STR学校コード = txtGAKKOU_CODE.Text
                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If
                STR学校コード = txtGAKKOU_CODE.Text

                '口座振替結果一覧表印刷 
                'パラメータ設定：ログイン名,学校コード,振替日,初振日,対象年月,合算判定,印刷区分("0"固定),振替区分,帳票ソート順
                Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR初振日 & "," & _
                        STR対象年月 & "," & IIf(chk振替結果一覧合算出力.Checked, "1", "0") & ",0," & STR振替区分 & "," & STR帳票ソート順

                nRet = ExeRepo.ExecReport("KFGP016.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "口座振替結果一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                blnPRINT_FLG = True
            End If

            If chk振替不能明細.Checked = True Then
                STR学校コード = txtGAKKOU_CODE.Text
                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If
                '不能結果一覧表印刷 
                'パラメータ設定：ログイン名,学校コード,振替日,初振日,対象年月,合算判定,印刷区分("1"固定),振替区分,帳票ソート順
                Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR初振日 & "," & _
                        STR対象年月 & "," & IIf(chk振替結果一覧合算出力.Checked, "1", "0") & ",1," & STR振替区分 & "," & STR帳票ソート順

                nRet = ExeRepo.ExecReport("KFGP016.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "口座振替不能明細一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                blnPRINT_FLG = True
            End If

            If chk振替店別集計表.Checked = True Then
                If PFUNC_口座振替店別集計表() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If
                '口座振替店別集計表印刷 
                'パラメータ設定：ログイン名,学校コード,振替日,入出金区分
                Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR振替区分
                nRet = ExeRepo.ExecReport("KFGP019.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "口座振替店別集計表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                blnPRINT_FLG = True
            End If

            '1枚でも印刷していたら、完了メッセージを出力する
            If blnPRINT_FLG = True Then
                MessageBox.Show("印刷が完了しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Private Sub "
    '****************************
    'Private Sub
    '****************************
    Private Sub PSUB_CHK_ONOFF()

        '口座振替結果一覧表
        Select Case (STR_REPORT_KBN(0))
            Case "1"
                chk振替結果一覧.Checked = True
            Case Else
                chk振替結果一覧.Checked = False
        End Select

        '口座振替不能明細一覧表
        Select Case (STR_REPORT_KBN(1))
            Case "1"
                chk振替不能明細.Checked = True
            Case Else
                chk振替不能明細.Checked = False
        End Select

        '口座振替店別集計表
        Select Case (STR_REPORT_KBN(3))
            Case "1"
                chk振替店別集計表.Checked = True
            Case Else
                chk振替店別集計表.Checked = False
        End Select


    End Sub
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
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '学校検索後の学校コード設定
        lab学校名.Text = cmbGakkouName.Text.Trim
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
        STR学校コード = txtGAKKOU_CODE.Text.Trim

        '学校名の取得
        If PFUNC_GAKNAME_GET() = False Then
            Exit Sub
        End If

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '学校名の取得
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '2010/11/09 学校コードゼロ埋め
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            '学校名の取得
            STR学校コード = Trim(txtGAKKOU_CODE.Text)
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            Else
                '振替日コンボボックスの設定
                If PFUNC_Set_cmbFURIKAEBI() = False Then
                    Exit Sub
                End If
            End If
        End If


    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET() As Boolean

        '学校名の設定
        PFUNC_GAKNAME_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim OraDB As New MyOracle
        Try
            If Trim(STR学校コード) = "9999999999" Then
                lab学校名.Text = ""
            Else
                OraReader = New MyOracleReader(OraDB)
                SQL.Append(" SELECT ")
                SQL.Append(" GAKMAST1.*")
                SQL.Append(",MEISAI_FUNOU_T")
                SQL.Append(",MEISAI_KEKKA_T")
                SQL.Append(",MEISAI_HOUKOKU_T")
                SQL.Append(",MEISAI_TENBETU_T")
                SQL.Append(",MEISAI_MINOU_T")
                SQL.Append(",MEISAI_YOUKYU_T")
                SQL.Append(",MEISAI_OUT_T")
                SQL.Append(" FROM GAKMAST1,GAKMAST2")
                SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")
                SQL.Append(" AND GAKKOU_CODE_G =" & SQ(STR学校コード))

                If OraReader.DataReader(SQL) = False Then
                    lab学校名.Text = ""
                    STR帳票ソート順 = 0
                    Exit Function
                End If

                If txtGAKKOU_CODE.Text <> "9999999999" Then
                    lab学校名.Text = OraReader.GetString("GAKKOU_NNAME_G")
                    STR_REPORT_KBN(0) = OraReader.GetString("MEISAI_FUNOU_T")
                    STR_REPORT_KBN(1) = OraReader.GetString("MEISAI_KEKKA_T")
                    STR_REPORT_KBN(2) = OraReader.GetString("MEISAI_HOUKOKU_T")
                    STR_REPORT_KBN(3) = OraReader.GetString("MEISAI_TENBETU_T")
                    STR_REPORT_KBN(4) = OraReader.GetString("MEISAI_MINOU_T")
                    STR_REPORT_KBN(5) = OraReader.GetString("MEISAI_YOUKYU_T")
                End If

                STR帳票ソート順 = OraReader.GetString("MEISAI_OUT_T")

                OraReader.Close()

                If txtGAKKOU_CODE.Text <> "9999999999" Then
                    Call PSUB_CHK_ONOFF()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校名取得)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraDB IsNot Nothing Then OraDB.Close()
        End Try
        PFUNC_GAKNAME_GET = True

    End Function
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)

            '学校コード必須チェック
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

            If Trim(cmbFURIKAEBI.Text) = "" Then
                MessageBox.Show(String.Format(MSG0285W, "振替年月日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbFURIKAEBI.Focus()
                Exit Function
            End If

            strFURI_DATE_Y = Mid(cmbFURIKAEBI.Text, 1, 4)
            strFURI_DATE_M = Mid(cmbFURIKAEBI.Text, 6, 2)
            strFURI_DATE_D = Mid(cmbFURIKAEBI.Text, 9, 2)
            STR_FURIKAE_DATE(0) = Mid(cmbFURIKAEBI.Text, 1, 4) & "/" & Mid(cmbFURIKAEBI.Text, 6, 2) & "/" & Mid(cmbFURIKAEBI.Text, 9, 2)
            STR_FURIKAE_DATE(1) = Trim(Mid(cmbFURIKAEBI.Text, 1, 4)) & Format(CInt(Mid(cmbFURIKAEBI.Text, 6, 2)), "00") & Format(CInt(Mid(cmbFURIKAEBI.Text, 9, 2)), "00")

            '日付整合性チェック
            If Information.IsDate(STR_FURIKAE_DATE(0)) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbFURIKAEBI.Focus()
                Return False
            End If

            SQL.Append("SELECT * FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(Trim(txtGAKKOU_CODE.Text)))
            SQL.Append(" AND FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
            Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
                Case "入金"
                    SQL.Append(" AND FURI_KBN_S ='2'")
                Case "出金"
                    SQL.Append(" AND FURI_KBN_S ='3'")
            End Select

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("スケジュールが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            If OraReader.GetString("FUNOU_FLG_S") = "0" Then
                MessageBox.Show("このスケジュールは不能結果更新処理が未処理です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_SCHMAST_GET() As Boolean

        PFUNC_SCHMAST_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S  = " & SQ(STR学校コード))
            SQL.Append(" AND FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
            '2017/03/14 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ START
            Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
                Case "入金"
                    SQL.Append(" AND FURI_KBN_S ='2'")
                Case "出金"
                    SQL.Append(" AND FURI_KBN_S ='3'")
            End Select
            '2017/03/14 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ END

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("スケジュールが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            '振替区分の取得
            STR振替区分 = OraReader.GetString("FURI_KBN_S")
            STR対象年月 = OraReader.GetString("NENGETUDO_S")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュールチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_SCHMAST_GET = True

    End Function
    Private Function PFUNC_口座振替店別集計表() As Boolean

        PFUNC_口座振替店別集計表 = False

        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            '印刷を行うデータが存在するかどうかの判定
            SQL.Append(" SELECT * FROM G_MEIMAST,G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_M = " & SQ(STR_FURIKAE_DATE(1)))
            SQL.Append(" AND GAKKOU_CODE_M  =" & SQ(Trim(txtGAKKOU_CODE.Text)))
            Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
                Case "入金"
                    SQL.Append(" AND G_MEIMAST.FURI_KBN_M ='2'")
                Case "出金"
                    SQL.Append(" AND G_MEIMAST.FURI_KBN_M ='3'")
            End Select
            '2006/10/11 不能フラグを条件に追加
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M =G_SCHMAST.GAKKOU_CODE_S ")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M =G_SCHMAST.FURI_DATE_S ")
            SQL.Append(" AND FUNOU_FLG_S ='1' ")
            '2006/12/25 スケジュールの振替区分追加
            SQL.Append(" AND G_MEIMAST.FURI_KBN_M =G_SCHMAST.FURI_KBN_S ")

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("スケジュールが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_口座振替店別集計表 = True
    End Function
    Private Function PFUNC_Set_cmbFURIKAEBI() As Boolean

        '振替日コンボの設定
        Dim str振替日 As String

        PFUNC_Set_cmbFURIKAEBI = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim OraDB As New MyOracle
        Try
            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                '振替日コンボボックスのクリア
                cmbFURIKAEBI.Items.Clear()
                OraReader = New MyOracleReader(OraDB)
                'スケジュールマスタの検索、キーは学校コード、スケジュール区分、明細作成フラグ
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_S =" & SQ(txtGAKKOU_CODE.Text))
                SQL.Append(" AND SCH_KBN_S ='2'")
                SQL.Append(" AND ENTRI_FLG_S ='1'")
                SQL.Append(" ORDER BY FURI_DATE_S")

                If OraReader.DataReader(SQL) = False Then
                    Exit Function
                End If


                While OraReader.EOF = False
                    '振替日の編集
                    str振替日 = Mid(OraReader.GetString("FURI_DATE_S"), 1, 4) & "/" & Mid(OraReader.GetString("FURI_DATE_S"), 5, 2) & _
                                "/" & Mid(OraReader.GetString("FURI_DATE_S"), 7, 2)

                    '入金、出金の編集
                    Select Case OraReader.GetString("FURI_KBN_S")
                        Case "2"
                            str振替日 += " 入金"
                        Case "3"
                            str振替日 += " 出金"
                    End Select
                    '振替日コンボボックスへ追加
                    cmbFURIKAEBI.Items.Add(str振替日)
                    OraReader.NextRead()
                End While

                'コンボ先頭の設定
                cmbFURIKAEBI.SelectedIndex = 0
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振替日設定)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraDB IsNot Nothing Then OraDB.Close()
        End Try
        PFUNC_Set_cmbFURIKAEBI = True

    End Function
#End Region

#Region " 旧クエリ(クリレポ)"
    Private Function PFUC_SQLQuery_振替結果() As String
        Dim SSQL As String

        PFUC_SQLQuery_振替結果 = ""


        SSQL = "SELECT "
        SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.NENDO_M"
        SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
        SSQL = SSQL & ",G_MEIMAST.TUUBAN_M"
        SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU1_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU2_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU3_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU4_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU5_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU6_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU7_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU8_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU9_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU10_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU11_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU12_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU13_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU14_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU15_KIN_M"

        '2006/02/14
        SSQL = SSQL & ",G_MEIMAST.FURI_DATE_M"
        'SSQL = SSQL & ",G_SCHMAST.FURI_DATE_S"

        SSQL = SSQL & ",GAKMAST1.GAKKOU_CODE_G"
        SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME01_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME02_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME03_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME04_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME05_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME06_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME07_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME08_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME09_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME10_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME11_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME12_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME13_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME14_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME15_H"

        'SSQL = SSQL & ",SEITOMASTVIEW.NENDO_O"
        'SSQL = SSQL & ",SEITOMASTVIEW.TUUBAN_O"
        SSQL = SSQL & ",SEITOMASTVIEW.GAKUNEN_CODE_O"
        SSQL = SSQL & ",SEITOMASTVIEW.SEITO_KNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.SEITO_NNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.KAMOKU_O"
        SSQL = SSQL & ",SEITOMASTVIEW.KOUZA_O"
        SSQL = SSQL & ",SEITOMASTVIEW.MEIGI_KNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.TYOUSI_FLG_O"


        SSQL = SSQL & ",TENMAST.KIN_NO_N "
        SSQL = SSQL & ",TENMAST.SIT_NO_N "
        SSQL = SSQL & ",TENMAST.SIT_NNAME_N "


        SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.NENDO_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.GAKUNEN_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.TUUBAN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU1_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU2_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU3_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU4_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU5_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU6_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU7_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU8_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU9_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU10_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU11_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU12_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU13_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU14_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU15_KIN_M, 0)"

        '2006/02/14
        'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_DATE_S, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.FURI_DATE_M, 0)"

        SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_CODE_G, 0)"
        SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME01_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME02_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME03_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME04_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME05_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME06_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME07_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME08_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME09_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME10_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME11_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME12_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME13_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME14_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME15_H, 0)"

        SSQL = SSQL & ", NVL(SEITOMASTVIEW.NENDO_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.TUUBAN_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.GAKUNEN_CODE_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_KNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_NNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.KAMOKU_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.KOUZA_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.MEIGI_KNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.TYOUSI_FLG_O, 0)"

        SSQL = SSQL & ", NVL(TENMAST.KIN_NO_N, 0)"
        SSQL = SSQL & ", NVL(TENMAST.SIT_NO_N, 0)"
        SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

        SSQL = SSQL & " FROM "
        SSQL = SSQL & "  KZFMAST.G_MEIMAST"
        'SSQL = SSQL & " ,KZFMAST.G_SCHMAST"
        SSQL = SSQL & " ,KZFMAST.GAKMAST1"
        SSQL = SSQL & " ,KZFMAST.HIMOMAST"
        SSQL = SSQL & " ,KZFMAST.SEITOMASTVIEW"
        SSQL = SSQL & " ,KZFMAST.TENMAST"

        SSQL = SSQL & " WHERE "
        '2006/02/14
        'SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S  "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_KBN_M     = G_SCHMAST.FURI_KBN_S  "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    = G_SCHMAST.FURI_DATE_S  "

        'SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
        SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
        SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G(+)  "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = HIMOMAST.GAKKOU_CODE_H(+)  "
        SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = HIMOMAST.GAKUNEN_CODE_H(+)  "
        SSQL = SSQL & " AND G_MEIMAST.HIMOKU_ID_M    = HIMOMAST.HIMOKU_ID_H(+)  "
        SSQL = SSQL & " AND SUBSTR(G_MEIMAST.SEIKYU_TUKI_M,5,2)  = HIMOMAST.TUKI_NO_H(+)  "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMASTVIEW.GAKKOU_CODE_O(+)  "
        SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMASTVIEW.NENDO_O(+)  "
        SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMASTVIEW.TUUBAN_O(+)  "
        SSQL = SSQL & " AND '04'                     = SEITOMASTVIEW.TUKI_NO_O(+)  "

        SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N(+) "
        SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N(+) "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  =" & "'" & STR学校コード & "'"
        '2006/10/13 請求金額が0円のデータは出力しないように修正
        SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

        If chk振替結果一覧合算出力.Checked = True And STR初振日 <> "" Then
            '再振日の入力で合算出力の場合
            '入力した再振日の明細は全て対象だが
            '取得した初振日の明細は振替済のものが対象
            SSQL = SSQL & " AND (G_MEIMAST.FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"
            SSQL = SSQL & " OR (G_MEIMAST.FURI_DATE_M = '" & STR初振日 & "'"
            SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M = 0))"
        Else
            '初振日または再振日の入力で合算出力なしの場合
            SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    =" & "'" & STR_FURIKAE_DATE(1) & "'"
        End If

        '2006/10/20 入出金区分を検索条件に追加
        Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
            Case "入金"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='2'"
            Case "出金"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='3'"
        End Select

        SSQL = SSQL & " ORDER BY "
        Select Case (STR帳票ソート順)
            Case "0"
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
                SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
            Case "1"
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
            Case Else
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '明細マスタには名義人しか無い 2006/10/11
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
        End Select

        PFUC_SQLQuery_振替結果 = SSQL

        'Debug.WriteLine("SSQL=" & SSQL)

    End Function
    Private Function PFUC_SQLQuery_振替不能結果() As String
        Dim SSQL As String

        PFUC_SQLQuery_振替不能結果 = ""


        SSQL = "SELECT "
        SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
        SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU1_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU2_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU3_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU4_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU5_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU6_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU7_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU8_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU9_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU10_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU11_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU12_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU13_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU14_KIN_M"
        SSQL = SSQL & ",G_MEIMAST.HIMOKU15_KIN_M"

        '2006/02/14
        'SSQL = SSQL & ",G_SCHMAST.FURI_DATE_S"
        SSQL = SSQL & ",G_MEIMAST.FURI_DATE_M"

        SSQL = SSQL & ",GAKMAST1.GAKKOU_CODE_G"
        SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME01_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME02_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME03_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME04_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME05_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME06_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME07_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME08_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME09_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME10_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME11_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME12_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME13_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME14_H"
        SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME15_H"

        SSQL = SSQL & ",SEITOMASTVIEW.NENDO_O"
        SSQL = SSQL & ",SEITOMASTVIEW.TUUBAN_O"
        SSQL = SSQL & ",SEITOMASTVIEW.GAKUNEN_CODE_O"
        SSQL = SSQL & ",SEITOMASTVIEW.SEITO_KNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.SEITO_NNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.KAMOKU_O"
        SSQL = SSQL & ",SEITOMASTVIEW.KOUZA_O"
        SSQL = SSQL & ",SEITOMASTVIEW.MEIGI_KNAME_O"
        SSQL = SSQL & ",SEITOMASTVIEW.TYOUSI_FLG_O"


        SSQL = SSQL & ",TENMAST.KIN_NO_N "
        SSQL = SSQL & ",TENMAST.SIT_NO_N "
        SSQL = SSQL & ",TENMAST.SIT_NNAME_N "


        SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.GAKUNEN_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU1_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU2_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU3_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU4_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU5_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU6_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU7_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU8_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU9_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU10_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU11_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU12_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU13_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU14_KIN_M, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU15_KIN_M, 0)"

        '2006/02/14
        'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_DATE_S, 0)"
        SSQL = SSQL & ", NVL(G_MEIMAST.FURI_DATE_M, 0)"

        SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_CODE_G, 0)"
        SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME01_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME02_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME03_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME04_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME05_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME06_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME07_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME08_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME09_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME10_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME11_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME12_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME13_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME14_H, 0)"
        SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME15_H, 0)"

        SSQL = SSQL & ", NVL(SEITOMASTVIEW.NENDO_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.TUUBAN_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.GAKUNEN_CODE_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_KNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_NNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.KAMOKU_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.KOUZA_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.MEIGI_KNAME_O, 0)"
        SSQL = SSQL & ", NVL(SEITOMASTVIEW.TYOUSI_FLG_O, 0)"

        SSQL = SSQL & ", NVL(TENMAST.KIN_NO_N, 0)"
        SSQL = SSQL & ", NVL(TENMAST.SIT_NO_N, 0)"
        SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

        SSQL = SSQL & " FROM "
        SSQL = SSQL & "  KZFMAST.G_MEIMAST"
        '2006/02/14
        'SSQL = SSQL & " ,KZFMAST.G_SCHMAST"
        SSQL = SSQL & " ,KZFMAST.GAKMAST1"
        SSQL = SSQL & " ,KZFMAST.HIMOMAST"
        SSQL = SSQL & " ,KZFMAST.SEITOMASTVIEW"
        SSQL = SSQL & " ,KZFMAST.TENMAST"

        SSQL = SSQL & " WHERE "
        '2006/02/14
        'SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S  "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_KBN_M     = G_SCHMAST.FURI_KBN_S  "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    = G_SCHMAST.FURI_DATE_S  "

        'SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
        SSQL = SSQL & "  G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
        SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G(+)  "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = HIMOMAST.GAKKOU_CODE_H(+)  "
        SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = HIMOMAST.GAKUNEN_CODE_H(+)  "
        SSQL = SSQL & " AND G_MEIMAST.HIMOKU_ID_M    = HIMOMAST.HIMOKU_ID_H(+)  "
        SSQL = SSQL & " AND SUBSTR(G_MEIMAST.SEIKYU_TUKI_M,5,2)  = HIMOMAST.TUKI_NO_H(+)  "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMASTVIEW.GAKKOU_CODE_O(+)  "
        SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMASTVIEW.NENDO_O(+)  "
        SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMASTVIEW.TUUBAN_O(+)  "
        SSQL = SSQL & " AND '04'                     = SEITOMASTVIEW.TUKI_NO_O(+)  "

        SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N(+) "
        SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N(+) "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  =" & "'" & STR学校コード & "'"
        '2006/10/13 請求金額が0円のデータは出力しないように修正
        SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

        If chk振替不能明細合算出力.Checked = True And STR初振日 <> "" Then
            '再振日の入力で合算出力の場合
            SSQL = SSQL & " AND (G_MEIMAST.FURI_DATE_M = '" & strFURI_DATE_Y & strFURI_DATE_M & strFURI_DATE_D & "'"
            SSQL = SSQL & "  OR  G_MEIMAST.FURI_DATE_M = '" & STR初振日 & "'" & ") "
        Else
            '初振日または再振日の入力で合算出力なしの場合
            SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & strFURI_DATE_Y & strFURI_DATE_M & strFURI_DATE_D & "'"
        End If

        '2006/10/20 入出金区分を検索条件に追加
        Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
            Case "入金"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='2'"
            Case "出金"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='3'"
        End Select

        '2006/10/12 合計欄に振替済件数・金額も出力するように変更
        'SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <> 0 "

        SSQL = SSQL & " ORDER BY "
        Select Case (STR帳票ソート順)
            Case "0"
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
                SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
            Case "1"
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
            Case Else
                SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
                SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '明細マスタには名義人しか無い 2006/10/11
                SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
                SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
        End Select

        PFUC_SQLQuery_振替不能結果 = SSQL

        'Debug.WriteLine("SSQL=" & SSQL)

    End Function
    Private Function PFUC_SQLQuery_店別集計() As String
        Dim SSQL As String = ""

        PFUC_SQLQuery_店別集計 = ""

        SSQL = SSQL & " SELECT "
        SSQL = SSQL & "NVL(GAKMAST1.GAKKOU_NNAME_G,0), "
        SSQL = SSQL & "NVL(TENMAST.SIT_NNAME_N,0), "
        SSQL = SSQL & "NVL(G_MEIMAST.GAKKOU_CODE_M,0), "
        SSQL = SSQL & "NVL(G_MEIMAST.TKIN_NO_M,0), "
        SSQL = SSQL & "NVL(G_MEIMAST.FURIKETU_CODE_M,0), "
        SSQL = SSQL & "NVL(G_MEIMAST.FURI_DATE_M,0), "
        SSQL = SSQL & "NVL(G_MEIMAST.SEIKYU_KIN_M,0), "
        SSQL = SSQL & "GAKMAST1.GAKUNEN_CODE_G, "
        SSQL = SSQL & "NVL(GAKMAST2.TSIT_NO_T,0), "
        SSQL = SSQL & "NVL(TENMAST_1.SIT_NNAME_N,0), "
        SSQL = SSQL & "GAKMAST2.TKIN_NO_T, "
        SSQL = SSQL & "G_MEIMAST.TSIT_NO_M "
        SSQL = SSQL & "FROM   "
        SSQL = SSQL & "KZFMAST.G_MEIMAST G_MEIMAST, "
        SSQL = SSQL & "KZFMAST.TENMAST TENMAST, "
        SSQL = SSQL & "KZFMAST.GAKMAST1 GAKMAST1, "
        SSQL = SSQL & "KZFMAST.GAKMAST2 GAKMAST2, "
        SSQL = SSQL & "KZFMAST.TENMAST TENMAST_1, "
        SSQL = SSQL & "KZFMAST.G_SCHMAST G_SCHMAST "
        SSQL = SSQL & "WHERE  "
        SSQL = SSQL & "((G_MEIMAST.TKIN_NO_M=TENMAST.KIN_NO_N) AND (G_MEIMAST.TSIT_NO_M=TENMAST.SIT_NO_N)) AND "
        SSQL = SSQL & "((G_MEIMAST.FURI_DATE_M=G_SCHMAST.FURI_DATE_S) AND (G_MEIMAST.FURI_KBN_M=G_SCHMAST.FURI_KBN_S)) AND "
        SSQL = SSQL & "(G_MEIMAST.GAKKOU_CODE_M=GAKMAST1.GAKKOU_CODE_G) AND (GAKMAST1.GAKKOU_CODE_G=GAKMAST2.GAKKOU_CODE_T) AND "
        SSQL = SSQL & "((GAKMAST2.TSIT_NO_T=TENMAST_1.SIT_NO_N (+)) AND (GAKMAST2.TKIN_NO_T=TENMAST_1.KIN_NO_N (+))) AND "
        SSQL = SSQL & "GAKMAST1.GAKUNEN_CODE_G=1 "
        Select Case (Trim(txtGAKKOU_CODE.Text))
            Case Is <> "9999999999"
                '指定学校コード
                SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  ='" & Trim(txtGAKKOU_CODE.Text) & "'"
        End Select
        '振替日
        SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"

        '契約金融機関
        '2006/10/12　自金庫データ以外をその他欄に集計して印字するように変更
        'SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M = '" & STR自金庫コード_INI & "'"

        '2006/10/20 入出金区分を検索条件に追加
        Select Case (Mid(cmbFURIKAEBI.Text, 12, 2))
            Case "入金"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='2'"
            Case "出金"
                SSQL = SSQL & " AND"
                SSQL = SSQL & " G_MEIMAST.FURI_KBN_M ='3'"
        End Select

        '2006/10/11 条件に不能フラグ追加
        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S  "
        SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = G_SCHMAST.FURI_DATE_S  "
        SSQL = SSQL & " AND G_SCHMAST.FUNOU_FLG_S = '1'  "
        '2006/10/13 請求金額が0円のデータは出力しないように修正
        SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

        SSQL = SSQL & "ORDER BY "
        SSQL = SSQL & "G_MEIMAST.GAKKOU_CODE_M ASC , G_MEIMAST.FURI_DATE_M ASC"


        PFUC_SQLQuery_店別集計 = SSQL

        'Debug.WriteLine("SSQL=" & SSQL)

    End Function
#End Region

End Class
