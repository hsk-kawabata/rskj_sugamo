Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text

Public Class KFGMAIN040

    Structure gstrFURI_DATA
        <VBFixedString(120)> Public strDATA As String
    End Structure
    '120バイト格納用
    Public gstrDATA As gstrFURI_DATA

#Region " 共通変数定義 "
    Private Str_Date_Start As String
    Private Str_Date_End As String

    Private Str_Select_Nyusyutu_Code As String
    Private Str_Itaku_Code As String

    Private Str_Gakkou_Code As String

    Private Str_Takou_Kbn As String

    Private Str_Ginko_Kbn As String

    Private Str_Seikyu_Nentuki As String

    Private Lng_Zumi(1) As Long
    Private Lng_Funou(1) As Long

    Private Lng_GZumi(9, 1) As Long
    Private Lng_GFunou(9, 1) As Long


    Private Str_ZenginFileName As String
    Private Str_Gakkou_Name As String
    Private Str_SaveFileName As String

    Private Str_Jikou_Path As String
    Private Str_Takou_Path As String
    Private Str_KekkaTakou_Path As String

    Private Str_Takou_FileName(,) As String
    Private Str_Jikou_FileName As String
    Private Str_Tak_FileName As String = "" '追加 2006/10/03

    Private Lng_Takou_Count As Long

    Private Lng_Syori(1) As Long

    '追加 2006/10/05
    Public lngSUMI_KEN_ALL As Long = 0
    Public dblSUMI_KIN_ALL As Double = 0
    Public lngFUNOU_KEN_ALL As Long = 0
    Public dblFUNOU_KIN_ALL As Double = 0
    Public Str_Takou_Ginko_Code As String = ""
    Public Str_Takou_BAITAI() As String
    Public Str_Jikou_BAITAI As String = ""

    Public flgJIKOU_MEISAI_ZERO As Boolean = False '自行明細が0件かどうか
    '2006/10/27
    Public blnGAK_FLG As Boolean = False

#End Region

#Region " Form Load "
    Private Sub KFGMAIN040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        With Me
            .WindowState = FormWindowState.Normal
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .ControlBox = True
        End With

        STR_SYORI_NAME = "口座振替不能結果更新"
        STR_COMMAND = "Form_Load"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = ""

        Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
        MainLog = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

        '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
        Call GSUB_CONNECT()
        '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

        '学校コンボ設定（全学校）
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = False Then
            Call GSUB_LOG(0, "コンボボックス設定(cmbGAKKOUNAME)")

            Call GSUB_MESSAGE_WARNING("学校名コンボボックス設定でエラーが発生しました")

            Exit Sub
        End If

        '自行全銀結果格納PATH取得
        Str_Jikou_Path = GFUNC_INI_READ("COMMON", "KEKKAFL")

        If Trim(Str_Jikou_Path) = "" Then
            Call GSUB_LOG(0, "INIファイル取得(KEKKAFL)")

            Call GSUB_MESSAGE_WARNING("INIﾌｧｲﾙ取得で自行全銀結果格納PATHの取得でエラーが発生しました")

            Exit Sub
        End If

        '他行全銀結果格納PATH取得
        Str_Takou_Path = GFUNC_INI_READ("COMMON", "TAKKEKKAFL")

        If Trim(Str_Takou_Path) = "" Then
            Call GSUB_LOG(0, "INIファイル取得(TAKKEKKAFL)")

            Call GSUB_MESSAGE_WARNING("INIﾌｧｲﾙ取得で他行全銀結果格納PATHの取得でエラーが発生しました")

            Exit Sub
        End If


        '振替結果ファイル取得PATH(他行)
        Str_KekkaTakou_Path = GFUNC_INI_READ("COMMON", "TAK")

        If Trim(Str_KekkaTakou_Path) = "" Then
            Call GSUB_LOG(0, "INIファイル取得(TAK)")

            Call GSUB_MESSAGE_WARNING("INIﾌｧｲﾙ取得で振替結果ファイル取得PATH(他行)の取得でエラーが発生しました")

            Exit Sub
        End If

        'テキストファイルからコンボボックス設定
        If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_TAKOU_TXT, cmbTakou) = False Then
            Call GSUB_LOG(0, "コンボボックス設定(cmbTakou)")

            Call GSUB_MESSAGE_WARNING("他行区分コンボボックス設定でエラーが発生しました")

            Exit Sub
        End If

        'テキストファイルからコンボボックス設定
        If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbNyusyutu) = False Then
            Call GSUB_LOG(0, "コンボボックス設定(cmbNyusyutu)")

            Call GSUB_MESSAGE_WARNING("入出区分コンボボックス設定でエラーが発生しました")

            Exit Sub
        End If

        'Oracle 接続(Read専用)
        OBJ_CONNECTION_DREAD = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read専用)
        OBJ_CONNECTION_DREAD.Open()

        'Oracle 接続(Read専用)
        OBJ_CONNECTION_DREAD2 = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read専用)
        OBJ_CONNECTION_DREAD2.Open()

        Dim WorkDate As String = fn_GetEigyoubi(Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd"), STR_JIFURI_FUNOU, "-")
        If WorkDate.Length = 8 Then
            txtFuriDateY.Text = WorkDate.Substring(0, 4)
            txtFuriDateM.Text = WorkDate.Substring(4, 2)
            txtFuriDateD.Text = WorkDate.Substring(6)
        Else
            Call GSUB_LOG(0, "営業日補正")
            Call GSUB_MESSAGE_WARNING("営業日補正時にエラーが発生しました")
            Exit Sub
        End If

    End Sub
#End Region

#Region " Button Click "
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUPDATE.Click

        Cursor.Current = Cursors.WaitCursor()
        Dim strDIR As String
        strDIR = CurDir()

        flgJIKOU_MEISAI_ZERO = False
        '2006/10/18　続けて2校処理をするとPATHが変わってしまっているため、再度INIファイルの読込を行う
        '自行全銀結果格納PATH取得
        Str_Jikou_Path = GFUNC_INI_READ("COMMON", "KEKKAFL")

        If Trim(Str_Jikou_Path) = "" Then
            Call GSUB_LOG(0, "INIファイル取得(KEKKAFL)")

            Call GSUB_MESSAGE_WARNING("INIﾌｧｲﾙ取得で自行全銀結果格納PATHの取得でエラーが発生しました")

            Exit Sub
        End If

        '他行全銀結果格納PATH取得
        Str_Takou_Path = GFUNC_INI_READ("COMMON", "TAKKEKKAFL")

        If Trim(Str_Takou_Path) = "" Then
            Call GSUB_LOG(0, "INIファイル取得(TAKKEKKAFL)")

            Call GSUB_MESSAGE_WARNING("INIﾌｧｲﾙ取得で他行全銀結果格納PATHの取得でエラーが発生しました")

            Exit Sub
        End If

        '振替結果ファイル取得PATH(他行)
        Str_KekkaTakou_Path = GFUNC_INI_READ("COMMON", "TAK")

        If Trim(Str_KekkaTakou_Path) = "" Then
            Call GSUB_LOG(0, "INIファイル取得(TAK)")

            Call GSUB_MESSAGE_WARNING("INIﾌｧｲﾙ取得で振替結果ファイル取得PATH(他行)の取得でエラーが発生しました")

            Exit Sub
        End If

        '入力チェック
        If PFUNC_Nyuryoku_Check() = False Then
            '自行分の明細が0件だった場合、不能フラグの更新を行う 2006/10/16
            If flgJIKOU_MEISAI_ZERO = True Then
                'スケジュールマスタ更新(済・不能の件数＆金額の更新)
                If PFUNC_Update_Schedule(0) = False Then
                    Call GSUB_LOG(0, "スケジュールマスタ更新でエラーが発生しました")
                    ChDir(strDIR)
                    btnUPDATE.Focus()
                Else
                    '完了メッセージ
                    Call GSUB_MESSAGE_INFOMATION("更新しました")
                    Call GSUB_LOG(1, "不能結果更新")
                    ChDir(strDIR)
                    btnEnd.Focus()
                End If
                Exit Sub
            Else
                ChDir(strDIR)
                btnUPDATE.Focus()
                Exit Sub
            End If
        End If

        STR_COMMAND = "更新"
        STR_LOG_GAKKOU_CODE = txtGAKKOU_CODE.Text
        STR_LOG_FURI_DATE = STR_FURIKAE_DATE(1)

        '初期化 2006/04/18
        ReDim Lng_GZumi(9, 1)
        ReDim Lng_GFunou(9, 1)

        '確認メッセージ
        If GFUNC_MESSAGE_QUESTION("更新しますか？") <> vbOK Then
            ChDir(strDIR)
            btnUPDATE.Focus()
            Exit Sub
        End If

        Select Case (CInt(Str_Takou_Kbn))
            Case 0
                '自行

                'スケジュール件数、金額を取得
                If PFUNC_Get_Kingaku() = False Then
                    ChDir(strDIR)
                    btnUPDATE.Focus()
                    Exit Sub
                End If

                ' 2017/06/07 タスク）綾部 CHG (RSV2標準対応 媒体コード調整) -------------------- START
                'If Str_Jikou_BAITAI = "0" Then
                '    'Str_Jikou_Path = Str_Kekka_Path
                'Else
                '    Str_Jikou_Path = "A:\"
                'End If
                Select Case Str_Jikou_BAITAI
                    Case "1"
                        Str_Jikou_Path = "A:\"
                    Case Else
                        ' 処理なし
                End Select
                ' 2017/06/07 タスク）綾部 CHG (RSV2標準対応 媒体コード調整) -------------------- END

                'Str_ZenginFileName = Str_Jikou_Path & "R" & Str_Gakkou_Code & ".dat"
                Str_ZenginFileName = STR_DAT_PATH & "GO" & Str_Gakkou_Code & ".dat"

                'ＦＤデータ又はファイルの読み取りコピー
                Select Case (PFUNC_FD_Copy())
                    Case 0
                        '正常
                    Case 1
                        'キャンセル
                        Call GSUB_MESSAGE_WARNING("処理が中断されました")
                        'キャンセル
                        ChDir(strDIR)
                        btnUPDATE.Focus()
                        Exit Sub
                    Case Else
                        'エラー
                        Call GSUB_MESSAGE_WARNING("処理が中断されました")
                        Call GSUB_LOG(0, "不能結果ファイル読み込みエラー")
                        ChDir(strDIR)
                        btnUPDATE.Focus()
                        Exit Sub
                End Select

                'チェック＆更新
                If PFUNC_File_Check_Update(Str_ZenginFileName) = False Then
                    Call GSUB_LOG(0, "不能結果更新")

                    Call GSUB_MESSAGE_WARNING("不能結果更新処理失敗")
                    ChDir(strDIR)
                    btnUPDATE.Focus()
                    Exit Sub
                End If

                'スケジュールマスタ更新(済・不能の件数＆金額の更新)
                If PFUNC_Update_Schedule(0) = False Then
                    Call GSUB_LOG(0, "スケジュールマスタ更新でエラーが発生しました")
                    ChDir(strDIR)
                    btnUPDATE.Focus()
                    Exit Sub
                End If
            Case 1
                '他行

                For iLoopCount As Integer = 0 To Lng_Takou_Count

                    ''指定受信ファイルの存在チェック
                    ''存在しない場合は次の他行ファイル処理へ
                    'If Dir(Str_KekkaTakou_Path & Str_Takou_FileName(0, iLoopCount), vbNormal) = "" Then
                    '    GoTo NexFor
                    'End If
                    Str_Tak_FileName = ""
                    '媒体によって指定先変更
                    If Str_Takou_BAITAI(iLoopCount) = "0" Then '伝送
                        'Str_Takou_Path = Str_KekkaTakou_Path
                    Else 'FD3.5
                        Str_Takou_Path = "A:\"
                    End If
                    'Str_Tak_FileName = Str_Takou_Path & Str_Takou_FileName(0, iLoopCount)
                    Str_Tak_FileName = Str_Takou_FileName(0, iLoopCount)

                    '"O"+学校コード+金融機関コード+".dat"
                    'Str_ZenginFileName = Str_KekkaTakou_Path & "R" & Str_Gakkou_Code & Str_Takou_FileName(1, iLoopCount) & ".dat"
                    Str_ZenginFileName = Str_KekkaTakou_Path & "GO" & Str_Gakkou_Code & Str_Takou_FileName(1, iLoopCount) & ".dat"

                    Str_Takou_Ginko_Code = Str_Takou_FileName(1, iLoopCount)

                    'ＦＤデータ又はファイルの読み取りコピー
                    Select Case (PFUNC_FD_Copy_TAKOU())
                        Case 0
                            '正常
                        Case 1
                            'キャンセル
                            Call GSUB_MESSAGE_WARNING("処理が中断されました")

                            If iLoopCount < Lng_Takou_Count Then
                                If MessageBox.Show("次の他行の不能結果を更新しますか？", "不能結果更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                                    ChDir(strDIR)
                                    btnUPDATE.Focus()
                                    Exit Sub
                                Else
                                    GoTo NexFor
                                End If
                            Else
                                ChDir(strDIR)
                                btnUPDATE.Focus()
                                Exit Sub
                            End If
                        Case Else
                            'エラー
                            Call GSUB_LOG(0, "不能結果ファイル読み込みエラー")
                            Call GSUB_MESSAGE_WARNING("処理が中断されました")
                            ChDir(strDIR)
                            txtGAKKOU_CODE.Focus()
                            txtGAKKOU_CODE.SelectionStart = 0
                            txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length
                            Exit Sub
                    End Select

                    '他行スケジュール件数、金額を取得（現在処理中の金融機関のみ）
                    If PFUNC_Get_Kingaku_Takou(Str_Takou_FileName(1, iLoopCount)) = False Then
                        ChDir(strDIR)
                        btnUPDATE.Focus()
                        Exit Sub
                    End If

                    'チェック＆更新
                    If PFUNC_File_Check_Update(Str_ZenginFileName) = False Then
                        Call GSUB_LOG(0, "不能結果更新")
                        ChDir(strDIR)
                        btnUPDATE.Focus()
                        Exit Sub
                    End If

                    '他行スケジュールマスタ更新(済・不能の件数＆金額の更新)
                    '※他行は学校、金融機関、学年毎に１レコード存在するので
                    '　集計も同様の条件で集計した結果を更新する
                    If PFUNC_Update_TakouSchedule(Str_Takou_FileName(1, iLoopCount)) = False Then
                        Call GSUB_LOG(0, "他行スケジュールマスタ更新でエラーが発生しました")
                        ChDir(strDIR)
                        btnUPDATE.Focus()
                        Exit Sub
                    End If

                    'スケジュールマスタ更新(済・不能の件数＆金額の更新)
                    If PFUNC_Update_Schedule(1) = False Then
                        Call GSUB_LOG(0, "スケジュールマスタ更新でエラーが発生しました")
                        ChDir(strDIR)
                        btnUPDATE.Focus()
                        Exit Sub
                    End If


NexFor:
                Next iLoopCount
        End Select

        '口座実績マスタの更新（自行／他行共通）
        If PFUNC_Update_Jisseki() = False Then
            Call GSUB_LOG(0, "口座実績マスタの更新")
            ChDir(strDIR)
            btnUPDATE.Focus()
            Exit Sub
        End If

        '完了メッセージ
        Call GSUB_MESSAGE_INFOMATION("更新しました")
        Call GSUB_LOG(1, "不能結果更新")
        ChDir(strDIR)
        btnEnd.Focus()

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        If OBJ_CONNECTION_DREAD Is Nothing Then
        Else
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD.Close()
            OBJ_CONNECTION_DREAD = Nothing
        End If

        If OBJ_CONNECTION_DREAD2 Is Nothing Then
        Else
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD2.Close()
            OBJ_CONNECTION_DREAD2 = Nothing
        End If

        Me.Close()

    End Sub
#End Region

#Region " Private Sub "
    '****************************
    'Private Sub
    '****************************
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '学校カナ絞込みコンボ
        '********************************************
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校名コンボボックス設定
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = True Then
            cmbGAKKOUNAME.Focus()
        End If

    End Sub
    Private Sub cmbGAKKOUNAME_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGAKKOUNAME.SelectedIndexChanged

        'COMBOBOX選択時学校名,学校コード設定
        lblGAKKOU_NAME.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)

        '学年テキストボックスにFOCUS
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        With CType(sender, TextBox)
            If .Text.Trim <> "" Then
                '学校名検索
                STR_SQL = "SELECT GAKKOU_NNAME_G FROM GAKMAST1 "
                STR_SQL += " WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Sub
                End If

                OBJ_DATAREADER.Read()
                If OBJ_DATAREADER.HasRows = True Then
                    lblGAKKOU_NAME.Text = CStr(OBJ_DATAREADER.Item("GAKKOU_NNAME_G"))
                Else
                    lblGAKKOU_NAME.Text = ""
                End If

                If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                    Exit Sub
                End If

                STR_SQL = "SELECT KAISI_DATE_T , SYURYOU_DATE_T FROM GAKMAST2 "
                STR_SQL += " WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Sub
                End If

                OBJ_DATAREADER.Read()
                If OBJ_DATAREADER.HasRows = False Then
                    Str_Date_Start = ""
                    Str_Date_End = ""
                Else
                    Str_Date_Start = CStr(OBJ_DATAREADER.Item("KAISI_DATE_T"))
                    Str_Date_End = CStr(OBJ_DATAREADER.Item("SYURYOU_DATE_T"))
                End If

                If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                    Exit Sub
                End If
            End If
        End With

    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " Private Function "
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        Dim sSch_Data_Flg As String
        Dim sSch_Funou_Flg As String
        Dim sSch_Tyudan_Flg As String
        Dim sSch_Furi_Kbn As String

        Dim lCount As Long

        PFUNC_Nyuryoku_Check = False

        Dim SQL As New StringBuilder(128)

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("学校コードが未入力です。")
            txtGAKKOU_CODE.Focus()

            Exit Function
        Else
            '学校マスタ存在チェック
            SQL.Length = 0
            SQL.Append(" SELECT ")
            SQL.Append(" TAKO_KBN_T ")
            SQL.Append(",ITAKU_CODE_T ")
            SQL.Append(",FILE_NAME_T ")
            SQL.Append(",GAKKOU_NNAME_G ")
            SQL.Append(",GAKKOU_KNAME_G ")
            SQL.Append(",BAITAI_CODE_T ")
            SQL.Append(" FROM GAKMAST1 , GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")
            SQL.Append(" AND GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
                Exit Function
            End If

            OBJ_DATAREADER.Read()

            If OBJ_DATAREADER.HasRows = False Then
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If

                Call GSUB_MESSAGE_WARNING("入力された学校コードが存在しません。")
                txtGAKKOU_CODE.Focus()

                Exit Function
            End If

            'Str_Takou_Kbn = OBJ_DATAREADER.Item("TAKO_KBN_T")
            Str_Itaku_Code = OBJ_DATAREADER.Item("ITAKU_CODE_T")
            If IsDBNull(OBJ_DATAREADER.Item("FILE_NAME_T")) = True Then
                Str_Jikou_FileName = ""
            Else
                Str_Jikou_FileName = CStr(OBJ_DATAREADER.Item("FILE_NAME_T")).Trim
            End If
            If IsDBNull(OBJ_DATAREADER.Item("GAKKOU_NNAME_G")) Then
                Str_Gakkou_Name = CStr(OBJ_DATAREADER.Item("GAKKOU_NNAME_G")).Trim
            Else
                Str_Gakkou_Name = CStr(OBJ_DATAREADER.Item("GAKKOU_KNAME_G")).Trim
            End If

            Str_Jikou_BAITAI = CStr(OBJ_DATAREADER.Item("BAITAI_CODE_T")).Trim

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If
        End If

        Str_Gakkou_Code = txtGAKKOU_CODE.Text

        '振替日
        If Trim(txtFuriDateY.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("振替日(年)が未入力です。")
            txtFuriDateY.Focus()

            Exit Function
        End If

        If Trim(txtFuriDateM.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("振替日(月)が未入力です。")
            txtFuriDateM.Focus()

            Exit Function
        Else
            Select Case (CInt(txtFuriDateM.Text))
                Case 1 To 12
                Case Else
                    Call GSUB_MESSAGE_WARNING("請求月は１月～１２月を設定してください。")
                    txtFuriDateM.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtFuriDateD.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("振替日(日)が未入力です。")
            txtFuriDateD.Focus()

            Exit Function
        End If

        STR_FURIKAE_DATE(0) = Trim(txtFuriDateY.Text) & "/" & Trim(txtFuriDateM.Text) & "/" & Trim(txtFuriDateD.Text)
        STR_FURIKAE_DATE(1) = Trim(txtFuriDateY.Text) & Format(CInt(txtFuriDateM.Text), "00") & Format(CInt(txtFuriDateD.Text), "00")

        If Not IsDate(STR_FURIKAE_DATE(0)) Then
            Call GSUB_MESSAGE_WARNING("振替年月日が正しくありません")

            txtFuriDateY.Focus()

            Exit Function
        End If

        Str_Select_Nyusyutu_Code = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbNyusyutu)

        SQL.Length = 0
        SQL.Append(" SELECT ")
        SQL.Append(" *")
        SQL.Append(" FROM G_SCHMAST")
        SQL.Append(" WHERE GAKKOU_CODE_S = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
        SQL.Append(" AND FURI_DATE_S = '" & STR_FURIKAE_DATE(1) & "'")
        SQL.Append(" AND FURI_KBN_S = '" & Str_Select_Nyusyutu_Code & "'")

        If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
            Exit Function
        End If

        OBJ_DATAREADER.Read()

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            Call GSUB_MESSAGE_WARNING("スケジュールが存在しません")

            Exit Function
        End If

        sSch_Data_Flg = OBJ_DATAREADER.Item("DATA_FLG_S")
        sSch_Funou_Flg = OBJ_DATAREADER.Item("FUNOU_FLG_S")
        sSch_Tyudan_Flg = OBJ_DATAREADER.Item("TYUUDAN_FLG_S")
        sSch_Furi_Kbn = OBJ_DATAREADER.Item("FURI_KBN_S")
        Str_Seikyu_Nentuki = OBJ_DATAREADER.Item("NENGETUDO_S") '追加 2006/10/05

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        If CInt(sSch_Tyudan_Flg) <> 0 Then

            Call GSUB_MESSAGE_WARNING("このスケジュールは中断中です")

            Exit Function
        End If

        If CInt(sSch_Data_Flg) <> 1 Then
            Call GSUB_MESSAGE_WARNING("振替データが未作成です")

            Exit Function
        End If

        Str_Takou_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_TAKOU_TXT, cmbTakou)

        Select Case (CInt(Str_Takou_Kbn))
            Case 0
                '先に自行のみの不能結果更新を行ってしまうと
                '他行の更新ができなくなる為、ここで判定する
                If CInt(sSch_Funou_Flg) <> 0 Then
                    Call GSUB_MESSAGE_WARNING("不能結果更新済です")

                    Exit Function
                Else
                    '自行分の明細が存在するか確認 2006/10/16
                    SQL.Length = 0
                    SQL.Append(" SELECT * ")
                    SQL.Append(" FROM G_MEIMAST")
                    SQL.Append(" WHERE GAKKOU_CODE_M = '" & Trim(txtGAKKOU_CODE.Text) & "'")
                    SQL.Append(" AND FURI_KBN_M = '" & Str_Select_Nyusyutu_Code & "'")
                    SQL.Append(" AND FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'")
                    SQL.Append(" AND SEIKYU_KIN_M > 0")
                    SQL.Append(" AND TKIN_NO_M = '" & STR_JIKINKO_CODE & "'")

                    If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
                        If GFUNC_SELECT_SQL2("", 1) = False Then
                            Exit Function
                        End If
                        Exit Function
                    End If

                    OBJ_DATAREADER.Read()

                    If OBJ_DATAREADER.HasRows = False Then
                        If GFUNC_SELECT_SQL2("", 1) = False Then
                            Exit Function
                        End If

                        Call GSUB_MESSAGE_INFOMATION("更新対象明細(自行分)がありません" & vbCrLf & "不能フラグのみ更新します")
                        flgJIKOU_MEISAI_ZERO = True

                        Exit Function
                    End If

                    If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                        Exit Function
                    End If

                End If
            Case 1
                '他行スケジュールマスタチェック
                '選択した学校の他行区分が１(他行)の場合のみチェックを行う
                SQL.Length = 0
                SQL.Append(" SELECT ")
                SQL.Append(" FUNOU_FLG_U")
                SQL.Append(" FROM G_TAKOUSCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_U = '" & Trim(txtGAKKOU_CODE.Text) & "'")
                SQL.Append(" AND FURI_KBN_U = '" & Str_Select_Nyusyutu_Code & "'")
                SQL.Append(" AND FURI_DATE_U = '" & STR_FURIKAE_DATE(1) & "'")

                If GFUNC_SELECT_SQL2(SQL.ToString, 0) = False Then
                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Function
                    End If
                    Exit Function
                End If

                OBJ_DATAREADER.Read()

                If OBJ_DATAREADER.HasRows = False Then
                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Function
                    End If

                    Call GSUB_MESSAGE_WARNING("他行スケジュールが存在しません")

                    Exit Function
                End If

                If OBJ_DATAREADER.Item("FUNOU_FLG_U") = 1 Then
                    '確認メッセージ
                    If GFUNC_MESSAGE_QUESTION("不能結果更新済ですが、再度更新しますか？") <> vbOK Then
                        If GFUNC_SELECT_SQL2("", 1) = False Then
                            Exit Function
                        End If

                        Exit Function
                    End If
                End If

                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If

                '他行スケジュールが存在している(=処理件数・金額が０件０円でない)金庫コードを取得 2006/10/16
                STR_SQL = " SELECT TKIN_NO_U,sum(SYORI_KEN_U), sum(SYORI_KIN_U) "
                STR_SQL += " FROM G_TAKOUSCHMAST "
                STR_SQL += " WHERE GAKKOU_CODE_U = '" & Str_Gakkou_Code & "'"
                STR_SQL += " AND FURI_KBN_U = '" & Str_Select_Nyusyutu_Code & "'"
                STR_SQL += " AND FURI_DATE_U = '" & STR_FURIKAE_DATE(1) & "'"
                STR_SQL += " group by TKIN_NO_U"
                STR_SQL += " ORDER BY TKIN_NO_U"

                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Function
                    End If
                    Exit Function
                End If

                If OBJ_DATAREADER.HasRows = False Then
                    If GFUNC_SELECT_SQL2("", 1) = False Then
                        Exit Function
                    End If

                    Call GSUB_MESSAGE_WARNING("指定した学校の他行スケジュールが登録されていません")

                    Exit Function
                End If

                lCount = 0
                Lng_Takou_Count = 0

                While (OBJ_DATAREADER.Read = True)
                    With OBJ_DATAREADER
                        ReDim Preserve Str_Takou_FileName(1, lCount)
                        ReDim Preserve Str_Takou_BAITAI(lCount)

                        '指定学校に対する他行情報を取得
                        STR_SQL = " SELECT "
                        STR_SQL += " RFILE_NAME_V"
                        STR_SQL += ",TKIN_NO_V"
                        STR_SQL += ",GAKKOU_CODE_V"
                        STR_SQL += ",BAITAI_CODE_V"
                        STR_SQL += " FROM G_TAKOUMAST"
                        STR_SQL += " WHERE GAKKOU_CODE_V = '" & Str_Gakkou_Code & "'"
                        STR_SQL += " AND TKIN_NO_V = '" & .Item("TKIN_NO_U") & "'"
                        STR_SQL += " ORDER BY TKIN_NO_V"

                        If GFUNC_SELECT_SQL4(STR_SQL, 0) = False Then
                            If GFUNC_SELECT_SQL4("", 1) = False Then
                                Exit Function
                            End If
                            'CLOSE
                            If GFUNC_SELECT_SQL2("", 1) = False Then
                                Exit Function
                            End If

                            Exit Function
                        End If

                        If OBJ_DATAREADER_DREAD2.HasRows = False Then
                            If GFUNC_SELECT_SQL4("", 1) = False Then
                                Exit Function
                            End If

                            'CLOSE
                            If GFUNC_SELECT_SQL2("", 1) = False Then
                                Exit Function
                            End If

                            Call GSUB_MESSAGE_WARNING("指定した学校の他行マスタが登録されていません")

                            Exit Function
                        End If

                        OBJ_DATAREADER_DREAD2.Read()

                        With OBJ_DATAREADER_DREAD2
                            If Trim(ConvNullToString(.Item("RFILE_NAME_V"), "")) <> "" Then
                                Str_Takou_FileName(0, lCount) = Trim(.Item("RFILE_NAME_V"))
                            Else
                                Str_Takou_FileName(0, lCount) = "R" & .Item("GAKKOU_CODE_V") & .Item("TKIN_NO_V") & ".dat"
                            End If
                            Str_Takou_FileName(1, lCount) = .Item("TKIN_NO_V")

                            Str_Takou_BAITAI(lCount) = .Item("BAITAI_CODE_V")
                        End With

                        If GFUNC_SELECT_SQL4("", 1) = False Then
                            'CLOSE
                            If GFUNC_SELECT_SQL2("", 1) = False Then
                                Exit Function
                            End If
                            Exit Function
                        End If

                        lCount += 1

                    End With
                End While
                'CLOSE
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If

                If lCount = 0 Then
                    MessageBox.Show("更新対象明細(他行分)がありません", "不能結果更新", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Exit Function
                End If

                Lng_Takou_Count = lCount - 1


        End Select

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_Update_Schedule(ByVal aintFURI_KBN As Integer) As Boolean
        '引数追加　aintFURI_KBN=0：自行　1：他行 2006/10/05

        Dim iG_Flg(9) As Integer

        Dim lSch_Zumi(1) As Long
        Dim lSch_Funo(1) As Long
        Dim bG_Flg As Boolean

        bG_Flg = False

        PFUNC_Update_Schedule = False

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Function
        End If

        'スケジュールチェック
        '年間と特別で振替日が同一のスケジュールが存在する為
        'まずは存在するスケジュールのプライマリキー情報を全取得する
        'スケジュール区分と再振日（←振替日は同一の為、この２項目で集計することでスケジュール単位の集計が取得可能）
        STR_SQL = " SELECT "
        STR_SQL += " SCH_KBN_S"
        STR_SQL += ",SFURI_DATE_S"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " FURI_KBN_S ='" & Str_Select_Nyusyutu_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_S = '" & Str_Gakkou_Code & "'"
        '追加 2006/04/18
        If Str_Select_Nyusyutu_Code = 2 Or Str_Select_Nyusyutu_Code = 3 Then
            STR_SQL += " AND"
            STR_SQL += " SCH_KBN_S = '2'"
        Else
            STR_SQL += " AND"
            STR_SQL += " SCH_KBN_S <> '2'"
        End If
        STR_SQL += " group by SCH_KBN_S , SFURI_DATE_S"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            ReDim iG_Flg(9)
            blnGAK_FLG = False
            '取得したプライマリ情報で単一のスケジュールを取得
            '※集計するスケジュールの対象学年を取得する為
            If PFUNC_Get_Gakunen2(OBJ_DATAREADER_DREAD.Item("SCH_KBN_S"), OBJ_DATAREADER_DREAD.Item("SFURI_DATE_S"), iG_Flg) = False Then
                GoTo NextWhile
            End If

            lSch_Zumi(0) = 0
            lSch_Zumi(1) = 0

            lSch_Funo(0) = 0
            lSch_Funo(1) = 0

            For i As Integer = 1 To 9
                If iG_Flg(i) = 1 Then
                    lSch_Zumi(0) += Lng_GZumi(i, 0)
                    lSch_Zumi(1) += Lng_GZumi(i, 1)

                    lSch_Funo(0) += Lng_GFunou(i, 0)
                    lSch_Funo(1) += Lng_GFunou(i, 1)
                End If
            Next i

            lngSUMI_KEN_ALL = 0
            dblSUMI_KIN_ALL = 0
            lngFUNOU_KEN_ALL = 0
            dblFUNOU_KIN_ALL = 0
            bG_Flg = False

            STR_SQL = " SELECT "
            STR_SQL += "  sum(decode(FURIKETU_CODE_M,'0',1,0)) as SUM_FURIZUMIKENSUU"
            STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',SEIKYU_KIN_M,0)) as SUM_FURIZUMIKINGAKU"
            STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',0,1)) as SUM_FUNOUKENSUU"
            STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',0,SEIKYU_KIN_M)) as SUM_FUNOUKINGAKU"
            STR_SQL += ", GAKKOU_CODE_M"
            STR_SQL += " FROM G_MEIMAST"
            STR_SQL += " WHERE GAKKOU_CODE_M = '" & Str_Gakkou_Code & "'"
            STR_SQL += " AND FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"
            STR_SQL += " AND FURI_KBN_M = '" & Str_Select_Nyusyutu_Code & "'"
            If blnGAK_FLG = True Then
                STR_SQL += " AND ("
                For i As Integer = 1 To 9
                    If iG_Flg(i) = 1 Then
                        If bG_Flg = True Then
                            STR_SQL += " OR "
                        End If
                        STR_SQL += " GAKUNEN_CODE_M=" & i
                        bG_Flg = True

                    End If
                Next i
                STR_SQL += " )"
            End If

            STR_SQL += " group by GAKKOU_CODE_M "

            If GFUNC_SELECT_SQL4(STR_SQL, 0) = False Then
                Exit Function
            End If

            While (OBJ_DATAREADER_DREAD2.Read = True)
                With OBJ_DATAREADER_DREAD2
                    lngSUMI_KEN_ALL = .Item("SUM_FURIZUMIKENSUU")
                    dblSUMI_KIN_ALL = .Item("SUM_FURIZUMIKINGAKU")
                    lngFUNOU_KEN_ALL = .Item("SUM_FUNOUKENSUU")
                    dblFUNOU_KIN_ALL = .Item("SUM_FUNOUKINGAKU")
                End With

            End While

            If GFUNC_SELECT_SQL4(STR_SQL, 1) = False Then
                Exit Function
            End If

            '指定学年がALL0かどうかチェック 2006/12/26
            Dim blnSITEI_GAK As Boolean = False
            For i As Integer = 1 To 9
                If iG_Flg(i) = 1 Then
                    blnSITEI_GAK = True
                    Exit For
                End If
            Next i

            'スケジュールマスタ不能結果更新部分更新
            STR_SQL = " UPDATE  G_SCHMAST SET "
            STR_SQL += " FUNOU_DATE_S='" & Format(Now, "yyyyMMdd") & "'"
            If aintFURI_KBN = 0 Then '自行分更新時のみ不能フラグが立つ 2006/10/05
                STR_SQL += ",FUNOU_FLG_S='1'"
            End If

            If blnSITEI_GAK = True Then '指定学年が一つもない時は処理件数・金額まで全て0にする 2006/12/26
                STR_SQL += ",FURI_KEN_S=" & lngSUMI_KEN_ALL
                STR_SQL += ",FURI_KIN_S=" & dblSUMI_KIN_ALL
                STR_SQL += ",FUNOU_KEN_S=" & lngFUNOU_KEN_ALL
                STR_SQL += ",FUNOU_KIN_S=" & dblFUNOU_KIN_ALL
            Else
                STR_SQL += ",SYORI_KEN_S = 0"
                STR_SQL += ",SYORI_KIN_S = 0"
                STR_SQL += ",FURI_KEN_S = 0"
                STR_SQL += ",FURI_KIN_S = 0"
                STR_SQL += ",FUNOU_KEN_S = 0"
                STR_SQL += ",FUNOU_KIN_S = 0"
            End If
            'STR_SQL += ",FURI_KEN_S=" & lSch_Zumi(0)
            'STR_SQL += ",FURI_KIN_S=" & lSch_Zumi(1)
            'STR_SQL += ",FUNOU_KEN_S=" & lSch_Funo(0)
            'STR_SQL += ",FUNOU_KIN_S=" & lSch_Funo(1)
            STR_SQL += ",TIME_STAMP_S='" & Format(Now, "yyyyMMddHHmmss") & "'"
            'STR_SQL += " WHERE GAKKOU_CODE_S = '" & Str_Gakkou_Code & "'"
            'STR_SQL += " AND FURI_DATE_S = '" & STR_FURIKAE_DATE(1) & "'"
            'STR_SQL += " AND FURI_KBN_S = '" & Str_Select_Nyusyutu_Code & "'"

            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_S = '" & Str_Gakkou_Code & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_S = '" & STR_FURIKAE_DATE(1) & "'"
            STR_SQL += " AND"
            STR_SQL += " SFURI_DATE_S ='" & OBJ_DATAREADER_DREAD.Item("SFURI_DATE_S") & "'"
            STR_SQL += " AND"
            STR_SQL += " SCH_KBN_S = '" & OBJ_DATAREADER_DREAD.Item("SCH_KBN_S") & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_KBN_S = '" & Str_Select_Nyusyutu_Code & "'"

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 3) = False Then
                    Exit Function
                End If
                Exit Function
            End If

NextWhile:
        End While

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_Update_Schedule = True

    End Function
    Private Function PFUNC_Update_TakouSchedule(ByVal pTakou_Code As String) As Boolean

        PFUNC_Update_TakouSchedule = False

        '指定した他行で集計を行う
        STR_SQL = " SELECT "
        STR_SQL += "  sum(decode(FURIKETU_CODE_M,'0',1,0)) as SUM_FURIZUMIKENSUU"
        STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',SEIKYU_KIN_M,0)) as SUM_FURIZUMIKINGAKU"
        STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',0,1)) as SUM_FUNOUKENSUU"
        STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',0,SEIKYU_KIN_M)) as SUM_FUNOUKINGAKU"
        STR_SQL += ", GAKKOU_CODE_M"
        STR_SQL += ", TKIN_NO_M"
        STR_SQL += ", GAKUNEN_CODE_M"
        STR_SQL += " FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M = '" & Str_Gakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_M = '" & pTakou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_M = '" & Str_Select_Nyusyutu_Code & "'"
        STR_SQL += " group by GAKKOU_CODE_M , TKIN_NO_M , GAKUNEN_CODE_M"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
                Exit Function
            End If

            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                '他行スケジュールマスタ不能結果更新部分更新
                STR_SQL = " UPDATE  G_TAKOUSCHMAST SET "
                STR_SQL += " FUNOU_FLG_U='1'"
                STR_SQL += ",FURI_KEN_U=" & .Item("SUM_FURIZUMIKENSUU")
                STR_SQL += ",FURI_KIN_U=" & .Item("SUM_FURIZUMIKINGAKU")
                STR_SQL += ",FUNOU_KEN_U=" & .Item("SUM_FUNOUKENSUU")
                STR_SQL += ",FUNOU_KIN_U=" & .Item("SUM_FUNOUKINGAKU")
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_U = '" & Str_Gakkou_Code & "'"
                STR_SQL += " AND"
                STR_SQL += " TKIN_NO_U = '" & pTakou_Code & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN_U = " & .Item("GAKUNEN_CODE_M")
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_U = '" & STR_FURIKAE_DATE(1) & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_U = '" & Str_Select_Nyusyutu_Code & "'"
            End With

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
                    Exit Function
                End If

                Exit Function
            End If
        End While

        If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_Update_TakouSchedule = True

    End Function
    Private Function PFUNC_Update_Jisseki() As Boolean

        PFUNC_Update_Jisseki = False

        STR_SQL = " SELECT "
        STR_SQL += "  sum(decode(FURIKETU_CODE_M,'0',1,0)) as SUM_FURIZUMIKENSUU"
        STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',SEIKYU_KIN_M,0)) as SUM_FURIZUMIKINGAKU"
        STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',0,1)) as SUM_FUNOUKENSUU"
        STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',0,SEIKYU_KIN_M)) as SUM_FUNOUKINGAKU"
        STR_SQL += ", GAKKOU_CODE_M"
        STR_SQL += ", GAKUNEN_CODE_M"
        STR_SQL += " FROM G_MEIMAST"
        STR_SQL += " WHERE GAKKOU_CODE_M = '" & Str_Gakkou_Code & "'"
        STR_SQL += " AND FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND FURI_KBN_M = '" & Str_Select_Nyusyutu_Code & "'"
        STR_SQL += " group by GAKKOU_CODE_M , GAKUNEN_CODE_M"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
                Exit Function
            End If

            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)

            With OBJ_DATAREADER_DREAD
                '実績マスタ不能結果更新部分更新
                STR_SQL = " UPDATE  JISSEKIMAST SET "
                STR_SQL += " FURISUMI_KEN_F=" & .Item("SUM_FURIZUMIKENSUU")
                STR_SQL += ",FURISUMI_KIN_F=" & .Item("SUM_FURIZUMIKINGAKU")
                STR_SQL += ",FUNOU_KEN_F=" & .Item("SUM_FUNOUKENSUU")
                STR_SQL += ",FUNOU_KIN_F=" & .Item("SUM_FUNOUKINGAKU")

                STR_SQL += " WHERE GAKKOU_CODE_F = '" & .Item("GAKKOU_CODE_M") & "'"
                STR_SQL += " AND GAKUNEN_CODE_F = " & .Item("GAKUNEN_CODE_M")
                STR_SQL += " AND SEIKYU_NENGETU_F = '" & Str_Seikyu_Nentuki & "'"
                STR_SQL += " AND FURI_DATE_F = '" & STR_FURIKAE_DATE(1) & "'"
            End With

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
                    Exit Function
                End If
                Exit Function
            End If
        End While

        If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_Update_Jisseki = True

    End Function
    Private Function PFUNC_Update_Meisai() As Boolean

        PFUNC_Update_Meisai = False

        With gZENGIN_REC2
            '口座振替明細データの更新
            STR_SQL = " UPDATE  G_MEIMAST SET "
            STR_SQL += " FURIKETU_CODE_M=" & CInt(.ZG14)
            If .ZG14 <> "0" Then
                STR_SQL += ", SAIFURI_SUMI_M='0'"
            End If
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_M='" & Str_Gakkou_Code & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_KBN_M = '" & Str_Select_Nyusyutu_Code & "'"
            STR_SQL += " AND"
            STR_SQL += " TKIN_NO_M = '" & .ZG2 & "'"
            STR_SQL += " AND"
            STR_SQL += " TSIT_NO_M = '" & .ZG4 & "'"
            STR_SQL += " AND"
            STR_SQL += " TKAMOKU_M = '" & Format(CInt(.ZG7), "00") & "'"
            STR_SQL += " AND"
            STR_SQL += " TKOUZA_M = '" & .ZG8 & "'"
            STR_SQL += " AND"
            STR_SQL += " NENDO_M = '" & Mid(.ZG12, 1, 4) & "'"
            STR_SQL += " AND"
            STR_SQL += " TUUBAN_M = " & CInt(Mid(.ZG12, 5, 4))

            Select Case (CInt(Mid(.ZG12, 9, 2)))
                Case 4 To 12
                    Select Case (CInt(Mid(STR_FURIKAE_DATE(1), 5, 2)))
                        Case 4 To 12
                            STR_SQL += " AND"
                            STR_SQL += " SEIKYU_TUKI_M = '" & CStr(CInt(Mid(STR_FURIKAE_DATE(1), 1, 4))) & Mid(.ZG12, 9, 2) & "'"
                        Case 1 To 3
                            STR_SQL += " AND"
                            STR_SQL += " SEIKYU_TUKI_M = '" & CStr(CInt(Mid(STR_FURIKAE_DATE(1), 1, 4)) - 1) & Mid(.ZG12, 9, 2) & "'"
                    End Select
                Case 1 To 3
                    STR_SQL += " AND"
                    STR_SQL += " SEIKYU_TUKI_M = '" & Mid(STR_FURIKAE_DATE(1), 1, 4) & Mid(.ZG12, 9, 2) & "'"
            End Select
            STR_SQL += " AND"
            STR_SQL += " SEIKYU_TAISYOU_M = '" & Str_Seikyu_Nentuki & "'" '2006/12/25
            'STR_SQL += " SEIKYU_TAISYOU_M = '" & Mid(STR_FURIKAE_DATE(1), 1, 6) & "'"

        End With

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        PFUNC_Update_Meisai = True

    End Function
    Private Function PFUNC_File_Check_Update(ByVal pFilePath As String) As Boolean

        Dim iFileNo As Integer

        Dim lLineCount As Double

        Dim lTotal_Cnt As Double
        Dim lTotal_Kingaku As Double
        Dim lFurizumi_Cnt As Double
        Dim lFurizumi_Kingaku As Double
        Dim lFuriFunou_Cnt As Double
        Dim lFuriFunou_Kingaku As Double

        Dim sKbn As String

        PFUNC_File_Check_Update = False

        iFileNo = FreeFile()

        FileOpen(iFileNo, pFilePath, OpenMode.Random, , , 120)

        lLineCount = 0

        lTotal_Cnt = 0
        lFurizumi_Cnt = 0
        lFuriFunou_Cnt = 0

        lTotal_Kingaku = 0
        lFurizumi_Kingaku = 0
        lFuriFunou_Kingaku = 0

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        Do Until EOF(iFileNo)

            lLineCount += 1

            Try
                FileGet(iFileNo, gstrDATA, lLineCount)
                sKbn = gstrDATA.strDATA.Substring(0, 1)
            Catch EX As Exception

                Call GSUB_MESSAGE_WARNING("ファイルの読み込みに失敗しました。" & vbCrLf & "レコードNO：" & lLineCount)

                GoTo End_FileClose
            End Try

            Select Case (sKbn)
                Case "1"     'ヘッダーレコード
                    'データ区分(=1)
                    '種別コード
                    'コード区分
                    '振込依頼人コード
                    '振込依頼人名
                    '取扱日
                    '仕向銀行ｺｰﾄﾞ
                    '仕向銀行名
                    '仕向支店ｺｰﾄﾞ
                    '仕向支店名
                    '預金種目
                    '口座番号
                    'ダミー
                    FileGet(iFileNo, gZENGIN_REC1, lLineCount)

                    With gZENGIN_REC1
                        Select Case (CInt(Str_Select_Nyusyutu_Code))
                            Case 0, 1, 3
                                If Trim(.ZG2) <> "91" Then
                                    Call GSUB_MESSAGE_WARNING("種別コード異常" & vbCrLf & "ファイル種別コード：" & Trim(.ZG2) & vbCrLf & "入出金区分を確認してください")

                                    GoTo End_FileClose
                                End If
                            Case 2
                                If Trim(.ZG2) <> "21" Then
                                    Call GSUB_MESSAGE_WARNING("種別コード異常" & vbCrLf & "ファイル種別コード：" & Trim(.ZG2) & vbCrLf & "入出金区分を確認してください")

                                    GoTo End_FileClose
                                End If
                        End Select

                        '入力した振替日(MMDD)かどうかのチェック
                        If Mid(STR_FURIKAE_DATE(1), 5, 4) <> Trim(.ZG6) Then
                            Call GSUB_MESSAGE_WARNING("指定した振替日ではありません" & vbCrLf & "指定振替日：" & STR_FURIKAE_DATE(1).Substring(4, 2) & "月" & STR_FURIKAE_DATE(1).Substring(6, 2) & "日" & vbCrLf & "データ振替日：" & Trim(.ZG6).Substring(0, 2) & "月" & Trim(.ZG6).Substring(2, 2) & "日")

                            GoTo End_FileClose
                        End If

                        Select Case (CInt(Str_Takou_Kbn))
                            Case 0
                                '自行
                                '選択した学校の委託者コードかどうかのチェック
                                If Trim(Str_Itaku_Code) <> Trim(.ZG4) Then
                                    Call GSUB_MESSAGE_WARNING("指定した学校の委託者コードではありません" & vbCrLf & "委託者コード：" & Str_Itaku_Code & vbCrLf & "データ委託者コード：" & Trim(.ZG4))

                                    GoTo End_FileClose
                                End If

                                '自行銀行コードを設定しているかのチェック
                                If Trim(STR_JIKINKO_CODE) <> Trim(.ZG7) Then
                                    Call GSUB_MESSAGE_WARNING("指定した学校の金融機関(自行)コードではありません" & vbCrLf & "自金庫コード：" & STR_JIKINKO_CODE & vbCrLf & "データ金庫コード：" & Trim(.ZG7))

                                    GoTo End_FileClose
                                End If
                            Case 1
                                '他行
                                '他行マスタに登録されている委託者コードかどうかのチェック
                                STR_SQL = " SELECT "
                                STR_SQL += " ITAKU_CODE_V"
                                STR_SQL += " FROM "
                                STR_SQL += " G_TAKOUMAST"
                                STR_SQL += " WHERE"
                                STR_SQL += " GAKKOU_CODE_V = '" & Str_Gakkou_Code & "'"
                                STR_SQL += " AND"
                                STR_SQL += " TKIN_NO_V = '" & .ZG7 & "'"

                                If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
                                    GoTo End_FileClose
                                End If

                                If OBJ_DATAREADER_DREAD.HasRows = False Then
                                    If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
                                        GoTo End_FileClose
                                    End If

                                    Call GSUB_MESSAGE_WARNING("指定した学校の他行マスタに存在しません" & vbCrLf & "未登録他行コード：" & .ZG7)

                                    GoTo End_FileClose
                                End If

                                OBJ_DATAREADER_DREAD.Read()

                                If Trim(OBJ_DATAREADER_DREAD.Item("ITAKU_CODE_V")) <> Trim(.ZG4) Then
                                    Dim strERR_ITAKU As String = ""
                                    strERR_ITAKU = Trim(OBJ_DATAREADER_DREAD.Item("ITAKU_CODE_V"))
                                    If GFUNC_SELECT_SQL3("", 1) = False Then
                                        GoTo End_FileClose
                                    End If

                                    Call GSUB_MESSAGE_WARNING("指定した学校の委託者コードではありません" & vbCrLf & "委託者コード：" & strERR_ITAKU & vbCrLf & "データ委託者コード：" & Trim(.ZG4))

                                    GoTo End_FileClose
                                End If
                                '追加 2006/10/05
                                If Trim(Str_Takou_Ginko_Code) <> Trim(.ZG7) Then
                                    Call GSUB_MESSAGE_WARNING("指定したファイルの金融機関コードが違います" & vbCrLf & "指定金融機関コード：" & Trim(Str_Takou_Ginko_Code) & vbCrLf & "データ金融機関コード：" & Trim(.ZG7))

                                    GoTo End_FileClose
                                End If

                                If GFUNC_SELECT_SQL3("", 1) = False Then
                                    GoTo End_FileClose
                                End If
                        End Select

                        ''銀行マスタ存在チェック
                        'If PFUNC_Serch_Ginko(Trim(.ZG7), Trim(.ZG9)) = False Then
                        '    GoTo End_FileClose
                        'End If
                    End With
                Case "2"        'データレコード
                    'データ区分(=2)
                    '被仕向銀行番号
                    '被仕向銀行名　
                    '被仕向支店番号
                    '被仕向支店名
                    '手形交換所番号
                    '預金種目
                    '口座番号
                    '受取人
                    '振込金額
                    '新規コード
                    '顧客コード１
                    '顧客コード２
                    '振込指定区分
                    'ダミー
                    Call FileGet(iFileNo, gZENGIN_REC2, lLineCount)

                    Select Case (CInt(Str_Takou_Kbn))
                        Case 0 '自行
                            '追加 2006/10/12
                            If Trim(STR_JIKINKO_CODE) <> Trim(gZENGIN_REC2.ZG2) Then
                                Call GSUB_MESSAGE_WARNING("指定したファイルの金融機関コードが違います" & vbCrLf & "指定金融機関コード：" & Trim(STR_JIKINKO_CODE) & vbCrLf & "データ金融機関コード：" & Trim(gZENGIN_REC2.ZG2))
                                GoTo End_FileClose
                            End If
                        Case 1 '他行
                            '追加 2006/10/12
                            If Trim(Str_Takou_Ginko_Code) <> Trim(gZENGIN_REC2.ZG2) Then
                                Call GSUB_MESSAGE_WARNING("指定したファイルの金融機関コードが違います" & vbCrLf & "指定金融機関コード：" & Trim(Str_Takou_Ginko_Code) & vbCrLf & "データ金融機関コード：" & Trim(gZENGIN_REC2.ZG2))
                                GoTo End_FileClose
                            End If
                    End Select


                    '銀行マスタ存在チェック 2006/10/12
                    If PFUNC_Serch_Ginko(Trim(gZENGIN_REC2.ZG2), Trim(gZENGIN_REC2.ZG4)) = False Then
                        Call GSUB_MESSAGE_WARNING("更新ファイルの明細情報が不正です" & vbCrLf & "レコードNO：" & lLineCount & vbCrLf & "マスタ未登録金融機関　金庫コード：" & Trim(gZENGIN_REC2.ZG2) & " 支店コード：" & Trim(gZENGIN_REC2.ZG4))
                        GoTo End_FileClose
                    End If

                    '明細マスタ更新（取得したデータレコードが対象）
                    If PFUNC_Update_Meisai() = False Then
                        Call GSUB_MESSAGE_WARNING("不能結果更新に失敗しました" & vbCrLf & "レコードNO：" & lLineCount)
                        GoTo End_FileClose
                    End If

                    With gZENGIN_REC2
                        Select Case (Trim(.ZG14))
                            Case "0"
                                '振替済
                                Call PFUNC_Gakunen_Syukei(.ZG12, 0, .ZG10)

                                lFurizumi_Cnt += 1
                                lFurizumi_Kingaku += CDbl(.ZG10)
                            Case Else
                                '不能
                                Call PFUNC_Gakunen_Syukei(.ZG12, 1, .ZG10)

                                lFuriFunou_Cnt += 1
                                lFuriFunou_Kingaku += CDbl(.ZG10)
                        End Select

                        '合計
                        lTotal_Cnt += 1
                        lTotal_Kingaku += CDbl(.ZG10)
                    End With
                Case "8"        'トレーラレコード
                    'データ区分(=8)
                    '合計件数
                    '合計金額
                    '振替済件数
                    '振替済金額
                    '振替不能件数
                    '振替不能金額
                    'ダミー
                    Call FileGet(iFileNo, gZENGIN_REC8, lLineCount)

                    With gZENGIN_REC8
                        '合計件数チェック
                        If CDbl(.ZG2) <> lTotal_Cnt Then
                            Call GSUB_MESSAGE_WARNING("トレーラ：処理件数不一致")
                            GoTo End_FileClose
                        End If
                        '合計金額チェック
                        If CDbl(.ZG3) <> lTotal_Kingaku Then
                            Call GSUB_MESSAGE_WARNING("トレーラ：処理金額不一致")
                            GoTo End_FileClose
                        End If

                        '自行分＋他行分の件数・金額がセットされるからチェック外す 2006/10/05
                        ''合計件数チェック(スケジュールにある値)
                        'If CDbl(.ZG2) <> Lng_Syori(0) Then
                        '    Call GSUB_MESSAGE_WARNING( "スケジュールマスタ：処理件数不一致")
                        '    GoTo End_FileClose
                        'End If
                        ''合計金額チェック(スケジュールにある値)
                        'If CDbl(.ZG3) <> Lng_Syori(1) Then
                        '    Call GSUB_MESSAGE_WARNING( "スケジュールマスタ：処理金額不一致")
                        '    GoTo End_FileClose
                        'End If

                        '振替済件数チェック
                        If CDbl(.ZG4) <> lFurizumi_Cnt Then
                            Call GSUB_MESSAGE_WARNING("トレーラ：振替済件数不一致")
                            GoTo End_FileClose
                        End If
                        '振替済金額チェック
                        If CDbl(.ZG5) <> lFurizumi_Kingaku Then
                            Call GSUB_MESSAGE_WARNING("トレーラ：振替済金額不一致")
                            GoTo End_FileClose
                        End If

                        '振替不能件数チェック
                        If CDbl(.ZG6) <> lFuriFunou_Cnt Then
                            Call GSUB_MESSAGE_WARNING("トレーラ：不能件数不一致")
                            GoTo End_FileClose
                        End If
                        '振替不能金額チェック
                        If CDbl(.ZG7) <> lFuriFunou_Kingaku Then
                            Call GSUB_MESSAGE_WARNING("トレーラ：不能金額不一致")
                            GoTo End_FileClose
                        End If

                        Lng_Zumi(0) = lFurizumi_Cnt
                        Lng_Zumi(1) = lFurizumi_Kingaku

                        Lng_Funou(0) = lFuriFunou_Cnt
                        Lng_Funou(1) = lFuriFunou_Kingaku
                    End With
                Case "9"        'エンドレコード
                    'データ区分(=9)
                    'ダミー数
                    Call FileGet(iFileNo, gZENGIN_REC9, lLineCount)
            End Select
NEXT_DATA:
        Loop

        FileClose(iFileNo)

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_File_Check_Update = True

        Exit Function

End_FileClose:

        Call GFUNC_EXECUTESQL_TRANS("", 3)

        FileClose(iFileNo)

        btnUPDATE.Focus()

    End Function
    Private Function PFUNC_Serch_Ginko(ByVal pGinko_Code As String, ByVal pSiten_Code As String) As Boolean

        PFUNC_Serch_Ginko = False

        '銀行マスタ存在チェック
        If Trim(pGinko_Code) = "" Or Trim(pSiten_Code) = "" Then
            Exit Function
        End If

        STR_SQL = "SELECT KIN_NNAME_N FROM TENMAST "
        STR_SQL += " WHERE KIN_NO_N = '" & pGinko_Code & "'"
        STR_SQL += " AND SIT_NO_N = '" & pSiten_Code & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
                Exit Function
            End If

            Exit Function
        End If

        If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
            Exit Function
        End If

        PFUNC_Serch_Ginko = True

    End Function
    Private Function PFUNC_FD_Copy_TAKOU() As Integer

        Dim sBuff As String
        Dim oDlg As New OpenFileDialog

        PFUNC_FD_Copy_TAKOU = -1

        '--------------------
        'FD保存
        '--------------------

        Select Case (StrConv(Mid(Str_Takou_Path, 1, 1), vbProperCase))
            Case "A", "B"
                'ＦＤ読み取り処理
                '確認メッセージ表示
                If GFUNC_MESSAGE_QUESTION("ＦＤをセットして下さい。") <> vbOK Then
                    PFUNC_FD_Copy_TAKOU = 1
                    Exit Function
                End If
        End Select

        With oDlg
            '.Filter = STR_DLG_FILTER_NAME & " (" & STR_DLG_FILTER & ")|" & STR_DLG_FILTER
            .Filter = "すべてのファイル (*.*)|*.*"

            .FilterIndex = 1
            .InitialDirectory = Str_Takou_Path

            '.DefaultExt = STR_DEF_FILE_KBN
            .FileName = Str_Tak_FileName
            .Title = "[" & Str_Gakkou_Name.Trim & ":" & Str_Takou_Ginko_Code.Trim & "]不能データ更新"
            '.ShowDialog()
            If .ShowDialog = DialogResult.Cancel Then 'キャンセルのとき
                PFUNC_FD_Copy_TAKOU = 1
                Exit Function
            End If
            sBuff = .FileName
        End With

        If Dir(sBuff, vbNormal) = "" Then
            Call GSUB_MESSAGE_WARNING("不能ファイルが選択されませんでした")
            PFUNC_FD_Copy_TAKOU = 1
            Exit Function
        End If


        If Dir$(Str_KekkaTakou_Path.Substring(0, Str_KekkaTakou_Path.Length - 1), FileAttribute.Directory) = "" Then
            Call GSUB_MESSAGE_WARNING("他行全銀結果格納PATHが存在しません")

            Exit Function
        End If


        FileCopy(sBuff, Str_ZenginFileName)

        If Err.Number <> 0 Then
            'ファイル保存失敗
            Exit Function
        End If

        PFUNC_FD_Copy_TAKOU = 0

    End Function

    Private Function PFUNC_FD_Copy() As Integer

        Dim sBuff As String
        Dim oDlg As New OpenFileDialog

        PFUNC_FD_Copy = -1

        '--------------------
        'FD保存
        '--------------------

        Select Case (StrConv(Mid(Str_Jikou_Path, 1, 1), vbProperCase))
            Case "A", "B"
                'ＦＤ読み取り処理
                '確認メッセージ表示
                If GFUNC_MESSAGE_QUESTION("ＦＤをセットして下さい。") <> vbOK Then
                    PFUNC_FD_Copy = 1
                    Exit Function
                End If
        End Select

        With oDlg
            '.Filter = STR_DLG_FILTER_NAME & " (" & STR_DLG_FILTER & ")|" & STR_DLG_FILTER
            .Filter = "すべてのファイル (*.*)|*.*"

            .FilterIndex = 1
            .InitialDirectory = Str_Jikou_Path

            '.DefaultExt = STR_DEF_FILE_KBN
            .FileName = Str_Jikou_FileName
            .Title = "[" & Str_Gakkou_Name & "]不能データ更新"
            '.ShowDialog()
            If .ShowDialog = DialogResult.Cancel Then 'キャンセルのとき
                PFUNC_FD_Copy = 1
                Exit Function
            End If

            sBuff = .FileName
        End With

        If Dir(sBuff, vbNormal) = "" Then
            Call GSUB_MESSAGE_WARNING("不能ファイルが存在しません")

            Exit Function
        End If


        If Dir$(STR_DAT_PATH.Substring(0, STR_DAT_PATH.Length - 1), FileAttribute.Directory) = "" Then
            Call GSUB_MESSAGE_WARNING("自行全銀結果格納PATHが存在しません")

            Exit Function
        End If


        FileCopy(sBuff, Str_ZenginFileName)

        If Err.Number <> 0 Then
            'ファイル保存失敗
            Exit Function
        End If

        PFUNC_FD_Copy = 0

    End Function



    Private Function PFUNC_Gakunen_Syukei(ByVal pJyuyouka_No As String, _
                                          ByVal pIndex As Integer, _
                                          ByVal pKingaku As Long) As Boolean


        '全銀から取得した行データの需要家番号（顧客コード１）から
        'その生徒の学年を取得し学年ごとの集計値を取得する

        PFUNC_Gakunen_Syukei = False

        STR_SQL = " SELECT "
        STR_SQL += " GAKUNEN_CODE_O"
        STR_SQL += " FROM SEITOMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_O ='" & Str_Gakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " NENDO_O ='" & Mid(pJyuyouka_No, 1, 4) & "'"
        STR_SQL += " AND"
        STR_SQL += " TUUBAN_O = " & CInt(Mid(pJyuyouka_No, 5, 4))
        STR_SQL += " AND"
        STR_SQL += " TUKI_NO_O ='" & Mid(pJyuyouka_No, 9, 2) & "'"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If

            Exit Function
        End If

        OBJ_DATAREADER_DREAD.Read()

        With OBJ_DATAREADER_DREAD
            Select Case (pIndex)
                Case 0
                    '振替済
                    Lng_GZumi(.Item("GAKUNEN_CODE_O"), 0) += 1
                    Lng_GZumi(.Item("GAKUNEN_CODE_O"), 1) += pKingaku
                Case 1
                    '不能
                    Lng_GFunou(.Item("GAKUNEN_CODE_O"), 0) += 1
                    Lng_GFunou(.Item("GAKUNEN_CODE_O"), 1) += pKingaku
            End Select
        End With

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        PFUNC_Gakunen_Syukei = True

    End Function
    Private Function PFUNC_Get_Gakunen2(ByVal pSch_Kbn As String, _
                                        ByVal pSFuri_Date As String, _
                                        ByRef pSiyou_gakunen() As Integer) As Boolean

        Dim iMaxGakunen As Integer

        ReDim pSiyou_gakunen(9)

        PFUNC_Get_Gakunen2 = False

        '選択された学校に存在するスケジュールより
        '再振替日までを抽出条件とし、
        'スケジュール毎に集計をかける学年を取得する
        STR_SQL = " SELECT "
        STR_SQL += " SCH_KBN_S"
        For iLoopCount As Integer = 1 To 9
            STR_SQL += ", GAKUNEN" & iLoopCount & "_FLG_S"
            pSiyou_gakunen(iLoopCount) = 0
        Next iLoopCount
        STR_SQL += ", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " left join GAKMAST2 on "
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S ='" & Str_Gakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND"
        STR_SQL += " SFURI_DATE_S ='" & pSFuri_Date & "'"
        STR_SQL += " AND"
        STR_SQL += " CHECK_FLG_S ='1'"
        STR_SQL += " AND"
        STR_SQL += " TYUUDAN_FLG_S ='0'"
        STR_SQL += " AND SCH_KBN_S = '" & pSch_Kbn & "'"

        If GFUNC_SELECT_SQL4(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD2.HasRows = False Then
            If GFUNC_SELECT_SQL4("", 1) = False Then
                Exit Function
            End If

            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD2.Read)
            With OBJ_DATAREADER_DREAD2
                iMaxGakunen = CInt(.Item("SIYOU_GAKUNEN_T"))
                For iLoopCount As Integer = 1 To iMaxGakunen
                    Select Case (CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S")))
                        Case 1
                            pSiyou_gakunen(iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                            blnGAK_FLG = True
                    End Select
                Next iLoopCount
            End With
        End While

        If GFUNC_SELECT_SQL4("", 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Gakunen2 = True

    End Function

    Private Function PFUNC_Get_Kingaku() As Boolean

        PFUNC_Get_Kingaku = False

        Select Case (CInt(Str_Takou_Kbn))
            Case 0
                '自行
                STR_SQL = " SELECT "
                STR_SQL += " GAKKOU_CODE_S"
                STR_SQL += ",SUM(SYORI_KEN_S) AS KENSUU"
                STR_SQL += ",SUM(SYORI_KIN_S) AS KINGAKU"
                STR_SQL += " FROM G_SCHMAST"
                STR_SQL += " WHERE"
                STR_SQL += " FURI_KBN_S ='" & Str_Select_Nyusyutu_Code & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKKOU_CODE_S = '" & Str_Gakkou_Code & "'"
                '追加 2006/04/18
                If Str_Select_Nyusyutu_Code = 2 Or Str_Select_Nyusyutu_Code = 3 Then
                    STR_SQL += " AND"
                    STR_SQL += " SCH_KBN_S = '2'"
                Else
                    STR_SQL += " AND"
                    STR_SQL += " SCH_KBN_S <> '2'"
                End If
                STR_SQL += " group by GAKKOU_CODE_S"
            Case 1
                '他行
                STR_SQL = " SELECT "
                STR_SQL += " GAKKOU_CODE_U"
                STR_SQL += ",SUM(SYORI_KEN_U) AS KENSUU"
                STR_SQL += ",SUM(SYORI_KIN_U) AS KINGAKU"
                STR_SQL += " FROM G_TAKOUSCHMAST"
                STR_SQL += " WHERE"
                STR_SQL += " FURI_KBN_U ='" & Str_Select_Nyusyutu_Code & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_U ='" & STR_FURIKAE_DATE(1) & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKKOU_CODE_U = '" & Str_Gakkou_Code & "'"
                '追加 2006/04/18
                If Str_Select_Nyusyutu_Code = 2 Or Str_Select_Nyusyutu_Code = 3 Then
                    STR_SQL += " AND"
                    STR_SQL += " SCH_KBN_U = '2'"
                Else
                    STR_SQL += " AND"
                    STR_SQL += " SCH_KBN_U <> '2'"
                End If

                STR_SQL += " group by GAKKOU_CODE_U"
        End Select

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER.Read = True)
            Lng_Syori(0) = OBJ_DATAREADER.Item("KENSUU")
            Lng_Syori(1) = OBJ_DATAREADER.Item("KINGAKU")
        End While

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Kingaku = True

    End Function
    Private Function PFUNC_Get_Kingaku_Takou(ByVal pGinkoCode As String) As Boolean

        PFUNC_Get_Kingaku_Takou = False

        '他行
        STR_SQL = " SELECT "
        STR_SQL += " GAKKOU_CODE_U"
        STR_SQL += ",SUM(SYORI_KEN_U) AS KENSUU"
        STR_SQL += ",SUM(SYORI_KIN_U) AS KINGAKU"
        STR_SQL += " FROM G_TAKOUSCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " FURI_KBN_U ='" & Str_Select_Nyusyutu_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_U ='" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_U = '" & Str_Gakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_U = '" & pGinkoCode & "'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_U <> '2'"
        STR_SQL += " group by GAKKOU_CODE_U"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER.Read = True)
            Lng_Syori(0) = OBJ_DATAREADER.Item("KENSUU")
            Lng_Syori(1) = OBJ_DATAREADER.Item("KINGAKU")
        End While

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Kingaku_Takou = True

    End Function

    '追加 2006/10/05
    Public Function fn_SELECT_G_MEIMAST() As Boolean

        fn_SELECT_G_MEIMAST = False

        STR_SQL = " SELECT "
        STR_SQL += "  sum(decode(FURIKETU_CODE_M,'0',1,0)) as SUM_FURIZUMIKENSUU"
        STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',SEIKYU_KIN_M,0)) as SUM_FURIZUMIKINGAKU"
        STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',0,1)) as SUM_FUNOUKENSUU"
        STR_SQL += ", sum(decode(FURIKETU_CODE_M,'0',0,SEIKYU_KIN_M)) as SUM_FUNOUKINGAKU"
        STR_SQL += ", GAKKOU_CODE_M"
        STR_SQL += " FROM G_MEIMAST"
        STR_SQL += " WHERE GAKKOU_CODE_M = '" & Str_Gakkou_Code & "'"
        STR_SQL += " AND FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND FURI_KBN_M = '" & Str_Select_Nyusyutu_Code & "'"
        STR_SQL += " group by GAKKOU_CODE_M "

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                lngSUMI_KEN_ALL = .Item("SUM_FURIZUMIKENSUU")
                dblSUMI_KIN_ALL = .Item("SUM_FURIZUMIKINGAKU")
                lngFUNOU_KEN_ALL = .Item("SUM_FUNOUKENSUU")
                dblFUNOU_KIN_ALL = .Item("SUM_FUNOUKINGAKU")
            End With

        End While

        If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
            Exit Function
        End If

        fn_SELECT_G_MEIMAST = True

    End Function

#End Region

End Class
