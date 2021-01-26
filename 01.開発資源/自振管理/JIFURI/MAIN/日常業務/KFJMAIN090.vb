' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
Imports System.IO
Imports System.Text
Imports CASTCommon

Public Class KFJMAIN090

#Region " 宣言 "

    Private MainLog As New CASTCommon.BatchLOG("KFJMAIN090", "依頼データ落込（一括）画面")
    Private Const msgTitle As String = "依頼データ落込（一括）画面(KFJMAIN090)"
    Private CAST As New CASTCommon.Events

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
        Dim COMMON_DEN As String                     ' DENフォルダ
        Dim COMMON_BAITAIREAD As String              ' 媒体読込データ格納フォルダ
        Dim COMMON_ETC As String                     ' ETCフォルダ
    End Structure
    Private IniInfo As INI_INFO

    Private FuriKbn_Nname As String = ""

#End Region

#Region " ロード "

    Private Sub KFJMAIN090_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            Select Case GCom.SetComboBox(cmbBaitai, "KFJMAIN090_媒体.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "媒体", "KFJMAIN090_媒体.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", "ファイルなし。ファイル:KFJMAIN090_媒体.TXT")
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "媒体"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ロード", "失敗", "コンボボックス設定失敗 コンボボックス名:媒体")
            End Select
            cmbBaitai.SelectedIndex = 0

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

#Region "ボタン"

    '=================================
    ' 実行ボタン
    '=================================
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim GyomuCode As String = MyClass.Name.Substring(2, 1)
        Dim SelectBaitaiCode As String = Nothing
        Dim SelectBaitaiKCode As String = Nothing
        Dim FileList() As String

        Try
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "開始", "")
            Cursor.Current = Cursors.WaitCursor()

            '---------------------------------
            ' ファイル格納フォルダ設定
            '---------------------------------
            SelectBaitaiCode = Format(DirectCast(cmbBaitai.SelectedItem, MenteCommon.clsAddItem).Data1, "00")
            Select Case SelectBaitaiCode
                Case "00"
                    FileList = Directory.GetFiles(IniInfo.COMMON_DEN)
                Case "04", "09"
                    FileList = Directory.GetFiles(IniInfo.COMMON_ETC)
                Case Else
                    FileList = Directory.GetFiles(IniInfo.COMMON_BAITAIREAD)
            End Select

            '---------------------------------
            ' 媒体名設定(媒体命名規約)
            '---------------------------------
            SelectBaitaiKCode = GetTextFileInfo(IniInfo.COMMON_TXT & "媒体命名規約.txt", SelectBaitaiCode)

            '---------------------------------
            ' 対象ファイル件数チェック 
            '---------------------------------
            Dim FileCount As Integer = 0
            For i As Integer = 0 To FileList.Length - 1 Step 1
                Dim SubFileName() As String = Path.GetFileName(FileList(i)).Split("_")
                If SubFileName(0) = GyomuCode Then
                    If SubFileName(1) = SelectBaitaiKCode Then
                        FileCount += 1
                    End If
                End If
            Next

            If FileCount = 0 Then
                MessageBox.Show(String.Format(MSG0373W, "処理対象"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Try
            End If

            '---------------------------------
            ' 対象ファイル取得 
            '---------------------------------
            MainDB = New CASTCommon.MyOracle
            For i As Integer = 0 To FileList.Length - 1 Step 1
                Dim SubFileName() As String = Path.GetFileName(FileList(i)).Split("_")
                If SubFileName(0) = GyomuCode Then
                    If SubFileName(1) = SelectBaitaiKCode Then
                        '---------------------------------
                        ' 取引先コード取得
                        '---------------------------------
                        Dim TorisCode As String = ""
                        Dim TorifCode As String = ""
                        Dim CodeKbn As String = ""
                        Dim LabelKbn As String = ""
                        If GetMastInfo(MainDB, GyomuCode, SubFileName(2), SubFileName(3), SubFileName(5), _
                                       TorisCode, TorifCode, CodeKbn, LabelKbn) = True Then
                            '---------------------------------
                            ' ＪＯＢ登録(対象ファイル数分)
                            '---------------------------------
                            Dim TourokuParam As String = TorisCode & TorifCode & "," & _
                                                         SubFileName(5) & "," & _
                                                         CodeKbn & "," & _
                                                         SubFileName(4) & "," & _
                                                         SelectBaitaiCode & "," & _
                                                         LabelKbn
                            If SetJobMast(MainDB, "J010", TourokuParam) = False Then
                                MessageBox.Show(String.Format(MSG0371W, "一括落込", "処理を中断します。"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                Exit Try
                            End If
                        End If
                    End If
                End If
            Next

            MessageBox.Show(String.Format(MSG0021I, "一括落込"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "失敗", ex.Message)
        Finally
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

#Region "関数"

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

            IniInfo.COMMON_DEN = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If IniInfo.COMMON_DEN.ToUpper = "ERR" OrElse IniInfo.COMMON_DEN = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "DENフォルダ", "COMMON", "DEN"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:DENフォルダ 分類:COMMON 項目:DEN")
                Return False
            End If

            IniInfo.COMMON_BAITAIREAD = CASTCommon.GetFSKJIni("COMMON", "BAITAIREAD")
            If IniInfo.COMMON_BAITAIREAD.ToUpper = "ERR" OrElse IniInfo.COMMON_BAITAIREAD = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "読込データ格納フォルダ", "COMMON", "BAITAIREAD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:読込データ格納フォルダ 分類:COMMON 項目:BAITAIREAD")
                Return False
            End If

            IniInfo.COMMON_ETC = CASTCommon.GetFSKJIni("COMMON", "ETC")
            If IniInfo.COMMON_ETC.ToUpper = "ERR" OrElse IniInfo.COMMON_ETC = Nothing Then
                MessageBox.Show(String.Format(MSG0001E, "ETCフォルダ", "COMMON", "ETC"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "INI情報取得", "失敗", "項目名:ETCフォルダ 分類:COMMON 項目:ETC")
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
    ' テキストファイル情報取得
    '================================
    Private Function GetTextFileInfo(ByVal TextFileName As String, ByVal KeyInfo As String) As String

        Dim sr As StreamReader = Nothing

        Try
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストファイル読込", "開始", TextFileName)

            sr = New StreamReader(TextFileName, Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLineData() As String = sr.ReadLine().Split(","c)
                If strLineData(0) = KeyInfo Then
                    Return strLineData(1).Trim
                End If
            End While

            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストファイル読込", "", "該当なし")
            Return "NON"

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストファイル読込", "", ex.Message)
            Return "ERR"
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "テキストファイル読込", "終了", "")
        End Try

    End Function

    '================================
    ' 取引先コード取得
    '================================
    Private Function GetMastInfo(ByVal MainDB As CASTCommon.MyOracle, ByVal GyomuCode As String, ByVal MultiKbn As String, ByVal SearchCode As String, ByVal FuriDate As String, _
                                 ByRef TorisCode As String, ByRef TorifCode As String, ByRef CodeKbn As String, ByRef LabelKbn As String) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As StringBuilder = Nothing
        Dim SelectMast As String = Nothing

        Try
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先、スケジュール取得", "開始", "")

            '--------------------------------
            ' 処理初期設定
            '--------------------------------
            SQL = New StringBuilder(128)
            TorisCode = ""
            TorisCode = ""

            '--------------------------------
            ' 取引先マスタチェック
            '--------------------------------
            SQL.Length = 0
            Select Case GyomuCode
                Case "J"
                    SelectMast = " TORIMAST "
                Case Else
                    SelectMast = " S_TORIMAST "
            End Select

            Select Case MultiKbn
                Case "S"
                    '--------------------------------
                    ' シングル先
                    '--------------------------------
                    SQL.Append("SELECT")
                    SQL.Append("     TORIS_CODE_T")
                    SQL.Append("   , TORIF_CODE_T")
                    SQL.Append("   , CODE_KBN_T")
                    SQL.Append("   , LABEL_KBN_T")
                    SQL.Append(" FROM")
                    SQL.Append(SelectMast)
                    SQL.Append(" WHERE")
                    SQL.Append("     TORIS_CODE_T   = '" & SearchCode.Substring(0, 10) & "'")
                    SQL.Append(" AND TORIF_CODE_T   = '" & SearchCode.Substring(10, 2) & "'")
                Case Else
                    '--------------------------------
                    ' マルチ先
                    '--------------------------------
                    SQL.Append("SELECT")
                    SQL.Append("     TORIS_CODE_T")
                    SQL.Append("   , TORIF_CODE_T")
                    SQL.Append("   , CODE_KBN_T")
                    SQL.Append("   , LABEL_KBN_T")
                    SQL.Append(" FROM")
                    SQL.Append(SelectMast)
                    SQL.Append(" WHERE")
                    SQL.Append("     ITAKU_KANRI_CODE_T  = '" & SearchCode.Substring(0, 10) & "'")
                    SQL.Append(" AND ITAKU_KANRI_CODE_T  = ITAKU_CODE_T")
            End Select

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                Select Case MultiKbn
                    Case "S"
                        MessageBox.Show(String.Format(MSG0002E, "検索") & vbCrLf & "  取引先コード:" & SearchCode.Substring(0, 10) & "-" & SearchCode.Substring(10, 2), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先、スケジュール取得", "失敗", "取引先マスタ該当なし　取引先コード:" & SearchCode.Substring(0, 10) & "-" & SearchCode.Substring(10, 2))
                    Case Else
                        MessageBox.Show(String.Format(MSG0002E, "検索") & vbCrLf & "  代表委託者コード:" & SearchCode.Substring(0, 10), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先、スケジュール取得", "失敗", "取引先マスタ該当なし　代表委託者コード:" & SearchCode.Substring(0, 10))
                End Select
                Return False
            Else
                TorisCode = OraReader.GetString("TORIS_CODE_T")
                TorifCode = OraReader.GetString("TORIF_CODE_T")
                CodeKbn = OraReader.GetString("CODE_KBN_T")
                LabelKbn = OraReader.GetString("LABEL_KBN_T")
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            '--------------------------------
            ' シングルはここで終了
            '--------------------------------
            If MultiKbn = "S" Then
                Return True
            End If

            '--------------------------------
            ' スケジュールから取得(マルチのみ)
            '--------------------------------
            SQL.Length = 0
            Select Case GyomuCode
                Case "J"
                    SelectMast = " TORIMAST , SCHMAST "
                Case Else
                    SelectMast = " S_TORIMAST , S_SCHMAST"
            End Select

            '--------------------------------
            ' マルチ先(代表を先頭にする)
            '--------------------------------
            SQL.Append("SELECT")
            SQL.Append("     TORIS_CODE_T")
            SQL.Append("   , TORIF_CODE_T")
            SQL.Append("   , CODE_KBN_T")
            SQL.Append("   , LABEL_KBN_T")
            SQL.Append("   , FURI_DATE_S")
            SQL.Append("   , '0' AS SORTKBN")
            SQL.Append(" FROM")
            SQL.Append(SelectMast)
            SQL.Append(" WHERE")
            SQL.Append("     ITAKU_KANRI_CODE_T  = '" & SearchCode.Substring(0, 10) & "'")
            SQL.Append(" AND ITAKU_KANRI_CODE_T  = ITAKU_CODE_T")
            SQL.Append(" AND TORIS_CODE_T        = TORIS_CODE_S(+)")
            SQL.Append(" AND TORIF_CODE_T        = TORIF_CODE_S(+)")
            SQL.Append(" AND FURI_DATE_S(+)      = '" & FuriDate & "'")
            SQL.Append(" UNION ")
            SQL.Append("SELECT")
            SQL.Append("     TORIS_CODE_T")
            SQL.Append("   , TORIF_CODE_T")
            SQL.Append("   , CODE_KBN_T")
            SQL.Append("   , LABEL_KBN_T")
            SQL.Append("   , FURI_DATE_S")
            SQL.Append("   , '1' AS SORTKBN")
            SQL.Append(" FROM")
            SQL.Append(SelectMast)
            SQL.Append(" WHERE")
            SQL.Append("     ITAKU_KANRI_CODE_T  = '" & SearchCode.Substring(0, 10) & "'")
            SQL.Append(" AND ITAKU_KANRI_CODE_T <> ITAKU_CODE_T")
            SQL.Append(" AND TORIS_CODE_T        = TORIS_CODE_S(+)")
            SQL.Append(" AND TORIF_CODE_T        = TORIF_CODE_S(+)")
            SQL.Append(" AND FURI_DATE_S(+)      = '" & FuriDate & "'")
            SQL.Append(" ORDER BY")
            SQL.Append("     FURI_DATE_S")
            SQL.Append("   , SORTKBN")
            SQL.Append("   , TORIS_CODE_T")
            SQL.Append("   , TORIF_CODE_T")

            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先、スケジュール取得", "失敗", "スケジュールマスタ検索失敗　代表委託者コード:" & SearchCode.Substring(0, 10) & " 振替日:" & FuriDate)
                Return False
            Else
                TorisCode = OraReader.GetString("TORIS_CODE_T")
                TorifCode = OraReader.GetString("TORIF_CODE_T")
                CodeKbn = OraReader.GetString("CODE_KBN_T")
                LabelKbn = OraReader.GetString("LABEL_KBN_T")
            End If

            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If

            Return True

        Catch ex As Exception
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先、スケジュール取得", "", ex.Message)
            Return False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "取引先、スケジュール取得", "終了", "")
        End Try

    End Function

#End Region

End Class
' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END