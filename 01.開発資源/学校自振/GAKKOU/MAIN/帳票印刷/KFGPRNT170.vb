Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text

Public Class KFGPRNT170
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 学校生徒名簿印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Private STR学校名 As String
    Private STR学年名 As String
    '2017/06/23 saitou 標準版修正 UPD ---------------------------------------- START
    '帳票名間違い
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT170", "生徒登録情報一覧表印刷画面")
    Private Const msgTitle As String = "生徒登録情報一覧表印刷画面(KFGPRNT170)"
    'Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT170", "学校生徒名簿印刷画面")
    'Private Const msgTitle As String = "学校生徒名簿印刷画面(KFGPRNT170)"
    '2017/06/23 saitou 標準版修正 UPD ---------------------------------------- END
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
    Private Sub GFJPRNT0700G_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
        Dim Flg As String = ""
        Try
            '印刷ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            MainDB = New MyOracle

            '入力値チェック
            If PFUNC_NYUURYOKU_CHECK() = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text


            SQL = New StringBuilder
            SQL.Append("SELECT DISTINCT GAKKOU_CODE_O")
            SQL.Append(" FROM SEITOMAST")
            SQL.Append(" WHERE KAIYAKU_FLG_O ='0'")
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) = "" Then
            ElseIf Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) <> "" Then
                SQL.Append(" AND GAKUNEN_CODE_O = " & Trim(txtGAKUNEN.Text))
            ElseIf Trim(txtGAKKOU_CODE.Text) <> "9999999999" And Trim(txtGAKUNEN.Text) = "" Then
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
            Else
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
                SQL.Append(" AND GAKUNEN_CODE_O = " & Trim(txtGAKUNEN.Text))
            End If
            SQL.Append(" ORDER BY GAKKOU_CODE_O")

            OraReader = New MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '印刷前確認メッセージ
            '2017/06/23 saitou 標準版修正 UPD ---------------------------------------- START
            '帳票名間違い
            If MessageBox.Show(String.Format(MSG0013I, "生徒登録情報一覧表"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            'If MessageBox.Show(String.Format(MSG0013I, "学校生徒名簿"), _
            '                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            '    Return
            'End If
            '2017/06/23 saitou 標準版修正 UPD ---------------------------------------- END

            While OraReader.EOF = False
                'ログインID,学校コード,学年,クラス
                Param = GCom.GetUserID & "," & OraReader.GetString("GAKKOU_CODE_O") & "," & txtGAKUNEN.Text.Trim & "," & txtCLASS.Text
                nRet = ExeRepo.ExecReport("KFGP039.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        '2017/06/23 saitou 標準版修正 UPD ---------------------------------------- START
                        '帳票名間違い
                        MessageBox.Show(String.Format(MSG0004E, "生徒登録情報一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        'MessageBox.Show(String.Format(MSG0004E, "学校生徒名簿"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        '2017/06/23 saitou 標準版修正 UPD ---------------------------------------- END
                        Return
                End Select
                OraReader.NextRead()
            End While

            '2017/06/23 saitou 標準版修正 UPD ---------------------------------------- START
            '帳票名間違い
            MessageBox.Show(String.Format(MSG0014I, "生徒登録情報一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            'MessageBox.Show(String.Format(MSG0014I, "学校生徒名簿"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            '2017/06/23 saitou 標準版修正 UPD ---------------------------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
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
    Private Function PFUNC_GAKNAME_GET() As Boolean

        PFUNC_GAKNAME_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            '学校名の設定
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab学校名.Text = "全学校対象"
            Else
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE")
                SQL.Append(" GAKKOU_CODE_G = " & SQ(txtGAKKOU_CODE.Text))

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("学校マスタに登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lab学校名.Text = ""
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                lab学校名.Text = OraReader.GetString("GAKKOU_NNAME_G")
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
    Private Function PFUNC_GAKUNENNAME_GET() As Boolean

        PFUNC_GAKUNENNAME_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            '学校名の設定
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lblGAKUNEN_NAME.Text = ""
            Else
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(txtGAKKOU_CODE.Text))
                SQL.Append(" AND GAKUNEN_CODE_G =" & txtGAKUNEN.Text)

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("学年： " & txtGAKUNEN.Text & " は学校マスタに登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lblGAKUNEN_NAME.Text = ""
                    txtGAKUNEN.Text = ""
                    txtGAKUNEN.Focus()
                    Exit Function
                End If

                STR学年名 = OraReader.GetString("GAKUNEN_NAME_G")

                lblGAKUNEN_NAME.Text = STR学年名
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学年検索)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
        PFUNC_GAKUNENNAME_GET = True

    End Function

    Private Function PFUNC_NYUURYOKU_CHECK() As Boolean

        PFUNC_NYUURYOKU_CHECK = False

        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            '学校コード必須チェック
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                SQL = New StringBuilder
                '学校マスタ存在チェック
                SQL.Append("SELECT GAKKOU_CODE_G")
                SQL.Append(" FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(txtGAKKOU_CODE.Text))

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("入力された学校コードが存在しません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If
            End If


            '印刷する生徒データがあるか検索
            SQL = New StringBuilder
            SQL.Append("SELECT * FROM SEITOMAST")
            SQL.Append(" WHERE KAIYAKU_FLG_O ='0'")
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) = "" Then

            ElseIf Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) <> "" Then
                SQL.Append(" AND GAKUNEN_CODE_O = " & SQ(Trim(txtGAKUNEN.Text)))
            ElseIf Trim(txtGAKKOU_CODE.Text) <> "9999999999" And Trim(txtGAKUNEN.Text) = "" Then
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
            Else
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
                SQL.Append(" AND GAKUNEN_CODE_O = " & Trim(txtGAKUNEN.Text))
            End If

            If OraReader.DataReader(SQL) = False Then
                If Trim(txtGAKUNEN.Text) <> "" Then
                    MessageBox.Show("指定学年に生徒が登録されていません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    MessageBox.Show("指定学校に生徒が登録されていません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                txtGAKUNEN.Focus()
                Exit Function
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_NYUURYOKU_CHECK = True

    End Function
#End Region
#Region " イベント"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '学校カナ絞込みコンボ
        '********************************************
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
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '学校名の取得
        lab学校名.Text = cmbGakkouName.Text

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
       Handles txtCLASS.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("ゼロパディング", "失敗", ex.ToString)
        End Try
    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '学校コード
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            CType(sender, TextBox).Text = CType(sender, TextBox).Text.Trim.PadLeft(CType(sender, TextBox).MaxLength, "0")
            '学校名の取得
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If
    End Sub
    Private Sub txtGAKUNEN_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKUNEN.Validating
        '学年
        If Trim(txtGAKUNEN.Text) <> "" Then
            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                If PFUNC_GAKUNENNAME_GET() = False Then
                    Exit Sub
                End If
            End If
        End If
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

End Class
