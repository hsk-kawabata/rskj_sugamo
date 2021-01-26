Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' 他行分不能結果更新画面
''' </summary>
''' <remarks></remarks>
Public Class KF3SMAIN020

#Region "クラス定数"
    Private MainLOG As New CASTCommon.BatchLOG("KF3SMAIN020", "他行分不能結果更新画面")
    Private Const msgTitle As String = "他行分不能結果更新画面(KF3SMAIN020)"
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
        Dim DEN As String           'DENフォルダ
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
    Private Sub KF3SMAIN020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
    ''' 実行ボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        MainDB = New CASTCommon.MyOracle

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            '------------------------------------------------
            '画面入力値のチェック
            '------------------------------------------------
            If fn_check_text() = False Then
                Return
            End If
            Dim strFuriDate As String = Me.txtFuriDateY.Text & Me.txtFuriDateM.Text & Me.txtFuriDateD.Text
            LW.FuriDate = strFuriDate

            If File.Exists(Path.Combine(IniInfo.DEN, "FUNOU_SYUKINDAIKOU.DAT")) = False Then
                MessageBox.Show(String.Format(MSG0255W, "不能"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                If MessageBox.Show(String.Format(MSG0023I, "他行分不能結果更新"), _
                                   msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                    Return
                End If
            End If

            '------------------------------------------------
            'ジョブマスタに登録
            '------------------------------------------------
            Dim JOBID As String = "J040"
            Dim PARAM As String = String.Empty
            If Me.rdbKIGYO_SEQ.Checked = True Then
                '更新キー項目が企業シーケンス
                PARAM = strFuriDate & ",4,0,20"
            Else
                '更新キー項目が口座情報
                PARAM = strFuriDate & ",4,1,20"
            End If

            Dim nRet As Integer = MainLOG.SearchJOBMAST(JOBID, PARAM, MainDB)
            If nRet = 1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf nRet = -1 Then
                Throw New Exception(String.Format(MSG0002E, "検索"))
            End If

            If MainLOG.InsertJOBMAST(JOBID, LW.UserID, PARAM, MainDB) = False Then
                Throw New Exception(MSG0005E)
            End If

            MainDB.Commit()

            MessageBox.Show(String.Format(MSG0021I, "他行分不能結果更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
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
    ''' 設定ファイルを読み込みます。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function GetIniInfo() As Boolean
        Try
            IniInfo.DEN = GetFSKJIni("COMMON", "DEN")
            If IniInfo.DEN.Equals("err") OrElse IniInfo.DEN = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "DENフォルダ", "COMMON", "DEN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:DENフォルダ 分類:COMMON 項目:DEN")
                Return False
            End If

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