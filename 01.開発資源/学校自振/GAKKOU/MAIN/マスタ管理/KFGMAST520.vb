Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Imports CAstReports

Public Class KFGMAST520

#Region " 共通変数定義 "
    Private Lng_Himoku_Ok_Cnt As Long
    Private Lng_Seito_Ok_Cnt As Long
    Private Lng_Gak_Ok_Cnt As Long
    Private Lng_Kin_Ok_Cnt As Long

    Private Lng_Himoku_Ng_Cnt As Long
    Private Lng_Seito_Ng_Cnt As Long
    Private Lng_Gak_Ng_Cnt As Long

    'Private Lng_Csv_Ok_Cnt As String = 0
    'Private Lng_Csv_Ng_Cnt As String = 0

    Private Lng_Himoku_SeqNo As Long
    Private Lng_Seito_SeqNo As Long
    Private Lng_Gak_SeqNo As Long

    Public STR帳票ソート順 As String = "0"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST520", "学校ツール連携画面")
    Private ToolLOG As New CASTCommon.BatchLOG("KFGMAST520/ERROR", "データ取込エラーログ")

    Private Const msgTitle As String = "学校ツール連携画面(KFGMAST520)"
    Private MainDB As MyOracle

    Private KobetuLogFileName As String 'エラーログ
    Private KobetuLogFileName2 As String '読込ログ

    Private err_code As Integer = 0
    Private err_Filename As String = ""

    '学校ツール作成先、取込先PATH取得
    Private GAKTOOL_IN_PATH As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "SAVE_PATH")
    Private GAKTOOL_INBK_PATH As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "CSVBK")

    Private GAKTOOL_SEITO_FILE As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "TORIKOMI_CSV_SEITO")
    Private GAKTOOL_HIMO_FILE As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "TORIKOMI_CSV_HIMOKU")
    Private GAKTOOL_GAK_FILE As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "TORIKOMI_CSV_GAKKO")

    Private GAKTOOL_LOG_PATH As String = GFUNC_INI_READ("COMMON", "LOG")

    '請求年月
    Private strSEIKYUTUKI As String
    '生徒マスタ口座名義漢字・カナ・住所・ＴＥＬ
    Private strKANJI_MEI As String
    Private strKANA_MEI As String
    Private strADDRESS As String
    Private strTELNO As String

    Private EXP_NEND As Integer
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAST520_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            'ログ用
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            With Me
                .WindowState = FormWindowState.Normal
                .FormBorderStyle = FormBorderStyle.FixedDialog
                .ControlBox = True
            End With

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region " Button Click "
    Private Sub btnImp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImp.Click
        MainDB = New MyOracle
        Dim sql As New StringBuilder(128)
        Dim aoraReader As New MyOracleReader(MainDB)
        Dim OK_ken As Long = 0
        Dim NG_ken As Long = 0

        Try
            err_code = 0
            Dim Gakkou_Code As String = 0
            LW.ToriCode = txtGAKKOU_CODE.Text
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移入)開始", "成功", "")

            Cursor.Current = Cursors.WaitCursor()
            Dim strDIR As String

            '入力値チェック
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                MessageBox.Show("移入処理は全件処理を行えません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If
            If Trim(txtGAKKOU_CODE.Text) = "" Then
                MessageBox.Show("学校コードが入力されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If

            '確認メッセージ
            If MessageBox.Show("ＣＳＶからのインポートを開始します" & vbCrLf & "費目マスタは現在の情報をすべて削除してから登録を行います", _
                                          msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            strDIR = CurDir()

            'バックアップフォルダの確認
            If Directory.Exists(GAKTOOL_INBK_PATH) = False Then
                Directory.CreateDirectory(GAKTOOL_INBK_PATH)
            End If

            '学校コードの取得
            sql.Append(" SELECT DISTINCT GAKKOU_CODE_T FROM GAKMAST1,GAKMAST2")
            sql.Append(" WHERE GAKKOU_CODE_T = GAKKOU_CODE_G")
            sql.Append(" AND GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text & "'")
            sql.Append(" ORDER BY GAKKOU_CODE_T")

            If aoraReader.DataReader(sql) = True Then

                Do Until aoraReader.EOF
                    Lng_Himoku_Ok_Cnt = 0
                    Lng_Seito_Ok_Cnt = 0

                    Lng_Himoku_Ng_Cnt = 0
                    Lng_Seito_Ng_Cnt = 0

                    Lng_Himoku_SeqNo = 1
                    Lng_Seito_SeqNo = 1

                    Gakkou_Code = aoraReader.GetString("GAKKOU_CODE_T")

                    MainDB.BeginTrans()

                    '***学校・スケジュールチェック***
                    If PFUNC_IMP_GAKKOU(Gakkou_Code, GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") = False Then
                        MessageBox.Show("処理が中断されました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainDB.Rollback()
                        ChDir(strDIR)
                        btnEnd.Focus()
                        Return
                    Else
                        'ファイルバックアップ＆削除
                        If File.Exists(GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv", GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                        End If
                    End If


                    '*** 費目マスタ ***
                    'CSV入力＆マスタ登録
                    If PFUNC_IMP_HIMOKU(Gakkou_Code, GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv", Lng_Himoku_Ok_Cnt, Lng_Himoku_Ng_Cnt) = False Then
                        MessageBox.Show("処理が中断されました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainDB.Rollback()
                        ChDir(strDIR)
                        btnEnd.Focus()

                        'ファイルバックアップ戻し（学校データ）
                        If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                        End If

                        Return
                    Else
                        'ファイルバックアップ＆削除
                        If File.Exists(GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv", GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                        End If
                    End If

                    'ログファイル削除
                    KobetuLogFileName = Path.Combine(GAKTOOL_LOG_PATH, "Err_SEITO" & Gakkou_Code & ".log")
                    If File.Exists(KobetuLogFileName) Then
                        File.Delete(KobetuLogFileName)
                    End If
                    KobetuLogFileName2 = Path.Combine(GAKTOOL_LOG_PATH, "Red_SEITO" & Gakkou_Code & ".log")
                    If File.Exists(KobetuLogFileName2) Then
                        File.Delete(KobetuLogFileName2)
                    End If

                    '*** 生徒マスタ ***
                    'CSV入力＆マスタ登録
                    If PFUNC_IMP_SEITO(Gakkou_Code, GAKTOOL_IN_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv", Lng_Seito_Ok_Cnt, Lng_Seito_Ng_Cnt) = False Then
                        MessageBox.Show("処理が中断されました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        MainDB.Rollback()
                        ChDir(strDIR)
                        btnEnd.Focus()

                        'ファイルバックアップ戻し（学校データ）
                        If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                        End If
                        'ファイルバックアップ戻し（費目データ）
                        If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                        End If

                        Return
                    Else
                        'ファイルバックアップ＆削除
                        If File.Exists(GAKTOOL_IN_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_IN_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv", GAKTOOL_INBK_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_IN_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv")
                        End If
                    End If

                    aoraReader.NextRead()
                    OK_ken += Lng_Himoku_Ok_Cnt + Lng_Seito_Ok_Cnt
                    NG_ken += Lng_Himoku_Ng_Cnt + Lng_Seito_Ng_Cnt
                Loop
            Else
                aoraReader.Close()
                MessageBox.Show("学校マスタに登録されていません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return

            End If


            If NG_ken = 0 Then     '成功

                MainDB.Commit()

                '完了メッセージ
                MessageBox.Show("インポートを終了しました。" & vbCrLf & "費目:" & Lng_Himoku_Ok_Cnt & "件のデータを登録しました。" & vbCrLf & "生徒:" & Lng_Seito_Ok_Cnt & "件のデータを登録しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else

                MainDB.Rollback()

                'ファイルバックアップ戻し（学校データ）
                If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") Then
                    FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                    File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                End If
                'ファイルバックアップ戻し（費目データ）
                If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv") Then
                    FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                    File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_HIMO_FILE & Gakkou_Code & ".csv")
                End If
                'ファイルバックアップ戻し（生徒データ）
                If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv") Then
                    FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv")
                    File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_SEITO_FILE & Gakkou_Code & ".csv")
                End If


                'エラー時メッセージ
                '2010/11/22　文言変更
                MessageBox.Show("インポートに失敗しました。" & vbCrLf & NG_ken & "件のエラーをログに出力しました。" & vbCrLf & _
                             "ログを確認してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            ChDir(strDIR)
            btnEnd.Focus()

        Catch ex As System.IO.IOException
            MessageBox.Show("ファイルが使用中です。" & vbCrLf & KobetuLogFileName, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ログファイルアクセス", "失敗", ex.Message)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移入)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移入)終了", "成功", "")
            If Not aoraReader Is Nothing Then aoraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try


    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Me.Close()

    End Sub
#End Region

#Region " Private Function "
#Region "移入"
    Private Function PFUNC_IMP_GAKKOU(ByVal gakkou_code_H As String, ByVal pCsvPath As String) As Boolean

        Dim red As System.IO.StreamReader = Nothing


        Dim strReadLine As String
        Dim strSplitValue() As String
        Dim strGET_gakkou_code As String
        Dim strGET_furi_date As String

        Dim strErrorLine As String = ""

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUNC_IMP_GAKKOU = False

        Try

            Dim dirs As String() = System.IO.Directory.GetFiles(GAKTOOL_IN_PATH)
            Dim dir As String
            Dim dirflg As String

            dirflg = "0"
            For Each dir In dirs
                If dir = pCsvPath Then
                    dirflg = "1"
                End If
            Next

            If dirflg = "0" Then
                err_code = 2
                Throw New FileNotFoundException(String.Format("ファイルが見つかりません。" _
                                        & ControlChars.Cr & "'{0}'", pCsvPath))
            End If

            red = New System.IO.StreamReader(pCsvPath, System.Text.Encoding.Default)

            Do While Not red.Peek() = -1

                strReadLine = red.ReadLine.ToString
                strSplitValue = Split(strReadLine, ",")

                strGET_gakkou_code = Trim(strSplitValue(0).PadLeft(10, "0"c))
                strGET_furi_date = Trim(strSplitValue(11).PadLeft(8, "0"c))

                '入力内容のチェック（学校コードチェック＆請求月取得）
                If gakkou_code_H = strGET_gakkou_code Then

                    'スケジュールマスタのチェック
                    sql = New StringBuilder(128)
                    oraReader = New MyOracleReader(MainDB)
                    sql.Append("SELECT NENGETUDO_S FROM G_SCHMAST ")
                    sql.Append(" WHERE")
                    sql.Append(" GAKKOU_CODE_S  = '" & gakkou_code_H & "' ")
                    sql.Append(" AND")
                    sql.Append(" FURI_KBN_S  = '0' ")
                    sql.Append(" AND")
                    sql.Append(" FURI_DATE_S = '" & strGET_furi_date & "' ")

                    If oraReader.DataReader(sql) = False Then
                        MessageBox.Show("スケジュールが存在しないので処理できません。" & vbCrLf & "データ振替日:" & strGET_furi_date, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "該当スケジュールなし データ振替日:" & strGET_furi_date, "失敗", "")
                        oraReader.Close()
                        Return False
                    Else
                        strSEIKYUTUKI = oraReader.GetItem("NENGETUDO_S")
                        oraReader.Close()
                    End If

                Else
                    MessageBox.Show("データの学校コードと一致しませんので処理できません。" & vbCrLf & "データ学校コード:" & strGET_gakkou_code, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "学校コード不一致 データ学校コード:" & strGET_gakkou_code, "失敗", "")
                    Return False
                End If
            Loop

            red.Close()
            Return True

        Catch ex As FileNotFoundException
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                MessageBox.Show("ファイルが見つかりません。" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "学校ファイルなし", "失敗", ex.Message)
            Else
                Return True
            End If
        Catch ex As System.IO.IOException
            MessageBox.Show("ファイルが使用中です。" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "学校ファイルアクセス", "失敗", ex.Message)
            err_code = 1
            err_Filename = pCsvPath
            Return False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "学校チェック例外エラー", "失敗", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function


    Private Function PFUNC_IMP_HIMOKU(ByVal gakkou_code_H As String, ByVal pCsvPath As String, ByRef lngOkCnt As Long, ByRef lngErrCnt As Long) As Boolean

        Dim red As System.IO.StreamReader = Nothing


        Dim strReadLine As String

        Dim strErrorLine As String = ""

        Dim sql As StringBuilder

        lngOkCnt = 0
        lngErrCnt = 0
        PFUNC_IMP_HIMOKU = False

        Try

            Dim dirs As String() = System.IO.Directory.GetFiles(GAKTOOL_IN_PATH)
            Dim dir As String
            Dim dirflg As String

            dirflg = "0"
            For Each dir In dirs
                If dir = pCsvPath Then
                    dirflg = "1"
                End If
            Next

            If dirflg = "0" Then
                err_code = 2
                Throw New FileNotFoundException(String.Format("ファイルが見つかりません。" _
                                        & ControlChars.Cr & "'{0}'", pCsvPath))
            End If

            red = New System.IO.StreamReader(pCsvPath, System.Text.Encoding.Default)

            'SQL文作成
            '画面に設定されている学校コードを条件に費目マスタを削除する
            sql = New StringBuilder(128)
            sql.Append(" DELETE  FROM HIMOMAST")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_H ='" & gakkou_code_H & "'")

            'トランザクションデータ操作開始
            If MainDB.ExecuteNonQuery(sql) < 0 Then
                MainLOG.Write("費目マスタ削除", "失敗", sql.ToString)
                Return False
            End If

            Do While Not red.Peek() = -1

                strReadLine = red.ReadLine.ToString

                '入力内容のチェック
                If PFUNC_CHECK_HIMOKU(gakkou_code_H, Lng_Himoku_SeqNo, strReadLine, strErrorLine) = True Then
                    'SQL文作成＆SQL処理実行
                    If PFUNC_INSERT_HIMOKU(Lng_Himoku_SeqNo, strReadLine, strErrorLine) = True Then
                        lngOkCnt += 1
                    Else
                        lngErrCnt += 1

                        'ｴﾗｰﾛｸﾞの書き出し
                        ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_INSERT_HIMOKU", "失敗", Replace(strErrorLine, ",", "/"))
                        'SQLでエラーが発生した場合は処理中断
                        red.Close()
                        Return False
                    End If
                Else
                    lngErrCnt += 1

                    'ｴﾗｰﾛｸﾞの書き出し
                    ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_CHECK_HIMOKU", "失敗", Replace(strErrorLine, ",", "/"))
                End If

                Lng_Himoku_SeqNo += 1
            Loop

            red.Close()
            Return True

        Catch ex As FileNotFoundException
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                MessageBox.Show("ファイルが見つかりません。" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "費目ファイルなし", "失敗", ex.Message)
            Else
                Return True
            End If
        Catch ex As System.IO.IOException
            MessageBox.Show("ファイルが使用中です。" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "費目ファイルアクセス", "失敗", ex.Message)
            err_code = 1
            err_Filename = pCsvPath
            Return False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "費目チェック例外エラー", "失敗", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function
    Private Function PFUNC_CHECK_HIMOKU(ByVal gakkou_code_C As String, ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intCnt As Integer
        Dim strSplitValue() As String
        Dim strRET As String = ""

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader


        PFUNC_CHECK_HIMOKU = False
        Try
            strSplitValue = Split(pLineValue, ",")

            '入力内容チェック
            If strSplitValue.Length = 76 Then
                For intCnt = 0 To UBound(strSplitValue)
                    Select Case intCnt
                        Case 0
                            '申請区分
                            Select Case Trim(strSplitValue(intCnt))
                                Case 1
                                Case 2
                                Case 9 '削除なのでスキップ
                                    PFUNC_CHECK_HIMOKU = True
                                    Exit Function
                                Case ""
                                Case Else
                                    pError = pSeqNo & ",申請区分,申請区分入力不備です"
                                    Exit Function
                            End Select

                        Case 1
                            '学校コード

                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",学校コード,未入力です"
                                Exit Function
                            End If

                            'ファイル名に記載されている学校コード以外のコードの場合はエラー
                            If Trim(gakkou_code_C) <> Trim(strSplitValue(1).PadLeft(10, "0"c)) Then
                                pError = pSeqNo & ",学校コード,ファイル名とデータの学校コードが違います"
                                Exit Function
                            End If

                            ''学校マスタチェック
                            'sql = New StringBuilder(128)
                            'oraReader = New MyOracleReader(MainDB)
                            'sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                            'sql.Append(" WHERE GAKKOU_CODE_G = '" & strSplitValue(intCnt).PadLeft(10, "0"c) & "'")

                            'If oraReader.DataReader(sql) = False Then

                            '    pError = pSeqNo & ",学校コード,学校マスタに登録されている学校コード以外のものは登録できません"
                            '    oraReader.Close()
                            '    Return False
                            'End If
                            'oraReader.Close()
                        Case 2
                            '学年コード

                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",学年コード,未入力です"

                                Exit Function
                            End If

                            '数値チェック
                            If IsNumeric(strSplitValue(intCnt)) = False Then
                                pError = pSeqNo & ",学年コード,学年コードに数値以外を設定することはできません"
                                Exit Function
                            Else
                                '入力値範囲チェック
                                Select Case CLng(strSplitValue(intCnt))
                                    Case 1 To 9
                                    Case Else
                                        pError = pSeqNo & ",学年コード,学年コードは１～９以外の数値を設定することはできません"
                                        Exit Function
                                End Select
                            End If

                            '学校マスタチェック(入力学年)
                            sql = New StringBuilder(128)
                            oraReader = New MyOracleReader(MainDB)
                            sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                            sql.Append(" WHERE")
                            sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(1).PadLeft(10, "0"c)) & "'")
                            sql.Append(" AND")
                            sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(intCnt)))

                            If oraReader.DataReader(sql) = False Then
                                pError = pSeqNo & ",学年コード,学年コードが存在しません"
                                oraReader.Close()
                                Return False
                            End If
                            oraReader.Close()
                        Case 3
                            '費目ID
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",費目ID,未入力です"
                                Exit Function
                            End If

                            '費目IDの長さチェック 2007/02/12
                            If strSplitValue(intCnt).Trim.Length >= 4 Then
                                pError = pSeqNo & ",費目ID,費目IDが４桁を超えて設定されています"
                                Exit Function
                            End If
                        Case 4
                            '費目名称
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",費目名,未入力です"
                                Exit Function
                            End If
                            '費目名称の長さチェック 
                            '2011/01/27
                            If strSplitValue(intCnt).Trim.Length > 20 Then
                                pError = pSeqNo & ",費目名,費目名称が20桁を超えて設定されています"
                                Exit Function
                            End If
                        Case 5
                            '請求月
                            '費目IDが000(決済口座)の場合請求月が入力されていないためチェックは行わない
                            Select Case strSplitValue(3).Trim.PadLeft(3, "0"c)
                                Case Is <> "000"
                                    If IsNumeric(strSplitValue(intCnt)) = False Then

                                        pError = pSeqNo & ",請求月,未入力です"

                                        Exit Function
                                    Else
                                        If IsNumeric(strSplitValue(intCnt)) = False Then
                                            pError = pSeqNo & ",請求月,請求月は１～１２以外の数値を設定することはできません"
                                            Exit Function
                                        Else
                                            Select Case CLng(strSplitValue(intCnt))
                                                Case 1 To 12
                                                Case Else
                                                    pError = pSeqNo & ",請求月,請求月は１～１２以外の数値を設定することはできません"

                                                    Exit Function
                                            End Select

                                        End If
                                    End If
                            End Select
                        Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69
                            '費目名称
                            '費目名称の長さチェック 
                            If strSplitValue(intCnt).Trim.Length > 20 Then
                                pError = pSeqNo & ",費目名称,費目名称が20桁を超えて設定されています"

                                Exit Function
                            End If
                        Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70
                            If Trim(strSplitValue(intCnt - 1)) <> "" Then
                                '金融機関コード
                                If Trim(strSplitValue(intCnt)) = "" Then

                                Else

                                    '桁数チェック 2007/02/12
                                    If strSplitValue(intCnt).Trim.Length > 4 Then

                                        pError = pSeqNo & ",決済金融機関,決済金融機関コードが４桁を超えています"

                                        Exit Function
                                    End If

                                    '数値チェック
                                    If IsNumeric(strSplitValue(intCnt)) = False Then
                                        pError = pSeqNo & ",決済金融機関,決済金融機関コードには数値を入力してください"

                                        Exit Function
                                    End If
                                End If
                            End If


                        Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71

                            If Trim(strSplitValue(intCnt - 1)) <> "" Then
                                '支店コード
                                If Trim(strSplitValue(intCnt)) = "" Then

                                Else

                                    '桁数チェック 2007/02/12
                                    If strSplitValue(intCnt).Trim.Length > 3 Then

                                        pError = pSeqNo & ",決済支店,決済支店コードが３桁を超えています"

                                        Exit Function
                                    End If
                                    '数値チェック
                                    If IsNumeric(strSplitValue(intCnt)) = False Then
                                        pError = pSeqNo & ",決済支店,決済支店コードには数値を入力してください"

                                        Exit Function
                                    End If


                                    '金融機関マスタ存在チェック
                                    sql = New StringBuilder(128)
                                    oraReader = New MyOracleReader(MainDB)
                                    sql.Append("SELECT * FROM TENMAST ")
                                    sql.Append(" WHERE")
                                    sql.Append(" KIN_NO_N = '" & Trim(strSplitValue(intCnt - 1)).PadLeft(4, "0"c) & "'")
                                    sql.Append(" AND")
                                    sql.Append(" SIT_NO_N = '" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                                    If oraReader.DataReader(sql) = False Then
                                        pError = pSeqNo & ",決済支店,金融機関マスタに登録されていない金融機関を設定することはできません"
                                        oraReader.Close()
                                        Return False
                                    End If
                                    oraReader.Close()
                                End If
                            End If
                        Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72
                            '決済科目 未入力のときエラーメッセージ出力
                            If Trim(strSplitValue(intCnt - 1)) <> "" Then '支店コードが設定されている場合
                                If Trim(strSplitValue(intCnt)) = "" Then
                                    pError = pSeqNo & ",決済科目,未入力です"
                                    Exit Function
                                Else '2007/02/15
                                    '数値チェック
                                    If IsNumeric(strSplitValue(intCnt)) = False Then
                                        pError = pSeqNo & ",決済科目,決済科目には数値を入力してください"

                                        Exit Function
                                    End If
                                    Select Case CInt(Trim(strSplitValue(intCnt)))
                                        Case 1, 2
                                        Case Else
                                            pError = pSeqNo & ",決済科目,決済科目は普通または当座のみ指定可能です"
                                            Exit Function
                                    End Select

                                End If
                            End If

                        Case 10, 17, 24, 31, 38, 45, 52, 59, 66, 73

                            '口座番号
                            If Trim(strSplitValue(intCnt - 4)) <> "" Then
                                If Trim(strSplitValue(intCnt)) = "" Then

                                Else
                                    '数値チェック
                                    If IsNumeric(strSplitValue(intCnt)) = False Then
                                        pError = pSeqNo & ",決済口座番号,決済口座番号には数値を入力してください"

                                        Exit Function
                                    End If
                                    '決済口座番号桁数チェック
                                    If strSplitValue(intCnt).Trim.Length > 7 Then

                                        pError = pSeqNo & ",決済口座番号,決済口座番号が７桁を超えています"

                                        Exit Function
                                    End If
                                End If
                            End If
                        Case 11, 18, 25, 32, 39, 46, 53, 60, 67, 74

                            '2007/09/19　追加
                            '名義人名(ｶﾅ)

                            '未入力チェック
                            If Trim(strSplitValue(intCnt - 1)) <> "" Then
                                If Trim(strSplitValue(intCnt)) = "" Then

                                Else
                                    '規定外文字チェック 2006/04/16
                                    If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                                        pError = pSeqNo & ",名義人名(ｶﾅ),規定外文字が有ります"
                                        Exit Function
                                    End If
                                    '文字数チェック 2006/05/10
                                    If Trim(strSplitValue(intCnt)).Length > 48 Then
                                        pError = pSeqNo & ",名義人名(ｶﾅ),半角48文字以内で設定してください"
                                        Exit Function
                                    End If

                                End If
                            End If
                        Case 12, 19, 26, 33, 40, 47, 54, 61, 68, 75

                            '費目金額 
                            If strSplitValue(intCnt).Trim <> "" Then
                                If IsNumeric(strSplitValue(intCnt)) = False Then
                                    pError = pSeqNo & ",費目金額,費目金額に数値以外を設定することはできません"
                                    Exit Function
                                End If
                            End If
                            '費目IDの長さチェック
                            If strSplitValue(intCnt).Trim.Length > 8 Then
                                pError = pSeqNo & ",費目金額,費目金額が８桁を超えて設定されています"
                                Exit Function
                            End If
                            'Case 76
                            '    '作成日付
                            '    If Trim(strSplitValue(intCnt)) = "" Then
                            '        'pError = pSeqNo & ",作成日付,未入力です"
                            '        'Exit Function
                            '    Else
                            '        '数値チェック
                            '        If IsNumeric(Trim(strSplitValue(intCnt))) = False Then
                            '            pError = pSeqNo & ",作成日付,作成日付に数値以外を設定することはできません"
                            '            Exit Function
                            '        End If

                            '        '文字数チェック 
                            '        If Trim(strSplitValue(intCnt)).Length > 8 Then
                            '            pError = pSeqNo & ",作成日付,作成日付が８桁を超えて設定されています"
                            '            Exit Function
                            '        End If
                            '    End If
                            'Case 77
                            '    '更新日付
                            '    If Trim(strSplitValue(intCnt)) = "" Then
                            '        'pError = pSeqNo & "更新日付,未入力です"
                            '        'Exit Function
                            '    Else
                            '        '数値チェック
                            '        If IsNumeric(Trim(strSplitValue(intCnt))) = False Then
                            '            pError = pSeqNo & ",更新日付,更新日付に数値以外を設定することはできません"
                            '            Exit Function
                            '        End If
                            '        '文字数チェック
                            '        If Trim(strSplitValue(intCnt)).Length > 8 Then
                            '            pError = pSeqNo & ",更新日付,更新日付が８桁を超えて設定されています"
                            '            Exit Function
                            '        End If
                            '    End If
                    End Select
                Next intCnt
            Else
                pError = pSeqNo & ",項目数,費目情報データの項目数に不備があります"
                Exit Function
            End If

            PFUNC_CHECK_HIMOKU = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "費目チェック例外エラー", "失敗", intCnt & ex.Message)
            Return False
        End Try
    End Function
    Private Function PFUNC_INSERT_HIMOKU(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intCnt As Integer
        Dim strSplitValue() As String
        Dim strRET As String = ""
        Dim KIN_NO As String = ""
        Dim SIT_NO As String = ""
        Dim KOUZA_NO As String = ""

        Dim sql As New StringBuilder(128)

        PFUNC_INSERT_HIMOKU = False
        Try
            strSplitValue = Split(pLineValue, ",")

            '学校マスタより金融機関コード、支店コード、口座番号取得
            Get_HIMOKU_INFO(strSplitValue(1), KIN_NO, SIT_NO, KOUZA_NO)
            For intCnt = 0 To 112
                Select Case intCnt
                    Case 0
                        If strSplitValue(intCnt).Trim = "9" Then
                            Return True
                            Exit Function
                        Else
                            sql.Append(" INSERT INTO HIMOMAST VALUES(")
                        End If
                    Case 1
                        sql.Append("'" & strSplitValue(intCnt).Trim.PadLeft(10, "0"c) & "'")
                        '学年＆費目1～10までの金額
                    Case 2, 12, 19, 26, 33, 40, 47, 54, 61, 68, 75
                        If Trim(strSplitValue(intCnt)) = "" Then
                            sql.Append(",0")
                        Else
                            sql.Append("," & strSplitValue(intCnt))
                        End If
                        '費目11～15(未設定なので固定)
                    Case 82, 89, 96, 103, 110
                        sql.Append(",0")
                    Case 3
                        '費目ID前ZERO詰め
                        sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")
                    Case 4
                        '費目ID名
                        strSplitValue(intCnt) = StrConv(strSplitValue(intCnt), VbStrConv.Wide)
                        If strSplitValue(intCnt).Trim.Length > 15 Then
                            sql.Append(",'" & strSplitValue(intCnt).Substring(0, 15) & "'")
                        Else
                            sql.Append(",'" & strSplitValue(intCnt) & "'")
                        End If
                    Case 5
                        '請求月
                        If Trim(strSplitValue(intCnt - 2)) = "000" Then
                            sql.Append(",'  '")
                        Else
                            sql.Append(",'" & Format(CInt(strSplitValue(intCnt)), "00") & "'")
                        End If
                        '費目1～10まで
                    Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69
                        '費目名
                        strSplitValue(intCnt) = StrConv(strSplitValue(intCnt), VbStrConv.Wide)
                        If strSplitValue(intCnt).Trim.Length > 10 Then
                            sql.Append(",'" & strSplitValue(intCnt).Substring(0, 10) & "'")
                        Else
                            sql.Append(",'" & strSplitValue(intCnt) & "'")
                        End If
                        '費目11～15(未設定なので固定)
                    Case 76, 83, 90, 97, 104
                        sql.Append(",''")
                        'Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69, 76, 83, 90, 97, 104
                        '費目1～10まで
                    Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70
                        '決済金融機関
                        If Trim(strSplitValue(intCnt - 1)) <> "" Then
                            If strSplitValue(intCnt).Trim = "" Then
                                sql.Append(",'" & KIN_NO & "'")
                            Else
                                sql.Append(",'" & strSplitValue(intCnt).PadLeft(4, "0"c) & "'")
                            End If
                        Else
                            sql.Append(",''")
                        End If
                        '費目11～15(未設定なので固定)
                    Case 77, 84, 91, 98, 105
                        sql.Append(",''")
                    Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71
                        '決済金融機関支店
                        If Trim(strSplitValue(intCnt - 2)) <> "" Then
                            If strSplitValue(intCnt).Trim = "" Then
                                sql.Append(",'" & SIT_NO & "'")
                            Else
                                sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(3, "0"c) & "'")
                            End If
                        Else
                            sql.Append(",''")
                        End If
                        '費目11～15(未設定なので固定)
                    Case 78, 85, 92, 99, 106
                        sql.Append(",''")
                        '費目1～10まで
                    Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72
                        '決済科目
                        If Trim(strSplitValue(intCnt - 3)) <> "" Then
                            If Trim(strSplitValue(intCnt)) = "" Then
                                sql.Append(",'01'")
                            Else
                                Select Case Trim(strSplitValue(intCnt)).PadLeft(2, "0"c)
                                    Case "01", "02", "05", "37"
                                        sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(2, "0"c) & "'")
                                    Case Else
                                        sql.Append(",'01'")
                                End Select
                            End If
                        Else
                            sql.Append(",''")
                        End If
                        '費目11～15(未設定なので固定)
                    Case 79, 86, 93, 100, 107
                        sql.Append(",''")
                        '費目1～10まで
                    Case 10, 17, 24, 31, 38, 45, 52, 59, 66, 73
                        '口座番号
                        If Trim(strSplitValue(intCnt - 4)) <> "" Then
                            If strSplitValue(intCnt).Trim = "" Then
                                sql.Append(",'" & KOUZA_NO & "'")
                            Else
                                sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(7, "0"c) & "'")
                            End If
                        Else
                            sql.Append(",''")
                        End If
                        '費目11～15(未設定なので固定)
                    Case 80, 87, 94, 101, 108
                        sql.Append(",''")

                    Case 11, 18, 25, 32, 39, 46, 53, 60, 67, 74
                        '名義人（小文字等を変換し設定）
                        If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = True Then
                            sql.Append(",'" & strRET & "'")
                        Else
                            sql.Append(",'" & Space(40) & "'")
                        End If
                        '費目11～15(未設定なので固定)
                    Case 81, 88, 95, 102, 109
                        sql.Append(",''")

                    Case 111
                        '作成日付
                        sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
                    Case 112
                        '更新日付
                        sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")

                        sql.Append(",'" & Space(50) & "'")
                        sql.Append(",'" & Space(50) & "'")
                        sql.Append(",'" & Space(50) & "'")
                        sql.Append(",'" & Space(50) & "'")
                        sql.Append(",'" & Space(50) & "'")
                        sql.Append(")")

                    Case Else
                        sql.Append(",'" & strSplitValue(intCnt) & "'")
                End Select
            Next intCnt

            'sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
            'sql.Append(",'00000000'")


            If MainDB.ExecuteNonQuery(sql) < 0 Then
                pError = pSeqNo & ",移入,移入処理中にエラーが発生しました"
                Return False
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "費目SQL例外エラー", "失敗", intCnt & ex.Message)
        End Try

    End Function
    Private Function PFUNC_IMP_SEITO(ByVal gakkou_code_S As String, ByVal pCsvPath As String, ByRef lngOkCnt As Long, ByRef lngErrCnt As Long) As Boolean

        Dim red As System.IO.StreamReader = Nothing

        Dim strReadLine As String
        Dim Shinsei_KBN As String   '申請区分
        Dim NENDO As Integer        '入学年度
        Dim TUUBAN As Integer       '通番

        Dim strErrorLine As String = ""
        Dim strSEITOLine As String = ""

        lngOkCnt = 0
        lngErrCnt = 0
        Dim blnSQL As Boolean = True

        PFUNC_IMP_SEITO = False

        Try

            Dim dirs As String() = System.IO.Directory.GetFiles(GAKTOOL_IN_PATH)
            Dim dir As String
            Dim dirflg As String

            dirflg = "0"
            For Each dir In dirs
                If dir = pCsvPath Then
                    dirflg = "1"
                End If
            Next

            If dirflg = "0" Then
                err_code = 2
                Throw New FileNotFoundException(String.Format("ファイルが見つかりません。" _
                                        & ControlChars.Cr & "'{0}'", pCsvPath))
            End If

            red = New System.IO.StreamReader(pCsvPath, System.Text.Encoding.Default)

            Do While Not red.Peek() = -1

                strReadLine = red.ReadLine.ToString

                '入力内容のチェック
                Shinsei_KBN = ""
                If PFUNC_CHECK_SEITO(gakkou_code_S, Lng_Seito_SeqNo, strReadLine, strErrorLine, Shinsei_KBN, NENDO, TUUBAN) = True Then

                    Dim strSeitoInfoOld As String = ""

                    '20130405 旧生徒情報の取得
                    If Not GetSeitoInfo(gakkou_code_S, NENDO, TUUBAN, strSeitoInfoOld) Then
                        Return False
                    End If

                    '読み込みログの書き出し
                    Select Case Shinsei_KBN
                        Case 1
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "新規," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                        Case 2
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "変更," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "0") = False Then
                                Return False
                            End If
                            If Get_SEITO_UPDATE_BEFORE(gakkou_code_S, NENDO, TUUBAN, strReadLine, strSEITOLine) = False Then
                                'Return False
                            End If
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "変更前," & strSEITOLine & ",," & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                        Case 9
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "削除," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                    End Select
                    Select Case Shinsei_KBN
                        Case "1"    '登録
                            'SQL文作成＆SQL処理実行
                            If PFUNC_INSERT_SEITO(Lng_Seito_SeqNo, strReadLine, strErrorLine) = True Then
                                lngOkCnt += 1
                            Else

                                lngErrCnt += 1
                                blnSQL = False

                                'ｴﾗｰﾛｸﾞの書き出し
                                If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                                    Return False
                                End If
                                ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_INSERT_SEITO", "失敗", Replace(strErrorLine, ",", "/"))

                                'SQLでエラーが発生した場合は処理中断
                                red.Close()
                                Exit Do
                            End If
                            'ｴﾗｰﾛｸﾞの書き出し（口座情報取得エラーの場合のみ）
                            If strErrorLine <> "" Then
                                If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                                    Return False
                                End If
                            End If
                        Case "2"    '更新
                            If PFUNC_UPDATE_SEITO(Lng_Seito_SeqNo, strReadLine, strErrorLine) = True Then
                                lngOkCnt += 1
                            Else

                                lngErrCnt += 1
                                blnSQL = False

                                'ｴﾗｰﾛｸﾞの書き出し
                                If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                                    Return False
                                End If
                                ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_UPDATE_SEITO", "失敗", Replace(strErrorLine, ",", "/"))

                                'SQLでエラーが発生した場合は処理中断
                                red.Close()
                                Exit Do
                            End If
                            'ｴﾗｰﾛｸﾞの書き出し（口座情報取得エラーの場合のみ）
                            If strErrorLine <> "" Then
                                If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                                    Return False
                                End If
                            End If
                        Case "9"    '削除
                            If PFUNC_DELETE_SEITO(Lng_Seito_SeqNo, strReadLine, strErrorLine) = True Then
                                'lngOkCnt += 1
                            Else

                                lngErrCnt += 1
                                blnSQL = False

                                'ｴﾗｰﾛｸﾞの書き出し
                                If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                                    Return False
                                End If
                                ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_DELETE_SEITO", "失敗", Replace(strErrorLine, ",", "/"))

                                'SQLでエラーが発生した場合は処理中断
                                red.Close()
                                Exit Do
                            End If
                        Case Else
                            lngOkCnt += 1
                    End Select
                Else
                    lngErrCnt += 1

                    Dim strSeitoInfoOld As String = ""

                    '20130405 旧生徒情報の取得
                    If Not GetSeitoInfo(gakkou_code_S, NENDO, TUUBAN, strSeitoInfoOld) Then
                        Return False
                    End If

                    '読み込みログの書き出し
                    Select Case Shinsei_KBN
                        Case 1
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "新規," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                        Case 2
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "変更," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "0") = False Then
                                Return False
                            End If
                            If Get_SEITO_UPDATE_BEFORE(gakkou_code_S, NENDO, TUUBAN, strReadLine, strSEITOLine) = False Then
                                Return False
                            End If
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "変更前," & strSEITOLine & ",," & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                        Case 9
                            If PFUNC_PRINT_LOG_WRITE(Lng_Seito_SeqNo.ToString & "," & "削除," & strReadLine & "," & strKANA_MEI & "," & strKANJI_MEI & "," & strSeitoInfoOld & "," & "1") = False Then
                                Return False
                            End If
                    End Select
                    'ｴﾗｰﾛｸﾞの書き出し
                    If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                        Return False
                    End If
                    ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_CHECK_SEITO", "失敗", Replace(strErrorLine, ",", "/"))

                End If

                Lng_Seito_SeqNo += 1
            Loop

            '生徒データ取込登録リスト出力
            If File.Exists(KobetuLogFileName2) Then
                'If lngErrCnt > 0 Then
                Dim param As String = ""
                Dim nret As Integer = 0
                'パラメータ設定
                param = GCom.GetUserID & "," & gakkou_code_S ' & "," & pCsvPath

                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me
                nret = ExeRepo.ExecReport("KFGP501.EXE", param)
                '戻り値に対応したメッセージを表示する
                Select Case nret
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "生徒データ取込登録リスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                End Select
            Else
                MessageBox.Show("更新対象がありませんでした。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            If blnSQL = False Then
                Return False
            Else
                red.Close()
                Return True
            End If


        Catch ex As FileNotFoundException
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                MessageBox.Show("ファイルが見つかりません。" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "生徒ファイルなし", "失敗", ex.Message)
            Else
                Return True
            End If
        Catch ex As System.IO.IOException
            MessageBox.Show("ファイルが使用中です。" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "生徒ファイルアクセス", "失敗", ex.Message)
            err_code = 1
            err_Filename = pCsvPath
            Return False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "生徒チェック例外エラー", "失敗", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function

    Private Function PFUNC_CHECK_SEITO(ByVal gakkou_code_C As String, ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String, ByRef S_KBN As String, ByRef NENDO As Integer, ByRef TUUBAN As Integer) As Boolean

        Dim intCnt As Integer
        Dim intCnt2 As Integer
        Dim strSplitValue() As String
        Dim strRET As String = ""

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader


        PFUNC_CHECK_SEITO = False
        Try
            strKANJI_MEI = ""
            strKANA_MEI = ""
            strADDRESS = ""
            strTELNO = ""

            pError = ""

            strSplitValue = Split(pLineValue, ",")

            '入力内容チェック
            If strSplitValue.Length = 44 Then
                For intCnt = 0 To UBound(strSplitValue)
                    Select Case intCnt
                        Case 0
                            '申請区分
                            Select Case Trim(strSplitValue(intCnt))
                                Case 1
                                    S_KBN = "1"
                                Case 2
                                    S_KBN = "2"
                                Case 9
                                    S_KBN = "9"
                                Case ""
                                    '2011/12/14
                                    PFUNC_CHECK_SEITO = True
                                    Exit Function
                                    '2011/12/14
                                Case Else
                                    pError = pSeqNo & ",申請区分,申請区分入力不備です"
                                    Exit Function
                            End Select
                        Case 1
                            '学校コード

                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",学校,未入力です"

                                Exit Function
                            End If

                            '画面上で入力されているコード以外のコードの場合はエラー
                            If gakkou_code_C <> Trim(strSplitValue(intCnt)).PadLeft(10, "0"c) Then

                                pError = pSeqNo & ",学校コード,ファイル名とデータの学校コードが違います"
                                Exit Function
                            End If

                        Case 2
                            '入学年度

                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",入学年度,未入力です"

                                Exit Function
                            End If

                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",入学年度,数値以外を設定することはできません"

                                Exit Function

                            End If
                            '入力桁数チェック
                            If strSplitValue(intCnt).Trim.Length > 4 Then
                                pError = pSeqNo & ",入学年度,入学年度が４桁を超えて設定されています"

                                Exit Function

                            End If

                            If strSplitValue(intCnt).Trim.Length < 4 Then
                                pError = pSeqNo & ",入学年度,入学年度が４桁未満で設定されています"

                                Exit Function

                            End If

                            NENDO = CInt(strSplitValue(intCnt))

                        Case 3
                            '通番

                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",通番,未入力です"

                                Exit Function
                            End If

                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",通番,数値以外を設定することはできません"

                                Exit Function
                            Else
                                '入力値範囲チェック
                                Select Case CLng(strSplitValue(intCnt))
                                    Case 1 To 9999
                                    Case Else
                                        pError = pSeqNo & ",通番,1～9999以外の数値を設定することはできません"
                                        Exit Function
                                End Select
                            End If

                            TUUBAN = CInt(strSplitValue(intCnt))

                        Case 4
                            '学年コード

                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",学年コード,未入力です"

                                Exit Function
                            End If

                            '数値チェック
                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",学年コード,学年に数値以外を設定することはできません"

                                Exit Function
                            Else
                                '入力値範囲チェック
                                Select Case CLng(strSplitValue(intCnt))
                                    Case 1 To 9
                                    Case Else
                                        pError = pSeqNo & ",学年コード,学年は1～9以外の数値を設定することはできません"
                                        Exit Function
                                End Select
                            End If

                            '学校マスタチェック(入力学年)
                            sql = New StringBuilder(128)
                            oraReader = New MyOracleReader(MainDB)
                            sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                            sql.Append(" WHERE")
                            sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(1)).PadLeft(10, "0"c) & "'")
                            sql.Append(" AND")
                            sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(intCnt)))

                            If oraReader.DataReader(sql) = False Then
                                pError = pSeqNo & ",学年コード,学年コードが学校マスタに登録されていません"
                                oraReader.Close()
                                Return False
                            End If
                            oraReader.Close()
                        Case 5
                            'クラスコード
                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",クラス,未入力です。"

                                Exit Function
                            Else

                                If IsNumeric(strSplitValue(intCnt)) = False Then

                                    pError = pSeqNo & ",クラス,数値以外を設定することはできません"

                                    Exit Function
                                Else
                                    '入力値範囲チェック
                                    Select Case CLng(strSplitValue(intCnt))
                                        Case 1 To 99
                                        Case Else
                                            pError = pSeqNo & ",クラス,1～99以外の数値を設定することはできません"

                                            Exit Function
                                    End Select
                                End If

                                '使用クラスチェック
                                sql = New StringBuilder(128)
                                oraReader = New MyOracleReader(MainDB)
                                sql.Append("SELECT * FROM GAKMAST1")
                                sql.Append(" WHERE")
                                sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(1)).PadLeft(10, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(4)))
                                sql.Append(" AND (")
                                For intCnt2 = 1 To 20
                                    sql.Append(IIf(intCnt2 = 1, "", "or") & " CLASS_CODE1" & Format(intCnt2, "00") & "_G = '" & Trim(strSplitValue(intCnt)).PadLeft(2, "0"c) & "'")
                                Next intCnt2
                                sql.Append(" )")

                                If oraReader.DataReader(sql) = False Then
                                    pError = pSeqNo & ",クラス,学校マスタに存在しないクラスを設定することは出来ません"
                                    oraReader.Close()
                                    Return False
                                End If
                                oraReader.Close()
                            End If
                        Case 6
                            '生徒番号

                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",生徒番号,未入力です"

                                Exit Function
                            End If

                            '数値チェック
                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",生徒番号,生徒番号に数値以外を設定することはできません。"

                                Exit Function
                            End If

                            '桁数チェック 2007/02/12
                            If strSplitValue(intCnt).Trim.Length > 7 Then

                                pError = pSeqNo & ",生徒番号,生徒番号が７桁を超えています"

                                Exit Function
                            End If

                        Case 7
                            '生徒氏名(ｶﾅ)

                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",生徒氏名(ｶﾅ),未入力です"

                                Exit Function
                            Else
                                '規定外文字チェック 2006/04/16
                                If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                                    pError = pSeqNo & ",生徒氏名(ｶﾅ),規定外文字が有ります"
                                    Exit Function
                                End If
                                '文字数チェック 2006/05/10
                                If Trim(strSplitValue(intCnt)).Length > 40 Then
                                    pError = pSeqNo & ",生徒氏名(ｶﾅ),半角40文字以内で設定してください"
                                    Exit Function
                                End If

                            End If
                        Case 8 '生徒名漢字　長さチェック
                            If Trim(strSplitValue(intCnt)) <> "" Then
                                '２バイト変換
                                strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Wide)
                                '文字数チェック
                                If Trim(strSplitValue(intCnt)).Length > 25 Then
                                    pError = pSeqNo & ",生徒氏名(漢字),全角25文字以内で設定してください"
                                    Exit Function
                                End If
                            End If
                        Case 9
                            '性別
                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",性別,未入力です"
                                Exit Function
                            Else
                                '数値チェック
                                If IsNumeric((strSplitValue(intCnt))) = False Then
                                    pError = pSeqNo & ",性別,性別に数値以外を設定することはできません。"
                                    Exit Function
                                Else
                                    '0、1、2以外は許可しない
                                    Select Case CLng(strSplitValue(intCnt))
                                        Case 0, 1, 2
                                        Case Else
                                            pError = pSeqNo & ",性別,性別は0～2以外の数値を設定することはできません"
                                            Exit Function
                                    End Select

                                End If
                            End If
                        Case 10
                            '費目ID
                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",費目ID,未入力です"
                                Exit Function
                            Else
                                '数値チェック
                                If IsNumeric((strSplitValue(intCnt))) = False Then
                                    pError = pSeqNo & ",費目ID,費目IDに数値以外を設定することはできません。"
                                    Exit Function
                                Else
                                    '桁数チェック
                                    If strSplitValue(intCnt).Trim.Length > 3 Then
                                        pError = pSeqNo & ",費目ID,費目IDが３桁を超えて設定されています"

                                        Exit Function
                                    End If

                                    '費目マスタ存在チェック
                                    sql = New StringBuilder(128)
                                    oraReader = New MyOracleReader(MainDB)
                                    sql.Append(" SELECT * FROM HIMOMAST")
                                    sql.Append(" WHERE")
                                    sql.Append(" GAKKOU_CODE_H  ='" & Trim(strSplitValue(1)).PadLeft(10, "0"c) & "'")
                                    sql.Append(" AND")
                                    sql.Append(" GAKUNEN_CODE_H =" & Trim(strSplitValue(4)))
                                    sql.Append(" AND")
                                    sql.Append(" HIMOKU_ID_H    ='" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                                    If oraReader.DataReader(sql) = False Then

                                        pError = pSeqNo & ",費目ID,費目マスタに登録されていない費目IDを設定することはできません"
                                        oraReader.Close()
                                        Return False
                                    End If
                                    oraReader.Close()
                                End If
                            End If

                        Case 11, 13, 15, 17, 19, 21, 23, 25, 27, 29
                            '請求方法
                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",請求方法" & Format((intCnt - 9) \ 2, "00") & ",請求方法が未入力です。"
                                Exit Function
                            End If

                            '数値チェック
                            If IsNumeric((strSplitValue(intCnt))) = False Then
                                pError = pSeqNo & ",請求方法" & Format((intCnt - 9) \ 2, "00") & ",請求方法に数値以外を設定することはできません"
                                Exit Function
                            Else
                                '0、1以外は許可しない
                                Select Case CLng(strSplitValue(intCnt))
                                    Case 0, 1
                                    Case Else
                                        pError = pSeqNo & ",請求方法" & Format((intCnt - 9) \ 2, "00") & ",請求方法は0,1以外の数値を設定することはできません"
                                        Exit Function
                                End Select

                            End If

                        Case 12, 14, 16, 18, 20, 22, 24, 26, 28, 30
                            '費目金額
                            '未入力チェック
                            If IsNumeric(strSplitValue(intCnt)) = False Then
                                pError = pSeqNo & ",費目金額" & Format((intCnt - 10) \ 2, "00") & ",費目金額に数値以外を設定することはできません"
                                Exit Function
                            End If

                            If strSplitValue(intCnt).Trim.Length > 12 Then
                                pError = pSeqNo & ",費目金額" & Format((intCnt - 10) \ 2, "00") & ",費目金額が１２桁を超えて設定されています"
                                Exit Function
                            End If

                        Case 31
                            '合計金額　チェックなし
                        Case 32
                            '取扱金融機関コード

                            '数値チェック
                            If Trim(strSplitValue(intCnt)) = "" Or IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",取扱金融機関,金融機関コードに空白または数値ではない値が設定されています"

                                Exit Function
                            End If

                            '桁数チェック
                            If strSplitValue(intCnt).Trim.Length > 4 Then

                                pError = pSeqNo & ",取扱金融機関,金融機関コードが４桁を超えています"

                                Exit Function
                            End If
                        Case 33
                            '金融機関名チェックなし
                        Case 34
                            '取扱支店コード
                            '数値チェック
                            If Trim(strSplitValue(intCnt)) = "" Or IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",取扱支店,支店コードに空白または数値以外の値が設定されています"

                                Exit Function
                            End If

                            '桁数チェック
                            If strSplitValue(intCnt).Trim.Length > 3 Then

                                pError = pSeqNo & ",取扱支店,支店コードが３桁を超えています"

                                Exit Function
                            End If

                            If Trim(strSplitValue(40)) = "0" Then
                                '金融機関マスタ存在チェック
                                sql = New StringBuilder(128)
                                oraReader = New MyOracleReader(MainDB)
                                sql.Append("SELECT * FROM TENMAST ")
                                sql.Append(" WHERE")
                                sql.Append(" KIN_NO_N = '" & Trim(strSplitValue(intCnt - 2)).PadLeft(4, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" SIT_NO_N = '" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                                If oraReader.DataReader(sql) = False Then

                                    pError = pSeqNo & ",取扱支店,金融機関マスタに登録されていません"
                                    oraReader.Close()
                                    Return False
                                End If
                                oraReader.Close()
                            End If
                        Case 35
                            '支店名チェックなし
                        Case 36
                            '科目
                            If Trim(strSplitValue(intCnt)) = "" Or IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",科目,科目に空白または数値以外の値が設定されています"

                                Exit Function
                            Else
                                If Trim(strSplitValue(intCnt)).Length > 2 Then
                                    pError = pSeqNo & ",科目,科目が2桁を超えています"
                                    Exit Function
                                End If
                                Select Case CDec(Trim(strSplitValue(intCnt)))
                                    Case 2, 1, 37, 5, 9
                                    Case Else
                                        pError = pSeqNo & ",科目,存在しない科目が設定されています"
                                        Exit Function
                                End Select
                            End If
                        Case 37
                            '口座番号
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",取扱口座番号,未入力です"

                                Exit Function
                            End If

                            '数値チェック  2007/02/12
                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",取扱口座番号,口座番号に数値以外の値が設定されています"

                                Exit Function
                            End If

                            '桁数チェック 2007/02/12
                            If strSplitValue(intCnt).Trim.Length > 7 Then

                                pError = pSeqNo & ",取扱口座番号,口座番号が７桁を超えて設定されています"

                                Exit Function
                            End If


                        Case 38
                            '名義人(ｶﾅ)
                            'カスタマイズ　口座チェック・情報取得を行う。

                            If strSplitValue(intCnt).Trim = STR_JIKINKO_CODE Then
                                '口座マスタ存在チェック
                                sql = New StringBuilder(128)
                                oraReader = New MyOracleReader(MainDB)
                                sql.Append("SELECT KOKYAKU_KNAME_D, KOKYAKU_NNAME_D FROM KDBMAST ")
                                sql.Append(" WHERE")
                                sql.Append(" TSIT_NO_D = '" & Trim(strSplitValue(intCnt - 4)).PadLeft(3, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" KAMOKU_D = '" & Trim(strSplitValue(intCnt - 2)).PadLeft(2, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" KOUZA_D = '" & Trim(strSplitValue(intCnt - 1)).PadLeft(7, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" KATU_KOUZA_D = '1'") '活口座のみ使用する

                                If oraReader.DataReader(sql) = False Then
                                    pError = pSeqNo & ",○,口座マスタ(KDBMAST)に登録されていません。（正常扱い）"
                                    oraReader.Close()
                                    'Return False
                                Else
                                    strADDRESS = ""
                                    strTELNO = ""
                                    oraReader.Close()
                                End If
                            End If

                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",取扱名義人(ｶﾅ),未入力です"

                                Exit Function
                            Else

                                '規定外文字チェック 2006/04/16
                                If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                                    pError = pSeqNo & ",取扱名義人(ｶﾅ),規定外文字が有ります"
                                    Exit Function
                                End If
                                '文字数チェック 2006/05/10
                                If Trim(strSplitValue(intCnt)).Length > 40 Then
                                    pError = pSeqNo & ",取扱名義人(ｶﾅ),半角40文字以内で設定してください"
                                    Exit Function
                                End If
                            End If

                            strKANA_MEI = Trim(strSplitValue(intCnt))

                        Case 39    '名義人漢字

                            If Trim(strSplitValue(intCnt)) <> "" Then
                                '２バイト変換
                                strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Wide)
                                '文字数チェック
                                If Trim(strSplitValue(intCnt)).Length > 25 Then
                                    pError = pSeqNo & ",名義人(漢字),全角25文字以内で設定してください"
                                    Exit Function
                                End If
                            End If

                            strKANJI_MEI = Trim(strSplitValue(intCnt))

                        Case 40
                            '振替方法
                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",振替方法,未入力です"
                                Exit Function
                            Else
                                '数値チェック
                                If IsNumeric((strSplitValue(intCnt))) = False Then
                                    pError = pSeqNo & ",振替方法,振替方法に数値以外を設定することはできません。"
                                    Exit Function
                                Else
                                    '0、1、2以外は許可しない
                                    Select Case CLng(strSplitValue(intCnt))
                                        Case 0, 1, 2
                                        Case Else
                                            pError = pSeqNo & ",振替方法,振替方法は0～2以外の数値を設定することはできません"
                                            Exit Function
                                    End Select

                                End If
                            End If
                        Case 41
                        Case 42
                        Case 43
                            '解約区分
                            '未入力チェック
                            If Trim(strSplitValue(intCnt)) = "" Then
                                pError = pSeqNo & ",解約区分,未入力です"
                                Exit Function
                            Else
                                '数値チェック
                                If IsNumeric((strSplitValue(intCnt))) = False Then
                                    pError = pSeqNo & ",解約区分,解約区分に数値以外を設定することはできません。"
                                    Exit Function
                                Else
                                    '0、9以外は許可しない
                                    Select Case CLng(strSplitValue(intCnt))
                                        Case 0, 9
                                        Case Else
                                            pError = pSeqNo & ",解約区分,解約区分は0,9以外の数値を設定することはできません"
                                            Exit Function
                                    End Select

                                End If
                            End If
                    End Select
                Next intCnt
            Else
                pError = pSeqNo & ",項目数,生徒情報データの項目数に不備があります"
                'pError = pSeqNo & ",生徒情報データの項目数に不備があります"
                Exit Function
            End If

            PFUNC_CHECK_SEITO = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "生徒チェック例外エラー", "失敗", intCnt & ex.Message)
            Return False
        End Try
    End Function
    Private Function PFUNC_INSERT_SEITO(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean
        '新規登録
        Dim intCnt As Integer = 0
        Dim strSplitValue() As String
        Dim strRET As String = ""
        Dim sql As StringBuilder
        Dim strSql As String = ""
        PFUNC_INSERT_SEITO = False
        Try

            strSplitValue = Split(pLineValue, ",")

            'SQL文作成
            For intTuki As Integer = 1 To 12
                sql = New StringBuilder(128)
                strSql = ""
                Dim TUKI_FURIKIN As Long = 0
                For intCnt = 0 To 65
                    Select Case intCnt
                        Case 0
                            If strSplitValue(intCnt).Trim = "9" Then
                                Return True
                                Exit Function
                            Else
                                sql.Append(" INSERT INTO SEITOMAST VALUES(")
                            End If
                        Case 1
                            '学校コード
                            sql.Append("'" & strSplitValue(intCnt).Trim.PadLeft(10, "0"c) & "'")

                        Case 2
                            '年度
                            sql.Append("," & "'" & strSplitValue(intCnt).Trim.PadLeft(4, "0"c) & "'")

                        Case 3, 4, 5
                            '通番,学年ｺｰﾄﾞ,ｸﾗｽｺｰﾄﾞ
                            sql.Append("," & Trim(strSplitValue(intCnt)))
                        Case 6
                            '生徒番号
                            sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(7, "0"c) & "'")
                        Case 7 '生徒氏名(ｶﾅ)
                            If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = True Then
                                sql.Append(",'" & strRET & "'")
                            Else
                                sql.Append(",'" & Space(40) & "'")
                            End If
                        Case 8
                            '生徒氏名(漢字)
                            sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                        Case 9
                            '性別
                            '値なしの場合は２固定
                            Select Case Trim(strSplitValue(intCnt))
                                Case "0", "1", "2"
                                    sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                                Case Else
                                    sql.Append(",'2'")
                            End Select
                        Case 10
                            '取扱金融機関コード
                            If Trim(strSplitValue(32)) <> "" Then
                                sql.Append(",'" & Trim(strSplitValue(32)).PadLeft(4, "0"c) & "'")
                            Else
                                sql.Append(",' '")
                            End If
                        Case 11
                            '取扱支店コード
                            If Trim(strSplitValue(34)) <> "" Then
                                sql.Append(",'" & Trim(strSplitValue(34)).PadLeft(3, "0"c) & "'")
                            Else
                                sql.Append(",' '")
                            End If
                        Case 12
                            '科目
                            Select Case Trim(strSplitValue(36)).PadLeft(2, "0"c)
                                Case "01", "02", "05", "09", "37"
                                    sql.Append(",'" & Trim(strSplitValue(36)).PadLeft(2, "0"c) & "'")
                                Case Else
                                    sql.Append(",'02'")
                            End Select
                        Case 13
                            '口座番号
                            sql.Append(",'" & Trim(strSplitValue(37)).PadLeft(7, "0"c) & "'")
                        Case 14
                            '名義人(ｶﾅ) 2007/02/12
                            If ConvKanaNGToKanaOK(strSplitValue(38), strRET) = True Then
                                sql.Append(",'" & strRET & "'")
                            Else
                                sql.Append(",'" & Space(40) & "'")
                            End If
                        Case 15
                            '名義人(漢字)
                            If Trim(strSplitValue(39)) = "" Then
                                sql.Append(",'" & Space(50) & "'")
                            Else
                                sql.Append(",'" & Trim(strSplitValue(39)) & "'")
                            End If
                        Case 16
                            '振替方法
                            '値なしの場合は０固定
                            Select Case Trim(strSplitValue(40))
                                Case "0", "1", "2"
                                    sql.Append(",'" & Trim(strSplitValue(40)) & "'")
                                Case Else
                                    sql.Append(",'0'")
                            End Select
                        Case 17
                            '契約者住所
                            sql.Append(",'" & Space(50) & "'")
                        Case 18
                            ' カスタマイズ
                            sql.Append(",'" & Space(13) & "'")
                        Case 19
                            '解約区分
                            '値なしの場合は０固定
                            Select Case Trim(strSplitValue(43))
                                Case "0", "9"
                                    sql.Append(",'" & Trim(strSplitValue(43)) & "'")
                                Case Else
                                    sql.Append(",'0'")
                            End Select
                        Case 20
                            '進級区分
                            '基本０固定（進級する）
                            sql.Append(",'0'")
                        Case 21
                            '費目ID
                            sql.Append(",'" & Trim(strSplitValue(10)).PadLeft(3, "0"c) & "'")
                        Case 22, 24 To 26
                            '長子フラグ ,長子通番,長子学年,長子クラス
                            sql.Append(",'0'")
                        Case 23
                            '長子入学年度
                            sql.Append(",'" & Space(4) & "'")
                        Case 27
                            '長子生徒番号
                            sql.Append(",'" & Space(7) & "'")
                        Case 28
                            '請求月
                            sql.Append(",'" & Format(intTuki, "00") & "'")

                        Case 29, 31, 33, 35, 37, 39, 41, 43, 45, 47
                            '請求方法1～10 
                            sql.Append(",'" & Trim(strSplitValue(intCnt - 18)) & "'")
                        Case 49, 51, 53, 55, 57
                            '請求方法11～15 
                            sql.Append(", '0'")

                        Case 30, 32, 34, 36, 38, 40, 42, 44, 46, 48
                            '請求金額1～10
                            'If Trim(strSplitValue(intCnt - 18)) = "1" Then
                            sql.Append(", " & CLng(Trim(strSplitValue(intCnt - 18))))
                            'Else
                            '    sql.Append(",0")
                            'End If

                        Case 50, 52, 54, 56, 58
                            '請求金額11～15　0円固定
                            sql.Append(",0")

                        Case 59
                            '作成日付
                            sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")

                        Case 60
                            '更新日付
                            sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")

                            '20130308 maeda
                            'Case 61 To 65
                            ''予備
                            'sql.Append(",''")
                        Case 61
                            '予備1
                            sql.Append(",''")
                        Case 62
                            '予備2
                            sql.Append(",''")
                        Case 63
                            '予備3
                            sql.Append(",''")
                        Case 64
                            '予備4
                            sql.Append(",''")
                        Case 65
                            '予備5
                            sql.Append(",''")
                            '20130308 maeda

                    End Select
                Next intCnt
                sql.Append(" )")
                '重複チェック
                strSql = " SELECT * FROM SEITOMAST"
                strSql += " WHERE"
                strSql += " GAKKOU_CODE_O ='" & Trim(strSplitValue(1)).PadLeft(10, "0"c) & "'"
                strSql += " AND"
                strSql += " NENDO_O ='" & Trim(strSplitValue(2)) & "'"
                strSql += " AND"
                strSql += " TUUBAN_O =" & Trim(strSplitValue(3))
                strSql += " AND"
                strSql += " TUKI_NO_O ='" & Format(intTuki, "00") & "'"

                Dim oraReader As New MyOracleReader(MainDB)
                If oraReader.DataReader(strSql) = True Then
                    pError = pSeqNo & ",主キー,データが重複しています"
                    oraReader.Close()
                    Return False
                End If

                If MainDB.ExecuteNonQuery(sql) <> 1 Then
                    pError = pSeqNo & ",移入,登録処理中にエラーが発生しました"
                    Return False
                End If
            Next intTuki

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "生徒SQL例外エラー", "失敗", intCnt & ex.Message)
        End Try

    End Function
    Private Function PFUNC_UPDATE_SEITO(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean
        '更新
        Dim strSplitValue() As String
        Dim strRET As String = ""
        Dim sql As StringBuilder
        Dim strSql As String = ""
        PFUNC_UPDATE_SEITO = False
        Try

            strSplitValue = Split(pLineValue, ",")

            'SQL文作成
            For intTuki As Integer = 1 To 12
                sql = New StringBuilder(128)
                strSql = ""
                Dim count As Integer = 0
                Dim TUKI_FURIKIN As Long = 0
                sql.Append(" UPDATE SEITOMAST SET ")
                sql.Append(" GAKUNEN_CODE_O = " & Trim(strSplitValue(4)))
                sql.Append(", CLASS_CODE_O = " & Trim(strSplitValue(5)))
                sql.Append(", SEITO_NO_O = '" & Trim(strSplitValue(6)).PadLeft(7, "0"c) & "'")

                '生徒氏名カナ
                If ConvKanaNGToKanaOK(strSplitValue(7), strRET) = True Then
                    sql.Append(", SEITO_KNAME_O = '" & strRET & "'")
                End If
                '生徒名漢字
                strSplitValue(8) = StrConv(Trim(strSplitValue(8)), VbStrConv.Wide)

                sql.Append(", SEITO_NNAME_O = '" & strSplitValue(8) & "'")
                sql.Append(", SEIBETU_O = '" & Trim(strSplitValue(9)) & "'")
                sql.Append(", TKIN_NO_O = '" & Trim(strSplitValue(32)).PadLeft(4, "0"c) & "'")
                sql.Append(", TSIT_NO_O = '" & Trim(strSplitValue(34)).PadLeft(3, "0"c) & "'")
                sql.Append(", HIMOKU_ID_O = '" & Trim(strSplitValue(10)).PadLeft(3, "0"c) & "'")
                sql.Append(", KAMOKU_O = '" & Trim(strSplitValue(36)).PadLeft(2, "0"c) & "'")
                sql.Append(", KOUZA_O = '" & Trim(strSplitValue(37)).PadLeft(7, "0"c) & "'")

                '名義人カナ
                'カスタマイズ
                If ConvKanaNGToKanaOK(strSplitValue(38), strRET) = True Then
                    sql.Append(", MEIGI_KNAME_O = '" & strRET & "'")
                End If

                'strSplitValue(39) = StrConv(Trim(strSplitValue(39)), VbStrConv.Wide)
                '名義人漢字・住所・電話番号
                'カスタマイズ
                sql.Append(", MEIGI_NNAME_O = '" & Trim(strSplitValue(39)) & "'")
                sql.Append(", KEIYAKU_NJYU_O = '" & strADDRESS & "'")
                sql.Append(", KEIYAKU_DENWA_O = '" & strTELNO & "'")

                sql.Append(", FURIKAE_O = '" & Trim(strSplitValue(40)) & "'")
                sql.Append(", KAIYAKU_FLG_O = '" & Trim(strSplitValue(43)) & "'")

                '請求月のみ費目金額変更する→2011/09/16　（カスタマイズ）請求月以外も変更する
                ''If strSEIKYUTUKI.Substring(4, 2) = Format(intTuki, "00") Then
                '請求方法・請求金額1～10
                Dim intCNT As Integer = 11
                For count = 1 To 10
                    sql.Append(", SEIKYU" & Format(count, "00") & "_O = '" & Trim(strSplitValue(intCNT)) & "'")

                    sql.Append(", KINGAKU" & Format(count, "00") & "_O = " & CLng(strSplitValue(intCNT + 1)))

                    intCNT += 2
                Next count
                ''End If

                '更新日付
                sql.Append(", KOUSIN_DATE_O = '" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")

                sql.Append(" WHERE GAKKOU_CODE_O = '" & strSplitValue(1).Trim.PadLeft(10, "0"c) & "'")
                sql.Append(" AND NENDO_O = '" & strSplitValue(2).Trim.PadLeft(4, "0"c) & "'")
                sql.Append(" AND TUUBAN_O = '" & Trim(strSplitValue(3)) & "'")
                sql.Append(" AND TUKI_NO_O = '" & Format(intTuki, "00") & "'")

                '存在チェック
                strSql = " SELECT * FROM SEITOMAST"
                strSql += " WHERE"
                strSql += " GAKKOU_CODE_O ='" & Trim(strSplitValue(1)).PadLeft(10, "0"c) & "'"
                strSql += " AND"
                strSql += " NENDO_O ='" & Trim(strSplitValue(2)) & "'"
                strSql += " AND"
                strSql += " TUUBAN_O =" & Trim(strSplitValue(3))
                strSql += " AND"
                strSql += " TUKI_NO_O ='" & Format(intTuki, "00") & "'"

                Dim oraReader As New MyOracleReader(MainDB)
                If oraReader.DataReader(strSql) = False Then
                    pError = pSeqNo & ",申請区分,更新対象の生徒が存在しません"
                    oraReader.Close()
                    Return False
                End If

                If MainDB.ExecuteNonQuery(sql) < 0 Then
                    pError = pSeqNo & ",移入,更新処理中にエラーが発生しました"
                    Return False
                End If
            Next intTuki

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "生徒SQL例外エラー", "失敗", ex.Message)
        End Try

    End Function
    Private Function PFUNC_DELETE_SEITO(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean
        '削除
        Dim strSplitValue() As String
        Dim sql As StringBuilder

        PFUNC_DELETE_SEITO = False
        Try

            strSplitValue = Split(pLineValue, ",")

            'SQL文作成
            sql = New StringBuilder(128)
            sql.Append("DELETE FROM SEITOMAST ")
            sql.Append(" WHERE GAKKOU_CODE_O = '" & strSplitValue(1).Trim.PadLeft(10, "0"c) & "'")
            sql.Append(" AND NENDO_O = '" & strSplitValue(2).Trim.PadLeft(4, "0"c) & "'")
            sql.Append(" AND TUUBAN_O = '" & Trim(strSplitValue(3)) & "'")

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                pError = pSeqNo & ",移入,削除処理中にエラーが発生しました"
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "生徒SQL例外エラー", "失敗", ex.Message)
        End Try

    End Function
#End Region

    Private Function Get_HIMOKU_INFO(ByVal gakkou_code As String, ByRef kin_no As String, ByRef sit_no As String, ByRef kouza As String) As Boolean
        '学校マスタ２の取得
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        sql.Append(" SELECT TKIN_NO_T,TSIT_NO_T,KOUZA_T FROM GAKMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_T ='" & gakkou_code & "'")

        If oraReader.DataReader(sql) = True Then
            kin_no = oraReader.GetString("TKIN_NO_T").ToString
            sit_no = oraReader.GetString("TSIT_NO_T").ToString
            kouza = oraReader.GetString("KOUZA_T").ToString
        Else
            oraReader.Close()
            Return False
        End If

        oraReader.Close()
        Return True

    End Function
    Private Function Get_SEITO_UPDATE_BEFORE(ByVal GAKCODE As String, ByVal NENDO As Integer, ByVal TUUBAN As Integer, ByVal strRec As String, ByRef strTBL As String) As Boolean
        '更新前情報の取得（生徒マスタ）
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)
        Dim strSplitValue() As String

        Dim 申請区分 As String = ""
        Dim 依頼人コード As String = ""
        Dim 登録年度 As String = ""
        Dim 通番 As String = ""
        Dim 学年 As String = ""
        Dim クラス As String = ""
        Dim 生徒番号 As String = ""
        Dim 生徒名カナ As String = ""
        Dim 生徒名漢字 As String = ""
        Dim 性別 As String = ""
        Dim 費目ＩＤ As String = ""
        Dim 請求方法フラグ１ As String = ""
        Dim 費目１ As String = ""
        Dim 請求方法フラグ２ As String = ""
        Dim 費目２ As String = ""
        Dim 請求方法フラグ３ As String = ""
        Dim 費目３ As String = ""
        Dim 請求方法フラグ４ As String = ""
        Dim 費目４ As String = ""
        Dim 請求方法フラグ５ As String = ""
        Dim 費目５ As String = ""
        Dim 請求方法フラグ６ As String = ""
        Dim 費目６ As String = ""
        Dim 請求方法フラグ７ As String = ""
        Dim 費目７ As String = ""
        Dim 請求方法フラグ８ As String = ""
        Dim 費目８ As String = ""
        Dim 請求方法フラグ９ As String = ""
        Dim 費目９ As String = ""
        Dim 請求方法フラグ１０ As String = ""
        Dim 費目１０ As String = ""
        Dim 合計金額 As String = ""
        Dim 金融機関コード As String = ""
        Dim 金融機関名 As String = ""
        Dim 支店コード As String = ""
        Dim 支店名 As String = ""
        Dim 科目 As String = ""
        Dim 口座番号 As String = ""
        Dim 口座名義カナ As String = ""
        Dim 口座名義漢字 As String = ""
        Dim 振替方法 As String = ""
        Dim 住所 As String = ""
        Dim 電話番号 As String = ""
        Dim 解約方法 As String = ""

        sql.Append(" SELECT * FROM SEITOMASTVIEW")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_O ='" & GAKCODE & "'")
        sql.Append(" AND")
        sql.Append(" NENDO_O = " & NENDO)
        sql.Append(" AND")
        sql.Append(" TUUBAN_O = " & TUUBAN)
        sql.Append(" AND")
        sql.Append(" TUKI_NO_O = '" & strSEIKYUTUKI.Substring(4, 2) & "'")

        If oraReader.DataReader(sql) = True Then
            strSplitValue = Split(strRec, ",")
            Dim intCnt As Integer
            For intCnt = 0 To UBound(strSplitValue)
                Select Case intCnt
                    Case 0
                        申請区分 = "2"
                    Case 1
                        依頼人コード = ""
                    Case 2
                        登録年度 = ""
                    Case 3
                        通番 = ""
                    Case 4
                        If oraReader.GetInt("GAKUNEN_CODE_O") = CInt(strSplitValue(intCnt)) Then
                            学年 = ""
                        Else
                            学年 = oraReader.GetInt("GAKUNEN_CODE_O").ToString
                        End If
                    Case 5
                        If oraReader.GetInt("CLASS_CODE_O") = CInt(strSplitValue(intCnt)) Then
                            クラス = ""
                        Else
                            クラス = oraReader.GetInt("CLASS_CODE_O").ToString
                        End If
                    Case 6
                        If oraReader.GetString("SEITO_NO_O").Trim = strSplitValue(intCnt) Then
                            生徒番号 = ""
                        Else
                            生徒番号 = oraReader.GetString("SEITO_NO_O").Trim
                        End If
                    Case 7
                        If oraReader.GetString("SEITO_KNAME_O").Trim = strSplitValue(intCnt) Then
                            生徒名カナ = ""
                        Else
                            生徒名カナ = oraReader.GetString("SEITO_KNAME_O").Trim
                        End If
                    Case 8
                        If oraReader.GetString("SEITO_NNAME_O").Trim = strSplitValue(intCnt) Then
                            生徒名漢字 = ""
                        Else
                            生徒名漢字 = oraReader.GetString("SEITO_NNAME_O").Trim
                        End If
                    Case 9
                        If oraReader.GetString("SEIBETU_O").Trim = strSplitValue(intCnt) Then
                            性別 = ""
                        Else
                            性別 = oraReader.GetString("SEIBETU_O").Trim
                        End If
                    Case 10
                        If oraReader.GetString("HIMOKU_ID_O").Trim = strSplitValue(intCnt) Then
                            費目ＩＤ = ""
                        Else
                            費目ＩＤ = oraReader.GetString("HIMOKU_ID_O").Trim
                        End If
                    Case 11
                        'カスタマイズ 個別金額表示させるため請求方法フラグ１は常に取得させる（2011/09/16）
                        請求方法フラグ１ = oraReader.GetString("SEIKYU01_O").Trim
                        'If oraReader.GetString("SEIKYU01_O").Trim = strSplitValue(intCnt) Then
                        '    請求方法フラグ１ = ""
                        'Else
                        '    請求方法フラグ１ = oraReader.GetString("SEIKYU01_O").Trim
                        'End If
                    Case 12
                        If oraReader.GetInt64("KINGAKU01_O") = CLng(strSplitValue(intCnt)) Then
                            費目１ = ""
                        Else
                            費目１ = oraReader.GetInt64("KINGAKU01_O").ToString
                        End If
                    Case 13
                        If oraReader.GetString("SEIKYU02_O").Trim = strSplitValue(intCnt) Then
                            請求方法フラグ２ = ""
                        Else
                            請求方法フラグ２ = oraReader.GetString("SEIKYU02_O").Trim
                        End If
                    Case 14
                        If oraReader.GetInt64("KINGAKU02_O") = CLng(strSplitValue(intCnt)) Then
                            費目２ = ""
                        Else
                            費目２ = oraReader.GetInt64("KINGAKU02_O").ToString
                        End If
                    Case 15
                        If oraReader.GetString("SEIKYU03_O").Trim = strSplitValue(intCnt) Then
                            請求方法フラグ３ = ""
                        Else
                            請求方法フラグ３ = oraReader.GetString("SEIKYU03_O").Trim
                        End If
                    Case 16
                        If oraReader.GetInt64("KINGAKU03_O") = CLng(strSplitValue(intCnt)) Then
                            費目３ = ""
                        Else
                            費目３ = oraReader.GetInt64("KINGAKU03_O").ToString
                        End If
                    Case 17
                        If oraReader.GetString("SEIKYU04_O").Trim = strSplitValue(intCnt) Then
                            請求方法フラグ４ = ""
                        Else
                            請求方法フラグ４ = oraReader.GetString("SEIKYU04_O").Trim
                        End If
                    Case 18
                        If oraReader.GetInt64("KINGAKU04_O") = CLng(strSplitValue(intCnt)) Then
                            費目４ = ""
                        Else
                            費目４ = oraReader.GetInt64("KINGAKU04_O").ToString
                        End If
                    Case 19
                        If oraReader.GetString("SEIKYU05_O").Trim = strSplitValue(intCnt) Then
                            請求方法フラグ５ = ""
                        Else
                            請求方法フラグ５ = oraReader.GetString("SEIKYU05_O").Trim
                        End If
                    Case 20
                        If oraReader.GetInt64("KINGAKU05_O") = CLng(strSplitValue(intCnt)) Then
                            費目５ = ""
                        Else
                            費目５ = oraReader.GetInt64("KINGAKU05_O").ToString
                        End If
                    Case 21
                        If oraReader.GetString("SEIKYU06_O").Trim = strSplitValue(intCnt) Then
                            請求方法フラグ６ = ""
                        Else
                            請求方法フラグ６ = oraReader.GetString("SEIKYU06_O").Trim
                        End If
                    Case 22
                        If oraReader.GetInt64("KINGAKU06_O") = CLng(strSplitValue(intCnt)) Then
                            費目６ = ""
                        Else
                            費目６ = oraReader.GetInt64("KINGAKU06_O").ToString
                        End If
                    Case 23
                        If oraReader.GetString("SEIKYU07_O").Trim = strSplitValue(intCnt) Then
                            請求方法フラグ７ = ""
                        Else
                            請求方法フラグ７ = oraReader.GetString("SEIKYU07_O").Trim
                        End If
                    Case 24
                        If oraReader.GetInt64("KINGAKU07_O") = CLng(strSplitValue(intCnt)) Then
                            費目７ = ""
                        Else
                            費目７ = oraReader.GetInt64("KINGAKU07_O").ToString
                        End If
                    Case 25
                        If oraReader.GetString("SEIKYU08_O").Trim = strSplitValue(intCnt) Then
                            請求方法フラグ８ = ""
                        Else
                            請求方法フラグ８ = oraReader.GetString("SEIKYU08_O").Trim
                        End If
                    Case 26
                        If oraReader.GetInt64("KINGAKU08_O") = CLng(strSplitValue(intCnt)) Then
                            費目８ = ""
                        Else
                            費目８ = oraReader.GetInt64("KINGAKU08_O").ToString
                        End If
                    Case 27
                        If oraReader.GetString("SEIKYU09_O").Trim = strSplitValue(intCnt) Then
                            請求方法フラグ９ = ""
                        Else
                            請求方法フラグ９ = oraReader.GetString("SEIKYU09_O").Trim
                        End If
                    Case 28
                        If oraReader.GetInt64("KINGAKU09_O") = CLng(strSplitValue(intCnt)) Then
                            費目９ = ""
                        Else
                            費目９ = oraReader.GetInt64("KINGAKU09_O").ToString
                        End If
                    Case 29
                        If oraReader.GetString("SEIKYU10_O").Trim = strSplitValue(intCnt) Then
                            請求方法フラグ１０ = ""
                        Else
                            請求方法フラグ１０ = oraReader.GetString("SEIKYU10_O").Trim
                        End If
                    Case 30
                        If oraReader.GetInt64("KINGAKU10_O") = CLng(strSplitValue(intCnt)) Then
                            費目１０ = ""
                        Else
                            費目１０ = oraReader.GetInt64("KINGAKU10_O").ToString
                        End If
                    Case 31
                        合計金額 = ""
                    Case 32
                        If oraReader.GetString("TKIN_NO_O").Trim = strSplitValue(intCnt) Then
                            金融機関コード = ""
                        Else
                            金融機関コード = oraReader.GetString("TKIN_NO_O").Trim
                        End If
                    Case 33
                        金融機関名 = ""
                    Case 34
                        If oraReader.GetString("TSIT_NO_O").Trim = strSplitValue(intCnt) Then
                            支店コード = ""
                        Else
                            支店コード = oraReader.GetString("TSIT_NO_O").Trim
                        End If
                    Case 35
                        支店名 = ""
                    Case 36
                        If oraReader.GetString("KAMOKU_O").Trim = strSplitValue(intCnt) Then
                            科目 = ""
                        Else
                            科目 = oraReader.GetString("KAMOKU_O").Trim
                        End If
                    Case 37
                        If oraReader.GetString("KOUZA_O").Trim = strSplitValue(intCnt) Then
                            口座番号 = ""
                        Else
                            口座番号 = oraReader.GetString("KOUZA_O").Trim
                        End If
                    Case 38
                        口座名義カナ = ""
                    Case 39
                        口座名義漢字 = ""
                    Case 40
                        If oraReader.GetString("FURIKAE_O").Trim = strSplitValue(intCnt) Then
                            振替方法 = ""
                        Else
                            振替方法 = oraReader.GetString("FURIKAE_O").Trim
                        End If
                    Case 41
                        住所 = ""
                    Case 42
                        電話番号 = ""
                    Case 43
                        If oraReader.GetString("KAIYAKU_FLG_O").Trim = strSplitValue(intCnt) Then
                            解約方法 = ""
                        Else
                            解約方法 = oraReader.GetString("KAIYAKU_FLG_O").Trim
                        End If
                End Select
            Next

            strTBL = _
            申請区分 & "," & _
            依頼人コード & "," & _
            登録年度 & "," & _
            通番 & "," & _
            学年 & "," & _
            クラス & "," & _
            生徒番号 & "," & _
            生徒名カナ & "," & _
            生徒名漢字 & "," & _
            性別 & "," & _
            費目ＩＤ & "," & _
            請求方法フラグ１ & "," & _
            費目１ & "," & _
            請求方法フラグ２ & "," & _
            費目２ & "," & _
            請求方法フラグ３ & "," & _
            費目３ & "," & _
            請求方法フラグ４ & "," & _
            費目４ & "," & _
            請求方法フラグ５ & "," & _
            費目５ & "," & _
            請求方法フラグ６ & "," & _
            費目６ & "," & _
            請求方法フラグ７ & "," & _
            費目７ & "," & _
            請求方法フラグ８ & "," & _
            費目８ & "," & _
            請求方法フラグ９ & "," & _
            費目９ & "," & _
            請求方法フラグ１０ & "," & _
            費目１０ & "," & _
            合計金額 & "," & _
            金融機関コード & "," & _
            金融機関名 & "," & _
            支店コード & "," & _
            支店名 & "," & _
            科目 & "," & _
            口座番号 & "," & _
            口座名義カナ & "," & _
            口座名義漢字 & "," & _
            振替方法 & "," & _
            住所 & "," & _
            電話番号 & "," & _
            解約方法

        Else
            oraReader.Close()
            Return False
        End If

        oraReader.Close()
        Return True

    End Function
    Private Function PFUNC_ERR_LOG_WRITE(ByVal pError As String) As Boolean

        Dim Obj_StreamWriter As StreamWriter

        PFUNC_ERR_LOG_WRITE = False

        Try
            'ログの書き込み
            Obj_StreamWriter = New StreamWriter(KobetuLogFileName, _
                                                True, _
                                                Encoding.GetEncoding("Shift_JIS"))
        Catch ex As Exception
            '
            Exit Function
        End Try

        Obj_StreamWriter.WriteLine(pError)
        Obj_StreamWriter.Flush()
        Obj_StreamWriter.Close()

        PFUNC_ERR_LOG_WRITE = True

    End Function
    Private Function PFUNC_PRINT_LOG_WRITE(ByVal strRec As String) As Boolean

        Dim Obj_StreamWriter As StreamWriter

        PFUNC_PRINT_LOG_WRITE = False

        Try
            'ログの書き込み
            Obj_StreamWriter = New StreamWriter(KobetuLogFileName2, _
                                                True, _
                                                Encoding.GetEncoding("Shift_JIS"))
        Catch ex As Exception
            '
            Exit Function
        End Try

        Obj_StreamWriter.WriteLine(strRec)
        Obj_StreamWriter.Flush()
        Obj_StreamWriter.Close()

        PFUNC_PRINT_LOG_WRITE = True

    End Function
    '20130405 maeda
    'カンマ区切りで文字列を戻す
    ''' <summary>
    ''' 更新前の旧学年クラス生徒番号情報を取得する
    ''' </summary>
    ''' <param name="GAKCODE">学校コード</param>
    ''' <param name="NENDO">年度</param>
    ''' <param name="TUUBAN">通番</param>
    ''' <param name="strTBL">旧学年クラス生徒番号情報</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function GetSeitoInfo(ByVal GAKCODE As String, ByVal NENDO As Integer, ByVal TUUBAN As Integer, ByRef strTBL As String) As Boolean

        Dim ret As Boolean = False

        Try
            Dim sql As New StringBuilder(128)
            Dim oraReader As New MyOracleReader(MainDB)

            '更新前情報の取得（生徒マスタ）
            sql.Append(" SELECT * FROM SEITOMASTVIEW")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_O ='" & GAKCODE & "'")
            sql.Append(" AND")
            sql.Append(" NENDO_O = " & NENDO)
            sql.Append(" AND")
            sql.Append(" TUUBAN_O = " & TUUBAN)
            sql.Append(" AND")
            sql.Append(" TUKI_NO_O = '" & strSEIKYUTUKI.Substring(4, 2) & "'")

            If oraReader.DataReader(sql) = True Then
                strTBL = ""
                strTBL &= oraReader.GetItem("GAKUNEN_CODE_O") '学年
                strTBL &= "," & oraReader.GetItem("CLASS_CODE_O").Trim.PadLeft(2, "0"c) 'クラス
                strTBL &= "," & oraReader.GetItem("SEITO_NO_O").Trim.PadLeft(7, "0"c) '生徒番号
            Else
                '新規分又はエラー分
                strTBL = "9,99,9999999"
            End If

            oraReader.Close()

            ret = True

        Catch ex As Exception

        End Try

        Return ret

    End Function
    '20130405 maeda
#End Region

#Region "イベント"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校名コンボボックス設定
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME) = True Then
            cmbGAKKOUNAME.Focus()
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGAKKOUNAME.SelectedIndexChanged

        'COMBOBOX選択時学校名,学校コード設定
        lblGAKKOU_NAME.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)

        '学年テキストボックスにFOCUS
        txtGAKKOU_CODE.Focus()

    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

End Class
