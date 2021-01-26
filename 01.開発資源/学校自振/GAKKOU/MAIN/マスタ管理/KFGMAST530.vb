Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Imports CAstReports

Public Class KFGMAST530

#Region " 共通変数定義 "

    Private Lng_Seito_Ok_Cnt As Long
    Private Lng_Gak_Ok_Cnt As Long
    Private Lng_Kin_Ok_Cnt As Long

    Private Lng_Seito_Ng_Cnt As Long
    Private Lng_Gak_Ng_Cnt As Long

    'Private Lng_Csv_Ok_Cnt As String = 0
    'Private Lng_Csv_Ng_Cnt As String = 0

    Private Lng_Seito_SeqNo As Long
    Private Lng_Gak_SeqNo As Long

    Public STR帳票ソート順 As String = "0"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST530", "学校ツール連携画面")
    Private ToolLOG As New CASTCommon.BatchLOG("KFGMAST530/ERROR", "データ取込エラーログ")

    Private Const msgTitle As String = "学校ツール連携画面(KFGMAST530)"
    Private MainDB As MyOracle

    Private KobetuLogFileName As String 'エラーログ
    Private KobetuLogFileName2 As String '読込ログ

    Private err_code As Integer = 0
    Private err_Filename As String = ""

    '学校ツール作成先、取込先PATH取得
    Private GAKTOOL_IN_PATH As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "SAVE_PATH")
    Private GAKTOOL_INBK_PATH As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "CSVBK")

    Private GAKTOOL_MOD_FILE As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "TORIKOMI_CSV_MOD")
    Private GAKTOOL_GAK_FILE As String = GFUNC_INI_READ("KINGAKUKAKUNIN", "TORIKOMI_CSV_GAKKO")

    Private GAKTOOL_LOG_PATH As String = GFUNC_INI_READ("COMMON", "LOG")

    Private EXP_NEND As Integer
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAST530_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            If MessageBox.Show("ＣＳＶからのインポートを開始します", _
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

                    Lng_Seito_Ok_Cnt = 0
                    Lng_Seito_Ng_Cnt = 0
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


                    '*** 振替入金明細レコード ***
                    'CSV入力＆マスタ登録
                    If PFUNC_IMP_SEITO(Gakkou_Code, GAKTOOL_IN_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv", Lng_Seito_Ok_Cnt, Lng_Seito_Ng_Cnt) = False Then
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
                        If File.Exists(GAKTOOL_IN_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv") Then
                            FileCopy(GAKTOOL_IN_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv", GAKTOOL_INBK_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv")
                            File.Delete(GAKTOOL_IN_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv")
                        End If
                    End If

                    aoraReader.NextRead()
                    OK_ken += Lng_Seito_Ok_Cnt
                    NG_ken += Lng_Seito_Ng_Cnt
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
                MessageBox.Show("インポートを終了しました。" & vbCrLf & "入金明細:" & Lng_Seito_Ok_Cnt & "件のデータを登録しました。" & _
                                 vbCrLf & vbCrLf & "KFGMAIN070 生徒明細入力/KFGPRNT110 生徒明細入力チェックリスト印刷で入力内容を確認してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            Else

                MainDB.Rollback()

                'ファイルバックアップ戻し（学校データ）
                If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv") Then
                    FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                    File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_GAK_FILE & Gakkou_Code & ".csv")
                End If
                'ファイルバックアップ戻し（入金明細データ）
                If File.Exists(GAKTOOL_INBK_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv") Then
                    FileCopy(GAKTOOL_INBK_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv", GAKTOOL_IN_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv")
                    File.Delete(GAKTOOL_INBK_PATH & GAKTOOL_MOD_FILE & Gakkou_Code & ".csv")
                End If


                'エラー時メッセージ
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
                    sql.Append("SELECT ENTRI_FLG_S FROM G_SCHMAST ")
                    sql.Append(" WHERE")
                    sql.Append(" GAKKOU_CODE_S  = '" & gakkou_code_H & "' ")
                    sql.Append(" AND")
                    sql.Append(" SCH_KBN_S  = '2' ")
                    sql.Append(" AND")
                    sql.Append(" FURI_KBN_S  = '2' ")
                    sql.Append(" AND")
                    sql.Append(" FURI_DATE_S = '" & strGET_furi_date & "' ")

                    If oraReader.DataReader(sql) = False Then
                        MessageBox.Show("振替入金スケジュールが存在しないので処理できません。" & vbCrLf & "データ振替日:" & strGET_furi_date, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "該当スケジュールなし データ振替日:" & strGET_furi_date, "失敗", "")
                        oraReader.Close()
                        Return False
                    Else
                        If oraReader.GetItem("ENTRI_FLG_S").ToString = "1" Then
                            oraReader.Close()
                        Else
                            MessageBox.Show("KFGMAIN060 生徒明細作成を行ってください。" & vbCrLf & "データ振替日:" & strGET_furi_date, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                            oraReader.Close()
                            Return False
                        End If
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

    Private Function PFUNC_IMP_SEITO(ByVal gakkou_code_S As String, ByVal pCsvPath As String, ByRef lngOkCnt As Long, ByRef lngErrCnt As Long) As Boolean

        Dim red As System.IO.StreamReader = Nothing

        Dim strReadLine As String
        'Dim Shinsei_KBN As String   '申請区分
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
                If PFUNC_CHECK_SEITO(gakkou_code_S, Lng_Seito_SeqNo, strReadLine, strErrorLine, NENDO, TUUBAN) = True Then
                    If PFUNC_UPDATE_SEITO(Lng_Seito_SeqNo, strReadLine, strErrorLine) = True Then
                        lngOkCnt += 1
                    Else
                        lngErrCnt += 1
                        blnSQL = False

                        'ｴﾗｰﾛｸﾞの書き出し
                        ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_UPDATE_SEITO", "失敗", Replace(strErrorLine, ",", "/"))

                        'SQLでエラーが発生した場合は処理中断
                        red.Close()
                        Exit Do
                    End If
                    
                Else
                    lngErrCnt += 1

                    'ｴﾗｰﾛｸﾞの書き出し
                    ToolLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "PFUNC_CHECK_SEITO", "失敗", Replace(strErrorLine, ",", "/"))

                End If

                Lng_Seito_SeqNo += 1
            Loop

            red.Close()
            Return True

        Catch ex As FileNotFoundException
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                MessageBox.Show("ファイルが見つかりません。" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替入金明細ファイルなし", "失敗", ex.Message)
            Else
                Return True
            End If
        Catch ex As System.IO.IOException
            MessageBox.Show("ファイルが使用中です。" & vbCrLf & pCsvPath, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替入金明細ファイルアクセス", "失敗", ex.Message)
            err_code = 1
            err_Filename = pCsvPath
            Return False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替入金明細チェック例外エラー", "失敗", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function

    Private Function PFUNC_CHECK_SEITO(ByVal gakkou_code_C As String, ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String, ByRef NENDO As Integer, ByRef TUUBAN As Integer) As Boolean

        Dim intCnt As Integer
        Dim intCnt2 As Integer
        Dim strSplitValue() As String
        Dim strRET As String = ""

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader


        PFUNC_CHECK_SEITO = False
        Try

            strSplitValue = Split(pLineValue, ",")

            '入力内容チェック
            If strSplitValue.Length = 17 Then
                For intCnt = 0 To UBound(strSplitValue)
                    Select Case intCnt
                        Case 0
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

                        Case 1
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

                        Case 2
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

                        Case 3
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
                            sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(0)).PadLeft(10, "0"c) & "'")
                            sql.Append(" AND")
                            sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(intCnt)))

                            If oraReader.DataReader(sql) = False Then
                                pError = pSeqNo & ",学年コード,学年コードが学校マスタに登録されていません"
                                oraReader.Close()
                                Return False
                            End If
                            oraReader.Close()
                        Case 4
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
                                sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(0)).PadLeft(10, "0"c) & "'")
                                sql.Append(" AND")
                                sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(3)))
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
                        Case 5
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

                        Case 6
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
                        Case 7 '生徒名漢字　長さチェック
                            If Trim(strSplitValue(intCnt)) <> "" Then
                                '２バイト変換
                                strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Wide)
                                '文字数チェック
                                If Trim(strSplitValue(intCnt)).Length > 25 Then
                                    pError = pSeqNo & ",生徒氏名(漢字),全角25文字以内で設定してください"
                                    Exit Function
                                End If
                            End If
                        Case 8
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
                        Case 9
                            '金額
                            '未入力チェック
                            If IsNumeric(strSplitValue(intCnt)) = False Then
                                pError = pSeqNo & ",費目金額" & Format((intCnt - 10) \ 2, "00") & ",費目金額に数値以外を設定することはできません"
                                Exit Function
                            End If

                            If strSplitValue(intCnt).Trim.Length > 12 Then
                                pError = pSeqNo & ",費目金額" & Format((intCnt - 10) \ 2, "00") & ",費目金額が１２桁を超えて設定されています"
                                Exit Function
                            End If

                        Case 10
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

                        Case 11
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

                        Case 12
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

                        Case 13
                            '口座番号
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",取扱口座番号,未入力です"

                                Exit Function
                            End If

                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",取扱口座番号,口座番号に数値以外の値が設定されています"

                                Exit Function
                            End If

                            '桁数チェック
                            If strSplitValue(intCnt).Trim.Length > 7 Then

                                pError = pSeqNo & ",取扱口座番号,口座番号が７桁を超えて設定されています"

                                Exit Function
                            End If


                        Case 14
                            '名義人(ｶﾅ)

                        Case 15
                            '名義人漢字


                        Case 16
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
                    End Select
                Next intCnt
            Else
                pError = pSeqNo & ",項目数,振替入金明細データの項目数に不備があります"
                'pError = pSeqNo & ",生徒情報データの項目数に不備があります"
                Exit Function
            End If

            PFUNC_CHECK_SEITO = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替入金明細データチェック例外エラー", "失敗", intCnt & ex.Message)
            Return False
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
            sql = New StringBuilder(128)
            strSql = ""
            Dim count As Integer = 0
            Dim TUKI_FURIKIN As Long = 0
            sql.Append(" UPDATE G_ENTMAST1 SET ")
            sql.Append(" FURIKIN_E = " & Trim(strSplitValue(9)))
            sql.Append(", KOUSIN_DATE_E = '" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
            sql.Append(" WHERE GAKKOU_CODE_E = '" & strSplitValue(0).Trim.PadLeft(10, "0"c) & "'")
            sql.Append(" AND NENDO_E = '" & strSplitValue(1).Trim.PadLeft(4, "0"c) & "'")
            sql.Append(" AND TUUBAN_E = " & Trim(strSplitValue(2)))

            Dim RET As Integer = MainDB.ExecuteNonQuery(sql)

            If RET < 0 Then
                pError = pSeqNo & ",移入,更新処理中にエラーが発生しました"
                Return False
            ElseIf RET = 0 Then
                pError = pSeqNo & ",移入,更新対象がありません"
                Return False
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "振替入金明細SQL例外エラー", "失敗", ex.Message)
        End Try

    End Function
#End Region

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
