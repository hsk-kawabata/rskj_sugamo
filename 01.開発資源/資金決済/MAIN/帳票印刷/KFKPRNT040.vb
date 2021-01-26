Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT040

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT040", "手数料一括徴求明細表印刷画面")
    Private Const msgtitle As String = "手数料一括徴求明細表印刷画面(KFKPRNT040)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private Sub KFKPRNT040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            txtKessaiDateY.Text = strGetdate.Substring(0, 4)
            txtKessaiDateM.Text = strGetdate.Substring(4, 2)
            txtKessaiDateD.Text = strGetdate.Substring(6, 2)
            '2009/12/03 追加 =======================================
            txtKessaiDateY2.Text = strGetdate.Substring(0, 4)
            txtKessaiDateM2.Text = strGetdate.Substring(4, 2)
            txtKessaiDateD2.Text = strGetdate.Substring(6, 2)
            '=======================================================
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

            '決済日の取得
            Dim strKESSAI_DATE As String
            strKESSAI_DATE = txtKessaiDateY.Text & txtKessaiDateM.Text & txtKessaiDateD.Text
            Dim strKESSAI_DATE2 As String
            strKESSAI_DATE2 = txtKessaiDateY2.Text & txtKessaiDateM2.Text & txtKessaiDateD2.Text
            '------------------------------------------------
            '印刷対象先の件数チェック
            '------------------------------------------------
            Dim lngDataCNT As Long = 0

            If fn_Select_Data(strKESSAI_DATE, strKESSAI_DATE2, lngDataCNT) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtKessaiDateY.Focus()
                Exit Sub
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0106W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(MSG0013I.Replace("{0}", "手数料一括徴求明細表"), _
                               msgtitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                txtKessaiDateY.Focus()
                Return
            End If

            '------------------------------------------------
            ' 手数料一括徴求明細表印刷バッチの呼び出し
            ' 引数：決済日
            ' 戻り値:1:正常　-1:件数0件　以外:エラー
            '------------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            '2009/12/01 引数変更 =========================================
            'Dim Param As String = GCom.GetUserID & "," & strKESSAI_DATE
            Dim Param As String = GCom.GetUserID & "," & strKESSAI_DATE & "," & strKESSAI_DATE2
            '=============================================================
            Dim nRet As Integer = ExeRepo.ExecReport("KFKP008.EXE", Param)

            If nRet = 0 Then
                '正常
                MessageBox.Show(MSG0014I.Replace("{0}", "手数料一括徴求明細表"), _
                                msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf nRet = -1 Then
                '対象0件
                MessageBox.Show(MSG0106W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '異常　レポエージェントエラーコードを受けたい
                MessageBox.Show(MSG0004E.Replace("{0}", "手数料一括徴求明細表"), _
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try

    End Sub

#Region "KFKPRNT040用関数"

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
            '決済年チェック
            '------------------------------------------------
            '必須チェック
            If txtKessaiDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '決済月チェック
            '------------------------------------------------
            '必須チェック
            If txtKessaiDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtKessaiDateM.Text >= 1 And txtKessaiDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '決済日チェック
            '------------------------------------------------
            '必須チェック
            If txtKessaiDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtKessaiDateD.Text >= 1 And txtKessaiDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = txtKessaiDateY.Text & "/" & txtKessaiDateM.Text & "/" & txtKessaiDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Return False
            End If

            '2009/12/01 チェック追加 ========================
            '------------------------------------------------
            '決済年チェック
            '------------------------------------------------
            '必須チェック
            If txtKessaiDateY2.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY2.Focus()
                Return False
            End If

            '------------------------------------------------
            '決済月チェック
            '------------------------------------------------
            '必須チェック
            If txtKessaiDateM2.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM2.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtKessaiDateM2.Text >= 1 And txtKessaiDateM2.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateM2.Focus()
                Return False
            End If

            '------------------------------------------------
            '決済日チェック
            '------------------------------------------------
            '必須チェック
            If txtKessaiDateD2.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD2.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtKessaiDateD2.Text >= 1 And txtKessaiDateD2.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateD2.Focus()
                Return False
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE2 As String = txtKessaiDateY2.Text & "/" & txtKessaiDateM2.Text & "/" & txtKessaiDateD2.Text

            If Information.IsDate(WORK_DATE2) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY2.Focus()
                Return False
            End If

            '日付前後チェック
            If WORK_DATE > WORK_DATE2 Then
                MessageBox.Show(MSG0099W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKessaiDateY.Focus()
                Return False
            End If
            '=================================================
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.Message)
            Return False
        End Try

        Return True

    End Function

    Private Function fn_Select_Data(ByVal astrKESSAI_DATE As String, ByVal astrKESSAI_DATE2 As String, ByRef alngDataCNT As Long) As Boolean
        '============================================================================
        'NAME           :fn_Select_Data
        'Parameter      :
        'Description    :印刷対象先があるかどうか
        'Return         :True=OK(対象あり),False=NG(対象なし)
        'Create         :
        'Update         :2009/12/01 引数追加
        '============================================================================

        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try

            alngDataCNT = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(",TORIMAST")
            SQL.Append(" WHERE　FSYORI_KBN_T ='1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND TESUUKEI_FLG_S = '1' ")                        '1:手数料計算済
            '2009/12/01 条件変更 =============
            'SQL.Append(" AND TESUU_YDATE_S = '" & astrKESSAI_DATE & "'")
            SQL.Append(" AND TESUU_YDATE_S >= '" & astrKESSAI_DATE & "'")
            SQL.Append(" AND TESUU_YDATE_S <= '" & astrKESSAI_DATE2 & "'")
            '=================================
            SQL.Append(" AND TESUUTYO_FLG_S = '0'")                         '0:手数料未徴求
            SQL.Append(" AND TESUUTYO_KBN_T = '1'")                         '1:一括徴求
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")                           '0:有効
            SQL.Append(" AND TOUROKU_FLG_S = '1'")                          '1:登録済
            SQL.Append(" AND TESUU_KIN_S > 0")                              '手数料金額

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
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKessaiDateY.Validating, txtKessaiDateM.Validating, txtKessaiDateD.Validating, txtKessaiDateM2.Validating, txtKessaiDateD2.Validating, txtKessaiDateY2.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

End Class