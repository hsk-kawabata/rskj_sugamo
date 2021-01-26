Imports System
Imports System.Text
Imports CASTCommon

''' <summary>
''' 一括照合画面
''' </summary>
''' <remarks>2017/12/04 saitou 広島信金(RSV2標準版) added for 大規模構築対応</remarks>
Public Class KFJMAIN140

#Region "クラス定数"
    Private Const msgTitle As String = "一括照合画面(KFJMAIN140)"

#End Region

#Region "クラス変数"
    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN140", "一括照合画面")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KFJMAIN140_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '--------------------------------------------------
            'ログの書込に必要な情報の取得
            '--------------------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '--------------------------------------------------
            'システム日付とユーザ名を表示
            '--------------------------------------------------
            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            If MessageBox.Show(String.Format(MSG0023I, "一括照合処理"), _
                               msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Return
            End If

            '--------------------------------------------------
            '対象スケジュールの更新
            '--------------------------------------------------
            Me.MainDB = New CASTCommon.MyOracle
            If Me.UpdateSchmast() = False Then
                Me.MainDB.Rollback()
                Return
            End If

            If Me.UpdateMediaEntryTbl() = False Then
                Me.MainDB.Rollback()
                Return
            End If

            '--------------------------------------------------
            'ジョブマスタに登録
            '--------------------------------------------------
            Dim jobid As String = "J110"
            Dim para As String = "0,1"      '連携区分,振替処理区分(連携区分は無いので暫定で0を設定)
            Dim iRet As Integer = MainLOG.SearchJOBMAST(jobid, para, Me.MainDB)
            If iRet = 1 Then
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
                Return
            ElseIf iRet = -1 Then
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainDB.Rollback()
                Return
            End If

            If MainLOG.InsertJOBMAST(jobid, GCom.GetUserID, para, Me.MainDB) = False Then
                MainDB.Rollback()
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                MainDB.Commit()
                MessageBox.Show(String.Format(MSG0021I, "一括照合処理"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If

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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 対象スケジュールの更新を行います。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function UpdateSchmast() As Boolean
        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("update SCHMAST_SUB")
                .Append(" set SYOUGOU_FLG_S = '8'")
                .Append("    ,SYOUGOU_DATE_S = " & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))
                .Append(" where")
                .Append("     SYOUGOU_FLG_S in ('0', '9')")
                .Append(" and FURI_DATE_SSUB >= " & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))
                .Append(" and exists (")
                .Append("     select")
                .Append("         *")
                .Append("     from")
                .Append("         SCHMAST")
                .Append("     inner join")
                .Append("         TORIMAST_VIEW")
                .Append("     on  TORIS_CODE_S = TORIS_CODE_T")
                .Append("     and TORIF_CODE_S = TORIF_CODE_T")
                .Append("     where")
                .Append("         TORIS_CODE_SSUB = TORIS_CODE_S")
                .Append("     and TORIF_CODE_SSUB = TORIF_CODE_S")
                .Append("     and FURI_DATE_SSUB = FURI_DATE_S")
                .Append("     and UKETUKE_FLG_S = '1'")
                .Append("     and TOUROKU_FLG_S = '0'")
                .Append("     and TYUUDAN_FLG_S = '0'")
                .Append("     and SYOUGOU_KBN_T = '1'")
                .Append(")")
            End With
            Dim iRet As Integer = Me.MainDB.ExecuteNonQuery(SQL)
            If iRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタ更新", "失敗", Me.MainDB.Message)
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタ更新", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 対象の送付状実績テーブルの更新を行います。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function UpdateMediaEntryTbl() As Boolean
        Dim SQL As New StringBuilder

        Try
            With SQL
                .Append("update MEDIA_ENTRY_TBL")
                .Append(" set CHECK_FLG_ME = '8'")
                .Append("    ,CYCLE_NO_ME = 0")
                .Append("    ,UPDATE_DATE_ME = " & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMddHHmmss")))
                .Append(" where")
                .Append("     UPLOAD_FLG_ME = '1'")
                .Append(" and CHECK_KBN_ME = '1'")
                .Append(" and CHECK_FLG_ME in ('0', '9')")
                .Append(" and FURI_DATE_ME >= " & CASTCommon.Calendar.Now.ToString("yyyyMMdd"))
                .Append(" and FSYORI_KBN_ME = '1'")
                .Append(" and DELETE_FLG_ME = '0'")
            End With
            Dim iRet As Integer = Me.MainDB.ExecuteNonQuery(SQL)
            If iRet < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "送付状実績テーブル更新", "失敗", Me.MainDB.Message)
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "送付状実績テーブル更新", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

    End Function

#End Region

End Class