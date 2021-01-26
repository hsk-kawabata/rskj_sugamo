Imports System
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports CASTCommon.ModMessage
Public Class KFUMAST050
    'キーコントロールクラス
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9A-Za-z", CASTCommon.Events.KeyMode.GOOD)

    'ログ書込クラス
    Private BatchLOG As New CASTCommon.BatchLOG("KFUMAST050", "ログインパスワード変更")
    Private Const msgTitle As String = "ログインパスワード変更(KFUMAST050)"
    Private MyOwnerForm As Form

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private strUSER_ID As String

    'Private Sub Form1_FormClosing(ByVal sender As Object, _
    '    ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
    '    If e.CloseReason = CloseReason.UserClosing Then
    '        Me.Owner.Show()
    '    End If
    'End Sub

    Private Sub KFUMAST050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            BatchLOG.Write("(ロード)開始", "成功", "")
            GOwnerForm = Me
            MyOwnerForm = GOwnerForm

            gstrUSER_ID = ""
            Dim CmdLine() As String = System.Environment.GetCommandLineArgs
            If CmdLine.Length >= 2 Then
                gstrUSER_ID = CmdLine(1)
                strUSER_ID = CmdLine(1)
            End If
            '   GCom.GetUserID = gstrUSER_ID
            GCom.GetUserID = strUSER_ID
            LW.UserID = strUSER_ID
            'ユーザID／システム日付表示
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "失敗", ex.Message)
        Finally
            BatchLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            BatchLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")

            Me.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.ToString)
        Finally
            BatchLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

    Private Sub btnKousin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKousin.Click
        Try
            BatchLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "更新(開始)", "成功", "")

            Dim mLogin = New ClsLogin

            ' ユーザ権限チェック
            Select Case mLogin.PasswordCheck(LW.UserID, txtUserName.Text, txtPassword.Text, txtPassword2.Text)
                Case 0
                    '正常終了時のメッセージについて設計書記載なし！#############################################################
                    MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    BatchLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "更新", "成功", "")
                    '###########################################################################################################
                Case 1
                    txtUserName.Focus()
                Case 2
                    txtPassword.Focus()
                Case 3
                    txtPassword2.Focus()
                Case Else
                    txtUserName.Focus()
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "更新", "失敗", ex.Message)
        Finally
            BatchLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "更新(終了)", "成功", "")
        End Try
    End Sub

    '全角文字の排除
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtUserName.Validating, txtPassword.Validating, txtPassword2.Validating
        Dim Ret As String = ""

        Try
            With CType(sender, TextBox)
                If Not .Text = Nothing AndAlso .Text.Length > 0 Then

                    For Idx As Integer = 0 To .Text.Length - 1 Step 1

                        Dim Temp As String = .Text.Substring(Idx, 1)

                        Dim JISEncoding As Encoding = Encoding.GetEncoding("SHIFT-JIS")
                        Dim onByte() As Byte = JISEncoding.GetBytes(Temp)

                        Do While onByte.Length > 1
                            Temp = Temp.Substring(0, Temp.Length - 1)
                            onByte = JISEncoding.GetBytes(Temp)
                        Loop

                        Ret &= Temp
                    Next Idx
                End If

                .Text = Ret
            End With

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "文字チェック", "失敗", ex.Message)
        End Try
    End Sub

#Region "関数がPublicではないため、呼び出せない。とりあえずClsLoginのコピーを入れておく"
    Public Class ClsLogin

        Private BatchLog As New BatchLOG("ClsLogin", "ログイン")

        'ユーザーＩＤマスタ構造体
        Private Structure stUIDMAST                 ' ユーザーＩＤマスタ
            Public LOGINID_U As String               ' ユーザー名
            Public PASSWORD_U As String             ' パスワード
            Public KENGEN_U As String               ' 権限
            Public PASSWORD_1S_U As String         ' １世代前パスワード
            Public PASSWORD_DATE_U As String        ' パスワード更新日付
            Public SAKUSEI_DATE_U As String         ' 作成日付
            Public KOUSIN_DATE_U As String          ' 更新日付
        End Structure
        Private UIDMAST As stUIDMAST


        Private OraDB As MyOracle

        '機能   :   ログインチェック
        '
        '引数   :   
        '
        '戻り値 :  0 - 成功，1 - ユーザＩＤエラー, 2 - パスワードエラー
        '
        '備考   :   
        '
        Public Function LoginCheck(ByVal userid As String, ByVal password As String) As Integer
            Dim nRet As Integer

            Try
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログインチェック(開始)", "成功", "")

                If OraDB Is Nothing Then
                    OraDB = New MyOracle
                End If

                OraDB.BeginTrans()

                nRet = CheckUIDMAST(userid, password)
                If nRet <> 0 And nRet <> 2 Then
                    OraDB.Rollback()
                Else
                    OraDB.Commit()
                End If

            Catch ex As Exception
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログインチェック(終了)", "失敗", ex.Message)
                nRet = 1

            Finally
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログインチェック(終了)", "成功", "")

            End Try

            Return nRet
        End Function

        '機能   :   パスワード変更チェック
        '
        '引数   :   
        '
        '戻り値 :  0 - 成功，1 - ユーザＩＤエラー, 2 - パスワードエラー
        '
        '備考   :   
        '
        Protected Friend Function PasswordCheck(ByVal userid As String, ByVal oldpass As String, ByVal newpass As String, ByVal exactpass As String) As Integer
            Dim nRet As Integer

            Try
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログインチェック(開始)", "成功", "")

                If OraDB Is Nothing Then
                    OraDB = New MyOracle
                End If

                OraDB.BeginTrans()

                nRet = CheckPassword(userid, oldpass, newpass, exactpass)
                If nRet <> 0 Then
                    OraDB.Rollback()
                Else
                    OraDB.Commit()
                End If

            Catch ex As Exception
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログインチェック", "失敗", ex.Message)
                nRet = 1

            Finally
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログインチェック(終了)", "成功", "")

            End Try

            Return nRet

        End Function

        Private Function CheckUIDMAST(ByVal userid As String, ByVal password As String) As Integer
            Dim SQL As String
            Dim OraReader As New MyOracleReader(OraDB)

            Try
                userid = userid.TrimEnd
                password = password.TrimEnd
                BatchLog.UserID = userid
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログイン認証(開始)", "成功", "")

                ' ユーザIDマスタ取得
                If GetUIDMAST(userid) > 0 Then
                    Return 1
                End If

                If password.CompareTo(UIDMAST.PASSWORD_U) <> 0 Then
                    'ユーザIDマスタ「パスワード」と画面「パスワード」が一致しない場合
                    'Msg005:パスワードが一致しません。
                    MessageBox.Show(MSG0006W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログイン認証(終了)", "成功", "")
                    Return 2
                Else
                    If UIDMAST.PASSWORD_DATE_U = "" OrElse UIDMAST.PASSWORD_U = "" Then
                        'ユーザIDマスタ「パスワード」　＝　画面「パスワード」が一致している場合、
                        'かつ　ユーザIDマスタ「パスワード更新日付」が空の場合

                        MessageBox.Show(MSG0005W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                        Return 1
                    End If

                    Try
                        Dim sCompDate As String = ""

                        'ユーザIDマスタ「パスワード」　＝　画面「パスワード」が一致している場合、
                        'かつ　ユーザIDマスタ「パスワード更新日付」から100日経過している場合
                        SQL = "SELECT SYSDATE - 100 HIDUKE FROM DUAL"
                        If OraReader.DataReader(SQL) = True Then
                            sCompDate = OraReader.GetDate(0).ToString("yyyyMMdd")
                        End If
                        OraReader.Close()

                    Catch ex As Exception
                        MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログイン認証", "失敗", ex.Message)
                        Return 1
                    End Try

                    Try
                        Dim sCompDate As String = ""

                        'ユーザIDマスタ「パスワード」　＝　画面「パスワード」が一致している場合、
                        'かつ　ユーザIDマスタ「パスワード更新日付」から９３日経過している場合
                        SQL = "SELECT SYSDATE - 93 HIDUKE FROM DUAL"
                        If OraReader.DataReader(SQL) = True Then
                            sCompDate = OraReader.GetDate(0).ToString("yyyyMMdd")
                        End If
                        OraReader.Close()

                        If UIDMAST.PASSWORD_DATE_U < sCompDate Then
                            'レコード内パスワード更新日付 <  本日日付 - 93日  ← パスワード更新日付より93日以上経ったか判断
                            'パスワード更新日付+101日（日付の差異数を計算する）
                            Dim dAddPasswordCompDate As Date = ConvertDate(UIDMAST.PASSWORD_DATE_U).AddDays(101)
                            Dim nNissu As Integer

                            '本日日付（サーバー内）参照  Date.Nowはだめ
                            SQL = "SELECT SYSDATE HIDUKE FROM DUAL"

                            If OraReader.DataReader(SQL) = True Then
                                nNissu = dAddPasswordCompDate.Subtract(OraReader.GetDate(0)).Days
                            End If

                            OraReader.Close()

                            'Msg011:パスワードの有効期限が切れるまで。
                            MessageBox.Show(String.Format(MSG0012I, nNissu), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If

                    Catch ex As Exception
                        MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログイン認証", "失敗", ex.Message)
                        Return 1
                    End Try

                    Return 0
                End If

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログイン認証", "失敗", ex.Message)
                Return 1
            Finally
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ログイン認証(終了)", "成功", "")

            End Try

        End Function

        Protected Friend Function CheckPassword(ByVal userid As String, ByVal oldpass As String, ByVal newpass As String, ByVal exactpass As String) As DialogResult
            Dim SQL As String

            Try
                Dim newpassW As String, exactpassW As String
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "パスワードチェック(開始)", "成功", "")

                If oldpass = "" Then
                    MessageBox.Show(MSG0007W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 1
                End If

                If newpass = "" Then
                    MessageBox.Show(MSG0008W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 2
                End If

                If exactpass = "" Then
                    MessageBox.Show(MSG0009W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 3
                End If

                If UIDMAST.LOGINID_U Is Nothing Then
                    ' ユーザIDマスタ取得
                    Call GetUIDMAST(userid)
                End If

                '「登録」押下時にチェック処理を行う。
                If oldpass.CompareTo(UIDMAST.PASSWORD_U) <> 0 Then
                    '旧パスワードがレコード内パスワードと一致しない場合
                    MessageBox.Show(MSG0010W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 1
                End If

                newpassW = newpass.Trim.Replace(" ", "")

                If newpass.Length <> newpassW.Length Then
                    MessageBox.Show(MSG0341W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 2
                End If

                exactpassW = exactpass.Trim.Replace(" ", "")

                If exactpass.Length <> exactpassW.Length Then
                    MessageBox.Show(MSG0341W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 3
                End If

                If newpass.Length < 6 Then
                    '新パスワードが6ケタ未満
                    MessageBox.Show(MSG0229W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 2
                End If

                If newpass.CompareTo(oldpass) = 0 Then
                    '新パスワードが旧パスワードと一致する場合
                    MessageBox.Show(MSG0012W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 2
                End If

                If newpass.CompareTo(exactpass) <> 0 Then
                    '新パスワードと新パスワード確認が一致しない場合
                    MessageBox.Show(MSG0011W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 3
                End If

                If newpass.CompareTo(oldpass) <> 0 Then
                Else
                    MessageBox.Show(MSG0013W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 2
                End If

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "パスワードチェック", "失敗", ex.Message)
                Return 1
            Finally
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "パスワードチェック(終了)", "成功", "")

            End Try

            If MessageBox.Show(MSG0005I, _
                               msgTitle, _
                               MessageBoxButtons.YesNo, _
                               MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Return 9
            End If

            'エラーチェックの結果が正常の場合、更新処理を行いログイン画面（KFJLGIN0001)へ遷移する。
            '正常時アップデート
            Try
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "パスワード変更(開始)", "成功", "")

                SQL = "UPDATE UIDMAST SET"
                SQL &= " PASSWORD_U    = " & SQ(newpass)                    '新パスワードをレコードに設定
                SQL &= ",PASSWORD_1S_U = " & SQ(UIDMAST.PASSWORD_U)         '旧パスワードを1世代前パスワードに設定
                SQL &= ",PASSWORD_DATE_U  = TO_CHAR(SYSDATE,'YYYYMMDD') "   'パスワード更新日付に現在の日付を設定
                SQL &= ",KOUSIN_DATE_U  = TO_CHAR(SYSDATE,'YYYYMMDD') "     '更新日付に現在の日付を設定
                SQL &= " WHERE"
                SQL &= " LOGINID_U = " & SQ(UIDMAST.LOGINID_U)

                If OraDB.ExecuteNonQuery(SQL) = 1 Then
                    Return 0
                End If

                Return 1

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "パスワード変更", "失敗", ex.Message)
                Return 1

            Finally
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "パスワード変更(終了)", "成功", "")

            End Try

        End Function

        Private Function GetUIDMAST(ByVal userid As String) As Integer
            Dim SQL As String
            Dim OraReader As New MyOracleReader(OraDB)

            ' ユーザIDマスタの該当データを取得する。
            Try
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ユーザ参照(開始)", "成功", "")

                SQL = "SELECT"
                SQL &= "  LOGINID_U"             'ユーザー名
                SQL &= ", PASSWORD_U"           'パスワード
                SQL &= ", KENGEN_U"             '権限
                SQL &= ", PASSWORD_1S_U"        '１世代前パスワード
                SQL &= ", PASSWORD_DATE_U"      'パスワード変更日付
                SQL &= ", SAKUSEI_DATE_U"       '作成日付
                SQL &= ", KOUSIN_DATE_U"        '更新日付
                SQL &= " FROM UIDMAST"          'ユーザIDマスタ
                SQL &= " WHERE LOGINID_U = " & SQ(userid)    'キー = ユーザーID(画面入力値)

                If OraReader.DataReader(SQL) = False Then
                    'ユーザIDマスタに該当レコードが存在しない場合
                    ' ログインＩＤに誤りがあります。
                    '   MessageBox.Show(MSG001, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 1
                End If

                With UIDMAST
                    .LOGINID_U = OraReader.GetString("LOGINID_U")
                    .PASSWORD_U = OraReader.GetString("PASSWORD_U")
                    .PASSWORD_1S_U = OraReader.GetString("PASSWORD_1S_U")
                    .PASSWORD_DATE_U = OraReader.GetString("PASSWORD_DATE_U")
                    .KOUSIN_DATE_U = OraReader.GetString("KOUSIN_DATE_U")
                End With

                OraReader.Close()
                Return 0

            Catch ex As Exception
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ユーザ参照", "失敗", ex.Message)
                Return 1

            Finally
                BatchLog.Write(BatchLog.UserID, "0000000000-00", "00000000", "ユーザ参照(終了)", "成功", "")

            End Try
        End Function

        Protected Overrides Sub Finalize()
            If Not OraDB Is Nothing Then
                OraDB.Close()
            End If
            OraDB = Nothing

            MyBase.Finalize()
        End Sub
    End Class
#End Region
End Class
