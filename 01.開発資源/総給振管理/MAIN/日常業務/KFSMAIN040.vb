Option Strict On
Option Explicit On

Imports CASTCommon
Imports System.Text

Public Class KFSMAIN040

#Region "変数宣言"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFSMAIN040", "振込発信リエンタ作成画面")
    Private Const msgTitle As String = "振込発信リエンタ作成画面(KFSMAIN040)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure

    Private LW As LogWrite

    Private KYUFURI_HANI As Integer             '給振範囲
    Private SOUFURI_HANI As Integer             '総振範囲
    Private HASSIN_DATE As String               '発信日
    Private KYUFURI_END_DATE As String          '給振範囲終了日
    Private SOUFURI_END_DATE As String          '総振範囲終了日
    Private EIGYOUBI_1_AFTER As String          '１営業日後
    Private EIGYOUBI_2_AFTER As String          '２営業日後

    'クリックした列の番号
    Dim ClickedColumn As Integer
    'ソートオーダーフラグ
    Dim SortOrderFlag As Boolean = True
#End Region

#Region "関数"
    Private Function HassinDataList() As Integer
        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim row As Integer = 0
        Dim Data(10) As String

        Try
            MainDB = New MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As StringBuilder = New StringBuilder(128)

            SQL.AppendLine("SELECT *")

            SQL.AppendLine(" FROM S_SCHMAST, S_TORIMAST")

            SQL.AppendLine(" WHERE FSYORI_KBN_S = '3'")
            SQL.AppendLine(" AND TOUROKU_FLG_S = '1'")
            '発信フラグ０と２を対象とする
            SQL.AppendLine(" AND HASSIN_FLG_S IN ('0', '2')")
            SQL.AppendLine(" AND FURI_DATE_S >= '" & HASSIN_DATE & "'")
            SQL.AppendLine(" AND TYUUDAN_FLG_S = '0'")
            SQL.AppendLine(" AND (")
            SQL.AppendLine("(FURI_DATE_S <= '" & KYUFURI_END_DATE & "' AND SYUBETU_S IN ('11', '12'))")
            SQL.AppendLine(" OR (FURI_DATE_S <= '" & SOUFURI_END_DATE & "' AND SYUBETU_S = '21')")
            SQL.AppendLine(")")

            SQL.AppendLine(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.AppendLine(" AND TORIF_CODE_S = TORIF_CODE_T ")

            SQL.AppendLine(" ORDER BY FURI_DATE_S")
            SQL.AppendLine(", SOUSIN_KBN_S")
            SQL.AppendLine(", TORIS_CODE_S")
            SQL.AppendLine(", TORIF_CODE_S")
            SQL.AppendLine(", MOTIKOMI_SEQ_S")

            If OraReader.DataReader(SQL) = True Then

                Do While OraReader.EOF = False

                    row += 1

                    'ListViewの値設定

                    Data(1) = row.ToString("#,##0")
                    '取引先名
                    Data(2) = GCom.GetLimitString(OraReader.GetString("ITAKU_NNAME_T"), 40)
                    '取引先コード（主コード + "-" + 副コード）
                    Data(3) = OraReader.GetString("TORIS_CODE_T") & "-" & OraReader.GetString("TORIF_CODE_T")
                    '振込日
                    Dim furidate As String = OraReader.GetString("FURI_DATE_S")
                    Data(7) = furidate.Substring(0, 4) & "/" & furidate.Substring(4, 2) & "/" & furidate.Substring(6, 2)

                    '公金区分取得
                    '信組対応するなら更に追加必要・・・
                    Dim koukin As Boolean = False
                    Select Case OraReader.GetString("SYUMOKU_CODE_T")
                        Case "01", "02"
                            koukin = True
                        Case Else
                            koukin = False
                    End Select

                    '契約種別
                    Select Case OraReader.GetString("SYUBETU_S")
                        Case "11"
                            If koukin Then
                                Data(4) = "給与公金"
                            Else
                                Data(4) = "給与"
                            End If
                        Case "12"
                            If koukin Then
                                Data(4) = "賞与公金"
                            Else
                                Data(4) = "賞与"
                            End If
                        Case Else
                            If koukin Then
                                Data(4) = "振込公金"
                            Else
                                Data(4) = "振込"
                            End If
                    End Select

                    '適用種別
                    Select Case furidate
                        Case HASSIN_DATE : Data(5) = "振込"
                        Case EIGYOUBI_1_AFTER : Data(5) = "先振"
                        Case Else
                            Select Case OraReader.GetString("SYUBETU_S")
                                Case "11" : Data(5) = "給与"
                                Case "12" : Data(5) = "賞与"
                                Case Else : Data(5) = "先振"
                            End Select
                    End Select

                    If koukin Then
                        Data(5) &= "公金"
                    End If

                    '送信区分
                    Select Case OraReader.GetString("SOUSIN_KBN_S")
                        Case "0"
                            Data(6) = "為替振込"
                        Case "1"
                            Data(6) = "ロギング"
                        Case "2"
                            Data(6) = "CSVﾘｴﾝﾀ"
                        Case Else
                            Data(6) = "為替振込"
                    End Select

                    '持込SEQ
                    Data(8) = OraReader.GetString("MOTIKOMI_SEQ_S")

                    '2017/04/05 saitou 近畿産業信組(RSV2標準) MODIFY インプットエラー分を除く ------------------------------------- START
                    Data(9) = (OraReader.GetInt64("SYORI_KEN_S") - OraReader.GetInt64("ERR_KEN_S")).ToString("###,###")
                    Data(10) = (OraReader.GetInt64("SYORI_KIN_S") - OraReader.GetInt64("ERR_KIN_S")).ToString("###,###")
                    'Data(9) = OraReader.GetInt64("SYORI_KEN_S").ToString("###,###")
                    'Data(10) = OraReader.GetInt64("SYORI_KIN_S").ToString("###,###")
                    '2017/04/05 saitou 近畿産業信組(RSV2標準) MODIFY -------------------------------------------------------------- END

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, Color.White, Nothing)

                    'チェックボックス制御
                    Select Case GCom.NzDec(OraReader.GetString("KESSAI_PATN_T"), 0)
                        Case 0
                            If OraReader.GetString("KAKUHO_FLG_S") = "1" Then
                                vLstItem.Checked = True
                            End If
                        Case Else
                            vLstItem.Checked = True
                    End Select

                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})
                    
                    OraReader.NextRead()
                Loop
            Else
                Return 0    '0件
            End If

            Return row      '正常 件数

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(データ検索)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return -1

        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function
#End Region

    '画面LOAD時
    Private Sub KFSMAIN040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim strSysDate As String

        Try
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '------------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '休日マスタ取り込み
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時にシステム日付を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            '給振／総振範囲
            KYUFURI_HANI = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "KYUFURI"), 0)
            SOUFURI_HANI = GCom.NzInt(CASTCommon.GetFSKJIni("KAWASE", "SOUFURI"), 0)

            'リスト画面の初期化
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    '実行ボタン押下時
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Dim MainDB As MyOracle = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            Dim nSelectItems As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            'リストに１件も表示されていないとき
            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                'リストに１件以上表示されているが、チェックされていないとき
                If nSelectItems.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            Dim dRet As DialogResult
            dRet = MessageBox.Show(MSG0023I.Replace("{0}", "振込発信リエンタ作成"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            If Not dRet = DialogResult.OK Then
                Return
            End If

            MainDB = New CASTCommon.MyOracle
            Dim SQL As StringBuilder = New StringBuilder

            '***ｽｹｼﾞｭｰﾙﾏｽﾀの更新
            Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items

            For Each item As ListViewItem In nItems

                Dim FURI_DATE As String = GCom.NzDec(item.SubItems(7).Text, "")
                Dim Temp As String = GCom.NzDec(item.SubItems(3).Text, "")
                Dim TORIS_CODE As String = Temp.Substring(0, 10)
                Dim TORIF_CODE As String = Temp.Substring(10)
                Dim MOTIKOMI_SEQ As String = GCom.NzDec(item.SubItems(8).Text, "0")

                SQL.Length = 0
                LW.ToriCode = TORIS_CODE & "-" & TORIF_CODE
                LW.FuriDate = FURI_DATE

                SQL.Append("UPDATE S_SCHMAST SET ")

                If item.Checked = True Then
                    SQL.Append("HASSIN_FLG_S = '2'")
                Else
                    SQL.Append("HASSIN_FLG_S = '0'")
                End If

                SQL.Append(" WHERE TORIS_CODE_S = '" & TORIS_CODE & "'")
                SQL.Append(" AND TORIF_CODE_S = '" & TORIF_CODE & "'")
                SQL.Append(" AND FURI_DATE_S = '" & FURI_DATE & "'")
                SQL.Append(" AND MOTIKOMI_SEQ_S = " & MOTIKOMI_SEQ)

                Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
                If nRet = 0 Then
                    MainDB.Rollback()
                    Return
                ElseIf nRet < 0 Then
                    Throw New Exception(MSG0002E.Replace("{0}", "登録"))
                End If
            Next

            Dim jobid As String
            Dim para As String

            'ジョブマスタに登録
            jobid = "S040"

            para = HASSIN_DATE                 '発信日をパラメタとして設定
            para &= "," & KYUFURI_END_DATE
            para &= "," & SOUFURI_END_DATE

            '#########################
            'job検索
            '#########################
            Dim iRet As Integer
            iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            If iRet = 1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf iRet = -1 Then
                Throw New Exception(MSG0002E.Replace("{0}", "検索"))
            End If

            '#########################
            'job登録
            '#########################
            If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then
                Throw New Exception(MSG0005E)
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0021I.Replace("{0}", "振込発信リエンタ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            'リスト画面の初期化
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False

            Me.txtKijyunDateY.Enabled = True
            Me.txtKijyunDateM.Enabled = True
            Me.txtKijyunDateD.Enabled = True

        Catch ex As Exception

            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.Message)

        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try
    End Sub

    '終了ボタン押下時
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

#Region "KFSMAIN040用関数"

    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :実行ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :
        'Update         :
        '============================================================================

        Try
            '------------------------------------------------
            '振込年チェック
            '------------------------------------------------
            '必須チェック
            If txtKijyunDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '振込月チェック
            '------------------------------------------------
            '必須チェック
            If txtKijyunDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '範囲チェック
            If Not (GCom.NzInt(txtKijyunDateM.Text) >= 1 And GCom.NzInt(txtKijyunDateM.Text) <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '振込日チェック
            '------------------------------------------------
            '必須チェック
            If txtKijyunDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '範囲チェック
            If Not (GCom.NzInt(txtKijyunDateD.Text) >= 1 And GCom.NzInt(txtKijyunDateD.Text) <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = txtKijyunDateY.Text & "/" & txtKijyunDateM.Text & "/" & txtKijyunDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
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

    'ゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKijyunDateY.Validating, txtKijyunDateM.Validating, txtKijyunDateD.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

    '一覧表示領域のソート追加
    Private Sub LetSort_ListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
    Handles ListView1.ColumnClick

        Try

            With CType(sender, ListView)

                If ClickedColumn = e.Column Then
                    ' 同じ列をクリックした場合は，逆順にする 
                    SortOrderFlag = Not SortOrderFlag
                End If

                ' 列番号設定
                ClickedColumn = e.Column

                ' 列水平方向配置
                Dim ColAlignment As HorizontalAlignment = .Columns(e.Column).TextAlign

                ' ソート
                .ListViewItemSorter = New CASTCommon.ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)

                ' ソート実行
                .Sort()

            End With

        Catch ex As Exception
            Throw
        End Try

    End Sub
#End Region

    Private Sub btnAllon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOn.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)開始", "成功", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = True '((CType(sender, Button).Name.ToUpper = "BTNALLON")) OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)", "失敗", ex.Message)
        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)終了", "成功", "")

        End Try

    End Sub

    Private Sub btnAlloff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOff.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)開始", "成功", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = False 'OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)", "失敗", ex.Message)
        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)終了", "成功", "")

        End Try

    End Sub

    Private Sub btnRef_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRef.Click

        Dim iRet As Integer

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")


            'テキストボックス入力チェック
            If fn_check_text() = False Then
                Return
            End If

            'リスト画面の初期化
            Me.ListView1.Items.Clear()

            HASSIN_DATE = txtKijyunDateY.Text & txtKijyunDateM.Text & txtKijyunDateD.Text

            '１／２営業日後
            GCom.CheckDateModule(HASSIN_DATE, EIGYOUBI_1_AFTER, 1, 0)
            GCom.CheckDateModule(HASSIN_DATE, EIGYOUBI_2_AFTER, 2, 0)

            '総給振範囲取得
            GCom.CheckDateModule(HASSIN_DATE, KYUFURI_END_DATE, KYUFURI_HANI, 0)
            GCom.CheckDateModule(HASSIN_DATE, SOUFURI_END_DATE, SOUFURI_HANI, 0)

            iRet = HassinDataList()

            If iRet = 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtKijyunDateY.Focus()
                Return
            ElseIf iRet = -1 Then

                Return

            Else

                Me.btnRef.Enabled = False
                Me.btnClear.Enabled = True
                Me.btnAllOn.Enabled = True
                Me.btnAllOff.Enabled = True
                Me.btnAction.Enabled = True
                Me.txtKijyunDateY.Enabled = False
                Me.txtKijyunDateM.Enabled = False
                Me.txtKijyunDateD.Enabled = False
                Me.btnAction.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)", "失敗", ex.Message)

        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
        End Try

    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)開始", "成功", "")

            '基準日にシステム日付を表示
            '休日マスタ取り込み
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時に振込日に前営業日を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            'リスト画面の初期化
            Me.ListView1.Items.Clear()

            Me.btnRef.Enabled = True
            Me.btnClear.Enabled = False
            Me.btnAllOn.Enabled = False
            Me.btnAllOff.Enabled = False
            Me.btnAction.Enabled = False
            Me.txtKijyunDateY.Enabled = True
            Me.txtKijyunDateM.Enabled = True
            Me.txtKijyunDateD.Enabled = True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)終了", "成功", "")
        End Try

    End Sub

    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Dim mRet As Integer
        Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items
        Dim CreateCSV As New KFSPxxx("KFSP009")

        Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")
        Dim SyoriTime As String = System.DateTime.Now.ToString("HHmmss")

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", "未検索")
                Return
            End If

            mRet = MessageBox.Show(MSG0013I.Replace("{0}", "振込発信リエンタ対象一覧"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)

            If mRet <> DialogResult.OK Then
                Return
            End If

            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName
            For Each item As ListViewItem In nItems
                CreateCSV.OutputCsvData(HASSIN_DATE)                                    '発信予定日
                CreateCSV.OutputCsvData(item.SubItems(1).Text.Replace(",", ""))         '項番
                CreateCSV.OutputCsvData(SyoriDate)                                      '処理日
                CreateCSV.OutputCsvData(SyoriTime)                                      'タイムスタンプ
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(0, 10))         '取引先主コード
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(11, 2))         '取引先副コード
                CreateCSV.OutputCsvData(item.SubItems(2).Text)                          '取引先名（漢字）
                CreateCSV.OutputCsvData(item.SubItems(4).Text)                          '契約種別
                CreateCSV.OutputCsvData(item.SubItems(5).Text)                          '適用種別
                CreateCSV.OutputCsvData(item.SubItems(6).Text)                          '送信区分
                CreateCSV.OutputCsvData(item.SubItems(7).Text.Replace("/", ""))         '振込日
                CreateCSV.OutputCsvData(item.SubItems(8).Text)                          '持込SEQ
                CreateCSV.OutputCsvData(item.SubItems(9).Text.Replace(",", ""))         '依頼件数
                CreateCSV.OutputCsvData(item.SubItems(10).Text.Replace(",", ""), False, True) '依頼金額
          
            Next
            CreateCSV.CloseCsv()

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String
            ExeRepo.SetOwner = Me
            Dim iRet As Integer
            Dim errMsg As String = ""

            'パラメータ設定：ログイン名、ＣＳＶファイル名、取引先コード
            '2016/02/04 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- START
            'パラメータ変更　ログイン名、ＣＳＶファイル名、金バッチ使用フラグ
            param = GCom.GetUserID & "," & strCSVFileName & ",0"
            'param = GCom.GetUserID & "," & strCSVFileName
            '2016/02/04 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- END
            iRet = ExeRepo.ExecReport("KFSP009.EXE", param)

            '2016/02/04 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- START
            '印刷完了メッセージも出力する
            Select Case iRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, "振込発信リエンタ対象一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振込発信リエンタ対象一覧印刷", "失敗", "印刷対象なし")
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "振込発信リエンタ対象一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振込発信リエンタ対象一覧印刷", "失敗")
            End Select

            'If iRet <> 0 Then
            '    '印刷失敗：戻り値に対応したエラーメッセージを表示する
            '    Select Case iRet
            '        Case -1
            '            errMsg = MSG0226W.Replace("{0}", "振込発信リエンタ対象一覧")
            '            MessageBox.Show(errMsg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        Case Else
            '            errMsg = MSG0004E.Replace("{0}", "振込発信リエンタ対象一覧")
            '            MessageBox.Show(errMsg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    End Select

            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振込発信リエンタ対象一覧印刷", "失敗")
            '    Return
            'End If
            '2016/02/04 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- END

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
        End Try

    End Sub
End Class

