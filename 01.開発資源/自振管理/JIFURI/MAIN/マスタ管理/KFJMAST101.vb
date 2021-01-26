Imports clsFUSION.clsMain
Imports System.Drawing.Printing
Imports System.Text
Imports System.IO
Imports CASTCommon.ModPublic
Imports System.Data.OracleClient
Imports System.Runtime.InteropServices
Imports CASTCommon

Public Class KFJMAST101
    Inherits System.Windows.Forms.Form

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAST101", "手数料徴求フラグ一括更新画面")
    Private Const msgTitle As String = "手数料徴求フラグ一括更新画面(KFJMAST101)"
    'ソートオーダーフラグ
    Dim SortOrderFlag As Boolean = True

    'クリックした列の番号
    Dim ClickedColumn As Integer

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Public startFuriDate As String = ""
    Public endFuriDate As String = ""
    Public TorisCode As String = ""
    Public TorifCode As String = ""

#Region " 画面のロード "
    Protected Sub KFJMAST101_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim OraDB As New CASTCommon.MyOracle()
        Dim OraReader As New CASTCommon.MyOracleReader(OraDB)
        Dim SQL As StringBuilder
        Dim MotherForm As New KFJMAST100

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = String.Concat(TorisCode, TorifCode.PadLeft(2, "0"c))
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            With Me.ListView1
                .Clear()
                .CheckBoxes = True
                .Columns.Add("", 30, HorizontalAlignment.Center)
                .Columns.Add("取引先ｺｰﾄﾞ", 100, HorizontalAlignment.Center)
                .Columns.Add("取引先名", 150, HorizontalAlignment.Center)
                .Columns.Add("決済区分", 100, HorizontalAlignment.Center)
                .Columns.Add("振替ｺｰﾄﾞ", 60, HorizontalAlignment.Center)
                .Columns.Add("企業ｺｰﾄﾞ", 60, HorizontalAlignment.Center)
                .Columns.Add("振替日", 80, HorizontalAlignment.Center)
                .Columns.Add("手数料徴求予定日", 80, HorizontalAlignment.Center)
                .Columns.Add("手数料金額", 100, HorizontalAlignment.Right)
            End With

            SQL = New StringBuilder(128)

            SQL.Append("SELECT TORIS_CODE_S")
            SQL.Append(",TORIF_CODE_S")
            SQL.Append(",ITAKU_NNAME_T")
            SQL.Append(",KESSAI_KBN_T")
            SQL.Append(",FURI_CODE_S")
            SQL.Append(",KIGYO_CODE_S")
            SQL.Append(",FURI_DATE_S")
            SQL.Append(",TESUU_YDATE_S")
            SQL.Append(",TESUU_KIN_S")
            SQL.Append(" FROM SCHMAST,TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" TORIS_CODE_T = TORIS_CODE_S ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            SQL.Append(" AND TORIS_CODE_T =" & SQ(TorisCode))
            If TorifCode.Trim.Length = 2 Then
                SQL.Append(" AND TORIF_CODE_T =" & SQ(TorifCode))
            End If
            SQL.Append(" AND FURI_DATE_S >= " & SQ(startFuriDate))
            SQL.Append(" AND FURI_DATE_S <= " & SQ(endFuriDate))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND KESSAI_FLG_S = '1'")
            SQL.Append(" AND TESUUTYO_FLG_S = '0'")
            SQL.Append(" AND KESSAI_KBN_T <> '99'")
            SQL.Append("ORDER BY TORIS_CODE_S ASC,TORIF_CODE_S ASC,FURI_DATE_S ASC")

            Dim LineColor As Color
            Dim ROW As Integer = 0

            If OraReader.DataReader(SQL) = True Then
                Do Until OraReader.EOF
                    Dim Data(8) As String
                    Data(1) = String.Concat(New String() {OraReader.GetString("TORIS_CODE_S"), "-", OraReader.GetString("TORIF_CODE_S")})    ' 取引先コード        
                    Data(2) = OraReader.GetString("ITAKU_NNAME_T").Trim     ' 取引先名

                    Select Case OraReader.GetString("KESSAI_KBN_T")         ' 決済区分
                        Case "00"
                            Data(3) = "預け金"
                        Case "01"
                            Data(3) = "口座入金"
                        Case "02"
                            Data(3) = "為替振込"
                        Case "03"
                            Data(3) = "為替付替"
                        Case "04"
                            Data(3) = "別段出金のみ"
                        Case "05"
                            Data(3) = "特別企業"
                        Case "99"
                            Data(3) = "決済対象外"
                    End Select

                    Data(4) = OraReader.GetString("FURI_CODE_S")            ' 振替コード
                    Data(5) = OraReader.GetString("KIGYO_CODE_S")           ' 企業コード
                    Data(6) = String.Concat(New String() {OraReader.GetString("FURI_DATE_S").Substring(0, 4), _
                                                          "/", OraReader.GetString("FURI_DATE_S").Substring(4, 2), "/" _
                                                          , OraReader.GetString("FURI_DATE_S").Substring(6, 2)})           ' 振替日
                    Data(7) = String.Concat(New String() {OraReader.GetString("TESUU_YDATE_S").Substring(0, 4), _
                                      "/", OraReader.GetString("TESUU_YDATE_S").Substring(4, 2), "/" _
                                      , OraReader.GetString("TESUU_YDATE_S").Substring(6, 2)})           ' 手数料徴求予定日
                    Data(8) = OraReader.GetInt64("TESUU_KIN_S").ToString("#,###")          ' 手数料金額

                    If ROW Mod 2 = 0 Then
                        LineColor = Color.White
                    Else
                        LineColor = Color.WhiteSmoke
                    End If

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, LineColor, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                    ROW += 1
                    OraReader.NextRead()

                Loop
            End If

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not OraDB Is Nothing Then OraDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
#End Region
    
#Region " 終了 "
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

#Region " 実行ボタン "
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :作成ボタン押下時の処理
        'Return         :
        'Create         :
        'Update         :
        '=====================================================================================
        Dim MainDB As New CASTCommon.MyOracle()
        Dim SQL As StringBuilder
        Dim CreateCSV As New KFJP057

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)開始", "成功", "")

            Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")
            Dim SyoriTime As String = System.DateTime.Now.ToString("HHmmss")

            Dim nSelectItems As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            '******************************************
            ' チェックボックスのチェック
            '******************************************
            'リストに１件も表示されていないとき
            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                'リストに１件以上表示されているが、チェックされていないとき
                If nSelectItems.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            '------------------------------
            '実行確認メッセージ
            '------------------------------
            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.Yes Then
                Return
            End If
            MainDB.BeginTrans()

            '********************************************
            ' スケジュールマスタの更新
            '********************************************
            Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items
            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName

            For Each item As ListViewItem In nItems
                SQL = New StringBuilder(128)

                If item.Checked = True Then

                    SQL.Append("UPDATE SCHMAST SET TESUUTYO_FLG_S = '1' ")
                    SQL.Append(",TESUU_DATE_S = '" & SyoriDate & "'")
                    SQL.Append(",TESUU_TIME_STAMP_S = '" & String.Concat(SyoriDate, SyoriTime) & "'")
                    SQL.Append(" WHERE TORIS_CODE_S = '" & item.SubItems(1).Text.Replace("-", "").Substring(0, 10) & "' ")
                    SQL.Append(" AND TORIF_CODE_S = '" & item.SubItems(1).Text.Replace("-", "").Substring(10, 2) & "' ")
                    SQL.Append(" AND FURI_DATE_S = '" & item.SubItems(6).Text.Replace("/", "") & "' ")

                    Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)

                    If nRet <= 0 Then
                        MainDB.Rollback()
                        CreateCSV.CloseCsv()
                        MessageBox.Show(String.Format(MSG0027E, "スケジュールマスタ", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If

                    '***帳票に追加していく
                    CreateCSV.OutputCsvData(SyoriDate, True)                                                    '処理日
                    CreateCSV.OutputCsvData(SyoriTime, True)                                                    'タイムスタンプ
                    CreateCSV.OutputCsvData(item.SubItems(1).Text.Replace("-", "").Substring(0, 10), True)      '取引先主コード
                    CreateCSV.OutputCsvData(item.SubItems(1).Text.Replace("-", "").Substring(10, 2), True)      '取引先副コード
                    CreateCSV.OutputCsvData(item.SubItems(2).Text, True)                                        '取引先名（漢字）
                    CreateCSV.OutputCsvData(item.SubItems(3).Text, True)                                        '決済区分
                    CreateCSV.OutputCsvData(item.SubItems(4).Text, True)                                        '振替コード
                    CreateCSV.OutputCsvData(item.SubItems(5).Text, True)                                        '企業コード
                    CreateCSV.OutputCsvData(item.SubItems(6).Text.Replace("/", ""), True)                       '振替日
                    CreateCSV.OutputCsvData(item.SubItems(7).Text.Replace("/", ""), True)                       '手数料徴求予定日
                    CreateCSV.OutputCsvData(item.SubItems(8).Text.Replace(",", ""), True, True)                 '手数料金額
                    '***帳票に追加していく

                End If

            Next

            CreateCSV.CloseCsv()

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String
            ExeRepo.SetOwner = Me
            Dim iRet As Integer
            Dim errMsg As String = ""

            'パラメータ設定：ログイン名、ＣＳＶファイル名、取引先コード
            param = GCom.GetUserID & "," & strCSVFileName
            iRet = ExeRepo.ExecReport("KFJP057.EXE", param)

            If iRet <> 0 Then
                '印刷失敗：戻り値に対応したエラーメッセージを表示する
                Select Case iRet
                    Case -1
                        MessageBox.Show(MSG0226W.Replace("{0}", "手数料徴求フラグ一括更新一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Case Else
                        MessageBox.Show(MSG0004E.Replace("{0}", "手数料徴求フラグ一括更新一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select

                MainDB.Rollback()

                Return
            End If

            MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            MainDB.Commit()

            Me.ListView1.Items.Clear()

        Catch ex As Exception
            MainDB.Rollback()
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)例外エラー", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region " 全選択ボタン "
    Private Sub btnAllOn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOn.Click
        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)開始", "成功", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = True
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region " 全解除ボタン "
    Private Sub btnAllOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOff.Click
        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)開始", "成功", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = False
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region " 関数 "

    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Call GCom.MonitorCsvFile(CType(sender, ListView))
    End Sub

    Private Sub ListView1_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick

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

        End Try
    End Sub

#End Region

End Class
