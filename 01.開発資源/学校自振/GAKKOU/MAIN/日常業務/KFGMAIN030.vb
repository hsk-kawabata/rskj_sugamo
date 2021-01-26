Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Public Class KFGMAIN030

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAIN030", "口座振替データ作成画面")
    Private Const msgTitle As String = "口座振替データ作成画面(KFGMAIN030)"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAIN030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim str_Format_Furikae_Date As String
        '#####################################
        'ログの書込に必要な情報の取得
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            STR_SYORI_NAME = "口座振替データ作成画面"
            MainLOG = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            str_Format_Furikae_Date = fn_GetEigyoubi(Format(Now, "yyyyMMdd"), STR_JIFURI_HAISIN, "+")

            txtFuriDateY.Text = Mid(str_Format_Furikae_Date, 1, 4)
            txtFuriDateM.Text = Mid(str_Format_Furikae_Date, 5, 2)
            txtFuriDateD.Text = Mid(str_Format_Furikae_Date, 7, 2)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")
            Dim KFGMAIN031 As New GAKKOU.KFGMAIN031

            '入力値チェック
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            With KFGMAIN031
                .lblFuriDateY.Text = Me.txtFuriDateY.Text
                .lblFuriDateM.Text = Me.txtFuriDateM.Text
                .lblFuriDateD.Text = Me.txtFuriDateD.Text
            End With

            KFGMAIN031.ShowDialog(Me)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Private Sub "
    '****************************
    'Private Sub
    '****************************
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False

        '振替日
        If Trim(txtFuriDateY.Text) = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY.Focus()

            Exit Function
        Else
            Select Case CInt(txtFuriDateY.Text)
                Case Is >= 2004
                Case Else
                    Call MessageBox.Show(G_MSG0014W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateY.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtFuriDateM.Text) = "" Then
            MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateM.Focus()

            Exit Function
        Else
            Select Case CInt(txtFuriDateM.Text)
                Case 1 To 12
                Case Else
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateM.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtFuriDateD.Text) = "" Then
            MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateD.Focus()

            Exit Function
        End If

        STR_FURIKAE_DATE(0) = Trim(txtFuriDateY.Text) & "/" & Trim(txtFuriDateM.Text) & "/" & Trim(txtFuriDateD.Text)
        STR_FURIKAE_DATE(1) = Trim(txtFuriDateY.Text) & Format(CInt(txtFuriDateM.Text), "00") & Format(CInt(txtFuriDateD.Text), "00")

        If Not IsDate(STR_FURIKAE_DATE(0)) Then
            MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            txtFuriDateY.Focus()

            Exit Function
        End If

        STR_SQL = "SELECT CHECK_FLG_S,DATA_FLG_S,FUNOU_FLG_S,TYUUDAN_FLG_S,FURI_KBN_S FROM G_SCHMAST"
        STR_SQL += " WHERE FURI_DATE_S = '" & STR_FURIKAE_DATE(1) & "'"
        'ｽｹｼﾞｭｰﾙ区分2(随時)は処理に含まない為
        STR_SQL += " AND SCH_KBN_S <> '2'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        OBJ_DATAREADER.Read()

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Function
            End If

            MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        PFUNC_Nyuryoku_Check = True

    End Function
#End Region

End Class
