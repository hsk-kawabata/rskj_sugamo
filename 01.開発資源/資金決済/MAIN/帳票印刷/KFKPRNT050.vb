Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text

Public Class KFKPRNT050

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFKPRNT050", "処理結果確認表再印刷画面")
    Private Const msgTitle As String = "処理結果確認表再印刷画面(KFKPRNT050)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private PrintName As String  '帳票名
    Private PrintID As String    '帳票ID
    Private CsvNames() As String '取得CSVファイル名
    Private SyoriDate As String  '処理日
    Private PrtFolder As String
#Region " ロード"
    Private Sub KFKPRNT050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------
            'INIファイルの読み込み
            '------------------------------------
            If fn_INI_Read() = False Then
                Return
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時に処理日にシステム日付を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtSyoriDateY.Text = strSysDate.Substring(0, 4)
            txtSyoriDateM.Text = strSysDate.Substring(4, 2)
            txtSyoriDateD.Text = strSysDate.Substring(6, 2)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'コンボボックスを表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim Msg As String
            Select Case GCom.SetComboBox(cmbPrint, "KFKPRNT050_対象帳票.TXT", True)
                Case 1  'ファイルなし
                    Msg = "対象帳票の設定に失敗しました。" & vbCrLf & _
                          "テキストファイル:KFKPRNT050_対象帳票.TXTが存在しません。"
                    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "対象帳票設定ファイルなし。ファイル:KFKPRNT050_対象帳票.TXT"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
                Case 2  '異常
                    Msg = "対象帳票の設定に失敗しました。"
                    MessageBox.Show(Msg.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "コンボボックス設定失敗 コンボボックス名:対象帳票"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
            End Select
            'コントロール制御
            btnAction.Enabled = False
            btnReset.Enabled = False
            cmbTimeStamp.Enabled = False

            cmbPrint.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

#End Region
#Region " 検索"
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        '=====================================================================================
        'NAME           :btnSearch_Click
        'Parameter      :
        'Description    :検索ボタン
        'Return         :
        'Create         :2009/09/09
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '必須項目の入力値チェック
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '必須項目：振替日
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            SyoriDate = txtSyoriDateY.Text & txtSyoriDateM.Text & txtSyoriDateD.Text
            Call Get_PrintKbn()

            '対象のCSVファイル検索(帳票ID_処理日(YYYYMMDD)タイムスタンプ(HHmmss)_プロセスID_連番.CSV)
            Dim FileList() As String = Directory.GetFiles(PrtFolder, PrintID & "_" & SyoriDate & "*.CSV")
            If FileList.Length = 0 Then
                '対象印刷用のCSVなし
                MessageBox.Show(MSG0232W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return
            End If
            ReDim CsvNames(0)
            'タイムスタンプコンボボックスにアイテムを追加
            For No As Integer = 0 To FileList.Length - 1
                Dim FileInfo() As String = Split(Path.GetFileName(FileList(No)), "_"c)
                'YYYY/MM/DD HH:mm:dd形式に編集
                Dim Time As String = Mid(FileInfo(1), 1, 4) & "/" & Mid(FileInfo(1), 5, 2) & "/" & Mid(FileInfo(1), 7, 2) _
                                     & " " & Mid(FileInfo(1), 9, 2) & ":" & Mid(FileInfo(1), 11, 2) & ":" & Mid(FileInfo(1), 13, 2)
                cmbTimeStamp.Items.Add(Time)
                ReDim Preserve CsvNames(No)
                CsvNames(No) = FileList(No)
            Next

            'コントロール制御
            cmbPrint.Enabled = False
            cmbTimeStamp.SelectedIndex = 0
            txtSyoriDateY.Enabled = False
            txtSyoriDateM.Enabled = False
            txtSyoriDateD.Enabled = False
            btnSearch.Enabled = False
            btnAction.Enabled = True
            btnReset.Enabled = True
            cmbTimeStamp.Enabled = True
            cmbTimeStamp.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)終了", "成功", "")
        End Try
    End Sub

#End Region
#Region " 印刷"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :印刷ボタン
        'Return         :
        'Create         :2009/09/15
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            '印刷前確認メッセージ
            If MessageBox.Show(MSG0013I.Replace("{0}", PrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            Dim param As String
            Dim nRet As Integer

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：ログイン名、ＣＳＶファイル名

            param = GCom.GetUserID & "," & CsvNames(cmbTimeStamp.SelectedIndex)

            nRet = ExeRepo.ExecReport(PrintID & ".EXE", param)

            '戻り値に対応したメッセージを表示する
            ' 2017/05/26 タスク）綾部 CHG 【ME】(RSV2対応 メッセージ定数化) -------------------- START
            'Select Case nRet
            '    Case 0
            '        '印刷後確認メッセージ
            '        MessageBox.Show(PrintName & "の印刷を行いました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            '    Case -1
            '        '印刷対象なしメッセージ
            '        MessageBox.Show(PrintName & "の印刷対象が0件です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Case Else
            '        '印刷失敗メッセージ
            '        MessageBox.Show(PrintName & "の印刷に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            'End Select
            Select Case nRet
                Case 0
                    MessageBox.Show(MSG0014I.Replace("{0}", PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    MessageBox.Show(MSG0004E.Replace("{0}", PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select
            ' 2017/05/26 タスク）綾部 CHG 【ME】(RSV2対応 メッセージ定数化) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region
#Region " 取消"
    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        '=====================================================================================
        'NAME           :btnSearch_Click
        'Parameter      :
        'Description    :取消ボタン
        'Return         :
        'Create         :2009/09/18
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)開始", "成功", "")
            'コントロール制御
            cmbTimeStamp.Items.Clear()  'タイムスタンプ初期化
            cmbPrint.Enabled = True
            txtSyoriDateY.Enabled = True
            txtSyoriDateM.Enabled = True
            txtSyoriDateD.Enabled = True
            btnSearch.Enabled = True
            btnAction.Enabled = False
            btnReset.Enabled = False
            txtSyoriDateY.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外", "失敗", ex.Message)
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
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        Try
            fn_check_text = False
            '年必須チェック
            If txtSyoriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If
            '月必須チェック
            If txtSyoriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(txtSyoriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtSyoriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateM.Focus()
                Return False
            End If
            '日付必須チェック
            If txtSyoriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(txtSyoriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtSyoriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtSyoriDateY.Text & "/" & txtSyoriDateM.Text & "/" & txtSyoriDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSyoriDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.Message)
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
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        fn_INI_Read = False
        'PRT格納フォルダ
        PrtFolder = CASTCommon.GetFSKJIni("COMMON", "PRT")
        If PrtFolder.ToUpper = "ERR" Or PrtFolder = Nothing Then

            MessageBox.Show(String.Format(MSG0001E, "PRTCSV格納フォルダ", "COMMON", "PRT"), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        fn_INI_Read = True
    End Function
    Private Function Get_PrintKbn() As Boolean
        '============================================================================
        'NAME           :Get_PrintKbn
        'Parameter      :
        'Description    :帳票の情報を変数に格納する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        Try
            Select Case GCom.GetComboBox(cmbPrint)
                Case 0
                    PrintID = "KFKP001"
                    PrintName = "処理結果確認表(資金決済)"
                Case 1
                    PrintID = "KFKP003"
                    PrintName = "処理結果確認表(結果更新)"
                Case Else
                    Return False
            End Select
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷情報取得", "失敗", ex.Message)
            Return False
        End Try
    End Function
#End Region
#Region " イベント"
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
              Handles txtSyoriDateY.Validating, txtSyoriDateM.Validating, txtSyoriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ゼロパディング", "失敗", ex.Message)
        End Try
    End Sub
#End Region
End Class
