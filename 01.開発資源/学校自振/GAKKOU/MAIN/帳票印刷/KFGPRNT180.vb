Imports System.Text
Imports CASTCommon

''' <summary>
''' 年間スケジュール表印刷画面
''' </summary>
''' <remarks>2017/04/28 saitou RSV2 added for 標準機能追加(年間スケジュール表)</remarks>
Public Class KFGPRNT180

#Region "クラス変数"
    Private Const msgTitle As String = "年間スケジュール表印刷画面(KFGPRNT180)"

    'ログ
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT180", "年間スケジュール表印刷画面")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    'グローバル変数
    Private GAKKOU_CODE As String
    Private NENDO As String

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' フォームロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFGPRNT180_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '----------------------------------------
            'ログ書込みに必要な情報の取得
            '----------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            'ユーザID、システム日付表示
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '学校用オラクル接続
            Call GSUB_CONNECT()

            '----------------------------------------
            '学校検索コンボボックス設定
            '----------------------------------------
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGakkouName)")
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' フォームクローズイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        '学校用オラクル切断
        Call GSUB_CLOSE()
    End Sub

    ''' <summary>
    ''' 印刷ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnPrnt_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPrnt.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            '----------------------------------------
            'テキストボックスの入力チェック
            '----------------------------------------
            If Me.CheckTextBox() = False Then
                Return
            End If

            '----------------------------------------
            'マスタのチェック
            '----------------------------------------
            Me.GAKKOU_CODE = Me.txtGAKKOU_CODE.Text
            Me.NENDO = Me.txtFuriDateY.Text
            LW.ToriCode = Me.GAKKOU_CODE
            If Me.CheckTable() = False Then
                Return
            End If

            If MessageBox.Show(String.Format(MSG0013I, "年間スケジュール表"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If

            '----------------------------------------
            '印刷バッチ呼び出し
            '----------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            Dim nRet As Integer
            Dim CmdArg As String = GCom.GetUserID & "," & Me.GAKKOU_CODE & "," & Me.NENDO

            nRet = ExeRepo.ExecReport("KFGP037.EXE", CmdArg)

            Select Case nRet
                Case 0
                    MessageBox.Show(String.Format(MSG0014I, "年間スケジュール表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    MessageBox.Show(String.Format(MSG0004E, "年間スケジュール表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
            LW.ToriCode = "0000000000"
        End Try
    End Sub

    ''' <summary>
    ''' 終了ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEnd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEnd.Click
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

    ''' <summary>
    ''' 学校名カナコンボボックス　インデックスチェンジイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            If Me.cmbKana.SelectedItem.ToString = "" Then
                Return
            End If

            '学校名コンボボックス設定
            If GFUNC_DB_COMBO_SET(Me.cmbKana, cmbGakkouName) = True Then
                Me.cmbGakkouName.Focus()
            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' 学校名コンボボックス　インデックスチェンジイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmbGakkouName.SelectedIndexChanged
        Try
            If Not cmbGakkouName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbGakkouName.SelectedItem.ToString) Then
                '学校名の取得
                Me.lab学校名.Text = Me.cmbGakkouName.Text
                Me.txtGAKKOU_CODE.Text = STR_GCOAD(Me.cmbGakkouName.SelectedIndex)

                '学校コードにカーソル設定
                Me.txtGAKKOU_CODE.Focus()
            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' 学校コードテキスト　バリデイティングイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        Try
            Call GCom.NzNumberString(txtGAKKOU_CODE, True)
        Catch ex As Exception
            MainLOG.Write("ゼロパディング", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' 学校コード　バリデイテッドイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtGAKKOU_CODE_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validated
        Dim dbConn As CASTCommon.MyOracle = Nothing
        Dim dbReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            Me.lab学校名.Text = ""
            If Me.txtGAKKOU_CODE.Text = "9999999999" Then
                Return
            End If

            SQL.Append("select distinct")
            SQL.Append("     GAKKOU_NNAME_G")
            SQL.Append(" from")
            SQL.Append("     GAKMAST1")
            SQL.Append(" where")
            SQL.Append("     GAKKOU_CODE_G = " & SQ(Me.txtGAKKOU_CODE.Text))

            dbConn = New CASTCommon.MyOracle
            dbReader = New CASTCommon.MyOracleReader(dbConn)
            If dbReader.DataReader(SQL) = True Then
                lab学校名.Text = dbReader.GetString("GAKKOU_NNAME_G")
            End If

        Catch ex As Exception
            MainLOG.Write("学校情報取得", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not dbReader Is Nothing Then
                dbReader.Close()
                dbReader = Nothing
            End If
            If Not dbConn Is Nothing Then
                dbConn.Close()
                dbConn = Nothing
            End If
        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' テキストボックスの入力チェックを行います。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function CheckTextBox() As Boolean
        Try
            '学校コード
            If Me.txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtGAKKOU_CODE.Focus()
                Return False
            End If

            '年度
            If Me.txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "年度"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストボックス入力チェック", "失敗", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' マスタのチェックを行います。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function CheckTable() As Boolean
        Dim dbConn As CASTCommon.MyOracle = Nothing
        Dim dbReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder

        Try
            dbConn = New CASTCommon.MyOracle
            dbReader = New CASTCommon.MyOracleReader(dbConn)

            '----------------------------------------
            '学校マスタに存在するかチェック
            '----------------------------------------
            With SQL
                .Length = 0
                .Append("select")
                .Append("     *")
                .Append(" from")
                .Append("     GAKMAST2")
                If Me.GAKKOU_CODE = "9999999999" Then
                Else
                    .Append(" where")
                    .Append("     GAKKOU_CODE_T = " & SQ(Me.GAKKOU_CODE))
                End If
            End With

            If dbReader.DataReader(SQL) = False Then
                If Me.GAKKOU_CODE = "9999999999" Then
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                Else
                    MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If

            dbReader.Close()

            '----------------------------------------
            'スケジュールマスタに存在するかチェック
            '----------------------------------------
            With SQL
                .Length = 0
                .Append("select")
                .Append("     *")
                .Append(" from")
                .Append("     G_SCHMAST")
                .Append(" inner join")
                .Append("     GAKMAST2")
                .Append(" on  GAKKOU_CODE_S = GAKKOU_CODE_T")
                .Append(" where ")
                .Append("     NENGETUDO_S")
                .Append(" between")
                .Append("     " & SQ(Me.NENDO & "04"))
                .Append(" and " & SQ(CStr(CInt(Me.NENDO) + 1) & "03"))
                If Me.GAKKOU_CODE = "9999999999" Then
                Else
                    .Append(" and GAKKOU_CODE_S = " & SQ(Me.GAKKOU_CODE))
                End If
            End With

            If dbReader.DataReader(SQL) = False Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタチェック)", "失敗", ex.Message)
            Return False
        Finally
            If Not dbReader Is Nothing Then
                dbReader.Close()
                dbReader = Nothing
            End If

            If Not dbConn Is Nothing Then
                dbConn.Close()
                dbConn = Nothing
            End If
        End Try
    End Function

#End Region

End Class
