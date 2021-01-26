Imports clsFUSION.clsMain
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Imports System.Data.OracleClient
Public Class KFJMAIN080
    Inherits System.Windows.Forms.Form
    Private clsFUSION As New clsFUSION.clsMain()


    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MainLOG As New CASTCommon.BatchLOG("KFJMAIN080", "再振データ作成画面")
    Private Const msgTitle As String = "再振データ作成画面(KFJMAIN080)"
    Private Const errMsg As String = "例外が発生しました。ログを確認のうえ、保守要員に連絡してください。"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private Const ThisModuleName As String = "KFKMAST080.vb"

    '*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
    Private sfuriformat As String
    '*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

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
    Private Sub KFJMAIN042_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'システム日付とユーザ名を表示
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Call GCom.SetMonitorTopArea(Label2, Label4, lblUser, lblDate)
            '休日マスタ取り込み
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
            '--------------------------------
            '委託者名リストボックスの設定
            '--------------------------------
            '取引先コンボボックス設定(Msg????W)
            Dim Jyoken As String = " AND SFURI_FLG_T = '1'"         '再振契約あり
            '*** Str Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
            'Jyoken &= " AND FMT_KBN_T IN('00','04','05')"                     '再振対象フォーマット
            sfuriformat = CASTCommon.GetFSKJIni("COMMON", "SFURI_FORMAT")
            If sfuriformat = "err" Or sfuriformat = "" then  ' iniファイル定義なし
                sfuriformat = ""
                Jyoken &= " AND FMT_KBN_T IN('00','04','05')"                     '再振対象フォーマット
            Else
                sfuriformat = sfuriformat.Replace(" ", "")
                sfuriformat = sfuriformat.Replace(vbTab, "")
                Dim work As String = sfuriformat.Replace(",", "','")
                Jyoken &= " AND FMT_KBN_T IN('00','04','05','" & work & "')" '再振対象フォーマット
            End If
            '*** End Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
            Jyoken &= " AND BAITAI_CODE_T NOT IN('07')"                 '再振対象媒体
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
           MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region
#Region " 更新"
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        '=====================================================================================
        'NAME           :btnAction_Click
        'Parameter      :
        'Description    :作成
        'Return         :
        'Create         :2009/10/07
        'Update         :
        '=====================================================================================
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)開始", "成功", "")
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            strSFURI_DATE = txtSFuriDateY.Text & txtSFuriDateM.Text & txtSFuriDateD.Text
            strToris_Code = txtTorisCode.Text
            strTorif_Code = txtTorifCode.Text
            LW.ToriCode = strToris_Code & strTorif_Code
            LW.FuriDate = strFURI_DATE

            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If
            If MessageBox.Show(MSG0023I.Replace("{0}", "再振データ作成"), msgTitle, _
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            MainDB = New CASTCommon.MyOracle
            MainDB.BeginTrans()

            Dim jobid As String
            Dim para As String

            'ジョブマスタに登録
            jobid = "J080"                      '..\Batch\再振データ作成\
            'パラメータ(取引先コード振替日,再振日)
            para = strToris_Code & strTorif_Code & "," & strFURI_DATE & "," & strSFURI_DATE

            '#########################
            'job検索
            '#########################
            Dim iRet As Integer
            iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            If iRet = 1 Then    'ジョブ登録済
                MainDB.Rollback()
                MessageBox.Show(MSG0221W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            ElseIf iRet = -1 Then 'ジョブ検索失敗
                MainDB.Rollback()
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
               MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブマスタ検索失敗 パラメータ:" & para)
                Return
            End If

            '#########################
            'job登録
            '#########################
            If MainLOG.InsertJOBMAST(jobid, LW.UserID, para, MainDB) = False Then 'ジョブ登録失敗
                MainDB.Rollback()
                MessageBox.Show(MSG0005E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", "ジョブ登録失敗 パラメータ:" & para)
                Return
            End If

            MainDB.Commit()

            MessageBox.Show(MSG0021I.Replace("{0}", "再振データ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            If MainDB IsNot Nothing Then
                MainDB.Rollback()
            End If
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)", "失敗", ex.ToString)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(更新)終了", "成功", "")
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
        'Description    :実行ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/10
        'Update         :
        '============================================================================
        fn_check_text = False
        Try
            '取引先主コード必須チェック
            If txtTorisCode.Text.Trim = "" Then
                MessageBox.Show(MSG0057W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If
            '取引先副コード必須チェック
            If txtTorifCode.Text.Trim = "" Then
                MessageBox.Show(MSG0059W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorifCode.Focus()
                Return False
            End If
            '振替日
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

            '再振日
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
            fn_check_text = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try
    End Function

    Function fn_check_Table() As Boolean
        '============================================================================
        'NAME           :fn_check_Table
        'Parameter      :
        'Description    :ボタンを押下時にマスタの相関チェックを実行(条件のチェック)
        'Return         :True=OK,False=NG
        'Create         :2009/10/07
        'Update         :
        '============================================================================
        fn_check_Table = False
        MainDB = New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim OraReader2 As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '取引先マスタ検索(再振指定の登録がされているかチェック)
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            SQL.Append("SELECT * FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strToris_Code))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(strTorif_Code))
            If OraReader.DataReader(SQL) = True Then
                strSFURI_FLG = GCom.NzStr(OraReader.Reader.Item("SFURI_FLG_T"))
                strSFURI_FCODE = GCom.NzStr(OraReader.Reader.Item("SFURI_FCODE_T"))
                strITAKU_KANRI = GCom.NzStr(OraReader.Reader.Item("ITAKU_KANRI_CODE_T"))
                strFMT_KBN = GCom.NzStr(OraReader.Reader.Item("FMT_KBN_T"))
                strBAITAI = GCom.NzStr(OraReader.Reader.Item("BAITAI_CODE_T"))
                strMULTI_KBN = GCom.NzStr(OraReader.Reader.Item("MULTI_KBN_T"))
                OraReader.Close()
            Else
                '取引先なし
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                OraReader.Close()
                Return False
            End If


            If strSFURI_FLG <> "1" Then
                '再振非対象
                MessageBox.Show(MSG0088W, _
                              msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            ElseIf strSFURI_FCODE = "" Then
                '再振副コード未設定
                MessageBox.Show(MSG0331W, _
                                      msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Else
                Select Case strFMT_KBN
                    Case "00", "04", "05"
                    Case Else
                        '*** Str Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                        'フォーマット異常
                        'MessageBox.Show(MSG0332W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        'Return False
                        Dim errflg As Boolean = True
                        If sfuriformat <> "" then  ' iniファイル定義あり
                            Dim format() As String = sfuriformat.Split(","c)
                            For Each value As String In format
                                If value.Trim = strFMT_KBN Then
                                    errflg = False
                                    Exit For
                                End IF
                            Next
                        End If

                        If errflg = True Then
                            'フォーマット異常
                            MessageBox.Show(MSG0332W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return False
                        End If
                        '*** End Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                End Select
                Select Case strBAITAI
                    Case "07"
                        '媒体コード異常
                        MessageBox.Show(MSG0333W, _
                                 msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                End Select
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'スケジュールの検索（初振のスケジュール）
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            SQL = New StringBuilder(128)
            SQL.Append("SELECT *")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(strToris_Code))
            SQL.Append(" AND TORIF_CODE_S = " & SQ(strTorif_Code))
            SQL.Append(" AND FURI_DATE_S = " & SQ(strFURI_DATE))
            SQL.Append(" AND KSAIFURI_DATE_S = " & SQ(strSFURI_DATE))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            If OraReader.DataReader(SQL) = True Then

                intMOTIKOMI_SEQ = GCom.NzInt(OraReader.Reader.Item("MOTIKOMI_SEQ_S"))
                If GCom.NzStr(OraReader.Reader.Item("HENKAN_FLG_S")) <> "1" Then
                    '返還未処理あり
                    MessageBox.Show(MSG0090W, _
                                      msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
                If strMULTI_KBN = "0" Then
                    If GCom.NzInt(OraReader.GetInt("FUNOU_KEN_S")) = 0 Then
                        '不能対象無し
                        MessageBox.Show(MSG0091W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        OraReader.Close()
                        Return False
                    End If
                End If
                OraReader.Close()
            Else
                'スケジュールなし
                MessageBox.Show(MSG0089W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                OraReader.Close()
                Return False
            End If

            If strMULTI_KBN = "1" Then  'マルチ指定の場合、同じ持ち込みシーケンスのスケジュール全て検索
                Dim lngFUNOU_KEN As Long = 0
                SQL = New StringBuilder(128)
                SQL.Append(" SELECT * ")
                SQL.Append(" FROM TORIMAST,SCHMAST")
                SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
                SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                SQL.Append(" AND FURI_DATE_S = " & SQ(strFURI_DATE))
                SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(strITAKU_KANRI))
                SQL.Append(" AND MOTIKOMI_SEQ_S = " & intMOTIKOMI_SEQ)
                SQL.Append(" AND TYUUDAN_FLG_S = '0'")
                If OraReader.DataReader(SQL) = True Then
                    While OraReader.EOF = False
                        If GCom.NzStr(OraReader.GetString("HENKAN_FLG_S")) <> "1" Then
                            '返還未処理あり
                            MessageBox.Show(MSG0090W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return False
                        End If
                        lngFUNOU_KEN += GCom.NzInt(OraReader.Reader.Item("FUNOU_KEN_S"))
                        OraReader.NextRead()
                    End While
                    OraReader.Close()
                Else
                    'スケジュール検索失敗
                    MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    OraReader.Close()
                    Return False
                End If

                If lngFUNOU_KEN = 0 Then
                    '不能対象無し
                    MessageBox.Show(MSG0091W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    OraReader.Close()
                    Return False
                End If
            End If
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'スケジュールの検索（再振のスケジュール）
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            OraReader2 = New MyOracleReader(MainDB)

            SQL = New StringBuilder(128)
            SQL.Append(" SELECT * ")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND FURI_DATE_S = " & SQ(strFURI_DATE))
            SQL.Append(" AND KSAIFURI_DATE_S = " & SQ(strSFURI_DATE))
            SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(strITAKU_KANRI))
            SQL.Append(" AND MOTIKOMI_SEQ_S = " & intMOTIKOMI_SEQ)
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT * FROM SCHMAST,TORIMAST")
                    SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
                    SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                    SQL.Append(" AND TORIS_CODE_S = " & SQ(GCom.NzStr(OraReader.GetString("TORIS_CODE_T"))))
                    SQL.Append(" AND TORIF_CODE_S = " & SQ(GCom.NzStr(OraReader.GetString("SFURI_FCODE_T"))))
                    SQL.Append(" AND FURI_DATE_S = " & SQ(strSFURI_DATE))
                    If OraReader2.DataReader(SQL) = False Then
                        '再振スケジュールなし
                        MessageBox.Show(MSG0089W & vbCrLf & _
                                        "取引先コード：" & strToris_Code & "-" & OraReader.GetString("SFURI_FCODE_T") & vbCrLf & _
                                        "振替日：" & strSFURI_DATE.Substring(0, 4) & "年" & strSFURI_DATE.Substring(4, 2) & "月" & strSFURI_DATE.Substring(6, 2) & "日", _
                                        msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        OraReader2.Close()
                        Return False
                    Else
                        If GCom.NzStr(OraReader2.GetString("TOUROKU_FLG_S")) = "1" Then
                            '登録処理済
                            MessageBox.Show(MSG0061W & vbCrLf & _
                                            "取引先コード ： " & GCom.NzStr(OraReader2.GetString("TORIS_CODE_T")) & " - " & GCom.NzStr(OraReader2.GetString("TORIF_CODE_T")), _
                                              msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            OraReader2.Close()
                            Return False
                        End If
                        If strFMT_KBN <> GCom.NzStr(OraReader2.GetString("FMT_KBN_T")) Then
                            'フォーマット区分一致チェック
                            MessageBox.Show(MSG0334W & vbCrLf & _
                                         "取引先コード ： " & OraReader2.GetString("TORIS_CODE_S") & " - " & OraReader2.GetString("SFURI_FCODE_T"), _
                                              msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
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
            fn_check_Table = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraReader2 IsNot Nothing Then OraReader2.Close()
            If MainDB IsNot Nothing Then MainDB.Close()
        End Try
    End Function
#End Region
#Region " イベント"
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '--------------------------------
        '選択カナで始まる委託者名を取得
        '--------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                '取引先コンボボックス設定
                Dim Jyoken As String = " AND SFURI_FLG_T = '1'"   '再振契約あり
                '*** Str Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                'Jyoken &= " AND FMT_KBN_T IN('00','04','05')"               '再振対象フォーマット
                If sfuriformat = "" then  ' iniファイル定義なし
                    Jyoken &= " AND FMT_KBN_T IN('00','04','05')"                     '再振対象フォーマット
                Else
                    Dim work As String = sfuriformat.Replace(",", "','")
                    Jyoken &= " AND FMT_KBN_T IN('00','04','05','" & work & "')" '再振対象フォーマット
                End If
                '*** End Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

                Jyoken &= " AND BAITAI_CODE_T NOT IN('07')"           '再振対象媒体
                If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                    MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                cmbToriName.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コンボボックス設定)", "失敗", ex.ToString)
        End Try
    End Sub

    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        '-------------------------------------------
        '取引先コードテキストボックスに取引先コード設定
        '-------------------------------------------
        Try
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(取引先コード取得)", "失敗", ex.ToString)
        End Try

    End Sub
    'ゼロパディング
    Private Sub FMT_NzNumberString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating, _
                    txtTorisCode.Validating, txtTorifCode.Validating, _
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
