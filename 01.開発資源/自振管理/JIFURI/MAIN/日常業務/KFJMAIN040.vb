Imports System.Data.OracleClient
Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJMAIN040
    Inherits System.Windows.Forms.Form
    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN040", "不能結果更新画面")
    Private Const msgTitle As String = "不能結果更新画面(KFJMAIN040)"
    Private Const errMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private DenFolder As String
    Private Center As String '2009/12/17 追加
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Public strFURI_DATE As String

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private KFJMAIN041 As New KFJMAIN041
    Private KFJMAIN042 As New KFJMAIN042
    Private KFJMAIN043 As New KFJMAIN043
#Region " ロード"
    Private Sub KFJMAIN040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '#####################################
        'ログの書込に必要な情報の取得
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------
            'INIファイルの読み込み
            '------------------------------------
            If fn_INI_Read() = False Then
                Return
            End If

            '休日マスタ取り込み
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)終了", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時に振替日に前営業日を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strSysDate As String
            Dim strGetdate As String = ""

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            bRet = GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)
            '休日に営業日取得を行うとFalseが戻ってくるためコメント化
            'If bRet = False Then
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(前営業日取得)終了", "失敗", "")
            '    MessageBox.Show(MSG0012E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If
            '===================================================
            txtFuriDateY.Text = strGetdate.Substring(0, 4)
            txtFuriDateM.Text = strGetdate.Substring(4, 2)
            txtFuriDateD.Text = strGetdate.Substring(6, 2)

            '2010/12/24 信組対応 ここから
            Dim MotikomiText As String
            If Center = "0" Then
                MotikomiText = "KFJMAIN040_持込区分(信組).TXT"
            Else
                MotikomiText = "KFJMAIN040_持込区分.TXT"
            End If
            '2010/12/24 信組対応 ここまで

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'コンボボックスを表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim Msg As String
            '2010/12/24 信組対応 ここから
            'Select Case GCom.SetComboBox(cmbMotikomiKbn, "KFJMAIN040_持込区分.TXT", True)
            '    Case 1  'ファイルなし
            '        Msg = String.Format(MSG0025E, "持込区分", "KFJMAIN040_持込区分.TXT")
            '        MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '        Msg = "持込区分設定ファイルなし。ファイル:KFJMAIN040_持込区分.TXT"
            Select Case GCom.SetComboBox(cmbMotikomiKbn, MotikomiText, True)
                Case 1  'ファイルなし
                    Msg = String.Format(MSG0025E, "持込区分", MotikomiText)
                    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "持込区分設定ファイルなし。ファイル:" & MotikomiText
                    '2010/12/24 信組対応 ここまで
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
                Case 2  '異常
                    Msg = MSG0026E.Replace("{0}", "持込区分")
                    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "コンボボックス設定失敗 コンボボックス名:持込区分"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
#End Region
#Region " 更新"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :不能結果更新実行ボタン
        'Return         :
        'Create         :2009/09/09
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '必須項目の入力値チェック
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '必須項目：振替日
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            LW.FuriDate = strFURI_DATE
            Select Case GCom.GetComboBox(cmbMotikomiKbn)
                Case 0, 3 '金庫持込、センター直接持込
                    If fn_check_Table() = False Then
                        Exit Sub
                    End If
            End Select

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '不能結果更新の種類選択
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Select Case GCom.GetComboBox(cmbMotikomiKbn)
                Case 0       '金庫持込
                    bUpdateKinko()
                Case 1       '企業持込
                    KFJMAIN041 = New KFJMAIN041
                    KFJMAIN041.txtFuriDateY.Text = txtFuriDateY.Text
                    KFJMAIN041.txtFuriDateM.Text = txtFuriDateM.Text
                    KFJMAIN041.txtFuriDateD.Text = txtFuriDateD.Text
                    KFJMAIN041.txtTorisCode.Focus()
                    Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN041, Form), Me)
                    Exit Sub
                Case 2       '他行媒体
                    KFJMAIN042 = New KFJMAIN042
                    KFJMAIN042.txtFuriDateY.Text = txtFuriDateY.Text
                    KFJMAIN042.txtFuriDateM.Text = txtFuriDateM.Text
                    KFJMAIN042.txtFuriDateD.Text = txtFuriDateD.Text
                    KFJMAIN042.txtTorisCode.Focus()
                    Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN042, Form), Me)
                    Exit Sub
                Case 3       'センター直接持込
                    KFJMAIN043 = New KFJMAIN043
                    KFJMAIN043.lblFuriDateY.Text = txtFuriDateY.Text
                    KFJMAIN043.lblFuriDateM.Text = txtFuriDateM.Text
                    KFJMAIN043.lblFuriDateD.Text = txtFuriDateD.Text
                    KFJMAIN043.txtSyoriKen1.Focus()
                    Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN043, Form), Me)
                    Exit Sub
                Case 4 'SSS用
                    'bUpdateSSS()
                    Exit Sub
            End Select
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
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
    Public Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :更新ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/09
        'Update         :
        '============================================================================
        Try
            fn_check_text = False
            '年必須チェック
            If txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
            '月必須チェック
            If txtFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If
            '日付必須チェック
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        Finally

        End Try
        fn_check_text = True
    End Function
    Private Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :更新ボタンを押下時にマスタ相関チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/09
        'Update         :
        '============================================================================
        fn_check_Table = False
        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        fn_check_Table = False
        Try
            Dim lngDataCNT As Long = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" FSYORI_KBN_T = '1'")
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_S =" & SQ(strFURI_DATE))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            Select Case GCom.GetComboBox(cmbMotikomiKbn)
                Case 0  '金庫持込
                    '2009/12/17 東海センターの場合持込区分は無視
                    'SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    If Center <> "4" Then
                        SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    End If
                    '==========================================
                    SQL.Append(" AND HAISIN_FLG_S = '1'")
                Case 3  'センター直接持込
                    SQL.Append(" AND MOTIKOMI_KBN_S = '1'")
            End Select

            If OraReader.DataReader(SQL) = True Then
                lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0068W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
            fn_check_Table = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function
    Public Function fn_INI_Read() As Boolean
        '============================================================================
        'NAME           :fn_INI_Read
        'Parameter      :
        'Description    :FSKJ.INIファイルの読み込み
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/09
        'Update         :
        '============================================================================
        fn_INI_Read = False
        'DAT格納フォルダ
        DenFolder = CASTCommon.GetFSKJIni("COMMON", "DEN")
        If DenFolder.ToUpper = "ERR" OrElse DenFolder = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "DEN格納フォルダ", "COMMON", "DEN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        '2009/12/17 センターコード追加 ======================
        Center = CASTCommon.GetFSKJIni("COMMON", "CENTER")
        If Center.ToUpper = "ERR" OrElse Center = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "センターコード", "COMMON", "CENTER"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        '====================================================
        fn_INI_Read = True
    End Function
    Private Function bUpdateKinko() As Boolean
        '=====================================================================================
        'NAME           :bUpdateKinko
        'Parameter      :
        'Description    :金庫持込更新のジョブ登録
        'Return         :
        'Create         :2009/09/09
        'Update         :
        '=====================================================================================
        Dim MSG As String = ""
        Try
            If Not File.Exists(IO.Path.Combine(DenFolder, "FUNOU.DAT")) Then
                '更新前確認メッセージ
                MSG = MSG0052I
                If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return False
                Else
                    Dim FW As New StreamWriter(IO.Path.Combine(DenFolder, "FUNOU.DAT"))
                    FW.Close()
                End If
            Else
                '更新前確認メッセージ
                MSG = MSG0023I.Replace("{0}","不能結果更新処理")
                If MessageBox.Show(MSG, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return False
                End If
            End If

            '------------------------------------------------
            'ジョブマスタに登録
            '------------------------------------------------
            Dim jobid As String
            jobid = "J040"      '..\Batch\結果更新\
            Dim para As String
            Dim FuriDate As String = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            If rdbKIGYO_SEQ.Checked = True Then     ' 更新キー項目が企業シーケンス
                para = FuriDate & ",0,0"
            Else
                para = FuriDate & ",0,1"   ' 更新キーが口座情報
            End If

            MainDB = New CASTCommon.MyOracle
            MainDB.BeginTrans()

            '#########################
            'job検索
            '#########################
            Dim iRet As Integer
            iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            If iRet = 1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            ElseIf iRet = -1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブマスタ検索失敗 パラメータ:" & para)
                Return False
            End If

            '#########################
            'job登録
            '#########################
            If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then
                MainDB.Rollback()
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブ登録失敗 パラメータ:" & para)
                Return False
            End If
            MainDB.Commit()

            'ジョブ登録完了メッセージ
            '2010/12/24 信組対応
            If Center = "0" Then
                MSG = MSG0021I.Replace("{0}", "不能結果更新(組合持込)")
            Else
                MSG = MSG0021I.Replace("{0}", "不能結果更新(金庫持込)")
            End If
            MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return True
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '2010/12/24 信組対応
            If Center = "0" Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(組合持込ジョブ登録)", "失敗", ex.ToString)
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(金庫持込ジョブ登録)", "失敗", ex.ToString)
            End If
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Function
#End Region
#Region " イベント"
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
              Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub
#End Region
#Region " SSS"
    'Private Function bUpdateSSS() As Boolean
    '    '============================================================================
    '    'NAME           :bUpdateSSS
    '    'Parameter      :
    '    'Description    :ＳＳＳ持込更新
    '    'Return         :
    '    'Create         :2005/06/14
    '    'Update         :
    '    '============================================================================
    '    Dim MSG As String = "不能結果更新処理を登録します" & Environment.NewLine & _
    '                  "よろしいですか？"
    '    If MessageBox.Show(MSG, gstrSYORI_R, MessageBoxButtons.YesNo, MessageBoxIcon.Information) <> DialogResult.Yes Then
    '        Exit Function
    '    End If

    '    '------------------------------------------------
    '    'ジョブマスタに登録
    '    '------------------------------------------------
    '    gastrJOB_ID = "J040"

    '    Dim FuriDate As String = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
    '    If rdbKIGYO_SEQ.Checked = True Then     ' 更新キー項目が企業シーケンス
    '        gastrPARAMETA = "20,99,0," & FuriDate
    '    Else
    '        gastrPARAMETA = "20,99,1," & FuriDate       ' 更新キー項目が口座情報
    '    End If

    '    If clsFUSION.fn_JOBMAST_TOUROKU_CHECK(gastrJOB_ID, gstrUSER_ID, gastrPARAMETA) = False Then
    '        gdbcCONNECT.Close()
    '        Return False
    '    End If
    '    If clsFUSION.fn_INSERT_JOBMAST(gastrJOB_ID, gstrUSER_ID, gastrPARAMETA) = False Then
    '        fn_LOG_WRITE(gstrUSER_ID, gastrTORI_CODE_MAIN0100, gastrFURI_DATE_MAIN0100, gstrSYORI_R, "パラメータ登録", "失敗", Err.Description)
    '    Else
    '        MessageBox.Show("起動パラメタを登録しました", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Information)
    '        fn_LOG_WRITE(gstrUSER_ID, gastrTORI_CODE_MAIN0100, gastrFURI_DATE_MAIN0100, gstrSYORI_R, "パラメータ登録", "成功", Err.Description)
    '        Return False
    '    End If
    '    Return True
    'End Function
    'Public Function fn_INI_SSSREAD() As Boolean
    '    '============================================================================
    '    'NAME           :fn_INI_SSSREAD
    '    'Parameter      :
    '    'Description    :FSKJ.INIファイルの読み込み
    '    'Return         :True=OK(成功),False=NG（失敗）
    '    'Create         :2004/08/26
    '    'Update         :
    '    '============================================================================
    '    fn_INI_SSSREAD = False
    '    gstrIFileName = CurDir()     'カレントディレクトリの取得
    '    gstrIFileName = gstrIFileName & "\FSKJ.INI"
    '    gstrIAppName = "COMMON"
    '    gstrIKeyName = "LOG"
    '    gstrIDefault = "err"
    '    gintTEMP_LEN = 0
    '    gstrTEMP = Space(100)

    '    gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
    '    If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
    '        MessageBox.Show("LOGディレクトリ取得に失敗しました", gstrSYORI_R)
    '        fn_LOG_WRITE(gstrUSER_ID, "", strFURI_DATE, gstrSYORI_R, "LOGディレクトリ取得", "失敗", Err.Description)
    '        Exit Function
    '    Else
    '        '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
    '        gstrLOG_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
    '    End If

    '    gstrIFileName = CurDir()     'カレントディレクトリの取得
    '    gstrIFileName = gstrIFileName & "\FSKJ.INI"
    '    gstrIAppName = "COMMON"
    '    gstrIKeyName = "TXT"
    '    gstrIDefault = "err"
    '    gintTEMP_LEN = 0
    '    gstrTEMP = Space(100)

    '    gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
    '    If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
    '        MessageBox.Show("TXTディレクトリ取得に失敗しました", gstrSYORI_R)
    '        fn_LOG_WRITE(gstrUSER_ID, "", strFURI_DATE, gstrSYORI_R, "TXTディレクトリ取得", "失敗", Err.Description)
    '        Exit Function
    '    Else
    '        '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
    '        gstrTXT_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
    '    End If

    '    gstrIFileName = CurDir()     'カレントディレクトリの取得
    '    gstrIFileName = gstrIFileName & "\FSKJ.INI"
    '    gstrIAppName = "COMMON"
    '    gstrIKeyName = "DEN"
    '    gstrIDefault = "err"
    '    gintTEMP_LEN = 0
    '    gstrTEMP = Space(100)

    '    gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
    '    If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
    '        MessageBox.Show("DENディレクトリ取得に失敗しました", gstrSYORI_R)
    '        fn_LOG_WRITE(gstrUSER_ID, "", strFURI_DATE, gstrSYORI_R, "DENディレクトリ取得", "失敗", Err.Description)
    '        Exit Function
    '    Else
    '        '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
    '        gstrDEN_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
    '    End If

    '    gstrIFileName = CurDir()     'カレントディレクトリの取得
    '    gstrIFileName = gstrIFileName & "\FSKJ.INI"
    '    gstrIKeyName = "DENBK"
    '    gstrIDefault = "err"
    '    gintTEMP_LEN = 0
    '    gstrTEMP = Space(100)

    '    gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
    '    If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
    '        MessageBox.Show("DENBKディレクトリ取得に失敗しました", gstrSYORI_R)
    '        fn_LOG_WRITE(gstrUSER_ID, "", strFURI_DATE, gstrSYORI_R, "DENBKディレクトリ取得", "失敗", Err.Description)
    '        Exit Function
    '    Else
    '        '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
    '        gstrDENBK_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
    '    End If


    '    gstrIFileName = CurDir()     'カレントディレクトリの取得
    '    gstrIFileName = gstrIFileName & "\FSKJ.INI"
    '    gstrIKeyName = "DATBK"
    '    gstrIDefault = "err"
    '    gintTEMP_LEN = 0
    '    gstrTEMP = Space(100)

    '    gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
    '    If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
    '        MessageBox.Show("DATBKディレクトリ取得に失敗しました", gstrSYORI_R)
    '        fn_LOG_WRITE(gstrUSER_ID, "", strFURI_DATE, gstrSYORI_R, "DATBKディレクトリ取得", "失敗", Err.Description)
    '        Exit Function
    '    Else
    '        '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
    '        gstrDATBK_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
    '    End If


    '    gstrIFileName = CurDir()     'カレントディレクトリの取得
    '    gstrIFileName = gstrIFileName & "\FSKJ.INI"
    '    gstrIKeyName = "FTR"
    '    gstrIDefault = "err"
    '    gintTEMP_LEN = 0
    '    gstrTEMP = Space(100)

    '    gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
    '    If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
    '        MessageBox.Show("FTRディレクトリ参照に失敗しました", gstrSYORI_R)
    '        fn_LOG_WRITE(gstrUSER_ID, "", strFURI_DATE, gstrSYORI_R, "FTRディレクトリ取得", "失敗", Err.Description)
    '        Exit Function
    '    Else
    '        '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
    '        gstrFTR_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
    '    End If

    '    gstrIFileName = CurDir()     '自金庫コード取得
    '    gstrIFileName = gstrIFileName & "\FSKJ.INI"
    '    gstrIAppName = "COMMON"
    '    gstrIKeyName = "KINKOCD"
    '    gstrIDefault = "err"
    '    gintTEMP_LEN = 0
    '    gstrTEMP = Space(100)

    '    gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
    '    If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
    '        MessageBox.Show("自金庫コード取得に失敗しました", gstrSYORI_R)
    '        fn_LOG_WRITE(gstrUSER_ID, "", strFURI_DATE, gstrSYORI_R, "自金庫コード取得", "失敗", Err.Description)
    '        Exit Function
    '    Else
    '        '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
    '        gstrJIKINKO = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
    '    End If

    '    fn_INI_SSSREAD = True

    'End Function

#End Region

End Class