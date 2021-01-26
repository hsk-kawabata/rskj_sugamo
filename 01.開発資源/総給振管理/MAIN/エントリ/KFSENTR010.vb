Option Strict On
Option Explicit On

Imports System
Imports CASTCommon
Imports System.Text

Public Class KFSENTR010
    Inherits System.Windows.Forms.Form

    Public FURI_DATE As String
    Private CAST As New CASTCommon.Events
    Private CASTx As New CASTCommon.Events("0-9a-zA-Z", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx03 As New CASTCommon.Events("0-9()-", CASTCommon.Events.KeyMode.GOOD)
    '' 非許可文字設定
    '2017/05/22 saitou RSV2 MODIFY 標準版（潜在バグ修正） ------------------------------------- START
    '「"」は許可しない。
    Private CASTx2 As New CASTCommon.Events("""", CASTCommon.Events.KeyMode.BAD)
    'Private CASTx2 As New CASTCommon.Events("", CASTCommon.Events.KeyMode.BAD)
    '2017/05/22 saitou RSV2 MODIFY ------------------------------------------------------------ END

    Private MainLOG As New CASTCommon.BatchLOG("KFSENTR010", "依頼書用振込口座登録画面")
    Private Const msgTitle As String = "依頼書用振込口座登録画面(KFSENTR010)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース
    Private Jikinko As String

#Region " 変数"
    Private strTEIKEI_KBN As String

    Private strFSYORI_KBN As String
    Private strFURI_CODE As String
    Private strKIGYO_CODE As String
    Private strTORI_NAME As String
    Private lngHYOUJI_NUM As Long
    Private lngKOKYAKU_NUM As Long
    Private lngRECORD_NUM As Long
    Private strSAVE_KIN_NO As String
    Private strSAVE_SIT_NO As String
    Private strSAVE_KAMOKU As String
    Private strSAVE_KOUZA As String
    Private strSAVE_FURI_DATE As String
    Private strSAVE_SINKI_CODE As String
    Private strBAITAI_CODE As String

    Private UP_DATA(20) As String
    Private ENTRY_DATA(39) As String

    Private strIN_KIN As String
    Private strIN_SIT As String
    Private strIN_KOUZA As String
    Private strIN_KAMOKU As String
    Private strIN_KokyakuName As String

    'SQLキー項目用
    Private strKEY1 As String
    Private strKEY2 As String
    Private strKEY3 As String
    Private strKEY4 As String

    Public lngTUUBAN As Long

#End Region

#Region " ロード"
    Private Sub KFSENTR010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label2, Label4, lblUser, lblDate)


            If fn_Settei() = False Then
                Exit Sub
            End If

            '画面クリア
            If fn_clear_GAMEN() = False Then
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            Else
                cmbToriName.Enabled = True
                txtTorisCode.Enabled = True
                txtTorifCode.Enabled = True
                txtKeiyakuCode.Enabled = True
            End If
            txtTorisCode.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region " 登録"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle
            '------------------------------------
            'テキストボックスのキー項目入力チェック
            '------------------------------------
            If fn_check_text() = False Then
                '2012/03/23 saitou 標準修正 DEL ---------------------------------------->>>>
                'チェック関数でフォーカス移動を行っているため、ここでフォーカス移動しない
                'txtTorisCode.Focus()
                '2012/03/23 saitou 標準修正 DEL ----------------------------------------<<<<
                Exit Sub
            End If
            strKEY1 = txtTorisCode.Text.Trim
            strKEY2 = txtTorifCode.Text.Trim
            strKEY3 = txtKeiyakuCode.Text.Trim
            LW.ToriCode = strKEY1 + strKEY2

            'そのほかの画面項目入力チェック
            If fn_check_other() = False Then
                Exit Sub
            End If

            '取引先マスタに取引先コードが存在することを確認
            If fn_select_TORIMAST(strKEY1, strKEY2) = False Then
                txtTorisCode.Focus()
                Exit Sub
            End If

            'S_ENTMAST存在チェック
            If fn_select_ENTMAST() = True Then '検索にヒットしたら
                MessageBox.Show(MSG0222W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            End If


            lngHYOUJI_NUM = 0
            If fn_select_HYOJIJUN() = False Then
                txtTorisCode.Focus()
                Exit Sub
            End If

            ''顧客番号算出
            If lngKOKYAKU_NUM = 9999999999 Then
                'MessageBox.Show("顧客番号が９９９９９９９９９９を超えました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'Exit Sub
                lngKOKYAKU_NUM = 9999999998
            End If
            lngKOKYAKU_NUM = lngKOKYAKU_NUM + 1
            ''レコード番号算出
            'If lngRECORD_NUM = 9999 Then
            '    MessageBox.Show("レコード番号が９９９９を超えました" & vbCrLf & "ソート処理を実行後、再度登録してください", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If
            'lngRECORD_NUM = lngRECORD_NUM + 1
            lngRECORD_NUM = 0
            lblKokyakuCode.Text = Format(lngKOKYAKU_NUM, "0000000000")

            '確認MSG
            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '登録データセット
            If fn_ENTDATA_SET() = False Then
                txtTorisCode.Focus()
                Exit Sub
            End If
            MainDB.BeginTrans()
            If fn_insert_ENTMAST() = True Then  '登録成功
                MainDB.Commit()
                MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MainDB.Rollback()
                txtTorisCode.Focus()
                Exit Sub
            End If

            '画面クリア
            If fn_clear_GAMEN() = False Then
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            Else
                cmbKana.Enabled = True
                cmbToriName.Enabled = True
                txtTorisCode.Enabled = True
                txtTorifCode.Enabled = True
                txtKeiyakuCode.Enabled = True
            End If
            txtTorisCode.Focus()

        Catch ex As Exception
            If MainDB IsNot Nothing Then
                MainDB.Rollback()
            End If
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region

#Region " 更新"
    Private Sub btnKousin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKousin.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle
            '------------------------------------
            'テキストボックスのキー項目入力チェック
            '------------------------------------
            If fn_check_text() = False Then
                '2012/03/23 saitou 標準修正 DEL ---------------------------------------->>>>
                'チェック関数でフォーカス移動を行っているため、ここでフォーカス移動しない
                'txtTorisCode.Focus()
                '2012/03/23 saitou 標準修正 DEL ----------------------------------------<<<<
                Exit Sub
            Else
                strKEY1 = txtTorisCode.Text.Trim
                strKEY2 = txtTorifCode.Text.Trim
                strKEY3 = txtKeiyakuCode.Text.Trim
                LW.ToriCode = strKEY1 + strKEY2
            End If

            'そのほかの画面項目入力チェック
            If fn_check_other() = False Then
                Exit Sub
            End If

            '取引先マスタに取引先コードが存在することを確認
            If fn_select_TORIMAST(strKEY1, strKEY2) = False Then  '検索にヒットしなかったら
                txtTorisCode.Focus()
                Exit Sub
            End If

            'S_ENTMAST存在チェック
            If fn_select_ENTMAST() = False Then '検索にヒットしなかったら
                MessageBox.Show(MSG0223W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            End If

            '確認MSG
            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '更新データセット
            If fn_DATA_SET() = False Then
                txtTorisCode.Focus()
                Exit Sub
            End If

            'トランザクション開始
            MainDB.BeginTrans()

            If fn_update_ENTMAST() = True Then
                MainDB.Commit()
                MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MainDB.Rollback()
                txtTorisCode.Focus()
                Exit Sub
            End If

            '画面クリア
            If fn_clear_GAMEN() = False Then
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            Else
                cmbKana.Enabled = True
                cmbToriName.Enabled = True
                txtTorisCode.Enabled = True
                txtTorifCode.Enabled = True
                txtKeiyakuCode.Enabled = True
            End If
            txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region

#Region " 解約"
    Private Sub btnKaiyaku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKaiyaku.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(解約)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle

            '------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------
            If fn_check_text() = False Then
                '2012/03/23 saitou 標準修正 DEL ---------------------------------------->>>>
                'チェック関数でフォーカス移動を行っているため、ここでフォーカス移動しない
                'txtTorisCode.Focus()
                '2012/03/23 saitou 標準修正 DEL ----------------------------------------<<<<
                Exit Sub
            End If
            strKEY1 = txtTorisCode.Text.Trim
            strKEY2 = txtTorifCode.Text.Trim
            strKEY3 = txtKeiyakuCode.Text.Trim
            LW.ToriCode = strKEY1 + strKEY2

            '取引先マスタに取引先コードが存在することを確認
            If fn_select_TORIMAST(strKEY1, strKEY2) = False Then  '検索にヒットしなかったら
                txtTorisCode.Focus()
                Exit Sub
            End If

            'S_ENTMAST存在チェック
            If fn_select_ENTMAST() = False Then '検索にヒットしなかったら
                MessageBox.Show(MSG0223W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            End If

            '------------------------------------------------
            '確認メッセージ
            '------------------------------------------------
            If MessageBox.Show(String.Format(MSG0015I, "解約"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            'トランザクション開始
            MainDB.BeginTrans()
            '解約
            If fn_update_KAIYAKU() = False Then
                txtTorisCode.Focus()
                Exit Sub
            Else
                MessageBox.Show(String.Format(MSG0016I, "解約"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            '画面クリア
            If fn_clear_GAMEN() = False Then
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            Else
                cmbKana.Enabled = True
                cmbToriName.Enabled = True
                txtTorisCode.Enabled = True
                txtTorifCode.Enabled = True
                txtKeiyakuCode.Enabled = True
            End If
            txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(解約)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(解約)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region

#Region " 解約解除"
    Private Sub btnKaijyo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKaijyo.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(解約解除)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle

            '------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------
            If fn_check_text() = False Then
                '2012/03/23 saitou 標準修正 DEL ---------------------------------------->>>>
                'チェック関数でフォーカス移動を行っているため、ここでフォーカス移動しない
                'txtTorisCode.Focus()
                '2012/03/23 saitou 標準修正 DEL ----------------------------------------<<<<
                Exit Sub
            End If
            strKEY1 = txtTorisCode.Text.Trim
            strKEY2 = txtTorifCode.Text.Trim
            strKEY3 = txtKeiyakuCode.Text.Trim
            LW.ToriCode = strKEY1 + strKEY2

            '取引先マスタに取引先コードが存在することを確認
            If fn_select_TORIMAST(strKEY1, strKEY2) = False Then  '検索にヒットしなかったら
                txtTorisCode.Focus()
                Exit Sub
            End If

            'ENTMAST存在チェック
            If fn_select_ENTMAST() = False Then '検索にヒットしなかったら
                MessageBox.Show(MSG0223W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            End If

            '------------------------------------------------
            '確認メッセージ
            '------------------------------------------------
            If MessageBox.Show(String.Format(MSG0015I, "解約解除"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Exit Sub
            End If

            'トランザクション開始
            MainDB.BeginTrans()
            '解約解除処理
            If fn_update_KAIJYO() = False Then
                txtTorisCode.Focus()
                Exit Sub
            Else
                MessageBox.Show(String.Format(MSG0016I, "解約解除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            '画面クリア
            If fn_clear_GAMEN() = False Then
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            Else
                cmbKana.Enabled = True
                cmbToriName.Enabled = True
                txtTorisCode.Enabled = True
                txtTorifCode.Enabled = True
                txtKeiyakuCode.Enabled = True
            End If
            txtTorisCode.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(解約解除)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(解約解除)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region

#Region " 参照"
    Private Sub btnSansyo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSansyo.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(解約解除)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle

            '------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------
            If fn_check_text() = False Then
                'txtTorisCode.Focus()   '2010/02/18 不要
                Exit Sub
            End If
            strKEY1 = txtTorisCode.Text.Trim
            strKEY2 = txtTorifCode.Text.Trim
            strKEY3 = txtKeiyakuCode.Text.Trim
            LW.ToriCode = strKEY1 + strKEY2

            '取引先マスタに取引先コードが存在することを確認
            If fn_select_TORIMAST(strKEY1, strKEY2) = False Then  '検索にヒットしなかったら
                txtTorisCode.Focus()
                Exit Sub
            End If

            'S_ENTMAST存在チェック
            If fn_select_ENTMAST() = False Then '検索にヒットしなかったら
                MessageBox.Show(MSG0223W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            End If

            '画面設定
            If fn_GAMEN_SET() = False Then
                txtTorisCode.Focus()
                Exit Sub
            End If

            'コントロール設定
            btnAction.Enabled = False
            btnKousin.Enabled = True
            If lblKaiyaku.Text.Trim = "" Then
                '解約なし
                btnKaiyaku.Enabled = True
                btnKaijyo.Enabled = False
            Else
                '解約済
                btnKaiyaku.Enabled = False
                btnKaijyo.Enabled = True
            End If
            btnSansyo.Enabled = False
            cmbKana.Enabled = False
            cmbToriName.Enabled = False
            txtTorisCode.Enabled = False
            txtTorifCode.Enabled = False
            txtKeiyakuCode.Enabled = False
            btnKousin.Focus()
            btnTUUBAN.Enabled = False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region

#Region " 取消"
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")
            ''画面クリア
            If fn_clear_GAMEN() = False Then
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Exit Sub
            Else
                cmbKana.Enabled = True
                cmbKana.SelectedIndex = 0
                cmbToriName.Enabled = True
                cmbToriName.SelectedIndex = 0
                txtTorisCode.Enabled = True
                txtTorifCode.Enabled = True
                txtKeiyakuCode.Enabled = True
            End If
            txtTorisCode.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " 通番取得"
    Private Sub btnTUUBAN_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTUUBAN.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(通番取得)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle
            '------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------
            If fn_check_text(1) = False Then
                Return
            End If
            strKEY1 = txtTorisCode.Text
            strKEY2 = txtTorifCode.Text

            '通番取得
            If fn_count_TUUBAN() = False Then
                Exit Sub
            Else
                txtKeiyakuCode.Text = Format(lngTUUBAN, "000000000000")
            End If

            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            '通番取得後は需要家番号にフォーカスあてる
            Me.txtJyuyokaNo.Focus()
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

        Catch ex As Exception
            If MainDB IsNot Nothing Then
                MainDB.Rollback()
            End If
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(通番取得)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(通番取得)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region

#Region " 口座チェック"
    Private Sub btnKouzaCheck_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKouzaCheck.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座チェック)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle
            '------------------------------------
            'テキストボックスの入力チェック
            '------------------------------------
            If fn_check_text(1) = False Then
                Exit Sub
            Else
                strKEY1 = txtTorisCode.Text.Trim
                strKEY2 = txtTorifCode.Text.Trim
                strKEY3 = txtKeiyakuCode.Text.Trim
            End If

            'そのほかの画面項目入力チェック
            If fn_check_other(1) = False Then
                Exit Sub
            End If
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '取引先マスタに取引先コードが存在することを確認
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_select_TORIMAST(strKEY1, strKEY2) = False Then  '検索にヒットしなかったら
                txtTorisCode.Focus()
                Exit Sub
            End If

            strIN_KIN = txtKinCode.Text
            strIN_SIT = txtSitCode.Text
            strIN_KOUZA = txtKouza.Text
            strIN_KAMOKU = GCom.GetComboBox(cmbKamoku).ToString("00")
            strIN_KokyakuName = txtkeiyakuKName.Text.Trim
            Dim strErr As String = ""
            '----------------------------------------------------------------------
            '　KDBMASTより口座チェック
            '----------------------------------------------------------------------
            If strIN_KIN = Jikinko Then       '自金庫以外はチェックしない
                Dim Ret As Integer = GCom.KouzaChk_ENTRY(strIN_SIT, strIN_KAMOKU, strIN_KOUZA, _
                                     strKIGYO_CODE, strFURI_CODE, strIN_KokyakuName, strErr, MainDB)
                Select Case Ret
                    Case -1 '口座チェック失敗
                        MessageBox.Show(MSG0236W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Case 0  'チェック成功
                        lblKekka.Text = ""
                        If txtkeiyakuKName.Text.Trim = "" Then txtkeiyakuKName.Text = strIN_KokyakuName
                    Case Else
                        '自振契約なしは無視
                        '2011/06/28 標準版修正 総振は自振契約なしは正常扱い ------------------START
                        If Ret = 2 Then
                            'If strErr = "自振契約無し" Then
                            '2011/06/28 標準版修正 総振は自振契約なしは正常扱い ------------------END
                            lblKekka.Text = ""
                        Else
                            lblKekka.Text = strErr
                        End If
                        If txtkeiyakuKName.Text.Trim = "" Then txtkeiyakuKName.Text = strIN_KokyakuName
                End Select
            Else
                lblKekka.Text = ""
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座チェック)終了", "成功", "")
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " 関数"
    Private Function fn_Settei() As Boolean
        Try
            '--------------------------------
            ' 設定ファイル取得
            '--------------------------------
            '自金庫コード
            Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If Jikinko.ToUpper = "ERR" OrElse Jikinko = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                Return False
            End If

            '2018/02/27 タスク）西野 ADD 標準版修正：広島信金対応（依頼書振替口座入力制御追加（金融機関自行のみ））-- START
            'エントリ画面の自金庫区分
            Dim TMP As String = CASTCommon.GetRSKJIni("FORM", "KFSENTR010_JIKINKOKBN")
            If TMP.ToUpper = "ERR" OrElse TMP.Trim = "" Then
                '何もしない
            Else
                If TMP = "1" Then
                    '金融機関コードを入力不可にする
                    txtKinCode.Enabled = False
                End If
            End If
            '2018/02/27 タスク）西野 ADD 標準版修正：広島信金対応（依頼書振替口座入力制御追加（金融機関自行のみ））-- END

            '--------------------------------
            'コンボボックスの設定
            '--------------------------------
            Dim Msg As String
            Select Case GCom.SetComboBox(cmbKamoku, "Common_総振_科目.TXT", True)
                Case 1  'ファイルなし
                    Msg = String.Format(MSG0025E, "科目", "Common_総振_科目.TXT")
                    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "科目設定ファイルなし。ファイル:Common_総振_科目.TXT"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
                Case 2  '異常
                    Msg = String.Format(MSG0026E, "科目")
                    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "コンボボックス設定失敗 コンボボックス名:科目"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
            End Select
            Select Case GCom.SetComboBox(cmbKingakuKbn, "KFSENTR010_金額表示区分.TXT", True)
                Case 1  'ファイルなし
                    Msg = String.Format(MSG0025E, "金額表示区分", "KFSENTR010_金額表示区分.TXT")
                    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "金額表示区分設定ファイルなし。ファイル:KFSENTR010_金額表示区分.TXT"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
                Case 2  '異常
                    Msg = String.Format(MSG0026E, "金額表示区分")
                    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "コンボボックス設定失敗 コンボボックス名:金額表示区分"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
            End Select
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(初期設定)", "失敗", ex.ToString)
        End Try
    End Function

    Function fn_clear_GAMEN() As Boolean
        '============================================================================
        'NAME           :fn_clear_GAMEN
        'Parameter      :
        'Description    :画面クリア
        'Return         :True=OK,False=NG
        'Create         :2009/09/25
        'Update         :
        '============================================================================
        Try
            fn_clear_GAMEN = False
            btnAction.Enabled = True
            btnKousin.Enabled = False
            btnKaiyaku.Enabled = False
            btnKaijyo.Enabled = False
            btnSansyo.Enabled = True
            btnTUUBAN.Enabled = True
            '2010/02/18 口座チェックボタン使用可能とする
            'btnKouzaCheck.Enabled = False
            btnKouzaCheck.Enabled = True
            '=======================

            '画面を初期状態に戻す
            txtTorisCode.Focus()
            txtTorisCode.Enabled = True
            txtTorifCode.Enabled = True
            txtTorisCode.Text = ""
            txtTorifCode.Text = ""
            '2010/02/18 項目追加
            lblToriName.Text = ""
            lblFuriDate.Text = ""
            '=======================
            txtKeiyakuCode.Enabled = True
            txtKeiyakuCode.Text = ""
            lblKokyakuCode.Text = ""
            txtJyuyokaNo.Text = ""
            txtkeiyakuKName.Text = ""
            txtKeiyakuNName.Text = ""
            txtKeiyakuNAdress.Text = ""
            txtKeiyakuTelNo.Text = ""
            '2010/02/18 初期値設定
            'txtKinCode.Text = ""
            'lblKinNName.Text = ""
            txtKinCode.Text = Jikinko
            lblKinNName.Text = GCom.GetBKBRName(txtKinCode.Text, "", 30)
            '=======================
            txtSitCode.Text = ""
            lblSitNName.Text = ""
            txtKouza.Text = ""
            lblKaiyaku.Text = ""
            cmbKamoku.SelectedIndex = 1
            cmbKingakuKbn.SelectedIndex = 0
            '2010/02/18 初期値設定
            'txtKaisiDateY.Text = ""
            'txtKaisiDateM.Text = ""
            'txtKaisiDateD.Text = ""
            txtKaisiDateY.Text = Now.ToString("yyyy")
            txtKaisiDateM.Text = Now.ToString("MM")
            txtKaisiDateD.Text = Now.ToString("dd")
            '=======================
            lblKekka.Text = ""

            '--------------------------------
            '委託者名リストボックスの設定
            '--------------------------------
            '取引先コンボボックス設定
            Dim Jyoken As String = " AND BAITAI_CODE_T = '04'"   '媒体コードが依頼書
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "3", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            txtTorisCode.Focus()
            fn_clear_GAMEN = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(画面初期表示)", "失敗", ex.ToString)
        End Try
    End Function

    Function fn_check_text(Optional ByVal ChkKbn As Integer = 0) As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :ChkKbn 0:通常 1:契約者コードチェックなし
        'Description    ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/25
        'Update         :
        '============================================================================
        fn_check_text = False
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

            If ChkKbn = 1 Then
                Return True
            End If

            '契約者コード必須チェック
            If txtKeiyakuCode.Text.Trim = "" Then
                MessageBox.Show(MSG0216W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKeiyakuCode.Focus()
                Return False
            End If

            fn_check_text = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_check_other(Optional ByVal chkKbn As Integer = 0) As Boolean
        '============================================================================
        'NAME           :fn_check_other
        'Parameter      :chkKbn チェック区分 0:登録/更新 1:口座チェック
        'Description    ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/09
        'Update         :
        '============================================================================
        fn_check_other = False
        Try

            '金融機関コード必須チェック
            If txtKinCode.Text.Trim = "" Then
                MessageBox.Show(MSG0032W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKinCode.Focus()
                Return False
            End If

            '支店コード必須チェック
            If txtSitCode.Text.Trim = "" Then
                MessageBox.Show(MSG0035W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSitCode.Focus()
                Return False
            End If

            '金融機関存在チェック
            If fn_select_TENMAST(txtKinCode.Text, txtSitCode.Text) = False Then
                txtKinCode.Focus()
                Return False
            End If

            '口座番号必須チェック
            If txtKouza.Text.Trim = "" Then
                MessageBox.Show(MSG0123W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKouza.Focus()
                Return False
            End If

            '口座チェックの場合は以下の入力値チェックは不要
            If chkKbn = 1 Then
                Return True
            End If

            '契約者名（カナ）必須チェック
            If txtkeiyakuKName.Text.Trim = "" Then
                MessageBox.Show(MSG0218W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtkeiyakuKName.Focus()
                Return False
            End If

            '2010/02/23 必須から除く
            ''契約者名（漢字）必須チェック
            'If txtKeiyakuNName.Text.Trim = "" Then
            '    MessageBox.Show(MSG0219W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    txtKeiyakuNName.Focus()
            '    Return False
            'End If
            '==========================

            '開始年(年)必須チェック
            If txtKaisiDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0168W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKaisiDateY.Focus()
                Return False
            End If
            '開始年(月)必須チェック
            If txtKaisiDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0170W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKaisiDateM.Focus()
                Return False
            End If
            '開始年(月)範囲チェック
            If GCom.NzInt(txtKaisiDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtKaisiDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKaisiDateM.Focus()
                Return False
            End If
            '開始年(日)必須チェック
            If txtKaisiDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0172W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKaisiDateD.Focus()
                Return False
            End If
            '開始年(日)範囲チェック
            If GCom.NzInt(txtKaisiDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtKaisiDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKaisiDateD.Focus()
                Return False
            End If

            '開始年日付整合性チェック
            Dim WORK_DATE As String = txtKaisiDateY.Text & "/" & txtKaisiDateM.Text & "/" & txtKaisiDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKaisiDateY.Focus()
                Return False
            End If
            fn_check_other = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try

    End Function

    Function fn_select_TORIMAST(ByVal TORIS_CODE As String, ByVal TORIF_CODE As String) As Boolean
        '=====================================================================================
        'NAME           :fn_Select_TORIMAST
        'Parameter      :TORIS_CODE：取引先主コード／TORIF_CODE：取引先副コード
        'Description    :取引先マスタ検索
        'Return         :True=OK(検索ヒット),False=NG（検索失敗）
        'Create         :2009/09/25
        'Update         :
        '=====================================================================================
        fn_select_TORIMAST = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            '取引先情報取得
            SQL.Append("SELECT * FROM S_TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(Trim(TORIS_CODE)))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(TORIF_CODE)))
            If OraReader.DataReader(SQL) = True Then
                strFSYORI_KBN = GCom.NzStr(OraReader.Reader.Item("FSYORI_KBN_T"))
                strFURI_CODE = GCom.NzStr(OraReader.Reader.Item("FURI_CODE_T"))
                strKIGYO_CODE = GCom.NzStr(OraReader.Reader.Item("KIGYO_CODE_T"))
                strBAITAI_CODE = GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T"))
                OraReader.Close()
            Else
                '取引先なし
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                OraReader.Close()
                Return False
            End If
            '媒体コードチェック
            If strBAITAI_CODE <> "04" Then '依頼書
                MessageBox.Show(MSG0108W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

        fn_select_TORIMAST = True
    End Function

    Function fn_select_ENTMAST() As Boolean
        '============================================================================
        'NAME           :fn_select_ENTMAST
        'Parameter      :
        'Description    :S_ENTMASTに該当データが存在するかどうか
        'Return         :True=OK,False=NG
        'Create         :2009/09/25
        'Update         :
        '============================================================================
        fn_select_ENTMAST = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            SQL.Append(" SELECT * ")
            SQL.Append(" FROM S_ENTMAST")
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strKEY1))
            SQL.Append(" AND   TORIF_CODE_E = " & SQ(strKEY2))
            SQL.Append(" AND   KEIYAKU_NO_E = " & SQ(strKEY3))

            If OraReader.DataReader(SQL) = False Then
                Return False '依頼書なし
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(依頼書マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_select_ENTMAST = True
    End Function

    Function fn_GAMEN_SET() As Boolean
        '============================================================================
        'NAME           :fn_GAMEN_SET
        'Parameter      :
        'Description    :SELECTで取得したデータを画面項目にセットする
        'Return         :True=OK,False=NG
        'Create         :2009/09/25
        'Update         :
        '============================================================================
        fn_GAMEN_SET = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            SQL.Append(" SELECT * ")
            SQL.Append(" FROM S_ENTMAST")
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strKEY1))
            SQL.Append(" AND   TORIF_CODE_E = " & SQ(strKEY2))
            SQL.Append(" AND   KEIYAKU_NO_E = " & SQ(strKEY3))

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
            lblKokyakuCode.Text = GCom.NzStr(OraReader.GetString("KOKYAKU_NO_E"))
            txtJyuyokaNo.Text = GCom.NzStr(OraReader.GetString("JYUYOUKA_NO_E"))
            txtkeiyakuKName.Text = GCom.NzStr(OraReader.GetString("KEIYAKU_KNAME_E")).Trim
            txtKeiyakuNName.Text = GCom.NzStr(OraReader.GetString("KEIYAKU_NNAME_E")).Trim
            txtKeiyakuNAdress.Text = GCom.NzStr(OraReader.GetString("KEIYAKU_NJYU_E")).Trim
            txtKeiyakuTelNo.Text = GCom.NzStr(OraReader.GetString("KEIYAKU_DENWA_E")).Trim
            txtKinCode.Text = GCom.NzStr(OraReader.GetString("TKIN_NO_E"))
            lblKinNName.Text = GCom.GetBKBRName(txtKinCode.Text, "", 30)
            txtSitCode.Text = GCom.NzStr(OraReader.GetString("TSIT_NO_E"))
            lblSitNName.Text = GCom.GetBKBRName(txtKinCode.Text, txtSitCode.Text, 30)

            Dim IntTemp As Integer = GCom.NzInt(OraReader.GetString("KAMOKU_E"))
            For Cnt As Integer = 0 To cmbKamoku.Items.Count - 1 Step 1
                cmbKamoku.SelectedIndex = Cnt
                If GCom.GetComboBox(cmbKamoku) = IntTemp Then
                    Exit For
                End If
            Next Cnt

            txtKouza.Text = GCom.NzStr(OraReader.GetString("KOUZA_E"))
            If GCom.NzInt(OraReader.GetString("KAIYAKU_E")) = 1 Then
                lblKaiyaku.Text = "解約済み"
            End If
            IntTemp = GCom.NzInt(OraReader.GetString("KINGAKU_KBN_E"))
            For Cnt As Integer = 0 To cmbKingakuKbn.Items.Count - 1 Step 1
                cmbKingakuKbn.SelectedIndex = Cnt
                If GCom.GetComboBox(cmbKingakuKbn) = IntTemp Then
                    Exit For
                End If
            Next Cnt

            txtKaisiDateY.Text = Mid(GCom.NzStr(OraReader.GetString("KAISI_DATE_E")), 1, 4)
            txtKaisiDateM.Text = Mid(GCom.NzStr(OraReader.GetString("KAISI_DATE_E")), 5, 2)
            txtKaisiDateD.Text = Mid(GCom.NzStr(OraReader.GetString("KAISI_DATE_E")), 7, 2)

            '新規コード判定用項目退避
            strSAVE_KIN_NO = GCom.NzStr(OraReader.GetString("TKIN_NO_E"))
            strSAVE_SIT_NO = GCom.NzStr(OraReader.GetString("TKIN_NO_E"))
            strSAVE_KAMOKU = GCom.NzStr(OraReader.GetString("KAMOKU_E"))
            strSAVE_KOUZA = GCom.NzStr(OraReader.GetString("KOUZA_E"))
            strSAVE_FURI_DATE = GCom.NzStr(OraReader.GetString("FURI_DATE_E"))
            strSAVE_SINKI_CODE = GCom.NzStr(OraReader.GetString("SINKI_CODE_E"))
            If txtKinCode.Text = Jikinko Then
                btnKouzaCheck.Enabled = True
            End If
            fn_GAMEN_SET = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(画面設定)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function

    Function fn_select_TENMAST(ByVal KinCode As String, ByVal SitCode As String) As Boolean
        '============================================================================
        'NAME           :fn_select_TENMAST
        'Parameter      :KinCode 金融機関コード, SitCode 支店コード
        'Description    :TENMASTに該当データが存在するかどうか
        'Return         :True=OK,False=NG
        'Create         :2009/09/25
        'Update         :
        '============================================================================
        fn_select_TENMAST = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            '2017/01/18 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
            SQL.Append(" SELECT COUNT(*) COUNTER,MAX(TEIKEI_KBN_N) TEIKEI_KBN")
            SQL.Append(" FROM TENMAST")
            SQL.Append(" WHERE TENMAST.KIN_NO_N = " & SQ(KinCode))
            SQL.Append(" AND TENMAST.SIT_NO_N = " & SQ(SitCode))
            ''金融機関情報マスタが作成されるまでコメント化 =======================
            'SQL.Append(" SELECT COUNT(*) COUNTER,MAX(TEIKEI_KBN_N) TEIKEI_KBN")
            'SQL.Append(" FROM TENMAST,KIN_INFOMAST")
            'SQL.Append(" WHERE TENMAST.KIN_NO_N = " & SQ(KinCode))
            'SQL.Append(" AND TENMAST.SIT_NO_N = " & SQ(SitCode))
            ''金融機関情報マスタと連結
            'SQL.Append(" AND TENMAST.KIN_NO_N = KIN_INFOMAST.KIN_NO_N")
            '2017/01/18 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
            Dim Count As Integer
            If OraReader.DataReader(SQL) = True Then
                Count = GCom.NzInt(OraReader.Reader.Item("COUNTER"))
                strTEIKEI_KBN = GCom.NzStr(OraReader.Reader.Item("TEIKEI_KBN"))
                OraReader.Close()
            Else
                '検索失敗
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                Return False
            End If

            If Count = 0 Then
                '金融機関なし
                MessageBox.Show(MSG0119W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
            '====================================================================
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(金融機関マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_select_TENMAST = True
    End Function

    Function fn_select_HYOJIJUN() As Boolean
        '=====================================================================================
        'NAME           :fn_select_HYOJIJUN
        'Parameter      :
        'Description    :S_ENTMAST表示順番検索
        'Return         :True=OK(検索ヒット),False=NG（検索失敗）
        'Create         :2009/09/25
        'Update         :
        '=====================================================================================
        fn_select_HYOJIJUN = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            SQL.Append("SELECT NVL(MAX(HYOUJI_SEQ_E),0) AS HYOUJI_MAX")
            SQL.Append(",NVL(MAX(KOKYAKU_NO_E),0) AS KOKYAKU_MAX")
            SQL.Append(",NVL(MAX(RECORD_NO_E),0) AS RECORD_MAX")
            SQL.Append(" FROM S_ENTMAST")
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strKEY1))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strKEY2))

            If OraReader.DataReader(SQL) = True Then
                lngHYOUJI_NUM = GCom.NzLong(OraReader.Reader.Item("HYOUJI_MAX"))
                lngKOKYAKU_NUM = GCom.NzLong(OraReader.Reader.Item("KOKYAKU_MAX"))
                lngRECORD_NUM = GCom.NzLong(OraReader.Reader.Item("RECORD_MAX"))
                OraReader.Close()
            Else
                'マスタ検索失敗(MSG0002E)
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(表示順番検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

        fn_select_HYOJIJUN = True
    End Function

    Function fn_ENTDATA_SET() As Boolean
        '============================================================================
        'NAME           :fn_ENTDATA_SET
        'Parameter      :
        'Description    :ENTRY_DATA配列にデータをセットする
        'Return         :True=OK,False=NG
        'Create         :2009/09/25
        'Update         :
        '============================================================================
        fn_ENTDATA_SET = False
        Try
            ENTRY_DATA(0) = strFSYORI_KBN 'FSYORI_KBN_E
            ENTRY_DATA(1) = strKEY1 'TORIS_CODE_E 
            ENTRY_DATA(2) = strKEY2 'TORIF_CODE_E
            ENTRY_DATA(3) = "00000000" 'FURI_DATE_E
            ENTRY_DATA(4) = strFURI_CODE 'FURI_CODE_E
            ENTRY_DATA(5) = strTEIKEI_KBN 'TEIKEI_KBN_E
            ENTRY_DATA(6) = lngHYOUJI_NUM.ToString 'HYOUJI_SEQ_E
            ENTRY_DATA(7) = GCom.GetComboBox(cmbKamoku).ToString.PadLeft(2, "0"c) 'KAMOKU_E
            ENTRY_DATA(8) = txtKouza.Text 'KOUZA_E
            ENTRY_DATA(9) = txtKeiyakuCode.Text 'KEIYAKU_NO_E
            ENTRY_DATA(10) = txtkeiyakuKName.Text.Trim 'KEIYAKU_KNAME_E
            ENTRY_DATA(11) = txtKeiyakuNName.Text.Trim 'KEIYAKU_NNAME_E
            ENTRY_DATA(12) = CStr(lngKOKYAKU_NUM).PadLeft(10, "0"c) 'KOKYAKU_NO_E
            ENTRY_DATA(13) = CStr(GCom.GetComboBox(cmbKingakuKbn))  'KINGAKU_KBN_E
            ENTRY_DATA(14) = "0" 'FURIKIN_E
            ENTRY_DATA(15) = "0" 'TESUU_E
            ENTRY_DATA(16) = "0" 'TESUU2_E
            ENTRY_DATA(17) = "0" 'TESUU_KBN_E
            ENTRY_DATA(18) = txtKinCode.Text 'TKIN_NO_E
            ENTRY_DATA(19) = txtSitCode.Text 'TSIT_NO_E
            '2010/02/18 登録の場合必ず新規にする
            ''開始日 ＞=  作成日 の場合　新規とする
            'If txtKaisiDateY.Text & txtKaisiDateM.Text & txtKaisiDateD.Text >= Format(Now, "yyyyMMdd") Then
            '    ENTRY_DATA(20) = "1" 'SINKI_CODE_E 
            'Else
            '    ENTRY_DATA(20) = "0" 'SINKI_CODE_E 
            'End If
            ENTRY_DATA(20) = "1" 'SINKI_CODE_E 
            '======================================
            ENTRY_DATA(21) = lngRECORD_NUM.ToString 'RECORD_NO_E  
            ENTRY_DATA(22) = "0" 'KAIYAKU_E     
            ENTRY_DATA(23) = txtKaisiDateY.Text & txtKaisiDateM.Text & txtKaisiDateD.Text  'KAISI_DATE_E    
            ENTRY_DATA(24) = Format(Now, "yyyyMMdd") 'SAKUSEI_DATE_E    
            ENTRY_DATA(25) = "00000000" 'KOUSIN_DATE_E   
            ENTRY_DATA(26) = txtKeiyakuNAdress.Text.Trim 'KEIYAKU_NJYU_E   
            ENTRY_DATA(27) = txtKeiyakuTelNo.Text.Trim 'KEIYAKU_DENWA_E    
            ENTRY_DATA(28) = txtJyuyokaNo.Text.PadLeft(13, " "c) 'JYUYOUKA_NO_E 
            ENTRY_DATA(29) = Space(50) 'ERR_MSG_E  
            ENTRY_DATA(30) = Space(30) 'YOBI1_E           
            ENTRY_DATA(31) = Space(30) 'YOBI2_E            
            ENTRY_DATA(32) = Space(30) 'YOBI3_E          
            ENTRY_DATA(33) = Space(30) 'YOBI4_E         
            ENTRY_DATA(34) = Space(30) 'YOBI5_E           
            ENTRY_DATA(35) = Space(30) 'YOBI6_E           
            ENTRY_DATA(36) = Space(30) 'YOBI7_E        
            ENTRY_DATA(37) = Space(30) 'YOBI8_E   
            ENTRY_DATA(38) = Space(30) 'YOBI9_E   
            ENTRY_DATA(39) = Space(30) 'YOBI10_E 
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録データ作成)", "失敗", ex.ToString)
            Return False
        End Try
        fn_ENTDATA_SET = True
    End Function

    Function fn_insert_ENTMAST() As Boolean
        '============================================================================
        'NAME           :fn_insert_ENTMAST
        'Parameter      :
        'Description    :S_ENTMAST登録
        'Return         :True=OK,False=NG
        'Create         :2009/09/25
        'Update         :
        '============================================================================
        fn_insert_ENTMAST = False
        Dim SQL As StringBuilder
        Dim Ret As Integer
        Try
            For No As Integer = 1 To 2
                SQL = New StringBuilder(128)
                If No = 1 Then
                    SQL.Append(" INSERT INTO S_ENTMAST VALUES (")
                Else
                    SQL.Append(" INSERT INTO S_FUKU_ENTMAST VALUES (")
                End If
                SQL.Append(SQ(ENTRY_DATA(0)))           'FSYORI_KBN_E
                SQL.Append("," & SQ(ENTRY_DATA(1)))     'TORIS_CODE_E
                SQL.Append("," & SQ(ENTRY_DATA(2)))     'TORIF_CODE_E
                SQL.Append("," & SQ(ENTRY_DATA(3)))     'FURI_DATE_E
                SQL.Append("," & SQ(ENTRY_DATA(4)))     'FURI_CODE_E
                SQL.Append("," & SQ(ENTRY_DATA(5)))     'TEIKEI_KBN_E
                SQL.Append("," & ENTRY_DATA(6))         'HYOUJI_SEQ_E
                SQL.Append("," & SQ(ENTRY_DATA(7)))     'KAMOKU_E
                SQL.Append("," & SQ(ENTRY_DATA(8)))     'KOUZA_E
                SQL.Append("," & SQ(ENTRY_DATA(9)))     'KEIYAKU_NO_E
                SQL.Append("," & SQ(ENTRY_DATA(10)))    'KEIYAKU_KNAME_E
                SQL.Append("," & SQ(ENTRY_DATA(11)))    'KEIYAKU_NNAME_E
                SQL.Append("," & SQ(ENTRY_DATA(12)))    'KOKYAKU_NO_E
                SQL.Append("," & SQ(ENTRY_DATA(13)))    'KINGAKU_KBN_E
                SQL.Append("," & ENTRY_DATA(14))        'FURIKIN_E
                SQL.Append("," & ENTRY_DATA(15))        'TESUU_E
                SQL.Append("," & ENTRY_DATA(16))        'TESUU2_E
                SQL.Append("," & SQ(ENTRY_DATA(17)))    'TESUU_KBN_E
                SQL.Append("," & SQ(ENTRY_DATA(18)))    'TKIN_NO_E
                SQL.Append("," & SQ(ENTRY_DATA(19)))    'TSIT_NO_E
                SQL.Append("," & SQ(ENTRY_DATA(20)))    'SINKI_CODE_E
                SQL.Append("," & SQ(ENTRY_DATA(21)))    'RECORD_NO_E
                SQL.Append("," & SQ(ENTRY_DATA(22)))    'KAIYAKU_E
                SQL.Append("," & SQ(ENTRY_DATA(23)))    'KAISI_DATE_E
                SQL.Append("," & SQ(ENTRY_DATA(24)))    'SAKUSEI_DATE_E
                SQL.Append("," & SQ(ENTRY_DATA(25)))    'KOUSIN_DATE_E
                SQL.Append("," & SQ(ENTRY_DATA(26)))    'KEIYAKU_NJYU_E
                SQL.Append("," & SQ(ENTRY_DATA(27)))    'KEIYAKU_DENWA_E
                SQL.Append("," & SQ(ENTRY_DATA(28)))    'KEIYAKU_DENWA_E
                SQL.Append("," & SQ(ENTRY_DATA(29)))    'ERR_MSG_E
                SQL.Append("," & SQ(ENTRY_DATA(30)))    'YOBI1_E
                SQL.Append("," & SQ(ENTRY_DATA(31)))    'YOBI2_E
                SQL.Append("," & SQ(ENTRY_DATA(32)))    'YOBI3_E
                SQL.Append("," & SQ(ENTRY_DATA(33)))    'YOBI4_E
                SQL.Append("," & SQ(ENTRY_DATA(34)))    'YOBI5_E
                SQL.Append("," & SQ(ENTRY_DATA(35)))    'YOBI6_E
                SQL.Append("," & SQ(ENTRY_DATA(36)))    'YOBI7_E
                SQL.Append("," & SQ(ENTRY_DATA(37)))    'YOBI8_E
                SQL.Append("," & SQ(ENTRY_DATA(38)))    'YOBI9_E
                SQL.Append("," & SQ(ENTRY_DATA(39)))    'YOBI10_E
                SQL.Append(")")
                Ret = MainDB.ExecuteNonQuery(SQL)
                If Ret <> 1 Then    '登録失敗(MSG0002E)
                    MessageBox.Show(String.Format(MSG0002E, "登録"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If
            Next

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(依頼書マスタ登録)", "失敗", ex.ToString)
            Return False
        End Try
        fn_insert_ENTMAST = True
    End Function

    Function fn_DATA_SET() As Boolean
        '============================================================================
        'NAME           :fn_DATA_SET
        'Parameter      :
        'Description    :UP_DATA配列にデータをセットする
        'Return         :True=OK,False=NG
        'Create         :2009/09/25
        'Update         :
        '============================================================================
        fn_DATA_SET = False
        Try
            '更新項目の設定
            UP_DATA(1) = txtJyuyokaNo.Text  'JYUYOUKA_NO_E
            UP_DATA(2) = txtkeiyakuKName.Text 'KEIYAKU_KNAME_E
            UP_DATA(3) = txtKeiyakuNName.Text 'KEIYAKU_NNAME_E
            UP_DATA(4) = txtKeiyakuNAdress.Text 'KEIYAKU_NJYU_E
            UP_DATA(5) = txtKeiyakuTelNo.Text 'KEIYAKU_DENWA_E
            UP_DATA(6) = txtKinCode.Text    'TKIN_NO_E
            UP_DATA(7) = txtSitCode.Text    'TSIT_NO_E
            UP_DATA(8) = GCom.GetComboBox(cmbKamoku).ToString.PadLeft(2, "0"c) 'KAMOKU_E
            UP_DATA(9) = txtKouza.Text      'KOUZA_E
            UP_DATA(10) = CStr(GCom.GetComboBox(cmbKingakuKbn))  'KINGAKU_KBN_E
            UP_DATA(11) = txtKaisiDateY.Text & txtKaisiDateM.Text & txtKaisiDateD.Text 'KAISI_DATE_E
            UP_DATA(12) = Format(Now, "yyyyMMdd") 'KOUSIN_DATE_E
            '2010/02/18 新規コードは落し込み時に更新する
            ''SINKI_CODE_E
            'If strSAVE_KIN_NO = UP_DATA(6) Or _
            '   strSAVE_SIT_NO = UP_DATA(7) Or _
            '   strSAVE_KAMOKU = UP_DATA(8) Or _
            '   strSAVE_KOUZA = UP_DATA(9) Then
            '    UP_DATA(13) = strSAVE_SINKI_CODE
            'Else
            '    If UP_DATA(11) > UP_DATA(12) Then        '開始日　>  更新日　開始日が未来の場合は新規とする
            '        UP_DATA(13) = "1"
            '    Else
            '        If UP_DATA(12) > strSAVE_FURI_DATE Then       '更新日　>　振込日  過去に振込処理を行っている場合は変更とする
            '            UP_DATA(13) = "2"
            '        Else
            '            UP_DATA(13) = strSAVE_SINKI_CODE
            '        End If
            '    End If
            'End If
            UP_DATA(13) = strSAVE_SINKI_CODE
            '========================================================
            fn_DATA_SET = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新データ作成)", "失敗", ex.ToString)
            Return False
        End Try

    End Function

    Function fn_update_ENTMAST() As Boolean
        '============================================================================
        'NAME           :fn_update_ENTMAST
        'Parameter      :
        'Description    :S_ENTMAST更新
        'Return         :True=OK,False=NG
        'Create         :2009/09/25
        'Update         :
        '============================================================================
        fn_update_ENTMAST = False
        Dim SQL As StringBuilder
        Dim Ret As Integer
        Try
            For No As Integer = 1 To 2
                SQL = New StringBuilder(128)
                If No = 1 Then
                    SQL.Append(" UPDATE S_ENTMAST SET")
                Else
                    SQL.Append(" UPDATE S_FUKU_ENTMAST SET")
                End If
                SQL.Append(" JYUYOUKA_NO_E = " & SQ(UP_DATA(1)))    'JYUYOUKA_NO_E
                SQL.Append(",KEIYAKU_KNAME_E = " & SQ(UP_DATA(2)))  'KEIYAKU_KNAME_E
                SQL.Append(",KEIYAKU_NNAME_E = " & SQ(UP_DATA(3)))  'KEIYAKU_NNAME_E
                SQL.Append(",KEIYAKU_NJYU_E = " & SQ(UP_DATA(4)))   'KEIYAKU_NJYU_E
                SQL.Append(",KEIYAKU_DENWA_E = " & SQ(UP_DATA(5)))  'KEIYAKU_DENWA_E
                SQL.Append(",TKIN_NO_E = " & SQ(UP_DATA(6)))        'TKIN_NO_E
                SQL.Append(",TSIT_NO_E = " & SQ(UP_DATA(7)))        'TSIT_NO_E
                SQL.Append(",KAMOKU_E = " & SQ(UP_DATA(8)))         'KAMOKU_E
                SQL.Append(",KOUZA_E = " & SQ(UP_DATA(9)))          'KOUZA_E
                SQL.Append(",KINGAKU_KBN_E = " & SQ(UP_DATA(10)))   'KINGAKU_KBN_E
                SQL.Append(",KAISI_DATE_E = " & SQ(UP_DATA(11)))    'KAISI_DATE_E
                SQL.Append(",KOUSIN_DATE_E = " & SQ(UP_DATA(12)))   'KOUSIN_DATE_E
                SQL.Append(",SINKI_CODE_E = " & SQ(UP_DATA(13)))    'SINKI_CODE_E
                SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strKEY1))
                SQL.Append(" AND TORIF_CODE_E = " & SQ(strKEY2))
                SQL.Append(" AND KEIYAKU_NO_E = " & SQ(strKEY3))

                Ret = MainDB.ExecuteNonQuery(SQL)
                If Ret < 1 Then    '更新失敗(MSG0002E)
                    MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If
            Next

            fn_update_ENTMAST = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(依頼書マスタ更新)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Public Function fn_count_TUUBAN() As Boolean
        '指定企業内で使用している最大の契約者コードを取得し、＋１したものを画面表示する
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            SQL.Append("SELECT NVL(MAX(KEIYAKU_NO_E),0) AS KEIYAKU_MAX")
            SQL.Append(" FROM S_ENTMAST")
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strKEY1))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strKEY2))

            If OraReader.DataReader(SQL) = True Then
                lngTUUBAN = GCom.NzLong(OraReader.Reader.Item("KEIYAKU_MAX")) + 1
                If lngTUUBAN.ToString.Length = 13 Then
                    lngTUUBAN = 999999999999
                End If
                OraReader.Close()
            Else
                'マスタ検索失敗
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                Return False
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(表示順番検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function

    Function fn_update_KAIYAKU() As Boolean
        '============================================================================
        'NAME           :fn_update_KAIYAKU
        'Parameter      :
        'Description    :解約時
        'Return         :True=OK,False=NG
        'Create         :2009/09/26
        'Update         :
        '============================================================================
        fn_update_KAIYAKU = False
        Dim SQL As StringBuilder
        Dim Ret As Integer
        Try
            For No As Integer = 1 To 2
                SQL = New StringBuilder(128)
                If No = 1 Then
                    SQL.Append(" UPDATE S_ENTMAST SET")
                Else
                    SQL.Append(" UPDATE S_FUKU_ENTMAST SET")
                End If
                SQL.Append(" KAIYAKU_E = '1' ")
                '2018/02/27 タスク）西野 ADD 標準版修正：広島信金対応（振替口座解約処理変更（更新日設定））-------------- START
                SQL.Append(",KOUSIN_DATE_E = " & SQ(Now.ToString("yyyyMMdd")))
                '2018/02/27 タスク）西野 ADD 標準版修正：広島信金対応（振替口座解約処理変更（更新日設定））-------------- END
                SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strKEY1))
                SQL.Append(" AND TORIF_CODE_E = " & SQ(strKEY2))
                SQL.Append(" AND KEIYAKU_NO_E = " & SQ(strKEY3))

                Ret = MainDB.ExecuteNonQuery(SQL)
                If Ret < 1 Then    '更新失敗(MSG0002E)
                    MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If
            Next
            fn_update_KAIYAKU = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(依頼書解約)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_update_KAIJYO() As Boolean
        '============================================================================
        'NAME           :fn_update_KAIJYO
        'Parameter      :
        'Description    :解約解除時
        'Return         :True=OK,False=NG
        'Create         :2004/08/23
        'Update         :
        '============================================================================
        fn_update_KAIJYO = False
        Dim SQL As StringBuilder
        Dim Ret As Integer
        Try
            For No As Integer = 1 To 2
                SQL = New StringBuilder(128)
                If No = 1 Then
                    SQL.Append(" UPDATE S_ENTMAST SET")
                Else
                    SQL.Append(" UPDATE S_FUKU_ENTMAST SET")
                End If
                SQL.Append(" KAIYAKU_E = '0' ")
                '2018/02/27 タスク）西野 ADD 標準版修正：広島信金対応（振替口座解約処理変更（更新日設定））-------------- START
                SQL.Append(",KOUSIN_DATE_E = " & SQ(Now.ToString("yyyyMMdd")))
                '2018/02/27 タスク）西野 ADD 標準版修正：広島信金対応（振替口座解約処理変更（更新日設定））-------------- END
                SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strKEY1))
                SQL.Append(" AND TORIF_CODE_E = " & SQ(strKEY2))
                SQL.Append(" AND KEIYAKU_NO_E = " & SQ(strKEY3))

                Ret = MainDB.ExecuteNonQuery(SQL)
                If Ret < 1 Then    '更新失敗(MSG0002E)
                    MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End If
            Next
            fn_update_KAIJYO = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(依頼書解約解除)", "失敗", ex.ToString)
            Return False
        End Try
    End Function
#End Region

#Region " イベント"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '選択カナで始まる委託者名を取得
        '--------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定
                Dim Jyoken As String = " AND BAITAI_CODE_T = '04'"   '媒体が依頼書
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "3", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                cmbToriName.Focus()
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
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                '2010/02/18 イベント追加
                FMT_NzNumberString_Validated(sender, e)
                '=======================
                txtKeiyakuCode.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コード取得)", "失敗", ex.ToString)
        End Try

    End Sub

    'ゼロパディング
    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
    '契約者コードは別途イベント追加
    'Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    '             Handles txtTorisCode.Validating, txtTorifCode.Validating, txtKeiyakuCode.Validating, _
    '                    txtKouza.Validating, txtKaisiDateY.Validating, txtKaisiDateM.Validating, txtKaisiDateD.Validating
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
                 Handles txtTorisCode.Validating, txtTorifCode.Validating, _
                        txtKouza.Validating, txtKaisiDateY.Validating, txtKaisiDateM.Validating, txtKaisiDateD.Validating
        ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub

    '2010/02/18 イベント追加
    '取引先情報設定(取引先名・振込日)
    Private Sub FMT_NzNumberString_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) _
             Handles txtTorisCode.Validated, txtTorifCode.Validated

        lblToriName.Text = ""
        lblFuriDate.Text = ""
        If (txtTorisCode.Text.Trim & txtTorifCode.Text.Trim).Length <> 12 Then
            Return
        End If
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder()
        Try
            MainDB = New MyOracle
            OraReader = New MyOracleReader(MainDB)

            SQL.Append(" SELECT * FROM S_TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(txtTorisCode.Text))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(txtTorifCode.Text))


            If OraReader.DataReader(SQL) = True Then
                lblToriName.Text = OraReader.GetString("ITAKU_NNAME_T")
                For No As Integer = 1 To 31
                    If OraReader.GetString("DATE" & No.ToString & "_T") = "1" Then
                        lblFuriDate.Text = No.ToString("00")
                        Exit For
                    End If
                Next
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(取引先情報設定)", "失敗", ex.ToString)
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub
    '======================================

    '数値評価(Zero埋めフォーマット有り：金融機関登録エリア：金融機関の場合)
    Private Sub Bank_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKinCode.Validating
        Try
            Dim Temp As String = GCom.NzDec(txtKinCode.Text, "")
            If Temp.Length = 0 Then
                txtKinCode.Text = ""
                lblKinNName.Text = ""
            Else
                Call GCom.NzNumberString(txtKinCode, True)
                lblKinNName.Text = GCom.GetBKBRName(txtKinCode.Text, "", 30)
                If txtKinCode.Text = Jikinko Then
                    btnKouzaCheck.Enabled = True
                Else
                    btnKouzaCheck.Enabled = False
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("金融機関設定", "失敗", ex.ToString)
        End Try
    End Sub

    '数値評価(Zero埋めフォーマット有り：金融機関登録エリア：支店の場合)
    Private Sub Branch_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtSitCode.Validating
        Try
            Dim Temp As String = GCom.NzDec(txtSitCode.Text, "")
            If Temp.Length = 0 Then
                txtSitCode.Text = ""
                lblSitNName.Text = ""
            Else
                Call GCom.NzNumberString(txtSitCode, True)
                Dim BKCode As String = txtKinCode.Text
                Dim BRCode As String = txtSitCode.Text
                lblSitNName.Text = GCom.GetBKBRName(BKCode, BRCode, 30)
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("支店設定", "失敗", ex.ToString)
        End Try

    End Sub

    'カナ項目チェック用
    Private Sub NzCheckString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtkeiyakuKName.Validating, txtJyuyokaNo.Validating
        Try
            Call GCom.NzCheckString(CType(sender, TextBox))
            Dim BRet As Boolean = GCom.CheckZenginChar(CType(sender, TextBox))
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("カナ変換", "失敗", ex.ToString)
        End Try
    End Sub

    '全角混入領域バイト数調整用
    Private Sub GetLimitString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKeiyakuNAdress.Validating, txtKeiyakuNName.Validating
        Try
            With CType(sender, TextBox)
                .Text = GCom.GetLimitString(.Text, .MaxLength)
            End With
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("全角バイト数調整", "失敗", ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' 契約者コードバリデイティングイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>2016/04/25 タスク）綾部 ADD 画面操作性向上</remarks>
    Private Sub txtKeiyakuCode_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtKeiyakuCode.Validating
        Try
            If Me.txtKeiyakuCode.Text.Trim = String.Empty Then
                '空だったら新規だと判断
                Me.btnTUUBAN.Focus()
            Else
                '何か入っていたら参照ボタンにフォーカス
                Call GCom.NzNumberString(txtKeiyakuCode, True)
                Me.btnSansyo.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub

#End Region

End Class