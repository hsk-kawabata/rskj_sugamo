Option Explicit On 
Option Strict Off

Imports System.data
Imports System.Data.SqlClient
Imports System.Data.SqlDbType
Imports CASTCommon
Imports System.Text
Public Class KFGMAST020

    Private btnTakouKin(20) As Button
    Private txtKinCode(20) As TextBox
    Private txtKinNName(20) As TextBox
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST020", "学校他行マスタメンテナンス画面")
    Private Const msgTitle As String = "学校他行マスタメンテナンス画面(KFGMAST020)"
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite


#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGMAST020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim mycontrol As Control

        '#####################################
        'ログの書込に必要な情報の取得
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            STR_SYORI_NAME = "学校他行マスタメンテナンス画面"
            MainLOG = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '2016/10/07 saitou RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            'コントロールをグループ化
            For Each mycontrol In Me.Controls
                If mycontrol.Name.Length >= 7 Then
                    Select Case mycontrol.Name.Substring(0, 7)
                        Case "btnTako"
                            AddHandler mycontrol.Click, AddressOf btnTakouKin_Click
                            btnTakouKin(CInt(mycontrol.Name.Substring(11, mycontrol.Name.Length - 11))) = mycontrol
                            '2006/10/23　参照ボタンを押さなくても入力できるように修正
                            'mycontrol.Enabled = False
                        Case "txtKinC"
                            txtKinCode(CInt(mycontrol.Name.Substring(10, mycontrol.Name.Length - 10))) = mycontrol
                        Case "txtKinN"
                            txtKinNName(CInt(mycontrol.Name.Substring(11, mycontrol.Name.Length - 11))) = mycontrol
                    End Select
                End If
            Next

            '学校コンボ設定（全学校）
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME, True) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", "コンボボックス設定(cmbToriName)")
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

                btnFind.Enabled = False
                btnPrint.Enabled = False

                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Activated "

    '2006/10/20　登録後、自動的に読み込み
    Private Sub KFGMAST020_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '参照処理
            Call PSUB_Set_TakouIchiran()
        End If
    End Sub

#End Region

#Region " Button Click "
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)開始", "成功", "")

            '********************************************
            '参照ボタン
            '********************************************
            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                '他行情報一覧設定
                Call PSUB_Set_TakouIchiran()
            Else
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                txtGAKKOU_CODE.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(参照)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '********************************************
        '終了ボタン
        '********************************************
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnTakouKin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        '********************************************
        '他行ボタン（詳細画面に遷移）
        '********************************************
        Dim KFGMAST021 As New KFGMAST021
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(他行ボタン)開始", "成功", "")

            STR_SQL = "SELECT * FROM GAKMAST1,GAKMAST2 "
            STR_SQL += " WHERE GAKKOU_CODE_G = GAKKOU_CODE_T"
            STR_SQL += " AND GAKKOU_CODE_G ='" & Trim(txtGAKKOU_CODE.Text) & "'"
            STR_SQL += " AND TAKO_KBN_T ='1'"

            If GFUNC_ISEXIST(STR_SQL) = False Then

                Call MessageBox.Show(G_MSG0004W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Sub
            End If

            '金融機関コードを渡す
            KFGMAST021.KFGMAST020_KCoad = txtKinCode(sender.Name.Substring(11, sender.Name.Length - 11)).Text
            '学校コードを渡す
            KFGMAST021.KFGMAST020_GCoad = txtGAKKOU_CODE.Text

            KFGMAST021.ShowDialog(Me)

            '他行情報一覧再作成
            Call PSUB_Set_TakouIchiran()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(他行ボタン)終了", "成功", "")
        End Try
    End Sub
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            MainDB = New MyOracle
            OraReader = New MyOracleReader(MainDB)
            '********************************************
            '印刷ボタン
            '********************************************
            '学校コードチェック
            If Trim(txtGAKKOU_CODE.Text) = "" Then

                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                '
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            '存在チェック
            SQL.Append(" SELECT DISTINCT GAKKOU_CODE_V")
            SQL.Append(",TKIN_NO_V")
            SQL.Append(",KIN_NNAME_N")
            SQL.Append(" FROM G_TAKOUMAST")
            SQL.Append(" LEFT JOIN TENMAST ON ")
            SQL.Append(" TKIN_NO_V = KIN_NO_N ")
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                SQL.Append(" WHERE GAKKOU_CODE_V = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
            End If
            SQL.Append(" ORDER BY GAKKOU_CODE_V ")

            If OraReader.DataReader(SQL) = False Then

                Call MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            If MessageBox.Show(String.Format(MSG0013I, "学校他行マスタ一覧"), _
                               msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
                Exit Sub
            End If

            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me
            Dim nRet As Integer
            Dim Param As String

            Param = GCom.GetUserID & "," & txtGAKKOU_CODE.Text
            nRet = ExeRepo.ExecReport("KFGP009.EXE", Param)
            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, "学校他行マスタ一覧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
            End Select
            OraReader.NextRead()

            MessageBox.Show(String.Format(MSG0014I, "学校他行マスタ一覧"), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub
#End Region

#Region " Private Sub "
    '****************************
    'Private Sub
    '****************************
    Private Sub PSUB_Set_TakouIchiran()
        '********************************************
        '他行情報設定
        '********************************************
        Dim Int_Counter As Integer
        Dim Int_Start As Integer
        Dim Str_Ginko_Code As String = ""

        '情報抽出
        STR_SQL = ""
        STR_SQL += " SELECT "
        STR_SQL += " TKIN_NO_V "
        STR_SQL += ",KIN_NNAME_N "
        STR_SQL += " FROM G_TAKOUMAST left join TENMAST on "
        STR_SQL += " TKIN_NO_V = KIN_NO_N "
        STR_SQL += " WHERE GAKKOU_CODE_V = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        STR_SQL += " ORDER BY TKIN_NO_V "

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Sub
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Sub
            End If

            'ボタン再制御
            Call PSUB_Set_TakouButton_Clear()
            Exit Sub
        End If

        Int_Counter = 1
        '情報入力
        While (OBJ_DATAREADER.Read = True)
            If Str_Ginko_Code <> Trim(OBJ_DATAREADER.Item("TKIN_NO_V")).PadLeft(4, "0"c) Then
                txtKinCode(Int_Counter).Text = Trim(OBJ_DATAREADER.Item("TKIN_NO_V")).PadLeft(4, "0"c)
                txtKinNName(Int_Counter).Text = OBJ_DATAREADER.Item("KIN_NNAME_N")

                Int_Counter += 1
            End If

            Str_Ginko_Code = Trim(OBJ_DATAREADER.Item("TKIN_NO_V")).PadLeft(4, "0"c)
        End While

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Sub
        End If

        Int_Start = Int_Counter

        For Int_Counter = Int_Start To 20
            txtKinCode(Int_Counter).Text = ""
            txtKinNName(Int_Counter).Text = ""

            btnTakouKin(Int_Counter).Enabled = False
        Next

        'ボタン再制御
        Call PSUB_Set_TakouButton()
    End Sub
    Private Sub PSUB_Set_TakouButton()
        '********************************************
        '他行ボタン入力制御
        '********************************************
        Dim Int_Hantei As Integer
        Dim Int_Counter As Integer

        '入力場所制御
        Int_Hantei = 0
        For Int_Counter = 1 To 20
            If txtKinCode(Int_Counter).Text = "" Then
                Int_Hantei += 1
            End If
            'ボタンはひとつずれる
            If Int_Hantei >= 2 Then
                btnTakouKin(Int_Counter).Enabled = False
            Else
                btnTakouKin(Int_Counter).Enabled = True
            End If
        Next

    End Sub
    Private Sub PSUB_Set_TakouButton_Clear()
        '********************************************
        '他行ボタン入力制御(全クリア)
        '********************************************
        Dim Int_Hantei As Integer
        Dim Int_Counter As Integer

        '入力場所制御
        Int_Hantei = 0
        For Int_Counter = 1 To 20
            txtKinCode(Int_Counter).Text = ""
            txtKinNName(Int_Counter).Text = ""

            'ボタンはひとつずれる
            If Int_Counter >= 2 Then
                btnTakouKin(Int_Counter).Enabled = False
            Else
                btnTakouKin(Int_Counter).Enabled = True
            End If
        Next

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
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME, True) = True Then
            cmbGAKKOUNAME.Focus()
        End If

    End Sub
    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGAKKOUNAME.SelectedIndexChanged
        'COMBOBOX選択時学校名,学校コード設定
        Label4.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)

        '参照ボタンにFOCUS
        btnFind.Focus()
    End Sub
    '2010/10/20 学校コードが変更された場合表示をクリアする
    Private Sub txtTorisCode_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.TextChanged
        PSUB_Set_TakouButton_Clear()
    End Sub
    Private Sub txtGAKKOU_CODE_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        '********************************************
        '学校コード LostFocus
        '********************************************

        If GFUNC_GAKKOU_ISEXIST2(txtGAKKOU_CODE, Label4) = True Then
            '参照ボタンにFOCUS
            btnFind.Focus()
        ElseIf txtGAKKOU_CODE.Text = "9999999999" Then '2006/10/20　追加：一覧印刷
            'ラベルに文字を表示
            Label4.Text = "他行マスタ一覧"
            '印刷ボタンにFOCUS
            btnPrint.Focus()
        Else
            '他行情報クリア
            For Int_Counter As Integer = 1 To 20
                '2006/10/23　参照ボタンを押さなくても入力できるように修正
                'btnTakouKin(Int_Counter).Enabled = False
                txtKinCode(Int_Counter).Text = ""
                txtKinNName(Int_Counter).Text = ""
            Next

            '2006/10/11　追加:メッセージを表示後、フォーカスを当てる
            If Trim(txtGAKKOU_CODE.Text) <> "" Then
                Call MessageBox.Show(G_MSG0061W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Text = ""
                txtGAKKOU_CODE.Focus()
            End If

        End If
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function GFUNC_GAKKOU_ISEXIST2(ByVal pGakkou_Code As TextBox, ByVal pGakkou_Name As Label) As Boolean
        '*****************************************
        '学校コード存在チェック
        '*****************************************
        GFUNC_GAKKOU_ISEXIST2 = False

        If pGakkou_Code.Text = "" Then

            Exit Function
        End If

        '学校名検索
        STR_SQL = ""
        STR_SQL += "SELECT GAKKOU_CODE_G , GAKKOU_NNAME_G FROM GAKMAST1,GAKMAST2 "
        STR_SQL += " WHERE GAKKOU_CODE_G = GAKKOU_CODE_T"
        STR_SQL += " AND GAKKOU_CODE_G = '" & pGakkou_Code.Text.PadLeft(pGakkou_Code.MaxLength, "0"c) & "'"
        STR_SQL += " AND TAKO_KBN_T ='1'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            '該当コード無し
            pGakkou_Name.Text = ""
        Else
            OBJ_DATAREADER.Read()

            pGakkou_Name.Text = OBJ_DATAREADER.Item("GAKKOU_NNAME_G").ToString

            GFUNC_GAKKOU_ISEXIST2 = True
        End If

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

    End Function
#End Region

End Class
