Imports System
Imports CASTCommon
Imports System.Text
Public Class KFJMENU010
    Inherits System.Windows.Forms.Form
    Private noClose As Boolean
    Private MyOwnerForm As Form

    Private MainLOG As New CASTCommon.BatchLOG("KFJMENU010", "口座振替メニュー")
    Private Const msgTitle As String = "口座振替メニュー(KFJMENU010)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
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

    ' 2016/10/11 タスク）綾部 ADD 【PG】UI_11-1-4(飯田信金 再振データ作成機能カスタマイズ対応) -------------------- START
    ' 再振データ作成機能選択
    '  1:標準(個別実行) 2:拡張(一括実行)
    Private INI_RSV2_SFURITYPE As String
    ' 2016/10/11 タスク）綾部 ADD 【PG】UI_11-1-4(飯田信金 再振データ作成機能カスタマイズ対応) -------------------- END

#Region " ロード"
    Private Sub KFJMENU010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            'GCom.GetUserID = gstrUSER_ID
            'lblUser.Text = GCom.GetUserID
            'lblDate.Text = String.Format("{0:yyyy年MM月dd日}", GCom.GetSysDate)

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            GOwnerForm = Me
            MyOwnerForm = GOwnerForm

            ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
            If GetIniInfo() = False Then
                Exit Try
            End If

            If SetDisplayInit() = False Then
                Exit Try
            End If
            ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

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
            clsExDsp.Read_Menu(gstrUSER_ID, Me, TAB, CAstExternal.ClsExternalMenu.ExternalMENU_JIFURI)
            ' 2016/09/01 タスク）綾部 ADD 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

            '*** Str Add 2015/12/09 SO)H.Yamagishi for 拡張印刷 ***
            Dim clsExPrt As New CAstExtendPrint.ClsExtendPrintMenu()
            clsExPrt.Read_PrtMenu(gstrUSER_ID, Me, TAB, CAstExtendPrint.ClsExtendPrintMenu.EXPRTMENU_JIFURI)
            '*** End Add 2015/12/09 SO)H.Yamagishi for 拡張印刷 ***

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
#End Region
#Region " 日常業務"
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        '口振依頼データ落込
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口振依頼データ落込画面(呼出)開始", "成功", "")

            Dim KFJMAIN010 As New KFJMAIN010
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN010, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口振依頼データ落込画面(呼出)終了", "成功", "")
        End Try
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        '他行データ作成
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "他行データ作成画面(呼出)開始", "成功", "")

            Dim KFJMAIN020 As New KFJMAIN020
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN020, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "他行データ作成画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        'センターカットデータ作成
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "センターカットデータ作成画面(呼出)開始", "成功", "")

            Dim KFJMAIN030 As New KFJMAIN030
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN030, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "センターカットデータ作成画面(呼出)終了", "成功", "")
        End Try
    End Sub

    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "他行データ媒体書込画面(呼出)開始", "成功", "")
            Dim KFJMAIN110 As New KFJMAIN110
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN110, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "他行データ媒体書込画面(呼出)終了", "成功", "")
        End Try
    End Sub
    ' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        '不能結果更新
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "不能結果更新画面(呼出)開始", "成功", "")

            Dim KFJMAIN040 As New KFJMAIN040
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN040, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "不能結果更新画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        '日報集計
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "日報集計画面(呼出)開始", "成功", "")

            Dim KFJMAIN050 As New KFJMAIN050
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN050, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "日報集計画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        '返還データ作成
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "返還データ作成画面(呼出)開始", "成功", "")
            Dim KFJMAIN060 As New KFJMAIN060
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN060, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "返還データ作成画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click
        '手数料計算作成
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料計算画面(呼出)開始", "成功", "")
            Dim KFJMAIN070 As New KFJMAIN070
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN070, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料計算画面(呼出)終了", "成功", "")
        End Try
    End Sub
    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        '再振データ作成
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振データ作成画面(呼出)開始", "成功", "")
            ' 2016/10/11 タスク）綾部 CHG 【PG】UI_11-1-4(飯田信金 再振データ作成機能カスタマイズ対応) -------------------- START
            'Dim KFJMAIN080 As New KFJMAIN080
            'Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN080, Form), Me)
            Select Case INI_RSV2_SFURITYPE
                Case "1"
                    Dim KFJMAIN080 As New KFJMAIN080
                    Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN080, Form), Me)
                Case "2"
                    Dim KFJMAIN081 As New KFJMAIN081
                    Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN081, Form), Me)
            End Select
            ' 2016/10/11 タスク）綾部 CHG 【PG】UI_11-1-4(飯田信金 再振データ作成機能カスタマイズ対応) -------------------- START
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振データ作成画面(呼出)終了", "成功", "")
        End Try
    End Sub

    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    '-----------------------------------
    ' 依頼データ落込(一括)
    '-----------------------------------
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "依頼データ落込(一括)画面(呼出)開始", "成功", "")
            Dim KFJMAIN090 As New KFJMAIN090
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN090, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "依頼データ落込(一括)画面(呼出)終了", "成功", "")
        End Try
    End Sub

    '-----------------------------------
    ' 返還データ作成(一括)
    '-----------------------------------
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "返還データ作成(一括)画面(呼出)開始", "成功", "")
            Dim KFJMAIN100 As New KFJMAIN100
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAIN100, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "返還データ作成(一括)画面(呼出)終了", "成功", "")
        End Try
    End Sub
    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

#End Region
#Region " マスタ管理"
    Private Sub Button32_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button32.Click
        '取引先マスタメンテナンス
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタメンテナンス画面(呼出)開始", "成功", "")

            ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- START
            'Dim KFJMAST010 As New KFJMAST010
            'Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST010, Form), Me)
            Select INI_RSV2_MASTPTN
                Case "1"
                    Dim KFJMAST010 As New KFJMAST010
                    Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST010, Form), Me)
                Case "2"
                    Dim KFJMAST011 As New KFJMAST011
                    Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST011, Form), Me)
            End Select
            ' 2016/09/01 タスク）綾部 CHG 【PG】(飯田信金 カスタマイズ外対応(大規模マスタ統合)) -------------------- END

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタメンテナンス画面(呼出)終了", "成功", "")
        End Try
    End Sub
    Private Sub Button31_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button31.Click
        '月間スケジュール作成
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "月間スケジュール作成画面(呼出)開始", "成功", "")
            Dim KFJMAST020 As New KFJMAST020
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST020, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "月間スケジュール作成画面(呼出)終了", "成功", "")
        End Try
    End Sub
    Private Sub Button30_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button30.Click
        'スケジュールメンテナンス
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールメンテナンス画面(呼出)開始", "成功", "")
            Dim KFJMAST030 As New KFJMAST030
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST030, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールメンテナンス画面(呼出)終了", "成功", "")
        End Try
    End Sub
    Private Sub Button29_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button29.Click
        '他行マスタメンテナンス
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "他行マスタメンテナンス画面(呼出)開始", "成功", "")
            Dim KFJMAST040 As New KFJMAST040
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST040, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "他行マスタメンテナンス画面(呼出)終了", "成功", "")
        End Try
    End Sub
    Private Sub Button27_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button27.Click
        '支店読替マスタメンテナンス
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "支店読替マスタメンテナンス画面(呼出)開始", "成功", "")
            Dim KFJMAST050 As New KFJMAST050
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST050, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "支店読替マスタメンテナンス画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button17_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button17.Click
        '手数料徴求フラグ一括更新
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料徴求フラグ一括更新画面(呼出)開始", "成功", "")
            Dim KFJMAST100 As New KFJMAST100
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST100, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "手数料徴求フラグ一括更新画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button18_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button18.Click
        'スケジュール変更
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュール変更画面(呼出)開始", "成功", "")
            Dim KFJMAST070 As New KFJMAST070
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST070, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュール変更画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button19_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button19.Click
        'スケジュール管理
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュール管理画面(呼出)開始", "成功", "")
            Dim KFJMAST060 As New KFJMAST060
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST060, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュール管理画面(呼出)終了", "成功", "")
        End Try
    End Sub

    ''' <summary>
    ''' 振込手数料マスタメンテナンスボタン押下イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>2013/11/25 saitou 消費税対応 ADD</remarks>
    Private Sub Button25_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button25.Click
        '振込手数料マスタメンテナンス
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振込手数料マスタメンテナンス画面(呼出)開始", "成功", "")
            Dim KFJMAST090 As New KFJMAST090
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJMAST090, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振込手数料マスタメンテナンス画面(呼出)終了", "成功", "")
        End Try
    End Sub

#End Region
#Region " エントリ"
    Private Sub Button49_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button49.Click
        '依頼書用振替口座登録
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "依頼書用振替口座登録画面(呼出)開始", "成功", "")
            Dim KFJENTR010 As New KFJENTR010
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJENTR010, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "依頼書用振替口座登録画面(呼出)終了", "成功", "")
        End Try
    End Sub


    Private Sub Button48_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button48.Click
        '口座振替依頼書印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座振替依頼書印刷画面(呼出)開始", "成功", "")
            Dim KFJENTR020 As New KFJENTR020
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJENTR020, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座振替依頼書印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button47_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button47.Click
        '依頼書用データ入力
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "依頼書用データ入力画面(呼出)開始", "成功", "")
            Dim KFJENTR030 As New KFJENTR030
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJENTR030, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "依頼書用データ入力画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button46_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button46.Click
        '伝票用データ入力
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "伝票用データ入力画面(呼出)開始", "成功", "")
            Dim KFJENTR040 As New KFJENTR040
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJENTR040, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "伝票用データ入力画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button36_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button36.Click
        '口座振替入力チェックリスト印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座振替入力チェックリスト印刷画面(呼出)開始", "成功", "")
            Dim KFJENTR050 As New KFJENTR050
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJENTR050, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座振替入力チェックリスト印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button35_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button35.Click
        '契約者一覧表印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "契約者一覧表印刷画面(呼出)開始", "成功", "")
            Dim KFJENTR060 As New KFJENTR060
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJENTR060, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "契約者一覧表印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button43_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button43.Click
        '振替口座一括削除
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替口座一括削除画面(呼出)開始", "成功", "")
            Dim KFJENTR070 As New KFJENTR070
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJENTR070, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替口座一括削除画面(呼出)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " その他"
    Private Sub Button65_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button65.Click
        'センターカットデータ作成取消
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "センターカットデータ作成取消画面(呼出)開始", "成功", "")
            Dim KFJOTHR010 As New KFJOTHR010
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJOTHR010, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "センターカットデータ作成取消画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button64_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button64.Click
        '振替結果変更
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替結果変更画面(呼出)開始", "成功", "")
            Dim KFJOTHR020 As New KFJOTHR020
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJOTHR020, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替結果変更画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button63_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button63.Click
        'センター直接持込落込取消
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "センター直接持込落込取消画面(呼出)開始", "成功", "")
            Dim KFJOTHR030 As New KFJOTHR030
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJOTHR030, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "センター直接持込落込取消画面(呼出)終了", "成功", "")
        End Try
    End Sub

#End Region
#Region " 帳票印刷"
    Private Sub Button81_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button81.Click
        '取引先マスタチェックリスト印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタチェックリスト印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT010 As New KFJPRNT010
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT010, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタチェックリスト印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub
    Private Sub Button80_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button80.Click
        '口座振替明細表印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座振替明細表印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT020 As New KFJPRNT020
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT020, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座振替明細表印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button79_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button79.Click
        'スケジュール表印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュール表印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT030 As New KFJPRNT030
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT030, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュール表印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button78_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button78.Click
        '取引先マスタ索引簿印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタ索引簿印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT040 As New KFJPRNT040
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT040, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタ索引簿印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button76_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button76.Click
        '処理結果確認表再印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "処理結果確認表再印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT050 As New KFJPRNT050
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT050, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "処理結果確認表再印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button74_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button74.Click
        '振替結果変更チェックリスト印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替結果変更チェックリスト印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT060 As New KFJPRNT060
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT060, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替結果変更チェックリスト印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button72_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button72.Click
        '未処理一覧表(落込)印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "未処理一覧表(落込)印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT070 As New KFJPRNT070
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT070, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "未処理一覧表(落込)印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub
    Private Sub Button70_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button70.Click
        '振替結果明細表印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替結果明細表印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT080 As New KFJPRNT080
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT080, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替結果明細表印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button68_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button68.Click
        '自振管理リスト印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "自振管理リスト印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT090 As New KFJPRNT090
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT090, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "自振管理リスト印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    '2010/09/14.Sakon　店別集計表（画面印刷）追加
    Private Sub Button66_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button66.Click
        '店別集計表印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "店別集計表印刷印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT219 As New KFJPRNT219
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT219, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "店別集計表印刷印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button67_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button67.Click
        '処理結果確認表一覧印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "処理結果確認表一覧印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT100 As New KFJPRNT100
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT100, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "処理結果確認表一覧印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    '2010/09/21.Sakon　受付明細表（画面印刷）追加
    Private Sub Button77_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button77.Click
        '受付明細表印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "受付明細表印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT110 As New KFJPRNT110
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT110, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "受付明細表印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    '2010/09/22.Sakon　振替不能事由別集計表（画面印刷）追加
    Private Sub Button75_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button75.Click
        '振替不能事由別集計表印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替不能事由別集計表印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT120 As New KFJPRNT120
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT120, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替不能事由別集計表印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

    ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-09(RSV2対応<小浜信金>) -------------------- START
    Private Sub Button73_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button73.Click
        'データ伝送通知書印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ伝送通知書印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT130 As New KFJPRNT130
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT130, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ伝送通知書印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub
    ' 2016/06/13 タスク）綾部 ADD 【PG】UI-03-09(RSV2対応<小浜信金>) -------------------- END

    ' 2016/10/11 タスク）綾部 ADD 【PG】UI_11-1-4(飯田信金 再振データ作成機能カスタマイズ対応) -------------------- START
    Private Sub Button71_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button71.Click
        '再振対象先チェックリスト印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振対象先チェックリスト印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT140 As New KFJPRNT140
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT140, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振対象先チェックリスト印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub
    ' 2016/10/11 タスク）綾部 ADD 【PG】UI_11-1-4(飯田信金 再振データ作成機能カスタマイズ対応) -------------------- END

    '2017/05/09 タスク）西野 ADD 標準版修正（取引先マスタ項目確認票印刷機能追加）---------------------- START
    Private Sub Button69_Click(sender As Object, e As EventArgs) Handles Button69.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタ項目確認票印刷画面(呼出)開始", "成功", "")
            Dim KFJPRNT150 As New KFJPRNT150
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJPRNT150, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタ項目確認票印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub
    '2017/05/09 タスク）西野 ADD 標準版修正（取引先マスタ項目確認票印刷機能追加）---------------------- END

#End Region
#Region " 他行処理"
    Private Sub Button97_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button97.Click
        '受信ファイル一括振分(元請)
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "受信ファイル一括振分(元請)画面(呼出)開始", "成功", "")
            Dim KFJTAKO010 As New KFJTAKO010
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJTAKO010, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "受信ファイル一括振分(元請)画面(呼出)終了", "成功", "")
        End Try
    End Sub
    Private Sub Button96_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button96.Click
        '一括送信ファイル作成(元請)
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "一括送信ファイル作成(元請)画面(呼出)開始", "成功", "")
            Dim KFJTAKO020 As New KFJTAKO020
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJTAKO020, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "一括送信ファイル作成(元請)画面(呼出)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 年金処理（廃止）"
    'Private Sub Button128_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button128.Click
    '    '年金振込支店ｺｰﾄﾞﾁｪｯｸﾘｽﾄ印刷
    '    Try
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "年金振込支店ｺｰﾄﾞﾁｪｯｸﾘｽﾄ印刷画面(呼出)開始", "成功", "")
    '        Dim KFJNENK020 As New KFJNENK020
    '        Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJNENK020, Form), Me)
    '    Catch ex As Exception
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
    '        MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    Finally
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "年金振込支店ｺｰﾄﾞﾁｪｯｸﾘｽﾄ印刷画面(呼出)終了", "成功", "")
    '    End Try
    'End Sub

    'Private Sub Button127_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button127.Click
    '    '年金振込支店コード変更
    '    Try
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "年金振込支店コード変更画面(呼出)開始", "成功", "")
    '        Dim KFJNENK030 As New KFJNENK030
    '        Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJNENK030, Form), Me)
    '    Catch ex As Exception
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
    '        MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    Finally
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "年金振込支店コード変更画面(呼出)終了", "成功", "")
    '    End Try
    'End Sub
    'Private Sub Button126_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button126.Click
    '    '年金受取人別振込明細表印刷
    '    Try
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "年金受取人別振込明細表印刷画面(呼出)開始", "成功", "")
    '        Dim KFJNENK040 As New KFJNENK040
    '        Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJNENK040, Form), Me)
    '    Catch ex As Exception
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
    '        MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    Finally
    '        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "年金受取人別振込明細表印刷画面(呼出)終了", "成功", "")
    '    End Try
    'End Sub
#End Region
#Region " 口座オプション"
    Private Sub Button113_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button113.Click
        '口座チェック（当日受付分）
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座チェック（当日受付分）画面(呼出)開始", "成功", "")
            Dim KFJKOZA010 As New KFJKOZA010
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJKOZA010, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座チェック（当日受付分）画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button112_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button112.Click
        '口座チェック（随時）
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座チェック（随時）画面(呼出)開始", "成功", "")
            Dim KFJKOZA020 As New KFJKOZA020
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJKOZA020, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座チェック（随時）画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button100_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button100.Click
        '自振契約リエンタ作成
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "自振契約リエンタ作成画面(呼出)開始", "成功", "")
            Dim KFJKOZA030 As New KFJKOZA030
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJKOZA030, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "自振契約リエンタ作成画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button99_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button99.Click
        '自振契約リエンタ結果更新
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "自振契約リエンタ結果更新画面(呼出)開始", "成功", "")
            Dim KFJKOZA040 As New KFJKOZA040
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJKOZA040, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "自振契約リエンタ結果更新画面(呼出)終了", "成功", "")
        End Try
    End Sub

    Private Sub Button98_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button98.Click
        '自振契約リエンタ書込
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "自振契約リエンタ書込画面(呼出)開始", "成功", "")
            Dim KFJKOZA050 As New KFJKOZA050
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJKOZA050, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "自振契約リエンタ結果更新画面(呼出)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 国税オプション"
    Private Sub Button145_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '国税特殊帳票印刷
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "国税特殊帳票印刷画面(呼出)開始", "成功", "")
            Dim KFJKOKZ010 As New KFJKOKZ010
            Call CASTCommon.ShowFORM(gstrUSER_ID, CType(KFJKOKZ010, Form), Me)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "国税特殊帳票印刷画面(呼出)終了", "成功", "")
        End Try
    End Sub

#End Region
#Region " クローズ"
    Private Sub KFJMENU010_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
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

#Region " 関数"

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

            ' 2016/10/11 タスク）綾部 ADD 【PG】UI_11-1-4(飯田信金 再振データ作成機能カスタマイズ対応) -------------------- START
            '--------------------------------
            ' 再振データ作成機能選択
            '--------------------------------
            INI_RSV2_SFURITYPE = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SFURITYPE")
            If INI_RSV2_SFURITYPE.ToUpper = "ERR" OrElse INI_RSV2_SFURITYPE = Nothing Then
                INI_RSV2_SFURITYPE = "1"
            Else
                Select Case INI_RSV2_SFURITYPE
                    Case "1", "2"
                        ' 取得ＯＫ
                    Case Else
                        MessageBox.Show(String.Format(MSG0001E, "再振データ作成機能", "RSV2_V1.0.0", "SFURITYPE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "設定ファイル取得", "失敗", "項目名:再振データ作成機能 分類:RSV2_V1.0.0 項目:SFURITYPE")
                        Return False
                End Select
            End If
            ' 2016/10/11 タスク）綾部 ADD 【PG】UI_11-1-4(飯田信金 再振データ作成機能カスタマイズ対応) -------------------- END

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
                    Me.Button2.Tag = 1
                    Me.Button4.Tag = 1
                    Me.Button6.Tag = 1
                    Me.Button98.Tag = 1
                Case Else
                    Me.Button2.Tag = 0
                    Me.Button4.Tag = 0
                    Me.Button6.Tag = 0
                    Me.Button98.Tag = 0
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
    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

#End Region

End Class