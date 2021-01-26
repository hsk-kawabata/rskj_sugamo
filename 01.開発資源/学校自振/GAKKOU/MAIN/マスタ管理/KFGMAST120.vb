Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon

Public Class KFGMAST120

#Region " 共通変数定義 "
    Private Lng_Csv_Ok_Cnt As Long
    Private Lng_Csv_Ng_Cnt As Long
    Private Lng_Csv_SeqNo As Long
    Private strSplitValue() As String
    Public STR帳票ソート順 As String = "0"

#End Region

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST120", "新入生マスタ処理画面")
    Private Const msgTitle As String = "新入生マスタ処理画面(KFGMAST120)"
    Private MainDB As MyOracle

    Private KobetuLogFileName As String

    '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- START
    Private GAKKOU_CODE_LENGTH As Integer = 10
    '2016/11/04 タスク）西野 ADD 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- END

#Region " Form Load "
    Private Sub KFGMAST120_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            If MessageBox.Show(G_MSG0014I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            strDIR = CurDir()

            Lng_Csv_Ok_Cnt = 0
            Lng_Csv_Ng_Cnt = 0
            Lng_Csv_SeqNo = 1 '0→1に変更 2006/04/16
            'エラーログファイル削除 2006/04/16
            KobetuLogFileName = Path.Combine(STR_LOG_PATH, "SINNYU" & txtGAKKOU_CODE.Text & ".log")
            If File.Exists(KobetuLogFileName) Then
                File.Delete(KobetuLogFileName)
            End If

            '入力先の指定
            'ファイルの選択ダイアログを表示
            Dim dlg As New Windows.Forms.OpenFileDialog

            dlg.ReadOnlyChecked = False

            dlg.CheckFileExists = True

            dlg.InitialDirectory = STR_CSV_PATH
            dlg.FileName = "SINNYU" & Trim(txtGAKKOU_CODE.Text)

            dlg.Title = "CSVファイルの選択"
            dlg.Filter = "CSVファイル (*.csv)|*.csv"
            dlg.FilterIndex = 1

            Select Case (dlg.ShowDialog())
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
            If PFUNC_IMP_SINNYU(dlg.FileName, Lng_Csv_Ok_Cnt, Lng_Csv_Ng_Cnt) = False Then
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
            Else
                '帳票ソート順取得 2007/02/15
                If PFUNC_GAKMAST2_GET() = False Then
                    Call GSUB_LOG(0, "学校マスタに登録されていません")
                    txtGAKKOU_CODE.Focus()
                    Return
                End If
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
            dlg.FileName = "SINNYU" & Trim(txtGAKKOU_CODE.Text)

            dlg.Title = "CSVファイルの作成先"
            dlg.Filter = "CSVファイル (.csv)|*.csv"
            dlg.FilterIndex = 1

            Select Case (dlg.ShowDialog())
                Case DialogResult.OK
                    'ダイアログOKボタン押下時
                    Console.WriteLine(dlg.FileName)
                Case Else
                    'ダイアログOKボタン以外押下時
                    ChDir(strDIR)
                    Return
            End Select

            'CSV出力
            Lng_Csv_Ok_Cnt = PFUNC_EXP_SINNYU(dlg.FileName)

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
    Private Function PFUNC_IMP_SINNYU(ByVal pCsvPath As String, ByRef lngOkCnt As Long, ByRef lngErrCnt As Long) As Boolean

        Dim red As System.IO.StreamReader = Nothing

        Dim strHeadder As String
        Dim strReadLine As String

        Dim strErrorLine As String = ""
        Dim sql As StringBuilder


        lngOkCnt = 0
        lngErrCnt = 0
        PFUNC_IMP_SINNYU = False

        '2007/02/12
        Try
            red = New System.IO.StreamReader(pCsvPath, System.Text.Encoding.Default)

            'SQL文作成
            '画面に設定されている学校コードを条件に新入生マスタを削除する
            sql = New StringBuilder(128)
            sql.Append(" DELETE  FROM SEITOMAST2")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'")

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                'データ処理中エラー
                MainLOG.Write("新入生マスタ削除", "失敗", sql.ToString)
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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_IMP_SINNYU)例外エラー", "失敗", ex.Message)
            Return False
        Finally
            If Not red Is Nothing Then red.Close()
        End Try

    End Function
    Private Function PFUNC_CHECK_STR(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intCnt As Integer
        Dim intCnt2 As Integer

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        '追加 2006/04/16
        Dim strRET As String = ""

        PFUNC_CHECK_STR = False

        strSplitValue = Split(pLineValue, ",")

        '入力内容チェック
        For intCnt = 0 To UBound(strSplitValue)
            Select Case (intCnt)
                Case 0
                    '学校コード

                    '未入力チェック
                    If Trim(strSplitValue(intCnt)) = "" Then

                        pError = pSeqNo & ",学校,未入力です"

                        Exit Function
                    End If

                    '画面上で入力されているコード以外のコードの場合はエラー
                    If Trim(txtGAKKOU_CODE.Text) <> Trim(strSplitValue(intCnt)).PadLeft(10, "0"c) Then

                        pError = pSeqNo & ",学校,画面で入力されている学校コード以外のものは登録できません"

                        Exit Function
                    End If

                    '学校マスタチェック
                    sql = New StringBuilder(128)
                    oraReader = New MyOracleReader(MainDB)
                    sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                    sql.Append(" WHERE GAKKOU_CODE_G = '" & Trim(strSplitValue(intCnt)).PadLeft(10, "0"c) & "'")

                    If oraReader.DataReader(sql) = False Then

                        pError = pSeqNo & ",学校,学校マスタに登録されている学校コード以外のものは登録できません"
                        oraReader.Close()
                        Return False
                    End If
                    oraReader.Close()
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

                    '入力桁数チェック 2007/02/12
                    If strSplitValue(intCnt).Trim.Length > 4 Then
                        pError = pSeqNo & ",入学年度,入学年度が４桁を超えて設定されています"

                        Exit Function

                    End If

                    If strSplitValue(intCnt).Trim.Length < 4 Then
                        pError = pSeqNo & ",入学年度,入学年度が４桁未満で設定されています"

                        Exit Function

                    End If
                    '入力値範囲チェック
                    Select Case (CInt(strSplitValue(intCnt)))
                        Case Is >= CInt(txtNENDO.Text)
                        Case Else
                            pError = pSeqNo & ",入学年度,進級処理年度以下の数値を設定することはできません"
                            Exit Function
                    End Select

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
                        Select Case (CLng(strSplitValue(intCnt)))
                            Case 1 To 9999
                            Case Else
                                pError = pSeqNo & ",通番,１～９９９９以外の数値を設定することはできません"

                                Exit Function
                        End Select
                    End If

                Case 3
                    '学年コード

                    '未入力チェック
                    If Trim(strSplitValue(intCnt)) = "" Then

                        pError = pSeqNo & ",学年,未入力です"

                        Exit Function
                    End If

                    '数値チェック
                    If IsNumeric(strSplitValue(intCnt)) = False Then

                        pError = pSeqNo & ",学年,学年に数値以外を設定することはできません"

                        Exit Function
                    Else
                        '入力値範囲チェック
                        Select Case (CLng(strSplitValue(intCnt)))
                            Case 0
                            Case Else
                                pError = pSeqNo & ",学年,新入生の学年は０以外の数値を設定することはできません"

                                Exit Function
                        End Select
                    End If
                Case 4
                    'クラスコード
                    '未入力チェック
                    If Trim(strSplitValue(intCnt)) = "" Then
                        pError = pSeqNo & ",クラス,未入力です"

                        Exit Function
                    Else

                        If IsNumeric(strSplitValue(intCnt)) = False Then

                            pError = pSeqNo & ",クラス,数値以外を設定することはできません"

                            Exit Function
                        Else
                            '入力値範囲チェック
                            Select Case (CLng(strSplitValue(intCnt)))
                                Case 1 To 20
                                Case Else
                                    pError = pSeqNo & ",クラス,１～２０以外の数値を設定することはできません"

                                    Exit Function
                            End Select
                        End If

                        '使用クラスチェック
                        '1学年のクラスを参照
                        sql = New StringBuilder(128)
                        oraReader = New MyOracleReader(MainDB)
                        sql.Append("SELECT * FROM GAKMAST1")
                        sql.Append(" WHERE")
                        sql.Append(" GAKKOU_CODE_G = '" & Trim(strSplitValue(0)).PadLeft(10, "0"c) & "'")
                        sql.Append(" AND")
                        sql.Append(" GAKUNEN_CODE_G = 1")
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

                        pError = pSeqNo & ",生徒番号,生徒番号に数値以外を設定することはできません"

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
                Case 7 '生徒名漢字　長さチェック2006/05/10
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
                    '取扱金融機関コード

                    '振替方法が集金扱い(=1)の場合は未入力でも可
                    Select Case (Trim(strSplitValue(15)))
                        Case "1"

                            If Trim(strSplitValue(intCnt)) <> "" Then
                                '数値チェック  2007/02/12
                                If IsNumeric(strSplitValue(intCnt)) = False Then

                                    pError = pSeqNo & ",取扱金融機関,金融機関コードに数値以外を設定することはできません"

                                    Exit Function
                                End If

                                '桁数チェック 2007/02/12
                                If strSplitValue(intCnt).Trim.Length > 4 Then

                                    pError = pSeqNo & ",取扱金融機関,金融機関コードが４桁を超えています"

                                    Exit Function
                                End If
                            End If

                        Case Else
                            '数値チェック  2007/02/12
                            If Trim(strSplitValue(intCnt)) = "" Or IsNumeric(strSplitValue(intCnt)) = False Then

                                pError = pSeqNo & ",取扱金融機関,金融機関コードに空白または数値ではない値が設定されています"

                                Exit Function
                            End If

                            '桁数チェック 2007/02/12
                            If strSplitValue(intCnt).Trim.Length > 4 Then

                                pError = pSeqNo & ",取扱金融機関,金融機関コードが４桁を超えています"

                                Exit Function
                            End If
                    End Select
                Case 10
                    '取扱支店コード
                    '振替方法が集金扱い(=1)の場合は未入力でも可
                    Select Case (Trim(strSplitValue(15)))
                        Case "1"
                            If Trim(strSplitValue(intCnt)) <> "" Then
                                '数値チェック  2007/02/12
                                If IsNumeric(strSplitValue(intCnt)) = False Then

                                    pError = pSeqNo & ",取扱支店,支店コードに数値以外を設定することはできません"

                                    Exit Function
                                End If

                                '桁数チェック 2007/02/12
                                If strSplitValue(intCnt).Trim.Length > 3 Then

                                    pError = pSeqNo & ",取扱支店,支店コードが３桁を超えています"

                                    Exit Function
                                Else
                                    If Trim(strSplitValue(intCnt - 1)) <> "" Then '金融機関も空白でないなら存在チェック
                                        '金融機関マスタ存在チェック　2007/02/12
                                        sql = New StringBuilder(128)
                                        oraReader = New MyOracleReader(MainDB)
                                        sql.Append("SELECT * FROM TENMAST ")
                                        sql.Append(" WHERE")
                                        sql.Append(" KIN_NO_N = '" & Trim(strSplitValue(intCnt - 1)).PadLeft(4, "0"c) & "'")
                                        sql.Append(" AND")
                                        sql.Append(" SIT_NO_N = '" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                                        If oraReader.DataReader(sql) = False Then
                                            pError = pSeqNo & ",取扱支店,金融機関マスタに登録されていません"
                                            oraReader.Close()
                                            Return False
                                        End If
                                        oraReader.Close()
                                    End If
                                End If

                            End If
                        Case Else
                            '数値チェック  2007/02/12
                            If Trim(strSplitValue(intCnt)) = "" Or IsNumeric(strSplitValue(intCnt)) = False Then
                                pError = pSeqNo & ",取扱支店,支店コードに空白または数値以外の値が設定されています"
                                Exit Function
                            End If

                            '桁数チェック 2007/02/12
                            If strSplitValue(intCnt).Trim.Length > 3 Then
                                pError = pSeqNo & ",取扱支店,支店コードが３桁を超えています"
                                Exit Function
                            End If

                            '金融機関マスタ存在チェック　2007/02/12
                            sql = New StringBuilder(128)
                            oraReader = New MyOracleReader(MainDB)
                            sql.Append("SELECT * FROM TENMAST ")
                            sql.Append(" WHERE")
                            sql.Append(" KIN_NO_N = '" & Trim(strSplitValue(intCnt - 1)).PadLeft(4, "0"c) & "'")
                            sql.Append(" AND")
                            sql.Append(" SIT_NO_N = '" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                            If oraReader.DataReader(sql) = False Then

                                pError = pSeqNo & ",取扱支店,金融機関マスタに登録されていません"
                                oraReader.Close()
                                Return False
                            End If
                            oraReader.Close()
                    End Select
                Case 12
                    '口座番号
                    '振替方法が集金扱い(=1)の場合は未入力でも可
                    Select Case (Trim(strSplitValue(15)))
                        Case "1"
                            If Trim(strSplitValue(intCnt)) <> "" Then

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
                            End If
                        Case Else
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

                            '''''チェックデジット処理追加 2007/02/12
                            If STR_CHK_DJT = "1" Then '<---iniファイルのチェックデジット判定区分 0:しない 1:する
                                If Format(CLng(strSplitValue(9)), "0000") = STR_JIKINKO_CODE Then '自金庫データのみチェックデジット実行
                                    If GFUNC_CHK_DEJIT(Format(CLng(strSplitValue(9)), "0000"), Format(CLng(strSplitValue(10)), "000"), CInt(strSplitValue(11)), Format(CLng(strSplitValue(12)), "0000000")) = False Then
                                        pError = pSeqNo & ",取扱口座番号,チェックデジットエラーです"
                                        Exit Function
                                    End If
                                End If
                            End If
                    End Select
                Case 13
                    '名義人(ｶﾅ)
                    '振替方法が集金扱い(=1)の場合は未入力でも可
                    Select Case (Trim(strSplitValue(15)))
                        Case "1"
                            If strSplitValue(intCnt) <> "" Then
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
                        Case Else
                            If Trim(strSplitValue(intCnt)) = "" Then

                                pError = pSeqNo & ",取扱名義人(ｶﾅ),未入力です。"

                                Exit Function
                            Else
                                '規定外文字チェック 2006/04/16
                                If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                                    pError = pSeqNo & ",取扱名義人(ｶﾅ),規定外文字が有ります。"
                                    Exit Function
                                End If
                                '文字数チェック 2006/05/10
                                If Trim(strSplitValue(intCnt)).Length > 40 Then
                                    pError = pSeqNo & ",取扱名義人(ｶﾅ),半角40文字以内で設定してください"
                                    Exit Function
                                End If
                            End If
                    End Select
                Case 14    '名義人漢字　長さチェック2006/05/10

                    If Trim(strSplitValue(intCnt)) <> "" Then
                        '２バイト変換
                        strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Wide)
                        '文字数チェック
                        If Trim(strSplitValue(intCnt)).Length > 25 Then
                            pError = pSeqNo & ",名義人(漢字),全角25文字以内で設定してください"
                            Exit Function
                        End If
                    End If
                Case 16 '住所　長さチェック 2006/05/10
                    If Trim(strSplitValue(intCnt)) <> "" Then
                        '２バイト変換
                        strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Wide)
                        '文字数チェック
                        If Trim(strSplitValue(intCnt)).Length > 25 Then
                            pError = pSeqNo & ",住所,全角25文字以内で設定してください"
                            Exit Function
                        End If
                    End If
                Case 17 '電話番号　長さチェック 2006/05/10
                    If strSplitValue(intCnt) <> "" Then
                        '規定外文字チェック 2006/04/16
                        If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = False Then
                            pError = pSeqNo & ",電話番号,規定外文字が有ります。"
                            Exit Function
                        End If
                        '1バイト変換
                        strSplitValue(intCnt) = StrConv(Trim(strSplitValue(intCnt)), VbStrConv.Narrow)
                        '文字数チェック
                        '2012/04/13 saitou 標準修正 MODIFY ---------------------------------------->>>>
                        '電話番号12桁→13桁に変更
                        If Trim(strSplitValue(intCnt)).Length > 13 Then
                            pError = pSeqNo & ",電話番号,半角13文字以内で設定してください"
                            Exit Function
                        End If
                        'If Trim(strSplitValue(intCnt)).Length > 12 Then
                        '    pError = pSeqNo & ",電話番号,半角12文字以内で設定してください"
                        '    Exit Function
                        'End If
                        '2012/04/13 saitou 標準修正 MODIFY ----------------------------------------<<<<
                    End If
                Case 20
                    '費目ID
                    If Trim(strSplitValue(intCnt)) = "" Then
                        pError = pSeqNo & ",費目ID,未入力です"
                        Exit Function
                    Else
                        '数値チェック  2007/02/15
                        If IsNumeric(strSplitValue(intCnt)) = False Then

                            pError = pSeqNo & ",費目ID,費目IDに数値以外の値が設定されています"

                            Exit Function
                        End If

                        '桁数チェック追加 2007/02/12
                        If strSplitValue(intCnt).Trim.Length >= 4 Then
                            pError = pSeqNo & ",費目ID,費目IDが４桁を超えて設定されています"
                            Exit Function
                        End If
                        '費目マスタ存在チェック
                        sql = New StringBuilder(128)
                        oraReader = New MyOracleReader(MainDB)
                        sql.Append(" SELECT * FROM HIMOMAST")
                        sql.Append(" WHERE")
                        sql.Append(" GAKKOU_CODE_H ='" & Trim(txtGAKKOU_CODE.Text) & "'")
                        sql.Append(" AND ")
                        sql.Append(" GAKUNEN_CODE_H = 1")
                        sql.Append(" AND")
                        sql.Append(" HIMOKU_ID_H ='" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                        If oraReader.DataReader(sql) = False Then

                            pError = pSeqNo & ",費目ID,費目マスタに登録されていない費目IDを設定することはできません"
                            oraReader.Close()
                            Return False
                        End If
                        oraReader.Close()
                    End If
                    '新入生は長子情報はチェックしない 2007/02/12
                    'Case 21  '長子情報追加 2005/03/09
                    '        '長子フラグ
                    '        '未入力チェック
                    '        If Trim(strSplitValue(intCnt)) = "" Then

                    '            pError = pSeqNo & ",長子フラグ,未入力です。"

                    '            Exit Function
                    '        End If

                    'Case 22
                    '        '長子入学年度
                    '        If Trim(strSplitValue(21)) = "1" Then
                    '            If Trim(strSplitValue(intCnt)) = "" Then
                    '                pError = pSeqNo & ",長子入学年度,未入力です。"

                    '                Exit Function
                    '            End If
                    '        End If
                    'Case 23
                    '        '長子通番
                    '        If Trim(strSplitValue(21)) = "1" Then
                    '            If Trim(strSplitValue(intCnt)) = "0" OR Trim(strSplitValue(intCnt)) = "" Then
                    '                pError = pSeqNo & ",長子通番,未入力です。"

                    '                Exit Function
                    '            End If
                    '        End If
                    'Case 24
                    '        '長子学年
                    '        If Trim(strSplitValue(21)) = "1" Then
                    '            If Trim(strSplitValue(intCnt)) = "0" OR Trim(strSplitValue(intCnt)) = "" Then
                    '                pError = pSeqNo & ",長子学年,未入力です。"

                    '                Exit Function
                    '            End If
                    '        End If
                    'Case 25
                    '        '長子クラス
                    '        If Trim(strSplitValue(21)) = "1" Then
                    '            If Trim(strSplitValue(intCnt)) = "0" OR Trim(strSplitValue(intCnt)) = "" Then
                    '                pError = pSeqNo & ",長子クラス,未入力です。"

                    '                Exit Function
                    '            End If
                    '        End If
                    'Case 26
                    '        '長子生徒番号
                    '        If Trim(strSplitValue(21)) = "1" Then
                    '            If Trim(strSplitValue(intCnt)) = "0" OR Trim(strSplitValue(intCnt)) = "" Then
                    '                pError = pSeqNo & ",長子生徒番号,未入力です。"

                    '                Exit Function
                    '            End If
                    '        End If

            End Select
        Next intCnt

        Return True

    End Function
    Private Function PFUNC_INSERT_STR(ByVal pSeqNo As Long, ByVal pLineValue As String, ByRef pError As String) As Boolean

        Dim intTuki As Integer
        Dim intCol As Integer

        Dim intCnt As Integer

        Dim strSql As String

        Dim sql As StringBuilder

        Dim strRET As String = ""

        PFUNC_INSERT_STR = False

        For intTuki = 1 To 12
            'SQL文作成
            sql = New StringBuilder(128)
            sql.Append(" INSERT INTO SEITOMAST2 values(")
            'For intCnt = 0 To UBound(strSplitValue)
            For intCnt = 0 To 26 '2005/06/15修正
                Select Case (intCnt)
                    Case 0
                        '学校コード
                        sql.Append(IIf(intCnt <> 0, ",", "") & "'" & Trim(strSplitValue(intCnt)).PadLeft(10, "0"c) & "'")

                    Case 5, 12
                        '生徒番号,口座番号
                        sql.Append(IIf(intCnt <> 0, ",", "") & "'" & Trim(strSplitValue(intCnt)).PadLeft(7, "0"c) & "'")

                    Case 1, 7, 14, 16, 17
                        '入学年度,生徒氏名(漢字),名義人(漢字),
                        '契約住所(漢字),契約電話番号
                        sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                    Case 6, 13 '生徒氏名(ｶﾅ),名義人(カナ) 2007/02/12
                        If ConvKanaNGToKanaOK(strSplitValue(intCnt), strRET) = True Then
                            sql.Append(",'" & strRET & "'")
                        Else
                            sql.Append(",'" & Space(40) & "'")
                        End If

                    Case 2, 3, 4
                        '通番,学年ｺｰﾄﾞ,ｸﾗｽｺｰﾄﾞ
                        sql.Append("," & Trim(strSplitValue(intCnt)))
                    Case 8
                        '性別
                        '値なしの場合は２固定
                        Select Case (Trim(strSplitValue(intCnt)))
                            Case "0", "1", "2"
                                sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                            Case Else
                                sql.Append(",'2'")
                        End Select
                    Case 9
                        '取扱金融機関コード
                        If Trim(strSplitValue(intCnt)) <> "" Then
                            sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(4, "0"c) & "'")
                        Else
                            sql.Append(",' '")
                        End If
                    Case 10
                        '取扱支店コード
                        If Trim(strSplitValue(intCnt)) <> "" Then
                            sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")
                        Else
                            sql.Append(",' '")
                        End If
                    Case 11
                        '科目
                        Select Case (Trim(strSplitValue(intCnt)).PadLeft(2, "0"c))
                            Case "01", "02", "05"
                                sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(2, "0"c) & "'")
                            Case Else
                                sql.Append(",'01'")
                        End Select
                    Case 15
                        '振替方法
                        '値なしの場合は０固定
                        Select Case (Trim(strSplitValue(intCnt)))
                            Case "0", "1", "2"
                                sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                            Case Else
                                sql.Append(",'0'")
                        End Select
                    Case 18
                        '解約区分
                        '値なしの場合は０固定
                        Select Case (Trim(strSplitValue(intCnt)))
                            Case "0", "9"
                                sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                            Case Else
                                sql.Append(",'0'")
                        End Select
                    Case 19
                        '進級区分
                        '値なしの場合は０固定
                        Select Case (Trim(strSplitValue(intCnt)))
                            Case "0", "1"
                                sql.Append(",'" & Trim(strSplitValue(intCnt)) & "'")
                            Case Else
                                sql.Append(",'0'")
                        End Select
                    Case 20
                        '費目ID
                        sql.Append(",'" & Trim(strSplitValue(intCnt)).PadLeft(3, "0"c) & "'")

                End Select
            Next intCnt

            '長子情報
            For intCol = 0 To 5
                Select Case (intCol)
                    Case 0, 2, 3, 4
                        '長子有無ﾌﾗｸﾞ,長子通番,長子学年,長子ｸﾗｽ
                        sql.Append(",0")
                    Case 1, 5
                        '長子年度,長子生徒番号
                        sql.Append(",' '")
                End Select
            Next intCol

            '請求月
            sql.Append(",'" & Format(intTuki, "00") & "'")

            '費目１～１５
            For intCol = 1 To 15
                '請求方法
                sql.Append(",'0'")
                '請求金額
                sql.Append(",0")
            Next intCol

            sql.Append(",'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
            sql.Append(",'00000000'")
            sql.Append(",'" & Space(50) & "'") 'YOBI1_O
            sql.Append(",'" & Space(50) & "'") 'YOBI2_O
            sql.Append(",'" & Space(50) & "'") 'YOBI3_O
            sql.Append(",'" & Space(50) & "'") 'YOBI4_O
            sql.Append(",'" & Space(50) & "'") 'YOBI5_O
            sql.Append(")")

            '重複チェック
            strSql = " SELECT * FROM SEITOMAST2"
            strSql += " WHERE GAKKOU_CODE_O ='" & Trim(strSplitValue(0)).PadLeft(10, "0"c) & "'"
            strSql += " AND NENDO_O ='" & Trim(strSplitValue(1)) & "'"
            strSql += " AND TUUBAN_O =" & Trim(strSplitValue(2).PadLeft(4, "0"c))
            strSql += " AND TUKI_NO_O ='" & Format(intTuki, "00") & "'"

            Dim oraReader As New MyOracleReader(MainDB)

            If oraReader.DataReader(strSql) = True Then
                pError = pSeqNo & ",主キー,データが重複しています"
                oraReader.Close()
                Return False
            End If
            oraReader.Close()

            'トランザクションデータ処理実行
            If MainDB.ExecuteNonQuery(sql) < 0 Then

                pError = pSeqNo & ",移入,移入処理中にエラーが発生しました"
                Return False

            End If

        Next intTuki

        Return True

    End Function
    Private Function PFUNC_EXP_SINNYU(ByVal pCsvPath As String) As Long

        Dim lngLine As Long = 0
        '2012/10/06 saitou 大垣信金 カスタマイズ UPD -------------------------------------------------->>>>
        'PTA会費口数追加対応
        'String→StringBuilderに変更し、処理速度向上を図る
        Dim sbLine As New StringBuilder
        'Dim strLine As String
        '2012/10/06 saitou 大垣信金 カスタマイズ UPD --------------------------------------------------<<<<

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)
        Dim wrt As System.IO.StreamWriter = Nothing

        Try
            sql.Append(" SELECT distinct")
            sql.Append(" GAKKOU_CODE_O , NENDO_O , TUUBAN_O , GAKUNEN_CODE_O , CLASS_CODE_O")
            sql.Append(",SEITO_NO_O , SEITO_KNAME_O , SEITO_NNAME_O , SEIBETU_O , TKIN_NO_O")
            sql.Append(",TSIT_NO_O , KAMOKU_O , KOUZA_O , MEIGI_KNAME_O , MEIGI_NNAME_O")
            sql.Append(",FURIKAE_O , KEIYAKU_NJYU_O , KEIYAKU_DENWA_O , KAIYAKU_FLG_O , SINKYU_KBN_O")
            sql.Append(",HIMOKU_ID_O")
            sql.Append(" FROM SEITOMAST2")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_O ='" & Trim(txtGAKKOU_CODE.Text) & "'")
            sql.Append(" ORDER BY ")
            Select Case (STR帳票ソート順)
                Case "0"
                    sql.Append("      GAKKOU_CODE_O    ASC")
                    sql.Append("     ,GAKUNEN_CODE_O   ASC")
                    sql.Append("     ,CLASS_CODE_O     ASC")
                    sql.Append("     ,SEITO_NO_O       ASC")
                    sql.Append("     ,NENDO_O          ASC")
                    sql.Append("     ,TUUBAN_O         ASC")
                Case "1"
                    sql.Append("      GAKKOU_CODE_O    ASC")
                    sql.Append("     ,GAKUNEN_CODE_O   ASC")
                    sql.Append("     ,NENDO_O          ASC")
                    sql.Append("     ,TUUBAN_O         ASC")
                Case Else
                    sql.Append("      GAKKOU_CODE_O    ASC")
                    sql.Append("     ,GAKUNEN_CODE_O   ASC")
                    sql.Append("     ,SEITO_KNAME_O    ASC")
                    sql.Append("     ,NENDO_O          ASC")
                    sql.Append("     ,TUUBAN_O         ASC")
            End Select

            If oraReader.DataReader(sql) = True Then

                lngLine = 0

                '項目名見出し設定
                '2012/10/06 saitou 大垣信金 カスタマイズ UPD -------------------------------------------------->>>>
                'PTA会費口数追加対応
                'String→StringBuilderに変更し、処理速度向上を図る
                sbLine.Length = 0
                With sbLine
                    .Append("学校コード,入学年度,通番,学年コード,クラス")
                    .Append(",生徒番号,生徒氏名(カナ),生徒氏名(漢字),性別")
                    .Append(",金融機関コード,支店コード,科目,口座番号,名義人（カナ),名義人（漢字),振替方法")
                    .Append(",契約住所(漢字),契約電話番号")
                    .AppendLine(",解約区分,進級区分,費目ID")
                End With
                'strLine = "学校コード,入学年度,通番,学年コード,クラス"
                'strLine += ",生徒番号,生徒氏名(カナ),生徒氏名(漢字),性別"
                'strLine += ",金融機関コード,支店コード,科目,口座番号,名義人（カナ),名義人（漢字),振替方法"
                'strLine += ",契約住所(漢字),契約電話番号"
                'strLine += ",解約区分,進級区分,費目ID"
                'strLine += vbCrLf
                '2012/10/06 saitou 大垣信金 カスタマイズ UPD --------------------------------------------------<<<<

                Do Until oraReader.EOF

                    '2012/10/06 saitou 大垣信金 カスタマイズ UPD -------------------------------------------------->>>>
                    'PTA会費口数追加対応
                    'String→StringBuilderに変更し、処理速度向上を図る
                    'GetStringで取得している時点でDBNullは有りえないし、Trimも必要なし

                    '学校コード
                    '2016/11/04 タスク）西野 CHG 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- START
                    sbLine.Append(Microsoft.VisualBasic.Right(oraReader.GetString("GAKKOU_CODE_O"), GAKKOU_CODE_LENGTH))
                    'sbLine.Append(oraReader.GetString("GAKKOU_CODE_O"))
                    '2016/11/04 タスク）西野 CHG 【PG】飯田信金 カスタマイズ対応(標準版改善) -------------------- END
                    '入学年度
                    sbLine.Append("," & oraReader.GetString("NENDO_O"))
                    '通番
                    sbLine.Append("," & oraReader.GetString("TUUBAN_O"))
                    '学年コード
                    sbLine.Append("," & oraReader.GetString("GAKUNEN_CODE_O"))
                    'クラスコード
                    sbLine.Append("," & oraReader.GetString("CLASS_CODE_O"))
                    '生徒番号
                    sbLine.Append("," & oraReader.GetString("SEITO_NO_O"))
                    '生徒氏名(カナ)
                    sbLine.Append("," & oraReader.GetString("SEITO_KNAME_O"))
                    '生徒氏名(漢字)
                    sbLine.Append("," & oraReader.GetString("SEITO_NNAME_O"))
                    '性別
                    If oraReader.GetString("SEIBETU_O") <> String.Empty Then
                        sbLine.Append("," & oraReader.GetString("SEIBETU_O"))
                    Else
                        sbLine.Append(",2")
                    End If
                    '金融機関コード
                    sbLine.Append("," & oraReader.GetString("TKIN_NO_O"))
                    '支店コード
                    sbLine.Append("," & oraReader.GetString("TSIT_NO_O"))
                    '科目
                    sbLine.Append("," & oraReader.GetString("KAMOKU_O"))
                    '口座番号
                    sbLine.Append("," & oraReader.GetString("KOUZA_O"))
                    '名義人(カナ)
                    sbLine.Append("," & oraReader.GetString("MEIGI_KNAME_O"))
                    '名義人(漢字)
                    sbLine.Append("," & oraReader.GetString("MEIGI_NNAME_O"))
                    '振替方法
                    sbLine.Append("," & oraReader.GetString("FURIKAE_O"))
                    '契約住所(漢字)
                    sbLine.Append("," & oraReader.GetString("KEIYAKU_NJYU_O"))
                    '契約電話番号
                    sbLine.Append("," & oraReader.GetString("KEIYAKU_DENWA_O"))
                    '解約区分
                    sbLine.Append("," & oraReader.GetString("KAIYAKU_FLG_O"))
                    '進級区分
                    sbLine.Append("," & oraReader.GetString("SINKYU_KBN_O"))
                    '費目ID
                    sbLine.AppendLine("," & oraReader.GetString("HIMOKU_ID_O"))

                    ''学校コード
                    'If IsDBNull(oraReader.GetString("GAKKOU_CODE_O")) = False Then
                    '    strLine += Trim(CStr(oraReader.GetString("GAKKOU_CODE_O")))
                    'End If

                    ''入学年度
                    'If IsDBNull(oraReader.GetString("NENDO_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("NENDO_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''通番
                    'If IsDBNull(oraReader.GetString("TUUBAN_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("TUUBAN_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''学年コード
                    'If IsDBNull(oraReader.GetString("GAKUNEN_CODE_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("GAKUNEN_CODE_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''クラスコード
                    'If IsDBNull(oraReader.GetString("CLASS_CODE_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("CLASS_CODE_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''生徒番号
                    'If IsDBNull(oraReader.GetString("SEITO_NO_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("SEITO_NO_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''生徒氏名(カナ)
                    'If IsDBNull(oraReader.GetString("SEITO_KNAME_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("SEITO_KNAME_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''生徒氏名(漢字)
                    'If IsDBNull(oraReader.GetString("SEITO_NNAME_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("SEITO_NNAME_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''性別
                    'If IsDBNull(oraReader.GetString("SEIBETU_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("SEIBETU_O")))
                    'Else
                    '    strLine += ",2"
                    'End If

                    ''金融機関コード
                    'If IsDBNull(oraReader.GetString("TKIN_NO_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("TKIN_NO_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''支店コード
                    'If IsDBNull(oraReader.GetString("TSIT_NO_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("TSIT_NO_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''科目
                    'If IsDBNull(oraReader.GetString("KAMOKU_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("KAMOKU_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''口座番号
                    'If IsDBNull(oraReader.GetString("KOUZA_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("KOUZA_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''名義人(カナ)
                    'If IsDBNull(oraReader.GetString("MEIGI_KNAME_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("MEIGI_KNAME_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''名義人(漢字)
                    'If IsDBNull(oraReader.GetString("MEIGI_NNAME_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("MEIGI_NNAME_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''振替方法
                    'If IsDBNull(oraReader.GetString("FURIKAE_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("FURIKAE_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''契約住所(漢字)
                    'If IsDBNull(oraReader.GetString("KEIYAKU_NJYU_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("KEIYAKU_NJYU_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''契約電話番号
                    'If IsDBNull(oraReader.GetString("KEIYAKU_DENWA_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("KEIYAKU_DENWA_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''解約区分
                    'If IsDBNull(oraReader.GetString("KAIYAKU_FLG_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("KAIYAKU_FLG_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''進級区分
                    'If IsDBNull(oraReader.GetString("SINKYU_KBN_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("SINKYU_KBN_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    ''費目ID
                    'If IsDBNull(oraReader.GetString("HIMOKU_ID_O")) = False Then
                    '    strLine += "," & Trim(CStr(oraReader.GetString("HIMOKU_ID_O")))
                    'Else
                    '    strLine += ","
                    'End If

                    'strLine += vbCrLf
                    '2012/10/06 saitou 大垣信金 カスタマイズ UPD --------------------------------------------------<<<<

                    lngLine += 1

                    oraReader.NextRead()

                Loop

            Else

                Return 0

            End If
            oraReader.Close()

            wrt = New System.IO.StreamWriter(pCsvPath, False, System.Text.Encoding.Default)

            '2012/10/06 saitou 大垣信金 カスタマイズ UPD -------------------------------------------------->>>>
            'PTA会費口数追加対応
            'String→StringBuilderに変更し、処理速度向上を図る
            wrt.Write(sbLine.ToString)
            'wrt.Write(strLine)
            '2012/10/06 saitou 大垣信金 カスタマイズ UPD --------------------------------------------------<<<<

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
    Private Function PFUNC_GAKMAST2_GET() As Boolean

        '学校マスタ２の取得
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        sql.Append(" SELECT MEISAI_OUT_T FROM GAKMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_T ='" & txtGAKKOU_CODE.Text.Trim & "'")

        If oraReader.DataReader(sql) = True Then
            STR帳票ソート順 = oraReader.GetString("MEISAI_OUT_T").ToString
        Else
            oraReader.Close()
            Return False
        End If

        oraReader.Close()
        Return True

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
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        Dim oraDB As New MyOracle
        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader = Nothing

        'COMBOBOX選択時学校名,学校コード設定
        lblGAKKOU_NAME.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)

        '入学年度設定処理　2007/02/12
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            Sql = New StringBuilder(128)
            oraReader = New MyOracleReader(oraDB)
            Sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
            Sql.Append(" WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If oraReader.DataReader(Sql) = True Then
                lblGAKKOU_NAME.Text = CStr(oraReader.GetString("GAKKOU_NNAME_G"))
            Else
                lblGAKKOU_NAME.Text = ""
            End If
            oraReader.Close()

            '最終進級処理年月を表示する
            Sql = New StringBuilder(128)
            oraReader = New MyOracleReader(oraDB)
            Sql.Append("SELECT SINKYU_NENDO_T FROM GAKMAST2 ")
            Sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

            If oraReader.DataReader(Sql) = True Then
                txtNENDO.Text = CInt(oraReader.GetString("SINKYU_NENDO_T")) + 1
            Else
                txtNENDO.Text = ""
            End If
            oraReader.Close()
        End If


        '学年テキストボックスにFOCUS
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        Dim oraDB As New MyOracle
        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader = Nothing

        Try

            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                '学校名検索
                sql = New StringBuilder(128)
                oraReader = New MyOracleReader(oraDB)
                sql.Append("SELECT GAKKOU_NNAME_G FROM GAKMAST1 ")
                sql.Append(" WHERE GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

                If oraReader.DataReader(sql) = False Then
                    MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    lblGAKKOU_NAME.Text = ""
                    oraReader.Close()
                    Return
                Else
                    lblGAKKOU_NAME.Text = CStr(oraReader.GetString("GAKKOU_NNAME_G"))
                End If
                oraReader.Close()


                '最終進級処理年月を表示する
                sql = New StringBuilder(128)
                oraReader = New MyOracleReader(oraDB)
                sql.Append("SELECT SINKYU_NENDO_T FROM GAKMAST2 ")
                sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

                If oraReader.DataReader(sql) = True Then
                    txtNENDO.Text = CInt(oraReader.GetString("SINKYU_NENDO_T")) + 1
                Else
                    txtNENDO.Text = ""
                End If
                oraReader.Close()
            End If

        Catch ex As Exception

        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
