Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text

Public Class KFGPRNT030
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 口座振替結果帳票印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Private gstrLISTWORKS_FLG As String  '2007/10/07追加

#Region " 共通変数定義 "
    Private gstrPRINT_DEVICE As String
    Private gstrPRINT_NAME As String
    Private gstrPRINT_PORT As String

    Private Str_Report_Path As String
    Private STR学校コード As String
    Private STR処理名 As String
    Private STR帳票ソート順 As String
    Private STR振替区分 As String
    Private STR初振日 As String

    Private STR_REPORT_KBN(5) As String
    '2006/10/20
    Private blnPRINT_FLG As Boolean
    Private STR再振替日 As String
    Private STR対象年月 As String


    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT030", "口座振替結果帳票印刷画面")
    Private Const msgTitle As String = "口座振替結果帳票印刷画面(KFGPRNT030)"
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- START
    Private SFuriCode As String = String.Empty
    ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- END

#End Region

#Region " Form Load "
    '****************************
    'Form Load
    '****************************
    Private Sub KFGPRNT030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

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
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- START
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            ' 2017/06/06 タスク）綾部 ADD 【OT】(再振対象コードIniファイル化) -------------------- END

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
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click

        Try
            '印刷ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            MainDB = New MyOracle

            blnPRINT_FLG = False
            '印刷ボタン
            '入力チェック
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            LW.ToriCode = Trim(txtGAKKOU_CODE.Text)
            LW.FuriDate = STR_FURIKAE_DATE(1)

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "結果帳票"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            '出力帳票選択チェック
            If chk振替結果一覧.Checked = False AndAlso chk振替不能明細.Checked = False AndAlso chk収納報告書.Checked = False AndAlso _
               chk振替店別集計表.Checked = False AndAlso chk振替未納のお知らせ.Checked = False AndAlso chk入金伝票.Checked = False Then
                MessageBox.Show(G_MSG0026W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If chk振替結果一覧.Checked = True Then
                If PrintKFGP030_1() = False Then
                    Exit Sub
                End If
            End If

            If chk振替不能明細.Checked = True Then
                If PrintKFGP030_2() = False Then
                    Exit Sub
                End If
            End If


            If chk収納報告書.Checked = True Then
                If PrintKFGP030_3() = False Then
                    Exit Sub
                End If
            End If

            If chk振替店別集計表.Checked = True Then
                If PrintKFGP030_4() = False Then
                    Exit Sub
                End If
            End If

            If chk振替未納のお知らせ.Checked = True Then
                If PrintKFGP030_5() = False Then
                    Exit Sub
                End If
            End If

            If chk入金伝票.Checked = True Then
                If PrintKFGP030_6() = False Then
                    Exit Sub
                End If
            End If

            '1枚でも印刷していたら、完了メッセージを出力する
            If blnPRINT_FLG = True Then
                MessageBox.Show(String.Format(MSG0014I, "結果帳票"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MainDB.Rollback()
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
        lab学校名.Text = cmbGakkouName.Text.Trim
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
        STR学校コード = txtGAKKOU_CODE.Text.Trim

        '学校名の取得
        If PFUNC_GAKNAME_GET() = False Then
            Exit Sub
        End If

        '学校コードにカーソル設定
        txtGAKKOU_CODE.Focus()
    End Sub
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
        With CType(sender, TextBox)
            If .Text.Trim <> "" Then
                STR学校コード = Trim(txtGAKKOU_CODE.Text)
                '学校名の取得
                If PFUNC_GAKNAME_GET() = False Then
                    Exit Sub
                End If
            End If
        End With
    End Sub
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
        Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write("ゼロパディング", "失敗", ex.ToString)
        End Try
    End Sub
    '2016/10/07 ayabe RSV2 ADD 学校諸会費メンテナンス ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

#End Region
#Region " Private Sub "
    '****************************
    'Private Sub
    '****************************
    Private Sub PSUB_CHK_ONOFF()

        '口座振替結果一覧表
        Select Case STR_REPORT_KBN(0)
            Case "1"
                chk振替結果一覧.Checked = True
            Case Else
                chk振替結果一覧.Checked = False
        End Select

        '口座振替不能明細一覧表
        Select Case STR_REPORT_KBN(1)
            Case "1"
                chk振替不能明細.Checked = True
            Case Else
                chk振替不能明細.Checked = False
        End Select

        '収納報告書
        Select Case STR_REPORT_KBN(2)
            Case "1"
                chk収納報告書.Checked = True
            Case Else
                chk収納報告書.Checked = False
        End Select

        '口座振替店別集計表
        Select Case STR_REPORT_KBN(3)
            Case "1"
                chk振替店別集計表.Checked = True
            Case Else
                chk振替店別集計表.Checked = False
        End Select

        '口座振替未納のお知らせ
        Select Case STR_REPORT_KBN(4)
            Case "1"
                chk振替未納のお知らせ.Checked = True
            Case Else
                chk振替未納のお知らせ.Checked = False
        End Select

        '要求性預金入金伝票
        Select Case STR_REPORT_KBN(5)
            Case "1"
                chk入金伝票.Checked = True
            Case Else
                chk入金伝票.Checked = False
        End Select

    End Sub
#End Region
#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET() As Boolean

        '学校名の設定
        PFUNC_GAKNAME_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim OraDB As New MyOracle
        Try
            If Trim(STR学校コード) = "9999999999" Then
                lab学校名.Text = ""
            Else
                OraReader = New MyOracleReader(OraDB)
                SQL.Append(" SELECT ")
                SQL.Append(" GAKMAST1.*")
                SQL.Append(",MEISAI_FUNOU_T")
                SQL.Append(",MEISAI_KEKKA_T")
                SQL.Append(",MEISAI_HOUKOKU_T")
                SQL.Append(",MEISAI_TENBETU_T")
                SQL.Append(",MEISAI_MINOU_T")
                SQL.Append(",MEISAI_YOUKYU_T")
                SQL.Append(",MEISAI_OUT_T")
                SQL.Append(" FROM GAKMAST1,GAKMAST2")
                SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T")
                SQL.Append(" AND GAKKOU_CODE_G =" & SQ(STR学校コード))

                If OraReader.DataReader(SQL) = False Then
                    lab学校名.Text = ""
                    STR帳票ソート順 = 0
                    Exit Function
                End If

                If txtGAKKOU_CODE.Text <> "9999999999" Then
                    lab学校名.Text = OraReader.GetString("GAKKOU_NNAME_G")
                    STR_REPORT_KBN(0) = OraReader.GetString("MEISAI_FUNOU_T")
                    STR_REPORT_KBN(1) = OraReader.GetString("MEISAI_KEKKA_T")
                    STR_REPORT_KBN(2) = OraReader.GetString("MEISAI_HOUKOKU_T")
                    STR_REPORT_KBN(3) = OraReader.GetString("MEISAI_TENBETU_T")
                    STR_REPORT_KBN(4) = OraReader.GetString("MEISAI_MINOU_T")
                    STR_REPORT_KBN(5) = OraReader.GetString("MEISAI_YOUKYU_T")
                End If

                STR帳票ソート順 = OraReader.GetString("MEISAI_OUT_T")

                OraReader.Close()

                If txtGAKKOU_CODE.Text <> "9999999999" Then
                    Call PSUB_CHK_ONOFF()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校名取得)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraDB IsNot Nothing Then OraDB.Close()
        End Try
        PFUNC_GAKNAME_GET = True

    End Function
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
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

            STR_FURIKAE_DATE(0) = Trim(txtFuriDateY.Text) & "/" & Trim(txtFuriDateM.Text) & "/" & Trim(txtFuriDateD.Text)
            STR_FURIKAE_DATE(1) = Trim(txtFuriDateY.Text) & Format(CInt(txtFuriDateM.Text), "00") & Format(CInt(txtFuriDateD.Text), "00")

            SQL.Append("SELECT * FROM G_SCHMAST")
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
                '2006/10/11　不能済みを条件に追加
                SQL.Append(" AND FUNOU_FLG_S = '1'")
            Else
                SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(Trim(txtGAKKOU_CODE.Text)))
                SQL.Append(" AND FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
            End If
            SQL.Append(" AND (FURI_KBN_S = '0' OR FURI_KBN_S='1')")

            'ｽｹｼﾞｭｰﾙ区分2(随時)は処理に含まない為
            SQL.Append(" AND SCH_KBN_S <> '2'")

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            If OraReader.GetString("FUNOU_FLG_S") <> "1" Then
                MessageBox.Show(MSG0085W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_SCHMAST_GET() As Boolean

        PFUNC_SCHMAST_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)

            SQL.Append(" SELECT * FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S  = " & SQ(STR学校コード))
            SQL.Append(" AND FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            '振替区分の取得
            STR振替区分 = OraReader.GetString("FURI_KBN_S")
            STR対象年月 = OraReader.GetString("NENGETUDO_S")
            OraReader.Close()

            '初振日の取得
            If STR振替区分 = "1" Then
                OraReader = New MyOracleReader(MainDB)
                SQL = New StringBuilder(128)
                '再振日を画面で入力してきた場合に、
                '画面で入力してきた振替日を再振日にもつ初振のスケジュールを取得し
                'その初振日を退避しておく
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_S  = " & SQ(Trim(STR学校コード)))
                SQL.Append(" AND SFURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND FURI_KBN_S ='0'")

                If OraReader.DataReader(SQL) Then
                    '初振日の取得
                    STR初振日 = OraReader.GetString("FURI_DATE_S")
                Else
                    STR初振日 = ""
                End If

                OraReader.Close()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュールチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_SCHMAST_GET = True

    End Function
#End Region
#Region " 結果一覧表印刷関数"
    Private Function PrintKFGP030_1() As Boolean

        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                STR学校コード = txtGAKKOU_CODE.Text
                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
                '合算は再振日を分割したとき、最後の再振日の処理が終わるまで印刷不可
                If chk振替結果一覧合算出力.Checked = True Then
                    SQL.Append(" SELECT * FROM G_SCHMAST")
                    SQL.Append(" WHERE GAKKOU_CODE_S  = " & SQ(Trim(STR学校コード)))
                    SQL.Append(" AND NENGETUDO_S = " & SQ(STR対象年月))
                    SQL.Append(" AND FURI_DATE_S > " & SQ(STR_FURIKAE_DATE(1)))
                    SQL.Append(" AND FURI_KBN_S ='1'")

                    'スケジュール存在チェック
                    If OraReader.DataReader(SQL) = True Then
                        MessageBox.Show(G_MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txtGAKKOU_CODE.Focus()
                        Exit Function
                    End If
                End If
                OraReader.Close()

                STR学校コード = txtGAKKOU_CODE.Text
                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me
                Dim nRet As Integer
                Dim Param As String
                '口座振替結果一覧表印刷 
                'パラメータ設定：ログイン名,学校コード,振替日,初振日,対象年月,合算判定,印刷区分("0"固定),振替区分,帳票ソート順
                Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR初振日 & "," & _
                        STR対象年月 & "," & IIf(chk振替結果一覧合算出力.Checked, "1", "0") & ",0," & STR振替区分 & "," & STR帳票ソート順

                nRet = ExeRepo.ExecReport("KFGP016.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "口座振替結果一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                End Select
                blnPRINT_FLG = True
            Else
                ''全学校レコードが対象
                SQL.Append(" SELECT DISTINCT GAKKOU_CODE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S =" & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND (FURI_KBN_S ='0'")
                SQL.Append(" OR FURI_KBN_S ='1')")
                '2006/10/11 不能フラグを条件に追加
                SQL.Append(" AND FUNOU_FLG_S ='1' ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

                'スケジュールが存在チェック
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                While OraReader.EOF = False
                    STR学校コード = OraReader.GetString("GAKKOU_CODE_S")
                    '帳票ソート順の取得
                    If PFUNC_GAKNAME_GET() = False Then
                        STR帳票ソート順 = "0"
                    End If

                    '2014/04/28 saitou 西濃信金 MODIFY ----------------------------------------------->>>>
                    '口座振替結果一覧表不具合修正（ALL9印刷時に異常終了する）
                    If PFUNC_SCHMAST_GET() = False Then
                        Exit While
                    End If
                    '2014/04/28 saitou 西濃信金 MODIFY -----------------------------------------------<<<<

                    SQL = New StringBuilder(128)
                    '2006/10/27 合算は再振日を分割したとき、最後の再振日の処理が終わるまで印刷不可
                    If chk振替結果一覧合算出力.Checked = True Then
                        SQL = New StringBuilder(128)
                        SQL.Append(" SELECT * FROM G_SCHMAST")
                        SQL.Append(" WHERE GAKKOU_CODE_S  = '" & Trim(STR学校コード) & "'")
                        SQL.Append(" AND NENGETUDO_S = '" & STR対象年月 & "'")
                        SQL.Append(" AND FURI_DATE_S > '" & STR_FURIKAE_DATE(1) & "'")
                        SQL.Append(" AND FURI_KBN_S ='1'")

                        Dim OraReader2 As New MyOracleReader(MainDB)
                        'スケジュールが存在チェック
                        If OraReader2.DataReader(SQL) = True Then
                            'Call GSUB_MESSAGE_WARNING( "再振日を分割した場合、合算指定は最後の再振日でしか印刷できません")
                            'txtGAKKOU_CODE.Focus()
                            OraReader2.Close()
                            GoTo next_GAKKOU
                        End If
                    End If

                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    Dim nRet As Integer
                    Dim Param As String
                    '口座振替結果一覧表印刷 
                    'パラメータ設定：ログイン名,学校コード,振替日,初振日,対象年月,合算判定,印刷区分("0"固定),振替区分,帳票ソート順
                    Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR初振日 & "," & _
                            STR対象年月 & "," & IIf(chk振替結果一覧合算出力.Checked, "1", "0") & ",0," & STR振替区分 & "," & STR帳票ソート順

                    nRet = ExeRepo.ExecReport("KFGP016.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "口座振替結果一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return False
                    End Select

                    blnPRINT_FLG = True
next_GAKKOU:
                    OraReader.NextRead()
                End While
                OraReader.Close()
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

    End Function
#End Region
#Region " 不能結果一覧表印刷関数"
    Private Function PrintKFGP030_2() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)

            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                STR学校コード = txtGAKKOU_CODE.Text
                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If

                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me
                Dim nRet As Integer
                Dim Param As String
                '不能結果一覧表印刷 
                'パラメータ設定：ログイン名,学校コード,振替日,初振日,対象年月,合算判定,印刷区分("1"固定),振替区分,帳票ソート順
                Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR初振日 & "," & _
                        STR対象年月 & "," & IIf(chk振替結果一覧合算出力.Checked, "1", "0") & ",1," & STR振替区分 & "," & STR帳票ソート順


                nRet = ExeRepo.ExecReport("KFGP016.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "口座振替不能明細一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                End Select
                blnPRINT_FLG = True

            Else
                ''全学校レコードが対象
                SQL.Append(" SELECT distinct GAKKOU_CODE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND ( FURI_KBN_S ='0'")
                SQL.Append(" OR FURI_KBN_S ='1')")

                '2006/10/11 不能フラグを条件に追加
                SQL.Append(" AND FUNOU_FLG_S ='1' ")

                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

                'スケジュールが存在チェック
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                While OraReader.EOF = False
                    STR学校コード = OraReader.GetString("GAKKOU_CODE_S")
                    '帳票ソート順の取得
                    If PFUNC_GAKNAME_GET() = False Then
                        STR帳票ソート順 = "0"
                    End If

                    If PFUNC_SCHMAST_GET() = False Then
                        Exit While
                    End If

                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    Dim nRet As Integer
                    Dim Param As String
                    '不能結果一覧表印刷 
                    'パラメータ設定：ログイン名,学校コード,振替日,初振日,対象年月,合算判定,印刷区分("1"固定),振替区分,帳票ソート順
                    Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR初振日 & "," & _
                            STR対象年月 & "," & IIf(chk振替結果一覧合算出力.Checked, "1", "0") & ",1," & STR振替区分 & "," & STR帳票ソート順

                    nRet = ExeRepo.ExecReport("KFGP016.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "口座振替不能明細一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return False
                    End Select

                    blnPRINT_FLG = True
                    OraReader.NextRead()
                End While
                OraReader.Close()
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(不能結果一覧表印刷関数)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
#End Region
#Region " 収納報告書印刷関数"
    Private Function PrintKFGP030_3() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            Dim nRet As Integer
            Dim Param As String
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                STR学校コード = Trim(txtGAKKOU_CODE.Text)
                If PFUNC_収納報告書() = True Then
                    '収納報告書印刷 
                    'パラメータ設定：ログイン名,学校コード,振替日
                    Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1)
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    nRet = ExeRepo.ExecReport("KFGP018.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "収納報告書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return False
                    End Select
                    blnPRINT_FLG = True
                Else
                End If
            Else
                ''全学校レコードが対象
                SQL.Append(" SELECT GAKKOU_CODE_S,SFURI_DATE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND (FURI_KBN_S ='0' OR FURI_KBN_S ='1')")
                '2006/10/11 不能フラグを条件に追加
                SQL.Append(" AND FUNOU_FLG_S ='1' ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC,SFURI_DATE_S DESC")

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                STR学校コード = ""
                While OraReader.EOF = False
                    If STR学校コード = OraReader.GetString("GAKKOU_CODE_S") Then
                    Else
                        STR学校コード = OraReader.GetString("GAKKOU_CODE_S")
                        STR再振替日 = OraReader.GetString("SFURI_DATE_S")

                        '収納報告書印刷 
                        'パラメータ設定：ログイン名,学校コード,振替日
                        Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1)
                        Dim ExeRepo As New CAstReports.ClsExecute
                        ExeRepo.SetOwner = Me
                        nRet = ExeRepo.ExecReport("KFGP018.EXE", Param)
                        '戻り値に対応したメッセージを表示する
                        Select Case nRet
                            Case 0
                            Case Else
                                '印刷失敗メッセージ
                                MessageBox.Show(String.Format(MSG0004E, "収納報告書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return False
                        End Select

                        blnPRINT_FLG = True
                    End If
                    OraReader.NextRead()
                End While
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(収納報告書印刷関数)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
    Private Function PFUNC_収納報告書() As Boolean

        PFUNC_収納報告書 = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            '印刷を行うデータが存在するかどうかの判定
            SQL.Append(" SELECT distinct GAKKOU_CODE_S,SFURI_DATE_S")
            SQL.Append(" FROM G_MEIMAST,G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_M = " & SQ(STR_FURIKAE_DATE(1)))
            Select Case Trim(STR学校コード)
                Case Is <> "9999999999"
                    '指定学校コード
                    SQL.Append(" AND GAKKOU_CODE_M  =" & SQ(Trim(STR学校コード)))
            End Select
            SQL.Append(" AND (FURI_KBN_M = '0' OR FURI_KBN_M = '1') ")

            '2006/10/11 不能フラグを条件に追加
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M =G_SCHMAST.GAKKOU_CODE_S ")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M =G_SCHMAST.FURI_DATE_S ")
            SQL.Append(" AND FUNOU_FLG_S ='1' ")
            SQL.Append(" ORDER BY SFURI_DATE_S DESC ")


            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(G_MSG0009I, "収納報告書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Function
            End If

            While OraReader.EOF = False
                STR学校コード = OraReader.GetString("GAKKOU_CODE_S")
                STR再振替日 = OraReader.GetString("SFURI_DATE_S")
                OraReader.NextRead()
            End While

            PFUNC_収納報告書 = True
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function

#End Region
#Region " 口座振替店別集計表印刷関数"
    Private Function PrintKFGP030_4() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Dim nRet As Integer
        Dim Param As String
        Try
            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                STR学校コード = Trim(txtGAKKOU_CODE.Text)
                If PFUNC_口座振替店別集計表() = True Then
                    '口座振替店別集計表印刷 
                    'パラメータ設定：ログイン名,学校コード,振替日,入出金区分
                    Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR振替区分
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    nRet = ExeRepo.ExecReport("KFGP019.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "口座振替店別集計表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return False
                    End Select
                    blnPRINT_FLG = True
                Else
                    MessageBox.Show(String.Format(G_MSG0009I, "口座振替店別集計表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                ''全学校レコードが対象
                SQL.Append(" SELECT GAKKOU_CODE_S,SFURI_DATE_S,FURI_KBN_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S =" & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND ( FURI_KBN_S ='0' OR FURI_KBN_S ='1')")

                '2006/10/11 不能フラグを条件に追加
                SQL.Append(" AND FUNOU_FLG_S ='1' ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC,SFURI_DATE_S DESC")

                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                STR学校コード = ""
                While OraReader.EOF = False
                    If STR学校コード = OraReader.GetString("GAKKOU_CODE_S") Then
                    Else
                        STR学校コード = OraReader.GetString("GAKKOU_CODE_S")
                        STR再振替日 = OraReader.GetString("SFURI_DATE_S")
                        STR振替区分 = OraReader.GetString("FURI_KBN_S")
                        '口座振替店別集計表印刷 
                        'パラメータ設定：ログイン名,学校コード,振替日,入出金区分
                        Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR振替区分
                        Dim ExeRepo As New CAstReports.ClsExecute
                        ExeRepo.SetOwner = Me
                        nRet = ExeRepo.ExecReport("KFGP019.EXE", Param)
                        '戻り値に対応したメッセージを表示する
                        Select Case nRet
                            Case 0
                            Case Else
                                '印刷失敗メッセージ
                                MessageBox.Show(String.Format(MSG0004E, "口座振替店別集計表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return False
                        End Select
                        blnPRINT_FLG = True
                    End If
                    OraReader.NextRead()
                End While
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座振替店別集計表印刷関数)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
    Private Function PFUNC_口座振替店別集計表() As Boolean

        PFUNC_口座振替店別集計表 = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            '印刷を行うデータが存在するかどうかの判定
            SQL.Append(" SELECT * FROM G_MEIMAST,G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_M = " & SQ(STR_FURIKAE_DATE(1)))
            Select Case Trim(txtGAKKOU_CODE.Text)
                Case Is <> "9999999999"
                    '指定学校コード
                    SQL.Append(" AND GAKKOU_CODE_M  =" & SQ(Trim(txtGAKKOU_CODE.Text)))
            End Select
            SQL.Append(" AND (FURI_KBN_M = '0' OR FURI_KBN_M = '1') ")

            '存在チェックは自行分・他行分まとめて行う 2006/10/04
            'SQL.Append(" AND")
            'SQL.Append( " TKIN_NO_M ='" & STR自金庫コード_INI & "'")
            '2006/10/11 不能フラグを条件に追加
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M =G_SCHMAST.GAKKOU_CODE_S ")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M =G_SCHMAST.FURI_DATE_S ")
            SQL.Append(" AND FUNOU_FLG_S ='1' ")

            If OraReader.DataReader(SQL) = False Then
                Exit Function
            End If

            STR振替区分 = OraReader.GetString("FURI_KBN_M")

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_口座振替店別集計表 = True

    End Function

#End Region
#Region " 口座振替未納のお知らせ印刷関数"
    Private Function PrintKFGP030_5() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Dim Param As String
        Dim nRet As Integer
        Try
            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                '印刷判定
                If PFUNC_未納のお知らせ印刷可能判定() = True Then
                    '口座振替未納のお知らせ
                    'パラメータ設定：ログイン名,学校コード,振替日,ソート順
                    Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR帳票ソート順
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    nRet = ExeRepo.ExecReport("KFGP020.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "口座振替未納のお知らせ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return False
                    End Select
                    blnPRINT_FLG = True
                Else
                    MessageBox.Show(String.Format(G_MSG0009I, "口座振替未納のお知らせ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                ''全学校レコードが対象
                SQL.Append(" SELECT GAKKOU_CODE_S,SFURI_DATE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S =" & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND( FURI_KBN_S ='0' OR FURI_KBN_S ='1')")

                '2006/10/11 不能フラグを条件に追加
                SQL.Append(" AND FUNOU_FLG_S ='1' ")

                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC,SFURI_DATE_S DESC")

                'スケジュールが存在チェック
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                STR学校コード = ""
                Dim intINSATU_FLG As Integer = 0
                While OraReader.EOF = False
                    If STR学校コード = OraReader.GetString("GAKKOU_CODE_S") Then
                    Else
                        STR学校コード = OraReader.GetString("GAKKOU_CODE_S")
                        STR再振替日 = OraReader.GetString("SFURI_DATE_S")
                        '帳票ソート順の取得
                        If PFUNC_GAKNAME_GET() = False Then
                            STR帳票ソート順 = "0"
                        End If

                        '印刷判定
                        If PFUNC_未納のお知らせ印刷可能判定() = True Then
                            intINSATU_FLG = 1
                            '口座振替未納のお知らせ
                            'パラメータ設定：ログイン名,学校コード,振替日,ソート順
                            Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR帳票ソート順
                            Dim ExeRepo As New CAstReports.ClsExecute
                            ExeRepo.SetOwner = Me
                            nRet = ExeRepo.ExecReport("KFGP020.EXE", Param)
                            '戻り値に対応したメッセージを表示する
                            Select Case nRet
                                Case 0
                                Case Else
                                    '印刷失敗メッセージ
                                    MessageBox.Show(String.Format(MSG0004E, "口座振替未納のお知らせ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Return False
                            End Select
                            blnPRINT_FLG = True
                        Else
                            MessageBox.Show(String.Format(G_MSG0009I, "口座振替未納のお知らせ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                        If intINSATU_FLG = 0 Then
                            MessageBox.Show(String.Format(G_MSG0009I, "口座振替未納のお知らせ"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    End If
                    OraReader.NextRead()
                End While
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座振替未納のお知らせ印刷関数)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
    Private Function PFUNC_未納のお知らせ印刷可能判定() As Boolean

        PFUNC_未納のお知らせ印刷可能判定 = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            '印刷を行うデータが存在するかどうかの判定
            SQL.Append(" SELECT * FROM G_MEIMAST,G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_M = " & SQ(STR_FURIKAE_DATE(1)))
            Select Case Trim(STR学校コード)
                Case Is <> "9999999999"
                    '指定学校コード
                    SQL.Append(" AND GAKKOU_CODE_M  = " & SQ(Trim(STR学校コード)))
            End Select
            SQL.Append(" AND (FURI_KBN_M = '0' OR FURI_KBN_M = '1') ")
            ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- START
            'SQL.Append(" AND FURIKETU_CODE_M <>0 ")
            SQL.Append(" AND FURIKETU_CODE_M IN (" & SFuriCode & ")")
            ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- END
            '2006/10/11 不能フラグを条件に追加
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M =G_SCHMAST.GAKKOU_CODE_S ")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M =G_SCHMAST.FURI_DATE_S ")
            SQL.Append(" AND FUNOU_FLG_S ='1' ")

            If OraReader.DataReader(SQL) = False Then
                Return False
            End If

            STR振替区分 = OraReader.GetString("FURI_KBN_M")


            PFUNC_未納のお知らせ印刷可能判定 = True
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
#End Region
#Region " 要求性預金入金伝票印刷関数"
    Private Function PrintKFGP030_6() As Boolean
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Dim Param As String
        Dim nRet As Integer
        Try
            OraReader = New MyOracleReader(MainDB)
            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                If PFUNC_入金伝票印刷可能判定() = True Then
                    '要求性入金伝票(普通預金入金伝票)印刷
                    'パラメータ設定：ログイン名,学校コード,振替日,ソート順
                    Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR帳票ソート順
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    nRet = ExeRepo.ExecReport("KFGP021.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "要求性入金伝票"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return False
                    End Select
                    blnPRINT_FLG = True
                Else
                    MessageBox.Show(String.Format(G_MSG0009I, "要求性入金伝票"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                ''全学校レコードが対象
                SQL.Append(" SELECT DISTINCT GAKKOU_CODE_S FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND( FURI_KBN_S ='0' OR FURI_KBN_S ='1')")
                '2006/10/11 不能フラグを条件に追加
                SQL.Append(" AND FUNOU_FLG_S ='1' ")
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

                'スケジュールが存在チェック
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                Dim intINSATU_FLG2 As Integer = 0
                While OraReader.EOF = False
                    STR学校コード = OraReader.GetString("GAKKOU_CODE_S")
                    '帳票ソート順の取得
                    If PFUNC_GAKNAME_GET() = False Then
                        STR帳票ソート順 = "0"
                    End If

                    If PFUNC_入金伝票印刷可能判定() = True Then
                        intINSATU_FLG2 = 1
                        '要求性入金伝票(普通預金入金伝票)印刷
                        'パラメータ設定：ログイン名,学校コード,振替日,ソート順
                        Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURIKAE_DATE(1) & "," & STR帳票ソート順
                        Dim ExeRepo As New CAstReports.ClsExecute
                        ExeRepo.SetOwner = Me
                        nRet = ExeRepo.ExecReport("KFGP021.EXE", Param)
                        '戻り値に対応したメッセージを表示する
                        Select Case nRet
                            Case 0
                            Case Else
                                '印刷失敗メッセージ
                                MessageBox.Show(String.Format(MSG0004E, "要求性入金伝票"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return False
                        End Select
                        blnPRINT_FLG = True
                    Else
                        'MessageBox.Show("帳票(要求性入金伝票)は印刷対象が１件も存在しませんでした。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If

                    If intINSATU_FLG2 = 0 Then
                        MessageBox.Show(String.Format(G_MSG0009I, "要求性入金伝票"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If

                    OraReader.NextRead()
                End While
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(要求性入金伝票印刷関数)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
    End Function
    Private Function PFUNC_入金伝票印刷可能判定() As Boolean

        PFUNC_入金伝票印刷可能判定 = False
        Dim SQL As New StringBuilder(128)
        Dim OraReader As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            '印刷を行うデータが存在するかどうかの判定
            SQL.Append(" SELECT * FROM G_MEIMAST,G_SCHMAST")
            SQL.Append(" WHERE FURI_DATE_M = " & SQ(STR_FURIKAE_DATE(1)))
            Select Case Trim(STR学校コード)
                Case Is <> "9999999999"
                    '指定学校コード
                    SQL.Append(" AND GAKKOU_CODE_M  =" & SQ(Trim(STR学校コード)))
            End Select
            SQL.Append(" AND (FURI_KBN_M = '0' OR FURI_KBN_M = '1' ) ")
            SQL.Append(" AND TKIN_NO_M =" & SQ(STR_JIKINKO_CODE))
            ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- START
            'SQL.Append(" AND FURIKETU_CODE_M <> 0 ")
            SQL.Append(" AND FURIKETU_CODE_M IN (" & SFuriCode & ")")
            ' 2017/06/06 タスク）綾部 CHG 【OT】(再振対象コードIniファイル化) -------------------- END
            '2006/10/11 不能フラグを条件に追加
            SQL.Append(" AND G_MEIMAST.GAKKOU_CODE_M =G_SCHMAST.GAKKOU_CODE_S ")
            SQL.Append(" AND G_MEIMAST.FURI_DATE_M =G_SCHMAST.FURI_DATE_S ")
            SQL.Append(" AND FUNOU_FLG_S ='1' ")

            If OraReader.DataReader(SQL) = False Then
                Return False
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        PFUNC_入金伝票印刷可能判定 = True

    End Function
#End Region
#Region " 旧クエリ(クリレポ)"
    'Private Function PFUC_SQLQuery_振替結果() As String
    '    Dim SSQL As String

    '    PFUC_SQLQuery_振替結果 = ""


    '    SSQL = "SELECT "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.NENDO_M"
    '    SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
    '    SSQL = SSQL & ",G_MEIMAST.TUUBAN_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU1_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU2_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU3_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU4_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU5_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU6_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU7_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU8_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU9_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU10_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU11_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU12_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU13_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU14_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU15_KIN_M"

    '    '2006/02/14
    '    SSQL = SSQL & ",G_MEIMAST.FURI_DATE_M"
    '    'SSQL = SSQL & ",G_SCHMAST.FURI_DATE_S"

    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_CODE_G"
    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME01_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME02_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME03_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME04_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME05_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME06_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME07_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME08_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME09_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME10_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME11_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME12_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME13_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME14_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME15_H"

    '    'SSQL = SSQL & ",SEITOMASTVIEW.NENDO_O"
    '    'SSQL = SSQL & ",SEITOMASTVIEW.TUUBAN_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.GAKUNEN_CODE_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.SEITO_KNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.SEITO_NNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.KAMOKU_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.KOUZA_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.MEIGI_KNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.TYOUSI_FLG_O"


    '    SSQL = SSQL & ",TENMAST.KIN_NO_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NO_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NNAME_N "


    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.NENDO_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKUNEN_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.TUUBAN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU1_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU2_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU3_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU4_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU5_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU6_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU7_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU8_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU9_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU10_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU11_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU12_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU13_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU14_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU15_KIN_M, 0)"

    '    '2006/02/14
    '    'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_DATE_S, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.FURI_DATE_M, 0)"

    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_CODE_G, 0)"
    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME01_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME02_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME03_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME04_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME05_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME06_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME07_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME08_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME09_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME10_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME11_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME12_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME13_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME14_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME15_H, 0)"

    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.NENDO_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.TUUBAN_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.GAKUNEN_CODE_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_KNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_NNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.KAMOKU_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.KOUZA_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.MEIGI_KNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.TYOUSI_FLG_O, 0)"

    '    SSQL = SSQL & ", NVL(TENMAST.KIN_NO_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NO_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

    '    SSQL = SSQL & " FROM "
    '    SSQL = SSQL & "  KZFMAST.G_MEIMAST"
    '    'SSQL = SSQL & " ,KZFMAST.G_SCHMAST"
    '    SSQL = SSQL & " ,KZFMAST.GAKMAST1"
    '    SSQL = SSQL & " ,KZFMAST.HIMOMAST"
    '    SSQL = SSQL & " ,KZFMAST.SEITOMASTVIEW"
    '    SSQL = SSQL & " ,KZFMAST.TENMAST"

    '    SSQL = SSQL & " WHERE "
    '    '2006/02/14
    '    'SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S  "
    '    'SSQL = SSQL & " AND G_MEIMAST.FURI_KBN_M     = G_SCHMAST.FURI_KBN_S  "
    '    'SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    = G_SCHMAST.FURI_DATE_S  "

    '    'SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = HIMOMAST.GAKKOU_CODE_H(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = HIMOMAST.GAKUNEN_CODE_H(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.HIMOKU_ID_M    = HIMOMAST.HIMOKU_ID_H(+)  "
    '    SSQL = SSQL & " AND SUBSTR(G_MEIMAST.SEIKYU_TUKI_M,5,2)  = HIMOMAST.TUKI_NO_H(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMASTVIEW.GAKKOU_CODE_O(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMASTVIEW.NENDO_O(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMASTVIEW.TUUBAN_O(+)  "
    '    SSQL = SSQL & " AND '04'                     = SEITOMASTVIEW.TUKI_NO_O(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N(+) "
    '    SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N(+) "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  =" & "'" & STR学校コード & "'"
    '    '2006/10/13 請求金額が0円のデータは出力しないように修正
    '    SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

    '    If chk振替結果一覧合算出力.Checked = True And STR初振日 <> "" Then
    '        '再振日の入力で合算出力の場合
    '        '入力した再振日の明細は全て対象だが
    '        '取得した初振日の明細は振替済のものが対象
    '        SSQL = SSQL & " AND ((G_MEIMAST.FURI_DATE_M <= '" & STR_FURIKAE_DATE(1) & "' AND G_MEIMAST.SEIKYU_TAISYOU_M = '" & STR対象年月 & "' AND G_MEIMAST.FURI_KBN_M =  '1')"
    '        SSQL = SSQL & " OR (G_MEIMAST.FURI_DATE_M = '" & STR初振日 & "'"
    '        SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M = 0)"
    '        SSQL = SSQL & " OR (G_MEIMAST.FURI_DATE_M = '" & STR初振日 & "'"
    '        SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <> 0"
    '        SSQL = SSQL & " AND G_MEIMAST.SAIFURI_SUMI_M = 0))"
    '    Else
    '        '初振日または再振日の入力で合算出力なしの場合
    '        SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    =" & "'" & STR_FURIKAE_DATE(1) & "'"
    '    End If

    '    SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M    =  '0' OR G_MEIMAST.FURI_KBN_M =  '1') "

    '    '        SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M = 0 "
    '    SSQL = SSQL & " ORDER BY "
    '    Select Case STR帳票ソート順
    '        Case "0"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case "1"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case Else
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            'SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '明細マスタには名義人しか無い 2006/10/11
    '            SSQL = SSQL & "    ,SEITOMASTVIEW.SEITO_KNAME_O   ASC" '2007/02/14 生徒名カナ順に出力
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"

    '    End Select

    '    PFUC_SQLQuery_振替結果 = SSQL

    '    'Debug.WriteLine("SSQL=" & SSQL)

    'End Function
    'Private Function PFUC_SQLQuery_振替不能結果() As String
    '    Dim SSQL As String

    '    PFUC_SQLQuery_振替不能結果 = ""


    '    SSQL = "SELECT "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.GAKUNEN_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU1_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU2_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU3_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU4_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU5_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU6_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU7_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU8_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU9_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU10_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU11_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU12_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU13_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU14_KIN_M"
    '    SSQL = SSQL & ",G_MEIMAST.HIMOKU15_KIN_M"

    '    '2006/02/14
    '    'SSQL = SSQL & ",G_SCHMAST.FURI_DATE_S"
    '    SSQL = SSQL & ",G_MEIMAST.FURI_DATE_M"

    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_CODE_G"
    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME01_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME02_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME03_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME04_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME05_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME06_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME07_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME08_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME09_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME10_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME11_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME12_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME13_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME14_H"
    '    SSQL = SSQL & ",HIMOMAST.HIMOKU_NAME15_H"

    '    SSQL = SSQL & ",SEITOMASTVIEW.NENDO_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.TUUBAN_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.GAKUNEN_CODE_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.SEITO_KNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.SEITO_NNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.KAMOKU_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.KOUZA_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.MEIGI_KNAME_O"
    '    SSQL = SSQL & ",SEITOMASTVIEW.TYOUSI_FLG_O"


    '    SSQL = SSQL & ",TENMAST.KIN_NO_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NO_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NNAME_N "


    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKUNEN_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU1_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU2_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU3_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU4_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU5_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU6_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU7_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU8_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU9_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU10_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU11_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU12_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU13_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU14_KIN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.HIMOKU15_KIN_M, 0)"

    '    '2006/02/14
    '    'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_DATE_S, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.FURI_DATE_M, 0)"

    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_CODE_G, 0)"
    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME01_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME02_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME03_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME04_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME05_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME06_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME07_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME08_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME09_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME10_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME11_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME12_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME13_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME14_H, 0)"
    '    SSQL = SSQL & ", NVL(HIMOMAST.HIMOKU_NAME15_H, 0)"

    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.NENDO_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.TUUBAN_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.GAKUNEN_CODE_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_KNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.SEITO_NNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.KAMOKU_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.KOUZA_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.MEIGI_KNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMASTVIEW.TYOUSI_FLG_O, 0)"

    '    SSQL = SSQL & ", NVL(TENMAST.KIN_NO_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NO_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

    '    SSQL = SSQL & " FROM "
    '    SSQL = SSQL & "  KZFMAST.G_MEIMAST"
    '    '2006/02/14
    '    'SSQL = SSQL & " ,KZFMAST.G_SCHMAST"
    '    SSQL = SSQL & " ,KZFMAST.GAKMAST1"
    '    SSQL = SSQL & " ,KZFMAST.HIMOMAST"
    '    SSQL = SSQL & " ,KZFMAST.SEITOMASTVIEW"
    '    SSQL = SSQL & " ,KZFMAST.TENMAST"

    '    SSQL = SSQL & " WHERE "
    '    '2006/02/14
    '    'SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = G_SCHMAST.GAKKOU_CODE_S  "
    '    'SSQL = SSQL & " AND G_MEIMAST.FURI_KBN_M     = G_SCHMAST.FURI_KBN_S  "
    '    'SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M    = G_SCHMAST.FURI_DATE_S  "

    '    'SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
    '    SSQL = SSQL & "  G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = HIMOMAST.GAKKOU_CODE_H(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = HIMOMAST.GAKUNEN_CODE_H(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.HIMOKU_ID_M    = HIMOMAST.HIMOKU_ID_H(+)  "
    '    SSQL = SSQL & " AND SUBSTR(G_MEIMAST.SEIKYU_TUKI_M,5,2)  = HIMOMAST.TUKI_NO_H(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMASTVIEW.GAKKOU_CODE_O(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMASTVIEW.NENDO_O(+)  "
    '    SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMASTVIEW.TUUBAN_O(+)  "
    '    SSQL = SSQL & " AND '04'                     = SEITOMASTVIEW.TUKI_NO_O(+)  "

    '    SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N(+) "
    '    SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N(+) "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  =" & "'" & STR学校コード & "'"
    '    '2006/10/13 請求金額が0円のデータは出力しないように修正
    '    SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

    '    If chk振替不能明細合算出力.Checked = True And STR初振日 <> "" Then
    '        '再振日の入力で合算出力の場合
    '        SSQL = SSQL & " AND (G_MEIMAST.FURI_DATE_M = '" & txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text & "'"
    '        SSQL = SSQL & "  OR  G_MEIMAST.FURI_DATE_M = '" & STR初振日 & "'" & ") "
    '    Else
    '        '初振日または再振日の入力で合算出力なしの場合
    '        SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text & "'"
    '    End If

    '    SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M    =  '0' OR G_MEIMAST.FURI_KBN_M =  '1') "

    '    '2006/10/12 合計欄に振替済件数・金額も出力するように変更
    '    'SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <> 0 "

    '    SSQL = SSQL & " ORDER BY "
    '    Select Case STR帳票ソート順
    '        Case "0"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case "1"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case Else
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            'SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '明細マスタには名義人しか無い 2006/10/11
    '            SSQL = SSQL & "    ,SEITOMASTVIEW.SEITO_KNAME_O   ASC" '2007/02/14 生徒名カナ順に出力
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '    End Select

    '    PFUC_SQLQuery_振替不能結果 = SSQL

    '    'Debug.WriteLine("SSQL=" & SSQL)

    'End Function
    'Private Function PFUC_SQLQuery_店別集計() As String

    '    Dim SSQL As String = ""

    '    PFUC_SQLQuery_店別集計 = ""

    '    SSQL = SSQL & " SELECT "
    '    SSQL = SSQL & "NVL(GAKMAST1.GAKKOU_NNAME_G,0), "
    '    SSQL = SSQL & "NVL(TENMAST.SIT_NNAME_N,0), "
    '    SSQL = SSQL & "NVL(G_MEIMAST.GAKKOU_CODE_M,0), "
    '    SSQL = SSQL & "NVL(G_MEIMAST.TKIN_NO_M,0), "
    '    SSQL = SSQL & "NVL(G_MEIMAST.FURIKETU_CODE_M,0), "
    '    SSQL = SSQL & "NVL(G_MEIMAST.FURI_DATE_M,0), "
    '    SSQL = SSQL & "NVL(G_MEIMAST.SEIKYU_KIN_M,0), "
    '    SSQL = SSQL & "GAKMAST1.GAKUNEN_CODE_G, "
    '    SSQL = SSQL & "NVL(GAKMAST2.TSIT_NO_T,0), "
    '    SSQL = SSQL & "NVL(TENMAST_1.SIT_NNAME_N,0), "
    '    SSQL = SSQL & "GAKMAST2.TKIN_NO_T, "
    '    SSQL = SSQL & "G_MEIMAST.TSIT_NO_M "
    '    SSQL = SSQL & "FROM   "
    '    SSQL = SSQL & "KZFMAST.G_MEIMAST G_MEIMAST, "
    '    SSQL = SSQL & "KZFMAST.TENMAST TENMAST, "
    '    SSQL = SSQL & "KZFMAST.GAKMAST1 GAKMAST1, "
    '    SSQL = SSQL & "KZFMAST.GAKMAST2 GAKMAST2, "
    '    SSQL = SSQL & "KZFMAST.TENMAST TENMAST_1 "
    '    SSQL = SSQL & "WHERE  "
    '    SSQL = SSQL & "((G_MEIMAST.TKIN_NO_M=TENMAST.KIN_NO_N) AND (G_MEIMAST.TSIT_NO_M=TENMAST.SIT_NO_N)) AND "
    '    SSQL = SSQL & "(G_MEIMAST.GAKKOU_CODE_M=GAKMAST1.GAKKOU_CODE_G) AND (GAKMAST1.GAKKOU_CODE_G=GAKMAST2.GAKKOU_CODE_T) AND "
    '    SSQL = SSQL & "((GAKMAST2.TSIT_NO_T=TENMAST_1.SIT_NO_N (+)) AND (GAKMAST2.TKIN_NO_T=TENMAST_1.KIN_NO_N (+))) AND "
    '    SSQL = SSQL & "GAKMAST1.GAKUNEN_CODE_G=1 "
    '    If STR学校コード.Trim <> "9999999999" Then
    '        '指定学校コード
    '        SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  ='" & Trim(STR学校コード) & "'"
    '    End If
    '    '振替日
    '    SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"

    '    '契約金融機関
    '    '2006/10/12　自金庫データ以外をその他欄に集計して印字するように変更
    '    'SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M = '" & STR自金庫コード_INI & "'"

    '    SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M    =  '0' OR G_MEIMAST.FURI_KBN_M =  '1') "
    '    SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

    '    SSQL = SSQL & "ORDER BY "
    '    SSQL = SSQL & "G_MEIMAST.GAKKOU_CODE_M ASC , G_MEIMAST.FURI_DATE_M ASC"


    '    PFUC_SQLQuery_店別集計 = SSQL

    '    'Debug.WriteLine("SSQL=" & SSQL)

    'End Function
    'Private Function PFUC_SQLQuery_未納のお知らせ() As String
    '    Dim SSQL As String

    '    PFUC_SQLQuery_未納のお知らせ = ""


    '    SSQL = "SELECT "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
    '    SSQL = SSQL & ",G_MEIMAST.TKAMOKU_M"
    '    SSQL = SSQL & ",G_MEIMAST.TKOUZA_M"
    '    SSQL = SSQL & ",G_MEIMAST.TMEIGI_KNM_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_TUKI_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"

    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

    '    SSQL = SSQL & ",SEITOMAST.SEITO_KNAME_O "
    '    SSQL = SSQL & ",SEITOMAST.SEITO_NNAME_O "

    '    SSQL = SSQL & ",TENMAST.KIN_NNAME_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NNAME_N "


    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.TKAMOKU_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.TKOUZA_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.TMEIGI_KNM_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_TUKI_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"

    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

    '    SSQL = SSQL & ", NVL(SEITOMAST.SEITO_KNAME_O, 0)"
    '    SSQL = SSQL & ", NVL(SEITOMAST.SEITO_NNAME_O, 0)"

    '    SSQL = SSQL & ", NVL(TENMAST.KIN_NNAME_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

    '    SSQL = SSQL & " FROM "
    '    SSQL = SSQL & "  KZFMAST.G_MEIMAST"
    '    SSQL = SSQL & " ,KZFMAST.GAKMAST1"
    '    SSQL = SSQL & " ,KZFMAST.SEITOMAST"
    '    SSQL = SSQL & " ,KZFMAST.TENMAST"
    '    ''SSQL = SSQL & " ,KZFMAST.G_SCHMAST"

    '    SSQL = SSQL & " WHERE "
    '    SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMAST.GAKKOU_CODE_O  "
    '    SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMAST.NENDO_O  "
    '    SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMAST.TUUBAN_O  "
    '    SSQL = SSQL & " AND '04'                     = SEITOMAST.TUKI_NO_O  "

    '    SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N "
    '    SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N "

    '    Select Case Trim(STR学校コード)
    '        Case Is <> "9999999999"
    '            '指定学校コード
    '            SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  ='" & Trim(STR学校コード) & "'"
    '    End Select

    '    '振替日
    '    SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"

    '    '契約金融機関
    '    '他行分も出力 2006/10/04
    '    'SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M = '" & STR自金庫コード_INI & "'"
    '    SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M    =  '0' OR G_MEIMAST.FURI_KBN_M =  '1') "

    '    SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <>0 "
    '    SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

    '    SSQL = SSQL & " ORDER BY "
    '    Select Case STR帳票ソート順
    '        Case "0"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case "1"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case Else
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            'SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '明細マスタには名義人しか無い 2006/10/05
    '            SSQL = SSQL & "    ,SEITOMAST.SEITO_KNAME_O   ASC" '2007/02/14　生徒カナ名順で出力
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '    End Select

    '    PFUC_SQLQuery_未納のお知らせ = SSQL

    '    'Debug.WriteLine("SSQL=" & SSQL)

    'End Function
    'Private Function PFUC_SQLQuery_入金伝票() As String
    '    Dim SSQL As String

    '    PFUC_SQLQuery_入金伝票 = ""


    '    SSQL = "SELECT "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.CLASS_CODE_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEITO_NO_M"
    '    SSQL = SSQL & ",G_MEIMAST.TUUBAN_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_TAISYOU_M"
    '    SSQL = SSQL & ",G_MEIMAST.SEIKYU_KIN_M"
    '    'SSQL = SSQL & ",G_MEIMAST.FURI_KBN_M"
    '    'SSQL = SSQL & ",G_MEIMAST.G_MEIMAST.FURIKETU_CODE_M"
    '    'SSQL = SSQL & ",G_MEIMAST.TKIN_NO_M"
    '    'SSQL = SSQL & ",G_MEIMAST.TSIT_NO_M"

    '    SSQL = SSQL & ",GAKMAST1.GAKKOU_NNAME_G"

    '    SSQL = SSQL & ",GAKMAST2.KAMOKU_T"
    '    SSQL = SSQL & ",GAKMAST2.KTEKIYOU_T"

    '    SSQL = SSQL & ",SEITOMAST.SEITO_NNAME_O "

    '    SSQL = SSQL & ",TENMAST.KIN_NNAME_N "
    '    SSQL = SSQL & ",TENMAST.SIT_NNAME_N "

    '    'SSQL = SSQL & ", NVL(G_SCHMAST.GAKKOU_CODE_S, 0)"
    '    'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_KBN_S, 0)"
    '    'SSQL = SSQL & ", NVL(G_SCHMAST.FURI_DATE_S, 0)"

    '    SSQL = SSQL & ", NVL(G_MEIMAST.GAKKOU_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.CLASS_CODE_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEITO_NO_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.TUUBAN_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_TAISYOU_M, 0)"
    '    SSQL = SSQL & ", NVL(G_MEIMAST.SEIKYU_KIN_M, 0)"
    '    'SSQL = SSQL & ", NVL(G_MEIMAST.FURI_KBN_M, 0)"
    '    'SSQL = SSQL & ", NVL(G_MEIMAST.FURIKETU_CODE_M, 0)"

    '    SSQL = SSQL & ", NVL(GAKMAST1.GAKKOU_NNAME_G, 0)"

    '    SSQL = SSQL & ", NVL(GAKMAST2.KAMOKU_T, 0)"
    '    SSQL = SSQL & ", NVL(GAKMAST2.KTEKIYOU_T, 0)"

    '    SSQL = SSQL & ", NVL(SEITOMAST.SEITO_NNAME_O, 0)"

    '    SSQL = SSQL & ", NVL(TENMAST.KIN_NNAME_N, 0)"
    '    SSQL = SSQL & ", NVL(TENMAST.SIT_NNAME_N, 0)"

    '    SSQL = SSQL & " FROM "
    '    SSQL = SSQL & "  KZFMAST.G_MEIMAST"
    '    SSQL = SSQL & " ,KZFMAST.GAKMAST1"
    '    SSQL = SSQL & " ,KZFMAST.GAKMAST2"
    '    SSQL = SSQL & " ,KZFMAST.SEITOMAST"
    '    SSQL = SSQL & " ,KZFMAST.TENMAST"

    '    SSQL = SSQL & " WHERE "
    '    SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M  = GAKMAST1.GAKKOU_CODE_G  "
    '    SSQL = SSQL & " AND G_MEIMAST.GAKUNEN_CODE_M = GAKMAST1.GAKUNEN_CODE_G  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = GAKMAST2.GAKKOU_CODE_T  "

    '    SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  = SEITOMAST.GAKKOU_CODE_O  "
    '    SSQL = SSQL & " AND G_MEIMAST.NENDO_M        = SEITOMAST.NENDO_O  "
    '    SSQL = SSQL & " AND G_MEIMAST.TUUBAN_M       = SEITOMAST.TUUBAN_O  "
    '    SSQL = SSQL & " AND '04'                     = SEITOMAST.TUKI_NO_O  "

    '    SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M      = TENMAST.KIN_NO_N "
    '    SSQL = SSQL & " AND G_MEIMAST.TSIT_NO_M      = TENMAST.SIT_NO_N "

    '    Select Case Trim(STR学校コード)
    '        Case Is <> "9999999999"
    '            '指定学校コード
    '            SSQL = SSQL & " AND G_MEIMAST.GAKKOU_CODE_M  ='" & Trim(STR学校コード) & "'"
    '    End Select
    '    '振替日
    '    SSQL = SSQL & " AND G_MEIMAST.FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"
    '    '振替区分=0,1
    '    SSQL = SSQL & " AND (G_MEIMAST.FURI_KBN_M    =  '0' OR G_MEIMAST.FURI_KBN_M =  '1'　) "

    '    '契約金融機関
    '    SSQL = SSQL & " AND G_MEIMAST.TKIN_NO_M='" & STR_JIKINKO_CODE & "'"

    '    SSQL = SSQL & " AND G_MEIMAST.FURIKETU_CODE_M <>0 "
    '    '2006/10/13 請求金額が0円のデータは出力しないように修正
    '    SSQL = SSQL & " AND G_MEIMAST.SEIKYU_KIN_M  > 0 "

    '    SSQL = SSQL & " ORDER BY "
    '    Select Case STR帳票ソート順
    '        Case "0"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.CLASS_CODE_M     ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.SEITO_NO_M       ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case "1"
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '        Case Else
    '            SSQL = SSQL & "     G_MEIMAST.GAKKOU_CODE_M    ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.GAKUNEN_CODE_M   ASC"
    '            'SSQL = SSQL & "    ,G_MEIMAST.TMEIGI_KNM_M   ASC" '明細マスタには名義人しか無い 2006/10/11
    '            SSQL = SSQL & "    ,SEITOMAST.SEITO_KNAME_O   ASC" '2007/02/14 生徒カナ名順に出力
    '            SSQL = SSQL & "    ,G_MEIMAST.NENDO_M          ASC"
    '            SSQL = SSQL & "    ,G_MEIMAST.TUUBAN_M         ASC"
    '    End Select

    '    PFUC_SQLQuery_入金伝票 = SSQL

    '    'Debug.WriteLine("SSQL=" & SSQL)

    'End Function
    'Private Function PFUC_SQLQuery_収納報告書() As String
    '    '2006/10/11 帳票の出力順を指定するため、JYOKENではなくSQLを指定するように変更
    '    Dim SSQL As String = ""

    '    PFUC_SQLQuery_収納報告書 = ""


    '    SSQL = SSQL & " SELECT "
    '    SSQL = SSQL & " GAKMAST1.GAKKOU_NNAME_G, "
    '    SSQL = SSQL & " TENMAST.KIN_NNAME_N, "
    '    SSQL = SSQL & " TENMAST.SIT_NNAME_N, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME01_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME02_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME03_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME04_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME05_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME06_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO01_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO02_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO03_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO04_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO05_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO06_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA01_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA02_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA03_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA04_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA05_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA06_H, "
    '    SSQL = SSQL & " GAKMAST2.TKIN_NO_T, "
    '    SSQL = SSQL & " GAKMAST2.TSIT_NO_T, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME07_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO07_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA07_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME08_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO08_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA08_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME09_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO09_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA09_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME10_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO10_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA10_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME11_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO11_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA11_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME12_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO12_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA12_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME13_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO13_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA13_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME14_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO14_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA14_H, "
    '    SSQL = SSQL & " HIMOMAST.HIMOKU_NAME15_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_TENPO15_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KOUZA15_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU01_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU02_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU03_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU04_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU05_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU06_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU07_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU08_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU09_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU10_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU11_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU12_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU13_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU14_H, "
    '    SSQL = SSQL & " HIMOMAST.KESSAI_KAMOKU15_H, "
    '    SSQL = SSQL & " G_MEIMAST.GAKUNEN_CODE_M, "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU1_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU2_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU3_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU4_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU5_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU6_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU7_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU8_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU9_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU10_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU11_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU12_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU13_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU14_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.HIMOKU15_KIN_M, "
    '    SSQL = SSQL & " G_MEIMAST.FURI_DATE_M, "
    '    SSQL = SSQL & " G_MEIMAST.FURIKETU_CODE_M, "
    '    SSQL = SSQL & " G_MEIMAST.SEIKYU_KIN_M, "
    '    SSQL = SSQL & " GAKMAST1.GAKUNEN_NAME_G, "
    '    SSQL = SSQL & " G_MEIMAST.SEIKYU_TUKI_M "
    '    SSQL = SSQL & " FROM   "
    '    SSQL = SSQL & " KZFMAST.G_MEIMAST G_MEIMAST, "
    '    SSQL = SSQL & " KZFMAST.GAKMAST2 GAKMAST2, "
    '    SSQL = SSQL & " KZFMAST.HIMOMAST HIMOMAST, "
    '    SSQL = SSQL & " KZFMAST.GAKMAST1 GAKMAST1, "
    '    SSQL = SSQL & " KZFMAST.TENMAST TENMAST"
    '    SSQL = SSQL & " WHERE  "
    '    SSQL = SSQL & " (G_MEIMAST.GAKKOU_CODE_M=GAKMAST2.GAKKOU_CODE_T (+)) AND "
    '    SSQL = SSQL & " (((G_MEIMAST.GAKKOU_CODE_M=HIMOMAST.GAKKOU_CODE_H (+)) AND "
    '    SSQL = SSQL & " (G_MEIMAST.GAKUNEN_CODE_M=HIMOMAST.GAKUNEN_CODE_H (+))) AND "
    '    SSQL = SSQL & " (G_MEIMAST.HIMOKU_ID_M=HIMOMAST.HIMOKU_ID_H (+))) AND "
    '    SSQL = SSQL & " ((G_MEIMAST.GAKKOU_CODE_M=GAKMAST1.GAKKOU_CODE_G (+)) AND "
    '    SSQL = SSQL & " (G_MEIMAST.GAKUNEN_CODE_M=GAKMAST1.GAKUNEN_CODE_G (+))) AND "
    '    SSQL = SSQL & " ((GAKMAST2.TKIN_NO_T=TENMAST.KIN_NO_N (+)) AND "
    '    SSQL = SSQL & " (GAKMAST2.TSIT_NO_T=TENMAST.SIT_NO_N (+))) AND "
    '    Select Case Trim(STR学校コード)
    '        Case Is <> "9999999999"
    '            SSQL = SSQL & " GAKMAST2.GAKKOU_CODE_T= '" & Trim(STR学校コード) & "'AND "
    '        Case Else
    '    End Select

    '    SSQL = SSQL & " G_MEIMAST.FURI_DATE_M= '" & STR_FURIKAE_DATE(1) & "' AND "
    '    SSQL = SSQL & " (G_MEIMAST.FURI_KBN_M= '0' OR G_MEIMAST.FURI_KBN_M= '1') AND "

    '    SSQL = SSQL & " HIMOMAST.TUKI_NO_H= SUBSTR(G_MEIMAST.SEIKYU_TUKI_M,5,2)  "

    '    SSQL = SSQL & " ORDER BY "
    '    SSQL = SSQL & " G_MEIMAST.GAKKOU_CODE_M, "
    '    SSQL = SSQL & " G_MEIMAST.GAKUNEN_CODE_M"

    '    PFUC_SQLQuery_収納報告書 = SSQL

    'End Function

#End Region
End Class
