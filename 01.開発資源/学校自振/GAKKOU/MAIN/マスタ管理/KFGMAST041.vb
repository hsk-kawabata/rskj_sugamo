Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text
Imports CASTCommon
Public Class KFGMAST041

#Region " 共通変数定義 "
    Private str入学年度 As String
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST041", "長子設定画面")
    Private Const msgTitle As String = "長子設定画面(KFGMAST041)"
#End Region

#Region " Form Load "
    Private Sub KFGMAST041_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            Call PSUB_FORMAT_VALUE()
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
    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
        Try
            'クリアボタン
            txtNENDO.Text = ""
            txtTUUBAN.Text = ""
            txtGAKUNEN.Text = ""
            txtCLASS.Text = ""
            txtSEITONO.Text = ""

            labSEITO_KANA.Text = ""
            labSEITO_KANJI.Text = ""

            txtNENDO.Enabled = True
            txtTUUBAN.Enabled = True
            txtGAKUNEN.Enabled = True
            txtCLASS.Enabled = True
            txtSEITONO.Enabled = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クリア)例外エラー", "失敗", ex.Message)
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            '閉じるボタン
            Select Case True
                Case (Trim(txtNENDO.Text) <> "" And Trim(txtTUUBAN.Text) <> "")
                    STR_SQL = " SELECT * FROM SEITOMAST"
                    STR_SQL += " WHERE"
                    STR_SQL += " GAKKOU_CODE_O ='" & str長子学校コード & "'"
                    STR_SQL += " AND"
                    STR_SQL += " NENDO_O ='" & txtNENDO.Text & "'"
                    STR_SQL += " AND"
                    STR_SQL += " TUUBAN_O = " & txtTUUBAN.Text
                Case (Trim(txtGAKUNEN.Text) <> "" And Trim(txtCLASS.Text) <> "" And Trim(txtSEITONO.Text) <> "")
                    STR_SQL = " SELECT * FROM SEITOMAST"
                    STR_SQL += " WHERE"
                    STR_SQL += " GAKKOU_CODE_O ='" & str長子学校コード & "'"
                    STR_SQL += " AND"
                    STR_SQL += " GAKUNEN_CODE_O = " & txtGAKUNEN.Text
                    STR_SQL += " AND"
                    STR_SQL += " CLASS_CODE_O = " & txtCLASS.Text
                    STR_SQL += " AND"
                    STR_SQL += " SEITO_NO_O ='" & txtSEITONO.Text & "'"
                Case Else
                    Me.Close()

                    Exit Sub
            End Select

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Sub
            End If

            If OBJ_DATAREADER.HasRows = False Then
                str長子入学年度 = ""
                str長子通番 = ""
                str長子学年 = ""
                str長子クラス = ""
                str長子生徒番号 = ""
            Else
                OBJ_DATAREADER.Read()

                str長子入学年度 = OBJ_DATAREADER.Item("NENDO_O")
                str長子通番 = OBJ_DATAREADER.Item("TUUBAN_O")
                str長子学年 = OBJ_DATAREADER.Item("GAKUNEN_CODE_O")
                str長子クラス = OBJ_DATAREADER.Item("CLASS_CODE_O")
                str長子生徒番号 = OBJ_DATAREADER.Item("SEITO_NO_O")
            End If

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Sub
            End If

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)例外エラー", "成功", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Private Sub "
    Private Sub PSUB_FORMAT_VALUE()

        txtNENDO.Text = str長子入学年度
        txtTUUBAN.Text = str長子通番
        txtGAKUNEN.Text = str長子学年
        txtCLASS.Text = str長子クラス
        txtSEITONO.Text = str長子生徒番号

        Select Case True
            Case (Trim(txtNENDO.Text) <> "" And Trim(txtTUUBAN.Text) <> "")
                Call PSUB_Set_SEITONAME(0)
            Case (Trim(txtGAKUNEN.Text) <> "" And Trim(txtCLASS.Text) <> "" And Trim(txtSEITONO.Text) <> "")
                Call PSUB_Set_SEITONAME(1)
        End Select

    End Sub
    Private Sub PSUB_Set_SEITONAME(ByVal pIndex As Integer)

        Select Case pIndex
            Case 0
                STR_SQL = " SELECT * FROM SEITOMAST"
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_O ='" & str長子学校コード.Trim.PadLeft(10, "0"c) & "'"
                STR_SQL += " AND"
                STR_SQL += " NENDO_O ='" & txtNENDO.Text & "'"
                STR_SQL += " AND"
                STR_SQL += " TUUBAN_O = " & txtTUUBAN.Text
            Case 1
                STR_SQL = " SELECT * FROM SEITOMAST"
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_O ='" & str長子学校コード.Trim.PadLeft(10, "0"c) & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN_CODE_O = " & txtGAKUNEN.Text
                STR_SQL += " AND"
                If txtCLASS.Text.Trim = "" Then
                    STR_SQL += " CLASS_CODE_O = 0"
                Else
                    STR_SQL += " CLASS_CODE_O = " & CLng(txtCLASS.Text)
                End If
                STR_SQL += " AND"
                STR_SQL += " SEITO_NO_O ='" & txtSEITONO.Text & "'"
        End Select

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Sub
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Sub
            End If

            '2014/01/06 saitou 標準版 メッセージ定数化 UPD -------------------------------------------------->>>>
            MessageBox.Show(G_MSG0065W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            'MessageBox.Show("生徒マスタに登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '2014/01/06 saitou 標準版 メッセージ定数化 UPD --------------------------------------------------<<<<

            labSEITO_KANA.Text = ""
            labSEITO_KANJI.Text = ""
            txtNENDO.Text = ""
            txtTUUBAN.Text = ""
            txtCLASS.Text = ""
            txtSEITONO.Text = ""


            Select Case pIndex
                Case 0
                    txtGAKUNEN.Enabled = True
                    txtCLASS.Enabled = True
                    txtSEITONO.Enabled = True
                Case 1
                    txtNENDO.Enabled = True
                    txtTUUBAN.Enabled = True
            End Select

            btnClear.Focus()

            Exit Sub
        Else
            OBJ_DATAREADER.Read()

            If IsDBNull(OBJ_DATAREADER.Item("SEITO_KNAME_O")) = True Then
                labSEITO_KANA.Text = ""
            Else
                labSEITO_KANA.Text = OBJ_DATAREADER.Item("SEITO_KNAME_O")
            End If
            If IsDBNull(OBJ_DATAREADER.Item("SEITO_NNAME_O")) = True Then
                labSEITO_KANJI.Text = ""
            Else
                labSEITO_KANJI.Text = OBJ_DATAREADER.Item("SEITO_NNAME_O")
            End If

            txtNENDO.Text = OBJ_DATAREADER.Item("NENDO_O")
            txtTUUBAN.Text = OBJ_DATAREADER.Item("TUUBAN_O")
            txtGAKUNEN.Text = OBJ_DATAREADER.Item("GAKUNEN_CODE_O")
            txtCLASS.Text = Format(CLng(OBJ_DATAREADER.Item("CLASS_CODE_O")), "00")
            txtSEITONO.Text = Format(CLng(OBJ_DATAREADER.Item("SEITO_NO_O")), "0000000")
        End If

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Sub
        End If

        Select Case pIndex
            Case 0
                txtGAKUNEN.Enabled = False
                txtCLASS.Enabled = False
                txtSEITONO.Enabled = False
            Case 1
                txtNENDO.Enabled = False
                txtTUUBAN.Enabled = False
        End Select

    End Sub
#End Region

    Private Sub TextBox0_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    txtNENDO.Validating, _
    txtTUUBAN.Validating

        If txtNENDO.Text.Trim <> "" And txtTUUBAN.Text.Trim <> "" Then
            Call PSUB_Set_SEITONAME(0)
        End If
    End Sub

    Private Sub TextBox1_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    txtGAKUNEN.Validating, _
    txtCLASS.Validating, _
    txtSEITONO.Validating

        If Len(txtGAKUNEN.Text) <> 0 And Len(txtCLASS.Text) <> 0 And Len(txtSEITONO.Text) <> 0 Then
            Call PSUB_Set_SEITONAME(1)
        End If
    End Sub

End Class
