Imports System
Imports System.Text
Imports CASTCommon

''' <summary>
''' 振込手数料マスタメンテナンス画面
''' </summary>
''' <remarks></remarks>
Public Class KFJMAST090

#Region "クラス定数"
    Private Const msgTitle As String = "振込手数料マスタメンテナンス画面(KFJMAST090)"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx02 As New CASTCommon.Events("1-9", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx2 As New CASTCommon.Events("", CASTCommon.Events.KeyMode.BAD)

#End Region

#Region "クラス変数"
    Private MainLOG As New CASTCommon.BatchLOG("KFJMAST090", "振込手数料マスタメンテナンス画面")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle

    '2014/01/15 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
    Private TAX As CASTCommon.ClsTAX
    Private strSysDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    '2014/01/15 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFJMAST090_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            '手数料検索コンボボックスの設定
            '------------------------------------------------
            If GCom.SelectTesuuName(Me.cmbTesuuName, Me.txtTaxID, Me.txtTesuuID) = -1 Then
                MessageBox.Show(MSG0367W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            '------------------------------------------------
            '画面コントロールの設定
            '------------------------------------------------
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True

            '2014/01/15 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
            '画面表示用の印紙税区分を設定する
            Me.TAX = New CASTCommon.ClsTAX
            Me.TAX.GetInshizei(strSysDate)          '画面と帳票で印紙税の区分を統一するため、システム日付で印紙税取得
            If Me.TAX.INSHIZEI_ID.Equals("err") = True Then
                '印紙税が設定されていない
                Me.Label11.Text = "1万円未満"
                Me.Label20.Text = "1万円以上3万円未満"
                Me.Label19.Text = "3万円以上"
            Else
                '金額によって表示形式を変える
                Dim strInshizei1 As String
                Dim strInshizei2 As String
                If Me.TAX.INSHIZEI1 >= 10000 Then
                    strInshizei1 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI1 / 10000) & "万"
                ElseIf Me.TAX.INSHIZEI1 >= 1000 Then
                    strInshizei1 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI1 / 1000) & "千"
                Else
                    strInshizei1 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI1)
                End If

                If Me.TAX.INSHIZEI2 >= 10000 Then
                    strInshizei2 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI2 / 10000) & "万"
                ElseIf Me.TAX.INSHIZEI1 >= 1000 Then
                    strInshizei2 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI2 / 1000) & "千"
                Else
                    strInshizei2 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI2)
                End If

                Me.Label11.Text = strInshizei1 & "円未満"
                Me.Label20.Text = strInshizei1 & "円以上" & strInshizei2 & "円未満"
                Me.Label19.Text = strInshizei2 & "円以上"
            End If
            '2014/01/15 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<

            Me.ActiveControl = Me.txtTaxID

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 登録ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")

            '------------------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------------------
            If Me.txtTaxID.Text.Trim = String.Empty Then
                '税率IDは必須項目
                MessageBox.Show(String.Format(MSG0285W, "税率ID"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTaxID.Focus()
                Return
            End If
            If Me.txtTesuuID.Text.Trim = String.Empty Then
                '振込手数料基準IDは必須項目
                MessageBox.Show(String.Format(MSG0285W, "振込手数料基準ID"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuID.Focus()
                Return
            End If
            If Me.CheckText() = False Then
                Return
            End If

            '------------------------------------------------
            '消費税マスタチェック
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(Me.CreateGetTaxmastSQL(Me.txtTaxID.Text.Trim)) = True Then
            Else
                '消費税マスタに設定されていない
                MessageBox.Show(MSG0364W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTaxID.Focus()
                Return
            End If

            '------------------------------------------------
            '振込手数料マスタチェック
            '------------------------------------------------
            If oraReader.DataReader(Me.CreateGetTesuumastSQL(Me.txtTesuuID.Text.Trim, Me.txtTaxID.Text.Trim)) = True Then
                '既に設定済み
                MessageBox.Show(MSG0361W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuID.Focus()
                Return
            End If

            '------------------------------------------------
            '確認メッセージ
            '------------------------------------------------
            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '------------------------------------------------
            '登録処理
            '------------------------------------------------
            Dim SQL As New StringBuilder
            With SQL
                .Length = 0
                .Append("insert into TESUUMAST")
                .Append(" (FSYORI_KBN_C, TAX_ID_C, TESUU_TABLE_ID_C, TESUU_TABLE_NAME_C, SYUBETU_C, TESUU_A1_C, TESUU_A2_C, TESUU_A3_C, TESUU_B1_C, TESUU_B2_C, TESUU_B3_C, TESUU_C1_C, TESUU_C2_C, TESUU_C3_C)")
                .Append(" values (")
                .Append(" '1'")
                .Append("," & SQ(Me.txtTaxID.Text.Trim))
                .Append("," & SQ(Me.txtTesuuID.Text.Trim))
                .Append("," & SQ(Me.txtTesuuName.Text.Trim))
                .Append(",'91'")
                .Append("," & Me.DeleteComma(Me.txtTesuuA1F.Text))
                .Append("," & Me.DeleteComma(Me.txtTesuuA2F.Text))
                .Append("," & Me.DeleteComma(Me.txtTesuuA3F.Text))
                .Append("," & Me.DeleteComma(Me.txtTesuuB1F.Text))
                .Append("," & Me.DeleteComma(Me.txtTesuuB2F.Text))
                .Append("," & Me.DeleteComma(Me.txtTesuuB3F.Text))
                .Append("," & Me.DeleteComma(Me.txtTesuuC1F.Text))
                .Append("," & Me.DeleteComma(Me.txtTesuuC2F.Text))
                .Append("," & Me.DeleteComma(Me.txtTesuuC3F.Text))
                .Append(")")

            End With

            If MainDB.ExecuteNonQuery(SQL) <> 1 Then
                MainLOG.Write("振込手数料マスタ挿入", "失敗", MainDB.Message)
                MessageBox.Show(String.Format(MSG0002E, "挿入"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            MainDB.Commit()
            MainDB.Close()
            MainDB = Nothing

            MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '------------------------------------------------
            '画面初期化
            '------------------------------------------------
            Me.ClearText()
            Me.txtTaxID.Enabled = True
            Me.txtTesuuID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True
            Me.cmbTesuuName.Enabled = True

            '------------------------------------------------
            '手数料検索コンボボックスの設定
            '------------------------------------------------
            If GCom.SelectTesuuName(Me.cmbTesuuName, Me.txtTaxID, Me.txtTesuuID) = -1 Then
                MessageBox.Show(MSG0367W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            Me.txtTaxID.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If

            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")

        End Try
    End Sub

    ''' <summary>
    ''' 更新ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")

            '------------------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------------------
            If Me.CheckText() = False Then
                Return
            End If

            '------------------------------------------------
            '確認メッセージ
            '------------------------------------------------
            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '------------------------------------------------
            '更新処理
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            Dim SQL As New StringBuilder
            With SQL
                .Length = 0
                .Append("update TESUUMAST set")
                .Append(" TESUU_TABLE_NAME_C = " & SQ(Me.txtTesuuName.Text.Trim))
                .Append(",TESUU_A1_C = " & Me.DeleteComma(Me.txtTesuuA1F.Text))
                .Append(",TESUU_A2_C = " & Me.DeleteComma(Me.txtTesuuA2F.Text))
                .Append(",TESUU_A3_C = " & Me.DeleteComma(Me.txtTesuuA3F.Text))
                .Append(",TESUU_B1_C = " & Me.DeleteComma(Me.txtTesuuB1F.Text))
                .Append(",TESUU_B2_C = " & Me.DeleteComma(Me.txtTesuuB2F.Text))
                .Append(",TESUU_B3_C = " & Me.DeleteComma(Me.txtTesuuB3F.Text))
                .Append(",TESUU_C1_C = " & Me.DeleteComma(Me.txtTesuuC1F.Text))
                .Append(",TESUU_C2_C = " & Me.DeleteComma(Me.txtTesuuC2F.Text))
                .Append(",TESUU_C3_C = " & Me.DeleteComma(Me.txtTesuuC3F.Text))
                .Append(" where FSYORI_KBN_C = '1'")
                .Append(" and SYUBETU_C = '91'")
                .Append(" and TESUU_TABLE_ID_C = " & SQ(Me.txtTesuuID.Text.Trim))
                .Append(" and TAX_ID_C = " & SQ(Me.txtTaxID.Text.Trim))
            End With

            Dim iRet As Integer
            iRet = MainDB.ExecuteNonQuery(SQL)
            If iRet <> 1 Then
                MainLOG.Write("振込手数料マスタ更新", "失敗", MainDB.Message)
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            MainDB.Commit()
            MainDB.Close()
            MainDB = Nothing

            MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '------------------------------------------------
            '画面初期化
            '------------------------------------------------
            Me.ClearText()
            Me.txtTaxID.Enabled = True
            Me.txtTesuuID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True
            Me.cmbTesuuName.Enabled = True

            '------------------------------------------------
            '手数料検索コンボボックスの設定
            '------------------------------------------------
            If GCom.SelectTesuuName(Me.cmbTesuuName, Me.txtTaxID, Me.txtTesuuID) = -1 Then
                MessageBox.Show(MSG0367W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            Me.txtTaxID.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 削除ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)開始", "成功", "")

            '------------------------------------------------
            '振込手数料マスタチェック
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(CreateGetTesuumastSQL(Me.txtTesuuID.Text.Trim, Me.txtTaxID.Text.Trim)) = False Then
                '振込手数料設定なし
                MessageBox.Show(String.Format(MSG0258W, "振込手数料マスタ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTaxID.Focus()
                Return
            End If

            '------------------------------------------------
            '確認メッセージ
            '------------------------------------------------
            If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '------------------------------------------------
            '削除処理
            '------------------------------------------------
            Dim SQL As New StringBuilder
            With SQL
                .Append("delete from TESUUMAST")
                .Append(" where FSYORI_KBN_C = '1'")
                .Append(" and TESUU_TABLE_ID_C = " & SQ(Me.txtTesuuID.Text.Trim))
                .Append(" and TAX_ID_C = " & SQ(Me.txtTaxID.Text.Trim))
            End With
            Dim iRet As Integer
            iRet = MainDB.ExecuteNonQuery(SQL)
            '自振の場合は必ず1レコード削除
            If iRet <> 1 Then
                MainLOG.Write("振込手数料マスタ削除", "失敗", MainDB.Message)
                MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            MainDB.Commit()
            MainDB.Close()
            MainDB = Nothing

            MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '------------------------------------------------
            '画面初期化
            '------------------------------------------------
            Me.ClearText()
            Me.txtTesuuID.Enabled = True
            Me.txtTaxID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True
            Me.cmbTesuuName.Enabled = True

            '------------------------------------------------
            '手数料検索コンボボックスの設定
            '------------------------------------------------
            If GCom.SelectTesuuName(Me.cmbTesuuName, Me.txtTaxID, Me.txtTesuuID) = -1 Then
                MessageBox.Show(MSG0367W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            Me.txtTaxID.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 参照ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelect.Click

        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            '------------------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------------------
            If Me.txtTaxID.Text.Trim = String.Empty Then
                '税率IDは必須項目
                MessageBox.Show(String.Format(MSG0285W, "税率ID"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTaxID.Focus()
                Return
            End If
            If Me.txtTesuuID.Text.Trim = String.Empty Then
                '振込手数料基準IDは必須項目
                MessageBox.Show(String.Format(MSG0285W, "振込手数料基準ID"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuID.Focus()
                Return
            End If

            '------------------------------------------------
            '振込手数料マスタチェック
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(CreateGetTesuumastSQL(Me.txtTesuuID.Text.Trim, Me.txtTaxID.Text.Trim)) = True Then
                Me.txtTesuuName.Text = oraReader.GetString("TESUU_TABLE_NAME_C")
                Me.txtTesuuA1F.Text = Me.AddComma(oraReader.GetString("TESUU_A1_C"))
                Me.txtTesuuA2F.Text = Me.AddComma(oraReader.GetString("TESUU_A2_C"))
                Me.txtTesuuA3F.Text = Me.AddComma(oraReader.GetString("TESUU_A3_C"))
                Me.txtTesuuB1F.Text = Me.AddComma(oraReader.GetString("TESUU_B1_C"))
                Me.txtTesuuB2F.Text = Me.AddComma(oraReader.GetString("TESUU_B2_C"))
                Me.txtTesuuB3F.Text = Me.AddComma(oraReader.GetString("TESUU_B3_C"))
                Me.txtTesuuC1F.Text = Me.AddComma(oraReader.GetString("TESUU_C1_C"))
                Me.txtTesuuC2F.Text = Me.AddComma(oraReader.GetString("TESUU_C2_C"))
                Me.txtTesuuC3F.Text = Me.AddComma(oraReader.GetString("TESUU_C3_C"))
            Else
                '振込手数料設定なし
                MessageBox.Show(MSG0362W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuID.Focus()
                Return
            End If

            '------------------------------------------------
            '画面コントロールの設定
            '------------------------------------------------
            Me.txtTesuuID.Enabled = False
            Me.txtTaxID.Enabled = False
            Me.btnAction.Enabled = False
            Me.btnUpdate.Enabled = True
            Me.btnDelete.Enabled = True
            Me.btnSelect.Enabled = False
            Me.cmbTesuuName.Enabled = False

            Me.txtTesuuA1F.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 取消ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")

            '------------------------------------------------
            '入力内容クリア
            '------------------------------------------------
            Me.ClearText()

            '------------------------------------------------
            '画面コントロールの設定
            '------------------------------------------------
            Me.txtTesuuID.Enabled = True
            Me.txtTaxID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True
            Me.cmbTesuuName.Enabled = True

            Me.txtTaxID.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 印刷ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click

        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            '------------------------------------------------
            '印刷データの存在チェック
            '------------------------------------------------
            Dim SQL As New StringBuilder
            With SQL
                .Append("select count(*) as COUNTER ")
                .Append(" from TESUUMAST, TAXMAST")
                .Append(" where TAX_ID_C = TAX_ID_Z")
                .Append(" and FSYORI_KBN_C = '1'")          '自振の振込手数料が設定されているか
            End With

            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(SQL) = True Then
                If oraReader.GetInt("COUNTER") > 0 Then
                    '印刷対象あり
                Else
                    '印刷対象なし
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtTaxID.Focus()
                    Return
                End If
            Else
                '異常終了
                MessageBox.Show(String.Format(MSG0002E, "参照"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Me.txtTaxID.Focus()
                Return
            End If

            oraReader.Close()
            oraReader = Nothing
            MainDB.Close()
            MainDB = Nothing

            If MessageBox.Show(String.Format(MSG0013I, "振込手数料マスタ登録リスト"), _
                   msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '------------------------------------------------
            '印刷バッチ呼び出し
            '------------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me
            '2014/01/15 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
            'パラメータに画面を開いた時のシステム日付を追加
            Dim param As String = GCom.GetUserID & "," & "1" & "," & Me.strSysDate
            'Dim param As String = GCom.GetUserID & "," & "1"
            '2014/01/15 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
            Dim nRet As Integer = ExeRepo.ExecReport("KFJP056.EXE", param)

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, "振込手数料マスタ登録リスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "振込手数料マスタ登録リスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 終了ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' テキストボックスバリデイティングイベント（ゼロパディング）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtTesuuID.Validating, txtTaxID.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' テキストボックスバリデイテッドイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtTaxID_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles txtTaxID.Validated

        Me.lblTax.Text = String.Empty

        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL.Append("select * from TAXMAST")
            SQL.Append(" where TAX_ID_Z = " & SQ(Me.txtTaxID.Text.Trim))

            If oraReader.DataReader(SQL) = True Then
                Me.lblTax.Text = oraReader.GetString("TAX_Z")
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(税率情報設定)", "失敗", ex.ToString)
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try
    End Sub

    ''' <summary>
    ''' テキストボックスバリデイティングイベント（金額テキスト）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Money_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtTesuuA1F.Validating, txtTesuuA2F.Validating, txtTesuuA3F.Validating, _
        txtTesuuB1F.Validating, txtTesuuB2F.Validating, txtTesuuB3F.Validating, _
        txtTesuuC1F.Validating, txtTesuuC2F.Validating, txtTesuuC3F.Validating
        Try
            With CType(sender, TextBox)
                .Text = String.Format("{0:#,##0}", GCom.NzDec(.Text, 1))
            End With
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("金額表示変数", "失敗", ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' 手数料検索コンボボックスセレクテッドインデックスチェンジドイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbTesuuName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles cmbTesuuName.SelectedIndexChanged
        Try
            If Not Me.cmbTesuuName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(Me.cmbTesuuName.SelectedItem.ToString) Then
                GCom.Set_TESUU_CODE(Me.cmbTesuuName, Me.txtTaxID, Me.txtTesuuID)
                Me.txtTaxID_Validated(sender, e)
                Me.btnSelect.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(手数料コード取得)", "失敗", ex.ToString)
        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 登録・更新時のテキストボックスの入力チェックを行います。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function CheckText() As Boolean
        Try

            ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
            '振込手数料ＩＤチェック
            If CInt(Me.txtTesuuID.Text.Trim) = 0 Then
                MessageBox.Show(String.Format(MSG0312W, "振込手数料ＩＤに「00」"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuName.Focus()
                Return False
            End If
            ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

            '振込手数料名称チェック
            If Me.txtTesuuName.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "振込手数料名称"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuName.Focus()
                Return False
            End If

            '振込手数料A1チェック
            If Me.txtTesuuA1F.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "振込手数料"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuA1F.Focus()
                Return False
            End If

            '振込手数料A2チェック
            If Me.txtTesuuA2F.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "振込手数料"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuA2F.Focus()
                Return False
            End If

            '振込手数料A3チェック
            If Me.txtTesuuA3F.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "振込手数料"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuA3F.Focus()
                Return False
            End If

            '振込手数料B1チェック
            If Me.txtTesuuB1F.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "振込手数料"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuB1F.Focus()
                Return False
            End If

            '振込手数料B2チェック
            If Me.txtTesuuB2F.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "振込手数料"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuB2F.Focus()
                Return False
            End If

            '振込手数料B3チェック
            If Me.txtTesuuB3F.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "振込手数料"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuB3F.Focus()
                Return False
            End If

            '振込手数料C1チェック
            If Me.txtTesuuC1F.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "振込手数料"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuC1F.Focus()
                Return False
            End If

            '振込手数料C2チェック
            If Me.txtTesuuC2F.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "振込手数料"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuC2F.Focus()
                Return False
            End If

            '振込手数料C3チェック
            If Me.txtTesuuC3F.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "振込手数料"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTesuuC3F.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(テキストボックスチェック)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 振込手数料マスタから手数料情報を取得するSQLを作成します。
    ''' </summary>
    ''' <param name="strTesuuID">振込手数料基準ID</param>
    ''' <param name="strTaxID">税率ID</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetTesuumastSQL(ByVal strTesuuID As String, _
                                           ByVal strTaxID As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select * from TESUUMAST")
            .Append(" where TESUU_TABLE_ID_C = " & SQ(strTesuuID))
            .Append(" and TAX_ID_C = " & SQ(strTaxID))
            .Append(" and FSYORI_KBN_C = '1'")
        End With

        Return SQL
    End Function

    ''' <summary>
    ''' 消費税マスタから消費税情報を取得するSQLを作成します。
    ''' </summary>
    ''' <param name="strTaxID">消費税ID</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetTaxmastSQL(ByVal strTaxID As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select * from TAXMAST")
            .Append(" where TAX_ID_Z = " & SQ(strTaxID))
        End With

        Return SQL
    End Function

    ''' <summary>
    ''' テキストボックスをクリアします。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ClearText()
        Try
            Me.txtTesuuID.Text = String.Empty
            Me.txtTaxID.Text = String.Empty
            Me.lblTax.Text = String.Empty
            Me.txtTesuuName.Text = String.Empty
            Me.txtTesuuA1F.Text = "0"
            Me.txtTesuuA2F.Text = "0"
            Me.txtTesuuA3F.Text = "0"
            Me.txtTesuuB1F.Text = "0"
            Me.txtTesuuB2F.Text = "0"
            Me.txtTesuuB3F.Text = "0"
            Me.txtTesuuC1F.Text = "0"
            Me.txtTesuuC2F.Text = "0"
            Me.txtTesuuC3F.Text = "0"
            Me.cmbTesuuName.SelectedIndex = 0

        Catch ex As Exception
            MainLOG.Write("(テキストボックスクリア)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' 金額表示項目のカンマを削除します。
    ''' </summary>
    ''' <param name="strCommaText"></param>
    ''' <returns>カンマ削除後の文字列</returns>
    ''' <remarks></remarks>
    Private Function DeleteComma(ByVal strCommaText As String) As String
        Dim strRet As String = String.Empty
        strRet = strCommaText.Trim.Replace(","c, "")
        Return strRet
    End Function

    ''' <summary>
    ''' 金額表示項目にカンマを付加します。
    ''' </summary>
    ''' <param name="strCommaText"></param>
    ''' <returns>カンマ付加後の文字列</returns>
    ''' <remarks></remarks>
    Private Function AddComma(ByVal strCommaText As String) As String
        Dim strRet As String = String.Empty
        If IsNumeric(strCommaText) = True Then
            strRet = CLng(strCommaText).ToString("###,##0")
        Else
            strRet = "0"
        End If
        Return strRet
    End Function

#End Region

End Class