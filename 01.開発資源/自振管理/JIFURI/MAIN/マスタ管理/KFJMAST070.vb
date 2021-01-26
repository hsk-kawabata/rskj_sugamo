Imports System.Text
Imports CASTCommon

Public Class KFJMAST070
    '共通イベントコントロール
    Private CAST As New CASTCommon.Events

    '数値を許可する
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    '' 空白を許可しない
    Private CASTx1 As New CASTCommon.Events(" ", CASTCommon.Events.KeyMode.BAD)
    Public clsFUSION As New clsFUSION.clsMain
#Region "宣言"

    'Public strFURI_NEN1 As String
    'Public strFURI_TUKI1 As String
    'Public strFURI_HI1 As String
    'Public strFURI_NEN2 As String
    'Public strFURI_TUKI2 As String
    'Public strFURI_HI2 As String

    Dim L As Integer
    Dim P As Integer
    Dim intSCH_COUNT As Integer
    Dim intMAXCNT As Integer
    Dim intMAXLINE As Integer
    Dim intPAGE_CNT As Integer

    Public strTORI_CODE(500, 18) As String
    Public strTORI_NAME(500, 18) As String
    Public strBAITAI(500, 18) As String
    Public strFURI_DATE(500, 18) As String
    Public strSFURI_DATE(500, 18) As String
    'Public strUKETUKE(500, 18) As String
    'Public strTOUROKU(500, 18) As String
    'Public strHAISIN(500, 18) As String
    'Public strFUNOU(500, 18) As String
    'Public strHENKAN(500, 18) As String
    'Public strKESSAI(500, 18) As String
    'Public dblSYORI_KEN(500, 18) As Double
    'Public dblSYORI_KIN(500, 18) As Double
    Public strMOTIKOMI(500, 18) As Double

    'Dim KFJMAST1200G As KFJMAST070
    Public gdbcTAKOU_CONNECT As New OracleClient.OracleConnection   'CONNECTION
    Public gdbrTAKOU_READER As OracleClient.OracleDataReader 'READER
    Public gdbTAKOU_COMMAND As OracleClient.OracleCommand   'COMMAND関数

    Public strFDY(500, 18) As String
    Public strFDM(500, 18) As String
    Public strFDD(500, 18) As String
    Public intDEL(500, 18) As Integer

    Public objFDY(18) As TextBox
    Public objFDM(18) As TextBox
    Public objFDD(18) As TextBox

    'スケジュールマスタ（SCHMAST）更新用
    Public gdbcSCH_CONNECT As New OracleClient.OracleConnection   'CONNECTION
    Public gdbrSCH_READER As OracleClient.OracleDataReader 'READER
    Public gdbSCH_COMMAND As OracleClient.OracleCommand   'COMMAND関数
    Public gdbSCH_TRANS As OracleClient.OracleTransaction 'TRANSACTION

    Dim strTORIS_CODE As String
    Dim strTORIF_CODE As String
    Dim strFURIDATE As String
    Dim strNEW_FURIDATE As String

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAST070", "スケジュール変更画面")
    Private Const msgTitle As String = "スケジュール変更画面(KFJMAST070)"

    Private MyOwnerForm As Form

#End Region

    '画面終了時処理

    Private Sub KFJMAST070_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed

        Try
            GOwnerForm = MyOwnerForm
            GOwnerForm.Visible = True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)", "失敗", ex.Message)
        End Try

    End Sub

    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdBack.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")

            Me.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
    Private Sub KFJMAST070_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Dim SyoriDate As String = System.DateTime.Now.ToString("yyyyMMdd")

            MyOwnerForm = GOwnerForm
            GOwnerForm = Me
            Me.CmdBack.DialogResult = DialogResult.None

            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            txtFuriDateY1.Text = SyoriDate.Substring(0, 4)
            txtFuriDateM1.Text = SyoriDate.Substring(4, 2)
            txtFuriDateD1.Text = SyoriDate.Substring(6, 2)
            txtFuriDateY2.Text = SyoriDate.Substring(0, 4)
            txtFuriDateM2.Text = SyoriDate.Substring(4, 2)
            txtFuriDateD2.Text = SyoriDate.Substring(6, 2)

            '休日マスタ取り込み
            '2013/03/19 saitou 標準修正 UPD -------------------------------------------------->>>>
            '共通関数一元化
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)終了", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            'If fn_select_YASUMIMAST() = False Then
            '    MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If
            '2013/03/19 saitou 標準修正 UPD --------------------------------------------------<<<<
            'iniファイル読み込み つかっていないと思われる。 2009.10.05
            'If FN_SET_INIFILE() = False Then
            '    fn_LOG_WRITE(gstrUSER_ID, "", "", msgTitle, "iniファイルの取得", "失敗", Err.Description)
            '    MessageBox.Show("iniファイルの取得に失敗しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If

            '-----------------------------------
            'コントロール配列セット
            '-----------------------------------
            If fn_set_Control() = False Then
                MessageBox.Show(MSG0239W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            '--------------------------------------
            '画面のクリア
            '--------------------------------------
            btnNextGamen.Enabled = False
            btnPreGamen.Enabled = False
            btnAction.Enabled = False
            btnClear.Enabled = False
            CmdSelect.Enabled = True

            txtFuriDateY1.Enabled = True
            txtFuriDateM1.Enabled = True
            txtFuriDateD1.Enabled = True
            txtFuriDateY2.Enabled = True
            txtFuriDateM2.Enabled = True
            txtFuriDateD2.Enabled = True
            Call fn_GAMEN_CLEAR()

            intPAGE_CNT = 0

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub
    Function fn_SCHMAST_KENSAKU(ByVal db As CASTCommon.MyOracle) As Boolean
        '=====================================================================================
        'NAME           :fn_SCHMAST_KENSAKU
        'Parameter      :
        'Description    :対象年月のスケジュールを検索する
        'Return         :
        'Create         :2006/01/26
        'Update         :
        '=====================================================================================

        Dim sql As New StringBuilder(128)
        Dim oraReader As New CASTCommon.MyOracleReader(db)

        Try

            fn_SCHMAST_KENSAKU = False

            sql.Append("SELECT * FROM SCHMAST,TORIMAST WHERE FSYORI_KBN_S = '1'")
            sql.Append(" AND FURI_DATE_S BETWEEN '" & txtFuriDateY1.Text & txtFuriDateM1.Text & txtFuriDateD1.Text & "'")
            sql.Append(" AND '" & txtFuriDateY2.Text & txtFuriDateM2.Text & txtFuriDateD2.Text & "'")
            'sql.Append(" AND YUUKOU_FLG_S = '1'")
            sql.Append(" AND TYUUDAN_FLG_S = '0'")
            sql.Append(" AND SCHMAST.TORIS_CODE_S = TORIMAST.TORIS_CODE_T")
            sql.Append(" AND  SCHMAST.TORIF_CODE_S = TORIMAST.TORIF_CODE_T")
            sql.Append(" AND  TORIMAST.BAITAI_CODE_T <> '07'")
            '***進行中のスケジュールは表示もしない
            sql.Append(" AND  SCHMAST.UKETUKE_FLG_S = '0'")
            sql.Append(" AND  SCHMAST.TOUROKU_FLG_S = '0'")
            sql.Append(" AND  SCHMAST.HAISIN_FLG_S = '0'")
            sql.Append(" AND  SCHMAST.SAIFURI_FLG_S = '0'")
            sql.Append(" AND  SCHMAST.SOUSIN_FLG_S = '0'")
            sql.Append(" AND  SCHMAST.FUNOU_FLG_S = '0'")
            sql.Append(" AND  SCHMAST.TESUUKEI_FLG_S = '0'")
            sql.Append(" AND  SCHMAST.TESUUTYO_FLG_S = '0'")
            sql.Append(" AND  SCHMAST.KESSAI_FLG_S = '0'")
            sql.Append(" AND  SCHMAST.HENKAN_FLG_S = '0'")
            sql.Append(" AND  SCHMAST.TAKOU_FLG_S = '0'")
            sql.Append(" AND  SCHMAST.NIPPO_FLG_S = '0'")
            sql.Append(" AND  NOT EXISTS (SELECT TORIS_CODE_T FROM TORIMAST WHERE (SFURI_FLG_T ='1' AND TORIS_CODE_T = SCHMAST.TORIS_CODE_S AND SFURI_FCODE_T = SCHMAST.TORIF_CODE_S))")
            sql.Append(" ORDER BY FURI_DATE_S ASC,TORIS_CODE_S ASC,TORIF_CODE_S ASC")

            intSCH_COUNT = 0
            L = 1
            P = 1
            '読込のみ
            If oraReader.DataReader(sql) = True Then

                Do Until oraReader.EOF
                    strTORI_CODE(P, L) = oraReader.GetString("TORIS_CODE_S") & "-" & oraReader.GetString("TORIF_CODE_S")
                    strTORI_NAME(P, L) = clsFUSION.fn_chenge_null_value(oraReader.GetString("ITAKU_NNAME_T"))
                    strBAITAI(P, L) = clsFUSION.fn_chenge_null_value(oraReader.GetString("BAITAI_CODE_T"))
                    strFURI_DATE(P, L) = oraReader.GetString("FURI_DATE_S")
                    strSFURI_DATE(P, L) = oraReader.GetString("KSAIFURI_DATE_S")
                    strFURI_DATE(P, L) = strFURI_DATE(P, L).Substring(0, 4) & "/" & strFURI_DATE(P, L).Substring(4, 2) & "/" & strFURI_DATE(P, L).Substring(6, 2)
                    'strUKETUKE(P, L) = clsFUSION.fn_chenge_null_value(oraReader.GetString("UKETUKE_FLG_S"))
                    'strTOUROKU(P, L) = clsFUSION.fn_chenge_null_value(oraReader.GetString("TOUROKU_FLG_S"))
                    'strHAISIN(P, L) = clsFUSION.fn_chenge_null_value(oraReader.GetString("HAISIN_FLG_S"))
                    'strFUNOU(P, L) = clsFUSION.fn_chenge_null_value(oraReader.GetString("FUNOU_FLG_S"))
                    'strHENKAN(P, L) = clsFUSION.fn_chenge_null_value(oraReader.GetString("HENKAN_FLG_S"))
                    'strKESSAI(P, L) = clsFUSION.fn_chenge_null_value(oraReader.GetString("KESSAI_FLG_S"))
                    'dblSYORI_KEN(P, L) = oraReader.GetInt64("SYORI_KEN_S")
                    'dblSYORI_KIN(P, L) = oraReader.GetInt64("SYORI_KIN_S")
                    strFDY(P, L) = ""
                    strFDM(P, L) = ""
                    strFDD(P, L) = ""
                    intDEL(P, L) = 0
                    strMOTIKOMI(P, L) = clsFUSION.fn_chenge_null_value(oraReader.GetString("MOTIKOMI_KBN_T"))
                    intSCH_COUNT += 1
                    If L >= 18 Then
                        L = 1
                        P = P + 1
                    Else
                        L = L + 1
                    End If

                    oraReader.NextRead()
                Loop
            Else
                Return False
            End If

            'カウントを1回多くしているため、－１する
            If L = 1 Then
                intMAXLINE = 18
                intMAXCNT = P - 1
            Else
                intMAXCNT = P
                intMAXLINE = L - 1
            End If

            fn_SCHMAST_KENSAKU = True

        Catch ex As Exception
            Throw
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try
    End Function
    Function fn_GAMEN_HYOUJI(ByVal aintPAGE_NO As Integer) As Boolean
        '============================================================================
        'NAME           :fn_GAMEN_HYOUJI
        'Parameter      :
        'Description    :指定した画面の表示
        'Return         :
        'Create         :2006/01/27
        'Update         :
        '============================================================================
        fn_GAMEN_HYOUJI = False
        Dim intLINE As Integer

        Try

 
            P = aintPAGE_NO
            If intMAXCNT = 0 Then
                intLINE = 0
            Else
                intLINE = 18
            End If
            For L = 1 To intLINE
                Select Case L
                    Case 1
                        lblNO1.Text = L + ((P - 1) * 18)
                        lblToriName1.Text = strTORI_NAME(P, L)
                        lblTORI_CODE1.Text = strTORI_CODE(P, L)
                        lblFURI_DATE1.Text = strFURI_DATE(P, L)
                        lblBAITAI1.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel1.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY1.Text = strFDY(P, L)
                        txtFDM1.Text = strFDM(P, L)
                        txtFDD1.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY1.Enabled = False
                        '    txtFDM1.Enabled = False
                        '    txtFDD1.Enabled = False
                        '    ckbDel1.Checked = False
                        '    ckbDel1.Enabled = False
                        'End If
                    Case 2
                        lblNO2.Text = L + ((P - 1) * 18)
                        lblToriName2.Text = strTORI_NAME(P, L)
                        lblTORI_CODE2.Text = strTORI_CODE(P, L)
                        lblFURI_DATE2.Text = strFURI_DATE(P, L)
                        lblBAITAI2.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel2.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY2.Text = strFDY(P, L)
                        txtFDM2.Text = strFDM(P, L)
                        txtFDD2.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY2.Enabled = False
                        '    txtFDM2.Enabled = False
                        '    txtFDD2.Enabled = False
                        '    ckbDel2.Checked = False
                        '    ckbDel2.Enabled = False
                        'End If
                    Case 3
                        lblNO3.Text = L + ((P - 1) * 18)
                        lblToriName3.Text = strTORI_NAME(P, L)
                        lblTORI_CODE3.Text = strTORI_CODE(P, L)
                        lblFURI_DATE3.Text = strFURI_DATE(P, L)
                        lblBAITAI3.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel3.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY3.Text = strFDY(P, L)
                        txtFDM3.Text = strFDM(P, L)
                        txtFDD3.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY3.Enabled = False
                        '    txtFDM3.Enabled = False
                        '    txtFDD3.Enabled = False
                        '    ckbDel3.Checked = False
                        '    ckbDel3.Enabled = False
                        'End If
                    Case 4
                        lblNO4.Text = L + ((P - 1) * 18)
                        lblToriName4.Text = strTORI_NAME(P, L)
                        lblTORI_CODE4.Text = strTORI_CODE(P, L)
                        lblFURI_DATE4.Text = strFURI_DATE(P, L)
                        lblBAITAI4.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel4.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY4.Text = strFDY(P, L)
                        txtFDM4.Text = strFDM(P, L)
                        txtFDD4.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY4.Enabled = False
                        '    txtFDM4.Enabled = False
                        '    txtFDD4.Enabled = False
                        '    ckbDel4.Checked = False
                        '    ckbDel4.Enabled = False
                        'End If
                    Case 5
                        lblNO5.Text = L + ((P - 1) * 18)
                        lblToriName5.Text = strTORI_NAME(P, L)
                        lblTORI_CODE5.Text = strTORI_CODE(P, L)
                        lblFURI_DATE5.Text = strFURI_DATE(P, L)
                        lblBAITAI5.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel5.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY5.Text = strFDY(P, L)
                        txtFDM5.Text = strFDM(P, L)
                        txtFDD5.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY5.Enabled = False
                        '    txtFDM5.Enabled = False
                        '    txtFDD5.Enabled = False
                        '    ckbDel5.Checked = False
                        '    ckbDel5.Enabled = False
                        'End If
                    Case 6
                        lblNO6.Text = L + ((P - 1) * 18)
                        lblToriName6.Text = strTORI_NAME(P, L)
                        lblTORI_CODE6.Text = strTORI_CODE(P, L)
                        lblFURI_DATE6.Text = strFURI_DATE(P, L)
                        lblBAITAI6.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel6.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY6.Text = strFDY(P, L)
                        txtFDM6.Text = strFDM(P, L)
                        txtFDD6.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY6.Enabled = False
                        '    txtFDM6.Enabled = False
                        '    txtFDD6.Enabled = False
                        '    ckbDel6.Checked = False
                        '    ckbDel6.Enabled = False
                        'End If
                    Case 7
                        lblNO7.Text = L + ((P - 1) * 18)
                        lblToriName7.Text = strTORI_NAME(P, L)
                        lblTORI_CODE7.Text = strTORI_CODE(P, L)
                        lblFURI_DATE7.Text = strFURI_DATE(P, L)
                        lblBAITAI7.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel7.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY7.Text = strFDY(P, L)
                        txtFDM7.Text = strFDM(P, L)
                        txtFDD7.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY7.Enabled = False
                        '    txtFDM7.Enabled = False
                        '    txtFDD7.Enabled = False
                        '    ckbDel7.Checked = False
                        '    ckbDel7.Enabled = False
                        'End If
                    Case 8
                        lblNO8.Text = L + ((P - 1) * 18)
                        lblToriName8.Text = strTORI_NAME(P, L)
                        lblTORI_CODE8.Text = strTORI_CODE(P, L)
                        lblFURI_DATE8.Text = strFURI_DATE(P, L)
                        lblBAITAI8.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel8.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY8.Text = strFDY(P, L)
                        txtFDM8.Text = strFDM(P, L)
                        txtFDD8.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY8.Enabled = False
                        '    txtFDM8.Enabled = False
                        '    txtFDD8.Enabled = False
                        '    ckbDel8.Checked = False
                        '    ckbDel8.Enabled = False
                        'End If
                    Case 9
                        lblNO9.Text = L + ((P - 1) * 18)
                        lblToriName9.Text = strTORI_NAME(P, L)
                        lblTORI_CODE9.Text = strTORI_CODE(P, L)
                        lblFURI_DATE9.Text = strFURI_DATE(P, L)
                        lblBAITAI9.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel9.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY9.Text = strFDY(P, L)
                        txtFDM9.Text = strFDM(P, L)
                        txtFDD9.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY9.Enabled = False
                        '    txtFDM9.Enabled = False
                        '    txtFDD9.Enabled = False
                        '    ckbDel9.Checked = False
                        '    ckbDel9.Enabled = False
                        'End If
                    Case 10
                        lblNO10.Text = L + ((P - 1) * 18)
                        lblToriName10.Text = strTORI_NAME(P, L)
                        lblTORI_CODE10.Text = strTORI_CODE(P, L)
                        lblFURI_DATE10.Text = strFURI_DATE(P, L)
                        lblBAITAI10.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel10.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY10.Text = strFDY(P, L)
                        txtFDM10.Text = strFDM(P, L)
                        txtFDD10.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY10.Enabled = False
                        '    txtFDM10.Enabled = False
                        '    txtFDD10.Enabled = False
                        '    ckbDel10.Checked = False
                        '    ckbDel10.Enabled = False
                        'End If
                    Case 11
                        lblNO11.Text = L + ((P - 1) * 18)
                        lblToriName11.Text = strTORI_NAME(P, L)
                        lblTORI_CODE11.Text = strTORI_CODE(P, L)
                        lblFURI_DATE11.Text = strFURI_DATE(P, L)
                        lblBAITAI11.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel11.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY11.Text = strFDY(P, L)
                        txtFDM11.Text = strFDM(P, L)
                        txtFDD11.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY11.Enabled = False
                        '    txtFDM11.Enabled = False
                        '    txtFDD11.Enabled = False
                        '    ckbDel11.Checked = False
                        '    ckbDel11.Enabled = False
                        'End If
                    Case 12
                        lblNO12.Text = L + ((P - 1) * 18)
                        lblToriName12.Text = strTORI_NAME(P, L)
                        lblTORI_CODE12.Text = strTORI_CODE(P, L)
                        lblFURI_DATE12.Text = strFURI_DATE(P, L)
                        lblBAITAI12.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel12.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY12.Text = strFDY(P, L)
                        txtFDM12.Text = strFDM(P, L)
                        txtFDD12.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY12.Enabled = False
                        '    txtFDM12.Enabled = False
                        '    txtFDD12.Enabled = False
                        '    ckbDel12.Checked = False
                        '    ckbDel12.Enabled = False
                        'End If
                    Case 13
                        lblNO13.Text = L + ((P - 1) * 18)
                        lblToriName13.Text = strTORI_NAME(P, L)
                        lblTORI_CODE13.Text = strTORI_CODE(P, L)
                        lblFURI_DATE13.Text = strFURI_DATE(P, L)
                        lblBAITAI13.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel13.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY13.Text = strFDY(P, L)
                        txtFDM13.Text = strFDM(P, L)
                        txtFDD13.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY13.Enabled = False
                        '    txtFDM13.Enabled = False
                        '    txtFDD13.Enabled = False
                        '    ckbDel13.Checked = False
                        '    ckbDel13.Enabled = False
                        'End If
                    Case 14
                        lblNO14.Text = L + ((P - 1) * 18)
                        lblToriName14.Text = strTORI_NAME(P, L)
                        lblTORI_CODE14.Text = strTORI_CODE(P, L)
                        lblFURI_DATE14.Text = strFURI_DATE(P, L)
                        lblBAITAI14.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel14.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY14.Text = strFDY(P, L)
                        txtFDM14.Text = strFDM(P, L)
                        txtFDD14.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY14.Enabled = False
                        '    txtFDM14.Enabled = False
                        '    txtFDD14.Enabled = False
                        '    ckbDel14.Checked = False
                        '    ckbDel14.Enabled = False
                        'End If
                    Case 15
                        lblNO15.Text = L + ((P - 1) * 18)
                        lblToriName15.Text = strTORI_NAME(P, L)
                        lblTORI_CODE15.Text = strTORI_CODE(P, L)
                        lblFURI_DATE15.Text = strFURI_DATE(P, L)
                        lblBAITAI15.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel15.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY15.Text = strFDY(P, L)
                        txtFDM15.Text = strFDM(P, L)
                        txtFDD15.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY15.Enabled = False
                        '    txtFDM15.Enabled = False
                        '    txtFDD15.Enabled = False
                        '    ckbDel15.Checked = False
                        '    ckbDel15.Enabled = False
                        'End If
                    Case 16
                        lblNO16.Text = L + ((P - 1) * 18)
                        lblToriName16.Text = strTORI_NAME(P, L)
                        lblTORI_CODE16.Text = strTORI_CODE(P, L)
                        lblFURI_DATE16.Text = strFURI_DATE(P, L)
                        lblBAITAI16.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel16.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY16.Text = strFDY(P, L)
                        txtFDM16.Text = strFDM(P, L)
                        txtFDD16.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY16.Enabled = False
                        '    txtFDM16.Enabled = False
                        '    txtFDD16.Enabled = False
                        '    ckbDel16.Checked = False
                        '    ckbDel16.Enabled = False
                        'End If
                    Case 17
                        lblNO17.Text = L + ((P - 1) * 18)
                        lblToriName17.Text = strTORI_NAME(P, L)
                        lblTORI_CODE17.Text = strTORI_CODE(P, L)
                        lblFURI_DATE17.Text = strFURI_DATE(P, L)
                        lblBAITAI17.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel17.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY17.Text = strFDY(P, L)
                        txtFDM17.Text = strFDM(P, L)
                        txtFDD17.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY17.Enabled = False
                        '    txtFDM17.Enabled = False
                        '    txtFDD17.Enabled = False
                        '    ckbDel17.Checked = False
                        '    ckbDel17.Enabled = False
                        'End If
                    Case 18
                        lblNO18.Text = L + ((P - 1) * 18)
                        lblToriName18.Text = strTORI_NAME(P, L)
                        lblTORI_CODE18.Text = strTORI_CODE(P, L)
                        lblFURI_DATE18.Text = strFURI_DATE(P, L)
                        lblBAITAI18.Text = fn_BAITAI_YOMIKAE(strBAITAI(P, L))
                        ckbDel18.Checked = IIf(intDEL(P, L) = 1, True, False)
                        txtFDY18.Text = strFDY(P, L)
                        txtFDM18.Text = strFDM(P, L)
                        txtFDD18.Text = strFDD(P, L)
                        'If strUKETUKE(P, L) <> 0 Or strTOUROKU(P, L) <> 0 Or strHAISIN(P, L) <> 0 Or strFUNOU(P, L) <> 0 Or strHENKAN(P, L) <> 0 Or strKESSAI(P, L) Then
                        '    txtFDY18.Enabled = False
                        '    txtFDM18.Enabled = False
                        '    txtFDD18.Enabled = False
                        '    ckbDel18.Checked = False
                        '    ckbDel18.Enabled = False
                        'End If
                End Select
                If P = intMAXCNT Then
                    If L = intMAXLINE Then
                        Exit For
                    End If
                End If
            Next

            lblPage.Text = Format(intPAGE_CNT, "00") & "/" & Format(intMAXCNT, "00")

            fn_GAMEN_HYOUJI = True

        Catch ex As Exception
            Throw
        End Try

    End Function
    Private Function fn_BAITAI_YOMIKAE(ByVal astrBAITAI_CODE As String) As String
        '=====================================================================================
        'NAME           :fn_BAITAI_YOMIKAE
        'Parameter      :
        'Description    :媒体コードより媒体名取得
        'Return         :媒体名
        'Create         :2004/09/02
        'Update         :
        '=====================================================================================
        fn_BAITAI_YOMIKAE = ""
        ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
        '媒体名をテキストから取得する
        fn_BAITAI_YOMIKAE = CASTCommon.GetText_CodeToName(System.IO.Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"), _
                                                          astrBAITAI_CODE)
        'Select Case astrBAITAI_CODE
        '    Case "00"
        '        fn_BAITAI_YOMIKAE = "伝送"
        '    Case "01"
        '        fn_BAITAI_YOMIKAE = "3.5FD"
        '    Case "04"
        '        fn_BAITAI_YOMIKAE = "依頼書"
        '    Case "05"
        '        fn_BAITAI_YOMIKAE = "ＭＴ"
        '    Case "06"
        '        fn_BAITAI_YOMIKAE = "ＣＭＴ"
        '    Case "07"
        '        fn_BAITAI_YOMIKAE = "学校"
        '    Case "09"
        '        fn_BAITAI_YOMIKAE = "伝票"
        '        '2012/06/30 標準版　WEB伝送対応
        '    Case "10"
        '        fn_BAITAI_YOMIKAE = "WEB伝送"
        '    Case "11"
        '        fn_BAITAI_YOMIKAE = "DVD-RAM"
        '    Case "12"
        '        fn_BAITAI_YOMIKAE = "その他"
        '    Case "13"
        '        fn_BAITAI_YOMIKAE = "その他"
        '    Case "14"
        '        fn_BAITAI_YOMIKAE = "その他"
        '    Case "15"
        '        fn_BAITAI_YOMIKAE = "その他"
        '    Case Else
        '        fn_BAITAI_YOMIKAE = "ERROR"
        'End Select
        ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END
    End Function

#Region "次画面"
    Private Sub btnNextGamen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNextGamen.Click
        '============================================================================
        'NAME           :btnNextGamen_Click
        'Parameter      :
        'Description    :次画面ボタン押下時の処理
        'Return         :
        'Create         :2006/01/27
        'Update         :
        '============================================================================

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(次画面)開始", "成功", "")


            If intPAGE_CNT = intMAXCNT Then
                If MessageBox.Show(MSG0266W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning) = DialogResult.OK Then
                    Exit Sub
                End If

            End If

            '画面の振替日の日付チェック
            If CheckFuriDate() = False Then
                Return
            End If

            '--------------------------------------
            '画面の保存
            '--------------------------------------
            Call fn_GAMEN_HOZON(intPAGE_CNT)

            '--------------------------------------
            '画面のクリア
            '--------------------------------------
            Call fn_GAMEN_CLEAR()

            '--------------------------------------
            '画面の表示
            '--------------------------------------
            intPAGE_CNT += 1
            If fn_GAMEN_HYOUJI(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0253W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(次画面)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(次画面)終了", "成功", "")
        End Try

    End Sub
#End Region
#Region "前画面"
    Private Sub btnPreGamen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPreGamen.Click
        '============================================================================
        'NAME           :btnPreGamen_Click
        'Parameter      :
        'Description    :前画面ボタン押下時の処理
        'Return         :
        'Create         :2006/01/27
        'Update         :
        '============================================================================

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(前画面)開始", "成功", "")

            If intPAGE_CNT <= 1 Then
                If MessageBox.Show(MSG0264W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning) = DialogResult.OK Then
                    Exit Sub
                End If

            End If

            '画面の振替日の日付チェック
            If CheckFuriDate() = False Then
                Return
            End If

            '--------------------------------------
            '画面の保存
            '--------------------------------------
            Call fn_GAMEN_HOZON(intPAGE_CNT)

            '--------------------------------------
            '画面のクリア
            '--------------------------------------
            Call fn_GAMEN_CLEAR()

            '--------------------------------------
            '画面の表示
            '--------------------------------------
            intPAGE_CNT -= 1
            If fn_GAMEN_HYOUJI(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0253W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(前画面)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(前画面)終了", "成功", "")
        End Try

    End Sub
#End Region
    Private Function fn_GAMEN_CLEAR() As Boolean
        '=====================================================================================
        'NAME           :fn_GAMEN_CLEAR
        'Parameter      :
        'Description    :画面のラベルクリア
        'Return         :
        'Create         :2006/01/27
        'Update         :
        '=====================================================================================

        Try

            lblNO1.Text = ""
            lblToriName1.Text = ""
            lblTORI_CODE1.Text = ""
            lblFURI_DATE1.Text = ""
            lblBAITAI1.Text = ""
            txtFDY1.Text = ""
            txtFDM1.Text = ""
            txtFDD1.Text = ""
            ckbDel1.Checked = False
            txtFDY1.Enabled = True
            txtFDM1.Enabled = True
            txtFDD1.Enabled = True
            ckbDel1.Enabled = True


            lblNO2.Text = ""
            lblToriName2.Text = ""
            lblTORI_CODE2.Text = ""
            lblFURI_DATE2.Text = ""
            lblBAITAI2.Text = ""
            txtFDY2.Text = ""
            txtFDM2.Text = ""
            txtFDD2.Text = ""
            ckbDel2.Checked = False
            txtFDY2.Enabled = True
            txtFDM2.Enabled = True
            txtFDD2.Enabled = True
            ckbDel2.Enabled = True

            lblNO3.Text = ""
            lblToriName3.Text = ""
            lblTORI_CODE3.Text = ""
            lblFURI_DATE3.Text = ""
            lblBAITAI3.Text = ""
            txtFDY3.Text = ""
            txtFDM3.Text = ""
            txtFDD3.Text = ""
            ckbDel3.Checked = False
            txtFDY3.Enabled = True
            txtFDM3.Enabled = True
            txtFDD3.Enabled = True
            ckbDel3.Enabled = True

            lblNO4.Text = ""
            lblToriName4.Text = ""
            lblTORI_CODE4.Text = ""
            lblFURI_DATE4.Text = ""
            lblBAITAI4.Text = ""
            txtFDY4.Text = ""
            txtFDM4.Text = ""
            txtFDD4.Text = ""
            ckbDel4.Checked = False
            txtFDY4.Enabled = True
            txtFDM4.Enabled = True
            txtFDD4.Enabled = True
            ckbDel4.Enabled = True

            lblNO5.Text = ""
            lblToriName5.Text = ""
            lblTORI_CODE5.Text = ""
            lblFURI_DATE5.Text = ""
            lblBAITAI5.Text = ""
            txtFDY5.Text = ""
            txtFDM5.Text = ""
            txtFDD5.Text = ""
            ckbDel5.Checked = False
            txtFDY5.Enabled = True
            txtFDM5.Enabled = True
            txtFDD5.Enabled = True
            ckbDel5.Enabled = True

            lblNO6.Text = ""
            lblToriName6.Text = ""
            lblTORI_CODE6.Text = ""
            lblFURI_DATE6.Text = ""
            lblBAITAI6.Text = ""
            txtFDY6.Text = ""
            txtFDM6.Text = ""
            txtFDD6.Text = ""
            ckbDel6.Checked = False
            txtFDY6.Enabled = True
            txtFDM6.Enabled = True
            txtFDD6.Enabled = True
            ckbDel6.Enabled = True


            lblNO7.Text = ""
            lblToriName7.Text = ""
            lblTORI_CODE7.Text = ""
            lblFURI_DATE7.Text = ""
            lblBAITAI7.Text = ""
            txtFDY7.Text = ""
            txtFDM7.Text = ""
            txtFDD7.Text = ""
            ckbDel7.Checked = False
            txtFDY7.Enabled = True
            txtFDM7.Enabled = True
            txtFDD7.Enabled = True
            ckbDel7.Enabled = True

            lblNO8.Text = ""
            lblToriName8.Text = ""
            lblTORI_CODE8.Text = ""
            lblFURI_DATE8.Text = ""
            lblBAITAI8.Text = ""
            txtFDY8.Text = ""
            txtFDM8.Text = ""
            txtFDD8.Text = ""
            ckbDel8.Checked = False
            txtFDY8.Enabled = True
            txtFDM8.Enabled = True
            txtFDD8.Enabled = True
            ckbDel8.Enabled = True

            lblNO9.Text = ""
            lblToriName9.Text = ""
            lblTORI_CODE9.Text = ""
            lblFURI_DATE9.Text = ""
            lblBAITAI9.Text = ""
            txtFDY9.Text = ""
            txtFDM9.Text = ""
            txtFDD9.Text = ""
            ckbDel9.Checked = False
            txtFDY9.Enabled = True
            txtFDM9.Enabled = True
            txtFDD9.Enabled = True
            ckbDel9.Enabled = True

            lblNO10.Text = ""
            lblToriName10.Text = ""
            lblTORI_CODE10.Text = ""
            lblFURI_DATE10.Text = ""
            lblBAITAI10.Text = ""
            txtFDY10.Text = ""
            txtFDM10.Text = ""
            txtFDD10.Text = ""
            ckbDel10.Checked = False
            txtFDY10.Enabled = True
            txtFDM10.Enabled = True
            txtFDD10.Enabled = True
            ckbDel10.Enabled = True

            lblNO11.Text = ""
            lblToriName11.Text = ""
            lblTORI_CODE11.Text = ""
            lblFURI_DATE11.Text = ""
            lblBAITAI11.Text = ""
            txtFDY11.Text = ""
            txtFDM11.Text = ""
            txtFDD11.Text = ""
            ckbDel11.Checked = False
            txtFDY11.Enabled = True
            txtFDM11.Enabled = True
            txtFDD11.Enabled = True
            ckbDel11.Enabled = True

            lblNO12.Text = ""
            lblToriName12.Text = ""
            lblTORI_CODE12.Text = ""
            lblFURI_DATE12.Text = ""
            lblBAITAI12.Text = ""
            txtFDY12.Text = ""
            txtFDM12.Text = ""
            txtFDD12.Text = ""
            ckbDel12.Checked = False
            txtFDY12.Enabled = True
            txtFDM12.Enabled = True
            txtFDD12.Enabled = True
            ckbDel12.Enabled = True


            lblNO13.Text = ""
            lblToriName13.Text = ""
            lblTORI_CODE13.Text = ""
            lblFURI_DATE13.Text = ""
            lblBAITAI13.Text = ""
            txtFDY13.Text = ""
            txtFDM13.Text = ""
            txtFDD13.Text = ""
            ckbDel13.Checked = False
            txtFDY13.Enabled = True
            txtFDM13.Enabled = True
            txtFDD13.Enabled = True
            ckbDel13.Enabled = True



            lblNO14.Text = ""
            lblToriName14.Text = ""
            lblTORI_CODE14.Text = ""
            lblFURI_DATE14.Text = ""
            lblBAITAI14.Text = ""
            txtFDY14.Text = ""
            txtFDM14.Text = ""
            txtFDD14.Text = ""
            ckbDel14.Checked = False
            txtFDY14.Enabled = True
            txtFDM14.Enabled = True
            txtFDD14.Enabled = True
            ckbDel14.Enabled = True

            lblNO15.Text = ""
            lblToriName15.Text = ""
            lblTORI_CODE15.Text = ""
            lblFURI_DATE15.Text = ""
            lblBAITAI15.Text = ""
            txtFDY15.Text = ""
            txtFDM15.Text = ""
            txtFDD15.Text = ""
            ckbDel15.Checked = False
            txtFDY15.Enabled = True
            txtFDM15.Enabled = True
            txtFDD15.Enabled = True
            ckbDel15.Enabled = True

            lblNO16.Text = ""
            lblToriName16.Text = ""
            lblTORI_CODE16.Text = ""
            lblFURI_DATE16.Text = ""
            lblBAITAI16.Text = ""
            txtFDY16.Text = ""
            txtFDM16.Text = ""
            txtFDD16.Text = ""
            ckbDel16.Checked = False
            txtFDY16.Enabled = True
            txtFDM16.Enabled = True
            txtFDD16.Enabled = True
            ckbDel16.Enabled = True

            lblNO17.Text = ""
            lblToriName17.Text = ""
            lblTORI_CODE17.Text = ""
            lblFURI_DATE17.Text = ""
            lblBAITAI17.Text = ""
            txtFDY17.Text = ""
            txtFDM17.Text = ""
            txtFDD17.Text = ""
            ckbDel17.Checked = False
            txtFDY17.Enabled = True
            txtFDM17.Enabled = True
            txtFDD17.Enabled = True
            ckbDel17.Enabled = True

            lblNO18.Text = ""
            lblToriName18.Text = ""
            lblTORI_CODE18.Text = ""
            lblFURI_DATE18.Text = ""
            lblBAITAI18.Text = ""
            txtFDY18.Text = ""
            txtFDM18.Text = ""
            txtFDD18.Text = ""
            ckbDel18.Checked = False
            txtFDY18.Enabled = True
            txtFDM18.Enabled = True
            txtFDD18.Enabled = True
            ckbDel18.Enabled = True

        Catch ex As Exception
            Throw
        End Try

        Return True

    End Function
    Private Function fn_GAMEN_HOZON(ByVal aintPAGE As Integer) As Boolean
        '=====================================================================================
        'NAME           :fn_GAMEN_HOZON
        'Parameter      :aintPAGE:ページ番号
        'Description    :画面の保存
        'Return         :
        'Create         :2007/02/28
        'Update         :
        '=====================================================================================

        Try


            strFDY(aintPAGE, 1) = txtFDY1.Text
            strFDM(aintPAGE, 1) = IIf(txtFDM1.Text = "", "", txtFDM1.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 1) = IIf(txtFDD1.Text = "", "", txtFDD1.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 1) = IIf(ckbDel1.Checked = True, 1, 0)

            strFDY(aintPAGE, 2) = txtFDY2.Text
            strFDM(aintPAGE, 2) = IIf(txtFDM2.Text = "", "", txtFDM2.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 2) = IIf(txtFDD2.Text = "", "", txtFDD2.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 2) = IIf(ckbDel2.Checked = True, 1, 0)

            strFDY(aintPAGE, 3) = txtFDY3.Text
            strFDM(aintPAGE, 3) = IIf(txtFDM3.Text = "", "", txtFDM3.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 3) = IIf(txtFDD3.Text = "", "", txtFDD3.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 3) = IIf(ckbDel3.Checked = True, 1, 0)

            strFDY(aintPAGE, 4) = txtFDY4.Text
            strFDM(aintPAGE, 4) = IIf(txtFDM4.Text = "", "", txtFDM4.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 4) = IIf(txtFDD4.Text = "", "", txtFDD4.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 4) = IIf(ckbDel4.Checked = True, 1, 0)

            strFDY(aintPAGE, 5) = txtFDY5.Text
            strFDM(aintPAGE, 5) = IIf(txtFDM5.Text = "", "", txtFDM5.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 5) = IIf(txtFDD5.Text = "", "", txtFDD5.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 5) = IIf(ckbDel5.Checked = True, 1, 0)

            strFDY(aintPAGE, 6) = txtFDY6.Text
            strFDM(aintPAGE, 6) = IIf(txtFDM6.Text = "", "", txtFDM6.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 6) = IIf(txtFDD6.Text = "", "", txtFDD6.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 6) = IIf(ckbDel6.Checked = True, 1, 0)

            strFDY(aintPAGE, 7) = txtFDY7.Text
            strFDM(aintPAGE, 7) = IIf(txtFDM7.Text = "", "", txtFDM7.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 7) = IIf(txtFDD7.Text = "", "", txtFDD7.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 7) = IIf(ckbDel7.Checked = True, 1, 0)

            strFDY(aintPAGE, 8) = txtFDY8.Text
            strFDM(aintPAGE, 8) = IIf(txtFDM8.Text = "", "", txtFDM8.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 8) = IIf(txtFDD8.Text = "", "", txtFDD8.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 8) = IIf(ckbDel8.Checked = True, 1, 0)

            strFDY(aintPAGE, 9) = txtFDY9.Text
            strFDM(aintPAGE, 9) = IIf(txtFDM9.Text = "", "", txtFDM9.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 9) = IIf(txtFDD9.Text = "", "", txtFDD9.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 9) = IIf(ckbDel9.Checked = True, 1, 0)

            strFDY(aintPAGE, 10) = txtFDY10.Text
            strFDM(aintPAGE, 10) = IIf(txtFDM10.Text = "", "", txtFDM10.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 10) = IIf(txtFDD10.Text = "", "", txtFDD10.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 10) = IIf(ckbDel10.Checked = True, 1, 0)


            strFDY(aintPAGE, 11) = txtFDY11.Text
            strFDM(aintPAGE, 11) = IIf(txtFDM11.Text = "", "", txtFDM11.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 11) = IIf(txtFDD11.Text = "", "", txtFDD11.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 11) = IIf(ckbDel11.Checked = True, 1, 0)

            strFDY(aintPAGE, 12) = txtFDY12.Text
            strFDM(aintPAGE, 12) = IIf(txtFDM12.Text = "", "", txtFDM12.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 12) = IIf(txtFDD12.Text = "", "", txtFDD12.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 12) = IIf(ckbDel12.Checked = True, 1, 0)

            strFDY(aintPAGE, 13) = txtFDY13.Text
            strFDM(aintPAGE, 13) = IIf(txtFDM13.Text = "", "", txtFDM13.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 13) = IIf(txtFDD13.Text = "", "", txtFDD13.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 13) = IIf(ckbDel13.Checked = True, 1, 0)

            strFDY(aintPAGE, 14) = txtFDY14.Text
            strFDM(aintPAGE, 14) = IIf(txtFDM14.Text = "", "", txtFDM14.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 14) = IIf(txtFDD14.Text = "", "", txtFDD14.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 14) = IIf(ckbDel14.Checked = True, 1, 0)

            strFDY(aintPAGE, 15) = txtFDY15.Text
            strFDM(aintPAGE, 15) = IIf(txtFDM15.Text = "", "", txtFDM15.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 15) = IIf(txtFDD15.Text = "", "", txtFDD15.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 15) = IIf(ckbDel15.Checked = True, 1, 0)

            strFDY(aintPAGE, 16) = txtFDY16.Text
            strFDM(aintPAGE, 16) = IIf(txtFDM16.Text = "", "", txtFDM16.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 16) = IIf(txtFDD16.Text = "", "", txtFDD16.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 16) = IIf(ckbDel16.Checked = True, 1, 0)

            strFDY(aintPAGE, 17) = txtFDY17.Text
            strFDM(aintPAGE, 17) = IIf(txtFDM17.Text = "", "", txtFDM17.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 17) = IIf(txtFDD17.Text = "", "", txtFDD17.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 17) = IIf(ckbDel17.Checked = True, 1, 0)

            strFDY(aintPAGE, 18) = txtFDY18.Text
            strFDM(aintPAGE, 18) = IIf(txtFDM18.Text = "", "", txtFDM18.Text.PadLeft(2, "0"))
            strFDD(aintPAGE, 18) = IIf(txtFDD18.Text = "", "", txtFDD18.Text.PadLeft(2, "0"))
            intDEL(aintPAGE, 18) = IIf(ckbDel18.Checked = True, 1, 0)

        Catch ex As Exception
            Throw
        End Try

        Return True
    End Function

#Region "リフレッシュ"
    Private Sub btnREFLESH_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        '============================================================================
        'NAME           :btnREFLESH_Click
        'Parameter      :
        'Description    :クリア押下時の処理
        'Return         :
        'Create         :2006/01/30
        'Update         :
        '============================================================================

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)開始", "成功", "")

            If MessageBox.Show(MSG0047I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then Return

            ReDim strTORI_CODE(500, 18)
            ReDim strTORI_NAME(500, 18)
            ReDim strBAITAI(500, 18)
            ReDim strFURI_DATE(500, 18)
            ReDim strSFURI_DATE(500, 18)
            'ReDim dblSYORI_KEN(500, 18)
            'ReDim dblSYORI_KIN(500, 18)
            ReDim strMOTIKOMI(500, 18)
            ReDim strFDY(500, 18)
            ReDim strFDM(500, 18)
            ReDim strFDD(500, 18)
            ReDim intDEL(500, 18)

            ''--------------------------------------
            ''画面のクリア
            ''--------------------------------------
            btnNextGamen.Enabled = False
            btnPreGamen.Enabled = False
            btnAction.Enabled = False
            btnClear.Enabled = False
            CmdSelect.Enabled = True

            txtFuriDateY1.Enabled = True
            txtFuriDateM1.Enabled = True
            txtFuriDateD1.Enabled = True
            txtFuriDateY2.Enabled = True
            txtFuriDateM2.Enabled = True
            txtFuriDateD2.Enabled = True

            Call fn_GAMEN_CLEAR()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)終了", "成功", "")
        End Try


    End Sub
#End Region
    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :実行ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/07/14
        'Update         :
        '============================================================================
        fn_check_text = False
        If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateY1.Text, "振替年", msgTitle) = False Then
            txtFuriDateY1.Focus()
            Exit Function
        End If

        If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateM1.Text, "振替月", msgTitle) = False Then
            txtFuriDateM1.Focus()
            Exit Function
        Else
            If txtFuriDateM1.Text < 1 Or txtFuriDateM1.Text > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM1.Focus()
                Exit Function
            End If
        End If

        If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateD1.Text, "振替日", msgTitle) = False Then
            txtFuriDateD1.Focus()
            Exit Function
        Else
            If txtFuriDateD1.Text < 1 Or txtFuriDateD1.Text > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD1.Focus()
                Exit Function
            End If
        End If

        Dim WORK_DATE As String = txtFuriDateY1.Text & "/" & txtFuriDateM1.Text & "/" & txtFuriDateD1.Text
        If Information.IsDate(WORK_DATE) = False Then
            MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY1.Focus()
            Return False
        End If

        If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateY2.Text, "振替年", msgTitle) = False Then
            txtFuriDateY2.Focus()
            Exit Function
        End If

        If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateM2.Text, "振替月", msgTitle) = False Then
            txtFuriDateM2.Focus()
            Exit Function
        Else
            If txtFuriDateM2.Text < 1 Or txtFuriDateM2.Text > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateM2.Focus()
                Exit Function
            End If
        End If

        If clsFUSION.fn_CHECK_NUM_MSG(txtFuriDateD2.Text, "振替日", msgTitle) = False Then
            txtFuriDateD2.Focus()
            Exit Function
        Else
            If txtFuriDateD2.Text < 1 Or txtFuriDateD2.Text > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateD2.Focus()
                Exit Function
            End If
        End If

        Dim WORK_DATE2 As String = txtFuriDateY2.Text & "/" & txtFuriDateM2.Text & "/" & txtFuriDateD2.Text
        If Information.IsDate(WORK_DATE) = False Then
            MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY2.Focus()
            Return False
        End If

        '日付前後チェック
        If WORK_DATE > WORK_DATE2 Then
            MessageBox.Show(MSG0099W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFuriDateY1.Focus()
            Return False
        End If

        fn_check_text = True
    End Function

    Private Sub txtFDY1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtFDY1.TextChanged, txtFDY2.TextChanged, txtFDY3.TextChanged, txtFDY4.TextChanged, txtFDY5.TextChanged, txtFDY6.TextChanged, txtFDY7.TextChanged, txtFDY8.TextChanged, txtFDY9.TextChanged, txtFDY10.TextChanged, txtFDY11.TextChanged, txtFDY12.TextChanged, txtFDY13.TextChanged, txtFDY14.TextChanged, txtFDY15.TextChanged, txtFDY16.TextChanged, txtFDY17.TextChanged, txtFDY18.TextChanged
        Dim strtxtNM As String
        Dim strINDEX As String

        strtxtNM = sender.Name
        If strtxtNM.Length = 7 Then
            strINDEX = strtxtNM.Substring(6, 1)
        Else
            strINDEX = strtxtNM.Substring(6, 2)
        End If

        If objFDY(strINDEX).Text.Length = 4 Then
            '数値ﾁｪｯｸ
            If clsFUSION.fn_CHECK_NUM_MSG(objFDY(strINDEX).Text, "振替年", msgTitle) = True Then
                objFDM(strINDEX).Focus()  'ﾌｫｰｶｽ移動
            Else
                objFDY(strINDEX).Focus()
                objFDY(strINDEX).SelectionStart = 0
                objFDY(strINDEX).SelectionLength = 4
            End If
        Else
        End If

    End Sub
    Function fn_set_Control() As Boolean
        '============================================================================
        'NAME           :fn_set_Control
        'Parameter      :
        'Description    :コントロール配列セット
        'Return         :True=OK,False=NG
        'Create         :2004/08/30
        'Update         :
        '============================================================================
        Try

            fn_set_Control = False

            objFDY(1) = txtFDY1
            objFDY(2) = txtFDY2
            objFDY(3) = txtFDY3
            objFDY(4) = txtFDY4
            objFDY(5) = txtFDY5
            objFDY(6) = txtFDY6
            objFDY(7) = txtFDY7
            objFDY(8) = txtFDY8
            objFDY(9) = txtFDY9
            objFDY(10) = txtFDY10
            objFDY(11) = txtFDY11
            objFDY(12) = txtFDY12
            objFDY(13) = txtFDY13
            objFDY(14) = txtFDY14
            objFDY(15) = txtFDY15
            objFDY(16) = txtFDY16
            objFDY(17) = txtFDY17
            objFDY(18) = txtFDY18

            objFDM(1) = txtFDM1
            objFDM(2) = txtFDM2
            objFDM(3) = txtFDM3
            objFDM(4) = txtFDM4
            objFDM(5) = txtFDM5
            objFDM(6) = txtFDM6
            objFDM(7) = txtFDM7
            objFDM(8) = txtFDM8
            objFDM(9) = txtFDM9
            objFDM(10) = txtFDM10
            objFDM(11) = txtFDM11
            objFDM(12) = txtFDM12
            objFDM(13) = txtFDM13
            objFDM(14) = txtFDM14
            objFDM(15) = txtFDM15
            objFDM(16) = txtFDM16
            objFDM(17) = txtFDM17
            objFDM(18) = txtFDM18

            objFDD(1) = txtFDD1
            objFDD(2) = txtFDD2
            objFDD(3) = txtFDD3
            objFDD(4) = txtFDD4
            objFDD(5) = txtFDD5
            objFDD(6) = txtFDD6
            objFDD(7) = txtFDD7
            objFDD(8) = txtFDD8
            objFDD(9) = txtFDD9
            objFDD(10) = txtFDD10
            objFDD(11) = txtFDD11
            objFDD(12) = txtFDD12
            objFDD(13) = txtFDD13
            objFDD(14) = txtFDD14
            objFDD(15) = txtFDD15
            objFDD(16) = txtFDD16
            objFDD(17) = txtFDD17
            objFDD(18) = txtFDD18

            Return True
        Catch ex As Exception
            Throw
        End Try

    End Function
    Private Sub txtFDY1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtFDY1.KeyPress, txtFDY2.KeyPress, txtFDY3.KeyPress, txtFDY4.KeyPress, txtFDY5.KeyPress, txtFDY6.KeyPress, txtFDY7.KeyPress, txtFDY8.KeyPress, txtFDY9.KeyPress, txtFDY10.KeyPress, txtFDY11.KeyPress, txtFDY12.KeyPress, txtFDY13.KeyPress, txtFDY14.KeyPress, txtFDY15.KeyPress, txtFDY16.KeyPress, txtFDY17.KeyPress, txtFDY18.KeyPress
        Dim strtxtNM As String
        Dim strINDEX As String
        Dim KeyAscii As Short = Asc(e.KeyChar)
        strtxtNM = sender.Name
        If strtxtNM.Length = 7 Then
            strINDEX = strtxtNM.Substring(6, 1)
        Else
            strINDEX = strtxtNM.Substring(6, 2)
        End If

        If KeyAscii = Keys.Enter Then 'Enterｷｰ
            If objFDY(strINDEX).Text.Length <> 0 Then
                '数値ﾁｪｯｸ
                If clsFUSION.fn_CHECK_NUM_MSG(objFDY(strINDEX).Text, "振替年", msgTitle) = True Then
                    objFDY(strINDEX).Text = objFDY(strINDEX).Text.PadLeft(4, "0")
                    KeyAscii = 0
                    objFDM(strINDEX).Focus()   'ﾌｫｰｶｽ移動
                    Exit Sub
                Else
                    KeyAscii = 0
                    objFDY(strINDEX).Focus()
                    objFDY(strINDEX).SelectionStart = 0
                    objFDY(strINDEX).SelectionLength = 4
                    Exit Sub
                End If
            End If
            KeyAscii = 0
            objFDM(strINDEX).Focus()     'ﾌｫｰｶｽ移動
        End If
    End Sub

    Private Sub txtFDM1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtFDM1.TextChanged, txtFDM2.TextChanged, txtFDM3.TextChanged, txtFDM4.TextChanged, txtFDM5.TextChanged, txtFDM6.TextChanged, txtFDM7.TextChanged, txtFDM8.TextChanged, txtFDM9.TextChanged, txtFDM10.TextChanged, txtFDM11.TextChanged, txtFDM12.TextChanged, txtFDM13.TextChanged, txtFDM14.TextChanged, txtFDM15.TextChanged, txtFDM16.TextChanged, txtFDM17.TextChanged, txtFDM18.TextChanged
        Dim strtxtNM As String
        Dim strINDEX As String

        strtxtNM = sender.Name
        If strtxtNM.Length = 7 Then
            strINDEX = strtxtNM.Substring(6, 1)
        Else
            strINDEX = strtxtNM.Substring(6, 2)
        End If

        If objFDM(strINDEX).Text.Length = 2 Then
            '数値ﾁｪｯｸ
            If clsFUSION.fn_CHECK_NUM_MSG(objFDM(strINDEX).Text, "振替月", msgTitle) = True Then
                If objFDM(strINDEX).Text < 1 Or objFDM(strINDEX).Text > 12 Then
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    objFDM(strINDEX).Focus()
                    objFDM(strINDEX).SelectionStart = 0
                    objFDM(strINDEX).SelectionLength = 2
                    Exit Sub
                End If
                objFDD(strINDEX).Focus()  'ﾌｫｰｶｽ移動
            Else
                objFDM(strINDEX).Focus()
                objFDM(strINDEX).SelectionStart = 0
                objFDM(strINDEX).SelectionLength = 2
            End If
        Else
        End If

    End Sub

    Private Sub txtFDM1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtFDM1.KeyPress, txtFDM2.KeyPress, txtFDM3.KeyPress, txtFDM4.KeyPress, txtFDM5.KeyPress, txtFDM6.KeyPress, txtFDM7.KeyPress, txtFDM8.KeyPress, txtFDM9.KeyPress, txtFDM10.KeyPress, txtFDM11.KeyPress, txtFDM12.KeyPress, txtFDM13.KeyPress, txtFDM14.KeyPress, txtFDM15.KeyPress, txtFDM16.KeyPress, txtFDM17.KeyPress, txtFDM18.KeyPress
        Dim strtxtNM As String
        Dim strINDEX As String
        Dim KeyAscii As Short = Asc(e.KeyChar)

        strtxtNM = sender.Name
        If strtxtNM.Length = 7 Then
            strINDEX = strtxtNM.Substring(6, 1)
        Else
            strINDEX = strtxtNM.Substring(6, 2)
        End If

        If KeyAscii = Keys.Enter Then 'Enterｷｰ
            If objFDM(strINDEX).Text.Length <> 0 Then
                '数値ﾁｪｯｸ
                If clsFUSION.fn_CHECK_NUM_MSG(objFDM(strINDEX).Text, "対象月", msgTitle) = True Then
                    objFDM(strINDEX).Text = objFDM(strINDEX).Text.PadLeft(2, "0")
                    If objFDM(strINDEX).Text < 0 Or objFDM(strINDEX).Text > 12 Then
                        MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    KeyAscii = 0
                    objFDD(strINDEX).Focus()   'ﾌｫｰｶｽ移動
                    Exit Sub
                Else
                    KeyAscii = 0
                    objFDM(strINDEX).Focus()
                    objFDM(strINDEX).SelectionStart = 0
                    objFDM(strINDEX).SelectionLength = 2
                    Exit Sub
                End If
            End If
            KeyAscii = 0
        End If

    End Sub

    Private Sub txtFDD1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtFDD1.TextChanged, txtFDD2.TextChanged, txtFDD3.TextChanged, txtFDD4.TextChanged, txtFDD5.TextChanged, txtFDD6.TextChanged, txtFDD7.TextChanged, txtFDD8.TextChanged, txtFDD9.TextChanged, txtFDD10.TextChanged, txtFDD11.TextChanged, txtFDD12.TextChanged, txtFDD13.TextChanged, txtFDD14.TextChanged, txtFDD15.TextChanged, txtFDD16.TextChanged, txtFDD17.TextChanged, txtFDD18.TextChanged
        Dim strtxtNM As String
        Dim strINDEX As String

        strtxtNM = sender.Name
        If strtxtNM.Length = 7 Then
            strINDEX = strtxtNM.Substring(6, 1)
        Else
            strINDEX = strtxtNM.Substring(6, 2)
        End If

        If objFDD(strINDEX).Text.Length = 2 Then
            '数値ﾁｪｯｸ
            If clsFUSION.fn_CHECK_NUM_MSG(objFDD(strINDEX).Text, "振替月", msgTitle) = True Then
                If objFDD(strINDEX).Text < 1 Or objFDD(strINDEX).Text > 31 Then
                    MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    objFDD(strINDEX).Focus()
                    objFDD(strINDEX).SelectionStart = 0
                    objFDD(strINDEX).SelectionLength = 2
                    Exit Sub
                End If
                '新振替日に値が入っていたら、営業日判定する
                Dim strDATE_Y As String, strDATE_M As String, strDATE_D As String
                '2012/01/13 saitou 標準修正 警告回避 MODIFY ---------------------------->>>>
                Dim strOUT_Y As String = String.Empty
                Dim strOUT_M As String = String.Empty
                Dim strOUT_D As String = String.Empty
                'Dim strOUT_Y As String, strOUT_M As String, strOUT_D As String
                '2012/01/13 saitou 標準修正 警告回避 MODIFY ----------------------------<<<<

                strDATE_Y = objFDY(strINDEX).Text
                strDATE_M = objFDM(strINDEX).Text.PadLeft(2, "0")
                strDATE_D = objFDD(strINDEX).Text.PadLeft(2, "0")

                '2013/03/19 saitou 標準修正 UPD -------------------------------------------------->>>>
                '共通関数一元化
                If GCom.CheckDateModule(strDATE_Y & strDATE_M & strDATE_D, "", 0) = False Then
                    MessageBox.Show(MSG0320W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    objFDY(strINDEX).Focus()
                    objFDY(strINDEX).SelectionStart = 0
                    objFDY(strINDEX).SelectionLength = 4
                    Exit Sub
                End If

                'If fn_set_EIGYOBI(strDATE_Y, strDATE_M, strDATE_D, 0, gintSYORI_KBN.BEFORE, strOUT_Y, strOUT_M, strOUT_D) = False Then
                '    Exit Sub
                'End If

                'If strDATE_Y & strDATE_M & strDATE_D <> strOUT_Y & strOUT_M & strOUT_D Then
                '    MessageBox.Show(MSG0320W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                '    objFDY(strINDEX).Focus()
                '    objFDY(strINDEX).SelectionStart = 0
                '    objFDY(strINDEX).SelectionLength = 4
                '    Exit Sub
                'End If
                '2013/03/19 saitou 標準修正 UPD --------------------------------------------------<<<<

                If strINDEX = 18 Then
                    btnNextGamen.Focus() 'ﾌｫｰｶｽ移動
                Else
                    objFDY(strINDEX + 1).Focus() 'ﾌｫｰｶｽ移動
                End If
            Else
                objFDD(strINDEX).Focus()
                objFDD(strINDEX).SelectionStart = 0
                objFDD(strINDEX).SelectionLength = 2
            End If
        Else
        End If
    End Sub

    Private Sub txtFDD1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtFDD1.KeyPress, txtFDD2.KeyPress, txtFDD3.KeyPress, txtFDD4.KeyPress, txtFDD5.KeyPress, txtFDD6.KeyPress, txtFDD7.KeyPress, txtFDD8.KeyPress, txtFDD9.KeyPress, txtFDD10.KeyPress, txtFDD11.KeyPress, txtFDD12.KeyPress, txtFDD13.KeyPress, txtFDD14.KeyPress, txtFDD15.KeyPress, txtFDD16.KeyPress, txtFDD17.KeyPress, txtFDD18.KeyPress
        Dim strtxtNM As String
        Dim strINDEX As String
        Dim KeyAscii As Short = Asc(e.KeyChar)

        strtxtNM = sender.Name
        If strtxtNM.Length = 7 Then
            strINDEX = strtxtNM.Substring(6, 1)
        Else
            strINDEX = strtxtNM.Substring(6, 2)
        End If

        If KeyAscii = Keys.Enter Then 'Enterｷｰ
            If objFDD(strINDEX).Text.Length <> 0 Then
                '数値ﾁｪｯｸ
                If clsFUSION.fn_CHECK_NUM_MSG(objFDD(strINDEX).Text, "対象月", msgTitle) = True Then
                    objFDD(strINDEX).Text = objFDD(strINDEX).Text.PadLeft(2, "0")
                    If objFDD(strINDEX).Text < 0 Or objFDD(strINDEX).Text > 31 Then
                        MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    KeyAscii = 0
                    '新振替日に値が入っていたら、営業日判定する
                    Dim strDATE_Y As String, strDATE_M As String, strDATE_D As String
                    Dim strOUT_Y As String = "", strOUT_M As String = "", strOUT_D As String = ""
                    strDATE_Y = objFDY(strINDEX).Text
                    strDATE_M = objFDM(strINDEX).Text.PadLeft(2, "0")
                    strDATE_D = objFDD(strINDEX).Text.PadLeft(2, "0")

                    '2013/03/19 saitou 標準修正 UPD -------------------------------------------------->>>>
                    '共通関数一元化
                    If GCom.CheckDateModule(strDATE_Y & strDATE_M & strDATE_D, "", 0) = False Then
                        MessageBox.Show(MSG0320W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        objFDY(strINDEX).Focus()
                        objFDY(strINDEX).SelectionStart = 0
                        objFDY(strINDEX).SelectionLength = 4
                        Exit Sub
                    End If

                    'If fn_set_EIGYOBI(strDATE_Y, strDATE_M, strDATE_D, 0, gintSYORI_KBN.BEFORE, strOUT_Y, strOUT_M, strOUT_D) = False Then
                    '    Exit Sub
                    'End If

                    'If strDATE_Y & strDATE_M & strDATE_D <> strOUT_Y & strOUT_M & strOUT_D Then
                    '    MessageBox.Show(MSG0320W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    '    objFDY(strINDEX).Focus()
                    '    objFDY(strINDEX).SelectionStart = 0
                    '    objFDY(strINDEX).SelectionLength = 4
                    '    Exit Sub
                    'End If
                    '2013/03/19 saitou 標準修正 UPD --------------------------------------------------<<<<

                    If strINDEX = 18 Then
                        btnNextGamen.Focus() 'ﾌｫｰｶｽ移動
                    Else
                        objFDY(strINDEX + 1).Focus() 'ﾌｫｰｶｽ移動
                    End If
                    Exit Sub
                Else
                    KeyAscii = 0
                    objFDD(strINDEX).Focus()
                    objFDD(strINDEX).SelectionStart = 0
                    objFDD(strINDEX).SelectionLength = 2
                    Exit Sub
                End If
            End If
            KeyAscii = 0
        End If
    End Sub

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click


        Dim bRet As Boolean
        Dim strTorisCode As String
        Dim strTorifCode As String
        Dim MSG As String = ""
        Dim Ret As Integer = 0

        Dim MsgIcon As MessageBoxIcon = MessageBoxIcon.Information

        Dim CLsSCH As New ClsSchduleMaintenanceClass


        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")

            '画面の振替日の日付チェック
            If CheckFuriDate() = False Then
                MsgIcon = MessageBoxIcon.None
                Return
            End If

            CLsSCH.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                MsgIcon = Nothing
                Return
            End If

            '休日情報の蓄積
            Call CLsSCH.SetKyuzituInformation()

            'SCHMAST項目名の蓄積
            Call CLsSCH.SetSchMastInformation()

            '--------------------------------------
            '画面の保存
            '--------------------------------------
            Call fn_GAMEN_HOZON(intPAGE_CNT)

            'トランザクション開始
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Begin)

            Dim i As Integer, j As Integer
            Dim intDEL_KEN As Integer = 0
            For i = 1 To intMAXCNT
                For j = 1 To 18
                    If i = intMAXCNT Then
                        If j = intMAXLINE + 1 Then
                            Exit For
                        End If
                    End If

                    strTorisCode = strTORI_CODE(i, j).Replace("-", "").Substring(0, 10)
                    strTorifCode = strTORI_CODE(i, j).Replace("-", "").Substring(10, 2)
                    CLsSCH.SCH.FURI_DATE = strFURI_DATE(i, j).Replace("/", "").Substring(0, 4) & strFURI_DATE(i, j).Replace("/", "").Substring(4, 2) & strFURI_DATE(i, j).Replace("/", "").Substring(6, 2)

                    CLsSCH.SCH.WRK_SFURI_YDATE = strSFURI_DATE(i, j)  '再振日

                    '取引先マスタに取引先コードが存在することを確認
                    bRet = CLsSCH.GET_SELECT_TORIMAST(Nothing, _
                                strTorisCode, strTorifCode, ClsSchduleMaintenanceClass.OPT.OptionNothing)

                    If Not bRet Then
                        MSG = MSG0063W
                        Return
                    End If

                    ''削除チェックが入っていたら削除する
                    If intDEL(i, j) = 1 Then
                        'スケジュールマスタから該当既存レコードを削除
                        If CLsSCH.DELETE_SCHMAST() = False Then
                            MSG = String.Format(MSG0027E, "スケジュールマスタ", "削除")
                            MsgIcon = MessageBoxIcon.Error
                            Return
                        End If
                    End If


                    'ここから変更パターン
                    strNEW_FURIDATE = strFDY(i, j) & strFDM(i, j) & strFDD(i, j)
                    If strFDY(i, j) <> "" Then
                        '旧の振替日と新の振替日が一致したら、メッセージを表示し、中断する
                        '旧振替日を削除する前に新振替日が存在しないことを確認()
                        If CLsSCH.SCH.FURI_DATE = strNEW_FURIDATE Or CLsSCH.SEARCH_NEW_SCHMAST(strTorisCode, strTorifCode, strNEW_FURIDATE) = False Then
                            If CLsSCH.SCH.FURI_DATE = strNEW_FURIDATE Then
                                MSG = String.Format(MSG0318W, strTorisCode, strTorifCode, CLsSCH.SCH.FURI_DATE.Substring(0, 4) & _
                                               "年" & CLsSCH.SCH.FURI_DATE.Substring(4, 2) & "月" & CLsSCH.SCH.FURI_DATE.Substring(6, 2) & "日")
                                MsgIcon = MessageBoxIcon.Warning
                                Return
                            Else
                                MSG = String.Format(MSG0319W, strTorisCode, strTorifCode, strNEW_FURIDATE.Substring(0, 4) & _
                                               "年" & strNEW_FURIDATE.Substring(4, 2) & "月" & strNEW_FURIDATE.Substring(6, 2) & "日")
                                MsgIcon = MessageBoxIcon.Warning
                                Return
                            End If
                        Else
                            'スケジュールマスタから該当既存レコードを削除

                            If CLsSCH.DELETE_SCHMAST(False) = False Then
                                MSG = String.Format(MSG0027E, "スケジュールマスタ", "削除")
                                MsgIcon = MessageBoxIcon.Error
                                Return
                            End If

                            '登録行為の実行
                            CLsSCH.SCH.FURI_DATE = strNEW_FURIDATE     'これでいいのか・・・
                            CLsSCH.SCH.KFURI_DATE = strNEW_FURIDATE     'これでいいのか・・・
                            If CLsSCH.INSERT_NEW_SCHMAST(0) = True Then

                                If CLsSCH.TR(0).SFURI_FLG = 1 Then      '再振のスケジュールも作り変え

                                    Dim wrkTorifCode As String = CLsSCH.TR(0).TORIF_CODE
                                    Dim wrkSfuriFlg As Integer = CLsSCH.TR(0).SFURI_FLG
                                    Dim wrkFuriDate As String = CLsSCH.SCH.FURI_DATE
                                    CLsSCH.TR(0).TORIF_CODE = CLsSCH.TR(0).SFURI_FCODE
                                    CLsSCH.TR(0).SFURI_FLG = 0
                                    CLsSCH.SCH.FURI_DATE = CLsSCH.SCH.KSAIFURI_DATE

                                    If CLsSCH.INSERT_NEW_SCHMAST(0) = True Then

                                        CLsSCH.TR(0).TORIF_CODE = wrkTorifCode
                                        CLsSCH.TR(0).SFURI_FLG = wrkSfuriFlg
                                        CLsSCH.SCH.FURI_DATE = wrkFuriDate

                                        MSG = MSG0004I
                                        MsgIcon = MessageBoxIcon.Information

                                    Else

                                        CLsSCH.TR(0).TORIF_CODE = wrkTorifCode
                                        CLsSCH.TR(0).SFURI_FLG = wrkSfuriFlg
                                        CLsSCH.SCH.FURI_DATE = wrkFuriDate

                                        MSG = MSG0002E.Replace("{0}", "登録")
                                        MsgIcon = MessageBoxIcon.Error
                                        Return
                                    End If
                                Else

                                    MSG = MSG0004I
                                    MsgIcon = MessageBoxIcon.Information

                                End If

                            Else
                                MSG = MSG0002E.Replace("{0}", "登録")
                                MsgIcon = MessageBoxIcon.Error
                                Return
                            End If
                        End If
                    End If
                Next
            Next

            '--------------------------------------
            '画面のクリア
            '--------------------------------------
            ReDim strTORI_CODE(500, 18)
            ReDim strTORI_NAME(500, 18)
            ReDim strBAITAI(500, 18)
            ReDim strFURI_DATE(500, 18)
            ReDim strSFURI_DATE(500, 18)
            'ReDim dblSYORI_KEN(500, 18)
            'ReDim dblSYORI_KIN(500, 18)
            ReDim strMOTIKOMI(500, 18)
            ReDim strFDY(500, 18)
            ReDim strFDM(500, 18)
            ReDim strFDD(500, 18)
            ReDim intDEL(500, 18)

            btnNextGamen.Enabled = False
            btnPreGamen.Enabled = False
            btnAction.Enabled = False
            btnClear.Enabled = False
            CmdSelect.Enabled = True

            txtFuriDateY1.Enabled = True
            txtFuriDateM1.Enabled = True
            txtFuriDateD1.Enabled = True
            txtFuriDateY2.Enabled = True
            txtFuriDateM2.Enabled = True
            txtFuriDateD2.Enabled = True
            Call fn_GAMEN_CLEAR()

        Catch ex As Exception
            MsgIcon = MessageBoxIcon.Error
            MSG = MSG0006E
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)例外エラー", "成功", ex.Message)
        Finally
            Select Case MsgIcon
                Case MessageBoxIcon.Information
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)
                    Call MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MsgIcon)
                Case MessageBoxIcon.Error
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
                Case MessageBoxIcon.Warning
                    Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
                    Call MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MsgIcon)
            End Select
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
        End Try

    End Sub

    Private Sub ckbDel1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ckbDel1.CheckedChanged, ckbDel2.CheckedChanged, ckbDel3.CheckedChanged, ckbDel4.CheckedChanged, ckbDel5.CheckedChanged, ckbDel6.CheckedChanged, ckbDel7.CheckedChanged, ckbDel8.CheckedChanged, ckbDel9.CheckedChanged, ckbDel10.CheckedChanged, ckbDel11.CheckedChanged, ckbDel12.CheckedChanged, ckbDel13.CheckedChanged, ckbDel14.CheckedChanged, ckbDel15.CheckedChanged, ckbDel16.CheckedChanged, ckbDel17.CheckedChanged, ckbDel18.CheckedChanged
        Dim strtxtNM As String
        Dim strINDEX As String

        strtxtNM = sender.Name
        If strtxtNM.Length = 7 Then
            strINDEX = strtxtNM.Substring(6, 1)
        Else
            strINDEX = strtxtNM.Substring(6, 2)
        End If

        If sender.checked = True Then
            objFDY(strINDEX).Text = ""
            objFDM(strINDEX).Text = ""
            objFDD(strINDEX).Text = ""
            objFDY(strINDEX).Enabled = False
            objFDM(strINDEX).Enabled = False
            objFDD(strINDEX).Enabled = False
        Else
            objFDY(strINDEX).Enabled = True
            objFDM(strINDEX).Enabled = True
            objFDD(strINDEX).Enabled = True
        End If
    End Sub

    Private Sub CmdSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmdSelect.Click

        Dim oraDB As New CASTCommon.MyOracle

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            '危険なので念のため初期化
            ReDim strTORI_CODE(500, 18)
            ReDim strTORI_NAME(500, 18)
            ReDim strBAITAI(500, 18)
            ReDim strFURI_DATE(500, 18)
            ReDim strSFURI_DATE(500, 18)
            'ReDim dblSYORI_KEN(500, 18)
            'ReDim dblSYORI_KIN(500, 18)
            ReDim strMOTIKOMI(500, 18)
            ReDim strFDY(500, 18)
            ReDim strFDM(500, 18)
            ReDim strFDD(500, 18)
            ReDim intDEL(500, 18)

            '--------------------------------------
            '名細部のクリア
            '--------------------------------------
            Call fn_GAMEN_CLEAR()

            If fn_check_text() = False Then
                Exit Sub
            End If

            '-----------------------------------
            '対象スケジュールの検索
            '-----------------------------------
            If fn_SCHMAST_KENSAKU(oraDB) = False Then
                MessageBox.Show(MSG0055W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY1.Focus()
                Exit Sub
            End If

            '-----------------------------------
            'スケジュールの表示
            '-----------------------------------
            intPAGE_CNT = 1
            If fn_GAMEN_HYOUJI(intPAGE_CNT) = False Then
                MessageBox.Show(MSG0253W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            '### ボタンの切替
            txtFuriDateY1.Enabled = False
            txtFuriDateM1.Enabled = False
            txtFuriDateD1.Enabled = False
            txtFuriDateY2.Enabled = False
            txtFuriDateM2.Enabled = False
            txtFuriDateD2.Enabled = False
            If intMAXCNT = 0 Then
                btnPreGamen.Enabled = False
                btnNextGamen.Enabled = False
            Else
                btnPreGamen.Enabled = True
                btnNextGamen.Enabled = True
            End If
            btnClear.Enabled = True
            btnAction.Enabled = True
            CmdSelect.Enabled = False

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)
        Finally
            If Not oraDB Is Nothing Then oraDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
        End Try
    End Sub

    Private Sub TxtDate_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
Handles txtFuriDateY1.Validating, txtFuriDateM1.Validating, txtFuriDateD1.Validating, _
        txtFuriDateY2.Validating, txtFuriDateM2.Validating, txtFuriDateD2.Validating, _
        txtFDY1.Validating, txtFDM1.Validating, txtFDD1.Validating, _
        txtFDY2.Validating, txtFDM2.Validating, txtFDD2.Validating, _
        txtFDY3.Validating, txtFDM3.Validating, txtFDD3.Validating, _
        txtFDY4.Validating, txtFDM4.Validating, txtFDD4.Validating, _
        txtFDY5.Validating, txtFDM5.Validating, txtFDD5.Validating, _
        txtFDY6.Validating, txtFDM6.Validating, txtFDD6.Validating, _
        txtFDY7.Validating, txtFDM7.Validating, txtFDD7.Validating, _
        txtFDY8.Validating, txtFDM8.Validating, txtFDD8.Validating, _
        txtFDY9.Validating, txtFDM9.Validating, txtFDD9.Validating, _
        txtFDY10.Validating, txtFDM10.Validating, txtFDD10.Validating, _
        txtFDY11.Validating, txtFDM11.Validating, txtFDD11.Validating, _
        txtFDY12.Validating, txtFDM12.Validating, txtFDD12.Validating, _
        txtFDY13.Validating, txtFDM13.Validating, txtFDD13.Validating, _
        txtFDY14.Validating, txtFDM14.Validating, txtFDD14.Validating, _
        txtFDY15.Validating, txtFDM15.Validating, txtFDD15.Validating, _
        txtFDY16.Validating, txtFDM16.Validating, txtFDD16.Validating, _
        txtFDY17.Validating, txtFDM17.Validating, txtFDD17.Validating, _
        txtFDY18.Validating, txtFDM18.Validating, txtFDD18.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            Throw
        End Try

    End Sub

    Private Function CheckFuriDate() As Boolean
        Try
            CheckFuriDate = False
            For No As Integer = 1 To 18
                Dim FuriDateY As TextBox = Nothing, FuriDateM As TextBox = Nothing, _
                    FuriDateD As TextBox = Nothing
                Select Case No
                    Case 1
                        FuriDateY = txtFDY1
                        FuriDateM = txtFDM1
                        FuriDateD = txtFDD1
                    Case 2
                        FuriDateY = txtFDY2
                        FuriDateM = txtFDM2
                        FuriDateD = txtFDD2
                    Case 3
                        FuriDateY = txtFDY3
                        FuriDateM = txtFDM3
                        FuriDateD = txtFDD3
                    Case 4
                        FuriDateY = txtFDY4
                        FuriDateM = txtFDM4
                        FuriDateD = txtFDD4
                    Case 5
                        FuriDateY = txtFDY5
                        FuriDateM = txtFDM5
                        FuriDateD = txtFDD5
                    Case 6
                        FuriDateY = txtFDY6
                        FuriDateM = txtFDM6
                        FuriDateD = txtFDD6
                    Case 7
                        FuriDateY = txtFDY7
                        FuriDateM = txtFDM7
                        FuriDateD = txtFDD7
                    Case 8
                        FuriDateY = txtFDY8
                        FuriDateM = txtFDM8
                        FuriDateD = txtFDD8
                    Case 9
                        FuriDateY = txtFDY9
                        FuriDateM = txtFDM9
                        FuriDateD = txtFDD9
                    Case 10
                        FuriDateY = txtFDY10
                        FuriDateM = txtFDM10
                        FuriDateD = txtFDD10
                    Case 11
                        FuriDateY = txtFDY11
                        FuriDateM = txtFDM11
                        FuriDateD = txtFDD11
                    Case 12
                        FuriDateY = txtFDY12
                        FuriDateM = txtFDM12
                        FuriDateD = txtFDD12
                    Case 13
                        FuriDateY = txtFDY13
                        FuriDateM = txtFDM13
                        FuriDateD = txtFDD13
                    Case 14
                        FuriDateY = txtFDY14
                        FuriDateM = txtFDM14
                        FuriDateD = txtFDD14
                    Case 15
                        FuriDateY = txtFDY15
                        FuriDateM = txtFDM15
                        FuriDateD = txtFDD15
                    Case 16
                        FuriDateY = txtFDY16
                        FuriDateM = txtFDM16
                        FuriDateD = txtFDD16
                    Case 17
                        FuriDateY = txtFDY17
                        FuriDateM = txtFDM17
                        FuriDateD = txtFDD17
                    Case 18
                        FuriDateY = txtFDY18
                        FuriDateM = txtFDM18
                        FuriDateD = txtFDD18
                End Select
                If Trim(FuriDateY.Text & FuriDateM.Text & FuriDateD.Text) <> "" Then
                    '年必須チェック
                    If FuriDateY.Text.Trim = "" Then
                        MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        FuriDateY.Focus()
                        Return False
                    End If
                    '月必須チェック
                    If FuriDateM.Text.Trim = "" Then
                        MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        FuriDateM.Focus()
                        Return False
                    End If
                    '月範囲チェック
                    If GCom.NzInt(FuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(FuriDateM.Text.Trim) > 12 Then
                        MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        FuriDateM.Focus()
                        Return False
                    End If
                    '日付必須チェック
                    If FuriDateD.Text.Trim = "" Then
                        MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        FuriDateD.Focus()
                        Return False
                    End If
                    '日付範囲チェック
                    If GCom.NzInt(FuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(FuriDateD.Text.Trim) > 31 Then
                        MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        FuriDateD.Focus()
                        Return False
                    End If

                    '日付整合性チェック
                    Dim WORK_DATE As String = FuriDateY.Text & "/" & FuriDateM.Text & "/" & FuriDateD.Text
                    If Information.IsDate(WORK_DATE) = False Then
                        MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        FuriDateY.Focus()
                        Return False
                    End If
                End If
            Next

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        Finally

        End Try
        CheckFuriDate = True

    End Function
End Class