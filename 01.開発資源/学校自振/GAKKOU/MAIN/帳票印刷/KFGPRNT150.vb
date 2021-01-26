Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT150
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 長子チェックリスト印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Private STR学校名 As String
    '2006/10/11 帳票のソート機能追加
    Dim STR帳票ソート順 As String
    Dim STR学校コード As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT150", "生徒マスタ管理表印刷画面")
    Private Const msgTitle As String = "生徒マスタ管理表印刷画面(KFGPRNT150)"
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAST130_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '入力ボタン制御
            btnPrnt.Enabled = True
            btnEnd.Enabled = True
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
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Try
            '印刷ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            MainDB = New MyOracle

            '学校コード必須チェック
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If

            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If

            LW.ToriCode = Trim(txtGAKKOU_CODE.Text)

            SQL.Append("SELECT DISTINCT GAKKOU_CODE_T FROM GAKMAST2 ")

            'SQL.Append(" AND TYOUSI_FLG_O <> 0 ")
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                SQL.Append(" WHERE ")
                SQL.Append(" GAKKOU_CODE_T = " & SQ(txtGAKKOU_CODE.Text))
            End If
            SQL.Append(" ORDER BY GAKKOU_CODE_T ASC")

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "生徒マスタ管理表"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim intINSATU_FLG2 As Integer = 0
            OraReader = New MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    STR学校コード = OraReader.GetString("GAKKOU_CODE_T")
                    '帳票ソート順の取得
                    Dim counter As Double = 0
                    If PFUNC_SEITO_CNT(STR学校コード, counter) = False Then
                        Return
                    End If
                    'ログインID,学校コード
                    Param = GCom.GetUserID & "," & STR学校コード & "," & counter
                    nRet = ExeRepo.ExecReport("KFGP033.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                        Case -1
                            '印刷対象なしメッセージ
                            MessageBox.Show("生徒マスタに生徒が存在しません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            Return
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "生徒マスタ管理表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                    End Select

                    OraReader.NextRead()
                End While
            Else
                MessageBox.Show("生徒マスタの検索に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校検索2)", "失敗", "")
                Return
            End If
            MessageBox.Show(String.Format(MSG0014I, "生徒マスタ管理表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            If OraReader IsNot Nothing Then OraReader.Close()
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET(Optional ByVal NameChg As Boolean = True) As Boolean

        '学校名の設定
        PFUNC_GAKNAME_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            If Trim(STR学校コード) = "9999999999" Then
                lab学校名.Text = ""
            Else
                SQL.Append(" SELECT ")
                SQL.Append(" DISTINCT GAKKOU_NNAME_G")
                SQL.Append(" FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(STR学校コード))

                If OraReader.DataReader(SQL) = False Then

                    MessageBox.Show("学校マスタに登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    'lab学校名.Text = ""
                    'STR帳票ソート順 = 0

                    Exit Function
                End If

                If NameChg Then lab学校名.Text = OraReader.GetString("GAKKOU_NNAME_G")
                'STR帳票ソート順 = OraReader.GetInt("MEISAI_OUT_T")

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校検索)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        PFUNC_GAKNAME_GET = True

    End Function

    Private Function PFUNC_SEITO_CNT(ByVal GakkouCode As String, ByRef Count As Double) As Boolean

        '学校名の設定
        PFUNC_SEITO_CNT = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            If Trim(STR学校コード) = "9999999999" Then
                lab学校名.Text = ""
            Else
                SQL.Append(" SELECT ")
                SQL.Append(" COUNT(*) AS CNT")
                SQL.Append(" FROM SEITOMASTVIEW")
                SQL.Append(" WHERE ")
                SQL.Append(" GAKKOU_CODE_O = " & SQ(GakkouCode))

                If OraReader.DataReader(SQL) = False Then

                    MessageBox.Show("生徒マスタの検索に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

                    'lab学校名.Text = ""
                    'STR帳票ソート順 = 0

                    Exit Function
                Else
                    Count = CDbl(OraReader.GetString("CNT"))
                End If


            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(生徒数検索)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        PFUNC_SEITO_CNT = True

    End Function
#End Region

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校名コンボボックス設定
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = True Then
            cmbGakkouName.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '学校検索後の学校コード設定
        '学校名の取得
        lab学校名.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '学校コード
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '2010/11/09 学校コードゼロ埋め
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            '学校名の取得
            STR学校コード = Trim(txtGAKKOU_CODE.Text)
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If

    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
