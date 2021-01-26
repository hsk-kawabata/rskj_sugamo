Imports System
Imports System.Text
Imports CASTCommon

''' <summary>
''' 印紙税マスタメンテナンス画面
''' </summary>
''' <remarks></remarks>
Public Class KFUMAST070

#Region "クラス定数"
    Private Const msgTitle As String = "印紙税マスタメンテナンス画面(KFUMAST070)"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
#End Region

#Region "クラス変数"
    Private MainLOG As New CASTCommon.BatchLOG("KFUMAST100", "印紙税マスタメンテナンス画面")
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
    Private Sub KFUMAST070_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            Me.CreateInshizeiList()
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True

            Me.ActiveControl = Me.txtInshizeiID

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
            If Me.txtInshizeiID.Text.Trim = String.Empty Then
                'IDは必須項目
                MessageBox.Show(String.Format(MSG0285W, "印紙税ID"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtInshizeiID.Focus()
                Return
            End If
            If Me.CheckText() = False Then
                Return
            End If

            Dim strKaishiDate As String = String.Concat(New String() {Me.txtKaishiDateY.Text, Me.txtKaishiDateM.Text, Me.txtKaishiDateD.Text})
            Dim strSyuryoDate As String = String.Concat(New String() {Me.txtSyuryoDateY.Text, Me.txtSyuryoDateM.Text, Me.txtSyuryoDateD.Text})

            '------------------------------------------------
            '印紙税マスタチェック
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(Me.CreateGetInshizeimastSQL(Me.txtInshizeiID.Text.Trim)) = True Then
                '既に設定済み
                MessageBox.Show(MSG0363W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtInshizeiID.Focus()
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
                .Append("insert into INSHIZEIMAST")
                .Append(" (INSHIZEI_ID_I, INSHIZEI1_I, INSHIZEI2_I, START_DATE_I, END_DATE_I)")
                .Append(" values (")
                .Append(" " & SQ(Me.txtInshizeiID.Text.Trim))
                .Append("," & SQ(Me.DeleteComma(Me.txtInshizei1_1.Text.Trim)))
                .Append("," & SQ(Me.DeleteComma(Me.txtInshizei2_2.Text.Trim)))
                .Append("," & SQ(strKaishiDate))
                .Append("," & SQ(strSyuryoDate))
                .Append(")")
            End With

            If MainDB.ExecuteNonQuery(SQL) <> 1 Then
                MainLOG.Write("印紙税マスタ挿入", "失敗", MainDB.Message)
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
            Me.CreateInshizeiList()
            Me.txtInshizeiID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True
            Me.txtInshizeiID.Focus()

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
            '印紙税マスタチェック
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            If Me.CheckTekiyoDate(strKaishiDate, strSyuryoDate, Me.txtInshizeiID.Text.Trim) = False Then
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
                .Append("update INSHIZEIMAST set")
                .Append(" INSHIZEI1_I = " & SQ(Me.DeleteComma(Me.txtInshizei1_1.Text.Trim)))
                .Append(",INSHIZEI2_I = " & SQ(Me.DeleteComma(Me.txtInshizei2_2.Text.Trim)))
                .Append(",START_DATE_I = " & SQ(strKaishiDate))
                .Append(",END_DATE_I = " & SQ(strSyuryoDate))
                .Append(" where INSHIZEI_ID_I = " & SQ(Me.txtInshizeiID.Text.Trim))
            End With

            Dim iRet As Integer
            iRet = MainDB.ExecuteNonQuery(SQL)
            If iRet <> 1 Then
                MainLOG.Write("印紙税マスタ更新", "失敗", MainDB.Message)
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
            Me.CreateInshizeiList()
            Me.txtInshizeiID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True
            Me.txtInshizeiID.Focus()

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
            '印紙税マスタチェック
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(Me.CreateGetInshizeimastSQL(Me.txtInshizeiID.Text.Trim)) = False Then
                '印紙税設定なし
                MessageBox.Show(String.Format(MSG0258W, "印紙税マスタ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtInshizeiID.Focus()
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
                .Append("delete from INSHIZEIMAST where INSHIZEI_ID_I = " & SQ(Me.txtInshizeiID.Text.Trim))
            End With
            Dim iRet As Integer
            iRet = MainDB.ExecuteNonQuery(SQL)
            '必ず1レコード削除
            If iRet <> 1 Then
                MainLOG.Write("印紙税マスタ削除", "失敗", MainDB.Message)
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
            Me.CreateInshizeiList()
            Me.txtInshizeiID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True
            Me.txtInshizeiID.Focus()

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
            If Me.txtInshizeiID.Text.Trim = String.Empty Then
                '印紙税IDは必須項目
                MessageBox.Show(String.Format(MSG0285W, "印紙税ID"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtInshizeiID.Focus()
                Return
            End If

            '------------------------------------------------
            '印紙税マスタチェック
            '------------------------------------------------
            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(Me.CreateGetInshizeimastSQL(Me.txtInshizeiID.Text.Trim)) = True Then
                While oraReader.EOF = False
                    Me.txtInshizei1_1.Text = oraReader.GetString("INSHIZEI1_I")
                    Me.txtInshizei2_1.Text = oraReader.GetString("INSHIZEI1_I")
                    Me.txtInshizei2_2.Text = oraReader.GetString("INSHIZEI2_I")
                    Me.txtInshizei3_1.Text = oraReader.GetString("INSHIZEI2_I")
                    Me.txtKaishiDateY.Text = oraReader.GetString("START_DATE_I").Substring(0, 4)
                    Me.txtKaishiDateM.Text = oraReader.GetString("START_DATE_I").Substring(4, 2)
                    Me.txtKaishiDateD.Text = oraReader.GetString("START_DATE_I").Substring(6, 2)
                    Me.txtSyuryoDateY.Text = oraReader.GetString("END_DATE_I").Substring(0, 4)
                    Me.txtSyuryoDateM.Text = oraReader.GetString("END_DATE_I").Substring(4, 2)
                    Me.txtSyuryoDateD.Text = oraReader.GetString("END_DATE_I").Substring(6, 2)
                    oraReader.NextRead()
                End While
            Else
                '印紙税設定なし
                MessageBox.Show(MSG0364W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtInshizeiID.Focus()
                Return
            End If

            '------------------------------------------------
            '画面コントロールの設定
            '------------------------------------------------
            Me.txtInshizeiID.Enabled = False
            Me.btnAction.Enabled = False
            Me.btnUpdate.Enabled = True
            Me.btnDelete.Enabled = True
            Me.btnSelect.Enabled = False

            Me.txtInshizei1_1.Focus()

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
            Me.CreateInshizeiList()

            '------------------------------------------------
            '画面コントロールの設定
            '------------------------------------------------
            Me.txtInshizeiID.Enabled = True
            Me.btnAction.Enabled = True
            Me.btnUpdate.Enabled = False
            Me.btnDelete.Enabled = False
            Me.btnSelect.Enabled = True

            Me.txtInshizeiID.Focus()

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
        Handles txtInshizeiID.Validating, txtKaishiDateY.Validating, txtKaishiDateM.Validating, txtKaishiDateD.Validating, txtSyuryoDateY.Validating, txtSyuryoDateM.Validating, txtSyuryoDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' テキストボックスバリデイティングイベント（金額テキスト）
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Money_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtInshizei1_1.Validating, txtInshizei2_1.Validating, txtInshizei2_2.Validating, txtInshizei3_1.Validating
        Try
            Dim InputTextBox As TextBox = CType(sender, TextBox)
            InputTextBox.Text = String.Format("{0:#,##0}", GCom.NzDec(InputTextBox.Text, 1))
            Select Case InputTextBox.Name
                Case Me.txtInshizei1_1.Name
                    Me.txtInshizei2_1.Text = InputTextBox.Text
                Case Me.txtInshizei2_2.Name
                    Me.txtInshizei3_1.Text = InputTextBox.Text
            End Select
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("金額表示変数", "失敗", ex.ToString)
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
            '印紙税チェック
            If Me.txtInshizei1_1.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "印紙税"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtInshizei1_1.Focus()
                Return False
            End If
            If Me.txtInshizei2_2.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "印紙税"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtInshizei2_2.Focus()
                Return False
            End If

            '範囲チェック
            If GCom.NzInt(Me.DeleteComma(Me.txtInshizei1_1.Text.Trim)) >= GCom.NzInt(Me.DeleteComma(Me.txtInshizei2_2.Text.Trim)) Then
                '2014/02/07 saitou 標準版 印紙税対応 MODIFY ----------------------------------------------->>>>
                'メッセージ追加
                MessageBox.Show(MSG0370W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtInshizei2_2.Focus()
                '2014/02/07 saitou 標準版 印紙税対応 MODIFY -----------------------------------------------<<<<
                Return False
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
    ''' 印紙税マスタから印紙税情報を取得するSQLを作成します。
    ''' </summary>
    ''' <param name="strInshizeiID">印紙税ID</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function CreateGetInshizeimastSQL(ByVal strInshizeiID As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select * from INSHIZEIMAST")
            .Append(" where INSHIZEI_ID_I = " & SQ(strInshizeiID))
        End With

        Return SQL
    End Function

    ''' <summary>
    ''' テキストボックスをクリアします。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ClearText()
        Try
            Me.txtInshizeiID.Text = String.Empty
            Me.txtInshizei1_1.Text = "0"
            Me.txtInshizei2_1.Text = "0"
            Me.txtInshizei2_2.Text = "0"
            Me.txtInshizei3_1.Text = "0"
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
    ''' 印紙税情報設定用リストを作成します。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateInshizeiList()
        MainDB = New CASTCommon.MyOracle
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder

        Try
            With Me.ListView1
                .Clear()
                .Columns.Add("ＩＤ", 45, HorizontalAlignment.Center)
                .Columns.Add("区分１", 105, HorizontalAlignment.Center)
                .Columns.Add("区分２", 155, HorizontalAlignment.Center)
                .Columns.Add("区分３", 105, HorizontalAlignment.Center)
                .Columns.Add("適用開始日", 105, HorizontalAlignment.Center)
                .Columns.Add("適用終了日", 105, HorizontalAlignment.Center)
            End With

            With SQL
                .Length = 0
                .Append("select * from INSHIZEIMAST")
                .Append(" order by INSHIZEI_ID_I")
            End With

            If oraReader.DataReader(SQL) = True Then

                Dim ROW As Integer = 0

                While oraReader.EOF = False
                    Dim Data(5) As String

                    Data(0) = oraReader.GetString("INSHIZEI_ID_I")
                    Data(1) = "0～" & Me.AddComma(oraReader.GetString("INSHIZEI1_I"))
                    Data(2) = Me.AddComma(oraReader.GetString("INSHIZEI1_I")) & "～" & Me.AddComma(oraReader.GetString("INSHIZEI2_I"))
                    Data(3) = Me.AddComma(oraReader.GetString("INSHIZEI2_I")) & "～"
                    Data(4) = oraReader.GetString("START_DATE_I").Substring(0, 4) & "/" & oraReader.GetString("START_DATE_I").Substring(4, 2) & "/" & oraReader.GetString("START_DATE_I").Substring(6, 2)
                    Data(5) = oraReader.GetString("END_DATE_I").Substring(0, 4) & "/" & oraReader.GetString("END_DATE_I").Substring(4, 2) & "/" & oraReader.GetString("END_DATE_I").Substring(6, 2)

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
    ''' <param name="strInshizeiID">印紙税ID（更新時）</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function CheckTekiyoDate(ByVal strStartDate As String, _
                                     ByVal strEndDate As String, _
                                     Optional ByVal strInshizeiID As String = "") As Boolean

        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder

        Try
            '------------------------------------------------------------
            '適用期間が他の印紙税の適用期間と被っていないかチェック
            '------------------------------------------------------------
            '2014/02/07 saitou 標準版 印紙税対応 MODIFY ----------------------------------------------->>>>
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
                            .Append("select * from INSHIZEIMAST")
                            .Append(" where START_DATE_I between " & SQ(strStartDate) & " and " & SQ(strEndDate))
                            If strInshizeiID <> String.Empty Then
                                .Append(" and INSHIZEI_ID_I <> " & SQ(strInshizeiID))
                            End If
                        End With

                    Case 2
                        With SQL
                            .Length = 0
                            .Append("select * from INSHIZEIMAST")
                            .Append(" where END_DATE_I between " & SQ(strStartDate) & " and " & SQ(strEndDate))
                            If strInshizeiID <> String.Empty Then
                                .Append(" and INSHIZEI_ID_I <> " & SQ(strInshizeiID))
                            End If
                        End With

                    Case 3
                        With SQL
                            .Length = 0
                            .Append("select * from INSHIZEIMAST")
                            .Append(" where START_DATE_I <= " & SQ(strStartDate))
                            .Append(" and END_DATE_I >= " & SQ(strEndDate))
                            If strInshizeiID <> String.Empty Then
                                .Append(" and INSHIZEI_ID_I <> " & SQ(strInshizeiID))
                            End If
                        End With

                    Case 4
                        With SQL
                            .Length = 0
                            .Append("select * from INSHIZEIMAST")
                            .Append(" where " & SQ(strStartDate) & " <= START_DATE_I")
                            .Append(" and END_DATE_I <= " & SQ(strEndDate))
                            If strInshizeiID <> String.Empty Then
                                .Append(" and INSHIZEI_ID_I <> " & SQ(strInshizeiID))
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
            '    .Append("select * from INSHIZEIMAST")
            '    .Append(" where (( START_DATE_I between " & SQ(strStartDate) & " and " & SQ(strEndDate) & ")")
            '    .Append(" or ( END_DATE_I between " & SQ(strStartDate) & " and " & SQ(strEndDate) & "))")
            '    If strInshizeiID <> String.Empty Then
            '        .Append(" and INSHIZEI_ID_I <> " & SQ(strInshizeiID))
            '    End If
            'End With

            'If oraReader.DataReader(SQL) = True Then
            '    '適用期間被りあり
            '    Me.txtKaishiDateY.Focus()
            '    Return False
            'End If
            'oraReader.Close()
            '2014/02/07 saitou 標準版 印紙税対応 MODIFY -----------------------------------------------<<<<

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
            '印紙税マスタを適用開始日の昇順で並べる
            With SQL
                .Append("select * from INSHIZEIMAST")
                .Append(" order by START_DATE_I")
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
                        strNowLoopStartDate = oraReader.GetString("START_DATE_I")
                        strNowLoopEndDate = oraReader.GetString("END_DATE_I")

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

                    strBfrLoopStartDate = oraReader.GetString("START_DATE_I")
                    strBfrLoopEndDate = oraReader.GetString("END_DATE_I")

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