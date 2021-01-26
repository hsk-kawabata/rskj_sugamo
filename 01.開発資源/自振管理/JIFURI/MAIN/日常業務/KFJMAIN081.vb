Imports clsFUSION.clsMain
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Imports System.Data.OracleClient
Imports System.Windows.Forms

Public Class KFJMAIN081
    Inherits System.Windows.Forms.Form

    Private clsFUSION As New clsFUSION.clsMain()

    Private GCom As MenteCommon.clsCommon
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN081", "再振データ作成画面")
    Private SaifuriCheckLOG As New CASTCommon.BatchLOG("KFJMAIN081", "再振処理チェックログ", "SAIFURI")

    Private Const msgTitle As String = "再振データ作成画面(KFJMAIN081)"
    Private Const errMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure
    Private LW As LogWrite

    Private sfuriformat As String
    Private strITAKU_KANRI As String
    Private strSFURI_FLG As String
    Private strSFURI_FCODE As String
    Private strFMT_KBN As String
    Private strBAITAI As String
    Private intMOTIKOMI_SEQ As Integer
    Private strMULTI_KBN As String
    Private strToris_Code As String
    Private strTorif_Code As String
    Private strFURI_DATE As String
    Private strSFURI_DATE As String
    Private strTAKOU_BAITAI_CODE As String

#Region " ロード"
    Private Sub KFJMAIN081_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '#####################################
            'ログの書込に必要な情報の取得
            '-------------------------------------
            'ログインIDを取得
            '-------------------------------------
            Dim Args As String() = Environment.GetCommandLineArgs
            GCom = New MenteCommon.clsCommon

            '-------------------------------------
            'ログインID、日付設定
            '-------------------------------------
            If Args.Length <= 1 Then
                GCom.GetUserID = ""
            Else
                GCom.GetUserID = Args(1).Trim
            End If
            GCom.GetSysDate = Date.Now

            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label2, Label4, lblUser, lblDate)

            '--------------------------------
            ' 休日マスタ取り込み
            '--------------------------------
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)
            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)終了", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            Dim strSysDate As String
            Dim strGetdate As String = ""

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            bRet = GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)
            txtFuriDateY.Text = strGetdate.Substring(0, 4)
            txtFuriDateM.Text = strGetdate.Substring(4, 2)
            txtFuriDateD.Text = strGetdate.Substring(6, 2)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub

#End Region

#Region " ボタン "

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :作成
        'Return         :
        'Create         :2016/10/03
        'Update         :
        '=====================================================================================

        Dim MainDB As CASTCommon.MyOracle = Nothing
        Dim SchReader As CASTCommon.MyOracleReader = Nothing

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "開始", "")
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If CheckText() = False Then
                Exit Sub
            End If

            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            strSFURI_DATE = txtSFuriDateY.Text & txtSFuriDateM.Text & txtSFuriDateD.Text
            LW.FuriDate = strFURI_DATE

            MainDB = New CASTCommon.MyOracle
            MainDB.BeginTrans()

            Dim SchSQL As New StringBuilder(128)
            SchSQL.Length = 0
            SchSQL.Append("SELECT")
            SchSQL.Append("     TORIS_CODE_T")
            SchSQL.Append("   , TORIF_CODE_T")
            SchSQL.Append("   , KSAIFURI_DATE_S")
            SchSQL.Append("   , ITAKU_KANRI_CODE_T")
            SchSQL.Append("   , FMT_KBN_T")
            SchSQL.Append("   , MULTI_KBN_T")
            SchSQL.Append("   , SFURI_FCODE_T")
            SchSQL.Append("   , TYUUDAN_FLG_S")
            SchSQL.Append(" FROM")
            SchSQL.Append("     TORIMAST")
            SchSQL.Append("   , SCHMAST")
            SchSQL.Append(" WHERE")
            SchSQL.Append("     FURI_DATE_S     = '" & strFURI_DATE & "'")
            SchSQL.Append(" AND TOUROKU_FLG_S   = '1'")
            SchSQL.Append(" AND FILE_SEQ_S      = 1")
            SchSQL.Append(" AND TORIS_CODE_S    = TORIS_CODE_T")
            SchSQL.Append(" AND TORIF_CODE_S    = TORIF_CODE_T")
            SchSQL.Append(" AND BAITAI_CODE_T   NOT IN ('07')")
            SchSQL.Append(" AND SFURI_FLG_T     = '1'")
            SchSQL.Append(" ORDER BY")
            SchSQL.Append("     TORIS_CODE_T")
            SchSQL.Append("   , TORIF_CODE_T")
            SchSQL.Append("   , FMT_KBN_T")
            SchSQL.Append("   , BAITAI_CODE_T")

            SchReader = New CASTCommon.MyOracleReader(MainDB)
            If SchReader.DataReader(SchSQL) = True Then
                '--------------------------------
                ' 【パラメータ設定】
                '--------------------------------
                Dim jobid As String = String.Empty
                Dim para As String = String.Empty
                jobid = "J080"                      '..\Batch\再振データ作成\

                While SchReader.EOF = False
                    strToris_Code = GCom.NzStr(SchReader.GetString("TORIS_CODE_T"))
                    strTorif_Code = GCom.NzStr(SchReader.GetString("TORIF_CODE_T"))
                    strITAKU_KANRI = GCom.NzStr(SchReader.GetString("ITAKU_KANRI_CODE_T"))
                    strFMT_KBN = GCom.NzStr(SchReader.GetString("FMT_KBN_T"))
                    strMULTI_KBN = GCom.NzStr(SchReader.GetString("MULTI_KBN_T"))
                    strSFURI_FCODE = GCom.NzStr(SchReader.GetString("SFURI_FCODE_T"))
                    LW.ToriCode = strToris_Code & strTorif_Code

                    '-------------------------------------------------
                    ' 【再振日の正当性チェック】
                    '-------------------------------------------------
                    Dim CheckFlg As Boolean = True
                    If GCom.NzStr(SchReader.GetString("KSAIFURI_DATE_S")) <> strSFURI_DATE Then
                        SaifuriCheckLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振チェック", "再振日相違:" & GCom.NzStr(SchReader.GetString("KSAIFURI_DATE_S")))
                        CheckFlg = False
                    End If

                    '-------------------------------------------------
                    ' 【中断フラグチェック】
                    '-------------------------------------------------
                    If GCom.NzStr(SchReader.GetString("TYUUDAN_FLG_S")) <> "0" Then
                        SaifuriCheckLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振チェック", "中断フラグ = 1")
                        CheckFlg = False
                    End If

                    '---------------------------------------------
                    ' 【マスタチェック】
                    '---------------------------------------------
                    If CheckFlg = True Then
                        If CheckTable(MainDB) = True Then
                            para = strToris_Code & strTorif_Code & "," & strFURI_DATE & "," & strSFURI_DATE

                            '-----------------------------------------
                            ' 【JOB検索】
                            '-----------------------------------------
                            Dim iRet As Integer = MainLOG.SearchJOBMAST(jobid, para, MainDB)
                            If iRet = 1 Then    'ジョブ登録済
                                MainDB.Rollback()
                                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Return
                            ElseIf iRet = -1 Then 'ジョブ検索失敗
                                MainDB.Rollback()
                                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "失敗", "ジョブマスタ検索失敗 パラメータ:" & para)
                                Return
                            End If

                            '-----------------------------------------
                            ' 【JOB登録】
                            '-----------------------------------------
                            If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then 'ジョブ登録失敗
                                MainDB.Rollback()
                                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "失敗", "ジョブ登録失敗 パラメータ:" & para)
                                Return
                            End If
                        End If
                    End If
                End While

                SchReader.Close()
                SchReader = Nothing

                MainDB.Commit()
                MessageBox.Show(MSG0021I.Replace("{0}", "再振データ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return

            Else
                SchReader.Close()
                SchReader = Nothing
                '========================================================
                ' ここにメッセージ追加（再振対象０件）
                '========================================================
                MessageBox.Show(MSG0089W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

        Catch ex As Exception
            If MainDB IsNot Nothing Then
                MainDB.Rollback()
            End If
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "失敗", ex.Message)
        Finally
            If Not SchReader Is Nothing Then
                SchReader.Close()
                SchReader = Nothing
            End If

            If Not MainDB Is Nothing Then
                MainDB.Close()
                MainDB = Nothing
            End If
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "実行", "終了", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        Try

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "終了", "開始", "")
            Me.Close()
            Me.Dispose()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "終了", "成功", "")

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "終了", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "終了", "終了", "")
        End Try
    End Sub

#End Region

#Region " 関数"

    Private Function CheckText() As Boolean
        '============================================================================
        'NAME           :CheckText
        'Parameter      :
        'Description    :実行ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2016/10/03
        'Update         :
        '============================================================================

        Try

            '--------------------------------
            '振替日のチェック
            '--------------------------------
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
            If GCom.NzInt(txtFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtFuriDateM.Text.Trim) > 12 Then '(MSG0022W)
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

            '--------------------------------
            '再振日のチェック
            '--------------------------------
            '年必須チェック
            If txtSFuriDateY.Text.Trim = "" Then
                MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSFuriDateY.Focus()
                Return False
            End If
            '月必須チェック
            If txtSFuriDateM.Text.Trim = "" Then
                MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSFuriDateM.Focus()
                Return False
            End If
            '月範囲チェック
            If GCom.NzInt(txtSFuriDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtSFuriDateM.Text.Trim) > 12 Then
                MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSFuriDateM.Focus()
                Return False
            End If
            '日付必須チェック
            If txtSFuriDateD.Text.Trim = "" Then
                MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSFuriDateD.Focus()
                Return False
            End If
            '日付範囲チェック
            If GCom.NzInt(txtSFuriDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtSFuriDateD.Text.Trim) > 31 Then
                MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSFuriDateD.Focus()
                Return False
            End If

            '日付整合性チェック
            Dim WORK_DATE2 As String = txtSFuriDateY.Text & "/" & txtSFuriDateM.Text & "/" & txtSFuriDateD.Text
            If Information.IsDate(WORK_DATE2) = False Then
                MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtSFuriDateY.Focus()
                Return False
            End If

            '日付前後チェック
            If WORK_DATE > WORK_DATE2 Then
                MessageBox.Show(MSG0148W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtFuriDateY.Focus()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try

    End Function

    Private Function CheckTable(ByVal MainDB As CASTCommon.MyOracle) As Boolean
        '============================================================================
        'NAME           :fn_check_Table
        'Parameter      :
        'Description    :ボタンを押下時にマスタの相関チェックを実行(条件のチェック)
        'Return         :True=OK,False=NG
        'Create         :2009/10/07
        'Update         :
        '============================================================================

        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Dim OraReader2 As CASTCommon.MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)

        Try

            '---------------------------------------------
            ' 【取引先マスタ情報チェック】
            '   ※情報は呼び出し元で取得済
            '---------------------------------------------
            If strSFURI_FCODE = "" Then
                '再振副コード未設定
                SaifuriCheckLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振チェック", "再振副コード未設定")
                Return False
            Else
                Select Case strFMT_KBN
                    Case "00", "04", "05"
                    Case Else
                        Dim errflg As Boolean = True
                        If sfuriformat <> "" Then  ' iniファイル定義あり
                            Dim format() As String = sfuriformat.Split(","c)
                            For Each value As String In format
                                If value.Trim = strFMT_KBN Then
                                    errflg = False
                                    Exit For
                                End If
                            Next
                        End If

                        If errflg = True Then
                            'フォーマット異常
                            SaifuriCheckLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振チェック", "再振対象外フォーマット")
                            Return False
                        End If
                End Select
            End If

            '---------------------------------------------
            ' 【スケジュール検索】
            '   初振スケジュール
            '---------------------------------------------
            OraReader = New MyOracleReader(MainDB)
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     MOTIKOMI_SEQ_S")
            SQL.Append("   , HENKAN_FLG_S")
            SQL.Append("   , FUNOU_KEN_S")
            SQL.Append(" FROM")
            SQL.Append("     SCHMAST")
            SQL.Append(" WHERE ")
            SQL.Append("     TORIS_CODE_S    = " & SQ(strToris_Code))
            SQL.Append(" AND TORIF_CODE_S    = " & SQ(strTorif_Code))
            SQL.Append(" AND FURI_DATE_S     = " & SQ(strFURI_DATE))
            SQL.Append(" AND KSAIFURI_DATE_S = " & SQ(strSFURI_DATE))
            SQL.Append(" AND TYUUDAN_FLG_S   = '0'")

            If OraReader.DataReader(SQL) = True Then

                intMOTIKOMI_SEQ = GCom.NzInt(OraReader.Reader.Item("MOTIKOMI_SEQ_S"))

                If GCom.NzStr(OraReader.Reader.Item("HENKAN_FLG_S")) <> "1" Then
                    SaifuriCheckLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振チェック", "返還データ作成未処理")
                    Return False
                End If
                If strMULTI_KBN = "0" Then
                    If GCom.NzInt(OraReader.GetInt("FUNOU_KEN_S")) = 0 Then
                        SaifuriCheckLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振チェック", "不能件数０件")
                        OraReader.Close()
                        Return False
                    End If
                End If
                OraReader.Close()
            Else
                SaifuriCheckLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振チェック", "再振データ作成対象スケジュールなし")
                OraReader.Close()
                Return False
            End If

            '---------------------------------------------
            ' 【マルチ指定先のスケジュール検索】
            '   初振スケジュール
            '---------------------------------------------
            If strMULTI_KBN = "1" Then  'マルチ指定の場合、同じ持ち込みシーケンスのスケジュール全て検索
                SQL.Length = 0
                SQL.Append("SELECT")
                SQL.Append("     HENKAN_FLG_S")
                SQL.Append("   , FUNOU_KEN_S")
                SQL.Append("   , TORIS_CODE_S")
                SQL.Append("   , TORIF_CODE_S")
                SQL.Append(" FROM")
                SQL.Append("     TORIMAST")
                SQL.Append("   , SCHMAST")
                SQL.Append(" WHERE")
                SQL.Append("     TORIS_CODE_S       = TORIS_CODE_T")
                SQL.Append(" AND TORIF_CODE_S       = TORIF_CODE_T")
                SQL.Append(" AND FURI_DATE_S        = " & SQ(strFURI_DATE))
                SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(strITAKU_KANRI))
                SQL.Append(" AND MOTIKOMI_SEQ_S     = " & intMOTIKOMI_SEQ)
                SQL.Append(" AND TYUUDAN_FLG_S      = '0'")

                Dim lngFUNOU_KEN As Long = 0
                If OraReader.DataReader(SQL) = True Then
                    While OraReader.EOF = False
                        If GCom.NzStr(OraReader.GetString("HENKAN_FLG_S")) <> "1" Then
                            '返還未処理あり
                            SaifuriCheckLOG.Write(LW.UserID, _
                                                  GCom.NzStr(OraReader.GetString("TORIS_CODE_S")) & _
                                                  GCom.NzStr(OraReader.GetString("TORIF_CODE_S")), LW.FuriDate, "再振チェック", "返還データ作成未処理")
                            Return False
                        End If
                        lngFUNOU_KEN += GCom.NzInt(OraReader.Reader.Item("FUNOU_KEN_S"))
                        OraReader.NextRead()
                    End While
                    OraReader.Close()
                End If

                If lngFUNOU_KEN = 0 Then
                    '不能対象無し
                    SaifuriCheckLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "再振チェック", "不能件数０件")
                    OraReader.Close()
                    Return False
                End If
            End If

            '---------------------------------------------
            ' 【スケジュール検索】
            '   再振スケジュール
            '---------------------------------------------
            SQL.Length = 0
            SQL.Append("SELECT")
            SQL.Append("     * ")
            SQL.Append(" FROM")
            SQL.Append("     TORIMAST")
            SQL.Append("   , SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     TORIS_CODE_S       = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S       = TORIF_CODE_T")
            SQL.Append(" AND FURI_DATE_S        = " & SQ(strFURI_DATE))
            SQL.Append(" AND KSAIFURI_DATE_S    = " & SQ(strSFURI_DATE))
            SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(strITAKU_KANRI))
            SQL.Append(" AND MOTIKOMI_SEQ_S     = " & intMOTIKOMI_SEQ)
            SQL.Append(" AND TYUUDAN_FLG_S      = '0'")

            If OraReader.DataReader(SQL) = True Then

                OraReader2 = New MyOracleReader(MainDB)
                While OraReader.EOF = False

                    SQL.Length = 0
                    SQL.Append("SELECT")
                    SQL.Append("     * ")
                    SQL.Append(" FROM")
                    SQL.Append("     TORIMAST")
                    SQL.Append("   , SCHMAST")
                    SQL.Append(" WHERE")
                    SQL.Append("     TORIS_CODE_S = TORIS_CODE_T")
                    SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                    SQL.Append(" AND TORIS_CODE_S = " & SQ(GCom.NzStr(OraReader.GetString("TORIS_CODE_T"))))
                    SQL.Append(" AND TORIF_CODE_S = " & SQ(GCom.NzStr(OraReader.GetString("SFURI_FCODE_T"))))
                    SQL.Append(" AND FURI_DATE_S  = " & SQ(strSFURI_DATE))

                    If OraReader2.DataReader(SQL) = False Then
                        SaifuriCheckLOG.Write(LW.UserID, _
                                              strToris_Code & OraReader.GetString("SFURI_FCODE_T"), _
                                              strSFURI_DATE, _
                                              "再振チェック", _
                                              "再振データ作成対象スケジュールなし")
                        OraReader2.Close()
                        Return False
                    Else
                        If GCom.NzStr(OraReader2.GetString("TOUROKU_FLG_S")) = "1" Then
                            SaifuriCheckLOG.Write(LW.UserID, _
                                                  OraReader2.GetString("TORIS_CODE_T") & OraReader2.GetString("TORIF_CODE_T"), _
                                                  strSFURI_DATE, _
                                                  "再振チェック", _
                                                  "落込処理済み")
                            OraReader2.Close()
                            Return False
                        End If
                        If strFMT_KBN <> GCom.NzStr(OraReader2.GetString("FMT_KBN_T")) Then
                            SaifuriCheckLOG.Write(LW.UserID, _
                                                  OraReader2.GetString("TORIS_CODE_S") & OraReader2.GetString("SFURI_FCODE_T"), _
                                                  strSFURI_DATE, _
                                                  "再振チェック", _
                                                  "初・再フォーマット区分一致")
                            OraReader2.Close()
                            Return False
                        End If
                    End If
                    OraReader.NextRead()

                End While
                OraReader.Close()
            Else
                'スケジュール検索失敗
                MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                Return False
            End If

            Return True

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If OraReader2 IsNot Nothing Then
                OraReader2.Close()
                OraReader2 = Nothing
            End If
        End Try
    End Function

#End Region

#Region " イベント"
   
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating, _
                    txtSFuriDateY.Validating, txtSFuriDateM.Validating, txtSFuriDateD.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub

#End Region

End Class
