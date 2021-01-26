Imports Microsoft.VisualBasic.FileIO
Imports System.Text
Imports System.Collections.Generic
Imports CASTCommon
Imports System.Windows.Forms
Imports System.Drawing

Public Class FrmExtendPrint

#Region "変数宣言"

    Private GCom As MenteCommon.clsCommon
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private LOG As CASTCommon.BatchLOG

#Region "エラーログ"
    Private Const ERRLOG_001 As String = "ファイル '{0}' が見つかりませんでした。"
    Private Const ERRLOG_002 As String = "{0}定義エラー：[{1}]セクションの定義に誤りがあります。"
    Private Const ERRLOG_003 As String = "{0}定義エラー：[{1}]セクションが定義されていません。"
    Private Const ERRLOG_004 As String = "{0}定義エラー：[{1}]セクションの{2}番目の部品名({3})は存在しません。"
    Private Const ERRLOG_005 As String = "{0}定義エラー：[{1}]セクションの{2}番目のプロパティ({3})は存在しません。"
    Private Const ERRLOG_006 As String = "{0}定義エラー：[{1}]セクションの{2}番目のプロパティ({3})の値に誤りがあります。"
    Private Const ERRLOG_007 As String = "{0}定義エラー：[{1}]セクションのプロパティ({2})は重複しないように設定してください。"
    Private Const ERRLOG_008 As String = "{0}定義エラー：[{1}]セクションの{2}番目の部品名({3})の必須プロパティ({4})が未定義です。"
    Private Const ERRLOG_009 As String = "{0}定義エラー：[{1}]セクションの{2}番目の部品固有情報に誤りがあります。"
    Private Const ERRLOG_010 As String = "{0}コンボボックスの設定に失敗しました。"
    Private Const ERRLOG_011 As String = "営業日の取得に失敗しました。"
    Private Const ERRLOG_012 As String = "内部エラーが発生しました。"
#End Region

#Region "部品情報"
    Private pCOMBO_CODE(,) As String = {{"PNAME", "COMBO_CODE"}, _
                                        {"DB_KIND", ""}, _
                                        {"LABEL", ""}, _
                                        {"LABEL_STYLE", "1"}, _
                                        {"LABEL2", ""}, _
                                        {"LABEL2_STYLE", "1"}, _
                                        {"WHERE", ""}, _
                                        {"ALL9", "YES"}, _
                                        {"TAB_INDEX", "-1"}, _
                                        {"FOCUS", "0"}, _
                                        {"SQL_NO", ""}}

    Private pCOMBO_SQL(,) As String = {{"PNAME", "COMBO_SQL"}, _
                                       {"COMBO_DEF", ""}, _
                                       {"LABEL", ""}, _
                                       {"LABEL_STYLE", "1"}, _
                                       {"LABEL2", ""}, _
                                       {"LABEL2_STYLE", "1"}, _
                                       {"LABEL3", ""}, _
                                       {"LABEL3_STYLE", "1"}, _
                                       {"TAB_INDEX", "-1"}, _
                                       {"FOCUS", "0"}, _
                                       {"SQL_NO", ""}, _
                                       {"IN_SIZE", "10"}}

    Private pCODE(,) As String = {{"PNAME", "CODE"}, _
                                  {"LABEL", ""}, _
                                  {"LABEL_STYLE", "1"}, _
                                  {"CODE_TYPE", "2"}, _
                                  {"TAB_INDEX", "-1"}, _
                                  {"FOCUS", "0"}, _
                                  {"SQL_NO", ""}}

    Private pRANGE_CODE(,) As String = {{"PNAME", "RANGE_CODE"}, _
                                        {"LABEL", ""}, _
                                        {"LABEL_STYLE", "1"}, _
                                        {"IN_SIZE", "10"}, _
                                        {"TAB_INDEX", "-1"}, _
                                        {"FOCUS", "0"}, _
                                        {"SQL_NO", ""}}

    Private pDATE(,) As String = {{"PNAME", "DATE"}, _
                                  {"LABEL", ""}, _
                                  {"LABEL_STYLE", "1"}, _
                                  {"DATE_TYPE", "1"}, _
                                  {"INIT_DATE", "0"}, _
                                  {"BUSINESS_DAY", ""}, _
                                  {"TAB_INDEX", "-1"}, _
                                  {"FOCUS", "0"}, _
                                  {"SQL_NO", ""}}

    Private pRANGE_DATE(,) As String = {{"PNAME", "RANGE_DATE"}, _
                                        {"LABEL", ""}, _
                                        {"LABEL_STYLE", "1"}, _
                                        {"DATE_TYPE", "1"}, _
                                        {"INIT_DATE1", "0"}, _
                                        {"INIT_DATE2", "0"}, _
                                        {"BUSINESS_DAY1", ""}, _
                                        {"BUSINESS_DAY2", ""}, _
                                        {"TAB_INDEX", "-1"}, _
                                        {"FOCUS", "0"}, _
                                        {"SQL_NO", ""}}

    Private pCOMBO(,) As String = {{"PNAME", "COMBO"}, _
                                   {"LABEL", ""}, _
                                   {"LABEL_STYLE", "1"}, _
                                   {"COMBO_DEF", ""}, _
                                   {"SELECT", "0"}, _
                                   {"TAB_INDEX", "-1"}, _
                                   {"FOCUS", "0"}, _
                                   {"SQL_NO", ""}}

    Private pRADIO(,) As String = {{"PNAME", "RADIO"}, _
                                   {"LABEL", ""}, _
                                   {"LABEL_STYLE", "1"}, _
                                   {"RADIO_DEF", ""}, _
                                   {"RADIO_STYLE", "1"}, _
                                   {"GRID", ""}, _
                                   {"SELECT", "1"}, _
                                   {"TAB_INDEX", "-1"}, _
                                   {"FOCUS", "0"}, _
                                   {"SQL_NO", ""}}

    Private pCHECK(,) As String = {{"PNAME", "CHECK"}, _
                                   {"LABEL", ""}, _
                                   {"LABEL_STYLE", "1"}, _
                                   {"CHECK_DEF", ""}, _
                                   {"CHECK_STYLE", "1"}, _
                                   {"GRID", ""}, _
                                   {"OMIT", "YES"}, _
                                   {"SELECT", "0"}, _
                                   {"TAB_INDEX", "-1"}, _
                                   {"FOCUS", "0"}, _
                                   {"SQL_NO", ""}}

    Private pTEXT(,) As String = {{"PNAME", "TEXT"}, _
                                  {"LABEL", ""}, _
                                  {"LABEL_STYLE", "1"}, _
                                  {"IN_TYPE", "1"}, _
                                  {"IN_SIZE", ""}, _
                                  {"IN_CORRECT", "1"}, _
                                  {"INIT_TEXT", ""}, _
                                  {"TAB_INDEX", "-1"}, _
                                  {"FOCUS", "0"}, _
                                  {"SQL_NO", ""}}

    Private pLABEL(,) As String = {{"PNAME", "LABEL"}, _
                                   {"LABEL", ""}, _
                                   {"LABEL_STYLE", "1"}}

    Private CmbItemList As String() = {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", _
                                           "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", _
                                           "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", _
                                           "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", _
                                           "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", _
                                           "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", _
                                           "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", _
                                           "U", "V", "W", "X", "Y", "Z", _
                                           "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", _
                                           "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", _
                                           "u", "v", "w", "x", "y", "z", _
                                           "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"}

    Private TabIndexList As New List(Of Control)    'タブ移動が可能なコントロールを格納

#End Region

#Region "構造体"

    '部品情報構造体
    Private Structure PartsInformation
        Dim PartsName As String                        '部品名
        Dim PartsInputList As List(Of Control)         'フォームに配置するコントロール(ラベル以外)
        Dim PartsLabelList As List(Of Label)           'フォームに配置するラベル
        Dim PartsSqlno As String()                     'SQL置換番号
        Dim PartsFocus As String                       'フォーカス設定情報
        Dim PartsFlag As Boolean                       'フラグ
        Dim PartsOption As String                      '補足情報
        Dim PartsOptionList As List(Of String())       '補足情報
        '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
        Dim CodeErrFlg As Boolean                      ' 入力コードエラーフラグ
        '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
    End Structure
    Private PartsList As New List(Of PartsInformation)

    '印刷情報構造体
    Private Structure PrtInformation
        Dim PrtID As String                          '帳票ID
        Dim PrtName As String                        '帳票名
        Dim PrtTitleName As String                   '帳票タイトル名
        Dim PrtFileName As String                    '帳票印刷画面定義体名
        Dim PrtMsgTitle As String                    'メッセージダイアログタイトル
        Dim PrtDevNameArray As String()                   'プリンタ名配列
    End Structure
    Private PrtInf As PrtInformation

    '業務固有印刷情報構造体
    Private Structure ExternalInformation
        Dim PrtExFlag As Boolean                     '業務固有印刷判定フラグ
        Dim PrtDLL As String                         'DLL名
        Dim PrtClass As String                       'クラス名
        Dim PrtMethod As String                      'メソッド名
    End Structure
    Private ExInf As ExternalInformation

    '画面定義情報構造体
    Private Structure ReferencePoint
        Dim Point_X_Label As Integer                 'ラベル域に置く部品の開始位置（Ｘ座標）
        Dim Point_X_Input As Integer                 '入力域に置く部品の開始位置（Ｘ座標）
        Dim Point_Y As Integer                       'ラベル域・入力域共通の部品の開始位置（Ｙ座標）
        Dim Margin As Integer                        '部品の配置間隔
        Dim ButtonPos As Integer                     'ボタンの配置位置(1:左寄せ 2:センタリング 3:右寄せ[デフォルト])
    End Structure
    Private RefP As ReferencePoint
#End Region

#End Region

#Region "ロード/クローズ"
    Public Sub New()
        InitializeComponent()
    End Sub

    '------------------------------------------------------------
    ' 機能   ： コンストラクタ
    ' 引数   ： prtid - 帳票ID
    '           title - 帳票タイトル名
    ' 戻り値 ： なし
    '------------------------------------------------------------
    Public Sub New(ByVal prtid As String, ByVal title As String)
        Me.New()

        '帳票ID、帳票タイトル名を設定する
        PrtInf.PrtID = prtid
        PrtInf.PrtTitleName = title

        '帳票タイトル名の末尾から「印刷」または「再印刷」の
        '文字列を除いたものを帳票名として設定する
        If PrtInf.PrtTitleName.EndsWith("印刷") = True Then
            Dim length As Integer
            If PrtInf.PrtTitleName.EndsWith("再印刷") = True Then
                length = PrtInf.PrtTitleName.Length - 3
            Else
                length = PrtInf.PrtTitleName.Length - 2
            End If
            PrtInf.PrtName = PrtInf.PrtTitleName.Substring(0, length)

        Else
            PrtInf.PrtName = PrtInf.PrtTitleName
        End If

        'メッセージボックスのタイトル設定
        PrtInf.PrtMsgTitle = PrtInf.PrtTitleName & "画面(" & PrtInf.PrtID & ")"

        LOG = New CASTCommon.BatchLOG("FrmExtendPrint", PrtInf.PrtTitleName & "画面")
    End Sub

    '------------------------------------------------------------
    ' 機能   ： フォームを閉じる
    ' 引数   ： なし
    ' 戻り値 ： なし
    '------------------------------------------------------------
    Private Sub Close_Frm()

        TabIndexList.Clear()
        TabIndexList = Nothing
        PartsList.Clear()
        PartsList = Nothing

        Me.Close()
        Me.Dispose()

    End Sub

    '------------------------------------------------------------
    ' 機能   ： フォーム読込に失敗した場合、フォームを閉じる
    '           フォーム読込完了時（初回）に呼び出されるイベントハンドラ
    '------------------------------------------------------------
    Private Sub FrmExtendPrint_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        'フォームが非表示（最小化）になっている場合
        If Me.WindowState = FormWindowState.Minimized Then
            'フォームを閉じる
            Close_Frm()
        End If
    End Sub

    '------------------------------------------------------------
    ' 機能   ： フォームをロードする
    '           フォーム読込時（初回）に呼び出されるイベントハンドラ
    '------------------------------------------------------------
    Private Sub FrmExtendPrint_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter1("FrmExtendPrint.FrmExtendPrint_Load", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            'ログインIDを取得
            Dim Args As String() = Environment.GetCommandLineArgs
            GCom = New MenteCommon.clsCommon

            'ログインID、日付設定
            If Args.Length <= 1 Then
                GCom.GetUserID = ""
            Else
                GCom.GetUserID = Args(1).Trim
            End If
            GCom.GetSysDate = Date.Now

            '画面定義読込処理
            If Read_InitData() = False Then
                MessageBox.Show(P_MSG0002E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Me.WindowState = FormWindowState.Minimized
                Return
            End If

            '部品情報作成処理
            If Make_Parts() = False Then
                MessageBox.Show(P_MSG0002E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Me.WindowState = FormWindowState.Minimized
                Return
            End If

        Catch ex As Exception
            LOG.Write_Err("FrmExtendPrint.FrmExtendPrint_Load", ex)
            MessageBox.Show(P_MSG0009E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.WindowState = FormWindowState.Minimized
        Finally
            LOG.Write_Exit1(sw, "FrmExtendPrint.FrmExtendPrint_Load")
        End Try

    End Sub
#End Region

#Region "定義体読込"

    '------------------------------------------------------------
    ' 機能   ： 画面定義を読み込む
    ' 引数   ： なし
    ' 戻り値 ： True - 正常 ， False - 異常
    '------------------------------------------------------------
    Private Function Read_InitData() As Boolean

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Read_InitData", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            'フォルダの末尾\チェック（なかったらつける）
            Dim filepath As String = CASTCommon.GetFSKJIni("COMMON", "PRT_FLD")
            If filepath.EndsWith("\") = False Then
                filepath = filepath & "\"
            End If

            '帳票印刷画面定義ファイル名の設定
            PrtInf.PrtFileName = filepath & PrtInf.PrtID & "_" & PrtInf.PrtName & "\" & PrtInf.PrtID & "_" & PrtInf.PrtName & "_DSP.ini"

            'ファイル存在確認
            If Not System.IO.File.Exists(PrtInf.PrtFileName) Then
                LOG.Write_Err("FrmExtendPrint.Read_InitData", String.Format(ERRLOG_001, PrtInf.PrtFileName))
                Return False
            End If

            '画面の基準点を設定する
            RefP.Point_X_Label = Set_RefP(GetIniFileValue(PrtInf.PrtFileName, "COMMON", "TOP_X"), 100)
            RefP.Point_X_Input = Set_RefP(GetIniFileValue(PrtInf.PrtFileName, "COMMON", "INPUT_X"), 250)
            RefP.Point_Y = Set_RefP(GetIniFileValue(PrtInf.PrtFileName, "COMMON", "TOP_Y"), 100)
            RefP.Margin = Set_RefP(GetIniFileValue(PrtInf.PrtFileName, "COMMON", "LINE_HEIGHT"), 50)
            RefP.ButtonPos = Set_RefP(GetIniFileValue(PrtInf.PrtFileName, "COMMON", "BUTTON_POS"), 3)

            'プリンタ名取得
            Dim prtrsplits As String() = CASTCommon.GetIniFileValues(PrtInf.PrtFileName, "PRINTERS", "PRINTER")
            If prtrsplits Is Nothing Then
                PrtInf.PrtDevNameArray = Nothing
            Else
                PrtInf.PrtDevNameArray = New String(prtrsplits.Length - 1) {}
                Dim i As Integer = 0
                For Each prtrvalue As String In prtrsplits
                    PrtInf.PrtDevNameArray(i) = prtrvalue.Trim
                    i += 1
                Next
            End If

            '業務固有印刷メソッド情報取得
            ExInf.PrtExFlag = False
            ExInf.PrtDLL = CASTCommon.GetIniFileValue(PrtInf.PrtFileName, "EXTERNAL", "DLL")
            ExInf.PrtClass = CASTCommon.GetIniFileValue(PrtInf.PrtFileName, "EXTERNAL", "CLASS")
            ExInf.PrtMethod = CASTCommon.GetIniFileValue(PrtInf.PrtFileName, "EXTERNAL", "METHOD")

            If ExInf.PrtDLL = "err" AndAlso _
               ExInf.PrtClass = "err" AndAlso _
               ExInf.PrtMethod = "err" Then
                ExInf.PrtExFlag = False
            Else
                If ExInf.PrtDLL <> "err" AndAlso _
                   ExInf.PrtClass <> "err" AndAlso _
                   ExInf.PrtMethod <> "err" Then
                    ExInf.PrtExFlag = True
                    ExInf.PrtDLL = ExInf.PrtDLL.Trim
                    ExInf.PrtClass = ExInf.PrtClass.Trim
                    ExInf.PrtMethod = ExInf.PrtMethod.Trim
                Else
                    '[EXTERNAL]セクションの定義が揃っていないため、エラー
                    LOG.Write_Err("FrmExtendPrint.Read_InitData", String.Format(ERRLOG_002, PrtInf.PrtFileName, "EXTERNAL"))
                    Return False
                End If
            End If

            'フォームのタイトルを設定
            Me.Text = PrtInf.PrtID

            'ラベルに印刷条件画面のタイトルを設定
            PrtTitle.Text = "＜" & PrtInf.PrtTitleName & "＞"

            'タイトルラベルを設定する
            PrtTitle.Left = Set_RefP(GetIniFileValue(PrtInf.PrtFileName, "COMMON", "TITLE_X"), 0)
            PrtTitle.Top = Set_RefP(GetIniFileValue(PrtInf.PrtFileName, "COMMON", "TITLE_Y"), 8)

            Dim fontsize As Single
            Dim fontsizestr As String = GetIniFileValue(PrtInf.PrtFileName, "COMMON", "TITLE_FONTSIZE")

            If Single.TryParse(fontsizestr, fontsize) = False Then
                fontsize = 14.25
            End If

            If fontsize <= 0 Then
                fontsize = 14.25
            End If

            PrtTitle.Font = New Font("ＭＳ ゴシック", fontsize, FontStyle.Bold)

            'ラベルにログインID、システム日付を設定
            GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '印刷・終了ボタン設定（デフォルト：右寄せ）
            If RefP.ButtonPos = 2 Then      '中央
                btnPrt.Location = New System.Drawing.Point(277, 521)
                btnClose.Location = New System.Drawing.Point(403, 521)
                btnPrt.Update()
                btnClose.Update()
            ElseIf RefP.ButtonPos = 1 Then  '左寄せ
                btnPrt.Location = New System.Drawing.Point(20, 521)
                btnClose.Location = New System.Drawing.Point(146, 521)
                btnPrt.Update()
                btnClose.Update()
            End If

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Read_InitData")
        End Try

        Return True
    End Function

    '------------------------------------------------------------
    ' 機能   ： 部品情報を読み込む
    ' 引数   ： なし
    ' 戻り値 ： True - 正常 ， False - 異常
    '------------------------------------------------------------
    Private Function Make_Parts() As Boolean

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_Parts", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            '==============================================================================
            ' 帳票印刷画面定義ファイルの解析
            '
            ' 例：KFJPRNT110_DEF.ini
            '-------------------------------------------------------------------------
            ' [ITEMS]
            ' ITEM=COMBO_CODE, DB_KIND=1,LABEL=取引先検索,LABEL_STYLE=1,
            '      LABEL2=取引先コード, LABEL2_STYLE=2, TAB_INDEX=1,FOCUS=1, SQL_NO=1:2
            '
            ' ITEM=DATE, LABEL=振 替 日, LABEL_STYLE=2, DATE_TYPE=3,
            '      INIT_DATE=0,TAB_INDEX=2, SQL_NO=3
            '-------------------------------------------------------------------------
            '==============================================================================

            '部品情報を読み込む
            Dim strsplits As String() = CASTCommon.GetIniFileValues(PrtInf.PrtFileName, "ITEMS", "ITEM")
            If strsplits Is Nothing Then
                '[ITEMS]セクションが定義されていない場合、エラー
                LOG.Write_Err("FrmExtendPrint.Make_Parts", String.Format(ERRLOG_003, PrtInf.PrtFileName, "ITEMS"))
                Return False
            End If

            Dim itemlist As New List(Of String(,))      '読み込んだ部品情報（itemproplist）を格納する
            Dim itemproplist As New List(Of String())   'ITEMキーの解析結果を格納する（部品プロパティ名,値）
            Dim itemsqllist As New List(Of String)      '定義ファイルから読み込まれたSQL番号を保持する（重複チェック用）
            Dim itemtablist As New List(Of String)      '定義ファイルから読み込まれたタブ順番号を保持する（重複チェック用）
            Dim itemcount As Integer = 1                '読み込んだ部品情報の数

            '定義されている部品の数だけループする
            For Each strValue As String In strsplits

                'ITEMキーを解析する
                'itemsplit(0)="COMBO_CODE"
                'itemsplit(1)="DB_KIND=1"
                'itemsplit(2)="LABEL=取引先検索"
                ':
                Dim itemsplit As String() = Split(strValue, ",")
                itemproplist.Clear()

                For i As Integer = 0 To itemsplit.Length - 1

                    '部品プロパティを取得する
                    ' 部品名取得の場合     : propinf(0)="COMBO_CODE"
                    '                        propinf(1)=Nothing
                    ' プロパティ取得の場合 : propinf(0)="DB_KIND"
                    '                        propinf(1)="1"

                    Dim propinf As String() = New String(1) {}
                    Dim index As Integer = itemsplit(i).IndexOf("=")

                    If index < 1 Then  '部品名取得
                        propinf(0) = itemsplit(i).Trim
                        propinf(1) = Nothing
                    Else               'プロパティ取得
                        propinf(0) = itemsplit(i).Substring(0, index).Trim
                        propinf(1) = itemsplit(i).Substring(index + 1).Trim

                        'COMBO_CODEのプロパティWHEREは文字列の途中に
                        'カンマが入る場合があるため、文字列チェックを行う
                        If propinf(0) = "WHERE" Then
                            'プロパティ値の先頭がダブルクォートの場合
                            If propinf(1).StartsWith(ControlChars.Quote) = True Then
                                'ダブルクォートで括られた文字列を取得する
                                For j As Integer = i + 1 To itemsplit.Length - 1
                                    i = j
                                    propinf(1) = propinf(1) & "," & itemsplit(j).Trim
                                    If itemsplit(j).Trim.EndsWith(ControlChars.Quote) = True Then
                                        Exit For
                                    End If
                                Next
                            End If
                        End If
                    End If

                    itemproplist.Add(propinf)
                Next

                '部品プロパティを格納する配列を作成する
                Dim propdef As String(,) = Nothing
                Select Case itemproplist(0)(0)
                    Case "COMBO_CODE"
                        propdef = pCOMBO_CODE.Clone
                    Case "COMBO_SQL"
                        propdef = pCOMBO_SQL.Clone
                    Case "CODE"
                        propdef = pCODE.Clone
                    Case "RANGE_CODE"
                        propdef = pRANGE_CODE.Clone
                    Case "DATE"
                        propdef = pDATE.Clone
                    Case "RANGE_DATE"
                        propdef = pRANGE_DATE.Clone
                    Case "COMBO"
                        propdef = pCOMBO.Clone
                    Case "RADIO"
                        propdef = pRADIO.Clone
                    Case "CHECK"
                        propdef = pCHECK.Clone
                    Case "TEXT"
                        propdef = pTEXT.Clone
                    Case "LABEL"
                        propdef = pLABEL.Clone
                    Case Else
                        'NOP
                End Select

                '部品名が不正の場合、エラー
                If propdef Is Nothing Then
                    LOG.Write_Err("FrmExtendPrint.Make_Parts", String.Format(ERRLOG_004, PrtInf.PrtFileName, "ITEMS", itemcount, itemproplist(0)(0)))
                    Return False
                End If

                '部品プロパティ設定処理
                Dim err As String = ""
                If Set_Prop(propdef, itemproplist, itemsqllist, itemtablist, itemcount) = False Then
                    Return False
                End If

                '設定した情報をリストに格納
                itemlist.Add(propdef)

                '部品数が25個以上になったら読み込まない
                If itemlist.Count >= 25 Then
                    Exit For
                End If

                itemcount += 1
            Next

            '部品作成メソッドを呼び出す
            'Make_部品名(表示行数,部品プロパティ配列,読込部品数)
            Dim row As Integer = 0
            For i As Integer = 0 To itemlist.Count - 1
                Select Case itemlist(i)(0, 1)
                    Case "COMBO_CODE"
                        row = Make_COMBO_CODE(row, itemlist(i), i + 1)
                    Case "CODE"
                        row = Make_CODE(row, itemlist(i), i + 1)
                    Case "RANGE_CODE"
                        row = Make_RANGE_CODE(row, itemlist(i), i + 1)
                    Case "DATE"
                        row = Make_DATE(row, itemlist(i), i + 1)
                    Case "RANGE_DATE"
                        row = Make_RANGE_DATE(row, itemlist(i), i + 1)
                    Case "COMBO"
                        row = Make_COMBO(row, itemlist(i), i + 1)
                    Case "RADIO"
                        row = Make_RADIO(row, itemlist(i), i + 1)
                    Case "CHECK"
                        row = Make_CHECK(row, itemlist(i), i + 1)
                    Case "TEXT"
                        row = Make_TEXT(row, itemlist(i), i + 1)
                    Case "LABEL"
                        row = Make_LABEL(row, itemlist(i), i + 1)
                    Case "COMBO_SQL"
                        row = Make_COMBO_SQL(row, itemlist(i), i + 1)
                    Case Else
                        'NOP
                End Select

                '読込失敗
                If row = -1 Then
                    Return False
                End If
            Next

            For i As Integer = 0 To PartsList.Count - 1

                'フォームに追加する
                For j As Integer = 0 To PartsList(i).PartsLabelList.Count - 1
                    PartsList(i).PartsLabelList(j).TextAlign = ContentAlignment.MiddleLeft
                    Me.Controls.Add(PartsList(i).PartsLabelList(j))
                Next

                For j As Integer = 0 To PartsList(i).PartsInputList.Count - 1
                    AddHandler PartsList(i).PartsInputList(j).GotFocus, AddressOf CAST.GotFocus
                    AddHandler PartsList(i).PartsInputList(j).LostFocus, AddressOf CAST.LostFocus
                    Me.Controls.Add(PartsList(i).PartsInputList(j))
                Next

                '初期フォーカスを設定する
                If PartsList(i).PartsFocus = "1" Then
                    If PartsList(i).PartsName = "COMBO_CODE" Then
                        Me.ActiveControl = PartsList(i).PartsInputList(2)
                    ElseIf PartsList(i).PartsName = "COMBO_SQL" Then
                        Me.ActiveControl = PartsList(i).PartsInputList(1)
                    Else
                        Me.ActiveControl = PartsList(i).PartsInputList(0)
                    End If
                End If
            Next

            'タブ順を設定する
            Set_TabIndex()
        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_Parts")
        End Try

        Return True
    End Function
#End Region

#Region "部品作成処理"
    '------------------------------------------------------------
    '部品作成処理
    '部品定義に従ってのプロパティを設定し、部品を作成する
    '戻り値： -1       - 異常
    '         それ以外 - 正常
    '------------------------------------------------------------

    '------------------------------------------------------------
    '取引先検索／学校検索
    '------------------------------------------------------------
    Private Function Make_COMBO_CODE(ByVal row As Integer, ByVal prop As String(,), ByVal group As Integer) As Integer

        '定義例
        '-------------------------------------------------------------------------
        ' [ITEM]
        ' ITEM=COMBO_CODE, DB_KIND=1,LABEL=取引先検索,LABEL_STYLE=1,
        '      LABEL2=取引先コード, LABEL2_STYLE=2, TAB_INDEX=1,FOCUS=1,SQL_NO=1:2
        '--------------------------------------------------------------------------

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_COMBO_CODE", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            Dim index As String = prop(8, 1)                    'TAB_INDEX
            Dim dbkind As String = prop(1, 1)                   'DB_KIND
            Dim name As String = "COMBO_CODE_" & group & "_"
            Dim where As String = ""

            If prop(6, 1).Length > 0 Then
                where = Set_Replace_Quote(prop(6, 1))           'WHERE
            End If

            Dim pinf As PartsInformation = New PartsInformation()
            pinf.PartsLabelList = New List(Of Label)
            pinf.PartsInputList = New List(Of Control)
            pinf.PartsOptionList = Nothing                      '学校検索用
            pinf.PartsName = "COMBO_CODE"
            pinf.PartsFocus = prop(9, 1)                        'FOCUS
            pinf.PartsOption = where                            'WHERE句に追加する条件
            pinf.PartsFlag = True                               'ALL9判定
            pinf.PartsSqlno = Split(prop(10, 1), ":")           'SQL_NO

            If prop(7, 1) = "NO" Then                           'ALL9
                pinf.PartsFlag = False
            End If

            Dim lbl1 As Label = New Label()                 'ラベル領域の1行目
            Dim lbl2 As Label = New Label()                 'ラベル領域の2行目
            Dim cmbFurikana As ComboBox = New ComboBox()    'フリガナを格納するコンボボックス
            Dim cmbToriname As ComboBox = New ComboBox()    'コード検索結果を格納するコンボボックス
            Dim txtCode_s As TextBox = New TextBox()        '主コード
            Dim lblToriname As Label = New Label()          '取引先/学校名表示

            '1行目の設定
            Set_Labels(lbl1, name & "1", prop(2, 1), prop(3, 1), row, 0, 100)
            Set_Controls(cmbFurikana, name & "2", "", row, 0, 50, dbkind)
            Set_Controls(cmbToriname, name & "3", "", row, 52, 355, dbkind)
            row += 1

            'コンボボックスのプロパティ設定
            cmbFurikana.Items.AddRange(CmbItemList)
            cmbFurikana.TabStop = False
            cmbToriname.TabStop = False
            cmbFurikana.DropDownStyle = ComboBoxStyle.DropDownList
            cmbToriname.DropDownStyle = ComboBoxStyle.DropDownList
            cmbFurikana.Font = New Font("ＭＳ ゴシック", 9.75)
            cmbToriname.Font = New Font("ＭＳ ゴシック", 9.75)

            '2行目の設定
            Set_Labels(lbl2, name & "4", prop(4, 1), prop(5, 1), row, 0, 100)
            Set_Controls(txtCode_s, name & "5", "", row, 0, 90, index)

            'テキストボックスのプロパティ設定
            txtCode_s.ImeMode = Windows.Forms.ImeMode.Disable
            txtCode_s.MaxLength = 10
            AddHandler txtCode_s.Validating, AddressOf TextBox_Validating
            AddHandler txtCode_s.KeyPress, AddressOf CASTx01.KeyPress

            'ラベルのプロパティ設定
            lblToriname.BorderStyle = BorderStyle.Fixed3D
            lblToriname.TextAlign = ContentAlignment.MiddleLeft

            'リストに作成したコントロールを格納
            TabIndexList.Add(txtCode_s)
            pinf.PartsLabelList.Add(lbl1)
            pinf.PartsLabelList.Add(lbl2)
            pinf.PartsInputList.Add(cmbFurikana)
            pinf.PartsInputList.Add(cmbToriname)
            pinf.PartsInputList.Add(txtCode_s)

            '副コード入力欄設定（自振、総振）
            If dbkind = "1" OrElse dbkind = "2" Then
                Dim lblHyphen As Label = New Label()      '主コードと副コードの間のハイフンを表示する
                Dim txtCode_f As TextBox = New TextBox()  '副コード
                Dim shorikbn As String = "1"

                If dbkind = "2" Then
                    shorikbn = "3"
                End If

                Set_Controls(lblHyphen, name & "6", "－", row, 95, 15, index)
                Set_Controls(txtCode_f, name & "7", "", row, 120, 25, index)
                Set_Controls(lblToriname, name & "8", "", row, 155, 252, index)

                '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                AddHandler txtCode_s.Validated, AddressOf TextBox_Validated
                '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***

                txtCode_f.ImeMode = Windows.Forms.ImeMode.Disable
                txtCode_f.MaxLength = 2
                AddHandler txtCode_f.Validating, AddressOf TextBox_Validating
                AddHandler txtCode_f.KeyPress, AddressOf CASTx01.KeyPress
                AddHandler txtCode_f.Validated, AddressOf TextBox_Validated

                'コンボボックスの初期アイテムを設定（全件検索結果）
                'shorikbn： 1 - 自振
                '        ： 3 - 総振
                If GCom.SelectItakuName("", cmbToriname, txtCode_s, txtCode_f, shorikbn, where) = -1 Then
                    LOG.Write_Err("FrmExtendPrint.Make_COMBO_CODE", String.Format(ERRLOG_010, lbl1.Text))
                    Return -1
                End If

                TabIndexList.Add(txtCode_f)
                pinf.PartsLabelList.Add(lblHyphen)
                pinf.PartsInputList.Add(txtCode_f)

            Else '学校検索

                Set_Controls(lblToriname, name & "8", "", row, 100, 307, index)
                AddHandler txtCode_s.Validated, AddressOf TextBox_Validated

                '学校検索の全件検索結果をPartsOptionListに格納する
                pinf.PartsOptionList = New List(Of String())
                If Set_GAKKOU_COMBO(cmbToriname, "", where, pinf.PartsOptionList) = -1 Then
                    LOG.Write_Err("FrmExtendPrint.Make_COMBO_CODE", String.Format(ERRLOG_010, lbl1.Text))
                    Return -1
                End If
            End If

            AddHandler cmbFurikana.SelectedIndexChanged, AddressOf cmbKana_SelectedIndexChanged
            AddHandler cmbToriname.SelectedIndexChanged, AddressOf cmbToriName_SelectedIndexChanged
            AddHandler cmbFurikana.KeyPress, AddressOf CAST.KeyPress
            AddHandler cmbToriname.KeyPress, AddressOf CAST.KeyPress

            row += 1


            pinf.PartsInputList.Add(lblToriname)
            PartsList.Add(pinf)

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_COMBO_CODE")
        End Try

        Return row
    End Function

    '------------------------------------------------------------
    'SQLコンボ検索
    '------------------------------------------------------------
    Private Function Make_COMBO_SQL(ByVal row As Integer, ByVal prop As String(,), ByVal group As Integer) As Integer

        '定義例
        '-----------------------------------------------------------------------
        '[ITEM]
        'ITEM=COMBO_SQL,COMBO_DEF=市町村検索, LABEL=市町村検索,LABEL_STYLE=1,
        '     LABEL2=市町村コード,LABEL2_STYLE=2,LABEL3=市町村名,LABEL3_STYLE=2,
        '     TAB_INDEX=2,FOCUS=0, SQL_NO=3
        '
        '[市町村検索]
        'ORIGIN=SELECT CODE, CODE_NAME FROM XXMAST
        '-----------------------------------------------------------------------

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_COMBO_SQL", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try
            Dim index As String = prop(8, 1)                           'TAB_INDEX
            Dim name As String = "COMBO_SQL_" & group & "_"

            Dim max As Integer = Integer.Parse(prop(11, 1))      'IN_SIZE
            Dim maxstr As String = ""

            Dim pinf As PartsInformation = New PartsInformation()
            pinf.PartsLabelList = New List(Of Label)
            pinf.PartsInputList = New List(Of Control)
            pinf.PartsName = "COMBO_SQL"
            pinf.PartsFocus = prop(9, 1)                              'FOCUS
            pinf.PartsSqlno = New String(0) {prop(10, 1)}             'SQL_NO

            Dim lbl1 As Label = New Label()              'ラベル領域の1行目
            Dim lbl2 As Label = New Label()              'ラベル領域の2行目
            Dim lbl3 As Label = New Label()              'ラベル領域の3行目
            Dim lblToriname As Label = New Label()       '取引先名(日本語)表示用
            Dim cmbToriname As ComboBox = New ComboBox() 'SQL検索結果格納用
            Dim txtCode As TextBox = New TextBox()       'コード表示用

            '1行目の設定
            Set_Labels(lbl1, name & "1", prop(2, 1), prop(3, 1), row, 0, 100)
            Set_Controls(cmbToriname, name & "2", "", row, 0, 300, "4")
            row += 1

            '2行目の設定
            Set_Labels(lbl2, name & "3", prop(4, 1), prop(5, 1), row, 0, 100)
            Set_Controls(txtCode, name & "4", "", row, 0, 90, index)
            row += 1

            '3行目の設定
            Set_Labels(lbl3, name & "5", prop(6, 1), prop(7, 1), row, 0, 100)
            Set_Controls(lblToriname, name & "6", "", row, 0, 300, index)
            row += 1

            'コンボボックスのプロパティ設定
            cmbToriname.TabStop = False
            cmbToriname.DropDownStyle = ComboBoxStyle.DropDownList
            cmbToriname.Font = New Font("ＭＳ ゴシック", 9.75)

            '部品固有情報を取得する
            Dim SQL As New StringBuilder
            Dim sqldef As String = CASTCommon.GetIniFileValue(PrtInf.PrtFileName, prop(1, 1), "ORIGIN")
            If sqldef = "err" Then
                LOG.Write_Err("FrmExtendPrint.Make_COMBO_SQL", String.Format(ERRLOG_003, PrtInf.PrtFileName, prop(1, 1)))
                Return -1
            End If

            'タブスペースを削除
            sqldef = sqldef.Replace(vbTab, "")
            SQL.Append(sqldef.Trim)

            'コンボボックスアイテム設定
            cmbToriname.Items.Add(Space(50))
            Dim datalist As List(Of String()) = Get_DB_Data(SQL, 2)
            If datalist Is Nothing Then
                LOG.Write_Err("FrmExtendPrint.Make_COMBO_SQL", String.Format(ERRLOG_010, Label1.Text))
                Return -1
            End If

            'コンボボックスにSQL検索結果を格納する
            'アイテムに"コード ： 名前"を設定
            For Each dbdata As String() In datalist
                cmbToriname.Items.Add(dbdata(0) & " : " & dbdata(1))
            Next
            cmbToriname.Text = cmbToriname.Items.Item(0).ToString

            'ラベルのプロパティ設定
            lblToriname.BorderStyle = BorderStyle.Fixed3D
            lblToriname.TextAlign = ContentAlignment.MiddleLeft

            'テキストボックスのプロパティ設定
            txtCode.ImeMode = Windows.Forms.ImeMode.Disable
            txtCode.MaxLength = max
            txtCode.Text = maxstr.PadLeft(max, "0")
            txtCode.Width = txtCode.PreferredSize.Width
            txtCode.Text = ""

            AddHandler txtCode.Validating, AddressOf TextBox_Validating
            AddHandler txtCode.Validated, AddressOf TextBox_Validated
            AddHandler txtCode.KeyPress, AddressOf CASTx01.KeyPress

            AddHandler cmbToriname.SelectedIndexChanged, AddressOf cmbToriName_SelectedIndexChanged
            AddHandler cmbToriname.KeyPress, AddressOf CAST.KeyPress

            '作成したコントロールをリストに格納する
            TabIndexList.Add(txtCode)
            pinf.PartsLabelList.Add(lbl1)
            pinf.PartsLabelList.Add(lbl2)
            pinf.PartsLabelList.Add(lbl3)
            pinf.PartsInputList.Add(cmbToriname)
            pinf.PartsInputList.Add(txtCode)
            pinf.PartsInputList.Add(lblToriname)

            PartsList.Add(pinf)

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_COMBO_SQL")
        End Try

        Return row
    End Function

    '------------------------------------------------------------
    'コード入力
    '------------------------------------------------------------
    Private Function Make_CODE(ByVal row As Integer, ByVal prop As String(,), ByVal group As Integer) As Integer

        '定義例
        '-----------------------------------------------------------------------
        '[ITEM]
        'ITEM=CODE, LABEL=取引先コード,LABEL_STYLE=1,CODE_TYPE=1
        '     TAB_INDEX=3,FOCUS=0,SQL_NO=4:5
        '-----------------------------------------------------------------------

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_CODE", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try
            Dim index As String = prop(4, 1)                         'TAB_INDEX
            Dim name As String = "CODE_" & group & "_"

            Dim pinf As PartsInformation = New PartsInformation()
            pinf.PartsLabelList = New List(Of Label)
            pinf.PartsInputList = New List(Of Control)
            pinf.PartsName = "CODE"
            pinf.PartsFocus = prop(5, 1)                             'FOCUS
            pinf.PartsSqlno = Split(prop(6, 1), ":")                 'SQL_NO

            Dim lbl As Label = New Label()          'ラベル域表示用
            Dim txtCode_s As TextBox = New TextBox() '主コード

            '1行目の設定
            Set_Labels(lbl, name & "1", prop(1, 1), prop(2, 1), row, 0, 100)
            Set_Controls(txtCode_s, name & "2", "", row, 0, 90, index)

            'テキストボックスのプロパティ設定
            txtCode_s.ImeMode = Windows.Forms.ImeMode.Disable
            txtCode_s.MaxLength = 10
            AddHandler txtCode_s.Validating, AddressOf TextBox_Validating
            AddHandler txtCode_s.KeyPress, AddressOf CASTx01.KeyPress

            '作成したコントロールをリストに格納
            TabIndexList.Add(txtCode_s)
            pinf.PartsLabelList.Add(lbl)
            pinf.PartsInputList.Add(txtCode_s)

            '副コード設定
            If prop(3, 1) = "2" Then
                Dim lblHyphen As Label = New Label()          'ハイフン
                Dim txtCode_f As TextBox = New TextBox()      '副コード

                Set_Controls(lblHyphen, name & "3", "－", row, 100, 20, index)
                Set_Controls(txtCode_f, name & "4", "", row, 130, 25, index)

                txtCode_f.ImeMode = Windows.Forms.ImeMode.Disable
                txtCode_f.MaxLength = 2
                AddHandler txtCode_f.Validating, AddressOf TextBox_Validating
                AddHandler txtCode_f.KeyPress, AddressOf CASTx01.KeyPress

                TabIndexList.Add(txtCode_f)
                pinf.PartsLabelList.Add(lblHyphen)
                pinf.PartsInputList.Add(txtCode_f)

            End If

            row += 1
            PartsList.Add(pinf)

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_CODE")
        End Try

        Return row
    End Function

    '------------------------------------------------------------
    '範囲入力
    '------------------------------------------------------------
    Private Function Make_RANGE_CODE(ByVal row As Integer, ByVal prop As String(,), ByVal group As Integer) As Integer

        '定義例
        '-----------------------------------------------------------------------
        '[ITEM]
        'ITEM=RANGE_CODE, LABEL=取引先コード,LABEL_STYLE=1,IN_SIZE=10
        '     TAB_INDEX=4,FOCUS=0,SQL_NO=6:7
        '-----------------------------------------------------------------------

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_RANGE_CODE", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try
            Dim max As Integer = Integer.Parse(prop(3, 1))         'IN_SIZE
            Dim maxstr As String = ""

            Dim index As String = prop(4, 1)                       'TAB_INDEX
            Dim name As String = "RANGE_CODE_" & group & "_"

            Dim pinf As PartsInformation = New PartsInformation()
            pinf.PartsLabelList = New List(Of Label)
            pinf.PartsInputList = New List(Of Control)
            pinf.PartsName = "RANGE_CODE"
            pinf.PartsFocus = prop(5, 1)                           'FOCUS
            pinf.PartsSqlno = Split(prop(6, 1), ":")               'SQL_NO

            Dim lbl As Label = New Label()              'ラベル域表示用
            Dim lblRange As Label = New Label()          '～表示用
            Dim txtStartCode As TextBox = New TextBox()  '開始コード
            Dim txtEndCode As TextBox = New TextBox()    '終了コード

            '1行目の設定
            Set_Labels(lbl, name & "1", prop(1, 1), prop(2, 1), row, 0, 100)
            Set_Controls(txtStartCode, name & "2", "", row, 0, 100, index)
            Set_Controls(lblRange, name & "3", "～", row, 110, 20, index)
            Set_Controls(txtEndCode, name & "4", "", row, 140, 100, index)
            row += 1

            'テキストボックスのプロパティ設定
            txtStartCode.ImeMode = Windows.Forms.ImeMode.Disable
            txtEndCode.ImeMode = Windows.Forms.ImeMode.Disable
            txtStartCode.MaxLength = max
            txtEndCode.MaxLength = max

            txtStartCode.Text = maxstr.PadLeft(max, "0")
            Dim txtWidth As Integer = txtStartCode.PreferredSize.Width
            txtStartCode.Text = ""

            txtStartCode.Width = txtWidth
            txtEndCode.Width = txtWidth

            lblRange.Left = txtStartCode.Left + txtWidth + 2
            txtEndCode.Left = lblRange.Left + 26

            AddHandler txtStartCode.KeyPress, AddressOf CASTx01.KeyPress
            AddHandler txtEndCode.KeyPress, AddressOf CASTx01.KeyPress
            AddHandler txtStartCode.Validating, AddressOf TextBox_Validating
            AddHandler txtEndCode.Validating, AddressOf TextBox_Validating

            '作成したコントロールをリストに格納
            TabIndexList.Add(txtStartCode)
            TabIndexList.Add(txtEndCode)
            pinf.PartsLabelList.Add(lbl)
            pinf.PartsLabelList.Add(lblRange)
            pinf.PartsInputList.Add(txtStartCode)
            pinf.PartsInputList.Add(txtEndCode)
            PartsList.Add(pinf)

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_RANGE_CODE")
        End Try

        Return row
    End Function

    '------------------------------------------------------------
    '日付入力
    '------------------------------------------------------------
    Private Function Make_DATE(ByVal row As Integer, ByVal prop As String(,), ByVal group As Integer) As Integer

        '定義例
        '-----------------------------------------------------------------------
        '[ITEM]
        'ITEM=DATE, LABEL=取引先コード,LABEL_STYLE=1,DATE_TYPE=1,INIT_DATE=2
        '     BUSINESS_DAY=2,TAB_INDEX=5,FOCUS=0,SQL_NO=8
        '-----------------------------------------------------------------------

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_DATE", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)  '現在の日付
            Dim initdate As String = Nothing                                           '初期値を格納
            Dim index As String = prop(6, 1)                        'TAB_INDEX
            Dim name As String = "DATE_" & group & "_"
            Dim datetype As String

            If prop(3, 1) = "2" Then                                'DATE_TYPE
                datetype = "2"
            Else
                datetype = "1"
            End If

            Dim pinf As PartsInformation = New PartsInformation
            pinf.PartsLabelList = New List(Of Label)
            pinf.PartsInputList = New List(Of Control)
            pinf.PartsName = "DATE"
            pinf.PartsFocus = prop(7, 1)                           'FOCUS
            pinf.PartsSqlno = New String(0) {prop(8, 1)}           'SQL_NO

            Dim lbl As Label = New Label()             'ラベル域表示用
            Dim lblYear As Label = New Label()          '年（ラベル）
            Dim lblMonth As Label = New Label()         '月（ラベル）
            Dim txtYear As TextBox = New TextBox()      '年入力用
            Dim txtMonth As TextBox = New TextBox()     '月入力用

            Set_Labels(lbl, name & "1", prop(1, 1), prop(2, 1), row, 0, 100)
            Set_Controls(txtYear, name & "2", "", row, 0, 44, index)
            Set_Controls(lblYear, name & "3", "年", row, 46, 24, index)
            Set_Controls(txtMonth, name & "4", "", row, 72, 24, index)
            Set_Controls(lblMonth, name & "5", "月", row, 98, 24, index)

            txtYear.ImeMode = Windows.Forms.ImeMode.Disable
            txtMonth.ImeMode = Windows.Forms.ImeMode.Disable
            txtYear.MaxLength = 4
            txtMonth.MaxLength = 2
            AddHandler txtYear.Validating, AddressOf TextBox_Validating
            AddHandler txtMonth.Validating, AddressOf TextBox_Validating
            AddHandler txtYear.KeyPress, AddressOf CASTx01.KeyPress
            AddHandler txtMonth.KeyPress, AddressOf CASTx01.KeyPress

            '初期値設定
            If prop(4, 1) = "1" Then     '初期値がシステム日付の場合
                ' 2016/07/13 ＦＪＨ）小嶋 ADD 【PG】拡張印刷仕様変更(RSV2<小浜信金>) ---------------- START
                ' 日付関連部品の初期値に、暦日のn営業日前/後を設定可能とする。

                'initdate = strSysDate

                Dim onDate As Date = GCom.SET_DATE(strSysDate)           '日付変数

                '------------------------------------------------
                '画面表示時にn日前/後を表示する
                '------------------------------------------------
                If prop(5, 1) = "" Then
                Else
                    Dim aDayTeam As Integer = Integer.Parse(prop(5, 1))
                    onDate = onDate.AddDays(aDayTeam)
                End If

                initdate = String.Format("{0:yyyyMMdd}", onDate)
                ' 2016/07/13 ＦＪＨ）小嶋 DEL 【PG】拡張印刷仕様変更(RSV2<小浜信金>) ------------------ END

            ElseIf prop(4, 1) = "2" Then '初期値が営業日指定の場合

                Dim strGetdate As String = ""  '営業日計算結果

                '------------------------------------------------
                '全休日情報を蓄積
                '------------------------------------------------
                Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

                If bRet = False Then
                    LOG.Write_Err("FrmExtendPrint.Make_DATE", ERRLOG_011)
                    Return -1
                End If

                '------------------------------------------------
                '画面表示時に振替日に前営業日を表示する
                '------------------------------------------------
                Dim aDayTeam As Integer = System.Math.Abs(Integer.Parse(prop(5, 1)))
                Dim aFrontBackType As Integer = 0

                If Integer.Parse(prop(5, 1)) < 0 Then
                    aFrontBackType = -1
                End If

                GCom.CheckDateModule(strSysDate, strGetdate, aDayTeam, aFrontBackType)
                initdate = strGetdate
            End If

            '初期値をテキストボックスに反映する
            If Not initdate Is Nothing Then
                txtYear.Text = initdate.Substring(0, 4)
                txtMonth.Text = initdate.Substring(4, 2)
            End If

            '作成したコントロールをリストに格納する
            TabIndexList.Add(txtYear)
            TabIndexList.Add(txtMonth)
            pinf.PartsLabelList.Add(lbl)
            pinf.PartsLabelList.Add(lblYear)
            pinf.PartsLabelList.Add(lblMonth)
            pinf.PartsInputList.Add(txtYear)
            pinf.PartsInputList.Add(txtMonth)

            '日付タイプが"年月日"の場合
            If datetype = "1" Then
                Dim txtDate As TextBox = New TextBox()
                Dim lblDate As Label = New Label()

                Set_Controls(txtDate, name & "6", "", row, 124, 24, index)
                Set_Controls(lblDate, name & "7", "日", row, 150, 24, index)

                txtDate.ImeMode = Windows.Forms.ImeMode.Disable
                txtDate.MaxLength = 2
                AddHandler txtDate.Validating, AddressOf TextBox_Validating
                AddHandler txtDate.KeyPress, AddressOf CASTx01.KeyPress

                If Not initdate Is Nothing Then
                    txtDate.Text = initdate.Substring(6, 2)
                End If

                TabIndexList.Add(txtDate)
                pinf.PartsLabelList.Add(lblDate)
                pinf.PartsInputList.Add(txtDate)

            End If

            PartsList.Add(pinf)
            row += 1

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_DATE")
        End Try

        Return row
    End Function

    '------------------------------------------------------------
    '日付範囲入力
    '------------------------------------------------------------
    Private Function Make_RANGE_DATE(ByVal row As Integer, ByVal prop As String(,), ByVal group As Integer) As Integer

        '定義例
        '-----------------------------------------------------------------------
        '[ITEM]
        'ITEM=RANGE_DATE, LABEL=取引先コード,LABEL_STYLE=1,DATE_TYPE=1,
        '     INIT_DATE1=2, BUSINESS_DAY1=-1,INIT_DATE2=0,
        '     TAB_INDEX=6,FOCUS=0,SQL_NO=9:10
        '-----------------------------------------------------------------------

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_RANGE_DATE", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)  '現在の日付
            Dim initdate_f As String = Nothing                                         '初期値を格納(開始日)
            Dim initdate_l As String = Nothing                                         '初期値を格納(終了日))

            Dim index As String = prop(8, 1)                      'TAB_INDEX
            Dim name As String = "RANGE_DATE_" & group & "_"

            Dim datetype As String
            If prop(3, 1) = "2" Then                              'DATE_TYPE
                datetype = "2"
            Else
                datetype = "1"
            End If

            Dim pinf As PartsInformation = New PartsInformation()
            pinf.PartsLabelList = New List(Of Label)()
            pinf.PartsInputList = New List(Of Control)()
            pinf.PartsName = "RANGE_DATE"
            pinf.PartsFocus = prop(9, 1)
            pinf.PartsSqlno = Split(prop(10, 1), ":")

            '開始日の部分を作成
            Dim lbl As Label = New Label()                  'ラベル域表示用
            Dim lblStartYear As Label = New Label()          '開始年（ラベル）
            Dim lblStartMonth As Label = New Label()         '開始月（ラベル）
            Dim txtStartYear As TextBox = New TextBox()      '開始年入力用
            Dim txtStartMonth As TextBox = New TextBox()     '開始月入力用

            Set_Labels(lbl, name & "1", prop(1, 1), prop(2, 1), row, 0, 100)
            Set_Controls(txtStartYear, name & "2", "", row, 0, 44, index)
            Set_Controls(lblStartYear, name & "3", "年", row, 46, 24, index)
            Set_Controls(txtStartMonth, name & "4", "", row, 72, 24, index)
            Set_Controls(lblStartMonth, name & "5", "月", row, 98, 24, index)

            'テキストボックスのプロパティ設定
            txtStartYear.ImeMode = Windows.Forms.ImeMode.Disable
            txtStartMonth.ImeMode = Windows.Forms.ImeMode.Disable
            txtStartYear.MaxLength = 4
            txtStartMonth.MaxLength = 2
            AddHandler txtStartYear.Validating, AddressOf TextBox_Validating
            AddHandler txtStartMonth.Validating, AddressOf TextBox_Validating
            AddHandler txtStartYear.KeyPress, AddressOf CASTx01.KeyPress
            AddHandler txtStartMonth.KeyPress, AddressOf CASTx01.KeyPress

            '初期値設定
            If prop(4, 1) = "2" OrElse _
               prop(5, 1) = "2" Then

                '------------------------------------------------
                '全休日情報を蓄積
                '------------------------------------------------
                Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

                If bRet = False Then
                    LOG.Write_Err("FrmExtendPrint.Make_RANGE_DATE", ERRLOG_011)
                    Return -1
                End If

            End If

            If prop(4, 1) = "1" Then      '初期値がシステム日付の場合
                ' 2016/07/13 ＦＪＨ）小嶋 ADD 【PG】拡張印刷仕様変更(RSV2<小浜信金>) ---------------- START
                ' 日付関連部品の初期値に、暦日のn営業日前/後を設定可能とする。

                'initdate_f = strSysDate

                Dim onDate As Date = GCom.SET_DATE(strSysDate)           '日付変数

                '------------------------------------------------
                '画面表示時にn日前/後を表示する
                '------------------------------------------------
                If prop(6, 1) = "" Then
                Else
                    Dim aDayTeam As Integer = Integer.Parse(prop(6, 1))
                    onDate = onDate.AddDays(aDayTeam)
                End If

                initdate_f = String.Format("{0:yyyyMMdd}", onDate)
                ' 2016/07/13 ＦＪＨ）小嶋 DEL 【PG】拡張印刷仕様変更(RSV2<小浜信金>) ------------------ END

            ElseIf prop(4, 1) = "2" Then  '初期値が営業日指定の場合

                Dim strGetdate As String = ""  '営業日計算結果

                '------------------------------------------------
                '画面表示時に振替日に前営業日を表示する
                '------------------------------------------------
                Dim aDayTeam As Integer = System.Math.Abs(Integer.Parse(prop(6, 1)))
                Dim aFrontBackType As Integer = 0

                If Integer.Parse(prop(6, 1)) < 0 Then
                    aFrontBackType = -1
                End If

                GCom.CheckDateModule(strSysDate, strGetdate, aDayTeam, aFrontBackType)
                initdate_f = strGetdate
            End If

            '初期値をテキストボックスに反映
            If Not initdate_f Is Nothing Then
                txtStartYear.Text = initdate_f.Substring(0, 4)
                txtStartMonth.Text = initdate_f.Substring(4, 2)
            End If

            '作成したコントロールをリストに格納
            TabIndexList.Add(txtStartYear)
            TabIndexList.Add(txtStartMonth)
            pinf.PartsLabelList.Add(lbl)
            pinf.PartsLabelList.Add(lblStartYear)
            pinf.PartsLabelList.Add(lblStartMonth)
            pinf.PartsInputList.Add(txtStartYear)
            pinf.PartsInputList.Add(txtStartMonth)

            '終了日の部分を作成
            Dim lblRange As Label = New Label()
            Dim lblEndYear As Label = New Label()
            Dim lblEndMonth As Label = New Label()
            Dim txtEndYear As TextBox = New TextBox()
            Dim txtEndMonth As TextBox = New TextBox()

            'テキストボックスのプロパティ設定
            txtEndYear.ImeMode = Windows.Forms.ImeMode.Disable
            txtEndMonth.ImeMode = Windows.Forms.ImeMode.Disable
            txtEndYear.MaxLength = 4
            txtEndMonth.MaxLength = 2
            AddHandler txtEndYear.Validating, AddressOf TextBox_Validating
            AddHandler txtEndMonth.Validating, AddressOf TextBox_Validating
            AddHandler txtEndYear.KeyPress, AddressOf CASTx01.KeyPress
            AddHandler txtEndMonth.KeyPress, AddressOf CASTx01.KeyPress

            '初期値設定
            If prop(5, 1) = "1" Then      '初期値が現在の日付の場合
                ' 2016/07/13 ＦＪＨ）小嶋 ADD 【PG】拡張印刷仕様変更(RSV2<小浜信金>) ---------------- START
                ' 日付関連部品の初期値に、暦日のn営業日前/後を設定可能とする。

                'initdate_l = strSysDate

                Dim onDate As Date = GCom.SET_DATE(strSysDate)           '日付変数

                '------------------------------------------------
                '画面表示時にn日前/後を表示する
                '------------------------------------------------
                If prop(7, 1) = "" Then
                Else
                    Dim aDayTeam As Integer = Integer.Parse(prop(7, 1))
                    onDate = onDate.AddDays(aDayTeam)
                End If

                initdate_l = String.Format("{0:yyyyMMdd}", onDate)
                ' 2016/07/13 ＦＪＨ）小嶋 DEL 【PG】拡張印刷仕様変更(RSV2<小浜信金>) ------------------ END


            ElseIf prop(5, 1) = "2" Then  '初期値が営業日指定の場合

                Dim strGetdate As String = ""  '営業日計算結果

                '------------------------------------------------
                '画面表示時に振替日に前営業日を表示する
                '------------------------------------------------
                Dim aDayTeam As Integer = System.Math.Abs(Integer.Parse(prop(7, 1)))
                Dim aFrontBackType As Integer = 0

                If Integer.Parse(prop(7, 1)) < 0 Then
                    aFrontBackType = -1
                End If

                GCom.CheckDateModule(strSysDate, strGetdate, aDayTeam, aFrontBackType)
                initdate_l = strGetdate

            End If

            If Not initdate_l Is Nothing Then
                txtEndYear.Text = initdate_l.Substring(0, 4)
                txtEndMonth.Text = initdate_l.Substring(4, 2)
            End If

            If datetype = "1" Then   '年月日表示の場合
                Dim txtStartDate As TextBox = New TextBox()  '開始日入力用
                Dim lblStartDate As Label = New Label()      '開始日（ラベル）
                Dim lblEndDate As Label = New Label()        '終了日入力用
                Dim txtEndDate As TextBox = New TextBox()    '終了日（ラベル）

                Set_Controls(txtStartDate, name & "6", "", row, 124, 24, index)
                Set_Controls(lblStartDate, name & "7", "日", row, 150, 24, index)

                txtStartDate.ImeMode = Windows.Forms.ImeMode.Disable
                txtStartDate.MaxLength = 2
                AddHandler txtStartDate.Validating, AddressOf TextBox_Validating
                AddHandler txtStartDate.KeyPress, AddressOf CASTx01.KeyPress

                If Not initdate_f Is Nothing Then
                    txtStartDate.Text = initdate_f.Substring(6, 2)
                End If

                TabIndexList.Add(txtStartDate)
                pinf.PartsLabelList.Add(lblStartDate)
                pinf.PartsInputList.Add(txtStartDate)

                Set_Controls(lblRange, name & "8", "～", row, 177, 24, index)
                Set_Controls(txtEndYear, name & "9", txtEndYear.Text, row, 207, 44, index)
                Set_Controls(lblEndYear, name & "10", "年", row, 253, 24, index)
                Set_Controls(txtEndMonth, name & "11", txtEndMonth.Text, row, 279, 24, index)
                Set_Controls(lblEndMonth, name & "12", "月", row, 305, 24, index)
                Set_Controls(txtEndDate, name & "13", "", row, 331, 24, index)
                Set_Controls(lblEndDate, name & "14", "日", row, 357, 24, index)

                txtEndDate.ImeMode = Windows.Forms.ImeMode.Disable
                txtEndDate.MaxLength = 2
                AddHandler txtEndDate.Validating, AddressOf TextBox_Validating
                AddHandler txtEndDate.KeyPress, AddressOf CASTx01.KeyPress

                If Not initdate_l Is Nothing Then
                    txtEndDate.Text = initdate_l.Substring(6, 2)
                End If

                TabIndexList.Add(txtEndYear)
                TabIndexList.Add(txtEndMonth)
                TabIndexList.Add(txtEndDate)
                pinf.PartsLabelList.Add(lblRange)
                pinf.PartsLabelList.Add(lblEndYear)
                pinf.PartsLabelList.Add(lblEndMonth)
                pinf.PartsLabelList.Add(lblEndDate)
                pinf.PartsInputList.Add(txtEndYear)
                pinf.PartsInputList.Add(txtEndMonth)
                pinf.PartsInputList.Add(txtEndDate)

            Else          '年月表示の場合
                Set_Controls(lblRange, name & "6", "～", row, 125, 24, index)
                Set_Controls(txtEndYear, name & "7", txtEndYear.Text, row, 155, 44, index)
                Set_Controls(lblEndYear, name & "8", "年", row, 201, 24, index)
                Set_Controls(txtEndMonth, name & "9", txtEndMonth.Text, row, 227, 24, index)
                Set_Controls(lblEndMonth, name & "10", "月", row, 253, 24, index)

                TabIndexList.Add(txtEndYear)
                TabIndexList.Add(txtEndMonth)
                pinf.PartsLabelList.Add(lblRange)
                pinf.PartsLabelList.Add(lblEndYear)
                pinf.PartsLabelList.Add(lblEndMonth)
                pinf.PartsInputList.Add(txtEndYear)
                pinf.PartsInputList.Add(txtEndMonth)

            End If

            PartsList.Add(pinf)
            row += 1

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_RANGE_DATE")
        End Try

        Return row
    End Function

    '------------------------------------------------------------
    'コンボボックス
    '------------------------------------------------------------
    Private Function Make_COMBO(ByVal row As Integer, ByVal prop As String(,), ByVal group As Integer) As Integer

        '定義例
        '-----------------------------------------------------------------------
        ' [ITEM]
        ' ITEM=COMBO, LABEL=出力区分, LABEL_STYLE=2, COMBO_DEF=出力区分,
        '      SELECT=0,TAB_INDEX=7, SQL_NO=11
        '
        ' [出力区分]
        ' ORIGIN=店番ソート,"ORDER BY MISE_NO"
        ' ORIGIN=非ソート,"ORDER BY RECORD_NO_K"
        ' ORIGIN=エラー分のみ,"AND YOBI5_K <> ' ' ORDER BY RECORD_NO_K"
        '-----------------------------------------------------------------------

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_COMBO", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            Dim index As String = prop(5, 1)               'TAB_INDEX
            Dim name As String = "COMBO_" & group & "_"
            Dim maxlen As Integer = 0
            Dim maxtext As String = ""

            Dim pinf As PartsInformation = New PartsInformation()
            pinf.PartsLabelList = New List(Of Label)()
            pinf.PartsInputList = New List(Of Control)()
            pinf.PartsOptionList = New List(Of String())()
            pinf.PartsName = "COMBO"
            pinf.PartsFocus = prop(6, 1)                   'FOCUS
            pinf.PartsSqlno = New String(0) {prop(7, 1)}   'SQL_NO

            'コントロールを作成する
            Dim lbl As Label = New Label()         'ラベル域表示用
            Dim cmb As ComboBox = New ComboBox()   'コンボボックス

            Set_Labels(lbl, name & "1", prop(1, 1), prop(2, 1), row, 0, 100)
            Set_Controls(cmb, name & "2", "", row, 0, 200, index)
            row += 1

            '部品固有情報を取得する
            Dim strsplits As String() = CASTCommon.GetIniFileValues(PrtInf.PrtFileName, prop(3, 1), "ORIGIN")
            If strsplits Is Nothing Then
                LOG.Write_Err("FrmExtendPrint.Make_COMBO", String.Format(ERRLOG_003, PrtInf.PrtFileName, prop(3, 1)))
                Return -1
            End If

            Dim i As Integer = 1
            For Each strvalue As String In strsplits
                Dim search As Integer = strvalue.IndexOf(",")
                If search < 1 Then
                    LOG.Write_Err("FrmExtendPrint.Make_COMBO", String.Format(ERRLOG_009, PrtInf.PrtFileName, prop(1, 1), i))
                    Return -1
                End If

                Dim item1 As String = strvalue.Substring(0, search).Trim     'コンボボックスに表示する文字列
                Dim item2 As String = strvalue.Substring(search + 1).Trim    'SQL置換文字列

                If item2.StartsWith(ControlChars.Quote) = True Then   '文字列の先頭がダブルクォートの場合

                    Dim quoteindex As Integer = item2.Substring(1).IndexOf(ControlChars.Quote)
                    If quoteindex > 0 Then
                        'ダブルクォートを取り除く
                        item2 = item2.Substring(1, quoteindex)
                    Else
                        '文字列がダブルクォートで囲まれていない場合は定義エラー
                        LOG.Write_Err("FrmExtendPrint.Make_COMBO", String.Format(ERRLOG_009, PrtInf.PrtFileName, prop(3, 1), i))
                        Return -1
                    End If

                Else                                                  'それ以外
                    Dim items As String() = item2.Split(",")
                    item2 = items(0)
                End If

                cmb.Items.Add(item1)
                pinf.PartsOptionList.Add(New String() {item1, item2})

                'アイテムに格納する文字列の最大長を取得する
                Dim len As Integer = System.Text.Encoding.GetEncoding(932).GetByteCount(item1)
                If maxlen < len Then
                    maxlen = len
                    maxtext = item1
                End If

                i += 1
            Next

            cmb.DropDownStyle = ComboBoxStyle.DropDownList

            Dim selectindex As Integer = Integer.Parse(prop(4, 1))
            If selectindex <> "0" AndAlso selectindex <= cmb.Items.Count Then
                cmb.Text = cmb.GetItemText(cmb.Items(selectindex - 1))
            End If

            'コンボボックスの最適幅を取得する
            'ダミーでラベルを作り、最大長文字列に対する最適な幅を取得する
            Dim temp As Label = New Label()
            Set_Controls(temp, "temp", "", 0, 0, 0, 0)
            temp.Text = maxtext
            cmb.Width = temp.PreferredWidth + 30

            AddHandler cmb.KeyPress, AddressOf CAST.KeyPress

            TabIndexList.Add(cmb)

            pinf.PartsLabelList.Add(lbl)
            pinf.PartsInputList.Add(cmb)
            PartsList.Add(pinf)

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_COMBO")
        End Try

        Return row
    End Function

    '------------------------------------------------------------
    'ラジオボタン
    '------------------------------------------------------------
    Private Function Make_RADIO(ByVal row As Integer, ByVal prop As String(,), ByVal group As Integer) As Integer

        '定義例
        '-----------------------------------------------------------------------
        ' [ITEM]
        ' ITEM=RADIO, LABEL=出力区分, LABEL_STYLE=2, RADIO_DEF=出力区分,
        '      SELECT=0,TAB_INDEX=8, SQL_NO=12, GRID=2:2
        '
        ' [出力区分]
        ' ORIGIN=店番ソート,"ORDER BY MISE_NO"
        ' ORIGIN=非ソート,"ORDER BY RECORD_NO_K"
        ' ORIGIN=エラー分のみ,"AND YOBI5_K <> ' ' ORDER BY RECORD_NO_K"
        '-----------------------------------------------------------------------

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_RADIO", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            Dim grid As String() = Split(prop(5, 1), ":")         'GRID
            Dim index As String = prop(7, 1)                      'TAB_INDEX
            Dim name As String = "RADIO_" & group & "_"

            Dim pinf As PartsInformation = New PartsInformation()
            pinf.PartsLabelList = New List(Of Label)()
            pinf.PartsInputList = New List(Of Control)()
            pinf.PartsOptionList = New List(Of String())()
            pinf.PartsName = "RADIO"
            pinf.PartsFocus = prop(8, 1)                          'FOCUS
            pinf.PartsSqlno = New String(0) {prop(9, 1)}          'SQL_NO

            Dim lbl As Label = New Label()        'ラベル域表示用
            Set_Labels(lbl, name & "1", prop(1, 1), prop(2, 1), row, 0, 100)

            '部品固有情報読込み
            Dim strsplits As String() = CASTCommon.GetIniFileValues(PrtInf.PrtFileName, prop(3, 1), "ORIGIN")
            If strsplits Is Nothing Then
                LOG.Write_Err("FrmExtendPrint.Make_RADIO", String.Format(ERRLOG_003, PrtInf.PrtFileName, prop(3, 1)))
                Return -1
            End If

            Dim radiolist As New List(Of Control)
            Dim i As Integer = 1

            'ラジオボタン群をグループ化するため、パネル内にラジオボタンを配置して管理する
            Dim panel As Panel = New Panel
            Dim pwidth As Integer = Me.Width - RefP.Point_X_Input

            Set_Controls(panel, "RADIO_" & group, "", row, 0, pwidth, index)

            Dim radiocheck As Boolean = False
            For Each strvalue As String In strsplits

                Dim search As Integer = strvalue.IndexOf(",")
                If search < 1 Then
                    LOG.Write_Err("FrmExtendPrint.Make_RADIO", String.Format(ERRLOG_009, PrtInf.PrtFileName, prop(3, 1), i))
                    Return -1
                End If

                Dim item1 As String = strvalue.Substring(0, search).Trim
                Dim item2 As String = strvalue.Substring(search + 1).Trim

                If item2.StartsWith(ControlChars.Quote) = True Then   '文字列の先頭がダブルクォートの場合

                    Dim quoteindex As Integer = item2.Substring(1).IndexOf(ControlChars.Quote)
                    If quoteindex > 0 Then
                        'ダブルクォートを取り除く
                        item2 = item2.Substring(1, quoteindex)
                    Else
                        '文字列がダブルクォートで囲まれていない場合は定義エラー
                        LOG.Write_Err("FrmExtendPrint.Make_RADIO", String.Format(ERRLOG_009, PrtInf.PrtFileName, prop(3, 1), i))
                        Return -1
                    End If

                Else                                                  'それ以外
                    Dim items As String() = item2.Split(",")
                    item2 = items(0)
                End If

                Dim radio As RadioButton = New RadioButton()
                Set_Controls(radio, name & i + 1, item1, row, 0, 200, index)

                radio.AutoSize = False
                radio.Width = radio.PreferredSize.Width + 30

                If i.ToString = prop(6, 1) Then
                    radio.Checked = True
                    radiocheck = True
                End If

                If prop(4, 1) = "2" Then
                    radio.Font = New Font("ＭＳ ゴシック", 12, FontStyle.Bold)
                End If

                AddHandler radio.KeyPress, AddressOf CAST.KeyPress

                panel.Controls.Add(radio)
                pinf.PartsOptionList.Add(New String() {item1, item2})
                radiolist.Add(radio)
                i += 1
            Next

            If radiocheck = False Then
                CType(radiolist(0), RadioButton).Checked = True
            End If

            Dim res As Integer = Set_GRID(radiolist.ToArray, grid(0), grid(1))
            If res = -1 Then
                LOG.Write_Err("FrmExtendPrint.Make_RADIO", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", group, "GRID"))
                Return -1
            ElseIf res = -2 Then
                LOG.Write_Err("FrmExtendPrint.Make_RADIO", ERRLOG_012)
                Return -1
            End If

            panel.Size = New Size(panel.Width, Integer.Parse(grid(0)) * (RefP.Margin + 20))
            TabIndexList.Add(panel)

            pinf.PartsLabelList.Add(lbl)
            pinf.PartsInputList.Add(panel)
            PartsList.Add(pinf)
            row = row + grid(0)

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_RADIO")
        End Try

        Return row
    End Function

    '------------------------------------------------------------
    'チェックボックス
    '------------------------------------------------------------
    Private Function Make_CHECK(ByVal row As Integer, ByVal prop As String(,), ByVal group As Integer) As Integer

        '定義例
        '-----------------------------------------------------------------------
        ' [ITEM]
        ' ITEM=CHECK, LABEL=ソート順, LABEL_STYLE=2, CHECK_DEF=ソート順,
        '      CHECK_STYLE=1,OMIT=YES,SELECT=2,TAB_INDEX=9, SQL_NO=13, GRID=2:2
        '
        ' [ソート順]
        ' ORIGIN=生徒番号順, " ,G_MEIMAST.SEITO_NO_M "
        ' ORIGIN=通番順, ",G_MEIMAST.NENDO_M,G_MEIMAST.TUUBAN_M "
        ' ORIGIN=あいうえお順, ",SEITOMAST.SEITO_KNAME_O"
        ' ORIGIN=性別順, ",SEITOMAST.SEIBETU_O"
        ' JOIN=,
        '-----------------------------------------------------------------------

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_CHECK", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            Dim index As String = prop(8, 1)                  'TAB_INDEX
            Dim name As String = "CHECK_" & group & "_"
            Dim grid As String() = Split(prop(5, 1), ":")     'GRID

            '部品固有情報を取得する
            Dim join As String = GetIniFileValue(PrtInf.PrtFileName, prop(3, 1), "JOIN")
            If join = "err" Then
                join = ""
            End If

            Dim pinf As PartsInformation = New PartsInformation()
            pinf.PartsLabelList = New List(Of Label)()
            pinf.PartsInputList = New List(Of Control)()
            pinf.PartsOptionList = New List(Of String())()
            pinf.PartsName = "CHECK"
            pinf.PartsFocus = prop(9, 1)                     'FOCUS
            pinf.PartsOption = join.Trim                     '連結文字
            pinf.PartsSqlno = New String(0) {prop(10, 1)}    'SQL_NO

            pinf.PartsFlag = True                            'チェック項目省略可否
            If prop(6, 1) = "NO" Then                        'OMIT
                pinf.PartsFlag = False
            End If

            Dim lbl As Label = New Label()   'ラベル域表示用
            Set_Labels(lbl, name & "1", prop(1, 1), prop(2, 1), row, 0, 100)
            pinf.PartsLabelList.Add(lbl)

            '部品固有情報読込み
            Dim strsplits As String()
            strsplits = CASTCommon.GetIniFileValues(PrtInf.PrtFileName, prop(3, 1), "ORIGIN")
            If strsplits Is Nothing Then
                LOG.Write_Err("FrmExtendPrint.Make_CHECK", String.Format(ERRLOG_003, PrtInf.PrtFileName, prop(3, 1)))
                Return -1
            End If

            Dim i As Integer = 1
            Dim chklist As New List(Of CheckBox)

            For Each strvalue As String In strsplits

                Dim search As Integer = strvalue.IndexOf(",")
                If search < 1 Then
                    LOG.Write_Err("FrmExtendPrint.Make_CHECK", String.Format(ERRLOG_009, PrtInf.PrtFileName, prop(3, 1), i))
                    Return -1
                End If

                Dim item1 As String = strvalue.Substring(0, search).Trim
                Dim item2 As String = strvalue.Substring(search + 1).Trim

                If item2.StartsWith(ControlChars.Quote) = True Then   '文字列の先頭がダブルクォートの場合

                    Dim quoteindex As Integer = item2.Substring(1).IndexOf(ControlChars.Quote)
                    If quoteindex > 0 Then
                        'ダブルクォートを取り除く
                        item2 = item2.Substring(1, quoteindex)
                    Else
                        '文字列がダブルクォートで囲まれていない場合は定義エラー
                        LOG.Write_Err("FrmExtendPrint.Make_CHECK", String.Format(ERRLOG_009, PrtInf.PrtFileName, prop(3, 1), i))
                        Return -1
                    End If

                Else                                                  'それ以外
                    Dim items As String() = item2.Split(",")
                    item2 = items(0)
                End If

                Dim checkbox As CheckBox = New CheckBox()

                Set_Controls(checkbox, name & i + 1, item1, row, 0, 10, index)

                checkbox.AutoSize = False
                checkbox.Width = checkbox.PreferredSize.Width + 30

                If prop(4, 1) = "2" Then                              'CHECK_STYLE
                    checkbox.Font = New Font("ＭＳ ゴシック", 12, FontStyle.Bold)
                End If

                AddHandler checkbox.KeyPress, AddressOf CAST.KeyPress
                TabIndexList.Add(checkbox)

                pinf.PartsOptionList.Add(New String() {item1, item2})
                chklist.Add(checkbox)

                pinf.PartsInputList.Add(checkbox)
                i += 1
            Next

            Dim checked As String() = Split(prop(7, 1), ":")         'SELECT
            For j As Integer = 0 To checked.Length - 1
                Dim checknum As Integer = Integer.Parse(checked(j))
                If checknum <> 0 AndAlso checknum <= chklist.Count Then
                    chklist(checknum - 1).Checked = True
                End If
            Next

            Dim res As Integer = Set_GRID(chklist.ToArray, grid(0), grid(1))
            If res = -1 Then
                LOG.Write_Err("FrmExtendPrint.Make_CHECK", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", group, "GRID"))
                Return -1
            ElseIf res = -2 Then
                LOG.Write_Err("FrmExtendPrint.Make_CHECK", ERRLOG_012)
                Return -1
            End If

            PartsList.Add(pinf)
            row = row + grid(0)

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_CHECK")
        End Try

        Return row
    End Function

    '------------------------------------------------------------
    'テキストボックス
    '------------------------------------------------------------
    Private Function Make_TEXT(ByVal row As Integer, ByVal prop As String(,), ByVal group As Integer) As Integer

        '定義例
        '-----------------------------------------------------------------------
        ' [ITEM]
        ' ITEM=TEXT, LABEL=出力区分, LABEL_STYLE=2, IN_TYPE=2,IN_SIZE=10,
        '      IN_CORRECT=2,INIT_TEXT=TEST,TAB_INDEX=10, SQL_NO=14
        '-----------------------------------------------------------------------

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_TEXT", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            Dim index As String = prop(7, 1)                     'TAB_INDEX
            Dim name As String = "TEXT_" & group & "_"

            Dim pinf As PartsInformation = New PartsInformation()
            pinf.PartsLabelList = New List(Of Label)()
            pinf.PartsInputList = New List(Of Control)()
            pinf.PartsName = "TEXT"
            pinf.PartsFocus = prop(8, 1)                         'FOCUS
            pinf.PartsSqlno = New String(0) {prop(9, 1)}         'SQL_NO
            pinf.PartsOption = prop(5, 1)                        'IN_CORRECT

            Dim lbl As Label = New Label()
            Dim txt As TextBox = New TextBox()

            Set_Labels(lbl, name & "1", prop(1, 1), prop(2, 1), row, 0, 100)
            Set_Controls(txt, name & "2", "", row, 0, 100, index)
            row += 1

            Dim max As Integer = Integer.Parse(prop(4, 1))      'IN_SIZE
            Dim maxstr As String = ""

            '幅を設定する
            txt.MaxLength = max
            txt.Text = maxstr.PadLeft(max, "W")
            txt.Width = txt.PreferredSize.Width
            txt.Text = ""
            AddHandler txt.Validating, AddressOf TextBox_Validating

            Dim inittext As String = prop(6, 1)       'INIT_TEXT
            If inittext.Length > 0 Then
                If inittext.Length > max Then
                    inittext = inittext.Substring(0, max)
                End If
                txt.Text = inittext
                'If System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(txt.Text) <> Len(txt.Text) Then
                '    LOG.Write_Err("FrmExtendPrint.Make_TEXT", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", group, "INIT_TEXT"))
                '    Return -1
                'End If
            End If

            txt.ImeMode = Windows.Forms.ImeMode.Disable
            If prop(3, 1) = "1" Then                            'IN_TYPE
                AddHandler txt.KeyPress, AddressOf CASTx01.KeyPress
                pinf.PartsFlag = True
                Dim res As Integer
                If Integer.TryParse(txt.Text, res) = False Then
                    txt.Text = ""
                End If
            Else
                AddHandler txt.KeyPress, AddressOf CAST.KeyPress
                pinf.PartsFlag = False
                '全角文字が入力されたら削除する
                If System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(txt.Text) <> Len(txt.Text) Then
                    txt.Text = ""
                End If
            End If

            If txt.Text.Length > 0 Then
                If pinf.PartsOption = "1" Then
                    '前0パディング
                    txt.Text = txt.Text.PadLeft(txt.MaxLength, "0")
                ElseIf pinf.PartsOption = "2" Then
                    '後空白パディング
                    txt.Text = txt.Text.PadRight(txt.MaxLength)
                End If
            End If

            TabIndexList.Add(txt)

            pinf.PartsLabelList.Add(lbl)
            pinf.PartsInputList.Add(txt)
            PartsList.Add(pinf)

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_TEXT")
        End Try

        Return row
    End Function

    '------------------------------------------------------------
    'ラベル
    '------------------------------------------------------------
    Private Function Make_LABEL(ByVal row As Integer, ByVal prop As String(,), ByVal group As Integer) As Integer

        '定義例
        '-----------------------------------------------------------------------
        ' [ITEM]
        ' ITEM=LABEL, LABEL=ラベル, LABEL_STYLE=2
        '-----------------------------------------------------------------------

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Make_LABEL", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            Dim index As String = Nothing
            Dim name As String = "LABEL_" & group & "_"

            Dim pinf As PartsInformation = New PartsInformation()
            pinf.PartsLabelList = New List(Of Label)()
            pinf.PartsInputList = New List(Of Control)()
            pinf.PartsName = "LABEL"

            Dim lbl As Label = New Label()
            Set_Labels(lbl, name & "1", prop(1, 1), prop(2, 1), row, 0, 400)
            row += 1

            pinf.PartsLabelList.Add(lbl)
            PartsList.Add(pinf)

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Make_LABEL")
        End Try

        Return row
    End Function
#End Region

#Region "内部処理"

    '------------------------------------------------------------
    ' 機能   ： 部品定義情報を配列に格納する
    ' 引数   ： propdef      - 部品情報を格納する配列
    '           itemproplist - 読み込んだ部品情報
    '           itemsqllist  - SQL番号を格納する配列
    '           itemtablist  - タブ順を格納する配列
    '           itemcount    - 読み込んだ部品の数
    ' 戻り値 ： True - 正常 ， False - 異常
    '------------------------------------------------------------
    Private Function Set_Prop(ByRef propdef As String(,), ByVal itemproplist As List(Of String()), _
                              ByRef itemsqllist As List(Of String), ByRef itemtablist As List(Of String), _
                              ByVal itemcount As Integer) As Boolean

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Set_Prop", PrtInf.PrtID & "_" & PrtInf.PrtName)
        Try

            For i As Integer = 1 To itemproplist.Count - 1

                Dim pname As String = itemproplist(i)(0)
                Dim pprop As String = itemproplist(i)(1)
                Dim res As Boolean = True

                If pprop Is Nothing Then
                    pprop = ""
                End If

                '=============================================================
                '指定値が範囲外の場合デフォルト値になるプロパティ（エラーなし）
                '=============================================================
                If pname = "LABEL_STYLE" OrElse _
                  (pname = "LABEL2_STYLE" AndAlso (itemproplist(0)(0) = "COMBO_CODE" OrElse itemproplist(0)(0) = "COMBO_SQL")) OrElse _
                  (pname = "LABEL3_STYLE" AndAlso itemproplist(0)(0) = "COMBO_SQL") OrElse _
                  (pname = "CODE_TYPE" AndAlso itemproplist(0)(0) = "CODE") OrElse _
                  (pname = "IN_TYPE" AndAlso itemproplist(0)(0) = "TEXT") OrElse _
                  (pname = "RADIO_STYLE" AndAlso itemproplist(0)(0) = "RADIO") OrElse _
                  (pname = "DATE_TYPE" AndAlso (itemproplist(0)(0) = "DATE" OrElse itemproplist(0)(0) = "RANGE_DATE")) OrElse _
                  (pname = "CHECK_STYLE" AndAlso itemproplist(0)(0) = "CHECK") Then

                    If Check_Num(pprop, 1, 2) = False Then
                        Continue For
                    End If

                ElseIf pname = "FOCUS" AndAlso itemproplist(0)(0) <> "LABEL" Then

                    If Check_Num(pprop, 0, 1) = False Then
                        Continue For
                    End If

                ElseIf (pname = "IN_CORRECT" AndAlso itemproplist(0)(0) = "TEXT") OrElse _
                       (pname = "INIT_DATE" AndAlso itemproplist(0)(0) = "DATE") OrElse _
                       (pname = "INIT_DATE1" AndAlso itemproplist(0)(0) = "RANGE_DATE") OrElse _
                       (pname = "INIT_DATE2" AndAlso itemproplist(0)(0) = "RANGE_DATE") Then

                    If Check_Num(pprop, 0, 2) = False Then
                        Continue For
                    End If

                ElseIf (pname = "IN_SIZE" AndAlso itemproplist(0)(0) = "RANGE_CODE") OrElse _
                       (pname = "IN_SIZE" AndAlso itemproplist(0)(0) = "COMBO_SQL") Then

                    If Check_Num(pprop, 0, 0) = False Then
                        Continue For
                    End If

                    If Integer.Parse(pprop) < 1 Then
                        Continue For
                    End If

                ElseIf (pname = "ALL9" AndAlso itemproplist(0)(0) = "COMBO_CODE") OrElse _
                       (pname = "OMIT" AndAlso itemproplist(0)(0) = "CHECK") Then

                    If pprop <> "NO" Then
                        Continue For
                    End If

                ElseIf (pname = "SELECT" AndAlso _
                       (itemproplist(0)(0) = "COMBO" OrElse itemproplist(0)(0) = "RADIO" OrElse itemproplist(0)(0) = "CHECK")) Then

                    If itemproplist(0)(0) = "CHECK" Then
                        Dim checkres As Boolean = True
                        Dim numsplit As String() = Split(pprop, ":")

                        For Each num As String In numsplit
                            If Check_Num(num, 0, 0) = False Then
                                checkres = False
                                Exit For
                            End If
                            If Integer.Parse(num) < 0 Then
                                checkres = False
                                Exit For
                            End If
                        Next

                        If checkres = False Then
                            Continue For
                        End If

                    Else
                        If Check_Num(pprop, 0, 0) = False Then
                            Continue For
                        End If

                        If Integer.Parse(pprop) < 0 Then
                            Continue For
                        End If
                    End If

                    '==========================================
                    '指定値が範囲外の場合エラーになるプロパティ
                    '==========================================
                ElseIf pname = "DB_KIND" AndAlso itemproplist(0)(0) = "COMBO_CODE" Then
                    res = Check_Num(pprop, 1, 3)

                ElseIf pname = "IN_SIZE" AndAlso itemproplist(0)(0) = "TEXT" Then

                    If Check_Num(pprop, 0, 0) = True Then
                        If Integer.Parse(pprop) < 1 Then
                            res = False
                        End If
                    Else
                        res = False
                    End If

                ElseIf pname = "TAB_INDEX" AndAlso itemproplist(0)(0) <> "LABEL" Then

                    If Check_Num(pprop, 0, 0) = True Then
                        If Integer.Parse(pprop) < 0 Then
                            res = False
                        End If

                        If itemtablist.Contains(pprop) = True Then
                            LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_007, PrtInf.PrtFileName, "ITEMS", pname))
                            Return False
                        End If

                        itemtablist.Add(pprop)
                    Else
                        res = False
                    End If

                ElseIf (pname = "BUSINESS_DAY" AndAlso itemproplist(0)(0) = "DATE") OrElse _
                       (pname = "BUSINESS_DAY1" AndAlso itemproplist(0)(0) = "RANGE_DATE") OrElse _
                       (pname = "BUSINESS_DAY2" AndAlso itemproplist(0)(0) = "RANGE_DATE") Then

                    'NOP
                    'BUSINESS_DAYの値判定結果はINIT_DATEの値に依存するため
                    '値チェックは後から行う

                ElseIf (pname = "GRID" AndAlso _
                       (itemproplist(0)(0) = "RADIO" OrElse itemproplist(0)(0) = "CHECK")) Then

                    Dim num As String()

                    num = Split(pprop, ":")
                    If num.Length = 2 Then
                        '0以下の場合エラー
                        If Check_Num(num(0), 0, 0) = True AndAlso _
                           Check_Num(num(1), 0, 0) = True Then
                            If (Integer.Parse(num(0)) < 1 OrElse _
                                Integer.Parse(num(1)) < 1) Then
                                res = False
                            End If
                        Else
                            res = False
                        End If
                    Else
                        res = False
                    End If

                ElseIf pname = "SQL_NO" AndAlso itemproplist(0)(0) <> "LABEL" Then

                    Dim num As String()
                    If itemproplist(0)(0) = "RANGE_CODE" OrElse _
                       itemproplist(0)(0) = "RANGE_DATE" Then

                        num = Split(pprop, ":")
                        If num.Length = 2 Then
                            'SQL置換え番号:0以下の場合エラー
                            If Check_Num(num(0), 0, 0) = True AndAlso _
                               Check_Num(num(1), 0, 0) = True Then

                                If (Integer.Parse(num(0)) < 1 OrElse _
                                    Integer.Parse(num(1)) < 1) Then
                                    res = False
                                End If

                                If itemsqllist.Contains(num(0)) = True Then
                                    LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_007, PrtInf.PrtFileName, "ITEMS", pname))
                                    Return False
                                End If

                                itemsqllist.Add(num(0))

                                If itemsqllist.Contains(num(1)) = True Then
                                    LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_007, PrtInf.PrtFileName, "ITEMS", pname))
                                    Return False
                                End If

                                itemsqllist.Add(num(1))

                            Else
                                res = False
                            End If
                        Else
                            res = False
                        End If


                    ElseIf itemproplist(0)(0) = "COMBO_CODE" OrElse _
                           itemproplist(0)(0) = "CODE" Then

                        num = Split(pprop, ":")
                        For j As Integer = 0 To num.Length - 1
                            If Check_Num(num(j), 0, 0) = True Then
                                If Integer.Parse(num(j)) < 1 Then
                                    res = False
                                End If
                                If itemsqllist.Contains(num(j)) = True Then
                                    LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_007, PrtInf.PrtFileName, "ITEMS", pname))
                                    Return False
                                End If
                                itemsqllist.Add(num(j))
                            Else
                                res = False
                            End If
                        Next

                    Else

                        If Check_Num(pprop, 0, 0) = True Then
                            If Integer.Parse(pprop) < 0 Then
                                res = False
                            End If

                            If itemsqllist.Contains(pprop) = True Then
                                LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_007, PrtInf.PrtFileName, "ITEMS", pname))
                                Return False
                            End If

                            itemsqllist.Add(pprop)
                        Else
                            res = False
                        End If

                    End If

                ElseIf pname = "LABEL" AndAlso itemproplist(0)(0) = "RADIO" Then
                    'NOP

                ElseIf pname = "LABEL" OrElse _
                      (pname = "LABEL2" AndAlso (itemproplist(0)(0) = "COMBO_CODE" OrElse itemproplist(0)(0) = "COMBO_SQL")) OrElse _
                      (pname = "LABEL3" AndAlso itemproplist(0)(0) = "COMBO_SQL") OrElse _
                      (pname = "COMBO_DEF" AndAlso (itemproplist(0)(0) = "COMBO" OrElse itemproplist(0)(0) = "COMBO_SQL")) OrElse _
                      (pname = "RADIO_DEF" AndAlso itemproplist(0)(0) = "RADIO") OrElse _
                      (pname = "CHECK_DEF" AndAlso itemproplist(0)(0) = "CHECK") Then

                    If pprop.Length = 0 Then
                        res = False
                    End If

                ElseIf pname = "INIT_TEXT" AndAlso itemproplist(0)(0) = "TEXT" Then

                    'NOP

                    '==========================================
                    '範囲チェックなしのプロパティ
                    '==========================================
                ElseIf pname = "WHERE" AndAlso itemproplist(0)(0) = "COMBO_CODE" Then
                    'NOP
                Else
                    LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_005, PrtInf.PrtFileName, "ITEMS", itemcount, pname))
                    Return False
                End If

                    If res = False Then
                        LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, pname))
                        Return False
                    End If

                    For j As Integer = 1 To UBound(propdef)
                        If propdef(j, 0) = pname Then
                            propdef(j, 1) = pprop
                            Exit For
                        End If
                    Next
            Next

            'プロパティ必須入力チェック
            For i As Integer = 1 To UBound(propdef)
                If propdef(i, 1) = "" Then

                    If propdef(i, 0) = "INIT_TEXT" OrElse _
                       propdef(i, 0) = "WHERE" OrElse _
                       propdef(i, 0) = "BUSINESS_DAY" OrElse _
                       propdef(i, 0) = "BUSINESS_DAY1" OrElse _
                       propdef(i, 0) = "BUSINESS_DAY2" OrElse _
                      (propdef(i, 0) = "LABEL" AndAlso itemproplist(0)(0) = "RADIO") Then
                        Continue For
                    End If

                    LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_008, PrtInf.PrtFileName, "ITEMS", itemcount, propdef(0, 1), propdef(i, 0)))

                    Return False
                End If
            Next

            If (propdef(0, 1) = "COMBO_CODE") OrElse _
               (propdef(0, 1) = "CODE") Then
                Dim res As Boolean = True
                Dim sp As Integer = 0
                Select Case propdef(0, 1)
                    Case "COMBO_CODE"
                        sp = Split(propdef(10, 1), ":").Length
                        If (propdef(1, 1) = "1" OrElse propdef(1, 1) = "2") AndAlso sp <> 2 Then
                            res = False
                        ElseIf propdef(1, 1) = "3" AndAlso sp <> 1 Then
                            res = False
                        End If
                    Case "CODE"
                        sp = Split(propdef(6, 1), ":").Length
                        If propdef(3, 1) = "1" AndAlso sp <> 1 Then
                            res = False
                        ElseIf propdef(3, 1) = "2" AndAlso sp <> 2 Then
                            res = False
                        End If
                    Case Else
                        'NOP
                End Select

                If res = False Then
                    LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "SQL_NO"))
                    Return False
                End If
            End If

            If (propdef(0, 1) = "DATE" AndAlso propdef(4, 1) = "2") OrElse _
               (propdef(0, 1) = "RANGE_DATE" AndAlso propdef(4, 1) = "2") OrElse _
               (propdef(0, 1) = "RANGE_DATE" AndAlso propdef(5, 1) = "2") Then

                Select Case propdef(0, 1)
                    Case "DATE"
                        If propdef(5, 1) = "" Then
                            LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_008, PrtInf.PrtFileName, "ITEMS", itemcount, propdef(0, 1), "BUSINESS_DAY"))
                            Return False
                        End If
                        If Check_Num(propdef(5, 1), 0, 0) = False Then
                            LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY"))
                            Return False
                        End If
                        If propdef(5, 1) = "0" Then
                            LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY"))
                            Return False
                        End If

                    Case "RANGE_DATE"
                        If propdef(4, 1) = "2" Then
                            If propdef(6, 1) = "" Then
                                LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_008, PrtInf.PrtFileName, "ITEMS", itemcount, propdef(0, 1), "BUSINESS_DAY1"))
                                Return False
                            End If
                            If Check_Num(propdef(6, 1), 0, 0) = False Then
                                LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY1"))
                                Return False
                            End If
                            If propdef(6, 1) = "0" Then
                                LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY1"))
                                Return False
                            End If
                        End If

                        If propdef(5, 1) = "2" Then
                            If propdef(7, 1) = "" Then
                                LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_008, PrtInf.PrtFileName, "ITEMS", itemcount, propdef(0, 1), "BUSINESS_DAY2"))
                                Return False
                            End If
                            If Check_Num(propdef(7, 1), 0, 0) = False Then
                                LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY2"))
                                Return False
                            End If
                            If propdef(7, 1) = "0" Then
                                LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY2"))
                                Return False
                            End If
                        End If
                    Case Else
                        'NOP
                End Select
            End If

            ' 2016/07/13 ＦＪＨ）小嶋 ADD 【PG】拡張印刷仕様変更(RSV2<小浜信金>) ---------------- START
            ' 日付関連部品の初期値に、暦日のn営業日前/後を設定可能とする。
            If (propdef(0, 1) = "DATE" AndAlso propdef(4, 1) = "1") OrElse _
               (propdef(0, 1) = "RANGE_DATE" AndAlso propdef(4, 1) = "1") OrElse _
               (propdef(0, 1) = "RANGE_DATE" AndAlso propdef(5, 1) = "1") Then

                Select Case propdef(0, 1)
                    Case "DATE"
                        ' BUSSINESS_DAYのチェック
                        If propdef(5, 1) = "" Then
                            ' 定義無しの場合正常復帰
                            Return True
                        End If
                        If Check_Num(propdef(5, 1), 0, 0) = False Then
                            ' 数値以外の場合エラー
                            LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY"))
                            Return False
                        End If
                        If propdef(5, 1) = "0" Then
                            ' 0の場合エラー
                            LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY"))
                            Return False
                        End If

                    Case "RANGE_DATE"
                        If propdef(4, 1) = "1" Then
                            If propdef(6, 1) = "" Then
                                ' 定義無しの場合正常復帰
                                Return True
                            End If
                            If Check_Num(propdef(6, 1), 0, 0) = False Then
                                ' 数値以外の場合エラー
                                LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY1"))
                                Return False
                            End If
                            If propdef(6, 1) = "0" Then
                                ' 0の場合エラー
                                LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY1"))
                                Return False
                            End If
                        End If

                        If propdef(5, 1) = "1" Then
                            If propdef(7, 1) = "" Then
                                ' 定義無しの場合正常復帰
                                Return True
                            End If
                            If Check_Num(propdef(7, 1), 0, 0) = False Then
                                ' 数値以外の場合エラー
                                LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY2"))
                                Return False
                            End If
                            If propdef(7, 1) = "0" Then
                                ' 0の場合エラー
                                LOG.Write_Err("FrmExtendPrint.Set_Prop", String.Format(ERRLOG_006, PrtInf.PrtFileName, "ITEMS", itemcount, "BUSINESS_DAY2"))
                                Return False
                            End If
                        End If
                    Case Else
                        'NOP
                End Select
            End If
            ' 2016/07/13 ＦＪＨ）小嶋 ADD 【PG】拡張印刷仕様変更(RSV2<小浜信金>) ------------------ END

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Set_Prop")
        End Try

        Return True
    End Function

    '------------------------------------------------------------
    ' 機能   ： SQL置換文字列配列を設定する
    ' 引数   ： isall9 - ALL9判定フラグ
    ' 戻り値 ： SQL置換文字列配列
    '------------------------------------------------------------
    Private Function Set_Param(ByRef isall9 As Boolean) As String()

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Set_Params", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Dim plist As String()

        Try

            Dim pnumlist As New List(Of Integer)
            Dim pvaluelist As New List(Of String)
            Dim maxnum As Integer = 0

            For i As Integer = 0 To PartsList.Count - 1
                Dim param1 As String = ""
                Dim param2 As String = ""
                Dim name As String = ""
                Dim skip As Boolean = False

                If PartsList(i).PartsName = "LABEL" Then
                    Continue For
                End If

                If PartsList(i).PartsName = "COMBO_CODE" Then
                    param1 = PartsList(i).PartsInputList(2).Text  '主コード
                    name = PartsList(i).PartsLabelList(1).Text    '部品ラベル名

                    Dim all9 As String = ""
                    Dim cmpstr As String = ""
                    '入力チェック
                    If param1.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(2).Focus()
                        Return Nothing
                    End If

                    If PartsList(i).PartsInputList.Count = 5 Then  '自振、総振検索の場合
                        param2 = PartsList(i).PartsInputList(3).Text '副コード
                        If param2.Length = 0 Then
                            MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            PartsList(i).PartsInputList(3).Focus()
                            Return Nothing
                        End If
                        all9 = "999999999999"
                        cmpstr = param1 & param2
                    Else                                          '学校検索の場合
                        all9 = "9999999999"
                        cmpstr = param1
                    End If

                    'ALL9判定
                    If PartsList(i).PartsFlag = True AndAlso _
                       cmpstr = all9 Then
                        skip = True
                        isall9 = True

                        '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                        Dim pinf As PartsInformation = PartsList(i)
                        pinf.CodeErrFlg = False   ' 入力コードエラーフラグOFF
                        PartsList(i) = pinf
                        '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    End If

                    '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    ' 入力コードエラーフラグがONの場合
                    If PartsList(i).CodeErrFlg = True Then
                        Dim cmb As ComboBox = CType(PartsList(i).PartsInputList(0), ComboBox)
                        Dim dbkind As String = cmb.Tag.ToString
                        'dbkind 1（自振）
                        '       2（総振)
                        '       3（学校）
                        If dbkind = "1" Then
                            MessageBox.Show(P_MSG0008W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                        ElseIf dbkind = "2" Then
                            MessageBox.Show(P_MSG0008W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                        ElseIf dbkind = "3" Then
                            MessageBox.Show(P_MSG0009W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If

                        PartsList(i).PartsInputList(2).Focus()
                        Return Nothing
                    End If
                    '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***

                ElseIf PartsList(i).PartsName = "COMBO_SQL" Then
                    param1 = PartsList(i).PartsInputList(1).Text   '入力コード
                    name = PartsList(i).PartsLabelList(1).Text     '部品ラベル名

                    '入力チェック
                    If param1.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(1).Focus()
                        Return Nothing
                    End If

                    '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    ' 入力コードエラーフラグがONの場合
                    If PartsList(i).CodeErrFlg = True Then
                        MessageBox.Show(String.Format(P_MSG0003W, name, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(1).Focus()
                        Return Nothing
                    End If
                    '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***

                ElseIf PartsList(i).PartsName = "CODE" Then

                    param1 = PartsList(i).PartsInputList(0).Text   '主コード
                    name = PartsList(i).PartsLabelList(0).Text     '部品ラベル名

                    '入力チェック
                    If param1.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(0).Focus()
                        Return Nothing
                    End If

                    If PartsList(i).PartsInputList.Count = 2 Then  '副コード入力有の場合
                        param2 = PartsList(i).PartsInputList(1).Text '副コード
                        '入力チェック
                        If param2.Length = 0 Then
                            MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            PartsList(i).PartsInputList(1).Focus()
                            Return Nothing
                        End If
                    End If

                ElseIf PartsList(i).PartsName = "RANGE_CODE" Then

                    param1 = PartsList(i).PartsInputList(0).Text  '開始コード
                    param2 = PartsList(i).PartsInputList(1).Text  '終了コード
                    name = PartsList(i).PartsLabelList(0).Text    '部品ラベル名

                    '入力チェック
                    If param1.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(0).Focus()
                        Return Nothing
                    End If
                    If param2.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(1).Focus()
                        Return Nothing
                    End If

                    '値前後チェック
                    If param1 > param2 Then
                        MessageBox.Show(String.Format(P_MSG0006W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(0).Focus()
                        Return Nothing
                    End If

                ElseIf PartsList(i).PartsName = "DATE" Then
                    name = PartsList(i).PartsLabelList(0).Text

                    Dim y As String = PartsList(i).PartsInputList(0).Text  '年
                    Dim m As String = PartsList(i).PartsInputList(1).Text  '月

                    '入力チェック
                    If y.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(0).Focus()
                        Return Nothing
                    End If
                    If m.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(1).Focus()
                        Return Nothing
                    End If

                    '月範囲チェック
                    If GCom.NzInt(m) < 1 OrElse _
                       GCom.NzInt(m) > 12 Then
                        MessageBox.Show(P_MSG0011W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(1).Focus()
                        Return Nothing
                    End If

                    If PartsList(i).PartsInputList.Count = 2 Then  '年月の場合
                        param1 = y & m

                    Else                                           '年月日の場合
                        Dim d As String = PartsList(i).PartsInputList(2).Text

                        If d.Length = 0 Then
                            MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            PartsList(i).PartsInputList(2).Focus()
                            Return Nothing
                        End If

                        '日付範囲チェック
                        If GCom.NzInt(d) < 1 OrElse _
                           GCom.NzInt(d) > 31 Then
                            MessageBox.Show(P_MSG0012W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            PartsList(i).PartsInputList(2).Focus()
                            Return Nothing
                        End If

                        '日付整合性チェック
                        Dim checkdate As String = y & "/" & m & "/" & d
                        If Not Information.IsDate(checkdate) Then
                            MessageBox.Show(P_MSG0010W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            PartsList(i).PartsInputList(0).Focus()
                            Return Nothing
                        End If

                        param1 = y & m & d
                    End If

                ElseIf PartsList(i).PartsName = "RANGE_DATE" Then
                    name = PartsList(i).PartsLabelList(0).Text

                    Dim y1 As TextBox
                    Dim m1 As TextBox
                    Dim d1 As TextBox
                    Dim y2 As TextBox
                    Dim m2 As TextBox
                    Dim d2 As TextBox

                    Dim datetype As Integer = 2
                    If PartsList(i).PartsInputList.Count = 6 Then
                        datetype = 1
                    End If

                    If datetype = 2 Then  '年月の場合
                        y1 = PartsList(i).PartsInputList(0)
                        m1 = PartsList(i).PartsInputList(1)
                        d1 = Nothing
                        y2 = PartsList(i).PartsInputList(2)
                        m2 = PartsList(i).PartsInputList(3)
                        d2 = Nothing
                    Else                  '年月日の場合
                        y1 = PartsList(i).PartsInputList(0)
                        m1 = PartsList(i).PartsInputList(1)
                        d1 = PartsList(i).PartsInputList(2)
                        y2 = PartsList(i).PartsInputList(3)
                        m2 = PartsList(i).PartsInputList(4)
                        d2 = PartsList(i).PartsInputList(5)
                    End If

                    '入力チェック
                    If y1.Text.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        y1.Focus()
                        Return Nothing
                    End If
                    If m1.Text.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        m1.Focus()
                        Return Nothing
                    End If

                    If datetype = 1 AndAlso d1.Text.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        d1.Focus()
                        Return Nothing
                    End If

                    If y2.Text.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        y2.Focus()
                        Return Nothing
                    End If
                    If m2.Text.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        m2.Focus()
                        Return Nothing
                    End If

                    If datetype = 1 AndAlso d2.Text.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0004W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        d2.Focus()
                        Return Nothing
                    End If

                    '月範囲チェック
                    If GCom.NzInt(m1.Text) < 1 OrElse _
                       GCom.NzInt(m1.Text) > 12 Then
                        MessageBox.Show(P_MSG0011W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        m1.Focus()
                        Return Nothing
                    End If
                    If GCom.NzInt(m2.Text) < 1 OrElse _
                       GCom.NzInt(m2.Text) > 12 Then
                        MessageBox.Show(P_MSG0011W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        m2.Focus()
                        Return Nothing
                    End If

                    If datetype = 2 Then   '年月の場合
                        param1 = y1.Text & m1.Text
                        param2 = y2.Text & m2.Text

                    Else                   '年月日の場合

                        '日付範囲チェック
                        If GCom.NzInt(d1.Text) < 1 OrElse _
                           GCom.NzInt(d1.Text) > 31 Then
                            MessageBox.Show(P_MSG0012W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            d1.Focus()
                            Return Nothing
                        End If
                        If GCom.NzInt(d2.Text) < 1 OrElse _
                           GCom.NzInt(d2.Text) > 31 Then
                            MessageBox.Show(P_MSG0012W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            d2.Focus()
                            Return Nothing
                        End If

                        '日付整合性チェック
                        Dim checkdate1 As String = y1.Text & "/" & m1.Text & "/" & d1.Text
                        If Not Information.IsDate(checkdate1) Then
                            MessageBox.Show(P_MSG0010W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            d1.Focus()
                            Return Nothing
                        End If
                        Dim checkdate2 As String = y2.Text & "/" & m2.Text & "/" & d2.Text
                        If Not Information.IsDate(checkdate2) Then
                            MessageBox.Show(P_MSG0010W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            d2.Focus()
                            Return Nothing
                        End If

                        param1 = y1.Text & m1.Text & d1.Text
                        param2 = y2.Text & m2.Text & d2.Text

                    End If

                    '日付前後チェック
                    If param1 > param2 Then
                        MessageBox.Show(P_MSG0013W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        y1.Focus()
                        Return Nothing
                    End If

                ElseIf PartsList(i).PartsName = "RADIO" Then
                    Dim panel As Panel = CType(PartsList(i).PartsInputList(0), Panel)
                    For index As Integer = 0 To panel.Controls.Count - 1
                        Dim radio As RadioButton = CType(panel.Controls(index), RadioButton)
                        If radio.Checked = True Then
                            param1 = Set_ItemList(PartsList(i).PartsOptionList, radio.Text)
                            Exit For
                        End If
                    Next
                ElseIf PartsList(i).PartsName = "COMBO" Then
                    name = PartsList(i).PartsLabelList(0).Text

                    If PartsList(i).PartsInputList(0).Text.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0005W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(0).Focus()
                        Return Nothing
                    End If

                    param1 = Set_ItemList(PartsList(i).PartsOptionList, PartsList(i).PartsInputList(0).Text)

                ElseIf PartsList(i).PartsName = "CHECK" Then
                    name = PartsList(i).PartsLabelList(0).Text

                    Dim join As String = PartsList(i).PartsOption
                    For j As Integer = 0 To PartsList(i).PartsInputList.Count - 1
                        Dim chk As CheckBox = CType(PartsList(i).PartsInputList(j), CheckBox)
                        If chk.Checked = True Then
                            param1 = param1 & Set_ItemList(PartsList(i).PartsOptionList, chk.Text)
                        End If
                    Next

                    '連結文字削除
                    If join.Length > 0 AndAlso param1.StartsWith(join) Then
                        param1 = param1.Substring(join.Length)
                    End If

                    '省略不可チェック
                    If PartsList(i).PartsFlag = False AndAlso _
                       param1.Length = 0 Then
                        MessageBox.Show(String.Format(P_MSG0005W, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        PartsList(i).PartsInputList(0).Focus()
                        Return Nothing
                    End If

                ElseIf PartsList(i).PartsName = "TEXT" Then
                    'TEXTの入力は省略可
                    param1 = PartsList(i).PartsInputList(0).Text
                End If

                If skip = False Then
                    pvaluelist.Add(param1)
                    pnumlist.Add(PartsList(i).PartsSqlno(0))

                    If param2.Length > 0 Then
                        pvaluelist.Add(param2)
                        pnumlist.Add(PartsList(i).PartsSqlno(1))
                    End If
                End If

                'SQL置換番号の最大値を取得
                For j As Integer = 0 To PartsList(i).PartsSqlno.Length - 1
                    Dim pnum As Integer = Integer.Parse(PartsList(i).PartsSqlno(j))
                    If maxnum < pnum Then
                        maxnum = pnum
                    End If
                Next
            Next

            'SQL置換番号の最大値の長さを持つ配列を作成する
            plist = New String(maxnum - 1) {}
            'SQL置換文字列配列に格納
            For i As Integer = 0 To pnumlist.Count - 1
                plist(pnumlist(i) - 1) = pvaluelist(i)
            Next

        Finally
            LOG.Write_Exit3(sw, "FrmExtendPrint.Set_Param")
        End Try

        Return plist
    End Function

    '------------------------------------------------------------
    ' 機能   ： コントロールのプロパティを設定する
    ' 引数   ： c     - プロパティを設定するコントロール
    '           name  - コントロール名
    '           text  - コントロールテキスト
    '           top   - 表示位置（Y座標）
    '           left  - 表示位置（X座標）
    '           width - コントロール幅
    '           tag   - 補足情報
    ' 戻り値 ： なし
    '------------------------------------------------------------
    Private Sub Set_Controls(ByVal c As Control, ByVal name As String, ByVal text As String _
                             , ByVal top As Integer, ByVal left As Integer _
                             , ByVal width As Integer, ByVal tag As String)

        c.Name = name
        c.Text = text
        c.Top = top * (RefP.Margin + 20) + RefP.Point_Y
        c.Left = left + RefP.Point_X_Input
        c.Width = width
        c.Tag = tag
        c.Font = New Font("ＭＳ ゴシック", 12)

    End Sub

    '------------------------------------------------------------
    ' 機能   ： コントロールのプロパティを設定する（ラベル）
    ' 引数   ： c     - プロパティを設定するコントロール
    '           name  - コントロール名
    '           text  - コントロールテキスト
    '           font  - フォント
    '           top   - 表示位置（Y座標）
    '           left  - 表示位置（X座標）
    '           width - コントロール幅
    ' 戻り値 ： なし
    '------------------------------------------------------------
    Private Sub Set_Labels(ByVal c As Control, ByVal name As String, ByVal text As String _
                             , ByVal font As String, ByVal top As Integer, ByVal left As Integer _
                             , ByVal width As Integer)
        c.Name = name
        c.Text = text
        c.Top = top * (RefP.Margin + 20) + RefP.Point_Y + 3
        c.Left = left + RefP.Point_X_Label
        c.Width = width
        c.AutoSize = True

        If font = "2" Then
            c.Font = New Font("ＭＳ ゴシック", 12, FontStyle.Bold)
        Else
            c.Font = New Font("ＭＳ ゴシック", 12)
        End If

    End Sub

    '------------------------------------------------------------
    ' 機能   ： タブ順を設定する
    ' 引数   ： なし
    ' 戻り値 ： なし
    '------------------------------------------------------------
    Private Shadows Sub Set_TabIndex()

        Dim tablist As New List(Of Integer)
        Dim ctrllist As New List(Of Control)

        Dim i As Integer
        Dim j As Integer

        Dim max As Integer = 0

        For i = 0 To TabIndexList.Count - 1
            Dim tabindex As Integer = Integer.Parse(TabIndexList(i).Tag)
            If tabindex = -1 Then
                TabIndexList(i).TabStop = False
            Else
                ctrllist.Add(TabIndexList(i))
                tablist.Add(tabindex)
                If max < tabindex Then
                    max = tabindex
                End If
            End If
        Next

        Dim index As Integer = 0
        For i = 0 To max
            For j = 0 To tablist.Count - 1
                If i = tablist(j) Then
                    ctrllist(j).TabIndex = index
                    index += 1
                End If
            Next
        Next

        btnPrt.TabIndex = index
        btnClose.TabIndex = index + 1

    End Sub

    '------------------------------------------------------------
    ' 機能   ： 画面の基準点を取得する
    ' 引数   ： value - 定義ファイルから取得した値
    '           init  - 初期値
    ' 戻り値 ： 画面の基準点
    '------------------------------------------------------------
    Private Function Set_RefP(ByVal value As String, ByVal init As Integer) As Integer
        Dim res As Integer
        value = value.Trim

        '数値以外が指定された場合は初期値を返す
        If Integer.TryParse(value, res) = False Then
            res = init
        End If
        Return res
    End Function

    '------------------------------------------------------------
    ' 機能   ： ラジオボタン、チェックボックスの配置をする
    ' 引数   ： list   - コントロールリスト
    '           row    - 行数
    '           column - 列数
    ' 戻り値 ： True - 正常, False - 異常
    '------------------------------------------------------------
    Private Function Set_GRID(ByVal list As Control(), ByVal row As Integer, ByVal column As Integer) As Integer

        Dim num As Integer = 0
        Dim maxsize As Integer = 0

        '表示するコントロールの数が行*列より多い場合エラー
        If list.Length > (row * column) Then
            Return -1
        End If

        'コントロール幅の最大値を基準に配置する
        For i As Integer = 0 To list.Length - 1
            If list(i).Width > maxsize Then
                maxsize = list(i).Width
            End If
        Next

        'コントロールを配置する
        If list(0).Name.StartsWith("RADIO_") Then
            For i As Integer = 0 To row - 1
                For j As Integer = 0 To column - 1
                    list(num).Top = i * (RefP.Margin + 20)
                    list(num).Left = j * maxsize
                    num += 1
                    If num = list.Length Then
                        Return 0
                    End If
                Next
            Next
        Else
            For i As Integer = 0 To row - 1
                For j As Integer = 0 To column - 1
                    list(num).Top = i * (RefP.Margin + 20) + list(num).Top
                    list(num).Left = j * maxsize + list(num).Left
                    num += 1
                    If num = list.Length Then
                        Return 0
                    End If
                Next
            Next
        End If

        Return -2
    End Function

    '------------------------------------------------------------
    ' 機能   ： 文字列の両端にダブルクォートがある場合に取り除く
    ' 引数   ： item - チェック対象文字列
    ' 戻り値 ： ダブルクォートを取り除いた文字列
    '------------------------------------------------------------
    Private Function Set_Replace_Quote(ByVal item As String) As String

        '対象文字列が1文字以下の場合何もしない
        If item.Length < 2 Then
            Return item
        End If

        '両端にダブルクォートがあるかチェック
        If item.StartsWith(ControlChars.Quote) = True AndAlso _
            item.EndsWith(ControlChars.Quote) = True Then
            item = item.Substring(1, item.Length - 2)
        End If
        Return item
    End Function

    '------------------------------------------------------------
    ' 機能   ： 学校検索コンボボックスの設定を行う
    ' 引数   ： cmb      - コンボボックスオブジェクト
    '           kana     - フリガナ
    '           where    - WHERE句条件文字列
    '           datalist - 学校コードと学校名を格納するリスト
    ' 戻り値 ： 0 - 正常, -1 - 異常
    '------------------------------------------------------------
    Private Function Set_GAKKOU_COMBO(ByVal cmb As ComboBox, _
                                      ByVal kana As String, ByVal where As String, _
                                      ByRef datalist As List(Of String())) As Integer

        cmb.Text = ""
        cmb.Items.Clear()
        datalist.Clear()

        Dim SQL As New StringBuilder
        SQL.Append(" SELECT GAKKOU_CODE_G , GAKKOU_NNAME_G FROM GAKMAST1,GAKMAST2 ")
        SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T ")

        If kana.Trim <> "" Then
            SQL.Append(" AND GAKKOU_KNAME_G LIKE '" & kana & "%'")
        End If

        If where.Trim <> "" Then
            SQL.Append(" " & where)
        End If

        SQL.Append(" GROUP BY GAKKOU_CODE_G , GAKKOU_NNAME_G ")
        SQL.Append(" ORDER BY GAKKOU_CODE_G ")

        Dim temp As List(Of String())
        temp = Get_DB_Data(SQL, 2)

        'DB検索が失敗した場合
        If temp Is Nothing Then
            Return -1
        End If

        datalist.AddRange(temp)

        For Each dbdata As String() In datalist
            cmb.Items.Add(dbdata(1))
        Next

        Return 0

    End Function

    '------------------------------------------------------------
    ' 機能   ： コンボボックス、チェックボックス、ラジオボタン
    '           で選択したアイテムをSQL置換文字列に置き換える
    ' 引数   ： itemlist - 部品固有情報[コントロール表示名,SQL置換文字列]
    '           item     - 置換え対象文字列
    ' 戻り値 ： SQL置換文字列
    '------------------------------------------------------------
    Private Function Set_ItemList(ByVal itemlist As List(Of String()), ByVal item As String) As String
        Dim res As String = ""
        For i As Integer = 0 To itemlist.Count - 1
            If itemlist(i)(0) = item Then
                res = itemlist(i)(1)
                Exit For
            End If
        Next
        Return res
    End Function

    '------------------------------------------------------------
    ' 機能   ： 数値範囲チェックを行う
    ' 引数   ： value - チェックする値
    '           min   - 最小値
    '           max   - 最大値
    ' 戻り値 ： True - 正常 ， False - 異常(指定範囲外)
    '------------------------------------------------------------
    Private Function Check_Num(ByVal value As String, ByVal min As Integer, ByVal max As Integer) As Boolean

        'min,max共に0が指定された場合、数値チェックのみ行う
        Dim num As Integer

        '数値チェック
        If Not Integer.TryParse(value, num) Then
            Return False
        End If

        '範囲チェック
        If (Not (min = 0 AndAlso max = 0)) AndAlso _
           (num < min OrElse num > max) Then
            Return False
        End If

        Return True

    End Function

    '------------------------------------------------------------
    ' 機能   ： DBからデータを取得する
    ' 引数   ： SQL - 検索用SQL文字列
    '           column - 取得カラム数
    ' 戻り値 ： 取得したデータリスト
    '------------------------------------------------------------
    Private Function Get_DB_Data(ByVal SQL As StringBuilder, ByVal column As Integer) As List(Of String())

        Dim datalist As New List(Of String())
        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim oraReader As CASTCommon.MyOracleReader = Nothing

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter3("FrmExtendPrint.Get_DB_Data", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try
            MainDB = New CASTCommon.MyOracle
            oraReader = New CASTCommon.MyOracleReader(MainDB)

            If oraReader.DataReader(SQL) = True Then
                While oraReader.EOF = False
                    Dim data As String() = New String(column - 1) {}

                    For i As Integer = 0 To column - 1
                        data(i) = oraReader.GetValue(i)
                    Next

                    datalist.Add(data)
                    oraReader.NextRead()
                End While
            Else
                'DB異常と取得データ数0件を区別するため、
                'エラーメッセージの中身を確認する
                If oraReader.Message <> "" Then
                    datalist = Nothing
                End If
            End If
        Catch ex As Exception
            LOG.Write_Err("FrmExtendPrint.Get_DB_Data", ex)
            datalist = Nothing
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            LOG.Write_Exit3(sw, "FrmExtendPrint.Get_DB_Data")
        End Try

        Return datalist
    End Function

#End Region

#Region "イベント"

    '------------------------------------------------------------
    ' 機能   ： SQL置換文字列配列を設定し、印刷処理を呼び出す
    '           印刷ボタン押下時に呼び出されるイベントハンドラ
    '------------------------------------------------------------
    Private Sub btnPrt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrt.Click

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter1("FrmExtendPrint.btnPrt_Click", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try

            Dim nRet As Integer
            Dim plist As String()
            Dim isall9 As Boolean = False

            '入力値のチェックと、SQL置換文字列配列設定を行う
            plist = Set_Param(isall9)
            If plist Is Nothing Then
                Return
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(P_MSG0001I, PrtInf.PrtName), _
                               PrtInf.PrtMsgTitle, MessageBoxButtons.YesNo, _
                               MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            '印刷処理呼び出し
            Dim clsExPrt As New CAstExtendPrint.CExtendPrint
            clsExPrt.SetOwner = Me

            'Me.Enabled = False

            If ExInf.PrtExFlag = True Then
                '業務固有印刷あり
                nRet = clsExPrt.ExtendPrint(PrtInf.PrtID, PrtInf.PrtName, plist, PrtInf.PrtDevNameArray, _
                                            isall9, ExInf.PrtDLL, ExInf.PrtClass, ExInf.PrtMethod)
            Else
                '業務固有印刷なし
                nRet = clsExPrt.ExtendPrint(PrtInf.PrtID, PrtInf.PrtName, plist, PrtInf.PrtDevNameArray, isall9)
            End If

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷対象なしメッセージ
                    MessageBox.Show(P_MSG0014W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case -1
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(P_MSG0007E, PrtInf.PrtTitleName), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Case Else
                    '印刷成功メッセージ
                    MessageBox.Show(String.Format(P_MSG0002I, PrtInf.PrtName), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Select

        Catch ex As Exception
            LOG.Write_Err("FrmExtendPrint.btnPrt_Click", ex)
            MessageBox.Show(P_MSG0009E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            LOG.Write_Exit1(sw, "FrmExtendPrint.btnPrt_Click")
            'Me.Enabled = True
        End Try

    End Sub

    '------------------------------------------------------------
    ' 機能   ： フォームクローズ処理を呼び出す
    '           閉じるボタン押下時に呼び出されるイベントハンドラ
    '------------------------------------------------------------
    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click

        Dim sw As System.Diagnostics.Stopwatch
        sw = LOG.Write_Enter1("FrmExtendPrint.btnClose_Click", PrtInf.PrtID & "_" & PrtInf.PrtName)

        Try
            Close_Frm()
        Catch ex As Exception
            LOG.Write_Err("FrmExtendPrint.btnPrt_Click", ex)
            MessageBox.Show(P_MSG0009E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            LOG.Write_Exit1(sw, "FrmExtendPrint.btnClose_Click")
        End Try

    End Sub

    '------------------------------------------------------------
    ' 機能    ： 取引先／学校コードのフリガナ検索を行う
    '            コンボボックスのアイテムが選択された場合呼び出されるイベントハンドラ
    ' 対象部品： COMBO_CODE
    '------------------------------------------------------------
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Try

            Dim cmb As ComboBox = CType(sender, ComboBox)
            Dim dbkind As String = ""
            Dim i As Integer
            For i = 0 To PartsList.Count
                If PartsList(i).PartsName = "COMBO_CODE" AndAlso _
                   cmb.Name = PartsList(i).PartsInputList(0).Name Then
                    Exit For
                End If
            Next

            dbkind = cmb.Tag.ToString

            'dbkind 1（自振）
            '       2（総振)
            '       3（学校）
            If dbkind = "1" Then
                If GCom.SelectItakuName(cmb.SelectedItem.ToString _
                                      , CType(PartsList(i).PartsInputList(1), ComboBox) _
                                      , CType(PartsList(i).PartsInputList(2), TextBox) _
                                      , CType(PartsList(i).PartsInputList(3), TextBox), _
                                      "1", PartsList(i).PartsOption) = -1 Then
                    MessageBox.Show(String.Format(P_MSG0008E, PartsList(i).PartsLabelList(0).Text.Trim), _
                                    PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            ElseIf dbkind = "2" Then
                If GCom.SelectItakuName(cmb.SelectedItem.ToString _
                                      , CType(PartsList(i).PartsInputList(1), ComboBox) _
                                      , CType(PartsList(i).PartsInputList(2), TextBox) _
                                      , CType(PartsList(i).PartsInputList(3), TextBox), _
                                      "3", PartsList(i).PartsOption) = -1 Then
                    MessageBox.Show(String.Format(P_MSG0008E, PartsList(i).PartsLabelList(0).Text.Trim), _
                                    PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            ElseIf dbkind = "3" Then
                If Set_GAKKOU_COMBO(CType(PartsList(i).PartsInputList(1), ComboBox) _
                                  , cmb.SelectedItem.ToString _
                                  , PartsList(i).PartsOption _
                                  , PartsList(i).PartsOptionList) = -1 Then
                    MessageBox.Show(String.Format(P_MSG0008E, PartsList(i).PartsLabelList(0).Text.Trim), _
                                    PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            End If

            ' ADD 2016/01/29 SO)山岸 For IT_D-05_003 取引先検索コンボカナ選択時のフォーカス動作バグ
            PartsList(i).PartsInputList(1).Focus()

        Catch ex As Exception
            LOG.Write_Err("FrmExtendPrint.cmbKana_SelectedIndexChanged", ex)
            MessageBox.Show(P_MSG0009E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    '------------------------------------------------------------
    ' 機能    ： コンボボックスで選択されたアイテムの値をテキストボックスとラベルに反映する
    '            コンボボックスのアイテムが選択された場合呼び出されるイベントハンドラ
    ' 対象部品： COMBO_CODE、COMBO_SQL
    '------------------------------------------------------------
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Try

            Dim cmb As ComboBox = CType(sender, ComboBox)

            If cmb.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmb.SelectedItem.ToString) Then
                Return
            End If

            Dim txt As TextBox = New TextBox()
            Dim dbkind As String = cmb.Tag.ToString

            Dim i As Integer
            For i = 0 To PartsList.Count
                If PartsList(i).PartsName = "COMBO_CODE" OrElse PartsList(i).PartsName = "COMBO_SQL" Then
                    If cmb.Name = PartsList(i).PartsInputList(0).Name OrElse _
                       cmb.Name = PartsList(i).PartsInputList(1).Name Then
                        Exit For
                    End If
                End If
            Next

            'dbkind 1（自振）
            '       2（総振)
            '       3（学校）
            '       4（SQL検索）

            If dbkind = "1" OrElse dbkind = "2" Then
                Dim txt_s As TextBox = CType(PartsList(i).PartsInputList(2), TextBox)
                txt = CType(PartsList(i).PartsInputList(3), TextBox)
                Dim lbl As Label = CType(PartsList(i).PartsInputList(4), Label)

                lbl.Text = ""
                GCom.Set_TORI_CODE(cmb, txt_s, txt)

                TextBox_Validated(txt, e)

            ElseIf dbkind = "3" Then

                txt = CType(PartsList(i).PartsInputList(2), TextBox)
                Dim lbl As Label = CType(PartsList(i).PartsInputList(3), Label)

                txt.Text = ""
                lbl.Text = ""

                '選択されたコンボボックスアイテムのインデックスを取得
                Dim index As Integer = cmb.SelectedIndex

                'データリストから対応する値を取得し、
                'テキストボックスとラベルに値を反映する
                txt.Text = PartsList(i).PartsOptionList(index)(0)
                lbl.Text = PartsList(i).PartsOptionList(index)(1)

            ElseIf dbkind = "4" Then

                txt = CType(PartsList(i).PartsInputList(1), TextBox)
                Dim lbl As Label = CType(PartsList(i).PartsInputList(2), Label)

                txt.Text = ""
                lbl.Text = ""

                'コンボボックスのアイテムからコードと名前を取得する
                If cmb.Text.Trim.Length <> 0 Then
                    Dim str As String() = Split(cmb.Text, ":")
                    txt.Text = Trim(str(0))
                    lbl.Text = Trim(str(1))
                End If

            End If

            '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
            Dim pinf As PartsInformation = PartsList(i)
            pinf.CodeErrFlg = False   ' 入力コードエラーフラグOFF
            PartsList(i) = pinf
            '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***

            'タブ移動が有効の場合
            If txt.TabStop = True Then
                'フォーカス移動処理を呼び出す
                Me.SelectNextControl(txt, True, True, True, True)
            End If

        Catch ex As Exception
            LOG.Write_Err("FrmExtendPrint.cmbToriName_SelectedIndexChanged", ex)
            MessageBox.Show(P_MSG0009E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    '------------------------------------------------------------
    ' 機能    ： コード検索を行い、ラベルに反映する
    '            テキストボックスからフォーカスが移った時に
    '            呼び出されるイベントハンドラ
    ' 対象部品： COMBO_CODE、COMBO_SQL
    '------------------------------------------------------------
    Private Sub TextBox_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Try
            Dim txt As TextBox = CType(sender, TextBox)

            'テキストが未入力の場合、何もしない
            If txt.Text.Length < 1 Then
                Return
            End If

            Dim i As Integer
            For i = 0 To PartsList.Count
                If PartsList(i).PartsName = "COMBO_CODE" Then
                    If txt.Name = PartsList(i).PartsInputList(2).Name OrElse _
                       txt.Name = PartsList(i).PartsInputList(3).Name Then
                        Exit For
                    End If
                End If
                If PartsList(i).PartsName = "COMBO_SQL" Then
                    If txt.Name = PartsList(i).PartsInputList(1).Name Then
                        Exit For
                    End If
                End If
            Next

            Dim cmb As ComboBox = CType(PartsList(i).PartsInputList(0), ComboBox)
            Dim lbl As New Label()
            Dim datalist As List(Of String())
            Dim SQL As New StringBuilder
            Dim dbkind As String = cmb.Tag.ToString

            'dbkind 1（自振）
            '       2（総振)
            '       3（学校）
            '       4（SQL検索）

            If dbkind = "1" Then
                Dim txt_s As TextBox = CType(PartsList(i).PartsInputList(2), TextBox)
                '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                txt = CType(PartsList(i).PartsInputList(3), TextBox)
                '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***

                '*** Str Upd 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                '主コードが未入力の場合、何もしない
                'If txt_s.Text.Length < 1 Then
                '主コード、副コードのどちらかが未入力の場合、何もしない
                If txt_s.Text.Length < 1 OrElse txt.Text.Length < 1 Then
                '*** End Upd 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    Return
                End If

                lbl = CType(PartsList(i).PartsInputList(4), Label)
                lbl.Text = ""

                'ALL9指定の場合、何もしない
                Dim all9 As String = "999999999999"
                If PartsList(i).PartsFlag = True AndAlso _
                   txt_s.Text & txt.Text = all9 Then
                    Return
                End If

                'SQL検索
                SQL.Append(" SELECT ITAKU_NNAME_T FROM TORIMAST ")
                SQL.Append(" WHERE FSYORI_KBN_T = '1' ")
                SQL.Append(" AND TORIS_CODE_T = '" & txt_s.Text & "'")
                SQL.Append(" AND TORIF_CODE_T = '" & txt.Text & "'")

                If PartsList(i).PartsOption.Trim <> "" Then
                    SQL.Append(Space(1) & PartsList(i).PartsOption.Trim)
                End If

                datalist = Get_DB_Data(SQL, 1)

                'DB検索が失敗した場合
                If datalist Is Nothing Then
                    MessageBox.Show(P_MSG0009E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                '該当なしの場合
                If datalist.Count = 0 Then
                    '既存処理に倣い、エラーは出力しない
                    'MessageBox.Show(P_MSG0008W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    'Me.ActiveControl = txt
                    '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    Dim pinf As PartsInformation = PartsList(i)
                    pinf.CodeErrFlg = True   ' 入力コードエラーフラグON
                    PartsList(i) = pinf
                    '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    Return
                '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                Else
                    Dim pinf As PartsInformation = PartsList(i)
                    pinf.CodeErrFlg = False   ' 入力コードエラーフラグOFF
                    PartsList(i) = pinf
                '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                End If

                lbl.Text = datalist(0)(0)

            ElseIf dbkind = "2" Then
                Dim txt_s As TextBox = CType(PartsList(i).PartsInputList(2), TextBox)
                '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                txt = CType(PartsList(i).PartsInputList(3), TextBox)
                '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***

                '*** Str Upd 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                '主コードが未入力の場合、何もしない
                'If txt_s.Text.Length < 1 Then
                '主コード、副コードのどちらかが未入力の場合、何もしない
                If txt_s.Text.Length < 1 OrElse txt.Text.Length < 1 Then
                '*** End Upd 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    Return
                End If

                lbl = CType(PartsList(i).PartsInputList(4), Label)
                lbl.Text = ""

                'ALL9指定の場合、何もしない
                Dim all9 As String = "999999999999"
                If PartsList(i).PartsFlag = True AndAlso _
                   txt_s.Text & txt.Text = all9 Then
                    Return
                End If

                SQL.Append(" SELECT ITAKU_NNAME_T FROM S_TORIMAST ")
                SQL.Append(" WHERE FSYORI_KBN_T = '3' ")
                SQL.Append(" AND TORIS_CODE_T = '" & txt_s.Text & "'")
                SQL.Append(" AND TORIF_CODE_T = '" & txt.Text & "'")
                If PartsList(i).PartsOption.Trim <> "" Then
                    SQL.Append(Space(1) & PartsList(i).PartsOption.Trim)
                End If

                datalist = Get_DB_Data(SQL, 1)

                'DB検索が失敗した場合
                If datalist Is Nothing Then
                    MessageBox.Show(P_MSG0009E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                '該当なしの場合
                If datalist.Count = 0 Then
                    '既存処理に倣い、エラーは出力しない
                    'MessageBox.Show(P_MSG0008W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    'Me.ActiveControl = txt
                    '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    Dim pinf As PartsInformation = PartsList(i)
                    pinf.CodeErrFlg = True   ' 入力コードエラーフラグON
                    PartsList(i) = pinf
                    '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    Return
                '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                Else
                    Dim pinf As PartsInformation = PartsList(i)
                    pinf.CodeErrFlg = False   ' 入力コードエラーフラグOFF
                    PartsList(i) = pinf
                '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                End If

                lbl.Text = datalist(0)(0)

            ElseIf dbkind = "3" Then

                lbl = CType(PartsList(i).PartsInputList(3), Label)
                lbl.Text = ""

                Dim all9 As String = "9999999999"
                If PartsList(i).PartsFlag = True AndAlso _
                   txt.Text = all9 Then
                    Return
                End If

                SQL.Append(" SELECT GAKKOU_NNAME_G FROM GAKMAST1,GAKMAST2 ")
                SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T ")
                SQL.Append(" AND GAKKOU_CODE_G = '" & txt.Text & "'")
                If PartsList(i).PartsOption.Trim <> "" Then
                    SQL.Append(Space(1) & PartsList(i).PartsOption.Trim)
                End If

                datalist = Get_DB_Data(SQL, 1)

                'DB検索が失敗した場合
                If datalist Is Nothing Then
                    MessageBox.Show(P_MSG0009E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                '該当なしの場合
                If datalist.Count = 0 Then
                    '*** Str Upd 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    'MessageBox.Show(P_MSG0009W, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    'Me.ActiveControl = txt
                    Dim pinf As PartsInformation = PartsList(i)
                    pinf.CodeErrFlg = True   ' 入力コードエラーフラグON
                    PartsList(i) = pinf
                    '*** End Upd 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    Return
                '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                Else
                    Dim pinf As PartsInformation = PartsList(i)
                    pinf.CodeErrFlg = False   ' 入力コードエラーフラグOFF
                    PartsList(i) = pinf
                '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                End If

                lbl.Text = datalist(0)(0)

            ElseIf dbkind = "4" Then
                lbl = CType(PartsList(i).PartsInputList(2), Label)
                lbl.Text = ""

                'コンボボックスからコードと名前の値を取得する
                'アイテムは" コード ： 名前 "の形式で格納されている
                For num As Integer = 0 To cmb.Items.Count - 1
                    If cmb.Items(num).ToString.Contains(":") = True Then
                        Dim str As String() = Split(cmb.Items(num).ToString, ":")
                        If txt.Text = Trim(str(0)) Then
                            lbl.Text = Trim(str(1))
                            Exit For
                        End If
                    End If
                Next

                '該当なしの場合
                If lbl.Text.Length = 0 Then
                    '*** Str Upd 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    'Dim name As String = PartsList(i).PartsLabelList(1).Text
                    'MessageBox.Show(String.Format(P_MSG0003W, name, name), PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    'Me.ActiveControl = txt
                    Dim pinf As PartsInformation = PartsList(i)
                    pinf.CodeErrFlg = True   ' 入力コードエラーフラグON
                    PartsList(i) = pinf
                    '*** End Upd 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                    Return
                '*** Str Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                Else
                    Dim pinf As PartsInformation = PartsList(i)
                    pinf.CodeErrFlg = False   ' 入力コードエラーフラグOFF
                    PartsList(i) = pinf
                '*** End Add 2016/03/02 SO)荒木 for 検査インシデントJIV2-007修正 ***
                End If
            End If

        Catch ex As Exception
            LOG.Write_Err("FrmExtendPrint.TextBox_Validated", ex)
            MessageBox.Show(P_MSG0009E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    '------------------------------------------------------------
    ' 機能    ： パディング処理を行う
    '            テキストボックスからフォーカスが移った時に
    '            呼び出されるイベントハンドラ
    ' 対象部品： COMBO_CODE、COMBO_SQL、CODE、RANGE_CODE
    '            DATE、RANGE_DATE、TEXT
    '------------------------------------------------------------
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs)

        Try
            Dim txt As TextBox = CType(sender, TextBox)
            '未入力の場合は何もしない
            If txt.Text.Length < 1 Then
                Return
            End If

            '部品種別がTEXT以外の場合、前0パディング処理を行う
            If txt.Name.StartsWith("TEXT_") = False Then
                GCom.NzNumberString(txt, True)
                Return
            End If


            '部品種別がTEXTの場合
            For i As Integer = 0 To PartsList.Count - 1
                If txt.Name = PartsList(i).PartsInputList(0).Name Then

                    '数値入力制限有の場合、数値チェックを行う
                    If PartsList(i).PartsFlag = True Then
                        Dim res As Integer
                        If Integer.TryParse(txt.Text, res) = False Then
                            txt.Text = ""
                        End If
                        If txt.Text.Length < 1 Then
                            Return
                        End If
                    End If

                    '全角文字が入力されたら削除する
                    If System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(txt.Text) <> Len(txt.Text) Then
                        txt.Text = ""
                        Return
                    End If

                    '最大桁数（文字数）を超えた場合は超えた分を切り取る
                    If Len(txt.Text) > txt.MaxLength Then
                        txt.Text = txt.Text.Substring(0, txt.MaxLength)
                        Return
                    End If

                    If PartsList(i).PartsOption = "0" Then
                        '処理なし
                        Return
                    ElseIf PartsList(i).PartsOption = "1" Then
                        '前0パディング
                        txt.Text = txt.Text.PadLeft(txt.MaxLength, "0")
                        Return
                    ElseIf PartsList(i).PartsOption = "2" Then
                        '後空白パディング
                        txt.Text = txt.Text.PadRight(txt.MaxLength)
                        Return
                    End If
                End If
            Next

        Catch ex As Exception
            LOG.Write_Err("FrmExtendPrint.TextBox_Validating", ex)
            MessageBox.Show(P_MSG0009E, PrtInf.PrtMsgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

End Class
