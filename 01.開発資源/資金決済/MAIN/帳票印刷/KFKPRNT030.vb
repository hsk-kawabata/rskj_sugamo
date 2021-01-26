Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT030

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT030", "手数料未徴求企業一覧表印刷画面")
    Private Const msgtitle As String = "手数料未徴求企業一覧表印刷画面(KFKPRNT030)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private Sub KFKPRNT030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '------------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------------
            '画面表示時、基準日にシステム日付を表示
            '------------------------------------------------
            '休日マスタ取り込み
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MessageBox.Show(MSG0003E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時に振替日に前営業日を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strGetdate As String = ""

            strGetdate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            'bRet = GCom.CheckDateModule(strGetdate, strGetdate, 1, 1)
            txtKijyunDateY.Text = strGetdate.Substring(0, 4)
            txtKijyunDateM.Text = strGetdate.Substring(4, 2)
            txtKijyunDateD.Text = strGetdate.Substring(6, 2)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    '印刷ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            '------------------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------------------
            If fn_check_TEXT() = False Then Exit Sub

            '基準日の取得
            Dim strKIJYUN_DATE As String
            strKIJYUN_DATE = txtKijyunDateY.Text & txtKijyunDateM.Text & txtKijyunDateD.Text

            '------------------------------------------------
            '印刷対象先の件数チェック
            '------------------------------------------------
            Dim lngDataCNT As Long = 0
            If fn_Select_Data(strKIJYUN_DATE, lngDataCNT) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtKijyunDateY.Focus()
                Exit Sub
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0106W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Exit Sub
            End If


            '印刷前確認メッセージ
            If MessageBox.Show(MSG0013I.Replace("{0}", "手数料未徴求企業一覧表"), _
                               msgtitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                txtKijyunDateY.Focus()
                Return
            End If

            '------------------------------------------------
            ' 手数料未徴求企業一覧表印刷バッチの呼び出し
            ' 引数：基準日
            ' 戻り値:1:正常　-1:件数0件　以外:エラー
            '------------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            Dim Param As String = GCom.GetUserID & "," & strKIJYUN_DATE
            Dim nRet As Integer = ExeRepo.ExecReport("KFKP007.EXE", Param)

            If nRet = 0 Then
                '正常
                MessageBox.Show(MSG0014I.Replace("{0}", "手数料未徴求企業一覧表"), _
                                msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf nRet = -1 Then
                '対象0件
                MessageBox.Show(MSG0106W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '異常　レポエージェントエラーコードを受けたい
                MessageBox.Show(MSG0004E.Replace("{0}", "手数料未徴求企業一覧表"), _
                                msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try

    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")

            Me.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try

    End Sub

#Region "KFKPRNT030用関数"

    Private Function fn_check_TEXT() As Boolean
        '============================================================================
        'NAME           :fn_check_TEXT
        'Parameter      :
        'Description    :印刷ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :
        'Update         :
        '============================================================================
        Try
            '------------------------------------------------
            '基準年チェック
            '------------------------------------------------
            '必須チェック
            If txtKijyunDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '基準月チェック
            '------------------------------------------------
            '必須チェック
            If txtKijyunDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtKijyunDateM.Text >= 1 And txtKijyunDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '基準日チェック
            '------------------------------------------------
            '必須チェック
            If txtKijyunDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtKijyunDateD.Text >= 1 And txtKijyunDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = txtKijyunDateY.Text & "/" & txtKijyunDateM.Text & "/" & txtKijyunDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.Message)
            Return False
        Finally
        End Try

        Return True
    End Function

    Private Function fn_Select_Data(ByVal astrKIJYUN_DATE As String, ByRef alngDataCNT As Long) As Boolean
        '============================================================================
        'NAME           :fn_Select_Data
        'Parameter      :
        'Description    :印刷対象先があるかどうか
        'Return         :True=OK(対象あり),False=NG(対象なし)
        'Create         :
        'Update         :
        '============================================================================

        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try
            alngDataCNT = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM V1_KESMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TESUU_YDATE_SV1 <= '" & astrKIJYUN_DATE & "'")
            '2009/09/23 手数料徴求待ち・決済済み・中断フラグを条件に追加
            'SQL.Append(" AND TESUUTYO_FLG_SV1 = '0'")
            SQL.Append(" AND TESUUTYO_FLG_SV1 IN('0','2')")   '未徴求・徴求待ち
            SQL.Append(" AND TESUUKEI_FLG_SV1 = '1'")
            SQL.Append(" AND TYUUDAN_FLG_SV1 = '0'")
            '=====================================
            SQL.Append(" AND TESUU_KIN_SV1 > 0")
            SQL.Append(" AND TOUROKU_FLG_SV1 = '1'")
            SQL.Append(" AND FUNOU_FLG_SV1 = '1' ")
            ' 00:しんきん中金預け金,01:口座入金,02:為替振込,03:為替付替,04:特別企業,05:決済対象外
            'SQL.Append(" AND (KESSAI_KBN_TV1 = '00'")
            'SQL.Append("   OR KESSAI_KBN_TV1 = '01'")
            'SQL.Append("   OR KESSAI_KBN_TV1 = '02'")
            'SQL.Append("   OR KESSAI_KBN_TV1 = '03'")
            'SQL.Append("   OR KESSAI_KBN_TV1 = '04'")
            'SQL.Append("   OR KESSAI_KBN_TV1 = '05')")

            If OraReader.DataReader(SQL) = True Then
                alngDataCNT = OraReader.GetInt64("COUNTER")
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(データ検索)", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function

    'ゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKijyunDateY.Validating, txtKijyunDateM.Validating, txtKijyunDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

End Class
