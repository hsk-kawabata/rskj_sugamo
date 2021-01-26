Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGMAIN060
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 生徒明細作成
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Private Str_Syori_Date(1) As String
    Private Str_Gakunen_Flg() As String
    Private Bln_Gakunen_Flg As Boolean

    Private Str_Gakkou_Code As String
    Private Str_Syori_Nentuki As String
    Private Str_FurikaeDate As String
    Private Str_Nyusyutu_Kbn As String

    Private Str_GAK_INFO(3) As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAIN060", "生徒明細作成画面")
    Private Const msgTitle As String = "生徒明細作成画面(KFGMAIN060)"
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

#End Region

#Region " Form Load "
    Private Sub KFGMAIN060_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            txtTAISYONEN.Text = Format(Now, "yyyy")
            txtTAISYOUTUKI.Text = Format(Now, "MM")
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            '作成ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)開始", "成功", "")
            '入力値チェック
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            If MessageBox.Show(String.Format(MSG0015I, "生徒明細作成"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = Str_FurikaeDate

            '指定学年の取得
            If PFUNC_Get_Gakunen() = False Then
                Exit Sub
            End If

            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Sub
            End If

            '既存データの削除
            If PFUNC_Delete_ENTMAST() = False Then
                Exit Sub
            End If

            'トランザクション終了（ＣＯＭＭＩＴ）
            If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                Exit Sub
            End If

            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Sub
            End If

            'エントリデータの作成
            If PFUNC_Insert_ENTMAST() = False Then
                Exit Sub
            End If

            'スケジュールマスタの明細作成済フラグを「１」（作成済）にする
            If PFUNC_Update_Schedule() = False Then
                Exit Sub
            End If

            'トランザクション終了（ＣＯＭＭＩＴ）
            If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                Exit Sub
            End If

            '完了メッセージ
            MessageBox.Show(String.Format(MSG0016I, "生徒明細作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            txtGAKKOU_CODE.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "失敗", ex.ToString)
        Finally
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)終了", "成功", "")
        End Try

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '終了ボタン
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

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_Get_GakkouName() As Boolean

        Dim sGakkou_Name As String

        '学校名の設定
        PFUNC_Get_GakkouName = False

        Dim SQL As New System.Text.StringBuilder(128)

        SQL.Append(" SELECT GAKKOU_NNAME_G ")
        SQL.Append(" FROM GAKMAST1 ")
        SQL.Append(" WHERE GAKKOU_CODE_G  ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

        sGakkou_Name = GFUNC_GET_SELECTSQL_ITEM(SQL.ToString, "GAKKOU_NNAME_G")

        If Trim(sGakkou_Name) = "" Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            lab学校名.Text = ""
            txtGAKKOU_CODE.Text = "" '2006/10/24　追加
            txtGAKKOU_CODE.Focus()

            Exit Function
        End If

        lab学校名.Text = sGakkou_Name

        PFUNC_Get_GakkouName = True

    End Function
    Private Function PFUNC_Set_cmbFURIKAEBI() As Boolean

        '振替日コンボの設定
        Dim str振替日 As String

        PFUNC_Set_cmbFURIKAEBI = False

        If Trim(txtGAKKOU_CODE.Text) <> "" And _
           Trim(txtTAISYONEN.Text) <> "" And _
           Trim(txtTAISYOUTUKI.Text) <> "" Then

            '振替日コンボボックスのクリア
            cmbFURIKAEBI.Items.Clear()

            Dim SQL As New System.Text.StringBuilder(128)
            'スケジュールマスタの検索、キーは学校コード、スケジュール区分、明細作成フラグ
            SQL.Append(" SELECT * FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
            '2017/05/15 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
            'ここでは前ゼロ埋めになっていないので補完する
            SQL.Append(" AND NENGETUDO_S ='" & txtTAISYONEN.Text.PadLeft(4, "0"c) & txtTAISYOUTUKI.Text.PadLeft(2, "0"c) & "'")
            'SQL.Append(" AND NENGETUDO_S ='" & Trim(txtTAISYONEN.Text) & Trim(txtTAISYOUTUKI.Text) & "'")
            '2017/05/15 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
            SQL.Append(" AND SCH_KBN_S ='2'")
            SQL.Append(" ORDER BY FURI_DATE_S")

            If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
                Exit Function
            End If

            If OBJ_DATAREADER.HasRows = False Then
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If

                Exit Function
            End If

            While (OBJ_DATAREADER.Read = True)
                With OBJ_DATAREADER
                    '振替日の編集
                    str振替日 = Mid(.Item("FURI_DATE_S"), 1, 4) & "/" & Mid(.Item("FURI_DATE_S"), 5, 2) & "/" & Mid(.Item("FURI_DATE_S"), 7, 2)

                    '入金、出金の編集
                    Select Case OBJ_DATAREADER.Item("FURI_KBN_S")
                        Case "2"
                            str振替日 += " 入金"
                        Case "3"
                            str振替日 += " 出金"
                    End Select
                    '振替日コンボボックスへ追加
                    cmbFURIKAEBI.Items.Add(str振替日)
                End With
            End While

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            'コンボ先頭の設定
            cmbFURIKAEBI.SelectedIndex = 0
        End If

        PFUNC_Set_cmbFURIKAEBI = True

    End Function
    Private Function PFUNC_Update_Schedule() As Boolean

        'スケジュールマスタの更新

        PFUNC_Update_Schedule = False
        Dim SQL As New System.Text.StringBuilder(128)
        SQL.Append(" UPDATE  G_SCHMAST SET ")
        SQL.Append(" ENTRI_FLG_S ='1'")
        SQL.Append(" ,CHECK_FLG_S ='0'") 'チェックフラグ初期化 2007/02/10
        SQL.Append(",TIME_STAMP_S ='" & Str_Syori_Date(1) & "'")
        SQL.Append(" WHERE GAKKOU_CODE_S ='" & Str_Gakkou_Code & "'")
        SQL.Append(" AND NENGETUDO_S ='" & Str_Syori_Nentuki & "'")
        SQL.Append(" AND SCH_KBN_S ='2'")
        SQL.Append(" AND FURI_KBN_S ='" & Str_Nyusyutu_Kbn & "'")
        SQL.Append(" AND FURI_DATE_S ='" & Str_FurikaeDate & "'")

        If GFUNC_EXECUTESQL_TRANS(SQL.ToString, 1) = False Then
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        PFUNC_Update_Schedule = True

    End Function
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Exit Function
        Else
            '学校マスタ存在チェック
            Dim SQL As String = " SELECT * FROM GAKMAST1"
            SQL &= " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_ISEXIST(SQL) = False Then

                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()

                Exit Function
            End If

        End If

        Str_Gakkou_Code = Trim(txtGAKKOU_CODE.Text)

        '対象年
        If Trim(txtTAISYONEN.Text) = "" Then
            MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTAISYONEN.Focus()
            Exit Function
        Else
            '数値チェック 2006/10/10
            If IsNumeric(txtTAISYONEN.Text) = False Then
                MessageBox.Show(MSG0019W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTAISYONEN.Focus()
                Exit Function
                'Else
                '    If txtTAISYONEN.Text.Trim.Length < 4 Then
                '        MessageBox.Show("処理対象年の入力が不正です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '        txtTAISYONEN.Focus()
                '        Exit Function
                '    End If

            End If

        End If

        '対象月
        If Trim(txtTAISYOUTUKI.Text) = "" Then
            Call MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtTAISYOUTUKI.Focus()
            Exit Function
        Else

            '数値チェック 2006/10/10
            If IsNumeric(txtTAISYOUTUKI.Text) = False Then
                Call MessageBox.Show(MSG0021W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTAISYOUTUKI.Focus()
                Exit Function
            End If

            Select Case CInt(txtTAISYOUTUKI.Text)
                Case 1 To 12
                Case Else
                    Call MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTAISYOUTUKI.Focus()
                    Exit Function
            End Select
        End If

        Str_Syori_Nentuki = Trim(txtTAISYONEN.Text) & Trim(txtTAISYOUTUKI.Text)
        Str_FurikaeDate = Mid(cmbFURIKAEBI.Text, 1, 4) & Mid(cmbFURIKAEBI.Text, 6, 2) & Mid(cmbFURIKAEBI.Text, 9, 2)
        Select Case Mid(cmbFURIKAEBI.Text, 12, 2)
            Case "入金"
                Str_Nyusyutu_Kbn = "2"
            Case "出金"
                Str_Nyusyutu_Kbn = "3"
        End Select

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_Get_Gakunen() As Boolean

        '年間スケジュールから随時処理のものを検索する
        Dim iLoopCount As Integer
        Dim iGakunenCount As Integer

        PFUNC_Get_Gakunen = False

        Dim SQL As New System.Text.StringBuilder(128)
        SQL.Append(" SELECT ")
        SQL.Append(" G_SCHMAST.* ")
        SQL.Append(", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T")
        SQL.Append(" FROM G_SCHMAST , GAKMAST2")
        SQL.Append(" WHERE GAKKOU_CODE_S = GAKKOU_CODE_T")
        SQL.Append(" AND GAKKOU_CODE_S ='" & Str_Gakkou_Code & "'")
        SQL.Append(" AND FURI_DATE_S ='" & Str_FurikaeDate & "'")
        SQL.Append(" AND SCH_KBN_S = '2'")
        SQL.Append(" AND FURI_KBN_S ='" & Str_Nyusyutu_Kbn & "'")

        If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
            Exit Function
        End If

        'スケジュールマスタ存在チェック
        If OBJ_DATAREADER.HasRows = False Then
            Call MessageBox.Show(G_MSG0008W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Call GFUNC_SELECT_SQL2("", 1)
            Exit Function
        End If

        OBJ_DATAREADER.Read()

        '生徒明細データ作成フラグチェック 2007/02/10
        If OBJ_DATAREADER.Item("DATA_FLG_S") = "1" Then
            Call MessageBox.Show(G_MSG0012W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Call GFUNC_SELECT_SQL2("", 1)
            txtGAKKOU_CODE.Focus()
            Exit Function
        End If
        '生徒明細データ合計チェックフラグチェック 2007/02/10
        If OBJ_DATAREADER.Item("CHECK_FLG_S") = "1" Then
            If MessageBox.Show(G_MSG0003I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If
                txtGAKKOU_CODE.Focus()
                Exit Function
            End If
        End If

        If OBJ_DATAREADER.Item("ENTRI_FLG_S") = "1" Then
            If MessageBox.Show(G_MSG0004I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If

                Exit Function
            End If
        End If

        iGakunenCount = 1

        With OBJ_DATAREADER

            For iLoopCount = 1 To CInt(.Item("SIYOU_GAKUNEN_T"))
                If .Item("GAKUNEN" & iLoopCount & "_FLG_S") = "1" Then
                    ReDim Preserve Str_Gakunen_Flg(iGakunenCount)
                    Str_Gakunen_Flg(iGakunenCount) = iLoopCount
                    iGakunenCount += 1
                End If
            Next

            '使用学年数が学校マスタで設定されている使用学年数と一致する場合は
            '全学年が抽出対象
            If CInt(.Item("SIYOU_GAKUNEN_T")) = UBound(Str_Gakunen_Flg) Then
                Bln_Gakunen_Flg = False
            Else
                Bln_Gakunen_Flg = True
            End If
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Gakunen = True

    End Function
    Private Function PFUNC_Delete_ENTMAST() As Boolean
        '削除処理、キーは学校コード、対象年月、学年
        '振替日追加 2006/10/16

        Dim iLoopCount As Integer

        PFUNC_Delete_ENTMAST = False

        Dim SQL As String = ""

        Select Case Str_Nyusyutu_Kbn
            Case "2"
                '入金
                SQL = " DELETE  FROM G_ENTMAST1"
            Case "3"
                '出金
                SQL = " DELETE  FROM G_ENTMAST2"
        End Select
        SQL += " WHERE GAKKOU_CODE_E ='" & Str_Gakkou_Code & "'"
        SQL += " AND SYORI_NENGETU_E ='" & Str_Syori_Nentuki & "'"
        SQL += " AND FURI_DATE_E ='" & Str_FurikaeDate & "'" '追加 2006/10/16

        '学年指定が存在する場合は、指定されている学年のみを削除する
        If Bln_Gakunen_Flg = True Then
            SQL += " and("
            For iLoopCount = 1 To UBound(Str_Gakunen_Flg)
                If iLoopCount = 1 Then
                    SQL += " GAKUNEN_CODE_E =" & Str_Gakunen_Flg(iLoopCount)
                Else
                    SQL += " OR GAKUNEN_CODE_E =" & Str_Gakunen_Flg(iLoopCount)
                End If
            Next iLoopCount
            SQL += ")"
        End If

        If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If

        PFUNC_Delete_ENTMAST = True

    End Function
    Private Function PFUNC_Insert_ENTMAST() As Boolean

        Dim iLoopCount As Integer

        PFUNC_Insert_ENTMAST = False

        Dim SQL As String = ""

        Select Case Str_Nyusyutu_Kbn
            Case "2"
                '入金
                SQL = " INSERT INTO G_ENTMAST1"
            Case "3"
                '出金
                SQL = " INSERT INTO G_ENTMAST2"
        End Select
        SQL += " SELECT "
        SQL += " GAKKOU_CODE_O"
        SQL += ",'" & Str_Syori_Nentuki & "'"
        SQL += ",'" & Str_FurikaeDate & "'"
        SQL += ", rownum"
        SQL += ", NENDO_O"
        SQL += ", TUUBAN_O"
        SQL += ", GAKUNEN_CODE_O"
        SQL += ", CLASS_CODE_O"
        SQL += ", SEITO_NO_O"
        SQL += ", SEITO_KNAME_O"
        SQL += ", SEITO_NNAME_O"
        SQL += ", SEIBETU_O"
        SQL += ", KAMOKU_O"
        SQL += ", KOUZA_O"
        SQL += ", concat('000' , SEITO_NO_O)"
        SQL += ", MEIGI_KNAME_O"
        SQL += ", MEIGI_NNAME_O"
        SQL += ", 0"
        SQL += ", 0"
        SQL += ", 0"
        SQL += ", '0'"
        SQL += ", TKIN_NO_O"
        SQL += ", TSIT_NO_O"
        SQL += ", '0'"
        SQL += ", rownum"
        SQL += ", 0"
        SQL += ",'" & Str_Syori_Date(0) & "'"
        SQL += ", '00000000'"
        SQL += ", '" & Space(10) & "'"
        SQL += ", '" & Space(10) & "'"
        SQL += ", '" & Space(10) & "'"
        SQL += ", '" & Space(10) & "'"
        SQL += ", '" & Space(10) & "'"
        SQL += " FROM SEITOMAST"
        SQL += " WHERE"
        SQL += " GAKKOU_CODE_O ='" & Str_Gakkou_Code & "'"
        '１生徒に複数レコードが存在するための対処
        '↓
        SQL += " AND"
        SQL += " TUKI_NO_O = '04'"
        '↑
        SQL += " AND"
        SQL += " FURIKAE_O = '0'"
        SQL += " AND"
        SQL += " KAIYAKU_FLG_O <> '9'"
        '学年指定が存在する場合は、指定されている学年のみを削除する
        If Bln_Gakunen_Flg = True Then
            SQL += " and("
            For iLoopCount = 1 To UBound(Str_Gakunen_Flg)
                If iLoopCount = 1 Then
                    SQL += " GAKUNEN_CODE_O =" & Str_Gakunen_Flg(iLoopCount)
                Else
                    SQL += " OR GAKUNEN_CODE_O =" & Str_Gakunen_Flg(iLoopCount)
                End If
            Next iLoopCount
            SQL += ")"
        End If

        If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
            MessageBox.Show(String.Format(MSG0002E, "登録"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If

        PFUNC_Insert_ENTMAST = True

    End Function

#End Region

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
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
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        'COMBOBOX選択時学校名,学校コード設定
        lab学校名.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex)

        '学年テキストボックスにFOCUS
        txtGAKKOU_CODE.Focus()
    End Sub
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    txtGAKKOU_CODE.Validating, _
    txtTAISYOUTUKI.Validating

        With CType(sender, TextBox)
            If .Text.Trim <> "" Then
                Select Case .Name
                    Case "txtGAKKOU_CODE"
                        If IsNumeric(txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c)) = False Then
                            Call MessageBox.Show(G_MSG0013W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtTAISYONEN.Focus()
                            Exit Sub
                        End If

                        '学校名の取得
                        If PFUNC_Get_GakkouName() = False Then
                            Exit Sub
                        Else
                            If PFUNC_Set_cmbFURIKAEBI() = False Then
                                Exit Sub
                            End If
                        End If
                    Case "txtTAISYOUTUKI"
                        '振替日コンボボックスの設定
                        If PFUNC_Set_cmbFURIKAEBI() = False Then
                            Exit Sub
                        End If
                End Select
            Else
                If .Name = "txtGAKKOU_CODE" Then
                    lab学校名.Text = ""
                End If
            End If
        End With
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
