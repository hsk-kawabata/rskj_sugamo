' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKMAIN040

#Region " 定数/変数 "

    '--------------------------------
    ' 共通関連項目
    '--------------------------------
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    '--------------------------------
    ' LOG関連項目
    '--------------------------------
    Private MainLOG As New CASTCommon.BatchLOG("KFKMAIN040", "資金決済リエンタ書込画面")
    Private Const msgtitle As String = "資金決済リエンタ書込画面(KFKMAIN040)"

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
        Dim COMMON_RIENTADR As String                ' リエンタファイル格納先
        Dim KESSAI_RIENTANAME As String              ' リエンタファイル名
    End Structure
    Private IniInfo As INI_INFO

#End Region

#Region " ロード "

    Private Sub KFKMAIN040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            ' 画面表示時、処理日にシステム日付を表示
            '------------------------------------------------
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            '------------------------------------------------
            ' 画面初期設定
            '------------------------------------------------
            btnAction.Enabled = False
            cmbTimeStamp.Enabled = False
            btnReset.Enabled = False

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
            ' リエンタファイル検索処理
            '------------------------------------------------
            If GetFileInfo("Search", txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text) = False Then
                MessageBox.Show(MSG0370W.Replace("{0}", "リエンタファイル検索"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "検索", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "検索", "終了", "")
        End Try

    End Sub

    '================================
    ' ★実行ボタン
    '================================
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "開始", "")

            If MessageBox.Show(String.Format(MSG0077I, "資金決済リエンタ", "媒体書込処理"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Try
            End If

            '------------------------------------------------
            ' タイムスタンプの取得
            '------------------------------------------------
            Dim TimeStamp As String = ""
            Dim TimeStamp_Date As String = ""
            Dim TimeStamp_Time As String = ""
            TimeStamp = cmbTimeStamp.SelectedItem
            TimeStamp_Date = TimeStamp.Substring(0, 4) & TimeStamp.Substring(5, 2) & TimeStamp.Substring(8, 2)
            TimeStamp_Time = TimeStamp.Substring(11, 2) & TimeStamp.Substring(14, 2) & TimeStamp.Substring(17, 2)

            '------------------------------------------------
            ' ファイル名構築
            '------------------------------------------------
            Dim FileName As String = ""
            FileName = "RNT_KR_" & TimeStamp_Date & "_" & TimeStamp_Time & "_" & "1"

            '------------------------------------------------
            ' ファイル書込
            '------------------------------------------------
            Dim iRet As Integer = 0
            Do
                Try
                    Dim DirInfo As New DirectoryInfo(IniInfo.COMMON_RIENTADR)
                    Dim Dirs As FileSystemInfo() = DirInfo.GetDirectories()
                    iRet = 0
                    Exit Do
                Catch ex As Exception
                    If MessageBox.Show(String.Format(MSG0079I, "媒体", "ドライブ(" & Path.GetPathRoot(IniInfo.COMMON_RIENTADR) & ")"), msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "媒体挿入キャンセル")
                        iRet = -1
                        Exit Do
                    End If
                End Try
            Loop

            Select Case iRet
                Case 0          ' データ格納成功
                    If File.Exists(Path.Combine(IniInfo.COMMON_RIENTADR, IniInfo.KESSAI_RIENTANAME)) Then
                        If MessageBox.Show(MSG0067I, msgtitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> DialogResult.OK Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "中断", "既存ファイル上書キャンセル")
                            iRet = -1
                        End If
                    End If

                    If iRet = 0 Then
                        File.Copy(Path.Combine(IniInfo.COMMON_BAITAIWRITE, FileName), Path.Combine(IniInfo.COMMON_RIENTADR, IniInfo.KESSAI_RIENTANAME), True)
                    End If
            End Select

            '------------------------------------------------
            ' 画面初期設定
            '------------------------------------------------
            cmbTimeStamp.Items.Clear()
            btnSearch.Enabled = True
            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True
            btnReset.Enabled = False
            cmbTimeStamp.Enabled = False
            btnAction.Enabled = False
            txtSyoriDateY.Focus()

            '------------------------------------------------
            ' リエンタファイル検索処理
            '------------------------------------------------
            If GetFileInfo("Submit", txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text) = False Then
                MessageBox.Show(MSG0370W.Replace("{0}", "リエンタファイル検索"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            End If

            '------------------------------------------------
            ' 完了メッセージ表示
            '------------------------------------------------
            If iRet = 0 Then
                MessageBox.Show(String.Format(MSG0078I, "資金決済リエンタの媒体書込"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "成功", "出力ファイル:" & IniInfo.COMMON_BAITAIWRITE & FileName)
            Else
                MessageBox.Show(String.Format(MSG0080I, "資金決済リエンタの媒体書込", "中断"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
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
            cmbTimeStamp.Enabled = False

            txtSyoriDateD.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateY.Enabled = True

            'システム日付を再表示
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            cmbTimeStamp.Items.Clear()
            txtSyoriDateY.Focus()

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
                MessageBox.Show(String.Format(MSG0001E, "書込データ格納フォルダ", "COMMON", "BAITAIWRITE"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:書込データ格納フォルダ 分類:COMMON 項目:BAITAIWRITE")
                Return False
            End If

            IniInfo.COMMON_RIENTADR = CASTCommon.GetFSKJIni("COMMON", "RIENTADR")
            If IniInfo.COMMON_RIENTADR.ToUpper = "ERR" OrElse IniInfo.COMMON_RIENTADR = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "リエンタファイル格納先", "COMMON", "RIENTADR"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:リエンタファイル格納先 分類:COMMON 項目:RIENTADR")
                Return False
            End If

            IniInfo.KESSAI_RIENTANAME = CASTCommon.GetFSKJIni("KESSAI", "RIENTANAME")
            If IniInfo.KESSAI_RIENTANAME.ToUpper = "ERR" OrElse IniInfo.KESSAI_RIENTANAME = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "リエンタファイル名", "KESSAI", "RIENTANAME"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:リエンタファイル名 分類:KESSAI 項目:RIENTANAME")
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "入力チェック", "開始", "入力:" & txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text)

            '------------------------------------------------
            '処理年チェック
            '------------------------------------------------
            '必須チェック
            If txtSyoriDateY.Text.Length = 0 Then
                MessageBox.Show(MSG0018W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If

            '------------------------------------------------
            '処理月チェック
            '------------------------------------------------
            '必須チェック
            If txtSyoriDateM.Text.Length = 0 Then
                MessageBox.Show(MSG0020W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If

            '範囲チェック
            If Not (txtSyoriDateM.Text >= 1 And txtSyoriDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If

            '------------------------------------------------
            '処理日チェック
            '------------------------------------------------
            '必須チェック
            If txtSyoriDateD.Text.Length = 0 Then
                MessageBox.Show(MSG0023W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '桁数チェック
            If Not (txtSyoriDateD.Text >= 1 And txtSyoriDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
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
    ' リエンタ情報取得
    '================================
    Private Function GetFileInfo(ByVal Info As String, ByVal SyoriDate As String) As Boolean

        Dim TimeStamp As String

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "リエンタ情報取得", "開始", "処理日:" & SyoriDate)

            '------------------------------------------------
            ' コンボボックスの初期化
            '------------------------------------------------
            cmbTimeStamp.Items.Clear()

            '------------------------------------------------
            ' 再描画停止
            '------------------------------------------------
            cmbTimeStamp.BeginUpdate()

            '------------------------------------------------
            ' コンボボックス設定
            '------------------------------------------------
            Dim FileList() As String = Directory.GetFiles(IniInfo.COMMON_BAITAIWRITE)
            For i As Integer = 0 To FileList.Length - 1 Step 1
                Dim SetFileName() As String = Path.GetFileName(FileList(i)).Split("_"c)
                If SetFileName.Length = 5 Then
                    If SetFileName(0) = "RNT" And _
                       SetFileName(1) = "KR" And _
                       SetFileName(2) = SyoriDate And _
                       SetFileName(4) = "1" Then
                        TimeStamp = SetFileName(2) & SetFileName(3)
                        TimeStamp = TimeStamp.Substring(0, 4) & "/" & TimeStamp.Substring(4, 2) & "/" & TimeStamp.Substring(6, 2) & _
                                    Space(1) & _
                                    TimeStamp.Substring(8, 2) & ":" & TimeStamp.Substring(10, 2) & ":" & TimeStamp.Substring(12, 2)

                        cmbTimeStamp.Items.Add(TimeStamp)
                    End If
                End If
            Next

            '------------------------------------------------
            ' 再描画再開
            '------------------------------------------------
            cmbTimeStamp.EndUpdate()

            '------------------------------------------------
            ' 画面設定
            '------------------------------------------------
            If cmbTimeStamp.Items.Count = 0 Then
                '--------------------------------------------
                ' 検索時はメッセージ出力
                '--------------------------------------------
                If Info = "Search" Then
                    MessageBox.Show(MSG0255W.Replace("{0}", "リエンタ"), msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                btnAction.Enabled = False
                txtSyoriDateY.Focus()
            Else
                btnAction.Enabled = True
                btnSearch.Enabled = False
                txtSyoriDateD.Enabled = False
                txtSyoriDateM.Enabled = False
                txtSyoriDateY.Enabled = False
                cmbTimeStamp.Enabled = True
                btnReset.Enabled = True
                cmbTimeStamp.SelectedIndex = 0
                cmbTimeStamp.Focus()
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgtitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "リエンタ情報取得", "開始", ex.Message)

            cmbTimeStamp.EndUpdate()
            btnAction.Enabled = False
            txtSyoriDateY.Focus()

            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "リエンタ情報取得", "開始", "")
        End Try

    End Function

#End Region

#Region " 関数(Sub) "

    '================================
    ' ゼロ埋め
    '================================
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtSyoriDateY.Validating, _
            txtSyoriDateM.Validating, _
            txtSyoriDateD.Validating

        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

#End Region

End Class
' 2016/01/19 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END
