' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFJMAIN110

#Region " 定数/変数 "

    '--------------------------------
    ' 共通関連項目
    '--------------------------------
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private MainDB As CASTCommon.MyOracle

    '--------------------------------
    ' LOG関連項目
    '--------------------------------
    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN110", "他行データ媒体書込画面")
    Private Const msgtitle As String = "他行データ媒体書込画面(KFJMAIN110)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    '--------------------------------
    ' INI関連項目
    '--------------------------------
    Friend Structure INI_INFO
        Dim COMMON_BAITAIWRITE As String             ' 媒体書込データ格納フォルダ
        Dim COMMON_FDDRIVE As String                 ' ＦＤ３．５ドライブ
    End Structure
    Private IniInfo As INI_INFO

    '--------------------------------
    ' 取引先情報
    '--------------------------------
    Friend Structure TORI_INFO
        Dim CodeKbn As String
        Dim OutFileName As String
        Dim FtranP As String
        Dim FtranP_IBM As String
    End Structure
    Private ToriInfo As TORI_INFO

#End Region

#Region " ロード "

    Private Sub KFJMAIN110_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "開始", "")

            '------------------------------------------------
            ' システム日付とユーザ名を表示
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '--------------------------------
            ' INI情報取得
            '--------------------------------
            If SetIniFIle() = False Then
                Exit Try
            End If

            '------------------------------------------------
            ' 画面初期設定
            '------------------------------------------------
            txtTorisCode.Text = ""
            txtTorifCode.Text = ""
            txtFuriDateY.Text = ""
            txtFuriDateY.Text = ""
            txtFuriDateY.Text = ""
            btnAction.Enabled = False
            cmbKinkoCode.Enabled = False
            btnReset.Enabled = False
            txtTorisCode.Focus()

            '--------------------------------
            ' 取引先コンボボックスの設定
            '--------------------------------
            Dim Jyoken As String = " AND TAKO_KBN_T = '1'"
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "成功", "")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "終了", "")
        End Try

    End Sub

#End Region

#Region " ボタン "

    '================================
    ' 検索ボタン
    '================================
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "検索", "開始", "")

            '------------------------------------------------
            ' テキストボックスの入力チェック
            '------------------------------------------------
            If CheckDisplayInput() = False Then
                Exit Try
            End If

            '------------------------------------------------
            ' 取引先情報チェック
            '------------------------------------------------
            ToriInfo.CodeKbn = ""
            ToriInfo.OutFileName = ""
            If CheckToriInfo(txtTorisCode.Text, txtTorifCode.Text, "") = False Then
                Exit Try
            End If

            '------------------------------------------------
            ' リエンタファイル検索処理
            '------------------------------------------------
            If GetFileInfo("Search", txtTorisCode.Text & txtTorifCode.Text, txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text) = False Then
                MessageBox.Show(MSG0370W.Replace("{0}", "他行データ検索"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            End If

            btnAction.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "検索", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "検索", "終了", "")
        End Try

    End Sub

    '================================
    ' 実行ボタン
    '================================
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "開始", "")

            If MessageBox.Show(String.Format(MSG0077I, "他行データ", "媒体書込処理"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Try
            End If

            '------------------------------------------------
            ' ファイル名構築用変数設定
            '------------------------------------------------
            Dim ToriCode As String = ""
            Dim FuriDate As String = ""
            Dim KinkoCode As String = ""
            ToriCode = txtTorisCode.Text & txtTorifCode.Text
            FuriDate = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            KinkoCode = cmbKinkoCode.SelectedItem

            '------------------------------------------------
            ' 取引先情報チェック
            '------------------------------------------------
            ToriInfo.CodeKbn = ""
            ToriInfo.OutFileName = ""
            If CheckToriInfo(txtTorisCode.Text, txtTorifCode.Text, KinkoCode) = False Then
                Exit Try
            End If

            '------------------------------------------------
            ' 入力・出力ファイル名構築
            '------------------------------------------------
            Dim InFileName As String = ""
            InFileName = "TAK_FD_" & ToriCode & "_" & FuriDate & "_" & KinkoCode

            '------------------------------------------------
            ' ファイル書込
            '------------------------------------------------
            Dim iRet As Integer = 0
            If BaitaiWrite(InFileName, ToriInfo.OutFileName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "失敗", "正媒体書込")
                iRet = -1
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "成功", "正媒体書込")
            End If

            If iRet = 0 Then
                If MessageBox.Show(MSG0061I, msgtitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                    If BaitaiWrite(InFileName, ToriInfo.OutFileName) = False Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "失敗", "副媒体書込")
                        iRet = -1
                    Else
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "成功", "副媒体書込")
                    End If
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "副媒体書込キャンセル")
                End If
            End If

            '------------------------------------------------
            ' 画面初期設定
            '------------------------------------------------
            cmbKinkoCode.Items.Clear()
            btnSearch.Enabled = True
            txtFuriDateY.Enabled = True
            txtFuriDateM.Enabled = True
            txtFuriDateD.Enabled = True
            btnReset.Enabled = False
            cmbKinkoCode.Enabled = False
            btnAction.Enabled = False
            txtFuriDateY.Focus()

            '------------------------------------------------
            ' リエンタファイル検索処理
            '------------------------------------------------
            If GetFileInfo("Submit", txtTorisCode.Text & txtTorifCode.Text, txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text) = False Then
                MessageBox.Show(MSG0370W.Replace("{0}", "他行データ検索"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            End If

            '------------------------------------------------
            ' 完了メッセージ表示
            '------------------------------------------------
            If iRet = 0 Then
                MessageBox.Show(String.Format(MSG0078I, "他行データの媒体書込"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "成功", "")
            Else
                MessageBox.Show(String.Format(MSG0080I, "他行データの媒体書込", "中断"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "")
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "終了", "")
        End Try

    End Sub

    '================================
    ' 取消ボタン
    '================================
    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取消", "開始", "")

            btnSearch.Enabled = True
            btnAction.Enabled = False
            btnReset.Enabled = False
            cmbKinkoCode.Enabled = False

            txtTorisCode.Text = ""
            txtTorifCode.Text = ""
            txtFuriDateY.Enabled = True
            txtFuriDateY.Text = ""
            txtFuriDateM.Enabled = True
            txtFuriDateM.Text = ""
            txtFuriDateD.Enabled = True
            txtFuriDateD.Text = ""

            cmbKinkoCode.Items.Clear()
            txtTorisCode.Focus()

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取消", "成功", "")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取消", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取消", "終了", "")
        End Try

    End Sub

    '================================
    ' 終了ボタン
    '================================
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "開始", "")

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "終了", "")
        End Try

    End Sub

#End Region

#Region " 関数(Function)"

    '================================
    ' INI情報取得
    '================================
    Private Function SetIniFIle() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "開始", "")

            IniInfo.COMMON_BAITAIWRITE = CASTCommon.GetFSKJIni("COMMON", "BAITAIWRITE")
            If IniInfo.COMMON_BAITAIWRITE.ToUpper = "ERR" OrElse IniInfo.COMMON_BAITAIWRITE = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "読込データ格納フォルダ", "COMMON", "BAITAIWRITE"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:書込データ格納フォルダ 分類:COMMON 項目:BAITAIWRITE")
                Return False
            End If

            IniInfo.COMMON_FDDRIVE = CASTCommon.GetFSKJIni("COMMON", "FDDRIVE")
            If IniInfo.COMMON_FDDRIVE.ToUpper = "ERR" OrElse IniInfo.COMMON_FDDRIVE = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "ＦＤドライブ", "COMMON", "FDDRIVE"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:ＦＤドライブ 分類:COMMON 項目:FDDRIVE")
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "終了", "")
        End Try
    End Function

    '================================
    ' 入力チェック
    '================================
    Private Function CheckDisplayInput() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "開始", "入力:" & txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text)

            '------------------------------------------------
            ' 取引先主コードチェック
            '------------------------------------------------
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If

            '------------------------------------------------
            ' 取引先副コードチェック
            '------------------------------------------------
            If txtTorifCode.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorifCode.Focus()
                Return False
            End If

            '------------------------------------------------
            ' 処理年チェック
            '------------------------------------------------
            ' 必須チェック
            If txtFuriDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            ' 処理月チェック
            '------------------------------------------------
            ' 必須チェック
            If txtFuriDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            ' 範囲チェック
            If Not (txtFuriDateM.Text >= 1 And txtFuriDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            ' 処理日チェック
            '------------------------------------------------
            ' 必須チェック
            If txtFuriDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            ' 桁数チェック
            If Not (txtFuriDateD.Text >= 1 And txtFuriDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            ' 日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "終了", "")
        End Try

    End Function

    '================================
    ' 取引先情報チェック
    '================================
    Private Function CheckToriInfo(ByVal TorisCode As String, ByVal TorifCode As String, ByVal KinkoCode As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者情報取得", "開始", "取引先:" & TorisCode & "-" & TorifCode & " / 金融機関コード:" & KinkoCode)

            MainDB = New CASTCommon.MyOracle
            SQL = New StringBuilder(128)

            '------------------------------------------------
            ' 取引先マスタチェック
            '------------------------------------------------
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & TorifCode & "'")
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                If GCom.NzStr(OraReader.Reader.Item("TAKO_KBN_T")) <> "1" Then
                    MessageBox.Show(String.Format(MSG0372W, "取引先コードの媒体", "他行データ作成対象"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先情報チェック", "失敗", "他行作成非対象")
                    Return False
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先情報チェック", "成功", "")
                End If
            Else
                MessageBox.Show(MSG0063W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先情報チェック", "失敗", "取引先マスタ該当なし")
                Return False
            End If
            OraReader.Close()

            '------------------------------------------------
            ' 他行マスタチェック
            '------------------------------------------------
            If KinkoCode <> "" Then
                SQL.Length = 0
                SQL.Append("SELECT")
                SQL.Append("     *")
                SQL.Append(" FROM")
                SQL.Append("     TORIMAST , TAKOUMAST")
                SQL.Append(" WHERE")
                SQL.Append("     TORIS_CODE_T = '" & TorisCode & "'")
                SQL.Append(" AND TORIF_CODE_T = '" & TorifCode & "'")
                SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_V")
                SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_V")
                SQL.Append(" AND TKIN_NO_V    = '" & KinkoCode & "'")
                OraReader = New CASTCommon.MyOracleReader(MainDB)
                If OraReader.DataReader(SQL) = True Then
                    If GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_V")) <> "01" Then
                        MessageBox.Show(String.Format(MSG0372W, "取引先コードの媒体", "他行データ書込の対象"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先情報チェック", "失敗", "媒体区分対象外")
                        Return False
                    Else
                        ToriInfo.CodeKbn = GCom.NzStr(OraReader.Reader.Item("CODE_KBN_V"))
                        ToriInfo.OutFileName = GCom.NzStr(OraReader.Reader.Item("SFILE_NAME_V"))
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先情報チェック", "成功", "")
                    End If
                Else
                    MessageBox.Show(MSG0070W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先情報チェック", "失敗", "他行マスタ該当なし")
                    Return False
                End If
                OraReader.Close()
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先情報チェック", "", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先情報チェック", "終了", "")
        End Try
    End Function

    '================================
    ' 他行データ情報取得
    '================================
    Private Function GetFileInfo(ByVal Info As String, ByVal ToriCode As String, ByVal FuriDate As String) As Boolean

        Dim KinkoCode As String = ""

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "他行データ情報取得", "開始", "処理日:" & FuriDate)

            '------------------------------------------------
            ' コンボボックスの初期化
            '------------------------------------------------
            cmbKinkoCode.Items.Clear()

            '------------------------------------------------
            ' 再描画停止
            '------------------------------------------------
            cmbKinkoCode.BeginUpdate()

            '------------------------------------------------
            ' コンボボックス設定
            '------------------------------------------------
            Dim FileList() As String = Directory.GetFiles(IniInfo.COMMON_BAITAIWRITE)
            For i As Integer = 0 To FileList.Length - 1 Step 1
                Dim SetFileName() As String = Path.GetFileName(FileList(i)).Split("_"c)
                If SetFileName.Length = 5 Then
                    If SetFileName(0) = "TAK" And _
                        SetFileName(1) = "FD" And _
                        SetFileName(2) = ToriCode And _
                        SetFileName(3) = FuriDate Then
                        KinkoCode = SetFileName(4)
                        cmbKinkoCode.Items.Add(KinkoCode)
                    End If
                End If
            Next

            '------------------------------------------------
            ' 再描画再開
            '------------------------------------------------
            cmbKinkoCode.EndUpdate()

            '------------------------------------------------
            ' 画面設定
            '------------------------------------------------
            If cmbKinkoCode.Items.Count = 0 Then
                '--------------------------------------------
                ' 検索時はメッセージ出力
                '--------------------------------------------
                If Info = "Search" Then
                    MessageBox.Show(MSG0255W.Replace("{0}ファイル", "他行データ"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                btnAction.Enabled = False
                txtTorisCode.Focus()
            Else
                btnAction.Enabled = True
                btnSearch.Enabled = False
                txtFuriDateD.Enabled = False
                txtFuriDateM.Enabled = False
                txtFuriDateY.Enabled = False
                cmbKinkoCode.Enabled = True
                btnReset.Enabled = True
                cmbKinkoCode.SelectedIndex = 0
                cmbKinkoCode.Focus()
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "他行データ情報取得", "開始", ex.Message)

            cmbKinkoCode.EndUpdate()
            btnAction.Enabled = False
            txtFuriDateY.Focus()

            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "他行データ情報取得", "開始", "")
        End Try

    End Function

    '================================
    ' 媒体書込
    '================================
    Private Function BaitaiWrite(ByVal InFileName As String, ByVal OutFileName As String) As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "開始", "")

            '------------------------------------
            '媒体に書き込み
            '------------------------------------
            Dim FtranP As String = ""
            Select Case ToriInfo.CodeKbn
                Case "4"
                    FtranP = "120IBM.P"
                Case "0"
                    FtranP = "120JIS→JIS.P"
                Case Else
                    FtranP = "120JIS→JIS.P"
            End Select

            Dim intKEKKA As Integer = clsFUSION.fn_DISK_CPYTO_FD(txtTorisCode.Text & txtTorifCode.Text, _
                                                                 Path.Combine(IniInfo.COMMON_BAITAIWRITE, InFileName), _
                                                                 OutFileName, _
                                                                 120, _
                                                                 ToriInfo.CodeKbn, FtranP, False, msgtitle)

            Select Case intKEKKA
                Case 100
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "ＦＤ書込み失敗（IBM形式）")
                    Return False
                Case 200
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "ＦＤ書込み失敗（DOS形式）")
                    Return False
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "成功 ", "ＦＤ書込み")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "終了", "")
        End Try

    End Function

#End Region

#Region " 関数(Sub) "

    '================================
    ' 取引先(コンボボックス)を取得
    '================================
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                Dim Jyoken As String = " AND TAKO_KBN_T = '1'"
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If

            cmbToriName.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先コンボボックス設定", "", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

    '================================
    ' 取引先(コンボボックス)選択処理
    '================================
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged

        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先コード設定", "", ex.Message)
        Finally
            ' NOP
        End Try

    End Sub

    '================================
    ' ゼロ埋め
    '================================
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
                Handles txtTorisCode.Validating, txtTorifCode.Validating, _
                        txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

End Class
' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END
