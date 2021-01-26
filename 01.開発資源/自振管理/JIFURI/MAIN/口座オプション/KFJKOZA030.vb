Imports System.Text
Imports CASTCommon


Public Class KFJKOZA030

    '共通イベントコントロール
    Private CAST As New CASTCommon.Events

    '' 許可文字指定

    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MyOwnerForm As Form

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private Structure strcIni
        Dim JIKINKO_CODE As String            '自金庫コード
    End Structure

    Private ini_info As strcIni

    Private MainLOG As New CASTCommon.BatchLOG("KFJKOZA030", "自振契約リエンタ作成画面")
    Private Const msgTitle As String = "自振契約リエンタ作成画面(KFJKOZA030)"


    Private Sub KFJKOZA030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtKaisiDateY.Text = strSysDate.Substring(0, 4)
            txtKaisiDateM.Text = strSysDate.Substring(4, 2)
            txtKaisiDateD.Text = strSysDate.Substring(6, 2)

            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub


    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim MainDB As New CASTCommon.MyOracle
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim strJOBID As String = "J090"
        Dim strPARAMETA As String = ""
        Dim iRet As Integer
        Dim sql As New StringBuilder(128)
        Dim SyoriDate As String = ""

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            SyoriDate = txtKaisiDateY.Text & txtKaisiDateM.Text & txtKaisiDateD.Text

            If check_input() = False Then Return

            '***********************************
            ' 自振契約リエンタ対象の検索
            '***********************************
            sql.Append(" SELECT ")
            sql.Append(" COUNT(*) AS COUNTER FROM STORE_JIFKEIYAKU ")
            '条件は指定しない。あるものすべて作る。

            If oraReader.DataReader(sql) = True Then
                If oraReader.GetInt64("COUNTER") <= 0 Then
                    MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            Else
                MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            oraReader.Close()

            If MessageBox.Show(MSG0023I.Replace("{0}", "自振契約リエンタ作成"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then Return

            strPARAMETA = SyoriDate

            iRet = MainLOG.SearchJOBMAST(strJOBID, strPARAMETA, MainDB)
            If iRet = 1 Then
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return
            ElseIf iRet = -1 Then
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            If MainLOG.InsertJOBMAST(strJOBID, LW.UserID, strPARAMETA, MainDB) = False Then
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            '2016/02/05 タスク）斎藤 RSV2対応 MODIFY ------------------------------------- START
            'コミット追加
            MainDB.Commit()
            MainDB.Close()
            MainDB = Nothing
            '2016/02/05 タスク）斎藤 RSV2対応 MODIFY ------------------------------------- END

            MessageBox.Show(MSG0021I.Replace("{0}", "自振契約リエンタ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception

            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)例外エラー", "失敗", ex.Message)

        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()

        End Try

    End Sub

    Private Function check_input() As Boolean

        '年必須チェック
        If txtKaisiDateY.Text.Trim = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtKaisiDateY.Focus()
            Return False
        End If
        '月必須チェック
        If txtKaisiDateM.Text.Trim = "" Then
            MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtKaisiDateM.Focus()
            Return False
        End If
        '月範囲チェック
        If GCom.NzInt(txtKaisiDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtKaisiDateM.Text.Trim) > 12 Then
            MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtKaisiDateM.Focus()
            Return False
        End If

        '日付必須チェック
        If txtKaisiDateD.Text.Trim = "" Then
            MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtKaisiDateD.Focus()
            Return False
        End If
        '日付範囲チェック
        If GCom.NzInt(txtKaisiDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtKaisiDateD.Text.Trim) > 31 Then
            MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtKaisiDateD.Focus()
            Return False
        End If
        '日付整合性チェック
        Dim WORK_DATE As String = txtKaisiDateY.Text & "/" & txtKaisiDateM.Text & "/" & txtKaisiDateD.Text
        If Information.IsDate(WORK_DATE) = False Then
            MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtKaisiDateY.Focus()
            Return False
        End If

        Return True

    End Function

    Private Sub FURI_DATE_S_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtKaisiDateY.Validating, txtKaisiDateM.Validating, txtKaisiDateD.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力後処置)例外エラー", "失敗", ex.Message)
        End Try


    End Sub

End Class
