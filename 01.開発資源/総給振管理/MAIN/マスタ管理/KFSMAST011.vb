' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
Option Explicit On
Option Strict On

Imports System.IO
Imports CASTCommon
Imports System.Text
Imports System.Data.OracleClient

Public Class KFSMAST011
    '共通イベントコントロール
    Private CAST As New CASTCommon.Events

    ' 許可文字指定
    Private CASTx As New CASTCommon.Events("0-9a-zA-Z", CASTCommon.Events.KeyMode.GOOD)
    Private CASTxx As New CASTCommon.Events("0-9a-zA-Z._-", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx02 As New CASTCommon.Events("0-9-", CASTCommon.Events.KeyMode.GOOD)
    Private CASTx03 As New CASTCommon.Events("0-9()-", CASTCommon.Events.KeyMode.GOOD)

    ' 非許可文字設定
    Private CASTx1 As New CASTCommon.Events(" ", CASTCommon.Events.KeyMode.BAD)
    Private CASTx2 As New CASTCommon.Events("", CASTCommon.Events.KeyMode.BAD)

    '取引先マスタ(総振)最大項目数(開始=Zero)
    'Private Const MaxColumns As Integer = 150   '(全151項目)
    Private Const MaxColumns As Integer = 267   '(全151項目 + 117項目)
    Private Const MaxColumns_Sub As Integer = 117

    '取引先マスタレコード項目情報       'Index 0 : 日本語項目名
    '                                   'Index 1 : ORACLE項目名
    '                                   'Index 2 : 項目属性(NUMBER, CHAR)
    '                                   'Index 3 : 変更前の値
    '                                   'Index 4 : 変更後の値
    '                                   'Index 5 : 備考(必須項目)
    Private sTORI(MaxColumns, 6) As String

    '取引先マスタレコード項目情報       'Control Object
    Private oTORI(MaxColumns) As Control

    '取引先マスタレコード項目情報           'Index 0 : 精度(NUMBER=PRECISION, CHAR=LENGTH)
    Private nTORI(MaxColumns, 1) As Integer 'Index 1 : Zero埋め有無(0=しない, 1=する)

    Private memTORI(MaxColumns, 1) As String    'ComboBox 値参照用配列
    '                                           'Index 0 : 既存値
    '                                           'Index 1 : 変更値

    Private Const 参照モード As String = "参　　照"
    Private Const 解除モード As String = "ロック解除"

    '更新時に取引先マスタメンテに出力する項目があるか判定
    Private updCnt As Integer

    Private MyOwnerForm As Form
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース
    Private MainLOG As New CASTCommon.BatchLOG("KFSMAST011", "取引先マスタメンテナンス画面")
    Private Const msgTitle As String = "取引先マスタメンテナンス画面(KFSMAST011)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure

    Private LW As LogWrite

    Private Jikinko As String = ""  '自金庫コード
    Private Center As String = ""   '信金･信組判定
    Private KESSAI As String = ""    '決済使用区分
    '2016/01/11 タスク）斎藤 RSV2金バッチ対応 ADD ---------------------------------------- START
    Private INI_RSV2_KINBATCH As String
    '2016/01/11 タスク）斎藤 RSV2金バッチ対応 ADD ---------------------------------------- END
    Private strCSV_FILE_NAME As String

    Private ErrMsgFlg As Boolean = True 'エラーメッセージ表示判定
    Private SyoriKBN As Integer '進行中の処理を特定する 0:登録 1:更新 2:参照 3:削除 4:取消

    '2013/11/27 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
    Private TAX As CASTCommon.ClsTAX
    '2013/11/27 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
    '2014/01/14 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
    Private Structure strcInshizeiLabelString
        Dim Inshizei1 As String
        Dim Inshizei2 As String
        Dim Inshizei3 As String

        Public Sub Init()
            Inshizei1 = "1万円未満"
            Inshizei2 = "1万円以上3万円未満"
            Inshizei3 = "3万円以上"
        End Sub
    End Structure
    Private InshizeiLabel As strcInshizeiLabelString
    '2014/01/14 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<
#Region " ロード"
    Private Sub KFSMAST011_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Me.CmdBack.DialogResult = DialogResult.None

            'ユーザID／システム日付表示
            Call GCom.SetMonitorTopArea(Label96, Label95, lblUser, lblDate)

            '設定ファイル取得
            If Not SetIniFIle() Then
                Return
            End If

            '2013/11/27 saitou 標準版 消費税対応 ADD -------------------------------------------------->>>>
            '税率取得
            Me.TAX = New CASTCommon.ClsTAX
            Me.TAX.GetZeiritsu()
            '2013/11/27 saitou 標準版 消費税対応 ADD --------------------------------------------------<<<<
            '2013/12/27 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
            Me.TAX.GetInshizei()
            '2014/01/14 saitou 標準版 印紙税対応 ADD -------------------------------------------------->>>>
            '帳票印刷用に印紙税の区分を変数に持つようにする
            Me.InshizeiLabel.Init()
            '2014/01/14 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<
            If Me.TAX.INSHIZEI_ID.Equals("err") = True Then
                '印紙税が登録されてない
                '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                Me.Label21.Text = Me.InshizeiLabel.Inshizei1
                Me.Label22.Text = Me.InshizeiLabel.Inshizei2
                Me.TESUU2_L.Text = Me.InshizeiLabel.Inshizei3
                'Me.Label21.Text = "1万円未満"
                'Me.Label22.Text = "1万円以上3万円未満"
                'Me.TESUU2_L.Text = "3万円以上"
                '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
            Else
                '金額によって表示形式を変える
                Dim strInshizei1 As String
                Dim strInshizei2 As String
                If Me.TAX.INSHIZEI1 >= 10000 Then
                    strInshizei1 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI1 / 10000) & "万"
                ElseIf Me.TAX.INSHIZEI1 >= 1000 Then
                    strInshizei1 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI1 / 1000) & "千"
                Else
                    strInshizei1 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI1)
                End If

                If Me.TAX.INSHIZEI2 >= 10000 Then
                    strInshizei2 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI2 / 10000) & "万"
                ElseIf Me.TAX.INSHIZEI1 >= 1000 Then
                    strInshizei2 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI2 / 1000) & "千"
                Else
                    strInshizei2 = String.Format("{0:#,##0}", Me.TAX.INSHIZEI2)
                End If

                '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                Me.InshizeiLabel.Inshizei1 = strInshizei1 & "円未満"
                Me.InshizeiLabel.Inshizei2 = strInshizei1 & "円以上" & strInshizei2 & "円未満"
                Me.InshizeiLabel.Inshizei3 = strInshizei2 & "円以上"
                Me.Label21.Text = Me.InshizeiLabel.Inshizei1
                Me.Label22.Text = Me.InshizeiLabel.Inshizei2
                Me.TESUU2_L.Text = Me.InshizeiLabel.Inshizei3
                'Me.Label21.Text = strInshizei1 & "円未満"
                'Me.Label22.Text = strInshizei1 & "円以上" & strInshizei2 & "円未満"
                'Me.TESUU2_L.Text = strInshizei2 & "円以上"
                '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
            End If
            '2013/12/27 saitou 標準版 印紙税対応 ADD --------------------------------------------------<<<<

            '初期表示設定
            Call SetInformation()
            If FormInitializa() = False Then
                Return
            End If

            '取引先コンボボックス設定
            If GCom.SelectItakuName("", cmbToriName, TORIS_CODE_T, TORIF_CODE_T, "3") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            'ボタンの活性･非活性化
            Me.CmdInsert.Enabled = True
            Me.CmdUpdate.Enabled = False
            Me.CmdSelect.Enabled = True
            Me.CmdDelete.Enabled = False
            Me.CmdCancel.Enabled = True
            Me.CmdBack.Enabled = True

            If KESSAI_Disused() = False Then
                Return
            End If

            If Center = "0" Then '信組の場合(互換機能)
                SYUMOKU_CODE_L.Visible = True
                SYUMOKU_CODE_T.Visible = True
                FUKA_CODE_L.Visible = True
                FUKA_CODE_T.Visible = True
            Else
                SYUMOKU_CODE_T.Size = New Size(85, 20)
            End If

            Me.TORIS_CODE_T.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " 設定ファイル取得"
    Private Function SetIniFIle() As Boolean
        Try
            '自金庫コード
            Jikinko = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If Jikinko.ToUpper = "ERR" OrElse Jikinko = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                Return False
            End If
            'センター区分
            Center = CASTCommon.GetFSKJIni("COMMON", "CENTER")
            If Center.ToUpper = "ERR" OrElse Center = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "センター区分", "COMMON", "CENTER"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:センター区分 分類:COMMON 項目:CENTER")
                Return False
            End If
            '決済使用区分
            KESSAI = CASTCommon.GetFSKJIni("OPTION", "SOU_KESSAI")
            If KESSAI.ToUpper = "ERR" OrElse KESSAI = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "総振決済使用区分", "OPTION", "SOU_KESSAI"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:総振決済使用区分 分類:OPTION 項目:SOU_KESSAI")
                Return False
            End If
            '2016/01/11 タスク）斎藤 RSV2金バッチ対応 ADD ---------------------------------------- START
            '金バッチ使用区分
            Me.INI_RSV2_KINBATCH = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KINBATCH")
            If Me.INI_RSV2_KINBATCH.ToUpper = "ERR" OrElse Me.INI_RSV2_KINBATCH = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "金バッチ使用区分", "RSV2_V1.0.0", "KINBATCH"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:金バッチ使用区分 分類:RSV2_V1.0.0 項目:KINBATCH")
                Return False
            End If
            '2016/01/11 タスク）斎藤 RSV2金バッチ対応 ADD ---------------------------------------- END

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(設定ファイル取得)", "失敗", ex.ToString)
        End Try
    End Function
#End Region

#Region " コントロールの設定値を編集"
    Private Sub SetInformation()
        Dim Index As Integer
        Try
            For Index = 0 To MaxColumns Step 1
                Select Case Index
                    Case Is = 0
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "振込処理区分"
                        sTORI(Index, 1) = "FSYORI_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = "3"
                        sTORI(Index, 4) = "3"
                        sTORI(Index, 5) = "1"
                    Case Is = 1
                        oTORI(Index) = TORIS_CODE_T
                        nTORI(Index, 0) = 10
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "取引先主コード"
                        sTORI(Index, 1) = "TORIS_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 2
                        oTORI(Index) = TORIF_CODE_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "取引先副コード"
                        sTORI(Index, 1) = "TORIF_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 3
                        oTORI(Index) = BAITAI_CODE_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "媒体コード"
                        sTORI(Index, 1) = "BAITAI_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 4
                        oTORI(Index) = LABEL_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "ラベル区分"
                        sTORI(Index, 1) = "LABEL_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 5
                        oTORI(Index) = CODE_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "コード区分"
                        sTORI(Index, 1) = "CODE_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 6
                        oTORI(Index) = ITAKU_KANRI_CODE_T
                        nTORI(Index, 0) = 10
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "代表委託者コード"
                        sTORI(Index, 1) = "ITAKU_KANRI_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 7
                        oTORI(Index) = FILE_NAME_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "ファイル名"
                        sTORI(Index, 1) = "FILE_NAME_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 8
                        oTORI(Index) = FMT_KBN_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "フォーマット区分"
                        sTORI(Index, 1) = "FMT_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 9
                        oTORI(Index) = MULTI_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "マルチ区分"
                        sTORI(Index, 1) = "MULTI_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 10
                        oTORI(Index) = NS_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "入出金区分"
                        sTORI(Index, 1) = "NS_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 11
                        oTORI(Index) = SYUBETU_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "種別"
                        sTORI(Index, 1) = "SYUBETU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 12
                        oTORI(Index) = ITAKU_CODE_T
                        nTORI(Index, 0) = 10
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "委託者コード"
                        sTORI(Index, 1) = "ITAKU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 13
                        oTORI(Index) = ITAKU_KNAME_T
                        nTORI(Index, 0) = 40
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "委託者名カナ"
                        sTORI(Index, 1) = "ITAKU_KNAME_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 14
                        oTORI(Index) = TKIN_NO_T
                        nTORI(Index, 0) = 4
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "取扱金融機関"
                        sTORI(Index, 1) = "TKIN_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 15
                        oTORI(Index) = TSIT_NO_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "取扱支店名"
                        sTORI(Index, 1) = "TSIT_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 16
                        oTORI(Index) = KAMOKU_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "科目"
                        sTORI(Index, 1) = "KAMOKU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 17
                        oTORI(Index) = KOUZA_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "口座番号"
                        sTORI(Index, 1) = "KOUZA_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 18
                        oTORI(Index) = SOUSIN_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "送信区分"
                        sTORI(Index, 1) = "SOUSIN_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 19
                        oTORI(Index) = SYUMOKU_CODE_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "種目コード"
                        sTORI(Index, 1) = "SYUMOKU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 20
                        oTORI(Index) = FUKA_CODE_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "付加コード"
                        sTORI(Index, 1) = "FUKA_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 21
                        oTORI(Index) = CYCLE_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "サイクル管理"
                        sTORI(Index, 1) = "CYCLE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 22
                        oTORI(Index) = KIJITU_KANRI_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "期日管理"
                        sTORI(Index, 1) = "KIJITU_KANRI_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 23
                        oTORI(Index) = ITAKU_NNAME_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "委託者名漢字"
                        sTORI(Index, 1) = "ITAKU_NNAME_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 24
                        oTORI(Index) = YUUBIN_T
                        nTORI(Index, 0) = 10
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "郵便番号"
                        sTORI(Index, 1) = "YUUBIN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 25
                        oTORI(Index) = DENWA_T
                        nTORI(Index, 0) = 12
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "電話番号"
                        sTORI(Index, 1) = "DENWA_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 26
                        oTORI(Index) = FAX_T
                        nTORI(Index, 0) = 12
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "ＦＡＸ番号"
                        sTORI(Index, 1) = "FAX_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 27
                        oTORI(Index) = KOKYAKU_NO_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "顧客番号"
                        sTORI(Index, 1) = "KOKYAKU_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 28
                        oTORI(Index) = KANREN_KIGYO_CODE_T
                        nTORI(Index, 0) = 4
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "関連企業情報"
                        sTORI(Index, 1) = "KANREN_KIGYO_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 29
                        oTORI(Index) = ITAKU_NJYU_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "委託者住所漢字"
                        sTORI(Index, 1) = "ITAKU_NJYU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 30
                        oTORI(Index) = YUUBIN_KNAME_T
                        nTORI(Index, 0) = 40
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "郵送先カナ"
                        sTORI(Index, 1) = "YUUBIN_KNAME_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 31
                        oTORI(Index) = YUUBIN_NNAME_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "郵送先漢字"
                        sTORI(Index, 1) = "YUUBIN_NNAME_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 32
                        oTORI(Index) = FURI_KYU_CODE_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "振込休日シフト"
                        sTORI(Index, 1) = "FURI_KYU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 33
                        oTORI(Index) = DATE1_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１日"
                        sTORI(Index, 1) = "DATE1_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 34
                        oTORI(Index) = DATE2_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２日"
                        sTORI(Index, 1) = "DATE2_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 35
                        oTORI(Index) = DATE3_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "３日"
                        sTORI(Index, 1) = "DATE3_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 36
                        oTORI(Index) = DATE4_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "４日"
                        sTORI(Index, 1) = "DATE4_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 37
                        oTORI(Index) = DATE5_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "５日"
                        sTORI(Index, 1) = "DATE5_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 38
                        oTORI(Index) = DATE6_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "６日"
                        sTORI(Index, 1) = "DATE6_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 39
                        oTORI(Index) = DATE7_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "７日"
                        sTORI(Index, 1) = "DATE7_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 40
                        oTORI(Index) = DATE8_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "８日"
                        sTORI(Index, 1) = "DATE8_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 41
                        oTORI(Index) = DATE9_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "９日"
                        sTORI(Index, 1) = "DATE9_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 42
                        oTORI(Index) = DATE10_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１０日"
                        sTORI(Index, 1) = "DATE10_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 43
                        oTORI(Index) = DATE11_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１１日"
                        sTORI(Index, 1) = "DATE11_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 44
                        oTORI(Index) = DATE12_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１２日"
                        sTORI(Index, 1) = "DATE12_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 45
                        oTORI(Index) = DATE13_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１３日"
                        sTORI(Index, 1) = "DATE13_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 46
                        oTORI(Index) = DATE14_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１４日"
                        sTORI(Index, 1) = "DATE14_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 47
                        oTORI(Index) = DATE15_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１５日"
                        sTORI(Index, 1) = "DATE15_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 48
                        oTORI(Index) = DATE16_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１６日"
                        sTORI(Index, 1) = "DATE16_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 49
                        oTORI(Index) = DATE17_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１７日"
                        sTORI(Index, 1) = "DATE17_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 50
                        oTORI(Index) = DATE18_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１８日"
                        sTORI(Index, 1) = "DATE18_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 51
                        oTORI(Index) = DATE19_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１９日"
                        sTORI(Index, 1) = "DATE19_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 52
                        oTORI(Index) = DATE20_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２０日"
                        sTORI(Index, 1) = "DATE20_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 53
                        oTORI(Index) = DATE21_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２１日"
                        sTORI(Index, 1) = "DATE21_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 54
                        oTORI(Index) = DATE22_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２２日"
                        sTORI(Index, 1) = "DATE22_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 55
                        oTORI(Index) = DATE23_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２３日"
                        sTORI(Index, 1) = "DATE23_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 56
                        oTORI(Index) = DATE24_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２４日"
                        sTORI(Index, 1) = "DATE24_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 57
                        oTORI(Index) = DATE25_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２５日"
                        sTORI(Index, 1) = "DATE25_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 58
                        oTORI(Index) = DATE26_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２６日"
                        sTORI(Index, 1) = "DATE26_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 59
                        oTORI(Index) = DATE27_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２７日"
                        sTORI(Index, 1) = "DATE27_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 60
                        oTORI(Index) = DATE28_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２８日"
                        sTORI(Index, 1) = "DATE28_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 61
                        oTORI(Index) = DATE29_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２９日"
                        sTORI(Index, 1) = "DATE29_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 62
                        oTORI(Index) = DATE30_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "３０日"
                        sTORI(Index, 1) = "DATE30_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 63
                        oTORI(Index) = DATE31_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "３１日"
                        sTORI(Index, 1) = "DATE31_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 64
                        oTORI(Index) = TUKI1_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１月"
                        sTORI(Index, 1) = "TUKI1_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 65
                        oTORI(Index) = TUKI2_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "２月"
                        sTORI(Index, 1) = "TUKI2_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 66
                        oTORI(Index) = TUKI3_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "３月"
                        sTORI(Index, 1) = "TUKI3_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 67
                        oTORI(Index) = TUKI4_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "４月"
                        sTORI(Index, 1) = "TUKI4_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 68
                        oTORI(Index) = TUKI5_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "５月"
                        sTORI(Index, 1) = "TUKI5_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 69
                        oTORI(Index) = TUKI6_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "６月"
                        sTORI(Index, 1) = "TUKI6_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 70
                        oTORI(Index) = TUKI7_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "７月"
                        sTORI(Index, 1) = "TUKI7_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 71
                        oTORI(Index) = TUKI8_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "８月"
                        sTORI(Index, 1) = "TUKI8_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 72
                        oTORI(Index) = TUKI9_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "９月"
                        sTORI(Index, 1) = "TUKI9_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 73
                        oTORI(Index) = TUKI10_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１０月"
                        sTORI(Index, 1) = "TUKI10_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 74
                        oTORI(Index) = TUKI11_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１１月"
                        sTORI(Index, 1) = "TUKI11_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 75
                        oTORI(Index) = TUKI12_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "１２月"
                        sTORI(Index, 1) = "TUKI12_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 76
                        oTORI(Index) = KEIYAKU_DATE_T
                        nTORI(Index, 0) = 8
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "契約日"
                        sTORI(Index, 1) = "KEIYAKU_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 77
                        oTORI(Index) = KAISI_DATE_T
                        nTORI(Index, 0) = 8
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "開始年月"
                        sTORI(Index, 1) = "KAISI_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 78
                        oTORI(Index) = SYURYOU_DATE_T
                        nTORI(Index, 0) = 8
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "終了年月"
                        sTORI(Index, 1) = "SYURYOU_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 79
                        oTORI(Index) = MOTIKOMI_KIJITSU_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "持込期日"
                        sTORI(Index, 1) = "MOTIKOMI_KIJITSU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 80
                        oTORI(Index) = IRAISYO_YDATE_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "日数/基準日(依頼書)"
                        sTORI(Index, 1) = "IRAISYO_YDATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 81
                        oTORI(Index) = IRAISYO_KIJITSU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "日付区分(依頼書)"
                        sTORI(Index, 1) = "IRAISYO_KIJITSU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 82
                        oTORI(Index) = IRAISYO_KYU_CODE_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "依頼書休日シフト"
                        sTORI(Index, 1) = "IRAISYO_KYU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 83
                        oTORI(Index) = IRAISYO_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "依頼書種別"
                        sTORI(Index, 1) = "IRAISYO_KBN_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 84
                        oTORI(Index) = IRAISYO_SORT_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "依頼書出力順"
                        sTORI(Index, 1) = "IRAISYO_SORT_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 85
                        oTORI(Index) = TEIGAKU_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "定額区分"
                        sTORI(Index, 1) = "TEIGAKU_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 86
                        oTORI(Index) = UMEISAI_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "受付明細表作成区分"
                        sTORI(Index, 1) = "UMEISAI_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 87
                        oTORI(Index) = KESSAI_KBN_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "決済区分"
                        sTORI(Index, 1) = "KESSAI_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 88
                        oTORI(Index) = KESSAI_PATN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "資金確保方法"
                        sTORI(Index, 1) = "KESSAI_PATN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 89
                        oTORI(Index) = TORIMATOME_SIT_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "とりまとめ店"
                        sTORI(Index, 1) = "TORIMATOME_SIT_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = "1"
                    Case Is = 90
                        oTORI(Index) = HONBU_KOUZA_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "本部別段口座番号"
                        sTORI(Index, 1) = "HONBU_KOUZA_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 91
                        oTORI(Index) = KESSAI_DAY_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "日数／基準日(決済)"
                        sTORI(Index, 1) = "KESSAI_DAY_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 92
                        oTORI(Index) = KESSAI_KIJITSU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "日付区分(決済)"
                        sTORI(Index, 1) = "KESSAI_KIJITSU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 93
                        oTORI(Index) = KESSAI_KYU_CODE_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "決済休日シフト"
                        sTORI(Index, 1) = "KESSAI_KYU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 94
                        oTORI(Index) = TUKEKIN_NO_T
                        nTORI(Index, 0) = 4
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "決済金融機関"
                        sTORI(Index, 1) = "TUKEKIN_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 95
                        oTORI(Index) = TUKESIT_NO_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "決済支店"
                        sTORI(Index, 1) = "TUKESIT_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 96
                        oTORI(Index) = TUKEKAMOKU_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "決済科目"
                        sTORI(Index, 1) = "TUKEKAMOKU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 97
                        oTORI(Index) = TUKEKOUZA_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "決済口座番号"
                        sTORI(Index, 1) = "TUKEKOUZA_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 98
                        oTORI(Index) = TUKEMEIGI_KNAME_T
                        nTORI(Index, 0) = 40
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "決済名義人（カナ）"
                        sTORI(Index, 1) = "TUKEMEIGI_KNAME_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 99
                        oTORI(Index) = BIKOU1_T
                        nTORI(Index, 0) = 29
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "備考１"
                        sTORI(Index, 1) = "BIKOU1_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 100
                        oTORI(Index) = BIKOU2_T
                        nTORI(Index, 0) = 29
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "備考２"
                        sTORI(Index, 1) = "BIKOU2_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 101
                        oTORI(Index) = TESUUTYO_SIT_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "手数料徴求支店"
                        sTORI(Index, 1) = "TESUUTYO_SIT_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 102
                        oTORI(Index) = TESUUTYO_KAMOKU_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "手数料徴求科目"
                        sTORI(Index, 1) = "TESUUTYO_KAMOKU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 103
                        oTORI(Index) = TESUUTYO_KOUZA_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料徴口座番号"
                        sTORI(Index, 1) = "TESUUTYO_KOUZA_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 104
                        oTORI(Index) = TESUUTYO_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料徴求区分"
                        sTORI(Index, 1) = "TESUUTYO_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 105
                        oTORI(Index) = TESUUTYO_PATN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料徴求方法"
                        sTORI(Index, 1) = "TESUUTYO_PATN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 106
                        oTORI(Index) = TESUUMAT_NO_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料集計周期"
                        sTORI(Index, 1) = "TESUUMAT_NO_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 107
                        oTORI(Index) = TESUUTYO_DAY_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "日数／基準日(手数料徴求)"
                        sTORI(Index, 1) = "TESUUTYO_DAY_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 108
                        oTORI(Index) = TESUUTYO_KIJITSU_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "日付区分(手数料徴求)"
                        sTORI(Index, 1) = "TESUUTYO_KIJITSU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 109
                        oTORI(Index) = TESUU_KYU_CODE_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "手数料休日シフト"
                        sTORI(Index, 1) = "TESUU_KYU_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 110
                        oTORI(Index) = SOURYO_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "送料"
                        sTORI(Index, 1) = "SOURYO_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 111
                        oTORI(Index) = KOTEI_TESUU1_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "固定手数料１"
                        sTORI(Index, 1) = "KOTEI_TESUU1_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 112
                        oTORI(Index) = KOTEI_TESUU2_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "固定手数料２"
                        sTORI(Index, 1) = "KOTEI_TESUU2_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 113
                        oTORI(Index) = TESUUMAT_MONTH_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "集計基準月"
                        sTORI(Index, 1) = "TESUUMAT_MONTH_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 114
                        oTORI(Index) = TESUUMAT_ENDDAY_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "集計終了日"
                        sTORI(Index, 1) = "TESUUMAT_ENDDAY_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 115
                        oTORI(Index) = TESUUMAT_KIJYUN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "集計基準"
                        sTORI(Index, 1) = "TESUUMAT_KIJYUN_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 116
                        oTORI(Index) = TESUUMAT_PATN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "集計方法"
                        sTORI(Index, 1) = "TESUUMAT_PATN_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 117
                        oTORI(Index) = TESUU_GRP_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "集計企業GRP"
                        sTORI(Index, 1) = "TESUU_GRP_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 118
                        oTORI(Index) = TESUU_TABLE_ID_T
                        ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
                        ' 振込手数料基準ＩＤの属性を変更（桁数:2、Zero埋め有、NUMBER型からCHAR型）
                        'nTORI(Index, 0) = 1
                        'nTORI(Index, 1) = 0
                        'sTORI(Index, 0) = "振込手数料基準ＩＤ"
                        'sTORI(Index, 1) = "TESUU_TABLE_ID_T"
                        'sTORI(Index, 2) = "NUMBER"
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "振込手数料基準ＩＤ"
                        sTORI(Index, 1) = "TESUU_TABLE_ID_T"
                        sTORI(Index, 2) = "CHAR"
                        ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 119
                        oTORI(Index) = TESUU_A1_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "自行" & Me.InshizeiLabel.Inshizei1 & "手数料"
                        'sTORI(Index, 0) = "自行1万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_A1_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 120
                        oTORI(Index) = TESUU_A2_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "自行" & Me.InshizeiLabel.Inshizei2 & "手数料"
                        'sTORI(Index, 0) = "自行1万以上3万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_A2_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 121
                        oTORI(Index) = TESUU_A3_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "自行" & Me.InshizeiLabel.Inshizei3 & "手数料"
                        'sTORI(Index, 0) = "自行3万以上"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_A3_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 122
                        oTORI(Index) = TESUU_B1_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "支店" & Me.InshizeiLabel.Inshizei1 & "手数料"
                        'sTORI(Index, 0) = "支店1万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_B1_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 123
                        oTORI(Index) = TESUU_B2_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "支店" & Me.InshizeiLabel.Inshizei2 & "手数料"
                        'sTORI(Index, 0) = "支店1万以上3万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_B2_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 124
                        oTORI(Index) = TESUU_B3_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "支店" & Me.InshizeiLabel.Inshizei3 & "手数料"
                        'sTORI(Index, 0) = "支店3万以上"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_B3_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 125
                        oTORI(Index) = ENC_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "暗号化処理区分"
                        sTORI(Index, 1) = "ENC_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 126
                        oTORI(Index) = ENC_OPT1_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "ＡＥＳオプション"
                        sTORI(Index, 1) = "ENC_OPT1_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 127
                        oTORI(Index) = ENC_KEY1_T
                        nTORI(Index, 0) = 64
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "暗号化キー１"
                        sTORI(Index, 1) = "ENC_KEY1_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 128
                        oTORI(Index) = ENC_KEY2_T
                        nTORI(Index, 0) = 32
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "暗号化キー２"
                        sTORI(Index, 1) = "ENC_KEY2_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 129
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 14
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "作成日"
                        sTORI(Index, 1) = "SAKUSEI_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 130
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 14
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "更新日"
                        sTORI(Index, 1) = "KOUSIN_DATE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 131
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備1"
                        sTORI(Index, 1) = "YOBI1_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 132
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備2"
                        sTORI(Index, 1) = "YOBI2_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 133
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備3"
                        sTORI(Index, 1) = "YOBI3_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 134
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備4"
                        sTORI(Index, 1) = "YOBI4_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 135
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備5"
                        sTORI(Index, 1) = "YOBI5_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 136
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備6"
                        sTORI(Index, 1) = "YOBI6_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 137
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備7"
                        sTORI(Index, 1) = "YOBI7_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 138
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備8"
                        sTORI(Index, 1) = "YOBI8_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 139
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備9"
                        sTORI(Index, 1) = "YOBI9_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 140
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "予備10"
                        sTORI(Index, 1) = "YOBI10_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 141
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "更新識別"
                        sTORI(Index, 1) = "KOUSIN_SIKIBETU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 142
                        oTORI(Index) = PRTNUM_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "印刷部数"
                        sTORI(Index, 1) = "PRTNUM_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 143
                        oTORI(Index) = TESUU_C1_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "他行" & Me.InshizeiLabel.Inshizei1 & "手数料"
                        'sTORI(Index, 0) = "他行1万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_C1_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 144
                        oTORI(Index) = TESUU_C2_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "他行" & Me.InshizeiLabel.Inshizei2 & "手数料"
                        'sTORI(Index, 0) = "他行1万以上3万未満手数料"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_C2_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 145
                        oTORI(Index) = TESUU_C3_T
                        nTORI(Index, 0) = 6
                        nTORI(Index, 1) = 0
                        '2014/01/14 saitou 標準版 印紙税対応 UPD -------------------------------------------------->>>>
                        '文言変更
                        sTORI(Index, 0) = "他行" & Me.InshizeiLabel.Inshizei3 & "手数料"
                        'sTORI(Index, 0) = "他行3万以上"
                        '2014/01/14 saitou 標準版 印紙税対応 UPD --------------------------------------------------<<<<
                        sTORI(Index, 1) = "TESUU_C3_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 146
                        oTORI(Index) = TEKIYOU_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "摘要区分"
                        sTORI(Index, 1) = "TEKIYOU_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 147
                        oTORI(Index) = KTEKIYOU_T
                        nTORI(Index, 0) = 13
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "カナ摘要"
                        sTORI(Index, 1) = "KTEKIYOU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 148
                        oTORI(Index) = NTEKIYOU_T
                        nTORI(Index, 0) = 12
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "漢字摘要"
                        sTORI(Index, 1) = "NTEKIYOU_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 149
                        oTORI(Index) = FURI_CODE_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "振替コード"
                        sTORI(Index, 1) = "FURI_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 150
                        oTORI(Index) = KIGYO_CODE_T
                        nTORI(Index, 0) = 4
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "企業コード"
                        sTORI(Index, 1) = "KIGYO_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 151
                        oTORI(Index) = AITE_CNT_CODE_T
                        nTORI(Index, 0) = 14
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "相手センター確認CD"
                        sTORI(Index, 1) = "AITE_CNT_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 152
                        oTORI(Index) = TOHO_CNT_CODE_T
                        nTORI(Index, 0) = 14
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "当方センター確認CD"
                        sTORI(Index, 1) = "TOHO_CNT_CODE_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 153
                        oTORI(Index) = DENSO_FILE_ID_T
                        nTORI(Index, 0) = 12
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "伝送ファイルＩＤ"
                        sTORI(Index, 1) = "DENSO_FILE_ID_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 154
                        oTORI(Index) = HANSOU_KBN_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "搬送方法"
                        sTORI(Index, 1) = "HANSOU_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 155
                        oTORI(Index) = HANSOU_ROOT1_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "搬送ルート１"
                        sTORI(Index, 1) = "HANSOU_ROOT1_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 156
                        oTORI(Index) = HANSOU_ROOT2_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "搬送ルート２"
                        sTORI(Index, 1) = "HANSOU_ROOT2_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 157
                        oTORI(Index) = HANSOU_ROOT3_T
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "搬送ルート３"
                        sTORI(Index, 1) = "HANSOU_ROOT3_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 158
                        oTORI(Index) = HENKYAKU_SIT_NO_T
                        nTORI(Index, 0) = 3
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "返却支店"
                        sTORI(Index, 1) = "HENKYAKU_SIT_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 159
                        oTORI(Index) = SYOUGOU_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "照合要否区分"
                        sTORI(Index, 1) = "SYOUGOU_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 160
                        oTORI(Index) = KEIYAKU_NO_T
                        nTORI(Index, 0) = 7
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "契約書番号"
                        sTORI(Index, 1) = "KEIYAKU_NO_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 161
                        oTORI(Index) = MAE_SYORI_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "個別前処理"
                        sTORI(Index, 1) = "MAE_SYORI_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 162
                        oTORI(Index) = ATO_SYORI_T
                        nTORI(Index, 0) = 2
                        nTORI(Index, 1) = 1
                        sTORI(Index, 0) = "個別後処理"
                        sTORI(Index, 1) = "ATO_SYORI_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 163
                        oTORI(Index) = TOKKIJIKOU1_T
                        nTORI(Index, 0) = 500
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "特記事項"
                        sTORI(Index, 1) = "TOKKIJIKOU1_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 164
                        oTORI(Index) = TOKKIJIKOU2_T
                        nTORI(Index, 0) = 500
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "受付特記事項"
                        sTORI(Index, 1) = "TOKKIJIKOU2_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 165
                        oTORI(Index) = TOKKIJIKOU3_T
                        nTORI(Index, 0) = 500
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "返却特記事項"
                        sTORI(Index, 1) = "TOKKIJIKOU3_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 166
                        oTORI(Index) = TOKKIJIKOU4_T
                        nTORI(Index, 0) = 500
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = "搬送特記事項"
                        sTORI(Index, 1) = "TOKKIJIKOU4_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 167
                        oTORI(Index) = KYUUYO_KBN_T
                        nTORI(Index, 0) = 1
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "KYUUYO_KBN_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 168
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM01_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 169
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM02_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 170
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM03_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 171
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM04_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 172
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM05_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 173
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM06_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 174
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM07_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 175
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM08_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 176
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM09_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 177
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM10_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 178
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM11_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 179
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM12_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 180
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM13_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 181
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM14_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 182
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM15_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 183
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM16_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 184
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM17_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 185
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM18_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 186
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM19_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 187
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM20_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 188
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM21_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 189
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM22_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 190
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM23_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 191
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM24_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 192
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM25_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 193
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM26_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 194
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM27_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 195
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM28_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 196
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM29_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 197
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM30_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 198
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM31_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 199
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM32_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 200
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM33_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 201
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM34_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 202
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM35_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 203
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM36_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 204
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM37_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 205
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM38_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 206
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM39_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 207
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM40_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 208
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM41_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 209
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM42_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 210
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM43_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 211
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM44_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 212
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM45_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 213
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM46_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 214
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM47_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 215
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM48_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 216
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM49_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 217
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 15
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_NUM50_T"
                        sTORI(Index, 2) = "NUMBER"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 218
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR01_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 219
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR02_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 220
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR03_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 221
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR04_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 222
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR05_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 223
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR06_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 224
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR07_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 225
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR08_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 226
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR09_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 227
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR10_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 228
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR11_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 229
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR12_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 230
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR13_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 231
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR14_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 232
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR15_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 233
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR16_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 234
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR17_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 235
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR18_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 236
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR19_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 237
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR20_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 238
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR21_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 239
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR22_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 240
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR23_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 241
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR24_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 242
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR25_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 243
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR26_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 244
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR27_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 245
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR28_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 246
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR29_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 247
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR30_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 248
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR31_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 249
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR32_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 250
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR33_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 251
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR34_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 252
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR35_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 253
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR36_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 254
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR37_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 255
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR38_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 256
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR39_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 257
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR40_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 258
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR41_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 259
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR42_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 260
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR43_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 261
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR44_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 262
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR45_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 263
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR46_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 264
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR47_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 265
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR48_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 266
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR49_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                    Case Is = 267
                        oTORI(Index) = Nothing
                        nTORI(Index, 0) = 50
                        nTORI(Index, 1) = 0
                        sTORI(Index, 0) = ""
                        sTORI(Index, 1) = "CUSTOM_VCR50_T"
                        sTORI(Index, 2) = "CHAR"
                        sTORI(Index, 3) = ""
                        sTORI(Index, 4) = ""
                        sTORI(Index, 5) = ""
                End Select
            Next Index
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region " フォーム上の値を初期化"

    '
    ' 機　能 : フォーム上の値を初期化
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 単純初期化 = 0, 参照時初期化 = 1
    '
    ' 備　考 : 初期化関数
    '    
    Private Function FormInitializa(Optional ByVal SEL As Integer = 0) As Boolean
        Dim CTL As Control
        Dim Index As Integer
        Dim Ret As Integer
        Dim CmbName As String = ""
        Dim TxtFileName As String = ""
        Dim Msg As String = ""
        Try
            FormInitializa = False

            For Index = 1 To MaxColumns Step 1
                CTL = oTORI(Index)
                Select Case True
                    Case (TypeOf CTL Is TextBox)
                        With CType(CTL, TextBox)
                            If FormInitializeSub(SEL, .Name) Then
                                .Text = ""
                            End If
                            .BackColor = System.Drawing.SystemColors.Window
                            '初期値設定
                            Select Case .Name.ToUpper
                                Case Is = "TORIF_CODE_T"
                                    If FormInitializeSub(SEL, .Name) Then
                                        .Text = ""
                                    End If
                                Case Is = "KIGYO_CODE_T"
                                    .Text = ""
                                Case Is = "FURI_CODE_T"
                                    .Text = ""
                                Case Is = "ITAKU_KANRI_CODE_T"
                                    .Text = ""
                                Case Is = "KOKYAKU_NO_T"
                                    .Text = ""
                                Case Is = "TKIN_NO_T"
                                    .Text = Jikinko
                                Case Is = "TUKEKIN_NO_T"
                                    .Text = Jikinko
                                Case Is = "KEIYAKU_DATE_T"
                                    .Text = "2000"
                                    KEIYAKU_DATE_T1.Text = "01"
                                    KEIYAKU_DATE_T2.Text = "01"
                                Case Is = "KAISI_DATE_T"
                                    .Text = "2000"
                                    KAISI_DATE_T1.Text = "01"
                                    KAISI_DATE_T2.Text = "01"
                                Case Is = "SYURYOU_DATE_T"
                                    .Text = "2099"
                                    SYURYOU_DATE_T1.Text = "12"
                                    SYURYOU_DATE_T2.Text = "31"
                                Case Is = "MOTIKOMI_KIJITSU_T"
                                    .Text = "03"
                                Case Is = "KESSAI_DAY_T", "TESUUTYO_DAY_T"
                                    .Text = "01"
                                Case Is = "SOURYO_T"
                                    .Text = "0"
                                Case Is = "KOTEI_TESUU1_T", "KOTEI_TESUU2_T"
                                    .Text = "0"
                                Case Is = "FKEKKA_TBL_T"
                                    .Text = "0"
                                Case Is = "TORIMATOME_SIT_T", "TUKESIT_NO_T", "TESUUTYO_SIT_NO_T"
                                Case Is = "HONBU_KOUZA_T", "TUKEKOUZA_T", "TESUUTYO_KOUZA_T"
                                    .Text = ""
                                Case Is = "KIHTESUU_T"
                                    .Text = "0"
                                Case Is = "TESUUMAT_NO_T"
                                    .Text = "0"
                                Case Is = "AITE_CNT_CODE_T", "TOHO_CNT_CODE_T", "DENSO_FILE_ID_T"
                                    .Text = ""
                                Case Is = "HANSOU_ROOT1_T", "HANSOU_ROOT2_T", "HANSOU_ROOT3_T", "HENKYAKU_SIT_NO_T", "KEIYAKU_NO_T"
                                    .Text = ""
                                Case Is = "TOKKIJIKOU1_T", "TOKKIJIKOU2_T", "TOKKIJIKOU3_T", "TOKKIJIKOU4_T"
                                    .Text = ""
                            End Select
                        End With
                    Case (TypeOf CTL Is ComboBox)
                        Dim CMB As ComboBox = CType(CTL, ComboBox)
                        With CMB
                            .Items.Clear()
                            .Text = ""
                            Select Case .Name.ToUpper
                                Case "BAITAI_CODE_T"
                                    CmbName = "媒体コード"
                                    TxtFileName = "Common_総振_媒体コード.TXT"
                                    Ret = GCom.SetComboBox(CMB, "Common_総振_媒体コード.TXT", True)
                                Case "LABEL_KBN_T"
                                    CmbName = "ラベル区分"
                                    TxtFileName = "KFSMAST010_ラベル区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_ラベル区分.TXT", True)
                                Case "CODE_KBN_T"
                                    CmbName = "コード区分"
                                    TxtFileName = "Common_コード区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "Common_コード区分.TXT", True)
                                Case "KIJITU_KANRI_T"
                                    CmbName = "期日管理"
                                    TxtFileName = "KFSMAST010_期日管理.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_期日管理.TXT", True)
                                Case "CYCLE_T"
                                    CmbName = "サイクル管理"
                                    TxtFileName = "KFSMAST010_サイクル管理.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_サイクル管理.TXT", True)
                                Case "FMT_KBN_T"
                                    CmbName = "フォーマット区分"
                                    TxtFileName = "Common_総振_フォーマット区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "Common_総振_フォーマット区分.TXT", True)
                                Case "MULTI_KBN_T"
                                    CmbName = "マルチ区分"
                                    TxtFileName = "KFSMAST010_マルチ区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_マルチ区分.TXT", 0)
                                Case "NS_KBN_T"
                                    CmbName = "入出金区分"
                                    TxtFileName = "KFSMAST010_入出金区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_入出金区分.TXT", 1)
                                Case "SYUBETU_T"
                                    CmbName = "種別"
                                    TxtFileName = "KFSMAST010_種別.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_種別.TXT", 21)
                                Case "KAMOKU_T"
                                    CmbName = "科目"
                                    TxtFileName = "Common_総振_科目.TXT"
                                    Ret = GCom.SetComboBox(CMB, "Common_総振_科目.TXT", 1)
                                Case "SOUSIN_KBN_T"
                                    CmbName = "送信区分"
                                    '信組考慮
                                    If Center = "0" Then
                                        TxtFileName = "KFSMAST010_送信区分(信組).TXT"
                                        Ret = GCom.SetComboBox(CMB, "KFSMAST010_送信区分(信組).TXT", 0)
                                    Else
                                        TxtFileName = "KFSMAST010_送信区分.TXT"
                                        Ret = GCom.SetComboBox(CMB, "KFSMAST010_送信区分.TXT", 0)
                                    End If
                                Case "SYUMOKU_CODE_T"
                                    CmbName = "種目コード"
                                    '信組考慮
                                    If Center = "0" Then
                                        TxtFileName = "KFSMAST010_種目コード(信組).TXT"
                                        Ret = GCom.SetComboBox(CMB, "KFSMAST010_種目コード(信組).TXT", True)
                                    Else
                                        TxtFileName = "KFSMAST010_種目コード.TXT"
                                        Ret = GCom.SetComboBox(CMB, "KFSMAST010_種目コード.TXT", True)
                                    End If
                                Case "FUKA_CODE_T"
                                    CmbName = "付加コード"
                                    TxtFileName = "KFSMAST010_付加コード.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_付加コード.TXT", True)
                                Case "FURI_KYU_CODE_T"
                                    CmbName = "振込休日シフト"
                                    TxtFileName = "KFSMAST010_振込休日シフト.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_振込休日シフト.TXT", 0)
                                Case "IRAISYO_KIJITSU_T"
                                    CmbName = "日付区分(依頼書)"
                                    TxtFileName = "KFSMAST010_日付区分(依頼書).TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_日付区分(依頼書).TXT", True)
                                Case "IRAISYO_KYU_CODE_T"
                                    CmbName = "依頼書休日シフト"
                                    TxtFileName = "KFSMAST010_依頼書休日シフト.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_依頼書休日シフト.TXT", True)
                                Case "IRAISYO_KBN_T"
                                    CmbName = "依頼書種別"
                                    TxtFileName = "Common_総振_依頼書種別.TXT"
                                    Ret = GCom.SetComboBox(CMB, "Common_総振_依頼書種別.TXT", True)
                                Case "IRAISYO_SORT_T"
                                    CmbName = "依頼書出力順"
                                    TxtFileName = "KFSMAST010_依頼書出力順.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_依頼書出力順.TXT", True)
                                Case "TEIGAKU_KBN_T"
                                    CmbName = "定額区分"
                                    TxtFileName = "KFSMAST010_定額区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_定額区分.TXT", 0)
                                Case "UMEISAI_KBN_T"
                                    CmbName = "受付明細表出力区分"
                                    TxtFileName = "KFSMAST010_受付明細表出力区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_受付明細表出力区分.TXT", 0)
                                Case "KESSAI_KBN_T"
                                    CmbName = "決済区分"
                                    TxtFileName = "KFSMAST010_決済区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_決済区分.TXT", 99)
                                    '決済使用区分=2の場合のみ対象外以外可能
                                    If KESSAI <> "2" Then
                                        Call GCom.RemoveComboItem(CMB, New Integer(0) {99})
                                    End If
                                    CMB.SelectedIndex = 0
                                Case "KESSAI_PATN_T"
                                    CmbName = "資金確保方法"
                                    TxtFileName = "KFSMAST010_資金確保方法.TXT"
                                    '2016/01/11 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- START
                                    If Me.INI_RSV2_KINBATCH.Equals("1") = True Then
                                        '金バッチを使用する
                                        Ret = GCom.SetComboBox(CMB, "KFSMAST010_資金確保方法.TXT", True)
                                    Else
                                        '金バッチを使用しない
                                        Ret = GCom.SetComboBox(CMB, "KFSMAST010_資金確保方法.TXT", 2)
                                        '資金確保は使用不可にする
                                        Call GCom.RemoveComboItem(CMB, New Integer(0) {2})
                                    End If
                                    'Ret = GCom.SetComboBox(CMB, "KFSMAST010_資金確保方法.TXT", 2)
                                    ''資金確保は使用不可にする
                                    'Call GCom.RemoveComboItem(CMB, New Integer(0) {2})
                                    '2016/01/11 タスク）斎藤 RSV2金バッチ対応 UPD ---------------------------------------- END
                                Case "KESSAI_KIJITSU_T"
                                    CmbName = "日付区分(決済)"
                                    TxtFileName = "KFSMAST010_日付区分(決済).TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_日付区分(決済).TXT", 0)
                                Case "KESSAI_KYU_CODE_T"
                                    CmbName = "決済休日シフト"
                                    TxtFileName = "KFSMAST010_決済休日シフト.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_決済休日シフト.TXT", True)
                                Case "TUKEKAMOKU_T"
                                    CmbName = "決済科目"
                                    TxtFileName = "Common_決済科目.TXT"
                                    Ret = GCom.SetComboBox(CMB, "Common_決済科目.TXT", 1)
                                Case "TESUUTYO_KAMOKU_T"
                                    CmbName = "手数料徴求科目"
                                    TxtFileName = "Common_手数料徴求科目.TXT"
                                    Ret = GCom.SetComboBox(CMB, "Common_手数料徴求科目.TXT", 1)
                                Case "TESUUTYO_KBN_T"
                                    CmbName = "手数料徴求区分"
                                    TxtFileName = "KFSMAST010_手数料徴求区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_手数料徴求区分.TXT", 0)
                                Case "TESUUTYO_PATN_T"
                                    CmbName = "手数料徴求方法"
                                    TxtFileName = "KFSMAST010_手数料徴求方法.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_手数料徴求方法.TXT", 0)
                                Case "TESUUTYO_KIJITSU_T"
                                    CmbName = "日付区分(手数料徴求)"
                                    TxtFileName = "KFSMAST010_日付区分(手数料徴求).TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_日付区分(手数料徴求).TXT", 0)
                                Case "TESUU_KYU_CODE_T"
                                    CmbName = "手数料休日シフト"
                                    TxtFileName = "KFSMAST010_手数料休日シフト.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_手数料休日シフト.TXT", 0)
                                Case "TESUUMAT_KIJYUN_T"
                                    CmbName = "集計基準"
                                    TxtFileName = "KFSMAST010_集計基準.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_集計基準.TXT", True)
                                Case "TESUUMAT_PATN_T"
                                    CmbName = "集計方法"
                                    TxtFileName = "KFSMAST010_集計方法.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_集計方法.TXT", True)
                                Case "TESUU_TABLE_ID_T"
                                    CmbName = "振込手数料基準ID"
                                    '2013/11/27 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
                                    Ret = GCom.SetComboBox_TESUU_TABLE_ID_T(CMB, TAX.ZEIRITSU_ID, -1, "3")
                                    'TxtFileName = "KFSMAST010_振込手数料基準ID.TXT"
                                    'Ret = GCom.SetComboBox(CMB, "KFSMAST010_振込手数料基準ID.TXT", -1)
                                    '2013/11/27 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<
                                Case "ENC_KBN_T"
                                    CmbName = "暗号化処理区分"
                                    TxtFileName = "KFSMAST010_暗号化処理区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_暗号化処理区分.TXT", True)
                                Case "ENC_OPT1_T"
                                    CmbName = "ＡＥＳオプション"
                                    TxtFileName = "KFSMAST010_ＡＥＳオプション.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_ＡＥＳオプション.TXT", True)
                                Case "PRTNUM_T"
                                    CmbName = "印刷部数"
                                    TxtFileName = "KFSMAST010_印刷部数.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_印刷部数.TXT", True)
                                Case "TEKIYOU_KBN_T"
                                    CmbName = "摘要区分"
                                    TxtFileName = "KFSMAST010_摘要区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST010_摘要区分.TXT", True)
                                Case "HANSOU_KBN_T"
                                    CmbName = "搬送方法"
                                    TxtFileName = "KFSMAST011_搬送方法.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST011_搬送方法.TXT", True)
                                Case "SYOUGOU_KBN_T"
                                    CmbName = "照合要否区分"
                                    TxtFileName = "KFSMAST011_照合要否区分.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST011_照合要否区分.TXT", True)
                                Case "MAE_SYORI_T"
                                    CmbName = "個別前処理"
                                    TxtFileName = "KFSMAST011_個別前処理.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST011_個別前処理.TXT", True)
                                Case "ATO_SYORI_T"
                                    CmbName = "個別後処理"
                                    TxtFileName = "KFSMAST011_個別後処理.TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST011_個別後処理.TXT", True)
                                Case "KYUUYO_KBN_T"
                                    CmbName = "給与摘要区分"
                                    TxtFileName = "KFSMAST011_給与摘要区分(総振).TXT"
                                    Ret = GCom.SetComboBox(CMB, "KFSMAST011_給与摘要区分(総振).TXT", True)
                            End Select

                            Select Case Ret
                                Case 1  'ファイルなし
                                    Msg = String.Format(MSG0025E, CmbName, TxtFileName)
                                    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Msg = CmbName & "設定ファイルなし。ファイル:" & TxtFileName
                                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(初期表示関数)", "失敗", Msg)
                                    Return False
                                Case 2  '異常
                                    Msg = String.Format(MSG0026E, CmbName)
                                    MessageBox.Show(Msg.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Msg = "コンボボックス設定失敗 コンボボックス名:" & CmbName
                                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(初期表示関数)", "失敗", Msg)
                                    Return False
                            End Select
                            CMB.BackColor = System.Drawing.SystemColors.Window
                        End With
                    Case (TypeOf CTL Is CheckBox)
                        With CType(CTL, CheckBox)
                            .Checked = False
                            .BackColor = System.Drawing.SystemColors.Control
                        End With
                    Case (TypeOf CTL Is Label)
                        With CType(CTL, Label)
                            If Not GCom.NzInt(.Tag, 0) = 6 Then
                                .Text = ""
                                .BackColor = System.Drawing.Color.LemonChiffon
                            End If
                        End With
                End Select

                '変更後の格納エリアを初期化する
                sTORI(Index, 4) = ""

                ''ラベルの文字色を調整する(必須項目判定)
                'Call GetLabelColor(Index)
            Next Index

            '配列外のコントロールを設定する
            Dim EachObj() As Control = {KEIYAKU_DATE_T1, KEIYAKU_DATE_T2, KAISI_DATE_T1, KAISI_DATE_T2, _
                                       SYURYOU_DATE_T1, SYURYOU_DATE_T2, TORIS_CODE_T, TORIF_CODE_T}

            For Each CTL In EachObj
                If TypeOf CTL Is TextBox Then
                    CTL.BackColor = System.Drawing.SystemColors.Window
                    If GCom.NzInt(CTL.Tag, 0) = 0 AndAlso FormInitializeSub(SEL, CTL.Name) Then
                        CTL.Text = ""
                    End If
                Else
                    CTL.Text = ""
                End If
            Next CTL

            'ラベル初期化
            For Index = 0 To 3 Step 1
                For Each CTL In SSTab.TabPages(Index).Controls
                    If TypeOf CTL Is GroupBox Then
                        For Each OBJ As Control In CTL.Controls
                            If TypeOf OBJ Is Label AndAlso GCom.NzInt(OBJ.Tag, 0) = 1 Then
                                OBJ.Text = ""
                            End If
                        Next OBJ
                    End If
                Next CTL
            Next Index

            'ラベル初期化
            For Each CTL In Me.Controls
                If TypeOf CTL Is Label AndAlso GCom.NzInt(CTL.Tag, 0) = 1 Then
                    CTL.Text = ""
                End If
            Next CTL
            TKIN_NO_TL.Text = GCom.GetBKBRName(Jikinko, "", 30)
            TUKEKIN_NO_TL.Text = GCom.GetBKBRName(Jikinko, "", 30)

            Call SetControlToReadOnly(True)
            FormInitializa = True
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    '
    ' 機　能 : 初期化時の有効無効設定
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 初期化時の場合に=True
    '
    ' 備　考 : なし
    '    
    Private Sub SetControlToReadOnly(ByVal onBoolean As Boolean)
        Try
            With Me
                .CmdInsert.Enabled = (onBoolean)
                .CmdUpdate.Enabled = (Not onBoolean)
                .CmdDelete.Enabled = (Not onBoolean)
                .TORIS_CODE_T.ReadOnly = (Not onBoolean)
                .TORIF_CODE_T.ReadOnly = (Not onBoolean)
                .CmdSelect.Text = IIf(onBoolean, 参照モード, 解除モード).ToString
            End With
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        End Try
    End Sub

#End Region

#Region " 登録ボタン処理"
    Private Sub CmdInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdInsert.Click
        SyoriKBN = 0
        Dim MSG As String
        Try
            MainDB = New MyOracle
            If Not TORIS_CODE_T.Text.Trim = "" AndAlso Not TORIF_CODE_T.Text.Trim = "" Then
                LW.ToriCode = TORIS_CODE_T.Text & TORIF_CODE_T.Text
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)開始", "成功", "")

            Me.SuspendLayout()

            If KESSAI_Disused() = False Then
                Return
            End If
            '設定値の参照(取得)
            Call GetControlsValue()

            '登録可能な取引先コードかチェックする
            Select Case TORIS_CODE_T.Text + TORIF_CODE_T.Text
                Case "000000000000", "111111111111", "222222222222", "333333333333", _
                     "444444444444", "555555555555", "666666666666", "777777777777", _
                     "888888888888", "999999999999"
                    Call MessageBox.Show(MSG0348W, msgTitle, _
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                Case Else

            End Select

            '参照値の検査(主に相関チェック)
            MSG = ""
            Dim Ret As Integer = CheckMutualRelation(MSG)
            If Ret = 0 Then


                '変更登録の有無チェック関数を使って印刷用CSVを出力する。
                If Not CheckUpdateItem(0) Then

                    MSG = MSG0040I
                    Call MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Information)

                    Call SetFocusOnErrorControl(GetIndex("BAITAI_CODE_T"))
                    Return
                End If
            Else
                If ErrMsgFlg = True Then    '不要な場合は表示しない
                    If MSG = "" Then
                        MSG = String.Format(MSG0281W, sTORI(Ret, 0))
                    End If
                    Call MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If

                Application.DoEvents()
                Call SetFocusOnErrorControl(Ret)
                Return
            End If

            '先行レコードのチェック
            Select Case CheckRecordModule()
                Case 1
                    MSG = MSG0149W

                    Call MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case 0
                    '登録処理
                    If MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons. _
                        YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                        Return
                    End If

                    MainDB.BeginTrans()
                    If InsertModule() Then
                        MainDB.Commit()
                        'コンボボックス再設定
                        If GCom.SelectItakuName("", cmbToriName, TORIS_CODE_T, TORIF_CODE_T, "3") = -1 Then
                            MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Else
                            TORIS_CODE_T.Text = Mid(LW.ToriCode, 1, 10)
                            TORIF_CODE_T.Text = Mid(LW.ToriCode, 11, 2)
                        End If

                        MSG = MSG0004I

                        Call MessageBox.Show(MSG, msgTitle, _
                                MessageBoxButtons.OK, MessageBoxIcon.Information)

                        '印刷処理を起動する
                        If fn_CreateCSV_INSERT() Then
                            Call fn_Print(1)
                        End If
                    Else
                        MainDB.Rollback()
                    End If
                Case Else
                    MSG = String.Format(MSG0002E, "検索")

                    Call MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.ResumeLayout()
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
        End Try
    End Sub
#End Region

#Region " 更新ボタン処理"
    Private Sub CmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdUpdate.Click
        Dim MSG As String
        SyoriKBN = 1
        Try
            LW.ToriCode = Me.TORIS_CODE_T.Text.PadLeft(10, "0"c) & "-" & Me.TORIF_CODE_T.Text.PadLeft(2, "0"c)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            MainDB = New MyOracle
            Me.SuspendLayout()

            If KESSAI_Disused() = False Then
                Return
            End If
            '設定値の参照(取得)
            Call GetControlsValue()

            '参照値の検査(主に相関チェック)
            MSG = ""
            Dim Ret As Integer = CheckMutualRelation(MSG)
            If Ret = 0 Then

                '変更登録の有無チェック
                If Not CheckUpdateItem(1) Then
                    MSG = MSG0040I
                    Call MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Information)

                    Call SetFocusOnErrorControl(GetIndex("BAITAI_CODE_T"))
                    Return
                End If
            Else
                If ErrMsgFlg = True Then '不要な場合は表示しない
                    If MSG = "" Then
                        MSG = String.Format(MSG0281W, sTORI(Ret, 0))
                    End If
                    Call MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If


                Application.DoEvents()
                Call SetFocusOnErrorControl(Ret)
                Return
            End If

            '先行レコードのチェック
            Select Case CheckRecordModule()
                Case 1
                    '更新処理
                    If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons. _
                       YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                        Return
                    End If
                    MainDB.BeginTrans()
                    If UpdateModule() Then

                        MainDB.Commit()
                        MSG = MSG0006I

                        Call MessageBox.Show(MSG, msgTitle, _
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

                        Dim CmbItem As String = cmbToriName.SelectedItem.ToString
                        If GCom.SelectItakuName("", cmbToriName, TORIS_CODE_T, TORIF_CODE_T, "3") = -1 Then
                            MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                        For No As Integer = 0 To cmbToriName.Items.Count - 1
                            If cmbToriName.Items.Item(No).ToString = CmbItem Then
                                cmbToriName.SelectedIndex = No
                                Exit For
                            End If
                        Next
                        TORIS_CODE_T.Text = sTORI(GetIndex("TORIS_CODE_T"), 3)
                        TORIF_CODE_T.Text = sTORI(GetIndex("TORIF_CODE_T"), 3)

                        '印刷処理を起動する(失敗しても以降の処理は実行する)    
                        Call fn_Print(2)

                        '取り敢えずロック解除モードにする。
                        Call SetControlToReadOnly(True)
                        Me.TORIS_CODE_T.Focus()
                    Else
                        MainDB.Rollback()

                    End If
                Case 0
                    MSG = MSG0063W

                    Call MessageBox.Show(MSG, msgTitle, _
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    MSG = String.Format(MSG0002E, "検索")

                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End Select

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.ResumeLayout()
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
        End Try
    End Sub
#End Region

#Region " 参照ボタン処理"
    Private Sub CmdSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdSelect.Click
        Try
            SyoriKBN = 2
            LW.ToriCode = Me.TORIS_CODE_T.Text.PadLeft(10, "0"c) & "-" & Me.TORIF_CODE_T.Text.PadLeft(2, "0"c)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            Me.SuspendLayout()
            Dim CTL As TextBox
            Select Case CmdSelect.Text
                Case 参照モード
                    Select Case True
                        Case Not Me.TORIS_CODE_T.Text.Length = 10
                            CTL = Me.TORIS_CODE_T
                            MessageBox.Show(String.Format(MSG0285W, "取引先主コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            TORIS_CODE_T.Focus()
                        Case Not Me.TORIF_CODE_T.Text.Length = 2
                            CTL = Me.TORIF_CODE_T
                            TORIF_CODE_T.Focus()
                            MessageBox.Show(String.Format(MSG0285W, "取引先副コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Case Else
                            Call FormInitializa(1)
                            If CmdSelect_Action() = False Then
                                Return
                            End If

                            Call SetControlToReadOnly(False)
                    End Select
                Case 解除モード
                    Call SetControlToReadOnly(True)
                    SSTab.SelectedIndex = 0
                    Me.TORIS_CODE_T.Focus()
            End Select
            If KESSAI_Disused() = False Then
                Return
            End If
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.ResumeLayout()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
        End Try
    End Sub
#End Region

#Region " 削除ボタン処理"
    Private Sub CmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdDelete.Click
        SyoriKBN = 3
        Dim MSG As String
        Dim SQL As New StringBuilder(128)
        Dim Print As Boolean = False
        Dim REC As OracleDataReader = Nothing
        Try
            MainDB = New MyOracle
            LW.ToriCode = Me.TORIS_CODE_T.Text.PadLeft(10, "0"c) & "-" & Me.TORIF_CODE_T.Text.PadLeft(2, "0"c)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)開始", "成功", "")

            MSG = MSG0007I
            If DialogResult.OK = MessageBox.Show(MSG, msgTitle, _
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) Then


                'スケジュールマスタに進行中のレコードが有るか否かを検索する。
                SQL.AppendLine("SELECT COUNT(*) COUNTER FROM S_SCHMAST")
                SQL.AppendLine(" WHERE FSYORI_KBN_S = " & SQ(sTORI(GetIndex("FSYORI_KBN_T"), 3)))
                SQL.AppendLine(" AND TORIS_CODE_S = " & SQ(TORIS_CODE_T.Text.Trim))
                SQL.AppendLine(" AND TORIF_CODE_S = " & SQ(TORIF_CODE_T.Text.Trim))
                SQL.AppendLine(" AND TOUROKU_FLG_S = '1'")                                   '登録中
                SQL.AppendLine(" AND NVL(TYUUDAN_FLG_S, '0') = '0'")                         '中断なし
                SQL.AppendLine(" AND HASSIN_FLG_S = '0'")                                    '未発信

                If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then

                    If GCom.NzInt(REC.Item("COUNTER"), 0) > 0 Then
                        MSG = MSG0295W
                        MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                Else
                    MSG = String.Format(MSG0002E, "検索")

                    MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                '取引先マスタ削除情報印刷
                Print = fn_CreateCSV_Delete()

                '==================================================
                ' 取引先マスタ <S_TORIMAST> の削除
                '==================================================
                SQL = New StringBuilder(128)
                SQL.Length = 0
                SQL.AppendLine("DELETE FROM S_TORIMAST")
                SQL.AppendLine(" WHERE FSYORI_KBN_T = " & SQ(sTORI(GetIndex("FSYORI_KBN_T"), 3)))
                SQL.AppendLine(" AND TORIS_CODE_T   = " & SQ(sTORI(GetIndex("TORIS_CODE_T"), 3)))
                SQL.AppendLine(" AND TORIF_CODE_T   = " & SQ(sTORI(GetIndex("TORIF_CODE_T"), 3)))

                Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)
                If Ret = 1 Then
                    '==================================================
                    ' 取引先マスタ(固有情報) <S_TORIMAST_SUB> の削除
                    '==================================================
                    SQL.Length = 0
                    SQL.AppendLine("DELETE FROM S_TORIMAST_SUB")
                    SQL.AppendLine(" WHERE FSYORI_KBN_TSUB = " & SQ(sTORI(GetIndex("FSYORI_KBN_T"), 3)))
                    SQL.AppendLine(" AND TORIS_CODE_TSUB   = " & SQ(sTORI(GetIndex("TORIS_CODE_T"), 3)))
                    SQL.AppendLine(" AND TORIF_CODE_TSUB   = " & SQ(sTORI(GetIndex("TORIF_CODE_T"), 3)))

                    Ret = MainDB.ExecuteNonQuery(SQL)
                    If Ret = 1 Then
                        MainDB.Commit()
                        If GCom.SelectItakuName("", cmbToriName, TORIS_CODE_T, TORIF_CODE_T, "3") = -1 Then
                            MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If

                        MSG = MSG0008I
                        MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                        '---------------------------------------------------
                        '印刷実行
                        '---------------------------------------------------
                        If Print = True Then
                            Call fn_Print(3)
                        Else
                            MSG = String.Format(MSG0231W, "取引先マスタメンテ(削除)")
                            MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    Else
                        MainDB.Rollback()
                        MSG = String.Format(MSG0002E, "削除")
                        MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End If
                Else
                    MainDB.Rollback()
                    MSG = String.Format(MSG0002E, "削除")
                    MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                Call FormInitializa()
                Call SetControlToReadOnly(True)
                SSTab.SelectedIndex = 0
                Me.TORIS_CODE_T.Focus()
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(削除)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
        End Try
    End Sub
#End Region

#Region " 取消ボタン処理"
    Private Sub CmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdCancel.Click
        Try
            SyoriKBN = 4
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")
            '画面を初期化
            Call FormInitializa()
            'ロック解除する
            Call SetControlToReadOnly(True)
            '取引先検索コンボボックスの一番上を表示する
            cmbKana.SelectedIndex = 0
            cmbToriName.SelectedIndex = 0
            '最初のタブ、最初の入力場所にフォーカス設定
            SSTab.SelectedIndex = 0
            Me.TORIS_CODE_T.Focus()
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
            LW.ToriCode = "0000000000-00"
        End Try
    End Sub
#End Region

#Region " 終了ボタン処理 "
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click
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
#End Region

#Region " 全部選択／解除(CheckBox)ボタン処理(日／月)"
    Private Sub CmdAllSelectDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles CmdDaySet.Click, CmdDayClear.Click, CmdMonthSet.Click, CmdMonthClear.Click
        Dim Msg As String = ""
        Select Case CType(sender, Control).Name
            Case "CmdDaySet"
                Msg = "全選択(振込日)"
            Case "CmdDayClear"
                Msg = "全解除(振込日)"
            Case "CmdMonthSet"
                Msg = "全選択(振込月)"
            Case "CmdMonthClear"
                Msg = "全解除(振込月)"
        End Select

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(" & Msg & ")開始", "成功", "")
            Dim SetString As String
            Dim CmdName As String = CType(sender, Button).Name.ToUpper
            Dim FLG As Boolean = CmdName.EndsWith("Set".ToUpper)

            Select Case CmdName.Substring(3, 3)
                Case "Day".ToUpper : SetString = "日"
                Case "Mon".ToUpper : SetString = "月"
                Case Else : Return
            End Select

            For Each CTL As Control In GB21.Controls
                If TypeOf CTL Is CheckBox AndAlso CTL.Text.EndsWith(SetString) Then
                    CType(CTL, CheckBox).Checked = FLG
                End If
            Next

            Select Case True
                Case SetString = "日" : Me.FURI_KYU_CODE_T.Focus()
                Case SetString = "月" : Me.KEIYAKU_DATE_T.Focus()
            End Select
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(" & Msg & ")終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " 帳票印刷"
    Public Function fn_CreateCSV_INSERT() As Boolean
        '============================================================================
        'NAME           :fn_CreateCSV_INSERT
        'Parameter      :
        'Description    :KFSP031(取引先マスタメンテ(登録))印刷用ＣＳＶファイル作成
        'Return         :
        'Create         :2016/09/01
        'Update         :
        '============================================================================
        Dim i As Integer = 0
        Dim REC As OracleDataReader = Nothing
        Try
            '------------------------------------------------
            'ＣＳＶファイル作成
            '------------------------------------------------
            Dim CreateCSV As New KFSP031
            Dim Today As String = DateTime.Now.ToString("yyyyMMdd")
            Dim NowTime As String = DateTime.Now.ToString("HHmmss")

            Dim SQL As New StringBuilder
            SQL.AppendLine("SELECT")
            SQL.AppendLine("     *")
            SQL.AppendLine(" FROM ")
            SQL.AppendLine("     S_TORIMAST_VIEW")
            SQL.AppendLine(" WHERE ")
            SQL.AppendLine("     FSYORI_KBN_T = '3'")
            SQL.AppendLine(" AND TORIS_CODE_T = " & SQ(CType(oTORI(GetIndex("TORIS_CODE_T")), TextBox).Text.Trim))
            SQL.AppendLine(" AND TORIF_CODE_T = " & SQ(CType(oTORI(GetIndex("TORIF_CODE_T")), TextBox).Text.Trim))
            Dim KINKO As String
            Dim SITEN As String

            If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then
                strCSV_FILE_NAME = CreateCSV.CreateCsvFile()
                With CreateCSV
                    'GCOM.NzStrはNULL値対策
                    'コンボボックスの内容は、画面の更新後項目を取得する

                    .OutputCsvData(Today, True)                                                     'システム日付
                    .OutputCsvData(NowTime, True)                                                   'タイムスタンプ
                    .OutputCsvData(GCom.GetUserID, True)                                            'ログイン名
                    .OutputCsvData(Environment.MachineName, True)                                   '端末名
                    .OutputCsvData(GCom.NzStr(REC.Item("TORIS_CODE_T")), True)                      '取引先主コード
                    .OutputCsvData(GCom.NzStr(REC.Item("TORIF_CODE_T")), True)                      '取引先副コード
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("BAITAI_CODE_T"), 1), True)         '媒体コード
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("LABEL_KBN_T"), 1), True)           'ラベル区分
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("CODE_KBN_T"), 1), True)            'コード区分
                    .OutputCsvData(GCom.NzStr(REC.Item("ITAKU_KANRI_CODE_T")), True)                '代表委託者コード
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("KIJITU_KANRI_T"), 1), True)        '期日管理
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("CYCLE_T"), 1), True)               'サイクル管理
                    .OutputCsvData(GCom.NzStr(REC.Item("FILE_NAME_T")), True)                       'ファイル名
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("FMT_KBN_T"), 1), True)             'フォーマット区分
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("MULTI_KBN_T"), 1), True)           'マルチ区分
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("NS_KBN_T"), 1), True)              '入出金区分
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("SYUBETU_T"), 1), True)             '種別
                    .OutputCsvData(GCom.NzStr(REC.Item("ITAKU_CODE_T")), True)                      '委託者コード
                    .OutputCsvData(GCom.NzStr(REC.Item("ITAKU_KNAME_T")), True)                     '委託者名カナ
                    KINKO = GCom.NzStr(REC.Item("TKIN_NO_T"))
                    .OutputCsvData(KINKO, True)                                                     '取扱金融機関コード
                    .OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                           '取扱金融機関名
                    SITEN = GCom.NzStr(REC.Item("TSIT_NO_T"))
                    .OutputCsvData(SITEN, True)                                                     '取扱支店コード
                    .OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                        '取扱支店名
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("KAMOKU_T"), 1), True)              '科目
                    .OutputCsvData(GCom.NzStr(REC.Item("KOUZA_T")), True)                           '口座番号
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("SOUSIN_KBN_T"), 1), True)          '送信区分
                    '信組判定用
                    .OutputCsvData(Center, True)                                                    'センター区分
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("SYUMOKU_CODE_T"), 1), True)        '種目コード
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("FUKA_CODE_T"), 1), True)           '付加コード
                    .OutputCsvData(GCom.NzStr(REC.Item("ITAKU_NNAME_T")), True)                     '委託者名漢字
                    .OutputCsvData(GCom.NzStr(REC.Item("YUUBIN_T")), True)                          '郵便番号
                    .OutputCsvData(GCom.NzStr(REC.Item("DENWA_T")), True)                           '電話番号
                    .OutputCsvData(GCom.NzStr(REC.Item("FAX_T")), True)                             'FAX番号
                    .OutputCsvData(GCom.NzStr(REC.Item("KOKYAKU_NO_T")), True)                      '顧客番号
                    .OutputCsvData(GCom.NzStr(REC.Item("KANREN_KIGYO_CODE_T")), True)               '関連企業情報
                    .OutputCsvData(GCom.NzStr(REC.Item("ITAKU_NJYU_T")), True)                      '委託者住所漢字
                    .OutputCsvData(GCom.NzStr(REC.Item("YUUBIN_KNAME_T")), True)                    '郵送先名カナ
                    .OutputCsvData(GCom.NzStr(REC.Item("YUUBIN_NNAME_T")), True)                    '郵送先名漢字
                    For No As Integer = 1 To 31                                                     '振込日
                        If GCom.NzStr(GCom.NzStr(REC.Item("DATE" & No & "_T"))) = "1" Then
                            .OutputCsvData("○", True)
                        Else
                            .OutputCsvData("", True)
                        End If
                    Next
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("FURI_KYU_CODE_T"), 1), True)       '振込休日シフト
                    For No As Integer = 1 To 12                                                     '月別処理フラグ（振込・資金決済）
                        If GCom.NzStr(GCom.NzStr(REC.Item("TUKI" & No & "_T"))) = "1" Then
                            .OutputCsvData("○", True)
                        Else
                            .OutputCsvData("", True)
                        End If
                    Next
                    If GCom.NzStr(REC.Item("KEIYAKU_DATE_T")) = "00000000" Then                     '契約日
                        .OutputCsvData("", True)
                    Else
                        .OutputCsvData(GCom.NzStr(REC.Item("KEIYAKU_DATE_T")), True)
                    End If
                    .OutputCsvData(GCom.NzStr(REC.Item("KAISI_DATE_T")), True)                      '開始年月
                    .OutputCsvData(GCom.NzStr(REC.Item("SYURYOU_DATE_T")), True)                    '終了年月
                    .OutputCsvData(GCom.NzStr(REC.Item("MOTIKOMI_KIJITSU_T")), True)                '持込期日
                    .OutputCsvData(GCom.NzStr(REC.Item("IRAISYO_YDATE_T")), True)                   '日数／基準日(依頼書)
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("IRAISYO_KIJITSU_T"), 1), True)     '日付区分(依頼書)
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("IRAISYO_KYU_CODE_T"), 1), True)    '依頼書休日シフト
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("IRAISYO_KBN_T"), 1), True)         '依頼書種別
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("IRAISYO_SORT_T"), 1), True)        '依頼書出力順
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("TEIGAKU_KBN_T"), 1), True)         '定額区分
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("UMEISAI_KBN_T"), 1), True)         '受付明細表出力区分
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("PRTNUM_T"), 1), True)              '印刷部数
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("KESSAI_KBN_T"), 1), True)          '決済区分
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("KESSAI_PATN_T"), 1), True)         '資金確保方法
                    SITEN = GCom.NzStr(REC.Item("TORIMATOME_SIT_T"))
                    .OutputCsvData(SITEN, True)                                                     'とりまとめ店コード
                    .OutputCsvData(GCom.GetBKBRName(Jikinko, SITEN, 30), True)                      'とりまとめ店名
                    .OutputCsvData(GCom.NzStr(REC.Item("HONBU_KOUZA_T")), True)                     '本部別段口座番号
                    .OutputCsvData(GCom.NzStr(REC.Item("KESSAI_DAY_T")), True)                      '日数／基準日(決済)
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("KESSAI_KIJITSU_T"), 1), True)      '日付区分(決済)
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("KESSAI_KYU_CODE_T"), 1), True)     '決済休日シフト
                    KINKO = GCom.NzStr(REC.Item("TUKEKIN_NO_T"))
                    .OutputCsvData(KINKO, True)                                                     '決済金融機関コード
                    .OutputCsvData(GCom.GetBKBRName(KINKO, "", 30), True)                           '決済金融機関名
                    SITEN = GCom.NzStr(REC.Item("TUKESIT_NO_T"))
                    .OutputCsvData(SITEN, True)                                                     '決済支店コード
                    .OutputCsvData(GCom.GetBKBRName(KINKO, SITEN, 30), True)                        '決済支店名
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("TUKEKAMOKU_T"), 1), True)          '決済科目
                    .OutputCsvData(GCom.NzStr(REC.Item("TUKEKOUZA_T")), True)                       '決済口座番号
                    .OutputCsvData(GCom.NzStr(REC.Item("TUKEMEIGI_KNAME_T")), True)                 '決済名義人(カナ)
                    .OutputCsvData(GCom.NzStr(REC.Item("BIKOU1_T")), True)                          '備考１
                    .OutputCsvData(GCom.NzStr(REC.Item("BIKOU2_T")), True)                          '備考２
                    SITEN = GCom.NzStr(REC.Item("TESUUTYO_SIT_T"))
                    .OutputCsvData(SITEN, True)                                                     '手数料徴求支店コード
                    .OutputCsvData(GCom.GetBKBRName(Jikinko, SITEN, 30), True)                      '手数料徴求支店名
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUTYO_KAMOKU_T"), 1), True)     '手数料徴求科目
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUUTYO_KOUZA_T")), True)                  '手数料徴求口座番号
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUTYO_KBN_T"), 1), True)        '手数料徴求区分
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUTYO_PATN_T"), 1), True)       '手数料徴求方法
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUUMAT_NO_T")).Trim, True)                '手数料集計周期
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUUTYO_DAY_T")), True)                    '日数／基準日(手数料徴求)
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUTYO_KIJITSU_T"), 1), True)    '日付区分(手数料徴求)
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUU_KYU_CODE_T"), 1), True)      '手数料徴求休日シフト
                    .OutputCsvData(GCom.NzStr(REC.Item("SOURYO_T")), True)                          '送料
                    .OutputCsvData(GCom.NzStr(REC.Item("KOTEI_TESUU1_T")), True)                    '固定手数料１
                    .OutputCsvData(GCom.NzStr(REC.Item("KOTEI_TESUU2_T")), True)                    '固定手数料２
                    .OutputCsvData("", True)                                                        '手数料徴求対象区分
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUUMAT_MONTH_T")), True)                  '集計基準月
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUUMAT_ENDDAY_T")), True)                 '集計終了日
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUMAT_KIJYUN_T"), 1), True)     '集計基準
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUUMAT_PATN_T"), 1), True)       '集計方法
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUU_GRP_T")), True)                       '集計企業ＧＲＰ
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("TESUU_TABLE_ID_T"), 1), True)      '振込手数料基準ＩＤ
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUU_A1_T")), True)                        '自店１万円未満
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUU_A2_T")), True)                        '自店１万円以上３万円未満
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUU_A3_T")), True)                        '自店３万円以上
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUU_B1_T")), True)                        '本支店１万円未満
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUU_B2_T")), True)                        '本支店１万円以上３万円未満
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUU_B3_T")), True)                        '本支店３万円以上
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUU_C1_T")), True)                        '他行１万円未満
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUU_C2_T")), True)                        '他行１万円以上３万円未満
                    .OutputCsvData(GCom.NzStr(REC.Item("TESUU_C3_T")), True)                        '他行３万円以上
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("ENC_KBN_T"), 1), True)             '暗号化処理区分
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("ENC_OPT1_T"), 1), True)            'ＡＥＳオプション
                    .OutputCsvData(GCom.NzStr(REC.Item("ENC_KEY1_T")), True)                        '暗号化キー１
                    .OutputCsvData(GCom.NzStr(REC.Item("ENC_KEY2_T")), True)                        '暗号化キー２
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("TEKIYOU_KBN_T"), 1), True)         '摘要区分
                    .OutputCsvData(GCom.NzStr(REC.Item("KTEKIYOU_T")), True)                        'カナ摘要
                    .OutputCsvData(GCom.NzStr(REC.Item("NTEKIYOU_T")), True)                        '漢字摘要
                    .OutputCsvData(GCom.NzStr(REC.Item("FURI_CODE_T")), True)                       '振替コード
                    .OutputCsvData(GCom.NzStr(REC.Item("KIGYO_CODE_T")), True)                      '企業コード
                    .OutputCsvData(Me.InshizeiLabel.Inshizei1, True)                                '印紙税１
                    .OutputCsvData(Me.InshizeiLabel.Inshizei2, True)                                '印紙税２
                    .OutputCsvData(Me.InshizeiLabel.Inshizei3, True)                                '印紙税３
                    .OutputCsvData(GCom.NzStr(REC.Item("AITE_CNT_CODE_T")), True)                   '相手センター確認ＣＤ
                    .OutputCsvData(GCom.NzStr(REC.Item("TOHO_CNT_CODE_T")), True)                   '当方センター確認ＣＤ
                    .OutputCsvData(GCom.NzStr(REC.Item("DENSO_FILE_ID_T")), True)                   '伝送ファイルＩＤ
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("SYOUGOU_KBN_T"), 1), True)         '照合要否区分
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("MAE_SYORI_T"), 1), True)           '個別前処理
                    .OutputCsvData(GCom.NzStr(REC.Item("TOKKIJIKOU1_T")), True)                     '特記事項
                    .OutputCsvData(GCom.NzStr(REC.Item("TOKKIJIKOU2_T")), True)                     '受付特記事項
                    .OutputCsvData(GCom.NzStr(REC.Item("TOKKIJIKOU3_T")), True)                     '返却特記事項
                    .OutputCsvData(GCom.NzStr(REC.Item("TOKKIJIKOU4_T")), True)                     '搬送特記事項
                    .OutputCsvData(GetComboBoxValue(2, GetIndex("KYUUYO_KBN_T"), 1), True)          '給与摘要区分
                    .OutputCsvData("", True, True)
                End With
                CreateCSV.CloseCsv()
            Else
                MessageBox.Show(String.Format(MSG0231W, "取引先マスタメンテ(登録)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", "登録なし")
                Return False
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(String.Format(MSG0231W, "取引先マスタメンテ(登録)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", ex.Message)
            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            Me.ResumeLayout()
        End Try

    End Function

    Public Function fn_CreateCSV_Delete() As Boolean
        '============================================================================
        'NAME           :fn_CreateCSV_Delete
        'Parameter      :
        'Description    :KFSP018(取引先マスタメンテ(削除))印刷用ＣＳＶファイル作成
        'Return         :
        'Create         :2009/09/08
        'Update         :
        '============================================================================
        Dim i As Integer = 0
        Dim REC As OracleDataReader = Nothing
        Try
            '------------------------------------------------
            'ＣＳＶファイル作成
            '------------------------------------------------
            Dim CreateCSV As New KFSPxxx("KFSP019")
            Dim Today As String = Format(DateTime.Now, "yyyyMMdd")
            Dim NowTime As String = Format(DateTime.Now, "HHmmss")
            Dim SQL As New StringBuilder(128)
            strCSV_FILE_NAME = CreateCSV.CreateCsvFile()

            SQL.AppendLine("SELECT *")
            SQL.AppendLine(" FROM S_TORIMAST,TENMAST")
            SQL.AppendLine(" WHERE FSYORI_KBN_T = '3'")
            SQL.AppendLine(" AND TORIS_CODE_T = " & SQ(CType(oTORI(GetIndex("TORIS_CODE_T")), TextBox).Text.Trim))
            SQL.AppendLine(" AND TORIF_CODE_T = " & SQ(CType(oTORI(GetIndex("TORIF_CODE_T")), TextBox).Text.Trim))
            SQL.AppendLine(" AND TKIN_NO_T = KIN_NO_N(+)")
            SQL.AppendLine(" AND TSIT_NO_T = SIT_NO_N(+)")
            If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then
                CreateCSV.OutputCsvData(Today, True)                                              'システム日付
                CreateCSV.OutputCsvData(NowTime, True)                                            'タイムスタンプ
                CreateCSV.OutputCsvData(GCom.GetUserID, True)                                     'ログイン名
                CreateCSV.OutputCsvData(Environment.MachineName, True)                            '端末名
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("TORIS_CODE_T")), True)               '取引先主コード
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("TORIF_CODE_T")), True)               '取引先副コード
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("ITAKU_CODE_T")), True)               '委託者コード
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("ITAKU_NNAME_T")), True)              '委託者漢字名
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("ITAKU_KNAME_T")), True)              '委託者カナ名
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("TKIN_NO_T")), True)                  '取扱金融機関コード
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("KIN_NNAME_N")), True)                '取扱金融機関名
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("TSIT_NO_T")), True)                  '取扱支店コード
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("SIT_NNAME_N")), True)                '取扱支店名
                CreateCSV.OutputCsvData(GetComboBoxValue(2, GetIndex("KAMOKU_T"), 0), True)       '科目
                CreateCSV.OutputCsvData(GCom.NzStr(REC.Item("KOUZA_T")), True)                    '口座番号
                CreateCSV.OutputCsvData(GetComboBoxValue(2, GetIndex("BAITAI_CODE_T"), 0), True)  '媒体コード
                CreateCSV.OutputCsvData(GetComboBoxValue(2, GetIndex("LABEL_KBN_T"), 0), True)    'ラベル区分
                CreateCSV.OutputCsvData(GetComboBoxValue(2, GetIndex("CODE_KBN_T"), 0), True)     'コード区分
                CreateCSV.OutputCsvData(GetComboBoxValue(2, GetIndex("SOUSIN_KBN_T"), 0), True)   '送信区分
                CreateCSV.OutputCsvData(GetComboBoxValue(2, GetIndex("SYUBETU_T"), 0), True)      '種別
                CreateCSV.OutputCsvData(GetComboBoxValue(2, GetIndex("SYUMOKU_CODE_T"), 0), True, True) '種目コード
            Else
                Return False
            End If
            CreateCSV.CloseCsv()
            CreateCSV = Nothing

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷データ作成", "失敗", ex.Message)
            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

    End Function

    Public Function fn_Print(ByVal PrintNo As Integer) As Boolean
        '============================================================================
        'NAME           :fn_CreateCSV
        'Parameter      :PrintNo 1:登録 2:更新 3:削除
        'Description    :帳票印刷用ＣＳＶファイル作成
        'Return         :
        'Create         :2009/09/08
        'Update         :
        '============================================================================
        Dim ErrMessage As String = ""
        Dim Param As String = ""
        Dim nRet As Integer = 0
        Dim Syori As String = ""

        Select Case PrintNo
            Case 1
                Syori = "登録"
            Case 2
                Syori = "更新"
            Case 3
                Syori = "削除"
        End Select
        Try
            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：ログイン名、ＣＳＶファイル名、取引先コード
            Param = GCom.GetUserID & "," & strCSV_FILE_NAME & "," & _
                    LW.ToriCode
            Select Case PrintNo
                Case 1
                    nRet = ExeRepo.ExecReport("KFSP031.EXE", Param)
                Case 2
                    nRet = ExeRepo.ExecReport("KFSP018.EXE", Param)
                Case 3
                    nRet = ExeRepo.ExecReport("KFSP019.EXE", Param)
            End Select

            If nRet <> 0 Then
                '印刷失敗：戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case -1
                        ErrMessage = String.Format(MSG0106W, "取引先マスタメンテ(" & Syori & ")")
                    Case Else
                        ErrMessage = String.Format(MSG0004E, "取引先マスタメンテ(" & Syori & ")")
                End Select

                MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
            MessageBox.Show(String.Format(MSG0014I, "取引先マスタメンテ(" & Syori & ")"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return True

        Catch ex As Exception
            ErrMessage = String.Format(MSG0004E, "取引先マスタメンテ(" & Syori & ")")
            MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタメンテ(" & Syori & ")印刷", "失敗")
            Return False
        End Try

    End Function
#End Region

#Region " 関数"

    '

    '
    ' 機　能 : 変更差分印刷用の支援関数
    '
    ' 戻り値 : 印刷項目値
    '
    ' 引き数 : ARG1 - 登録 = 0, 更新 = 1, 削除 = 2
    ' 　　　   ARG2 - Index
    ' 　　　   ARG3 - OLD = 0, NEW = 1
    '
    ' 備　考 : なし
    '
    Private Function GetComboBoxValue(ByVal aSEL As Integer, _
                        ByVal Index As Integer, ByVal NewOld As Integer) As String
        Try
            If aSEL = 2 Then
                Return memTORI(Index, NewOld)
            Else
                Return "( " & sTORI(Index, NewOld + 3) & " ) " & memTORI(Index, NewOld)
            End If
        Catch ex As Exception
            Return ""
        End Try
    End Function

    '
    ' 機能　　 : 取引先マスタへの登録（新規更新）処理
    '
    ' 戻り値　 : 成功 = True
    ' 　　　　   失敗 = False
    '
    ' 引き数　 : ARG1 - なし
    '
    ' 備考　　 : 自振取引先マスタ専用関数
    '
    Private Function InsertModule() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim Cnt As Integer

        sTORI(GetIndex("SAKUSEI_DATE_T"), 4) = String.Format("{0:yyyyMMdd}", Date.Now)
        sTORI(GetIndex("KOUSIN_DATE_T"), 4) = String.Format("{0:yyyyMMdd}", Date.Now)

        Try
            '----------------------------------------------------------------
            ' 取引先マスタ <S_TORIMAST>
            '----------------------------------------------------------------
            SQL.Length = 0
            SQL.AppendLine("INSERT INTO S_TORIMAST")

            For Cnt = 0 To MaxColumns - MaxColumns_Sub Step 1

                If TypeOf oTORI(Cnt) Is Label Then
                    Select Case sTORI(Cnt, 1).ToUpper
                        Case "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                             "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                             "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"
                            Select Case Cnt
                                Case 0
                                    SQL.AppendLine(" (" & sTORI(Cnt, 1))
                                Case Else
                                    SQL.AppendLine(", " & sTORI(Cnt, 1))
                            End Select
                    End Select
                Else
                    Select Case sTORI(Cnt, 1).ToUpper
                        Case "SAKUSEI_DATE_T", "KOUSIN_DATE_T", "KOUSIN_SIKIBETU_T"
                            '作成日、更新日、スケジュール更新識別は最後に回す。
                        Case Else
                            Select Case Cnt
                                Case 0
                                    SQL.AppendLine(" (" & sTORI(Cnt, 1))
                                Case Else
                                    SQL.AppendLine(", " & sTORI(Cnt, 1))
                            End Select
                    End Select
                End If
            Next Cnt
            SQL.AppendLine(", KOUSIN_SIKIBETU_T")   '更新識別追加
            SQL.AppendLine(", SAKUSEI_DATE_T")
            SQL.AppendLine(", KOUSIN_DATE_T)")

            SQL.AppendLine(" VALUES")

            For Cnt = 0 To MaxColumns - MaxColumns_Sub Step 1

                If TypeOf oTORI(Cnt) Is Label Then
                    Select Case sTORI(Cnt, 1).ToUpper
                        Case "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                             "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                             "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"
                            Select Case Cnt
                                Case 0
                                    SQL.AppendLine(" (" & SetColumnData(Cnt))
                                Case Else
                                    SQL.AppendLine(", " & SetColumnData(Cnt))
                            End Select
                    End Select
                Else
                    Select Case sTORI(Cnt, 1).ToUpper
                        Case "SAKUSEI_DATE_T", "KOUSIN_DATE_T", "KOUSIN_SIKIBETU_T"
                            '作成日、更新日は最後に回す。
                        Case Else
                            Select Case Cnt
                                Case 0
                                    SQL.AppendLine(" (" & SetColumnData(Cnt))
                                Case Else
                                    SQL.AppendLine(", " & SetColumnData(Cnt))
                            End Select
                    End Select
                End If
            Next Cnt
            SQL.AppendLine(", '0'")                                     '更新識別追加
            SQL.AppendLine(", TO_CHAR(SYSDATE, 'yyyymmddHH24MISS')")    '作成日
            SQL.AppendLine(", TO_CHAR(SYSDATE, 'yyyymmddHH24MISS'))")    '更新日

            Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)

            If Ret <> 1 Then
                MessageBox.Show(String.Format(MSG0027E, "取引先マスタ", "登録"), msgTitle, _
                 MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            '----------------------------------------------------------------
            ' 取引先マスタ（個別情報） <S_TORIMAST_SUB>
            '----------------------------------------------------------------
            SQL.Length = 0
            SQL.Append("INSERT INTO S_TORIMAST_SUB")
            SQL.Append(" ( ")
            SQL.Append("   FSYORI_KBN_TSUB")
            SQL.Append(" , TORIS_CODE_TSUB")
            SQL.Append(" , TORIF_CODE_TSUB")
            For Cnt = MaxColumns - MaxColumns_Sub + 1 To MaxColumns Step 1
                If Not TypeOf oTORI(Cnt) Is Label Then
                    SQL.Append(", " & sTORI(Cnt, 1))
                End If
            Next Cnt
            SQL.Append(" ) VALUES (")
            SQL.Append("   " & SQ(sTORI(GetIndex("FSYORI_KBN_T"), 4)))
            SQL.Append(" , " & SQ(sTORI(GetIndex("TORIS_CODE_T"), 4)))
            SQL.Append(" , " & SQ(sTORI(GetIndex("TORIF_CODE_T"), 4)))
            For Cnt = MaxColumns - MaxColumns_Sub + 1 To MaxColumns Step 1
                If Not TypeOf oTORI(Cnt) Is Label Then
                    SQL.Append(", " & SetColumnData(Cnt))
                End If
            Next Cnt
            SQL.Append(" )")

            Ret = MainDB.ExecuteNonQuery(SQL)
            If Ret = 1 Then
                Return True
            Else
                MessageBox.Show(String.Format(MSG0027E, "取引先マスタ(個別情報)", "登録"), msgTitle, _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            Return (Ret = 1)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, _
                 MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(登録処理)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    '
    ' 機能　　 : 取引先マスタへの登録（既存更新）処理
    '
    ' 戻り値　 : 成功 = True
    ' 　　　　   失敗 = False
    '
    ' 引き数　 : ARG1 - なし
    '
    ' 備考　　 : 自振取引先マスタ専用関数
    '
    Private Function UpdateModule() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim Cnt As Integer

        sTORI(GetIndex("KOUSIN_DATE_T"), 4) = String.Format("{0:yyyyMMddHHmmss}", Date.Now)
        sTORI(GetIndex("KOUSIN_SIKIBETU_T"), 4) = "1"
        Try
            '----------------------------------------------------------------
            ' 取引先マスタ <S_TORIMAST>
            '----------------------------------------------------------------
            SQL.Length = 0
            SQL.AppendLine("UPDATE S_TORIMAST")

            Cnt = GetIndex("FSYORI_KBN_T")
            SQL.AppendLine(" SET " & sTORI(Cnt, 1))               '項目名
            SQL.AppendLine(" = " & SetColumnData(Cnt))             '項目値

            For Cnt = 0 To MaxColumns - MaxColumns_Sub Step 1

                If Not TypeOf oTORI(Cnt) Is Label Then
                    If Not sTORI(Cnt, 3) = sTORI(Cnt, 4) Then
                        '変更された項目のみ更新する。
                        SQL.AppendLine(", " & sTORI(Cnt, 1))             '項目名
                        SQL.AppendLine(" = " & SetColumnData(Cnt))       '項目値
                    End If

                ElseIf sTORI(Cnt, 1).ToUpper = "KOUSIN_DATE_T" Then
                    '更新日
                    SQL.AppendLine(", " & sTORI(Cnt, 1) & " = TO_CHAR(SYSDATE, 'yyyymmddHH24MMSS')")
                ElseIf sTORI(Cnt, 1).ToUpper = "KOUSIN_SIKIBETU_T" Then '更新識別
                    SQL.AppendLine(", KOUSIN_SIKIBETU_T = '1'")
                Else
                    Select Case sTORI(Cnt, 1).ToUpper
                        Case "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                             "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                             "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"

                            SQL.AppendLine(", " & sTORI(Cnt, 1))             '項目名
                            SQL.AppendLine(" = " & SetColumnData(Cnt))      '項目値
                    End Select
                End If
            Next Cnt

            '更新条件
            SQL.AppendLine(" WHERE FSYORI_KBN_T = " & SetColumnData(GetIndex("FSYORI_KBN_T")))
            SQL.AppendLine(" AND TORIS_CODE_T = " & SetColumnData(GetIndex("TORIS_CODE_T")))
            SQL.AppendLine(" AND TORIF_CODE_T = " & SetColumnData(GetIndex("TORIF_CODE_T")))

            Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)
            If Ret <> 1 Then '更新失敗
                MessageBox.Show(String.Format(MSG0027E, "取引先マスタ", "更新"), msgTitle, _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            '----------------------------------------------------------------
            ' 取引先マスタ（個別情報） <TORIMAST_SUB>
            '----------------------------------------------------------------
            SQL.Length = 0
            SQL.Append("UPDATE S_TORIMAST_SUB SET")
            SQL.Append(" FSYORI_KBN_TSUB = " & SetColumnData(GetIndex("FSYORI_KBN_T")))        '処理区分

            For Cnt = MaxColumns - MaxColumns_Sub + 1 To MaxColumns Step 1
                If Not TypeOf oTORI(Cnt) Is Label Then
                    '--------------------------------------------------------
                    ' 変更された項目のみを更新する。
                    '--------------------------------------------------------
                    If Not sTORI(Cnt, 3) = sTORI(Cnt, 4) Then
                        SQL.Append(", " & sTORI(Cnt, 1))                                       '項目名
                        SQL.Append(" = " & SetColumnData(Cnt))                                 '項目値
                    End If
                End If
            Next Cnt

            '更新条件
            SQL.Append(" WHERE ")
            SQL.Append("     FSYORI_KBN_TSUB = " & SetColumnData(GetIndex("FSYORI_KBN_T")))
            SQL.Append(" AND TORIS_CODE_TSUB = " & SetColumnData(GetIndex("TORIS_CODE_T")))
            SQL.Append(" AND TORIF_CODE_TSUB = " & SetColumnData(GetIndex("TORIF_CODE_T")))

            Ret = MainDB.ExecuteNonQuery(SQL)
            If Ret <> 1 Then '更新失敗
                MessageBox.Show(String.Format(MSG0027E, "取引先マスタ(個別情報)", "更新"), msgTitle, _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            '----------------------------------------------------------------
            ' 【スケジュールマスタの項目更新処理】
            ' 　下記①～⑥のいずれかに変更があった場合、
            '   スケジュールマスタの更新処理も同時に行う
            ' 　　①媒体コード
            ' 　　②種別
            ' 　　③委託者コード
            ' 　　④取扱金融機関コード
            ' 　　⑤取扱支店コード
            ' 　　⑥送信区分
            '----------------------------------------------------------------
            If sTORI(GetIndex("BAITAI_CODE_T"), 3) <> sTORI(GetIndex("BAITAI_CODE_T"), 4) OrElse _
               sTORI(GetIndex("SYUBETU_T"), 3) <> sTORI(GetIndex("SYUBETU_T"), 4) OrElse _
               sTORI(GetIndex("ITAKU_CODE_T"), 3) <> sTORI(GetIndex("ITAKU_CODE_T"), 4) OrElse _
               sTORI(GetIndex("TKIN_NO_T"), 3) <> sTORI(GetIndex("TKIN_NO_T"), 4) OrElse _
               sTORI(GetIndex("TSIT_NO_T"), 3) <> sTORI(GetIndex("TSIT_NO_T"), 4) OrElse _
               sTORI(GetIndex("SOUSIN_KBN_T"), 3) <> sTORI(GetIndex("SOUSIN_KBN_T"), 4) Then
                If UpdateModuleSCHMAST() = False Then
                    Return False
                End If
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, _
                 MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新処理)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Private Function UpdateModuleSCHMAST() As Boolean
        Dim SQL As New StringBuilder(128)
        Try
            SQL = New StringBuilder(128)
            SQL.AppendLine(" UPDATE S_SCHMAST SET ")
            SQL.AppendLine(" SYUBETU_S = " & SQ(sTORI(GetIndex("SYUBETU_T"), 4)))
            SQL.AppendLine(",BAITAI_CODE_S = " & SQ(sTORI(GetIndex("BAITAI_CODE_T"), 4)))
            SQL.AppendLine(",SOUSIN_KBN_S = " & SQ(sTORI(GetIndex("SOUSIN_KBN_T"), 4)))
            SQL.AppendLine(",ITAKU_CODE_S = " & SQ(sTORI(GetIndex("ITAKU_CODE_T"), 4)))
            SQL.AppendLine(",TKIN_NO_S = " & SQ(sTORI(GetIndex("TKIN_NO_T"), 4)))
            SQL.AppendLine(",TSIT_NO_S = " & SQ(sTORI(GetIndex("TSIT_NO_T"), 4)))
            SQL.AppendLine(" WHERE FSYORI_KBN_S = " & SQ(sTORI(GetIndex("FSYORI_KBN_T"), 3)))
            SQL.AppendLine(" AND TORIS_CODE_S = " & SQ(sTORI(GetIndex("TORIS_CODE_T"), 3)))
            SQL.AppendLine(" AND TORIF_CODE_S = " & SQ(sTORI(GetIndex("TORIF_CODE_T"), 3)))

            Dim Ret As Integer = MainDB.ExecuteNonQuery(SQL)
            If Ret = -1 Then
                MessageBox.Show(String.Format(MSG0027E, "スケジュールマスタ", "更新"), msgTitle, _
                               MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, _
                 MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新処理)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    '
    '
    ' 機能　　 : 取引先マスタORACLE設定項目値作成支援関数
    '
    ' 戻り値　 : 設定項目値
    '
    ' 引き数　 : ARG1 - Index
    '
    ' 備考　　 : 自振取引先マスタ専用関数
    '
    Private Function SetColumnData(ByVal Index As Integer) As String
        Try
            If IsNothing(sTORI(Index, 4)) OrElse sTORI(Index, 4).Trim.Length = 0 Then
                Return "NULL"
            Else
                Select Case sTORI(Index, 2).Trim.ToUpper
                    Case "NUMBER"
                        Return GCom.NzDec(sTORI(Index, 4)).ToString
                    Case Else
                        Dim Temp As String = " "
                        If sTORI(Index, 4).Trim.Length > 0 Then
                            Temp = sTORI(Index, 4).Trim
                        End If
                        Return "'" & Temp & "'"
                End Select
            End If
        Catch ex As Exception

            Return "NULL"
        End Try
    End Function

    '
    ' 機能　　 : 格納位置の検索
    '
    ' 戻り値　 : 格納位置
    '
    ' 引き数　 : ARG1 - ORACLE項目名
    '
    ' 備考　　 : 自振取引先マスタ専用
    '
    Private Function GetIndex(ByVal avColumnName As String) As Integer
        Dim Index As Integer
        Try
            For Index = 0 To MaxColumns Step 1
                If avColumnName.ToUpper = sTORI(Index, 1).ToUpper Then

                    Return Index
                End If
            Next Index
        Catch ex As Exception
            MainLOG.Write("格納位置取得", "失敗", ex.ToString)
        End Try

        Return -1
    End Function

    '
    ' 機　能 : エラーのあった場所にフォーカスを設定する
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - Control格納配列のIndex
    '
    ' 備　考 : なし
    '    
    Private Sub SetFocusOnErrorControl(ByVal Index As Integer)
        Try
            Select Case Index
                Case 1, 2, 3
                    oTORI(Index).Focus()
                    Return
                Case Else
                    For Each TAB As TabPage In SSTab.TabPages
                        For Each CTL As Control In TAB.Controls
                            If TypeOf CTL Is GroupBox Then
                                For Each OBJ As Control In CTL.Controls
                                    If OBJ.Name.ToUpper = sTORI(Index, 1).ToUpper Then
                                        SSTab.SelectedIndex = TAB.TabIndex
                                        oTORI(Index).Focus()
                                        Return
                                    End If
                                Next OBJ
                            ElseIf TypeOf CTL Is TextBox Then
                                If CTL.Name.ToUpper = sTORI(Index, 1).ToUpper Then
                                    SSTab.SelectedIndex = TAB.TabIndex
                                    oTORI(Index).Focus()
                                    Return
                                End If
                            ElseIf TypeOf CTL Is CheckBox Then
                                If CTL.Name.ToUpper = sTORI(Index, 1).ToUpper Then
                                    SSTab.SelectedIndex = TAB.TabIndex
                                    oTORI(Index).Focus()
                                    Return
                                End If
                            ElseIf TypeOf CTL Is ComboBox Then
                                If CTL.Name.ToUpper = sTORI(Index, 1).ToUpper Then
                                    SSTab.SelectedIndex = TAB.TabIndex
                                    oTORI(Index).Focus()
                                    Return
                                End If
                            End If
                        Next CTL
                    Next TAB
            End Select
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("フォーカス移動", "失敗", ex.ToString)
        End Try
    End Sub
#End Region

#Region " チェック"

    '
    ' 機　能 : 項目間の相関をチェックする
    '
    ' 戻り値 : チェック抵触コントロール
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : 相関チェック用の関数(戻り値がZERO以外はエラー)
    '    
    Private Function CheckMutualRelation(ByRef MSG As String) As Integer
        ErrMsgFlg = True  'エラーメッセージ表示判定
        Dim Index As Integer
        Try
            '必須項目入力有無チェック
            Index = CheckMutualRelation_001(MSG)
            If Not Index = -1 Then
                Return Index
            End If
            '契約日等日付チェック
            Index = CheckMutualRelation_003(MSG)
            If Not Index = -1 Then
                Return Index
            End If

            '金融機関情報チェック
            Index = CheckMutualRelation_005(MSG)
            If Not Index = -1 Then
                Return Index
            End If

            '委託者コード関連項目チェック
            Index = CheckMutualRelation_008(MSG)
            If Not Index = -1 Then
                Return Index
            End If

            '諸々区分項目チェック
            Index = CheckMutualRelation_009(MSG)
            If Not Index = -1 Then
                Return Index
            End If

            '日項目範囲チェック
            Index = CheckMutualRelation_010(MSG)
            If Not Index = -1 Then
                Return Index
            End If

            '決済関連項目チェック
            '2010/10/21 決済資金対象外の場合は、チェックしない---
            'Index = CheckMutualRelation_011(MSG)
            'If Not Index = -1 Then
            '    Return Index
            'End If
            If KESSAI <> "0" Then
                Index = CheckMutualRelation_011(MSG)
                If Not Index = -1 Then
                    Return Index
                End If
            End If
            '-----------------------------------------------------

            '備考項目チェック
            Index = CheckMutualRelation_012(MSG)
            If Not Index = -1 Then
                Return Index
            End If

            '依頼書/伝票チェック
            Index = CheckMutualRelation_013(MSG)
            If Not Index = -1 Then
                Return Index
            End If

            ''決済関連チェック2
            'Index = CheckMutualRelation_014(MSG)
            'If Not Index = -1 Then
            '    Return Index
            'End If

            '摘要区分チェック
            Index = CheckMutualRelation_004(MSG)
            If Not Index = -1 Then
                Return Index
            End If

            Return 0
        Catch ex As Exception
            Return -9
        End Try
    End Function

    '
    ' 機　能 : 必須入力項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_001(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            For Index = 1 To MaxColumns Step 1

                If GCom.NzInt(sTORI(Index, 5)) = 1 Then

                    If sTORI(Index, 4).Trim.Length <= 0 Then

                        If sTORI(Index, 1) = "TESUUMAT_NO_T" Then
                            '手数料集計周期
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_MONTH_T" Then
                            '集計基準月
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_ENDDAY_T" Then
                            '集計終了日
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_KIJYUN2_T" Then
                            '集計基準
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_PATN_T" Then
                            '集計方法
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "KESSAI_DAY_T" Then
                            '資金確保日数／基準日
                            Select Case GCom.NzInt(sTORI(GetIndex("KESSAI_KIJITSU_T"), 4))
                                Case 4
                                Case Else
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0285W, sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUTYO_DAY_T" Then
                            '手数料徴求日数／基準日
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KIJITSU_T"), 4))
                                Case 3, 4
                                Case Else
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0285W, sTORI(Index, 0)))
                            End Select
                        Else
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0285W, sTORI(Index, 0)))
                        End If
                    End If
                Else
                    If sTORI(Index, 4).Trim.Length <= 0 Then
                        If sTORI(Index, 1) = "TESUUMAT_NO_T" Then
                            '手数料集計周期
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_MONTH_T" Then
                            '集計基準月
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_ENDDAY_T" Then
                            '集計終了日
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_KIJYUN2_T" Then
                            '集計基準
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        ElseIf sTORI(Index, 1) = "TESUUMAT_PATN_T" Then
                            '集計方法
                            Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KBN_T"), 4))
                                Case 1
                                    '1, 一括徴求
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0296W, "一括徴求", sTORI(Index, 0)))
                            End Select
                        End If
                    End If
                End If


                'テキストボックスの場合
                If TypeOf oTORI(Index) Is TextBox Then

                    If sTORI(Index, 4).Trim.Length > 0 Then

                        Select Case sTORI(Index, 1).ToUpper
                            Case "ITAKU_KNAME_T", _
                                "KTEKIYOU_T", _
                                "ENC_KEY1_T", _
                                "ENC_KEY2_T", _
                                "TUKEMEIGI_KNAME_T"
                                '半角英数カナ評価(全角文字抹消) > 2009/11/13 FILE_NAME_T 削除

                                Dim JISEncoding As Encoding = Encoding.GetEncoding("SHIFT-JIS")
                                Dim onByte() As Byte = JISEncoding.GetBytes(sTORI(Index, 4))

                                If onByte.Length > sTORI(Index, 4).Length Then
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0297W, sTORI(Index, 0)))
                                Else
                                    Select Case sTORI(Index, 1).ToUpper
                                        Case "ITAKU_KNAME_T", _
                                            "TUKEMEIGI_KNAME_T"
                                            '全銀規制文字

                                            If Not GCom.CheckZenginChar(CType(oTORI(Index), TextBox)) Then
                                                MsgIcon = MessageBoxIcon.Warning
                                                Throw New Exception(String.Format(MSG0298W, sTORI(Index, 0)))
                                            End If
                                    End Select
                                End If
                        End Select
                    Else
                        Select Case sTORI(Index, 1).ToUpper
                            Case "FILE_NAME_T"
                                ' 2016/06/06 タスク）綾部 CHG 【OM】(RSV2対応) -------------------- START
                                ''ファイル名かつ媒体がFD(01)の場合は必須チェックを行う
                                'If GCom.NzInt(sTORI(GetIndex("BAITAI_CODE_T"), 4)) = 1 Then
                                '    MsgIcon = MessageBoxIcon.Warning
                                '    Throw New Exception(MSG0153W)
                                'End If
                                '--------------------------------------------------------------------
                                ' ファイル名入力チェック       <媒体コード> 01,11,12,13,14,15
                                '--------------------------------------------------------------------
                                Select Case GCom.NzInt(sTORI(GetIndex("BAITAI_CODE_T"), 4))
                                    Case 1, 11, 12, 13, 14, 15
                                        MsgIcon = MessageBoxIcon.Warning
                                        Throw New Exception(MSG0153W)
                                End Select
                                ' 2016/06/06 タスク）綾部 CHG 【OM】(RSV2対応) -------------------- END
                            Case "HONBU_KOUZA_T", "KESSAI_DAY_T", "TUKEKIN_NO_T", "TUKESIT_NO_T", "TUKEKOUZA_T", "TUKEMEIGI_KNAME_T", _
                                 "TESUUTYO_SIT_T", "TESUUTYO_KOUZA_T", "TESUUTYO_DAY_T"
                                '2010/10/21 決済資金対象外の場合は、チェックしない-----------------------------------------------------
                                'If GCom.NzInt(sTORI(GetIndex("KESSAI_KBN_T"), 4)) <> 99 Then    '決済対象外以外必須
                                If GCom.NzInt(sTORI(GetIndex("KESSAI_KBN_T"), 4)) <> 99 And KESSAI <> "0" Then    '決済対象外以外必須
                                    '--------------------------------------------------------------------------------------------------
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(String.Format(MSG0285W, sTORI(Index, 0)))
                                End If
                        End Select

                    End If
                End If

            Next Index

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then
            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    '
    ' 機　能 : 日付歴日チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_003(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            Dim Ret As Integer
            Dim onDate As Date
            Dim onText(2) As Integer

            '契約日
            Index = GetIndex("KAISI_DATE_T")
            onText(0) = GCom.NzInt(sTORI(Index, 4).Substring(0, 4))
            onText(1) = GCom.NzInt(sTORI(Index, 4).Substring(4, 2))
            onText(2) = GCom.NzInt(sTORI(Index, 4).Substring(6))

            If Not (onText(0) + onText(1) + onText(2)) = 0 Then
                '契約日範囲チェック(月)(MSG0022W)
                If onText(1) < 1 OrElse onText(1) > 12 Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception(MSG0022W)
                End If
            End If

            If Not (onText(0) + onText(1) + onText(2)) = 0 Then
                '契約日範囲チェック(日)(MSG0025W)
                If onText(2) < 1 OrElse onText(2) > 31 Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception(MSG0025W)
                End If
            End If

            If Not (onText(0) + onText(1) + onText(2)) = 0 Then
                '契約日日付整合性チェック(MSG0027W)
                Ret = GCom.SET_DATE(onDate, onText)
                If Not Ret = -1 Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception(MSG0027W)
                End If
            End If

            '開始年月日
            Dim StartDate As String = "00000000"
            Index = GetIndex("KAISI_DATE_T")
            onText(0) = GCom.NzInt(sTORI(Index, 4).Substring(0, 4))
            onText(1) = GCom.NzInt(sTORI(Index, 4).Substring(4, 2))
            onText(2) = GCom.NzInt(sTORI(Index, 4).Substring(6))

            '開始年月必須チェック(年)(MSG0168W)
            If KAISI_DATE_T.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0168W)
            End If
            '開始年月必須チェック(月)(MSG0170W)
            If KAISI_DATE_T1.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0170W)
            End If
            '開始年月必須チェック(日)(MSG0172W)
            If KAISI_DATE_T2.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0172W)
            End If
            '開始年月範囲チェック(月)(MSG0022W)
            If onText(1) < 1 OrElse onText(1) > 12 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0022W)
            End If

            '開始年月範囲チェック(日)(MSG0025W)
            If onText(2) < 1 OrElse onText(2) > 31 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0025W)
            End If

            '開始年月日付整合性チェック(MSG0027W)
            Ret = GCom.SET_DATE(onDate, onText)
            If Not Ret = -1 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0027W)
            End If

            StartDate = sTORI(Index, 4).Trim

            '終了年月日
            Dim FinalDate As String = "99999999"
            Index = GetIndex("SYURYOU_DATE_T")
            onText(0) = GCom.NzInt(sTORI(Index, 4).Substring(0, 4))
            onText(1) = GCom.NzInt(sTORI(Index, 4).Substring(4, 2))
            onText(2) = GCom.NzInt(sTORI(Index, 4).Substring(6))

            '終了年月必須チェック(年)(MSG0174W)
            If SYURYOU_DATE_T.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0174W)
            End If
            '終了年月必須チェック(月)(MSG0176W)
            If SYURYOU_DATE_T.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0176W)
            End If
            '終了年月必須チェック(日)(MSG0178W)
            If SYURYOU_DATE_T.Text.Trim = "" Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0178W)
            End If
            '終了年月日範囲チェック(月)(MSG0022W)
            If onText(1) < 1 OrElse onText(1) > 12 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0022W)
            End If

            '終了年月日範囲チェック(日)(MSG0025W)
            If onText(2) < 1 OrElse onText(2) > 31 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0025W)
            End If

            '終了年月日日付整合性チェック(MSG0027W)
            Ret = GCom.SET_DATE(onDate, onText)
            If Not Ret = -1 Then
                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0027W)
            End If

            FinalDate = sTORI(Index, 4).Trim

            If GCom.NzDec(StartDate) >= GCom.NzDec(FinalDate) Then

                MsgIcon = MessageBoxIcon.Warning
                Throw New Exception(MSG0099W)
            End If

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    '
    ' 機　能 : 金融機関情報項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_005(ByRef MSG As String) As Integer
        Try
            Dim Ret As Integer
            Dim BANK As Integer
            Dim BRANCH As Integer

            'ヘッダー金融機関情報
            BANK = GetIndex("TKIN_NO_T")
            BRANCH = GetIndex("TSIT_NO_T")

            If GCom.NzInt(sTORI(BANK, 4)) + GCom.NzInt(sTORI(BRANCH, 4)) > 0 Then
                Ret = GCom.CheckBankBranch(sTORI(BANK, 4), sTORI(BRANCH, 4))
                Select Case Ret
                    Case 1
                        '銀行無し
                        Return BANK
                    Case 2
                        '支店無し
                        Return BRANCH
                End Select
            End If

            '決済金融機関情報
            BANK = GetIndex("TUKEKIN_NO_T")
            BRANCH = GetIndex("TUKESIT_NO_T")

            If GCom.NzInt(sTORI(BANK, 4)) + GCom.NzInt(sTORI(BRANCH, 4)) > 0 Then
                Ret = GCom.CheckBankBranch(sTORI(BANK, 4), sTORI(BRANCH, 4))
                Select Case Ret
                    Case 1
                        '銀行無し
                        Return BANK
                    Case 2
                        '支店無し
                        Return BRANCH
                End Select
            End If

            '手数料徴求支店(稼働金融機関イニファイル)     
            Dim MyBank As String = Jikinko

            Dim FieldName() As String = {"TESUUTYO_SIT_T", "TORIMATOME_SIT_T"}

            For Cnt As Integer = 0 To 1 Step 1

                BRANCH = GetIndex(FieldName(Cnt))
                If GCom.NzInt(sTORI(BRANCH, 4)) > 0 Then
                    Ret = GCom.CheckBankBranch(MyBank, sTORI(BRANCH, 4))
                    If Ret = 2 Then
                        '支店無し
                        Return BRANCH
                    End If
                End If
            Next Cnt

            Return -1
        Catch ex As Exception
            MSG = ex.Message
        End Try
    End Function

    '
    ' 機　能 : 委託者コード関連項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_008(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error

        Dim SQL As New StringBuilder(128)
        Dim REC As OracleDataReader = Nothing
        Try
            '取引先コード(主／副)
            Dim TORI_CODE As String = sTORI(GetIndex("TORIS_CODE_T"), 4)
            TORI_CODE &= sTORI(GetIndex("TORIF_CODE_T"), 4)

            '委託者コード
            Dim ITAKUIndex As Integer = GetIndex("ITAKU_CODE_T")
            Dim ITAKU_CODE As String = sTORI(ITAKUIndex, 4)

            '代表委託者コード
            Dim BAITAIIndex As Integer = GetIndex("ITAKU_KANRI_CODE_T")
            Dim BAITAI_KANRI_CODE As String = sTORI(BAITAIIndex, 4)

            '種別コード
            Dim SyubetuIndex As Integer = GetIndex("SYUBETU_T")
            Dim SYUBETU_CODE As String = sTORI(SyubetuIndex, 4)

            '委託者コードチェック
            SQL.AppendLine("SELECT COUNT(*) COUNTER FROM S_TORIMAST")
            SQL.AppendLine(" WHERE ITAKU_CODE_T = " & SQ(ITAKU_CODE))
            SQL.AppendLine(" AND SYUBETU_T = " & SQ(SYUBETU_CODE))
            SQL.AppendLine(" AND NOT TORIS_CODE_T||TORIF_CODE_T = " & SQ(TORI_CODE))

            If GCom.SetDynaset(SQL.ToString, REC) AndAlso _
                    REC.Read AndAlso GCom.NzDec(REC.Item("COUNTER"), 0) > 0 Then
                Index = ITAKUIndex

                If MessageBox.Show(MSG0037I, msgTitle, _
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    ErrMsgFlg = False
                    Return Index
                End If
            End If
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            If Not BAITAI_KANRI_CODE = ITAKU_CODE Then
                SQL = New StringBuilder(128)
                SQL.AppendLine("SELECT COUNT(*) COUNTER FROM S_TORIMAST")
                SQL.AppendLine(" WHERE ITAKU_KANRI_CODE_T = " & SQ(BAITAI_KANRI_CODE))
                SQL.AppendLine(" AND NOT TORIS_CODE_T||TORIF_CODE_T = " & SQ(TORI_CODE))

                If Not GCom.SetDynaset(SQL.ToString, REC) OrElse _
                        Not REC.Read OrElse GCom.NzDec(REC.Item("COUNTER"), 0) = 0 Then
                    Index = BAITAIIndex
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception(MSG0302W)
                End If
                If Not REC Is Nothing Then
                    REC.Close()
                    REC.Dispose()
                End If
            End If

            '振替・企業コード
            Dim FURI_CODE As String = sTORI(GetIndex("FURI_CODE_T"), 4)
            Dim KIGYO_CODE As String = sTORI(GetIndex("KIGYO_CODE_T"), 4)
            SQL = New StringBuilder(128)
            SQL.Append("SELECT COUNT(*) COUNTER FROM S_TORIMAST")
            SQL.Append(" WHERE FURI_CODE_T = " & SQ(FURI_CODE))
            SQL.Append(" AND KIGYO_CODE_T = " & SQ(KIGYO_CODE))
            SQL.Append(" AND NOT TORIS_CODE_T||TORIF_CODE_T = " & SQ(TORI_CODE))
       
            If GCom.SetDynaset(SQL.ToString, REC) AndAlso _
                    REC.Read AndAlso GCom.NzDec(REC.Item("COUNTER"), 0) > 0 Then
                If MessageBox.Show(MSG0038I, msgTitle, _
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    ErrMsgFlg = False
                    Return GetIndex("FURI_CODE_T")
                End If
            End If
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
        End Try

        Return Index
    End Function

    '
    ' 機　能 : 区分項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_009(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Try
            '暗号化有りの場合
            Dim ENC_KBN As Integer = GetIndex("ENC_KBN_T")
            Dim Temp2() As String = {"ENC_OPT1_T", "ENC_KEY1_T", "ENC_KEY2_T"}
            If GCom.NzInt(sTORI(ENC_KBN, 4)) = 1 Then
                For Cnt As Integer = 0 To 2 Step 1

                    Index = GetIndex(Temp2(Cnt))
                    If sTORI(Index, 4).Length = 0 Then

                        MSG = String.Format(MSG0305W, memTORI(ENC_KBN, 1))
                        Return Index
                    End If
                Next Cnt
            End If

            Dim Label_Kbn_Index As Integer = GetIndex("LABEL_KBN_T")

            '媒体コード,連携区分,フォーマット区分の相関
            Select Case GCom.NzInt(sTORI(GetIndex("BAITAI_CODE_T"), 4), 0)
                Case 0
                    '伝送(フォーマット区分)
                    '2017/05/16 タスク）西野 CHG 標準版修正（媒体とラベル区分の相関チェック追加）----------------- START
                    If sTORI(Label_Kbn_Index, 4) <> "0" Then
                        Index = Label_Kbn_Index
                        Throw New Exception(String.Format(MSG0381W, GetComboBoxValue(2, GetIndex("BAITAI_CODE_T"), 1)))
                    End If
                    'sTORI(Label_Kbn_Index, 4) = "0"
                    '2017/05/16 タスク）西野 CHG 標準版修正（媒体とラベル区分の相関チェック追加）----------------- END
                Case 1
                    '3.5FD(フォーマット区分)
                    '2017/05/16 タスク）西野 CHG 標準版修正（媒体とラベル区分の相関チェック追加）----------------- START
                    If sTORI(Label_Kbn_Index, 4) <> "0" Then
                        Index = Label_Kbn_Index
                        Throw New Exception(String.Format(MSG0381W, GetComboBoxValue(2, GetIndex("BAITAI_CODE_T"), 1)))
                    End If
                    'sTORI(Label_Kbn_Index, 4) = "0"
                    '2017/05/16 タスク）西野 CHG 標準版修正（媒体とラベル区分の相関チェック追加）----------------- END
                Case 4
                    '依頼書(フォーマット区分)
                    '2017/05/16 タスク）西野 CHG 標準版修正（媒体とラベル区分の相関チェック追加）----------------- START
                    If sTORI(Label_Kbn_Index, 4) <> "0" Then
                        Index = Label_Kbn_Index
                        Throw New Exception(String.Format(MSG0381W, GetComboBoxValue(2, GetIndex("BAITAI_CODE_T"), 1)))
                    End If
                    'sTORI(Label_Kbn_Index, 4) = "0"
                    '2017/05/16 タスク）西野 CHG 標準版修正（媒体とラベル区分の相関チェック追加）----------------- END
                Case 5
                    'MT(フォーマット区分)
                    If sTORI(Label_Kbn_Index, 4) = "" Then
                        Return Label_Kbn_Index
                    End If
                Case 6
                    'CMT(フォーマット区分)
                    If sTORI(Label_Kbn_Index, 4) = "" Then
                        Return Label_Kbn_Index
                    End If
                Case 9
                    '伝票(フォーマット区分)
                    '2017/05/16 タスク）西野 CHG 標準版修正（媒体とラベル区分の相関チェック追加）----------------- START
                    If sTORI(Label_Kbn_Index, 4) <> "0" Then
                        Index = Label_Kbn_Index
                        Throw New Exception(String.Format(MSG0381W, GetComboBoxValue(2, GetIndex("BAITAI_CODE_T"), 1)))
                    End If
                    'sTORI(Label_Kbn_Index, 4) = "0"
                    '2017/05/16 タスク）西野 CHG 標準版修正（媒体とラベル区分の相関チェック追加）----------------- END
                    '2017/05/16 タスク）西野 ADD 標準版修正（媒体とラベル区分の相関チェック追加）----------------- START
                Case 11, 12, 13, 14, 15 '外部媒体
                    If sTORI(Label_Kbn_Index, 4) <> "0" Then
                        Index = Label_Kbn_Index
                        Throw New Exception(String.Format(MSG0381W, GetComboBoxValue(2, GetIndex("BAITAI_CODE_T"), 1)))
                    End If
                    '2017/05/16 タスク）西野 ADD 標準版修正（媒体とラベル区分の相関チェック追加）----------------- END
                Case Else 'とりあえず・・・
                    Throw New Exception
            End Select

            Return -1
        Catch ex As Exception
            MSG = ex.Message
        End Try
        '2017/05/16 タスク）西野 ADD 標準版修正（媒体とラベル区分の相関チェック追加）----------------- START
        Return Index
        '2017/05/16 タスク）西野 ADD 標準版修正（媒体とラベル区分の相関チェック追加）----------------- END
    End Function

    '
    ' 機　能 : 日項目範囲チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_010(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            Dim KESSAI_KIJITSU_Index As Integer = GetIndex("KESSAI_KIJITSU_T")
            Dim kESSAI_KBN As Integer = GCom.NzInt(sTORI(GetIndex("KESSAI_KBN_T"), 4)) '決済区分
            Index = GetIndex("KESSAI_DAY_T")
            If kESSAI_KBN <> 99 Then    '決済対象外以外
                Select Case GCom.NzInt(sTORI(KESSAI_KIJITSU_Index, 4))
                    Case 0
                        '0:営業日数指定
                        '日数/基準日＜決済情報＞範囲チェック(MSG????W)
                        If GCom.NzInt(sTORI(Index, 4)) = 0 Then
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(MSG0301W)
                        End If
                    Case 1, 2
                        '1:基準日指定, 2:翌月基準日指定
                        Select Case GCom.NzInt(sTORI(Index, 4))
                            Case 1 To 31
                            Case Else  '日数/基準日＜決済情報＞範囲チェック(MSG0025W)
                                MsgIcon = MessageBoxIcon.Warning
                                Throw New Exception(MSG0025W)
                        End Select
                End Select
            End If

            '*** 手数料徴求区分が「一括徴求(1)」　　　　　　　　               ***
            '*** の場合,手数料徴求日数／基準日はチェック対象外                 ***

            '手数料徴求日数／基準日
            Index = GetIndex("TESUUTYO_DAY_T")
            If kESSAI_KBN <> 99 Then    '決済対象外以外
                '*** 都度徴求なら、IF先のチェック判定を行う            ***
                If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "0" Then
                    Select Case GCom.NzInt(sTORI(GetIndex("TESUUTYO_KIJITSU_T"), 4))
                        Case 0
                            '0, 営業日数指定
                            '日数/基準日＜決済情報＞範囲チェック(MSG????W)
                            If GCom.NzInt(sTORI(Index, 4)) = 0 Then
                                MsgIcon = MessageBoxIcon.Warning
                                Throw New Exception(MSG0301W)
                            ElseIf GCom.NzInt(sTORI(GetIndex("KESSAI_DAY_T"), 4)) > GCom.NzInt(sTORI(Index, 4)) Then

                                MsgIcon = MessageBoxIcon.Warning
                                Throw New Exception(MSG0306W)
                            End If
                        Case 1
                            '1, 基準日指定
                            Select Case sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)
                                Case "1"
                                    If GCom.NzInt(sTORI(GetIndex("KESSAI_DAY_T"), 4)) > GCom.NzInt(sTORI(Index, 4)) Then

                                        MsgIcon = MessageBoxIcon.Warning
                                        Throw New Exception(MSG0306W)
                                    Else
                                        If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "1" Then
                                            Select Case GCom.NzInt(sTORI(Index, 4))
                                                Case 1 To 31
                                                Case Else  '日数/基準日＜決済情報＞範囲チェック
                                                    MsgIcon = MessageBoxIcon.Warning
                                                    Throw New Exception(MSG0025W)
                                            End Select
                                        End If
                                    End If
                                Case Else
                                    If GCom.NzInt(sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)) > GCom.NzInt(sTORI(Index, 4)) Then

                                        MsgIcon = MessageBoxIcon.Warning
                                        Throw New Exception(MSG0306W)
                                    End If
                            End Select
                        Case 2
                            '2, 翌月基準日指定
                            Select Case sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)
                                Case "1"
                                    If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "1" Then
                                        Select Case GCom.NzInt(sTORI(Index, 4))
                                            Case 1 To 31
                                            Case Else
                                                Return Index
                                        End Select
                                    End If
                                Case Else
                                    If GCom.NzInt(sTORI(GetIndex("KESSAI_DAY_T"), 4)) > GCom.NzInt(sTORI(Index, 4)) Then

                                        MsgIcon = MessageBoxIcon.Warning
                                        Throw New Exception(MSG0306W)
                                    Else
                                        If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "1" Then
                                            Select Case GCom.NzInt(sTORI(Index, 4))
                                                Case 1 To 31
                                                Case Else  '日数/基準日＜決済情報＞範囲チェック
                                                    MsgIcon = MessageBoxIcon.Warning
                                                    Throw New Exception(MSG0025W)
                                            End Select
                                        End If
                                    End If
                            End Select
                    End Select
                End If
            End If

            '手数料区分
            Select Case sTORI(GetIndex("TESUUTYO_KBN_T"), 4)
                Case Is = "0", "2", "3"
                    '都度徴求,特別免除,別途徴求
                    Index = GetIndex("TESUUMAT_NO_T")
                    If GCom.NzInt(sTORI(Index, 4)) > 0 Then

                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0307W, "一括徴求"))
                    End If

                    Index = GetIndex("TESUUMAT_MONTH_T")
                    If GCom.NzInt(sTORI(Index, 4)) > 0 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0307W, "一括徴求"))
                    End If

                    Index = GetIndex("TESUUMAT_ENDDAY_T")
                    If GCom.NzInt(sTORI(Index, 4)) > 0 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0307W, "一括徴求"))
                    End If

                Case Is = "1"
                    '一括徴求
                    Index = GetIndex("TESUUMAT_MONTH_T")
                    If GCom.NzInt(sTORI(Index, 4)) = 0 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0285W, "集計基準月"))
                    End If

                    Index = GetIndex("TESUUMAT_ENDDAY_T")
                    If GCom.NzInt(sTORI(Index, 4)) = 0 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0285W, "集計終了日"))
                    End If

                    Index = GetIndex("TESUUMAT_NO_T")
                    If GCom.NzInt(sTORI(Index, 4)) = 0 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(String.Format(MSG0285W, "手数料集計周期"))
                    Else
                        If Not (12 Mod GCom.NzInt(sTORI(Index, 4))) = 0 Then

                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(MSG0308W)
                        End If
                    End If
                Case Else
                    '他
            End Select

            '手数料徴求期日区分
            Index = GetIndex("TESUUTYO_KIJITSU_T")

            '都度徴求なら、IF先の判定を行う
            If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "0" Then

                Select Case sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)
                    Case "1"
                        If GCom.NzInt(sTORI(GetIndex("KESSAI_KIJITSU_T"), 4)) > GCom.NzInt(sTORI(Index, 4)) Then

                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(MSG0306W)
                        End If
                    Case Else
                        If Not sTORI(GetIndex("KESSAI_KIJITSU_T"), 4) = sTORI(Index, 4) Then
                            Select Case GCom.NzInt(sTORI(Index, 4))
                                Case 3, 4
                                    '3, 振込日指定, 4, 確保日指定
                                Case Else
                                    MsgIcon = MessageBoxIcon.Warning
                                    Throw New Exception(MSG0309W)
                            End Select
                        End If
                End Select
            End If
            '月範囲
            If sTORI(GetIndex("TESUUTYO_KBN_T"), 4) = "1" Then
                Index = GetIndex("TESUUMAT_MONTH_T")
                Select Case GCom.NzInt(sTORI(Index, 4))
                    Case 1 To 12
                    Case Else
                        Return Index
                End Select
            End If

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    ' 機　能 : 決済関連項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_011(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            Select Case GCom.NzDec(sTORI(GetIndex("TESUUTYO_KBN_T"), 4), "")
                Case "0"
                    '都度徴求
                    Index = GetIndex("KESSAI_KBN_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        '決済するなら区分毎に相関チェック必要
                        Case "00" '為替請求
                        Case "99" '対象外
                        Case Else
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0311W, sTORI(Index, 0)))
                    End Select
                Case "1"  '一括徴求
                    '2010/01/20 追加 =======================
                    ' 基準日指定または翌月基準日指定のみ可
                    Index = GetIndex("TESUUTYO_KIJITSU_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        Case "1", "2"
                            '手数料徴求期日区分が基準日指定、翌月基準日指定
                        Case Else
                            'それ以外はエラー
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0312W, "営業日数指定"))
                    End Select
                    '=======================================
                    Index = GetIndex("KESSAI_KBN_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        '決済するなら区分毎に相関チェック必要
                        Case "00" '為替請求
                        Case "99" '決済対象外
                        Case Else
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0311W, sTORI(Index, 0)))
                    End Select
                Case "2" '特別免除
                    Index = GetIndex("KESSAI_KBN_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        '決済するなら区分毎に相関チェック必要
                        Case "00" '為替請求
                        Case "99" '決済対象外
                        Case Else
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0311W, sTORI(Index, 0)))
                    End Select
                Case "3"    '別途徴求
                    '2010/01/20 追加 =======================
                    ' 基準日指定または翌月基準日指定のみ可
                    Index = GetIndex("TESUUTYO_KIJITSU_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        Case "1", "2"
                            '手数料徴求期日区分が基準日指定、翌月基準日指定
                        Case Else
                            'それ以外はエラー
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0312W, "営業日数指定"))
                    End Select
                    '=======================================
                    Index = GetIndex("KESSAI_KBN_T")
                    Select Case GCom.NzDec(sTORI(Index, 4), "")
                        '決済するなら区分毎に相関チェック必要
                        Case "00" '為替請求
                        Case "99" '決済対象外
                        Case Else
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(String.Format(MSG0311W, sTORI(Index, 0)))
                    End Select
            End Select

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    '
    ' 機　能 : 備考項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : @MM-DD@ , @TEN@ , @KEN@
    '    
    Private Function CheckMutualRelation_012(ByRef MSG As String) As Integer

        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            Dim Counter As Integer
            Dim Temp() As String = {"@MM-DD@", "@TEN@", "@KEN@"}

            '備考１
            Index = GetIndex("BIKOU1_T")
            Counter = 0
            For Cnt As Integer = 0 To 2 Step 1

                If sTORI(Index, 4).IndexOf(Temp(Cnt)) >= 0 Then
                    Counter += 1
                    If Counter > 1 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(MSG0313W)
                    End If
                End If
            Next Cnt

            '備考２
            Index = GetIndex("BIKOU2_T")
            Counter = 0
            For Cnt As Integer = 0 To 2 Step 1

                If sTORI(Index, 4).IndexOf(Temp(Cnt)) >= 0 Then
                    Counter += 1
                    If Counter > 1 Then
                        MsgIcon = MessageBoxIcon.Warning
                        Throw New Exception(MSG0313W)
                    End If
                End If
            Next Cnt

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    '
    ' 機　能 : 依頼書/伝票項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_013(ByRef MSG As String) As Integer
        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            Dim Baitai_CODE As Integer '媒体コード
            Baitai_CODE = GCom.NzInt(sTORI(GetIndex("BAITAI_CODE_T"), 4))

            '期日管理
            Index = GetIndex("KIJITU_KANRI_T")
            Dim KIJITU_KANRI As String = sTORI(Index, 4).Trim
            If Baitai_CODE = 4 OrElse Baitai_CODE = 9 Then
                '伝票・依頼書の場合期日管理あり
                If KIJITU_KANRI = "0" Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception(MSG0504W)
                End If
            End If

            'サイクル管理
            Index = GetIndex("CYCLE_T")
            Dim CYCLE As String = sTORI(Index, 4).Trim
            If Baitai_CODE = 4 OrElse Baitai_CODE = 9 Then
                '伝票・依頼書の場合複数持込なし
                If CYCLE = "1" Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception(MSG0503W)
                End If
            End If

            Index = 0

            '持込期日
            Dim Item As String = sTORI(GetIndex("MOTIKOMI_KIJITSU_T"), 4).Trim

            If Baitai_CODE = 4 OrElse Baitai_CODE = 9 Then
                '伝票・依頼書の場合持込期日必須チェック
                If Item = "" Then
                    MsgIcon = MessageBoxIcon.Warning
                    '持込期日必須チェック(MSG0180W)
                    Throw New Exception(MSG0180W)
                End If
            End If

            If Not Item = "" Then
                '持ち込み期日範囲チェック(MSG0025W)
                If GCom.NzInt(Item) < 0 OrElse GCom.NzInt(Item) > 31 Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception(MSG0025W)
                End If
            End If
            '日数/基準日<依頼書/伝票>
            Item = sTORI(GetIndex("IRAISYO_YDATE_T"), 4).Trim
            If Baitai_CODE = 4 OrElse Baitai_CODE = 9 Then
                '伝票・依頼書の場合、日数/基準日<依頼書/伝票>必須チェック
                If Item = "" Then
                    '2012/11/12 saitou 標準修正 MODIFY ----------------------------------------------->>>>
                    Index = GetIndex("IRAISYO_YDATE_T")
                    '2012/11/12 saitou 標準修正 MODIFY -----------------------------------------------<<<<
                    MsgIcon = MessageBoxIcon.Warning
                    '日数/基準日<依頼書/伝票>必須チェック(MSG0182W)
                    Throw New Exception(MSG0182W)
                End If
            End If

            If Not Item = "" Then
                Index = GetIndex("IRAISYO_YDATE_T")
                Dim Kijitsu As Integer = GCom.NzInt(sTORI(GetIndex("IRAISYO_KIJITSU_T"), 4))
                '日付区分(依頼書)が1:基準日指定 2:翌月基準日指定の場合 
                Select Case Kijitsu
                    Case 0 '営業日数指定
                        '日数/基準日<依頼書/伝票>範囲チェック
                        If GCom.NzInt(Item) < 1 Then
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(MSG0301W)
                        End If
                    Case 1, 2
                        '日数/基準日<依頼書/伝票>範囲チェック(MSG0025W)
                        If GCom.NzInt(Item) < 0 OrElse GCom.NzInt(Item) > 31 Then
                            MsgIcon = MessageBoxIcon.Warning
                            Throw New Exception(MSG0025W)
                        End If
                    Case Else

                End Select
            End If

            Return -1

        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    '
    ' 機　能 : 決済項目チェック
    '
    ' 戻り値 : チェック抵触コントロール配列位置
    '
    ' 引き数 : ARG1 - エラーメッセージ
    '
    ' 備　考 : なし
    '    
    Private Function CheckMutualRelation_014(ByRef MSG As String) As Integer
        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            Dim Item As String = sTORI(GetIndex("MOTIKOMI_KIJITSU_T"), 4).Trim
            Dim Kessai_KBN As Integer '決済区分
            Kessai_KBN = GCom.NzInt(sTORI(GetIndex("KESSAI_KBN_T"), 4))
            If Kessai_KBN <> 99 Then    '決済対象外


            End If

            If Kessai_KBN = 4 OrElse Kessai_KBN = 9 Then
                '伝票・依頼書の場合持込期日必須チェック
                If Item = "" Then
                    MsgIcon = MessageBoxIcon.Warning
                    '持込期日必須チェック(MSG0180W)
                    Throw New Exception(MSG0180W)
                End If
            End If

            If Not Item = "" Then
                '持ち込み期日範囲チェック(MSG0025W)
                If GCom.NzInt(Item) < 0 OrElse GCom.NzInt(Item) > 31 Then
                    MsgIcon = MessageBoxIcon.Warning
                    Throw New Exception(MSG0025W)
                End If
            End If
            '日数/基準日<依頼書/伝票>
            Item = sTORI(GetIndex("IRAISYO_YDATE_T"), 4).Trim
            If Kessai_KBN = 4 OrElse Kessai_KBN = 9 Then
                '伝票・依頼書の場合、日数/基準日<依頼書/伝票>必須チェック
                If Item = "" Then
                    MsgIcon = MessageBoxIcon.Warning
                    '日数/基準日<依頼書/伝票>必須チェック(MSG0182W)
                    Throw New Exception(MSG0182W)
                End If
            End If

            If Not Item = "" Then
                Dim Kijitsu As Integer = GCom.NzInt(sTORI(GetIndex("IRAISYO_KIJITSU_T"), 4))
                '日付区分(依頼書)が1:基準日指定 2:翌月基準日指定の場合 
                Select Case Kijitsu
                    Case 0 '0:営業日数指定

                    Case 1, 2
                        '日数/基準日<依頼書/伝票>範囲チェック(MSG0025W)
                        If GCom.NzInt(Item) < 0 OrElse GCom.NzInt(Item) > 31 Then
                            MsgIcon = MessageBoxIcon.Warning
                            Index = GetIndex("IRAISYO_YDATE_T")
                            Throw New Exception(MSG0025W)
                        End If
                    Case Else
                End Select
            End If
            Return -1

        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    '摘要チェック
    Private Function CheckMutualRelation_004(ByRef MSG As String) As Integer
        '項目基本配列位置
        Dim Index As Integer = 0
        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Error
        Try
            '摘要区分
            Select Case GCom.NzInt(sTORI(GetIndex("TEKIYOU_KBN_T"), 4))
                Case Is = 0
                    'カナ摘要(摘要区分は必須)

                    'Index = GetIndex("KTEKIYOU_T")
                    'If sTORI(Index, 4).Length = 0 Then
                    '    'カナ摘要の設定が抜けている
                    '    MsgIcon = MessageBoxIcon.Warning
                    '    'カナ摘要必須チェック(MSG0212W)
                    '    Throw New Exception(MSG0212W)
                    'End If

                Case Is = 1
                    '漢字摘要(摘要区分は必須)

                    Index = GetIndex("NTEKIYOU_T")
                    If sTORI(Index, 4).Length = 0 Then
                        '漢字摘要の設定が抜けている
                        MsgIcon = MessageBoxIcon.Warning
                        '漢字摘要必須チェック(MSG0213W)
                        Throw New Exception(MSG0213W)
                    End If
                Case Else
                    '可変摘要(摘要区分は必須)

            End Select

            Return -1
        Catch ex As Exception
            If MsgIcon = MessageBoxIcon.Error Then

            End If
            MSG = ex.Message
        End Try

        Return Index
    End Function

    '
    ' 機　能 : テキストボックス名で対のラベルオブジェクトを返す
    '
    ' 戻り値 : Label Object
    '
    ' 引き数 : ARG1 - ラベル名
    '
    ' 備　考 : なし
    '    
    Private Function GetLabel(ByVal TextBoxName As String) As Label
        Try
            Select Case TextBoxName.ToUpper
                Case "TKIN_NO_T".ToUpper
                    Return TKIN_NO_TL
                Case "TUKEKIN_NO_T".ToUpper
                    Return TUKEKIN_NO_TL
                Case "TSIT_NO_T".ToUpper
                    Return TSIT_NO_TL
                Case "TUKESIT_NO_T".ToUpper
                    Return TUKESIT_NO_TL
                Case "TESUUTYO_SIT_T".ToUpper
                    Return TESUUTYO_SIT_TL
                Case "TORIMATOME_SIT_T".ToUpper
                    Return TORIMATOME_SIT_TL
                Case "HENKYAKU_SIT_NO_T".ToUpper
                    Return HENKYAKU_SIT_NO_TL
                Case Else
                    Return Nothing
            End Select
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    '半角英数カナ評価(全角文字抹消)


    '
    ' 機　能 : 取引先主／副保全可否
    '
    ' 戻り値 : 通常とおり初期化 = True
    ' 　　　   初期化回避　　　 = False
    '
    ' 引き数 : ARG1 - 参照=1. 初期化=0
    ' 　　　   ARG2 - コントロール名
    '
    ' 備　考 : IF文支援関数(2008.04.18 By Astar)
    '    
    Private Function FormInitializeSub(ByVal SEL As Integer, ByVal CTL_NAME As String) As Boolean
        Try
            Select Case SEL
                Case 0
                Case 1
                    Select Case CTL_NAME.ToUpper
                        Case "TORIS_CODE_T", "TORIF_CODE_T"

                            Return False
                    End Select
            End Select
        Catch ex As Exception

        End Try

        Return True
    End Function

    '
    ' 機　能 : 配列番号からラベルオブジェクトを探して設定する
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - 値設定コントロールObject格納Index
    ' 　　　   ARG2 - 設定タイプ
    '
    ' 備　考 : 見つからない場合には何もしない
    '    
    Private Sub GetLabelColor(ByVal Index As Integer)
        Try
            If sTORI(Index, 5) = "1" Then
                Dim CTLName As String = sTORI(Index, 1)
                CTLName = CTLName.Substring(0, CTLName.Length - 1)
                CTLName &= "L"

                For Each CTL As Control In oTORI(Index).Parent.Controls

                    If CTL.Name.ToUpper = CTLName.ToUpper Then

                        With CType(CTL, Label)

                            .ForeColor = System.Drawing.Color.Maroon                '色
                            .Font = New Font(.Font, .Font.Style Or FontStyle.Bold)  '太さ
                        End With
                    End If
                Next
            End If
        Catch ex As Exception

        End Try
    End Sub

    '
    ' 機　能 : 既存MAST参照共通処理
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - なし
    '
    ' 備　考 : 取引先マスタ参照結果の画面展開
    '    
    Public Function CmdSelect_Action() As Boolean
        Dim Ret As Integer
        Dim Cnt As Integer
        Dim onDate As DateTime
        Dim onText(2) As Integer
        Dim Index As Integer
        Dim REC As OracleDataReader = Nothing
        Try
            CmdSelect_Action = False
            Me.SuspendLayout()
            Dim SQL As New StringBuilder(128)
            SQL.AppendLine("SELECT *")
            SQL.AppendLine(" FROM ")
            SQL.AppendLine("     S_TORIMAST_VIEW")
            SQL.AppendLine(" WHERE ")
            SQL.AppendLine("     FSYORI_KBN_T = '3'")
            SQL.AppendLine(" AND TORIS_CODE_T = " & SQ(CType(oTORI(GetIndex("TORIS_CODE_T")), TextBox).Text.Trim))
            SQL.AppendLine(" AND TORIF_CODE_T = " & SQ(CType(oTORI(GetIndex("TORIF_CODE_T")), TextBox).Text.Trim))

            If GCom.SetDynaset(SQL.ToString, REC) AndAlso REC.Read Then

                For Idx As Integer = 0 To REC.FieldCount - 1 Step 1

                    '変更前の配列へ値を格納する
                    Index = GetIndex(REC.GetName(Idx))
                    sTORI(Index, 3) = REC(Idx).ToString.Trim

                    Select Case True
                        Case TypeOf oTORI(Index) Is TextBox
                            'テキストボックス

                            Select Case sTORI(Index, 1).ToUpper
                                Case "KAISI_DATE_T", "SYURYOU_DATE_T", "KEIYAKU_DATE_T"

                                    Dim OK_FLG As Boolean = False

                                    If sTORI(Index, 3).Length = 8 AndAlso _
                                        GCom.IsNumber(sTORI(Index, 3)) Then

                                        onText(0) = GCom.NzInt(sTORI(Index, 3).Substring(0, 4))
                                        onText(1) = GCom.NzInt(sTORI(Index, 3).Substring(4, 2))
                                        onText(2) = GCom.NzInt(sTORI(Index, 3).Substring(6))

                                        Ret = GCom.SET_DATE(onDate, onText)
                                        If Ret = -1 Then

                                            OK_FLG = True

                                            With Me
                                                Select Case sTORI(Index, 1).ToUpper
                                                    Case "KAISI_DATE_T"
                                                        .KAISI_DATE_T.Text = onDate.Year.ToString.PadLeft(4, "0"c)
                                                        .KAISI_DATE_T1.Text = onDate.Month.ToString.PadLeft(2, "0"c)
                                                        .KAISI_DATE_T2.Text = onDate.Day.ToString.PadLeft(2, "0"c)
                                                    Case "SYURYOU_DATE_T"
                                                        .SYURYOU_DATE_T.Text = onDate.Year.ToString.PadLeft(4, "0"c)
                                                        .SYURYOU_DATE_T1.Text = onDate.Month.ToString.PadLeft(2, "0"c)
                                                        .SYURYOU_DATE_T2.Text = onDate.Day.ToString.PadLeft(2, "0"c)
                                                    Case "KEIYAKU_DATE_T"
                                                        .KEIYAKU_DATE_T.Text = onDate.Year.ToString.PadLeft(4, "0"c)
                                                        .KEIYAKU_DATE_T1.Text = onDate.Month.ToString.PadLeft(2, "0"c)
                                                        .KEIYAKU_DATE_T2.Text = onDate.Day.ToString.PadLeft(2, "0"c)
                                                End Select
                                            End With
                                        Else
                                            If sTORI(Index, 1).ToUpper = "KEIYAKU_DATE_T" AndAlso sTORI(Index, 3) = "00000000" Then
                                                Me.KEIYAKU_DATE_T.Text = "0000"
                                                Me.KEIYAKU_DATE_T1.Text = "00"
                                                Me.KEIYAKU_DATE_T2.Text = "00"
                                                OK_FLG = True
                                            End If
                                        End If
                                    End If

                                    If Not OK_FLG Then

                                        If GCom.NzLong(sTORI(Index, 3)) = 99999999 Then
                                            With Me
                                                Select Case sTORI(Index, 1).ToUpper
                                                    Case "KAISI_DATE_T"
                                                        .KAISI_DATE_T.Text = "2000"
                                                        .KAISI_DATE_T1.Text = "01"
                                                        .KAISI_DATE_T2.Text = "01"
                                                    Case "SYURYOU_DATE_T"
                                                        .SYURYOU_DATE_T.Text = "2099"
                                                        .SYURYOU_DATE_T1.Text = "12"
                                                        .SYURYOU_DATE_T2.Text = "31"
                                                    Case "KEIYAKU_DATE_T"
                                                        .KEIYAKU_DATE_T.Text = "2000"
                                                        .KEIYAKU_DATE_T1.Text = "01"
                                                        .KEIYAKU_DATE_T2.Text = "01"
                                                End Select
                                            End With
                                        Else
                                            With Me
                                                Select Case sTORI(Index, 1).ToUpper
                                                    Case "KAISI_DATE_T"
                                                        .KAISI_DATE_T.Text = "2000"
                                                        .KAISI_DATE_T1.Text = "01"
                                                        .KAISI_DATE_T2.Text = "01"
                                                    Case "SYURYOU_DATE_T"
                                                        .SYURYOU_DATE_T.Text = "2099"
                                                        .SYURYOU_DATE_T1.Text = "12"
                                                        .SYURYOU_DATE_T2.Text = "31"
                                                    Case "KEIYAKU_DATE_T"
                                                        .KEIYAKU_DATE_T.Text = "2000"
                                                        .KEIYAKU_DATE_T1.Text = "01"
                                                        .KEIYAKU_DATE_T2.Text = "01"
                                                End Select
                                            End With
                                        End If
                                    End If
                                Case "SOURYO_T", "KOTEI_TESUU1_T", "KOTEI_TESUU2_T", _
                                       "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                                        "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                                        "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"
                                    '金額設定項目の場合
                                    oTORI(Index).Text = _
                                    String.Format("{0:#,##0}", GCom.NzDec(sTORI(Index, 3)))
                                Case "TKIN_NO_T", "TUKEKIN_NO_T"
                                    '金融機関
                                    oTORI(Index).Text = sTORI(Index, 3)
                                    GetLabel(sTORI(Index, 1)).Text = GCom.GetBKBRName(sTORI(Index, 3), "", 30)
                                Case "TSIT_NO_T", "TUKESIT_NO_T", "TESUUTYO_SIT_T", _
                                     "TORIMATOME_SIT_T"
                                    '支店
                                    oTORI(Index).Text = sTORI(Index, 3)
                                    Dim BKCode As String = ""
                                    Dim BRCode As String = sTORI(Index, 3)
                                    Select Case sTORI(Index, 1).ToUpper
                                        Case "TSIT_NO_T".ToUpper
                                            BKCode = Me.TKIN_NO_T.Text
                                        Case "TUKESIT_NO_T".ToUpper
                                            BKCode = Me.TUKEKIN_NO_T.Text
                                        Case Else
                                            If BRCode.Trim.Length > 0 Then
                                                BKCode = Jikinko
                                            End If
                                    End Select
                                    If BRCode <> "" Then
                                        GetLabel(sTORI(Index, 1)).Text = GCom.GetBKBRName(BKCode, BRCode, 30)
                                    End If
                                Case "ITAKU_CODE_T"
                                    '委託者コード
                                    oTORI(Index).Text = sTORI(Index, 3)

                                Case "KIGYO_CODE_T"
                                    '企業コード
                                    If Center = "0" Then
                                        '信組の場合はそのまま使用
                                        oTORI(Index).Text = sTORI(Index, 3)
                                    Else
                                        '5桁の内1桁目は固定のため、2文字目以降を設定する
                                        oTORI(Index).Text = Mid(sTORI(Index, 3), 2)
                                    End If
                                Case Else
                                    '金額項目以外の場合
                                    oTORI(Index).Text = sTORI(Index, 3)
                            End Select

                        Case TypeOf oTORI(Index) Is ComboBox
                            'コンボボックス

                            '照合要否はデフォルト値が "1" のため特別な処置を行う。
                            Dim IntTemp As Integer = -1
                            If sTORI(Index, 1).ToUpper = "CHECK_KBN_T".ToUpper AndAlso _
                                (IsDBNull(REC(Idx)) OrElse REC(Idx).ToString.Trim.Length = 0) Then
                                IntTemp = 0
                            Else
                                If sTORI(Index, 3).Length > 0 Then
                                    IntTemp = GCom.NzInt(sTORI(Index, 3))
                                End If
                            End If

                            If IntTemp >= 0 Then
                                Dim CMB As ComboBox = CType(oTORI(Index), ComboBox)
                                For Cnt = 0 To CMB.Items.Count - 1 Step 1

                                    CMB.SelectedIndex = Cnt

                                    If GCom.GetComboBox(CMB) = IntTemp Then
                                        Exit For
                                    End If
                                Next Cnt

                                If Cnt >= CMB.Items.Count AndAlso CMB.Items.Count > 0 Then

                                    CMB.SelectedIndex = -1
                                End If

                                memTORI(Index, 0) = CMB.Text
                            End If

                        Case TypeOf oTORI(Index) Is CheckBox
                            'チェックボックス

                            Dim CHK As CheckBox = CType(oTORI(Index), CheckBox)
                            CHK.Checked = (sTORI(Index, 3) = "1")

                        Case TypeOf oTORI(Index) Is Label

                            Dim LBL As Label = CType(oTORI(Index), Label)

                            Select Case LBL.Name
                                Case "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"
                                    If GCom.NzDec(sTORI(GetIndex("TESUU_TABLE_ID_T"), 4), "").Length > 0 Then
                                        Dim onDec As Decimal = GCom.NzDec(sTORI(Index, 3))
                                        LBL.Text = String.Format("{0:#,##0}", onDec)
                                    End If
                                Case "YOBI1_T", "YOBI2_T", "YOBI3_T", "YOBI4_T", "YOBI5_T"
                                    LBL.Text = GCom.NzStr(sTORI(Index, 3))
                                    sTORI(Index, 4) = sTORI(Index, 3)
                                Case Else
                                    'ラベル(作成日／更新日)
                                    If sTORI(Index, 3).Length >= 8 AndAlso _
                                        GCom.IsNumber(sTORI(Index, 3)) Then

                                        onDate = GCom.SET_DATE(sTORI(Index, 3))

                                        If Not onDate = Nothing Then

                                            Dim Temp As String = "yyyy年MM月dd日 HH時mm分ss秒"
                                            LBL.Text = String.Format("{0:" & Temp & "}", onDate)

                                            sTORI(Index, 4) = sTORI(Index, 3)
                                        End If
                                    End If
                            End Select
                    End Select
                Next Idx

                Call SetControlToReadOnly(False)
                Me.SSTab.SelectedIndex = 0
                Me.BAITAI_CODE_T.Focus()
            Else
                Dim MSG As String = MSG0063W
                Dim DRet As DialogResult = MessageBox.Show(MSG, _
                   msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.TORIS_CODE_T.Focus()
                Return False
            End If
            CmdSelect_Action = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("マスタ参照", "失敗", ex.ToString)
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            Me.ResumeLayout()
        End Try
    End Function

    '
    ' 機　能 : 設定データの参照
    '
    ' 戻り値 : なし
    '
    ' 引き数 : ARG1 - なし
    '
    ' 備　考 : 値参考用の関数
    '    
    Private Function GetControlsValue() As Boolean
        Dim Index As Integer
        Try
            For Index = 1 To MaxColumns Step 1

                Dim Temp As String = ""

                Select Case True
                    Case (TypeOf oTORI(Index) Is TextBox)
                        'テキストボックス

                        Select Case CType(oTORI(Index), TextBox).Name.ToUpper
                            Case "KAISI_DATE_T", "SYURYOU_DATE_T", "KEIYAKU_DATE_T"
                                '日付関係(複数の領域：年月日)
                                With Me
                                    Select Case CType(oTORI(Index), TextBox).Name.ToUpper
                                        Case "KAISI_DATE_T"
                                            Temp = .KAISI_DATE_T.Text.PadLeft(4).Replace(" "c, "0"c)
                                            Temp &= .KAISI_DATE_T1.Text.PadLeft(2).Replace(" "c, "0"c)
                                            Temp &= .KAISI_DATE_T2.Text.PadLeft(2).Replace(" "c, "0"c)
                                        Case "SYURYOU_DATE_T"
                                            Temp = .SYURYOU_DATE_T.Text.PadLeft(4).Replace(" "c, "0"c)
                                            Temp &= .SYURYOU_DATE_T1.Text.PadLeft(2).Replace(" "c, "0"c)
                                            Temp &= .SYURYOU_DATE_T2.Text.PadLeft(2).Replace(" "c, "0"c)
                                        Case "KEIYAKU_DATE_T"
                                            Temp = .KEIYAKU_DATE_T.Text.PadLeft(4).Replace(" "c, "0"c)
                                            Temp &= .KEIYAKU_DATE_T1.Text.PadLeft(2).Replace(" "c, "0"c)
                                            Temp &= .KEIYAKU_DATE_T2.Text.PadLeft(2).Replace(" "c, "0"c)
                                    End Select

                                    '空白であると推定できる場合には "" とする
                                    If GCom.NzLong(Temp) = 0 Then

                                        Temp = "00000000"
                                    End If
                                End With
                            Case "SOURYO_T", "KOTEI_TESUU1_T", "KOTEI_TESUU2_T", _
                                 "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"
                                '金額設定項目の場合

                                Temp = CType(oTORI(Index), TextBox).Text.Trim.Replace(","c, "")
                            Case "KIGYO_CODE_T"
                                If Center = "0" Then
                                    Temp = CType(oTORI(Index), TextBox).Text.Trim
                                Else
                                    '企業コード未入力の場合は設定しない
                                    If CType(oTORI(Index), TextBox).Text.Trim = "" Then
                                        Temp = CType(oTORI(Index), TextBox).Text.Trim
                                    Else
                                        Temp = Center & CType(oTORI(Index), TextBox).Text.Trim
                                    End If
                                End If

                            Case Else
                                '日付以外の設定値(単独の領域)

                                Temp = CType(oTORI(Index), TextBox).Text.Trim
                        End Select

                    Case (TypeOf oTORI(Index) Is ComboBox)
                        'コンボボックス

                        Dim Ret As Integer = GCom.GetComboBox(CType(oTORI(Index), ComboBox))

                        'コンボ蓄積Add無しや非選択時には -1 が返る(PG仕様)
                        If Ret >= 0 Then
                            Temp = Ret.ToString
                        End If
                        memTORI(Index, 1) = CType(oTORI(Index), ComboBox).Text

                    Case (TypeOf oTORI(Index) Is CheckBox)
                        'チェックボックス

                        If CType(oTORI(Index), CheckBox).Checked Then
                            Temp = 1.ToString
                        Else
                            Temp = 0.ToString
                        End If
                End Select

                '参照結果を格納する
                If Not TypeOf oTORI(Index) Is Label Then

                    Select Case nTORI(Index, 1)
                        Case 0
                            '通常データ
                            sTORI(Index, 4) = Temp
                        Case Else
                            '区分等で頭Zero埋めするデータ
                            If Temp.Trim.Length > 0 Then
                                Dim FormatString As String = New String("0"c, nTORI(Index, 0))
                                sTORI(Index, 4) = String.Format("{0:" & FormatString & "}", GCom.NzDec(Temp))
                            Else
                                sTORI(Index, 4) = Temp
                            End If
                    End Select
                End If
            Next Index
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("設定データの参照", "失敗", ex.ToString)
        End Try
    End Function

    '
    ' 機　能 : 項目データ変更有無チェック
    '
    ' 戻り値 : 変更あり = True
    ' 　　　   変更なし = False
    '
    ' 引き数 : ARG1 - 登録 = 0, 更新 = 1
    ' 　　　   
    '
    ' 備　考 : 変更がある場合には印刷用CSV作成を行う
    '    
    Private Function CheckUpdateItem(ByVal aSEL As Integer) As Boolean
        Dim Index As Integer

        '変更の有無を検証する(更新日は除外)
        For Index = 1 To MaxColumns Step 1
            If Not sTORI(Index, 3) = sTORI(Index, 4) AndAlso sTORI(Index, 1) <> "KOUSIN_DATE_T" AndAlso sTORI(Index, 1) <> "SAKUSEI_DATE_T" _
               AndAlso Not TypeOf oTORI(Index) Is Label AndAlso sTORI(Index, 1) <> "KOUSIN_SIKIBETU_T" Then

                Exit For
            End If
        Next

        '更新項目が無ければ関数を抜ける
        If aSEL = 1 AndAlso Index > MaxColumns Then
            Return False
        End If

        '登録の場合は抜ける
        If aSEL = 0 Then
            Return True
        End If

        Dim Prn_KFSP017 As KFSPxxx = New KFSPxxx("KFSP018")
        Try
            strCSV_FILE_NAME = Prn_KFSP017.CreateCsvFile
            updCnt = 0

            '印刷用のパラメータ設定
            Dim Today As String = Now.ToString("yyyyMMdd")
            Dim Time As String = Now.ToString("HHmmss")

            Dim Machine As String = Environment.MachineName


            For Index = 1 To MaxColumns Step 1
                If Not TypeOf oTORI(Index) Is Label Then

                    Select Case True

                        Case sTORI(Index, 1).ToUpper = "SAKUSEI_DATE_T"
                            '作成日=何もしない
                        Case sTORI(Index, 1).ToUpper = "KOUSIN_DATE_T"
                            '更新日=何もしない
                        Case sTORI(Index, 1).ToUpper = "KOUSIN_SIKIBETU_T"
                            '更新識別=何もしない
                        Case Else
                            If aSEL = 0 OrElse Not sTORI(Index, 3) = sTORI(Index, 4) Then

                                Dim DQ As String = ControlChars.Quote

                                Dim SetData(1) As String
                                Select Case sTORI(Index, 1)
                                    Case "TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                                         "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T", "SOURYO_T", "KOTEI_TESUU1_T", _
                                         "KOTEI_TESUU2_T"
                                        SetData(0) = String.Format("{0:#,##0}", GCom.NzDec(sTORI(Index, 3)))
                                        SetData(1) = String.Format("{0:#,##0}", GCom.NzDec(sTORI(Index, 4)))
                                    Case Else
                                        If TypeOf oTORI(Index) Is ComboBox Then
                                            SetData(0) = GetComboBoxValue(aSEL, Index, 0)
                                            SetData(1) = GetComboBoxValue(aSEL, Index, 1)
                                        Else
                                            SetData(0) = sTORI(Index, 3)
                                            SetData(1) = sTORI(Index, 4)
                                        End If
                                End Select

                                If aSEL = 0 Then
                                    SetData(0) = "新規登録"
                                End If

                                If SetData(0) <> SetData(1) Then
                                    '更新帳票印刷用CSVファイル作成
                                    With Prn_KFSP017
                                        .OutputCsvData(Today, True)                                 '処理日
                                        .OutputCsvData(Time, True)                                  'タイムスタンプ
                                        .OutputCsvData(GCom.GetUserID, True)                        'ログイン名
                                        .OutputCsvData(Machine, True)                               '端末名
                                        .OutputCsvData(sTORI(GetIndex("TORIS_CODE_T"), 3), True)    '取引先主コード
                                        .OutputCsvData(sTORI(GetIndex("TORIF_CODE_T"), 3), True)    '取引先副コード
                                        .OutputCsvData(sTORI(GetIndex("ITAKU_CODE_T"), 3), True)    '委託者コード
                                        .OutputCsvData(sTORI(GetIndex("ITAKU_NNAME_T"), 3), True)   '委託者名
                                        .OutputCsvData(sTORI(Index, 0), True)                       '日本語項目名
                                        .OutputCsvData(sTORI(Index, 1), True)                       'データベース項目名
                                        .OutputCsvData(SetData(0), True)                            '変更前
                                        .OutputCsvData(SetData(1), True, True)                      '変更後
                                    End With
                                    '更新項目数を加算
                                    updCnt += 1
                                End If

                            End If
                    End Select
                End If
            Next Index
            Prn_KFSP017.CloseCsv()
            Return True
        Catch ex As Exception
            MainLOG.Write("変更有無チェック", "失敗", ex.ToString)
            Return False
        Finally

        End Try
    End Function

    '
    ' 機能　　 : 取引先マスタの存在確認
    '
    ' 戻り値　 : 存在 = 1
    ' 　　　　   不在 = 0
    ' 　　　　   失敗 = -1
    '
    ' 引き数　 : ARG1 - なし
    '
    ' 備考　　 : 自振取引先マスタ専用関数
    '
    Private Function CheckExistTorimast(ByVal TorisCode As String, ByVal TorifCode As String) As Integer
        Dim SQL As New StringBuilder(128)
        Dim oraReader As OracleDataReader = Nothing
        Try
            SQL.AppendLine("SELECT * FROM S_TORIMAST_VIEW")
            SQL.AppendLine(" WHERE FSYORI_KBN_T = '3'")
            SQL.AppendLine(" AND TORIS_CODE_T = " & SQ(TorisCode))
            SQL.AppendLine(" AND TORIF_CODE_T = " & SQ(TorifCode))

            Return CType(IIf(GCom.SetDynaset(SQL.ToString, oraReader), 1, 0), Integer)
        Catch ex As Exception
            MainLOG.Write("取引先存在チェック", "失敗", ex.ToString)

            Return -1
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
            End If
            oraReader = Nothing
        End Try
    End Function

    Private Function CheckRecordModule() As Integer
        Dim SQL As New StringBuilder(128)
        Dim oraReader As OracleDataReader = Nothing
        Try
            SQL.AppendLine("SELECT * FROM S_TORIMAST_VIEW")
            SQL.AppendLine(" WHERE FSYORI_KBN_T = " & SetColumnData(GetIndex("FSYORI_KBN_T")))
            SQL.AppendLine(" AND TORIS_CODE_T = " & SetColumnData(GetIndex("TORIS_CODE_T")))
            SQL.AppendLine(" AND TORIF_CODE_T = " & SetColumnData(GetIndex("TORIF_CODE_T")))

            Return CType(IIf(GCom.SetDynaset(SQL.ToString, oraReader), 1, 0), Integer)
        Catch ex As Exception
            MainLOG.Write("取引先存在チェック", "失敗", ex.ToString)

            Return -1
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
            End If
            oraReader = Nothing
        End Try
    End Function

    '
    ' 機能　　 : 決済未使用設定
    '
    ' 戻り値　 :
    '
    ' 引き数　 : ARG1 - なし
    '
    ' 備考　　 : 自振取引先マスタ専用関数
    '
    Private Function KESSAI_Disused() As Boolean
        KESSAI_Disused = False
        Try
            If KESSAI = "0" Then
                '決済対象外設定
                GCom.SetComboBox(KESSAI_KBN_T, "KFSMAST010_決済区分.TXT", 99)
                '2012/02/15 saitou 標準修正 MODIFY ---------------------------------------->>>>
                GCom.SetComboBox(Me.KESSAI_PATN_T, "KFSMAST010_資金確保方法.TXT", 2)
                'GCom.SetComboBox(KESSAI_KBN_T, "KFSMAST010_資金確保方法.TXT", 2)
                '2012/02/15 saitou 標準修正 MODIFY ----------------------------------------<<<<
                '取りまとめ店 = 取り扱い支店
                TORIMATOME_SIT_T.Text = TSIT_NO_T.Text
                '2009/12/09 決済金融機関・支店追加
                TUKEKIN_NO_T.Text = ""
                '2012/02/15 saitou 標準修正 MODIFY ---------------------------------------->>>>
                Me.TUKEKIN_NO_TL.Text = String.Empty
                'TUKEKIN_NO_L.Text = ""
                '2012/02/15 saitou 標準修正 MODIFY ----------------------------------------<<<<
                TUKESIT_NO_T.Text = ""
                '2012/02/15 saitou 標準修正 MODIFY ---------------------------------------->>>>
                Me.TUKESIT_NO_TL.Text = String.Empty
                'TUKESIT_NO_L.Text = ""
                '2012/02/15 saitou 標準修正 MODIFY ----------------------------------------<<<<
                '=================================
                GB31.Enabled = False
                GB32.Enabled = False
                GB33.Enabled = False
            End If
        Catch ex As Exception
            MainLOG.Write("取引先存在チェック", "失敗", ex.ToString)
            Return False
        End Try
        KESSAI_Disused = True
    End Function

#End Region

#Region " イベント"

    Private Sub TORIF_CODE_T_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TORIF_CODE_T.LostFocus
        Me.BAITAI_CODE_T.Focus()
    End Sub

    'タブ・クリック・イベント処理
    Private Sub SSTab_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SSTab.Click
        Try
            Select Case SSTab.SelectedIndex
                Case 0
                    Me.BAITAI_CODE_T.Focus()
                Case 1
                    'Me.FURI_KYU_CODE_T.Focus()
                    Me.CmdDaySet.Focus()
                Case 2
                    Me.KESSAI_KBN_T.Focus()
                Case 3
                    Me.ENC_KBN_T.Focus()
                Case 4
                    Me.TOKKIJIKOU1_T.Focus()
            End Select
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("タブクリック時フォーカス移動", "失敗", ex.ToString)
        End Try
    End Sub

    '数値だけを抜き出して金額用の表示編集を行う
    Private Sub Money_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles KOTEI_TESUU1_T.Validating, KOTEI_TESUU2_T.Validating, SOURYO_T.Validating
        Try
            With CType(sender, TextBox)
                .Text = String.Format("{0:#,##0}", GCom.NzDec(.Text, 1))
            End With
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("金額表示変数", "失敗", ex.ToString)
        End Try
    End Sub

    '許可したい文字だけでフィルタをかける
    Private Sub AnyChar_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles DENWA_T.Validating
        Try
            Dim OBJ As TextBox = CType(sender, TextBox)
            Select Case OBJ.Name.ToUpper
                Case "YUUBIN_T".ToUpper
                    CType(sender, TextBox).Text = GCom.NzAny("0123456789-", CType(sender, TextBox).Text)
                Case "DENWA_T".ToUpper, "FAX_T".ToUpper
                    CType(sender, TextBox).Text = GCom.NzAny("0123456789()-", CType(sender, TextBox).Text)
            End Select
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("文字フィルタ", "失敗", ex.ToString)
        End Try

    End Sub
    '数値評価してZERO埋めする
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles TORIS_CODE_T.Validating, TORIF_CODE_T.Validating, ITAKU_KANRI_CODE_T.Validating, ITAKU_CODE_T.Validating, _
                    TKIN_NO_T.Validating, TSIT_NO_T.Validating, KOUZA_T.Validating, _
                    KANREN_KIGYO_CODE_T.Validating, KEIYAKU_DATE_T.Validating, KEIYAKU_DATE_T1.Validating, KEIYAKU_DATE_T2.Validating, _
                    KAISI_DATE_T.Validating, KAISI_DATE_T1.Validating, KAISI_DATE_T2.Validating, SYURYOU_DATE_T.Validating, _
                    SYURYOU_DATE_T1.Validating, SYURYOU_DATE_T2.Validating, TORIMATOME_SIT_T.Validating, HONBU_KOUZA_T.Validating, _
                    TUKEKIN_NO_T.Validating, TUKESIT_NO_T.Validating, TESUUTYO_SIT_T.Validating, _
                    TESUUTYO_KOUZA_T.Validating, TESUUMAT_NO_T.Validating, TUKEKIN_NO_T.Validating, TUKEKOUZA_T.Validating, _
                    FURI_CODE_T.Validating, KIGYO_CODE_T.Validating, _
                    HENKYAKU_SIT_NO_T.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("ゼロパディング", "失敗", ex.ToString)
        End Try

    End Sub

    '数値評価(Zero埋めフォーマット有り：金融機関登録エリア：金融機関の場合)
    Private Sub Bank_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TKIN_NO_T.Validating, TUKEKIN_NO_T.Validating

        Try
            Dim OBJ As TextBox = CType(sender, TextBox)
            Dim Temp As String = GCom.NzDec(OBJ.Text, "")
            If Temp.Length = 0 Then
                OBJ.Text = ""
                GetLabel(OBJ.Name).Text = ""
            Else
                Call GCom.NzNumberString(OBJ, True)
                GetLabel(OBJ.Name).Text = GCom.GetBKBRName(OBJ.Text, "", 30)
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("金融機関ゼロパディングフォーマット", "失敗", ex.ToString)
        End Try

    End Sub

    '数値評価(Zero埋めフォーマット有り：金融機関登録エリア：支店の場合)
    Private Sub Branch_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TSIT_NO_T.Validating, TUKESIT_NO_T.Validating, TESUUTYO_SIT_T.Validating, TORIMATOME_SIT_T.Validating, HENKYAKU_SIT_NO_T.Validating

        Dim OBJ As TextBox = CType(sender, TextBox)
        Dim Temp As String = GCom.NzDec(OBJ.Text, "")
        If Temp.Length = 0 Then
            OBJ.Text = ""
            GetLabel(OBJ.Name).Text = ""
        Else
            Call GCom.NzNumberString(OBJ, True)
            Dim BKCode As String = ""
            Dim BRCode As String = OBJ.Text
            Select Case OBJ.Name.ToUpper
                Case "TSIT_NO_T".ToUpper
                    BKCode = GCom.NzDec(Me.TKIN_NO_T.Text, "")
                Case "TUKESIT_NO_T".ToUpper
                    BKCode = GCom.NzDec(Me.TUKEKIN_NO_T.Text, "")
                Case Else
                    If BRCode.Trim.Length > 0 Then
                        BKCode = Jikinko
                    End If
            End Select

            GetLabel(OBJ.Name).Text = GCom.GetBKBRName(BKCode, BRCode, 30)
        End If
    End Sub

    Private Sub NzCheckString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles ITAKU_KNAME_T.Validating, ITAKU_KNAME_T.Validating, TUKEMEIGI_KNAME_T.Validating, KTEKIYOU_T.Validating, YUUBIN_KNAME_T.Validating, BIKOU1_T.Validating, BIKOU2_T.Validating

        Call GCom.NzCheckString(CType(sender, TextBox))
        With CType(sender, TextBox)
            Select Case .Name
                'Case "FILE_NAME_T"
                '    .Text = .Text.ToUpper
                Case "ITAKU_KNAME_T", "TUKEMEIGI_KNAME_T", "KTEKIYOU_T", "YUUBIN_KNAME_T", "BIKOU1_T", "BIKOU2_T"
                    Dim BRet As Boolean = GCom.CheckZenginChar(CType(sender, TextBox))
            End Select
        End With
    End Sub

    '全角混入領域バイト数調整用
    Private Sub GetLimitString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles ITAKU_NNAME_T.Validating, ITAKU_NJYU_T.Validating, YUUBIN_NNAME_T.Validating, NTEKIYOU_T.Validating, FILE_NAME_T.Validating, _
                    HANSOU_ROOT1_T.Validating, HANSOU_ROOT2_T.Validating, HANSOU_ROOT3_T.Validating, _
                    TOKKIJIKOU1_T.Validating, TOKKIJIKOU2_T.Validating, TOKKIJIKOU3_T.Validating, TOKKIJIKOU4_T.Validating

        Dim Index As Integer
        With CType(sender, TextBox)
            Select Case .Name
                Case "ITAKU_NNAME_T" : Index = GetIndex("ITAKU_NNAME_T")
                Case "YUUBIN_NNAME_T" : Index = GetIndex("YUUBIN_NNAME_T")
                Case "ITAKU_NJYU_T" : Index = GetIndex("ITAKU_NJYU_T")
                Case "NTEKIYOU_T" : Index = GetIndex("NTEKIYOU_T")
                Case "FILE_NAME_T" : Index = GetIndex("FILE_NAME_T")
                Case "HANSOU_ROOT1_T" : Index = GetIndex("HANSOU_ROOT1_T")
                Case "HANSOU_ROOT2_T" : Index = GetIndex("HANSOU_ROOT2_T")
                Case "HANSOU_ROOT3_T" : Index = GetIndex("HANSOU_ROOT3_T")
                Case "TOKKIJIKOU1_T" : Index = GetIndex("TOKKIJIKOU1_T")
                Case "TOKKIJIKOU2_T" : Index = GetIndex("TOKKIJIKOU2_T")
                Case "TOKKIJIKOU3_T" : Index = GetIndex("TOKKIJIKOU3_T")
                Case "TOKKIJIKOU4_T" : Index = GetIndex("TOKKIJIKOU4_T")
            End Select
            .Text = GCom.GetLimitString(.Text, nTORI(Index, 0))
        End With
    End Sub

    '日付／月の文字色変更
    Private Sub DATE1_T_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
          Handles DATE1_T.GotFocus, DATE2_T.GotFocus, DATE3_T.GotFocus, DATE4_T.GotFocus, DATE5_T.GotFocus, _
            DATE6_T.GotFocus, DATE7_T.GotFocus, DATE8_T.GotFocus, DATE9_T.GotFocus, DATE10_T.GotFocus, _
            DATE11_T.GotFocus, DATE12_T.GotFocus, DATE13_T.GotFocus, DATE14_T.GotFocus, DATE15_T.GotFocus, _
            DATE16_T.GotFocus, DATE17_T.GotFocus, DATE18_T.GotFocus, DATE19_T.GotFocus, DATE20_T.GotFocus, _
            DATE21_T.GotFocus, DATE22_T.GotFocus, DATE23_T.GotFocus, DATE24_T.GotFocus, DATE25_T.GotFocus, _
            DATE26_T.GotFocus, DATE27_T.GotFocus, DATE28_T.GotFocus, DATE29_T.GotFocus, DATE30_T.GotFocus, _
            DATE31_T.GotFocus, TUKI1_T.GotFocus, TUKI2_T.GotFocus, TUKI3_T.GotFocus, TUKI4_T.GotFocus, _
            TUKI5_T.GotFocus, TUKI6_T.GotFocus, TUKI7_T.GotFocus, TUKI8_T.GotFocus, TUKI9_T.GotFocus, _
            TUKI10_T.GotFocus, TUKI11_T.GotFocus, TUKI12_T.GotFocus
        With CType(sender, CheckBox)
            .ForeColor = System.Drawing.Color.White
            .BackColor = System.Drawing.Color.Black
        End With
    End Sub

    '日付／月の文字色復帰
    Private Sub DATE1_T_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles DATE1_T.LostFocus, DATE2_T.LostFocus, DATE3_T.LostFocus, DATE4_T.LostFocus, DATE5_T.LostFocus, _
            DATE6_T.LostFocus, DATE7_T.LostFocus, DATE8_T.LostFocus, DATE9_T.LostFocus, DATE10_T.LostFocus, _
            DATE11_T.LostFocus, DATE12_T.LostFocus, DATE13_T.LostFocus, DATE14_T.LostFocus, DATE15_T.LostFocus, _
            DATE16_T.LostFocus, DATE17_T.LostFocus, DATE18_T.LostFocus, DATE19_T.LostFocus, DATE20_T.LostFocus, _
            DATE21_T.LostFocus, DATE22_T.LostFocus, DATE23_T.LostFocus, DATE24_T.LostFocus, DATE25_T.LostFocus, _
            DATE26_T.LostFocus, DATE27_T.LostFocus, DATE28_T.LostFocus, DATE29_T.LostFocus, DATE30_T.LostFocus, _
            DATE31_T.LostFocus, TUKI1_T.LostFocus, TUKI2_T.LostFocus, TUKI3_T.LostFocus, TUKI4_T.LostFocus, _
            TUKI5_T.LostFocus, TUKI6_T.LostFocus, TUKI7_T.LostFocus, TUKI8_T.LostFocus, TUKI9_T.LostFocus, _
            TUKI10_T.LostFocus, TUKI11_T.LostFocus, TUKI12_T.LostFocus
        With CType(sender, CheckBox)
            .ForeColor = System.Drawing.SystemColors.ControlText
            .BackColor = System.Drawing.SystemColors.Control
        End With
    End Sub

    '摘要区分用
    Private Sub TEKIYOU_KBN_T_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs)
        Select Case GCom.GetComboBox(Me.TEKIYOU_KBN_T)
            Case 0
                'カナ摘要の時
                Me.KTEKIYOU_T.MaxLength = 13
                Me.NTEKIYOU_T.Text = ""
                Me.NTEKIYOU_T.MaxLength = 0
            Case 1
                '漢字摘要の時
                Me.KTEKIYOU_T.Text = ""
                Me.KTEKIYOU_T.MaxLength = 0
                Me.NTEKIYOU_T.MaxLength = 12
            Case Else
                '可変摘要の時
                Me.KTEKIYOU_T.Text = ""
                Me.NTEKIYOU_T.Text = ""
                Me.KTEKIYOU_T.MaxLength = 0
                Me.NTEKIYOU_T.MaxLength = 0
        End Select
    End Sub

    'タブ間のフォーカス移動
    ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№171)) -------------------- START
    Private Sub TabIndexSetFocus_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) _
        Handles YUUBIN_NNAME_T.KeyPress, PRTNUM_T.KeyPress, TESUU_TABLE_ID_T.KeyPress, ENC_KEY2_T.KeyPress, _
                MAE_SYORI_T.KeyPress, TOKKIJIKOU4_T.KeyPress
        'Private Sub TabIndexSetFocus_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) _
        ' Handles YUUBIN_NNAME_T.KeyPress, TESUU_TABLE_ID_T.KeyPress, ENC_KEY2_T.KeyPress, _
        '         MAE_SYORI_T.KeyPress, TOKKIJIKOU4_T.KeyPress
        ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№171)) -------------------- END

        Try
            Select Case Microsoft.VisualBasic.Asc(e.KeyChar)
                Case Keys.Tab, Keys.Return
                    With Me.SSTab
                        Select Case CType(sender, Control).Name
                            Case "YUUBIN_NNAME_T"
                                .SelectedIndex = 1
                                Me.CmdDaySet.Focus()
                                ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№171)) -------------------- START
                            Case "PRTNUM_T"
                                'Case "FKEKKA_TBL_T"
                                ' 2017/11/10 タスク）西野 CHG (標準版不具合対応(№171)) -------------------- END
                                .SelectedIndex = 2
                                Me.KESSAI_KBN_T.Focus()
                            Case "TESUU_TABLE_ID_T"
                                .SelectedIndex = 3
                                Me.ENC_KBN_T.Focus()
                            Case "MAE_SYORI_T"
                                .SelectedIndex = 4
                                Me.TOKKIJIKOU1_T.Focus()
                            Case "TOKKIJIKOU4_T"
                                .SelectedIndex = 0
                                Me.BAITAI_CODE_T.Focus()
                        End Select
                    End With
            End Select
        Catch ex As Exception

        End Try
    End Sub

    '振込手数料基準ＩＤ
    Private Sub TESUU_TABLE_ID_T_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TESUU_TABLE_ID_T.SelectedIndexChanged
        Dim FL As StreamReader = Nothing
        Try
            Dim Name() As String = {"TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                                    "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                                    "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"}

            Dim Index As Integer = GetIndex("TESUU_TABLE_ID_T")
            Dim Ret As Integer = GCom.GetComboBox(CType(oTORI(Index), ComboBox))

            If Ret < 0 Then
                For Cnt As Integer = 0 To 8 Step 1
                    Index = GetIndex(Name(Cnt))
                    CType(oTORI(Index), Label).Text = ""
                    sTORI(Index, 4) = ""
                Next
            Else
                '2013/11/27 saitou 標準版 消費税対応 UPD -------------------------------------------------->>>>
                '振込手数料取得方法変更
                Dim SQL As New StringBuilder
                Dim oraReader As OracleDataReader = Nothing

                Try
                    With SQL
                        .Append("select * from TESUUMAST")
                        .Append(" where FSYORI_KBN_C = '3'")
                        '2017/04/28 saitou RSV2 UPD 標準機能追加(種別によって表示する手数料変更) ---------------------------------------- START
                        Select Case GCom.GetComboBox(Me.SYUBETU_T)
                            Case 11, 12
                                .Append(" and SYUBETU_C = '12'")
                            Case Else
                                .Append(" and SYUBETU_C = '10'")
                        End Select
                        '.Append(" and SYUBETU_C = '10'")
                        '2017/04/28 saitou RSV2 UPD ------------------------------------------------------------------------------------- END
                        ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
                        '.Append(" and TESUU_TABLE_ID_C = " & SQ(Ret.ToString))
                        .Append(" and TESUU_TABLE_ID_C = " & SQ(Format(Ret, "00")))
                        ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END
                        .Append(" and TAX_ID_C = " & SQ(TAX.ZEIRITSU_ID))
                    End With

                    If GCom.SetDynaset(SQL.ToString, oraReader) AndAlso oraReader.Read Then
                        For Cnt As Integer = 0 To 8 Step 1
                            Index = GetIndex(Name(Cnt))
                            Dim onData As Decimal

                            Select Case Cnt
                                Case 0 : onData = GCom.NzDec(oraReader.Item("TESUU_A1_C"), 0)
                                Case 1 : onData = GCom.NzDec(oraReader.Item("TESUU_A2_C"), 0)
                                Case 2 : onData = GCom.NzDec(oraReader.Item("TESUU_A3_C"), 0)
                                Case 3 : onData = GCom.NzDec(oraReader.Item("TESUU_B1_C"), 0)
                                Case 4 : onData = GCom.NzDec(oraReader.Item("TESUU_B2_C"), 0)
                                Case 5 : onData = GCom.NzDec(oraReader.Item("TESUU_B3_C"), 0)
                                Case 6 : onData = GCom.NzDec(oraReader.Item("TESUU_C1_C"), 0)
                                Case 7 : onData = GCom.NzDec(oraReader.Item("TESUU_C2_C"), 0)
                                Case 8 : onData = GCom.NzDec(oraReader.Item("TESUU_C3_C"), 0)
                            End Select

                            Dim Temp As String = onData.ToString("#,##0")
                            CType(oTORI(Index), Label).Text = Temp
                            sTORI(Index, 4) = Temp.Replace(","c, "")
                        Next

                    End If

                Catch ex As Exception
                    MainLOG.Write("振込手数料マスタ参照", "失敗", ex.ToString)
                Finally
                    If Not oraReader Is Nothing Then
                        oraReader.Close()
                    End If
                    oraReader = Nothing
                End Try

                'Dim FileName As String = ""
                'FileName &= GCom.SET_PATH(GCom.GetTXTFolder)
                'FileName &= "KFSMAST010_振込手数料基準ID.TXT"

                'FL = New StreamReader(FileName, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

                'Dim LineData As String = FL.ReadLine
                'Do While Not LineData Is Nothing

                '    '決められた項目数でないとダメ
                '    Dim Data() As String = LineData.Split(","c)
                '    If Data.Length = 11 Then

                '        '該当データが見つかれば
                '        If GCom.NzInt(Data(0), 0) = Ret Then

                '            For Cnt As Integer = 0 To 8 Step 1

                '                Index = GetIndex(Name(Cnt))
                '                Dim onData As Decimal = GCom.NzDec(Data(Cnt + 2), 0)
                '                Dim Temp As String = onData.ToString("#,##0")
                '                CType(oTORI(Index), Label).Text = Temp
                '                sTORI(Index, 4) = Temp.Replace(","c, "")
                '            Next
                '        End If
                '    End If
                '    LineData = FL.ReadLine
                'Loop
                'FL.Close()
                '2013/11/27 saitou 標準版 消費税対応 UPD --------------------------------------------------<<<<
            End If
        Catch ex As Exception

        Finally
            If Not FL Is Nothing Then
                FL.Close()
            End If
        End Try
    End Sub

    'カナコンボボックス設定変更時再読み込み
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            '取引先コードがReadOnlyの場合は処理終了
            If TORIS_CODE_T.ReadOnly Then
                Exit Sub
            End If
            '取引先コンボボックス設定
            If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, TORIS_CODE_T, TORIF_CODE_T, "3") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Catch ex As Exception

        End Try
    End Sub

    '取引先コード設定
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        Try
            '取引先コードがReadOnlyの場合は処理終了
            If TORIS_CODE_T.ReadOnly Then
                Exit Sub
            End If
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, TORIS_CODE_T, TORIF_CODE_T)
            End If
        Catch ex As Exception

        End Try
    End Sub

    '適用区分入力制限
    Private Sub TEKIYOU_KBN_T_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TEKIYOU_KBN_T.SelectedIndexChanged
        Dim Ret As Integer = GCom.GetComboBox(CType(oTORI(GetIndex("TEKIYOU_KBN_T")), ComboBox))
        Select Case Ret
            Case 0 'カナ適用
                KTEKIYOU_T.Enabled = True
                NTEKIYOU_T.Enabled = False
                NTEKIYOU_T.Text = ""
            Case 1  '漢字適用
                KTEKIYOU_T.Enabled = False
                KTEKIYOU_T.Text = ""
                NTEKIYOU_T.Enabled = True
            Case Else '可変適用
                KTEKIYOU_T.Enabled = False
                KTEKIYOU_T.Text = ""
                NTEKIYOU_T.Enabled = False
                NTEKIYOU_T.Text = ""
        End Select
    End Sub

    '種別変更時の給与摘要区分コンボボックス再読込
    Private Sub SYUBETU_T_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SYUBETU_T.SelectedIndexChanged

        Try
            Dim CTL_KYUUYO_KBN As Control = KYUUYO_KBN_T()
            Dim CTL_TESUU_TABLE_ID As Control = TESUU_TABLE_ID_T
            Dim CMB_KYUUYO_KBN As ComboBox = CType(CTL_KYUUYO_KBN, ComboBox)
            Dim CMB_TESUU_TABLE_ID As ComboBox = CType(CTL_TESUU_TABLE_ID, ComboBox)

            Dim Ret As Integer = 0

            KYUUYO_KBN_T.Items.Clear()
            KYUUYO_KBN_T.Text = ""
            Select Case GCom.GetComboBox(CType(oTORI(GetIndex("SYUBETU_T")), ComboBox)).ToString
                Case "21"
                    Ret = GCom.SetComboBox(KYUUYO_KBN_T, "KFSMAST011_給与摘要区分(総振).TXT", True)
                Case Else
                    Ret = GCom.SetComboBox(KYUUYO_KBN_T, "KFSMAST011_給与摘要区分(給与).TXT", True)
            End Select

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(給与摘要区分)再構成", "失敗", ex.ToString)
        End Try

        '2017/04/28 saitou RSV2 ADD 標準機能追加(種別によって表示する手数料変更) ---------------------------------------- START
        Try
            Dim Name() As String = {"TESUU_A1_T", "TESUU_A2_T", "TESUU_A3_T", _
                                    "TESUU_B1_T", "TESUU_B2_T", "TESUU_B3_T", _
                                    "TESUU_C1_T", "TESUU_C2_T", "TESUU_C3_T"}

            Dim Index As Integer = GetIndex("TESUU_TABLE_ID_T")
            Dim Ret As Integer = GCom.GetComboBox(CType(oTORI(Index), ComboBox))

            If Ret < 0 Then
                For Cnt As Integer = 0 To 8 Step 1
                    Index = GetIndex(Name(Cnt))
                    CType(oTORI(Index), Label).Text = ""
                    sTORI(Index, 4) = ""
                Next
            Else
                Dim SQL As New StringBuilder
                Dim OraReader As OracleDataReader = Nothing

                Try
                    With SQL
                        .Append("select * from TESUUMAST")
                        .Append(" where FSYORI_KBN_C = '3'")
                        Select Case GCom.GetComboBox(Me.SYUBETU_T)
                            Case 11, 12
                                .Append(" and SYUBETU_C = '12'")
                            Case Else
                                .Append(" and SYUBETU_C = '10'")
                        End Select
                        .Append(" and TESUU_TABLE_ID_C = " & SQ(Format(Ret, "00")))
                        .Append(" and TAX_ID_C = " & SQ(TAX.ZEIRITSU_ID))
                    End With

                    If GCom.SetDynaset(SQL.ToString, OraReader) AndAlso OraReader.Read Then
                        For Cnt As Integer = 0 To 8 Step 1
                            Index = GetIndex(Name(Cnt))
                            Dim onData As Decimal

                            Select Case Cnt
                                Case 0 : onData = GCom.NzDec(OraReader.Item("TESUU_A1_C"), 0)
                                Case 1 : onData = GCom.NzDec(OraReader.Item("TESUU_A2_C"), 0)
                                Case 2 : onData = GCom.NzDec(OraReader.Item("TESUU_A3_C"), 0)
                                Case 3 : onData = GCom.NzDec(OraReader.Item("TESUU_B1_C"), 0)
                                Case 4 : onData = GCom.NzDec(OraReader.Item("TESUU_B2_C"), 0)
                                Case 5 : onData = GCom.NzDec(OraReader.Item("TESUU_B3_C"), 0)
                                Case 6 : onData = GCom.NzDec(OraReader.Item("TESUU_C1_C"), 0)
                                Case 7 : onData = GCom.NzDec(OraReader.Item("TESUU_C2_C"), 0)
                                Case 8 : onData = GCom.NzDec(OraReader.Item("TESUU_C3_C"), 0)
                            End Select

                            Dim Temp As String = onData.ToString("#,##0")
                            CType(oTORI(Index), Label).Text = Temp
                            sTORI(Index, 4) = Temp.Replace(","c, "")
                        Next

                    End If

                Catch ex As Exception
                    MainLOG.Write("振込手数料マスタ参照", "失敗", ex.ToString)
                Finally
                    If Not OraReader Is Nothing Then
                        OraReader.Close()
                    End If
                    OraReader = Nothing
                End Try
            End If
        Catch ex As Exception

        End Try
        '2017/04/28 saitou RSV2 ADD ------------------------------------------------------------------------------------- END
    End Sub

#End Region

End Class
' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END
