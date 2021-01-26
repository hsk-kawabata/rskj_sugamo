Imports System.Text
Imports CASTCommon
Public Class KFJMAST050

#Region "宣言"

    Private BatchLog As New CASTCommon.BatchLOG("KFJMAST050", "支店読替マスタメンテナンス画面")
    Private Const msgTitle As String = "支店読替マスタメンテナンス画面(KFJMAST050)"
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MyOwnerForm As Form
    Private Jikinko As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

    '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
    Private MainDB As CASTCommon.MyOracle
    '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<

#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFJMAST050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '------------------------------------------
            'ログ設定
            '------------------------------------------
            BatchLog.UserID = gstrUSER_ID
            BatchLog.ToriCode = "0000000000-00"
            BatchLog.FuriDate = "00000000"
            BatchLog.Write("(ロード)開始", "成功")

            '------------------------------------------
            'フォーム設定
            '------------------------------------------
            MyOwnerForm = GOwnerForm
            GOwnerForm = Me
            btnEnd.DialogResult = DialogResult.None

            '------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------
            GCom.GetUserID = gstrUSER_ID
            GCom.GetSysDate = String.Format("{0:yyyy年MM月dd日}", Date.Now)
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------
            '画面初期表示
            '------------------------------------------
            Call sub_Clear()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)", "失敗", ex.Message)
            Me.Close()

        Finally
            BatchLog.Write("(ロード)終了", "成功")
        End Try
    End Sub

    '画面クローズ
    Private Sub KFJMAST050_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        Try
            GOwnerForm = MyOwnerForm
            GOwnerForm.Visible = True
        Catch ex As Exception
            BatchLog.Write("(画面クローズ)", "失敗", ex.Message)
        End Try
    End Sub

#End Region

#Region "ボタン"

    '登録ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            BatchLog.Write("(登録)開始", "成功")

            '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
            MainDB = New CASTCommon.MyOracle
            '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<

            '------------------------------------------------
            '入力チェック
            '------------------------------------------------
            If fn_TextCheck(0) = False Then
                Exit Sub
            End If

            '------------------------------------------------
            'マスタ重複チェック
            '------------------------------------------------
            If fn_SelectSITENYOMIKAE(0) = False Then
                Exit Sub
            End If

            '------------------------------------------------
            '確認メッセージ
            '------------------------------------------------
            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                txtOldKinCode.Focus()
                Exit Sub
            End If
            '------------------------------------------------
            '登録処理実行
            '------------------------------------------------
            If fn_InsertSITENYOMIKAE() = True Then
                MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Exit Sub
            End If

            '------------------------------------------------
            '画面クリア
            '------------------------------------------------
            Call sub_Clear()
            txtOldKinCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("(登録)", "失敗", ex.Message)
        Finally
            '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<

            BatchLog.Write("(登録)終了", "成功")
        End Try
    End Sub

    '更新ボタン
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        Try
            BatchLog.Write("(更新)開始", "成功")

            '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
            MainDB = New CASTCommon.MyOracle
            '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<

            '------------------------------------------------
            '入力チェック
            '------------------------------------------------
            If fn_TextCheck(1) = False Then
                Exit Sub
            End If

            '------------------------------------------------
            '確認メッセージ
            '------------------------------------------------
            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                txtNewKinCode.Focus()
                Exit Sub
            End If

            '------------------------------------------------
            '更新処理実行
            '------------------------------------------------
            If fn_UpdateSITENYOMIKAE() = True Then
                MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Exit Sub
            End If

            '------------------------------------------------
            '画面クリア
            '------------------------------------------------
            Call sub_Clear()
            txtOldKinCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("(更新)", "失敗", ex.Message)
        Finally
            '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<

            BatchLog.Write("(更新)終了", "成功")
        End Try
    End Sub

    '参照ボタン
    Private Sub btnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelect.Click
        Try
            BatchLog.Write("(参照)開始", "成功")

            '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
            MainDB = New CASTCommon.MyOracle
            '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<

            '------------------------------------------------
            '入力チェック
            '------------------------------------------------
            If fn_TextCheck(2) = False Then
                Exit Sub
            End If

            '------------------------------------------------
            '参照処理実行
            '------------------------------------------------
            If fn_SelectSITENYOMIKAE(1) = False Then
                Exit Sub
            End If

            '------------------------------------------------
            '金融機関名設定
            '------------------------------------------------
            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            '共通関数使用
            Me.lblNewKinNName.Text = GCom.GetBKBRName(Me.txtNewKinCode.Text, "")
            Me.lblNewSitNName.Text = GCom.GetBKBRName(Me.txtNewKinCode.Text, Me.txtNewSitCode.Text)
            'Call fn_GetKinNName(txtNewKinCode.Text, lblNewKinNName.Text)
            'Call fn_GetSitNName(txtNewKinCode.Text, txtNewSitCode.Text, lblNewSitNName.Text)
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<

            '------------------------------------------------
            '入力制御
            '------------------------------------------------
            'テキストボックス
            txtOldKinCode.Enabled = False
            txtOldSitCode.Enabled = False

            'ボタン
            btnAction.Enabled = False
            btnUpdate.Enabled = True
            btnSelect.Enabled = False
            btnDelete.Enabled = True
            btnClear.Enabled = True
            btnEnd.Enabled = True

            txtNewKinCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("(参照)", "失敗", ex.Message)

            '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<
        End Try
    End Sub

    '削除ボタン
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            BatchLog.Write("(削除)開始", "成功")

            '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
            MainDB = New CASTCommon.MyOracle
            '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<

            '------------------------------------------------
            '確認メッセージ
            '------------------------------------------------
            If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                txtNewKinCode.Focus()
                Exit Sub
            End If

            '------------------------------------------------
            '入力チェック
            '------------------------------------------------
            If fn_TextCheck(2) = False Then
                Exit Sub
            End If

            '------------------------------------------------
            '削除処理実行
            '------------------------------------------------
            If fn_DeleteSITENYOMIKAE() = False Then
                Exit Sub
            End If

            '------------------------------------------------
            '画面クリア
            '------------------------------------------------
            Call sub_Clear()
            MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            txtOldKinCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("(削除)", "失敗", ex.Message)
        Finally
            '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<
        End Try
    End Sub

    '取消ボタン
    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Try
            BatchLog.Write("(取消)開始", "成功", "")
            Call sub_Clear()
            txtOldKinCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("(取消)", "失敗", ex.Message)
        Finally
            BatchLog.Write("(取消)終了", "成功", "")
        End Try

    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            BatchLog.Write("(終了)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write("(終了)", "失敗", ex.Message)
        Finally
            BatchLog.Write("(終了)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "テキストボックス"

    '旧金融機関コード
    Private Sub txtOldKinCode_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtOldKinCode.Validating
        Try
            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            '共通関数使用
            Dim Temp As String = GCom.NzDec(CType(sender, TextBox).Text, "")
            If Temp.Length = 0 Then
                Me.txtOldKinCode.Text = String.Empty
                Me.lblOldKinNName.Text = String.Empty
            Else
                Call GCom.NzNumberString(CType(sender, TextBox), True)          '0埋め
                Me.lblOldKinNName.Text = GCom.GetBKBRName(Me.txtOldKinCode.Text, "")
                If Me.txtOldSitCode.Text.Trim <> "" Then
                    Me.lblOldSitNName.Text = GCom.GetBKBRName(Me.txtOldKinCode.Text, Me.txtOldSitCode.Text)
                End If
            End If
            'Call GCom.NzNumberString(CType(sender, TextBox), True)          '0埋め
            'Call fn_GetKinNName(txtOldKinCode.Text, lblOldKinNName.Text)    '金融機関名取得
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(テキスト制御)", "失敗", ex.Message)
        End Try
    End Sub

    '旧支店コード
    Private Sub txtOldSitCode_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtOldSitCode.Validating
        Try
            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            '共通関数使用
            Dim Temp As String = GCom.NzDec(CType(sender, TextBox).Text, "")
            If Temp.Length = 0 Then
                Me.txtOldSitCode.Text = String.Empty
                Me.lblOldSitNName.Text = String.Empty
            Else
                Call GCom.NzNumberString(CType(sender, TextBox), True)                              '0埋め
                Me.lblOldKinNName.Text = GCom.GetBKBRName(Me.txtOldKinCode.Text, "")
                Me.lblOldSitNName.Text = GCom.GetBKBRName(Me.txtOldKinCode.Text, Me.txtOldSitCode.Text)
            End If
            'Call GCom.NzNumberString(CType(sender, TextBox), True)                              '0埋め
            'Call fn_GetSitNName(txtOldKinCode.Text, txtOldSitCode.Text, lblOldSitNName.Text)    '支店名取得
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(テキスト制御)", "失敗", ex.Message)
        End Try
    End Sub

    '読替後金融機関コード
    Private Sub txtNewKinCode_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtNewKinCode.Validating
        Try
            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            '共通関数使用
            Dim Temp As String = GCom.NzDec(CType(sender, TextBox).Text, "")
            If Temp.Length = 0 Then
                Me.txtNewKinCode.Text = String.Empty
                Me.lblNewKinNName.Text = String.Empty
            Else
                Call GCom.NzNumberString(CType(sender, TextBox), True)          '0埋め
                Me.lblNewKinNName.Text = GCom.GetBKBRName(Me.txtNewKinCode.Text, "")
                If Me.txtNewSitCode.Text.Trim <> "" Then
                    Me.lblNewSitNName.Text = GCom.GetBKBRName(Me.txtNewKinCode.Text, Me.txtNewSitCode.Text)
                End If
            End If
            'Call GCom.NzNumberString(CType(sender, TextBox), True)          '0埋め
            'Call fn_GetKinNName(txtNewKinCode.Text, lblNewKinNName.Text)    '金融機関名取得
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(テキスト制御)", "失敗", ex.Message)
        End Try
    End Sub

    '読替後支店コード
    Private Sub txtNewSitCode_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtNewSitCode.Validating
        Try
            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            '共通関数使用
            Dim Temp As String = GCom.NzDec(CType(sender, TextBox).Text, "")
            If Temp.Length = 0 Then
                Me.txtNewSitCode.Text = String.Empty
                Me.lblNewSitNName.Text = String.Empty
            Else
                Call GCom.NzNumberString(CType(sender, TextBox), True)                              '0埋め
                Me.lblNewKinNName.Text = GCom.GetBKBRName(Me.txtNewKinCode.Text, "")
                Me.lblNewSitNName.Text = GCom.GetBKBRName(Me.txtNewKinCode.Text, Me.txtNewSitCode.Text)
            End If
            'Call GCom.NzNumberString(CType(sender, TextBox), True)                              '0埋め
            'Call fn_GetSitNName(txtNewKinCode.Text, txtNewSitCode.Text, lblNewSitNName.Text)    '支店名取得
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(テキスト制御)", "失敗", ex.Message)
        End Try
    End Sub

#End Region

#Region "関数"

    Private Sub sub_Clear()
        '=====================================================================================
        'NAME           :sub_Clear
        'Parameter      :なし
        'Description    :画面初期化
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Try
            '------------------------------------------------
            'テキストボックス初期化
            '------------------------------------------------
            txtOldKinCode.Text = ""
            txtOldSitCode.Text = ""
            'txtNewKinCode.Text = ""
            txtNewKinCode.Text = Jikinko
            txtNewSitCode.Text = ""
            txtOldKinCode.Enabled = True
            txtOldSitCode.Enabled = True

            '------------------------------------------------
            'ラベル初期化
            '------------------------------------------------
            lblOldKinNName.Text = ""
            lblOldSitNName.Text = ""
            'lblNewKinNName.Text = ""
            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            '共通関数使用
            Me.lblNewKinNName.Text = GCom.GetBKBRName(Me.txtNewKinCode.Text, "")
            'Call fn_GetKinNName(txtNewKinCode.Text, lblNewKinNName.Text)
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<
            lblNewSitNName.Text = ""

            '------------------------------------------------
            'ボタン制御初期化
            '------------------------------------------------
            btnAction.Enabled = True
            btnUpdate.Enabled = False
            btnSelect.Enabled = True
            btnDelete.Enabled = False
            btnClear.Enabled = True
            btnEnd.Enabled = True

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Function fn_TextCheck(ByVal SyoriKBN As Integer) As String
        '=====================================================================================
        'NAME           :fn_InsertSITENYOMIKAE
        'Parameter      :SyoriKBN - チェック区分(0 - 登録, 1 - 更新, 2 - 削除)
        'Description    :入力チェック
        'Return         :True=OK(成功/入力値正常),False=NG（失敗/入力値異常）
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Try
            '------------------------------------------------
            'テキストボックス未入力チェック
            '------------------------------------------------
            Select Case True    'True = 未入力

                Case Trim(txtOldKinCode.Text) = ""      '旧金融機関コード
                    MessageBox.Show(String.Format(MSG0285W, "旧金融機関コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtOldKinCode.Focus()
                    Return False

                Case Trim(txtOldSitCode.Text) = ""      '旧支店コード
                    MessageBox.Show(String.Format(MSG0285W, "旧支店コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtOldSitCode.Focus()
                    Return False

                Case Trim(txtNewKinCode.Text) = ""      '読替後金融機関コード
                    If SyoriKBN <> 2 Then
                        MessageBox.Show(String.Format(MSG0285W, "読替後金融機関コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtNewKinCode.Focus()
                        Return False
                    End If

                Case Trim(txtNewSitCode.Text) = ""      '読替後支店コード
                    If SyoriKBN <> 2 Then
                        MessageBox.Show(String.Format(MSG0285W, "読替後支店コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtNewSitCode.Focus()
                        Return False
                    End If
            End Select

            '2010/02/23 読替後の金融機関は通常自金庫となる
            If SyoriKBN <> 2 AndAlso Trim(txtNewKinCode.Text) <> Jikinko Then
                MessageBox.Show(MSG0356W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtNewKinCode.Focus()
                Return False
            End If
            '=============================================

            '------------------------------------------------
            '読替後金融機関存在チェック(削除処理時は不要)
            '------------------------------------------------
            If SyoriKBN <> 2 AndAlso fn_SelectKin() = False Then
                txtNewKinCode.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(入力チェック)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtOldKinCode.Focus()
            Return False
        End Try
    End Function

    Private Function fn_SelectKin() As Boolean
        '=====================================================================================
        'NAME           :fn_SelectKin
        'Parameter      :KinCode - 金融機関コード / KinNName - 金融機関名
        'Description    :金融機関名を金融機関マスタから取得する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<
        Try
            Dim SQL As New StringBuilder(128)

            '------------------------------------------------
            '金融機関マスタより、読替後金融機関の存在確認を行う
            '------------------------------------------------
            SQL.Append("SELECT COUNT(*) AS COUNT")
            SQL.Append(" FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = '" & txtNewKinCode.Text & "'")
            SQL.Append(" AND SIT_NO_N = '" & txtNewSitCode.Text & "'")

            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(SQL) = True AndAlso oraReader.GetInt("COUNT") > 0 Then
                Return True
            Else
                MessageBox.Show(MSG0034W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            'gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
            'gdbcCONNECT.Open()

            'gdbCOMMAND = New OracleClient.OracleCommand
            'gdbCOMMAND.CommandText = SQL.ToString
            'gdbCOMMAND.Connection = gdbcCONNECT

            'gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

            'If gdbrREADER.Read() = True AndAlso gdbrREADER.Item("COUNT") > 0 Then
            '    Return True
            'Else
            '    MessageBox.Show(MSG0034W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Return False
            'End If
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "金融機関名取得", "失敗", ex.Message)
            Return False

        Finally
            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
            'If gdbrREADER IsNot Nothing Then gdbrREADER.Close() : gdbrREADER = Nothing
            'If gdbcCONNECT IsNot Nothing Then
            '    gdbcCONNECT.Close()
            'End If
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<
        End Try
    End Function

    Private Function fn_InsertSITENYOMIKAE() As Boolean
        '=====================================================================================
        'NAME           :fn_InsertSITENYOMIKAE
        'Parameter      :なし
        'Description    :支店読替マスタに登録する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Dim SQL As New StringBuilder(128)

        Try
            '------------------------------------------------
            'INSERT文実行
            '------------------------------------------------
            SQL.Append("INSERT INTO SITENYOMIKAE")
            SQL.Append(" VALUES(")
            SQL.Append(" '" & txtOldKinCode.Text & "'")
            SQL.Append(",'" & txtOldSitCode.Text & "'")
            SQL.Append(",'" & txtNewKinCode.Text & "'")
            SQL.Append(",'" & txtNewSitCode.Text & "'")
            SQL.Append(",''")
            SQL.Append(")")

            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            Dim iRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If iRet <> 1 Then
                MessageBox.Show(String.Format(MSG0027E, "支店読替マスタ", "登録"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return False
            End If

            MainDB.Commit()

            'gdbcCONNECT.ConnectionString = gstrDB_CONNECT
            'gdbcCONNECT.Open()

            'gdbCOMMAND = New OracleClient.OracleCommand
            'gdbCOMMAND.CommandText = SQL.ToString
            'gdbCOMMAND.Connection = gdbcCONNECT
            'gdbTRANS = gdbcCONNECT.BeginTransaction
            'gdbCOMMAND.Transaction = gdbTRANS

            'Try
            '    gdbCOMMAND.ExecuteNonQuery()
            '    gdbTRANS.Commit()
            'Catch ex As Exception
            '    gdbTRANS.Rollback()
            '    gdbcCONNECT.Close()
            '    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(支店読替マスタ登録)", "失敗", ex.Message)
            '    MessageBox.Show(String.Format(MSG0027E, "支店読替マスタ", "登録"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    txtOldKinCode.Focus()
            '    Return False
            'End Try
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<

            Return True

        Catch ex As Exception
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(支店読替マスタ登録)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtOldKinCode.Focus()
            '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
            MainDB.Rollback()
            '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<
            Return False

        Finally
            '2013/12/24 saitou 標準版 処理改善 DEL -------------------------------------------------->>>>
            'If gdbcCONNECT IsNot Nothing Then
            '    gdbcCONNECT.Close()
            'End If
            '2013/12/24 saitou 標準版 処理改善 DEL --------------------------------------------------<<<<
        End Try
    End Function

    Private Function fn_UpdateSITENYOMIKAE() As Boolean
        '=====================================================================================
        'NAME           :fn_UpdateSITENYOMIKAE
        'Parameter      :なし
        'Description    :支店読替マスタを更新する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Dim SQL As New StringBuilder(128)

        Try
            '------------------------------------------------
            'UPDATE文実行
            '------------------------------------------------
            SQL.Append("UPDATE SITENYOMIKAE SET")
            SQL.Append(" NEW_KIN_NO_S = '" & txtNewKinCode.Text & "'")
            SQL.Append(",NEW_SIT_NO_S = '" & txtNewSitCode.Text & "'")
            SQL.Append(" WHERE OLD_KIN_NO_S = '" & txtOldKinCode.Text & "'")
            SQL.Append(" AND OLD_SIT_NO_S = '" & txtOldSitCode.Text & "'")

            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            Dim iRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If iRet <> 1 Then
                MessageBox.Show(String.Format(MSG0027E, "支店読替マスタ", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return False
            End If

            MainDB.Commit()

            'gdbcCONNECT.ConnectionString = gstrDB_CONNECT
            'gdbcCONNECT.Open()

            'gdbCOMMAND = New OracleClient.OracleCommand
            'gdbCOMMAND.CommandText = SQL.ToString
            'gdbCOMMAND.Connection = gdbcCONNECT
            'gdbTRANS = gdbcCONNECT.BeginTransaction
            'gdbCOMMAND.Transaction = gdbTRANS

            'Try
            '    gdbCOMMAND.ExecuteNonQuery()
            '    gdbTRANS.Commit()
            'Catch ex As Exception
            '    gdbTRANS.Rollback()
            '    gdbcCONNECT.Close()
            '    BatchLog.Write("(支店読替マスタ更新)", "失敗", ex.Message)
            '    MessageBox.Show(String.Format(MSG0027E, "支店読替マスタ", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    txtOldKinCode.Focus()
            '    Return False
            'End Try
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<

            Return True

        Catch ex As Exception
            BatchLog.Write("(支店読替マスタ更新)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtOldKinCode.Focus()
            '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
            MainDB.Rollback()
            '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<
            Return False

        Finally
            '2013/12/24 saitou 標準版 処理改善 DEL -------------------------------------------------->>>>
            'If gdbcCONNECT IsNot Nothing Then
            '    gdbcCONNECT.Close()
            'End If
            '2013/12/24 saitou 標準版 処理改善 DEL --------------------------------------------------<<<<
        End Try
    End Function

    Private Function fn_SelectSITENYOMIKAE(ByVal SyoriKBN As Integer) As Boolean
        '=====================================================================================
        'NAME           :fn_SelectSITENYOMIKAE
        'Parameter      :SyoriKBN - 処理区分(0 - 登録済＝ＮＧ, 1 - 未登録＝ＮＧ)
        'Description    :支店読替マスタから参照する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Dim SQL As New StringBuilder(128)
        '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<
        Try
            '------------------------------------------------
            '金融機関マスタより、読替後金融機関の存在確認を行う
            '------------------------------------------------
            SQL.Append("SELECT NEW_KIN_NO_S")
            SQL.Append(",NEW_SIT_NO_S")
            SQL.Append(" FROM SITENYOMIKAE")
            SQL.Append(" WHERE OLD_KIN_NO_S = '" & txtOldKinCode.Text & "'")
            SQL.Append(" AND OLD_SIT_NO_S = '" & txtOldSitCode.Text & "'")

            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(SQL) = True Then
                Select Case SyoriKBN
                    Case 0
                        '登録済メッセージ出力
                        MessageBox.Show(MSG0097W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtOldKinCode.Focus()
                        Return False
                    Case 1
                        '取得した値を設定
                        txtNewKinCode.Text = oraReader.GetItem("NEW_KIN_NO_S")
                        txtNewSitCode.Text = oraReader.GetItem("NEW_SIT_NO_S")
                        Return True
                End Select

            Else
                Select Case SyoriKBN
                    Case 0
                        '正常値を返すのみ
                        Return True
                    Case 1
                        '未登録メッセージ出力
                        MessageBox.Show(MSG0098W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtOldKinCode.Focus()
                        Return False
                End Select
            End If

            'gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
            'gdbcCONNECT.Open()

            'gdbCOMMAND = New OracleClient.OracleCommand
            'gdbCOMMAND.CommandText = SQL.ToString
            'gdbCOMMAND.Connection = gdbcCONNECT

            'gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

            'If gdbrREADER.Read() = True Then
            '    Select Case SyoriKBN
            '        Case 0
            '            '登録済メッセージ出力
            '            MessageBox.Show(MSG0097W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '            txtOldKinCode.Focus()
            '            Return False
            '        Case 1
            '            '取得した値を設定
            '            txtNewKinCode.Text = gdbrREADER.Item("NEW_KIN_NO_S")
            '            txtNewSitCode.Text = gdbrREADER.Item("NEW_SIT_NO_S")
            '            Return True
            '    End Select

            'Else
            '    Select Case SyoriKBN
            '        Case 0
            '            '正常値を返すのみ
            '            Return True
            '        Case 1
            '            '未登録メッセージ出力
            '            MessageBox.Show(MSG0098W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '            txtOldKinCode.Focus()
            '            Return False
            '    End Select
            'End If
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "金融機関名取得", "失敗", ex.Message)
            txtOldKinCode.Focus()
            Return False

        Finally
            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If

            'If gdbrREADER IsNot Nothing Then gdbrREADER.Close() : gdbrREADER = Nothing

            'If gdbcCONNECT IsNot Nothing Then
            '    gdbcCONNECT.Close()
            'End If
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<
        End Try
    End Function

    Private Function fn_DeleteSITENYOMIKAE() As Boolean
        '=====================================================================================
        'NAME           :fn_DeleteSITENYOMIKAE
        'Parameter      :なし
        'Description    :支店読替マスタから削除する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Dim SQL As New StringBuilder(128)

        Try
            '------------------------------------------------
            'DELETE文実行
            '------------------------------------------------
            SQL.Append("DELETE")
            SQL.Append(" FROM SITENYOMIKAE")
            SQL.Append(" WHERE OLD_KIN_NO_S = '" & txtOldKinCode.Text & "'")
            SQL.Append(" AND OLD_SIT_NO_S = '" & txtOldSitCode.Text & "'")

            '2013/12/24 saitou 標準版 処理改善 UPD -------------------------------------------------->>>>
            Dim iRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If iRet <> 1 Then
                MessageBox.Show(String.Format(MSG0027E, "支店読替マスタ", "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return False
            End If

            'gdbcCONNECT.ConnectionString = gstrDB_CONNECT
            'gdbcCONNECT.Open()

            'gdbCOMMAND = New OracleClient.OracleCommand
            'gdbCOMMAND.CommandText = SQL.ToString
            'gdbCOMMAND.Connection = gdbcCONNECT
            'gdbTRANS = gdbcCONNECT.BeginTransaction
            'gdbCOMMAND.Transaction = gdbTRANS

            'Try
            '    gdbCOMMAND.ExecuteNonQuery()
            '    gdbTRANS.Commit()
            'Catch ex As Exception
            '    gdbTRANS.Rollback()
            '    gdbcCONNECT.Close()
            '    BatchLog.Write("(支店読替マスタ削除)", "失敗", ex.Message)
            '    MessageBox.Show(String.Format(MSG0027E, "支店読替マスタ", "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    txtOldKinCode.Focus()
            '    Return False
            'End Try
            '2013/12/24 saitou 標準版 処理改善 UPD --------------------------------------------------<<<<

            Return True

        Catch ex As Exception
            BatchLog.Write("(支店読替マスタ削除)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtOldKinCode.Focus()
            '2013/12/24 saitou 標準版 処理改善 ADD -------------------------------------------------->>>>
            MainDB.Rollback()
            '2013/12/24 saitou 標準版 処理改善 ADD --------------------------------------------------<<<<
            Return False

        Finally
            '2013/12/24 saitou 標準版 処理改善 DEL -------------------------------------------------->>>>
            'If gdbcCONNECT IsNot Nothing Then
            '    gdbcCONNECT.Close()
            'End If
            '2013/12/24 saitou 標準版 処理改善 DEL --------------------------------------------------<<<<
        End Try
    End Function

    '2013/12/24 saitou 標準版 処理改善 DEL -------------------------------------------------->>>>
    'もう呼ばれることはない
    'Function fn_GetKinNName(ByVal KinCode As String, ByRef KinNName As String) As Boolean
    '    '=====================================================================================
    '    'NAME           :fn_GetKinNName
    '    'Parameter      :KinCode - 金融機関コード / KinNName - 金融機関名
    '    'Description    :金融機関名を金融機関マスタから取得する
    '    'Return         :True=OK(成功),False=NG（失敗）
    '    'Create         :2009/09/18
    '    'Update         :
    '    '=====================================================================================
    '    Try
    '        Dim SQL As New StringBuilder(128)

    '        '------------------------------------------------
    '        '金融機関マスタより、金融機関名を取得する
    '        '------------------------------------------------
    '        SQL.Append("SELECT KIN_NNAME_N")
    '        SQL.Append(" FROM TENMAST")
    '        SQL.Append(" WHERE KIN_NO_N = '" & KinCode & "'")

    '        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
    '        gdbcCONNECT.Open()

    '        gdbCOMMAND = New OracleClient.OracleCommand
    '        gdbCOMMAND.CommandText = SQL.ToString
    '        gdbCOMMAND.Connection = gdbcCONNECT

    '        gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

    '        If gdbrREADER.Read() = True Then
    '            KinNName = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("KIN_NNAME_N"))
    '        Else
    '            KinNName = ""
    '        End If

    '        Return True

    '    Catch ex As Exception
    '        MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "金融機関名取得", "失敗", ex.Message)
    '        Return False

    '    Finally
    '        If gdbrREADER IsNot Nothing Then gdbrREADER.Close() : gdbrREADER = Nothing
    '        If gdbcCONNECT IsNot Nothing Then
    '            gdbcCONNECT.Close()
    '        End If

    '    End Try
    'End Function

    'Function fn_GetSitNName(ByVal KinCode As String, ByVal SitCode As String, ByRef SitNName As String) As Boolean
    '    '=====================================================================================
    '    'NAME           :fn_GetSitNName
    '    'Parameter      :KinCode - 金融機関コード / SitCode - 支店コード / SitNName - 金融機関名
    '    'Description    :支店名を金融機関マスタから取得する
    '    'Return         :True=OK(成功),False=NG（失敗）
    '    'Create         :2009/09/18
    '    'Update         :
    '    '=====================================================================================
    '    Dim SQL As New StringBuilder(128)

    '    Try
    '        '------------------------------------------------
    '        '金融機関マスタより、支店名を取得する
    '        '------------------------------------------------
    '        SQL.Append("SELECT SIT_NNAME_N")
    '        SQL.Append(" FROM TENMAST")
    '        SQL.Append(" WHERE KIN_NO_N = '" & KinCode & "'")
    '        SQL.Append(" AND SIT_NO_N = '" & SitCode & "'")

    '        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
    '        gdbcCONNECT.Open()

    '        gdbCOMMAND = New OracleClient.OracleCommand
    '        gdbCOMMAND.CommandText = SQL.ToString
    '        gdbCOMMAND.Connection = gdbcCONNECT

    '        gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

    '        If gdbrREADER.Read() = True Then
    '            SitNName = clsFUSION.fn_chenge_null_value(gdbrREADER.Item("SIT_NNAME_N"))
    '        Else
    '            SitNName = ""
    '        End If

    '        Return True

    '    Catch ex As Exception
    '        MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "金融機関名取得", "失敗", ex.Message)
    '        Return False

    '    Finally
    '        If gdbrREADER IsNot Nothing Then gdbrREADER.Close() : gdbrREADER = Nothing
    '        If gdbcCONNECT IsNot Nothing Then
    '            gdbcCONNECT.Close()
    '        End If

    '    End Try
    'End Function
    '2013/12/24 saitou 標準版 処理改善 DEL --------------------------------------------------<<<<

#End Region

End Class