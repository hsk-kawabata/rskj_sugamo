Option Explicit On 
Option Strict Off

Imports CASTCommon

Public Class KFGMAIN071

#Region " 共通変数定義 "
    Private INTCNT01 As Integer
    Private INTCNT02 As Integer
    Private LNG合計金額 As Long

    Private Str_Syori_Date(1) As String
    Private Str_Ginko(3) As String

    Dim flg As Boolean = False
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAIN071", "生徒明細入力画面")
    Private Const msgTitle As String = "生徒明細入力画面(KFGMAIN071)"
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
#End Region

#Region " Form Load "
    Private Sub KFGMAIN071_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            LW.ToriCode = STR_生徒明細学校コード
            LW.FuriDate = STR_生徒明細振替日
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '学校コード
            lab学校コード.Text = STR_生徒明細学校コード

            '学校名の設定
            lab学校名.Text = STR_生徒明細学校名

            lab振替日.Text = Mid(STR_生徒明細振替日, 1, 4) & "/" & Mid(STR_生徒明細振替日, 5, 2) & "/" & Mid(STR_生徒明細振替日, 7, 2)

            Select Case STR_生徒明細入出区分
                Case "2"
                    lab入出金区分.Text = "入金"
                Case "3"
                    lab入出金区分.Text = "出金"
            End Select

            If Trim(STR_生徒明細初期値) <> "" Then
                lab初期値.Text = Format(CDbl(STR_生徒明細初期値), "#,##0")
            Else
                lab初期値.Text = ""
            End If

            'エントリマスタの読込み
            If PFUNC_Spread_Set() = False Then
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnGOKEI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGOKEI.Click

        Try
            '合計ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(合計)開始", "成功", "")

            Str_Syori_Date(0) = Format(Now, "yyyyMMdd")
            Str_Syori_Date(1) = Format(Now, "yyyyMMddHHmmss")

            '入力値チェック
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            If MessageBox.Show(String.Format(MSG0015I, "合計"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If

            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Sub
            End If

            'エントリマスタの更新
            If PFUNC_Update_ENTMAST() = False Then
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Exit Sub
                End If
                Exit Sub
            End If

            'スケジュールマスタの更新
            If PFUNC_Update_Schedule() = False Then
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Exit Sub
                End If
                Exit Sub
            End If

            If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                Exit Sub
            End If

            '完了メッセージ
            MessageBox.Show(String.Format(MSG0016I, "合計"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(合計)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(合計)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnTOUROKU_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTOUROKU.Click

        Try
            '登録ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")

            If MessageBox.Show(String.Format(MSG0015I, "登録"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If

            'エントリマスタの更新
            If PFUNC_Update_ENTMAST() = False Then
                Exit Sub
            End If

            '完了メッセージ
            MessageBox.Show(String.Format(MSG0016I, "登録"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")
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
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        Dim ret As Boolean = False

        Try
            If Trim(txt入力合計金額.Text) = "" Then
                MessageBox.Show(MSG0249W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txt入力合計金額.Focus()
                Exit Try
            End If

            LNG合計金額 = 0
            For INTCNT02 = 0 To INTCNT01 - 1
                With DataGridView
                    If Trim(.Rows(INTCNT02).Cells(17).Value) <> "" Then
                        LNG合計金額 = LNG合計金額 + CDec(.Rows(INTCNT02).Cells(17).Value)
                    End If
                End With
            Next INTCNT02

            If CDbl(txt入力合計金額.Text) <> CDbl(LNG合計金額) Then
                MessageBox.Show(G_MSG0009W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                txt入力合計金額.Focus()
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            ret = False
        End Try

        Return ret

    End Function
    Private Function PFUNC_Spread_Set() As Boolean

        Dim iNo As Integer
        Dim MainDB As New MyOracle

        PFUNC_Spread_Set = False

        Try

            Select Case STR_生徒明細入出区分
                Case "2"
                    iNo = 1
                Case "3"
                    iNo = 2
            End Select

            'エントリマスタを検索する


            'スプレッドヘッダの編集
            Select Case iNo
                Case 1
                    DataGridView.Columns(17).HeaderText = "入金金額"
                Case 2
                    DataGridView.Columns(17).HeaderText = "出金金額"
            End Select

            'エントリマスタ検索のSQL文作成
            STR_SQL = " SELECT * FROM "
            STR_SQL += " G_ENTMAST" & iNo
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_E ='" & STR_生徒明細学校コード & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_E ='" & STR_生徒明細振替日 & "'"
            STR_SQL += " ORDER BY "
            Select Case STR_生徒明細ソート順
                Case "1"
                    '学年、クラス、生徒番号
                    STR_SQL += " GAKUNEN_CODE_E ASC, CLASS_CODE_E ASC, SEITO_NO_E ASC"
                Case "2"
                    '入学年度、通番
                    STR_SQL += " GAKUNEN_CODE_E ASC, NENDO_E ASC, TUUBAN_E ASC"
                Case "3"
                    '生徒名のアイウエオ順
                    STR_SQL += " GAKUNEN_CODE_E ASC, SEITO_KNAME_E ASC"
            End Select

            'エントリマスタ存在チェック
            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(G_MSG0007W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Function
            End If

            INTCNT01 = 0

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Function
            End If

            With DataGridView
                Select Case STR_生徒明細ソート順
                    Case "1"
                        '学年、クラス、生徒番号
                        .Columns(0).Visible = False
                        .Columns(1).Visible = False
                        .Columns(2).Visible = True
                        .Columns(3).Visible = True
                        .Columns(4).Visible = True
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = False
                        .Columns(18).Visible = False

                    Case "2"
                        '入学年度、通番
                        .Columns(0).Visible = True
                        .Columns(1).Visible = True
                        .Columns(2).Visible = False
                        .Columns(3).Visible = False
                        .Columns(4).Visible = False
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = False
                        .Columns(18).Visible = False
                    Case "3"
                        '生徒名のアイウエオ順
                        .Columns(0).Visible = False
                        .Columns(1).Visible = False
                        .Columns(2).Visible = False
                        .Columns(3).Visible = False
                        .Columns(4).Visible = False
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = True
                        .Columns(18).Visible = False
                End Select
            End With

            While (OBJ_DATAREADER.Read = True)
                With DataGridView

                    '追加行
                    Dim RowItem As New DataGridViewRow
                    RowItem.CreateCells(DataGridView)

                    '入学年度
                    RowItem.Cells(0).Value = OBJ_DATAREADER.Item("NENDO_E")
                    '通番
                    RowItem.Cells(1).Value = OBJ_DATAREADER.Item("TUUBAN_E")
                    '学年
                    RowItem.Cells(2).Value = OBJ_DATAREADER.Item("GAKUNEN_CODE_E")
                    'クラス
                    RowItem.Cells(3).Value = OBJ_DATAREADER.Item("CLASS_CODE_E")
                    '生徒番号
                    RowItem.Cells(4).Value = OBJ_DATAREADER.Item("SEITO_NO_E")
                    '生徒名カナ
                    RowItem.Cells(5).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                    '生徒名漢字 2007/02/10
                    If IsDBNull(OBJ_DATAREADER.Item("SEITO_NNAME_E")) = True Then
                        RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                    Else
                        If Trim(OBJ_DATAREADER.Item("SEITO_NNAME_E")) = "" Then 'スペースの場合カナ表示
                            RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                        Else
                            RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_NNAME_E")
                        End If
                    End If

                    '表示用生徒名
                    If RowItem.Cells(6).Value = "" Then
                        RowItem.Cells(7).Value = RowItem.Cells(5).Value
                    Else
                        RowItem.Cells(7).Value = RowItem.Cells(6).Value
                    End If

                    Call PSUB_GET_GINKONAME(OBJ_DATAREADER.Item("TKIN_NO_E"), OBJ_DATAREADER.Item("TSIT_NO_E"), MainDB)

                    '金融機関コードの格納
                    RowItem.Cells(8).Value = OBJ_DATAREADER.Item("TKIN_NO_E")
                    RowItem.Cells(9).Value = Str_Ginko(1)

                    '支店コードの格納
                    RowItem.Cells(10).Value = OBJ_DATAREADER.Item("TSIT_NO_E")
                    RowItem.Cells(11).Value = Str_Ginko(3)

                    '科目コードの格納（２桁から１桁に変換）
                    Select Case OBJ_DATAREADER.Item("KAMOKU_E")
                        Case "01"
                            RowItem.Cells(12).Value = "2"
                        Case "02"
                            RowItem.Cells(12).Value = "1"
                        Case "05"
                            RowItem.Cells(12).Value = "3"
                        Case "37"
                            RowItem.Cells(12).Value = "4"
                        Case "04"
                            RowItem.Cells(12).Value = "9"
                        Case Else
                            RowItem.Cells(12).Value = "2"
                    End Select
                    '科目名の変換、格納
                    Select Case OBJ_DATAREADER.Item("KAMOKU_E")
                        '2011/06/16 標準版修正 科目が01の場合当座 ------------------START
                        'Case "02"
                        Case "01"
                            '2011/06/16 標準版修正 科目が01の場合当座 ------------------END
                            '当座
                            RowItem.Cells(13).Value = "当"
                        Case "03"
                            '
                            RowItem.Cells(13).Value = "納"
                        Case "04"
                            '
                            RowItem.Cells(13).Value = "職"
                        Case Else
                            '普通
                            'その他
                            RowItem.Cells(13).Value = "普"
                    End Select
                    '口座番号の格納
                    RowItem.Cells(14).Value = OBJ_DATAREADER.Item("KOUZA_E")

                    '契約者名の格納
                    '2006/12/08　データベースにはスペースが入っているため、IsDBNullでは空白判定できない
                    If IsDBNull(OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")) = True Then
                        RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_KNAME_E")
                    Else
                        If Trim(OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")) = "" Then 'スペースの場合カナ表示
                            RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_KNAME_E")
                        Else
                            RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")
                        End If
                    End If

                    '契約者番号の格納
                    RowItem.Cells(16).Value = OBJ_DATAREADER.Item("KEIYAKU_NO_E")

                    '金額の格納
                    RowItem.Cells(17).ReadOnly = False

                    If Trim(STR_生徒明細初期値) <> "" Then
                        RowItem.Cells(17).Value = Format(CDbl(STR_生徒明細初期値), "#,##0")
                    Else
                        RowItem.Cells(17).Value = Format(CDbl(OBJ_DATAREADER.Item("FURIKIN_E")), "#,##0")
                    End If

                    '手数料の格納
                    RowItem.Cells(18).Value = Format(CDbl(OBJ_DATAREADER.Item("TESUU_E")), "#,##0")

                    For Cnt As Integer = 0 To RowItem.Cells.Count - 1
                        If Cnt <> 17 Then
                            RowItem.Cells(Cnt).Style.BackColor = Color.Yellow
                        End If
                    Next

                    INTCNT01 += 1

                    .Rows.Add(RowItem)
                End With
            End While

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            lab件数.Text = Format(CDbl(INTCNT01), "#,##0")

            With DataGridView
                Dim SumKin As Decimal = 0
                For RowCnt As Integer = 0 To INTCNT01 - 1
                    SumKin += CDec(.Rows(RowCnt).Cells(17).Value)
                Next
                .RowCount = INTCNT01 + 1

                '合計金額表示行
                .Rows(INTCNT01).Cells(17).ReadOnly = True
                .Rows(INTCNT01).Cells(17).Value = Format(SumKin, "#,##0")

            End With

            PFUNC_Spread_Set = True
        Catch ex As Exception
            MainLOG.Write("PFUNC_Spread_Set", "失敗", ex.Message)
            Return False
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function
    Private Function PFUNC_Update_ENTMAST() As Boolean

        'エントリマスタを更新する
        PFUNC_Update_ENTMAST = False

        For lLoopCount As Integer = 0 To (DataGridView.RowCount - 1)
            With DataGridView
                If Trim(.Rows(lLoopCount).Cells(0).Value) <> "" Then
                    Select Case STR_生徒明細入出区分
                        Case "2"
                            'エントリマスタ１処理
                            STR_SQL = " UPDATE  G_ENTMAST1 SET "
                        Case "3"
                            'エントリマスタ２処理
                            STR_SQL = " UPDATE  G_ENTMAST2 SET "
                    End Select
                    '入金・出金金額
                    If Trim(.Rows(lLoopCount).Cells(17).Value) = "" Then
                        STR_SQL += " FURIKIN_E = " & 0
                    Else
                        STR_SQL += " FURIKIN_E = " & CDec(.Rows(lLoopCount).Cells(17).Value.ToString.Trim)
                    End If
                    '更新日
                    STR_SQL += ", KOUSIN_DATE_E    ='" & Str_Syori_Date(0) & "'"
                    STR_SQL += " WHERE"
                    STR_SQL += " GAKKOU_CODE_E  ='" & STR_生徒明細学校コード & "'"
                    STR_SQL += " AND"
                    STR_SQL += " FURI_DATE_E  ='" & STR_生徒明細振替日 & "'"
                    STR_SQL += " AND"
                    STR_SQL += " NENDO_E  =" & CInt(Trim(.Rows(lLoopCount).Cells(0).Value))
                    STR_SQL += " AND"
                    STR_SQL += " TUUBAN_E  =" & CInt(Trim(.Rows(lLoopCount).Cells(1).Value))
                Else
                    Exit For
                End If
            End With

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                '更新処理エラー
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Exit Function
            End If
        Next lLoopCount

        PFUNC_Update_ENTMAST = True

    End Function
    Private Function PFUNC_Update_Schedule() As Boolean
        'スケジュールマスタの更新

        PFUNC_Update_Schedule = False


        STR_SQL = " UPDATE  G_SCHMAST SET "
        STR_SQL += " CHECK_FLG_S ='1'"
        STR_SQL += ",CHECK_DATE_S ='" & Str_Syori_Date(0) & "'"
        STR_SQL += ",TIME_STAMP_S ='" & Str_Syori_Date(1) & "'"
        STR_SQL += " WHERE"

        STR_SQL += " GAKKOU_CODE_S ='" & STR_生徒明細学校コード & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S   ='" & STR_生徒明細振替日 & "'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S ='2'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S   ='" & STR_生徒明細入出区分 & "'"
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Exit Function
        End If

        PFUNC_Update_Schedule = True

    End Function

    Private Sub PSUB_GET_GINKONAME(ByVal pGinko_Code As String, ByVal pSiten_Code As String, ByVal db As MyOracle)

        '金融機関コードと支店コードから金融機関名、支店名を抽出

        Str_Ginko(0) = ""
        Str_Ginko(1) = ""
        Str_Ginko(2) = ""
        Str_Ginko(3) = ""

        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            If pGinko_Code.Trim = "" OrElse pSiten_Code.Trim = "" Then
                Exit Sub
            End If

            Dim SQL As New System.Text.StringBuilder(128)

            SQL.Append(" SELECT ")
            SQL.Append(" KIN_KNAME_N ")
            SQL.Append(",KIN_NNAME_N ")
            SQL.Append(",SIT_KNAME_N ")
            SQL.Append(",SIT_NNAME_N ")
            SQL.Append(" FROM TENMAST ")
            SQL.Append(" WHERE KIN_NO_N = '" & pGinko_Code & "'")
            SQL.Append(" AND SIT_NO_N = '" & pSiten_Code & "'")

            Orareader = New CASTCommon.MyOracleReader(db)

            If Orareader.DataReader(SQL) Then
                Str_Ginko(0) = Orareader.GetItem("KIN_KNAME_N")
                Str_Ginko(1) = Orareader.GetItem("KIN_NNAME_N")
                Str_Ginko(2) = Orareader.GetItem("SIT_KNAME_N")
                Str_Ginko(3) = Orareader.GetItem("SIT_NNAME_N")
            Else
                Exit Sub
            End If

            Orareader.Close()
            Orareader = Nothing

        Catch ex As Exception
            Throw New Exception("TENMAST取得失敗", ex)
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
                Orareader = Nothing
            End If
        End Try

    End Sub
#End Region

    Private colNo, rowNo As Integer
    Private TextEditCtrl As DataGridViewTextBoxEditingControl

    Private Sub CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        colNo = e.ColumnIndex
        rowNo = e.RowIndex

        CType(sender, DataGridView).ImeMode = ImeMode.Disable
    End Sub
    Private Sub CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        'スプレット内項目前ZERO詰め
        With CType(sender, DataGridView)
            Select Case colNo
                Case 17
                    Dim str_Value As String
                    str_Value = .Rows(e.RowIndex).Cells(e.ColumnIndex).Value
                    If Not str_Value Is Nothing Then
                        If IsNumeric(str_Value) Then
                            .Rows(e.RowIndex).Cells(e.ColumnIndex).Value = Format(CDec(str_Value), "#,##0")
                        End If
                    End If
            End Select
        End With

    End Sub
    Private Sub EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs)
        TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)

        Select Case colNo
            Case 17
                AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
        End Select
    End Sub
    Private Sub CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Select Case colNo
            Case 17
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney

                '合計金額表示処理を追加
                With DataGridView
                    Dim SumKin As Decimal = 0
                    For RowCnt As Integer = 0 To INTCNT01 - 1
                        SumKin += CDec(.Rows(RowCnt).Cells(17).Value)
                    Next
                    .RowCount = INTCNT01 + 1

                    '合計金額表示行
                    .Rows(INTCNT01).Cells(17).ReadOnly = True
                    .Rows(INTCNT01).Cells(17).Value = Format(SumKin, "#,##0")
                End With

        End Select

        Call CellLeave(sender, e)
    End Sub

    Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DataGridView.RowPostPaint
        Dim dgv As DataGridView = CType(sender, DataGridView)

        ' 行ヘッダのセル領域を、行番号を描画する長方形とする
        ' （ただし右端に4ドットのすき間を空ける）
        Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, dgv.Rows(e.RowIndex).Height)

        ' 上記の長方形内に行番号を縦方向中央＆右詰で描画する
        ' フォントや色は行ヘッダの既定値を使用する
        TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, _
                              rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
                              Or TextFormatFlags.Right)

    End Sub

End Class
