Imports CASTCommon

''' <summary>
''' 集金代行メインメニュー画面　メインクラス
''' </summary>
''' <remarks>2017/01/17 saitou 東春信金(RSV2標準) added for スリーエス対応</remarks>
Public Class KF3SMENU010

#Region "クラス変数"
    Private noClose As Boolean
    Private MyOwnerForm As Form

    Private Const msgTitle As String = "集金代行メニュー(KF3SMENU010)"

    'ログ
    Private MainLOG As New CASTCommon.BatchLOG("KF3SMENU010", "集金代行メニュー")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure
    Private LW As LogWrite

#End Region

#Region "イベントハンドラ"

    ''' <summary>
    ''' 画面ロードイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KF3SMENU010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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

            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000-00"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '------------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            GOwnerForm = Me
            MyOwnerForm = GOwnerForm

            '未使用のボタンを非表示にする。
            For Each OBJ As TabPage In TAB.TabPages
                For Each CTL As Control In OBJ.Controls
                    If TypeOf CTL Is Button Then
                        CTL.Visible = (GCom.NzInt(CTL.Tag) > 0)
                        If CTL.Visible Then
                            CTL.Enabled = (GCom.NzInt(CTL.Tag) = 1)
                        End If
                    End If
                Next CTL
            Next OBJ

            Dim clsExDsp As New CAstExternal.ClsExternalMenu()
            clsExDsp.Read_Menu(gstrUSER_ID, Me, TAB, CAstExternal.ClsExternalMenu.ExternalMENU_SSS)

            Dim clsExPrt As New CAstExtendPrint.ClsExtendPrintMenu()
            clsExPrt.Read_PrtMenu(gstrUSER_ID, Me, TAB, CAstExtendPrint.ClsExtendPrintMenu.EXPRTMENU_SSS)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    ''' <summary>
    ''' 画面クローズイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KF3SMENU010_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub

    ''' <summary>
    ''' メインメニューボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click
        Try
            noClose = True
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try

    End Sub

    ''' <summary>
    ''' 他行分センター送信データ作成ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Call ShowForm("他行分センター送信データ作成", New KF3SMAIN010)
    End Sub

    ''' <summary>
    ''' 他行分不能結果更新ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Call ShowForm("他行分不能結果更新", New KF3SMAIN020)
    End Sub

    ''' <summary>
    ''' 他行分センター送信データ作成取消ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Call ShowForm("他行分不能結果更新", New KF3SMAIN030)
    End Sub

    ''' <summary>
    ''' 引落結果合計表印刷ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Call ShowForm("引落結果合計表印刷", New KF3SPRNT010)
    End Sub

    ''' <summary>
    ''' 委託者別決済額一覧表印刷ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Call ShowForm("委託者別決済額一覧表印刷", New KF3SPRNT020)
    End Sub

    ''' <summary>
    ''' 銀行別振替結果合計表印刷ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Call ShowForm("銀行別振替結果合計表印刷", New KF3SPRNT030)
    End Sub

    ''' <summary>
    ''' 企業別結果表印刷ボタンクリックイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Call ShowForm("企業別結果表印刷", New KF3SPRNT040)
    End Sub

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' 画面の呼び出しを行います。
    ''' </summary>
    ''' <param name="gamenname"></param>
    ''' <param name="frm"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ShowForm(ByVal gamenname As String, ByRef frm As Form) As Boolean
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, gamenname & "画面(呼出)開始", "成功", "")
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(frm, Form), Me)
            Return True
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, gamenname & "画面(呼出)終了", "成功", "")
        End Try
    End Function

    ''' <summary>
    ''' アプリケーションの起動を行います。
    ''' </summary>
    ''' <param name="dir"></param>
    ''' <param name="exename"></param>
    ''' <param name="wait"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "アプリケーション起動", "失敗", MessageText)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return ""
    End Function

#End Region

End Class
