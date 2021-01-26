Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT040
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 未収リスト印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Dim Str_Report_Path As String
    Dim STR処理名 As String
    '2006/10/20
    Dim STR学校コード As String

    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT040", "未収リスト印刷画面")
    Private Const msgTitle As String = "未収リスト印刷画面(KFGPRNT040)"
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGPRNT040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '入力ボタン制御
            btnPrnt.Enabled = True
            btnEnd.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click
        Dim OraReader As MyOracleReader
        Dim SQL As New StringBuilder
        Dim Flg As String = ""
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Try
            '印刷ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            MainDB = New MyOracle

            '入力チェック
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            'ソート順番選択チェック
            If chk生徒番号順.Checked = False And chk通番順.Checked = False And chkあいうえお順.Checked = False And chk性別順.Checked = False Then
                MessageBox.Show("ソート順番が１つも選択されていません。 ", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                chk生徒番号順.Focus()
                Exit Sub
            End If
            Flg = IIf(chk生徒番号順.Checked, "1,", "0,") & IIf(chk通番順.Checked, "1,", "0,") & _
                  IIf(chkあいうえお順.Checked, "1,", "0,") & IIf(chk性別順.Checked, "1", "0")


            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)

            STR学校コード = txtGAKKOU_CODE.Text

            If PFUC_SCHMAST_GET() <> 0 Then
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            If PFUNC_未収リスト印刷可能判定() = False Then
                '印刷対象なしメッセージ
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "未収リスト"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                OraReader = New MyOracleReader(MainDB)
                SQL.Append(" SELECT distinct GAKKOU_CODE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S =" & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND( FURI_KBN_S ='0' OR FURI_KBN_S ='1')")

                '2006/10/11 不能フラグを条件に追加
                SQL.Append(" AND FUNOU_FLG_S ='1' ")
                SQL.Append(" AND FUNOU_KEN_S > 0 ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

                'スケジュール存在チェック
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("スケジュールが存在しません", msgTitle, MessageBoxButtons.OK)
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                While OraReader.EOF = False

                    STR学校コード = OraReader.GetString("GAKKOU_CODE_S")
                    Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & Flg
                    nRet = ExeRepo.ExecReport("KFGP022.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "未収リスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                    End Select


                    OraReader.NextRead()
                End While
            Else
                Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & Flg
                nRet = ExeRepo.ExecReport("KFGP022.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "未収リスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
            End If
            MessageBox.Show(String.Format(MSG0014I, "未収リスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try
    End Sub
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
#End Region
#Region " イベント"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '学校カナ絞込みコンボ
        '********************************************
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校名コンボボックス設定
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = True Then
            cmbGakkouName.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '学校検索後の学校コード設定
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '学校名の取得
        lab学校名.Text = cmbGakkouName.Text

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
        With CType(sender, TextBox)
            If .Text.Trim <> "" Then
                STR学校コード = Trim(txtGAKKOU_CODE.Text)
                '学校名の取得
                If PFUC_GAKNAME_GET() = False Then
                    Exit Sub
                End If
            End If
        End With
    End Sub
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("ゼロパディング", "失敗", ex.ToString)
        End Try
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False

        '学校コード必須チェック
        If txtGAKKOU_CODE.Text.Trim = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
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

        STR_FURIKAE_DATE(0) = Trim(txtFuriDateY.Text) & "/" & Trim(txtFuriDateM.Text) & "/" & Trim(txtFuriDateD.Text)
        STR_FURIKAE_DATE(1) = Trim(txtFuriDateY.Text) & Format(CInt(txtFuriDateM.Text), "00") & Format(CInt(txtFuriDateD.Text), "00")

        PFUNC_Nyuryoku_Check = True

    End Function

    Private Function PFUC_GAKNAME_GET() As Boolean
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim OraDB As MyOracle = Nothing
        Try

            '学校名の設定
            PFUC_GAKNAME_GET = True

            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab学校名.Text = "全学校対象"
            Else
                SQL.Append("SELECT * FROM KZFMAST.GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G  = " & SQ(txtGAKKOU_CODE.Text.Trim))
                '学校マスタ１存在チェック
                OraDB = New MyOracle
                OraReader = New MyOracleReader(OraDB)
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("学校マスタに登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lab学校名.Text = ""
                    txtGAKKOU_CODE.Focus()
                    PFUC_GAKNAME_GET = False
                    Exit Function
                End If

                lab学校名.Text = OraReader.GetString("GAKKOU_NNAME_G")
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校名取得)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraDB IsNot Nothing Then OraDB.Close()
        End Try

    End Function
    Private Function PFUC_SCHMAST_GET() As Integer

        PFUC_SCHMAST_GET = 0
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append("SELECT * FROM KZFMAST.G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_S    = " & SQ(STR_FURIKAE_DATE(1)))
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                SQL.Append(" AND GAKKOU_CODE_S = " & SQ(txtGAKKOU_CODE.Text.Trim))
            Else
                SQL.Append(" AND FUNOU_FLG_S = '1'")
            End If
            SQL.Append(" AND (FURI_KBN_S = '0' OR  FURI_KBN_S = '1')")

            'スケジュールが存在チェック
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("対象スケジュールが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                PFUC_SCHMAST_GET = 1
                Exit Function
            Else
                SQL = New StringBuilder
                SQL.Append("SELECT * FROM KZFMAST.G_SCHMAST WHERE")
                SQL.Append(" FURI_DATE_S    = " & SQ(STR_FURIKAE_DATE(1)))
                If txtGAKKOU_CODE.Text <> "9999999999" Then
                    SQL.Append(" AND GAKKOU_CODE_S  = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
                Else
                    SQL.Append(" AND FUNOU_FLG_S  = '1'")
                    SQL.Append(" AND FUNOU_KEN_S  > 0")
                End If
                SQL.Append(" AND (FURI_KBN_S = '0' OR  FURI_KBN_S = '1')")

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("不能件数が0件以上のスケジュールが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    PFUC_SCHMAST_GET = 1
                    Exit Function
                End If

                If OraReader.GetString("FUNOU_FLG_S") = "0" Then
                    MessageBox.Show("このスケジュールは不能結果更新処理が未処理です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
                If OraReader.GetString("FUNOU_KEN_S") = 0 Then
                    MessageBox.Show("このスケジュールは不能件数が0件です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    PFUC_SCHMAST_GET = 1
                    Exit Function
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール取得)", "失敗", ex.ToString)
            Return 1
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
    Private Function PFUNC_未収リスト印刷可能判定() As Boolean

        PFUNC_未収リスト印刷可能判定 = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM G_MEIMAST")
            SQL.Append(" WHERE")
            SQL.Append(" FURIKETU_CODE_M <> 0")
            '2006/10/16 再振済みフラグのチェックをはずす。再振処理後も初振の未収リストを出力できるようにするため。
            'STR_SQL += " G_MEIMAST.SAIFURI_SUMI_M ='0'"
            'STR_SQL += " AND"
            SQL.Append(" AND FURI_DATE_M <=" & SQ(STR_FURIKAE_DATE(1)))
            SQL.Append(" AND ( FURI_KBN_M ='0' OR FURI_KBN_M ='1')")
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                SQL.Append(" AND GAKKOU_CODE_M =" & SQ(txtGAKKOU_CODE.Text))
            End If

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            PFUNC_未収リスト印刷可能判定 = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(未収リスト印刷可能判定)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function


#End Region
#Region " 旧クエリ(クリレポ)"
    Private Function PFUC_SQLQuery_MAKE() As String
        Dim SSQL As String

        PFUC_SQLQuery_MAKE = ""

        SSQL = "SELECT "
        SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
        SSQL = SSQL & ",G_MEIMAST.NENDO_M "
        SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M "
        SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M "
        SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M "
        SSQL = SSQL & ",G_MEIMAST.TUUBAN_M "
        SSQL = SSQL & ",G_MEIMAST.TKIN_NO_M "
        SSQL = SSQL & ",G_MEIMAST.TSIT_NO_M "
        SSQL = SSQL & ",G_MEIMAST.TKAMOKU_M "
        SSQL = SSQL & ",G_MEIMAST.TKOUZA_M "
        SSQL = SSQL & ",G_MEIMAST.TMEIGI_KNM_M "
        SSQL = SSQL & ",G_MEIMAST.SEIKYU_TUKI_M "
        SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M "

        SSQL = SSQL & ",SEITOMAST.GAKKOU_CODE_O"
        SSQL = SSQL & ",SEITOMAST.NENDO_O "
        SSQL = SSQL & ",SEITOMAST.TUUBAN_O "
        'SSQL = SSQL & ",SEITOMAST.GAKUNEN_CODE_O "
        'SSQL = SSQL & ",SEITOMAST.CLASS_CODE_O "
        SSQL = SSQL & ",SEITOMAST.SEITO_NO_O "
        SSQL = SSQL & ",SEITOMAST.SEITO_KNAME_O "
        SSQL = SSQL & ",SEITOMAST.SEITO_NNAME_O "
        SSQL = SSQL & ",SEITOMAST.SEIBETU_O "

        SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"
        'SSQL = SSQL & ",GAKMAST1.GAKUNEN_NNAME_G"
        'SSQL = SSQL & ",GAKMAST1.GAKKOU_CODE_G"

        SSQL = SSQL & ",TENMAST.KIN_NO_N"
        SSQL = SSQL & ",TENMAST.SIT_NO_N"
        'SSQL = SSQL & ",TENMAST.KIN_NNAME_N "
        'SSQL = SSQL & ",TENMAST.SIT_NNAME_N "

        SSQL = SSQL & ",NVL(G_MEIMAST.GAKKOU_CODE_M,0)"
        SSQL = SSQL & ",NVL(G_MEIMAST.NENDO_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.GAKUNEN_CODE_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.CLASS_CODE_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.SEITO_NO_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TUUBAN_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TKIN_NO_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TSIT_NO_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TKAMOKU_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TKOUZA_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.TMEIGI_KNM_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.SEIKYU_TUKI_M,0) "
        SSQL = SSQL & ",NVL(G_MEIMAST.SEIKYU_KIN_M,0) "

        SSQL = SSQL & ",NVL(SEITOMAST.GAKKOU_CODE_O,0)"
        SSQL = SSQL & ",NVL(SEITOMAST.NENDO_O,0) "
        SSQL = SSQL & ",NVL(SEITOMAST.TUUBAN_O,0) "
        SSQL = SSQL & ",NVL(SEITOMAST.SEITO_NO_O,0) "
        SSQL = SSQL & ",NVL(SEITOMAST.SEITO_KNAME_O,0) "
        SSQL = SSQL & ",NVL(SEITOMAST.SEITO_NNAME_O,0) "
        SSQL = SSQL & ",NVL(SEITOMAST.SEIBETU_O,0) "

        SSQL = SSQL & ",NVL(GAKMAST1.GAKKOU_NNAME_G,0)"

        SSQL = SSQL & ",NVL(TENMAST.KIN_NO_N,0)"
        SSQL = SSQL & ",NVL(TENMAST.SIT_NO_N,0)"

        SSQL = SSQL & " FROM "
        SSQL = SSQL & "  KZFMAST.G_MEIMAST"
        SSQL = SSQL & " ,KZFMAST.SEITOMAST"
        'SSQL = SSQL & " ,KZFMAST.G_SCHMAST"
        SSQL = SSQL & " ,KZFMAST.GAKMAST1"
        SSQL = SSQL & " ,KZFMAST.TENMAST"

        SSQL = SSQL & " WHERE "
        SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = SEITOMAST.GAKKOU_CODE_O  "
        SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMAST.NENDO_O "
        SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMAST.TUUBAN_O "
        SSQL = SSQL & " AND G_MEIMAST.HIMOKU_ID_M    = SEITOMAST.HIMOKU_ID_O "

        'SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_KBN_M     = G_SCHMAST.FURI_KBN_S "
        'SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    = G_SCHMAST.FURI_DATE_S "

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G  "

        SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N "
        SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N "

        SSQL = SSQL & " AND GAKMAST1.GAKUNEN_CODE_G = 1"

        SSQL = SSQL & " AND SEITOMAST.TUKI_NO_O ='" & Format(CInt(txtFuriDateM.Text), "00") & "'"

        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  =" & "'" & STR学校コード & "'"

        SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <> 0"
        '2006/10/16　再振フラグのチェックをはずす。再振処理後も初振の未収リストを出力するため。
        'SSQL = SSQL & " AND G_MEIMAST.SAIFURI_SUMI_M  =" & "'0'"
        SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M     =" & "'" & STR_FURIKAE_DATE(1) & "'"
        SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M     =" & "'0'" & " OR G_MEIMAST.FURI_KBN_M = " & "'1')"

        'ソート条件
        SSQL = SSQL & " ORDER BY "
        '2006/10/12　学校コードをソート条件に追加
        SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M ASC"
        SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M ASC"
        SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M ASC"
        'If chk生徒番号順.Checked = True OR chk通番順.Checked = True OR _
        '   chkあいうえお順.Checked = True OR chk性別順.Checked = True Then
        '    SSQL += ","
        'End If

        If chk生徒番号順.Checked = True Then
            SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M ASC"
        End If

        If chk通番順.Checked = True Then
            'If chk生徒番号順.Checked = True Then
            '    SSQL = SSQL & ","
            'End If
            SSQL = SSQL & ",G_MEIMAST.NENDO_M ASC"
            SSQL = SSQL & ",G_MEIMAST.TUUBAN_M ASC"
        End If

        If chkあいうえお順.Checked = True Then
            'If chk生徒番号順.Checked = True OR chk通番順.Checked = True Then
            '    SSQL = SSQL & ","
            'End If
            '2007/02/14 生徒カナ名順に出力
            'SSQL = SSQL & ",SEITOMAST.SEITO_NNAME_O ASC"
            SSQL = SSQL & ",SEITOMAST.SEITO_KNAME_O ASC"
        End If

        If chk性別順.Checked = True Then
            'If chk生徒番号順.Checked = True OR chk通番順.Checked = True OR chkあいうえお順.Checked = True Then
            '    SSQL = SSQL & ","
            'End If
            SSQL = SSQL & ",SEITOMAST.SEIBETU_O ASC"
        End If

        PFUC_SQLQuery_MAKE = SSQL

    End Function

#End Region
End Class
