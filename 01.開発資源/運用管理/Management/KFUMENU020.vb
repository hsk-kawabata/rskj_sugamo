Option Explicit On
Option Strict On
Imports System
Imports CASTCommon
Imports System.Text

Public Class KFUMENU020
    Inherits System.Windows.Forms.Form
    Private Mainlog As New CASTCommon.BatchLOG("KFUMENU020", "運用管理メニュー")
    Private Const msgTitle As String = "運用管理メニュー(KFUMENU020)"
    Private MyOwnerForm As Form
    Private noClose As Boolean

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private Sub KFUMENU020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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

            Mainlog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '------------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------------
            'GCom.GetUserID = gstrUSER_ID
            'lblUser.Text = GCom.GetUserID
            'lblDate.Text = String.Format("{0:yyyy年MM月dd日}", GCom.GetSysDate)

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            GOwnerForm = Me
            MyOwnerForm = GOwnerForm

            '未使用のボタンを非表示にする。
            For Each CTL As Control In TabPage1.Controls
                If TypeOf CTL Is Button Then
                    CTL.Visible = (GCom.NzInt(CTL.Tag) > 0)
                End If
            Next CTL

            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
            Dim clsExDsp As New CAstExternal.ClsExternalMenu()
            clsExDsp.Read_Menu(gstrUSER_ID, Me, TabControl1, CAstExternal.ClsExternalMenu.ExternalMENU_UNYO)
            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
            Dim clsExPrt As New CAstExtendPrint.ClsExtendPrintMenu()
            clsExPrt.Read_PrtMenu(gstrUSER_ID, Me, TabControl1, CAstExtendPrint.ClsExtendPrintMenu.EXPRTMENU_UNYO)
            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

        Catch ex As Exception
            Mainlog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Mainlog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    Private Sub btnDailyJob1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDailyJob1.Click
        Call CASTCommon.ShowFORM(Management.gstrUSER_ID, New KFUMAST010, Me)
    End Sub

    Private Sub btnDailyJob2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDailyJob2.Click
        Call CASTCommon.ShowFORM(Management.gstrUSER_ID, New KFUMAST020, Me)
    End Sub

    Private Sub btnDailyJob3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDailyJob3.Click
        Call CASTCommon.ShowFORM(Management.gstrUSER_ID, New KFUMAST030, Me)
    End Sub

    Private Sub btnDailyJob4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDailyJob4.Click
        Call CASTCommon.ShowFORM(Management.gstrUSER_ID, New KFUMAST040, Me)
    End Sub

    Private Sub btnDailyJob5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDailyJob5.Click
        Call CASTCommon.ShowFORM(Management.gstrUSER_ID, New KFUMAST050, Me)
    End Sub

    Private Sub btnDailyJob9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDailyJob9.Click
        Call CASTCommon.ShowFORM(Management.gstrUSER_ID, New KFULOGR010, Me)
    End Sub

    Private Sub btnDailyJob10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDailyJob10.Click
        Call CASTCommon.ShowFORM(Management.gstrUSER_ID, New KFULOGR020, Me)
    End Sub

#Region " クローズ"
    Private Sub KFJMENU010_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub
    '処理選択画面へ戻る
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEND.Click
        Try
            noClose = True
            Mainlog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")

            Dim MenuModule As String = "MENU.EXE"
            Dim BinDirectory As String = GCom.SET_PATH(GCom.GetBinFolder)

            If System.IO.File.Exists(BinDirectory & MenuModule) Then
                Call StartProc(BinDirectory, MenuModule)
            End If
            Me.Close()
            Application.Exit()
        Catch ex As Exception
            Mainlog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Mainlog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
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
            Mainlog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "アプリケーション起動", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return ""
    End Function
#End Region

    ''' <summary>
    ''' 消費税マスタメンテナンスボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnDailyJob12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDailyJob12.Click
        Call CASTCommon.ShowFORM(Management.gstrUSER_ID, New KFUMAST060, Me)
    End Sub

    ''' <summary>
    ''' 印紙税マスタメンテナンスボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>2013/12/27 saitou 消費税対応 add</remarks>
    Private Sub btnDailyJob13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDailyJob13.Click
        Call CASTCommon.ShowFORM(Management.gstrUSER_ID, New KFUMAST070, Me)
    End Sub

    ''' <summary>
    ''' 提携区分マスタメンテナンスボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>2017/01/18 saitou 東春信金(RSV2標準) added for スリーエス対応</remarks>
    Private Sub btnDailyJob7_Click(sender As Object, e As EventArgs) Handles btnDailyJob7.Click
        Call CASTCommon.ShowFORM(Management.gstrUSER_ID, New KFUMAST080, Me)
    End Sub
End Class
