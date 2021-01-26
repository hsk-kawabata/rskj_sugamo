Imports System
Imports System.Text
Imports CASTCommon

''' <summary>
''' 端末マスタメンテナンス画面　メインクラス
''' </summary>
''' <remarks>2017/12/06 saitou 広島信金(RSV2標準) added for 大規模構築対応</remarks>
Public Class KFUMAST090

#Region "クラス変数"
    'メッセージ
    Private Const msgTitle As String = "端末マスタメンテナンス画面(KFUMAST090)"

    'イベント
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    'ログ
    Private MainLOG As New CASTCommon.BatchLOG("KFUMAST090", "端末マスタメンテナンス画面")
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
    Private Sub KFUMAST090_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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
            If Me.CheckTextBox(True) = False Then
                Return
            End If

            '----------------------------------------
            'テーブルの存在チェック
            '----------------------------------------
            Dim ComputerName As String = Me.txtComputerName.Text.Trim
            Dim StationNo As String = Me.txtStationNo.Text.Trim
            Me.dbConn = New CASTCommon.MyOracle
            If Me.CheckExistsTable(ComputerName) = True Then
                MessageBox.Show(MSG0389W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtComputerName.Focus()
                Return
            End If

            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '----------------------------------------
            'テーブル登録
            '----------------------------------------
            Dim SQL As New StringBuilder
            With SQL
                .Append("insert")
                .Append(" into STATION_TBL")
                .Append(" values(")
                .Append("     '" & ComputerName & "'")
                .Append("    ,'" & StationNo & "'")
                .Append("    ,'" & System.DateTime.Now.ToString("yyyyMMdd") & "'")
                .Append(" )")
            End With

            Dim iRet As Integer = Me.dbConn.ExecuteNonQuery(SQL)
            If iRet <> 1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "端末マスタ登録", "失敗", "戻り値:" & iRet.ToString)
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
            'テキストボックスの入力チェック
            '----------------------------------------
            If Me.CheckTextBox(True) = False Then
                Return
            End If

            '----------------------------------------
            'テーブルの存在チェック
            '----------------------------------------
            Dim ComputerName As String = Me.txtComputerName.Text.Trim
            Dim StationNo As String = Me.txtStationNo.Text.Trim
            Me.dbConn = New CASTCommon.MyOracle
            If Me.CheckExistsTable(ComputerName) = False Then
                MessageBox.Show(String.Format(MSG0258W, "入力されたコンピュータ名は端末マスタ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtComputerName.Focus()
                Return
            End If

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '----------------------------------------
            'テーブル更新
            '----------------------------------------
            Dim SQL As New StringBuilder
            With SQL
                .Append("update STATION_TBL")
                .Append(" set")
                .Append("     STATION_NO = '" & StationNo & "'")
                .Append(" where")
                .Append("     COMPUTER_NAME = '" & ComputerName & "'")
            End With

            Dim iRet As Integer = Me.dbConn.ExecuteNonQuery(SQL)
            'キーで更新なので、必ず1レコード更新
            If iRet <> 1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "端末マスタ更新", "失敗", "戻り値:" & iRet.ToString)
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
            If Me.CheckTextBox(False) = False Then
                Return
            End If

            '----------------------------------------
            'テーブルの存在チェック
            '----------------------------------------
            Dim ComputerName As String = Me.txtComputerName.Text.Trim
            Me.dbConn = New CASTCommon.MyOracle
            If Me.CheckExistsTable(ComputerName) = False Then
                MessageBox.Show(String.Format(MSG0258W, "入力されたコンピュータ名は端末マスタ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtComputerName.Focus()
                Return
            End If

            '----------------------------------------
            '内容設定
            '----------------------------------------
            Dim StationNo As String = String.Empty
            OraReader = New CASTCommon.MyOracleReader(Me.dbConn)
            If OraReader.DataReader(GetExistsSQL(ComputerName)) = True Then
                While OraReader.EOF = False
                    StationNo = OraReader.GetString("STATION_NO")
                    OraReader.NextRead()
                End While
            End If

            'テキストボックス非活性
            Me.txtComputerName.Enabled = False
            'ボタン設定
            Me.SetButtonEnabled(False)

            Me.txtStationNo.Text = StationNo
            Me.txtStationNo.Focus()

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
            Dim ComputerName As String = Me.txtComputerName.Text.Trim
            Me.dbConn = New CASTCommon.MyOracle
            If Me.CheckExistsTable(ComputerName) = False Then
                MessageBox.Show(String.Format(MSG0258W, "入力されたコンピュータ名は端末マスタ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtComputerName.Focus()
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
                .Append("     STATION_TBL")
                .Append(" where")
                .Append("     COMPUTER_NAME = '" & ComputerName & "'")
            End With

            Dim iRet As Integer = Me.dbConn.ExecuteNonQuery(SQL)
            'キーで削除なので、必ず1レコード削除
            If iRet <> 1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "端末マスタ削除", "失敗", "戻り値:" & iRet.ToString)
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

    ''' <summary>
    ''' コンピュータ名テキストボックス　バリデイティングイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub txtComputerName_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtComputerName.Validating
        Call GCom.NzCheckString(CType(sender, TextBox))
        Call GCom.CheckZenginChar(CType(sender, TextBox))
    End Sub

    ''' <summary>
    ''' 端末リストビュー ダブルクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(端末リストビューダブルクリック)開始", "成功")

            If Me.ListView1.Items.Count <= 0 Then
                Return
            End If

            Dim ComputerName As String = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 1))
            Dim StationNo As String = GCom.NzStr(GCom.SelectedItem(Me.ListView1, 2))

            '参照ボタンクリックイベントを実行
            If Me.btnSansyo.Enabled = True Then
                '参照ボタンが活性化されているときにクリックイベント実行
                '非活性時はイベントが走らない
                Me.txtComputerName.Text = ComputerName.Trim
                Me.btnSansyo.PerformClick()
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(端末リストビューダブルクリック)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(端末リストビューダブルクリック)終了", "成功")
        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' テキストボックスの入力チェックを行います。
    ''' </summary>
    ''' <param name="Mode">チェック区分(True - 登録,更新 , False - 参照,削除)</param>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function CheckTextBox(Mode As Boolean) As Boolean
        Try
            'コンピュータ名必須チェック
            If Me.txtComputerName.Text.Trim = String.Empty Then
                MessageBox.Show(String.Format(MSG0285W, "コンピュータ名"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtComputerName.Focus()
                Return False
            End If

            If Mode = True Then
                '端末番号必須チェック
                If Me.txtStationNo.Text.Trim = String.Empty Then
                    MessageBox.Show(String.Format(MSG0285W, "端末番号"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtStationNo.Focus()
                    Return False
                End If
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 端末マスタを取得するSQLを作成します。
    ''' </summary>
    ''' <param name="ComputerName">対象コンピュータ名</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function GetExistsSQL(ByVal ComputerName As String) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select")
            .Append("     COMPUTER_NAME")
            .Append("    ,STATION_NO")
            .Append(" from")
            .Append("     STATION_TBL")
            .Append(" where")
            .Append("     COMPUTER_NAME = '" & ComputerName & "'")
            .Append(" order by")
            .Append("     STATION_NO")
            .Append("    ,COMPUTER_NAME")
        End With

        Return SQL
    End Function

    ''' <summary>
    ''' 端末マスタの存在チェックを行います。
    ''' </summary>
    ''' <param name="ComputerName">対象コンピュータ名</param>
    ''' <returns>True - 対象有り , False - 対象無し</returns>
    ''' <remarks></remarks>
    Private Function CheckExistsTable(ByVal ComputerName As String) As Boolean
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            OraReader = New CASTCommon.MyOracleReader(Me.dbConn)
            If OraReader.DataReader(Me.GetExistsSQL(ComputerName)) = True Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "端末マスタ存在チェック", "失敗", ex.Message)
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
            Me.txtComputerName.Text = String.Empty
            Me.txtStationNo.Text = String.Empty
            Me.txtComputerName.Enabled = True

            '----------------------------------------
            'ボタンの設定
            '----------------------------------------
            Call Me.SetButtonEnabled(True)

            '----------------------------------------
            'カーソル設定
            '----------------------------------------
            Me.txtComputerName.Focus()

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
            Me.ListView1.Columns.Add("コンピュータ名", 255, HorizontalAlignment.Left)
            Me.ListView1.Columns.Add("端末番号", 80, HorizontalAlignment.Center)
            Me.ListView1.Columns.Add("登録日", 125, HorizontalAlignment.Center)

            '----------------------------------------
            '端末マスタ参照
            '----------------------------------------
            If Me.dbConn Is Nothing Then
                Me.dbConn = New CASTCommon.MyOracle
                DBCloseFlg = True
            End If

            OraReader = New CASTCommon.MyOracleReader(Me.dbConn)

            Dim SQL As New StringBuilder
            With SQL
                .Append("select")
                .Append("     COMPUTER_NAME")
                .Append("    ,STATION_NO")
                .Append("    ,CREATE_DATE")
                .Append(" from")
                .Append("     STATION_TBL")
                .Append(" order by")
                .Append("     STATION_NO")
                .Append("    ,COMPUTER_NAME")
            End With

            If OraReader.DataReader(SQL) = True Then
                Dim ROW As Integer = 0

                While OraReader.EOF = False
                    Dim Data(3) As String

                    Data(1) = OraReader.GetString("COMPUTER_NAME")
                    Data(2) = OraReader.GetString("STATION_NO")
                    Dim CreateDate As String = OraReader.GetString("CREATE_DATE").PadLeft(8, "0"c)
                    Data(3) = CreateDate.Substring(0, 4) & "年" & CreateDate.Substring(4, 2) & "月" & CreateDate.Substring(6, 2) & "日"

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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "端末リストビュー設定", "失敗", ex.Message)
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