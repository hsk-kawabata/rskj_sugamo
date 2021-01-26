Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text

Public Class KFGPRNT070
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 学校生徒名簿印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Private STR学校名 As String
    Private STR学年名 As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT070", "学校生徒名簿印刷画面")
    Private Const msgTitle As String = "学校生徒名簿印刷画面(KFGPRNT070)"
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
    Private Sub GFJPRNT0700G_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
        Dim Flg As String = ""
        Try
            '印刷ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            MainDB = New MyOracle

            '入力値チェック
            If PFUNC_NYUURYOKU_CHECK() = False Then
                Exit Sub
            End If

            LW.ToriCode = txtGAKKOU_CODE.Text


            SQL = New StringBuilder
            SQL.Append("SELECT DISTINCT GAKKOU_CODE_O")
            SQL.Append(" FROM SEITOMAST")
            SQL.Append(" WHERE KAIYAKU_FLG_O ='0'")
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) = "" Then
            ElseIf Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) <> "" Then
                SQL.Append(" AND GAKUNEN_CODE_O = " & Trim(txtGAKUNEN.Text))
            ElseIf Trim(txtGAKKOU_CODE.Text) <> "9999999999" And Trim(txtGAKUNEN.Text) = "" Then
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
            Else
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
                SQL.Append(" AND GAKUNEN_CODE_O = " & Trim(txtGAKUNEN.Text))
            End If
            SQL.Append(" ORDER BY GAKKOU_CODE_O")

            OraReader = New MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "学校生徒名簿"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Flg = IIf(chk生徒番号順.Checked, "1,", "0,") & IIf(chk通番順.Checked, "1,", "0,") & _
                  IIf(chkあいうえお順.Checked, "1,", "0,") & IIf(chk性別順.Checked, "1", "0")

            While OraReader.EOF = False
                'ログインID,学校コード,学年,月,費目,ソート順
                Param = GCom.GetUserID & "," & OraReader.GetString("GAKKOU_CODE_O") & "," & txtGAKUNEN.Text.Trim & "," & txtHIMOKU.Text & "," & Flg
                nRet = ExeRepo.ExecReport("KFGP026.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "学校生徒名簿"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                OraReader.NextRead()
            End While

            MessageBox.Show(String.Format(MSG0014I, "学校生徒名簿"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

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
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET() As Boolean

        PFUNC_GAKNAME_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            '学校名の設定
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab学校名.Text = "全学校対象"
            Else
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE")
                SQL.Append(" GAKKOU_CODE_G = " & SQ(txtGAKKOU_CODE.Text))

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
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校検索)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
        PFUNC_GAKNAME_GET = True

    End Function
    Private Function PFUNC_GAKUNENNAME_GET() As Boolean

        PFUNC_GAKUNENNAME_GET = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraDB As New MyOracle
        Try
            OraReader = New MyOracleReader(OraDB)
            '学校名の設定
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lblGAKUNEN_NAME.Text = ""
            Else
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G = " & SQ(txtGAKKOU_CODE.Text))
                SQL.Append(" AND GAKUNEN_CODE_G =" & txtGAKUNEN.Text)

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show("学年： " & txtGAKUNEN.Text & " は学校マスタに登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lblGAKUNEN_NAME.Text = ""
                    txtGAKUNEN.Text = ""
                    txtGAKUNEN.Focus()
                    Exit Function
                End If

                STR学年名 = OraReader.GetString("GAKUNEN_NAME_G")

                lblGAKUNEN_NAME.Text = STR学年名
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学年検索)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraDB Is Nothing Then OraDB.Close()
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
        PFUNC_GAKUNENNAME_GET = True

    End Function
    Private Function PFUC_SQLQuery_MAKE() As String
        Dim SSQL As String

        PFUC_SQLQuery_MAKE = ""

        SSQL = "SELECT "
        SSQL += "SEITOMAST.GAKKOU_CODE_O"
        SSQL += ",SEITOMAST.NENDO_O "
        SSQL += ",SEITOMAST.TUUBAN_O "
        SSQL += ",SEITOMAST.GAKUNEN_CODE_O "
        SSQL += ",SEITOMAST.CLASS_CODE_O "
        SSQL += ",SEITOMAST.SEITO_NO_O "
        SSQL += ",SEITOMAST.SEITO_KNAME_O "
        SSQL += ",SEITOMAST.SEITO_NNAME_O "
        SSQL += ",SEITOMAST.SEIBETU_O "
        SSQL += ",SEITOMAST.TKIN_NO_O "
        SSQL += ",SEITOMAST.TSIT_NO_O "
        SSQL += ",SEITOMAST.KAMOKU_O "
        SSQL += ",SEITOMAST.KOUZA_O "
        SSQL += ",SEITOMAST.MEIGI_KNAME_O "
        SSQL += ",SEITOMAST.FURIKAE_O "
        SSQL += ",SEITOMAST.KEIYAKU_NJYU_O "
        SSQL += ",SEITOMAST.KEIYAKU_DENWA_O "
        SSQL += ",SEITOMAST.HIMOKU_ID_O"
        SSQL += ",SEITOMAST.TUKI_NO_O"
        SSQL += ",SEITOMAST.SEIKYU01_O"
        SSQL += ",SEITOMAST.KINGAKU01_O"
        SSQL += ",SEITOMAST.SEIKYU02_O"
        SSQL += ",SEITOMAST.KINGAKU02_O"
        SSQL += ",SEITOMAST.SEIKYU03_O"
        SSQL += ",SEITOMAST.KINGAKU03_O"
        SSQL += ",SEITOMAST.SEIKYU04_O"
        SSQL += ",SEITOMAST.KINGAKU04_O"
        SSQL += ",SEITOMAST.SEIKYU05_O"
        SSQL += ",SEITOMAST.KINGAKU05_O"
        SSQL += ",SEITOMAST.SEIKYU06_O"
        SSQL += ",SEITOMAST.KINGAKU06_O"
        SSQL += ",SEITOMAST.SEIKYU07_O"
        SSQL += ",SEITOMAST.KINGAKU07_O"
        SSQL += ",SEITOMAST.SEIKYU08_O"
        SSQL += ",SEITOMAST.KINGAKU08_O"
        SSQL += ",SEITOMAST.SEIKYU09_O"
        SSQL += ",SEITOMAST.KINGAKU09_O"
        SSQL += ",SEITOMAST.SEIKYU10_O"
        SSQL += ",SEITOMAST.KINGAKU10_O"
        SSQL += ",SEITOMAST.SEIKYU11_O"
        SSQL += ",SEITOMAST.KINGAKU11_O"
        SSQL += ",SEITOMAST.SEIKYU12_O"
        SSQL += ",SEITOMAST.KINGAKU12_O"
        SSQL += ",SEITOMAST.SEIKYU13_O"
        SSQL += ",SEITOMAST.KINGAKU13_O"
        SSQL += ",SEITOMAST.SEIKYU14_O"
        SSQL += ",SEITOMAST.KINGAKU14_O"
        SSQL += ",SEITOMAST.SEIKYU15_O"
        SSQL += ",SEITOMAST.KINGAKU15_O"

        SSQL += ",GAKMAST1.GAKKOU_NNAME_G"
        SSQL += ",GAKMAST1.GAKUNEN_NNAME_G"
        SSQL += ",GAKMAST1.GAKKOU_CODE_G"

        SSQL += ",TENMAST.KIN_NO_N"
        SSQL += ",TENMAST.SIT_NO_N"
        SSQL += ",TENMAST.KIN_NNAME_N "
        SSQL += ",TENMAST.SIT_NNAME_N "

        SSQL += ",HIMOMAST.HIMOKU_ID_H"
        SSQL += ",HIMOMAST.HIMOKU_NAME01_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU01_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME02_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU02_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME03_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU03_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME04_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU04_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME05_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU05_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME06_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU06_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME07_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU07_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME08_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU08_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME09_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU09_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME10_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU10_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME11_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU11_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME12_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU12_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME13_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU13_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME14_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU14_H "
        SSQL += ",HIMOMAST.HIMOKU_NAME15_H "
        SSQL += ",HIMOMAST.HIMOKU_KINGAKU15_H "

        SSQL += ", NVL(SEITOMAST.GAKKOU_CODE_O, 0)"
        SSQL += ", NVL(SEITOMAST.NENDO_O, 0)"
        SSQL += ", NVL(SEITOMAST.TUUBAN_O, 0)"
        SSQL += ", NVL(SEITOMAST.GAKUNEN_CODE_O, 0)"
        SSQL += ", NVL(SEITOMAST.CLASS_CODE_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEITO_NO_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEITO_KNAME_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEITO_NNAME_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIBETU_O, 0)"
        SSQL += ", NVL(SEITOMAST.TKIN_NO_O, 0)"
        SSQL += ", NVL(SEITOMAST.TSIT_NO_O, 0)"
        SSQL += ", NVL(SEITOMAST.KAMOKU_O, 0)"
        SSQL += ", NVL(SEITOMAST.KOUZA_O, 0)"
        SSQL += ", NVL(SEITOMAST.MEIGI_KNAME_O, 0)"
        SSQL += ", NVL(SEITOMAST.FURIKAE_O, 0)"
        SSQL += ", NVL(SEITOMAST.KEIYAKU_NJYU_O, 0)"
        SSQL += ", NVL(SEITOMAST.KEIYAKU_DENWA_O, 0)"
        SSQL += ", NVL(SEITOMAST.HIMOKU_ID_O, 0)"
        SSQL += ", NVL(SEITOMAST.TUKI_NO_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU01_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU01_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU02_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU02_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU03_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU03_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU04_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU04_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU05_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU05_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU06_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU06_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU07_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU07_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU08_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU08_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU09_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU09_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU10_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU10_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU11_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU11_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU12_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU12_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU13_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU13_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU14_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU14_O, 0)"
        SSQL += ", NVL(SEITOMAST.SEIKYU15_O, 0)"
        SSQL += ", NVL(SEITOMAST.KINGAKU15_O, 0)"

        SSQL += ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"
        SSQL += ", NVL(GAKMAST1.GAKUNEN_NNAME_G, 0)"
        SSQL += ", NVL(GAKMAST1.GAKKOU_CODE_G, 0)"

        SSQL += ", NVL(TENMAST.KIN_NO_N, 0)"
        SSQL += ", NVL(TENMAST.SIT_NO_N, 0)"
        SSQL += ", NVL(TENMAST.KIN_NNAME_N, 0)"
        SSQL += ", NVL(TENMAST.SIT_NNAME_N, 0)"

        SSQL += ", NVL(HIMOMAST.HIMOKU_ID_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME01_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU01_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME02_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU02_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME03_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU03_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME04_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU04_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME05_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU05_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME06_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU06_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME07_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU07_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME08_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU08_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME09_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU09_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME10_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU10_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME11_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU11_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME12_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU12_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME13_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU13_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME14_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU14_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_NAME15_H, 0)"
        SSQL += ", NVL(HIMOMAST.HIMOKU_KINGAKU15_H, 0)"

        SSQL += " FROM "
        SSQL += "  KZFMAST.SEITOMAST"
        SSQL += " ,KZFMAST.GAKMAST1"
        SSQL += " ,KZFMAST.TENMAST"
        SSQL += " ,KZFMAST.HIMOMAST"

        SSQL += " WHERE "
        SSQL += "     SEITOMAST.GAKKOU_CODE_O  = GAKMAST1.GAKKOU_CODE_G  "
        SSQL += " AND SEITOMAST.GAKUNEN_CODE_O = GAKMAST1.GAKUNEN_CODE_G "

        SSQL += " AND SEITOMAST.GAKKOU_CODE_O  = HIMOMAST.GAKKOU_CODE_H  "
        SSQL += " AND SEITOMAST.GAKUNEN_CODE_O = HIMOMAST.GAKUNEN_CODE_H "
        SSQL += " AND SEITOMAST.HIMOKU_ID_O    = HIMOMAST.HIMOKU_ID_H  "
        SSQL += " AND SEITOMAST.TUKI_NO_O      = HIMOMAST.TUKI_NO_H  "

        SSQL += " AND SEITOMAST.TKIN_NO_O      = TENMAST.KIN_NO_N "
        SSQL += " AND SEITOMAST.TSIT_NO_O      = TENMAST.SIT_NO_N "
        '2006/10/23　ALL9指定時は全学校印刷するように修正
        If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
            SSQL += " AND SEITOMAST.GAKKOU_CODE_O  =" & "'" & txtGAKKOU_CODE.Text & "'"
        End If
        '2006/10/23　学年未入力時は全学年印刷するように修正
        If Trim(txtGAKUNEN.Text) <> "" Then
            SSQL += " AND SEITOMAST.GAKUNEN_CODE_O =" & "'" & txtGAKUNEN.Text & "'"
        End If
        SSQL += " AND SEITOMAST.TUKI_NO_O      =" & "'" & txtHIMOKU.Text & "'"

        '2007/10/23 mitsu 解約フラグも見る
        SSQL += " AND SEITOMAST.KAIYAKU_FLG_O ='0'"

        SSQL += " ORDER BY "
        SSQL += " SEITOMAST.GAKKOU_CODE_O ASC"
        SSQL += ",SEITOMAST.GAKUNEN_CODE_O ASC"
        SSQL += ",SEITOMAST.CLASS_CODE_O ASC"

        If chk生徒番号順.Checked = True Then
            SSQL += ",SEITOMAST.SEITO_NO_O ASC"
        End If

        If chk通番順.Checked = True Then
            SSQL += ",SEITOMAST.TUUBAN_O ASC"
        End If

        If chkあいうえお順.Checked = True Then
            SSQL += ",SEITOMAST.SEITO_KNAME_O ASC"
        End If

        If chk性別順.Checked = True Then
            SSQL += ",SEITOMAST.SEIBETU_O ASC"
        End If

        PFUC_SQLQuery_MAKE = SSQL

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

            '月必須チェック
            If txtHIMOKU.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtHIMOKU.Focus()
                Return False
            End If

            '月範囲チェック
            If GCom.NzInt(txtHIMOKU.Text.Trim) < 1 OrElse GCom.NzInt(txtHIMOKU.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtHIMOKU.Focus()
                Return False
            End If

            If chk生徒番号順.Checked = False And chk通番順.Checked = False And chkあいうえお順.Checked = False And chk性別順.Checked = False Then
                MessageBox.Show("出力ソート順番を指定してください。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                chk生徒番号順.Focus()
                Exit Function
            End If

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


            '印刷する生徒データがあるか検索
            SQL = New StringBuilder
            SQL.Append("SELECT * FROM SEITOMAST")
            SQL.Append(" WHERE KAIYAKU_FLG_O ='0'")
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) = "" Then

            ElseIf Trim(txtGAKKOU_CODE.Text) = "9999999999" And Trim(txtGAKUNEN.Text) <> "" Then
                SQL.Append(" AND GAKUNEN_CODE_O = " & SQ(Trim(txtGAKUNEN.Text)))
            ElseIf Trim(txtGAKKOU_CODE.Text) <> "9999999999" And Trim(txtGAKUNEN.Text) = "" Then
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
            Else
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(Trim(txtGAKKOU_CODE.Text)))
                SQL.Append(" AND GAKUNEN_CODE_O = " & Trim(txtGAKUNEN.Text))
            End If

            If OraReader.DataReader(SQL) = False Then
                If Trim(txtGAKUNEN.Text) <> "" Then
                    MessageBox.Show("指定学年に生徒が登録されていません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    MessageBox.Show("指定学校に生徒が登録されていません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                txtGAKUNEN.Focus()
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
#End Region
#Region " イベント"
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

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
       Handles txtHIMOKU.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("ゼロパディング", "失敗", ex.ToString)
        End Try
    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '学校コード
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            CType(sender, TextBox).Text = CType(sender, TextBox).Text.Trim.PadLeft(CType(sender, TextBox).MaxLength, "0")
            '学校名の取得
            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If
        End If
    End Sub
    Private Sub txtGAKUNEN_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKUNEN.Validating
        '学年
        If Trim(txtGAKUNEN.Text) <> "" Then
            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                If PFUNC_GAKUNENNAME_GET() = False Then
                    Exit Sub
                End If
            End If
        End If
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

End Class
