Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text

Public Class KFGOTHR090

    Private gstrSYORI_R As String = ""

    '変数
    Private strTUUBAN As String
    Private lngLength As Long
    Private strRet As String
    Private lngITEM_CNT As Long
    <VBFixedString(14)> Private strTIME_STAMP_W As String

    '画面LOAD時
    Private Sub KFGOTHR090_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        STR_SYORI_NAME = "資金決済データ一括取消"
        STR_COMMAND = "Form_Load"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = ""

        Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
        MainLog = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

        '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
        Call GSUB_CONNECT()
        '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

        txtSyoriDateY.Text = Format(Now, "yyyy")
        txtSyoriDateM.Text = Format(Now, "MM")
        txtSyoriDateD.Text = Format(Now, "dd")
        '実行ボタンEnabled
        btnAction.Enabled = False
        'INIファイル読み込み
        gstrSYORI_R = "資金決済用リエンタＦＤ作成処理取消"

    End Sub

    '検索ボタン押下時
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If fn_check_TEXT() = False Then
            Exit Sub
        End If

        If fn_select_G_SCHMAST(0) = False Then
            MessageBox.Show("検索処理中にエラーが発生しました[ｽｹｼﾞｭｰﾙﾏｽﾀ]" & vbCrLf _
            & Str(Err.Number) & " :" & Err.Description, gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
    End Sub

    '実行ボタン押下時
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        strTIME_STAMP_W = cmbTimeStamp.SelectedItem 'コンボボックスから値取得
        strTIME_STAMP_W = strTIME_STAMP_W.Substring(0, 4) & strTIME_STAMP_W.Substring(5, 2) & strTIME_STAMP_W.Substring(8, 2) _
                        & strTIME_STAMP_W.Substring(11, 2) & strTIME_STAMP_W.Substring(14, 2) & strTIME_STAMP_W.Substring(17, 2)

        If MessageBox.Show("資金決済リエンタＦＤ作成処理を取消します。" & vbCrLf _
                      & "処理を継続しますか？", gstrSYORI_R, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
            Exit Sub
        Else
            If fn_update_G_SCHMAST() = False Then
                MessageBox.Show("取消処理異常終了", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                GSUB_LOG_OUT("", "", gstrSYORI_R, "資金決済リエンタFD作成処理取消", "失敗", "タイムスタンプ：" & strTIME_STAMP_W & " " & Err.Description)
                Exit Sub
            Else
                MessageBox.Show("取消処理正常終了" & vbCrLf & _
                 "ｽｹｼﾞｭｰﾙﾏｽﾀ更新先数 = " & lngITEM_CNT, gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Information)
                GSUB_LOG_OUT("", "", gstrSYORI_R, "資金決済リエンタFD作成処理取消", "成功", "タイムスタンプ：" & strTIME_STAMP_W & " " & Err.Description)
                If fn_select_G_SCHMAST(1) = False Then
                    MessageBox.Show("検索処理中にエラーが発生しました[ｽｹｼﾞｭｰﾙﾏｽﾀ]" & vbCrLf _
                    & Str(Err.Number) & " :" & Err.Description, gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            End If
        End If

    End Sub

    '終了ボタン押下時
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Me.Close()
    End Sub

    Private Function fn_check_TEXT() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :検索ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/09/27
        'UPDATE         :
        '============================================================================
        Dim ret As Boolean = False

        Try
            '処理年チェック
            If txtSyoriDateY.Text.Length <> 0 Then
            Else
                Call GSUB_MESSAGE_WARNING("処理年を入力してください")
                txtSyoriDateY.Focus()
                Exit Try
            End If

            '処理月チェック
            If txtSyoriDateM.Text.Length <> 0 Then
            Else
                Call GSUB_MESSAGE_WARNING("処理月を入力してください")
                txtSyoriDateM.Focus()
                Exit Try
            End If

            '処理日チェック
            If txtSyoriDateD.Text.Length <> 0 Then
            Else
                Call GSUB_MESSAGE_WARNING("処理日を入力してください")
                txtSyoriDateD.Focus()
                Exit Try
            End If

            '日付チェック
            Dim WORK_DATE As String
            WORK_DATE = txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text
            If Not IsDate(WORK_DATE) Then
                Call GSUB_MESSAGE_WARNING("処理年月日が正しくありません")
                txtSyoriDateY.Focus()
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            Call GSUB_MESSAGE_WARNING("入力チェックに失敗しました")
        End Try

        Return ret

    End Function

    Function fn_select_G_SCHMAST(ByVal aintFLG As Integer) As Boolean
        '============================================================================
        'NAME           :fn_select_G_SCHMAST
        'Parameter      :aintFLG=0:検索ボタン押下時　1:画面再表示時
        'Description    :G_SCHMASTから決済タイムスタンプ取得
        'Return         :True=OK,False=NG
        'Create         :2004/09/27
        'UPDATE         :
        '============================================================================
        Dim strJOKEN1 As String '抽出条件1
        Dim strJOKEN2 As String '抽出条件2
        Dim strTIME_STAMP As String

        strJOKEN1 = txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text & "000000"
        strJOKEN2 = txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text & "999999"

        fn_select_G_SCHMAST = False

        '＜ｽｹｼﾞｭｰﾙﾏｽﾀﾁｪｯｸ＞
        Dim SQL As New StringBuilder(128)
        SQL.Append("SELECT DISTINCT TIME_STAMP_S FROM G_SCHMAST")
        SQL.Append(" WHERE TIME_STAMP_S BETWEEN '" & strJOKEN1 & "'")
        SQL.Append(" AND '" & strJOKEN2 & "'")
        SQL.Append(" AND KESSAI_FLG_S = '1'")

        gdbcCONNECT.ConnectionString = STR_CONNECTION
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = SQL.ToString
        gdbCOMMAND.Connection = gdbcCONNECT

        gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ

        '-----------------------------------------
        'コンボボックスにリストを追加する
        '-----------------------------------------
        cmbTimeStamp.Items.Clear()
        cmbTimeStamp.BeginUpdate() '再描画しないようにする
        lngITEM_CNT = 0
        While (gdbrREADER.Read)
            strTIME_STAMP = CStr(gdbrREADER.Item("TIME_STAMP_S")).Trim
            strTIME_STAMP = strTIME_STAMP.Substring(0, 4) & "/" & strTIME_STAMP.Substring(4, 2) & "/" & strTIME_STAMP.Substring(6, 2) & Space(1) _
                            & strTIME_STAMP.Substring(8, 2) & ":" & strTIME_STAMP.Substring(10, 2) & ":" & strTIME_STAMP.Substring(12, 2)
            cmbTimeStamp.Items.Add(strTIME_STAMP)
            lngITEM_CNT = lngITEM_CNT + 1
        End While
        cmbTimeStamp.EndUpdate() '再描画するようにする

        gdbcCONNECT.Close()

        If Err.Number = 0 Then
            fn_select_G_SCHMAST = True
            If lngITEM_CNT = 0 Then
                If aintFLG = 0 Then
                    MessageBox.Show("該当スケジュールが存在しません", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                btnAction.Enabled = False
                txtSyoriDateY.Focus()
            Else
                btnAction.Enabled = True
                cmbTimeStamp.SelectedIndex = 0
                cmbTimeStamp.Focus()
            End If
        Else
            btnAction.Enabled = False
            txtSyoriDateY.Focus()
        End If
    End Function

    Function fn_update_G_SCHMAST() As Boolean
        '============================================================================
        'NAME           :fn_update_G_SCHMAST
        'Parameter      :
        'Description    :G_SCHMAST更新
        'Return         :True=OK,False=NG
        'Create         :2004/09/27
        'UPDATE         :
        '============================================================================
        fn_update_G_SCHMAST = False
        '＜ｽｹｼﾞｭｰﾙﾏｽﾀ該当データ数＞
        Dim SQL As New StringBuilder(128)
        SQL.Append("SELECT * FROM G_SCHMAST")
        SQL.Append(" WHERE TIME_STAMP_S = '" & strTIME_STAMP_W & "'")

        gdbcCONNECT.ConnectionString = STR_CONNECTION
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = SQL.ToString
        gdbCOMMAND.Connection = gdbcCONNECT

        gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ
        lngITEM_CNT = 0
        While (gdbrREADER.Read)
            lngITEM_CNT = lngITEM_CNT + 1
        End While
        gdbcCONNECT.Close()

        'スケジュールマスタ・該当データ更新
        SQL.Length = 0
        SQL.Append("UPDATE  G_SCHMAST SET ")
        SQL.Append(" KESSAI_FLG_S = '0'")
        SQL.Append(" WHERE TIME_STAMP_S = '" & strTIME_STAMP_W & "'")

        gdbcCONNECT.ConnectionString = STR_CONNECTION
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = SQL.ToString
        gdbCOMMAND.Connection = gdbcCONNECT
        gdbTRANS = gdbcCONNECT.BeginTransaction
        gdbCOMMAND.Transaction = gdbTRANS

        Try
            gdbCOMMAND.ExecuteNonQuery()
            gdbTRANS.Commit()
        Catch ex As Exception
            gdbTRANS.Rollback()
            gdbcCONNECT.Close()
            fn_update_G_SCHMAST = False
            Exit Function
        End Try
        gdbcCONNECT.Close()

        If Err.Number = 0 Then
            fn_update_G_SCHMAST = True
        End If
    End Function
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
