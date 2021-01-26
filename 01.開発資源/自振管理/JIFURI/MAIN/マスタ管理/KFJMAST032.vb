Imports System
Imports System.Data.OracleClient
Imports System.Text
Imports CASTCommon

Public Class KFJMAST032

    '共通イベントコントロール
    Private CAST As New CASTCommon.Events
    Private CASTx1 As New CASTCommon.Events(" ", CASTCommon.Events.KeyMode.BAD)
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx02 As New CASTCommon.Events("0-2", CASTCommon.Events.KeyMode.GOOD)
    '**修正 2008/06/17 sakai *****************************************************
    '"0","1","9" を入力できるように変更
    Private CASTx03 As New CASTCommon.Events("0,1,9", CASTCommon.Events.KeyMode.GOOD)
    '******************************************************************************

    Private CLS As ClsSchduleMaintenanceClass

    Private Structure TORIMAST_KeyColumn
        Dim TORIS_CODE As String
        Dim TORIF_CODE As String
    End Structure
    Private TRKEY As TORIMAST_KeyColumn

    Private MyOwnerForm As Form
    Private Const ThisModuleName As String = "KFJMAST032.vb"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAST032", "スケジュールメンテナンス画面")
    Private Const msgTitle As String = "スケジュールメンテナンス画面(KFJMAST032)"
    Private MainDB As MyOracle

    '画面起動時イベント
    Private Sub KFJMAST0320G_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            CLS = New ClsSchduleMaintenanceClass
            CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            '休日情報の蓄積
            Call CLS.SetKyuzituInformation()

            'SCHMAST項目名の蓄積
            Call CLS.SetSchMastInformation()

            '画面クリア(初期化)
            Call Form_Initialize(True)

            '取引先コンボボックス設定
            If GCom.SelectItakuName("", cmbToriName, TORIS_CODE_S, TORIF_CODE_S, "1") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            Application.DoEvents()
            Me.TORIS_CODE_S.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

    '画面終了時処理
    Private Sub KFJMAST032_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed

        Try
            GOwnerForm = MyOwnerForm
            GOwnerForm.Visible = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        End Try

    End Sub

    '登録ボタン押下
    Private Sub CmdInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdInsert.Click
        Dim MSG As String = ""
        Dim Ret As Integer = 0
        Dim BRet As Boolean
        Dim DRet As DialogResult
        Dim SQLCode As Integer = 0
        Dim CTL As Control = Nothing
        Dim MsgIcon As System.Windows.Forms.MessageBoxIcon = MessageBoxIcon.Warning
        Dim wrkTorifCode As String = ""
        Dim wrkSfuriFlg As Integer
        Dim wrkFuriDate As String = ""

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")

            Call SetButtonEnabled(0)
            With GCom.GLog
                .Job2 = "登録ボタン押下"
                .Result = ""
                .Discription = ""
            End With

            'テキストボックスの入力チェック
            If CheckEntryText(CTL, MSG, 1) = False Then
                If MSG = MSG0043I Then MsgIcon = MessageBoxIcon.None 'MSG0043I時のキャンセル対処
                Return
            End If
            CTL = Me.KFURI_DATE_S

            '取引先マスタに取引先コードが存在することを確認
            BRet = CLS.GET_SELECT_TORIMAST(Nothing, _
                        TRKEY.TORIS_CODE, TRKEY.TORIF_CODE, ClsSchduleMaintenanceClass.OPT.OptionNothing)
            If Not BRet Then
                MSG = MSG0063W
                CTL = TORIS_CODE_S
                Return
            End If

            '2010/03/25 学校チェック追加
            If CLS.TR(0).BAITAI_CODE = "07" Then
                MSG = String.Format(MSG0039I, "登録")
                CTL = TORIS_CODE_S
                Return
            End If
            '===========================

            '自分が再振用の取引先マスタか判断
            BRet = CLS.CHECK_SAIFURI_SELF(TRKEY.TORIS_CODE, TRKEY.TORIF_CODE)
            If Not BRet Then
                MSG = MSG0283W
                Return
            End If

            'トランザクション開始
            GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Begin)

            'スケジュールマスタチェック
            If CLS.READ_SCHMAST() = True Then

                '受付済フラグ, 登録済フラグ, 日報フラグ, 中断フラグ を判定する。
                Dim IntFLG() As Integer = {CLS.GetColumnValue("UKETUKE_FLG_S", 0), _
                                           CLS.GetColumnValue("TOUROKU_FLG_S", 0), _
                                           CLS.GetColumnValue("TYUUDAN_FLG_S", 0), _
                                           CLS.GetColumnValue("NIPPO_FLG_S", 0)}
                If IntFLG(0) + IntFLG(1) + IntFLG(2) + IntFLG(3) > 0 Then

                    MSG = String.Format(MSG0286W, IntFLG(0).ToString, IntFLG(1).ToString, IntFLG(2).ToString, IntFLG(3).ToString)

                    With Me
                        .CmdSelect.PerformClick()
                        Select Case True
                            Case IntFLG(0) > 0 : CTL = .UKETUKE_FLG_S
                            Case IntFLG(1) > 0 : CTL = .TOUROKU_FLG_S
                            Case IntFLG(2) > 0 : CTL = .TYUUDAN_FLG_S
                            Case IntFLG(3) > 0 : CTL = .NIPPO_FLG_S
                        End Select
                    End With
                    CTL = CmdSelect
                    '2016/11/30 saitou RSV2 ADD メンテナンス ---------------------------------------- START
                    '並列トランザクションになってしまうため、ロールバックしてトランザクションを閉じる
                    Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                    '2016/11/30 saitou RSV2 ADD ----------------------------------------------------- END
                    Return
                End If
                'End If

                '登録確認メッセージ
                MSG = MSG0041I
                DRet = MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If Not DRet = DialogResult.OK Then
                    MsgIcon = MessageBoxIcon.None
                    '2016/11/30 saitou RSV2 ADD メンテナンス ---------------------------------------- START
                    '並列トランザクションになってしまうため、ロールバックしてトランザクションを閉じる
                    Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                    '2016/11/30 saitou RSV2 ADD ----------------------------------------------------- END
                    Return
                End If

                'スケジュールマスタから該当既存レコードを削除
                '***
                '再振2009.10.05
                CLS.SCH.WRK_SFURI_YDATE = CLS.GetColumnValue("KSAIFURI_DATE_S", 0)
                If CLS.DELETE_SCHMAST() = False Then
                    Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                    MSG = String.Format(MSG0027E, "スケジュールマスタ", "削除")
                    MsgIcon = MessageBoxIcon.Error
                    Return
                End If
            Else
                '新規登録確認メッセージ
                MSG = MSG0003I
                DRet = MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If Not DRet = DialogResult.OK Then
                    MsgIcon = MessageBoxIcon.None
                    '2016/11/30 saitou RSV2 ADD メンテナンス ---------------------------------------- START
                    '並列トランザクションになってしまうため、ロールバックしてトランザクションを閉じる
                    Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                    '2016/11/30 saitou RSV2 ADD ----------------------------------------------------- END
                    Return
                End If
            End If

            '登録行為の実行
            If CLS.INSERT_NEW_SCHMAST(0) = True Then

                '***************************
                '再振2009.10.05
                '***************************
                If CLS.TR(0).SFURI_FLG = 1 Then

                    wrkTorifCode = CLS.TR(0).TORIF_CODE
                    wrkSfuriFlg = CLS.TR(0).SFURI_FLG
                    wrkFuriDate = CLS.SCH.FURI_DATE

                    CLS.TR(0).TORIF_CODE = CLS.TR(0).SFURI_FCODE
                    CLS.TR(0).SFURI_FLG = 0
                    CLS.SCH.FURI_DATE = CLS.SCH.KSAIFURI_DATE

                    '2017/04/12 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ START
                    '再振スケジュールマスタチェック
                    If CLS.READ_SCHMAST() = True Then
                        MSG = MSG0378W
                        MsgIcon = MessageBoxIcon.Warning
                        Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                        Return
                    End If
                    '2017/04/12 タスク）西野 ADD 標準版修正（飯田信金分反映）------------------------------------ END

                    If CLS.INSERT_NEW_SCHMAST(0) = True Then

                        CLS.TR(0).TORIF_CODE = wrkTorifCode
                        CLS.TR(0).SFURI_FLG = wrkSfuriFlg
                        CLS.SCH.FURI_DATE = wrkFuriDate

                        Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)

                        With GCom.GLog
                            .Result = MenteCommon.clsCommon.OK
                            .Discription = ""
                        End With

                        MSG = MSG0004I
                        MsgIcon = MessageBoxIcon.Information

                        Call Form_Initialize(False)
                    Else

                        CLS.TR(0).TORIF_CODE = wrkTorifCode
                        CLS.TR(0).SFURI_FLG = wrkSfuriFlg
                        CLS.SCH.FURI_DATE = wrkFuriDate

                        Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)

                        With GCom.GLog
                            .Result = MenteCommon.clsCommon.NG
                            .Discription = ""
                        End With

                        MSG = MSG0002E.Replace("{0}", "登録")
                        MsgIcon = MessageBoxIcon.Error
                    End If


                Else
                    Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)

                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.OK
                        .Discription = ""
                    End With

                    MSG = MSG0004I
                    MsgIcon = MessageBoxIcon.Information

                    Call Form_Initialize(False)

                End If

            Else

                Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)

                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = ""
                End With

                MSG = MSG0002E.Replace("{0}", "登録")
                MsgIcon = MessageBoxIcon.Error
            End If


        Catch ex As Exception
            Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
            MsgIcon = MessageBoxIcon.Error
            MSG = MSG0006E
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)例外エラー", "成功", ex.Message)
        Finally
            Call SetButtonEnabled()
            Select Case MsgIcon
                Case MessageBoxIcon.None
                Case MessageBoxIcon.Information
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
                    Call SetActionAfter(True)
                    CTL = TORIS_CODE_S
                    cmbKana.SelectedIndex = 0
                    cmbToriName.SelectedIndex = 0
                Case MessageBoxIcon.Error
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
                Case MessageBoxIcon.Warning
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")

            Application.DoEvents()
            CTL.Focus()
            Application.DoEvents()
        End Try
    End Sub

    '更新ボタン押下
    Private Sub CmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdUpdate.Click
        Dim MSG As String = ""
        Dim CTL As Control = Nothing
        Dim MsgIcon As System.Windows.Forms.MessageBoxIcon = MessageBoxIcon.Warning

        Dim wrkTorifCode As String = ""
        Dim wrkSfuriFlg As Integer
        Dim wrkFuriDate As String = ""
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")

            Call SetButtonEnabled(0)
            With GCom.GLog
                .Job2 = "更新ボタン押下"
                .Result = ""
                .Discription = ""
            End With

            'テキストボックスの入力チェック
            If CheckEntryText(CTL, MSG, 2) = False Then
                Return
            End If
            CTL = Me.KFURI_DATE_S

            '他行スケジュールマスタ存在チェック
            If Not CLS.SCH.FURI_DATE = CLS.SCH.NFURI_DATE Then
                If CLS.CHECK_TAKOSCHMAST(0) = True Then
                    MSG = MSG0287W
                    Return
                End If
            End If

            '更新確認(メッセージ)
            MSG = MSG0005I
            Dim dRet As DialogResult = MessageBox.Show(MSG, msgTitle, _
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            If Not dRet = DialogResult.OK Then
                MsgIcon = MessageBoxIcon.None
                Return
            End If

            GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Begin)

            'スケジュールマスタの更新
            Dim SQL As String = "UPDATE SCHMAST"
            SQL &= " SET "
            SQL &= "  TESUU_YDATE_S = '" & CLS.SCH.TESUU_YDATE & "'"
            SQL &= ", KSAIFURI_DATE_S = '" & CLS.SCH.KSAIFURI_DATE & "'"
            SQL &= ", KFURI_DATE_S = '" & CLS.SCH.KFURI_DATE & "'"
            SQL &= ", KESSAI_YDATE_S = '" & CLS.SCH.KESSAI_YDATE & "'"
            SQL &= ", UKETUKE_FLG_S = '" & GCom.NzInt(Me.UKETUKE_FLG_S.Text, 0).ToString & "'"
            SQL &= ", TOUROKU_FLG_S = '" & GCom.NzInt(Me.TOUROKU_FLG_S.Text, 0).ToString & "'"
            SQL &= ", HAISIN_FLG_S = '" & GCom.NzInt(Me.HAISIN_FLG_S.Text, 0).ToString & "'"
            SQL &= ", SAIFURI_FLG_S = '" & GCom.NzInt(Me.SAIFURI_FLG_S.Text, 0).ToString & "'"
            SQL &= ", TESUUKEI_FLG_S = '" & GCom.NzInt(Me.TESUUKEI_FLG_S.Text, 0).ToString & "'"
            SQL &= ", SOUSIN_FLG_S = '" & GCom.NzInt(Me.SOUSIN_FLG_S.Text, 0).ToString & "'"
            SQL &= ", FUNOU_FLG_S = '" & GCom.NzInt(Me.FUNOU_FLG_S.Text, 0).ToString & "'"
            SQL &= ", TESUUTYO_FLG_S = '" & GCom.NzInt(Me.TESUUTYO_FLG_S.Text, 0).ToString & "'"
            SQL &= ", KESSAI_FLG_S = '" & GCom.NzInt(Me.KESSAI_FLG_S.Text, 0).ToString & "'"
            SQL &= ", HENKAN_FLG_S = '" & GCom.NzInt(Me.HENKAN_FLG_S.Text, 0).ToString & "'"
            SQL &= ", TYUUDAN_FLG_S = '" & GCom.NzInt(Me.TYUUDAN_FLG_S.Text, 0).ToString & "'"
            SQL &= ", TAKOU_FLG_S = '" & GCom.NzInt(Me.TAKOU_FLG_S.Text, 0).ToString & "'"
            SQL &= ", NIPPO_FLG_S = '" & GCom.NzInt(Me.NIPPO_FLG_S.Text, 0).ToString & "'"
            SQL &= " WHERE FSYORI_KBN_S = '" & CLS.TR(0).FSYORI_KBN & "'"
            SQL &= " AND TORIS_CODE_S = '" & CLS.TR(0).TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_S = '" & CLS.TR(0).TORIF_CODE & "'"
            SQL &= " AND FURI_DATE_S = '" & CLS.SCH.FURI_DATE & "'"

            Dim SQLCode As Integer = 0
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
            If Ret = 1 AndAlso SQLCode = 0 Then

                SQL = "UPDATE SCHMAST_SUB"
                SQL &= " SET "
                SQL &= "  SYOUGOU_FLG_S = '" & GCom.NzInt(Me.SYOUGOU_FLG_S.Text, 0).ToString & "'"
                SQL &= " WHERE FSYORI_KBN_SSUB = '" & CLS.TR(0).FSYORI_KBN & "'"
                SQL &= " AND TORIS_CODE_SSUB = '" & CLS.TR(0).TORIS_CODE & "'"
                SQL &= " AND TORIF_CODE_SSUB = '" & CLS.TR(0).TORIF_CODE & "'"
                SQL &= " AND FURI_DATE_SSUB = '" & CLS.SCH.FURI_DATE & "'"

                SQLCode = 0
                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)

                If Ret = 1 AndAlso SQLCode = 0 Then

                Else
                    GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                    MSG = String.Format(MSG0027E, "スケジュールマスタ", "更新")
                    MsgIcon = MessageBoxIcon.Error
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = ""
                    End With
                    Return
                End If

                '2010/03/25 学校の場合は学校スケジュールの決済予定日を更新する
                If CLS.TR(0).BAITAI_CODE = "07" Then
                    Dim G_SQL As New StringBuilder
                    G_SQL.Append(" UPDATE G_SCHMAST")
                    G_SQL.Append(" SET ")
                    G_SQL.Append(" KESSAI_YDATE_S = '" & CLS.SCH.KESSAI_YDATE & "'")
                    G_SQL.Append(" WHERE GAKKOU_CODE_S = '" & CLS.TR(0).TORIS_CODE & "'")
                    Select Case CLS.TR(0).TORIF_CODE
                        Case "01"   '初振
                            G_SQL.Append(" AND FURI_KBN_S = '0'")
                        Case "02"   '再振
                            G_SQL.Append(" AND FURI_KBN_S = '1'")
                        Case "03"   '入金
                            G_SQL.Append(" AND FURI_KBN_S = '2'")
                        Case "04"   '出金
                            G_SQL.Append(" AND FURI_KBN_S = '3'")
                    End Select
                    G_SQL.Append(" AND FURI_DATE_S = '" & CLS.SCH.FURI_DATE & "'")
                    SQLCode = 0
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, G_SQL.ToString, SQLCode)

                    If Ret = 1 AndAlso SQLCode = 0 Then

                    Else
                        GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                        MSG = String.Format(MSG0027E, "学校スケジュール", "更新")
                        MsgIcon = MessageBoxIcon.Error
                        With GCom.GLog
                            .Result = MenteCommon.clsCommon.NG
                            .Discription = ""
                        End With
                        Return
                    End If
                End If
                '============================================================

                '***再振2009.10.05
                '再振ありで再振予定日が修正されたら、再振のスケジュールを作り直す
                If CLS.TR(0).SFURI_FLG = 1 AndAlso txtBeforeSaifuri.Text.Equals(CLS.SCH.KSAIFURI_DATE) = False Then
                    Dim Dsql As New StringBuilder(128)

                    Dsql.Append("DELETE FROM SCHMAST ")
                    Dsql.Append("WHERE TORIS_CODE_S ='" & CLS.TR(0).TORIS_CODE & "'")
                    Dsql.Append("AND TORIF_CODE_S ='" & CLS.TR(0).SFURI_FCODE & "'")
                    Dsql.Append("AND FURI_DATE_S ='" & txtBeforeSaifuri.Text & "'")  '保存してある再振日
                    Dsql.Append("AND UKETUKE_FLG_S ='0'")
                    Dsql.Append("AND TOUROKU_FLG_S ='0'")
                    Dsql.Append("AND HAISIN_FLG_S ='0'")
                    Dsql.Append("AND SAIFURI_FLG_S ='0'")
                    Dsql.Append("AND SOUSIN_FLG_S ='0'")
                    Dsql.Append("AND FUNOU_FLG_S ='0'")
                    Dsql.Append("AND TESUUKEI_FLG_S ='0'")
                    Dsql.Append("AND TESUUTYO_FLG_S ='0'")
                    Dsql.Append("AND KESSAI_FLG_S ='0'")
                    Dsql.Append("AND HENKAN_FLG_S ='0'")
                    Dsql.Append("AND TYUUDAN_FLG_S ='0'")
                    Dsql.Append("AND TAKOU_FLG_S ='0'")
                    Dsql.Append("AND NIPPO_FLG_S ='0'")
                    Dsql.Append("AND EXISTS (")
                    Dsql.Append("     SELECT")
                    Dsql.Append("         *")
                    Dsql.Append("     FROM")
                    Dsql.Append("         SCHMAST_SUB")
                    Dsql.Append("     WHERE")
                    Dsql.Append("         TORIS_CODE_S = TORIS_CODE_SSUB")
                    Dsql.Append("     AND TORIF_CODE_S = TORIF_CODE_SSUB")
                    Dsql.Append("     AND FURI_DATE_S = FURI_DATE_SSUB")
                    Dsql.Append("     AND SYOUGOU_FLG_S = '0'")
                    Dsql.Append(" )")

                    SQLCode = 0
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, Dsql.ToString, SQLCode)

                    ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
                    If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                        If Ret = 1 AndAlso SQLCode = 0 Then
                            Dsql.Length = 0
                            Dsql.Append("DELETE FROM SCHMAST_SUB ")
                            Dsql.Append("WHERE TORIS_CODE_SSUB ='" & CLS.TR(0).TORIS_CODE & "'")
                            Dsql.Append(" AND  TORIF_CODE_SSUB ='" & CLS.TR(0).SFURI_FCODE & "'")
                            Dsql.Append(" AND  FURI_DATE_SSUB  ='" & txtBeforeSaifuri.Text & "'")  '保存してある再振日

                            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, Dsql.ToString, SQLCode)
                        End If
                    End If
                    ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END

                    If Ret = 1 AndAlso SQLCode = 0 Then

                        wrkTorifCode = CLS.TR(0).TORIF_CODE
                        wrkSfuriFlg = CLS.TR(0).SFURI_FLG
                        wrkFuriDate = CLS.SCH.FURI_DATE

                        CLS.TR(0).TORIF_CODE = CLS.TR(0).SFURI_FCODE
                        CLS.TR(0).SFURI_FLG = 0
                        CLS.SCH.FURI_DATE = CLS.SCH.KSAIFURI_DATE

                        If CLS.INSERT_NEW_SCHMAST(0) Then

                            CLS.TR(0).TORIF_CODE = wrkTorifCode
                            CLS.TR(0).SFURI_FLG = wrkSfuriFlg
                            CLS.SCH.FURI_DATE = wrkFuriDate

                            GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)

                            Dim BRet As Boolean

                            MSG = MSG0006I
                            MsgIcon = MessageBoxIcon.Information

                            '画面へ表示展開／初期化
                            CLS.SCH.FURI_DATE = CLS.SCH.NFURI_DATE
                            BRet = CLS.READ_SCHMAST()
                            Call SetEachControlValueInForm(True)
                        Else

                            CLS.TR(0).TORIF_CODE = wrkTorifCode
                            CLS.TR(0).SFURI_FLG = wrkSfuriFlg
                            CLS.SCH.FURI_DATE = wrkFuriDate

                            GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)

                            If SQLCode = 1 Then
                                MSG = String.Format(MSG0027E, "再振スケジュール", "作成")
                            Else
                                MSG = String.Format(MSG0027E, "再振スケジュール", "作成")
                                MsgIcon = MessageBoxIcon.Error
                                With GCom.GLog
                                    .Result = MenteCommon.clsCommon.NG
                                    .Discription = ""
                                End With
                            End If
                        End If
                    Else

                        GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)

                        If SQLCode = 1 Then
                            MSG = String.Format(MSG0027E, "再振スケジュール", "削除")
                        Else
                            MSG = MSG0288W
                            MsgIcon = MessageBoxIcon.Error
                            With GCom.GLog
                                .Result = MenteCommon.clsCommon.NG
                                .Discription = ""
                            End With
                        End If
                    End If

                Else
                    GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)

                    MSG = MSG0006I
                    MsgIcon = MessageBoxIcon.Information

                    Call Form_Initialize(False)

                End If

            Else

                GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)

                If SQLCode = 1 Then
                    MSG = MSG0289W
                Else
                    MSG = MSG0002E.Replace("{0}", "更新")
                    MsgIcon = MessageBoxIcon.Error
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                        .Discription = ""
                    End With
                End If
            End If
        Catch ex As Exception
            GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
            MSG = MSG0006E
            MsgIcon = MessageBoxIcon.Error
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)例外エラー", "成功", ex.Message)
        Finally
            Call SetButtonEnabled()
            Select Case MsgIcon
                Case MessageBoxIcon.Information
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
                    Call SetActionAfter(True)
                    CTL = TORIS_CODE_S
                    cmbKana.SelectedIndex = 0
                    cmbToriName.SelectedIndex = 0
                Case MessageBoxIcon.Error
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
                Case MessageBoxIcon.Warning
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")

            Application.DoEvents()
            CTL.Focus()
        End Try
    End Sub

    '参照ボタン押下
    Private Sub CmdSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdSelect.Click
        Dim MSG As String = ""
        Dim CTL As Control = Nothing
        Dim MsgIcon As System.Windows.Forms.MessageBoxIcon = MessageBoxIcon.Warning

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            'テキストボックスの入力チェック
            If CheckEntryText(CTL, MSG, 3) = False Then
                Return
            End If
            CTL = Me.SAIFURI_DATE_S

            '取引先マスタに取引先コードが存在することを確認
            Dim BRet As Boolean = CLS.GET_SELECT_TORIMAST(Nothing, _
                        TRKEY.TORIS_CODE, TRKEY.TORIF_CODE, ClsSchduleMaintenanceClass.OPT.OptionNothing)
            If Not BRet Then
                MSG = MSG0063W
                CTL = TORIS_CODE_S
                Return
            End If

            'スケジュールマスタチェック
            If CLS.READ_SCHMAST() = False Then
                MSG = MSG0095W
                Return
            End If

            If CLS.READ_SCHMAST_SUB() = False Then
                MSG = MSG0095W
                Return
            End If

            BRet = CLS.CHECK_SAIFURI_SELF(TRKEY.TORIS_CODE, TRKEY.TORIF_CODE)
            If Not BRet Then
                KSAIFURI_DATE_S.Enabled = False
                KSAIFURI_DATE_S1.Enabled = False
                KSAIFURI_DATE_S2.Enabled = False
            Else
                KSAIFURI_DATE_S.Enabled = True
                KSAIFURI_DATE_S1.Enabled = True
                KSAIFURI_DATE_S2.Enabled = True
            End If

            '画面へ表示展開と表示／非表示制御
            Call SetEachControlValueInForm()

            MsgIcon = MessageBoxIcon.None
        Catch ex As Exception
            MSG = MSG0006E
            MsgIcon = MessageBoxIcon.Error
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)
        Finally
            Select Case MsgIcon
                Case MessageBoxIcon.None
                    Call SetActionAfter(False)
                Case MessageBoxIcon.Warning
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
                Case MessageBoxIcon.Error
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
            End Select
            Application.DoEvents()
            CTL.Focus()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
        End Try
    End Sub

    '削除ボタン押下
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdDelete.Click
        Dim MSG As String = ""
        Dim Ret As Integer = 0
        Dim SQLCode As Integer = 0
        Dim CTL As Control = Nothing
        Dim MsgIcon As System.Windows.Forms.MessageBoxIcon = MessageBoxIcon.Warning
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)開始", "成功", "")

            Call SetButtonEnabled(0)
            With GCom.GLog
                .Job2 = "削除ボタン押下"
                .Result = ""
                .Discription = ""
            End With

            'テキストボックスの入力チェック
            If CheckEntryText(CTL, MSG, 4) = False Then
                Return
            End If
            CTL = Me.TORIS_CODE_S

            'スケジュールマスタチェック
            If CLS.READ_SCHMAST() = False Then
                MSG = MSG0095W
                Return
            Else
                '2010/03/25 学校チェック追加
                If CLS.TR(0).BAITAI_CODE = "07" Then
                    MSG = String.Format(MSG0039I, "削除")
                    Return
                End If
                '===========================

                '他行スケジュールマスタ存在チェック
                If CLS.CHECK_TAKOSCHMAST(0) = True Then
                    MSG = MSG0314W
                    Return
                End If

                If Not CLS.CHECK_SAIFURI_SELF(TRKEY.TORIS_CODE, TRKEY.TORIF_CODE) Then
                    MSG = MSG0291W
                    Return
                End If

                '処理フラグチェック追加(処理フラグが一つでも立っていたら削除できない)
                If CLS.GetColumnValue("UKETUKE_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("TOUROKU_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("HAISIN_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("SAIFURI_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("SOUSIN_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("FUNOU_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("TESUUKEI_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("TESUUTYO_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("KESSAI_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("HENKAN_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("TYUUDAN_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("TAKOU_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("NIPPO_FLG_S", 0) > 0 OrElse _
                   CLS.GetColumnValue("SYOUGOU_FLG_S", 0) Then

                    MSG = MSG0290W
                    Return
                End If
                'End Select
            End If

            '削除確認メッセージ
            MSG = MSG0042I
            Dim dRet As DialogResult = MessageBox.Show(MSG, msgTitle, _
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            If Not dRet = DialogResult.OK Then
                MsgIcon = MessageBoxIcon.None
                Return
            End If

            '有効フラグを 0 にする(2008.02.12変更物理削除とする)
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Begin)

            'スケジュールマスタから該当既存レコードを削除

            '再振2009.10.05
            CLS.SCH.WRK_SFURI_YDATE = CLS.GetColumnValue("KSAIFURI_DATE_S", 0)
            Dim BRet As Boolean = CLS.DELETE_SCHMAST()
            If BRet Then
                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)

                With GCom.GLog
                    .Result = MenteCommon.clsCommon.OK
                    .Discription = ""
                End With

                MSG = MSG0008I
                MsgIcon = MessageBoxIcon.Information
            Else
                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                    .Discription = "(SQLCode=" & SQLCode & ":" & MSG
                End With

                MSG = MSG0002E.Replace("{0}", "削除")
                MsgIcon = MessageBoxIcon.Error
            End If
        Catch ex As Exception
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
            MSG = MSG0006E
            MsgIcon = MessageBoxIcon.Error
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)例外エラー", "失敗", ex.Message)
        Finally
            Call SetButtonEnabled()
            Select Case MsgIcon
                Case MessageBoxIcon.None
                Case MessageBoxIcon.Information
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
                    Call Form_Initialize(False)
                Case MessageBoxIcon.Error
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
                Case MessageBoxIcon.Warning
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)終了", "成功", "")
            Application.DoEvents()
            CTL.Focus()
        End Try
    End Sub

    '取消ボタン押下
    Private Sub CmdClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdClear.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")

            Call Form_Initialize(False)

            cmbKana.SelectedIndex = 0
            cmbToriName.SelectedIndex = 0

            TORIS_CODE_S.Focus()

        Catch ex As Exception

            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)例外エラー", "失敗", ex.Message)

        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try

    End Sub

    '終了ボタン押下時
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)開始", "成功", "")

            Me.Close()
        Catch ex As Exception
            Call MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)例外エラー", "失敗", ex.Message)

        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(終了)終了", "成功", "")
        End Try
    End Sub

    '
    ' 機能　 ： テキストボックスの入力値チェック
    '
    ' 引数　 ： ARG1 - 設定不備コントロール
    ' 　　　 　 ARG2 - エラーメッセージ
    ' 　　　 　 ARG3 - 登録 = 1, 更新 = 2, 参照 = 3, 削除 = 4
    '
    ' 戻り値 ： チェックＯＫ = True
    ' 　　　 　 チェックＮＧ = False
    '
    ' 備考　 ： 必須項目チェック兼務
    ' 　　　 　 入力振替日の有効チェック(営業日以外は不可)
    '
    Private Function CheckEntryText(ByRef CTL As Control, ByRef MSG As String, ByVal SEL As Short) As Boolean
        Dim nTemp As Decimal
        Dim sTemp As String
        Dim sTemp2 As String
        Dim onText(2) As Integer
        Dim onDate As Date
        Dim nRet As Integer
        Try
            '変数設定(FJH Package 仕様を踏襲)
            TRKEY.TORIS_CODE = ""
            TRKEY.TORIF_CODE = ""
            With CLS.SCH
                .FURI_DATE = New String("0"c, 8)
                .NFURI_DATE = New String("0"c, 8)
                .TESUU_YDATE = New String("0"c, 8)
                .KESSAI_YDATE = New String("0"c, 8)
                .KSAIFURI_DATE = New String("0"c, 8)
                .YUUKOU_FLG = 1
            End With

            '取引先主コード
            nTemp = GCom.NzDec(TORIS_CODE_S.Text, 0)
            If TORIS_CODE_S.Text.Trim.Length = 0 Then
                MSG = MSG0057W
                CTL = TORIS_CODE_S
                Exit Try
            End If
            TRKEY.TORIS_CODE = String.Format("{0:0000000000}", nTemp)

            '取引先副コード
            sTemp = GCom.NzStr(TORIF_CODE_S.Text)
            If TORIF_CODE_S.Text.Trim.Length = 0 Then
                TRKEY.TORIS_CODE = ""
                MSG = MSG0059W
                CTL = TORIF_CODE_S
                Exit Try
            End If
            TRKEY.TORIF_CODE = String.Format("{0:00}", sTemp)

            If SEL = 3 Then
                '契約振替日
                onText(0) = GCom.NzInt(KFURI_DATE_S.Text)
                onText(1) = GCom.NzInt(KFURI_DATE_S1.Text)
                onText(2) = GCom.NzInt(KFURI_DATE_S2.Text)
                If onText(0) + onText(1) + onText(2) > 0 Then
                    nRet = GCom.SET_DATE(onDate, onText)
                    Select Case nRet
                        Case 0
                            MSG = MSG0281W.Replace("{0}", "契約振替日（年）")
                            CTL = KFURI_DATE_S
                            Exit Try
                        Case 1
                            MSG = MSG0281W.Replace("{0}", "契約振替日（月）")
                            CTL = KFURI_DATE_S1
                            Exit Try
                        Case 2
                            MSG = MSG0281W.Replace("{0}", "契約振替日（日）")
                            CTL = KFURI_DATE_S2
                            Exit Try
                    End Select
                End If
            End If

            '振替日(先ず歴日判定)
            onText(0) = GCom.NzInt(FURI_DATE_S.Text)
            onText(1) = GCom.NzInt(FURI_DATE_S1.Text)
            onText(2) = GCom.NzInt(FURI_DATE_S2.Text)
            If onText(0) + onText(1) + onText(2) = 0 Then
                MSG = MSG0285W.Replace("{0}", "振替日")
                CTL = FURI_DATE_S
                Exit Try
            Else
                nRet = GCom.SET_DATE(onDate, onText)
                Select Case nRet
                    Case 0
                        MSG = MSG0281W.Replace("{0}", "振替日（年）")
                        CTL = FURI_DATE_S
                        Exit Try
                    Case 1
                        MSG = MSG0281W.Replace("{0}", "振替日（月）")
                        CTL = FURI_DATE_S1
                        Exit Try
                    Case 2
                        MSG = MSG0281W.Replace("{0}", "振替日（日）")
                        CTL = FURI_DATE_S2
                        Exit Try
                    Case Else
                        '営業日判定
                        sTemp = String.Format("{0:yyyyMMdd}", onDate)
                        sTemp2 = ""
                        If Not GCom.CheckDateModule(sTemp, sTemp2, 0) Then
                            MSG = MSG0292W.Replace("{0}", "振替日")
                            CTL = FURI_DATE_S1
                            Exit Try
                        End If
                End Select
            End If

            '変数設定(FJH Package 仕様を踏襲)
            CLS.SCH.FURI_DATE = sTemp
            CLS.SCH.NFURI_DATE = sTemp
            With GCom.GLog
                .ToriCode = TRKEY.TORIS_CODE & "-" & TRKEY.TORIF_CODE
                .FuriDate = sTemp
            End With

            '処理ボタン判定(登録／更新でチェックする)
            If SEL = 1 OrElse SEL = 2 Then

                '手数料徴求予定日
                onText(0) = GCom.NzInt(TESUU_YDATE_S.Text)
                onText(1) = GCom.NzInt(TESUU_YDATE_S1.Text)
                onText(2) = GCom.NzInt(TESUU_YDATE_S2.Text)
                If onText(0) + onText(1) + onText(2) > 0 Then
                    nRet = GCom.SET_DATE(onDate, onText)
                    Select Case nRet
                        Case 0
                            MSG = MSG0281W.Replace("{0}", "手数料徴求予定日（年）")
                            CTL = TESUU_YDATE_S
                            Exit Try
                        Case 1
                            MSG = MSG0281W.Replace("{0}", "手数料徴求予定日（月）")
                            CTL = TESUU_YDATE_S1
                            Exit Try
                        Case 2
                            MSG = MSG0281W.Replace("{0}", "手数料徴求予定日（日）")
                            CTL = TESUU_YDATE_S2
                            Exit Try
                        Case Else
                            '営業日判定
                            sTemp = String.Format("{0:yyyyMMdd}", onDate)
                            sTemp2 = ""
                            If Not GCom.CheckDateModule(sTemp, sTemp2, 0) Then

                                MSG = MSG0292W.Replace("{0}", "手数料徴求予定日")
                                CTL = TESUU_YDATE_S2
                                Exit Try
                            End If
                            CLS.SCH.TESUU_YDATE = sTemp
                    End Select
                Else
                    TESUU_YDATE_S.Text = "0000"
                    TESUU_YDATE_S1.Text = "00"
                    TESUU_YDATE_S2.Text = "00"
                End If

                '決済予定日
                onText(0) = GCom.NzInt(KESSAI_YDATE_S.Text)
                onText(1) = GCom.NzInt(KESSAI_YDATE_S1.Text)
                onText(2) = GCom.NzInt(KESSAI_YDATE_S2.Text)
                If onText(0) + onText(1) + onText(2) > 0 Then
                    nRet = GCom.SET_DATE(onDate, onText)
                    Select Case nRet
                        Case 0
                            MSG = MSG0281W.Replace("{0}", "決済予定日（年）")
                            CTL = KESSAI_YDATE_S
                            Exit Try
                        Case 1
                            MSG = MSG0281W.Replace("{0}", "決済予定日（月）")
                            CTL = KESSAI_YDATE_S1
                            Exit Try
                        Case 2
                            MSG = MSG0281W.Replace("{0}", "決済予定日（日）")
                            CTL = KESSAI_YDATE_S2
                            Exit Try
                        Case Else
                            '営業日判定
                            sTemp = String.Format("{0:yyyyMMdd}", onDate)
                            sTemp2 = ""
                            If Not GCom.CheckDateModule(sTemp, sTemp2, 0) Then

                                MSG = MSG0292W.Replace("{0}", "決済予定日")
                                CTL = KESSAI_YDATE_S2
                                Exit Try
                            End If
                            CLS.SCH.KESSAI_YDATE = sTemp
                    End Select
                Else
                    KESSAI_YDATE_S.Text = "0000"
                    KESSAI_YDATE_S1.Text = "00"
                    KESSAI_YDATE_S2.Text = "00"
                End If

                '再振予定日(要否は再振フラグの設定如何に依る)
                Dim TempInteger As Integer = 0
                TempInteger += GCom.NzInt(KSAIFURI_DATE_S.Text, 0)
                TempInteger += GCom.NzInt(KSAIFURI_DATE_S1.Text, 0)
                TempInteger += GCom.NzInt(KSAIFURI_DATE_S2.Text, 0)
                Dim SFURI_FLG_Temp As Integer = 0
                If Not SEL = 1 Then
                    SFURI_FLG_Temp = GCom.NzInt(CLS.TR(0).SFURI_FLG)
                End If
                If SFURI_FLG_Temp = 0 AndAlso TempInteger = 0 Then
                    KSAIFURI_DATE_S.Text = "0000"
                    KSAIFURI_DATE_S1.Text = "00"
                    KSAIFURI_DATE_S2.Text = "00"
                Else
                    onText(0) = GCom.NzInt(KSAIFURI_DATE_S.Text)
                    onText(1) = GCom.NzInt(KSAIFURI_DATE_S1.Text)
                    onText(2) = GCom.NzInt(KSAIFURI_DATE_S2.Text)
                    nRet = GCom.SET_DATE(onDate, onText)
                    Select Case nRet
                        Case 0
                            MSG = MSG0281W.Replace("{0}", "再振予定日（年）")
                            CTL = KSAIFURI_DATE_S
                            Exit Try
                        Case 1
                            MSG = MSG0281W.Replace("{0}", "再振予定日（月）")
                            CTL = KSAIFURI_DATE_S1
                            Exit Try
                        Case 2
                            MSG = MSG0281W.Replace("{0}", "再振予定日（日）")
                            CTL = KSAIFURI_DATE_S2
                            Exit Try
                        Case Else
                            '営業日判定
                            sTemp = String.Format("{0:yyyyMMdd}", onDate)
                            sTemp2 = ""
                            If Not GCom.CheckDateModule(sTemp, sTemp2, 0) Then

                                MSG = MSG0292W.Replace("{0}", "再振予定日")
                                CTL = KSAIFURI_DATE_S1
                                Exit Try
                            End If
                            CLS.SCH.KSAIFURI_DATE = sTemp
                    End Select
                End If

                Dim ObjFLG() As Control = {UKETUKE_FLG_S, TOUROKU_FLG_S, _
                 HAISIN_FLG_S, SAIFURI_FLG_S, _
                SOUSIN_FLG_S, FUNOU_FLG_S, TESUUKEI_FLG_S, TESUUTYO_FLG_S, _
                KESSAI_FLG_S, HENKAN_FLG_S, TYUUDAN_FLG_S, _
                TAKOU_FLG_S, NIPPO_FLG_S, SYOUGOU_FLG_S}

                Dim StrFLG() As String = {"受付済フラグ", "登録済フラグ", _
                    "自行配信済フラグ", "再振済フラグ", _
                    "送信済フラグ", "不能済フラグ", "手数料計算済フラグ", "手数料徴求済フラグ", _
                    "決済フラグ", "返還済フラグ", "中断フラグ", "他行フラグ", "日報フラグ", "照合済フラグ"}

                Dim Index As Integer = 0
                For Each OBJ As Control In ObjFLG
                    nTemp = GCom.NzDec(OBJ.Text, 0)
                    Select Case OBJ.Name.ToUpper
                        Case "FUNOU_FLG_S", "HAISIN_FLG_S", "KESSAI_FLG_S", "TESUU_FLG_S", "NIPPO_FLG_S"
                            Select Case nTemp
                                Case 0, 1, 2
                                Case Else
                                    MSG = MSG0293W.Replace("{0}", StrFLG(Index))
                                    CTL = OBJ
                                    Exit Try
                            End Select
                        Case Else
                            Select Case nTemp
                                Case 0, 1
                                Case Else
                                    MSG = MSG0294W.Replace("{0}", StrFLG(Index))
                                    CTL = OBJ
                                    Exit Try
                            End Select
                    End Select
                    OBJ.Text = nTemp.ToString
                    Index += 1
                Next OBJ

                '契約振替日
                Dim EIGYOUBI_FLG As Boolean = False
                onText(0) = GCom.NzInt(KFURI_DATE_S.Text)
                onText(1) = GCom.NzInt(KFURI_DATE_S1.Text)
                onText(2) = GCom.NzInt(KFURI_DATE_S2.Text)
                If onText(0) + onText(1) + onText(2) = 0 Then
                    MSG = MSG0285W.Replace("{0}", "契約振替日")
                    CTL = KFURI_DATE_S
                    Exit Try
                Else
                    nRet = GCom.SET_DATE(onDate, onText)
                    Select Case nRet
                        Case 0
                            MSG = MSG0281W.Replace("{0}", "契約振替日（年）")
                            CTL = KFURI_DATE_S
                            Exit Try
                        Case 1
                            MSG = MSG0281W.Replace("{0}", "契約振替日（月）")
                            CTL = KFURI_DATE_S1
                            Exit Try
                        Case 2
                            MSG = MSG0281W.Replace("{0}", "契約振替日（日）")
                            CTL = KFURI_DATE_S2
                            Exit Try
                        Case Else
                            '営業日判定
                            sTemp = String.Format("{0:yyyyMMdd}", onDate)
                            CLS.SCH.KFURI_DATE = sTemp
                            sTemp2 = ""
                            EIGYOUBI_FLG = GCom.CheckDateModule(sTemp, sTemp2, 0)
                            If Not CLS.SCH.FURI_DATE = CLS.SCH.KFURI_DATE AndAlso EIGYOUBI_FLG Then

                                MSG = MSG0043I
                                Select Case MessageBox.Show(MSG, msgTitle, _
                                                MessageBoxButtons.OKCancel, _
                                                MessageBoxIcon.Question, _
                                                MessageBoxDefaultButton.Button1)
                                    Case DialogResult.OK
                                    Case Else
                                        CTL = KFURI_DATE_S2
                                        Exit Try
                                End Select
                            End If
                    End Select
                End If
            End If

            Return True

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力値ﾁｪｯｸ関数)", "失敗", ex.Message)

        End Try

        Return False

    End Function

    '
    ' 機能　 ： 画面初期化
    '
    ' 引数　 ： ARG1 - 画面初期起動識別
    '
    ' 戻り値 ： 正常処理 = True, 処理異常 = False
    '
    ' 備考　 ： なし
    '
    Private Sub Form_Initialize(ByVal SEL As Boolean)
        Try
            CmdInsert.Enabled = True
            CmdUpdate.Enabled = False
            CmdDelete.Enabled = False
            CmdSelect.Enabled = True

            '画面直接配置のコントロール
            For Each CTL As Control In Me.Controls
                Dim FLG As Boolean = False
                If TypeOf CTL Is TextBox Then
                    FLG = Form_Initialize_Sub(CTL)
                ElseIf TypeOf CTL Is Label Then
                    FLG = Form_Initialize_Sub(CTL)
                End If
                If FLG Then
                    CTL.Text = ""
                End If
            Next CTL

            '初期 Enabled 設定
            Call SetActionAfter(True)

            txtBeforeSaifuri.Text = ""

            MEM_FLG.Text = ""
            MEM_FLG.Visible = False

        Catch ex As Exception
            Throw
        Finally
            Me.TORIS_CODE_S.Focus()
        End Try
    End Sub

    '
    ' 機能　 ： 画面初期化サブ関数
    '
    ' 引数　 ： コントロール変数
    '
    ' 戻り値 ： 該当するか否か
    '
    ' 備考　 ： なし
    '
    Private Function Form_Initialize_Sub(ByVal CTL As Control) As Boolean
        Try
            Return CTL.Name.EndsWith("S") OrElse CTL.Name.EndsWith("T") _
                OrElse CTL.Name.EndsWith("S1") OrElse CTL.Name.EndsWith("S2") _
                OrElse CTL.Name.EndsWith("S3") OrElse CTL.Name.EndsWith("S4") _
                OrElse CTL.Name.EndsWith("S5")
        Catch ex As Exception
            Throw
        End Try
        Return False
    End Function

    '
    ' 機能　 ： 各表示領域への値設定
    '
    ' 引数　 ： ARG1 - 更新動作識別 True = 更新, False = 登録
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： なし
    '
    Private Sub SetEachControlValueInForm(Optional ByVal aUpdate As Boolean = False)
        Try
            Dim Temp As String

            '委託者名
            If CLS.TR(0).ITAKU_NNAME.Trim.Length = 0 Then
                Temp = CLS.TR(0).ITAKU_KNAME
            Else
                Temp = CLS.TR(0).ITAKU_NNAME
            End If
            ITAKU_NNAME_T.Text = GCom.GetLimitString(Temp, 44)

            '契約振替日
            Call SetDateTypeControl(CLS.GetColumnValue("KFURI_DATE_S"), _
                                    KFURI_DATE_S, KFURI_DATE_S1, KFURI_DATE_S2)
            '契約振替日
            If aUpdate Then
                Call SetDateTypeControl(CLS.GetColumnValue("FURI_DATE_S"), _
                                        FURI_DATE_S, FURI_DATE_S1, FURI_DATE_S2)
            End If

            '再振替日
            Call SetDateTypeControl(CLS.GetColumnValue("SAIFURI_DATE_S"), _
                                    SAIFURI_DATE_S, SAIFURI_DATE_S1, SAIFURI_DATE_S2)
            '再振替予定日
            Call SetDateTypeControl(CLS.GetColumnValue("KSAIFURI_DATE_S"), _
                                    KSAIFURI_DATE_S, KSAIFURI_DATE_S1, KSAIFURI_DATE_S2)

            '***再振2009.10.05
            txtBeforeSaifuri.Text = CLS.GetColumnValue("KSAIFURI_DATE_S")


            '振替コード
            FURI_CODE_S.Text = CLS.GetColumnValue("FURI_CODE_S")

            '企業コード
            KIGYO_CODE_S.Text = CLS.GetColumnValue("KIGYO_CODE_S")

            '委託者コード
            ITAKU_CODE_S.Text = CLS.GetColumnValue("ITAKU_CODE_S")

            '送信区分
            SOUSIN_KBN_S.Text = CLS.GetColumnValue("SOUSIN_KBN_S")

            '持込区分
            MOTIKOMI_KBN_S.Text = CLS.GetColumnValue("MOTIKOMI_KBN_S")

            '媒体コード
            BAITAI_CODE_S.Text = CLS.GetColumnValue("BAITAI_CODE_S")

            '持込ＳＥＱ
            MOTIKOMI_SEQ_S.Text = CLS.GetColumnValue("MOTIKOMI_SEQ_S")

            'ファイルＳＥＱ
            FILE_SEQ_S.Text = CLS.GetColumnValue("FILE_SEQ_S")

            '手数料計算区分
            TESUU_KBN_S.Text = CLS.GetColumnValue("TESUU_KBN_S")

            '依頼書作成日
            Call SetDateTypeControl(CLS.GetColumnValue("IRAISYO_DATE_S"), _
                                    IRAISYO_DATE_S, IRAISYO_DATE_S1, IRAISYO_DATE_S2)

            '依頼書回収予定日
            Call SetDateTypeControl(CLS.GetColumnValue("IRAISYOK_YDATE_S"), _
                                    IRAISYOK_YDATE_S, IRAISYOK_YDATE_S1, IRAISYOK_YDATE_S2)

            '受付日時
            Call SetDateTypeControl(CLS.GetColumnValue("UKETUKE_DATE_S"), _
                                    UKETUKE_DATE_S, UKETUKE_DATE_S1, UKETUKE_DATE_S2)

            '照合日
            Call SetDateTypeControl(CLS.GetColumnValue("SYOUGOU_DATE_S"), _
                                    SYOUGOU_DATE_S, SYOUGOU_DATE_S1, SYOUGOU_DATE_S2)

            '登録日(落込)
            Call SetDateTypeControl(CLS.GetColumnValue("TOUROKU_DATE_S"), _
                                    TOUROKU_DATE_S, TOUROKU_DATE_S1, TOUROKU_DATE_S2)
            ''配信処理予定日他行提携外分
            'Call SetDateTypeControl(CLS.GetColumnValue("HAISIN_T2YDATE_S"), _
            '                        HAISIN_T2YDATE_S, HAISIN_T2YDATE_S1, HAISIN_T2YDATE_S2)
            ''配信処理日他行提携外分
            'Call SetDateTypeControl(CLS.GetColumnValue("HAISIN_T2DATE_S"), _
            '                        HAISIN_T2DATE_S, HAISIN_T2DATE_S1, HAISIN_T2DATE_S2, _
            '                        HAISIN_T2DATE_S3, HAISIN_T2DATE_S4, HAISIN_T2DATE_S5)
            ''配信処理予定日他行提携内分
            'Call SetDateTypeControl(CLS.GetColumnValue("HAISIN_T1YDATE_S"), _
            '                        HAISIN_T1YDATE_S, HAISIN_T1YDATE_S1, HAISIN_T1YDATE_S2)
            '配信処理予定日
            Call SetDateTypeControl(CLS.GetColumnValue("HAISIN_YDATE_S"), _
                                    HAISIN_YDATE_S, HAISIN_YDATE_S1, HAISIN_YDATE_S2)
            ''配信処理日他行提携内分
            'Call SetDateTypeControl(CLS.GetColumnValue("HAISIN_T1DATE_S"), _
            '                        HAISIN_T1DATE_S, HAISIN_T1DATE_S1, HAISIN_T1DATE_S2, _
            '                        HAISIN_T1DATE_S3, HAISIN_T1DATE_S4, HAISIN_T1DATE_S5)
            '配信処理日
            Call SetDateTypeControl(CLS.GetColumnValue("HAISIN_DATE_S"), _
                                    HAISIN_DATE_S, HAISIN_DATE_S1, HAISIN_DATE_S2)
            '送信予定日
            Call SetDateTypeControl(CLS.GetColumnValue("SOUSIN_YDATE_S"), _
                                    SOUSIN_YDATE_S, SOUSIN_YDATE_S1, SOUSIN_YDATE_S2)
            '送信処理日
            Call SetDateTypeControl(CLS.GetColumnValue("SOUSIN_DATE_S"), _
                                    SOUSIN_DATE_S, SOUSIN_DATE_S1, SOUSIN_DATE_S2)
            '不能処理予定日
            Call SetDateTypeControl(CLS.GetColumnValue("FUNOU_YDATE_S"), _
                                    FUNOU_YDATE_S, FUNOU_YDATE_S1, FUNOU_YDATE_S2)
            '不能処理日
            Call SetDateTypeControl(CLS.GetColumnValue("FUNOU_DATE_S"), _
                                    FUNOU_DATE_S, FUNOU_DATE_S1, FUNOU_DATE_S2)
            ''不能処理予定日(T1)
            'Call SetDateTypeControl(CLS.GetColumnValue("FUNOU_T1YDATE_S"), _
            '                        FUNOU_T1YDATE_S, FUNOU_T1YDATE_S1, FUNOU_T1YDATE_S2)
            ''不能処理日
            'Call SetDateTypeControl(CLS.GetColumnValue("FUNOU_T1DATE_S"), _
            '                        FUNOU_T1DATE_S, FUNOU_T1DATE_S1, FUNOU_T1DATE_S2)
            ''不能処理予定日
            'Call SetDateTypeControl(CLS.GetColumnValue("FUNOU_T2YDATE_S"), _
            '                        FUNOU_T2YDATE_S, FUNOU_T2YDATE_S1, FUNOU_T2YDATE_S2)
            ''不能処理日
            'Call SetDateTypeControl(CLS.GetColumnValue("FUNOU_T2DATE_S"), _
            '                        FUNOU_T2DATE_S, FUNOU_T2DATE_S1, FUNOU_T2DATE_S2)
            '手数料徴求予定日
            Call SetDateTypeControl(CLS.GetColumnValue("TESUU_YDATE_S"), _
                                    TESUU_YDATE_S, TESUU_YDATE_S1, TESUU_YDATE_S2)
            '手数料徴求日
            Call SetDateTypeControl(CLS.GetColumnValue("TESUU_DATE_S"), _
                                    TESUU_DATE_S, TESUU_DATE_S1, TESUU_DATE_S2)
            '決済予定日
            Call SetDateTypeControl(CLS.GetColumnValue("KESSAI_YDATE_S"), _
                                    KESSAI_YDATE_S, KESSAI_YDATE_S1, KESSAI_YDATE_S2)
            '資金決済日 KESSAI_DATE_S
            Call SetDateTypeControl(CLS.GetColumnValue("KESSAI_DATE_S"), _
                                    KESSAI_DATE_S, KESSAI_DATE_S1, KESSAI_DATE_S2)
            '返還処理予定日
            Call SetDateTypeControl(CLS.GetColumnValue("HENKAN_YDATE_S"), _
                                    HENKAN_YDATE_S, HENKAN_YDATE_S1, HENKAN_YDATE_S2)
            '返還処理日
            Call SetDateTypeControl(CLS.GetColumnValue("HENKAN_DATE_S"), _
                                    HENKAN_DATE_S, HENKAN_DATE_S1, HENKAN_DATE_S2)
            ''返却送信日時
            'Call SetDateTypeControl(CLS.GetColumnValue("HENKYAKU_DATE_S"), _
            '                        HENKYAKU_DATE_S, HENKYAKU_DATE_S1, HENKYAKU_DATE_S2, _
            '                        HENKYAKU_DATE_S3, HENKYAKU_DATE_S4, HENKYAKU_DATE_S5)
            '持込期日
            Call SetDateTypeControl(CLS.GetColumnValue("MOTIKOMI_DATE_S"), _
                                    MOTIKOMI_DATE_S, MOTIKOMI_DATE_S1, MOTIKOMI_DATE_S2)

            '落込済フラグ
            UKETUKE_FLG_S.Text = CLS.GetColumnValue("UKETUKE_FLG_S")

            '参照時落込フラグ
            MEM_FLG.Text = UKETUKE_FLG_S.Text

            '照合済フラグ
            SYOUGOU_FLG_S.Text = CLS.GetColumnValue("SYOUGOU_FLG_S")

            '登録済フラグ
            TOUROKU_FLG_S.Text = CLS.GetColumnValue("TOUROKU_FLG_S")

            ''他行提携外分配信済フラグ
            'HAISIN_T2FLG_S.Text = CLS.GetColumnValue("HAISIN_T2FLG_S")

            ''他行提携内分配信済フラグ
            'HAISIN_T1FLG_S.Text = CLS.GetColumnValue("HAISIN_T1FLG_S")

            '配信済フラグ
            HAISIN_FLG_S.Text = CLS.GetColumnValue("HAISIN_FLG_S")

            '再振済フラグ
            SAIFURI_FLG_S.Text = CLS.GetColumnValue("SAIFURI_FLG_S")

            '送信済フラグ
            SOUSIN_FLG_S.Text = CLS.GetColumnValue("SOUSIN_FLG_S")

            '不能済フラグ
            FUNOU_FLG_S.Text = CLS.GetColumnValue("FUNOU_FLG_S")

            '手数料計算済フラグ
            TESUUKEI_FLG_S.Text = CLS.GetColumnValue("TESUUKEI_FLG_S")

            '手数料徴求済フラグ
            TESUUTYO_FLG_S.Text = CLS.GetColumnValue("TESUUTYO_FLG_S")

            '決済フラグ
            KESSAI_FLG_S.Text = CLS.GetColumnValue("KESSAI_FLG_S")

            '返還済フラグ       
            HENKAN_FLG_S.Text = CLS.GetColumnValue("HENKAN_FLG_S")

            ''返却送信フラグ
            'HENKYAKU_FLG_S.Text = CLS.GetColumnValue("HENKYAKU_FLG_S")

            '中断フラグ
            TYUUDAN_FLG_S.Text = CLS.GetColumnValue("TYUUDAN_FLG_S")

            ''他行フラグ
            TAKOU_FLG_S.Text = CLS.GetColumnValue("TAKOU_FLG_S")

            '日報フラグ
            NIPPO_FLG_S.Text = CLS.GetColumnValue("NIPPO_FLG_S")

            '処理件数
            SYORI_KEN_S.Text = String.Format("{0:#,##0}", CLS.GetColumnValue("SYORI_KEN_S", 0, 0))

            '処理金額
            SYORI_KIN_S.Text = String.Format("{0:#,##0}", CLS.GetColumnValue("SYORI_KIN_S", 0, 0))

            'インプットエラー件数
            ERR_KEN_S.Text = String.Format("{0:#,##0}", CLS.GetColumnValue("ERR_KEN_S", 0, 0))

            'インプットエラー金額
            ERR_KIN_S.Text = String.Format("{0:#,##0}", CLS.GetColumnValue("ERR_KIN_S", 0, 0))

            '手数料金額
            TESUU_KIN_S.Text = String.Format("{0:#,##0}", CLS.GetColumnValue("TESUU_KIN_S", 0, 0))

            '振替済件数
            FURI_KEN_S.Text = String.Format("{0:#,##0}", CLS.GetColumnValue("FURI_KEN_S", 0, 0))

            '振替済金額
            FURI_KIN_S.Text = String.Format("{0:#,##0}", CLS.GetColumnValue("FURI_KIN_S", 0, 0))

            '不能件数
            FUNOU_KEN_S.Text = String.Format("{0:#,##0}", CLS.GetColumnValue("FUNOU_KEN_S", 0, 0))

            '不能金額
            FUNOU_KIN_S.Text = String.Format("{0:#,##0}", CLS.GetColumnValue("FUNOU_KIN_S", 0, 0))

            '送信ファイル名
            SFILE_NAME_S.Text = CLS.GetColumnValue("SFILE_NAME_S", "")

            '配信日時
            JIFURI_TIME_STAMP_S.Text = CLS.GetColumnValue("JIFURI_TIME_STAMP_S", "")

            '決済日時
            KESSAI_TIME_STAMP_S.Text = CLS.GetColumnValue("KESSAI_TIME_STAMP_S", "")

            '手数料日時
            TESUU_TIME_STAMP_S.Text = CLS.GetColumnValue("TESUU_TIME_STAMP_S", "")

            '作成日付
            Call SetDateTypeControl(CLS.GetColumnValue("SAKUSEI_DATE_S"), _
                        SAKUSEI_DATE_S, SAKUSEI_DATE_S1, SAKUSEI_DATE_S2)

            '受付日時
            UKETUKE_TIME_STAMP_S.Text = CLS.GetColumnValue("UKETUKE_TIME_STAMP_S", "")

        Catch ex As Exception
            Throw
        End Try
    End Sub

    '
    ' 機能　 ： ラベルへのデータ設定
    '
    ' 引数　 ： ARG1 - 日付データ
    ' 　　　 　 ARG2 - 年ラベル
    ' 　　　 　 ARG3 - 月ラベル
    ' 　　　 　 ARG4 - 日ラベル
    ' 　　　 　 ARG5 - 時ラベル
    ' 　　　 　 ARG6 - 分ラベル
    ' 　　　 　 ARG7 - 秒ラベル
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： 年月日＋時間表示
    '
    Private Sub SetDateTypeControl(ByVal DateSTring As String, ByVal onYear As Label, ByVal onMonth As Label, _
            ByVal onDay As Label, ByVal onHour As Label, ByVal onMinute As Label, ByVal onSecond As Label)
        Try
            Dim onDate As DateTime = GCom.SET_DATE(DateSTring)
            If onDate = Nothing Then
                onYear.Text = "0000"
                onMonth.Text = "00"
                onDay.Text = "00"
                onHour.Text = "00"
                onMinute.Text = "00"
                onSecond.Text = "00"
            Else
                onYear.Text = onDate.Year.ToString("0000")
                onMonth.Text = onDate.Month.ToString("00")
                onDay.Text = onDate.Day.ToString("00")
                onHour.Text = onDate.Hour.ToString("00")
                onMinute.Text = onDate.Minute.ToString("00")
                onSecond.Text = onDate.Second.ToString("00")
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub

    '
    ' 機能　 ： ラベルへのデータ設定
    '
    ' 引数　 ： ARG1 - 日付データ
    ' 　　　 　 ARG2 - 年ラベル
    ' 　　　 　 ARG3 - 月ラベル
    ' 　　　 　 ARG4 - 日ラベル
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： 年月日＋時間表示
    '
    Private Sub SetDateTypeControl(ByVal DateSTring As String, _
            ByVal onYear As Label, ByVal onMonth As Label, ByVal onDay As Label)
        Try
            Dim onDate As DateTime = GCom.SET_DATE(DateSTring)
            If onDate = Nothing Then
                onYear.Text = "0000"
                onMonth.Text = "00"
                onDay.Text = "00"
            Else
                onYear.Text = onDate.Year.ToString("0000")
                onMonth.Text = onDate.Month.ToString("00")
                onDay.Text = onDate.Day.ToString("00")
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub

    '
    ' 機能　 ： ラベルへのデータ設定
    '
    ' 引数　 ： ARG1 - 日付データ
    ' 　　　 　 ARG2 - 年テキストボックス
    ' 　　　 　 ARG3 - 月テキストボックス
    ' 　　　 　 ARG4 - 日テキストボックス
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： 年月日＋時間表示
    '
    Private Sub SetDateTypeControl(ByVal DateSTring As String, _
            ByVal onYear As TextBox, ByVal onMonth As TextBox, ByVal onDay As TextBox)
        Try
            Dim onDate As DateTime = GCom.SET_DATE(DateSTring)
            If onDate = Nothing Then
                onYear.Text = "0000"
                onMonth.Text = "00"
                onDay.Text = "00"
            Else
                onYear.Text = onDate.Year.ToString("0000")
                onMonth.Text = onDate.Month.ToString("00")
                onDay.Text = onDate.Day.ToString("00")
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub


    '取引先主／副コード／委託者コード入力時
    Private Sub TORI_CODE_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles TORIS_CODE_S.Validating, TORIF_CODE_S.Validating

        With CType(sender, TextBox)

            '設定値を、一応、検査しよう
            Call GCom.NzNumberString(CType(sender, TextBox), True)

            '2008.03.11 FJH 指示により機能停止
            Return

            'If .Name = "TORIS_CODE_S" OrElse .Name = "TORIF_CODE_S" Then

            '    'ちゃんと入っていれば
            '    If (GCom.NzDec(TORIS_CODE_S.Text, 0) * GCom.NzDec(TORIF_CODE_S.Text, 0) > 0) Then

            '        '取引先マスタ検索
            '        Dim REC As OracleDataReader = Nothing
            '        Try
            '            Dim SQL As String = "SELECT ITAKU_NNAME_T"
            '            SQL &= " FROM TORIMAST"
            '            SQL &= " WHERE FSYORI_KBN_T = '1'"
            '            SQL &= " AND TORIS_CODE_T = '" & GCom.NzDec(TORIS_CODE_S.Text, "") & "'"
            '            SQL &= " AND TORIF_CODE_T = '" & GCom.NzDec(TORIF_CODE_S.Text, "") & "'"

            '            Dim Ret As Boolean = GCom.SetDynaset(SQL, REC)
            '            If Ret AndAlso REC.Read Then

            '                '取引先マスタからの情報展開(委託者漢字名)
            '                Dim Temp As String = GCom.NzStr(REC.Item("ITAKU_NNAME_T"))
            '                ITAKU_NNAME_T.Text = GCom.GetLimitString(Temp.Trim, 44)
            '            End If
            '        Catch ex As Exception
            '            Throw
            '        Finally
            '            If Not REC Is Nothing Then
            '                REC.Close()
            '                REC.Dispose()
            '            End If
            '        End Try

            '        '振替日領域へ移動
            '        If Me.KFURI_DATE_S.Visible AndAlso Me.KFURI_DATE_S.Enabled Then
            '            Me.KFURI_DATE_S.Focus()
            '        Else
            '            Me.FURI_DATE_S.Focus()
            '        End If
            '    End If
            'End If
        End With
    End Sub

    '日付関係の入力領域チェック
    Private Sub DATE_S_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles _
 _
 _
            FURI_DATE_S.Validating, _
            FURI_DATE_S1.Validating, _
            FURI_DATE_S2.Validating, _
            TESUU_YDATE_S.Validating, _
            TESUU_YDATE_S1.Validating, _
            TESUU_YDATE_S2.Validating, _
            KSAIFURI_DATE_S.Validating, _
            KSAIFURI_DATE_S1.Validating, _
            KSAIFURI_DATE_S2.Validating, _
            KFURI_DATE_S2.Validating, _
            KFURI_DATE_S1.Validating, _
            KFURI_DATE_S.Validating, _
            KESSAI_YDATE_S.Validating, _
            KESSAI_YDATE_S2.Validating, _
            KESSAI_YDATE_S1.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

    '振替日入力後の移動場所設定
    Private Sub FURI_DATE_S2_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles FURI_DATE_S2.LostFocus

        Call GCom.NzNumberString(CType(sender, TextBox), True)

        Dim OBJ() As Button = New Button(5) {Me.CmdInsert, Me.CmdUpdate, Me.CmdSelect, Me.CmdDelete, Me.CmdClear, Me.CmdBack}

        For Index As Integer = OBJ.GetLowerBound(0) To OBJ.GetUpperBound(0) Step 1

            If OBJ(Index).Enabled Then

                OBJ(Index).Focus()
                Return
            End If
        Next Index
    End Sub

    'フラグ関係の入力領域チェック
    Private Sub FLG_S_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles UKETUKE_FLG_S.Validating, HAISIN_FLG_S.Validating, SAIFURI_FLG_S.Validating, SOUSIN_FLG_S.Validating, FUNOU_FLG_S.Validating, TESUUKEI_FLG_S.Validating, HENKAN_FLG_S.Validating, KESSAI_FLG_S.Validating, TESUUTYO_FLG_S.Validating, TYUUDAN_FLG_S.Validating, NIPPO_FLG_S.Validating, TOUROKU_FLG_S.Validating, SYOUGOU_FLG_S.Validating

        With CType(sender, TextBox)
            Select Case GCom.NzDec(.Text, "")
                '**修正 2008/06/17 sakai ***********
                '"9"を入力できるように変更
                Case "0", "1", "2", "9"
                    'Case "0", "1", "2"
                    '*******************************
                Case Else
                    '有効な値でない場合更新しないような仕様とする
                    .Text = ""
            End Select
        End With
    End Sub

    '
    ' 機能　 ： 初期化、登録、削除、実行後処置
    '
    ' 引数　 ： ARG1 - なし
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： 共通化
    '
    Private Sub SetActionAfter(ByVal SetVisible As Boolean)
        Try
            '処理ボタン
            With Me
                .CmdInsert.Enabled = (SetVisible)
                .CmdUpdate.Enabled = (Not SetVisible)
                .CmdDelete.Enabled = (Not SetVisible)
                .CmdSelect.Enabled = SetVisible
            End With

            '主要設定領域
            With Me
                .TORIS_CODE_S.Enabled = SetVisible
                .TORIF_CODE_S.Enabled = SetVisible
                .FURI_DATE_S.Enabled = SetVisible
                .FURI_DATE_S1.Enabled = SetVisible
                .FURI_DATE_S2.Enabled = SetVisible
            End With

        Catch ex As Exception
            Throw
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
            Dim onControl() As Control = {Me.CmdInsert, Me.CmdUpdate, Me.CmdSelect, _
                    Me.CmdDelete, Me.CmdClear, Me.CmdBack, Me.btnFuriDate}
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
            End Select
        Catch ex As Exception
            Throw
        End Try
    End Sub

    '数値だけを抜き出して金額用の表示編集を行う
    Private Sub Money_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles TESUU_KIN_S.Validating

        Try
            With CType(sender, TextBox)
                .Text = String.Format("{0:#,##0}", GCom.NzDec(.Text, 1))
            End With
        Catch ex As Exception
            Throw
        End Try
    End Sub

    'カナコンボボックス設定変更時再読み込み
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            '取引先コードがReadOnlyの場合は処理終了
            If TORIS_CODE_S.Enabled = False Then
                Exit Sub
            End If

            '取引先コンボボックス設定
            If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, TORIS_CODE_S, TORIF_CODE_S, "1") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            Throw
        End Try
    End Sub

    '取引先コード設定
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        Try
            '取引先コードがReadOnlyの場合は処理終了
            If TORIS_CODE_S.Enabled = False Then
                Exit Sub
            End If
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, TORIS_CODE_S, TORIF_CODE_S)
            End If

            TORIS_CODE_S.Focus()

        Catch ex As Exception
            Throw
        End Try
    End Sub
#Region " 振替日変更"
    Private Sub btnFuriDate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFuriDate.Click
        '=====================================================================================
        'NAME           :btnFuriDate_Click
        'Parameter      :
        'Description    :振替日変更ボタン
        'Return         :
        'Create         :2010/02/05
        'Update         :
        '=====================================================================================
        Dim KFJMAST031 As KFJMAST031
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振替日変更)開始", "成功", "")
            KFJMAST031 = New KFJMAST031
            If KFJMAST031.fn_check_text(TORIS_CODE_S, TORIF_CODE_S, FURI_DATE_S, FURI_DATE_S1, FURI_DATE_S2) = False Then
                Return
            End If
            If KFJMAST031.fn_check_Table(TORIS_CODE_S, TORIF_CODE_S, FURI_DATE_S, FURI_DATE_S1, FURI_DATE_S2) = False Then
                Return
            End If

            gstrFURI_DATE = ""
            With KFJMAST031
                .Owner = Me
                .ShowDialog()
            End With

            If gstrFURI_DATE = "" Then
                Return
            End If

            FURI_DATE_S.Text = Mid(gstrFURI_DATE, 1, 4)
            FURI_DATE_S1.Text = Mid(gstrFURI_DATE, 5, 2)
            FURI_DATE_S2.Text = Mid(gstrFURI_DATE, 7, 2)

            CmdSelect_Click(sender, e)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振替日変更)例外", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振替日変更)終了", "成功", "")
        End Try
    End Sub
#End Region

End Class