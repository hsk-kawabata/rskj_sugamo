Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon

Public Class KFGNENJ020

#Region " 共通変数定義 "
    Dim strCSV_InFileName As String
    Dim strCSV_OutFileName As String
    Dim strExport_Sort As String
    Dim strExport_GakkouCode As String
    Dim strExport_Nendo As String
    Dim intExport_Tuban As Integer
    Dim strExport_KName As String
    Dim strExport_NName As String
    Dim intExport_Gakunen As Integer
    Dim intExport_Class As Integer
    Dim strExport_SeitoNo As String
    Dim strExport_OldGakkouName As String
    Dim strExport_OldClass As String
    Dim strExport_OldSeitoNo As String
    Dim strLine As String
    Dim strImport_SakujoFLG As String
    Dim strImport_GakkouCode As String
    Dim strImport_Nendo As String
    Dim strImport_Tuban As String
    Dim strImport_KName As String
    Dim strImport_NName As String
    Dim strImport_Gakunen As String
    Dim strImport_Class As String
    Dim strImport_SeitoNo As String
    Dim strImport_NewClass As String
    Dim strImport_NewSeitoNo As String
    Dim strImport_Kaigyo As String
    Dim fnbr As Integer

    Dim strErrLog As String
    Dim Str_Report_Path As String
    Dim intSiyouGakunen As Integer
    Dim strSyori As String
#End Region

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGNENJ020", "クラス替え処理画面")
    Private Const msgTitle As String = "クラス替え処理画面(KFGNENJ020)"
    Private MainDB As MyOracle

    Private KobetuLogFileName As String

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGNENJ020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            '入力ボタン制御
            btnInyu.Enabled = True
            btnIsyutu.Enabled = True
            btnPrint.Enabled = True
            btnAction.Enabled = True
            btnEnd.Enabled = True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnInyu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInyu.Click
        '移入ボタン
        'ＣＳＶ形式テキストを生徒マスタに取込む
        Dim strDIR As String

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移入)開始", "成功", "")

            MainDB = New MyOracle

            strDIR = CurDir()

            '共通チェック
            If PFUC_COMMON_CHK() = False Then
                Return
            End If

            If MessageBox.Show(String.Format(MSG0015I, "移入"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            'ログファイル削除
            KobetuLogFileName = Path.Combine(STR_LOG_PATH, "CLASS" & txtGAKKOU_CODE.Text & ".log")
            If File.Exists(KobetuLogFileName) Then
                File.Delete(KobetuLogFileName)
            End If

            'ファイルを開くダイアログボックスの設定

            With OpenFileDialog1
                .InitialDirectory = STR_CSV_PATH
                .Filter = "csvﾌｧｲﾙ(*.csv)|*.csv"
                .FilterIndex = 1
                .FileName = "CLASS" & txtGAKKOU_CODE.Text & txtGAKUNEN.Text & ".csv"
            End With

            If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
                strCSV_InFileName = OpenFileDialog1.FileName
            Else
                ChDir(strDIR)
                txtGAKKOU_CODE.Focus()
                txtGAKKOU_CODE.SelectionStart = 0
                txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length
                Return
            End If

            'ＣＳＶファイルのオープン
            fnbr = FreeFile()
            FileOpen(fnbr, strCSV_InFileName, OpenMode.Input)
            Try
                If PFUC_CSVINPUT_CHK() <> 0 Then
                    'ＣＳＶファイルクローズ
                    FileClose(fnbr)

                    ChDir(strDIR)   '2008/04/17 追加

                    MainLOG.Write("ＣＳＶファイルのオープン", "失敗", "")

                    MessageBox.Show(String.Format(G_MSG0041W, strErrLog, STR_CSV_PATH & "CLASS" & txtGAKKOU_CODE.Text & txtGAKUNEN.Text & ".log"), _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    Return
                End If
            Catch ex As Exception
                Throw New Exception(ex.ToString, ex)
            Finally
                'ＣＳＶファイルクローズ
                FileClose(fnbr)
            End Try


            'ＣＳＶファイルのオープン
            fnbr = FreeFile()
            FileOpen(fnbr, strCSV_InFileName, OpenMode.Input)

            'トランザクション開始
            MainDB.BeginTrans()

            If PFUC_SEITOMAST_KOUSIN() <> 0 Then
                MainDB.Rollback()
                'ＣＳＶファイルクローズ
                FileClose(fnbr)
                MainLOG.Write("移入", "失敗", "生徒マスタ更新")
                ChDir(strDIR)   '2008/04/17 追加
                Return
            End If

            'トランザクション終了（ＣＯＭＭＩＴ）
            MainDB.Commit()

            'ＣＳＶファイルクローズ
            FileClose(fnbr)
            MainLOG.Write("移入", "成功", "")

            ChDir(strDIR)

            MessageBox.Show(String.Format(MSG0016I, "移入"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            txtGAKKOU_CODE.Focus()
            txtGAKKOU_CODE.SelectionStart = 0
            txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length

        Catch ex As Exception
            MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移入)例外エラー", "失敗", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移入)終了", "成功", "")
        End Try

    End Sub
    Private Sub btnIsyutu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnIsyutu.Click
        '移出ボタン
        '生徒マスタからＣＳＶ形式テキストをはき出す
        '2007/08/16　進学生徒対応

        Dim wrt As StreamWriter = Nothing
        Dim csvFileName As String

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader = Nothing

        Dim lngLine As Long = 0
        Dim strLine As String

        Dim strDIR As String
        strDIR = CurDir()

        Try
            MainDB = New MyOracle

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出)開始", "成功", "")

            '==========================================
            '共通チェック
            '==========================================
            If PFUC_COMMON_CHK() = False Then
                Return
            End If

            '生徒存在チェック
            '2006/10/19　生徒が1人も登録されていない場合、メッセージを表示し、処理を中断する
            If PFUNC_SEITO_CHECK() = False Then
                Exit Sub
            End If

            If MessageBox.Show(String.Format(MSG0015I, "移出"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            '==========================================
            'ファイル保存のダイアログボックスの設定
            '==========================================
            SaveFileDialog1.InitialDirectory = STR_CSV_PATH
            SaveFileDialog1.Filter = "csvﾌｧｲﾙ(*.csv)|*.csv|全てのﾌｧｲﾙ(*.*)|*.*"
            SaveFileDialog1.FilterIndex = 1
            SaveFileDialog1.FileName = "CLASS" & txtGAKKOU_CODE.Text & txtGAKUNEN.Text & ".csv"

            If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
                csvFileName = SaveFileDialog1.FileName
            Else
                ChDir(strDIR)
                txtGAKKOU_CODE.Focus()
                txtGAKKOU_CODE.SelectionStart = 0
                txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length
                Exit Sub
            End If

            '==========================================
            '生徒マスタの読込み
            '==========================================

            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)

            sql.Append("SELECT * FROM SEITOMAST")
            sql.Append(" WHERE GAKKOU_CODE_O = " & "'" & txtGAKKOU_CODE.Text & "'")
            sql.Append(" AND GAKUNEN_CODE_O = " & CInt(txtGAKUNEN.Text))
            sql.Append(" AND TUKI_NO_O ='04'")

            Select Case strExport_Sort
                Case "0"      '学年、クラス、生徒番号
                    sql.Append(" ORDER BY GAKUNEN_CODE_O ASC")
                    sql.Append(",CLASS_CODE_O ASC")
                    sql.Append(",SEITO_NO_O ASC")
                Case "1"      '入学年度、通番
                    sql.Append(" ORDER BY NENDO_O ASC")
                    sql.Append(",TUUBAN_O ASC")
                Case "2"      '生徒名のアイウエオ順
                    sql.Append(" ORDER BY SEITO_KNAME_O ASC")
            End Select

            If oraReader.DataReader(sql) = True Then

                'タイトル出力
                strLine = "削除フラグ,学校コード,年度,通番,生徒名カナ,生徒名,学年,旧クラス,旧生徒番号,新クラス,新生徒番号"
                strLine += vbCrLf

                Do Until oraReader.EOF
                    '各項目取得
                    strExport_GakkouCode = Trim(oraReader.GetString("GAKKOU_CODE_O"))
                    strExport_Nendo = Trim(oraReader.GetString("NENDO_O"))
                    intExport_Tuban = oraReader.GetString("TUUBAN_O")
                    strExport_KName = Trim(oraReader.GetString("SEITO_KNAME_O"))
                    If IsDBNull(oraReader.GetString("SEITO_NNAME_O")) = False Then
                        strExport_NName = Trim(oraReader.GetString("SEITO_NNAME_O"))
                    Else
                        strExport_NName = ""
                    End If
                    intExport_Gakunen = oraReader.GetString("GAKUNEN_CODE_O")
                    intExport_Class = oraReader.GetString("CLASS_CODE_O")
                    strExport_SeitoNo = Trim(oraReader.GetString("SEITO_NO_O"))

                    'ＣＳＶファイル書込み
                    strLine += "0" & ","
                    strLine += strExport_GakkouCode & ","
                    strLine += strExport_Nendo & ","
                    strLine += intExport_Tuban & ","
                    strLine += strExport_KName & ","
                    strLine += strExport_NName & ","
                    strLine += intExport_Gakunen & ","
                    strLine += intExport_Class & ","
                    strLine += strExport_SeitoNo & ","
                    strLine += " " & ","
                    strLine += " "
                    strLine += vbCrLf

                    oraReader.NextRead()

                Loop

                wrt = New System.IO.StreamWriter(csvFileName, False, System.Text.Encoding.Default)

                wrt.Write(strLine)

                wrt.Close()

            Else
                MessageBox.Show(G_MSG0042W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                oraReader.Close()
                ChDir(strDIR)
                Return
            End If

            oraReader.Close()

            ChDir(strDIR)

            MessageBox.Show(String.Format(MSG0016I, "移出"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

            txtGAKKOU_CODE.Focus()
            txtGAKKOU_CODE.SelectionStart = 0
            txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(移出)終了", "成功", "")
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
            If Not wrt Is Nothing Then wrt.Close()
        End Try

    End Sub

    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click

        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim Param As String
        Try
            '印刷ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            MainDB = New MyOracle

            LW.ToriCode = txtGAKKOU_CODE.Text

            '共通チェック
            If PFUC_COMMON_CHK() = False Then
                Exit Sub
            End If

            '2006/10/19　生徒が1人も登録されていない場合、メッセージを表示し、処理を中断する
            If PFUNC_SEITO_CHECK() = False Then
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "進級リスト"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me
            Dim nRet As Integer
            '月間スケジュール表印刷 
            'パラメータ設定：ログイン名,学校コード,学年,帳票ソート順
            Param = GCom.GetUserID & "," & txtGAKKOU_CODE.Text & "," & txtGAKUNEN.Text & "," & strExport_Sort

            nRet = ExeRepo.ExecReport("KFGP011.EXE", Param)
            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, "進級リスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "進級リスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
            End Select
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
            MainDB.Rollback()
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            If OraReader IsNot Nothing Then OraReader.Close()
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '実行ボタン

        Try
            MainDB = New MyOracle

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            '共通チェック
            If PFUC_COMMON_CHK() = False Then
                Exit Sub
            End If

            '2006/10/19　生徒が1人も登録されていない場合、メッセージを表示し、処理を中断する
            If PFUNC_SEITO_CHECK() = False Then
                Exit Sub
            End If

            STR_クラス替学校コード = txtGAKKOU_CODE.Text
            STR_クラス替学校名 = lab学校名.Text
            STR_クラス替学年コード = txtGAKUNEN.Text
            STR_クラス替学年名 = labGAKUNEN.Text

            Dim KFGNENJ021 As New GAKKOU.KFGNENJ021
            'KFGNENJ021.ShowDialog(Me)
            Call CASTCommon.ShowFORM(GCom.GetUserID, CType(KFGNENJ021, Form), Me)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
            If Not MainDB Is Nothing Then MainDB.Close()
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
    Private Function PFUC_GAKNAME_GET() As Integer
        '学校名の設定
        PFUC_GAKNAME_GET = 0

        STR_SQL = ""
        STR_SQL += "SELECT * FROM KZFMAST.GAKMAST1 WHERE"
        STR_SQL += "     GAKKOU_CODE_G  = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        '学校マスタ１存在チェック
        If GFUNC_ISEXIST(STR_SQL) = False Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            lab学校名.Text = ""
            txtGAKKOU_CODE.Focus()
            PFUC_GAKNAME_GET = 1
            Exit Function
        End If
        OBJ_COMMAND.Connection = OBJ_CONNECTION
        OBJ_COMMAND.CommandText = STR_SQL
        OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()
        OBJ_DATAREADER.Read()

        lab学校名.Text = OBJ_DATAREADER.Item("GAKKOU_NNAME_G")

        OBJ_DATAREADER.Close()


    End Function
    Private Function PFUC_GAKNENNAME_GET() As Integer
        '学年名の設定
        PFUC_GAKNENNAME_GET = 0

        STR_SQL = ""
        STR_SQL += "SELECT * FROM KZFMAST.GAKMAST1 WHERE"
        STR_SQL += "     GAKKOU_CODE_G  = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        STR_SQL += " AND GAKUNEN_CODE_G = " & "'" & txtGAKUNEN.Text & "'"
        '学校マスタ１存在チェック
        If GFUNC_ISEXIST(STR_SQL) = False Then
            MessageBox.Show(String.Format(G_MSG0033W, txtGAKUNEN.Text), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            labGAKUNEN.Text = ""
            txtGAKUNEN.Focus()
            PFUC_GAKNENNAME_GET = 1
            Exit Function
        End If
        OBJ_COMMAND.Connection = OBJ_CONNECTION
        OBJ_COMMAND.CommandText = STR_SQL
        OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()
        OBJ_DATAREADER.Read()

        labGAKUNEN.Text = OBJ_DATAREADER.Item("GAKUNEN_NAME_G")

        OBJ_DATAREADER.Close()


    End Function
    Private Function PFUC_GAKMAST2_GET() As Integer
        '学校マスタ２の取得
        PFUC_GAKMAST2_GET = 0

        STR_SQL = ""
        STR_SQL += "SELECT * FROM KZFMAST.GAKMAST2 WHERE"
        STR_SQL += "     GAKKOU_CODE_T  = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        '学校マスタ２存在チェック
        If GFUNC_ISEXIST(STR_SQL) = False Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            PFUC_GAKMAST2_GET = 1
            Exit Function
        End If
        OBJ_COMMAND.Connection = OBJ_CONNECTION
        OBJ_COMMAND.CommandText = STR_SQL
        OBJ_DATAREADER = OBJ_COMMAND.ExecuteReader()

        OBJ_DATAREADER.Read()

        '使用学年数の格納
        intSiyouGakunen = OBJ_DATAREADER.Item("SIYOU_GAKUNEN_T")
        '帳票ソート順
        If IsDBNull(OBJ_DATAREADER.Item("MEISAI_OUT_T")) = False Then
            strExport_Sort = OBJ_DATAREADER.Item("MEISAI_OUT_T")
        Else
            '初期値設定
            strExport_Sort = "0"
        End If
        OBJ_DATAREADER.Close()

    End Function
    Private Function PFUC_COMMON_CHK() As Boolean

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Return False
        End If

        '学年
        If Trim(txtGAKUNEN.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学年コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKUNEN.Focus()
            Return False
        End If

        If CInt(txtGAKUNEN.Text) > intSiyouGakunen Then
            MessageBox.Show(String.Format(G_MSG0033W, txtGAKUNEN.Text), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKUNEN.Focus()
            Return False
        End If

        Return True

    End Function
    Private Function PFUC_CSVINPUT_CHK() As Integer

        'ＣＳＶ形式テキストの読込みとチェック
        Dim J As Integer
        Dim SEQNO As Integer
        Dim chkSEITO As Boolean = False
        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUC_CSVINPUT_CHK = 0

        SEQNO = 0

        Do While Not EOF(fnbr)
            'ヘッダタイトルの読捨て
            For i As Integer = 1 To 11 Step 1
                Input(fnbr, strLine)
            Next i

            SEQNO = SEQNO + 1
            Exit Do
        Loop

        Do While Not EOF(fnbr)
            For i As Integer = 1 To 11 Step 1
                Input(fnbr, strLine)
                Select Case i
                    Case 1
                        strImport_SakujoFLG = strLine
                    Case 2
                        strImport_GakkouCode = strLine
                    Case 3
                        strImport_Nendo = strLine
                    Case 4
                        strImport_Tuban = strLine
                    Case 5
                        strImport_KName = strLine
                    Case 6
                        strImport_NName = strLine
                    Case 7
                        strImport_Gakunen = strLine
                    Case 8
                        strImport_Class = strLine
                    Case 9
                        strImport_SeitoNo = strLine
                    Case 10
                        strImport_NewClass = strLine
                    Case 11
                        strImport_NewSeitoNo = strLine
                        'Case 12
                        'strImport_Kaigyo = strLine
                End Select
            Next i

            SEQNO = SEQNO + 1

            '入力項目のチェク
            ''削除フラグ
            If strImport_SakujoFLG = "0" Or strImport_SakujoFLG = "1" Then
            Else
                strErrLog = "削除フラグエラー" & " SEQNO=" & SEQNO & " 削除フラグ＝" & strImport_SakujoFLG
                PFUC_ERRLOG_OUT()
                PFUC_CSVINPUT_CHK = 1
            End If


            ''学校マスタ１の検索
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)
            sql.Append("SELECT * FROM KZFMAST.GAKMAST1 WHERE")
            sql.Append("     GAKKOU_CODE_G  = " & "'" & Format(GCom.NzLong(strImport_GakkouCode), "0000000000") & "'")
            sql.Append("AND  GAKUNEN_CODE_G  = " & CInt(strImport_Gakunen)) '追加 2006/12/05

            '学校マスタ１存在チェック
            If oraReader.DataReader(sql) = False Then
                strErrLog = "学校コードエラー" & " SEQNO=" & SEQNO & "　学校コード＝" & strImport_GakkouCode
                PFUC_ERRLOG_OUT()
                PFUC_CSVINPUT_CHK = 1
            Else
                If Trim(strImport_NewClass) <> "" Then

                    'クラスコード
                    J = 0
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE101_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE101_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE102_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE102_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE103_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE103_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE104_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE104_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE105_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE105_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE106_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE106_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE107_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE107_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE108_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE108_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE109_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE109_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE110_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE110_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE111_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE111_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE112_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE112_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE113_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE113_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE114_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE114_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE115_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE115_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE116_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE116_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE117_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE117_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE118_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE118_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE119_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE119_G")) Then
                                J = 1
                            End If
                    End Select
                    Select Case IsDBNull(oraReader.GetString("CLASS_CODE120_G"))
                        Case False
                            If strImport_NewClass = CStr(oraReader.GetString("CLASS_CODE120_G")) Then
                                J = 1
                            End If
                    End Select

                    '新クラスが学校マスタ１にあるか？
                    If J = 0 Then
                        strErrLog = "新クラス未登録エラー" & " SEQNO=" & SEQNO & " 新クラス＝" & strImport_NewClass
                        PFUC_ERRLOG_OUT()
                        PFUC_CSVINPUT_CHK = 1
                    End If
                End If

            End If
            oraReader.Close()

            '生徒マスタの検索
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)
            sql.Append("SELECT * FROM KZFMAST.SEITOMAST WHERE")
            sql.Append("     GAKKOU_CODE_O   = " & "'" & Format(GCom.NzLong(strImport_GakkouCode), "0000000000") & "'")
            sql.Append(" AND NENDO_O         = " & "'" & strImport_Nendo & "'")
            sql.Append(" AND TUUBAN_O        = " & CInt(strImport_Tuban))
            sql.Append(" AND GAKUNEN_CODE_O  = " & CInt(strImport_Gakunen))
            If oraReader.DataReader(sql) = False Then
                strErrLog = "生徒マスタに存在しません" & " SEQNO=" & SEQNO
                PFUC_ERRLOG_OUT()
                PFUC_CSVINPUT_CHK = 1
            End If
            oraReader.Close()
            chkSEITO = True
        Loop

        'ＣＳＶファイルに1件も入力されていない場合、移入できない
        If chkSEITO = False Then
            PFUC_CSVINPUT_CHK = 1
            MessageBox.Show(G_MSG0043W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If

    End Function
    Private Function PFUC_ERRLOG_OUT() As Integer
        Dim Obj_StreamWriter As StreamWriter

        PFUC_ERRLOG_OUT = 0

        Try

            'ログの書き込み
            Obj_StreamWriter = New StreamWriter(KobetuLogFileName, _
                                                True, _
                                                Encoding.GetEncoding("Shift_JIS"))
        Catch ex As Exception

            PFUC_ERRLOG_OUT = 1
            Exit Function

        End Try

        Obj_StreamWriter.WriteLine(strErrLog)
        Obj_StreamWriter.Flush()
        Obj_StreamWriter.Close()
    End Function
    Private Function PFUC_SEITOMAST_KOUSIN() As Integer
        '生徒マスタ更新（新クラス、新生徒番号）

        PFUC_SEITOMAST_KOUSIN = 0

        Do While Not EOF(fnbr)
            'ヘッダタイトルの読捨て
            For i As Integer = 1 To 11 Step 1
                Input(fnbr, strLine)
            Next i
            Exit Do
        Loop

        Do While Not EOF(fnbr)
            'ＣＳＶ１行読込み
            For i As Integer = 1 To 11 Step 1
                Input(fnbr, strLine)
                Select Case i
                    Case 1
                        strImport_SakujoFLG = Trim(strLine)
                    Case 2
                        strImport_GakkouCode = Trim(strLine)
                    Case 3
                        strImport_Nendo = Trim(strLine)
                    Case 4
                        strImport_Tuban = Trim(strLine)
                    Case 5
                        strImport_KName = Trim(strLine)
                    Case 6
                        strImport_NName = Trim(strLine)
                    Case 7
                        strImport_Gakunen = Trim(strLine)
                    Case 8
                        strImport_Class = Trim(strLine)
                    Case 9
                        strImport_SeitoNo = Trim(strLine)
                    Case 10
                        strImport_NewClass = Trim(strLine)
                    Case 11
                        strImport_NewSeitoNo = Trim(strLine)
                        'Case 12
                        'strImport_Kaigyo = strLine
                End Select
            Next i

            If strImport_SakujoFLG = "1" Then
                '生徒マスタ削除処理
                If PFUC_SEITOMAST_DEL() = False Then
                    Return False
                End If
            Else
                '生徒マスタ更新処理
                If PFUC_SEITOMAST_UPD() = False Then
                    Return False
                End If
            End If

        Loop

    End Function

    Private Function PFUC_SEITOMAST_UPD() As Boolean

        Dim sql As New StringBuilder(128)

        '新クラス、新生徒番号
        If Trim(strImport_NewClass) = "" And Trim(strImport_NewSeitoNo) = "" Then
            Return True
        Else
            '新クラス、新生徒番号の入力あり
            If Trim(strImport_NewClass) <> "" And Trim(strImport_NewSeitoNo) <> "" Then
                sql.Append("UPDATE  KZFMAST.SEITOMAST SET ")
                sql.Append("   CLASS_CODE_O     = " & CInt(strImport_NewClass))
                sql.Append("  ,SEITO_NO_O       = " & "'" & Format(CInt(strImport_NewSeitoNo), "0000000") & "'")
            Else
                '新クラスのみ入力あり
                If Trim(strImport_NewClass) <> "" And Trim(strImport_NewSeitoNo) = "" Then
                    sql.Append("UPDATE  KZFMAST.SEITOMAST SET ")
                    sql.Append("   CLASS_CODE_O     = " & CInt(strImport_NewClass))
                Else
                    '新生徒番号のみ入力あり
                    If Trim(strImport_NewClass) = "" And Trim(strImport_NewSeitoNo) <> "" Then
                        sql.Append("UPDATE  KZFMAST.SEITOMAST SET ")
                        sql.Append("   SEITO_NO_O       = " & "'" & Format(CInt(strImport_NewSeitoNo), "0000000") & "'")
                    End If
                End If
            End If
        End If
        sql.Append("  ,KOUSIN_DATE_O    = " & "'" & Format(Now, "yyyy") & Format(Now, "MM") & Format(Now, "dd") & "'")
        sql.Append("  WHERE ")
        sql.Append("     GAKKOU_CODE_O  = " & "'" & Format(GCom.NzLong(strImport_GakkouCode), "0000000000") & "'")
        sql.Append(" AND GAKUNEN_CODE_O = " & CInt(strImport_Gakunen))
        sql.Append(" AND NENDO_O   = '" & CStr(strImport_Nendo) & "'")
        sql.Append(" AND TUUBAN_O   　= " & CInt(strImport_Tuban))

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show(String.Format(MSG0002E, "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function

    Private Function PFUC_SEITOMAST_DEL() As Boolean
        '生徒マスタの削除

        Dim sql As New StringBuilder(128)

        sql.Append("DELETE  FROM KZFMAST.SEITOMAST WHERE")
        sql.Append("     GAKKOU_CODE_O   = " & "'" & Format(GCom.NzLong(strImport_GakkouCode), "0000000000") & "'")
        sql.Append(" AND GAKUNEN_CODE_O  = " & CInt(strImport_Gakunen))
        sql.Append(" AND NENDO_O   = '" & CStr(strImport_Nendo) & "'")
        sql.Append(" AND TUUBAN_O   　= " & CInt(strImport_Tuban))

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show(String.Format(MSG0002E, "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True

    End Function

    Private Function PFUNC_SEITO_CHECK() As Boolean
        '2006/10/19　追加：入力された学校・学年の生徒が存在するかどうかチェック
        '2007/08/16　追加：通常と進学生徒の選択によってチェック内容を変更

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        PFUNC_SEITO_CHECK = False

        '生徒マスタの読込み
        sql.Append("SELECT * FROM KZFMAST.SEITOMAST WHERE")
        sql.Append(" GAKKOU_CODE_O = " & "'" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
        sql.Append(" AND GAKUNEN_CODE_O = " & CInt(txtGAKUNEN.Text))
        sql.Append(" AND TUKI_NO_O ='04'")

        If oraReader.DataReader(sql) = True Then
            oraReader.Close()
            Return True
        Else
            oraReader.Close()
            MessageBox.Show(G_MSG0044W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Return False
        End If

    End Function
#End Region

    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '学校コード
        If Trim(txtGAKKOU_CODE.Text) <> "" Then

            '学校名の取得
            If PFUC_GAKNAME_GET() <> 0 Then
                Exit Sub
            End If

            '学年コードが入力されていた場合、学年名取得
            If Trim(txtGAKUNEN.Text) <> "" Then
                If PFUC_GAKNENNAME_GET() <> 0 Then
                    Exit Sub
                End If
            End If

            '帳票ソート順の取得
            If PFUC_GAKMAST2_GET() <> 0 Then
                Exit Sub
            End If
        Else
            lab学校名.Text = ""
            labGAKUNEN.Text = ""
        End If

    End Sub
    Private Sub txtGAKUNEN_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKUNEN.Validating
        '学年コード
        If Trim(txtGAKUNEN.Text) <> "" Then
            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                If PFUC_GAKNENNAME_GET() <> 0 Then
                    Exit Sub
                End If
            End If
        Else
            labGAKUNEN.Text = ""
        End If

    End Sub

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 タスク）西野 DEL 標準版修正（カナ検索のクリア対応）----------------- END

        '学校検索
        Call GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName)

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then
            Exit Sub
        End If

        lab学校名.Text = cmbGakkouName.Text

        '学校検索後の学校コード設定
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
