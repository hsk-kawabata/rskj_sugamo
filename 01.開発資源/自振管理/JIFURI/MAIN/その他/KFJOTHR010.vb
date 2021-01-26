Option Explicit On
Option Strict On

Imports System
Imports System.IO
Imports System.Data.OracleClient
Imports CASTCommon

Public Class KFJOTHR010
#Region "宣言"
    Inherits System.Windows.Forms.Form

    Private BatchLog As New CASTCommon.BatchLOG("KFJOTHR010", "センターカットデータ作成取消画面")
    'Private Const ErrMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private Const ThisModuleName As String = "KFJOTHR010.vb"
    Private GCom As New MenteCommon.clsCommon

    Private Const msgTitle As String = "センターカットデータ作成取消画面(KFJOTHR010)"

#End Region

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了)開始", "成功", "")

            Me.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了)", "失敗", ex.Message)
            Me.Close()

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(終了)終了", "成功", "")

        End Try

    End Sub

    Private Sub KFJOTHR010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)開始", "成功", "")

            GCom.GetSysDate = Date.Now
            GCom.GetUserID = gstrUSER_ID

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '処理日にシステム日付を表示
            txtFuriDateY.Text = Format(System.DateTime.Today, "yyyy")
            txtFuriDateM.Text = Format(System.DateTime.Today, "MM")
            txtFuriDateD.Text = Format(System.DateTime.Today, "dd")

            '項目入力制御
            txtFuriDateY.Enabled = True
            txtFuriDateM.Enabled = True
            txtFuriDateD.Enabled = True
            btnSearch.Enabled = True
            btnAction.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)", "失敗", ex.Message)
            Me.Close()

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(ロード)終了", "成功", "")

        End Try
     
    End Sub

    '検索ボタン
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim REC As OracleDataReader = Nothing
        'Dim REC As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(検索)開始", "成功", "")

            Cursor.Current = Cursors.WaitCursor()

            'テキストボックスの入力チェック
            Dim onDate As New Date
            Dim strDate As String = "00000000"
            Dim CTL As TextBox = Nothing
            If Not FN_CHECK_TEXT(strDate) Then
                Return
            End If

            'コンボボックス設定
            cmbTimeStamp.Items.Clear()
            cmbTimeStamp.Text = ""

            Dim SQL As String = "SELECT DISTINCT JIFURI_TIME_STAMP_S FROM SCHMAST"
            SQL &= " WHERE SUBSTR(JIFURI_TIME_STAMP_S, 1, 8)"
            SQL &= " = '" & strDate & "'"
            SQL &= " AND HAISIN_FLG_S = '1'"
            SQL &= " AND FUNOU_FLG_S = '0'"
            SQL &= " AND SOUSIN_KBN_S = '0'" '2010/12/24 信組対応 送信区分追加
            SQL &= " ORDER BY JIFURI_TIME_STAMP_S ASC"


            Dim Counter As Integer = 0
            If GCom.SetDynaset(SQL, REC) Then

                Do While REC.Read

                    Counter += 1

                    onDate = GCom.SET_DATE(GCom.NzDec(REC.Item("JIFURI_TIME_STAMP_S"), ""))
                    cmbTimeStamp.Items.Add(onDate.ToString("yyyy/MM/dd HH:mm:ss"))
                Loop

                If Counter > 0 Then
                    If cmbTimeStamp.Items.Count > 0 Then
                        cmbTimeStamp.SelectedIndex = 0
                    End If

                    '項目入力制御
                    txtFuriDateY.Enabled = False
                    txtFuriDateM.Enabled = False
                    txtFuriDateD.Enabled = False
                    btnSearch.Enabled = False
                    btnAction.Enabled = True

                End If
            End If

            If Counter = 0 Then
                MessageBox.Show(MSG0054W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(検索)", "失敗", ex.Message)

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            Cursor.Current = Cursors.Default

            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(検索)終了", "成功", "")

        End Try

    
    End Sub

    '実行ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        'Dim MSG As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<
        Dim SQLCode As Integer
        Dim Ret As Integer
        Dim SQL As String
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
        Dim REC As OracleDataReader = Nothing
        'Dim REC As OracleDataReader
        '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消実行)開始", "成功", "")

            Cursor.Current = Cursors.WaitCursor()

            'テキストボックスの入力チェック
            Dim onDate As New Date
            Dim strDate As String = "00000000"
            Dim CTL As TextBox = Nothing
            If Not FN_CHECK_TEXT(strDate) Then
                Return
            End If

            '2017/01/20 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
            Dim KINKOCD As String = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If KINKOCD.Equals("err") = True OrElse KINKOCD = String.Empty Then
                MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            '2017/01/20 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

            '確認メッセージ出力 2007/08/27
            If MessageBox.Show(MSG0015I.Replace("{0}", "センターカット取消"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Return
            End If

            '対象スケジュール検索
            SQL = "SELECT TORIS_CODE_S,TORIF_CODE_S,FURI_DATE_S FROM SCHMAST"
            SQL &= " WHERE FSYORI_KBN_S = '1'"
            SQL &= " AND FUNOU_FLG_S = '0'"
            SQL &= " AND SOUSIN_KBN_S = '0'" '2010/12/24 信組対応 送信区分追加
            SQL &= " AND JIFURI_TIME_STAMP_S = '" & GCom.NzDec(cmbTimeStamp.SelectedItem, "") & "'"

            Dim Counter As Integer = 0
            If GCom.SetDynaset(SQL, REC) Then
                Do While REC.Read

                    'スケジュールマスタ 更新
                    SQL = "UPDATE SCHMAST"
                    SQL &= " SET HAISIN_FLG_S = '0'"
                    SQL &= " WHERE FSYORI_KBN_S = '1'"
                    SQL &= " AND FUNOU_FLG_S = '0'"
                    SQL &= " AND SOUSIN_KBN_S = '0'" '2010/12/24 信組対応 送信区分追加
                    SQL &= " AND JIFURI_TIME_STAMP_S = '" & GCom.NzDec(cmbTimeStamp.SelectedItem, "") & "'"
                    SQL &= " AND TORIS_CODE_S = '" & GCom.NzDec(REC.Item("TORIS_CODE_S"), "") & "'"
                    SQL &= " AND TORIF_CODE_S = '" & GCom.NzDec(REC.Item("TORIF_CODE_S"), "") & "'"

                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode, True)
                    If Ret <= 0 AndAlso SQLCode <> 0 Then
                        MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End If

                    '明細マスタ 更新
                    SQL = "UPDATE MEIMAST"
                    SQL &= " SET KIGYO_SEQ_K = '00000000'"
                    SQL &= " WHERE TORIS_CODE_K = '" & GCom.NzDec(REC.Item("TORIS_CODE_S"), "") & "'"
                    SQL &= " AND TORIF_CODE_K = '" & GCom.NzDec(REC.Item("TORIF_CODE_S"), "") & "'"
                    SQL &= " AND FURI_DATE_K = '" & GCom.NzDec(REC.Item("FURI_DATE_S"), "") & "'"
                    '2017/01/20 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
                    '明細マスタ更新の条件を追加
                    SQL &= " AND KEIYAKU_KIN_K = '" & KINKOCD & "'"         '自金庫分のみ対象
                    '2017/01/20 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode, True)
                    '2017/01/20 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                    'スリーエスの委託者は他行分しか明細が存在しない場合があるため、更新件数は0件でもOKにする
                    If Ret < 0 AndAlso SQLCode <> 0 Then
                        MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End If
                    'If Ret <= 0 AndAlso SQLCode <> 0 Then
                    '    MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    '    Exit Sub
                    'End If
                    '2017/01/20 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END

                Loop
            End If

            cmbTimeStamp.Items.Clear()
            cmbTimeStamp.Text = ""

            '項目入力制御
            txtFuriDateY.Enabled = True
            txtFuriDateM.Enabled = True
            txtFuriDateD.Enabled = True
            btnSearch.Enabled = True
            btnAction.Enabled = False

            MessageBox.Show(MSG0025I.Replace("{0}", "センターカットデータ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消実行)", "失敗", ex.Message)

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消実行)終了", "成功", "")

        End Try
    End Sub

    '
    ' 機能　 ： 入力日付チェック
    '
    ' 引数　 ： なし
    '
    ' 戻り値 ： True=OK,False=NG
    '
    ' 備考　 ： Create 2009.10.12

    Private Function FN_CHECK_TEXT(ByRef strDate As String) As Boolean
        Try
            '振替日(年)
            If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateY.Text, "振替年", msgTitle) = False Then
                txtFuriDateY.Focus()
                Return False
            End If

            '振替日(月)
            If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateM.Text, "振替月", msgTitle) = False Then
                txtFuriDateM.Focus()
                Return False
            Else
                If CInt(txtFuriDateM.Text) < 1 Or CInt(txtFuriDateM.Text) > 12 Then
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateM.Focus()
                    Return False
                End If
            End If

            '振替日(日)
            If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateD.Text, "振替日", msgTitle) = False Then
                txtFuriDateD.Focus()
                Return False
            Else
                If CInt(txtFuriDateD.Text) < 1 Or CInt(txtFuriDateD.Text) > 31 Then
                    MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateD.Focus()
                    Return False
                End If
            End If

            '振替日(日付型)を返す
            strDate = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(入力チェック)", "失敗", ex.Message)
        End Try

        Return False

    End Function

    '日付関連項目
    Private Sub Date_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

    '取消ボタン押下時
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        Try
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消)開始", "成功", "")

            '処理日にシステム日付を表示
            txtFuriDateY.Text = Format(System.DateTime.Today, "yyyy")
            txtFuriDateM.Text = Format(System.DateTime.Today, "MM")
            txtFuriDateD.Text = Format(System.DateTime.Today, "dd")

            'コンボボックス設定
            cmbTimeStamp.Items.Clear()
            cmbTimeStamp.Text = ""

            '項目入力制御
            txtFuriDateY.Enabled = True
            txtFuriDateM.Enabled = True
            txtFuriDateD.Enabled = True
            btnSearch.Enabled = True
            btnAction.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消)", "失敗", ex.Message)

        Finally
            BatchLog.Write(gstrUSER_ID, "0000000000-00", "00000000", "(取消)終了", "成功", "")

        End Try
    End Sub
End Class

