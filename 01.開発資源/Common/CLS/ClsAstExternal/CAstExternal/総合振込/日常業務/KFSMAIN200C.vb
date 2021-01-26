'2017/09/05 タスク）綾部 ADD 【PG】青梅信金 カスタマイズ対応(UI_5-2) -------------------- START
Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text
Imports CASTCommon
Imports System.Windows.Forms

Public Class KFSMAIN200C

#Region "変数宣言"

    Private MainLOG As New CASTCommon.BatchLOG("KFSMAIN200C", "基本料金データ作成画面")
    Private Const msgTitle As String = "基本料金データ作成画面(KFSMAIN200C)"

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private Structure LogWrite
        Dim UserID As String            ' ユーザID
        Dim ToriCode As String          ' 取引先主副コード
        Dim FuriDate As String          ' 振替日
    End Structure
    Private LW As LogWrite

    Private MainDB As CASTCommon.MyOracle

    Private Structure JobInfo
        Dim JobID As String             ' ジョブＩＤ
        Dim ToriCode As String          ' 取引先主副コード
        Dim FuriDate As String          ' 振替日
        Dim CodeKbn As String           ' コード区分
        Dim FmtKbn As String            ' フォーマット区分
        Dim BaitaiCode As String        ' 媒体コード
        Dim LabelKbn As String          ' ラベル区分
    End Structure
    Private Param As JobInfo

    Private Structure IniInfo
        Dim KijunShiftDate As String    ' 基準日数
        Dim TesuuTorisCode As String    ' 基本料金徴求取引先(主コード)
        Dim TesuuTorifCode As String    ' 基本料金徴求取引先(副コード)
        Dim TesuuItakuCode As String    ' 基本料金データ委託者コード
        Dim TesuuItakuName As String    ' 基本料金データ委託者名
        Dim TesuuShitenCode As String   ' 基本料金データ支店コード
        Dim TesuuShitenName As String   ' 基本料金データ支店名
        Dim TesuuFileName As String     ' 基本料金データ出力ファイル名
        '2017/12/28 FJH）向井 ADD 青梅信金(基本料金関連チェック) ---------------------------------------- START
        Dim TesuuBaitaiCode As String
        '2017/12/28 FJH）向井 ADD 青梅信金(基本料金関連チェック) ---------------------------------------- END
        Dim JikinkoCode As String       ' 自金庫コード
        Dim JikinkoName As String       ' 自金庫名
    End Structure
    Private Ini As IniInfo

#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFSMAIN200C_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '------------------------------------------
            ' 【拡張処理用】ログインIDを取得
            '------------------------------------------
            Dim Args As String() = Environment.GetCommandLineArgs
            GCom = New MenteCommon.clsCommon

            '------------------------------------------
            ' 【拡張処理用】ログインID、日付設定
            '------------------------------------------
            If Args.Length <= 1 Then
                GCom.GetUserID = ""
            Else
                GCom.GetUserID = Args(1).Trim
            End If
            GCom.GetSysDate = Date.Now

            '------------------------------------------
            ' 【拡張処理用】ログ情報設定
            '------------------------------------------
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "開始", "")

            '------------------------------------------
            ' 【拡張処理用】システム日付/ユーザ名表示
            '------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------
            '　設定ファイル取得
            '------------------------------------------
            If GetIni() = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", "設定ファイル取得")
                Exit Sub
            End If

            '------------------------------------------
            '　休日マスタ取り込み
            '------------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", "休日情報取得")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '------------------------------------------
            ' 初期処理(振替日を算出し表示)
            '------------------------------------------
            Dim SystemDate As String = Date.Now.ToString("yyyyMMdd")     ' システム日付
            Dim KijunDate As String = String.Empty                       ' システム日付の前営業日
            Dim KijunFuriDate As String = String.Empty
            bRet = GCom.CheckDateModule(SystemDate.Substring(0, 6) & "10", KijunDate, CInt(Ini.KijunShiftDate), 1)

            If CInt(SystemDate) <= CInt(KijunDate) Then
                bRet = GCom.CheckDateModule(SystemDate.Substring(0, 6) & "10", KijunFuriDate, 0, 0)
                If bRet = False Then
                    bRet = GCom.CheckDateModule(SystemDate.Substring(0, 6) & "10", KijunFuriDate, 1, 0)
                End If
            Else
                Dim SystemDate_Obj As String = SystemDate.Substring(0, 4) & "/" & SystemDate.Substring(4, 2) & "/" & SystemDate.Substring(6, 2)
                bRet = GCom.CheckDateModule(CDate(SystemDate_Obj).AddMonths(1).ToString("yyyyMM") & "10", KijunFuriDate, 0, 0)
                If bRet = False Then
                    bRet = GCom.CheckDateModule(CDate(SystemDate_Obj).AddMonths(1).ToString("yyyyMM") & "10", KijunFuriDate, 1, 0)
                End If
            End If

            txtFuriDateY.Text = KijunFuriDate.Substring(0, 4)
            txtFuriDateM.Text = KijunFuriDate.Substring(4, 2)
            txtFuriDateD.Text = KijunFuriDate.Substring(6, 2)

            MainLOG.Write(gstrUSER_ID, "0000000000-00", "00000000", "ロード", "成功", "")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(gstrUSER_ID, "0000000000-00", "00000000", "ロード", "失敗", ex.Message)
            Me.Close()
        Finally
            MainLOG.Write(gstrUSER_ID, "0000000000-00", "00000000", "ロード", "終了", "")
        End Try
    End Sub

#End Region

#Region "ボタン"

    '登録ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "開始", "")
            Cursor.Current = Cursors.WaitCursor()

            '------------------------------------------
            ' 入力項目チェック
            '------------------------------------------
            If CheckInput() = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "入力項目チェック")
                Exit Sub
            End If

            '------------------------------------------
            ' マスタチェック
            '------------------------------------------
            MainDB = New CASTCommon.MyOracle
            Dim KijunFuriDate As String = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            If CheckMaster(KijunFuriDate) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "マスタチェック")
                Exit Sub
            End If

            '------------------------------------------
            ' 出力ファイル有無チェック
            '------------------------------------------
            If File.Exists(Path.Combine(Ini.TesuuFileName)) = True Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "成功", "既存ファイルあり ファイル名:" & Path.Combine(Ini.TesuuFileName))

                If MessageBox.Show(String.Format(CUST_MSG0003I, "作成済みの基本料金データ"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                    File.Delete(Path.Combine(Ini.TesuuFileName))
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "成功", "既存ファイル削除")
                Else
                    MessageBox.Show(String.Format(CUST_MSG0004I, "基本料金データ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "既存ファイル削除キャンセル")
                    Exit Sub
                End If
            End If

            '------------------------------------------
            ' 処理開始前確認
            '------------------------------------------
            If MessageBox.Show(String.Format(CUST_MSG0002I, "基本料金データ作成"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                MessageBox.Show(String.Format(CUST_MSG0004I, "基本料金データ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "ユーザキャンセル")
                Exit Sub
            End If

            '------------------------------------------
            ' 基本料金データ作成
            '------------------------------------------
            MainDB.BeginTrans()
            If MakeZenginData(KijunFuriDate) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "基本料金データ作成失敗")
                Exit Sub
            End If

            '------------------------------------------
            ' ジョブ登録(口座振替落込処理)
            '------------------------------------------
            Param.JobID = "J010"
            Dim JobParam As String = Param.ToriCode & "," & _
                                     Param.FuriDate & "," & _
                                     Param.CodeKbn & "," & _
                                     Param.FmtKbn & "," & _
                                     Param.BaitaiCode & "," & _
                                     Param.LabelKbn

            Dim iRet As Integer = MainLOG.SearchJOBMAST(Param.JobID, JobParam, MainDB)
            Select Case iRet
                Case 1
                    MainDB.Rollback()
                    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "ＪＯＢ登録済み")
                    Return
                Case -1
                    MainDB.Rollback()
                    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "ＪＯＢ検索失敗")
                    Return
                Case Else
                    If MainLOG.InsertJOBMAST(Param.JobID, GCom.GetUserID, JobParam, MainDB) = False Then
                        MainDB.Rollback()
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "ＪＯＢ登録失敗")
                        Return
                    Else
                        MainDB.Commit()
                    End If
            End Select

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MessageBox.Show(String.Format(MSG0021I, "落込"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "成功", "")

            Return

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "終了", "")
        End Try
    End Sub

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try

            MainLOG.Write(gstrUSER_ID, "0000000000-00", "00000000", "クローズ", "開始", "")
            Me.Close()
            MainLOG.Write(gstrUSER_ID, "0000000000-00", "00000000", "クローズ", "成功", "")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(gstrUSER_ID, "0000000000-00", "00000000", "クローズ", "失敗", ex.Message)
        Finally
            MainLOG.Write(gstrUSER_ID, "0000000000-00", "00000000", "クローズ", "終了", "")
        End Try

    End Sub

#End Region

#Region "テキスト"
    'テキストボックスゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating

        Try

            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(gstrUSER_ID, "0000000000-00", "00000000", "(テキスト制御)", "失敗", ex.Message)

        Finally

        End Try
    End Sub

#End Region

#Region "関数"

    Private Function GetIni() As Boolean

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "開始", "")

            '------------------------------------------
            ' 基準日数
            '------------------------------------------
            Ini.KijunShiftDate = CASTCommon.GetRSKJIni("CUSTOMIZE_1358", "KIHONTESUU_KIJUN")
            Select Case Ini.KijunShiftDate
                Case "err", "", Nothing
                    Ini.KijunShiftDate = "1"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:基準日数 分類:CUSTOMIZE_1358 項目:KIHONTESUU_KIJUN")
            End Select

            '------------------------------------------
            ' 基本料金徴求取引先(主コード)
            '------------------------------------------
            Ini.TesuuTorisCode = CASTCommon.GetRSKJIni("CUSTOMIZE_1358", "KIHONTESUU_TORIS_CODE")
            Select Case Ini.TesuuTorisCode
                Case "err", "", Nothing
                    MessageBox.Show(String.Format(MSG0001E, "基本料金徴求取引先(主コード)", "CUSTOMIZE_1358", "KIHONTESUU_TORIS_CODE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:基本料金徴求取引先(主コード) 分類:CUSTOMIZE_1358 項目:KIHONTESUU_TORIS_CODE")
                    Return False
            End Select

            '------------------------------------------
            ' 基本料金徴求取引先(副コード)
            '------------------------------------------
            Ini.TesuuTorifCode = CASTCommon.GetRSKJIni("CUSTOMIZE_1358", "KIHONTESUU_TORIF_CODE")
            Select Case Ini.TesuuTorifCode
                Case "err", "", Nothing
                    MessageBox.Show(String.Format(MSG0001E, "基本料金徴求取引先(副コード)", "CUSTOMIZE_1358", "KIHONTESUU_TORIF_CODE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:基本料金徴求取引先(副コード) 分類:CUSTOMIZE_1358 項目:KIHONTESUU_TORIF_CODE")
                    Return False
            End Select

            '------------------------------------------
            ' 基本料金徴求委託者コード
            '------------------------------------------
            Ini.TesuuItakuCode = CASTCommon.GetRSKJIni("CUSTOMIZE_1358", "KIHONTESUU_ITAKU_CODE")
            Select Case Ini.TesuuItakuCode
                Case "err", "", Nothing
                    MessageBox.Show(String.Format(MSG0001E, "基本料金徴求委託者コード", "CUSTOMIZE_1358", "KIHONTESUU_ITAKU_CODE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:基本料金徴求委託者コード 分類:CUSTOMIZE_1358 項目:KIHONTESUU_ITAKU_CODE")
                    Return False
            End Select

            '------------------------------------------
            ' 基本料金徴求委託者名
            '------------------------------------------
            Ini.TesuuItakuName = CASTCommon.GetRSKJIni("CUSTOMIZE_1358", "KIHONTESUU_ITAKU_NAME")
            Select Case Ini.TesuuItakuName
                Case "err", "", Nothing
                    MessageBox.Show(String.Format(MSG0001E, "基本料金徴求委託者名", "CUSTOMIZE_1358", "KIHONTESUU_ITAKU_NAME"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:基本料金徴求委託者名 分類:CUSTOMIZE_1358 項目:KIHONTESUU_ITAKU_NAME")
                    Return False
            End Select

            '------------------------------------------
            ' 基本料金徴求支店コード
            '------------------------------------------
            Ini.TesuuShitenCode = CASTCommon.GetRSKJIni("CUSTOMIZE_1358", "KIHONTESUU_SHITEN_CODE")
            Select Case Ini.TesuuShitenCode
                Case "err", "", Nothing
                    MessageBox.Show(String.Format(MSG0001E, "基本料金徴求支店コード", "CUSTOMIZE_1358", "KIHONTESUU_SHITEN_CODE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:基本料金徴求支店コード 分類:CUSTOMIZE_1358 項目:KIHONTESUU_SHITEN_CODE")
                    Return False
            End Select

            '------------------------------------------
            ' 基本料金徴求支店名
            '------------------------------------------
            Ini.TesuuShitenName = CASTCommon.GetRSKJIni("CUSTOMIZE_1358", "KIHONTESUU_SHITEN_NAME")
            Select Case Ini.TesuuShitenName
                Case "err", "", Nothing
                    MessageBox.Show(String.Format(MSG0001E, "基本料金徴求支店名", "CUSTOMIZE_1358", "KIHONTESUU_SHITEN_NAME"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:基本料金徴求支店名 分類:CUSTOMIZE_1358 項目:KIHONTESUU_SHITEN_NAME")
                    Return False
            End Select

            '------------------------------------------
            ' 基本料金データ出力ファイル名
            '------------------------------------------
            Ini.TesuuFileName = CASTCommon.GetRSKJIni("CUSTOMIZE_1358", "KIHONTESUU_FILE_NAME")
            Select Case Ini.TesuuFileName
                Case "err", "", Nothing
                    MessageBox.Show(String.Format(MSG0001E, "基本料金データ出力ファイル名", "CUSTOMIZE_1358", "KIHONTESUU_FILE_NAME"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:基本料金データ出力ファイル名 分類:CUSTOMIZE_1358 項目:KIHONTESUU_FILE_NAME")
                    Return False
            End Select

            '2017/12/28 FJH）向井 ADD 青梅信金(基本料金関連チェック) ---------------------------------------- START
            '------------------------------------------
            ' 基本料金データ作成対象媒体
            '------------------------------------------
            Ini.TesuuBaitaiCode = CASTCommon.GetRSKJIni("CUSTOMIZE_1358", "KIHONTESUU_BAITAI_CODE")
            Select Case Ini.TesuuBaitaiCode
                Case "err", "", Nothing
                    MessageBox.Show(String.Format(MSG0001E, "基本料金徴求媒体", "CUSTOMIZE_1358", "KIHONTESUU_BAITAI_CODE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:基本料金徴求媒体 分類:CUSTOMIZE_1358 項目:KIHONTESUU_BAITAI_CODE")
                    Return False
            End Select
            '2017/12/28 FJH）向井 ADD 青梅信金(基本料金関連チェック) ---------------------------------------- END

            '------------------------------------------
            ' 自金庫コード
            '------------------------------------------
            Ini.JikinkoCode = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            Select Case Ini.JikinkoCode
                Case "err", "", Nothing
                    MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                    Return False
            End Select

            '------------------------------------------
            ' 自金庫名
            '------------------------------------------
            Ini.JikinkoName = CASTCommon.GetFSKJIni("COMMON", "KINKONAME")
            Select Case Ini.JikinkoName
                Case "err", "", Nothing
                    MessageBox.Show(String.Format(MSG0001E, "自金庫名", "COMMON", "KINKONAME"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:自金庫名 分類:COMMON 項目:KINKONAME")
                    Return False
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "成功", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", ex.Message)
            Return False
        End Try

    End Function

    Private Function CheckInput() As Boolean

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力項目チェック", "開始", "")

            '------------------------------------------
            ' 年必須チェック
            '------------------------------------------
            If txtFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力項目チェック", "失敗", "年必須チェック")
                txtFuriDateY.Focus()
                Return False
            End If

            '------------------------------------------
            ' 月必須チェック
            '------------------------------------------
            If txtFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力項目チェック", "失敗", "月必須チェック")
                txtFuriDateM.Focus()
                Return False
            End If

            '------------------------------------------
            ' 月範囲チェック
            '------------------------------------------
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力項目チェック", "失敗", "月範囲チェック")
                txtFuriDateM.Focus()
                Return False
            End If

            '------------------------------------------
            ' 日付必須チェック
            '------------------------------------------
            If txtFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力項目チェック", "失敗", "日付必須チェック")
                txtFuriDateD.Focus()
                Return False
            End If

            '------------------------------------------
            ' 日付範囲チェック
            '------------------------------------------
            If GCom.NzInt(txtFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力項目チェック", "失敗", "日付範囲チェック")
                txtFuriDateD.Focus()
                Return False
            End If

            '------------------------------------------
            ' 日付整合性チェック
            '------------------------------------------
            Dim WORK_DATE As String = txtFuriDateY.Text & "/" & txtFuriDateM.Text & "/" & txtFuriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力項目チェック", "失敗", "日付整合性チェック")
                txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力項目チェック", "失敗", ex.Message)
            Return False
        End Try

    End Function

    Private Function CheckMaster(ByVal KijunDate As String) As Boolean

        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "マスタチェック", "開始", "")

            ' 2017/12/15 FJH）向井 CHG 青梅信金 作成対象の条件追加-------------------- START
            Dim KijunDate_END = txtFuriDateY.Text & txtFuriDateM.Text & "01"
            '2018/01/22 FJH）向井 CHG 青梅信金(支店登録月追加) ---------------------------------------- START
            Dim KijunDate_START = String.Format("{0:yyyyMM}", GCom.SET_DATE(KijunDate_END).AddMonths(-1))
            'Dim KijunDate_START = String.Format("{0:yyyyMMDD}", GCom.SET_DATE(KijunDate_END).AddMonths(-1))
            '2018/01/22 FJH）向井 CHG 青梅信金(支店登録月追加) ---------------------------------------- END
            ' 2017/12/15 FJH）向井 CHG 青梅信金 作成対象の条件追加-------------------- END

            '------------------------------------------
            ' 取引先マスタ存在チェック
            '------------------------------------------
            SQL.Length = 0
            SQL.Append(" SELECT ")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     S_TORIMAST")
            SQL.Append(" WHERE")
            SQL.Append("     YOBI7_T                  <> '0'")
            ' 2017/12/15 FJH）向井 CHG 青梅信金 作成対象の条件追加-------------------- START
            SQL.Append(" AND '" & KijunDate_END & "' < SYURYOU_DATE_T")         '契約終了日が振替月の1日より後か
            '2018/01/22 FJH）向井 CHG 青梅信金(支店登録月追加) ---------------------------------------- START
            SQL.Append(" AND '" & KijunDate_START & "' > YOBI2_T")         '契約開始日が振替月の1日の1ヶ月前より前か
            'SQL.Append(" AND '" & KijunDate_START & "' > KAISI_DATE_T")         '契約開始日が振替月の1日の1ヶ月前より前か
            '2018/01/22 FJH）向井 CHG 青梅信金(支店登録月追加) ---------------------------------------- END
            'SQL.Append(" AND '" & KijunDate & "' BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")
            ' 2017/12/15 FJH）向井 CHG 青梅信金 作成対象の条件追加-------------------- END
            '2017/12/28 FJH）向井 ADD 青梅信金(基本料金関連チェック) ---------------------------------------- START
            SQL.Append(" AND BAITAI_CODE_T IN (" & Ini.TesuuBaitaiCode & ")")
            '2017/12/28 FJH）向井 ADD 青梅信金(基本料金関連チェック) ---------------------------------------- END

            If OraReader.DataReader(SQL) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "マスタチェック", "成功", "基本料金データ作成先０件")
                MessageBox.Show("基本料金データ作成" & MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '------------------------------------------
            ' 手数料徴求取引先存在チェック
            '------------------------------------------
            SQL.Length = 0
            SQL.Append(" SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append("     TORIS_CODE_T    = " & SQ(Ini.TesuuTorisCode))
            SQL.Append(" AND TORIF_CODE_T    = " & SQ(Ini.TesuuTorifCode))

            If OraReader.DataReader(SQL) = True Then
                Param.ToriCode = Ini.TesuuTorisCode & Ini.TesuuTorifCode
                Param.FuriDate = KijunDate
                Param.CodeKbn = OraReader.GetString("CODE_KBN_T")
                Param.FmtKbn = OraReader.GetString("FMT_KBN_T")
                Param.BaitaiCode = OraReader.GetString("BAITAI_CODE_T")
                Param.LabelKbn = OraReader.GetString("LABEL_KBN_T")
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "マスタチェック", "失敗", "基本料金徴求取引先未登録")
                MessageBox.Show(String.Format(MSG0278W, Ini.TesuuTorisCode, Ini.TesuuTorifCode), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '------------------------------------------
            ' 手数料徴求取引先スケジュールチェック
            '------------------------------------------
            SQL.Length = 0
            SQL.Append(" SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append("   , SCHMAST")
            SQL.Append(" WHERE ")
            SQL.Append("     TORIS_CODE_T    = " & SQ(Ini.TesuuTorisCode))
            SQL.Append(" AND TORIF_CODE_T    = " & SQ(Ini.TesuuTorifCode))
            SQL.Append(" AND TORIS_CODE_T    = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T    = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_S     = " & SQ(KijunDate))

            If OraReader.DataReader(SQL) = True Then
                If OraReader.GetString("TYUUDAN_FLG_S") <> "0" Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "マスタチェック", "失敗", "スケジュール中断済み")
                    MessageBox.Show(CUST_MSG0007W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If

                If OraReader.GetString("TOUROKU_FLG_S") <> "0" Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "マスタチェック", "失敗", "スケジュール落込処理済み")
                    MessageBox.Show("指定した振替日のスケジュールは、" & MSG0061W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "マスタチェック", "失敗", "スケジュール該当なし")
                MessageBox.Show(MSG0062W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "マスタチェック", "成功", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "マスタチェック", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "マスタチェック", "終了", "")
        End Try

    End Function

    Private Function MakeZenginData(ByVal KijunDate As String) As Boolean

        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim MeiReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        Dim FileWrite As StreamWriter = Nothing
        Dim gZENGIN_REC1 As CAstFormat.CFormatZengin.ZGRECORD1 = Nothing
        Dim gZENGIN_REC2 As CAstFormat.CFormatZengin.ZGRECORD2 = Nothing
        Dim gZENGIN_REC8 As CAstFormat.CFormatZengin.ZGRECORD8 = Nothing
        Dim gZENGIN_REC9 As CAstFormat.CFormatZengin.ZGRECORD9 = Nothing

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "基本料金データ作成", "開始", "")

            '------------------------------------------
            ' 基本料金手数料徴求取引先情報取得
            '------------------------------------------
            SQL.Length = 0
            SQL.Append(" SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append("     TORIS_CODE_T    = " & SQ(Ini.TesuuTorisCode))
            SQL.Append(" AND TORIF_CODE_T    = " & SQ(Ini.TesuuTorifCode))

            If OraReader.DataReader(SQL) = True Then
                '------------------------------------------
                ' トレーラレコード編集用項目定義
                '------------------------------------------
                Dim IraiKen As Integer = 0
                Dim IraiKin As Long = 0

                '------------------------------------------
                ' ファイル生成
                '------------------------------------------
                FileWrite = New StreamWriter(Path.Combine(Ini.TesuuFileName), False, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

                '------------------------------------------
                ' ヘッダレコード作成
                '------------------------------------------
                With gZENGIN_REC1
                    .ZG1 = "1"
                    .ZG2 = "91"
                    .ZG3 = "0"
                    .ZG4 = Ini.TesuuItakuCode
                    .ZG5 = Ini.TesuuItakuName.PadRight(40)
                    .ZG6 = KijunDate.Substring(4, 4)
                    .ZG7 = Ini.JikinkoCode
                    .ZG8 = Ini.JikinkoName.PadRight(15)
                    .ZG9 = Ini.TesuuShitenCode
                    .ZG10 = Ini.TesuuShitenName.PadRight(15)
                    .ZG11 = "1"
                    .ZG12 = "0000001"
                    .ZG13 = Space(17)
                End With

                Select Case Param.CodeKbn
                    Case "1"
                        FileWrite.WriteLine(gZENGIN_REC1.Data)
                    Case Else
                        FileWrite.Write(gZENGIN_REC1.Data)
                End Select

                '------------------------------------------
                ' データレコード作成
                '------------------------------------------
                SQL.Length = 0
                SQL.Append(" SELECT ")
                SQL.Append("     *")
                SQL.Append(" FROM")
                SQL.Append("     S_TORIMAST")
                SQL.Append(" WHERE")
                SQL.Append("     YOBI7_T                  <> '0'")
                '2017/12/28 FJH）向井 ADD 青梅信金(基本料金関連チェック) ---------------------------------------- START
                Dim KijunDate_END = txtFuriDateY.Text & txtFuriDateM.Text & "01"
                '2018/01/22 FJH）向井 CHG 青梅信金(支店登録月追加) ---------------------------------------- START
                Dim KijunDate_START = String.Format("{0:yyyyMM}", GCom.SET_DATE(KijunDate_END).AddMonths(-1))
                'Dim KijunDate_START = String.Format("{0:yyyyMMDD}", GCom.SET_DATE(KijunDate_END).AddMonths(-1))
                '2018/01/22 FJH）向井 CHG 青梅信金(支店登録月追加) ---------------------------------------- END
                SQL.Append(" AND '" & KijunDate_END & "' < SYURYOU_DATE_T")         '契約終了日が振替月の1日より後か
                '2018/01/22 FJH）向井 CHG 青梅信金(支店登録月追加) ---------------------------------------- START
                SQL.Append(" AND '" & KijunDate_START & "' > YOBI2_T")         '契約開始日が振替月の1日の1ヶ月前より前か
                'SQL.Append(" AND '" & KijunDate_START & "' > KAISI_DATE_T")         '契約開始日が振替月の1日の1ヶ月前より前か
                '2018/01/22 FJH）向井 CHG 青梅信金(支店登録月追加) ---------------------------------------- END
                SQL.Append(" AND BAITAI_CODE_T IN (" & Ini.TesuuBaitaiCode & ")")
                'SQL.Append(" AND '" & KijunDate & "' BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")
                '2017/12/28 FJH）向井 ADD 青梅信金(基本料金関連チェック) ---------------------------------------- END
                SQL.Append(" ORDER BY")
                SQL.Append("     YOBI8_T")
                SQL.Append("   , YOBI9_T")
                SQL.Append("   , YOBI10_T")

                MeiReader = New CASTCommon.MyOracleReader(MainDB)
                If MeiReader.DataReader(SQL) = True Then
                    While MeiReader.EOF = False
                        With gZENGIN_REC2
                            .ZG1 = "2"
                            .ZG2 = Ini.JikinkoCode
                            .ZG3 = Ini.JikinkoName.PadRight(15)
                            .ZG4 = MeiReader.GetString("YOBI8_T")
                            If .ZG2.Trim = "" Or .ZG4.Trim = "" Then
                                .ZG5 = Space(15)
                            Else
                                '支店名取得
                                Dim KinKName As String = String.Empty
                                Dim SitKName As String = String.Empty
                                If Ex_GetTenMast(.ZG2, .ZG4, KinKName, SitKName) = True Then
                                    .ZG5 = SitKName.PadRight(15)
                                Else
                                    .ZG5 = Space(15)
                                End If
                                Dim KinName As String = String.Empty
                                Dim SitName As String = String.Empty
                            End If
                            .ZG6 = Space(4)
                            .ZG7 = ConvertKamoku2TO1(MeiReader.GetString("YOBI9_T").PadLeft(2, "0"c))
                            .ZG8 = MeiReader.GetString("YOBI10_T").PadLeft(7, "0"c)
                            .ZG9 = MeiReader.GetString("ITAKU_KNAME_T").PadRight(30).Substring(0, 30)

                            Dim FuriKin As Long = GetKihonTesuuKin(MeiReader.GetString("YOBI7_T"))
                            .ZG10 = Format(FuriKin, "0000000000")
                            .ZG11 = "0"
                            .ZG12 = MeiReader.GetString("TORIS_CODE_T")
                            .ZG13 = MeiReader.GetString("TORIF_CODE_T").PadRight(10)
                            .ZG14 = "0"
                            .ZG15 = Space(8)
                        End With

                        Select Case Param.CodeKbn
                            Case "1"
                                FileWrite.WriteLine(gZENGIN_REC2.Data)
                            Case Else
                                FileWrite.Write(gZENGIN_REC2.Data)
                        End Select

                        IraiKen += 1
                        IraiKin += CLng(gZENGIN_REC2.ZG10.Trim)

                        MeiReader.NextRead()

                    End While
                End If

                '-----------------------------------------------------
                ' トレーラレコード出力
                '-----------------------------------------------------
                With gZENGIN_REC8
                    .ZG1 = "8"
                    .ZG2 = Format(IraiKen, "000000")
                    .ZG3 = Format(IraiKin, "000000000000")
                    .ZG4 = "000000"
                    .ZG5 = "000000000000"
                    .ZG6 = "000000"
                    .ZG7 = "000000000000"
                    .ZG8 = Space(65)
                End With

                Select Case Param.CodeKbn
                    Case "1"
                        FileWrite.WriteLine(gZENGIN_REC8.Data)
                    Case Else
                        FileWrite.Write(gZENGIN_REC8.Data)
                End Select

                '-----------------------------------------------------
                ' エンドレコード出力
                '-----------------------------------------------------
                With gZENGIN_REC9
                    .ZG1 = "9"
                    .ZG2 = Space(119)
                End With

                Select Case Param.CodeKbn
                    Case "1"
                        FileWrite.WriteLine(gZENGIN_REC9.Data)
                    Case Else
                        FileWrite.Write(gZENGIN_REC9.Data)
                End Select

                '-----------------------------------------------------
                ' ファイルクローズ
                '-----------------------------------------------------
                FileWrite.Close()
                FileWrite = Nothing

            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "基本料金データ作成", "成功", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "基本料金データ作成", "失敗", ex.Message)
            Return False
        Finally
            If Not MeiReader Is Nothing Then
                MeiReader.Close()
                MeiReader = Nothing
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not FileWrite Is Nothing Then
                FileWrite.Close()
                FileWrite = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "基本料金データ作成", "終了", "")
        End Try

    End Function

    Private Function GetKihonTesuuKin(ByVal KihonTesuuID As String) As Long

        Dim ReturnData As Long = 0

        Dim sr As StreamReader = Nothing

        Try

            sr = New StreamReader(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFSMAST010C_基本料金区分.TXT"), Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLineData() As String = sr.ReadLine().Split(","c)
                If strLineData(0).Trim = KihonTesuuID.Trim Then
                    ReturnData = CLng(strLineData(2).Trim)
                    Exit While
                End If
            End While

            sr.Close()
            sr = Nothing

            Return ReturnData

        Catch ex As Exception
            Return 0
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
        End Try

    End Function

#End Region

End Class
'2017/09/05 タスク）綾部 ADD 【PG】青梅信金 カスタマイズ対応(UI_5-2) -------------------- END
