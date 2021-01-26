Imports System
Imports System.Text
Imports CASTCommon

''' <summary>
''' 提携区分マスタメンテナンス画面　メインクラス
''' </summary>
''' <remarks>2017/01/18 saitou 東春信金(RSV2標準) added for スリーエス対応</remarks>
Public Class KFUMAST080

#Region "クラス変数"
    'メッセージ
    Private Const msgTitle As String = "提携区分マスタメンテナンス画面(KFUMAST080)"

    'イベント
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    'ログ
    Private MainLOG As New CASTCommon.BatchLOG("KFUMAST080", "提携区分マスタメンテナンス画面")
    Private Structure LogWrite
        Dim UserID As String        'ユーザID
        Dim ToriCode As String      '取引先コード
        Dim FuriDate As String      '振替日
    End Structure
    Private LW As LogWrite

    'データベース
    Private dbConn As CASTCommon.MyOracle

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' フォームロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFUMAST080_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            '----------------------------------------
            'ログ書き込みに必要な情報の取得
            '----------------------------------------
            With LW
                .UserID = GCom.GetUserID
                .ToriCode = "000000000000"
                .FuriDate = "00000000"
            End With

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '----------------------------------------
            'システム日付とユーザ名を表示
            '----------------------------------------
            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            '----------------------------------------
            '画面初期設定
            '----------------------------------------
            If Me.SetFormInitialize() = False Then
                Return
            End If
            Call Me.SetButtonEnabled(True)
            Call Me.SetListInitialize()

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 登録ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAction_Click(sender As Object, e As EventArgs) Handles btnAction.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")

            '----------------------------------------
            'テキストボックスの入力チェック
            '----------------------------------------
            If Me.CheckTextBox() = False Then
                Return
            End If

            '----------------------------------------
            'テーブルの存在チェック
            '----------------------------------------
            Dim KinNo As String = Me.txtKinNo.Text.Trim
            Me.dbConn = New CASTCommon.MyOracle
            If Me.CheckExistsTable(KinNo) = True Then
                MessageBox.Show(MSG0122W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '----------------------------------------
            'テーブル登録
            '----------------------------------------
            Dim TeikeiKbn As String = GCom.GetComboBox(Me.cmbTeikeiKbn)
            Dim SQL As New StringBuilder
            With SQL
                .Append("insert")
                .Append(" into SSS_TKBNMAST")
                .Append(" values(")
                .Append("     '" & KinNo & "'")
                .Append("    ,'" & TeikeiKbn & "'")
                .Append(" )")
            End With

            Dim iRet As Integer = Me.dbConn.ExecuteNonQuery(SQL)
            If iRet <> 1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "提携区分マスタ登録", "失敗", "戻り値:" & iRet.ToString)
                MessageBox.Show(String.Format(MSG0002E, "登録"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Me.dbConn.Commit()
            Me.dbConn.Close()
            Me.dbConn = Nothing

            MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '----------------------------------------
            '画面クリア
            '----------------------------------------
            Call Me.SetFormInitialize()
            Call Me.SetListInitialize()

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not Me.dbConn Is Nothing Then
                Me.dbConn.Rollback()
                Me.dbConn.Close()
                Me.dbConn = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 更新ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnKousin_Click(sender As Object, e As EventArgs) Handles btnKousin.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")

            '----------------------------------------
            'テーブルの存在チェック
            '----------------------------------------
            Dim KinNo As String = Me.txtKinNo.Text.Trim
            Me.dbConn = New CASTCommon.MyOracle
            If Me.CheckExistsTable(KinNo) = False Then
                MessageBox.Show(MSG0034W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '----------------------------------------
            'テーブル更新
            '----------------------------------------
            Dim TeikeiKbn As String = GCom.GetComboBox(Me.cmbTeikeiKbn)
            Dim SQL As New StringBuilder
            With SQL
                .Append("update SSS_TKBNMAST")
                .Append(" set")
                .Append("     TEIKEI_KBN_N = '" & TeikeiKbn & "'")
                .Append(" where")
                .Append("     KIN_NO_N = '" & KinNo & "'")
            End With

            Dim iRet As Integer = Me.dbConn.ExecuteNonQuery(SQL)
            'キーで更新なので、必ず1レコード更新
            If iRet <> 1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "提携区分マスタ更新", "失敗", "戻り値:" & iRet.ToString)
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Me.dbConn.Commit()
            Me.dbConn.Close()
            Me.dbConn = Nothing

            MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '----------------------------------------
            '画面クリア
            '----------------------------------------
            Call Me.SetFormInitialize()
            Call Me.SetListInitialize()

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not Me.dbConn Is Nothing Then
                Me.dbConn.Rollback()
                Me.dbConn.Close()
                Me.dbConn = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 参照ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnSansyo_Click(sender As Object, e As EventArgs) Handles btnSansyo.Click

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            '----------------------------------------
            'テキストボックスの入力チェック
            '----------------------------------------
            If Me.CheckTextBox() = False Then
                Return
            End If

            '----------------------------------------
            'テーブルの存在チェック
            '----------------------------------------
            Dim KinNo As String = Me.txtKinNo.Text.Trim
            Me.dbConn = New CASTCommon.MyOracle
            If Me.CheckExistsTable(KinNo) = False Then
                MessageBox.Show(MSG0034W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKinNo.Focus()
                Return
            End If

            '----------------------------------------
            '内容設定
            '----------------------------------------
            Dim TeikeiKbn As String = String.Empty

            OraReader = New CASTCommon.MyOracleReader(Me.dbConn)
            If OraReader.DataReader(GetExistsSQL(KinNo)) = True Then
                While OraReader.EOF = False
                    TeikeiKbn = OraReader.GetString("TEIKEI_KBN_N")
                    OraReader.NextRead()
                End While
            End If

            Dim IntTemp As Integer = GCom.NzInt(TeikeiKbn)
            For Cnt As Integer = 0 To Me.cmbTeikeiKbn.Items.Count - 1
                Me.cmbTeikeiKbn.SelectedIndex = Cnt
                If GCom.GetComboBox(Me.cmbTeikeiKbn) = IntTemp Then
                    Exit For
                End If
            Next

            'テキストボックス非活性
            Me.txtKinNo.Enabled = False
            'ボタン設定
            Me.SetButtonEnabled(False)

            Me.cmbTeikeiKbn.Focus()

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not Me.dbConn Is Nothing Then
                Me.dbConn.Close()
                Me.dbConn = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 削除ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)開始", "成功", "")

            '----------------------------------------
            'テーブルの存在チェック
            '----------------------------------------
            Dim KinNo As String = Me.txtKinNo.Text.Trim
            Me.dbConn = New CASTCommon.MyOracle
            If Me.CheckExistsTable(KinNo) = False Then
                MessageBox.Show(MSG0034W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '----------------------------------------
            'テーブル削除
            '----------------------------------------
            Dim SQL As New StringBuilder
            With SQL
                .Append("delete from")
                .Append("     SSS_TKBNMAST")
                .Append(" where")
                .Append("     KIN_NO_N = '" & KinNo & "'")
            End With

            Dim iRet As Integer = Me.dbConn.ExecuteNonQuery(SQL)
            'キーで削除なので、必ず1レコード削除
            If iRet <> 1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "提携区分マスタ削除", "失敗", "戻り値:" & iRet.ToString)
                MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Me.dbConn.Commit()
            Me.dbConn.Close()
            Me.dbConn = Nothing

            MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '----------------------------------------
            '画面クリア
            '----------------------------------------
            Call Me.SetFormInitialize()
            Call Me.SetListInitialize()

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not Me.dbConn Is Nothing Then
                Me.dbConn.Rollback()
                Me.dbConn.Close()
                Me.dbConn = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 取消ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEraser_Click(sender As Object, e As EventArgs) Handles btnEraser.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")

            If MessageBox.Show(MSG0009I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '----------------------------------------
            '画面クリア
            '----------------------------------------
            Call Me.SetFormInitialize()

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 終了ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEnd_Click(sender As Object, e As EventArgs) Handles btnEnd.Click
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

#Region "プライベートメソッド"

    ''' <summary>
    ''' テキストボックスの入力チェックを行います。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function CheckTextBox() As Boolean
        Try
            '金融機関コード必須チェック
            If Me.txtKinNo.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0032W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKinNo.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 提携区分マスタを取得するSQLを作成します。
    ''' </summary>
    ''' <param name="KinNo">対象金融機関コード</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function GetExistsSQL(ByVal KinNo As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select")
            .Append("     KIN_NO_N")
            .Append("    ,TEIKEI_KBN_N")
            .Append(" from")
            .Append("     SSS_TKBNMAST")
            .Append(" where")
            .Append("     KIN_NO_N = '" & KinNo & "'")
            .Append(" order by")
            .Append("     KIN_NO_N")
        End With

        Return SQL
    End Function

    ''' <summary>
    ''' 提携区分マスタの存在チェックを行います。
    ''' </summary>
    ''' <param name="KinNo">対象金融機関コード</param>
    ''' <returns>True - 対象有り , False - 対象無し</returns>
    ''' <remarks></remarks>
    Private Function CheckExistsTable(ByVal KinNo As String) As Boolean
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(Me.dbConn)
            If OraReader.DataReader(Me.GetExistsSQL(KinNo)) = True Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "提携区分マスタ存在チェック", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

    End Function

    ''' <summary>
    ''' 画面項目の初期化を行います。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function SetFormInitialize() As Boolean
        Try
            '----------------------------------------
            'テキストボックスの設定
            '----------------------------------------
            Me.txtKinNo.Text = String.Empty
            Me.txtKinNo.Enabled = True

            '----------------------------------------
            'コンボボックスの設定
            '----------------------------------------
            Select Case GCom.SetComboBox(Me.cmbTeikeiKbn, "KFUMAST010_提携区分.TXT", True)
                Case 1
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "コンボボックス設定", "失敗", "コンボボックス設定テキスト無し KFUMAST010_提携区分.TXT")
                    MessageBox.Show(String.Format(MSG0025E, "提携区分", "KFUMAST010_提携区分.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Case 2
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "コンボボックス設定", "失敗", "コンボボックス設定失敗 KFUMAST010_提携区分.TXT")
                    MessageBox.Show(String.Format(MSG0026E, "提携区分"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
            End Select

            '----------------------------------------
            'ボタンの設定
            '----------------------------------------
            Call Me.SetButtonEnabled(True)

            '----------------------------------------
            'カーソル設定
            '----------------------------------------
            Me.txtKinNo.Focus()

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "画面初期設定", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' ボタンの活性、非活性を設定します。
    ''' </summary>
    ''' <param name="Pattern"></param>
    ''' <remarks></remarks>
    Private Sub SetButtonEnabled(ByVal Pattern As Boolean)
        Me.btnAction.Enabled = Pattern
        Me.btnSansyo.Enabled = Pattern
        Me.btnKousin.Enabled = Not Pattern
        Me.btnDelete.Enabled = Not Pattern
    End Sub

    ''' <summary>
    ''' 提携区分リストの設定を行います。
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetListInitialize()
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim DBCloseFlg As Boolean = False

        Try
            '----------------------------------------
            'リストビュー初期化
            '----------------------------------------
            Me.ListView1.Clear()
            Me.ListView1.Columns.Add("", 0, HorizontalAlignment.Center)
            Me.ListView1.Columns.Add("金融機関コード", 125, HorizontalAlignment.Center)
            Me.ListView1.Columns.Add("金融機関名", 255, HorizontalAlignment.Left)
            Me.ListView1.Columns.Add("提携区分", 80, HorizontalAlignment.Center)

            '----------------------------------------
            '提携区分テキスト設定
            '----------------------------------------
            Dim TeikeiKbnText As New CAstFormat.ClsText("KFUMAST010_提携区分.TXT")

            '----------------------------------------
            '提携区分マスタ参照
            '----------------------------------------
            If Me.dbConn Is Nothing Then
                Me.dbConn = New CASTCommon.MyOracle
                DBCloseFlg = True
            End If

            OraReader = New CASTCommon.MyOracleReader(Me.dbConn)

            Dim SQL As New StringBuilder
            With SQL
                .Append("select")
                .Append("     SSS_TKBNMAST.KIN_NO_N")
                .Append("    ,TENMAST.KIN_NNAME_N")
                .Append("    ,SSS_TKBNMAST.TEIKEI_KBN_N")
                .Append(" from")
                .Append("     SSS_TKBNMAST")
                .Append(" left outer join")
                .Append("     TENMAST")
                .Append(" on  SSS_TKBNMAST.KIN_NO_N = TENMAST.KIN_NO_N")
                .Append(" group by")
                .Append("     SSS_TKBNMAST.KIN_NO_N")
                .Append("    ,TENMAST.KIN_NNAME_N")
                .Append("    ,SSS_TKBNMAST.TEIKEI_KBN_N")
                .Append(" order by")
                .Append("     SSS_TKBNMAST.KIN_NO_N")
            End With

            If OraReader.DataReader(SQL) = True Then
                Dim ROW As Integer = 0

                While OraReader.EOF = False
                    Dim Data(3) As String

                    Data(1) = OraReader.GetString("KIN_NO_N")
                    If OraReader.GetString("KIN_NNAME_N") = String.Empty Then
                        Data(2) = "※※金融機関マスタ登録なし※※"
                    Else
                        Data(2) = OraReader.GetString("KIN_NNAME_N")
                    End If
                    Data(3) = TeikeiKbnText.GetBaitaiCode(OraReader.GetString("TEIKEI_KBN_N"))

                    Dim LineColor As Color
                    If ROW Mod 2 = 0 Then
                        LineColor = Color.White
                    Else
                        LineColor = Color.WhiteSmoke
                    End If

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    ROW += 1

                    OraReader.NextRead()
                End While
            End If

            OraReader.Close()
            OraReader = Nothing

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "提携区分リストビュー設定", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If DBCloseFlg = True Then
                If Not Me.dbConn Is Nothing Then
                    Me.dbConn.Close()
                    Me.dbConn = Nothing
                End If
            End If
        End Try
    End Sub

#End Region

End Class