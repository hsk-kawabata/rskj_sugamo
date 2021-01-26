Imports System.Text
Imports CASTCommon

''' <summary>
''' 振込発信CSVリエンタ作成画面　メインクラス
''' </summary>
''' <remarks>
''' 2016/10/12 saitou added for RSV2対応(信組)
''' 近畿産業信組ベースでRSV2に標準組込み
''' </remarks>
Public Class KFSMAIN120

#Region "クラス定数"

    'イベント
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    'メッセージ
    Private Const msgTitle As String = "振込発信CSVリエンタ作成画面(KFSMAIN120)"

#End Region

#Region "クラス変数"

    'イベントログ
    Private MainLOG As New CASTCommon.BatchLOG("KFSMAIN120", "振込発信CSVリエンタ作成画面")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先コード
        Dim FuriDate As String          '振込日
    End Structure
    Private LW As LogWrite

    Private HASSIN_DATE As String                   '発信日
    Private SOUFURI_DATE As String                  '総振対象日
    Private KYUFURI_DATE As String                  '給振対象日

    'クリックした列の番号
    Private ClickedColumn As Integer
    'ソートオーダーフラグ
    Private SortOrderFlag As Boolean = True

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' フォームロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KSFMAIN120_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            '--------------------------------------------------
            'ログ書き込み変数の初期化
            '--------------------------------------------------
            With LW
                .UserID = GCom.GetUserID
                .ToriCode = "0000000000-00"
                .FuriDate = "00000000"
            End With

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '--------------------------------------------------
            'システム日付とユーザ名を表示
            '--------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '--------------------------------------------------
            '発信日にシステム日付を設定
            '--------------------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            Me.txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            Me.txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            Me.txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            '--------------------------------------------------
            '画面項目の初期化
            '--------------------------------------------------
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 参照ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnRef_Click(sender As Object, e As EventArgs) Handles btnRef.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            '--------------------------------------------------
            'テキストボックスの入力チェック
            '--------------------------------------------------
            If Me.fn_check_text() = False Then
                Return
            End If

            '--------------------------------------------------
            'リストビューの設定
            '--------------------------------------------------
            Dim iRet As Integer
            Me.ListView1.Items.Clear()

            Me.HASSIN_DATE = String.Concat(New String() {Me.txtKijyunDateY.Text, Me.txtKijyunDateM.Text, Me.txtKijyunDateD.Text})

            '総振対象日は発信日と同一（システム日付）
            Me.SOUFURI_DATE = Me.HASSIN_DATE
            '給振対象日は発信日の２営業日後（システム日付の２営業日後）
            GCom.CheckDateModule(Me.HASSIN_DATE, Me.KYUFURI_DATE, 2, 0)

            iRet = Me.HassinDataList()

            If iRet = 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateY.Focus()
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' クリアボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)開始", "成功", "")

            '--------------------------------------------------
            '発信日にシステム日付を設定
            '--------------------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            Me.txtKijyunDateY.Text = strSysDate.Substring(0, 4)
            Me.txtKijyunDateM.Text = strSysDate.Substring(4, 2)
            Me.txtKijyunDateD.Text = strSysDate.Substring(6, 2)

            '--------------------------------------------------
            '画面項目の初期化
            '--------------------------------------------------
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

    ''' <summary>
    ''' 全選択ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAllOn_Click(sender As Object, e As EventArgs) Handles btnAllOn.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)開始", "成功", "")
            For Each item As ListViewItem In ListView1.Items
                item.Checked = True
            Next item
            Me.ListView1.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 全解除ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAllOff_Click(sender As Object, e As EventArgs) Handles btnAllOff.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)開始", "成功", "")
            For Each item As ListViewItem In ListView1.Items
                item.Checked = False
            Next item
            Me.ListView1.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 作成ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnAction_Click(sender As Object, e As EventArgs) Handles btnAction.Click

        Dim db As CASTCommon.MyOracle = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            '--------------------------------------------------
            'リストビューのチェック
            '--------------------------------------------------
            Dim nSelectItems As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            'リストに１件も表示されていないとき
            If Me.ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                'リストに１件以上表示されているが、チェックされていないとき
                If nSelectItems.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            If MessageBox.Show(String.Format(MSG0023I, "振込発信CSVリエンタ作成"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            '--------------------------------------------------
            'スケジュールマスタの更新
            '--------------------------------------------------
            db = New CASTCommon.MyOracle
            Dim SQL As New StringBuilder
            Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items

            For Each item As ListViewItem In nItems

                Dim FURI_DATE As String = GCom.NzDec(item.SubItems(7).Text, "")
                Dim Temp As String = GCom.NzDec(item.SubItems(3).Text, "")
                Dim TORIS_CODE As String = Temp.Substring(0, 10)
                Dim TORIF_CODE As String = Temp.Substring(10)
                Dim MOTIKOMI_SEQ As String = GCom.NzDec(item.SubItems(8).Text, "0")

                LW.ToriCode = TORIS_CODE & "-" & TORIF_CODE
                LW.FuriDate = FURI_DATE

                With SQL
                    .Length = 0
                    .Append("update S_SCHMAST set ")
                    If item.Checked = True Then
                        .Append("HASSIN_FLG_S = '2'")
                    Else
                        .Append("HASSIN_FLG_S = '0'")
                    End If
                    .Append(" where TORIS_CODE_S = '" & TORIS_CODE & "'")
                    .Append(" and TORIF_CODE_S = '" & TORIF_CODE & "'")
                    .Append(" and FURI_DATE_S = '" & FURI_DATE & "'")
                    .Append(" and MOTIKOMI_SEQ_S = " & MOTIKOMI_SEQ)
                End With

                Dim nRet As Integer = db.ExecuteNonQuery(SQL)
                If nRet = 0 Then
                    db.Rollback()
                    Return
                ElseIf nRet < 0 Then
                    Throw New Exception(String.Format(MSG0002E, "更新"))
                End If
            Next

            '--------------------------------------------------
            'ジョブ登録
            '--------------------------------------------------
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            Dim jobid As String
            Dim para As String

            jobid = "S120"
            para = String.Concat(New String() {Me.HASSIN_DATE, ",", Me.KYUFURI_DATE, ",", Me.SOUFURI_DATE})

            Dim iRet As Integer
            iRet = MainLOG.SearchJOBMAST(jobid, para, db)
            If iRet = 1 Then
                db.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf iRet = -1 Then
                Throw New Exception(String.Format(MSG0002E, "検索"))
            End If

            If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, db) = False Then
                Throw New Exception(MSG0005E)
            End If

            db.Commit()

            MessageBox.Show(String.Format(MSG0021I, "振込発信CSVリエンタ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '--------------------------------------------------
            '画面項目の初期化
            '--------------------------------------------------
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
            db.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not db Is Nothing Then
                db.Close()
                db = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 印刷ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Dim nItems As ListView.ListViewItemCollection = Me.ListView1.Items
        Dim CreateCSV As New KFSPxxx("KFSP009")

        Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")
        Dim SyoriTime As String = System.DateTime.Now.ToString("HHmmss")

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            If Me.ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0224W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", "未検索")
                Return
            End If

            If MessageBox.Show(String.Format(MSG0013I, "振込発信CSVリエンタ対象一覧"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            '--------------------------------------------------
            '帳票印刷用データ作成
            '--------------------------------------------------
            CreateCSV.CreateCsvFile()
            Dim strCSVFileName As String = CreateCSV.FileName
            For Each item As ListViewItem In nItems
                CreateCSV.OutputCsvData(Me.HASSIN_DATE)                                         '発信予定日
                CreateCSV.OutputCsvData(item.SubItems(1).Text.Replace(",", ""))                 '項番
                CreateCSV.OutputCsvData(SyoriDate)                                              '処理日
                CreateCSV.OutputCsvData(SyoriTime)                                              'タイムスタンプ
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(0, 10))                 '取引先主コード
                CreateCSV.OutputCsvData(item.SubItems(3).Text.Substring(11, 2))                 '取引先副コード
                CreateCSV.OutputCsvData(item.SubItems(2).Text)                                  '取引先名（漢字）
                CreateCSV.OutputCsvData(item.SubItems(4).Text)                                  '契約種別
                CreateCSV.OutputCsvData(item.SubItems(5).Text)                                  '適用種別
                CreateCSV.OutputCsvData(item.SubItems(6).Text)                                  '送信区分
                CreateCSV.OutputCsvData(item.SubItems(7).Text.Replace("/", ""))                 '振込日
                CreateCSV.OutputCsvData(item.SubItems(8).Text)                                  '持込SEQ
                CreateCSV.OutputCsvData(item.SubItems(9).Text.Replace(",", ""))                 '依頼件数
                CreateCSV.OutputCsvData(item.SubItems(10).Text.Replace(",", ""), False, True)   '依頼金額
            Next
            CreateCSV.CloseCsv()

            '--------------------------------------------------
            '印刷処理実行
            '--------------------------------------------------
            Dim ExeRepo As New CAstReports.ClsExecute
            Dim param As String
            ExeRepo.SetOwner = Me
            Dim iRet As Integer
            Dim errMsg As String = ""

            'パラメータ設定：ユーザーID,CSVファイル名,3(振込発信CSVリエンタ)
            param = GCom.GetUserID & "," & strCSVFileName & ",3"
            iRet = ExeRepo.ExecReport("KFSP009.EXE", param)

            Select Case iRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, "振込発信CSVリエンタ対象一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振込発信CSVリエンタ対象一覧印刷", "失敗", "印刷対象なし")
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "振込発信CSVリエンタ対象一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振込発信CSVリエンタ対象一覧印刷", "失敗")
            End Select

        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 終了ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnEnd_Click(sender As Object, e As EventArgs) Handles btnEnd.Click
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

    ''' <summary>
    ''' リストビューカラムクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>リストビューのソート</remarks>
    Private Sub ListView1_ColumnClick(sender As Object, e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick
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

    ''' <summary>
    ''' テキストボックスバリデーティングイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>テキストボックスのゼロ埋め</remarks>
    Private Sub TextBox_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles _
        txtKijyunDateY.Validating, txtKijyunDateM.Validating, txtKijyunDateD.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 発信対象のリストビューを設定します。
    ''' </summary>
    ''' <returns>対象レコード件数</returns>
    ''' <remarks></remarks>
    Private Function HassinDataList() As Integer

        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim row As Integer = 0
        Dim Data(10) As String

        Try
            With SQL
                .Append("select *")
                .Append(" from S_SCHMAST")
                .Append(" inner join S_TORIMAST")
                .Append(" on TORIS_CODE_S = TORIS_CODE_T")
                .Append(" and TORIF_CODE_S = TORIF_CODE_T")
                .Append(" where FSYORI_KBN_S = '3'")
                .Append(" and TOUROKU_FLG_S = '1'")
                .Append(" and HASSIN_FLG_S = '0'")
                .Append(" and TYUUDAN_FLG_S = '0'")
                .Append(" and SOUSIN_KBN_S = '2'")          'CSVリエンタ
                .Append(" and ((SYUBETU_S in ('11', '12') and FURI_DATE_S = '" & Me.KYUFURI_DATE & "')")
                .Append(" or   (SYUBETU_S = '21'          and FURI_DATE_S = '" & Me.SOUFURI_DATE & "'))")
                .Append(" order by")
                .Append(" FURI_DATE_S")
                .Append(",SOUSIN_KBN_S")
                .Append(",TORIS_CODE_S")
                .Append(",TORIF_CODE_S")
                .Append(",MOTIKOMI_SEQ_S")
            End With

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False

                    row += 1

                    '項番
                    Data(1) = row.ToString("#,##0")
                    '取引先名
                    Data(2) = GCom.GetLimitString(OraReader.GetString("ITAKU_NNAME_T"), 40)
                    '取引先コード（主コード + "-" + 副コード）
                    Data(3) = OraReader.GetString("TORIS_CODE_T") & "-" & OraReader.GetString("TORIF_CODE_T")
                    '振込日
                    Dim furidate As String = OraReader.GetString("FURI_DATE_S")
                    Data(7) = furidate.Substring(0, 4) & "/" & furidate.Substring(4, 2) & "/" & furidate.Substring(6, 2)

                    '公金区分取得
                    Dim koukin As Boolean = False
                    Select Case OraReader.GetString("SYUMOKU_CODE_T")
                        Case "01", "02", "03", "04"
                            '国庫金、公金、公金(指定日決済)、公金(指定日前営業日決済)
                            koukin = True
                        Case Else
                            '一般、株式配当金(一般)、株式配当金(自行)、貸付配当収益配当金、年金給付金(年金信託)、年金給付金(公的年金)、年金給付金(医療保険)
                            koukin = False
                    End Select

                    '契約種別
                    Select Case OraReader.GetString("SYUBETU_S")
                        Case "11"
                            Data(4) = "給与"
                        Case "12"
                            Data(4) = "賞与"
                        Case Else
                            Data(4) = "振込"
                    End Select

                    If koukin Then
                        Data(4) &= "公金"
                    End If

                    '適用種別
                    Select Case OraReader.GetString("SYUBETU_S")
                        Case "11"
                            Data(5) = "給与"
                        Case "12"
                            Data(5) = "賞与"
                        Case Else
                            Data(5) = "振込"
                    End Select

                    If koukin Then
                        Data(5) &= "公金"
                    End If

                    '送信区分
                    Data(6) = "CSVﾘｴﾝﾀ"

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
                End While
            Else
                Return 0
            End If

            Return row

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(データ検索)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return -1

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' テキストボックスの必須チェックを行います。
    ''' </summary>
    ''' <returns>True - 正常 , False - 異常</returns>
    ''' <remarks></remarks>
    Private Function fn_check_text() As Boolean
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)開始", "成功", "")

            '発信日（年）チェック
            If Me.txtKijyunDateY.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateY.Focus()
                Return False
            End If

            '発信日（月）チェック
            If Me.txtKijyunDateM.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateM.Focus()
                Return False
            End If

            If Not (GCom.NzInt(Me.txtKijyunDateM.Text) >= 1 And GCom.NzInt(Me.txtKijyunDateM.Text) <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateM.Focus()
                Return False
            End If

            '発信日（日）チェック
            If Me.txtKijyunDateD.Text.Trim = String.Empty Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateD.Focus()
                Return False
            End If

            If Not (GCom.NzInt(Me.txtKijyunDateD.Text) >= 1 And GCom.NzInt(Me.txtKijyunDateD.Text) <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateD.Focus()
                Return False
            End If

            '発信日日付整合性チェック
            Dim WORK_DATE As String = String.Concat(New String() {Me.txtKijyunDateY.Text, "/", Me.txtKijyunDateM.Text, "/", Me.txtKijyunDateD.Text})
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKijyunDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)終了", "成功", "")
        End Try
    End Function

#End Region

End Class

