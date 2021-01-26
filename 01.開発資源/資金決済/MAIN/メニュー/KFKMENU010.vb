Option Explicit On 
Option Strict On

Imports System.Reflection
Imports CASTCommon

<Assembly: AssemblyInformationalVersion("1.0.2.0")> 

Public Class KFKMENU010

    Private MyOwnerForm As Form
    Private noClose As Boolean

    Private MainLOG As New CASTCommon.BatchLOG("KFKMENU010", "資金決済メニュー画面")
    Private Const msgTitle As String = "資金決済メニュー画面(KFKMENU010)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Private INI_RSV2_EDITION As String
    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

    Private Sub KFKMENU010_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub

    Private Sub KFKMENU010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            Dim Temp As String() = Environment.GetCommandLineArgs

            With GCom
                .GetSysDate = Date.Now
                If Temp.Length <= 1 Then
                    .GetUserID = ""
                Else
                    .GetUserID = Temp(1).Trim
                End If
                gstrUSER_ID = .GetUserID        'ユーザID
            End With

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '------------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            GOwnerForm = Me
            MyOwnerForm = GOwnerForm

            ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
            If GetIniInfo() = False Then
                Exit Try
            End If

            If SetDisplayInit() = False Then
                Exit Try
            End If
            ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

            '未使用のボタンを非表示にする。
            For Each CTL As Control In TabPage1.Controls
                If TypeOf CTL Is Button Then
                    CTL.Visible = (GCom.NzInt(CTL.Tag) > 0)
                End If
            Next CTL

            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
            Dim clsExDsp As New CAstExternal.ClsExternalMenu()
            clsExDsp.Read_Menu(gstrUSER_ID, Me, TabControl1, CAstExternal.ClsExternalMenu.ExternalMENU_KESSAI)
            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

            '*** Str Add 2015/12/09 SO)H.Yamagishi for 拡張印刷 ***
            Dim clsExPrt As New CAstExtendPrint.ClsExtendPrintMenu()
            clsExPrt.Read_PrtMenu(gstrUSER_ID, Me, TabControl1, CAstExtendPrint.ClsExtendPrintMenu.EXPRTMENU_KESSAI)
            '*** End Add 2015/12/09 SO)H.Yamagishi for 拡張印刷 ***

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    Private Sub CmdAction01_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction01.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ作成画面(呼出)開始", "成功", "")

            Dim KFKMAIN010 As New KFKMAIN010
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKMAIN010, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ作成画面(呼出)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ作成画面(呼出)終了", "成功", "")
        End Try

    End Sub

    Private Sub CmdAction02_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction02.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ作成取消画面(呼出)開始", "成功", "")

            Dim KFKMAIN020 As New KFKMAIN020
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKMAIN020, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ作成取消画面(呼出)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ作成取消画面呼出)終了", "成功", "")
        End Try

    End Sub

    Private Sub CmdAction03_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction03.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ結果更新画面(呼出)開始", "成功", "")

            Dim KFKMAIN030 As New KFKMAIN030
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKMAIN030, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ結果更新画面(呼出)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ結果更新画面(呼出)終了", "成功", "")
        End Try

    End Sub

    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Private Sub CmdAction04_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction04.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ書込画面(呼出)開始", "成功", "")

            Dim KFKMAIN040 As New KFKMAIN040
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKMAIN040, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ書込画面(呼出)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済リエンタ書込画面(呼出)終了", "成功", "")
        End Try

    End Sub
    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

    Private Sub CmdAction10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction10.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "自振処理企業一覧表印刷画面(呼出)開始", "成功", "")

            Dim KFKPRNT010 As New KFKPRNT010
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT010, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "自振処理企業一覧表印刷画面(呼出)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "自振処理企業一覧表印刷画面(呼出)終了", "成功", "")
        End Try

    End Sub

    Private Sub CmdAction11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction11.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済企業一覧表印刷画面(呼出)開始", "成功", "")

            Dim KFKPRNT020 As New KFKPRNT020
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT020, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済企業一覧表印刷画面(呼出)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "資金決済企業一覧表印刷画面(呼出)終了", "成功", "")

        End Try

    End Sub

    Private Sub CmdAction12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction12.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料未徴求企業一覧表印刷(呼出)開始", "成功", "")

            Dim KFKPRNT030 As New KFKPRNT030
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT030, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料未徴求企業一覧表印刷(呼出)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料未徴求企業一覧表印刷(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub CmdAction13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction13.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料一括徴求明細表印刷(呼出)開始", "成功", "")

            Dim KFKPRNT040 As New KFKPRNT040
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT040, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料一括徴求明細表印刷(呼出)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料一括徴求明細表印刷(呼出)終了", "成功", "")
        End Try

    End Sub

    Private Sub CmdAction14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction14.Click
        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "処理結果確認表再印刷画面(呼出)開始", "成功", "")

            Dim KFKPRNT050 As New KFKPRNT050
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT050, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "処理結果確認表再印刷画面(呼出)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "処理結果確認表再印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    '処理選択画面へ戻る
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")

            Dim MenuModule As String = "MENU.EXE"
            Dim BinDirectory As String = GCom.SET_PATH(GCom.GetBinFolder)

            If System.IO.File.Exists(BinDirectory & MenuModule) Then
                Call StartProc(BinDirectory, MenuModule)
            End If
            Me.Close()
            Me.Dispose()
            Application.Exit()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try

    End Sub

    Private Function StartProc(ByVal dir As String, ByVal exename As String, Optional ByVal wait As Boolean = False) As String
        Dim Arguments As String = LW.UserID    '引数
        Try
            Dim ExecProc As Process = Process.Start(dir & exename, Arguments)
            If ExecProc.Id <> 0 Then
                Me.Visible = False
                If wait = True Then
                    '終了待機
                    ExecProc.WaitForExit()
                    Me.Visible = True
                    Me.Activate()
                Else
                    Me.Close()
                End If
            Else
                Throw New Exception(String.Format("アプリケーションの起動に失敗しました。'{0}'", exename))
            End If

        Catch ex As Exception
            Dim MessageText As String
            MessageText = ex.Message
            MessageText &= Environment.NewLine
            MessageText &= dir & exename
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "アプリケーション起動", "失敗", ex.Message)
        End Try

        Return ""
    End Function


    Private Sub CmdAction15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdAction15.Click
        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "預金口座振替内訳票手数料請求書印刷画面(呼出)開始", "成功", "")

            Dim KFKPRNT060 As New KFKPRNT060
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFKPRNT060, Form), Me)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "預金口座振替内訳票手数料請求書印刷画面(呼出)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "預金口座振替内訳票手数料請求書印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    '-----------------------------------
    ' 設定ファイル取得
    '-----------------------------------
    Private Function GetIniInfo() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "開始", "")

            '--------------------------------
            ' RSV2機能設定
            '--------------------------------
            INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            If INI_RSV2_EDITION.ToUpper = "ERR" OrElse INI_RSV2_EDITION = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "RSV2機能設定", "RSV2_V1.0.0", "EDITION"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:RSV2機能設定 分類:RSV2_V1.0.0 項目:EDITION")
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "終了", "")
        End Try
    End Function

    '-----------------------------------
    ' 画面初期設定
    '-----------------------------------
    Private Function SetDisplayInit() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "画面初期設定", "開始", "")

            Select Case INI_RSV2_EDITION
                Case "2"
                    Me.CmdAction04.Tag = 1
                Case Else
                    Me.CmdAction04.Tag = 0
            End Select

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "画面初期設定", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "画面初期設定", "終了", "")
        End Try
    End Function
    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END
End Class
