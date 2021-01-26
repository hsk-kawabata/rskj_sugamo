Imports System.Data.OracleClient
Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon

Public Class KFJMAIN050
    Inherits System.Windows.Forms.Form
    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN050", "日報集計処理画面")
    Private Const msgTitle As String = "日報集計処理画面(KFJMAIN050)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private DenFolder As String
    ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Private INI_RSV2_EDTION As String
    ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private strFURI_DATE As String
#Region " ロード"
    Private Sub KFJMAIN050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            Call GCom.SetMonitorTopArea(Label3, Label2, lblUser, lblDate)

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
            '休日に営業日取得を行うとFalseが変えるためコメント化
            'If bRet = False Then
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(前営業日取得)終了", "失敗", "")
            '    MessageBox.Show(MSG0012E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If
            '===================================================
            txtFuriDateY.Text = strGetdate.Substring(0, 4)
            txtFuriDateM.Text = strGetdate.Substring(4, 2)
            txtFuriDateD.Text = strGetdate.Substring(6, 2)

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
        'Description    :日報集計処理更新ボタン
        'Return         :
        'Create         :2009/09/10
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            LW.FuriDate = strFURI_DATE

            '--------------------------------
            '日報ファイル存在チェック(MSG????W)
            '--------------------------------
            Dim NippouFileName As String
            If rdbNippou1.Checked Then
                NippouFileName = Path.Combine(DenFolder, "NIPPOU.DAT")
            Else
                NippouFileName = Path.Combine(DenFolder, "NIPPOU2.DAT")
            End If
            If File.Exists(NippouFileName) = False Then
                MessageBox.Show(MSG0255W.Replace("{0}", "日報"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' 2016/01/18 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START 
            ''更新前確認メッセージ
            'If MessageBox.Show(MSG0023I.Replace("{0}", "日報処理"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            '    Return
            'End If
            'MainDB = New CASTCommon.MyOracle
            'MainDB.BeginTrans()

            'Dim jobid As String
            'Dim para As String

            ''ジョブマスタに登録
            'jobid = "J050"                      '..\Batch\日報\
            ''パラメータ(振替日,ログイン名,ファイル区分)      
            'If rdbNippou1.Checked Then
            '    para = strFURI_DATE & "," & GCom.GetUserID & ",1"
            'Else
            '    para = strFURI_DATE & "," & GCom.GetUserID & ",2"
            'End If


            ''#########################
            ''job検索
            ''#########################
            'Dim iRet As Integer
            'iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            'If iRet = 1 Then    'ジョブ登録済)
            '    MainDB.Rollback()
            '    MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Return
            'ElseIf iRet = -1 Then 'ジョブ検索失敗
            '    MainDB.Rollback()
            '    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブマスタ検索失敗 パラメータ:" & para)
            '    Return
            'End If

            ''#########################
            ''job登録
            ''#########################
            'If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then 'ジョブ登録失敗
            '    MainDB.Rollback()
            '    MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブ登録失敗 パラメータ:" & para)
            '    Return
            'End If

            'MainDB.Commit()

            'MessageBox.Show(MSG0021I.Replace("{0}", "日報処理"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Select Case INI_RSV2_EDTION
                Case "2"
                    '----------------------------------------------------------
                    ' 実行前確認メッセージ      
                    '----------------------------------------------------------
                    If MessageBox.Show(MSG0015I.Replace("{0}", "日報"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Return
                    End If

                    '----------------------------------------------------------
                    ' パラメータ設定
                    ' パラメータ:[振替日],[ログイン名],ファイル区分      
                    '----------------------------------------------------------
                    Dim para As String
                    If rdbNippou1.Checked Then
                        para = strFURI_DATE & "," & GCom.GetUserID & ",1"
                    Else
                        para = strFURI_DATE & "," & GCom.GetUserID & ",2"
                    End If

                    '----------------------------------------------------------
                    ' 日報処理開始      
                    '----------------------------------------------------------
                    Dim ExePath As String = CASTCommon.GetFSKJIni("COMMON", "EXE")
                    Dim ReturnCode As Integer = 0

                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "開始", "パラメータ" & para)
                    Dim myProcess As New Process
                    Dim StartInfo As New ProcessStartInfo(Path.Combine(ExePath, "KFJ050_EDITION2.EXE"), para)
                    StartInfo.WorkingDirectory = ExePath
                    StartInfo.CreateNoWindow = True
                    StartInfo.UseShellExecute = False
                    myProcess = Process.Start(StartInfo)
                    myProcess.WaitForExit()

                    ReturnCode = myProcess.ExitCode
                    If ReturnCode > 10 Then
                        Dim ErrMsg As String = ""
                        Select Case ReturnCode
                            Case 110
                                ErrMsg = "パラメータ設定なし。"
                            Case 111
                                ErrMsg = "ファイル区分異常。"
                            Case 112
                                ErrMsg = "パラメータ数不正。"
                            Case 210
                                ErrMsg = "設定ファイル取得失敗"
                            Case 211
                                ErrMsg = "日報ファイルなし。"
                            Case 212
                                ErrMsg = "日報ファイルのコード変換処理失敗。"
                            Case 213
                                ErrMsg = "日報ファイルのソート処理失敗。"
                            Case 214
                                ErrMsg = "日報ファイル読込失敗。"
                            Case 215
                                ErrMsg = "スケジュールマスタエラーリスト印刷失敗。"
                            Case 216
                                ErrMsg = "取引先マスタエラーリスト印刷失敗。"
                            Case 290
                                ErrMsg = "手数料計算のＪＯＢ登録失敗。"
                        End Select
                        ErrMsg += "詳細はログを参照してください。"
                        MessageBox.Show(String.Format(MSG0371W, "日報", ErrMsg), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "エラーコード： " & ReturnCode)
                        Return
                    Else
                        Select Case ReturnCode
                            Case 0
                                MessageBox.Show(String.Format(MSG0078I, "日報"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "成功", "")
                            Case Else
                                MessageBox.Show(String.Format(MSG0078I, "日報") & vbCrLf & "手数料計算のＪＯＢ登録に失敗したため、ログファイルの確認をお願いします。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "成功", "手数料計算失敗　エラーコード： " & ReturnCode)
                        End Select
                    End If

                Case Else
                    '----------------------------------------------------------
                    ' 更新前確認メッセージ      
                    '----------------------------------------------------------
                    If MessageBox.Show(MSG0023I.Replace("{0}", "日報処理"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                        Return
                    End If

                    '----------------------------------------------------------
                    ' ＪＯＢ登録開始     
                    '----------------------------------------------------------
                    MainDB = New CASTCommon.MyOracle
                    MainDB.BeginTrans()

                    Dim jobid As String
                    Dim para As String

                    'ジョブマスタに登録
                    jobid = "J050"                      '..\Batch\日報\
                    'パラメータ(振替日,ログイン名,ファイル区分)      
                    If rdbNippou1.Checked Then
                        para = strFURI_DATE & "," & GCom.GetUserID & ",1"
                    Else
                        para = strFURI_DATE & "," & GCom.GetUserID & ",2"
                    End If


                    '#########################
                    ' job検索
                    '#########################
                    Dim iRet As Integer
                    iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                    If iRet = 1 Then    'ジョブ登録済)
                        MainDB.Rollback()
                        MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    ElseIf iRet = -1 Then 'ジョブ検索失敗
                        MainDB.Rollback()
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブマスタ検索失敗 パラメータ:" & para)
                        Return
                    End If

                    '#########################
                    ' job登録
                    '#########################
                    If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then 'ジョブ登録失敗
                        MainDB.Rollback()
                        MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブ登録失敗 パラメータ:" & para)
                        Return
                    End If

                    MainDB.Commit()

                    MessageBox.Show(MSG0021I.Replace("{0}", "日報処理"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Select
            ' 2016/01/18 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END 

        Catch ex As Exception
            If MainDB IsNot Nothing Then
                MainDB.Rollback()
            End If
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
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
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then '(MSG0022W)
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

    Public Function fn_INI_Read() As Boolean
        '============================================================================
        'NAME           :fn_INI_Read
        'Parameter      :
        'Description    :FSKJ.INIファイルの読み込み
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/15
        'Update         :
        '============================================================================
        fn_INI_Read = False
        'DAT格納フォルダ
        DenFolder = CASTCommon.GetFSKJIni("COMMON", "DEN")
        If DenFolder.ToUpper = "ERR" OrElse DenFolder = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "伝送格納フォルダ", "COMMON", "DEN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
        INI_RSV2_EDTION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
        If INI_RSV2_EDTION.ToUpper = "ERR" OrElse INI_RSV2_EDTION = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "RSV2機能設定", "RSV2_V1.0.0", "EDITION"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        ' 2016/01/18 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

        fn_INI_Read = True
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


End Class