'Imports System.Data.OracleClient
Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJTAKO010
    Inherits System.Windows.Forms.Form
    Private MainLOG As New CASTCommon.BatchLOG("KFJTAKO010", "受信ファイル一括振分(元請)画面")
    Private Const msgTitle As String = "受信ファイル一括振分(元請)画面(KFJTAKO010)"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private DenFolder As String
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
#Region "宣言"
    Dim clsFUSION As New clsFUSION.clsMain()
    Dim KFJTAKO011 As New KFJTAKO011()
    'Dim KFJMAIN042 As New KFJMAIN042
    Dim strKANRI_FILE As String
    Dim strJYUSIN_DATE As String
    Dim strJYUSIN_FILE As String
    Dim strJYUSIN_FILE_JIS As String
    Dim strJYUSIN_FILE_BK As String
    Dim strFILE_DATE As String
    Dim strFILE_TIME As String
    Dim strFURI_DATE As String
    Dim strITAKU_CODE As String
    Dim strTORIS_CODE As String
    Dim strTORIF_CODE As String
    Dim strITAKU_NAME As String
    Dim strKEIYAKU_DATE As String ' 2006/06/20

    'Dim strKANRI_NO As String
    'Dim strFMT_KBN As String
    'Dim intRECORD_LEN As String
    'Dim strCODE_KBN As String
    'Dim strMULTI_KBN As String
    'Dim strKIDOU_KBN As String
    'Dim strSOUJYUSIN As String
    'Dim strFILE_NAME As String
    'Dim strTITLE As String
    'Dim strSENTER_NAME As String
    'Dim strSENTER_KIN As String

    Dim strIRAI_TORIS_CODE(100) As String
    Dim strIRAI_TORIF_CODE(100) As String
    Dim strIRAI_ITAKU_NAME(100) As String
    Dim strIRAI_FURI_DATE(100) As String
    Dim strIRAI_SENTAKU(100) As String
    Dim strKEKKA_TORIS_CODE(100) As String
    Dim strKEKKA_TORIF_CODE(100) As String
    Dim strKEKKA_ITAKU_NAME(100) As String
    Dim strKEKKA_FURI_DATE(100) As String
    Dim strKEKKA_SENTAKU(100) As String
    Dim strKEKKA_FILE(100) As String

    Dim strKOBETU_FILE As String
    Dim strDATA_KBN As String

    '他行マスタ検索用
    Private strTAKOU_KIN As String
    Private strTAKOU_ITAKU_CODE As String
    Private strTAKOU_ITAKU_KIN As String
    Private strTAKOU_ITAKU_SIT As String
    Private strTAKOU_ITAKU_KAMOKU As String
    Private strTAKOU_ITAKU_KOUZA As String
    Private strTAKOU_BAITAI_CODE As String
    Private strTAKOU_S_FILE_NAME As String
    Private strTAKOU_R_FILE_NAME As String
    Private strTAKOU_CODE_KBN As String
    Private strLABEL_CODE As String

    'フォーマット
    Private intREC_LENGTH As Integer
    Private intBLK_SIZE As Integer
    'ファイル名
    Private strIN_FILE_NAME As String
    Private strOUT_FILE_NAME As String

    ''明細マスタ更新用
    'Public gdbcFUNOU_CONNECT As New OracleClient.OracleConnection() 'CONNECTION
    'Public gdbrFUNOU_READER As OracleClient.OracleDataReader 'READER
    'Public gdbFUNOU_COMMAND As OracleClient.OracleCommand   'COMMAND関数
    'Public gdbFUNOU_TRANS As OracleClient.OracleTransaction 'TRANSACTION

    ''スケジュールマスタ更新用
    'Public gdbcSCH_CONNECT As New OracleClient.OracleConnection() 'CONNECTION
    'Public gdbrSCH_READER As OracleClient.OracleDataReader 'READER
    'Public gdbSCH_COMMAND As OracleClient.OracleCommand   'COMMAND関数
    'Public gdbSCH_TRANS As OracleClient.OracleTransaction 'TRANSACTION

    '更新キー項目
    Private strKIN_CODE As String
    Private strSIT_CODE As String
    Private strKAMOKU As String
    Private strKOUZA As String
    Private strKINGAKU As String
    Private strKEIYAKU_KNAME As String
    Private strJYUYOKA_NO As String
    Private strFURIKETU_CODE As String
    Private strMINASI As String
    Private intMINASI_COUNT As Integer
    Private strTEISEI_SIT As String
    Private strTEISEI_KAMOKU As String
    Private strTEISEI_KOUZA As String
    Private strTEISEI_AKAMOKU As String
    Private strTEISEI_AKOUZA As String
    Private dblKIGYO_RECORD_COUNT As Double

    Private DENBKFolder As String
    Private DATFolder As String
    Private DATBKFolder As String
    Private FTRFolder As String
    Private FTRANP_Folder As String
    Private TXTFolder As String
    Private TAKFolder As String
    Private JikinkoCd As String
    Private JikinkoName As String
    '2010/01/19 農協コード
    Private MatomeNoukyo As String
    Private Noukyo_From As String
    Private Noukyo_To As String
    '=============================

    '2014/05/21 saitou 標準版 ADD -------------------------------------------------->>>>
    'テキストファイルの内容を構造体で管理する
    Private Structure strcDenTextInfo
        Dim KANRI_NO As String
        Dim FMT_KBN As String
        Dim RECORD_LEN As String
        Dim CODE_KBN As String
        Dim MULTI_KBN As String
        Dim KIDOU_KBN As String
        Dim SOUJYUSIN As String
        Dim FILE_NAME As String
        Dim TITLE As String
        Dim CENTER_NAME As String
        Dim CENTER_KIN As String

        Public Sub Init()
            KANRI_NO = String.Empty
            FMT_KBN = String.Empty
            RECORD_LEN = String.Empty
            CODE_KBN = String.Empty
            MULTI_KBN = String.Empty
            KIDOU_KBN = String.Empty
            SOUJYUSIN = String.Empty
            FILE_NAME = String.Empty
            TITLE = String.Empty
            CENTER_NAME = String.Empty
            CENTER_KIN = String.Empty
        End Sub

    End Structure
    Private DEN_TEXT As strcDenTextInfo
    '2014/05/21 saitou 標準版 ADD --------------------------------------------------<<<<

#End Region

#Region " ロード"
    Private Sub KFJTAKO010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            Call GCom.SetMonitorTopArea(Label2, Label4, lblUser, lblDate)

            '------------------------------------
            'INIファイルの読み込み
            '------------------------------------
            If fn_INI_READ() = False Then
                Return
            End If

            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            txtJyusinDateY.Text = strSysDate.Substring(0, 4)
            txtJyusinDateM.Text = strSysDate.Substring(4, 2)
            txtJyusinDateD.Text = strSysDate.Substring(6, 2)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '受信ファイル名リストボックスにをファイル名表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strKANRI_FILE As String = Path.Combine(TXTFolder, "伝送ファイル管理.TXT")
            If Dir(strKANRI_FILE) = "" Then
                MessageBox.Show(String.Format(MSG0274W, "伝送ファイル管理.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Dim FileReader As New StreamReader(strKANRI_FILE, System.Text.Encoding.GetEncoding("Shift-JIS"))
            Dim Line() As String '0:管理番号 1:フォーマット区分 2:レコード長 3:コード区分 4:マルチ区分 5:起動区分 6:送受信 
            '                     7:ファイル名 8:タイトル 9:センター名 10:センター金融機関コード
            cmbFileName.Items.Clear()
            Do Until FileReader.EndOfStream
                Line = FileReader.ReadLine.Split(","c)
                If Line(6) = "R" Then   '受信

                    cmbFileName.Items.Add(Line(8))
                End If
            Loop
            FileReader.Close()
            cmbFileName.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show(ex.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region "読み込み"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(読込)開始", "成功", "")
            Dim SQL As New StringBuilder(128)
            MainDB = New MyOracle
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            strJYUSIN_DATE = txtJyusinDateY.Text & txtJyusinDateM.Text & txtJyusinDateD.Text

            '管理ファイルの読み込み
            Dim strKANRI_FILE As String = Path.Combine(TXTFolder, "伝送ファイル管理.TXT")
            If Dir(strKANRI_FILE) = "" Then
                MessageBox.Show(String.Format(MSG0274W, "伝送ファイル管理.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Dim FileReader As New StreamReader(strKANRI_FILE, System.Text.Encoding.GetEncoding("Shift-JIS"))
            Dim Line() As String '0:管理番号 1:フォーマット区分 2:レコード長 3:コード区分 4:マルチ区分 5:起動区分 6:送受信 
            '                     7:ファイル名 8:タイトル 9:センター名 10:センター金融機関コード
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            Do Until FileReader.EndOfStream
                Line = FileReader.ReadLine.Split(","c)
                Me.DEN_TEXT.KANRI_NO = Line(0)
                Me.DEN_TEXT.FMT_KBN = Line(1)
                Me.DEN_TEXT.RECORD_LEN = Line(2)
                Me.DEN_TEXT.CODE_KBN = Line(3)
                Me.DEN_TEXT.MULTI_KBN = Line(4)
                Me.DEN_TEXT.KIDOU_KBN = Line(5)
                Me.DEN_TEXT.SOUJYUSIN = Line(6)
                Me.DEN_TEXT.FILE_NAME = Line(7).Trim
                Me.DEN_TEXT.TITLE = Line(8)
                Me.DEN_TEXT.CENTER_NAME = Line(9)
                Me.DEN_TEXT.CENTER_KIN = Line(10)
                If Me.DEN_TEXT.SOUJYUSIN = "R" Then  '受信
                    If Me.DEN_TEXT.TITLE = cmbFileName.SelectedItem Then
                        Exit Do
                    End If
                End If
            Loop
            FileReader.Close()
            strJYUSIN_FILE = Path.Combine(DenFolder, Me.DEN_TEXT.FILE_NAME)
            strJYUSIN_FILE_JIS = Path.Combine(DATFolder, Me.DEN_TEXT.FILE_NAME)
            strJYUSIN_FILE_BK = Path.Combine(DENBKFolder, Me.DEN_TEXT.FILE_NAME)

            'Do Until FileReader.EndOfStream
            '    Line = FileReader.ReadLine.Split(","c)
            '    strKANRI_NO = Line(0)
            '    strFMT_KBN = Line(1)
            '    intRECORD_LEN = Line(2)
            '    strCODE_KBN = Line(3)
            '    strMULTI_KBN = Line(4)
            '    strKIDOU_KBN = Line(5)
            '    strSOUJYUSIN = Line(6)
            '    strFILE_NAME = Line(7).Trim
            '    strTITLE = Line(8)
            '    strSENTER_NAME = Line(9)
            '    strSENTER_KIN = Line(10)
            '    If strSOUJYUSIN = "R" Then  '受信
            '        If strTITLE = cmbFileName.SelectedItem Then
            '            Exit Do
            '        End If
            '    End If
            'Loop
            'FileReader.Close()
            'strJYUSIN_FILE = DenFolder & strFILE_NAME
            'strJYUSIN_FILE_JIS = DATFolder & strFILE_NAME
            'strJYUSIN_FILE_BK = DENBKFolder & strFILE_NAME
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

            If fn_YOMIKOMI_MAIN() = False Then
                Exit Sub
            End If

            '--------------------------------
            '受信ファイルのバックアップ
            '--------------------------------
            If Dir(strJYUSIN_FILE_BK) <> "" Then
                Kill(strJYUSIN_FILE_BK)
            End If
            FileCopy(strJYUSIN_FILE, strJYUSIN_FILE_BK)
            Kill(strJYUSIN_FILE)
            If Err.Number <> 0 Then
                MessageBox.Show(MSG0269W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(読込)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(読込)終了", "成功", "")
        End Try

    End Sub
#End Region
#Region "再読み込み"
    Private Sub btnReAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReAction.Click
        Dim SQL As New StringBuilder(128)
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(再読込)開始", "成功", "")
            MainDB = New MyOracle
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            strJYUSIN_DATE = txtJyusinDateY.Text & txtJyusinDateM.Text & txtJyusinDateD.Text

            '管理ファイルの読み込み
            Dim strKANRI_FILE As String = Path.Combine(TXTFolder, "伝送ファイル管理.TXT")
            If Dir(strKANRI_FILE) = "" Then
                MessageBox.Show(String.Format(MSG0274W, "伝送ファイル管理.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Dim FileReader As New StreamReader(strKANRI_FILE, System.Text.Encoding.GetEncoding("Shift-JIS"))
            Dim Line() As String '0:管理番号 1:フォーマット区分 2:レコード長 3:コード区分 4:マルチ区分 5:起動区分 6:送受信 
            '                     7:ファイル名 8:タイトル 9:センター名 10:センター金融機関コード
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            Do Until FileReader.EndOfStream
                Line = FileReader.ReadLine.Split(","c)
                Me.DEN_TEXT.KANRI_NO = Line(0)
                Me.DEN_TEXT.FMT_KBN = Line(1)
                Me.DEN_TEXT.RECORD_LEN = Line(2)
                Me.DEN_TEXT.CODE_KBN = Line(3)
                Me.DEN_TEXT.MULTI_KBN = Line(4)
                Me.DEN_TEXT.KIDOU_KBN = Line(5)
                Me.DEN_TEXT.SOUJYUSIN = Line(6)
                Me.DEN_TEXT.FILE_NAME = Line(7).Trim
                Me.DEN_TEXT.TITLE = Line(8)
                Me.DEN_TEXT.CENTER_NAME = Line(9)
                Me.DEN_TEXT.CENTER_KIN = Line(10)
                If Me.DEN_TEXT.SOUJYUSIN = "R" Then  '受信
                    If Me.DEN_TEXT.TITLE = cmbFileName.SelectedItem Then
                        Exit Do
                    End If
                End If
            Loop
            FileReader.Close()
            strJYUSIN_FILE = Path.Combine(DENBKFolder, Me.DEN_TEXT.FILE_NAME)
            strJYUSIN_FILE_JIS = Path.Combine(DATFolder, Me.DEN_TEXT.FILE_NAME)

            'Do Until FileReader.EndOfStream
            '    Line = FileReader.ReadLine.Split(","c)
            '    strKANRI_NO = Line(0)
            '    strFMT_KBN = Line(1)
            '    intRECORD_LEN = Line(2)
            '    strCODE_KBN = Line(3)
            '    strMULTI_KBN = Line(4)
            '    strKIDOU_KBN = Line(5)
            '    strSOUJYUSIN = Line(6)
            '    strFILE_NAME = Line(7).Trim
            '    strTITLE = Line(8)
            '    strSENTER_NAME = Line(9)
            '    strSENTER_KIN = Line(10)
            '    If strSOUJYUSIN = "R" Then  '受信
            '        If strTITLE = cmbFileName.SelectedItem Then
            '            Exit Do
            '        End If
            '    End If
            'Loop
            'strJYUSIN_FILE = DENBKFolder & strFILE_NAME
            'strJYUSIN_FILE_JIS = DATFolder & strFILE_NAME
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

            If fn_YOMIKOMI_MAIN() = False Then
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(再読込)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(再読込)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region "個別登録"
    Private Sub btnTouroku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTouroku.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(個別登録)開始", "成功", "")
            Dim I As Integer = 0
            Dim Counter As Integer = 0
            For I = 1 To chlIrai.Items.Count
                If chlIrai.GetItemChecked(I - 1) = True Then
                    Counter += 1
                End If
            Next
            'チェックがついた項目がない場合は終了
            If Counter = 0 Then
                MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            '更新前確認メッセージ
            If MessageBox.Show(String.Format(MSG0023I, "落し込み処理"), msgTitle, _
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            MainDB = New MyOracle
            MainDB.BeginTrans()
            For I = 1 To chlIrai.Items.Count
                If chlIrai.GetItemChecked(I - 1) = True Then   'チェックが入っていれば落し込み処理する
                    '------------------------------------------------
                    'ジョブマスタに登録
                    '------------------------------------------------
                    Dim okFlg As Boolean = True
                    Dim jobid As String
                    jobid = "J010"      '落とし込み
                    Dim para As String
                    '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
                    'テキストファイルの内容を構造体で管理
                    para = strIRAI_TORIS_CODE(I) & strIRAI_TORIF_CODE(I) & "," & strIRAI_FURI_DATE(I) & "," & "0" & "," & Me.DEN_TEXT.FMT_KBN _
                                    & "," & "0" & "," & "0"
                    'para = strIRAI_TORIS_CODE(I) & strIRAI_TORIF_CODE(I) & "," & strIRAI_FURI_DATE(I) & "," & "0" & "," & strFMT_KBN _
                    '                & "," & "0" & "," & "0"
                    '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
                    '#########################
                    'job検索
                    '#########################
                    Dim iRet As Integer
                    iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                    If iRet = 1 Then
                        MainDB.Rollback()
                        MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        okFlg = False
                    ElseIf iRet = -1 Then 'ジョブ検索失敗
                        MainDB.Rollback()
                        MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブマスタ検索失敗 パラメータ:" & para)
                        okFlg = False
                    End If

                    If okFlg Then
                        '#########################
                        'job登録
                        '#########################
                        If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then 'ジョブ登録失敗
                            MainDB.Rollback()
                            MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブ登録失敗 パラメータ:" & para)
                        Else
                            chlIrai.SetItemChecked(I - 1, False)
                        End If
                    End If
                End If
            Next I
            MainDB.Commit()
            MessageBox.Show(String.Format(MSG0021I, "落し込み処理"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(ex.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(個別登録)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(個別登録)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region "不能結果更新"
    Private Sub btnKousin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKousin.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(不能結果更新)開始", "成功", "")
            Dim I As Integer = 0
            Dim Counter As Integer = 0
            For I = 1 To chlKekka.Items.Count
                If chlKekka.GetItemChecked(I - 1) = True Then
                    Counter += 1
                End If
            Next
            'チェックがついた項目がない場合は終了
            If Counter = 0 Then
                MessageBox.Show(MSG0100W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            '更新前確認メッセージ
            If MessageBox.Show(String.Format(MSG0023I, "結果更新処理"), msgTitle, _
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            MainDB = New MyOracle
            MainDB.BeginTrans()
            Dim Orareader As New MyOracleReader(MainDB)
            Dim SQL As StringBuilder
            For I = 1 To chlKekka.Items.Count
                If chlKekka.GetItemChecked(I - 1) = True Then    'チェックが入っていれば落し込み処理する
                    strTORIS_CODE = strKEKKA_TORIS_CODE(I)
                    strTORIF_CODE = strKEKKA_TORIF_CODE(I)
                    '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
                    'テキストファイルの内容を構造体で管理
                    strTAKOU_KIN = Me.DEN_TEXT.CENTER_KIN
                    'strTAKOU_KIN = strSENTER_KIN
                    '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
                    '-------------------------------------
                    '他行マスタ検索
                    '-------------------------------------
                    If fn_SELECT_TAKOUMAST(strTORIS_CODE, strTORIF_CODE, strTAKOU_KIN) = False Then
                        '他行マスタ未登録
                        MessageBox.Show(MSG0070W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If

                    '-------------------------------------
                    '他行スケジュールマスタ検索
                    '-------------------------------------
                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT * FROM TAKOSCHMAST")
                    SQL.Append(" WHERE TORIS_CODE_U = " & SQ(strTORIS_CODE))
                    SQL.Append(" AND TORIF_CODE_U = " & SQ(strTORIF_CODE))
                    SQL.Append(" AND FURI_DATE_U = " & SQ(strKEKKA_FURI_DATE(I)))
                    '地域G:2010/01/14 nakamura 農協対応
                    'SQL.Append(" AND TKIN_NO_U = " & SQ(strTAKOU_KIN))
                    'If Trim(strTAKOU_KIN) = "5900" Then
                    '    SQL.Append(" AND TKIN_NO_U >= '5874' AND TKIN_NO_U <= '5939' ")    'iniファイル対応
                    If Trim(strTAKOU_KIN) = MatomeNoukyo Then
                        SQL.Append(" AND TKIN_NO_U BETWEEN " & SQ(Noukyo_From) & " AND " & SQ(Noukyo_To))
                    Else
                        SQL.Append(" AND TKIN_NO_U = " & SQ(strTAKOU_KIN))
                    End If


                    '読込のみ
                    If Orareader.DataReader(SQL) = False Then
                        MessageBox.Show(MSG0271W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Orareader.Close()
                        Exit Sub
                    End If
                    Orareader.Close()
                    '----------------------------------------------
                    '不能ジョブ登録
                    '----------------------------------------------
                    Dim okFlg As Boolean = True
                    Dim jobid As String
                    jobid = "J040"      '不能結果更新
                    Dim para As String
                    '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
                    'テキストファイルの内容を構造体で管理
                    para = strKEKKA_FURI_DATE(I) & ",2,0," & strTORIS_CODE & strTORIF_CODE & "," & Me.DEN_TEXT.CENTER_KIN & ",0"
                    'para = strKEKKA_FURI_DATE(I) & ",2,0," & strTORIS_CODE & strTORIF_CODE & "," & strSENTER_KIN & ",0"
                    '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
                    '#########################
                    'job検索
                    '#########################
                    Dim iRet As Integer
                    iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                    If iRet = 1 Then    'ジョブ登録済
                        MainDB.Rollback()
                        MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        okFlg = False
                    ElseIf iRet = -1 Then 'ジョブ検索失敗
                        MainDB.Rollback()
                        MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブマスタ検索失敗 パラメータ:" & para)
                        okFlg = False
                    End If

                    If okFlg Then
                        '#########################
                        'job登録
                        '#########################
                        If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then 'ジョブ登録失敗
                            MainDB.Rollback()
                            MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブ登録失敗 パラメータ:" & para)
                        Else
                            chlKekka.SetItemChecked(I - 1, False)
                        End If
                    End If
                End If
            Next I
            MainDB.Commit()
            MessageBox.Show(String.Format(MSG0021I, "結果更新処理"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(不能結果更新)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(不能結果更新)終了", "成功", "")
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
    Private Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/18
        'Update         :
        '============================================================================
        Try
            fn_check_text = False
            '年必須チェック
            If txtJyusinDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateY.Focus()
                Return False
            End If
            '月必須チェック
            If txtJyusinDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateM.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(txtJyusinDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtJyusinDateM.Text.Trim) > 12 Then '(MSG0022W)
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateM.Focus()
                Return False
            End If
            '日付必須チェック
            If txtJyusinDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateD.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(txtJyusinDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtJyusinDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtJyusinDateY.Text & "/" & txtJyusinDateM.Text & "/" & txtJyusinDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtJyusinDateY.Focus()
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try
        fn_check_text = True
    End Function
    Private Function fn_INI_READ() As Boolean
        '============================================================================
        'NAME           :fn_INI_READ
        'Parameter      :
        'Description    :FSKJ.INIファイルの読み込み
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/09/27
        'Update         :
        '============================================================================
        fn_INI_READ = False
        Try
            'DEN格納フォルダ
            DenFolder = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If DenFolder.ToUpper = "ERR" OrElse DenFolder = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "DEN格納フォルダ", "COMMON", "DEN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名: 分類:COMMON 項目:DEN")
                Return False
            End If

            'DENバックアップ格納フォルダ
            DENBKFolder = CASTCommon.GetFSKJIni("COMMON", "DENBK")
            If DENBKFolder.ToUpper = "ERR" OrElse DENBKFolder = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "DENバックアップ格納フォルダ", "COMMON", "DENBK"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:DENバックアップ格納フォルダ 分類:COMMON 項目:DENBK")
                Return False
            End If


            'DAT格納フォルダ
            DATFolder = CASTCommon.GetFSKJIni("COMMON", "DAT")
            If DATFolder.ToUpper = "ERR" OrElse DATFolder = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "DAT格納フォルダ", "COMMON", "DAT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:DAT格納フォルダ 分類:COMMON 項目:DAT")
                Return False
            End If

            'DATバックアップ格納フォルダ
            DATBKFolder = CASTCommon.GetFSKJIni("COMMON", "DATBK")

            If DATFolder.ToUpper = "ERR" OrElse DATFolder = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "DATバックアップ格納フォルダ", "COMMON", "DATBK"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:DATバックアップ格納フォルダ 分類:COMMON 項目:DATBK")
                Return False
            End If


            'Pファイル格納フォルダ
            FTRFolder = CASTCommon.GetFSKJIni("COMMON", "FTR")
            If FTRFolder.ToUpper = "ERR" OrElse FTRFolder = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "Pファイル格納フォルダ", "COMMON", "FTR"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:Pファイル格納フォルダ 分類:COMMON 項目:FTR")
                Return False
            End If

            'FTRAN_EXEパス
            FTRANP_Folder = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
            If FTRANP_Folder.ToUpper = "ERR" OrElse FTRANP_Folder = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "FTRAN_EXEパス", "COMMON", "FTRANP"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:FTRAN_EXEパス 分類:COMMON 項目:FTRANP")
                Return False
            End If

            'テキストファイル格納フォルダ
            TXTFolder = CASTCommon.GetFSKJIni("COMMON", "TXT")
            If TXTFolder.ToUpper = "ERR" OrElse TXTFolder = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "テキストファイル格納フォルダ", "COMMON", "TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:テキストファイル格納フォルダ 分類:COMMON 項目:TXT")
                Return False
            End If


            '他行フォルダ
            TAKFolder = CASTCommon.GetFSKJIni("COMMON", "TAK")
            If TAKFolder.ToUpper = "ERR" OrElse TAKFolder = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "他行フォルダ", "COMMON", "TAK"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:他行フォルダ 分類:COMMON 項目:TAK")
                Return False
            End If


            '自金庫コード
            JikinkoCd = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If JikinkoCd.ToUpper = "ERR" OrElse JikinkoCd = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫コード 分類:COMMON 項目:KINKOCD")
                Return False
            End If

            '自金庫名
            JikinkoName = CASTCommon.GetFSKJIni("PRINT", "KINKONAME")
            If JikinkoName.ToUpper = "ERR" OrElse JikinkoName = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "自金庫名", "PRINT", "KINKONAME"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:自金庫名 分類:PRINT 項目:KINKONAME")
                Return False
            End If

            '2010/01/19 農協関連追加
            MatomeNoukyo = CASTCommon.GetFSKJIni("TAKO", "NOUKYOMATOME")
            If MatomeNoukyo.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "代表農協コード", "TAKO", "NOUKYOMATOME"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:代表農協コード 分類:TAKO 項目:NOUKYOMATOME")
                Return False
            End If

            Noukyo_From = CASTCommon.GetFSKJIni("TAKO", "NOUKYOFROM")
            If Noukyo_From.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "農協コード(FROM)", "TAKO", "NOUKYOFROM"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:農協コード(FROM) 分類:TAKO 項目:NOUKYOFROM")
                Return False
            End If

            Noukyo_To = CASTCommon.GetFSKJIni("TAKO", "NOUKYOTO")
            If Noukyo_To.ToUpper = "ERR" Then
                MessageBox.Show(String.Format(MSG0001E, "農協コード(TO)", "TAKO", "NOUKYOTO"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write("設定ファイル取得", "失敗", "項目名:農協コード(TO) 分類:TAKO 項目:NOUKYOTO")
                Return False
            End If
            '=================================

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(INIファイル取得)", "失敗", ex.ToString)
            Return False
        End Try
        Return True
    End Function
    Private Function fn_YOMIKOMI_MAIN() As Boolean
        '============================================================================
        'NAME           :fn_YOMIKOMI_MAIN
        'Parameter      :
        'Description    :受信ファイルの読み込み   
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/09/27
        'Update         :
        '============================================================================
        fn_YOMIKOMI_MAIN = False
        Try
            '2013/10/24 saitou 標準修正 ADD -------------------------------------------------->>>>
            Dim strCODE_KBN As String = String.Empty
            Dim strP_FILE As String = String.Empty
            '2013/10/24 saitou 標準修正 ADD --------------------------------------------------<<<<

            '--------------------------------
            '指定受信ファイルの存在確認
            '--------------------------------
            If Dir(strJYUSIN_FILE) = "" Then
                MessageBox.Show(MSG0132W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            strFILE_DATE = Format(FileDateTime(strJYUSIN_FILE), "yyyyMMdd")
            strFILE_TIME = Format(FileDateTime(strJYUSIN_FILE), "HHmmss")

            If strJYUSIN_DATE <> strFILE_DATE Then
                strFILE_DATE = strFILE_DATE.Substring(0, 4) & "年" & strFILE_DATE.Substring(4, 2) & "月" & strFILE_DATE.Substring(6, 2) & "日"
                If MessageBox.Show(String.Format(MSG0032I, strFILE_DATE, strJYUSIN_FILE), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                    Exit Function
                End If
            End If
            '--------------------------------
            'ファイルのコード変換
            '--------------------------------
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            Select Case Me.DEN_TEXT.FMT_KBN
                Case "00"  '全銀
                    Select Case Me.DEN_TEXT.CODE_KBN
                        Case "J"
                            strCODE_KBN = "0"
                            strP_FILE = "120JIS→JIS.P"
                        Case "E"
                            strCODE_KBN = "4"
                            strP_FILE = "120.P"
                    End Select
                Case "51"  '地公体
                    Select Case Me.DEN_TEXT.CODE_KBN
                        Case "J"
                            strCODE_KBN = "0"
                            strP_FILE = "220JIS→JIS.P"
                        Case "E"
                            strCODE_KBN = "4"
                            strP_FILE = "220.P"
                    End Select
                Case Else
                    MessageBox.Show(String.Format(MSG0275W, Me.DEN_TEXT.FMT_KBN), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
            End Select
            'Select Case strFMT_KBN
            '    Case "00"  '全銀
            '        Select Case strCODE_KBN
            '            Case "J"
            '                gstrCODE_KBN = "0"
            '                gstrP_FILE = "120JIS→JIS.P"
            '            Case "E"
            '                gstrCODE_KBN = "4"
            '                gstrP_FILE = "120.P"
            '        End Select
            '    Case "51"  '地公体
            '        Select Case strCODE_KBN
            '            Case "J"
            '                gstrCODE_KBN = "0"
            '                gstrP_FILE = "220JIS→JIS.P"
            '            Case "E"
            '                gstrCODE_KBN = "4"
            '                gstrP_FILE = "220.P"
            '        End Select
            '    Case Else
            '        MessageBox.Show(String.Format(MSG0275W, strFMT_KBN), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '        Exit Function
            'End Select
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
            Dim intKEKKA As Integer

            '地域G:2010/01/13 nakamura 全銀と地公体で場合分け
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            Select Case Me.DEN_TEXT.FMT_KBN
                Case "00" '全銀
                    intKEKKA = clsFUSION.fn_DEN_CPYTO_DISK("", strJYUSIN_FILE, strJYUSIN_FILE_JIS, _
                                                           GCom.NzInt(Me.DEN_TEXT.RECORD_LEN), strCODE_KBN, strP_FILE, msgTitle)
                Case "51" '地公体
                    '地域G:2010/01/13 nakamura NULL対応
                    intKEKKA = fn_DEN_CPYTO_DISK("", strJYUSIN_FILE, strJYUSIN_FILE_JIS, _
                                                 GCom.NzInt(Me.DEN_TEXT.RECORD_LEN), strCODE_KBN, strP_FILE, msgTitle)
            End Select
            'Select Case strFMT_KBN
            '    Case "00" '全銀
            '        intKEKKA = clsFUSION.fn_DEN_CPYTO_DISK("", strJYUSIN_FILE, strJYUSIN_FILE_JIS, _
            '                                               intRECORD_LEN, strCODE_KBN, strP_FILE, msgTitle)
            '    Case "51" '地公体
            '        '地域G:2010/01/13 nakamura NULL対応
            '        intKEKKA = fn_DEN_CPYTO_DISK("", strJYUSIN_FILE, strJYUSIN_FILE_JIS, _
            '                                   intRECORD_LEN, strCODE_KBN, strP_FILE, msgTitle)

            'End Select
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

            Select Case intKEKKA
                Case 0
                    MainLOG.Write("ファイル変換", "成功")
                Case 100
                    'Return         :0=成功、100=コード変換失敗
                    MainLOG.Write("ファイル変換（コード変換）", "失敗")
                    MessageBox.Show(MSG0019E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Function
                Case Else
                    'Return         :0=成功、100=コード変換失敗
                    MainLOG.Write("ファイル変換（コード変換）", "失敗")
                    MessageBox.Show(MSG0019E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Function
            End Select

            '--------------------------------
            '受信ファイルの読み込み
            '--------------------------------
            chlIrai.Items.Clear()
            chlKekka.Items.Clear()
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            Select Case Me.DEN_TEXT.FMT_KBN
                Case "00" '全銀
                    If fn_YOMIKOMI_00() = False Then
                        Exit Function
                    End If
                Case "51" '地公体
                    If fn_YOMIKOMI_01() = False Then
                        Exit Function
                    End If
            End Select
            'Select Case strFMT_KBN
            '    Case "00" '全銀
            '        If fn_YOMIKOMI_00() = False Then
            '            Exit Function
            '        End If
            '    Case "51" '地公体
            '        If fn_YOMIKOMI_01() = False Then
            '            Exit Function
            '        End If
            'End Select
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(読込メイン処理)", "失敗", ex.ToString)
            Return False
        End Try

        fn_YOMIKOMI_MAIN = True
    End Function
    Private Function fn_YOMIKOMI_00() As Boolean
        '============================================================================
        'NAME           :fn_YOMIKOMI_00
        'Parameter      :
        'Description    :全銀ファイルの読み込み   
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/09/27
        'Update         :
        '============================================================================
        fn_YOMIKOMI_00 = False
        Dim EntryStream As FileStream = Nothing
        Dim gZENGIN_REC1 As CAstFormat.CFormatZengin.ZGRECORD1 = Nothing
        Dim gZENGIN_REC2 As CAstFormat.CFormatZengin.ZGRECORD2 = Nothing
        Dim gZENGIN_REC8 As CAstFormat.CFormatZengin.ZGRECORD8 = Nothing
        Dim gZENGIN_REC9 As CAstFormat.CFormatZengin.ZGRECORD9 = Nothing
        Dim FileWriter As StreamWriter = Nothing

        Dim strFURI_DATA As String
        Try
            EntryStream = New FileStream(strJYUSIN_FILE_JIS, FileMode.Open, FileAccess.Read) '読込ファイル
            Dim br As BinaryReader = New BinaryReader(EntryStream)
            Dim FileLen As Integer = EntryStream.Length
            Dim _cnt As Long = 0
            Dim lngRECORD_COUNT As Long
            Dim lngKOBETU_RECORD_COUNT As Long
            Dim intIRAI_COUNT As Integer = 0
            Dim intKEKKA_COUNT As Integer = 0
            Dim WorkFile As String = ""
            Dim strITAKU_KANA As String
            Dim strNS_KBN As String
            Dim strKINKO_CODE As String

            strDATA_KBN = "0"
            lngRECORD_COUNT = 0
            Do
                lngRECORD_COUNT += 1
                br.BaseStream.Seek(_cnt, SeekOrigin.Begin)
                strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(120))

                Select Case strFURI_DATA.Substring(0, 1)
                    Case "1"     'ヘッダーレコード
                        Select Case strDATA_KBN
                            Case "0"
                            Case "2"
                                MessageBox.Show(MSG0260W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                MainLOG.Write("ファイル読み込み", "失敗", "レコードNO：" & lngRECORD_COUNT & MSG0260W)
                                Exit Function
                            Case "8"
                                'ファイルは必ず12･･･89形式にする
                                gZENGIN_REC9.ZG1 = 9
                                FileWriter.Write(gZENGIN_REC9.Data)
                                FileWriter.Close()
                                FileWriter = Nothing
                                File.Move(WorkFile, strKOBETU_FILE)
                            Case "9"
                            Case Else
                                MessageBox.Show(MSG0260W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                        End Select
                        gZENGIN_REC1.Data = strFURI_DATA
                        lngKOBETU_RECORD_COUNT = 1
                        ''富山信金カスタマイズ　2009/04/09
                        strITAKU_KANA = gZENGIN_REC1.ZG5.Trim
                        strNS_KBN = gZENGIN_REC1.ZG2.Trim
                        strITAKU_CODE = gZENGIN_REC1.ZG4
                        strKINKO_CODE = gZENGIN_REC1.ZG7
                        '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
                        'テキストファイルの内容を構造体で管理
                        If Me.DEN_TEXT.CENTER_KIN = "0000" Then
                            Me.DEN_TEXT.CENTER_KIN = strKINKO_CODE
                        End If
                        'If strSENTER_KIN = "0000" Then
                        '    strSENTER_KIN = strKINKO_CODE
                        'End If
                        '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

                        '--------------------------
                        '振替日の判定
                        '--------------------------
                        '振替日<=受信日：結果分
                        '振替日>受信日 ：依頼分

                        strFURI_DATE = strJYUSIN_DATE.Substring(0, 4) & gZENGIN_REC1.ZG6
                        If strJYUSIN_DATE.Substring(4, 2) = "01" Or strJYUSIN_DATE.Substring(4, 2) = "02" Then
                            If gZENGIN_REC1.ZG6.Substring(0, 2) = "11" Or gZENGIN_REC1.ZG6.Substring(0, 2) = "12" Then
                                '前年の結果データなので西暦年を１年ダウン
                                strFURI_DATE = CStr(CInt(strJYUSIN_DATE.Substring(0, 4)) - 1) & gZENGIN_REC1.ZG6
                            End If
                        ElseIf strJYUSIN_DATE.Substring(4, 2) = "11" Or strJYUSIN_DATE.Substring(4, 2) = "12" Then
                            If gZENGIN_REC1.ZG6.Substring(0, 2) = "01" Or gZENGIN_REC1.ZG6.Substring(0, 2) = "02" Then
                                '翌年の依頼データなので西暦年を１年アップ
                                strFURI_DATE = CStr(CInt(strJYUSIN_DATE.Substring(0, 4)) + 1) & gZENGIN_REC1.ZG6
                            End If
                        End If
                        strITAKU_CODE = gZENGIN_REC1.ZG4

                        '--------------------------
                        'ファイルのオープン
                        '--------------------------
                        WorkFile = Path.Combine(DATBKFolder, strITAKU_CODE & strFURI_DATE & ".DAT")
                        If Dir(WorkFile) <> "" Then
                            Kill(WorkFile)
                        End If
                        FileWriter = New StreamWriter(WorkFile, False, System.Text.Encoding.GetEncoding("Shift-JIS"))  '書込みファイル
                        FileWriter.Write(strFURI_DATA)
                    Case "2"
                        lngKOBETU_RECORD_COUNT += 1
                        strDATA_KBN = "2"
                        FileWriter.Write(strFURI_DATA)
                    Case "8"
                        lngKOBETU_RECORD_COUNT += 1
                        strDATA_KBN = "8"
                        FileWriter.Write(strFURI_DATA)
                        '振替済・不能の値を判定し、依頼分・不能分を判定
                        gZENGIN_REC8.Data = strFURI_DATA
                        If GCom.NzLong(Trim(gZENGIN_REC8.ZG4)) = 0 AndAlso GCom.NzLong(Trim(gZENGIN_REC8.ZG6)) = 0 Then
                            '済件数・不能件数が0の場合依頼分
                            intIRAI_COUNT += 1
                            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
                            'テキストファイルの内容を構造体で管理
                            If fn_SELECT_TORIMAST(strITAKU_CODE, Me.DEN_TEXT.FMT_KBN, strFURI_DATE, strIRAI_TORIS_CODE(intIRAI_COUNT), strIRAI_TORIF_CODE(intIRAI_COUNT), strIRAI_ITAKU_NAME(intIRAI_COUNT)) = False Then
                                MessageBox.Show(String.Format(MSG0276W, strITAKU_CODE, strFURI_DATE.Substring(0, 4) & "年" & strFURI_DATE.Substring(4, 2) & "月" & strFURI_DATE.Substring(6, 2) & "日") _
                                    , msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            End If
                            'If fn_SELECT_TORIMAST(strITAKU_CODE, strFMT_KBN, strFURI_DATE, strIRAI_TORIS_CODE(intIRAI_COUNT), strIRAI_TORIF_CODE(intIRAI_COUNT), strIRAI_ITAKU_NAME(intIRAI_COUNT)) = False Then
                            '    MessageBox.Show(String.Format(MSG0276W, strITAKU_CODE, strFURI_DATE.Substring(0, 4) & "年" & strFURI_DATE.Substring(4, 2) & "月" & strFURI_DATE.Substring(6, 2) & "日") _
                            '        , msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
                            strIRAI_SENTAKU(intIRAI_COUNT) = "1"
                            strIRAI_FURI_DATE(intIRAI_COUNT) = strFURI_DATE

                            '--------------------------
                            '依頼分リストに追加
                            '--------------------------
                            chlIrai.Items.Add(strIRAI_TORIS_CODE(intIRAI_COUNT) & "-" & strIRAI_TORIF_CODE(intIRAI_COUNT) & Space(3) & strIRAI_ITAKU_NAME(intIRAI_COUNT), True)

                            'ファイル名の設定
                            strKOBETU_FILE = Path.Combine(DenFolder, "D" & strIRAI_TORIS_CODE(intIRAI_COUNT) & strIRAI_TORIF_CODE(intIRAI_COUNT) & ".DAT")
                            If Dir(strKOBETU_FILE) <> "" Then
                                Kill(strKOBETU_FILE)
                            End If

                        Else
                            '済件数・不能件数が0でない場合不能分
                            intKEKKA_COUNT += 1
                            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
                            'テキストファイルの内容を構造体で管理
                            If fn_SELECT_TAKOUMAST(strITAKU_CODE, Me.DEN_TEXT.FMT_KBN, strFURI_DATE, strKEKKA_TORIS_CODE(intKEKKA_COUNT), strKEKKA_TORIF_CODE(intKEKKA_COUNT), strKEKKA_ITAKU_NAME(intKEKKA_COUNT)) = False Then
                                MessageBox.Show(String.Format(MSG0277W, strITAKU_CODE, strFURI_DATE.Substring(0, 4) & "年" & strFURI_DATE.Substring(4, 2) & "月" & strFURI_DATE.Substring(6, 2) & "日") _
                                                , msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            End If
                            'If fn_SELECT_TAKOUMAST(strITAKU_CODE, strFMT_KBN, strFURI_DATE, strKEKKA_TORIS_CODE(intKEKKA_COUNT), strKEKKA_TORIF_CODE(intKEKKA_COUNT), strKEKKA_ITAKU_NAME(intKEKKA_COUNT)) = False Then
                            '    MessageBox.Show(String.Format(MSG0277W, strITAKU_CODE, strFURI_DATE.Substring(0, 4) & "年" & strFURI_DATE.Substring(4, 2) & "月" & strFURI_DATE.Substring(6, 2) & "日") _
                            '                    , msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
                            strKEKKA_SENTAKU(intKEKKA_COUNT) = "1"
                            strKEKKA_FURI_DATE(intKEKKA_COUNT) = strFURI_DATE

                            '--------------------------
                            '結果分リストに追加
                            '--------------------------
                            chlKekka.Items.Add(strKEKKA_TORIS_CODE(intKEKKA_COUNT) & "-" & strKEKKA_TORIF_CODE(intKEKKA_COUNT) & Space(3) & strKEKKA_ITAKU_NAME(intKEKKA_COUNT), True)

                            '--------------------------
                            'ファイル名の設定 
                            '--------------------------
                            '対象金融機関の受信ファイル名を取得する
                            Dim RFileName As String = ""
                            Call fn_SELECT_TAKOUMAST(strKEKKA_TORIS_CODE(intKEKKA_COUNT), strKEKKA_TORIF_CODE(intKEKKA_COUNT), gZENGIN_REC1.ZG7, RFileName)
                            If Trim(RFileName) = "" Then
                                MessageBox.Show(String.Format(MSG0354W, strKEKKA_TORIS_CODE(intKEKKA_COUNT) & "-" & strKEKKA_TORIF_CODE(intKEKKA_COUNT), gZENGIN_REC1.ZG7), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                MainLOG.Write("他行受信ファイル名取得", "失敗", "取引先コード:" & strKEKKA_TORIS_CODE(intKEKKA_COUNT) & "-" & strKEKKA_TORIF_CODE(intKEKKA_COUNT) & " 金融機関コード:" & gZENGIN_REC1.ZG7)
                            End If
                            strKOBETU_FILE = Path.Combine(DenFolder, RFileName)
                            strKEKKA_FILE(intKEKKA_COUNT) = strKOBETU_FILE
                            If Dir(strKOBETU_FILE) <> "" Then
                                Kill(strKOBETU_FILE)
                            End If
                        End If
                    Case "9"
                        lngKOBETU_RECORD_COUNT += 1
                        strDATA_KBN = "9"
                        FileWriter.Write(strFURI_DATA)
                        FileWriter.Close()
                        FileWriter = Nothing
                        File.Move(WorkFile, strKOBETU_FILE)
                    Case Else
                        MessageBox.Show(MSG0260W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLOG.Write("ファイル読み込み", "失敗", "レコードNO：" & lngRECORD_COUNT & MSG0260W)
                        Exit Function
                        FileWriter.Close()
                End Select
                _cnt += 120
            Loop While FileLen > _cnt
            EntryStream.Close()
            If strDATA_KBN <> "9" Then
                MessageBox.Show(MSG0260W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write("ファイル読み込み", "失敗", "レコードNO：" & lngRECORD_COUNT & MSG0260W)
                Return False
                Exit Function
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全銀読込処理)", "失敗", ex.ToString)
            Return False
        Finally
            If Not FileWriter Is Nothing Then FileWriter.Close()
            If Not EntryStream Is Nothing Then EntryStream.Close()
        End Try
        fn_YOMIKOMI_00 = True

    End Function
    Private Function fn_YOMIKOMI_01() As Boolean
        ''============================================================================
        ''NAME           :fn_YOMIKOMI_01
        ''Parameter      :
        ''Description    :地公体ファイルの読み込み   
        ''Return         :True=OK(成功),False=NG（失敗）
        ''Create         :2004/09/27
        ''Update         :
        ''============================================================================

        fn_YOMIKOMI_01 = False
        Dim EntryStream As FileStream = Nothing
        Dim g220_REC1 As CAstFormat.CFormatZeikin220.ZEIKIN_RECORD1 = Nothing
        Dim g220_REC2 As CAstFormat.CFormatZeikin220.ZEIKIN_RECORD2 = Nothing
        Dim g220_REC8 As CAstFormat.CFormatZeikin220.ZEIKIN_RECORD8 = Nothing
        Dim g220_REC9 As CAstFormat.CFormatZeikin220.ZEIKIN_RECORD9 = Nothing
        Dim FileWriter As StreamWriter = Nothing

        Dim strFURI_DATA As String
        Try
            EntryStream = New FileStream(strJYUSIN_FILE_JIS, FileMode.Open, FileAccess.Read) '読込ファイル
            Dim br As BinaryReader = New BinaryReader(EntryStream)
            Dim FileLen As Integer = EntryStream.Length
            Dim _cnt As Long = 0
            Dim lngRECORD_COUNT As Long
            Dim lngKOBETU_RECORD_COUNT As Long
            Dim intIRAI_COUNT As Integer = 0
            Dim intKEKKA_COUNT As Integer = 0
            Dim WorkFile As String = ""
            Dim strITAKU_KANA As String
            Dim strNS_KBN As String
            Dim strKINKO_CODE As String
            strDATA_KBN = "0"
            lngRECORD_COUNT = 0
            Do
                lngRECORD_COUNT += 1
                br.BaseStream.Seek(_cnt, SeekOrigin.Begin)
                strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(220))

                Select Case strFURI_DATA.Substring(0, 1)
                    Case "1"     'ヘッダーレコード
                        Select Case strDATA_KBN
                            Case "0"
                            Case "2"
                                MessageBox.Show(MSG0260W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                MainLOG.Write("ファイル読み込み", "失敗", "レコードNO：" & lngRECORD_COUNT & MSG0260W)
                                Exit Function
                            Case "8"
                                'ファイルは必ず12･･･89形式にする
                                g220_REC9.ZK1 = 9
                                FileWriter.Write(g220_REC9.Data)
                                FileWriter.Close()
                                FileWriter = Nothing
                                File.Move(WorkFile, strKOBETU_FILE)
                            Case "9"
                            Case Else
                                MessageBox.Show(MSG0260W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                        End Select
                        g220_REC1.Data = strFURI_DATA
                        lngKOBETU_RECORD_COUNT = 1

                        ''富山信金カスタマイズ　2009/04/06
                        strITAKU_KANA = g220_REC1.ZK5.Trim
                        strNS_KBN = g220_REC1.ZK2
                        strITAKU_CODE = g220_REC1.ZK4
                        strKINKO_CODE = g220_REC1.ZK7
                        '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
                        'テキストファイルの内容を構造体で管理
                        If Me.DEN_TEXT.CENTER_KIN = "0000" Then
                            Me.DEN_TEXT.CENTER_KIN = strKINKO_CODE
                        End If
                        'If strSENTER_KIN = "0000" Then
                        '    strSENTER_KIN = strKINKO_CODE
                        'End If
                        '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

                        '--------------------------
                        '振替日の判定
                        '--------------------------
                        '振替日<=受信日：結果分
                        '振替日>受信日 ：依頼分

                        strFURI_DATE = strJYUSIN_DATE.Substring(0, 4) & g220_REC1.ZK6
                        If strJYUSIN_DATE.Substring(4, 2) = "01" Or strJYUSIN_DATE.Substring(4, 2) = "02" Then
                            If g220_REC1.ZK6.Substring(0, 2) = "11" Or g220_REC1.ZK6.Substring(0, 2) = "12" Then
                                '前年の結果データなので西暦年を１年ダウン
                                strFURI_DATE = CStr(CInt(strJYUSIN_DATE.Substring(0, 4)) - 1) & g220_REC1.ZK6
                            End If
                        ElseIf strJYUSIN_DATE.Substring(4, 2) = "11" Or strJYUSIN_DATE.Substring(4, 2) = "12" Then
                            If g220_REC1.ZK6.Substring(0, 2) = "01" Or g220_REC1.ZK6.Substring(0, 2) = "02" Then
                                '翌年の依頼データなので西暦年を１年アップ
                                strFURI_DATE = CStr(CInt(strJYUSIN_DATE.Substring(0, 4)) + 1) & g220_REC1.ZK6
                            End If
                        End If
                        strITAKU_CODE = g220_REC1.ZK4

                        '--------------------------
                        'ファイルのオープン
                        '--------------------------
                        WorkFile = Path.Combine(DATBKFolder, strITAKU_CODE & strFURI_DATE & ".DAT")
                        If Dir(WorkFile) <> "" Then
                            Kill(WorkFile)
                        End If
                        FileWriter = New StreamWriter(WorkFile, False, System.Text.Encoding.GetEncoding("Shift-JIS"))  '書込みファイル
                        FileWriter.Write(strFURI_DATA)
                    Case "2"
                        lngKOBETU_RECORD_COUNT += 1
                        strDATA_KBN = "2"
                        FileWriter.Write(strFURI_DATA)
                    Case "8"
                        lngKOBETU_RECORD_COUNT += 1
                        strDATA_KBN = "8"
                        FileWriter.Write(strFURI_DATA)

                        '振替済・不能の値を判定し、依頼分・不能分を判定
                        g220_REC8.Data = strFURI_DATA
                        If GCom.NzLong(Trim(g220_REC8.ZK4)) = 0 AndAlso GCom.NzLong(Trim(g220_REC8.ZK6)) = 0 Then
                            '済件数・不能件数が0の場合依頼分
                            intIRAI_COUNT += 1
                            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
                            'テキストファイルの内容を構造体で管理
                            If fn_SELECT_TORIMAST(strITAKU_CODE, Me.DEN_TEXT.FMT_KBN, strFURI_DATE, strIRAI_TORIS_CODE(intIRAI_COUNT), strIRAI_TORIF_CODE(intIRAI_COUNT), strIRAI_ITAKU_NAME(intIRAI_COUNT)) = False Then
                                MessageBox.Show(String.Format(MSG0276W, strITAKU_CODE, strFURI_DATE.Substring(0, 4) & "年" & strFURI_DATE.Substring(4, 2) & "月" & strFURI_DATE.Substring(6, 2) & "日") _
                                       , msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            End If
                            'If fn_SELECT_TORIMAST(strITAKU_CODE, strFMT_KBN, strFURI_DATE, strIRAI_TORIS_CODE(intIRAI_COUNT), strIRAI_TORIF_CODE(intIRAI_COUNT), strIRAI_ITAKU_NAME(intIRAI_COUNT)) = False Then
                            '    MessageBox.Show(String.Format(MSG0276W, strITAKU_CODE, strFURI_DATE.Substring(0, 4) & "年" & strFURI_DATE.Substring(4, 2) & "月" & strFURI_DATE.Substring(6, 2) & "日") _
                            '           , msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
                            strIRAI_SENTAKU(intIRAI_COUNT) = "1"
                            strIRAI_FURI_DATE(intIRAI_COUNT) = strFURI_DATE

                            '--------------------------
                            '依頼分リストに追加
                            '--------------------------
                            chlIrai.Items.Add(strIRAI_TORIS_CODE(intIRAI_COUNT) & "-" & strIRAI_TORIF_CODE(intIRAI_COUNT) & Space(3) & strIRAI_ITAKU_NAME(intIRAI_COUNT), True)

                            'ファイル名の設定
                            strKOBETU_FILE = Path.Combine(DenFolder, "D" & strIRAI_TORIS_CODE(intIRAI_COUNT) & strIRAI_TORIF_CODE(intIRAI_COUNT) & ".DAT")
                            If Dir(strKOBETU_FILE) <> "" Then
                                Kill(strKOBETU_FILE)
                            End If
                        Else
                            '済件数・不能件数が0でない場合不能分
                            intKEKKA_COUNT += 1
                            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
                            'テキストファイルの内容を構造体で管理
                            If fn_SELECT_TAKOUMAST(strITAKU_CODE, Me.DEN_TEXT.FMT_KBN, strFURI_DATE, strKEKKA_TORIS_CODE(intKEKKA_COUNT), strKEKKA_TORIF_CODE(intKEKKA_COUNT), strKEKKA_ITAKU_NAME(intKEKKA_COUNT)) = False Then
                                MessageBox.Show(String.Format(MSG0277W, strITAKU_CODE, strFURI_DATE.Substring(0, 4) & "年" & strFURI_DATE.Substring(4, 2) & "月" & strFURI_DATE.Substring(6, 2) & "日") _
                                       , msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            End If
                            'If fn_SELECT_TAKOUMAST(strITAKU_CODE, strFMT_KBN, strFURI_DATE, strKEKKA_TORIS_CODE(intKEKKA_COUNT), strKEKKA_TORIF_CODE(intKEKKA_COUNT), strKEKKA_ITAKU_NAME(intKEKKA_COUNT)) = False Then
                            '    MessageBox.Show(String.Format(MSG0277W, strITAKU_CODE, strFURI_DATE.Substring(0, 4) & "年" & strFURI_DATE.Substring(4, 2) & "月" & strFURI_DATE.Substring(6, 2) & "日") _
                            '           , msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
                            strKEKKA_SENTAKU(intKEKKA_COUNT) = "1"
                            strKEKKA_FURI_DATE(intKEKKA_COUNT) = strFURI_DATE

                            '--------------------------
                            '結果分リストに追加
                            '--------------------------
                            chlKekka.Items.Add(strKEKKA_TORIS_CODE(intKEKKA_COUNT) & "-" & strKEKKA_TORIF_CODE(intKEKKA_COUNT) & Space(3) & strKEKKA_ITAKU_NAME(intKEKKA_COUNT), True)

                            '--------------------------
                            'ファイル名の設定 
                            '--------------------------
                            '対象金融機関の受信ファイル名を取得する
                            Dim RFileName As String = ""
                            Call fn_SELECT_TAKOUMAST(strKEKKA_TORIS_CODE(intKEKKA_COUNT), strKEKKA_TORIF_CODE(intKEKKA_COUNT), g220_REC1.ZK7, RFileName)
                            If Trim(RFileName) = "" Then
                                MessageBox.Show(String.Format(MSG0354W, strKEKKA_TORIS_CODE(intKEKKA_COUNT) & "-" & strKEKKA_TORIF_CODE(intKEKKA_COUNT), g220_REC1.ZK7), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                MainLOG.Write("他行受信ファイル名取得", "失敗", "取引先コード:" & strKEKKA_TORIS_CODE(intKEKKA_COUNT) & "-" & strKEKKA_TORIF_CODE(intKEKKA_COUNT) & " 金融機関コード:" & g220_REC1.ZK7)
                            End If
                            strKOBETU_FILE = Path.Combine(DenFolder, RFileName)
                            strKEKKA_FILE(intKEKKA_COUNT) = strKOBETU_FILE
                            If Dir(strKOBETU_FILE) <> "" Then
                                Kill(strKOBETU_FILE)
                            End If
                        End If
                    Case "9"
                        lngKOBETU_RECORD_COUNT += 1
                        strDATA_KBN = "9"
                        FileWriter.Write(strFURI_DATA)
                        FileWriter.Close()
                        FileWriter = Nothing
                        File.Move(WorkFile, strKOBETU_FILE)
                    Case Else
                        MessageBox.Show(MSG0260W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLOG.Write("ファイル読み込み", "失敗", "レコードNO：" & lngRECORD_COUNT & MSG0260W)
                        Exit Function
                        FileWriter.Close()
                End Select
                _cnt += 220
            Loop While FileLen > _cnt
            EntryStream.Close()
            If strDATA_KBN <> "9" Then
                MessageBox.Show(MSG0260W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLOG.Write("ファイル読み込み", "失敗", "レコードNO：" & lngRECORD_COUNT & MSG0260W)
                Exit Function
            End If
            fn_YOMIKOMI_01 = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(全銀読込処理)", "失敗", ex.ToString)
            Return False
        Finally
            If Not FileWriter Is Nothing Then FileWriter.Close()
            If Not EntryStream Is Nothing Then EntryStream.Close()
        End Try
    End Function
    Private Function fn_SELECT_TORIMAST(ByVal astrITAKU_CODE As String, ByVal astrFMT_CODE As String, ByVal astrFURI_DATE As String, ByRef astrTORIS_CODE As String, ByRef astrTORIF_CODE As String, ByRef astrITAKU_NAME As String) As Boolean
        '============================================================================
        'NAME           :fn_SELECT_TORIMAST
        'Parameter      :
        'Description    :取引先マスタ・スケジュールマスタの検索   
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/09/27
        'Update         :2006/06/20 条件に有効フラグ追加
        'Update         :2006/06/20  契約振替基準日(最初にヒットした振替日)を表示
        '============================================================================
        Dim Orareader As New MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            fn_SELECT_TORIMAST = False
            Dim intTORI_COUNT As Integer
            Dim intDATE_CNT As Integer
            SQL.Append(" SELECT * ")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE FSYORI_KBN_T = '1'")
            SQL.Append(" AND ITAKU_CODE_T = " & SQ(Trim(astrITAKU_CODE)))
            SQL.Append(" AND FURI_DATE_S = " & SQ(Trim(astrFURI_DATE)))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND BAITAI_CODE_T = '00'")  '伝送
            SQL.Append(" AND FMT_KBN_T = " & SQ(Trim(astrFMT_CODE)))
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")

            '読込のみ
            intTORI_COUNT = 0
            KFJTAKO011.lstSCHLIST.Items.Clear()
            If Orareader.DataReader(SQL) Then
                While Orareader.EOF = False
                    intTORI_COUNT += 1
                    strTORIS_CODE = GCom.NzStr(Orareader.GetString("TORIS_CODE_T"))
                    strTORIF_CODE = GCom.NzStr(Orareader.GetString("TORIF_CODE_T"))
                    strITAKU_NAME = GCom.NzStr(Orareader.GetString("ITAKU_NNAME_T"))
                    For intDATE_CNT = 1 To 31
                        If GCom.NzStr(Orareader.GetString("DATE" & intDATE_CNT & "_T")) = "1" Then
                            strKEIYAKU_DATE = CStr(intDATE_CNT).PadLeft(2, "0")
                            Exit For
                        End If
                    Next
                    KFJTAKO011.lblITAKU_CODE.Text = astrITAKU_CODE
                    KFJTAKO011.lstSCHLIST.Items.Add(strTORIS_CODE & "-" & strTORIF_CODE & Space(5) & strKEIYAKU_DATE & Space(5) & astrFURI_DATE & Space(5) & strITAKU_NAME)
                    Orareader.NextRead()
                End While
            End If

            If intTORI_COUNT = 0 Then
                fn_SELECT_TORIMAST = False
                Orareader.Close()
                Exit Function
            End If

            If intTORI_COUNT <> 1 Then
                Dim strITEM As String
                KFJTAKO011.lblKEN.Text = intTORI_COUNT
                KFJTAKO011.ShowDialog()
                Application.DoEvents()
                If KFJTAKO011.lstSCHLIST.SelectedItem Is Nothing Then '追加 2006/06/20
                    fn_SELECT_TORIMAST = False
                    Orareader.Close()
                    Exit Function
                End If
                '2010/01/14 値を取得できるように修正
                'strITEM = KFJTAKO011.lstSCHLIST.SelectedItem
                strITEM = KFJTAKO011.lstSCHLIST.Items.Item(gintMAIN0101G_LIST_INDEX)
                astrTORIS_CODE = strITEM.Substring(0, 10)
                astrTORIF_CODE = strITEM.Substring(11, 2)
                astrITAKU_NAME = strITEM.Substring(30).Trim
            ElseIf intTORI_COUNT = 1 Then
                astrTORIS_CODE = strTORIS_CODE
                astrTORIF_CODE = strTORIF_CODE
                astrITAKU_NAME = strITAKU_NAME
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先マスタ情報取得)", "失敗", ex.ToString)
            Return False
        Finally
            If Not Orareader Is Nothing = False Then Orareader.Close()
        End Try
        fn_SELECT_TORIMAST = True
    End Function
    Private Function fn_SELECT_TAKOUMAST(ByVal astrITAKU_CODE As String, ByVal astrFMT_CODE As String, ByVal astrFURI_DATE As String, ByRef astrTORIS_CODE As String, ByRef astrTORIF_CODE As String, ByRef astrITAKU_NAME As String) As Boolean
        '============================================================================
        'NAME           :fn_SELECT_TAKOUMAST
        'Parameter      :
        'Description    :取引先マスタ・スケジュールマスタ・他行スケジュールマスタの検索   
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/09/27
        'Update         :
        '============================================================================
        Dim Orareader As New MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            fn_SELECT_TAKOUMAST = False
            Dim intTAKOU_COUNT As Integer
            SQL.Append("SELECT * ")
            SQL.Append("FROM TAKOUMAST,SCHMAST,TORIMAST ")
            SQL.Append(" WHERE ")
            SQL.Append(" ITAKU_CODE_V = " & SQ(Trim(astrITAKU_CODE)))
            '地域G:2010/01/14 nakamura 農協対応
            'SQL.Append(" AND TKIN_NO_V = " & SQ(Trim(strSENTER_KIN)))
            'If Trim(strTAKOU_KIN) = "5900" Then
            '    SQL.Append(" AND TKIN_NO_U >= '5874' AND TKIN_NO_U <= '5939' ")    'iniファイル対応
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            If Trim(Me.DEN_TEXT.CENTER_KIN) = MatomeNoukyo Then
                SQL.Append(" AND TKIN_NO_V BETWEEN " & SQ(Noukyo_From) & " AND " & SQ(Noukyo_To))
            Else
                SQL.Append(" AND TKIN_NO_V = " & SQ(Trim(Me.DEN_TEXT.CENTER_KIN)))
            End If
            'If Trim(strSENTER_KIN) = MatomeNoukyo Then
            '    SQL.Append(" AND TKIN_NO_V BETWEEN " & SQ(Noukyo_From) & " AND " & SQ(Noukyo_To))
            'Else
            '    SQL.Append(" AND TKIN_NO_V = " & SQ(Trim(strSENTER_KIN)))
            'End If
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
            SQL.Append(" AND FURI_DATE_S = " & SQ(Trim(astrFURI_DATE)))
            SQL.Append(" AND BAITAI_CODE_V = '00' ")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            SQL.Append(" AND FMT_KBN_T =" & SQ(Trim(astrFMT_CODE)))
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIS_CODE_V = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND TORIF_CODE_V = TORIF_CODE_T")

            '読込のみ
            intTAKOU_COUNT = 0
            KFJTAKO011.lstSCHLIST.Items.Clear()
            If Orareader.DataReader(SQL) Then
                While Orareader.EOF = False
                    intTAKOU_COUNT += 1
                    strTORIS_CODE = GCom.NzStr(Orareader.GetString("TORIS_CODE_T"))
                    strTORIF_CODE = GCom.NzStr(Orareader.GetString("TORIF_CODE_T"))
                    strITAKU_NAME = GCom.NzStr(Orareader.GetString("ITAKU_NNAME_T"))
                    KFJTAKO011.lblITAKU_CODE.Text = astrITAKU_CODE
                    KFJTAKO011.lstSCHLIST.Items.Add(strTORIS_CODE & "-" & strTORIF_CODE & Space(5) & strKEIYAKU_DATE & Space(5) & astrFURI_DATE & Space(5) & strITAKU_NAME)
                    '2010/01/21 農協まとめの場合は選択一覧を出力する必要なし
                    '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
                    'テキストファイルの内容を構造体で管理
                    If Trim(Me.DEN_TEXT.CENTER_KIN) = MatomeNoukyo Then
                        Exit While
                    End If
                    'If Trim(strSENTER_KIN) = MatomeNoukyo Then
                    '    Exit While
                    'End If
                    '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
                    '=======================================================
                    Orareader.NextRead()
                End While
            End If

            If intTAKOU_COUNT = 0 Then
                fn_SELECT_TAKOUMAST = False
                Exit Function
            End If
            If intTAKOU_COUNT <> 1 Then
                'Dim strITEM As String
                KFJTAKO011.lblKEN.Text = intTAKOU_COUNT
                KFJTAKO011.ShowDialog()
                Application.DoEvents()
                Dim strITEM As String
                '2010/01/14 値を取得できるように修正
                'strITEM = KFJTAKO011.lstSCHLIST.SelectedItem(gintMAIN0101G_LIST_INDEX)
                strITEM = KFJTAKO011.lstSCHLIST.Items.Item(gintMAIN0101G_LIST_INDEX)
                astrTORIS_CODE = strITEM.Substring(0, 10)
                astrTORIF_CODE = strITEM.Substring(11, 2)
                astrITAKU_NAME = strITEM.Substring(32).Trim
            ElseIf intTAKOU_COUNT = 1 Then
                astrTORIS_CODE = strTORIS_CODE
                astrTORIF_CODE = strTORIF_CODE
                astrITAKU_NAME = strITAKU_NAME
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(他行マスタ情報取得)", "失敗", ex.ToString)
            Return False
        Finally
            If Not Orareader Is Nothing = False Then Orareader.Close()
        End Try
        fn_SELECT_TAKOUMAST = True

    End Function
    Private Function fn_SELECT_TAKOUMAST(ByVal astrTORIS_CODE As String, ByVal astrTORIF_CODE As String, ByRef astrTKIN_NO As String, Optional ByRef RFileName As String = "") As Boolean
        '============================================================================
        'NAME           :fn_SELECT_TAKOUMAST
        'Parameter      :
        'Description    :他行マスタの検索   
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/09/27
        'Update         :
        '============================================================================
        Dim Orareader As New MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Try
            fn_SELECT_TAKOUMAST = False
            SQL.Append(" SELECT COUNT(*) COUNTER,MAX(RFILE_NAME_V) RFILE_NAME_V ")
            SQL.Append(" FROM TAKOUMAST ")
            SQL.Append(" WHERE TORIS_CODE_V = " & SQ(astrTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_V = " & SQ(Trim(astrTORIF_CODE)))
            '地域G:2010/01/14 nakamura 農協対応
            'SQL.Append(" AND TKIN_NO_V = " & SQ(Trim(astrTKIN_NO)))
            'If Trim(strTAKOU_KIN) = "5900" Then
            '    SQL.Append(" AND TKIN_NO_V >= '5874' AND TKIN_NO_U <= '5939' ")    'iniファイル対応
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            If Trim(Me.DEN_TEXT.CENTER_KIN) = MatomeNoukyo Then '2010/03/03 strTAKOU_KIN → strSENTER_KIN
                SQL.Append(" AND TKIN_NO_V BETWEEN " & SQ(Noukyo_From) & " AND " & SQ(Noukyo_To))
            Else
                SQL.Append(" AND TKIN_NO_V = " & SQ(Trim(astrTKIN_NO)))
            End If
            'If Trim(strSENTER_KIN) = MatomeNoukyo Then '2010/03/03 strTAKOU_KIN → strSENTER_KIN
            '    SQL.Append(" AND TKIN_NO_V BETWEEN " & SQ(Noukyo_From) & " AND " & SQ(Noukyo_To))
            'Else
            '    SQL.Append(" AND TKIN_NO_V = " & SQ(Trim(astrTKIN_NO)))
            'End If
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

            '読込のみ
            If Orareader.DataReader(SQL) Then
                If Orareader.GetInt("COUNTER") > 0 Then
                    RFileName = Orareader.GetString("RFILE_NAME_V")
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(他行マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If Not Orareader Is Nothing = False Then Orareader.Close()
        End Try
        fn_SELECT_TAKOUMAST = True

    End Function
    Private Function fn_DEN_CPYTO_DISK(ByVal strTORI_CODE As String, ByVal strIN_FILE_NAME As String, ByVal strOUT_FILE_NAME As String, ByVal intREC_LENGTH As Integer, ByVal strCODE_KBN As String, ByVal strP_FILE As String, ByVal msgTitle As String) As Integer
        '=====================================================================================
        'NAME           :fn_DEN_CPYTO_DISK
        'Parameter      :strTORI_CODE：取引先コード／strIN_FILE_NAME：入力ファイル名／
        '               :strOUT_FILE_NAME：出力ファイル／intREC_LENGTH：レコード長／
        '               :strCODE_KBN：コード区分／strP_FILE：FTRAN+定義ファイル／ msgTitle:メッセージタイトル
        'Description    :伝送ファイルをコピーする
        'Return         :0=成功、50=ファイルなし、100=コード変換失敗、200=コード区分異常（JIS改行あり）、
        '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
        'Create         :2010/01/13
        'Update         :
        '=====================================================================================
        fn_DEN_CPYTO_DISK = 100
        Dim strDIR As String
        strDIR = CurDir()

        '-----------------------------------------------------------
        'ファイルの存在チェック
        '-----------------------------------------------------------
        If Dir(strIN_FILE_NAME) = "" Then
            MessageBox.Show(String.Format(MSG0274W, strIN_FILE_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            fn_DEN_CPYTO_DISK = 50
            Exit Function
        End If

        ' まずローカルへコピーする
        Dim sDestFileName As String
        For nCounter As Integer = 1 To 100
            sDestFileName = Path.GetDirectoryName(strOUT_FILE_NAME) & "\" & Path.GetFileName(strIN_FILE_NAME)
            sDestFileName &= "." & Now.ToString("yyyyMMddHHmmssfffffff")
            Try
                File.Copy(strIN_FILE_NAME, sDestFileName, False)
                Exit For
            Catch ex As FileNotFoundException
                Return 400                      '出力ファイル作成失敗
            Catch ex As IOException
                ' ファイルがコピーできるまで繰り返す
            Catch ex As Exception
                Return 400                      '出力ファイル作成失敗
            End Try
        Next nCounter

        '-----------------------------------------------------------
        'コード変換処理
        'コード変換区分が 2 の場合はIBM形式 EBCDIC コードとみなし
        'JIS8コードへのコード変換を行う
        '-----------------------------------------------------------
        Select Case strCODE_KBN
            'Case "1", "3"     'EBCDIC
            Case "4"            'EBCDIC コード区分変更 2009/09/30 kakiwaki
                If Dir(strOUT_FILE_NAME) <> "" Then
                    Kill(strOUT_FILE_NAME)
                End If
                Dim strCMD As String
                Dim strTEIGI_FILE As String

                ChDir(FTRANP_Folder)
                strTEIGI_FILE = Path.Combine(FTRFolder, strP_FILE)

                ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                'strCMD = "FP /nwd/ cload " & FTRFolder & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & strIN_FILE_NAME & " " & strOUT_FILE_NAME & " ++" & strTEIGI_FILE
                strCMD = "FP /nwd/ cload " & FTRFolder & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & """" & strIN_FILE_NAME & """" & " " & """" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FILE & """"
                ' 2016/04/22 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                Dim ProcFT As New Process
                Dim ProcInfo As New ProcessStartInfo
                ProcInfo.FileName = FTRANP_Folder & "FP.EXE"
                ProcInfo.Arguments = strCMD.Substring(3)
                ProcInfo.WorkingDirectory = FTRANP_Folder
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                    fn_DEN_CPYTO_DISK = 0
                Else
                    fn_DEN_CPYTO_DISK = 100         'コード変換失敗
                    Exit Function
                End If
            Case Else        'JIS,JIS改
                If Dir(strOUT_FILE_NAME) <> "" Then
                    Kill(strOUT_FILE_NAME)
                End If

                Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
                intFILE_NO_1 = FreeFile()
                '2005/03/29   北銀より２００バイト目から２０バイトNULL値が設定されて入ってくる
                'NULL値が入ってきたら､スペースに置き換える
                FileOpen(intFILE_NO_1, strIN_FILE_NAME, OpenMode.Binary)   '入力ファイル
                intFILE_NO_2 = FreeFile()
                FileOpen(intFILE_NO_2, strOUT_FILE_NAME, OpenMode.Binary)   '出力ファイル
                Dim byt1 As Byte
                Dim dblREC As Double = 0
                Do Until EOF(intFILE_NO_1)
                    FileGet(intFILE_NO_1, byt1)
                    If byt1 = 0 Then
                        byt1 = 32
                    End If
                    FilePut(intFILE_NO_2, byt1)
                Loop
                FileClose(intFILE_NO_1)
                FileClose(intFILE_NO_2)
                If Err.Number <> 0 Then
                    fn_DEN_CPYTO_DISK = 400    '出力ファイル作成失敗
                End If

        End Select
        ChDir(strDIR)
        fn_DEN_CPYTO_DISK = 0

    End Function
#End Region
#Region " イベント"
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
              Handles txtJyusinDateY.Validating, txtJyusinDateM.Validating, txtJyusinDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub
#End Region
End Class