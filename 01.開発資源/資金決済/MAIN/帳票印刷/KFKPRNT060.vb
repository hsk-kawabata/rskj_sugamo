Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT060

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT060", "預金口座振替内訳票手数料請求書印刷画面")
    Private Const msgTitle As String = "預金口座振替内訳票手数料請求書印刷画面(KFKPRNT060)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private PrintName As String  '帳票名
    Private PrintID As String    '帳票ID
    Private CsvNames() As String '取得CSVファイル名
    Private SyoriDate As String

    '2011/06/24 標準版修正 初期値は前営業日を表示 ------------------START
    Private FmtComm As New CAstFormat.CFormat
    '2011/06/24 標準版修正 初期値は前営業日を表示 ------------------END
#Region " ロード"
    Private Sub KFKPRNT060_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            Dim strSysDate As String
            '2011/06/24 標準版修正 初期値は前営業日を表示 ------------------START
            '2016/11/11 saitou RSV2 ADD 休日取得不具合対応 ---------------------------------------- START
            'オラクルコネクションをフォーマットクラスに渡して休日リストの作成を行う
            Dim MainDB As CASTCommon.MyOracle = Nothing
            Try
                MainDB = New CASTCommon.MyOracle
                Me.FmtComm.Oracle = MainDB

            Catch ex As Exception

            Finally
                If Not MainDB Is Nothing Then
                    MainDB.Close()
                    MainDB = Nothing
                End If
            End Try
            '2016/11/11 saitou RSV2 ADD ----------------------------------------------------------- END
            strSysDate = String.Format("{0:yyyyMMdd}", CASTCommon.GetEigyobi(Date.Now, -1, FmtComm.HolidayList))
            'strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            '2011/06/24 標準版修正 初期値は前営業日を表示 ------------------END


            txtFuriDateY.Text = strSysDate.Substring(0, 4)
            txtFuriDateM.Text = strSysDate.Substring(4, 2)
            txtFuriDateD.Text = strSysDate.Substring(6, 2)

            Dim Jyoken As String = " AND KESSAI_KBN_T <> '99'"
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            'コントロール制御

            txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region " 印刷"
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

            If fn_check_text() = False Then
                Return
            End If

            '------------------------------------------------
            '印刷対象先の件数チェック
            '------------------------------------------------
            Dim lngDataCNT As Long = 0

            If fn_Select_Data(String.Concat(New String() {txtFuriDateY.Text, txtFuriDateM.Text, txtFuriDateD.Text}), lngDataCNT) = False Then
                MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtTorisCode.Focus()
                Exit Sub
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(MSG0013I.Replace("{0}", "預金口座振替内訳票手数料請求書"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) <> DialogResult.Yes Then
                txtTorisCode.Focus()
                Return
            End If
            Dim param As String
            Dim nRet As Integer

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：ログイン名、ＣＳＶファイル名

            param = GCom.GetUserID & "," & String.Concat(txtTorisCode.Text, txtTorifCode.Text) & "," _
            & String.Concat(New String() {txtFuriDateY.Text, txtFuriDateM.Text, txtFuriDateD.Text})

            nRet = ExeRepo.ExecReport("KFKP009.EXE", param)

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    MessageBox.Show(MSG0014I.Replace("{0}", "預金口座振替内訳票手数料請求書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    MessageBox.Show(MSG0004E.Replace("{0}", "預金口座振替内訳票手数料請求書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region
#Region " 取消"
    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '=====================================================================================
        'NAME           :btnSearch_Click
        'Parameter      :
        'Description    :取消ボタン
        'Return         :
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")
            'コントロール制御
            txtFuriDateY.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 終了"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 関数"
    Public Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :更新ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        Try

            '取引先主コード必須チェック
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If
            '取引先副コード必須チェック
            If txtTorifCode.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorifCode.Focus()
                Return False
            End If

            '年必須チェック
            If txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
            '月必須チェック
            If txtFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '日付必須チェック
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.Message)
            Return False
        Finally

        End Try

        Return True

    End Function


    Private Function fn_Select_Data(ByVal astrFURI_DATE As String, ByRef alngDataCNT As Long) As Boolean
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
        Dim ToriCode As String = String.Concat(txtTorisCode.Text, txtTorifCode.Text)

        Try

            alngDataCNT = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(",TORIMAST")
            SQL.Append(" WHERE　FSYORI_KBN_T ='1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")

            If ToriCode <> "0".PadLeft(12, "0"c) Then

                SQL.Append(" AND TORIS_CODE_S = " & SQ(ToriCode.Substring(0, 10)))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(ToriCode.Substring(10, 2)))

            End If

            SQL.Append(" AND FURI_DATE_S = '" & astrFURI_DATE & "'")        '振替日がパラメータ日
            SQL.Append(" AND TESUUKEI_FLG_S = '1'")                       '手数料計算済
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")                       '0:有効

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

#End Region
#Region " イベント"
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
              Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating, txtTorisCode.Validating, txtTorifCode.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ゼロパディング", "失敗", ex.Message)
        End Try
    End Sub

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '選択カナで始まる委託者名を取得
        '--------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定(Msg????W)
                Dim Jyoken As String = " AND KESSAI_KBN_T <> '99'"
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コンボボックス設定)", "失敗", ex.ToString)
        End Try
    End Sub

    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        '-------------------------------------------
        '取引先コードテキストボックスに取引先コード設定
        '-------------------------------------------
        Try
            '取引先コンボボックス設定
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コード取得)", "失敗", ex.ToString)
        End Try
    End Sub

#End Region

End Class
