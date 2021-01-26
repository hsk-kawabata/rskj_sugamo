Option Strict On
Option Explicit On

Imports System.Text
Imports System.IO
Imports System.Collections
Imports System.Data
Imports System.Data.OracleClient
Imports CASTCommon

Public Class KFJMAST040

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)


    'ログ初期化
    Private BLOG As New CASTCommon.BatchLOG("KFJMAST040", "他行マスタメンテナンス")
    Private Const ErrMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"
    Private Const msgTitle As String = "他行マスタメンテナンス画面(KFJMAST040)"

    Private MainDB As New CASTCommon.MyOracle

    Private txtKinCode(20) As TextBox
    Private txtKinNNAME(20) As TextBox
    Private btnTakouKin(20) As Button

    'フォームLOAD処理
    Private Sub KFJMAST040_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            Me.btnEnd.DialogResult = Windows.Forms.DialogResult.None

            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(ロード)開始", "成功", "")

            'ユーザＩＤ／システム日付表示
            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            Me.txtTorisCode.Clear()
            Me.txtTorifCode.Clear()
            Call SetControlIndex()

            '--------------------------------
            '委託者名リストボックスの設定
            '--------------------------------
            Dim Jyoken As String = " AND TAKO_KBN_T = '1'"   '他行対象
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(ロード)", "失敗", ex.Message)
            Me.Close()

        Finally
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(ロード)終了", "成功", "")
        End Try
    End Sub

    '画面終了時処理
    Private Sub KFJMAST040_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed

        Try

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(クローズ)例外エラー", "失敗", ex.Message)
        End Try

    End Sub

    '参照ボタン
    Private Sub btnSansyou_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSansyou.Click

        'Dim MSG As String
        'Dim DRet As DialogResult
        'Dim REC As OracleDataReader
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As StringBuilder
        Dim intIndex As Integer = 0

        Try
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(参照)開始", "成功", "")

            Dim TORIS_CODE As String = Trim(Me.txtTorisCode.Text)
            Dim TORIF_CODE As String = Trim(Me.txtTorifCode.Text)
            BLOG.ToriCode = TORIS_CODE & TORIF_CODE

            'If txtTorisCode.Text <> "" AndAlso _
            '   txtTorifCode.Text = "" Then
            '    txtTorifCode.Text = TORIF_CODE
            'End If
            If TORIS_CODE.Length = 0 Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorisCode.Focus()
                Return
            End If

            If TORIF_CODE.Length = 0 Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorifCode.Focus()
                Return
            End If

            GCom.GLog.ToriCode = TORIS_CODE & "-" & TORIF_CODE

            SQL = New StringBuilder(128)
            SQL.Append("SELECT ITAKU_CODE_T")
            SQL.Append(", ITAKU_NNAME_T")
            SQL.Append(", BAITAI_CODE_T")
            SQL.Append(", TAKO_KBN_T")
            SQL.Append(" FROM TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_T = '1'")
            SQL.Append(" AND TORIS_CODE_T = '" & TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & TORIF_CODE & "'")

            If OraReader.DataReader(SQL) = True Then

                If OraReader.GetString("TAKO_KBN_T") = "1" Then
                    GCom.GLog.ToriCode = TORIS_CODE & TORIF_CODE
                    '2013/07/17 saitou 標準修正 ADD -------------------------------------------------->>>>
                    'テキストボックスの初期化を追加
                    Call Me.InitializeTextBox()
                    '2013/07/17 saitou 標準修正 ADD --------------------------------------------------<<<<
                    Call SetTAKOU_KIN(TORIS_CODE, TORIF_CODE, intIndex)
                    '他行マスタに登録されていない
                    If intIndex = 0 Then
                        MessageBox.Show(MSG0349W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtTorisCode.Focus()
                    Else
                        btnTakouKin1.Focus()
                        Exit Sub
                    End If
                Else
                    MessageBox.Show(MSG0069W, _
                                    msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtTorisCode.Focus()
                End If
            Else
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorisCode.Focus()
            End If

            OraReader.Close()
            txtTorisCode.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(参照)", "失敗", ex.Message)
        Finally
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(参照)終了", "成功", "")
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        End Try

    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(クローズ)開始", "成功", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "", "失敗", ex.ToString)
        Finally
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(クローズ)終了", "成功", "")
        End Try
    End Sub

    '印刷ボタン
    Private Sub btnPrint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        'Dim MSG As String
        'Dim toriSQL As String
        Dim OraReader As CASTCommon.MyOracleReader
        'Dim BRet As Boolean
        'Dim DRet As DialogResult
        Dim SQL As StringBuilder
        Dim strALL_Print As String = "0"    '印刷対象フラグ(0:指定の取引先,1:全取引先)

        Try
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(印刷)開始", "成功", "")
            SQL = New StringBuilder(128)
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim TORIS_CODE As String = GCom.NzDec(txtTorisCode.Text, "")
            Dim TORIF_CODE As String = GCom.NzDec(txtTorifCode.Text, "")
            GCom.GLog.ToriCode = TORIS_CODE & "-" & TORIF_CODE

            If TORIS_CODE.Length = 0 AndAlso TORIF_CODE.Length = 0 Then
                strALL_Print = "1"

                SQL.Append("SELECT COUNT(*) COUNT")
                SQL.Append(" FROM TORIMAST")
                SQL.Append(" WHERE TAKO_KBN_T = '1'")

                If OraReader.DataReader(SQL) = True Then
                    If GCom.NzLong(OraReader.GetItem("COUNT")) >= 1 Then
                    Else
                        MessageBox.Show(MSG0326W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                        OraReader.Close()
                        Return
                    End If
                Else
                    MessageBox.Show(MSG0326W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    OraReader.Close()
                    Return
                End If
                OraReader.Close()
            Else

                SQL.Append("SELECT COUNT(*) COUNT,MAX(TAKO_KBN_T) TAKO_KBN")
                SQL.Append(" FROM TORIMAST")
                SQL.Append(" WHERE TORIS_CODE_T = '" & TORIS_CODE & "'")
                SQL.Append(" AND TORIF_CODE_T = '" & TORIF_CODE & "'")

                If OraReader.DataReader(SQL) = True Then
                    If GCom.NzLong(OraReader.GetItem("COUNT")) = 0 Then
                        MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtTorisCode.Focus()
                        OraReader.Close()
                        Return
                    ElseIf GCom.NzLong(OraReader.GetItem("TAKO_KBN")) = 0 Then
                        MessageBox.Show(MSG0069W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Me.txtTorisCode.Focus()
                        OraReader.Close()
                        Return
                    End If
                Else
                    MessageBox.Show(Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    OraReader.Close()
                    Return
                End If
                OraReader.Close()
            End If

            SQL = New StringBuilder(128)
            SQL.Append("SELECT COUNT(*) COUNT")
            SQL.Append(" FROM TAKOUMAST,TORIMAST")
            SQL.Append(" WHERE TAKO_KBN_T = '1'")
            If strALL_Print <> "1" Then
                SQL.Append(" AND TORIS_CODE_T = '" & TORIS_CODE & "'")
                SQL.Append(" AND TORIF_CODE_T = '" & TORIF_CODE & "'")
            End If
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_V")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_V")

            If OraReader.DataReader(SQL) = True Then
                If GCom.NzLong(OraReader.GetItem("COUNT")) >= 1 Then
                Else
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtTorisCode.Focus()
                    OraReader.Close()
                    Return
                End If
            Else
                MessageBox.Show(MSG0069W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtTorisCode.Focus()
                OraReader.Close()
                Return
            End If
            OraReader.Close()
            If MessageBox.Show(String.Format(MSG0013I, "他行マスタ一覧表"), _
                               msgTitle, _
                               MessageBoxButtons.YesNo, _
                               MessageBoxIcon.Question) = DialogResult.No Then
                Exit Sub
            End If

            'レポエージェント印刷
            Dim ExeRepo As New CAstReports.ClsExecute

            ExeRepo.SetOwner = Me
            Dim Param As String = ""
            '取引先コード、印刷対象フラグを引数にする
            Param = TORIS_CODE & "," & _
                    TORIF_CODE & "," & _
                    strALL_Print
            '他行マスタ一覧表印刷
            Dim nRet As Integer = ExeRepo.ExecReport("KFJP025.exe", Param)
            If nRet <> 0 Then
                MessageBox.Show(String.Format(MSG0004E, "他行マスタ一覧表"), _
                                msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            MessageBox.Show(String.Format(MSG0014I, "他行マスタ一覧表"), _
                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "印刷", "失敗", ex.Message)
            Return
        Finally
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(印刷)終了", "成功", "")
        End Try
    End Sub

    '他行マスタ詳細画面呼出ボタンClick
    Private Sub btnTakouKin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                                                    Handles btnTakouKin1.Click, _
                                                                                            btnTakouKin2.Click, _
                                                                                            btnTakouKin3.Click, _
                                                                                            btnTakouKin4.Click, _
                                                                                            btnTakouKin5.Click, _
                                                                                            btnTakouKin6.Click, _
                                                                                            btnTakouKin7.Click, _
                                                                                            btnTakouKin8.Click, _
                                                                                            btnTakouKin9.Click, _
                                                                                            btnTakouKin10.Click, _
                                                                                            btnTakouKin11.Click, _
                                                                                            btnTakouKin12.Click, _
                                                                                            btnTakouKin13.Click, _
                                                                                            btnTakouKin14.Click, _
                                                                                            btnTakouKin15.Click, _
                                                                                            btnTakouKin16.Click, _
                                                                                            btnTakouKin17.Click, _
                                                                                            btnTakouKin18.Click, _
                                                                                            btnTakouKin19.Click, _
                                                                                            btnTakouKin20.Click
        '   Dim MSG As String
        '   Dim DRet As DialogResult
        Dim OraReader As CASTCommon.MyOracleReader
        Dim SQL As StringBuilder
        Try

            Dim KFJMAST041 As New KFJMAST041
            Dim tagIndex As Integer
            SQL = New StringBuilder(128)
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            Dim TORIS_CODE As String = GCom.NzDec(Me.txtTorisCode.Text, "")
            Dim TORIF_CODE As String = GCom.NzDec(Me.txtTorifCode.Text, "")
            BLOG.ToriCode = TORIS_CODE & TORIF_CODE

            If txtTorisCode.Text <> "" AndAlso _
               txtTorifCode.Text = "" Then
                txtTorifCode.Text = TORIF_CODE
            End If
            If Trim(TORIS_CODE).Length <> 0 Then
                GCom.GLog.ToriCode = TORIS_CODE & "-" & TORIF_CODE
            Else
                '"取引先主コードが入力されていません。
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Me.txtTorisCode.Focus()
                Return
            End If


            SQL.Append("SELECT COUNT(*) COUNT,MAX(TAKO_KBN_T) TAKO_KBN ,MAX(BAITAI_CODE_T) BAITAI_CODE_T")
            Sql.Append(" FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = '" & TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & TORIF_CODE & "'")

            If OraReader.DataReader(Sql) = True Then
                If GCom.NzLong(OraReader.GetItem("COUNT")) = 0 Then
                    MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtTorisCode.Focus()
                    OraReader.Close()
                    Return
                ElseIf GCom.NzLong(OraReader.GetItem("TAKO_KBN")) = 0 Then
                    MessageBox.Show(MSG0069W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtTorisCode.Focus()
                    OraReader.Close()
                    Return
                End If
                KFJMAST041.Baitai_Code = GCom.NzStr(OraReader.GetItem("BAITAI_CODE_T"))
            Else
                MessageBox.Show(Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                OraReader.Close()
                Return
            End If
            OraReader.Close()




            If sender Is btnTakouKin1 Then
                tagIndex = CInt(btnTakouKin1.Tag)
            ElseIf sender Is btnTakouKin2 Then
                tagIndex = CInt(btnTakouKin2.Tag)
            ElseIf sender Is btnTakouKin3 Then
                tagIndex = CInt(btnTakouKin3.Tag)
            ElseIf sender Is btnTakouKin4 Then
                tagIndex = CInt(btnTakouKin4.Tag)
            ElseIf sender Is btnTakouKin5 Then
                tagIndex = CInt(btnTakouKin5.Tag)
            ElseIf sender Is btnTakouKin6 Then
                tagIndex = CInt(btnTakouKin6.Tag)
            ElseIf sender Is btnTakouKin7 Then
                tagIndex = CInt(btnTakouKin7.Tag)
            ElseIf sender Is btnTakouKin8 Then
                tagIndex = CInt(btnTakouKin8.Tag)
            ElseIf sender Is btnTakouKin9 Then
                tagIndex = CInt(btnTakouKin9.Tag)
            ElseIf sender Is btnTakouKin10 Then
                tagIndex = CInt(btnTakouKin10.Tag)
            ElseIf sender Is btnTakouKin11 Then
                tagIndex = CInt(btnTakouKin11.Tag)
            ElseIf sender Is btnTakouKin12 Then
                tagIndex = CInt(btnTakouKin12.Tag)
            ElseIf sender Is btnTakouKin13 Then
                tagIndex = CInt(btnTakouKin13.Tag)
            ElseIf sender Is btnTakouKin14 Then
                tagIndex = CInt(btnTakouKin14.Tag)
            ElseIf sender Is btnTakouKin15 Then
                tagIndex = CInt(btnTakouKin15.Tag)
            ElseIf sender Is btnTakouKin16 Then
                tagIndex = CInt(btnTakouKin16.Tag)
            ElseIf sender Is btnTakouKin17 Then
                tagIndex = CInt(btnTakouKin17.Tag)
            ElseIf sender Is btnTakouKin18 Then
                tagIndex = CInt(btnTakouKin18.Tag)
            ElseIf sender Is btnTakouKin19 Then
                tagIndex = CInt(btnTakouKin19.Tag)
            ElseIf sender Is btnTakouKin20 Then
                tagIndex = CInt(btnTakouKin20.Tag)
            End If

            With KFJMAST041
                .MotherForm = Me
                .CallIndex = tagIndex
                .ShowDialog()
            End With
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(他行マスタ詳細画面呼出ボタンClick)", "失敗", ex.Message)

        End Try
    End Sub

    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        Try
            Call InitializeTextBox()

            '-------------------------------------------
            '取引先コードテキストボックスに取引先コード設定
            '-------------------------------------------
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                If Trim(cmbToriName.Text) = "" Then
                    cmbToriName.Focus()
                Else
                    Me.btnSansyou.PerformClick()
                    txtTorisCode.Focus()
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(取引先コード設定)", "失敗", ex.Message)

        Finally

        End Try
    End Sub

    'カナコンボボックス設定変更時再読み込み
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            '取引先コードがReadOnlyの場合は処理終了
            If txtTorisCode.ReadOnly Then
                Exit Sub
            End If
            '取引先コンボボックス設定
            Dim Jyoken As String = " AND TAKO_KBN_T = '1'"   '他行対象
            If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "カナコンボボックス設定変更", "失敗", ex.Message)
        End Try
    End Sub

    'テキストボックスのクリア
    Public Sub InitializeTextBox()

        Try
            Dim intIndex As Integer

            For intindex = 1 To 20
                txtKinCode(intIndex).Text = ""
                txtKinNNAME(intIndex).Text = ""
            Next

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "テキストボックス表示クリア", "失敗", ex.Message)
        End Try

    End Sub

    'ボタン、テキストボックスの関連付け
    Private Sub SetControlIndex()

        'Dim intIndex As Integer

        btnTakouKin(1) = btnTakouKin1
        btnTakouKin(2) = btnTakouKin2
        btnTakouKin(3) = btnTakouKin3
        btnTakouKin(4) = btnTakouKin4
        btnTakouKin(5) = btnTakouKin5
        btnTakouKin(6) = btnTakouKin6
        btnTakouKin(7) = btnTakouKin7
        btnTakouKin(8) = btnTakouKin8
        btnTakouKin(9) = btnTakouKin9
        btnTakouKin(10) = btnTakouKin10
        btnTakouKin(11) = btnTakouKin11
        btnTakouKin(12) = btnTakouKin12
        btnTakouKin(13) = btnTakouKin13
        btnTakouKin(14) = btnTakouKin14
        btnTakouKin(15) = btnTakouKin15
        btnTakouKin(16) = btnTakouKin16
        btnTakouKin(17) = btnTakouKin17
        btnTakouKin(18) = btnTakouKin18
        btnTakouKin(19) = btnTakouKin19
        btnTakouKin(20) = btnTakouKin20

        txtKinCode(1) = txtKinCode1
        txtKinCode(2) = txtKinCode2
        txtKinCode(3) = txtKinCode3
        txtKinCode(4) = txtKinCode4
        txtKinCode(5) = txtKinCode5
        txtKinCode(6) = txtKinCode6
        txtKinCode(7) = txtKinCode7
        txtKinCode(8) = txtKinCode8
        txtKinCode(9) = txtKinCode9
        txtKinCode(10) = txtKinCode10
        txtKinCode(11) = txtKinCode11
        txtKinCode(12) = txtKinCode12
        txtKinCode(13) = txtKinCode13
        txtKinCode(14) = txtKinCode14
        txtKinCode(15) = txtKinCode15
        txtKinCode(16) = txtKinCode16
        txtKinCode(17) = txtKinCode17
        txtKinCode(18) = txtKinCode18
        txtKinCode(19) = txtKinCode19
        txtKinCode(20) = txtKinCode20

        txtKinNNAME(1) = txtKinNName1
        txtKinNNAME(2) = txtKinNName2
        txtKinNNAME(3) = txtKinNName3
        txtKinNNAME(4) = txtKinNName4
        txtKinNNAME(5) = txtKinNName5
        txtKinNNAME(6) = txtKinNName6
        txtKinNNAME(7) = txtKinNName7
        txtKinNNAME(8) = txtKinNName8
        txtKinNNAME(9) = txtKinNName9
        txtKinNNAME(10) = txtKinNName10
        txtKinNNAME(11) = txtKinNName11
        txtKinNNAME(12) = txtKinNName12
        txtKinNNAME(13) = txtKinNName13
        txtKinNNAME(14) = txtKinNName14
        txtKinNNAME(15) = txtKinNName15
        txtKinNNAME(16) = txtKinNName16
        txtKinNNAME(17) = txtKinNName17
        txtKinNNAME(18) = txtKinNName18
        txtKinNNAME(19) = txtKinNName19
        txtKinNNAME(20) = txtKinNName20

    End Sub

    '他行金融機関コード、金融機関名設定（他行マスタに登録されている金融機関を表示）
    Public Function SetTAKOU_KIN(ByVal TORIS_CODE As String, ByVal TORIF_CODE As String, ByRef intIndex As Integer) As Boolean

        Dim OraReader As CASTCommon.MyOracleReader
        Try
            intIndex = 0

            'BLOG.Write(BLOG.ToriCode, "00000000", "金融機関コード、金融機関名設定(開始)", "成功")

            Dim SQL As StringBuilder
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            SQL = New StringBuilder(128)
            SQL.Append("SELECT KIN_NNAME_N")
            SQL.Append(", KIN_NO_N")
            SQL.Append(", MIN(SIT_NO_N) SIT_NO_N")
            SQL.Append(" FROM TENMAST")
            SQL.Append(", TAKOUMAST")
            SQL.Append(" WHERE TORIS_CODE_V = '" & TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_V = '" & TORIF_CODE & "'")
            SQL.Append(" AND TKIN_NO_V = KIN_NO_N")
            SQL.Append(" AND TSIT_NO_V = SIT_NO_N")
            SQL.Append(" GROUP BY KIN_NO_N")
            SQL.Append(", SIT_NO_N")
            SQL.Append(", KIN_NNAME_N")
            SQL.Append(" ORDER BY KIN_NO_N")
            SQL.Append(", SIT_NO_N")

            If OraReader.DataReader(SQL) = True Then
                Do While OraReader.EOF = False

                    intIndex += 1

                    txtKinCode(intIndex).Text = OraReader.GetString("KIN_NO_N")
                    txtKinNNAME(intIndex).Text = OraReader.GetString("KIN_NNAME_N")

                    OraReader.NextRead()
                    If intIndex >= 20 Then
                        Exit Do
                    End If
                Loop
            End If

            OraReader.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, "00000000", "金融機関コード、金融機関名設定", "失敗", ex.Message)
        End Try
    End Function

    '取引先主／副コード入力時ゼロ埋め
    Private Sub txtTorisCode_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtTorisCode.Validating, _
            txtTorifCode.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, "00000000", "取引先主/副コード入力ゼロ埋め(終了)", "失敗", ex.Message)
        End Try
    End Sub
End Class