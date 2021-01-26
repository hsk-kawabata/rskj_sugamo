Option Explicit On 
Option Strict Off

Imports System.Text
Imports CASTCommon
Public Class KFGPRNT090
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 新入生マスタ登録チェックリスト印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Private STR学校名 As String
    '2006/10/11 帳票のソート機能追加
    Dim STR帳票ソート順 As String
    Dim STR学校コード As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT090", "新入生マスタ登録チェックリスト印刷画面")
    Private Const msgTitle As String = "新入生マスタ登録チェックリスト印刷画面(KFGPRNT090)"
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGPRNT090_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '2012/04/10 saitou 標準修正 ADD ---------------------------------------->>>>
            '処理名の設定追加
            STR_SYORI_NAME = "新入生マスタ登録チェックリスト印刷画面"
            '2012/04/10 saitou 標準修正 ADD ----------------------------------------<<<<
            MainLOG = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbGAKKOUNAME)")
                MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '入力ボタン制御
            btnPrnt.Enabled = True
            btnEnd.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
#End Region

#Region " Button Click "
    '****************************
    'Button Click
    '****************************
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Dim ExeRepo As New CAstReports.ClsExecute
        ExeRepo.SetOwner = Me
        Dim nRet As Integer
        Dim Param As String
        Try
            '印刷ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            MainDB = New MyOracle

            '入力チェック
            If PFUNC_COMMON_CHECK() = False Then
                Exit Sub
            End If

            LW.ToriCode = Trim(txtGAKKOU_CODE.Text)
            LW.FuriDate = STR_FURIKAE_DATE(1)

            '新入生マスタ存在チェック 2006/10/10
            If fn_select_SEITOMAST2() = False Then
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            '全学校レコードが対象
            SQL.Append("SELECT DISTINCT GAKKOU_CODE_O FROM SEITOMASTVIEW2 ")
            SQL.Append(" WHERE TUKI_NO_O = " & SQ(txtSEIKYUTUKI.Text))
            SQL.Append(" AND (KOUSIN_DATE_O = " & SQ(STR_FURIKAE_DATE(1)) & " OR SAKUSEI_DATE_O = " & SQ(STR_FURIKAE_DATE(1)) & ")")
            If txtGAKKOU_CODE.Text.Trim <> "9999999999" Then
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(txtGAKKOU_CODE.Text))
            End If
            SQL.Append(" AND TUKI_NO_O = " & SQ(txtSEIKYUTUKI.Text))
            SQL.Append(" ORDER BY GAKKOU_CODE_O ASC")

            'スケジュール存在チェック
            OraReader = New MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("対象データが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "新入生マスタ登録チェックリスト"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim intINSATU_FLG2 As Integer = 0
            While OraReader.EOF = False
                STR学校コード = OraReader.GetString("GAKKOU_CODE_O")
                '帳票ソート順の取得
                If PFUNC_GAKNAME_GET(False) = False Then
                    STR帳票ソート順 = "0"
                End If
                'ログインID,学校コード,更新日,月,ソート順,印刷区分(1:生徒 2:新入生)
                Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & txtSEIKYUTUKI.Text & "," & STR帳票ソート順 & ",2"
                nRet = ExeRepo.ExecReport("KFGP027.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "新入生マスタ登録チェックリスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                OraReader.NextRead()
            End While
            MessageBox.Show(String.Format(MSG0014I, "新入生マスタ登録チェックリスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
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
    Private Function PFUNC_GAKNAME_GET(Optional ByVal NameChg As Boolean = True) As Boolean

        '学校名の設定
        PFUNC_GAKNAME_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            If Trim(STR学校コード) = "9999999999" Then
                lab学校名.Text = ""
            Else
                SQL.Append(" SELECT ")
                SQL.Append(" GAKMAST1.*")
                SQL.Append(",MEISAI_OUT_T")
                SQL.Append(" FROM GAKMAST1,GAKMAST2")
                SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")
                SQL.Append(" AND GAKKOU_CODE_G = " & SQ(STR学校コード))

                If OraReader.DataReader(SQL) = False Then

                    MessageBox.Show("学校マスタに登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                    lab学校名.Text = ""
                    STR帳票ソート順 = 0

                    Exit Function
                End If

                If NameChg Then lab学校名.Text = OraReader.GetString("GAKKOU_NNAME_G")
                STR帳票ソート順 = OraReader.GetInt("MEISAI_OUT_T")

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校検索)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        PFUNC_GAKNAME_GET = True

    End Function
    Private Function PFUNC_COMMON_CHECK() As Boolean

        PFUNC_COMMON_CHECK = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            '学校コード必須チェック
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
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

            '月必須チェック
            If txtSEIKYUTUKI.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSEIKYUTUKI.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(txtSEIKYUTUKI.Text.Trim) < 1 OrElse GCom.NzInt(txtSEIKYUTUKI.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSEIKYUTUKI.Focus()
                Return False
            End If

            OraReader = New MyOracleReader(MainDB)

            '学校コード
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab学校名.Text = "全学校対象"
            Else
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(txtGAKKOU_CODE.Text))

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("学校マスタに登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lab学校名.Text = ""
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                STR学校名 = OraReader.GetString("GAKKOU_NNAME_G")

                lab学校名.Text = STR学校名

            End If

            SQL = New StringBuilder
            SQL.Append(" SELECT * FROM GAKMAST2")
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(txtGAKKOU_CODE.Text))
            End If

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("学校マスタ2に登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Function
            End If
            STR帳票ソート順 = OraReader.GetString("MEISAI_OUT_T")

            STR_FURIKAE_DATE(0) = Trim(txtFuriDateY.Text) & "/" & Trim(txtFuriDateM.Text) & "/" & Trim(txtFuriDateD.Text)
            STR_FURIKAE_DATE(1) = Trim(txtFuriDateY.Text) & Format(GCom.NzInt(txtFuriDateM.Text), "00") & Format(GCom.NzInt(txtFuriDateD.Text), "00")
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try

        PFUNC_COMMON_CHECK = True

    End Function
    '追加 2006/10/10
    Function fn_select_SEITOMAST2() As Boolean
        Dim intRECORD_COUNT As Integer

        fn_select_SEITOMAST2 = False

        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append("SELECT COUNT(*) COUNTER FROM SEITOMASTVIEW2 ")
            SQL.Append(" WHERE TUKI_NO_O = " & SQ(txtSEIKYUTUKI.Text))
            SQL.Append(" AND (KOUSIN_DATE_O = " & SQ(STR_FURIKAE_DATE(1)) & " OR SAKUSEI_DATE_O = " & SQ(STR_FURIKAE_DATE(1)) & ")")
            If txtGAKKOU_CODE.Text.Trim <> "9999999999" Then
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(txtGAKKOU_CODE.Text))
            End If

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            '-----------------------------------------
            'レコードの件数取得
            '-----------------------------------------
            intRECORD_COUNT = OraReader.GetInt64("COUNTER")
            If intRECORD_COUNT = 0 Then
                MessageBox.Show("印刷対象データが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(生徒検索)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
        fn_select_SEITOMAST2 = True
    End Function

    Private Function PFUC_SQLQuery_新入生マスタ() As String
        '2006/10/11 帳票の出力順を指定するため、JYOKENではなくSQLを指定するように変更
        Dim SSQL As String

        PFUC_SQLQuery_新入生マスタ = ""


        SSQL = " SELECT "
        SSQL = SSQL & " SEITOMAST2.SEIBETU_O, "
        SSQL = SSQL & " SEITOMAST2.FURIKAE_O, "
        SSQL = SSQL & " SEITOMAST2.KAMOKU_O, "
        SSQL = SSQL & " SEITOMAST2.KAIYAKU_FLG_O, "
        SSQL = SSQL & " SEITOMAST2.GAKKOU_CODE_O, "
        SSQL = SSQL & " SEITOMAST2.NENDO_O, "
        SSQL = SSQL & " SEITOMAST2.GAKUNEN_CODE_O, "
        SSQL = SSQL & " SEITOMAST2.TUUBAN_O, "
        SSQL = SSQL & " SEITOMAST2.SEITO_NO_O, "
        SSQL = SSQL & " SEITOMAST2.SEITO_NNAME_O, "
        SSQL = SSQL & " GAKMAST1.GAKKOU_NNAME_G, "
        SSQL = SSQL & " SEITOMAST2.SEITO_KNAME_O, "
        SSQL = SSQL & " SEITOMAST2.MEIGI_KNAME_O, "
        SSQL = SSQL & " SEITOMAST2.MEIGI_NNAME_O, "
        SSQL = SSQL & " SEITOMAST2.TKIN_NO_O, "
        SSQL = SSQL & " SEITOMAST2.TSIT_NO_O, "
        SSQL = SSQL & " TENMAST.KIN_NNAME_N, "
        SSQL = SSQL & " TENMAST.SIT_NNAME_N, "
        SSQL = SSQL & " SEITOMAST2.CLASS_CODE_O, "
        SSQL = SSQL & " SEITOMAST2.KEIYAKU_DENWA_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME01_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME02_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME03_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME04_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME05_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME06_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME07_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME08_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME09_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME10_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME11_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME12_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME13_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME14_H, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_NAME15_H, "
        SSQL = SSQL & " SEITOMAST2.KOUZA_O, "
        SSQL = SSQL & " SEITOMAST2.HIMOKU_ID_O, "
        SSQL = SSQL & " SEITOMAST2.TUKI_NO_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU01_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU02_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU03_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU04_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU05_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU06_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU07_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU08_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU09_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU10_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU11_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU12_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU13_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU14_O, "
        SSQL = SSQL & " SEITOMAST2.SEIKYU15_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU01_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU01_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU02_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU02_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU03_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU03_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU04_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU04_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU05_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU05_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU06_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU06_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU07_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU07_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU08_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU08_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU09_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU09_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU10_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU10_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU11_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU11_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU12_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU12_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU13_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU13_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU14_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU14_O, "
        SSQL = SSQL & " HIMOMAST.HIMOKU_KINGAKU15_H, "
        SSQL = SSQL & " SEITOMAST2.KINGAKU15_O, "
        SSQL = SSQL & " SEITOMAST2.SAKUSEI_DATE_O, "
        SSQL = SSQL & " SEITOMAST2.KOUSIN_DATE_O "
        SSQL = SSQL & " FROM   "
        SSQL = SSQL & " KZFMAST.SEITOMAST2 SEITOMAST2, "
        SSQL = SSQL & " KZFMAST.GAKMAST1 GAKMAST1, "
        SSQL = SSQL & " KZFMAST.TENMAST TENMAST, "
        SSQL = SSQL & " KZFMAST.HIMOMAST HIMOMAST "
        SSQL = SSQL & " WHERE  "
        SSQL = SSQL & " SEITOMAST2.GAKKOU_CODE_O=GAKMAST1.GAKKOU_CODE_G (+) AND "
        SSQL = SSQL & " ((SEITOMAST2.TKIN_NO_O=TENMAST.KIN_NO_N (+)) AND "
        SSQL = SSQL & " (SEITOMAST2.TSIT_NO_O=TENMAST.SIT_NO_N (+))) AND "
        SSQL = SSQL & " ((((SEITOMAST2.GAKKOU_CODE_O=HIMOMAST.GAKKOU_CODE_H (+))) AND "
        SSQL = SSQL & " (SEITOMAST2.HIMOKU_ID_O=HIMOMAST.HIMOKU_ID_H (+))) AND "
        SSQL = SSQL & " (SEITOMAST2.TUKI_NO_O=HIMOMAST.TUKI_NO_H (+))) AND "


        If Trim(STR学校コード) <> "9999999999" Then
            '生徒マスタ（学校コード、作成日、更新日）
            'レコード抽出設定
            '指定学校コード
            SSQL = SSQL & " SEITOMAST2.GAKKOU_CODE_O = '" & Trim(STR学校コード) & "' AND "
        Else
        End If
        '作成日
        SSQL = SSQL & "(SEITOMAST2.SAKUSEI_DATE_O ='" & STR_FURIKAE_DATE(1) & "'"
        '更新日
        SSQL = SSQL & " OR SEITOMAST2.KOUSIN_DATE_O ='" & STR_FURIKAE_DATE(1) & "')"
        '請求月
        SSQL = SSQL & " AND SEITOMAST2.TUKI_NO_O ='" & txtSEIKYUTUKI.Text & "'"

        SSQL = SSQL & " AND GAKMAST1.GAKUNEN_CODE_G = 1　"
        SSQL = SSQL & " AND HIMOMAST.GAKUNEN_CODE_H = 1"

        SSQL = SSQL & " ORDER BY "
        SSQL = SSQL & " SEITOMAST2.GAKKOU_CODE_O　　ASC, "
        Select Case STR帳票ソート順
            Case "0"
                SSQL = SSQL & " SEITOMAST2.GAKUNEN_CODE_O　ASC, "
                SSQL = SSQL & " SEITOMAST2.CLASS_CODE_O     ASC,"
                SSQL = SSQL & " SEITOMAST2.SEITO_NO_O       ASC,"
                SSQL = SSQL & " SEITOMAST2.NENDO_O         ASC,"
                SSQL = SSQL & " SEITOMAST2.TUUBAN_O         ASC"
            Case "1"
                SSQL = SSQL & " SEITOMAST2.GAKUNEN_CODE_O　ASC, "
                SSQL = SSQL & " SEITOMAST2.NENDO_O         ASC,"
                SSQL = SSQL & " SEITOMAST2.TUUBAN_O         ASC"
            Case Else
                SSQL = SSQL & " SEITOMAST2.GAKUNEN_CODE_O　ASC, "
                SSQL = SSQL & " SEITOMAST2.SEITO_KNAME_O   ASC,"
                SSQL = SSQL & " SEITOMAST2.NENDO_O         ASC,"
                SSQL = SSQL & " SEITOMAST2.TUUBAN_O         ASC"
        End Select
        PFUC_SQLQuery_新入生マスタ = SSQL

        'Debug.WriteLine("SSQL=" & SSQL)

    End Function

#End Region

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

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
        '学校名の取得
        lab学校名.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '学校コード
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '2010/11/09 学校コードゼロ埋め
            txtGAKKOU_CODE.Text = Trim(txtGAKKOU_CODE.Text).PadLeft(10, "0"c)
            '学校名の取得
            STR学校コード = Trim(txtGAKKOU_CODE.Text)
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If

    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

End Class
