Option Explicit On
Option Strict Off
Imports CASTCommon
Imports System.Text
Imports Microsoft.VisualBasic
Imports System.Windows.Forms

Public Class KFGPRNT160
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 学校マスタ項目確認票印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Private STR学校名 As String
    Dim strKIGYO_CODE As String
    Dim strFURI_CODE As String
    Dim strGAKKOU_CODE As String
    Dim strKIN_NO As String
    Dim strSIT_NO As String
    Dim strKAMOKU As String
    Dim strKOUZA As String
    Dim strKNAME As String
    Dim strReKNAME As String
    Dim intKEKKA As Integer
    Dim STR帳票ソート順 As String
    Dim Str_Kubn As String
    Dim STR学校コード As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT160", "学校マスタ項目確認票印刷画面")
    Private Const msgTitle As String = "学校マスタ項目確認票印刷画面(KFGPRNT160)"
    Private Const PrintName As String = "学校マスタ項目確認票"
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
    Private Sub KFGPRNT160_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '-------------------------------------
            ' ログ情報設定
            '-------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "開始", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            Call GSUB_CONNECT()

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '------------------------------------------
            '基準日の設定
            '------------------------------------------
            txtDateY.Text = Now.ToString("yyyy")
            txtDateM.Text = Now.ToString("MM")
            txtDateD.Text = Now.ToString("dd")

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
        Dim SQL As New StringBuilder
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Try
            '印刷ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            '学校コード必須チェック
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If

            '--------------------------------
            ' 基準日必須チェック
            '--------------------------------
            If CheckIsInputRequiredControl() = False Then
                Return
            End If

            LW.ToriCode = Trim(txtGAKKOU_CODE.Text)

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, PrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            STR学校コード = txtGAKKOU_CODE.Text
            Dim prmKijyunDate As String = txtDateY.Text & txtDateM.Text & txtDateD.Text '基準日

            'ログインID,学校コード,基準日
            Param = GCom.GetUserID & "," & STR学校コード & "," & prmKijyunDate
            nRet = ExeRepo.ExecReport("KFGP038.EXE", Param)
            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
            End Select

            txtGAKKOU_CODE.Focus()
            MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
            MainDB.Rollback()
        Finally
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
            SQL.Append(" SELECT ")
            SQL.Append(" GAKMAST1.*")
            SQL.Append(",MEISAI_OUT_T")
            SQL.Append(" FROM GAKMAST1,GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")
            SQL.Append(" AND GAKKOU_CODE_G = " & SQ(STR学校コード))

            If OraReader.DataReader(SQL) = False Then

                MessageBox.Show("学校マスタに登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                lab学校名.Text = ""
                STR帳票ソート順 = 0

                Exit Function
            End If

            If NameChg Then lab学校名.Text = OraReader.GetString("GAKKOU_NNAME_G")
            STR帳票ソート順 = OraReader.GetInt("MEISAI_OUT_T")

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
    ' 入力チェック
    Private Function CheckIsInputRequiredControl() As Boolean

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "開始", "")

            '基準日(年)
            Dim FURIKAE_DATE As String
            If fn_CHECK_NUM_MSG(txtDateY.Text, "基準日(年)", msgTitle) = False Then
                txtDateY.Focus()
                Return False
            End If

            '基準日(月)
            If fn_CHECK_NUM_MSG(txtDateM.Text, "基準日(月)", msgTitle) = False Then
                txtDateM.Focus()
                Return False
            Else
                If txtDateM.Text < 1 Or txtDateM.Text > 12 Then
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtDateM.Focus()
                    Return False
                End If
            End If

            '基準日(日)
            If fn_CHECK_NUM_MSG(txtDateD.Text, "基準日(日)", msgTitle) = False Then
                txtDateD.Focus()
                Return False
            Else
                If txtDateD.Text < 1 Or txtDateD.Text > 31 Then
                    MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtDateD.Focus()
                    Return False
                End If
            End If

            '基準日妥当性チェック
            If txtDateY.Text.Length <> 0 Or txtDateM.Text.Length <> 0 Or txtDateD.Text.Length <> 0 Then
                FURIKAE_DATE = txtDateY.Text & "/" & txtDateM.Text & "/" & txtDateD.Text
                If Not IsDate(FURIKAE_DATE) Then
                    MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtDateY.Focus()
                    txtDateY.SelectionStart = 0
                    txtDateY.SelectionLength = txtDateY.Text.Length
                    Return False
                End If
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "成功", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "終了", "")
        End Try

    End Function

    Private Function fn_CHECK_NUM_MSG(ByVal objOBJ As String, ByVal strJNAME As String, ByVal gstrTITLE As String) As Boolean
        '============================================================================
        'NAME           :fn_CHECK_NUM_MSG
        'Parameter      :objOBJ：チェック対象オブジェクト／strJNAME：オブジェクト名称
        '               :gstrTITLE：タイトル
        'Description    :数値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/05/28
        'Update         :
        '============================================================================

        Try

            If Trim(objOBJ).Length = 0 Then
                MessageBox.Show(String.Format(MSG0285W, strJNAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            For i As Integer = 0 To objOBJ.Length - 1 Step 1       '小数点/符号ﾁｪｯｸ
                If Char.IsDigit(objOBJ.Chars(i)) = False Then
                    MessageBox.Show(String.Format(MSG0344W, strJNAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            Next i

            Return True

        Catch ex As Exception
            Return False
        End Try

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

    'テキストボックスゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtDateY.Validating, _
                txtDateM.Validating, _
                txtDateD.Validating


        Call GCom.NzNumberString(CType(sender, TextBox), True)

    End Sub

End Class
