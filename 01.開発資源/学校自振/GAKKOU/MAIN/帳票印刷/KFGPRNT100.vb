Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT100
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 長子チェックリスト印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Private STR学校名 As String
    '2006/10/11 帳票のソート機能追加
    Dim STR帳票ソート順 As String
    Dim STR学校コード As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT100", "長子チェックリスト明細表印刷画面")
    Private Const msgTitle As String = "長子チェックリスト明細表印刷画面(KFGPRNT100)"
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
    Private Sub KFGPRNT100_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

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
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLog.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
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

            '学校コード必須チェック
            If txtGAKKOU_CODE.Text.Trim = "" Then
                MessageBox.Show(String.Format(MSG0285W, "学校コード"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return
            End If

            If PFUNC_GAKNAME_GET() = False Then
                Exit Sub
            End If

            LW.ToriCode = Trim(txtGAKKOU_CODE.Text)


            OraReader = New MyOracleReader(MainDB)

            SQL.Append("SELECT DISTINCT GAKKOU_CODE_O FROM SEITOMAST ")
            SQL.Append(" WHERE TUKI_NO_O = '04'")
            SQL.Append(" AND TYOUSI_FLG_O <> 0 ")
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                SQL.Append(" AND GAKKOU_CODE_O = " & SQ(txtGAKKOU_CODE.Text))
            End If
            SQL.Append(" ORDER BY GAKKOU_CODE_O ASC")

            '生徒存在チェック
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("長子ありの生徒が存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "長子チェックリスト明細表"), _
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

                'ログインID,学校コード,ソート順
                Param = GCom.GetUserID & "," & STR学校コード & "," & STR帳票ソート順
                nRet = ExeRepo.ExecReport("KFGP029.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "長子チェックリスト明細表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select

                OraReader.NextRead()
            End While
            MessageBox.Show(String.Format(MSG0014I, "長子チェックリスト明細表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            If OraReader IsNot Nothing Then OraReader.Close()
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
    Private Function PFUC_SQLQuery_長子リスト() As String
        '2006/10/11 帳票の出力順を指定するため、JYOKENではなくSQLを指定するように変更
        Dim SSQL As String

        PFUC_SQLQuery_長子リスト = ""


        SSQL = " SELECT "
        SSQL = SSQL & " GAKMAST1.GAKKOU_NNAME_G, "
        SSQL = SSQL & " SEITOMAST.GAKKOU_CODE_O, "
        SSQL = SSQL & " SEITOMAST.NENDO_O, "
        SSQL = SSQL & " SEITOMAST.TUUBAN_O, "
        SSQL = SSQL & " SEITOMAST.GAKUNEN_CODE_O, "
        SSQL = SSQL & " SEITOMAST.CLASS_CODE_O, "
        SSQL = SSQL & " SEITOMAST.SEITO_NO_O, "
        SSQL = SSQL & " SEITOMAST.SEITO_KNAME_O, "
        SSQL = SSQL & " SEITOMAST.TYOUSI_NENDO_O, "
        SSQL = SSQL & " SEITOMAST.TYOUSI_GAKUNEN_O, "
        SSQL = SSQL & " SEITOMAST.TYOUSI_CLASS_O, "
        SSQL = SSQL & " SEITOMAST.TYOUSI_SEITONO_O, "
        SSQL = SSQL & " SEITOMAST_1.SEITO_KNAME_O, "
        SSQL = SSQL & " SEITOMAST.TYOUSI_FLG_O, "
        SSQL = SSQL & " SEITOMAST_1.TUUBAN_O "
        SSQL = SSQL & " FROM   "
        SSQL = SSQL & " KZFMAST.SEITOMAST SEITOMAST, "
        SSQL = SSQL & " KZFMAST.GAKMAST1 GAKMAST1, "
        SSQL = SSQL & " KZFMAST.SEITOMAST SEITOMAST_1"
        SSQL = SSQL & "  WHERE  "
        SSQL = SSQL & " ((SEITOMAST.GAKKOU_CODE_O=GAKMAST1.GAKKOU_CODE_G) AND "
        SSQL = SSQL & " (SEITOMAST.GAKUNEN_CODE_O=GAKMAST1.GAKUNEN_CODE_G)) AND "
        SSQL = SSQL & " (((((((SEITOMAST.GAKKOU_CODE_O=SEITOMAST_1.GAKKOU_CODE_O (+)) AND "
        SSQL = SSQL & " (SEITOMAST.TYOUSI_NENDO_O=SEITOMAST_1.NENDO_O (+))) AND "
        SSQL = SSQL & " (SEITOMAST.TYOUSI_TUUBAN_O=SEITOMAST_1.TUUBAN_O (+))) AND "
        SSQL = SSQL & " (SEITOMAST.TYOUSI_GAKUNEN_O=SEITOMAST_1.GAKUNEN_CODE_O (+))) AND "
        SSQL = SSQL & " (SEITOMAST.TYOUSI_CLASS_O=SEITOMAST_1.CLASS_CODE_O (+))) AND "
        SSQL = SSQL & " (SEITOMAST.TYOUSI_SEITONO_O=SEITOMAST_1.SEITO_NO_O (+))) AND "
        SSQL = SSQL & " (SEITOMAST.TUKI_NO_O=SEITOMAST_1.TUKI_NO_O (+))) AND "

        If Trim(STR学校コード) <> "9999999999" Then
            '生徒マスタ（学校コード、作成日、更新日）
            'レコード抽出設定
            '指定学校コード
            SSQL = SSQL & " SEITOMAST.GAKKOU_CODE_O = '" & Trim(STR学校コード) & "' AND "
        End If

        SSQL = SSQL & " SEITOMAST.TUKI_NO_O = '04' AND "
        SSQL = SSQL & " SEITOMAST.TYOUSI_FLG_O <>0 "

        SSQL = SSQL & " ORDER BY "
        SSQL = SSQL & " SEITOMAST.GAKKOU_CODE_O　ASC, "
        Select Case STR帳票ソート順
            Case "0"
                SSQL = SSQL & " SEITOMAST.GAKUNEN_CODE_O　ASC, "
                SSQL = SSQL & " SEITOMAST.CLASS_CODE_O     ASC,"
                SSQL = SSQL & " SEITOMAST.SEITO_NO_O       ASC,"
                SSQL = SSQL & " SEITOMAST.NENDO_O  ASC, "
                SSQL = SSQL & " SEITOMAST.TUUBAN_O  ASC"
            Case "1"
                SSQL = SSQL & " SEITOMAST.GAKUNEN_CODE_O　ASC, "
                '2007/02/14 年度を降順に修正
                'SSQL = SSQL & " SEITOMAST.NENDO_O  ASC, "
                SSQL = SSQL & " SEITOMAST.NENDO_O  DESC, "
                SSQL = SSQL & " SEITOMAST.TUUBAN_O  ASC"
            Case Else
                SSQL = SSQL & " SEITOMAST.GAKUNEN_CODE_O　ASC, "
                SSQL = SSQL & " SEITOMAST.SEITO_KNAME_O   ASC,"
                SSQL = SSQL & " SEITOMAST.NENDO_O  DESC, "
                SSQL = SSQL & " SEITOMAST.TUUBAN_O  ASC"
        End Select
        PFUC_SQLQuery_長子リスト = SSQL

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
