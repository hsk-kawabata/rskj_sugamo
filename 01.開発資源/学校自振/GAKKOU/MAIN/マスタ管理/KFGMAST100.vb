Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon

Public Class KFGMAST100

#Region " 共通変数定義 "
    Private Lng_Csv_Ok_Cnt As Long
    Private Lng_Csv_Ng_Cnt As Long
    Private Lng_Csv_SeqNo As Long

    Private Str_Csv_Himoku_Name(14) As String
#End Region

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST100", "費目マスタ処理画面")
    Private Const msgTitle As String = "費目マスタ処理画面(KFGMAST100)"
    Private MainDB As MyOracle

    Private KobetuLogFileName As String

    '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- START
    Private GAKKOU_CODE_LENGTH As Integer = 10
    '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- END
    '2017/02/22 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
    Private intMaxHimokuCnt As Integer = CInt(IIf(STR_HIMOKU_PTN = "1", 15, 10))
    '2017/02/22 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ END

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAST100_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- START
            '学校ツールの学校コード桁数を取得する
            Dim strLen As String = GetFSKJIni("GTOOL", "GAKKOU_CODE_LENGTH")
            If Not strLen.ToUpper = "ERR" AndAlso strLen <> "" AndAlso Not strLen = Nothing Then
                GAKKOU_CODE_LENGTH = CInt(strLen)
            End If
            '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- END

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

        Try
            MainDB = New MyOracle

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移入)開始", "成功", "")

            Cursor.Current = Cursors.WaitCursor()
            Dim strDIR As String

            '入力値チェック
            If Trim(txtGAKKOU_CODE.Text) = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If

            '確認メッセージ
            If MessageBox.Show(G_MSG0012I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If
            strDIR = CurDir()

            Lng_Csv_Ok_Cnt = 0
            Lng_Csv_Ng_Cnt = 0
            Lng_Csv_SeqNo = 1 '0→1に変更 2007/02/12
            'エラーログファイル削除 2007/02/12
            KobetuLogFileName = Path.Combine(STR_LOG_PATH, "HIMOKU" & txtGAKKOU_CODE.Text & ".log")
            If File.Exists(KobetuLogFileName) Then
                File.Delete(KobetuLogFileName)
            End If

            '入力先の指定
            'ファイルの選択ダイアログを表示
            Dim dlg As New Windows.Forms.OpenFileDialog

            dlg.ReadOnlyChecked = False

            dlg.CheckFileExists = True

            dlg.InitialDirectory = STR_CSV_PATH
            dlg.FileName = "HIMOKU" & Trim(txtGAKKOU_CODE.Text)

            dlg.Title = "CSVファイルの選択"
            dlg.Filter = "CSVファイル (*.csv)|*.csv"
            dlg.FilterIndex = 1

            Select Case dlg.ShowDialog()
                Case DialogResult.OK
                    'ダイアログOKボタン押下時
                    Console.WriteLine(dlg.FileName)
                Case Else
                    'ダイアログOKボタン以外押下時
                    ChDir(strDIR)
                    Exit Sub
            End Select

            MainDB.BeginTrans()

            'CSV入力＆マスタ登録
            If PFUNC_IMP_HIMOKU(dlg.FileName, Lng_Csv_Ok_Cnt, Lng_Csv_Ng_Cnt) = False Then
                MainDB.Rollback()
                ChDir(strDIR)
                btnEnd.Focus()
                Return
            End If

            If Lng_Csv_Ng_Cnt = 0 Then     '成功

                MainDB.Commit()

                '完了メッセージ
                MessageBox.Show(String.Format(G_MSG0015I, Lng_Csv_Ok_Cnt), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else

                MainDB.Rollback()

                'エラー時メッセージ
                MessageBox.Show(String.Format(G_MSG0046W, Lng_Csv_Ng_Cnt, KobetuLogFileName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            End If

            ChDir(strDIR)
            btnEnd.Focus()

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移入)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移入)終了", "成功", "")
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Sub
    Private Sub btnExp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExp.Click

        Try
            MainDB = New MyOracle

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出)開始", "成功", "")

            Cursor.Current = Cursors.WaitCursor()
            Dim strDIR As String

            '入力値チェック
            If Trim(txtGAKKOU_CODE.Text) = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text

            '確認メッセージ
            If MessageBox.Show(String.Format(MSG0015I, "移出"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            strDIR = CurDir()

            Lng_Csv_Ok_Cnt = 0
            Lng_Csv_Ng_Cnt = 0

            '名前をつけて保存ダイアログの表示
            Dim dlg As New Windows.Forms.SaveFileDialog

            dlg.InitialDirectory = STR_CSV_PATH
            dlg.FileName = "HIMOKU" & Trim(txtGAKKOU_CODE.Text)

            dlg.Title = "CSVファイルの作成先"
            dlg.Filter = "CSVファイル (.csv)|*.csv"
            dlg.FilterIndex = 1

            Select Case dlg.ShowDialog()
                Case DialogResult.OK
                    'ダイアログOKボタン押下時
                    Console.WriteLine(dlg.FileName)
                Case Else
                    'ダイアログOKボタン以外押下時
                    ChDir(strDIR)
                    Exit Sub
            End Select

            'CSV出力
            Lng_Csv_Ok_Cnt = PFUNC_EXP_HIMOKU(dlg.FileName)

            '完了メッセージ
            If Lng_Csv_Ok_Cnt < 0 Then
            ElseIf Lng_Csv_Ok_Cnt = 0 Then
                MessageBox.Show(G_MSG0042W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                MessageBox.Show(String.Format(G_MSG0016I, Lng_Csv_Ok_Cnt), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            ChDir(strDIR)
            btnEnd.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出)終了", "成功", "")
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Me.Close()

    End Sub
#End Region

#Region " Private Function "
    Private Function PFUNC_IMP_HIMOKU(ByVal pCsvPath As String, ByRef lngOkCnt As Long, ByRef lngErrCnt As Long) As Boolean

        Dim red As System.IO.StreamReader = Nothing

        Dim strHeadder As String
        Dim strReadLine As String

        Dim strErrorLine As String = ""

        Dim sql As StringBuilder

        lngOkCnt = 0
        lngErrCnt = 0
        PFUNC_IMP_HIMOKU = False

        Try
            red = New System.IO.StreamReader(pCsvPath, System.Text.Encoding.Default)


            'SQL文作成
            '画面に設定されている学校コードを条件に費目マスタを削除する
            sql = New StringBuilder(128)
            sql.Append(" DELETE  FROM HIMOMAST")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_H ='" & Trim(txtGAKKOU_CODE.Text) & "'")

            'トランザクションデータ操作開始
            If MainDB.ExecuteNonQuery(sql) < 0 Then
                MainLOG.Write("費目マスタ削除", "失敗", sql.ToString)
                Return False
            End If

            strHeadder = red.ReadLine.ToString()

            Do While Not red.Peek() = -1

                strReadLine = red.ReadLine.ToString

                '入力内容のチェック
                If PFUNC_CHECK_STR(Lng_Csv_SeqNo, strReadLine, strErrorLine) = True Then
                    'SQL文作成＆SQL処理実行
                    If PFUNC_INSERT_STR(Lng_Csv_SeqNo, strReadLine, strErrorLine) = True Then
                        lngOkCnt += 1
                    Else
                        lngErrCnt += 1

                        'ｴﾗｰﾛｸﾞの書き出し
                        If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                            Return False
                        End If
                        'SQLでエラーが発生した場合は処理中断　2007/02/12
                        red.Close()
                        Return False
                    End If
                Else
                    lngErrCnt += 1

                    'ｴﾗｰﾛｸﾞの書き出し
                    If PFUNC_ERR_LOG_WRITE(strErrorLine) = False Then
                        Return False
                    End If
                End If

                Lng_Csv_SeqNo += 1
            Loop

            red.Close()
            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_IMP_SEITO)例外エラー", "失敗", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function
    Private Function PFUNC_CHECK_STR(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intCnt As Integer
        Dim strSplitValue() As String
        Dim strRET As String = ""

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader


        PFUNC_CHECK_STR = False

        strSplitValue = Split(pLineValue, ",")

        '入力内容チェック
        For intCnt = 0 To UBound(strSplitValue)
            Select Case intCnt
                Case 0
                    '学校コード

                    '未入力チェック
                    If Trim(strSplitValue(intCnt)) = "" Then

                        pError = pSeqNo & ",学校コード,未入力です"

                        Exit Function
                    End If

                    '画面上で入力されているコード以外のコードの場合はエラー
                    If Trim(txtGAKKOU_CODE.Text) <> strSplitValue(intCnt).PadLeft(10, "0"c) Then

                        pError = pSeqNo & ",学校コード,画面で入力されている学校コード以外のものは登録できません"

                        Exit Function
                    End If

                    '学校マスタチェック
                    sql = New StringBuilder(128)
                    oraReader = New MyOracleReader(MainDB)
                    sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                    sql.Append(" WHERE GAKKOU_CODE_G = '" & strSplitValue(intCnt).PadLeft(10, "0"c) & "'")

                    If oraReader.DataReader(sql) = False Then

                        pError = pSeqNo & ",学校コード,学校マスタに登録されている学校コード以外のものは登録できません"
                        oraReader.Close()
                        Return False
                    End If
                    oraReader.Close()
                Case 1
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
                    sql.Append(" GAKKOU_CODE_G = '" & strSplitValue(0).PadLeft(10, "0"c) & "'")
                    sql.Append(" AND")
                    sql.Append(" GAKUNEN_CODE_G = " & Trim(strSplitValue(intCnt)))

                    If oraReader.DataReader(sql) = False Then
                        'MSG変更 2007/02/12
                        pError = pSeqNo & ",学年コード,学年コードが存在しません"
                        oraReader.Close()
                        Return False
                    End If
                    oraReader.Close()
                Case 2
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
                    '請求月

                    '費目IDが000(決済口座)の場合請求月が入力されていないためチェックは行わない
                    Select Case strSplitValue(2).Trim.PadLeft(3, "0"c)
                        Case Is <> "000"
                            If IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",請求月,未入力です"

                                Exit Function
                            Else
                                Select Case CLng(strSplitValue(intCnt))
                                    Case 1 To 12
                                    Case Else
                                        pError = pSeqNo & ",請求月,請求月は１～１２以外の数値を設定することはできません"

                                        Exit Function
                                End Select
                            End If
                    End Select
                Case 5, 12, 19, 26, 33, 40, 47, 54, 61, 68, 75, 82, 89, 96, 103
                    '費目名称

                Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69, 76, 83, 90, 97, 104
                    If Trim(strSplitValue(intCnt - 1)) <> "" Then
                        '金融機関コード
                        If Trim(strSplitValue(intCnt)) = "" Then

                            pError = pSeqNo & ",決済金融機関,未入力です"

                            Exit Function
                        End If

                        '桁数チェック 2007/02/12
                        If strSplitValue(intCnt).Trim.Length > 4 Then

                            pError = pSeqNo & ",決済金融機関,決済金融機関コードが４桁を超えています"

                            Exit Function
                        End If

                    End If


                Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70, 77, 84, 91, 98, 105

                    If Trim(strSplitValue(intCnt - 1)) <> "" Then
                        '支店コード
                        If Trim(strSplitValue(intCnt)) = "" Then

                            pError = pSeqNo & ",決済支店,未入力です"

                            Exit Function
                        Else

                            '桁数チェック 2007/02/12
                            If strSplitValue(intCnt).Trim.Length > 3 Then

                                pError = pSeqNo & ",決済支店,決済支店コードが３桁を超えています"

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
                Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71, 78, 85, 92, 99, 106

                    '決済科目 未入力のときエラーメッセージ出力 2007/02/12
                    If Trim(strSplitValue(intCnt - 1)) <> "" Then '支店コードが設定されている場合
                        If Trim(strSplitValue(intCnt)) = "" Then
                            pError = pSeqNo & ",決済科目,未入力です"
                            Exit Function
                        Else '2007/02/15
                            Select Case CInt(Trim(strSplitValue(intCnt)))
                                Case 1, 2
                                Case Else
                                    pError = pSeqNo & ",決済科目,決済科目は普通または当座のみ指定可能です"
                                    Exit Function
                            End Select

                        End If
                    End If

                Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72, 79, 86, 93, 100, 107

                    '口座番号
                    If Trim(strSplitValue(intCnt - 4)) <> "" Then
                        If Trim(strSplitValue(intCnt)) = "" Then

                            pError = pSeqNo & ",決済口座番号,未入力です"

                            Exit Function
                        End If
                        '決済口座番号桁数チェック 2007/02/12
                        If strSplitValue(intCnt).Trim.Length > 7 Then

                            pError = pSeqNo & ",決済口座番号,決済口座番号が７桁を超えています"

                            Exit Function
                        End If

                    End If
                Case 10, 17, 24, 31, 38, 45, 52, 59, 66, 73, 80, 87, 94, 101, 108

                    '2007/09/19　追加
                    '名義人名(ｶﾅ)

                    '未入力チェック
                    If Trim(strSplitValue(intCnt - 1)) <> "" Then
                        If Trim(strSplitValue(intCnt)) = "" Then
                            '===2008/04/17 未入力でもOKとする===============
                            'pError = pSeqNo & ",名義人名(ｶﾅ),未入力です"
                            'Exit Function
                            '===============================================
                        Else
                            '規定外文字チェック 2006/04/16
                            If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                                pError = pSeqNo & ",名義人名(ｶﾅ),規定外文字が有ります"
                                Exit Function
                            End If
                            '文字数チェック 2006/05/10
                            If Trim(strSplitValue(intCnt)).Length > 40 Then
                                pError = pSeqNo & ",名義人名(ｶﾅ),半角40文字以内で設定してください"
                                Exit Function
                            End If

                        End If
                    End If
                Case 11, 18, 25, 32, 39, 46, 53, 60, 67, 74, 81, 88, 95, 102, 109

                    '2007/09/19　追加
                    '費目金額
                    If IsNumeric(strSplitValue(intCnt)) = False Then
                        pError = pSeqNo & ",費目金額,費目金額に数値以外を設定することはできません"
                        Exit Function
                    End If

                    '費目IDの長さチェック 2007/02/12
                    If strSplitValue(intCnt).Trim.Length >= 8 Then
                        pError = pSeqNo & ",費目金額,費目金額が８桁を超えて設定されています"
                        Exit Function
                    End If
            End Select
        Next intCnt

        PFUNC_CHECK_STR = True

    End Function
    Private Function PFUNC_INSERT_STR(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intCnt As Integer
        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
        Dim strSplitValue(109) As String
        'Dim strSplitValue() As String
        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
        '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ START
        '読取用のワーク配列
        Dim strWKSplitValue() As String
        '2017/02/23 タスク）西野 ADD 標準版修正（費目数１０⇒１５）------------------------------------ END
        Dim strRET As String = ""

        Dim sql As New StringBuilder(128)

        PFUNC_INSERT_STR = False
        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
        strWKSplitValue = Split(pLineValue, ",")
        Array.Copy(strWKSplitValue, strSplitValue, strWKSplitValue.Length)
        'strSplitValue = Split(pLineValue, ",")
        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END

        'SQL文作成
        sql.Append(" INSERT INTO HIMOMAST values(")
        'For intCnt = 0 To UBound(strSplitValue)
        For intCnt = 0 To 109 '蒲郡信金向け　費目11～15の分まで固定でまわす
            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
            Select Case intCnt
                Case 0
                    '学校コード
                    sql.Append("'" & strSplitValue(intCnt).Trim.PadLeft(10, "0"c) & "'")
                Case 1
                    '学年
                    If Trim(strSplitValue(intCnt)) = "" Then
                        sql.Append(",0")
                    Else
                        sql.Append("," & strSplitValue(intCnt))
                    End If
                Case 2
                    '費目ID前ZERO詰め
                    sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")
                Case 4
                    '請求月
                    If Trim(strSplitValue(intCnt)) = "" Then
                        sql.Append(",'  '")
                    Else
                        sql.Append(",'" & Format(CInt(strSplitValue(intCnt)), "00") & "'")
                    End If
                Case 5, 12, 19, 26, 33, 40, 47, 54, 61, 68, 75, 82, 89, 96, 103
                    '費目名
                    strSplitValue(intCnt) = StrConv(strSplitValue(intCnt), VbStrConv.Wide)
                    If strSplitValue(intCnt).Trim.Length > 10 Then
                        sql.Append(",'" & strSplitValue(intCnt).Substring(0, 10) & "'")
                    Else
                        sql.Append(",'" & strSplitValue(intCnt) & "'")
                    End If
                Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69, 76, 83, 90, 97, 104
                    '決済金融機関
                    If Trim(strSplitValue(intCnt - 1)) <> "" Then
                        sql.Append(",'" & strSplitValue(intCnt).PadLeft(4, "0"c) & "'")
                    Else
                        sql.Append(",''")
                    End If
                Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70, 77, 84, 91, 98, 105
                    '決済金融機関支店
                    If Trim(strSplitValue(intCnt - 2)) <> "" Then
                        sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(3, "0"c) & "'")
                    Else
                        sql.Append(",''")
                    End If
                Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71, 78, 85, 92, 99, 106
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
                Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72, 79, 86, 93, 100, 107
                    '口座番号
                    If Trim(strSplitValue(intCnt - 4)) <> "" Then
                        sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(7, "0"c) & "'")
                    Else
                        sql.Append(",''")
                    End If
                Case 10, 17, 24, 31, 38, 45, 52, 59, 66, 73, 80, 87, 94, 101, 108
                    '名義人カナ
                    '2007/09/19　ちっちゃい文字等を変換し設定
                    If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = True Then
                        sql.Append(",'" & strRET & "'")
                    Else
                        sql.Append(",'" & Space(40) & "'")
                    End If
                Case 11, 18, 25, 32, 39, 46, 53, 60, 67, 74, 81, 88, 95, 102, 109
                    '金額
                    If IsNumeric(strSplitValue(intCnt)) = True Then
                        sql.Append("," & strSplitValue(intCnt))
                    Else
                        sql.Append(",0")
                    End If

                Case Else
                    sql.Append(",'" & strSplitValue(intCnt) & "'")
            End Select
            'Select Case intCnt
            '    Case 0
            '        sql.Append("'" & strSplitValue(intCnt).Trim.PadLeft(10, "0"c) & "'")
            '        'Case 1, 11, 18, 25, 32, 39, 46, 53, 60, 67, 74, 81, 88, 95, 102, 109
            '        '蒲郡信金向け 学年＆費目1～10までの金額
            '    Case 1, 11, 18, 25, 32, 39, 46, 53, 60, 67, 74
            '        If Trim(strSplitValue(intCnt)) = "" Then
            '            sql.Append(",0")
            '        Else
            '            sql.Append("," & strSplitValue(intCnt))
            '        End If
            '        '蒲郡信金向け　費目11～15(未設定なので固定)
            '    Case 81, 88, 95, 102, 109
            '        sql.Append(",0")
            '    Case 2
            '        '費目ID前ZERO詰め
            '        sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")
            '    Case 4
            '        '請求月
            '        If Trim(strSplitValue(intCnt)) = "" Then
            '            sql.Append(",'  '")
            '        Else
            '            sql.Append(",'" & Format(CInt(strSplitValue(intCnt)), "00") & "'")
            '        End If
            '        'Case 5, 12, 19, 26, 33, 40, 47, 54, 61, 68, 75, 82, 89, 96, 103
            '        '蒲郡信金向け 費目1～10まで
            '    Case 5, 12, 19, 26, 33, 40, 47, 54, 61, 68
            '        '2007/02/15 費目名
            '        strSplitValue(intCnt) = StrConv(strSplitValue(intCnt), VbStrConv.Wide)
            '        If strSplitValue(intCnt).Trim.Length > 10 Then
            '            sql.Append(",'" & strSplitValue(intCnt).Substring(0, 10) & "'")
            '        Else
            '            sql.Append(",'" & strSplitValue(intCnt) & "'")
            '        End If
            '        '蒲郡信金向け　費目11～15(未設定なので固定)
            '    Case 75, 82, 89, 96, 103
            '        sql.Append(",''")
            '        'Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69, 76, 83, 90, 97, 104
            '        '蒲郡信金向け 費目1～10まで
            '    Case 6, 13, 20, 27, 34, 41, 48, 55, 62, 69

            '        '決済金融機関
            '        If Trim(strSplitValue(intCnt - 1)) <> "" Then
            '            sql.Append(",'" & strSplitValue(intCnt).PadLeft(4, "0"c) & "'")
            '        Else
            '            sql.Append(",''")
            '        End If
            '        '蒲郡信金向け　費目11～15(未設定なので固定)
            '    Case 76, 83, 90, 97, 104
            '        sql.Append(",''")
            '        'Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70, 77, 84, 91, 98, 105
            '        '蒲郡信金向け 費目1～10まで
            '    Case 7, 14, 21, 28, 35, 42, 49, 56, 63, 70
            '        '決済金融機関支店
            '        If Trim(strSplitValue(intCnt - 2)) <> "" Then
            '            sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(3, "0"c) & "'")
            '        Else
            '            sql.Append(",''")
            '        End If
            '        '蒲郡信金向け　費目11～15(未設定なので固定)
            '    Case 77, 84, 91, 98, 105
            '        sql.Append(",''")
            '        'Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71, 78, 85, 92, 99, 106
            '        '蒲郡信金向け 費目1～10まで
            '    Case 8, 15, 22, 29, 36, 43, 50, 57, 64, 71
            '        '決済科目
            '        If Trim(strSplitValue(intCnt - 3)) <> "" Then
            '            If Trim(strSplitValue(intCnt)) = "" Then
            '                sql.Append(",'01'")
            '            Else
            '                Select Case Trim(strSplitValue(intCnt)).PadLeft(2, "0"c)
            '                    Case "01", "02", "05", "37"
            '                        sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(2, "0"c) & "'")
            '                    Case Else
            '                        sql.Append(",'01'")
            '                End Select
            '            End If
            '        Else
            '            sql.Append(",''")
            '        End If
            '        '蒲郡信金向け　費目11～15(未設定なので固定)
            '    Case 78, 85, 92, 99, 106
            '        sql.Append(",''")
            '        'Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72, 79, 86, 93, 100, 107
            '        '蒲郡信金向け 費目1～10まで
            '    Case 9, 16, 23, 30, 37, 44, 51, 58, 65, 72
            '        '口座番号
            '        If Trim(strSplitValue(intCnt - 4)) <> "" Then
            '            sql.Append(",'" & strSplitValue(intCnt).Trim.PadLeft(7, "0"c) & "'")
            '        Else
            '            sql.Append(",''")
            '        End If
            '        '蒲郡信金向け　費目11～15(未設定なので固定)
            '    Case 79, 86, 93, 100, 107
            '        sql.Append(",''")

            '    Case 10, 17, 24, 31, 38, 45, 52, 59, 66, 73
            '        '2007/09/19　ちっちゃい文字等を変換し設定
            '        If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = True Then
            '            sql.Append(",'" & strRET & "'")
            '        Else
            '            sql.Append(",'" & Space(40) & "'")
            '        End If
            '    Case 11, 18, 25, 32, 39, 46, 53, 60, 67, 74
            '        If IsNumeric(strSplitValue(intCnt)) = True Then
            '            sql.Append(CInt(strSplitValue(intCnt)))
            '        Else
            '            sql.Append(",''")
            '        End If

            '        '蒲郡信金向け　費目11～15の名義人(未設定なので固定)
            '    Case 80, 87, 94, 101, 108
            '        sql.Append(",''")

            '    Case Else
            '        sql.Append(",'" & strSplitValue(intCnt) & "'")
            'End Select
            '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
        Next intCnt

        sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
        sql.Append(",'00000000'")
        sql.Append(",'" & Space(50) & "'")
        sql.Append(",'" & Space(50) & "'")
        sql.Append(",'" & Space(50) & "'")
        sql.Append(",'" & Space(50) & "'")
        sql.Append(",'" & Space(50) & "'")
        sql.Append(")")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            pError = pSeqNo & ",移入,移入処理中にエラーが発生しました"
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_EXP_HIMOKU(ByVal pCsvPath As String) As Long

        Dim intCol As Integer
        Dim lngLine As Long = 0
        Dim strLine As String
        Dim wrt As System.IO.StreamWriter = Nothing
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        PFUNC_EXP_HIMOKU = 0

        Try
            sql.Append(" SELECT * FROM HIMOMAST")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_H ='" & Trim(txtGAKKOU_CODE.Text) & "'")
            sql.Append(" ORDER BY GAKKOU_CODE_H ASC, GAKUNEN_CODE_H ASC, HIMOKU_ID_H ASC, TUKI_NO_H ASC")

            If oraReader.DataReader(sql) = True Then

                lngLine = 0

                '項目名見出し設定
                strLine = "学校コード,学年,費目ID,費目名,請求月"
                strLine += ",費目1名,費目1金融機関コード,費目1支店コード,費目1科目,費目1口座番号,費目1名義人カナ,費目1金額"
                strLine += ",費目2名,費目2金融機関コード,費目2支店コード,費目2科目,費目2口座番号,費目2名義人カナ,費目2金額"
                strLine += ",費目3名,費目3金融機関コード,費目3支店コード,費目3科目,費目3口座番号,費目3名義人カナ,費目3金額"
                strLine += ",費目4名,費目4金融機関コード,費目4支店コード,費目4科目,費目4口座番号,費目4名義人カナ,費目4金額"
                strLine += ",費目5名,費目5金融機関コード,費目5支店コード,費目5科目,費目5口座番号,費目5名義人カナ,費目5金額"
                strLine += ",費目6名,費目6金融機関コード,費目6支店コード,費目6科目,費目6口座番号,費目6名義人カナ,費目6金額"
                strLine += ",費目7名,費目7金融機関コード,費目7支店コード,費目7科目,費目7口座番号,費目7名義人カナ,費目7金額"
                strLine += ",費目8名,費目8金融機関コード,費目8支店コード,費目8科目,費目8口座番号,費目8名義人カナ,費目8金額"
                strLine += ",費目9名,費目9金融機関コード,費目9支店コード,費目9科目,費目9口座番号,費目9名義人カナ,費目9金額"
                strLine += ",費目10名,費目10金融機関コード,費目10支店コード,費目10科目,費目10口座番号,費目10名義人カナ,費目10金額"
                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                If STR_HIMOKU_PTN = "1" Then
                    strLine += ",費目11名,費目11金融機関コード,費目11支店コード,費目11科目,費目11口座番号,費目11名義人カナ,費目11金額"
                    strLine += ",費目12名,費目12金融機関コード,費目12支店コード,費目12科目,費目12口座番号,費目12名義人カナ,費目12金額"
                    strLine += ",費目13名,費目13金融機関コード,費目13支店コード,費目13科目,費目13口座番号,費目13名義人カナ,費目13金額"
                    strLine += ",費目14名,費目14金融機関コード,費目14支店コード,費目14科目,費目14口座番号,費目14名義人カナ,費目14金額"
                    strLine += ",費目15名,費目15金融機関コード,費目15支店コード,費目15科目,費目15口座番号,費目15名義人カナ,費目15金額"
                End If
                '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                '2007/04/06　費目は１０まで
                'strLine += ",費目11名,費目11金融機関コード,費目11支店コード,費目11科目,費目11口座番号,費目11名義人カナ,費目11金額"
                'strLine += ",費目12名,費目12金融機関コード,費目12支店コード,費目12科目,費目12口座番号,費目12名義人カナ,費目12金額"
                'strLine += ",費目13名,費目13金融機関コード,費目13支店コード,費目13科目,費目13口座番号,費目13名義人カナ,費目13金額"
                'strLine += ",費目14名,費目14金融機関コード,費目14支店コード,費目14科目,費目14口座番号,費目14名義人カナ,費目14金額"
                'strLine += ",費目15名,費目15金融機関コード,費目15支店コード,費目15科目,費目15口座番号,費目15名義人カナ,費目15金額"
                strLine += vbCrLf

                Do Until oraReader.EOF

                    '学校コード
                    If IsDBNull(oraReader.GetString("GAKKOU_CODE_H")) = False Then
                        '2016/11/04 タスク）西野 CHG 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- START
                        strLine += Trim(Microsoft.VisualBasic.Right(oraReader.GetString("GAKKOU_CODE_H"), GAKKOU_CODE_LENGTH))
                        'strLine += Trim(CStr(oraReader.GetString("GAKKOU_CODE_H")))
                        '2016/11/04 タスク）西野 CHG 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- END
                    End If

                    '学年コード
                    If IsDBNull(oraReader.GetString("GAKUNEN_CODE_H")) = False Then
                        strLine += "," & Trim(CStr(oraReader.GetString("GAKUNEN_CODE_H")))
                    Else
                        strLine += ","
                    End If
                    '費目ID
                    If IsDBNull(oraReader.GetString("HIMOKU_ID_H")) = False Then
                        strLine += "," & Trim(CStr(oraReader.GetString("HIMOKU_ID_H")))
                    Else
                        strLine += ","
                    End If
                    '費目名
                    If IsDBNull(oraReader.GetString("HIMOKU_ID_NAME_H")) = False Then
                        strLine += "," & Trim(CStr(oraReader.GetString("HIMOKU_ID_NAME_H")))
                    Else
                        strLine += ","
                    End If
                    '請求月
                    If IsDBNull(oraReader.GetString("TUKI_NO_H")) = False Then
                        strLine += "," & Trim(CStr(oraReader.GetString("TUKI_NO_H")))
                    Else
                        strLine += ","
                    End If

                    '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ START
                    For intCol = 1 To intMaxHimokuCnt
                        'For intCol = 1 To 10 '2007/04/06　費目は１０まで
                        '2017/02/23 タスク）西野 CHG 標準版修正（費目数１０⇒１５）------------------------------------ END
                        '費目名
                        If IsDBNull(oraReader.GetString("HIMOKU_NAME" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & Trim(CStr(oraReader.GetString("HIMOKU_NAME" & Format(intCol, "00") & "_H")))
                        Else
                            strLine += ","
                        End If

                        '金融機関コード
                        If IsDBNull(oraReader.GetString("KESSAI_KIN_CODE" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & CStr(oraReader.GetString("KESSAI_KIN_CODE" & Format(intCol, "00") & "_H"))
                        Else
                            strLine += ","
                        End If

                        '支店コード
                        If IsDBNull(oraReader.GetString("KESSAI_TENPO" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & CStr(oraReader.GetString("KESSAI_TENPO" & Format(intCol, "00") & "_H"))
                        Else
                            strLine += ","
                        End If

                        '科目
                        If IsDBNull(oraReader.GetString("KESSAI_KAMOKU" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & CStr(oraReader.GetString("KESSAI_KAMOKU" & Format(intCol, "00") & "_H"))
                        Else
                            strLine += ","
                        End If

                        '口座番号
                        If IsDBNull(oraReader.GetString("KESSAI_KOUZA" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & CStr(oraReader.GetString("KESSAI_KOUZA" & Format(intCol, "00") & "_H"))
                        Else
                            strLine += ","
                        End If

                        '口座名義人(ｶﾅ)
                        If IsDBNull(oraReader.GetString("KESSAI_MEIGI" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & Trim(CStr(oraReader.GetString("KESSAI_MEIGI" & Format(intCol, "00") & "_H")))
                        Else
                            strLine += ","
                        End If

                        '金額
                        If IsDBNull(oraReader.GetString("HIMOKU_KINGAKU" & Format(intCol, "00") & "_H")) = False Then
                            strLine += "," & CStr(oraReader.GetString("HIMOKU_KINGAKU" & Format(intCol, "00") & "_H"))
                        Else
                            strLine += ","
                        End If
                    Next intCol

                    strLine += vbCrLf

                    lngLine += 1

                    oraReader.NextRead()

                Loop

            Else
                oraReader.Close()
                Return 0
            End If
            oraReader.Close()

            wrt = New System.IO.StreamWriter(pCsvPath, False, System.Text.Encoding.Default)

            wrt.Write(strLine)

            wrt.Close()

            Return lngLine

        Catch ex As Exception
            '2014/01/06 saitou 標準版 メッセージ定数化 ADD -------------------------------------------------->>>>
            '例外発生時はメッセージ出力
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            '2014/01/06 saitou 標準版 メッセージ定数化 ADD --------------------------------------------------<<<<
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "ＣＳＶ作成", "失敗", ex.Message)
            Return -1       '失敗
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not wrt Is Nothing Then wrt.Close()
        End Try

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

#End Region

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
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        If Me.txtGAKKOU_CODE.Text.Trim <> "" Then

            '学校名検索
            sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If oraReader.DataReader(sql) = False Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                lblGAKKOU_NAME.Text = ""
                oraReader.Close()
                Exit Sub
            Else
                lblGAKKOU_NAME.Text = oraReader.GetString("GAKKOU_NNAME_G")
            End If

            oraReader.Close()
        End If
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
