Option Explicit On
Option Strict On

Imports System
Imports System.Data
Imports System.Text

Public Class KFUMAST040

#Region "宣言"

    Inherits System.Windows.Forms.Form

    Private BatchLog As New CASTCommon.BatchLOG("KFUMAST040", "ユーザ情報メンテナンス")
    Private Const ErrMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"

    'キーコントロールクラス
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9A-Za-z", CASTCommon.Events.KeyMode.GOOD)

    Const msgTitle As String = "ユーザ情報メンテナンス(KFUMAST040)"
    Private Const ThisModuleName As String = "KFUMAST040.vb"

    Private Structure UIDMAST_Data
        Dim LOGINID_U As String         'CHAR	20
        Dim PASSWORD_U As String        'CHAR	20
        Dim KENGEN_U As String          'CHAR	1
        Dim PASSWORD_1S_U As String     'CHAR	20
        Dim PASSWORD_DATE_U As String   'CHAR	8
        Dim SAKUSEI_DATE_U As String    'CHAR	8
        Dim KOUSIN_DATE_U As String     'CHAR	8
    End Structure
    Private Data As UIDMAST_Data

    Private MainDB As CASTCommon.MyOracle

#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFUMAST040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            BatchLog.Write(GCom.GetUserID, "0000000000-00", "00000000", "(ロード)開始", "成功", "")

            lblUser.Text = GCom.GetUserID
            lblDate.Text = Format(System.DateTime.Today, "yyyy年MM月dd日")

            'タイトル部位置調整
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '入力／表示領域初期化
            Call Fn_ControlValueClear()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(GCom.GetUserID, "0000000000-00", "00000000", "(ロード)", "失敗", ex.Message)

        Finally
            BatchLog.Write(GCom.GetUserID, "0000000000-00", "00000000", "(ロード)終了", "成功", "")

        End Try

    End Sub

    '画面クローズ
    Private Sub Form1_FormClosing(ByVal sender As Object, _
            ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(クローズ)開始", "成功", "")
            'If e.CloseReason = CloseReason.UserClosing Then
            '    Me.Owner.Show()
            'End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(クローズ)", "失敗", ex.Message)

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(クローズ)終了", "成功", "")

        End Try

    End Sub

#End Region

#Region "ボタン"

    '登録ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Dim Ret As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(登録)開始", "成功", "")

            '入力チェック／画面入力データ取得
            If fn_CheckText(True) = False Then
                Exit Sub
            End If

            MainDB = New CASTCommon.MyOracle

            '登録済みチェック
            SQL.Length = 0
            SQL.Append("SELECT COUNT(*) COUNTER")
            SQL.Append(" FROM UIDMAST")
            SQL.Append(" WHERE LOGINID_U = '" & Data.LOGINID_U & "'")

            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(SQL) = True Then
                Ret = GCom.NzInt(oraReader.GetInt("COUNTER"))
            End If

            If Ret >= 1 Then
                MessageBox.Show(MSG0015W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtUserName.Focus()
                Exit Sub
            Else
                If Not oraReader Is Nothing Then
                    oraReader.Close()
                    oraReader = Nothing
                End If
            End If
            If txtPassword.Text.Length < 6 Then
                MessageBox.Show(MSG0229W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtPassword.Focus()
                Return
            End If

            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            '登録処理
            SQL.Length = 0
            SQL.Append("INSERT INTO UIDMAST")
            SQL.Append(" VALUES")
            SQL.Append(" ('" & Data.LOGINID_U & "'")
            SQL.Append(", '" & Data.PASSWORD_U & "'")
            SQL.Append(", '" & Data.KENGEN_U & "'")
            SQL.Append(", '" & Data.PASSWORD_U & "'")
            SQL.Append(", TO_CHAR(SYSDATE, 'YYYYMMDD')")
            SQL.Append(", TO_CHAR(SYSDATE, 'YYYYMMDD')")
            SQL.Append(", '00000000')")

            Try
                MainDB.ExecuteNonQuery(SQL)
                MainDB.Commit()
            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(登録)", "失敗", ex.Message)
                MainDB.Rollback()
                Exit Sub
            End Try

            MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            MainDB.Close()
            MainDB = Nothing

            '画面初期化処理
            Call Fn_ControlValueClear()

            Me.btnAction.Enabled = True
            Me.btnKousin.Enabled = False
            Me.btnDelete.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(登録)", "失敗", ex.Message)

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

            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(登録)終了", "成功", "")
        End Try

        Me.txtUserName.Focus()
    End Sub

    '更新ボタン
    Private Sub btnKousin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKousin.Click
        Dim Ret As Integer = 0
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(更新)開始", "成功", "")

            '入力チェック／画面入力データ取得
            If fn_CheckText() = False Then
                Exit Sub
            End If

            If txtPassword.Text.Length < 6 Then
                MessageBox.Show(MSG0229W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtPassword.Focus()
                Return
            End If

            MainDB = New CASTCommon.MyOracle

            SQL.Append("SELECT COUNT(*) COUNTER ,PASSWORD_U")
            SQL.Append(" FROM UIDMAST")
            SQL.Append(" WHERE LOGINID_U = '" & Data.LOGINID_U & "'")
            SQL.Append(" GROUP BY LOGINID_U ,PASSWORD_U")

            oraReader = New CASTCommon.MyOracleReader(MainDB)
            If oraReader.DataReader(SQL) = True Then
                Ret = GCom.NzInt(oraReader.GetInt("COUNTER"))
            End If

            If Ret = 0 Then
                MessageBox.Show(MSG0014W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            Else
                '現在のパスワードを前回パスワードとして保持
                Data.PASSWORD_1S_U = GCom.NzStr(oraReader.GetItem("PASSWORD_U"))
                If Not oraReader Is Nothing Then
                    oraReader.Close()
                    oraReader = Nothing
                End If
            End If

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            'マスタ更新処理
            SQL.Length = 0
            SQL.Append("UPDATE UIDMAST SET")
            SQL.Append(" PASSWORD_U = '" & Data.PASSWORD_U & "'")
            SQL.Append(",KENGEN_U = '" & Data.KENGEN_U & "'")
            SQL.Append(",PASSWORD_1S_U = '" & Data.PASSWORD_1S_U & "'")
            SQL.Append(",PASSWORD_DATE_U = TO_CHAR(SYSDATE, 'YYYYMMDD')")
            SQL.Append(",KOUSIN_DATE_U = TO_CHAR(SYSDATE, 'YYYYMMDD')")
            SQL.Append(" WHERE LOGINID_U = '" & Data.LOGINID_U & "'")

            Try
                MainDB.ExecuteNonQuery(SQL)
                MainDB.Commit()

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(更新)", "失敗", ex.Message)
                MainDB.Rollback()
                Exit Sub
            End Try

            MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            MainDB.Close()
            MainDB = Nothing

            '画面初期化処理
            Call Fn_ControlValueClear()
            txtUserName.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(更新)", "失敗", ex.Message)

        Finally
            txtUserName.Focus()
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If

            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(更新)終了", "成功", "")

        End Try

    End Sub

    '参照ボタン
    Private Sub btnSansyo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSansyo.Click
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(参照)開始", "成功", "")

            '入力チェック
            If fn_CheckText() = False Then
                Exit Sub
            End If

            MainDB = New CASTCommon.MyOracle

            '参照データ取得
            SQL.Append("SELECT PASSWORD_U")
            SQL.Append(", KENGEN_U")
            SQL.Append(" FROM UIDMAST")
            SQL.Append(" WHERE LOGINID_U = '" & Data.LOGINID_U & "'")

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            '画面設定
            If oraReader.DataReader(SQL) = True Then
                txtPassword.Text = GCom.NzStr(oraReader.GetItem("PASSWORD_U")).Trim  'パスワード

                ' 2016/03/07 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- START
                'If GCom.NzDec(oraReader.GetItem("KENGEN_U"), "") = "1" Then
                '    rdbKanrisya.Checked = True                              'ユーザ権限：管理者
                'Else
                '    rdbIppan.Checked = True                                 'ユーザ権限：一般
                'End If
                For Cnt As Integer = 0 To Me.KENGEN_U.Items.Count - 1 Step 1
                    Me.KENGEN_U.SelectedIndex = Cnt
                    If GCom.GetComboBox(Me.KENGEN_U) = CType(GCom.NzDec(oraReader.GetItem("KENGEN_U"), ""), Integer) Then
                        Exit For
                    End If
                Next Cnt
                ' 2016/03/07 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- END

                txtUserName.Enabled = False                                 'ログイン名：非活性
                btnAction.Enabled = False                                   '登録ボタン：非活性
                btnKousin.Enabled = True                                    '更新ボタン：活性
                btnSansyo.Enabled = False                                   '参照ボタン：非活性
                btnDelete.Enabled = True                                    '削除ボタン：活性
                btnEraser.Enabled = True                                    '取消ボタン：活性
                btnEnd.Enabled = True                                       '終了ボタン：活性

                'Me.txtPassword.Enabled = False
            Else
                MessageBox.Show(MSG0014W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(参照)", "失敗", ex.Message)

        Finally
            txtUserName.Focus()
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(参照)終了", "成功", "")

        End Try

    End Sub

    '削除ボタン
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Dim Ret As Integer = 0
        Dim oraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(削除)開始", "成功", "")

            '入力チェック
            If fn_CheckText() = False Then
                Exit Sub
            End If

            MainDB = New CASTCommon.MyOracle

            '登録チェック
            SQL.Append("SELECT COUNT(*) COUNTER")
            SQL.Append(" FROM UIDMAST")
            SQL.Append(" WHERE LOGINID_U = '" & Data.LOGINID_U & "'")

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(SQL) = True Then
                Ret = GCom.NzInt(oraReader.GetItem("COUNTER"))
            End If

            If Ret = 0 Then
                MessageBox.Show(MSG0014W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                txtUserName.Focus()
                Exit Sub
            Else
                If Not oraReader Is Nothing Then
                    oraReader.Close()
                    oraReader = Nothing
                End If
            End If

            If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            '削除処理
            SQL.Length = 0
            SQL.Append("DELETE FROM UIDMAST")
            SQL.Append(" WHERE LOGINID_U = '" & Data.LOGINID_U & "'")

            Try
                MainDB.ExecuteNonQuery(SQL)
                MainDB.Commit()
            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(削除)", "失敗", ex.Message)
                MainDB.Rollback()
                Exit Sub
            End Try

            MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            MainDB.Close()
            MainDB = Nothing

            '画面初期化処理
            Call Fn_ControlValueClear()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(削除)", "失敗", ex.Message)

        Finally
            txtUserName.Focus()
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(削除)終了", "成功", "")

        End Try
    End Sub

    '取消ボタン
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        Try
            If MessageBox.Show(MSG0009I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消処理)開始", "成功", "")

            If Fn_ControlValueClear() = False Then
                Me.txtUserName.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消処理)", "失敗", ex.Message)

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消処理)終了", "成功", "")

        End Try
    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了処理)開始", "成功", "")
            Me.Close()

        Catch ex As Exception
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了処理)", "失敗", ex.Message)

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了処理)終了", "成功", "")

        End Try
    End Sub

    ''' <summary>
    ''' 印刷ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>2017/04/28 saitou RSV2 added for 標準機能追加(登録ユーザ一覧表)</remarks>
    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(印刷)開始", "成功", "")

            '----------------------------------------
            '登録チェック
            '----------------------------------------
            Dim SQL As New StringBuilder
            With SQL
                .Append("select")
                .Append("     count(*) as COUNTER")
                .Append(" from")
                .Append("     UIDMAST")
            End With

            Me.MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)
            If OraReader.DataReader(SQL) = True Then
                If OraReader.GetInt("COUNTER") = 0 Then
                    'ユーザ登録無し
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            Else
                '検索失敗
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            '----------------------------------------
            '印刷処理
            '----------------------------------------
            If MessageBox.Show(String.Format(MSG0013I, "登録ユーザ一覧表"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If

            Dim ExeRepo As New CAstReports.ClsExecute
            Dim nRet As Integer
            Dim CmdArg As String
            ExeRepo.SetOwner = Me

            'パラメータ：ユーザID
            CmdArg = gstrUSER_ID
            nRet = ExeRepo.ExecReport("KFUP003.EXE", CmdArg)
            Select Case nRet
                Case 0
                    '印刷完了
                    MessageBox.Show(String.Format(MSG0014I, "登録ユーザ一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象無し
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case Else
                    '印刷失敗
                    MessageBox.Show(String.Format(MSG0004E, "登録ユーザ一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "印刷", "失敗", "戻り値:" & nRet)
            End Select

        Catch ex As Exception
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(印刷)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If

            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(印刷)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "関数"
    '========================================================================================
    ' 機能　 ： 入力値チェック関数
    ' 引数　 ： 登録フラグ
    ' 戻り値 ： True - 正常 / False - 異常
    ' 作成日 ： 2009/09/03
    ' 備考　 ： 
    '========================================================================================
    Private Function fn_CheckText(Optional ByVal INSERT As Boolean = False) As Boolean
        Dim onText(2) As Integer
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Dim txtUserNameW As String, txtPasswordW As String

        Try

            If txtUserName.Text.Length = 0 Then
                MessageBox.Show(MSG0004W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtUserName.Focus()
                Return False
            Else
                Data.LOGINID_U = txtUserName.Text
            End If

            'ログイン名チェック
            txtUserNameW = txtUserName.Text.Trim.Replace(" ", "")

            If txtUserName.Text.Length <> txtUserNameW.Length Then
                MessageBox.Show(MSG0340W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtUserName.Focus()
                Return False
            End If

            'パスワードチェック
            If txtPassword.Text.Length = 0 AndAlso INSERT = True Then
                MessageBox.Show(MSG0005W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtPassword.Focus()
                Return False
            Else
                Data.PASSWORD_U = txtPassword.Text
            End If

            txtPasswordW = txtPassword.Text.Trim.Replace(" ", "")

            If txtPassword.Text.Length <> txtPasswordW.Length Then
                MessageBox.Show(MSG0341W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtPassword.Focus()
                Return False
            End If

            '権限チェック
            ' 2016/03/07 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- START
            'If rdbKanrisya.Checked = True Then
            '    Data.KENGEN_U = "1"
            'ElseIf rdbIppan.Checked = True Then
            '    Data.KENGEN_U = "0"
            'End If
            Data.KENGEN_U = GCom.GetComboBox(Me.KENGEN_U).ToString
            ' 2016/03/07 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- END

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(入力値チェック)", "失敗", ex.Message)
            Return False

        Finally

        End Try

    End Function

    '========================================================================================
    ' 機能　 ： 入力／表示領域初期化
    ' 引数　 ： なし
    ' 戻り値 ： True - 正常 / False - 異常
    ' 作成日 ： 2009/09/03
    ' 備考　 ： 
    '========================================================================================
    Private Function Fn_ControlValueClear() As Boolean
        Try

            txtUserName.Text = ""       'ログイン名
            txtPassword.Text = ""       'パスワード

            ' 2016/03/07 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- START
            'rdbIppan.Checked = True     'チェックボックス：一般
            Select Case GCom.SetComboBox(KENGEN_U, "KFUMAST040_ユーザ権限.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "ユーザ権限", "KFUMAST040_ユーザ権限.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "ユーザ権限").ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
            End Select
            ' 2016/03/07 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- END

            txtUserName.Enabled = True  'ログイン名：活性
            btnAction.Enabled = True    '登録ボタン：活性
            btnKousin.Enabled = False   '更新ボタン：非活性
            btnSansyo.Enabled = True    '参照ボタン：活性
            btnDelete.Enabled = False   '削除ボタン：活性
            btnEraser.Enabled = True    '取消ボタン：活性
            txtPassword.Enabled = True

            txtUserName.Focus()
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(画面初期化)", "失敗", ex.Message)
            Return False

        Finally

        End Try

    End Function
#End Region

End Class
