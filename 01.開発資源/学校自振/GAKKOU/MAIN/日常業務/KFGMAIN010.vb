Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Public Class KFGMAIN010

#Region " 共通変数定義 "
    Private Str_Date_Start As String
    Private Str_Date_End As String
    Private Str_Date_Saifuri As String

    Private Str_Key_1 As String
    Private Str_Key_2 As String
    Private Str_Key_3 As String

    Private Lng_Syukei_Cnt As Long
    Private Lng_Syukei_Kingaku As Long

    Private Lng_GSyofuri(1, 9) As Long
    Private Lng_Syofuri_Cnt As Long
    Private Lng_Syofuri_Kingaku As Long

    Private Lng_GSaifuri(1, 9) As Long
    Private Lng_Saifuri_Cnt As Long
    Private Lng_Saifuri_Kingaku As Long

    Private Lng_Himoku_Cnt(14, 9) As Long
    Private Lng_Himoku_Kingaku(14, 9) As Long

    Private Int_Record_Cnt As Integer

    Private Str_SFURI_SYUBETU As String

    '2006/01/11
    Private strFURI_KBN As String '振替区分 0:初振　1:再振
    '2006/10/25
    Private strSAIFURI_DATE(9) As String

    Public Syukei_Ketuka As Integer

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAIN010", "口座振替金額確認画面")
    Private Const msgTitle As String = "口座振替金額確認画面(KFGMAIN010)"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    '2017/04/11 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ START
    '再振対象コードIniファイル化対応
    Private SFuriCode As String = String.Empty
    '2017/04/11 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ END
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAIN010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            STR_SYORI_NAME = "口座振替金額確認画面"
            MainLOG = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                Call MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '2017/04/11 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ START
            '再振対象コードIniファイル化対応
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            '2017/04/11 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ END

            Call PSUB_Set_Format_Value()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnSerch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSerch.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(確認)開始", "成功", "")
            '入力値チェック
            If PFUNC_Nyuryoku_Check(1) = False Then
                Exit Sub
            End If

            '確認メッセージ
            If MessageBox.Show(G_MSG0008I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Exit Sub
            End If

            Int_Record_Cnt = 0

            '生徒マスタビューより請求月の集計件数と集計金額を表示
            '金額が０円の生徒は件数にカウントされない
            If PFUNC_Set_Shukei_Kingaku() = False Then
                Exit Sub
            End If

            '集計結果と入力値の差異チェック
            '一致していない場合は以下の処理は行わない
            If PFUNC_Chk_Shukei_Nyuryoku() = False Then

                Exit Sub
            End If

            '実績マスタ登録
            If PFUNC_Insert_Gakunen_Kingaku() = False Then

                Exit Sub
            End If

            '明細マスタ削除
            '取消処理後に残っている為
            If PFUNC_Delete_Meisai() = False Then

                Exit Sub
            End If

            MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(確認)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(確認)終了", "成功", "")
        End Try

    End Sub
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")
            '入力値チェック
            If PFUNC_Nyuryoku_Check(0) = False Then
                Exit Sub
            End If

            '参照時は、学校コード～振替日までを入力不可
            txtGAKKOU_CODE.Enabled = False
            txtSYear.Enabled = False
            txtSMonth.Enabled = False

            txtFuriDateY.Enabled = False
            txtFuriDateM.Enabled = False
            txtFuriDateD.Enabled = False

            STR_SQL = " SELECT "
            STR_SQL += " GAKKOU_CODE_F , SEIKYU_NENGETU_F , FURI_DATE_F"
            STR_SQL += ", sum(SEIKYU_KEN_F) as SUM_SEIKYU_KEN_F"
            STR_SQL += ", sum(SEIKYU_KIN_F) as SUM_SEIKYU_KIN_F"
            STR_SQL += ", sum(SYO_KEN_F) as SUM_SYO_KEN_F"
            STR_SQL += ", sum(SYO_KIN_F) as SUM_SYO_KIN_F"
            STR_SQL += ", sum(SAI_KEN_F) as SUM_SAI_KEN_F"
            STR_SQL += ", sum(SAI_KIN_F) as SUM_SAI_KIN_F"
            STR_SQL += " FROM JISSEKIMAST"
            STR_SQL += " WHERE GAKKOU_CODE_F = '" & Str_Key_1 & "'"
            STR_SQL += " AND SEIKYU_NENGETU_F = '" & Str_Key_2 & "'"
            STR_SQL += " AND FURI_DATE_F = '" & Str_Key_3 & "'"
            STR_SQL += " group by GAKKOU_CODE_F , SEIKYU_NENGETU_F , FURI_DATE_F"

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            If OBJ_DATAREADER.HasRows = False Then
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Sub
                End If

                Call MessageBox.Show(G_MSG0017W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Sub
            End If

            OBJ_DATAREADER.Read()

            Lng_Syukei_Cnt = OBJ_DATAREADER.Item("SUM_SEIKYU_KEN_F")
            Lng_Syukei_Kingaku = OBJ_DATAREADER.Item("SUM_SEIKYU_KIN_F")

            Lng_Syofuri_Cnt = OBJ_DATAREADER.Item("SUM_SYO_KEN_F")
            Lng_Syofuri_Kingaku = OBJ_DATAREADER.Item("SUM_SYO_KIN_F")

            Lng_Saifuri_Cnt = OBJ_DATAREADER.Item("SUM_SAI_KEN_F")
            Lng_Saifuri_Kingaku = OBJ_DATAREADER.Item("SUM_SAI_KIN_F")

            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If

            txtSeikyuCnt.Text = Format(Lng_Syukei_Cnt, "#,##0")
            txtSeikyuKingaku.Text = Format(Lng_Syukei_Kingaku, "#,##0")

            '集計＝初振＋再振
            lblSyukeiCnt.Text = Format(Lng_Syukei_Cnt, "#,##0")
            lblSyukeiKingaku.Text = Format(Lng_Syukei_Kingaku, "#,##0")

            lblSyofuriCnt.Text = Format(Lng_Syofuri_Cnt, "#,##0")
            lblSyofuriKingaku.Text = Format(Lng_Syofuri_Kingaku, "#,##0")

            lblSaifuriCnt.Text = Format(Lng_Saifuri_Cnt, "#,##0")
            lblSaifuriKingaku.Text = Format(Lng_Saifuri_Kingaku, "#,##0")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")
            Call PSUB_Set_Format_Value()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub
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

#Region " Private Sub "
    '****************************
    'Private Sub
    '****************************
    Private Sub PSUB_Set_Format_Value()

        cmbKana.SelectedIndex = -1
        cmbGAKKOUNAME.SelectedIndex = -1

        txtGAKKOU_CODE.Text = ""
        txtGAKKOU_CODE.Enabled = True
        lblGAKKOU_NAME.Text = ""

        txtSYear.Text = Format(Now, "yyyy")
        txtSYear.Enabled = True
        txtSMonth.Text = Format(Now, "MM")
        txtSMonth.Enabled = True

        txtFuriDateY.Text = ""
        txtFuriDateY.Enabled = True
        txtFuriDateM.Text = ""
        txtFuriDateM.Enabled = True
        txtFuriDateD.Text = ""
        txtFuriDateD.Enabled = True

        txtSeikyuCnt.Text = ""
        txtSeikyuKingaku.Text = ""

        lblSyukeiCnt.Text = ""
        lblSyukeiKingaku.Text = ""
        lblKekka.Text = ""

        lblSyofuriCnt.Text = ""
        lblSyofuriKingaku.Text = ""

        lblSaifuriCnt.Text = ""
        lblSaifuriKingaku.Text = ""

    End Sub
    Private Sub PSUB_Kanma_Set(ByVal pText As TextBox, ByVal pIndex As Integer)

        Select Case pIndex
            Case 0
                Select Case pText.MaxLength
                    Case 6
                        pText.MaxLength = pText.MaxLength + 1
                    Case 12
                        pText.MaxLength = pText.MaxLength + 3
                End Select

                pText.Text = Format(CLng(pText.Text), "#,##0")
            Case 1
                Select Case pText.MaxLength
                    Case 7
                        pText.MaxLength = pText.MaxLength - 1
                    Case 15
                        pText.MaxLength = pText.MaxLength - 3
                End Select

                pText.Text = Format(CLng(pText.Text), "###0")
        End Select
    End Sub
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
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = True Then
            cmbGAKKOUNAME.Focus()
        End If

    End Sub
    Private Sub cmbGAKKOUNAME_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGAKKOUNAME.SelectedIndexChanged

        If cmbGAKKOUNAME.SelectedIndex = -1 Then
            Exit Sub
        End If

        'COMBOBOX選択時学校名,学校コード設定
        lblGAKKOU_NAME.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)

        '学年テキストボックスにFOCUS
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '学校名検索
            STR_SQL = "SELECT GAKKOU_NNAME_G FROM GAKMAST1 "
            STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            OBJ_DATAREADER.Read()
            If OBJ_DATAREADER.HasRows = True Then
                lblGAKKOU_NAME.Text = CStr(OBJ_DATAREADER.Item("GAKKOU_NNAME_G"))
            Else
                lblGAKKOU_NAME.Text = ""
            End If

            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If

            STR_SQL = "SELECT KAISI_DATE_T , SYURYOU_DATE_T FROM GAKMAST2 "
            STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            OBJ_DATAREADER.Read()
            If OBJ_DATAREADER.HasRows = False Then
                Str_Date_Start = ""
                Str_Date_End = ""
            Else
                Str_Date_Start = CStr(OBJ_DATAREADER.Item("KAISI_DATE_T"))
                Str_Date_End = CStr(OBJ_DATAREADER.Item("SYURYOU_DATE_T"))
            End If

            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If
        End If

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
    Private Function PFUNC_Nyuryoku_Check(ByVal pIndex As Integer) As Boolean

        PFUNC_Nyuryoku_Check = False

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()

            Exit Function
        Else
            '学校マスタ存在チェック
            STR_SQL = "SELECT GAKKOU_CODE_G"
            STR_SQL += " FROM GAKMAST1"
            STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If Trim(GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "GAKKOU_CODE_G")) = "" Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()

                Exit Function
            End If

            STR_SQL = "SELECT * FROM GAKMAST2"
            STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Function
            End If

            With OBJ_DATAREADER
                .Read()
                Str_SFURI_SYUBETU = .Item("SFURI_SYUBETU_T").ToString
            End With

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If
        End If

        Str_Key_1 = Trim(txtGAKKOU_CODE.Text)

        '請求年月(年)
        If Trim(txtSYear.Text) = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSYear.Focus()

            Exit Function
        Else
            '数値チェック
            If IsNumeric(txtSYear.Text) = False Then
                Call MessageBox.Show(MSG0019W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSYear.Focus()
                Exit Function
                'Else
                '    If txtSYear.Text.Trim.Length < 4 Then
                '        Call MessageBox.Show("処理対象年の入力が不正です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '        txtSYear.Focus()
                '        Exit Function
                '    End If
            End If
        End If

        '請求年月(月)
        If Trim(txtSMonth.Text) = "" Then
            Call MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSMonth.Focus()

            Exit Function
        Else
            Select Case CInt(txtSMonth.Text)
                Case 1 To 12
                Case Else
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSMonth.Focus()

                    Exit Function
            End Select
        End If

        Str_Key_2 = Trim(txtSYear.Text) & Format(CLng(txtSMonth.Text), "00")

        If Trim(Str_Date_Start) = "" Or Trim(Str_Date_End) = "" Then
            Call MessageBox.Show(G_MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Select Case (CInt(Str_Date_Start) <= CInt(Str_Key_2) <= CInt(Str_Date_End))
            Case False
                Call MessageBox.Show(String.Format(G_MSG0019W, String.Concat(New String() {Mid(Str_Date_Start, 1, 4), "/", Mid(Str_Date_Start, 5, 2)}), String.Concat(New String() {Mid(Str_Date_End, 1, 4), "/", Mid(Str_Date_End, 5, 2)})), _
                                     msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSYear.Focus()
                Exit Function
        End Select

        '振替日
        If Trim(txtFuriDateY.Text) = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY.Focus()
            Exit Function
            'Else
            '    If txtFuriDateY.Text.Trim.Length <> 4 Then
            '        Call MessageBox.Show("振替日(年)は４桁で設定してください", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        txtFuriDateY.Focus()
            '        Exit Function
            '    End If
        End If

        If Trim(txtFuriDateM.Text) = "" Then
            Call MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateM.Focus()
            Exit Function
        Else
            Select Case CInt(txtFuriDateM.Text)
                Case 1 To 12
                Case Else
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateM.Focus()
                    Exit Function
            End Select
        End If

        If Trim(txtFuriDateD.Text) = "" Then
            Call MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateD.Focus()
            Exit Function
        End If

        Str_Key_3 = Trim(txtFuriDateY.Text) & "/" & Trim(txtFuriDateM.Text) & "/" & Trim(txtFuriDateD.Text)

        If Not IsDate(Str_Key_3) Then
            Call MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY.Focus()
            Exit Function
        End If

        Select Case pIndex
            Case 1
                '請求件数

                If Trim(txtSeikyuCnt.Text) = "" Then
                    Call MessageBox.Show(MSG0071W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSeikyuCnt.Focus()
                    Exit Function
                End If
                '請求金額
                If Trim(txtSeikyuKingaku.Text) = "" Then
                    Call MessageBox.Show(MSG0073W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSeikyuKingaku.Focus()
                    Exit Function
                End If
        End Select

        Str_Key_3 = Trim(txtFuriDateY.Text) & Format(CInt(txtFuriDateM.Text), "00") & Format(CInt(txtFuriDateD.Text), "00")

        STR_FURIKAE_DATE(1) = Str_Key_3

        STR_SQL = " SELECT G_SCHMAST.* , SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T FROM G_SCHMAST"
        STR_SQL += " LEFT JOIN GAKMAST2 ON "
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T "
        STR_SQL += " WHERE GAKKOU_CODE_S = '" & Str_Key_1 & "'"
        STR_SQL += " AND NENGETUDO_S = '" & Str_Key_2 & "'"
        STR_SQL += " AND FURI_DATE_S = '" & Str_Key_3 & "'"
        STR_SQL += " AND (FURI_KBN_S = '0' OR FURI_KBN_S='1')"

        'ｽｹｼﾞｭｰﾙ区分2(随時)は処理に含まない為
        STR_SQL += " AND SCH_KBN_S <> '2'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        OBJ_DATAREADER.Read()

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Function
            End If

            Call MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        If OBJ_DATAREADER.Item("TYUUDAN_FLG_S") = 1 Then
            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Function
            End If

            Call MessageBox.Show(G_MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        Select Case pIndex
            Case 0
                If OBJ_DATAREADER.Item("CHECK_FLG_S") = 0 And _
                   OBJ_DATAREADER.Item("DATA_FLG_S") = 0 And _
                   OBJ_DATAREADER.Item("FUNOU_FLG_S") = 0 Then
                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Function
                    End If
                    Call MessageBox.Show(G_MSG0021W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
            Case Else
                If OBJ_DATAREADER.Item("CHECK_FLG_S") = 0 And _
                   OBJ_DATAREADER.Item("DATA_FLG_S") = 0 And _
                   OBJ_DATAREADER.Item("FUNOU_FLG_S") = 0 Then
                Else
                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Function
                    End If
                    Call MessageBox.Show(G_MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
        End Select

        Str_Date_Saifuri = OBJ_DATAREADER.Item("SFURI_DATE_S")

        strFURI_KBN = OBJ_DATAREADER.Item("FURI_KBN_S") '2006/01/11　追加

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_Chk_Shukei_Nyuryoku() As Boolean

        PFUNC_Chk_Shukei_Nyuryoku = False

        '件数の判定
        If Format(CLng(txtSeikyuCnt.Text), "###0") <> Format(CLng(lblSyukeiCnt.Text), "###0") Then
            lblKekka.Text = "不一致"
            Exit Function
        End If

        '金額の判定
        If Format(CLng(txtSeikyuKingaku.Text), "###0") <> Format(CLng(lblSyukeiKingaku.Text), "###0") Then
            lblKekka.Text = "不一致"
            Exit Function
        End If

        lblKekka.Text = "一致"
        PFUNC_Chk_Shukei_Nyuryoku = True

    End Function
    Private Function PFUNC_Insert_Gakunen_Kingaku() As Boolean

        Dim iLCount As Integer

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Function
        End If

        '挿入実績マスタ削除
        STR_SQL = "DELETE  FROM JISSEKIMAST"
        STR_SQL += " WHERE GAKKOU_CODE_F = '" & Str_Key_1 & "'"
        STR_SQL += " AND SEIKYU_NENGETU_F = '" & Str_Key_2 & "'"
        STR_SQL += " AND FURI_DATE_F= '" & Str_Key_3 & "'"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        '削除後に一旦COMMITしてDBを確定しておかないと、次の登録で重複エラーが起こる為
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Function
        End If

        For iLCount = 1 To UBound(Lng_GSyofuri, 2)
            Select Case True
                Case (Lng_GSyofuri(0, iLCount) <> 0 And Lng_GSyofuri(1, iLCount) <> 0) Or _
                     (Lng_GSaifuri(0, iLCount) <> 0 And Lng_GSaifuri(1, iLCount) <> 0)

                    '集計結果を登録
                    STR_SQL = " INSERT INTO JISSEKIMAST"
                    STR_SQL += " values("
                    STR_SQL += "'" & Str_Key_1 & "'"
                    STR_SQL += "," & iLCount
                    STR_SQL += ",'" & Str_Key_2 & "'"
                    STR_SQL += ",'" & Str_Key_3 & "'"
                    'STR_SQL += ",'" & Str_Date_Saifuri & "'"
                    STR_SQL += ",'" & strSAIFURI_DATE(iLCount) & "'"
                    STR_SQL += "," & Lng_GSyofuri(0, iLCount) + Lng_GSaifuri(0, iLCount)
                    STR_SQL += "," & Lng_GSyofuri(1, iLCount) + Lng_GSaifuri(1, iLCount)
                    STR_SQL += "," & Lng_GSyofuri(0, iLCount)
                    STR_SQL += "," & Lng_GSyofuri(1, iLCount)
                    STR_SQL += "," & Lng_GSaifuri(0, iLCount)
                    STR_SQL += "," & Lng_GSaifuri(1, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(0, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(0, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(1, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(1, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(2, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(2, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(3, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(3, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(4, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(4, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(5, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(5, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(6, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(6, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(7, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(7, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(8, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(8, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(9, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(9, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(10, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(10, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(11, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(11, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(12, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(12, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(13, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(13, iLCount)
                    STR_SQL += "," & Lng_Himoku_Cnt(14, iLCount)
                    STR_SQL += "," & Lng_Himoku_Kingaku(14, iLCount)
                    STR_SQL += ",0"
                    STR_SQL += ",0"
                    STR_SQL += ",0"
                    STR_SQL += ",0"

                    '*********************************************************
                    '2008/01/23  チェック結果、チェック日時項目追加の為、変更

                    If Syukei_Ketuka = 0 Then
                        STR_SQL += ",1"   '一致
                    Else
                        STR_SQL += ",9"   '不一致
                    End If

                    STR_SQL += "," & Format(Now, "yyyyMMdd") & ")"
                    '*********************************************************

                    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                        Exit Function
                    End If
            End Select
        Next iLCount

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        'スケジュールマスタの更新
        If PFUNC_Update_Schedule() = False Then
            Exit Function
        End If

        PFUNC_Insert_Gakunen_Kingaku = True

    End Function
    Private Function PFUNC_Update_Schedule() As Boolean

        PFUNC_Update_Schedule = False

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Function
        End If

        STR_SQL = " UPDATE  G_SCHMAST SET "
        STR_SQL += " CHECK_DATE_S='" & Format(Now, "yyyyMMdd") & "'"
        STR_SQL += ",CHECK_FLG_S='1'"
        STR_SQL += " WHERE GAKKOU_CODE_S = '" & Str_Key_1 & "'"
        STR_SQL += " AND NENGETUDO_S = '" & Str_Key_2 & "'"
        STR_SQL += " AND FURI_DATE_S = '" & Str_Key_3 & "'"
        STR_SQL += " AND (FURI_KBN_S = '0' OR FURI_KBN_S='1')"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_Update_Schedule = True

    End Function
    Private Function PFUNC_Set_Shukei_Kingaku() As Boolean

        Dim bLoopFlg As Boolean
        Dim bFlg As Boolean

        Dim bGFlg As Boolean

        Dim iGakunen_Flag(,) As Integer = Nothing
        Dim iTGakunen_Flag() As Integer = Nothing

        Dim lSyofuri(1, 1) As Long
        Dim lRecCnt(1) As Long

        Dim SfuriTuki As String = ""

        Dim lngSCH_CNT As Long = 0

        PFUNC_Set_Shukei_Kingaku = False

        bFlg = False

        '退避配列初期化
        For iLCount As Integer = 0 To 9
            Lng_GSyofuri(0, iLCount) = 0
            Lng_GSyofuri(1, iLCount) = 0

            Lng_GSaifuri(0, iLCount) = 0
            Lng_GSaifuri(1, iLCount) = 0
        Next iLCount
        '追加 2006/10/13
        ReDim Lng_Himoku_Cnt(14, 9)
        ReDim Lng_Himoku_Kingaku(14, 9)
        ReDim strSAIFURI_DATE(9)

        'スケジュールが年間、特別から
        Select Case PFUNC_Get_Gakunen(Str_Key_1, iGakunen_Flag, iTGakunen_Flag)
            Case -1
                'エラー

                Exit Function
            Case -2
                'エラー
                Call MessageBox.Show(G_MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                txtFuriDateY.SelectionStart = 0
                txtFuriDateY.SelectionLength = txtFuriDateY.Text.Length
                Exit Function
            Case 0
                '全学年が対象
                bGFlg = False
            Case Else
                '特定学年のみが対象
                bGFlg = True
        End Select

        'G_SCHMASTのカウント取得
        STR_SQL = " SELECT COUNT(*) "
        STR_SQL += " FROM  G_SCHMAST"
        STR_SQL += " WHERE FURI_KBN_S ='0'" '初振スケジュール
        STR_SQL += " AND FURI_DATE_S ='" & Str_Key_3 & "'"
        STR_SQL += " AND GAKKOU_CODE_S = '" & Str_Key_1 & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = True Then
            OBJ_DATAREADER.Read()
            If ConvNullToString(OBJ_DATAREADER.Item(0)) = "" Then
                lngSCH_CNT = 0
            Else
                lngSCH_CNT = CLng(OBJ_DATAREADER.Item(0))
            End If
        End If

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        STR_SQL = " SELECT "
        STR_SQL += " GAKUNEN_CODE_O ,sum(1) as KENSUU,"
        STR_SQL += "sum("
        For iLCount As Integer = 1 To 15
            STR_SQL += " KINGAKU" & Format(iLCount, "00") & "_O"
            If iLCount <> 15 Then
                STR_SQL += " + "
            End If
        Next iLCount
        STR_SQL += " ) as KINGAKU"

        '各費目ごとの集計件数、集計金額を取得
        For iLCount As Integer = 1 To 15
            STR_SQL += ",sum(decode(KINGAKU" & Format(iLCount, "00") & "_O,'0',0,1)) as SUMKENSUU" & Format(iLCount, "00")
            STR_SQL += ",sum(KINGAKU" & Format(iLCount, "00") & "_O) as SUMKINGAKU" & Format(iLCount, "00")
        Next iLCount

        STR_SQL += " FROM SEITOMASTVIEW "
        STR_SQL += " WHERE GAKKOU_CODE_O = '" & Str_Key_1 & "'"
        STR_SQL += " AND TUKI_NO_O = '" & Format(CInt(txtSMonth.Text), "00") & "'"
        STR_SQL += " AND FURIKAE_O = '0'"
        STR_SQL += " AND KAIYAKU_FLG_O <> '9'"
        STR_SQL += " AND ("
        For iLCount As Integer = 1 To 15 '費目1～15の内一つでも0円以上の項目がある場合
            STR_SQL += " KINGAKU" & Format(iLCount, "00") & "_O > 0"
            If iLCount <> 15 Then
                STR_SQL += " OR "
            End If
        Next iLCount
        STR_SQL += " )"

        If bGFlg = True Then '特定学年のみ指定の場合
            bLoopFlg = False
            STR_SQL += " AND ("
            For iLCount As Integer = 1 To 9
                If iTGakunen_Flag(iLCount) = 1 Then
                    If bLoopFlg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_O=" & iLCount
                    bLoopFlg = True
                End If
            Next iLCount
            STR_SQL += " )"
        End If
        STR_SQL += " group by  GAKUNEN_CODE_O"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = True Then
            bFlg = True
        End If

        Lng_Syofuri_Cnt = 0
        Lng_Syofuri_Kingaku = 0

        lRecCnt(0) = 0
        lRecCnt(1) = 0

        While (OBJ_DATAREADER.Read = True)

            If lngSCH_CNT <> 0 Then '初振スケジュールが0件でないとき=初振スケジュールをカウント 2006/12/27
                For iLCount As Integer = 1 To 15
                    Lng_Himoku_Cnt(iLCount - 1, OBJ_DATAREADER.Item("GAKUNEN_CODE_O")) = OBJ_DATAREADER.Item("SUMKENSUU" & Format(iLCount, "00"))
                    Lng_Himoku_Kingaku(iLCount - 1, OBJ_DATAREADER.Item("GAKUNEN_CODE_O")) = OBJ_DATAREADER.Item("SUMKINGAKU" & Format(iLCount, "00"))
                Next iLCount

                Lng_GSyofuri(0, OBJ_DATAREADER.Item("GAKUNEN_CODE_O")) = OBJ_DATAREADER.Item("KENSUU")
                Lng_GSyofuri(1, OBJ_DATAREADER.Item("GAKUNEN_CODE_O")) = OBJ_DATAREADER.Item("KINGAKU")

                lSyofuri(0, 0) += OBJ_DATAREADER.Item("KENSUU")
                lSyofuri(0, 1) += OBJ_DATAREADER.Item("KINGAKU")

                lRecCnt(0) += 1
            Else '初振スケジュール0件=再振のスケジュール
                For iLCount As Integer = 1 To 15
                    Lng_Himoku_Cnt(iLCount - 1, OBJ_DATAREADER.Item("GAKUNEN_CODE_O")) = 0
                    Lng_Himoku_Kingaku(iLCount - 1, OBJ_DATAREADER.Item("GAKUNEN_CODE_O")) = 0
                Next iLCount

                Lng_GSyofuri(0, OBJ_DATAREADER.Item("GAKUNEN_CODE_O")) = 0
                Lng_GSyofuri(1, OBJ_DATAREADER.Item("GAKUNEN_CODE_O")) = 0

                lSyofuri(0, 0) += 0
                lSyofuri(0, 1) += 0

                lRecCnt(0) += 0
                '2011/06/16 標準版修正 振替件数0件の場合は、金額チェックを行わない------------------START
                bFlg = False
                '2011/06/16 標準版修正 振替件数0件の場合は、金額チェックを行わない------------------END
            End If
        End While

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        If lngSCH_CNT <> 0 Then '初振スケジュールが0件でないとき=初振スケジュールをカウント 2006/12/27
            If lSyofuri(0, 0) = 0 Then
                Lng_Syofuri_Cnt = lSyofuri(1, 0)
            Else
                Lng_Syofuri_Cnt = lSyofuri(0, 0)
            End If
            If lSyofuri(0, 1) = 0 Then
                Lng_Syofuri_Kingaku = lSyofuri(1, 1)
            Else
                Lng_Syofuri_Kingaku = lSyofuri(0, 1)
            End If
            If lRecCnt(0) = 0 Then
                Int_Record_Cnt = lRecCnt(1)
            Else
                Int_Record_Cnt = lRecCnt(0)
            End If
        Else '初振スケジュール0件=再振のスケジュール
            Lng_Syofuri_Cnt = 0
            Lng_Syofuri_Kingaku = 0
            Int_Record_Cnt = 0
        End If


        '学年毎にスケジュールが分かれている可能性を踏まえて
        '各学年ごとに初振スケジュールの再振日を取得 2006/10/25
        Dim cnt As Integer
        Dim SAIFURI_GET_SQL As String = ""
        For cnt = 1 To UBound(Lng_GSyofuri, 2)
            SAIFURI_GET_SQL = " SELECT * "
            SAIFURI_GET_SQL += " FROM  G_SCHMAST"
            SAIFURI_GET_SQL += " WHERE FURI_KBN_S ='0'" '初振スケジュール
            SAIFURI_GET_SQL += " AND FURI_DATE_S ='" & Str_Key_3 & "'"
            SAIFURI_GET_SQL += " AND GAKKOU_CODE_S = '" & Str_Key_1 & "'"
            SAIFURI_GET_SQL += " AND GAKUNEN" & cnt & "_FLG_S  = '1'"

            If GFUNC_GET_SELECTSQL_ITEM(SAIFURI_GET_SQL, "SFURI_DATE_S") = "" Then
                strSAIFURI_DATE(cnt) = "00000000"
            Else
                strSAIFURI_DATE(cnt) = GFUNC_GET_SELECTSQL_ITEM(SAIFURI_GET_SQL, "SFURI_DATE_S")
            End If
        Next

        '2010/11/10　前年度分は持ち越さない（抽出しない）-----------------
        Dim strFURIDATE As String
        Dim strFURINEN As String
        Dim strFURITUKI As String
        Dim strPRE_FURINEN As String
        '2017/03/15 saitou 東春信金(RSV2標準) MODIFY 再振年度跨ぎ対応 ------------------------------------- START
        '前年度持ち越しの条件は請求対象年月で判断する
        strFURIDATE = Str_Key_2
        'strFURIDATE = Str_Key_3
        '2017/03/15 saitou 東春信金(RSV2標準) MODIFY ------------------------------------------------------ END
        strFURINEN = strFURIDATE.Substring(0, 4)
        strFURITUKI = strFURIDATE.Substring(4, 2)
        strPRE_FURINEN = CStr(CInt(strFURINEN) - 1)
        '-----------------------------------------------------------------

        '持越し分が存在するかチェック
        '持越し分がある場合、前回請求月を取得
        STR_SQL = " SELECT SEIKYU_TAISYOU_M "
        STR_SQL += " FROM G_MEIMAST "
        STR_SQL += " left join GAKMAST2 on"
        STR_SQL += " GAKKOU_CODE_T = GAKKOU_CODE_M"
        STR_SQL += " WHERE GAKKOU_CODE_T ='" & Str_Key_1 & "'"
        STR_SQL += " AND"
        If strFURI_KBN = "0" Then '初振の場合、再振有/持越有と再振無/持越有の時にカウント
            STR_SQL += " (SFURI_SYUBETU_T = '2' OR SFURI_SYUBETU_T = '3')"
        Else '再振の場合、再振無/持越無以外すべての時にカウント
            STR_SQL += " SFURI_SYUBETU_T <> '0'"
            '再振の場合は同じ請求対象月の初振から不能分をカウントする
            STR_SQL += " AND SEIKYU_TAISYOU_M = '" & Str_Key_2 & "'"
        End If
        '2017/04/11 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ START
        STR_SQL += " AND FURIKETU_CODE_M IN (" & SFuriCode & ")"
        'STR_SQL += " AND FURIKETU_CODE_M <> 0"
        '2017/04/11 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ END
        STR_SQL += " AND SAIFURI_SUMI_M = '0'"

        If bGFlg = True Then '学年指定の場合
            bLoopFlg = False
            STR_SQL += " AND ("
            For iLCount As Integer = 1 To 9
                If iTGakunen_Flag(iLCount) = 1 Then
                    If bLoopFlg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_M=" & iLCount
                    bLoopFlg = True
                End If
            Next iLCount
            STR_SQL += " )"
        End If
        '2010/11/10　前年度分は持ち越さない（抽出しない）------------------------------
        '2011/06/16 標準版修正 SQL修正------------------START
        If CInt(strFURITUKI) >= 4 And CInt(strFURITUKI) <= 12 Then                  '振替日の「月」が４～１２月だったらその年の４月以降のみ抽出する
            STR_SQL += " AND FURI_DATE_M >= '" & strFURINEN & "0401'  "
        ElseIf CInt(strFURITUKI) >= 1 And CInt(strFURITUKI) <= 3 Then               '振替日の「月」が１～３月だったらその前の年の４月以降のみ抽出する
            STR_SQL += " AND FURI_DATE_M >= '" & strPRE_FURINEN & "0401'  "
        End If
        'If CInt(strFURITUKI) >= 4 And CInt(strFURITUKI) <= 12 Then                  '振替日の「月」が４～１２月だったらその年の４月以降のみ抽出する
        '    STR_SQL += " AND FURI_DATE_M >= '" & strFURINEN & "0401""'  "
        'ElseIf CInt(strFURITUKI) >= 1 And CInt(strFURITUKI) <= 3 Then               '振替日の「月」が１～３月だったらその前の年の４月以降のみ抽出する
        '    STR_SQL += " AND FURI_DATE_M >= '" & strPRE_FURINEN & "0401""'  "
        'End If
        '2011/06/16 標準版修正 SQL修正------------------END
        '------------------------------------------------------------------------------
        STR_SQL += " GROUP BY SEIKYU_TAISYOU_M , GAKUNEN_CODE_M"
        STR_SQL += " ORDER BY SEIKYU_TAISYOU_M DESC" '入力振替日から一番近い請求対象月

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        Lng_Saifuri_Cnt = 0
        Lng_Saifuri_Kingaku = 0

        lRecCnt(0) = 0

        If OBJ_DATAREADER.HasRows = True Then
            OBJ_DATAREADER.Read()
            SfuriTuki = OBJ_DATAREADER.Item("SEIKYU_TAISYOU_M").ToString
        End If

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        '↓再振分としてカウントするもの
        '口座振替明細マスタから件数と金額を取得
        STR_SQL = " SELECT GAKUNEN_CODE_M , sum(1) as KENSUU , SUM(SEIKYU_KIN_M) as KINGAKU"
        For iLCount As Integer = 1 To 15
            STR_SQL += ",sum(decode(HIMOKU" & iLCount & "_KIN_M,'0',0,1)) as SUMKENSUU" & Format(iLCount, "00")
            STR_SQL += ",sum(HIMOKU" & iLCount & "_KIN_M) as SUMKINGAKU" & Format(iLCount, "00")
        Next iLCount
        STR_SQL += " FROM G_MEIMAST "
        STR_SQL += " LEFT JOIN GAKMAST2 ON "
        STR_SQL += " GAKKOU_CODE_T = GAKKOU_CODE_M"
        STR_SQL += " INNER JOIN SEITOMAST "
        STR_SQL += " ON G_MEIMAST.GAKKOU_CODE_M = SEITOMAST.GAKKOU_CODE_O "
        STR_SQL += " AND G_MEIMAST.TUUBAN_M = SEITOMAST.TUUBAN_O "
        STR_SQL += " AND G_MEIMAST.NENDO_M = SEITOMAST.NENDO_O "
        STR_SQL += " AND SUBSTR(G_MEIMAST.SEIKYU_TAISYOU_M, 5, 2) = SEITOMAST.TUKI_NO_O "
        STR_SQL += " AND SEITOMAST.FURIKAE_O <> '2' "
        STR_SQL += " WHERE GAKKOU_CODE_T ='" & Str_Key_1 & "'"
        STR_SQL += " AND SFURI_SYUBETU_T <> '0'"
        STR_SQL += " AND SEIKYU_TAISYOU_M ='" & SfuriTuki & "'"
        '2010/11/10 入金と臨時出金分を外す------------
        STR_SQL += " AND FURI_KBN_M <> '2'" '入金
        STR_SQL += " AND FURI_KBN_M <> '3'" '臨時出金
        '---------------------------------------------

        Select Case CInt(Str_SFURI_SYUBETU)
            Case 1
                '再振有、持越無
                STR_SQL += " AND FURI_KBN_M ='0'"
        End Select

        '2017/04/11 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ START
        STR_SQL += " AND FURIKETU_CODE_M IN (" & SFuriCode & ")"
        'STR_SQL += " AND FURIKETU_CODE_M <> 0"
        '2017/04/11 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ END
        STR_SQL += " AND SAIFURI_SUMI_M = '0'"
        If bGFlg = True Then
            bLoopFlg = False
            STR_SQL += " AND ("
            For iLCount As Integer = 1 To 9
                If iTGakunen_Flag(iLCount) = 1 Then
                    If bLoopFlg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_M=" & iLCount
                    bLoopFlg = True
                End If
            Next iLCount
            STR_SQL += " )"
        End If
        STR_SQL += " GROUP BY GAKUNEN_CODE_M"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = True Then
            bFlg = True
        End If

        While (OBJ_DATAREADER.Read = True)

            For iLCount As Integer = 1 To 15
                Lng_Himoku_Cnt(iLCount - 1, OBJ_DATAREADER.Item("GAKUNEN_CODE_M")) += OBJ_DATAREADER.Item("SUMKENSUU" & Format(iLCount, "00"))
                Lng_Himoku_Kingaku(iLCount - 1, OBJ_DATAREADER.Item("GAKUNEN_CODE_M")) += OBJ_DATAREADER.Item("SUMKINGAKU" & Format(iLCount, "00"))
            Next iLCount

            Lng_GSaifuri(0, OBJ_DATAREADER.Item("GAKUNEN_CODE_M")) = OBJ_DATAREADER.Item("KENSUU")
            Lng_GSaifuri(1, OBJ_DATAREADER.Item("GAKUNEN_CODE_M")) = OBJ_DATAREADER.Item("KINGAKU")

            Lng_Saifuri_Cnt += OBJ_DATAREADER.Item("KENSUU")
            Lng_Saifuri_Kingaku += OBJ_DATAREADER.Item("KINGAKU")

            lRecCnt(0) += 1
        End While

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        If Int_Record_Cnt = 0 Then
            Int_Record_Cnt = lRecCnt(0)
        End If

        Lng_Syukei_Cnt = Lng_Syofuri_Cnt + Lng_Saifuri_Cnt
        Lng_Syukei_Kingaku = Lng_Syofuri_Kingaku + Lng_Saifuri_Kingaku

        '初振
        lblSyofuriCnt.Text = Lng_Syofuri_Cnt
        lblSyofuriKingaku.Text = Format(Lng_Syofuri_Kingaku, "#,##0")

        '再振
        lblSaifuriCnt.Text = Lng_Saifuri_Cnt
        lblSaifuriKingaku.Text = Format(Lng_Saifuri_Kingaku, "#,##0")

        '集計
        lblSyukeiCnt.Text = Lng_Syukei_Cnt
        lblSyukeiKingaku.Text = Format(Lng_Syukei_Kingaku, "#,##0")

        If bFlg = False Then
            Call MessageBox.Show("指定したデータは存在しませんでした。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            Exit Function
        End If

        PFUNC_Set_Shukei_Kingaku = True

    End Function
    Private Function PFUNC_Get_Gakunen(ByVal pGakkou_Code As String, ByRef pSiyou_gakunen(,) As Integer, ByRef pTotal_Flg() As Integer) As Integer

        Dim iLoopCount As Integer
        Dim iMaxGakunen As Integer

        ReDim pTotal_Flg(9)
        ReDim pSiyou_gakunen(1, 9)

        PFUNC_Get_Gakunen = -1

        '選択された学校の指定振替日で抽出
        '(全スケジュール区分が対象)
        STR_SQL = " SELECT "
        STR_SQL += " SCH_KBN_S"
        For iLoopCount = 1 To 9
            STR_SQL += ", GAKUNEN" & iLoopCount & "_FLG_S"
            pSiyou_gakunen(0, iLoopCount) = 0
            pSiyou_gakunen(1, iLoopCount) = 0
        Next iLoopCount
        STR_SQL += ", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " LEFT JOIN GAKMAST2 ON "
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & pGakkou_Code & "'"
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
        'STR_SQL += " AND"
        'STR_SQL += " CHECK_FLG_S ='1'"
        STR_SQL += " AND TYUUDAN_FLG_S ='0'"
        'ｽｹｼﾞｭｰﾙ区分2(随時)は処理に含まない為
        STR_SQL += " AND SCH_KBN_S <> '2'"


        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER.Read)
            With OBJ_DATAREADER
                iMaxGakunen = CInt(.Item("SIYOU_GAKUNEN_T"))
                Select Case CInt(.Item("SCH_KBN_S"))
                    Case 0
                        For iLoopCount = 1 To iMaxGakunen
                            Select Case CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S"))
                                Case 1
                                    pSiyou_gakunen(0, iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                                    pTotal_Flg(iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                            End Select
                        Next iLoopCount
                    Case 1
                        For iLoopCount = 1 To iMaxGakunen
                            Select Case CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S"))
                                Case 1
                                    pSiyou_gakunen(1, iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                                    pTotal_Flg(iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                            End Select
                        Next iLoopCount
                End Select
            End With
        End While

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        '追加 2006/12/25
        '使用学年すべてにフラグがない場合は該当なしとする 2006/12/21
        For iLoopCount = 1 To iMaxGakunen
            Select Case pTotal_Flg(iLoopCount)
                Case 1
                    Exit For
                Case Else 'すべて指定がない場合
                    If iLoopCount = iMaxGakunen Then '追加 2006/12/21
                        PFUNC_Get_Gakunen = -2
                        Exit Function
                    End If
            End Select
        Next iLoopCount

        '使用学年全てに学年フラグがある場合は全学年対象として扱う
        '学年
        For iLoopCount = 1 To iMaxGakunen
            Select Case pTotal_Flg(iLoopCount)
                Case Is <> 1
                    PFUNC_Get_Gakunen = iMaxGakunen

                    Exit Function
            End Select
        Next iLoopCount

        PFUNC_Get_Gakunen = 0

    End Function

    Private Function PFUNC_Delete_Meisai() As Boolean

        PFUNC_Delete_Meisai = False

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        '---------------------
        '口座振替明細マスタ削除
        '----------------------
        STR_SQL = " DELETE  FROM G_MEIMAST"
        STR_SQL += " WHERE GAKKOU_CODE_M = '" & Str_Key_1 & "'"
        STR_SQL += " AND SEIKYU_TAISYOU_M = '" & Str_Key_2 & "'"
        STR_SQL += " AND FURI_DATE_M = '" & Str_Key_3 & "'"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then

            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        PFUNC_Delete_Meisai = True

    End Function
#End Region

End Class
