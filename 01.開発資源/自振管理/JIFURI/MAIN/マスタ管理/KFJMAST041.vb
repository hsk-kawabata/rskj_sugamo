Option Strict On
Option Explicit On

Imports System.Text
Imports System.IO
Imports System.Data.OracleClient
Imports CASTCommon

Public Class KFJMAST041

    Public MotherForm As KFJMAST040
    Public CallIndex As Integer

    '共通イベントコントロール
    Private CAST As New CASTCommon.Events

    '' 非許可文字設定
    Private CASTx1 As New CASTCommon.Events(" ", CASTCommon.Events.KeyMode.BAD)

    '' 許可文字指定
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private BLOG As New CASTCommon.BatchLOG("KFJMAST041", "他行マスタ詳細")
    Private Const msgTitle As String = "他行マスタメンテナンス詳細画面(KFJMAIN041)"
    Public Baitai_Code As String
    Private MainDB As New CASTCommon.MyOracle

    Private Structure TAKOMAST_INF
        Dim TORIS_CODE As String        '10
        Dim TORIF_CODE As String        '2
        Dim TKIN_NO As String           '4
        Dim TSIT_NO As String           '3
        Dim ITAKU_CODE As String        '10
        Dim KAMOKU As String            '2
        Dim KOUZA As String             '7
        Dim BAITAI_CODE As String       '2
        Dim CODE_KBN As String          '1
        Dim SFILE_NAME As String        '26
        Dim RFILE_NAME As String        '26

        Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            TKIN_NO = ""
            TSIT_NO = ""
            ITAKU_CODE = ""
            KAMOKU = ""
            KOUZA = ""
            BAITAI_CODE = ""
            CODE_KBN = ""
            SFILE_NAME = ""
            RFILE_NAME = ""
        End Sub
    End Structure
    Private memData As TAKOMAST_INF

    Private Const strBAITAI_CODE_T_TXT As String = "KFJMAST041_媒体コード.TXT"
    Private Const strKAMOKU_T_TXT As String = "KFJMAST041_科目.TXT"
    Private Const strCODE_KBN_T_TXT As String = "KFJMAST041_コード区分.TXT"

    Private Const ThisModuleName As String = "KFJMAST041.vb"

    Private Sub KFJMAST041_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim OraReader As CASTCommon.MyOracleReader

        Try
            BLOG.UserID = GCom.GetUserID
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ詳細画面LOAD(開始)", "成功")

            '画面の初期化
            Call Form_Initialize()

            Dim SQL As StringBuilder
            SQL = New StringBuilder(128)
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim TORIS_CODE As String
            Dim TORIF_CODE As String
            Dim TKIN_NO As String

            TORIS_CODE = GCom.NzDec(MotherForm.txtTorisCode.Text, "").PadLeft(10, "0"c)
            TORIF_CODE = GCom.NzDec(MotherForm.txtTorifCode.Text, "").PadLeft(2, "0"c)
            Select Case CallIndex
                Case 1
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode1.Text)
                Case 2
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode2.Text)
                Case 3
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode3.Text)
                Case 4
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode4.Text)
                Case 5
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode5.Text)
                Case 6
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode6.Text)
                Case 7
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode7.Text)
                Case 8
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode8.Text)
                Case 9
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode9.Text)
                Case 10
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode10.Text)
                Case 11
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode11.Text)
                Case 12
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode12.Text)
                Case 13
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode13.Text)
                Case 14
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode14.Text)
                Case 15
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode15.Text)
                Case 16
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode16.Text)
                Case 17
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode17.Text)
                Case 18
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode18.Text)
                Case 19
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode19.Text)
                Case 20
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode20.Text)
                Case Else
                    TKIN_NO = ""
            End Select

            SQL.Append("SELECT TORIS_CODE_V")
            SQL.Append(", TORIF_CODE_V")
            SQL.Append(", TKIN_NO_V")
            SQL.Append(", KIN_NNAME_N")
            SQL.Append(", TSIT_NO_V")
            SQL.Append(", SIT_NNAME_N")
            SQL.Append(", ITAKU_CODE_V")
            SQL.Append(", KAMOKU_V")
            SQL.Append(", KOUZA_V")
            SQL.Append(", BAITAI_CODE_V")
            SQL.Append(", CODE_KBN_V")
            SQL.Append(", SFILE_NAME_V")
            SQL.Append(", RFILE_NAME_V")
            SQL.Append(" FROM TAKOUMAST")
            SQL.Append(", (SELECT MIN(KIN_NO_N) KIN_NO_N")
            SQL.Append(", MIN(SIT_NO_N) SIT_NO_N")
            SQL.Append(", KIN_NNAME_N")
            SQL.Append(", SIT_NNAME_N")
            SQL.Append(" FROM TENMAST")
            SQL.Append(" GROUP BY KIN_NO_N")
            SQL.Append(", SIT_NO_N")
            SQL.Append(", KIN_NNAME_N")
            SQL.Append(", SIT_NNAME_N) TENMAST")
            SQL.Append(" WHERE TKIN_NO_V = KIN_NO_N (+)")
            SQL.Append(" AND TSIT_NO_V = SIT_NO_N (+)")
            SQL.Append(" AND TORIS_CODE_V = '" & TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_V = '" & TORIF_CODE & "'")
            SQL.Append(" AND TKIN_NO_V = '" & TKIN_NO & "'")

            If OraReader.DataReader(SQL) = True Then

                '取引先コード
                memData.TORIS_CODE = OraReader.GetString("TORIS_CODE_V")
                memData.TORIF_CODE = OraReader.GetString("TORIF_CODE_V")

                '金融機関コード
                memData.TKIN_NO = OraReader.GetString("TKIN_NO_V")
                txtKinCode.Text = memData.TKIN_NO
                lblKIN_NM.Text = OraReader.GetString("KIN_NNAME_N")

                '支店コード
                memData.TSIT_NO = OraReader.GetString("TSIT_NO_V")
                txtSitCode.Text = memData.TSIT_NO
                lblSIT_NM.Text = OraReader.GetString("SIT_NNAME_N")

                '口座番号
                memData.KOUZA = OraReader.GetString("KOUZA_V")
                txtKouza.Text = memData.KOUZA

                '委託者コード
                memData.ITAKU_CODE = OraReader.GetString("ITAKU_CODE_V")
                txtItakuCode.Text = memData.ITAKU_CODE

                '送信ファイル名
                memData.SFILE_NAME = OraReader.GetString("SFILE_NAME_V")
                txtSousinFile.Text = memData.SFILE_NAME

                '受信ファイル名
                memData.RFILE_NAME = OraReader.GetString("RFILE_NAME_V")
                txtJyusinFile.Text = memData.RFILE_NAME

                '科目
                memData.KAMOKU = OraReader.GetString("KAMOKU_V")

                '媒体コード
                memData.BAITAI_CODE = OraReader.GetString("BAITAI_CODE_V")

                'コード区分
                memData.CODE_KBN = OraReader.GetString("CODE_KBN_V")
            Else

            End If
            OraReader.Close()

            For Each ctlControl As Control In Me.Controls
                Dim Cnt As Integer
                Dim Value As Integer

                If TypeOf ctlControl Is ComboBox Then
                    Dim cmbComboBox As ComboBox = CType(ctlControl, ComboBox)

                    Select Case cmbComboBox.Name
                        Case "cmbKamoku"
                            Call GCom.SetComboBox(cmbKamoku, strKAMOKU_T_TXT, False)
                            Dim ArgKAMOKU() As Integer = New Integer(4) {1, 2, 5, 37, 9}
                            Call GCom.RemoveComboItem(cmbKamoku, ArgKAMOKU)
                            Value = CInt(memData.KAMOKU)

                        Case "cmbBaitai"
                            Call GCom.SetComboBox(cmbBaitai, strBAITAI_CODE_T_TXT, False)
                            Dim ArgBAITAI() As Integer = New Integer(6) {0, 1, 4, 5, 6, 7, 9}
                            Call GCom.RemoveComboItem(cmbBaitai, ArgBAITAI)
                            Value = CInt(memData.BAITAI_CODE)

                        Case "cmbCodeKbn"
                            Call GCom.SetComboBox(cmbCodeKbn, strCODE_KBN_T_TXT, False)
                            Dim ArgCODE_KBN() As Integer = New Integer(4) {0, 1, 2, 3, 4}
                            Call GCom.RemoveComboItem(cmbCodeKbn, ArgCODE_KBN)
                            Value = CInt(memData.CODE_KBN)

                    End Select
                    With cmbComboBox
                        If TKIN_NO = "" Then
                            .SelectedIndex = -1
                        Else
                            For Cnt = 0 To .Items.Count - 1 Step 1
                                .SelectedIndex = Cnt
                                If GCom.GetComboBox(cmbComboBox) = Value Then
                                    Exit For
                                End If
                            Next Cnt
                            If Cnt >= .Items.Count AndAlso .Items.Count > 0 Then
                                .SelectedIndex = -1
                            End If
                        End If
                    End With
                End If
            Next

            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ詳細画面LOAD(終了)", "成功")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ詳細画面LOAD", "失敗", ex.Message)

        End Try
    End Sub

    '登録ボタン
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim MSG As String
        Dim DRet As DialogResult
        Dim Data As New TAKOMAST_INF

        Try
            Dim ctlControl As New Control
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(登録)開始", "成功")
            '入力チェック
            If Not CheckEntryStatus(ctlControl, Data) Then
                ctlControl.Focus()
                Return
            ElseIf GCom.NzDec(Baitai_Code, "") = "07" Then
                DRet = MessageBox.Show(MSG0150W, _
                                       msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Me.btnEnd.Focus()
                Return
            Else
                DRet = MessageBox.Show(MSG0003I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If Not DRet = DialogResult.OK Then
                    Me.txtKinCode.Focus()
                    Return
                End If

            End If

            '他行マスタに同じ取引先コード／金融機関が存在しないことを確認
            If FN_SELECT_TAKOUMAST(Data) > 0 Then
                DRet = MessageBox.Show(MSG0122W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKinCode.Focus()
                Return
            ElseIf Data.TKIN_NO = CASTCommon.GetFSKJIni("COMMON", "KINKOCD") Then
                DRet = MessageBox.Show(MSG0323W, _
                                       msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKinCode.Focus()
                Return
            End If

            With GCom.GLog
                MSG = "委託者コード=" & Data.ITAKU_CODE
                MSG &= ", " & "金融機関=" & Data.TKIN_NO
                .Discription = MSG
            End With
            If FN_INSERT_TAKOUMAST(Data) Then

                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ登録", "成功")
                DRet = MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

                Me.btnEnd.PerformClick()
                'Call MotherForm.SetSpreadArea(Data.TORIS_CODE, Data.TORIF_CODE)

                Call MotherForm.InitializeTextBox()
                Call MotherForm.SetTAKOU_KIN(Data.TORIS_CODE, Data.TORIF_CODE, 0)

                Return
            Else
                With GCom.GLog
                    .Result = MenteCommon.clsCommon.NG
                End With
                'Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ登録", "失敗")
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "登録", "失敗", ex.Message)
        Finally
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(登録)終了", "成功")
        End Try
    End Sub

    '更新ボタン
    Private Sub btnKousin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKousin.Click

        Dim MSG As String
        Dim DRet As DialogResult
        Dim Data As New TAKOMAST_INF
        Dim intRet As Integer

        Try
            Dim ctlControl As New Control
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(更新)開始", "成功")
            '入力チェック
            If Not CheckEntryStatus(ctlControl, Data) Then
                ctlControl.Focus()
                Return
            ElseIf GCom.NzDec(Baitai_Code, "") = "07" Then
                DRet = MessageBox.Show(MSG0150W, _
                                       msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.btnEnd.Focus()
                Return
            Else
                If CheckCurrentAndNew(Data) = 0 Then
                    DRet = MessageBox.Show(MSG0324W, _
                                            msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Me.txtKinCode.Focus()
                    Return
                Else
                    DRet = MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                    If Not DRet = DialogResult.OK Then
                        ' Me.txtKinCode.Focus()
                        Return
                    End If

                End If
            End If

            '他行マスタに同じ取引先コード／金融機関が存在することを確認
            intRet = FN_SELECT_TAKOUMAST(Data)
            Select Case intRet
                Case 0
                    DRet = MessageBox.Show(MSG0070W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case -1
                    Exit Sub
            End Select

            With GCom.GLog
                MSG = "委託者コード=" & Data.ITAKU_CODE
                MSG &= ", " & "金融機関=" & Data.TKIN_NO
                .Discription = MSG
            End With
            If FN_UPDATE_TAKOUMAST(Data) = True Then

                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ更新", "成功")
                DRet = MessageBox.Show(MSG0006I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.btnEnd.PerformClick()
                'Call MotherForm.SetSpreadArea(Data.TORIS_CODE, Data.TORIF_CODE)

                Call MotherForm.InitializeTextBox()
                Call MotherForm.SetTAKOU_KIN(Data.TORIS_CODE, Data.TORIF_CODE, 0)

                Return
            Else
                DRet = MessageBox.Show(String.Format(MSG0027E, "他行マスタ", "更新"), _
                                       msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ更新", "失敗")
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "更新", "失敗", ex.Message)
        Finally
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(更新)終了", "成功")
        End Try

    End Sub

    '削除ボタン
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        'Dim MSG As String
        Dim DRet As DialogResult
        Dim Data As New TAKOMAST_INF

        Try
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(削除)開始", "成功")

            If GCom.NzDec(Baitai_Code, "") = "07" Then
                DRet = MessageBox.Show(MSG0150W, _
                                       msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.btnEnd.Focus()
                Return
            End If

            If txtKinCode.Text = "" Then

                MessageBox.Show(String.Format(MSG0281W, "金融機関コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKinCode.Focus()
                Return
            End If

            If txtSitCode.Text = "" Then

                MessageBox.Show(String.Format(MSG0281W, "支店コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtSitCode.Focus()
                Return
            End If

            Data.TORIS_CODE = GCom.NzDec(MotherForm.txtTorisCode.Text, "").PadLeft(10, "0"c)
            Data.TORIF_CODE = GCom.NzDec(MotherForm.txtTorifCode.Text, "").PadLeft(2, "0"c)

            Data.TKIN_NO = Me.txtKinCode.Text
            Data.TSIT_NO = Me.txtSitCode.Text

            '   If FN_SELECT_TENMAST(Data) = False Then
            If Fn_Select_TENMAST_2(Data) = False Then
                MessageBox.Show(String.Format(MSG0248W, Data.TKIN_NO, Data.TSIT_NO), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Me.txtKinCode.Focus()
                Return
            End If

            DRet = MessageBox.Show(MSG0007I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question)

            If DRet = DialogResult.OK Then

                If FN_DELETE_TAKOUMAST(Data) Then

                    BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ削除", "成功")

                    DRet = MessageBox.Show(MSG0008I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.btnEnd.PerformClick()
                    'Call MotherForm.SetSpreadArea(Data.TORIS_CODE, Data.TORIF_CODE)

                    Call MotherForm.InitializeTextBox()
                    Call MotherForm.SetTAKOU_KIN(Data.TORIS_CODE, Data.TORIF_CODE, 0)

                    Return
                Else
                    With GCom.GLog
                        .Result = MenteCommon.clsCommon.NG
                    End With
                    BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ削除", "失敗")
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "削除", "失敗", ex.Message)
        Finally
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(削除)終了", "成功")
        End Try
    End Sub

    '取消ボタン
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        Dim OraReader As CASTCommon.MyOracleReader

        Try
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "取消(開始)", "成功")

            '画面の初期化
            Call Form_Initialize()

            Dim SQL As StringBuilder
            SQL = New StringBuilder(128)
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            Dim TORIS_CODE As String
            Dim TORIF_CODE As String
            Dim TKIN_NO As String

            TORIS_CODE = GCom.NzDec(MotherForm.txtTorisCode.Text, "").PadLeft(10, "0"c)
            TORIF_CODE = GCom.NzDec(MotherForm.txtTorifCode.Text, "").PadLeft(2, "0"c)
            Select Case CallIndex
                Case 1
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode1.Text)
                Case 2
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode2.Text)
                Case 3
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode3.Text)
                Case 4
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode4.Text)
                Case 5
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode5.Text)
                Case 6
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode6.Text)
                Case 7
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode7.Text)
                Case 8
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode8.Text)
                Case 9
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode9.Text)
                Case 10
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode10.Text)
                Case 11
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode11.Text)
                Case 12
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode12.Text)
                Case 13
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode13.Text)
                Case 14
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode14.Text)
                Case 15
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode15.Text)
                Case 16
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode16.Text)
                Case 17
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode17.Text)
                Case 18
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode18.Text)
                Case 19
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode19.Text)
                Case 20
                    TKIN_NO = GCom.NzStr(MotherForm.txtKinCode20.Text)
                Case Else
                    TKIN_NO = ""
            End Select

            SQL.Append("SELECT TORIS_CODE_V")
            SQL.Append(", TORIF_CODE_V")
            SQL.Append(", TKIN_NO_V")
            SQL.Append(", KIN_NNAME_N")
            SQL.Append(", TSIT_NO_V")
            SQL.Append(", SIT_NNAME_N")
            SQL.Append(", ITAKU_CODE_V")
            SQL.Append(", KAMOKU_V")
            SQL.Append(", KOUZA_V")
            SQL.Append(", BAITAI_CODE_V")
            SQL.Append(", CODE_KBN_V")
            SQL.Append(", SFILE_NAME_V")
            SQL.Append(", RFILE_NAME_V")
            SQL.Append(" FROM TAKOUMAST")
            SQL.Append(", (SELECT MIN(KIN_NO_N) KIN_NO_N")
            SQL.Append(", MIN(SIT_NO_N) SIT_NO_N")
            SQL.Append(", KIN_NNAME_N")
            SQL.Append(", SIT_NNAME_N")
            SQL.Append(" FROM TENMAST")
            SQL.Append(" GROUP BY KIN_NO_N")
            SQL.Append(", SIT_NO_N")
            SQL.Append(", KIN_NNAME_N")
            SQL.Append(", SIT_NNAME_N) TENMAST")
            SQL.Append(" WHERE TKIN_NO_V = KIN_NO_N (+)")
            SQL.Append(" AND TSIT_NO_V = SIT_NO_N (+)")
            SQL.Append(" AND TORIS_CODE_V = '" & TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_V = '" & TORIF_CODE & "'")
            SQL.Append(" AND TKIN_NO_V = '" & TKIN_NO & "'")

            If OraReader.DataReader(SQL) = True Then

                '取引先コード
                memData.TORIS_CODE = OraReader.GetString("TORIS_CODE_V")
                memData.TORIF_CODE = OraReader.GetString("TORIF_CODE_V")

                '金融機関コード
                memData.TKIN_NO = OraReader.GetString("TKIN_NO_V")
                txtKinCode.Text = memData.TKIN_NO
                lblKIN_NM.Text = OraReader.GetString("KIN_NNAME_N")

                '支店コード
                memData.TSIT_NO = OraReader.GetString("TSIT_NO_V")
                txtSitCode.Text = memData.TSIT_NO
                lblSIT_NM.Text = OraReader.GetString("SIT_NNAME_N")

                '口座番号
                memData.KOUZA = OraReader.GetString("KOUZA_V")
                txtKouza.Text = memData.KOUZA

                '委託者コード
                memData.ITAKU_CODE = OraReader.GetString("ITAKU_CODE_V")
                txtItakuCode.Text = memData.ITAKU_CODE

                '送信ファイル名
                memData.SFILE_NAME = OraReader.GetString("SFILE_NAME_V")
                txtSousinFile.Text = memData.SFILE_NAME

                '受信ファイル名
                memData.RFILE_NAME = OraReader.GetString("RFILE_NAME_V")
                txtJyusinFile.Text = memData.RFILE_NAME

                '科目
                memData.KAMOKU = OraReader.GetString("KAMOKU_V")

                '媒体コード
                memData.BAITAI_CODE = OraReader.GetString("BAITAI_CODE_V")

                'コード区分
                memData.CODE_KBN = OraReader.GetString("CODE_KBN_V")
            Else

            End If
            OraReader.Close()

            For Each ctlControl As Control In Me.Controls
                Dim Cnt As Integer
                Dim Value As Integer

                If TypeOf ctlControl Is ComboBox Then
                    Dim cmbComboBox As ComboBox = CType(ctlControl, ComboBox)

                    Select Case cmbComboBox.Name
                        Case "cmbKamoku"
                            Call GCom.SetComboBox(cmbKamoku, strKAMOKU_T_TXT, False)
                            Dim ArgKAMOKU() As Integer = New Integer(4) {1, 2, 5, 37, 9}
                            Call GCom.RemoveComboItem(cmbKamoku, ArgKAMOKU)
                            Value = CInt(memData.KAMOKU)

                        Case "cmbBaitai"
                            Call GCom.SetComboBox(cmbBaitai, strBAITAI_CODE_T_TXT, False)
                            Dim ArgBAITAI() As Integer = New Integer(6) {0, 1, 4, 5, 6, 7, 9}
                            Call GCom.RemoveComboItem(cmbBaitai, ArgBAITAI)
                            Value = CInt(memData.BAITAI_CODE)

                        Case "cmbCodeKbn"
                            Call GCom.SetComboBox(cmbCodeKbn, strCODE_KBN_T_TXT, False)
                            Dim ArgCODE_KBN() As Integer = New Integer(4) {0, 1, 2, 3, 4}
                            Call GCom.RemoveComboItem(cmbCodeKbn, ArgCODE_KBN)
                            Value = CInt(memData.CODE_KBN)

                    End Select
                    With cmbComboBox
                        If TKIN_NO = "" Then
                            .SelectedIndex = -1
                        Else
                            For Cnt = 0 To .Items.Count - 1 Step 1
                                .SelectedIndex = Cnt
                                If GCom.GetComboBox(cmbComboBox) = Value Then
                                    Exit For
                                End If
                            Next Cnt
                            If Cnt >= .Items.Count AndAlso .Items.Count > 0 Then
                                .SelectedIndex = -1
                            End If
                        End If
                    End With
                End If
            Next

            txtKinCode.Focus()

            'Dim ctlControl As Control
            'Dim cmbComboBox As ComboBox

            'For Each ctlControl In Me.Controls
            '    If ctlControl.Tag Is "1" Then
            '        If TypeOf ctlControl Is TextBox Then
            '            ctlControl.Text = ""
            '        ElseIf TypeOf ctlControl Is Label Then
            '            ctlControl.Text = ""
            '        ElseIf TypeOf ctlControl Is ComboBox Then
            '            cmbComboBox = CType(ctlControl, ComboBox)
            '            'cmbComboBox.SelectedIndex = -1
            '            cmbComboBox.SelectedIndex = 0
            '        End If
            '    End If
            'Next
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "取消", "失敗", ex.Message)
        Finally
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "取消(終了)", "成功")
        End Try
    End Sub

    '終了ボタン
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(クローズ)開始", "成功")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "クローズ", "失敗")
        Finally
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "(クローズ)終了", "成功")
        End Try

    End Sub

    '
    ' 機能　 ： 入力項目チェック
    '
    ' 引数　 ： ARG1 - 設定不備コントロール
    ' 　　　 　 ARG2 - 他行マスタ構造体
    '
    ' 戻り値 ： 正常 = True, 異常 = False
    '
    ' 備考　 ： Dim TORIS_CODE As String        '10
    '           Dim TORIF_CODE As String        '2
    '           Dim TKIN_NO As String           '4
    '           Dim TSIT_NO As String           '3
    '           Dim ITAKU_CODE As String        '10
    '           Dim KAMOKU As String            '2
    '           Dim KOUZA As String             '7
    '           Dim BAITAI_CODE As String       '2
    '           Dim CODE_KBN As String          '1
    '           Dim SFILE_NAME As String        '26
    '           Dim RFILE_NAME As String        '26
    '    
    Private Function CheckEntryStatus(ByRef CTL As Control, ByRef Data As TAKOMAST_INF) As Boolean
        Dim MSG As String = ""
        Try
            Dim Temp As String
            With GCom.GLog
                .Job2 = "入力項目チェック"
            End With

            Data.TORIS_CODE = GCom.NzDec(MotherForm.txtTorisCode.Text, "").PadLeft(10, "0"c)
            Data.TORIF_CODE = GCom.NzDec(MotherForm.txtTorifCode.Text, "").PadLeft(2, "0"c)

            CTL = Me.txtKinCode
            MSG = "金融機関コード"
            If Me.lblKIN_NM.Text = "" Then
                Exit Try
            Else
                Data.TKIN_NO = GCom.NzDec(Me.txtKinCode.Text, "").PadLeft(4, "0"c)
            End If

            CTL = Me.txtSitCode
            MSG = "支店コード"
            If Me.lblSIT_NM.Text = "" Then
                Exit Try
            Else
                Data.TSIT_NO = GCom.NzDec(Me.txtSitCode.Text, "").PadLeft(3, "0"c)
            End If

            CTL = Me.cmbKamoku
            MSG = "科目"
            'If Me.cmbKamoku.Text = "" Then
            '    Exit Try
            'Else
            '    Data.KAMOKU = GCom.NzDec(Me.cmbKamoku.Text, "").PadLeft(2, "0"c)
            'End If
            If Me.cmbKamoku.SelectedIndex = -1 Then
                Exit Try
            Else
                Data.KAMOKU = GCom.GetComboBox(Me.cmbKamoku).ToString.PadLeft(2, "0"c)
            End If

            CTL = Me.txtKouza
            MSG = "口座番号"
            Temp = GCom.NzDec(Me.txtKouza.Text, "")
            If Temp.Length = 0 Then
                Exit Try
            Else
                Data.KOUZA = Temp.PadLeft(7, "0"c)
            End If

            CTL = Me.txtItakuCode
            MSG = "委託者コード"
            Temp = GCom.NzDec(Me.txtItakuCode.Text, "")
            If Temp.Length = 0 Then
                Exit Try
            Else
                Data.ITAKU_CODE = Temp.PadLeft(10, "0"c)
            End If

            CTL = Me.cmbBaitai
            MSG = "媒体コード"
            If Me.cmbBaitai.SelectedIndex = -1 Then
                Exit Try
            Else
                Data.BAITAI_CODE = GCom.GetComboBox(Me.cmbBaitai).ToString.PadLeft(2, "0"c)
            End If

            CTL = Me.cmbCodeKbn
            MSG = "コード区分"
            If Me.cmbCodeKbn.SelectedIndex = -1 Then
                Exit Try
            Else
                Data.CODE_KBN = GCom.GetComboBox(Me.cmbCodeKbn).ToString
            End If

            CTL = Me.txtSousinFile
            MSG = "請求ファイル名"
            Data.SFILE_NAME = GCom.NzStr(Me.txtSousinFile.Text).Trim

            '208.03.11 FJH指示
            CTL = Me.txtJyusinFile
            MSG = "結果ファイル名"
            Data.RFILE_NAME = GCom.NzStr(Me.txtJyusinFile.Text).Trim

            Return True
        Catch ex As Exception
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "入力項目チェック", "失敗", ex.Message)
        End Try

        MessageBox.Show(String.Format(MSG0281W, MSG), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

        Return False
    End Function
    '
    ' 機能　 ： 画面初期化
    '
    ' 引数　 ： ARG1 - 初期処置有無
    '
    ' 戻り値 ： 正常処理 = True, 処理異常 = False
    '
    ' 備考　 ： なし
    '
    Private Function Form_Initialize() As Boolean

        Try
            txtKinCode.Text = ""        '金融機関コード
            txtSitCode.Text = ""        '支店コード
            lblKIN_NM.Text = ""         '金融機関名
            lblSIT_NM.Text = ""         '金融機関支店名
            txtKouza.Text = ""          '口座番号
            txtItakuCode.Text = ""      '委託者コード
            txtSousinFile.Text = ""     '送信ファイル名
            txtJyusinFile.Text = ""     '受信ファイル名

        Catch ex As Exception
            '例外が発生しました。ログを確認のうえ、保守要員に連絡してください。(ID:MSG0006E)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(gstrUSER_ID, BLOG.ToriCode, "00000000", "(画面初期化)", "成功", ex.Message)
        End Try
    End Function

    '
    ' 機能　 ： 他行マスタ・チェック
    '
    ' 引数　 ： ARG1 - 他行マスタ構造体
    '
    ' 戻り値 ： 同一レコードの数
    '
    ' 備考　 ： 同一レコードの有無
    '
    Private Function FN_SELECT_TAKOUMAST(ByVal Data As TAKOMAST_INF) As Integer

        '    Dim REC As OracleDataReader
        '    Dim Ret As Integer = 0
        '    Try
        '        Dim SQL As String = "SELECT * FROM TAKOUMAST"
        '        SQL &= " WHERE TKIN_NO_V = '" & Data.TKIN_NO & "'"
        '        SQL &= " AND TORIS_CODE_V = '" & Data.TORIS_CODE & "'"
        '        SQL &= " AND TORIF_CODE_V = '" & Data.TORIF_CODE & "'"
        '
        '        Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
        '        If BRet Then
        '            Do While REC.Read
        '                Ret += 1
        '            Loop
        '        End If
        '    Catch ex As Exception
        '        '例外が発生しました。ログを確認のうえ、保守要員に連絡してください。(ID:MSG0006E)
        '        MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        '        BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタチェック", "失敗", ex.Message)
        '        Ret = -1
        '    Finally
        '        If Not REC Is Nothing Then
        '            REC.Close()
        '            REC.Dispose()
        '        End If
        '    End Try
        '
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As StringBuilder
        Dim Ret As Integer = 0

        Try
            SQL = New StringBuilder(128)
            SQL.Append("SELECT *")
            SQL.Append(" FROM TAKOUMAST")
            SQL.Append(" WHERE TKIN_NO_V = '" & Data.TKIN_NO & "'")
            SQL.Append(" AND TORIS_CODE_V = '" & Data.TORIS_CODE & "'")
            SQL.Append(" AND TORIF_CODE_V = '" & Data.TORIF_CODE & "'")

            If OraReader.DataReader(SQL) = True Then
                Do
                    Ret += 1
                Loop Until OraReader.EOF = False
            End If

            OraReader.Close()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタチェック", "失敗", ex.Message)
            Ret = -1
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        End Try

        Return Ret
    End Function

    '============================================================================
    'NAME           :FN_INSERT_TAKOUMAST
    'Parameter      :他行マスタ構造体
    'Description    :セットした値を他行金融機関マスタに登録する
    'Return         :True=OK,False=NG
    'Create         :2004.07.28
    'Update         :2007.12.20 By Astar
    '============================================================================
    Private Function FN_INSERT_TAKOUMAST(ByVal Data As TAKOMAST_INF) As Boolean
        Try
            Dim SQL As String = "INSERT INTO TAKOUMAST"
            SQL &= " (TORIS_CODE_V"
            SQL &= ", TORIF_CODE_V"
            SQL &= ", TKIN_NO_V"
            SQL &= ", TSIT_NO_V"
            SQL &= ", ITAKU_CODE_V"
            SQL &= ", KAMOKU_V"
            SQL &= ", KOUZA_V"
            SQL &= ", LABEL_KBN_V"
            SQL &= ", BAITAI_CODE_V"
            SQL &= ", CODE_KBN_V"
            SQL &= ", SFILE_NAME_V"
            SQL &= ", RFILE_NAME_V"
            SQL &= ", SAKUSEI_DATE_V"
            SQL &= ", KOUSIN_DATE_V"
            SQL &= ") VALUES"
            With Data
                SQL &= " ('" & .TORIS_CODE & "'"
                SQL &= ", '" & .TORIF_CODE & "'"
                SQL &= ", '" & .TKIN_NO & "'"
                SQL &= ", '" & .TSIT_NO & "'"
                SQL &= ", '" & .ITAKU_CODE & "'"
                SQL &= ", '" & .KAMOKU & "'"
                SQL &= ", '" & .KOUZA & "'"
                SQL &= ", '0'"
                SQL &= ", '" & .BAITAI_CODE & "'"
                SQL &= ", '" & .CODE_KBN & "'"
                SQL &= ", '" & .SFILE_NAME & "'"
                SQL &= ", '" & .RFILE_NAME & "'"
                SQL &= ", TO_CHAR(SYSDATE, 'yyyymmdd')"
                SQL &= ", '00000000')"
            End With

            Dim SQLCode As Integer
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
            If Ret = 1 AndAlso SQLCode = 0 Then
                Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, "COMMIT WORK")
                Return True
            End If
        Catch ex As Exception
            With GCom.GLog
                .Job2 = "他行マスタ登録"
                .ToriCode = Data.TORIS_CODE & "-" & Data.TORIF_CODE
                .Result = MenteCommon.clsCommon.NG
                .Discription &= ex.Message
            End With
            'Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ登録", "失敗", ex.Message)
        End Try
        Return False
    End Function

    Private Function FN_DELETE_TAKOUMAST(ByVal Data As TAKOMAST_INF) As Boolean

        Dim MSG As String = ""
        Try
            Dim SQL As String = "DELETE FROM TAKOUMAST"
            SQL &= " WHERE TORIS_CODE_V = '" & Data.TORIS_CODE & "'"
            SQL &= " AND TORIF_CODE_V = '" & Data.TORIF_CODE & "'"
            SQL &= " AND TKIN_NO_V = '" & Data.TKIN_NO & "'"

            Dim SQLCode As Integer
            Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
            If Ret = 1 AndAlso SQLCode = 0 Then
                With GCom.GLog
                    .Job2 = "他行マスタ削除"
                    .Result = MenteCommon.clsCommon.OK
                    .Discription = "金融機関(" & Data.TKIN_NO & ")"
                End With
                'Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
                BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ削除", "成功")
                Return True
            End If
        Catch ex As Exception
            MSG = ex.Message
        End Try

        MessageBox.Show(String.Format(MSG0027E, "他行マスタ", "削除"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

    End Function

    '============================================================================
    'NAME           :FN_UPDATE_TAKOUMAST
    'Parameter      :他行マスタ構造体
    'Description    :セットした値を他行金融機関マスタに更新する
    'Return         :True=OK,False=NG
    'Create         :2004/07/29
    'Update         :2007/09/04 蒲郡信金向け 学校自振対応
    '============================================================================
    Private Function FN_UPDATE_TAKOUMAST(ByVal Data As TAKOMAST_INF) As Boolean
        Try
            Dim SQL As String = "UPDATE TAKOUMAST"
            With Data
                SQL &= " SET TSIT_NO_V = '" & .TSIT_NO & "'"
                SQL &= ", ITAKU_CODE_V = '" & .ITAKU_CODE & "'"
                SQL &= ", KAMOKU_V = '" & .KAMOKU & "'"
                SQL &= ", KOUZA_V = '" & .KOUZA & "'"
                SQL &= ", BAITAI_CODE_V = '" & .BAITAI_CODE & "'"
                SQL &= ", CODE_KBN_V = '" & .CODE_KBN & "'"
                SQL &= ", SFILE_NAME_V = '" & .SFILE_NAME & "'"
                SQL &= ", RFILE_NAME_V = '" & .RFILE_NAME & "'"
                SQL &= ", KOUSIN_DATE_V = TO_CHAR(SYSDATE, 'yyyymmdd')"
                SQL &= " WHERE TORIS_CODE_V = '" & .TORIS_CODE & "'"

                '媒体が「学校自振(７)の時は副コードは見ない
                If Not GCom.NzInt(.BAITAI_CODE) = 7 Then

                    SQL &= " AND TORIF_CODE_V = '" & .TORIF_CODE & "'"
                End If
                SQL &= " AND TKIN_NO_V = '" & .TKIN_NO & "'"

                Dim SQLCode As Integer
                Dim Ret As Integer = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, SQL, SQLCode)
                If Ret = 1 AndAlso SQLCode = 0 Then
                    Call GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Execute, "COMMIT WORK")
                    Return True
                End If
            End With

        Catch ex As Exception
            '   With GCom.GLog
            '       .Job2 = "他行マスタ更新"
            '       .ToriCode = Data.TORIS_CODE & "-" & Data.TORIF_CODE
            '       .Result = MenteCommon.clsCommon.NG
            '       .Discription &= ex.Message
            '   End With
            '   Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
            '呼出元でエラー表示
            ''例外が発生しました。ログを確認のうえ、保守要員に連絡してください。(ID:MSG0006E)
            'MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "他行マスタ更新", "失敗", ex.Message)
        End Try
        Return False
    End Function

    Private Function Fn_Select_TENMAST_2(ByVal Data As TAKOMAST_INF) As Boolean

        Dim OraReader As New CASTCommon.MyOracleReader

        Try
            Dim SQL As New StringBuilder(128)
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "金融機関マスタ参照(開始)", "成功")

            SQL.Append("SELECT KIN_NO_N")
            SQL.Append(", SIT_NO_N")
            SQL.Append(" FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = '" & Data.TKIN_NO & "'")
            SQL.Append(" AND SIT_NO_N = '" & Data.TSIT_NO & "'")
            SQL.Append(" GROUP BY KIN_NO_N")
            SQL.Append(", SIT_NO_N")

            If OraReader.DataReader(SQL) = True Then
                Return True

            End If

            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "金融機関マスタ参照(終了)", "成功")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "金融機関マスタ参照", "失敗", ex.Message)

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        End Try

        Return False
    End Function

    '    '
    '    ' 機能　 ： 金融機関マスタの参照
    '    '
    '    ' 引数　 ： ARG1 - 呼出モード
    '    '
    '    '
    '    ' 戻り値 ： OK = True, NG = False
    '    '
    '    ' 備考　 ：
    '    '
    '    Private Function FN_SELECT_TENMAST(ByVal Data As TAKOMAST_INF) As Boolean
    '
    '        With GCom.GLog
    '            .Job2 = "金融機関マスタ参照関数"
    '            .Result = ""
    '        End With
    '
    '        Dim REC As OracleDataReader
    '
    '        Try
    '            Dim SQL As String = "SELECT KIN_NO_N"
    '            SQL &= ", KIN_FUKA_N"
    '            SQL &= ", SIT_NO_N"
    '            SQL &= ", SIT_FUKA_N"
    '            SQL &= ", KIN_KNAME_N"
    '            SQL &= ", KIN_NNAME_N"
    '            SQL &= ", SIT_KNAME_N"
    '            SQL &= ", SIT_NNAME_N"
    '            SQL &= ", NEW_KIN_NO_N"
    '            SQL &= ", NEW_KIN_FUKA_N"
    '            SQL &= ", NEW_SIT_NO_N"
    '            SQL &= ", NEW_SIT_FUKA_N"
    '            SQL &= ", KIN_DEL_DATE_N"
    '            SQL &= ", SIT_DEL_DATE_N"
    '            SQL &= ", SAKUSEI_DATE_N"
    '            SQL &= ", KOUSIN_DATE_N"
    '            SQL &= " FROM TENMAST"
    '            SQL &= " WHERE TO_NUMBER(KIN_NO_N) = " & Data.TKIN_NO
    '            SQL &= " AND TO_NUMBER(SIT_NO_N) = " & Data.TSIT_NO
    '            SQL &= " AND TO_NUMBER(KIN_FUKA_N) = '1'"
    '            SQL &= " AND TO_NUMBER(SIT_FUKA_N) = '01'"
    '
    '            Dim BRet As Boolean = GCom.SetDynaset(SQL, REC)
    '            If BRet AndAlso REC.Read Then
    '
    '                Return True
    '            End If
    '        Catch ex As Exception
    '            With GCom.GLog
    '                .Result = MenteCommon.clsCommon.NG
    '                .Discription = ex.Message
    '            End With
    '            'Call GCom.FN_LOG_WRITE(ThisModuleName, New StackTrace(True))
    '            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "金融機関マスタ参照", "失敗", ex.Message)
    '        Finally
    '            If Not REC Is Nothing Then
    '                REC.Close()
    '                REC.Dispose()
    '            End If
    '        End Try
    '
    '        Return False
    '    End Function

    '
    ' 機能　 ： 新旧相違数チェック
    '
    ' 引数　 ： ARG1 - 他行マスタ構造体(入力値)
    '
    ' 戻り値 ： 設定値相違数
    '
    ' 備考　 ： 差分チェック
    '
    Private Function CheckCurrentAndNew(ByVal Data As TAKOMAST_INF) As Integer
        Dim Ret As Integer = 0

        Dim memDT() As String = New String(10) {memData.BAITAI_CODE, memData.CODE_KBN, memData.ITAKU_CODE, _
            memData.KAMOKU, memData.KOUZA, memData.RFILE_NAME, memData.SFILE_NAME, memData.TKIN_NO, _
            memData.TORIF_CODE, memData.TORIS_CODE, memData.TSIT_NO}

        Dim onDT() As String = New String(10) {Data.BAITAI_CODE, Data.CODE_KBN, Data.ITAKU_CODE, _
            Data.KAMOKU, Data.KOUZA, Data.RFILE_NAME, Data.SFILE_NAME, Data.TKIN_NO, _
            Data.TORIF_CODE, Data.TORIS_CODE, Data.TSIT_NO}

        For Idx As Integer = 0 To onDT.GetUpperBound(0) Step 1
            If Not memDT(Idx) = onDT(Idx) Then
                Ret += 1
            End If
        Next Idx

        Return Ret
    End Function

    '数値評価(Zero埋めフォーマット有り：金融機関登録エリア：金融機関の場合)
    Private Sub txtKinCode_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtKinCode.Validating

        Dim OBJ As TextBox = CType(sender, TextBox)
        Call GCom.NzNumberString(OBJ, True)
        Me.lblKIN_NM.Text = GetBKBRName(OBJ.Text, "", 40)
    End Sub

    '数値評価(Zero埋めフォーマット有り：金融機関登録エリア：支店の場合)
    Private Sub txtSitCode_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtSitCode.Validating

        Dim OBJ As TextBox = CType(sender, TextBox)
        Call GCom.NzNumberString(OBJ, True)
        Dim BKCode As String = Me.txtKinCode.Text
        Dim BRCode As String
        If OBJ.Text.Trim.Length = 0 Then
            BRCode = " "
        Else
            BRCode = OBJ.Text
        End If
        Me.lblSIT_NM.Text = GetBKBRName(BKCode, BRCode, 40, 1)
    End Sub

    'ファイル名設定領域入力後処理
    Private Sub txtSousinFile_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtSousinFile.Validating, _
            txtJyusinFile.Validating
        Call GCom.NzCheckString(CType(sender, TextBox))
        With CType(sender, TextBox)
            .Text = .Text.ToUpper
        End With
    End Sub

    '口座番号、委託者コードゼロ埋め
    Private Sub NumberZeroText_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
    Handles txtKouza.Validating, _
            txtItakuCode.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
    End Sub

    '
    ' 機能　 ： 金融機関情報検索
    '
    ' 引数　 ： ARG1 - 金融機関コード
    ' 　　　 　 ARG2 - 支店コード
    ' 　　　 　 ARG3 - 制限バイト数
    ' 　　　 　 ARG4 - 0:金融機関名検索、1:支店名検索
    '
    ' 戻り値 ： 金融機関名／支店名
    '
    ' 備考　 ： なし
    '
    Public Function GetBKBRName(ByVal BKCode As String, _
                                ByVal BRCode As String, _
                                Optional ByVal avShort As Integer = 0, _
                                Optional ByVal BKFlg As Integer = 0) As String

        Dim OraReader As CASTCommon.MyOracleReader

        Try
            Dim SQL As New StringBuilder(128)
            OraReader = New CASTCommon.MyOracleReader(MainDB)

            If BKCode.Trim.Length = 0 Then
                Return ""
            End If
            Dim BRFLG As Boolean = (BRCode.Trim.Length > 0)

            SQL.Append("SELECT KIN_NO_N")
            SQL.Append(", SIT_NO_N")
            SQL.Append(", KIN_NNAME_N")
            SQL.Append(", SIT_NNAME_N")
            SQL.Append(" FROM TENMAST")
            SQL.Append(" WHERE KIN_NO_N = '" & BKCode & "'")
            If BRFLG Then
                SQL.Append(" AND SIT_NO_N = '" & BRCode & "'")
            End If
            SQL.Append(" GROUP BY KIN_NO_N")
            SQL.Append(", SIT_NO_N")
            SQL.Append(", KIN_NNAME_N")
            SQL.Append(", SIT_NNAME_N")

            If OraReader.DataReader(SQL) = True Then
                If BRFLG Then

                    If avShort = 0 Then
                        Return GCom.NzStr(OraReader.GetString("SIT_NNAME_N")).Trim
                    Else
                        Return GCom.GetLimitString(OraReader.GetString("SIT_NNAME_N").ToString, avShort)
                    End If
                Else
                    If BKFlg = 0 Then
                        If avShort = 0 Then
                            Return GCom.NzStr(OraReader.GetString("KIN_NNAME_N")).Trim
                        Else
                            Return GCom.GetLimitString(OraReader.GetString("KIN_NNAME_N").ToString, avShort)
                        End If
                    Else
                        Return ""
                    End If
                End If
                If Not OraReader Is Nothing Then
                    OraReader.Close()
                End If
            Else
                If Not OraReader Is Nothing Then
                    OraReader.Close()
                End If
                Return ""
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            BLOG.Write(BLOG.ToriCode, BLOG.FuriDate, "金融機関情報検索", "失敗")
            Return ""
        End Try
    End Function
End Class