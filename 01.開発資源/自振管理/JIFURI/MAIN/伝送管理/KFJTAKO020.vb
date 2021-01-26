'Imports System.Data.OracleClient
Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJTAKO020
    Inherits System.Windows.Forms.Form
    Private MainLOG As New CASTCommon.BatchLOG("KFJTAKO020", "一括送信ファイル作成(元請)画面")
    Private Const msgTitle As String = "一括送信ファイル作成(元請)画面(KFJTAKO020)"
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

    Private KFJP038 As KFJP038  '帳票印刷クラス

    '帳票印刷用構造体
    Public Structure PrintData
        Public SyoriDate As String     '処理日
        Public Sousinsaki As String    '送信先
        Public Jikinko As String       '自金庫
        Public Iraisaki As String      '依頼先
        Public ItakuCode As String     '委託者コード
        Public Syumoku As String       '種目
        Public NSKin As String         '入出金
        Public SyoriKen As Long        '処理件数
        Public SyoriKin As Long        '処理金額
        Public FuriKen As Long         '振替件数
        Public FuriKin As Long         '振替金額
        Public FunouKen As Long        '不能件数
        Public FunouKin As Long        '不能金額
        Public FuriDate As Long        '振替日
    End Structure
    Public Print As PrintData
#Region "宣言"
    Dim clsFUSION As New clsFUSION.clsMain()
    Dim strKANRI_FILE As String

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

    Dim strFURI_DATE As String
    Dim strTORIS_CODE As String
    Dim strTORIF_CODE As String
    Dim strHENKAN_FILE As String

    Dim strHENKAN_TORIS_CODE(100) As String
    Dim strHENKAN_TORIF_CODE(100) As String
    Dim strHENKAN_ITAKU_NAME(100) As String
    Dim strHENKAN_FURI_DATE(100) As String
    Dim strHENKAN_FILE_NAME(100) As String
    Dim strHENKAN_BKFILE_NAME(100) As String
    Dim strTAKOU_TORIS_CODE(100) As String
    Dim strTAKOU_TORIF_CODE(100) As String
    Dim strTAKOU_ITAKU_NAME(100) As String
    Dim strTAKOU_FILE_NAME(100) As String
    Dim strTAKOU_BKFILE_NAME(100) As String

    Dim strSOUSIN_FILE As String
    Dim strSOUSIN_FILE_JIS As String
    Dim lngRECORD_COUNT As Long
    Dim lngKOBETU_RECORD_COUNT As Long
    Dim strFURI_DATA As String

    '帳票印字
    Public lngREC_MEI As Long
    Public strFURI_DATA_KBN As String
    Public dblFURI_KEN As Double
    Public dblFURI_KIN As Double
    Public dblFUNOU_KEN As Double
    Public dblFUNOU_KIN As Double
    Public dblSYORI_KEN As Double
    Public dblSYORI_KIN As Double
    Public flgPRNT As Boolean

    Private DENBKFolder As String
    Private DATFolder As String
    Private DATBKFolder As String
    Private FTRFolder As String
    Private TXTFolder As String
    Private TAKFolder As String
    Private JikinkoCd As String
    Private JikinkoName As String

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
#Region " ロード"
    Private Sub KFJTAKO020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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

            '休日マスタ取り込み
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)終了", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            'iniファイル読み込み
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時に振替日に前営業日を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strSysDate As String
            Dim strGetdate As String = ""

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            bRet = GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)

            txtHenkanDateY.Text = strGetdate.Substring(0, 4)
            txtHenkanDateM.Text = strGetdate.Substring(4, 2)
            txtHenkanDateD.Text = strGetdate.Substring(6, 2)

            '受信ファイル名リストボックスにをファイル名表示
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
                If Line(6) = "S" Then   '送信

                    cmbFileName.Items.Add(Line(8))
                End If
            Loop
            FileReader.Close()
            cmbFileName.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

#End Region
#Region " 読込"
    Private Sub btnRead_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRead.Click
        Dim SQL As New StringBuilder(128)
        MainDB = New MyOracle
        Dim OraReader As MyOracleReader = Nothing
        Try
            MainDB = New MyOracle
            OraReader = New MyOracleReader(MainDB)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(読込)開始", "成功", "")
            'テキストボックスの入力チェック
            If fn_check_text() = False Then
                Exit Sub
            End If

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
                Me.DEN_TEXT.FILE_NAME = Line(7)
                Me.DEN_TEXT.TITLE = Line(8)
                Me.DEN_TEXT.CENTER_NAME = Line(9)
                Me.DEN_TEXT.CENTER_KIN = Line(10)
                If Me.DEN_TEXT.SOUJYUSIN = "S" Then
                    If Me.DEN_TEXT.TITLE = cmbFileName.SelectedItem Then
                        Exit Do
                    End If
                End If
            Loop
            'Do Until FileReader.EndOfStream
            '    Line = FileReader.ReadLine.Split(","c)
            '    strKANRI_NO = Line(0)
            '    strFMT_KBN = Line(1)
            '    intRECORD_LEN = Line(2)
            '    strCODE_KBN = Line(3)
            '    strMULTI_KBN = Line(4)
            '    strKIDOU_KBN = Line(5)
            '    strSOUJYUSIN = Line(6)
            '    strFILE_NAME = Line(7)
            '    strTITLE = Line(8)
            '    strSENTER_NAME = Line(9)
            '    strSENTER_KIN = Line(10)
            '    If strSOUJYUSIN = "S" Then
            '        If strTITLE = cmbFileName.SelectedItem Then
            '            Exit Do
            '        End If
            '    End If
            'Loop
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
            FileReader.Close()
            strFURI_DATE = txtHenkanDateY.Text & txtHenkanDateM.Text & txtHenkanDateD.Text

            '-----------------------------------------------------------
            'まとめ対象取引先（返還）の検索　&　リストボックスにアイテム追加
            '-----------------------------------------------------------
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            SQL.Append(" SELECT * ")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE HENKAN_FLG_S = '1'")
            SQL.Append(" AND BAITAI_CODE_T = '00'")
            SQL.Append(" AND FMT_KBN_T = " & SQ(Me.DEN_TEXT.FMT_KBN))
            SQL.Append(" AND UPPER(SUBSTR(FILE_NAME_T,1," & Len(Trim(Me.DEN_TEXT.FILE_NAME)) & ")) = " & SQ(Trim(Me.DEN_TEXT.FILE_NAME.ToUpper)))
            SQL.Append(" AND FURI_DATE_S = " & SQ(strFURI_DATE)) '2009/11/18 振替日追加
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            'SQL.Append(" SELECT * ")
            'SQL.Append(" FROM TORIMAST,SCHMAST")
            'SQL.Append(" WHERE HENKAN_FLG_S = '1'")
            'SQL.Append(" AND BAITAI_CODE_T = '00'")
            'SQL.Append(" AND FMT_KBN_T = " & SQ(strFMT_KBN))
            'SQL.Append(" AND UPPER(SUBSTR(FILE_NAME_T,1," & Len(Trim(strFILE_NAME)) & ")) = " & SQ(Trim(strFILE_NAME.ToUpper)))
            'SQL.Append(" AND FURI_DATE_S = " & SQ(strFURI_DATE)) '2009/11/18 振替日追加
            'SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            'SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

            '読込のみ
            lsbKekka.Items.Clear()
            Dim intHENKAN_COUNT As Integer = 0
            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    strTORIS_CODE = GCom.NzStr(OraReader.GetString("TORIS_CODE_T"))
                    strTORIF_CODE = GCom.NzStr(OraReader.GetString("TORIF_CODE_T"))

                    '20130204 maeda マルチを考慮
                    'strHENKAN_FILE = DenFolder & "O" & strTORIS_CODE & strTORIF_CODE & ".DAT"
                    If OraReader.GetString("MULTI_KBN_T") = "0" Then
                        strHENKAN_FILE = DenFolder & "O" & strTORIS_CODE & strTORIF_CODE & ".DAT"
                    Else
                        strHENKAN_FILE = DenFolder & "O" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT"
                    End If
                    '20130204 maeda マルチを考慮

                    If Dir(strHENKAN_FILE) <> "" Then
                        intHENKAN_COUNT += 1
                        strHENKAN_TORIS_CODE(intHENKAN_COUNT) = strTORIS_CODE
                        strHENKAN_TORIF_CODE(intHENKAN_COUNT) = strTORIF_CODE
                        strHENKAN_ITAKU_NAME(intHENKAN_COUNT) = GCom.NzStr(OraReader.GetString("ITAKU_NNAME_T"))
                        strHENKAN_FURI_DATE(intHENKAN_COUNT) = GCom.NzStr(OraReader.GetString("FURI_DATE_S"))
                        strHENKAN_FILE_NAME(intHENKAN_COUNT) = strHENKAN_FILE
                        '20130204 maeda マルチを考慮
                        'strHENKAN_BKFILE_NAME(intHENKAN_COUNT) = DENBKFolder & "O" & strTORIS_CODE & strTORIF_CODE & ".DAT"
                        If OraReader.GetString("MULTI_KBN_T") = "0" Then
                            strHENKAN_BKFILE_NAME(intHENKAN_COUNT) = DENBKFolder & "O" & strTORIS_CODE & strTORIF_CODE & ".DAT"
                        Else
                            strHENKAN_BKFILE_NAME(intHENKAN_COUNT) = DENBKFolder & "O" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT"
                        End If
                        lsbKekka.Items.Add(strHENKAN_TORIS_CODE(intHENKAN_COUNT) & "-" & strHENKAN_TORIF_CODE(intHENKAN_COUNT) & Space(3) & strHENKAN_ITAKU_NAME(intHENKAN_COUNT))
                        '20130204 maeda マルチを考慮
                    End If
                    OraReader.NextRead()
                End While
            End If
            OraReader.Close()

            '-----------------------------------------------------------
            'まとめ対象取引先（依頼）の検索　&　リストボックスにアイテム追加
            '-----------------------------------------------------------
            lsbIrai.Items.Clear()
            Dim intTAKOU_COUNT As Integer = 0
            Dim strFOLDER As String
            Dim strFILE As String
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            strFOLDER = Path.Combine(DenFolder, Me.DEN_TEXT.CENTER_KIN)
            'フォルダの存在確認
            If Directory.Exists(strFOLDER) Then
                For Each strFILE In System.IO.Directory.GetFiles(strFOLDER)
                    If System.IO.Path.GetFileName(strFILE).Substring(0, 1) = "T" And System.IO.Path.GetFileName(strFILE).Substring(System.IO.Path.GetFileName(strFILE).Length - 3, 3) = Me.DEN_TEXT.RECORD_LEN Then
                        strTORIS_CODE = System.IO.Path.GetFileName(strFILE).Substring(1, 10)
                        strTORIF_CODE = System.IO.Path.GetFileName(strFILE).Substring(11, 2)
                        SQL = New StringBuilder(128)
                        SQL.Append(" SELECT * ")
                        SQL.Append(" FROM TORIMAST")
                        SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
                        SQL.Append(" AND TORIF_CODE_T =" & SQ(strTORIF_CODE))

                        If OraReader.DataReader(SQL) = True Then

                            intTAKOU_COUNT += 1
                            strTAKOU_TORIS_CODE(intTAKOU_COUNT) = strTORIS_CODE
                            strTAKOU_TORIF_CODE(intTAKOU_COUNT) = strTORIF_CODE
                            strTAKOU_ITAKU_NAME(intTAKOU_COUNT) = GCom.NzStr(OraReader.GetString("ITAKU_NNAME_T"))
                            strTAKOU_FILE_NAME(intTAKOU_COUNT) = Path.Combine(strFOLDER, System.IO.Path.GetFileName(strFILE))
                            strTAKOU_BKFILE_NAME(intTAKOU_COUNT) = DENBKFolder & "T" & Me.DEN_TEXT.CENTER_KIN & strTORIS_CODE & strTORIF_CODE & "." & Me.DEN_TEXT.RECORD_LEN
                            lsbIrai.Items.Add(strTAKOU_TORIS_CODE(intTAKOU_COUNT) & "-" & strTAKOU_TORIF_CODE(intTAKOU_COUNT) & Space(3) & strTAKOU_ITAKU_NAME(intTAKOU_COUNT))
                        Else
                            String.Format(MSG0278W, strTORIS_CODE, strTORIF_CODE)
                            MessageBox.Show(String.Format(MSG0278W, strTORIS_CODE, strTORIF_CODE), msgTitle, _
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                        OraReader.Close()
                    End If
                Next
            End If
            'strFOLDER = Path.Combine(DenFolder, strSENTER_KIN)
            ''フォルダの存在確認
            'If clsFUSION.fn_CHECK_DIR(DenFolder, strSENTER_KIN) Then
            '    For Each strFILE In System.IO.Directory.GetFiles(strFOLDER)
            '        If System.IO.Path.GetFileName(strFILE).Substring(0, 1) = "T" And System.IO.Path.GetFileName(strFILE).Substring(System.IO.Path.GetFileName(strFILE).Length - 3, 3) = CStr(intRECORD_LEN) Then
            '            strTORIS_CODE = System.IO.Path.GetFileName(strFILE).Substring(1, 10)
            '            strTORIF_CODE = System.IO.Path.GetFileName(strFILE).Substring(11, 2)
            '            SQL = New StringBuilder(128)
            '            SQL.Append(" SELECT * ")
            '            SQL.Append(" FROM TORIMAST")
            '            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
            '            SQL.Append(" AND TORIF_CODE_T =" & SQ(strTORIF_CODE))

            '            If OraReader.DataReader(SQL) = True Then

            '                intTAKOU_COUNT += 1
            '                strTAKOU_TORIS_CODE(intTAKOU_COUNT) = strTORIS_CODE
            '                strTAKOU_TORIF_CODE(intTAKOU_COUNT) = strTORIF_CODE
            '                strTAKOU_ITAKU_NAME(intTAKOU_COUNT) = GCom.NzStr(OraReader.GetString("ITAKU_NNAME_T"))
            '                strTAKOU_FILE_NAME(intTAKOU_COUNT) = Path.Combine(strFOLDER, System.IO.Path.GetFileName(strFILE))
            '                strTAKOU_BKFILE_NAME(intTAKOU_COUNT) = DENBKFolder & "T" & strSENTER_KIN & strTORIS_CODE & strTORIF_CODE & "." & CStr(intRECORD_LEN)
            '                lsbIrai.Items.Add(strTAKOU_TORIS_CODE(intTAKOU_COUNT) & "-" & strTAKOU_TORIF_CODE(intTAKOU_COUNT) & Space(3) & strTAKOU_ITAKU_NAME(intTAKOU_COUNT))
            '            Else
            '                String.Format(MSG0278W, strTORIS_CODE, strTORIF_CODE)
            '                MessageBox.Show(String.Format(MSG0278W, strTORIS_CODE, strTORIF_CODE), msgTitle, _
            '                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '            End If
            '            OraReader.Close()
            '        End If
            '    Next
            'End If
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

            '対象がない場合はメッセージを表示
            If intHENKAN_COUNT = 0 AndAlso intTAKOU_COUNT = 0 Then
                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(読込)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(読込)終了", "成功", "")
        End Try
    End Sub

#End Region
#Region " 実行"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim I As Integer
        Dim EntryStream As FileStream
        Dim FileWriter As StreamWriter
        Dim InputCnt As Long
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")
            If lsbIrai.Items.Count = 0 And lsbKekka.Items.Count = 0 Then
                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If MessageBox.Show(String.Format(MSG0015I, "一括送信ファイル作成"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.No Then
                Return
            End If

            strSOUSIN_FILE_JIS = Path.Combine(DENBKFolder, "SYUUKEI_SOUSIN_JIS.DAT")
            If Dir(strSOUSIN_FILE_JIS) <> "" Then
                Kill(strSOUSIN_FILE_JIS)
            End If
            '----------------------------
            '送信ファイルのオープン
            '----------------------------
            FileWriter = New StreamWriter(strSOUSIN_FILE_JIS, False, System.Text.Encoding.GetEncoding("Shift-JIS"))  '書込みファイル
            lngRECORD_COUNT = 0
            '----------------------------
            '依頼分のデータ書込み
            '----------------------------
            For I = 1 To lsbIrai.Items.Count
                EntryStream = New FileStream(strTAKOU_FILE_NAME(I), FileMode.Open, FileAccess.Read) '読込ファイル
                Dim br As BinaryReader = New BinaryReader(EntryStream)
                Dim FileLen As Integer = EntryStream.Length
                Dim _cnt As Long = 0
                Do
                    '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
                    'テキストファイルの内容を構造体で管理
                    lngKOBETU_RECORD_COUNT += 1
                    lngRECORD_COUNT += 1
                    br.BaseStream.Seek(_cnt, SeekOrigin.Begin)
                    Select Case GCom.NzInt(Me.DEN_TEXT.RECORD_LEN)
                        Case 120
                            strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(120))
                        Case 220
                            strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(220))
                    End Select
                    Select Case strFURI_DATA.Substring(0, 1)
                        Case "1", "2", "8"
                            Select Case GCom.NzInt(Me.DEN_TEXT.RECORD_LEN)
                                Case 120
                                    FileWriter.Write(strFURI_DATA)
                                Case 220
                                    FileWriter.Write(strFURI_DATA)
                            End Select
                        Case "9"
                            If Me.DEN_TEXT.MULTI_KBN = "0" Then  '擬似マルチ　　12･･･8912･･･89
                                Select Case GCom.NzInt(Me.DEN_TEXT.RECORD_LEN)
                                    Case 120
                                        FileWriter.Write(strFURI_DATA)
                                    Case 220
                                        FileWriter.Write(strFURI_DATA)
                                End Select
                            Else
                                lngRECORD_COUNT -= 1
                            End If
                    End Select
                    _cnt += GCom.NzInt(Me.DEN_TEXT.RECORD_LEN)

                    'lngKOBETU_RECORD_COUNT += 1
                    'lngRECORD_COUNT += 1
                    'br.BaseStream.Seek(_cnt, SeekOrigin.Begin)
                    'Select Case intRECORD_LEN
                    '    Case 120
                    '        strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(120))
                    '    Case 220
                    '        strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(220))
                    'End Select
                    'Select Case strFURI_DATA.Substring(0, 1)
                    '    Case "1", "2", "8"
                    '        Select Case intRECORD_LEN
                    '            Case 120
                    '                FileWriter.Write(strFURI_DATA)
                    '            Case 220
                    '                FileWriter.Write(strFURI_DATA)
                    '        End Select
                    '    Case "9"
                    '        If strMULTI_KBN = "0" Then  '擬似マルチ　　12･･･8912･･･89
                    '            Select Case intRECORD_LEN
                    '                Case 120
                    '                    FileWriter.Write(strFURI_DATA)
                    '                Case 220
                    '                    FileWriter.Write(strFURI_DATA)
                    '            End Select
                    '        Else
                    '            lngRECORD_COUNT -= 1
                    '        End If
                    'End Select
                    '_cnt += intRECORD_LEN
                    '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
                Loop While FileLen > _cnt
                EntryStream.Close()
            Next
            '----------------------------
            '返還分のデータ書込み
            '----------------------------
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            For I = 1 To lsbKekka.Items.Count
                EntryStream = New FileStream(strHENKAN_FILE_NAME(I), FileMode.Open, FileAccess.Read) '読込ファイル
                Dim br As BinaryReader = New BinaryReader(EntryStream)
                Dim FileLen As Integer = EntryStream.Length
                Dim _cnt As Long = 0
                lngKOBETU_RECORD_COUNT = 0
                Do
                    lngKOBETU_RECORD_COUNT += 1
                    lngRECORD_COUNT += 1
                    br.BaseStream.Seek(_cnt, SeekOrigin.Begin)
                    Select Case GCom.NzInt(Me.DEN_TEXT.RECORD_LEN)
                        Case 120
                            strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(120))
                        Case 220
                            strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(220))
                    End Select
                    Select Case strFURI_DATA.Substring(0, 1)
                        Case "1", "2", "8"
                            Select Case GCom.NzInt(Me.DEN_TEXT.RECORD_LEN)
                                Case 120
                                    FileWriter.Write(strFURI_DATA)
                                Case 220
                                    FileWriter.Write(strFURI_DATA)
                            End Select
                        Case "9"
                            If Me.DEN_TEXT.MULTI_KBN = "0" Then  '擬似マルチ　　12･･･8912･･･89
                                Select Case GCom.NzInt(Me.DEN_TEXT.RECORD_LEN)
                                    Case 120
                                        FileWriter.Write(strFURI_DATA)
                                    Case 220
                                        FileWriter.Write(strFURI_DATA)
                                End Select
                            Else
                                lngRECORD_COUNT -= 1
                            End If
                    End Select
                    _cnt += GCom.NzInt(Me.DEN_TEXT.RECORD_LEN)
                Loop While FileLen > _cnt
                EntryStream.Close()
            Next
            If Me.DEN_TEXT.MULTI_KBN = "1" Then  'マルチ　　12･･･812･･･89
                lngRECORD_COUNT += 1
                Select Case GCom.NzInt(Me.DEN_TEXT.RECORD_LEN)
                    Case 120
                        strFURI_DATA = "9" & Space(119)
                        FileWriter.Write(strFURI_DATA)
                End Select
            End If

            'For I = 1 To lsbKekka.Items.Count
            '    EntryStream = New FileStream(strHENKAN_FILE_NAME(I), FileMode.Open, FileAccess.Read) '読込ファイル
            '    Dim br As BinaryReader = New BinaryReader(EntryStream)
            '    Dim FileLen As Integer = EntryStream.Length
            '    Dim _cnt As Long = 0
            '    lngKOBETU_RECORD_COUNT = 0
            '    Do
            '        lngKOBETU_RECORD_COUNT += 1
            '        lngRECORD_COUNT += 1
            '        br.BaseStream.Seek(_cnt, SeekOrigin.Begin)
            '        Select Case intRECORD_LEN
            '            Case 120
            '                strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(120))
            '            Case 220
            '                strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(220))
            '        End Select
            '        Select Case strFURI_DATA.Substring(0, 1)
            '            Case "1", "2", "8"
            '                Select Case intRECORD_LEN
            '                    Case 120
            '                        FileWriter.Write(strFURI_DATA)
            '                    Case 220
            '                        FileWriter.Write(strFURI_DATA)
            '                End Select
            '            Case "9"
            '                If strMULTI_KBN = "0" Then  '擬似マルチ　　12･･･8912･･･89
            '                    Select Case intRECORD_LEN
            '                        Case 120
            '                            FileWriter.Write(strFURI_DATA)
            '                        Case 220
            '                            FileWriter.Write(strFURI_DATA)
            '                    End Select
            '                Else
            '                    lngRECORD_COUNT -= 1
            '                End If
            '        End Select
            '        _cnt += intRECORD_LEN
            '    Loop While FileLen > _cnt
            '    EntryStream.Close()
            'Next
            'If strMULTI_KBN = "1" Then  'マルチ　　12･･･812･･･89
            '    lngRECORD_COUNT += 1
            '    Select Case intRECORD_LEN
            '        Case 120
            '            strFURI_DATA = "9" & Space(119)
            '            FileWriter.Write(strFURI_DATA)
            '    End Select
            'End If
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

            FileWriter.Close()
            InputCnt = lngRECORD_COUNT
            '----------------------------
            'ファイルのコード変換
            '----------------------------
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            strSOUSIN_FILE_JIS = Path.Combine(DENBKFolder, "SYUUKEI_SOUSIN_JIS.DAT")
            strSOUSIN_FILE = Path.Combine(DenFolder, Me.DEN_TEXT.FILE_NAME)
            If Dir(strSOUSIN_FILE) <> "" Then
                Kill(strSOUSIN_FILE)
            End If
            Dim strFILE_CODE_KBN As String = 0
            Dim intKEKKA As Integer
            Dim strP_FILE As String = String.Empty
            Select Case GCom.NzInt(Me.DEN_TEXT.RECORD_LEN)
                Case 120
                    Select Case Me.DEN_TEXT.CODE_KBN
                        Case "J"
                            strFILE_CODE_KBN = "0"
                            strP_FILE = "120JIS→JIS.P"  'JIS→JIS改
                        Case "E"
                            strFILE_CODE_KBN = "4"
                            strP_FILE = "120.P"         'JIS→EBCDIC
                    End Select
                Case 220
                    Select Case Me.DEN_TEXT.CODE_KBN
                        Case "J"
                            strFILE_CODE_KBN = "0"
                            strP_FILE = "220JIS→JIS.P"  'JIS→JIS改
                        Case "E"
                            strFILE_CODE_KBN = "4"
                            strP_FILE = "220.P"         'JIS→EBCDIC
                    End Select
            End Select
            intKEKKA = clsFUSION.fn_DISK_CPYTO_DEN("", strSOUSIN_FILE_JIS, strSOUSIN_FILE, _
                                                   GCom.NzInt(Me.DEN_TEXT.RECORD_LEN), strFILE_CODE_KBN, strP_FILE)

            Select Case intKEKKA
                Case 0
                Case 100
                    'Return         :0=成功、100=コード変換失敗
                    MessageBox.Show(MSG0019E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
            End Select

            'strSOUSIN_FILE_JIS = Path.Combine(DENBKFolder, "SYUUKEI_SOUSIN_JIS.DAT")
            'strSOUSIN_FILE = Path.Combine(DenFolder, strFILE_NAME)
            'If Dir(strSOUSIN_FILE) <> "" Then
            '    Kill(strSOUSIN_FILE)
            'End If
            'Dim strFILE_CODE_KBN As String = 0
            'Dim intKEKKA As Integer
            ''2013/10/24 saitou 標準修正 ADD -------------------------------------------------->>>>
            'Dim strP_FILE As String = String.Empty
            ''2013/10/24 saitou 標準修正 ADD --------------------------------------------------<<<<
            'Select Case intRECORD_LEN
            '    Case 120
            '        Select Case strCODE_KBN
            '            Case "J"
            '                strFILE_CODE_KBN = "0"
            '                strP_FILE = "120JIS→JIS.P"  'JIS→JIS改
            '            Case "E"
            '                strFILE_CODE_KBN = "4"
            '                strP_FILE = "120.P"         'JIS→EBCDIC
            '        End Select
            '    Case 220
            '        Select Case strCODE_KBN
            '            Case "J"
            '                strFILE_CODE_KBN = "0"
            '                strP_FILE = "220JIS→JIS.P"  'JIS→JIS改
            '            Case "E"
            '                strFILE_CODE_KBN = "4"
            '                strP_FILE = "220.P"         'JIS→EBCDIC
            '        End Select
            'End Select
            'intKEKKA = clsFUSION.fn_DISK_CPYTO_DEN("", strSOUSIN_FILE_JIS, strSOUSIN_FILE, _
            '                                       intRECORD_LEN, strFILE_CODE_KBN, strP_FILE)

            'Select Case intKEKKA
            '    Case 0
            '    Case 100
            '        'Return         :0=成功、100=コード変換失敗
            '        MessageBox.Show(MSG0019E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '        Exit Sub
            'End Select
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

            '口座振替データ送信報告書を印刷する
            KFJP038 = New KFJP038
            Dim DataList As New ArrayList
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            Select Case GCom.NzInt(Me.DEN_TEXT.RECORD_LEN)
                Case 120
                    If MakeCSV_Zengin(strSOUSIN_FILE_JIS, DataList) = False Then
                        MessageBox.Show(String.Format(MSG0231W, "口座振替データ送信報告書"), msgTitle, _
                                         MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                Case 220
                    If MakeCSV_220(strSOUSIN_FILE_JIS, DataList) = False Then
                        MessageBox.Show(String.Format(MSG0231W, "口座振替データ送信報告書"), msgTitle, _
                                         MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
            End Select
            'Select Case intRECORD_LEN
            '    Case 120
            '        If MakeCSV_Zengin(strSOUSIN_FILE_JIS, DataList) = False Then
            '            MessageBox.Show(String.Format(MSG0231W, "口座振替データ送信報告書"), msgTitle, _
            '                             MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '            Return
            '        End If
            '    Case 220
            '        If MakeCSV_220(strSOUSIN_FILE_JIS, DataList) = False Then
            '            MessageBox.Show(String.Format(MSG0231W, "口座振替データ送信報告書"), msgTitle, _
            '                             MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '            Return
            '        End If
            'End Select
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

            Dim CsvFile As String = KFJP038.CreateCsvFile
            For No As Integer = 0 To DataList.Count - 1
                Print = CType(DataList.Item(No), PrintData)
                KFJP038.OutputCsvData(Print.SyoriDate)      '処理日
                KFJP038.OutputCsvData(Print.Sousinsaki)     '送信先
                KFJP038.OutputCsvData(Print.Jikinko)        '自金庫
                KFJP038.OutputCsvData(Print.Iraisaki)       '依頼先
                KFJP038.OutputCsvData(Print.ItakuCode)      '委託者コード
                KFJP038.OutputCsvData(Print.Syumoku)        '種目
                KFJP038.OutputCsvData(Print.NSKin)          '入出金
                KFJP038.OutputCsvData(Print.SyoriKen)       '処理件数
                KFJP038.OutputCsvData(Print.SyoriKin)       '処理金額
                KFJP038.OutputCsvData(Print.FuriKen)        '振替件数
                KFJP038.OutputCsvData(Print.FuriKin)        '振替金額
                KFJP038.OutputCsvData(Print.FunouKen)       '不能件数
                KFJP038.OutputCsvData(Print.FunouKin)       '不能金額
                KFJP038.OutputCsvData(Print.FuriDate)       '振替日
                KFJP038.OutputCsvData(InputCnt, False, True) 'レコード件数
            Next
            KFJP038.CloseCsv()
            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me
            'パラメータ設定：ログイン名、ＣＳＶファイル名
            Dim Param As String = LW.UserID & "," & CsvFile
            Dim nRet As Integer = ExeRepo.ExecReport("KFJP038.EXE", Param)
            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(String.Format(MSG0106W, "口座振替データ送信報告書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "口座振替データ送信報告書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
            End Select


            'ファイルの移動
            For I = 1 To lsbIrai.Items.Count
                If Dir(strTAKOU_BKFILE_NAME(I)) <> "" Then
                    Kill(strTAKOU_BKFILE_NAME(I))
                End If
                FileCopy(strTAKOU_FILE_NAME(I), strTAKOU_BKFILE_NAME(I))
                Kill(strTAKOU_FILE_NAME(I))
            Next
            For I = 1 To lsbKekka.Items.Count
                If Dir(strHENKAN_BKFILE_NAME(I)) <> "" Then
                    Kill(strHENKAN_BKFILE_NAME(I))
                End If
                FileCopy(strHENKAN_FILE_NAME(I), strHENKAN_BKFILE_NAME(I))
                Kill(strHENKAN_FILE_NAME(I))
            Next
            MessageBox.Show(String.Format(MSG0048I, strSOUSIN_FILE), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            '2014/04/04 saitou 奈良信金 MODIFY ----------------------------------------------->>>>
            '一覧をクリアする
            '（画面に残っている状態で再度実行ボタンを押下すると異常終了するため）
            Me.lsbIrai.Items.Clear()
            Me.lsbKekka.Items.Clear()
            '2014/04/04 saitou 奈良信金 MODIFY -----------------------------------------------<<<<

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
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
            If txtHenkanDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtHenkanDateY.Focus()
                Return False
            End If
            '月必須チェック
            If txtHenkanDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtHenkanDateM.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(txtHenkanDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtHenkanDateM.Text.Trim) > 12 Then '(MSG0022W)
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtHenkanDateM.Focus()
                Return False
            End If
            '日付必須チェック
            If txtHenkanDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtHenkanDateD.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(txtHenkanDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtHenkanDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtHenkanDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = txtHenkanDateY.Text & "/" & txtHenkanDateM.Text & "/" & txtHenkanDateD.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtHenkanDateY.Focus()
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

        'DEN格納フォルダ
        DenFolder = CASTCommon.GetFSKJIni("COMMON", "DEN")
        If DenFolder.ToUpper = "ERR" OrElse DenFolder = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "DEN格納フォルダ", "COMMON", "DEN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("設定ファイル取得", "失敗", "項目名: 分類:COMMON 項目:")
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
        Return True
    End Function

#End Region

#Region " 帳票印刷データ作成"
    Private Function MakeCSV_Zengin(ByVal FileName As String, ByRef DataList As ArrayList) As Boolean

        Dim gZENGIN_REC1 As CAstFormat.CFormatZengin.ZGRECORD1 = Nothing
        Dim gZENGIN_REC2 As CAstFormat.CFormatZengin.ZGRECORD2 = Nothing
        Dim gZENGIN_REC8 As CAstFormat.CFormatZengin.ZGRECORD8 = Nothing
        Dim gZENGIN_REC9 As CAstFormat.CFormatZengin.ZGRECORD9 = Nothing
        Dim EntryStream As FileStream = Nothing
        Dim Msg As String = ""
        Try
            '印刷情報設定
            Print = New PrintData
            Print.SyoriDate = Now.ToString("yyyyMMdd")
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            Print.Sousinsaki = Me.DEN_TEXT.CENTER_NAME
            'Print.Sousinsaki = strSENTER_NAME
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<
            Print.Jikinko = JikinkoName '自金庫名
            'ファイル読み取り情報設定
            EntryStream = New FileStream(FileName, FileMode.Open, FileAccess.Read) '読込ファイル
            Dim br As BinaryReader = New BinaryReader(EntryStream)
            Dim FileLen As Integer = EntryStream.Length
            Dim _cnt As Long = 0
            Do
                lngKOBETU_RECORD_COUNT += 1
                lngRECORD_COUNT += 1
                br.BaseStream.Seek(_cnt, SeekOrigin.Begin)


                strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(120))
                Select Case strFURI_DATA.Substring(0, 1)
                    Case "1"
                        gZENGIN_REC1.Data = strFURI_DATA
                        strFURI_DATA_KBN = "1"
                        dblFURI_KEN = 0
                        dblFURI_KIN = 0
                        dblFUNOU_KEN = 0
                        dblFUNOU_KIN = 0
                        dblSYORI_KEN = 0
                        dblSYORI_KIN = 0
                    Case "2"
                        gZENGIN_REC2.Data = strFURI_DATA
                        If gZENGIN_REC2.ZG14 <> "0" Then
                            dblFUNOU_KEN += 1
                            dblFUNOU_KIN += Val(gZENGIN_REC2.ZG10)
                            dblSYORI_KEN += 1
                            dblSYORI_KIN += Val(gZENGIN_REC2.ZG10)
                        Else
                            dblFURI_KEN += 1
                            dblFURI_KIN += Val(gZENGIN_REC2.ZG10)
                            dblSYORI_KEN += 1
                            dblSYORI_KIN += Val(gZENGIN_REC2.ZG10)
                        End If

                        strFURI_DATA_KBN = "2"
                    Case "8"
                        'If strFURI_DATA_KBN <> "2" Then
                        '    Exit Select
                        'End If
                        gZENGIN_REC8.Data = strFURI_DATA
                        strFURI_DATA_KBN = "8"
                        'If dblSYORI_KEN <> Val(gZENGIN_REC8.ZG2) Then
                        '    Msg = String.Format(MSG0327W, "処理件数", gZENGIN_REC1.ZG4, dblSYORI_KEN.ToString("#,##0"), Val(gZENGIN_REC8.ZG2).ToString("#,##0"))
                        '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        '    Exit Function
                        'End If
                        'If dblSYORI_KIN <> Val(gZENGIN_REC8.ZG3) Then
                        '    Msg = String.Format(MSG0327W, "処理金額", gZENGIN_REC1.ZG4, dblSYORI_KIN.ToString("#,##0"), Val(gZENGIN_REC8.ZG3).ToString("#,##0"))
                        '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        '    Exit Function
                        'End If
                        If (gZENGIN_REC8.ZG4.Trim = "" Or Val(gZENGIN_REC8.ZG4.Trim = 0)) And (gZENGIN_REC8.ZG6.Trim = "" Or Val(gZENGIN_REC8.ZG6.Trim = 0)) Then '依頼
                            If (gZENGIN_REC8.ZG2.Trim = "" Or Val(gZENGIN_REC8.ZG2.Trim = 0)) Then
                                Print.Syumoku = "結果" '種目(0件データ対応)
                            Else
                                Print.Syumoku = "依頼" '種目
                            End If
                            dblFURI_KEN = 0
                            dblFURI_KIN = 0
                            dblFUNOU_KEN = 0
                            dblFUNOU_KIN = 0
                        Else   '結果
                            Print.Syumoku = "結果" '種目
                            'If dblFURI_KEN <> Val(gZENGIN_REC8.ZG4) Then
                            '    Msg = String.Format(MSG0327W, "振替済件数", gZENGIN_REC1.ZG4, dblFURI_KEN.ToString("#,##0"), Val(gZENGIN_REC8.ZG4).ToString("#,##0"))
                            '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                            'If dblFURI_KIN <> Val(gZENGIN_REC8.ZG5) Then
                            '    Msg = String.Format(MSG0327W, "振替済金額", gZENGIN_REC1.ZG4, dblFURI_KIN.ToString("#,##0"), Val(gZENGIN_REC8.ZG5).ToString("#,##0"))
                            '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                            'If dblFUNOU_KEN <> Val(gZENGIN_REC8.ZG6) Then
                            '    Msg = String.Format(MSG0327W, "不能件数", gZENGIN_REC1.ZG4, dblFUNOU_KEN.ToString("#,##0"), Val(gZENGIN_REC8.ZG6).ToString("#,##0"))
                            '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                            'If dblFUNOU_KIN <> Val(gZENGIN_REC8.ZG7) Then
                            '    Msg = String.Format(MSG0327W, "不能金額", gZENGIN_REC1.ZG4, dblFUNOU_KIN.ToString("#,##0"), Val(gZENGIN_REC8.ZG7).ToString("#,##0"))
                            '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                        End If

                        Print.Iraisaki = gZENGIN_REC1.ZG5   '振替依頼先名
                        Print.ItakuCode = gZENGIN_REC1.ZG4  '委託者コード
                        Select Case gZENGIN_REC1.ZG2 '入出金
                            Case "91"
                                Print.NSKin = "出金"
                            Case "11", "21", "71"
                                Print.NSKin = "入金"
                            Case Else
                                Print.NSKin = ""
                        End Select
                        Print.SyoriKen = gZENGIN_REC8.ZG2
                        Print.SyoriKin = gZENGIN_REC8.ZG3
                        Print.FuriKen = gZENGIN_REC8.ZG4
                        Print.FuriKin = gZENGIN_REC8.ZG5
                        Print.FunouKen = gZENGIN_REC8.ZG6
                        Print.FunouKin = gZENGIN_REC8.ZG7
                        Print.FuriDate = Now.ToString("yyyy") & gZENGIN_REC1.ZG6   '振替日(MM/dd形式とするため年を仮に追加)
                        DataList.Add(Print)
                    Case "9"
                        strFURI_DATA_KBN = "9"
                End Select
                _cnt += 120
            Loop While FileLen > _cnt
            EntryStream.Close()
            Return True
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ファイル読み取り", "失敗", ex.ToString)
        Finally
            If Not EntryStream Is Nothing Then EntryStream.Close()
        End Try

    End Function
    Private Function MakeCSV_220(ByVal FileName As String, ByRef DataList As ArrayList) As Boolean

     
        Dim g220_REC1 As CAstFormat.CFormatZeikin220.ZEIKIN_RECORD1 = Nothing
        Dim g220_REC2 As CAstFormat.CFormatZeikin220.ZEIKIN_RECORD2 = Nothing
        Dim g220_REC8 As CAstFormat.CFormatZeikin220.ZEIKIN_RECORD8 = Nothing
        Dim g220_REC9 As CAstFormat.CFormatZeikin220.ZEIKIN_RECORD9 = Nothing
        Dim EntryStream As FileStream = Nothing
        Dim Msg As String = ""
        Try
            '印刷情報設定
            Print = New PrintData
            Print.SyoriDate = Now.ToString("yyyyMMdd")
            '2014/05/21 saitou 標準版 UPD -------------------------------------------------->>>>
            'テキストファイルの内容を構造体で管理
            Print.Sousinsaki = Me.DEN_TEXT.CENTER_NAME
            'Print.Sousinsaki = strSENTER_NAME
            '2014/05/21 saitou 標準版 UPD --------------------------------------------------<<<<

            'ファイル読み取り情報設定
            EntryStream = New FileStream(FileName, FileMode.Open, FileAccess.Read) '読込ファイル
            Dim br As BinaryReader = New BinaryReader(EntryStream)
            Dim FileLen As Integer = EntryStream.Length
            Dim _cnt As Long = 0
            Do
                lngKOBETU_RECORD_COUNT += 1
                lngRECORD_COUNT += 1
                br.BaseStream.Seek(_cnt, SeekOrigin.Begin)


                strFURI_DATA = Encoding.GetEncoding("SHIFT-JIS").GetString(br.ReadBytes(220))
                Select Case strFURI_DATA.Substring(0, 1)
                    Case "1"
                        g220_REC1.Data = strFURI_DATA
                        strFURI_DATA_KBN = "1"
                        dblFURI_KEN = 0
                        dblFURI_KIN = 0
                        dblFUNOU_KEN = 0
                        dblFUNOU_KIN = 0
                        dblSYORI_KEN = 0
                        dblSYORI_KIN = 0
                    Case "2"
                        g220_REC2.Data = strFURI_DATA
                        If g220_REC2.ZK17 <> "0" Then
                            dblFUNOU_KEN += 1
                            dblFUNOU_KIN += Val(g220_REC2.ZK9)
                            dblSYORI_KEN += 1
                            dblSYORI_KIN += Val(g220_REC2.ZK9)
                        Else
                            dblFURI_KEN += 1
                            dblFURI_KIN += Val(g220_REC2.ZK9)
                            dblSYORI_KEN += 1
                            dblSYORI_KIN += Val(g220_REC2.ZK9)
                        End If

                        strFURI_DATA_KBN = "2"
                    Case "8"
                        'If strFURI_DATA_KBN <> "2" Then
                        '    Exit Select
                        'End If
                        g220_REC8.Data = strFURI_DATA
                        strFURI_DATA_KBN = "8"

                        'If dblSYORI_KEN <> Val(g220_REC8.ZK2) Then
                        '    Msg = String.Format(MSG0327W, "処理件数", g220_REC1.ZK4, dblSYORI_KEN.ToString("#,##0"), Val(g220_REC8.ZK2).ToString("#,##0"))
                        '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        '    Exit Function
                        'End If
                        'If dblSYORI_KIN <> Val(g220_REC8.ZK3) Then
                        '    Msg = String.Format(MSG0327W, "処理金額", g220_REC1.ZK4, dblSYORI_KIN.ToString("#,##0"), Val(g220_REC8.ZK3).ToString("#,##0"))
                        '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        '    Exit Function
                        'End If
                        If (g220_REC8.ZK4.Trim = "" Or Val(g220_REC8.ZK4) = 0) And (g220_REC8.ZK6.Trim = "" Or Val(g220_REC8.ZK6) = 0) Then '依頼
                            If (g220_REC8.ZK2.Trim = "" Or Val(g220_REC8.ZK2.Trim = 0)) Then
                                Print.Syumoku = "結果" '種目(0件データ対応)
                            Else
                                Print.Syumoku = "依頼" '種目
                            End If
                            dblFURI_KEN = 0
                            dblFURI_KIN = 0
                            dblFUNOU_KEN = 0
                            dblFUNOU_KIN = 0
                        Else   '結果
                            Print.Syumoku = "結果" '種目
                            'If dblFURI_KEN <> Val(g220_REC8.ZK4) Then
                            '    Msg = String.Format(MSG0327W, "振替件数", g220_REC1.ZK4, dblFURI_KEN.ToString("#,##0"), Val(g220_REC8.ZK4).ToString("#,##0"))
                            '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                            'If dblFURI_KIN <> Val(g220_REC8.ZK5) Then
                            '    Msg = String.Format(MSG0327W, "振替金額", g220_REC1.ZK4, dblFURI_KIN.ToString("#,##0"), Val(g220_REC8.ZK5).ToString("#,##0"))
                            '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                            'If dblFUNOU_KEN <> Val(g220_REC8.ZK6) Then
                            '    Msg = String.Format(MSG0327W, "不能件数", g220_REC1.ZK4, dblFUNOU_KEN.ToString("#,##0"), Val(g220_REC8.ZK6).ToString("#,##0"))
                            '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                            'If dblFUNOU_KIN <> Val(g220_REC8.ZK7) Then
                            '    Msg = String.Format(MSG0327W, "不能金額", g220_REC1.ZK4, dblFUNOU_KIN.ToString("#,##0"), Val(g220_REC8.ZK7).ToString("#,##0"))
                            '    MessageBox.Show(Msg, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            '    Exit Function
                            'End If
                        End If

                        Print.Iraisaki = g220_REC1.ZK5   '振替依頼先名
                        Print.ItakuCode = g220_REC1.ZK4  '委託者コード
                        Select Case g220_REC1.ZK2 '入出金
                            Case "91"
                                Print.NSKin = "出金"
                            Case "11", "21", "71"
                                Print.NSKin = "入金"
                            Case Else
                                Print.NSKin = ""
                        End Select
                        Print.SyoriKen = g220_REC8.ZK2
                        Print.SyoriKin = g220_REC8.ZK3
                        Print.FuriKen = g220_REC8.ZK4
                        Print.FuriKin = g220_REC8.ZK5
                        Print.FunouKen = g220_REC8.ZK6
                        Print.FunouKin = g220_REC8.ZK7
                        Print.FuriDate = Now.ToString("yyyy") & g220_REC1.ZK6   '振替日
                        DataList.Add(Print)
                    Case "9"
                        strFURI_DATA_KBN = "9"
                End Select
                _cnt += 220
            Loop While FileLen > _cnt
            EntryStream.Close()
            Return True
        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ファイル読み取り", "失敗", ex.ToString)
        Finally
            If Not EntryStream Is Nothing Then EntryStream.Close()
        End Try

    End Function

#End Region

#Region " イベント"
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
              Handles txtHenkanDateY.Validating, txtHenkanDateM.Validating, txtHenkanDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub
#End Region

End Class