Imports System
Imports System.IO
Imports System.Windows.Forms

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

    'エラーメッセージ

    Private OraDB As MyOracle

    '2017/11/13 タスク）西野 ADD (標準版修正(№174)) -------------------- START
    Private PASS_SEDAI_CHK As Boolean = True

    Public Sub New()
        'パスワードの世代チェックを行うかINIファイルを取得する
        Dim WK As String = ""
        Dim MSG As String = ""

        Try
            WK = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "PASS_SEDAI_CHK")
            If WK.ToUpper = "ERR" OrElse WK = "" Then
                '項目なし、未設定の場合は、初期値のまま（世代チェックを行う）
            Else
                If WK.Trim = "0" Then PASS_SEDAI_CHK = False
            End If

        Catch ex As Exception
            BatchLog.Write_Err("0000000000-00", "00000000", "ClsLogin生成", "失敗", ex)
        End Try
    End Sub
    '2017/11/13 タスク）西野 ADD (標準版修正(№174)) -------------------- END

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

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Try
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ログインチェック(開始)", "成功", "")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "ログインチェック", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

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
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ログインチェック(終了)", "失敗", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "ログインチェック", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            nRet = 1

        Finally
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ログインチェック(終了)", "成功", "")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "ログインチェック", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

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

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Try
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ログインチェック(開始)", "成功", "")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "パスワード変更チェック", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

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
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ログインチェック", "失敗", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "パスワード変更チェック", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            nRet = 1

        Finally
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ログインチェック(終了)", "成功", "")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "パスワード変更チェック", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        End Try

        Return nRet

    End Function

    Private Function CheckUIDMAST(ByVal userid As String, ByVal password As String) As Integer
        Dim SQL As String
        Dim OraReader As New MyOracleReader(OraDB)

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        ' 2015/12/11 タスク）綾部 ADD 【PG】UI_B-14-13(RSV2対応) -------------------- START
        Dim INI_RSV2_PASS_LIMIT As String = GetRSKJIni("RSV2_V1.0.0", "PASS_LIMIT")
        Dim INI_RSV2_PASS_CHANGE As String = GetRSKJIni("RSV2_V1.0.0", "PASS_CHANGE")
        ' 2015/12/11 タスク）綾部 ADD 【PG】UI_B-14-13(RSV2対応) -------------------- END

        Try
            userid = userid.TrimEnd
            password = password.TrimEnd
            BatchLog.UserID = userid
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ログイン認証(開始)", "成功", "")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "ログイン認証", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

            ' ユーザIDマスタ取得
            If GetUIDMAST(userid) > 0 Then
                Return 1
            End If

            If password.CompareTo(UIDMAST.PASSWORD_U) <> 0 Then
                'ユーザIDマスタ「パスワード」と画面「パスワード」が一致しない場合
                'MSG0006W:パスワードが一致しません。
                MessageBox.Show(MSG0006W, "ログイン(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
                'BatchLog.Write("0000000000-00", "00000000", "ログイン認証", "失敗", "パスワードエラー")
                BatchLog.Write_Err("0000000000-00", "00000000", "ログイン認証", "失敗", "パスワードエラー")
                '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
                Return 2
            Else
                If UIDMAST.PASSWORD_DATE_U = "" OrElse UIDMAST.PASSWORD_U = "" Then
                    'ユーザIDマスタ「パスワード」　＝　画面「パスワード」が一致している場合、
                    'かつ　ユーザIDマスタ「パスワード更新日付」が空の場合

                    'MSG0007W:パスワードが未設定です。設定してください。
                    MessageBox.Show(MSG0007W, "ログイン(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    ' パスワード変更画面表示
                    Dim oFrm As New FrmPassChange(Me)
                    oFrm.UserID = userid
                    Call oFrm.ShowDialog()
                    If oFrm.DialogResult = DialogResult.OK Then
                        UIDMAST.PASSWORD_DATE_U = Calendar.Now.ToString("yyyyMMdd")
                        'Return 0
                    Else
                        Return 1
                    End If
                End If

                Try
                    Dim sCompDate As String = ""

                    ' 2015/12/11 タスク）綾部 CHG 【PG】UI_B-14-13(RSV2対応) -------------------- START
                    ''ユーザIDマスタ「パスワード」　＝　画面「パスワード」が一致している場合、
                    ''かつ　ユーザIDマスタ「パスワード更新日付」から100日経過している場合
                    'SQL = "SELECT SYSDATE - 100 HIDUKE FROM DUAL"
                    ' ユーザIDマスタ「パスワード更新日付」から指定日数([RSV2_V1.0.0]-[PASS_LIMIT])が経過している場合
                    SQL = "SELECT SYSDATE - " & INI_RSV2_PASS_LIMIT & " HIDUKE FROM DUAL"
                    ' 2015/12/11 タスク）綾部 CHG 【PG】UI_B-14-13(RSV2対応) -------------------- END

                    If OraReader.DataReader(SQL) = True Then
                        sCompDate = OraReader.GetDate(0).ToString("yyyyMMdd")
                    End If
                    OraReader.Close()

                    If sCompDate >= UIDMAST.PASSWORD_DATE_U Then
                        '本日日付 - 100日 > レコード内パスワード更新日付  ← 最終ログイン日より100日以上経ったか判断

                        'MSG0011I:パスワードの有効期限がきれました。パスワードを設定してください。
                        MessageBox.Show(MSG0011I, "ログイン(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Information)

                        ' パスワード変更画面表示
                        Dim oFrm As New FrmPassChange(Me)
                        oFrm.UserID = userid
                        Call oFrm.ShowDialog()
                        If oFrm.DialogResult = DialogResult.OK Then
                            'Return 0
                            UIDMAST.PASSWORD_DATE_U = Calendar.Now.ToString("yyyyMMdd")
                        Else
                            Return 1
                        End If
                    End If

                Catch ex As Exception
                    MessageBox.Show(MSG0006E, "ログイン(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
                    'BatchLog.Write("0000000000-00", "00000000", "ログイン認証", "失敗", ex.Message)
                    BatchLog.Write_Err("0000000000-00", "00000000", "ログイン認証", "失敗", ex)
                    '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
                    Return 1
                End Try

                Try
                    Dim sCompDate As String = ""

                    ' 2015/12/11 タスク）綾部 CHG 【PG】UI_B-14-13(RSV2対応) -------------------- START
                    ''ユーザIDマスタ「パスワード」　＝　画面「パスワード」が一致している場合、
                    ''かつ　ユーザIDマスタ「パスワード更新日付」から９３日経過している場合
                    'SQL = "SELECT SYSDATE - 93 HIDUKE FROM DUAL"
                    ' ユーザIDマスタ「パスワード更新日付」から変更日数([RSV2_V1.0.0]-[PASS_CHANGE])が経過している場合
                    SQL = "SELECT SYSDATE - " & INI_RSV2_PASS_CHANGE & " HIDUKE FROM DUAL"
                    ' 2015/12/11 タスク）綾部 CHG 【PG】UI_B-14-13(RSV2対応) -------------------- END

                    If OraReader.DataReader(SQL) = True Then
                        sCompDate = OraReader.GetDate(0).ToString("yyyyMMdd")
                    End If
                    OraReader.Close()

                    If UIDMAST.PASSWORD_DATE_U < sCompDate Then
                        ' 2015/12/11 タスク）綾部 CHG 【PG】UI_B-14-13(RSV2対応) -------------------- START
                        ''レコード内パスワード更新日付 <  本日日付 - 93日  ← パスワード更新日付より93日以上経ったか判断
                        ''パスワード更新日付+101日（日付の差異数を計算する）
                        'Dim dAddPasswordCompDate As Date = ConvertDate(UIDMAST.PASSWORD_DATE_U).AddDays(101)
                        'レコード内パスワード更新日付 <  本日日付 - 93日  ← パスワード更新日付より変更日数以上経ったか判断
                        'パスワード更新日付+(指定日数+1)（日付の差異数を計算する）
                        Dim PASS_LIMIT_Days As Integer = CInt(INI_RSV2_PASS_LIMIT) + 1
                        Dim dAddPasswordCompDate As Date = ConvertDate(UIDMAST.PASSWORD_DATE_U).AddDays(PASS_LIMIT_Days)
                        ' 2015/12/11 タスク）綾部 CHG 【PG】UI_B-14-13(RSV2対応) -------------------- END
                        Dim nNissu As Integer

                        '本日日付（サーバー内）参照  Date.Nowはだめ
                        SQL = "SELECT SYSDATE HIDUKE FROM DUAL"

                        If OraReader.DataReader(SQL) = True Then
                            nNissu = dAddPasswordCompDate.Subtract(OraReader.GetDate(0)).Days
                        End If

                        OraReader.Close()

                        'MSG0012I:パスワードの有効期限が切れるまで。
                        MessageBox.Show(String.Format(MSG0012I, nNissu), "ログイン(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If

                Catch ex As Exception
                    MessageBox.Show(MSG0006E, "ログイン(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
                    'BatchLog.Write("0000000000-00", "00000000", "ログイン認証", "失敗", ex.Message)
                    BatchLog.Write_Err("0000000000-00", "00000000", "ログイン認証", "失敗", ex)
                    '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
                    Return 1
                End Try

                Return 0
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, "ログイン(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ログイン認証", "失敗", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "ログイン認証", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return 1
        Finally
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ログイン認証(終了)", "成功", "")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "ログイン認証", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        End Try

    End Function

    Protected Friend Function CheckPassword(ByVal userid As String, ByVal oldpass As String, ByVal newpass As String, ByVal exactpass As String) As DialogResult
        Dim SQL As String

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        Try
            Dim newpassW As String, exactpassW As String
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "パスワードチェック(開始)", "成功", "")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "パスワードチェック", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

            If UIDMAST.LOGINID_U Is Nothing Then
                ' ユーザIDマスタ取得
                Call GetUIDMAST(userid)
            End If

            '「登録」押下時にチェック処理を行う。
            If oldpass.CompareTo(UIDMAST.PASSWORD_U) <> 0 Then
                '旧パスワードがレコード内パスワードと一致しない場合
                'MSG0010W:旧パスワードに誤りがあります。
                MessageBox.Show(MSG0010W, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return 1
            End If

            newpassW = newpass.Trim.Replace(" ", "")

            If newpass.Length <> newpassW.Length Then
                MessageBox.Show(MSG0341W, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return 2
            End If

            exactpassW = exactpass.Trim.Replace(" ", "")

            If exactpass.Length <> exactpassW.Length Then
                MessageBox.Show(MSG0341W, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return 3
            End If

            If newpass.CompareTo(oldpass) = 0 Then
                '新パスワードが旧パスワードと一致する場合
                'MSG0012W:旧パスワードと同じパスワードは指定できません。
                MessageBox.Show(MSG0012W, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return 2
            End If

            If newpass.CompareTo(exactpass) <> 0 Then
                '新パスワードと新パスワード確認が一致しない場合
                'MSG0011W:新パスワードに誤りがあります。
                MessageBox.Show(MSG0011W, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return 2
            End If

            '2017/11/13 タスク）西野 CHG (標準版修正(№174)) -------------------- START
            If PASS_SEDAI_CHK = True Then
                If newpass.CompareTo(UIDMAST.PASSWORD_1S_U) = 0 Then
                    '新パスワードが世代パスワードと一致した場合
                    'MSG0013W:過去に登録したパスワードは使用できません。
                    MessageBox.Show(MSG0013W, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return 2
                End If
            End If
            'If newpass.CompareTo(UIDMAST.PASSWORD_1S_U) = 0 Then
            '    '新パスワードが世代パスワードと一致した場合
            '    'MSG0013W:過去に登録したパスワードは使用できません。
            '    MessageBox.Show(MSG0013W, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Return 2
            'End If
            '2017/11/13 タスク）西野 CHG (標準版修正(№174)) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "パスワードチェック", "失敗", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "パスワードチェック", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return 1
        Finally
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "パスワードチェック(終了)", "成功", "")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "パスワードチェック", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        End Try

        'エラーチェックの結果が正常の場合、更新処理を行いログイン画面（KFJLGIN0001)へ遷移する。
        '正常時アップデート
        Try
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "パスワード変更(開始)", "成功", "")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "パスワード変更", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

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
            MessageBox.Show(MSG0006E, "パスワード変更(KFUMAST050)", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "パスワード変更", "失敗", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "パスワード変更", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return 1

        Finally
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "パスワード変更(終了)", "成功", "")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "パスワード変更", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

        End Try

    End Function

    Private Function GetUIDMAST(ByVal userid As String) As Integer
        Dim SQL As String
        Dim OraReader As New MyOracleReader(OraDB)

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        ' ユーザIDマスタの該当データを取得する。
        Try
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ユーザ参照(開始)", "成功")
            sw = BatchLog.Write_Enter1("0000000000-00", "00000000", "ユーザ参照", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

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
                MessageBox.Show(MSG0006W, "ログイン(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Error)
                '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
                'BatchLog.Write("0000000000-00", "00000000", "ユーザ参照", "失敗", "ユーザ情報マスタ未登録")
                BatchLog.Write_Err("0000000000-00", "00000000", "ユーザ参照", "失敗", "ユーザ情報マスタ未登録")
                '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
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
            MessageBox.Show(MSG0006E, "ログイン(KFULGIN010)", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ユーザ参照", "失敗", ex.Message)
            BatchLog.Write_Err("0000000000-00", "00000000", "ユーザ参照", "失敗", ex)
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***
            Return 1

        Finally
            '*** Str Upd 2015/12/01 SO)荒木 for ログ強化 ***
            'BatchLog.Write("0000000000-00", "00000000", "ユーザ参照(終了)", "成功")
            BatchLog.Write_Exit1(sw, "0000000000-00", "00000000", "ユーザ参照", "")
            '*** End Upd 2015/12/01 SO)荒木 for ログ強化 ***

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
