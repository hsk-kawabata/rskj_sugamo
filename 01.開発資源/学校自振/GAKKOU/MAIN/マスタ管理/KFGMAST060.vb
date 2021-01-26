Option Explicit On 
Option Strict Off

Imports System.Text
Imports CASTCommon

Public Class KFGMAST060

    Private Enum gintPG_KBN As Integer
        KOBETU = 1
        IKKATU = 2
    End Enum
    Private Enum gintKEKKA As Integer
        OK = 0
        NG = 1
        OTHER = 2
    End Enum

    Private gstrTORIS_CODE As String
    Private gstrFURI_DATE As String

    Private gastrTORIS_CODE_T As String
    Private gastrTORIF_CODE_T As String
    Private gastrITAKU_KNAME_T As String
    Private gastrITAKU_NNAME_T As String
    Private gastrFILE_NAME_T As String
    Private gastrKIGYO_CODE_T As String
    Private gastrFURI_CODE_T As String
    Private gastrBAITAI_CODE_T As String
    Private gastrFMT_KBN_T As String
    Private gastrTAKOU_KBN_T As String
    Private gastrITAKU_CODE_T As String
    Private gastrNS_KBN_T As String
    Private gastrLABEL_KBN As String
    Private gastrITAKU_KIN As String
    Private gastrITAKU_SIT As String
    Private gastrITAKU_KAMOKU As String
    Private gastrITAKU_KOUZA As String
    Private gastrTEKIYO_KBN As String
    Private gastrKTEKIYO As String
    Private gastrNTEKIYO As String
    Private gastrMULTI_KBN As String
    Private gastrNS_KBN As String
    Private gastrCODE_KBN_T As String

    'SCHMAST用データセット
    Private gstrKYUJITU As String
    Private gstrWORK_DATE As String
    Private gSCH_DATA(71) As String


#Region " 共通変数宣言 "

    Private STR請求年月 As String
    Private STR振替日 As String
    Private STR再振替日 As String
    '2010/10/21 契約振替日追加
    Private STR契約振替日 As String

    Private STRスケ区分 As String
    Private STR振替区分 As String
    Private STR学年１ As String
    Private STR学年２ As String
    Private STR学年３ As String
    Private STR学年４ As String
    Private STR学年５ As String
    Private STR学年６ As String
    Private STR学年７ As String
    Private STR学年８ As String
    Private STR学年９ As String
    Private STR１学年 As String
    Private STR２学年 As String
    Private STR３学年 As String
    Private STR４学年 As String
    Private STR５学年 As String
    Private STR６学年 As String
    Private STR７学年 As String
    Private STR８学年 As String
    Private STR９学年 As String

    Private STR年間入力振替日 As String

    'Private STR明細作成予定日 As String
    'Private STRチェック予定日 As String
    'Private STR振替データ作成予定日 As String
    'Private STR不能結果更新予定日 As String
    'Private STR決済予定日 As String
    Private STRW再振替年 As String
    Private STRW再振替月 As String
    Private STRW再振替日 As String
    Private STR処理名 As String
    Private STRYasumi_List(0) As String

    Private str旧振替日(6) As String '2006/11/22
    Private str旧再振日(6) As String '2006/11/22
    Private int旧振替ＩＤ As Integer '2006/11/22
    Private str通常振替日(12) As String '2006/11/22
    Private str通常再振日(12) As String '2006/11/22

    Private str通常再々振日(12) As String '2006/11/30
    Private str特別再々振日(6) As String '2006/11/30
    Private bln年間更新(12) As Boolean '2006/11/30
    Private bln特別更新(6) As Boolean '2006/11/30
    Private bln随時更新(6) As Boolean '2006/11/30

    Private Int_Zengo_Kbn(1) As String

    '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------START
    Private Sai_Zengo_Kbn As String       '再振休日シフト
    '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------END

    Private Structure NenkanData
        <VBFixedStringAttribute(2)> Public Furikae_Date As String
        <VBFixedStringAttribute(2)> Public SaiFurikae_Date As String
        <VBFixedStringAttribute(10)> Public Furikae_Day As String
        <VBFixedStringAttribute(10)> Public SaiFurikae_Day As String
        Public Furikae_Check As Boolean
        Public SaiFurikae_Check As Boolean
        Public Furikae_Enabled As Boolean
        Public SaiFurikae_Enabled As Boolean
        Public CheckFurikae_Flag As Boolean '2006/11/30
        Public FunouFurikae_Flag As Boolean '2006/11/30
        Public CheckSaiFurikae_Flag As Boolean '2006/11/30
    End Structure
    Private NENKAN_SCHINFO(12) As NenkanData
    Private SYOKI_NENKAN_SCHINFO(12) As NenkanData '2006/11/30

    Private Structure TokubetuData
        <VBFixedStringAttribute(2)> Public Seikyu_Tuki As String
        Public SyoriFurikae_Flag As Boolean
        Public CheckFurikae_Flag As Boolean '2006/11/30
        Public FunouFurikae_Flag As Boolean '2006/11/30
        <VBFixedStringAttribute(2)> Public Furikae_Tuki As String
        <VBFixedStringAttribute(2)> Public Furikae_Date As String
        Public SyoriSaiFurikae_Flag As Boolean
        Public CheckSaiFurikae_Flag As Boolean '2006/11/30
        <VBFixedStringAttribute(2)> Public SaiFurikae_Tuki As String
        <VBFixedStringAttribute(2)> Public SaiFurikae_Date As String
        Public SiyouGakunenALL_Check As Boolean
        Public SiyouGakunen1_Check As Boolean
        Public SiyouGakunen2_Check As Boolean
        Public SiyouGakunen3_Check As Boolean
        Public SiyouGakunen4_Check As Boolean
        Public SiyouGakunen5_Check As Boolean
        Public SiyouGakunen6_Check As Boolean
        Public SiyouGakunen7_Check As Boolean
        Public SiyouGakunen8_Check As Boolean
        Public SiyouGakunen9_Check As Boolean
    End Structure
    Private TOKUBETU_SCHINFO(6) As TokubetuData
    Private SYOKI_TOKUBETU_SCHINFO(6) As TokubetuData

    Private Structure ZuijiData
        <VBFixedStringAttribute(2)> Public Nyusyutu_Kbn As String
        <VBFixedStringAttribute(2)> Public Furikae_Tuki As String
        <VBFixedStringAttribute(2)> Public Furikae_Date As String
        Public Syori_Flag As Boolean
        Public SiyouGakunenALL_Check As Boolean
        Public SiyouGakunen1_Check As Boolean
        Public SiyouGakunen2_Check As Boolean
        Public SiyouGakunen3_Check As Boolean
        Public SiyouGakunen4_Check As Boolean
        Public SiyouGakunen5_Check As Boolean
        Public SiyouGakunen6_Check As Boolean
        Public SiyouGakunen7_Check As Boolean
        Public SiyouGakunen8_Check As Boolean
        Public SiyouGakunen9_Check As Boolean
    End Structure
    Private ZUIJI_SCHINFO(6) As ZuijiData
    Private SYOKI_ZUIJI_SCHINFO(6) As ZuijiData

    Private Structure GakData
        <VBFixedStringAttribute(7)> Public GAKKOU_CODE As String
        <VBFixedStringAttribute(50)> Public GAKKOU_NNAME As String
        Public SIYOU_GAKUNEN As Integer
        <VBFixedStringAttribute(2)> Public FURI_DATE As String
        <VBFixedStringAttribute(2)> Public SFURI_DATE As String
        <VBFixedStringAttribute(1)> Public BAITAI_CODE As String
        <VBFixedStringAttribute(10)> Public ITAKU_CODE As String
        <VBFixedStringAttribute(4)> Public TKIN_CODE As String
        <VBFixedStringAttribute(3)> Public TSIT_CODE As String
        <VBFixedStringAttribute(1)> Public SFURI_SYUBETU As String
        <VBFixedStringAttribute(6)> Public KAISI_DATE As String
        <VBFixedStringAttribute(6)> Public SYURYOU_DATE As String
        <VBFixedStringAttribute(1)> Public TESUUTYO_KBN As String
        <VBFixedStringAttribute(1)> Public TESUUTYO_KIJITSU As String
        Public TESUUTYO_NO As Integer
        <VBFixedStringAttribute(1)> Public TESUU_KYU_CODE As String
        <VBFixedStringAttribute(6)> Public TAISYOU_START_NENDO As String
        <VBFixedStringAttribute(6)> Public TAISYOU_END_NENDO As String
    End Structure
    Private GAKKOU_INFO As GakData

    Private Str_SyoriDate(1) As String

    '処理状況(0:年間1:特別2:随時)
    '0:スケジュール未作成
    '1:スケジュール作成成功
    '2:スケジュール作成失敗
    Private Int_Syori_Flag(2) As Integer

    Private Int_Zuiji_Flag As Integer
    Private Int_Tokubetu_Flag As Integer


    Private Str_FURI_DATE As String
    Private Str_SFURI_DATE As String

    Private strFURI_DT As String '学校マスタ２の振替日
    Private strSFURI_DT As String '学校マスタ２の再振替日

    '2006/10/24
    Private strENTRI_FLG As String = "0"
    Private strCHECK_FLG As String = "0"
    Private strDATA_FLG As String = "0"
    Private strFUNOU_FLG As String = "0"
    Private strHENKAN_FLG As String = "0"
    Private strSAIFURI_FLG As String = "0"
    Private strKESSAI_FLG As String = "0"
    Private strTYUUDAN_FLG As String = "0"
    Private strENTRI_FLG_SAI As String = "0"
    Private strCHECK_FLG_SAI As String = "0"
    Private strDATA_FLG_SAI As String = "0"
    Private strFUNOU_FLG_SAI As String = "0"
    Private strSAIFURI_FLG_SAI As String = "0"
    Private strKESSAI_FLG_SAI As String = "0"
    Private strTYUUDAN_FLG_SAI As String = "0"

    Private strSAIFURI_DEF As String = "00000000" '通常スケジュールの再振日

    Private lngSYORI_KEN As Long = 0
    Private dblSYORI_KIN As Double = 0
    Private lngFURI_KEN As Long = 0
    Private dblFURI_KIN As Double = 0
    Private lngFUNOU_KEN As Long = 0
    Private dblFUNOU_KIN As Double = 0

    '企業自振スケジュール連携用　2006/12/01
    Private strSYOFURI_NENKAN(12) As String
    Private strSAIFURI_NENKAN(12) As String
    Private strSYOFURI_TOKUBETU(6) As String
    Private strSAIFURI_TOKUBETU(6) As String
    Private strFURI_ZUIJI(6) As String
    Private strFURIKBN_ZUIJI(6) As String
    Private strSYOFURI_NENKAN_AFTER(12) As String '更新後のスケジュール
    Private strSAIFURI_NENKAN_AFTER(12) As String '更新後のスケジュール
    Private strSYOFURI_TOKUBETU_AFTER(6) As String '更新後のスケジュール
    Private strSAIFURI_TOKUBETU_AFTER(6) As String '更新後のスケジュール
    Private strFURI_ZUIJI_AFTER(6) As String '更新後のスケジュール
    Private strFURIKBN_ZUIJI_AFTER(6) As String '更新後のスケジュール

    Private intPUSH_BTN As Integer '0:作成　1:参照 2:更新 3:取消
#End Region

    '2010.02.27 変数整理のため新規作成 ↓************
    Private strGakkouCode As String

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST060", "年間スケジュール作成画面")
    Private Const msgTitle As String = "年間スケジュール作成画面(KFGMAST060)"
    Private MainDB As MyOracle

#Region " Form_Load "
    Private Sub KFGMAST060_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            'ログ用
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            'テキストファイルからコンボボックス設定
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分１) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmb入出区分１)")
                MessageBox.Show("入出区分コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分２) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmb入出区分２)")
                MessageBox.Show("入出区分コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分３) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmb入出区分３)")
                MessageBox.Show("入出区分コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分４) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmb入出区分４)")
                MessageBox.Show("入出区分コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分５) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmb入出区分５)")
                MessageBox.Show("入出区分コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分６) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmb入出区分６)")
                MessageBox.Show("入出区分コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '初期画面表示
            Call PSUB_FORMAT_ALL()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region " Button Click "
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click


        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")

            Cursor.Current = Cursors.WaitCursor()
            intPUSH_BTN = 0

            strGakkouCode = Trim(txtGAKKOU_CODE.Text)

            '2010/10/21 月が0または12よりも大きく設定された場合はエラー ここから
            For Each txt特別月 As Control In TabPage2.Controls
                If Mid(txt特別月.Name, 1, 8) = "txt特別請求月" OrElse Mid(txt特別月.Name, 1, 8) = "txt特別振替月" _
                    OrElse Mid(txt特別月.Name, 1, 9) = "txt特別再振替月" Then
                    If txt特別月.Text <> "" Then
                        If CInt(txt特別月.Text) > 12 OrElse CInt(txt特別月.Text) = 0 Then
                            MessageBox.Show("月は１〜１２を設定してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt特別月.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next

            For Each txt随時振替月 As Control In TabPage3.Controls
                If Mid(txt随時振替月.Name, 1, 8) = "txt随時振替月" Then
                    If txt随時振替月.Text <> "" Then
                        If CInt(txt随時振替月.Text) > 12 OrElse CInt(txt随時振替月.Text) = 0 Then
                            MessageBox.Show("月は１〜１２を設定してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt随時振替月.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next
            '2010/10/21 月が0または12よりも大きく設定された場合はエラー ここまで

            Call sb_HENSU_CLEAR()

            '2006/12/08　「作成する」というフラグを立てる
            Call PSUB_Kousin_Check()

            If PFUNC_SCH_INSERT_ALL() = False Then
                Return
            End If

            '入力項目制御
            txt対象年度.Enabled = False
            txtGAKKOU_CODE.Enabled = False

            If Int_Syori_Flag(0) = 2 Then '追加 2005/06/15
                '入力ボタン制御
                Call PSUB_BUTTON_Enable(0)
            Else
                '入力ボタン制御
                Call PSUB_BUTTON_Enable(1)
            End If

            Call sb_SANSYOU_SET()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")
        End Try
        

    End Sub
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")
            MainDB = New MyOracle

            Cursor.Current = Cursors.WaitCursor()
            intPUSH_BTN = 1

            '参照ボタン
            strGakkouCode = Trim(txtGAKKOU_CODE.Text)

            If PFUNC_SCH_GET_ALL() = False Then
                Exit Sub
            End If

            '2006/10/11　最高学年以上の学年の使用不可
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()

            '入力ボタン制御
            Call PSUB_BUTTON_Enable(1)

            '企業連携向け 2006/12/04
            Call sb_SANSYOU_SET()

        Catch ex As Exception

            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)

        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
            MainDB.Close()
        End Try

    End Sub
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUPDATE.Click


        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            MainDB = New MyOracle

            Cursor.Current = Cursors.WaitCursor()
            intPUSH_BTN = 2

            strGakkouCode = Trim(txtGAKKOU_CODE.Text)

            '2010/10/21 月が0または12よりも大きく設定された場合はエラー ここから
            For Each txt特別月 As Control In TabPage2.Controls
                If Mid(txt特別月.Name, 1, 8) = "txt特別請求月" OrElse Mid(txt特別月.Name, 1, 8) = "txt特別振替月" _
                    OrElse Mid(txt特別月.Name, 1, 9) = "txt特別再振替月" Then
                    If txt特別月.Text <> "" Then
                        If CInt(txt特別月.Text) > 12 OrElse CInt(txt特別月.Text) = 0 Then
                            MessageBox.Show("月は１〜１２を設定してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt特別月.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next

            '2011/06/16 標準版修正 特別振替日相関チェック追加 ------------------START
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If
            '2011/06/16 標準版修正 特別振替日相関チェック追加 ------------------END

            For Each txt随時振替月 As Control In TabPage3.Controls
                If Mid(txt随時振替月.Name, 1, 8) = "txt随時振替月" Then
                    If txt随時振替月.Text <> "" Then
                        If CInt(txt随時振替月.Text) > 12 OrElse CInt(txt随時振替月.Text) = 0 Then
                            MessageBox.Show("月は１〜１２を設定してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt随時振替月.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next
            '2010/10/21 月が0または12よりも大きく設定された場合はエラー ここまで

            Call sb_HENSU_CLEAR()

            If PFUNC_SCH_DELETE_INSERT_ALL() = False Then

                MainDB.Rollback()
                Return

            End If

            MainDB.Commit()

            '入力項目制御
            txt対象年度.Enabled = True
            txtGAKKOU_CODE.Enabled = True
            '2006/10/11　最高学年以上の学年の使用不可
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()

            '入力ボタン制御
            Call PSUB_BUTTON_Enable(2)

        Catch ex As Exception

            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)例外エラー", "失敗", ex.Message)
            MainDB.Rollback()

        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
            MainDB.Close()

        End Try

    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        intPUSH_BTN = 3

        '取消ボタン

        '画面初期状態
        Call PSUB_FORMAT_ALL()

        '追加 2006/12/27
        ReDim SYOKI_NENKAN_SCHINFO(12)
        ReDim SYOKI_TOKUBETU_SCHINFO(6)
        ReDim SYOKI_ZUIJI_SCHINFO(6)
        ReDim NENKAN_SCHINFO(12)
        ReDim TOKUBETU_SCHINFO(6)
        ReDim ZUIJI_SCHINFO(6)

        txt対象年度.Focus()

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub
#End Region

#Region " GotFocus "
    '学校情報
    Private Sub txt対象年度_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt対象年度.GotFocus
        Me.txt対象年度.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt対象年度)

    End Sub
    Private Sub txtGAKKOU_CODE_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.GotFocus
        Me.txtGAKKOU_CODE.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txtGAKKOU_CODE)

    End Sub
    '年間スケジュール
    Private Sub txt４月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt４月振替日.GotFocus
        Me.txt４月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt４月振替日)

    End Sub
    Private Sub txt５月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt５月振替日.GotFocus
        Me.txt５月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt５月振替日)

    End Sub
    Private Sub txt６月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt６月振替日.GotFocus
        Me.txt６月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt６月振替日)

    End Sub
    Private Sub txt７月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt７月振替日.GotFocus
        Me.txt７月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt７月振替日)

    End Sub
    Private Sub txt８月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt８月振替日.GotFocus
        Me.txt８月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt８月振替日)

    End Sub
    Private Sub txt９月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt９月振替日.GotFocus
        Me.txt９月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt９月振替日)

    End Sub
    Private Sub txt１０月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１０月振替日.GotFocus
        Me.txt１０月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt１０月振替日)

    End Sub
    Private Sub txt１１月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１１月振替日.GotFocus
        Me.txt１１月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt１１月振替日)

    End Sub
    Private Sub txt１２月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１２月振替日.GotFocus
        Me.txt１２月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt１２月振替日)

    End Sub
    Private Sub txt１月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１月振替日.GotFocus
        Me.txt１月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt１月振替日)

    End Sub
    Private Sub txt２月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt２月振替日.GotFocus
        Me.txt２月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt２月振替日)

    End Sub
    Private Sub txt３月振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt３月振替日.GotFocus
        Me.txt３月振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt３月振替日)

    End Sub
    Private Sub txt４月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt４月再振替日.GotFocus
        Me.txt４月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt４月再振替日)

    End Sub
    Private Sub txt５月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt５月再振替日.GotFocus
        Me.txt５月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt５月再振替日)

    End Sub
    Private Sub txt６月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt６月再振替日.GotFocus
        Me.txt６月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt６月再振替日)

    End Sub
    Private Sub txt７月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt７月再振替日.GotFocus
        Me.txt７月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt７月再振替日)

    End Sub
    Private Sub txt８月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt８月再振替日.GotFocus
        Me.txt８月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt８月再振替日)

    End Sub
    Private Sub txt９月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt９月再振替日.GotFocus
        Me.txt９月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt９月再振替日)

    End Sub
    Private Sub txt１０月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１０月再振替日.GotFocus
        Me.txt１０月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt１０月再振替日)

    End Sub
    Private Sub txt１１月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１１月再振替日.GotFocus
        Me.txt１１月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt１１月再振替日)

    End Sub
    Private Sub txt１２月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１２月再振替日.GotFocus
        Me.txt１２月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt１２月再振替日)

    End Sub
    Private Sub txt１月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１月再振替日.GotFocus
        Me.txt１月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt１月再振替日)

    End Sub
    Private Sub txt２月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt２月再振替日.GotFocus
        Me.txt２月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt２月再振替日)

    End Sub
    Private Sub txt３月再振替日_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt３月再振替日.GotFocus
        Me.txt３月再振替日.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt３月再振替日)

    End Sub
    '特別スケジュール
    Private Sub txt特別請求月１_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月１.GotFocus
        Me.txt特別請求月１.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別請求月１)

    End Sub
    Private Sub txt特別請求月２_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月２.GotFocus
        Me.txt特別請求月２.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別請求月２)

    End Sub
    Private Sub txt特別請求月３_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月３.GotFocus
        Me.txt特別請求月３.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別請求月３)

    End Sub
    Private Sub txt特別請求月４_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月４.GotFocus
        Me.txt特別請求月４.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別請求月４)

    End Sub
    Private Sub txt特別請求月５_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月５.GotFocus
        Me.txt特別請求月５.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別請求月５)

    End Sub
    Private Sub txt特別請求月６_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月６.GotFocus
        Me.txt特別請求月６.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別請求月６)

    End Sub
    Private Sub txt特別振替月１_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月１.GotFocus
        Me.txt特別振替月１.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替月１)

    End Sub
    Private Sub txt特別振替月２_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月２.GotFocus
        Me.txt特別振替月２.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替月２)

    End Sub
    Private Sub txt特別振替月３_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月３.GotFocus
        Me.txt特別振替月３.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替月３)

    End Sub
    Private Sub txt特別振替月４_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月４.GotFocus
        Me.txt特別振替月４.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替月４)

    End Sub
    Private Sub txt特別振替月５_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月５.GotFocus
        Me.txt特別振替月５.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替月５)

    End Sub
    Private Sub txt特別振替月６_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月６.GotFocus
        Me.txt特別振替月６.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替月６)

    End Sub
    Private Sub txt特別振替日１_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日１.GotFocus
        Me.txt特別振替日１.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替日１)

    End Sub
    Private Sub txt特別振替日２_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日２.GotFocus
        Me.txt特別振替日２.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替日２)

    End Sub
    Private Sub txt特別振替日３_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日３.GotFocus
        Me.txt特別振替日３.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替日３)

    End Sub
    Private Sub txt特別振替日４_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日４.GotFocus
        Me.txt特別振替日４.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替日４)

    End Sub
    Private Sub txt特別振替日５_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日５.GotFocus
        Me.txt特別振替日５.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替日５)

    End Sub
    Private Sub txt特別振替日６_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日６.GotFocus
        Me.txt特別振替日６.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別振替日６)

    End Sub
    Private Sub txt特別再振替月１_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月１.GotFocus
        Me.txt特別再振替月１.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替月１)

    End Sub
    Private Sub txt特別再振替月２_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月２.GotFocus
        Me.txt特別再振替月２.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替月２)

    End Sub
    Private Sub txt特別再振替月３_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月３.GotFocus
        Me.txt特別再振替月３.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替月３)

    End Sub
    Private Sub txt特別再振替月４_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月４.GotFocus
        Me.txt特別再振替月４.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替月４)

    End Sub
    Private Sub txt特別再振替月５_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月５.GotFocus
        Me.txt特別再振替月５.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替月５)

    End Sub
    Private Sub txt特別再振替月６_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月６.GotFocus
        Me.txt特別再振替月６.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替月６)

    End Sub
    Private Sub txt特別再振替日１_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日１.GotFocus
        Me.txt特別再振替日１.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替日１)

    End Sub
    Private Sub txt特別再振替日２_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日２.GotFocus
        Me.txt特別再振替日２.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替日２)

    End Sub
    Private Sub txt特別再振替日３_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日３.GotFocus
        Me.txt特別再振替日３.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替日３)

    End Sub
    Private Sub txt特別再振替日４_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日４.GotFocus
        Me.txt特別再振替日４.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替日４)

    End Sub
    Private Sub txt特別再振替日５_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日５.GotFocus
        Me.txt特別再振替日５.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替日５)

    End Sub
    Private Sub txt特別再振替日６_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日６.GotFocus
        Me.txt特別再振替日６.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt特別再振替日６)

    End Sub
    '随時スケジュール
    Private Sub txt随時振替月１_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月１.GotFocus
        Me.txt随時振替月１.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替月１)

    End Sub
    Private Sub txt随時振替月２_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月２.GotFocus
        Me.txt随時振替月２.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替月２)

    End Sub
    Private Sub txt随時振替月３_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月３.GotFocus
        Me.txt随時振替月３.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替月３)

    End Sub
    Private Sub txt随時振替月４_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月４.GotFocus
        Me.txt随時振替月４.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替月４)

    End Sub
    Private Sub txt随時振替月５_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月５.GotFocus
        Me.txt随時振替月５.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替月５)

    End Sub
    Private Sub txt随時振替月６_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月６.GotFocus
        Me.txt随時振替月６.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替月６)

    End Sub
    Private Sub txt随時振替日１_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日１.GotFocus
        Me.txt随時振替日１.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替日１)

    End Sub
    Private Sub txt随時振替日２_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日２.GotFocus
        Me.txt随時振替日２.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替日２)

    End Sub
    Private Sub txt随時振替日３_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日３.GotFocus
        Me.txt随時振替日３.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替日３)

    End Sub
    Private Sub txt随時振替日４_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日４.GotFocus
        Me.txt随時振替日４.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替日４)

    End Sub
    Private Sub txt随時振替日５_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日５.GotFocus
        Me.txt随時振替日５.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替日５)

    End Sub
    Private Sub txt随時振替日６_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日６.GotFocus
        Me.txt随時振替日６.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt随時振替日６)

    End Sub
#End Region

#Region " LostFocus "
    '基本
    Private Sub txt対象年度_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt対象年度.LostFocus
        Me.txt対象年度.BackColor = System.Drawing.Color.White
        '休日情報の表示
        If PFUNC_KYUJITULIST_SET() = False Then
            Exit Sub
        End If

        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txt対象年度.Text) <> "" Then
            '対象年度も入力されている場合、スケジュール存在チェックをかけ
            'スケジュールが存在する場合は参照ボタンにフォーカス移動
            Call PSUB_SANSYOU_FOCUS()
        End If

    End Sub
    Private Sub txtGAKKOU_CODE_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.LostFocus
        Me.txtGAKKOU_CODE.BackColor = System.Drawing.Color.White
        '学校名の取得
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txtGAKKOU_CODE, 10)

            '学校名の取得(学校情報も変数に格納される)
            If PFUNC_GAKINFO_GET() = False Then
                Exit Sub
            End If

            '年間スケジュール画面初期化
            Call PSUB_NENKAN_FORMAT()

            '特別スケジュール画面初期化
            Call PSUB_TOKUBETU_FORMAT()

            '随時スケジュール画面初期化
            Call PSUB_ZUIJI_FORMAT()

            '再振替日のプロテクトTrue
            Call PSUB_SAIFURI_PROTECT(True)

            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "0", "3"
                    Call PSUB_SAIFURI_PROTECT(False)
            End Select

            '2006/10/12　最高学年以上の学年のチェックボックス使用不可
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()

            If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txt対象年度.Text) <> "" Then
                '対象年度も入力されている場合、スケジュール存在チェックをかけ
                'スケジュールが存在する場合は参照ボタンにフォーカス移動
                Call PSUB_SANSYOU_FOCUS()
            End If
        Else
            '2006/10/12　学校コードが空白のとき、学校名ラベルを空白にする
            lab学校名.Text = ""
        End If

    End Sub
    '年間
    Private Sub txt特別請求月１_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月１.LostFocus
        Me.txt特別請求月１.BackColor = System.Drawing.Color.White
        If Trim(txt特別請求月１.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別請求月１, 2)
        End If

    End Sub
    Private Sub txt特別請求月２_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月２.LostFocus
        Me.txt特別請求月２.BackColor = System.Drawing.Color.White
        If Trim(txt特別請求月２.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別請求月２, 2)
        End If

    End Sub
    Private Sub txt特別請求月３_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月３.LostFocus
        Me.txt特別請求月３.BackColor = System.Drawing.Color.White
        If Trim(txt特別請求月３.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別請求月３, 2)
        End If

    End Sub
    Private Sub txt特別請求月４_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月４.LostFocus
        Me.txt特別請求月４.BackColor = System.Drawing.Color.White
        If Trim(txt特別請求月４.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別請求月４, 2)
        End If

    End Sub
    Private Sub txt特別請求月５_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月５.LostFocus
        Me.txt特別請求月５.BackColor = System.Drawing.Color.White
        If Trim(txt特別請求月５.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別請求月５, 2)
        End If

    End Sub
    Private Sub txt特別請求月６_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別請求月６.LostFocus
        Me.txt特別請求月６.BackColor = System.Drawing.Color.White
        If Trim(txt特別請求月６.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別請求月６, 2)
        End If

    End Sub
    Private Sub txt４月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt４月振替日.LostFocus
        Me.txt４月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt４月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt４月振替日, 2)
        End If

    End Sub
    Private Sub txt５月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt５月振替日.LostFocus
        Me.txt５月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt５月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt５月振替日, 2)
        End If

    End Sub
    Private Sub txt６月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt６月振替日.LostFocus
        Me.txt６月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt６月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt６月振替日, 2)
        End If

    End Sub
    Private Sub txt７月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt７月振替日.LostFocus
        Me.txt７月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt７月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt７月振替日, 2)
        End If

    End Sub
    Private Sub txt８月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt８月振替日.LostFocus
        Me.txt８月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt８月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt８月振替日, 2)
        End If

    End Sub
    Private Sub txt９月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt９月振替日.LostFocus
        Me.txt９月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt９月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt９月振替日, 2)
        End If

    End Sub
    Private Sub txt１０月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１０月振替日.LostFocus
        Me.txt１０月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt１０月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt１０月振替日, 2)
        End If

    End Sub
    Private Sub txt１１月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１１月振替日.LostFocus
        Me.txt１１月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt１１月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt１１月振替日, 2)
        End If

    End Sub
    Private Sub txt１２月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１２月振替日.LostFocus
        Me.txt１２月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt１２月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt１２月振替日, 2)
        End If

    End Sub
    Private Sub txt１月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１月振替日.LostFocus
        Me.txt１月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt１月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt１月振替日, 2)
        End If

    End Sub
    Private Sub txt２月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt２月振替日.LostFocus
        Me.txt２月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt２月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt２月振替日, 2)
        End If

    End Sub
    Private Sub txt３月振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt３月振替日.LostFocus
        Me.txt３月振替日.BackColor = System.Drawing.Color.White
        If Trim(txt３月振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt３月振替日, 2)
        End If

    End Sub
    Private Sub txt４月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt４月再振替日.LostFocus
        Me.txt４月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt４月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt４月再振替日, 2)
        End If

    End Sub
    Private Sub txt５月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt５月再振替日.LostFocus
        Me.txt５月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt５月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt５月再振替日, 2)
        End If

    End Sub
    Private Sub txt６月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt６月再振替日.LostFocus
        Me.txt６月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt６月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt６月再振替日, 2)
        End If

    End Sub
    Private Sub txt７月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt７月再振替日.LostFocus
        Me.txt７月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt７月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt７月再振替日, 2)
        End If

    End Sub
    Private Sub txt８月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt８月再振替日.LostFocus
        Me.txt８月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt８月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt８月再振替日, 2)
        End If

    End Sub
    Private Sub txt９月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt９月再振替日.LostFocus
        Me.txt９月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt９月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt９月再振替日, 2)
        End If

    End Sub
    Private Sub txt１０月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１０月再振替日.LostFocus
        Me.txt１０月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt１０月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt１０月再振替日, 2)
        End If

    End Sub
    Private Sub txt１１月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１１月再振替日.LostFocus
        Me.txt１１月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt１１月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt１１月再振替日, 2)
        End If

    End Sub
    Private Sub txt１２月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１２月再振替日.LostFocus
        Me.txt１２月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt１２月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt１２月再振替日, 2)
        End If

    End Sub
    Private Sub txt１月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt１月再振替日.LostFocus
        Me.txt１月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt１月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt１月再振替日, 2)
        End If

    End Sub
    Private Sub txt２月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt２月再振替日.LostFocus
        Me.txt２月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt２月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt２月再振替日, 2)
        End If

    End Sub
    Private Sub txt３月再振替日_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt３月再振替日.LostFocus
        Me.txt３月再振替日.BackColor = System.Drawing.Color.White
        If Trim(txt３月再振替日.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt３月再振替日, 2)
        End If

    End Sub
    '特別
    Private Sub txt特別振替月１_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月１.LostFocus
        Me.txt特別振替月１.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替月１.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替月１, 2)
        End If

    End Sub
    Private Sub txt特別振替月２_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月２.LostFocus
        Me.txt特別振替月２.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替月２.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替月２, 2)
        End If

    End Sub
    Private Sub txt特別振替月３_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月３.LostFocus
        Me.txt特別振替月３.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替月３.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替月３, 2)
        End If

    End Sub
    Private Sub txt特別振替月４_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月４.LostFocus
        Me.txt特別振替月４.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替月４.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替月４, 2)
        End If

    End Sub
    Private Sub txt特別振替月５_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月５.LostFocus
        Me.txt特別振替月５.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替月５.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替月５, 2)
        End If

    End Sub
    Private Sub txt特別振替月６_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替月６.LostFocus
        Me.txt特別振替月６.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替月６.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替月６, 2)
        End If

    End Sub
    Private Sub txt特別振替日１_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日１.LostFocus
        Me.txt特別振替日１.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替日１.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替日１, 2)
        End If

    End Sub
    Private Sub txt特別振替日２_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日２.LostFocus
        Me.txt特別振替日２.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替日２.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替日２, 2)
        End If

    End Sub
    Private Sub txt特別振替日３_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日３.LostFocus
        Me.txt特別振替日３.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替日３.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替日３, 2)
        End If

    End Sub
    Private Sub txt特別振替日４_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日４.LostFocus
        Me.txt特別振替日４.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替日４.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替日４, 2)
        End If

    End Sub
    Private Sub txt特別振替日５_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日５.LostFocus
        Me.txt特別振替日５.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替日５.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替日５, 2)
        End If

    End Sub
    Private Sub txt特別振替日６_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別振替日６.LostFocus
        Me.txt特別振替日６.BackColor = System.Drawing.Color.White
        If Trim(txt特別振替日６.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別振替日６, 2)
        End If

    End Sub
    Private Sub txt特別再振替月１_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月１.LostFocus
        Me.txt特別再振替月１.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替月１.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替月１, 2)
        End If

    End Sub
    Private Sub txt特別再振替月２_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月２.LostFocus
        Me.txt特別再振替月２.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替月２.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替月２, 2)
        End If

    End Sub
    Private Sub txt特別再振替月３_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月３.LostFocus
        Me.txt特別再振替月３.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替月３.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替月３, 2)
        End If

    End Sub
    Private Sub txt特別再振替月４_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月４.LostFocus
        Me.txt特別再振替月４.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替月４.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替月４, 2)
        End If

    End Sub
    Private Sub txt特別再振替月５_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月５.LostFocus
        Me.txt特別再振替月５.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替月５.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替月５, 2)
        End If

    End Sub
    Private Sub txt特別再振替月６_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替月６.LostFocus
        Me.txt特別再振替月６.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替月６.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替月６, 2)
        End If

    End Sub
    Private Sub txt特別再振替日１_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日１.LostFocus
        Me.txt特別再振替日１.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替日１.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替日１, 2)
        End If

    End Sub
    Private Sub txt特別再振替日２_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日２.LostFocus
        Me.txt特別再振替日２.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替日２.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替日２, 2)
        End If

    End Sub
    Private Sub txt特別再振替日３_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日３.LostFocus
        Me.txt特別再振替日３.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替日３.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替日３, 2)
        End If

    End Sub
    Private Sub txt特別再振替日４_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日４.LostFocus
        Me.txt特別再振替日４.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替日４.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替日４, 2)
        End If

    End Sub
    Private Sub txt特別再振替日５_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日５.LostFocus
        Me.txt特別再振替日５.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替日５.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替日５, 2)
        End If

    End Sub
    Private Sub txt特別再振替日６_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt特別再振替日６.LostFocus
        Me.txt特別再振替日６.BackColor = System.Drawing.Color.White
        If Trim(txt特別再振替日６.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt特別再振替日６, 2)
        End If

    End Sub
    '随時
    Private Sub txt随時振替月１_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月１.LostFocus
        Me.txt随時振替月１.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替月１.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替月１, 2)
        End If

    End Sub
    Private Sub txt随時振替月２_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月２.LostFocus
        Me.txt随時振替月２.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替月２.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替月２, 2)
        End If

    End Sub
    Private Sub txt随時振替月３_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月３.LostFocus
        Me.txt随時振替月３.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替月３.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替月３, 2)
        End If

    End Sub
    Private Sub txt随時振替月４_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月４.LostFocus
        Me.txt随時振替月４.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替月４.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替月４, 2)
        End If

    End Sub
    Private Sub txt随時振替月５_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月５.LostFocus
        Me.txt随時振替月５.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替月５.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替月５, 2)
        End If

    End Sub
    Private Sub txt随時振替月６_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替月６.LostFocus
        Me.txt随時振替月６.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替月６.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替月６, 2)
        End If

    End Sub
    Private Sub txt随時振替日１_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日１.LostFocus
        Me.txt随時振替日１.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替日１.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替日１, 2)
        End If

    End Sub
    Private Sub txt随時振替日２_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日２.LostFocus
        Me.txt随時振替日２.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替日２.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替日２, 2)
        End If

    End Sub
    Private Sub txt随時振替日３_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日３.LostFocus
        Me.txt随時振替日３.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替日３.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替日３, 2)
        End If

    End Sub
    Private Sub txt随時振替日４_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日４.LostFocus
        Me.txt随時振替日４.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替日４.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替日４, 2)
        End If

    End Sub
    Private Sub txt随時振替日５_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日５.LostFocus
        Me.txt随時振替日５.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替日５.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替日５, 2)
        End If

    End Sub
    Private Sub txt随時振替日６_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt随時振替日６.LostFocus
        Me.txt随時振替日６.BackColor = System.Drawing.Color.White
        If Trim(txt随時振替日６.Text) <> "" Then
            '０付加
            Call GFUNC_ZERO_ADD(txt随時振替日６, 2)
        End If

    End Sub
#End Region

#Region " KeyPress "
    '基本
    Private Sub txt対象年度_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt対象年度.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txtGAKKOU_CODE_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtGAKKOU_CODE.KeyPress
        '学校コードのKEY入力制御
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    '年間
    Private Sub txt４月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt４月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt５月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt５月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt６月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt６月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt７月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt７月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt８月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt８月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt９月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt９月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt１０月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt１０月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt１１月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt１１月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt１２月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt１２月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt１月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt１月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt２月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt２月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt３月振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt３月振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt４月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt４月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt５月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt５月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt６月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt６月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt７月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt７月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt８月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt８月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt９月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt９月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt１０月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt１０月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt１１月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt１１月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt１２月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt１２月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt１月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt１月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt２月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt２月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt３月再振替日_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt３月再振替日.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    '特別
    Private Sub txt特別請求月１_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別請求月１.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt特別請求月２_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別請求月２.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt特別請求月３_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別請求月３.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt特別請求月４_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別請求月４.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt特別請求月５_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別請求月５.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt特別請求月６_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別請求月６.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt特別振替月１_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替月１.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別振替月２_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替月２.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別振替月３_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替月３.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別振替月４_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替月４.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別振替月５_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替月５.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別振替月６_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替月６.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別振替日１_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替日１.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別振替日２_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替日２.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別振替日３_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替日３.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別振替日４_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替日４.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別振替日５_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替日５.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別振替日６_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別振替日６.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替月１_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替月１.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替月２_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替月２.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替月３_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替月３.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替月４_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替月４.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替月５_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替月５.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替月６_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替月６.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替日１_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替日１.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替日２_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替日２.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替日３_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替日３.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替日４_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替日４.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替日５_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替日５.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt特別再振替日６_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt特別再振替日６.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    '随時
    Private Sub txt随時振替月１_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替月１.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt随時振替月２_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替月２.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt随時振替月３_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替月３.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt随時振替月４_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替月４.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt随時振替月５_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替月５.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt随時振替月６_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替月６.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt随時振替日１_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替日１.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt随時振替日２_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替日２.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt随時振替日３_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替日３.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt随時振替日４_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替日４.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt随時振替日５_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替日５.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt随時振替日６_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt随時振替日６.KeyPress
        '入力数値チェック
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
#End Region

#Region " KeyUp "
    '学校情報
    Private Sub txt対象年度_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt対象年度.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt対象年度)

    End Sub
    Private Sub txtGAKKOU_CODE_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtGAKKOU_CODE.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txtGAKKOU_CODE)

    End Sub
    '年間スケジュール
    Private Sub txt４月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt４月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt４月振替日)

    End Sub
    Private Sub txt５月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt５月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt５月振替日)

    End Sub
    Private Sub txt６月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt６月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt６月振替日)

    End Sub
    Private Sub txt７月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt７月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt７月振替日)

    End Sub
    Private Sub txt８月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt８月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt８月振替日)

    End Sub
    Private Sub txt９月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt９月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt９月振替日)

    End Sub
    Private Sub txt１０月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt１０月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt１０月振替日)

    End Sub
    Private Sub txt１１月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt１１月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt１１月振替日)

    End Sub
    Private Sub txt１２月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt１２月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt１２月振替日)

    End Sub
    Private Sub txt１月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt１月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt１月振替日)

    End Sub
    Private Sub txt２月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt２月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt２月振替日)

    End Sub
    Private Sub txt３月振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt３月振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt３月振替日)

    End Sub
    Private Sub txt４月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt４月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt４月再振替日)

    End Sub
    Private Sub txt５月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt５月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt５月再振替日)

    End Sub
    Private Sub txt６月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt６月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt６月再振替日)

    End Sub
    Private Sub txt７月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt７月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt７月再振替日)

    End Sub
    Private Sub txt８月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt８月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt８月再振替日)

    End Sub
    Private Sub txt９月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt９月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt９月再振替日)

    End Sub
    Private Sub txt１０月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt１０月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt１０月再振替日)

    End Sub
    Private Sub txt１１月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt１１月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt１１月再振替日)

    End Sub
    Private Sub txt１２月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt１２月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt１２月再振替日)

    End Sub
    Private Sub txt１月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt１月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt１月再振替日)

    End Sub
    Private Sub txt２月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt２月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt２月再振替日)

    End Sub
    Private Sub txt３月再振替日_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt３月再振替日.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt３月再振替日)

    End Sub
    '特別スケジュール
    Private Sub txt特別請求月１_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別請求月１.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別請求月１)

    End Sub
    Private Sub txt特別請求月２_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別請求月２.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別請求月２)

    End Sub
    Private Sub txt特別請求月３_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別請求月３.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別請求月３)

    End Sub
    Private Sub txt特別請求月４_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別請求月４.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別請求月４)

    End Sub
    Private Sub txt特別請求月５_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別請求月５.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別請求月５)

    End Sub
    Private Sub txt特別請求月６_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別請求月６.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別請求月６)

    End Sub
    Private Sub txt特別振替月１_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替月１.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替月１)

    End Sub
    Private Sub txt特別振替月２_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替月２.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替月２)

    End Sub
    Private Sub txt特別振替月３_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替月３.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替月３)

    End Sub
    Private Sub txt特別振替月４_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替月４.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替月４)

    End Sub
    Private Sub txt特別振替月５_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替月５.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替月５)

    End Sub
    Private Sub txt特別振替月６_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替月６.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替月６)

    End Sub
    Private Sub txt特別振替日１_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替日１.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替日１)

    End Sub
    Private Sub txt特別振替日２_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替日２.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替日２)

    End Sub
    Private Sub txt特別振替日３_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替日３.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替日３)

    End Sub
    Private Sub txt特別振替日４_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替日４.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替日４)

    End Sub
    Private Sub txt特別振替日５_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替日５.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替日５)

    End Sub
    Private Sub txt特別振替日６_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別振替日６.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別振替日６)

    End Sub
    Private Sub txt特別再振替月１_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替月１.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替月１)

    End Sub
    Private Sub txt特別再振替月２_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替月２.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替月２)

    End Sub
    Private Sub txt特別再振替月３_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替月３.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替月３)

    End Sub
    Private Sub txt特別再振替月４_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替月４.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替月４)

    End Sub
    Private Sub txt特別再振替月５_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替月５.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替月５)

    End Sub
    Private Sub txt特別再振替月６_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替月６.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替月６)

    End Sub
    Private Sub txt特別再振替日１_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替日１.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替日１)

    End Sub
    Private Sub txt特別再振替日２_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替日２.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替日２)

    End Sub
    Private Sub txt特別再振替日３_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替日３.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替日３)

    End Sub
    Private Sub txt特別再振替日４_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替日４.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替日４)

    End Sub
    Private Sub txt特別再振替日５_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替日５.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替日５)

    End Sub
    Private Sub txt特別再振替日６_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt特別再振替日６.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt特別再振替日６)

    End Sub
    '随時スケジュール
    Private Sub txt随時振替月１_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替月１.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替月１)

    End Sub
    Private Sub txt随時振替月２_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替月２.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替月２)

    End Sub
    Private Sub txt随時振替月３_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替月３.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替月３)

    End Sub
    Private Sub txt随時振替月４_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替月４.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替月４)

    End Sub
    Private Sub txt随時振替月５_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替月５.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替月５)

    End Sub
    Private Sub txt随時振替月６_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替月６.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替月６)

    End Sub
    Private Sub txt随時振替日１_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替日１.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替日１)

    End Sub
    Private Sub txt随時振替日２_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替日２.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替日２)

    End Sub
    Private Sub txt随時振替日３_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替日３.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替日３)

    End Sub
    Private Sub txt随時振替日４_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替日４.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替日４)

    End Sub
    Private Sub txt随時振替日５_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替日５.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替日５)

    End Sub
    Private Sub txt随時振替日６_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt随時振替日６.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt随時振替日６)

    End Sub
#End Region

#Region " CheckedChanged(CheckBox) "
    '特別スケジュール
    Private Sub chk１_全学年_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk１_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk１_全学年, _
                                           chk１_１学年, _
                                           chk１_２学年, _
                                           chk１_３学年, _
                                           chk１_４学年, _
                                           chk１_５学年, _
                                           chk１_６学年, _
                                           chk１_７学年, _
                                           chk１_８学年, _
                                           chk１_９学年)

    End Sub
    Private Sub chk２_全学年_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk２_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk２_全学年, _
                                           chk２_１学年, _
                                           chk２_２学年, _
                                           chk２_３学年, _
                                           chk２_４学年, _
                                           chk２_５学年, _
                                           chk２_６学年, _
                                           chk２_７学年, _
                                           chk２_８学年, _
                                           chk２_９学年)

    End Sub
    Private Sub chk３_全学年_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk３_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk３_全学年, _
                                           chk３_１学年, _
                                           chk３_２学年, _
                                           chk３_３学年, _
                                           chk３_４学年, _
                                           chk３_５学年, _
                                           chk３_６学年, _
                                           chk３_７学年, _
                                           chk３_８学年, _
                                           chk３_９学年)

    End Sub
    Private Sub chk４_全学年_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk４_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk４_全学年, _
                                           chk４_１学年, _
                                           chk４_２学年, _
                                           chk４_３学年, _
                                           chk４_４学年, _
                                           chk４_５学年, _
                                           chk４_６学年, _
                                           chk４_７学年, _
                                           chk４_８学年, _
                                           chk４_９学年)

    End Sub
    Private Sub chk５_全学年_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk５_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk５_全学年, _
                                           chk５_１学年, _
                                           chk５_２学年, _
                                           chk５_３学年, _
                                           chk５_４学年, _
                                           chk５_５学年, _
                                           chk５_６学年, _
                                           chk５_７学年, _
                                           chk５_８学年, _
                                           chk５_９学年)

    End Sub
    Private Sub chk６_全学年_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk６_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk６_全学年, _
                                           chk６_１学年, _
                                           chk６_２学年, _
                                           chk６_３学年, _
                                           chk６_４学年, _
                                           chk６_５学年, _
                                           chk６_６学年, _
                                           chk６_７学年, _
                                           chk６_８学年, _
                                           chk６_９学年)

    End Sub
    '随時スケジュール
    Private Sub chk随時１_全学年_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk随時１_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk随時１_全学年, _
                                           chk随時１_１学年, _
                                           chk随時１_２学年, _
                                           chk随時１_３学年, _
                                           chk随時１_４学年, _
                                           chk随時１_５学年, _
                                           chk随時１_６学年, _
                                           chk随時１_７学年, _
                                           chk随時１_８学年, _
                                           chk随時１_９学年)

    End Sub
    Private Sub chk随時２_全学年_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk随時２_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk随時２_全学年, _
                                           chk随時２_１学年, _
                                           chk随時２_２学年, _
                                           chk随時２_３学年, _
                                           chk随時２_４学年, _
                                           chk随時２_５学年, _
                                           chk随時２_６学年, _
                                           chk随時２_７学年, _
                                           chk随時２_８学年, _
                                           chk随時２_９学年)

    End Sub
    Private Sub chk随時３_全学年_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk随時３_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk随時３_全学年, _
                                           chk随時３_１学年, _
                                           chk随時３_２学年, _
                                           chk随時３_３学年, _
                                           chk随時３_４学年, _
                                           chk随時３_５学年, _
                                           chk随時３_６学年, _
                                           chk随時３_７学年, _
                                           chk随時３_８学年, _
                                           chk随時３_９学年)

    End Sub
    Private Sub chk随時４_全学年_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk随時４_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk随時４_全学年, _
                                           chk随時４_１学年, _
                                           chk随時４_２学年, _
                                           chk随時４_３学年, _
                                           chk随時４_４学年, _
                                           chk随時４_５学年, _
                                           chk随時４_６学年, _
                                           chk随時４_７学年, _
                                           chk随時４_８学年, _
                                           chk随時４_９学年)

    End Sub
    Private Sub chk随時５_全学年_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk随時５_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk随時５_全学年, _
                                           chk随時５_１学年, _
                                           chk随時５_２学年, _
                                           chk随時５_３学年, _
                                           chk随時５_４学年, _
                                           chk随時５_５学年, _
                                           chk随時５_６学年, _
                                           chk随時５_７学年, _
                                           chk随時５_８学年, _
                                           chk随時５_９学年)

    End Sub
    Private Sub chk随時６_全学年_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk随時６_全学年.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk随時６_全学年, _
                                           chk随時６_１学年, _
                                           chk随時６_２学年, _
                                           chk随時６_３学年, _
                                           chk随時６_４学年, _
                                           chk随時６_５学年, _
                                           chk随時６_６学年, _
                                           chk随時６_７学年, _
                                           chk随時６_８学年, _
                                           chk随時６_９学年)

    End Sub
#End Region

#Region " CheckedChanged "

    Private Sub chk４月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk４月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk４月振替日.Checked = False Then
            chk４月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk４月再振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk４月再振替日.CheckedChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk４月再振替日.Checked = True Then
            chk４月振替日.Checked = True
        End If
    End Sub

    Private Sub chk５月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk５月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk５月振替日.Checked = False Then
            chk５月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk５月再振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk５月再振替日.CheckedChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk５月再振替日.Checked = True Then
            chk５月振替日.Checked = True
        End If
    End Sub

    Private Sub chk６月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk６月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk６月振替日.Checked = False Then
            chk６月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk６月再振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk６月再振替日.CheckedChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk６月再振替日.Checked = True Then
            chk６月振替日.Checked = True
        End If
    End Sub

    Private Sub chk７月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk７月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk７月振替日.Checked = False Then
            chk７月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk７月再振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk７月再振替日.CheckedChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk７月再振替日.Checked = True Then
            chk７月振替日.Checked = True
        End If
    End Sub

    Private Sub chk８月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk８月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk８月振替日.Checked = False Then
            chk８月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk８月再振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk８月再振替日.CheckedChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk８月再振替日.Checked = True Then
            chk８月振替日.Checked = True
        End If
    End Sub

    Private Sub chk９月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk９月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk９月振替日.Checked = False Then
            chk９月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk９月再振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk９月再振替日.CheckedChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk９月再振替日.Checked = True Then
            chk９月振替日.Checked = True
        End If
    End Sub

    Private Sub chk１０月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk１０月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk１０月振替日.Checked = False Then
            chk１０月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk１０月再振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk１０月再振替日.CheckedChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk１０月再振替日.Checked = True Then
            chk１０月振替日.Checked = True
        End If
    End Sub

    Private Sub chk１１月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk１１月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk１１月振替日.Checked = False Then
            chk１１月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk１１月再振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk１１月再振替日.CheckedChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk１１月再振替日.Checked = True Then
            chk１１月振替日.Checked = True
        End If
    End Sub

    Private Sub chk１２月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk１２月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk１２月振替日.Checked = False Then
            chk１２月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk１２月再振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk１２月再振替日.CheckedChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk１２月再振替日.Checked = True Then
            chk１２月振替日.Checked = True
        End If
    End Sub

    Private Sub chk１月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk１月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk１月振替日.Checked = False Then
            chk１月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk１月再振替日_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk１月再振替日.CheckStateChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk１月再振替日.Checked = True Then
            chk１月振替日.Checked = True
        End If
    End Sub

    Private Sub chk２月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk２月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk２月振替日.Checked = False Then
            chk２月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk２月再振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk２月再振替日.CheckedChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk２月再振替日.Checked = True Then
            chk２月振替日.Checked = True
        End If
    End Sub

    Private Sub chk３月振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk３月振替日.CheckedChanged
        '2006/11/22　初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If chk３月振替日.Checked = False Then
            chk３月再振替日.Checked = False
        End If
    End Sub

    Private Sub chk３月再振替日_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk３月再振替日.CheckedChanged
        '2006/11/22　再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If chk３月再振替日.Checked = True Then
            chk３月振替日.Checked = True
        End If
    End Sub

#End Region

#Region " Private Sub(共通)"
    Private Sub PSUB_FORMAT_ALL()

        '基本情報部初期化
        Call PSUB_KIHON_FORMAT()

        '年間スケジュール画面初期化
        Call PSUB_NENKAN_FORMAT()

        '特別スケジュール画面初期化
        Call PSUB_TOKUBETU_FORMAT()

        '随時スケジュール画面初期化
        Call PSUB_ZUIJI_FORMAT()

        'ボタンEnabled初期状態
        Call PSUB_BUTTON_Enable()

    End Sub

    Private Sub PSUB_BUTTON_Enable(Optional ByVal pIndex As Integer = 0)

        Select Case pIndex
            Case 0
                btnAction.Enabled = True
                btnFind.Enabled = True
                btnUpdate.Enabled = False
                btnEraser.Enabled = True
                txtGAKKOU_CODE.Enabled = True
                cmbGakkouName.Enabled = True
                cmbKana.Enabled = True
                txt対象年度.Enabled = True
            Case 1
                btnAction.Enabled = False
                btnFind.Enabled = True
                btnUpdate.Enabled = True
                btnEraser.Enabled = True
                txtGAKKOU_CODE.Enabled = False
                cmbGakkouName.Enabled = False
                cmbKana.Enabled = False
                txt対象年度.Enabled = False
            Case 2
                btnAction.Enabled = False '2007/02/15
                btnFind.Enabled = True
                btnUpdate.Enabled = False
                btnEraser.Enabled = True
                txtGAKKOU_CODE.Enabled = True
                cmbGakkouName.Enabled = True
                cmbKana.Enabled = True
                txt対象年度.Enabled = True
        End Select

    End Sub

    Private Sub PSUB_KIHON_FORMAT()

        txt対象年度.Enabled = True
        'txt対象年度.Text = ""

        txtGAKKOU_CODE.Enabled = True
        txtGAKKOU_CODE.Text = ""

        lab学校名.Text = ""

        '休日リストボックス初期化
        lst休日.Items.Clear()

        '学校検索（カナ）
        cmbKana.SelectedIndex = -1

        '追加 2007/02/15
        '学校コンボ設定（全学校）
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "学校コンボ設定", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
            MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        '学校検索（学校名）
        cmbGakkouName.SelectedIndex = -1

    End Sub

    '========================================
    'スケジュールマスタ登録
    '2006/11/30　学年フラグを変更
    '========================================
    Private Function PSUB_INSERT_G_SCHMAST_SQL() As String

        Dim sql As String = ""

        '各種予定日の算出
        Dim CLS As New GAKKOU.ClsSchduleMaintenanceClass
        Call CLS.SetKyuzituInformation()

        CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

        'スケジュール作成対象の取引先コードを抽出
        CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(STR振替日), strGakkouCode, "01")

        CLS.SCH.FURI_DATE = GCom.SET_DATE(STR振替日)
        If CLS.SCH.FURI_DATE = "00000000" Then
        Else
            CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
        End If

        Dim strFURI_DATE As String = CLS.SCH.FURI_DATE 'デバッグ用？

        '2010/10/21 契約振替日対応 ここから
        If STR契約振替日 = "" OrElse STR契約振替日.Length <> 8 Then
            '引数がない場合は実振替日を設定
            CLS.SCH.KFURI_DATE = CLS.SCH.FURI_DATE
        Else
            CLS.SCH.KFURI_DATE = STR契約振替日
        End If
        '2010/10/21 契約振替日対応 ここまで

        Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

        Dim ENTRY_Y_DATE As String = "00000000"                                                   '明細作成予定日算出
        Dim CHECK_Y_DATE As String = fn_GetEigyoubi(STR振替日, STR_JIFURI_CHECK, "-")           'チェック予定日算出
        Dim DATA_Y_DATE As String = fn_GetEigyoubi(STR振替日, STR_JIFURI_HAISIN, "-")       '振替データ作成予定日算出
        Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '不能結果更新予定日算出
        Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '資金決済予定日算出
        Dim HENKAN_Y_DATE As String = CLS.SCH.HENKAN_YDATE                                          '返還予定日

        'INSERT文作成
        sql += "INSERT INTO G_SCHMAST "
        sql += " VALUES ( "
        '学校コード
        sql += "'" & GAKKOU_INFO.GAKKOU_CODE & "'"
        '請求年月
        sql += ",'" & STR請求年月 & "'"
        'スケジュール区分
        sql += ",'" & STRスケ区分 & "'"
        '振替区分
        sql += ",'" & STR振替区分 & "'"
        '振替日
        sql += ",'" & STR振替日 & "'"
        '再振替日
        sql += ",'" & STR再振替日 & "'"
        '学年１
        sql += ",'" & STR１学年 & "'"
        '学年２
        sql += ",'" & STR２学年 & "'"
        '学年３
        sql += ",'" & STR３学年 & "'"
        '学年４
        sql += ",'" & STR４学年 & "'"
        '学年５
        sql += ",'" & STR５学年 & "'"
        '学年６
        sql += ",'" & STR６学年 & "'"
        '学年７
        sql += ",'" & STR７学年 & "'"
        '学年８
        sql += ",'" & STR８学年 & "'"
        '学年９
        sql += ",'" & STR９学年 & "'"
        '委託者コード
       
        '2011/06/16 標準版修正 委託者コードの下１桁変更を行わない------------------START
        sql += ",'" & GAKKOU_INFO.ITAKU_CODE & "'"
        'Select Case STR振替区分
        '    Case "0"
        '        sql += ",'" & "0" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        '    Case "1"
        '        sql += ",'" & "1" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        '    Case "2"
        '        sql += ",'" & "2" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        '    Case "3"
        '        sql += ",'" & "3" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        'End Select
        '2011/06/16 標準版修正 委託者コードの下１桁変更を行わない------------------END
        '取扱金融機関
        sql += ",'" & GAKKOU_INFO.TKIN_CODE & "'"
        '取扱支店
        sql += ",'" & GAKKOU_INFO.TSIT_CODE & "'"
        '媒体コード 
        sql += ",'" & GAKKOU_INFO.BAITAI_CODE & "'"
        '手数料区分 
        sql += ",'" & GAKKOU_INFO.TESUUTYO_KBN & "'"
        '明細作成予定日
        sql += "," & "'" & ENTRY_Y_DATE & "'"
        '明細作成日
        sql += "," & "'00000000'"
        'チェック予定日
        sql += "," & "'" & CHECK_Y_DATE & "'"
        'チェック日
        sql += "," & "'00000000'"
        '振替データ作成予定日
        sql += "," & "'" & DATA_Y_DATE & "'"
        '振替データ作成日
        sql += "," & "'00000000'"
        '不能結果更新予定日
        sql += "," & "'" & FUNOU_Y_DATE & "'"
        '不能結果更新日
        sql += "," & "'00000000'"
        '返還予定日
        sql += "," & "'" & HENKAN_Y_DATE & "'"
        '返還日
        sql += "," & "'00000000'"
        '決済予定日
        sql += "," & "'" & KESSAI_Y_DATE & "'"
        '決済日
        sql += "," & "'00000000'"
        '明細作成済フラグ
        sql += "," & "'" & strENTRI_FLG & "'"
        '金額確認済フラグ
        sql += "," & "'" & strCHECK_FLG & "'"
        '振替データ作成済フラグ
        sql += "," & "'" & strDATA_FLG & "'"
        '不能結果更新済フラグ
        sql += "," & "'" & strFUNOU_FLG & "'"
        '返還済フラグ
        sql += "," & "'" & strHENKAN_FLG & "'"
        '再振データ作成済フラグ
        sql += "," & "'" & strSAIFURI_FLG & "'"
        '決済済フラグ
        sql += "," & "'" & strKESSAI_FLG & "'"
        '中断フラグ
        sql += "," & "'" & strTYUUDAN_FLG & "'"
        '処理件数
        sql += "," & lngSYORI_KEN
        '処理金額
        sql += "," & dblSYORI_KIN
        '手数料
        sql += "," & 0
        '手数料１
        sql += "," & 0
        '手数料２
        sql += "," & 0
        '手数料３
        sql += "," & 0
        '振替済件数
        sql += "," & lngFURI_KEN
        '振替済金額
        sql += "," & dblFURI_KIN
        '不能件数
        sql += "," & lngFUNOU_KEN
        '不能金額
        sql += "," & dblFUNOU_KIN
        '作成日付
        sql += "," & "'" & Str_SyoriDate(0) & "'"
        'タイムスタンプ
        sql += "," & "'" & Str_SyoriDate(1) & "'"
        '予備１
        sql += "," & "'" & STR年間入力振替日 & "'"
        '予備２
        sql += "," & "'" & Space(30) & "'"
        '予備３
        sql += "," & "'" & Space(30) & "'"
        '予備４
        sql += "," & "'" & Space(30) & "'"
        '予備５
        sql += "," & "'" & Space(30) & "'"
        '予備６
        sql += "," & "'" & Space(30) & "'"
        '予備７
        sql += "," & "'" & Space(30) & "'"
        '予備８
        sql += "," & "'" & Space(30) & "'"
        '予備９
        sql += "," & "'" & Space(30) & "'"
        '予備１０
        sql += "," & "'" & Space(30) & "')"

        Return sql

    End Function

    '===================================================
    'PSUB_UPDATE_G_SCHMAST_SQL
    'UPDATE 2006/11/30　年間・特別・随時それぞれに対応
    '===================================================
    Private Function PSUB_UPDATE_G_SCHMAST_SQL(ByVal strJoken_Furi_Date As String, ByVal strJoken_SFuri_Date As String) As String
        'strJoken_Furi_Date ：更新前振替日
        'strJoken_SFuri_Date：更新前再振日

        Dim sql As String = ""

        '更新前再振日が空白の場合は0埋めする
        If Trim(strJoken_SFuri_Date) = "" Then
            strJoken_SFuri_Date = "00000000"
        End If

        '各種予定日の算出
        Dim CLS As New GAKKOU.ClsSchduleMaintenanceClass
        Call CLS.SetKyuzituInformation()

        CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

        'スケジュール作成対象の取引先コードを抽出
        CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(STR振替日), strGakkouCode, "01")

        CLS.SCH.FURI_DATE = GCom.SET_DATE(STR振替日)
        If CLS.SCH.FURI_DATE = "00000000" Then
        Else
            CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
        End If

        Dim strFURI_DATE As String = CLS.SCH.FURI_DATE 'デバッグ用？

        '2010/10/21 契約振替日対応 ここから
        If STR契約振替日 = "" OrElse STR契約振替日.Length <> 8 Then
            '引数がない場合は実振替日を設定
            CLS.SCH.KFURI_DATE = CLS.SCH.FURI_DATE
        Else
            CLS.SCH.KFURI_DATE = STR契約振替日
        End If
        '2010/10/21 契約振替日対応 ここまで

        Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

        Dim ENTRY_Y_DATE As String = "00000000"                                                   '明細作成予定日算出
        Dim CHECK_Y_DATE As String = fn_GetEigyoubi(STR振替日, STR_JIFURI_CHECK, "-")           'チェック予定日算出
        Dim DATA_Y_DATE As String = fn_GetEigyoubi(STR振替日, STR_JIFURI_HAISIN, "-")       '振替データ作成予定日算出
        Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '不能結果更新予定日算出
        Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '資金決済予定日算出
        Dim HENKAN_Y_DATE As String = CLS.SCH.HENKAN_YDATE                                          '返還予定日

        'UPDATE文作成
        sql = " UPDATE  G_SCHMAST"
        sql += " SET "
        sql += " FURI_DATE_S = '" & STR振替日 & "'," '   2006/11/22　振替日
        sql += " SFURI_DATE_S = '" & STR再振替日 & "'," '2006/11/22　再振日
        sql += " GAKUNEN1_FLG_S  ='" & STR１学年 & "',"
        sql += " GAKUNEN2_FLG_S  ='" & STR２学年 & "',"
        sql += " GAKUNEN3_FLG_S  ='" & STR３学年 & "',"
        sql += " GAKUNEN4_FLG_S  ='" & STR４学年 & "',"
        sql += " GAKUNEN5_FLG_S  ='" & STR５学年 & "',"
        sql += " GAKUNEN6_FLG_S  ='" & STR６学年 & "',"
        sql += " GAKUNEN7_FLG_S  ='" & STR７学年 & "',"
        sql += " GAKUNEN8_FLG_S  ='" & STR８学年 & "',"
        sql += " GAKUNEN9_FLG_S  ='" & STR９学年 & "',"
        sql += " SYORI_KEN_S =" & lngSYORI_KEN & ","
        sql += " SYORI_KIN_S =" & dblSYORI_KIN & ","
        sql += " FURI_KEN_S =" & lngFURI_KEN & ","
        sql += " FURI_KIN_S =" & dblFURI_KIN & ","
        sql += " FUNOU_KEN_S =" & lngFUNOU_KEN & ","
        sql += " FUNOU_KIN_S =" & dblFUNOU_KIN & ","
        sql += " YOBI1_S = '" & STR年間入力振替日 & "',"
        '各予定日更新 2007/12/13
        sql += " ENTRI_YDATE_S ='" & ENTRY_Y_DATE & "',"
        sql += " CHECK_YDATE_S ='" & CHECK_Y_DATE & "',"
        sql += " DATA_YDATE_S ='" & DATA_Y_DATE & "',"
        sql += " FUNOU_YDATE_S ='" & FUNOU_Y_DATE & "',"
        sql += " HENKAN_YDATE_S ='" & HENKAN_Y_DATE & "',"
        sql += " KESSAI_YDATE_S ='" & KESSAI_Y_DATE & "'"
        sql += " WHERE"
        sql += " GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'"
        sql += " AND"
        sql += " NENGETUDO_S ='" & STR請求年月 & "'"
        sql += " AND"
        sql += " SCH_KBN_S ='" & STRスケ区分 & "'"
        sql += " AND"
        sql += " FURI_KBN_S ='" & STR振替区分 & "'"
        sql += " AND"

        '2006/11/22　条件を旧データに修正
        'sql += " FURI_DATE_S ='" & STR振替日 & "'"
        'sql += " FURI_DATE_S ='" & str旧振替日(int旧振替ＩＤ) & "'"
        sql += " FURI_DATE_S = '" & strJoken_Furi_Date & "'"
        sql += " AND"
        'sql += " SFURI_DATE_S ='" & STR再振替日 & "'"
        'sql += " SFURI_DATE_S ='" & str旧再振日(int旧振替ＩＤ) & "'"
        sql += " SFURI_DATE_S = '" & strJoken_SFuri_Date & "'"

        Return sql

    End Function

    Private Sub PSUB_ZENGAKUNEN_CHKBOX_CNTROL(ByVal chkBOXALL As CheckBox, ByVal chkBOX1 As CheckBox, ByVal chkBOX2 As CheckBox, ByVal chkBOX3 As CheckBox, ByVal chkBOX4 As CheckBox, ByVal chkBOX5 As CheckBox, ByVal chkBOX6 As CheckBox, ByVal chkBOX7 As CheckBox, ByVal chkBOX8 As CheckBox, ByVal chkBOX9 As CheckBox)

        If chkBOXALL.Checked = True Then
            '各学年のチェックｏｆｆ
            chkBOX1.Checked = False
            chkBOX2.Checked = False
            chkBOX3.Checked = False
            chkBOX4.Checked = False
            chkBOX5.Checked = False
            chkBOX6.Checked = False
            chkBOX7.Checked = False
            chkBOX8.Checked = False
            chkBOX9.Checked = False
            '各学年のチェックボックス使用不可 
            chkBOX1.Enabled = False
            chkBOX2.Enabled = False
            chkBOX3.Enabled = False
            chkBOX4.Enabled = False
            chkBOX5.Enabled = False
            chkBOX6.Enabled = False
            chkBOX7.Enabled = False
            chkBOX8.Enabled = False
            chkBOX9.Enabled = False
        Else
            '各学年のチェックボックス使用可 
            chkBOX1.Enabled = True
            chkBOX2.Enabled = True
            chkBOX3.Enabled = True
            chkBOX4.Enabled = True
            chkBOX5.Enabled = True
            chkBOX6.Enabled = True
            chkBOX7.Enabled = True
            chkBOX8.Enabled = True
            chkBOX9.Enabled = True
            '2006/10/12　最高学年チェック
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()
        End If
    End Sub

    Private Sub PSUB_SANSYOU_FOCUS()

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & Trim(txtGAKKOU_CODE.Text) & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & Trim(txt対象年度.Text) & "04'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & CStr(CInt(Trim(txt対象年度.Text)) + 1) & "03'")

        If oraReader.DataReader(sql) = True Then
            btnAction.Enabled = False
            btnFind.Enabled = True
            btnFind.Focus()
        Else '追加 2007/02/15
            btnFind.Enabled = False
            btnAction.Enabled = True
            btnAction.Focus()
        End If

        oraReader.Close()

    End Sub
    '2006/10/25 変数クリア
    Public Sub sb_HENSU_CLEAR()
        strENTRI_FLG = "0"
        strCHECK_FLG = "0"
        strDATA_FLG = "0"
        strFUNOU_FLG = "0"
        strSAIFURI_FLG = "0"
        strKESSAI_FLG = "0"
        strTYUUDAN_FLG = "0"
        strENTRI_FLG_SAI = "0"
        strCHECK_FLG_SAI = "0"
        strDATA_FLG_SAI = "0"
        strFUNOU_FLG_SAI = "0"
        strSAIFURI_FLG_SAI = "0"
        strKESSAI_FLG_SAI = "0"
        strTYUUDAN_FLG_SAI = "0"

        strSAIFURI_DEF = "00000000" '通常スケジュールの再振日

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0
    End Sub

    '==========================================
    '変更された項目をチェック  2006/11/30
    '==========================================
    Private Sub PSUB_Kousin_Check()

        '--------------------------------------
        '各欄の値を構造体に入力（更新時のもの）
        '--------------------------------------
        Call PSUB_NENKAN_GET(NENKAN_SCHINFO)
        Call PSUB_TOKUBETU_GET(TOKUBETU_SCHINFO)
        Call PSUB_ZUIJI_GET(ZUIJI_SCHINFO)

        '参照時と更新時の項目を比べ、変更があったものの更新フラグを立てる

        For i As Integer = 1 To 12
            '--------------------------------------
            '年間スケジュールチェック
            '--------------------------------------
            If NENKAN_SCHINFO(i).Furikae_Check = SYOKI_NENKAN_SCHINFO(i).Furikae_Check And _
               NENKAN_SCHINFO(i).Furikae_Date = SYOKI_NENKAN_SCHINFO(i).Furikae_Date And _
               NENKAN_SCHINFO(i).Furikae_Day = SYOKI_NENKAN_SCHINFO(i).Furikae_Day And _
               NENKAN_SCHINFO(i).Furikae_Enabled = SYOKI_NENKAN_SCHINFO(i).Furikae_Enabled And _
               NENKAN_SCHINFO(i).SaiFurikae_Check = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Check And _
               NENKAN_SCHINFO(i).SaiFurikae_Date = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Date And _
               NENKAN_SCHINFO(i).SaiFurikae_Day = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day And _
               NENKAN_SCHINFO(i).SaiFurikae_Enabled = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Enabled Then

                bln年間更新(i) = False '変更なし
            Else
                bln年間更新(i) = True ' 変更あり
            End If
        Next

        For i As Integer = 1 To 6
            '--------------------------------------
            '特別スケジュールチェック
            '--------------------------------------
            '2006/12/12　一部追加：入力が不足していた場合、更新しない
            If (TOKUBETU_SCHINFO(i).Seikyu_Tuki = SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki And _
               TOKUBETU_SCHINFO(i).Furikae_Tuki = SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki And _
               TOKUBETU_SCHINFO(i).Furikae_Date = SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date And _
               TOKUBETU_SCHINFO(i).SaiFurikae_Tuki = SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki And _
               TOKUBETU_SCHINFO(i).SaiFurikae_Date = SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date And _
               TOKUBETU_SCHINFO(i).SiyouGakunen1_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen1_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen2_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen2_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen3_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen3_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen4_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen4_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen5_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen5_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen6_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen6_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen7_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen7_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen8_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen8_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen9_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen9_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) Or _
               ((TOKUBETU_SCHINFO(i).Furikae_Tuki = "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "") Or _
               (TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date = "")) Or _
               ((TOKUBETU_SCHINFO(i).SaiFurikae_Tuki = "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "") Or _
               (TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date = "")) Then

                bln特別更新(i) = False '変更なし
            Else
                bln特別更新(i) = True ' 変更あり
            End If

            '--------------------------------------
            '随時スケジュールチェック
            '--------------------------------------
            '2006/12/12　一部追加：入力が不足していた場合、更新しない
            If (ZUIJI_SCHINFO(i).Furikae_Tuki = SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki And _
               ZUIJI_SCHINFO(i).Furikae_Date = SYOKI_ZUIJI_SCHINFO(i).Furikae_Date And _
               ZUIJI_SCHINFO(i).Nyusyutu_Kbn = SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn And _
               ZUIJI_SCHINFO(i).SiyouGakunen1_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen1_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen2_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen2_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen3_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen3_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen4_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen4_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen5_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen5_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen6_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen6_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen7_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen7_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen8_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen8_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen9_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen9_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunenALL_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunenALL_Check) Or _
               ((ZUIJI_SCHINFO(i).Furikae_Tuki = "" And ZUIJI_SCHINFO(i).Furikae_Date <> "") Or _
               (ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And ZUIJI_SCHINFO(i).Furikae_Date = "")) Then

                bln随時更新(i) = False '変更なし
            Else
                bln随時更新(i) = True ' 変更あり
            End If
        Next

    End Sub

    '画面表示時退避　2006/12/04
    Public Sub sb_SANSYOU_SET()
        '年間初振
        '2010/10/21 チェックボックスの状態を見る
        'If lab１月振替日.Text.Trim = "" Then
        If lab１月振替日.Text.Trim = "" Or chk１月振替日.Checked = False Then
            strSYOFURI_NENKAN(1) = ""
        Else
            strSYOFURI_NENKAN(1) = Replace(lab１月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab２月振替日.Text.Trim = "" Then
        If lab２月振替日.Text.Trim = "" Or chk２月振替日.Checked = False Then
            strSYOFURI_NENKAN(2) = ""
        Else
            strSYOFURI_NENKAN(2) = Replace(lab２月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab３月振替日.Text.Trim = "" Then
        If lab３月振替日.Text.Trim = "" Or chk３月振替日.Checked = False Then
            strSYOFURI_NENKAN(3) = ""
        Else
            strSYOFURI_NENKAN(3) = Replace(lab３月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab４月振替日.Text.Trim = "" Then
        If lab４月振替日.Text.Trim = "" Or chk４月振替日.Checked = False Then
            strSYOFURI_NENKAN(4) = ""
        Else
            strSYOFURI_NENKAN(4) = Replace(lab４月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab５月振替日.Text.Trim = "" Then
        If lab５月振替日.Text.Trim = "" Or chk５月振替日.Checked = False Then
            strSYOFURI_NENKAN(5) = ""
        Else
            strSYOFURI_NENKAN(5) = Replace(lab５月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab６月振替日.Text.Trim = "" Then
        If lab６月振替日.Text.Trim = "" Or chk６月振替日.Checked = False Then
            strSYOFURI_NENKAN(6) = ""
        Else
            strSYOFURI_NENKAN(6) = Replace(lab６月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab７月振替日.Text.Trim = "" Then
        If lab７月振替日.Text.Trim = "" Or chk７月振替日.Checked = False Then
            strSYOFURI_NENKAN(7) = ""
        Else
            strSYOFURI_NENKAN(7) = Replace(lab７月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab８月振替日.Text.Trim = "" Then
        If lab８月振替日.Text.Trim = "" Or chk８月振替日.Checked = False Then
            strSYOFURI_NENKAN(8) = ""
        Else
            strSYOFURI_NENKAN(8) = Replace(lab８月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab９月振替日.Text.Trim = "" Then
        If lab９月振替日.Text.Trim = "" Or chk９月振替日.Checked = False Then
            strSYOFURI_NENKAN(9) = ""
        Else
            strSYOFURI_NENKAN(9) = Replace(lab９月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１０月振替日.Text.Trim = "" Then
        If lab１０月振替日.Text.Trim = "" Or chk１０月振替日.Checked = False Then
            strSYOFURI_NENKAN(10) = ""
        Else
            strSYOFURI_NENKAN(10) = Replace(lab１０月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１１月振替日.Text.Trim = "" Then
        If lab１１月振替日.Text.Trim = "" Or chk１１月振替日.Checked = False Then
            strSYOFURI_NENKAN(11) = ""
        Else
            strSYOFURI_NENKAN(11) = Replace(lab１１月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１２月振替日.Text.Trim = "" Then
        If lab１２月振替日.Text.Trim = "" Or chk１２月振替日.Checked = False Then
            strSYOFURI_NENKAN(12) = ""
        Else
            strSYOFURI_NENKAN(12) = Replace(lab１２月振替日.Text, "/", "")
        End If
        '年間再振
        '2010/10/21 チェックボックスの状態を見る
        'If lab１月再振替日.Text.Trim = "" Then
        If lab１月再振替日.Text.Trim = "" Or chk１月再振替日.Checked = False Then
            strSAIFURI_NENKAN(1) = ""
        Else
            strSAIFURI_NENKAN(1) = Replace(lab１月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab２月再振替日.Text.Trim = "" Then
        If lab２月再振替日.Text.Trim = "" Or chk２月再振替日.Checked = False Then
            strSAIFURI_NENKAN(2) = ""
        Else
            strSAIFURI_NENKAN(2) = Replace(lab２月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab３月再振替日.Text.Trim = "" Then
        If lab３月再振替日.Text.Trim = "" Or chk３月再振替日.Checked = False Then
            strSAIFURI_NENKAN(3) = ""
        Else
            strSAIFURI_NENKAN(3) = Replace(lab３月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab４月再振替日.Text.Trim = "" Then
        If lab４月再振替日.Text.Trim = "" Or chk４月再振替日.Checked = False Then
            strSAIFURI_NENKAN(4) = ""
        Else
            strSAIFURI_NENKAN(4) = Replace(lab４月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab５月再振替日.Text.Trim = "" Then
        If lab５月再振替日.Text.Trim = "" Or chk５月再振替日.Checked = False Then
            strSAIFURI_NENKAN(5) = ""
        Else
            strSAIFURI_NENKAN(5) = Replace(lab５月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab６月再振替日.Text.Trim = "" Then
        If lab６月再振替日.Text.Trim = "" Or chk６月再振替日.Checked = False Then
            strSAIFURI_NENKAN(6) = ""
        Else
            strSAIFURI_NENKAN(6) = Replace(lab６月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab７月再振替日.Text.Trim = "" Then
        If lab７月再振替日.Text.Trim = "" Or chk７月再振替日.Checked = False Then
            strSAIFURI_NENKAN(7) = ""
        Else
            strSAIFURI_NENKAN(7) = Replace(lab７月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab８月再振替日.Text.Trim = "" Then
        If lab８月再振替日.Text.Trim = "" Or chk８月再振替日.Checked = False Then
            strSAIFURI_NENKAN(8) = ""
        Else
            strSAIFURI_NENKAN(8) = Replace(lab８月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab９月再振替日.Text.Trim = "" Then
        If lab９月再振替日.Text.Trim = "" Or chk９月再振替日.Checked = False Then
            strSAIFURI_NENKAN(9) = ""
        Else
            strSAIFURI_NENKAN(9) = Replace(lab９月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１０月再振替日.Text.Trim = "" Then
        If lab１０月再振替日.Text.Trim = "" Or chk１０月再振替日.Checked = False Then
            strSAIFURI_NENKAN(10) = ""
        Else
            strSAIFURI_NENKAN(10) = Replace(lab１０月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１１月再振替日.Text.Trim = "" Then
        If lab１１月再振替日.Text.Trim = "" Or chk１１月再振替日.Checked = False Then
            strSAIFURI_NENKAN(11) = ""
        Else
            strSAIFURI_NENKAN(11) = Replace(lab１１月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１２月再振替日.Text.Trim = "" Then
        If lab１２月再振替日.Text.Trim = "" Or chk１２月再振替日.Checked = False Then
            strSAIFURI_NENKAN(12) = ""
        Else
            strSAIFURI_NENKAN(12) = Replace(lab１２月再振替日.Text, "/", "")
        End If
        '特別初振
        strSYOFURI_TOKUBETU(1) = PFUNC_SEIKYUTUKIHI(txt特別振替月１.Text) & txt特別振替日１.Text
        strSYOFURI_TOKUBETU(2) = PFUNC_SEIKYUTUKIHI(txt特別振替月２.Text) & txt特別振替日２.Text
        strSYOFURI_TOKUBETU(3) = PFUNC_SEIKYUTUKIHI(txt特別振替月３.Text) & txt特別振替日３.Text
        strSYOFURI_TOKUBETU(4) = PFUNC_SEIKYUTUKIHI(txt特別振替月４.Text) & txt特別振替日４.Text
        strSYOFURI_TOKUBETU(5) = PFUNC_SEIKYUTUKIHI(txt特別振替月５.Text) & txt特別振替日５.Text
        strSYOFURI_TOKUBETU(6) = PFUNC_SEIKYUTUKIHI(txt特別振替月６.Text) & txt特別振替日６.Text
        '特別再振
        strSAIFURI_TOKUBETU(1) = PFUNC_SEIKYUTUKIHI(txt特別再振替月１.Text) & txt特別再振替日１.Text
        strSAIFURI_TOKUBETU(2) = PFUNC_SEIKYUTUKIHI(txt特別再振替月２.Text) & txt特別再振替日２.Text
        strSAIFURI_TOKUBETU(3) = PFUNC_SEIKYUTUKIHI(txt特別再振替月３.Text) & txt特別再振替日３.Text
        strSAIFURI_TOKUBETU(4) = PFUNC_SEIKYUTUKIHI(txt特別再振替月４.Text) & txt特別再振替日４.Text
        strSAIFURI_TOKUBETU(5) = PFUNC_SEIKYUTUKIHI(txt特別再振替月５.Text) & txt特別再振替日５.Text
        strSAIFURI_TOKUBETU(6) = PFUNC_SEIKYUTUKIHI(txt特別再振替月６.Text) & txt特別再振替日６.Text
        '随時振替日
        strFURI_ZUIJI(1) = PFUNC_SEIKYUTUKIHI(txt随時振替月１.Text) & txt随時振替日１.Text
        strFURI_ZUIJI(2) = PFUNC_SEIKYUTUKIHI(txt随時振替月２.Text) & txt随時振替日２.Text
        strFURI_ZUIJI(3) = PFUNC_SEIKYUTUKIHI(txt随時振替月３.Text) & txt随時振替日３.Text
        strFURI_ZUIJI(4) = PFUNC_SEIKYUTUKIHI(txt随時振替月４.Text) & txt随時振替日４.Text
        strFURI_ZUIJI(5) = PFUNC_SEIKYUTUKIHI(txt随時振替月５.Text) & txt随時振替日５.Text
        strFURI_ZUIJI(6) = PFUNC_SEIKYUTUKIHI(txt随時振替月６.Text) & txt随時振替日６.Text
        '随時振替区分
        strFURIKBN_ZUIJI(1) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分１)
        strFURIKBN_ZUIJI(2) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分２)
        strFURIKBN_ZUIJI(3) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分３)
        strFURIKBN_ZUIJI(4) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分４)
        strFURIKBN_ZUIJI(5) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分５)
        strFURIKBN_ZUIJI(6) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分６)
    End Sub

    '更新後の状態退避　2006/12/04
    Public Sub sb_KOUSIN_SET()
        '年間初振
        '2010/10/21 チェックボックスの状態を見る
        'If lab１月振替日.Text.Trim = "" Then
        If lab１月振替日.Text.Trim = "" Or chk１月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(1) = ""
        Else
            strSYOFURI_NENKAN_AFTER(1) = Replace(lab１月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab２月振替日.Text.Trim = "" Then
        If lab２月振替日.Text.Trim = "" Or chk２月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(2) = ""
        Else
            strSYOFURI_NENKAN_AFTER(2) = Replace(lab２月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab３月振替日.Text.Trim = "" Then
        If lab３月振替日.Text.Trim = "" Or chk３月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(3) = ""
        Else
            strSYOFURI_NENKAN_AFTER(3) = Replace(lab３月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab４月振替日.Text.Trim = "" Then
        If lab４月振替日.Text.Trim = "" Or chk４月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(4) = ""
        Else
            strSYOFURI_NENKAN_AFTER(4) = Replace(lab４月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab５月振替日.Text.Trim = "" Then
        If lab５月振替日.Text.Trim = "" Or chk５月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(5) = ""
        Else
            strSYOFURI_NENKAN_AFTER(5) = Replace(lab５月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab６月振替日.Text.Trim = "" Then
        If lab６月振替日.Text.Trim = "" Or chk６月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(6) = ""
        Else
            strSYOFURI_NENKAN_AFTER(6) = Replace(lab６月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab７月振替日.Text.Trim = "" Then
        If lab７月振替日.Text.Trim = "" Or chk７月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(7) = ""
        Else
            strSYOFURI_NENKAN_AFTER(7) = Replace(lab７月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab８月振替日.Text.Trim = "" Then
        If lab８月振替日.Text.Trim = "" Or chk８月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(8) = ""
        Else
            strSYOFURI_NENKAN_AFTER(8) = Replace(lab８月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab９月振替日.Text.Trim = "" Then
        If lab９月振替日.Text.Trim = "" Or chk９月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(9) = ""
        Else
            strSYOFURI_NENKAN_AFTER(9) = Replace(lab９月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１０月振替日.Text.Trim = "" Then
        If lab１０月振替日.Text.Trim = "" Or chk１０月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(10) = ""
        Else
            strSYOFURI_NENKAN_AFTER(10) = Replace(lab１０月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１１月振替日.Text.Trim = "" Then
        If lab１１月振替日.Text.Trim = "" Or chk１１月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(11) = ""
        Else
            strSYOFURI_NENKAN_AFTER(11) = Replace(lab１１月振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１２月振替日.Text.Trim = "" Then
        If lab１２月振替日.Text.Trim = "" Or chk１２月振替日.Checked = False Then
            strSYOFURI_NENKAN_AFTER(12) = ""
        Else
            strSYOFURI_NENKAN_AFTER(12) = Replace(lab１２月振替日.Text, "/", "")
        End If
        '年間再振
        '2010/10/21 チェックボックスの状態を見る
        'If lab１月再振替日.Text.Trim = "" Then
        If lab１月再振替日.Text.Trim = "" Or chk１月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(1) = ""
        Else
            strSAIFURI_NENKAN_AFTER(1) = Replace(lab１月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab２月再振替日.Text.Trim = "" Then
        If lab２月再振替日.Text.Trim = "" Or chk２月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(2) = ""
        Else
            strSAIFURI_NENKAN_AFTER(2) = Replace(lab２月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab３月再振替日.Text.Trim = "" Then
        If lab３月再振替日.Text.Trim = "" Or chk３月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(3) = ""
        Else
            strSAIFURI_NENKAN_AFTER(3) = Replace(lab３月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab４月再振替日.Text.Trim = "" Then
        If lab４月再振替日.Text.Trim = "" Or chk４月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(4) = ""
        Else
            strSAIFURI_NENKAN_AFTER(4) = Replace(lab４月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab５月再振替日.Text.Trim = "" Then
        If lab５月再振替日.Text.Trim = "" Or chk５月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(5) = ""
        Else
            strSAIFURI_NENKAN_AFTER(5) = Replace(lab５月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab６月再振替日.Text.Trim = "" Then
        If lab６月再振替日.Text.Trim = "" Or chk６月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(6) = ""
        Else
            strSAIFURI_NENKAN_AFTER(6) = Replace(lab６月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab７月再振替日.Text.Trim = "" Then
        If lab７月再振替日.Text.Trim = "" Or chk７月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(7) = ""
        Else
            strSAIFURI_NENKAN_AFTER(7) = Replace(lab７月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab８月再振替日.Text.Trim = "" Then
        If lab８月再振替日.Text.Trim = "" Or chk８月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(8) = ""
        Else
            strSAIFURI_NENKAN_AFTER(8) = Replace(lab８月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab９月再振替日.Text.Trim = "" Then
        If lab９月再振替日.Text.Trim = "" Or chk９月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(9) = ""
        Else
            strSAIFURI_NENKAN_AFTER(9) = Replace(lab９月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１０月再振替日.Text.Trim = "" Then
        If lab１０月再振替日.Text.Trim = "" Or chk１０月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(10) = ""
        Else
            strSAIFURI_NENKAN_AFTER(10) = Replace(lab１０月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１１月再振替日.Text.Trim = "" Then
        If lab１１月再振替日.Text.Trim = "" Or chk１１月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(11) = ""
        Else
            strSAIFURI_NENKAN_AFTER(11) = Replace(lab１１月再振替日.Text, "/", "")
        End If
        '2010/10/21 チェックボックスの状態を見る
        'If lab１２月再振替日.Text.Trim = "" Then
        If lab１２月再振替日.Text.Trim = "" Or chk１２月再振替日.Checked = False Then
            strSAIFURI_NENKAN_AFTER(12) = ""
        Else
            strSAIFURI_NENKAN_AFTER(12) = Replace(lab１２月再振替日.Text, "/", "")
        End If
        '特別初振
        strSYOFURI_TOKUBETU_AFTER(1) = PFUNC_SEIKYUTUKIHI(txt特別振替月１.Text) & txt特別振替日１.Text
        strSYOFURI_TOKUBETU_AFTER(2) = PFUNC_SEIKYUTUKIHI(txt特別振替月２.Text) & txt特別振替日２.Text
        strSYOFURI_TOKUBETU_AFTER(3) = PFUNC_SEIKYUTUKIHI(txt特別振替月３.Text) & txt特別振替日３.Text
        strSYOFURI_TOKUBETU_AFTER(4) = PFUNC_SEIKYUTUKIHI(txt特別振替月４.Text) & txt特別振替日４.Text
        strSYOFURI_TOKUBETU_AFTER(5) = PFUNC_SEIKYUTUKIHI(txt特別振替月５.Text) & txt特別振替日５.Text
        strSYOFURI_TOKUBETU_AFTER(6) = PFUNC_SEIKYUTUKIHI(txt特別振替月６.Text) & txt特別振替日６.Text
        '特別再振
        strSAIFURI_TOKUBETU_AFTER(1) = PFUNC_SEIKYUTUKIHI(txt特別再振替月１.Text) & txt特別再振替日１.Text
        strSAIFURI_TOKUBETU_AFTER(2) = PFUNC_SEIKYUTUKIHI(txt特別再振替月２.Text) & txt特別再振替日２.Text
        strSAIFURI_TOKUBETU_AFTER(3) = PFUNC_SEIKYUTUKIHI(txt特別再振替月３.Text) & txt特別再振替日３.Text
        strSAIFURI_TOKUBETU_AFTER(4) = PFUNC_SEIKYUTUKIHI(txt特別再振替月４.Text) & txt特別再振替日４.Text
        strSAIFURI_TOKUBETU_AFTER(5) = PFUNC_SEIKYUTUKIHI(txt特別再振替月５.Text) & txt特別再振替日５.Text
        strSAIFURI_TOKUBETU_AFTER(6) = PFUNC_SEIKYUTUKIHI(txt特別再振替月６.Text) & txt特別再振替日６.Text
        '随時振替日
        strFURI_ZUIJI_AFTER(1) = PFUNC_SEIKYUTUKIHI(txt随時振替月１.Text) & txt随時振替日１.Text
        strFURI_ZUIJI_AFTER(2) = PFUNC_SEIKYUTUKIHI(txt随時振替月２.Text) & txt随時振替日２.Text
        strFURI_ZUIJI_AFTER(3) = PFUNC_SEIKYUTUKIHI(txt随時振替月３.Text) & txt随時振替日３.Text
        strFURI_ZUIJI_AFTER(4) = PFUNC_SEIKYUTUKIHI(txt随時振替月４.Text) & txt随時振替日４.Text
        strFURI_ZUIJI_AFTER(5) = PFUNC_SEIKYUTUKIHI(txt随時振替月５.Text) & txt随時振替日５.Text
        strFURI_ZUIJI_AFTER(6) = PFUNC_SEIKYUTUKIHI(txt随時振替月６.Text) & txt随時振替日６.Text
        '随時振替区分
        strFURIKBN_ZUIJI_AFTER(1) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分１)
        strFURIKBN_ZUIJI_AFTER(2) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分２)
        strFURIKBN_ZUIJI_AFTER(3) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分３)
        strFURIKBN_ZUIJI_AFTER(4) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分４)
        strFURIKBN_ZUIJI_AFTER(5) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分５)
        strFURIKBN_ZUIJI_AFTER(6) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分６)
    End Sub

#End Region

#Region " Private Function(共通)"
    Private Function PFUNC_COMMON_CHECK() As Boolean

        Dim sStart As String
        Dim sEnd As String

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        Try

            If Trim(txtGAKKOU_CODE.Text) = "" Then
                MessageBox.Show("学校コードが入力されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return False
            Else
                '学校マスタ存在チェック
                sql.Append("SELECT *")
                sql.Append(" FROM GAKMAST2")
                sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

                If oraReader.DataReader(sql) = True Then

                    Int_Zengo_Kbn(0) = oraReader.GetString("NKYU_CODE_T")
                    Int_Zengo_Kbn(1) = oraReader.GetString("SKYU_CODE_T")
                    '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------START
                    Sai_Zengo_Kbn = oraReader.GetString("SFURI_KYU_CODE_T")
                    '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------END

                    sStart = Mid(oraReader.GetString("KAISI_DATE_T"), 1, 4)
                    sEnd = Mid(oraReader.GetString("SYURYOU_DATE_T"), 1, 4)

                    strFURI_DT = oraReader.GetString("FURI_DATE_T") '2005/12/09
                    strSFURI_DT = ConvNullToString(oraReader.GetString("SFURI_DATE_T"), "") '2005/12/09

                Else
                    MessageBox.Show("入力された学校コードが存在しません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Return False
                End If

                oraReader.Close()

            End If

            If (Trim(txt対象年度.Text)) = "" Then
                MessageBox.Show("対象年度を入力してください", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txt対象年度.Focus()
                Return False
            Else
                Select Case (sStart <= txt対象年度.Text >= sEnd)
                    Case False
                        MessageBox.Show("対象年度が入力範囲外です(" & sStart & "〜" & sEnd & ")", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt対象年度.Focus()
                        Return False
                End Select
            End If

            GAKKOU_INFO.TAISYOU_START_NENDO = txt対象年度.Text & "04"
            GAKKOU_INFO.TAISYOU_END_NENDO = CStr(CInt(txt対象年度.Text) + 1) & "03"

            Return True

        Catch ex As Exception
            Throw
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

    End Function

    '==============================================================
    'チェックボックス状態チェック・学年フラグ変数取得　2006/11/30
    '==============================================================
    Private Function PFUNC_GAKUNENFLG_CHECK(ByVal blnCheck_FLG1 As Boolean, ByVal blnCheck_FLG2 As Boolean, ByVal blnCheck_FLG3 As Boolean, ByVal blnCheck_FLG4 As Boolean, ByVal blnCheck_FLG5 As Boolean, ByVal blnCheck_FLG6 As Boolean, ByVal blnCheck_FLG7 As Boolean, ByVal blnCheck_FLG8 As Boolean, ByVal blnCheck_FLG9 As Boolean, ByVal blnCheck_FLGALL As Boolean) As Boolean

        'チェックボックス状態チェック
        PFUNC_GAKUNENFLG_CHECK = False

        If blnCheck_FLG1 = False And _
           blnCheck_FLG2 = False And _
           blnCheck_FLG3 = False And _
           blnCheck_FLG4 = False And _
           blnCheck_FLG5 = False And _
           blnCheck_FLG6 = False And _
           blnCheck_FLG7 = False And _
           blnCheck_FLG8 = False And _
           blnCheck_FLG9 = False And _
           blnCheck_FLGALL = False Then

            Call MessageBox.Show("処理対象学年指定がされていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        'チェックボックス状態を共通変数に設定()
        If blnCheck_FLGALL = True Then
            STR１学年 = "1"
            STR２学年 = "1"
            STR３学年 = "1"
            STR４学年 = "1"
            STR５学年 = "1"
            STR６学年 = "1"
            STR７学年 = "1"
            STR８学年 = "1"
            STR９学年 = "1"
        Else
            If blnCheck_FLG1 = True Then
                STR１学年 = "1"
            Else
                STR１学年 = "0"
            End If
            If blnCheck_FLG2 = True Then
                STR２学年 = "1"
            Else
                STR２学年 = "0"
            End If
            If blnCheck_FLG3 = True Then
                STR３学年 = "1"
            Else
                STR３学年 = "0"
            End If
            If blnCheck_FLG4 = True Then
                STR４学年 = "1"
            Else
                STR４学年 = "0"
            End If
            If blnCheck_FLG5 = True Then
                STR５学年 = "1"
            Else
                STR５学年 = "0"
            End If
            If blnCheck_FLG6 = True Then
                STR６学年 = "1"
            Else
                STR６学年 = "0"
            End If
            If blnCheck_FLG7 = True Then
                STR７学年 = "1"
            Else
                STR７学年 = "0"
            End If
            If blnCheck_FLG8 = True Then
                STR８学年 = "1"
            Else
                STR８学年 = "0"
            End If
            If blnCheck_FLG9 = True Then
                STR９学年 = "1"
            Else
                STR９学年 = "0"
            End If
        End If

        PFUNC_GAKUNENFLG_CHECK = True

    End Function

    Private Function PFUNC_KYUJITULIST_SET() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        '休日情報の表示
        Dim sTuki As String
        Dim sDay As String
        Dim sYName As String

        lst休日.Items.Clear()

        If Trim(txt対象年度.Text) <> "" Then
            Select Case CInt(txt対象年度.Text)
                Case Is > 1900
                    sql.Append(" SELECT ")
                    sql.Append(" YASUMI_DATE_Y")
                    sql.Append(",YASUMI_NAME_Y")
                    sql.Append(" FROM YASUMIMAST")
                    sql.Append(" WHERE")
                    sql.Append(" YASUMI_DATE_Y > '" & txt対象年度.Text & "0400'")
                    sql.Append(" AND")
                    sql.Append(" YASUMI_DATE_Y < '" & CStr(CInt(txt対象年度.Text) + 1) & "0399'")
                    sql.Append(" ORDER BY YASUMI_DATE_Y ASC")

                    If oraReader.DataReader(sql) = True Then

                        Do Until oraReader.EOF

                            sTuki = Mid(oraReader.GetString("YASUMI_DATE_Y"), 5, 2)
                            sDay = Mid(oraReader.GetString("YASUMI_DATE_Y"), 7, 2)
                            sYName = Trim(oraReader.GetString("YASUMI_NAME_Y"))

                            lst休日.Items.Add(sTuki & "月" & sDay & "日" & Space(1) & sYName)

                            '2006/10/23　休日一覧を取得
                            STRYasumi_List(STRYasumi_List.Length - 1) = txt対象年度.Text & sTuki & sDay
                            ReDim Preserve STRYasumi_List(STRYasumi_List.Length)

                            oraReader.NextRead()

                        Loop

                    End If
                    oraReader.Close()

                Case Else
                    MessageBox.Show("対象年度は１９００年以降を入力してください", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt対象年度.Focus()
                    Return False
            End Select
        End If

        Return True

    End Function

    Private Function PFUNC_GAKINFO_GET() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        sql.Append(" SELECT ")
        sql.Append(" GAKKOU_NNAME_G")
        sql.Append(",SIYOU_GAKUNEN_T")
        sql.Append(",FURI_DATE_T")
        sql.Append(",SFURI_DATE_T")
        sql.Append(",BAITAI_CODE_T")
        sql.Append(",ITAKU_CODE_T")
        sql.Append(",TKIN_NO_T")
        sql.Append(",TSIT_NO_T")
        sql.Append(",SFURI_SYUBETU_T")
        sql.Append(",KAISI_DATE_T")
        sql.Append(",SYURYOU_DATE_T")
        sql.Append(",TESUUTYO_KBN_T")
        sql.Append(",TESUUTYO_KIJITSU_T")
        sql.Append(",TESUUTYO_DAY_T")
        sql.Append(",TESUU_KYU_CODE_T")
        sql.Append(" FROM ")
        sql.Append(" GAKMAST1")
        sql.Append(",GAKMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_G = GAKKOU_CODE_T")
        sql.Append(" AND")
        sql.Append(" GAKUNEN_CODE_G = 1")
        sql.Append(" AND")
        sql.Append(" GAKKOU_CODE_G ='" & Trim(txtGAKKOU_CODE.Text) & "'")


        If oraReader.DataReader(sql) = True Then

            GAKKOU_INFO.GAKKOU_CODE = Trim(txtGAKKOU_CODE.Text)
            GAKKOU_INFO.GAKKOU_NNAME = Trim(oraReader.GetString("GAKKOU_NNAME_G"))

            '使用学年数
            If IsDBNull(oraReader.GetString("SIYOU_GAKUNEN_T")) = False Then
                GAKKOU_INFO.SIYOU_GAKUNEN = CInt(oraReader.GetString("SIYOU_GAKUNEN_T"))
            Else
                GAKKOU_INFO.SIYOU_GAKUNEN = 0
            End If

            '振替日
            If IsDBNull(oraReader.GetString("FURI_DATE_T")) = False Then
                GAKKOU_INFO.FURI_DATE = oraReader.GetString("FURI_DATE_T")
            Else
                GAKKOU_INFO.FURI_DATE = ""
            End If

            '再振日
            If IsDBNull(oraReader.GetString("SFURI_DATE_T")) = False Then
                GAKKOU_INFO.SFURI_DATE = oraReader.GetString("SFURI_DATE_T")
            Else
                GAKKOU_INFO.SFURI_DATE = ""
            End If

            '媒体コード
            If IsDBNull(oraReader.GetString("BAITAI_CODE_T")) = False Then
                GAKKOU_INFO.BAITAI_CODE = oraReader.GetString("BAITAI_CODE_T")
            Else
                GAKKOU_INFO.BAITAI_CODE = ""
            End If

            '委託者コード
            If IsDBNull(oraReader.GetString("ITAKU_CODE_T")) = False Then
                GAKKOU_INFO.ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
            Else
                GAKKOU_INFO.ITAKU_CODE = ""
            End If

            '取扱金融機関コード
            GAKKOU_INFO.TKIN_CODE = oraReader.GetString("TKIN_NO_T")

            '取扱支店コード
            GAKKOU_INFO.TSIT_CODE = oraReader.GetString("TSIT_NO_T")

            '再振種別
            If IsDBNull(oraReader.GetString("SFURI_SYUBETU_T")) = False Then
                GAKKOU_INFO.SFURI_SYUBETU = oraReader.GetString("SFURI_SYUBETU_T")
            Else
                GAKKOU_INFO.SFURI_SYUBETU = ""
            End If

            '自振開始年月
            GAKKOU_INFO.KAISI_DATE = oraReader.GetString("KAISI_DATE_T")

            '自振終了年月
            GAKKOU_INFO.SYURYOU_DATE = oraReader.GetString("SYURYOU_DATE_T")

            '手数料徴求期日区分
            If IsDBNull(oraReader.GetString("TESUUTYO_KIJITSU_T")) = False Then
                GAKKOU_INFO.TESUUTYO_KIJITSU = oraReader.GetString("TESUUTYO_KIJITSU_T")
            Else
                GAKKOU_INFO.TESUUTYO_KIJITSU = ""
            End If

            '手数料徴求日数
            If IsDBNull(oraReader.GetString("TESUUTYO_DAY_T")) = False Then
                GAKKOU_INFO.TESUUTYO_NO = CInt(oraReader.GetString("TESUUTYO_DAY_T"))
            Else
                GAKKOU_INFO.TESUUTYO_NO = 0
            End If

            '手数料徴求区分
            If IsDBNull(oraReader.GetString("TESUUTYO_KBN_T")) = False Then
                GAKKOU_INFO.TESUUTYO_KBN = oraReader.GetString("TESUUTYO_KBN_T")
            Else
                GAKKOU_INFO.TESUUTYO_KBN = ""
            End If

            '決済休日コード
            If IsDBNull(oraReader.GetString("TESUU_KYU_CODE_T")) = False Then
                GAKKOU_INFO.TESUU_KYU_CODE = oraReader.GetString("TESUU_KYU_CODE_T")
            Else
                GAKKOU_INFO.TESUU_KYU_CODE = ""
            End If

        Else

            MessageBox.Show("学校マスタに登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            lab学校名.Text = ""

            oraReader.Close()
            Return False

        End If

        oraReader.Close()

        lab学校名.Text = GAKKOU_INFO.GAKKOU_NNAME

        Return True

    End Function

    Private Function PFUNC_SCH_GET_ALL() As Boolean

        PFUNC_SCH_GET_ALL = False

        '共通入力チェック
        If PFUNC_COMMON_CHECK() = False Then
            Exit Function
        End If

        'スケジュールマスタ存在チェック
        If PFUNC_SCHMAST_SERCH() = False Then
            Call MessageBox.Show("指定した年度の学校スケジュールは存在しませんでした。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        '年間スケジュール参照
        If PFUNC_SCH_GET_NENKAN() = False Then
            ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- START
            MainLOG.Write("年間スケジュール参照", "成功", "（対象件数０件）")
            'MainLOG.Write("年間スケジュール参照", "失敗", "（対象件数０件）")
            ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- END
        Else
            MainLOG.Write("年間スケジュール参照", "成功", "")
        End If

        '特別スケジュール参照
        If PFUNC_SCH_GET_TOKUBETU() = False Then
            ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- START
            MainLOG.Write("特別スケジュール参照", "成功", "（対象件数０件）")
            'MainLOG.Write("特別スケジュール参照", "失敗", "（対象件数０件）")
            ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- END
        Else
            MainLOG.Write("特別スケジュール参照", "成功", "")
        End If

        '随時スケジュール参照
        If PFUNC_SCH_GET_ZUIJI() = False Then
            ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- START
            MainLOG.Write("随時スケジュール参照", "成功", "（対象件数０件）")
            'MainLOG.Write("随時スケジュール参照", "失敗", "（対象件数０件）")
            ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- END
        Else
            MainLOG.Write("随時スケジュール参照", "成功", "")
        End If

        '2006/11/30　各欄の値を構造体に入力
        Call PSUB_NENKAN_GET(SYOKI_NENKAN_SCHINFO) '    年間スケジュール分
        Call PSUB_TOKUBETU_GET(SYOKI_TOKUBETU_SCHINFO) '特別スケジュール分
        Call PSUB_ZUIJI_GET(SYOKI_ZUIJI_SCHINFO) '      随時スケジュール分

        PFUNC_SCH_GET_ALL = True

    End Function

    Private Function PFUNC_SCH_INSERT_ALL() As Boolean

        PFUNC_SCH_INSERT_ALL = False

        Try
            MainDB = New MyOracle

            '共通入力チェック
            If PFUNC_COMMON_CHECK() = False Then
                Exit Function
            End If

            '2006/10/12　初振と再振が同じ日ではないかチェック
            If PFUNC_CHECK_SFURI() = False Then
                Exit Function
            End If

            '2006/11/22　スケジュール同一日チェック
            If PFUNC_CHECK_TOKUBETSU() = False Then
                Exit Function
            End If

            '2006/11/30　同請求月かつ同学年フラグがないかチェック
            If PFUNC_GAKNENFLG_CHECK() = False Then
                Exit Function
            End If

            '2010/10/21 随時の同一スケジュールチェック
            If PFUNC_CHECK_ZUIJI() = False Then
                Exit Function
            End If

            Int_Syori_Flag(0) = 0
            Int_Syori_Flag(1) = 0
            Int_Syori_Flag(2) = 0

            Str_SyoriDate(0) = Format(Now, "yyyyMMdd")
            Str_SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

            MainDB.BeginTrans()

            '年間スケジュール作成
            If PFUNC_NENKAN_SAKUSEI() = False Then
                MainDB.Rollback()
                'ここを通るということは１件でも処理したレコードが存在したということなので
                Int_Syori_Flag(0) = 2
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", "年間スケジュール作成でエラー")
                Return False
            End If

            '特別スケジュール作成
            If PFUNC_TOKUBETU_SAKUSEI("作成") = False Then
                MainDB.Rollback()
                'ここを通るということは１件でも処理したレコードが存在したということなので
                Int_Syori_Flag(1) = 2
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", "特別スケジュール作成でエラー")
                Return False
            End If

            '随時スケジュール作成
            If PFUNC_ZUIJI_SAKUSEI("作成") = False Then
                MainDB.Rollback()
                'ここを通るということは１件でも処理したレコードが存在したということなので
                Int_Syori_Flag(2) = 2
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", "随時スケジュール作成でエラー")
                Return False
            End If

            '不要年間スケジュール削除処理
            If PFUNC_DELETE_GSCHMAST() = False Then
                MainDB.Rollback()
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", "不要年間スケジュール削除でエラー")
                Return False
            End If

            'Select Case True
            '    Case (Int_Syori_Flag(0) = 0 And Int_Syori_Flag(1) = 0 And Int_Syori_Flag(2) = 0)
            '        '処理件数なし
            '        Exit Function
            'End Select

            If Int_Syori_Flag(0) = 1 Then
                '年間スケジュール参照
                If PFUNC_SCH_GET_NENKAN() = False Then
                    ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- START
                    MainLOG.Write("年間スケジュール参照", "成功", "（対象件数０件）")
                    'MainLOG.Write("年間スケジュール参照", "失敗", "（対象件数０件）")
                    ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- END
                Else
                    MainLOG.Write("年間スケジュール参照", "成功", "")
                End If
            End If

            If Int_Syori_Flag(1) = 1 Then
                '特別スケジュール参照
                If PFUNC_SCH_GET_TOKUBETU() = False Then
                    ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- START
                    MainLOG.Write("特別スケジュール参照", "成功", "（対象件数０件）")
                    'MainLOG.Write("特別スケジュール参照", "失敗", "（対象件数０件）")
                    ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- END
                Else
                    MainLOG.Write("特別スケジュール参照", "成功", "")
                End If
            End If

            If Int_Syori_Flag(2) = 1 Then
                '随時スケジュール参照
                If PFUNC_SCH_GET_ZUIJI() = False Then
                    ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- START
                    MainLOG.Write("随時スケジュール参照", "成功", "（対象件数０件）")
                    'MainLOG.Write("随時スケジュール参照", "失敗", "（対象件数０件）")
                    ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- END
                Else
                    MainLOG.Write("随時スケジュール参照", "成功", "")
                End If
            End If

            '2006/11/30　各欄の値を構造体に入力
            Call PSUB_NENKAN_GET(SYOKI_NENKAN_SCHINFO) '    年間スケジュール分
            Call PSUB_TOKUBETU_GET(SYOKI_TOKUBETU_SCHINFO) '特別スケジュール分
            Call PSUB_ZUIJI_GET(SYOKI_ZUIJI_SCHINFO) '      随時スケジュール分

            MainDB.Commit()

            If Int_Syori_Flag(0) <> 2 Then '追加 2005/06/15
                MessageBox.Show("スケジュールが作成されました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            PFUNC_SCH_INSERT_ALL = True

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error) '2010/10/21 例外時のエラーメッセージ追加
            Return False
        Finally
            MainDB.Close()
        End Try

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_ALL() As Boolean

        PFUNC_SCH_DELETE_INSERT_ALL = False

        '共通入力チェック
        If PFUNC_COMMON_CHECK() = False Then
            Exit Function
        End If

        '2006/10/12　初振と再振が同じ日ではないかチェック
        If PFUNC_CHECK_SFURI() = False Then
            Exit Function
        End If

        '2006/11/22　スケジュール同一日チェック
        If PFUNC_CHECK_TOKUBETSU() = False Then
            Exit Function
        End If

        '2006/11/30　同請求月かつ同学年フラグがないかチェック
        If PFUNC_GAKNENFLG_CHECK() = False Then
            Exit Function
        End If

        '2010/10/21 随時の同一スケジュールチェック
        If PFUNC_CHECK_ZUIJI() = False Then
            Exit Function
        End If

        If MessageBox.Show("現在のスケジュールの内容は一新されます", msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> vbOK Then
            Return False
        End If

        Int_Syori_Flag(0) = 0
        Int_Syori_Flag(1) = 0
        Int_Syori_Flag(2) = 0

        Str_SyoriDate(0) = Format(Now, "yyyyMMdd")
        Str_SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

        '2006/11/30　参照時のデータと比べ、更新が必要なデータをチェックする
        Call PSUB_Kousin_Check()

        '年間スケジュール作成
        If PFUNC_SCH_DELETE_INSERT_NENKAN() = False Then
            MainLOG.Write("年間スケジュール更新処理", "失敗", "")
            Exit Function
        Else
            MainLOG.Write("年間スケジュール更新処理", "成功", "")
        End If

        '特別スケジュール作成
        If PFUNC_SCH_DELETE_INSERT_TOKUBETU() = False Then
            MainLOG.Write("特別スケジュール更新処理", "失敗", "")
            Exit Function
        Else
            MainLOG.Write("特別スケジュール更新処理", "成功", "")
        End If

        '随時スケジュール作成
        If PFUNC_SCH_DELETE_INSERT_ZUIJI() = False Then
            MainLOG.Write("随時スケジュール更新処理", "失敗", "")
            Exit Function
        Else
            MainLOG.Write("随時スケジュール更新処理", "成功", "")
        End If

        'Select case True
        '    Case (Int_Syori_Flag(0) = 0 AND Int_Syori_Flag(1) = 0 AND Int_Syori_Flag(2) = 0)
        '        '処理件数なし
        '        Exit Function
        'End Select

        If Int_Syori_Flag(0) = 1 Then
            '年間スケジュール参照
            If PFUNC_SCH_GET_NENKAN() = False Then
                ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- START
                MainLOG.Write("年間スケジュール参照", "成功", "（対象件数０件）")
                'MainLOG.Write("年間スケジュール参照", "失敗", "（対象件数０件）")
                ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- END
            Else
                MainLOG.Write("年間スケジュール参照", "成功", "")
            End If
        End If

        If Int_Syori_Flag(1) = 1 Then
            '特別スケジュール参照
            If PFUNC_SCH_GET_TOKUBETU() = False Then
                ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- START
                MainLOG.Write("特別スケジュール参照", "成功", "（対象件数０件）")
                'MainLOG.Write("特別スケジュール参照", "失敗", "（対象件数０件）")
                ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- END
            Else
                MainLOG.Write("特別スケジュール参照", "成功", "")
            End If
        End If

        If Int_Syori_Flag(2) = 1 Then
            '随時スケジュール参照
            If PFUNC_SCH_GET_ZUIJI() = False Then
                ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- START
                MainLOG.Write("随時スケジュール参照", "成功", "（対象件数０件）")
                'MainLOG.Write("随時スケジュール参照", "失敗", "（対象件数０件）")
                ' 2016/02/08 タスク）岩城 CHG 【IT】UI_B-14-99(RSV2対応(既存バグ修正)) -------------------- END
            Else
                MainLOG.Write("随時スケジュール参照", "成功", "")
            End If
        End If

        '企業自振連携 2006/12/04
        Call sb_KOUSIN_SET()
        If fn_CHECK_CHANGE() = False Then
            Exit Function
        End If

        '2006/11/30　各欄の値を構造体に入力
        Call PSUB_NENKAN_GET(SYOKI_NENKAN_SCHINFO) '    年間スケジュール分
        Call PSUB_TOKUBETU_GET(SYOKI_TOKUBETU_SCHINFO) '特別スケジュール分
        Call PSUB_ZUIJI_GET(SYOKI_ZUIJI_SCHINFO) '      随時スケジュール分

        MessageBox.Show("スケジュールが更新されました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        PFUNC_SCH_DELETE_INSERT_ALL = True

    End Function

    Private Function PFUNC_SEIKYUTUKIHI(ByVal strTUKI As String) As String

        '入力対象年度と有効月をもとに請求年月を計算
        If strTUKI = "01" Or strTUKI = "02" Or strTUKI = "03" Then
            PFUNC_SEIKYUTUKIHI = CStr(CInt(txt対象年度.Text) + 1) & strTUKI
        Else
            PFUNC_SEIKYUTUKIHI = txt対象年度.Text & strTUKI
        End If

    End Function

    Private Function PFUNC_FURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String, ByVal strSCHKUBUN As String, ByVal strFURIKUBUN As String) As String

        '振替日の作成
        PFUNC_FURIHI_MAKE = ""

        Select Case strSCHKUBUN
            Case "0"     '通常
                If strFURIHI = "" Then
                    Select Case strFURIKUBUN
                        Case "0"   '初振
                            PFUNC_FURIHI_MAKE = STR請求年月 & GAKKOU_INFO.FURI_DATE
                        Case "1"   '再振
                            PFUNC_FURIHI_MAKE = STR請求年月 & GAKKOU_INFO.SFURI_DATE
                    End Select
                Else
                    PFUNC_FURIHI_MAKE = STR請求年月 & strFURIHI
                End If
            Case "1"     '特別
                '入力対象年度と入力振替月、日をもとに振替年月日を計算
                If strFURITUKI = "01" Or strFURITUKI = "02" Or strFURITUKI = "03" Then
                    PFUNC_FURIHI_MAKE = CStr(CInt(txt対象年度.Text) + 1) & strFURITUKI & strFURIHI
                Else
                    PFUNC_FURIHI_MAKE = txt対象年度.Text & strFURITUKI & strFURIHI
                End If
            Case "2"     '随時
                PFUNC_FURIHI_MAKE = STR請求年月 & strFURIHI
        End Select

        '営業日算出
        Select Case Int_Zengo_Kbn(1)
            Case 0
                '翌営業日
                PFUNC_FURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_FURIHI_MAKE, "0", "+")
            Case 1
                '前営業日
                PFUNC_FURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_FURIHI_MAKE, "0", "-")
        End Select

    End Function

    Private Function PFUNC_SAIFURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String) As String

        '再振替日の作成
        PFUNC_SAIFURIHI_MAKE = ""

        '再振替日の初期値設定
        PFUNC_SAIFURIHI_MAKE = STRW再振替年 & strFURITUKI & strFURIHI

        '再振種別が「０」、「３」の場合は再振替日の設定は不要
        Select Case GAKKOU_INFO.SFURI_SYUBETU
            Case "0"
                PFUNC_SAIFURIHI_MAKE = "00000000"
            Case "3"
                PFUNC_SAIFURIHI_MAKE = "00000000"
            Case Else
                '営業日算出
                '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------START
                Select Case Sai_Zengo_Kbn
                    'Select Case Int_Zengo_Kbn(1)
                    '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------END
                    Case 0
                        '翌営業日
                        PFUNC_SAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_SAIFURIHI_MAKE, "0", "+")
                    Case 1
                        '前営業日
                        PFUNC_SAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_SAIFURIHI_MAKE, "0", "-")
                End Select
        End Select

    End Function

    Private Function PFUNC_SAISAIFURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String) As String
        '再振レコードの再振替日の作成（次回の初振日）
        Dim str年 As String
        Dim str月 As String

        PFUNC_SAISAIFURIHI_MAKE = ""

        str年 = Mid(STR請求年月, 1, 4)

        If strFURIHI <= GAKKOU_INFO.FURI_DATE Then
            str月 = strFURITUKI
        Else
            If strFURITUKI = "12" Then
                str月 = "01"
                str年 = CStr(CInt(Mid(STR請求年月, 1, 4)) + 1)

            Else
                str月 = Format((CInt(strFURITUKI) + 1), "00")
            End If
        End If

        '営業日算出
        PFUNC_SAISAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(str年 & str月 & GAKKOU_INFO.FURI_DATE, "0", "+")

    End Function

    '2011/06/15 標準版修正 再々振替日算出用関数追加 -------------START
    Private Function PFUNC_SAISAIFURIHI_MAKE(ByVal strFURIDATE As String) As String
        '再振レコードの再振替日の作成（次回の初振日）
        Dim str年 As String
        Dim str月 As String

        PFUNC_SAISAIFURIHI_MAKE = ""

        str年 = strFURIDATE.Substring(0, 4)
        str月 = strFURIDATE.Substring(4, 2)

        '再振日 >= 同一月の初振日となる場合は、来月の初振日を次回の初振日とする
        If strFURIDATE >= str年 & str月 & GAKKOU_INFO.FURI_DATE Then
            If str月 = "12" Then
                str年 = (CInt(str年) + 1).ToString("0000")
                str月 = "01"
            Else
                str月 = (CInt(str月) + 1).ToString("00")
            End If
        End If

        '営業日算出
        Select Case Int_Zengo_Kbn(1)
            Case 0
                '翌営業日
                PFUNC_SAISAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(str年 & str月 & GAKKOU_INFO.FURI_DATE, "0", "+")
            Case 1
                '前営業日
                PFUNC_SAISAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(str年 & str月 & GAKKOU_INFO.FURI_DATE, "0", "-")
        End Select

    End Function
    '2011/06/15 標準版修正 再々振替日算出用関数追加 -------------END
    '2011/06/15 標準版修正 再々振替日算出用関数追加 -------------START
    Private Function PFUNC_KFURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String, ByVal strSCHKUBUN As String, ByVal strFURIKUBUN As String) As String

        '振替日の作成
        PFUNC_KFURIHI_MAKE = ""

        Select Case strSCHKUBUN
            Case "0"     '通常
                If strFURIHI = "" Then
                    Select Case strFURIKUBUN
                        Case "0"   '初振
                            PFUNC_KFURIHI_MAKE = STR請求年月 & GAKKOU_INFO.FURI_DATE
                        Case "1"   '再振
                            '2011/06/15 標準版修正 契約振替日と契約再振日が逆転する場合は翌月の再振日にする -------------START
                            'PFUNC_KFURIHI_MAKE = STR請求年月 & GAKKOU_INFO.SFURI_DATE
                            If GAKKOU_INFO.FURI_DATE < GAKKOU_INFO.SFURI_DATE Then
                                PFUNC_KFURIHI_MAKE = STR請求年月 & GAKKOU_INFO.SFURI_DATE
                            Else
                                If STR請求年月.Substring(4, 2) = "12" Then
                                    PFUNC_KFURIHI_MAKE = (CInt(STR請求年月.Substring(0, 4)) + 1).ToString("0000") & "01" & GAKKOU_INFO.SFURI_DATE
                                Else
                                    PFUNC_KFURIHI_MAKE = (CInt(STR請求年月) + 1).ToString("000000") & GAKKOU_INFO.SFURI_DATE
                                End If
                            End If
                            '2011/06/15 標準版修正 契約振替日と契約再振日が逆転する場合は翌月の再振日にする -------------E
                    End Select
                Else
                    ''入力日付を契約振替日にする場合
                    'PFUNC_KFURIHI_MAKE = STR請求年月 & strFURIHI

                    '実振替日を契約振替日にする場合
                    PFUNC_KFURIHI_MAKE = PFUNC_FURIHI_MAKE(strFURITUKI, strFURIHI, strSCHKUBUN, strFURIKUBUN)
                End If
            Case "1"     '特別
                ''入力日付を契約振替日にする場合
                ''入力対象年度と入力振替月、日をもとに振替年月日を計算
                'If strFURITUKI = "01" Or strFURITUKI = "02" Or strFURITUKI = "03" Then
                '    PFUNC_KFURIHI_MAKE = CStr(CInt(txt対象年度.Text) + 1) & strFURITUKI & strFURIHI
                'Else
                '    PFUNC_KFURIHI_MAKE = txt対象年度.Text & strFURITUKI & strFURIHI
                'End If

                '実振替日を契約振替日にする場合
                PFUNC_KFURIHI_MAKE = PFUNC_FURIHI_MAKE(strFURITUKI, strFURIHI, strSCHKUBUN, strFURIKUBUN)
            Case "2"     '随時
                ''入力日付を契約振替日にする場合
                'PFUNC_KFURIHI_MAKE = STR請求年月 & strFURIHI

                '実振替日を契約振替日にする場合
                PFUNC_KFURIHI_MAKE = PFUNC_FURIHI_MAKE(strFURITUKI, strFURIHI, strSCHKUBUN, strFURIKUBUN)
        End Select

        '月末補正(月末指定の場合実日に変換する)
        Dim strFURINEN As String = PFUNC_KFURIHI_MAKE.Substring(0, 4)
        strFURITUKI = PFUNC_KFURIHI_MAKE.Substring(4, 2)
        strFURIHI = PFUNC_KFURIHI_MAKE.Substring(6, 2)

        Dim intGETUMATU As Integer = Date.DaysInMonth(CInt(strFURINEN), CInt(strFURITUKI))
        If CInt(strFURIHI) > intGETUMATU Then
            PFUNC_KFURIHI_MAKE = strFURINEN & strFURITUKI & intGETUMATU.ToString("00")
        End If

    End Function
    '2011/06/15 標準版修正 再々振替日算出用関数追加 -------------END

    Private Function PFUNC_EIGYOUBI_GET(ByVal str年月日 As String, ByVal str日数 As String, ByVal str前後営業日区分 As String) As String

        '営業日算出
        Dim WORK_DATE As Date
        Dim YOUBI As Long
        Dim HOSEI As Integer

        Dim int日数 As Integer

        PFUNC_EIGYOUBI_GET = ""

        int日数 = CInt(str日数)

        '-------------------------------------
        '月末補正（月末指定の場合実日に変換する）
        '-------------------------------------
        Select Case Mid(str年月日, 5, 2)
            Case "01", "03", "05", "07", "08", "10", "12"
                If Mid(str年月日, 7, 2) < "01" Then
                    Mid(str年月日, 7, 2) = "01"
                End If
                If Mid(str年月日, 7, 2) > "31" Then
                    Mid(str年月日, 7, 2) = "31"
                End If
                WORK_DATE = DateSerial(CInt(Mid(str年月日, 1, 4)), CInt(Mid(str年月日, 5, 2)), CInt(Mid(str年月日, 7, 2)))
            Case "04", "06", "09", "11"
                If Mid(str年月日, 7, 2) < "01" Then
                    Mid(str年月日, 7, 2) = "01"
                End If
                If Mid(str年月日, 7, 2) > "30" Then
                    Mid(str年月日, 7, 2) = "30"
                End If
                WORK_DATE = DateSerial(CInt(Mid(str年月日, 1, 4)), CInt(Mid(str年月日, 5, 2)), CInt(Mid(str年月日, 7, 2)))
            Case "02"
                If Mid(str年月日, 7, 2) < "01" Then
                    Mid(str年月日, 7, 2) = "01"
                End If
                '２月２９日,２月３０日,２月３１日は２月末日指定扱いで２月末日（実日に変換）
                If Mid(str年月日, 7, 2) > "28" Then
                    '１月末の実日で日付型データ変換
                    WORK_DATE = Mid(str年月日, 1, 4) & "/" & "01" & "/" & "31"
                    '２月末の実日を算出
                    WORK_DATE = DateAdd(DateInterval.Month, 1, WORK_DATE)
                Else
                    '２月末日以外の日付変換
                    WORK_DATE = DateSerial(CInt(Mid(str年月日, 1, 4)), CInt(Mid(str年月日, 5, 2)), CInt(Mid(str年月日, 7, 2)))
                End If
        End Select

        '------------
        '０営業日対応
        '------------
        If int日数 = 0 Then
            YOUBI = Weekday(WORK_DATE)

            '曜日判定(Sun = 1:Sat = 7)
            If YOUBI = 1 Or _
               YOUBI = 7 Or _
               PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = False Then
                HOSEI = 1
            Else
                HOSEI = 0
            End If

            Do Until HOSEI = 0
                If str前後営業日区分 = "+" Then
                    WORK_DATE = DateAdd(DateInterval.Day, 1, WORK_DATE)
                End If
                If str前後営業日区分 = "-" Then
                    WORK_DATE = DateAdd(DateInterval.Day, -1, WORK_DATE)
                End If
                YOUBI = Weekday(WORK_DATE)

                '曜日判定(Sun = 1:Sat = 7)
                If (YOUBI <> 1) And (YOUBI <> 7) Then
                    If PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = True Then
                        HOSEI = HOSEI - 1
                    End If
                End If
            Loop
        Else
            '-----------------
            '０営業日以外の処理
            '-----------------
            Do Until int日数 = 0
                If str前後営業日区分 = "+" Then
                    WORK_DATE = DateAdd(DateInterval.Day, 1, WORK_DATE)
                End If
                If str前後営業日区分 = "-" Then
                    WORK_DATE = DateAdd(DateInterval.Day, -1, WORK_DATE)
                End If

                YOUBI = Weekday(WORK_DATE)

                '曜日判定(Sun = 1:Sat = 7)
                If (YOUBI <> 1) And (YOUBI <> 7) Then
                    If PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = True Then
                        int日数 = int日数 - 1
                    End If
                End If
            Loop
        End If

        PFUNC_EIGYOUBI_GET = Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")

    End Function

    Private Function PFUNC_COMMON_YASUMIGET(ByVal str年月日 As String) As Boolean

        '休日マスタ存在チェック
        PFUNC_COMMON_YASUMIGET = False

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        Try

            sql.Append(" SELECT * FROM YASUMIMAST")
            sql.Append(" WHERE")
            sql.Append(" YASUMI_DATE_Y ='" & str年月日 & "'")

            If oraReader.DataReader(sql) = True Then
                Return False
            End If

            PFUNC_COMMON_YASUMIGET = True

        Catch ex As Exception

            Throw

        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

    End Function

    Private Function PFUNC_SCHMAST_GET(ByVal strSCHKBN As String, ByVal strFURIKBN As String, ByVal strFURIHI As String, ByVal strSAIFURIHI As String) As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)
        Dim bret As Boolean = False

        'スケジュールマスタ存在チェック 
        'キーは、学校コード、スケジュール区分、振替区分、振替日,再振替日
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = '" & strSCHKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")
        '2006/11/30　年間スケジュールは再振日をチェックしない
        If strSCHKBN <> "0" Then
            sql.Append(" AND")
            sql.Append(" SFURI_DATE_S ='" & strSAIFURIHI & "'")
        End If

        If oraReader.DataReader(sql) = True Then
            bret = True
        End If
        oraReader.Close()

        Return bret

    End Function

    Private Function PFUNC_SCHMAST_GET_FLG(ByVal strSCHKBN As String, ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        '通常のスケジュールの処理フラグ取得 2006/10/24

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        'スケジュールマスタ存在チェック 
        'キーは、学校コード、スケジュール区分、振替区分、振替日
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = '" & strSCHKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")

        '初期化
        strENTRI_FLG = "0"
        strCHECK_FLG = "0"
        strDATA_FLG = "0"
        strFUNOU_FLG = "0"
        strSAIFURI_FLG = "0"
        strKESSAI_FLG = "0"
        strTYUUDAN_FLG = "0"
        strSAIFURI_DEF = "00000000"

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                strENTRI_FLG = oraReader.GetString("ENTRI_FLG_S")
                strCHECK_FLG = oraReader.GetString("CHECK_FLG_S")
                strDATA_FLG = oraReader.GetString("DATA_FLG_S")
                strFUNOU_FLG = oraReader.GetString("FUNOU_FLG_S")
                strSAIFURI_FLG = oraReader.GetString("SAIFURI_FLG_S")
                strKESSAI_FLG = oraReader.GetString("KESSAI_FLG_S")
                strTYUUDAN_FLG = oraReader.GetString("TYUUDAN_FLG_S")
                strSAIFURI_DEF = oraReader.GetString("SFURI_DATE_S")

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function
    Private Function PFUNC_SCHMAST_GET_FLG_SAI(ByVal strSCHKBN As String, ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        '通常のスケジュールの処理フラグ(再振分)取得 2006/10/24

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        'スケジュールマスタ存在チェック 
        'キーは、学校コード、スケジュール区分、振替区分、振替日
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = '" & strSCHKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")

        '初期化
        strENTRI_FLG_SAI = "0"
        strCHECK_FLG_SAI = "0"
        strDATA_FLG_SAI = "0"
        strFUNOU_FLG_SAI = "0"
        strSAIFURI_FLG_SAI = "0"
        strKESSAI_FLG_SAI = "0"
        strTYUUDAN_FLG_SAI = "0"

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                strENTRI_FLG_SAI = oraReader.GetString("ENTRI_FLG_S")
                strCHECK_FLG_SAI = oraReader.GetString("CHECK_FLG_S")
                strDATA_FLG_SAI = oraReader.GetString("DATA_FLG_S")
                strFUNOU_FLG_SAI = oraReader.GetString("FUNOU_FLG_S")
                strSAIFURI_FLG_SAI = oraReader.GetString("SAIFURI_FLG_S")
                strKESSAI_FLG_SAI = oraReader.GetString("KESSAI_FLG_S")
                strTYUUDAN_FLG_SAI = oraReader.GetString("TYUUDAN_FLG_S")

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function

    Private Function PFUNC_G_MEIMAST_COUNT_MOTO(ByVal strNENGETUDO As String, ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        '既存のスケジュール分の処理結果数を再カウント＆更新
        Dim iGakunen(8) As Integer
        Dim iCount As Integer
        Dim bFlg As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '通常レコードの存在チェック
        PFUNC_G_MEIMAST_COUNT_MOTO = True

        'キーは、学校コード、振替区分、振替日
        sql.Append(" SELECT * FROM G_MEIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_M ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_M = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_M ='" & strFURIHI & "'")

        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            sql.Append(" AND (")

            For iCount = 1 To 9
                If iGakunen(iCount - 1) = 1 Then
                    If bFlg = True Then
                        sql.Append(" OR ")
                    End If

                    sql.Append(" GAKUNEN_CODE_M = " & iCount)
                    bFlg = True
                End If
            Next iCount

            sql.Append(" )")
        End If

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                lngSYORI_KEN = lngSYORI_KEN + 1
                dblSYORI_KIN = dblSYORI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                If oraReader.GetString("FURIKETU_CODE_M") = "0" Then
                    lngFURI_KEN = lngFURI_KEN + 1
                    dblFURI_KIN = dblFURI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                Else
                    lngFUNOU_KEN = lngFUNOU_KEN + 1
                    dblFUNOU_KIN = dblFUNOU_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                End If

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()


        PFUNC_G_MEIMAST_COUNT_MOTO = False
        bFlg = False

        sql = New StringBuilder(128)

        '学年指定がない場合は処理をしない
        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            '特別レコードの対象学年フラグの状態を元に通常レコードの対象学年フラグをＯＦＦにする
            'ＯＮにする機能を持たせた場合、特別レコードが複数件存在した場合に前レコードでの処理が無駄になる
            '特別レコードの対象学年１フラグが「１」の場合
            sql.Append(" UPDATE  G_SCHMAST")
            sql.Append(" SET ")
            sql.Append(" SYORI_KEN_S =" & lngSYORI_KEN & ",")
            sql.Append(" SYORI_KIN_S =" & dblSYORI_KIN & ",")
            sql.Append(" FURI_KEN_S =" & lngFURI_KEN & ",")
            sql.Append(" FURI_KIN_S =" & dblFURI_KIN & ",")
            sql.Append(" FUNOU_KEN_S =" & lngFUNOU_KEN & ",")
            sql.Append(" FUNOU_KIN_S =" & dblFUNOU_KIN)
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
            sql.Append(" AND")
            sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
            sql.Append(" AND")
            sql.Append(" SCH_KBN_S ='0'")
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S ='" & strFURIKBN & "'")
            sql.Append(" AND")
            sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                '更新処理エラー
                MessageBox.Show("スケジュールマスタの更新処理でエラーが発生しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        End If

        Return True

    End Function

    Private Function PFUNC_G_MEIMAST_COUNT(ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        'データフラグ=1の場合は明細マスタから処理件数・金額をカウント
        '不能フラグ=1の場合は明細マスタから振替済み件数・金額、不能件数・金額をカウント
        Dim iGakunen(8) As Integer
        Dim iCount As Integer
        Dim bFlg As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        PFUNC_G_MEIMAST_COUNT = False

        'キーは、学校コード、振替区分、振替日
        sql.Append(" SELECT * FROM G_MEIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_M ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_M = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_M ='" & strFURIHI & "'")

        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            sql.Append(" AND (")

            For iCount = 1 To 9
                If iGakunen(iCount - 1) = 1 Then
                    If bFlg = True Then
                        sql.Append(" OR ")
                    End If

                    sql.Append(" GAKUNEN_CODE_M = " & iCount)
                    bFlg = True
                End If
            Next iCount

            sql.Append(" )")
        End If

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                lngSYORI_KEN = lngSYORI_KEN + 1
                dblSYORI_KIN = dblSYORI_KIN + CDbl(oraReader.GetString("SEIKYU_KIN_M"))
                If oraReader.GetString("FURIKETU_CODE_M") = "0" Then
                    lngFURI_KEN = lngFURI_KEN + 1
                    dblFURI_KIN = dblFURI_KIN + CDbl(oraReader.GetString("SEIKYU_KIN_M"))
                Else
                    lngFUNOU_KEN = lngFUNOU_KEN + 1
                    dblFUNOU_KIN = dblFUNOU_KIN + CDbl(oraReader.GetString("SEIKYU_KIN_M"))
                End If

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function


    Private Function PFUNC_FURIHI_HANI_CHECK() As Boolean

        '振替日、再振替日の契約期間チェック
        PFUNC_FURIHI_HANI_CHECK = False

        ' 2016/05/06 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
        'If Mid(STR振替日, 1, 6) >= GAKKOU_INFO.KAISI_DATE And Mid(STR振替日, 1, 6) <= GAKKOU_INFO.SYURYOU_DATE Then
        'Else
        '    Exit Function
        'End If
        If STR振替日 >= GAKKOU_INFO.KAISI_DATE And STR振替日 <= GAKKOU_INFO.SYURYOU_DATE Then
        Else
            Exit Function
        End If
        ' 2016/05/06 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

        PFUNC_FURIHI_HANI_CHECK = True

    End Function
    Private Function PFUNC_SCHMAST_SERCH() As Boolean

        Dim sql As New StringBuilder(128)
        Dim orareader As New MyOracleReader(MainDB)
        Dim bret As Boolean = False

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & Trim(txtGAKKOU_CODE.Text) & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & Trim(txt対象年度.Text) & "04'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & CStr(CInt(Trim(txt対象年度.Text)) + 1) & "03'")

        If orareader.DataReader(sql) = True Then
            bret = True
        End If

        orareader.Close()

        Return bret

    End Function

    Private Function PFUNC_SCHMAST_UPDATE_SFURIDATE(ByVal pSCH_KBN_S As String) As Boolean

        Dim sql As New StringBuilder(128)

        '処理中の初振日スケジュールにもつ再振日が現状更新できないので
        '再振を作成している時に一緒に更新も行う
        sql.Append(" UPDATE  G_SCHMAST SET ")
        sql.Append(" SFURI_DATE_S ='" & Str_SFURI_DATE & "'")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='" & pSCH_KBN_S & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & Str_FURI_DATE & "'")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            Return False
        End If

        Return True

    End Function

    Private Function PFUNC_DELETE_GSCHMAST() As Boolean

        ' 2017/05/26 タスク）綾部 CHG 【ME】(RSV2対応 年間スケジュールの削除条件不備修正) -------------------- START
        '=========================================================================
        ' 特別スケジュールを作成した場合、同一の年月度の年間スケジュールは
        ' 存在してはならないため、年間スケジュールは年月度単位に削除するよう
        ' 変更する。
        '=========================================================================
        'Dim sql As New StringBuilder(128)

        ''特別スケジュールを作成したことにより
        ''年間で対象学年が存在しないレコードが作成されてしまう為
        ''特別の処理確定後、年間のスケジュールで学年フラグがALLZEROの
        ''レコードを削除する
        'sql.Append(" DELETE  FROM G_SCHMAST")
        'sql.Append(" WHERE")
        'sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        'sql.Append(" AND")
        'sql.Append(" SCH_KBN_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN1_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN2_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN3_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN4_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN5_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN6_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN7_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN8_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN9_FLG_S ='0'")

        'If MainDB.ExecuteNonQuery(sql) < 0 Then
        '    Return False
        'End If

        'Return True
        Dim SQL As New StringBuilder(128)
        Dim SQL_DEL_TOKUSCH As New StringBuilder(128)
        Dim OraReader_Tokubetu As CASTCommon.MyOracleReader = Nothing

        Try

            SQL.Length = 0
            SQL.Append(" SELECT ")
            SQL.Append("     NENGETUDO_S")
            SQL.Append(" FROM ")
            SQL.Append("     G_SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     GAKKOU_CODE_S = '" & GAKKOU_INFO.GAKKOU_CODE & "'")
            SQL.Append(" AND SCH_KBN_S     = '1'")
            SQL.Append(" GROUP BY ")
            SQL.Append("     NENGETUDO_S")
            SQL.Append(" ORDER BY ")
            SQL.Append("     NENGETUDO_S")

            OraReader_Tokubetu = New CASTCommon.MyOracleReader(MainDB)
            If OraReader_Tokubetu.DataReader(SQL) = False Then
                '=================================================================
                ' 特別スケジュールが存在しないため、削除処理不要
                '=================================================================
                Return True
            Else
                '=================================================================
                ' 特別スケジュールが存在するため、削除処理開始
                '=================================================================
                Do Until OraReader_Tokubetu.EOF
                    SQL_DEL_TOKUSCH.Length = 0
                    SQL_DEL_TOKUSCH.Append(" DELETE FROM G_SCHMAST ")
                    SQL_DEL_TOKUSCH.Append(" WHERE")
                    SQL_DEL_TOKUSCH.Append("     GAKKOU_CODE_S = '" & GAKKOU_INFO.GAKKOU_CODE & "'")
                    SQL_DEL_TOKUSCH.Append(" AND NENGETUDO_S   = '" & OraReader_Tokubetu.GetString("NENGETUDO_S") & "'")
                    SQL_DEL_TOKUSCH.Append(" AND SCH_KBN_S     = '0'")

                    If MainDB.ExecuteNonQuery(SQL_DEL_TOKUSCH) < 0 Then
                        Return False
                    Else
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "不要年間スケジュール削除", "成功", "年月度:" & OraReader_Tokubetu.GetString("NENGETUDO_S"))
                    End If

                    OraReader_Tokubetu.NextRead()
                Loop
            End If

            OraReader_Tokubetu.Close()

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "不要年間スケジュール削除", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader_Tokubetu Is Nothing Then
                OraReader_Tokubetu.Close()
                OraReader_Tokubetu = Nothing
            End If
        End Try
        ' 2017/05/26 タスク）綾部 CHG 【ME】(RSV2対応 年間スケジュールの削除条件不備修正) -------------------- END

    End Function

    Private Function PFUNC_CHECK_SFURI() As Boolean
        '2006/10/12　初振と再振が同じ日でないかチェックする

        PFUNC_CHECK_SFURI = False

        '年間スケジュール部分チェック
        If (chk４月振替日.Checked = True And chk４月再振替日.Checked = True And txt４月振替日.Text <> "" And txt４月振替日.Text = txt４月再振替日.Text) Or _
           (chk５月振替日.Checked = True And chk５月再振替日.Checked = True And txt５月振替日.Text <> "" And txt５月振替日.Text = txt５月再振替日.Text) Or _
           (chk６月振替日.Checked = True And chk６月再振替日.Checked = True And txt６月振替日.Text <> "" And txt６月振替日.Text = txt６月再振替日.Text) Or _
           (chk７月振替日.Checked = True And chk７月再振替日.Checked = True And txt７月振替日.Text <> "" And txt７月振替日.Text = txt７月再振替日.Text) Or _
           (chk８月振替日.Checked = True And chk８月再振替日.Checked = True And txt８月振替日.Text <> "" And txt８月振替日.Text = txt８月再振替日.Text) Or _
           (chk９月振替日.Checked = True And chk９月再振替日.Checked = True And txt９月振替日.Text <> "" And txt９月振替日.Text = txt９月再振替日.Text) Or _
           (chk１０月振替日.Checked = True And chk１０月再振替日.Checked = True And txt１０月振替日.Text <> "" And txt１０月振替日.Text = txt１０月再振替日.Text) Or _
           (chk１１月振替日.Checked = True And chk１１月再振替日.Checked = True And txt１１月振替日.Text <> "" And txt１１月振替日.Text = txt１１月再振替日.Text) Or _
           (chk１２月振替日.Checked = True And chk１２月再振替日.Checked = True And txt１２月振替日.Text <> "" And txt１２月振替日.Text = txt１２月再振替日.Text) Or _
           (chk１月振替日.Checked = True And chk１月再振替日.Checked = True And txt１月振替日.Text <> "" And txt１月振替日.Text = txt１月再振替日.Text) Or _
           (chk２月振替日.Checked = True And chk２月再振替日.Checked = True And txt２月振替日.Text <> "" And txt２月振替日.Text = txt２月再振替日.Text) Or _
           (chk３月振替日.Checked = True And chk３月再振替日.Checked = True And txt３月振替日.Text <> "" And txt３月振替日.Text = txt３月再振替日.Text) Then

            MessageBox.Show("振替日と再振替日が同じものがあります", "年間スケジュール", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function

        End If

        '特別振替日部分チェック
        If (txt特別請求月１.Text <> "" And txt特別振替月１.Text & txt特別振替日１.Text <> "" And txt特別振替月１.Text & txt特別振替日１.Text = txt特別再振替月１.Text & txt特別再振替日１.Text) Or _
           (txt特別請求月２.Text <> "" And txt特別振替月２.Text & txt特別振替日２.Text <> "" And txt特別振替月２.Text & txt特別振替日２.Text = txt特別再振替月２.Text & txt特別再振替日２.Text) Or _
           (txt特別請求月３.Text <> "" And txt特別振替月３.Text & txt特別振替日３.Text <> "" And txt特別振替月３.Text & txt特別振替日３.Text = txt特別再振替月３.Text & txt特別再振替日３.Text) Or _
           (txt特別請求月４.Text <> "" And txt特別振替月４.Text & txt特別振替日４.Text <> "" And txt特別振替月４.Text & txt特別振替日４.Text = txt特別再振替月４.Text & txt特別再振替日４.Text) Or _
           (txt特別請求月５.Text <> "" And txt特別振替月５.Text & txt特別振替日５.Text <> "" And txt特別振替月５.Text & txt特別振替日５.Text = txt特別再振替月５.Text & txt特別再振替日５.Text) Or _
           (txt特別請求月６.Text <> "" And txt特別振替月６.Text & txt特別振替日６.Text <> "" And txt特別振替月６.Text & txt特別振替日６.Text = txt特別再振替月６.Text & txt特別再振替日６.Text) Then

            MessageBox.Show("振替日と再振替日が同じものがあります", "特別振替日", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function

        End If

        '2007/02/12KG ；特別振替日で、同一月の初振日と再振日が同一の場合ERRとみなす。
        '****************************************************************************
        If (txt特別請求月１.Text <> "" And txt特別振替月２.Text <> "") And ((txt特別請求月１.Text = txt特別振替月２.Text) And (txt特別振替月１.Text & txt特別振替日１.Text = txt特別再振替月２.Text & txt特別再振替日２.Text) Or (txt特別振替月２.Text & txt特別振替日２.Text = txt特別再振替月１.Text & txt特別再振替日１.Text)) Or _
            (txt特別請求月１.Text <> "" And txt特別振替月３.Text <> "") And ((txt特別請求月１.Text = txt特別振替月３.Text) And (txt特別振替月１.Text & txt特別振替日１.Text = txt特別再振替月３.Text & txt特別再振替日３.Text) Or (txt特別振替月３.Text & txt特別振替日３.Text = txt特別再振替月１.Text & txt特別再振替日１.Text)) Or _
            (txt特別請求月１.Text <> "" And txt特別振替月４.Text <> "") And ((txt特別請求月１.Text = txt特別振替月４.Text) And (txt特別振替月１.Text & txt特別振替日１.Text = txt特別再振替月４.Text & txt特別再振替日４.Text) Or (txt特別振替月４.Text & txt特別振替日４.Text = txt特別再振替月１.Text & txt特別再振替日１.Text)) Or _
            (txt特別請求月１.Text <> "" And txt特別振替月５.Text <> "") And ((txt特別請求月１.Text = txt特別振替月５.Text) And (txt特別振替月１.Text & txt特別振替日１.Text = txt特別再振替月５.Text & txt特別再振替日５.Text) Or (txt特別振替月５.Text & txt特別振替日５.Text = txt特別再振替月１.Text & txt特別再振替日１.Text)) Or _
           (txt特別請求月１.Text <> "" And txt特別振替月６.Text <> "") And ((txt特別請求月１.Text = txt特別振替月６.Text) And (txt特別振替月１.Text & txt特別振替日１.Text = txt特別再振替月６.Text & txt特別再振替日６.Text) Or (txt特別振替月６.Text & txt特別振替日６.Text = txt特別再振替月１.Text & txt特別再振替日１.Text)) Or _
            (txt特別請求月２.Text <> "" And txt特別振替月３.Text <> "") And ((txt特別請求月２.Text = txt特別振替月３.Text) And (txt特別振替月２.Text & txt特別振替日２.Text = txt特別再振替月３.Text & txt特別再振替日３.Text) Or (txt特別振替月３.Text & txt特別振替日３.Text = txt特別再振替月２.Text & txt特別再振替日２.Text)) Or _
            (txt特別請求月２.Text <> "" And txt特別振替月４.Text <> "") And ((txt特別請求月２.Text = txt特別振替月４.Text) And (txt特別振替月２.Text & txt特別振替日２.Text = txt特別再振替月４.Text & txt特別再振替日４.Text) Or (txt特別振替月４.Text & txt特別振替日４.Text = txt特別再振替月２.Text & txt特別再振替日２.Text)) Or _
            (txt特別請求月２.Text <> "" And txt特別振替月５.Text <> "") And ((txt特別請求月２.Text = txt特別振替月５.Text) And (txt特別振替月２.Text & txt特別振替日２.Text = txt特別再振替月５.Text & txt特別再振替日５.Text) Or (txt特別振替月５.Text & txt特別振替日５.Text = txt特別再振替月２.Text & txt特別再振替日２.Text)) Or _
            (txt特別請求月２.Text <> "" And txt特別振替月６.Text <> "") And ((txt特別請求月２.Text = txt特別振替月６.Text) And (txt特別振替月２.Text & txt特別振替日２.Text = txt特別再振替月６.Text & txt特別再振替日６.Text) Or (txt特別振替月６.Text & txt特別振替日６.Text = txt特別再振替月２.Text & txt特別再振替日２.Text)) Or _
            (txt特別請求月３.Text <> "" And txt特別振替月４.Text <> "") And ((txt特別請求月３.Text = txt特別振替月４.Text) And (txt特別振替月３.Text & txt特別振替日３.Text = txt特別再振替月４.Text & txt特別再振替日４.Text) Or (txt特別振替月４.Text & txt特別振替日４.Text = txt特別再振替月３.Text & txt特別再振替日３.Text)) Or _
            (txt特別請求月３.Text <> "" And txt特別振替月５.Text <> "") And ((txt特別請求月３.Text = txt特別振替月５.Text) And (txt特別振替月３.Text & txt特別振替日３.Text = txt特別再振替月５.Text & txt特別再振替日５.Text) Or (txt特別振替月５.Text & txt特別振替日５.Text = txt特別再振替月３.Text & txt特別再振替日３.Text)) Or _
            (txt特別請求月３.Text <> "" And txt特別振替月６.Text <> "") And ((txt特別請求月３.Text = txt特別振替月６.Text) And (txt特別振替月３.Text & txt特別振替日３.Text = txt特別再振替月６.Text & txt特別再振替日６.Text) Or (txt特別振替月６.Text & txt特別振替日６.Text = txt特別再振替月３.Text & txt特別再振替日３.Text)) Or _
            (txt特別請求月４.Text <> "" And txt特別振替月５.Text <> "") And ((txt特別請求月４.Text = txt特別振替月４.Text) And (txt特別振替月４.Text & txt特別振替日４.Text = txt特別再振替月５.Text & txt特別再振替日５.Text) Or (txt特別振替月５.Text & txt特別振替日５.Text = txt特別再振替月４.Text & txt特別再振替日４.Text)) Or _
            (txt特別請求月４.Text <> "" And txt特別振替月６.Text <> "") And ((txt特別請求月４.Text = txt特別振替月４.Text) And (txt特別振替月４.Text & txt特別振替日４.Text = txt特別再振替月６.Text & txt特別再振替日６.Text) Or (txt特別振替月６.Text & txt特別振替日６.Text = txt特別再振替月４.Text & txt特別再振替日４.Text)) Or _
            (txt特別請求月５.Text <> "" And txt特別振替月６.Text <> "") And ((txt特別請求月５.Text = txt特別振替月５.Text) And (txt特別振替月５.Text & txt特別振替日５.Text = txt特別再振替月６.Text & txt特別再振替日６.Text) Or (txt特別振替月６.Text & txt特別振替日６.Text = txt特別再振替月５.Text & txt特別再振替日５.Text)) Then


            MessageBox.Show("同一月で｢初振日｣と｢再振日｣が重複しています。", "特別振替日", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function

        End If
        '****************************************************************************

        PFUNC_CHECK_SFURI = True

    End Function

    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    '特別スケジュールチェック 2006/11/22
    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    Private Function PFUNC_CHECK_TOKUBETSU() As Boolean
        PFUNC_CHECK_TOKUBETSU = False

        '------------------------------------------
        '同一振替日の登録はできない
        '------------------------------------------
        Dim blnCHECK(13) As Boolean ' 振替実行チェック
        Dim blnSCHECK(13) As Boolean '再振実行チェック
        Dim strNyuuryoku(13) As String ' 振替日欄に入力された値
        Dim strSNyuuryoku(13) As String '再振日欄に入力された値
        Dim strTsuujyou(13) As String '通常スケジュール
        Dim strTokubetsu(6) As String '特別スケジュール

        '営業日を取得し、請求月・初振・再振を１つの文字列に結合
        '■通常スケジュール分（strTsuujyou）
        PFUNC_SET_EIGYOBI(chk４月振替日.Checked, "04", Trim(txt対象年度.Text), "04", Trim(txt４月振替日.Text), chk４月再振替日.Checked, Trim(txt対象年度.Text), "04", Trim(txt４月再振替日.Text), True, strTsuujyou(4))
        PFUNC_SET_EIGYOBI(chk５月振替日.Checked, "05", Trim(txt対象年度.Text), "05", Trim(txt５月振替日.Text), chk５月再振替日.Checked, Trim(txt対象年度.Text), "05", Trim(txt５月再振替日.Text), True, strTsuujyou(5))
        PFUNC_SET_EIGYOBI(chk６月振替日.Checked, "06", Trim(txt対象年度.Text), "06", Trim(txt６月振替日.Text), chk６月再振替日.Checked, Trim(txt対象年度.Text), "06", Trim(txt６月再振替日.Text), True, strTsuujyou(6))
        PFUNC_SET_EIGYOBI(chk７月振替日.Checked, "07", Trim(txt対象年度.Text), "07", Trim(txt７月振替日.Text), chk７月再振替日.Checked, Trim(txt対象年度.Text), "07", Trim(txt７月再振替日.Text), True, strTsuujyou(7))
        PFUNC_SET_EIGYOBI(chk８月振替日.Checked, "08", Trim(txt対象年度.Text), "08", Trim(txt８月振替日.Text), chk８月再振替日.Checked, Trim(txt対象年度.Text), "08", Trim(txt８月再振替日.Text), True, strTsuujyou(8))
        PFUNC_SET_EIGYOBI(chk９月振替日.Checked, "09", Trim(txt対象年度.Text), "09", Trim(txt９月振替日.Text), chk９月再振替日.Checked, Trim(txt対象年度.Text), "09", Trim(txt９月再振替日.Text), True, strTsuujyou(9))
        PFUNC_SET_EIGYOBI(chk１０月振替日.Checked, "10", Trim(txt対象年度.Text), "10", Trim(txt１０月振替日.Text), chk１０月再振替日.Checked, Trim(txt対象年度.Text), "10", Trim(txt１０月再振替日.Text), True, strTsuujyou(10))
        PFUNC_SET_EIGYOBI(chk１１月振替日.Checked, "11", Trim(txt対象年度.Text), "11", Trim(txt１１月振替日.Text), chk１１月再振替日.Checked, Trim(txt対象年度.Text), "11", Trim(txt１１月再振替日.Text), True, strTsuujyou(11))
        PFUNC_SET_EIGYOBI(chk１２月振替日.Checked, "12", Trim(txt対象年度.Text), "12", Trim(txt１２月振替日.Text), chk１２月再振替日.Checked, Trim(txt対象年度.Text), "12", Trim(txt１２月再振替日.Text), True, strTsuujyou(12))
        PFUNC_SET_EIGYOBI(chk１月振替日.Checked, "01", Trim(txt対象年度.Text), "01", Trim(txt１月振替日.Text), chk１月再振替日.Checked, Trim(txt対象年度.Text), "01", Trim(txt１月再振替日.Text), True, strTsuujyou(1))
        PFUNC_SET_EIGYOBI(chk２月振替日.Checked, "02", Trim(txt対象年度.Text), "02", Trim(txt２月振替日.Text), chk２月再振替日.Checked, Trim(txt対象年度.Text), "02", Trim(txt２月再振替日.Text), True, strTsuujyou(2))
        PFUNC_SET_EIGYOBI(chk３月振替日.Checked, "03", Trim(txt対象年度.Text), "03", Trim(txt３月振替日.Text), chk３月再振替日.Checked, Trim(txt対象年度.Text), "03", Trim(txt３月再振替日.Text), True, strTsuujyou(3))

        '■特別スケジュール分（strTokubetsu）
        PFUNC_SET_EIGYOBI(True, Trim(txt特別請求月１.Text), Trim(txt対象年度.Text), Trim(txt特別振替月１.Text), Trim(txt特別振替日１.Text), True, Trim(txt対象年度.Text), Trim(txt特別再振替月１.Text), Trim(txt特別再振替日１.Text), False, strTokubetsu(0))
        PFUNC_SET_EIGYOBI(True, Trim(txt特別請求月２.Text), Trim(txt対象年度.Text), Trim(txt特別振替月２.Text), Trim(txt特別振替日２.Text), True, Trim(txt対象年度.Text), Trim(txt特別再振替月２.Text), Trim(txt特別再振替日２.Text), False, strTokubetsu(1))
        PFUNC_SET_EIGYOBI(True, Trim(txt特別請求月３.Text), Trim(txt対象年度.Text), Trim(txt特別振替月３.Text), Trim(txt特別振替日３.Text), True, Trim(txt対象年度.Text), Trim(txt特別再振替月３.Text), Trim(txt特別再振替日３.Text), False, strTokubetsu(2))
        PFUNC_SET_EIGYOBI(True, Trim(txt特別請求月４.Text), Trim(txt対象年度.Text), Trim(txt特別振替月４.Text), Trim(txt特別振替日４.Text), True, Trim(txt対象年度.Text), Trim(txt特別再振替月４.Text), Trim(txt特別再振替日４.Text), False, strTokubetsu(3))
        PFUNC_SET_EIGYOBI(True, Trim(txt特別請求月５.Text), Trim(txt対象年度.Text), Trim(txt特別振替月５.Text), Trim(txt特別振替日５.Text), True, Trim(txt対象年度.Text), Trim(txt特別再振替月５.Text), Trim(txt特別再振替日５.Text), False, strTokubetsu(4))
        PFUNC_SET_EIGYOBI(True, Trim(txt特別請求月６.Text), Trim(txt対象年度.Text), Trim(txt特別振替月６.Text), Trim(txt特別振替日６.Text), True, Trim(txt対象年度.Text), Trim(txt特別再振替月６.Text), Trim(txt特別再振替日６.Text), False, strTokubetsu(5))

        '通常スケジュールと特別スケジュールのチェック
        For i As Integer = 0 To 5
            If Trim(strTokubetsu(i)) <> "" Then '未入力の場合、チェックの必要なし
                '※strTokubetsu(i).Substring(0, 2)は請求月
                '2010/10/21 strTsuujyouには振替日＋再振日が入っている場合があるので考慮する
                'If strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))) = strTokubetsu(i) Then
                If strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))) IsNot Nothing AndAlso _
                   strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))).PadRight(16).Substring(0, 8) = strTokubetsu(i).Substring(0, 8) Then
                    MessageBox.Show("通常スケジュールと特別スケジュールに同一振替日のデータが存在します", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If

                '2010/10/21 再振もチェックする ここから
                If strTokubetsu(i).Length = 16 Then
                    If strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))) IsNot Nothing AndAlso _
                       strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))).PadRight(16).Substring(8, 8) = strTokubetsu(i).Substring(8, 8) Then
                        MessageBox.Show("通常スケジュールと特別スケジュールに同一再振替日のデータが存在します", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                    End If
                End If
                '2010/10/21 再振もチェックする ここまで
            End If
        Next

        '特別スケジュール同士のチェック
        For i As Integer = 0 To 4
            If strTokubetsu(i) <> "" Then '未入力の場合、チェックの必要なし
                For j As Integer = i + 1 To 5
                    If strTokubetsu(i) = strTokubetsu(j) Then
                        MessageBox.Show("特別スケジュールに同一振替日のデータが存在します。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                    End If
                Next
            End If
        Next

        PFUNC_CHECK_TOKUBETSU = True

    End Function

    '2010/10/21
    '随時スケジュールチェック
    Private Function PFUNC_CHECK_ZUIJI() As Boolean
        PFUNC_CHECK_ZUIJI = False

        '------------------------------------------
        '同一入出区分、同一振替日の登録はできない
        '------------------------------------------
        Dim strZuiji(6) As String '随時スケジュール
        Dim intNsKbn(6) As Integer
        intNsKbn(0) = cmb入出区分１.SelectedIndex
        intNsKbn(1) = cmb入出区分２.SelectedIndex
        intNsKbn(2) = cmb入出区分３.SelectedIndex
        intNsKbn(3) = cmb入出区分４.SelectedIndex
        intNsKbn(4) = cmb入出区分５.SelectedIndex
        intNsKbn(5) = cmb入出区分６.SelectedIndex

        '営業日を取得
        PFUNC_SET_EIGYOBI(True, txt随時振替月１.Text.Trim, txt対象年度.Text.Trim, txt随時振替月１.Text.Trim, txt随時振替日１.Text.Trim, True, txt対象年度.Text.Trim, txt随時振替月１.Text.Trim, txt随時振替日１.Text.Trim, False, strZuiji(0))
        PFUNC_SET_EIGYOBI(True, txt随時振替月２.Text.Trim, txt対象年度.Text.Trim, txt随時振替月２.Text.Trim, txt随時振替日２.Text.Trim, True, txt対象年度.Text.Trim, txt随時振替月２.Text.Trim, txt随時振替日２.Text.Trim, False, strZuiji(1))
        PFUNC_SET_EIGYOBI(True, txt随時振替月３.Text.Trim, txt対象年度.Text.Trim, txt随時振替月３.Text.Trim, txt随時振替日３.Text.Trim, True, txt対象年度.Text.Trim, txt随時振替月３.Text.Trim, txt随時振替日３.Text.Trim, False, strZuiji(2))
        PFUNC_SET_EIGYOBI(True, txt随時振替月４.Text.Trim, txt対象年度.Text.Trim, txt随時振替月４.Text.Trim, txt随時振替日４.Text.Trim, True, txt対象年度.Text.Trim, txt随時振替月４.Text.Trim, txt随時振替日４.Text.Trim, False, strZuiji(3))
        PFUNC_SET_EIGYOBI(True, txt随時振替月５.Text.Trim, txt対象年度.Text.Trim, txt随時振替月５.Text.Trim, txt随時振替日５.Text.Trim, True, txt対象年度.Text.Trim, txt随時振替月５.Text.Trim, txt随時振替日５.Text.Trim, False, strZuiji(4))
        PFUNC_SET_EIGYOBI(True, txt随時振替月６.Text.Trim, txt対象年度.Text.Trim, txt随時振替月６.Text.Trim, txt随時振替日６.Text.Trim, True, txt対象年度.Text.Trim, txt随時振替月６.Text.Trim, txt随時振替日６.Text.Trim, False, strZuiji(5))

        '随時スケジュール同士のチェック
        For i As Integer = 0 To 4
            If strZuiji(i) <> "" Then '未入力の場合、チェックの必要なし
                For j As Integer = i + 1 To 5
                    If intNsKbn(i) = intNsKbn(j) AndAlso strZuiji(i) = strZuiji(j) Then
                        MessageBox.Show("随時スケジュールに同一入出区分、同一振替日のデータが存在します。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                    End If
                Next
            End If
        Next

        PFUNC_CHECK_ZUIJI = True

    End Function

    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    '営業日取得 2006/11/22
    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    Function PFUNC_SET_EIGYOBI(ByVal blnBOX As Boolean, ByVal strSeikyuTuki As String, ByVal strFuridateY As String, ByVal strFuridateM As String, ByVal strFuridateD As String, ByVal blnSBOX As Boolean, ByVal strSFuridateY As String, ByVal strSFuridateM As String, ByVal strSFuridateD As String, ByVal blnCheckFLG As Boolean, ByRef strReturnDate As String) As Boolean
        Dim strEigyobiY As String = "" '振替営業年
        Dim strEigyobiM As String = "" '振替営業月
        Dim strEigyobiD As String = "" '振替営業日
        Dim strSEigyobiY As String = "" '再振営業年
        Dim strSEigyobiM As String = "" '再振営業月
        Dim strSEigyobiD As String = "" '再振営業日

        '請求月が空白の場合・振替しない場合、取得する必要なし
        If strSeikyuTuki = "" Or blnBOX = False Then
            Exit Function
        End If

        '請求月が１〜３月の場合は年度を変える
        If CInt(strSeikyuTuki) <= 3 Then
            strFuridateY = CStr(CInt(strFuridateY + 1))
            strSFuridateY = CStr(CInt(strSFuridateY + 1))
        End If

        '日付が空白だった場合、基準日を使用する
        If blnCheckFLG = True Then
            If strFuridateD = "" Then
                strFuridateD = GAKKOU_INFO.FURI_DATE
            End If

            If blnSBOX = True And strSFuridateD = "" Then
                strSFuridateD = GAKKOU_INFO.SFURI_DATE
            End If
        End If

        '営業日を取得
        Dim FuriDate As String = fn_GetEigyoubi(strFuridateY & strFuridateM & strFuridateD, "0", "+")
        Dim SFuriDate As String = fn_GetEigyoubi(strSFuridateY & strSFuridateM & strSFuridateD, "0", "+")

        'START 20121114 maeda 修正 再振替日が未入力時の考慮を追加
        '2011/06/15 標準版修正 契約振替日と契約再振日が逆転する場合は翌月の再振日にする -------------START
        If SFuriDate <> "" Then
            If FuriDate >= SFuriDate Then
                If strSFuridateM = "12" Then
                    strSFuridateY = (CInt(strSFuridateY) + 1).ToString("0000")
                    strSFuridateM = "01"
                Else
                    strSFuridateM = (CInt(strSFuridateM) + 1).ToString("00")
                End If
                SFuriDate = fn_GetEigyoubi(strSFuridateY & strSFuridateM & strSFuridateD, "0", "+")
            End If
        End If
        '2011/06/15 標準版修正 契約振替日と契約再振日が逆転する場合は翌月の再振日にする -------------END
        'END   20121114 maeda 修正 再振替日が未入力時の考慮を追加

        '再振スケジュール（通常スケジュールと結合し、１つの変数として返す）
        strReturnDate = FuriDate & SFuriDate

    End Function

    '企業自振連携向け 2006/12/06
    Public Function fn_CHECK_CHANGE() As Boolean
        '================================================================
        '退避した参照時のスケジュールが更新後の変数に残っているかチェック
        '更新後に残っていない場合=削除されたので企業自振側のスケジュールも削除
        '================================================================

        fn_CHECK_CHANGE = False

        '年間スケジュール更新
        For i As Integer = 1 To 12
            '初振年間チェック
            If strSYOFURI_NENKAN(i).Length = 8 And strSYOFURI_NENKAN(i) <> strSYOFURI_NENKAN_AFTER(i) Then

                For j As Integer = 1 To 6
                    '特別振替日と一致する振替日がある場合、削除しないのでループを抜ける
                    If strSYOFURI_NENKAN(i) = strSYOFURI_TOKUBETU_AFTER(j) And strSYOFURI_TOKUBETU_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 6 Then '特別振替日ﾁｪｯｸ終了
                        If fn_DELETESCHMAST("01", strSYOFURI_NENKAN(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next

            End If
            '再振年間チェック
            If strSAIFURI_NENKAN(i).Length = 8 And strSAIFURI_NENKAN(i) <> strSAIFURI_NENKAN_AFTER(i) Then
                For j As Integer = 1 To 6
                    '特別振替日と一致する振替日がある場合、削除しないのでループを抜ける
                    If strSAIFURI_NENKAN(i) = strSAIFURI_TOKUBETU_AFTER(j) And strSAIFURI_TOKUBETU_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 6 Then '特別振替日ﾁｪｯｸ終了
                        If fn_DELETESCHMAST("02", strSAIFURI_NENKAN(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next

            End If
        Next

        '特別更新
        For i As Integer = 1 To 6
            '初振特別チェック
            If strSYOFURI_TOKUBETU(i).Length = 8 And strSYOFURI_TOKUBETU(i) <> strSYOFURI_TOKUBETU_AFTER(i) Then
                For j As Integer = 1 To 12
                    '年間振替日と一致する振替日がある場合、削除しないのでループを抜ける
                    If strSYOFURI_TOKUBETU(i) = strSYOFURI_NENKAN_AFTER(j) And strSYOFURI_NENKAN_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 12 Then '年間振替日ﾁｪｯｸ終了
                        If fn_DELETESCHMAST("01", strSYOFURI_TOKUBETU(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next
            End If
            '再振特別チェック
            If strSAIFURI_TOKUBETU(i).Length = 8 And strSAIFURI_TOKUBETU(i) <> strSAIFURI_TOKUBETU_AFTER(i) Then
                For j As Integer = 1 To 12
                    '年間振替日と一致する振替日がある場合、削除しないのでループを抜ける
                    If strSAIFURI_TOKUBETU(i) = strSAIFURI_NENKAN_AFTER(j) And strSAIFURI_NENKAN_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 12 Then '年間振替日ﾁｪｯｸ終了
                        If fn_DELETESCHMAST("02", strSAIFURI_TOKUBETU(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next
            End If
        Next

        '随時更新
        For i As Integer = 1 To 6
            If strFURI_ZUIJI(i).Length = 8 And strFURIKBN_ZUIJI(i) & strFURI_ZUIJI(i) <> strFURIKBN_ZUIJI_AFTER(i) & strFURI_ZUIJI_AFTER(i) Then
                For j As Integer = 1 To 6
                    If strFURIKBN_ZUIJI(i) & strFURI_ZUIJI(i) = strFURIKBN_ZUIJI_AFTER(j) & strFURI_ZUIJI_AFTER(j) And strFURI_ZUIJI_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 6 Then
                        If strFURIKBN_ZUIJI(i) = "2" Then '入金
                            If fn_DELETESCHMAST("03", strFURI_ZUIJI(i)) = False Then
                                Exit Function
                            End If
                        Else '出金
                            If fn_DELETESCHMAST("04", strFURI_ZUIJI(i)) = False Then
                                Exit Function
                            End If
                        End If
                    End If

                Next
            End If
        Next

        If Err.Number = 0 Then
            fn_CHECK_CHANGE = True
        End If
    End Function

#End Region

#Region " Private Sub(年間スケジュール)"
    Private Sub PSUB_NENKAN_GET(ByRef Get_Data() As NenkanData)

        Get_Data(4).Furikae_Check = chk４月振替日.Checked
        Get_Data(4).Furikae_Enabled = chk４月振替日.Enabled
        Get_Data(4).Furikae_Date = txt４月振替日.Text
        Get_Data(4).Furikae_Day = lab４月振替日.Text

        Get_Data(4).SaiFurikae_Check = chk４月再振替日.Checked
        Get_Data(4).SaiFurikae_Enabled = chk４月再振替日.Enabled
        Get_Data(4).SaiFurikae_Date = txt４月再振替日.Text
        Get_Data(4).SaiFurikae_Day = lab４月再振替日.Text

        Get_Data(5).Furikae_Check = chk５月振替日.Checked
        Get_Data(5).Furikae_Enabled = chk５月振替日.Enabled
        Get_Data(5).Furikae_Date = txt５月振替日.Text
        Get_Data(5).Furikae_Day = lab５月振替日.Text

        Get_Data(5).SaiFurikae_Check = chk５月再振替日.Checked
        Get_Data(5).SaiFurikae_Enabled = chk５月再振替日.Enabled
        Get_Data(5).SaiFurikae_Date = txt５月再振替日.Text
        Get_Data(5).SaiFurikae_Day = lab５月再振替日.Text

        Get_Data(6).Furikae_Check = chk６月振替日.Checked
        Get_Data(6).Furikae_Enabled = chk６月振替日.Enabled
        Get_Data(6).Furikae_Date = txt６月振替日.Text
        Get_Data(6).Furikae_Day = lab６月振替日.Text

        Get_Data(6).SaiFurikae_Check = chk６月再振替日.Checked
        Get_Data(6).SaiFurikae_Enabled = chk６月再振替日.Enabled
        Get_Data(6).SaiFurikae_Date = txt６月再振替日.Text
        Get_Data(6).SaiFurikae_Day = lab６月再振替日.Text

        Get_Data(7).Furikae_Check = chk７月振替日.Checked
        Get_Data(7).Furikae_Enabled = chk７月振替日.Enabled
        Get_Data(7).Furikae_Date = txt７月振替日.Text
        Get_Data(7).Furikae_Day = lab７月振替日.Text

        Get_Data(7).SaiFurikae_Check = chk７月再振替日.Checked
        Get_Data(7).SaiFurikae_Enabled = chk７月再振替日.Enabled
        Get_Data(7).SaiFurikae_Date = txt７月再振替日.Text
        Get_Data(7).SaiFurikae_Day = lab７月再振替日.Text

        Get_Data(8).Furikae_Check = chk８月振替日.Checked
        Get_Data(8).Furikae_Enabled = chk８月振替日.Enabled
        Get_Data(8).Furikae_Date = txt８月振替日.Text
        Get_Data(8).Furikae_Day = lab８月振替日.Text

        Get_Data(8).SaiFurikae_Check = chk８月再振替日.Checked
        Get_Data(8).SaiFurikae_Enabled = chk８月再振替日.Enabled
        Get_Data(8).SaiFurikae_Date = txt８月再振替日.Text
        Get_Data(8).SaiFurikae_Day = lab８月再振替日.Text

        Get_Data(9).Furikae_Check = chk９月振替日.Checked
        Get_Data(9).Furikae_Enabled = chk９月振替日.Enabled
        Get_Data(9).Furikae_Date = txt９月振替日.Text
        Get_Data(9).Furikae_Day = lab９月振替日.Text

        Get_Data(9).SaiFurikae_Check = chk９月再振替日.Checked
        Get_Data(9).SaiFurikae_Enabled = chk９月再振替日.Enabled
        Get_Data(9).SaiFurikae_Date = txt９月再振替日.Text
        Get_Data(9).SaiFurikae_Day = lab９月再振替日.Text

        Get_Data(10).Furikae_Check = chk１０月振替日.Checked
        Get_Data(10).Furikae_Enabled = chk１０月振替日.Enabled
        Get_Data(10).Furikae_Date = txt１０月振替日.Text
        Get_Data(10).Furikae_Day = lab１０月振替日.Text

        Get_Data(10).SaiFurikae_Check = chk１０月再振替日.Checked
        Get_Data(10).SaiFurikae_Enabled = chk１０月再振替日.Enabled
        Get_Data(10).SaiFurikae_Date = txt１０月再振替日.Text
        Get_Data(10).SaiFurikae_Day = lab１０月再振替日.Text

        Get_Data(11).Furikae_Check = chk１１月振替日.Checked
        Get_Data(11).Furikae_Enabled = chk１１月振替日.Enabled
        Get_Data(11).Furikae_Date = txt１１月振替日.Text
        Get_Data(11).Furikae_Day = lab１１月振替日.Text

        Get_Data(11).SaiFurikae_Check = chk１１月再振替日.Checked
        Get_Data(11).SaiFurikae_Enabled = chk１１月再振替日.Enabled
        Get_Data(11).SaiFurikae_Date = txt１１月再振替日.Text
        Get_Data(11).SaiFurikae_Day = lab１１月再振替日.Text

        Get_Data(12).Furikae_Check = chk１２月振替日.Checked
        Get_Data(12).Furikae_Enabled = chk１２月振替日.Enabled
        Get_Data(12).Furikae_Date = txt１２月振替日.Text
        Get_Data(12).Furikae_Day = lab１２月振替日.Text

        Get_Data(12).SaiFurikae_Check = chk１２月再振替日.Checked
        Get_Data(12).SaiFurikae_Enabled = chk１２月再振替日.Enabled
        Get_Data(12).SaiFurikae_Date = txt１２月再振替日.Text
        Get_Data(12).SaiFurikae_Day = lab１２月再振替日.Text

        Get_Data(1).Furikae_Check = chk１月振替日.Checked
        Get_Data(1).Furikae_Enabled = chk１月振替日.Enabled
        Get_Data(1).Furikae_Date = txt１月振替日.Text
        Get_Data(1).Furikae_Day = lab１月振替日.Text

        Get_Data(1).SaiFurikae_Check = chk１月再振替日.Checked
        Get_Data(1).SaiFurikae_Enabled = chk１月再振替日.Enabled
        Get_Data(1).SaiFurikae_Date = txt１月再振替日.Text
        Get_Data(1).SaiFurikae_Day = lab１月再振替日.Text

        Get_Data(2).Furikae_Check = chk２月振替日.Checked
        Get_Data(2).Furikae_Enabled = chk２月振替日.Enabled
        Get_Data(2).Furikae_Date = txt２月振替日.Text
        Get_Data(2).Furikae_Day = lab２月振替日.Text

        Get_Data(2).SaiFurikae_Check = chk２月再振替日.Checked
        Get_Data(2).SaiFurikae_Enabled = chk２月再振替日.Enabled
        Get_Data(2).SaiFurikae_Date = txt２月再振替日.Text
        Get_Data(2).SaiFurikae_Day = lab２月再振替日.Text

        Get_Data(3).Furikae_Check = chk３月振替日.Checked
        Get_Data(3).Furikae_Enabled = chk３月振替日.Enabled
        Get_Data(3).Furikae_Date = txt３月振替日.Text
        Get_Data(3).Furikae_Day = lab３月振替日.Text

        Get_Data(3).SaiFurikae_Check = chk３月再振替日.Checked
        Get_Data(3).SaiFurikae_Enabled = chk３月再振替日.Enabled
        Get_Data(3).SaiFurikae_Date = txt３月再振替日.Text
        Get_Data(3).SaiFurikae_Day = lab３月再振替日.Text

    End Sub

#End Region

#Region " Private Sub(年間スケジュール画面制御)"
    Private Sub PSUB_NENKAN_FORMAT()

        '年間スケジュール部分初期表示

        'チェックボックス値
        Call PSUB_NENKAN_CHK(True)

        'チェックボックスEnable値
        Call PSUB_NENKAN_CHKBOXEnabled(True)

        'テキストボックス
        Call PSUB_NENKAN_DAYCLER()

        'テキストボックスEnable値
        Call PSUB_NENKAN_TEXTEnabled(True)

        '表示用ラベル
        Call PSUB_NENKAN_LABCLER()

    End Sub
    Private Sub PSUB_NENKAN_CHK(ByVal pValue As Boolean)

        '振替日の有効チェック
        chk４月振替日.Checked = pValue
        chk５月振替日.Checked = pValue
        chk６月振替日.Checked = pValue
        chk７月振替日.Checked = pValue
        chk８月振替日.Checked = pValue
        chk９月振替日.Checked = pValue
        chk１０月振替日.Checked = pValue
        chk１１月振替日.Checked = pValue
        chk１２月振替日.Checked = pValue
        chk１月振替日.Checked = pValue
        chk２月振替日.Checked = pValue
        chk３月振替日.Checked = pValue

        '再振替日の有効チェック
        chk４月再振替日.Checked = pValue
        chk５月再振替日.Checked = pValue
        chk６月再振替日.Checked = pValue
        chk７月再振替日.Checked = pValue
        chk８月再振替日.Checked = pValue
        chk９月再振替日.Checked = pValue
        chk１０月再振替日.Checked = pValue
        chk１１月再振替日.Checked = pValue
        chk１２月再振替日.Checked = pValue
        chk１月再振替日.Checked = pValue
        chk２月再振替日.Checked = pValue
        chk３月再振替日.Checked = pValue

    End Sub
    Private Sub PSUB_NENKAN_CHKBOXEnabled(ByVal pValue As Boolean)

        '振替日チェックBOXの有効化
        chk４月振替日.Enabled = pValue
        chk５月振替日.Enabled = pValue
        chk６月振替日.Enabled = pValue
        chk７月振替日.Enabled = pValue
        chk８月振替日.Enabled = pValue
        chk９月振替日.Enabled = pValue
        chk１０月振替日.Enabled = pValue
        chk１１月振替日.Enabled = pValue
        chk１２月振替日.Enabled = pValue
        chk１月振替日.Enabled = pValue
        chk２月振替日.Enabled = pValue
        chk３月振替日.Enabled = pValue

        '再振替日チェックBOXの有効化
        chk４月再振替日.Enabled = pValue
        chk５月再振替日.Enabled = pValue
        chk６月再振替日.Enabled = pValue
        chk７月再振替日.Enabled = pValue
        chk８月再振替日.Enabled = pValue
        chk９月再振替日.Enabled = pValue
        chk１０月再振替日.Enabled = pValue
        chk１１月再振替日.Enabled = pValue
        chk１２月再振替日.Enabled = pValue
        chk１月再振替日.Enabled = pValue
        chk２月再振替日.Enabled = pValue
        chk３月再振替日.Enabled = pValue

    End Sub
    Private Sub PSUB_NENKAN_DAYCLER()

        '振替日のクリア処理
        txt４月振替日.Text = ""
        txt５月振替日.Text = ""
        txt６月振替日.Text = ""
        txt７月振替日.Text = ""
        txt８月振替日.Text = ""
        txt９月振替日.Text = ""
        txt１０月振替日.Text = ""
        txt１１月振替日.Text = ""
        txt１２月振替日.Text = ""
        txt１月振替日.Text = ""
        txt２月振替日.Text = ""
        txt３月振替日.Text = ""

        '再振替日のクリア処理
        txt４月再振替日.Text = ""
        txt５月再振替日.Text = ""
        txt６月再振替日.Text = ""
        txt７月再振替日.Text = ""
        txt８月再振替日.Text = ""
        txt９月再振替日.Text = ""
        txt１０月再振替日.Text = ""
        txt１１月再振替日.Text = ""
        txt１２月再振替日.Text = ""
        txt１月再振替日.Text = ""
        txt２月再振替日.Text = ""
        txt３月再振替日.Text = ""

    End Sub
    Private Sub PSUB_NENKAN_TEXTEnabled(ByVal pValue As Boolean)

        '振替日テキストBOXの有効化
        txt４月振替日.Enabled = pValue
        txt５月振替日.Enabled = pValue
        txt６月振替日.Enabled = pValue
        txt７月振替日.Enabled = pValue
        txt８月振替日.Enabled = pValue
        txt９月振替日.Enabled = pValue
        txt１０月振替日.Enabled = pValue
        txt１１月振替日.Enabled = pValue
        txt１２月振替日.Enabled = pValue
        txt１月振替日.Enabled = pValue
        txt２月振替日.Enabled = pValue
        txt３月振替日.Enabled = pValue

        '振替日テキストBOXの有効化
        txt４月再振替日.Enabled = pValue
        txt５月再振替日.Enabled = pValue
        txt６月再振替日.Enabled = pValue
        txt７月再振替日.Enabled = pValue
        txt８月再振替日.Enabled = pValue
        txt９月再振替日.Enabled = pValue
        txt１０月再振替日.Enabled = pValue
        txt１１月再振替日.Enabled = pValue
        txt１２月再振替日.Enabled = pValue
        txt１月再振替日.Enabled = pValue
        txt２月再振替日.Enabled = pValue
        txt３月再振替日.Enabled = pValue

    End Sub
    Private Sub PSUB_NENKAN_LABCLER()

        '年間スケジュールの振替日ラベル、再振替日ラベルのクリア
        lab４月振替日.Text = ""
        lab５月振替日.Text = ""
        lab６月振替日.Text = ""
        lab７月振替日.Text = ""
        lab８月振替日.Text = ""
        lab９月振替日.Text = ""
        lab１０月振替日.Text = ""
        lab１１月振替日.Text = ""
        lab１２月振替日.Text = ""
        lab１月振替日.Text = ""
        lab２月振替日.Text = ""
        lab３月振替日.Text = ""

        lab４月再振替日.Text = ""
        lab５月再振替日.Text = ""
        lab６月再振替日.Text = ""
        lab７月再振替日.Text = ""
        lab８月再振替日.Text = ""
        lab９月再振替日.Text = ""
        lab１０月再振替日.Text = ""
        lab１１月再振替日.Text = ""
        lab１２月再振替日.Text = ""
        lab１月再振替日.Text = ""
        lab２月再振替日.Text = ""
        lab３月再振替日.Text = ""

    End Sub
    Private Sub PSUB_SAIFURI_PROTECT(ByVal pValue As Boolean, Optional ByVal pTuki As Integer = 0)

        '振替日有効チェックと振替日入力欄のプロテクト(ON/OFF)処理
        Select Case pTuki
            Case 0
                '全月対象
                chk４月再振替日.Checked = pValue
                chk４月再振替日.Enabled = pValue
                txt４月再振替日.Enabled = pValue

                chk５月再振替日.Checked = pValue
                chk５月再振替日.Enabled = pValue
                txt５月再振替日.Enabled = pValue

                chk６月再振替日.Checked = pValue
                chk６月再振替日.Enabled = pValue
                txt６月再振替日.Enabled = pValue

                chk７月再振替日.Checked = pValue
                chk７月再振替日.Enabled = pValue
                txt７月再振替日.Enabled = pValue

                chk８月再振替日.Checked = pValue
                chk８月再振替日.Enabled = pValue
                txt８月再振替日.Enabled = pValue

                chk９月再振替日.Checked = pValue
                chk９月再振替日.Enabled = pValue
                txt９月再振替日.Enabled = pValue

                chk１０月再振替日.Checked = pValue
                chk１０月再振替日.Enabled = pValue
                txt１０月再振替日.Enabled = pValue

                chk１１月再振替日.Checked = pValue
                chk１１月再振替日.Enabled = pValue
                txt１１月再振替日.Enabled = pValue

                chk１２月再振替日.Checked = pValue
                chk１２月再振替日.Enabled = pValue
                txt１２月再振替日.Enabled = pValue

                chk１月再振替日.Checked = pValue
                chk１月再振替日.Enabled = pValue
                txt１月再振替日.Enabled = pValue

                chk２月再振替日.Checked = pValue
                chk２月再振替日.Enabled = pValue
                txt２月再振替日.Enabled = pValue

                chk３月再振替日.Checked = pValue
                chk３月再振替日.Enabled = pValue
                txt３月再振替日.Enabled = pValue
            Case 1
                '１月
                chk１月再振替日.Checked = pValue
                chk１月再振替日.Enabled = pValue
                txt１月再振替日.Enabled = pValue
            Case 2
                '２月
                chk２月再振替日.Checked = pValue
                chk２月再振替日.Enabled = pValue
                txt２月再振替日.Enabled = pValue
            Case 3
                '３月
                chk３月再振替日.Checked = pValue
                chk３月再振替日.Enabled = pValue
                txt３月再振替日.Enabled = pValue
            Case 4
                '４月
                chk４月再振替日.Checked = pValue
                chk４月再振替日.Enabled = pValue
                txt４月再振替日.Enabled = pValue
            Case 5
                '５月
                chk５月再振替日.Checked = pValue
                chk５月再振替日.Enabled = pValue
                txt５月再振替日.Enabled = pValue
            Case 6
                '６月
                chk６月再振替日.Checked = pValue
                chk６月再振替日.Enabled = pValue
                txt６月再振替日.Enabled = pValue
            Case 7
                '７月
                chk７月再振替日.Checked = pValue
                chk７月再振替日.Enabled = pValue
                txt７月再振替日.Enabled = pValue
            Case 8
                '８月
                chk８月再振替日.Checked = pValue
                chk８月再振替日.Enabled = pValue
                txt８月再振替日.Enabled = pValue
            Case 9
                '９月
                chk９月再振替日.Checked = pValue
                chk９月再振替日.Enabled = pValue
                txt９月再振替日.Enabled = pValue
            Case 10
                '１０月
                chk１０月再振替日.Checked = pValue
                chk１０月再振替日.Enabled = pValue
                txt１０月再振替日.Enabled = pValue
            Case 11
                '１１月
                chk１１月再振替日.Checked = pValue
                chk１１月再振替日.Enabled = pValue
                txt１１月再振替日.Enabled = pValue
            Case 12
                '１２月
                chk１２月再振替日.Checked = pValue
                chk１２月再振替日.Enabled = pValue
                txt１２月再振替日.Enabled = pValue
        End Select

    End Sub

    Private Sub PSUB_NENKAN_SET(ByVal A As CheckBox, ByVal B As TextBox, ByVal C As Label, ByVal aReader As MyOracleReader)

        '年間スケジュールの振替日有効チェック、振替日、日付表示、再振替日有効チェック、振替日、日付表示の編集
        A.Checked = True

        '予備領域１から入力された振替日を得る
        B.Text = Trim(aReader.GetString("YOBI1_S"))
        C.Text = Mid(aReader.GetString("FURI_DATE_S"), 1, 4) & "/" & Mid(aReader.GetString("FURI_DATE_S"), 5, 2) & "/" & Mid(aReader.GetString("FURI_DATE_S"), 7, 2)

        '処理フラグ判定
        '日常業務処理中は編集できない
        A.Enabled = False
        B.Enabled = False
        Select Case True
            Case aReader.GetString("ENTRI_FLG_S") = "1"
            Case aReader.GetString("CHECK_FLG_S") = "1"
            Case aReader.GetString("DATA_FLG_S") = "1"
            Case aReader.GetString("FUNOU_FLG_S") = "1"
            Case aReader.GetString("SAIFURI_FLG_S") = "1"
            Case aReader.GetString("KESSAI_FLG_S") = "1"
            Case aReader.GetString("TYUUDAN_FLG_S") = "1"
            Case Else
                A.Enabled = True
                B.Enabled = True
        End Select

    End Sub
#End Region

#Region " Private Function(年間スケジュール)"
    Private Function PFUNC_SCH_GET_NENKAN() As Boolean

        PFUNC_SCH_GET_NENKAN = False

        '振替日の有効チェックOFF、再振替日の有効チェックOFF
        Call PSUB_NENKAN_CHK(False)

        '振替日入力欄、再振替日入力欄のクリア
        Call PSUB_NENKAN_DAYCLER()

        '振替日、再振替日ラベルクリア
        Call PSUB_NENKAN_LABCLER()

        If PFUNC_NENKAN_SANSYOU() = False Then
            Exit Function
        End If

        PFUNC_SCH_GET_NENKAN = True

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_NENKAN() As Boolean

        '年間スケジュール更新処理
        If PFUNC_NENKAN_KOUSIN() = False Then
            'ここを通るということは１件でも処理したレコードが存在したということなので
            Int_Syori_Flag(0) = 2
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_SCH_NENKAN_GET(ByVal strNENGETUDO As String, ByVal strFURIKUBUN As String, ByVal astrFURI_DATE As String) As Boolean

        Dim iGakunen(8) As Integer
        Dim iCount As Integer
        Dim bFlg As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '通常レコードの存在チェック
        PFUNC_SCH_NENKAN_GET = True

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='" & strFURIKUBUN & "'")

        If oraReader.DataReader(sql) = False Then
            '通常レコ−ド無し
            oraReader.Close()
            Exit Function
        End If
        oraReader.Close()

        PFUNC_SCH_NENKAN_GET = False
        bFlg = False

        sql = New StringBuilder(128)

        '学年指定がない場合は処理をしない
        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            '特別レコードの対象学年フラグの状態を元に通常レコードの対象学年フラグをＯＦＦにする
            'ＯＮにする機能を持たせた場合、特別レコードが複数件存在した場合に前レコードでの処理が無駄になる
            '特別レコードの対象学年１フラグが「１」の場合
            sql.Append(" UPDATE  G_SCHMAST")
            sql.Append(" SET ")
            For iCount = 1 To 9
                If iGakunen(iCount - 1) = 1 Then
                    If bFlg = False Then
                        sql.Append(" ")

                        bFlg = True
                    Else
                        sql.Append(",")
                    End If

                    sql.Append(" GAKUNEN" & iCount & "_FLG_S ='0'")
                End If
            Next iCount
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
            sql.Append(" AND")
            sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
            sql.Append(" AND")
            sql.Append(" SCH_KBN_S ='0'")
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S ='" & strFURIKUBUN & "'")

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                MessageBox.Show("スケジュールマスタの更新処理でエラーが発生しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        End If

        PFUNC_SCH_NENKAN_GET = True

    End Function
    Private Function PFUNC_GAKUNEN_GET(ByRef pValue() As Integer) As Boolean

        PFUNC_GAKUNEN_GET = False

        ReDim pValue(8)

        If STR１学年 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(0) = 1
        Else
            pValue(0) = 0
        End If
        If STR２学年 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(1) = 1
        Else
            pValue(1) = 0
        End If
        If STR３学年 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(2) = 1
        Else
            pValue(2) = 0
        End If
        If STR４学年 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(3) = 1
        Else
            pValue(3) = 0
        End If
        If STR５学年 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(4) = 1
        Else
            pValue(4) = 0
        End If
        If STR６学年 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(5) = 1
        Else
            pValue(5) = 0
        End If
        If STR７学年 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(6) = 1
        Else
            pValue(6) = 0
        End If
        If STR８学年 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(7) = 1
        Else
            pValue(7) = 0
        End If
        If STR９学年 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(8) = 1
        Else
            pValue(8) = 0
        End If

    End Function

    Private Function PFUNC_NENKAN_SANSYOU() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '年間スケジュール　参照処理
        PFUNC_NENKAN_SANSYOU = False

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = 0")

        If oraReader.DataReader(sql) = False Then
            oraReader.Close()
            Exit Function
        End If

        Do Until oraReader.EOF
            Select Case oraReader.GetString("FURI_KBN_S")
                Case "0"
                    '初振スケジュール
                    Select Case Mid(oraReader.GetString("NENGETUDO_S"), 5, 2)
                        Case "04"   '振替日の月
                            Call PSUB_NENKAN_SET(chk４月振替日, txt４月振替日, lab４月振替日, oraReader)
                            '2006/11/22　表示時の振替日を取得
                            str通常振替日(4) = Replace(lab４月振替日.Text, "/", "")
                            '2006/11/30　チェックフラグ・不能フラグを構造体に格納
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(4).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(4).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(4).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(4).FunouFurikae_Flag = False
                            End If
                        Case "05"
                            Call PSUB_NENKAN_SET(chk５月振替日, txt５月振替日, lab５月振替日, oraReader)
                            str通常振替日(5) = Replace(lab５月振替日.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(5).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(5).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(5).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(5).FunouFurikae_Flag = False
                            End If
                        Case "06"
                            Call PSUB_NENKAN_SET(chk６月振替日, txt６月振替日, lab６月振替日, oraReader)
                            str通常振替日(6) = Replace(lab６月振替日.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(6).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(6).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(6).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(6).FunouFurikae_Flag = False
                            End If
                        Case "07"
                            Call PSUB_NENKAN_SET(chk７月振替日, txt７月振替日, lab７月振替日, oraReader)
                            str通常振替日(7) = Replace(lab７月振替日.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(7).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(7).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(7).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(7).FunouFurikae_Flag = False
                            End If
                        Case "08"
                            Call PSUB_NENKAN_SET(chk８月振替日, txt８月振替日, lab８月振替日, oraReader)
                            str通常振替日(8) = Replace(lab８月振替日.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(8).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(8).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(8).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(8).FunouFurikae_Flag = False
                            End If
                        Case "09"
                            Call PSUB_NENKAN_SET(chk９月振替日, txt９月振替日, lab９月振替日, oraReader)
                            str通常振替日(9) = Replace(lab９月振替日.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(9).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(9).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(9).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(9).FunouFurikae_Flag = False
                            End If
                        Case "10"
                            Call PSUB_NENKAN_SET(chk１０月振替日, txt１０月振替日, lab１０月振替日, oraReader)
                            str通常振替日(10) = Replace(lab１０月振替日.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(10).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(10).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(10).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(10).FunouFurikae_Flag = False
                            End If
                        Case "11"
                            Call PSUB_NENKAN_SET(chk１１月振替日, txt１１月振替日, lab１１月振替日, oraReader)
                            str通常振替日(11) = Replace(lab１１月振替日.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(11).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(11).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(11).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(11).FunouFurikae_Flag = False
                            End If
                        Case "12"
                            Call PSUB_NENKAN_SET(chk１２月振替日, txt１２月振替日, lab１２月振替日, oraReader)
                            str通常振替日(12) = Replace(lab１２月振替日.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(12).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(12).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(12).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(12).FunouFurikae_Flag = False
                            End If
                        Case "01"
                            Call PSUB_NENKAN_SET(chk１月振替日, txt１月振替日, lab１月振替日, oraReader)
                            str通常振替日(1) = Replace(lab１月振替日.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(1).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(1).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(1).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(1).FunouFurikae_Flag = False
                            End If
                        Case "02"
                            Call PSUB_NENKAN_SET(chk２月振替日, txt２月振替日, lab２月振替日, oraReader)
                            str通常振替日(2) = Replace(lab２月振替日.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(2).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(2).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(2).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(2).FunouFurikae_Flag = False
                            End If
                        Case "03"
                            Call PSUB_NENKAN_SET(chk３月振替日, txt３月振替日, lab３月振替日, oraReader)
                            str通常振替日(3) = Replace(lab３月振替日.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(3).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(3).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(3).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(3).FunouFurikae_Flag = False
                            End If
                    End Select
                Case "1"
                    '再振スケジュール
                    Select Case Mid(oraReader.GetString("NENGETUDO_S"), 5, 2)
                        Case "04"    '再振替日の月
                            Call PSUB_NENKAN_SET(chk４月再振替日, txt４月再振替日, lab４月再振替日, oraReader)
                            '2006/11/22　表示時の振替日を取得
                            str通常再振日(4) = Replace(lab４月再振替日.Text, "/", "")
                            '2006/11/30　再振日の再振日を求める
                            str通常再々振日(4) = oraReader.GetString("SFURI_DATE_S")
                            '2006/11/30　チェックフラグを取得
                            SYOKI_NENKAN_SCHINFO(4).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "05"
                            Call PSUB_NENKAN_SET(chk５月再振替日, txt５月再振替日, lab５月再振替日, oraReader)
                            str通常再振日(5) = Replace(lab５月再振替日.Text, "/", "")
                            str通常再々振日(5) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(5).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "06"
                            Call PSUB_NENKAN_SET(chk６月再振替日, txt６月再振替日, lab６月再振替日, oraReader)
                            str通常再振日(6) = Replace(lab６月再振替日.Text, "/", "")
                            str通常再々振日(6) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(6).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "07"
                            Call PSUB_NENKAN_SET(chk７月再振替日, txt７月再振替日, lab７月再振替日, oraReader)
                            str通常再振日(7) = Replace(lab７月再振替日.Text, "/", "")
                            str通常再々振日(7) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(7).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "08"
                            Call PSUB_NENKAN_SET(chk８月再振替日, txt８月再振替日, lab８月再振替日, oraReader)
                            str通常再振日(8) = Replace(lab８月再振替日.Text, "/", "")
                            str通常再々振日(8) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(8).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "09"
                            Call PSUB_NENKAN_SET(chk９月再振替日, txt９月再振替日, lab９月再振替日, oraReader)
                            str通常再振日(9) = Replace(lab９月再振替日.Text, "/", "")
                            str通常再々振日(9) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(9).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "10"
                            Call PSUB_NENKAN_SET(chk１０月再振替日, txt１０月再振替日, lab１０月再振替日, oraReader)
                            str通常再振日(10) = Replace(lab１０月再振替日.Text, "/", "")
                            str通常再々振日(10) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(10).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "11"
                            Call PSUB_NENKAN_SET(chk１１月再振替日, txt１１月再振替日, lab１１月再振替日, oraReader)
                            str通常再振日(11) = Replace(lab１１月再振替日.Text, "/", "")
                            str通常再々振日(11) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(11).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "12"
                            Call PSUB_NENKAN_SET(chk１２月再振替日, txt１２月再振替日, lab１２月再振替日, oraReader)
                            str通常再振日(12) = Replace(lab１２月再振替日.Text, "/", "")
                            str通常再々振日(12) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(12).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "01"
                            Call PSUB_NENKAN_SET(chk１月再振替日, txt１月再振替日, lab１月再振替日, oraReader)
                            str通常再振日(1) = Replace(lab１月再振替日.Text, "/", "")
                            str通常再々振日(1) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(1).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "02"
                            Call PSUB_NENKAN_SET(chk２月再振替日, txt２月再振替日, lab２月再振替日, oraReader)
                            str通常再振日(2) = Replace(lab２月再振替日.Text, "/", "")
                            str通常再々振日(2) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(2).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "03"
                            Call PSUB_NENKAN_SET(chk３月再振替日, txt３月再振替日, lab３月再振替日, oraReader)
                            str通常再振日(3) = Replace(lab３月再振替日.Text, "/", "")
                            str通常再々振日(3) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(3).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                    End Select
            End Select

            oraReader.NextRead()

        Loop

        oraReader.Close()

        PFUNC_NENKAN_SANSYOU = True

    End Function
    Private Function PFUNC_NENKAN_DATE_CHECK(ByVal pFurikae As String, ByVal pSaifuri As String) As Boolean

        PFUNC_NENKAN_DATE_CHECK = False

        '振替日と再振替日が同一？
        If Trim(pFurikae) <> "" And Trim(pSaifuri) <> "" Then
            If Trim(pFurikae) = Trim(pSaifuri) Then
                Exit Function
            End If
        End If

        PFUNC_NENKAN_DATE_CHECK = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI() As Boolean

        Dim sTuki As String

        PFUNC_NENKAN_SAKUSEI = False

        ''入力内容を変数に退避
        ''　後の処理を簡略化する為に必要
        'Call PSUB_NENKAN_GET() '2006/11/30　コメント化

        '振替日と再振替日が同一の場合はエラー
        For i As Integer = 1 To 12
            If NENKAN_SCHINFO(i).Furikae_Check = True And NENKAN_SCHINFO(i).SaiFurikae_Check = True Then
                If PFUNC_NENKAN_DATE_CHECK(NENKAN_SCHINFO(i).Furikae_Date, NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                    MessageBox.Show("(年間スケジュール)" & vbCrLf & "振替日と再振替日が同一です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If
        Next i

        '振替日チェック
        For i As Integer = 1 To 12
            If bln年間更新(i) = True Then '2006/11/30　更新のないものは更新の必要なし

                If NENKAN_SCHINFO(i).Furikae_Check = True And NENKAN_SCHINFO(i).Furikae_Enabled = True Then
                    sTuki = Format(i, "00")

                    STR１学年 = "1"
                    STR２学年 = "1"
                    STR３学年 = "1"
                    STR４学年 = "1"
                    STR５学年 = "1"
                    STR６学年 = "1"
                    STR７学年 = "1"
                    STR８学年 = "1"
                    STR９学年 = "1"

                    'パラメタは�@月 �A入力振替日 �B再振替月 �C再振替日
                    Select Case NENKAN_SCHINFO(i).SaiFurikae_Check
                        Case True
                            If PFUNC_NENKAN_SAKUSEI_SUB(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date, i) = False Then
                                Exit Function
                            End If
                        Case False
                            If PFUNC_NENKAN_SAKUSEI_SUB(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, "", NENKAN_SCHINFO(i).SaiFurikae_Date, i) = False Then
                                Exit Function
                            End If
                    End Select

                    'ここを通るということは処理に成功したということなので
                    Int_Syori_Flag(0) = 1
                Else
                    '初振のスケジュールが処理中でも再振のほうは
                    If NENKAN_SCHINFO(i).SaiFurikae_Check = True And NENKAN_SCHINFO(i).SaiFurikae_Enabled = True Then

                        sTuki = Format(i, "00")
                        STR１学年 = "1"
                        STR２学年 = "1"
                        STR３学年 = "1"
                        STR４学年 = "1"
                        STR５学年 = "1"
                        STR６学年 = "1"
                        STR７学年 = "1"
                        STR８学年 = "1"
                        STR９学年 = "1"
                        If PFUNC_NENKAN_SAKUSEI_SUB2(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date, i) = False Then
                            Exit Function
                        End If

                        '作成した再振のスケジュールの振替日を初振のスケジュールの再振日へ更新する
                        If PFUNC_SCHMAST_UPDATE_SFURIDATE("0") = False Then

                            Exit Function
                        End If
                        '追記 2006/12/04
                        'ここを通るということは処理に成功したということなので
                        Int_Syori_Flag(0) = 1

                    End If
                End If
            Else '更新しなくても企業側のスケジュールを見る

                '企業自振連携時のみ
                If NENKAN_SCHINFO(i).Furikae_Check = True And NENKAN_SCHINFO(i).Furikae_Enabled = True Then
                    sTuki = Format(i, "00")

                    STR１学年 = "1"
                    STR２学年 = "1"
                    STR３学年 = "1"
                    STR４学年 = "1"
                    STR５学年 = "1"
                    STR６学年 = "1"
                    STR７学年 = "1"
                    STR８学年 = "1"
                    STR９学年 = "1"

                    'パラメタは�@月 �A入力振替日 �B再振替月 �C再振替日
                    Select Case NENKAN_SCHINFO(i).SaiFurikae_Check
                        Case True
                            If PFUNC_NENKAN_SAKUSEI_SUB_KIGYO(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                                Exit Function
                            End If
                        Case False
                            If PFUNC_NENKAN_SAKUSEI_SUB_KIGYO(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, "", NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                                Exit Function
                            End If
                    End Select

                    'ここを通るということは処理に成功したということなので
                    Int_Syori_Flag(0) = 1
                Else
                    '初振のスケジュールが処理中でも再振のほうは
                    If NENKAN_SCHINFO(i).SaiFurikae_Check = True And NENKAN_SCHINFO(i).SaiFurikae_Enabled = True Then

                        sTuki = Format(i, "00")
                        STR１学年 = "1"
                        STR２学年 = "1"
                        STR３学年 = "1"
                        STR４学年 = "1"
                        STR５学年 = "1"
                        STR６学年 = "1"
                        STR７学年 = "1"
                        STR８学年 = "1"
                        STR９学年 = "1"
                        If PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                            Exit Function
                        End If
                        '追記 2006/12/04
                        'ここを通るということは処理に成功したということなので
                        Int_Syori_Flag(0) = 1

                    End If
                End If
            End If
        Next i

        PFUNC_NENKAN_SAKUSEI = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI_SUB(ByVal s請求月 As String, ByVal s月 As String, ByVal s振替日 As String, ByVal s再振替月 As String, ByVal s再振替日 As String, ByVal i As Integer) As Boolean
        'スケジュール　通常レコード(初振)作成

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUNC_NENKAN_SAKUSEI_SUB = False
        Dim updade As Boolean

        '請求年月の作成
        STR請求年月 = PFUNC_SEIKYUTUKIHI(s請求月)

        '振替日算出
        STR振替日 = PFUNC_FURIHI_MAKE(s月, s振替日, "0", "0")

        '2010/10/21 契約振替日を算出する
        STR契約振替日 = PFUNC_KFURIHI_MAKE(s月, s振替日, "0", "0")

        '再振日の翌月判定と再振替年、再振替月設定
        '再振替日が入力され、かつ年間スケジュールのチェックボックスがＯＮ（s再振替月に月が設定）の場合
        If s再振替月 <> "" And s再振替日 <> "" Then
            STRW再振替年 = Mid(STR振替日, 1, 4)

            'If Mid(STR振替日, 7, 2) <= s再振替日 Then
            If STR振替日 <= STR請求年月 & s再振替日 Then
                STRW再振替月 = s再振替月
                STRW再振替日 = s再振替日
            Else
                If s月 = "12" Then
                    STRW再振替月 = "01"
                    STRW再振替年 = CStr(CInt(Mid(STR請求年月, 1, 4)) + 1)

                Else
                    STRW再振替月 = Format((CInt(s再振替月) + 1), "00")
                End If
                STRW再振替日 = s再振替日
            End If
        End If

        '再振替日が入力なし、かつ年間スケジュールのチェックボックスがＯＮ
        If s再振替月 <> "" And s再振替日 = "" Then

            STRW再振替年 = Mid(STR振替日, 1, 4)

            'If Mid(STR振替日, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
            If STR振替日 <= STR請求年月 & GAKKOU_INFO.SFURI_DATE Then
                'STRW再振替月 = s再振替月
                'STRW再振替日 = GAKKOU_INFO.SFURI_DATE
                If Mid(STR振替日, 5, 2) > Mid(STR請求年月, 5, 2) Then
                    If s月 = "12" Then
                        STRW再振替月 = "01"
                        STRW再振替年 = CStr(CInt(Mid(STR請求年月, 1, 4)) + 1)
                    Else
                        STRW再振替月 = Format((CInt(s再振替月) + 1), "00")
                    End If
                    STRW再振替日 = GAKKOU_INFO.SFURI_DATE

                Else
                    STRW再振替月 = s再振替月
                    STRW再振替日 = GAKKOU_INFO.SFURI_DATE
                End If
            Else
                If s月 = "12" Then
                    STRW再振替月 = "01"
                    STRW再振替年 = CStr(CInt(Mid(STR請求年月, 1, 4)) + 1)
                Else
                    STRW再振替月 = Format((CInt(s再振替月) + 1), "00")
                End If
                STRW再振替日 = GAKKOU_INFO.SFURI_DATE
            End If
        End If


        If s再振替月 = "" Then
            STR再振替日 = "00000000"
        Else
            '再振替日算出
            STR再振替日 = PFUNC_SAIFURIHI_MAKE(Trim(STRW再振替月), Trim(STRW再振替日))
        End If

        '営業日を算出した結果で振替日と再振替日が同一になる場合がある為
        If STR振替日 = STR再振替日 Then
            MessageBox.Show("(年間スケジュール)" & vbCrLf & Mid(STR振替日, 5, 2) & "月の" & "振替日と再振替日が同一です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If


        '振替日の有効範囲チェック
        If PFUNC_FURIHI_HANI_CHECK() = False Then
            MessageBox.Show("(年間スケジュール)" & vbCrLf & "振替契約期間（" & GAKKOU_INFO.KAISI_DATE & "〜" & GAKKOU_INFO.SYURYOU_DATE & "）外の月です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '特別レコードの対象学年の設定し直し
        '学校コード、請求年月、振替区分（0:初振）
        If PFUNC_SCH_TOKUBETU_GET(STR請求年月, "0") = False Then
            MessageBox.Show("(年間スケジュール)" & vbCrLf & "特別スケジュール対象学年設定でエラーが発生しました(初振)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        '既存レコード有無チェック
        If PFUNC_SCHMAST_GET("0", "0", Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", ""), Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "")) = True Then
            updade = True
        End If

        'スケジュール区分の共通変数設定
        STRスケ区分 = "0"

        '振替区分の共通変数設定
        STR振替区分 = "0"

        '入力振替日の共通変数設定
        If s振替日 = "" Then
            STR年間入力振替日 = Space(15)
        Else
            STR年間入力振替日 = s振替日
        End If

        Dim strSQL As String = ""
        If updade = False Then
            'スケジュールマスタ登録SQL文(初振)作成
            strSQL = PSUB_INSERT_G_SCHMAST_SQL()
        Else
            'スケジュールマスタ更新SQL文(初振)作成
            strSQL = PSUB_UPDATE_G_SCHMAST_SQL(Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", ""), Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""))
        End If

        If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            MessageBox.Show("登録に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '-----------------------------------------------
        '2006/07/26　企業自振の初振のスケジュールも作成
        '-----------------------------------------------
        '企業自振連携時のみ

        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        '既に登録されているかチェック
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        sql.Append("FURI_DATE_S = '" & STR振替日 & "'")

        If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
        Else     'スケジュールが存在しない
            'コメント 2006/12/11
            'If intPUSH_BTN = 2 Then '更新時
            '    MessageBox.Show("企業自振側のスケジュール(" & STR請求年月.Substring(0, 4) & "年" & STR請求年月.Substring(4, 2) & "月分)が存在しません" & vbCrLf & "企業自振側で月間スケジュール作成後、" & vbCrLf & "学校スケジュールの更新処理を再度行ってください", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            'Else
            'スケジュール作成
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '取引先マスタに取引先コードが存在することを確認
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                     gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                '2010/10/21 契約振替日対応 引数に追加
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, "01", STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", "企業自振のスケジュールが登録できませんでした")
                    MessageBox.Show("企業自振のスケジュールが登録できませんでした", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If
            'End If

        End If
        oraReader.Close()


        '再振レコードの作成
        If STR再振替日 <> "00000000" Then

            '2006/11/30　updateフラグの初期化
            updade = False

            '初振で求めた再振日を振替日に設定
            STR振替日 = STR再振替日

            '2010/10/21 再振の契約振替日を算出する
            '2011/06/15 標準版修正 契約振替日と契約再振日が逆転する場合は翌月の再振日にする -------------START
            'STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "0", "1")
            If s再振替日 = "" Then
                STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "0", "1")
            Else
                STR契約振替日 = STR振替日
            End If
            '2011/06/15 標準版修正 契約振替日と契約再振日が逆転する場合は翌月の再振日にする -------------END

            '再振日の算出
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(再振有/繰越無)
                    STR再振替日 = "00000000"
                Case "2"    '2(再振有/繰越有)   次回初振日を設定
                    '2011/06/15 標準版修正 実際の振替日から再振日を算出する -------------START

                    'If s再振替日 = "" Then
                    '    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(strSFURI_DT))
                    'Else
                    '    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(s再振替日))
                    'End If
                    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(STR振替日)
                    '2011/06/15 標準版修正 実際の振替日から再振日を算出する -------------END
            End Select


            '特別レコードの対象学年の設定し直し
            '学校コード、請求年月、振替区分（1:再振）
            If PFUNC_SCH_TOKUBETU_GET(STR請求年月, "1") = False Then
                MessageBox.Show("(年間スケジュール)" & vbCrLf & "特別スケジュール対象学年設定でエラーが発生しました(再振)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            If PFUNC_SCHMAST_GET("0", "1", Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str通常再々振日(i)) = True Then
                updade = True
            End If

            'スケジュール区分の共通変数設定
            STRスケ区分 = "0"
            '振替区分の共通変数設定
            STR振替区分 = "1"
            '入力振替日の共通変数設定
            If s再振替日 = "" Then
                STR年間入力振替日 = Space(15)
            Else
                STR年間入力振替日 = s再振替日
            End If

            '2006/11/30　新規登録か更新か判定
            strSQL = ""
            If updade = False Then
                'スケジュールマスタ登録SQL文(再振)作成
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            Else
                'スケジュールマスタ更新SQL文(再振)作成
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str通常再々振日(i))
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                MessageBox.Show("登録に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            STR年間入力振替日 = Space(15)
            updade = False

            '-----------------------------------------------
            '2006/07/26　企業自振の再振のスケジュールも作成
            '-----------------------------------------------
            '企業自振連携時のみ
            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)

            '既に登録されているかチェック
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR振替日 & "'")

            '読込のみ
            If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
            Else     'スケジュールが存在しない
                'スケジュール作成
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '取引先マスタに取引先コードが存在することを確認
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                    '2010/10/21 契約振替日対応 引数に追加
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", "企業自振のスケジュールが登録できませんでした")
                        MessageBox.Show("企業自振のスケジュールが登録できませんでした", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()

        End If

        PFUNC_NENKAN_SAKUSEI_SUB = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI_SUB2(ByVal s請求月 As String, ByVal s月 As String, ByVal s振替日 As String, ByVal s再振替月 As String, ByVal s再振替日 As String, ByVal i As Integer) As Boolean
        'スケジュール　通常レコード作成

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUNC_NENKAN_SAKUSEI_SUB2 = False

        Dim updade As Boolean

        '初振レコードの作成

        '請求年月の作成
        STR請求年月 = PFUNC_SEIKYUTUKIHI(s請求月)

        '振替日算出
        STR振替日 = PFUNC_FURIHI_MAKE(s月, s振替日, "0", "0")

        '2010/10/21 契約振替日を算出する
        STR契約振替日 = PFUNC_KFURIHI_MAKE(s月, s振替日, "0", "0")

        '再振日の翌月判定と再振替年、再振替月設定
        '再振替日が入力され、かつ年間スケジュールのチェックボックスがＯＮ（s再振替月に月が設定）の場合
        If s再振替月 <> "" And s再振替日 <> "" Then
            STRW再振替年 = Mid(STR振替日, 1, 4)

            '2011/06/15 標準版修正 実際の振替日から再振日を算出する -------------START
            If STR振替日 <= STR請求年月 & s再振替日 Then
                'If Mid(STR振替日, 7, 2) <= s再振替日 Then
                '2011/06/15 標準版修正 実際の振替日から再振日を算出する -------------END

                STRW再振替月 = s再振替月
                STRW再振替日 = s再振替日
            Else
                If s月 = "12" Then
                    STRW再振替月 = "01"
                    STRW再振替年 = CStr(CInt(Mid(STR請求年月, 1, 4)) + 1)

                Else
                    STRW再振替月 = Format((CInt(s再振替月) + 1), "00")
                End If
                STRW再振替日 = s再振替日
            End If
        End If

        '再振替日が入力なし、かつ年間スケジュールのチェックボックスがＯＮ
        If s再振替月 <> "" And s再振替日 = "" Then

            STRW再振替年 = Mid(STR振替日, 1, 4)
            '2011/06/15 標準版修正 実際の振替日から再振日を算出する -------------START
            If STR振替日 <= STR請求年月 & GAKKOU_INFO.SFURI_DATE Then
            'If Mid(STR振替日, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
                '2011/06/15 標準版修正 実際の振替日から再振日を算出する -------------END
                STRW再振替月 = s再振替月
                STRW再振替日 = GAKKOU_INFO.SFURI_DATE
            Else
                If s月 = "12" Then
                    STRW再振替月 = "01"
                    STRW再振替年 = CStr(CInt(Mid(STR請求年月, 1, 4)) + 1)
                Else
                    STRW再振替月 = Format((CInt(s再振替月) + 1), "00")
                End If
                STRW再振替日 = GAKKOU_INFO.SFURI_DATE
            End If
        End If

        If s再振替月 = "" Then
            STR再振替日 = "00000000"
        Else
            '再振替日算出
            STR再振替日 = PFUNC_SAIFURIHI_MAKE(Trim(STRW再振替月), Trim(STRW再振替日))
        End If

        '営業日を算出した結果で振替日と再振替日が同一になる場合がある為
        If STR振替日 = STR再振替日 Then
            MessageBox.Show("(年間スケジュール)" & vbCrLf & Mid(STR振替日, 5, 2) & "月の" & "振替日と再振替日が同一です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR振替日
        Str_SFURI_DATE = STR再振替日

        '再振レコードの作成
        If STR再振替日 <> "00000000" Then

            '初振で求めた再振日を振替日に設定
            STR振替日 = STR再振替日

            '2010/10/21 再振の契約振替日を算出する
            '2011/06/16 標準版修正 再振日が入力されている場合は実振替日を契約振替日とする ------------------START
            'STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "0", "1")
            If s再振替日 = "" Then
                STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "0", "1")
            Else
                STR契約振替日 = STR振替日
            End If
            '2011/06/16 標準版修正 再振日が入力されている場合は実振替日を契約振替日とする ------------------END

            '再振日の算出
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(再振有/繰越無)
                    STR再振替日 = "00000000"
                Case "2"    '2(再振有/繰越有)   次回初振日を設定
                    '2011/06/16 標準版修正 実際の振替日から再振日を算出する ------------------START
                    ''s再振替日に学校マスタ２の再振替日をセット 2005/12/09
                    'If s再振替日 = "" Then
                    '    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(strSFURI_DT))
                    'Else
                    '    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(s再振替日))
                    'End If
                    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(STR振替日)
                    '2011/06/16 標準版修正 実際の振替日から再振日を算出する ------------------END
            End Select


            '特別レコードの対象学年の設定し直し
            '学校コード、請求年月、振替区分（1:再振）
            If PFUNC_SCH_TOKUBETU_GET(STR請求年月, "1") = False Then
                MessageBox.Show("(年間スケジュール)" & vbCrLf & "特別スケジュール対象学年設定でエラーが発生しました(再振)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '既存レコード有無チェック
            '2011/06/16 標準版修正 実際の振替日から再振日を算出する ------------------START
            If PFUNC_SCHMAST_GET("0", "1", Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str通常再々振日(i)) = True Then
                'If PFUNC_SCHMAST_GET("0", "0", Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", ""), Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "")) = True Then
                '2011/06/16 標準版修正 実際の振替日から再振日を算出する ------------------END

                updade = True
            End If

            'スケジュール区分の共通変数設定
            STRスケ区分 = "0"
            '振替区分の共通変数設定
            STR振替区分 = "1"
            '入力振替日の共通変数設定
            If s再振替日 = "" Then
                STR年間入力振替日 = Space(15)
            Else
                STR年間入力振替日 = s再振替日
            End If

            '2006/11/30　新規登録か更新か判定
            Dim strSQL As String = ""
            If updade = False Then
                'スケジュールマスタ登録SQL文(初振)作成
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            Else
                'スケジュールマスタ更新SQL文(初振)作成
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str通常再々振日(i))
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                MessageBox.Show("登録に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '-----------------------------------------------
            '2006/07/26　企業自振の再振のスケジュールも作成
            '-----------------------------------------------
            '企業自振連携時のみ
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)

            '既に登録されているかチェック
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR振替日 & "'")

            If oraReader.DataReader(sql) = True Then 'スケジュールが既に存在する

            Else     'スケジュールが存在しない
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '取引先マスタに取引先コードが存在することを確認
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then

                    '2010/10/21 契約振替日対応 引数に追加
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", "企業自振のスケジュールが登録できませんでした")
                        MessageBox.Show("企業自振のスケジュールが登録できませんでした", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If
                End If

            End If
            oraReader.Close()
        End If

        '-----------------------------------------------
        PFUNC_NENKAN_SAKUSEI_SUB2 = True

    End Function
    '企業のスケジュール更新用 2006/12/08
    Private Function PFUNC_NENKAN_SAKUSEI_SUB_KIGYO(ByVal s請求月 As String, ByVal s月 As String, ByVal s振替日 As String, ByVal s再振替月 As String, ByVal s再振替日 As String) As Boolean
        'スケジュール　通常レコード(初振)作成

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUNC_NENKAN_SAKUSEI_SUB_KIGYO = False
        Dim updade As Boolean

        '請求年月の作成
        STR請求年月 = PFUNC_SEIKYUTUKIHI(s請求月)

        '振替日算出
        STR振替日 = PFUNC_FURIHI_MAKE(s月, s振替日, "0", "0")

        '*** 修正 mitsu 2009/07/29 契約振替日を算出する ***
        STR契約振替日 = PFUNC_KFURIHI_MAKE(s月, s振替日, "0", "0")

        '再振日の翌月判定と再振替年、再振替月設定
        '再振替日が入力され、かつ年間スケジュールのチェックボックスがＯＮ（s再振替月に月が設定）の場合
        If s再振替月 <> "" And s再振替日 <> "" Then
            STRW再振替年 = Mid(STR振替日, 1, 4)

            '2011/06/16 標準版修正 年を考慮する ------------------START
            'If Mid(STR振替日, 7, 2) <= s再振替日 Then
            If STR振替日 <= STR請求年月 & s再振替日 Then
                '2011/06/16 標準版修正 年を考慮する ------------------END
                STRW再振替月 = s再振替月
                STRW再振替日 = s再振替日
            Else
                If s月 = "12" Then
                    STRW再振替月 = "01"
                    STRW再振替年 = CStr(CInt(Mid(STR請求年月, 1, 4)) + 1)

                Else
                    STRW再振替月 = Format((CInt(s再振替月) + 1), "00")
                End If
                STRW再振替日 = s再振替日
            End If
        End If

        '再振替日が入力なし、かつ年間スケジュールのチェックボックスがＯＮ
        If s再振替月 <> "" And s再振替日 = "" Then

            STRW再振替年 = Mid(STR振替日, 1, 4)

            '2011/06/16 標準版修正 年を考慮する ------------------START
            'If Mid(STR振替日, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
            If STR振替日 <= STR請求年月 & GAKKOU_INFO.SFURI_DATE Then
                '2011/06/16 標準版修正 年を考慮する ------------------END
                'STRW再振替月 = s再振替月
                'STRW再振替日 = GAKKOU_INFO.SFURI_DATE
                If Mid(STR振替日, 5, 2) > Mid(STR請求年月, 5, 2) Then
                    If s月 = "12" Then
                        STRW再振替月 = "01"
                        STRW再振替年 = CStr(CInt(Mid(STR請求年月, 1, 4)) + 1)
                    Else
                        STRW再振替月 = Format((CInt(s再振替月) + 1), "00")
                    End If
                    STRW再振替日 = GAKKOU_INFO.SFURI_DATE

                Else
                    STRW再振替月 = s再振替月
                    STRW再振替日 = GAKKOU_INFO.SFURI_DATE
                End If
            Else
                If s月 = "12" Then
                    STRW再振替月 = "01"
                    STRW再振替年 = CStr(CInt(Mid(STR請求年月, 1, 4)) + 1)
                Else
                    STRW再振替月 = Format((CInt(s再振替月) + 1), "00")
                End If
                STRW再振替日 = GAKKOU_INFO.SFURI_DATE
            End If
        End If


        If s再振替月 = "" Then
            STR再振替日 = "00000000"
        Else
            '再振替日算出
            STR再振替日 = PFUNC_SAIFURIHI_MAKE(Trim(STRW再振替月), Trim(STRW再振替日))
        End If

        '営業日を算出した結果で振替日と再振替日が同一になる場合がある為
        If STR振替日 = STR再振替日 Then
            MessageBox.Show("(年間スケジュール)" & vbCrLf & Mid(STR振替日, 5, 2) & "月の" & "振替日と再振替日が同一です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If


        '振替日の有効範囲チェック
        If PFUNC_FURIHI_HANI_CHECK() = False Then
            MessageBox.Show("(年間スケジュール)" & vbCrLf & "振替契約期間（" & GAKKOU_INFO.KAISI_DATE & "〜" & GAKKOU_INFO.SYURYOU_DATE & "）外の月です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        'スケジュール区分の共通変数設定
        STRスケ区分 = "0"

        '振替区分の共通変数設定
        STR振替区分 = "0"

        '入力振替日の共通変数設定
        If s振替日 = "" Then
            STR年間入力振替日 = Space(15)
        Else
            STR年間入力振替日 = s振替日
        End If

        '-----------------------------------------------
        '2006/07/26　企業自振の初振のスケジュールも作成
        '-----------------------------------------------
        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        '既に登録されているかチェック
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------START
        sql.Append("FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(Int(s月)).Furikae_Day, "/", "") & "'")
        'sql.Append("FURI_DATE_S = '" & STR振替日 & "'")
        '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------END

        '読込のみ
        If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
        Else     'スケジュールが存在しない
            'コメント 2006/12/11
            'If intPUSH_BTN = 2 Then '更新時
            '    MessageBox.Show("企業自振側のスケジュール(" & STR請求年月.Substring(0, 4) & "年" & STR請求年月.Substring(4, 2) & "月分)が存在しません" & vbCrLf & "企業自振側で月間スケジュール作成後、" & vbCrLf & "学校スケジュールの更新処理を再度行ってください", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            'Else
            'スケジュール作成
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '取引先マスタに取引先コードが存在することを確認
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                        gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                '2010/10/21 契約振替日対応 引数に追加
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------START
                If fn_INSERTSCHMAST(strGakkouCode, "01", Replace(SYOKI_NENKAN_SCHINFO(Int(s月)).Furikae_Day, "/", ""), gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                    'If fn_INSERTSCHMAST(strGakkouCode, "01", STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                    '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------END
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", Err.Description)
                    MessageBox.Show("企業自振のスケジュールが登録できませんでした。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Return False
                End If
            End If
            'End If

        End If
        oraReader.Close()
        '-----------------------------------------------

        '再振レコードの作成
        If STR再振替日 <> "00000000" Then

            '2006/11/30　updateフラグの初期化
            updade = False

            '初振で求めた再振日を振替日に設定
            STR振替日 = STR再振替日

            '2010/10/21 再振の契約振替日を算出する
            '2011/06/16 標準版修正 再振日が入力されている場合は実振替日を契約振替日とする ------------------START
            'STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "0", "1")
            If s再振替日 = "" Then
                STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "0", "1")
            Else
                STR契約振替日 = STR振替日
            End If
            '2011/06/16 標準版修正 再振日が入力されている場合は実振替日を契約振替日とする ------------------END

            '再振日の算出
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(再振有/繰越無)
                    STR再振替日 = "00000000"
                Case "2"    '2(再振有/繰越有)   次回初振日を設定
                    '2011/06/16 標準版修正 実際の振替日から再振日を算出する ------------------START
                    'If s再振替日 = "" Then
                    '    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(strSFURI_DT))
                    'Else
                    '    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(s再振替日))
                    'End If
                                        STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(STR振替日)
                    '2011/06/16 標準版修正 実際の振替日から再振日を算出する ------------------END
            End Select

            'スケジュール区分の共通変数設定
            STRスケ区分 = "0"
            '振替区分の共通変数設定
            STR振替区分 = "1"
            '入力振替日の共通変数設定
            If s再振替日 = "" Then
                STR年間入力振替日 = Space(15)
            Else
                STR年間入力振替日 = s再振替日
            End If

            STR年間入力振替日 = Space(15)

            '-----------------------------------------------
            '2006/07/26　企業自振の再振のスケジュールも作成
            '-----------------------------------------------
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)

            '既に登録されているかチェック
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------START
            sql.Append("FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(Int(s月)).SaiFurikae_Day, "/", "") & "'")
            'sql.Append("FURI_DATE_S = '" & STR振替日 & "'")
            '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------END

            '読込のみ
            If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
            Else     'スケジュールが存在しない
                'コメント 2006/12/11
                'If intPUSH_BTN = 2 Then '更新時
                '    MessageBox.Show("企業自振側のスケジュール(" & STR請求年月.Substring(0, 4) & "年" & STR請求年月.Substring(4, 2) & "月分)が存在しません" & vbCrLf & "企業自振側で月間スケジュール作成後、" & vbCrLf & "学校スケジュールの更新処理を再度行ってください", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'Else
                'スケジュール作成
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '取引先マスタに取引先コードが存在することを確認
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                            gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                    '2010/10/21 契約振替日対応 引数に追加
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------START
                    If fn_INSERTSCHMAST(strGakkouCode, "02", Replace(SYOKI_NENKAN_SCHINFO(Int(s月)).SaiFurikae_Day, "/", ""), gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                        'If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                        '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------END
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", Err.Description)
                        MessageBox.Show("企業自振のスケジュールが登録できませんでした。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If

        PFUNC_NENKAN_SAKUSEI_SUB_KIGYO = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO(ByVal s請求月 As String, ByVal s月 As String, ByVal s振替日 As String, ByVal s再振替月 As String, ByVal s再振替日 As String) As Boolean
        'スケジュール　通常レコード作成

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO = False

        '初振レコードの作成

        '請求年月の作成
        STR請求年月 = PFUNC_SEIKYUTUKIHI(s請求月)

        '振替日算出
        STR振替日 = PFUNC_FURIHI_MAKE(s月, s振替日, "0", "0")

        '2010/10/21 契約振替日を算出する
        STR契約振替日 = PFUNC_KFURIHI_MAKE(s月, s振替日, "0", "0")

        '再振日の翌月判定と再振替年、再振替月設定
        '再振替日が入力され、かつ年間スケジュールのチェックボックスがＯＮ（s再振替月に月が設定）の場合
        If s再振替月 <> "" And s再振替日 <> "" Then
            STRW再振替年 = Mid(STR振替日, 1, 4)

            '2011/06/16 標準版修正 年を考慮する ------------------START
            'If Mid(STR振替日, 7, 2) <= s再振替日 Then
            If STR振替日 <= STR請求年月 & s再振替日 Then            
                '2011/06/16 標準版修正 年を考慮する ------------------END
                STRW再振替月 = s再振替月
                STRW再振替日 = s再振替日
            Else
                If s月 = "12" Then
                    STRW再振替月 = "01"
                    STRW再振替年 = CStr(CInt(Mid(STR請求年月, 1, 4)) + 1)

                Else
                    STRW再振替月 = Format((CInt(s再振替月) + 1), "00")
                End If
                STRW再振替日 = s再振替日
            End If
        End If

        '再振替日が入力なし、かつ年間スケジュールのチェックボックスがＯＮ
        If s再振替月 <> "" And s再振替日 = "" Then

            STRW再振替年 = Mid(STR振替日, 1, 4)

            '2011/06/16 標準版修正 年を考慮する ------------------START
            'If Mid(STR振替日, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
            If STR振替日 <= STR請求年月 & GAKKOU_INFO.SFURI_DATE Then
                '2011/06/16 標準版修正 年を考慮する ------------------END
                STRW再振替月 = s再振替月
                STRW再振替日 = GAKKOU_INFO.SFURI_DATE
            Else
                If s月 = "12" Then
                    STRW再振替月 = "01"
                    STRW再振替年 = CStr(CInt(Mid(STR請求年月, 1, 4)) + 1)
                Else
                    STRW再振替月 = Format((CInt(s再振替月) + 1), "00")
                End If
                STRW再振替日 = GAKKOU_INFO.SFURI_DATE
            End If
        End If

        If s再振替月 = "" Then
            STR再振替日 = "00000000"
        Else
            '再振替日算出
            STR再振替日 = PFUNC_SAIFURIHI_MAKE(Trim(STRW再振替月), Trim(STRW再振替日))
        End If

        '営業日を算出した結果で振替日と再振替日が同一になる場合がある為
        If STR振替日 = STR再振替日 Then
            MessageBox.Show("(年間スケジュール)" & vbCrLf & Mid(STR振替日, 5, 2) & "月の" & "振替日と再振替日が同一です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR振替日
        Str_SFURI_DATE = STR再振替日

        '再振レコードの作成
        If STR再振替日 <> "00000000" Then

            '初振で求めた再振日を振替日に設定
            STR振替日 = STR再振替日

            '2010/10/21 再振の契約振替日を算出する
            '2011/06/16 標準版修正 再振日が入力されている場合は実振替日を契約振替日とする ------------------START
            'STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "0", "1")
            If s再振替日 = "" Then
                STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "0", "1")
            Else
                STR契約振替日 = STR振替日
            End If
            '2011/06/16 標準版修正 再振日が入力されている場合は実振替日を契約振替日とする ------------------END

            '再振日の算出
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(再振有/繰越無)
                    STR再振替日 = "00000000"
                Case "2"    '2(再振有/繰越有)   次回初振日を設定
                    '2011/06/16 標準版修正 実際の振替日から再振日を算出する ------------------START
                    ''s再振替日に学校マスタ２の再振替日をセット 2005/12/09
                    'If s再振替日 = "" Then
                    '    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(strSFURI_DT))
                    'Else
                    '    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(s再振替日))
                    'End If
                                        STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(STR振替日)
                    '2011/06/16 標準版修正 実際の振替日から再振日を算出する ------------------END
            End Select

            'スケジュール区分の共通変数設定
            STRスケ区分 = "0"
            '振替区分の共通変数設定
            STR振替区分 = "1"
            '入力振替日の共通変数設定
            If s再振替日 = "" Then
                STR年間入力振替日 = Space(15)
            Else
                STR年間入力振替日 = s再振替日
            End If

            '既に登録されているかチェック
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------START
            sql.Append("FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(Int(s月)).SaiFurikae_Day, "/", "") & "'")
            'sql.Append("FURI_DATE_S = '" & STR振替日 & "'")
            '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------END

            '読込のみ
            If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
            Else     'スケジュールが存在しない
                'スケジュール作成
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '取引先マスタに取引先コードが存在することを確認
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                            gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                    '2010/10/21 契約振替日対応 引数に追加
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------START
                    If fn_INSERTSCHMAST(strGakkouCode, "02", Replace(SYOKI_NENKAN_SCHINFO(Int(s月)).SaiFurikae_Day, "/", ""), gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                        'If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                        '2011/06/16 標準版修正 口振スケジュールは画面に表示された値を基準に検索 ------------------END
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", Err.Description)
                        MessageBox.Show("企業自振のスケジュールが登録できませんでした。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
            End If
            oraReader.Close()
        End If

        PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO = True

    End Function


    Private Function PFUNC_NENKAN_KOUSIN() As Boolean

        '年間スケジュール　更新処理

        '削除処理（DELETE） 2006/11/30
        If PFUNC_NENKAN_DELETE() = False Then
            Return False
        End If

        '作成処理（INSERT)
        If PFUNC_NENKAN_SAKUSEI() = False Then
            Return False
        End If

        Return True

    End Function

    '================================================
    '年間スケジュール削除　2006/11/30
    '================================================
    Private Function PFUNC_NENKAN_DELETE() As Boolean
        PFUNC_NENKAN_DELETE = False

        Dim sql As New StringBuilder(128)
        Dim orareader As New MyOracleReader(MainDB)

        Dim blnSakujo_Check As Boolean = False '2006/11/30

        '全削除処理、キーは学校コード、対象年度、スケジュール区分（０）、処理フラグ（０）
        sql.Append(" DELETE  FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S =0")
        sql.Append(" AND")
        sql.Append(" ENTRI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" CHECK_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" DATA_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" FUNOU_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" SAIFURI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" KESSAI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" TYUUDAN_FLG_S =0")

        '2006/11/30　条件追加（変更のあったデータのみ削除）=========================
        For i As Integer = 1 To 12
            '変更があり、チェックが外れているものを削除する
            If bln年間更新(i) = True And NENKAN_SCHINFO(i).Furikae_Check = False And Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", "") <> "" Then
                If blnSakujo_Check = True Then
                    sql.Append(" or")
                Else
                    sql.Append(" and(")
                End If

                '条件追加
                sql.Append(" FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", "") & "'")

                '再振のスケジュールも削除する
                If SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day <> "" Then
                    sql.Append(" or")
                    sql.Append(" FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "") & "'")
                End If

                bln年間更新(i) = False '変更フラグを降ろす
                blnSakujo_Check = True '削除フラグを立てる

            ElseIf bln年間更新(i) = True And NENKAN_SCHINFO(i).SaiFurikae_Check = False And SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day <> "" Then
                If blnSakujo_Check = True Then
                    sql.Append(" or")
                Else
                    sql.Append(" and(")
                End If

                '条件追加
                sql.Append(" FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "") & "'")

                '再振のみ削除した場合、初振のスケジュールも変更が必要なので変更フラグは降ろさない
                blnSakujo_Check = True '削除フラグを立てる

            End If
        Next

        If blnSakujo_Check = True Then
            sql.Append(")")
            '削除データがある場合のみ実行する
            If MainDB.ExecuteNonQuery(sql) < 0 Then
                '削除処理エラー
                MessageBox.Show("(年間スケジュール)" & vbCrLf & "スケジュールの削除処理でエラーが発生しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        End If

        PFUNC_NENKAN_DELETE = True

    End Function

#End Region

#Region " Private Sub(特別スケジュール)"
    Private Sub PSUB_TOKUBETU_GET(ByRef Get_Data() As TokubetuData)

        Get_Data(1).Seikyu_Tuki = txt特別請求月１.Text
        Get_Data(1).Furikae_Tuki = txt特別振替月１.Text
        Get_Data(1).Furikae_Date = txt特別振替日１.Text
        Get_Data(1).SaiFurikae_Tuki = txt特別再振替月１.Text
        Get_Data(1).SaiFurikae_Date = txt特別再振替日１.Text

        Select Case chk１_全学年.Checked
            Case True
                Get_Data(1).SiyouGakunenALL_Check = True
                Get_Data(1).SiyouGakunen1_Check = True
                Get_Data(1).SiyouGakunen2_Check = True
                Get_Data(1).SiyouGakunen3_Check = True
                Get_Data(1).SiyouGakunen4_Check = True
                Get_Data(1).SiyouGakunen5_Check = True
                Get_Data(1).SiyouGakunen6_Check = True
                Get_Data(1).SiyouGakunen7_Check = True
                Get_Data(1).SiyouGakunen8_Check = True
                Get_Data(1).SiyouGakunen9_Check = True
            Case False
                Get_Data(1).SiyouGakunenALL_Check = False
                Get_Data(1).SiyouGakunen1_Check = chk１_１学年.Checked
                Get_Data(1).SiyouGakunen2_Check = chk１_２学年.Checked
                Get_Data(1).SiyouGakunen3_Check = chk１_３学年.Checked
                Get_Data(1).SiyouGakunen4_Check = chk１_４学年.Checked
                Get_Data(1).SiyouGakunen5_Check = chk１_５学年.Checked
                Get_Data(1).SiyouGakunen6_Check = chk１_６学年.Checked
                Get_Data(1).SiyouGakunen7_Check = chk１_７学年.Checked
                Get_Data(1).SiyouGakunen8_Check = chk１_８学年.Checked
                Get_Data(1).SiyouGakunen9_Check = chk１_９学年.Checked
        End Select


        Get_Data(2).Seikyu_Tuki = txt特別請求月２.Text
        Get_Data(2).Furikae_Tuki = txt特別振替月２.Text
        Get_Data(2).Furikae_Date = txt特別振替日２.Text
        Get_Data(2).SaiFurikae_Tuki = txt特別再振替月２.Text
        Get_Data(2).SaiFurikae_Date = txt特別再振替日２.Text

        Select Case chk２_全学年.Checked
            Case True
                Get_Data(2).SiyouGakunenALL_Check = True
                Get_Data(2).SiyouGakunen1_Check = True
                Get_Data(2).SiyouGakunen2_Check = True
                Get_Data(2).SiyouGakunen3_Check = True
                Get_Data(2).SiyouGakunen4_Check = True
                Get_Data(2).SiyouGakunen5_Check = True
                Get_Data(2).SiyouGakunen6_Check = True
                Get_Data(2).SiyouGakunen7_Check = True
                Get_Data(2).SiyouGakunen8_Check = True
                Get_Data(2).SiyouGakunen9_Check = True
            Case False
                Get_Data(2).SiyouGakunenALL_Check = False
                Get_Data(2).SiyouGakunen1_Check = chk２_１学年.Checked
                Get_Data(2).SiyouGakunen2_Check = chk２_２学年.Checked
                Get_Data(2).SiyouGakunen3_Check = chk２_３学年.Checked
                Get_Data(2).SiyouGakunen4_Check = chk２_４学年.Checked
                Get_Data(2).SiyouGakunen5_Check = chk２_５学年.Checked
                Get_Data(2).SiyouGakunen6_Check = chk２_６学年.Checked
                Get_Data(2).SiyouGakunen7_Check = chk２_７学年.Checked
                Get_Data(2).SiyouGakunen8_Check = chk２_８学年.Checked
                Get_Data(2).SiyouGakunen9_Check = chk２_９学年.Checked
        End Select


        Get_Data(3).Seikyu_Tuki = txt特別請求月３.Text
        Get_Data(3).Furikae_Tuki = txt特別振替月３.Text
        Get_Data(3).Furikae_Date = txt特別振替日３.Text
        Get_Data(3).SaiFurikae_Tuki = txt特別再振替月３.Text
        Get_Data(3).SaiFurikae_Date = txt特別再振替日３.Text

        Select Case chk３_全学年.Checked
            Case True
                Get_Data(3).SiyouGakunenALL_Check = True
                Get_Data(3).SiyouGakunen1_Check = True
                Get_Data(3).SiyouGakunen2_Check = True
                Get_Data(3).SiyouGakunen3_Check = True
                Get_Data(3).SiyouGakunen4_Check = True
                Get_Data(3).SiyouGakunen5_Check = True
                Get_Data(3).SiyouGakunen6_Check = True
                Get_Data(3).SiyouGakunen7_Check = True
                Get_Data(3).SiyouGakunen8_Check = True
                Get_Data(3).SiyouGakunen9_Check = True
            Case False
                Get_Data(3).SiyouGakunenALL_Check = False
                Get_Data(3).SiyouGakunen1_Check = chk３_１学年.Checked
                Get_Data(3).SiyouGakunen2_Check = chk３_２学年.Checked
                Get_Data(3).SiyouGakunen3_Check = chk３_３学年.Checked
                Get_Data(3).SiyouGakunen4_Check = chk３_４学年.Checked
                Get_Data(3).SiyouGakunen5_Check = chk３_５学年.Checked
                Get_Data(3).SiyouGakunen6_Check = chk３_６学年.Checked
                Get_Data(3).SiyouGakunen7_Check = chk３_７学年.Checked
                Get_Data(3).SiyouGakunen8_Check = chk３_８学年.Checked
                Get_Data(3).SiyouGakunen9_Check = chk３_９学年.Checked
        End Select


        Get_Data(4).Seikyu_Tuki = txt特別請求月４.Text
        Get_Data(4).Furikae_Tuki = txt特別振替月４.Text
        Get_Data(4).Furikae_Date = txt特別振替日４.Text
        Get_Data(4).SaiFurikae_Tuki = txt特別再振替月４.Text
        Get_Data(4).SaiFurikae_Date = txt特別再振替日４.Text

        Select Case chk４_全学年.Checked
            Case True
                Get_Data(4).SiyouGakunenALL_Check = True
                Get_Data(4).SiyouGakunen1_Check = True
                Get_Data(4).SiyouGakunen2_Check = True
                Get_Data(4).SiyouGakunen3_Check = True
                Get_Data(4).SiyouGakunen4_Check = True
                Get_Data(4).SiyouGakunen5_Check = True
                Get_Data(4).SiyouGakunen6_Check = True
                Get_Data(4).SiyouGakunen7_Check = True
                Get_Data(4).SiyouGakunen8_Check = True
                Get_Data(4).SiyouGakunen9_Check = True
            Case False
                Get_Data(4).SiyouGakunenALL_Check = False
                Get_Data(4).SiyouGakunen1_Check = chk４_１学年.Checked
                Get_Data(4).SiyouGakunen2_Check = chk４_２学年.Checked
                Get_Data(4).SiyouGakunen3_Check = chk４_３学年.Checked
                Get_Data(4).SiyouGakunen4_Check = chk４_４学年.Checked
                Get_Data(4).SiyouGakunen5_Check = chk４_５学年.Checked
                Get_Data(4).SiyouGakunen6_Check = chk４_６学年.Checked
                Get_Data(4).SiyouGakunen7_Check = chk４_７学年.Checked
                Get_Data(4).SiyouGakunen8_Check = chk４_８学年.Checked
                Get_Data(4).SiyouGakunen9_Check = chk４_９学年.Checked
        End Select


        Get_Data(5).Seikyu_Tuki = txt特別請求月５.Text
        Get_Data(5).Furikae_Tuki = txt特別振替月５.Text
        Get_Data(5).Furikae_Date = txt特別振替日５.Text
        Get_Data(5).SaiFurikae_Tuki = txt特別再振替月５.Text
        Get_Data(5).SaiFurikae_Date = txt特別再振替日５.Text

        Select Case chk５_全学年.Checked
            Case True
                Get_Data(5).SiyouGakunenALL_Check = True
                Get_Data(5).SiyouGakunen1_Check = True
                Get_Data(5).SiyouGakunen2_Check = True
                Get_Data(5).SiyouGakunen3_Check = True
                Get_Data(5).SiyouGakunen4_Check = True
                Get_Data(5).SiyouGakunen5_Check = True
                Get_Data(5).SiyouGakunen6_Check = True
                Get_Data(5).SiyouGakunen7_Check = True
                Get_Data(5).SiyouGakunen8_Check = True
                Get_Data(5).SiyouGakunen9_Check = True
            Case False
                Get_Data(5).SiyouGakunenALL_Check = False
                Get_Data(5).SiyouGakunen1_Check = chk５_１学年.Checked
                Get_Data(5).SiyouGakunen2_Check = chk５_２学年.Checked
                Get_Data(5).SiyouGakunen3_Check = chk５_３学年.Checked
                Get_Data(5).SiyouGakunen4_Check = chk５_４学年.Checked
                Get_Data(5).SiyouGakunen5_Check = chk５_５学年.Checked
                Get_Data(5).SiyouGakunen6_Check = chk５_６学年.Checked
                Get_Data(5).SiyouGakunen7_Check = chk５_７学年.Checked
                Get_Data(5).SiyouGakunen8_Check = chk５_８学年.Checked
                Get_Data(5).SiyouGakunen9_Check = chk５_９学年.Checked
        End Select

        Get_Data(6).Seikyu_Tuki = txt特別請求月６.Text
        Get_Data(6).Furikae_Tuki = txt特別振替月６.Text
        Get_Data(6).Furikae_Date = txt特別振替日６.Text
        Get_Data(6).SaiFurikae_Tuki = txt特別再振替月６.Text
        Get_Data(6).SaiFurikae_Date = txt特別再振替日６.Text

        Select Case chk６_全学年.Checked
            Case True
                Get_Data(6).SiyouGakunenALL_Check = True
                Get_Data(6).SiyouGakunen1_Check = True
                Get_Data(6).SiyouGakunen2_Check = True
                Get_Data(6).SiyouGakunen3_Check = True
                Get_Data(6).SiyouGakunen4_Check = True
                Get_Data(6).SiyouGakunen5_Check = True
                Get_Data(6).SiyouGakunen6_Check = True
                Get_Data(6).SiyouGakunen7_Check = True
                Get_Data(6).SiyouGakunen8_Check = True
                Get_Data(6).SiyouGakunen9_Check = True
            Case False
                Get_Data(6).SiyouGakunenALL_Check = False
                Get_Data(6).SiyouGakunen1_Check = chk６_１学年.Checked
                Get_Data(6).SiyouGakunen2_Check = chk６_２学年.Checked
                Get_Data(6).SiyouGakunen3_Check = chk６_３学年.Checked
                Get_Data(6).SiyouGakunen4_Check = chk６_４学年.Checked
                Get_Data(6).SiyouGakunen5_Check = chk６_５学年.Checked
                Get_Data(6).SiyouGakunen6_Check = chk６_６学年.Checked
                Get_Data(6).SiyouGakunen7_Check = chk６_７学年.Checked
                Get_Data(6).SiyouGakunen8_Check = chk６_８学年.Checked
                Get_Data(6).SiyouGakunen9_Check = chk６_９学年.Checked
        End Select

    End Sub

#End Region

#Region " Private Sub(特別スケジュール画面制御)"
    Private Sub PSUB_TOKUBETU_FORMAT(Optional ByVal pIndex As Integer = 1)

        'Select case pIndex
        '    Case 0
        '対象学年チェックＢＯＸの有効化
        Call PSUB_TOKUBETU_CHKBOXEnabled(True)
        'End Select

        '処理対象学年指定チェックOFF
        Call PSUB_TOKUBETU_CHK(False)

        '振替日入力欄、再振替日入力欄のクリア
        Call PSUB_TOKUBETU_DAYCLER()

    End Sub
    Private Sub PSUB_TOKUBETU_CHKBOXEnabled(ByVal pValue As Boolean)

        '対象学年チェックBOXの有効化
        chk１_１学年.Enabled = pValue
        chk１_２学年.Enabled = pValue
        chk１_３学年.Enabled = pValue
        chk１_４学年.Enabled = pValue
        chk１_５学年.Enabled = pValue
        chk１_６学年.Enabled = pValue
        chk１_７学年.Enabled = pValue
        chk１_８学年.Enabled = pValue
        chk１_９学年.Enabled = pValue
        chk１_全学年.Enabled = pValue

        chk２_１学年.Enabled = pValue
        chk２_２学年.Enabled = pValue
        chk２_３学年.Enabled = pValue
        chk２_４学年.Enabled = pValue
        chk２_５学年.Enabled = pValue
        chk２_６学年.Enabled = pValue
        chk２_７学年.Enabled = pValue
        chk２_８学年.Enabled = pValue
        chk２_９学年.Enabled = pValue
        chk２_全学年.Enabled = pValue

        chk３_１学年.Enabled = pValue
        chk３_２学年.Enabled = pValue
        chk３_３学年.Enabled = pValue
        chk３_４学年.Enabled = pValue
        chk３_５学年.Enabled = pValue
        chk３_６学年.Enabled = pValue
        chk３_７学年.Enabled = pValue
        chk３_８学年.Enabled = pValue
        chk３_９学年.Enabled = pValue
        chk３_全学年.Enabled = pValue

        chk４_１学年.Enabled = pValue
        chk４_２学年.Enabled = pValue
        chk４_３学年.Enabled = pValue
        chk４_４学年.Enabled = pValue
        chk４_５学年.Enabled = pValue
        chk４_６学年.Enabled = pValue
        chk４_７学年.Enabled = pValue
        chk４_８学年.Enabled = pValue
        chk４_９学年.Enabled = pValue
        chk４_全学年.Enabled = pValue

        chk５_１学年.Enabled = pValue
        chk５_２学年.Enabled = pValue
        chk５_３学年.Enabled = pValue
        chk５_４学年.Enabled = pValue
        chk５_５学年.Enabled = pValue
        chk５_６学年.Enabled = pValue
        chk５_７学年.Enabled = pValue
        chk５_８学年.Enabled = pValue
        chk５_９学年.Enabled = pValue
        chk５_全学年.Enabled = pValue

        chk６_１学年.Enabled = pValue
        chk６_２学年.Enabled = pValue
        chk６_３学年.Enabled = pValue
        chk６_４学年.Enabled = pValue
        chk６_５学年.Enabled = pValue
        chk６_６学年.Enabled = pValue
        chk６_７学年.Enabled = pValue
        chk６_８学年.Enabled = pValue
        chk６_９学年.Enabled = pValue
        chk６_全学年.Enabled = pValue

    End Sub
    Private Sub PSUB_TOKUBETU_DAYCLER()

        '請求月のクリア処理
        txt特別請求月１.Text = ""
        txt特別請求月２.Text = ""
        txt特別請求月３.Text = ""
        txt特別請求月４.Text = ""
        txt特別請求月５.Text = ""
        txt特別請求月６.Text = ""

        '振替日のクリア処理
        txt特別振替月１.Text = ""
        txt特別振替日１.Text = ""
        txt特別振替月２.Text = ""
        txt特別振替日２.Text = ""
        txt特別振替月３.Text = ""
        txt特別振替日３.Text = ""
        txt特別振替月４.Text = ""
        txt特別振替日４.Text = ""
        txt特別振替月５.Text = ""
        txt特別振替日５.Text = ""
        txt特別振替月６.Text = ""
        txt特別振替日６.Text = ""

        '再振替日のクリア処理
        txt特別再振替月１.Text = ""
        txt特別再振替日１.Text = ""
        txt特別再振替月２.Text = ""
        txt特別再振替日２.Text = ""
        txt特別再振替月３.Text = ""
        txt特別再振替日３.Text = ""
        txt特別再振替月４.Text = ""
        txt特別再振替日４.Text = ""
        txt特別再振替月５.Text = ""
        txt特別再振替日５.Text = ""
        txt特別再振替月６.Text = ""
        txt特別再振替日６.Text = ""

    End Sub
    Private Sub PSUB_TOKUBETU_CHK(ByVal pValue As Boolean)

        '対象学年有効チェックOFF
        chk１_１学年.Checked = pValue
        chk１_２学年.Checked = pValue
        chk１_３学年.Checked = pValue
        chk１_４学年.Checked = pValue
        chk１_５学年.Checked = pValue
        chk１_６学年.Checked = pValue
        chk１_７学年.Checked = pValue
        chk１_８学年.Checked = pValue
        chk１_９学年.Checked = pValue
        chk１_全学年.Checked = pValue

        chk２_１学年.Checked = pValue
        chk２_２学年.Checked = pValue
        chk２_３学年.Checked = pValue
        chk２_４学年.Checked = pValue
        chk２_５学年.Checked = pValue
        chk２_６学年.Checked = pValue
        chk２_７学年.Checked = pValue
        chk２_８学年.Checked = pValue
        chk２_９学年.Checked = pValue
        chk２_全学年.Checked = pValue

        chk３_１学年.Checked = pValue
        chk３_２学年.Checked = pValue
        chk３_３学年.Checked = pValue
        chk３_４学年.Checked = pValue
        chk３_５学年.Checked = pValue
        chk３_６学年.Checked = pValue
        chk３_７学年.Checked = pValue
        chk３_８学年.Checked = pValue
        chk３_９学年.Checked = pValue
        chk３_全学年.Checked = pValue

        chk４_１学年.Checked = pValue
        chk４_２学年.Checked = pValue
        chk４_３学年.Checked = pValue
        chk４_４学年.Checked = pValue
        chk４_５学年.Checked = pValue
        chk４_６学年.Checked = pValue
        chk４_７学年.Checked = pValue
        chk４_８学年.Checked = pValue
        chk４_９学年.Checked = pValue
        chk４_全学年.Checked = pValue

        chk５_１学年.Checked = pValue
        chk５_２学年.Checked = pValue
        chk５_３学年.Checked = pValue
        chk５_４学年.Checked = pValue
        chk５_５学年.Checked = pValue
        chk５_６学年.Checked = pValue
        chk５_７学年.Checked = pValue
        chk５_８学年.Checked = pValue
        chk５_９学年.Checked = pValue
        chk５_全学年.Checked = pValue

        chk６_１学年.Checked = pValue
        chk６_２学年.Checked = pValue
        chk６_３学年.Checked = pValue
        chk６_４学年.Checked = pValue
        chk６_５学年.Checked = pValue
        chk６_６学年.Checked = pValue
        chk６_７学年.Checked = pValue
        chk６_８学年.Checked = pValue
        chk６_９学年.Checked = pValue
        chk６_全学年.Checked = pValue

    End Sub

    Private Sub PSUB_TOKUBETU_SET(ByVal txtbox請求月 As TextBox, ByVal txtbox月 As TextBox, ByVal txtbox日 As TextBox, ByVal chkbox1 As CheckBox, ByVal chkbox2 As CheckBox, ByVal chkbox3 As CheckBox, ByVal chkbox4 As CheckBox, ByVal chkbox5 As CheckBox, ByVal chkbox6 As CheckBox, ByVal chkbox7 As CheckBox, ByVal chkbox8 As CheckBox, ByVal chkbox9 As CheckBox, ByVal chkboxALL As CheckBox, ByVal aReader As MyOracleReader)

        '特別振替日　参照ボタン共通編集

        '請求月の設定
        txtbox請求月.Text = Mid(aReader.GetString("NENGETUDO_S"), 5, 2)

        '振替月の設定
        txtbox月.Text = Mid(aReader.GetString("FURI_DATE_S"), 5, 2)

        '振替日の設定
        txtbox日.Text = Mid(aReader.GetString("FURI_DATE_S"), 7, 2)

        Select Case CInt(aReader.GetString("FURI_KBN_S"))
            Case 0
                Select Case True
                    Case aReader.GetString("ENTRI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("CHECK_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("DATA_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("FUNOU_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("SAIFURI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("KESSAI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("TYUUDAN_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                End Select
            Case 1
                Select Case True
                    Case aReader.GetString("ENTRI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("CHECK_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                        '2006/11/30　チェックフラグを取得
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                    Case aReader.GetString("DATA_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("FUNOU_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("SAIFURI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("KESSAI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("TYUUDAN_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                End Select
        End Select

        If aReader.GetString("GAKUNEN1_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN2_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN3_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN4_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN5_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN6_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN7_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN8_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN9_FLG_S") = "1" Then

            '全学年チェックボックスＯＮ
            chkboxALL.Checked = True

            '１から９学年チェックボクスの使用不可
            chkbox1.Enabled = False
            chkbox2.Enabled = False
            chkbox3.Enabled = False
            chkbox4.Enabled = False
            chkbox5.Enabled = False
            chkbox6.Enabled = False
            chkbox7.Enabled = False
            chkbox8.Enabled = False
            chkbox9.Enabled = False
        Else
            If aReader.GetString("GAKUNEN1_FLG_S") = "1" Then
                '１学年チェックボックスＯＮ
                chkbox1.Checked = True
            Else
                chkbox1.Checked = False
            End If

            If aReader.GetString("GAKUNEN2_FLG_S") = "1" Then
                '２学年チェックボックスＯＮ
                chkbox2.Checked = True
            Else
                chkbox2.Checked = False
            End If

            If aReader.GetString("GAKUNEN3_FLG_S") = "1" Then
                '３学年チェックボックスＯＮ
                chkbox3.Checked = True
            Else
                chkbox3.Checked = False
            End If

            If aReader.GetString("GAKUNEN4_FLG_S") = "1" Then
                '４学年チェックボックスＯＮ
                chkbox4.Checked = True
            Else
                chkbox4.Checked = False
            End If

            If aReader.GetString("GAKUNEN5_FLG_S") = "1" Then
                '５学年チェックボックスＯＮ
                chkbox5.Checked = True
            Else
                chkbox5.Checked = False
            End If

            If aReader.GetString("GAKUNEN6_FLG_S") = "1" Then
                '６学年チェックボックスＯＮ
                chkbox6.Checked = True
            Else
                chkbox6.Checked = False
            End If

            If aReader.GetString("GAKUNEN7_FLG_S") = "1" Then
                '７学年チェックボックスＯＮ
                chkbox7.Checked = True
            Else
                chkbox7.Checked = False
            End If

            If aReader.GetString("GAKUNEN8_FLG_S") = "1" Then
                '８学年チェックボックスＯＮ
                chkbox8.Checked = True
            Else
                chkbox8.Checked = False
            End If

            If aReader.GetString("GAKUNEN9_FLG_S") = "1" Then
                '９学年チェックボックスＯＮ
                chkbox9.Checked = True
            Else
                chkbox9.Checked = False
            End If
        End If

    End Sub
    Private Sub PSUB_TGAKUNEN_CHK()
        '2006/10/12　使用していない学年のチェックボックスを使用不可にする

        If GAKKOU_INFO.SIYOU_GAKUNEN <> 9 Then
            chk１_９学年.Enabled = False
            chk２_９学年.Enabled = False
            chk３_９学年.Enabled = False
            chk４_９学年.Enabled = False
            chk５_９学年.Enabled = False
            chk６_９学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 8 Then
            chk１_８学年.Enabled = False
            chk２_８学年.Enabled = False
            chk３_８学年.Enabled = False
            chk４_８学年.Enabled = False
            chk５_８学年.Enabled = False
            chk６_８学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 7 Then
            chk１_７学年.Enabled = False
            chk２_７学年.Enabled = False
            chk３_７学年.Enabled = False
            chk４_７学年.Enabled = False
            chk５_７学年.Enabled = False
            chk６_７学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 6 Then
            chk１_６学年.Enabled = False
            chk２_６学年.Enabled = False
            chk３_６学年.Enabled = False
            chk４_６学年.Enabled = False
            chk５_６学年.Enabled = False
            chk６_６学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 5 Then
            chk１_５学年.Enabled = False
            chk２_５学年.Enabled = False
            chk３_５学年.Enabled = False
            chk４_５学年.Enabled = False
            chk５_５学年.Enabled = False
            chk６_５学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 4 Then
            chk１_４学年.Enabled = False
            chk２_４学年.Enabled = False
            chk３_４学年.Enabled = False
            chk４_４学年.Enabled = False
            chk５_４学年.Enabled = False
            chk６_４学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 3 Then
            chk１_３学年.Enabled = False
            chk２_３学年.Enabled = False
            chk３_３学年.Enabled = False
            chk４_３学年.Enabled = False
            chk５_３学年.Enabled = False
            chk６_３学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 2 Then
            chk１_２学年.Enabled = False
            chk２_２学年.Enabled = False
            chk３_２学年.Enabled = False
            chk４_２学年.Enabled = False
            chk５_２学年.Enabled = False
            chk６_２学年.Enabled = False
        End If
    End Sub

    '=============================================
    '学年フラグを二次元配列に格納する　2006/11/30
    '=============================================
    Private Sub PSUB_GAKUNENFLG_GET(ByRef strGakunen_FLG(,) As Boolean)

        strGakunen_FLG(1, 1) = chk１_１学年.Checked
        strGakunen_FLG(1, 2) = chk１_２学年.Checked
        strGakunen_FLG(1, 3) = chk１_３学年.Checked
        strGakunen_FLG(1, 4) = chk１_４学年.Checked
        strGakunen_FLG(1, 5) = chk１_５学年.Checked
        strGakunen_FLG(1, 6) = chk１_６学年.Checked
        strGakunen_FLG(1, 7) = chk１_７学年.Checked
        strGakunen_FLG(1, 8) = chk１_８学年.Checked
        strGakunen_FLG(1, 9) = chk１_９学年.Checked
        strGakunen_FLG(1, 10) = chk１_全学年.Checked

        strGakunen_FLG(2, 1) = chk２_１学年.Checked
        strGakunen_FLG(2, 2) = chk２_２学年.Checked
        strGakunen_FLG(2, 3) = chk２_３学年.Checked
        strGakunen_FLG(2, 4) = chk２_４学年.Checked
        strGakunen_FLG(2, 5) = chk２_５学年.Checked
        strGakunen_FLG(2, 6) = chk２_６学年.Checked
        strGakunen_FLG(2, 7) = chk２_７学年.Checked
        strGakunen_FLG(2, 8) = chk２_８学年.Checked
        strGakunen_FLG(2, 9) = chk２_９学年.Checked
        strGakunen_FLG(2, 10) = chk２_全学年.Checked

        strGakunen_FLG(3, 1) = chk３_１学年.Checked
        strGakunen_FLG(3, 2) = chk３_２学年.Checked
        strGakunen_FLG(3, 3) = chk３_３学年.Checked
        strGakunen_FLG(3, 4) = chk３_４学年.Checked
        strGakunen_FLG(3, 5) = chk３_５学年.Checked
        strGakunen_FLG(3, 6) = chk３_６学年.Checked
        strGakunen_FLG(3, 7) = chk３_７学年.Checked
        strGakunen_FLG(3, 8) = chk３_８学年.Checked
        strGakunen_FLG(3, 9) = chk３_９学年.Checked
        strGakunen_FLG(3, 10) = chk３_全学年.Checked

        strGakunen_FLG(4, 1) = chk４_１学年.Checked
        strGakunen_FLG(4, 2) = chk４_２学年.Checked
        strGakunen_FLG(4, 3) = chk４_３学年.Checked
        strGakunen_FLG(4, 4) = chk４_４学年.Checked
        strGakunen_FLG(4, 5) = chk４_５学年.Checked
        strGakunen_FLG(4, 6) = chk４_６学年.Checked
        strGakunen_FLG(4, 7) = chk４_７学年.Checked
        strGakunen_FLG(4, 8) = chk４_８学年.Checked
        strGakunen_FLG(4, 9) = chk４_９学年.Checked
        strGakunen_FLG(4, 10) = chk４_全学年.Checked

        strGakunen_FLG(5, 1) = chk５_１学年.Checked
        strGakunen_FLG(5, 2) = chk５_２学年.Checked
        strGakunen_FLG(5, 3) = chk５_３学年.Checked
        strGakunen_FLG(5, 4) = chk５_４学年.Checked
        strGakunen_FLG(5, 5) = chk５_５学年.Checked
        strGakunen_FLG(5, 6) = chk５_６学年.Checked
        strGakunen_FLG(5, 7) = chk５_７学年.Checked
        strGakunen_FLG(5, 8) = chk５_８学年.Checked
        strGakunen_FLG(5, 9) = chk５_９学年.Checked
        strGakunen_FLG(5, 10) = chk５_全学年.Checked

        strGakunen_FLG(6, 1) = chk６_１学年.Checked
        strGakunen_FLG(6, 2) = chk６_２学年.Checked
        strGakunen_FLG(6, 3) = chk６_３学年.Checked
        strGakunen_FLG(6, 4) = chk６_４学年.Checked
        strGakunen_FLG(6, 5) = chk６_５学年.Checked
        strGakunen_FLG(6, 6) = chk６_６学年.Checked
        strGakunen_FLG(6, 7) = chk６_７学年.Checked
        strGakunen_FLG(6, 8) = chk６_８学年.Checked
        strGakunen_FLG(6, 9) = chk６_９学年.Checked
        strGakunen_FLG(6, 10) = chk６_全学年.Checked

    End Sub

#End Region

#Region " Private Function(特別スケジュール)"
    Private Function PFUNC_SCH_GET_TOKUBETU() As Boolean

        PFUNC_SCH_GET_TOKUBETU = False

        '特別振替日
        '対象学年チェックＢＯＸの有効化
        Call PSUB_TOKUBETU_CHKBOXEnabled(True)

        '処理対象学年指定チェックOFF
        Call PSUB_TOKUBETU_CHK(False)

        '振替日入力欄、再振替日入力欄のクリア
        Call PSUB_TOKUBETU_DAYCLER()

        '特別振替日参照処理
        If PFUNC_TOKUBETU_SANSYOU() = False Then
            Exit Function
        End If

        PFUNC_SCH_GET_TOKUBETU = True

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_TOKUBETU() As Boolean

        '特別スケジュール更新処理
        If PFUNC_TOKUBETU_KOUSIN() = False Then

            'ここを通るということは１件でも処理したレコードが存在したということなので
            Int_Syori_Flag(1) = 2

            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_SCH_TOKUBETU_GET(ByVal strNENGETUDO As String, ByVal strFURIKUBUN As String) As Boolean


        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        Try

            PFUNC_SCH_TOKUBETU_GET = False

            '特別スケジュールのレコード存在チェック

            sql.Append(" SELECT * FROM G_SCHMAST")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
            sql.Append(" AND")
            sql.Append(" NENGETUDO_S = '" & strNENGETUDO & "'")
            sql.Append(" AND")
            sql.Append(" SCH_KBN_S = '1'")
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S = " & "'" & strFURIKUBUN & "'")

            If oraReader.DataReader(sql) = True Then '存在すれば

                '特別レコードの対象学年を元に、通常レコードの対象学年を設定し直す
                '※特別スケジュールで指定されている学年は年間スケジュールでは指定しない
                Do Until oraReader.EOF
                    If oraReader.GetString("GAKUNEN1_FLG_S") = "1" Then
                        STR１学年 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN2_FLG_S") = "1" Then
                        STR２学年 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN3_FLG_S") = "1" Then
                        STR３学年 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN4_FLG_S") = "1" Then
                        STR４学年 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN5_FLG_S") = "1" Then
                        STR５学年 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN6_FLG_S") = "1" Then
                        STR６学年 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN7_FLG_S") = "1" Then
                        STR７学年 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN8_FLG_S") = "1" Then
                        STR８学年 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN9_FLG_S") = "1" Then
                        STR９学年 = "0"
                    End If
                    oraReader.NextRead()
                Loop

            Else    '存在しなければTrue
                PFUNC_SCH_TOKUBETU_GET = True
                Return True
            End If

            PFUNC_SCH_TOKUBETU_GET = True

        Catch ex As Exception
            Throw
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

    End Function

    Private Function PFUNC_TOKUBETU_SANSYOU() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '特別振替日　参照処理
        PFUNC_TOKUBETU_SANSYOU = False

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = 1")
        sql.Append(" ORDER BY FURI_KBN_S asc , FURI_DATE_S ASC")

        If oraReader.DataReader(sql) = False Then
            oraReader.Close()
            Exit Function
        End If

        Do Until oraReader.EOF

            Select Case oraReader.GetString("FURI_KBN_S")
                Case "0"
                    'まだ値が設定されていない行に特別スケジュールを設定する
                    Select Case True
                        Case (txt特別振替月１.Text = "")
                            Int_Tokubetu_Flag = 1
                            Call PSUB_TOKUBETU_SET(txt特別請求月１, txt特別振替月１, txt特別振替日１, chk１_１学年, chk１_２学年, chk１_３学年, chk１_４学年, chk１_５学年, chk１_６学年, chk１_７学年, chk１_８学年, chk１_９学年, chk１_全学年, oraReader)

                            '振替日と再振替日の表示上の対応関係（セット）をとるため、タグに振替日レコード中の再振替日を一時保存する
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt特別振替月１.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            '2006/11/30　チェックフラグを取得
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            '2006/11/30　不能フラグを取得
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt特別振替月２.Text = "")
                            Int_Tokubetu_Flag = 2
                            Call PSUB_TOKUBETU_SET(txt特別請求月２, txt特別振替月２, txt特別振替日２, chk２_１学年, chk２_２学年, chk２_３学年, chk２_４学年, chk２_５学年, chk２_６学年, chk２_７学年, chk２_８学年, chk２_９学年, chk２_全学年, oraReader)

                            '振替日と再振替日の表示上の対応関係（セット）をとるため、タグに振替日レコード中の再振替日を一時保存する
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt特別振替月２.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt特別振替月３.Text = "")
                            Int_Tokubetu_Flag = 3
                            Call PSUB_TOKUBETU_SET(txt特別請求月３, txt特別振替月３, txt特別振替日３, chk３_１学年, chk３_２学年, chk３_３学年, chk３_４学年, chk３_５学年, chk３_６学年, chk３_７学年, chk３_８学年, chk３_９学年, chk３_全学年, oraReader)

                            '振替日と再振替日の表示上の対応関係（セット）をとるため、タグに振替日レコード中の再振替日を一時保存する
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt特別振替月３.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt特別振替月４.Text = "")
                            Int_Tokubetu_Flag = 4
                            Call PSUB_TOKUBETU_SET(txt特別請求月４, txt特別振替月４, txt特別振替日４, chk４_１学年, chk４_２学年, chk４_３学年, chk４_４学年, chk４_５学年, chk４_６学年, chk４_７学年, chk４_８学年, chk４_９学年, chk４_全学年, oraReader)

                            '振替日と再振替日の表示上の対応関係（セット）をとるため、タグに振替日レコード中の再振替日を一時保存する
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt特別振替月４.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt特別振替月５.Text = "")
                            Int_Tokubetu_Flag = 5
                            Call PSUB_TOKUBETU_SET(txt特別請求月５, txt特別振替月５, txt特別振替日５, chk５_１学年, chk５_２学年, chk５_３学年, chk５_４学年, chk５_５学年, chk５_６学年, chk５_７学年, chk５_８学年, chk５_９学年, chk５_全学年, oraReader)

                            '振替日と再振替日の表示上の対応関係（セット）をとるため、タグに振替日レコード中の再振替日を一時保存する
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt特別振替月５.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt特別振替月６.Text = "")
                            Int_Tokubetu_Flag = 6
                            Call PSUB_TOKUBETU_SET(txt特別請求月６, txt特別振替月６, txt特別振替日６, chk６_１学年, chk６_２学年, chk６_３学年, chk６_４学年, chk６_５学年, chk６_６学年, chk６_７学年, chk６_８学年, chk６_９学年, chk６_全学年, oraReader)

                            '振替日と再振替日の表示上の対応関係（セット）をとるため、タグに振替日レコード中の再振替日を一時保存する
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt特別振替月６.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                    End Select

                Case "1"
                    Select Case oraReader.GetString("FURI_DATE_S")
                        Case txt特別振替月１.Tag
                            Int_Tokubetu_Flag = 1
                            Call PSUB_TOKUBETU_SET(txt特別請求月１, txt特別再振替月１, txt特別再振替日１, chk１_１学年, chk１_２学年, chk１_３学年, chk１_４学年, chk１_５学年, chk１_６学年, chk１_７学年, chk１_８学年, chk１_９学年, chk１_全学年, oraReader)

                            '2006/11/30　再々振替日を取得
                            str特別再々振日(1) = oraReader.GetString("SFURI_DATE_S")

                            '2006/11/30　チェックフラグを取得
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt特別振替月２.Tag
                            Int_Tokubetu_Flag = 2
                            Call PSUB_TOKUBETU_SET(txt特別請求月２, txt特別再振替月２, txt特別再振替日２, chk２_１学年, chk２_２学年, chk２_３学年, chk２_４学年, chk２_５学年, chk２_６学年, chk２_７学年, chk２_８学年, chk２_９学年, chk２_全学年, oraReader)

                            '2006/11/30　再々振替日を取得
                            str特別再々振日(2) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt特別振替月３.Tag
                            Int_Tokubetu_Flag = 3
                            Call PSUB_TOKUBETU_SET(txt特別請求月３, txt特別再振替月３, txt特別再振替日３, chk３_１学年, chk３_２学年, chk３_３学年, chk３_４学年, chk３_５学年, chk３_６学年, chk３_７学年, chk３_８学年, chk３_９学年, chk３_全学年, oraReader)

                            '2006/11/30　再々振替日を取得
                            str特別再々振日(3) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt特別振替月４.Tag
                            Int_Tokubetu_Flag = 4
                            Call PSUB_TOKUBETU_SET(txt特別請求月４, txt特別再振替月４, txt特別再振替日４, chk４_１学年, chk４_２学年, chk４_３学年, chk４_４学年, chk４_５学年, chk４_６学年, chk４_７学年, chk４_８学年, chk４_９学年, chk４_全学年, oraReader)

                            '2006/11/30　再々振替日を取得
                            str特別再々振日(4) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt特別振替月５.Tag
                            Int_Tokubetu_Flag = 5
                            Call PSUB_TOKUBETU_SET(txt特別請求月５, txt特別再振替月５, txt特別再振替日５, chk５_１学年, chk５_２学年, chk５_３学年, chk５_４学年, chk５_５学年, chk５_６学年, chk５_７学年, chk５_８学年, chk５_９学年, chk５_全学年, oraReader)

                            '2006/11/30　再々振替日を取得
                            str特別再々振日(5) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt特別振替月６.Tag
                            Int_Tokubetu_Flag = 6
                            Call PSUB_TOKUBETU_SET(txt特別請求月６, txt特別再振替月６, txt特別再振替日６, chk６_１学年, chk６_２学年, chk６_３学年, chk６_４学年, chk６_５学年, chk６_６学年, chk６_７学年, chk６_８学年, chk６_９学年, chk６_全学年, oraReader)

                            '2006/11/30　再々振替日を取得
                            str特別再々振日(6) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                    End Select
            End Select

            oraReader.NextRead()

        Loop

        oraReader.Close()

        'Tagの消去
        txt特別振替月１.Tag = ""
        txt特別振替月２.Tag = ""
        txt特別振替月３.Tag = ""
        txt特別振替月４.Tag = ""
        txt特別振替月５.Tag = ""
        txt特別振替月６.Tag = ""

        PFUNC_TOKUBETU_SANSYOU = True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI(ByVal str処理 As String) As Boolean
        '特別振替日　作成処理　　　
        PFUNC_TOKUBETU_SAKUSEI = False

        '入力チェック
        Select Case True
            Case (Trim(txt特別再振替月１.Text) <> "" And Trim(txt特別再振替日１.Text) <> "" And Trim(txt特別請求月１.Text) = "" And Trim(txt特別振替月１.Text) = "" And Trim(txt特別振替日１.Text) = "")
                MessageBox.Show("(特別スケジュール)" & vbCrLf & "振替日または再振替日の入力に誤りがあります", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt特別再振替月２.Text) <> "" And Trim(txt特別再振替日２.Text) <> "" And Trim(txt特別請求月２.Text) = "" And Trim(txt特別振替月２.Text) = "" And Trim(txt特別振替日２.Text) = "")
                MessageBox.Show("(特別スケジュール)" & vbCrLf & "振替日または再振替日の入力に誤りがあります", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt特別再振替月３.Text) <> "" And Trim(txt特別再振替日３.Text) <> "" And Trim(txt特別請求月３.Text) = "" And Trim(txt特別振替月３.Text) = "" And Trim(txt特別振替日３.Text) = "")
                MessageBox.Show("(特別スケジュール)" & vbCrLf & "振替日または再振替日の入力に誤りがあります", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt特別再振替月４.Text) <> "" And Trim(txt特別再振替日４.Text) <> "" And Trim(txt特別請求月４.Text) = "" And Trim(txt特別振替月４.Text) = "" And Trim(txt特別振替日４.Text) = "")
                MessageBox.Show("(特別スケジュール)" & vbCrLf & "振替日または再振替日の入力に誤りがあります", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt特別再振替月５.Text) <> "" And Trim(txt特別再振替日５.Text) <> "" And Trim(txt特別請求月５.Text) = "" And Trim(txt特別振替月５.Text) = "" And Trim(txt特別振替日５.Text) = "")
                MessageBox.Show("(特別スケジュール)" & vbCrLf & "振替日または再振替日の入力に誤りがあります", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt特別再振替月６.Text) <> "" And Trim(txt特別再振替日６.Text) <> "" And Trim(txt特別請求月６.Text) = "" And Trim(txt特別振替月６.Text) = "" And Trim(txt特別振替日６.Text) = "")
                MessageBox.Show("(特別スケジュール)" & vbCrLf & "振替日または再振替日の入力に誤りがあります", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
        End Select

        '2006/11/30　ループ化
        For i As Integer = 1 To 6

            '2006/11/30　変更があった場合のみ実行する
            If bln特別更新(i) = True Then

                '2006/12/12　旧振替日取得
                If SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki = "" Then
                    '空白の場合は入力の必要なし
                ElseIf CInt(SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki) < 4 Then
                    '１〜３月
                    str旧振替日(i) = CInt(txt対象年度.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date
                Else
                    '４〜１２月
                    str旧振替日(i) = txt対象年度.Text & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date
                End If

                '2006/12/12　旧再振日取得
                If Trim(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date) = "" Then
                    '再振日なし
                    str旧再振日(i) = "00000000"
                ElseIf CInt(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki) < 4 Then
                    '１〜３月
                    str旧再振日(i) = CInt(txt対象年度.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                Else
                    '４〜１２月
                    str旧再振日(i) = txt対象年度.Text & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                End If

                '振替日チェック 
                If SYOKI_TOKUBETU_SCHINFO(i).SyoriFurikae_Flag = True Then

                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then
                        If PFUNC_TOKUBETU_CHECK(i, TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check, TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check) = False Then
                            MessageBox.Show("(特別スケジュール)" & vbCrLf & "このスケジュールは処理中のスケジュールです。" & vbCrLf & "変更できません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Function
                        End If

                        If SYOKI_TOKUBETU_SCHINFO(i).SyoriSaiFurikae_Flag = True Then
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki Then
                                MessageBox.Show("(特別スケジュール)" & vbCrLf & "このスケジュールは処理中のスケジュールです。" & vbCrLf & "変更できません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If

                            If TOKUBETU_SCHINFO(i).SaiFurikae_Date <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date Then
                                MessageBox.Show("(特別スケジュール)" & vbCrLf & "このスケジュールは処理中のスケジュールです。" & vbCrLf & "変更できません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If
                        Else
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                                If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                    MessageBox.Show("(特別スケジュール)" & vbCrLf & "再振は行わない設定になっています", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    Exit Function
                                End If

                                'CHKBOXチェック&共通変数に設定
                                If PFUNC_GAKUNENFLG_CHECK(TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                                    Exit Function
                                End If

                                '再振のスケジュールのみ作成
                                If PFUNC_TOKUBETU_SAKUSEI_SUB2(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, i) = False Then
                                    Exit Function
                                End If

                                If PFUNC_SCHMAST_UPDATE_SFURIDATE(CStr(i)) = False Then
                                    Exit Function
                                End If

                                'ここを通るということは処理に成功したということなので
                                Int_Syori_Flag(1) = 1
                            End If
                        End If
                    Else
                        MessageBox.Show("(特別スケジュール)" & vbCrLf & "このスケジュールは処理中のスケジュールです。" & vbCrLf & "削除できません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                Else
                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then

                        '構造体を使用するため、共通変数は不要
                        If PFUNC_GAKUNENFLG_CHECK(TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                            Exit Function
                        End If

                        If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                            If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                MessageBox.Show("(特別スケジュール)" & vbCrLf & "再振は行わない設定になっています", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            End If
                        End If

                        'パラメタは�@月、�A入力振替日、�B再振替月　�C再振替日　�D振替区分（0:初振)、�Eスケジュール区分（1:特別)
                        If PFUNC_TOKUBETU_SAKUSEI_SUB(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, i) = False Then
                            Exit Function
                        End If

                        'ここを通るということは処理に成功したということなので
                        Int_Syori_Flag(1) = 1
                    End If
                End If

            Else '更新がない場合でも企業自振側のスケジュールを見る
                '2011/06/16 標準版修正 特別振替日相関チェック追加 ------------------START
                '2006/12/12　旧再振日取得
                If Trim(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date) = "" Then
                    '再振日なし
                    str旧再振日(i) = "00000000"
                ElseIf CInt(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki) < 4 Then
                    '１〜３月
                    str旧再振日(i) = CInt(txt対象年度.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                Else
                    '４〜１２月
                    str旧再振日(i) = txt対象年度.Text & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                End If
                '2011/06/16 標準版修正 特別振替日相関チェック追加 ------------------END

                '企業自振連携時のみ
                '振替日チェック 
                If SYOKI_TOKUBETU_SCHINFO(i).SyoriFurikae_Flag = True Then

                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then
                        If PFUNC_TOKUBETU_CHECK(i, TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check, TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check) = False Then
                            MessageBox.Show("(特別スケジュール)" & vbCrLf & "このスケジュールは処理中のスケジュールです。" & vbCrLf & "変更できません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function
                        End If

                        If SYOKI_TOKUBETU_SCHINFO(i).SyoriSaiFurikae_Flag = True Then
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki Then
                                MessageBox.Show("(特別スケジュール)" & vbCrLf & "このスケジュールは処理中のスケジュールです。" & vbCrLf & "変更できません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If

                            If TOKUBETU_SCHINFO(i).SaiFurikae_Date <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date Then
                                MessageBox.Show("(特別スケジュール)" & vbCrLf & "このスケジュールは処理中のスケジュールです。" & vbCrLf & "変更できません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If
                        Else
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                                If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                    MessageBox.Show("(特別スケジュール)" & vbCrLf & "再振は行わない設定になっています", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    Exit Function
                                End If

                                '再振のスケジュールのみ作成
                                If PFUNC_TOKUBETU_SAKUSEI_SUB2_KIGYO(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date) = False Then
                                    Exit Function
                                End If

                                'ここを通るということは処理に成功したということなので
                                Int_Syori_Flag(1) = 1
                            End If
                        End If
                    Else
                        MessageBox.Show("(特別スケジュール)" & vbCrLf & "このスケジュールは処理中のスケジュールです。" & vbCrLf & "削除できません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                Else
                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then

                        If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                            If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                MessageBox.Show("(特別スケジュール)" & vbCrLf & "再振は行わない設定になっています", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            End If
                        End If

                        'パラメタは�@月、�A入力振替日、�B再振替月　�C再振替日　�D振替区分（0:初振)、�Eスケジュール区分（1:特別)
                        If PFUNC_TOKUBETU_SAKUSEI_SUB_KIGYO(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, i) = False Then
                            Exit Function
                        End If

                        'ここを通るということは処理に成功したということなので
                        Int_Syori_Flag(1) = 1
                    End If
                End If

            End If
        Next

        If PFUNC_TOKUBETU_GAKNENFLG_CHECK() = False Then
            Exit Function
        End If

        PFUNC_TOKUBETU_SAKUSEI = True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB(ByVal s請求月 As String, ByVal s月 As String, ByVal s振替日 As String, ByVal s再振替月 As String, ByVal s再振替日 As String, ByVal i As Integer) As Boolean

        Dim oraReader As MyOracleReader
        Dim sql As StringBuilder

        'スケジュール　特別レコード作成
        PFUNC_TOKUBETU_SAKUSEI_SUB = False

        '初振レコードの作成

        '請求年月の作成
        STR請求年月 = PFUNC_SEIKYUTUKIHI(s請求月)

        '振替日算出
        STR振替日 = PFUNC_FURIHI_MAKE(s月, s振替日, "1", "0")

        '2010/10/21 契約振替日を算出す
        STR契約振替日 = PFUNC_KFURIHI_MAKE(s月, s振替日, "1", "0")

        If s再振替月 <> "" And s再振替日 <> "" Then
            '再振日の年の確定処理
            If s再振替月 = "01" Or s再振替月 = "02" Or s再振替月 = "03" Then
                STRW再振替年 = CStr(CInt(txt対象年度.Text) + 1)
            Else
                STRW再振替年 = txt対象年度.Text
            End If

            '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------START
            '営業日算出
            Select Sai_Zengo_Kbn
                Case 0
                    '翌営業日
                    STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "+")
                Case 1
                    '前営業日
                    STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "-")
            End Select
            'STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "+")

            '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------END
        Else
            STR再振替日 = "00000000"
        End If

        'スケジュール区分の共通変数設定
        STRスケ区分 = "1"

        '振替区分の共通変数設定
        STR振替区分 = "0"

        '入力振替日の共通変数設定
        STR年間入力振替日 = Space(15)

        '通常レコードの対象学年のフラグ更新（初振レコード）
        '学校コード、請求年月、振替区分。振替日（0:初振）
        If PFUNC_SCH_NENKAN_GET(STR請求年月, "0", STR振替日) = False Then
            MessageBox.Show("(特別スケジュール)" & vbCrLf & "年間スケジュールの対象学年設定でエラーが発生しました(初振)")
            Exit Function
        End If

        '既存スケジュールに「再度更新」用の処理件数・金額、振替済件数・金額、不能件数・金額の取得
        If PFUNC_G_MEIMAST_COUNT_MOTO(STR請求年月, "0", STR振替日) = False Then
            MessageBox.Show("(特別スケジュール)" & vbCrLf & "明細マスタ情報取得失敗")
            Exit Function
        End If

        Dim blnUP As Boolean = False

        '既存レコード（特別スケジュールがすでに作成されているか)有無チェック
        '2006/11/22　
        'If PFUNC_SCHMAST_GET("1", "0", STR振替日, STR再振替日) = True Then
        If PFUNC_SCHMAST_GET("1", "0", str旧振替日(i), str旧再振日(i)) = True Then
            '存在している場合UPDATEとする 2006/10/25
            blnUP = True
        End If

        '既存レコード（年間）の処理フラグ有無 2006/10/24
        If PFUNC_SCHMAST_GET_FLG("0", "0", STR振替日) = False Then
            MessageBox.Show("(特別スケジュール)" & vbCrLf & "通常スケジュール処理状況取得失敗", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        Else
            If strTYUUDAN_FLG = "1" Then
                MessageBox.Show("通常スケジュール(初振分)が中断中です" & vbCrLf & "振替日：" & STR振替日.Substring(0, 4) & "年" & STR振替日.Substring(4, 2) & "月" & STR振替日.Substring(6, 2) & "日の中断を取消してください", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        End If

        If strSAIFURI_DEF <> "00000000" Then '通常スケジュールの再振日が設定されている場合
            '既存レコード（年間）の処理フラグ有無 2006/10/24
            If PFUNC_SCHMAST_GET_FLG_SAI("0", "1", strSAIFURI_DEF) = False Then
                MessageBox.Show("(特別スケジュール)" & vbCrLf & "通常スケジュール(再振)処理状況取得失敗", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            Else
                If strTYUUDAN_FLG_SAI = "1" Then
                    MessageBox.Show("通常スケジュール(再振分)が処理中です" & vbCrLf & "再振日：" & strSAIFURI_DEF.Substring(0, 4) & "年" & strSAIFURI_DEF.Substring(4, 2) & "月" & strSAIFURI_DEF.Substring(6, 2) & "日の処理を取消してください", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
                '通常スケジュール（再振）の処理対象学年フラグ更新
                If PFUNC_SCH_NENKAN_GET(STR請求年月, "1", strSAIFURI_DEF) = False Then
                    MessageBox.Show("(特別スケジュール)" & vbCrLf & "年間スケジュールの対象学年設定でエラーが発生しました(再振)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If

            End If
        End If

        If PFUNC_G_MEIMAST_COUNT("0", STR振替日) = False Then
            MessageBox.Show("(特別スケジュール)" & vbCrLf & "明細マスタ情報取得失敗", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        '----------------------------------------------
        '更新・登録処理
        '----------------------------------------------
        Dim strSQL As String = ""
        If blnUP = True Then
            '2006/11/30　スケジュールの処理状況チェック
            If PFUNC_TOKUBETUFLG_CHECK("更新", "", i) = False Then
                Exit Function
            End If
            '既にスケジュール(初振)が存在している場合UPDATE
            strSQL = PSUB_UPDATE_G_SCHMAST_SQL(str旧振替日(i), str旧再振日(i))
        Else
            '2006/11/30　スケジュールの処理状況チェック
            If PFUNC_TOKUBETUFLG_CHECK("作成", "", i) = False Then
                Exit Function
            End If
            '2006/11/30　年間スケジュール更新
            If PFUNC_TokINSERT_NenUPDATE(STR請求年月, Replace(SYOKI_NENKAN_SCHINFO(CInt(s請求月)).Furikae_Day, "/", "")) = False Then
                Exit Function
            End If
            'スケジュールマスタ登録(初振)SQL文作成
            strSQL = PSUB_INSERT_G_SCHMAST_SQL()
        End If
        blnUP = False

        If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            '作成処理エラー
            Exit Function
        End If

        '2006/11/30　年間スケジュールの学年フラグの更新
        If PFUNC_NENKAN_GAKUNENFLG_UPDATE(STR請求年月, STR振替区分) = False Then
            Exit Function
        End If

        '-----------------------------------------------
        '2006/07/26　企業自振の初振のスケジュールも作成
        '-----------------------------------------------
        oraReader = New MyOracleReader(MainDB)
        sql = New StringBuilder(128)
        '既に登録されているかチェック
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        sql.Append("FURI_DATE_S = '" & STR振替日 & "'")

        '読込のみ
        If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
        Else     'スケジュールが存在しない
            'スケジュール作成
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '取引先マスタに取引先コードが存在することを確認
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                     gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                '2010/10/21 契約振替日対応 引数に追加
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, "01", STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", Err.Description)
                    MessageBox.Show("企業自振のスケジュールが登録できませんでした", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Return False
                End If
            End If
        End If
        oraReader.Close()
        '-----------------------------------------------

        '再振レコードの作成
        If STR再振替日 <> "00000000" Then

            '初振で求めた再振日を振替日に設定
            STR振替日 = STR再振替日
            str旧振替日(i) = str旧再振日(i)

            '2010/10/21 再振の契約振替日を算出する
            STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "1", "1")

            '振替区分は再振とする

            '再振日の算出
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(再振有/繰越無)
                    STR再振替日 = "00000000"
                    '2006/11/22　既存レコードチェック用
                    str旧再振日(i) = "00000000"
                Case "2"
                    '2(再振有/繰越有)   次回初振日を設定
                    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(s再振替日))
                    '2006/11/22　既存レコードチェック用
                    str旧再振日(i) = PFUNC_SAISAIFURIHI_MAKE(str旧再振日(i).Substring(4, 2), str旧再振日(i).Substring(6, 2))
            End Select

            '通常レコードの対象学年の設定し直し（再振レコード）
            '学校コード、請求年月、振替区分（1:再振）
            If PFUNC_SCH_NENKAN_GET(STR請求年月, "1", STR再振替日) = False Then
                MessageBox.Show("(特別スケジュール)" & vbCrLf & "特別スケジュール対象学年設定でエラーが発生しました(再振)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            blnUP = False

            '既存レコード有無チェック
            '2006/11/22
            'If PFUNC_SCHMAST_GET("1", "1", STR振替日, STR再振替日) = True Then
            If PFUNC_SCHMAST_GET("1", "1", str旧振替日(i), str旧再振日(i)) = True Then
                '存在している場合UPDATEとする 2006/10/25
                blnUP = True
            End If

            '既存レコード（年間）の処理フラグ有無 2006/10/24
            If PFUNC_SCHMAST_GET_FLG("0", "1", STR振替日) = False Then
                '通常振替日が無い場合(※特別振替日で全学年割り振られている時などは無視
            End If

            If PFUNC_G_MEIMAST_COUNT("1", STR振替日) = False Then
                MessageBox.Show("(特別スケジュール)" & vbCrLf & "明細マスタ情報取得失敗", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            'スケジュール区分の共通変数設定
            STRスケ区分 = "1"
            '振替区分の共通変数設定
            STR振替区分 = "1"
            '入力振替日の共通変数設定
            STR年間入力振替日 = Space(15)

            strSQL = ""
            If blnUP = True Then
                '既にスケジュール(初振)が存在している場合UPDATE
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(str旧振替日(i), str旧再振日(i))
            Else
                'スケジュールマスタ登録(再振)SQL文作成
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            End If

            '2006/11/30　年間スケジュールの学年フラグの更新
            If PFUNC_NENKAN_GAKUNENFLG_UPDATE(STR請求年月, STR振替区分) = False Then
                Exit Function
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                '作成処理エラー
                Exit Function
            End If
            '-----------------------------------------------
            '2006/07/26　企業自振の再振のスケジュールも作成
            '-----------------------------------------------
            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)
            '既に登録されているかチェック
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR振替日 & "'")

            '読込のみ
            If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
            Else     'スケジュールが存在しない
                'スケジュール作成
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '取引先マスタに取引先コードが存在することを確認
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                    '2010/10/21 契約振替日対応 引数に追加
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", Err.Description)
                        MessageBox.Show("企業自振のスケジュールが登録できませんでした", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If


            End If
            oraReader.Close()
        End If
        '-----------------------------------------------

        PFUNC_TOKUBETU_SAKUSEI_SUB = True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB2(ByVal s請求月 As String, ByVal s月 As String, ByVal s振替日 As String, ByVal s再振替月 As String, ByVal s再振替日 As String, ByVal i As Integer) As Boolean

        Dim kousin As Boolean = False   'インサートモード

        PFUNC_TOKUBETU_SAKUSEI_SUB2 = False

        'スケジュール　特別レコード作成
        '初振が処理中に再振のスケジュールを追加する際に使用

        '請求年月の作成
        STR請求年月 = PFUNC_SEIKYUTUKIHI(s請求月)

        '振替日算出
        STR振替日 = PFUNC_FURIHI_MAKE(s月, s振替日, "1", "0")

        '2010/10/21 契約振替日を算出する
        STR契約振替日 = PFUNC_KFURIHI_MAKE(s月, s振替日, "1", "0")

        '再振日の年の確定処理
        If s再振替月 = "01" Or s再振替月 = "02" Or s再振替月 = "03" Then
            STRW再振替年 = CStr(CInt(txt対象年度.Text) + 1)
        Else
            STRW再振替年 = txt対象年度.Text
        End If

        '営業日算出
        '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------START
        '営業日算出
        Select Case Sai_Zengo_Kbn
            Case 0
                '翌営業日
                STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "+")
            Case 1
                '前営業日
                STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "-")
        End Select
        'STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "+")
        '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------END


        '営業日を算出した結果で振替日と再振替日が同一になる場合がある為
        If STR振替日 = STR再振替日 Then
            MessageBox.Show("(特別スケジュール)" & vbCrLf & Mid(STR振替日, 5, 2) & "月の" & "振替日と再振替日が同一です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR振替日
        Str_SFURI_DATE = STR再振替日

        '再振レコードの作成
        If STR再振替日 <> "00000000" Then

            '初振で求めた再振日を振替日に設定
            STR振替日 = STR再振替日

            '2010/10/21 再振の契約振替日を算出する
            STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "1", "1")

            '振替区分は再振とする

            '再振日の算出
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(再振有/繰越無)
                    STR再振替日 = "00000000"
                Case "2"
                    '2(再振有/繰越有)   次回初振日を設定
                    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(s再振替日))
            End Select

            '通常レコードの対象学年の設定し直し（再振レコード）
            '学校コード、請求年月、振替区分（1:再振）
            If PFUNC_SCH_NENKAN_GET(STR請求年月, "1", STR再振替日) = False Then
                MessageBox.Show("(特別スケジュール)" & vbCrLf & "特別スケジュール対象学年設定でエラーが発生しました(再振)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            '既存レコード有無チェック
            If PFUNC_SCHMAST_GET("1", "1", STR請求年月 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date, "00000000") = True Then
                'If PFUNC_SCHMAST_GET("1", "1", STR振替日, STR再振替日) = True Then
                kousin = True   'アップデートモード
                'MessageBox.Show("(特別スケジュール)" & vbCrLf & "特別スケジュール作成済です(再振)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'Exit Function
            End If

            'スケジュール区分の共通変数設定
            STRスケ区分 = "1"
            '入力振替日の共通変数設定
            STR年間入力振替日 = Space(15)

            'スケジュールマスタ更新(初振)SQL文作成　2006/11/30
            Dim strSQL As String = ""
            '何かわからないのでコメント 2010.03.29 start
            'STR振替区分 = "0" '初振の反映のため、一時的に0に設定
            'strSQL = PSUB_UPDATE_G_SCHMAST_SQL(STR請求年月 & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date, STR請求年月 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date)

            'If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            '    Return False
            'End If
            '何かわからないのでコメント 2010.03.29 end

            '振替区分の共通変数設定
            STR振替区分 = "1"

            'スケジュールマスタ登録(再振)SQL文作成
            strSQL = ""
            If kousin = True Then
                'アップデート
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(STR請求年月 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date, "00000000")
            Else
                'インサート
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                Return False
            End If

            '2006/11/30　年間スケジュールの学年フラグの更新
            If PFUNC_NENKAN_GAKUNENFLG_UPDATE(STR請求年月, STR振替区分) = False Then
                Return False
            End If

            '-----------------------------------------------
            '2006/07/26　企業自振の再振のスケジュールも作成
            '-----------------------------------------------
            Dim oraReader As New MyOracleReader(MainDB)
            Dim sql As New StringBuilder(128)

            '既に登録されているかチェック
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR振替日 & "'")

            '読込のみ
            If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
            Else     'スケジュールが存在しない
                'コメント 2006/12/11
                'If intPUSH_BTN = 2 Then '更新時
                '    MessageBox.Show("企業自振側のスケジュール(" & STR請求年月.Substring(0, 4) & "年" & STR請求年月.Substring(4, 2) & "月分)が存在しません" & vbCrLf & "企業自振側で月間スケジュール作成後、" & vbCrLf & "学校スケジュールの更新処理を再度行ってください", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'Else
                'スケジュール作成
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '取引先マスタに取引先コードが存在することを確認
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                    '2010/10/21 契約振替日対応 引数に追加
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", Err.Description)
                        MessageBox.Show("企業自振のスケジュールが登録できませんでした", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If

        Return True

    End Function
    '企業のスケジュール更新用 2006/12/08
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB_KIGYO(ByVal s請求月 As String, ByVal s月 As String, ByVal s振替日 As String, ByVal s再振替月 As String, ByVal s再振替日 As String, ByVal i As Integer) As Boolean

        Dim oraReader As MyOracleReader
        Dim sql As StringBuilder

        'スケジュール　特別レコード作成
        PFUNC_TOKUBETU_SAKUSEI_SUB_KIGYO = False

        '初振レコードの作成

        '請求年月の作成
        STR請求年月 = PFUNC_SEIKYUTUKIHI(s請求月)

        '振替日算出
        STR振替日 = PFUNC_FURIHI_MAKE(s月, s振替日, "1", "0")

        '2010/10/21 契約振替日を算出する
        STR契約振替日 = PFUNC_KFURIHI_MAKE(s月, s振替日, "1", "0")

        If s再振替月 <> "" And s再振替日 <> "" Then
            '再振日の年の確定処理
            If s再振替月 = "01" Or s再振替月 = "02" Or s再振替月 = "03" Then
                STRW再振替年 = CStr(CInt(txt対象年度.Text) + 1)
            Else
                STRW再振替年 = txt対象年度.Text
            End If
            '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------START
            '営業日算出
            Select Case Sai_Zengo_Kbn
                Case 0
                    '翌営業日
                    STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "+")
                Case 1
                    '前営業日
                    STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "-")
            End Select
            'STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "+")
            '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------END
        Else
            STR再振替日 = "00000000"
        End If

        'スケジュール区分の共通変数設定
        STRスケ区分 = "1"

        '振替区分の共通変数設定
        STR振替区分 = "0"

        '入力振替日の共通変数設定
        STR年間入力振替日 = Space(15)

        oraReader = New MyOracleReader(MainDB)
        sql = New StringBuilder(128)
        '既に登録されているかチェック
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        sql.Append("FURI_DATE_S = '" & STR振替日 & "'")

        If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
        Else     'スケジュールが存在しない
            'スケジュール作成
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '取引先マスタに取引先コードが存在することを確認
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                        gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                '2010/10/21 契約振替日対応 引数に追加
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, "01", STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", Err.Description)
                    MessageBox.Show("企業自振のスケジュールが登録できませんでした", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Return False
                End If
            End If
            'End If

        End If
        oraReader.Close()

        '-----------------------------------------------

        '再振レコードの作成
        If STR再振替日 <> "00000000" Then

            '初振で求めた再振日を振替日に設定
            STR振替日 = STR再振替日
            str旧振替日(i) = str旧再振日(i)

            '2010/10/21 再振の契約振替日を算出する
            STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "1", "1")

            '振替区分は再振とする

            '再振日の算出
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(再振有/繰越無)
                    STR再振替日 = "00000000"
                    '2006/11/22　既存レコードチェック用
                    str旧再振日(i) = "00000000"
                Case "2"
                    '2(再振有/繰越有)   次回初振日を設定
                    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(s再振替日))
                    '2006/11/22　既存レコードチェック用
                    str旧再振日(i) = PFUNC_SAISAIFURIHI_MAKE(str旧再振日(i).Substring(4, 2), str旧再振日(i).Substring(6, 2))
            End Select

            'スケジュール区分の共通変数設定
            STRスケ区分 = "1"
            '振替区分の共通変数設定
            STR振替区分 = "1"
            '入力振替日の共通変数設定
            STR年間入力振替日 = Space(15)

            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)
            '既に登録されているかチェック
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR振替日 & "'")

            If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
            Else     'スケジュールが存在しない
                'スケジュール作成
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '取引先マスタに取引先コードが存在することを確認
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                    '2010/10/21 契約振替日対応 引数に追加
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", Err.Description)
                        MessageBox.Show("企業自振のスケジュールが登録できませんでした", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If
        '-----------------------------------------------

        Return True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB2_KIGYO(ByVal s請求月 As String, ByVal s月 As String, ByVal s振替日 As String, ByVal s再振替月 As String, ByVal s再振替日 As String) As Boolean

        Dim oraReader As MyOracleReader
        Dim sql As StringBuilder

        PFUNC_TOKUBETU_SAKUSEI_SUB2_KIGYO = False

        'スケジュール　特別レコード作成
        '初振が処理中に再振のスケジュールを追加する際に使用

        '請求年月の作成
        STR請求年月 = PFUNC_SEIKYUTUKIHI(s請求月)

        '振替日算出
        STR振替日 = PFUNC_FURIHI_MAKE(s月, s振替日, "1", "0")

        '2010/10/21 契約振替日を算出する
        STR契約振替日 = PFUNC_KFURIHI_MAKE(s月, s振替日, "1", "0")

        '再振日の年の確定処理
        If s再振替月 = "01" Or s再振替月 = "02" Or s再振替月 = "03" Then
            STRW再振替年 = CStr(CInt(txt対象年度.Text) + 1)
        Else
            STRW再振替年 = txt対象年度.Text
        End If

        '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------START
        '営業日算出
        Select Case Sai_Zengo_Kbn
            Case 0
                '翌営業日
                STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "+")
            Case 1
                '前営業日
                STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "-")
        End Select
        'STR再振替日 = PFUNC_EIGYOUBI_GET(STRW再振替年 & s再振替月 & s再振替日, "0", "+")
        '2011/06/16 標準版修正 再振休日シフトの翌営業日考慮 ------------------END

        '営業日を算出した結果で振替日と再振替日が同一になる場合がある為
        If STR振替日 = STR再振替日 Then
            MessageBox.Show("(特別スケジュール)" & vbCrLf & Mid(STR振替日, 5, 2) & "月の" & "振替日と再振替日が同一です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR振替日
        Str_SFURI_DATE = STR再振替日

        '再振レコードの作成
        If STR再振替日 <> "00000000" Then

            '初振で求めた再振日を振替日に設定
            STR振替日 = STR再振替日

            '2010/10/21 再振の契約振替日を算出する
            STR契約振替日 = PFUNC_KFURIHI_MAKE(s再振替月, s再振替日, "1", "1")

            '振替区分は再振とする

            '再振日の算出
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(再振有/繰越無)
                    STR再振替日 = "00000000"
                Case "2"
                    '2(再振有/繰越有)   次回初振日を設定
                    STR再振替日 = PFUNC_SAISAIFURIHI_MAKE(Trim(s再振替月), Trim(s再振替日))
            End Select

            'スケジュール区分の共通変数設定
            STRスケ区分 = "1"
            '入力振替日の共通変数設定
            STR年間入力振替日 = Space(15)

            '振替区分の共通変数設定
            STR振替区分 = "1"

            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)
            '既に登録されているかチェック
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR振替日 & "'")

            If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
            Else     'スケジュールが存在しない
                'スケジュール作成
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '取引先マスタに取引先コードが存在することを確認
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                            gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                    '2010/10/21 契約振替日対応 引数に追加
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", Err.Description)
                        MessageBox.Show("企業自振のスケジュールが登録できませんでした", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If

        Return True

    End Function

    '=========================================================
    '特別スケジュール登録時の年間スケジュール更新　2006/11/30
    '=========================================================
    Private Function PFUNC_TokINSERT_NenUPDATE(ByVal strNENGETUDO As String, ByVal strFURI_DATE As String) As Boolean

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        Dim j As Integer '               ループ用変数
        Dim strGakunen_FLG(9) As String '学年フラグ格納配列
        Dim bFlg As Boolean = False '    ループ内条件通過判定

        '特別スケジュールの学年フラグを配列に格納
        strGakunen_FLG(1) = STR１学年
        strGakunen_FLG(2) = STR２学年
        strGakunen_FLG(3) = STR３学年
        strGakunen_FLG(4) = STR４学年
        strGakunen_FLG(5) = STR５学年
        strGakunen_FLG(6) = STR６学年
        strGakunen_FLG(7) = STR７学年
        strGakunen_FLG(8) = STR８学年
        strGakunen_FLG(9) = STR９学年

        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)
        '------------------------------------------------
        '明細マスタ検索（件数・金額の取得）
        '------------------------------------------------
        sql.Append(" SELECT * FROM G_MEIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_M ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_M = '0'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_M ='" & strFURI_DATE & "'")

        sql.Append(" AND (")

        'フラグの立っている学年を条件に追加
        For j = 1 To 9
            If strGakunen_FLG(j) = 1 Then
                If bFlg = True Then
                    sql.Append(" or")
                End If

                sql.Append(" GAKUNEN_CODE_M = " & j)
                bFlg = True
            End If
        Next j

        sql.Append(" )")

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then

            '------------------------------------------------
            '件数・金額取得
            '------------------------------------------------

            Do Until oraReader.EOF

                lngSYORI_KEN = lngSYORI_KEN + 1
                dblSYORI_KIN = dblSYORI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                If oraReader.GetString("FURIKETU_CODE_M") = "0" Then
                    lngFURI_KEN = lngFURI_KEN + 1
                    dblFURI_KIN = dblFURI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                Else
                    lngFUNOU_KEN = lngFUNOU_KEN + 1
                    dblFUNOU_KIN = dblFUNOU_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                End If
                oraReader.NextRead()
            Loop

        End If
        oraReader.Close()

        '------------------------------------------------
        '年間スケジュール更新
        '------------------------------------------------
        bFlg = False

        sql = New StringBuilder(128)

        sql.Append("UPDATE  G_SCHMAST SET ")

        '元のデータに合算分の件数・金額を足す
        sql.Append(" SYORI_KEN_S = SYORI_KEN_S - " & CDbl(lngSYORI_KEN) & ",")
        sql.Append(" SYORI_KIN_S = SYORI_KIN_S - " & dblSYORI_KIN & ",")
        sql.Append(" FURI_KEN_S = FURI_KEN_S - " & CDbl(lngFURI_KEN) & ",")
        sql.Append(" FURI_KIN_S =  FURI_KIN_S - " & dblFURI_KIN & ",")
        sql.Append(" FUNOU_KEN_S = FUNOU_KEN_S - " & CDbl(lngFUNOU_KEN) & ",")
        sql.Append(" FUNOU_KIN_S = FUNOU_KIN_S - " & dblFUNOU_KIN)
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            '更新処理エラー
            MessageBox.Show("スケジュールマスタの更新処理でエラーが発生しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '-----------------------------------------------------
        '処理フラグ取得（特別スケジュールのINSERT処理に使用）
        '-----------------------------------------------------
        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                strENTRI_FLG = oraReader.GetString("ENTRI_FLG_S")
                strCHECK_FLG = oraReader.GetString("CHECK_FLG_S")
                strDATA_FLG = oraReader.GetString("DATA_FLG_S")
                strFUNOU_FLG = oraReader.GetString("FUNOU_FLG_S")
                strSAIFURI_FLG = oraReader.GetString("SAIFURI_FLG_S")
                strKESSAI_FLG = oraReader.GetString("KESSAI_FLG_S")

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function

    Private Function PFUNC_TOKUBETU_KOUSIN() As Boolean

        '削除処理（DELETE）
        If PFUNC_TOKUBETU_DELETE() = False Then
            Return False
        End If

        '作成処理（INSERT/UPDATE)
        If PFUNC_TOKUBETU_SAKUSEI("更新") = False Then
            Return False
        End If

        '不要年間スケジュール削除処理
        If PFUNC_DELETE_GSCHMAST() = False Then
            Return False
        End If

        Return True

    End Function

    '====================================================
    '年間スケジュールの学年フラグ更新　2006/11/30
    '====================================================
    Private Function PFUNC_NENKAN_GAKUNENFLG_UPDATE(ByVal strNENGETUDO As String, ByVal strFURIKUBUN As String) As Boolean

        PFUNC_NENKAN_GAKUNENFLG_UPDATE = False

        Dim strGakunen_FLG(9) As String '学年フラグ格納用配列
        Dim sql As New StringBuilder(128) '             SQL文格納変数

        '特別スケジュールの学年フラグを配列に格納
        strGakunen_FLG(1) = STR１学年
        strGakunen_FLG(2) = STR２学年
        strGakunen_FLG(3) = STR３学年
        strGakunen_FLG(4) = STR４学年
        strGakunen_FLG(5) = STR５学年
        strGakunen_FLG(6) = STR６学年
        strGakunen_FLG(7) = STR７学年
        strGakunen_FLG(8) = STR８学年
        strGakunen_FLG(9) = STR９学年

        '年間スケジュールの学年フラグの更新
        sql.Append("UPDATE  G_SCHMAST SET ")

        For j As Integer = 1 To 9
            If strGakunen_FLG(j) = "1" Then
                sql.Append(" GAKUNEN" & j & "_FLG_S ='0'") '特別でフラグが立っている学年は年間では降ろす
            Else
                sql.Append(" GAKUNEN" & j & "_FLG_S ='1'") '特別でフラグが降りている学年は年間では立てる
            End If
            If j <> 9 Then
                sql.Append(",")
            End If
        Next

        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")

        If strFURIKUBUN <> "*" Then '*：初振・再振両方更新
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S ='" & strFURIKUBUN & "'")
        Else
            sql.Append(" AND")
            sql.Append(" (FURI_KBN_S ='0'")
            sql.Append(" or")
            sql.Append(" FURI_KBN_S ='1')")
        End If

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show("スケジュールマスタの更新処理でエラーが発生しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True

    End Function


    '===============================================
    '特別スケジュール処理フラグチェック　2006/11/30
    '===============================================
    Private Function PFUNC_TOKUBETUFLG_CHECK(ByVal strSyori As String, ByVal strSeikyuNenGetu As String, ByVal i As Integer) As Boolean

        PFUNC_TOKUBETUFLG_CHECK = False

        '処理によってチェック内容を変更
        Select Case strSyori

            Case "更新" '特別スケジュールが処理中
                If SYOKI_TOKUBETU_SCHINFO(i).SyoriFurikae_Flag = True Then

                    MessageBox.Show("(特別スケジュール)" & vbCrLf & _
                                                  "処理中のため、変更出来ません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function

                End If

            Case "作成" '年間スケジュールの �@チェックフラグが立っていて、不能フラグが降りている
                '                           �A再振スケジュールが処理中
                If SYOKI_NENKAN_SCHINFO(CInt(TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckFurikae_Flag <> SYOKI_NENKAN_SCHINFO(CInt(TOKUBETU_SCHINFO(i).Seikyu_Tuki)).FunouFurikae_Flag Or _
                   SYOKI_NENKAN_SCHINFO(CInt(TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckSaiFurikae_Flag = True Then

                    MessageBox.Show("(特別スケジュール)" & vbCrLf & _
                                                  "年間スケジュールが処理中のため、作成出来ません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function

                End If

            Case "削除" '年間・特別スケジュールが処理中で、違う振替日
                If (SYOKI_TOKUBETU_SCHINFO(i).CheckFurikae_Flag = True Or SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckFurikae_Flag = True) And _
                    Replace(SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).Furikae_Day, "/", "") <> strSeikyuNenGetu & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date Then

                    MessageBox.Show("(特別スケジュール)" & vbCrLf & _
                                                  "年間スケジュールが処理中のため、削除できません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                ElseIf (SYOKI_TOKUBETU_SCHINFO(i).CheckFurikae_Flag = True Or SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckFurikae_Flag = True) And _
                (SYOKI_TOKUBETU_SCHINFO(i).FunouFurikae_Flag = False Or SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).FunouFurikae_Flag = False) And _
                    Replace(SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).Furikae_Day, "/", "") <> strSeikyuNenGetu & TOKUBETU_SCHINFO(i).Furikae_Date Then
                    '削除条件追加(修正) 2007/01/09
                    MessageBox.Show("(特別スケジュール)" & vbCrLf & _
                                                      "年間スケジュールが処理中のため、削除できません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
        End Select

        PFUNC_TOKUBETUFLG_CHECK = True

    End Function

    '====================================================
    '特別スケジュール削除処理　2006/11/30
    '====================================================
    Private Function PFUNC_TOKUBETU_DELETE() As Boolean
        PFUNC_TOKUBETU_DELETE = False

        Dim sql As New StringBuilder(128)

        Dim blnSakujo_Check As Boolean = False
        Dim strNengetu As String '   処理年月
        Dim strSFuri_Date As String '再振日

        '全削除処理、キーは学校コード、対象年度、スケジュール区分（１：特別）、処理フラグ（０）
        sql.Append(" DELETE  FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S =1")
        sql.Append(" AND")
        sql.Append(" ((CHECK_FLG_S =0 AND DATA_FLG_S =0 AND FUNOU_FLG_S =0 ) OR (CHECK_FLG_S =1 AND DATA_FLG_S =1 AND FUNOU_FLG_S =1 ))")
        sql.Append(" AND")
        sql.Append(" TYUUDAN_FLG_S =0")

        '2006/11/30　条件追加（変更のあったデータのみ削除）=========================
        For i As Integer = 1 To 6

            '------------------------------------------------------------
            '変更があり、請求月・初振月・初振日欄が空白のものを削除する
            '------------------------------------------------------------
            If bln特別更新(i) = True And TOKUBETU_SCHINFO(i).Seikyu_Tuki = "" And TOKUBETU_SCHINFO(i).Furikae_Date = "" And _
               TOKUBETU_SCHINFO(i).Furikae_Date = "" And SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And _
               SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then

                '年月度を取得
                If CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki) < 4 Then
                    '１〜３月
                    strNengetu = CInt(txt対象年度.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                Else
                    '４〜１２月
                    strNengetu = txt対象年度.Text & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                End If

                'スケジュールの処理状況チェック
                If PFUNC_TOKUBETUFLG_CHECK("削除", strNengetu, i) = False Then
                    Exit Function
                End If

                '再振日取得
                If SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date = "" Then
                    '再振日が空白の場合、0埋めする
                    strSFuri_Date = "00000000"
                Else
                    strSFuri_Date = strNengetu & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                End If

                '接続詞追加
                If blnSakujo_Check = True Then
                    sql.Append(" or") '  二文目以降
                Else
                    sql.Append(" and(") '一文目
                End If

                '振替日・再振日・振替区分の設定
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & strSFuri_Date & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '0')") 'FURI_KBN_S = 0：初振分

                '再振のスケジュールも削除する
                sql.Append(" or")
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & str特別再々振日(i) & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '1')") 'FURI_KBN_S = 1：再振分

                '----------------------------------------------
                '年間スケジュール学年フラグ更新
                '----------------------------------------------
                '使用学年フラグ取得
                If PFUNC_GAKUNENFLG_CHECK(SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen9_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                    MessageBox.Show("(特別スケジュール)" & vbCrLf & "明細マスタ情報取得失敗", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
                '年間スケジュール更新処理
                If PFUNC_TokDELETE_NenUPDATE(strNengetu, strNengetu & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date, strSFuri_Date) = False Then
                    Exit Function
                End If

                bln特別更新(i) = False '変更フラグを降ろす
                blnSakujo_Check = True '削除フラグを立てる

                '------------------------------------------------------------
                '再振スケジュールのみの削除
                '------------------------------------------------------------
            ElseIf bln特別更新(i) = True And TOKUBETU_SCHINFO(i).SaiFurikae_Tuki = "" And _
                TOKUBETU_SCHINFO(i).SaiFurikae_Date = "" And SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And _
                SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then

                '年月度を取得
                If CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki) < 4 Then
                    '１〜３月
                    strNengetu = CInt(txt対象年度.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                Else
                    '４〜１２月
                    strNengetu = txt対象年度.Text & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                End If

                If blnSakujo_Check = True Then
                    sql.Append(" or") '  二文目以降
                Else
                    sql.Append(" and(") '一文目
                End If

                '振替日・再振日・振替区分の設定
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & str特別再々振日(i) & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '1')") 'FURI_KBN_S = 1：再振分

                '再振のみ削除した場合、初振も変更が必要なので変更フラグは降ろさない
                blnSakujo_Check = True '削除フラグを立てる

            End If
        Next

        If blnSakujo_Check = True Then
            sql.Append(")")
            '削除データがある場合のみ実行する
            If MainDB.ExecuteNonQuery(sql) < 0 Then
                MessageBox.Show("スケジュールの削除処理でエラーが発生しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        End If

        Return True

    End Function

    '==========================================================
    '特別スケジュール削除時の年間スケジュール更新　2006/11/30
    '==========================================================
    Private Function PFUNC_TokDELETE_NenUPDATE(ByVal strNENGETUDO As String, ByVal strFURI_DATE As String, ByVal strSFURI_DATE As String) As Boolean

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        Dim strGakunen_FLG(9) As String '学年フラグ格納配列
        Dim bFlg As Boolean = False '    ループ内条件通過判定

        '特別スケジュールの学年フラグを配列に格納
        strGakunen_FLG(1) = STR１学年
        strGakunen_FLG(2) = STR２学年
        strGakunen_FLG(3) = STR３学年
        strGakunen_FLG(4) = STR４学年
        strGakunen_FLG(5) = STR５学年
        strGakunen_FLG(6) = STR６学年
        strGakunen_FLG(7) = STR７学年
        strGakunen_FLG(8) = STR８学年
        strGakunen_FLG(9) = STR９学年

        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        '---------------------------------------------------
        '削除するスケジュールマスタ検索（件数・金額の取得）
        '---------------------------------------------------
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '0'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURI_DATE & "'")
        sql.Append(" AND")
        sql.Append(" SFURI_DATE_S ='" & strSFURI_DATE & "'")

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then
            '------------------------------------------------
            '件数・金額取得
            '------------------------------------------------
            Do Until oraReader.EOF

                '処理件数・金額取得
                lngSYORI_KEN = CDbl(oraReader.GetInt64("SYORI_KEN_S"))
                dblSYORI_KIN = CDbl(oraReader.GetInt64("SYORI_KIN_S"))
                '振替件数・金額取得
                lngFURI_KEN = CDbl(oraReader.GetInt64("FURI_KEN_S"))
                dblFURI_KIN = CDbl(oraReader.GetInt64("FURI_KIN_S"))
                '不能件数・金額取得
                lngFUNOU_KEN = CDbl(oraReader.GetInt64("FUNOU_KEN_S"))
                dblFUNOU_KIN = CDbl(oraReader.GetInt64("FUNOU_KIN_S"))

                oraReader.NextRead()
            Loop

        End If
        oraReader.Close()

        '------------------------------------------------
        '年間スケジュール件数・金額更新（初振分のみ）
        '------------------------------------------------
        sql = New StringBuilder(128)

        sql.Append("UPDATE  G_SCHMAST SET ")

        '元のデータに合算分の件数・金額を足す
        sql.Append(" SYORI_KEN_S = SYORI_KEN_S + " & lngSYORI_KEN & ",")
        sql.Append(" SYORI_KIN_S = SYORI_KIN_S + " & dblSYORI_KIN & ",")
        sql.Append(" FURI_KEN_S = FURI_KEN_S + " & lngFURI_KEN & ",")
        sql.Append(" FURI_KIN_S =  FURI_KIN_S + " & dblFURI_KIN & ",")
        sql.Append(" FUNOU_KEN_S = FUNOU_KEN_S + " & lngFUNOU_KEN & ",")
        sql.Append(" FUNOU_KIN_S = FUNOU_KIN_S + " & dblFUNOU_KIN)

        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show("スケジュールマスタの更新処理でエラーが発生しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '-------------------------------------------------
        '年間スケジュール学年フラグ変更（初振・再振両方）
        '-------------------------------------------------
        bFlg = False

        sql = New StringBuilder(128)

        sql.Append("UPDATE  G_SCHMAST SET ")

        '合算データ分の学年フラグを立てる
        For j As Integer = 1 To 9
            If strGakunen_FLG(j) = "1" Then
                If bFlg = True Then
                    sql.Append(",")
                End If
                sql.Append(" GAKUNEN" & j & "_FLG_S = '1'")
                bFlg = True
            End If
        Next

        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" (FURI_KBN_S ='0'")
        sql.Append(" or")
        sql.Append(" FURI_KBN_S ='1')")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show("スケジュールマスタの更新処理でエラーが発生しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True

    End Function

    Private Function PFUNC_TOKUBETU_CHECK(ByVal pIndex As Integer, _
                                          ByVal pSeikyu_Tuki As String, _
                                          ByVal pFuri_Tuki As String, _
                                          ByVal pFuri_Hi As String, _
                                          ByVal pSaiFuri_Tuki As String, _
                                          ByVal pSaiFuri_Hi As String, _
                                          ByVal pSiyouFlag0 As Boolean, ByVal pSiyouFlag1 As Boolean, ByVal pSiyouFlag2 As Boolean, ByVal pSiyouFlag3 As Boolean, ByVal pSiyouFlag4 As Boolean, ByVal pSiyouFlag5 As Boolean, ByVal pSiyouFlag6 As Boolean, ByVal pSiyouFlag7 As Boolean, ByVal pSiyouFlag8 As Boolean, ByVal pSiyouFlag9 As Boolean) As Boolean

        PFUNC_TOKUBETU_CHECK = False

        '参照時に取得した内容と更新時に取得した内容に変更があるかどうかの判定を行う

        If pSeikyu_Tuki <> SYOKI_TOKUBETU_SCHINFO(pIndex).Seikyu_Tuki Then
            Exit Function
        End If

        If pFuri_Tuki <> SYOKI_TOKUBETU_SCHINFO(pIndex).Furikae_Tuki Then
            Exit Function
        End If

        If pFuri_Hi <> SYOKI_TOKUBETU_SCHINFO(pIndex).Furikae_Date Then
            Exit Function
        End If

        Select Case pSiyouFlag0
            Case True
                If SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen1_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen2_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen3_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen4_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen5_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen6_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen7_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen8_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen9_Check = True Then
                Else
                    Exit Function
                End If
            Case False
                If pSiyouFlag1 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen1_Check Then
                    Exit Function
                End If
                If pSiyouFlag2 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen2_Check Then
                    Exit Function
                End If
                If pSiyouFlag3 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen3_Check Then
                    Exit Function
                End If
                If pSiyouFlag4 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen4_Check Then
                    Exit Function
                End If
                If pSiyouFlag5 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen5_Check Then
                    Exit Function
                End If
                If pSiyouFlag6 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen6_Check Then
                    Exit Function
                End If
                If pSiyouFlag7 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen7_Check Then
                    Exit Function
                End If
                If pSiyouFlag8 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen8_Check Then
                    Exit Function
                End If
                If pSiyouFlag9 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen9_Check Then
                    Exit Function
                End If
        End Select

        PFUNC_TOKUBETU_CHECK = True

    End Function

    '==================================================================
    '同じ請求月に同じ学年フラグが複数立っていないかチェック 2006/11/30
    '==================================================================
    Private Function PFUNC_GAKNENFLG_CHECK() As Boolean

        PFUNC_GAKNENFLG_CHECK = False

        Dim strSeikyu_Tuki(6) As String '請求月
        Dim strGakunen_FLG(6, 10) As Boolean '学年フラグ（特別スケジュール番号,学年）

        strSeikyu_Tuki(1) = txt特別請求月１.Text
        strSeikyu_Tuki(2) = txt特別請求月２.Text
        strSeikyu_Tuki(3) = txt特別請求月３.Text
        strSeikyu_Tuki(4) = txt特別請求月４.Text
        strSeikyu_Tuki(5) = txt特別請求月５.Text
        strSeikyu_Tuki(6) = txt特別請求月６.Text

        '全学年フラグを取得
        PSUB_GAKUNENFLG_GET(strGakunen_FLG)

        '同請求月かつ同学年のフラグが立っていないかチェック
        For i As Integer = 1 To 5
            For j As Integer = i + 1 To 6
                '同請求月チェック（空欄でなく、請求月が同じ）
                If strSeikyu_Tuki(i) <> "" And strSeikyu_Tuki(i) = strSeikyu_Tuki(j) Then
                    For k As Integer = 1 To 9
                        If strGakunen_FLG(i, k) = True And strGakunen_FLG(j, k) = True Then
                            '同学年フラグチェック（両方True）
                            MessageBox.Show("(特別スケジュール)" & vbCrLf & "同請求月に同学年の処理があります", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Function
                        ElseIf strGakunen_FLG(i, 10) = True Or strGakunen_FLG(j, 10) = True Then
                            '全学年フラグチェック（どちらかがTrue）
                            MessageBox.Show("(特別スケジュール)" & vbCrLf & "同請求月に全学年の処理があります", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Function
                        End If
                    Next
                End If
            Next
        Next

        PFUNC_GAKNENFLG_CHECK = True

    End Function

    '2007/01/04
    Private Function PFUNC_TOKUBETU_GAKNENFLG_CHECK() As Boolean
        '==================================================================
        '特別スケジュールに指定されている学年について年間スケジュールの
        '学年フラグを0に更新する 2007/01/04
        '==================================================================

        PFUNC_TOKUBETU_GAKNENFLG_CHECK = False
        Dim strSeikyu_Tuki(6) As String '請求月
        Dim strGakunen_FLG(6, 10) As Boolean '学年フラグ（特別スケジュール番号,学年）

        Dim strSEIKYU_NENGETU As String = ""

        Dim sql As New StringBuilder(128)

        strSeikyu_Tuki(1) = txt特別請求月１.Text
        strSeikyu_Tuki(2) = txt特別請求月２.Text
        strSeikyu_Tuki(3) = txt特別請求月３.Text
        strSeikyu_Tuki(4) = txt特別請求月４.Text
        strSeikyu_Tuki(5) = txt特別請求月５.Text
        strSeikyu_Tuki(6) = txt特別請求月６.Text

        '全学年フラグを取得
        PSUB_GAKUNENFLG_GET(strGakunen_FLG)

        For i As Integer = 1 To 6
            If strSeikyu_Tuki(i).Trim = "" Then
                GoTo Next_SEIKYUTUKI
            End If

            '請求年月の作成
            strSEIKYU_NENGETU = PFUNC_SEIKYUTUKIHI(strSeikyu_Tuki(i))

            For j As Integer = 1 To 10
                If strGakunen_FLG(i, j) = True Then

                    sql.Length = 0
                    sql.Append("UPDATE  G_SCHMAST SET ")
                    If j = 10 Then
                        sql.Append(" GAKUNEN1_FLG_S ='0', ")
                        sql.Append(" GAKUNEN2_FLG_S ='0', ")
                        sql.Append(" GAKUNEN3_FLG_S ='0', ")
                        sql.Append(" GAKUNEN4_FLG_S ='0', ")
                        sql.Append(" GAKUNEN5_FLG_S ='0', ")
                        sql.Append(" GAKUNEN6_FLG_S ='0', ")
                        sql.Append(" GAKUNEN7_FLG_S ='0', ")
                        sql.Append(" GAKUNEN8_FLG_S ='0', ")
                        sql.Append(" GAKUNEN9_FLG_S ='0' ")
                    Else
                        sql.Append(" GAKUNEN" & j & "_FLG_S ='0' ")
                    End If
                    sql.Append(" WHERE GAKKOU_CODE_S = '" & txtGAKKOU_CODE.Text.Trim & "' ")
                    sql.Append(" AND SCH_KBN_S = '0'")
                    sql.Append(" AND NENGETUDO_S = '" & strSEIKYU_NENGETU & "' ")

                    If MainDB.ExecuteNonQuery(sql) < 0 Then
                        MessageBox.Show("スケジュールマスタの更新処理でエラーが発生しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If

                End If

            Next
Next_SEIKYUTUKI:
        Next

        Return True

    End Function


#End Region

#Region " Private Sub(随時スケジュール)"
    Private Sub PSUB_ZUIJI_GET(ByRef Get_Data() As ZuijiData)

        '随時スケジュールタブ画面で現在表示されている項目の内容を構造体に取得
        Get_Data(1).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分１)
        Get_Data(1).Furikae_Tuki = txt随時振替月１.Text
        Get_Data(1).Furikae_Date = txt随時振替日１.Text

        Select Case chk随時１_全学年.Checked
            Case True
                Get_Data(1).SiyouGakunen1_Check = True
                Get_Data(1).SiyouGakunen2_Check = True
                Get_Data(1).SiyouGakunen3_Check = True
                Get_Data(1).SiyouGakunen4_Check = True
                Get_Data(1).SiyouGakunen5_Check = True
                Get_Data(1).SiyouGakunen6_Check = True
                Get_Data(1).SiyouGakunen7_Check = True
                Get_Data(1).SiyouGakunen8_Check = True
                Get_Data(1).SiyouGakunen9_Check = True
            Case False
                Get_Data(1).SiyouGakunen1_Check = chk随時１_１学年.Checked
                Get_Data(1).SiyouGakunen2_Check = chk随時１_２学年.Checked
                Get_Data(1).SiyouGakunen3_Check = chk随時１_３学年.Checked
                Get_Data(1).SiyouGakunen4_Check = chk随時１_４学年.Checked
                Get_Data(1).SiyouGakunen5_Check = chk随時１_５学年.Checked
                Get_Data(1).SiyouGakunen6_Check = chk随時１_６学年.Checked
                Get_Data(1).SiyouGakunen7_Check = chk随時１_７学年.Checked
                Get_Data(1).SiyouGakunen8_Check = chk随時１_８学年.Checked
                Get_Data(1).SiyouGakunen9_Check = chk随時１_９学年.Checked
        End Select

        Get_Data(2).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分２)
        Get_Data(2).Furikae_Tuki = txt随時振替月２.Text
        Get_Data(2).Furikae_Date = txt随時振替日２.Text

        Select Case chk随時２_全学年.Checked
            Case True
                Get_Data(2).SiyouGakunen1_Check = True
                Get_Data(2).SiyouGakunen2_Check = True
                Get_Data(2).SiyouGakunen3_Check = True
                Get_Data(2).SiyouGakunen4_Check = True
                Get_Data(2).SiyouGakunen5_Check = True
                Get_Data(2).SiyouGakunen6_Check = True
                Get_Data(2).SiyouGakunen7_Check = True
                Get_Data(2).SiyouGakunen8_Check = True
                Get_Data(2).SiyouGakunen9_Check = True
            Case False
                Get_Data(2).SiyouGakunen1_Check = chk随時２_１学年.Checked
                Get_Data(2).SiyouGakunen2_Check = chk随時２_２学年.Checked
                Get_Data(2).SiyouGakunen3_Check = chk随時２_３学年.Checked
                Get_Data(2).SiyouGakunen4_Check = chk随時２_４学年.Checked
                Get_Data(2).SiyouGakunen5_Check = chk随時２_５学年.Checked
                Get_Data(2).SiyouGakunen6_Check = chk随時２_６学年.Checked
                Get_Data(2).SiyouGakunen7_Check = chk随時２_７学年.Checked
                Get_Data(2).SiyouGakunen8_Check = chk随時２_８学年.Checked
                Get_Data(2).SiyouGakunen9_Check = chk随時２_９学年.Checked
        End Select

        Get_Data(3).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分３)
        Get_Data(3).Furikae_Tuki = txt随時振替月３.Text
        Get_Data(3).Furikae_Date = txt随時振替日３.Text

        Select Case chk随時３_全学年.Checked
            Case True
                Get_Data(3).SiyouGakunen1_Check = True
                Get_Data(3).SiyouGakunen2_Check = True
                Get_Data(3).SiyouGakunen3_Check = True
                Get_Data(3).SiyouGakunen4_Check = True
                Get_Data(3).SiyouGakunen5_Check = True
                Get_Data(3).SiyouGakunen6_Check = True
                Get_Data(3).SiyouGakunen7_Check = True
                Get_Data(3).SiyouGakunen8_Check = True
                Get_Data(3).SiyouGakunen9_Check = True
            Case False
                Get_Data(3).SiyouGakunen1_Check = chk随時３_１学年.Checked
                Get_Data(3).SiyouGakunen2_Check = chk随時３_２学年.Checked
                Get_Data(3).SiyouGakunen3_Check = chk随時３_３学年.Checked
                Get_Data(3).SiyouGakunen4_Check = chk随時３_４学年.Checked
                Get_Data(3).SiyouGakunen5_Check = chk随時３_５学年.Checked
                Get_Data(3).SiyouGakunen6_Check = chk随時３_６学年.Checked
                Get_Data(3).SiyouGakunen7_Check = chk随時３_７学年.Checked
                Get_Data(3).SiyouGakunen8_Check = chk随時３_８学年.Checked
                Get_Data(3).SiyouGakunen9_Check = chk随時３_９学年.Checked
        End Select

        Get_Data(4).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分４)
        Get_Data(4).Furikae_Tuki = txt随時振替月４.Text
        Get_Data(4).Furikae_Date = txt随時振替日４.Text

        Select Case chk随時４_全学年.Checked
            Case True
                Get_Data(4).SiyouGakunen1_Check = True
                Get_Data(4).SiyouGakunen2_Check = True
                Get_Data(4).SiyouGakunen3_Check = True
                Get_Data(4).SiyouGakunen4_Check = True
                Get_Data(4).SiyouGakunen5_Check = True
                Get_Data(4).SiyouGakunen6_Check = True
                Get_Data(4).SiyouGakunen7_Check = True
                Get_Data(4).SiyouGakunen8_Check = True
                Get_Data(4).SiyouGakunen9_Check = True
            Case False
                Get_Data(4).SiyouGakunen1_Check = chk随時４_１学年.Checked
                Get_Data(4).SiyouGakunen2_Check = chk随時４_２学年.Checked
                Get_Data(4).SiyouGakunen3_Check = chk随時４_３学年.Checked
                Get_Data(4).SiyouGakunen4_Check = chk随時４_４学年.Checked
                Get_Data(4).SiyouGakunen5_Check = chk随時４_５学年.Checked
                Get_Data(4).SiyouGakunen6_Check = chk随時４_６学年.Checked
                Get_Data(4).SiyouGakunen7_Check = chk随時４_７学年.Checked
                Get_Data(4).SiyouGakunen8_Check = chk随時４_８学年.Checked
                Get_Data(4).SiyouGakunen9_Check = chk随時４_９学年.Checked
        End Select

        Get_Data(5).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分５)
        Get_Data(5).Furikae_Tuki = txt随時振替月５.Text
        Get_Data(5).Furikae_Date = txt随時振替日５.Text

        Select Case chk随時５_全学年.Checked
            Case True
                Get_Data(5).SiyouGakunen1_Check = True
                Get_Data(5).SiyouGakunen2_Check = True
                Get_Data(5).SiyouGakunen3_Check = True
                Get_Data(5).SiyouGakunen4_Check = True
                Get_Data(5).SiyouGakunen5_Check = True
                Get_Data(5).SiyouGakunen6_Check = True
                Get_Data(5).SiyouGakunen7_Check = True
                Get_Data(5).SiyouGakunen8_Check = True
                Get_Data(5).SiyouGakunen9_Check = True
            Case False
                Get_Data(5).SiyouGakunen1_Check = chk随時５_１学年.Checked
                Get_Data(5).SiyouGakunen2_Check = chk随時５_２学年.Checked
                Get_Data(5).SiyouGakunen3_Check = chk随時５_３学年.Checked
                Get_Data(5).SiyouGakunen4_Check = chk随時５_４学年.Checked
                Get_Data(5).SiyouGakunen5_Check = chk随時５_５学年.Checked
                Get_Data(5).SiyouGakunen6_Check = chk随時５_６学年.Checked
                Get_Data(5).SiyouGakunen7_Check = chk随時５_７学年.Checked
                Get_Data(5).SiyouGakunen8_Check = chk随時５_８学年.Checked
                Get_Data(5).SiyouGakunen9_Check = chk随時５_９学年.Checked
        End Select

        Get_Data(6).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分６)
        Get_Data(6).Furikae_Tuki = txt随時振替月６.Text
        Get_Data(6).Furikae_Date = txt随時振替日６.Text

        Select Case chk随時６_全学年.Checked
            Case True
                Get_Data(6).SiyouGakunen1_Check = True
                Get_Data(6).SiyouGakunen2_Check = True
                Get_Data(6).SiyouGakunen3_Check = True
                Get_Data(6).SiyouGakunen4_Check = True
                Get_Data(6).SiyouGakunen5_Check = True
                Get_Data(6).SiyouGakunen6_Check = True
                Get_Data(6).SiyouGakunen7_Check = True
                Get_Data(6).SiyouGakunen8_Check = True
                Get_Data(6).SiyouGakunen9_Check = True
            Case False
                Get_Data(6).SiyouGakunen1_Check = chk随時６_１学年.Checked
                Get_Data(6).SiyouGakunen2_Check = chk随時６_２学年.Checked
                Get_Data(6).SiyouGakunen3_Check = chk随時６_３学年.Checked
                Get_Data(6).SiyouGakunen4_Check = chk随時６_４学年.Checked
                Get_Data(6).SiyouGakunen5_Check = chk随時６_５学年.Checked
                Get_Data(6).SiyouGakunen6_Check = chk随時６_６学年.Checked
                Get_Data(6).SiyouGakunen7_Check = chk随時６_７学年.Checked
                Get_Data(6).SiyouGakunen8_Check = chk随時６_８学年.Checked
                Get_Data(6).SiyouGakunen9_Check = chk随時６_９学年.Checked
        End Select

    End Sub
    Private Sub PSUB_ZUIJI_CLEAR()

        '取得した構造体の初期化

        For i As Integer = 1 To 6
            SYOKI_ZUIJI_SCHINFO(i).Furikae_Date = ""
            SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki = ""
            SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn = 0
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen1_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen2_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen3_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen4_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen5_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen6_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen7_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen8_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen9_Check = False
            SYOKI_ZUIJI_SCHINFO(i).Syori_Flag = False
        Next i

    End Sub
#End Region

#Region " Private Sub(随時スケジュール画面制御)"
    Private Sub PSUB_ZUIJI_FORMAT(Optional ByVal pIndex As Integer = 1)

        'Select case pIndex
        '    Case 0
        '対象学年チェックＢＯＸの有効化
        Call PSUB_ZUIJI_CHKBOXEnabled(True)
        'End Select

        '処理対象学年指定チェックOFF
        Call PSUB_ZUIJI_CHK(False)

        '振替日入力欄、再振替日入力欄のクリア
        Call PSUB_ZUIJI_DAYCLER()

        '入出区分コンボボックス初期
        Call PSUB_ZUIJI_CMB()

        '参照時取得値保持構造体初期化
        Call PSUB_ZUIJI_CLEAR()

    End Sub
    Private Sub PSUB_ZUIJI_CHKBOXEnabled(ByVal pValue As Boolean)

        '対象学年チェックBOXの有効化
        chk随時１_１学年.Enabled = pValue
        chk随時１_２学年.Enabled = pValue
        chk随時１_３学年.Enabled = pValue
        chk随時１_４学年.Enabled = pValue
        chk随時１_５学年.Enabled = pValue
        chk随時１_６学年.Enabled = pValue
        chk随時１_７学年.Enabled = pValue
        chk随時１_８学年.Enabled = pValue
        chk随時１_９学年.Enabled = pValue
        chk随時１_全学年.Enabled = pValue

        chk随時２_１学年.Enabled = pValue
        chk随時２_２学年.Enabled = pValue
        chk随時２_３学年.Enabled = pValue
        chk随時２_４学年.Enabled = pValue
        chk随時２_５学年.Enabled = pValue
        chk随時２_６学年.Enabled = pValue
        chk随時２_７学年.Enabled = pValue
        chk随時２_８学年.Enabled = pValue
        chk随時２_９学年.Enabled = pValue
        chk随時２_全学年.Enabled = pValue

        chk随時３_１学年.Enabled = pValue
        chk随時３_２学年.Enabled = pValue
        chk随時３_３学年.Enabled = pValue
        chk随時３_４学年.Enabled = pValue
        chk随時３_５学年.Enabled = pValue
        chk随時３_６学年.Enabled = pValue
        chk随時３_７学年.Enabled = pValue
        chk随時３_８学年.Enabled = pValue
        chk随時３_９学年.Enabled = pValue
        chk随時３_全学年.Enabled = pValue

        chk随時４_１学年.Enabled = pValue
        chk随時４_２学年.Enabled = pValue
        chk随時４_３学年.Enabled = pValue
        chk随時４_４学年.Enabled = pValue
        chk随時４_５学年.Enabled = pValue
        chk随時４_６学年.Enabled = pValue
        chk随時４_７学年.Enabled = pValue
        chk随時４_８学年.Enabled = pValue
        chk随時４_９学年.Enabled = pValue
        chk随時４_全学年.Enabled = pValue

        chk随時５_１学年.Enabled = pValue
        chk随時５_２学年.Enabled = pValue
        chk随時５_３学年.Enabled = pValue
        chk随時５_４学年.Enabled = pValue
        chk随時５_５学年.Enabled = pValue
        chk随時５_６学年.Enabled = pValue
        chk随時５_７学年.Enabled = pValue
        chk随時５_８学年.Enabled = pValue
        chk随時５_９学年.Enabled = pValue
        chk随時５_全学年.Enabled = pValue

        chk随時６_１学年.Enabled = pValue
        chk随時６_２学年.Enabled = pValue
        chk随時６_３学年.Enabled = pValue
        chk随時６_４学年.Enabled = pValue
        chk随時６_５学年.Enabled = pValue
        chk随時６_６学年.Enabled = pValue
        chk随時６_７学年.Enabled = pValue
        chk随時６_８学年.Enabled = pValue
        chk随時６_９学年.Enabled = pValue
        chk随時６_全学年.Enabled = pValue

    End Sub
    Private Sub PSUB_ZUIJI_DAYCLER()

        '振替日のクリア処理
        txt随時振替月１.Text = ""
        txt随時振替日１.Text = ""
        txt随時振替月２.Text = ""
        txt随時振替日２.Text = ""
        txt随時振替月３.Text = ""
        txt随時振替日３.Text = ""
        txt随時振替月４.Text = ""
        txt随時振替日４.Text = ""
        txt随時振替月５.Text = ""
        txt随時振替日５.Text = ""
        txt随時振替月６.Text = ""
        txt随時振替日６.Text = ""

    End Sub
    Private Sub PSUB_ZUIJI_CHK(ByVal pValue As Boolean)

        '対象学年有効チェックOFF
        chk随時１_１学年.Checked = pValue
        chk随時１_２学年.Checked = pValue
        chk随時１_３学年.Checked = pValue
        chk随時１_４学年.Checked = pValue
        chk随時１_５学年.Checked = pValue
        chk随時１_６学年.Checked = pValue
        chk随時１_７学年.Checked = pValue
        chk随時１_８学年.Checked = pValue
        chk随時１_９学年.Checked = pValue
        chk随時１_全学年.Checked = pValue

        chk随時２_１学年.Checked = pValue
        chk随時２_２学年.Checked = pValue
        chk随時２_３学年.Checked = pValue
        chk随時２_４学年.Checked = pValue
        chk随時２_５学年.Checked = pValue
        chk随時２_６学年.Checked = pValue
        chk随時２_７学年.Checked = pValue
        chk随時２_８学年.Checked = pValue
        chk随時２_９学年.Checked = pValue
        chk随時２_全学年.Checked = pValue

        chk随時３_１学年.Checked = pValue
        chk随時３_２学年.Checked = pValue
        chk随時３_３学年.Checked = pValue
        chk随時３_４学年.Checked = pValue
        chk随時３_５学年.Checked = pValue
        chk随時３_６学年.Checked = pValue
        chk随時３_７学年.Checked = pValue
        chk随時３_８学年.Checked = pValue
        chk随時３_９学年.Checked = pValue
        chk随時３_全学年.Checked = pValue

        chk随時４_１学年.Checked = pValue
        chk随時４_２学年.Checked = pValue
        chk随時４_３学年.Checked = pValue
        chk随時４_４学年.Checked = pValue
        chk随時４_５学年.Checked = pValue
        chk随時４_６学年.Checked = pValue
        chk随時４_７学年.Checked = pValue
        chk随時４_８学年.Checked = pValue
        chk随時４_９学年.Checked = pValue
        chk随時４_全学年.Checked = pValue

        chk随時５_１学年.Checked = pValue
        chk随時５_２学年.Checked = pValue
        chk随時５_３学年.Checked = pValue
        chk随時５_４学年.Checked = pValue
        chk随時５_５学年.Checked = pValue
        chk随時５_６学年.Checked = pValue
        chk随時５_７学年.Checked = pValue
        chk随時５_８学年.Checked = pValue
        chk随時５_９学年.Checked = pValue
        chk随時５_全学年.Checked = pValue

        chk随時６_１学年.Checked = pValue
        chk随時６_２学年.Checked = pValue
        chk随時６_３学年.Checked = pValue
        chk随時６_４学年.Checked = pValue
        chk随時６_５学年.Checked = pValue
        chk随時６_６学年.Checked = pValue
        chk随時６_７学年.Checked = pValue
        chk随時６_８学年.Checked = pValue
        chk随時６_９学年.Checked = pValue
        chk随時６_全学年.Checked = pValue

    End Sub
    Private Sub PSUB_ZUIJI_CMB(Optional ByVal pIndex As Integer = 0)

        cmb入出区分１.SelectedIndex = pIndex
        cmb入出区分２.SelectedIndex = pIndex
        cmb入出区分３.SelectedIndex = pIndex
        cmb入出区分４.SelectedIndex = pIndex
        cmb入出区分５.SelectedIndex = pIndex
        cmb入出区分６.SelectedIndex = pIndex

    End Sub
    Private Sub PSUB_ZUIJI_SET(ByVal cmbBOX As ComboBox, ByVal txtBOX月 As TextBox, ByVal txtBOX日 As TextBox, ByVal chkBOX1 As CheckBox, ByVal chkBOX2 As CheckBox, ByVal chkBOX3 As CheckBox, ByVal chkBOX4 As CheckBox, ByVal chkBOX5 As CheckBox, ByVal chkBOX6 As CheckBox, ByVal chkBOX7 As CheckBox, ByVal chkBOX8 As CheckBox, ByVal chkBOX9 As CheckBox, ByVal chkBOXALL As CheckBox, ByVal aReader As MyOracleReader)

        '現在OPENしているデータベースの内容を画面に表示する（１項目行単位）

        '入出金コンボの設定
        cmbBOX.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_NYUSYUTU2_TXT, aReader.GetString("FURI_KBN_S"))

        txtBOX月.Text = Mid(aReader.GetString("FURI_DATE_S"), 5, 2)
        txtBOX日.Text = Mid(aReader.GetString("FURI_DATE_S"), 7, 2)

        Select Case True
            Case aReader.GetString("ENTRI_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("CHECK_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("DATA_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("FUNOU_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("SAIFURI_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("KESSAI_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("TYUUDAN_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
        End Select

        If aReader.GetString("GAKUNEN1_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN2_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN3_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN4_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN5_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN6_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN7_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN8_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN9_FLG_S") = "1" Then
            '全学年チェックボックスＯＮ
            chkBOXALL.Checked = True

            '１から９学年チェックボクスの使用不可
            chkBOX1.Enabled = False
            chkBOX2.Enabled = False
            chkBOX3.Enabled = False
            chkBOX4.Enabled = False
            chkBOX5.Enabled = False
            chkBOX6.Enabled = False
            chkBOX7.Enabled = False
            chkBOX8.Enabled = False
            chkBOX9.Enabled = False
        Else
            If aReader.GetString("GAKUNEN1_FLG_S") = "1" Then
                '１学年チェックボックスＯＮ
                chkBOX1.Checked = True
            Else
                chkBOX1.Checked = False
            End If

            If aReader.GetString("GAKUNEN2_FLG_S") = "1" Then
                '２学年チェックボックスＯＮ
                chkBOX2.Checked = True
            Else
                chkBOX2.Checked = False
            End If

            If aReader.GetString("GAKUNEN3_FLG_S") = "1" Then
                '３学年チェックボックスＯＮ
                chkBOX3.Checked = True
            Else
                chkBOX3.Checked = False
            End If

            If aReader.GetString("GAKUNEN4_FLG_S") = "1" Then
                '４学年チェックボックスＯＮ
                chkBOX4.Checked = True
            Else
                chkBOX4.Checked = False
            End If

            If aReader.GetString("GAKUNEN5_FLG_S") = "1" Then
                '５学年チェックボックスＯＮ
                chkBOX5.Checked = True
            Else
                chkBOX5.Checked = False
            End If

            If aReader.GetString("GAKUNEN6_FLG_S") = "1" Then
                '６学年チェックボックスＯＮ
                chkBOX6.Checked = True
            Else
                chkBOX6.Checked = False
            End If

            If aReader.GetString("GAKUNEN7_FLG_S") = "1" Then
                '７学年チェックボックスＯＮ
                chkBOX7.Checked = True
            Else
                chkBOX7.Checked = False
            End If

            If aReader.GetString("GAKUNEN8_FLG_S") = "1" Then
                '８学年チェックボックスＯＮ
                chkBOX8.Checked = True
            Else
                chkBOX8.Checked = False
            End If

            If aReader.GetString("GAKUNEN9_FLG_S") = "1" Then
                '９学年チェックボックスＯＮ
                chkBOX9.Checked = True
            Else
                chkBOX9.Checked = False
            End If
        End If

    End Sub
#End Region

#Region " Private Function(随時スケジュール)"
    Private Function PFUNC_SCH_GET_ZUIJI() As Boolean

        PFUNC_SCH_GET_ZUIJI = False

        '随時処理
        '対象学年チェックＢＯＸの有効化
        Call PSUB_ZUIJI_CHKBOXEnabled(True)

        '処理対象学年指定チェックOFF
        Call PSUB_ZUIJI_CHK(False)

        '振替日入力欄のクリア
        Call PSUB_ZUIJI_DAYCLER()

        '随時処理 参照機能
        If PFUNC_ZUIJI_SANSYOU() = False Then
            Exit Function
        End If

        PFUNC_SCH_GET_ZUIJI = True

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_ZUIJI() As Boolean

        '随時スケジュール更新処理
        If PFUNC_ZUIJI_KOUSIN() = False Then

            'ここを通るということは１件でも処理したレコードが存在したということなので
            Int_Syori_Flag(2) = 2

            Return False
        End If

        Return True

    End Function

    Private Function PFUNC_ZUIJI_SANSYOU() As Boolean

        '随時振替日　参照処理
        PFUNC_ZUIJI_SANSYOU = False

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        For i As Integer = 1 To 6
            SYOKI_ZUIJI_SCHINFO(i).Syori_Flag = False
        Next

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = 2")
        sql.Append(" ORDER BY FURI_DATE_S ASC")

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                '空いている項目行にデータベースの内容をセットする
                Select Case True
                    Case (txt随時振替月１.Text = "")
                        Int_Zuiji_Flag = 1
                        Call PSUB_ZUIJI_SET(cmb入出区分１, txt随時振替月１, txt随時振替日１, chk随時１_１学年, chk随時１_２学年, chk随時１_３学年, chk随時１_４学年, chk随時１_５学年, chk随時１_６学年, chk随時１_７学年, chk随時１_８学年, chk随時１_９学年, chk随時１_全学年, oraReader)
                    Case (txt随時振替月２.Text = "")
                        Int_Zuiji_Flag = 2
                        Call PSUB_ZUIJI_SET(cmb入出区分２, txt随時振替月２, txt随時振替日２, chk随時２_１学年, chk随時２_２学年, chk随時２_３学年, chk随時２_４学年, chk随時２_５学年, chk随時２_６学年, chk随時２_７学年, chk随時２_８学年, chk随時２_９学年, chk随時２_全学年, oraReader)
                    Case (txt随時振替月３.Text = "")
                        Int_Zuiji_Flag = 3
                        Call PSUB_ZUIJI_SET(cmb入出区分３, txt随時振替月３, txt随時振替日３, chk随時３_１学年, chk随時３_２学年, chk随時３_３学年, chk随時３_４学年, chk随時３_５学年, chk随時３_６学年, chk随時３_７学年, chk随時３_８学年, chk随時３_９学年, chk随時３_全学年, oraReader)
                    Case (txt随時振替月４.Text = "")
                        Int_Zuiji_Flag = 4
                        Call PSUB_ZUIJI_SET(cmb入出区分４, txt随時振替月４, txt随時振替日４, chk随時４_１学年, chk随時４_２学年, chk随時４_３学年, chk随時４_４学年, chk随時４_５学年, chk随時４_６学年, chk随時４_７学年, chk随時４_８学年, chk随時４_９学年, chk随時４_全学年, oraReader)
                    Case (txt随時振替月５.Text = "")
                        Int_Zuiji_Flag = 5
                        Call PSUB_ZUIJI_SET(cmb入出区分５, txt随時振替月５, txt随時振替日５, chk随時５_１学年, chk随時５_２学年, chk随時５_３学年, chk随時５_４学年, chk随時５_５学年, chk随時５_６学年, chk随時５_７学年, chk随時５_８学年, chk随時５_９学年, chk随時５_全学年, oraReader)
                    Case (txt随時振替月６.Text = "")
                        Int_Zuiji_Flag = 6
                        Call PSUB_ZUIJI_SET(cmb入出区分６, txt随時振替月６, txt随時振替日６, chk随時６_１学年, chk随時６_２学年, chk随時６_３学年, chk随時６_４学年, chk随時６_５学年, chk随時６_６学年, chk随時６_７学年, chk随時６_８学年, chk随時６_９学年, chk随時６_全学年, oraReader)
                End Select

                oraReader.NextRead()

            Loop
        Else

            oraReader.Close()
            Return False

        End If

        oraReader.Close()

        Return True

    End Function
    Private Function PFUNC_ZUIJI_SAKUSEI(ByVal str処理 As String) As Boolean

        '随時振替　作成処理
        Dim str入出区分 As String
        Dim cmbComboName(6) As ComboBox '2006/11/30　コンボボックス名

        PFUNC_ZUIJI_SAKUSEI = False

        '2006/11/30　コンボボックス名を取得
        cmbComboName(1) = cmb入出区分１
        cmbComboName(2) = cmb入出区分２
        cmbComboName(3) = cmb入出区分３
        cmbComboName(4) = cmb入出区分４
        cmbComboName(5) = cmb入出区分５
        cmbComboName(6) = cmb入出区分６

        For i As Integer = 1 To 6

            '新規作成
            '2006/12/06　変更があった欄のみを更新・作成
            If bln随時更新(i) = True And ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And ZUIJI_SCHINFO(i).Furikae_Date <> "" Then

                If PFUNC_GAKUNENFLG_CHECK(ZUIJI_SCHINFO(i).SiyouGakunen1_Check, ZUIJI_SCHINFO(i).SiyouGakunen2_Check, ZUIJI_SCHINFO(i).SiyouGakunen3_Check, ZUIJI_SCHINFO(i).SiyouGakunen4_Check, ZUIJI_SCHINFO(i).SiyouGakunen5_Check, ZUIJI_SCHINFO(i).SiyouGakunen6_Check, ZUIJI_SCHINFO(i).SiyouGakunen7_Check, ZUIJI_SCHINFO(i).SiyouGakunen8_Check, ZUIJI_SCHINFO(i).SiyouGakunen9_Check, ZUIJI_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                    Exit Function
                End If

                str入出区分 = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmbComboName(i))

                'パラメタは�@月、�A入力振替日、�B再振替月　�C再振替日　�D振替区分（入出区分)、�Eスケジュール区分（2:随時)
                If PFUNC_ZUIJI_SAKUSEI_SUB(ZUIJI_SCHINFO(i).Furikae_Tuki, ZUIJI_SCHINFO(i).Furikae_Tuki, ZUIJI_SCHINFO(i).Furikae_Date, "", "", str入出区分) = False Then
                    Exit Function
                End If

                'ここを通るということは処理に成功したということなので
                Int_Syori_Flag(2) = 1
            End If
            'End If

        Next

        PFUNC_ZUIJI_SAKUSEI = True

    End Function

    Private Function PFUNC_ZUIJI_SAKUSEI_SUB(ByVal s請求月 As String, ByVal s月 As String, ByVal s振替日 As String, ByVal s再振替月 As String, ByVal s再振替日 As String, ByVal s振替区分 As String) As Boolean

        'スケジュール作成　随時レコード作成
        PFUNC_ZUIJI_SAKUSEI_SUB = False
        '請求年月の作成
        STR請求年月 = PFUNC_SEIKYUTUKIHI(s請求月)
        '振替日算出
        STR振替日 = PFUNC_FURIHI_MAKE(s月, s振替日, "2", s振替区分)

        '2010/10/21 契約振替日を算出する
        STR契約振替日 = PFUNC_KFURIHI_MAKE(s月, s振替日, "2", s振替区分)
        '再振日
        STR再振替日 = "00000000"

        'スケジュール区分の共通変数設定
        STRスケ区分 = "2"
        '振替区分の共通変数設定
        STR振替区分 = s振替区分
        '入力振替日の共通変数設定
        STR年間入力振替日 = Space(15)

        Dim strSQL As String = ""
        'スケジュールマスタ登録(初振)SQL文作成
        strSQL = PSUB_INSERT_G_SCHMAST_SQL()

        If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            Return False
        End If

        '-----------------------------------------------
        '2006/07/26　企業自振の随時のスケジュールも作成
        '-----------------------------------------------
        '企業自振連携時のみ
        Dim strTORIF_CODE_N As String
        If STR振替区分 = "2" Then  '入金
            strTORIF_CODE_N = "03"
        Else  '出金
            strTORIF_CODE_N = "04"
        End If

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '既に登録されているかチェック
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '" & strTORIF_CODE_N & "' AND ")
        sql.Append("FURI_DATE_S = '" & STR振替日 & "'")

        '読込のみ
        If oraReader.DataReader(sql) = True Then    'スケジュールが既に存在する
        Else     'スケジュールが存在しない
            'スケジュール作成
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '取引先マスタに取引先コードが存在することを確認
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, strTORIF_CODE_N, gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                     gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '検索にヒットしなかったら

                '2010/10/21 契約振替日対応 引数に追加
                'If fn_INSERTSCHMAST(strGakkouCode, strTORIF_CODE_N, STR振替日, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, strTORIF_CODE_N, STR振替日, gintPG_KBN.KOBETU, STR契約振替日) = gintKEKKA.NG Then
                    oraReader.Close()
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", "企業自振のスケジュールが登録できませんでした")
                    MessageBox.Show("企業自振のスケジュールが登録できませんでした", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If
        End If
        oraReader.Close()

        '再振レコード作成はない
        PFUNC_ZUIJI_SAKUSEI_SUB = True

    End Function

    Private Function PFUNC_ZUIJI_KOUSIN() As Boolean

        '削除処理（DELETE）
        If PFUNC_ZUIJI_DELETE() = False Then
            Return False
        End If

        '2010/10/21 随時スケジュールの変更に対応する
        '削除されたレコードの更新フラグがFalseとなっているため、もう一度、作成して良いかチェックする
        For i As Integer = 1 To 6
            '--------------------------------------
            '随時スケジュールチェック
            '--------------------------------------
            '2006/12/12　一部追加：入力が不足していた場合、更新しない
            If (ZUIJI_SCHINFO(i).Furikae_Tuki = SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki And _
               ZUIJI_SCHINFO(i).Furikae_Date = SYOKI_ZUIJI_SCHINFO(i).Furikae_Date And _
               ZUIJI_SCHINFO(i).Nyusyutu_Kbn = SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn And _
               ZUIJI_SCHINFO(i).SiyouGakunen1_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen1_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen2_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen2_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen3_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen3_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen4_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen4_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen5_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen5_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen6_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen6_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen7_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen7_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen8_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen8_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen9_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen9_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunenALL_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunenALL_Check) Or _
               ((ZUIJI_SCHINFO(i).Furikae_Tuki = "" And ZUIJI_SCHINFO(i).Furikae_Date <> "") Or _
               (ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And ZUIJI_SCHINFO(i).Furikae_Date = "")) Then

                bln随時更新(i) = False '変更なし
            Else
                bln随時更新(i) = True ' 変更あり
            End If
        Next

        '作成処理（INSERT & UPDATE)
        If PFUNC_ZUIJI_SAKUSEI("更新") = False Then
            Return False
        End If

        Return True

    End Function

    '===============================
    '随時データ削除処理　2006/11/30
    '===============================
    Private Function PFUNC_ZUIJI_DELETE() As Boolean

        Dim sql As New StringBuilder(128)
        Dim bret As Boolean = False
        Dim blnSakujo_Check As Boolean = False '2006/11/30
        Dim strNengetu As String '   処理年月
        Dim strSFuri_Date As String '再振日

        '全削除処理、キーは学校コード、対象年度、スケジュール区分（２：随時）、処理フラグ（０）

        sql.Append(" DELETE  FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S =2")

        '2006/11/30　条件変更（フラグの立っていないデータ・変更のあったデータを削除）
        sql.Append(" AND")
        sql.Append(" (ENTRI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" CHECK_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" DATA_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" FUNOU_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" SAIFURI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" KESSAI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" TYUUDAN_FLG_S =0)")

        For i As Integer = 1 To 6

            '変更があったものを削除する（随時スケジュールは常に再作成可能とする）
            If bln随時更新(i) = True And SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And SYOKI_ZUIJI_SCHINFO(i).Furikae_Date <> "" Then

                '年月度を取得
                If CInt(SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki) < 4 Then
                    strNengetu = CInt(txt対象年度.Text) + 1 & SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki
                Else
                    strNengetu = txt対象年度.Text & SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki
                End If

                '再振日は "0" 8桁
                strSFuri_Date = "00000000"

                If blnSakujo_Check = True Then
                    sql.Append(" or")
                Else
                    '2010/10/21 orだと全ての随時スケジュールが削除されてしまう
                    'sql.Append(" or(")
                    sql.Append(" AND (")
                End If

                '条件追加
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_ZUIJI_SCHINFO(i).Furikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & strSFuri_Date & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '" & SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn & "')")

                bln随時更新(i) = False '変更フラグを降ろす
                blnSakujo_Check = True '削除フラグを立てる

            End If
        Next

        If blnSakujo_Check = True Then
            sql.Append(")")
        End If

        '2006/12/11　削除する対象が一件も無かったら実行しない
        If blnSakujo_Check = True Then

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                '削除処理エラー
                MessageBox.Show("(随時スケジュール)" & vbCrLf & "スケジュールの削除処理でエラーが発生しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                bret = False
            End If

        End If

        Return True

    End Function

    Private Sub PSUB_ZGAKUNEN_CHK()
        '2006/10/12　使用していない学年のチェックボックスを使用不可にする

        If GAKKOU_INFO.SIYOU_GAKUNEN <> 9 Then
            chk随時１_９学年.Enabled = False
            chk随時２_９学年.Enabled = False
            chk随時３_９学年.Enabled = False
            chk随時４_９学年.Enabled = False
            chk随時５_９学年.Enabled = False
            chk随時６_９学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 8 Then
            chk随時１_８学年.Enabled = False
            chk随時２_８学年.Enabled = False
            chk随時３_８学年.Enabled = False
            chk随時４_８学年.Enabled = False
            chk随時５_８学年.Enabled = False
            chk随時６_８学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 7 Then
            chk随時１_７学年.Enabled = False
            chk随時２_７学年.Enabled = False
            chk随時３_７学年.Enabled = False
            chk随時４_７学年.Enabled = False
            chk随時５_７学年.Enabled = False
            chk随時６_７学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 6 Then
            chk随時１_６学年.Enabled = False
            chk随時２_６学年.Enabled = False
            chk随時３_６学年.Enabled = False
            chk随時４_６学年.Enabled = False
            chk随時５_６学年.Enabled = False
            chk随時６_６学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 5 Then
            chk随時１_５学年.Enabled = False
            chk随時２_５学年.Enabled = False
            chk随時３_５学年.Enabled = False
            chk随時４_５学年.Enabled = False
            chk随時５_５学年.Enabled = False
            chk随時６_５学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 4 Then
            chk随時１_４学年.Enabled = False
            chk随時２_４学年.Enabled = False
            chk随時３_４学年.Enabled = False
            chk随時４_４学年.Enabled = False
            chk随時５_４学年.Enabled = False
            chk随時６_４学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 3 Then
            chk随時１_３学年.Enabled = False
            chk随時２_３学年.Enabled = False
            chk随時３_３学年.Enabled = False
            chk随時４_３学年.Enabled = False
            chk随時５_３学年.Enabled = False
            chk随時６_３学年.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 2 Then
            chk随時１_２学年.Enabled = False
            chk随時２_２学年.Enabled = False
            chk随時３_２学年.Enabled = False
            chk随時４_２学年.Enabled = False
            chk随時５_２学年.Enabled = False
            chk随時６_２学年.Enabled = False
        End If
    End Sub

#End Region

#Region "関数"

    Public Function fn_DELETESCHMAST(ByVal astrTORIF_CODE As String, ByVal astrFURI_DATE As String) As Boolean
        '----------------------------------------------------------------------------
        'Name       :fn_UPDATE_SCHMAST
        'Description:SCHMAST更新(有効フラグ)
        'Create     :
        'UPDATE     :
        '----------------------------------------------------------------------------

        '企業自振のスケジュールを削除
        Dim ret As Boolean = False

        Dim SQL As New System.Text.StringBuilder(128)

        Try
            SQL.Append(" DELETE  FROM SCHMAST ")
            SQL.Append(" WHERE TORIS_CODE_S = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
            SQL.Append(" AND TORIF_CODE_S = '" & astrTORIF_CODE & "'")
            SQL.Append(" AND FURI_DATE_S = '" & astrFURI_DATE & "'")
            SQL.Append(" AND UKETUKE_FLG_S = '0'")
            SQL.Append(" AND TOUROKU_FLG_S = '0'")
            SQL.Append(" AND HAISIN_FLG_S = '0'")

            If MainDB.ExecuteNonQuery(SQL) < 0 Then
                MainLOG.Write("自振スケジュールDELETE", "失敗", "SQL:" & SQL.ToString)
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            MainLOG.Write("自振スケジュールDELETE", "失敗", "SQL:" & SQL.ToString & "DETAIL:" & ex.ToString)
        End Try

        Return ret

    End Function

#End Region

#Region "INSERTSCHMAST"
    '
    '　関数名　-　fn_INSERTSCHMAST
    '
    '　機能    -  スケジュール作成
    '
    '　引数    -  TORIS_CODE , TORIF_CODE,FURI_DATE,TIME_STAMP,PG_KUBUN 1:個別 　2:一括
    '
    '　備考    -  通常、随時共に初期化
    '
    '
    '2010/10/21 契約振替日対応 契約振替日を引数に追加(省略化)
    'Private Function fn_INSERTSCHMAST(ByVal aTORIS_CODE As String, ByVal aTORIF_CODE As String, ByVal aFURI_DATE As String, ByVal aPG_KUBUN As Integer) As Integer
    Private Function fn_INSERTSCHMAST(ByVal aTORIS_CODE As String, ByVal aTORIF_CODE As String, ByVal aFURI_DATE As String, ByVal aPG_KUBUN As Integer, Optional ByVal aKFURI_DATE As String = "") As Integer
        '----------------------------------------------------------------------------
        'Name       :fn_insert_SCHMAST
        'Description:スケジュール作成
        'Parameta   :TORIS_CODE , TORIF_CODE,FURI_DATE,TIME_STAMP,PG_KUBUN 1:個別 　2:一括
        'Create     :2004/08/02
        'UPDATE     :2007/12/26
        '           :***修正　にｶｽﾀﾏｲｽﾞ (企業自振ｽｹｼﾞｭｰﾙﾏｽﾀ生成時に企業側ｽｹｼﾞｭｰﾙﾏｽﾀの項目追加の為）
        '----------------------------------------------------------------------------

        Dim RetCode As Integer = gintKEKKA.NG

        Dim oraReader As New MyOracleReader(MainDB)

        Try
            Dim SQL As StringBuilder
            Dim SCH_DATA(77) As String
            Dim strFURI_DATE As String
            Dim Ret As String

            Dim CLS As New GAKKOU.ClsSchduleMaintenanceClass
            CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            strFURI_DATE = aFURI_DATE.Substring(0, 4) & "/" & aFURI_DATE.Substring(4, 2) & "/" & aFURI_DATE.Substring(6, 2)

            '----------------
            '取引先マスタ検索
            '----------------
            SQL = New StringBuilder(128)

            SQL.Append(" SELECT * FROM TORIMAST ")
            SQL.Append(" WHERE TORIS_CODE_T = '" & aTORIS_CODE.Trim & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & aTORIF_CODE.Trim & "'")

            If oraReader.DataReader(SQL) = False Then
                MessageBox.Show("取引先マスタに再振取引先が登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                RetCode = gintKEKKA.NG
                Return RetCode
            End If

            '-------------------------------------
            '振替日は営業日の営業日判定（土・日・祝祭日判定）
            '-------------------------------------
            'スケジュール作成対象の取引先コードを抽出
            CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(aFURI_DATE), aTORIS_CODE, aTORIF_CODE)

            CLS.SCH.FURI_DATE = GCom.SET_DATE(aFURI_DATE)
            If CLS.SCH.FURI_DATE = "00000000" Then
            Else
                CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
            End If

            strFURI_DATE = CLS.SCH.FURI_DATE

            '2010/10/21 契約振替日対応 ここから
            If aKFURI_DATE = "" OrElse aKFURI_DATE.Length <> 8 Then
                '引数がない場合は実振替日を設定
                CLS.SCH.KFURI_DATE = strFURI_DATE
            Else
                CLS.SCH.KFURI_DATE = aKFURI_DATE
            End If
            '2010/10/21 契約振替日対応 ここまで

            Ret = CLS.INSERT_NEW_SCHMAST(0, False, True)

            '------------------
            'マスタ登録項目設定
            '------------------
            SCH_DATA(0) = oraReader.GetString("FSYORI_KBN_T")                                       '振替処理区分
            SCH_DATA(1) = aTORIS_CODE                                                           '取引先主コード
            SCH_DATA(2) = aTORIF_CODE                                                           '取引先副コード
            SCH_DATA(3) = CLS.SCH.FURI_DATE 'strIN_NEN & strIN_TUKI & strIN_HI 'FURI_DATE_S　 　'振替日
            '2010/10/21 契約振替日対応
            'SCH_DATA(4) = CLS.SCH.FURI_DATE '"00000000" 'SAIFURI_DATE_S                         '契約振替日=振替日
            SCH_DATA(4) = CLS.SCH.KFURI_DATE                                                    '契約振替日
            SCH_DATA(5) = "00000000"                                                            '再振日
            SCH_DATA(6) = CLS.SCH.KSAIFURI_DATE                                                 '再振予定日
            SCH_DATA(7) = CStr(ConvNullToString(oraReader.GetString("FURI_CODE_T"))).PadLeft(3, "0")  '振替コードＳ
            SCH_DATA(8) = CStr(ConvNullToString(oraReader.GetString("KIGYO_CODE_T"))).PadLeft(4, "0") '企業コードＳ
            SCH_DATA(9) = CLS.TR(0).ITAKU_CODE '委託者コード
            SCH_DATA(10) = CStr(oraReader.GetString("TKIN_NO_T")).PadLeft(4, "0")
            SCH_DATA(11) = CStr(oraReader.GetString("TSIT_NO_T")).PadLeft(3, "0")
            SCH_DATA(12) = oraReader.GetString("SOUSIN_KBN_T")
            SCH_DATA(13) = oraReader.GetString("MOTIKOMI_KBN_T")
            SCH_DATA(14) = oraReader.GetString("BAITAI_CODE_T") 'BAITAI_CODE_S
            SCH_DATA(15) = 0 'MOTIKOMI_SEQ_S
            SCH_DATA(16) = 0 'FILE_SEQ_S
            '手数料計算区分の算出
            Dim strTUKI_KBN As String = ""
            Select Case aFURI_DATE.Substring(4, 2)
                Case "01"
                    strTUKI_KBN = oraReader.GetString("TUKI1_T")
                Case "02"
                    strTUKI_KBN = oraReader.GetString("TUKI2_T")
                Case "03"
                    strTUKI_KBN = oraReader.GetString("TUKI3_T")
                Case "04"
                    strTUKI_KBN = oraReader.GetString("TUKI4_T")
                Case "05"
                    strTUKI_KBN = oraReader.GetString("TUKI5_T")
                Case "06"
                    strTUKI_KBN = oraReader.GetString("TUKI6_T")
                Case "07"
                    strTUKI_KBN = oraReader.GetString("TUKI7_T")
                Case "08"
                    strTUKI_KBN = oraReader.GetString("TUKI8_T")
                Case "09"
                    strTUKI_KBN = oraReader.GetString("TUKI9_T")
                Case "10"
                    strTUKI_KBN = oraReader.GetString("TUKI10_T")
                Case "11"
                    strTUKI_KBN = oraReader.GetString("TUKI11_T")
                Case "12"
                    strTUKI_KBN = oraReader.GetString("TUKI12_T")
            End Select

            Select Case oraReader.GetString("TESUUTYO_KBN_T")
                Case 0
                    SCH_DATA(17) = "1"          'TESUU_KBN_S
                Case 1
                    Select Case strTUKI_KBN
                        Case "1", "3"
                            SCH_DATA(17) = "2"
                        Case Else
                            SCH_DATA(17) = "3"
                    End Select
                Case 2
                    SCH_DATA(17) = "0"
                Case Else
                    SCH_DATA(17) = "0"
            End Select

            SCH_DATA(18) = "00000000"              '依頼書作成日
            SCH_DATA(19) = CLS.SCH.IRAISYOK_YDATE  '依頼書回収予定日
            SCH_DATA(20) = CLS.SCH.MOTIKOMI_DATE   'MOTIKOMI_DATE_S
            SCH_DATA(21) = "00000000"              'UKETUKE_DATE_S   
            SCH_DATA(22) = "00000000"              'TOUROKU_DATE_S
            SCH_DATA(23) = CLS.SCH.HAISIN_YDATE    'HAISIN_YDATE_S
            SCH_DATA(24) = "00000000"              'HAISIN_DATE_S
            SCH_DATA(25) = CLS.SCH.HAISIN_YDATE    'SOUSIN_YDATE_S
            SCH_DATA(26) = "00000000"              'SOUSIN_DATE_S
            SCH_DATA(27) = CLS.SCH.FUNOU_YDATE     'FUNOU_YDATE_S
            SCH_DATA(28) = "00000000"              'FUNOU_DATE_S
            SCH_DATA(29) = CLS.SCH.KESSAI_YDATE    'KESSAI_YDATE_S
            SCH_DATA(30) = "00000000"              'KESSAI_DATE_S
            SCH_DATA(31) = CLS.SCH.TESUU_YDATE     'TESUU_YDATE_S
            SCH_DATA(32) = "00000000"              'TESUU_DATE_S
            SCH_DATA(33) = CLS.SCH.HENKAN_YDATE    'HENKAN_YDATE_S
            SCH_DATA(34) = "00000000"              'HENKAN_DATE_S
            SCH_DATA(35) = "00000000"              'UKETORI_DATE_S
            SCH_DATA(36) = "0"                     'UKETUKE_FLG_S
            SCH_DATA(37) = "0"                     'TOUROKU_FLG_S
            SCH_DATA(38) = "0"                     'HAISIN_FLG_S
            SCH_DATA(39) = "0"                     'SAIFURI_FLG_S
            SCH_DATA(40) = "0"                     'SOUSIN_FLG_S
            SCH_DATA(41) = "0"                     'FUNOU_FLG_S
            SCH_DATA(42) = "0"                     'TESUUKEI_FLG_S
            SCH_DATA(43) = "0"                     'TESUUTYO_FLG_S
            SCH_DATA(44) = "0"                     'KESSAI_FLG_S
            SCH_DATA(45) = "0"                     'HENKAN_FLG_S
            SCH_DATA(46) = "0"                     'TYUUDAN_FLG_S
            SCH_DATA(47) = "0"                     'TAKOU_FLG_S
            SCH_DATA(48) = "0"                     'NIPPO_FLG_S
            SCH_DATA(49) = Space(3)                'ERROR_INF_S
            SCH_DATA(50) = 0                       'SYORI_KEN_S
            SCH_DATA(51) = 0                       'SYORI_KIN_S
            SCH_DATA(52) = 0                       'ERR_KEN_S
            SCH_DATA(53) = 0                       'ERR_KIN_S
            SCH_DATA(54) = 0                       'TESUU_KIN_S
            SCH_DATA(55) = 0                       'TESUU_KIN1_S
            SCH_DATA(56) = 0                       'TESUU_KIN2_S
            SCH_DATA(57) = 0                       'TESUU_KIN3_S
            SCH_DATA(58) = 0                       'FURI_KEN_S
            SCH_DATA(59) = 0                       'FURI_KIN_S
            SCH_DATA(60) = 0                       'FUNOU_KEN_S
            SCH_DATA(61) = 0                       'FUNOU_KIN_S
            SCH_DATA(62) = Space(50)               'UFILE_NAME_S
            SCH_DATA(63) = Space(50)               'SFILE_NAME_S
            SCH_DATA(64) = Format(Now, "yyyyMMdd") 'SAKUSEI_DATE_S
            SCH_DATA(65) = Space(14)               'JIFURI_TIME_STAMP_S
            SCH_DATA(66) = Space(14)               'KESSAI_TIME_STAMP_S
            SCH_DATA(67) = Space(14)               'TESUU_TIME_STAMP_S
            SCH_DATA(68) = Space(15)               'YOBI1_S
            SCH_DATA(69) = Space(15)               'YOBI2_S
            SCH_DATA(70) = Space(15)               'YOBI3_S
            SCH_DATA(71) = Space(15)               'YOBI4_S
            SCH_DATA(72) = Space(15)               'YOBI5_S
            SCH_DATA(73) = Space(15)               'YOBI6_S
            SCH_DATA(74) = Space(15)               'YOBI7_S
            SCH_DATA(75) = Space(15)               'YOBI8_S
            SCH_DATA(76) = Space(15)               'YOBI9_S
            SCH_DATA(77) = Space(15)               'YOBI10_S

            '----------------------
            'スケジュールマスタ登録
            '----------------------
            SQL = New StringBuilder(1024)

            SQL.Append("INSERT INTO SCHMAST ( ")
            SQL.Append("FSYORI_KBN_S")      '0
            SQL.Append(",TORIS_CODE_S")     '1
            SQL.Append(",TORIF_CODE_S")     '2
            SQL.Append(",FURI_DATE_S")      '3
            SQL.Append(",KFURI_DATE_S")     '4
            SQL.Append(",SAIFURI_DATE_S")   '5
            SQL.Append(",KSAIFURI_DATE_S")  '6
            SQL.Append(",FURI_CODE_S")      '7
            SQL.Append(",KIGYO_CODE_S")     '8
            SQL.Append(",ITAKU_CODE_S")     '9
            SQL.Append(",TKIN_NO_S")        '10
            SQL.Append(",TSIT_NO_S")        '11
            SQL.Append(",SOUSIN_KBN_S")     '12
            SQL.Append(",MOTIKOMI_KBN_S")   '13
            SQL.Append(",BAITAI_CODE_S")    '14
            SQL.Append(",MOTIKOMI_SEQ_S")   '15
            SQL.Append(",FILE_SEQ_S")       '16
            SQL.Append(",TESUU_KBN_S")      '17
            SQL.Append(",IRAISYO_DATE_S")   '18
            SQL.Append(",IRAISYOK_YDATE_S") '19
            SQL.Append(",MOTIKOMI_DATE_S")  '20
            SQL.Append(",UKETUKE_DATE_S")   '21
            SQL.Append(",TOUROKU_DATE_S")   '22
            SQL.Append(",HAISIN_YDATE_S")   '23
            SQL.Append(",HAISIN_DATE_S")    '24
            SQL.Append(",SOUSIN_YDATE_S")   '25
            SQL.Append(",SOUSIN_DATE_S")    '26
            SQL.Append(",FUNOU_YDATE_S")    '27
            SQL.Append(",FUNOU_DATE_S")     '28
            SQL.Append(",KESSAI_YDATE_S")   '29
            SQL.Append(",KESSAI_DATE_S")    '30
            SQL.Append(",TESUU_YDATE_S")    '31
            SQL.Append(",TESUU_DATE_S")     '32
            SQL.Append(",HENKAN_YDATE_S")   '33
            SQL.Append(",HENKAN_DATE_S")    '34
            SQL.Append(",UKETORI_DATE_S")   '35
            SQL.Append(",UKETUKE_FLG_S")    '36
            SQL.Append(",TOUROKU_FLG_S")    '37
            SQL.Append(",HAISIN_FLG_S")     '38
            SQL.Append(",SAIFURI_FLG_S")    '39
            SQL.Append(",SOUSIN_FLG_S")     '40
            SQL.Append(",FUNOU_FLG_S")      '41
            SQL.Append(",TESUUKEI_FLG_S")   '42
            SQL.Append(",TESUUTYO_FLG_S")   '43
            SQL.Append(",KESSAI_FLG_S")     '44
            SQL.Append(",HENKAN_FLG_S")     '45
            SQL.Append(",TYUUDAN_FLG_S")    '46
            SQL.Append(",TAKOU_FLG_S")      '47
            SQL.Append(",NIPPO_FLG_S")      '48
            SQL.Append(",ERROR_INF_S")      '49
            SQL.Append(",SYORI_KEN_S")      '50
            SQL.Append(",SYORI_KIN_S")      '51
            SQL.Append(",ERR_KEN_S")        '52
            SQL.Append(",ERR_KIN_S")        '53
            SQL.Append(",TESUU_KIN_S")      '54
            SQL.Append(",TESUU_KIN1_S")     '55
            SQL.Append(",TESUU_KIN2_S")     '56
            SQL.Append(",TESUU_KIN3_S")     '57
            SQL.Append(",FURI_KEN_S")       '58
            SQL.Append(",FURI_KIN_S")       '59
            SQL.Append(",FUNOU_KEN_S")      '60
            SQL.Append(",FUNOU_KIN_S")      '61
            SQL.Append(",UFILE_NAME_S")     '62
            SQL.Append(",SFILE_NAME_S")     '63
            SQL.Append(",SAKUSEI_DATE_S")   '64
            SQL.Append(",JIFURI_TIME_STAMP_S")      '65
            SQL.Append(",KESSAI_TIME_STAMP_S")      '66
            SQL.Append(",TESUU_TIME_STAMP_S")       '67
            SQL.Append(",YOBI1_S")          '68
            SQL.Append(",YOBI2_S")          '69
            SQL.Append(",YOBI3_S")          '70
            SQL.Append(",YOBI4_S")          '71
            SQL.Append(",YOBI5_S")          '72
            SQL.Append(",YOBI6_S")          '73
            SQL.Append(",YOBI7_S")          '74
            SQL.Append(",YOBI8_S")          '75
            SQL.Append(",YOBI9_S")          '76
            SQL.Append(",YOBI10_S")         '77
            SQL.Append(" ) VALUES ( ")
            For cnt As Integer = LBound(SCH_DATA) To UBound(SCH_DATA)
                SQL.Append("'" & SCH_DATA(cnt) & "',")
            Next

            Dim InsertSchmastSQL As String = SQL.ToString

            InsertSchmastSQL = InsertSchmastSQL.Substring(0, SQL.Length - 1) & ")"

            If MainDB.ExecuteNonQuery(InsertSchmastSQL) < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", SQL.ToString)
                Return False
                ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- START
            Else
                If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                    Dim ReturnMessage As String = String.Empty
                    Dim SubMastInsert_Ret As Integer = 0
                    Call CAstExternal.ModExternal.Ex_InsertSchmastSub(SCH_DATA(0), _
                                                                      SCH_DATA(1), _
                                                                      SCH_DATA(2), _
                                                                      SCH_DATA(3), _
                                                                      0, _
                                                                      ReturnMessage, _
                                                                      MainDB)
                End If
                ' 2016/10/14 タスク）綾部 ADD 【ME】UI_B-99-99(RSV2対応) -------------------- END
            End If

            RetCode = gintKEKKA.OK

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "予期せぬエラー", "失敗", ex.ToString)
            RetCode = gintKEKKA.NG

            Return RetCode

        Finally

            If Not oraReader Is Nothing Then oraReader.Close()

        End Try

        Return RetCode

    End Function
#End Region

#Region " 入力Key制御関数"

    Public Function GFUNC_KEYCHECK(ByRef P_FORM As Form, _
                                   ByRef P_e As System.Windows.Forms.KeyPressEventArgs, _
                                   ByVal P_Mode As Integer) As Boolean
        GFUNC_KEYCHECK = False

        '*****************************************
        '入力KEY制御
        '*****************************************
        'ENTERキーで次ControlへFocus移動
        If P_e.KeyChar = ChrW(13) Then
            P_FORM.SelectNextControl(P_FORM.ActiveControl, True, True, True, True)
        End If

        'BS・TAB・ENTERキー入力時はスキップ
        Select Case P_e.KeyChar
            Case ControlChars.Back, ControlChars.Tab, ChrW(13)

                Exit Function
        End Select

        Select Case P_Mode
            Case 1
                If (P_e.KeyChar < "0"c Or P_e.KeyChar > "9"c) Then
                    P_e.Handled = True
                End If
            Case 2
                If (P_e.KeyChar >= "0"c Or P_e.KeyChar <= "9"c) Or _
                   (P_e.KeyChar >= "A"c Or P_e.KeyChar <= "Z"c) Or _
                   (P_e.KeyChar >= "a"c Or P_e.KeyChar <= "z"c) Then
                Else
                    P_e.Handled = True
                End If
            Case 3
                If (P_e.KeyChar >= "A"c Or P_e.KeyChar <= "Z"c) Or _
                   (P_e.KeyChar >= "a"c Or P_e.KeyChar <= "z"c) Then
                Else
                    P_e.Handled = True
                End If
            Case 5
                If (P_e.KeyChar < "ｱ"c Or P_e.KeyChar > "ﾝ"c) Then
                    P_e.Handled = True
                End If
            Case 6 '2007/02/12　フラグ用
                If (P_e.KeyChar < "0"c Or P_e.KeyChar > "1"c) Then
                    P_e.Handled = True
                End If
            Case 10
                If (P_e.KeyChar < "1"c Or P_e.KeyChar > "9"c) Then
                    P_e.Handled = True
                End If
        End Select

        GFUNC_KEYCHECK = True
    End Function
    Public Sub GSUB_PRESEL(ByRef pTxtFile As TextBox)
        'TEXTｵﾌﾞｼﾞｪｸﾄの内容を全選択
        pTxtFile.SelectionStart = 0
        pTxtFile.SelectionLength = Len(pTxtFile.Text)
    End Sub
    Public Sub GSUB_NEXTFOCUS(ByRef P_FORM As Form, _
                              ByRef P_e As System.Windows.Forms.KeyEventArgs, _
                              ByRef pTxtFile As TextBox)

        Select Case P_e.KeyData
            Case Keys.Right, Keys.Left
                '→・←ボタン
                Exit Sub
            Case Keys.Back, Keys.Tab, Keys.Enter
                'BS・TAB・ENTERキー
                Exit Sub
            Case Keys.ShiftKey, 65545
                'Shift + Tabキー(KeyUpなのでShiftキー単体も必要)
                Exit Sub
        End Select

        '入力桁と最大入力桁数が一致すればFocus移動
        If pTxtFile.MaxLength = Len(Trim(pTxtFile.Text)) Then
            P_FORM.SelectNextControl(P_FORM.ActiveControl, True, True, True, True)
        End If

    End Sub
#End Region

#Region " 指定桁前ZERO詰め共通関数"
    Public Function GFUNC_ZERO_ADD(ByRef pTxtFile As TextBox, _
                                   ByVal pKeta As Byte) As Boolean
        GFUNC_ZERO_ADD = False
        pTxtFile.Text = pTxtFile.Text.Trim.PadLeft(pKeta, "0"c)
        GFUNC_ZERO_ADD = True
    End Function

#End Region

    '
    '　関数名　-　fn_ToriMastIsExist
    '
    '　機能    -  取引先マスタ存在チェック
    '
    '　引数    -  
    '
    '　備考    -  通常、随時共に初期化
    '
    '
    Private Function fn_IsExistToriMast(ByVal TorisCode As String, _
                                        ByVal TorifCode As String, _
                                        ByRef ItakuKName As String, _
                                        ByRef ItakuNName As String, _
                                        ByRef KigyoCode As String, _
                                        ByRef FuriCode As String, _
                                        ByRef BaitaiCode As String, _
                                        ByRef FmtKbn As String, _
                                        ByRef FileName As String) As Boolean

        Dim ret As Boolean = False
        Dim OraReader As New MyOracleReader(MainDB)

        Try
            Dim SQL As String = ""
            SQL = " SELECT * "
            SQL &= " FROM TORIMAST "
            SQL &= " WHERE TORIS_CODE_T = '" & TorisCode & "'"
            SQL &= " AND TORIF_CODE_T = '" & TorifCode & "'"

            If OraReader.DataReader(SQL) = False Then
                ret = False
            Else
                ItakuKName = OraReader.GetString("ITAKU_KNAME_T")
                ItakuNName = OraReader.GetString("ITAKU_NNAME_T")
                KigyoCode = OraReader.GetString("KIGYO_CODE_T")
                FuriCode = OraReader.GetString("FURI_CODE_T")
                BaitaiCode = OraReader.GetString("BAITAI_CODE_T")
                FmtKbn = OraReader.GetString("FMT_KBN_T")
                FileName = OraReader.GetString("FILE_NAME_T")

                ret = True
            End If

            OraReader.Close()
            OraReader = Nothing

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "予期せぬエラー", "失敗", ex.ToString)
            ret = False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校検索
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            Exit Sub
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then

            Exit Sub
        End If

        '年間スケジュール画面初期化
        Call PSUB_NENKAN_FORMAT()

        '特別スケジュール画面初期化
        Call PSUB_TOKUBETU_FORMAT()

        '随時スケジュール画面初期化
        Call PSUB_ZUIJI_FORMAT()

        '学校検索後の学校コード設定
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
        '2007/02/15
        txtGAKKOU_CODE.Focus()

        '学校名の取得(学校情報も変数に格納される)
        If PFUNC_GAKINFO_GET() = False Then
            Exit Sub
        End If

        '2006/10/12　最高学年以上の学年の使用不可
        PSUB_TGAKUNEN_CHK()
        PSUB_ZGAKUNEN_CHK()

        '再振替日のプロテクトTrue
        Call PSUB_SAIFURI_PROTECT(True)

        Select Case GAKKOU_INFO.SFURI_SYUBETU
            Case "0", "3"
                Call PSUB_SAIFURI_PROTECT(False)
        End Select
        '2007/02/15
        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txt対象年度.Text) <> "" Then
            '対象年度も入力されている場合、スケジュール存在チェックをかけ
            'スケジュールが存在する場合は参照ボタンにフォーカス移動
            Call PSUB_SANSYOU_FOCUS()
        End If

    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

#Region "特別振替日入力チェック"
    '2011/06/16 標準版修正 特別振替日相関チェック追加 ------------------START
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False
        Try
            '特別請求日１
            If txt特別請求月１.Text.Trim <> "" Then
                If txt特別振替月１.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別振替月１.Focus()
                    Return False
                Else
                    If txt特別振替日１.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "特別振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別振替日１.Focus()
                        Return False
                    End If
                End If
            Else
                If txt特別振替日１.Text.Trim <> "" OrElse txt特別振替月１.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別請求月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別請求月１.Focus()
                    Return False
                End If
            End If
            If txt特別再振替月１.Text.Trim = "" Then
                If txt特別再振替日１.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替月１.Focus()
                    Return False
                End If
            Else
                If txt特別再振替日１.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替日１.Focus()
                    Return False
                Else
                    '振替日、再振日相関チェック
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '初振日設定
                    If CInt(txt特別振替月１.Text) >= 1 AndAlso CInt(txt特別振替月１.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別振替月１.Text & txt特別振替日１.Text
                    Else
                        FURI_DATE = txt対象年度.Text & txt特別振替月１.Text & txt特別振替日１.Text
                    End If
                    '再振日設定
                    If CInt(txt特別再振替月１.Text) >= 1 AndAlso CInt(txt特別再振替月１.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月１.Text & txt特別再振替日１.Text
                    Else
                        If txt特別振替月１.Text = "03" AndAlso txt特別再振替月１.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月１.Text & txt特別再振替日１.Text
                        Else
                            SAIFURI_DATE = txt対象年度.Text & txt特別再振替月１.Text & txt特別再振替日１.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("再振日には初振日以降の振替日を設定してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別再振替月１.Focus()
                        Return False
                    End If
                End If
            End If

            '特別請求日２
            If txt特別請求月２.Text.Trim <> "" Then
                If txt特別振替月２.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別振替月２.Focus()
                    Return False
                Else
                    If txt特別振替日２.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "特別振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別振替日２.Focus()
                        Return False
                    End If
                End If
            Else
                If txt特別振替日２.Text.Trim <> "" OrElse txt特別振替月２.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別請求月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別請求月２.Focus()
                    Return False
                End If
            End If
            If txt特別再振替月２.Text.Trim = "" Then
                If txt特別再振替日２.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替月２.Focus()
                    Return False
                End If
            Else
                If txt特別再振替日２.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替日２.Focus()
                    Return False
                Else
                    '振替日、再振日相関チェック
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '初振日設定
                    If CInt(txt特別振替月２.Text) >= 1 AndAlso CInt(txt特別振替月２.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別振替月２.Text & txt特別振替日２.Text
                    Else
                        FURI_DATE = txt対象年度.Text & txt特別振替月２.Text & txt特別振替日２.Text
                    End If
                    '再振日設定
                    If CInt(txt特別再振替月２.Text) >= 1 AndAlso CInt(txt特別再振替月２.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月２.Text & txt特別再振替日２.Text
                    Else
                        If txt特別振替月２.Text = "03" AndAlso txt特別再振替月２.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月２.Text & txt特別再振替日２.Text
                        Else
                            SAIFURI_DATE = txt対象年度.Text & txt特別再振替月２.Text & txt特別再振替日２.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("再振日には初振日以降の振替日を設定してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別再振替月２.Focus()
                        Return False
                    End If
                End If
            End If

            '特別請求日３
            If txt特別請求月３.Text.Trim <> "" Then
                If txt特別振替月３.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別振替月３.Focus()
                    Return False
                Else
                    If txt特別振替日３.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "特別振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別振替日３.Focus()
                        Return False
                    End If
                End If
            Else
                If txt特別振替日３.Text.Trim <> "" OrElse txt特別振替月３.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別請求月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別請求月３.Focus()
                    Return False
                End If
            End If
            If txt特別再振替月３.Text.Trim = "" Then
                If txt特別再振替日３.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替月３.Focus()
                    Return False
                End If
            Else
                If txt特別再振替日３.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替日３.Focus()
                    Return False
                Else
                    '振替日、再振日相関チェック
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '初振日設定
                    If CInt(txt特別振替月３.Text) >= 1 AndAlso CInt(txt特別振替月３.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別振替月３.Text & txt特別振替日３.Text
                    Else
                        FURI_DATE = txt対象年度.Text & txt特別振替月３.Text & txt特別振替日３.Text
                    End If
                    '再振日設定
                    If CInt(txt特別再振替月３.Text) >= 1 AndAlso CInt(txt特別再振替月３.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月３.Text & txt特別再振替日３.Text
                    Else
                        If txt特別振替月３.Text = "03" AndAlso txt特別再振替月３.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月３.Text & txt特別再振替日３.Text
                        Else
                            SAIFURI_DATE = txt対象年度.Text & txt特別再振替月３.Text & txt特別再振替日３.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("再振日には初振日以降の振替日を設定してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別再振替月３.Focus()
                        Return False
                    End If
                End If
            End If

            '特別請求日４
            If txt特別請求月４.Text.Trim <> "" Then
                If txt特別振替月４.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別振替月４.Focus()
                    Return False
                Else
                    If txt特別振替日４.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "特別振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別振替日４.Focus()
                        Return False
                    End If
                End If
            Else
                If txt特別振替日４.Text.Trim <> "" OrElse txt特別振替月４.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別請求月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別請求月４.Focus()
                    Return False
                End If
            End If
            If txt特別再振替月４.Text.Trim = "" Then
                If txt特別再振替日４.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替月４.Focus()
                    Return False
                End If
            Else
                If txt特別再振替日４.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替日４.Focus()
                    Return False
                Else
                    '振替日、再振日相関チェック
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '初振日設定
                    If CInt(txt特別振替月４.Text) >= 1 AndAlso CInt(txt特別振替月４.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別振替月４.Text & txt特別振替日４.Text
                    Else
                        FURI_DATE = txt対象年度.Text & txt特別振替月４.Text & txt特別振替日４.Text
                    End If
                    '再振日設定
                    If CInt(txt特別再振替月４.Text) >= 1 AndAlso CInt(txt特別再振替月４.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月４.Text & txt特別再振替日４.Text
                    Else
                        If txt特別振替月４.Text = "03" AndAlso txt特別再振替月４.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月４.Text & txt特別再振替日４.Text
                        Else
                            SAIFURI_DATE = txt対象年度.Text & txt特別再振替月４.Text & txt特別再振替日４.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("再振日には初振日以降の振替日を設定してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別再振替月４.Focus()
                        Return False
                    End If
                End If
            End If

            '特別請求日５
            If txt特別請求月５.Text.Trim <> "" Then
                If txt特別振替月５.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別振替月５.Focus()
                    Return False
                Else
                    If txt特別振替日５.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "特別振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別振替日５.Focus()
                        Return False
                    End If
                End If
            Else
                If txt特別振替日５.Text.Trim <> "" OrElse txt特別振替月５.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別請求月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別請求月５.Focus()
                    Return False
                End If
            End If
            If txt特別再振替月５.Text.Trim = "" Then
                If txt特別再振替日５.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替月５.Focus()
                    Return False
                End If
            Else
                If txt特別再振替日５.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替日５.Focus()
                    Return False
                Else
                    '振替日、再振日相関チェック
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '初振日設定
                    If CInt(txt特別振替月５.Text) >= 1 AndAlso CInt(txt特別振替月５.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別振替月５.Text & txt特別振替日５.Text
                    Else
                        FURI_DATE = txt対象年度.Text & txt特別振替月５.Text & txt特別振替日５.Text
                    End If
                    '再振日設定
                    If CInt(txt特別再振替月５.Text) >= 1 AndAlso CInt(txt特別再振替月５.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月５.Text & txt特別再振替日５.Text
                    Else
                        If txt特別振替月５.Text = "03" AndAlso txt特別再振替月５.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月５.Text & txt特別再振替日５.Text
                        Else
                            SAIFURI_DATE = txt対象年度.Text & txt特別再振替月５.Text & txt特別再振替日５.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("再振日には初振日以降の振替日を設定してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別再振替月５.Focus()
                        Return False
                    End If
                End If
            End If

            '特別請求日６
            If txt特別請求月６.Text.Trim <> "" Then
                If txt特別振替月６.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別振替月６.Focus()
                    Return False
                Else
                    If txt特別振替日６.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "特別振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別振替日６.Focus()
                        Return False
                    End If
                End If
            Else
                If txt特別振替日６.Text.Trim <> "" OrElse txt特別振替月６.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別請求月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別請求月６.Focus()
                    Return False
                End If
            End If
            If txt特別再振替月６.Text.Trim = "" Then
                If txt特別再振替日６.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替月"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替月６.Focus()
                    Return False
                End If
            Else
                If txt特別再振替日６.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "特別再振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt特別再振替日６.Focus()
                    Return False
                Else
                    '振替日、再振日相関チェック
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '初振日設定
                    If CInt(txt特別振替月６.Text) >= 1 AndAlso CInt(txt特別振替月６.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別振替月６.Text & txt特別振替日６.Text
                    Else
                        FURI_DATE = txt対象年度.Text & txt特別振替月６.Text & txt特別振替日６.Text
                    End If
                    '再振日設定
                    If CInt(txt特別再振替月６.Text) >= 1 AndAlso CInt(txt特別再振替月６.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月６.Text & txt特別再振替日６.Text
                    Else
                        If txt特別振替月６.Text = "03" AndAlso txt特別再振替月６.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt対象年度.Text) + 1) & txt特別再振替月６.Text & txt特別再振替日６.Text
                        Else
                            SAIFURI_DATE = txt対象年度.Text & txt特別再振替月６.Text & txt特別再振替日６.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("再振日には初振日以降の振替日を設定してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt特別再振替月６.Focus()
                        Return False
                    End If
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try

        PFUNC_Nyuryoku_Check = True

    End Function
    '2011/06/16 標準版修正 特別振替日相関チェック追加 ------------------END
#End Region

End Class
