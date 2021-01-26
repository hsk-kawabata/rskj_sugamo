' 2016/01/11 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Imports CAstBatch
Imports System.Configuration
Imports System.Xml
Imports System.Windows.Forms
Imports System.Drawing

Public Class KFCMAIN020

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
    Private MainLOG As New CASTCommon.BatchLOG("KFCMAIN020", "媒体書込（ディスク→媒体）画面")
    Private Const msgTitle As String = "媒体書込（ディスク→媒体）画面(KFCMAIN020)"
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
    Private ListViewArray As ArrayList               ' リスト編集用ArrayList
    Private WriteKbn As String
    Private UketukeNoDisp As String
    Private UketukeNo As Integer

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
        Dim COMMON_BAITAIWRITE As String             ' 媒体書込データ格納フォルダ
        Dim COMMON_DATBK As String                   ' 媒体書込データ退避フォルダ
    End Structure
    Private IniInfo As INI_INFO

    '--------------------------------
    ' 取引先、スケジュール項目
    '--------------------------------
    Friend Structure TORI_INFO
        Dim TORIS_CODE As String                     ' 取引先主コード
        Dim TORIF_CODE As String                     ' 取引先副コード
        Dim ITAKU_CODE As String                     ' 委託者コード
        Dim ITAKU_KANRI_CODE As String               ' 代表委託者コード
        Dim BAITAI_CODE As String                    ' 媒体コード
        Dim CODE_KBN As String                       ' コード区分
        Dim FMT_KBN As String                        ' フォーマット区分
        Dim MULTI_KBN As String                      ' マルチ区分
        Dim FILE_NAME As String                      ' ファイル名
        Dim FURIDATE As String                       ' 振替日
        Dim RECORD_LENGTH As String                  ' レコード長
        Dim FTRANP As String                         ' コード変換定義
        Dim FTRANP_IBM As String                     ' コード変換定義（IBMフォーマット）
        Dim SFURI_FLG As String                      ' 再振フラグ
        Dim HENKAN_FILENAME As String                ' 返還データファイル名
    End Structure
    Private ToriInfo As TORI_INFO

    Structure gstrFURI_DATA_118
        <VBFixedString(118)> Public strDATA As String
    End Structure
    Public gstrDATA_118 As gstrFURI_DATA_118

    Structure gstrFURI_DATA_119
        <VBFixedString(119)> Public strDATA As String
    End Structure
    Public gstrDATA_119 As gstrFURI_DATA_119

    Structure gstrFURI_DATA_120
        <VBFixedString(120)> Public strDATA As String
    End Structure
    Public gstrDATA_120 As gstrFURI_DATA_120

    Structure gstrFURI_DATA_130
        <VBFixedString(130)> Public strDATA As String
    End Structure
    Public gstrDATA_130 As gstrFURI_DATA_130

    Structure gstrFURI_DATA_220
        <VBFixedString(220)> Public strDATA As String
    End Structure
    Public gstrDATA_220 As gstrFURI_DATA_220

    Structure gstrFURI_DATA_390
        <VBFixedString(390)> Public strDATA As String
    End Structure
    Public gstrDATA_390 As gstrFURI_DATA_390

    '全銀
    Public gZENGIN_REC1 As CAstFormat.CFormatZengin.ZGRECORD1
    Public gZENGIN_REC2 As CAstFormat.CFormatZengin.ZGRECORD2
    Public gZENGIN_REC8 As CAstFormat.CFormatZengin.ZGRECORD8
    '国税
    Public gKOKUZEI_REC1 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD1
    Public gKOKUZEI_REC2 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD2
    Public gKOKUZEI_REC3 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD3
    Public gKOKUZEI_REC8 As CAstFormat.CFormatKokuzei.KOKUZEI_RECORD8
    'ＮＨＫ
    Public gNHK_REC1 As CAstFormat.CFormatNHK.NHKRECORD1
    Public gNHK_REC2 As CAstFormat.CFormatNHK.NHKRECORD2
    Public gNHK_REC8 As CAstFormat.CFormatNHK.NHKRECORD8
    Public gNHK_REC9 As CAstFormat.CFormatNHK.NHKRECORD9

#End Region

#Region " ロード "

    Private Sub KFCMAIN020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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

            '------------------------------------
            ' 振替日に前営業日を表示
            '------------------------------------
            Dim strSysDate As String
            Dim strGetdate As String = ""

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            bRet = GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)
            txtFuriDateY.Text = strGetdate.Substring(0, 4)
            txtFuriDateM.Text = strGetdate.Substring(4, 2)
            txtFuriDateD.Text = strGetdate.Substring(6, 2)

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
            ' リストの初期化
            '--------------------------------
            Me.ListView1.Items.Clear()
            UketukeNo = 1
            UketukeNoDisp = ""
            lblItakuCode.Text = ""
            lblItakuName.Text = ""
            lblCodeKbn.Text = ""
            lblFileName.Text = ""
            lblBaitai.Text = ""

            '--------------------------------
            ' コンボボックス設定
            '--------------------------------
            Select Case GCom.SetComboBox(cmbKakikomiHouhou, "KFCMAIN020_書込方法.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "書込方法", "KFCMAIN020_書込方法.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", "書込方法設定ファイルなし。ファイル:KFCMAIN020_書込方法.TXT")
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "書込方法"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", "コンボボックス設定失敗 コンボボックス名:書込方法")
            End Select

            '--------------------------------
            ' 取引先(構造体)情報クリア
            '--------------------------------
            If ClearToriInfo() = False Then
                Exit Try
            End If
            
            '--------------------------------
            ' ボタンの初期化
            '--------------------------------
            Me.btnWrite.Enabled = True
            Me.btnReset.Enabled = True
            Me.btnEnd.Enabled = True

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
    ' 書込開始ボタン
    '================================
    Private Sub btnWrite_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWrite.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "書込開始", "開始", "")

            '--------------------------------
            ' ログ情報設定
            '--------------------------------
            If SetLogInfo(False) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "書込開始", "失敗", "ログ情報設定")
                Exit Try
            End If

            '--------------------------------
            ' 取引先(構造体)情報クリア
            '--------------------------------
            If ClearToriInfo() = False Then
                Exit Try
            End If

            '--------------------------------
            ' 取引先マスタチェック
            '--------------------------------
            If CheckTorimast(txtTorisCode.Text, txtTorifCode.Text) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "書込開始", "失敗", "取引先マスタチェック")
                Exit Try
            End If

            '--------------------------------
            ' スケジュールマスタチェック
            '--------------------------------
            Dim FuriDate As String = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            If CheckSchmast(txtTorisCode.Text, txtTorifCode.Text, FuriDate) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "書込開始", "失敗", "スケジュールマスタチェック")
                Exit Try
            End If

            '--------------------------------
            ' 媒体書込処理
            '--------------------------------
            If MessageBox.Show(String.Format(MSG0077I, "入力した取引先", "媒体書込処理(" & cmbKakikomiHouhou.Text & ")"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "書込開始", "中断", "ユーザキャンセル")
                Exit Try
            End If

            WriteKbn = ""
            Select Case cmbKakikomiHouhou.Text
                Case "通常書込"
                    WriteKbn = "1"
                Case "強制書込"
                    WriteKbn = "2"
            End Select
            ListViewArray = New ArrayList
            ListViewArray.Clear()

            ToriInfo.TORIS_CODE = txtTorisCode.Text
            ToriInfo.TORIF_CODE = txtTorifCode.Text
            ToriInfo.FURIDATE = FuriDate

            If BaitaiWrite() = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "書込開始", "失敗", "媒体書込")
                Exit Try
            End If

            '--------------------------------
            ' リスト情報編集
            '--------------------------------
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "書込開始", "", "リスト情報編集 開始")
            For i As Integer = 0 To ListViewArray.Count - 1 Step 1
                Dim vLstItem As New ListViewItem(ListViewArray(i).ToString.Split("/"), -1, Color.Black, Color.White, Nothing)
                ListView1.Items.AddRange({vLstItem})
            Next

            '--------------------------------
            ' 完了メッセージ表示
            '--------------------------------
            MessageBox.Show(String.Format(MSG0078I, "媒体書込処理"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "書込開始", "成功", "")
            UketukeNo += 1

            '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- START
            UketukeNoDisp = ""
            '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "書込開始", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "書込開始", "終了", "")
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
            If IniInfo.COMMON_FDDRIVE.ToUpper = "err" OrElse IniInfo.COMMON_FDDRIVE = Nothing Then
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

            IniInfo.COMMON_BAITAIWRITE = CASTCommon.GetFSKJIni("COMMON", "BAITAIWRITE")
            If IniInfo.COMMON_BAITAIWRITE.ToUpper = "ERR" OrElse IniInfo.COMMON_BAITAIWRITE = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "書込データ格納フォルダ", "COMMON", "BAITAIWRITE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:書込データ格納フォルダ 分類:COMMON 項目:BAITAIWRITE")
                Return False
            End If

            IniInfo.COMMON_DATBK = CASTCommon.GetFSKJIni("COMMON", "DATBK")
            If IniInfo.COMMON_DATBK.ToUpper = "ERR" OrElse IniInfo.COMMON_DATBK = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "書込データ退避フォルダ", "COMMON", "DATBK"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:書込データ退避フォルダ 分類:COMMON 項目:DATBK")
                Return False
            End If

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
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T   = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_T   = '" & TorifCode & "'")
            SQL.Append(" AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先マスタチェック", "失敗", "該当なし")
                MessageBox.Show(String.Format(MSG0372W, "取引先", "媒体の取扱"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Else
                '--------------------------------
                ' 結果返却要否チェック
                '--------------------------------
                Select Case GCom.NzStr(OraReader.Reader.Item("KEKKA_HENKYAKU_KBN_T"))
                    Case "1", "2"
                        ' NOP
                    Case Else
                        MessageBox.Show(String.Format(MSG0372W, "取引先コード", "媒体返却先"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                End Select

                '--------------------------------
                ' 取引先項目取得
                '--------------------------------
                ToriInfo.BAITAI_CODE = GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T"))
                ToriInfo.CODE_KBN = GCom.NzStr(OraReader.Reader.Item("CODE_KBN_T"))
                ToriInfo.FILE_NAME = GCom.NzStr(OraReader.Reader.Item("FILE_NAME_T")).Trim
                ToriInfo.ITAKU_CODE = GCom.NzStr(OraReader.Reader.Item("ITAKU_CODE_T"))
                ToriInfo.ITAKU_KANRI_CODE = GCom.NzStr(OraReader.Reader.Item("ITAKU_KANRI_CODE_T"))
                ToriInfo.FMT_KBN = GCom.NzStr(OraReader.Reader.Item("FMT_KBN_T"))
                ToriInfo.SFURI_FLG = GCom.NzStr(OraReader.Reader.Item("SFURI_FLG_T"))
                ToriInfo.MULTI_KBN = GCom.NzStr(OraReader.Reader.Item("MULTI_KBN_T"))
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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
    ' スケジュールマスタチェック
    '================================
    Private Function CheckSchmast(ByVal TorisCode As String, ByVal TorifCode As String, ByVal FuriDate As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタチェック", "開始", "")

            SQL = New StringBuilder(128)
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     *")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST , SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_T   = '" & TorisCode & "'")
            SQL.Append(" AND TORIF_CODE_T   = '" & TorifCode & "'")
            SQL.Append(" AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')")
            SQL.Append(" AND TORIS_CODE_T   = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T   = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_S    = '" & FuriDate & "'")
            SQL.Append(" AND TOUROKU_FLG_S <> '0'")
            SQL.Append(" AND HAISIN_FLG_S  <> '0'")

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "スケジュールマスタチェック", "失敗", "該当なし")
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Else
                '--------------------------------
                ' 進捗状態チェック
                '--------------------------------
                If GCom.NzStr(OraReader.Reader.Item("FUNOU_FLG_S")) <> "1" Then
                    MessageBox.Show("入力されたスケジュールは、" & MSG0322W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    '================================
    ' 媒体書込
    '================================
    Private Function BaitaiWrite() As Boolean

        Dim ErrMessage As String

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込メイン", "開始", "")

            '--------------------------------
            ' 媒体書込メイン処理(媒体1本目)
            '--------------------------------
            ErrMessage = ""
            If BaitaiWrite_Main(ErrMessage) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込メイン", "失敗", "正媒体書込")
                If ErrMessage <> "" Then
                    MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                Return False
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込メイン", "成功", "正媒体書込")
            End If

            '--------------------------------
            ' 媒体書込メイン処理(媒体2本目)
            '--------------------------------
            If MessageBox.Show(MSG0061I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                ErrMessage = ""
                If BaitaiWrite_Main(ErrMessage) = False Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込メイン", "失敗", "副媒体書込")
                    If ErrMessage <> "" Then
                        MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    Return False
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込メイン", "成功", "副媒体書込")
                End If
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込メイン", "成功", "副媒体書込キャンセル")
            End If

            '--------------------------------
            ' 媒体書込メイン処理(媒体検証)
            '--------------------------------
            MessageBox.Show(MSG0062I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            ErrMessage = ""
            If BaitaiWrite_Check(ErrMessage) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込メイン", "成功", "媒体検証")
                If ErrMessage <> "" Then
                    MessageBox.Show(ErrMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                Return False
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込メイン", "成功", "媒体検証")
            End If

            '--------------------------------
            ' 返還データ退避
            '--------------------------------
            If File.Exists(IniInfo.COMMON_DATBK & Path.GetFileName(ToriInfo.HENKAN_FILENAME)) = True Then
                File.Delete(IniInfo.COMMON_DATBK & Path.GetFileName(ToriInfo.HENKAN_FILENAME))
            End If
            File.Move(ToriInfo.HENKAN_FILENAME, IniInfo.COMMON_DATBK & Path.GetFileName(ToriInfo.HENKAN_FILENAME))

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込メイン", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込メイン", "終了", "")
        End Try

    End Function

    Private Function BaitaiWrite_Main(ByRef ErrMessage As String) As Boolean

        Dim WORK_FILENAME As String = ""  ' 媒体内ファイル（チェック用）
        Dim OUT_FILENAME As String        ' 媒体内ファイル名
        Dim IN_FILENAME As String         ' 返還処理作成ファイル
        Dim intKEKKA As Integer

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "開始", "")

            Dim HenkanFMT As New CAstFormat.CFormat
            Dim para As New CAstBatch.CommData.stPARAMETER
            para.FSYORI_KBN = "1"
            para.FMT_KBN = ToriInfo.FMT_KBN
            para.CODE_KBN = ToriInfo.CODE_KBN
            HenkanFMT = CAstFormat.CFormat.GetFormat(para)

            If GetPFileInfo(ToriInfo.RECORD_LENGTH, ToriInfo.FTRANP, ToriInfo.FTRANP_IBM) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "コード変換情報取得")
                ErrMessage = String.Format(MSG0371W, "コード変換情報の取得処理", "ログを確認してください。")
                Return False
            End If

            WORK_FILENAME = IniInfo.COMMON_BAITAIWRITE & "W_BEF_" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & ".dat"
            If File.Exists(WORK_FILENAME) = True Then
                Kill(WORK_FILENAME)
            End If
            OUT_FILENAME = ToriInfo.FILE_NAME.Trim

            Select Case ToriInfo.MULTI_KBN
                Case "0"
                    IN_FILENAME = IniInfo.COMMON_BAITAIWRITE & "O" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & "_" & ToriInfo.FURIDATE & ".dat"
                Case "1"
                    IN_FILENAME = IniInfo.COMMON_BAITAIWRITE & "O" & ToriInfo.ITAKU_KANRI_CODE & "_" & ToriInfo.FURIDATE & ".dat"
                Case Else
                    IN_FILENAME = IniInfo.COMMON_BAITAIWRITE & "O" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & "_" & ToriInfo.FURIDATE & ".dat"
            End Select

            If File.Exists(IN_FILENAME) = False Then
                ErrMessage = String.Format(MSG0255W, "選択したスケジュールの返還対象")
                Return False
            End If

            If ToriInfo.HENKAN_FILENAME = "" Then
                ToriInfo.HENKAN_FILENAME = IN_FILENAME
            End If

            Select Case ToriInfo.BAITAI_CODE
                Case "01"       'FD書込み
                    If WriteKbn = "1" Then
                        intKEKKA = clsFUSION.fn_FD_CPYTO_DISK(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                                              OUT_FILENAME, _
                                                              WORK_FILENAME, _
                                                              ToriInfo.RECORD_LENGTH, _
                                                              ToriInfo.CODE_KBN, _
                                                              ToriInfo.FTRANP, msgTitle)
                        Select Case intKEKKA
                            Case 0
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "成功 ", "ＦＤ取込(コード変換)")
                            Case 100
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "ＦＤ取込(コード変換)")
                                ErrMessage = String.Format(MSG0371W, "ＦＤ取込", "コード変換")
                                Return False
                            Case 200
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "ＦＤ取込(コード区分異常(JIS改行あり))")
                                ErrMessage = String.Format(MSG0371W, "ＦＤ取込", "コード区分異常(JIS改行あり)")
                                Return False
                            Case 300
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "ＦＤ取込(コード区分異常(JIS改行なし))")
                                ErrMessage = String.Format(MSG0371W, "ＦＤ取込", "コード区分異常(JIS改行なし)")
                                Return False
                            Case 400
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "ＦＤ取込(出力ファイル作成)失敗")
                                ErrMessage = String.Format(MSG0371W, "ＦＤ取込", "出力ファイル作成")
                                Return False
                        End Select

                        '------------------------------------
                        '媒体内のファイルのチェック
                        '------------------------------------
                        If CheckBaitai(WORK_FILENAME, IN_FILENAME) = False Then
                            ErrMessage = String.Format(MSG0370W, "媒体内ファイルチェック")
                            Return False
                        End If
                    End If

                    '------------------------------------
                    '媒体に書き込み
                    '------------------------------------
                    intKEKKA = clsFUSION.fn_DISK_CPYTO_FD(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                                          IN_FILENAME, _
                                                          OUT_FILENAME, _
                                                          ToriInfo.RECORD_LENGTH, _
                                                          ToriInfo.CODE_KBN, ToriInfo.FTRANP_IBM, False, msgTitle)

                    Select Case intKEKKA
                        Case 0
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "成功 ", "ＦＤ書込み")
                        Case 100
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "ＦＤ書込み失敗(IBM形式)")
                            ErrMessage = String.Format(MSG0371W, "ＦＤ書込", "IBM形式")
                            Return False
                        Case 200
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "ＦＤ書込み失敗(DOS形式)")
                            ErrMessage = String.Format(MSG0371W, "ＦＤ書込", "DOS形式")
                            Return False
                    End Select

                Case "11", "12", "13", "14", "15"       'DVD書込み
                    If WriteKbn = "1" Then
                        intKEKKA = clsFUSION.fn_DVD_CPYTO_DISK(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                               OUT_FILENAME, _
                                               WORK_FILENAME, _
                                               ToriInfo.RECORD_LENGTH, _
                                               ToriInfo.CODE_KBN, _
                                               ToriInfo.FTRANP, _
                                               msgTitle, _
                                               ToriInfo.BAITAI_CODE)

                        'ログ出力内容を変更（ＤＶＤ→外部媒体）
                        Select Case intKEKKA
                            Case 0
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "成功 ", "外部媒体取込(コード変換)")
                            Case 100
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "外部媒体取込(コード変換)")
                                ErrMessage = String.Format(MSG0371W, "外部媒体取込", "コード変換")
                                Return False
                            Case 200
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "外部媒体取込(コード区分異常(JIS改行あり))")
                                ErrMessage = String.Format(MSG0371W, "外部媒体取込", "コード区分異常(JIS改行あり)")
                                Return False
                            Case 300
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "外部媒体取込(コード区分異常(JIS改行なし))")
                                ErrMessage = String.Format(MSG0371W, "外部媒体取込", "コード区分異常(JIS改行なし)")
                                Return False
                            Case 400
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "外部媒体取込(出力ファイル作成)失敗")
                                ErrMessage = String.Format(MSG0371W, "外部媒体取込", "出力ファイル作成")
                                Return False
                        End Select

                        '------------------------------------
                        '媒体内のファイルのチェック
                        '------------------------------------
                        If CheckBaitai(WORK_FILENAME, IN_FILENAME) = False Then
                            ErrMessage = String.Format(MSG0370W, "媒体内ファイルチェック")
                            Return False
                        End If
                    End If

                    '------------------------------------
                    '媒体に書き込み
                    '------------------------------------
                    intKEKKA = clsFUSION.fn_DISK_CPYTO_DVD(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                                           IN_FILENAME, _
                                                           OUT_FILENAME, _
                                                           ToriInfo.RECORD_LENGTH, _
                                                           ToriInfo.CODE_KBN, ToriInfo.FTRANP, False, msgTitle, _
                                                           ToriInfo.BAITAI_CODE)

                    Select Case intKEKKA
                        Case 0
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "成功 ", "外部媒体書込み")
                        Case 100
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "外部媒体書込み失敗(EBCDIC)")
                            ErrMessage = String.Format(MSG0371W, "媒体書込", "文字コード(EBCDIC)")
                            Return False
                        Case 200
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗 ", "外部媒体書込み失敗(JIS)")
                            ErrMessage = String.Format(MSG0371W, "媒体書込", "文字コード(JIS)")
                            Return False
                    End Select
            End Select

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "失敗", ex.Message)
            Return False
        Finally
            If Dir(WORK_FILENAME) <> "" Then
                Kill(WORK_FILENAME)
            End If

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込", "終了", "")
        End Try

    End Function

    Private Function CheckBaitai(ByVal astrCHK_SEND_FILE As String, _
                                 ByVal astrIN_FILE_NAME As String) As Boolean

        Dim FileNo1_Open As String = Nothing
        Dim FileNo2_Open As String = Nothing
        Dim intFILE_NO_1 As Integer
        Dim intFILE_NO_2 As Integer
        Dim strFURI_DATA_1 As String = ""
        Dim strFURI_DATA_2 As String = ""

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体チェック", "開始", "")

            '------------------------------------
            ' 再振はチェック対象外
            '------------------------------------
            If ToriInfo.SFURI_FLG = "1" Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体チェック", "成功", "再振スケジュール")
                Return True
            End If

            '------------------------------------
            ' ファイル１オープン
            '------------------------------------
            intFILE_NO_1 = FreeFile()
            FileOpen(intFILE_NO_1, astrCHK_SEND_FILE, OpenMode.Random, , , ToriInfo.RECORD_LENGTH)
            FileNo1_Open = "1"
            If Err.Number = 0 Then
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体チェック", "失敗 ", "媒体ファイルオープン ファイル名：" & astrCHK_SEND_FILE & Err.Description)
                Return False
            End If

            '------------------------------------
            ' ファイル２オープン
            '------------------------------------
            intFILE_NO_2 = FreeFile()
            FileOpen(intFILE_NO_2, astrIN_FILE_NAME, OpenMode.Random, , , ToriInfo.RECORD_LENGTH)
            FileNo2_Open = "1"
            If Err.Number = 0 Then
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体チェック", "失敗 ", "媒体ファイルオープン ファイル名：" & astrIN_FILE_NAME & Err.Description)
                Return False
            End If

            '------------------------------------
            ' ファイルチェック
            '------------------------------------
            Dim sysValueType As System.ValueType
            Select Case ToriInfo.RECORD_LENGTH
                Case 120
                    sysValueType = DirectCast(gstrDATA_120, ValueType)
                    FileGet(intFILE_NO_1, sysValueType, 1)
                    gstrDATA_120 = DirectCast(sysValueType, gstrFURI_DATA_120)
                    strFURI_DATA_1 = gstrDATA_120.strDATA

                    sysValueType = DirectCast(gstrDATA_120, ValueType)
                    FileGet(intFILE_NO_2, sysValueType, 1)
                    gstrDATA_120 = DirectCast(sysValueType, gstrFURI_DATA_120)
                    strFURI_DATA_2 = gstrDATA_120.strDATA
            End Select

            If strFURI_DATA_1 <> strFURI_DATA_2 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体チェック", "失敗 ", "媒体不一致" & Err.Description)
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体チェック", ex.Message)
            Return False
        Finally
            If FileNo1_Open = "1" Then FileClose(intFILE_NO_1)
            If FileNo2_Open = "1" Then FileClose(intFILE_NO_2)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体チェック", "終了", "")
        End Try

    End Function

    Private Function BaitaiWrite_Check(ByRef ErrMessage As String) As Boolean

        Dim WORK_FILENAME As String = ""
        Dim OUT_FILENAME As String
        Dim intKEKKA As Integer

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込後検証", "開始", "")

            WORK_FILENAME = IniInfo.COMMON_BAITAIWRITE & "W_AFT_" & ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE & ".dat"
            If Dir(WORK_FILENAME) <> "" Then
                Kill(WORK_FILENAME)
            End If
            OUT_FILENAME = ToriInfo.FILE_NAME.Trim

            Select Case ToriInfo.BAITAI_CODE
                Case "01"
                    '------------------------------------
                    ' 媒体：ＦＤ
                    '------------------------------------
                    intKEKKA = clsFUSION.fn_FD_CPYTO_DISK(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                                          OUT_FILENAME, _
                                                          WORK_FILENAME, _
                                                          ToriInfo.RECORD_LENGTH, _
                                                          ToriInfo.CODE_KBN, _
                                                          ToriInfo.FTRANP, _
                                                          msgTitle)
                    Select Case intKEKKA
                        Case 0
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込後検証", "成功 ", "ＦＤ取込")
                        Case 100
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込後検証", "失敗 ", "ＦＤ取込(コード変換)失敗")
                            ErrMessage = String.Format(MSG0371W, "媒体検証", "ＦＤ取込(コード変換)")
                            Return False
                        Case 200
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体書込後検証", "失敗 ", "ＦＤ取込(コード区分異常(JIS改行あり))")
                            ErrMessage = String.Format(MSG0371W, "媒体検証", "ＦＤ取込(コード区分異常(JIS改行あり))")
                            Return False
                        Case 300
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒媒体書込後検証", "失敗 ", "ＦＤ取込(コード区分異常(JIS改行なし))")
                            ErrMessage = String.Format(MSG0371W, "媒体検証", "ＦＤ取込(コード区分異常(JIS改行なし))")
                            Return False
                        Case 400
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒媒体書込後検証", "失敗 ", "ＦＤ取込(出力ファイル作成)失敗")
                            ErrMessage = String.Format(MSG0371W, "媒体検証", "ＦＤ取込(出力ファイル作成)")
                            Return False
                    End Select

                    '------------------------------------
                    '媒体内のファイルのチェック
                    '------------------------------------
                    '2016/02/05 タスク）岩城 RSV2対応 UPD ---------------------------------------- START
                    'XMLフォーマット考慮漏れ
                    If IsNumeric(ToriInfo.FMT_KBN) = True Then
                        Dim nFmtKbn As Integer = CInt(ToriInfo.FMT_KBN)
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            If BaitaiWrite_DataCheck_XML(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                                ErrMessage = String.Format(MSG0371W, "媒体検証", "媒体内ファイルチェック")
                                Return False
                            End If
                        Else
                            If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                                ErrMessage = String.Format(MSG0371W, "媒体検証", "媒体内ファイルチェック")
                                Return False
                            End If
                        End If
                    Else
                        If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                            ErrMessage = String.Format(MSG0371W, "媒体検証", "媒体内ファイルチェック")
                            Return False
                        End If
                    End If
                    'If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                    '    ErrMessage = String.Format(MSG0371W, "媒体検証", "媒体内ファイルチェック")
                    '    Return False
                    'End If
                    '2016/02/05 タスク）岩城 RSV2対応 UPD ---------------------------------------- END

                Case "11", "12", "13", "14", "15"
                    '------------------------------------
                    ' 媒体：外部媒体
                    '------------------------------------
                    intKEKKA = clsFUSION.fn_DVD_CPYTO_DISK(ToriInfo.TORIS_CODE & ToriInfo.TORIF_CODE, _
                                                           OUT_FILENAME, _
                                                           WORK_FILENAME, _
                                                           ToriInfo.RECORD_LENGTH, _
                                                           ToriInfo.CODE_KBN, _
                                                           ToriInfo.FTRANP, _
                                                           msgTitle, _
                                                           ToriInfo.BAITAI_CODE)
                    Select Case intKEKKA
                        Case 0
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒媒体書込後検証", "成功 ", "外部媒体取込")
                        Case 100
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒媒体書込後検証", "失敗 ", "外部媒体取込(コード変換)失敗")
                            ErrMessage = String.Format(MSG0371W, "媒体検証", "外部媒体取込(コード変換")
                            Return False
                        Case 200
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒媒体書込後検証", "失敗 ", "外部媒体取込(コード区分異常(JIS改行あり))")
                            ErrMessage = String.Format(MSG0371W, "媒体検証", "外部媒体取込(コード区分異常(JIS改行あり))")
                            Return False
                        Case 300
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒媒体書込後検証", "失敗 ", "外部媒体取込(コード区分異常(JIS改行なし))")
                            ErrMessage = String.Format(MSG0371W, "媒体検証", "外部媒体取込(コード区分異常(JIS改行なし))")
                            Return False
                        Case 400
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒媒体書込後検証", "失敗 ", "外部媒体取込(出力ファイル作成)失敗")
                            ErrMessage = String.Format(MSG0371W, "媒体検証", "外部媒体取込(出力ファイル作成)")
                            Return False
                    End Select

                    '------------------------------------
                    '媒体内のファイルのチェック
                    '------------------------------------
                    '2016/02/05 タスク）岩城 RSV2対応 UPD ---------------------------------------- START
                    'XMLフォーマット考慮漏れ
                    If IsNumeric(ToriInfo.FMT_KBN) = True Then
                        Dim nFmtKbn As Integer = CInt(ToriInfo.FMT_KBN)
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            If BaitaiWrite_DataCheck_XML(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                                ErrMessage = String.Format(MSG0371W, "媒体検証", "媒体内ファイルチェック")
                                Return False
                            End If
                        Else
                            If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                                ErrMessage = String.Format(MSG0371W, "媒体検証", "媒体内ファイルチェック")
                                Return False
                            End If
                        End If
                    Else
                        If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                            ErrMessage = String.Format(MSG0371W, "媒体検証", "媒体内ファイルチェック")
                            Return False
                        End If
                    End If
                    'If BaitaiWrite_DataCheck(WORK_FILENAME, ToriInfo.RECORD_LENGTH) = False Then
                    '    ErrMessage = String.Format(MSG0371W, "媒体検証", "媒体内ファイルチェック")
                    '    Return False
                    'End If
                    '2016/02/05 タスク）岩城 RSV2対応 UPD ---------------------------------------- END
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒媒体書込後検証", "成功", "")
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒媒体書込後検証", "失敗", ex.Message)
            Return False
        Finally
            If Dir(WORK_FILENAME) <> "" Then
                Kill(WORK_FILENAME)
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒媒体書込後検証", "終了", "")
        End Try

    End Function

    Function BaitaiWrite_DataCheck(ByVal astrCHK_SEND_FILE As String, ByVal aintREC_LENGTH As Integer) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing

        Dim intFILE_NO_1 As Integer
        Dim FileNo1_Open As String = Nothing

        Dim strFURI_DATA As String
        Dim dblRECORD_COUNT As Double
        Dim FileItakuCode As String = ""
        Dim FileItakuName As String = ""
        Dim FileSyubetu As String = ""
        Dim FileTorisCode As String = ""
        Dim FileTorifCode As String = ""

        Dim dblSCH_SYORI_KEN As Double
        Dim dblSCH_SYORI_KIN As Double
        Dim dblSCH_FURI_KEN As Double
        Dim dblSCH_FURI_KIN As Double
        Dim dblSCH_FUNOU_KEN As Double
        Dim dblSCH_FUNOU_KIN As Double

        Dim dblFILE_SYORI_KEN As Double
        Dim dblFILE_SYORI_KIN As Double
        Dim dblFILE_FURI_KEN As Double
        Dim dblFILE_FURI_KIN As Double
        Dim dblFILE_FUNOU_KEN As Double
        Dim dblFILE_FUNOU_KIN As Double

        '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- START
        Dim UketukeNoDispFlg As Boolean = True
        '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- END

        Try

            MainDB = New CASTCommon.MyOracle
            UketukeNoDisp = ""
            dblRECORD_COUNT = 1
            intFILE_NO_1 = FreeFile()
            FileOpen(intFILE_NO_1, astrCHK_SEND_FILE, OpenMode.Random, , , aintREC_LENGTH)
            FileNo1_Open = "1"
            If Err.Number = 0 Then
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "書き込みファイルオープン ファイル名：" & astrCHK_SEND_FILE & Err.Description)
                Return False
            End If

            Dim sysValueType As ValueType
            Do Until EOF(intFILE_NO_1)
                strFURI_DATA = ""
                Select Case aintREC_LENGTH
                    Case 118
                        sysValueType = DirectCast(gstrDATA_118, ValueType)
                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        gstrDATA_118 = DirectCast(sysValueType, gstrFURI_DATA_118)
                        strFURI_DATA = gstrDATA_118.strDATA
                    Case 119
                        sysValueType = DirectCast(gstrDATA_119, ValueType)
                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        gstrDATA_119 = DirectCast(sysValueType, gstrFURI_DATA_119)
                        strFURI_DATA = gstrDATA_119.strDATA
                    Case 120
                        sysValueType = DirectCast(gstrDATA_120, ValueType)
                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        gstrDATA_120 = DirectCast(sysValueType, gstrFURI_DATA_120)
                        strFURI_DATA = gstrDATA_120.strDATA
                    Case 390
                        sysValueType = DirectCast(gstrDATA_390, ValueType)
                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                        gstrDATA_390 = DirectCast(sysValueType, gstrFURI_DATA_390)
                        strFURI_DATA = gstrDATA_390.strDATA
                End Select

                If strFURI_DATA = "" Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "ファイル読み込み レコードNO：" & dblRECORD_COUNT)
                    Return False
                End If

                Select Case strFURI_DATA.Substring(0, 1)
                    Case "1"
                        '2016/02/08 タスク）岩城 RSV2対応 UPD ---------------------------------------- START
                        If UketukeNoDisp = "" And UketukeNoDispFlg = True Then
                            UketukeNoDisp = UketukeNo
                            UketukeNoDispFlg = False
                        Else
                            UketukeNoDisp = ""
                        End If
                        'If UketukeNoDisp = "" Then
                        '    UketukeNoDisp = UketukeNo
                        'Else
                        '    UketukeNoDisp = ""
                        'End If
                        '2016/02/08 タスク）岩城 RSV2対応 UPD ---------------------------------------- END

                        '------------------------------------
                        ' ファイル内委託者コードを取得
                        '------------------------------------
                        Select Case aintREC_LENGTH
                            Case 118, 119, 120
                                Select Case ToriInfo.FMT_KBN
                                    Case "00"       '全銀
                                        sysValueType = DirectCast(gZENGIN_REC1, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gZENGIN_REC1 = DirectCast(sysValueType, CAstFormat.CFormatZengin.ZGRECORD1)
                                        FileItakuCode = gZENGIN_REC1.ZG4
                                        FileSyubetu = gZENGIN_REC1.ZG2
                                    Case "01"       'ＮＨＫ
                                        sysValueType = DirectCast(gNHK_REC1, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gNHK_REC1 = DirectCast(sysValueType, CAstFormat.CFormatNHK.NHKRECORD1)
                                        FileItakuCode = gNHK_REC1.NH04
                                        FileSyubetu = gNHK_REC1.NH02
                                End Select
                            Case 390
                                sysValueType = DirectCast(gKOKUZEI_REC1, ValueType)
                                FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                gKOKUZEI_REC1 = DirectCast(sysValueType, CAstFormat.CFormatKokuzei.KOKUZEI_RECORD1)
                                FileItakuCode = ""
                                FileSyubetu = ""
                            Case Else
                                FileItakuCode = ""
                                FileSyubetu = ""
                        End Select

                        '--------------------------------------------------------------
                        '取引先コード検索（マルチの場合も考慮する）、委託者コードのチェック
                        '--------------------------------------------------------------
                        SQL = New StringBuilder(128)
                        SQL.Length = 0
                        If FileItakuCode = "" Then
                            Return False
                        Else
                            SQL.Append("SELECT")
                            SQL.Append("     *")
                            SQL.Append(" FROM")
                            SQL.Append("     TORIMAST")
                            SQL.Append(" WHERE")
                            SQL.Append("     ITAKU_CODE_T = '" & Trim(FileItakuCode) & "'")
                        End If
                        OraReader = New CASTCommon.MyOracleReader(MainDB)
                        If OraReader.DataReader(SQL) = False Then            '取引先コードが存在しない
                            Return False
                        Else
                            FileItakuName = OraReader.GetString("ITAKU_KNAME_T").Trim
                            FileTorisCode = OraReader.GetString("TORIS_CODE_T")
                            FileTorifCode = OraReader.GetString("TORIF_CODE_T")
                            If FileSyubetu = "" Then
                                FileSyubetu = OraReader.GetString("SYUBETU_T")
                            End If
                            If Not OraReader Is Nothing Then
                                OraReader.Close()
                                OraReader = Nothing
                            End If
                        End If

                        '------------------------------------
                        'スケジュールの検索
                        '------------------------------------
                        If ToriInfo.FMT_KBN <> "02" Then
                            '------------------------------------
                            ' 国税以外の場合
                            '------------------------------------
                            SQL.Length = 0
                            SQL.Append("SELECT")
                            SQL.Append("     *")
                            SQL.Append(" FROM")
                            SQL.Append("     SCHMAST")
                            SQL.Append(" WHERE")
                            SQL.Append("     ITAKU_CODE_S = " & SQ(FileItakuCode))
                            SQL.Append(" AND FURI_DATE_S  = " & SQ(ToriInfo.FURIDATE))
                            SQL.Append(" AND FUNOU_FLG_S  = '1'")
                            If ToriInfo.MULTI_KBN = "0" Then  'シングル
                                SQL.Append(" AND TORIS_CODE_S  = " & SQ(ToriInfo.TORIS_CODE))
                                SQL.Append(" AND TORIF_CODE_S  = " & SQ(ToriInfo.TORIF_CODE))
                            End If
                        Else
                            '------------------------------------
                            ' 国税の場合(委託者コードを含めない)
                            '------------------------------------
                            SQL.Length = 0
                            SQL.Append("SELECT")
                            SQL.Append("     *")
                            SQL.Append(" FROM")
                            SQL.Append("     SCHMAST")
                            SQL.Append(" WHERE")
                            SQL.Append("     FURI_DATE_S   = " & SQ(ToriInfo.FURIDATE))
                            SQL.Append(" AND TORIS_CODE_S  = " & SQ(ToriInfo.TORIS_CODE))
                            SQL.Append(" AND TORIF_CODE_S  = " & SQ(ToriInfo.TORIF_CODE))
                        End If
                        OraReader = New CASTCommon.MyOracleReader(MainDB)
                        If OraReader.DataReader(SQL) = False Then            'スケジュールが存在しない
                            Return False
                        Else
                            dblSCH_SYORI_KEN = OraReader.GetInt64("SYORI_KEN_S")
                            dblSCH_SYORI_KIN = OraReader.GetInt64("SYORI_KIN_S")
                            dblSCH_FURI_KEN = OraReader.GetInt64("FURI_KEN_S")
                            dblSCH_FURI_KIN = OraReader.GetInt64("FURI_KIN_S")
                            dblSCH_FUNOU_KEN = OraReader.GetInt64("FUNOU_KEN_S")
                            dblSCH_FUNOU_KIN = OraReader.GetInt64("FUNOU_KIN_S")

                            If Not OraReader Is Nothing Then
                                OraReader.Close()
                                OraReader = Nothing
                            End If
                        End If
                    Case "2", "3", "4", "5"
                        ' NOP
                    Case "8"
                        Select Case aintREC_LENGTH
                            Case 118, 119, 120
                                Select Case ToriInfo.FMT_KBN
                                    Case "00"
                                        sysValueType = DirectCast(gZENGIN_REC8, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gZENGIN_REC8 = DirectCast(sysValueType, CAstFormat.CFormatZengin.ZGRECORD8)
                                        dblFILE_SYORI_KEN = Val(gZENGIN_REC8.ZG2)
                                        dblFILE_SYORI_KIN = Val(gZENGIN_REC8.ZG3)
                                        dblFILE_FURI_KEN = Val(gZENGIN_REC8.ZG4.Substring(0, 6))
                                        dblFILE_FURI_KIN = Val(gZENGIN_REC8.ZG5.Substring(0, 12))
                                        dblFILE_FUNOU_KEN = Val(gZENGIN_REC8.ZG6.Substring(0, 6))
                                        dblFILE_FUNOU_KIN = Val(gZENGIN_REC8.ZG7.Substring(0, 12))
                                    Case "01"
                                        sysValueType = DirectCast(gNHK_REC8, ValueType)
                                        FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                        gNHK_REC8 = DirectCast(sysValueType, CAstFormat.CFormatNHK.NHKRECORD8)
                                        dblFILE_SYORI_KEN = Val(gNHK_REC8.NH02)
                                        dblFILE_SYORI_KIN = Val(gNHK_REC8.NH03)
                                        dblFILE_FURI_KEN = Val(gNHK_REC8.NH04.Substring(0, 6))
                                        dblFILE_FURI_KIN = Val(gNHK_REC8.NH05.Substring(0, 12))
                                        dblFILE_FUNOU_KEN = Val(gNHK_REC8.NH06.Substring(0, 6))
                                        dblFILE_FUNOU_KIN = Val(gNHK_REC8.NH07.Substring(0, 12))
                                End Select
                            Case 390
                                sysValueType = DirectCast(gKOKUZEI_REC8, ValueType)
                                FileGet(intFILE_NO_1, sysValueType, CLng(dblRECORD_COUNT))
                                gKOKUZEI_REC8 = DirectCast(sysValueType, CAstFormat.CFormatKokuzei.KOKUZEI_RECORD8)
                                dblFILE_SYORI_KEN = Val(gKOKUZEI_REC8.KZ7.Trim)
                                dblFILE_SYORI_KIN = Val(gKOKUZEI_REC8.KZ8.Trim)
                                dblFILE_FURI_KEN = Val(gKOKUZEI_REC8.KZ11.Trim)
                                dblFILE_FURI_KIN = Val(gKOKUZEI_REC8.KZ12.Trim)
                                dblFILE_FUNOU_KEN = Val(gKOKUZEI_REC8.KZ9.Trim)
                                dblFILE_FUNOU_KIN = Val(gKOKUZEI_REC8.KZ10.Trim)
                        End Select

                        If dblSCH_SYORI_KEN <> dblFILE_SYORI_KEN Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "トレーラチェック 処理件数不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                            Return False
                        End If

                        If dblSCH_SYORI_KIN <> dblFILE_SYORI_KIN Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "トレーラチェック 処理金額不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                            Return False
                        End If

                        If dblSCH_FUNOU_KEN <> dblFILE_FUNOU_KEN Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "トレーラチェック 不能件数不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                            Return False
                        End If

                        If dblSCH_FUNOU_KIN <> dblFILE_FUNOU_KIN Then
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "トレーラチェック 不能金額不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                            Return False
                        End If

                        Dim ListViewData As String = UketukeNoDisp & "/" & _
                                                     FileItakuCode & "/" & _
                                                     FileItakuName & "/" & _
                                                     ToriInfo.FURIDATE & "/" & _
                                                     FileSyubetu & "/" & _
                                                     Format(CInt(dblFILE_SYORI_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_SYORI_KIN), "###,###,###,##0") & "/" & _
                                                     Format(CInt(dblFILE_FURI_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_FURI_KIN), "###,###,###,##0") & "/" & _
                                                     Format(CInt(dblFILE_FUNOU_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_FUNOU_KIN), "###,###,###,##0") & "/" & _
                                                     FileTorisCode & "-" & FileTorifCode
                        ListViewArray.Add(ListViewData)
                    Case "9"
                        ' NOP
                End Select

                dblRECORD_COUNT += 1
            Loop

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", ex.Message)
            Return False
        Finally
            If FileNo1_Open = "1" Then
                FileClose(intFILE_NO_1)
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "終了", "")
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
            SQL.Append("     TORIMAST")
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
                lblBaitai.Text = GetTextFileInfo(IniInfo.COMMON_TXT & "Common_媒体コード.TXT", GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T")))
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "委託者情報取得", "成功", "")
            Else
                lblItakuCode.Text = ""
                lblItakuName.Text = ""
                lblCodeKbn.Text = ""
                lblFileName.Text = ""
                lblBaitai.Text = ""
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
            ' コンボボックス初期化
            '--------------------------------
            cmbKakikomiHouhou.SelectedIndex = 0
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
            ' 2017/03/23 タスク）綾部 CHG 【ME】(RSV2対応) -------------------- START
            'txtFuriDateY.Text = ""
            'txtFuriDateM.Text = ""
            'txtFuriDateD.Text = ""
            '------------------------------------
            ' 振替日に前営業日を表示
            '------------------------------------
            Dim strSysDate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)
            Dim strGetdate As String = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            Dim bRet As Boolean = GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)
            txtFuriDateY.Text = strGetdate.Substring(0, 4)
            txtFuriDateM.Text = strGetdate.Substring(4, 2)
            txtFuriDateD.Text = strGetdate.Substring(6, 2)
            ' 2017/03/23 タスク）綾部 CHG 【ME】(RSV2対応) -------------------- END

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
    ' 取引先(構造体)情報クリア
    '================================
    Private Function ClearToriInfo() As Boolean

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先(構造体)情報クリア", "開始", "")

            '--------------------------------
            ' 構造体初期化
            '--------------------------------
            ToriInfo.TORIS_CODE = ""
            ToriInfo.TORIF_CODE = ""
            ToriInfo.ITAKU_CODE = ""
            ToriInfo.ITAKU_KANRI_CODE = ""
            ToriInfo.BAITAI_CODE = ""
            ToriInfo.CODE_KBN = ""
            ToriInfo.FMT_KBN = ""
            ToriInfo.MULTI_KBN = ""
            ToriInfo.FILE_NAME = ""
            ToriInfo.FURIDATE = ""
            ToriInfo.RECORD_LENGTH = ""
            ToriInfo.FTRANP = ""
            ToriInfo.FTRANP_IBM = ""
            ToriInfo.SFURI_FLG = ""
            ToriInfo.HENKAN_FILENAME = ""

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先(構造体)情報クリア", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先(構造体)情報クリア", "終了", "")
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
    ' コード変換情報取得
    '================================
    Private Function GetPFileInfo(ByRef Length As String, ByRef FtranP As String, ByRef FtranPIBM As String) As String

        Dim sr As StreamReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "コード変換情報取得", "開始", "")

            Length = ""
            FtranP = ""
            FtranPIBM = ""

            Select Case ToriInfo.FMT_KBN
                Case "00"
                    Select Case ToriInfo.CODE_KBN
                        Case "0"
                            Length = "120"
                            FtranP = "120JIS→JIS.P"        'JIS→JIS改
                            FtranPIBM = "120JIS→JIS.P"
                        Case "1"
                            Length = "120"
                            FtranP = "120JIS→JIS改.P"      'JIS→JIS改
                            FtranPIBM = "120JIS→JIS改.P"
                        Case "2"
                            Length = "119"
                            FtranP = "119JIS→JIS改.P"
                            FtranPIBM = "119JIS→JIS改.P"
                        Case "3"
                            Length = "118"
                            FtranP = "118JIS→JIS改.P"
                            FtranPIBM = "118JIS→JIS改.P"
                        Case "4"
                            Length = "120"
                            FtranP = "120.P"                'JIS→EBCDIC
                            FtranPIBM = "120IBM.P"
                    End Select
                Case "01"   'NHK
                    Select Case ToriInfo.CODE_KBN
                        Case "0"
                            Length = "120"
                            FtranP = "120JIS→JIS.P"        'JIS→JIS改
                            FtranPIBM = "120JIS→JIS.P"
                        Case "1"
                            Length = "120"
                            FtranP = "120JIS→JIS改.P"      'JIS→JIS改
                            FtranPIBM = "120JIS→JIS改.P"
                        Case "4"
                            Length = "120"
                            FtranP = "120.P"                'JIS→EBCDIC
                            FtranPIBM = "120IBM.P"
                    End Select
                Case "02"  '地公体
                    Select Case ToriInfo.CODE_KBN
                        Case "0"
                            Length = "390"
                            FtranP = "390JIS→JIS.P"        'JIS→JIS改
                            FtranPIBM = "390JIS→JIS.P"
                        Case "1"
                            Length = "390"
                            FtranP = "390JIS→JIS改.P"      'JIS→JIS改
                            FtranPIBM = "390JIS→JIS改.P"
                        Case "4"
                            Length = "390"
                            FtranP = "390.P"                'JIS→EBCDIC
                            FtranPIBM = "390IBM.P"
                    End Select
                Case Else
                    If IsNumeric(ToriInfo.FMT_KBN) Then
                        Dim nFmtKbn As Integer = CInt(ToriInfo.FMT_KBN)
                        'フォーマット区分が50～99の場合
                        If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                            ' XMLフォーマットのrootオブジェクト生成
                            Dim xmlDoc As New ConfigXmlDocument
                            Dim mXmlRoot As XmlElement
                            Dim node As XmlNode
                            Dim attribute As XmlAttribute
                            Dim sWork As String

                            'XMLパス作成
                            Dim xmlFolderPath As String = CASTCommon.GetFSKJIni("COMMON", "XML_FORMAT_FLD")
                            If xmlFolderPath = "err" Or xmlFolderPath = "" Then
                                Throw New Exception("fskj.iniでXML_FORMAT_FLDが定義されていません。")
                            End If
                            If xmlFolderPath.EndsWith("\") = False Then
                                xmlFolderPath &= "\"
                            End If
                            Dim mXmlFile As String = "XML_FORMAT_" & ToriInfo.FMT_KBN & ".xml"

                            xmlDoc.Load(xmlFolderPath & mXmlFile)
                            mXmlRoot = xmlDoc.DocumentElement

                            ' 返還ファイルを伝送ファイルにコピーする際のレコード長
                            node = mXmlRoot.SelectSingleNode("返還/コピー設定一覧/コピー設定[@コード区分='" & ToriInfo.CODE_KBN & "']")
                            If node Is Nothing Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定[@コード区分='" & ToriInfo.CODE_KBN & "']」タグが定義されていません。")
                            End If

                            attribute = node.Attributes.ItemOf("データ長")
                            If attribute Is Nothing Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「データ長」属性が定義されていません。（" & _
                                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                            End If
                            sWork = attribute.Value.Trim
                            If IsNumeric(sWork) = False OrElse CInt(sWork) <= 0 Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「データ長」属性の値（" & sWork & "）が不当です。（" & _
                                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                            End If
                            Length = sWork

                            ' 返還ファイルを伝送ファイルにコピーする際のパラメータファイル名
                            attribute = node.Attributes.ItemOf("パラメータファイル")
                            If attribute Is Nothing OrElse attribute.Value.Trim = "" Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「パラメータファイル」属性が定義されていません。（" & _
                                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                            End If
                            FtranP = attribute.Value.Trim

                            ' 返還ファイルをＦＤ３．５にコピーする際のパラメータファイル名
                            attribute = node.Attributes.ItemOf("IBMパラメータファイル")
                            If attribute Is Nothing OrElse attribute.Value.Trim = "" Then
                                Throw New Exception(mXmlFile & "定義エラー：" & "「返還/コピー設定一覧/コピー設定」タグの「IBMパラメータファイル」属性が定義されていません。（" & _
                                                    ConfigurationErrorsException.GetLineNumber(node) & "行目）")
                            End If
                            FtranPIBM = attribute.Value.Trim
                        End If
                    End If
            End Select

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "コード変換情報取得", "成功", _
                                 "Length" & Length & " / " & _
                                 "FtranP" & FtranP & " / " & _
                                 "FtranPIBM" & FtranPIBM)
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "コード変換情報取得", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "コード変換情報取得", "終了", "")
        End Try

    End Function

    '2016/02/05 タスク）岩城 RSV2対応 UPD ---------------------------------------- START
    Function BaitaiWrite_DataCheck_XML(ByVal astrCHK_SEND_FILE As String, ByVal aintREC_LENGTH As Integer) As Boolean
        Dim HenkanFMT As New CAstFormat.CFormat
        Dim sRet As String = ""

        Dim SQL As StringBuilder = Nothing
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim FileItakuCode As String = ""
        Dim FileItakuName As String = ""
        Dim FileSyubetu As String = ""
        Dim FileTorisCode As String = ""
        Dim FileTorifCode As String = ""

        Dim dblSCH_SYORI_KEN As Double
        Dim dblSCH_SYORI_KIN As Double
        Dim dblSCH_FURI_KEN As Double
        Dim dblSCH_FURI_KIN As Double
        Dim dblSCH_FUNOU_KEN As Double
        Dim dblSCH_FUNOU_KIN As Double
        Dim dblFILE_SYORI_KEN As Double
        Dim dblFILE_SYORI_KIN As Double
        Dim dblFILE_FURI_KEN As Double
        Dim dblFILE_FURI_KIN As Double
        Dim dblFILE_FUNOU_KEN As Double
        Dim dblFILE_FUNOU_KIN As Double
        Dim dblRECORD_COUNT As Double

        '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- START
        Dim UketukeNoDispFlg As Boolean = True
        '2016/02/08 タスク）岩城 RSV2対応 ADD ---------------------------------------- END

        Try
            MainDB = New CASTCommon.MyOracle
            Dim sw As System.Diagnostics.Stopwatch = Nothing

            dblRECORD_COUNT = 0

            Dim para As New CAstBatch.CommData.stPARAMETER
            para.FSYORI_KBN = "1"
            para.FMT_KBN = ToriInfo.FMT_KBN
            HenkanFMT = CAstFormat.CFormat.GetFormat(para)
            HenkanFMT.LOG = MainLOG

            If HenkanFMT.FirstRead(astrCHK_SEND_FILE) = 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "ファイル読込 対象ファイル:" & astrCHK_SEND_FILE)
                Return False
            End If

            Do Until HenkanFMT.EOF()
                dblRECORD_COUNT += 1
                sRet = HenkanFMT.CheckDataFormat()

                If HenkanFMT.IsHeaderRecord Then
                    '=========================================
                    ' ヘッダレコード
                    '=========================================
                    '2016/02/08 タスク）岩城 RSV2対応 UPD ---------------------------------------- START
                    If UketukeNoDisp = "" And UketukeNoDispFlg = True Then
                        UketukeNoDisp = UketukeNo
                        UketukeNoDispFlg = False
                    Else
                        UketukeNoDisp = ""
                    End If
                    'If UketukeNoDisp = "" Then
                    '    UketukeNoDisp = UketukeNo
                    'Else
                    '    UketukeNoDisp = ""
                    'End If
                    '2016/02/08 タスク）岩城 RSV2対応 UPD ---------------------------------------- END
                    '-----------------------------------------
                    ' 取引先コード検索
                    '-----------------------------------------
                    SQL = New StringBuilder
                    SQL.Length = 0
                    If HenkanFMT.InfoMeisaiMast.ITAKU_CODE = "" Then
                        Return False
                    Else
                        FileItakuCode = HenkanFMT.InfoMeisaiMast.ITAKU_CODE

                        SQL.Append("SELECT")
                        SQL.Append("     *")
                        SQL.Append(" FROM")
                        SQL.Append("     TORIMAST")
                        SQL.Append(" WHERE")
                        SQL.Append("     FSYORI_KBN_T = '1'")
                        SQL.Append(" AND ITAKU_CODE_T = '" & Trim(HenkanFMT.InfoMeisaiMast.ITAKU_CODE) & "'")
                    End If

                    OraReader = New CASTCommon.MyOracleReader(MainDB)
                    If OraReader.DataReader(SQL) = False Then            '取引先コードが存在しない
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "取引先該当なし　委託者コード:" & Trim(HenkanFMT.InfoMeisaiMast.ITAKU_CODE))
                        OraReader.Close()
                        OraReader = Nothing
                        Return False
                    Else
                        FileItakuName = OraReader.GetString("ITAKU_KNAME_T").Trim
                        FileTorisCode = OraReader.GetString("TORIS_CODE_T")
                        FileTorifCode = OraReader.GetString("TORIF_CODE_T")
                        If FileSyubetu = "" Then
                            FileSyubetu = OraReader.GetString("SYUBETU_T")
                        End If
                        OraReader.Close()
                        OraReader = Nothing
                    End If

                    '------------------------------------
                    'スケジュール検索
                    '------------------------------------
                    SQL.Length = 0
                    SQL.Append("SELECT")
                    SQL.Append("     *")
                    SQL.Append(" FROM")
                    SQL.Append("     SCHMAST")
                    SQL.Append(" WHERE")
                    SQL.Append("     FSYORI_KBN_S     = '1'")
                    SQL.Append(" AND ITAKU_CODE_S     = '" & Trim(HenkanFMT.InfoMeisaiMast.ITAKU_CODE) & "'")
                    SQL.Append(" AND FURI_DATE_S      = '" & Trim(ToriInfo.FURIDATE) & "'")
                    SQL.Append(" AND FUNOU_FLG_S      = '1'")
                    If ToriInfo.MULTI_KBN = "0" Then  'シングル
                        SQL.Append(" AND TORIS_CODE_S = '" & Trim(ToriInfo.TORIS_CODE) & "'")
                        SQL.Append(" AND TORIF_CODE_S = '" & Trim(ToriInfo.TORIF_CODE) & "'")
                    End If

                    OraReader = New CASTCommon.MyOracleReader(MainDB)
                    If OraReader.DataReader(SQL) = False Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "スケジュール該当なし")
                        OraReader.Close()
                        OraReader = Nothing
                        Return False
                    Else
                        dblSCH_SYORI_KEN = OraReader.GetInt64("SYORI_KEN_S")
                        dblSCH_SYORI_KIN = OraReader.GetInt64("SYORI_KIN_S")
                        dblSCH_FURI_KEN = OraReader.GetInt64("FURI_KEN_S")
                        dblSCH_FURI_KIN = OraReader.GetInt64("FURI_KIN_S")
                        dblSCH_FUNOU_KEN = OraReader.GetInt64("FUNOU_KEN_S")
                        dblSCH_FUNOU_KIN = OraReader.GetInt64("FUNOU_KIN_S")

                        OraReader.Close()
                        OraReader = Nothing
                    End If

                ElseIf HenkanFMT.IsTrailerRecord Then
                    '=========================================
                    ' トレーラレコード
                    '=========================================
                    dblFILE_SYORI_KEN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_IRAI_KEN_MOTO.Trim)
                    dblFILE_SYORI_KIN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_IRAI_KIN_MOTO.Trim)
                    dblFILE_FURI_KEN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_ZUMI_KEN_MOTO.Trim)
                    dblFILE_FURI_KIN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_ZUMI_KIN_MOTO.Trim)
                    dblFILE_FUNOU_KEN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_FUNO_KEN_MOTO.Trim)
                    dblFILE_FUNOU_KIN = CDbl(HenkanFMT.InfoMeisaiMast.TOTAL_FUNO_KIN_MOTO.Trim)

                    '-----------------------------------------
                    ' 件数・金額チェック
                    '-----------------------------------------
                    If dblSCH_SYORI_KEN <> dblFILE_SYORI_KEN Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "処理件数不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                    If dblSCH_SYORI_KIN <> dblFILE_SYORI_KIN Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "処理金額不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                    If dblSCH_FUNOU_KEN <> dblFILE_FUNOU_KEN Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "不能件数不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                    If dblSCH_FUNOU_KIN <> dblFILE_FUNOU_KIN Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", "不能金額不一致、レコードNO：" & dblRECORD_COUNT & Err.Description)
                        Return False
                    End If

                    Dim ListViewData As String = UketukeNoDisp & "/" & _
                                                     FileItakuCode & "/" & _
                                                     FileItakuName & "/" & _
                                                     ToriInfo.FURIDATE & "/" & _
                                                     FileSyubetu & "/" & _
                                                     Format(CInt(dblFILE_SYORI_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_SYORI_KIN), "###,###,###,##0") & "/" & _
                                                     Format(CInt(dblFILE_FURI_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_FURI_KIN), "###,###,###,##0") & "/" & _
                                                     Format(CInt(dblFILE_FUNOU_KEN), "###,##0") & "/" & _
                                                     Format(CLng(dblFILE_FUNOU_KIN), "###,###,###,##0") & "/" & _
                                                     FileTorisCode & "-" & FileTorifCode
                    ListViewArray.Add(ListViewData)
                End If
            Loop

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "成功", "")

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "失敗", ex.Message)
            Return False
        Finally
            If Not HenkanFMT Is Nothing Then
                HenkanFMT.Close()
                HenkanFMT = Nothing
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "媒体データチェック", "終了", "")
        End Try
    End Function
    '2016/02/05 タスク）岩城 RSV2対応 UPD ---------------------------------------- END
#End Region

#Region " 関数(Sub) "

    '================================
    ' テキストボックス0埋め
    '================================
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtTorisCode.Validating, txtTorifCode.Validating, txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating

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
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先コード入力処理", "失敗", ex.Message)
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
                ' コンボボックス設定
                '--------------------------------
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
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
    Private Sub rdb_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Try
            '--------------------------------
            ' 追加条件設定
            '--------------------------------
            Dim Jyoken As String = " AND BAITAI_CODE_T IN ('01', '11', '12', '13', '14', '15')"   '媒体コードが媒体のもの

            '--------------------------------
            ' コンボボックス設定
            '--------------------------------
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
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
' 2016/01/11 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END
