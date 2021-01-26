Imports System.Globalization
Imports System.Text
Imports System.IO
Imports CASTCommon
Public Class KFJPRNT030
    Private MainLOG As New CASTCommon.BatchLOG("KFJPRNT030", "スケジュール表印刷画面")
    Private Const msgTitle As String = "スケジュール表印刷画面(KFJPRNT030)"
    Private Const errMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
    Private strFuriDate As String

    Private Sub KFJPRNT030_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '#####################################
        'ログの書込に必要な情報の取得
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時に対象日にシステム日付を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strSysDate As String
            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            txtFuriDateY.Text = strSysDate.Substring(0, 4)
            txtFuriDateM.Text = strSysDate.Substring(4, 2)
            txtFuriDateD.Text = strSysDate.Substring(6, 2)

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'コンボボックスを表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim Msg As String = ""
            '2010/12/24 信組対応 テキスト名変更
            Dim Txt As String = ""
            If GetFSKJIni("COMMON", "CENTER") = "0" Then
                Txt = "KFJPRNT030_印刷区分(信組).TXT"
            Else
                Txt = "KFJPRNT030_印刷区分.TXT"
            End If

            Select Case GCom.SetComboBox(cmbPrintKbn, Txt, True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "持込区分", Txt), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Msg = "持込区分設定ファイルなし。ファイル:" & Txt
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "持込区分"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(コンボボックス設定)", "失敗", Msg)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#Region " 印刷"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :印刷ボタン
        'Return         :
        'Create         :2009/09/15
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            Dim PrintName As String

            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            If rdbDATE.Checked Then
                strFuriDate = txtFuriDateY.Text + txtFuriDateM.Text + txtFuriDateD.Text
                PrintName = "振替日指定スケジュール表"
            Else
                strFuriDate = txtFuriDateY.Text + txtFuriDateM.Text
                PrintName = "月間スケジュール表"
                LW.FuriDate = strFuriDate + "01"
            End If
            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, PrintName), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            Dim param As String
            Dim nRet As Integer
            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：ログイン名,対象年月日,印刷帳票(1:月間スケジュール表 2:振替日指定スケジュール表),印刷区分
            If rdbDATE.Checked Then '振替日指定
                param = GCom.GetUserID & "," & strFuriDate & ",2," & GCom.GetComboBox(cmbPrintKbn)
            Else    '対象年月指定
                param = GCom.GetUserID & "," & strFuriDate & ",1," & GCom.GetComboBox(cmbPrintKbn)
            End If

            nRet = ExeRepo.ExecReport("KFJP033.EXE", param)

            '戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(String.Format(MSG0014I, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(String.Format(MSG0004E, PrintName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try

    End Sub
#End Region

#Region " 終了"
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
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
#End Region
#Region " 関数"
    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :印刷ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/17
        'Update         :
        '============================================================================
        fn_check_text = False
        Try
            '年必須チェック(MSG0018W)
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

            '振替日指定の場合のみチェックを実行
            If rdbDATE.Checked Then
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
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try

        fn_check_text = True

    End Function
    Private Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :印刷ボタンを押下時にマスタ相関チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/15
        'Update         :
        '============================================================================
        fn_check_Table = False
        Dim MainDB As New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        fn_check_Table = False
        Try
            Dim lngDataCNT As Long = 0

            SQL.Append("SELECT")
            SQL.Append(" COUNT(*) COUNTER")
            SQL.Append(" FROM SCHMAST,TORIMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='1'")
            SQL.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            Select Case GCom.GetComboBox(cmbPrintKbn)
                Case 0  '金庫持込
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                Case 1  '伝送
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    '2012/06/30 標準版　WEB伝送対応
                    'SQL.Append(" AND BAITAI_CODE_S = '00'")
                    SQL.Append(" AND BAITAI_CODE_S IN ('00','10')")
                Case 2  '媒体
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    'SQL.Append(" AND BAITAI_CODE_S IN ('01','05', '06')") 'FD3.5/MT/CMT
                    SQL.Append(" AND BAITAI_CODE_S IN ('01','05','06','11','12','13','14','15')") 'FD3.5/MT/CMT/DVD/その他追加
                Case 3  '依頼書
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    SQL.Append(" AND BAITAI_CODE_S = '04'")
                Case 4  '伝票
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    SQL.Append(" AND BAITAI_CODE_S = '09'")
                Case 5  '学校
                    SQL.Append(" AND MOTIKOMI_KBN_S = '0'")
                    SQL.Append(" AND BAITAI_CODE_S = '07'")
                Case 6  'センター直接持込
                    SQL.Append(" AND MOTIKOMI_KBN_S = '1'")
            End Select
            If rdbDATE.Checked Then '振替日指定
                SQL.Append(" AND FURI_DATE_S = " & SQ(strFuriDate))
            Else '対象月指定
                Dim strKIJYUN_DATE1 As String = strFuriDate & "01"
                Dim strKIJYUN_DATE2 As String = strFuriDate & "31"
                SQL.Append("  AND (FURI_DATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR HAISIN_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR KESSAI_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR TESUU_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" OR HENKAN_YDATE_S BETWEEN " & SQ(strKIJYUN_DATE1) & " AND " & SQ(strKIJYUN_DATE2))
                SQL.Append(" )")
            End If
            If OraReader.DataReader(SQL) = True Then
                lngDataCNT = GCom.NzLong(OraReader.GetInt64("COUNTER"))
            Else
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", "マスタ検索失敗")
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtFuriDateY.Focus()
                Return False
            End If
            If lngDataCNT = 0 Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
            fn_check_Table = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function

#End Region
#Region " イベント"
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
             Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub
    'ラジオボタン制御
    Private Sub rdbMonth_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbMonth.CheckedChanged
        Try
            txtFuriDateD.Visible = False
            Label10.Visible = False
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ラジオボタン制御)", "失敗", ex.ToString)
        End Try
    End Sub
    'ラジオボタン制御
    Private Sub rdbDATE_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rdbDATE.CheckedChanged
        Try
            txtFuriDateD.Visible = True
            Label10.Visible = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ラジオボタン制御)", "失敗", ex.ToString)
        End Try
    End Sub
#End Region
End Class