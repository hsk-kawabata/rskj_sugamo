Imports System.Text
Imports CASTCommon

''' <summary>
''' 委託者別決済額一覧表印刷画面
''' </summary>
''' <remarks></remarks>
Public Class KF3SPRNT020

#Region "クラス定数"
    Private MainLOG As New CASTCommon.BatchLOG("KF3SPRNT020", "委託者別決済額一覧表印刷画面")
    Private Const msgTitle As String = "委託者別決済額一覧表印刷画面(KF3SPRNT020)"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
#End Region

#Region "クラス変数"
    Private Structure LogWrite
        Dim UserID As String        'ユーザID
        Dim ToriCode As String      '取引先コード
        Dim FuriDate As String      '振替日
    End Structure
    Private LW As LogWrite

    Private Structure strcIniInfo
        Dim FUNOU_SSS_1 As String   'SSS分不能結果更新期日
    End Structure
    Dim IniInfo As strcIniInfo

    Private MainDB As CASTCommon.MyOracle

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KF3SPRNT020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '------------------------------------------------
            'ログの書込に必要な情報の取得
            '------------------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '------------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Me.Label1, Me.Label2, Me.lblUser, Me.lblDate)

            '------------------------------------------------
            '設定ファイル読み込み
            '------------------------------------------------
            If GetIniInfo() = False Then
                Return
            End If

            '------------------------------------------------
            '休日マスタ取り込み
            '------------------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)", "失敗")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            '------------------------------------------------
            '画面表示時に振替日に3営業日前を表示する
            '------------------------------------------------
            Dim strSysDate As String = String.Empty
            Dim strFuriDate As String = String.Empty
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            bRet = GCom.CheckDateModule(strSysDate, strFuriDate, CInt(IniInfo.FUNOU_SSS_1), 1)
            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)", "失敗")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            Me.txtFuriDateY.Text = strFuriDate.Substring(0, 4)
            Me.txtFuriDateM.Text = strFuriDate.Substring(4, 2)
            Me.txtFuriDateD.Text = strFuriDate.Substring(6, 2)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 印刷ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            '------------------------------------------------
            '画面入力値のチェック
            '------------------------------------------------
            If fn_check_text() = False Then
                Return
            End If

            '------------------------------------------------
            'テーブルのチェック
            '------------------------------------------------
            If fn_check_Table() = False Then
                Return
            End If

            Dim strFuriDate As String = Me.txtFuriDateY.Text & Me.txtFuriDateM.Text & Me.txtFuriDateD.Text
            LW.FuriDate = strFuriDate

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "委託者別決済額一覧表"), _
                               msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '------------------------------------------------
            '印刷バッチ呼び出し
            '------------------------------------------------
            Dim param As String
            Dim nRet As Integer
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            param = GCom.GetUserID & "," & strFuriDate
            nRet = ExeRepo.ExecReport("KF3SP002.EXE", param)

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, "委託者別決済額一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "委託者別決済額一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 終了ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' テキストボックスゼロ埋めイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
         Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' テキストボックスの入力チェックを行います。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_check_text() As Boolean
        Try
            '振替日(年)必須チェック
            If Me.txtFuriDateY.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            '振替日(月)必須チェック
            If Me.txtFuriDateM.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateM.Focus()
                Return False
            End If

            '振替日(月)範囲チェック
            If CInt(Me.txtFuriDateM.Text) < 1 OrElse CInt(Me.txtFuriDateM.Text) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateM.Focus()
                Return False
            End If

            '振替日(日)必須チェック
            If Me.txtFuriDateD.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateD.Focus()
                Return False
            End If

            '振替日(日)範囲チェック
            If CInt(Me.txtFuriDateD.Text) < 1 OrElse CInt(Me.txtFuriDateD.Text) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateD.Focus()
                Return False
            End If

            '振替日整合性チェック
            Dim WORK_DATE As String = Me.txtFuriDateY.Text & "/" & Me.txtFuriDateM.Text & "/" & Me.txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' テーブルのチェックを行います。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_check_Table() As Boolean
        MainDB = New CASTCommon.MyOracle
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("select count(*) as COUNTER")
                .Append(" from TORIMAST, SCHMAST")
                .Append(" where TORIS_CODE_T = TORIS_CODE_S")
                .Append(" and TORIF_CODE_T = TORIF_CODE_S")
                .Append(" and FURI_DATE_S = " & SQ(Me.txtFuriDateY.Text & Me.txtFuriDateM.Text & Me.txtFuriDateD.Text))
                .Append(" and FUNOU_FLG_S = '1'")
                .Append(" and TAKOU_FLG_S = '1'")
                .Append(" and FMT_KBN_T in ('20', '21')")
                .Append(" and TYUUDAN_FLG_S = '0'")
            End With

            Dim Count As Integer
            If oraReader.DataReader(SQL) = True Then
                Count = oraReader.GetInt("COUNTER")
                oraReader.Close()
            Else
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                oraReader.Close()
                Me.txtFuriDateY.Focus()
                Return False
            End If

            If Count = 0 Then
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Function

    ''' <summary>
    ''' 設定ファイルを読み込みます。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function GetIniInfo() As Boolean
        Try
            IniInfo.FUNOU_SSS_1 = GetFSKJIni("JIFURI", "FUNOU_SSS_1")
            If IniInfo.FUNOU_SSS_1.Equals("err") OrElse IniInfo.FUNOU_SSS_1 = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "SSS分不能結果期日", "JIFURI", "FUNOU_SSS_1"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:SSS分不能結果期日 分類:JIFURI 項目:FUNOU_SSS_1")
                Return False
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(設定ファイル取得)", "失敗", ex.Message)
            Return False
        End Try
    End Function

#End Region

End Class