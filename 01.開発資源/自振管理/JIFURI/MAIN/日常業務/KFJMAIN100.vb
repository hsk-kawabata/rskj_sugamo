' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
Imports System.IO
Imports System.Text
Imports CASTCommon

Public Class KFJMAIN100

#Region " 宣言 "

    Private MainLog As New CASTCommon.BatchLOG("KFJMAIN100", "返還データ作成（一括）画面")
    Private Const msgTitle As String = "返還データ作成（一括）画面(KFJMAIN100)"
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainDB As CASTCommon.MyOracle

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
        Dim COMMON_TXT As String                     ' TXTフォルダ
    End Structure
    Private IniInfo As INI_INFO

#End Region

#Region " ロード "

    Private Sub KFJMAIN100_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim SystemDate As String = String.Empty

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "開始", "")

            '------------------------------------------
            'システム日付とユーザ名を表示
            '------------------------------------------
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '------------------------------------------
            ' コンボボックス設定
            '------------------------------------------
            Select Case GCom.SetComboBox(cmbBaitai, "KFJMAIN100_媒体.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "媒体", "KFJMAIN100_媒体.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", "ファイルなし。ファイル:KFJMAIN100_媒体.TXT")
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "媒体"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", "コンボボックス設定失敗 コンボボックス名:媒体")
            End Select
            cmbBaitai.SelectedIndex = 0

            '------------------------------------------
            ' 日付初期設定
            '------------------------------------------
            SystemDate = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
            txtHenkanYDateY.Text = SystemDate.Substring(0, 4)
            txtHenkanYDateM.Text = SystemDate.Substring(4, 2)
            txtHenkanYDateD.Text = SystemDate.Substring(6, 2)

            '--------------------------------
            ' INI情報取得
            '--------------------------------
            If SetIniFIle() = False Then
                Exit Try
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", ex.Message)
            Me.Close()
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "終了", "")
        End Try
    End Sub

#End Region

#Region " ボタン "

    '=================================
    ' 実行ボタン
    '=================================
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim SelectBaitaiCode As String = String.Empty
        Dim InputHenkanYDate As String = String.Empty
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing
        Dim JobInsertCount As Integer = 0

        Try
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "開始", "")
            Cursor.Current = Cursors.WaitCursor()

            '---------------------------------
            ' 画面情報取得
            '---------------------------------
            SelectBaitaiCode = Format(DirectCast(cmbBaitai.SelectedItem, MenteCommon.clsAddItem).Data1, "00")
            InputHenkanYDate = txtHenkanYDateY.Text & txtHenkanYDateM.Text & txtHenkanYDateD.Text
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "開始", "媒体:" & SelectBaitaiCode & " / 返還予定日:" & InputHenkanYDate)

            '---------------------------------
            ' スケジュールマスタチェック
            '---------------------------------
            SQL = New StringBuilder(128)
            If MakeSQL(SelectBaitaiCode, InputHenkanYDate, SQL) = False Then
                MessageBox.Show(String.Format(MSG0371W, "対象条件構築処理", "処理を中断します。"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Try
            End If

            MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(MSG0086W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "失敗", "対象スケジュールなし")
                Exit Try
            Else
                '2018/06/15 saitou 広島信金(RSV2) ADD ---------------------------------------- START
                '処理前にメッセージ出力
                If MessageBox.Show(String.Format(MSG0023I, "一括返還"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    Return
                End If
                '2018/06/15 saitou 広島信金(RSV2) ADD ---------------------------------------- END

                While OraReader.EOF = False
                    Dim ToriCode As String = OraReader.GetString("TORIS_CODE_S") & OraReader.GetString("TORIF_CODE_S")
                    Dim FuriDate As String = OraReader.GetString("FURI_DATE_S")
                    Dim MotikomiSeq As Integer = OraReader.GetInt("MOTIKOMI_SEQ_S")

                    Dim TourokuParam As String = ToriCode & "," & _
                                                 FuriDate & "," & _
                                                 CStr(MotikomiSeq).PadLeft(2, "0"c)

                    If SetJobMast(MainDB, "J060", TourokuParam) = False Then
                        MessageBox.Show(String.Format(MSG0371W, "一括返還", "処理を中断します。"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "失敗", "JOB登録(J060) パラメータ:" & TourokuParam)
                        Exit Try
                    End If

                    JobInsertCount += 1
                    OraReader.NextRead()
                End While
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If

            MessageBox.Show(String.Format(MSG0021I, "一括返還"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "成功", "JOB登録件数" & JobInsertCount & "件")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "失敗", ex.Message)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "終了", "")
        End Try
    End Sub

    '=================================
    ' 終了ボタン
    '=================================
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "開始", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "失敗", ex.Message)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "クローズ", "終了", "")
        End Try
    End Sub

#End Region

#Region " 関数(Function) "

    '================================
    ' INI情報取得
    '================================
    Private Function SetIniFIle() As Boolean

        Try

            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "開始", "")

            IniInfo.COMMON_TXT = CASTCommon.GetFSKJIni("COMMON", "TXT")
            If IniInfo.COMMON_TXT.ToUpper = "ERR" OrElse IniInfo.COMMON_TXT = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "TXTフォルダ", "COMMON", "TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:TXTフォルダ 分類:COMMON 項目:TXT")
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "", ex.Message)
            Return False
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "終了", "")
        End Try
    End Function

    '================================
    ' JOB登録(落込)
    '================================
    Private Function SetJobMast(ByVal MainDB As CASTCommon.MyOracle, ByVal JobID As String, ByVal TourokuParam As String) As Boolean

        Try
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ジョブ登録", "開始", "")

            MainDB.BeginTrans()

            Dim iRet As Integer = MainLog.SearchJOBMAST(JobID, TourokuParam, MainDB)
            If iRet = 1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            ElseIf iRet = -1 Then
                MainDB.Rollback()
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            If MainLog.InsertJOBMAST(JobID, GCom.GetUserID, TourokuParam, MainDB) = False Then
                MainDB.Rollback()
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            MainDB.Commit()

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ジョブ登録", "失敗", ex.Message)
            Return False
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ジョブ登録", "終了", "")
        End Try

    End Function

    '================================
    ' 返還対象抽出ＳＱＬ作成
    '================================
    Private Function MakeSQL(ByVal BaitaiCode As String, ByVal HenkanYDate As String, _
                             ByRef SQL As StringBuilder) As Boolean

        Try
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "返還対象抽出ＳＱＬ作成", "開始", "")

            '--------------------------------
            ' ＳＱＬ作成
            '--------------------------------
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     TORIS_CODE_S")
            SQL.Append("   , TORIF_CODE_S")
            SQL.Append("   , FURI_DATE_S")
            SQL.Append("   , MOTIKOMI_SEQ_S")
            SQL.Append(" FROM")
            SQL.Append("     SCHMAST")
            SQL.Append("   , TORIMAST")
            SQL.Append(" WHERE")
            SQL.Append("     BAITAI_CODE_S         = '" & BaitaiCode & "'")
            SQL.Append(" AND HENKAN_YDATE_S        = '" & HenkanYDate & "'")
            SQL.Append(" AND TOUROKU_FLG_S         = '1'")
            SQL.Append(" AND FUNOU_FLG_S           = '1'")
            SQL.Append(" AND HENKAN_FLG_S         <> '1'")
            SQL.Append(" AND TYUUDAN_FLG_S         = '0'")
            SQL.Append(" AND FILE_SEQ_S            = 1")
            SQL.Append(" AND TORIS_CODE_S          = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S          = TORIF_CODE_T")
            SQL.Append(" AND BAITAI_CODE_T        <> '07'")
            SQL.Append(" AND MOTIKOMI_KBN_T       <> '1'")
            SQL.Append(" AND KEKKA_HENKYAKU_KBN_T <> '0'")
            SQL.Append(" ORDER BY")
            SQL.Append("     FURI_DATE_S")
            SQL.Append("   , TORIS_CODE_S")
            SQL.Append("   , TORIF_CODE_S")

            Return True

        Catch ex As Exception
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "返還対象抽出ＳＱＬ作成", "", ex.Message)
            Return False
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "返還対象抽出ＳＱＬ作成", "終了", "")
        End Try

    End Function

#End Region

#Region " 関数(Sub) "

    '================================
    ' テキストボックス0埋め
    '================================
    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtHenkanYDateY.Validating, txtHenkanYDateM.Validating, txtHenkanYDateD.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MainLog.Write_Err(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストボックス0埋め", "", ex)
        Finally
            ' NOP
        End Try
    End Sub

#End Region

End Class
' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END