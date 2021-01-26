Imports System
Imports System.Text
Imports MenteCommon
Imports System.IO

Public Class KFWMAIN020

#Region "宣言"

    ' 出力プリンタ
    Public PRINTERNAME As String = ""

    Private MainLog As New CASTCommon.BatchLOG("KFWMAIN020", "WEB伝送ログ一覧印刷画面")
    Const msgTitle As String = "WEB伝送ログ一覧印刷画面(KFWMAIN020)"

    Private strJyusinDate As String 'WEB伝送受信日

    '共通イベントコントロール
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFWMAIN020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            MainLog.UserID = GCom.GetUserID
            MainLog.Write("0000000000-00", "00000000", "(ロード)開始", "成功")

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            txtJyusinDateY.Text = Format(System.DateTime.Today, "yyyy")
            txtJyusinDateM.Text = Format(System.DateTime.Today, "MM")
            txtJyusinDateD.Text = Format(System.DateTime.Today, "dd")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(ロード)", "失敗", ex.Message)
        Finally
            MainLog.Write("0000000000-00", "00000000", "(ロード)終了", "成功")
        End Try
    End Sub

#End Region

#Region "ボタン"

    '印刷ボタン
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click

        Try
            MainLog.Write("0000000000-00", "00000000", "(印刷)開始", "成功")
            '2013/07/03 saitou 蒲郡信金 MODIFY ----------------------------------------------->>>>
            'メッセージはチェックが正常だった場合に出す。
            'If MessageBox.Show(MSG0013I.Replace("{0}", "WEB伝送ログ一覧"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            '    Exit Sub
            'End If
            '2013/07/03 saitou 蒲郡信金 MODIFY -----------------------------------------------<<<<

            '------------------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------------------

            If fn_check_TEXT() = False Then Exit Sub

            strJyusinDate = txtJyusinDateY.Text & txtJyusinDateM.Text & txtJyusinDateD.Text

            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            '2013/07/03 saitou 蒲郡信金 MODIFY ----------------------------------------------->>>>
            '上記より移動
            If MessageBox.Show(MSG0013I.Replace("{0}", "WEB伝送ログ一覧"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If
            '2013/07/03 saitou 蒲郡信金 MODIFY -----------------------------------------------<<<<

            '---------------------------------------------------
            '印刷実行
            '---------------------------------------------------
            If fn_Print() = False Then
                Exit Sub
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(印刷)終了", "失敗", ex.Message)
        Finally
            MainLog.Write("0000000000-00", "00000000", "(印刷)終了", "成功")
        End Try

    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLog.Write("0000000000-00", "00000000", "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(クローズ)", "失敗", ex.Message)
        Finally
            MainLog.Write("0000000000-00", "00000000", "(クローズ)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "テキストボックス"

    '基準年／登録月日
    Private Sub DateTextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtJyusinDateY.Validating, _
            txtJyusinDateM.Validating, _
            txtJyusinDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MainLog.Write("0000000000-00", "00000000", "(日付テキストボックス)", "失敗", ex.Message)
        End Try
    End Sub

#End Region

#Region "関数"
    Private Function fn_check_TEXT() As Boolean
        '============================================================================
        'NAME           :fn_check_TEXT
        'Parameter      :
        'Description    :印刷ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/09/10
        'Update         :
        '============================================================================
        Try

            '------------------------------------------------
            '振替年チェック
            '------------------------------------------------
            '必須チェック
            If txtJyusinDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '振替月チェック
            '------------------------------------------------
            '必須チェック
            If txtJyusinDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateM.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtJyusinDateM.Text >= 1 And txtJyusinDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '振替日チェック
            '------------------------------------------------
            '必須チェック
            If txtJyusinDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateD.Focus()
                Return False
            End If

            '桁数チェック
            If Not (txtJyusinDateD.Text >= 1 And txtJyusinDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = txtJyusinDateY.Text & "/" & txtJyusinDateM.Text & "/" & txtJyusinDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLog.Write("0000000000-00", "00000000", "(入力チェック)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
        End Try

    End Function

    Private Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :印刷ボタンを押下時にマスタ相関チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/15
        'Update         :
        '============================================================================
        fn_check_Table = False
        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        fn_check_Table = False
        Try
            Dim lngDataCNT As Long = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM WEB_RIREKIMAST")
            SQL.Append(" WHERE SAKUSEI_DATE_W ='" & strJyusinDate & "'")
            SQL.Append(" AND STATUS_KBN_W <> '2'")

            If OraReader.DataReader(SQL) = True Then
                lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
            Else
                MainLog.Write("0000000000-00", "00000000", "(テーブルチェック)", "失敗", "マスタ検索失敗")
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtJyusinDateY.Focus()
                Return False
            End If
            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateY.Focus()
                Return False
            End If
            fn_check_Table = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function

    Public Function fn_Print() As Boolean
        '============================================================================
        'NAME           :fn_CreateCSV
        'Parameter      :
        'Description    :帳票印刷用ＣＳＶファイル作成
        'Return         :
        'Create         :
        'Update         :
        '============================================================================
        Dim ErrMessage As String = ""
        Dim Param As String = ""

        Try
            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：ログイン名、受信日
            Param = gstrUSER_ID & "," & strJyusinDate

            Dim nRet As Integer = ExeRepo.ExecReport("KFWP001.EXE", Param)

            If nRet <> 0 Then
                '印刷失敗：戻り値に対応したエラーメッセージを表示する
                Select Case nRet
                    Case 1
                        ErrMessage = "パラメータの取得に失敗しました。"
                    Case 2
                        ErrMessage = "ログを参照してください。"
                    Case -1
                        ErrMessage = "ログを参照してください。"
                End Select

                MessageBox.Show(MSG0004E.Replace("{0}", "WEB伝送ログ一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Return False
            End If

            MessageBox.Show(MSG0014I.Replace("{0}", "WEB伝送ログ一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(WEB伝送ログ一覧印刷)", "失敗", ex.Message)
            Return False
        End Try

    End Function

#End Region

End Class


