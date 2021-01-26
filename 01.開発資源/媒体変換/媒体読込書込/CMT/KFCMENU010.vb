Option Explicit On 
Option Strict On

Imports System.Data.OracleClient
Imports System
Imports System.IO
Imports System.text
Imports CASTCommon
Imports System.Collections
Imports System.String

Public Class KFCMENU010

#Region " 定数・変数 "

    Private Const ThisModuleName As String = "KFCMENU010.vb"
    Private noClose As Boolean

    Private MainLOG As New CASTCommon.BatchLOG("KFCMENU010", "媒体変換メニュー")
    Private Const msgTitle As String = "媒体変換メニュー(KFCMENU010)"
    Private DisplayName As String = ""
    Private MyOwnerForm As Form

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

#End Region

#Region " ロード "

    '================================
    ' 画面起動時処理
    '================================
    Private Sub KFCMENU010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        DisplayName = "ロード"

        Try
            '-------------------------------------
            ' ログ情報取得
            '-------------------------------------
            Dim Temp As String() = Environment.GetCommandLineArgs
            With GCom
                .GetSysDate = Date.Now
                If Temp.Length <= 1 Then
                    .GetUserID = ""
                Else
                    .GetUserID = Temp(1).Trim
                End If
            End With

            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            '-------------------------------------
            ' 画面初期設定
            '-------------------------------------
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "開始", "")

            Me.CmdBack.DialogResult = DialogResult.None
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            GOwnerForm = Me
            MyOwnerForm = GOwnerForm

            '-------------------------------------
            ' 未使用ボタン非表示(Tag)
            '-------------------------------------
            For Each CTL As Control In TabPage1.Controls
                If TypeOf CTL Is Button Then
                    CTL.Visible = (GCom.NzInt(CTL.Tag) > 0)
                End If
            Next CTL

            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
            Dim clsExDsp As New CAstExternal.ClsExternalMenu()
            clsExDsp.Read_Menu(GCom.GetUserID, Me, TabControl1, CAstExternal.ClsExternalMenu.ExternalMENU_BAITAI)
            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
            Dim clsExPrt As New CAstExtendPrint.ClsExtendPrintMenu()
            clsExPrt.Read_PrtMenu(GCom.GetUserID, Me, TabControl1, CAstExtendPrint.ClsExtendPrintMenu.EXPRTMENU_BAITAI)
            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "終了", "")
        End Try
    End Sub

#End Region

#Region " ボタン "

    '================================
    ' 媒体読込（媒体→ディスク）
    '================================
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        DisplayName = "媒体読込（媒体→ディスク）画面(呼出)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "開始", "")

            Dim DispLayClass As New KFCMAIN010
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "終了", "")
        End Try
    End Sub

    '================================
    ' 媒体書込（ディスク→媒体）
    '================================
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        DisplayName = "媒体書込（ディスク→媒体）画面(呼出)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "開始", "")

            Dim DispLayClass As New KFCMAIN020
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "終了", "")
        End Try
    End Sub

    '================================
    ' 未使用ボタン
    '================================
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

    End Sub

    '================================
    ' 未使用ボタン
    '================================
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click

    End Sub

    '================================
    ' 未使用ボタン
    '================================
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click

    End Sub

    '================================
    ' 未使用ボタン
    '================================
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click

    End Sub

    '================================
    ' 未使用ボタン
    '================================
    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click

    End Sub

    '================================
    ' 未使用ボタン
    '================================
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click

    End Sub

    '================================
    ' ＣＭＴ読込
    '================================
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click

        DisplayName = "ＣＭＴ読込画面(呼出)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "開始", "")

            Dim DispLayClass As New KFCCMT011
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "終了", "")
        End Try
    End Sub

    '================================
    ' ＣＭＴ通常書込
    '================================
    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click

        DisplayName = "ＣＭＴ通常書込画面(呼出)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "開始", "")

            Dim DispLayClass As New KFCCMT021
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "終了", "")
        End Try
    End Sub

    '================================
    ' 他行向結果データ取込
    '================================
    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click

        DisplayName = "他行向結果データ取込画面(呼出)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "開始", "")

            Dim DispLayClass As New KFCCMT051
            DispLayClass.SendFlag = False
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "終了", "")
        End Try
    End Sub

    '================================
    ' 他行向請求データ書込ボタン処理
    '================================
    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click

        DisplayName = "他行向請求データ書込画面(呼出)"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "開始", "")

            Dim DispLayClass As New KFCCMT051
            DispLayClass.SendFlag = True
            Call CASTCommon.ShowFORM(LW.UserID, CType(DispLayClass, Form), Me)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "終了", "")
        End Try

    End Sub

    '================================
    ' 未使用ボタン
    '================================
    Private Sub Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click

    End Sub

    '================================
    ' 未使用ボタン
    '================================
    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click

    End Sub

    '================================
    ' 未使用ボタン
    '================================
    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click

    End Sub

    '================================
    ' 未使用ボタン
    '================================
    Private Sub Button16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button16.Click

    End Sub

    '================================
    ' メインメニューボタン
    '================================
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click

        DisplayName = "クローズ"

        Try
            noClose = True
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "開始", "")

            Dim MenuModule As String = "MENU.EXE"
            Dim BinDirectory As String = GCom.SET_PATH(GCom.GetBinFolder)

            If System.IO.File.Exists(BinDirectory & MenuModule) Then
                Call StartProc(BinDirectory, MenuModule)
            End If
            Me.Close()
            Me.Dispose()
            Application.Exit()

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            DisplayName = ""
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, DisplayName, "終了", "")
        End Try

    End Sub

    '================================
    ' 画面クローズ
    '================================
    Private Sub KFJCMT0001G_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub

    '================================
    ' 画面表示（再表示）時処理
    '================================
    Private Sub KFJCMT0001G_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        GCom.GLog.Job1 = "媒体変換処理選択画面"
    End Sub

#End Region

#Region " 関数 "

    Private Function StartProc(ByVal dir As String, ByVal exename As String, Optional ByVal wait As Boolean = False) As String
        Dim Arguments As String = GCom.GetUserID     '引数
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
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "アプリケーション起動", "失敗", MessageText)
            '    MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return ""
    End Function

#End Region

End Class
