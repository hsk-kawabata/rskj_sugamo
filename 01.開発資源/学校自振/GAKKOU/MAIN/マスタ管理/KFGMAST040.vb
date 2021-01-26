Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Imports System.Data.OracleClient
Public Class KFGMAST040

    '費目マスタ月別金額退避
    Private Str_Spread(11, 14) As String
    Private int合計金額 As Integer
    Private intRC As Integer

    '口座チェック用
    Private strKIGYO_CODE As String
    Private strFURI_CODE As String
    '読込用DB
    Private MainDB As CASTCommon.MyOracle
    '2017/02/22 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
    Private intMaxHimokuCnt As Integer = CInt(IIf(STR_HIMOKU_PTN = "1", 15, 10))
    '2017/02/22 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ END
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST040", "生徒マスタメンテナンス画面")
    Private Const msgTitle As String = "生徒マスタメンテナンス画面(KFGMAST040)"

    ' 2010/09/16 TASK)saitou 生徒マスタチェックリスト出力対応 -------------------->
    Private strCSV_FILE_NAME As String
    '2017/04/05 saitou 東春信金(RSV2標準) DEL 生徒マスタメンテ帳票改善 ---------------------------------------- START
    'Private Const SeitoMastMaxColumns As Integer = 65
    '2017/04/05 saitou 東春信金(RSV2標準) DEL ----------------------------------------------------------------- END
    ' 2010/09/16 TASK)saitou 生徒マスタチェックリスト出力対応 --------------------<
    '2017/04/05 saitou 東春信金(RSV2標準) ADD 生徒マスタメンテ帳票改善 ---------------------------------------- START
    '生徒マスタメンテ帳票出力に必要なキー項目を格納する構造体
    Private Structure strcSeitomastMenteListInfo
        Dim GakkouCode As String
        Dim Nendo As String
        Dim Tuuban As String
    End Structure
    Private SeitomastMenteListInfo As strcSeitomastMenteListInfo

    Private Structure strcSeitomastMenteUpdateListInfo
        Dim GakkouCode As String
        Dim Nendo As String
        Dim Tuuban As String
        Dim GakunenCode As String
        Dim ClassCode As String
        Dim SeitoNo As String
        Dim GakkouName As String
    End Structure
    Private SeitomastMenteUpdateListInfo As strcSeitomastMenteUpdateListInfo

    Private dtBeforeSeitomast As DataTable
    Private dtAfterSeitomast As DataTable
    '2017/04/05 saitou 東春信金(RSV2標準) ADD ----------------------------------------------------------------- END
    '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- START
    Private STR_TYOUSIUMU_TXT As String = "KFGMAST040_長子有無フラグ.TXT"
    '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- END

#Region " Form Load "
    Private Sub KFGMAST040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            STR_SYORI_NAME = "生徒マスタメンテナンス画面"
            MainLOG = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            'テキストファイルからコンボボックス設定
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_SEIBETU_TXT, cmbSEIBETU) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbSEIBETU)")
                MessageBox.Show(String.Format(MSG0013E, "性別"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_FURIKAE_TXT, cmbFURIKAE) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbFURIKAE)")
                MessageBox.Show(String.Format(MSG0013E, "振替方法"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbKAMOKU)")
                MessageBox.Show(String.Format(MSG0013E, "科目"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_SINKYU_TXT, cmbSINKYU_KBN) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbSINKYU_KBN)")
                MessageBox.Show(String.Format(MSG0013E, "進級区分"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_KAIYAKU_TXT, cmbKAIYAKU_FLG) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbKAIYAKU_FLG)")
                MessageBox.Show(String.Format(MSG0013E, "解約区分"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- START
            '長子有無フラグ
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_TYOUSIUMU_TXT, cmbTYOUSI) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbTYOUSI)")
                MessageBox.Show(String.Format(MSG0013E, "長子有無フラグ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            '2017/11/28 タスク）西野 ADD (標準版修正(№177)) -------------------- END

            Dim ArrayDGV As CustomDataGridView() = { _
            CustomDataGridView1, _
            CustomDataGridView2, _
            CustomDataGridView3, _
            CustomDataGridView4, _
            CustomDataGridView5, _
            CustomDataGridView6, _
            CustomDataGridView7, _
            CustomDataGridView8, _
            CustomDataGridView9, _
            CustomDataGridView10, _
            CustomDataGridView11, _
            CustomDataGridView12}

            For MonthCnt As Integer = 0 To ArrayDGV.Length - 1 Step 1
                Dim CmbCol As New DataGridViewComboBoxColumn
                Dim MonthCntWide As String = StrConv((MonthCnt + 1).ToString, VbStrConv.Wide)

                CmbCol.Items.Add("一律")
                CmbCol.Items.Add("個別")

                CmbCol.DataPropertyName = "請求方法" & MonthCntWide
                ArrayDGV(MonthCnt).Columns.Insert(ArrayDGV(MonthCnt).Columns("請求方法" & MonthCntWide).Index, CmbCol)
                ArrayDGV(MonthCnt).Columns.Remove("請求方法" & MonthCntWide)
                CmbCol.Name = "請求方法"
                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                ArrayDGV(MonthCnt).Rows.Add(intMaxHimokuCnt)
                'ArrayDGV(MonthCnt).Rows.Add(10)
                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
            Next

            '2017/05/09 タスク）西野 ADD 標準版修正（費目設定の一括変更対応）---------------------- START
            CustomDataGridView13.Rows.Add(intMaxHimokuCnt)
            '2017/05/09 タスク）西野 ADD 標準版修正（費目設定の一括変更対応）---------------------- END

            'コンボボックスの先頭に位置づけ

            cmbSEIBETU.SelectedIndex = 0 '性別コンボ
            cmbFURIKAE.SelectedIndex = 0   '振替方法コンボ
            cmbKAMOKU.SelectedIndex = 0  '科目コンボ
            cmbSINKYU_KBN.SelectedIndex = 0   '進級区分コンボ
            cmbKAIYAKU_FLG.SelectedIndex = 0  '解約区分コンボ
            cmbTYOUSI.SelectedIndex = 0    '長子情報コンボ

            '入力禁止制御
            btnUPDATE.Enabled = False
            btnDelete.Enabled = False

            btnKOUZA_CHK.Visible = True
            lblKOUZA_CHK.Visible = True

            '2017/05/11 タスク）西野 ADD 標準版修正（長子設定項目の表示／非表示制御）---------------------- START
            If STR_TYOUSI_KBN = "0" Then
                '長子項目を非表示にする
                Label28.Visible = False
                cmbTYOUSI.Visible = False
                btnTyousi.Visible = False
            End If
            '2017/05/11 タスク）西野 ADD 標準版修正（長子設定項目の表示／非表示制御）---------------------- END

            'ロードで開ける
            MainDB = New CASTCommon.MyOracle
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " Button Click "
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click

        '新規登録処理
        Try
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")
            Dim Orareader As CASTCommon.MyOracleReader = Nothing

            Try
                '入力値チェック
                If fn_CheckEntry(0) = False Then
                    Exit Sub
                End If

                '重複チェック
                '新入生マスタからも重複チェックを行なう
                Dim SQL As New StringBuilder(128)
                SQL.Append(" SELECT * FROM SEITOMAST S1,SEITOMAST2 S2")
                SQL.Append(" WHERE (S1.GAKKOU_CODE_O = '" & Trim(txtGAKKOU_CODE.Text) & "'")    '生徒マスタ：学校コード
                SQL.Append(" AND S1.NENDO_O = '" & Trim(txtNENDO.Text) & "' ")                  '生徒マスタ：入学年度
                SQL.Append(" AND S1.TUUBAN_O = " & CInt(txtTUUBAN.Text) & ")")                  '生徒マスタ：通番
                SQL.Append(" OR (S2.GAKKOU_CODE_O = '" & Trim(txtGAKKOU_CODE.Text) & "'")       '新入生マスタ：学校コード
                SQL.Append(" AND S2.NENDO_O = '" & Trim(txtNENDO.Text) & "'")                   '新入生マスタ：入学年度
                SQL.Append(" AND S2.TUUBAN_O = " & CInt(txtTUUBAN.Text) & ")")                  '新入生マスタ：通番

                Orareader = New CASTCommon.MyOracleReader(MainDB)

                If Orareader.DataReader(SQL) Then
                    MessageBox.Show(G_MSG0064W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()

                    Orareader.Close()
                    Orareader = Nothing
                    Exit Sub
                End If

                Orareader.Close()
                Orareader = Nothing

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)例外エラー", "失敗", ex.Message)
            Finally
                If Not Orareader Is Nothing Then
                    Orareader.Close()
                    Orareader = Nothing
                End If
            End Try


            '確認メッセージを表示
            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '2017/04/05 saitou 東春信金(RSV2標準) ADD 生徒マスタメンテ帳票改善 ---------------------------------------- START
            'キー項目格納
            Me.SeitomastMenteListInfo.GakkouCode = Me.txtGAKKOU_CODE.Text
            Me.SeitomastMenteListInfo.Nendo = Me.txtNENDO.Text
            Me.SeitomastMenteListInfo.Tuuban = Me.txtTUUBAN.Text
            '2017/04/05 saitou 東春信金(RSV2標準) ADD ----------------------------------------------------------------- END

            'DB操作結果フラグ
            Dim bret As Boolean = False

            Try
                'TRANS
                Call GFUNC_EXECUTESQL_TRANS("", 0)

                If fn_INSERTSEITOMAST(CustomDataGridView1, 4) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView2, 5) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView3, 6) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView4, 7) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView5, 8) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView6, 9) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView7, 10) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView8, 11) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView9, 12) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView10, 1) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView11, 2) = False Then
                    Exit Try
                End If

                If fn_INSERTSEITOMAST(CustomDataGridView12, 3) = False Then
                    Exit Try
                End If

                '2017/04/05 saitou 東春信金(RSV2標準) UPD 生徒マスタメンテ帳票改善 ---------------------------------------- START
                '元に戻す
                '画面クリア
                Call fn_FormInitialize()
                '入力項目制御
                txtGAKKOU_CODE.Enabled = True
                txtNENDO.Enabled = True
                txtTUUBAN.Enabled = True
                txtGAKUNEN_CODE.Enabled = True
                txtCLASS_CODE.Enabled = True
                txtSEITO_NO.Enabled = True

                cmbKana.Enabled = True
                cmbGakkouName.Enabled = True

                '入力禁止ボタン制御
                btnAdd.Enabled = True
                btnUPDATE.Enabled = False
                btnDelete.Enabled = False
                btnFind.Enabled = True
                btnEraser.Enabled = True
                btnEnd.Enabled = True
                btnTuuban.Enabled = True
                txtTUUBAN.Focus()

                '' 2010/09/16 TASK)saitou 生徒マスタチェックリスト出力対応 -------------------->
                '' 画面項目制御を生徒マスタチェックリスト出力後に行うようにする（項目が取得できず印刷できないため）
                ' ''画面クリア
                ''Call fn_FormInitialize()
                ' ''入力項目制御
                ''txtGAKKOU_CODE.Enabled = True
                ''txtNENDO.Enabled = True
                ''txtTUUBAN.Enabled = True
                ''txtGAKUNEN_CODE.Enabled = True
                ''txtCLASS_CODE.Enabled = True
                ''txtSEITO_NO.Enabled = True

                ''cmbKana.Enabled = True
                ''cmbGakkouName.Enabled = True

                ' ''入力禁止ボタン制御
                ''btnAdd.Enabled = True
                ''btnUPDATE.Enabled = False
                ''btnDelete.Enabled = False
                ''btnFind.Enabled = True
                ''btnEraser.Enabled = True
                ''btnEnd.Enabled = True
                ''btnTuuban.Enabled = True
                ''txtTUUBAN.Focus()
                '' 2010/09/16 TASK)saitou 生徒マスタチェックリスト出力対応 --------------------<
                '2017/04/05 saitou 東春信金(RSV2標準) UPD ----------------------------------------------------------------- END

                bret = True

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)例外エラー", "失敗", ex.Message)
            Finally
                If bret Then
                    'COMMIT
                    Call GFUNC_EXECUTESQL_TRANS("", 2)
                    MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' 2010/09/16 TASK)saitou 生徒マスタチェックリスト出力対応 -------------------->
                    ' 印刷を実行する
                    If fn_CreateCSV_ADDDEL("0") Then
                        Call fn_Print(0)
                    End If

                    '2017/04/05 saitou 東春信金(RSV2標準) DEL 生徒マスタメンテ帳票改善 ---------------------------------------- START
                    ''画面クリア
                    'Call fn_FormInitialize()
                    ''入力項目制御
                    'txtGAKKOU_CODE.Enabled = True
                    'txtNENDO.Enabled = True
                    'txtTUUBAN.Enabled = True
                    'txtGAKUNEN_CODE.Enabled = True
                    'txtCLASS_CODE.Enabled = True
                    'txtSEITO_NO.Enabled = True

                    'cmbKana.Enabled = True
                    'cmbGakkouName.Enabled = True

                    ''入力禁止ボタン制御
                    'btnAdd.Enabled = True
                    'btnUPDATE.Enabled = False
                    'btnDelete.Enabled = False
                    'btnFind.Enabled = True
                    'btnEraser.Enabled = True
                    'btnEnd.Enabled = True
                    'btnTuuban.Enabled = True
                    'txtTUUBAN.Focus()
                    '2017/04/05 saitou 東春信金(RSV2標準) DEL ----------------------------------------------------------------- END
                    ' 2010/09/16 TASK)saitou 生徒マスタチェックリスト出力対応 --------------------<

                Else
                    'ROLLBACK
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    MessageBox.Show(String.Format(MSG0002E, "登録"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Try
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")
            LW.ToriCode = "000000000000"
        End Try

    End Sub
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUPDATE.Click

        '更新変更処理
        Try
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")

            '入力値チェック
            If fn_CheckEntry(1) = False Then
                Exit Sub
            End If

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '2017/04/05 saitou 東春信金(RSV2標準) ADD 生徒マスタメンテ帳票改善 ---------------------------------------- START
            'キー項目格納
            Me.SeitomastMenteListInfo.GakkouCode = Me.txtGAKKOU_CODE.Text
            Me.SeitomastMenteListInfo.Nendo = Me.txtNENDO.Text
            Me.SeitomastMenteListInfo.Tuuban = Me.txtTUUBAN.Text
            '2017/04/05 saitou 東春信金(RSV2標準) ADD ----------------------------------------------------------------- END

            Dim bret As Boolean = False

            '2017/04/05 saitou 東春信金(RSV2標準) UPD 生徒マスタメンテ帳票改善 ---------------------------------------- START
            '更新用データテーブルの初期化
            Me.CreateSeitomastDataTable()
            Dim beforeDataSetFlg As Boolean = False
            Dim afterDataSetFlg As Boolean = False

            '更新リスト用の情報格納
            Me.SeitomastMenteUpdateListInfo.GakkouCode = Me.txtGAKKOU_CODE.Text
            Me.SeitomastMenteUpdateListInfo.Nendo = Me.txtNENDO.Text
            Me.SeitomastMenteUpdateListInfo.Tuuban = Me.txtTUUBAN.Text
            Me.SeitomastMenteUpdateListInfo.GakunenCode = Me.txtGAKUNEN_CODE.Text
            Me.SeitomastMenteUpdateListInfo.ClassCode = Me.txtCLASS_CODE.Text
            Me.SeitomastMenteUpdateListInfo.SeitoNo = Me.txtSEITO_NO.Text
            Me.SeitomastMenteUpdateListInfo.GakkouName = Me.lab学校名.Text.Trim

            '' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 -------------------->
            'Dim beforeSEITOMAST(,) As String = Nothing
            'Dim afterSEITOMAST(,) As String = Nothing
            '' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 --------------------<
            '2017/04/05 saitou 東春信金(RSV2標準) UPD ----------------------------------------------------------------- END

            Try
                '2017/04/05 saitou 東春信金(RSV2標準) UPD 生徒マスタメンテ帳票改善 ---------------------------------------- START
                beforeDataSetFlg = Me.fn_GetSEITOMAST_PrnCSVUpd(True)
                '' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 -------------------->
                '' 生徒マスタが更新される前に現在の情報を配列に格納しておく
                'beforeDataSetFlg = fn_GetSEITOMAST_PrnCSVUpd(beforeSEITOMAST)
                '' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 --------------------<
                '2017/04/05 saitou 東春信金(RSV2標準) UPD ----------------------------------------------------------------- END

                'トランザクション開始
                Call GFUNC_EXECUTESQL_TRANS("", 0)

                If fn_UPDATESEITOMAST(CustomDataGridView1, 4) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView2, 5) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView3, 6) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView4, 7) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView5, 8) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView6, 9) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView7, 10) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView8, 11) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView9, 12) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView10, 1) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView11, 2) = False Then
                    Exit Try
                End If

                If fn_UPDATESEITOMAST(CustomDataGridView12, 3) = False Then
                    Exit Try
                End If

                '2017/04/05 saitou 東春信金(RSV2標準) UPD 生徒マスタメンテ帳票改善 ---------------------------------------- START
                '元に戻す
                '画面クリア 2007/10/31
                Call fn_FormInitialize()
                '入力項目制御
                txtGAKKOU_CODE.Enabled = True
                txtNENDO.Enabled = True
                txtTUUBAN.Enabled = True
                txtGAKUNEN_CODE.Enabled = True
                txtCLASS_CODE.Enabled = True
                txtSEITO_NO.Enabled = True

                cmbKana.Enabled = True
                cmbGakkouName.Enabled = True

                '入力禁止ボタン制御
                btnAdd.Enabled = True
                btnUPDATE.Enabled = False
                btnDelete.Enabled = False
                btnFind.Enabled = True
                btnEraser.Enabled = True
                btnEnd.Enabled = True
                btnTuuban.Enabled = True
                txtTUUBAN.Focus()

                '' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 -------------------->
                '' 画面をクリアされると画面項目を取得できないため、処理を移動
                ' ''画面クリア 2007/10/31
                ''Call fn_FormInitialize()
                ' ''入力項目制御
                ''txtGAKKOU_CODE.Enabled = True
                ''txtNENDO.Enabled = True
                ''txtTUUBAN.Enabled = True
                ''txtGAKUNEN_CODE.Enabled = True
                ''txtCLASS_CODE.Enabled = True
                ''txtSEITO_NO.Enabled = True

                ''cmbKana.Enabled = True
                ''cmbGakkouName.Enabled = True

                ' ''入力禁止ボタン制御
                ''btnAdd.Enabled = True
                ''btnUPDATE.Enabled = False
                ''btnDelete.Enabled = False
                ''btnFind.Enabled = True
                ''btnEraser.Enabled = True
                ''btnEnd.Enabled = True
                ''btnTuuban.Enabled = True
                ''txtTUUBAN.Focus()
                '' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 --------------------<
                '2017/04/05 saitou 東春信金(RSV2標準) UPD ----------------------------------------------------------------- END

                bret = True

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)例外エラー", "失敗", ex.Message)
            Finally
                If bret Then
                    'COMMIT
                    Call GFUNC_EXECUTESQL_TRANS("", 2)
                    MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    '2017/04/05 saitou 東春信金(RSV2標準) UPD 生徒マスタメンテ帳票改善 ---------------------------------------- START
                    If beforeDataSetFlg = True Then
                        afterDataSetFlg = Me.fn_GetSEITOMAST_PrnCSVUpd(False)
                        If afterDataSetFlg = True Then
                            '生徒マスタチェックリスト（更新）のCSVデータを作成
                            If fn_CreateCSV_UPD() = True Then
                                Call fn_Print(1)
                            End If
                        End If
                    End If

                    '' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 -------------------->
                    'If beforeDataSetFlg Then
                    '    afterDataSetFlg = fn_GetSEITOMAST_PrnCSVUpd(afterSEITOMAST)
                    '    If afterDataSetFlg Then
                    '        '生徒マスタチェックリスト（更新）のCSVデータを作成
                    '        If fn_CreateCSV_UPD(beforeSEITOMAST, afterSEITOMAST) Then
                    '            Call fn_Print(1)
                    '        End If

                    '    End If
                    'End If

                    'Call fn_FormInitialize()
                    ''入力項目制御
                    'txtGAKKOU_CODE.Enabled = True
                    'txtNENDO.Enabled = True
                    'txtTUUBAN.Enabled = True
                    'txtGAKUNEN_CODE.Enabled = True
                    'txtCLASS_CODE.Enabled = True
                    'txtSEITO_NO.Enabled = True

                    'cmbKana.Enabled = True
                    'cmbGakkouName.Enabled = True

                    ''入力禁止ボタン制御
                    'btnAdd.Enabled = True
                    'btnUPDATE.Enabled = False
                    'btnDelete.Enabled = False
                    'btnFind.Enabled = True
                    'btnEraser.Enabled = True
                    'btnEnd.Enabled = True
                    'btnTuuban.Enabled = True
                    '' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 --------------------<
                    '2017/04/05 saitou 東春信金(RSV2標準) UPD ----------------------------------------------------------------- END

                Else
                    'ROLLBACK
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Try
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
            LW.ToriCode = "000000000000"
        End Try
    End Sub
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        '削除ボタン
        Try
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)開始", "成功", "")

            '入力値チェック
            If fn_CheckEntry(1) = False Then
                Exit Sub
            End If

            '確認メッセージを表示
            If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '2017/04/05 saitou 東春信金(RSV2標準) ADD 生徒マスタメンテ帳票改善 ---------------------------------------- START
            'キー項目格納
            Me.SeitomastMenteListInfo.GakkouCode = Me.txtGAKKOU_CODE.Text
            Me.SeitomastMenteListInfo.Nendo = Me.txtNENDO.Text
            Me.SeitomastMenteListInfo.Tuuban = Me.txtTUUBAN.Text
            '2017/04/05 saitou 東春信金(RSV2標準) ADD ----------------------------------------------------------------- END

            Dim bret As Boolean = False

            ' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 -------------------->
            Dim blCsvFlg As Boolean = False         '印刷用CSV作成フラグ
            ' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 --------------------<

            Try
                ' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 -------------------->
                ' SEITOMASTが削除される前にCSVを作成しておく
                blCsvFlg = fn_CreateCSV_ADDDEL("2")
                ' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応 --------------------<

                'トランザクション開始
                Call GFUNC_EXECUTESQL_TRANS("", 0)

                Dim SQL As String = ""
                SQL = " DELETE  FROM SEITOMAST"
                SQL &= " WHERE"
                SQL &= " GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'"
                SQL &= " AND"
                SQL &= " NENDO_O ='" & Trim(txtNENDO.Text) & "'"
                SQL &= " AND"
                SQL &= " TUUBAN_O = " & CInt(txtTUUBAN.Text)

                If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                    Exit Try
                End If

                '画面の初期化
                Call fn_FormInitialize()

                '入力項目制御
                cmbKana.Enabled = True
                cmbGakkouName.Enabled = True
                txtGAKKOU_CODE.Enabled = True
                txtNENDO.Enabled = True
                txtTUUBAN.Enabled = True
                txtSEITO_NO.Enabled = True
                txtGAKUNEN_CODE.Enabled = True
                txtCLASS_CODE.Enabled = True

                '入力ボタン制御
                btnAdd.Enabled = True
                btnUPDATE.Enabled = False
                btnDelete.Enabled = False
                btnFind.Enabled = True
                btnEraser.Enabled = True
                btnEnd.Enabled = True
                btnTuuban.Enabled = True

                bret = True

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)例外エラー", "失敗", ex.Message)
            Finally
                If bret Then
                    'COMMIT
                    Call GFUNC_EXECUTESQL_TRANS("", 2)
                    MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応-------------------->
                    If blCsvFlg Then
                        '印刷処理実行
                        Call fn_Print(2)
                    End If
                    ' 2010/09/17 TASK)saitou 生徒マスタチェックリスト出力対応--------------------<
                Else
                    'ROLLBACK
                    Call GFUNC_EXECUTESQL_TRANS("", 3)
                    MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Try
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)終了", "成功", "")
            LW.ToriCode = "00000000000"
        End Try
    End Sub
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click

        Dim intSpdRow As Integer
        Dim dblKingaku_Goukei As Double
        Dim strHimoku As String = ""

        '参照ボタン
        Try
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            Dim OraReader As CASTCommon.MyOracleReader = Nothing
            '2017/05/11 タスク）西野 ADD 標準版修正（生徒マスタの検索２重対応）---------------------- START
            Dim OraReader_Cnt As CASTCommon.MyOracleReader = Nothing
            '2017/05/11 タスク）西野 ADD 標準版修正（生徒マスタの検索２重対応）---------------------- END

            Try
                Dim SQL As New StringBuilder(128)
                '2017/05/11 タスク）西野 ADD 標準版修正（生徒マスタの検索２重対応）---------------------- START
                Dim SQL_CNT As New StringBuilder(128)
                '2017/05/11 タスク）西野 ADD 標準版修正（生徒マスタの検索２重対応）---------------------- END

                '関数内で入力チェックし、OKなら検索のSQLを返す
                If fn_GetSELECTSEITOMASTSQL(SQL) = False Then
                    Exit Sub
                End If

                OraReader = New CASTCommon.MyOracleReader(MainDB)

                '生徒マスタ存在チェック
                If Not OraReader.DataReader(SQL) Then
                    MessageBox.Show(G_MSG0065W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                Else
                    '2017/05/11 タスク）西野 ADD 標準版修正（生徒マスタの検索２重対応）---------------------- START
                    '照会件数を取得し、２件以上の場合は再検索を促すメッセージを表示する
                    SQL_CNT = SQL.Replace("*", "COUNT(*) CNT")
                    SQL_CNT.Append(" AND TUKI_NO_O='04'")
                    OraReader_Cnt = New CASTCommon.MyOracleReader(MainDB)
                    If Not OraReader_Cnt.DataReader(SQL_CNT) Then
                        MessageBox.Show(G_MSG0065W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtGAKKOU_CODE.Focus()
                        Exit Sub
                    Else
                        '照会件数取得
                        Dim intCount As Integer = OraReader_Cnt.GetInt("CNT")
                        OraReader_Cnt.Close()

                        If intCount > 1 Then
                            'メッセージ生成
                            Dim MSG_INFO As String = ""
                            While OraReader.EOF = False
                                If OraReader.GetString("TUKI_NO_O") = "04" Then
                                    MSG_INFO &= "入学年度=" & OraReader.GetItem("NENDO_O")
                                    MSG_INFO &= "　通番=" & OraReader.GetItem("TUUBAN_O").ToString.Trim.PadLeft(4, "0"c)
                                    MSG_INFO &= vbCrLf
                                End If
                                OraReader.NextRead()
                            End While
                            OraReader.Close()

                            MessageBox.Show(String.Format(G_MSG0108W, "生徒", MSG_INFO), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtNENDO.Focus()
                            Exit Sub
                        End If
                    End If
                    '2017/05/11 タスク）西野 ADD 標準版修正（生徒マスタの検索２重対応）---------------------- END

                    '費目情報４月から翌３月のクリア
                    cmbHIMOKU.Text = ""
                    cmbHIMOKU.Items.Clear()
                    Call Sub_Himoku_Initialize()
                End If

                While OraReader.EOF = False
                    With OraReader
                        '入学年度
                        txtNENDO.Text = Trim(.GetItem("NENDO_O"))
                        '通番
                        txtTUUBAN.Text = .GetItem("TUUBAN_O").ToString.Trim.PadLeft(4, "0"c)
                        '学年コード
                        txtGAKUNEN_CODE.Text = Trim(.GetItem("GAKUNEN_CODE_O"))
                        'クラスコード
                        txtCLASS_CODE.Text = Trim(.GetItem("CLASS_CODE_O"))
                        '生徒番号
                        txtSEITO_NO.Text = Trim(.GetItem("SEITO_NO_O"))
                        '性別
                        cmbSEIBETU.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEIBETU_TXT, Trim(.GetItem("SEIBETU_O")))
                        '生徒名(カナ)
                        txtSEITO_KNAME.Text = Trim(.GetItem("SEITO_KNAME_O"))
                        '生徒名(漢字)
                        txtSEITO_NNAME.Text = ConvDBNullToString(.GetItem("SEITO_NNAME_O"))
                        '振替？
                        cmbFURIKAE.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_FURIKAE_TXT, Trim(.GetItem("FURIKAE_O")))
                        '取扱金融機関コード
                        txtTKIN_NO.Text = ConvDBNullToString(.GetItem("TKIN_NO_O"))
                        '取扱支店コード
                        txtTSIT_NO.Text = ConvDBNullToString(.GetItem("TSIT_NO_O"))
                        '科目
                        cmbKAMOKU.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, Trim(.GetItem("KAMOKU_O")))
                        '口座番号
                        txtKOUZA.Text = ConvDBNullToString(.GetItem("KOUZA_O"))
                        '口座名義人カナ
                        txtMEIGI_KNAME.Text = Trim(.GetItem("MEIGI_KNAME_O"))
                        '保護者漢字
                        txtMEIGI_NNAME.Text = ConvDBNullToString(.GetItem("MEIGI_NNAME_O"))
                        '住所漢字
                        txtKEIYAKU_NJYU.Text = ConvDBNullToString(.GetItem("KEIYAKU_NJYU_O"))
                        '進級区分
                        cmbSINKYU_KBN.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_SINKYU_TXT, Trim(.GetItem("SINKYU_KBN_O")))
                        '電話番号
                        txtKEIYAKU_DENWA.Text = ConvDBNullToString(.GetItem("KEIYAKU_DENWA_O"))
                        '解約フラグ
                        cmbKAIYAKU_FLG.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_KAIYAKU_TXT, Trim(.GetItem("KAIYAKU_FLG_O")))
                        '費目情報コンボの設定
                        strHimoku = .GetItem("HIMOKU_ID_O")

                        '長子情報の設定
                        '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- START
                        cmbTYOUSI.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_TYOUSIUMU_TXT, Trim(.GetItem("TYOUSI_FLG_O")))
                        Select Case cmbTYOUSI.SelectedIndex
                            Case 0
                                btnTyousi.Enabled = False
                                str長子学校コード = ""
                                str長子入学年度 = ""
                                str長子通番 = ""
                                str長子学年 = ""
                                str長子クラス = ""
                                str長子生徒番号 = ""
                            Case 1
                                btnTyousi.Enabled = True

                                str長子学校コード = .GetItem("GAKKOU_CODE_O")
                                str長子入学年度 = .GetItem("TYOUSI_NENDO_O")
                                str長子通番 = .GetItem("TYOUSI_TUUBAN_O")
                                str長子学年 = .GetItem("TYOUSI_GAKUNEN_O")
                                str長子クラス = .GetItem("TYOUSI_CLASS_O")
                                str長子生徒番号 = .GetItem("TYOUSI_SEITONO_O")
                        End Select
                        'Select Case Trim(.GetItem("TYOUSI_FLG_O"))
                        '    Case 0
                        '        cmbTYOUSI.SelectedIndex = 0
                        '        btnTyousi.Enabled = False
                        '        str長子学校コード = ""
                        '        str長子入学年度 = ""
                        '        str長子通番 = ""
                        '        str長子学年 = ""
                        '        str長子クラス = ""
                        '        str長子生徒番号 = ""
                        '    Case 1
                        '        cmbTYOUSI.SelectedIndex = 1
                        '        btnTyousi.Enabled = True

                        '        str長子学校コード = .GetItem("GAKKOU_CODE_O")
                        '        str長子入学年度 = .GetItem("TYOUSI_NENDO_O")
                        '        str長子通番 = .GetItem("TYOUSI_TUUBAN_O")
                        '        str長子学年 = .GetItem("TYOUSI_GAKUNEN_O")
                        '        str長子クラス = .GetItem("TYOUSI_CLASS_O")
                        '        str長子生徒番号 = .GetItem("TYOUSI_SEITONO_O")
                        'End Select
                        '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- END

                        '2017/04/28 saitou RSV2 ADD 標準機能追加(更新日) ---------------------------------------- START
                        Dim KousinDate As String = ConvDBNullToString(.GetItem("KOUSIN_DATE_O")).PadLeft(8, "0"c)
                        Me.lblKousinDate.Text = KousinDate.Substring(0, 4) & "年" & KousinDate.Substring(4, 2) & "月" & KousinDate.Substring(6, 2) & "日"
                        '2017/04/28 saitou RSV2 ADD ------------------------------------------------------------- END
                    End With


                    '費目情報金額の設定
                    Dim dgv As DataGridView = Nothing
                    Select Case Format(CInt(OraReader.GetItem("TUKI_NO_O")), "00")
                        Case "04"
                            dgv = CustomDataGridView1
                        Case "05"
                            dgv = CustomDataGridView2
                        Case "06"
                            dgv = CustomDataGridView3
                        Case "07"
                            dgv = CustomDataGridView4
                        Case "08"
                            dgv = CustomDataGridView5
                        Case "09"
                            dgv = CustomDataGridView6
                        Case "10"
                            dgv = CustomDataGridView7
                        Case "11"
                            dgv = CustomDataGridView8
                        Case "12"
                            dgv = CustomDataGridView9
                        Case "01"
                            dgv = CustomDataGridView10
                        Case "02"
                            dgv = CustomDataGridView11
                        Case "03"
                            dgv = CustomDataGridView12
                    End Select

                    dblKingaku_Goukei = 0

                    With dgv
                        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                        For intSpdRow = 0 To intMaxHimokuCnt - 1
                            'For intSpdRow = 0 To 9 '2007/04/04　費目は１０まで
                            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                            If IsDBNull(OraReader.GetItem("HIMOKU_NAME" & Format(intSpdRow + 1, "00") & "_O")) = True Then
                                .Rows(intSpdRow).Cells(1).ReadOnly = True
                                .Rows(intSpdRow).Cells(2).ReadOnly = True
                            Else
                                '費目名
                                .Rows(intSpdRow).Cells(0).Value = OraReader.GetItem("HIMOKU_NAME" & Format(intSpdRow + 1, "00") & "_O")
                                .Rows(intSpdRow).Cells(1).ReadOnly = False

                                '請求方法が個別の場合は金額のセルの入力を可にする
                                Select Case OraReader.GetItem("SEIKYU" & Format(intSpdRow + 1, "00") & "_O")
                                    Case "0"
                                        .Rows(intSpdRow).Cells(1).Value = "一律"
                                        .Rows(intSpdRow).Cells(2).ReadOnly = True
                                    Case "1"
                                        .Rows(intSpdRow).Cells(1).Value = "個別"
                                        .Rows(intSpdRow).Cells(2).ReadOnly = False
                                End Select

                                '金額
                                Str_Spread((CInt(OraReader.GetItem("TUKI_NO_O")) - 1), intSpdRow) = OraReader.GetItem("HIMOKU_KINGAKU" & Format(intSpdRow + 1, "00") & "_O")
                                .Rows(intSpdRow).Cells(2).Value = Format(CDec(OraReader.GetItem("KINGAKU" & Format(intSpdRow + 1, "00") & "_O")), "#,##0")

                                dblKingaku_Goukei += CDbl(OraReader.GetItem("KINGAKU" & Format(intSpdRow + 1, "00") & "_O"))
                            End If
                        Next intSpdRow
                    End With

                    Select Case Format(CInt(OraReader.GetItem("TUKI_NO_O")), "00")
                        Case "04"
                            lblKingaku_4.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "05"
                            lblKingaku_5.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "06"
                            lblKingaku_6.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "07"
                            lblKingaku_7.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "08"
                            lblKingaku_8.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "09"
                            lblKingaku_9.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "10"
                            lblKingaku_10.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "11"
                            lblKingaku_11.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "12"
                            lblKingaku_12.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "01"
                            lblKingaku_1.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "02"
                            lblKingaku_2.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "03"
                            lblKingaku_3.Text = Format(dblKingaku_Goukei, "#,##0")
                    End Select

                    OraReader.NextRead()
                End While

                OraReader.Close()
                OraReader = Nothing

                '2017/05/09 タスク）西野 ADD 標準版修正（費目設定の一括変更対応）---------------------- START
                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    '費目名を設定する
                    CustomDataGridView13.Rows(intRow).Cells(0).Value = CustomDataGridView1.Rows(intRow).Cells(0).Value
                Next
                '2017/05/09 タスク）西野 ADD 標準版修正（費目設定の一括変更対応）---------------------- END

                '入力禁止項目制御
                cmbKana.Enabled = False
                cmbGakkouName.Enabled = False
                txtGAKKOU_CODE.Enabled = False
                txtNENDO.Enabled = False
                txtTUUBAN.Enabled = False

                '入力禁止ボタン制御
                btnAdd.Enabled = False
                btnUPDATE.Enabled = True
                btnDelete.Enabled = True
                btnFind.Enabled = False
                btnEraser.Enabled = True
                btnEnd.Enabled = True
                btnTuuban.Enabled = False '2006/10/19　通番取得ボタン

                '学年名の取得
                Call fn_GetGakunenName()
                'クラス名の取得
                Call fn_GetClassName()
                '金融機関名、支店名の設定
                Call fn_GetKinNameSitName()
                '費目情報コンボの設定
                Call Sub_GetHimokuID()
                Call fn_SetHimokuID(strHimoku)

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)
            Finally
                If Not OraReader Is Nothing Then
                    OraReader.Close()
                    OraReader = Nothing
                End If
                '2017/05/11 タスク）西野 ADD 標準版修正（生徒マスタの検索２重対応）---------------------- START
                If Not OraReader_Cnt Is Nothing Then
                    OraReader_Cnt.Close()
                    OraReader_Cnt = Nothing
                End If
                '2017/05/11 タスク）西野 ADD 標準版修正（生徒マスタの検索２重対応）---------------------- END
            End Try
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
            LW.ToriCode = "0000000000000"
        End Try
    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        '取消ボタン

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")
            Call fn_FormInitialize()

            str長子学校コード = ""
            str長子入学年度 = ""
            str長子通番 = ""
            str長子学年 = ""
            str長子クラス = ""
            str長子生徒番号 = ""

            '入力項目制御
            txtGAKKOU_CODE.Enabled = True
            txtNENDO.Enabled = True
            txtTUUBAN.Enabled = True
            txtGAKUNEN_CODE.Enabled = True
            txtCLASS_CODE.Enabled = True
            txtSEITO_NO.Enabled = True

            cmbKana.Enabled = True
            cmbGakkouName.Enabled = True

            '入力禁止ボタン制御
            btnAdd.Enabled = True
            btnUPDATE.Enabled = False
            btnDelete.Enabled = False
            btnFind.Enabled = True
            btnEraser.Enabled = True
            btnEnd.Enabled = True
            btnTuuban.Enabled = True
            cmbKana.SelectedIndex = -1

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            txtGAKKOU_CODE.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub
    Private Sub btnTuuban_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTuuban.Click

        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtNENDO.Text) <> "" Then
                Dim SQL As New StringBuilder(128)

                SQL.Append(" SELECT * FROM SEITOMAST ")
                SQL.Append(" WHERE GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'")
                SQL.Append(" AND NENDO_O =" & CInt(Trim(txtNENDO.Text)))
                SQL.Append(" ORDER BY TUUBAN_O DESC ")

                Orareader = New CASTCommon.MyOracleReader(MainDB)

                If Not Orareader.DataReader(SQL) Then
                    txtTUUBAN.Text = "0001"
                Else
                    '2019/11/07 saitou UPD 標準版修正（通番取得時の最大値チェック） --------------------  START
                    If Orareader.GetInt("TUUBAN_O") + 1 > 9999 Then
                        MessageBox.Show(G_MSG0109W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Else
                        txtTUUBAN.Text = CStr(Orareader.GetInt("TUUBAN_O") + 1).Trim.PadLeft(4, "0"c)
                    End If
                    'txtTUUBAN.Text = CStr(Orareader.GetItem("TUUBAN_O") + 1).Trim.PadLeft(4, "0"c)
                    '2019/11/07 saitou UPD 標準版修正（通番取得時の最大値チェック） --------------------  END
                End If

                Orareader.Close()
                Orareader = Nothing

                txtTUUBAN.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(通番取得)例外エラー", "失敗", ex.Message)
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
                Orareader = Nothing
            End If
        End Try

    End Sub
    Private Sub btnHimoku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHimoku.Click

        Dim intSpdRow As Integer

        Dim dblKingaku_Goukei As Double
        Try
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(費目取得)開始", "成功", "")
            '費目情報ｺﾝﾎﾞﾎﾞｯｸｽ未選択チェック
            If cmbHIMOKU.SelectedIndex = -1 Then
                MessageBox.Show(G_MSG0066W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '学校コード入力チェック
            If txtGAKKOU_CODE.Text = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If
            '学年入力チェック
            If txtGAKUNEN_CODE.Text = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学年コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKUNEN_CODE.Focus()
                Exit Sub
            End If
            '確認メッセージ
            If MessageBox.Show(G_MSG0019I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Exit Sub
            End If

            Dim OraReader As CASTCommon.MyOracleReader = Nothing


            '金額変更時の再合計計算
            '費目レコードの取得
            Dim SQL As New StringBuilder(128)
            SQL.Append(" SELECT * FROM HIMOMAST")
            SQL.Append(" WHERE GAKKOU_CODE_H ='" & Trim(txtGAKKOU_CODE.Text) & "'")
            SQL.Append(" AND GAKUNEN_CODE_H ='" & Trim(txtGAKUNEN_CODE.Text) & "'")
            SQL.Append(" AND HIMOKU_ID_H ='" & fn_SplitHimokuID() & "'")

            OraReader = New CASTCommon.MyOracleReader(MainDB)

            '費目レコード存在チェック
            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(G_MSG0067W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            Else
                '費目情報４月から翌３月のクリア
                Call Sub_Himoku_Initialize()

                '費目情報への値の設定
                Dim dgv As DataGridView = Nothing
                While OraReader.EOF = False
                    Select Case Format(CInt(OraReader.GetItem("TUKI_NO_H")), "00")
                        Case "04"
                            dgv = CustomDataGridView1
                        Case "05"
                            dgv = CustomDataGridView2
                        Case "06"
                            dgv = CustomDataGridView3
                        Case "07"
                            dgv = CustomDataGridView4
                        Case "08"
                            dgv = CustomDataGridView5
                        Case "09"
                            dgv = CustomDataGridView6
                        Case "10"
                            dgv = CustomDataGridView7
                        Case "11"
                            dgv = CustomDataGridView8
                        Case "12"
                            dgv = CustomDataGridView9
                        Case "01"
                            dgv = CustomDataGridView10
                        Case "02"
                            dgv = CustomDataGridView11
                        Case "03"
                            dgv = CustomDataGridView12
                    End Select
                    dblKingaku_Goukei = 0
                    With dgv
                        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                        For intSpdRow = 0 To intMaxHimokuCnt - 1
                            'For intSpdRow = 0 To 9 '2007/04/04　費目は１０まで
                            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                            If IsDBNull(OraReader.GetItem("HIMOKU_NAME" & Format(intSpdRow + 1, "00") & "_H")) = True Then
                                .Rows(intSpdRow).Cells(1).ReadOnly = True
                                .Rows(intSpdRow).Cells(2).ReadOnly = True
                            Else
                                .Rows(intSpdRow).Cells(1).ReadOnly = False
                                .Rows(intSpdRow).Cells(2).ReadOnly = True

                                .Rows(intSpdRow).Cells(0).Value = OraReader.GetItem("HIMOKU_NAME" & Format(intSpdRow + 1, "00") & "_H")
                                'コンボボックスを”一律”にする
                                .Rows(intSpdRow).Cells(1).Value = "一律"
                                Str_Spread((CInt(OraReader.GetItem("TUKI_NO_H")) - 1), intSpdRow) = OraReader.GetItem("HIMOKU_KINGAKU" & Format(intSpdRow + 1, "00") & "_H")
                                .Rows(intSpdRow).Cells(2).Value = Format(CDec(OraReader.GetItem("HIMOKU_KINGAKU" & Format(intSpdRow + 1, "00") & "_H")), "#,##0")

                                dblKingaku_Goukei += CDbl(OraReader.GetItem("HIMOKU_KINGAKU" & Format(intSpdRow + 1, "00") & "_H"))
                            End If
                        Next intSpdRow
                    End With

                    Select Case Format(CInt(OraReader.GetItem("TUKI_NO_H")), "00")
                        Case "04"
                            lblKingaku_4.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "05"
                            lblKingaku_5.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "06"
                            lblKingaku_6.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "07"
                            lblKingaku_7.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "08"
                            lblKingaku_8.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "09"
                            lblKingaku_9.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "10"
                            lblKingaku_10.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "11"
                            lblKingaku_11.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "12"
                            lblKingaku_12.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "01"
                            lblKingaku_1.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "02"
                            lblKingaku_2.Text = Format(dblKingaku_Goukei, "#,##0")
                        Case "03"
                            lblKingaku_3.Text = Format(dblKingaku_Goukei, "#,##0")
                    End Select

                    OraReader.NextRead()
                End While

                OraReader.Close()
                OraReader = Nothing

            End If

            '2017/05/09 タスク）西野 ADD 標準版修正（費目設定の一括変更対応）---------------------- START
            For intRow As Integer = 0 To intMaxHimokuCnt - 1
                '費目名を設定する
                CustomDataGridView13.Rows(intRow).Cells(0).Value = CustomDataGridView1.Rows(intRow).Cells(0).Value
            Next
            '2017/05/09 タスク）西野 ADD 標準版修正（費目設定の一括変更対応）---------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(費目取得)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(費目取得)終了", "成功", "")
            LW.ToriCode = "000000000000"
        End Try

    End Sub
    Private Sub btnTyousi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTyousi.Click
        '長子取得ボタン
        Try
            Dim KFGMAST041 As New GAKKOU.KFGMAST041

            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                str長子学校コード = txtGAKKOU_CODE.Text

                '長子設定画面表示
                KFGMAST041.ShowDialog(Me)
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(長子取得)例外エラー", "失敗", ex.Message)
        End Try

    End Sub
    Private Sub btnKOUZA_CHK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKOUZA_CHK.Click
        Try
            Call fn_CheckEntryKouza()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座チェック)例外エラー", "失敗", ex.Message)
        End Try
    End Sub
#End Region

    Private Sub Sub_GetHimokuID()

        '学校コードと学年から費目レコード（費目ＩＤ／費目パターン名）を取得
        Dim str費目ID As String = ""

        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtGAKUNEN_CODE.Text) <> "" Then

            Dim OraReader As CASTCommon.MyOracleReader = Nothing

            Try
                Dim SQL As New StringBuilder(128)

                SQL.Append(" SELECT * FROM HIMOMAST")
                SQL.Append(" WHERE GAKKOU_CODE_H ='" & Trim(txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c)) & "'")
                SQL.Append(" AND GAKUNEN_CODE_H = " & CByte(txtGAKUNEN_CODE.Text))
                SQL.Append(" AND HIMOKU_ID_H <> '000'")
                SQL.Append(" ORDER BY HIMOKU_ID_H ASC")

                OraReader = New CASTCommon.MyOracleReader(MainDB)

                If Not OraReader.DataReader(SQL) Then
                    cmbHIMOKU.Text = ""
                    cmbHIMOKU.Items.Clear()
                    str費目ID = ""
                Else
                    While OraReader.EOF = False
                        If str費目ID <> OraReader.GetItem("HIMOKU_ID_H") Then
                            cmbHIMOKU.Items.Add(Trim(OraReader.GetItem("HIMOKU_ID_H") & _
                                                "・" & _
                                                OraReader.GetItem("HIMOKU_ID_NAME_H")))
                            '比較のためのＩＤを退避する
                            str費目ID = OraReader.GetItem("HIMOKU_ID_H")
                        End If
                        OraReader.NextRead()
                    End While

                    OraReader.Close()
                    OraReader = Nothing

                    '先頭の費目ＩＤを表示
                    If cmbHIMOKU.Items.Count <> 0 Then
                        cmbHIMOKU.SelectedIndex = 0
                    End If
                End If
            Catch ex As Exception
            Finally
                If Not OraReader Is Nothing Then
                    OraReader.Close()
                    OraReader = Nothing
                End If
            End Try
        End If
    End Sub
    Private Sub Sub_Himoku_Initialize()

        'スプレットのクリア
        Call Sub_ClearHimoku(CustomDataGridView1)
        Call Sub_ClearHimoku(CustomDataGridView2)
        Call Sub_ClearHimoku(CustomDataGridView3)
        Call Sub_ClearHimoku(CustomDataGridView4)
        Call Sub_ClearHimoku(CustomDataGridView5)
        Call Sub_ClearHimoku(CustomDataGridView6)
        Call Sub_ClearHimoku(CustomDataGridView7)
        Call Sub_ClearHimoku(CustomDataGridView8)
        Call Sub_ClearHimoku(CustomDataGridView9)
        Call Sub_ClearHimoku(CustomDataGridView10)
        Call Sub_ClearHimoku(CustomDataGridView11)
        Call Sub_ClearHimoku(CustomDataGridView12)

        '2017/05/09 タスク）西野 ADD 標準版修正（費目設定の一括変更対応）---------------------- START
        For intRow As Integer = 0 To intMaxHimokuCnt - 1
            '変更タブ上のDataGridViewの費目名クリア
            CustomDataGridView13.Rows(intRow).Cells(0).Value = ""
        Next
        '2017/05/09 タスク）西野 ADD 標準版修正（費目設定の一括変更対応）---------------------- END

        '合計金額のクリア
        lblKingaku_1.Text = ""
        lblKingaku_2.Text = ""
        lblKingaku_3.Text = ""
        lblKingaku_4.Text = ""
        lblKingaku_5.Text = ""
        lblKingaku_6.Text = ""
        lblKingaku_7.Text = ""
        lblKingaku_8.Text = ""
        lblKingaku_9.Text = ""
        lblKingaku_10.Text = ""
        lblKingaku_11.Text = ""
        lblKingaku_12.Text = ""

    End Sub
    Private Sub Sub_ClearHimoku(ByVal dgv As DataGridView)

        Try
            With dgv
                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    'For intRow As Integer = 0 To 9 '　費目は１０まで
                    '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                    For intCol As Integer = 0 To 2
                        If intCol <> 0 Then
                            .Rows(intRow).Cells(intCol).ReadOnly = False
                        End If

                        Select Case intCol
                            Case 0
                                '費目名
                                .Rows(intRow).Cells(intCol).Value = ""
                            Case 1
                                '請求方法
                                .Rows(intRow).Cells(intCol).Value = ""
                            Case 2
                                '金額
                                .Rows(intRow).Cells(intCol).Value = ""
                        End Select
                    Next intCol
                Next intRow
            End With
        Catch ex As Exception
            '2014/01/06 saitou 標準版 メッセージ定数化 UPD -------------------------------------------------->>>>
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            'MessageBox.Show("費目一覧の初期化に失敗しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '2014/01/06 saitou 標準版 メッセージ定数化 UPD --------------------------------------------------<<<<
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外エラー", "失敗", ex.Message)
        End Try
    End Sub
    Private Function fn_INSERTSEITOMAST(ByVal dgv As DataGridView, ByVal pTuki As Integer) As Boolean

        Dim ret As Boolean = False

        Try
            Dim SQL As New StringBuilder(128)

            SQL.Append("INSERT INTO SEITOMAST ")
            SQL.Append(" (GAKKOU_CODE_O ")
            SQL.Append(", NENDO_O ")
            SQL.Append(", TUUBAN_O ")
            SQL.Append(", GAKUNEN_CODE_O ")
            SQL.Append(", CLASS_CODE_O ")
            SQL.Append(", SEITO_NO_O ")
            SQL.Append(", SEITO_KNAME_O ")
            SQL.Append(", SEITO_NNAME_O ")
            SQL.Append(", SEIBETU_O ")
            SQL.Append(", TKIN_NO_O ")
            SQL.Append(", TSIT_NO_O ")
            SQL.Append(", KAMOKU_O ")
            SQL.Append(", KOUZA_O ")
            SQL.Append(", MEIGI_KNAME_O ")
            SQL.Append(", MEIGI_NNAME_O ")
            SQL.Append(", FURIKAE_O ")
            SQL.Append(", KEIYAKU_NJYU_O ")
            SQL.Append(", KEIYAKU_DENWA_O ")
            SQL.Append(", KAIYAKU_FLG_O ")
            SQL.Append(", SINKYU_KBN_O ")
            SQL.Append(", HIMOKU_ID_O ")
            SQL.Append(", TYOUSI_FLG_O ")
            SQL.Append(", TYOUSI_NENDO_O ")
            SQL.Append(", TYOUSI_TUUBAN_O ")
            SQL.Append(", TYOUSI_GAKUNEN_O ")
            SQL.Append(", TYOUSI_CLASS_O ")
            SQL.Append(", TYOUSI_SEITONO_O ")
            SQL.Append(", TUKI_NO_O ")
            SQL.Append(", SEIKYU01_O ")
            SQL.Append(", KINGAKU01_O ")
            SQL.Append(", SEIKYU02_O ")
            SQL.Append(", KINGAKU02_O ")
            SQL.Append(", SEIKYU03_O ")
            SQL.Append(", KINGAKU03_O ")
            SQL.Append(", SEIKYU04_O ")
            SQL.Append(", KINGAKU04_O ")
            SQL.Append(", SEIKYU05_O ")
            SQL.Append(", KINGAKU05_O ")
            SQL.Append(", SEIKYU06_O ")
            SQL.Append(", KINGAKU06_O ")
            SQL.Append(", SEIKYU07_O ")
            SQL.Append(", KINGAKU07_O ")
            SQL.Append(", SEIKYU08_O ")
            SQL.Append(", KINGAKU08_O ")
            SQL.Append(", SEIKYU09_O ")
            SQL.Append(", KINGAKU09_O ")
            SQL.Append(", SEIKYU10_O ")
            SQL.Append(", KINGAKU10_O ")
            SQL.Append(", SEIKYU11_O ")
            SQL.Append(", KINGAKU11_O ")
            SQL.Append(", SEIKYU12_O ")
            SQL.Append(", KINGAKU12_O ")
            SQL.Append(", SEIKYU13_O ")
            SQL.Append(", KINGAKU13_O ")
            SQL.Append(", SEIKYU14_O ")
            SQL.Append(", KINGAKU14_O ")
            SQL.Append(", SEIKYU15_O ")
            SQL.Append(", KINGAKU15_O ")
            SQL.Append(", SAKUSEI_DATE_O ")
            SQL.Append(", KOUSIN_DATE_O ) ")
            SQL.Append(" VALUES ( ")

            '学校コード
            SQL.Append("'" & txtGAKKOU_CODE.Text & "'")
            '入学年度
            SQL.Append("," & "'" & txtNENDO.Text & "'")
            '通番
            SQL.Append("," & CInt(txtTUUBAN.Text))
            '学年
            SQL.Append("," & CByte(txtGAKUNEN_CODE.Text))
            'クラス
            SQL.Append("," & CByte(txtCLASS_CODE.Text))
            '生徒番号
            SQL.Append("," & "'" & txtSEITO_NO.Text & "'")
            '生徒氏名（カナ）
            SQL.Append("," & "'" & Trim(txtSEITO_KNAME.Text) & "'")
            '生徒氏名（漢字）
            If Trim(txtSEITO_NNAME.Text) = "" Then
                SQL.Append("," & "'" & Space(50) & "'")
            Else
                SQL.Append("," & "'" & Trim(txtSEITO_NNAME.Text) & "'")
            End If
            '性別
            SQL.Append(",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEIBETU_TXT, cmbSEIBETU) & "'")

            '金融機関コード
            If Trim(txtTKIN_NO.Text) = "" Then
                SQL.Append("," & "'    '")
            Else
                SQL.Append("," & "'" & Trim(txtTKIN_NO.Text) & "'")
            End If
            '支店コード
            If Trim(txtTSIT_NO.Text) = "" Then
                SQL.Append("," & "'   '")
            Else
                SQL.Append("," & "'" & Trim(txtTSIT_NO.Text) & "'")
            End If
            '科目
            SQL.Append(",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU) & "'")

            '口座番号
            If Trim(txtKOUZA.Text) = "" Then
                SQL.Append("," & "'       '")
            Else
                SQL.Append("," & "'" & Trim(txtKOUZA.Text) & "'")
            End If
            '名義人（カナ）
            SQL.Append("," & "'" & Trim(txtMEIGI_KNAME.Text) & "'")
            '名義人（漢字）
            If Trim(txtMEIGI_NNAME.Text) = "" Then
                SQL.Append("," & "'" & Space(50) & "'")
            Else
                SQL.Append("," & "'" & Trim(txtMEIGI_NNAME.Text) & "'")
            End If
            '振替方法
            SQL.Append(",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURIKAE_TXT, cmbFURIKAE) & "'")

            '契約住所（漢字）
            If Trim(txtKEIYAKU_NJYU.Text) = "" Then
                SQL.Append("," & "'" & Space(50) & "'")
            Else
                SQL.Append("," & "'" & Trim(txtKEIYAKU_NJYU.Text) & "'")
            End If
            '電話番号
            If Trim(txtKEIYAKU_DENWA.Text) = "" Then
                SQL.Append("," & "'" & Space(13) & "'")
            Else
                SQL.Append("," & "'" & Trim(txtKEIYAKU_DENWA.Text) & "'")
            End If
            '解約区分
            SQL.Append(",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_KAIYAKU_TXT, cmbKAIYAKU_FLG) & "'")

            '進級区分
            SQL.Append(",'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SINKYU_TXT, cmbSINKYU_KBN) & "'")

            '費目ＩＤ
            SQL.Append("," & "'" & fn_SplitHimokuID() & "'")
            '長子有無フラグ
            '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- START
            SQL.Append("," & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_TYOUSIUMU_TXT, cmbTYOUSI))
            'SQL.Append("," & cmbTYOUSI().SelectedIndex)
            '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- END
            '長子入学年度
            If Trim(str長子入学年度) = "" Then
                SQL.Append("," & "'" & Space(4) & "'")
            Else
                SQL.Append("," & "'" & Trim(str長子入学年度) & "'")
            End If
            '長子通番
            If Trim(str長子通番) = "" Then
                SQL.Append("," & 0)
            Else
                SQL.Append("," & "'" & CDbl(str長子通番) & "'")
            End If
            '長子学年
            If Trim(str長子学年) = "" Then
                SQL.Append("," & 0)
            Else
                SQL.Append("," & "'" & CByte(str長子学年) & "'")
            End If
            '長子クラス
            If Trim(str長子クラス) = "" Then
                SQL.Append("," & 0)
            Else
                SQL.Append("," & "'" & CByte(str長子クラス) & "'")
            End If
            '長子生徒番号
            If Trim(str長子生徒番号) = "" Then
                SQL.Append("," & "'" & Space(7) & "'")
            Else
                SQL.Append("," & "'" & Trim(str長子生徒番号) & "'")
            End If
            '請求月
            SQL.Append("," & "'" & Format(pTuki, "00") & "'")

            '費目１～費目１５の請求方法,金額
            For intRowCnt As Integer = 0 To 14 '2007/04/04　費目は１０まで画面入力、１１からは０固定
                '2017/04/06 タスク）西野 DEL 標準版修正（費目数１０⇒１５）------------------------------------ START
                If intRowCnt <= (intMaxHimokuCnt - 1) Then
                    'If intRowCnt <= 9 Then
                    '2017/04/06 タスク）西野 DEL 標準版修正（費目数１０⇒１５）------------------------------------ END
                    '費目１～１０
                    With dgv
                        '費目名の有無
                        Select Case Trim(.Rows(intRowCnt).Cells(0).Value)
                            Case ""
                                '請求方法
                                SQL.Append(",'0'")
                                '金額
                                SQL.Append(",0")
                            Case Else
                                '請求方法
                                '請求方法が一律(=0)の場合、金額は0固定を設定する
                                Select Case .Rows(intRowCnt).Cells(1).Value
                                    Case "一律"
                                        '一律
                                        SQL.Append(",0")
                                        SQL.Append(",0")
                                    Case "個別"
                                        '個別
                                        SQL.Append(",1")
                                        SQL.Append("," & CDec(.Rows(intRowCnt).Cells(2).Value).ToString)
                                End Select
                        End Select
                    End With
                    '2017/02/23 タスク）西野 UPD 標準版修正（費目数１０⇒１５）------------------------------------ START
                Else
                    '請求方法
                    SQL.Append(",'0'")
                    '金額
                    SQL.Append(",0")
                End If
                'Else
                '       '費目１１～１５
                '       '請求方法
                '       SQL.Append(",'0'")
                '       '金額
                '       SQL.Append(",0")
                '       End If
                '2017/02/23 タスク）西野 UPD 標準版修正（費目数１０⇒１５）------------------------------------ END
            Next

            SQL.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
            SQL.Append(",'00000000') ")

            If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)例外エラー", "失敗", ex.Message)
            ret = False
        End Try

        Return ret


    End Function
    Private Function fn_UPDATESEITOMAST(ByVal dgv As DataGridView, ByVal pTuki As Integer) As Boolean

        Dim ret As Boolean = False

        Try
            Dim SQL As New StringBuilder(128)

            SQL.Append("UPDATE  SEITOMAST SET ")
            '学年
            SQL.Append(" GAKUNEN_CODE_O = " & CByte(txtGAKUNEN_CODE.Text))
            '追加：クラス
            SQL.Append(" ,CLASS_CODE_O = " & CByte(txtCLASS_CODE.Text))
            '生徒番号
            SQL.Append(" ,SEITO_NO_O = '" & Trim(txtSEITO_NO.Text) & "'")
            '生徒名（カナ）
            SQL.Append(" ,SEITO_KNAME_O   = '" & Trim(txtSEITO_KNAME.Text) & "'")
            '生徒名（漢字）
            If Trim(txtSEITO_NNAME.Text) = "" Then
                SQL.Append(",SEITO_NNAME_O   = " & "'" & Space(50) & "'")
            Else
                SQL.Append(",SEITO_NNAME_O   = " & "'" & Trim(txtSEITO_NNAME.Text) & "'")
            End If

            '性別
            SQL.Append(",SEIBETU_O       = " & "'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEIBETU_TXT, cmbSEIBETU) & "'")

            '金融機関コード
            If Trim(txtTKIN_NO.Text) = "" Then
                SQL.Append(",TKIN_NO_O       = " & "'" & Space(4) & "'")
            Else
                SQL.Append(",TKIN_NO_O       = " & "'" & Trim(txtTKIN_NO.Text) & "'")
            End If
            '支店コード
            If Trim(txtTSIT_NO.Text) = "" Then
                SQL.Append(",TSIT_NO_O       = " & "'" & Space(3) & "'")
            Else
                SQL.Append(",TSIT_NO_O       = " & "'" & Trim(txtTSIT_NO.Text) & "'")
            End If
            '科目
            SQL.Append(",KAMOKU_O       = " & "'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU) & "'")

            '口座番号
            If Trim(txtKOUZA.Text) = "" Then
                SQL.Append(",KOUZA_O         = " & "'" & Space(7) & "'")
            Else
                SQL.Append(",KOUZA_O         = " & "'" & Trim(txtKOUZA.Text) & "'")
            End If
            '名義人（カナ）
            SQL.Append(",MEIGI_KNAME_O   = " & "'" & Trim(txtMEIGI_KNAME.Text) & "'")
            '名義人（漢字）
            If Trim(txtMEIGI_NNAME.Text) = "" Then
                SQL.Append(",MEIGI_NNAME_O   = " & "'" & Space(50) & "'")
            Else
                SQL.Append(",MEIGI_NNAME_O   = " & "'" & Trim(txtMEIGI_NNAME.Text) & "'")
            End If
            '振替区分
            SQL.Append(",FURIKAE_O       = " & "'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURIKAE_TXT, cmbFURIKAE) & "'")

            '契約住所（漢字）
            If Trim(txtKEIYAKU_NJYU.Text) = "" Then
                SQL.Append(",KEIYAKU_NJYU_O  = " & "'" & Space(50) & "'")
            Else
                SQL.Append(",KEIYAKU_NJYU_O  = " & "'" & Trim(txtKEIYAKU_NJYU.Text) & "'")
            End If
            '電話番号
            If Trim(txtKEIYAKU_DENWA.Text) = "" Then
                SQL.Append(",KEIYAKU_DENWA_O = " & "'" & Space(12) & "'")
            Else
                SQL.Append(",KEIYAKU_DENWA_O = " & "'" & Trim(txtKEIYAKU_DENWA.Text) & "'")
            End If
            '解約区分
            SQL.Append(",KAIYAKU_FLG_O       = " & "'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_KAIYAKU_TXT, cmbKAIYAKU_FLG) & "'")

            '進級区分
            SQL.Append(",SINKYU_KBN_O       = " & "'" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SINKYU_TXT, cmbSINKYU_KBN) & "'")

            '費目ＩＤ
            SQL.Append(",HIMOKU_ID_O     = " & "'" & fn_SplitHimokuID() & "'")
            '長子有無フラグ
            '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- START
            SQL.Append(",TYOUSI_FLG_O    = " & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_TYOUSIUMU_TXT, cmbTYOUSI))
            'SQL.Append(",TYOUSI_FLG_O    = " & cmbTYOUSI().SelectedIndex)
            '2017/11/28 タスク）西野 CHG (標準版修正(№177)) -------------------- END

            '長子入学年度
            If Trim(str長子入学年度) <> "" Then
                SQL.Append(",TYOUSI_NENDO_O  = " & "'" & Trim(str長子入学年度) & "'")
            End If
            '長子通番
            If Trim(str長子通番) <> "" Then
                SQL.Append(",TYOUSI_TUUBAN_O = " & CInt(str長子通番))
            End If
            '長子学年
            If Trim(str長子学年) <> "" Then
                SQL.Append(",TYOUSI_GAKUNEN_O= " & CByte(str長子学年))
            End If
            '長子クラス
            If Trim(str長子クラス) <> "" Then
                SQL.Append(",TYOUSI_CLASS_O  = " & CByte(str長子クラス))
            End If
            '長子生徒番号
            If Trim(str長子生徒番号) <> "" Then
                SQL.Append(",TYOUSI_SEITONO_O= " & "'" & Trim(str長子生徒番号) & "'")
            End If

            '費目１～費目１５の請求方法,金額
            For intRowCnt As Integer = 0 To 14 '2007/04/04　費目は１０まで画面入力、１１から０固定
                '2017/04/06 タスク）西野 DEL 標準版修正（費目数１０⇒１５）------------------------------------ START
                If intRowCnt <= (intMaxHimokuCnt - 1) Then
                    'If intRowCnt <= 9 Then
                    '2017/04/06 タスク）西野 DEL 標準版修正（費目数１０⇒１５）------------------------------------ END
                    '費目１～１０
                    With dgv
                        '費目名の有無
                        Select Case Trim(.Rows(intRowCnt).Cells(0).Value)
                            Case ""
                                '請求方法
                                SQL.Append(",SEIKYU" & Format(intRowCnt + 1, "00") & "_O  ='0'")
                                '金額
                                SQL.Append(",KINGAKU" & Format(intRowCnt + 1, "00") & "_O  =0")
                            Case Else
                                '請求方法
                                Dim SeikyuFlg As String = "0"
                                If .Rows(intRowCnt).Cells(1).Value = "一律" Then
                                    SeikyuFlg = "0"
                                ElseIf .Rows(intRowCnt).Cells(1).Value = "個別" Then
                                    SeikyuFlg = "1"
                                End If
                                SQL.Append(",SEIKYU" & Format(intRowCnt + 1, "00") & "_O  ='" & SeikyuFlg & "'")

                                '2006/10/24　請求方法が一律(=0)の場合・金額が空欄だった場合は金額は0固定を設定する 
                                '2017/05/09 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
                                '型暗黙変換によるエラー修正
                                If .Rows(intRowCnt).Cells(1).Value = "一律" Or CStr(.Rows(intRowCnt).Cells(2).Value).Trim = "" Then
                                    'If .Rows(intRowCnt).Cells(1).Value = "一律" Or .Rows(intRowCnt).Cells(2).Value = "" Then
                                    '2017/05/09 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
                                    '一律
                                    SQL.Append(",KINGAKU" & Format(intRowCnt + 1, "00") & "_O  =0")
                                Else
                                    '個別
                                    SQL.Append(",KINGAKU" & Format(intRowCnt + 1, "00") & "_O  =" & CDec(.Rows(intRowCnt).Cells(2).Value).ToString)
                                End If
                        End Select
                    End With
                    '2017/04/06 タスク）西野 UPD 標準版修正（費目数１０⇒１５）------------------------------------ START
                Else
                    '請求方法
                    SQL.Append(",SEIKYU" & Format(intRowCnt + 1, "00") & "_O  ='0'")
                    '金額
                    SQL.Append(",KINGAKU" & Format(intRowCnt + 1, "00") & "_O  =0")
                End If
                'Else
                '   '費目１１～１５
                '   '請求方法
                '   SQL.Append(",SEIKYU" & Format(intRowCnt + 1, "00") & "_O  ='0'")
                '   '金額
                '   SQL.Append(",KINGAKU" & Format(intRowCnt + 1, "00") & "_O  =0")
                '   End If
                '2017/02/23 タスク）西野 UPD 標準版修正（費目数１０⇒１５）------------------------------------ END
            Next intRowCnt

            '更新日付
            SQL.Append(",KOUSIN_DATE_O ='" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
            SQL.Append(" WHERE")
            SQL.Append(" GAKKOU_CODE_O  ='" & txtGAKKOU_CODE.Text & "'")
            SQL.Append(" AND NENDO_O        ='" & txtNENDO.Text & "'")
            SQL.Append(" AND TUUBAN_O       = " & CInt(txtTUUBAN.Text))
            SQL.Append(" AND TUKI_NO_O     ='" & Format(pTuki, "00") & "'")

            If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)例外エラー", "失敗", ex.Message)
            ret = False
        End Try

        Return ret

    End Function
    Private Function fn_GetKinNameSitName() As Integer
        '金融機関名、支店名の取得

        fn_GetKinNameSitName = 0

        If Trim(txtTKIN_NO.Text) <> "" And Trim(txtTSIT_NO.Text) <> "" Then
            txtTKIN_NO.Text = txtTKIN_NO.Text.Trim.PadLeft(4, "0")
            txtTSIT_NO.Text = txtTSIT_NO.Text.Trim.PadLeft(3, "0")

            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""
            SQL = " SELECT * FROM TENMAST"
            SQL += " WHERE KIN_NO_N ='" & Trim(txtTKIN_NO.Text) & "'"
            SQL += " AND SIT_NO_N ='" & Trim(txtTSIT_NO.Text) & "'"

            If Not OraReader.DataReader(SQL) Then
                txtTKIN_NO.Text = ""
                txtTSIT_NO.Text = ""
                MessageBox.Show(MSG0096W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                lab取扱金融機関.Text = ""
                lab取扱支店.Text = ""
                fn_GetKinNameSitName = 1
                txtTKIN_NO.Focus()
            Else
                lab取扱金融機関.Text = OraReader.GetItem("KIN_NNAME_N")
                lab取扱支店.Text = OraReader.GetItem("SIT_NNAME_N")
            End If

            OraReader.Close()
            OraReader = Nothing
        End If

    End Function
    Private Function fn_GetClassName() As Integer

        Dim intCnt As Integer
        Dim chkCLASS As Boolean = False '2006/10/11　クラス存在チェック

        'クラス名の取得

        '学校コード,学年コードが未入力の場合はチェックを行わない
        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtGAKUNEN_CODE.Text) <> "" And Trim(txtCLASS_CODE.Text) <> "" Then

            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""

            SQL = " SELECT * FROM GAKMAST1"
            SQL += " WHERE GAKKOU_CODE_G ='" & Trim(txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c)) & "'"
            SQL += " AND GAKUNEN_CODE_G =" & CByte(txtGAKUNEN_CODE.Text)

            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                labクラス名.Text = ""
                txtGAKUNEN_CODE.Focus()
            Else
                '入力したクラスと一致するクラスをDB上より取得
                '一致したクラスコードのクラス名称を取得する
                For intCnt = 1 To 20
                    '設定されていないNULL値の場合はクラス名設定を抜ける
                    'クラスをとびとびで登録した場合の対処追加
                    If IsDBNull(OraReader.GetItem("CLASS_CODE1" & Format(intCnt, "00") & "_G")) = True Then
                        'クラス飛ばし対応のため処理なし
                    Else
                        '画面上で入力されているクラスコードと一致するクラスコードのクラス名称をラベルに設定
                        If GCom.NzInt(txtCLASS_CODE.Text) = GCom.NzInt((OraReader.GetItem("CLASS_CODE1" & Format(intCnt, "00") & "_G"))) Then
                            labクラス名.Text = OraReader.GetItem("CLASS_NAME1" & Format(intCnt, "00") & "_G")
                            chkCLASS = True
                            Exit For
                        End If
                    End If
                Next intCnt

                'クラスが検出されなかった場合、クラス名ラベルは空白にする
                If chkCLASS = False Then
                    MessageBox.Show(G_MSG0068W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtCLASS_CODE.Text = ""
                    txtCLASS_CODE.Focus()
                    labクラス名.Text = ""
                End If
            End If

            OraReader.Close()
            OraReader = Nothing
        Else
            '2006/10/11　学年欄・クラス欄が空欄だった場合、クラス名ラベルも空白にする
            labクラス名.Text = ""
        End If

    End Function
    Private Function fn_GetSELECTSEITOMASTSQL(ByRef SQL As StringBuilder) As Boolean

        SQL.Append(" SELECT * FROM SEITOMASTVIEW ")
        Select Case True
            Case (Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtNENDO.Text) <> "" And Trim(txtTUUBAN.Text) <> "")
                '学校コード、入学年度、通番が入力されている場合
                SQL.Append(" WHERE GAKKOU_CODE_O ='" & txtGAKKOU_CODE.Text & "'")
                SQL.Append(" AND NENDO_O ='" & Trim(txtNENDO.Text) & "'")
                SQL.Append(" AND TUUBAN_O = " & CInt(txtTUUBAN.Text))

                Return True
            Case (Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtGAKUNEN_CODE.Text) <> "" And Trim(txtCLASS_CODE.Text) <> "" And Trim(txtSEITO_NO.Text) <> "")
                '学校コード、学年コード、クラスコード、生徒番号が入力されている場合
                SQL.Append(" WHERE GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'")
                SQL.Append(" AND GAKUNEN_CODE_O = " & CByte(txtGAKUNEN_CODE.Text))
                SQL.Append(" AND CLASS_CODE_O  = " & CByte(txtCLASS_CODE.Text))
                SQL.Append(" AND SEITO_NO_O ='" & Trim(txtSEITO_NO.Text) & "'")

                Return True
            Case Else
                '学校コード
                If Trim(txtGAKKOU_CODE.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Return False
                End If
                '入学年度
                If Trim(txtNENDO.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "入学年度"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtNENDO.Focus()
                    Return False
                End If
                '通番
                If Trim(txtTUUBAN.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "通番"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTUUBAN.Focus()
                    Return False
                End If
                '学年
                If Trim(txtGAKUNEN_CODE.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "学年コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKUNEN_CODE.Focus()
                    Return False
                End If
                'クラス
                If Trim(txtCLASS_CODE.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "クラスコード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtCLASS_CODE.Focus()
                    Return False
                End If
                '生徒番号
                If Trim(txtSEITO_NO.Text) = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "生徒番号"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSEITO_NO.Focus()
                    Return False
                End If
        End Select

    End Function
    Private Function fn_GetGakunenName() As Boolean

        '学年名の取得（学校マスタ１参照）
        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txtGAKUNEN_CODE.Text) <> "" Then

            Dim OraReader As CASTCommon.MyOracleReader = Nothing

            Try
                Dim SQL As New StringBuilder(128)
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G ='" & Trim(txtGAKKOU_CODE.Text) & "'")
                SQL.Append(" AND GAKUNEN_CODE_G = " & CByte(txtGAKUNEN_CODE.Text))

                OraReader = New CASTCommon.MyOracleReader(MainDB)

                If Not OraReader.DataReader(SQL) Then
                    MessageBox.Show(G_MSG0069W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lab学年名.Text = ""
                    txtGAKUNEN_CODE.Text = "" '2006/10/11
                    txtGAKUNEN_CODE.Focus() '2006/10/11　フォーカスを学年コード欄に当てる

                    Return False
                Else
                    lab学年名.Text = OraReader.GetItem("GAKUNEN_NAME_G")

                    Return True
                End If

            Catch ex As Exception
                Return False
            Finally
                If Not OraReader Is Nothing Then
                    OraReader.Close()
                    OraReader = Nothing
                End If
            End Try
        Else
            '学校コード欄空白時、学校名ラベルを空白にする
            lab学年名.Text = ""
        End If

        Return True

    End Function
    Private Function fn_SplitHimokuID() As String
        '費目ＩＤ・パターン名から費目ＩＤを抽出
        Return cmbHIMOKU.Text.Substring(0, InStr(cmbHIMOKU.Text, "・") - 1)
    End Function
    Private Function fn_SetHimokuID(ByVal pHimoku_ID As String) As Boolean
        If Trim(pHimoku_ID) = "" Then
            Exit Function
        End If
        For i As Integer = 0 To 998
            cmbHIMOKU.SelectedIndex = i
            If pHimoku_ID = Mid(cmbHIMOKU.Text, 1, 3) Then
                Exit For
            End If
        Next
    End Function
    Private Function fn_FormInitialize() As Integer
        '画面の初期化処理

        fn_FormInitialize = 0

        '通番
        txtTUUBAN.Text = ""
        '性別コンボ
        cmbSEIBETU.SelectedIndex = 0
        '生徒氏名（カナ）
        txtSEITO_KNAME.Text = ""
        '生徒氏名（漢字）
        txtSEITO_NNAME.Text = ""
        '振替方法コンボ
        cmbFURIKAE.SelectedIndex = 0
        '金融機関コード
        txtTKIN_NO.Text = ""
        lab取扱金融機関.Text = ""
        lab取扱支店.Text = ""
        '支店コード
        txtTSIT_NO.Text = ""
        '科目コンボ
        cmbKAMOKU.SelectedIndex = 0
        '口座番号
        txtKOUZA.Text = ""
        '名義人（カナ）
        txtMEIGI_KNAME.Text = ""
        '名義人（漢字）
        txtMEIGI_NNAME.Text = ""
        '契約住所（漢字）
        txtKEIYAKU_NJYU.Text = ""
        '電話番号
        txtKEIYAKU_DENWA.Text = ""
        '進級区分コンボ
        cmbSINKYU_KBN.SelectedIndex = 0
        '解約区分コンボ
        cmbKAIYAKU_FLG.SelectedIndex = 0
        '長子情報コンボ
        cmbTYOUSI.SelectedIndex = 0
        '費目コンボボックスのクリア
        cmbHIMOKU.Items.Clear()
        '費目情報４月から翌３月のクリア
        Call Sub_Himoku_Initialize()

        lblKOUZA_CHK.Text = ""

        '2017/04/28 saitou RSV2 ADD 標準機能追加(更新日) ---------------------------------------- START
        Me.lblKousinDate.Text = ""
        '2017/04/28 saitou RSV2 ADD ------------------------------------------------------------- END

    End Function
    Private Function fn_CheckEntry(ByVal pIndex As Integer) As Boolean

        Dim intCnt As Integer

        fn_CheckEntry = False

        '学校コード
        '未入力チェック
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Exit Function
        Else
            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""
            '学校マスタ存在チェック
            SQL = "SELECT GAKKOU_CODE_G"
            SQL += " FROM GAKMAST1"
            SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Function
            End If
        End If

        '入学年度
        '未入力チェック
        If Trim(txtNENDO.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "入学年度"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtNENDO.Focus()
            Exit Function
            'Else
            '    '2007/02/15
            '    If txtNENDO.Text.Trim.Length <> 4 Then
            '        MessageBox.Show("入学年度は４桁で設定してください", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtNENDO.Focus()
            '        Exit Function
            '    End If
        End If

        '通番
        '未入力チェック
        If Trim(txtTUUBAN.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "通番"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTUUBAN.Focus()
            Exit Function
        Else
            '新規登録の場合のみチェックする
            Select Case pIndex
                Case 0
                    If Trim(txtGAKUNEN_CODE.Text) <> "" Then
                        '生徒マスタ存在チェック(通番)
                        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
                        Dim SQL As String = ""
                        SQL = " SELECT * FROM SEITOMAST"
                        SQL += " WHERE GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'"
                        SQL += " AND GAKUNEN_CODE_O =" & CByte(txtGAKUNEN_CODE.Text)
                        SQL += " AND TUUBAN_O = " & CDbl(txtTUUBAN.Text)
                        SQL += " AND NENDO_O = '" & Trim(txtNENDO.Text) & "'"

                        If Not OraReader.DataReader(SQL) Then
                        Else
                            MessageBox.Show(G_MSG0070W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtTUUBAN.Focus()
                            Exit Function
                        End If
                    End If
            End Select
        End If

        '学年
        '未入力チェック
        If Trim(txtGAKUNEN_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学年コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKUNEN_CODE.Focus()
            Exit Function
        Else
            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""
            '使用学年チェック
            SQL = "SELECT SIYOU_GAKUNEN_T "
            SQL += " FROM GAKMAST2"
            SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKUNEN_CODE.Focus()
                Exit Function
            Else
                Select Case CInt(OraReader.GetItem("SIYOU_GAKUNEN_T"))
                    Case Is < CInt(txtGAKUNEN_CODE.Text)
                        '使用学年以上の学年を入力した場合はエラー
                        MessageBox.Show(G_MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtGAKUNEN_CODE.Focus()
                        Exit Function
                End Select
            End If
        End If

        'クラス
        '未入力チェック
        If Trim(txtCLASS_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "クラスコード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCLASS_CODE.Focus()
            Exit Function
        Else
            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""
            '使用クラスチェック
            SQL = "SELECT * FROM GAKMAST1"
            SQL += " WHERE GAKKOU_CODE_G = '" & Trim(txtGAKKOU_CODE.Text) & "'"
            SQL += " AND GAKUNEN_CODE_G = " & CByte(txtGAKUNEN_CODE.Text)
            SQL += " AND ("
            For intCnt = 1 To 20
                SQL += IIf(intCnt = 1, "", "or") & " CLASS_CODE1" & Format(intCnt, "00") & "_G = '" & Trim(txtCLASS_CODE.Text).PadLeft(2, "0"c) & "'"
            Next intCnt
            SQL += " )"

            If Not OraReader.DataReader(SQL) Then
                MessageBox.Show(G_MSG0068W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtCLASS_CODE.Focus()
                Exit Function
            End If
        End If

        '生徒番号
        '未入力チェック
        If Trim(txtSEITO_NO.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "生徒番号"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSEITO_NO.Focus()
            Exit Function
        End If

        '性別
        '未選択チェック
        If cmbSEIBETU.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0071W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbSEIBETU.Focus()
            Exit Function
        End If

        '生徒氏名（カナ）
        '未入力チェック
        If Trim(txtSEITO_KNAME.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "生徒名(カナ)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSEITO_KNAME.Focus()
            Exit Function
        Else
            If StrConv(txtSEITO_KNAME.Text, VbStrConv.Narrow).Length > 40 Then
                MessageBox.Show(G_MSG0087W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSEITO_KNAME.Focus()
                Exit Function
            End If
        End If

        '振替方法
        '未選択チェック
        If cmbFURIKAE.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0072W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbFURIKAE.Focus()
            Exit Function
        End If

        '取扱金融機関　
        '振替方法が口座振替(=0)の場合のみ必須チェック
        Select Case cmbFURIKAE.SelectedIndex
            Case 0
                '取扱金融機関
                '未入力チェック
                If Trim(txtTKIN_NO.Text) = "" Then
                    MessageBox.Show(MSG0032W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTKIN_NO.Focus()
                    Exit Function
                End If

                '取扱支店
                '未入力チェック
                If Trim(txtTSIT_NO.Text) = "" Then
                    MessageBox.Show(MSG0035W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTSIT_NO.Focus()
                    Exit Function
                End If

                '取扱金融機関
                '存在・学校マスタ・他行マスタチェック
                If fn_GetKinNameSitName() = 1 Then
                    txtTKIN_NO.Focus()
                    Exit Function
                ElseIf fn_CheckGakMast2FromGakkouCodeTKinNo() = 1 AndAlso fn_CheckG_TakoMastFromGakkouCodeTKinNo() = False Then
                    MessageBox.Show(G_MSG0073W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTKIN_NO.Focus()
                    Exit Function
                End If

                '科目
                '未選択チェック
                If cmbKAMOKU.SelectedIndex = -1 Then
                    MessageBox.Show(G_MSG0055W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    cmbKAMOKU.Focus()
                    Exit Function
                End If

                '口座番号
                '未入力チェック
                If Trim(txtKOUZA.Text) = "" Then
                    MessageBox.Show(MSG0123W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtKOUZA.Focus()
                    Exit Function
                End If

                'チェックデジット追加 2007/02/13
                If STR_CHK_DJT = "1" Then '<---iniファイルのチェックデジット判定区分 0:しない 1:する
                    If txtTKIN_NO.Text.Trim = STR_JIKINKO_CODE Then '自金庫データのみチェックデジット実行
                        If GFUNC_CHK_DEJIT(txtTKIN_NO.Text.Trim, txtTSIT_NO.Text.Trim, CInt(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU)), txtKOUZA.Text.Trim) = False Then
                            MessageBox.Show(G_MSG0074W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtKOUZA.Focus()
                            Exit Function
                        End If
                    End If
                End If
        End Select

        '口座名義人（カナ）
        '未入力チェック
        If Trim(txtMEIGI_KNAME.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "口座名義人(カナ)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtMEIGI_KNAME.Focus()
            Exit Function
        Else
            If StrConv(txtMEIGI_KNAME.Text, VbStrConv.Narrow).Length > 40 Then
                MessageBox.Show(G_MSG0088W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtMEIGI_KNAME.Focus()
                Exit Function
            End If

        End If


        '任意項目チェック追加 2006/05/10
        '生徒氏名（漢字）
        If Trim(txtSEITO_NNAME.Text) <> "" Then
            If StrConv(txtSEITO_NNAME.Text, VbStrConv.Wide).Length > 25 Then
                MessageBox.Show(G_MSG0075W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSEITO_NNAME.Focus()
                Exit Function
            End If

        End If


        '口座名義人氏名（漢字）
        If Trim(txtMEIGI_NNAME.Text) <> "" Then
            If StrConv(txtMEIGI_NNAME.Text, VbStrConv.Wide).Length > 25 Then
                MessageBox.Show(G_MSG0076W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtMEIGI_NNAME.Focus()
                Exit Function
            End If

        End If

        '契約住所（漢字）
        If Trim(txtKEIYAKU_NJYU.Text) <> "" Then
            If StrConv(txtKEIYAKU_NJYU.Text, VbStrConv.Wide).Length > 25 Then
                MessageBox.Show(G_MSG0077W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKEIYAKU_NJYU.Focus()
                Exit Function
            End If
        End If

        '進級区分
        '未選択チェック
        If cmbSINKYU_KBN.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0078W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbSINKYU_KBN.Focus()
            Exit Function
        End If

        '解約区分
        '未選択チェック
        If cmbKAIYAKU_FLG.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0079W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbKAIYAKU_FLG.Focus()
            Exit Function
        End If

        '費目情報
        '未選択チェック
        If cmbHIMOKU.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0080W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbHIMOKU.Focus()
            Exit Function
        End If

        '長子情報
        Select Case cmbTYOUSI.SelectedIndex
            Case -1
                MessageBox.Show(G_MSG0081W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbTYOUSI.Focus()
                Exit Function
            Case 1
                '長子有り選択時
                If Trim(str長子学校コード) = "" Or _
                   Trim(str長子入学年度) = "" Or _
                   Trim(str長子通番) = "" Or _
                   Trim(str長子学年) = "" Or _
                   Trim(str長子クラス) = "" Or _
                   Trim(str長子生徒番号) = "" Then
                    MessageBox.Show(String.Format(MSG0281W, "長子情報"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    cmbTYOUSI.Focus()

                    Exit Function
                End If
        End Select

        fn_CheckEntry = True

    End Function
    Private Function fn_CheckG_TakoMastFromGakkouCodeTKinNo() As Boolean
        If Trim(txtTKIN_NO.Text) <> "" And Trim(txtGAKKOU_CODE.Text) <> "" Then
            If STR_JIKINKO_CODE <> Trim(txtTKIN_NO.Text) Then
                Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
                Dim SQL As String = ""
                SQL = " SELECT * FROM G_TAKOUMAST"
                SQL &= " WHERE GAKKOU_CODE_V ='" & Trim(txtGAKKOU_CODE.Text) & "'"
                SQL &= " AND TKIN_NO_V ='" & Trim(txtTKIN_NO.Text) & "'"

                If Not OraReader.DataReader(SQL) Then
                    '他行扱いで存在
                    OraReader.Close()
                    OraReader = Nothing

                    Return False
                End If

                OraReader.Close()
                OraReader = Nothing
            End If
        End If

        Return True

    End Function
    Private Function fn_CheckGakMast2FromGakkouCodeTKinNo() As Integer
        '金融機関が学校マスタ２に存在するかチェク
        fn_CheckGakMast2FromGakkouCodeTKinNo = 0

        If Trim(txtTKIN_NO.Text) <> "" And Trim(txtGAKKOU_CODE.Text) <> "" Then

            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As String = ""
            SQL = " SELECT * FROM GAKMAST2"
            SQL += " WHERE GAKKOU_CODE_T ='" & Trim(txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c)) & "'"
            SQL += " AND TKIN_NO_T ='" & Trim(txtTKIN_NO.Text) & "'"

            If OraReader.DataReader(SQL) Then
                '自行扱いで存在
            Else
                fn_CheckGakMast2FromGakkouCodeTKinNo = 1
            End If

            OraReader.Close()
            OraReader = Nothing
        End If

    End Function
    Private Function fn_CheckEntryKouza() As Boolean
        lblKOUZA_CHK.Text = ""
        fn_CheckEntryKouza = False

        '取扱金融機関　
        '取扱金融機関
        '未入力チェック
        If Trim(txtTKIN_NO.Text) = "" Then
            MessageBox.Show(MSG0032W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTKIN_NO.Focus()
            Exit Function
        End If

        '取扱支店
        '未入力チェック
        If Trim(txtTSIT_NO.Text) = "" Then
            MessageBox.Show(MSG0035W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTSIT_NO.Focus()
            Exit Function
        End If

        '科目
        '未選択チェック
        If cmbKAMOKU.SelectedIndex = -1 Then
            MessageBox.Show(G_MSG0055W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbKAMOKU.Focus()
            Exit Function
        End If

        '口座番号
        '未入力チェック
        If Trim(txtKOUZA.Text) = "" Then
            MessageBox.Show(MSG0123W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtKOUZA.Focus()
            Exit Function
        End If

        '自金庫コードの場合のみ口座チェック実行
        If txtTKIN_NO.Text <> STR_JIKINKO_CODE Then
            lblKOUZA_CHK.Text = ""
            fn_CheckEntryKouza = True
            Exit Function
        End If


        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As String = ""
        '使用学年チェック
        SQL = "SELECT KIGYO_CODE_T,FURI_CODE_T"
        SQL += " FROM GAKMAST2"
        SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

        If Not OraReader.DataReader(SQL) Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKUNEN_CODE.Focus()
            Exit Function
        Else
            strKIGYO_CODE = OraReader.GetItem("KIGYO_CODE_T")
            strFURI_CODE = OraReader.GetItem("FURI_CODE_T")
        End If

        '口座情報マスタを検索し、口座が存在するか、自振契約があるか検索
        '口座があり、口座名義人（カナ）が未入力の場合は、口座情報マスタの値を表示

        '企業コード、振替コードが一致するものがあるかどうか検索
        OraReader.Close()
        OraReader = Nothing
        OraReader = New CASTCommon.MyOracleReader(MainDB)
        SQL = " SELECT * FROM KDBMAST"
        SQL += " WHERE TSIT_NO_D =" & SQ(Trim(txtTSIT_NO.Text))
        SQL += " AND KAMOKU_D =" & SQ(CStr(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU)))
        SQL += " AND KOUZA_D =" & SQ(Trim(txtKOUZA.Text))
        SQL += " AND KIGYOU_CODE_D =" & SQ(strKIGYO_CODE)
        SQL += " AND FURI_CODE_D =" & SQ(strFURI_CODE)

        If Not OraReader.DataReader(SQL) Then
        Else
            lblKOUZA_CHK.Text = ""
            '2008/09/15 蒲郡信金　解約チェックを追加===============
            If OraReader.GetItem("KATU_KOUZA_D") = "0" Then
                lblKOUZA_CHK.Text = "口座解約済"
                fn_CheckEntryKouza = True
                Exit Function
            End If
            '======================================================
            If Trim(txtMEIGI_KNAME.Text) = "" Then
                If Trim(OraReader.GetItem("KOKYAKU_KNAME_D")).Length > 40 Then
                    txtMEIGI_KNAME.Text = Trim(OraReader.GetItem("KOKYAKU_KNAME_D")).Substring(0, 40)
                Else
                    txtMEIGI_KNAME.Text = Trim(OraReader.GetItem("KOKYAKU_KNAME_D"))
                End If
            Else
                If Trim(txtMEIGI_KNAME.Text) <> Trim(OraReader.GetItem("KOKYAKU_KNAME_D")) Then
                    MessageBox.Show(String.Format(G_MSG0083W, Trim(txtMEIGI_KNAME.Text), OraReader.GetItem("KOKYAKU_KNAME_D")), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lblKOUZA_CHK.Text = "カナ不一致"
                Else
                    lblKOUZA_CHK.Text = ""
                End If
            End If
            fn_CheckEntryKouza = True
            Exit Function
        End If

        '企業コード、振替コードが一致するものがなかったとき、口座が一致するものがあるか検索
        OraReader.Close()
        OraReader = Nothing
        OraReader = New CASTCommon.MyOracleReader(MainDB)
        SQL = " SELECT * FROM KDBMAST"
        SQL += " WHERE TSIT_NO_D =" & SQ(Trim(txtTSIT_NO.Text))
        SQL += " AND KAMOKU_D =" & SQ(CStr(GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_SEITO_KAMOKU_TXT, cmbKAMOKU)))
        SQL += " AND KOUZA_D =" & SQ(Trim(txtKOUZA.Text))

        If Not OraReader.DataReader(SQL) Then
            lblKOUZA_CHK.Text = "口座なし"
            fn_CheckEntryKouza = True
            Exit Function
        Else
            '2008/09/15 蒲郡信金　解約チェックを追加===============
            If OraReader.GetItem("KATU_KOUZA_D") = "0" Then
                lblKOUZA_CHK.Text = "口座解約済"
                fn_CheckEntryKouza = True
                Exit Function
            End If
            '======================================================

            lblKOUZA_CHK.Text = "自振契約なし"
            If Trim(txtMEIGI_KNAME.Text) = "" Then
                txtMEIGI_KNAME.Text = Mid(OraReader.GetItem("KOKYAKU_KNAME_D").ToString.Trim, 1, 40)
            Else
                lblKOUZA_CHK.Text = ""
                If Trim(txtMEIGI_KNAME.Text) <> Trim(OraReader.GetItem("KOKYAKU_KNAME_D")) Then
                    MessageBox.Show(String.Format(G_MSG0083W, Trim(txtMEIGI_KNAME.Text), OraReader.GetItem("KOKYAKU_KNAME_D")), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lblKOUZA_CHK.Text = "契約無ｶﾅ不一致"
                Else
                    lblKOUZA_CHK.Text = "自振契約なし"
                End If
            End If
            fn_CheckEntryKouza = True
            Exit Function
        End If
    End Function
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '学校名コンボボックス設定
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = True Then
            cmbGakkouName.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        'COMBOBOX選択時学校名,学校コード設定
        lab学校名.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex)

        '入学年度テキストボックスにFOCUS
        txtNENDO.Focus()

        If txtGAKUNEN_CODE.Text.Trim <> "" Then
            '学年名の取得
            If fn_GetGakunenName() = True Then
                '学校コードと学年から費目レコード（費目ＩＤ／費目パターン名）を取得
                Call Sub_GetHimokuID()
                Call Sub_Himoku_Initialize()
            End If
        End If
    End Sub
    Private Sub cmbTYOUSI_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbTYOUSI.SelectedIndexChanged

        Select Case cmbTYOUSI.SelectedIndex
            Case 0
                btnTyousi.Enabled = False
            Case Else
                btnTyousi.Enabled = True
        End Select

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            Dim SQL As String = ""
            '学校名検索
            SQL = "SELECT GAKKOU_NNAME_G FROM GAKMAST1 "
            SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
            If Not OraReader.DataReader(SQL) Then
                lab学校名.Text = ""
            Else

                lab学校名.Text = CStr(OraReader.GetItem("GAKKOU_NNAME_G"))
            End If
        End If

    End Sub
    Private Sub txtCLASS_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCLASS_CODE.Validating
        If Trim(txtCLASS_CODE.Text) <> "" Then
            'クラス名の取得
            Call fn_GetClassName()
        End If
    End Sub
    Private Sub txtTKIN_NO_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTKIN_NO.Validating

        '金融機関
        If Trim(txtTKIN_NO.Text) <> "" Then
            '金融機関名、支店名の設定
            If fn_GetKinNameSitName() <> 0 Then
                Exit Sub
            End If
            '学校マスタ２チェック
            If fn_CheckGakMast2FromGakkouCodeTKinNo() = 0 Then
            Else
                '他行マスタチェック
                If fn_CheckG_TakoMastFromGakkouCodeTKinNo() = True Then
                Else
                    MessageBox.Show(G_MSG0073W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
        End If

    End Sub
    Private Sub txtTSIT_NO_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTSIT_NO.Validating
        '支店
        If Trim(txtTSIT_NO.Text) <> "" Then
            '金融機関名、支店名の設定
            If fn_GetKinNameSitName() <> 0 Then
                Exit Sub
            End If
            '学校マスタ２チェック
            If fn_CheckGakMast2FromGakkouCodeTKinNo() = 0 Then
            Else
                '他行マスタチェック
                If fn_CheckG_TakoMastFromGakkouCodeTKinNo() = True Then
                Else
                    MessageBox.Show(G_MSG0073W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
        End If

    End Sub
    Private Sub txtGAKUNEN_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKUNEN_CODE.Validating
        '学年名の取得
        If fn_GetGakunenName() = True Then
            '学校コードと学年から費目レコード（費目ＩＤ／費目パターン名）を取得
            cmbHIMOKU.Items.Clear()
            Call Sub_GetHimokuID()
            '費目情報４月から翌３月のクリア
            Call Sub_Himoku_Initialize()
        End If
    End Sub
    Private Sub txtTUUBAN_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTUUBAN.Validating
        If cmbHIMOKU.Items.Count = 0 AndAlso lab学年名.Text.Trim <> "" AndAlso lab学校名.Text.Trim <> "" Then
            '学校コードと学年から費目レコード（費目ＩＤ／費目パターン名）を取得
            cmbHIMOKU.Items.Clear()
            Call Sub_GetHimokuID()
        End If
    End Sub
    '全角混入領域バイト数調整用
    Private Sub GetLimitString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles txtKEIYAKU_NJYU.Validating, txtSEITO_NNAME.Validating, txtMEIGI_NNAME.Validating
        With CType(sender, TextBox)
            .Text = GCom.GetLimitString(.Text, .MaxLength)
        End With
    End Sub
    Private colNo, rowNo As Integer
    Private TextEditCtrl As DataGridViewTextBoxEditingControl
    Private CmbEditCtrl As DataGridViewComboBoxEditingControl
    Private Sub CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        colNo = e.ColumnIndex
        rowNo = e.RowIndex

        Select Case e.ColumnIndex
            Case 0
                CType(sender, DataGridView).ImeMode = ImeMode.Hiragana
            Case 1, 2
                CType(sender, DataGridView).ImeMode = ImeMode.Disable
        End Select
    End Sub
    Private Sub EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs)
        If colNo = 1 Then
            CmbEditCtrl = CType(e.Control, DataGridViewComboBoxEditingControl)
        Else
            TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)
        End If
        Select Case colNo
            Case 0
                AddHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                AddHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
            Case 1
                AddHandler CmbEditCtrl.GotFocus, AddressOf CAST.GotFocus
                AddHandler CmbEditCtrl.KeyPress, AddressOf CAST.KeyPress
            Case 2
                AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
        End Select
    End Sub
    Private Sub CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        Select Case colNo
            Case 0
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CAST.GotFocus
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CAST.KeyPress
            Case 1
                RemoveHandler CmbEditCtrl.GotFocus, AddressOf CAST.GotFocus
                RemoveHandler CmbEditCtrl.KeyPress, AddressOf CAST.KeyPress
            Case 2
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
        End Select

        Call CellLeave(sender, e)
    End Sub
    Private Sub Sub_DataGridViewComboLock(ByVal dgv As DataGridView, ByVal RowNo As Integer, ByVal Tuki As Integer)

        '請求方法の選択による金額の変更可・不可の設定
        With dgv
            Select Case .Rows(RowNo).Cells(1).Value
                Case "一律"
                    .Rows(RowNo).Cells(2).Value = Format(CDec(Str_Spread(Tuki, RowNo)), "#,##0")
                    .Rows(RowNo).Cells(2).ReadOnly = True
                Case "個別"
                    .Rows(RowNo).Cells(2).ReadOnly = False
            End Select
        End With

    End Sub
    Private Sub CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Select Case CType(sender, DataGridView).Name
            Case "CustomDataGridView1"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 3)
            Case "CustomDataGridView2"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 4)
            Case "CustomDataGridView3"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 5)
            Case "CustomDataGridView4"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 6)
            Case "CustomDataGridView5"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 7)
            Case "CustomDataGridView6"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 8)
            Case "CustomDataGridView7"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 9)
            Case "CustomDataGridView8"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 10)
            Case "CustomDataGridView9"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 11)
            Case "CustomDataGridView10"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 0)
            Case "CustomDataGridView11"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 1)
            Case "CustomDataGridView12"
                Call Sub_DataGridViewComboLock(CType(sender, DataGridView), e.RowIndex, 2)
        End Select
    End Sub
    Private Sub CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        Dim SumKingaku As Decimal = 0

        For cnt As Integer = 0 To CType(sender, DataGridView).Rows.Count - 1
            If IsNumeric(CType(sender, DataGridView).Rows(cnt).Cells(2).Value) Then
                SumKingaku += CDec(CType(sender, DataGridView).Rows(cnt).Cells(2).Value)
                '2017/08/04 タスク）綾部 ADD 標準版修正（個別金額の0円設定不具合）---------------------- START
            Else
                CType(sender, DataGridView).Rows(cnt).Cells(2).Value = "0"
                '2017/08/04 タスク）綾部 ADD 標準版修正（個別金額の0円設定不具合）---------------------- END
            End If
        Next

        Select Case CType(sender, DataGridView).Name
            Case "CustomDataGridView1"
                lblKingaku_4.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView2"
                lblKingaku_5.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView3"
                lblKingaku_6.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView4"
                lblKingaku_7.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView5"
                lblKingaku_8.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView6"
                lblKingaku_9.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView7"
                lblKingaku_10.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView8"
                lblKingaku_11.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView9"
                lblKingaku_12.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView10"
                lblKingaku_1.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView11"
                lblKingaku_2.Text = Format(SumKingaku, "#,##0")
            Case "CustomDataGridView12"
                lblKingaku_3.Text = Format(SumKingaku, "#,##0")
        End Select

        If e.ColumnIndex = 2 Then
            If IsNumeric(CType(sender, DataGridView).Rows(e.RowIndex).Cells(2).Value) Then
                CType(sender, DataGridView).Rows(e.RowIndex).Cells(2).Value = Format(CDec(CType(sender, DataGridView).Rows(e.RowIndex).Cells(2).Value), "#,##0")
            End If
        End If

    End Sub
    Private Function ConvDBNullToString(ByVal Item As Object) As String
        If IsDBNull(Item) Then
            Return ""
        Else
            Return Item.ToString.Trim
        End If
    End Function
    '2017/05/09 タスク）西野 CHG 標準版修正（費目設定の一括変更対応）---------------------- START
    Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles _
      CustomDataGridView1.RowPostPaint, CustomDataGridView2.RowPostPaint, CustomDataGridView3.RowPostPaint, _
      CustomDataGridView4.RowPostPaint, CustomDataGridView5.RowPostPaint, CustomDataGridView6.RowPostPaint, _
      CustomDataGridView7.RowPostPaint, CustomDataGridView8.RowPostPaint, CustomDataGridView9.RowPostPaint, _
      CustomDataGridView10.RowPostPaint, CustomDataGridView11.RowPostPaint, CustomDataGridView12.RowPostPaint, CustomDataGridView13.RowPostPaint
        ' Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles _
        'CustomDataGridView1.RowPostPaint, CustomDataGridView2.RowPostPaint, CustomDataGridView3.RowPostPaint, _
        'CustomDataGridView4.RowPostPaint, CustomDataGridView5.RowPostPaint, CustomDataGridView6.RowPostPaint, _
        'CustomDataGridView7.RowPostPaint, CustomDataGridView8.RowPostPaint, CustomDataGridView9.RowPostPaint, _
        'CustomDataGridView10.RowPostPaint, CustomDataGridView11.RowPostPaint, CustomDataGridView12.RowPostPaint
        '2017/05/09 タスク）西野 CHG 標準版修正（費目設定の一括変更対応）---------------------- END

        Dim dgv As DataGridView = CType(sender, DataGridView)

        ' 行ヘッダのセル領域を、行番号を描画する長方形とする
        ' （ただし右端に4ドットのすき間を空ける）
        Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, dgv.Rows(e.RowIndex).Height)

        ' 上記の長方形内に行番号を縦方向中央＆右詰で描画する
        ' フォントや色は行ヘッダの既定値を使用する
        TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, _
                              rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
                              Or TextFormatFlags.Right)

    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

    '2017/05/09 タスク）西野 ADD 標準版修正（費目設定の一括変更対応）---------------------- START
    Private Sub DataGridView13_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles CustomDataGridView13.CellContentClick
        Dim dgv_H As DataGridView = CType(sender, CustomDataGridView)

        If dgv_H.Rows(e.RowIndex).Cells(0).Value = "" Then
            Exit Sub
        End If

        Dim HimoName As String = Trim(dgv_H.Rows(e.RowIndex).Cells(0).Value)
        Dim Seikyu As String = ""
        Select Case e.ColumnIndex
            Case 1  '一律
                Seikyu = "一律"
            Case 2  '個別
                Seikyu = "個別"
        End Select

        If MessageBox.Show(HimoName & "の請求方法を一括で" & Seikyu & "に変更します。" & vbCrLf & "よろしいですか？", "請求方法一括変更", _
                         MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> DialogResult.OK Then
            Exit Sub
        End If

        For TUKI As Integer = 1 To 12
            Dim dgv As DataGridView = Nothing
            Dim kei As Label = Nothing
            Select Case TUKI.ToString.PadLeft(2, "0"c)
                Case "04"
                    dgv = CustomDataGridView1
                    kei = lblKingaku_4
                Case "05"
                    dgv = CustomDataGridView2
                    kei = lblKingaku_5
                Case "06"
                    dgv = CustomDataGridView3
                    kei = lblKingaku_6
                Case "07"
                    dgv = CustomDataGridView4
                    kei = lblKingaku_7
                Case "08"
                    dgv = CustomDataGridView5
                    kei = lblKingaku_8
                Case "09"
                    dgv = CustomDataGridView6
                    kei = lblKingaku_9
                Case "10"
                    dgv = CustomDataGridView7
                    kei = lblKingaku_10
                Case "11"
                    dgv = CustomDataGridView8
                    kei = lblKingaku_11
                Case "12"
                    dgv = CustomDataGridView9
                    kei = lblKingaku_12
                Case "01"
                    dgv = CustomDataGridView10
                    kei = lblKingaku_1
                Case "02"
                    dgv = CustomDataGridView11
                    kei = lblKingaku_2
                Case "03"
                    dgv = CustomDataGridView12
                    kei = lblKingaku_3
            End Select

            Dim lngGoukei As Long = 0

            With dgv
                Select Case e.ColumnIndex
                    Case 1  '一律
                        .Rows(e.RowIndex).Cells(1).Value = "一律"
                        .Rows(e.RowIndex).Cells(2).Value = Str_Spread(0, e.RowIndex)
                        .Rows(e.RowIndex).Cells(2).ReadOnly = True
                    Case 2  '個別
                        .Rows(e.RowIndex).Cells(1).Value = "個別"
                        .Rows(e.RowIndex).Cells(2).ReadOnly = False
                        .Rows(e.RowIndex).Cells(2).Value = 0
                End Select

                For intRow As Integer = 0 To intMaxHimokuCnt - 1
                    lngGoukei += CLng(.Rows(intRow).Cells(2).Value)
                Next
                kei.Text = Format(lngGoukei, "#,##0")
            End With

        Next

    End Sub
    '2017/05/09 タスク）西野 ADD 標準版修正（費目設定の一括変更対応）---------------------- END

    '2017/06/05 saitou ADD 標準版修正（修正No.0004【生徒名カナ、口座名義人カナに全角が入る】） ---------------------------------------- START
    Private Sub NzCheckString_Validating(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles _
        txtSEITO_KNAME.Validating, txtMEIGI_KNAME.Validating
        Try
            Call GCom.NzCheckString(CType(sender, TextBox))
            Dim BRet As Boolean = GCom.CheckZenginChar(CType(sender, TextBox))
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("カナ変換", "失敗", ex.ToString)
        End Try
    End Sub
    '2017/06/05 saitou ADD 標準版修正 ------------------------------------------------------------------------------------------------- END

#Region "帳票印刷"
    '============================================================================
    'NAME           :fn_CreateCSV_ADDDEL
    'Parameter      :PntSyoriKbn 印刷処理区分
    'Description    :KFGP106,108(生徒マスタメンテ(登録,削除))印刷用ＣＳＶファイル作成
    'Return         :
    'Create         :2010/09/16
    'Update         :
    '============================================================================
    Private Function fn_CreateCSV_ADDDEL(ByVal PntSyoriKbn As String) As Boolean

        '2017/04/05 saitou 東春信金(RSV2標準) UPD 生徒マスタメンテ帳票改善 ---------------------------------------- START
        Dim oraMainReader As CASTCommon.MyOracleReader = Nothing
        Dim oraHimoReader As CASTCommon.MyOracleReader = Nothing

        Dim SQL As New StringBuilder

        Try
            '2010/10/18 登録、削除クラスの判別
            Dim CreateCSV As Object = Nothing
            Select Case PntSyoriKbn
                Case "0" : CreateCSV = New KFGP106
                Case "2" : CreateCSV = New KFGP108
                Case Else
            End Select
            Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
            Dim NowTime As String = Format(DateTime.Now, "HHmmss")

            '生徒マスタビューから生徒情報取得
            '費目情報はここでは取得しない
            With SQL
                .Append("select distinct ")
                .Append(" GAKKOU_CODE_O")
                .Append(",NENDO_O")
                .Append(",TUUBAN_O")
                .Append(",GAKUNEN_CODE_O")
                .Append(",CLASS_CODE_O")
                .Append(",SEITO_NO_O")
                .Append(",SEITO_KNAME_O")
                .Append(",SEITO_NNAME_O")
                .Append(",SEIBETU_O")
                .Append(",TKIN_NO_O")
                .Append(",TSIT_NO_O")
                .Append(",KAMOKU_O")
                .Append(",KOUZA_O")
                .Append(",MEIGI_KNAME_O")
                .Append(",MEIGI_NNAME_O")
                .Append(",FURIKAE_O")
                .Append(",KEIYAKU_NJYU_O")
                .Append(",KEIYAKU_DENWA_O")
                .Append(",KAIYAKU_FLG_O")
                .Append(",SINKYU_KBN_O")
                .Append(",HIMOKU_ID_O")
                .Append(",GAKKOU_NNAME_G")
                .Append(",TENMAST_G.SIT_NNAME_N as G_SIT_NNAME")
                .Append(",TENMAST_S.KIN_NNAME_N as S_KIN_NNAME")
                .Append(",TENMAST_S.SIT_NNAME_N as S_SIT_NNAME")

                .Append(" from SEITOMASTVIEW, GAKMAST1, GAKMAST2, TENMAST TENMAST_G, TENMAST TENMAST_S")

                .Append(" where GAKKOU_CODE_O = GAKKOU_CODE_G")
                .Append(" and GAKUNEN_CODE_O = GAKUNEN_CODE_G")

                .Append(" and GAKKOU_CODE_G = GAKKOU_CODE_T")

                .Append(" and TKIN_NO_T = TENMAST_G.KIN_NO_N")
                .Append(" and TSIT_NO_T = TENMAST_G.SIT_NO_N")

                '集金扱いは金融機関入力せずとも登録できるため外部結合
                .Append(" and TKIN_NO_O = TENMAST_S.KIN_NO_N(+)")
                .Append(" and TSIT_NO_O = TENMAST_S.SIT_NO_N(+)")

                .Append(" and GAKKOU_CODE_O = " & SQ(Me.SeitomastMenteListInfo.GakkouCode))
                .Append(" and NENDO_O = " & SQ(Me.SeitomastMenteListInfo.Nendo))
                .Append(" and TUUBAN_O = " & GCom.NzInt(Me.SeitomastMenteListInfo.Tuuban))

            End With

            oraMainReader = New CASTCommon.MyOracleReader(Me.MainDB)

            If oraMainReader.DataReader(SQL) = True Then
                'CSV作成
                Me.strCSV_FILE_NAME = CreateCSV.CreateCsvFile()

                oraHimoReader = New CASTCommon.MyOracleReader(Me.MainDB)

                While oraMainReader.EOF = False

                    '費目情報取得 
                    For i As Integer = 1 To 15
                        With SQL
                            .Length = 0
                            .Append("select ")
                            .Append(" KINGAKU" & i.ToString("00") & "_O")
                            .Append(",HIMOKU_NAME" & i.ToString("00") & "_O")
                            .Append(" from SEITOMASTVIEW")
                            .Append(" where GAKKOU_CODE_O = " & SQ(Me.SeitomastMenteListInfo.GakkouCode))
                            .Append(" and NENDO_O = " & SQ(Me.SeitomastMenteListInfo.Nendo))
                            .Append(" and TUUBAN_O = " & GCom.NzInt(Me.SeitomastMenteListInfo.Tuuban))
                            .Append(" order by TUKI_NO_O")
                        End With

                        If oraHimoReader.DataReader(SQL) = True Then
                            With CreateCSV
                                .OutputCsvData(PntSyoriKbn, True)
                                .OutputCsvData(Today, True)
                                .OutputCsvData(NowTime, True)
                                .OutputCsvData(oraMainReader.GetString("G_SIT_NNAME"), True)
                                .OutputCsvData(oraMainReader.GetString("GAKKOU_CODE_O"), True)
                                .OutputCsvData(oraMainReader.GetString("GAKKOU_NNAME_G"), True)
                                .OutputCsvData(oraMainReader.GetString("NENDO_O"), True)
                                .OutputCsvData(oraMainReader.GetString("GAKUNEN_CODE_O"), True)
                                .OutputCsvData(oraMainReader.GetString("TUUBAN_O"), True)
                                .OutputCsvData(oraMainReader.GetString("SEITO_NO_O"), True)
                                .OutputCsvData(oraMainReader.GetString("SEITO_KNAME_O"), True)
                                .OutputCsvData(oraMainReader.GetString("SEITO_NNAME_O"), True)
                                .OutputCsvData(oraMainReader.GetString("MEIGI_KNAME_O"), True)
                                .OutputCsvData(oraMainReader.GetString("MEIGI_NNAME_O"), True)
                                .OutputCsvData(oraMainReader.GetString("TKIN_NO_O"), True)
                                .OutputCsvData(oraMainReader.GetString("S_KIN_NNAME"), True)
                                .OutputCsvData(oraMainReader.GetString("TSIT_NO_O"), True)
                                .OutputCsvData(oraMainReader.GetString("S_SIT_NNAME"), True)
                                Select Case oraMainReader.GetString("KAMOKU_O")
                                    Case "02" : .OutputCsvData("普通", True)
                                    Case "01" : .OutputCsvData("当座", True)
                                    Case Else : .OutputCsvData("", True)
                                End Select
                                .OutputCsvData(oraMainReader.GetString("KOUZA_O"), True)
                                .OutputCsvData(oraMainReader.GetString("CLASS_CODE_O"), True)
                                Select Case oraMainReader.GetString("SEIBETU_O")
                                    Case "0" : .OutputCsvData("男", True)
                                    Case "1" : .OutputCsvData("女", True)
                                    Case "2" : .OutputCsvData("－", True)
                                End Select
                                Select Case oraMainReader.GetString("FURIKAE_O")
                                    Case "0" : .OutputCsvData("口座振替", True)
                                    Case "1" : .OutputCsvData("集金扱い", True)
                                    Case "2" : .OutputCsvData("停止", True)
                                End Select
                                Select Case oraMainReader.GetString("KAIYAKU_FLG_O")
                                    Case "0" : .OutputCsvData("通常", True)
                                    Case "9" : .OutputCsvData("解約", True)
                                End Select
                                .OutputCsvData(oraMainReader.GetString("KEIYAKU_DENWA_O"), True)
                                Select Case oraMainReader.GetString("SINKYU_KBN_O")
                                    Case "0" : .OutputCsvData("進級する", True)
                                    Case "1" : .OutputCsvData("進級しない", True)
                                End Select
                                .OutputCsvData(oraMainReader.GetString("KEIYAKU_NJYU_O"), True)
                                .OutputCsvData(oraMainReader.GetString("HIMOKU_ID_O"), True)

                                .OutputCsvData(oraHimoReader.GetString("HIMOKU_NAME" & i.ToString("00") & "_O"))
                                Dim bHimokuHyoujiFlg As Boolean = False
                                If oraHimoReader.GetString("HIMOKU_NAME" & i.ToString("00") & "_O") = String.Empty Then
                                    bHimokuHyoujiFlg = False
                                Else
                                    bHimokuHyoujiFlg = True
                                End If

                                While oraHimoReader.EOF = False
                                    If bHimokuHyoujiFlg = True Then
                                        .OutputCsvData(oraHimoReader.GetString("KINGAKU" & i.ToString("00") & "_O"))
                                    Else
                                        .OutputCsvData("")
                                    End If
                                    oraHimoReader.NextRead()
                                End While

                                If bHimokuHyoujiFlg = True Then
                                    .OutputCsvData("1", True, True)
                                Else
                                    .OutputCsvData("0", True, True)
                                End If

                            End With

                            oraHimoReader.Close()

                        End If
                    Next

                    oraMainReader.NextRead()
                End While

            Else
                Select Case PntSyoriKbn
                    Case "0"
                        MessageBox.Show(String.Format(MSG0231W, "生徒マスタメンテ(登録)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", "印刷対象なし")
                    Case "2"
                        MessageBox.Show(String.Format(MSG0231W, "生徒マスタメンテ(削除)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", "印刷対象なし")
                    Case Else

                End Select

                Return False

            End If

            CreateCSV.CloseCsv()
            CreateCSV = Nothing

            Return True

        Catch ex As Exception
            Select Case PntSyoriKbn
                Case "0"
                    MessageBox.Show(String.Format(MSG0231W, "生徒マスタメンテ(登録)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", ex.Message)
                Case "2"
                    MessageBox.Show(String.Format(MSG0231W, "生徒マスタメンテ(削除)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", ex.Message)
                Case Else

            End Select
            Return False
        Finally
            If Not oraMainReader Is Nothing Then
                oraMainReader.Close()
                oraMainReader = Nothing
            End If

            If Not oraHimoReader Is Nothing Then
                oraHimoReader.Close()
                oraHimoReader = Nothing
            End If
        End Try

        'Dim i As Integer = 0
        'Dim REC As OracleDataReader = Nothing
        'Dim REC2 As OracleDataReader = Nothing

        'Try
        '    '2010/10/18 登録、削除クラスの判別
        '    'Dim CreateCSV As New KFGP106
        '    Dim CreateCSV As Object = Nothing
        '    Select Case PntSyoriKbn
        '        Case "0" : CreateCSV = New KFGP106
        '        Case "2" : CreateCSV = New KFGP108
        '        Case Else
        '    End Select
        '    Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
        '    Dim NowTime As String = Format(DateTime.Now, "HHmmss")
        '    Dim SQL As New StringBuilder(128)

        '    '生徒マスタからSEIKYU01_O～SEIKYU15_O、KINGAKU01_O～KINGAKU15_Oを取得
        '    Dim SeitoSEIKYU(12, 15) As String
        '    Dim SeitoKINGAKU(12, 15) As String
        '    SQL.Append("SELECT * ")
        '    SQL.Append(" FROM SEITOMAST")
        '    SQL.Append(" WHERE SEITOMAST.GAKKOU_CODE_O = " & SQ(txtGAKKOU_CODE.Text))
        '    SQL.Append(" AND SEITOMAST.NENDO_O = " & SQ(txtNENDO.Text))
        '    SQL.Append(" AND SEITOMAST.TUUBAN_O = " & SQ(txtTUUBAN.Text))
        '    SQL.Append(" ORDER BY SEITOMAST.TUKI_NO_O asc")

        '    'SQL実行
        '    If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then
        '        Do
        '            For j As Integer = 0 To 14
        '                SeitoSEIKYU(i, j) = GCom.NzStr(REC.Item("SEIKYU" & (j + 1).ToString("00") & "_O"))
        '                SeitoKINGAKU(i, j) = GCom.NzStr(REC.Item("KINGAKU" & (j + 1).ToString("00") & "_O"))
        '            Next
        '            i = i + 1
        '        Loop Until REC.Read = False
        '    End If

        '    'CSV作成用SQLを作成
        '    Dim StData(12, 56) As String       '※今後カスタマイズ時注意！
        '    Dim SQL2 As New StringBuilder(128)

        '    SQL2.Append("SELECT DISTINCT")
        '    SQL2.Append(" TENMAST_G.SIT_NNAME_N AS G_SIT_NNAME")   'とりまとめ店名
        '    SQL2.Append(",SEITOMAST.GAKKOU_CODE_O")      '学校コード
        '    SQL2.Append(",GAKMAST1.GAKKOU_NNAME_G")      '学校名
        '    SQL2.Append(",SEITOMAST.NENDO_O")            '入学年度
        '    SQL2.Append(",SEITOMAST.GAKUNEN_CODE_O")     '学年
        '    SQL2.Append(",SEITOMAST.TUUBAN_O")           '通番
        '    SQL2.Append(",SEITOMAST.SEITO_NO_O")         '生徒番号
        '    SQL2.Append(",SEITOMAST.SEITO_KNAME_O")      '生徒氏名カナ
        '    SQL2.Append(",SEITOMAST.SEITO_NNAME_O")      '生徒氏名漢字
        '    SQL2.Append(",SEITOMAST.MEIGI_KNAME_O")      '口座名義カナ
        '    SQL2.Append(",SEITOMAST.MEIGI_NNAME_O")      '口座名義漢字
        '    SQL2.Append(",SEITOMAST.TKIN_NO_O")          '取扱金融機関コード
        '    SQL2.Append(",TENMAST_S.KIN_NNAME_N")        '取扱金融機関名
        '    SQL2.Append(",SEITOMAST.TSIT_NO_O")          '取扱支店コード
        '    SQL2.Append(",TENMAST_S.SIT_NNAME_N AS S_SIT_NNAME")          '取扱支店名
        '    SQL2.Append(",SEITOMAST.KAMOKU_O")           '科目※
        '    SQL2.Append(",SEITOMAST.KOUZA_O")            '口座番号
        '    SQL2.Append(",SEITOMAST.CLASS_CODE_O")       'クラス
        '    SQL2.Append(",SEITOMAST.SEIBETU_O")          '性別※
        '    SQL2.Append(",SEITOMAST.FURIKAE_O")          '振替方法※
        '    SQL2.Append(",SEITOMAST.KAIYAKU_FLG_O")      '解約区分※
        '    SQL2.Append(",SEITOMAST.KEIYAKU_DENWA_O")    '電話番号
        '    SQL2.Append(",SEITOMAST.SINKYU_KBN_O")       '進級区分※
        '    SQL2.Append(",SEITOMAST.KEIYAKU_NJYU_O")     '住所
        '    SQL2.Append(",SEITOMAST.HIMOKU_ID_O")        '費目ID
        '    For i = 1 To 15
        '        SQL2.Append(",HIMOMAST.HIMOKU_NAME" & i.ToString("00") & "_H")  '費目名
        '    Next
        '    For i = 1 To 15
        '        SQL2.Append(",HIMOMAST.HIMOKU_KINGAKU" & i.ToString("00") & "_H")   '金額
        '    Next

        '    SQL2.Append(",HIMOMAST.TUKI_NO_H")

        '    SQL2.Append(" FROM")
        '    SQL2.Append(" SEITOMAST")
        '    SQL2.Append(" INNER JOIN GAKMAST1")
        '    SQL2.Append(" ON SEITOMAST.GAKKOU_CODE_O = GAKMAST1.GAKKOU_CODE_G")
        '    SQL2.Append(" AND SEITOMAST.GAKUNEN_CODE_O = GAKMAST1.GAKUNEN_CODE_G")

        '    SQL2.Append(" INNER JOIN GAKMAST2")
        '    SQL2.Append(" ON GAKMAST1.GAKKOU_CODE_G = GAKMAST2.GAKKOU_CODE_T")
        '    SQL2.Append(" INNER JOIN TENMAST TENMAST_G")
        '    SQL2.Append(" ON GAKMAST2.TKIN_NO_T = TENMAST_G.KIN_NO_N")
        '    SQL2.Append(" AND GAKMAST2.TSIT_NO_T = TENMAST_G.SIT_NO_N")

        '    SQL2.Append(" INNER JOIN TENMAST TENMAST_S")
        '    SQL2.Append(" ON SEITOMAST.TKIN_NO_O = TENMAST_S.KIN_NO_N")
        '    SQL2.Append(" AND SEITOMAST.TSIT_NO_O = TENMAST_S.SIT_NO_N")

        '    SQL2.Append(" INNER JOIN HIMOMAST")
        '    SQL2.Append(" ON SEITOMAST.GAKKOU_CODE_O = HIMOMAST.GAKKOU_CODE_H")
        '    SQL2.Append(" AND SEITOMAST.GAKUNEN_CODE_O = HIMOMAST.GAKUNEN_CODE_H")
        '    SQL2.Append(" AND SEITOMAST.HIMOKU_ID_O = HIMOMAST.HIMOKU_ID_H")
        '    '2011/06/16 標準版修正 処理速度向上 ------------------START
        '    SQL2.Append(" AND SEITOMAST.TUKI_NO_O = HIMOMAST.TUKI_NO_H")
        '    '2011/06/16 標準版修正 処理速度向上 ------------------END

        '    SQL2.Append(" WHERE SEITOMAST.GAKKOU_CODE_O = " & SQ(txtGAKKOU_CODE.Text))
        '    SQL2.Append(" AND SEITOMAST.NENDO_O = " & SQ(txtNENDO.Text))
        '    SQL2.Append(" AND SEITOMAST.TUUBAN_O = " & SQ(txtTUUBAN.Text))

        '    SQL2.Append(" ORDER BY HIMOMAST.TUKI_NO_H ASC")


        '    'SQL実行
        '    i = 0
        '    If GCom.SetDynaset(SQL2.ToString, REC2) AndAlso REC2.Read Then
        '        Do
        '            StData(i, 0) = GCom.NzStr(REC2.Item("G_SIT_NNAME"))             'とりまとめ店名
        '            StData(i, 1) = GCom.NzStr(REC2.Item("GAKKOU_CODE_O"))           '学校コード
        '            StData(i, 2) = GCom.NzStr(REC2.Item("GAKKOU_NNAME_G"))          '学校名
        '            StData(i, 3) = GCom.NzStr(REC2.Item("NENDO_O"))                 '入学年度
        '            StData(i, 4) = GCom.NzStr(REC2.Item("GAKUNEN_CODE_O"))          '学年
        '            StData(i, 5) = GCom.NzStr(REC2.Item("TUUBAN_O"))                '通番
        '            StData(i, 6) = GCom.NzStr(REC2.Item("SEITO_NO_O"))              '生徒番号
        '            StData(i, 7) = GCom.NzStr(REC2.Item("SEITO_KNAME_O"))           '生徒氏名カナ
        '            StData(i, 8) = GCom.NzStr(REC2.Item("SEITO_NNAME_O")).Trim      '生徒氏名漢字
        '            StData(i, 9) = GCom.NzStr(REC2.Item("MEIGI_KNAME_O"))           '口座名義カナ
        '            StData(i, 10) = GCom.NzStr(REC2.Item("MEIGI_NNAME_O")).Trim     '口座名義漢字
        '            StData(i, 11) = GCom.NzStr(REC2.Item("TKIN_NO_O"))              '取扱金融機関コード
        '            StData(i, 12) = GCom.NzStr(REC2.Item("KIN_NNAME_N"))            '取扱金融機関名
        '            StData(i, 13) = GCom.NzStr(REC2.Item("TSIT_NO_O"))              '取扱支店コード
        '            StData(i, 14) = GCom.NzStr(REC2.Item("S_SIT_NNAME"))            '取扱支店名
        '            StData(i, 15) = GCom.NzStr(REC2.Item("KAMOKU_O"))               '科目
        '            StData(i, 16) = GCom.NzStr(REC2.Item("KOUZA_O"))                '口座番号
        '            StData(i, 17) = GCom.NzStr(REC2.Item("CLASS_CODE_O"))           'クラス
        '            StData(i, 18) = GCom.NzStr(REC2.Item("SEIBETU_O"))              '性別
        '            StData(i, 19) = GCom.NzStr(REC2.Item("FURIKAE_O"))              '振替区分
        '            StData(i, 20) = GCom.NzStr(REC2.Item("KAIYAKU_FLG_O"))          '解約区分
        '            StData(i, 21) = GCom.NzStr(REC2.Item("KEIYAKU_DENWA_O")).Trim   '電話番号
        '            StData(i, 22) = GCom.NzStr(REC2.Item("SINKYU_KBN_O"))           '進級区分
        '            StData(i, 23) = GCom.NzStr(REC2.Item("KEIYAKU_NJYU_O")).Trim    '住所
        '            StData(i, 24) = GCom.NzStr(REC2.Item("HIMOKU_ID_O"))            '費目ID
        '            For j As Integer = 25 To 39
        '                StData(i, j) = GCom.NzStr(REC2.Item("HIMOKU_NAME" & (j - 24).ToString("00") & "_H")).Trim
        '            Next
        '            For j As Integer = 40 To 54
        '                StData(i, j) = GCom.NzStr(REC2.Item("HIMOKU_KINGAKU" & (j - 39).ToString("00") & "_H"))
        '            Next
        '            StData(i, 55) = GCom.NzStr(REC2.Item("TUKI_NO_H"))

        '            i = i + 1
        '        Loop Until REC2.Read = False
        '    End If

        '    'CSV作成
        '    strCSV_FILE_NAME = CreateCSV.CreateCsvFile()

        '    '2010/10/18 処理改善 テキストを読込んでリスト化 ここから
        '    Dim KAMOKU_LIST As New MyStringDictionary(STR_TXT_PATH & STR_KAMOKU_TXT)
        '    Dim SEIBETU_LIST As New MyStringDictionary(STR_TXT_PATH & STR_SEIBETU_TXT)
        '    Dim FURIKAE_LIST As New MyStringDictionary(STR_TXT_PATH & STR_FURIKAE_TXT)
        '    Dim KAIYAKU_LIST As New MyStringDictionary(STR_TXT_PATH & STR_KAIYAKU_TXT)
        '    Dim SINKYU_LIST As New MyStringDictionary(STR_TXT_PATH & STR_SINKYU_TXT)
        '    '2010/10/18 処理改善 テキストを読込んでリスト化 ここまで

        '    ' CSV作成方法
        '    ' 上記SQLだと12レコード（月単位）でしか項目を取得できないため、CSV用にデータを加工する
        '    ' （DBからの取得が12レコードに対し、帳票のCSVは15項目のためデータを加工する必要がある）
        '    ' 生徒マスタの請求方法が"個別"の場合、金額は生徒マスタに格納されるため、生徒マスタの金額を参照

        '    '費目数分ループする
        '    For i = 0 To 14
        '        With CreateCSV
        '            .OutputCsvData(PntSyoriKbn, True)                           'タイトル（登録or削除）
        '            .OutputCsvData(Today, True)                                 'システム日付
        '            .OutputCsvData(NowTime, True)                               'タイムスタンプ
        '            .OutputCsvData(StData(0, 0), True)                          'とりまとめ店名
        '            .OutputCsvData(StData(0, 1), True)                          '学校コード
        '            .OutputCsvData(StData(0, 2), True)                          '学校名
        '            .OutputCsvData(StData(0, 3), True)                          '入学年度
        '            .OutputCsvData(StData(0, 4), True)                          '学年
        '            .OutputCsvData(StData(0, 5), True)                          '通番
        '            .OutputCsvData(StData(0, 6), True)                          '生徒番号
        '            .OutputCsvData(StData(0, 7), True)                          '生徒氏名カナ
        '            .OutputCsvData(StData(0, 8), True)                          '生徒氏名漢字
        '            .OutputCsvData(StData(0, 9), True)                          '口座名義カナ
        '            .OutputCsvData(StData(0, 10), True)                         '口座名義漢字
        '            .OutputCsvData(StData(0, 11), True)                         '取扱金融機関コード
        '            .OutputCsvData(StData(0, 12), True)                         '取扱金融機関名
        '            .OutputCsvData(StData(0, 13), True)                         '取扱支店コード
        '            .OutputCsvData(StData(0, 14), True)                         '取扱支店名
        '            '2010/10/18 リストから値を取得する
        '            '.OutputCsvData(GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_KAMOKU_TXT, _
        '            '                                 StData(0, 15)), True)      '科目
        '            .OutputCsvData(KAMOKU_LIST.Item(StData(0, 15)), True)       '科目
        '            .OutputCsvData(StData(0, 16), True)                         '口座番号
        '            .OutputCsvData(StData(0, 17), True)                         'クラス
        '            '2010/10/18 リストから値を取得する
        '            '.OutputCsvData(GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_SEIBETU_TXT, _
        '            '                                  StData(0, 18)), True)     '性別
        '            '.OutputCsvData(GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_FURIKAE_TXT, _
        '            '                                  StData(0, 19)), True)     '振替方法
        '            '.OutputCsvData(GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_KAIYAKU_TXT, _
        '            '                                  StData(0, 20)), True)     '解約区分
        '            .OutputCsvData(SEIBETU_LIST.Item(StData(0, 18)), True)      '性別
        '            .OutputCsvData(FURIKAE_LIST.Item(StData(0, 19)), True)      '振替方法
        '            .OutputCsvData(KAIYAKU_LIST(StData(0, 20)), True)           '解約区分
        '            .OutputCsvData(StData(0, 21), True)                         '電話番号
        '            '2010/10/18 リストから値を取得する
        '            '.OutputCsvData(GFUNC_CODE_TO_NAME(STR_TXT_PATH & STR_SINKYU_TXT, _
        '            '                                  StData(0, 22)), True)     '進級区分
        '            .OutputCsvData(SINKYU_LIST(StData(0, 22)), True)            '進級区分
        '            .OutputCsvData(StData(0, 23), True)                         '住所
        '            .OutputCsvData(StData(0, 24), True)                         '費目ID
        '            .OutputCsvData(StData(0, i + 25), True)                     '費目名
        '            For j As Integer = 0 To 11
        '                ' 金額クリア
        '                ' 費目名が入力されていないレコードの場合、金額を0→空白に変更
        '                If Trim(StData(0, i + 25)) <> "" Then
        '                    '比較処理
        '                    '生徒マスタの請求方法が"個別"の場合、生徒マスタの金額を出力
        '                    If SeitoSEIKYU(j, i) = "1" Then
        '                        .OutputCsvData(SeitoKINGAKU(j, i), True)
        '                    Else
        '                        .OutputCsvData(StData(j, i + 15 + 25), True)
        '                    End If

        '                Else
        '                    .OutputCsvData("", True)
        '                End If
        '            Next
        '            '表示区分
        '            If Trim(StData(0, i + 25)) <> "" Then
        '                .OutputCsvData("1", True)
        '            Else
        '                .OutputCsvData("0", True)
        '            End If

        '            .OutputCsvData("", False, True)                             '改行
        '        End With
        '    Next

        '    CreateCSV.CloseCsv()
        '    CreateCSV = Nothing

        '    Return True

        'Catch ex As Exception
        '    Select Case PntSyoriKbn
        '        Case "0"
        '            MessageBox.Show(String.Format(MSG0231W, "生徒マスタメンテ(登録)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", ex.Message)
        '        Case "2"
        '            MessageBox.Show(String.Format(MSG0231W, "生徒マスタメンテ(削除)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", ex.Message)
        '        Case Else

        '    End Select
        '    Return False
        'Finally
        '    If Not REC Is Nothing Then
        '        REC.Close()
        '        REC.Dispose()
        '    End If
        '    If Not REC2 Is Nothing Then
        '        REC2.Close()
        '        REC2.Dispose()
        '    End If
        '    Me.ResumeLayout()
        'End Try
        '2017/04/05 saitou 東春信金(RSV2標準) UPD ----------------------------------------------------------------- END
    End Function

    '============================================================================
    'NAME           :fn_CreateCSV_UPD
    'Parameter      :PntSyoriKbn 印刷処理区分
    'Description    :KFGP107(生徒マスタメンテ(更新))印刷用ＣＳＶファイル作成
    'Return         :
    'Create         :2010/09/16
    'Update         :2017/04/05 saitou 東春信金(RSV2標準) deleted for 生徒マスタメンテ帳票改善
    '============================================================================
    'Private Function fn_CreateCSV_UPD(ByVal beforeData(,) As String _
    '                                  , ByVal afterData(,) As String) As Boolean
    '    Try
    '        Dim CreateCSV As New KFGP107
    '        Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
    '        Dim NowTime As String = Format(DateTime.Now, "HHmmss")

    '        If beforeData.GetLength(0) <> afterData.GetLength(0) Then
    '            Return False
    '        End If
    '        If beforeData.GetLength(1) <> afterData.GetLength(1) Then
    '            Return False
    '        End If

    '        Dim bUpdFlg As Boolean = False      '更新フラグ

    '        'リストヘッダ部取得
    '        Dim strGakCode As String = afterData(0, 0)
    '        Dim strNendo As String = afterData(0, 1)
    '        Dim strTuuban As String = afterData(0, 2)
    '        Dim strGakunen As String = afterData(0, 3)
    '        Dim strClass As String = afterData(0, 4)
    '        Dim strSeitoNo As String = afterData(0, 5)
    '        Dim strGakName As String = lab学校名.Text

    '        'オラクル項目名取得
    '        Dim oraColumnJapanName() As String = Nothing
    '        Dim oraColumnName() As String = Nothing
    '        fn_GetSEITOMAST_ColumnName(oraColumnName, oraColumnJapanName)

    '        'CSV作成
    '        strCSV_FILE_NAME = CreateCSV.CreateCsvFile()

    '        ' 更新されているデータを検索
    '        ' 検索方法
    '        ' ①更新前と更新後のデータを比べる
    '        ' ②TUKI_NO_Oの影響で、1箇所更新で12箇所更新されるので、費目関連以外は1レコード目を比べる
    '        ' ③上記方法で相違があった場合、CSVデータ作成
    '        ' ※更新日は出力しない
    '        ' ※更新項目が存在しない場合、帳票を印刷しない

    '        ' ※※※ 相違点があった場合のＣＳＶ作成イメージ ※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※
    '        ' 学校コード　入学年度　…　費目1の請求方法　費目1の請求金額　…　費目15の請求方法　費目15の請求金額　…　更新日　…　予備5
    '        '     ○         ○     …        ○               ○         …         ○                ○               ×    …    ○          (配列1レコード)
    '        '     ×         ×     …        ○               ○         …         ○                ○               ×    …    ×          (配列2レコード)
    '        '     ・         ・     　        ・               ・         　         ・                ・               ・    　    ・
    '        '     ・         ・     　        ・               ・         　         ・                ・               ・    　    ・
    '        '     ×         ×     …        ○               ○         …         ○                ○               ×    …    ×          (配列最終レコード)
    '        ' ※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※

    '        For i As Integer = 0 To beforeData.GetLength(0) - 1
    '            For j As Integer = 0 To beforeData.GetLength(1) - 1
    '                If beforeData(i, j) <> afterData(i, j) Then
    '                    If i = 0 Then
    '                        '1レコード目の相違は無条件で書き込み（更新日を除く）
    '                        If j = 59 Then
    '                        Else
    '                            'CSVデータ作成
    '                            With CreateCSV
    '                                .OutputCsvData(Today, True)                         '処理日
    '                                .OutputCsvData(NowTime, True)                       'タイムスタンプ
    '                                .OutputCsvData(GCom.GetUserID, True)                'ログイン名
    '                                .OutputCsvData(Environment.MachineName, True)       '端末名
    '                                .OutputCsvData(strGakCode, True)                    '学校コード
    '                                .OutputCsvData(strNendo, True)                      '年度
    '                                .OutputCsvData(strTuuban, True)                     '通番
    '                                .OutputCsvData(strGakunen, True)                    '学年
    '                                .OutputCsvData(strClass, True)                      'クラス
    '                                .OutputCsvData(strSeitoNo, True)                    '生徒番号
    '                                .OutputCsvData(strGakName, True)                    '学校名
    '                                .OutputCsvData(oraColumnJapanName(j), True)         '日本語表示名
    '                                .OutputCsvData(oraColumnName(j), True)              'ORACLE項目名
    '                                .OutputCsvData(beforeData(i, j), True)              '変更前
    '                                .OutputCsvData(afterData(i, j), True, True)         '変更後
    '                            End With
    '                        End If

    '                        '更新フラグ更新
    '                        bUpdFlg = True

    '                    Else
    '                        '1レコード目でない場合
    '                        Select Case j
    '                            Case 28 To 57
    '                                '費目関連の情報を書き込み
    '                                'CSVデータ作成
    '                                With CreateCSV
    '                                    .OutputCsvData(Today, True)                         '処理日
    '                                    .OutputCsvData(NowTime, True)                       'タイムスタンプ
    '                                    .OutputCsvData(GCom.GetUserID, True)                'ログイン名
    '                                    .OutputCsvData(Environment.MachineName, True)       '端末名
    '                                    .OutputCsvData(strGakCode, True)                    '学校コード
    '                                    .OutputCsvData(strNendo, True)                      '年度
    '                                    .OutputCsvData(strTuuban, True)                     '通番
    '                                    .OutputCsvData(strGakunen, True)                    '学年
    '                                    .OutputCsvData(strClass, True)                      'クラス
    '                                    .OutputCsvData(strSeitoNo, True)                    '生徒番号
    '                                    .OutputCsvData(strGakName, True)                    '学校名
    '                                    .OutputCsvData(oraColumnJapanName(j), True)         '日本語表示名
    '                                    .OutputCsvData(oraColumnName(j), True)              'ORACLE項目名
    '                                    .OutputCsvData(beforeData(i, j), True)              '変更前
    '                                    .OutputCsvData(afterData(i, j), True, True)         '変更後
    '                                End With
    '                            Case Else
    '                                '費目関連以外は読み飛ばす
    '                        End Select
    '                    End If
    '                End If
    '            Next
    '        Next

    '        CreateCSV.CloseCsv()
    '        CreateCSV = Nothing

    '        Return bUpdFlg

    '    Catch ex As Exception
    '        MessageBox.Show(String.Format(MSG0231W, "生徒マスタメンテ(更新)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", ex.Message)
    '        Return False
    '    Finally

    '    End Try
    'End Function

    '============================================================================
    'NAME           :fn_GetSEITOMAST_PrnCSVUpd
    'Parameter      :seitoData(,) 生徒マスタのレコード
    'Description    :生徒マスタのレコードを取得する（※生徒マスタメンテ（更新）用）
    'Return         :
    'Create         :2010/09/16
    'Update         :2017/04/05 saitou 東春信金(RSV2標準) deleted for 生徒マスタメンテ帳票改善
    '============================================================================
    'Private Function fn_GetSEITOMAST_PrnCSVUpd(ByRef seitoData(,) As String) As Boolean
    '    Dim REC As OracleDataReader = Nothing
    '    Try
    '        Dim SQL As New StringBuilder(128)
    '        Dim RecordIndex As Integer = 0
    '        Dim RecordCount As Integer = 0
    '        SQL.Append("SELECT * ")
    '        SQL.Append(" FROM SEITOMAST")
    '        SQL.Append(" WHERE SEITOMAST.GAKKOU_CODE_O = " & SQ(txtGAKKOU_CODE.Text))
    '        SQL.Append(" AND SEITOMAST.NENDO_O = " & SQ(txtNENDO.Text))
    '        SQL.Append(" AND SEITOMAST.TUUBAN_O = " & SQ(txtTUUBAN.Text))
    '        SQL.Append(" ORDER BY SEITOMAST.TUKI_NO_O ASC")

    '        ' 項目数取得の検索（カウントのみ）
    '        If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then
    '            Do
    '                RecordCount = RecordCount + 1
    '            Loop Until REC.Read = False
    '        End If

    '        ' 配列の要素数を取得
    '        ReDim seitoData(RecordCount, SeitoMastMaxColumns)

    '        ' 項目取得のためにもう一度検索
    '        If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then
    '            Do
    '                seitoData(RecordIndex, 0) = GCom.NzStr(REC.Item("GAKKOU_CODE_O"))
    '                seitoData(RecordIndex, 1) = GCom.NzStr(REC.Item("NENDO_O"))
    '                seitoData(RecordIndex, 2) = GCom.NzStr(REC.Item("TUUBAN_O"))
    '                seitoData(RecordIndex, 3) = GCom.NzStr(REC.Item("GAKUNEN_CODE_O"))
    '                seitoData(RecordIndex, 4) = GCom.NzStr(REC.Item("CLASS_CODE_O"))
    '                seitoData(RecordIndex, 5) = GCom.NzStr(REC.Item("SEITO_NO_O"))
    '                seitoData(RecordIndex, 6) = GCom.NzStr(REC.Item("SEITO_KNAME_O"))
    '                seitoData(RecordIndex, 7) = GCom.NzStr(REC.Item("SEITO_NNAME_O"))
    '                seitoData(RecordIndex, 8) = GCom.NzStr(REC.Item("SEIBETU_O"))
    '                seitoData(RecordIndex, 9) = GCom.NzStr(REC.Item("TKIN_NO_O"))
    '                seitoData(RecordIndex, 10) = GCom.NzStr(REC.Item("TSIT_NO_O"))
    '                seitoData(RecordIndex, 11) = GCom.NzStr(REC.Item("KAMOKU_O"))
    '                seitoData(RecordIndex, 12) = GCom.NzStr(REC.Item("KOUZA_O"))
    '                seitoData(RecordIndex, 13) = GCom.NzStr(REC.Item("MEIGI_KNAME_O"))
    '                seitoData(RecordIndex, 14) = GCom.NzStr(REC.Item("MEIGI_NNAME_O"))
    '                seitoData(RecordIndex, 15) = GCom.NzStr(REC.Item("FURIKAE_O"))
    '                seitoData(RecordIndex, 16) = GCom.NzStr(REC.Item("KEIYAKU_NJYU_O"))
    '                seitoData(RecordIndex, 17) = GCom.NzStr(REC.Item("KEIYAKU_DENWA_O"))
    '                seitoData(RecordIndex, 18) = GCom.NzStr(REC.Item("KAIYAKU_FLG_O"))
    '                seitoData(RecordIndex, 19) = GCom.NzStr(REC.Item("SINKYU_KBN_O"))
    '                seitoData(RecordIndex, 20) = GCom.NzStr(REC.Item("HIMOKU_ID_O"))
    '                seitoData(RecordIndex, 21) = GCom.NzStr(REC.Item("TYOUSI_FLG_O"))
    '                seitoData(RecordIndex, 22) = GCom.NzStr(REC.Item("TYOUSI_NENDO_O"))
    '                seitoData(RecordIndex, 23) = GCom.NzStr(REC.Item("TYOUSI_TUUBAN_O"))
    '                seitoData(RecordIndex, 24) = GCom.NzStr(REC.Item("TYOUSI_GAKUNEN_O"))
    '                seitoData(RecordIndex, 25) = GCom.NzStr(REC.Item("TYOUSI_CLASS_O"))
    '                seitoData(RecordIndex, 26) = GCom.NzStr(REC.Item("TYOUSI_SEITONO_O"))
    '                seitoData(RecordIndex, 27) = GCom.NzStr(REC.Item("TUKI_NO_O"))
    '                seitoData(RecordIndex, 28) = GCom.NzStr(REC.Item("SEIKYU01_O"))
    '                seitoData(RecordIndex, 29) = GCom.NzStr(REC.Item("KINGAKU01_O"))
    '                seitoData(RecordIndex, 30) = GCom.NzStr(REC.Item("SEIKYU02_O"))
    '                seitoData(RecordIndex, 31) = GCom.NzStr(REC.Item("KINGAKU02_O"))
    '                seitoData(RecordIndex, 32) = GCom.NzStr(REC.Item("SEIKYU03_O"))
    '                seitoData(RecordIndex, 33) = GCom.NzStr(REC.Item("KINGAKU03_O"))
    '                seitoData(RecordIndex, 34) = GCom.NzStr(REC.Item("SEIKYU04_O"))
    '                seitoData(RecordIndex, 35) = GCom.NzStr(REC.Item("KINGAKU04_O"))
    '                seitoData(RecordIndex, 36) = GCom.NzStr(REC.Item("SEIKYU05_O"))
    '                seitoData(RecordIndex, 37) = GCom.NzStr(REC.Item("KINGAKU05_O"))
    '                seitoData(RecordIndex, 38) = GCom.NzStr(REC.Item("SEIKYU06_O"))
    '                seitoData(RecordIndex, 39) = GCom.NzStr(REC.Item("KINGAKU06_O"))
    '                seitoData(RecordIndex, 40) = GCom.NzStr(REC.Item("SEIKYU07_O"))
    '                seitoData(RecordIndex, 41) = GCom.NzStr(REC.Item("KINGAKU07_O"))
    '                seitoData(RecordIndex, 42) = GCom.NzStr(REC.Item("SEIKYU08_O"))
    '                seitoData(RecordIndex, 43) = GCom.NzStr(REC.Item("KINGAKU08_O"))
    '                seitoData(RecordIndex, 44) = GCom.NzStr(REC.Item("SEIKYU09_O"))
    '                seitoData(RecordIndex, 45) = GCom.NzStr(REC.Item("KINGAKU09_O"))
    '                seitoData(RecordIndex, 46) = GCom.NzStr(REC.Item("SEIKYU10_O"))
    '                seitoData(RecordIndex, 47) = GCom.NzStr(REC.Item("KINGAKU10_O"))
    '                seitoData(RecordIndex, 48) = GCom.NzStr(REC.Item("SEIKYU11_O"))
    '                seitoData(RecordIndex, 49) = GCom.NzStr(REC.Item("KINGAKU11_O"))
    '                seitoData(RecordIndex, 50) = GCom.NzStr(REC.Item("SEIKYU12_O"))
    '                seitoData(RecordIndex, 51) = GCom.NzStr(REC.Item("KINGAKU12_O"))
    '                seitoData(RecordIndex, 52) = GCom.NzStr(REC.Item("SEIKYU13_O"))
    '                seitoData(RecordIndex, 53) = GCom.NzStr(REC.Item("KINGAKU13_O"))
    '                seitoData(RecordIndex, 54) = GCom.NzStr(REC.Item("SEIKYU14_O"))
    '                seitoData(RecordIndex, 55) = GCom.NzStr(REC.Item("KINGAKU14_O"))
    '                seitoData(RecordIndex, 56) = GCom.NzStr(REC.Item("SEIKYU15_O"))
    '                seitoData(RecordIndex, 57) = GCom.NzStr(REC.Item("KINGAKU15_O"))
    '                seitoData(RecordIndex, 58) = GCom.NzStr(REC.Item("SAKUSEI_DATE_O"))
    '                seitoData(RecordIndex, 59) = GCom.NzStr(REC.Item("KOUSIN_DATE_O"))
    '                seitoData(RecordIndex, 60) = GCom.NzStr(REC.Item("YOBI1_O"))
    '                seitoData(RecordIndex, 61) = GCom.NzStr(REC.Item("YOBI2_O"))
    '                seitoData(RecordIndex, 62) = GCom.NzStr(REC.Item("YOBI3_O"))
    '                seitoData(RecordIndex, 63) = GCom.NzStr(REC.Item("YOBI4_O"))
    '                seitoData(RecordIndex, 64) = GCom.NzStr(REC.Item("YOBI5_O"))

    '                RecordIndex = RecordIndex + 1

    '            Loop Until REC.Read = False
    '        End If

    '        Return True
    '    Catch ex As Exception
    '        Return False
    '    Finally
    '        If Not REC Is Nothing Then
    '            REC.Close()
    '            REC.Dispose()
    '        End If
    '        Me.ResumeLayout()
    '    End Try
    'End Function

    '============================================================================
    'NAME           :fn_GetSEITOMAST_ColumnName
    'Parameter      :ColumnNameE() 生徒マスタの項目名（英名）
    '               :ColumnNameJ() 生徒マスタの項目名（日本名）
    'Description    :生徒マスタの列名を設定
    'Return         :
    'Create         :2010/09/16
    'Update         :2017/04/05 saitou 東春信金(RSV2標準) deleted for 生徒マスタメンテ帳票改善
    '============================================================================
    'Private Sub fn_GetSEITOMAST_ColumnName(ByRef ColumnNameE() As String, ByRef ColumnNameJ() As String)
    '    ReDim ColumnNameE(SeitoMastMaxColumns)
    '    ReDim ColumnNameJ(SeitoMastMaxColumns)

    '    ColumnNameE(0) = "GAKKOU_CODE_O"
    '    ColumnNameJ(0) = "学校コード"
    '    ColumnNameE(1) = "NENDO_O"
    '    ColumnNameJ(1) = "入学年度"
    '    ColumnNameE(2) = "TUUBAN_O"
    '    ColumnNameJ(2) = "通番"
    '    ColumnNameE(3) = "GAKUNEN_CODE_O"
    '    ColumnNameJ(3) = "学年コード"
    '    ColumnNameE(4) = "CLASS_CODE_O"
    '    ColumnNameJ(4) = "クラスコード"
    '    ColumnNameE(5) = "SEITO_NO_O"
    '    ColumnNameJ(5) = "生徒番号"
    '    ColumnNameE(6) = "SEITO_KNAME_O"
    '    ColumnNameJ(6) = "生徒氏名（カナ）"
    '    ColumnNameE(7) = "SEITO_NNAME_O"
    '    ColumnNameJ(7) = "生徒氏名（漢字）"
    '    ColumnNameE(8) = "SEIBETU_O"
    '    ColumnNameJ(8) = "性別"
    '    ColumnNameE(9) = "TKIN_NO_O"
    '    ColumnNameJ(9) = "金融機関コード"
    '    ColumnNameE(10) = "TSIT_NO_O"
    '    ColumnNameJ(10) = "支店コード"
    '    ColumnNameE(11) = "KAMOKU_O"
    '    ColumnNameJ(11) = "科目"
    '    ColumnNameE(12) = "KOUZA_O"
    '    ColumnNameJ(12) = "口座番号"
    '    ColumnNameE(13) = "MEIGI_KNAME_O"
    '    ColumnNameJ(13) = "名義人（カナ）"
    '    ColumnNameE(14) = "MEIGI_NNAME_O"
    '    ColumnNameJ(14) = "名義人（漢字）"
    '    ColumnNameE(15) = "FURIKAE_O"
    '    ColumnNameJ(15) = "振替方法"
    '    ColumnNameE(16) = "KEIYAKU_NJYU_O"
    '    ColumnNameJ(16) = "契約住所（漢字）"
    '    ColumnNameE(17) = "KEIYAKU_DENWA_O"
    '    ColumnNameJ(17) = "契約電話番号"
    '    ColumnNameE(18) = "KAIYAKU_FLG_O"
    '    ColumnNameJ(18) = "解約区分"
    '    ColumnNameE(19) = "SINKYU_KBN_O"
    '    ColumnNameJ(19) = "進級区分"
    '    ColumnNameE(20) = "HIMOKU_ID_O"
    '    ColumnNameJ(20) = "費目ID"
    '    ColumnNameE(21) = "TYOUSI_FLG_O"
    '    ColumnNameJ(21) = "長子有無フラグ"
    '    ColumnNameE(22) = "TYOUSI_NENDO_O"
    '    ColumnNameJ(22) = "長子入学年度"
    '    ColumnNameE(23) = "TYOUSI_TUUBAN_O"
    '    ColumnNameJ(23) = "長子通番"
    '    ColumnNameE(24) = "TYOUSI_GAKUNEN_O"
    '    ColumnNameJ(24) = "長子学年"
    '    ColumnNameE(25) = "TYOUSI_CLASS_O"
    '    ColumnNameJ(25) = "長子クラス"
    '    ColumnNameE(26) = "TYOUSI_SEITONO_O"
    '    ColumnNameJ(26) = "長子生徒番号"
    '    ColumnNameE(27) = "TUKI_NO_O"
    '    ColumnNameJ(27) = "請求月"
    '    ColumnNameE(28) = "SEIKYU01_O"
    '    ColumnNameJ(28) = "費目1の請求方法"
    '    ColumnNameE(29) = "KINGAKU01_O"
    '    ColumnNameJ(29) = "費目1の請求金額"
    '    ColumnNameE(30) = "SEIKYU02_O"
    '    ColumnNameJ(30) = "費目2の請求方法"
    '    ColumnNameE(31) = "KINGAKU02_O"
    '    ColumnNameJ(31) = "費目2の請求金額"
    '    ColumnNameE(32) = "SEIKYU03_O"
    '    ColumnNameJ(32) = "費目3の請求方法"
    '    ColumnNameE(33) = "KINGAKU03_O"
    '    ColumnNameJ(33) = "費目3の請求金額"
    '    ColumnNameE(34) = "SEIKYU04_O"
    '    ColumnNameJ(34) = "費目4の請求方法"
    '    ColumnNameE(35) = "KINGAKU04_O"
    '    ColumnNameJ(35) = "費目4の請求金額"
    '    ColumnNameE(36) = "SEIKYU05_O"
    '    ColumnNameJ(36) = "費目5の請求方法"
    '    ColumnNameE(37) = "KINGAKU05_O"
    '    ColumnNameJ(37) = "費目5の請求金額"
    '    ColumnNameE(38) = "SEIKYU06_O"
    '    ColumnNameJ(38) = "費目6の請求方法"
    '    ColumnNameE(39) = "KINGAKU06_O"
    '    ColumnNameJ(39) = "費目6の請求金額"
    '    ColumnNameE(40) = "SEIKYU07_O"
    '    ColumnNameJ(40) = "費目7の請求方法"
    '    ColumnNameE(41) = "KINGAKU07_O"
    '    ColumnNameJ(41) = "費目7の請求金額"
    '    ColumnNameE(42) = "SEIKYU08_O"
    '    ColumnNameJ(42) = "費目8の請求方法"
    '    ColumnNameE(43) = "KINGAKU08_O"
    '    ColumnNameJ(43) = "費目8の請求金額"
    '    ColumnNameE(44) = "SEIKYU09_O"
    '    ColumnNameJ(44) = "費目9の請求方法"
    '    ColumnNameE(45) = "KINGAKU09_O"
    '    ColumnNameJ(45) = "費目9の請求金額"
    '    ColumnNameE(46) = "SEIKYU10_O"
    '    ColumnNameJ(46) = "費目10の請求方法"
    '    ColumnNameE(47) = "KINGAKU10_O"
    '    ColumnNameJ(47) = "費目10の請求金額"
    '    ColumnNameE(48) = "SEIKYU11_O"
    '    ColumnNameJ(48) = "費目11の請求方法"
    '    ColumnNameE(49) = "KINGAKU11_O"
    '    ColumnNameJ(49) = "費目11の請求金額"
    '    ColumnNameE(50) = "SEIKYU12_O"
    '    ColumnNameJ(50) = "費目12の請求方法"
    '    ColumnNameE(51) = "KINGAKU12_O"
    '    ColumnNameJ(51) = "費目12の請求金額"
    '    ColumnNameE(52) = "SEIKYU13_O"
    '    ColumnNameJ(52) = "費目13の請求方法"
    '    ColumnNameE(53) = "KINGAKU13_O"
    '    ColumnNameJ(53) = "費目13の請求金額"
    '    ColumnNameE(54) = "SEIKYU14_O"
    '    ColumnNameJ(54) = "費目14の請求方法"
    '    ColumnNameE(55) = "KINGAKU14_O"
    '    ColumnNameJ(55) = "費目14の請求金額"
    '    ColumnNameE(56) = "SEIKYU15_O"
    '    ColumnNameJ(56) = "費目15の請求方法"
    '    ColumnNameE(57) = "KINGAKU15_O"
    '    ColumnNameJ(57) = "費目15の請求金額"
    '    ColumnNameE(58) = "SAKUSEI_DATE_O"
    '    ColumnNameJ(58) = "作成日付"
    '    ColumnNameE(59) = "KOUSIN_DATE_O"
    '    ColumnNameJ(59) = "更新日付"
    '    ColumnNameE(60) = "YOBI1_O"
    '    ColumnNameJ(60) = "予備1"
    '    ColumnNameE(61) = "YOBI2_O"
    '    ColumnNameJ(61) = "予備2"
    '    ColumnNameE(62) = "YOBI3_O"
    '    ColumnNameJ(62) = "予備3"
    '    ColumnNameE(63) = "YOBI4_O"
    '    ColumnNameJ(63) = "予備4"
    '    ColumnNameE(64) = "YOBI5_O"
    '    ColumnNameJ(64) = "予備5"
    'End Sub



    '============================================================================
    'NAME           :fn_Print
    'Parameter      :PrintNo 0:登録 1:更新 2:削除
    'Description    :帳票印刷用ＣＳＶファイル作成
    'Return         :
    'Create         :2010/03/16
    'Update         :
    '============================================================================
    Public Function fn_Print(ByVal PrintNo As Integer) As Boolean
        Dim ErrMessage As String = ""
        Dim Param As String = ""
        Dim nRet As Integer = 0
        Dim Syori As String = ""

        Select Case PrintNo
            Case 0
                Syori = "登録"
            Case 1
                Syori = "更新"
            Case 2
                Syori = "削除"
        End Select
        Try
            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：ログイン名、ＣＳＶファイル名、取引先コード
            Param = GCom.GetUserID & "," & strCSV_FILE_NAME & "," & _
                    LW.ToriCode
            Select Case PrintNo
                Case 0
                    nRet = ExeRepo.ExecReport("KFGP106.EXE", Param)
                Case 1
                    nRet = ExeRepo.ExecReport("KFGP107.EXE", Param)
                Case 2
                    nRet = ExeRepo.ExecReport("KFGP108.EXE", Param)
            End Select

            If nRet <> 0 Then
                '印刷失敗：戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case -1
                        ErrMessage = String.Format(MSG0106W, "生徒マスタメンテ(" & Syori & ")")
                    Case Else
                        ErrMessage = String.Format(MSG0004E, "生徒マスタメンテ(" & Syori & ")")
                End Select

                MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
            MessageBox.Show(String.Format(MSG0014I, "生徒マスタメンテ(" & Syori & ")"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return True

        Catch ex As Exception
            ErrMessage = String.Format(MSG0004E, "生徒マスタメンテ(" & Syori & ")")
            MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "生徒マスタメンテ(" & Syori & ")印刷", "失敗")
            Return False
        End Try

    End Function

    ''' <summary>
    ''' 生徒マスタメンテ（更新）印刷用のデータテーブルを初期化します。
    ''' </summary>
    ''' <remarks>2017/04/05 saitou 東春信金(RSV2標準) added for 生徒マスタメンテ帳票改善</remarks>
    Private Sub CreateSeitomastDataTable()
        Try
            '生徒マスタメンテ更新帳票印刷クラスから生徒マスタのカラム情報を取得する
            Dim KFGP107 As New KFGP107

            Me.dtBeforeSeitomast = New DataTable
            Me.dtAfterSeitomast = New DataTable

            'カラム情報取得
            For Each strColumnName As String In KFGP107.TableInfo.ColumnEnglishName
                Me.dtBeforeSeitomast.Columns.Add(strColumnName)
                Me.dtAfterSeitomast.Columns.Add(strColumnName)
            Next

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("生徒マスタ更新データテーブル作成", "失敗", ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 生徒マスタメンテ（更新）の更新前と更新後の生徒マスタの情報をデータテーブルに格納します。
    ''' </summary>
    ''' <param name="bBeforeFlg">True : 更新前 False : 更新後</param>
    ''' <returns>True or False</returns>
    ''' <remarks>2017/04/05 saitou 東春信金(RSV2標準) added for 生徒マスタメンテ帳票改善</remarks>
    Private Function fn_GetSEITOMAST_PrnCSVUpd(ByVal bBeforeFlg As Boolean) As Boolean

        Dim oraReader As New CASTCommon.MyOracleReader(Me.MainDB)
        Dim SQL As New StringBuilder
        Dim dtWorkTable As New DataTable

        Try
            'ワーク用のデータテーブルの作成
            dtWorkTable = Me.dtBeforeSeitomast.Clone

            '生徒マスタ抽出用のSQL作成
            With SQL
                .Append("select ")
                For i As Integer = 0 To KFGP107.TableInfo.ColumnEnglishName.Length - 1
                    If i = 0 Then : .Append(" " & KFGP107.TableInfo.ColumnEnglishName(i))
                    Else : .Append("," & KFGP107.TableInfo.ColumnEnglishName(i))
                    End If
                Next
                .Append(" from SEITOMAST")
                .Append(" where GAKKOU_CODE_O = " & SQ(Me.SeitomastMenteListInfo.GakkouCode))
                .Append(" and NENDO_O = " & SQ(Me.SeitomastMenteListInfo.Nendo))
                .Append(" and TUUBAN_O = " & GCom.NzInt(Me.SeitomastMenteListInfo.Tuuban))
                .Append(" order by TUKI_NO_O")
            End With

            '生徒マスタ情報格納
            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    Dim dr As DataRow = dtWorkTable.NewRow
                    For i As Integer = 0 To KFGP107.TableInfo.ColumnEnglishName.Length - 1
                        dr(KFGP107.TableInfo.ColumnEnglishName(i)) = oraReader.GetString(KFGP107.TableInfo.ColumnEnglishName(i))
                    Next
                    dtWorkTable.Rows.Add(dr)
                    oraReader.NextRead()
                End While
            Else
                '生徒マスタなし
                MainLOG.Write("生徒マスタ検索", "失敗", "生徒マスタ対象なし")
                Return False
            End If

            If bBeforeFlg = True Then
                Me.dtBeforeSeitomast = dtWorkTable.Copy
            Else
                Me.dtAfterSeitomast = dtWorkTable.Copy
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write("生徒マスタ更新データテーブル作成", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False

        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 生徒マスタメンテ（更新）のCSVを作成します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks>2017/04/05 saitou 東春信金(RSV2標準) added for 生徒マスタメンテ帳票改善</remarks>
    Private Function fn_CreateCSV_UPD() As Boolean
        Try
            Dim CreateCSV As New KFGP107
            Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
            Dim NowTime As String = Format(DateTime.Now, "HHmmss")
            Dim bUpdFlg As Boolean = False      '更新フラグ

            'CSV作成
            strCSV_FILE_NAME = CreateCSV.CreateCsvFile()

            '生徒の振替情報で更新された項目があるかチェック
            Dim drBeforeValue As DataRow() = Me.dtBeforeSeitomast.Select("TUKI_NO_O = '04'")
            Dim drAfterValue As DataRow() = Me.dtAfterSeitomast.Select("TUKI_NO_O = '04'")
            If drBeforeValue.Length = 0 OrElse drAfterValue.Length = 0 Then
                Return False
            End If

            '費目の情報が設定されているカラムの前まででチェック
            For i As Integer = 0 To KFGP107.TableInfo.HIMOKU_INFO_START_INDEX - 1
                If drBeforeValue(0)(i).ToString.Trim <> drAfterValue(0)(i).ToString.Trim Then
                    Call Me.SetUpdateListData(CreateCSV, Today, NowTime, i, drBeforeValue(0)(i).ToString.Trim, drAfterValue(0)(i).ToString)
                    bUpdFlg = True
                End If
            Next

            '更新日以降の予備項目のチェック
            For i As Integer = KFGP107.TableInfo.KOUSIN_DATE_INDEX + 1 To KFGP107.TableInfo.ColumnEnglishName.Length - 1
                If drBeforeValue(0)(i).ToString.Trim <> drAfterValue(0)(i).ToString.Trim Then
                    Call Me.SetUpdateListData(CreateCSV, Today, NowTime, i, drBeforeValue(0)(i).ToString.Trim, drAfterValue(0)(i).ToString)
                    bUpdFlg = True
                End If
            Next

            '費目情報のチェック
            '出力順は4月→3月
            For i As Integer = 4 To 12
                drBeforeValue = Me.dtBeforeSeitomast.Select("TUKI_NO_O = '" & i.ToString.PadLeft(2, "0"c) & "'")
                drAfterValue = Me.dtAfterSeitomast.Select("TUKI_NO_O = '" & i.ToString.PadLeft(2, "0"c) & "'")

                '費目の情報が設定されているカラムから更新日の前までチェック
                For j As Integer = KFGP107.TableInfo.HIMOKU_INFO_START_INDEX To KFGP107.TableInfo.KOUSIN_DATE_INDEX - 1
                    If drBeforeValue(0)(j).ToString.Trim <> drAfterValue(0)(j).ToString.Trim Then
                        Call Me.SetUpdateListData(CreateCSV, Today, NowTime, j, drBeforeValue(0)(j).ToString.Trim, drAfterValue(0)(j).ToString)
                        bUpdFlg = True
                    End If
                Next
            Next

            For i As Integer = 1 To 3
                drBeforeValue = Me.dtBeforeSeitomast.Select("TUKI_NO_O = '" & i.ToString.PadLeft(2, "0"c) & "'")
                drAfterValue = Me.dtAfterSeitomast.Select("TUKI_NO_O = '" & i.ToString.PadLeft(2, "0"c) & "'")

                '費目の情報が設定されているカラムから更新日の前までチェック
                For j As Integer = KFGP107.TableInfo.HIMOKU_INFO_START_INDEX To KFGP107.TableInfo.KOUSIN_DATE_INDEX - 1
                    If drBeforeValue(0)(j).ToString.Trim <> drAfterValue(0)(j).ToString.Trim Then
                        Call Me.SetUpdateListData(CreateCSV, Today, NowTime, j, drBeforeValue(0)(j).ToString.Trim, drAfterValue(0)(j).ToString)
                        bUpdFlg = True
                    End If
                Next
            Next

            CreateCSV.CloseCsv()
            CreateCSV = Nothing

            Return bUpdFlg

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(生徒マスタメンテ(更新))印刷", "失敗", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 生徒マスタメンテ（更新）のCSVを設定します。
    ''' </summary>
    ''' <remarks>2017/04/05 saitou 東春信金(RSV2標準) added for 生徒マスタメンテ帳票改善</remarks>
    Private Sub SetUpdateListData(ByRef CreateCSV As KFGP107, _
                                      ByVal Today As String, _
                                      ByVal NowTime As String, _
                                      ByVal intColumnIndex As Integer, _
                                      ByVal strBeforeValue As String, _
                                      ByVal strAfterValue As String)

        With CreateCSV
            .OutputCsvData(Today, True)
            .OutputCsvData(NowTime, True)
            .OutputCsvData(GCom.GetUserID, True)
            .OutputCsvData(Environment.MachineName, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.GakkouCode, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.Nendo, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.Tuuban, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.GakunenCode, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.ClassCode, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.SeitoNo, True)
            .OutputCsvData(Me.SeitomastMenteUpdateListInfo.GakkouName, True)
            .OutputCsvData(KFGP107.TableInfo.GetColumnJapaneseName(intColumnIndex), True)
            .OutputCsvData(KFGP107.TableInfo.GetColumnEnglishName(intColumnIndex), True)
            .OutputCsvData(strBeforeValue, True)
            .OutputCsvData(strAfterValue, True, True)
        End With
    End Sub

    '2017/04/05 saitou 東春信金(RSV2標準) DEL 生徒マスタメンテ帳票改善 ---------------------------------------- START
    ''2010/10/18 生徒マスタメンテナンス印刷処理改善用クラス
    'Private Class MyStringDictionary
    '    Inherits System.Collections.Specialized.StringDictionary

    '    Public Sub New(ByVal filename As String)
    '        Try
    '            Dim FileNameFull As String = Path.Combine(GCom.GetTXTFolder, filename)

    '            If File.Exists(FileNameFull) Then
    '                Using sr As StreamReader = New StreamReader(FileNameFull, Encoding.GetEncoding(932))
    '                    While sr.Peek > -1
    '                        Dim s() As String = sr.ReadLine().Split(","c)
    '                        If MyBase.ContainsKey(s(0).Trim) = False Then
    '                            MyBase.Add(s(0).Trim, s(1).Trim)
    '                        End If
    '                    End While
    '                End Using
    '            End If
    '        Catch
    '            Throw
    '        End Try
    '    End Sub

    '    Default Public Overrides Property Item(ByVal key As String) As String
    '        Get
    '            If ContainsKey(key) = True Then
    '                Return MyBase.Item(key)
    '            Else
    '                Return ""
    '            End If
    '        End Get
    '        Set(ByVal value As String)
    '            MyBase.Item(key) = value
    '        End Set
    '    End Property

    'End Class
    '2017/04/05 saitou 東春信金(RSV2標準) DEL ----------------------------------------------------------------- END

#End Region


End Class
