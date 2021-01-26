Option Strict On
Option Explicit On

Imports System.Text
Imports CASTCommon

Public Class KFSPRNT040

    Private MainLOG As New CASTCommon.BatchLOG("KFSPRNT040", "取引先マスタ索引簿印刷画面")
    Private Const msgTitle As String = "取引先マスタ索引簿印刷画面(KFSPRNT040)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure

    Private LW As LogWrite

    Private CAST As New CASTCommon.Events

#Region "ロード"
    Private Sub KFSPRNT040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'ログの書込に必要な情報の取得
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "0000000000-00"
        LW.FuriDate = "00000000"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label3, Label2, lblUser, lblDate)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region "終了"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region "印刷"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :印刷ボタン
        'Return         :
        'Create         :2009/09/15
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "取引先マスタ索引簿"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            Dim param As String
            Dim nRet As Integer

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：ログイン名、ＣＳＶファイル名、取引先コード
            If rdbToriCd.Checked Then
                param = GCom.GetUserID & ",1"
            ElseIf rdbSyubetu.Checked Then
                param = GCom.GetUserID & ",2"
            Else
                param = GCom.GetUserID & ",3"
            End If

            nRet = ExeRepo.ExecReport("KFSP029.EXE", param)

            '印刷失敗：戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, "取引先マスタ索引簿"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "取引先マスタ索引簿"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"
        End Try

    End Sub
#End Region

#Region " 関数"
    Private Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :印刷ボタンを押下時にマスタ相関チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/15
        'Update         :
        '============================================================================
        fn_check_Table = False
        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)

        Try
            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim lngDataCNT As Long = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM S_TORIMAST")

            If OraReader.DataReader(SQL) = True Then
                lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            fn_check_Table = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function

#End Region

End Class