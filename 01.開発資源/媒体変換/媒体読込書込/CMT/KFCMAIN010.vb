' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Imports CAstBatch
Imports System.Configuration
Imports System.Xml
Imports System.Windows.Forms
Imports System.Drawing

Public Class KFCMAIN010

#Region " 定数/変数 "

    '--------------------------------
    ' 共通関連項目
    '--------------------------------
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private clsFUSION As New clsFUSION.clsMain()

    '--------------------------------
    ' LOG関連項目
    '--------------------------------
    Private MainLOG As New CASTCommon.BatchLOG("KFCMAIN010", "媒体読込（媒体→ディスク）画面")
    Private Const msgTitle As String = "媒体読込（媒体→ディスク）画面(KFCMAIN010)"
    Private Structure LogWrite
        Dim UserID As String                         ' ユーザID
        Dim ToriCode As String                       ' 取引先主副コード
        Dim FuriDate As String                       ' 振替日
    End Structure
    Private LW As LogWrite

    '--------------------------------
    ' Oracle関連項目
    '--------------------------------
    Private MainDB As CASTCommon.MyOracle            ' パブリックデーターベース

    '--------------------------------
    ' リスト関連項目
    '--------------------------------
    Private ClickedColumn As Integer                     ' クリックした列番号
    Private SortOrderFlag As Boolean = True              ' ソートオーダーフラグ
    Private ListViewArray As ArrayList                   ' リスト編集用ArrayList

    '--------------------------------
    ' INI関連項目
    '--------------------------------
    Friend Structure INI_INFO
        Dim COMMON_TXT As String                     ' TXTフォルダ
        Dim COMMON_FDDRIVE As String                 ' FDドライブ
        Dim COMMON_BAITAI_1 As String                ' COMMON-BAITAI_1
        Dim COMMON_BAITAI_2 As String                ' COMMON-BAITAI_2
        Dim COMMON_BAITAI_3 As String                ' COMMON-BAITAI_3
        Dim COMMON_BAITAI_4 As String                ' COMMON-BAITAI_4
        Dim COMMON_BAITAI_5 As String                ' COMMON-BAITAI_5
        Dim COMMON_BAITAIREAD As String              ' 媒体読込データ格納フォルダ
        '2016/02/05 タスク）斎藤 RSV2対応 ADD ---------------------------------------- START
        Dim COMMON_FTR As String                     ' FTRフォルダ
        Dim COMMON_FTRANP As String                  ' FTRANPフォルダ
        '2016/02/05 タスク）斎藤 RSV2対応 ADD ---------------------------------------- END
    End Structure
    Private IniInfo As INI_INFO

    Private mArgumentData As CommData

    '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- START
    Private nRecordNumber As Integer
    '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- END

    '2017/02/24 タスク）綾部 RSV2対応 ADD ---------------------------------------- START
    Private SearchFlg As Boolean
    '2017/02/24 タスク）綾部 RSV2対応 ADD ---------------------------------------- END

#End Region

#Region " ロード "

    Private Sub KFCMAIN010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '--------------------------------
            ' ログ情報設定
            '--------------------------------
            LW.UserID = GCom.GetUserID
            If SetLogInfo(True) = False Then
                Exit Try
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "開始", "")

            '--------------------------------
            ' 休日マスタ取込
            '--------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", "休日情報取得")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            End If

            '--------------------------------
            ' システム日付とユーザ名を表示
            '--------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '--------------------------------
            ' 委託者名リストボックス設定
            '--------------------------------
            Dim Jyoken As String = " AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')"
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            '--------------------------------
            ' INI情報取得
            '--------------------------------
            If SetIniFIle() = False Then
                Exit Try
            End If

            '--------------------------------
            ' 画面の初期化
            '--------------------------------
            Me.ListView1.Items.Clear()
            '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- START
            nRecordNumber = 1
            '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- END
            lblItakuCode.Text = ""
            lblItakuName.Text = ""
            lblCodeKbn.Text = ""
            lblFileName.Text = ""
            lblBaitai.Text = ""
            lblBaitaiCode.Text = ""
            SearchFlg = False

            '--------------------------------
            ' ボタンの初期化
            '--------------------------------
            Me.btnRead.Enabled = True
            Me.btnReset.Enabled = True
            Me.btnEnd.Enabled = True

            Me.txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "終了", "")
        End Try

    End Sub

#End Region

#Region " ボタン "

    '================================
    ' 読込開始ボタン
    '================================
    Private Sub btnRead_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRead.Click

        Dim WorkFileName As String = IniInfo.COMMON_BAITAIREAD & "WORKDATA.DAT"

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "開始", "")

            '--------------------------------
            ' ログ情報設定
            '--------------------------------
            If SetLogInfo(False) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "失敗", "ログ情報設定")
                Exit Try
            End If

            '--------------------------------
            ' 取引先マスタチェック
            '--------------------------------
            If CheckTorimast(txtTorisCode.Text, txtTorifCode.Text) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "失敗", "取引先マスタチェック")
                Exit Try
            End If

            '--------------------------------
            ' 処理開始確認メッセージ
            '--------------------------------
            If MessageBox.Show(String.Format(MSG0077I, "入力した取引先", "媒体読込処理"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "中断", "ユーザキャンセル")
                Exit Try
            End If

            Dim CopyReturn As Integer = 0

            MainDB = New CASTCommon.MyOracle
            mArgumentData = New CommData(MainDB)
            Dim InfoParam As New CommData.stPARAMETER
            Call mArgumentData.GetTORIMAST(txtTorisCode.Text, txtTorifCode.Text)

            InfoParam.FSYORI_KBN = mArgumentData.INFOToriMast.FSYORI_KBN_T
            InfoParam.TORI_CODE = mArgumentData.INFOToriMast.TORIS_CODE_T & mArgumentData.INFOToriMast.TORIF_CODE_T
            InfoParam.BAITAI_CODE = mArgumentData.INFOToriMast.BAITAI_CODE_T
            InfoParam.FMT_KBN = mArgumentData.INFOToriMast.FMT_KBN_T
            InfoParam.FURI_DATE = ""
            InfoParam.CODE_KBN = mArgumentData.INFOToriMast.CODE_KBN_T
            InfoParam.LABEL_KBN = mArgumentData.INFOToriMast.LABEL_KBN_T
            InfoParam.RENKEI_FILENAME = ""
            InfoParam.ENC_KBN = mArgumentData.INFOToriMast.ENC_KBN_T
            InfoParam.ENC_KEY1 = mArgumentData.INFOToriMast.ENC_KEY1_T
            InfoParam.ENC_KEY2 = mArgumentData.INFOToriMast.ENC_KEY2_T
            InfoParam.ENC_OPT1 = mArgumentData.INFOToriMast.ENC_OPT1_T
            InfoParam.CYCLENO = ""
            InfoParam.JOBTUUBAN = 1
            InfoParam.TIME_STAMP = DateTime.Now.ToString("HHmmss")
            mArgumentData.INFOParameter = InfoParam

            Dim ReadFMT As New CAstFormat.CFormat
            ReadFMT.LOG = MainLOG
            ReadFMT = CAstFormat.CFormat.GetFormat(mArgumentData.INFOParameter)
            ReadFMT.ToriData = mArgumentData

            '--------------------------------
            ' 媒体内ファイルチェック
            '--------------------------------
            Dim BaitaiDrive As String = String.Empty
            Dim BaitaiFileCount As Integer = 0
            Select Case lblBaitaiCode.Text
                Case "01" : BaitaiDrive = IniInfo.COMMON_FDDRIVE
                Case "11" : BaitaiDrive = IniInfo.COMMON_BAITAI_1
                Case "12" : BaitaiDrive = IniInfo.COMMON_BAITAI_2
                Case "13" : BaitaiDrive = IniInfo.COMMON_BAITAI_3
                Case "14" : BaitaiDrive = IniInfo.COMMON_BAITAI_4
                Case "15" : BaitaiDrive = IniInfo.COMMON_BAITAI_5
            End Select
            If lblBaitaiCode.Text = "01" And mArgumentData.INFOToriMast.CODE_KBN_T = "4" Then
                ' ＩＢＭフォーマットＦＤの場合はチェックを行わない
            Else
                BaitaiFileCount = Directory.GetFiles(BaitaiDrive).Length
                Select Case BaitaiFileCount
                    Case 0
                        MessageBox.Show(String.Format(MSG0377W, "セットした媒体にファイルが存在しません"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "失敗", "媒体内ファイル０件")
                        Exit Try
                    Case 1
                    Case Else
                        If SearchFlg = False Then
                            MessageBox.Show(String.Format(MSG0377W, "セットした媒体にファイルが複数存在します"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "失敗", "媒体内ファイル複数")
                            Exit Try
                        End If
                End Select
            End If

            '--------------------------------
            ' 媒体読込　処理開始
            '--------------------------------
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            Dim ReadFileName As String = lblFileName.Text.Trim
            Select Case mArgumentData.INFOToriMast.BAITAI_CODE_T
                Case "01"
                    CopyReturn = clsFUSION.fn_FD_CPYTO_DISK(mArgumentData.INFOToriMast.TORIS_CODE_T & mArgumentData.INFOToriMast.TORIF_CODE_T, _
                                                            ReadFileName, _
                                                            WorkFileName, _
                                                            ReadFMT.RecordLen, _
                                                            mArgumentData.INFOToriMast.CODE_KBN_T, _
                                                            ReadFMT.FTRANP, _
                                                            msgTitle)
                Case Else
                    CopyReturn = clsFUSION.fn_DVD_CPYTO_DISK(mArgumentData.INFOToriMast.TORIS_CODE_T & mArgumentData.INFOToriMast.TORIF_CODE_T, _
                                                             ReadFileName, _
                                                             WorkFileName, _
                                                             ReadFMT.RecordLen, _
                                                             mArgumentData.INFOToriMast.CODE_KBN_T, _
                                                             ReadFMT.FTRANP, _
                                                             msgTitle, _
                                                             mArgumentData.INFOToriMast.BAITAI_CODE_T)
            End Select

            Select Case CopyReturn
                Case 0
                    'ReadFMT. = WorkFileName
                Case 100
                    MessageBox.Show(String.Format(MSG0371W, "媒体読込処理", "取引先のファイル名を確認してください。"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "失敗", "処理異常またはファイル名なし")
                    Exit Try
                Case 200
                    MessageBox.Show(String.Format(MSG0371W, "媒体読込処理", "ファイル読込をキャンセルします。"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "中断", "ファイル読込キャンセル")
                    Exit Try
                Case 300
                    MessageBox.Show(String.Format(MSG0371W, "媒体読込処理", "コード区分異常（JIS改行なし）"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "失敗", "コード区分異常（JIS改行なし）")
                    Exit Try
                Case 400
                    MessageBox.Show(String.Format(MSG0371W, "媒体読込処理", "中間ファイル作成処理失敗。"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "失敗", "出力ファイル作成:" & WorkFileName)
                    Exit Try
            End Select

            '--------------------------------
            ' データ情報取得
            '--------------------------------
            Dim FuriDate As String = ""
            Dim Message As String = ""
            ListViewArray = New ArrayList
            ListViewArray.Clear()

            If GetDataInfo(ReadFMT, InfoParam, WorkFileName, FuriDate, Message) = False Then
                If Message <> "" Then
                    MessageBox.Show(String.Format(MSG0371W, "媒体読込処理", Message), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "失敗", "データ情報取得処理失敗(" & Message & ")")
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "失敗", "データ情報取得処理失敗")
                End If
                If Not ReadFMT Is Nothing Then
                    ReadFMT.Close()
                    ReadFMT = Nothing
                End If
                Exit Try
            Else
                ReadFMT.Close()
                ReadFMT = Nothing
            End If

            '--------------------------------
            ' ファイル名構築
            '--------------------------------
            Dim FileName As String = ""
            If MakeFileName(FileName, FuriDate) = False Then
                Exit Try
            End If

            Dim FileList() As String = Directory.GetFiles(IniInfo.COMMON_BAITAIREAD)
            For i As Integer = 0 To FileList.Length - 1 Step 1
                Dim DelFileName As String = Path.GetFileName(FileList(i))
                If DelFileName <> Path.GetFileName(WorkFileName) Then
                    If DelFileName.Substring(0, 32) = FileName.Substring(0, 32) Then
                        Dim FSyoriName As String = String.Empty
                        Select Case rdbKigyo.Checked
                            Case True
                                FSyoriName = "振替日"
                            Case False
                                FSyoriName = "振込日"
                        End Select

                        Dim DispName As String = String.Empty
                        Select Case FileName.Substring(6, 1)
                            Case "S"
                                DispName = "取引先コード：" & FileName.Substring(8, 10) & "-" & FileName.Substring(18, 2)
                            Case "M"
                                DispName = "代表委託者コード：" & FileName.Substring(8, 10)
                        End Select

                        If MessageBox.Show("同一" & FSyoriName & "のデータが存在します。" & vbCrLf & _
                                        "既に読み込まれていファイルを削除し、処理を続行しますか？" & vbCrLf & vbCrLf & _
                                        "　" & DispName & vbCrLf & _
                                        "　" & FSyoriName & "：" & FileName.Substring(24, 8), _
                                        msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then

                            MessageBox.Show("媒体読込処理を終了します。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "キャンセル", _
                                          "同一" & FSyoriName & "データあり。" & _
                                          "　" & DispName & _
                                          "　" & FSyoriName & "：" & FileName.Substring(24, 8))
                            Exit Try
                        End If
                    End If
                End If
            Next

            '--------------------------------
            ' リスト情報編集
            '--------------------------------
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "", "リスト情報編集 開始")
            For i As Integer = 0 To ListViewArray.Count - 1 Step 1
                Dim vLstItem As New ListViewItem(ListViewArray(i).ToString.Split("/"), -1, Color.Black, Color.White, Nothing)
                ListView1.Items.AddRange({vLstItem})
            Next

            '--------------------------------
            ' ファイル操作
            '--------------------------------
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "", "ファイル操作 開始(既存ファイル削除)")
            FileList = Directory.GetFiles(IniInfo.COMMON_BAITAIREAD)
            For i As Integer = 0 To FileList.Length - 1 Step 1
                Dim DelFileName As String = Path.GetFileName(FileList(i))
                If DelFileName <> Path.GetFileName(WorkFileName) Then
                    If DelFileName.Substring(0, 32) = FileName.Substring(0, 32) Then
                        File.Delete(FileList(i))
                    End If
                End If
            Next

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "", "ファイル操作 開始(ファイルコピー)")
            '2016/02/05 タスク）斎藤 RSV2対応 UPD ---------------------------------------- START
            '落し込み時は取引先マスタに設定されているコード区分で処理されるため、
            'コピーしたファイルを元のコードに戻す必要がある。
            If InfoParam.CODE_KBN = "0" Then
                'JISの場合はそのままコピー
                File.Copy(WorkFileName, Path.Combine(Me.IniInfo.COMMON_BAITAIREAD, FileName))
            Else
                'JIS以外はFTRANPを使用してコード変換を行う
                Dim MapFile As String = String.Empty
                If Me.GetPFileInfo(InfoParam.FMT_KBN, InfoParam.CODE_KBN, MapFile) = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "コード変換情報取得")
                    Exit Try
                End If

                Dim GetOrPut As String = If(InfoParam.CODE_KBN = "4", "PUTRAND", "GETDATA")
                If Me.ConvertFileFtranP(GetOrPut, WorkFileName, Path.Combine(Me.IniInfo.COMMON_BAITAIREAD, FileName), Path.Combine(Me.IniInfo.COMMON_FTR, MapFile)) <> 0 Then
                    MessageBox.Show(String.Format(MSG0371W, "媒体読込処理", "FTRANPのコード変換処理に失敗しました。"), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Try
                End If
            End If
            'File.Copy(WorkFileName, IniInfo.COMMON_BAITAIREAD & FileName)
            '2016/02/05 タスク）斎藤 RSV2対応 UPD ---------------------------------------- END


            '--------------------------------
            ' 完了メッセージ表示
            '--------------------------------
            MessageBox.Show(String.Format(MSG0078I, "媒体読込処理"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "成功", "出力ファイル:" & IniInfo.COMMON_BAITAIREAD & FileName)
            '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- START
            nRecordNumber += 1
            '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "失敗", ex.Message)
        Finally
            SearchFlg = False

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "", "ファイル操作 開始(ワークファイル削除)")
            If File.Exists(WorkFileName) = True Then
                File.Delete(WorkFileName)
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "読込開始", "終了", "")
        End Try

    End Sub

    '================================
    ' 取消ボタン
    '================================
    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取消", "開始", "")

            '--------------------------------
            ' ログ情報設定
            '--------------------------------
            If SetLogInfo(False) = False Then
                Exit Try
            End If

            '--------------------------------
            ' 画面初期化
            '--------------------------------
            If ClearInfo() = False Then
                Exit Try
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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

            '--------------------------------
            ' ログ情報設定
            '--------------------------------
            If SetLogInfo(True) = False Then
                Exit Try
            End If

            Me.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "終了", "")
        End Try

    End Sub

    Private Sub btnFileSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFileSelect.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "参照", "開始", "")

            '--------------------------------
            ' ログ情報設定
            '--------------------------------
            If SetLogInfo(False) = False Then
                Exit Try
            End If

            '--------------------------------
            ' 媒体情報取得
            '--------------------------------
            Dim BaitaiDrive As String = ""
            Select Case lblBaitaiCode.Text
                Case "01" : BaitaiDrive = IniInfo.COMMON_FDDRIVE
                Case "11" : BaitaiDrive = IniInfo.COMMON_BAITAI_1
                Case "12" : BaitaiDrive = IniInfo.COMMON_BAITAI_2
                Case "13" : BaitaiDrive = IniInfo.COMMON_BAITAI_3
                Case "14" : BaitaiDrive = IniInfo.COMMON_BAITAI_4
                Case "15" : BaitaiDrive = IniInfo.COMMON_BAITAI_5
            End Select

            Dim OPENFILEDIALOG1 As New OpenFileDialog
            OPENFILEDIALOG1.InitialDirectory = BaitaiDrive
            OPENFILEDIALOG1.Multiselect = False
            OPENFILEDIALOG1.CheckFileExists = True
            OPENFILEDIALOG1.FileName = ""
            OPENFILEDIALOG1.AddExtension = True
            OPENFILEDIALOG1.CheckFileExists = True
            Dim dlgRESULT As DialogResult
            dlgRESULT = OPENFILEDIALOG1.ShowDialog()

            If dlgRESULT = DialogResult.Cancel Then    'キャンセルボタンが押されたら
                Exit Try
            Else
                lblFileName.Text = Path.GetFileName(OPENFILEDIALOG1.FileName)
                SearchFlg = True
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "参照", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "参照", "終了", "")
        End Try

    End Sub

#End Region

#Region " 関数(Function) "

    '================================
    ' ログ情報設定
    '================================
    Private Function SetLogInfo(ByVal Initialize As Boolean) As Boolean

        Try
            Select Case Initialize
                Case True
                    LW.ToriCode = "000000000000"
                    LW.FuriDate = "00000000"
                Case False
                    If txtTorisCode.Text.Trim = "" Or txtTorifCode.Text.Trim = "" Then
                        LW.ToriCode = "000000000000"
                    Else
                        LW.ToriCode = txtTorisCode.Text & txtTorifCode.Text
                    End If
                    LW.FuriDate = "00000000"
            End Select

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ログ情報設定", "失敗", ex.Message)
            Return False
        Finally
            ' NOP
        End Try

    End Function

    '================================
    ' INI情報取得
    '================================
    Private Function SetIniFIle() As Boolean

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "開始", "")

            IniInfo.COMMON_TXT = CASTCommon.GetFSKJIni("COMMON", "TXT")
            If IniInfo.COMMON_TXT.ToUpper = "ERR" OrElse IniInfo.COMMON_TXT = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "TXTフォルダ", "COMMON", "TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:TXTフォルダ 分類:COMMON 項目:TXT")
                Return False
            End If

            IniInfo.COMMON_FDDRIVE = CASTCommon.GetFSKJIni("COMMON", "FDDRIVE")
            If IniInfo.COMMON_FDDRIVE.ToUpper = "ERR" OrElse IniInfo.COMMON_FDDRIVE = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "ＦＤドライブ", "COMMON", "FDDRIVE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:ＦＤドライブ 分類:COMMON 項目:FDDRIVE")
                Return False
            End If

            IniInfo.COMMON_BAITAI_1 = CASTCommon.GetFSKJIni("COMMON", "BAITAI_1")
            If IniInfo.COMMON_BAITAI_1.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "フォルダ（媒体１）", "COMMON", "BAITAI_1"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:フォルダ（媒体１） 分類:COMMON 項目:BAITAI_1")
                Return False
            End If

            IniInfo.COMMON_BAITAI_2 = CASTCommon.GetFSKJIni("COMMON", "BAITAI_2")
            If IniInfo.COMMON_BAITAI_2.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "フォルダ（媒体２）", "COMMON", "BAITAI_2"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:フォルダ（媒体２） 分類:COMMON 項目:BAITAI_2")
                Return False
            End If

            IniInfo.COMMON_BAITAI_3 = CASTCommon.GetFSKJIni("COMMON", "BAITAI_3")
            If IniInfo.COMMON_BAITAI_3.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "フォルダ（媒体３）", "COMMON", "BAITAI_3"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:フォルダ（媒体３） 分類:COMMON 項目:BAITAI_3")
                Return False
            End If

            IniInfo.COMMON_BAITAI_4 = CASTCommon.GetFSKJIni("COMMON", "BAITAI_4")
            If IniInfo.COMMON_BAITAI_4.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "フォルダ（媒体４）", "COMMON", "BAITAI_4"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:フォルダ（媒体４） 分類:COMMON 項目:BAITAI_4")
                Return False
            End If

            IniInfo.COMMON_BAITAI_5 = CASTCommon.GetFSKJIni("COMMON", "BAITAI_5")
            If IniInfo.COMMON_BAITAI_5.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "フォルダ（媒体５）", "COMMON", "BAITAI_5"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:フォルダ（媒体５） 分類:COMMON 項目:BAITAI_5")
                Return False
            End If

            IniInfo.COMMON_BAITAIREAD = CASTCommon.GetFSKJIni("COMMON", "BAITAIREAD")
            If IniInfo.COMMON_BAITAIREAD.ToUpper = "ERR" OrElse IniInfo.COMMON_BAITAIREAD = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "読込データ格納フォルダ", "COMMON", "BAITAIREAD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:読込データ格納フォルダ 分類:COMMON 項目:BAITAIREAD")
                Return False
            End If

            '2016/02/05 タスク）斎藤 RSV2対応 ADD ---------------------------------------- START
            Me.IniInfo.COMMON_FTR = CASTCommon.GetFSKJIni("COMMON", "FTR")
            If IniInfo.COMMON_FTR.ToUpper = "ERR" OrElse IniInfo.COMMON_FTR = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "FTRフォルダ", "COMMON", "FTR"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:FTRフォルダ 分類:COMMON 項目:FTR")
                Return False
            End If

            Me.IniInfo.COMMON_FTRANP = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
            If IniInfo.COMMON_FTRANP.ToUpper = "ERR" OrElse IniInfo.COMMON_FTRANP = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "FTRANPフォルダ", "COMMON", "FTRANP"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:FTRANPフォルダ 分類:COMMON 項目:FTRANP")
                Return False
            End If
            '2016/02/05 タスク）斎藤 RSV2対応 ADD ---------------------------------------- END

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "終了", "")
        End Try
    End Function

    '================================
    ' 取引先マスタチェック
    '================================
    Private Function CheckTorimast(ByVal TorisCode As String, ByVal TorifCode As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタチェック", "開始", "")

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            If rdbKigyo.Checked = True Then
                SQL.Append("     TORIMAST")
            Else
                SQL.Append("     S_TORIMAST")
            End If
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T   = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_T   = '" & TorifCode & "'")
            SQL.Append(" AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタチェック", "失敗", "該当なし")
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタチェック", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタチェック", "終了", "")
        End Try

    End Function

    '================================
    ' 委託者コード取得
    '================================
    Private Function GetItakuInfo(ByVal TorisCode As String, ByVal TorifCode As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者情報取得", "開始", "")

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            If rdbKigyo.Checked = True Then
                SQL.Append("     TORIMAST")
            Else
                SQL.Append("     S_TORIMAST")
            End If
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & TorifCode & "'")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                lblItakuCode.Text = GCom.NzStr(OraReader.Reader.Item("ITAKU_CODE_T"))
                lblItakuName.Text = GCom.NzStr(OraReader.Reader.Item("ITAKU_NNAME_T"))
                lblCodeKbn.Text = GetTextFileInfo(IniInfo.COMMON_TXT & "Common_コード区分.TXT", GCom.NzStr(OraReader.Reader.Item("CODE_KBN_T")))
                lblFileName.Text = GCom.NzStr(OraReader.Reader.Item("FILE_NAME_T"))
                If rdbKigyo.Checked = True Then
                    lblBaitai.Text = GetTextFileInfo(IniInfo.COMMON_TXT & "Common_媒体コード.TXT", GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T")))
                Else
                    lblBaitai.Text = GetTextFileInfo(IniInfo.COMMON_TXT & "Common_総振_媒体コード.TXT", GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T")))
                End If
                lblBaitaiCode.Text = GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T"))
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者情報取得", "成功", "")
            Else
                lblItakuCode.Text = ""
                lblItakuName.Text = ""
                lblCodeKbn.Text = ""
                lblFileName.Text = ""
                lblBaitai.Text = ""
                lblBaitaiCode.Text = ""
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者情報取得", "成功", "該当なし")
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者情報取得", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者情報取得", "終了", "")
        End Try
    End Function

    '================================
    ' 画面初期化
    '================================
    Private Function ClearInfo() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "画面初期化", "開始", "")

            '--------------------------------
            ' ラジオボタン初期化
            '--------------------------------
            rdbKigyo.Checked = True

            '--------------------------------
            ' コンボボックス初期化
            '--------------------------------
            cmbKana.SelectedItem = ""
            Dim Jyoken As String = " AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')"
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            '--------------------------------
            ' テキストボックス初期化
            '--------------------------------
            txtTorisCode.Text = ""
            txtTorifCode.Text = ""
            lblItakuCode.Text = ""
            lblItakuName.Text = ""
            lblCodeKbn.Text = ""
            lblFileName.Text = ""
            lblBaitai.Text = ""
            lblBaitaiCode.Text = ""

            '--------------------------------
            ' 変数初期化
            '--------------------------------
            SearchFlg = False

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "画面初期化", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "画面初期化", "終了", "")
        End Try

    End Function

    '================================
    ' ファイル名構築
    '================================
    Private Function MakeFileName(ByRef FileName As String, ByVal FuriDate As String) As Boolean

        Dim TimeStamp As String

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ファイル名構築", "開始", "")

            TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff")

            '--------------------------------
            ' 業務
            '--------------------------------
            If mArgumentData.INFOToriMast.FSYORI_KBN_T = "1" Then
                FileName = "J_"
            Else
                FileName = "S_"
            End If

            '--------------------------------
            ' 媒体
            '--------------------------------
            FileName &= GetTextFileInfo(IniInfo.COMMON_TXT & "媒体命名規約.txt", mArgumentData.INFOToriMast.BAITAI_CODE_T) & "_"

            '--------------------------------
            ' マルチ区分
            '--------------------------------
            If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                FileName &= "S_" & mArgumentData.INFOToriMast.TORIS_CODE_T & mArgumentData.INFOToriMast.TORIF_CODE_T & "_"
            Else
                FileName &= "M_" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & "00" & "_"
            End If

            '--------------------------------
            ' フォーマット区分
            '--------------------------------
            FileName &= mArgumentData.INFOToriMast.FMT_KBN_T & "_"

            '--------------------------------
            ' 指定日(振替日または振込日)
            '--------------------------------
            FileName &= FuriDate & "_"

            '--------------------------------
            ' 処理日時
            '--------------------------------
            FileName &= TimeStamp & "_"

            '--------------------------------
            ' プロセスＩＤ
            '--------------------------------
            FileName &= Format(Process.GetCurrentProcess.Id, "0000") & "_"

            '--------------------------------
            ' 通番 + 拡張子
            '--------------------------------
            FileName &= "000" & ".DAT"

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ファイル名構築", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ファイル名構築", "終了", "")
        End Try

    End Function

    '================================
    ' テキストファイル情報取得
    '================================
    Private Function GetTextFileInfo(ByVal TextFileName As String, ByVal KeyInfo As String) As String

        Dim sr As StreamReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストファイル読込", "開始", TextFileName)

            sr = New StreamReader(TextFileName, Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLineData() As String = sr.ReadLine().Split(","c)
                If strLineData(0) = KeyInfo Then
                    Return strLineData(1).Trim
                End If
            End While

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストファイル読込", "", "該当なし")
            Return "NON"

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストファイル読込", "失敗", ex.Message)
            Return "ERR"
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストファイル読込", "終了", "")
        End Try

    End Function

    '================================
    ' データ情報取得
    '================================
    Private Function GetDataInfo(ByVal aReadFMT As CAstFormat.CFormat, ByRef InfoParam As CommData.stPARAMETER, ByVal FileName As String, ByRef FuriDate As String, ByRef Message As String) As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ情報取得", "開始", "")

            '--------------------------------
            ' ファイルオープン
            '--------------------------------
            If aReadFMT.FirstRead(FileName) = 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ情報取得", "ファイルオープン失敗", aReadFMT.Message)
                Message = "エラー内容：中間ファイルオープン失敗"
                Return False
            End If

            '--------------------------------
            ' ファイル読込
            '--------------------------------
            Dim nRecordCount As Integer = 0                         'ファイル全体のレコードカウント
            '2016/02/08 タスク）岩城 RSV2対応 UPD ---------------------------------------- START
            Dim UketukeNoDisp As String = ""
            Dim UketukeNoDispFlg As Boolean = True
            'Dim nRecordNumber As Integer = 0                        'ヘッダ単位のレコードカウント
            '2016/02/08 タスク）岩城 RSV2対応 UPD ---------------------------------------- END
            Dim EndFlag As Boolean = False                          'エンドレコード存在フラグ
            Dim SplitData As String = ""
            Dim TorisCode_Header As String = ""
            Dim TorifCode_Header As String = ""
            Do Until aReadFMT.EOF
                Dim sCheckRet As String = ""
                nRecordCount += 1
                '2016/02/08 タスク）岩城 RSV2対応 DEL ---------------------------------------- START
                'nRecordNumber += 1
                '2016/02/08 タスク）岩城 RSV2対応 DEL ---------------------------------------- END

                '--------------------------------
                ' フォーマットチェック
                '--------------------------------
                Try
                    sCheckRet = aReadFMT.CheckDataFormat()
                Catch ex As Exception
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ情報取得", "フォーマットチェック", ex.Message)
                    Message = "エラー内容：" & nRecordCount.ToString & "行目　フォーマットチェック失敗"
                    Return False
                End Try

                Select Case sCheckRet
                    Case "ERR"
                        Dim nPos As Long
                        If aReadFMT.RecordData.Length > 0 Then nPos = aReadFMT.CheckRegularString
                        If nPos > 0 Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ情報取得", "フォーマットエラー", nRecordCount.ToString & "行目，" & (nPos + 1).ToString & "バイト目 " & aReadFMT.Message)
                            Message = "エラー内容：" & nRecordCount.ToString & "行目，" & (nPos + 1).ToString & "バイト目 フォーマットエラー" & vbCrLf & _
                                      aReadFMT.Message
                            Return False
                        Else
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ情報取得", "フォーマットエラー", nRecordCount.ToString & "行目 " & aReadFMT.Message)
                            Message = "エラー内容：" & nRecordCount.ToString & "行目　フォーマットエラー" & vbCrLf & _
                                      aReadFMT.Message
                            Return False
                        End If

                    Case "IJO"

                    Case "H"
                        EndFlag = False
                    Case "E"
                        EndFlag = True
                    Case "99"
                        EndFlag = True
                    Case "1A"
                        EndFlag = True
                    Case ""
                        Exit Do
                End Select

                'ヘッダレコード
                If aReadFMT.IsHeaderRecord() = True Then
                    '2016/02/08 タスク）岩城 RSV2対応 UPD ---------------------------------------- START
                    If UketukeNoDisp = "" And UketukeNoDispFlg = True Then
                        UketukeNoDisp = nRecordNumber.ToString
                        UketukeNoDispFlg = False
                    Else
                        UketukeNoDisp = ""
                    End If
                    'nRecordNumber = 1
                    '2016/02/08 タスク）岩城 RSV2対応 UPD ---------------------------------------- END

                    If CheckItakuCode(aReadFMT.InfoMeisaiMast.ITAKU_CODE, aReadFMT.InfoMeisaiMast.SYUBETU_CODE, mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T, TorisCode_Header, TorifCode_Header, Message, nRecordCount) = False Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ情報取得", "失敗", "ファイル内委託者コードチェック失敗 委託者コード:" & aReadFMT.InfoMeisaiMast.ITAKU_CODE & _
                                                                                                                                        " / 代表委託者コード:" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T)
                        Return False
                    End If

                    FuriDate = aReadFMT.InfoMeisaiMast.FURIKAE_DATE
                    '2016/02/08 タスク）岩城 RSV2対応 UPD ---------------------------------------- START
                    SplitData = UketukeNoDisp & "/" & _
                                aReadFMT.InfoMeisaiMast.ITAKU_CODE & "/" & _
                                aReadFMT.InfoMeisaiMast.ITAKU_KNAME.Trim & "/" & _
                                aReadFMT.InfoMeisaiMast.FURIKAE_DATE & "/" & _
                                aReadFMT.InfoMeisaiMast.SYUBETU_CODE & "/"
                    'SplitData = nRecordNumber & "/" & _
                    '            aReadFMT.InfoMeisaiMast.ITAKU_CODE & "/" & _
                    '            aReadFMT.InfoMeisaiMast.ITAKU_KNAME.Trim & "/" & _
                    '            aReadFMT.InfoMeisaiMast.FURIKAE_DATE & "/" & _
                    '            aReadFMT.InfoMeisaiMast.SYUBETU_CODE & "/"
                    '2016/02/08 タスク）岩城 RSV2対応 UPD ---------------------------------------- END
                End If

                ' トレーラーレコード
                If aReadFMT.IsTrailerRecord Then
                    If aReadFMT.EOF = True AndAlso EndFlag = False Then
                        If aReadFMT.ToriData.INFOParameter.FMT_KBN <> "TO" Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ情報取得", "フォーマットエラー", (nRecordCount + 1).ToString & "行目 エンドレコードがありません")
                            Message = "エラー内容：エンドレコードなし"
                            Return False
                        End If
                    End If

                    Dim Bikou As String = ""
                    Select Case rdbKigyo.Checked
                        Case True
                            If CheckSchmast(TorisCode_Header, TorifCode_Header, FuriDate) = False Then
                                Bikou = "ｽｹｼﾞｭｰﾙなし"
                            End If
                    End Select

                    SplitData &= Format(CInt(aReadFMT.InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO), "###,##0") & "/" & _
                                 Format(CLng(aReadFMT.InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO), "###,###,###,##0") & "/" & _
                                 TorisCode_Header & "-" & TorifCode_Header & "/" & _
                                 Bikou
                    ListViewArray.Add(SplitData)

                End If
            Loop

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ情報取得", "成功", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ情報取得", "失敗", ex.Message)
            Message = ""
            Return False
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "データ情報取得", "終了", "")
        End Try

    End Function

    '================================
    ' 委託者コードチェック
    '================================
    Private Function CheckItakuCode(ByVal FileItakuCode As String, ByVal FileSyubetu As String, ByVal DispItakuKanriCode As String, ByRef TorisCode As String, ByRef TorifCode As String, ByRef RetMessage As String, ByVal RecCount As Integer) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者コードチェック", "開始", "ファイル内委託者コード:" & FileItakuCode & " / 画面入力代表委託者コード:" & DispItakuKanriCode)

            TorisCode = ""
            TorifCode = ""

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            If rdbKigyo.Checked = True Then
                SQL.Append("     TORIMAST")
                SQL.Append(" WHERE")
                SQL.Append("     ITAKU_CODE_T = '" & FileItakuCode & "'")
                SQL.Append(" AND SYUBETU_T    = '" & FileSyubetu & "'")
            Else
                SQL.Append("     S_TORIMAST")
                SQL.Append(" WHERE")
                SQL.Append("     ITAKU_CODE_T = '" & FileItakuCode & "'")
                SQL.Append(" AND SYUBETU_T    = '" & FileSyubetu & "'")
            End If

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then

                If GCom.NzStr(OraReader.Reader.Item("ITAKU_KANRI_CODE_T")) = DispItakuKanriCode Then
                    TorisCode = GCom.NzStr(OraReader.Reader.Item("TORIS_CODE_T"))
                    TorifCode = GCom.NzStr(OraReader.Reader.Item("TORIF_CODE_T"))
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者コードチェック", "成功", "取引先コード:" & TorisCode & "-" & TorifCode)
                Else
                    RetMessage = "エラー内容：" & RecCount.ToString & "行目　ヘッダレコードの委託者コード検索失敗(代表委託者コード不一致)"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者コードチェック", "失敗", "代表委託者コード不一致")
                    Return False
                End If
            Else
                RetMessage = "エラー内容：" & RecCount.ToString & "行目　ヘッダレコードの委託者コード検索失敗(取引先該当なし)"
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者コードチェック", "成功", "取引先マスタ該当なし")
                Return False
            End If

            Return True

        Catch ex As Exception
            RetMessage = "エラー内容：" & RecCount.ToString & "行目　ヘッダレコードの委託者コード検索失敗"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者コードチェック", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者コードチェック", "終了", "")
        End Try
    End Function

    Private Function CheckSchmast(ByVal TorisCode As String, ByVal TorifCode As String, ByVal FuriDate As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタチェック", "開始", "取引先コード:" & TorisCode & "-" & TorifCode & " / 振替日:" & FuriDate)

            SQL = New StringBuilder(128)
            SQL.Append("SELECT")
            SQL.Append("     FURI_DATE_S")
            SQL.Append(" FROM")
            SQL.Append("     SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_S = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_S = '" & TorifCode & "'")
            SQL.Append(" AND FURI_DATE_S  = '" & FuriDate & "'")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタチェック", "成功", "スケジュールなし")
                Return False
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタチェック", "成功", "スケジュールあり")
            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタチェック", "失敗", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタチェック", "終了", "")
        End Try
    End Function

    '2016/02/05 タスク）斎藤 RSV2対応 ADD ---------------------------------------- START
    '================================
    ' コード変換情報取得
    '================================
    Private Function GetPFileInfo(ByVal FmtKbn As String, _
                                  ByVal CodeKbn As String, _
                                  ByRef FtranP As String) As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "コード変換情報取得", "開始", "")

            Select Case FmtKbn
                Case "00"
                    Select Case CodeKbn
                        Case "0" : FtranP = "120JIS→JIS.P"
                        Case "1" : FtranP = "120JIS→JIS改.P"
                        Case "2" : FtranP = "119JIS→JIS改.P"
                        Case "3" : FtranP = "118JIS→JIS改.P"
                        Case "4" : FtranP = "120.P"
                    End Select
                Case "01"
                    Select Case CodeKbn
                        Case "0" : FtranP = "120JIS→JIS.P"
                        Case "1" : FtranP = "120JIS→JIS改.P"
                        Case "4" : FtranP = "120.P"
                    End Select
                Case "02"
                    Select Case CodeKbn
                        Case "0" : FtranP = "390JIS→JIS.P"
                        Case "1" : FtranP = "390JIS→JIS改.P"
                        Case "4" : FtranP = "390.P"
                    End Select
                Case Else
                    If IsNumeric(FmtKbn) Then
                        Dim nFmtKbn As Integer = CInt(FmtKbn)
                        'フォーマット区分が50～99の場合
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            ' XMLフォーマットのrootオブジェクト生成
                            Dim xmlDoc As New ConfigXmlDocument
                            Dim mXmlRoot As XmlElement
                            Dim node As XmlNode
                            Dim attribute As XmlAttribute

                            'XMLパス作成
                            Dim xmlFolderPath As String = CASTCommon.GetFSKJIni("COMMON", "XML_FORMAT_FLD")
                            If xmlFolderPath = "err" Or xmlFolderPath = "" Then
                                Throw New Exception("fskj.iniでXML_FORMAT_FLDが定義されていません。")
                            End If
                            If xmlFolderPath.EndsWith("\") = False Then
                                xmlFolderPath &= "\"
                            End If
                            Dim mXmlFile As String = "XML_FORMAT_" & FmtKbn & ".xml"

                            xmlDoc.Load(xmlFolderPath & mXmlFile)
                            mXmlRoot = xmlDoc.DocumentElement

                            ' 返還ファイルを伝送ファイルにコピーする際のパラメータファイル
                            node = mXmlRoot.SelectSingleNode("返還/コピー設定一覧/コピー設定[@コード区分='" & CodeKbn & "']")
                            If node Is Nothing Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定[@コード区分='" & CodeKbn & "']」タグが定義されていません。")
                            End If

                            attribute = node.Attributes.ItemOf("パラメータファイル")
                            If attribute Is Nothing OrElse attribute.Value.Trim = "" Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「パラメータファイル」属性が定義されていません。（" & _
                                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                            End If
                            FtranP = attribute.Value.Trim

                        End If
                    End If
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "コード変換情報取得", "成功", _
                                 "FtranP" & FtranP)
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "コード変換情報取得", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "コード変換情報取得", "終了", "")
        End Try
    End Function

    '================================
    ' FTRANPコード変換
    '================================
    Private Function ConvertFileFtranP(ByVal strGetOrPut As String, _
                                           ByVal strInFileName As String, _
                                           ByVal strOutFileName As String, _
                                           ByVal strPFileName As String) As Integer
        Try
            '変換コマンド組み立て
            Dim Command As New StringBuilder
            With Command
                .Append(" /nwd/ cload ")
                .Append("""" & Me.IniInfo.COMMON_FTR & "FUSION" & """")
                .Append(" ; kanji 83_jis")
                .Append(" " & strGetOrPut & " ")
                .Append("""" & strInFileName & """" & " ")
                .Append("""" & strOutFileName & """" & " ")
                .Append(" ++" & """" & strPFileName & """")
            End With

            Dim Proc As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(Me.IniInfo.COMMON_FTRANP, "FP.EXE")
            ProcInfo.WorkingDirectory = Me.IniInfo.COMMON_FTRANP
            ProcInfo.Arguments = Command.ToString
            Proc = Process.Start(ProcInfo)
            Proc.WaitForExit()
            If Proc.ExitCode = 0 Then
                MainLOG.Write("(FTRANPコード変換)", "成功", "終了コード：" & Proc.ExitCode)
                Return 0
            Else
                MainLOG.Write("(FTRANPコード変換)", "失敗", "終了コード：" & Proc.ExitCode)
                Return 100
            End If
        Catch ex As Exception
            MainLOG.Write("(FTRANPコード変換)", "失敗", ex.Message)
            Return 100
        End Try
    End Function
    '2016/02/05 タスク）斎藤 RSV2対応 ADD ---------------------------------------- END

#End Region

#Region " 関数(Sub) "

    '================================
    ' テキストボックス0埋め
    '================================
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtTorisCode.Validating, txtTorifCode.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストボックス0埋め", "失敗", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

    '================================
    ' 取引先コード入力処理
    '================================
    Private Sub TextBox_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtTorisCode.Validated, txtTorifCode.Validated

        Try
            '--------------------------------
            ' ログ情報設定
            '--------------------------------
            If SetLogInfo(False) = False Then
                Exit Try
            End If

            '--------------------------------
            ' 委託者コード設定
            '--------------------------------
            If txtTorisCode.Text.Trim <> "" And txtTorifCode.Text.Trim <> "" Then
                If GetItakuInfo(txtTorisCode.Text, txtTorifCode.Text) = False Then
                    Exit Try
                End If
            Else
                lblItakuCode.Text = ""
                lblItakuName.Text = ""
                lblCodeKbn.Text = ""
                lblFileName.Text = ""
                lblBaitai.Text = ""
                lblBaitaiCode.Text = ""
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先コード入力処理", "失敗", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

    '================================
    ' リスト領域ソート
    '================================
    Private Sub SortListView(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles ListView1.ColumnClick

        Try

            With CType(sender, ListView)

                '--------------------------------
                ' 同列の場合，逆順
                '--------------------------------
                If ClickedColumn = e.Column Then
                    SortOrderFlag = Not SortOrderFlag
                End If

                '--------------------------------
                ' 列番号設定
                '--------------------------------
                ClickedColumn = e.Column

                '--------------------------------
                ' 列水平方向配置
                '--------------------------------
                Dim ColAlignment As HorizontalAlignment = .Columns(e.Column).TextAlign

                '--------------------------------
                ' ソート
                '--------------------------------
                .ListViewItemSorter = New CASTCommon.ListViewItemComparer(e.Column, SortOrderFlag, ColAlignment)
                .Sort()

            End With

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "リスト領域ソート", "失敗", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

    '================================
    ' 取引先(コンボボックス)を取得
    '================================
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '--------------------------------
                ' 追加条件設定
                '--------------------------------
                Dim Jyoken As String = " AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')"   '媒体コードが媒体のもの

                '--------------------------------
                ' 業務判定(企業自振 OR 総合振込)
                '--------------------------------
                Dim Gyoumu As String = ""
                If rdbKigyo.Checked = True Then
                    Gyoumu = "1"
                Else
                    Gyoumu = "3"
                End If

                '--------------------------------
                ' コンボボックス設定
                '--------------------------------
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, Gyoumu, Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            End If

            cmbToriName.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先コンボボックス設定", "失敗", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

    '================================
    ' 取引先(コンボボックス)選択処理
    '================================
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged

        Try
            '--------------------------------
            ' 取引先コード設定
            '--------------------------------
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
            End If

            '--------------------------------
            ' ログ情報設定
            '--------------------------------
            If SetLogInfo(False) = False Then
                Exit Try
            End If

            '--------------------------------
            ' 委託者コード設定
            '--------------------------------
            If txtTorisCode.Text.Trim <> "" And txtTorifCode.Text.Trim <> "" Then
                If GetItakuInfo(txtTorisCode.Text, txtTorifCode.Text) = False Then
                    Exit Try
                End If
            Else
                lblItakuCode.Text = ""
                lblItakuName.Text = ""
                lblCodeKbn.Text = ""
                lblFileName.Text = ""
                lblBaitai.Text = ""
                lblBaitaiCode.Text = ""
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先コード設定", "失敗", ex.Message)
        Finally
            ' NOP
        End Try

    End Sub

    '================================
    ' ラジオボタン変更
    '================================
    Private Sub rdb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbKigyo.CheckedChanged, rdbSofuri.CheckedChanged

        Try
            '--------------------------------
            ' 追加条件設定
            '--------------------------------
            Dim Jyoken As String = " AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')"   '媒体コードが媒体のもの

            '--------------------------------
            ' 業務判定(企業自振 OR 総合振込)
            '--------------------------------
            Dim Gyoumu As String = ""
            If rdbKigyo.Checked = True Then
                Gyoumu = "1"
            Else
                Gyoumu = "3"
            End If

            '--------------------------------
            ' コンボボックス設定
            '--------------------------------
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, Gyoumu, Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            cmbToriName.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ラジオボタン変更", "失敗", ex.Message)
        Finally
            ' NOP
        End Try
    End Sub

#End Region

End Class
' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END
