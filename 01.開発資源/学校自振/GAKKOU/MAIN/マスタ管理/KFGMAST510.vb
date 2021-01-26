Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Imports System.Data.OracleClient

Public Class KFGMAST510
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST510", "学校ツール連携（データ移出）画面")
    Private Const msgTitle As String = "学校ツール連携（データ移出）画面(KFGMAST510)"
    Private Const ThisFormName As String = "学校ツール連携（データ移出）"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private Structure ThisFormUseData
        Dim TORIS_CODE As String
        Dim FURI_DATE As String
    End Structure
    Private DT As ThisFormUseData

    'ソートオーダーフラグ
    Dim SortOrderFlag As Boolean = True

    'クリックした列の番号
    Dim ClickedColumn As Integer

    Private SourceDirectory As String = GCom.SET_PATH(System.IO.Path.GetTempPath) & "GData"
    Private DestinationDirectory As String
    Private DestinationDirectory_KEKKA As String
    Private strHENKAN_CSV_SEITO As String
    Private strHENKAN_CSV_GAKKO As String
    Private strHENKAN_CSV_HIMOKU As String
    Private strHENKAN_CSV_GAKKO2 As String
    Private strHENKAN_CSV_KEKKA As String
    Private strHENKAN_CSV_KITEN As String
    Private DRet As DialogResult

    Private Sub KFGMAST510_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            GCom.GetSysDate = Date.Now
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            Dim strDirectory As String

            strDirectory = CASTCommon.GetFSKJIni("GHENKAN", "SAVE_PATH")
            If InStrRev(strDirectory, "\") = strDirectory.Length Then
                DestinationDirectory = strDirectory.Substring(0, strDirectory.Length - 1)
            End If
            strDirectory = CASTCommon.GetFSKJIni("GHENKAN", "KEKKA_SAVE_PATH")
            If InStrRev(strDirectory, "\") = strDirectory.Length Then
                DestinationDirectory_KEKKA = strDirectory.Substring(0, strDirectory.Length - 1)
            End If


            '保存ファイル名iniファイル取得名 + 学校コード.csv
            strHENKAN_CSV_SEITO = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_SEITO")
            strHENKAN_CSV_GAKKO = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_GAKKO")
            strHENKAN_CSV_HIMOKU = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_HIMOKU")
            strHENKAN_CSV_GAKKO2 = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_GAKKO2")
            strHENKAN_CSV_KEKKA = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_KEKKA")
            strHENKAN_CSV_KITEN = CASTCommon.GetFSKJIni("GHENKAN", "HENKAN_CSV_KITEN")

            Me.FURI_DATE.Value = Date.Now
            Me.FURI_DATE.CustomFormat = " yyyy 年 MM 月 dd 日 dddd"

            'カレンダー初期設定
            Dim Ret As Boolean = GCom.CheckDateModule(Nothing, 1)

            Me.CK_L.Text = ""

            'スプレッド領域基本設定
            With Me.ListView1
                .Clear()
                .Columns.Add("", 22, HorizontalAlignment.Center)
                .Columns.Add("学校コード", 100, HorizontalAlignment.Center)
                .Columns.Add("学校名", 250, HorizontalAlignment.Left)
                .Columns.Add("学校カナ名", 0, HorizontalAlignment.Left)
                .Columns.Add("振替日", 100, HorizontalAlignment.Center)
                .Columns.Add("再振日", 100, HorizontalAlignment.Center)
                .Columns.Add("対象年月", 0, HorizontalAlignment.Center)
            End With

            Me.CheckBox1.Checked = True

            If Me.CheckBox1.Checked = False Then
                Label13.Enabled = False
                txtSYear.Enabled = False
                Label12.Enabled = False
                txtSMonth.Enabled = False
                Label11.Enabled = False

                FURI_DATE_L.Enabled = True
                FURI_DATE.Enabled = True
            Else
                Label13.Enabled = True
                txtSYear.Enabled = True
                Label12.Enabled = True
                txtSMonth.Enabled = True
                Label11.Enabled = True

                FURI_DATE_L.Enabled = False
                FURI_DATE.Enabled = False

                txtSYear.Text = Format(Now, "yyyy")
                txtSMonth.Text = Format(Now, "MM")

            End If

            Me.CmdSelect.Enabled = True

            Application.DoEvents()
            Me.FURI_DATE.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    '請求年月対象チェックボックス変更
    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        Me.ListView1.Items.Clear()

        If Me.CheckBox1.Checked = False Then
            Label13.Enabled = False
            txtSYear.Enabled = False
            Label12.Enabled = False
            txtSMonth.Enabled = False
            Label11.Enabled = False

            FURI_DATE_L.Enabled = True
            FURI_DATE.Enabled = True
        Else
            Label13.Enabled = True
            txtSYear.Enabled = True
            Label12.Enabled = True
            txtSMonth.Enabled = True
            Label11.Enabled = True

            FURI_DATE_L.Enabled = False
            FURI_DATE.Enabled = False

            txtSYear.Text = Format(Now, "yyyy")
            txtSMonth.Text = Format(Now, "MM")
        End If

        Me.FURI_DATE.Focus()
    End Sub

    '日付チェック
    Private Sub FURI_DATE_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FURI_DATE.ValueChanged
        Call CK_EigyouDay()
    End Sub

    '一覧表示領域のソート
    Private Sub LetSort_ListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
    Handles ListView1.ColumnClick

        With CType(sender, ListView)

            If ClickedColumn = e.Column Then
                ' 同じ列をクリックした場合は，逆順にする 
                SortOrderFlag = Not SortOrderFlag
            End If

            ' 列番号設定
            ClickedColumn = e.Column

            ' 列水平方向配置
            Dim ColAlignment As HorizontalAlignment = .Columns(e.Column).TextAlign

            ' ソート
            .ListViewItemSorter = New CASTCommon.ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)

            ' ソート実行
            .Sort()

        End With
    End Sub

    '
    ' 機能　 ： 営業日判定関数
    '
    ' 引数　 ： なし
    '
    ' 戻り値 ： 営業日 = False, 非営業日 = True
    '
    ' 備考　 ： なし
    '
    Private Function CK_EigyouDay() As Boolean
        Try
            '------------------------------------------------
            '全休日情報を蓄積
            '------------------------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If

            Dim ChangeDate As String = ""
            Dim CurrDate As String = String.Format("{0:yyyyMMdd}", Me.FURI_DATE.Value)
            Dim FLG As Boolean = GCom.CheckDateModule(CurrDate, ChangeDate, 0)

            Me.CK_L.Visible = (Not FLG)
            Me.CmdSelect.Enabled = FLG


        Catch ex As Exception

        End Try
    End Function

    Private Sub CmdSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdSelect.Click
        Call Get_GakkouMonitor()
    End Sub

    '一覧表示領域行データ表示
    Private Sub ListView1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.DoubleClick
        Call GCom.MonitorCsvFile(CType(sender, ListView))
    End Sub

    ' 機能　 ： 対象学校一覧を設定する
    '
    ' 引数　 ： なし
    '
    ' 戻り値 ： なし
    '
    ' 備考　 ： なし
    '
    Private Sub Get_GakkouMonitor()

        Me.SuspendLayout()
        Dim SQL As String
        Dim MSG As String
        Dim BRet As Boolean
        Dim REC As OracleDataReader = Nothing
        Dim RC2 As OracleDataReader = Nothing
        Dim onText(2) As Integer
        Dim onDate As Date

        Me.CK_L.Text = ""

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            SQL = "SELECT GAKKOU_CODE_S"
            SQL &= " , GAKKOU_NNAME_G"
            SQL &= " , GAKKOU_KNAME_G"
            SQL &= " , NENGETUDO_S"
            SQL &= " , FURI_DATE_S"
            SQL &= " , SFURI_DATE_S"
            SQL &= " FROM (SELECT DISTINCT GAKKOU_CODE_G,GAKKOU_NNAME_G, GAKKOU_KNAME_G FROM GAKMAST1),G_SCHMAST"
            SQL &= " WHERE GAKKOU_CODE_G = GAKKOU_CODE_S"
            Select Case Me.CheckBox1.Checked
                Case True
                    SQL &= " AND NENGETUDO_S = '" & txtSYear.Text + txtSMonth.Text & "'"
                Case Else
                    SQL &= " AND FURI_DATE_S = '" & String.Format("{0:yyyyMMdd}", Me.FURI_DATE.Value) & "'"
            End Select
            SQL &= " AND SCH_KBN_S IN ('0','1')"
            SQL &= " AND FURI_KBN_S = '0'"
            'SQL &= " AND DATA_FLG_S = '0'"
            SQL &= " ORDER BY FURI_DATE_S,GAKKOU_CODE_S"

            Me.ListView1.Items.Clear()

            BRet = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read
                    If Not RC2 Is Nothing Then
                        RC2.Close()
                        RC2.Dispose()
                    End If

                    Dim Data(6) As String

                    '学校コード
                    Data(1) = GCom.NzDec(REC.Item("GAKKOU_CODE_S"), "")

                    '学校名
                    Data(2) = GCom.NzStr(REC.Item("GAKKOU_NNAME_G")).Trim

                    '学校カナ名
                    Data(3) = GCom.NzStr(REC.Item("GAKKOU_KNAME_G")).Trim

                    '振替日
                    Data(4) = GCom.NzDec(REC.Item("FURI_DATE_S"), "")
                    If Data(4).Length >= 8 Then
                        onText(0) = GCom.NzInt(Data(4).Substring(0, 4))
                        onText(1) = GCom.NzInt(Data(4).Substring(4, 2))
                        onText(2) = GCom.NzInt(Data(4).Substring(6, 2))
                        Dim Ret As Integer = GCom.SET_DATE(onDate, onText)
                        If Ret = -1 Then
                            Data(4) = String.Format("{0:yyyy/MM/dd}", onDate)
                        End If
                    End If

                    '再振替日
                    Data(5) = GCom.NzDec(REC.Item("SFURI_DATE_S"), "")
                    If Data(5).Length >= 8 Then
                        onText(0) = GCom.NzInt(Data(5).Substring(0, 4))
                        onText(1) = GCom.NzInt(Data(5).Substring(4, 2))
                        onText(2) = GCom.NzInt(Data(5).Substring(6, 2))
                        Dim Ret As Integer = GCom.SET_DATE(onDate, onText)
                        If Ret = -1 Then
                            Data(5) = String.Format("{0:yyyy/MM/dd}", onDate)
                        End If
                    End If
                    If Data(5) = "00000000" Then
                        Data(5) = ""
                    End If

                    '対象年月
                    Data(6) = GCom.NzStr(REC.Item("NENGETUDO_S")).Trim

                    Dim vLstItem As New ListViewItem(Data, -1, Color.Black, Color.White, Nothing)
                    ListView1.Items.AddRange(New ListViewItem() {vLstItem})

                Loop

                Me.ListView1.Focus()
            Else
                MSG = "対象スケジュールがありません。" & Space(5)
                DRet = MessageBox.Show(MSG, _
                        "学校ツール連携データ移出処理", MessageBoxButtons.OK, MessageBoxIcon.Warning)

                LW.FuriDate = String.Format("{0:yyyyMMdd}", Me.FURI_DATE.Value)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "対象スケジュールがありません", "失敗", "")

            End If

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not RC2 Is Nothing Then
                RC2.Close()
                RC2.Dispose()
            End If
        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not RC2 Is Nothing Then
                RC2.Close()
                RC2.Dispose()
            End If
            Me.ResumeLayout()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
        End Try
    End Sub

    Private Sub CmdCreate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdCreate.Click
        Dim MSG As String = ""
        Dim Temp As String
        Dim BRet As Boolean
        Dim LBRet As Boolean

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出)開始", "成功", "")

            Dim BreakFast As ListView.CheckedListViewItemCollection = Me.ListView1.CheckedItems

            'リストに１件も表示されていないとき
            If ListView1.Items.Count <= 0 Then
                MessageBox.Show(MSG0052W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Else
                'リストに１件以上表示されているが、チェックされていないとき
                If BreakFast.Count <= 0 Then
                    MessageBox.Show(MSG0053W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If

            For Row As Integer = 0 To BreakFast.Count - 1 Step 1

                Me.SuspendLayout()
                Dim lsvItem As ListViewItem.ListViewSubItemCollection = BreakFast.Item(Row).SubItems
                Dim Data(6) As String

                For Idx As Integer = 0 To 6 Step 1
                    Select Case Idx
                        Case 1, 2, 3, 4, 5, 6
                            Data(Idx) = lsvItem.Item(Idx).Text
                    End Select
                Next Idx

                '連携の学校(先ず作成先のフォルダを空にする)
                If System.IO.Directory.Exists(SourceDirectory) Then
                    Dim DFL() As String = System.IO.Directory.GetFiles(SourceDirectory)
                    For Each Temp In DFL
                        System.IO.File.Delete(Temp)
                    Next Temp
                Else
                    System.IO.Directory.CreateDirectory(SourceDirectory)
                End If
                Application.DoEvents()


                '学校情報1CSV (GAKKO+学校コード.CSV)
                BRet = Set_GAKKO(Data)

                '学校情報2CSV (GAKKO2+学校コード.CSV)
                If BRet Then
                    BRet = Set_GAKKO2(Data(1))
                End If

                '費目情報CSV (HIMOKU+学校コード.CSV) 
                If BRet Then
                    BRet = Set_HIMOKU(Data(1), Data(6))
                End If

                '契約者情報登録CSV (SEITO+学校コード.CSV)
                If BRet Then
                    BRet = Set_SEITO(Data(1), Data(6))
                End If


                '金融機関情報CSV (BANK+学校コード.CSV) 
                If BRet Then
                    BRet = Set_BANK_INF(Data(1))
                End If

                LBRet = False

                Dim onDate As Date

                MSG = "(" & Data(1) & ") " & Data(2)
                MSG &= New String(ControlChars.Cr, 2)
                MSG &= "データ移出処理を実行してもよろしいですか？" & Space(5)
                DRet = MessageBox.Show(MSG, "学校ツール連携移出処理", MessageBoxButtons.OKCancel, _
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                BRet = (DRet = DialogResult.OK)

                '移出キャンセル場合
                If BRet = False Then
                    Exit Sub
                End If


                '複写関数CALL
                LBRet = CopyInterfaceFile(0, Data(1), Data(4), Data(5), onDate)

                MSG = "(" & Data(1) & ") " & Data(2)
                MSG &= New String(ControlChars.Cr, 2)
                If LBRet = True Then
                    MSG &= "データ移出処理が成功しました。" & Space(5)
                    DRet = MessageBox.Show(MSG, "学校ツール連携移出処理", MessageBoxButtons.OK, _
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
                Else
                    MSG &= "データ移出処理が失敗しました。" & Space(5)
                    DRet = MessageBox.Show(MSG, "学校ツール連携移出処理", MessageBoxButtons.OK, _
                        MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                End If
                BRet = (DRet = DialogResult.OK)

                Me.ResumeLayout()
                Application.DoEvents()

            Next Row

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            Application.DoEvents()
            Me.FURI_DATE.Focus()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出)終了", "成功", "")
        End Try
    End Sub

    Private Sub CmdFinal_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdFinal.Click
        Me.Close()
        Me.Dispose()
    End Sub

    '
    ' 機能　 ： 作成データを指定フォルダに複写する
    '
    ' 引数　 ： ARG1 - 書込回数（使用していない）
    ' 　　　 　 ARG2 - 学校コード
    ' 　　　 　 ARG3 - 振替日
    ' 　　　 　 ARG4 - 再振替日
    ' 　　　 　 ARG5 - 識別日付
    '
    ' 戻り値 ： OK=True, NG=False
    '
    ' 備考　 ： 学校スケジュール更新機能含む
    '
    Private Function CopyInterfaceFile(ByRef STATUS As Integer, ByVal TORIS_CODE As String, _
        ByVal FURI_DATE As String, ByVal SFURI_DATE As String, ByVal ONDATE As Date) As Boolean
        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE
            LW.FuriDate = FURI_DATE
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-複写)開始", "成功", "")

            Dim FD() As String

            FD = System.IO.Directory.GetDirectories(DestinationDirectory) '.Substring(0, 3))

            If Not FD Is Nothing AndAlso FD.Length > 0 Then
                If System.IO.Directory.Exists(DestinationDirectory) Then
                Else
                    '指定ディレクトリがない為作成する
                    System.IO.Directory.CreateDirectory(DestinationDirectory)
                End If
            Else
                '指定ディレクトリがない為作成する
                System.IO.Directory.CreateDirectory(DestinationDirectory)
            End If

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-複写)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Return False
        End Try

        Try

            '移出処理
            Dim SFL() As String = System.IO.Directory.GetFiles(SourceDirectory)

            For Each SourcePath As String In SFL

                Dim DestinationPath As String


                DestinationPath = GCom.SET_PATH(DestinationDirectory)

                DestinationPath &= System.IO.Path.GetFileName(SourcePath)

                System.IO.File.Copy(SourcePath, DestinationPath, True)

                ONDATE = System.DateTime.Now
                Call System.IO.File.SetLastWriteTime(SourcePath, ONDATE)
            Next SourcePath

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-複写)", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Return False
        End Try

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-複写)終了", "成功", "")
        Return True

    End Function

    '
    ' 機能　 ： 学校情報CSV (GAKKO.CSV) 作成
    '
    ' 引数　 ： Data()
    '
    ' 戻り値 ： OK=True, NG=False
    '
    ' 備考　 ： なし
    '
    Private Function Set_GAKKO(ByVal DATA() As String) As Boolean
        Dim REC As OracleDataReader = Nothing
        Dim FS As FileStream = Nothing
        Dim FW As StreamWriter = Nothing
        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = DATA(1)
            LW.FuriDate = DATA(4).Substring(0, 4) & DATA(4).Substring(5, 2) & DATA(4).Substring(8, 2)
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(学校情報CSV))開始", "成功", "")

            Dim FileName As String = GCom.SET_PATH(SourceDirectory) & strHENKAN_CSV_GAKKO & DATA(1) & ".CSV"

            FS = New FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None)
            FW = New StreamWriter(FS, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            Dim SQL As String = "SELECT SIYOU_GAKUNEN_T,SAIKOU_GAKUNEN_T,TKIN_NO_T,TSIT_NO_T,KIN_NNAME_N,SIT_NNAME_N"
            SQL &= " FROM GAKMAST2,TENMAST"
            SQL &= " WHERE GAKKOU_CODE_T = '" & DATA(1) & "'"
            SQL &= " AND TKIN_NO_T = KIN_NO_N AND TSIT_NO_T = SIT_NO_N"

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    Dim LineData As String = DATA(1) & ","
                    LineData &= DATA(2) & ","
                    LineData &= DATA(3) & ","
                    LineData &= GCom.NzDec(REC.Item("SIYOU_GAKUNEN_T"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("SAIKOU_GAKUNEN_T"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("TKIN_NO_T")) & ","
                    LineData &= GCom.NzStr(REC.Item("KIN_NNAME_N")) & ","
                    LineData &= GCom.NzStr(REC.Item("TSIT_NO_T")) & ","
                    LineData &= GCom.NzStr(REC.Item("SIT_NNAME_N")) & ","
                    LineData &= "00000000" & ","
                    LineData &= "00000000" & ","
                    LineData &= DATA(4).Substring(0, 4) & DATA(4).Substring(5, 2) & DATA(4).Substring(8, 2) & ","
                    If DATA(5) = "" Then
                        LineData &= "00000000" & ","
                    Else
                        LineData &= DATA(5).Substring(0, 4) & DATA(5).Substring(5, 2) & DATA(5).Substring(8, 2) & ","
                    End If
                    LineData &= "00000000" & ","
                    LineData &= "0" & ","
                    LineData &= "1"

                    FW.WriteLine(LineData)
                Loop
            End If

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            Return True

        Catch ex As Exception
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(学校情報CSV))", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Return False

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not FW Is Nothing Then
                FW.Close()
            End If
            If Not FS Is Nothing Then
                FS.Close()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(学校情報CSV))終了", "成功", "")
        End Try

    End Function

    '
    ' 機能　 ： 学校学年クラス情報CSV (GAKKO2.CSV) 作成
    '
    ' 引数　 ： ARG1 - 学校コード
    '
    ' 戻り値 ： OK=True, NG=False
    '
    ' 備考　 ： なし
    '
    Private Function Set_GAKKO2(ByVal TORIS_CODE As String) As Boolean
        Dim REC As OracleDataReader = Nothing
        Dim FS As FileStream = Nothing
        Dim FW As StreamWriter = Nothing
        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(学校学年クラス情報CSV))開始", "成功", "")

            Dim FileName As String = GCom.SET_PATH(SourceDirectory) & strHENKAN_CSV_GAKKO2 & TORIS_CODE & ".CSV"

            FS = New FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None)
            FW = New StreamWriter(FS, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            Dim SQL As String = "SELECT * FROM GAKMAST1"
            SQL &= " WHERE GAKKOU_CODE_G = '" & TORIS_CODE & "'"
            SQL &= " ORDER BY GAKUNEN_CODE_G ASC"

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    Dim LineData As String = GCom.NzDec(REC.Item("GAKKOU_CODE_G"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("GAKUNEN_CODE_G"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("GAKUNEN_NAME_G")).Trim.PadLeft(1, " "c) & ","
                    For Cnt As Integer = 1 To 19 Step 1
                        Dim Temp As String = (100 + Cnt).ToString
                        LineData &= GCom.NzDec(REC.Item("CLASS_CODE" & Temp & "_G"), "") & ","
                        LineData &= GCom.NzStr(REC.Item("CLASS_NAME" & Temp & "_G")).Trim.PadLeft(1, " "c) & ","
                    Next Cnt
                    LineData &= GCom.NzDec(REC.Item("CLASS_CODE120_G"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("CLASS_NAME120_G")).Trim.PadLeft(1, " "c)

                    FW.WriteLine(LineData)
                Loop
            End If

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            Return True
        Catch ex As Exception
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(学校学年クラス情報CSV))", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not FW Is Nothing Then
                FW.Close()
            End If
            If Not FS Is Nothing Then
                FS.Close()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(学校学年クラス情報CSV))終了", "成功", "")
        End Try

    End Function

    '
    ' 機能　 ： 費目情報CSV (HIMOKU.CSV) 作成
    '
    ' 引数　 ： ARG1 - 学校コード
    ' 　　　 　 ARG2 - 振替日
    '
    ' 戻り値 ： OK=True, NG=False
    '
    ' 備考　 ： なし
    '
    Private Function Set_HIMOKU(ByVal TORIS_CODE As String, ByVal FURI_MMDD As String) As Boolean
        Dim Temp As String
        Dim REC As OracleDataReader = Nothing
        Dim FS As FileStream = Nothing
        Dim FW As StreamWriter = Nothing
        Dim SQL As String
        Dim BRet As Boolean
        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(費目CSV作成))開始", "成功", "")

            Dim FileName As String = GCom.SET_PATH(SourceDirectory) & strHENKAN_CSV_HIMOKU & TORIS_CODE & ".CSV"

            FS = New FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None)
            FW = New StreamWriter(FS, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            SQL = "SELECT ' ' KBN"
            SQL &= ", GAKKOU_CODE_H"
            SQL &= ", GAKUNEN_CODE_H"
            SQL &= ", HIMOKU_ID_H"
            SQL &= ", HIMOKU_ID_NAME_H"
            SQL &= ", TUKI_NO_H"
            For Idx As Integer = 1 To 10 Step 1
                Temp = String.Format("{0:00}", Idx)
                SQL &= ", HIMOKU_NAME" & Temp & "_H"
                SQL &= ", KESSAI_KIN_CODE" & Temp & "_H"
                SQL &= ", KESSAI_TENPO" & Temp & "_H"
                SQL &= ", KESSAI_KAMOKU" & Temp & "_H"
                SQL &= ", KESSAI_KOUZA" & Temp & "_H"
                SQL &= ", KESSAI_MEIGI" & Temp & "_H"
                SQL &= ", HIMOKU_KINGAKU" & Temp & "_H"
            Next Idx
            SQL &= " FROM HIMOMAST"
            SQL &= " WHERE GAKKOU_CODE_H = '" & TORIS_CODE & "'"
            SQL &= " ORDER BY"
            SQL &= "      GAKUNEN_CODE_H ASC"
            SQL &= "    , HIMOKU_ID_H ASC"
            SQL &= "    , TUKI_NO_H ASC"

            BRet = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    Dim LineData As String = GCom.NzStr(REC.Item("KBN")) & ","
                    LineData &= GCom.NzDec(REC.Item("GAKKOU_CODE_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("GAKUNEN_CODE_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("HIMOKU_ID_H"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("HIMOKU_ID_NAME_H")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("TUKI_NO_H"), "") & ","
                    For Cnt As Integer = 1 To 9 Step 1
                        Temp = Cnt.ToString.PadLeft(2, "0"c)
                        LineData &= GCom.NzStr(REC.Item("HIMOKU_NAME" & Temp & "_H")).Trim.PadLeft(1, " "c) & ","
                        LineData &= GCom.NzDec(REC.Item("KESSAI_KIN_CODE" & Temp & "_H"), "") & ","
                        LineData &= GCom.NzDec(REC.Item("KESSAI_TENPO" & Temp & "_H"), "") & ","
                        LineData &= GCom.NzDec(REC.Item("KESSAI_KAMOKU" & Temp & "_H"), "") & ","
                        LineData &= GCom.NzDec(REC.Item("KESSAI_KOUZA" & Temp & "_H"), "") & ","
                        LineData &= GCom.NzDec(REC.Item("KESSAI_MEIGI" & Temp & "_H"), "") & ","
                        LineData &= GCom.NzDec(REC.Item("HIMOKU_KINGAKU" & Temp & "_H"), "") & ","
                    Next Cnt
                    LineData &= GCom.NzStr(REC.Item("HIMOKU_NAME10_H")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("KESSAI_KIN_CODE10_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("KESSAI_TENPO10_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("KESSAI_KAMOKU10_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("KESSAI_KOUZA10_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("KESSAI_MEIGI10_H"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("HIMOKU_KINGAKU10_H"), "")

                    FW.WriteLine(LineData)
                Loop
            End If

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            Return True
        Catch ex As Exception
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(費目CSV作成))", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not FW Is Nothing Then
                FW.Close()
            End If
            If Not FS Is Nothing Then
                FS.Close()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(費目CSV作成))終了", "成功", "")

        End Try

    End Function

    '
    ' 機能　 ： 契約者情報登録CSV (SEITO+学校コード.CSV) 作成
    '
    ' 引数　 ： ARG1 - 学校コード
    ' 　　　 　 ARG2 - 振替日
    '
    ' 戻り値 ： OK=True, NG=False
    '
    ' 備考　 ： なし
    '
    Private Function Set_SEITO(ByVal TORIS_CODE As String, ByVal FURI_MMDD As String) As Boolean
        Dim Temp As String
        Dim Tuki As String
        Dim REC As OracleDataReader = Nothing
        Dim RECA As OracleDataReader = Nothing
        Dim FS As FileStream = Nothing
        Dim FW As StreamWriter = Nothing
        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(契約者情報登録CSV作成))開始", "成功", "")

            Dim FileName As String = GCom.SET_PATH(SourceDirectory) & strHENKAN_CSV_SEITO & TORIS_CODE & ".CSV"

            FS = New FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None)
            FW = New StreamWriter(FS, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            If FURI_MMDD.Length >= 6 Then
                Tuki = FURI_MMDD.Substring(4, 2)
            Else
                Tuki = "00"
            End If

            Dim SQL As String = "SELECT ' ' KBN"
            SQL &= ", GAKKOU_CODE_O"
            SQL &= ", NENDO_O"
            SQL &= ", TUUBAN_O"
            SQL &= ", GAKUNEN_CODE_O"
            SQL &= ", CLASS_CODE_O"
            SQL &= ", SEITO_NO_O"
            SQL &= ", SEITO_KNAME_O"
            SQL &= ", SEITO_NNAME_O"
            SQL &= ", SEIBETU_O"
            SQL &= ", HIMOKU_ID_O"
            For Idx As Integer = 1 To 10 Step 1
                Temp = String.Format("{0:00}", Idx)
                SQL &= ", SEIKYU" & Temp & "_O"
                SQL &= ", KINGAKU" & Temp & "_O"
            Next Idx
            SQL &= ", 0 TOTAL"
            SQL &= ", TKIN_NO_O"
            SQL &= ", KIN_NNAME_N"
            SQL &= ", TSIT_NO_O"
            SQL &= ", SIT_NNAME_N"
            SQL &= ", KAMOKU_O"
            SQL &= ", KOUZA_O"
            SQL &= ", MEIGI_KNAME_O"
            SQL &= ", MEIGI_NNAME_O"
            SQL &= ", FURIKAE_O"
            SQL &= ", KEIYAKU_NJYU_O"
            SQL &= ", KEIYAKU_DENWA_O"
            SQL &= ", KAIYAKU_FLG_O"
            SQL &= " FROM SEITOMASTVIEW"
            SQL &= ", (SELECT KIN_NO_N"                 'BANK-Code
            SQL &= ", SIT_NO_N"                         'BRANCH-Code
            SQL &= ", KIN_NNAME_N"                      'BANK-Name
            SQL &= ", SIT_NNAME_N"                      'BRANCH-Name
            SQL &= " FROM TENMAST"
            SQL &= ")"
            SQL &= " WHERE GAKKOU_CODE_O = '" & TORIS_CODE & "'"


            SQL &= " AND TUKI_NO_O = '" & Tuki & "'"

            SQL &= " AND TKIN_NO_O = KIN_NO_N (+)"
            SQL &= " AND TSIT_NO_O = SIT_NO_N (+)"
            SQL &= " ORDER BY GAKUNEN_CODE_O ASC"
            SQL &= ", CLASS_CODE_O ASC"
            SQL &= ", SEITO_NO_O ASC"

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    Dim LineData As String = GCom.NzStr(REC.Item("KBN")) & ","
                    LineData &= GCom.NzDec(REC.Item("GAKKOU_CODE_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("NENDO_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("TUUBAN_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("GAKUNEN_CODE_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("CLASS_CODE_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("SEITO_NO_O"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("SEITO_KNAME_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzStr(REC.Item("SEITO_NNAME_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("SEIBETU_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("HIMOKU_ID_O"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("TOTAL"), "") & ","
                    LineData &= GCom.NzDec(REC.Item("TKIN_NO_O"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("KIN_NNAME_N")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("TSIT_NO_O"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("SIT_NNAME_N")).Trim.PadLeft(1, " "c) & ","

                    '1:当座、2:普通
                    If String.Format("{0:0}", GCom.NzInt(GCom.NzDec(REC.Item("KAMOKU_O"))), "") = "1" Then
                        LineData &= "1" & ","
                    Else
                        LineData &= "2" & ","
                    End If

                    LineData &= GCom.NzDec(REC.Item("KOUZA_O"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("MEIGI_KNAME_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzStr(REC.Item("MEIGI_NNAME_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("FURIKAE_O"), "") & ","
                    LineData &= GCom.NzStr(REC.Item("KEIYAKU_NJYU_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzStr(REC.Item("KEIYAKU_DENWA_O")).Trim.PadLeft(1, " "c) & ","
                    LineData &= GCom.NzDec(REC.Item("KAIYAKU_FLG_O"), "") & ","

                    For Cnt As Integer = 1 To 10 Step 1
                        Temp = Cnt.ToString.PadLeft(2, "0"c)

                        LineData &= GCom.NzDec(REC.Item("SEIKYU" & Temp & "_O"), "") & ","
                        If Cnt = 10 Then
                            LineData &= GCom.NzDec(REC.Item("KINGAKU" & Temp & "_O"), "")
                        Else
                            LineData &= GCom.NzDec(REC.Item("KINGAKU" & Temp & "_O"), "") & ","
                        End If
                        If Not RECA Is Nothing Then
                            RECA.Close()
                            RECA.Dispose()
                        End If
                    Next Cnt

                    FW.WriteLine(LineData)
                Loop
            End If

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If

            Return True
        Catch ex As Exception

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(契約者情報登録CSV作成))", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not RECA Is Nothing Then
                RECA.Close()
                RECA.Dispose()
            End If
            If Not FW Is Nothing Then
                FW.Close()
            End If
            If Not FS Is Nothing Then
                FS.Close()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(契約者情報登録CSV作成))終了", "成功", "")

        End Try

    End Function

    '
    ' 機能　 ： 金融機関情報CSV (BANK.CSV) 作成
    '
    ' 引数　 ： ARG1 - 学校コード
    '
    ' 戻り値 ： OK=True, NG=False
    '
    ' 備考　 ： なし
    '
    Private Function Set_BANK_INF(ByVal TORIS_CODE As String) As Boolean
        Dim REC As OracleDataReader = Nothing
        Dim FS As FileStream = Nothing
        Dim FW As StreamWriter = Nothing
        Dim SELF_BANK_CODE As String                            'INIファイルからの自金庫コード
        'Dim SELF_BANK_NAME As String                            'INIファイルからの自金庫コード
        Dim LineData As String

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(金融機関CSV作成))開始", "成功", "")

            Dim FileName As String = GCom.SET_PATH(SourceDirectory) & strHENKAN_CSV_KITEN & TORIS_CODE & ".CSV"

            FS = New FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None)
            FW = New StreamWriter(FS, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            Dim SQL As String = "SELECT TKIN_NO_V"
            SQL &= ", KIN_KNAME_N"
            SQL &= ", KIN_NNAME_N"
            SQL &= ", SIT_NO_N"
            SQL &= ", SIT_KNAME_N"
            SQL &= ", SIT_NNAME_N"
            SQL &= " FROM G_TAKOUMAST"
            SQL &= ", ("
            SQL &= "SELECT KIN_NO_N"
            SQL &= ", KIN_KNAME_N"
            SQL &= ", KIN_NNAME_N"
            SQL &= ", SIT_NO_N"
            SQL &= ", SIT_KNAME_N"
            SQL &= ", SIT_NNAME_N"
            SQL &= " FROM TENMAST"
            SQL &= ")"
            SQL &= " WHERE TKIN_NO_V = KIN_NO_N"
            SQL &= " AND GAKKOU_CODE_V = '" & TORIS_CODE & "'"
            SQL &= " ORDER BY"
            SQL &= "  TKIN_NO_V , SIT_NO_N"

            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    LineData = GCom.NzDec(REC.Item("TKIN_NO_V"), "")
                    LineData &= "," & GCom.NzStr(REC.Item("KIN_NNAME_N"))
                    LineData &= "," & GCom.NzDec(REC.Item("SIT_NO_N"), "")
                    LineData &= "," & GCom.NzStr(REC.Item("SIT_NNAME_N"))

                    FW.WriteLine(LineData)
                Loop
            End If

            '自金庫コード取得
            SELF_BANK_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")

            SQL = "SELECT KIN_NO_N"
            SQL &= ", KIN_KNAME_N"
            SQL &= ", KIN_NNAME_N"
            SQL &= ", SIT_NO_N"
            SQL &= ", SIT_KNAME_N"
            SQL &= ", SIT_NNAME_N"
            SQL &= " FROM TENMAST"
            SQL &= " WHERE KIN_NO_N = '" & SELF_BANK_CODE & "'"
            SQL &= " ORDER BY"
            SQL &= "  KIN_NO_N , SIT_NO_N"

            BRet = GCom.SetDynaset(SQL, REC)
            If BRet Then
                Do While REC.Read

                    LineData = GCom.NzDec(REC.Item("KIN_NO_N"), "")
                    LineData &= "," & GCom.NzStr(REC.Item("KIN_NNAME_N"))
                    LineData &= "," & GCom.NzDec(REC.Item("SIT_NO_N"), "")
                    LineData &= "," & GCom.NzStr(REC.Item("SIT_NNAME_N"))

                    FW.WriteLine(LineData)
                Loop
            End If

            'LineData = SELF_BANK_CODE
            'LineData &= "," & SELF_BANK_NAME

            'FW.WriteLine(LineData)

            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            Return True

        Catch ex As Exception
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(金融機関CSV作成))", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False

        Finally
            If Not REC Is Nothing Then
                REC.Close()
                REC.Dispose()
            End If
            If Not FW Is Nothing Then
                FW.Close()
            End If
            If Not FS Is Nothing Then
                FS.Close()
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出-学校ツール連携データ移出(金融機関CSV作成))終了", "成功", "")

        End Try
    End Function

    Private Sub txtSYear_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSYear.LostFocus
        txtSYear.Text = String.Format("{0:0000}", GCom.NzInt(txtSYear.Text))
    End Sub

    Private Sub txtSMonth_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSMonth.LostFocus
        txtSMonth.Text = String.Format("{0:00}", GCom.NzInt(txtSMonth.Text))
    End Sub

    Private Sub btnAllon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOn.Click

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)開始", "成功", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = True '((CType(sender, Button).Name.ToUpper = "BTNALLON")) OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)", "失敗", ex.Message)
        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全選択)終了", "成功", "")

        End Try

    End Sub

    Private Sub btnAlloff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllOff.Click

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "0000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)開始", "成功", "")

            For Each item As ListViewItem In ListView1.Items
                item.Checked = False 'OrElse GCom.NzInt(item.SubItems(3), 0) = 1 OrElse GCom.NzInt(item.SubItems(3), 0) = 2)
            Next item
            Me.ListView1.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)", "失敗", ex.Message)
        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全解除)終了", "成功", "")

        End Try

    End Sub

    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
