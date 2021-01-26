Option Explicit On
Option Strict On

Imports System
Imports System.Text

Public Class KFUMAST010

#Region "宣言"

    Private MainLog As New CASTCommon.BatchLOG("KFUMAST010", "金融機関マスタメンテナンス画面")

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx02 As New CASTCommon.Events("0-9-", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx03 As New CASTCommon.Events("0-9()-", CASTCommon.Events.KeyMode.GOOD)

    '主キー部
    Private Structure TENMAST_KEY
        Dim KIN_NO_N As String
        Dim SIT_NO_N As String
        Dim KIN_FUKA_N As String
        Dim SIT_FUKA_N As String
    End Structure
    Private KY As TENMAST_KEY

    'データ部
    Private Structure TENMAST_DAT

        'Dim TENMAST_SAKUSEI_DATE_N As String
        'Dim TENMAST_KOUSIN_DATE_N As String

        '金融機関情報マスタ
        Dim KIN_KIN_KNAME_N As String
        Dim KIN_KIN_NNAME_N As String
        Dim KIN_DAIHYO_NO_N As String
        Dim KIN_RYAKU_KIN_KNAME_N As String
        Dim KIN_RYAKU_KIN_NNAME_N As String
        Dim KIN_JC1_N As String
        Dim KIN_JC2_N As String
        Dim KIN_JC3_N As String
        Dim KIN_JC4_N As String
        Dim KIN_JC5_N As String
        Dim KIN_JC6_N As String
        Dim KIN_JC7_N As String
        Dim KIN_JC8_N As String
        Dim KIN_JC9_N As String
        Dim KIN_IDO_DATE_N As String
        Dim KIN_IDO_CODE_N As String
        Dim KIN_NEW_KIN_NO_N As String
        Dim KIN_NEW_KIN_FUKA_N As String
        Dim KIN_KIN_DEL_DATE_N As String
        Dim KIN_TEIKEI_KBN_N As String
        Dim KIN_SYUBETU_N As String
        Dim KIN_TIKU_CODE_N As String
        Dim KIN_TESUU_TANKA_N As String
        Dim KIN_SAKUSEI_DATE_N As String
        Dim KIN_KOUSIN_DATE_N As String

        '支店情報マスタ
        Dim SIT_SIT_KNAME_N As String
        Dim SIT_SIT_NNAME_N As String
        Dim SIT_SEIDOKU_HYOUJI_N As String
        Dim SIT_YUUBIN_N As String
        Dim SIT_KJYU_N As String
        Dim SIT_NJYU_N As String
        Dim SIT_TKOUKAN_NO_N As String
        Dim SIT_DENWA_N As String
        Dim SIT_FAX_N As String
        Dim SIT_TENPO_ZOKUSEI_N As String
        Dim SIT_JIKOU_HYOUJI_N As String
        Dim SIT_FURI_HYOUJI_N As String
        Dim SIT_SYUUTE_HYOUJI_N As String
        Dim SIT_KAWASE_HYOUJI_N As String
        Dim SIT_DAITE_HYOUJI_N As String
        Dim SIT_JISIN_HYOUJI_N As String
        Dim SIT_JC_CODE_N As String
        Dim SIT_IDO_DATE_N As String
        Dim SIT_IDO_CODE_N As String
        Dim SIT_NEW_SIT_NO_N As String
        Dim SIT_NEW_SIT_FUKA_N As String
        Dim SIT_SIT_DEL_DATE_N As String
        Dim SIT_TKOUKAN_NNAME_N As String
        Dim SIT_SAKUSEI_DATE_N As String
        Dim SIT_KOUSIN_DATE_N As String

        '2011/03/30 削除日考慮 更新時の削除日退避用
        Dim OLD_KIN_DEL_DATE_N As String
        Dim OLD_SIT_DEL_DATE_N As String
    End Structure
    Private DT As TENMAST_DAT

    Private Enum CMD
        CmdClearMode = 0
        CmdInsertMode = 1
        CmdUpdateMode = 2
        CmdSelectMode = 3
        CmdDeleteMode = 4
    End Enum

    '2011/03/30 Publicへ
    'Private CancelFlg As Boolean
    Public CancelFlg As Boolean

    Private MyOwnerForm As Form

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    Private Const msgTitle As String = "金融機関マスタメンテナンス(KFUMAST010)"

#End Region

#Region "画面制御"

    Private Sub KFUMAST010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLog.UserID = LW.UserID
            MainLog.ToriCode = LW.ToriCode
            MainLog.FuriDate = LW.FuriDate

            MainLog.Write("(ロード)開始", "成功")

            MyOwnerForm = GOwnerForm
            GOwnerForm = Me

            'ユーザID／システム日付表示
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            Call Clear_gamen()

        Catch ex As Exception
            MessageBox.Show(MSG0004E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("(ロード)終了", "失敗", ex.Message)
        Finally
            MainLog.Write("(ロード)終了", "成功")
        End Try
    End Sub

    'Private Sub Form1_FormClosing(ByVal sender As Object, _
    '    ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
    '    If e.CloseReason = CloseReason.UserClosing Then
    '        Me.Owner.Show()
    '    End If
    'End Sub

#End Region

#Region "ボタン"

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLog.Write("(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0004E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("(クローズ)例外エラー", "失敗", ex.Message)
        Finally
            MainLog.Write("(クローズ)終了", "成功", "")
        End Try
    End Sub

    '登録ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Dim MSG As String
        Dim DRet As DialogResult

        Dim MainDB As New CASTCommon.MyOracle

        Try
            MainLog.Write("(登録)開始", "成功")

            Me.SuspendLayout()

            'キー項目／データ項目入力チェック
            If Not FN_CHK_FIRST(CMD.CmdInsertMode) Then
                Return
            End If

            '金融機関マスタに金融機関コードが存在しないことを確認
            Dim Dummy As TENMAST_DAT = Nothing

            If SET_SELECT_ACTION(CMD.CmdInsertMode, Dummy, MainDB) Then
                MSG = MSG0038W
                DRet = MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.KIN_NO_N.Focus()
                MainLog.Write("(登録)終了", "成功")
                Return
            End If

            MSG = MSG0003I
            DRet = MessageBox.Show(MSG, msgTitle, _
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            If DRet = DialogResult.OK Then

                '金融機関マスタへレコードを挿入する。
                Select Case FN_INSERT_TENMAST(MainDB)
                    Case True

                        MainDB.Commit()

                        MSG = MSG0004I
                        DRet = MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information)

                        Call Clear_gamen()

                        Me.KIN_NO_N.Focus()

                    Case Else

                        MainDB.Rollback()

                        MSG = String.Format(MSG0002E, "更新")
                        DRet = MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Select
            End If

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0002E.Replace("{0}", "登録"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("(登録)終了", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            Me.ResumeLayout()
            MainLog.Write("(登録)終了", "成功")
        End Try
    End Sub

    '更新ボタン
    Private Sub btnKousin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKousin.Click
        Dim MSG As String
        Dim DRet As DialogResult

        Dim MainDB As New CASTCommon.MyOracle

        Try
            MainLog.Write("(更新)開始", "成功")

            Me.SuspendLayout()

            'キー項目／データ項目入力チェック
            If Not FN_CHK_FIRST(CMD.CmdUpdateMode) Then
                Return
            End If

            '金融機関マスタに金融機関コードが存在することを確認
            Dim CurInf As TENMAST_DAT = Nothing
            If Not SET_SELECT_ACTION(CMD.CmdUpdateMode, CurInf, MainDB) Then
                MSG = MSG0034W
                DRet = MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.KIN_NO_N.Focus()
                Return
            End If

            '2011/03/30 削除日考慮 更新先のレコード存在チェック ここから
            If DT.KIN_KIN_DEL_DATE_N <> DT.OLD_KIN_DEL_DATE_N OrElse _
               DT.SIT_SIT_DEL_DATE_N <> DT.OLD_SIT_DEL_DATE_N Then

                If SET_SELECT_ACTION(MainDB) Then
                    MSG = MSG0038W
                    DRet = MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.KIN_DEL_DATE_N.Focus()
                    Return
                End If
            End If
            '2011/03/30 削除日考慮 更新先のレコード存在チェック ここから

            '変更の有無を確認する。
            Dim FLG(45) As Boolean   '金融機関情報マスタ18 支店情報マスタ18
            Dim Counter As Integer = 0
            For Idx As Integer = FLG.GetLowerBound(0) To FLG.GetUpperBound(0) Step 1

                Select Case Idx

                    '金融機関情報マスタ18

                    Case 0
                        FLG(Idx) = (CurInf.KIN_KIN_KNAME_N = DT.KIN_KIN_KNAME_N)
                    Case 1
                        FLG(Idx) = (CurInf.KIN_KIN_NNAME_N = DT.KIN_KIN_NNAME_N)
                    Case 2
                        FLG(Idx) = (CurInf.KIN_DAIHYO_NO_N = DT.KIN_DAIHYO_NO_N)
                    Case 3
                        FLG(Idx) = (CurInf.KIN_RYAKU_KIN_KNAME_N = DT.KIN_RYAKU_KIN_KNAME_N)
                    Case 4
                        FLG(Idx) = (CurInf.KIN_RYAKU_KIN_NNAME_N = DT.KIN_RYAKU_KIN_NNAME_N)
                    Case 5
                        FLG(Idx) = (CurInf.KIN_JC1_N = DT.KIN_JC1_N)
                    Case 6
                        FLG(Idx) = (CurInf.KIN_JC2_N = DT.KIN_JC2_N)
                    Case 7
                        FLG(Idx) = (CurInf.KIN_JC3_N = DT.KIN_JC3_N)
                    Case 8
                        FLG(Idx) = (CurInf.KIN_JC4_N = DT.KIN_JC4_N)
                    Case 9
                        FLG(Idx) = (CurInf.KIN_JC5_N = DT.KIN_JC5_N)
                    Case 10
                        FLG(Idx) = (CurInf.KIN_JC6_N = DT.KIN_JC6_N)
                    Case 11
                        FLG(Idx) = (CurInf.KIN_JC7_N = DT.KIN_JC7_N)
                    Case 12
                        FLG(Idx) = (CurInf.KIN_JC8_N = DT.KIN_JC8_N)
                    Case 13
                        FLG(Idx) = (CurInf.KIN_JC9_N = DT.KIN_JC9_N)
                    Case 14
                        FLG(Idx) = (CurInf.KIN_IDO_DATE_N = DT.KIN_IDO_DATE_N)
                    Case 15
                        FLG(Idx) = (CurInf.KIN_IDO_CODE_N = DT.KIN_IDO_CODE_N)
                    Case 16
                        FLG(Idx) = (CurInf.KIN_NEW_KIN_NO_N = DT.KIN_NEW_KIN_NO_N)
                    Case 17
                        FLG(Idx) = (CurInf.KIN_NEW_KIN_FUKA_N = DT.KIN_NEW_KIN_FUKA_N)
                    Case 18
                        FLG(Idx) = (CurInf.KIN_KIN_DEL_DATE_N = DT.KIN_KIN_DEL_DATE_N)
                    Case 19
                        FLG(Idx) = (CurInf.KIN_TEIKEI_KBN_N = DT.KIN_TEIKEI_KBN_N)
                    Case 20
                        FLG(Idx) = (CurInf.KIN_SYUBETU_N = DT.KIN_SYUBETU_N)
                    Case 21
                        FLG(Idx) = (CurInf.KIN_TIKU_CODE_N = DT.KIN_TIKU_CODE_N)
                    Case 22
                        FLG(Idx) = (CurInf.KIN_TESUU_TANKA_N = DT.KIN_TESUU_TANKA_N)

                        '支店情報マスタ18
                    Case 23
                        FLG(Idx) = (CurInf.SIT_SIT_KNAME_N = DT.SIT_SIT_KNAME_N)
                    Case 24
                        FLG(Idx) = (CurInf.SIT_SIT_NNAME_N = DT.SIT_SIT_NNAME_N)
                    Case 25
                        FLG(Idx) = (CurInf.SIT_SEIDOKU_HYOUJI_N = DT.SIT_SEIDOKU_HYOUJI_N)
                    Case 26
                        FLG(Idx) = (CurInf.SIT_YUUBIN_N = DT.SIT_YUUBIN_N)
                    Case 27
                        FLG(Idx) = (CurInf.SIT_KJYU_N = DT.SIT_KJYU_N)
                    Case 28
                        FLG(Idx) = (CurInf.SIT_NJYU_N = DT.SIT_NJYU_N)
                    Case 29
                        FLG(Idx) = (CurInf.SIT_TKOUKAN_NO_N = DT.SIT_TKOUKAN_NO_N)
                    Case 30
                        FLG(Idx) = (CurInf.SIT_DENWA_N = DT.SIT_DENWA_N)
                    Case 31
                        FLG(Idx) = (CurInf.SIT_FAX_N = DT.SIT_FAX_N)
                    Case 32
                        FLG(Idx) = (CurInf.SIT_TENPO_ZOKUSEI_N = DT.SIT_TENPO_ZOKUSEI_N)
                    Case 33
                        FLG(Idx) = (CurInf.SIT_JIKOU_HYOUJI_N = DT.SIT_JIKOU_HYOUJI_N)
                    Case 34
                        FLG(Idx) = (CurInf.SIT_FURI_HYOUJI_N = DT.SIT_FURI_HYOUJI_N)
                    Case 35
                        FLG(Idx) = (CurInf.SIT_SYUUTE_HYOUJI_N = DT.SIT_SYUUTE_HYOUJI_N)
                    Case 36
                        FLG(Idx) = (CurInf.SIT_KAWASE_HYOUJI_N = DT.SIT_KAWASE_HYOUJI_N)
                    Case 37
                        FLG(Idx) = (CurInf.SIT_DAITE_HYOUJI_N = DT.SIT_DAITE_HYOUJI_N)
                    Case 38
                        FLG(Idx) = (CurInf.SIT_JISIN_HYOUJI_N = DT.SIT_JISIN_HYOUJI_N)
                    Case 39
                        FLG(Idx) = (CurInf.SIT_JC_CODE_N = DT.SIT_JC_CODE_N)
                    Case 40
                        FLG(Idx) = (CurInf.SIT_IDO_DATE_N = DT.SIT_IDO_DATE_N)
                    Case 41
                        FLG(Idx) = (CurInf.SIT_IDO_CODE_N = DT.SIT_IDO_CODE_N)
                    Case 42
                        FLG(Idx) = (CurInf.SIT_NEW_SIT_NO_N = DT.SIT_NEW_SIT_NO_N)
                    Case 43
                        FLG(Idx) = (CurInf.SIT_NEW_SIT_FUKA_N = DT.SIT_NEW_SIT_FUKA_N)
                    Case 44
                        FLG(Idx) = (CurInf.SIT_SIT_DEL_DATE_N = DT.SIT_SIT_DEL_DATE_N)
                    Case 45
                        FLG(Idx) = (CurInf.SIT_TKOUKAN_NNAME_N = DT.SIT_TKOUKAN_NNAME_N)

                End Select

                Counter += GCom.NzInt(IIf(FLG(Idx), 0, 1))
            Next Idx

            If Counter = 0 Then
                MSG = MSG0040I
                DRet = MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.KIN_KNAME_N.Focus()
                Return
            End If

            MSG = MSG0005I
            DRet = MessageBox.Show(MSG, msgTitle, _
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            If DRet = DialogResult.OK Then

                '金融機関マスタのレコードを更新する。
                Select Case FN_UPDATE_TENMAST(FLG, MainDB)
                    Case True
                        MainDB.Commit()
                        MSG = MSG0006I
                        DRet = MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Me.btnKousin.Enabled = False

                        Clear_gamen()

                        Me.KIN_NO_N.Focus()
                    Case Else
                        MainDB.Rollback()
                        MSG = String.Format(MSG0002E, "更新")
                        DRet = MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select
            End If

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0002E.Replace("{0}", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("(更新)終了", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            Me.ResumeLayout()
            MainLog.Write("(更新)終了", "成功")
        End Try
    End Sub

    '参照ボタン
    Private Sub btnSansyo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSansyo.Click

        Dim MainDB As New CASTCommon.MyOracle

        Try
            MainLog.Write("(参照)開始", "成功")

            CancelFlg = True
            Me.SuspendLayout()

            'キー項目入力チェック
            If Not FN_CHK_FIRST(CMD.CmdSelectMode) Then
                MainLog.Write("(参照)終了", "成功")
                Return
            End If

            Dim BRet As Boolean = SET_SELECT_ACTION(CMD.CmdSelectMode, DT, MainDB)
            If BRet Then

                If Clear_gamen() = False Then
                    MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                With Me
                    'キー
                    .KIN_NO_N.Text = KY.KIN_NO_N
                    .SIT_NO_N.Text = KY.SIT_NO_N
                    .KIN_FUKA_N.Text = KY.KIN_FUKA_N
                    .SIT_FUKA_N.Text = KY.SIT_FUKA_N

                    '金融機関マスタ
                    .KIN_KNAME_N.Text = DT.KIN_KIN_KNAME_N
                    .KIN_NNAME_N.Text = DT.KIN_KIN_NNAME_N
                    .SIT_KNAME_N.Text = DT.SIT_SIT_KNAME_N
                    .SIT_NNAME_N.Text = DT.SIT_SIT_NNAME_N
                    .NEW_KIN_NO_N.Text = String.Format("{0:0000}", DT.KIN_NEW_KIN_NO_N)
                    .NEW_KIN_FUKA_N.Text = DT.KIN_NEW_KIN_FUKA_N
                    .NEW_SIT_NO_N.Text = String.Format("{0:000}", DT.SIT_NEW_SIT_NO_N)
                    .NEW_SIT_FUKA_N.Text = String.Format("{0:000}", DT.SIT_NEW_SIT_FUKA_N)

                    Dim onDate As Date
                    onDate = GCom.SET_DATE(DT.KIN_KIN_DEL_DATE_N)
                    If Not onDate = Nothing Then
                        KIN_DEL_DATE_N.Text = String.Format("{0:yyyy}", onDate)
                        KIN_DEL_DATE_N1.Text = String.Format("{0:MM}", onDate)
                        KIN_DEL_DATE_N2.Text = String.Format("{0:dd}", onDate)
                    End If

                    onDate = GCom.SET_DATE(DT.SIT_SIT_DEL_DATE_N)
                    If Not onDate = Nothing Then
                        SIT_DEL_DATE_N.Text = String.Format("{0:yyyy}", onDate)
                        SIT_DEL_DATE_N1.Text = String.Format("{0:MM}", onDate)
                        SIT_DEL_DATE_N2.Text = String.Format("{0:dd}", onDate)
                    End If

                    onDate = GCom.SET_DATE(DT.SIT_SAKUSEI_DATE_N)
                    If Not onDate = Nothing Then
                        Me.SAKUSEI_DATE_N.Text = String.Format("{0:yyyy/MM/dd}", onDate)
                    End If

                    onDate = GCom.SET_DATE(DT.SIT_KOUSIN_DATE_N)
                    If Not onDate = Nothing Then
                        Me.KOUSIN_DATE_N.Text = String.Format("{0:yyyy/MM/dd}", onDate)
                    End If

                    '2011/03/30 削除日考慮 更新用に退避 ここから
                    DT.OLD_KIN_DEL_DATE_N = DT.KIN_KIN_DEL_DATE_N
                    DT.OLD_SIT_DEL_DATE_N = DT.SIT_SIT_DEL_DATE_N
                    '2011/03/30 削除日考慮 更新用に退避 ここまで

                    '金融機関情報マスタ
                    '.KIN_DAIHYO_NO_N = ""
                    '.KIN_RYAKU_KIN_KNAME_N = ""
                    '.KIN_RYAKU_KIN_NNAME_N = ""
                    '.KIN_JC1_N = ""
                    '.KIN_JC2_N = ""
                    '.KIN_JC3_N = ""
                    '.KIN_JC4_N = ""
                    '.KIN_JC5_N = ""
                    '.KIN_JC6_N = ""
                    '.KIN_JC7_N = ""
                    '.KIN_JC8_N = ""
                    '.KIN_JC9_N = ""
                    '.KIN_IDO_DATE_N = ""
                    '.KIN_IDO_CODE_N = ""

                    '2017/01/18 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                    'コード値をインデックスにしない
                    'コード値に紐付く内容を選択するように修正
                    Dim IntTemp As Integer
                    IntTemp = GCom.NzInt(DT.KIN_TEIKEI_KBN_N)
                    For Cnt As Integer = 0 To .TEIKEI_KBN_N.Items.Count - 1
                        .TEIKEI_KBN_N.SelectedIndex = Cnt
                        If GCom.GetComboBox(.TEIKEI_KBN_N) = IntTemp Then
                            Exit For
                        End If
                    Next

                    IntTemp = GCom.NzInt(DT.KIN_SYUBETU_N)
                    For Cnt As Integer = 0 To .SYUBETU_N.Items.Count - 1
                        .SYUBETU_N.SelectedIndex = Cnt
                        If GCom.GetComboBox(.SYUBETU_N) = IntTemp Then
                            Exit For
                        End If
                    Next

                    IntTemp = GCom.NzInt(DT.KIN_TIKU_CODE_N)
                    For Cnt As Integer = 0 To .TIKU_CODE_N.Items.Count - 1
                        .TIKU_CODE_N.SelectedIndex = Cnt
                        If GCom.GetComboBox(.TIKU_CODE_N) = IntTemp Then
                            Exit For
                        End If
                    Next

                    '.TEIKEI_KBN_N.SelectedIndex = CType(DT.KIN_TEIKEI_KBN_N, Integer)
                    '.SYUBETU_N.SelectedIndex = CType(DT.KIN_SYUBETU_N, Integer)
                    '.TIKU_CODE_N.SelectedIndex = CType(DT.KIN_TIKU_CODE_N, Integer)
                    '2017/01/18 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END

                    .TESUU_TANKA_N.Text = String.Format("{0:#,##0}", GCom.NzDec(DT.KIN_TESUU_TANKA_N))


                    '支店情報マスタ
                    '.SIT_SEIDOKU_HYOUJI_N = ""
                    .YUUBIN_N.Text = DT.SIT_YUUBIN_N
                    .KJYU_N.Text = DT.SIT_KJYU_N
                    .NJYU_N.Text = DT.SIT_NJYU_N
                    '.SIT_TKOUKAN_NO_N = ""
                    .DENWA_N.Text = DT.SIT_DENWA_N
                    .FAX_N.Text = DT.SIT_FAX_N
                    '.SIT_TENPO_ZOKUSEI_N = ""
                    '.SIT_JIKOU_HYOUJI_N = ""
                    '.SIT_FURI_HYOUJI_N = ""
                    '.SIT_SYUUTE_HYOUJI_N = ""
                    '.SIT_KAWASE_HYOUJI_N = ""
                    '.SIT_DAITE_HYOUJI_N = ""
                    '.SIT_JISIN_HYOUJI_N = ""
                    '.SIT_JC_CODE_N = ""
                    '.SIT_IDO_DATE_N = ""
                    '.SIT_IDO_CODE_N = ""
                    '.SIT_TKOUKAN_NNAME_N = ""

                    .KIN_KNAME_N.Focus()

                    btnKousin.Enabled = True
                    btnDelete.Enabled = True
                    btnAction.Enabled = False
                    btnKin_Fuka_Sansyo.Enabled = False
                    btnSit_Fuka_Sansyo.Enabled = False
                    btnSansyo.Enabled = False
                    KIN_NO_N.Enabled = False
                    KIN_FUKA_N.Enabled = False
                    SIT_NO_N.Enabled = False
                    SIT_FUKA_N.Enabled = False

                End With
            Else
                Dim MSG As String = MSG0096W
                Dim DRet As DialogResult = MessageBox.Show(MSG, msgTitle, _
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.KIN_NO_N.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("(参照)終了", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            Me.ResumeLayout()
            MainLog.Write("(参照)終了", "成功")
        End Try
    End Sub

    '削除ボタン
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Dim MSG As String
        Dim DRet As DialogResult
        Dim MainDB As New CASTCommon.MyOracle

        Try
            MainLog.Write("(削除)開始", "成功")
            Me.SuspendLayout()

            'キー項目／データ項目入力チェック
            If Not FN_CHK_FIRST(CMD.CmdDeleteMode) Then
                Return
            End If

            '金融機関マスタに金融機関コードが存在することを確認
            Dim Dummy As TENMAST_DAT = Nothing
            If Not SET_SELECT_ACTION(CMD.CmdUpdateMode, Dummy, MainDB) Then
                MSG = MSG0034W
                DRet = MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.KIN_NO_N.Focus()
                Return
            End If

            MSG = MSG0007I
            DRet = MessageBox.Show(MSG, msgTitle, _
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
            If DRet = DialogResult.OK Then

                Select Case FN_DELETE_TENMAST(MainDB)
                    Case True
                        MainDB.Commit()
                        MSG = MSG0008I
                        DRet = MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information)

                        Call Clear_gamen()

                        Me.KIN_NO_N.Focus()
                    Case Else
                        MainDB.Rollback()
                        MSG = String.Format(MSG0002E, "削除")
                        DRet = MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select
            End If

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("(削除)例外エラー", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            Me.ResumeLayout()
            MainLog.Write("(削除)終了", "成功")
        End Try
    End Sub


    Private Function Clear_gamen() As Boolean

        Dim MSG As String

        Try

            For Each CTL As Control In Me.Controls
                If TypeOf CTL Is TextBox Then
                    CTL.Text = ""
                ElseIf TypeOf CTL Is Label Then
                    If GCom.NzInt(CTL.Tag) = 1 Then
                        CTL.Text = ""
                    End If
                ElseIf TypeOf CTL Is ComboBox Then
                    With CType(CTL, ComboBox)
                        .SelectedIndex = -1
                    End With
                End If

                If TypeOf CTL Is TabControl Then

                    For Each CTL_ko As Control In CTL.Controls

                        If TypeOf CTL_ko Is TabPage Then

                            For Each CTL_mago As Control In CTL_ko.Controls

                                If TypeOf CTL_mago Is TextBox Then
                                    CTL_mago.Text = ""
                                ElseIf TypeOf CTL_mago Is Label Then
                                    If GCom.NzInt(CTL.Tag) = 1 Then
                                        CTL_mago.Text = ""
                                    End If
                                ElseIf TypeOf CTL_mago Is ComboBox Then
                                    With CType(CTL_mago, ComboBox)
                                        .SelectedIndex = -1
                                    End With
                                End If

                            Next
                        End If
                    Next

                End If

            Next CTL

            SAKUSEI_DATE_N.Text = ""
            KOUSIN_DATE_N.Text = ""

            Select Case GCom.SetComboBox(TEIKEI_KBN_N, "KFUMAST010_提携区分.TXT", True)
                Case 1  'ファイルなし
                    MSG = String.Format(MSG0025E, "提携区分", "KFJKOZA010_提携区分.TXT")
                    MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Case 2  '異常
                    MSG = String.Format(MSG0026E, "提携区分")
                    MessageBox.Show(MSG.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
            End Select

            Select Case GCom.SetComboBox(SYUBETU_N, "KFUMAST010_種別.TXT", True)
                Case 1  'ファイルなし
                    MSG = String.Format(MSG0025E, "種別", "KFJKOZA010_種別.TXT")
                    MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Case 2  '異常
                    MSG = String.Format(MSG0026E, "種別")
                    MessageBox.Show(MSG.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
            End Select

            Select Case GCom.SetComboBox(TIKU_CODE_N, "KFUMAST010_地区コード.TXT", True)
                Case 1  'ファイルなし
                    MSG = String.Format(MSG0025E, "種別", "KFJKOZA010_地区コード.TXT")
                    MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Case 2  '異常
                    MSG = String.Format(MSG0026E, "地区コード")
                    MessageBox.Show(MSG.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
            End Select

            btnKousin.Enabled = False
            btnDelete.Enabled = False
            btnAction.Enabled = True
            btnKin_Fuka_Sansyo.Enabled = True
            btnSit_Fuka_Sansyo.Enabled = True
            btnSansyo.Enabled = True
            KIN_NO_N.Enabled = True
            KIN_FUKA_N.Enabled = True
            SIT_NO_N.Enabled = True
            SIT_FUKA_N.Enabled = True

            KIN_NO_N.Focus()

            Return True

        Catch ex As Exception
            Throw
        End Try
    End Function

    '取消ボタン
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        Try
            MainLog.Write("(取消)開始", "成功")

            If MessageBox.Show(MSG0009I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If

            If Clear_gamen() = False Then
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If


            Application.DoEvents()
            Me.KIN_NO_N.Focus()


        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("(取消)例外エラー", "失敗", ex.Message)
        Finally
            MainLog.Write("(取消)終了", "成功")
        End Try
    End Sub

    '金融機関参照
    Private Sub btnKin_Fuka_Sansyo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKin_Fuka_Sansyo.Click

        Dim MSG As String = ""
        Dim Temp As String = ""
        Dim MainDB As New MyOracle
        Dim oraReader As New MyOracleReader(MainDB)
        Dim sql As New StringBuilder(128)
        Try
            MainLog.Write("(金融機関参照)開始", "成功")

            With KY
                '金融機関コード
                Temp = GCom.NzDec(Me.KIN_NO_N.Text, "")
                If Temp.Length = 0 Then
                    MSG &= MSG0032W
                    Dim DRet As DialogResult = MessageBox.Show(MSG, msgTitle, _
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Application.DoEvents()
                    Me.KIN_NO_N.Focus()
                    Return
                Else
                    .KIN_NO_N = Temp

                    'GCom.GLog.Discription = "金融機関="
                    'GCom.GLog.Discription &= String.Format("{0:0000}", .KIN_NO_N)
                    MSG = MSG0235W.Replace("{0}", String.Format("{0:0000}", .KIN_NO_N))
                End If
            End With

            'MSG = GCom.GLog.Discription & " は、" & Space(5)

            sql.Append("SELECT KIN_NNAME_N")
            sql.Append(", KIN_NO_N")
            sql.Append(", KIN_FUKA_N")
            '2011/03/30 削除日考慮 項目追加 ここから
            sql.Append(", KIN_KNAME_N")
            sql.Append(", KIN_DEL_DATE_N")
            '2011/03/30 削除日考慮 項目追加 ここまで
            sql.Append(" FROM KIN_INFOMAST")
            sql.Append(" WHERE KIN_NO_N = " & SQ(KY.KIN_NO_N))
            '2011/03/30 削除日考慮 金融機関削除日を考慮する ここから
            'sql.Append(" GROUP BY KIN_NNAME_N,KIN_NO_N,KIN_FUKA_N")
            'sql.Append(" ORDER BY KIN_FUKA_N ASC")
            sql.Append(" GROUP BY KIN_KNAME_N,KIN_NNAME_N,KIN_NO_N,KIN_FUKA_N,KIN_DEL_DATE_N")
            sql.Append(" ORDER BY KIN_FUKA_N ASC,KIN_DEL_DATE_N ASC")
            '2011/03/30 削除日考慮 金融機関削除日を考慮する ここまで

            If oraReader.DataReader(sql) = True Then
                Dim onForm As New KFUMAST011
                With onForm
                    .oraReader = oraReader
                    .KFUMAST010 = Me
                    CancelFlg = False
                    .intLoadFlg = 0
                    .ShowDialog()
                End With
                Application.DoEvents()
                Select Case CancelFlg
                    Case True
                        '2011/03/30 支店コードの間違い
                        'Me.KIN_KNAME_N.Focus()
                        Me.SIT_NO_N.Focus()
                    Case Else
                        Me.KIN_FUKA_N.Focus()
                End Select
            Else
                MSG = MSG0037W
                Dim DRet As DialogResult = MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Application.DoEvents()
                Me.KIN_NO_N.Focus()
            End If


        Catch ex As Exception
            MessageBox.Show(MSG0002E.Replace("{0}", "参照"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("(金融機関参照)例外エラー", "失敗", ex.Message)
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLog.Write("(金融機関参照)終了", "成功")

        End Try
    End Sub

    Private Sub btnSit_Fuka_Sansyo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSit_Fuka_Sansyo.Click

        Dim MSG As String = ""
        Dim Temp As String = ""

        Dim MainDB As New MyOracle
        Dim oraReader As New MyOracleReader(MainDB)
        Dim sql As New StringBuilder(128)

        Try
            MainLog.Write("(支店参照)開始", "成功")

            With KY
                '金融機関コード
                Temp = GCom.NzDec(Me.KIN_NO_N.Text, "")
                If Temp.Length = 0 Then
                    MSG = MSG0032W
                    Dim DRet As DialogResult = MessageBox.Show(MSG, msgTitle, _
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Application.DoEvents()
                    Me.KIN_NO_N.Focus()
                    Return
                ElseIf GCom.NzDec(Me.KIN_FUKA_N.Text, "").Length = 0 Then
                    MSG = MSG0050W
                    Dim DRet As DialogResult = MessageBox.Show(MSG, msgTitle, _
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Application.DoEvents()
                    Me.KIN_FUKA_N.Focus()
                    Return
                Else
                    .KIN_NO_N = Temp
                    .KIN_FUKA_N = Me.KIN_FUKA_N.Text
                End If

                '支店コード
                Temp = GCom.NzDec(Me.SIT_NO_N.Text, "")
                If Temp.Length = 0 Then
                    MSG = MSG0035W
                    Dim DRet As DialogResult = MessageBox.Show(MSG, msgTitle, _
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Application.DoEvents()
                    Me.SIT_NO_N.Focus()
                    MainLog.Write("(支店参照)終了", "成功")
                    Return
                Else
                    .SIT_NO_N = Temp
                End If

                '2011/03/30 削除日考慮 金融機関削除日が入力されている場合 ここから
                '金融機関削除日付
                Dim onText(5) As Integer
                onText(0) = GCom.NzInt(Me.KIN_DEL_DATE_N.Text, 0)
                onText(1) = GCom.NzInt(Me.KIN_DEL_DATE_N1.Text, 0)
                onText(2) = GCom.NzInt(Me.KIN_DEL_DATE_N2.Text, 0)

                If onText(0) + onText(1) + onText(2) > 0 Then

                    Dim onDate As Date
                    Dim Ret As Integer = GCom.SET_DATE(onDate, onText)
                    If Ret = -1 Then
                        DT.KIN_KIN_DEL_DATE_N = String.Format("{0:yyyyMMdd}", onDate)
                    Else
                        MSG = MSG0027W
                        MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Select Case Ret
                            Case 0
                                Me.KIN_DEL_DATE_N.Focus()
                            Case 1
                                Me.KIN_DEL_DATE_N1.Focus()
                            Case 2
                                Me.KIN_DEL_DATE_N2.Focus()
                        End Select

                        Return
                    End If
                Else
                    DT.KIN_KIN_DEL_DATE_N = "00000000"
                End If
                '2011/03/30 削除日考慮 金融機関削除日が入力されている場合 ここまで
            End With

            sql.Append("SELECT KIN_INFOMAST.KIN_NNAME_N")
            sql.Append(", SITEN_INFOMAST.SIT_NO_N")
            sql.Append(", SITEN_INFOMAST.SIT_FUKA_N")
            sql.Append(", SITEN_INFOMAST.SIT_NNAME_N")
            sql.Append(", SITEN_INFOMAST.SIT_DEL_DATE_N") '2011/03/30 削除日考慮 項目追加
            sql.Append(" FROM KIN_INFOMAST,SITEN_INFOMAST")
            sql.Append(" WHERE ")
            sql.Append("     KIN_INFOMAST.KIN_NO_N = SITEN_INFOMAST.KIN_NO_N ")
            sql.Append(" AND KIN_INFOMAST.KIN_FUKA_N = SITEN_INFOMAST.KIN_FUKA_N ")
            sql.Append(" AND KIN_INFOMAST.KIN_DEL_DATE_N = " & SQ(DT.KIN_KIN_DEL_DATE_N)) '2011/03/30 削除日考慮 抽出条件追加
            sql.Append(" AND SITEN_INFOMAST.KIN_NO_N = " & SQ(KY.KIN_NO_N))
            sql.Append(" AND SITEN_INFOMAST.KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N))
            sql.Append(" AND SITEN_INFOMAST.SIT_NO_N = " & SQ(KY.SIT_NO_N))
            '2011/03/30 削除日考慮 支店削除日を抽出条件に追加 ここから
            'sql.Append(" GROUP BY KIN_INFOMAST.KIN_NNAME_N,SITEN_INFOMAST.SIT_NO_N,SITEN_INFOMAST.SIT_FUKA_N,SITEN_INFOMAST.SIT_NNAME_N ")
            'sql.Append(" ORDER BY SITEN_INFOMAST.SIT_FUKA_N ASC")
            sql.Append(" GROUP BY KIN_INFOMAST.KIN_NNAME_N,SITEN_INFOMAST.SIT_NO_N,SITEN_INFOMAST.SIT_FUKA_N,SITEN_INFOMAST.SIT_NNAME_N,SITEN_INFOMAST.SIT_DEL_DATE_N ")
            sql.Append(" ORDER BY SITEN_INFOMAST.SIT_FUKA_N ASC,SITEN_INFOMAST.SIT_DEL_DATE_N ASC")
            '2011/03/30 削除日考慮 支店削除日を抽出条件に追加 ここまで

            If oraReader.DataReader(sql) = True Then

                Dim onForm As New KFUMAST011
                With onForm
                    .oraReader = oraReader
                    .KFUMAST010 = Me
                    CancelFlg = False
                    .intLoadFlg = 1
                    .ShowDialog()
                End With
                Application.DoEvents()
                Select Case CancelFlg
                    Case True
                        Me.KIN_KNAME_N.Focus()
                    Case Else
                        Me.SIT_FUKA_N.Focus()
                End Select
            Else
                MSG = MSG0037W
                Dim DRet As DialogResult = MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Application.DoEvents()
                Me.KIN_NO_N.Focus()
            End If

        Catch ex As Exception

            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("(支店参照)終了", "失敗", ex.Message)

        Finally

            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLog.Write("(支店参照)終了", "成功")

        End Try
    End Sub

#End Region

#Region "関数"

    'ZERO埋めする
    Private Sub ZERO_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles KIN_NO_N.Validating, SIT_NO_N.Validating, SIT_FUKA_N.Validating, _
    NEW_KIN_NO_N.Validating, NEW_SIT_NO_N.Validating, NEW_SIT_FUKA_N.Validating, _
        KIN_DEL_DATE_N.Validating, KIN_DEL_DATE_N1.Validating, KIN_DEL_DATE_N2.Validating, _
        SIT_DEL_DATE_N.Validating, SIT_DEL_DATE_N1.Validating, SIT_DEL_DATE_N2.Validating

        Dim OBJ As TextBox = CType(sender, TextBox)
        Call GCom.NzNumberString(OBJ, True)
    End Sub

    '全角文字抹消
    Private Sub KIN_KNAME_N_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles KIN_KNAME_N.Validating, SIT_KNAME_N.Validating, KJYU_N.Validating

        Call GCom.NzCheckString(CType(sender, TextBox))
        '2010/12/24.Sakon　文字評価方法を修正
        Call GCom.CheckZenginChar(CType(sender, TextBox))

    End Sub

    '全角部文字数チェック
    Private Sub KIN_NNAME_N_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles KIN_NNAME_N.Validating, SIT_NNAME_N.Validating, NJYU_N.Validating

        With CType(sender, TextBox)
            .Text = GCom.GetLimitString(.Text, .MaxLength)
        End With

    End Sub

    '金融機関削除日（月）LostFocus
    Private Sub KIN_DEL_DATE_N1_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles KIN_DEL_DATE_N1.LostFocus

        Try

            If FN_CHK_DATE("MONTH") = False Then
                Return
            End If

        Catch ex As Exception
            MainLog.Write("金融機関削除日(月)フォーカス失効", "失敗", ex.Message)
        End Try

    End Sub

    '金融機関削除日（日）LostFocus
    Private Sub KIN_DEL_DATE_N2_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles KIN_DEL_DATE_N2.LostFocus

        Try

            If FN_CHK_DATE("DAY") = False Then
                Return
            End If

        Catch ex As Exception
            MainLog.Write("金融機関削除日(日)フォーカス失効", "失敗", ex.Message)
        End Try

    End Sub

    '支店削除日（月）LostFocus
    Private Sub SIT_DEL_DATE_N1_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles SIT_DEL_DATE_N1.LostFocus

        Try

            If FN_CHK_DATE("MONTH") = False Then
                Return
            End If

        Catch ex As Exception
            MainLog.Write("支店削除日(月)フォーカス失効", "失敗", ex.Message)
        End Try

    End Sub

    '支店削除日（日）LostFocus
    Private Sub SIT_DEL_DATE_N2_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles SIT_DEL_DATE_N2.LostFocus

        Try

            If FN_CHK_DATE("DAY") = False Then
                Return
            End If

        Catch ex As Exception
            MainLog.Write("支店削除日(日)フォーカス失効", "失敗", ex.Message)
        End Try

    End Sub

    '支店削除日（日）LostFocus
    Private Sub SIT_NNAME_N_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles SIT_NNAME_N.LostFocus

        Try

            DENWA_N.Focus()

        Catch ex As Exception
            MainLog.Write("支店名(漢字)フォーカス失効", "失敗", ex.Message)
        End Try

    End Sub

    Private Function FN_CHK_DATE(ByVal TextType As String) As Boolean

        Try
            Dim MSG As String
            Dim DRet As DialogResult

            Select Case TextType
                Case "MONTH"
                    If Me.KIN_DEL_DATE_N1.Text <> "" Then

                        MSG = MSG0022W
                        Select Case CInt(Me.KIN_DEL_DATE_N1.Text)
                            Case 0, Is >= 13
                                DRet = MessageBox.Show(MSG, msgTitle, _
                                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Me.KIN_DEL_DATE_N1.Focus()
                                Return False
                        End Select
                    End If

                    If Me.SIT_DEL_DATE_N1.Text <> "" Then

                        MSG = MSG0022W
                        Select Case CInt(Me.SIT_DEL_DATE_N1.Text)
                            Case 0, Is >= 13
                                DRet = MessageBox.Show(MSG, msgTitle, _
                                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Me.SIT_DEL_DATE_N1.Focus()
                                Return False
                        End Select
                    End If
                Case "DAY"
                    If Me.KIN_DEL_DATE_N2.Text <> "" Then

                        MSG = MSG0025W
                        Select Case CInt(Me.KIN_DEL_DATE_N2.Text)
                            Case 0, Is >= 32
                                DRet = MessageBox.Show(MSG, msgTitle, _
                                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Me.KIN_DEL_DATE_N2.Focus()
                                Return False
                        End Select
                    End If

                    If Me.SIT_DEL_DATE_N2.Text <> "" Then

                        MSG = MSG0025W
                        Select Case CInt(Me.SIT_DEL_DATE_N2.Text)
                            Case 0, Is >= 32
                                DRet = MessageBox.Show(MSG, msgTitle, _
                                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Me.SIT_DEL_DATE_N2.Focus()
                                Return False
                        End Select
                    End If

            End Select
            Return True
        Catch ex As Exception
            MainLog.Write("日付チェック", "失敗", ex.Message)
        End Try
    End Function
    '**************** 2009/10/12 *****
    '
    ' 機能　 ： 画面の入力チェック(金庫CD/支店CD/金融機関名/支店名)
    '
    ' 引数　 ： ARG1 - 呼出モード
    '
    ' 戻り値 ： True = OK, False = NG
    '
    ' 備考　 ： Create 2009/08/28
    '
    Private Function FN_CHK_FIRST(ByVal SEL As Integer) As Boolean
        Dim MSG As String
        Dim DRet As DialogResult
        Dim Index As Integer
        Dim CTL() As Control = New Control(7) {Me.KIN_NO_N, Me.KIN_FUKA_N, Me.SIT_NO_N, Me.SIT_FUKA_N, _
                            Me.KIN_KNAME_N, Me.KIN_NNAME_N, Me.SIT_KNAME_N, Me.SIT_NNAME_N}
        Dim Temp() As String = New String(7) {MSG0032W, MSG0050W, MSG0035W, MSG0051W, _
                MSG0040W, MSG0039W, MSG0042W, MSG0041W}

        Try

            For Index = 0 To 3 Step 1
                If CTL(Index).Text.Length = 0 Then
                    Exit Try
                Else
                    With KY
                        Select Case Index
                            Case 0
                                '金融機関コード
                                .KIN_NO_N = CTL(Index).Text
                                GCom.GLog.Discription = "金融機関="
                                GCom.GLog.Discription &= String.Format("{0:0000}", KY.KIN_NO_N)
                            Case 1
                                '金融機関付加コード
                                .KIN_FUKA_N = CTL(Index).Text
                                GCom.GLog.Discription &= "金融機関付加コード="
                                GCom.GLog.Discription &= String.Format("{0:00}", KY.KIN_FUKA_N)
                            Case 2
                                '支店コード
                                .SIT_NO_N = CTL(Index).Text
                                GCom.GLog.Discription &= "支店="
                                GCom.GLog.Discription &= String.Format("{0:000}", KY.SIT_NO_N)
                            Case 3
                                '支店付加コード
                                .SIT_FUKA_N = CTL(Index).Text
                                GCom.GLog.Discription &= "支店付加コード="
                                GCom.GLog.Discription &= String.Format("{0:00}", KY.SIT_FUKA_N)
                        End Select
                    End With
                End If
            Next Index

            '登録／更新の場合
            If SEL = CMD.CmdInsertMode OrElse SEL = CMD.CmdUpdateMode Then

                For Index = 4 To 7 Step 1
                    If CTL(Index).Text.Length = 0 Then
                        Exit Try
                    Else
                        With DT
                            Select Case Index
                                Case 4
                                    '金融機関名(カナ)

                                    .KIN_KIN_KNAME_N = CTL(Index).Text
                                    '2011/07/04 標準版修正 規定外文字チェック追加 ------------------START
                                    If Not GCom.CheckZenginChar(KIN_KNAME_N) Then
                                        DRet = MessageBox.Show(String.Format(MSG0298W, KIN_KNAME_L.Text), msgTitle, _
                                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                        KIN_KNAME_N.Focus()
                                        Return False
                                    End If
                                    '2011/07/04 標準版修正 規定外文字チェック追加 ------------------END
                                Case 5
                                    '金融機関名(漢字)
                                    .KIN_KIN_NNAME_N = CTL(Index).Text
                                Case 6
                                    '支店名(カナ)
                                    .SIT_SIT_KNAME_N = CTL(Index).Text
                                    '2011/07/04 標準版修正 規定外文字チェック追加 ------------------START
                                    If Not GCom.CheckZenginChar(SIT_KNAME_N) Then
                                        DRet = MessageBox.Show(String.Format(MSG0298W, SIT_KNAME_L.Text), msgTitle, _
                                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                        SIT_KNAME_N.Focus()
                                        Return False
                                    End If
                                    '2011/07/04 標準版修正 規定外文字チェック追加 ------------------END
                                Case 7
                                    '支店名(漢字)
                                    .SIT_SIT_NNAME_N = CTL(Index).Text
                            End Select
                        End With
                    End If
                Next Index

                With DT
                    .KIN_NEW_KIN_NO_N = Me.NEW_KIN_NO_N.Text        '新金融機関コード
                    .KIN_NEW_KIN_FUKA_N = Me.NEW_KIN_FUKA_N.Text    '新金融機関付加コード
                    .SIT_NEW_SIT_NO_N = Me.NEW_SIT_NO_N.Text        '新支店コード
                    .SIT_NEW_SIT_FUKA_N = Me.NEW_SIT_FUKA_N.Text    '新支店付加コード
                    '金融機関削除日付
                    Dim onText(5) As Integer
                    onText(0) = GCom.NzInt(Me.KIN_DEL_DATE_N.Text, 0)
                    onText(1) = GCom.NzInt(Me.KIN_DEL_DATE_N1.Text, 0)
                    onText(2) = GCom.NzInt(Me.KIN_DEL_DATE_N2.Text, 0)

                    If onText(0) + onText(1) + onText(2) > 0 Then

                        Dim onDate As Date
                        Dim Ret As Integer = GCom.SET_DATE(onDate, onText)
                        If Ret = -1 Then
                            .KIN_KIN_DEL_DATE_N = String.Format("{0:yyyyMMdd}", onDate)
                        Else
                            MSG = MSG0027W
                            DRet = MessageBox.Show(MSG, msgTitle, _
                                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Select Case Ret
                                Case 0
                                    Me.KIN_DEL_DATE_N.Focus()
                                Case 1
                                    Me.KIN_DEL_DATE_N1.Focus()
                                Case 2
                                    Me.KIN_DEL_DATE_N2.Focus()
                            End Select

                            Return False
                        End If
                    Else
                        .KIN_KIN_DEL_DATE_N = "00000000"
                    End If

                    '支店削除日
                    onText(0) = GCom.NzInt(Me.SIT_DEL_DATE_N.Text, 0)
                    onText(1) = GCom.NzInt(Me.SIT_DEL_DATE_N1.Text, 0)
                    onText(2) = GCom.NzInt(Me.SIT_DEL_DATE_N2.Text, 0)

                    If onText(0) + onText(1) + onText(2) > 0 Then

                        Dim onDate As Date
                        Dim Ret As Integer = GCom.SET_DATE(onDate, onText)
                        If Ret = -1 Then
                            .SIT_SIT_DEL_DATE_N = String.Format("{0:yyyyMMdd}", onDate)
                        Else
                            MSG = MSG0027W
                            DRet = MessageBox.Show(MSG, msgTitle, _
                                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Select Case Ret
                                Case 0
                                    Me.SIT_DEL_DATE_N.Focus()
                                Case 1
                                    Me.SIT_DEL_DATE_N1.Focus()
                                Case 2
                                    Me.SIT_DEL_DATE_N2.Focus()
                            End Select

                            Return False
                        End If
                    Else
                        .SIT_SIT_DEL_DATE_N = "00000000"
                    End If
                    .SIT_SAKUSEI_DATE_N = ""
                    .SIT_KOUSIN_DATE_N = ""

                    '2016/10/14 saitou RSV2 UPD 運用管理メンテナンス ---------------------------------------- START
                    '更新を行うと、金融機関ファイルから読み込んだ内容が一部クリアされてしまうため、
                    '更新時は画面で変更できない項目は参照時に取得した内容をそのまま使用する。
                    If SEL = CMD.CmdInsertMode Then
                        '登録時は画面設定できない項目は初期値(暫定)で登録

                        '金融機関情報マスタ
                        .KIN_DAIHYO_NO_N = Me.KIN_NO_N.Text      '代表金融機関コード 暫定
                        .KIN_RYAKU_KIN_KNAME_N = ""
                        .KIN_RYAKU_KIN_NNAME_N = ""
                        .KIN_JC1_N = ""
                        .KIN_JC2_N = ""
                        .KIN_JC3_N = ""
                        .KIN_JC4_N = ""
                        .KIN_JC5_N = ""
                        .KIN_JC6_N = ""
                        .KIN_JC7_N = ""
                        .KIN_JC8_N = ""
                        .KIN_JC9_N = ""
                        .KIN_IDO_DATE_N = ""
                        .KIN_IDO_CODE_N = ""
                        .KIN_TEIKEI_KBN_N = GCom.GetComboBox(Me.TEIKEI_KBN_N).ToString  '提携区分
                        .KIN_SYUBETU_N = GCom.GetComboBox(Me.SYUBETU_N).ToString        '種別
                        .KIN_TIKU_CODE_N = GCom.GetComboBox(Me.TIKU_CODE_N).ToString    '地区コード
                        .KIN_TESUU_TANKA_N = Me.TESUU_TANKA_N.Text.Replace(",", "")                      '支払手数料
                        If .KIN_TESUU_TANKA_N.Length = 0 Then .KIN_TESUU_TANKA_N = "0"

                        '支店情報マスタ
                        .SIT_SEIDOKU_HYOUJI_N = "0"     '正読表示 暫定
                        .SIT_YUUBIN_N = GCom.NzStr(Me.YUUBIN_N.Text).Trim   '郵便番号
                        .SIT_KJYU_N = GCom.NzStr(Me.KJYU_N.Text).Trim   'カナ住所
                        .SIT_NJYU_N = GCom.NzStr(Me.NJYU_N.Text).Trim       '漢字住所
                        .SIT_TKOUKAN_NO_N = ""
                        .SIT_DENWA_N = GCom.NzStr(Me.DENWA_N.Text).Trim '電話番号
                        .SIT_FAX_N = GCom.NzStr(Me.FAX_N.Text).Trim     'ＦＡＸ番号
                        .SIT_TENPO_ZOKUSEI_N = "0" '暫定
                        .SIT_JIKOU_HYOUJI_N = "0" '暫定
                        .SIT_FURI_HYOUJI_N = "0" '暫定
                        .SIT_SYUUTE_HYOUJI_N = "0" '暫定
                        .SIT_KAWASE_HYOUJI_N = "0" '暫定
                        .SIT_DAITE_HYOUJI_N = "0" '暫定
                        .SIT_JISIN_HYOUJI_N = "0" '暫定
                        .SIT_JC_CODE_N = ""
                        .SIT_IDO_DATE_N = ""
                        .SIT_IDO_CODE_N = ""
                        .SIT_TKOUKAN_NNAME_N = ""

                    Else
                        '更新時は画面で設定できる項目以外は既存の内容を使用する(変数を初期化しない)

                        '金融機関情報マスタ
                        .KIN_DAIHYO_NO_N = Me.KIN_NO_N.Text      '代表金融機関コード 暫定
                        .KIN_TEIKEI_KBN_N = GCom.GetComboBox(Me.TEIKEI_KBN_N).ToString  '提携区分
                        .KIN_SYUBETU_N = GCom.GetComboBox(Me.SYUBETU_N).ToString        '種別
                        .KIN_TIKU_CODE_N = GCom.GetComboBox(Me.TIKU_CODE_N).ToString    '地区コード
                        .KIN_TESUU_TANKA_N = Me.TESUU_TANKA_N.Text.Replace(",", "")                      '支払手数料
                        If .KIN_TESUU_TANKA_N.Length = 0 Then .KIN_TESUU_TANKA_N = "0"

                        '支店情報マスタ
                        .SIT_YUUBIN_N = GCom.NzStr(Me.YUUBIN_N.Text).Trim   '郵便番号
                        .SIT_KJYU_N = GCom.NzStr(Me.KJYU_N.Text).Trim   'カナ住所
                        .SIT_NJYU_N = GCom.NzStr(Me.NJYU_N.Text).Trim       '漢字住所
                        .SIT_DENWA_N = GCom.NzStr(Me.DENWA_N.Text).Trim '電話番号
                        .SIT_FAX_N = GCom.NzStr(Me.FAX_N.Text).Trim     'ＦＡＸ番号
                    End If

                    ''金融機関情報マスタ
                    '.KIN_DAIHYO_NO_N = Me.KIN_NO_N.Text      '代表金融機関コード 暫定
                    '.KIN_RYAKU_KIN_KNAME_N = ""
                    '.KIN_RYAKU_KIN_NNAME_N = ""
                    '.KIN_JC1_N = ""
                    '.KIN_JC2_N = ""
                    '.KIN_JC3_N = ""
                    '.KIN_JC4_N = ""
                    '.KIN_JC5_N = ""
                    '.KIN_JC6_N = ""
                    '.KIN_JC7_N = ""
                    '.KIN_JC8_N = ""
                    '.KIN_JC9_N = ""
                    '.KIN_IDO_DATE_N = ""
                    '.KIN_IDO_CODE_N = ""
                    '.KIN_TEIKEI_KBN_N = GCom.GetComboBox(Me.TEIKEI_KBN_N).ToString  '提携区分
                    '.KIN_SYUBETU_N = GCom.GetComboBox(Me.SYUBETU_N).ToString        '種別
                    '.KIN_TIKU_CODE_N = GCom.GetComboBox(Me.TIKU_CODE_N).ToString    '地区コード
                    '.KIN_TESUU_TANKA_N = Me.TESUU_TANKA_N.Text.Replace(",", "")                      '支払手数料
                    'If .KIN_TESUU_TANKA_N.Length = 0 Then .KIN_TESUU_TANKA_N = "0"

                    ''支店情報マスタ
                    '.SIT_SEIDOKU_HYOUJI_N = "0"     '正読表示 暫定
                    '.SIT_YUUBIN_N = GCom.NzStr(Me.YUUBIN_N.Text).Trim   '郵便番号
                    '.SIT_KJYU_N = GCom.NzStr(Me.KJYU_N.Text).Trim   'カナ住所
                    '.SIT_NJYU_N = GCom.NzStr(Me.NJYU_N.Text).Trim       '漢字住所
                    '.SIT_TKOUKAN_NO_N = ""
                    '.SIT_DENWA_N = GCom.NzStr(Me.DENWA_N.Text).Trim '電話番号
                    '.SIT_FAX_N = GCom.NzStr(Me.FAX_N.Text).Trim     'ＦＡＸ番号
                    '.SIT_TENPO_ZOKUSEI_N = "0" '暫定
                    '.SIT_JIKOU_HYOUJI_N = "0" '暫定
                    '.SIT_FURI_HYOUJI_N = "0" '暫定
                    '.SIT_SYUUTE_HYOUJI_N = "0" '暫定
                    '.SIT_KAWASE_HYOUJI_N = "0" '暫定
                    '.SIT_DAITE_HYOUJI_N = "0" '暫定
                    '.SIT_JISIN_HYOUJI_N = "0" '暫定
                    '.SIT_JC_CODE_N = ""
                    '.SIT_IDO_DATE_N = ""
                    '.SIT_IDO_CODE_N = ""
                    '.SIT_TKOUKAN_NNAME_N = ""
                    '2016/10/14 saitou RSV2 UPD ------------------------------------------------------------- END

                End With

                '2011/03/30 削除日考慮 参照の場合は削除日を考慮する ここから
            ElseIf SEL = CMD.CmdSelectMode Then
                With DT
                    '金融機関削除日付
                    Dim onText(5) As Integer
                    onText(0) = GCom.NzInt(Me.KIN_DEL_DATE_N.Text, 0)
                    onText(1) = GCom.NzInt(Me.KIN_DEL_DATE_N1.Text, 0)
                    onText(2) = GCom.NzInt(Me.KIN_DEL_DATE_N2.Text, 0)

                    If onText(0) + onText(1) + onText(2) > 0 Then

                        Dim onDate As Date
                        Dim Ret As Integer = GCom.SET_DATE(onDate, onText)
                        If Ret = -1 Then
                            .KIN_KIN_DEL_DATE_N = String.Format("{0:yyyyMMdd}", onDate)
                        Else
                            MSG = MSG0027W
                            DRet = MessageBox.Show(MSG, msgTitle, _
                                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Select Case Ret
                                Case 0
                                    Me.KIN_DEL_DATE_N.Focus()
                                Case 1
                                    Me.KIN_DEL_DATE_N1.Focus()
                                Case 2
                                    Me.KIN_DEL_DATE_N2.Focus()
                            End Select

                            Return False
                        End If
                    Else
                        .KIN_KIN_DEL_DATE_N = "00000000"
                    End If

                    '支店削除日
                    onText(0) = GCom.NzInt(Me.SIT_DEL_DATE_N.Text, 0)
                    onText(1) = GCom.NzInt(Me.SIT_DEL_DATE_N1.Text, 0)
                    onText(2) = GCom.NzInt(Me.SIT_DEL_DATE_N2.Text, 0)

                    If onText(0) + onText(1) + onText(2) > 0 Then

                        Dim onDate As Date
                        Dim Ret As Integer = GCom.SET_DATE(onDate, onText)
                        If Ret = -1 Then
                            .SIT_SIT_DEL_DATE_N = String.Format("{0:yyyyMMdd}", onDate)
                        Else
                            MSG = MSG0027W
                            DRet = MessageBox.Show(MSG, msgTitle, _
                                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Select Case Ret
                                Case 0
                                    Me.SIT_DEL_DATE_N.Focus()
                                Case 1
                                    Me.SIT_DEL_DATE_N1.Focus()
                                Case 2
                                    Me.SIT_DEL_DATE_N2.Focus()
                            End Select

                            Return False
                        End If
                    Else
                        .SIT_SIT_DEL_DATE_N = "00000000"
                    End If
                End With
                '2011/03/30 削除日考慮 参照の場合は削除日を考慮する ここまで
            End If

            Return True

        Catch ex As Exception
            MainLog.Write("(入力チェック)", "失敗", ex.Message)
            Return False
        End Try

        MSG = Temp(Index)
        DRet = MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        CTL(Index).Focus()

        Return False
    End Function

    '
    ' 機能　 ： 金融機関マスタの参照
    '
    ' 引数　 ： ARG1 - 呼出モード
    ' 　　　 　 ARG2 - DB格納現在値
    '
    ' 戻り値 ： OK = True, NG = False
    '
    ' 備考　 ： INF.DT() 第一配列に参照値を格納する。
    '
    Private Function SET_SELECT_ACTION(ByVal SEL As Integer, ByRef CurInf As TENMAST_DAT, ByVal db As CASTCommon.MyOracle) As Boolean

        Dim oraReader As New CASTCommon.MyOracleReader(db)
        Dim sql As New StringBuilder(128)

        Try

            sql.Append("SELECT ")
            '金融機関マスタ
            sql.Append("  KIN_INFOMAST.KIN_KNAME_N")
            sql.Append(", KIN_INFOMAST.KIN_NNAME_N")
            sql.Append(", SITEN_INFOMAST.SIT_KNAME_N")
            sql.Append(", SITEN_INFOMAST.SIT_NNAME_N")
            sql.Append(", KIN_INFOMAST.NEW_KIN_NO_N")
            sql.Append(", KIN_INFOMAST.NEW_KIN_FUKA_N")
            sql.Append(", SITEN_INFOMAST.NEW_SIT_NO_N")
            sql.Append(", SITEN_INFOMAST.NEW_SIT_FUKA_N")
            sql.Append(", KIN_INFOMAST.KIN_DEL_DATE_N")
            sql.Append(", SITEN_INFOMAST.SIT_DEL_DATE_N")
            sql.Append(", SITEN_INFOMAST.SAKUSEI_DATE_N")
            sql.Append(", SITEN_INFOMAST.KOUSIN_DATE_N")

            '金融機関情報マスタ
            sql.Append(", KIN_INFOMAST.DAIHYO_NO_N")
            sql.Append(", KIN_INFOMAST.RYAKU_KIN_KNAME_N")
            sql.Append(", KIN_INFOMAST.RYAKU_KIN_NNAME_N")
            sql.Append(", KIN_INFOMAST.JC1_N")
            sql.Append(", KIN_INFOMAST.JC2_N")
            sql.Append(", KIN_INFOMAST.JC3_N")
            sql.Append(", KIN_INFOMAST.JC4_N")
            sql.Append(", KIN_INFOMAST.JC5_N")
            sql.Append(", KIN_INFOMAST.JC6_N")
            sql.Append(", KIN_INFOMAST.JC7_N")
            sql.Append(", KIN_INFOMAST.JC8_N")
            sql.Append(", KIN_INFOMAST.JC9_N")
            sql.Append(", KIN_INFOMAST.KIN_IDO_DATE_N")
            sql.Append(", KIN_INFOMAST.KIN_IDO_CODE_N")
            sql.Append(", KIN_INFOMAST.TEIKEI_KBN_N")
            sql.Append(", KIN_INFOMAST.SYUBETU_N")
            sql.Append(", KIN_INFOMAST.TIKU_CODE_N")
            sql.Append(", KIN_INFOMAST.TESUU_TANKA_N")

            '支店情報マスタ
            sql.Append(", SITEN_INFOMAST.KIN_NO_N")
            sql.Append(", SITEN_INFOMAST.KIN_FUKA_N")
            sql.Append(", SITEN_INFOMAST.SIT_NO_N")
            sql.Append(", SITEN_INFOMAST.SIT_FUKA_N")
            sql.Append(", SITEN_INFOMAST.SEIDOKU_HYOUJI_N")
            sql.Append(", SITEN_INFOMAST.YUUBIN_N")
            sql.Append(", SITEN_INFOMAST.KJYU_N")
            sql.Append(", SITEN_INFOMAST.NJYU_N")
            sql.Append(", SITEN_INFOMAST.TKOUKAN_NO_N")
            sql.Append(", SITEN_INFOMAST.DENWA_N")
            sql.Append(", SITEN_INFOMAST.FAX_N")
            sql.Append(", SITEN_INFOMAST.TENPO_ZOKUSEI_N")
            sql.Append(", SITEN_INFOMAST.JIKOU_HYOUJI_N")
            sql.Append(", SITEN_INFOMAST.FURI_HYOUJI_N")
            sql.Append(", SITEN_INFOMAST.SYUUTE_HYOUJI_N")
            sql.Append(", SITEN_INFOMAST.KAWASE_HYOUJI_N")
            sql.Append(", SITEN_INFOMAST.DAITE_HYOUJI_N")
            sql.Append(", SITEN_INFOMAST.JISIN_HYOUJI_N")
            sql.Append(", SITEN_INFOMAST.JC_CODE_N")
            sql.Append(", SITEN_INFOMAST.SIT_IDO_DATE_N")
            sql.Append(", SITEN_INFOMAST.SIT_IDO_CODE_N")
            sql.Append(", SITEN_INFOMAST.TKOUKAN_NNAME_N")

            sql.Append(" FROM ")
            sql.Append(" KIN_INFOMAST")
            sql.Append(" ,SITEN_INFOMAST")

            sql.Append(" WHERE ")
            sql.Append(" KIN_INFOMAST.KIN_NO_N = SITEN_INFOMAST.KIN_NO_N")
            sql.Append(" AND KIN_INFOMAST.KIN_FUKA_N = SITEN_INFOMAST.KIN_FUKA_N")
            sql.Append(" AND KIN_INFOMAST.KIN_NO_N = " & SQ(KY.KIN_NO_N))
            sql.Append(" AND KIN_INFOMAST.KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N))
            '2011/03/30 削除日考慮 金融機関削除日を抽出条件に追加 ここから
            If SEL = CMD.CmdUpdateMode Then
                '更新の場合は、退避した値を抽出条件にする
                sql.Append(" AND KIN_INFOMAST.KIN_DEL_DATE_N = " & SQ(DT.OLD_KIN_DEL_DATE_N))
            Else
                sql.Append(" AND KIN_INFOMAST.KIN_DEL_DATE_N = " & SQ(DT.KIN_KIN_DEL_DATE_N))
            End If
            '2011/03/30 削除日考慮 金融機関削除日を抽出条件に追加 ここまで
            sql.Append(" AND SITEN_INFOMAST.SIT_NO_N = " & SQ(KY.SIT_NO_N))
            sql.Append(" AND SITEN_INFOMAST.SIT_FUKA_N = " & SQ(KY.SIT_FUKA_N))
            '2011/03/30 削除日考慮 支店削除日を抽出条件に追加 ここから
            If SEL = CMD.CmdUpdateMode Then
                '更新の場合は、退避した値を抽出条件にする
                sql.Append(" AND SITEN_INFOMAST.SIT_DEL_DATE_N = " & SQ(DT.OLD_SIT_DEL_DATE_N))
            Else
                sql.Append(" AND SITEN_INFOMAST.SIT_DEL_DATE_N = " & SQ(DT.SIT_SIT_DEL_DATE_N))
            End If
            '2011/03/30 削除日考慮 支店削除日を抽出条件に追加 ここまで

            If oraReader.DataReader(sql) = True Then
                If SEL = CMD.CmdSelectMode OrElse SEL = CMD.CmdUpdateMode Then

                    With CurInf

                        .KIN_KIN_KNAME_N = oraReader.GetString("KIN_KNAME_N")
                        .KIN_KIN_NNAME_N = oraReader.GetString("KIN_NNAME_N")
                        .SIT_SIT_KNAME_N = oraReader.GetString("SIT_KNAME_N")
                        .SIT_SIT_NNAME_N = oraReader.GetString("SIT_NNAME_N")
                        .KIN_NEW_KIN_NO_N = oraReader.GetString("NEW_KIN_NO_N")
                        .KIN_NEW_KIN_FUKA_N = oraReader.GetString("NEW_KIN_FUKA_N")
                        .SIT_NEW_SIT_NO_N = oraReader.GetString("NEW_SIT_NO_N")
                        .SIT_NEW_SIT_FUKA_N = oraReader.GetString("NEW_SIT_FUKA_N")
                        .KIN_KIN_DEL_DATE_N = oraReader.GetString("KIN_DEL_DATE_N")
                        .SIT_SIT_DEL_DATE_N = oraReader.GetString("SIT_DEL_DATE_N")
                        .SIT_SAKUSEI_DATE_N = oraReader.GetString("SAKUSEI_DATE_N")
                        .SIT_KOUSIN_DATE_N = oraReader.GetString("KOUSIN_DATE_N")

                        '金融機関情報マスタ
                        .KIN_DAIHYO_NO_N = oraReader.GetString("DAIHYO_NO_N")
                        .KIN_RYAKU_KIN_KNAME_N = oraReader.GetString("RYAKU_KIN_KNAME_N")
                        .KIN_RYAKU_KIN_NNAME_N = oraReader.GetString("RYAKU_KIN_NNAME_N")
                        .KIN_JC1_N = oraReader.GetString("JC1_N")
                        .KIN_JC2_N = oraReader.GetString("JC2_N")
                        .KIN_JC3_N = oraReader.GetString("JC3_N")
                        .KIN_JC4_N = oraReader.GetString("JC4_N")
                        .KIN_JC5_N = oraReader.GetString("JC5_N")
                        .KIN_JC6_N = oraReader.GetString("JC6_N")
                        .KIN_JC7_N = oraReader.GetString("JC7_N")
                        .KIN_JC8_N = oraReader.GetString("JC8_N")
                        .KIN_JC9_N = oraReader.GetString("JC9_N")
                        .KIN_IDO_DATE_N = oraReader.GetString("KIN_IDO_DATE_N")
                        .KIN_IDO_CODE_N = oraReader.GetString("KIN_IDO_CODE_N")
                        .KIN_TEIKEI_KBN_N = oraReader.GetString("TEIKEI_KBN_N")
                        .KIN_SYUBETU_N = oraReader.GetString("SYUBETU_N")
                        .KIN_TIKU_CODE_N = oraReader.GetString("TIKU_CODE_N")
                        .KIN_TESUU_TANKA_N = oraReader.GetString("TESUU_TANKA_N")

                        '支店情報マスタ

                        .SIT_SEIDOKU_HYOUJI_N = oraReader.GetString("SEIDOKU_HYOUJI_N")
                        .SIT_YUUBIN_N = oraReader.GetString("YUUBIN_N")
                        .SIT_KJYU_N = oraReader.GetString("KJYU_N")
                        .SIT_NJYU_N = oraReader.GetString("NJYU_N")
                        .SIT_TKOUKAN_NO_N = oraReader.GetString("TKOUKAN_NO_N")
                        .SIT_DENWA_N = oraReader.GetString("DENWA_N")
                        .SIT_FAX_N = oraReader.GetString("FAX_N")
                        .SIT_TENPO_ZOKUSEI_N = oraReader.GetString("TENPO_ZOKUSEI_N")
                        .SIT_JIKOU_HYOUJI_N = oraReader.GetString("JIKOU_HYOUJI_N")
                        .SIT_FURI_HYOUJI_N = oraReader.GetString("FURI_HYOUJI_N")
                        .SIT_SYUUTE_HYOUJI_N = oraReader.GetString("SYUUTE_HYOUJI_N")
                        .SIT_KAWASE_HYOUJI_N = oraReader.GetString("KAWASE_HYOUJI_N")
                        .SIT_DAITE_HYOUJI_N = oraReader.GetString("DAITE_HYOUJI_N")
                        .SIT_JISIN_HYOUJI_N = oraReader.GetString("JISIN_HYOUJI_N")
                        .SIT_JC_CODE_N = oraReader.GetString("JC_CODE_N")
                        .SIT_IDO_DATE_N = oraReader.GetString("SIT_IDO_DATE_N")
                        .SIT_IDO_CODE_N = oraReader.GetString("SIT_IDO_CODE_N")
                        .SIT_TKOUKAN_NNAME_N = oraReader.GetString("TKOUKAN_NNAME_N")

                        'Select Case .KIN_TEIKEI_KBN_N
                        '    Case "0"
                        '        .KIN_TEIKEI_KBN_N = "0"
                        '    Case "1"
                        '        .KIN_TEIKEI_KBN_N = "1"
                        '    Case Else
                        '        .KIN_TEIKEI_KBN_N = "9" '暫定 2009.10.08
                        'End Select

                    End With
                End If
                Return True
            End If
        Catch ex As Exception
            Throw
        Finally

            If Not oraReader Is Nothing Then oraReader.Close()

        End Try

        Return False
    End Function

    '2011/03/30 削除日考慮 更新先のレコードが登録されているかチェックする ここから
    Private Function SET_SELECT_ACTION(ByVal db As CASTCommon.MyOracle) As Boolean

        Dim oraReader As New CASTCommon.MyOracleReader(db)
        Dim sql As New StringBuilder(128)

        Try

            sql.Append("SELECT '1' ")

            sql.Append(" FROM ")
            sql.Append(" KIN_INFOMAST")
            sql.Append(" ,SITEN_INFOMAST")

            sql.Append(" WHERE ")
            sql.Append(" KIN_INFOMAST.KIN_NO_N = SITEN_INFOMAST.KIN_NO_N")
            sql.Append(" AND KIN_INFOMAST.KIN_FUKA_N = SITEN_INFOMAST.KIN_FUKA_N")
            sql.Append(" AND KIN_INFOMAST.KIN_NO_N = " & SQ(KY.KIN_NO_N))
            sql.Append(" AND KIN_INFOMAST.KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N))
            sql.Append(" AND KIN_INFOMAST.KIN_DEL_DATE_N = " & SQ(DT.KIN_KIN_DEL_DATE_N))
            sql.Append(" AND SITEN_INFOMAST.SIT_NO_N = " & SQ(KY.SIT_NO_N))
            sql.Append(" AND SITEN_INFOMAST.SIT_FUKA_N = " & SQ(KY.SIT_FUKA_N))
            sql.Append(" AND SITEN_INFOMAST.SIT_DEL_DATE_N = " & SQ(DT.SIT_SIT_DEL_DATE_N))

            If oraReader.DataReader(sql) = True Then
                Return True
            End If
        Catch ex As Exception
            Throw
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

        Return False
    End Function
    '2011/03/30 削除日考慮 更新先のレコードが登録されているかチェックする ここまで

    '
    ' 機能　 ： セットした値を金融機関マスタに登録する
    '
    ' 引数　 ： ARG1 - なし
    '
    ' 戻り値 ： True = OK, False = NG
    '
    ' 備考　 ： 
    '
    Private Function FN_INSERT_TENMAST(ByVal db As CASTCommon.MyOracle) As Boolean

        Dim sql As StringBuilder
        Dim nRet As Integer = -1

        Try

            'sql = New StringBuilder(128)
            'sql.Append("INSERT INTO TENMAST")
            'sql.Append(" (KIN_NO_N")
            'sql.Append(", KIN_FUKA_N")
            'sql.Append(", SIT_NO_N")
            'sql.Append(", SIT_FUKA_N")
            'sql.Append(", KIN_KNAME_N")
            'sql.Append(", KIN_NNAME_N")
            'sql.Append(", SIT_KNAME_N")
            'sql.Append(", SIT_NNAME_N")
            'sql.Append(", NEW_KIN_NO_N")
            'sql.Append(", NEW_KIN_FUKA_N")
            'sql.Append(", NEW_SIT_NO_N")
            'sql.Append(", NEW_SIT_FUKA_N")
            'sql.Append(", KIN_DEL_DATE_N")
            'sql.Append(", SIT_DEL_DATE_N")
            'sql.Append(", SAKUSEI_DATE_N")
            'sql.Append(", KOUSIN_DATE_N")
            'sql.Append(") VALUES")
            'sql.Append(" (" & SQ(KY.KIN_NO_N))
            'sql.Append(", " & SQ(KY.KIN_FUKA_N))
            'sql.Append(", " & SQ(KY.SIT_NO_N))
            'sql.Append(", " & SQ(KY.SIT_FUKA_N))
            'sql.Append(", " & SQ(DT.TENMAST_KIN_KNAME_N))
            'sql.Append(", " & SQ(DT.TENMAST_KIN_NNAME_N))
            'sql.Append(", " & SQ(DT.TENMAST_SIT_KNAME_N))
            'sql.Append(", " & SQ(DT.TENMAST_SIT_NNAME_N))
            'sql.Append(", " & SQ(DT.TENMAST_NEW_KIN_NO_N))
            'sql.Append(", " & SQ(DT.TENMAST_NEW_KIN_FUKA_N))
            'sql.Append(", " & SQ(DT.TENMAST_NEW_SIT_NO_N))
            'sql.Append(", " & SQ(DT.TENMAST_NEW_SIT_FUKA_N))
            'sql.Append(", " & SQ(DT.TENMAST_KIN_DEL_DATE_N))
            'sql.Append(", " & SQ(DT.TENMAST_SIT_DEL_DATE_N))
            'sql.Append(", TO_CHAR(SYSDATE, 'yyyymmdd')")
            'sql.Append(", TO_CHAR(SYSDATE, 'yyyymmdd')")
            'sql.Append(")")

            'nRet = db.ExecuteNonQuery(sql)
            'If nRet > 0 Then
            '    '正常
            'ElseIf nRet < 0 Then
            '    MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタ登録関数))", "失敗", "予期せぬエラー エラーコード" & nRet)
            '    Return False
            'End If

            '金融機関情報マスタ
            sql = New StringBuilder(128)

            sql.Append("MERGE INTO KIN_INFOMAST")
            sql.Append(" USING (SELECT")
            sql.Append(" " & SQ(KY.KIN_NO_N) & " KIN_NO")
            sql.Append("," & SQ(KY.KIN_FUKA_N) & " KIN_FUKA")
            sql.Append("," & SQ(DT.KIN_KIN_DEL_DATE_N) & " KIN_DEL_DATE") '2011/03/30 削除日考慮 金融機関削除日を抽出条件に追加
            sql.Append(" FROM DUAL) WORK ")
            sql.Append(" ON (")
            sql.Append("     KIN_INFOMAST.KIN_NO_N = WORK.KIN_NO ")
            sql.Append(" AND KIN_INFOMAST.KIN_FUKA_N = WORK.KIN_FUKA ")
            sql.Append(" AND KIN_INFOMAST.KIN_DEL_DATE_N = WORK.KIN_DEL_DATE ") '2011/03/30 削除日考慮 金融機関削除日を抽出条件に追加
            sql.Append(" )")
            'UPDATE句
            sql.Append(" WHEN MATCHED THEN")
            sql.Append(" UPDATE SET")
            '2009.10.09 現在のところ以下の項目しか画面で変更できないので
            sql.Append("  TEIKEI_KBN_N = " & SQ(DT.KIN_TEIKEI_KBN_N))
            sql.Append(", SYUBETU_N = " & SQ(DT.KIN_SYUBETU_N))
            sql.Append(", TIKU_CODE_N = " & SQ(DT.KIN_TIKU_CODE_N))
            sql.Append(", TESUU_TANKA_N = " & SQ(DT.KIN_TESUU_TANKA_N))
            sql.Append(", KOUSIN_DATE_N = " & SQ(System.DateTime.Now.ToString("yyyyMMdd")))
            'INSERT句
            sql.Append(" WHEN NOT MATCHED THEN")
            sql.Append(" INSERT ")
            sql.Append(" (KIN_NO_N")
            sql.Append(", KIN_FUKA_N")
            sql.Append(", DAIHYO_NO_N")
            sql.Append(", KIN_KNAME_N")
            sql.Append(", KIN_NNAME_N")
            sql.Append(", RYAKU_KIN_KNAME_N")
            sql.Append(", RYAKU_KIN_NNAME_N")
            sql.Append(", JC1_N")
            sql.Append(", JC2_N")
            sql.Append(", JC3_N")
            sql.Append(", JC4_N")
            sql.Append(", JC5_N")
            sql.Append(", JC6_N")
            sql.Append(", JC7_N")
            sql.Append(", JC8_N")
            sql.Append(", JC9_N")
            sql.Append(", KIN_IDO_DATE_N")
            sql.Append(", KIN_IDO_CODE_N")
            sql.Append(", NEW_KIN_NO_N")
            sql.Append(", NEW_KIN_FUKA_N")
            sql.Append(", KIN_DEL_DATE_N")
            sql.Append(", TEIKEI_KBN_N")
            sql.Append(", SYUBETU_N")
            sql.Append(", TIKU_CODE_N")
            sql.Append(", TESUU_TANKA_N")
            sql.Append(", SAKUSEI_DATE_N")
            sql.Append(", KOUSIN_DATE_N")
            sql.Append(") VALUES")
            sql.Append(" (" & SQ(KY.KIN_NO_N))
            sql.Append(", " & SQ(KY.KIN_FUKA_N))
            sql.Append(", " & SQ(DT.KIN_DAIHYO_NO_N))
            sql.Append(", " & SQ(DT.KIN_KIN_KNAME_N))
            sql.Append(", " & SQ(DT.KIN_KIN_NNAME_N))
            sql.Append(", " & SQ(DT.KIN_RYAKU_KIN_KNAME_N))
            sql.Append(", " & SQ(DT.KIN_RYAKU_KIN_NNAME_N))
            sql.Append(", " & SQ(DT.KIN_JC1_N))
            sql.Append(", " & SQ(DT.KIN_JC2_N))
            sql.Append(", " & SQ(DT.KIN_JC3_N))
            sql.Append(", " & SQ(DT.KIN_JC4_N))
            sql.Append(", " & SQ(DT.KIN_JC5_N))
            sql.Append(", " & SQ(DT.KIN_JC6_N))
            sql.Append(", " & SQ(DT.KIN_JC7_N))
            sql.Append(", " & SQ(DT.KIN_JC8_N))
            sql.Append(", " & SQ(DT.KIN_JC9_N))
            sql.Append(", " & SQ(DT.KIN_IDO_DATE_N))
            sql.Append(", " & SQ(DT.KIN_IDO_CODE_N))
            sql.Append(", " & SQ(DT.KIN_NEW_KIN_NO_N))
            sql.Append(", " & SQ(DT.KIN_NEW_KIN_FUKA_N))
            sql.Append(", " & SQ(DT.KIN_KIN_DEL_DATE_N))
            sql.Append(", " & SQ(DT.KIN_TEIKEI_KBN_N))
            sql.Append(", " & SQ(DT.KIN_SYUBETU_N))
            sql.Append(", " & SQ(DT.KIN_TIKU_CODE_N))
            sql.Append(", " & SQ(DT.KIN_TESUU_TANKA_N))
            sql.Append(", " & SQ(System.DateTime.Now.ToString("yyyyMMdd")))
            sql.Append(", " & SQ(System.DateTime.Now.ToString("yyyyMMdd")))
            sql.Append(")")

            nRet = db.ExecuteNonQuery(sql)
            If nRet > 0 Then
                '正常

            ElseIf nRet < 0 Then
                MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタ登録関数))", "失敗", "予期せぬエラー エラーコード" & nRet)
                Return False
            End If

            '支店情報マスタ
            sql = New StringBuilder(128)
            sql.Append("INSERT INTO SITEN_INFOMAST")
            sql.Append(" (KIN_NO_N")
            sql.Append(", KIN_FUKA_N")
            sql.Append(", SIT_NO_N")
            sql.Append(", SIT_FUKA_N")
            sql.Append(", SIT_KNAME_N")
            sql.Append(", SIT_NNAME_N")
            sql.Append(", SEIDOKU_HYOUJI_N")
            sql.Append(", YUUBIN_N")
            sql.Append(", KJYU_N")
            sql.Append(", NJYU_N")
            sql.Append(", TKOUKAN_NO_N")
            sql.Append(", DENWA_N")
            sql.Append(", FAX_N")
            sql.Append(", TENPO_ZOKUSEI_N")
            sql.Append(", JIKOU_HYOUJI_N")
            sql.Append(", FURI_HYOUJI_N")
            sql.Append(", SYUUTE_HYOUJI_N")
            sql.Append(", KAWASE_HYOUJI_N")
            sql.Append(", DAITE_HYOUJI_N")
            sql.Append(", JISIN_HYOUJI_N")
            sql.Append(", JC_CODE_N")
            sql.Append(", SIT_IDO_DATE_N")
            sql.Append(", SIT_IDO_CODE_N")
            sql.Append(", NEW_KIN_NO_N")
            sql.Append(", NEW_KIN_FUKA_N")
            sql.Append(", NEW_SIT_NO_N")
            sql.Append(", NEW_SIT_FUKA_N")
            sql.Append(", SIT_DEL_DATE_N")
            sql.Append(", TKOUKAN_NNAME_N")
            sql.Append(", SAKUSEI_DATE_N")
            sql.Append(", KOUSIN_DATE_N")
            sql.Append(") VALUES")
            sql.Append(" (" & SQ(KY.KIN_NO_N))
            sql.Append(", " & SQ(KY.KIN_FUKA_N))
            sql.Append(", " & SQ(KY.SIT_NO_N))
            sql.Append(", " & SQ(KY.SIT_FUKA_N))
            sql.Append(", " & SQ(DT.SIT_SIT_KNAME_N))
            sql.Append(", " & SQ(DT.SIT_SIT_NNAME_N))
            sql.Append(", " & SQ(DT.SIT_SEIDOKU_HYOUJI_N))
            sql.Append(", " & SQ(DT.SIT_YUUBIN_N))
            sql.Append(", " & SQ(DT.SIT_KJYU_N))
            sql.Append(", " & SQ(DT.SIT_NJYU_N))
            sql.Append(", " & SQ(DT.SIT_TKOUKAN_NO_N))
            sql.Append(", " & SQ(DT.SIT_DENWA_N))
            sql.Append(", " & SQ(DT.SIT_FAX_N))
            sql.Append(", " & SQ(DT.SIT_TENPO_ZOKUSEI_N))
            sql.Append(", " & SQ(DT.SIT_JIKOU_HYOUJI_N))
            sql.Append(", " & SQ(DT.SIT_FURI_HYOUJI_N))
            sql.Append(", " & SQ(DT.SIT_SYUUTE_HYOUJI_N))
            sql.Append(", " & SQ(DT.SIT_KAWASE_HYOUJI_N))
            sql.Append(", " & SQ(DT.SIT_DAITE_HYOUJI_N))
            sql.Append(", " & SQ(DT.SIT_JISIN_HYOUJI_N))
            sql.Append(", " & SQ(DT.SIT_JC_CODE_N))
            sql.Append(", " & SQ(DT.SIT_IDO_DATE_N))
            sql.Append(", " & SQ(DT.SIT_IDO_CODE_N))
            sql.Append(", " & SQ(DT.KIN_NEW_KIN_NO_N))
            sql.Append(", " & SQ(DT.KIN_NEW_KIN_FUKA_N))
            sql.Append(", " & SQ(DT.SIT_NEW_SIT_NO_N))
            sql.Append(", " & SQ(DT.SIT_NEW_SIT_FUKA_N))
            sql.Append(", " & SQ(DT.SIT_SIT_DEL_DATE_N))
            sql.Append(", " & SQ(DT.SIT_TKOUKAN_NNAME_N))
            sql.Append(", " & SQ(System.DateTime.Now.ToString("yyyyMMdd")))
            sql.Append(", " & SQ(System.DateTime.Now.ToString("yyyyMMdd")))
            sql.Append(")")

            nRet = db.ExecuteNonQuery(sql)
            If nRet > 0 Then
                '正常

            ElseIf nRet < 0 Then
                MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタ登録関数))", "失敗", "予期せぬエラー エラーコード" & nRet)
                Return False
            End If

            Return True

        Catch ex As Exception
            Throw
        End Try

    End Function

    '
    ' 機能　 ： セットした値を金融機関マスタから削除する
    '
    ' 引数　 ： ARG1 - なし
    '
    ' 戻り値 ： True = OK, False = NG
    '
    ' 備考　 ： Create 2004/07/15 By FJH, Update 2007.12.27 By Astar
    '
    Private Function FN_DELETE_TENMAST(ByVal db As CASTCommon.MyOracle) As Boolean

        Dim sql As StringBuilder
        Dim nRet As Integer

        Try

            '金融機関マスタ
            'sql = New StringBuilder(128)
            'sql.Append("DELETE TENMAST")
            'sql.Append(" WHERE KIN_NO_N = " & SQ(KY.KIN_NO_N))
            'sql.Append(" AND KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N))
            'sql.Append(" AND SIT_NO_N = " & SQ(KY.SIT_NO_N))
            'sql.Append(" AND SIT_FUKA_N = " & SQ(KY.SIT_FUKA_N))

            'nRet = db.ExecuteNonQuery(sql)
            'If nRet > 0 Then
            '    '正常
            'ElseIf nRet < 0 Then
            '    MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタ削除関数)", "失敗", "予期せぬエラー エラーコード" & nRet)
            '    Return False
            'End If

            '支店情報マスタ
            sql = New StringBuilder(128)
            sql.Append("DELETE SITEN_INFOMAST")
            sql.Append(" WHERE KIN_NO_N = " & SQ(KY.KIN_NO_N))
            sql.Append(" AND KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N))
            sql.Append(" AND SIT_NO_N = " & SQ(KY.SIT_NO_N))
            sql.Append(" AND SIT_FUKA_N = " & SQ(KY.SIT_FUKA_N))
            sql.Append(" AND SIT_DEL_DATE_N = " & SQ(DT.SIT_SIT_DEL_DATE_N)) '2011/03/30 削除日考慮 支店削除日を抽出条件に追加

            nRet = db.ExecuteNonQuery(sql)
            If nRet > 0 Then
                '正常
            ElseIf nRet < 0 Then
                MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタ削除関数)", "失敗", "予期せぬエラー エラーコード" & nRet)
                Return False
            End If

            '金融機関情報マスタ
            sql = New StringBuilder(128)
            sql.Append("DELETE KIN_INFOMAST")
            sql.Append(" WHERE KIN_NO_N = " & SQ(KY.KIN_NO_N))
            sql.Append(" AND KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N))
            sql.Append(" AND KIN_DEL_DATE_N = " & SQ(DT.KIN_KIN_DEL_DATE_N)) '2011/03/30 削除日考慮 金融機関削除日を抽出条件に追加
            sql.Append(" AND NOT EXISTS ")
            sql.Append("(SELECT * FROM SITEN_INFOMAST WHERE KIN_NO_N = " & SQ(KY.KIN_NO_N) & " AND KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N) & ")")

            nRet = db.ExecuteNonQuery(sql)
            If nRet > 0 Then
                '正常
            ElseIf nRet < 0 Then
                MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタ削除関数)", "失敗", "予期せぬエラー エラーコード" & nRet)
                Return False
            End If

            Return True

        Catch ex As Exception
            Throw
        End Try

        Return False
    End Function

    '
    ' 機能　 ： セットした値を金融機関マスタに更新する
    '
    ' 引数　 ： ARG1 - 変更識別
    '
    ' 戻り値 ： True = OK, False = NG
    '
    ' 備考　 ： Create 2004/07/15 By FJH, Update 2007.12.27 By Astar
    '
    Private Function FN_UPDATE_TENMAST(ByVal FLG() As Boolean, ByVal db As CASTCommon.MyOracle) As Boolean

        'Dim sql_TENMAST As New StringBuilder(128)
        Dim sql_KIN_INFOMAST As New StringBuilder(128)
        Dim sql_SITEN_INFOMAST As New StringBuilder(128)
        Dim nRet As Integer = -1

        Dim Counter_TENMAST As Integer = 0
        Dim Counter_KIN_INFOMAST As Integer = 0
        Dim Counter_SITEN_INFOMAST As Integer = 0

        Try
            'sql_TENMAST.Append("UPDATE TENMAST")
            sql_KIN_INFOMAST.Append("UPDATE KIN_INFOMAST")
            sql_SITEN_INFOMAST.Append("UPDATE SITEN_INFOMAST")

            'For Idx As Integer = FLG.GetLowerBound(0) To FLG.GetUpperBound(0) Step 1

            '    If Not FLG(Idx) Then

            '        If Idx <= 9 Then    '金融機関マスタ領域
            '            Select Case Counter_TENMAST
            '                Case 0
            '                    sql_TENMAST.Append(" SET")
            '                Case Else
            '                    sql_TENMAST.Append(",")
            '            End Select

            '            Counter_TENMAST += 1
            '        End If

            '        Select Case Idx
            '            '金融機関マスタ

            '        End Select

            '    End If
            'Next Idx

            'sql_TENMAST.Append(", KOUSIN_DATE_N = TO_CHAR(SYSDATE, 'yyyymmdd')")

            'sql_TENMAST.Append(" WHERE KIN_NO_N = " & SQ(KY.KIN_NO_N))
            'sql_TENMAST.Append(" AND KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N))
            'sql_TENMAST.Append(" AND SIT_NO_N = " & SQ(KY.SIT_NO_N))
            'sql_TENMAST.Append(" AND SIT_FUKA_N = " & SQ(KY.SIT_FUKA_N))


            For Idx As Integer = FLG.GetLowerBound(0) To FLG.GetUpperBound(0) Step 1

                If Not FLG(Idx) Then

                    If Idx <= 22 Then    '金融機関情報マスタ領域
                        Select Case Counter_KIN_INFOMAST
                            Case 0
                                sql_KIN_INFOMAST.Append(" SET")
                            Case Else
                                sql_KIN_INFOMAST.Append(",")
                        End Select

                        Counter_KIN_INFOMAST += 1
                    End If

                    Select Case Idx

                        '金融機関情報マスタ18
                        Case 0
                            sql_KIN_INFOMAST.Append(" KIN_KNAME_N = " & SetItem(DT.KIN_KIN_KNAME_N))

                        Case 1
                            sql_KIN_INFOMAST.Append(" KIN_NNAME_N = " & SetItem(DT.KIN_KIN_NNAME_N))

                        Case 2
                            sql_KIN_INFOMAST.Append(" DAIHYO_NO_N = " & SetItem(DT.KIN_DAIHYO_NO_N))

                        Case 3
                            sql_KIN_INFOMAST.Append(" RYAKU_KIN_KNAME_N = " & SetItem(DT.KIN_RYAKU_KIN_KNAME_N))

                        Case 4
                            sql_KIN_INFOMAST.Append(" RYAKU_KIN_NNAME_N = " & SetItem(DT.KIN_RYAKU_KIN_NNAME_N))

                        Case 5
                            sql_KIN_INFOMAST.Append(" JC1_N = " & SetItem(DT.KIN_JC1_N))

                        Case 6
                            sql_KIN_INFOMAST.Append(" JC2_N = " & SetItem(DT.KIN_JC2_N))

                        Case 7
                            sql_KIN_INFOMAST.Append(" JC3_N = " & SetItem(DT.KIN_JC3_N))

                        Case 8
                            sql_KIN_INFOMAST.Append(" JC4_N = " & SetItem(DT.KIN_JC4_N))

                        Case 9
                            sql_KIN_INFOMAST.Append(" JC5_N = " & SetItem(DT.KIN_JC5_N))

                        Case 10
                            sql_KIN_INFOMAST.Append(" JC6_N = " & SetItem(DT.KIN_JC6_N))

                        Case 11
                            sql_KIN_INFOMAST.Append(" JC7_N = " & SetItem(DT.KIN_JC7_N))

                        Case 12
                            sql_KIN_INFOMAST.Append(" JC8_N = " & SetItem(DT.KIN_JC8_N))

                        Case 13
                            sql_KIN_INFOMAST.Append(" JC9_N = " & SetItem(DT.KIN_JC9_N))

                        Case 14
                            sql_KIN_INFOMAST.Append(" KIN_IDO_DATE_N = " & SetItem(DT.KIN_IDO_DATE_N))

                        Case 15
                            sql_KIN_INFOMAST.Append(" KIN_IDO_CODE_N = " & SetItem(DT.KIN_IDO_CODE_N))

                        Case 16
                            sql_KIN_INFOMAST.Append(" NEW_KIN_NO_N = " & SetItem(DT.KIN_NEW_KIN_NO_N))

                        Case 17
                            sql_KIN_INFOMAST.Append(" NEW_KIN_FUKA_N = " & SetItem(DT.KIN_NEW_KIN_FUKA_N))

                        Case 18
                            sql_KIN_INFOMAST.Append(" KIN_DEL_DATE_N = " & SetItem(DT.KIN_KIN_DEL_DATE_N))

                        Case 19
                            sql_KIN_INFOMAST.Append(" TEIKEI_KBN_N = " & SetItem(DT.KIN_TEIKEI_KBN_N))

                        Case 20
                            sql_KIN_INFOMAST.Append(" SYUBETU_N = " & SetItem(DT.KIN_SYUBETU_N))

                        Case 21
                            sql_KIN_INFOMAST.Append(" TIKU_CODE_N = " & SetItem(DT.KIN_TIKU_CODE_N))

                        Case 22
                            sql_KIN_INFOMAST.Append(" TESUU_TANKA_N = " & SetItem(DT.KIN_TESUU_TANKA_N))

                    End Select

                End If
            Next Idx

            sql_KIN_INFOMAST.Append(" ,KOUSIN_DATE_N = " & SQ(System.DateTime.Now.ToString("yyyyMMdd")))

            sql_KIN_INFOMAST.Append(" WHERE KIN_NO_N = " & SQ(KY.KIN_NO_N))
            sql_KIN_INFOMAST.Append(" AND KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N))
            sql_KIN_INFOMAST.Append(" AND KIN_DEL_DATE_N = " & SQ(DT.OLD_KIN_DEL_DATE_N)) '2011/03/30 削除日考慮 金融機関削除日を抽出条件に追加

            For Idx As Integer = FLG.GetLowerBound(0) To FLG.GetUpperBound(0) Step 1

                If Not FLG(Idx) Then

                    If Idx >= 23 Then    '支店情報マスタ領域

                        Select Case Counter_SITEN_INFOMAST
                            Case 0
                                sql_SITEN_INFOMAST.Append(" SET")
                            Case Else
                                sql_SITEN_INFOMAST.Append(",")
                        End Select

                        Counter_SITEN_INFOMAST += 1
                    End If
                    Select Case Idx

                        '支店情報マスタ18
                        Case 23
                            sql_SITEN_INFOMAST.Append(" SIT_KNAME_N = " & SetItem(DT.SIT_SIT_KNAME_N))

                        Case 24
                            sql_SITEN_INFOMAST.Append(" SIT_NNAME_N = " & SetItem(DT.SIT_SIT_NNAME_N))

                            'Case 25
                            '    sql_KIN_INFOMAST.Append(" NEW_KIN_NO_N = " & SetItem(DT.KIN_NEW_KIN_NO_N))

                            'Case 26
                            '    sql_KIN_INFOMAST.Append(" NEW_KIN_FUKA_N = " & SetItem(DT.KIN_NEW_KIN_FUKA_N))

                        Case 25
                            sql_SITEN_INFOMAST.Append(" SEIDOKU_HYOUJI_N = " & SetItem(DT.SIT_SEIDOKU_HYOUJI_N))

                        Case 26
                            sql_SITEN_INFOMAST.Append(" YUUBIN_N = " & SetItem(DT.SIT_YUUBIN_N))

                        Case 27
                            sql_SITEN_INFOMAST.Append(" KJYU_N = " & SetItem(DT.SIT_KJYU_N))

                        Case 28
                            sql_SITEN_INFOMAST.Append(" NJYU_N = " & SetItem(DT.SIT_NJYU_N))

                        Case 29
                            sql_SITEN_INFOMAST.Append(" TKOUKAN_NO_N = " & SetItem(DT.SIT_TKOUKAN_NO_N))

                        Case 30
                            sql_SITEN_INFOMAST.Append(" DENWA_N = " & SetItem(DT.SIT_DENWA_N))

                        Case 31
                            sql_SITEN_INFOMAST.Append(" FAX_N = " & SetItem(DT.SIT_FAX_N))

                        Case 32
                            sql_SITEN_INFOMAST.Append(" TENPO_ZOKUSEI_N = " & SetItem(DT.SIT_TENPO_ZOKUSEI_N))

                        Case 33
                            sql_SITEN_INFOMAST.Append(" JIKOU_HYOUJI_N = " & SetItem(DT.SIT_JIKOU_HYOUJI_N))

                        Case 34
                            sql_SITEN_INFOMAST.Append(" FURI_HYOUJI_N = " & SetItem(DT.SIT_FURI_HYOUJI_N))

                        Case 35
                            sql_SITEN_INFOMAST.Append(" SYUUTE_HYOUJI_N = " & SetItem(DT.SIT_SYUUTE_HYOUJI_N))

                        Case 36
                            sql_SITEN_INFOMAST.Append(" KAWASE_HYOUJI_N = " & SetItem(DT.SIT_KAWASE_HYOUJI_N))

                        Case 37
                            sql_SITEN_INFOMAST.Append(" DAITE_HYOUJI_N = " & SetItem(DT.SIT_DAITE_HYOUJI_N))

                        Case 38
                            sql_SITEN_INFOMAST.Append(" JISIN_HYOUJI_N = " & SetItem(DT.SIT_JISIN_HYOUJI_N))

                        Case 39
                            sql_SITEN_INFOMAST.Append(" JC_CODE_N = " & SetItem(DT.SIT_JC_CODE_N))

                        Case 40
                            sql_SITEN_INFOMAST.Append(" SIT_IDO_DATE_N = " & SetItem(DT.SIT_IDO_DATE_N))

                        Case 41
                            sql_SITEN_INFOMAST.Append(" SIT_IDO_CODE_N = " & SetItem(DT.SIT_IDO_CODE_N))

                        Case 42
                            sql_SITEN_INFOMAST.Append(" NEW_SIT_NO_N = " & SetItem(DT.SIT_NEW_SIT_NO_N))

                        Case 43
                            sql_SITEN_INFOMAST.Append(" NEW_SIT_FUKA_N = " & SetItem(DT.SIT_NEW_SIT_FUKA_N))

                        Case 44
                            sql_SITEN_INFOMAST.Append(" SIT_DEL_DATE_N = " & SetItem(DT.SIT_SIT_DEL_DATE_N))

                        Case 45
                            sql_SITEN_INFOMAST.Append(" TKOUKAN_NNAME_N = " & SetItem(DT.SIT_TKOUKAN_NNAME_N))

                    End Select

                End If
            Next Idx

            sql_SITEN_INFOMAST.Append(" ,KOUSIN_DATE_N = " & SQ(System.DateTime.Now.ToString("yyyyMMdd")))

            sql_SITEN_INFOMAST.Append(" WHERE KIN_NO_N = " & SQ(KY.KIN_NO_N))
            sql_SITEN_INFOMAST.Append(" AND KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N))
            sql_SITEN_INFOMAST.Append(" AND SIT_NO_N = " & SQ(KY.SIT_NO_N))
            sql_SITEN_INFOMAST.Append(" AND SIT_FUKA_N = " & SQ(KY.SIT_FUKA_N))
            sql_SITEN_INFOMAST.Append(" AND SIT_DEL_DATE_N = " & SQ(DT.OLD_SIT_DEL_DATE_N)) '2011/03/30 削除日考慮 支店削除日を抽出条件に追加

            'If Counter_TENMAST > 0 Then
            '    nRet = db.ExecuteNonQuery(sql_TENMAST)
            '    If nRet > 0 Then
            '        '正常
            '    Else
            '        MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタ更新関数)", "失敗", "予期せぬエラー エラーコード" & nRet)
            '        Return False
            '    End If
            'End If

            If Counter_KIN_INFOMAST > 0 Then
                nRet = db.ExecuteNonQuery(sql_KIN_INFOMAST)
                If nRet > 0 Then
                    '正常
                Else
                    MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタ更新関数)", "失敗", "予期せぬエラー エラーコード" & nRet)
                    Return False
                End If
            End If

            If Counter_SITEN_INFOMAST > 0 Then
                nRet = db.ExecuteNonQuery(sql_SITEN_INFOMAST)
                If nRet > 0 Then
                    '正常
                Else
                    MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタ更新関数)", "失敗", "予期せぬエラー エラーコード" & nRet)
                    Return False
                End If
            End If

            '*** 修正 mitsu 2008/06/18 金融機関名、金融機関削除日更新時 ***
            '同じ金融機関全てを更新する 
            'sql_TENMAST = New StringBuilder(128)
            'Counter_TENMAST = 0
            'sql_TENMAST.Append("UPDATE TENMAST")

            sql_KIN_INFOMAST = New StringBuilder(128)
            Counter_KIN_INFOMAST = 0
            sql_KIN_INFOMAST.Append("UPDATE KIN_INFOMAST")

            'For i As Integer = 0 To FLG.Length - 1
            '    Select Case i
            '        Case 0, 1, 2, 3, 8

            '            If Not FLG(i) Then

            '                Select Case Counter_TENMAST
            '                    Case 0
            '                        sql_TENMAST.Append(" SET")
            '                    Case Else
            '                        sql_TENMAST.Append(",")
            '                End Select

            '                Counter_TENMAST += 1

            '                Select Case i

            '                    Case 0
            '                        sql_TENMAST.Append(" KIN_KNAME_N = " & SetItem(DT.KIN_KIN_KNAME_N))

            '                    Case 1
            '                        sql_TENMAST.Append(" KIN_NNAME_N = " & SetItem(DT.KIN_KIN_NNAME_N))

            '                    Case 2
            '                        sql_TENMAST.Append(" SIT_KNAME_N = " & SetItem(DT.SIT_SIT_KNAME_N))

            '                    Case 3
            '                        sql_TENMAST.Append(" SIT_NNAME_N = " & SetItem(DT.SIT_SIT_NNAME_N))

            '                    Case 8
            '                        sql_TENMAST.Append(" KIN_DEL_DATE_N = " & SetItem(DT.KIN_KIN_DEL_DATE_N))

            '                End Select

            '            End If
            '    End Select
            'Next

            'sql_TENMAST.Append(", KOUSIN_DATE_N = TO_CHAR(SYSDATE, 'yyyymmdd')")
            'sql_TENMAST.Append(" WHERE KIN_NO_N = " & SQ(KY.KIN_NO_N))
            'sql_TENMAST.Append(" AND KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N))

            For i As Integer = 0 To FLG.Length - 1

                Select Case i

                    Case 19, 22

                        If Not FLG(i) Then

                            Select Case Counter_KIN_INFOMAST
                                Case 0
                                    sql_KIN_INFOMAST.Append(" SET")
                                Case Else
                                    sql_KIN_INFOMAST.Append(",")
                            End Select

                            Counter_KIN_INFOMAST += 1

                            Select Case i

                                Case 19
                                    sql_KIN_INFOMAST.Append(" TEIKEI_KBN_N = " & SetItem(DT.KIN_TEIKEI_KBN_N))

                                Case 22
                                    sql_KIN_INFOMAST.Append(" TESUU_TANKA_N = " & SetItem(DT.KIN_TESUU_TANKA_N))

                            End Select

                        End If
                End Select
            Next

            sql_KIN_INFOMAST.Append(" ,KOUSIN_DATE_N = " & SQ(System.DateTime.Now.ToString("yyyyMMdd")))

            sql_KIN_INFOMAST.Append(" WHERE KIN_NO_N = " & SQ(KY.KIN_NO_N))
            sql_KIN_INFOMAST.Append(" AND KIN_FUKA_N = " & SQ(KY.KIN_FUKA_N))
            sql_KIN_INFOMAST.Append(" AND KIN_DEL_DATE_N = " & SQ(DT.OLD_KIN_DEL_DATE_N)) '2011/03/30 削除日考慮 金融機関削除日を抽出条件に追加

            'If Counter_TENMAST > 0 Then
            '    nRet = db.ExecuteNonQuery(sql_TENMAST)
            '    If nRet > 0 Then
            '        '正常
            '    Else
            '        MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタ更新関数)", "失敗", "予期せぬエラー エラーコード" & nRet)
            '        Return False
            '    End If
            'End If

            If Counter_KIN_INFOMAST > 0 Then
                nRet = db.ExecuteNonQuery(sql_KIN_INFOMAST)
                If nRet > 0 Then
                    '正常
                Else
                    MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(マスタ更新関数)", "失敗", "予期せぬエラー エラーコード" & nRet)
                    Return False
                End If
            End If

            Return True

        Catch ex As Exception
            Throw
        End Try

        Return False
    End Function

    '
    ' 機能　 ： 挿入文支援関数
    '
    ' 引数　 ： ARG1 - 設定値
    '
    ' 戻り値 ： SQL文挿入文字列
    '
    ' 備考　 ： なし
    '
    Private Function SetItem(ByVal InfData As String) As String

        Select Case GCom.NzStr(InfData).Trim.Length
            Case 0
                Return "NULL"
            Case Else
                Return "'" & InfData & "'"
        End Select
    End Function

    'タブ間のフォーカス移動
    Private Sub TabIndexSetFocus_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles SIT_DEL_DATE_N2.KeyPress, TESUU_TANKA_N.KeyPress
        Try
            Select Case Microsoft.VisualBasic.Asc(e.KeyChar)
                Case Keys.Tab, Keys.Return
                    With Me.TabControl1
                        Select Case CType(sender, Control).Name
                            Case "SIT_DEL_DATE_N2"
                                .SelectedIndex = 2
                                Me.SYUBETU_N.Focus()

                                'Case "TESUU_TANKA_N"
                                '    .SelectedIndex = 2
                                '    Me.SYUBETU_N.Focus()

                            Case "TESUU_TANKA_N"
                                '.SelectedIndex = 0
                                If btnAction.Enabled = False Then
                                    Me.btnKousin.Focus()
                                Else
                                    Me.btnAction.Focus()
                                End If


                        End Select
                    End With
            End Select
        Catch ex As Exception

        End Try
    End Sub

#End Region

End Class
