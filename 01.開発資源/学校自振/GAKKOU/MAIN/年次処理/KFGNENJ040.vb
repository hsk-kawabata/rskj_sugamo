Option Explicit On 
Option Strict Off

Imports System.Text
Imports CASTCommon


Public Class KFGNENJ040
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 進級戻し処理
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

#Region " 共通変数定義 "
    Dim INT使用学年数 As Integer
    Dim INT最高学年数 As Integer
    Dim STR進級年度 As String
    Dim STR学校コード As String
    Dim STR入学年度 As String
    Dim INT通番 As Integer
    Dim INT学年 As Integer
    Dim INTクラス As Integer
    Dim STR生徒番号 As String
    Dim STR生徒氏名カナ As String
    Dim STR生徒氏名漢字 As String
    Dim STR性別 As String
    Dim STR金融機関コード As String
    Dim STR支店コード As String
    Dim STR科目 As String
    Dim STR口座番号 As String
    Dim STR名義人カナ As String
    Dim STR名義人漢字 As String
    Dim STR振替方法 As String
    Dim STR契約住所漢字 As String
    Dim STR契約電話番号 As String
    Dim STR解約区分 As String
    Dim STR進級区分 As String
    Dim STR費目ＩＤ As String
    Dim INT長子_有無フラグ As Integer
    Dim STR長子_入学年度 As String
    Dim INT長子_通番 As Integer
    Dim INT長子_学年 As Integer
    Dim INT長子_クラス As Integer
    Dim STR長子_生徒番号 As String
    Dim STR請求月 As String
    Dim STR費目１請求方法 As String
    Dim LNG費目１請求金額 As Long
    Dim STR費目２請求方法 As String
    Dim LNG費目２請求金額 As Long
    Dim STR費目３請求方法 As String
    Dim LNG費目３請求金額 As Long
    Dim STR費目４請求方法 As String
    Dim LNG費目４請求金額 As Long
    Dim STR費目５請求方法 As String
    Dim LNG費目５請求金額 As Long
    Dim STR費目６請求方法 As String
    Dim LNG費目６請求金額 As Long
    Dim STR費目７請求方法 As String
    Dim LNG費目７請求金額 As Long
    Dim STR費目８請求方法 As String
    Dim LNG費目８請求金額 As Long
    Dim STR費目９請求方法 As String
    Dim LNG費目９請求金額 As Long
    Dim STR費目１０請求方法 As String
    Dim LNG費目１０請求金額 As Long
    Dim STR費目１１請求方法 As String
    Dim LNG費目１１請求金額 As Long
    Dim STR費目１２請求方法 As String
    Dim LNG費目１２請求金額 As Long
    Dim STR費目１３請求方法 As String
    Dim LNG費目１３請求金額 As Long
    Dim STR費目１４請求方法 As String
    Dim LNG費目１４請求金額 As Long
    Dim STR費目１５請求方法 As String
    Dim LNG費目１５請求金額 As Long
    Dim STR作成日付 As String
    Dim STR更新日付 As String
    Dim STR処理名 As String
#End Region

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGNENJ040", "進級戻し処理画面")
    Private Const msgTitle As String = "進級戻し処理画面(KFGNENJ040)"

    '2010/10/21 システム日付より過去年度の場合、進級不可とする--------------------------------
    'タイムスタンプ取得
    Private mMatchingDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")  '現在日付
    '-----------------------------------------------------------------------------------------

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGNENJ040_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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

            txtNendo.Text = (CInt(Mid(lblDate.Text, 1, 4)) - 1).ToString("0000")

            '入力ボタン制御
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
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim MainDB As New MyOracle
        Dim oraReader As MyOracleReader = Nothing

        Try

            '入力値チェック
            If PFUNC_Nyuryoku_Check(MainDB) = False Then
                Return
            End If

            If MessageBox.Show(String.Format(MSG0015I, "進級戻し"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If

            '入力学校コードのみ対象
            LW.ToriCode = txtGAKKOU_CODE.Text

            'トランザクション開始
            MainDB.BeginTrans()
            MainLOG.Write("進級メイン処理", "成功", "開始")

            '進級戻し処理実行
            If Not PFUNC_RESTORE(txtGAKKOU_CODE.Text, MainDB) Then
                MainLOG.Write("進級戻し処理", "失敗", "")
                MainDB.Rollback()
                Return
            End If

            'トランザクション終了（ＣＯＭＭＩＴ）
            MainDB.Commit()
            MainLOG.Write("進級戻し処理", "成功", "コミット")

            MessageBox.Show(String.Format(MSG0016I, "進級戻し"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)例外エラー", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '終了ボタン
        Me.Close()

    End Sub
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET() As Boolean

        Dim sGakkkou_Name As String = ""
        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        '学校名の設定
        sql.Append(" SELECT GAKKOU_NNAME_G FROM GAKMAST1")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_G ='" & txtGAKKOU_CODE.Text & "'")

        If oraReader.DataReader(sql) = True Then
            sGakkkou_Name = oraReader.GetString("GAKKOU_NNAME_G")
        End If
        oraReader.Close()

        '学校マスタ１存在チェック
        If sGakkkou_Name = "" Then
            MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            lab学校名.Text = ""
            txtGAKKOU_CODE.Focus()
            Return False
        End If

        lab学校名.Text = sGakkkou_Name

        Return True

    End Function

    Private Sub PFUNC_GET_NENDO()

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        '進級用学校マスタ２の取得
        sql.Append(" SELECT * FROM SINQBK_GAKMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_T ='" & txtGAKKOU_CODE.Text & "'")

        If oraReader.DataReader(sql) = False Then
            txtNendo.Text = ""
        Else

            '進級年度
            If IsDBNull(oraReader.GetString("SINKYU_NENDO_T")) = False Then
                txtNendo.Text = oraReader.GetString("SINKYU_NENDO_T")
            Else
                txtNendo.Text = ""
            End If

        End If
        oraReader.Close()

    End Sub

    Private Function PFUNC_Nyuryoku_Check(ByVal db As MyOracle) As Boolean
        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtGAKKOU_CODE.Focus()
            Return False
        Else
            '学校マスタ存在チェック
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(db)
            Try

                sql.Append("SELECT *")
                sql.Append(" FROM GAKMAST2")
                sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

                If oraReader.DataReader(sql) = False Then
                    MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Return False
                End If
            Catch ex As Exception
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_Nyuryoku_Check)学校マスタ存在チェック", "失敗", ex.ToString)
                Return False
            Finally
                If Not oraReader Is Nothing Then
                    oraReader.Close()
                    oraReader = Nothing
                End If
            End Try

        End If

        If (Trim(txtNendo.Text)) = "" Then
            MessageBox.Show(String.Format(MSG0285W, "退避年度"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtNendo.Focus()
            Return False
        Else
            '退避年度チェック
            sql.Length = 0
            oraReader = New MyOracleReader(db)
            Try

                sql.Append("SELECT *")
                sql.Append(" FROM SINQBK_GAKMAST2")
                sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
                sql.Append(" AND   SINKYU_NENDO_T = '" & txtNendo.Text.Trim.PadLeft(txtNendo.MaxLength, "0"c) & "'")

                If oraReader.DataReader(sql) = False Then
                    MessageBox.Show(G_MSG0107W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Return False
                End If
            Catch ex As Exception
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_Nyuryoku_Check)退避年度チェック", "失敗", ex.ToString)
                Return False
            Finally
                If Not oraReader Is Nothing Then
                    oraReader.Close()
                    oraReader = Nothing
                End If
            End Try
        End If

        Return True

    End Function

    ''' <summary>
    ''' 指定された学校の進級前情報を退避する
    ''' </summary>
    ''' <param name="GAKKOU_CODE">対象の学校コード</param>
    ''' <param name="db">DB</param>
    ''' <returns>TRUE:退避成功 FALSE:退避失敗</returns>
    ''' <remarks></remarks>
    Private Function PFUNC_RESTORE(ByVal GAKKOU_CODE As String, ByRef db As MyOracle) As Boolean
        Dim TARGET_TABLE As String = ""
        Dim SINQBK_TABLE As String = ""
        Dim KEY_NAME As String = ""

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_RESTORE)開始", "成功", "学校コード：" & GAKKOU_CODE)

        '-----------------------------------------------
        ' 学校マスタ２退避処理
        '-----------------------------------------------
        TARGET_TABLE = "GAKMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_T"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 費目マスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "HIMOMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_H"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 学校スケジュールマスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "G_SCHMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_S"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 学校他行スケジュールマスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "G_TAKOUSCHMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_U"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 生徒マスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "SEITOMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_O"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 新入生マスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "SEITOMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_O"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 学校明細マスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "G_MEIMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_M"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 学校エントリマスタ１退避処理
        '-----------------------------------------------
        TARGET_TABLE = "G_ENTMAST1"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_E"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 学校エントリマスタ２退避処理
        '-----------------------------------------------
        TARGET_TABLE = "G_ENTMAST2"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_E"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If
        '-----------------------------------------------
        ' 実績マスタ退避処理
        '-----------------------------------------------
        TARGET_TABLE = "JISSEKIMAST"
        SINQBK_TABLE = "SINQBK_" & TARGET_TABLE
        KEY_NAME = "GAKKOU_CODE_F"
        If Not PFUNC_SINQBKtoMAST(GAKKOU_CODE, TARGET_TABLE, SINQBK_TABLE, KEY_NAME, db) Then
            Return False
        End If

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_RESTORE)終了", "成功", "学校コード：" & GAKKOU_CODE)
        Return True
    End Function

    ''' <summary>
    ''' マスタ戻し処理
    ''' </summary>
    ''' <param name="GAKKOU_CODE">学校コード</param>
    ''' <param name="TARGET_TABLE">退避元テーブル名</param>
    ''' <param name="SINQBK_TABLE">退避先テーブル名</param>
    ''' <param name="KEY_NAME">キー名</param>
    ''' <param name="db">DB</param>
    ''' <returns>TRUE:退避成功 FALSE:退避失敗</returns>
    ''' <remarks></remarks>
    Private Function PFUNC_SINQBKtoMAST(ByVal GAKKOU_CODE As String, _
                                        ByVal TARGET_TABLE As String, _
                                        ByVal SINQBK_TABLE As String, _
                                        ByVal KEY_NAME As String, _
                                        ByRef db As MyOracle) As Boolean

        Dim SYORI_NAME As String = ""

        Dim SQL As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(db)
        Dim bRet As Boolean = False
        Dim DEL_CNT As Long = 0
        Dim INS_CNT As Long = 0
        Dim ResultMsg As String = ""

        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)開始", "成功", TARGET_TABLE)

        '-----------------------------------------------
        ' マスタテーブル内の削除
        '-----------------------------------------------
        Try
            SYORI_NAME = "マスタ削除"

            With SQL
                .Append("DELETE FROM " & TARGET_TABLE)
                .Append(" WHERE " & KEY_NAME & " = " & SQ(GAKKOU_CODE))
            End With

            DEL_CNT = db.ExecuteNonQuery(SQL)
            If DEL_CNT = -1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)終了", "失敗", SYORI_NAME & "処理失敗 学校コード：" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)終了", "失敗", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        '-----------------------------------------------
        ' マスタテーブルへの戻し
        '-----------------------------------------------
        Try
            SYORI_NAME = "マスタ戻し"

            SQL.Length = 0
            With SQL
                .Append("INSERT INTO " & TARGET_TABLE)
                .Append(" SELECT  * FROM " & SINQBK_TABLE & " WHERE " & KEY_NAME & " = " & SQ(GAKKOU_CODE))
            End With

            INS_CNT = db.ExecuteNonQuery(SQL)
            If INS_CNT = -1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)終了", "失敗", SYORI_NAME & "処理失敗 学校コード：" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)終了", "失敗", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        '-----------------------------------------------
        ' 進級用テーブル内の削除
        '-----------------------------------------------
        Try
            SYORI_NAME = "進級用マスタ削除"

            SQL.Length = 0
            With SQL
                .Append("DELETE FROM " & SINQBK_TABLE)
                .Append(" WHERE " & KEY_NAME & " = " & SQ(GAKKOU_CODE))
            End With

            If db.ExecuteNonQuery(SQL) = -1 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)終了", "失敗", SYORI_NAME & "処理失敗 学校コード：" & GAKKOU_CODE)
                MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtGAKKOU_CODE.Focus()
                Return False
            End If

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)終了", "失敗", ex.ToString)
            MessageBox.Show(String.Format(MSG0034E, SYORI_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        ResultMsg = String.Format("{0} マスタレコード削除件数={1}件 戻し件数={2}件", TARGET_TABLE, DEL_CNT.ToString, INS_CNT.ToString)
        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(PFUNC_SINQBKtoMAST)終了", "成功", ResultMsg)
        Return True
    End Function

#End Region

    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If

            '進級用学校マスタ２より年度を取得する
            PFUNC_GET_NENDO()

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

        '学校検索後の学校コード設定
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '学校名の取得
        lab学校名.Text = cmbGakkouName.Text

        '進級用学校マスタ２より年度を取得する
        PFUNC_GET_NENDO()

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub

    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
End Class
