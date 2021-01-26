Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT010

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT010", "自振処理企業一覧表印刷画面")
    Private Const msgTitle As String = "自振処理企業一覧表印刷画面(KFKPRNT010)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    Private Sub KFKPRNT010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            '全休日情報を蓄積
            '------------------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時に振替日に前営業日を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strSysDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
            Dim strGetdate As String = ""
            Call GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)
            Me.txtFuriDateY.Text = strGetdate.Substring(0, 4)
            Me.txtFuriDateM.Text = strGetdate.Substring(4, 2)
            Me.txtFuriDateD.Text = strGetdate.Substring(6, 2)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    '印刷ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        '振替日の取得
        Dim strFURI_DATE As String

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            '------------------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------------------

            If fn_check_TEXT() = False Then Exit Sub

            '------------------------------------------------
            '印刷対象先の件数チェック
            '------------------------------------------------
            Dim lngDataCNT As Long = 0
            If fn_Select_Data(strFURI_DATE, lngDataCNT) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtFuriDateY.Focus()
                Exit Sub
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(MSG0013I.Replace("{0}", "自振処理企業一覧表"), msgTitle, MessageBoxButtons.YesNo, _
                               MessageBoxIcon.Question) <> DialogResult.Yes Then
                txtFuriDateY.Focus()
                Return
            End If

            '------------------------------------------------
            ' 自振処理企業一覧表印刷バッチの呼び出し
            ' 引数：振替日
            ' 戻り値:1:正常-1:件数0件　以外:エラー
            '------------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            Dim Param As String = GCom.GetUserID & "," & strFURI_DATE
            Dim nRet As Integer = ExeRepo.ExecReport("KFKP004.EXE", Param)

            If nRet = 0 Then
                '正常
                MessageBox.Show(MSG0014I.Replace("{0}", "自振処理企業一覧表"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            ElseIf nRet = -1 Then
                '対象0件
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                '異常　レポエージェントエラーコードを受けたい
                MessageBox.Show(MSG0004E.Replace("{0}", "自振処理企業一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.Message)
        Finally
            'btnEnd.Focus()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try

    End Sub

    '終了ボタン
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

#Region "KFJKESS0400G用関数"

    Private Function fn_check_TEXT() As Boolean
        '============================================================================
        'NAME           :fn_check_TEXT
        'Parameter      :
        'Description    :印刷ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/09/10
        'Update         :
        '============================================================================
        Try

            '------------------------------------------------
            '振替年チェック
            '------------------------------------------------
            '必須チェック
            If txtFuriDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '振替月チェック
            '------------------------------------------------
            '必須チェック
            If txtFuriDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtFuriDateM.Text >= 1 And txtFuriDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '振替日チェック
            '------------------------------------------------
            '必須チェック
            If txtFuriDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '桁数チェック
            If Not (txtFuriDateD.Text >= 1 And txtFuriDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
        End Try

    End Function

    ''' <summary>
    ''' fn_Select_Data
    ''' </summary>
    ''' <param name="astrFURI_DATE"></param>
    ''' <param name="alngDataCNT"></param>
    ''' <returns>True=OK(対象あり) False=NG(対象なし)</returns>
    ''' <remarks></remarks>
    Private Function fn_Select_Data(ByVal astrFURI_DATE As String, ByRef alngDataCNT As Long) As Integer


        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try

            alngDataCNT = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM V1_KESMAST")
            SQL.Append(" WHERE")
            SQL.Append("     FURI_DATE_SV1 = '" & astrFURI_DATE & "'")
            SQL.Append(" AND SYORI_KIN_SV1 > 0")
            SQL.Append(" AND TOUROKU_FLG_SV1 = '1'")
            SQL.Append(" AND FUNOU_FLG_SV1 = '1'")
            SQL.Append(" AND TESUUKEI_FLG_SV1 = '1'") '2009/09/23 追加
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
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtFuriDateY.Validating, _
            txtFuriDateM.Validating, _
            txtFuriDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

End Class
