' ============================================================================
'  HISTORY
'   No  Ver     Date          Name              Comment
'   01  V01L01  2020/06/16    FJH)AMANO         ＰＫＧ修正(PKG_2020_0012_000)
' ============================================================================
Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text

Public Class KFGPRNT050
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 収納状況一覧表印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

#Region " 共通変数定義 "
    Dim Str_Report_Path As String
    Dim Str_Report_Path2 As String
    'Dim Str_Connection As String

    'Dim OBJ_CONNECTION2 As New OleDb.OleDbConnection
    'Dim OBJ_DATAREADER2 As OleDb.OleDbDataReader
    'Dim OBJ_COMMAND2 As New OleDb.OleDbCommand

    'Dim OBJ_CONNECTION3 As New OleDb.OleDbConnection
    'Dim OBJ_DATAREADER3 As OleDb.OleDbDataReader
    'Dim OBJ_COMMAND3 As New OleDb.OleDbCommand

    'Dim OBJ_CONNECTION4 As New OleDb.OleDbConnection
    'Dim OBJ_DATAREADER4 As OleDb.OleDbDataReader
    'Dim OBJ_COMMAND4 As New OleDb.OleDbCommand

    Dim STR学校名カナ As String
    Dim STR学校名漢字 As String
    Dim STR学年名漢字 As String
    Dim STRクラス名 As String
    Dim STR振替結果 As String
    Dim STR請求年月 As String
    Dim LNG振替金額合計 As Long
    Dim STR費目名１ As String
    Dim STR費目名２ As String
    Dim STR費目名３ As String
    Dim STR費目名４ As String
    Dim STR費目名５ As String
    Dim STR費目名６ As String
    Dim STR費目名７ As String
    Dim STR費目名８ As String
    Dim STR費目名９ As String
    Dim STR費目名１０ As String
    Dim STR費目名１１ As String
    Dim STR費目名１２ As String
    Dim STR費目名１３ As String
    Dim STR費目名１４ As String
    Dim STR費目名１５ As String
    Dim STR学校コード As String
    Dim STR処理名 As String

    Private STRソート順 As String

    Private Str_GAKKOU_CODE As String
    Private Str_FURI_DATE As String
    ''2006/10/12 再振、持越しで処理済になった場合を考慮する
    ''DATAREADERを使用している時にデータ登録を行う際に使用
    'Public OBJ_CONNECTION_DREAD4 As Data.OracleClient.OracleConnection
    'Public OBJ_DATAREADER_DREAD4 As Data.OracleClient.OracleDataReader
    'Public OBJ_COMMAND_DREAD4 As Data.OracleClient.OracleCommand

    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT050", "収納状況一覧表印刷画面")
    Private Const msgTitle As String = "収納状況一覧表印刷画面(KFGPRNT050)"
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- START
    Private SFuriCode As String = String.Empty
    ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- END

#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGPRNT050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- START
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- END

            '入力ボタン制御
            btnPrnt.Enabled = True
            btnEnd.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Dim PrtCount As Integer = 0
        Try
            '印刷ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            MainDB = New MyOracle
            '印刷ボタン
            '入力値チェック
            If PFUNC_NYUURYOKU_CHECK() = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text & "00"
            LW.FuriDate = STR_FURIKAE_DATE(1)

            Dim str収納状況一覧表_NAME As String = "収納状況一覧表.rpt"
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then

                STR学校コード = Str_GAKKOU_CODE
                STR請求年月 = Mid(Str_FURI_DATE, 1, 6)

                If PSUB_GAK_SORT() = False Then
                    Return
                End If

                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                MainDB.BeginTrans()
                If PFUNC_PRTWORK_DEL() = False Then
                    MainDB.Rollback()
                    Exit Sub
                End If
                'G_PRTWORKの作成
                If PFUNC_PRTWORK_MAKE() = False Then
                    Exit Sub
                End If
                MainDB.Commit()

                '印刷前確認メッセージ
                If MessageBox.Show(String.Format(MSG0013I, "収納状況一覧表"), _
                                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                'チェックされていれば印刷 
                If chk1.Checked = True Then
                    Param = GCom.GetUserID & "," & STR学校コード & "," & STRソート順
                    nRet = ExeRepo.ExecReport("KFGP023.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                            PrtCount += 1
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "収納状況一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                    End Select
                End If

                If chk2.Checked = True Then '収納状況一覧表(費目別合計)印刷
                    Param = GCom.GetUserID & "," & STR学校コード & "," & Str_FURI_DATE & "," & STR請求年月
                    nRet = ExeRepo.ExecReport("KFGP024.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                            PrtCount += 1
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "収納状況一覧表(費目別合計)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                    End Select
                End If
            Else
                '入力学校コードがＡＬＬ９の場合
                'スケジュールマスタの検索
                '振替日、振替区分（初振）
                SQL.Append("SELECT * FROM KZFMAST.G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(Str_FURI_DATE))
                SQL.Append(" AND (FURI_KBN_S = '0' OR FURI_KBN_S='1')")
                SQL.Append(" AND FUNOU_FLG_S = '1' ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ")
                OraReader = New MyOracleReader(MainDB)
                'スケジュールが存在チェック
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("スケジュールが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                MainDB.BeginTrans()
                If PFUNC_PRTWORK_DEL() = False Then
                    MainDB.Rollback()
                    Exit Sub
                End If
                MainDB.Commit()
                STR学校コード = ""
                '印刷前確認メッセージ
                If MessageBox.Show(String.Format(MSG0013I, "収納状況一覧表"), _
                                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If
                While OraReader.EOF = False
                    If STR学校コード = OraReader.GetString("GAKKOU_CODE_S") Then
                    Else
                        STR学校コード = OraReader.GetString("GAKKOU_CODE_S")
                        If PFUNC_SCHMAST_GET() = False Then
                            GoTo next_GAKKOU
                        End If

                        STR請求年月 = OraReader.GetString("NENGETUDO_S")

                        If PSUB_GAK_SORT() = False Then
                            Return
                        End If

                        'PRTWORKの作成
                        MainDB.BeginTrans()
                        If PFUNC_PRTWORK_MAKE() = False Then
                            MainDB.Rollback()
                            Exit Sub
                        End If
                        MainDB.Commit()

                        'チェックされていれば印刷
                        If chk1.Checked = True Then
                            Param = GCom.GetUserID & "," & STR学校コード & "," & STRソート順
                            nRet = ExeRepo.ExecReport("KFGP023.EXE", Param)
                            '戻り値に対応したメッセージを表示する
                            Select Case nRet
                                Case 0
                                    PrtCount += 1
                                Case Else
                                    '印刷失敗メッセージ
                                    MessageBox.Show(String.Format(MSG0004E, "収納状況一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Return
                            End Select
                        End If

                        If chk2.Checked = True AndAlso False Then   '一応、印刷できないようにしておく
                            Param = GCom.GetUserID & "," & STR学校コード & "," & Str_FURI_DATE & "," & STR請求年月
                            nRet = ExeRepo.ExecReport("KFGP024.EXE", Param)
                            '戻り値に対応したメッセージを表示する
                            Select Case nRet
                                Case 0
                                    PrtCount += 1
                                Case Else
                                    '印刷失敗メッセージ
                                    MessageBox.Show(String.Format(MSG0004E, "収納状況一覧表(費目別合計)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Return
                            End Select
                        End If
                    End If
NEXT_GAKKOU:
                    OraReader.NextRead()
                End While
            End If

            'レコード選択条件設定

            '収納状況一覧表出力
            If PrtCount >= 1 Then
                MessageBox.Show(String.Format(MSG0014I, "収納状況一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                '印刷対象なしメッセージ
                MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            txtGAKKOU_CODE.Focus()
        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_PRTWORK_MAKE() As Boolean

        PFUNC_PRTWORK_MAKE = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            '生徒マスタビュの検索　キーは学校コード、請求月（PRTWORKの創生）
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            '結果帳票なので、生徒マスタビューからの取得ではなく、学校明細マスタから金額を取得する
            SQL.Length = 0
            SQL.Append("select ")
            SQL.Append(" SEITOMASTVIEW.*")
            SQL.Append(",G_MEIMAST.HIMOKU_ID_M")
            SQL.Append(",G_MEIMAST.SEIKYU_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU1_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU2_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU3_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU4_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU5_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU6_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU7_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU8_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU9_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU10_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU11_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU12_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU13_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU14_KIN_M")
            SQL.Append(",G_MEIMAST.HIMOKU15_KIN_M")
            SQL.Append(" from SEITOMASTVIEW, G_MEIMAST")
            SQL.Append(" where GAKKOU_CODE_O = " & SQ(STR学校コード))
            SQL.Append(" and TUKI_NO_O = " & SQ(Mid(STR請求年月, 5, 2)))

            ' V01L01 2020/06/16 CHG FJH)AMANO PKG_2020_0012_000 -------------------------------- START
            ' 請求金額が0円の生徒が出力されないので、出力されるように修正
            ' 請求金額が0円の場合、学校明細マスタが作成されないため外部結合に変更する
            SQL.Append(" and GAKKOU_CODE_O = GAKKOU_CODE_M(+)")
            SQL.Append(" and NENDO_O = NENDO_M(+)")
            SQL.Append(" and TUUBAN_O = TUUBAN_M(+)")
            SQL.Append(" and TUKI_NO_O = substr(SEIKYU_TUKI_M(+), 5, 2)")
            SQL.Append(" and FURI_DATE_M(+) = " & SQ(Str_FURI_DATE))
            SQL.Append(" and FURI_KBN_M(+) IN ('0','1')")
            'SQL.Append(" and GAKKOU_CODE_O = GAKKOU_CODE_M")
            'SQL.Append(" and NENDO_O = NENDO_M")
            'SQL.Append(" and TUUBAN_O = TUUBAN_M")
            'SQL.Append(" and TUKI_NO_O = substr(SEIKYU_TUKI_M, 5, 2)")
            'SQL.Append(" and FURI_DATE_M = " & SQ(Str_FURI_DATE))
            ''2017/05/15 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
            ''随時に同じ振替日が存在した場合に一意制約違反が発生する
            'SQL.Append(" and FURI_KBN_M IN ('0','1')")
            ''2017/05/15 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
            ' V01L01 2020/06/16 CHG FJH)AMANO PKG_2020_0012_000 -------------------------------- END

            SQL.Append(" order by GAKKOU_CODE_O, GAKUNEN_CODE_O, CLASS_CODE_O, TUUBAN_O")

            'SQL.Append(" SELECT * FROM SEITOMASTVIEW")
            'SQL.Append(" WHERE GAKKOU_CODE_O = " & SQ(STR学校コード))
            'SQL.Append(" AND TUKI_NO_O = " & SQ(Mid(STR請求年月, 5, 2)))
            'SQL.Append(" ORDER BY GAKKOU_CODE_O, GAKUNEN_CODE_O, CLASS_CODE_O")
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("ワークマスタの作成に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            While OraReader.EOF = False
                '振替金額合計計算
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                '明細マスタから取得
                LNG振替金額合計 = OraReader.GetInt64("SEIKYU_KIN_M")

                'LNG振替金額合計 = OraReader.GetInt64("KINGAKU01_O") _
                '                + OraReader.GetInt64("KINGAKU02_O") _
                '                + OraReader.GetInt64("KINGAKU03_O") _
                '                + OraReader.GetInt64("KINGAKU04_O") _
                '                + OraReader.GetInt64("KINGAKU05_O") _
                '                + OraReader.GetInt64("KINGAKU06_O") _
                '                + OraReader.GetInt64("KINGAKU07_O") _
                '                + OraReader.GetInt64("KINGAKU08_O") _
                '                + OraReader.GetInt64("KINGAKU09_O") _
                '                + OraReader.GetInt64("KINGAKU10_O") _
                '                + OraReader.GetInt64("KINGAKU11_O") _
                '                + OraReader.GetInt64("KINGAKU12_O") _
                '                + OraReader.GetInt64("KINGAKU13_O") _
                '                + OraReader.GetInt64("KINGAKU14_O") _
                '                + OraReader.GetInt64("KINGAKU15_O")
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                '生徒マスタビュを基準にＰＲＴＷＯＲＫを作成
                If PFUNC_PRTWORK_SEITOINS(OraReader) = False Then
                    Exit Function
                End If
                OraReader.NextRead()
            End While

            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            'きちんとクローズ
            OraReader.Close()
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

            'G_PRTWORKの読込み（学校マスタからの学校名等の取込み）
            SQL = New StringBuilder
            SQL.Append("SELECT * FROM G_PRTWORK")
            SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(STR学校コード))

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            While OraReader.EOF = False
                'ＰＲＴＷＯＲＫを作成
                If PFUNC_PRTWORK_GAKMASTINS(OraReader) = False Then
                    Exit Function
                End If
                OraReader.NextRead()
            End While

            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            'きちんとクローズ
            OraReader.Close()
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

            'G_PRTWORKの読込み（明細マスタからの振替結果の取込み）
            SQL = New StringBuilder
            SQL.Append("SELECT * FROM G_PRTWORK")
            SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(STR学校コード))

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            While OraReader.EOF = False
                'ＰＲＴＷＯＲＫの更新
                If PFUNC_PRTWORK_MEISAIINS(OraReader) = False Then
                    Exit Function
                End If
                OraReader.NextRead()
            End While

            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            'きちんとクローズ
            OraReader.Close()
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            '生徒マスタビューから既に取得しているため不要
            ''G_PRTWORKの読込み（費目マスタからの費目名の取込み）
            'SQL = New StringBuilder
            'SQL.Append("SELECT * FROM G_PRTWORK")
            'SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(STR学校コード))

            'If OraReader.DataReader(SQL) = False Then
            '    Exit Function
            'End If

            'While OraReader.EOF = False
            '    'ＰＲＴＷＯＲＫの更新
            '    If PFUNC_PRTWORK_HIMOMASTINS(OraReader) = False Then
            '        Exit Function
            '    End If
            '    OraReader.NextRead()
            'End While
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール取得)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_PRTWORK_MAKE = True

    End Function
    Private Function PFUNC_GAKNAME_GET2(ByVal WorkReader As MyOracleReader) As Boolean

        '学校名の設定
        PFUNC_GAKNAME_GET2 = False

        STR学校名カナ = ""
        STR学校名漢字 = ""
        STR学年名漢字 = ""
        STRクラス名 = ""

        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM GAKMAST1")
            SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" AND GAKUNEN_CODE_G  = " & WorkReader.GetString("GAKUNEN_CODE_P"))

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            While OraReader.EOF = False
                With OraReader
                    STR学校名カナ = .GetString("GAKKOU_KNAME_G")
                    STR学校名漢字 = .GetString("GAKKOU_NNAME_G")
                    STR学年名漢字 = .GetString("GAKUNEN_NAME_G")
                    'クラスコードからクラス名取得
                    For No As Integer = 1 To 20
                        If WorkReader.GetString("CLASS_CODE_P") = .GetString("CLASS_CODE1" & No.ToString("00") & "_G") Then
                            STRクラス名 = .GetString("CLASS_NAME1" & No.ToString("00") & "_G")
                            Exit For
                        End If
                    Next
                End With
                OraReader.NextRead()
            End While
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ワークマスタ行挿入)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
        PFUNC_GAKNAME_GET2 = True
    End Function
    Private Function PFUNC_PRTWORK_MEISAIINS(ByVal WorkReader As MyOracleReader) As Boolean

        PFUNC_PRTWORK_MEISAIINS = False

        '振替結果ををPRTWORKに設定する
        '明細マスタ検索
        '        Call PFUNC_MEISAI_GET()

        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)

            SQL.Append("SELECT * FROM KZFMAST.G_MEIMAST WHERE")
            SQL.Append(" GAKKOU_CODE_M = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" AND NENDO_M = " & SQ(WorkReader.GetString("NENDO_P")))
            SQL.Append(" AND FURI_DATE_M <= " & SQ(WorkReader.GetString("FURI_DATE_P")))
            SQL.Append(" AND TUUBAN_M = " & WorkReader.GetInt("TUUBAN_P").ToString)

            ' V01L01 2020/06/16 DEL FJH)AMANO PKG_2020_0012_000 -------------------------------- START
            ' 費目IDの変更考慮不足のため削除
            'SQL.Append(" AND HIMOKU_ID_M = " & SQ(WorkReader.GetString("HIMOKU_ID_P")))
            ' V01L01 2020/06/16 DEL FJH)AMANO PKG_2020_0012_000 -------------------------------- END

            ' V01L01 2020/06/16 ADD FJH)AMANO PKG_2020_0012_000 -------------------------------- START
            ' 前年度分の学校明細マスタがある場合に、前年度分の振替結果が出力される時があるので
            ' 対象年度のスケジュールで絞るように修正
            ' また、振替区分の考慮が存在しないため修正（随時入金、随時出金の更新をしてしまう場合あり）
            Select Case Mid(STR請求年月, 5, 2)
                Case "01", "02", "03"
                    SQL.Append(" AND SEIKYU_TUKI_M  BETWEEN '" & (CInt(Mid(STR請求年月, 1, 4)) - 1).ToString & "04' AND '" & Mid(STR請求年月, 1, 4) & "03'")
                Case Else
                    SQL.Append(" AND SEIKYU_TUKI_M  BETWEEN '" & Mid(STR請求年月, 1, 4) & "04' AND '" & (CInt(Mid(STR請求年月, 1, 4)) + 1).ToString & "03'")
            End Select
            SQL.Append(" AND FURI_KBN_M IN ('0','1')")
            ' V01L01 2020/06/16 ADD FJH)AMANO PKG_2020_0012_000 -------------------------------- END

            If OraReader.DataReader(SQL) = False Then
                Return True
            End If

            While OraReader.EOF = False
                With OraReader

                    STR振替結果 = .GetInt64("FURIKETU_CODE_M")
                    '2006/10/12 再振もしくは持越しで処理済になったか判定くを追加
                    ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- START
                    'If STR振替結果 <> "0" Then
                    '    Dim DataReader As MyOracleReader = New MyOracleReader(MainDB)
                    '    SQL = New StringBuilder
                    '    SQL.Append("SELECT * FROM KZFMAST.G_MEIMAST")
                    '    SQL.Append(" WHERE GAKKOU_CODE_M = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
                    '    SQL.Append(" AND NENDO_M = " & SQ(WorkReader.GetString("NENDO_P")))
                    '    SQL.Append(" AND TUUBAN_M = " & WorkReader.GetInt("TUUBAN_P").ToString)
                    '    SQL.Append(" AND HIMOKU_ID_M = " & SQ(WorkReader.GetString("HIMOKU_ID_P")))
                    '    SQL.Append(" AND SEIKYU_TUKI_M = " & SQ(OraReader.GetString("SEIKYU_TUKI_M")))
                    '    SQL.Append(" AND FURIKETU_CODE_M = '0' ")

                    '    If DataReader.DataReader(SQL) = False Then
                    '    Else
                    '        STR振替結果 = DataReader.GetInt("FURIKETU_CODE_M")
                    '    End If
                    '    DataReader.Close()
                    'End If
                    If SFuriCode.IndexOf(STR振替結果) >= 0 Then
                        Dim DataReader As MyOracleReader = New MyOracleReader(MainDB)
                        SQL = New StringBuilder
                        SQL.Append("SELECT * FROM KZFMAST.G_MEIMAST")
                        SQL.Append(" WHERE GAKKOU_CODE_M = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
                        SQL.Append(" AND NENDO_M = " & SQ(WorkReader.GetString("NENDO_P")))
                        SQL.Append(" AND TUUBAN_M = " & WorkReader.GetInt("TUUBAN_P").ToString)

                        ' V01L01 2020/06/16 DEL FJH)AMANO PKG_2020_0012_000 -------------------------------- START
                        ' 費目IDの変更考慮不足のため削除
                        'SQL.Append(" AND HIMOKU_ID_M = " & SQ(WorkReader.GetString("HIMOKU_ID_P")))
                        ' V01L01 2020/06/16 DEL FJH)AMANO PKG_2020_0012_000 -------------------------------- END

                        ' V01L01 2020/06/16 ADD FJH)AMANO PKG_2020_0012_000 -------------------------------- START
                        ' 持越処理時に正常となった場合に、過去分で印刷した際に未来の持越正常分で上書きされてしまうので、
                        ' 持越分が抽出されないように、抽出条件に画面入力した振替日を追加する
                        ' また、振替区分の考慮が存在しないため修正（随時入金、随時出金の更新をしてしまう場合あり）
                        SQL.Append(" AND FURI_DATE_M <= " & SQ(Str_FURI_DATE))
                        SQL.Append(" AND FURI_KBN_M IN ('0','1')")
                        ' V01L01 2020/06/16 ADD FJH)AMANO PKG_2020_0012_000 -------------------------------- END

                        SQL.Append(" AND SEIKYU_TUKI_M = " & SQ(OraReader.GetString("SEIKYU_TUKI_M")))
                        SQL.Append(" AND FURIKETU_CODE_M = '0' ")

                        If DataReader.DataReader(SQL) = False Then
                        Else
                            STR振替結果 = DataReader.GetInt("FURIKETU_CODE_M")
                        End If
                        DataReader.Close()
                    End If
                    ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- END

                    SQL = New StringBuilder
                    SQL.Append("UPDATE  KZFMAST.G_PRTWORK SET ")
                    '  '振替結果（請求月）
                    SQL.Append(" KEKKA_MM" & Mid(.GetString("SEIKYU_TUKI_M"), 5, 2) & "_P = " & SQ(STR振替結果))
                    SQL.Append((" WHERE GAKKOU_CODE_P  = " & SQ(WorkReader.GetString("GAKKOU_CODE_P"))))
                    SQL.Append((" And NENDO_P = " & SQ(WorkReader.GetString("NENDO_P"))))
                    SQL.Append((" And TUUBAN_P = " & WorkReader.GetString("TUUBAN_P")))

                    MainDB.ExecuteNonQuery(SQL)
                End With
                OraReader.NextRead()
            End While

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ワークマスタ行挿入)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        PFUNC_PRTWORK_MEISAIINS = True

    End Function
    Private Function PFUNC_MEISAI_GET(ByVal WorkReader As MyOracleReader) As Boolean

        '振替結果の取得
        PFUNC_MEISAI_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            STR振替結果 = ""

            SQL.Append("Select * FROM KZFMAST.G_MEIMAST")
            SQL.Append(" WHERE GAKKOU_CODE_M = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" And NENDO_M = " & SQ(WorkReader.GetString("NENDO_P")))
            SQL.Append(" And FURI_DATE_M <= " & SQ(WorkReader.GetString("FURI_DATE_P")))
            SQL.Append(" And TUUBAN_M = " & WorkReader.GetInt("TUUBAN_P").ToString)
            '        SQL.Append(" And SEIKYU_TUKI_M = " & SQ(STR請求年月))
            SQL.Append(" And HIMOKU_ID_M = " & SQ(WorkReader.GetString("HIMOKU_ID_P")))

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            While OraReader.EOF = False
                STR振替結果 = OraReader.GetInt("FURIKETU_CODE_M").ToString
                OraReader.NextRead()
            End While

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振替結果取得)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
        PFUNC_MEISAI_GET = True

    End Function
    Private Function PFUNC_PRTWORK_HIMOMASTINS(ByVal WorkReader As MyOracleReader) As Boolean

        '費目名の設定
        PFUNC_PRTWORK_HIMOMASTINS = False
        Dim SQL As New StringBuilder
        Try
            '費目マスタの検索
            Call PFUNC_HIMOMAST_GET(WorkReader)

            SQL.Append("UPDATE  KZFMAST.G_PRTWORK Set ")
            '費目名１
            SQL.Append(" HIMOKU_NAME01_P = " & SQ(STR費目名１))
            '費目名２
            SQL.Append(",HIMOKU_NAME02_P = " & SQ(STR費目名２))
            '費目名３
            SQL.Append(",HIMOKU_NAME03_P = " & SQ(STR費目名３))
            '費目名４
            SQL.Append(",HIMOKU_NAME04_P = " & SQ(STR費目名４))
            '費目名５
            SQL.Append(",HIMOKU_NAME05_P = " & SQ(STR費目名５))
            '費目名６
            SQL.Append(",HIMOKU_NAME06_P = " & SQ(STR費目名６))
            '費目名７
            SQL.Append(",HIMOKU_NAME07_P = " & SQ(STR費目名７))
            '費目名８
            SQL.Append(",HIMOKU_NAME08_P = " & SQ(STR費目名８))
            '費目名９
            SQL.Append(",HIMOKU_NAME09_P = " & SQ(STR費目名９))
            '費目名１０
            SQL.Append(",HIMOKU_NAME10_P = " & SQ(STR費目名１０))
            '費目名１１
            SQL.Append(",HIMOKU_NAME11_P = " & SQ(STR費目名１１))
            '費目名１２
            SQL.Append(",HIMOKU_NAME12_P = " & SQ(STR費目名１２))
            '費目名１３
            SQL.Append(",HIMOKU_NAME13_P = " & SQ(STR費目名１３))
            '費目名１４
            SQL.Append(",HIMOKU_NAME14_P = " & SQ(STR費目名１４))
            '費目名１５
            SQL.Append(",HIMOKU_NAME15_P = " & SQ(STR費目名１５))

            SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" And NENDO_P = " & SQ(WorkReader.GetString("NENDO_P")))
            SQL.Append(" And TUUBAN_P = " & WorkReader.GetInt("TUUBAN_P").ToString)

            MainDB.ExecuteNonQuery(SQL)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振替結果取得)", "失敗", ex.ToString)
            Return False
        End Try

        PFUNC_PRTWORK_HIMOMASTINS = True

    End Function
    Private Function PFUNC_HIMOMAST_GET(ByVal WorkReader As MyOracleReader) As Boolean

        '費目名の取得
        PFUNC_HIMOMAST_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            '費目名の初期化
            STR費目名１ = ""
            STR費目名２ = ""
            STR費目名３ = ""
            STR費目名４ = ""
            STR費目名５ = ""
            STR費目名６ = ""
            STR費目名７ = ""
            STR費目名８ = ""
            STR費目名９ = ""
            STR費目名１０ = ""
            STR費目名１１ = ""
            STR費目名１２ = ""
            STR費目名１３ = ""
            STR費目名１４ = ""
            STR費目名１５ = ""

            SQL.Append("Select * FROM KZFMAST.HIMOMAST")
            SQL.Append(" WHERE GAKKOU_CODE_H = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" And GAKUNEN_CODE_H = " & WorkReader.GetString("GAKUNEN_CODE_P"))
            SQL.Append(" And HIMOKU_ID_H = " & SQ(WorkReader.GetString("HIMOKU_ID_P")))
            SQL.Append(" And TUKI_NO_H = " & SQ(Mid(STR請求年月, 5, 2)))

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            While OraReader.EOF = False
                STR費目名１ = OraReader.GetString("HIMOKU_NAME01_H")
                STR費目名２ = OraReader.GetString("HIMOKU_NAME02_H")
                STR費目名３ = OraReader.GetString("HIMOKU_NAME03_H")
                STR費目名４ = OraReader.GetString("HIMOKU_NAME04_H")
                STR費目名５ = OraReader.GetString("HIMOKU_NAME05_H")
                STR費目名６ = OraReader.GetString("HIMOKU_NAME06_H")
                STR費目名７ = OraReader.GetString("HIMOKU_NAME07_H")
                STR費目名８ = OraReader.GetString("HIMOKU_NAME08_H")
                STR費目名９ = OraReader.GetString("HIMOKU_NAME09_H")
                STR費目名１０ = OraReader.GetString("HIMOKU_NAME10_H")
                STR費目名１１ = OraReader.GetString("HIMOKU_NAME11_H")
                STR費目名１２ = OraReader.GetString("HIMOKU_NAME12_H")
                STR費目名１３ = OraReader.GetString("HIMOKU_NAME13_H")
                STR費目名１４ = OraReader.GetString("HIMOKU_NAME14_H")
                STR費目名１５ = OraReader.GetString("HIMOKU_NAME15_H")
                OraReader.NextRead()
            End While

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(振替結果取得)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        PFUNC_HIMOMAST_GET = True

    End Function
    Private Function PSUB_GAK_SORT() As Boolean
        PSUB_GAK_SORT = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" Select MEISAI_OUT_T FROM GAKMAST2")
            SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(STR学校コード))

            If OraReader.DataReader(SQL) = False Then
                STRソート順 = "0"
            Else
                STRソート順 = OraReader.GetString("MEISAI_OUT_T")
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ソート順取得)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        Return True
    End Function
    Private Function PFUNC_PRTWORK_SEITOINS(ByVal OraReader As MyOracleReader) As Boolean

        PFUNC_PRTWORK_SEITOINS = False
        Dim SQL As New StringBuilder
        Try
            With OraReader
                SQL.Append("INSERT INTO KZFMAST.G_PRTWORK ")
                SQL.Append(" (GAKKOU_CODE_P ")
                SQL.Append(", NENDO_P ")
                SQL.Append(", TUUBAN_P ")
                SQL.Append(", FURI_DATE_P ")
                SQL.Append(", GAKKOU_KNAME_P ")
                SQL.Append(", GAKKOU_NNAME_P ")
                SQL.Append(", SEIBETU_P ")
                SQL.Append(", GAKUNEN_CODE_P ")
                SQL.Append(", GAKUNEN_NAME_P ")
                SQL.Append(", CLASS_CODE_P ")
                SQL.Append(", CLASS_NAME_P ")
                SQL.Append(", SEITO_NO_P ")
                SQL.Append(", SEITO_KNAME_P ")
                SQL.Append(", SEITO_NNAME_P ")
                SQL.Append(", TKIN_NO_P ")
                SQL.Append(", TSIT_NO_P ")
                SQL.Append(", KAMOKU_P ")
                SQL.Append(", KOUZA_P ")
                SQL.Append(", MEIGI_KNAME_P ")
                SQL.Append(", HIMOKU_ID_P ")
                SQL.Append(", KINGAKU_P ")
                SQL.Append(", KINGAKU01_P ")
                SQL.Append(", KINGAKU02_P ")
                SQL.Append(", KINGAKU03_P ")
                SQL.Append(", KINGAKU04_P ")
                SQL.Append(", KINGAKU05_P ")
                SQL.Append(", KINGAKU06_P ")
                SQL.Append(", KINGAKU07_P ")
                SQL.Append(", KINGAKU08_P ")
                SQL.Append(", KINGAKU09_P ")
                SQL.Append(", KINGAKU10_P ")
                SQL.Append(", KINGAKU11_P ")
                SQL.Append(", KINGAKU12_P ")
                SQL.Append(", KINGAKU13_P ")
                SQL.Append(", KINGAKU14_P ")
                SQL.Append(", KINGAKU15_P ")
                SQL.Append(", FUNOU_RIYUU_P ")
                SQL.Append(", KEKKA_MM04_P ")
                SQL.Append(", KEKKA_MM05_P ")
                SQL.Append(", KEKKA_MM06_P ")
                SQL.Append(", KEKKA_MM07_P ")
                SQL.Append(", KEKKA_MM08_P ")
                SQL.Append(", KEKKA_MM09_P ")
                SQL.Append(", KEKKA_MM10_P ")
                SQL.Append(", KEKKA_MM11_P ")
                SQL.Append(", KEKKA_MM12_P ")
                SQL.Append(", KEKKA_MM01_P ")
                SQL.Append(", KEKKA_MM02_P ")
                SQL.Append(", KEKKA_MM03_P ")
                SQL.Append(", HIMOKU_NAME01_P ")
                SQL.Append(", HIMOKU_NAME02_P ")
                SQL.Append(", HIMOKU_NAME03_P ")
                SQL.Append(", HIMOKU_NAME04_P ")
                SQL.Append(", HIMOKU_NAME05_P ")
                SQL.Append(", HIMOKU_NAME06_P ")
                SQL.Append(", HIMOKU_NAME07_P ")
                SQL.Append(", HIMOKU_NAME08_P ")
                SQL.Append(", HIMOKU_NAME09_P ")
                SQL.Append(", HIMOKU_NAME10_P ")
                SQL.Append(", HIMOKU_NAME11_P ")
                SQL.Append(", HIMOKU_NAME12_P ")
                SQL.Append(", HIMOKU_NAME13_P ")
                SQL.Append(", HIMOKU_NAME14_P ")
                SQL.Append(", HIMOKU_NAME15_P )")
                SQL.Append(" VALUES ( ")
                '学校コード
                SQL.Append(SQ(.GetString("GAKKOU_CODE_O")))
                '入学年度
                SQL.Append("," & SQ(.GetString("NENDO_O")))
                '通番
                SQL.Append("," & GCom.NzInt(.GetString("TUUBAN_O")))
                '振替日
                SQL.Append("," & SQ(Str_FURI_DATE))
                '学校名（カナ）
                SQL.Append("," & SQ(Space(40)))
                '学校名（漢字）
                SQL.Append("," & SQ(Space(50)))
                '性別
                SQL.Append("," & SQ(.GetString("SEIBETU_O")))
                '学年コード
                SQL.Append("," & GCom.NzInt(.GetString("GAKUNEN_CODE_O")))
                '学年名称
                SQL.Append("," & SQ(Space(20)))
                'クラスコード
                SQL.Append("," & GCom.NzInt(.GetString("CLASS_CODE_O")))
                'クラス名
                SQL.Append("," & SQ(Space(20)))
                '生徒番号
                SQL.Append("," & SQ(.GetString("SEITO_NO_O")))
                '生徒名（カナ）
                SQL.Append("," & SQ(.GetString("SEITO_KNAME_O")))
                '生徒名（漢字）
                SQL.Append("," & SQ(.GetString("SEITO_NNAME_O")))
                '取扱金融機関
                SQL.Append("," & SQ(.GetString("TKIN_NO_O")))
                '取扱支店コード
                SQL.Append("," & SQ(.GetString("TSIT_NO_O")))
                '科目
                SQL.Append("," & SQ(.GetString("KAMOKU_O")))
                '口座番号
                SQL.Append("," & SQ(.GetString("KOUZA_O")))
                '口座名義人カナ
                SQL.Append("," & SQ(.GetString("MEIGI_KNAME_O")))
                '費目ＩＤ
                SQL.Append("," & SQ(.GetString("HIMOKU_ID_O")))
                '振替金額（合計）
                SQL.Append("," & LNG振替金額合計)
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                '結果帳票なので、生徒マスタビューからの取得ではなく、学校明細マスタから金額を取得する
                '振替金額（費目１）
                SQL.Append("," & .GetInt64("HIMOKU1_KIN_M"))
                '振替金額（費目２）
                SQL.Append("," & .GetInt64("HIMOKU2_KIN_M"))
                '振替金額（費目３）
                SQL.Append("," & .GetInt64("HIMOKU3_KIN_M"))
                '振替金額（費目４）
                SQL.Append("," & .GetInt64("HIMOKU4_KIN_M"))
                '振替金額（費目５）
                SQL.Append("," & .GetInt64("HIMOKU5_KIN_M"))
                '振替金額（費目６）
                SQL.Append("," & .GetInt64("HIMOKU6_KIN_M"))
                '振替金額（費目７）
                SQL.Append("," & .GetInt64("HIMOKU7_KIN_M"))
                '振替金額（費目８）
                SQL.Append("," & .GetInt64("HIMOKU8_KIN_M"))
                '振替金額（費目９）
                SQL.Append("," & .GetInt64("HIMOKU9_KIN_M"))
                '振替金額（費目１０）
                SQL.Append("," & .GetInt64("HIMOKU10_KIN_M"))
                '振替金額（費目１１）
                SQL.Append("," & .GetInt64("HIMOKU11_KIN_M"))
                '振替金額（費目１２）
                SQL.Append("," & .GetInt64("HIMOKU12_KIN_M"))
                '振替金額（費目１３）
                SQL.Append("," & .GetInt64("HIMOKU13_KIN_M"))
                '振替金額（費目１４）
                SQL.Append("," & .GetInt64("HIMOKU14_KIN_M"))
                '振替金額（費目１５）
                SQL.Append("," & .GetInt64("HIMOKU15_KIN_M"))

                ''振替金額（費目１）
                'SQL.Append("," & .GetInt64("KINGAKU01_O"))
                ''振替金額（費目２）
                'SQL.Append("," & .GetInt64("KINGAKU02_O"))
                ''振替金額（費目３）
                'SQL.Append("," & .GetInt64("KINGAKU03_O"))
                ''振替金額（費目４）
                'SQL.Append("," & .GetInt64("KINGAKU04_O"))
                ''振替金額（費目５）
                'SQL.Append("," & .GetInt64("KINGAKU05_O"))
                ''振替金額（費目６）
                'SQL.Append("," & .GetInt64("KINGAKU06_O"))
                ''振替金額（費目７）
                'SQL.Append("," & .GetInt64("KINGAKU07_O"))
                ''振替金額（費目８）
                'SQL.Append("," & .GetInt64("KINGAKU08_O"))
                ''振替金額（費目９）
                'SQL.Append("," & .GetInt64("KINGAKU09_O"))
                ''振替金額（費目１０）
                'SQL.Append("," & .GetInt64("KINGAKU10_O"))
                ''振替金額（費目１１）
                'SQL.Append("," & .GetInt64("KINGAKU11_O"))
                ''振替金額（費目１２）
                'SQL.Append("," & .GetInt64("KINGAKU12_O"))
                ''振替金額（費目１３）
                'SQL.Append("," & .GetInt64("KINGAKU13_O"))
                ''振替金額（費目１４）
                'SQL.Append("," & .GetInt64("KINGAKU14_O"))
                ''振替金額（費目１５）
                'SQL.Append("," & .GetInt64("KINGAKU15_O"))
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                '不能理由 
                SQL.Append("," & SQ(Space(10)))
                '振替結果（４月）
                SQL.Append("," & SQ(Space(1)))
                '振替結果（５月）
                SQL.Append("," & SQ(Space(1)))
                '振替結果（６月）
                SQL.Append("," & SQ(Space(1)))
                '振替結果（７月）
                SQL.Append("," & SQ(Space(1)))
                '振替結果（８月）
                SQL.Append("," & SQ(Space(1)))
                '振替結果（９月）
                SQL.Append("," & SQ(Space(1)))
                '振替結果（１０月）
                SQL.Append("," & SQ(Space(1)))
                '振替結果（１１月）
                SQL.Append("," & SQ(Space(1)))
                '振替結果（１２月）
                SQL.Append("," & SQ(Space(1)))
                '振替結果（１月）
                SQL.Append("," & SQ(Space(1)))
                '振替結果（２月）
                SQL.Append("," & SQ(Space(1)))
                '振替結果（３月）
                SQL.Append("," & SQ(Space(1)))

                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                '生徒マスタビューに既に持っているため、設定してしまう。
                '費目名１
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME01_O")))
                '費目名２
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME02_O")))
                '費目名３
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME03_O")))
                '費目名４
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME04_O")))
                '費目名５
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME05_O")))
                '費目名６
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME06_O")))
                '費目名７
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME07_O")))
                '費目名８
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME08_O")))
                '費目名９
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME09_O")))
                '費目名１０
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME10_O")))
                '費目名１１
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME11_O")))
                '費目名１２
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME12_O")))
                '費目名１３
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME13_O")))
                '費目名１４
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME14_O")))
                '費目名１５
                SQL.Append("," & SQ(.GetString("HIMOKU_NAME15_O")) & ")")

                ''費目名１
                'SQL.Append("," & SQ(Space(20)))
                ''費目名２
                'SQL.Append("," & SQ(Space(20)))
                ''費目名３
                'SQL.Append("," & SQ(Space(20)))
                ''費目名４
                'SQL.Append("," & SQ(Space(20)))
                ''費目名５
                'SQL.Append("," & SQ(Space(20)))
                ''費目名６
                'SQL.Append("," & SQ(Space(20)))
                ''費目名７
                'SQL.Append("," & SQ(Space(20)))
                ''費目名８
                'SQL.Append("," & SQ(Space(20)))
                ''費目名９
                'SQL.Append("," & SQ(Space(20)))
                ''費目名１０
                'SQL.Append("," & SQ(Space(20)))
                ''費目名１１
                'SQL.Append("," & SQ(Space(20)))
                ''費目名１２
                'SQL.Append("," & SQ(Space(20)))
                ''費目名１３
                'SQL.Append("," & SQ(Space(20)))
                ''費目名１４
                'SQL.Append("," & SQ(Space(20)))
                ''費目名１５
                'SQL.Append("," & SQ(Space(20)) & ")")
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
            End With

            MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ワークマスタ行挿入)", "失敗", ex.ToString)
            Return False
        End Try

        PFUNC_PRTWORK_SEITOINS = True

    End Function
    Private Function PFUNC_PRTWORK_GAKMASTINS(ByVal WorkReader As MyOracleReader) As Boolean
        '学校名、学年名、クラス名をPRTWORKに設定する

        PFUNC_PRTWORK_GAKMASTINS = False
        Dim SQL As New StringBuilder
        Dim Ret As Integer
        Try
            '学校マスタ１検索
            Call PFUNC_GAKNAME_GET2(WorkReader)

            SQL.Append("UPDATE  KZFMAST.G_PRTWORK Set ")
            '学校名（カナ）
            SQL.Append(" GAKKOU_KNAME_P = " & SQ(STR学校名カナ))
            '学校名（漢字）
            SQL.Append(",GAKKOU_NNAME_P = " & SQ(STR学校名漢字))
            '学年名称
            SQL.Append(",GAKUNEN_NAME_P = " & SQ(STR学年名漢字))
            'クラス名
            SQL.Append(",CLASS_NAME_P = " & SQ(STRクラス名))

            SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(WorkReader.GetString("GAKKOU_CODE_P")))
            SQL.Append(" And NENDO_P = " & SQ(WorkReader.GetString("NENDO_P")))
            SQL.Append(" And TUUBAN_P = " & WorkReader.GetString("TUUBAN_P"))

            Ret = MainDB.ExecuteNonQuery(SQL)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ワークマスタ学校更新)", "失敗", ex.ToString)
            Return False
        End Try

        PFUNC_PRTWORK_GAKMASTINS = True

    End Function
    Private Function PFUNC_PRTWORK_DEL() As Boolean
        Try
            PFUNC_PRTWORK_DEL = False

            Dim SQL As New StringBuilder
            SQL.Append("DELETE FROM G_PRTWORK")

            MainDB.ExecuteNonQuery(SQL)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ワークマスタ削除)", "失敗", ex.ToString)
            Return False
        End Try
        PFUNC_PRTWORK_DEL = True
    End Function
    Private Function PFUNC_SCHMAST_GET() As Boolean

        PFUNC_SCHMAST_GET = False

        Dim OraReader As MyOracleReader = Nothing
        Dim OraReader2 As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            'スケジュールマスタの検索
            '学校コード、振替日、振替区分（初振）

            SQL.Append(" Select * FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(STR学校コード))

            SQL.Append(" And FURI_DATE_S = " & SQ(Str_FURI_DATE))

            'SQL.Append(" And FURI_KBN_S = '0'")

            'スケジュールが存在チェック
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("スケジュールが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            OraReader2 = New MyOracleReader(MainDB)
            While OraReader.EOF = False
                STR請求年月 = OraReader.GetString("NENGETUDO_S")
                SQL = New StringBuilder
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(STR学校コード))
                SQL.Append(" AND FURI_DATE_S > " & SQ(Str_FURI_DATE))
                SQL.Append(" AND NENGETUDO_S = " & SQ(STR請求年月))
                SQL.Append(" AND CHECK_FLG_S = '1'")
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                '初振と再振の間に随時が含まれると印刷できないため条件追加
                SQL.Append(" AND FURI_KBN_S IN ('0', '1')")
                SQL.Append(" AND SCH_KBN_S <> '2'")
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                'スケジュールが存在チェック
                If OraReader2.DataReader(SQL) Then
                    If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                        MessageBox.Show("このスケジュールは再振処理に進んでいます" & vbCrLf & "過去の状況を出力することはできません", _
                                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    Exit Function
                End If
                OraReader.NextRead()
            End While
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュール取得)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraReader2 IsNot Nothing Then OraReader2.Close()
        End Try
        PFUNC_SCHMAST_GET = True

    End Function
    Private Function PFUNC_NYUURYOKU_CHECK() As Boolean

        PFUNC_NYUURYOKU_CHECK = False

        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            '学校コード必須チェック
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return False
            End If
            Str_GAKKOU_CODE = txtGAKKOU_CODE.Text.Trim
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

            Str_FURI_DATE = Trim(txtFuriDateY.Text) & Trim(txtFuriDateM.Text) & Trim(txtFuriDateD.Text)

            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                SQL = New StringBuilder
                '学校マスタ存在チェック
                SQL.Append("SELECT GAKKOU_CODE_G")
                SQL.Append(" FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(txtGAKKOU_CODE.Text))

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("入力された学校コードが存在しません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If
            End If


            'スケジュールが不能結果更新済みか判定追加
            SQL = New StringBuilder
            SQL.Append("SELECT * FROM G_SCHMAST")
            If Trim(Str_GAKKOU_CODE) = "9999999999" Then
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(Str_FURI_DATE))
                SQL.Append(" AND FUNOU_FLG_S = '1'")
            Else
                SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(Trim(Str_GAKKOU_CODE)))
                SQL.Append(" AND FURI_DATE_S = " & SQ(Str_FURI_DATE))
            End If
            STR_SQL += " AND (FURI_KBN_S = '0' OR FURI_KBN_S='1')"

            'ｽｹｼﾞｭｰﾙ区分2(随時)は処理に含まない為
            STR_SQL += " AND SCH_KBN_S <> '2'"

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("スケジュールが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            If OraReader.GetString("FUNOU_FLG_S") = "0" Then
                MessageBox.Show("このスケジュールは不能結果更新処理が未処理です", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            '印刷帳票にチェックがついていなかったらエラー
            If chk1.Checked = False AndAlso chk2.Checked = False Then
                MessageBox.Show("印刷対象帳票が指定されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_NYUURYOKU_CHECK = True

    End Function
    Private Function PFUNC_GAKNAME_GET() As Boolean
        '学校名の設定
        PFUNC_GAKNAME_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim OraDB As MyOracle = Nothing
        Try
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab学校名.Text = ""
            Else
                If OraDB Is Nothing Then OraDB = New MyOracle
                OraReader = New MyOracleReader(OraDB)
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G =" & SQ(txtGAKKOU_CODE.Text))

                '学校マスタ１存在チェック
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("学校マスタに登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lab学校名.Text = ""
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                lab学校名.Text = OraReader.GetString("GAKKOU_NNAME_G")

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraDB IsNot Nothing Then OraDB.Close()
        End Try
        PFUNC_GAKNAME_GET = True

    End Function

#End Region

#Region " イベント"
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '学校コード
        Call GCom.NzNumberString(CType(sender, TextBox), True)
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '蒲郡信金向け　ALL9指定時、「収納状況一覧表(費目別合計)」のチェックボックスは入力不可とする 2007/08/16
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                chk2.Checked = False
                chk2.Enabled = False
                lab学校名.Text = ""
            Else
                ' 2017/05/26 タスク）綾部 DEL 【ME】(RSV2対応 自動チェック機能削除) -------------------- START
                'chk2.Checked = True
                ' 2017/05/26 タスク）綾部 DEL 【ME】(RSV2対応 自動チェック機能削除) -------------------- END
                chk2.Enabled = True

                '学校名の取得
                If PFUNC_GAKNAME_GET() <> False Then
                    Exit Sub
                End If
            End If

        End If
    End Sub

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '学校カナ絞込みコンボ
        '********************************************
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校名コンボボックス設定
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = True Then
            cmbGakkouName.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        '学校検索後の学校コード設定
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '学校名の取得
        lab学校名.Text = cmbGakkouName.Text
        'PFUC_GAKNAME_GET()

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("ゼロパディング", "失敗", ex.ToString)
        End Try
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

#End Region

End Class
