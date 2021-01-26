Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFSENTR041
#Region " 変数"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    '' 非許可文字設定
    '2017/05/22 saitou RSV2 MODIFY 標準版（潜在バグ修正） ------------------------------------- START
    '「"」は許可しない。
    Private CASTx2 As New CASTCommon.Events("""", CASTCommon.Events.KeyMode.BAD)
    'Private CASTx2 As New CASTCommon.Events("", CASTCommon.Events.KeyMode.BAD)
    '2017/05/22 saitou RSV2 MODIFY ------------------------------------------------------------ END

    Private MainLOG As New CASTCommon.BatchLOG("KFSENTR041", "伝票用情報入力画面")
    Private Const msgTitle As String = "伝票用情報入力画面(KFSENTR041)"

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
    Private strJIKINKO_CD As String
    Private strKIGYO_CODE As String
    Private strFURI_CODE As String
    Private strKDB_KNAME As String
    Private strERR_MSG As String
    Private strERR_COLOR As Color
    Private strMAST As String
    Private P As Integer
    Private L As Integer
    Private intPAGE_CNT As Integer    '表示しているページ番号
    Private intMAXCNT As Integer      '最大ページ番号
    Private intMAXLINE As Integer     '最大ページの最終行
    Private intENT_COUNT As Integer
    Private intSYOUKEI_KEN As Integer '小計のカウント

    'SQLキー項目用
    Private strKEY1 As String
    Private strKEY2 As String
    Private strKEY3 As String

    Private lngMAX_PAGE As Long   '頁MAX
    Private lngMAX_GYO As Long    '行MAX（最終頁）
    Private lngMAX_RECORD As Long 'ﾚｺｰﾄﾞMAX
    Private lngSYOUKEI As Long    '対象頁行数(小計件数)
    Private lngSYOUKEI2 As Long  '解約数または振込金額0円のものを引いた小計件数
    Private lngKAIYAKU_CNT As Long '全解約数
    Private lngPAGE_CNT As Long
    Private lngGYO_CNT As Long

    '画面表示項目用配列
    Public strDEL_CHK(700, 15) As String
    Public lngKOUBAN(700, 15) As Long
    Public strTKIN_NO(700, 15) As String
    Public strTKIN_NAME(700, 15) As String
    Public strTSIT_NO(700, 15) As String
    Public strTSIT_NAME(700, 15) As String
    Public strKAMOKU(700, 15) As String
    Public strKOUZA(700, 15) As String
    Public strKEIYAKU_KNAME(700, 15) As String
    Public lngFURIKIN(700, 15) As Long
    Public strBIKOU(700, 15) As String
    Public lngRECORD_NO(700, 15) As Long
    '画面の小計
    Public lngSYOUKEI_KEN(700) As Long
    Public lngGOUKEI_KEN As Long

    Private strD_TKIN_NO As String                ' 取引先マスタ 振込処理区分
    Private strD_TSIT_NO As String                ' 取引先マスタ 振込処理区分
    Private strD_KAMOKU_CD As String                ' 取引先マスタ 振込処理区分
    Private strD_KOUZA As String                ' 取引先マスタ 振込処理区分
    Private strD_KEIYAKU_NNAME As String                ' 取引先マスタ 振込処理区分
    Private strD_FURIKIN As String
    'ページの色を取得
    Private clrTKIN_NO_Color(700, 15) As Color
    Private clrTKIN_NAME_Color(700, 15) As Color
    Private clrTSIT_NO_Color(700, 15) As Color
    Private clrTSIT_NAME_Color(700, 15) As Color
    Private clrKAMOKU_Color(700, 15) As Color
    Private clrKOUZA_Color(700, 15) As Color
    Private clrKEIYAKU_NNAME_Color(700, 15) As Color
    Private clrFURIKIN_Color(700, 15) As Color

    'コントロール配列
    Dim ckbDel(15) As CheckBox
    Dim lblIndex(15) As Label
    Dim txtKinNo(15) As TextBox
    Dim txtKinName(15) As TextBox
    Dim txtSitNo(15) As TextBox
    Dim txtSitName(15) As TextBox
    Dim txtKamokuCode(15) As TextBox
    Dim lblKamoku(15) As Label
    Dim txtKouza(15) As TextBox
    Dim txtFurikin(15) As TextBox
    Dim txtKeiyakuName(15) As TextBox
    Dim lblBikou(15) As Label

    '2018/02/09 saitou 広島信金(RSV2標準) ADD エントリ20明細対応 -------------------- START
    'M_SOUFURIのグローバル変数宣言をやめ、画面毎にグローバル変数を持つように変更。
    '本画面で使用しているM_SOUFURIのグローバル変数を一括置換する。
    Public strTORIS_CODE As String
    Public strTORIF_CODE As String
    Public strFURI_DATE As String
    Public strSYOKITI_KIN As String
    Public strSYOKITI_SIT As String
    Public strSYOKITI As String
    Public intCHK As Integer
    Public lngPAGE As Long
    '2018/02/09 saitou 広島信金(RSV2標準) ADD --------------------------------------- END
    'ベリファイ要否区分
    Public BERYFAI As String = 0

    'ファイル格納フォルダ
    Private EtcFolder As String
    Private DatFolder As String

    Private objNEXT_TXT As Object

    Private gstrCHK_KOUZA As String
    Private gstrCHK_DEJIT As String

    ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Friend Structure INI_PARAM
        Dim RSV2_EDITION As String                          ' RSV2機能設定
    End Structure
    Private Ini_Info As INI_PARAM

    Private TimeStamp As String
    ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

#End Region

#Region " ロード"
    Private Sub KFSENTR041_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = strTORIS_CODE & "-" & strTORIF_CODE
            LW.FuriDate = strFURI_DATE

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            MainDB = New CASTCommon.MyOracle
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label20, Label19, lblUser, lblDate)

            '取引先主コード表示
            lblTorisCode.Text = strTORIS_CODE
            '取引先副コード表示
            lblTorifCode.Text = strTORIF_CODE
            '振込日表示
            lblFuriDate.Text = strFURI_DATE.Substring(0, 4) & "/" & strFURI_DATE.Substring(4, 2) & "/" & strFURI_DATE.Substring(6, 2)
            '初期値表示
            lblSyokiti.Text = strSYOKITI

            '-------------------------------------
            'iniファイル読み込み
            '-------------------------------------
            If fn_set_INIFILE() = False Then
                Exit Sub
            End If

            '-------------------------------------
            'スケジュールマスタ検索
            '-------------------------------------
            If fn_select_SCHMAST() = False Then
                Exit Sub
            End If

            'コントロール配列を作成する
            If fn_OBJECT_SET() = False Then
                Exit Sub
            End If

            '-------------------------------------
            '画面の色初期化
            '-------------------------------------
            If fn_COLOR_CLEAR() = False Then
                MessageBox.Show(MSG0252W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '-------------------------------------
            '画面の値初期化
            '-------------------------------------
            If fn_GAMEN_CLEAR() = False Then
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '-------------------------------------
            '伝票マスタ検索
            '-------------------------------------
            If intCHK = 0 Then '正にチェックの場合
                strMAST = "S_DENPYOMAST"
            Else
                strMAST = "S_FUKU_DENPYOMAST"
            End If
            If fn_select_DENPYOMAST() = False Then
                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '-------------------------------------
            '画面の表示
            '-------------------------------------
            If lngPAGE > intMAXCNT Then     '開始ページの指定がが最大ページより大きかったら最大ページを表示
                intPAGE_CNT = intMAXCNT
            Else
                intPAGE_CNT = lngPAGE
            End If
            If fn_GAMEN_SET(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0253W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '-------------------------------------
            '前画面・次画面ボタン
            '-------------------------------------
            If intPAGE_CNT = 1 Then                 'ページ番号が１なら前画面ボタンは非アクティブ
                btnPreGamen.Enabled = False
            End If
            If intPAGE_CNT = intMAXCNT Then        'ページ番号が最大ページなら次画面ボタンは非アクティブ
                btnNextGamen.Enabled = False
            End If

            '口座チェックボタン設定
            If gstrCHK_KOUZA.ToUpper = "YES" Then
                btnKOUZACHK.Enabled = True
            Else
                btnKOUZACHK.Enabled = False
            End If

            '初期値が設定されている場合、フォーカス位置を変更
            '2013/11/29 saitou 標準版 MODIFY ---------------------------------------------->>>>
            'フォーカス位置変更が正常動作していないため修正
            If strSYOKITI_KIN <> "" Then
                If strSYOKITI_SIT <> "" Then
                    Me.ActiveControl = Me.txtKamokuCode1
                Else
                    Me.ActiveControl = Me.txtSitNo1
                End If
            End If
            'If strSYOKITI_KIN <> "" Then
            '    If strSYOKITI_SIT <> "" Then
            '        txtKamokuCode1.Focus()
            '    Else
            '        txtSitNo1.Focus()
            '    End If
            'End If
            '2013/11/29 saitou 標準版 MODIFY ----------------------------------------------<<<<

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

#Region " 口座チェック"
    Private Sub btnKOUZACHK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKOUZACHK.Click
        Try
            'ページ単位での口座チェックを実行
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座チェック)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle
            Dim Ret As Integer

            '2010/02/01 チェックを全体に変更
            '画面保存 
            If fn_GAMEN_HOZON(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0241W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            For lngPAGE_LOOP As Long = 1 To intMAXCNT

                For No As Integer = 1 To 15

                    If strKEIYAKU_KNAME(lngPAGE_LOOP, No) = Nothing Then
                        strKEIYAKU_KNAME(lngPAGE_LOOP, No) = ""
                    End If
                    If strTKIN_NO(lngPAGE_LOOP, No) = Nothing Then
                        strTKIN_NO(lngPAGE_LOOP, No) = ""
                    End If
                    If strTSIT_NO(lngPAGE_LOOP, No) = Nothing Then
                        strTSIT_NO(lngPAGE_LOOP, No) = ""
                    End If
                    If strKAMOKU(lngPAGE_LOOP, No) = Nothing Then
                        strKAMOKU(lngPAGE_LOOP, No) = ""
                    End If
                    If strKOUZA(lngPAGE_LOOP, No) = Nothing Then
                        strKOUZA(lngPAGE_LOOP, No) = ""
                    End If
                    strKDB_KNAME = strKEIYAKU_KNAME(lngPAGE_LOOP, No).Trim    '契約者名退避

                    '空白行は無視
                    If strTKIN_NO(lngPAGE_LOOP, No).Trim = "" AndAlso strTSIT_NO(lngPAGE_LOOP, No).Trim = "" AndAlso strKAMOKU(lngPAGE_LOOP, No).Trim = "" _
                       AndAlso strKOUZA(lngPAGE_LOOP, No).Trim = "" AndAlso strKEIYAKU_KNAME(lngPAGE_LOOP, No).Trim = "" Then
                        strBIKOU(lngPAGE_LOOP, No) = ""
                        '2014/05/01 saitou 標準版修正 MODIFY ----------------------------------------------->>>>
                        '比較する配列間違い
                    ElseIf strTKIN_NO(lngPAGE_LOOP, No).Trim = strJIKINKO_CD AndAlso gstrCHK_KOUZA = "YES" Then
                        'ElseIf txtKinNo(No).Text.Trim = strJIKINKO_CD AndAlso gstrCHK_KOUZA = "YES" Then
                        '2014/05/01 saitou 標準版修正 MODIFY -----------------------------------------------<<<<
                        '口座チェックを実行
                        '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------START
                        Ret = GCom.KouzaChk_ENTRY(strTSIT_NO(lngPAGE_LOOP, No).Trim, CASTCommon.ConvertKamoku1TO2_K(strKAMOKU(lngPAGE_LOOP, No).Trim), _
                                               strKOUZA(lngPAGE_LOOP, No).Trim, strKIGYO_CODE, _
                                               strFURI_CODE, strKDB_KNAME, strERR_MSG, MainDB)
                        'Ret = GCom.KouzaChk_ENTRY(strTSIT_NO(lngPAGE_LOOP, No).Trim, CASTCommon.ConvertKamoku1TO2(strKAMOKU(lngPAGE_LOOP, No).Trim), _
                        '                          strKOUZA(lngPAGE_LOOP, No).Trim, strKIGYO_CODE, _
                        '                          strFURI_CODE, strKDB_KNAME, strERR_MSG, MainDB)
                        '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------END
                        Select Case Ret
                            Case -1 '口座チェック失敗
                                strBIKOU(lngPAGE_LOOP, No) = "チェック失敗"

                            Case Else
                                strBIKOU(lngPAGE_LOOP, No) = strERR_MSG
                                If intCHK = 0 Then       '正にチェックの場合 条件移動 

                                    '2011/06/28 標準版修正 自振契約なしは正常扱い ------------------START
                                    If Ret = 0 OrElse Ret = 2 Then
                                        strBIKOU(lngPAGE_LOOP, No) = ""
                                        strERR_COLOR = Color.White
                                        clrTKIN_NO_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrTKIN_NAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrTSIT_NO_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrTSIT_NAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrKAMOKU_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrKOUZA_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrKEIYAKU_NNAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrFURIKIN_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    Else
                                        strBIKOU(lngPAGE_LOOP, No) = strERR_MSG
                                        strERR_COLOR = Color.Yellow
                                        clrTKIN_NO_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrTKIN_NAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrTSIT_NO_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrTSIT_NAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrKAMOKU_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrKOUZA_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrKEIYAKU_NNAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                        clrFURIKIN_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    End If
                                    'If strERR_MSG <> "" Then
                                    '    strBIKOU(lngPAGE_LOOP, No) = strERR_MSG
                                    '    strERR_COLOR = Color.Yellow
                                    '    clrTKIN_NO_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrTKIN_NAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrTSIT_NO_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrTSIT_NAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrKAMOKU_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrKOUZA_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrKEIYAKU_NNAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrFURIKIN_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    'Else
                                    '    strBIKOU(lngPAGE_LOOP, No) = ""
                                    '    strERR_COLOR = Color.White
                                    '    clrTKIN_NO_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrTKIN_NAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrTSIT_NO_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrTSIT_NAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrKAMOKU_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrKOUZA_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrKEIYAKU_NNAME_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    '    clrFURIKIN_Color(lngPAGE_LOOP, No) = strERR_COLOR
                                    'End If
                                    '2011/06/28 標準版修正 自振契約なしは正常扱い ------------------END
                                End If
                                '2011/06/16 標準版修正 活口座チェック追加 ------------------START
                                'If (Ret = 0 OrElse Ret = 2) AndAlso strKEIYAKU_KNAME(lngPAGE_LOOP, No).Trim = "" Then   '口座無し以外の場合は契約者カナ名設定
                                If Ret <> 1 AndAlso strKEIYAKU_KNAME(lngPAGE_LOOP, No).Trim = "" Then   '口座無し以外の場合は契約者カナ名設定
                                    strKEIYAKU_KNAME(lngPAGE_LOOP, No) = GCom.NzStr(strKDB_KNAME.Trim)
                                End If
                                '2011/06/16 標準版修正 活口座チェック追加 ------------------END
                        End Select
                    Else
                        ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                        strBIKOU(lngPAGE_LOOP, No) = ""
                        'lblBikou(No).Text = ""
                        ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                    End If
                Next
            Next

            If fn_GAMEN_SET(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0253W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If fn_COLOR_SET(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0265W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座チェック)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座チェック)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " 挿入"
    '*******************************************
    '2009/10/21指定行に空白の行を挿入する
    '*******************************************
    Private Sub btnGyoSounyu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGyoSounyu.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(挿入)終了", "成功", "")

            If txtSounyuKoban.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "項番"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSounyuKoban.Focus()
                Return
            End If

            If (intMAXCNT - 1) * 15 + intMAXLINE < CDbl(txtSounyuKoban.Text) Then '指定項番チェック
                MessageBox.Show(MSG0343W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSounyuKoban.Focus()
                Return
            End If

            '現在のページにのみ行を挿入できるようにする
            Dim page As Integer
            If GCom.NzLong(txtSounyuKoban.Text.Trim) Mod 15 = 0 Then
                page = Math.Floor(GCom.NzLong(txtSounyuKoban.Text.Trim) / 15)
            Else
                page = Math.Floor(GCom.NzLong(txtSounyuKoban.Text.Trim) / 15) + 1
            End If
            If intPAGE_CNT <> page OrElse GCom.NzLong(txtSounyuKoban.Text.Trim) = 0 Then
                MessageBox.Show(MSG0347W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSounyuKoban.Focus()
                Return
            End If
            If fn_GAMEN_HOZON(intPAGE_CNT) = False Then '今のページの情報を配列に保存
                MessageBox.Show(MSG0241W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            If fn_COLOR_HOZON(intPAGE_CNT) = False Then '今のページの色を配列に保存
                MessageBox.Show(MSG0254W, msgTitle _
                                          , MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            If fn_GYO_SOUNYU() = False Then  '空白行の挿入処理
                Return
            End If
            If fn_GAMEN_SET(intPAGE_CNT) = False Then    '画面に情報をセット
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            If fn_COLOR_SET(intPAGE_CNT) = False Then '画面の色情報をセット
                MessageBox.Show(MSG0252W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim No As Integer
            No = GCom.NzLong(txtSounyuKoban.Text.Trim) Mod 15
            If No = 0 Then No = 15
            If (strSYOKITI_KIN + strSYOKITI_SIT).Trim = "" Then
                txtFurikin(No).Text = GCom.NzLong(strSYOKITI).ToString("#,###")
                txtKinNo(No).Focus()
            ElseIf strSYOKITI_SIT.Trim = "" Then
                txtKinNo(No).Text = strSYOKITI_KIN
                txtKinName(No).Text = GCom.GetBKBRName(txtKinNo(No).Text, "", 15)
                txtFurikin(No).Text = GCom.NzLong(strSYOKITI).ToString("#,###")
                txtSitNo(No).Focus()
            Else
                txtKinNo(No).Text = strSYOKITI_KIN
                txtSitNo(No).Text = strSYOKITI_SIT
                txtKinName(No).Text = GCom.GetBKBRName(txtKinNo(No).Text, "", 15)
                txtSitName(No).Text = GCom.GetBKBRName(txtKinNo(No).Text, txtSitNo(No).Text, 15)
                txtFurikin(No).Text = GCom.NzLong(strSYOKITI).ToString("#,###")
                txtKamokuCode(No).Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")
        End Try

    End Sub

#End Region

#Region " 登録"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '============================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :画面の値・色を保存し、全データを伝票マスタ/伝票副マスタに更新・インサートし、
        '               :スケジュールを更新する
        'Return         :
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle
            '-----------------------------------------
            '今のページの値を配列に保存
            '-----------------------------------------
            If fn_GAMEN_HOZON(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0241W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '-----------------------------------------
            '今のページの色を配列に保存
            '-----------------------------------------
            If fn_COLOR_HOZON(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0254W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '-----------------------------------------
            '小計の計算
            '-----------------------------------------
            If txtSyoukeiKin.Text = "" Then
            Else
                If Val(fn_DEL_KANMA(txtSyoukeiKin.Text)) <> fn_COUNT_SYOUKEI_KIN() Then

                    MessageBox.Show(String.Format(MSG0129W, txtSyoukeiKin.Text, Format(fn_COUNT_SYOUKEI_KIN(), "#,##0")), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSyoukeiKin.Focus()
                    Exit Sub
                End If
            End If

            '-----------------------------------------
            '合計の計算
            '-----------------------------------------
            Dim dblGOUKEI_KIN As Double
            If txtGoukeiKin.Text.Length = 0 Then
                MessageBox.Show(MSG0249W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGoukeiKin.Focus()
                Exit Sub
            Else
                dblGOUKEI_KIN = fn_COUNT_GOUKEI_KIN()
                If Val(fn_DEL_KANMA(txtGoukeiKin.Text)) <> dblGOUKEI_KIN Then
                    MessageBox.Show(String.Format(MSG0130W, txtGoukeiKin.Text, Format(dblGOUKEI_KIN, "#,##0")), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGoukeiKin.Focus()
                    Exit Sub
                End If
            End If

            '副の場合、正のマスタと合計件数･金額の一致チェックを実行
            If intCHK = 1 Then
                Dim Count As Long
                Dim Kingaku As Long
                If fn_CLC_DENPYOMAST(Count, Kingaku) = False Then
                    '合計件数取得失敗
                    MessageBox.Show(MSG0245W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
                Call fn_COUNT_GOUKEI_KEN()
                If GCom.NzLong(fn_DEL_KANMA(lblGoukeiKen.Text)) <> Count Then
                    '合計件数不一致
                    MessageBox.Show(String.Format(MSG0242W, Count.ToString("#,##0"), GCom.NzLong(fn_DEL_KANMA(lblGoukeiKen.Text)).ToString("#,##0")), _
                                 msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
                If dblGOUKEI_KIN <> Kingaku Then
                    '合計金額不一致
                    MessageBox.Show(String.Format(MSG0243W, Kingaku.ToString("#,##0"), dblGOUKEI_KIN.ToString("#,##0")), _
                                  msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            End If

            '伝票・スケジュールマスタ更新
            If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If

            '-----------------------------------------
            '伝票マスタ/伝票副マスタにインサート・更新する
            '-----------------------------------------
            MainDB.BeginTrans()
            If fn_UPDATE_DENPYOMAST() = False Then
                MainDB.Rollback()
                Exit Sub
            End If

            If intMAXCNT = intPAGE_CNT And intMAXLINE = 15 Then
                intMAXCNT += 1
                intMAXLINE = 0
                btnNextGamen.Enabled = True
            End If
            '-----------------------------------------
            '正の場合のみスケジュールを更新する>ベリファイの要否で更新内容を変更する
            '-----------------------------------------
            'If intCHK = 0 Then '正にチェックの場合
            Dim UKETUKE As String = "0"
            If fn_UPDATE_SCHMAST(UKETUKE) = False Then
                Exit Sub
            End If
            MainDB.Commit()
            'ベリファイ無しの場合・ベリファイありかつ副入力完了の場合データファイルを作成する
            Select Case UKETUKE
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
            btnEnd.Focus()
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
    Private Sub btnTouroku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHozon.Click
        '============================================================================
        'NAME           :btnNextGamen_Click
        'Parameter      :
        'Description    :画面の値・色を保存し、全データを伝票マスタ/伝票副マスタに更新・インサートする
        'Return         :
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(保存)開始", "成功", "")
            MainDB = New CASTCommon.MyOracle
            '-----------------------------------------
            '今のページの値を配列に保存
            '-----------------------------------------
            If fn_GAMEN_HOZON(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0241W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '-----------------------------------------
            '今のページの色を配列に保存
            '-----------------------------------------
            If fn_COLOR_HOZON(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0254W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '-----------------------------------------
            '小計の計算
            '-----------------------------------------
            If txtSyoukeiKin.Text = "" Then
            Else
                If Val(fn_DEL_KANMA(txtSyoukeiKin.Text)) <> fn_COUNT_SYOUKEI_KIN() Then
                    MessageBox.Show(String.Format(MSG0129W, txtSyoukeiKin.Text, Format(fn_COUNT_SYOUKEI_KIN(), "#,##0")), _
                                  msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtSyoukeiKin.SelectionLength = txtSyoukeiKin.TextLength
                    txtSyoukeiKin.Focus()
                    Exit Sub
                End If
            End If

            '-----------------------------------------
            '合計の計算
            '-----------------------------------------
            Dim dblGOUKEI_KIN As Double
            If txtGoukeiKin.Text.Length = 0 Then
            Else
                dblGOUKEI_KIN = fn_COUNT_GOUKEI_KIN()
                If Val(fn_DEL_KANMA(txtGoukeiKin.Text)) <> dblGOUKEI_KIN Then
                    '合計金額不一致
                    MessageBox.Show(String.Format(MSG0130W, txtGoukeiKin.Text, dblGOUKEI_KIN.ToString("#,##0")), _
                                  msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGoukeiKin.SelectionLength = txtGoukeiKin.TextLength
                    txtGoukeiKin.Focus()
                    Exit Sub
                End If
            End If

            '伝票マスタ更新
            If MessageBox.Show(String.Format(MSG0015I, "保存"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If

            '-----------------------------------------
            '伝票マスタ/伝票副マスタにインサート・更新する
            '-----------------------------------------
            MainDB.BeginTrans()
            If fn_UPDATE_DENPYOMAST() = False Then
                MainDB.Rollback()
                Exit Sub
            End If
            MainDB.Commit()
            If intMAXCNT = intPAGE_CNT And intMAXLINE = 15 Then
                intMAXCNT += 1
                intMAXLINE = 0
                btnNextGamen.Enabled = True
            End If
            MessageBox.Show(String.Format(MSG0016I, "保存"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            If btnNextGamen.Enabled = False Then
                btnAction.Focus()
            Else
                btnNextGamen.Focus()     'ﾌｫｰｶｽ移動
            End If
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
        '============================================================================
        'NAME           :btnPreGamen_Click
        'Parameter      :
        'Description    :今の画面の色・値を配列に保存し、前画面を表示する
        'Return         :
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(前画面)開始", "成功", "")
            '-----------------------------------------
            '今のページの値を配列に保存
            '-----------------------------------------
            If fn_GAMEN_HOZON(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0241W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '-----------------------------------------
            '今のページの色を配列に保存
            '-----------------------------------------
            If fn_COLOR_HOZON(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0254W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '-------------------------------------------------
            '小計のカウント
            '-------------------------------------------------
            If fn_COUNT_SYOUKEI_KEN() = False Then
                MessageBox.Show(MSG0263W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            lngSYOUKEI_KEN(intPAGE_CNT) = lblSyoukeiKen.Text

            '-------------------------------------
            '画面の色初期化
            '-------------------------------------
            If fn_COLOR_CLEAR() = False Then
                MessageBox.Show(MSG0252W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '-------------------------------------
            '画面の値初期化
            '-------------------------------------
            If fn_GAMEN_CLEAR() = False Then
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If intPAGE_CNT = 1 Then
                MessageBox.Show(MSG0264W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                '-----------------------------------------
                '前ページの値を画面に表示
                '-----------------------------------------
                intPAGE_CNT -= 1
                If fn_GAMEN_SET(intPAGE_CNT) = False Then

                    MessageBox.Show(MSG0253W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                '-----------------------------------------
                '前ページの色を画面に表示
                '-----------------------------------------
                If fn_COLOR_SET(intPAGE_CNT) = False Then
                    MessageBox.Show(MSG0265W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
                If intPAGE_CNT = 1 Then 'ページ番号が１なら前画面ボタンは非アクティブ
                    btnPreGamen.Enabled = False
                End If
            End If
            btnNextGamen.Enabled = True

            If strSYOKITI_KIN <> "" Then
                If strSYOKITI_SIT <> "" Then
                    txtKamokuCode1.Focus()
                Else
                    txtSitNo1.Focus()
                End If
            Else
                txtKinNO1.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(前画面)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(前画面)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region " 次画面ボタン"
    Private Sub btnNextGamen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNextGamen.Click
        '============================================================================
        'NAME           :btnNextGamen_Click
        'Parameter      :
        'Description    :今の画面の色・値を配列に保存し、次画面を表示する
        'Return         :
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(次画面)開始", "成功", "")
            '-----------------------------------------
            '今のページの値を配列に保存
            '-----------------------------------------
            If fn_GAMEN_HOZON(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0241W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '-----------------------------------------
            '今のページの色を配列に保存
            '-----------------------------------------
            If fn_COLOR_HOZON(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0254W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '-------------------------------------------------
            '小計のカウント
            '-------------------------------------------------
            If fn_COUNT_SYOUKEI_KEN() = False Then
                MessageBox.Show(MSG0263W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            lngSYOUKEI_KEN(intPAGE_CNT) = lblSyoukeiKen.Text

            '-------------------------------------
            '画面の色初期化
            '-------------------------------------
            If fn_COLOR_CLEAR() = False Then
                MessageBox.Show(MSG0252W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '-------------------------------------
            '画面の値初期化
            '-------------------------------------
            If fn_GAMEN_CLEAR() = False Then
                MessageBox.Show(MSG0234W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            intPAGE_CNT += 1
            If intPAGE_CNT <= intMAXCNT Then
                '-----------------------------------------
                '次ページの値を画面に表示
                '-----------------------------------------
                If fn_GAMEN_SET(intPAGE_CNT) = False Then
                    MessageBox.Show(MSG0266W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                '-----------------------------------------
                '次ページの色を画面に表示
                '-----------------------------------------
                If fn_COLOR_SET(intPAGE_CNT) = False Then
                    MessageBox.Show(MSG0265W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If
            Else
                intMAXCNT += 1
                btnNextGamen.Enabled = False  'ページ番号が最大ページなら次画面ボタンは非アクティブ
            End If
            btnPreGamen.Enabled = True

            If strSYOKITI_KIN <> "" Then
                If strSYOKITI_SIT <> "" Then
                    txtKamokuCode1.Focus()
                Else
                    txtSitNo1.Focus()
                End If
            Else
                txtKinNO1.Focus()
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " 関数"
    Function fn_select_SCHMAST() As Boolean
        '============================================================================
        'NAME           :fn_select_SCHMAST
        'Parameter      :
        'Description    :S_SCHMAST・S_TORIMASTから取引先情報取得
        'Return         :True=OK,False=NG
        'Create         :2009/09/29
        'Update         :
        '============================================================================

        fn_select_SCHMAST = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            SQL.Append("SELECT * ")
            SQL.Append(" FROM S_TORIMAST,S_SCHMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='3'")
            SQL.Append(" AND TORIS_CODE_T = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            SQL.Append(" AND FURI_DATE_S = " & SQ(strFURI_DATE))
            SQL.Append(" AND TYUUDAN_FLG_S = '0' ")

            Dim TOUROKU_FLG As String
            If OraReader.DataReader(SQL) = True Then
                TOUROKU_FLG = GCom.NzStr(OraReader.Reader.Item("TOUROKU_FLG_S"))
                strFURI_CODE = GCom.NzStr(OraReader.Reader.Item("FURI_CODE_T"))
                strKIGYO_CODE = GCom.NzStr(OraReader.Reader.Item("KIGYO_CODE_T"))
                lblToriNName.Text = GCom.NzStr(OraReader.Reader.Item("ITAKU_NNAME_T"))
                OraReader.Close()
            Else
                'スケジュールなし
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                OraReader.Close()
                Return False
            End If

            If TOUROKU_FLG = "1" Then
                '登録済み
                MessageBox.Show(MSG0061W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            fn_select_SCHMAST = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュールマスタ検索)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_select_DENPYOMAST() As Boolean
        '============================================================================
        'NAME           :fn_select_DENPYOMAST
        'Parameter      :
        'Description    :伝票マスタ OR 伝票副マスタから情報抽出
        'Return         :True=OK,False=NG
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            Dim strKIN_CODE As String = ""
            Dim strSIT_CODE As String = ""

            fn_select_DENPYOMAST = False
            SQL.Append("SELECT * FROM " & strMAST)
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            SQL.Append(" ORDER BY HYOUJI_SEQ_E ASC")

            lngPAGE_CNT = 0
            lngGYO_CNT = 0
            lngKAIYAKU_CNT = 0

            L = 1
            P = 1

            If OraReader.DataReader(SQL) = True Then
                Do
                    lngRECORD_NO(P, L) = OraReader.GetInt("KEIYAKU_NO_E")
                    strDEL_CHK(P, L) = GCom.NzStr(OraReader.GetString("KAIYAKU_E"))
                    strTKIN_NO(P, L) = GCom.NzStr(OraReader.GetString("TKIN_NO_E"))
                    strTSIT_NO(P, L) = GCom.NzStr(OraReader.GetString("TSIT_NO_E"))
                    clrTKIN_NAME_Color(P, L) = Color.White
                    clrTSIT_NAME_Color(P, L) = Color.White
                    If fn_Select_TENMAST(strTKIN_NO(P, L), strTSIT_NO(P, L), strKIN_NNM, strSIT_NNM, strKIN_KNM, strSIT_KNM) = False Then
                        strTKIN_NAME(P, L) = ""
                        strTSIT_NAME(P, L) = ""
                    Else
                    End If
                    strTKIN_NAME(P, L) = strKIN_NNM
                    strTSIT_NAME(P, L) = strSIT_NNM
                    If GCom.NzStr(OraReader.GetString("KAMOKU_E")) = "" Then
                        strKAMOKU(P, L) = ""
                    Else
                        '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------START
                        strKAMOKU(P, L) = CASTCommon.ConvertKamoku2TO1_K(GCom.NzStr(OraReader.GetString("KAMOKU_E")))
                        'strKAMOKU(P, L) = CASTCommon.ConvertKamoku2TO1(GCom.NzStr(OraReader.GetString("KAMOKU_E")))
                        '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------END
                    End If
                    strKOUZA(P, L) = GCom.NzStr(OraReader.GetString("KOUZA_E"))
                    GCom.NzStr(OraReader.GetString("KEIYAKU_KNAME_E"))
                    strKEIYAKU_KNAME(P, L) = GCom.NzStr(OraReader.GetString("KEIYAKU_KNAME_E"))
                    lngFURIKIN(P, L) = OraReader.GetInt64("FURIKIN_E")
                    If lngFURIKIN(P, L) <> 0 Then
                        lngSYOUKEI_KEN(P) += 1
                    End If
                    strBIKOU(P, L) = GCom.NzStr(OraReader.GetString("ERR_MSG_E")) '登録されているERR_MSGをセット

                    intENT_COUNT += 1
                    If L >= 15 Then
                        L = 1
                        P = P + 1
                        lngSYOUKEI_KEN(P) = 0
                    Else
                        L = L + 1
                    End If

                Loop Until OraReader.NextRead = False
            End If


            intMAXCNT = P
            intMAXLINE = L - 1

            intPAGE_CNT = 1
            fn_select_DENPYOMAST = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(伝票マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function

    Function fn_GAMEN_SET(ByVal intPAGE_NO As Integer) As Boolean
        '============================================================================
        'NAME           :fn_GAMEN_SET
        'Parameter      :intPAGE_NO：ページ番号
        'Description    :intPAGE_NOページの画面を表示する
        'Return         :True=OK,False=NG
        'Create         :2004/09/10
        'Update         :
        '============================================================================
        fn_GAMEN_SET = False
        Try
            P = intPAGE_NO
            Dim L_END As Integer
            If P = intMAXCNT Then
                L_END = intMAXLINE
            Else
                L_END = 15
            End If
            For L = 1 To L_END
                If strDEL_CHK(P, L) = "1" Then
                    ckbDel(L).Checked = True
                Else
                    ckbDel(L).Checked = False
                End If
                lblIndex(L).Text = L + ((P - 1) * 15)
                txtKinNo(L).Text = strTKIN_NO(P, L).Trim
                txtKinName(L).Text = strTKIN_NAME(P, L).Trim
                txtSitNo(L).Text = strTSIT_NO(P, L).Trim
                txtSitName(L).Text = strTSIT_NAME(P, L).Trim
                txtKamokuCode(L).Text = strKAMOKU(P, L).Trim
                Select Case txtKamokuCode(L).Text
                    Case "1"
                        lblKamoku(L).Text = "普"
                    Case "2"
                        lblKamoku(L).Text = "当"
                    Case "3"
                        lblKamoku(L).Text = "納"
                    Case "4"
                        lblKamoku(L).Text = "職"
                    Case "9"
                        lblKamoku(L).Text = "他"
                    Case ""
                        lblKamoku(L).Text = "  "
                    Case Else
                        lblKamoku(L).Text = "他"
                End Select
                txtKouza(L).Text = strKOUZA(P, L).Trim
                txtKeiyakuName(L).Text = strKEIYAKU_KNAME(P, L).Trim
                'txtFurikin(L).Text = lngFURIKIN(P, L)
                txtFurikin(L).Text = lngFURIKIN(P, L).ToString("#,##0")
                lblBikou(L).Text = strBIKOU(P, L).Trim
                If intMAXCNT = 1 Then
                    If L = intMAXLINE Then
                        Exit For
                    End If
                End If
            Next
            lblPage.Text = Format(intPAGE_NO, "00") & "/" & Format(intMAXCNT, "00")

            'MAXページだったら次へボタン非アクティブ
            If intPAGE_CNT = intMAXCNT Then        'ページ番号が最大ページなら次画面ボタンは非アクティブ
                btnNextGamen.Enabled = False
            End If

            '-------------------------------------------------
            '小計のカウント
            '-------------------------------------------------
            If fn_COUNT_SYOUKEI_KEN() = False Then
                MessageBox.Show(MSG0263W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            '--------------------------------------------------
            '合計のカウント
            '--------------------------------------------------
            If fn_COUNT_GOUKEI_KEN() = False Then
                MessageBox.Show(MSG0267W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            If intPAGE_NO <> intMAXCNT Then
                txtGoukeiKin.Enabled = False
                btnAction.Enabled = False
            Else
                txtGoukeiKin.Enabled = True
                btnAction.Enabled = True
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(画面セット)", "失敗", ex.ToString)
            Return False
        End Try
        fn_GAMEN_SET = True
    End Function

    Function fn_GAMEN_HOZON(ByVal intPAGE_NO As Integer) As Boolean
        '============================================================================
        'NAME           :fn_GAMEN_HOZON
        'Parameter      :intPAGE_NO：ページ番号
        'Description    :intPAGE_NOページの画面の値を配列に保存する
        'Return         :True=OK,False=NG
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_GAMEN_HOZON = False
        Try
            P = intPAGE_NO
            Dim L_END As Integer
            If P = intMAXCNT Then
                L_END = intMAXLINE
            Else
                L_END = 15
            End If
            For L = 1 To L_END
                If ckbDel(L).Checked = True Then
                    strDEL_CHK(P, L) = "1"
                Else
                    strDEL_CHK(P, L) = "0"
                End If
                strTKIN_NO(P, L) = txtKinNo(L).Text
                strTSIT_NO(P, L) = txtSitNo(L).Text
                strTKIN_NAME(P, L) = txtKinName(L).Text
                strTSIT_NAME(P, L) = txtSitName(L).Text
                strKAMOKU(P, L) = txtKamokuCode(L).Text
                strKOUZA(P, L) = txtKouza(L).Text
                strKEIYAKU_KNAME(P, L) = txtKeiyakuName(L).Text
                If txtFurikin(L).Text.Trim = "" Then
                    lngFURIKIN(P, L) = 0
                Else
                    lngFURIKIN(P, L) = fn_DEL_KANMA(txtFurikin(L).Text)
                End If
                strBIKOU(P, L) = lblBikou(L).Text
                lngRECORD_NO(P, L) = L + ((P - 1) * 15)
                If intMAXCNT = 1 Then
                    If L = intMAXLINE Then
                        Exit For
                    End If
                End If
            Next
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(画面保存)", "失敗", ex.ToString)
            Return False
        End Try
        fn_GAMEN_HOZON = True
    End Function

    Function fn_GAMEN_CLEAR() As Boolean
        '============================================================================
        'NAME           :fn_GAMEN_CLEAR
        'Parameter      :
        'Description    :画面をクリアにする
        'Return         :True=OK,False=NG
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_GAMEN_CLEAR = False
        Try
            For L = 1 To 15
                ckbDel(L).Checked = False
                lblIndex(L).Text = ""
                txtKinNo(L).Text = ""
                txtKinName(L).Text = ""
                txtSitNo(L).Text = ""
                txtSitName(L).Text = ""
                txtKamokuCode(L).Text = ""
                lblKamoku(L).Text = ""
                txtKouza(L).Text = ""
                txtKeiyakuName(L).Text = ""
                txtFurikin(L).Text = ""
                lblBikou(L).Text = ""
            Next
            '小計初期化
            txtSyoukeiKin.Text = ""
            lblSyoukeiTesuu.Text = ""
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(画面クリア)", "失敗", ex.ToString)
            Return False
        End Try
        fn_GAMEN_CLEAR = True
    End Function

    Function fn_COLOR_CLEAR() As Boolean
        '============================================================================
        'NAME           :fn_COLOR_CLEAR
        'Parameter      :
        'Description    :画面の色をクリアにする
        'Return         :True=OK,False=NG
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_COLOR_CLEAR = False
        Try
            For L = 1 To 15
                txtKinNo(L).BackColor = Color.White
                txtKinName(L).BackColor = Color.White
                txtSitNo(L).BackColor = Color.White
                txtSitName(L).BackColor = Color.White
                txtKamokuCode(L).BackColor = Color.White
                txtKouza(L).BackColor = Color.White
                txtKeiyakuName(L).BackColor = Color.White
                txtFurikin(L).BackColor = Color.White
            Next
            fn_COLOR_CLEAR = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(画面色設定)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_COLOR_SET(ByVal intPAGE_NO As Integer) As Boolean
        '============================================================================
        'NAME           :fn_COLOR_SET
        'Parameter      :intPAGE_NO：ページ番号
        'Description    :intPAGE_NOページの画面の色を表示する
        'Return         :True=OK,False=NG
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_COLOR_SET = False
        Try
            P = intPAGE_NO
            Dim L_END As Integer
            If P = intMAXCNT Then
                L_END = intMAXLINE
            Else
                L_END = 15
            End If
            For L = 1 To L_END
                txtKinNo(L).BackColor = clrTKIN_NO_Color(P, L)
                txtKinName(L).BackColor = clrTKIN_NAME_Color(P, L)
                txtSitNo(L).BackColor = clrTSIT_NO_Color(P, L)
                txtSitName(L).BackColor = clrTSIT_NAME_Color(P, L)
                txtKamokuCode(L).BackColor = clrKAMOKU_Color(P, L)
                txtKouza(L).BackColor = clrKOUZA_Color(P, L)
                txtKeiyakuName(L).BackColor = clrKEIYAKU_NNAME_Color(P, L)
                txtFurikin(L).BackColor = clrFURIKIN_Color(P, L)

                If intMAXCNT = 1 Then
                    If L = intMAXLINE Then
                        Exit For
                    End If
                End If
            Next
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(画面色変更)", "失敗", ex.ToString)
            Return False
        End Try
        fn_COLOR_SET = True
    End Function

    Function fn_COLOR_HOZON(ByVal intPAGE_NO As Integer) As Boolean
        '============================================================================
        'NAME           :fn_COLOR_HOZON
        'Parameter      :intPAGE_NO：ページ番号
        'Description    :intPAGE_NOページの画面の色を配列に保存する
        'Return         :True=OK,False=NG
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_COLOR_HOZON = False
        Try
            P = intPAGE_NO
            Dim L_END As Integer
            If P = intMAXCNT Then
                L_END = intMAXLINE
            Else
                L_END = 15
            End If
            For L = 1 To L_END
                clrTKIN_NO_Color(P, L) = txtKinNo(L).BackColor
                clrTKIN_NAME_Color(P, L) = txtKinName(L).BackColor
                clrTSIT_NO_Color(P, L) = txtSitNo(L).BackColor
                clrTSIT_NAME_Color(P, L) = txtSitName(L).BackColor
                clrKAMOKU_Color(P, L) = txtKamokuCode(L).BackColor
                clrKOUZA_Color(P, L) = txtKouza(L).BackColor
                clrKEIYAKU_NNAME_Color(P, L) = txtKeiyakuName(L).BackColor
                clrFURIKIN_Color(P, L) = txtFurikin(L).BackColor

                If intMAXCNT = 1 Then
                    If L = intMAXLINE Then
                        Exit For
                    End If
                End If
            Next
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(画面色保存)", "失敗", ex.ToString)
            Return False
        End Try
        fn_COLOR_HOZON = True
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
            strJIKINKO_CD = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If strJIKINKO_CD.ToUpper = "ERR" OrElse strJIKINKO_CD = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                Return False
            End If

            'チェックデジット
            gstrCHK_DEJIT = CASTCommon.GetFSKJIni("COMMON", "CHKDEJIT")
            If gstrCHK_DEJIT.ToUpper = "ERR" OrElse gstrCHK_DEJIT = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "チェックデジット判定", "COMMON", "CHKDEJIT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:チェックデジット判定 分類:COMMON 項目:CHKDEJIT")
                Return False
            End If

            '口座チェック判定
            gstrCHK_KOUZA = CASTCommon.GetFSKJIni("COMMON", "KOUZACHK")
            If gstrCHK_KOUZA.ToUpper = "ERR" OrElse gstrCHK_KOUZA = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "口座チェック判定", "COMMON", "KOUZACHK"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    Function fn_CHECK_FURIKIN(ByVal aobjKIN_CODE As TextBox, ByVal aobjKIN_NAME As TextBox, _
                             ByVal aobjSIT_CODE As TextBox, ByVal aobjSIT_NAME As TextBox, _
                             ByVal aobjKAMOKU As TextBox, ByVal aobjKOUZA As TextBox, ByVal aobjKEIYAKU_NAME As TextBox, _
                             ByVal aobjFURIKIN As TextBox, ByVal aobjERR As Label, ByVal aobjNEXT As Object, _
                             ByVal aobjKOUBAN As Label, ByVal aintPAGE As Integer, ByVal aintGYO As Integer) As Boolean
        '============================================================================
        'NAME           :fn_CHECK_FURIKIN
        'Parameter      :aobjKIN_CODE：金融機関コード／aobjKIN_NAME：金融機関名
        '               :aobjSIT_CODE：支店コード／aobjSIT_NAME：支店名／aobjKAMOKU：科目
        '               :aobjKOUZA：口座／aobjKEIYAKU_NAME：契約者名
        '               :aobjFURIKIN：振込金額／aobjERR：エラー内容／aobjNEXT：オブジェクト
        '               :aobjKOUBAN：項番／aintPAGE：ページ番号／aintGYO：行番号
        'Description    :振込金額エンターキー押下時のチェック処理
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/30
        'Update         :
        '============================================================================
        fn_CHECK_FURIKIN = False
        Try
            MainDB = New CASTCommon.MyOracle
            '---------------------------------------------------
            '必須項目の入力チェック（科目コード、口座番号）
            '---------------------------------------------------
            '科目コード
            If aobjKAMOKU.Text = "" Then
                MessageBox.Show(MSG0136W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                aobjKAMOKU.Focus()
                Exit Function
            End If
            '口座番号
            If aobjKOUZA.Text = "" Then
                MessageBox.Show(MSG0138W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                aobjKOUZA.Focus()
                Exit Function
            End If

            '-------------------------------------------------
            '初期値のセット（金融機関コード,支店コード、振込金額）
            '-------------------------------------------------
            If aobjKIN_CODE.Text = "" Then
                aobjKIN_CODE.Text = strSYOKITI_KIN
            End If
            If aobjSIT_CODE.Text = "" Then
                aobjSIT_CODE.Text = strSYOKITI_SIT
            End If
            If aobjFURIKIN.Text = "" Then
                aobjFURIKIN.Text = GCom.NzLong(strSYOKITI).ToString("#,###")
            End If

            '金融機関入力チェック
            If aobjKIN_CODE.Text = "" Then
                MessageBox.Show(MSG0032W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                aobjKIN_CODE.Focus()
                Exit Function
            End If
            '支店入力チェック
            If aobjSIT_CODE.Text = "" Then
                MessageBox.Show(MSG0035W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                aobjSIT_CODE.Focus()
                Exit Function
            End If

            '--------------
            '金融機関名表示
            '--------------
            '2011/06/28 標準版修正 金融機関名表示桁数変更 ------------------START
            aobjKIN_NAME.Text = GCom.GetBKBRName(aobjKIN_CODE.Text, "", 14)
            aobjSIT_NAME.Text = GCom.GetBKBRName(aobjKIN_CODE.Text, aobjSIT_CODE.Text, 14)
            'aobjKIN_NAME.Text = GCom.GetBKBRName(aobjKIN_CODE.Text, "", 12)
            'aobjSIT_NAME.Text = GCom.GetBKBRName(aobjKIN_CODE.Text, aobjSIT_CODE.Text, 12)
            '2011/06/28 標準版修正 金融機関名表示桁数変更 ------------------END
            If aobjKIN_NAME.Text.Trim = "" OrElse aobjSIT_NAME.Text.Trim = "" Then
                '金融機関無し
                MessageBox.Show(MSG0119W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                aobjKIN_CODE.Focus()
                Exit Function
            End If

            '-------------------------------------------------
            '振込金額の入力値チェック
            '-------------------------------------------------
            'Dim strKINGAKU As String = ""
            '振込金額入力チェック
            If aobjFURIKIN.Text = "" Then
                MessageBox.Show(MSG0140W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                aobjFURIKIN.Focus()
                Exit Function
            End If
            ' aobjFURIKIN.Text = strKINGAKU

            '-------------------------------------------------
            'チェックデジット(自金庫、チェックデジット対象のみ)
            '-------------------------------------------------
            If intCHK = 0 Then       '正にチェックの場合
                If aobjKIN_CODE.Text = strJIKINKO_CD And gstrCHK_DEJIT = "YES" Then
                    '2013/10/24 saitou 標準修正 UPD -------------------------------------------------->>>>
                    'チェックデジット関数をMenteCommonを、科目変換はCastCommonを使用するように修正
                    If GCom.ChkDejit_ENTRY(aobjKIN_CODE.Text, aobjSIT_CODE.Text, _
                       CASTCommon.ConvertKamoku1TO2_K(aobjKAMOKU.Text), aobjKOUZA.Text) = False Then
                        MessageBox.Show(MSG0268W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    'If clsFUSION.fn_CHECK_DEJIT(aobjKIN_CODE.Text, aobjSIT_CODE.Text, _
                    '   clsFUSION.fn_CHG_KAMOKU1TO2(aobjKAMOKU.Text), aobjKOUZA.Text) = False Then
                    '    MessageBox.Show(MSG0268W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    'End If
                    '2013/10/24 saitou 標準修正 UPD --------------------------------------------------<<<<
                End If
            End If

            '-------------------------------------------------
            '口座チェック（(自金庫、口座チェック対象のみ)
            '-------------------------------------------------
            If aobjKIN_CODE.Text = strJIKINKO_CD And gstrCHK_KOUZA = "YES" Then
                '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------START
                Dim Ret As Integer = GCom.KouzaChk_ENTRY(aobjSIT_CODE.Text, _
                     CASTCommon.ConvertKamoku1TO2_K(aobjKAMOKU.Text), _
                     aobjKOUZA.Text, strKIGYO_CODE, strFURI_CODE, strKDB_KNAME, strERR_MSG, MainDB)
                'Dim Ret As Integer = GCom.KouzaChk_ENTRY(aobjSIT_CODE.Text, _
                '                     CASTCommon.ConvertKamoku1TO2(aobjKAMOKU.Text), _
                '                     aobjKOUZA.Text, strKIGYO_CODE, strFURI_CODE, strKDB_KNAME, strERR_MSG, MainDB)
                '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------END
                If Ret <> -1 Then
                    If intCHK = 0 Then       '正にチェックの場合 条件移動 2007/09/19
                        '2011/06/28 標準版修正 自振契約なしは正常扱い ------------------START
                        If Ret = 0 OrElse Ret = 2 Then
                            aobjERR.Text = ""
                            strERR_COLOR = Color.White
                            aobjKIN_CODE.BackColor = strERR_COLOR
                            aobjKIN_NAME.BackColor = strERR_COLOR
                            aobjSIT_CODE.BackColor = strERR_COLOR
                            aobjSIT_NAME.BackColor = strERR_COLOR
                            aobjKAMOKU.BackColor = strERR_COLOR
                            aobjKOUZA.BackColor = strERR_COLOR
                            aobjKEIYAKU_NAME.BackColor = strERR_COLOR
                            aobjFURIKIN.BackColor = strERR_COLOR
                        Else
                            aobjERR.Text = strERR_MSG
                            strERR_COLOR = Color.Yellow
                            aobjKIN_CODE.BackColor = strERR_COLOR
                            aobjKIN_NAME.BackColor = strERR_COLOR
                            aobjSIT_CODE.BackColor = strERR_COLOR
                            aobjSIT_NAME.BackColor = strERR_COLOR
                            aobjKAMOKU.BackColor = strERR_COLOR
                            aobjKOUZA.BackColor = strERR_COLOR
                            aobjKEIYAKU_NAME.BackColor = strERR_COLOR
                            aobjFURIKIN.BackColor = strERR_COLOR
                        End If
                        '2011/06/28 標準版修正 自振契約なしは正常扱い ------------------END
                    End If
                    '口座無し以外の場合は契約カナ名を設定する
                    '2011/06/16 標準版修正 活口座チェック追加 ------------------START
                    'If (Ret = 0 OrElse Ret = 2) AndAlso aobjKEIYAKU_NAME.Text = "" Then
                    If Ret <> 1 AndAlso aobjKEIYAKU_NAME.Text = "" Then
                        aobjKEIYAKU_NAME.Text = GCom.NzStr(strKDB_KNAME.Trim)
                    End If
                    '2011/06/16 標準版修正 活口座チェック追加 ------------------END
                Else
                    MessageBox.Show(MSG0236W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    aobjFURIKIN.Focus()
                    Exit Function
                End If
            Else
                aobjERR.Text = ""
            End If
            '-------------------------------------------------
            'ベリファイチェック
            '-------------------------------------------------
            If intCHK = 1 Then       '副にチェックの場合
                If fn_BERYFAI(aintGYO + ((aintPAGE - 1) * 15), aobjKIN_CODE, _
                              aobjSIT_CODE, aobjKAMOKU, aobjKOUZA, _
                              aobjKEIYAKU_NAME, aobjFURIKIN) = False Then
                    Exit Function
                End If
                MainDB.Close()
            End If
            '-------------------------------------------------
            '最大入力値のアップデート
            '-------------------------------------------------
            If aintPAGE > intMAXCNT Then
                intMAXCNT = aintPAGE
                intMAXLINE = aintGYO
            ElseIf aintPAGE = intMAXCNT Then
                If aintGYO > intMAXLINE Then
                    intMAXLINE = aintGYO
                End If
            End If

            '-------------------------------------------------
            '小計のカウント
            '-------------------------------------------------
            If fn_COUNT_SYOUKEI_KEN() = False Then
                MessageBox.Show(MSG0263W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            lngSYOUKEI_KEN(aintPAGE) = lblSyoukeiKen.Text
            '--------------------------------------------------
            '合計のカウント
            '--------------------------------------------------
            If fn_COUNT_GOUKEI_KEN() = False Then
                MessageBox.Show(MSG0267W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            '-------------------------------------------------
            '項番の表示
            '-------------------------------------------------
            aobjKOUBAN.Text = aintGYO + ((aintPAGE - 1) * 15)

            aobjNEXT.Focus()     'ﾌｫｰｶｽ移動
            fn_CHECK_FURIKIN = True
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(レコードチェック)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function

    Function fn_COUNT_SYOUKEI_KEN() As Boolean
        '============================================================================
        'NAME           :fn_COUNT_SYOUKEI_KEN
        'Parameter      :
        'Description    :小計件数のカウント（0円はカウントしない）
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/09/12
        'Update         :
        '============================================================================
        fn_COUNT_SYOUKEI_KEN = False
        intSYOUKEI_KEN = 0
        Try
            If txtFuriKin1.Text <> "" And txtFuriKin1.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin2.Text <> "" And txtFuriKin2.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin3.Text <> "" And txtFuriKin3.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin4.Text <> "" And txtFuriKin4.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin5.Text <> "" And txtFuriKin5.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin6.Text <> "" And txtFuriKin6.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin7.Text <> "" And txtFuriKin7.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin8.Text <> "" And txtFuriKin8.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin9.Text <> "" And txtFuriKin9.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin10.Text <> "" And txtFuriKin10.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin11.Text <> "" And txtFuriKin11.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin12.Text <> "" And txtFuriKin12.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin13.Text <> "" And txtFuriKin13.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin14.Text <> "" And txtFuriKin14.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            If txtFuriKin15.Text <> "" And txtFuriKin15.Text <> "0" Then
                intSYOUKEI_KEN += 1
            End If
            lblSyoukeiKen.Text = intSYOUKEI_KEN
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(小計件数カウント)", "失敗", ex.ToString)
        End Try
        fn_COUNT_SYOUKEI_KEN = True
    End Function

    Function fn_COUNT_GOUKEI_KEN() As Boolean
        '============================================================================
        'NAME           :fn_COUNT_GOUKEI_KEN
        'Parameter      :
        'Description    :合計件数のカウント（0円はカウントしない）
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_COUNT_GOUKEI_KEN = False
        Try
            lngGOUKEI_KEN = 0
            Dim I As Integer
            For I = 1 To intMAXCNT
                lngGOUKEI_KEN += lngSYOUKEI_KEN(I)
            Next I
            lblGoukeiKen.Text = lngGOUKEI_KEN
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(合計件数カウント)", "失敗", ex.ToString)
        End Try
        fn_COUNT_GOUKEI_KEN = True
    End Function

    Function fn_COUNT_SYOUKEI_KIN() As Long
        '============================================================================
        'NAME           :fn_COUNT_SYOUKEI_KIN
        'Parameter      :
        'Description    :小計金額の合計
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        Try
            fn_COUNT_SYOUKEI_KIN = 0
            If txtFuriKin1.Text <> "" And txtFuriKin1.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin1.Text)
            End If
            If txtFuriKin2.Text <> "" And txtFuriKin2.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin2.Text)
            End If
            If txtFuriKin3.Text <> "" And txtFuriKin3.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin3.Text)
            End If
            If txtFuriKin4.Text <> "" And txtFuriKin4.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin4.Text)
            End If
            If txtFuriKin5.Text <> "" And txtFuriKin5.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin5.Text)
            End If
            If txtFuriKin6.Text <> "" And txtFuriKin6.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin6.Text)
            End If
            If txtFuriKin7.Text <> "" And txtFuriKin7.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin7.Text)
            End If
            If txtFuriKin8.Text <> "" And txtFuriKin8.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin8.Text)
            End If
            If txtFuriKin9.Text <> "" And txtFuriKin9.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin9.Text)
            End If
            If txtFuriKin10.Text <> "" And txtFuriKin10.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin10.Text)
            End If
            If txtFuriKin11.Text <> "" And txtFuriKin11.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin11.Text)
            End If
            If txtFuriKin12.Text <> "" And txtFuriKin12.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin12.Text)
            End If
            If txtFuriKin13.Text <> "" And txtFuriKin13.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin13.Text)
            End If
            If txtFuriKin14.Text <> "" And txtFuriKin14.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin14.Text)
            End If
            If txtFuriKin15.Text <> "" And txtFuriKin15.Text <> "0" Then
                fn_COUNT_SYOUKEI_KIN += fn_DEL_KANMA(txtFuriKin15.Text)
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(小計金額計算)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_COUNT_GOUKEI_KIN() As Double
        '============================================================================
        'NAME           :fn_COUNT_GOUKEI_KIN
        'Parameter      :
        'Description    :合計金額の合計
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_COUNT_GOUKEI_KIN = 0
        Try
            P = 1
            L = 1
            Dim L_CNT As Integer
            For P = 1 To intMAXCNT
                If P = intMAXCNT Then
                    L_CNT = intMAXLINE
                Else
                    L_CNT = 15
                End If
                For L = 1 To L_CNT
                    fn_COUNT_GOUKEI_KIN += lngFURIKIN(P, L)
                Next L
            Next P
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(合計金額計算)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_UPDATE_SCHMAST(ByRef UKETUKE As String) As Boolean
        '============================================================================
        'NAME           :fn_UPDATE_SCHMAST
        'Parameter      :
        'Description    :スケジュールの更新
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_UPDATE_SCHMAST = False
        Dim SQL As New StringBuilder(128)
        Dim Ret As Integer
        Try
            SQL.Append("UPDATE S_SCHMAST SET ")
            If BERYFAI = 0 Then     'ベリファイ無の場合
                SQL.Append(" UKETUKE_FLG_S = '1'")
                UKETUKE = "1"
            ElseIf intCHK = 1 Then 'ベリファイ有かつ副の登録の場合
                SQL.Append(" UKETUKE_FLG_S = '1'")
                UKETUKE = "1"
            Else                    'ベリファイ有かつ主の登録の場合
                SQL.Append(" UKETUKE_FLG_S = '2'")
                UKETUKE = "2"
            End If
            SQL.Append(",UKETUKE_DATE_S = " & SQ(Format(System.DateTime.Today, "yyyyMMdd")))
            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            SQL.Append(" AND FURI_DATE_S = " & SQ(strFURI_DATE))

            Ret = MainDB.ExecuteNonQuery(SQL)
            If Ret < 1 Then
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール更新)", "失敗", ex.ToString)
            Return False
        End Try
        fn_UPDATE_SCHMAST = True
    End Function

    Function fn_BERYFAI(ByVal alngKOUBAN As Long, ByVal aobjKIN_NO As TextBox, ByVal aobjSIT_NO As TextBox, _
                         ByVal aobjKAMOKU As TextBox, ByVal aobjKOUZA As TextBox, ByVal aobjKEIYAKU_NAME As TextBox, _
                         ByVal aobjFURIKIN As TextBox) As Boolean
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
        Dim MSG As String
        Try
            Dim ERR_COLOR As Color
            ERR_COLOR = Color.Aqua

            '-------------------------------------------
            'テキストボックスの色の初期化
            '-------------------------------------------
            aobjKIN_NO.BackColor = Color.White
            aobjSIT_NO.BackColor = Color.White
            aobjKAMOKU.BackColor = Color.White
            aobjKOUZA.BackColor = Color.White
            aobjKEIYAKU_NAME.BackColor = Color.White
            aobjFURIKIN.BackColor = Color.White

            '-------------------------------------------
            '伝票マスタに登録されているか検索
            '-------------------------------------------
            SQL.Append("SELECT * FROM S_DENPYOMAST")
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            SQL.Append(" AND KEIYAKU_NO_E = " & SQ(Format(alngKOUBAN, "000000000000")))

            If OraReader.DataReader(SQL) = False Then
                If MessageBox.Show(MSG0031I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    aobjFURIKIN.Focus()
                    Exit Function
                End If
            Else
                '金融機関コードのチェック
                If aobjKIN_NO.Text <> GCom.NzStr(OraReader.GetString("TKIN_NO_E")) Then
                    MSG = String.Format(MSG0028I, "金融機関コード", GCom.NzStr(OraReader.GetString("TKIN_NO_E")), aobjKIN_NO.Text)
                    If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        aobjKIN_NO.BackColor = ERR_COLOR
                        aobjKIN_NO.Focus()
                        Exit Function
                    Else
                        MSG = String.Format(MSG0029I, GCom.NzStr(OraReader.GetString("TKIN_NO_E")), aobjKIN_NO.Text)
                        If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            If fn_UPDATE_DENPYOMAST_SEI("TKIN_NO_E", aobjKIN_NO.Text, alngKOUBAN) = False Then
                                MainDB.Rollback()
                                Exit Function
                            End If
                            MainDB.Commit()
                            MainDB.BeginTrans()
                        End If
                    End If
                End If
                '支店コードのチェック
                If aobjSIT_NO.Text <> GCom.NzStr(OraReader.GetString("TSIT_NO_E")) Then
                    MSG = String.Format(MSG0028I, "支店コード", GCom.NzStr(OraReader.GetString("TSIT_NO_E")), aobjSIT_NO.Text)
                    If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        aobjSIT_NO.BackColor = ERR_COLOR
                        aobjSIT_NO.Focus()
                        Exit Function
                    Else
                        MSG = String.Format(MSG0029I, GCom.NzStr(OraReader.GetString("TSIT_NO_E")), aobjSIT_NO.Text)
                        If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            If fn_UPDATE_DENPYOMAST_SEI("TSIT_NO_E", aobjSIT_NO.Text, alngKOUBAN) = False Then
                                MainDB.Rollback()
                                Exit Function
                            End If
                            MainDB.Commit()
                            MainDB.BeginTrans()
                        End If
                    End If
                End If
                '科目コードのチェック
                '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------START
                If aobjKAMOKU.Text <> CASTCommon.ConvertKamoku2TO1_K(GCom.NzStr(OraReader.GetString("KAMOKU_E"))) Then
                    MSG = String.Format(MSG0028I, "科目", CASTCommon.ConvertKamoku2TO1_K(GCom.NzStr(OraReader.GetString("KAMOKU_E"))), aobjKAMOKU.Text)
                    'If aobjKAMOKU.Text <> CASTCommon.ConvertKamoku2TO1(GCom.NzStr(OraReader.GetString("KAMOKU_E"))) Then
                    'MSG = String.Format(MSG0028I, "科目", CASTCommon.ConvertKamoku2TO1(GCom.NzStr(OraReader.GetString("KAMOKU_E"))), aobjKAMOKU.Text)
                    '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------END
                    If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        aobjKAMOKU.BackColor = ERR_COLOR
                        aobjKAMOKU.Focus()
                        Exit Function
                    Else
                        MSG = String.Format(MSG0029I, CASTCommon.ConvertKamoku2TO1(GCom.NzStr(OraReader.GetString("KAMOKU_E"))), aobjKAMOKU.Text)
                        If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            '2013/10/24 saitou 標準修正 UPD -------------------------------------------------->>>>
                            '科目変換はCastCommonを使用するように修正
                            If fn_UPDATE_DENPYOMAST_SEI("KAMOKU_E", CASTCommon.ConvertKamoku1TO2_K(aobjKAMOKU.Text), alngKOUBAN) = False Then
                                MainDB.Rollback()
                                Exit Function
                            End If
                            'If fn_UPDATE_DENPYOMAST_SEI("KAMOKU_E", clsFUSION.fn_CHG_KAMOKU1TO2(aobjKAMOKU.Text), alngKOUBAN) = False Then
                            '    MainDB.Rollback()
                            '    Exit Function
                            'End If
                            '2013/10/24 saitou 標準修正 UPD --------------------------------------------------<<<<
                            MainDB.Commit()
                            MainDB.BeginTrans()
                        End If
                    End If
                End If
                '口座番号のチェック
                If aobjKOUZA.Text <> GCom.NzStr(OraReader.GetString("KOUZA_E")) Then
                    MSG = String.Format(MSG0028I, "口座番号", GCom.NzStr(OraReader.GetString("KOUZA_E")), aobjKOUZA.Text)
                    If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        aobjKOUZA.BackColor = ERR_COLOR
                        aobjKOUZA.Focus()
                        Exit Function
                    Else
                        MSG = String.Format(MSG0029I, GCom.NzStr(OraReader.GetString("KOUZA_E")), aobjKOUZA.Text)
                        If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            If fn_UPDATE_DENPYOMAST_SEI("KOUZA_E", aobjKOUZA.Text, alngKOUBAN) = False Then
                                Exit Function
                                MainDB.Rollback()
                                Exit Function
                            End If
                            MainDB.Commit()
                            MainDB.BeginTrans()
                        End If
                    End If
                End If
                '契約者名のチェック
                If aobjKEIYAKU_NAME.Text <> GCom.NzStr(OraReader.GetString("KEIYAKU_KNAME_E")) Then
                    MSG = String.Format(MSG0028I, "契約者名", GCom.NzStr(OraReader.GetString("KEIYAKU_KNAME_E")) & vbCrLf, aobjKEIYAKU_NAME.Text)
                    If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        aobjKEIYAKU_NAME.BackColor = ERR_COLOR
                        aobjKEIYAKU_NAME.Focus()
                        Exit Function
                    Else
                        MSG = String.Format(MSG0029I, GCom.NzStr(OraReader.GetString("KEIYAKU_KNAME_E")), aobjKEIYAKU_NAME.Text)
                        If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                            If fn_UPDATE_DENPYOMAST_SEI("KEIYAKU_KNAME_E", aobjKEIYAKU_NAME.Text, alngKOUBAN) = False Then
                                MainDB.Rollback()
                                Exit Function
                            End If
                            MainDB.Commit()
                            MainDB.BeginTrans()
                        End If
                    End If
                End If
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
                            If fn_UPDATE_DENPYOMAST_SEI("FURIKIN_E", fn_DEL_KANMA(aobjFURIKIN.Text), alngKOUBAN) = False Then
                                MainDB.Rollback()
                                Exit Function
                            End If
                            MainDB.Commit()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ベリファイ)", "失敗", ex.ToString)
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        fn_BERYFAI = True
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

    Function fn_UPDATE_DENPYOMAST_SEI(ByVal astrFIELD As String, ByVal astrATAI As String, ByVal alngKOUBAN As Long) As Boolean
        '============================================================================
        'NAME           :fn_UPDATE_DENPYOMAST_SEI
        'Parameter      :astrFIELD：フィールド名／astrATAI：更新値／alngKOUBAN：項番
        'Description    :正（伝票マスタ）の値を更新する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_UPDATE_DENPYOMAST_SEI = False
        Dim Ret As Integer
        Dim SQL As New StringBuilder(128)
        Try
            If astrFIELD = "KEIYAKU_KNAME_E" And astrATAI = "" Then
                astrATAI = Space(5)
            ElseIf astrFIELD = "FURIKIN_E" And astrATAI = "" Then
                astrATAI = "0"
            End If

            SQL.Append(" UPDATE S_DENPYOMAST SET ")
            SQL.Append(astrFIELD & "=" & SQ(astrATAI))
            SQL.Append(",KOUSIN_DATE_E = " & SQ(Format(System.DateTime.Today, "yyyyMMdd")))
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND   TORIF_CODE_E = " & SQ(strTORIF_CODE))
            SQL.Append(" AND   KEIYAKU_NO_E = " & SQ(Format(alngKOUBAN, "000000000000")))

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

        fn_UPDATE_DENPYOMAST_SEI = True
    End Function


    Function fn_DEL_KANMA(ByVal aINTEXT As String) As Long
        '============================================================================
        'NAME           :fn_DEL_KANMA
        'Parameter      :aINTEXT:入力値
        'Description    :カンマ編集(カンマ削除）関数
        'Return         :カンマ削除後の数値
        'Create         :2004/10/25
        'Update         :
        '============================================================================
        Dim I As Integer
        Dim strO_SYOKITI As String = ""
        Try
            For I = 0 To aINTEXT.Length - 1        'ｶﾝﾏ&ｽﾍﾟｰｽとる
                If aINTEXT.Substring(I, 1) = "," Or aINTEXT.Substring(I, 1) = " " Then
                    strO_SYOKITI = strO_SYOKITI & ""
                Else
                    strO_SYOKITI = strO_SYOKITI & aINTEXT.Substring(I, 1)
                End If
            Next I
            fn_DEL_KANMA = CLng(strO_SYOKITI)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(カンマ削除)", "失敗", ex.ToString)
        End Try
    End Function

    Function fn_OBJECT_SET() As Boolean
        '============================================================================
        'NAME           :fn_OBJECT_SET
        'Parameter      :
        'Description    :画面のコントロールを配列型オブジェクトにセットする
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_OBJECT_SET = False
        Try
            '解約フラグ
            ckbDel(1) = ckbDel1
            ckbDel(2) = ckbDel2
            ckbDel(3) = ckbDel3
            ckbDel(4) = ckbDel4
            ckbDel(5) = ckbDel5
            ckbDel(6) = ckbDel6
            ckbDel(7) = ckbDel7
            ckbDel(8) = ckbDel8
            ckbDel(9) = ckbDel9
            ckbDel(10) = ckbDel10
            ckbDel(11) = ckbDel11
            ckbDel(12) = ckbDel12
            ckbDel(13) = ckbDel13
            ckbDel(14) = ckbDel14
            ckbDel(15) = ckbDel15

            '項目番号
            lblIndex(1) = lblIndex1
            lblIndex(2) = lblIndex2
            lblIndex(3) = lblIndex3
            lblIndex(4) = lblIndex4
            lblIndex(5) = lblIndex5
            lblIndex(6) = lblIndex6
            lblIndex(7) = lblIndex7
            lblIndex(8) = lblIndex8
            lblIndex(9) = lblIndex9
            lblIndex(10) = lblIndex10
            lblIndex(11) = lblIndex11
            lblIndex(12) = lblIndex12
            lblIndex(13) = lblIndex13
            lblIndex(14) = lblIndex14
            lblIndex(15) = lblIndex15
            '金融機関番号
            txtKinNo(1) = txtKinNO1
            txtKinNo(2) = txtKinNO2
            txtKinNo(3) = txtKinNO3
            txtKinNo(4) = txtKinNO4
            txtKinNo(5) = txtKinNO5
            txtKinNo(6) = txtKinNO6
            txtKinNo(7) = txtKinNO7
            txtKinNo(8) = txtKinNO8
            txtKinNo(9) = txtKinNO9
            txtKinNo(10) = txtKinNO10
            txtKinNo(11) = txtKinNO11
            txtKinNo(12) = txtKinNO12
            txtKinNo(13) = txtKinNO13
            txtKinNo(14) = txtKinNO14
            txtKinNo(15) = txtKinNO15
            '金融機関名
            txtKinName(1) = txtKinName1
            txtKinName(2) = txtKinName2
            txtKinName(3) = txtKinName3
            txtKinName(4) = txtKinName4
            txtKinName(5) = txtKinName5
            txtKinName(6) = txtKinName6
            txtKinName(7) = txtKinName7
            txtKinName(8) = txtKinName8
            txtKinName(9) = txtKinName9
            txtKinName(10) = txtKinName10
            txtKinName(11) = txtKinName11
            txtKinName(12) = txtKinName12
            txtKinName(13) = txtKinName13
            txtKinName(14) = txtKinName14
            txtKinName(15) = txtKinName15
            '支店番号
            txtSitNo(1) = txtSitNo1
            txtSitNo(2) = txtSitNo2
            txtSitNo(3) = txtSitNo3
            txtSitNo(4) = txtSitNo4
            txtSitNo(5) = txtSitNo5
            txtSitNo(6) = txtSitNo6
            txtSitNo(7) = txtSitNo7
            txtSitNo(8) = txtSitNo8
            txtSitNo(9) = txtSitNo9
            txtSitNo(10) = txtSitNo10
            txtSitNo(11) = txtSitNo11
            txtSitNo(12) = txtSitNo12
            txtSitNo(13) = txtSitNo13
            txtSitNo(14) = txtSitNo14
            txtSitNo(15) = txtSitNo15
            '支店名
            txtSitName(1) = txtSitName1
            txtSitName(2) = txtSitName2
            txtSitName(3) = txtSitName3
            txtSitName(4) = txtSitName4
            txtSitName(5) = txtSitName5
            txtSitName(6) = txtSitName6
            txtSitName(7) = txtSitName7
            txtSitName(8) = txtSitName8
            txtSitName(9) = txtSitName9
            txtSitName(10) = txtSitName10
            txtSitName(11) = txtSitName11
            txtSitName(12) = txtSitName12
            txtSitName(13) = txtSitName13
            txtSitName(14) = txtSitName14
            txtSitName(15) = txtSitName15
            '科目コード
            txtKamokuCode(1) = txtKamokuCode1
            txtKamokuCode(2) = txtKamokuCode2
            txtKamokuCode(3) = txtKamokuCode3
            txtKamokuCode(4) = txtKamokuCode4
            txtKamokuCode(5) = txtKamokuCode5
            txtKamokuCode(6) = txtKamokuCode6
            txtKamokuCode(7) = txtKamokuCode7
            txtKamokuCode(8) = txtKamokuCode8
            txtKamokuCode(9) = txtKamokuCode9
            txtKamokuCode(10) = txtKamokuCode10
            txtKamokuCode(11) = txtKamokuCode11
            txtKamokuCode(12) = txtKamokuCode12
            txtKamokuCode(13) = txtKamokuCode13
            txtKamokuCode(14) = txtKamokuCode14
            txtKamokuCode(15) = txtKamokuCode15
            '科目名
            lblKamoku(1) = lblKamoku1
            lblKamoku(2) = lblKamoku2
            lblKamoku(3) = lblKamoku3
            lblKamoku(4) = lblKamoku4
            lblKamoku(5) = lblKamoku5
            lblKamoku(6) = lblKamoku6
            lblKamoku(7) = lblKamoku7
            lblKamoku(8) = lblKamoku8
            lblKamoku(9) = lblKamoku9
            lblKamoku(10) = lblKamoku10
            lblKamoku(11) = lblKamoku11
            lblKamoku(12) = lblKamoku12
            lblKamoku(13) = lblKamoku13
            lblKamoku(14) = lblKamoku14
            lblKamoku(15) = lblKamoku15
            '口座番号
            txtKouza(1) = txtKouza1
            txtKouza(2) = txtKouza2
            txtKouza(3) = txtKouza3
            txtKouza(4) = txtKouza4
            txtKouza(5) = txtKouza5
            txtKouza(6) = txtKouza6
            txtKouza(7) = txtKouza7
            txtKouza(8) = txtKouza8
            txtKouza(9) = txtKouza9
            txtKouza(10) = txtKouza10
            txtKouza(11) = txtKouza11
            txtKouza(12) = txtKouza12
            txtKouza(13) = txtKouza13
            txtKouza(14) = txtKouza14
            txtKouza(15) = txtKouza15
            '振込金額
            txtFurikin(1) = txtFuriKin1
            txtFurikin(2) = txtFuriKin2
            txtFurikin(3) = txtFuriKin3
            txtFurikin(4) = txtFuriKin4
            txtFurikin(5) = txtFuriKin5
            txtFurikin(6) = txtFuriKin6
            txtFurikin(7) = txtFuriKin7
            txtFurikin(8) = txtFuriKin8
            txtFurikin(9) = txtFuriKin9
            txtFurikin(10) = txtFuriKin10
            txtFurikin(11) = txtFuriKin11
            txtFurikin(12) = txtFuriKin12
            txtFurikin(13) = txtFuriKin13
            txtFurikin(14) = txtFuriKin14
            txtFurikin(15) = txtFuriKin15
            '契約者名
            txtKeiyakuName(1) = txtKeiyakuName1
            txtKeiyakuName(2) = txtKeiyakuName2
            txtKeiyakuName(3) = txtKeiyakuName3
            txtKeiyakuName(4) = txtKeiyakuName4
            txtKeiyakuName(5) = txtKeiyakuName5
            txtKeiyakuName(6) = txtKeiyakuName6
            txtKeiyakuName(7) = txtKeiyakuName7
            txtKeiyakuName(8) = txtKeiyakuName8
            txtKeiyakuName(9) = txtKeiyakuName9
            txtKeiyakuName(10) = txtKeiyakuName10
            txtKeiyakuName(11) = txtKeiyakuName11
            txtKeiyakuName(12) = txtKeiyakuName12
            txtKeiyakuName(13) = txtKeiyakuName13
            txtKeiyakuName(14) = txtKeiyakuName14
            txtKeiyakuName(15) = txtKeiyakuName15
            '備考
            lblBikou(1) = lblBikou1
            lblBikou(2) = lblBikou2
            lblBikou(3) = lblBikou3
            lblBikou(4) = lblBikou4
            lblBikou(5) = lblBikou5
            lblBikou(6) = lblBikou6
            lblBikou(7) = lblBikou7
            lblBikou(8) = lblBikou8
            lblBikou(9) = lblBikou9
            lblBikou(10) = lblBikou10
            lblBikou(11) = lblBikou11
            lblBikou(12) = lblBikou12
            lblBikou(13) = lblBikou13
            lblBikou(14) = lblBikou14
            lblBikou(15) = lblBikou15
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(オブジェクトセット)", "失敗", ex.ToString)
            Return False
        End Try

        fn_OBJECT_SET = True
    End Function

    Private Function fn_CLC_DENPYOMAST(ByRef Counter As Long, ByRef Kingaku As Long) As Boolean
        '============================================================================
        'NAME           :fn_CLC_DENPYOMAST
        'Parameter      :Counter 合計件数 ,Kingaku 合計金額
        'Description    :伝票の登録対象となる件数･金額を取得する
        'Return         :True=OK,False=NG
        'Create         :2009/10/01
        'Update         :
        '============================================================================
        fn_CLC_DENPYOMAST = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            '-------------------------------------------
            '伝票マスタの合計件数･金額を取得
            '-------------------------------------------
            '振込金額が0円以上のものを集計
            SQL.Append(" SELECT NVL(COUNT(FURIKIN_E),0) COUNTER ,NVL(SUM(FURIKIN_E),0) SUM_KIN")
            SQL.Append(" FROM S_DENPYOMAST")
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(伝票合計金額･件数取得)", "失敗", ex.ToString)
            OraReader.Close()
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_CLC_DENPYOMAST = True
    End Function

    Private Function fn_UPDATE_DENPYOMAST() As Boolean
        '============================================================================
        'NAME           :fn_UPDATE_DENPYOMAST
        'Parameter      :
        'Description    :全データを伝票マスタ/伝票副マスタに更新・インサートする
        'Return         :True=OK,False=NG
        'Create         :2009/09/29
        'Update         :
        '============================================================================
        fn_UPDATE_DENPYOMAST = False
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As StringBuilder
        Dim SQL2 As StringBuilder
        Try
            SQL = New StringBuilder(128)
            '挿入の際に不要なレコードが残っていることがあるため、一旦対象を全削除後に全部挿入する

            SQL.Append("DELETE FROM " & strMAST)
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            MainDB.ExecuteNonQuery(SQL)
            For P = 1 To intMAXCNT
                Dim L_END As Integer
                If P = intMAXCNT Then
                    L_END = intMAXLINE
                Else
                    L_END = 15
                End If
                For L = 1 To L_END
                    'SQL = New StringBuilder(128)
                    ''-------------------------------------------
                    ''伝票マスタ/伝票副マスタに登録されているか検索
                    ''-------------------------------------------
                    'SQL.Append("SELECT * FROM " & strMAST)
                    'SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
                    'SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
                    'SQL.Append(" AND KEIYAKU_NO_E = " & SQ(Format(lngRECORD_NO(P, L), "000000000000")))

                    SQL2 = New StringBuilder(128)
                    'If OraReader.DataReader(SQL) = False Then
                    '-----------------------------------------------------
                    '伝票マスタ/伝票副マスタに登録されていなかった場合、インサート
                    '-----------------------------------------------------
                    SQL2.Append("INSERT INTO " & strMAST)
                    SQL2.Append(" (FSYORI_KBN_E,TORIS_CODE_E,TORIF_CODE_E,FURI_DATE_E")
                    SQL2.Append(",FURI_CODE_E,TEIKEI_KBN_E,HYOUJI_SEQ_E,KAMOKU_E,KOUZA_E,KEIYAKU_NO_E")
                    SQL2.Append(",KEIYAKU_KNAME_E,KEIYAKU_NNAME_E,KOKYAKU_NO_E,KINGAKU_KBN_E,FURIKIN_E")
                    SQL2.Append(",TESUU_E,TESUU2_E,TESUU_KBN_E,TKIN_NO_E,TSIT_NO_E,SINKI_CODE_E")
                    SQL2.Append(",RECORD_NO_E,KAIYAKU_E,KAISI_DATE_E,SAKUSEI_DATE_E,KOUSIN_DATE_E")
                    SQL2.Append(",KEIYAKU_NJYU_E,KEIYAKU_DENWA_E,JYUYOUKA_NO_E,ERR_MSG_E,YOBI1_E,YOBI2_E")
                    SQL2.Append(",YOBI3_E,YOBI4_E,YOBI5_E,YOBI6_E,YOBI7_E,YOBI8_E,YOBI9_E,YOBI10_E) VALUES (")
                    SQL2.Append("'3'")                                          ' FSYORI_KBN_E
                    SQL2.Append("," & SQ(strTORIS_CODE))                       ' TORIS_CODE_E
                    SQL2.Append("," & SQ(strTORIF_CODE))                       ' TORIF_CODE_E
                    SQL2.Append("," & SQ(strFURI_DATE))                        ' FURI_DATE_E
                    SQL2.Append("," & SQ(strFURI_CODE))                         ' FURI_CODE_E
                    SQL2.Append(",'0'")                                         ' TEIKEI_KBN_E
                    SQL2.Append("," & SQ(Format(lngRECORD_NO(P, L), "0000")))   ' HYOJI_SEQ_E
                    If strKAMOKU(P, L).Trim = "" Then
                        SQL2.Append(",''")   ' KAMOKU_E
                    Else
                        '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------START
                        SQL2.Append("," & SQ(ConvertKamoku1TO2_K(strKAMOKU(P, L))))   ' KAMOKU_E
                        'SQL2.Append("," & SQ(ConvertKamoku1TO2(strKAMOKU(P, L))))   ' KAMOKU_E
                        '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------END
                    End If
                    SQL2.Append("," & SQ(strKOUZA(P, L)))                       ' KOUZA_E
                    SQL2.Append("," & SQ(Format(lngRECORD_NO(P, L), "000000000000")))   ' KEIYAYU_NO_E
                    If strKEIYAKU_KNAME(P, L) = "" Then
                        SQL2.Append("," & SQ(Space(1)))                         ' KEIYAKU_KNAME_E
                    Else
                        SQL2.Append("," & SQ(strKEIYAKU_KNAME(P, L)))           'KEIYAKU_KNAME_E
                    End If
                    SQL2.Append("," & SQ(Space(1)))                                     ' KEIYAKU_NNAME_E
                    SQL2.Append("," & SQ(Format(lngRECORD_NO(P, L), "0000000000")))      ' KOKYAKU_NO_E
                    SQL2.Append(",'0'")                                         ' KINGAKU_KBN_E
                    SQL2.Append("," & lngFURIKIN(P, L))                         ' FURIKIN_E
                    SQL2.Append(",0")                                           ' TESUU_E
                    SQL2.Append(",0")                                           ' TESUU2_E
                    SQL2.Append(",0")                                           ' TESUU_KBN_E
                    SQL2.Append("," & SQ(strTKIN_NO(P, L)))                     ' TKIN_NO_E
                    SQL2.Append("," & SQ(strTSIT_NO(P, L)))                     ' TSIT_NO_E
                    SQL2.Append(",'0'")                                         ' SINKI_CODE_E
                    SQL2.Append("," & SQ(Format(lngRECORD_NO(P, L), "0000")))   ' RECORD_NO_E
                    SQL2.Append("," & SQ(strDEL_CHK(P, L)))                     ' KAIYAKU_E
                    SQL2.Append(",'00000000'")                                  ' KAISI_DATE_E
                    SQL2.Append("," & SQ(Format(System.DateTime.Today, "yyyyMMdd")))        ' SAKUSEI_DATE_E
                    SQL2.Append(",'00000000'")                                  ' KOUSIN_DATE_E
                    SQL2.Append("," & SQ(Space(1)))                                   ' KEIYAKU_NJYU_E
                    SQL2.Append("," & SQ(Space(1)))                                     ' KEIYAKU_DENWA_E
                    SQL2.Append("," & SQ(Space(1)))                                     ' JYUYOUKA_E
                    SQL2.Append("," & SQ(strBIKOU(P, L)))                       ' ERR_MSG_E
                    SQL2.Append("," & SQ(Space(1)))                             ' YOBI1_E
                    SQL2.Append("," & SQ(Space(1)))                             ' YOBI2_E
                    SQL2.Append("," & SQ(Space(1)))                             ' YOBI3_E
                    SQL2.Append("," & SQ(Space(1)))                             ' YOBI4_E
                    SQL2.Append("," & SQ(Space(1)))                             ' YOBI5_E
                    SQL2.Append("," & SQ(Space(1)))                             ' YOBI6_E
                    SQL2.Append("," & SQ(Space(1)))                             ' YOBI7_E
                    SQL2.Append("," & SQ(Space(1)))                             ' YOBI8_E
                    SQL2.Append("," & SQ(Space(1)))                             ' YOBI9_E
                    SQL2.Append("," & SQ(Space(1)))                                          ' YOBI10_E
                    SQL2.Append(" )")
                    'Else
                    ''-----------------------------------------------------
                    ''伝票マスタ/伝票副マスタに登録されていた場合、更新
                    ''-----------------------------------------------------
                    'SQL2.Append("UPDATE " & strMAST & " SET ")
                    'SQL2.Append(" TKIN_NO_E =" & SQ(strTKIN_NO(P, L)))
                    'SQL2.Append(",TSIT_NO_E =" & SQ(strTSIT_NO(P, L)))
                    'If strKAMOKU(P, L).Trim = "" Then
                    '    SQL2.Append(",KAMOKU_E = ''")
                    'Else
                    '    SQL2.Append(",KAMOKU_E =" & SQ(ConvertKamoku1TO2(strKAMOKU(P, L))))
                    'End If
                    'SQL2.Append(",KOUZA_E =" & SQ(strKOUZA(P, L)))
                    'If strKEIYAKU_KNAME(P, L) = "" Then
                    '    SQL2.Append(",KEIYAKU_KNAME_E =" & SQ(Space(1)))
                    'Else
                    '    SQL2.Append(",KEIYAKU_KNAME_E =" & SQ(strKEIYAKU_KNAME(P, L)))
                    'End If
                    'SQL2.Append(",KAIYAKU_E =" & SQ(strDEL_CHK(P, L)))
                    'SQL2.Append(",FURI_DATE_E =" & SQ(strFURI_DATE))
                    'SQL2.Append(",FURIKIN_E =" & lngFURIKIN(P, L))
                    'SQL2.Append(",ERR_MSG_E =" & SQ(strBIKOU(P, L)))
                    'SQL2.Append(",KOUSIN_DATE_E =" & SQ(Format(System.DateTime.Today, "yyyyMMdd")))
                    'SQL2.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
                    'SQL2.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
                    'SQL2.Append(" AND KEIYAKU_NO_E = " & SQ(Format(lngRECORD_NO(P, L), "000000000000")))
                    'End If
                    MainDB.ExecuteNonQuery(SQL2)
                    If intMAXCNT = 1 Then
                        If L = intMAXLINE Then
                            Exit For
                        End If
                    End If
                Next
            Next
        Catch ex As Exception
            '更新/登録失敗
            MessageBox.Show(String.Format(MSG0002E, "登録"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録･更新処理)", "失敗", ex.ToString)
            OraReader.Close()
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_UPDATE_DENPYOMAST = True
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
                                       "S_DPY_S_" & _
                                       strTORIS_CODE & strTORIF_CODE & "_" & _
                                       "05" & "_" & _
                                       strFURI_DATE & "_" & _
                                       TimeStamp & "_" & _
                                       Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                       "000" & _
                                       ".DAT"
                    strIRAI_FILE = EtcFolder & _
                                   "S_DPY_S_" & _
                                   strTORIS_CODE & strTORIF_CODE & "_" & _
                                   "05" & "_" & _
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
            SQL.Append(" SELECT * FROM S_DENPYOMAST")
            SQL.Append(" WHERE TORIS_CODE_E = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_E = " & SQ(strTORIF_CODE))
            SQL.Append(" AND KAIYAKU_E = '0'")
            SQL.Append(" AND FURIKIN_E > 0")
            SQL.Append(" ORDER BY KEIYAKU_NO_E ASC")

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
                        .ZG12 = GCom.NzStr(OraReader.GetItem("KEIYAKU_NO_E")).PadRight(20).Substring(0, 10)
                        .ZG13 = GCom.NzStr(OraReader.GetItem("KEIYAKU_NO_E")).PadRight(20).Substring(10, 10)
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

    Private Function fn_GYO_SOUNYU() As Boolean
        '============================================================================
        'NAME           :fn_GYO_SOUNYU
        'Parameter      :
        'Description    :指定された行に空白行を挿入
        'Return         :True=OK,False=NG
        'Create         :2009/10/21
        'Update         :
        '============================================================================
        Dim SounyuPage As Integer
        Dim SounyuGyo As Integer
        Dim i As Integer
        Dim j As Integer

        '表示項目一時保持用配列
        Dim strDEL_CHK_A(15) As String
        Dim strTKIN_NO_A(15) As String
        Dim strTKIN_NAME_A(15) As String
        Dim strTSIT_NO_A(15) As String
        Dim strTSIT_NAME_A(15) As String
        Dim strKAMOKU_A(15) As String
        Dim strKOUZA_A(15) As String
        Dim strKEIYAKU_KNAME_A(15) As String
        Dim lngFURIKIN_A(15) As Long
        Dim strBIKOU_A(15) As String
        Dim lngRECORD_NO_A(15) As Long

        '背景色一時保持用配列
        Dim clrTKIN_NO_Color_A(15) As Color
        Dim clrTKIN_NAME_Color_A(15) As Color
        Dim clrTSIT_NO_Color_A(15) As Color
        Dim clrTSIT_NAME_Color_A(15) As Color
        Dim clrKAMOKU_Color_A(15) As Color
        Dim clrKOUZA_Color_A(15) As Color
        Dim clrKEIYAKU_NNAME_Color_A(15) As Color
        Dim clrFURIKIN_Color_A(15) As Color

        '項番設定
        Dim RecordNo As Long = 0
        Try
            '※指定項番のページ・行を取得（これ以前のデータは変更しない）
            SounyuPage = Math.Floor(CDbl(txtSounyuKoban.Text) / 15) + 1 '指定項番のあるページ

            SounyuGyo = CDbl(txtSounyuKoban.Text) Mod 15                '指定項番の行
            If SounyuGyo = 0 Then
                SounyuGyo = 15
                SounyuPage -= 1
            End If

            If intMAXLINE = 15 Then
                '※登録処理を促し、挿入処理を中断する
                MessageBox.Show(MSG0060I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return False
            Else
                '※挿入する分の行数を増やす
                intMAXLINE += 1
            End If

            '※指定行以降のデータは１行後へずらす
            For P = SounyuPage To intMAXCNT
                If P = SounyuPage Then
                    i = SounyuGyo
                Else
                    i = 1
                End If

                For L = i To 15
                    '※現在行の情報を一時的に保持
                    strDEL_CHK_A(L) = strDEL_CHK(P, L)
                    strTKIN_NO_A(L) = strTKIN_NO(P, L)
                    strTSIT_NO_A(L) = strTSIT_NO(P, L)
                    strTKIN_NAME_A(L) = strTKIN_NAME(P, L)
                    strTSIT_NAME_A(L) = strTSIT_NAME(P, L)
                    strKAMOKU_A(L) = strKAMOKU(P, L)
                    strKOUZA_A(L) = strKOUZA(P, L)
                    strKEIYAKU_KNAME_A(L) = strKEIYAKU_KNAME(P, L)
                    lngFURIKIN_A(L) = lngFURIKIN(P, L)
                    strBIKOU_A(L) = strBIKOU(P, L)
                    lngRECORD_NO_A(L) = lngRECORD_NO(P, L)

                    '※現在行の背景色情報を一時的に保持
                    clrTKIN_NO_Color_A(L) = clrTKIN_NO_Color(P, L)
                    clrTKIN_NAME_Color_A(L) = clrTKIN_NAME_Color(P, L)
                    clrTSIT_NO_Color_A(L) = clrTSIT_NO_Color(P, L)
                    clrTSIT_NAME_Color_A(L) = clrTSIT_NAME_Color(P, L)
                    clrKAMOKU_Color_A(L) = clrKAMOKU_Color(P, L)
                    clrKOUZA_Color_A(L) = clrKOUZA_Color(P, L)
                    clrKEIYAKU_NNAME_Color_A(L) = clrKEIYAKU_NNAME_Color(P, L)
                    clrFURIKIN_Color_A(L) = clrFURIKIN_Color(P, L)

                    If lngRECORD_NO(P, L) = CLng(txtSounyuKoban.Text) Then
                        '※空白の行を挿入
                        '  指定行の初期化
                        strDEL_CHK(P, L) = "0"
                        strTKIN_NO(P, L) = ""
                        strTSIT_NO(P, L) = ""
                        strTKIN_NAME(P, L) = ""
                        strTSIT_NAME(P, L) = ""
                        strKAMOKU(P, L) = ""
                        strKOUZA(P, L) = ""
                        strKEIYAKU_KNAME(P, L) = ""
                        lngFURIKIN(P, L) = "0"
                        strBIKOU(P, L) = ""
                        lngRECORD_NO(P, L) = L + ((P - 1) * 15)
                        RecordNo = L + ((P - 1) * 15)

                        '※背景色を白色にする
                        clrTKIN_NO_Color(P, L) = Color.White
                        clrTKIN_NAME_Color(P, L) = Color.White
                        clrTSIT_NO_Color(P, L) = Color.White
                        clrTSIT_NAME_Color(P, L) = Color.White
                        clrKAMOKU_Color(P, L) = Color.White
                        clrKOUZA_Color(P, L) = Color.White
                        clrKEIYAKU_NNAME_Color(P, L) = Color.White
                        clrFURIKIN_Color(P, L) = Color.White
                    Else
                        '※前行の値を設定
                        If L = 1 Then
                            j = 15
                        Else
                            j = L - 1
                        End If
                        RecordNo += 1   '以降は連番とする
                        '※値を設定
                        strDEL_CHK(P, L) = strDEL_CHK_A(j)
                        strTKIN_NO(P, L) = strTKIN_NO_A(j)
                        strTSIT_NO(P, L) = strTSIT_NO_A(j)
                        strTKIN_NAME(P, L) = strTKIN_NAME_A(j)
                        strTSIT_NAME(P, L) = strTSIT_NAME_A(j)
                        strKAMOKU(P, L) = strKAMOKU_A(j)
                        strKOUZA(P, L) = strKOUZA_A(j)
                        strKEIYAKU_KNAME(P, L) = strKEIYAKU_KNAME_A(j)
                        lngFURIKIN(P, L) = lngFURIKIN_A(j)
                        strBIKOU(P, L) = strBIKOU_A(j)
                        'lngRECORD_NO(P, L) = lngRECORD_NO_A(j)
                        lngRECORD_NO(P, L) = RecordNo
                        '※背景色を設定
                        clrTKIN_NO_Color(P, L) = clrTKIN_NO_Color_A(j)
                        clrTKIN_NAME_Color(P, L) = clrTKIN_NAME_Color_A(j)
                        clrTSIT_NO_Color(P, L) = clrTSIT_NO_Color_A(j)
                        clrTSIT_NAME_Color(P, L) = clrTSIT_NAME_Color_A(j)
                        clrKAMOKU_Color(P, L) = clrKAMOKU_Color_A(j)
                        clrKOUZA_Color(P, L) = clrKOUZA_Color_A(j)
                        clrKEIYAKU_NNAME_Color(P, L) = clrKEIYAKU_NNAME_Color_A(j)
                        clrFURIKIN_Color(P, L) = clrFURIKIN_Color_A(j)
                    End If

                    If P = intMAXCNT AndAlso L = intMAXLINE Then
                        Exit For
                    End If
                Next
            Next
            Return True

        Catch ex As Exception
            MessageBox.Show(String.Format(MSG0027E, "行", "挿入"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

    End Function
#End Region

#Region " イベント"
    '数値評価(Zero埋めフォーマット有り：金融機関登録エリア：金融機関の場合)
    Private Sub Bank_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtKinNO1.Validating, txtKinNO2.Validating, txtKinNO3.Validating, txtKinNO4.Validating, txtKinNO5.Validating, _
            txtKinNO6.Validating, txtKinNO7.Validating, txtKinNO8.Validating, txtKinNO9.Validating, txtKinNO10.Validating, _
            txtKinNO11.Validating, txtKinNO12.Validating, txtKinNO13.Validating, txtKinNO14.Validating, txtKinNO15.Validating
        Try
            Dim txtKinNo As TextBox = CType(sender, TextBox)
            Dim txtKinName As TextBox = Nothing
            Dim txtSitNo As TextBox = Nothing
            Dim txtSitName As TextBox = Nothing
            Select Case txtKinNo.Name
                Case txtKinNO1.Name
                    txtKinName = txtKinName1
                    txtSitNo = txtSitNo1
                    txtSitName = txtSitName1
                Case txtKinNO2.Name
                    txtKinName = txtKinName2
                    txtSitNo = txtSitNo2
                    txtSitName = txtSitName2
                Case txtKinNO3.Name
                    txtKinName = txtKinName3
                    txtSitNo = txtSitNo3
                    txtSitName = txtSitName3
                Case txtKinNO4.Name
                    txtKinName = txtKinName4
                    txtSitNo = txtSitNo4
                    txtSitName = txtSitName4
                Case txtKinNO5.Name
                    txtKinName = txtKinName5
                    txtSitNo = txtSitNo5
                    txtSitName = txtSitName5
                Case txtKinNO6.Name
                    txtKinName = txtKinName6
                    txtSitNo = txtSitNo6
                    txtSitName = txtSitName6
                Case txtKinNO7.Name
                    txtKinName = txtKinName7
                    txtSitNo = txtSitNo7
                    txtSitName = txtSitName7
                Case txtKinNO8.Name
                    txtKinName = txtKinName8
                    txtSitNo = txtSitNo8
                    txtSitName = txtSitName8
                Case txtKinNO9.Name
                    txtKinName = txtKinName9
                    txtSitNo = txtSitNo9
                    txtSitName = txtSitName9
                Case txtKinNO10.Name
                    txtKinName = txtKinName10
                    txtSitNo = txtSitNo10
                    txtSitName = txtSitName10
                Case txtKinNO11.Name
                    txtKinName = txtKinName11
                    txtSitNo = txtSitNo11
                    txtSitName = txtSitName11
                Case txtKinNO12.Name
                    txtKinName = txtKinName12
                    txtSitNo = txtSitNo12
                    txtSitName = txtSitName12
                Case txtKinNO13.Name
                    txtKinName = txtKinName13
                    txtSitNo = txtSitNo13
                    txtSitName = txtSitName13
                Case txtKinNO14.Name
                    txtKinName = txtKinName14
                    txtSitNo = txtSitNo14
                    txtSitName = txtSitName14
                Case txtKinNO15.Name
                    txtKinName = txtKinName15
                    txtSitNo = txtSitNo15
                    txtSitName = txtSitName15
            End Select
            Dim Temp As String = GCom.NzDec(CType(sender, TextBox).Text, "")
            If Temp.Length = 0 Then
                txtKinNo.Text = ""
                txtKinName.Text = ""
            Else
                Call GCom.NzNumberString(txtKinNo, True)
                txtKinName.Text = GCom.GetBKBRName(txtKinNo.Text, "", 15)
                If txtSitNo.Text.Trim <> "" Then
                    txtSitName.Text = GCom.GetBKBRName(txtKinNo.Text, txtSitNo.Text, 15)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("金融機関設定", "失敗", ex.ToString)
        End Try
    End Sub

    '数値評価(Zero埋めフォーマット有り：金融機関登録エリア：支店の場合)
    Private Sub Sit_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
   Handles txtSitNo1.Validating, txtSitNo2.Validating, txtSitNo3.Validating, txtSitNo4.Validating, txtSitNo5.Validating, _
           txtSitNo6.Validating, txtSitNo7.Validating, txtSitNo8.Validating, txtSitNo9.Validating, txtSitNo10.Validating, _
           txtSitNo11.Validating, txtSitNo12.Validating, txtSitNo13.Validating, txtSitNo14.Validating, txtSitNo15.Validating
        Try
            Dim txtSitNo As TextBox = CType(sender, TextBox)
            Dim txtSitName As TextBox = Nothing
            Dim txtKinNo As TextBox = Nothing
            Dim txtKinName As TextBox = Nothing

            Select Case txtSitNo.Name
                Case txtSitNo1.Name
                    txtSitName = txtSitName1
                    txtKinNo = txtKinNO1
                    txtKinName = txtKinName1
                Case txtSitNo2.Name
                    txtSitName = txtSitName2
                    txtKinNo = txtKinNO2
                    txtKinName = txtKinName2
                Case txtSitNo3.Name
                    txtSitName = txtSitName3
                    txtKinNo = txtKinNO3
                    txtKinName = txtKinName3
                Case txtSitNo4.Name
                    txtSitName = txtSitName4
                    txtKinNo = txtKinNO4
                    txtKinName = txtKinName4
                Case txtSitNo5.Name
                    txtSitName = txtSitName5
                    txtKinNo = txtKinNO5
                    txtKinName = txtKinName5
                Case txtSitNo6.Name
                    txtSitName = txtSitName6
                    txtKinNo = txtKinNO6
                    txtKinName = txtKinName6
                Case txtSitNo7.Name
                    txtSitName = txtSitName7
                    txtKinNo = txtKinNO7
                    txtKinName = txtKinName7
                Case txtSitNo8.Name
                    txtSitName = txtSitName8
                    txtKinNo = txtKinNO8
                    txtKinName = txtKinName8
                Case txtSitNo9.Name
                    txtSitName = txtSitName9
                    txtKinNo = txtKinNO9
                    txtKinName = txtKinName9
                Case txtSitNo10.Name
                    txtSitName = txtSitName10
                    txtKinNo = txtKinNO10
                    txtKinName = txtKinName10
                Case txtSitNo11.Name
                    txtSitName = txtSitName11
                    txtKinNo = txtKinNO11
                    txtKinName = txtKinName11
                Case txtSitNo12.Name
                    txtSitName = txtSitName12
                    txtKinNo = txtKinNO12
                    txtKinName = txtKinName12
                Case txtSitNo13.Name
                    txtSitName = txtSitName13
                    txtKinNo = txtKinNO13
                    txtKinName = txtKinName13
                Case txtSitNo14.Name
                    txtSitName = txtSitName14
                    txtKinNo = txtKinNO14
                    txtKinName = txtKinName14
                Case txtSitNo15.Name
                    txtSitName = txtSitName15
                    txtKinNo = txtKinNO15
                    txtKinName = txtKinName15
            End Select
            Dim Temp As String = GCom.NzDec(CType(sender, TextBox).Text, "")
            If Temp.Length = 0 Then
                txtSitNo.Text = ""
                txtSitName.Text = ""
            Else
                Call GCom.NzNumberString(txtSitNo, True)
                txtKinName.Text = GCom.GetBKBRName(txtKinNo.Text, "", 15)
                txtSitName.Text = GCom.GetBKBRName(txtKinNo.Text, txtSitNo.Text, 15)
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("支店設定", "失敗", ex.ToString)
        End Try
    End Sub

    '科目名設定
    Private Sub Kamoku_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles txtKamokuCode1.Validating, txtKamokuCode2.Validating, txtKamokuCode3.Validating, txtKamokuCode4.Validating, txtKamokuCode5.Validating, _
                    txtKamokuCode6.Validating, txtKamokuCode7.Validating, txtKamokuCode8.Validating, txtKamokuCode9.Validating, txtKamokuCode10.Validating, _
                    txtKamokuCode11.Validating, txtKamokuCode12.Validating, txtKamokuCode13.Validating, txtKamokuCode14.Validating, txtKamokuCode15.Validating
        Try
            Dim txtKamokuCode As TextBox = CType(sender, TextBox)
            Dim lblKamoku As Label = Nothing
            Select Case txtKamokuCode.Name
                Case txtKamokuCode1.Name
                    lblKamoku = lblKamoku1
                Case txtKamokuCode2.Name
                    lblKamoku = lblKamoku2
                Case txtKamokuCode3.Name
                    lblKamoku = lblKamoku3
                Case txtKamokuCode4.Name
                    lblKamoku = lblKamoku4
                Case txtKamokuCode5.Name
                    lblKamoku = lblKamoku5
                Case txtKamokuCode6.Name
                    lblKamoku = lblKamoku6
                Case txtKamokuCode7.Name
                    lblKamoku = lblKamoku7
                Case txtKamokuCode8.Name
                    lblKamoku = lblKamoku8
                Case txtKamokuCode9.Name
                    lblKamoku = lblKamoku9
                Case txtKamokuCode10.Name
                    lblKamoku = lblKamoku10
                Case txtKamokuCode11.Name
                    lblKamoku = lblKamoku11
                Case txtKamokuCode12.Name
                    lblKamoku = lblKamoku12
                Case txtKamokuCode13.Name
                    lblKamoku = lblKamoku13
                Case txtKamokuCode14.Name
                    lblKamoku = lblKamoku14
                Case txtKamokuCode15.Name
                    lblKamoku = lblKamoku15
            End Select

            txtKamokuCode.Text = Trim(txtKamokuCode.Text)
            If txtKamokuCode.Text.Length = 0 Then
                lblKamoku.Text = "　"
                Exit Sub
            End If
            Select Case txtKamokuCode.Text
                Case "1"
                    lblKamoku.Text = "普"
                Case "2"
                    lblKamoku.Text = "当"
                Case "3"
                    lblKamoku.Text = "納"
                Case "4"
                    lblKamoku.Text = "職"
                Case "9"
                    lblKamoku.Text = "他"
                Case Else
                    lblKamoku.Text = "他"
            End Select
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("科目設定", "失敗", ex.ToString)
        End Try

    End Sub

    'カナ項目チェック用
    Private Sub NzCheckString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles txtKeiyakuName1.Validating, txtKeiyakuName2.Validating, txtKeiyakuName3.Validating, txtKeiyakuName4.Validating, txtKeiyakuName5.Validating, _
                    txtKeiyakuName6.Validating, txtKeiyakuName7.Validating, txtKeiyakuName8.Validating, txtKeiyakuName9.Validating, txtKeiyakuName10.Validating, _
                    txtKeiyakuName11.Validating, txtKeiyakuName12.Validating, txtKeiyakuName13.Validating, txtKeiyakuName14.Validating, txtKeiyakuName15.Validating
        Try
            Call GCom.NzCheckString(CType(sender, TextBox))
            Dim BRet As Boolean = GCom.CheckZenginChar(CType(sender, TextBox))
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("カナ変換", "失敗", ex.ToString)
        End Try
    End Sub

    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
             Handles txtKouza1.Validating, txtKouza2.Validating, txtKouza3.Validating, txtKouza4.Validating, txtKouza5.Validating, _
                    txtKouza6.Validating, txtKouza7.Validating, txtKouza8.Validating, txtKouza9.Validating, txtKouza10.Validating, _
                    txtKouza11.Validating, txtKouza12.Validating, txtKouza13.Validating, txtKouza14.Validating, txtKouza15.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub

    '振込金額用
    Private Sub Furikin_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtFuriKin1.Validating, txtFuriKin2.Validating, txtFuriKin3.Validating, txtFuriKin4.Validating, txtFuriKin5.Validating, _
            txtFuriKin6.Validating, txtFuriKin7.Validating, txtFuriKin8.Validating, txtFuriKin9.Validating, txtFuriKin10.Validating, _
            txtFuriKin11.Validating, txtFuriKin12.Validating, txtFuriKin13.Validating, txtFuriKin14.Validating, txtFuriKin15.Validating
        Try
            Dim txtFurikin As TextBox = CType(sender, TextBox)

            Dim No As Integer
            Select Case txtFurikin.Name
                Case txtFuriKin1.Name
                    No = 1
                Case txtFuriKin2.Name
                    No = 2
                Case txtFuriKin3.Name
                    No = 3
                Case txtFuriKin4.Name
                    No = 4
                Case txtFuriKin5.Name
                    No = 5
                Case txtFuriKin6.Name
                    No = 6
                Case txtFuriKin7.Name
                    No = 7
                Case txtFuriKin8.Name
                    No = 8
                Case txtFuriKin9.Name
                    No = 9
                Case txtFuriKin10.Name
                    No = 10
                Case txtFuriKin11.Name
                    No = 11
                Case txtFuriKin12.Name
                    No = 12
                Case txtFuriKin13.Name
                    No = 13
                Case txtFuriKin14.Name
                    No = 14
                Case txtFuriKin15.Name
                    No = 15
            End Select
            Dim Temp As String = GCom.NzDec(CType(sender, TextBox).Text, "")
            'すべての項目が空白の場合はチェックしない
            If txtKinNo(No).Text.Trim = "" AndAlso txtSitNo(No).Text.Trim = "" AndAlso txtKamokuCode(No).Text.Trim = "" _
              AndAlso txtKouza(No).Text.Trim = "" AndAlso txtKeiyakuName(No).Text.Trim = "" AndAlso txtFurikin.Text.Trim = "" Then
                If No = 15 Then btnHozon.Focus()
                Exit Sub
            End If

            '遷移先のオブジェクトを決定
            If No = 15 Then
                objNEXT_TXT = btnHozon
            Else
                If strSYOKITI_KIN <> "" Then '初期金融機関有り
                    If strSYOKITI_SIT <> "" Then '初期支店有り
                        objNEXT_TXT = txtKamokuCode(No + 1)
                    Else
                        objNEXT_TXT = txtSitNo(No + 1)
                    End If
                Else
                    objNEXT_TXT = txtKinNo(No + 1)
                End If
            End If

            '振込金額チェック
            If fn_CHECK_FURIKIN(txtKinNo(No), txtKinName(No), txtSitNo(No), txtSitName(No), txtKamokuCode(No), _
                                txtKouza(No), txtKeiyakuName(No), txtFurikin, lblBikou(No), objNEXT_TXT, lblIndex(No), _
                                intPAGE_CNT, No) Then
                CType(objNEXT_TXT, Control).Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("振込金額入力チェック", "失敗", ex.ToString)
        End Try
    End Sub
#End Region


End Class