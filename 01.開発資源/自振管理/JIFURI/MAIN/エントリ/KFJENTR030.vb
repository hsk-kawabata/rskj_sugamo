Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Public Class KFJENTR030

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private strBAITAI_CODE As String

    Private MainLOG As New CASTCommon.BatchLOG("KFJENTR030", "依頼書用データ入力画面")
    Private Const msgTitle As String = "依頼書用データ入力画面(KFJENTR030)"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Const ThisModuleName As String = "KFJENTR030.vb"
    Dim KFJentr041 As New KFJENTR041()

    'SQLキー項目用
    Private strKEY1 As String
    Private strKEY2 As String
    Private strKEY3 As String

    Private BERYFAI As String   'ベリファイ要否区分
#Region " ロード"
    Private Sub KFJENTR030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label2, Label4, lblUser, lblDate)

            '------------------------------------
            'INIファイルの読み込み
            '------------------------------------
            If fn_INI_Read() = False Then
                Return
            End If
            'ベリファイ不要の場合は副を使用不可にする
            If BERYFAI = "0" Then
                rdbFuku.Enabled = False
            End If

            '--------------------------------
            '委託者名リストボックスの設定
            '--------------------------------
            '取引先コンボボックス設定(Msg????W)
            Dim Jyoken As String = " AND BAITAI_CODE_T = '04'"   '媒体が依頼書
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 実行"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :実行ボタン
        'Return         :
        'Create         :2009/09/29
        'Update         :
        '=====================================================================================
        Try
            '2018/02/09 saitou 広島信金(RSV2標準) UPD エントリ20明細対応 -------------------- START
            Dim KFJENTR031 As KFJENTR031 = Nothing
            Dim KFJENTR032 As KFJENTR032 = Nothing
            'Dim KFJENTR031 As New KFJENTR031
            '2018/02/09 saitou 広島信金(RSV2標準) UPD --------------------------------------- END
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            strKEY1 = txtTorisCode.Text
            strKEY2 = txtTorifCode.Text
            strKEY3 = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            LW.ToriCode = strKEY1 & strKEY2
            LW.FuriDate = strKEY3


            '2018/02/09 saitou 広島信金(RSV2標準) UPD エントリ20明細対応 -------------------- START
            Dim ShowFormFlg As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KFJENTR030_SHOWFORM")
            If ShowFormFlg = "1" Then
                KFJENTR032 = New KFJENTR032
                '必要項目を変数にセットする
                KFJENTR032.strTORIS_CODE = strKEY1
                KFJENTR032.strTORIF_CODE = strKEY2
                KFJENTR032.strFURI_DATE = strKEY3
                If txtSyokiti.Text = "" Then
                    KFJENTR032.strSYOKITI = ""
                Else
                    KFJENTR032.strSYOKITI = txtSyokiti.Text.Trim
                End If
                If rdbSei.Checked Then
                    KFJENTR032.intCHK = 0
                Else
                    KFJENTR032.intCHK = 1
                End If
                If txtKaisiPage.Text.Trim = "" Then                                  '開始ﾍﾟｰｼﾞ
                    KFJENTR032.lngPAGE = 1                                                '(未入力時は1をｾｯﾄ)
                Else
                    KFJENTR032.lngPAGE = GCom.NzLong(txtKaisiPage.Text)
                End If
                KFJENTR032.BERYFAI = BERYFAI
            Else
                KFJENTR031 = New KFJENTR031
                '必要項目を変数にセットする
                KFJENTR031.strTORIS_CODE = strKEY1
                KFJENTR031.strTORIF_CODE = strKEY2
                KFJENTR031.strFURI_DATE = strKEY3
                If txtSyokiti.Text = "" Then
                    KFJENTR031.strSYOKITI = ""
                Else
                    KFJENTR031.strSYOKITI = txtSyokiti.Text.Trim
                End If
                If rdbSei.Checked Then
                    KFJENTR031.intCHK = 0
                Else
                    KFJENTR031.intCHK = 1
                End If
                If txtKaisiPage.Text.Trim = "" Then                                  '開始ﾍﾟｰｼﾞ
                    KFJENTR031.lngPAGE = 1                                                '(未入力時は1をｾｯﾄ)
                Else
                    KFJENTR031.lngPAGE = GCom.NzLong(txtKaisiPage.Text)
                End If
                KFJENTR031.BERYFAI = BERYFAI
            End If
            '必要項目を変数にセットする
            'gstrTORIS_CODE = strKEY1
            'gstrTORIF_CODE = strKEY2
            'gstrFURI_DATE = strKEY3
            'If txtSyokiti.Text = "" Then
            '    gstrSYOKITI = ""
            'Else
            '    gstrSYOKITI = txtSyokiti.Text.Trim
            'End If
            'If rdbSei.Checked Then
            '    gintCHK = 0
            'Else
            '    gintCHK = 1
            'End If
            'If txtKaisiPage.Text.Trim = "" Then                                  '開始ﾍﾟｰｼﾞ
            '    glngPAGE = 1                                                '(未入力時は1をｾｯﾄ)
            'Else
            '    glngPAGE = GCom.NzLong(txtKaisiPage.Text)
            'End If
            'KFJENTR031.BERYFAI = BERYFAI
            '2018/02/09 saitou 広島信金(RSV2標準) UPD --------------------------------------- END
            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If


            '伝票用情報入力画面を開く
            '2018/02/09 saitou 広島信金(RSV2標準) UPD エントリ20明細対応 -------------------- START
            If ShowFormFlg = "1" Then
                Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFJENTR032, Form), Me)
            Else
                Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFJENTR031, Form), Me)
            End If
            'Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJENTR031, Form), Me)
            '2018/02/09 saitou 広島信金(RSV2標準) UPD --------------------------------------- END
        Catch ex As Exception
            If MainDB IsNot Nothing Then
                MainDB.Rollback()
            End If
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
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
    Public Function fn_INI_Read() As Boolean
        '============================================================================
        'NAME           :fn_INI_Read
        'Parameter      :
        'Description    :FSKJ.INIファイルの読み込み
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/30
        'Update         :
        '============================================================================
        fn_INI_Read = False
        'ベリファイ要否区分
        BERYFAI = CASTCommon.GetFSKJIni("COMMON", "BERYFAI")
        If BERYFAI.ToUpper = "ERR" OrElse BERYFAI = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "ベリファイ要否区分", "COMMON", "BERYFAI"), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("設定ファイル取得", "失敗", "項目名:ベリファイ要否区分 分類:COMMON 項目:BERYFAI")
            Return False
        End If
        fn_INI_Read = True
    End Function
    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :印刷ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_check_text = False
        Try
            '取引先主コード必須チェック(MSG0057W)
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If
            '取引先副コード必須チェック(MSG0059W)
            If txtTorifCode.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorifCode.Focus()
                Return False
            End If
            '年必須チェック(MSG0018W)
            If txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
            '月必須チェック(MSG0020W)
            If txtFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '月範囲チェック'(MSG0022W)
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '日付必須チェック(MSG0023W)
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If
            '日付範囲チェック(MSG0025W)
            If GCom.NzInt(txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '日付整合性チェック(MSG0027W)
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            If txtKaisiPage.Text <> "" AndAlso GCom.NzLong(txtKaisiPage.Text) = 0 Then
                txtKaisiPage.Text = "1"
            End If
            fn_check_text = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try

    End Function
    Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_Table
        'Parameter      :
        'Description    :印刷ボタンを押下時にマスタの相関チェックを実行
        'Return         :True=OK,False=NG
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_check_Table = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            '取引先情報取得
            SQL.Append("SELECT * FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(Trim(txtTorisCode.Text)))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(txtTorifCode.Text)))
            If OraReader.DataReader(SQL) = True Then
                strBAITAI_CODE = GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T"))
                OraReader.Close()
            Else
                '取引先なし
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                OraReader.Close()
                txtTorisCode.Focus()
                Return False
            End If

            '媒体コードチェック
            If strBAITAI_CODE <> "04" Then '依頼書
                MessageBox.Show(MSG0108W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If

            'スケジュールマスタに印刷対象のスケジュールが存在するかチェックする
            SQL = New StringBuilder(128)
            SQL.Append("SELECT * ")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='1'")
            SQL.Append(" AND TORIS_CODE_T = " & SQ(strKEY1))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(strKEY2))
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            SQL.Append(" AND FURI_DATE_S = " & SQ(strKEY3))
            SQL.Append(" AND TYUUDAN_FLG_S = '0' ")

            Dim TOUROKU_FLG As String
            If OraReader.DataReader(SQL) = True Then
                TOUROKU_FLG = GCom.NzStr(OraReader.Reader.Item("TOUROKU_FLG_S"))
                OraReader.Close()
            Else
                'スケジュールなし
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                OraReader.Close()
                Return False
            End If

            If TOUROKU_FLG = "1" Then
                '登録済み
                MessageBox.Show(MSG0061W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            '副を入力する際は、初回データが登録済みかチェックする
            If rdbFuku.Checked = False Then
                Return True
            End If
            Dim Count As Integer
            SQL = New StringBuilder(128)
            SQL.Append(" SELECT COUNT(*) COUNTER ")
            SQL.Append(" FROM ENTMAST")
            SQL.Append(" WHERE FSYORI_KBN_E ='1'")
            SQL.Append(" AND TORIS_CODE_E = " & SQ(strKEY1))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strKEY2))
            SQL.Append(" AND FURI_DATE_E = " & SQ(strKEY3))
            If OraReader.DataReader(SQL) = True Then
                Count = GCom.NzInt(OraReader.Reader.Item("COUNTER"))
                OraReader.Close()
            Else
                '検索失敗(MSG0002E)
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                txtFuriDateY.Focus()
                Return False
            End If

            If Count = 0 Then
                '初回登録なし
                MessageBox.Show(MSG0118W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_check_Table = True
    End Function
    Function fn_DEL_KANMA(ByVal aINTEXT As String) As Long
        '============================================================================
        'NAME           :fn_DEL_KANMA
        'Parameter      :aINTEXT:入力値
        'Description    :カンマ編集(カンマ削除）関数
        'Return         :カンマ削除後の数値
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        Dim I As Integer
        Dim strO_SYOKITI As String = ""

        For I = 0 To aINTEXT.Length - 1        'ｶﾝﾏ&ｽﾍﾟｰｽとる
            If aINTEXT.Substring(I, 1) = "," Or aINTEXT.Substring(I, 1) = " " Then
                strO_SYOKITI &= ""
            Else
                strO_SYOKITI &= aINTEXT.Substring(I, 1)
            End If
        Next I
        fn_DEL_KANMA = CLng(strO_SYOKITI)

    End Function
#End Region
#Region " イベント"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '選択カナで始まる委託者名を取得
        '--------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定(Msg????W)
                Dim Jyoken As String = " AND BAITAI_CODE_T = '04'"   '依頼書
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If
            cmbToriName.Focus()
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
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コード取得)", "失敗", ex.ToString)
        End Try

    End Sub
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
             Handles txtTorisCode.Validating, txtTorifCode.Validating, txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub
#End Region

#Region " 振替日変更"
    Private Sub btnFuriDate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFuriDate.Click
        '=====================================================================================
        'NAME           :btnFuriDate_Click
        'Parameter      :
        'Description    :振替日変更ボタン
        'Return         :
        'Create         :2010/02/05
        'Update         :
        '=====================================================================================
        Dim KFJMAST031 As KFJMAST031
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振替日変更)開始", "成功", "")
            KFJMAST031 = New KFJMAST031
            If KFJMAST031.fn_check_text(txtTorisCode, txtTorifCode, txtFuriDateY, txtFuriDateM, txtFuriDateD) = False Then
                Return
            End If
            If KFJMAST031.fn_check_Table(txtTorisCode, txtTorifCode, txtFuriDateY, txtFuriDateM, txtFuriDateD, True) = False Then
                Return
            End If

            gstrFURI_DATE = ""
            With KFJMAST031
                .Owner = Me
                .ShowDialog()
            End With

            If gstrFURI_DATE = "" Then
                Return
            End If

            txtFuriDateY.Text = Mid(gstrFURI_DATE, 1, 4)
            txtFuriDateM.Text = Mid(gstrFURI_DATE, 5, 2)
            txtFuriDateD.Text = Mid(gstrFURI_DATE, 7, 2)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振替日変更)例外", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振替日変更)終了", "成功", "")
        End Try
    End Sub
#End Region
End Class