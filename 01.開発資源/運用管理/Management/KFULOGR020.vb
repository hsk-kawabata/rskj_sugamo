Imports System
Imports System.Text
Imports System.Drawing.Printing
Imports MenteCommon
Imports System.IO

Public Class KFULOGR020

#Region "宣言"

    Inherits System.Windows.Forms.Form

    ' 出力プリンタ
    Public PRINTERNAME As String = ""

    Private MainLog As New CASTCommon.BatchLOG("KFULOGR020", "データ伝送ログ参照画面")
    Const msgTitle As String = "データ伝送ログ参照画面(KFULOGR020)"

    Private strIN_FILE_NAME As String
    Private strOUT_FILE_NAME As String
    Private strCSV_FILE_NAME As String
    Private intFILE_NO_1 As Integer
    Private intFILE_NO_2 As Integer
    Private intGYO As Integer, intPAGE As Integer
    Private dblREC_COUNT As Double
    Private intTUUBAN As Integer
    Private strDENSOU As String
    Private strDenDate As String

    '共通イベントコントロール
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    'クリックした列の番号
    Dim ClickedColumn As Integer

    'ソートオーダーフラグ
    Dim SortOrderFlag As Boolean = True

    ' 2016/03/15 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- START
    Private INI_RSV2_BIWARE_STATUS As String
    Private INI_RSV2_BIWARE_ERROR As String
    ' 2016/03/15 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- END

#End Region

#Region "画面制御"

    '画面ロード
    Private Sub KFULOGR020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            MainLog.UserID = GCom.GetUserID
            MainLog.Write("0000000000-00", "00000000", "(ロード)開始", "成功")

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            txtJyusinDateY.Text = Format(System.DateTime.Today, "yyyy")
            txtJyusinDateM.Text = Format(System.DateTime.Today, "MM")
            txtJyusinDateD.Text = Format(System.DateTime.Today, "dd")

            'gstrIFileName = CurDir()     'カレントディレクトリの取得
            'gstrIFileName = gstrIFileName & "\FSKJ.INI"
            'gstrIAppName = "COMMON"
            'gstrIKeyName = "LST"
            'gstrIDefault = "err"
            'gintTEMP_LEN = 0
            'gstrTEMP = Space(100)

            'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
            'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
            '    MessageBox.Show(MSG0001E.Replace("{0}", "LSTディレクトリ").Replace("{1}", "COMMON").Replace("{2}", "LST"), _
            '         msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    Exit Sub
            'Else
            '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
            '    Dim a As String = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
            '    'M_FUSION.gstrLST_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
            'End If
            Dim BIWARE As String = CASTCommon.GetFSKJIni("COMMON", "BIWARE")
            If BIWARE.ToUpper = "ERR" OrElse BIWARE = Nothing Then
                MessageBox.Show(MSG0001E.Replace("{0}", "BIWAREログフォルダ").Replace("{1}", "COMMON").Replace("{2}", "BIWARE"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            Dim LOG As String = CASTCommon.GetFSKJIni("COMMON", "LOG")
            If LOG.ToUpper = "ERR" OrElse LOG = Nothing Then
                MessageBox.Show(MSG0001E.Replace("{0}", "ログフォルダ").Replace("{1}", "COMMON").Replace("{2}", "LOG"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' 2016/03/15 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- START
            '-----------------------------------------
            ' リストのエラー出力有無
            '-----------------------------------------
            INI_RSV2_BIWARE_ERROR = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BIWARE_ERROR")
            If INI_RSV2_BIWARE_ERROR.ToUpper = "ERR" OrElse INI_RSV2_BIWARE_ERROR = Nothing Then
                MessageBox.Show(MSG0001E.Replace("{0}", "エラー表示有無").Replace("{1}", "RSV2_V1.0.0").Replace("{2}", "BIWARE_ERROR"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '-----------------------------------------
            ' リストのステータス表示有無
            '-----------------------------------------
            INI_RSV2_BIWARE_STATUS = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BIWARE_STATUS")
            If INI_RSV2_BIWARE_STATUS.ToUpper = "ERR" OrElse INI_RSV2_BIWARE_STATUS = Nothing Then
                MessageBox.Show(MSG0001E.Replace("{0}", "ステータス表示有無").Replace("{1}", "RSV2_V1.0.0").Replace("{2}", "BIWARE_STATUS"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Select Case INI_RSV2_BIWARE_STATUS
                Case "1"
                    '-----------------------------------------
                    ' ステータス表示あり
                    '-----------------------------------------
                    lstLireki.Columns(0).Width = 0
                    lstLireki.Columns(1).Width = 75
                    lstLireki.Columns(2).Width = 75
                    lstLireki.Columns(3).Width = 155
                    lstLireki.Columns(4).Width = 215
                    lstLireki.Columns(5).Width = 60
                    lstLireki.Columns(6).Width = 80
                    lstLireki.Columns(7).Width = 70
                Case Else
                    '-----------------------------------------
                    ' ステータス表示なし
                    '-----------------------------------------
                    lstLireki.Columns(0).Width = 0
                    lstLireki.Columns(1).Width = 80
                    lstLireki.Columns(2).Width = 80
                    lstLireki.Columns(3).Width = 215
                    lstLireki.Columns(4).Width = 215
                    lstLireki.Columns(5).Width = 60
                    lstLireki.Columns(6).Width = 80
                    lstLireki.Columns(7).Width = 0
            End Select
            ' 2016/03/15 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- END

            strIN_FILE_NAME = Path.Combine(BIWARE, "ZHistory.CTM")
            strOUT_FILE_NAME = LOG & "BIWARE.LOG"

            btnPrint.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(ロード)", "失敗", ex.Message)
        Finally
            MainLog.Write("0000000000-00", "00000000", "(ロード)終了", "成功")
        End Try
    End Sub

    '画面クローズ
    Private Sub Form1_FormClosing(ByVal sender As Object, _
            ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            MainLog.Write("0000000000-00", "00000000", "(終了)開始", "成功")
            'If e.CloseReason = CloseReason.UserClosing Then
            '    Me.Owner.Show()
            'End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(終了)", "失敗", ex.Message)
        Finally
            MainLog.Write("0000000000-00", "00000000", "(終了)終了", "成功")
        End Try
    End Sub

#End Region

#Region "ボタン"

    '参照ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Dim dblREC_COUNT As Double = 0
        Dim intLIST_COUNT As Integer = 0
        Dim dblGYO As Double = 1
        Dim byt1 As Byte
        Dim dblREC As Double = 0
        ' 2016/03/15 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- START
        'Dim strLogList(6) As String
        Dim strLogList(7) As String
        ' 2016/03/15 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- END

        Try
            MainLog.Write("0000000000-00", "00000000", "(参照)開始", "成功")
            Cursor.Current = Cursors.WaitCursor()

            strDenDate = txtJyusinDateY.Text & txtJyusinDateM.Text & txtJyusinDateD.Text
            If fn_Select_YASUMIMAST(strDenDate) = True Then

                MessageBox.Show(MSG0093W, _
                                msgTitle, _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Warning)
                Me.txtJyusinDateY.Focus()
                Exit Sub
            End If

            If txtJyusinDateY.Text = "" Then
                MessageBox.Show(MSG0018W, _
                                msgTitle, _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Warning)
                Me.txtJyusinDateY.Focus()
                Exit Sub
            End If

            If txtJyusinDateM.Text = "" Then
                MessageBox.Show(MSG0020W, _
                                msgTitle, _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Warning)
                Me.txtJyusinDateM.Focus()
                Exit Sub
            End If

            '範囲チェック
            If Not (txtJyusinDateM.Text >= 1 And txtJyusinDateM.Text <= 12) Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtJyusinDateM.Focus()
                Exit Sub
            End If

            If txtJyusinDateD.Text = "" Then
                MessageBox.Show(MSG0023W, _
                                msgTitle, _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Warning)
                Me.txtJyusinDateD.Focus()
                Exit Sub
            End If

            '範囲チェック
            If Not (txtJyusinDateD.Text >= 1 And txtJyusinDateD.Text <= 31) Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateD.Focus()
                Exit Sub
            End If

            '------------------------------------------------
            '日付チェック
            '------------------------------------------------
            Dim WORK_DATE As String = txtJyusinDateY.Text & "/" & txtJyusinDateM.Text & "/" & txtJyusinDateD.Text

            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateY.Focus()
                Exit Sub
            End If
            '-----------------------------------------
            'NULL値を､スペースに置き換える
            '-----------------------------------------
            If Dir(strOUT_FILE_NAME) <> "" Then
                Kill(strOUT_FILE_NAME)
            End If

            If File.Exists(strIN_FILE_NAME) = False Then
                MessageBox.Show(MSG0255W.Replace("{0}", "伝送ログ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            intFILE_NO_1 = FreeFile()
            FileOpen(intFILE_NO_1, strIN_FILE_NAME, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)   '入力ファイル
            intFILE_NO_2 = FreeFile()
            FileOpen(intFILE_NO_2, strOUT_FILE_NAME, OpenMode.Binary, OpenAccess.Write, OpenShare.Shared)   '出力ファイル

            Do Until EOF(intFILE_NO_1)
                FileGet(intFILE_NO_1, byt1)
                If byt1 = 0 Then
                    byt1 = 32
                End If
                If byt1 = 13 Then
                    FileGet(intFILE_NO_1, byt1)
                    If byt1 = 10 Then
                        dblGYO += 1
                    End If
                Else
                    If dblGYO > 1 Then
                        FilePut(intFILE_NO_2, byt1)
                    End If
                End If
            Loop

            FileClose(intFILE_NO_1)
            FileClose(intFILE_NO_2)

            If Err.Number <> 0 Then
                MessageBox.Show(MSG0233W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '-----------------------------------------
            'ログファイルの参照
            '-----------------------------------------
            lstLireki.Items.Clear()
            intFILE_NO_1 = FreeFile()
            FileOpen(intFILE_NO_1, strOUT_FILE_NAME, OpenMode.Random, , , 366)

            Do Until EOF(intFILE_NO_1)
                dblREC_COUNT += 1
                FileGet(intFILE_NO_1, gstrBiware, dblREC_COUNT)


                If gstrBiware.LOG1 = txtJyusinDateY.Text & "/" & txtJyusinDateM.Text & "/" & txtJyusinDateD.Text Then
                    ' 2016/03/15 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- START
                    'If gstrBiware.LOG3.Trim <> "" Then
                    '    '-----------------------------------------
                    '    'リストに追加
                    '    '-----------------------------------------
                    '    strDENSOU = ""
                    '    Select Case gstrBiware.LOG6
                    '        Case "端受"
                    '            strDENSOU = "受信"
                    '        Case "端送"
                    '            strDENSOU = "送信"
                    '        Case Else
                    '            strDENSOU = gstrBiware.LOG6
                    '    End Select

                    '    strLogList(1) = gstrBiware.LOG2
                    '    strLogList(2) = gstrBiware.LOG13
                    '    strLogList(3) = Trim(gstrBiware.LOG5)
                    '    strLogList(4) = Trim(gstrBiware.LOG8.Substring(0, 40))
                    '    strLogList(5) = strDENSOU
                    '    strLogList(6) = Val(gstrBiware.LOG10)

                    '    Dim lstWork As New ListViewItem(strLogList)

                    '    lstLireki.Items.AddRange(New ListViewItem() {lstWork})

                    '    intLIST_COUNT += 1

                    'End If
                    Select Case INI_RSV2_BIWARE_ERROR
                        Case "1"
                            '-----------------------------------------
                            ' リスト編集(エラー分あり)
                            '-----------------------------------------
                            If gstrBiware.LOG3.Trim <> "" Then
                                strDENSOU = ""
                                Select Case gstrBiware.LOG6
                                    Case "端受"
                                        strDENSOU = "受信"
                                    Case "端送"
                                        strDENSOU = "送信"
                                    Case Else
                                        strDENSOU = gstrBiware.LOG6
                                End Select

                                strLogList(1) = gstrBiware.LOG2
                                strLogList(2) = gstrBiware.LOG13
                                strLogList(3) = Trim(gstrBiware.LOG5)
                                strLogList(4) = Trim(gstrBiware.LOG8.Substring(0, 40))
                                strLogList(5) = strDENSOU
                                strLogList(6) = Val(gstrBiware.LOG10)
                                strLogList(7) = gstrBiware.LOG11

                                Dim lstWork As New ListViewItem(strLogList)
                                lstLireki.Items.AddRange(New ListViewItem() {lstWork})
                                intLIST_COUNT += 1
                            End If
                        Case Else
                            '-----------------------------------------
                            ' リスト編集(エラー分なし)
                            '-----------------------------------------
                            If gstrBiware.LOG3.Trim <> "" And gstrBiware.LOG11 = "0000000" Then
                                strDENSOU = ""
                                Select Case gstrBiware.LOG6
                                    Case "端受"
                                        strDENSOU = "受信"
                                    Case "端送"
                                        strDENSOU = "送信"
                                    Case Else
                                        strDENSOU = gstrBiware.LOG6
                                End Select

                                strLogList(1) = gstrBiware.LOG2
                                strLogList(2) = gstrBiware.LOG13
                                strLogList(3) = Trim(gstrBiware.LOG5)
                                strLogList(4) = Trim(gstrBiware.LOG8.Substring(0, 40))
                                strLogList(5) = strDENSOU
                                strLogList(6) = Val(gstrBiware.LOG10)
                                strLogList(7) = gstrBiware.LOG11

                                Dim lstWork As New ListViewItem(strLogList)
                                lstLireki.Items.AddRange(New ListViewItem() {lstWork})
                                intLIST_COUNT += 1
                            End If
                    End Select
                    ' 2016/03/15 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- END
                End If
            Loop

            FileClose(intFILE_NO_1)

            '印刷用に現在の処理日テキストボックスを保持
            strDenDate = txtJyusinDateY.Text & txtJyusinDateM.Text & txtJyusinDateD.Text

            If intLIST_COUNT = 0 Then
                MessageBox.Show(MSG0026W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                btnPrint.Enabled = False
                Exit Sub
            End If

            '開始時間
            lstLireki.ListViewItemSorter = New ListViewItemComparer(1, SortOrderFlag, HorizontalAlignment.Center)
            lstLireki.Sort()

            btnPrint.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(参照)終了", "失敗", ex.Message)
        Finally
            MainLog.Write("0000000000-00", "00000000", "(参照)終了", "成功")
        End Try

    End Sub

    '印刷ボタン
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Dim strDIR As String

        Try
            MainLog.Write("0000000000-00", "00000000", "(印刷)開始", "成功")
            If MessageBox.Show(MSG0013I.Replace("{0}", "データ伝送ログ一覧"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If

            Cursor.Current = Cursors.WaitCursor()
            strDIR = CurDir()

            '---------------------------------------------------
            'ＣＳＶファイル作成
            '---------------------------------------------------
            If fn_CreateCSV() = False Then
                Exit Sub
            End If
            ChDir(strDIR)

            '---------------------------------------------------
            '印刷実行
            '---------------------------------------------------
            If fn_Print() = False Then
                Exit Sub
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(印刷)終了", "失敗", ex.Message)
        Finally
            MainLog.Write("0000000000-00", "00000000", "(印刷)終了", "成功")
        End Try

    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Me.Close()
    End Sub

#End Region

#Region "テキストボックス"

    '基準年／登録月日
    Private Sub DateTextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtJyusinDateY.Validating, _
            txtJyusinDateM.Validating, _
            txtJyusinDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MainLog.Write("0000000000-00", "00000000", "(日付テキストボックス)", "失敗", ex.Message)
        End Try
    End Sub

#End Region

#Region "関数"
    Public Function fn_CreateCSV() As Boolean
        '============================================================================
        'NAME           :fn_CreateCSV
        'Parameter      :
        'Description    :帳票印刷用ＣＳＶファイル作成
        'Return         :
        'Create         :2009/09/02
        'Update         :
        '============================================================================
        Dim i As Integer = 0

        Try

            If lstLireki.Items.Count = 0 Then
                '印刷対象なし
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Else
                '------------------------------------------------
                'ＣＳＶファイル作成
                '------------------------------------------------
                Dim CreateCSV As New clsKFUP001CSV
                strCSV_FILE_NAME = CreateCSV.CreateCsvFile()

                For i = 0 To lstLireki.Items.Count - 1
                    Dim lstWork As New ListViewItem
                    lstWork = lstLireki.Items(i)

                    CreateCSV.OutputCsvData(Format(DateTime.Now, "yyyyMMdd"), True) 'システム日付
                    CreateCSV.OutputCsvData(Format(DateTime.Now, "HHmmss"), True)   'タイムスタンプ
                    CreateCSV.OutputCsvData(strDenDate, True)                       '伝送日付
                    CreateCSV.OutputCsvData(i + 1, True)                            '項番
                    'CreateCSV.OutputCsvData(lstWork.SubItems(0).Text, True)         '開始時間
                    'CreateCSV.OutputCsvData(lstWork.SubItems(1).Text, True)         '終了時間
                    'CreateCSV.OutputCsvData(lstWork.SubItems(2).Text, True)         'センター名
                    'CreateCSV.OutputCsvData(lstWork.SubItems(3).Text, True)         'ファイル名
                    'CreateCSV.OutputCsvData(lstWork.SubItems(4).Text, True)         '送受信
                    'CreateCSV.OutputCsvData(lstWork.SubItems(5).Text, True, True)   'レコード件数
                    CreateCSV.OutputCsvData(lstWork.SubItems(1).Text, True)         '開始時間
                    CreateCSV.OutputCsvData(lstWork.SubItems(2).Text, True)         '終了時間
                    CreateCSV.OutputCsvData(lstWork.SubItems(3).Text, True)         'センター名
                    CreateCSV.OutputCsvData(lstWork.SubItems(4).Text, True)         'ファイル名
                    CreateCSV.OutputCsvData(lstWork.SubItems(5).Text, True)         '送受信
                    ' 2016/03/15 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- START
                    'CreateCSV.OutputCsvData(lstWork.SubItems(6).Text, True, True)   'レコード件数
                    CreateCSV.OutputCsvData(lstWork.SubItems(6).Text, True)         'レコード件数
                    Select Case INI_RSV2_BIWARE_STATUS
                        Case "1"
                            '-----------------------------------------
                            ' ステータス表示あり
                            '-----------------------------------------
                            CreateCSV.OutputCsvData(lstWork.SubItems(7).Text, True, True)   '結果コード
                        Case Else
                            '-----------------------------------------
                            ' ステータス表示なし
                            '-----------------------------------------
                            CreateCSV.OutputCsvData("", True, True)   '結果コード
                    End Select
                    ' 2016/03/15 タスク）綾部 CHG 【ST】UI_B-14-99(RSV2対応) -------------------- END
                Next

                CreateCSV.CloseCsv()
                CreateCSV = Nothing

                Return True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0231W.Replace("{0}", "データ伝送ログ一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLog.Write("0000000000-00", "00000000", "(印刷用CSV作成)", "失敗", ex.Message)
            Return False
        End Try

    End Function

    Public Function fn_Print() As Boolean
        '============================================================================
        'NAME           :fn_CreateCSV
        'Parameter      :
        'Description    :帳票印刷用ＣＳＶファイル作成
        'Return         :
        'Create         :2009/09/02
        'Update         :
        '============================================================================
        Dim ErrMessage As String = ""
        Dim Param As String = ""

        Try
            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：ログイン名、ＣＳＶファイル名
            Param = gstrUSER_ID & "," & strCSV_FILE_NAME

            Dim nRet As Integer = ExeRepo.ExecReport("KFUP001.EXE", Param)

            If nRet <> 0 Then
                '印刷失敗：戻り値に対応したエラーメッセージを表示する
                Select Case nRet
                    Case 1
                        ErrMessage = "パラメータの取得に失敗しました。"
                    Case 2
                        ErrMessage = "ログを参照してください。"
                    Case -1
                        ErrMessage = "ログを参照してください。"
                End Select

                MessageBox.Show(MSG0004E.Replace("{0}", "データ伝送ログ一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Return False
            End If

            MessageBox.Show(MSG0014I.Replace("{0}", "データ伝送ログ一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write("0000000000-00", "00000000", "(データ伝送ログ一覧印刷)", "失敗", ex.Message)
            Return False
        End Try

    End Function

    '処理日（月）LostFocus
    Private Sub txtJyusinDateM_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles txtJyusinDateM.LostFocus

        Try

            If FN_CHK_DATE("MONTH") = False Then
                Return
            End If

        Catch ex As Exception
            MainLog.Write("処理日(月)フォーカス失効", "失敗", ex.Message)
        End Try

    End Sub

    '処理日（日）LostFocus
    Private Sub txtJyusinDateD_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles txtJyusinDateD.LostFocus

        Try

            If FN_CHK_DATE("DAY") = False Then
                Return
            End If

        Catch ex As Exception
            MainLog.Write("処理日(日)フォーカス失効", "失敗", ex.Message)
        End Try

    End Sub

    Private Function FN_CHK_DATE(ByVal TextType As String) As Boolean

        Try
            Dim MSG As String
            Dim DRet As DialogResult

            Select Case TextType
                Case "MONTH"
                    If Me.txtJyusinDateM.Text <> "" Then

                        MSG = MSG0022W
                        Select Case CInt(Me.txtJyusinDateM.Text)
                            Case 0, Is >= 13
                                DRet = MessageBox.Show(MSG, msgTitle, _
                                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Me.txtJyusinDateM.Focus()
                                Return False
                        End Select
                    End If

                Case "DAY"
                    If Me.txtJyusinDateD.Text <> "" Then

                        MSG = MSG0025W
                        Select Case CInt(Me.txtJyusinDateD.Text)
                            Case 0, Is >= 32
                                DRet = MessageBox.Show(MSG, msgTitle, _
                                                       MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Me.txtJyusinDateD.Focus()
                                Return False
                        End Select
                    End If
            End Select

            Return True
        Catch ex As Exception
            MainLog.Write("日付チェック", "失敗", ex.Message)
        End Try
    End Function

    'フォーカス移動
    Private Sub txtJyusinDateD_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) _
    Handles txtJyusinDateD.KeyPress
        Try

            Select Case Microsoft.VisualBasic.Asc(e.KeyChar)
                Case Keys.Tab, Keys.Return
                    Select Case CType(sender, Control).Name
                        Case "txtJyusinDateD"
                            '参照ボタン
                            Me.btnAction.Focus()
                    End Select
            End Select

        Catch ex As Exception
            MainLog.Write("フォーカス移動", "失敗", ex.Message)
        End Try
    End Sub

    Private Function fn_Select_YASUMIMAST(ByVal strYASUMI_DATE As String) As Boolean

        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As CASTCommon.MyOracleReader
        Dim SQL As StringBuilder

        Try

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            SQL = New StringBuilder(128)

            SQL.Append("SELECT YASUMI_DATE_Y")
            SQL.Append(" FROM YASUMIMAST")
            SQL.Append(" WHERE YASUMI_DATE_Y = '" & strYASUMI_DATE & "'")

            If OraReader.DataReader(SQL) = True Then
                OraReader.Close()
                Return True
            Else
                OraReader.Close()
                Return False
            End If

        Catch ex As Exception
            MainLog.Write("休日マスタ検索", "失敗", ex.Message)
            Return False
        End Try
    End Function

    '一覧表示領域のソート
    Private Sub LetSort_ListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
    Handles lstLireki.ColumnClick

        Try

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
                .ListViewItemSorter = New ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)

                ' ソート実行
                .Sort()

            End With

        Catch ex As Exception
            Throw
        End Try

    End Sub
#End Region

End Class


