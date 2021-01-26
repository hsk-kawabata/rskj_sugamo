Imports System
Imports System.Text
Imports CASTCommon

''' <summary>
''' 消費税マスタメンテナンス画面
''' </summary>
''' <remarks></remarks>
Public Class KFUMAST060

#Region "クラス定数"
    Private Const msgTitle As String = "消費税マスタメンテナンス画面(KFUMAST060)"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx02 As New CASTCommon.Events("0-9.", CASTCommon.Events.KeyMode.GOOD)
#End Region

#Region "クラス変数"
    Private MainLOG As New CASTCommon.BatchLOG("KFUMAST060", "消費税マスタメンテナンス画面")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFUMAST060_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            '画面コントロールの設定
            '------------------------------------------------
            Me.CreateTaxList()
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True

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
        Dim SQL As New StringBuilder

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")

            '------------------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------------------
            If Me.txtTaxID.Text.Trim = String.Empty Then
                'IDは必須項目
                MessageBox.Show(String.Format(MSG0285W, "税率ID"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTaxID.Focus()
                Return
            End If
            If Me.CheckText() = False Then
                Return
            End If

            Dim strKaishiDate As String = String.Concat(New String() {Me.txtKaishiDateY.Text, Me.txtKaishiDateM.Text, Me.txtKaishiDateD.Text})
            Dim strSyuryoDate As String = String.Concat(New String() {Me.txtSyuryoDateY.Text, Me.txtSyuryoDateM.Text, Me.txtSyuryoDateD.Text})

            '------------------------------------------------
            '消費税マスタチェック
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(Me.CreateGetTaxmastSQL(Me.txtTaxID.Text.Trim)) = True Then
                '既に設定済み
                MessageBox.Show(MSG0363W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTaxID.Focus()
                Return
            End If
            oraReader.Close()

            '適用期間チェック
            If Me.CheckTekiyoDate(strKaishiDate, strSyuryoDate) = False Then
                MessageBox.Show(MSG0366W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
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
            With SQL
                .Length = 0
                .Append("insert into TAXMAST")
                .Append(" (TAX_ID_Z, TAX_Z, START_DATE_Z, END_DATE_Z)")
                .Append(" values (")
                .Append(" " & SQ(Me.txtTaxID.Text.Trim))
                .Append("," & SQ(Me.txtTax.Text.Trim))
                .Append("," & SQ(strKaishiDate))
                .Append("," & SQ(strSyuryoDate))
                .Append(")")
            End With

            If MainDB.ExecuteNonQuery(SQL) <> 1 Then
                MainLOG.Write("消費税マスタ挿入", "失敗", MainDB.Message)
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
            Me.CreateTaxList()
            Me.txtTaxID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True
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

            Dim strKaishiDate As String = String.Concat(New String() {Me.txtKaishiDateY.Text, Me.txtKaishiDateM.Text, Me.txtKaishiDateD.Text})
            Dim strSyuryoDate As String = String.Concat(New String() {Me.txtSyuryoDateY.Text, Me.txtSyuryoDateM.Text, Me.txtSyuryoDateD.Text})

            '------------------------------------------------
            '消費税マスタチェック
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            If Me.CheckTekiyoDate(strKaishiDate, strSyuryoDate, Me.txtTaxID.Text.Trim) = False Then
                MessageBox.Show(MSG0366W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
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
            Dim SQL As New StringBuilder
            With SQL
                .Length = 0
                .Append("update TAXMAST set")
                .Append(" TAX_Z = " & SQ(Me.txtTax.Text.Trim))
                .Append(",START_DATE_Z = " & SQ(strKaishiDate))
                .Append(",END_DATE_Z = " & SQ(strSyuryoDate))
                .Append(" where TAX_ID_Z = " & SQ(Me.txtTaxID.Text.Trim))
            End With

            Dim iRet As Integer
            iRet = MainDB.ExecuteNonQuery(SQL)
            If iRet <> 1 Then
                MainLOG.Write("消費税マスタ更新", "失敗", MainDB.Message)
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
            Me.CreateTaxList()
            Me.txtTaxID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True
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
            '消費税マスタチェック
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(Me.CreateGetTaxmastSQL(Me.txtTaxID.Text.Trim)) = False Then
                '消費税設定なし
                MessageBox.Show(String.Format(MSG0258W, "消費税マスタ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
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
                .Append("delete from TAXMAST where TAX_ID_Z = " & SQ(Me.txtTaxID.Text.Trim))
            End With
            Dim iRet As Integer
            iRet = MainDB.ExecuteNonQuery(SQL)
            '必ず1レコード削除
            If iRet <> 1 Then
                MainLOG.Write("消費税マスタ削除", "失敗", MainDB.Message)
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
            Me.CreateTaxList()
            Me.txtTaxID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True
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

            '------------------------------------------------
            '消費税マスタチェック
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(Me.CreateGetTaxmastSQL(Me.txtTaxID.Text.Trim)) = True Then
                While oraReader.EOF = False
                    Me.txtTax.Text = oraReader.GetString("TAX_Z")
                    Me.txtKaishiDateY.Text = oraReader.GetString("START_DATE_Z").Substring(0, 4)
                    Me.txtKaishiDateM.Text = oraReader.GetString("START_DATE_Z").Substring(4, 2)
                    Me.txtKaishiDateD.Text = oraReader.GetString("START_DATE_Z").Substring(6, 2)
                    Me.txtSyuryoDateY.Text = oraReader.GetString("END_DATE_Z").Substring(0, 4)
                    Me.txtSyuryoDateM.Text = oraReader.GetString("END_DATE_Z").Substring(4, 2)
                    Me.txtSyuryoDateD.Text = oraReader.GetString("END_DATE_Z").Substring(6, 2)
                    oraReader.NextRead()
                End While
            Else
                '税率設定なし
                MessageBox.Show(MSG0364W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTaxID.Focus()
                Return
            End If

            '------------------------------------------------
            '画面コントロールの設定
            '------------------------------------------------
            Me.txtTaxID.Enabled = False
            Me.btnAction.Enabled = False
            Me.btnUpdate.Enabled = True
            Me.btnDelete.Enabled = True
            Me.btnSelect.Enabled = False

            Me.txtTax.Focus()

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
            Me.CreateTaxList()

            '------------------------------------------------
            '画面コントロールの設定
            '------------------------------------------------
            Me.txtTaxID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True

            Me.txtTaxID.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
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
            '--------------------------------------------------
            '登録されている適用期間の連続性チェックを行う
            '--------------------------------------------------
            If Me.CheckTekiyoDateSeries() = False Then
                Return
            End If

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
        Handles txtTaxID.Validating, txtKaishiDateY.Validating, txtKaishiDateM.Validating, txtKaishiDateD.Validating, txtSyuryoDateY.Validating, txtSyuryoDateM.Validating, txtSyuryoDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(ゼロパディング)", "失敗", ex.ToString)
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
            '税率チェック
            If Me.txtTax.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "税率"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTax.Focus()
                Return False
            Else
                '数値チェック
                If IsNumeric(Me.txtTax.Text.Trim) = False Then
                    MessageBox.Show(MSG0365W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtTax.Focus()
                    Return False
                Else
                    If CDec(Me.txtTax.Text.Trim) < 1.0 OrElse CDec(Me.txtTax.Text.Trim) > 1.99 Then
                        MessageBox.Show(MSG0369W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtTax.Focus()
                        Return False
                    End If
                End If
            End If

            '適用開始日(年)チェック
            If Me.txtKaishiDateY.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKaishiDateY.Focus()
                Return False
            End If

            '適用開始日(月)チェック
            If Me.txtKaishiDateM.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKaishiDateM.Focus()
                Return False
            Else
                If GCom.NzInt(Me.txtKaishiDateM.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtKaishiDateM.Text.Trim) > 12 Then
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtKaishiDateM.Focus()
                    Return False
                End If
            End If

            '適用開始日(日)チェック
            If Me.txtKaishiDateD.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKaishiDateD.Focus()
                Return False
            Else
                If GCom.NzInt(Me.txtKaishiDateD.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtKaishiDateD.Text.Trim) > 31 Then
                    MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtKaishiDateD.Focus()
                    Return False
                End If
            End If

            '日付整合性チェック
            Dim WORK_DATE_1 As String = Me.txtKaishiDateY.Text & "/" & Me.txtKaishiDateM.Text & "/" & Me.txtKaishiDateD.Text
            If Information.IsDate(WORK_DATE_1) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKaishiDateY.Focus()
                Return False
            End If

            '適用終了日(年)チェック
            If Me.txtSyuryoDateY.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyuryoDateY.Focus()
                Return False
            End If

            '適用終了日(月)チェック
            If Me.txtSyuryoDateM.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyuryoDateM.Focus()
                Return False
            Else
                If GCom.NzInt(Me.txtSyuryoDateM.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtSyuryoDateM.Text.Trim) > 12 Then
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtSyuryoDateM.Focus()
                    Return False
                End If
            End If

            '適用終了日(日)チェック
            If Me.txtSyuryoDateD.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyuryoDateD.Focus()
                Return False
            Else
                If GCom.NzInt(Me.txtSyuryoDateD.Text.Trim) < 1 OrElse GCom.NzInt(Me.txtSyuryoDateD.Text.Trim) > 31 Then
                    MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtSyuryoDateD.Focus()
                    Return False
                End If
            End If

            '日付整合性チェック
            Dim WORK_DATE_2 As String = Me.txtSyuryoDateY.Text & "/" & Me.txtSyuryoDateM.Text & "/" & Me.txtSyuryoDateD.Text
            If Information.IsDate(WORK_DATE_2) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSyuryoDateY.Focus()
                Return False
            End If

            '範囲チェック
            Dim intStartDate As Integer = GCom.NzInt(String.Concat(New String() {Me.txtKaishiDateY.Text, Me.txtKaishiDateM.Text, Me.txtKaishiDateD.Text}))
            Dim intEndDate As Integer = GCom.NzInt(String.Concat(New String() {Me.txtSyuryoDateY.Text, Me.txtSyuryoDateM.Text, Me.txtSyuryoDateD.Text}))
            If intStartDate >= intEndDate Then
                MessageBox.Show(MSG0099W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKaishiDateY.Focus()
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
    ''' 消費税マスタから消費税情報を取得するSQLを作成します。
    ''' </summary>
    ''' <param name="strTaxID">税率ID</param>
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
            Me.txtTaxID.Text = String.Empty
            Me.txtTax.Text = String.Empty
            Me.txtKaishiDateY.Text = String.Empty
            Me.txtKaishiDateM.Text = String.Empty
            Me.txtKaishiDateD.Text = String.Empty
            Me.txtSyuryoDateY.Text = String.Empty
            Me.txtSyuryoDateM.Text = String.Empty
            Me.txtSyuryoDateD.Text = String.Empty

        Catch ex As Exception
            MainLOG.Write("(テキストボックスクリア)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' 消費税情報設定用リストを作成します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateTaxList()
        MainDB = New CASTCommon.MyOracle
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder

        Try
            With Me.ListView1
                .Clear()
                .Columns.Add("ＩＤ", 95, HorizontalAlignment.Center)
                .Columns.Add("税率", 105, HorizontalAlignment.Center)
                .Columns.Add("適用開始日", 105, HorizontalAlignment.Center)
                .Columns.Add("適用終了日", 105, HorizontalAlignment.Center)
            End With

            With SQL
                .Length = 0
                .Append("select * from TAXMAST")
                .Append(" order by TAX_ID_Z")
            End With

            If oraReader.DataReader(SQL) = True Then

                Dim ROW As Integer = 0

                While oraReader.EOF = False
                    Dim Data(3) As String

                    Data(0) = oraReader.GetString("TAX_ID_Z")
                    Data(1) = oraReader.GetString("TAX_Z")
                    Data(2) = oraReader.GetString("START_DATE_Z").Substring(0, 4) & "/" & oraReader.GetString("START_DATE_Z").Substring(4, 2) & "/" & oraReader.GetString("START_DATE_Z").Substring(6, 2)
                    Data(3) = oraReader.GetString("END_DATE_Z").Substring(0, 4) & "/" & oraReader.GetString("END_DATE_Z").Substring(4, 2) & "/" & oraReader.GetString("END_DATE_Z").Substring(6, 2)

                    Dim LineColor As Color
                    If ROW Mod 2 = 0 Then
                        LineColor = Color.White
                    Else
                        LineColor = Color.WhiteSmoke
                    End If

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    ROW += 1

                    oraReader.NextRead()
                End While
            End If

        Catch ex As Exception
            Throw
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
    ''' 適用期間のチェックをします。
    ''' </summary>
    ''' <param name="strStartDate">適用開始日</param>
    ''' <param name="strEndDate">適用終了日</param>
    ''' <param name="strTaxID">税率ID（更新時）</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function CheckTekiyoDate(ByVal strStartDate As String, _
                                     ByVal strEndDate As String, _
                                     Optional ByVal strTaxID As String = "") As Boolean

        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder

        Try
            '------------------------------------------------------------
            '適用期間が他の税率の適用期間と被っていないかチェック
            '------------------------------------------------------------
            '2014/02/07 saitou 標準版 消費税対応 MODIFY ----------------------------------------------->>>>
            '適用期間のチェックまとめ
            '①開始日が他の適用期間内
            '②終了日が他の適用期間内
            '③開始～終了が他の適用期間内
            '④開始～終了が他の適用期間を含む
            For i As Integer = 1 To 4
                Select Case i
                    Case 1
                        With SQL
                            .Length = 0
                            .Append("select * from TAXMAST")
                            .Append(" where START_DATE_Z between " & SQ(strStartDate) & " and " & SQ(strEndDate))
                            If strTaxID <> String.Empty Then
                                .Append(" and TAX_ID_Z <> " & SQ(strTaxID))
                            End If
                        End With

                    Case 2
                        With SQL
                            .Length = 0
                            .Append("select * from TAXMAST")
                            .Append(" where END_DATE_Z between " & SQ(strStartDate) & " and " & SQ(strEndDate))
                            If strTaxID <> String.Empty Then
                                .Append(" and TAX_ID_Z <> " & SQ(strTaxID))
                            End If
                        End With

                    Case 3
                        With SQL
                            .Length = 0
                            .Append("select * from TAXMAST")
                            .Append(" where START_DATE_Z <= " & SQ(strStartDate))
                            .Append(" and END_DATE_Z >= " & SQ(strEndDate))
                            If strTaxID <> String.Empty Then
                                .Append(" and TAX_ID_Z <> " & SQ(strTaxID))
                            End If
                        End With

                    Case 4
                        With SQL
                            .Length = 0
                            .Append("select * from TAXMAST")
                            .Append(" where " & SQ(strStartDate) & " <= START_DATE_Z")
                            .Append(" and END_DATE_Z <= " & SQ(strEndDate))
                            If strTaxID <> String.Empty Then
                                .Append(" and TAX_ID_Z <> " & SQ(strTaxID))
                            End If
                        End With
                End Select

                If oraReader.DataReader(SQL) = True Then
                    '適用期間被りあり
                    Me.txtKaishiDateY.Focus()
                    Return False
                End If

                oraReader.Close()
            Next
            'With SQL
            '    .Length = 0
            '    .Append("select * from TAXMAST")
            '    .Append(" where (( START_DATE_Z between " & SQ(strStartDate) & " and " & SQ(strEndDate) & ")")
            '    .Append(" or ( END_DATE_Z between " & SQ(strStartDate) & " and " & SQ(strEndDate) & "))")
            '    If strTaxID <> String.Empty Then
            '        .Append(" and TAX_ID_Z <> " & SQ(strTaxID))
            '    End If
            'End With

            'If oraReader.DataReader(SQL) = True Then
            '    '適用期間被りあり
            '    Me.txtKaishiDateY.Focus()
            '    Return False
            'End If
            'oraReader.Close()
            '2014/02/07 saitou 標準版 消費税対応 MODIFY -----------------------------------------------<<<<

            Return True

        Catch ex As Exception
            MainLOG.Write("適用期間チェック", "失敗", ex.Message)
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
    ''' 適用期間の連続性チェックを行います。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function CheckTekiyoDateSeries() As Boolean
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            '消費税マスタを適用開始日の昇順で並べる
            With SQL
                .Append("select * from TAXMAST")
                .Append(" order by START_DATE_Z")
            End With

            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim strNowLoopStartDate As String = String.Empty
            Dim strNowLoopEndDate As String = String.Empty
            Dim strBfrLoopStartDate As String = String.Empty
            Dim strBfrLoopEndDate As String = String.Empty
            Dim intCounter As Integer = 0
            '----------------------------------------------------------------------
            '前回ループの適用終了日と今回ループの適用開始日が連続であるチェック
            '----------------------------------------------------------------------
            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    If intCounter = 0 Then
                        '初回ループは何もしない

                    Else
                        strNowLoopStartDate = oraReader.GetString("START_DATE_Z")
                        strNowLoopEndDate = oraReader.GetString("END_DATE_Z")

                        '前回ループの適用終了日に1日足したものが今回ループの適用開始日であるかチェック
                        Dim dtBfrLoopEndDate As Date = CASTCommon.ConvertDate(strBfrLoopEndDate)
                        Dim strCheckEndDate As String = dtBfrLoopEndDate.AddDays(1).ToString("yyyyMMdd")
                        If strNowLoopStartDate.Equals(strCheckEndDate) = True Then
                            'OK
                        Else
                            'NG
                            Dim strMsgWord1 As String = strBfrLoopEndDate.Substring(0, 4) & "/" & strBfrLoopEndDate.Substring(4, 2) & "/" & strBfrLoopEndDate.Substring(6, 2)
                            Dim strMsgWord2 As String = strNowLoopStartDate.Substring(0, 4) & "/" & strNowLoopStartDate.Substring(4, 2) & "/" & strNowLoopStartDate.Substring(6, 2)
                            MessageBox.Show(String.Format(MSG0368W, strMsgWord1, strMsgWord2), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return False
                        End If
                    End If

                    strBfrLoopStartDate = oraReader.GetString("START_DATE_Z")
                    strBfrLoopEndDate = oraReader.GetString("END_DATE_Z")

                    intCounter += 1

                    oraReader.NextRead()
                End While
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("適用期間連続性チェック", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try
    End Function

#End Region


End Class