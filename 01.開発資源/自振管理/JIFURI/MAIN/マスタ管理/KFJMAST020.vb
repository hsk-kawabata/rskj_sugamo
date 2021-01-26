Imports System.Data.OracleClient
Imports CASTCommon

Public Class KFJMAST020

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private CLS As ClsSchduleMaintenanceClass

    Private Enum OPT
        OptionAddNew = 1                '新規・再作成
        OptionAppend = 2                '追加作成
    End Enum

    Private SEL_OPTION As Integer       'ラジオボタン選択状態
    Private FormFuriDate As Date        '振替日入力状態値

    Dim SortOrderFlag As Boolean = True             'ソートオーダーフラグ
    Dim ClickedColumn As Integer                    'クリックした列の番号

    Private MyOwnerForm As Form
    Private Const ThisModuleName As String = "KFJMAST020.vb"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAST020", "月間スケジュール作成画面")
    Private Const msgTitle As String = "月間スケジュール作成画面(KFJMAST020)"

    '画面LOAD時
    Private Sub KFJMAST020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            MyOwnerForm = GOwnerForm
            GOwnerForm = Me

            'ユーザID／システム日付表示
            Call GCom.SetMonitorTopArea(Label2, Label3, lbluser, lblDate)

            CLS = New ClsSchduleMaintenanceClass
            CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            '休日情報表示設定
            Call CLS.Set_Kyuuzitu_Monitor_Area(Me.ListView1)

            'SCHMAST項目名の蓄積
            CLS.SetSchMastInformation()

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

    Private Sub KFJMAST020_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed

        Try
            GOwnerForm = MyOwnerForm
            GOwnerForm.Visible = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        End Try

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        Finally
            'MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub

    '振替日（年月）入力後処置
    Private Sub FURI_DATE_S_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles FURI_DATE_S.Validating, FURI_DATE_S1.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振替日（年月）入力後処置)例外エラー", "失敗", ex.Message)
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
        'Dim Cursor As Cursor
        Dim MsgIcon As System.Windows.Forms.MessageBoxIcon = MessageBoxIcon.Warning

        Dim wrkTorifCode As String = ""     '再振用
        Dim wrkSfuriFlg As Integer          '再振用
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

            '日付入力値チェック(ここで振替年月を確定し変数へ設定する)
            onText(0) = GCom.NzInt(FURI_DATE_S.Text, 0)
            onText(1) = GCom.NzInt(FURI_DATE_S1.Text, 0)
            onText(2) = 1

            'FormFuriDate = 確定的な振替年月値
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

                Call CmdAction_Click_Sub()

                MSG = MSG0052W
                CTL = FURI_DATE_S
                Return
            End If

            '*** デッドロック対応 ***
            '処理中、起動待ちのジョブが存在する場合は処理を中断する
            If CLS.GetJobCount > 0 Then
                MSG = MSG0279W
                Return
            End If
            '**********************************************

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
            'Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

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
                LW.ToriCode = CLS.TR(Index).TORIS_CODE & CLS.TR(Index).TORIF_CODE

                GCom.GLog.ToriCode = CLS.TR(Index).TORIS_CODE.PadLeft(10, "0"c) & "-"
                GCom.GLog.ToriCode &= CLS.TR(Index).TORIF_CODE.PadLeft(2, "0"c)

                MSG = String.Format(MSG0027E, "スケジュールマスタ", "登録")

                '最大初振日
                Dim MaxFuriDate As String = ""

                '契約日ベースのスケジュールを作成する
                For DayCnt As Integer = 1 To 31 Step 1

                    If CLS.TR(Index).DATEN(DayCnt) > 0 Then

                        '契約振替日
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

                        '2008.02.22 By Astar
                        Select Case CLS.TR(Index).FURI_KYU_CODE
                            Case 0, 1
                                '振替日,営業日判定(土・日・祝祭日判定)
                                BRet = GCom.CheckDateModule(CLS.SCH.KFURI_DATE, _
                                                CLS.SCH.FURI_DATE, CLS.TR(Index).FURI_KYU_CODE)
                            Case Else
                                '振替日,営業日判定(土・日・祝祭日判定)
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

                        If CLS.CHECK_SELECT_SCHMAST(Index) Then

                            If Not CLS.INSERT_NEW_SCHMAST(Index, False) Then

                                MsgIcon = MessageBoxIcon.Error
                                CTL = Me.FURI_DATE_S

                                Throw New Exception(MSG & Space(5))

                            Else

                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "成功", "スケジュール登録")

                            End If
                        End If

                        If GCom.NzDec(MaxFuriDate) < GCom.NzDec(CLS.SCH.FURI_DATE) Then

                            MaxFuriDate = CLS.SCH.FURI_DATE

                        End If

                        '再振ありで営業日数指定の再振の場合
                        '2008.01.16 時点では初振／再振識別をしないため存在すれば作成しない仕様とする
                        If CLS.TR(Index).SFURI_FLG = 1 AndAlso CLS.TR(Index).SFURI_KIJITSU = 0 Then

                            '営業日数の再振スケジュールを作成する
                            BRet = GCom.CheckDateModule(CLS.SCH.FURI_DATE, _
                                    CLS.SCH.FURI_DATE, CLS.TR(Index).SFURI_DAY, 0)

                            '2008.02.22 By Astar 月末超え防止指定の場合
                            Select Case CLS.TR(Index).FURI_KYU_CODE
                                Case 0, 1
                                Case Else
                                    If GCom.NzInt(CLS.SCH.KFURI_DATE.Substring(4, 2)) < _
                                        GCom.NzInt(CLS.SCH.FURI_DATE.Substring(4, 2)) Then

                                        BRet = GCom.CheckDateModule(CLS.SCH.FURI_DATE, CLS.SCH.FURI_DATE, 1)
                                    End If
                            End Select

                            '初振と再振が重なる場合にはエラーログを出力する
                            If GCom.NzDec(MaxFuriDate) < GCom.NzDec(CLS.SCH.FURI_DATE) Then
                                '作成できる場合

                                GCom.GLog.FuriDate = CLS.SCH.FURI_DATE

                                '***
                                '再振2009.10.05
                                '副コード・再振フラグをすりかえる
                                wrkTorifCode = CLS.TR(Index).TORIF_CODE
                                wrkSfuriFlg = CLS.TR(Index).SFURI_FLG
                                CLS.TR(Index).TORIF_CODE = CLS.TR(Index).SFURI_FCODE
                                CLS.TR(Index).SFURI_FLG = 0

                                LW.ToriCode = String.Concat(CLS.TR(Index).TORIS_CODE, CLS.TR(Index).TORIF_CODE)
                                LW.FuriDate = CLS.SCH.FURI_DATE
                                '***

                                If CLS.CHECK_SELECT_SCHMAST(Index) Then

                                    If Not CLS.INSERT_NEW_SCHMAST(Index, False) Then

                                        MsgIcon = MessageBoxIcon.Error
                                        CTL = Me.FURI_DATE_S

                                        Throw New Exception(MSG)
                                    Else
                                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "成功", "スケジュール登録(再振)")
                                    End If
                                End If

                                '***
                                '再振2009.10.05
                                '副コードをもどす
                                CLS.TR(Index).TORIF_CODE = wrkTorifCode
                                CLS.TR(Index).SFURI_FLG = wrkSfuriFlg

                                LW.ToriCode = String.Concat(CLS.TR(Index).TORIS_CODE, CLS.TR(Index).TORIF_CODE)
                                LW.FuriDate = CLS.SCH.FURI_DATE
                                '***

                            Else
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "失敗", "再振スケジュール:初振(" & MaxFuriDate & ") >= 再振(" & CLS.SCH.FURI_DATE & ")")
                            End If
                        End If
                    End If
                Next DayCnt

                ' 2017/05/08 タスク）綾部 DEL 【ME】標準バグ対応(再振ｽｹｼﾞｭｰﾙのDB不整合改修) -------------------- START
                ''再振有りで「基準日」の場合、最後に作成する。
                ''最大初振と再振が重なる場合にはエラーログを出力する
                'If CLS.TR(Index).SFURI_FLG = 1 AndAlso CLS.TR(Index).SFURI_KIJITSU = 1 Then

                '    '再振のスケジュールを作成する
                '    onText(0) = FormFuriDate.Year
                '    onText(1) = FormFuriDate.Month
                '    onText(2) = CLS.TR(Index).SFURI_DAY
                '    Ret = GCom.SET_DATE(onDate, onText)
                '    If Not Ret = -1 Then
                '        '月末補正(月末指定の場合実日に変換する) 2008.04.18 Update By Astar
                '        onText(0) = onDate.Year
                '        onText(1) = onDate.Month
                '        onText(2) = 1
                '        Ret = GCom.SET_DATE(onDate, onText)
                '        onDate = onDate.AddDays(-1)
                '    End If
                '    '2012/06/05 標準版修正
                '    'CLS.SCH.KFURI_DATE = String.Format("{0:yyyyMMdd}", onDate)
                '    CLS.SCH.FURI_DATE = String.Format("{0:yyyyMMdd}", onDate)

                '    '2008.02.22 By Astar
                '    Select Case CLS.TR(Index).FURI_KYU_CODE
                '        Case 0, 1
                '            '振替日,営業日判定(土・日・祝祭日判定)
                '            BRet = GCom.CheckDateModule(CLS.SCH.FURI_DATE, _
                '                            CLS.SCH.FURI_DATE, CLS.TR(Index).SFURI_KYU_CODE)
                '            '2012/06/05 標準版修正　追加--------------------------------------------------->
                '            Dim Temp As String
                '            '振替日より手前の場合には翌月へ再計算
                '            If MaxFuriDate >= CLS.SCH.FURI_DATE Then
                '                onDate = GCom.SET_DATE(CLS.SCH.FURI_DATE).AddMonths(1)
                '                onText(0) = onDate.Year
                '                onText(1) = onDate.Month
                '                onText(2) = CLS.TR(Index).SFURI_DAY
                '                Ret = GCom.SET_DATE(onDate, onText)
                '                If Not Ret = -1 Then
                '                    onText(0) = onDate.Year
                '                    onText(1) = onDate.Month
                '                    onText(2) = 1
                '                    Ret = GCom.SET_DATE(onDate, onText)
                '                    onDate = onDate.AddDays(-1)
                '                End If
                '                Temp = String.Format("{0:yyyyMMdd}", onDate)

                '                '振替日,営業日判定(土・日・祝祭日判定)
                '                BRet = GCom.CheckDateModule(Temp, _
                '                                CLS.SCH.FURI_DATE, CLS.TR(Index).SFURI_KYU_CODE)
                '            End If
                '            '2012/06/05 標準版修正　追加--------------------------------------------------------<
                '        Case Else
                '            '振替日,営業日判定(土・日・祝祭日判定)
                '            BRet = GCom.CheckDateModule(CLS.SCH.FURI_DATE, CLS.SCH.FURI_DATE, 0)
                '            '2012/06/05 標準版修正　追加--------------------------------------------------->
                '            'If GCom.NzInt(CLS.SCH.KFURI_DATE.Substring(4, 2)) < _
                '            '    GCom.NzInt(CLS.SCH.FURI_DATE.Substring(4, 2)) Then

                '            '    BRet = GCom.CheckDateModule(CLS.SCH.KFURI_DATE, CLS.SCH.FURI_DATE, 1)
                '            'End If
                '            '再振休日コードが「2」翌営業日振替(月跨ぎ時前営業日)では同月を原則とする。
                '            Dim Temp As String
                '            If GCom.NzInt(CLS.SCH.KFURI_DATE.Substring(0, 6), 0) < _
                '               GCom.NzInt(CLS.SCH.FURI_DATE.Substring(0, 6), 0) Then

                '                Temp = CLS.SCH.FURI_DATE

                '                '月が異なる場合に前月最終営業日へ補正する。
                '                onText(0) = GCom.NzInt(CLS.SCH.FURI_DATE.Substring(0, 4), 0)
                '                onText(1) = GCom.NzInt(CLS.SCH.FURI_DATE.Substring(4, 2), 0)
                '                onText(2) = 1
                '                Ret = GCom.SET_DATE(onDate, onText)
                '                BRet = GCom.CheckDateModule(onDate.AddDays(-1).ToString("yyyyMMdd"), _
                '                                                CLS.SCH.FURI_DATE, 1)

                '                If CLS.SCH.KFURI_DATE >= CLS.SCH.FURI_DATE Then
                '                    '振替日より手前の場合には再計算を破棄する

                '                    CLS.SCH.FURI_DATE = Temp
                '                End If
                '            End If
                '            '2012/06/05 標準版修正　追加---------------------------------------------------<
                '    End Select

                '    If GCom.NzDec(MaxFuriDate) < GCom.NzDec(CLS.SCH.FURI_DATE) Then
                '        '作成できる場合

                '        GCom.GLog.FuriDate = CLS.SCH.FURI_DATE

                '        '***
                '        '再振2009.10.05
                '        '副コード・再振フラグをすりかえる
                '        wrkTorifCode = CLS.TR(Index).TORIF_CODE
                '        wrkSfuriFlg = CLS.TR(Index).SFURI_FLG
                '        CLS.TR(Index).TORIF_CODE = CLS.TR(Index).SFURI_FCODE
                '        CLS.TR(Index).SFURI_FLG = 0

                '        LW.ToriCode = String.Concat(CLS.TR(Index).TORIS_CODE, CLS.TR(Index).TORIF_CODE)
                '        LW.FuriDate = CLS.SCH.FURI_DATE
                '        '***

                '        If CLS.CHECK_SELECT_SCHMAST(Index) Then

                '            If Not CLS.INSERT_NEW_SCHMAST(Index, False) Then

                '                MsgIcon = MessageBoxIcon.Error
                '                CTL = Me.FURI_DATE_S

                '                Throw New Exception(MSG)
                '            Else
                '                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "成功", "スケジュール登録(再振)")
                '            End If
                '        End If

                '        '***
                '        '再振2009.10.05
                '        '副コードをもどす
                '        CLS.TR(Index).TORIF_CODE = wrkTorifCode
                '        CLS.TR(Index).SFURI_FLG = wrkSfuriFlg

                '        LW.ToriCode = String.Concat(CLS.TR(Index).TORIS_CODE, CLS.TR(Index).TORIF_CODE)
                '        LW.FuriDate = CLS.SCH.FURI_DATE
                '        '***

                '    Else

                '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "失敗", "再振スケジュール:初振(" & MaxFuriDate & ") >= 再振(" & CLS.SCH.FURI_DATE & ")")
                '    End If
                'End If

                ''2012/06/05 標準版修正　追加--------------------------------------------------->
                ''再振有りで「翌月基準日」の場合
                ''最大初振と再振が重なる場合にはエラーログを出力する
                'If CLS.TR(Index).SFURI_FLG = 1 AndAlso CLS.TR(Index).SFURI_KIJITSU = 2 Then

                '    onDate = FormFuriDate.AddMonths(1)

                '    '再振のスケジュールを作成する
                '    onText(0) = onDate.Year
                '    onText(1) = onDate.Month
                '    onText(2) = CLS.TR(Index).SFURI_DAY
                '    Ret = GCom.SET_DATE(onDate, onText)
                '    If Not Ret = -1 Then
                '        '月末補正(月末指定の場合実日に変換する) 2008.04.18 Update By Astar
                '        onText(0) = onDate.Year
                '        onText(1) = onDate.Month
                '        onText(2) = 1
                '        Ret = GCom.SET_DATE(onDate, onText)
                '        onDate = onDate.AddDays(-1)
                '    End If
                '    CLS.SCH.FURI_DATE = String.Format("{0:yyyyMMdd}", onDate)

                '    '2008.02.22 By Astar
                '    Select Case CLS.TR(Index).FURI_KYU_CODE
                '        Case 0, 1
                '            '振替日,営業日判定(土・日・祝祭日判定)
                '            BRet = GCom.CheckDateModule(CLS.SCH.FURI_DATE, _
                '                            CLS.SCH.FURI_DATE, CLS.TR(Index).SFURI_KYU_CODE)
                '        Case Else
                '            '振替日,営業日判定(土・日・祝祭日判定)
                '            BRet = GCom.CheckDateModule(CLS.SCH.KFURI_DATE, CLS.SCH.FURI_DATE, 0)
                '            'If GCom.NzInt(CLS.SCH.KFURI_DATE.Substring(4, 2)) < _
                '            '    GCom.NzInt(CLS.SCH.FURI_DATE.Substring(4, 2)) Then

                '            '    BRet = GCom.CheckDateModule(CLS.SCH.KFURI_DATE, CLS.SCH.FURI_DATE, 1)
                '            'End If
                '            '再振休日コードが「2」翌営業日振替(月跨ぎ時前営業日)では同月を原則とする。
                '            Dim Temp As String
                '            If GCom.NzInt(CLS.SCH.KFURI_DATE.Substring(0, 6), 0) < _
                '               GCom.NzInt(CLS.SCH.FURI_DATE.Substring(0, 6), 0) Then

                '                Temp = CLS.SCH.FURI_DATE

                '                '月が異なる場合に前月最終営業日へ補正する。
                '                onText(0) = GCom.NzInt(CLS.SCH.FURI_DATE.Substring(0, 4), 0)
                '                onText(1) = GCom.NzInt(CLS.SCH.FURI_DATE.Substring(4, 2), 0)
                '                onText(2) = 1
                '                Ret = GCom.SET_DATE(onDate, onText)
                '                BRet = GCom.CheckDateModule(onDate.AddDays(-1).ToString("yyyyMMdd"), _
                '                                                CLS.SCH.FURI_DATE, 1)

                '                If CLS.SCH.KFURI_DATE >= CLS.SCH.FURI_DATE Then
                '                    '振替日より手前の場合には再計算を破棄する

                '                    CLS.SCH.FURI_DATE = Temp
                '                End If
                '            End If
                '    End Select

                '    If GCom.NzDec(MaxFuriDate) < GCom.NzDec(CLS.SCH.FURI_DATE) Then
                '        '作成できる場合

                '        GCom.GLog.FuriDate = CLS.SCH.FURI_DATE

                '        '***
                '        '再振2009.10.05
                '        '副コード・再振フラグをすりかえる
                '        wrkTorifCode = CLS.TR(Index).TORIF_CODE
                '        wrkSfuriFlg = CLS.TR(Index).SFURI_FLG
                '        CLS.TR(Index).TORIF_CODE = CLS.TR(Index).SFURI_FCODE
                '        CLS.TR(Index).SFURI_FLG = 0

                '        LW.ToriCode = String.Concat(CLS.TR(Index).TORIS_CODE, CLS.TR(Index).TORIF_CODE)
                '        LW.FuriDate = CLS.SCH.FURI_DATE
                '        '***

                '        If CLS.CHECK_SELECT_SCHMAST(Index) Then

                '            If Not CLS.INSERT_NEW_SCHMAST(Index, False) Then

                '                MsgIcon = MessageBoxIcon.Error
                '                CTL = Me.FURI_DATE_S

                '                Throw New Exception(MSG)
                '            Else
                '                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "成功", "スケジュール登録(再振)")
                '            End If
                '        End If

                '        '***
                '        '再振2009.10.05
                '        '副コードをもどす
                '        CLS.TR(Index).TORIF_CODE = wrkTorifCode
                '        CLS.TR(Index).SFURI_FLG = wrkSfuriFlg

                '        LW.ToriCode = String.Concat(CLS.TR(Index).TORIS_CODE, CLS.TR(Index).TORIF_CODE)
                '        LW.FuriDate = CLS.SCH.FURI_DATE
                '        '***

                '    Else

                '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(作成)", "失敗", "再振スケジュール:初振(" & MaxFuriDate & ") >= 再振(" & CLS.SCH.FURI_DATE & ")")
                '    End If
                'End If
                ''2012/06/05 標準版修正　追加--------------------------------------------------<
                ' 2017/05/08 タスク）綾部 DEL 【ME】標準バグ対応(再振ｽｹｼﾞｭｰﾙのDB不整合改修) -------------------- END

                'KOUSIN_SIKIBETU_T フラグを 2 に更新
                If Not CLS.UPDATE_TORIMAST(Index) Then

                    MSG = String.Format(MSG0027E, "取引先マスタ", "更新")
                    MsgIcon = MessageBoxIcon.Error
                    CTL = Me.FURI_DATE_S
                    Return
                End If
            Next Index

            Call CmdAction_Click_Sub()

            ' 2017/03/29 タスク）綾部 ADD 【ME】標準バグ対応(随時再振ｽｹｼﾞｭｰﾙの削除補正) -------------------- START
            ' スケジュールメンテナンス画面より、再振契約ありの取引先の随時スケジュールについて
            ' 初回振替スケジュール進行中の場合、初回振替スケジュールは残るが、再振の未進行スケジュールが
            ' 削除されてしまうため、スケジュールの再作成もできなくなるため、月間スケジュールの最後に、
            ' 削除されたスケジュールのマスタ不整合スケジュールを作成する。
            '
            ' 当事象の対象先は、再振契約ありの営業日数指定および、日数指定（SKS）のみ
            '
            If SfuriSchduleReMake() = False Then
                MSG = String.Format(MSG0027E, "随時再振スケジュール", "作成")
                MsgIcon = MessageBoxIcon.Error
                CTL = Me.FURI_DATE_S
                Return
            End If
            ' 2017/03/29 タスク）綾部 ADD 【ME】標準バグ対応(随時再振ｽｹｼﾞｭｰﾙの削除補正) -------------------- END

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
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CmdBack.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")

            Me.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "成功", ex.ToString)
        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")

        End Try

    End Sub

    '
    ' 機能　 ： 作成ボタン押下(サブ関数)
    '
    ' 引数　 ： ARG1 - なし
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： 本来「作成ボタン押下」内に記述されていた部分の分離を行う。
    '
    ' 　　　 　 前月の月末スケジュールが当月振替日になった場合、
    ' 　　　 　 スケジュールが削除されてしまっている。(らしい。)
    ' 　　　 　 依って、前月の26日～31日のスケジュールを作成する。(由)
    '
    Private Sub CmdAction_Click_Sub()

        '2007.12.25 FJH金沢事業所での打合せの結果運用を中止する。(Astar)
        '自振側から学校自振のスケジュールを更新しない。(仕様：西田様)
        If True Then
            Return
        End If

        Dim Ret As Integer
        Dim SQL As String
        Dim Temp As String
        Dim onText(2) As Integer
        Dim REC As OracleDataReader = Nothing

        'With GCom.GLog
        '    .Job2 = "作成後サブ関数処理"
        '    .Result = "開始"
        '    .Discription = "処理対象月：" & FormFuriDate.Year & "年" & FormFuriDate.Month & "月"
        'End With
        'Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))

        Try
            '休日情報の蓄積(該当月の休日だけを蓄積する)
            Call CLS.SetKyuzituInformation(FormFuriDate)

            '----------------------------------------------------------------------
            '' 学校自振(諸会費)関連処理
            '----------------------------------------------------------------------

            '学校自振スケジュールの初期化()
            Temp = String.Format("{0:yyyyMM}", FormFuriDate)

            SQL = "UPDATE SCHMAST"
            SQL &= " SET YUUKOU_FLG_S = '0'"
            SQL &= " WHERE SUBSTR(FURI_DATE_S, 1, 6) = '" & Temp & "'"
            SQL &= " AND UKETUKE_FLG_S = '0'"
            SQL &= " AND TOUROKU_FLG_S = '0'"
            SQL &= " AND NIPPO_FLG_S = '0'"
            SQL &= " AND TYUUDAN_FLG_S = '0'"
            SQL &= " AND BAITAI_CODE_S = '7'"

            Dim SQLCode As Integer
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)

            '学校スケジュールから有効フラグを立てる学校先を抽出
            SQL = "SELECT DISTINCT GAKKOU_CODE_S"
            SQL &= ", FURI_KBN_S"
            SQL &= ", FURI_DATE_S"
            SQL &= " FROM G_SCHMAST"
            SQL &= " WHERE SUBSTR(FURI_DATE_S, 1, 6) = '" & Temp & "'"

            If GCom.SetDynaset(SQL, REC) Then

                Do While REC.Read

                    '学校コード = 取引先主コード
                    Dim GAK_CODE As String = GCom.NzDec(REC.Item("GAKKOU_CODE_S"), "")

                    '振替区分 + 1 = 取引先副コード
                    Dim GAK_FURI_KBN As Integer = GCom.NzInt(REC.Item("FURI_KBN_S"), 0)

                    '学校スケジュールの振替日
                    Dim GAK_FURI_DATE As String = GCom.NzDec(REC.Item("FURI_DATE_S"), "")

                    If GAK_CODE.Trim.Length > 0 Then

                        SQL = "UPDATE SCHMAST"
                        SQL &= " SET YUUKOU_FLG_S = '1'"
                        SQL &= " WHERE FURI_DATE_S = '" & GAK_FURI_DATE & "'"
                        SQL &= " AND UKETUKE_FLG_S = '0'"
                        SQL &= " AND TOUROKU_FLG_S = '0'"
                        SQL &= " AND NIPPO_FLG_S = '0'"
                        SQL &= " AND TYUUDAN_FLG_S = '0'"
                        SQL &= " AND BAITAI_CODE_S = '7'"
                        SQL &= " AND TORIS_CODE_S = '" & GAK_CODE & "'"

                        Temp = String.Format("{0:00}", (GAK_FURI_KBN + 1))
                        SQL &= " AND TORIF_CODE_S = '" & Temp & "'"

                        Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL)
                    End If
                Loop
            End If
        Catch ex As Exception
            Throw
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        GCom.GLog.Result = MenteCommon.clsCommon.OK
        'Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    End Sub

    Private Function SfuriSchduleReMake() As Boolean

        Dim REC As OracleDataReader = Nothing
        Dim SQL As String = String.Empty

        Dim REC_ReSearch As OracleDataReader = Nothing
        Dim SQL_ReSearch As String = String.Empty

        Dim ReMakeCount As Integer = 0

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振スケジュール作成", "開始", "")

            CLS = New ClsSchduleMaintenanceClass
            CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            '休日情報の蓄積
            Call CLS.SetKyuzituInformation()

            'SCHMAST項目名の蓄積
            Call CLS.SetSchMastInformation()

            Dim FuriDate_Month As String = FURI_DATE_S.Text & FURI_DATE_S1.Text

            SQL = "SELECT"
            SQL &= "      TORIS_CODE_S            TORIS_CODE_S"
            SQL &= "    , MIN(TORIF_CODE_S)       TORIF_CODE_S"
            SQL &= "    , MAX(KSAIFURI_DATE_S)    KSAIFURI_DATE_S"
            SQL &= "    , ITAKU_CODE_S            ITAKU_CODE_S"
            SQL &= "    , KFURI_DATE_S            KFURI_DATE_S"
            SQL &= "    , SFURI_FCODE_T           SFURI_FCODE_T"
            SQL &= "    , COUNT(*)                SCH_COUNT"
            SQL &= " FROM TORIMAST , SCHMAST"
            SQL &= " WHERE"
            SQL &= "      MOTIKOMI_KBN_T          = '0'"
            SQL &= "  AND BAITAI_CODE_T          <> '07'"
            SQL &= "  AND SFURI_FLG_T             = '1'"
            SQL &= "  AND ITAKU_CODE_S            = ITAKU_CODE_T"
            SQL &= "  AND TORIS_CODE_S            = TORIS_CODE_T"
            SQL &= "  AND KFURI_DATE_S         LIKE '" & FuriDate_Month & "%'"
            SQL &= " GROUP BY"
            SQL &= "      TORIS_CODE_S"
            SQL &= "    , ITAKU_CODE_S"
            SQL &= "    , KFURI_DATE_S"
            SQL &= "    , SFURI_FCODE_T"
            SQL &= " ORDER BY"
            SQL &= "      TORIS_CODE_S"
            SQL &= "    , ITAKU_CODE_S"
            SQL &= "    , KFURI_DATE_S"

            If GCom.SetDynaset(SQL, REC) Then
                Do While REC.Read
                    If GCom.NzInt(REC.Item("SCH_COUNT"), 0) = 1 Then

                        SQL_ReSearch = "SELECT"
                        SQL_ReSearch &= "      FURI_DATE_S"
                        SQL_ReSearch &= " FROM SCHMAST"
                        SQL_ReSearch &= " WHERE"
                        SQL_ReSearch &= "      TORIS_CODE_S = '" & GCom.NzDec(REC.Item("TORIS_CODE_S"), "") & "'"
                        SQL_ReSearch &= "  AND TORIF_CODE_S = '" & GCom.NzDec(REC.Item("SFURI_FCODE_T"), "") & "'"
                        SQL_ReSearch &= "  AND FURI_DATE_S  = '" & GCom.NzDec(REC.Item("KSAIFURI_DATE_S"), "") & "'"

                        If Not GCom.SetDynaset(SQL_ReSearch, REC_ReSearch) Then

                            '取引先マスタに取引先コードが存在することを確認
                            Dim BRet As Boolean = CLS.GET_SELECT_TORIMAST(Nothing, _
                                                                          GCom.NzDec(REC.Item("TORIS_CODE_S"), ""), _
                                                                          GCom.NzDec(REC.Item("TORIF_CODE_S"), ""), _
                                                                          ClsSchduleMaintenanceClass.OPT.OptionNothing)

                            Dim ToriCode As String = GCom.NzDec(REC.Item("TORIS_CODE_S"), "") & "-" & GCom.NzDec(REC.Item("TORIF_CODE_S"), "")
                            CLS.TR(0).TORIF_CODE = CLS.TR(0).SFURI_FCODE
                            CLS.TR(0).SFURI_FLG = 0
                            CLS.SCH.KFURI_DATE = GCom.NzDec(REC.Item("KFURI_DATE_S"), "")
                            CLS.SCH.FURI_DATE = GCom.NzDec(REC.Item("KSAIFURI_DATE_S"), "")

                            If CLS.INSERT_NEW_SCHMAST(0) = False Then
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, _
                                              "再振スケジュール作成", "失敗", _
                                              " 取引先コード:" & ToriCode & _
                                              " 契約振替日:" & GCom.NzDec(REC.Item("KFURI_DATE_S"), "") & _
                                              " 再振替日:" & CLS.SCH.FURI_DATE)
                                Return False
                            Else
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, _
                                             "再振スケジュール作成", "成功", _
                                             " 取引先コード:" & ToriCode & _
                                             " 契約振替日:" & GCom.NzDec(REC.Item("KFURI_DATE_S"), "") & _
                                             " 再振替日:" & CLS.SCH.FURI_DATE)
                                ReMakeCount += 1
                            End If
                        End If
                    End If
                Loop
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振スケジュール作成", "成功", "再作成件数:" & ReMakeCount & " 件")
            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振スケジュール作成", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振スケジュール作成", "終了", "")
        End Try

    End Function

End Class