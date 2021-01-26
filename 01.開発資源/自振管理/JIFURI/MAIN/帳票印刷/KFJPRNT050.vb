Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJPRNT050
    Private MainLOG As New CASTCommon.BatchLOG("KFJPRNT050", "処理結果確認表再印刷画面")
    Private Const msgTitle As String = "処理結果確認表再印刷画面(KFJPRNT050)"
    Private Const errMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private PrtFolder As String
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Public strFURI_DATE As String

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private PrintName As String  '帳票名
    Private PrintID As String    '帳票ID
    Private CsvNames() As String '取得CSVファイル名

#Region " ロード"
    Private Sub KFJPRNT050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            Call GCom.SetMonitorTopArea(Label2, Label4, lblUser, lblDate)

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

            txtFuriDateY.Text = strSysDate.Substring(0, 4)
            txtFuriDateM.Text = strSysDate.Substring(4, 2)
            txtFuriDateD.Text = strSysDate.Substring(6, 2)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'コンボボックスを表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim Msg As String
            Select Case GCom.SetComboBox(cmbPrintKbn, "KFJPRNT050_対象帳票.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "対象帳票", "KFJPRNT050_対象帳票.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "対象帳票設定ファイルなし。ファイル:KFJPRNT050_対象帳票.TXT"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "対象帳票"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "コンボボックス設定失敗 コンボボックス名:対象帳票"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
            End Select

            'コントロール制御
            btnAction.Enabled = False
            btnCancel.Enabled = False
            cmbTimeStamp.Enabled = False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
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
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            Call Get_PrintKbn()

            '対象のCSVファイル検索(帳票ID_処理日(YYYYMMDD)タイムスタンプ(HHmmss)_プロセスID_連番.CSV)
            Dim FileList() As String = Directory.GetFiles(PrtFolder, PrintID & "_" & strFURI_DATE & "*.CSV")
            If FileList.Length = 0 Then
                MessageBox.Show(MSG0232W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
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
            cmbPrintKbn.Enabled = False
            cmbTimeStamp.SelectedIndex = 0
            txtFuriDateY.Enabled = False
            txtFuriDateM.Enabled = False
            txtFuriDateD.Enabled = False
            btnSearch.Enabled = False
            btnAction.Enabled = True
            btnCancel.Enabled = True
            cmbTimeStamp.Enabled = True
            cmbTimeStamp.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(検索)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 取消"
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
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
            cmbPrintKbn.Enabled = True
            txtFuriDateY.Enabled = True
            txtFuriDateM.Enabled = True
            txtFuriDateD.Enabled = True
            btnSearch.Enabled = True
            btnAction.Enabled = False
            btnCancel.Enabled = False
            cmbTimeStamp.Enabled = False
            txtFuriDateY.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取消)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 実行"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :実行ボタン
        'Return         :
        'Create         :2009/09/15
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, PrintName), _
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
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
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
        'Create         :2009/09/18
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
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        fn_INI_Read = False
        'DAT格納フォルダ
        PrtFolder = CASTCommon.GetFSKJIni("COMMON", "PRT")
        If PrtFolder.ToUpper = "ERR" OrElse PrtFolder = Nothing Then
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
            Select Case GCom.GetComboBox(cmbPrintKbn)
                Case 0
                    PrintID = "KFJP001"
                    PrintName = "処理結果確認表(落込)"
                Case 1
                    PrintID = "KFJP002"
                    PrintName = "処理結果確認表（センター直接持込）"
                Case 2
                    PrintID = "KFJP008"
                    PrintName = "処理結果確認表(配信データ作成)"
                Case 3
                    PrintID = "KFJP013"
                    PrintName = "処理結果確認表(不能結果更新)"
                Case 4
                    PrintID = "KFJP020"
                    PrintName = "処理結果確認表(返還データ作成)"
                Case 5
                    PrintID = "KFJP021"
                    PrintName = "処理結果確認表(再振データ作成)"
                Case 6
                    PrintID = "KFJP026"
                    PrintName = "処理結果確認表(落込取消)"
                Case 7
                    PrintID = "KFJP043"
                    PrintName = "処理結果確認表(自振契約)"
                Case 8
                    PrintID = "KFJP044"
                    PrintName = "処理結果確認表(自振契約結果)"
                Case Else
                    Return False
            End Select
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "印刷情報取得", "失敗", ex.ToString)
            Return False
        End Try
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ゼロパディング", "失敗", ex.ToString)
        End Try
    End Sub
#End Region

End Class