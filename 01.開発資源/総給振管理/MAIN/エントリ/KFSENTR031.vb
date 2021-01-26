Option Strict Off '旧VB互換用
Option Explicit On

Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFSENTR031
    Private CAST As New CASTCommon.Events
    Private CASTxx As New CASTCommon.Events("0-9a-zA-Z._-", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFSENTR031", "依頼書用金額入力画面")
    Private Const msgTitle As String = "依頼書用金額入力画面(KFSENTR031)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure

    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース
    '変数
    Private strKIN_NNM As String
    Private strSIT_NNM As String
    Private strKIN_KNM As String
    Private strSIT_KNM As String
    Private intRET As Integer
    Private strBAITAI_CD As String
    Private strKIGYO_CD As String
    Private strFURI_CD As String
    Private strIraisyo_Sort As String  '依頼書出力順

    Private lngMAX_Page As Long   '頁MAX
    Private lngMAX_Gyo As Long    '行MAX（最終頁）
    Private lngMAX_Record As Long 'ﾚｺｰﾄﾞMAX
    Private lngSYOUKEI As Long    '対象頁行数(小計件数)
    Private lngSYOUKEI2 As Long  '解約数または振込金額0円のものを引いた小計件数
    Private lngKAIYAKU_CNT As Long '全解約数
    Private lngPage_CNT As Long
    Private lngGyo_CNT As Long
    Private lngSYOUKEI_KEN As Long
    '画面表示項目用配列
    Public strTKIN_NO(700, 14) As String
    Public strTSIT_NO(700, 14) As String
    Public strTKIN_NAME(700, 14) As String
    Public strTSIT_NAME(700, 14) As String
    Public lngKOUBAN(700, 14) As Long
    Public strKAMOKU(700, 14) As String
    Public strKOUZA(700, 14) As String
    Public strKEIYAKU_NNAME(700, 14) As String
    Public strKEIYAKU_KNAME(700, 14) As String
    Public strKEIYAKU_NO(700, 14) As String
    Public lngFURIKIN(700, 14) As Long
    Public lngTESUU(700, 14) As Long
    Public strFURIDATE(700, 14) As String
    Public strKAIYAKU(700, 14) As String
    Public strKINGAKU_KBN(700, 14) As String
    Public lngHYOUJI_SEQ(700, 14) As Long  'update時のキー用
    Public strCHK_KEKKA(700, 14) As String '口座チェックの結果格納用
    Private strMAST As String
    Private strINPUT_FLG(700) As String
    Private strINPUT_K_SYOUKEI(700) As String
    Private strINPUT_T_SYOUKEI(700) As String
    'コントロール配列
    Private lblKOUBAN(14) As Label
    Private lblTKIN_NAME(14) As Label
    Private lblTSIT_NAME(14) As Label
    Private lblKAMOKU(14) As Label
    Private lblKOUZA(14) As Label
    Private lblKEIYAKU_NNAME(14) As Label
    Private lblKEIYAKU_NO(14) As Label
    Private txtFURIKIN(14) As TextBox
    Private lblTESUU(14) As Label

    '2018/02/09 saitou 広島信金(RSV2標準) ADD エントリ20明細対応 -------------------- START
    'M_SOUFURIのグローバル変数宣言をやめ、画面毎にグローバル変数を持つように変更。
    '本画面で使用しているM_SOUFURIのグローバル変数を一括置換する。
    Public strTORIS_CODE As String
    Public strTORIF_CODE As String
    Public strFURI_DATE As String
    Public strSYOKITI As String
    Public intCHK As Integer
    Public lngPAGE As Long
    '2018/02/09 saitou 広島信金(RSV2標準) ADD --------------------------------------- END
    'ベリファイ要否区分
    Public BERYFAI As String = 0

    'ファイル格納フォルダ
    Private EtcFolder As String
    Private DatFolder As String

    Private gstrCHK_KOUZA As String
    Private gstrJIKINKO As String
    Private gstrCHK_DEJIT As String
    Public Enum gintKEKKA As Integer
        OK = 0
        NG = 1
        OTHER = 2
    End Enum

    ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Friend Structure INI_PARAM
        Dim RSV2_EDITION As String                          ' RSV2機能設定
    End Structure
    Private Ini_Info As INI_PARAM

    Private TimeStamp As String
    ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END


#Region " ロード"
    Private Sub KFSENTR031_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = strTORIS_CODE & "-" & strTORIF_CODE
            LW.FuriDate = strFURI_DATE

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle
            Call GCom.SetMonitorTopArea(Label20, Label19, lblUser, lblDate)

            '取引先主コード表示
            lblTorisCode.Text = strTORIS_CODE
            '取引先副コード表示
            lblTorifCode.Text = strTORIF_CODE
            '振込日表示
            lblFuriDate.Text = strFURI_DATE.Substring(0, 4) & "/" & strFURI_DATE.Substring(4, 2) & "/" & strFURI_DATE.Substring(6, 2)
            '初期値表示
            lblSyokiti.Text = strSYOKITI

            'コントロール配列セット
            If fn_set_Control() = False Then
                MessageBox.Show(MSG0239W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            'iniファイル読み込み
            If fn_set_INIFILE() = False Then
                Exit Sub
            End If
            '口座チェックボタン設定
            If gstrCHK_KOUZA.ToUpper = "YES" Then
                btnKOUZACHK.Enabled = True
            Else
                btnKOUZACHK.Enabled = False
            End If

            '取引先マスタ情報取得
            If fn_select_TORIMAST() = False Then
                Exit Sub
            Else
                '媒体コードチェック
                If strBAITAI_CD <> "04" Then '依頼書
                    MessageBox.Show(MSG0108W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            End If

            'スケジュールマスタチェック
            If fn_select_SCHMAST() = False Then
                Exit Sub
            End If

            'エントリマスタチェック
            If intCHK = 0 Then '正にチェックの場合
                strMAST = "S_ENTMAST"
            Else
                strMAST = "S_FUKU_ENTMAST"
            End If
            If fn_select_ENTMAST() = False Then
                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If fn_HYOJI_SUB(lngPAGE) = False Then
                Exit Sub
            End If

            ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
            TimeStamp = ""
            ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region " 登録"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle

            lngSYOUKEI2 = 0 '解約数または振込金額0円のものを引いた小計件数

            '画面保存
            If fn_GAMEN_HOZON(lngPAGE) = False Then
                MessageBox.Show(MSG0241W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If


            '小計・合計ﾁｪｯｸ
            If fn_CHECK_SYOUKEI() = False Then
                Exit Sub
            ElseIf fn_CHECK_GOUKEI() = False Then
                Exit Sub
            End If

            lngPage_CNT = 0
            lngGyo_CNT = 0

            lngPage_CNT = lngPAGE - 1

            '副の場合、正のマスタと合計件数･金額の一致チェックを実行
            If intCHK = 1 Then
                Dim Count As Long
                Dim Kingaku As Long
                If fn_CLC_ENTMAST(Count, Kingaku) = False Then
                    '合計件数取得失敗
                    MessageBox.Show(MSG0245W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
                If GCom.NzLong(fn_DEL_KANMA(lblGoukeiKen.Text)) <> Count Then
                    MessageBox.Show(String.Format(MSG0242W, Count.ToString("#,##0"), lblGoukeiKen.Text), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
                If GCom.NzLong(fn_KEISAN_K_GOUKEI(lngMAX_Page)) <> Kingaku Then
                    '合計金額不一致
                    MessageBox.Show(String.Format(MSG0243W, Kingaku.ToString("#,##0"), (GCom.NzLong(fn_KEISAN_K_GOUKEI(lngMAX_Page))).ToString("#,##0")), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            End If

            '依頼書・スケジュールマスタ更新
            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If

            lngPage_CNT = 0
            lngGyo_CNT = 0

            lngPage_CNT = lngPAGE - 1
            'エントリマスタ更新サブルーチン
            MainDB.BeginTrans()
            intRET = fn_ENTMAST_WRITE(lngPage_CNT)

            If intRET = gintKEKKA.NG Then
                MainDB.Rollback()
                Exit Sub
            End If
            Dim UKETUKE As String = "0"
            'スケジュールマスタ更新
            If fn_update_SCHMAST(UKETUKE) = False Then
                MainDB.Rollback()
                Exit Sub
            Else
                '更新(登録)成功
                'MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainDB.Commit()
            End If
            'ベリファイ無しの場合・ベリファイありかつ副入力完了の場合データファイルを作成する
            Select Case UKETUKE
                Case "0", "3"
                    MessageBox.Show(MSG0056I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case "2"
                    MessageBox.Show(MSG0057I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case "1"
                    MessageBox.Show(MSG0058I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    If MakeRecord() = True Then
                        MessageBox.Show(MSG0059I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show(String.Format(MSG0027E, "依頼ファイル", "作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
            End Select


            btnEnd.Focus()     'ﾌｫｰｶｽ移動

            lngSYOUKEI2 = lngSYOUKEI

            For lngGyo_CNT = 0 To lngSYOUKEI - 1
                If txtFURIKIN(lngGyo_CNT).Text.Length <> 0 Then
                    If txtFURIKIN(lngGyo_CNT).Text = "0" Then
                        lngSYOUKEI2 = lngSYOUKEI2 - 1
                    Else
                        If strKAIYAKU(lngPage_CNT, lngGyo_CNT) = "1" Then   '引落金額:解約済の場合
                            txtFURIKIN(lngGyo_CNT).Text = "0"
                            lngFURIKIN(lngPage_CNT, lngGyo_CNT) = fn_DEL_KANMA(txtFURIKIN(lngGyo_CNT).Text)
                            lngSYOUKEI2 = lngSYOUKEI2 - 1
                        End If
                    End If
                Else
                    lngSYOUKEI2 = lngSYOUKEI2 - 1
                End If
            Next

            lblSyoukeiKen.Text = Format(lngSYOUKEI2, "#,##0")                            '共通設定
        Catch ex As Exception
            If MainDB IsNot Nothing Then
                MainDB.Rollback()
            End If
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " 保存"
    Private Sub btnTouroku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTouroku.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(保存)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle
            lngSYOUKEI2 = 0 '解約数または振込金額0円のものを引いた小計件数

            '画面保存 
            If fn_GAMEN_HOZON(lngPAGE) = False Then
                MessageBox.Show(MSG0241W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            lngPage_CNT = 0
            lngGyo_CNT = 0

            lngPage_CNT = lngPAGE - 1

            ''小計ﾁｪｯｸ
            If fn_CHECK_SYOUKEI() = False Then
                Exit Sub
            End If

            'エントリマスタ更新
            If MessageBox.Show(String.Format(MSG0015I, "保存"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If

            '----------------------------
            'エントリマスタ更新サブルーチン
            '----------------------------
            MainDB.BeginTrans()
            intRET = fn_ENTMAST_WRITE(lngPage_CNT)
            If intRET = gintKEKKA.NG Then
                MainDB.Rollback()
                Exit Sub
            ElseIf intRET = gintKEKKA.OK Then
                MainDB.Commit()
                MessageBox.Show(String.Format(MSG0016I, "保存"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            If btnNextGamen.Enabled = False Then
                btnAction.Focus()
            Else
                btnNextGamen.Focus()     'ﾌｫｰｶｽ移動
            End If

            lngSYOUKEI2 = lngSYOUKEI

            For lngGyo_CNT = 0 To lngSYOUKEI - 1
                If txtFURIKIN(lngGyo_CNT).Text.Length <> 0 Then
                    If txtFURIKIN(lngGyo_CNT).Text = "0" Then
                        lngSYOUKEI2 = lngSYOUKEI2 - 1
                    Else
                        If strKAIYAKU(lngPage_CNT, lngGyo_CNT) = "1" Then   '引落金額:解約済の場合
                            txtFURIKIN(lngGyo_CNT).Text = "0"
                            lngFURIKIN(lngPage_CNT, lngGyo_CNT) = fn_DEL_KANMA(txtFURIKIN(lngGyo_CNT).Text)
                            lngSYOUKEI2 = lngSYOUKEI2 - 1
                        End If
                    End If
                Else
                    lngSYOUKEI2 = lngSYOUKEI2 - 1
                End If
            Next

            lblSyoukeiKen.Text = lngSYOUKEI2                            '共通設定
        Catch ex As Exception
            If MainDB IsNot Nothing Then
                MainDB.Rollback()
            End If
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(保存)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(保存)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region " 前画面"
    Private Sub btnPreGamen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPreGamen.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(前画面)開始", "成功", "")
            strINPUT_FLG(lngPAGE) = 1
            If txtSyoukeiKin.Text.Trim = "" Then
                strINPUT_K_SYOUKEI(lngPAGE) = 0   '小計金額保存(未入力)
            Else
                strINPUT_K_SYOUKEI(lngPAGE) = fn_DEL_KANMA(txtSyoukeiKin.Text.Trim)   '小計金額保存(入力)
            End If

            strINPUT_T_SYOUKEI(lngPAGE) = lblSyoukeiTesuu.Text.Trim   '小計手数料保存

            '画面保存 
            If fn_GAMEN_HOZON(lngPAGE) = False Then
                MessageBox.Show(MSG0241W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            lngPAGE = lngPAGE - 1
            'ページ数表示
            lblPage.Text = lngPAGE.ToString.PadLeft(2, "0"c) & "/" & lngMAX_Page.ToString.PadLeft(2, "0"c)

            If fn_HYOJI_SUB(lngPAGE) = False Then
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(前画面)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(前画面)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " 次画面"
    Private Sub btnNextGamen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNextGamen.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(次画面)開始", "成功", "")
            strINPUT_FLG(lngPAGE) = 1
            If txtSyoukeiKin.Text.Trim = "" Then
                strINPUT_K_SYOUKEI(lngPAGE) = 0   '小計金額保存(未入力)
            Else
                strINPUT_K_SYOUKEI(lngPAGE) = fn_DEL_KANMA(txtSyoukeiKin.Text.Trim)   '小計金額保存(入力)
            End If
            strINPUT_T_SYOUKEI(lngPAGE) = lblSyoukeiTesuu.Text.Trim   '小計手数料保存

            '画面保存 
            If fn_GAMEN_HOZON(lngPAGE) = False Then
                MessageBox.Show(MSG0241W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            lngPAGE = lngPAGE + 1
            'ページ数表示
            lblPage.Text = lngPAGE.ToString.PadLeft(2, "0"c) & "/" & lngMAX_Page.ToString.PadLeft(2, "0"c)

            If fn_HYOJI_SUB(lngPAGE) = False Then
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(次画面)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(次画面)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " 終了"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region " 口座チェック"
    Private Sub btnKOUZACHK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKOUZACHK.Click
        Dim lngPAGE_LOOP As Long
        Dim lngGYO_LOOP As Long
        Dim strKOKYAKU_KNM As String = ""
        Dim strERR_MSG As String = ""
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座チェック)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle

            '2010/02/01 追加 ======================
            '画面保存 
            If fn_GAMEN_HOZON(lngPAGE) = False Then
                MessageBox.Show(MSG0241W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '======================================
            For lngPAGE_LOOP = LBound(strCHK_KEKKA, 1) To UBound(strCHK_KEKKA, 1)
                If strTSIT_NO(lngPAGE_LOOP, lngGYO_LOOP) = "" Then
                    Exit For
                End If
                For lngGYO_LOOP = LBound(strCHK_KEKKA, 2) To UBound(strCHK_KEKKA, 2)
                    If strTSIT_NO(lngPAGE_LOOP, lngGYO_LOOP) = "" Then
                        Exit For
                    End If

                    If gstrJIKINKO = strTKIN_NO(lngPAGE_LOOP, lngGYO_LOOP) Then   '自金庫のみ口座チェック
                        '2011/06/28 標準版修正 総振は自振契約なしは正常扱い ------------------START
                        Dim Ret As Integer = GCom.KouzaChk_ENTRY(strTSIT_NO(lngPAGE_LOOP, lngGYO_LOOP), strKAMOKU(lngPAGE_LOOP, lngGYO_LOOP), _
                                             strKOUZA(lngPAGE_LOOP, lngGYO_LOOP), strKIGYO_CD, strFURI_CD, _
                                             strKOKYAKU_KNM, strERR_MSG, MainDB)
                        If Ret = -1 Then
                            strERR_MSG = "チェック失敗"
                            Exit Sub
                        Else
                            If Ret = 2 Then
                                strERR_MSG = ""
                            End If
                            strCHK_KEKKA(lngPAGE_LOOP, lngGYO_LOOP) = strERR_MSG
                        End If
                        'If GCom.KouzaChk_ENTRY(strTSIT_NO(lngPAGE_LOOP, lngGYO_LOOP), strKAMOKU(lngPAGE_LOOP, lngGYO_LOOP), _
                        '                       strKOUZA(lngPAGE_LOOP, lngGYO_LOOP), strKIGYO_CD, strFURI_CD, _
                        '                       strKOKYAKU_KNM, strERR_MSG, MainDB) = -1 Then
                        '    Exit Sub
                        'Else
                        '    strCHK_KEKKA(lngPAGE_LOOP, lngGYO_LOOP) = strERR_MSG
                        'End If
                        '2011/06/28 標準版修正 総振は自振契約なしは正常扱い ------------------END
                    End If
                Next
                lngGYO_LOOP = 0 '行カウント初期化
            Next

            '現在表示している画面分のみ結果表示 2007/08/24
            For lngGYO_LOOP = LBound(strCHK_KEKKA, 2) To UBound(strCHK_KEKKA, 2)
                If strCHK_KEKKA(lngPAGE - 1, lngGYO_LOOP) Is Nothing Then
                    Exit For
                End If

                If strKEIYAKU_NNAME(lngPAGE - 1, lngGYO_LOOP) <> "<<解約済>>" Then
                    lblTESUU(lngGYO_LOOP).Text = strCHK_KEKKA(lngPAGE - 1, lngGYO_LOOP)
                End If
            Next
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座チェック)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座チェック)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region " 関数"

    Function fn_select_TORIMAST() As Boolean
        '============================================================================
        'NAME           :fn_select_TORIMAST
        'Parameter      :
        'Description    :S_TORIMASTから取引先情報取得
        'Return         :True=OK,False=NG
        'Create         :2004/08/28
        'Update         :
        '============================================================================
        fn_select_TORIMAST = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            SQL.Append(" SELECT * FROM S_TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
            SQL.Append("  AND TORIF_CODE_T = " & SQ(strTORIF_CODE))

            If OraReader.DataReader(SQL) = True Then
                lblToriNName.Text = GCom.NzStr(OraReader.Reader.Item("ITAKU_NNAME_T"))
                strKIGYO_CD = GCom.NzStr(OraReader.Reader.Item("KIGYO_CODE_T"))
                strFURI_CD = GCom.NzStr(OraReader.Reader.Item("FURI_CODE_T"))
                strBAITAI_CD = GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T"))
                'strJIKINKO_NO = GCom.NzStr(OraReader.Reader.Item("TKIN_NO_T"))
                strIraisyo_Sort = GCom.NzStr(OraReader.Reader.Item("IRAISYO_SORT_T"))
                OraReader.Close()
            Else
                '取引先なし
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                OraReader.Close()
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

        fn_select_TORIMAST = True
    End Function

    Function fn_select_SCHMAST() As Boolean
        '============================================================================
        'NAME           :fn_select_SCHMAST
        'Parameter      :
        'Description    :SCHMASTから取引先情報取得
        'Return         :True=OK,False=NG
        'Create         :2004/08/28
        'Update         :
        '============================================================================
        Dim strTOUROKU_FLG As String
        Dim lngSYORI_KEN As Long, lngSYORI_KIN As Long
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        fn_select_SCHMAST = False

        Try
            SQL.Append(" SELECT * FROM S_SCHMAST")
            SQL.Append("  WHERE TORIS_CODE_S = " & SQ(strTORIS_CODE))
            SQL.Append("  AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            SQL.Append("  AND FURI_DATE_S = " & SQ(strFURI_DATE))

            If OraReader.DataReader(SQL) = True Then
                strTOUROKU_FLG = GCom.NzStr(OraReader.Reader.Item("TOUROKU_FLG_S"))
                lngSYORI_KEN = GCom.NzLong(OraReader.Reader.Item("SYORI_KEN_S"))
                lngSYORI_KIN = GCom.NzLong(OraReader.Reader.Item("SYORI_KIN_S"))
                OraReader.Close()
            Else
                'スケジュールなし
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                OraReader.Close()
                Return False
            End If

            If strTOUROKU_FLG = "1" Then
                '登録済み
                MessageBox.Show(String.Format(MSG0240W, Format(lngSYORI_KEN, "#,##0"), Format(lngSYORI_KIN, "#,##0")), _
                                               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュールマスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_select_SCHMAST = True

    End Function

    Function fn_select_ENTMAST() As Boolean
        '============================================================================
        'NAME           :fn_select_ENTMAST
        'Parameter      :
        'Description    :S_ENTMASTから情報抽出
        'Return         :True=OK,False=NG
        'Create         :2004/08/28
        'Update         :2007/08/06 蒲郡信金向け　表示順は需要家番号(文字を含む)とする
        '============================================================================
        fn_select_ENTMAST = False
        Dim strKIN_CODE As String
        Dim strSIT_CODE As String
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)


        Try
            'SQL.Append("SELECT * FROM " & strMAST)
            'SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            'SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            'SQL.Append(" AND KAISI_DATE_E <= " & SQ(strFURI_DATE)) '開始年月日を比較
            'Select Case strIraisyo_Sort
            '    '2010/02/18 新規コードをソート条件に加える
            '    Case "0"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,KEIYAKU_NO_E ASC")
            '    Case "1"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,KEIYAKU_KNAME_E,KEIYAKU_NO_E ASC")
            '    Case "2"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,JYUYOUKA_NO_E,KEIYAKU_NO_E ASC")
            '        '=================================================
            'End Select
            SQL.Append("SELECT ")
            SQL.Append(" TSIT_NO_E,TKIN_NO_E,HYOUJI_SEQ_E,KAMOKU_E,KOUZA_E,JYUYOUKA_NO_E")
            SQL.Append(",KEIYAKU_NO_E, KINGAKU_KBN_E,KINGAKU_KBN_E,FURI_DATE_E,KAIYAKU_E")
            SQL.Append(",KEIYAKU_NNAME_E,KEIYAKU_KNAME_E,FURIKIN_E,ERR_MSG_E,SINKI_CODE_E")
            SQL.Append(",'0000000000' KOKYAKU_NO_E")
            SQL.Append(" FROM " & strMAST)
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            SQL.Append(" AND SINKI_CODE_E = '0'")
            SQL.Append(" AND KAISI_DATE_E <= " & SQ(strFURI_DATE)) '開始年月日を比較
            SQL.Append(" UNION SELECT ")
            SQL.Append(" TSIT_NO_E,TKIN_NO_E,HYOUJI_SEQ_E,KAMOKU_E,KOUZA_E,JYUYOUKA_NO_E")
            SQL.Append(",KEIYAKU_NO_E, KINGAKU_KBN_E,KINGAKU_KBN_E,FURI_DATE_E,KAIYAKU_E")
            SQL.Append(",KEIYAKU_NNAME_E,KEIYAKU_KNAME_E,FURIKIN_E,ERR_MSG_E,SINKI_CODE_E")
            SQL.Append(",KOKYAKU_NO_E ")
            SQL.Append(" FROM " & strMAST)
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            SQL.Append(" AND SINKI_CODE_E <> '0'")
            SQL.Append(" AND KAISI_DATE_E <= " & SQ(strFURI_DATE)) '開始年月日を比較
            '2011/06/24 標準版修正 エントリ処理に新規コードは考慮しない ------------------START
            Select Case strIraisyo_Sort
                Case "0"
                    SQL.Append(" ORDER BY KEIYAKU_NO_E ASC")
                Case "1"
                    SQL.Append(" ORDER BY KEIYAKU_KNAME_E , KEIYAKU_NO_E ASC")
                Case "2"
                    SQL.Append(" ORDER BY JYUYOUKA_NO_E , KEIYAKU_NO_E ASC")
                    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- START
                Case "3"
                    SQL.Append(" ORDER BY TKIN_NO_E , TSIT_NO_E , KAMOKU_E , KOUZA_E ASC")
                    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- END
            End Select
            'Select Case strIraisyo_Sort
            '    Case "0"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,KOKYAKU_NO_E,KEIYAKU_NO_E ASC")
            '    Case "1"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,KOKYAKU_NO_E,KEIYAKU_KNAME_E,KEIYAKU_NO_E ASC")
            '    Case "2"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,KOKYAKU_NO_E,JYUYOUKA_NO_E,KEIYAKU_NO_E ASC")
            'End Select
            '2011/06/24 標準版修正 エントリ処理に新規コードは考慮しない ------------------END
            '=======================================================
            lngPage_CNT = 0
            lngGyo_CNT = 0
            lngKAIYAKU_CNT = 0

            If OraReader.DataReader(SQL) = True Then
                Do
                    lngMAX_Record = lngMAX_Record + 1
                    If lngGyo_CNT >= 15 Then '15行超えたら次ページ
                        lngGyo_CNT = 0
                        lngPage_CNT = lngPage_CNT + 1
                    End If
                    strKIN_CODE = GCom.NzStr(OraReader.GetString("TKIN_NO_E"))
                    strSIT_CODE = GCom.NzStr(OraReader.GetString("TSIT_NO_E"))

                    '金融機関名取得
                    If fn_Select_TENMAST(strKIN_CODE, strSIT_CODE, strKIN_NNM, strSIT_NNM, strKIN_KNM, strSIT_KNM) = False Then
                        MessageBox.Show(String.Format(MSG0248W, strKIN_CODE, strSIT_CODE), _
                                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If

                    lngHYOUJI_SEQ(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("HYOUJI_SEQ_E")) '表示SEQ
                    strTKIN_NO(lngPage_CNT, lngGyo_CNT) = strKIN_CODE '金庫コード
                    strTSIT_NO(lngPage_CNT, lngGyo_CNT) = strSIT_CODE '支店コード
                    strTKIN_NAME(lngPage_CNT, lngGyo_CNT) = strKIN_NNM  '金融機関名
                    strTSIT_NAME(lngPage_CNT, lngGyo_CNT) = strSIT_NNM  '金融機関名
                    lngKOUBAN(lngPage_CNT, lngGyo_CNT) = lngPage_CNT * 15 + lngGyo_CNT + 1 '項番取得
                    strKAMOKU(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("KAMOKU_E")) '科目
                    strKOUZA(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("KOUZA_E")) '口座
                    strKEIYAKU_NO(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("KEIYAKU_NO_E")) '契約番号
                    strKINGAKU_KBN(lngPage_CNT, lngGyo_CNT) = GCom.NzLong(OraReader.GetString("KINGAKU_KBN_E")) '金額区分
                    strKAIYAKU(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("KAIYAKU_E")) '解約フラグ
                    strFURIDATE(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("FURI_DATE_E"))
                    If strKAIYAKU(lngPage_CNT, lngGyo_CNT) <> "1" Then       '解約済判定
                        ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                        '前回金額非表示の考慮追加
                        If strFURIDATE(lngPage_CNT, lngGyo_CNT) = strFURI_DATE Then
                            'マスタの振込日と入力振込日が一緒
                            '１回以上保存しているため、前回金額を表示する
                            strKEIYAKU_NNAME(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("KEIYAKU_NNAME_E")).Trim '契約者名ｾｯﾄ
                            strKEIYAKU_KNAME(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("KEIYAKU_KNAME_E")).Trim  '契約者名カナｾｯﾄ
                            lngFURIKIN(lngPage_CNT, lngGyo_CNT) = GCom.NzLong(OraReader.GetString("FURIKIN_E"))            '前回金額ｾｯﾄ
                            strCHK_KEKKA(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("ERR_MSG_E")) '口座チェック結果
                        Else
                            'マスタの振込日と入力振込日が異なる
                            '初回入力なので、金額表示区分に依存する
                            strKEIYAKU_NNAME(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("KEIYAKU_NNAME_E")).Trim '契約者名ｾｯﾄ
                            strKEIYAKU_KNAME(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("KEIYAKU_KNAME_E")).Trim  '契約者名カナｾｯﾄ
                            Select Case strKINGAKU_KBN(lngPage_CNT, lngGyo_CNT)
                                Case "0"        '前回金額表示
                                    lngFURIKIN(lngPage_CNT, lngGyo_CNT) = GCom.NzLong(OraReader.GetString("FURIKIN_E"))            '前回金額ｾｯﾄ
                                Case "1"        '前回金額非表示
                                    lngFURIKIN(lngPage_CNT, lngGyo_CNT) = 0                                     '0ｾｯﾄ
                            End Select
                            strCHK_KEKKA(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("ERR_MSG_E")) '口座チェック結果
                        End If

                        'strKEIYAKU_NNAME(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("KEIYAKU_NNAME_E")).Trim '契約者名ｾｯﾄ
                        'strKEIYAKU_KNAME(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("KEIYAKU_KNAME_E")).Trim  '契約者名カナｾｯﾄ
                        'lngFURIKIN(lngPage_CNT, lngGyo_CNT) = GCom.NzLong(OraReader.GetString("FURIKIN_E"))            '前回金額ｾｯﾄ
                        'strCHK_KEKKA(lngPage_CNT, lngGyo_CNT) = GCom.NzStr(OraReader.GetString("ERR_MSG_E")) '口座チェック結果
                        ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                    Else
                        strKEIYAKU_NNAME(lngPage_CNT, lngGyo_CNT) = "<<解約済>>"                  '解約済ｾｯﾄ
                        lngFURIKIN(lngPage_CNT, lngGyo_CNT) = 0                                     '0ｾｯﾄ
                        lngKAIYAKU_CNT = lngKAIYAKU_CNT + 1                     '解約数ｶｳﾝﾄ
                    End If

                    lngGyo_CNT = lngGyo_CNT + 1
                Loop Until OraReader.NextRead = False
            End If


            lngMAX_Page = lngPage_CNT + 1
            lngMAX_Gyo = lngGyo_CNT

            If Int((lngMAX_Record - 1)) / 15 + 1 < lngPAGE Then        '開始頁ﾁｪｯｸ
                lngPAGE = Int((lngMAX_Record - 1)) / 15 + 1
            End If

            'ページ数表示
            lblPage.Text = lngPAGE.ToString.PadLeft(2, "0"c) & "/" & lngMAX_Page.ToString.PadLeft(2, "0"c)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(依頼書マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_select_ENTMAST = True
    End Function

    Function fn_Select_TENMAST(ByVal KIN_NO As String, ByVal SIT_NO As String, ByRef KIN_NNAME As String, ByRef SIT_NNAME As String, ByRef KIN_KNAME As String, ByRef SIT_KNAME As String) As Boolean
        '=====================================================================================
        'NAME           :fn_Select_TENMAST
        'Parameter      :KIN_NO：金融機関コード／SIT_NO：支店コード／KIN_NNAME:金融機関漢字名
        '               :SIT_NNAME:支店漢字名／KIN_KNAME：金融機関カナ名／SIT_KNAME：支店カナ名
        'Description    :金融機関マスタ検索
        'Return         :True=OK(検索ヒット),False=NG（検索失敗）
        'Create         :2009/09/29
        'Update         :
        '=====================================================================================
        fn_Select_TENMAST = False
        Dim SQL As StringBuilder
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
start:
        Try
            SQL = New StringBuilder(128)
            SQL.Append(" SELECT KIN_NNAME_N,KIN_KNAME_N,SIT_NNAME_N,SIT_KNAME_N FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = " & SQ(Trim(KIN_NO)))
            SQL.Append(" AND SIT_NO_N = " & SQ(Trim(SIT_NO)))
            If OraReader.DataReader(SQL) = True Then
                KIN_NNAME = GCom.NzStr(OraReader.GetString("KIN_NNAME_N"))
                SIT_NNAME = GCom.NzStr(OraReader.GetString("SIT_NNAME_N"))
                KIN_KNAME = GCom.NzStr(OraReader.GetString("KIN_KNAME_N"))
                SIT_KNAME = GCom.NzStr(OraReader.GetString("SIT_KNAME_N"))
                Return True
            End If
            KIN_NNAME = ""
            SIT_NNAME = ""
            KIN_KNAME = ""
            SIT_KNAME = ""

        Catch ex As Exception
            If Err.Number = 5 Then
                '-----------------------------------------
                '１秒停止
                '-----------------------------------------
                Dim Start As Double
                Start = CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss"))           ' 中断の開始時刻を設定します。
                Do While CDbl(Format(System.DateTime.Now, "yyyyMMddHHmmss")) < Start + 1
                    Application.DoEvents()
                Loop
                Err.Clear()
                GoTo start
            End If
            If Err.Number <> 0 And Err.Number <> 5 Then
                MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(金融機関マスタ検索)", "失敗", ex.ToString)
                Exit Function
            End If
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

    Function fn_HYOJI_SUB(ByVal aIN_PAGE As Long) As Boolean
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_HYOJI_SUB
        'Parameter  :aIN_PAGE:表示開始ページ
        'Description:口座振込データ表示処理
        'Return     :
        'Create     :2004/08/30
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        Dim I As Long
        Try
            fn_HYOJI_SUB = False
            '画面ｸﾘｱ
            For I = 0 To 14
                lblKOUBAN(I).Text = ""
                lblTKIN_NAME(I).Text = ""
                lblTSIT_NAME(I).Text = ""
                lblKAMOKU(I).Text = ""
                lblKOUZA(I).Text = ""
                lblKEIYAKU_NNAME(I).Text = ""
                lblKEIYAKU_NO(I).Text = ""
                txtFURIKIN(I).Text = ""
                txtFURIKIN(I).Enabled = False
                lblTESUU(I).Text = ""
            Next

            lblSyoukeiKen.Text = ""
            txtSyoukeiKin.Text = ""
            lblSyoukeiTesuu.Text = ""
            lblGoukeiKen.Text = ""
            txtGoukeiKin.Text = ""
            lblGoukeiTesuu.Text = ""

            lblGOUKEI.Enabled = False
            lblGKEN.Enabled = False
            txtGoukeiKin.Enabled = False
            btnPreGamen.Enabled = True
            btnNextGamen.Enabled = True
            btnAction.Enabled = False

            '小計ｾｯﾄ
            If aIN_PAGE <> lngMAX_Page Then
                lngSYOUKEI = 15
            Else                        '最終頁設定
                lngSYOUKEI = lngMAX_Gyo
            End If

            '画面表示
            lngSYOUKEI2 = 0 '解約数または振込金額0円のものを引いた小計件数

            '初期化
            lngPage_CNT = 0
            lngGyo_CNT = 0

            lngPage_CNT = aIN_PAGE - 1
            lngGyo_CNT = 0
            lngSYOUKEI2 = lngSYOUKEI

            For lngGyo_CNT = 0 To lngSYOUKEI - 1
                If strKEIYAKU_NNAME(lngPage_CNT, lngGyo_CNT) <> "<<解約済>>" Then
                    txtFURIKIN(lngGyo_CNT).Enabled = True
                End If
                lblKOUBAN(lngGyo_CNT).Text = lngKOUBAN(lngPage_CNT, lngGyo_CNT)
                lblTKIN_NAME(lngGyo_CNT).Text = strTKIN_NAME(lngPage_CNT, lngGyo_CNT)
                lblTSIT_NAME(lngGyo_CNT).Text = strTSIT_NAME(lngPage_CNT, lngGyo_CNT)

                Select Case strKAMOKU(lngPage_CNT, lngGyo_CNT)
                    Case "02"
                        lblKAMOKU(lngGyo_CNT).Text = "普通"
                    Case "01"
                        lblKAMOKU(lngGyo_CNT).Text = "当座"
                    Case "05"
                        lblKAMOKU(lngGyo_CNT).Text = "納税"
                    Case "37"
                        lblKAMOKU(lngGyo_CNT).Text = "職員"
                    Case "04"
                        lblKAMOKU(lngGyo_CNT).Text = "別段"
                    Case "99"
                        lblKAMOKU(lngGyo_CNT).Text = "諸勘定"
                    Case Else
                        lblKAMOKU(lngGyo_CNT).Text = "その他"
                End Select

                lblKOUZA(lngGyo_CNT).Text = strKOUZA(lngPage_CNT, lngGyo_CNT)
                If strKEIYAKU_NNAME(lngPage_CNT, lngGyo_CNT).Trim = "" Then
                    lblKEIYAKU_NNAME(lngGyo_CNT).Text = strKEIYAKU_KNAME(lngPage_CNT, lngGyo_CNT)
                Else
                    lblKEIYAKU_NNAME(lngGyo_CNT).Text = strKEIYAKU_NNAME(lngPage_CNT, lngGyo_CNT)
                End If
                lblKEIYAKU_NO(lngGyo_CNT).Text = strKEIYAKU_NO(lngPage_CNT, lngGyo_CNT)

                If strINPUT_FLG(lngPAGE) <> 1 Then        '引落金額:初回表示時のみ設定
                    If lblSyokiti.Text.Trim = "" Then
                        If strKINGAKU_KBN(lngPage_CNT, lngGyo_CNT) = "0" Then
                            txtFURIKIN(lngGyo_CNT).Text = fn_CHG_KANMA_S(Str(lngFURIKIN(lngPage_CNT, lngGyo_CNT)))
                        Else        '前回金額:非表示のときは空白
                            If strFURIDATE(lngPage_CNT, lngGyo_CNT) = strFURI_DATE Then
                                txtFURIKIN(lngGyo_CNT).Text = fn_CHG_KANMA_S(Str(lngFURIKIN(lngPage_CNT, lngGyo_CNT)))
                            Else
                                txtFURIKIN(lngGyo_CNT).Text = 0
                                lngFURIKIN(lngPage_CNT, lngGyo_CNT) = 0
                            End If
                        End If
                    Else
                        If strFURIDATE(lngPage_CNT, lngGyo_CNT) = strFURI_DATE Then
                            'すでに振込日が登録済みである場合は初期値の表示は行わない
                            txtFURIKIN(lngGyo_CNT).Text = fn_CHG_KANMA_S(Str(lngFURIKIN(lngPage_CNT, lngGyo_CNT)))
                        Else
                            txtFURIKIN(lngGyo_CNT).Text = lblSyokiti.Text.Trim
                            lngFURIKIN(lngPage_CNT, lngGyo_CNT) = fn_DEL_KANMA(txtFURIKIN(lngGyo_CNT).Text)
                        End If

                    End If

                Else
                    txtFURIKIN(lngGyo_CNT).Text = fn_CHG_KANMA_S(Str(lngFURIKIN(lngPage_CNT, lngGyo_CNT)))
                End If
                '手数料欄に口座チェック結果を表示する
                If strKEIYAKU_NNAME(lngPage_CNT, lngGyo_CNT) <> "<<解約済>>" Then
                    lblTESUU(lngGyo_CNT).Text = strCHK_KEKKA(lngPage_CNT, lngGyo_CNT)
                End If

                If txtFURIKIN(lngGyo_CNT).Text = 0 Then
                    lngSYOUKEI2 = lngSYOUKEI2 - 1
                Else
                    If strKAIYAKU(lngPage_CNT, lngGyo_CNT) = "1" Then   '引落金額:解約済の場合
                        txtFURIKIN(lngGyo_CNT).Text = 0
                        lngFURIKIN(lngPage_CNT, lngGyo_CNT) = fn_DEL_KANMA(txtFURIKIN(lngGyo_CNT).Text)
                        lngSYOUKEI2 = lngSYOUKEI2 - 1
                    End If
                End If
            Next

            '件数・手数料設定＆ボタン設定
            '-----------------------------
            '小計件数の表示
            '-----------------------------
            lblSyoukeiKen.Text = Format(fn_COUNT_K_SYOUKEI(), "#,##0")


            'lblSyoukeiKen.Text = Format(lngSYOUKEI_KEN, "#,##0")                            '共通設定
            If strINPUT_FLG(lngPAGE) = 1 Then     '入力済頁は小計表示
                txtSyoukeiKin.Text = Format(CInt(strINPUT_K_SYOUKEI(lngPAGE)), "#,##0")
                If txtSyoukeiKin.Text = "0" Then
                    txtSyoukeiKin.Text = ""
                End If
                ' txtSyoukeiKin.Text = Format(CInt(strINPUT_K_SYOUKEI(lngPAGE)), "#,###") '蒲郡信金向け 0でなく空白表示　2007/10/23
                lblSyoukeiTesuu.Text = strINPUT_T_SYOUKEI(lngPAGE)
            End If

            If aIN_PAGE = 1 Then                            '１頁設定
                btnPreGamen.Enabled = False
            End If
            If aIN_PAGE = lngMAX_Page Then                     '最終頁設定
                lblGOUKEI.Enabled = True
                lblGKEN.Enabled = True
                txtGoukeiKin.Enabled = True

                If fn_HYOUJI_GOUKEI() = False Then
                    Exit Function
                End If
                btnNextGamen.Enabled = False
                btnAction.Enabled = True
            End If

            I = 0
            Do Until I > 14
                If txtFURIKIN(I).Enabled = True Then
                    txtFURIKIN(I).Focus()
                    Exit Do
                End If
                I = I + 1
            Loop

            fn_HYOJI_SUB = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(画面表示･サブ)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_HYOUJI_GOUKEI() As Boolean
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_HYOUJI_GOUKEI
        'Parameter  :合計件数算出
        'Description:口座振込データ表示処理
        'Return     :True/False
        'Create     :2004/08/31
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        Dim P_CNT As Integer
        Dim L_CNT As Integer
        Dim lngGOUKEI_CNT As Long
        Try
            fn_HYOUJI_GOUKEI = False
            lblGOUKEI.Enabled = True
            lblGKEN.Enabled = True
            txtGoukeiKin.Enabled = True

            lngGOUKEI_CNT = 0
            For P_CNT = 0 To lngMAX_Page - 1
                For L_CNT = 0 To 14
                    If lngFURIKIN(P_CNT, L_CNT) > 0 Then
                        lngGOUKEI_CNT = lngGOUKEI_CNT + 1
                    End If
                Next
            Next
            lblGoukeiKen.Text = lngGOUKEI_CNT
            btnNextGamen.Enabled = False
            btnAction.Enabled = True

            fn_HYOUJI_GOUKEI = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振込金額表示処理)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_set_Control() As Boolean
        '============================================================================
        'NAME           :fn_set_Control
        'Parameter      :
        'Description    :コントロール配列セット
        'Return         :True=OK,False=NG
        'Create         :2004/08/30
        'Update         :
        '============================================================================
        fn_set_Control = False
        Try
            lblKOUBAN(0) = lblIndex1
            lblKOUBAN(1) = lblIndex2
            lblKOUBAN(2) = lblIndex3
            lblKOUBAN(3) = lblIndex4
            lblKOUBAN(4) = lblIndex5
            lblKOUBAN(5) = lblIndex6
            lblKOUBAN(6) = lblIndex7
            lblKOUBAN(7) = lblIndex8
            lblKOUBAN(8) = lblIndex9
            lblKOUBAN(9) = lblIndex10
            lblKOUBAN(10) = lblIndex11
            lblKOUBAN(11) = lblIndex12
            lblKOUBAN(12) = lblIndex13
            lblKOUBAN(13) = lblIndex14
            lblKOUBAN(14) = lblIndex15

            lblTKIN_NAME(0) = lblKinNName1
            lblTKIN_NAME(1) = lblKinNName2
            lblTKIN_NAME(2) = lblKinNName3
            lblTKIN_NAME(3) = lblKinNName4
            lblTKIN_NAME(4) = lblKinNName5
            lblTKIN_NAME(5) = lblKinNName6
            lblTKIN_NAME(6) = lblKinNName7
            lblTKIN_NAME(7) = lblKinNName8
            lblTKIN_NAME(8) = lblKinNName9
            lblTKIN_NAME(9) = lblKinNName10
            lblTKIN_NAME(10) = lblKinNName11
            lblTKIN_NAME(11) = lblKinNName12
            lblTKIN_NAME(12) = lblKinNName13
            lblTKIN_NAME(13) = lblKinNName14
            lblTKIN_NAME(14) = lblKinNName15

            lblTSIT_NAME(0) = lblSitNName1
            lblTSIT_NAME(1) = lblSitNName2
            lblTSIT_NAME(2) = lblSitNName3
            lblTSIT_NAME(3) = lblSitNName4
            lblTSIT_NAME(4) = lblSitNName5
            lblTSIT_NAME(5) = lblSitNName6
            lblTSIT_NAME(6) = lblSitNName7
            lblTSIT_NAME(7) = lblSitNName8
            lblTSIT_NAME(8) = lblSitNName9
            lblTSIT_NAME(9) = lblSitNName10
            lblTSIT_NAME(10) = lblSitNName11
            lblTSIT_NAME(11) = lblSitNName12
            lblTSIT_NAME(12) = lblSitNName13
            lblTSIT_NAME(13) = lblSitNName14
            lblTSIT_NAME(14) = lblSitNName15

            lblKAMOKU(0) = lblKamoku1
            lblKAMOKU(1) = lblKamoku2
            lblKAMOKU(2) = lblKamoku3
            lblKAMOKU(3) = lblKamoku4
            lblKAMOKU(4) = lblKamoku5
            lblKAMOKU(5) = lblKamoku6
            lblKAMOKU(6) = lblKamoku7
            lblKAMOKU(7) = lblKamoku8
            lblKAMOKU(8) = lblKamoku9
            lblKAMOKU(9) = lblKamoku10
            lblKAMOKU(10) = lblKamoku11
            lblKAMOKU(11) = lblKamoku12
            lblKAMOKU(12) = lblKamoku13
            lblKAMOKU(13) = lblKamoku14
            lblKAMOKU(14) = lblKamoku15

            lblKOUZA(0) = lblKouza1
            lblKOUZA(1) = lblKouza2
            lblKOUZA(2) = lblKouza3
            lblKOUZA(3) = lblKouza4
            lblKOUZA(4) = lblKouza5
            lblKOUZA(5) = lblKouza6
            lblKOUZA(6) = lblKouza7
            lblKOUZA(7) = lblKouza8
            lblKOUZA(8) = lblKouza9
            lblKOUZA(9) = lblKouza10
            lblKOUZA(10) = lblKouza11
            lblKOUZA(11) = lblKouza12
            lblKOUZA(12) = lblKouza13
            lblKOUZA(13) = lblKouza14
            lblKOUZA(14) = lblKouza15

            lblKEIYAKU_NNAME(0) = lblKeiyakuNM1
            lblKEIYAKU_NNAME(1) = lblKeiyakuNM2
            lblKEIYAKU_NNAME(2) = lblKeiyakuNM3
            lblKEIYAKU_NNAME(3) = lblKeiyakuNM4
            lblKEIYAKU_NNAME(4) = lblKeiyakuNM5
            lblKEIYAKU_NNAME(5) = lblKeiyakuNM6
            lblKEIYAKU_NNAME(6) = lblKeiyakuNM7
            lblKEIYAKU_NNAME(7) = lblKeiyakuNM8
            lblKEIYAKU_NNAME(8) = lblKeiyakuNM9
            lblKEIYAKU_NNAME(9) = lblKeiyakuNM10
            lblKEIYAKU_NNAME(10) = lblKeiyakuNM11
            lblKEIYAKU_NNAME(11) = lblKeiyakuNM12
            lblKEIYAKU_NNAME(12) = lblKeiyakuNM13
            lblKEIYAKU_NNAME(13) = lblKeiyakuNM14
            lblKEIYAKU_NNAME(14) = lblKeiyakuNM15

            lblKEIYAKU_NO(0) = lblKeiyakuNo1
            lblKEIYAKU_NO(1) = lblKeiyakuNo2
            lblKEIYAKU_NO(2) = lblKeiyakuNo3
            lblKEIYAKU_NO(3) = lblKeiyakuNo4
            lblKEIYAKU_NO(4) = lblKeiyakuNo5
            lblKEIYAKU_NO(5) = lblKeiyakuNo6
            lblKEIYAKU_NO(6) = lblKeiyakuNo7
            lblKEIYAKU_NO(7) = lblKeiyakuNo8
            lblKEIYAKU_NO(8) = lblKeiyakuNo9
            lblKEIYAKU_NO(9) = lblKeiyakuNo10
            lblKEIYAKU_NO(10) = lblKeiyakuNo11
            lblKEIYAKU_NO(11) = lblKeiyakuNo12
            lblKEIYAKU_NO(12) = lblKeiyakuNo13
            lblKEIYAKU_NO(13) = lblKeiyakuNo14
            lblKEIYAKU_NO(14) = lblKeiyakuNo15

            txtFURIKIN(0) = txtFuriKin1
            txtFURIKIN(1) = txtFuriKin2
            txtFURIKIN(2) = txtFuriKin3
            txtFURIKIN(3) = txtFuriKin4
            txtFURIKIN(4) = txtFuriKin5
            txtFURIKIN(5) = txtFuriKin6
            txtFURIKIN(6) = txtFuriKin7
            txtFURIKIN(7) = txtFuriKin8
            txtFURIKIN(8) = txtFuriKin9
            txtFURIKIN(9) = txtFuriKin10
            txtFURIKIN(10) = txtFuriKin11
            txtFURIKIN(11) = txtFuriKin12
            txtFURIKIN(12) = txtFuriKin13
            txtFURIKIN(13) = txtFuriKin14
            txtFURIKIN(14) = txtFuriKin15

            lblTESUU(0) = lblTesuuKin1
            lblTESUU(1) = lblTesuuKin2
            lblTESUU(2) = lblTesuuKin3
            lblTESUU(3) = lblTesuuKin4
            lblTESUU(4) = lblTesuuKin5
            lblTESUU(5) = lblTesuuKin6
            lblTESUU(6) = lblTesuuKin7
            lblTESUU(7) = lblTesuuKin8
            lblTESUU(8) = lblTesuuKin9
            lblTESUU(9) = lblTesuuKin10
            lblTESUU(10) = lblTesuuKin11
            lblTESUU(11) = lblTesuuKin12
            lblTESUU(12) = lblTesuuKin13
            lblTESUU(13) = lblTesuuKin14
            lblTESUU(14) = lblTesuuKin15
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コントロール配列作成)", "失敗", ex.ToString)
            Return False
        End Try
        fn_set_Control = True
    End Function

    Function fn_CHG_KANMA_S(ByVal aINTEXT As String) As String
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_CHG_KANMA_S
        'Parameter  :aINTEXT:入力値
        'Description:カンマ編集(カンマ付け）関数
        'Return     :置換後の文字(STRING)
        'Create     :2004/08/31
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        Dim I As Integer
        Dim strO_SYOKITI As String = ""

        If aINTEXT.Trim <> "" Then
            For I = 0 To aINTEXT.Length - 1        'ｶﾝﾏ&ｽﾍﾟｰｽとる
                If aINTEXT.Substring(I, 1) = "," Or aINTEXT.Substring(I, 1) = " " Then
                    strO_SYOKITI = strO_SYOKITI & ""
                Else
                    strO_SYOKITI = strO_SYOKITI & aINTEXT.Substring(I, 1)
                End If
            Next I
            fn_CHG_KANMA_S = Format(CLng(strO_SYOKITI), "#,##0")
        Else
            fn_CHG_KANMA_S = ""
        End If
    End Function

    Function fn_DEL_KANMA(ByVal aINTEXT As String) As Long
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_DEL_KANMA
        'Parameter  :aINTEXT:入力値
        'Description:カンマ編集(カンマ削除）関数
        'Return     :置換後の文字(LONG)
        'Create     :2004/08/31
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        Dim I As Integer
        Dim strO_SYOKITI As String = ""

        For I = 0 To aINTEXT.Length - 1        'ｶﾝﾏ&ｽﾍﾟｰｽとる
            If aINTEXT.Substring(I, 1) = "," Or aINTEXT.Substring(I, 1) = " " Then
                strO_SYOKITI = strO_SYOKITI & ""
            Else
                strO_SYOKITI = strO_SYOKITI & aINTEXT.Substring(I, 1)
            End If
        Next I

        fn_DEL_KANMA = CLng(strO_SYOKITI)

    End Function
    Function fn_DEL_KANMA_STR(ByVal aINTEXT As String) As String
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_DEL_KANMA_STR
        'Parameter  :aINTEXT:入力値
        'Description:カンマ編集(カンマ削除）関数
        'Return     :置換後の文字(STRING)
        'Create     :2007/08/07
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        Dim I As Integer
        Dim strO_SYOKITI As String = ""

        For I = 0 To aINTEXT.Length - 1        'ｶﾝﾏ&ｽﾍﾟｰｽとる
            If aINTEXT.Substring(I, 1) = "," Or aINTEXT.Substring(I, 1) = " " Then
                strO_SYOKITI = strO_SYOKITI & ""
            Else
                strO_SYOKITI = strO_SYOKITI & aINTEXT.Substring(I, 1)
            End If
        Next I

        fn_DEL_KANMA_STR = strO_SYOKITI

    End Function

    Function fn_CHECK_SYOUKEI() As Boolean
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_CHECK_SYOUKEI
        'Parameter  :
        'Description:小計ﾁｪｯｸ
        'Return     :TRUE:OK/FALSE:NG
        'Create     :2004/09/01
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        Dim SYOUKEI_KEN As Integer
        Try
            fn_CHECK_SYOUKEI = False
            If txtSyoukeiKin.Text.Trim.Length <> 0 Then
                txtSyoukeiKin.Text = fn_DEL_KANMA(txtSyoukeiKin.Text)   'ｶﾝﾏ編集

                '＜小計チェック＞
                If txtSyoukeiKin.Text <> fn_KEISAN_K_SYOUKEI(SYOUKEI_KEN) Then
                    MessageBox.Show(String.Format(MSG0129W, GCom.NzLong(txtSyoukeiKin.Text).ToString("#,##0"), GCom.NzLong(fn_KEISAN_K_SYOUKEI(SYOUKEI_KEN)).ToString("#,##0")), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSyoukeiKin.Text = fn_CHG_KANMA_S(txtSyoukeiKin.Text)   'ｶﾝﾏ編集
                    txtSyoukeiKin.Focus()
                    Exit Function
                End If
                txtSyoukeiKin.Text = fn_CHG_KANMA_S(txtSyoukeiKin.Text)   'ｶﾝﾏ編集
                'lblSyoukeiTesuu.Text = fn_CHG_KANMA_S(CStr(fn_KEISAN_T_SYOUKEI()))   '手数料計算
            Else
                '未入力の場合はチェックしない
            End If
            fn_CHECK_SYOUKEI = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(小計チェック)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_CHECK_GOUKEI() As Boolean
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_CHECK_GOUKEI
        'Parameter  :
        'Description:合計ﾁｪｯｸ
        'Return     :
        'Create     :2004/09/01
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        fn_CHECK_GOUKEI = False
        Try
            If txtGoukeiKin.Text.Trim.Length <> 0 Then
                txtGoukeiKin.Text = fn_DEL_KANMA(txtGoukeiKin.Text)
                '＜合計チェック＞
                If txtGoukeiKin.Text <> CStr(fn_KEISAN_K_GOUKEI(lngMAX_Page)) Then
                    MessageBox.Show(String.Format(MSG0130W, GCom.NzLong(txtGoukeiKin.Text).ToString("#,##0"), fn_KEISAN_K_GOUKEI(lngMAX_Page).ToString("#,##0")), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGoukeiKin.Focus()
                    Exit Function
                End If
                txtGoukeiKin.Text = fn_CHG_KANMA_S(txtGoukeiKin.Text)    'ｶﾝﾏ編集
                'lblGoukeiTesuu.Text = fn_CHG_KANMA_S(CStr(fn_KEISAN_T_GOUKEI(lngMAX_Page)))
            Else '＜合計入力チェック＞
                MessageBox.Show(MSG0249W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGoukeiKin.Focus()
                Exit Function
            End If

            fn_CHECK_GOUKEI = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(合計チェック)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_KEISAN_K_SYOUKEI(ByVal SYOUKEI_KEN As Integer) As Long
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_KEISAN_K_SYOUKEI
        'Parameter  :
        'Description:小計計算
        'Return     :小計
        'Create     :2004/09/01
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        lngPage_CNT = lngPAGE - 1
        lngGyo_CNT = 0
        fn_KEISAN_K_SYOUKEI = 0
        For lngGyo_CNT = 0 To 14
            fn_KEISAN_K_SYOUKEI = fn_KEISAN_K_SYOUKEI + lngFURIKIN(lngPage_CNT, lngGyo_CNT)
            If lngFURIKIN(lngPage_CNT, lngGyo_CNT) <> 0 Then
                SYOUKEI_KEN = SYOUKEI_KEN + 1
            End If
        Next
        lngSYOUKEI_KEN = SYOUKEI_KEN
    End Function

    Function fn_COUNT_K_SYOUKEI() As Integer
        '============================================================================
        'Name       :fn_COUNT_K_SYOUKEI
        'Parameter  :
        'Description:小計件数計算
        'Return     :小計件数
        'Create     :2004/10/25
        'Update     :
        '============================================================================
        lngPage_CNT = lngPAGE - 1
        lngGyo_CNT = 0
        fn_COUNT_K_SYOUKEI = 0
        For lngGyo_CNT = 0 To 14
            If lngFURIKIN(lngPage_CNT, lngGyo_CNT) <> 0 Then
                fn_COUNT_K_SYOUKEI += 1
            End If
        Next
    End Function

    Function fn_KEISAN_T_SYOUKEI() As Long
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_KEISAN_T_SYOUKEI
        'Parameter  :
        'Description:手数料小計計算
        'Return     :手数料小計
        'Create     :2004/09/01
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        lngPage_CNT = lngPAGE - 1
        lngGyo_CNT = 0
        fn_KEISAN_T_SYOUKEI = 0
        For lngGyo_CNT = 0 To 14
            fn_KEISAN_T_SYOUKEI = fn_KEISAN_T_SYOUKEI + lngTESUU(lngPage_CNT, lngGyo_CNT)
        Next

    End Function

    Function fn_KEISAN_K_GOUKEI(ByVal aIN_MAXPage As Long) As Long
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_KEISAN_T_GOUKEI
        'Parameter  :aIN_MAXPage:頁MAX
        'Description:合計計算
        'Return     :合計
        'Create     :2004/09/02
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        Dim lngK_SUM As Long    '手数料小計
        lngPage_CNT = 0
        lngGyo_CNT = 0
        lngK_SUM = 0
        fn_KEISAN_K_GOUKEI = 0

        For lngPage_CNT = 0 To aIN_MAXPage - 1
            lngK_SUM = 0
            For lngGyo_CNT = 0 To 14
                lngK_SUM = lngK_SUM + lngFURIKIN(lngPage_CNT, lngGyo_CNT)
            Next
            fn_KEISAN_K_GOUKEI = fn_KEISAN_K_GOUKEI + lngK_SUM
        Next

    End Function

    Function fn_KEISAN_T_GOUKEI(ByVal aIN_MAXPage As Long) As Long
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        'Name       :fn_KEISAN_T_GOUKEI
        'Parameter  :aIN_MAXPage:頁MAX
        'Description:手数料合計計算
        'Return     :手数料合計
        'Create     :2004/09/02
        'Update     :
        '+++++++++++++++++++++++++++++++++++++++++++++++++
        Dim lngT_SUM As Long   '手数料小計

        lngPage_CNT = 0
        lngGyo_CNT = 0
        lngT_SUM = 0
        fn_KEISAN_T_GOUKEI = 0

        For lngPage_CNT = 0 To aIN_MAXPage - 1
            lngT_SUM = 0
            For lngGyo_CNT = 0 To 14
                lngT_SUM = lngT_SUM + lngTESUU(lngPage_CNT, lngGyo_CNT)
            Next
            fn_KEISAN_T_GOUKEI = fn_KEISAN_T_GOUKEI + lngT_SUM
        Next

    End Function

    Function fn_ENTMAST_WRITE(ByVal alngPage As Long) As Integer
        '----------------------------------
        'エントリーマスタ更新サブルーチン
        '----------------------------------
        Dim SQL As StringBuilder
        Try
            Dim lngLOOP_CNT As Long
            fn_ENTMAST_WRITE = gintKEKKA.NG

            lngLOOP_CNT = (alngPage * 15) + lngSYOUKEI
            lngPage_CNT = 0
            lngGyo_CNT = 0
            For lngPage_CNT = 0 To lngMAX_Page - 1
                Dim L_END As Integer
                If lngPage_CNT = lngMAX_Page - 1 Then
                    L_END = lngMAX_Gyo - 1
                Else
                    L_END = 15 - 1
                End If
                For lngGyo_CNT = 0 To L_END

                    '    If lngGyo_CNT = 15 Then
                    '        lngGyo_CNT = 0
                    '        lngPage_CNT = lngPage_CNT + 1
                    '        If lngPage_CNT > alngPage Then
                    '            Exit For
                    '        End If
                    '    End If
                    SQL = New StringBuilder(128)
                    SQL.Append(" UPDATE " & strMAST & " SET")
                    SQL.Append(" FURI_DATE_E = " & SQ(strFURI_DATE)) '振込日
                    SQL.Append(",FURIKIN_E = " & lngFURIKIN(lngPage_CNT, lngGyo_CNT)) '振込金額
                    SQL.Append(",ERR_MSG_E = " & SQ(strCHK_KEKKA(lngPage_CNT, lngGyo_CNT))) '口座チェック結果
                    SQL.Append(",KOUSIN_DATE_E =" & SQ(Format(Now, "yyyyMMdd"))) '更新日
                    SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
                    SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
                    SQL.Append(" AND KEIYAKU_NO_E = " & SQ(strKEIYAKU_NO(lngPage_CNT, lngGyo_CNT)))

                    '更新
                    If MainDB.ExecuteNonQuery(SQL) < 1 Then
                        '更新失敗(MSG0002E)
                        MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(依頼書更新処理)", "失敗", "契約者番号:" & strKEIYAKU_NO(lngPage_CNT, lngGyo_CNT))
                        Return gintKEKKA.NG
                    End If

                Next
            Next


            'For lngGyo_CNT = 0 To lngLOOP_CNT Step 1 '行順


            lngPage_CNT = alngPage
            fn_ENTMAST_WRITE = gintKEKKA.OK

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(依頼書更新処理)", "失敗", ex.ToString)
            Return gintKEKKA.NG
        End Try
    End Function

    Function fn_update_SCHMAST(ByRef UKETUKE As String) As Boolean
        '============================================================================
        'NAME           :fn_update_SCHMAST
        'Parameter      :
        'Description    :更新処理(変更可能は各フラグのみ)
        'Return         :True=OK,False=NG
        'Create         :2004/08/04
        'Update         :
        '============================================================================
        fn_update_SCHMAST = False
        Try
            Dim SQL As New StringBuilder(128)
            SQL.Append(" UPDATE S_SCHMAST SET")
            SQL.Append(" UKETUKE_DATE_S = " & SQ(Format(Now, "yyyyMMdd")))
            '振込金額0円の場合かつ主の場合は受付フラグを0にする
            If GCom.NzLong(fn_DEL_KANMA(txtGoukeiKin.Text)) = 0 AndAlso intCHK = 0 Then
                SQL.Append(",UKETUKE_FLG_S = '0'")      '未実行
                UKETUKE = "0"
            ElseIf GCom.NzLong(fn_DEL_KANMA(txtGoukeiKin.Text)) > 0 AndAlso BERYFAI = "0" AndAlso intCHK = 0 Then
                '振込金額0以上かつベリファイ無しの場合は受付フラグを1にする
                SQL.Append(",UKETUKE_FLG_S = '1'")      '受付済
                UKETUKE = "1"
            ElseIf GCom.NzLong(fn_DEL_KANMA(txtGoukeiKin.Text)) > 0 AndAlso BERYFAI = "1" AndAlso intCHK = 1 Then
                '振込金額0以上かつ副の場合は受付フラグを1にする
                SQL.Append(",UKETUKE_FLG_S = '1'")      '受付済
                UKETUKE = "1"
            ElseIf GCom.NzLong(fn_DEL_KANMA(txtGoukeiKin.Text)) = 0 AndAlso BERYFAI = "1" AndAlso intCHK = 1 Then
                '振込金額0円の場合かつ副の場合は受付フラグを2にする
                SQL.Append(",UKETUKE_FLG_S = '2'")      'ベリファイ待ち
                UKETUKE = "3"
            Else
                '振込金額0以上かつベリファイありかつ主の場合は受付フラグを2にする
                SQL.Append(",UKETUKE_FLG_S = '2'")      'ベリファイ待ち
                UKETUKE = "2"
            End If

            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            SQL.Append(" AND FURI_DATE_S = " & SQ(strFURI_DATE))

            '更新
            If MainDB.ExecuteNonQuery(SQL) < 1 Then
                '更新失敗(MSG0002E)
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール更新処理)", "失敗", "")
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール更新処理)", "失敗", ex.ToString)
            Return False
        End Try
        fn_update_SCHMAST = True

    End Function

    Private Function fn_CLC_ENTMAST(ByRef Counter As Long, ByRef Kingaku As Long) As Boolean
        '============================================================================
        'NAME           :fn_CLC_ENTMAST
        'Parameter      :Counter 合計件数 ,Kingaku 合計金額
        'Description    :依頼書の登録対象となる件数･金額を取得する
        'Return         :True=OK,False=NG
        'Create         :2009/10/02
        'Update         :
        '============================================================================
        fn_CLC_ENTMAST = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            '-------------------------------------------
            '依頼書マスタの合計件数･金額を取得
            '-------------------------------------------
            '振込金額が0円以上のものを集計
            SQL.Append(" SELECT NVL(COUNT(FURIKIN_E),0) COUNTER ,NVL(SUM(FURIKIN_E),0) SUM_KIN")
            SQL.Append(" FROM S_ENTMAST")
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            SQL.Append(" AND FURI_DATE_E = " & SQ(strFURI_DATE))
            SQL.Append(" AND FURIKIN_E > 0")

            If OraReader.DataReader(SQL) = True Then
                Counter = GCom.NzLong(OraReader.Reader.Item("COUNTER"))
                Kingaku = GCom.NzLong(OraReader.Reader.Item("SUM_KIN"))
                OraReader.Close()
            Else
                '検索失敗(MSG0002E)
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                Return False
            End If
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(依頼書合計金額･件数取得)", "失敗", ex.ToString)
            OraReader.Close()
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_CLC_ENTMAST = True
    End Function

    Function fn_set_INIFILE() As Boolean
        '----------------------------------------------------------------------------
        'Name       :fn_set_INIFILE
        'Description:INIファイルの情報セット
        'Create     :2009/09/29
        'Update     :
        '----------------------------------------------------------------------------
        fn_set_INIFILE = False
        Try
            '自金庫コード
            gstrJIKINKO = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If gstrJIKINKO.ToUpper = "ERR" OrElse gstrJIKINKO = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                Return False
            End If

            '税率
            gstrCHK_DEJIT = CASTCommon.GetFSKJIni("COMMON", "ZEIRITU")
            If gstrCHK_DEJIT.ToUpper = "ERR" OrElse gstrCHK_DEJIT = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "消費税率", "COMMON", "ZEIRITU"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                MainLOG.Write("設定ファイル取得", "失敗", "項目名:消費税率 分類:COMMON 項目:ZEIRITU")
                Return False
            End If

            '口座チェック判定
            gstrCHK_KOUZA = CASTCommon.GetFSKJIni("COMMON", "KOUZACHK")
            If gstrCHK_KOUZA.ToUpper = "ERR" OrElse gstrCHK_KOUZA = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "口座チェック判定", "COMMON", "KOUZACHK"), _
                   msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                MainLOG.Write("設定ファイル取得", "失敗", "項目名:口座チェック判定 分類:COMMON 項目:KOUZACHK")
                Return False
            End If

            'ETC格納フォルダ
            EtcFolder = CASTCommon.GetFSKJIni("COMMON", "ETC")
            If EtcFolder.ToUpper = "ERR" OrElse EtcFolder = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "ETC格納フォルダ", "COMMON", "ETC"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名: 分類:COMMON 項目:ETC")
                Return False
            End If

            'DAT格納フォルダ
            DatFolder = CASTCommon.GetFSKJIni("COMMON", "DAT")
            If DatFolder.ToUpper = "ERR" OrElse DatFolder = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "DAT格納フォルダ", "COMMON", "DAT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:DAT格納フォルダ 分類:COMMON 項目:DAT")
                Return False
            End If

            ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
            '==================================================================
            '　RSV2機能設定
            '==================================================================
            Ini_Info.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            If Ini_Info.RSV2_EDITION.ToUpper = "ERR" OrElse Ini_Info.RSV2_EDITION = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "RSV2機能設定", "RSV2_V1.0.0", "EDITION"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:RSV2機能設定 分類:RSV2_V1.0.0 項目:EDITION")
                Return False
            End If
            ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(INIファイル設定)", "失敗", ex.ToString)
            Return False
        End Try

        fn_set_INIFILE = True

    End Function

    Function fn_GAMEN_HOZON(ByVal alngPage As Long) As Boolean
        Try
            '画面の値を保存
            Dim I As Long

            For I = 0 To 14
                If lblKOUBAN(I).Text = "" Then
                    lngKOUBAN(alngPage - 1, I) = 0
                Else
                    lngKOUBAN(alngPage - 1, I) = lblKOUBAN(I).Text
                End If
                strTKIN_NAME(alngPage - 1, I) = lblTKIN_NAME(I).Text
                strTSIT_NAME(alngPage - 1, I) = lblTSIT_NAME(I).Text
                Select Case lblKAMOKU(I).Text
                    Case "普通"
                        strKAMOKU(alngPage - 1, I) = "02"
                    Case "当座"
                        strKAMOKU(alngPage - 1, I) = "01"
                    Case "納税"
                        strKAMOKU(alngPage - 1, I) = "05"
                    Case "職員"
                        strKAMOKU(alngPage - 1, I) = "37"
                    Case "別段"
                        strKAMOKU(alngPage - 1, I) = "04"
                    Case "諸勘定"
                        strKAMOKU(alngPage - 1, I) = "99"
                    Case Else
                        strKAMOKU(alngPage - 1, I) = "09"
                End Select
                strKOUZA(alngPage - 1, I) = lblKOUZA(I).Text
                If txtFURIKIN(I).Text = "" Then
                    lngFURIKIN(alngPage - 1, I) = 0
                Else
                    lngFURIKIN(alngPage - 1, I) = CLng(txtFURIKIN(I).Text)
                End If
                strCHK_KEKKA(alngPage - 1, I) = lblTESUU(I).Text.Trim '追加 2006/03/23
            Next

            fn_GAMEN_HOZON = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(画面保存)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_BERYFAI(ByVal aobjFURIKIN As TextBox, ByVal aobjlblKeiyakuNo As Label) As Boolean
        '============================================================================
        'NAME           :fn_BERYFAI
        'Parameter      :alngKOUBAN：項番／aobjKIN_NO：金融機関コード／aobjSIT_NO：支店コード
        '               :aobjKAMOKU：科目／aobjKOUZA：口座番号／aobjKEIYAKU_NAME：契約者名
        '               :aobjFURIKIN：振込金額
        'Description    :ベリファイチェック（正の値とチェック）
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/30
        'Update         :
        '============================================================================
        fn_BERYFAI = False

        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Dim MSG As String = ""
        Try
            Dim ERR_COLOR As Color
            ERR_COLOR = Color.Aqua
            MainDB.BeginTrans()
            '-------------------------------------------
            '依頼書マスタに登録されているか検索
            '-------------------------------------------
            SQL.Append("SELECT * FROM S_ENTMAST")
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            SQL.Append(" AND KEIYAKU_NO_E = " & SQ(aobjlblKeiyakuNo.Text))

            If OraReader.DataReader(SQL) = False Then
                If MessageBox.Show(MSG0251W, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) Then
                    aobjFURIKIN.Focus()
                    Exit Function
                End If
            Else
                '振込金額のチェック
                If fn_DEL_KANMA(aobjFURIKIN.Text) <> GCom.NzLong(OraReader.GetString("FURIKIN_E")) Then

                    MSG = String.Format(MSG0028I, "振込金額", Format(GCom.NzLong(OraReader.GetString("FURIKIN_E")), "#,##0"), GCom.NzLong(fn_DEL_KANMA(aobjFURIKIN.Text)).ToString("#,##0"))
                    If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        aobjFURIKIN.BackColor = ERR_COLOR
                        aobjFURIKIN.Focus()
                        Exit Function
                    Else
                        MSG = String.Format(MSG0029I, Format(GCom.NzLong(OraReader.GetString("FURIKIN_E")), "#,##0"), GCom.NzLong(fn_DEL_KANMA(aobjFURIKIN.Text)).ToString("#,##0"))
                        If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            If fn_UPDATE_ENTMAST_SEI(fn_DEL_KANMA(aobjFURIKIN.Text), aobjlblKeiyakuNo.Text) = False Then
                                MainDB.Rollback()
                                Exit Function
                            End If
                            MainDB.Commit()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ベリファイ)", "失敗", ex.ToString)
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        fn_BERYFAI = True
    End Function

    Function fn_UPDATE_ENTMAST_SEI(ByVal Furikin As Long, ByVal KeiyakuNo As String) As Boolean
        '============================================================================
        'NAME           :fn_UPDATE_ENTMAST_SEI
        'Parameter      :astrFIELD：フィールド名／astrATAI：更新値／alngKOUBAN：項番
        'Description    :正（依頼書マスタ）の値を更新する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/10/02
        'Update         :
        '============================================================================
        fn_UPDATE_ENTMAST_SEI = False
        Dim Ret As Integer
        Dim SQL As New StringBuilder(128)
        Try
            SQL.Append(" UPDATE S_ENTMAST SET ")
            SQL.Append(" FURIKIN_E = " & Furikin)
            SQL.Append(",KOUSIN_DATE_E = " & SQ(Format(System.DateTime.Today, "yyyyMMdd")))
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND   TORIF_CODE_E = " & SQ(strTORIF_CODE))
            SQL.Append(" AND   KEIYAKU_NO_E = " & SQ(KeiyakuNo))

            Ret = MainDB.ExecuteNonQuery(SQL)
            If Ret < 1 Then
                MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(正マスタ更新)", "失敗", ex.ToString)
            Return False
        End Try

        fn_UPDATE_ENTMAST_SEI = True
    End Function

    Private Function MakeRecord() As Boolean
        '------------------------------------------------
        '全銀データの作成
        '------------------------------------------------
        Dim SQL As New StringBuilder(128)
        Dim OraReader As New MyOracleReader(MainDB)
        Dim FileWrite As StreamWriter = Nothing
        Dim gZENGIN_REC1 As CAstFormat.CFormatZengin.ZGRECORD1 = Nothing
        Dim gZENGIN_REC2 As CAstFormat.CFormatZengin.ZGRECORD2 = Nothing
        Dim gZENGIN_REC8 As CAstFormat.CFormatZengin.ZGRECORD8 = Nothing
        Dim gZENGIN_REC9 As CAstFormat.CFormatZengin.ZGRECORD9 = Nothing
        Dim strIRAI_FILE_WRK As String, strIRAI_FILE As String
        Dim dblIRAI_KEN As Double = 0, dblIRAI_KIN As Double = 0
        Dim intRECORD_COUNT As Integer = 0
        Try

            ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
            TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

            'ヘッダーの作成
            SQL.Append(" SELECT * FROM S_TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))

            If OraReader.DataReader(SQL) = False Then
                OraReader.Close()
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            Else
                strIraisyo_Sort = GCom.NzStr(OraReader.GetItem("IRAISYO_SORT_T"))
                With gZENGIN_REC1
                    .ZG1 = "1"
                    .ZG2 = GCom.NzStr(OraReader.GetItem("SYUBETU_T")).PadRight(2)
                    .ZG3 = "0"
                    .ZG4 = GCom.NzStr(OraReader.GetItem("ITAKU_CODE_T")).PadRight(10)
                    .ZG5 = GCom.NzStr(OraReader.GetItem("ITAKU_KNAME_T")).PadRight(40)
                    .ZG6 = strFURI_DATE.Substring(4, 4)
                    .ZG7 = GCom.NzStr(OraReader.GetItem("TKIN_NO_T")).PadRight(4)
                    .ZG9 = GCom.NzStr(OraReader.GetItem("TSIT_NO_T")).PadRight(3)
                    If .ZG7.Trim = "" Or .ZG9.Trim = "" Then
                        .ZG8 = Space(15)
                        .ZG10 = Space(15)
                    Else
                        '金融機関名取得
                        If fn_Select_TENMAST(.ZG7, .ZG9, strKIN_NNM, strSIT_NNM, strKIN_KNM, strSIT_KNM) = True Then
                            .ZG8 = strKIN_KNM.PadRight(15)
                            .ZG10 = strSIT_KNM.PadRight(15)
                        Else
                            .ZG8 = Space(15)
                            .ZG10 = Space(15)
                        End If
                    End If
                    '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------START
                    .ZG11 = ConvertKamoku2TO1_K(GCom.NzStr(OraReader.GetItem("KAMOKU_T")).PadLeft(2, "0"))
                    '.ZG11 = ConvertKamoku2TO1(GCom.NzStr(OraReader.GetItem("KAMOKU_T")).PadLeft(2, "0"))
                    '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------END

                    .ZG12 = GCom.NzStr(OraReader.GetItem("KOUZA_T")).PadLeft(7, "0")
                    .ZG13 = Space(17)
                End With
                OraReader.Close()
            End If

            ' 2015/12/28 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
            'strIRAI_FILE_WRK = DatFolder & "E" & strTORIS_CODE & strTORIF_CODE & ".dat"
            'strIRAI_FILE = EtcFolder & "E" & strTORIS_CODE & strTORIF_CODE & ".dat"
            '=========================================================
            ' ファイル名設定
            '=========================================================
            Select Case Ini_Info.RSV2_EDITION
                Case "2"
                    '-------------------------------------------------
                    ' 大規模設定構築
                    '-------------------------------------------------
                    strIRAI_FILE_WRK = DatFolder & _
                                       "S_ENT_S_" & _
                                       strTORIS_CODE & strTORIF_CODE & "_" & _
                                       "04" & "_" & _
                                       strFURI_DATE & "_" & _
                                       TimeStamp & "_" & _
                                       Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                       "000" & _
                                       ".DAT"
                    strIRAI_FILE = EtcFolder & _
                                   "S_ENT_S_" & _
                                   strTORIS_CODE & strTORIF_CODE & "_" & _
                                   "04" & "_" & _
                                   strFURI_DATE & "_" & _
                                   TimeStamp & "_" & _
                                   Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                   "000" & _
                                   ".DAT"

                    '-------------------------------------------------
                    ' 同一スケジュールファイル削除
                    '-------------------------------------------------
                    Dim FileList() As String = Directory.GetFiles(EtcFolder)
                    For i As Integer = 0 To FileList.Length - 1 Step 1
                        Dim DelFileName As String = Path.GetFileName(FileList(i))
                        If DelFileName.Substring(0, 32) = Path.GetFileName(strIRAI_FILE).Substring(0, 32) Then
                            File.Delete(FileList(i))
                        End If
                    Next
                Case Else
                    '-------------------------------------------------
                    ' 標準設定構築
                    '-------------------------------------------------
                    strIRAI_FILE_WRK = DatFolder & "E" & strTORIS_CODE & strTORIF_CODE & ".dat"
                    strIRAI_FILE = EtcFolder & "E" & strTORIS_CODE & strTORIF_CODE & ".dat"
            End Select
            ' 2015/12/28 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

            FileWrite = New StreamWriter(strIRAI_FILE_WRK, False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))
            SQL = New StringBuilder(128)
            'SQL.Append(" SELECT * FROM S_ENTMAST")
            'SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            'SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            'SQL.Append(" AND KAIYAKU_E = '0'")
            'SQL.Append(" AND FURIKIN_E > 0")
            'SQL.Append(" AND KAISI_DATE_E <= " & SQ(strFURI_DATE))
            'Select Case strIraisyo_Sort
            '    '2010/02/18 新規コードをソート条件に加える
            '    Case "0"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,KEIYAKU_NO_E ASC")
            '    Case "1"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,KEIYAKU_KNAME_E,KEIYAKU_NO_E ASC")
            '    Case "2"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,JYUYOUKA_NO_E,KEIYAKU_NO_E ASC")
            '        '=================================================
            'End Select
            SQL.Append("SELECT ")
            SQL.Append(" TSIT_NO_E,TKIN_NO_E,KAMOKU_E,KOUZA_E,KEIYAKU_NO_E")
            SQL.Append(",FURI_DATE_E,JYUYOUKA_NO_E")
            SQL.Append(",KEIYAKU_KNAME_E,FURIKIN_E,SINKI_CODE_E")
            SQL.Append(",'0000000000' KOKYAKU_NO_E")
            SQL.Append(" FROM " & strMAST)
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            SQL.Append(" AND KAIYAKU_E = '0'")
            SQL.Append(" AND FURIKIN_E > 0")
            SQL.Append(" AND SINKI_CODE_E = '0'")
            SQL.Append(" AND KAISI_DATE_E <= " & SQ(strFURI_DATE)) '開始年月日を比較
            SQL.Append(" UNION SELECT ")
            SQL.Append(" TSIT_NO_E,TKIN_NO_E,KAMOKU_E,KOUZA_E,KEIYAKU_NO_E")
            SQL.Append(",FURI_DATE_E,JYUYOUKA_NO_E")
            SQL.Append(",KEIYAKU_KNAME_E,FURIKIN_E,SINKI_CODE_E")
            SQL.Append(",KOKYAKU_NO_E ")
            SQL.Append(" FROM " & strMAST)
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            SQL.Append(" AND KAIYAKU_E = '0'")
            SQL.Append(" AND FURIKIN_E > 0")
            SQL.Append(" AND SINKI_CODE_E <> '0'")
            SQL.Append(" AND KAISI_DATE_E <= " & SQ(strFURI_DATE)) '開始年月日を比較
            '2011/06/24 標準版修正 エントリ処理に新規コードは考慮しない ------------------START
            Select Case strIraisyo_Sort
                Case "0"
                    SQL.Append(" ORDER BY KEIYAKU_NO_E ASC")
                Case "1"
                    SQL.Append(" ORDER BY KEIYAKU_KNAME_E , KEIYAKU_NO_E ASC")
                Case "2"
                    SQL.Append(" ORDER BY JYUYOUKA_NO_E , KEIYAKU_NO_E ASC")
                    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- START
                Case "3"
                    SQL.Append(" ORDER BY TKIN_NO_E , TSIT_NO_E , KAMOKU_E , KOUZA_E ASC")
                    ' 2016/06/10 タスク）綾部 ADD 【PG】UI-03-06(RSV2対応<小浜信金>) -------------------- END
            End Select
            'Select Case strIraisyo_Sort
            '    Case "0"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,KOKYAKU_NO_E,KEIYAKU_NO_E ASC")
            '    Case "1"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,KOKYAKU_NO_E,KEIYAKU_KNAME_E,KEIYAKU_NO_E ASC")
            '    Case "2"
            '        SQL.Append(" ORDER BY SINKI_CODE_E,KOKYAKU_NO_E,JYUYOUKA_NO_E,KEIYAKU_NO_E ASC")
            'End Select
            '2011/06/24 標準版修正 エントリ処理に新規コードは考慮しない ------------------START
            '=======================================================

            'ヘッダー書き込み
            FileWrite.Write(gZENGIN_REC1.Data)

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    With gZENGIN_REC2
                        .ZG1 = "2"
                        .ZG2 = GCom.NzStr(OraReader.GetItem("TKIN_NO_E")).PadRight(4)
                        .ZG4 = GCom.NzStr(OraReader.GetItem("TSIT_NO_E")).PadRight(3)
                        If .ZG2.Trim = "" Or .ZG4.Trim = "" Then
                            .ZG3 = Space(15)
                            .ZG5 = Space(15)
                        Else
                            '金融機関名取得
                            If fn_Select_TENMAST(.ZG2, .ZG4, strKIN_NNM, strSIT_NNM, strKIN_KNM, strSIT_KNM) = True Then
                                .ZG3 = strKIN_KNM.PadRight(15)
                                .ZG5 = strSIT_KNM.PadRight(15)
                            Else
                                .ZG3 = Space(15)
                                .ZG5 = Space(15)
                            End If
                        End If
                        .ZG6 = Space(4)
                        '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------START
                        .ZG7 = ConvertKamoku2TO1_K(GCom.NzStr(OraReader.GetItem("KAMOKU_E")).PadLeft(2, "0"))
                        '.ZG7 = ConvertKamoku2TO1(GCom.NzStr(OraReader.GetItem("KAMOKU_E")).PadLeft(2, "0"))
                        '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------END
                        .ZG8 = GCom.NzStr(OraReader.GetItem("KOUZA_E")).PadLeft(7, "0")
                        .ZG9 = Mid(GCom.NzStr(OraReader.GetItem("KEIYAKU_KNAME_E")).PadRight(30), 1, 30)
                        .ZG10 = GCom.NzStr(OraReader.GetItem("FURIKIN_E")).PadLeft(10, "0")
                        .ZG11 = "0"
                        .ZG12 = GCom.NzStr(OraReader.GetItem("JYUYOUKA_NO_E")).PadRight(20).Substring(0, 10)
                        .ZG13 = GCom.NzStr(OraReader.GetItem("JYUYOUKA_NO_E")).PadRight(20).Substring(10, 10)
                        .ZG14 = "0"
                        .ZG15 = Space(8)
                    End With
                    intRECORD_COUNT += 1
                    FileWrite.Write(gZENGIN_REC2.Data)
                    dblIRAI_KEN += 1
                    dblIRAI_KIN += GCom.NzLong(OraReader.GetItem("FURIKIN_E"))
                    OraReader.NextRead()
                End While
            End If

            If dblIRAI_KEN > 0 Then
                'トレーラ部の書き込み
                With gZENGIN_REC8
                    .ZG1 = "8"
                    .ZG2 = Format(dblIRAI_KEN, "000000")
                    .ZG3 = Format(dblIRAI_KIN, "000000000000")
                    .ZG4 = Space(101)
                End With
                intRECORD_COUNT += 1
                FileWrite.Write(gZENGIN_REC8.Data)
                'エンド部の書き込み
                With gZENGIN_REC9
                    .ZG1 = "9"
                    .ZG2 = Space(119)
                End With
                intRECORD_COUNT += 1
                FileWrite.Write(gZENGIN_REC9.Data)
                FileWrite.Close()
                '媒体落し込みと同じフォルダにコピー
                If Dir(strIRAI_FILE) <> "" Then
                    Kill(strIRAI_FILE)
                End If
                FileCopy(strIRAI_FILE_WRK, strIRAI_FILE)

                '不要なファイルを削除
                If Dir(strIRAI_FILE_WRK) <> "" Then
                    Kill(strIRAI_FILE_WRK)
                End If
            Else
                Return False
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(依頼ファイル作成)", "失敗", ex.ToString)
        Finally
            If Not FileWrite Is Nothing Then FileWrite.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function
#End Region

#Region " イベント"
    '引落金額Validating
    Private Sub txtFuriKin_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtFuriKin1.Validating, txtFuriKin2.Validating, _
        txtFuriKin3.Validating, txtFuriKin4.Validating, txtFuriKin5.Validating, txtFuriKin6.Validating, txtFuriKin7.Validating, txtFuriKin8.Validating, txtFuriKin9.Validating, _
        txtFuriKin10.Validating, txtFuriKin11.Validating, txtFuriKin12.Validating, txtFuriKin13.Validating, txtFuriKin14.Validating, txtFuriKin15.Validating
        Dim strtxtFLG_NM As String
        Dim strINDEX As String
        Try
            MainDB = New CASTCommon.MyOracle
            strtxtFLG_NM = sender.Name
            If strtxtFLG_NM.Length = 11 Then
                strINDEX = strtxtFLG_NM.Substring(10, 1)
            Else
                strINDEX = strtxtFLG_NM.Substring(10, 2)
            End If


            If txtFURIKIN(strINDEX - 1).Text.Trim = "" Then
                If lblSyokiti.Text.Length <> 0 Then
                    txtFURIKIN(strINDEX - 1).Text = fn_DEL_KANMA(lblSyokiti.Text)
                Else
                    txtFURIKIN(strINDEX - 1).Text = "0"
                End If
            End If

            '配列に保存
            lngFURIKIN(lngPAGE - 1, strINDEX - 1) = fn_DEL_KANMA(txtFURIKIN(strINDEX - 1).Text)

            '-------------------------------------------------
            'ベリファイチェック
            '-------------------------------------------------
            If intCHK = 1 Then '副にチェックの場合
                If fn_BERYFAI(txtFURIKIN(strINDEX - 1), lblKEIYAKU_NO(strINDEX - 1)) = False Then
                    Exit Sub
                End If
            End If

            '口座チェック
            Dim strERR_MSG As String = ""
            If gstrCHK_KOUZA.ToUpper = "YES" Then
                If strTSIT_NO(lngPAGE - 1, strINDEX - 1) <> "" Then
                    If gstrJIKINKO = strTKIN_NO(lngPAGE - 1, strINDEX - 1) Then   '自金庫のみ口座チェック
                        '2011/06/28 標準版修正 総振は自振契約なしは正常扱い ------------------START
                        Dim Ret As Integer = GCom.KouzaChk_ENTRY(strTSIT_NO(lngPAGE - 1, strINDEX - 1), strKAMOKU(lngPAGE - 1, strINDEX - 1), _
                                            strKOUZA(lngPAGE - 1, strINDEX - 1), strKIGYO_CD, strFURI_CD, _
                                            "", strERR_MSG, MainDB)
                        If Ret = -1 Then
                            strCHK_KEKKA(lngPAGE - 1, strINDEX - 1) = "チェック失敗"
                            Exit Sub
                        Else
                            If Ret = 2 Then
                                strERR_MSG = ""
                            End If
                            strCHK_KEKKA(lngPAGE - 1, strINDEX - 1) = strERR_MSG
                        End If
                        'If GCom.KouzaChk_ENTRY(strTSIT_NO(lngPAGE - 1, strINDEX - 1), strKAMOKU(lngPAGE - 1, strINDEX - 1), _
                        '                       strKOUZA(lngPAGE - 1, strINDEX - 1), strKIGYO_CD, strFURI_CD, _
                        '                       "", strERR_MSG, MainDB) = -1 Then
                        '    Exit Sub
                        'Else
                        '    strCHK_KEKKA(lngPAGE - 1, strINDEX - 1) = strERR_MSG
                        'End If
                        '2011/06/28 標準版修正 総振は自振契約なしは正常扱い ------------------END
                    Else
                        strCHK_KEKKA(lngPAGE - 1, strINDEX - 1) = ""
                    End If
                    strCHK_KEKKA(lngPAGE - 1, strINDEX - 1) = ""
                End If
            End If

            MainDB.Close()

            '手数料欄に口座チェック結果を出す
            'strCHK_KEKKA(lngPAGE - 1, strINDEX - 1) = lblTESUU(strINDEX - 1).Text.Trim
            lblTESUU(strINDEX - 1).Text = strERR_MSG

            '件数・手数料設定＆ボタン設定
            '-----------------------------
            '小計件数の表示
            '-----------------------------
            lblSyoukeiKen.Text = Format(fn_COUNT_K_SYOUKEI(), "#,##0")

            If lngPAGE = lngMAX_Page Then                     '最終頁設定
                lblGOUKEI.Enabled = True
                lblGKEN.Enabled = True
                txtGoukeiKin.Enabled = True

                If fn_HYOUJI_GOUKEI() = False Then
                    Exit Sub
                End If
                btnNextGamen.Enabled = False
                btnAction.Enabled = True
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(引落金額設定)", "失敗", ex.ToString)
            Return
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub

    '2010/02/18 イベント追加 ==================
    Private Sub txtSyoukeiKin_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSyoukeiKin.Validated
        If lngPAGE <> lngMAX_Page Then
            btnNextGamen.Focus()
        End If
    End Sub

    Private Sub txtGoukeiKin_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGoukeiKin.Validated
        If lngPAGE = lngMAX_Page Then
            btnAction.Focus()
        End If
    End Sub
    '==========================================
#End Region

End Class