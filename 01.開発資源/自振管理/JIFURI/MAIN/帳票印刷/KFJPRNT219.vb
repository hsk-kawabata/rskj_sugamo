Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJPRNT219
    Private MainLOG As New CASTCommon.BatchLOG("KFJPRNT219", "店別集計表印刷画面")
    Private Const msgTitle As String = "店別集計表印刷画面(KFJPRNT219)"
    Private Const errMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    '2010/10/05.Sakon　ALL9対策 ++++++++++
    Private TORICODE As New ArrayList
    Private FUNOUFLG As New ArrayList
    '+++++++++++++++++++++++++++++++++++++

#Region "宣言"
    Dim strTORIS_CODE As String
    Dim strTORIF_CODE As String
    Dim strFURI_DATE As String

    Public strFLG As String = ""
    Public strFMT As String = "" 'フォーマット区分

#End Region

    Private Sub KFJPRNT219_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '--------------------------------
            '委託者名リストボックスの設定
            '--------------------------------
            '取引先コンボボックス設定
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            Dim Msg As String
            Select Case GCom.SetComboBox(cmbPrintKbn, "KFJPRNT219_対象帳票.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "対象帳票", "KFJPRNT219_対象帳票.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "対象帳票設定ファイルなし。ファイル:KFJPRNT219_対象帳票.TXT"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "対象帳票"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "コンボボックス設定失敗 コンボボックス名:対象帳票"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
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


#Region " 印刷"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :印刷ボタン
        'Return         :
        'Create         :2009/09/19
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")

            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Return
            End If

            strTORIS_CODE = txtTorisCode.Text
            strTORIF_CODE = txtTorifCode.Text
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            LW.ToriCode = strTORIS_CODE + strTORIF_CODE
            LW.FuriDate = strFURI_DATE
            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            Dim PrintID As String = ""
            Dim PrintName As String = ""
            Dim Funou As String = "0"
            Select Case GCom.GetComboBox(cmbPrintKbn)
                Case 0 '通常
                    PrintID = "KFJP019"
                    PrintName = "口座振替店別集計表"
                    Funou = strFLG
                Case 1 '県立高校用
                    Select Case strFMT
                        Case "51", "53"
                            PrintID = "KFJP219"
                            PrintName = "口座振替店別集計表(県立高校)"
                            Funou = strFLG
                        Case Else
                            MessageBox.Show("指定の帳票種類では印刷できません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                    End Select
                  
                Case 2 '小松市税

                    If strFMT <> "54" Then
                        MessageBox.Show("指定の帳票種類では印刷できません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    Else
                        PrintID = "KFJP220"
                        PrintName = "口座振替店別集計表(小松市税)"
                        Funou = strFLG
                    End If

            End Select

            If MessageBox.Show(String.Format(MSG0013I, PrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim param As String
            Dim nRet As Integer
            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            '2010/10/05.Sakon　ALL9対応 +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim i As Integer

            For i = 0 To TORICODE.Count - 1
                'パラメータセット
                param = TORICODE(i).ToString & "," & strFURI_DATE & "," & FUNOUFLG(i).ToString
                nRet = ExeRepo.ExecReport(PrintID & ".EXE", param)

                '戻り値に対応したメッセージを表示する(ALL9時は失敗メッセージのみ表示する)
                Select Case nRet
                    Case 0
                        If txtTorisCode.Text & txtTorifCode.Text <> "999999999999" Then
                            '印刷後確認メッセージ
                            MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    Case -1
                        '印刷対象なしメッセージ
                        MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select

            Next

            TORICODE.Clear()    '2010/11/11 ADD

            'ALL9時はここで完了メッセージを表示する
            If txtTorisCode.Text & txtTorifCode.Text = "999999999999" Then
                '印刷後確認メッセージ
                MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            ''param = GCom.GetUserID & "," & strTORIS_CODE + strTORIF_CODE & "," & strFURI_DATE & "," & Funou
            'param = strTORIS_CODE + strTORIF_CODE & "," & strFURI_DATE & "," & Funou
            'nRet = ExeRepo.ExecReport(PrintID & ".EXE", param)

            ''戻り値に対応したメッセージを表示する
            'Select Case nRet
            '    Case 0
            '        '印刷後確認メッセージ
            '        MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            '    Case -1
            '        '印刷対象なしメッセージ
            '        MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Case Else
            '        '印刷失敗メッセージ
            '        MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            'End Select
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            txtTorisCode.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

        End Try

    End Sub

#End Region

#Region "関数"
    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :印刷ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/19
        'Update         :
        '============================================================================
        fn_check_text = False
        Try
            '取引先主コード必須チェック
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If
            '取引先副コード必須チェック
            If txtTorifCode.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorifCode.Focus()
                Return False
            End If
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
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then
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
            fn_check_text = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try
    End Function
    Private Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :印刷ボタンを押下時にマスタ相関チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/19
        'Update         :2010/10/05：ALL9対応追加
        '============================================================================
        fn_check_Table = False
        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        fn_check_Table = False
        Dim lngDataCNT As Long = 0
        Try
            '------------------------------------------
            '取引先マスタ存在チェック
            '------------------------------------------
            If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                '取引先情報取得
                SQL.Append("SELECT ")
                SQL.Append(" COUNT(*) COUNTER")
                SQL.Append(" FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))
                If OraReader.DataReader(SQL) = True Then
                    lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                    MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    txtTorisCode.Focus()
                    Return False
                End If
                If lngDataCNT = 0 Then
                    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    OraReader.Close()
                    txtTorisCode.Focus()
                    Return False
                End If
            End If

            SQL = New StringBuilder(128)
            '------------------------------------------
            'スケジュールマスタ存在チェック
            '------------------------------------------
            '登録済みスケジュールが存在するかチェック
            SQL.Append(" SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM SCHMAST,TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" FURI_DATE_S = " & SQ(strFURI_DATE))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            'SQL.Append(" AND FUNOU_FLG_S = '1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                SQL.Append(" AND TORIS_CODE_S = " & SQ(strTORIS_CODE))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            End If

            If OraReader.DataReader(SQL) = True Then
                lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtFuriDateY.Focus()
                Return False
            End If
            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            SQL = New StringBuilder(128)
            '------------------------------------------
            '明細マスタ存在チェック
            '------------------------------------------
            SQL.Append(" SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM SCHMAST,MEIMAST,TORIMAST")
            SQL.Append(" WHERE ")
            SQL.Append(" FURI_DATE_S = " & SQ(strFURI_DATE))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            'SQL.Append(" AND FUNOU_FLG_S = '1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_K")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_K")
            SQL.Append(" AND FURI_DATE_S = FURI_DATE_K")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            '0件ﾃﾞｰﾀを印刷対象とする場合、このSQLは不要 ============
            SQL.Append(" AND ((FMT_KBN_T <> '02' AND DATA_KBN_K = '2')")   '国税以外のデータレコード
            SQL.Append(" OR (FMT_KBN_T = '02' AND DATA_KBN_K = '3'))")     '国税のデータレコード
            '=======================================================
            If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                SQL.Append(" AND TORIS_CODE_S = " & SQ(strTORIS_CODE))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            End If


            If OraReader.DataReader(SQL) = True Then
                lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtFuriDateY.Focus()
                Return False
            End If
            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            'ALL9対策：通常時はフォーマットチェックを行わない +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            'フォーマットチェック
            If GCom.GetComboBox(cmbPrintKbn) <> 0 Then
                SQL = New StringBuilder(128)

                SQL.Append(" SELECT")
                SQL.Append(" FMT_KBN_T")
                SQL.Append(" FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
                SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))

                If OraReader.DataReader(SQL) = True Then
                    strFMT = OraReader.GetItem("FMT_KBN_T")
                Else
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                    MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    txtFuriDateY.Focus()
                    Return False
                End If
                If strFMT.Trim = "" Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtFuriDateY.Focus()
                    Return False
                End If
            End If
            'SQL = New StringBuilder(128)

            'SQL.Append(" SELECT")
            'SQL.Append(" FMT_KBN_T")
            'SQL.Append(" FROM TORIMAST")
            'SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
            'SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))

            'If OraReader.DataReader(SQL) = True Then
            '    strFMT = OraReader.GetItem("FMT_KBN_T")
            'Else
            '    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
            '    MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '    txtFuriDateY.Focus()
            '    Return False
            'End If
            'If strFMT.Trim = "" Then
            '    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    txtFuriDateY.Focus()
            '    Return False
            'End If
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            'パラメータ項目を保持（取引先コード・不能フラグ）
            SQL = New StringBuilder(128)

            '2017/06/22 saitou MODIFY 標準版修正 ---------------------------------------- START
            'スケジュールが存在して、明細マスタが存在しない場合（大阪地区の日報など）に、
            '明細マスタが存在しない取引先が全て0件・0円で出力されるのを修正。
            SQL.Append("SELECT")
            SQL.Append("     TORIS_CODE_S")
            SQL.Append("    ,TORIF_CODE_S")
            SQL.Append("    ,FUNOU_FLG_S")
            SQL.Append("    ,COUNT(*)")
            SQL.Append(" FROM")
            SQL.Append("     SCHMAST")
            SQL.Append("    ,TORIMAST")
            SQL.Append("    ,MEIMAST")
            SQL.Append(" WHERE")
            SQL.Append("     FURI_DATE_S = " & SQ(strFURI_DATE))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" AND TOUROKU_FLG_S = '1'")
            If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
                SQL.Append(" AND TORIS_CODE_S = " & SQ(strTORIS_CODE))
                SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            End If
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_K")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_K")
            SQL.Append(" AND FURI_DATE_S = FURI_DATE_K")
            SQL.Append(" GROUP BY")
            SQL.Append("     TORIS_CODE_S")
            SQL.Append("    ,TORIF_CODE_S")
            SQL.Append("    ,FUNOU_FLG_S")
            SQL.Append(" HAVING")
            SQL.Append("     COUNT(*) > 0")
            SQL.Append(" ORDER BY")
            SQL.Append("     TORIS_CODE_S")
            SQL.Append("    ,TORIF_CODE_S")

            'SQL.Append(" SELECT")
            'SQL.Append(" TORIS_CODE_S")
            'SQL.Append(",TORIF_CODE_S")
            'SQL.Append(",FUNOU_FLG_S")
            'SQL.Append(" FROM SCHMAST")
            'SQL.Append(" WHERE ")
            'SQL.Append(" FURI_DATE_S = " & SQ(strFURI_DATE))
            'SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            'SQL.Append(" AND TOUROKU_FLG_S = '1'")
            'If strTORIS_CODE & strTORIF_CODE <> "999999999999" Then
            '    SQL.Append(" AND TORIS_CODE_S = " & SQ(strTORIS_CODE))
            '    SQL.Append(" AND TORIF_CODE_S = " & SQ(strTORIF_CODE))
            'End If
            '2017/06/22 saitou MODIFY --------------------------------------------------- END

            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    TORICODE.Add(OraReader.GetItem("TORIS_CODE_S") & OraReader.GetItem("TORIF_CODE_S"))
                    FUNOUFLG.Add(OraReader.GetItem("FUNOU_FLG_S"))
                    OraReader.NextRead()
                End While
                'strFLG = OraReader.GetItem("FUNOU_FLG_S")
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtFuriDateY.Focus()
                Return False
            End If
            'If strFLG = "" Then
            If TORICODE.Count = 0 Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            fn_check_Table = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function
#End Region
#Region " イベント"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '委託者名リストボックスの設定
        '--------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                cmbToriName.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コンボボックス設定)", "失敗", ex.ToString)
        End Try
    End Sub

    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        '-------------------------------------------
        '取引先コードテキストボックスに取引先コード設定
        '-------------------------------------------
        Try
            '取引先コンボボックス設定
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コード取得)", "失敗", ex.ToString)
        End Try

    End Sub
    'ゼロ埋め
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtTorisCode.Validating, txtTorifCode.Validating, txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub
#End Region

End Class
