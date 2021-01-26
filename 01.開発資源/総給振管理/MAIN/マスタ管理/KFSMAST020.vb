Option Strict On
Option Explicit On

Imports CASTCommon
Imports CAstFormatMini

Public Class KFSMAST020

#Region "変数宣言"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private CLS As ClsSchduleMaintenanceClass

    Private Enum OPT
        OptionAddNew = 1                '新規・再作成
        OptionAppend = 2                '追加作成
    End Enum

    Private SEL_OPTION As Integer       'ラジオボタン選択状態
    Private FormFuriDate As Date        '振込日入力状態値

    Dim SortOrderFlag As Boolean = True             'ソートオーダーフラグ
    Dim ClickedColumn As Integer                    'クリックした列の番号

    Private MyOwnerForm As Form

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFSMAST020", "月間スケジュール作成画面")
    Private Const msgTitle As String = "月間スケジュール作成画面(KFSMAST020)"
#End Region

    '画面LOAD時
    Private Sub KFSMAST020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            MyOwnerForm = GOwnerForm
            GOwnerForm = Me

            'ユーザID／システム日付表示
            Call GCom.SetMonitorTopArea(Label2, Label3, lbluser, lblDate)

            Comm.GCom = GCom
            CLS = New ClsSchduleMaintenanceClass

            '休日情報表示設定
            Call CLS.Set_Kyuuzitu_Monitor_Area(Me.ListView1)

            'SCHMAST項目名の蓄積
            CLS.SetSchMastInformation()

            '持込SEQは1から
            CLS.SCH.MOTIKOMI_SEQ = 1

            Application.DoEvents()
            Me.FURI_DATE_S.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    '画面終了時処理
    Private Sub KFSMAST020_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed

        Try
            GOwnerForm = MyOwnerForm
            GOwnerForm.Visible = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        End Try

    End Sub

    '振込日（年月）入力後処置
    Private Sub FURI_DATE_S_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles FURI_DATE_S.Validating, FURI_DATE_S1.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振込日（年月）入力後処置)例外エラー", "失敗", ex.Message)
        End Try


    End Sub

    '
    ' 機能　 ： コマンドボタン・ミスタッチ防止関数
    '
    ' 引数　 ： ARG1 - 防止／解放
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： なし
    '
    Private Sub SetButtonEnabled(Optional ByVal SEL As Short = 1)
        Try
            Static memStatus() As Boolean
            Dim onControl() As Control = {Me.FURI_DATE_S, Me.FURI_DATE_S1, _
                    Me.opAddNew, Me.opAppend, Me.GroupBox1, Me.CmdAction, Me.CmdBack}
            Static MaxIndex As Integer = onControl.GetUpperBound(0)
            Select Case SEL
                Case 0
                    ReDim memStatus(MaxIndex)
                    For Index As Integer = 0 To MaxIndex Step 1
                        memStatus(Index) = onControl(Index).Enabled
                        If memStatus(Index) Then
                            onControl(Index).Enabled = False
                        End If
                    Next Index
                Case Else
                    For Index As Integer = 0 To MaxIndex Step 1
                        If memStatus(Index) Then
                            onControl(Index).Enabled = True
                        End If
                    Next Index
                    Me.FURI_DATE_S.Focus()
            End Select
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ミスタッチ防止関数)例外エラー", "失敗", ex.Message)
        End Try
    End Sub

    '作成ボタン処理
    Private Sub CmdAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction.Click
        Dim MSG As String = ""
        Dim Ret As Integer = 0
        Dim CTL As Control = Nothing
        Dim BRet As Boolean
        Dim onDate As Date
        Dim onText(2) As Integer
        Dim MsgIcon As System.Windows.Forms.MessageBoxIcon = MessageBoxIcon.Warning

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)開始", "成功", "")

            Call SetButtonEnabled(0)

            '処理設定値チェック
            Select Case True
                Case Me.opAddNew.Checked
                    '新規・再作成
                    SEL_OPTION = OPT.OptionAddNew
                Case Me.opAppend.Checked
                    '追加作成
                    SEL_OPTION = OPT.OptionAppend
                Case Else
                    CTL = Me.opAddNew
                    Return
            End Select

            '日付入力値チェック(ここで振込年月を確定し変数へ設定する)
            onText(0) = GCom.NzInt(FURI_DATE_S.Text, 0)
            onText(1) = GCom.NzInt(FURI_DATE_S1.Text, 0)
            onText(2) = 1

            'FormFuriDate = 確定的な振込年月値
            Ret = GCom.SET_DATE(FormFuriDate, onText)
            If Ret = -1 Then
                '2008.04.18 Update By Astar
                Call CLS.SetKyuzituInformation(MenteCommon.clsCommon.BadResultDate)
            Else
                Select Case Ret
                    Case 1
                        MSG = MSG0027W
                        CTL = FURI_DATE_S1
                    Case Else
                        MSG = MSG0027W
                        CTL = FURI_DATE_S
                End Select
                Return
            End If

            '取引先マスタ
            If Not CLS.GET_SELECT_TORIMAST(FormFuriDate, Nothing, Nothing, SEL_OPTION) Then

                MSG = MSG0052W
                CTL = FURI_DATE_S
                Return
            End If

            'デッドロック対応
            '処理中、起動待ちのジョブが存在する場合は処理を中断する
            If CLS.GetJobCount > 0 Then
                MSG = MSG0279W
                Return
            End If
 
            '作成確認メッセージ
            Select Case SEL_OPTION
                Case OPT.OptionAddNew
                    MSG = MSG0033I
                Case OPT.OptionAppend
                    MSG = MSG0034I
            End Select

            Dim dRet As DialogResult = MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            If Not dRet = DialogResult.OK Then
                MsgIcon = MessageBoxIcon.None
                CTL = Me.FURI_DATE_S
                Return
            End If
            Application.DoEvents()
            Cursor.Current = System.Windows.Forms.Cursors.WaitCursor

            'スケジュール作成処理開始ログ出力
            With GCom.GLog
                .ToriCode = String.Format("{0:000000000-00}", 0)
                .FuriDate = New String("0"c, 8)
                .Job2 = "スケジュール作成"
                .Result = "開始"
                .Discription = "処理対象月：" & FormFuriDate.Year & "年" & FormFuriDate.Month & "月"
            End With

            'トランザクション開始
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Begin)

            If SEL_OPTION = OPT.OptionAddNew Then

                'スケジュール削除処理
                If Not CLS.DELETE_SCHMAST(FormFuriDate) Then

                    MSG = String.Format(MSG0027E, "スケジュールマスタ", "削除")
                    MsgIcon = MessageBoxIcon.Error
                    CTL = Me.FURI_DATE_S
                    Return
                End If
            End If

            'スケジュールマスタ登録処置
            For Index As Integer = CLS.TR.GetLowerBound(0) To CLS.TR.GetUpperBound(0) Step 1

                '***ログ用***
                LW.ToriCode = CLS.TR(Index).TORIS_CODE & "-" & CLS.TR(Index).TORIF_CODE

                MSG = String.Format(MSG0027E, "スケジュールマスタ", "登録")

                Dim memFuriDate As String = ""

                For DayCnt As Integer = 1 To 31 Step 1

                    If CLS.TR(Index).DATEN(DayCnt) > 0 Then

                        '契約振込日
                        onText(0) = FormFuriDate.Year
                        onText(1) = FormFuriDate.Month
                        onText(2) = DayCnt
                        Ret = GCom.SET_DATE(onDate, onText)
                        If Not Ret = -1 Then
                            '月末補正(月末指定の場合実日に変換する)
                            onText(0) = onDate.Year
                            onText(1) = onDate.Month
                            onText(2) = 1
                            Ret = GCom.SET_DATE(onDate, onText)
                            onDate = onDate.AddDays(-1)
                        End If
                        CLS.SCH.KFURI_DATE = String.Format("{0:yyyyMMdd}", onDate)

                        '2008.02.25 By Astar
                        Select Case CLS.TR(Index).FURI_KYU_CODE
                            Case 0, 1
                                '振込日,営業日判定(土・日・祝祭日判定)
                                BRet = GCom.CheckDateModule(CLS.SCH.KFURI_DATE, _
                                                CLS.SCH.FURI_DATE, CLS.TR(Index).FURI_KYU_CODE)
                            Case Else
                                '振込日,営業日判定(土・日・祝祭日判定)
                                BRet = GCom.CheckDateModule(CLS.SCH.KFURI_DATE, CLS.SCH.FURI_DATE, 0)
                                If GCom.NzInt(CLS.SCH.KFURI_DATE.Substring(0, 6), 0) < _
                                    GCom.NzInt(CLS.SCH.FURI_DATE.Substring(0, 6), 0) Then

                                    Dim Temp As String = CLS.SCH.FURI_DATE
                                    BRet = GCom.CheckDateModule(Temp, CLS.SCH.FURI_DATE, 1, 1)
                                End If
                        End Select

                        '***ログ用***
                        LW.FuriDate = CLS.SCH.FURI_DATE

                        GCom.GLog.FuriDate = CLS.SCH.FURI_DATE

                        If CLS.CHECK_SELECT_SCHMAST(Index) AndAlso Not memFuriDate = CLS.SCH.FURI_DATE Then

                            If Not CLS.INSERT_NEW_SCHMAST(Index) Then

                                MsgIcon = MessageBoxIcon.Error
                                CTL = Me.FURI_DATE_S

                                Throw New Exception(MSG & Space(5))

                            Else

                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "成功", "スケジュール登録")

                            End If
                        End If
                        memFuriDate = CLS.SCH.FURI_DATE

                    End If
                Next DayCnt

                'KOUSIN_SIKIBETU_T フラグを 2 に更新
                If Not CLS.UPDATE_TORIMAST(Index) Then

                    MSG = String.Format(MSG0027E, "取引先マスタ", "更新")
                    MsgIcon = MessageBoxIcon.Error
                    CTL = Me.FURI_DATE_S
                    Return
                End If
            Next Index

            MSG = MSG0016I.Replace("{0}", "スケジュール作成")
            MsgIcon = MessageBoxIcon.Information
            CTL = Me.FURI_DATE_S
        Catch ex As Exception
            MSG = MSG0006E
            MsgIcon = MessageBoxIcon.Error
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)例外エラー", "失敗", ex.Message)
        Finally
            Call SetButtonEnabled()
            Cursor.Current = System.Windows.Forms.Cursors.Default
            Select Case MsgIcon
                Case MessageBoxIcon.Information
                    'トランザクション終了(コミットする)
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
                Case MessageBoxIcon.Error
                    'トランザクション終了(ロールバックする)
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
                Case MessageBoxIcon.Warning
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
            End Select
            Application.DoEvents()
            If Not CTL Is Nothing Then
                CTL.Focus()
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)終了", "成功", "")

        End Try
    End Sub

    '戻るボタン処理
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

    '一覧表示領域のソート
    Private Sub LetSort_ListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
    Handles ListView1.ColumnClick

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
    End Sub

End Class