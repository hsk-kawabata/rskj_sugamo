Public Class KFUMAST020

    '数値を許可する
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFUMAST020", "金融機関マスタ更新画面")
    Private Const msgTitle As String = "金融機関マスタ更新画面(KFUMAST020)"

    Private Structure strcIni
        Dim JIKINKO_CODE As String            '自金庫コード
        Dim KIN_KOUSIN As String
    End Structure
    Private ini_info As strcIni

    'Private Sub Form1_FormClosing(ByVal sender As Object, _
    '    ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
    '    If e.CloseReason = CloseReason.UserClosing Then
    '        Me.Owner.Show()
    '    End If
    'End Sub

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")

            Me.Close()
            Me.Dispose()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

    Private Sub KFUMAST020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")

            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            '2017/06/29 saitou 永和信金(RSV2標準) UPD 基準日の初期値変更対応 ---------------------------------------- START
            '基準日にはデフォルトで翌営業日を設定する
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)終了", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim GetDate As String = ""
            bRet = GCom.CheckDateModule(SyoriDate, GetDate, 1, 0)
            txtSyoriDateY.Text = GetDate.Substring(0, 4)
            txtSyoriDateM.Text = GetDate.Substring(4, 2)
            txtSyoriDateD.Text = GetDate.Substring(6, 2)

            'txtSyoriDateY.Text = SyoriDate.Substring(0, 4)
            'txtSyoriDateM.Text = SyoriDate.Substring(4, 2)
            'txtSyoriDateD.Text = SyoriDate.Substring(6, 2)
            '2017/06/29 saitou 永和信金(RSV2標準) UPD --------------------------------------------------------------- END

            If read_ini() = False Then Return

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    Private Function read_ini() As Boolean

        Try

            ini_info.JIKINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If ini_info.JIKINKO_CODE = "err" OrElse ini_info.JIKINKO_CODE = "" Then
                MessageBox.Show(MSG0001E.Replace("{0}", "自金庫コード").Replace("{1}", "COMMON").Replace("{2}", "KINKOCD"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            ini_info.KIN_KOUSIN = CASTCommon.GetFSKJIni("COMMON", "KIN_KOUSIN")
            If ini_info.KIN_KOUSIN = "err" OrElse ini_info.KIN_KOUSIN = "" Then
                MessageBox.Show(MSG0001E.Replace("{0}", "金融機関更新フラグ").Replace("{1}", "COMMON").Replace("{2}", "KIN_KOUSIN"), _
                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

        Catch ex As Exception
            Throw
        End Try

        Return True
    End Function

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim iRet As Integer
        Dim strJOBID As String = "U010"
        Dim strPara As String
        Dim MainDB As New CASTCommon.MyOracle
        Dim KijunDate As String

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            KijunDate = String.Concat(New String() {txtSyoriDateY.Text, txtSyoriDateM.Text, txtSyoriDateD.Text})

            If check_input() = False Then Return

            If MessageBox.Show(MSG0023I.Replace("{0}", "金融機関マスタ更新"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then Return

            strPara = KijunDate & "," & ini_info.KIN_KOUSIN

            iRet = MainLOG.SearchJOBMAST(strJOBID, strPara, MainDB)
            If iRet = 1 Then
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return
            ElseIf iRet = -1 Then
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            If MainLOG.InsertJOBMAST(strJOBID, LW.UserID, strPara, MainDB) = False Then
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0021I.Replace("{0}", "金融機関マスタ更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)例外エラー", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try

    End Sub


    Private Function check_input() As Boolean
        Try

            If txtSyoriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If
            '月必須チェック
            If txtSyoriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(txtSyoriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtSyoriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If

            '日付必須チェック
            If txtSyoriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(txtSyoriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtSyoriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If
            '日付整合性チェック
            Dim WORK_DATE As String = txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If

            Return True
        Catch ex As Exception
            Throw
        End Try
    End Function

    'ZERO埋めする
    Private Sub ZERO_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtSyoriDateY.Validating, _
            txtSyoriDateM.Validating, _
            txtSyoriDateD.Validating

        Dim OBJ As TextBox = CType(sender, TextBox)
        Call GCom.NzNumberString(OBJ, True)

    End Sub

    '基準日（月）LostFocus
    Private Sub txtSyoriDateM_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles txtSyoriDateM.LostFocus

        Try

            If FN_CHK_DATE("MONTH") = False Then
                Return
            End If

        Catch ex As Exception
            MainLOG.Write("基準日(月)フォーカス失効", "失敗", ex.Message)
        End Try

    End Sub

    '基準日（日）LostFocus
    Private Sub txtSyoriDateD_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles txtSyoriDateD.LostFocus

        Try

            If FN_CHK_DATE("DAY") = False Then
                Return
            End If

        Catch ex As Exception
            MainLOG.Write("基準日(日)フォーカス失効", "失敗", ex.Message)
        End Try

    End Sub

    Private Function FN_CHK_DATE(ByVal TextType As String) As Boolean

        Try
            Dim MSG As String
            Dim DRet As DialogResult

            Select Case TextType
                Case "MONTH"
                    If Me.txtSyoriDateM.Text <> "" Then

                        MSG = MSG0022W
                        Select Case CInt(Me.txtSyoriDateM.Text)
                            Case 0, Is >= 13
                                DRet = MessageBox.Show(MSG, msgTitle, _
                                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Me.txtSyoriDateM.Focus()
                                Return False
                        End Select
                    End If

                Case "DAY"
                    If Me.txtSyoriDateD.Text <> "" Then

                        MSG = MSG0025W
                        Select Case CInt(Me.txtSyoriDateD.Text)
                            Case 0, Is >= 32
                                DRet = MessageBox.Show(MSG, msgTitle, _
                                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Me.txtSyoriDateD.Focus()
                                Return False
                        End Select
                    End If

            End Select

            Return True
        Catch ex As Exception
            MainLOG.Write("日付チェック", "失敗", ex.Message)

        End Try
    End Function

End Class
