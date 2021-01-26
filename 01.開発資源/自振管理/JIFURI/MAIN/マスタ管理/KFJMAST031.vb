Imports CASTCommon
Imports System.Text
Public Class KFJMAST031

#Region " Windows フォーム デザイナで生成されたコード "


#End Region
#Region " 変数"
    Private noClose As Boolean
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAST031", "スケジュール再作成画面")
    '2011/06/16 標準版修正 タイトル修正 ------------------
    Private Const msgTitle As String = "スケジュール再作成画面(KFJMAST031)"
    'Private Const msgTitle As String = "スケジュール再作成画面画面(KFJMAST031)"
    Private CLS As ClsSchduleMaintenanceClass

    Public TORIS_CODE As String '取引先主コード
    Public TORIF_CODE As String '取引先副コード
    Public KFURI_DATE As String '契約振替日
    Public FURI_DATE As String  '振替日
    Public FURI_DATE_After As String '変更後振替日
    Private MainDB As MyOracle
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)
#End Region
#Region " ロード"
    Private Sub KFJMAST031_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = TORIS_CODE & TORIF_CODE
            LW.FuriDate = FURI_DATE
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            '休日情報の蓄積
            Call CLS.SetKyuzituInformation()
            'SCHMAST項目名の蓄積
            Call CLS.SetSchMastInformation()
            Me.TopMost = True
            txtFuriDateY.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 実行"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        Dim Ret As Integer = 0
        Dim OkFlg As Boolean
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If

            If CheckSCHMAST() = False Then
                Exit Sub
            End If
            CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            'トランザクション開始
            Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Begin)

            If MessageBox.Show(MSG0005I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If
            CLS.SCH.FURI_DATE = FURI_DATE
            Ret = CLS.GET_SELECT_TORIMAST(Nothing, _
                               TORIS_CODE, TORIF_CODE, ClsSchduleMaintenanceClass.OPT.OptionNothing)

            'スケジュールマスタから該当既存レコードを削除

            If CLS.DELETE_SCHMAST(False) = False Then
                MessageBox.Show(String.Format(MSG0027E, "スケジュールマスタ", "削除"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                Return
            End If

            '登録行為の実行
            CLS.SCH.FURI_DATE = FURI_DATE_After
            CLS.SCH.KFURI_DATE = KFURI_DATE
            If CLS.INSERT_NEW_SCHMAST(0) = True Then

                If CLS.TR(0).SFURI_FLG = 1 Then      '再振のスケジュールも作り変え

                    Dim wrkTorifCode As String = CLS.TR(0).TORIF_CODE
                    Dim wrkSfuriFlg As Integer = CLS.TR(0).SFURI_FLG
                    Dim wrkFuriDate As String = CLS.SCH.FURI_DATE
                    CLS.TR(0).TORIF_CODE = CLS.TR(0).SFURI_FCODE
                    CLS.TR(0).SFURI_FLG = 0
                    CLS.SCH.FURI_DATE = CLS.SCH.KSAIFURI_DATE

                    If CLS.INSERT_NEW_SCHMAST(0) = True Then

                        CLS.TR(0).TORIF_CODE = wrkTorifCode
                        CLS.TR(0).SFURI_FLG = wrkSfuriFlg
                        CLS.SCH.FURI_DATE = wrkFuriDate
                    Else
                        CLS.TR(0).TORIF_CODE = wrkTorifCode
                        CLS.TR(0).SFURI_FLG = wrkSfuriFlg
                        CLS.SCH.FURI_DATE = wrkFuriDate

                        MessageBox.Show(String.Format(MSG0002E, "登録"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                        Return
                    End If
                End If
            Else
                MessageBox.Show(String.Format(MSG0002E, "登録"), msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                Return
            End If
            '2011/07/04 標準版修正 メッセージにはOKボタンのみ表示 ------------------START
            MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            'MessageBox.Show(MSG0004I, msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information)
            '2011/07/04 標準版修正 メッセージにはOKボタンのみ表示 ------------------END

            gstrFURI_DATE = FURI_DATE_After
            noClose = True
            OkFlg = True
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)", "失敗", ex.ToString)
        Finally
            If OkFlg Then
                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Commit)
            Else
                Ret = GCom.DBExecuteProcess(MenteCommon.clsCommon.enDB.DB_Rollback)
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try
    End Sub

#End Region
#Region " クローズ"
    Private Sub KFJMENU010_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If noClose = False Then
            e.Cancel = True
        End If
    End Sub
    Private Sub CmdBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            gstrFURI_DATE = ""
            noClose = True
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            Me.Close()
            Me.Dispose()

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try

    End Sub

#End Region
    '2011/06/16 標準版修正 0埋めを行う ------------------START
#Region " イベント"
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
             Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception

        End Try
    End Sub
#End Region
    '2011/06/16 標準版修正 0埋めを行う ------------------END

#Region " 関数"
    Public Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :更新ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2010/02/05
        'Update         :
        '============================================================================
        Try
            fn_check_text = False
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
            FURI_DATE_After = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

            If FURI_DATE_After = FURI_DATE Then
                MessageBox.Show(String.Format(MSG0318W, TORIS_CODE, TORIF_CODE, FURI_DATE.Substring(0, 4) & _
                                               "年" & FURI_DATE.Substring(4, 2) & "月" & FURI_DATE.Substring(6, 2) & "日"), _
                                               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            If Not GCom.CheckDateModule(FURI_DATE_After, "", 0) Then
                MessageBox.Show(MSG0292W.Replace("{0}", "振替日"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        Finally

        End Try
        fn_check_text = True
    End Function
    Public Function fn_check_text(ByVal TORIS As TextBox, ByVal TORIF As TextBox, _
                                  ByVal FURI_Y As TextBox, ByVal FURI_M As TextBox, ByVal FURI_D As TextBox) As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :更新ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2010/02/05
        'Update         :
        '============================================================================
        Try
            fn_check_text = False
            '取引先主コード必須チェック
            If TORIS.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIS.Focus()
                Return False
            End If
            '取引先副コード必須チェック
            If TORIF.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIF.Focus()
                Return False
            End If
            '年必須チェック
            If FURI_Y.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_Y.Focus()
                Return False
            End If
            '月必須チェック
            If FURI_M.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_M.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(FURI_M.Text.Trim) < 1 OrElse GCom.NzInt(FURI_M.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_M.Focus()
                Return False
            End If
            '日付必須チェック
            If FURI_D.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_D.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(FURI_D.Text.Trim) < 1 OrElse GCom.NzInt(FURI_D.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_D.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE As String = FURI_Y.Text & "/" & FURI_M.Text & "/" & FURI_D.Text
            If Information.IsDate(WORK_DATE) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                FURI_Y.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        Finally

        End Try
        fn_check_text = True
    End Function
    Public Function fn_check_Table(ByVal TORIS As TextBox, ByVal TORIF As TextBox, _
                                   ByVal FURI_Y As TextBox, ByVal FURI_M As TextBox, ByVal FURI_D As TextBox, _
                                   Optional ByVal Iraisyo As Boolean = False) As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :更新ボタンを押下時にマスタ相関チェック
        'Return         :True=OK,False=NG
        'Create         :2010/02/05
        'Update         :
        '============================================================================
        fn_check_Table = False
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            MainDB = New MyOracle
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            Dim strBAITAI_CODE As String

            '取引先情報取得
            SQL.Append("SELECT * FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(Trim(TORIS.Text)))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(Trim(TORIF.Text)))
            If OraReader.DataReader(SQL) = True Then
                strBAITAI_CODE = GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T"))
                OraReader.Close()
            Else
                '取引先なし
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIS.Focus()
                OraReader.Close()
                Return False
            End If

            '媒体コードチェック
            If strBAITAI_CODE = "07" Then '学校
                MessageBox.Show(String.Format(MSG0039I, "変更"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIS.Focus()
                Return False
            End If

            If Iraisyo AndAlso strBAITAI_CODE <> "04" Then '依頼書
                MessageBox.Show(MSG0108W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIS.Focus()
                Return False
            End If

            TORIS_CODE = TORIS.Text
            TORIF_CODE = TORIF.Text
            FURI_DATE = FURI_Y.Text + FURI_M.Text + FURI_D.Text

            '自分が再振用の取引先マスタか判断
            If CLS.CHECK_SAIFURI_SELF(TORIS_CODE, TORIF_CODE) = False Then
                MessageBox.Show(MSG0283W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TORIS.Focus()
                Return False
            End If

            'スケジュールマスタに対象のスケジュールが存在するかチェックする
            SQL = New StringBuilder(128)
            SQL.Append("SELECT * ")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE FSYORI_KBN_S ='1'")
            SQL.Append(" AND TORIS_CODE_T = " & SQ(TORIS.Text))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(TORIF.Text))
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            SQL.Append(" AND FURI_DATE_S = " & SQ(FURI_DATE))

            If OraReader.DataReader(SQL) = True Then
                If GCom.NzStr(OraReader.Reader.Item("UKETUKE_FLG_S")) <> "0" OrElse _
                   GCom.NzStr(OraReader.Reader.Item("TOUROKU_FLG_S")) <> "0" OrElse _
                   GCom.NzStr(OraReader.Reader.Item("TYUUDAN_FLG_S")) <> "0" OrElse _
                   GCom.NzStr(OraReader.Reader.Item("NIPPO_FLG_S")) <> "0" Then
                    MessageBox.Show(String.Format(MSG0286W, GCom.NzStr(OraReader.Reader.Item("UKETUKE_FLG_S")), _
                                                  GCom.NzStr(OraReader.Reader.Item("TOUROKU_FLG_S")), _
                                                  GCom.NzStr(OraReader.Reader.Item("NIPPO_FLG_S")), _
                                                  GCom.NzStr(OraReader.Reader.Item("TYUUDAN_FLG_S"))), _
                                                  msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    TORIS.Focus()
                    OraReader.Close()
                    Return False
                Else
                    KFURI_DATE = GCom.NzStr(OraReader.Reader.Item("KFURI_DATE_S"))
                    OraReader.Close()
                End If
            Else
                'スケジュールなし
                MessageBox.Show(MSG0095W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                OraReader.Close()
                txtFuriDateY.Focus()
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If MainDB IsNot Nothing Then MainDB.Close()
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try
        fn_check_Table = True
    End Function

    Public Function CheckSCHMAST() As Boolean
        Try
            If CLS.SEARCH_NEW_SCHMAST(TORIS_CODE, TORIF_CODE, FURI_DATE_After) = False Then
                MessageBox.Show(String.Format(MSG0319W, TORIS_CODE, TORIF_CODE, FURI_DATE_After.Substring(0, 4) & _
                                               "年" & FURI_DATE_After.Substring(4, 2) & "月" & FURI_DATE_After.Substring(6, 2) & "日"), _
                                               msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If
            Return True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        End Try
    End Function
#End Region
End Class
