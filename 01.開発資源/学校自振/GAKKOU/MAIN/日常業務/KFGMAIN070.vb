Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGMAIN070

    Public STR_0700G_FLG As String
    Public strCHECK_FLG As String = ""
    Public strDATA_FLG As String = ""
    Private Str_GAK_INFO(3) As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAIN070", "生徒明細入力画面")
    Private Const msgTitle As String = "生徒明細入力画面(KFGMAIN070)"
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

#Region " Form Load "
    Private Sub KFGMAIN070_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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

            txtGAKKOU_CODE.Focus()
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
            '実行ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")
            Dim KFGMAIN071 As New GAKKOU.KFGMAIN071

            '入力値チェック
            If PFUNC_Nyuryoku_Check(0) = False Then
                Exit Sub
            End If

            '画面間仕様値を変数に格納
            Call PSUB_Keep_Value()

            '生徒明細入力画面に遷移
            KFGMAIN071.ShowDialog(Me)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.ToString)
        Finally
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnCreate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreate.Click
        Try
            '実行ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(データ作成)開始", "成功", "")
            Dim KFGMAIN072 As New GAKKOU.KFGMAIN072

            '入力値チェック
            If PFUNC_Nyuryoku_Check(1) = False Then
                Exit Sub
            End If

            '画面間仕様値を変数に格納
            Call PSUB_Keep_Value()

            '生徒明細入力画面に遷移
            KFGMAIN072.ShowDialog(Me)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(データ作成)", "失敗", ex.ToString)
        Finally
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(データ作成)終了", "成功", "")
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

#Region " Private Sub "
    Private Sub PSUB_Keep_Value()

        '学校コードの設定
        STR_生徒明細学校コード = Trim(txtGAKKOU_CODE.Text)

        STR_生徒明細学校名 = Trim(lab学校名.Text)

        '振替日の設定
        STR_生徒明細振替日 = Mid(cmbFURIKAEBI.Text, 1, 4) & Mid(cmbFURIKAEBI.Text, 6, 2) & Mid(cmbFURIKAEBI.Text, 9, 2)

        '入出金区分の設定
        Select Case Mid(cmbFURIKAEBI.Text, 12, 2)
            Case "入金"
                STR_生徒明細入出区分 = 2
            Case "出金"
                STR_生徒明細入出区分 = 3
        End Select

        '初期値の設定
        If txtSYOKICHI.Text.Trim = "" Then
            STR_生徒明細初期値 = ""
        Else
            STR_生徒明細初期値 = CStr(CDbl(Trim(txtSYOKICHI.Text)))
        End If

        'ソート順の設定
        Select Case True
            Case rdoButton1.Checked
                '学年・クラス・生徒番号順
                STR_生徒明細ソート順 = 1
            Case rdoButton2.Checked
                '入学年度・通番順
                STR_生徒明細ソート順 = 2
            Case rdoButton3.Checked
                'あいうえお順
                STR_生徒明細ソート順 = 3
        End Select

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        With CType(sender, TextBox)
            If .Text.Trim <> "" Then

                '学校名の取得
                If PFUNC_Get_GakkouName() = False Then
                    Exit Sub
                Else
                    '振替日コンボボックスの設定
                    If PFUNC_Set_cmbFURIKAEBI() = False Then
                        Exit Sub
                    End If
                End If

            End If
        End With

    End Sub
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

        'COMBOBOX選択時学校名,学校コード設定
        lab学校名.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex)

        '学年テキストボックスにFOCUS
        txtGAKKOU_CODE.Focus()
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " Private Function "
    Private Function PFUNC_Nyuryoku_Check(ByVal pIndex As Integer) As Boolean

        PFUNC_Nyuryoku_Check = False

        Dim SQL As New System.Text.StringBuilder(128)

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()

            Exit Function
        Else
            '学校マスタ存在チェック
            SQL.Length = 0
            SQL.Append(" SELECT * FROM GAKMAST1 ")
            SQL.Append(" WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If GFUNC_ISEXIST(SQL.ToString) = False Then

                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()

                Exit Function
            End If

            SQL.Length = 0
            SQL.Append(" SELECT * FROM GAKMAST2 ")
            SQL.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
                Exit Function
            End If

            With OBJ_DATAREADER
                .Read()

                STR_生徒明細印刷区分 = .Item("MEISAI_KBN_T")
            End With

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If
        End If

        '振替日
        If Trim(cmbFURIKAEBI.Text) = "" Then
            MessageBox.Show(G_MSG0010W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cmbFURIKAEBI.Focus()

            Exit Function
        End If

        SQL.Length = 0
        SQL.Append(" SELECT CHECK_FLG_S,DATA_FLG_S FROM G_SCHMAST ")
        SQL.Append(" WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text & "'")
        SQL.Append(" AND FURI_DATE_S ='" & Mid(cmbFURIKAEBI.Text, 1, 4) & Mid(cmbFURIKAEBI.Text, 6, 2) & Mid(cmbFURIKAEBI.Text, 9, 2) & "'")
        SQL.Append(" AND SCH_KBN_S ='2'")
        SQL.Append(" AND ENTRI_FLG_S ='1'")
        '2006/10/20 入出金区分を検索条件に追加
        Select Case Mid(cmbFURIKAEBI.Text, 12, 2)
            Case "入金"
                SQL.Append(" AND FURI_KBN_S ='2'")
            Case "出金"
                SQL.Append(" AND FURI_KBN_S ='3'")
        End Select

        STR_0700G_FLG = GFUNC_GET_SELECTSQL_ITEM(SQL.ToString, "CHECK_FLG_S")
        strCHECK_FLG = GFUNC_GET_SELECTSQL_ITEM(SQL.ToString, "CHECK_FLG_S")
        strDATA_FLG = GFUNC_GET_SELECTSQL_ITEM(SQL.ToString, "DATA_FLG_S")

        Select Case pIndex
            Case 0 '明細入力
            Case 1 'データ作成
                If strCHECK_FLG = "0" Then
                    MessageBox.Show(G_MSG0011W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    btnAction.Focus()
                    Exit Function
                End If
        End Select

        If strDATA_FLG = "1" Then
            MessageBox.Show(G_MSG0012W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            btnEnd.Focus()
            Exit Function
        End If

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_Get_GakkouName() As Boolean

        Dim sGakkou_Name As String

        '学校名の設定
        PFUNC_Get_GakkouName = False

        Dim SQL As New System.Text.StringBuilder(128)
        SQL.Append(" SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
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
        STR_生徒明細学校名 = sGakkou_Name

        PFUNC_Get_GakkouName = True

    End Function
    Private Function PFUNC_Set_cmbFURIKAEBI() As Boolean

        '振替日コンボの設定
        Dim str振替日 As String

        PFUNC_Set_cmbFURIKAEBI = False

        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '振替日コンボボックスのクリア
            cmbFURIKAEBI.Items.Clear()

            'スケジュールマスタの検索、キーは学校コード、スケジュール区分、明細作成フラグ
            Dim SQL As New System.Text.StringBuilder(128)
            SQL.Append(" SELECT * FROM G_SCHMAST ")
            SQL.Append(" WHERE GAKKOU_CODE_S ='" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
            SQL.Append(" AND SCH_KBN_S ='2'")
            SQL.Append(" AND ENTRI_FLG_S ='1'")
            SQL.Append(" ORDER BY FURI_DATE_S")

            If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
                Exit Function
            End If

            If OBJ_DATAREADER.HasRows = True Then

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
            Else
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If
            End If
        End If

        PFUNC_Set_cmbFURIKAEBI = True

    End Function
#End Region

End Class
