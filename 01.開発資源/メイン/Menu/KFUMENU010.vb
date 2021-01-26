Option Explicit On
Option Strict On
Imports CASTCommon

Public Class KFUMENU010

    Inherits System.Windows.Forms.Form

    Private BatchLog As New CASTCommon.BatchLOG("KFUMENU010", "メインメニュー")
    Private Const msgTitle As String = "メインメニュー(KFUMENU010)"

    Private strCMD As String
    Private strCMD_KEKKA As String
    Private MyOwnerForm As Form
    Private noClose As Boolean

    Private EXE_DIR As String
    Private KESSAI As String
    Private SOUFURI As String
    Private GAKKOU As String
    Private TIIKI As String
    Private CMT As String
    Private MT As String
    Private WEB_DEN As String

    Private INI_RSV2_EDITION As String
    Private INI_RSV2_MAINMENU As ArrayList
    Private EXE_NAME As ArrayList

    Private Sub KFJMENU0001G_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Try

            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            sw = BatchLog.Write_Enter1("", "0000000000-00", "00000000", "ロード(開始)", "成功")
            'BatchLog.Write("0000000000-00", "00000000", "(ロード)開始", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

            GOwnerForm = Me
            MyOwnerForm = GOwnerForm

            gstrUSER_ID = ""
            Dim CmdLine() As String = System.Environment.GetCommandLineArgs
            If CmdLine.Length >= 2 Then
                gstrUSER_ID = CmdLine(1)
            End If
            GCom.GetUserID = gstrUSER_ID
            'ユーザID／システム日付表示
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '==================================================================
            '　初期設定
            '==================================================================
            INI_RSV2_MAINMENU = New ArrayList
            EXE_NAME = New ArrayList

            '==================================================================
            '　ＩＮＩファイル情報取得
            '==================================================================
            If GetIniInfo() = False Then
                BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "ロード", "設定ファイル取得", "失敗")
                Return
            End If

            '==================================================================
            '　メインメニュー表示設定
            '==================================================================
            If SetMainMenu() = False Then
                BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "ロード", "メインメニュー表示設定", "失敗")
                Return
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            BatchLog.Write_Err("0000000000-00", "00000000", "(ロード)終了", "失敗", ex)
            'BatchLog.Write("0000000000-00", "00000000", "(ロード)終了", "失敗", ex.ToString)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            BatchLog.Write_Exit1(sw, "", "0000000000-00", "00000000", "ロード(終了)", "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        End Try

    End Sub

    'ログイン画面へ戻る
    Private Sub CmdLogOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Call StartExeProc("LOGIN.EXE")
        Me.Close()
        Me.Dispose()
        Application.Exit()
    End Sub

    Private Sub KFJMENU010_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub

    'MAINMENU01
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        If CStr(EXE_NAME(0)) <> "" Then
            Call StartExeProc(CStr(EXE_NAME(0)))
            Me.Close()
            Me.Dispose()
            Application.Exit()
        End If

    End Sub

    'MAINMENU02
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        If CStr(EXE_NAME(1)) <> "" Then
            Call StartExeProc(CStr(EXE_NAME(1)))
            Me.Close()
            Me.Dispose()
            Application.Exit()
        End If

    End Sub

    'MAINMENU03
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        If CStr(EXE_NAME(2)) <> "" Then
            Call StartExeProc(CStr(EXE_NAME(2)))
            Me.Close()
            Me.Dispose()
            Application.Exit()
        End If

    End Sub

    'MAINMENU04
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click

        If CStr(EXE_NAME(3)) <> "" Then
            Call StartExeProc(CStr(EXE_NAME(3)))
            Me.Close()
            Me.Dispose()
            Application.Exit()
        End If

    End Sub

    'MAINMENU05
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click

        If CStr(EXE_NAME(4)) <> "" Then
            Call StartExeProc(CStr(EXE_NAME(4)))
            Me.Close()
            Me.Dispose()
            Application.Exit()
        End If

    End Sub

    'MAINMENU06
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click

        If CStr(EXE_NAME(5)) <> "" Then
            Call StartExeProc(CStr(EXE_NAME(5)))
            Me.Close()
            Me.Dispose()
            Application.Exit()
        End If

    End Sub

    'MAINMENU07
    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click

        If CStr(EXE_NAME(6)) <> "" Then
            Call StartExeProc(CStr(EXE_NAME(6)))
            Me.Close()
            Me.Dispose()
            Application.Exit()
        End If

    End Sub

    'MAINMENU08
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click

        If CStr(EXE_NAME(7)) <> "" Then
            Call StartExeProc(CStr(EXE_NAME(7)))
            Me.Close()
            Me.Dispose()
            Application.Exit()
        End If

    End Sub

    'MAINMENU09
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click

        If CStr(EXE_NAME(8)) <> "" Then
            Call StartExeProc(CStr(EXE_NAME(8)))
            Me.Close()
            Me.Dispose()
            Application.Exit()
        End If

    End Sub

    'MAINMENU10
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click

        If CStr(EXE_NAME(9)) <> "" Then
            Call StartExeProc(CStr(EXE_NAME(9)))
            Me.Close()
            Me.Dispose()
            Application.Exit()
        End If

    End Sub

    '
    ' 機能　　　: モジュール起動
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: ARG1 - exename        実行ファイル名
    '
    ' 備考　　　: 
    '
    Private Function StartExeProc(ByVal exename As String, Optional ByVal wait As Boolean = False) As String

        Return StartProc(EXE_DIR, exename, wait)

    End Function

    '
    ' 機能　　　: モジュール起動
    '
    ' 戻り値　　: なし
    '
    ' 引き数　　: ARG1 - dir            実行ファイルフォルダ名
    '             ARG2 - exename        実行ファイル名
    '             ARG3 - wait           True：終了待機する，FALSE：終了待機しない
    '
    ' 備考　　　: 
    '
    Private Function StartProc(ByVal dir As String, ByVal exename As String, Optional ByVal wait As Boolean = False) As String

        '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
        Dim sw As System.Diagnostics.Stopwatch = Nothing
        sw = BatchLog.Write_Enter1("", "0000000000-00", "00000000", "アプリケーション起動(開始)" & exename, "成功")
        '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

        Dim Arguments As String = gstrUSER_ID    '引数
        StartProc = ""

        Try

            '*** Str Chg 2015/12/26 sys)mori for LOG強化対応 ***
            'BatchLog.Write("0000000000-00", "00000000", "ログイン(開始)", "成功")
            '*** end Chg 2015/12/26 sys)mori for LOG強化対応 ***

            ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
            'If String.Compare(exename, "MEDIACONVERT.EXE", True) = 0 Or _
            '    String.Compare(exename, "BAITAIINOUT.EXE", True) = 0 Then
            '    Arguments &= "," & Date.Now.ToString("yyyyMMdd")
            'End If
            If String.Compare(exename, "BAITAIINOUT.EXE", True) = 0 Then
                Arguments &= "," & Date.Now.ToString("yyyyMMdd")
            End If
            ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

            ' 実行
            Dim ExecProc As Process = Process.Start(dir & exename, Arguments)
            If ExecProc.Id <> 0 Then
                'Me.Visible = False
                If wait = True Then
                    '終了待機
                    ExecProc.WaitForExit()
                    strCMD_KEKKA = ExecProc.ExitCode.ToString
                    'Me.Visible = True
                    'Me.Activate()
                Else
                    Me.Close()
                End If
            Else
                MessageBox.Show(String.Format(MSG0024E, exename), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return ""
            End If

        Catch ex As Exception
            Dim MessageText As String
            MessageText = ex.Message
            MessageText &= Environment.NewLine
            MessageText &= dir & exename
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            BatchLog.Write_Err("0000000000-00", "00000000", "アプリケーション起動" & exename,"失敗", ex)
            'BatchLog.Write("0000000000-00", "00000000", "アプリケーション起動", "失敗", MessageText)
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        Finally
            '*** Str Add 2015/12/26 sys)mori for LOG強化対応 ***
            BatchLog.Write_Exit1(sw, "", "0000000000-00", "00000000", "アプリケーション起動(終了)" & exename, "成功")
            '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***
        End Try

    End Function

    Private Function GetIniInfo() As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing
        sw = BatchLog.Write_Enter1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "")

        Try

            '==================================================================
            '　RSV2機能設定情報
            '==================================================================
            INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            If INI_RSV2_EDITION.ToUpper = "ERR" Or INI_RSV2_EDITION = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "RSV2機能設定", "RSV2_V1.0.0", "EDITION"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "失敗", "項目名:RSV2機能設定 分類:RSV2_V1.0.0 項目:EDITION")
                Return False
            End If

            Select Case INI_RSV2_EDITION
                Case "1", "2"
                    ' NOP
                Case Else
                    MessageBox.Show(String.Format(MSG0035E, "RSV2機能設定", "RSV2_V1.0.0", "EDITION"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "設定誤り", "項目名:RSV2機能設定 分類:RSV2_V1.0.0 項目:EDITION")
                    Return False
            End Select


            '==================================================================
            '　実行ファイルフォルダ
            '==================================================================
            EXE_DIR = CASTCommon.GetFSKJIni("COMMON", "EXE")
            If EXE_DIR.ToUpper = "ERR" Or EXE_DIR = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "EXE格納フォルダ", "COMMON", "EXE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "失敗", "項目名:EXE格納フォルダ 分類:COMMON 項目:EXE")
                Return False
            End If

            '==================================================================
            '　決済使用区分
            '==================================================================
            KESSAI = CASTCommon.GetFSKJIni("OPTION", "KESSAI")
            If KESSAI.ToUpper = "ERR" OrElse KESSAI = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "決済使用区分", "OPTION", "KESSAI"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "失敗", "項目名:決済使用区分 分類:OPTION 項目:KESSAI")
                Return False
            End If

            '==================================================================
            '　学校使用区分
            '==================================================================
            GAKKOU = CASTCommon.GetFSKJIni("OPTION", "GAKKOU")
            If GAKKOU.ToUpper = "ERR" OrElse GAKKOU = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "学校使用区分", "OPTION", "GAKKOU"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "失敗", "項目名:学校使用区分 分類:OPTION 項目:GAKKOU")
                Return False
            End If

            '==================================================================
            '　総振使用区分
            '==================================================================
            SOUFURI = CASTCommon.GetFSKJIni("OPTION", "SOUFURI")
            If SOUFURI.ToUpper = "ERR" OrElse SOUFURI = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "総振使用区分", "OPTION", "SOUFURI"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "失敗", "項目名:総振使用区分 分類:OPTION 項目:SOUFURI")
                Return False
            End If

            '==================================================================
            '　地域使用区分
            '==================================================================
            TIIKI = CASTCommon.GetFSKJIni("OPTION", "TIIKI")
            If TIIKI.ToUpper = "ERR" OrElse TIIKI = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "地域使用区分", "OPTION", "TIIKI"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "失敗", "項目名:地域使用区分 分類:OPTION 項目:TIIKI")
                Return False
            End If

            '==================================================================
            '　CMT使用区分
            '==================================================================
            CMT = CASTCommon.GetFSKJIni("COMMON", "CMT")
            If CMT.ToUpper = "ERR" OrElse CMT = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "CMT区分", "COMMON", "CMT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "失敗", "項目名:CMT区分 分類:COMMON 項目:CMT")
                Return False
            End If

            '==================================================================
            '　MT使用区分
            '==================================================================
            MT = CASTCommon.GetFSKJIni("COMMON", "MT")
            If MT.ToUpper = "ERR" OrElse MT = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "MT区分", "COMMON", "MT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "失敗", "項目名:MT区分 分類:COMMON 項目:MT")
                Return False
            End If

            '==================================================================
            '　WEB伝送使用区分
            '==================================================================
            WEB_DEN = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_DENSO")
            If WEB_DEN.ToUpper = "ERR" OrElse WEB_DEN = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "WEB伝送使用区分", "WEB_DEN", "WEB_DENSO"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "失敗", "項目名:WEB伝送使用区分 分類:WEB_DEN 項目:WEB_DENSO")
                Return False
            End If

            '==================================================================
            '　MAINMENUボタン情報(MAINMENU01～MAINMENU10取得)
            '==================================================================
            For i As Integer = 1 To 10 Step 1
                INI_RSV2_MAINMENU.Add(CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MAINMENU" & Format(i, "00")))
                Dim INI_CHECK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MAINMENU" & Format(i, "00"))
                If INI_CHECK.ToUpper = "ERR" OrElse INI_CHECK = Nothing Then
                    MessageBox.Show(String.Format(MSG0001E, "MAINMENUボタン情報", "RSV2_V1.0.0", "MAINMENU" & Format(i, "00")), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    BatchLog.Write_LEVEL1(gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "失敗", "項目名:MAINMENUボタン情報 分類:RSV2_V1.0.0 項目:MAINMENU" & Format(i, "00"))
                    Return False
                End If
            Next

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write_Err("0000000000-00", "00000000", "設定ファイル取得", "失敗", ex)
            Return False
        Finally
            BatchLog.Write_Exit1(sw, gstrUSER_ID, "0000000000-00", "00000000", "設定ファイル取得", "")
        End Try

    End Function

    Private Function SetMainMenu() As Boolean

        Dim sw As System.Diagnostics.Stopwatch = Nothing
        sw = BatchLog.Write_Enter1(gstrUSER_ID, "0000000000-00", "00000000", "MAINMENUボタン情報設定", "")

        Try

            '==================================================================
            '　MAINMENUボタン情報設定(INIﾌｧｲﾙ情報分解)
            '==================================================================
            For i As Integer = 0 To INI_RSV2_MAINMENU.Count - 1 Step 1
                Dim MAINMENU_INFO() As String = Split(CStr(INI_RSV2_MAINMENU(i)), ",")
                EXE_NAME.Add(MAINMENU_INFO(3).Trim)

                For Each CTL As Control In Me.Controls
                    If TypeOf CTL Is Button Then
                        If CTL.Name.ToString = "Button" & CStr(i + 1) Then

                            '==================================================
                            '　Visible設定
                            '==================================================
                            If MAINMENU_INFO(0) = "1" Then
                                CTL.Visible = True
                            Else
                                CTL.Visible = False
                            End If

                            '==================================================
                            '　Enabled設定
                            '==================================================
                            If MAINMENU_INFO(1) = "1" Then
                                CTL.Enabled = True
                            Else
                                CTL.Enabled = False
                            End If

                            '==================================================
                            '　ボタン名設定
                            '==================================================
                            If MAINMENU_INFO(2) <> "" Then
                                CTL.Text = MAINMENU_INFO(2)
                            Else
                                CTL.Text = ""
                            End If

                        End If

                    End If

                Next CTL
            Next

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BatchLog.Write_Err("0000000000-00", "00000000", "MAINMENUボタン情報設定", "失敗", ex)
            Return False
        Finally
            BatchLog.Write_Exit1(sw, gstrUSER_ID, "0000000000-00", "00000000", "MAINMENUボタン情報設定", "")
        End Try

    End Function

End Class
