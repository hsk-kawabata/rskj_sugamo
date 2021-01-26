Option Explicit On 
Option Strict Off

Imports System.Text
Imports CASTCommon


Public Class KFGNENJ010
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 進級処理
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

#Region " 共通変数定義 "
    Dim INT使用学年数 As Integer
    Dim INT最高学年数 As Integer
    Dim STR進級年度 As String
    Dim STR学校コード As String
    Dim STR入学年度 As String
    Dim INT通番 As Integer
    Dim INT学年 As Integer
    Dim INTクラス As Integer
    Dim STR生徒番号 As String
    Dim STR生徒氏名カナ As String
    Dim STR生徒氏名漢字 As String
    Dim STR性別 As String
    Dim STR金融機関コード As String
    Dim STR支店コード As String
    Dim STR科目 As String
    Dim STR口座番号 As String
    Dim STR名義人カナ As String
    Dim STR名義人漢字 As String
    Dim STR振替方法 As String
    Dim STR契約住所漢字 As String
    Dim STR契約電話番号 As String
    Dim STR解約区分 As String
    Dim STR進級区分 As String
    Dim STR費目ＩＤ As String
    Dim INT長子_有無フラグ As Integer
    Dim STR長子_入学年度 As String
    Dim INT長子_通番 As Integer
    Dim INT長子_学年 As Integer
    Dim INT長子_クラス As Integer
    Dim STR長子_生徒番号 As String
    Dim STR請求月 As String
    Dim STR費目１請求方法 As String
    Dim LNG費目１請求金額 As Long
    Dim STR費目２請求方法 As String
    Dim LNG費目２請求金額 As Long
    Dim STR費目３請求方法 As String
    Dim LNG費目３請求金額 As Long
    Dim STR費目４請求方法 As String
    Dim LNG費目４請求金額 As Long
    Dim STR費目５請求方法 As String
    Dim LNG費目５請求金額 As Long
    Dim STR費目６請求方法 As String
    Dim LNG費目６請求金額 As Long
    Dim STR費目７請求方法 As String
    Dim LNG費目７請求金額 As Long
    Dim STR費目８請求方法 As String
    Dim LNG費目８請求金額 As Long
    Dim STR費目９請求方法 As String
    Dim LNG費目９請求金額 As Long
    Dim STR費目１０請求方法 As String
    Dim LNG費目１０請求金額 As Long
    Dim STR費目１１請求方法 As String
    Dim LNG費目１１請求金額 As Long
    Dim STR費目１２請求方法 As String
    Dim LNG費目１２請求金額 As Long
    Dim STR費目１３請求方法 As String
    Dim LNG費目１３請求金額 As Long
    Dim STR費目１４請求方法 As String
    Dim LNG費目１４請求金額 As Long
    Dim STR費目１５請求方法 As String
    Dim LNG費目１５請求金額 As Long
    Dim STR作成日付 As String
    Dim STR更新日付 As String
    Dim STR処理名 As String
#End Region

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGNENJ010", "進級処理画面")
    Private Const msgTitle As String = "進級処理画面(KFGNENJ010)"

    '2010/10/21 システム日付より過去年度の場合、進級不可とする--------------------------------
    'タイムスタンプ取得
    Private mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  '現在日付
    '-----------------------------------------------------------------------------------------
    '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- START
    Private HIMOKU_CLR_FLG As String = "0"  '費目金額クリアフラグ(0:クリアする、1:クリアしない)
    Private HIMOKU_CHK As String = "0"      '進級後の費目存在チェック要否(0:チェックしない、1:チェックする)

    Private Structure typPrnList
        Dim GAKKOU_CODE As String
        Dim GAKKOU_NNAME As String
        Dim NENDO As String
        Dim TUUBAN As Integer
        Dim GAKUNEN As String
        Dim CLASS_CODE As String
        Dim SEITO_NO As String
        Dim SEITO_KNAME As String
        Dim SEITO_NNAME As String
        Dim HIMOKU_ID_OLD As String
        Dim HIMOKU_ID_NEW As String
    End Structure

    Private ArrPrnList As ArrayList
    'Private PrnList As typPrnList
    '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- END

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGNENJ010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            'ログ用
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            With Me
                .WindowState = FormWindowState.Normal
                .FormBorderStyle = FormBorderStyle.FixedDialog
                .ControlBox = True
            End With

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            txtFuriDateY.Text = Mid(lblDate.Text, 1, 4)

            '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- START
            Dim strFlg As String = GetFSKJIni("GNENJI", "HIMOKU_KINGAKU_CLEAR_FLG")
            If Not strFlg = "" AndAlso Not strFlg.ToUpper = "ERR" AndAlso Not strFlg Is Nothing Then
                HIMOKU_CLR_FLG = strFlg
            End If

            Dim strChk As String = GetFSKJIni("GNENJI", "HIMOKU_CHK")
            If Not strChk = "" AndAlso Not strChk.ToUpper = "ERR" AndAlso Not strChk Is Nothing Then
                HIMOKU_CHK = strChk
            End If

            If HIMOKU_CLR_FLG = "1" Then
                Label7.Text = "費目マスタの各費目金額はクリアせず、生徒マスタの個別設定と金額のクリアのみ行います。"
            End If
            '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- END

            '入力ボタン制御
            btnAction.Enabled = True
            btnEnd.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim MainDB As New MyOracle
        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader = Nothing

        Try

            '入力値チェック
            If PFUNC_Nyuryoku_Check(MainDB) = False Then
                Return
            End If

            '2017/04/18 タスク）西野 ADD 標準版修正（進級処理事前チェック処理追加）------------------------------------ START
            If Not PFUNC_PROGRESS_CHECK(txtGAKKOU_CODE.Text, MainDB) Then
                MessageBox.Show(G_MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If
            '2017/04/18 タスク）西野 ADD 標準版修正（進級処理事前チェック処理追加）------------------------------------ END

            If MessageBox.Show(String.Format(MSG0015I, "進級"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            Select Case (Trim(txtGAKKOU_CODE.Text))
                Case "9999999999"

                    '全学校対象
                    '学校マスタ２読込み用のコネクションを新設

                    sql = New StringBuilder(128)
                    oraReader = New MyOracleReader(MainDB)

                    sql.Append(" SELECT * FROM GAKMAST2")
                    sql.Append(" ORDER BY GAKKOU_CODE_T ASC")

                    If oraReader.DataReader(sql) = False Then

                        oraReader.Close()
                        Return

                    Else

                        Do Until oraReader.EOF

                            '学校マスタ２から進級年度と使用学年を得る
                            '学校コードを共通変数に設定

                            STR学校コード = oraReader.GetString("GAKKOU_CODE_T")
                            LW.ToriCode = STR学校コード

                            '使用学年数
                            INT使用学年数 = oraReader.GetString("SIYOU_GAKUNEN_T")

                            '最高学年数
                            INT最高学年数 = oraReader.GetString("SAIKOU_GAKUNEN_T")

                            '進級年度
                            If IsDBNull(oraReader.GetString("SINKYU_NENDO_T")) = False Then
                                STR進級年度 = oraReader.GetString("SINKYU_NENDO_T")
                            Else
                                STR進級年度 = ""
                            End If

                            '学校マスタ２の進級年度と対象年度をみて、処理の妥当性をチェック
                            '2010/10/21 システム日付より過去年度の場合、進級不可とする
                            ''2007/08/17　再処理できるように変更
                            'If Trim(txtFuriDateY.Text) <= STR進級年度 Then
                            '    If MessageBox.Show("すでに" & STR進級年度 & "年度の進級処理は終了しています" & vbCrLf & "処理しますか？", STR学校コード, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) = DialogResult.Cancel Then
                            '        Return
                            '    End If
                            'End If
                            If Trim(txtFuriDateY.Text) >= mMatchingDate.Substring(0, 4) Then
                                '2007/08/17　再処理できるように変更
                                If Trim(txtFuriDateY.Text) <= STR進級年度 Then
                                    If MessageBox.Show(String.Format(G_MSG0010I, STR進級年度, STR学校コード), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.Cancel Then
                                        GoTo NEXT_WHILE
                                    End If
                                End If
                            Else
                                MessageBox.Show(G_MSG0039W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Sub
                            End If


                            'トランザクション開始
                            MainDB.BeginTrans()
                            MainLOG.Write("進級メイン処理", "成功", "開始")

                            '2017/04/19 タスク）西野 ADD 標準版修正（進級戻し処理対応）------------------------------------ START
                            If Not PFUNC_SINQBK(txtGAKKOU_CODE.Text, MainDB) Then
                                MainLOG.Write("進級前情報退避処理", "失敗", "")
                                MainDB.Rollback()
                                Return
                            End If
                            '2017/04/19 タスク）西野 ADD 標準版修正（進級戻し処理対応）------------------------------------ END

                            '生徒マスタの更新
                            If PFUNC_SEITOMAST_UPD(MainDB) = False Then
                                MainLOG.Write("生徒マスタ更新", "失敗", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If

                            '新入生マスタの更新
                            If PFUNC_SEITOMAST2_UPD(MainDB) = False Then
                                MainLOG.Write("新入生マスタ", "失敗", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If

                            '新入生の削除処理
                            If PFUNC_SEITOMAST2_DEL(MainDB) = False Then
                                MainLOG.Write("新入生削除処理", "失敗", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If

                            '学校マスタ２の更新
                            If PFUNC_GAKMAST2_UPD(MainDB) = False Then
                                MainLOG.Write("学校マスタ２更新処理", "失敗", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If

                            'エントリ１の削除処理
                            If PFUNC_G_ENTMAST1_DEL(MainDB) = False Then
                                MainLOG.Write("エントリマスタ１削除処理", "失敗", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            'エントリ２の削除処理
                            If PFUNC_G_ENTMAST2_DEL(MainDB) = False Then
                                MainLOG.Write("エントリマスタ２削除処理", "失敗", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '異常リストの削除処理
                            If PFUNC_G_IJYOLIST_DEL(MainDB) = False Then
                                MainLOG.Write("異常リスト削除処理", "失敗", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '明細マスタの削除処理
                            If PFUNC_G_MEIMAST_DEL(MainDB) = False Then
                                MainLOG.Write("明細マスタ削除処理", "失敗", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '実績マスタの削除処理
                            If PFUNC_JISSEKIMAST_DEL(MainDB) = False Then
                                MainLOG.Write("実績マスタ削除処理", "失敗", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            'スケジュールマスタの削除処理
                            If PFUNC_G_SCHMAST_DEL(MainDB) = False Then
                                MainLOG.Write("スケジュールマスタ削除処理", "失敗", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '他行スケジュールマスタの削除処理
                            If PFUNC_G_TAKOUSCHMAST_DEL(MainDB) = False Then
                                MainLOG.Write("他行スケジュールマスタ削除処理", "失敗", "")
                                MainDB.Rollback()
                                GoTo NEXT_WHILE
                            End If
                            '2009/01/08 費目マスタ金額のクリアを行う----
                            '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- START
                            If HIMOKU_CLR_FLG = "0" Then
                                '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- END
                                '費目マスタの更新
                                If PFUNC_HIMOMAST_UPD(MainDB) = False Then
                                    MainLOG.Write("費目マスタ更新処理", "失敗", "")
                                    MainDB.Rollback()
                                    GoTo NEXT_WHILE
                                End If
                                '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- START
                            End If
                            '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- END
                            '-------------------------------------------

                            '2016/11/07 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- START
                            '進級後の生徒マスタ整合性チェックを行う
                            If HIMOKU_CHK = "1" Then
                                If PFUNC_HIMOKU_CHK(MainDB) = False Then
                                    MainLOG.Write("生徒マスタ整合性チェック", "失敗", "")
                                    MainDB.Rollback()
                                    GoTo NEXT_WHILE
                                Else
                                    MessageBox.Show(String.Format(MSG0014I, "生徒マスタ整合性チェックリスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                End If
                            End If
                            '2016/11/07 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- END

                            'トランザクション終了（ＣＯＭＭＩＴ）
                            MainDB.Commit()
                            MainLOG.Write("進級メイン処理", "成功", "コミット")

NEXT_WHILE:

                            oraReader.NextRead()

                        Loop

                    End If

                    oraReader.Close()

                Case Else

                    '入力学校コードのみ対象
                    '===2008/04/16 学校コードをログに出力するよう修正===
                    LW.ToriCode = txtGAKKOU_CODE.Text
                    '===================================================

                    '学校マスタ２から進級年度と使用学年を得る
                    If PFUNC_GAKMAST2_GET(MainDB) = False Then
                        Exit Sub
                    End If

                    '学校マスタ２の進級年度と対象年度をみて、処理の妥当性をチェック
                    '2010/10/21 システム日付より過去年度の場合、進級不可とする
                    ''2007/08/17　再処理できるように変更
                    'If Trim(txtFuriDateY.Text) <= STR進級年度 Then
                    '    If MessageBox.Show("すでに" & STR進級年度 & "年度の進級処理は終了しています" & vbCrLf & "処理しますか？", STR学校コード, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) = DialogResult.Cancel Then
                    '        Return
                    '    End If
                    'End If
                    If Trim(txtFuriDateY.Text) >= mMatchingDate.Substring(0, 4) Then
                        '2007/08/17　再処理できるように変更
                        If Trim(txtFuriDateY.Text) <= STR進級年度 Then
                            If MessageBox.Show(String.Format(G_MSG0010I, STR進級年度, STR学校コード), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.Cancel Then
                                Return
                            End If
                        End If
                    Else
                        MessageBox.Show(G_MSG0039W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                    '-----------------------------------------------------------------

                    'トランザクション開始
                    MainDB.BeginTrans()
                    MainLOG.Write("進級メイン処理", "成功", "開始")

                    '2017/04/19 タスク）西野 ADD 標準版修正（進級戻し処理対応）------------------------------------ START
                    If Not PFUNC_SINQBK(txtGAKKOU_CODE.Text, MainDB) Then
                        MainLOG.Write("進級前情報退避処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '2017/04/19 タスク）西野 ADD 標準版修正（進級戻し処理対応）------------------------------------ END

                    '生徒マスタの更新
                    If PFUNC_SEITOMAST_UPD(MainDB) = False Then
                        MainLOG.Write("生徒マスタ更新処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If

                    '新入生マスタの更新
                    If PFUNC_SEITOMAST2_UPD(MainDB) = False Then
                        MainLOG.Write("新入生マスタ更新処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If

                    '新入生の削除処理
                    If PFUNC_SEITOMAST2_DEL(MainDB) = False Then
                        MainLOG.Write("新入生削除処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If

                    '学校マスタ２の更新
                    If PFUNC_GAKMAST2_UPD(MainDB) = False Then
                        MainLOG.Write("学校マスタ２更新処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If
                    'エントリ１の削除処理
                    If PFUNC_G_ENTMAST1_DEL(MainDB) = False Then
                        MainLOG.Write("エントリマスタ１削除処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If
                    'エントリ２の削除処理
                    If PFUNC_G_ENTMAST2_DEL(MainDB) = False Then
                        MainLOG.Write("エントリマスタ２削除処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '異常リストの削除処理
                    If PFUNC_G_IJYOLIST_DEL(MainDB) = False Then
                        MainLOG.Write("異常リスト削除処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '明細マスタの削除処理
                    If PFUNC_G_MEIMAST_DEL(MainDB) = False Then
                        MainLOG.Write("明細マスタ削除処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '実績マスタの削除処理
                    If PFUNC_JISSEKIMAST_DEL(MainDB) = False Then
                        MainLOG.Write("実績マスタ削除処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If
                    'スケジュールマスタの削除処理
                    If PFUNC_G_SCHMAST_DEL(MainDB) = False Then
                        MainLOG.Write("スケジュールマスタ削除処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If
                    '他行スケジュールマスタの削除処理
                    If PFUNC_G_TAKOUSCHMAST_DEL(MainDB) = False Then
                        MainLOG.Write("他行スケジュールマスタ削除処理", "失敗", "")
                        MainDB.Rollback()
                        Return
                    End If

                    '2009/01/08 費目マスタ金額のクリアを行う----
                    '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- START
                    If HIMOKU_CLR_FLG = "0" Then
                        '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- END
                        '費目マスタの更新
                        If PFUNC_HIMOMAST_UPD(MainDB) = False Then
                            MainLOG.Write("費目マスタ更新処理", "失敗", "")
                            MainDB.Rollback()
                            Return
                        End If
                        '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- START
                    End If
                    '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- END
                    '-------------------------------------------

                    '2016/11/07 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- START
                    '進級後の生徒マスタ整合性チェックを行う
                    If HIMOKU_CHK = "1" Then
                        If PFUNC_HIMOKU_CHK(MainDB) = False Then
                            MainLOG.Write("生徒マスタ整合性チェック", "失敗", "")
                            MainDB.Rollback()
                            Return
                        End If
                    End If
                    '2016/11/07 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- END

                    'トランザクション終了（ＣＯＭＭＩＴ）
                    MainDB.Commit()
                    MainLOG.Write("進級メイン処理", "成功", "コミット")

            End Select

            MessageBox.Show(String.Format(MSG0016I, "進級"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)例外エラー", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '終了ボタン
        Me.Close()

    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET() As Boolean

        Dim sGakkkou_Name As String = ""
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        '学校名の設定
        sql.Append(" SELECT GAKKOU_NNAME_G FROM GAKMAST1")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_G ='" & txtGAKKOU_CODE.Text & "'")

        If oraReader.DataReader(sql) = True Then
            sGakkkou_Name = oraReader.GetString("GAKKOU_NNAME_G")
        End If
        oraReader.Close()

        '学校マスタ１存在チェック
        If sGakkkou_Name = "" Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            lab学校名.Text = ""
            txtGAKKOU_CODE.Focus()
            Return False
        End If

        lab学校名.Text = sGakkkou_Name

        Return True

    End Function
    Private Function PFUNC_GAKMAST2_GET(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(db)

        '学校マスタ２の取得
        sql.Append(" SELECT * FROM GAKMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_T ='" & txtGAKKOU_CODE.Text & "'")

        If oraReader.DataReader(sql) = False Then
            oraReader.Close()
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        Else

            '学校コードを共通変数に設定
            STR学校コード = oraReader.GetString("GAKKOU_CODE_T")

            '使用学年数
            INT使用学年数 = oraReader.GetString("SIYOU_GAKUNEN_T")

            '最高学年数
            INT最高学年数 = oraReader.GetString("SAIKOU_GAKUNEN_T")

            '進級年度
            If IsDBNull(oraReader.GetString("SINKYU_NENDO_T")) = False Then
                STR進級年度 = oraReader.GetString("SINKYU_NENDO_T")
            Else
                STR進級年度 = ""
            End If

        End If
        oraReader.Close()

        Return True

    End Function
    Private Function PFUNC_SEITOMAST_UPD(ByVal db As MyOracle) As Boolean

        Dim sql As StringBuilder

        '生徒マスタの更新（卒業生の削除）

        PFUNC_SEITOMAST_UPD = False

        '卒業生の削除
        sql = New StringBuilder(128)
        sql.Append(" DELETE  FROM SEITOMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR学校コード & "'")
        sql.Append(" AND")
        sql.Append(" GAKUNEN_CODE_O = " & INT最高学年数)
        sql.Append(" AND")
        sql.Append(" SINKYU_KBN_O ='0'")


        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        '在校生の進級処理
        sql = New StringBuilder(128)
        sql.Append(" UPDATE  SEITOMAST SET ")
        sql.Append(" GAKUNEN_CODE_O = GAKUNEN_CODE_O + 1")
        sql.Append(",KOUSIN_DATE_O = '" & Format(Now, "yyyyMMdd") & "'")

        '2008/02/07　請求方法と金額をクリアにする
        For i As Integer = 1 To 15
            sql.Append(" ,SEIKYU" & Format(i, "00") & "_O ='0' ")
            sql.Append(" ,KINGAKU" & Format(i, "00") & "_O = 0 ")
        Next
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR学校コード & "'")
        sql.Append(" AND")
        sql.Append(" GAKUNEN_CODE_O < " & INT最高学年数)
        sql.Append(" AND")
        sql.Append(" SINKYU_KBN_O ='0'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        '2008/03/13 解約済み生徒（解約フラグ＝９）の生徒マスタを削除する============================
        sql = New StringBuilder(128)
        sql.Append(" DELETE  FROM SEITOMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR学校コード & "'")
        sql.Append(" AND")
        sql.Append(" KAIYAKU_FLG_O = '9'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        '==========================================================================================

        '2009/01/14 進級しない生徒も請求方法と金額をクリアする----------------------------------------------------
        sql = New StringBuilder(128)
        sql.Append(" UPDATE  SEITOMAST SET ")
        sql.Append(" SEIKYU01_O ='0' ")
        sql.Append(" ,KINGAKU01_O = 0 ")
        Dim j As Integer
        For j = 2 To 15
            sql.Append(" ,SEIKYU" & Format(j, "00") & "_O ='0' ")
            sql.Append(" ,KINGAKU" & Format(j, "00") & "_O = 0 ")
        Next
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR学校コード & "'")
        sql.Append(" AND")
        sql.Append(" ( SINKYU_KBN_O ='1' OR GAKUNEN_CODE_O > " & INT最高学年数 & ")")  '進級しないor学年>最高学年

        If db.ExecuteNonQuery(sql) < 0 Then
            '更新処理エラー
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        '----------------------------------------------------------------------------------------------------------

        Return True

    End Function
    Private Function PFUNC_SEITOMAST2_UPD(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '新入生マスタの処理
        sql.Append(" INSERT INTO SEITOMAST")
        sql.Append(" SELECT ")
        sql.Append(" GAKKOU_CODE_O")
        sql.Append(", NENDO_O")
        sql.Append(", TUUBAN_O")
        sql.Append(", 1")
        sql.Append(", CLASS_CODE_O")
        sql.Append(", SEITO_NO_O")
        sql.Append(", SEITO_KNAME_O")
        sql.Append(", SEITO_NNAME_O")
        sql.Append(", SEIBETU_O")
        sql.Append(", TKIN_NO_O")
        sql.Append(", TSIT_NO_O")
        sql.Append(", KAMOKU_O")
        sql.Append(", KOUZA_O")
        sql.Append(", MEIGI_KNAME_O")
        sql.Append(", MEIGI_NNAME_O")
        sql.Append(", FURIKAE_O")
        sql.Append(", KEIYAKU_NJYU_O")
        sql.Append(", KEIYAKU_DENWA_O")
        sql.Append(", KAIYAKU_FLG_O")
        sql.Append(", SINKYU_KBN_O")
        sql.Append(", HIMOKU_ID_O")
        sql.Append(", TYOUSI_FLG_O")
        sql.Append(", TYOUSI_NENDO_O")
        sql.Append(", TYOUSI_TUUBAN_O")
        sql.Append(", TYOUSI_GAKUNEN_O")
        sql.Append(", TYOUSI_CLASS_O")
        sql.Append(", TYOUSI_SEITONO_O")
        sql.Append(", TUKI_NO_O")
        sql.Append(", SEIKYU01_O         ")
        sql.Append(", KINGAKU01_O")
        sql.Append(", SEIKYU02_O         ")
        sql.Append(", KINGAKU02_O")
        sql.Append(", SEIKYU03_O         ")
        sql.Append(", KINGAKU03_O")
        sql.Append(", SEIKYU04_O         ")
        sql.Append(", KINGAKU04_O")
        sql.Append(", SEIKYU05_O         ")
        sql.Append(", KINGAKU05_O")
        sql.Append(", SEIKYU06_O        ")
        sql.Append(", KINGAKU06_O")
        sql.Append(", SEIKYU07_O        ")
        sql.Append(", KINGAKU07_O")
        sql.Append(", SEIKYU08_O         ")
        sql.Append(", KINGAKU08_O")
        sql.Append(", SEIKYU09_O         ")
        sql.Append(", KINGAKU09_O")
        sql.Append(", SEIKYU10_O         ")
        sql.Append(", KINGAKU10_O")
        sql.Append(", SEIKYU11_O         ")
        sql.Append(", KINGAKU11_O")
        sql.Append(", SEIKYU12_O         ")
        sql.Append(", KINGAKU12_O")
        sql.Append(", SEIKYU13_O        ")
        sql.Append(", KINGAKU13_O")
        sql.Append(", SEIKYU14_O        ")
        sql.Append(", KINGAKU14_O")
        sql.Append(", SEIKYU15_O         ")
        sql.Append(", KINGAKU15_O")
        sql.Append(", SAKUSEI_DATE_O")
        sql.Append(", KOUSIN_DATE_O")
        sql.Append(", YOBI1_O")
        sql.Append(", YOBI2_O")
        sql.Append(", YOBI3_O")
        sql.Append(", YOBI4_O")
        sql.Append(", YOBI5_O")
        sql.Append(" FROM SEITOMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR学校コード & "'")
        sql.Append(" AND")
        sql.Append(" NENDO_O = '" & txtFuriDateY.Text & "'")
        sql.Append(" AND")
        sql.Append(" SINKYU_KBN_O = '0'")
        sql.Append(" AND KAIYAKU_FLG_O = '0' ")      '2008/03/23 解約の生徒はINSERTしない

        If db.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show(String.Format(MSG0002E, "挿入"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_SEITOMAST2_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '新入生マスタの削除
        sql.Append(" DELETE  FROM SEITOMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & STR学校コード & "'")
        sql.Append(" AND")
        sql.Append(" NENDO_O = '" & txtFuriDateY.Text & "'")
        sql.Append(" AND")
        sql.Append(" SINKYU_KBN_O = '0'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_GAKMAST2_UPD(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '学校マスタ２の進級年度更新
        sql.Append(" UPDATE  GAKMAST2 SET ")
        sql.Append(" SINKYU_NENDO_T ='" & txtFuriDateY.Text & "'")
        sql.Append(",KOUSIN_DATE_T ='" & Format(Now, "yyyyMMdd") & "'")
        sql.Append("  WHERE")
        sql.Append(" GAKKOU_CODE_T ='" & STR学校コード & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_Nyuryoku_Check(ByVal db As MyOracle) As Boolean

        Dim sStart As String
        Dim sEnd As String

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Return False
        ElseIf txtGAKKOU_CODE.Text <> "9999999999" Then '2007/03/28　全件対象の場合、存在チェックをしない
            '学校マスタ存在チェック

            Dim sql As New StringBuilder(128)
            Dim oraReader As New MyOracleReader(db)

            sql.Append("SELECT *")
            sql.Append(" FROM GAKMAST2")
            sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If oraReader.DataReader(sql) = False Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                oraReader.Close()
                Return False
            Else

                sStart = Mid(oraReader.GetString("KAISI_DATE_T"), 1, 4)
                '2007/03/28　修正
                'sEnd = Mid(OBJ_DATAREADER.Item("SYURYOU_DATE_T"), 1, 3)
                sEnd = Mid(oraReader.GetString("SYURYOU_DATE_T"), 1, 4)

            End If
            oraReader.Close()

        Else
            '2007/03/28　全件対象の場合、取引対象年度範囲のチェックはしない
            If (Trim(txtFuriDateY.Text)) = "" Then
                MessageBox.Show(String.Format(MSG0285W, "新年度"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            Else
                Return True
            End If

        End If

        If (Trim(txtFuriDateY.Text)) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "新年度"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY.Focus()
            Return False
        Else
            '2007/03/28　開始年・終了年チェックの修正
            'Select case (sStart <= txtFuriDateY.Text >= sEnd)
            '    Case False
            '        Call GSUB_MESSAGE_WARNING( "対象年度が入力範囲外です(" & sStart & "～" & sEnd & ")")
            '        txtFuriDateY.Focus()
            '        Exit Function
            'End Select
            If sStart > txtFuriDateY.Text Or sEnd < txtFuriDateY.Text Then
                MessageBox.Show(String.Format(G_MSG0040W, sStart, sEnd), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

        End If

        Return True

    End Function

    Private Function PFUNC_G_IJYOLIST_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '異常リストの削除
        sql.Append(" DELETE  FROM G_IJYOLIST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_L ='" & STR学校コード & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_G_ENTMAST1_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        'エントリマスタ１の削除
        sql.Append(" DELETE  FROM G_ENTMAST1")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_E ='" & STR学校コード & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_G_ENTMAST2_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        'エントリマスタ２の削除
        sql.Append(" DELETE  FROM G_ENTMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_E ='" & STR学校コード & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_G_MEIMAST_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '新入生マスタの削除
        sql.Append(" DELETE  FROM G_MEIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_M ='" & STR学校コード & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_JISSEKIMAST_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '新入生マスタの削除
        sql.Append(" DELETE  FROM JISSEKIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_F ='" & STR学校コード & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_G_SCHMAST_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '新入生マスタの削除
        sql.Append(" DELETE  FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" NENGETUDO_S BETWEEN '" & CStr(CInt(txtFuriDateY.Text) - 1) & "04' AND '" & txtFuriDateY.Text & "03'")
        sql.Append(" AND")
        sql.Append(" GAKKOU_CODE_S ='" & STR学校コード & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_G_TAKOUSCHMAST_DEL(ByVal db As MyOracle) As Boolean

        Dim sql As New StringBuilder(128)

        '新入生マスタの削除
        sql.Append(" DELETE  FROM G_TAKOUSCHMAST")
        sql.Append(" WHERE")
        sql.Append(" NENGETUDO_U BETWEEN '" & CStr(CInt(txtFuriDateY.Text) - 1) & "04' AND '" & txtFuriDateY.Text & "03'")
        sql.Append(" AND")
        sql.Append(" GAKKOU_CODE_U ='" & STR学校コード & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            '削除処理エラー
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function

    '2009/01/08 追加
    Private Function PFUNC_HIMOMAST_UPD(ByVal db As MyOracle) As Boolean

        '費目マスタの更新（金額クリア）

        Dim sql As New StringBuilder(128)

        sql.Append(" UPDATE  HIMOMAST SET ")
        sql.Append(" KOUSIN_DATE_H = '" & Format(Now, "yyyyMMdd") & "'")

        For i As Integer = 1 To 15
            sql.Append(" ,HIMOKU_KINGAKU" & Format(i, "00") & "_H = 0 ")
        Next
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_H ='" & STR学校コード & "'")

        If db.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function

    '2016/11/07 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- START
    '進級後の費目マスタが存在するかチェックし、存在しなければ対象学年の最小費目ＩＤを設定する（０００以外）
    'また、費目マスタが存在しなかった場合は、生徒マスタ整合性チェックリストを出力する。
    Private Function PFUNC_HIMOKU_CHK(ByVal db As MyOracle) As Boolean
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(db)

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)開始", "成功", "")

        '印刷配列をクリアする
        If Not ArrPrnList Is Nothing Then
            ArrPrnList.Clear()
        Else
            ArrPrnList = New ArrayList
        End If

        Try
            '新旧後の費目マスタが存在しない生徒を取得する
            With sql
                .Append("SELECT ")
                .Append(" GAKKOU_CODE_O ")
                .Append(",GAKKOU_NNAME_G ")
                .Append(",GAKKOU_KNAME_G ")
                .Append(",NENDO_O ")
                .Append(",TUUBAN_O ")
                .Append(",GAKUNEN_CODE_O ")
                .Append(",CLASS_CODE_O ")
                .Append(",SEITO_NO_O ")
                .Append(",SEITO_KNAME_O ")
                .Append(",SEITO_NNAME_O ")
                .Append(",HIMOKU_ID_O ")
                .Append(",HIMOKU_ID_H ")
                .Append("FROM ")
                .Append(" SEITOMAST ")
                .Append(",HIMOMAST ")
                .Append(",(SELECT DISTINCT ")
                .Append(" GAKKOU_CODE_G ")
                .Append(",GAKKOU_NNAME_G ")
                .Append(",GAKKOU_KNAME_G ")
                .Append("FROM ")
                .Append(" GAKMAST1) GMAST ")
                .Append("WHERE ")
                .Append("SEITOMAST.GAKKOU_CODE_O = GMAST.GAKKOU_CODE_G ")
                .Append("AND SEITOMAST.GAKKOU_CODE_O = HIMOMAST.GAKKOU_CODE_H(+) ")
                .Append("AND SEITOMAST.GAKUNEN_CODE_O = HIMOMAST.GAKUNEN_CODE_H(+) ")
                .Append("AND SEITOMAST.HIMOKU_ID_O = HIMOMAST.HIMOKU_ID_H(+) ")
                .Append("AND tuki_no_o=tuki_no_h(+) ")
                .Append("AND tuki_no_o='04' ")
                .Append("AND himoku_id_h is null ")
                .Append("order by GAKKOU_CODE_O,GAKUNEN_CODE_O,HIMOKU_ID_O ")
            End With

            If oraReader.DataReader(sql) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)終了", "失敗", "学校コード：" & STR学校コード)
                MessageBox.Show(String.Format(MSG0034E, "費目チェック"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            Else
                Do Until oraReader.EOF
                    Dim PrnLiST As New typPrnList

                    '印刷用の構造体に保持する
                    With PrnList
                        .GAKKOU_CODE = oraReader.GetString("GAKKOU_CODE_O")
                        .GAKKOU_NNAME = oraReader.GetString("GAKKOU_NNAME_G")
                        .NENDO = oraReader.GetString("NENDO_O")
                        .TUUBAN = oraReader.GetInt64("TUUBAN_O")
                        .GAKUNEN = oraReader.GetString("GAKUNEN_CODE_O")
                        .CLASS_CODE = oraReader.GetString("CLASS_CODE_O")
                        .SEITO_NO = oraReader.GetString("SEITO_NO_O")
                        .SEITO_KNAME = oraReader.GetString("SEITO_KNAME_O")
                        .SEITO_NNAME = oraReader.GetString("SEITO_NNAME_O")
                        .HIMOKU_ID_OLD = oraReader.GetString("HIMOKU_ID_O")
                    End With

                    '生徒データから対象学年の最小費目コードを取得し更新する
                    Dim oraReaderHimo As New MyOracleReader(db)
                    Dim SQL_HIMO As New StringBuilder
                    Dim SQL_UPD As New StringBuilder

                    Try
                        With SQL_HIMO
                            .Append("SELECT ")
                            .Append(" MIN(HIMOKU_ID_H) MIN_HIMOKU_ID ")
                            .Append("FROM ")
                            .Append(" HIMOMAST ")
                            .Append("WHERE ")
                            .Append(" GAKKOU_CODE_H = " & SQ(PrnList.GAKKOU_CODE))
                            .Append(" AND GAKUNEN_CODE_H = " & SQ(PrnList.GAKUNEN))
                            .Append(" AND HIMOKU_ID_H <> '000' ")
                            .Append("GROUP BY ")
                            .Append(" GAKKOU_CODE_H")
                            .Append(",GAKUNEN_CODE_H")
                        End With

                        If oraReaderHimo.DataReader(SQL_HIMO) = False Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)終了", "失敗", _
                                          String.Format("最小費目コード取得失敗　学校コード：{0}、学年：{1}", PrnList.GAKKOU_CODE, PrnList.GAKUNEN))
                            MessageBox.Show(String.Format(MSG0034E, "費目チェック"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            txtGAKKOU_CODE.Focus()
                            Return False
                        Else
                            Dim MIN_HIMOKU_ID As String = oraReaderHimo.GetString("MIN_HIMOKU_ID")
                            With SQL_UPD
                                .Append(" UPDATE SEITOMAST SET ")
                                .Append(" HIMOKU_ID_O = " & SQ(MIN_HIMOKU_ID))
                                .Append(" WHERE")
                                .Append(" GAKKOU_CODE_O =" & SQ(PrnList.GAKKOU_CODE))
                                .Append(" AND")
                                .Append(" NENDO_O = " & SQ(PrnList.NENDO))
                                .Append(" AND")
                                .Append(" TUUBAN_O = " & PrnList.TUUBAN.ToString)
                            End With

                            If db.ExecuteNonQuery(SQL_UPD) < 0 Then
                                '更新処理エラー
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)終了", "失敗", _
                                    String.Format("生徒マスタ更新失敗　学校コード：{0}、入学年度：{1}、通番：{2}", PrnList.GAKKOU_CODE, PrnList.NENDO, PrnList.TUUBAN.ToString))
                                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return False
                            Else
                                '更新した費目ＩＤを構造体に保持する
                                PrnList.HIMOKU_ID_NEW = oraReaderHimo.GetString("MIN_HIMOKU_ID")
                            End If
                        End If

                    Catch ex As Exception
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)終了", "失敗", ex.ToString)
                        MessageBox.Show(String.Format(MSG0034E, "費目チェック"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                    Finally
                        If Not oraReaderHimo Is Nothing Then
                            oraReaderHimo.Close()
                            oraReaderHimo = Nothing
                        End If
                    End Try

                    ArrPrnList.Add(PrnLiST)
                    oraReader.NextRead()
                Loop

            End If

            If ArrPrnList.Count > 0 Then
                Dim CreateCSV As New KFGP036(STR学校コード)

                Dim CSV_FILE_NAME As String = CreateCSV.CreateCsvFile()

                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(生徒マスタ整合性チェックリスト)印刷開始", "成功", "CSVファイル名：" & CSV_FILE_NAME)

                For i As Long = 0 To ArrPrnList.Count - 1
                    Dim PrnList As typPrnList = CType(ArrPrnList(i), typPrnList)

                    With CreateCSV
                        .OutputCsvData(PrnList.GAKKOU_CODE, True)
                        .OutputCsvData(PrnList.GAKKOU_NNAME, True)
                        .OutputCsvData(txtFuriDateY.Text, True)
                        .OutputCsvData(PrnList.NENDO, True)
                        .OutputCsvData(PrnList.TUUBAN.ToString, True)
                        .OutputCsvData(PrnList.GAKUNEN, True)
                        .OutputCsvData(PrnList.CLASS_CODE)
                        .OutputCsvData(PrnList.SEITO_NO, True)
                        .OutputCsvData(PrnList.SEITO_KNAME, True)
                        .OutputCsvData(PrnList.SEITO_NNAME, True)
                        .OutputCsvData(PrnList.HIMOKU_ID_OLD, True)
                        .OutputCsvData(PrnList.HIMOKU_ID_NEW, True, True)
                    End With
                Next
                CreateCSV.CloseCsv()

                '印刷バッチ呼び出し
                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me

                'パラメータ設定：ログイン名、ＣＳＶファイル名、学校コード
                Dim Param As String = GCom.GetUserID & "," & CSV_FILE_NAME & "," & STR学校コード
                Dim nRet As Integer = ExeRepo.ExecReport("KFGP036.EXE", Param)
                Dim ErrMessage As String = ""
                If nRet <> 0 Then
                    '印刷失敗：戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case -1
                            ErrMessage = String.Format(MSG0106W, "生徒マスタ整合性チェックリスト")
                        Case Else
                            ErrMessage = String.Format(MSG0004E, "生徒マスタ整合性チェックリスト")
                    End Select

                    MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(生徒マスタ整合性チェックリスト)印刷終了", "成功", "")
                If STR学校コード <> "9999999999" Then
                    MessageBox.Show(String.Format(MSG0014I, "生徒マスタ整合性チェックリスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)終了", "成功", "")
            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_HIMOKU_CHK)終了", "失敗", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, "費目チェック"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function
    '2016/11/07 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(UI_11-012) -------------------- END

    '2017/04/18 タスク）西野 ADD 標準版修正（進級処理事前チェック処理追加）------------------------------------ START
    ''' <summary>
    ''' 指定された学校の進行中スケジュールをチェックする
    ''' </summary>
    ''' <param name="GAKKOU_CODE">対象の学校コード</param>
    ''' <param name="db">DB</param>
    ''' <returns>TRUE:進行中スケジュールなし FALSE:進行中スケジュールあり</returns>
    ''' <remarks></remarks>
    Private Function PFUNC_PROGRESS_CHECK(ByVal GAKKOU_CODE As String, ByRef db As MyOracle) As Boolean
        Dim SQL As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(db)
        Dim bRet As Boolean = False

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_PROGRESS_CHECK)開始", "成功", "")

        Try
            With SQL
                .Append("SELECT ")
                .Append(" COUNT(FURI_DATE_S) CNT ")
                .Append("FROM ")
                .Append(" G_SCHMAST ")
                .Append("WHERE ")
                .Append(" (ENTRI_FLG_S = '1' OR CHECK_FLG_S = '1' OR DATA_FLG_S = '1') ")   '明細作成または金額チェックまたは落込が処理済
                .Append(" AND FUNOU_FLG_S = '0' ")                                          '不能が未処理
                .Append(" AND TYUUDAN_FLG_S = '0' ")                                        '中断ではない
                If GAKKOU_CODE <> "9999999999" Then
                    .Append(" AND GAKKOU_CODE_S = " & SQ(GAKKOU_CODE))
                End If
            End With

            If oraReader.DataReader(SQL) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_PROGRESS_CHECK)終了", "失敗", "学校コード：" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, "進行中スケジュールチェック"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            Else
                If oraReader.GetInt64("CNT") = 0 Then
                    '進行中スケジュールなし
                    bRet = True
                Else
                    '進行中スケジュールあり
                    bRet = False
                End If
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_PROGRESS_CHECK)終了", "成功", "")
            Return bRet

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_PROGRESS_CHECK)終了", "失敗", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, "進行中スケジュールチェック"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function
    '2017/04/18 タスク）西野 ADD 標準版修正（進級処理事前チェック処理追加）------------------------------------ END

    '2017/04/19 タスク）西野 ADD 標準版修正（進級戻し処理対応）------------------------------------ START
    ''' <summary>
    ''' 指定された学校の進級前情報を退避する
    ''' </summary>
    ''' <param name="GAKKOU_CODE">対象の学校コード</param>
    ''' <param name="db">DB</param>
    ''' <returns>TRUE:退避成功 FALSE:退避失敗</returns>
    ''' <remarks></remarks>
    Private Function PFUNC_SINQBK(ByVal GAKKOU_CODE As String, ByRef db As MyOracle) As Boolean
        Dim TARGET_TABLE As String = ""
        Dim SINQBK_TABLE As String = ""
        Dim KEY_NAME As String = ""

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBK)開始", "成功", "学校コード：" & GAKKOU_CODE)

        '-----------------------------------------------
        ' 学校マスタ２退避処理
        '-----------------------------------------------
        TARGET_TABLE = "GAKMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_T"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 費目マスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "HIMOMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_H"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 学校スケジュールマスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "G_SCHMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_S"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 学校他行スケジュールマスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "G_TAKOUSCHMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_U"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 生徒マスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "SEITOMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_O"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 新入生マスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "SEITOMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_O"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 学校明細マスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "G_MEIMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_M"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 学校エントリマスタ１退避処理
        '-----------------------------------------------
        TARGET_TABLE = "G_ENTMAST1"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_E"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 学校エントリマスタ２退避処理
        '-----------------------------------------------
        TARGET_TABLE = "G_ENTMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_E"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 実績マスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "JISSEKIMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_F"
        If Not PFUNC_MASTtoSINQBK(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBK)終了", "成功", "学校コード：" & GAKKOU_CODE)
        Return True
    End Function

    ''' <summary>
    ''' マスタ退避処理
    ''' </summary>
    ''' <param name="GAKKOU_CODE">学校コード</param>
    ''' <param name="TARGET_TABLE">退避元テーブル名</param>
    ''' <param name="SINQBK_TABLE">退避先テーブル名</param>
    ''' <param name="KEY_NAME">キー名</param>
    ''' <param name="db">DB</param>
    ''' <returns>TRUE:退避成功 FALSE:退避失敗</returns>
    ''' <remarks></remarks>
    Private Function PFUNC_MASTtoSINQBK(ByVal GAKKOU_CODE As String, _
                                        ByVal TARGET_TABLE As String, _
                                        ByVal SINQBK_TABLE As String, _
                                        ByVal KEY_NAME As String, _
                                        ByRef db As MyOracle) As Boolean

        Dim SYORI_NAME As String = ""

        Dim SQL As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(db)
        Dim bRet As Boolean = False
        Dim DEL_CNT As Long = 0
        Dim INS_CNT As Long = 0
        Dim ResultMsg As String = ""

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)開始", "成功", TARGET_TABLE)

        '-----------------------------------------------
        ' 進級用テーブル内の削除
        '-----------------------------------------------
        Try
            SYORI_NAME = "進級用マスタ削除"

            With SQL
                .Append("DELETE FROM " & SINQBK_TABLE)
                .Append(" WHERE " & KEY_NAME & " = " & SQ(GAKKOU_CODE))
            End With

            DEL_CNT = db.ExecuteNonQuery(SQL)
            If DEL_CNT = -1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)終了", "失敗", SYORI_NAME & "処理失敗 学校コード：" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)終了", "失敗", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        '-----------------------------------------------
        ' 進級用テーブルへの退避
        '-----------------------------------------------
        Try
            SYORI_NAME = "マスタ退避"

            SQL.Length = 0
            With SQL
                .Append("INSERT INTO " & SINQBK_TABLE)
                .Append(" SELECT  * FROM " & TARGET_TABLE & " WHERE " & KEY_NAME & " = " & SQ(GAKKOU_CODE))
            End With

            INS_CNT = db.ExecuteNonQuery(SQL)
            If INS_CNT = -1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)終了", "失敗", SYORI_NAME & "処理失敗 学校コード：" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)終了", "失敗", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        ResultMsg = String.Format("{0} ワークレコード削除件数={1}件 退避件数={2}件", TARGET_TABLE, DEL_CNT.ToString, INS_CNT.ToString)
        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_MASTtoSINQBK)終了", "成功", ResultMsg)
        Return True
    End Function
    '2017/04/19 タスク）西野 ADD 標準版修正（進級戻し処理対応）------------------------------------ END

#End Region

    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '2010/11/09 学校コードゼロ埋め
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            Select Case (Trim(txtGAKKOU_CODE.Text))
                Case "9".PadLeft(10, "9"c)
                    lab学校名.Text = "すべての学校が対象です"
                Case Else
                    If PFUNC_GAKNAME_GET() = False Then
                        Exit Sub
                    End If
            End Select
        End If

    End Sub

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校検索
        Call GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName)

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '学校検索後の学校コード設定
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '学校名の取得
        lab学校名.Text = cmbGakkouName.Text

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub

    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
