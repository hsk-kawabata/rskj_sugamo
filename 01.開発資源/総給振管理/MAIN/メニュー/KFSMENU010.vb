Option Explicit On
Option Strict On

Imports CASTCommon
Imports CAstExternal

Public Class KFSMENU010

#Region "変数宣言"
    Private noClose As Boolean
    Private MyOwnerForm As Form

    Private MainLOG As New CASTCommon.BatchLOG("KFSMENU010", "総合振込メニュー")
    Private Const msgTitle As String = "総合振込メニュー(KFSMENU010)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振込日
    End Structure

    Private LW As LogWrite

    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Private INI_RSV2_EDITION As String
    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

    ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
    ' 取引先マスタのマスタパターン指定
    '  1:標準(RSV1と同様) 2:大規模版的なもの
    Private INI_RSV2_MASTPTN As String
    ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

#End Region

#Region "ロード"
    Private Sub KFSMENU010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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

            ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
            If GetIniInfo() = False Then
                Exit Try
            End If
            ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

            '*** Str Add 2015/12/26 sys)mori for【PG】UI_B-14-8(RSV2対応)
            '*********************************************
            ' 使用画面初期設定
            '*********************************************
            Dim ERRMSG As String = ""
            If SetDisplayInfo(ERRMSG) = False Then
                If ERRMSG <> "" Then
                    MessageBox.Show(ERRMSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                Exit Try
            End If
            '*** end Add 2015/12/26 sys)mori for 【PG】UI_B-14-8(RSV2対応)

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

            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
            Dim clsExDsp As New CAstExternal.ClsExternalMenu()
            clsExDsp.Read_Menu(gstrUSER_ID, Me, TAB, CAstExternal.ClsExternalMenu.ExternalMENU_SOUFURI)
            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

            '*** Str Add 2015/12/09 SO)H.Yamagishi for 拡張印刷 ***
            Dim clsExPrt As New CAstExtendPrint.ClsExtendPrintMenu()
            clsExPrt.Read_PrtMenu(gstrUSER_ID, Me, TAB, CAstExtendPrint.ClsExtendPrintMenu.EXPRTMENU_SOUFURI)
            '*** End Add 2015/12/09 SO)H.Yamagishi for 拡張印刷 ***

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region "日常業務"
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        '振込依頼データ落込
        Call ShowForm("振込依頼データ落込", New KFSMAIN010)
    End Sub

    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        '依頼データ落込（一括）
        Call ShowForm("依頼データ落込（一括）", New KFSMAIN070)
    End Sub
    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        '振込発信リエンタ作成
        Call ShowForm("振込発信リエンタ作成", New KFSMAIN040)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        '振込発信データ作成
        Call ShowForm("振込発信データ作成", New KFSMAIN045)
    End Sub

    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        '振込発信リエンタ書込
        Call ShowForm("振込発信リエンタ書込", New KFSMAIN080)
    End Sub
    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

    ' 2016/10/12 タスク）綾部 ADD 【PG】UI_11-1-15(飯田信金 総合振込金バッチ連携カスタマイズ対応) -------------------- START
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        '振込発信データ作成(通常定例)
        Call ShowForm("振込発信データ作成(通常定例)", New KFSMAIN090)
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        '振込発信データ作成(当日異例)
        Call ShowForm("振込発信データ作成(当日異例)", New KFSMAIN100)
    End Sub
    ' 2016/10/12 タスク）綾部 ADD 【PG】UI_11-1-15(飯田信金 総合振込金バッチ連携カスタマイズ対応) -------------------- END

#End Region

#Region "マスタ管理"
    Private Sub Button32_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button32.Click
        '取引先マスタメンテナンス

        ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
        'Call ShowForm("取引先マスタメンテナンス", New KFSMAST010)
        Select Case INI_RSV2_MASTPTN
            Case "1"
                Call ShowForm("取引先マスタメンテナンス", New KFSMAST010C)
            Case "2"
                Call ShowForm("取引先マスタメンテナンス", New KFSMAST010C)
        End Select
        ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

    End Sub

    Private Sub Button31_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button31.Click
        '月間スケジュール作成
        Call ShowForm("月間スケジュール作成", New KFSMAST020)
    End Sub

    Private Sub Button30_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button30.Click
        'スケジュールメンテナンス
        Call ShowForm("スケジュールメンテナンス", New KFSMAST030)
    End Sub

    Private Sub Button19_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button19.Click
        'スケジュール管理
        Call ShowForm("スケジュール管理", New KFSMAST040)
    End Sub

    ''' <summary>
    ''' 振込手数料マスタメンテナンスボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>2013/11/25 saitou 消費税対応 ADD</remarks>
    Private Sub Button29_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button29.Click
        '振込手数料マスタメンテナンス
        Call ShowForm("振込手数料マスタメンテナンス", New KFSMAST050)
    End Sub

#End Region

#Region "エントリ"
    Private Sub Button49_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button49.Click
        '依頼書用振込口座登録
        Call ShowForm("依頼書用振込口座登録", New KFSENTR010)
    End Sub

    Private Sub Button48_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button48.Click
        '総合振込依頼書印刷
        Call ShowForm("総合振込依頼書印刷", New KFSENTR020)
    End Sub

    Private Sub Button47_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button47.Click
        '依頼書用データ入力
        Call ShowForm("依頼書用データ入力", New KFSENTR030)
    End Sub

    Private Sub Button46_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button46.Click
        '伝票用データ入力
        Call ShowForm("伝票用データ入力", New KFSENTR040)
    End Sub

    Private Sub Button36_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button36.Click
        '総合振込入力チェックリスト印刷
        Call ShowForm("総合振込入力チェックリスト印刷", New KFSENTR050)
    End Sub

    Private Sub Button35_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button35.Click
        '契約者一覧表印刷
        Call ShowForm("契約者一覧表印刷", New KFSENTR060)
    End Sub
    Private Sub Button43_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button43.Click
        '振込口座一括削除
        Call ShowForm("振込口座一括削除", New KFSENTR070)
    End Sub

#End Region

#Region "その他"
    Private Sub Button65_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button65.Click
        '振込発信リエンタ作成取消
        Call ShowForm("振込発信リエンタ作成取消", New KFSOTHR020)
    End Sub

    Private Sub Button64_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button64.Click
        '為替明細発信停止
        Call ShowForm("為替明細発信停止", New KFSOTHR040)
    End Sub

    Private Sub Button63_Click(sender As Object, e As EventArgs) Handles Button63.Click
        '振込発信データ作成取消
        Call ShowForm("振込発信データ作成取消", New KFSOTHR025)
    End Sub

#End Region

#Region "帳票印刷"
    Private Sub Button72_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button72.Click
        '取引先マスタチェックリスト印刷
        Call ShowForm("取引先マスタチェックリスト印刷", New KFSPRNT010)
    End Sub

    Private Sub Button81_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button81.Click
        '総合振込明細表印刷
        Call ShowForm("総合振込明細表印刷", New KFSPRNT020)
    End Sub

    Private Sub Button79_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button79.Click
        'スケジュール表印刷
        Call ShowForm("スケジュール表印刷", New KFSPRNT030)
    End Sub

    Private Sub Button78_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button78.Click
        '取引先マスタ索引簿印刷
        Call ShowForm("取引先マスタ索引簿印刷", New KFSPRNT040)
    End Sub

    Private Sub Button76_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button76.Click
        '処理結果確認表再印刷
        Call ShowForm("処理結果確認表再印刷", New KFSPRNT050)
    End Sub

    Private Sub Button74_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button74.Click
        '為替振込明細表再印刷
        Call ShowForm("為替振込明細表再印刷", New KFSPRNT060)
    End Sub

    '2017/05/09 タスク）西野 ADD 標準版修正（取引先マスタ項目確認票印刷機能追加）---------------------- START
    Private Sub Button80_Click(sender As Object, e As EventArgs) Handles Button80.Click
        '取引先マスタ項目確認一覧票印刷
        Call ShowForm("取引先マスタ項目確認一覧票印刷", New KFSPRNT070)
    End Sub
    '2017/05/09 タスク）西野 ADD 標準版修正（取引先マスタ項目確認票印刷機能追加）---------------------- END

#End Region

#Region "クローズ"
    Private Sub KFSMENU010_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub

    '処理選択画面へ戻る
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

    '画面呼び出し用関数
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

    '*** Str Add 2015/12/26 sys)mori for【PG】UI_B-14-8(RSV2対応)
#Region "関数"
    Private Function SetDisplayInfo(ByRef ERRMSG As String) As Boolean

        Try

            Dim INI_RSV2_KINBATCH As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "KINBATCH")
            If INI_RSV2_KINBATCH = "err" OrElse INI_RSV2_KINBATCH = "" Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INIファイル取得", "設定ファイル取得失敗 項目名:金バッチ使用有無 分類:RSV2_V1.0.0 項目:KINBATCH")
                ERRMSG = "設定ファイル取得失敗 項目名:金バッチ使用有無 分類:RSV2_V1.0.0 項目:KINBATCH"
                Return False
            End If

            If INI_RSV2_KINBATCH = "1" Then
                Button5.Tag = 1
                Button63.Tag = 1
                Button3.Tag = 0
                Button65.Tag = 0
            Else
                Button5.Tag = 0
                Button63.Tag = 0
                Button3.Tag = 1
                Button65.Tag = 1
            End If

            If INI_RSV2_EDITION = "2" Then
                Button2.Tag = 1
                If INI_RSV2_KINBATCH = "1" Then
                    Button7.Tag = 0
                Else
                    Button7.Tag = 1
                End If
            Else
                Button2.Tag = 0
                Button7.Tag = 0
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.ToriCode, LW.FuriDate, "使用画面初期設定", "失敗", ex.ToString)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "使用画面初期設定", "終了", "")
        End Try

    End Function

    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
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

            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
            '--------------------------------
            ' 取引先マスタパターン
            '--------------------------------
            INI_RSV2_MASTPTN = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MASTPTN")
            If INI_RSV2_MASTPTN.ToUpper = "ERR" OrElse INI_RSV2_MASTPTN = Nothing Then
                INI_RSV2_MASTPTN = "1"
            Else
                Select Case INI_RSV2_MASTPTN
                    Case "1", "2"
                        ' 取得ＯＫ
                    Case Else
                        MessageBox.Show(String.Format(MSG0001E, "取引先マスタパターン", "RSV2_V1.0.0", "MASTPTN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:取引先マスタパターン 分類:RSV2_V1.0.0 項目:MASTPTN")
                        Return False
                End Select
            End If
            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "終了", "")
        End Try
    End Function

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        'スケジュールメンテナンス
        Call ShowForm("基本料金データ作成", New KFSMAIN200C)
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        'スケジュールメンテナンス
        Call ShowForm("後取手数料データ作成", New KFSMAIN210C)
    End Sub
    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

#End Region
    '*** Str Add 2015/12/26 sys)mori for【PG】UI_B-14-8(RSV2対応)
End Class
