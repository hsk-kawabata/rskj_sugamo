Imports System.Text
Imports CASTCommon

Public Class KFJKOZA010

    '共通イベントコントロール
    Private CAST As New CASTCommon.Events

    '' 許可文字指定

    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    Private MyOwnerForm As Form

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private Structure strcIni
        Dim JIKINKO_CODE As String            '自金庫コード
    End Structure

    Private ini_info As strcIni

    Private MainLOG As New CASTCommon.BatchLOG("KFJKOZA010", "口座チェック（当日受付分）")
    Private Const msgTitle As String = "口座チェック（当日受付分）(KFJKOZA010)"

    Private Sub KFJKOZA010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '#####################################
            'ログの書込に必要な情報の取得
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
            '#####################################

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label2, Label3, lblUser, lblDate)

            '###################
            ' ini取得
            '###################
            If read_ini() = False Then Return

            '###################
            ' 画面初期化
            '###################
            If set_gamen() = False Then Return

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)例外エラー", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try

    End Sub

    Private Function read_ini() As Boolean

        Try

            ini_info.JIKINKO_CODE = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If ini_info.JIKINKO_CODE = "err" OrElse ini_info.JIKINKO_CODE = "" Then
                MessageBox.Show(String.Format(MSG0001E, "自金庫コード", "COMMON", "KINKOCD"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

        Catch ex As Exception
            Throw
        End Try

        Return True
    End Function

    Private Function set_gamen() As Boolean

        Dim msg As String = ""

        Try

            '一括の設定
            Select Case GCom.SetComboBox(cmbSyubetu, "KFJKOZA010_種別.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "種別（一括）", "KFJKOZA010_種別.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "種別（一括）"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
            End Select

            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
            '初期値にシステム日付を設定
            Me.txtTorokuDateY.Text = System.DateTime.Now.ToString("yyyy")
            Me.txtTorokuDateM.Text = System.DateTime.Now.ToString("MM")
            Me.txtTorokuDateD.Text = System.DateTime.Now.ToString("dd")
            'txtTorokuDateY.Text = ""
            'txtTorokuDateM.Text = ""
            'txtTorokuDateD.Text = ""
            ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

            '個別の設定
            Select Case GCom.SetComboBox(cmbSyubetu_KOBETU, "KFJKOZA010_種別.TXT", True)
                Case 1  'ファイルなし
                    MessageBox.Show(String.Format(MSG0025E, "種別（個別）", "KFJKOZA010_種別.TXT"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                Case 2  '異常
                    MessageBox.Show(String.Format(MSG0026E, "種別（個別）"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
            End Select

            '取引先コンボボックス設定
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            txtFuriDateY.Text = ""
            txtFuriDateM.Text = ""
            txtFuriDateD.Text = ""
            txtMotikomiSEQ.Text = ""

        Catch ex As Exception
            Throw
        End Try

        Return True

    End Function

    Private Sub Close_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
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

    Private Sub RadMode1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadMode1.CheckedChanged
        PanKobetu.Visible = False
        PanIkkatu.Visible = True
    End Sub

    Private Sub RadMode2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadMode2.CheckedChanged
        PanKobetu.Visible = True
        PanIkkatu.Visible = False
    End Sub


    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        Try
            '取引先コードがReadOnlyの場合は処理終了
            If txtTorisCode.ReadOnly Then
                Exit Sub
            End If
            '取引先コンボボックス設定
            If GCom.SelectItakuName(cmbKana.SelectedItem.ToString, cmbToriName, txtTorisCode, txtTorifCode, "1") = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub cmbToriName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbToriName.SelectedIndexChanged
        Try
            '取引先コードがReadOnlyの場合は処理終了
            If txtTorisCode.ReadOnly Then
                Exit Sub
            End If
            If Not cmbToriName.SelectedItem.ToString = Nothing OrElse String.IsNullOrEmpty(cmbToriName.SelectedItem.ToString) Then
                GCom.Set_TORI_CODE(cmbToriName, txtTorisCode, txtTorifCode)
                txtFuriDateY.Focus()
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Dim MainDB As New CASTCommon.MyOracle
        Dim oraReader As CASTCommon.MyOracleReader
        Dim sql As StringBuilder = Nothing
        Dim clsKCheck As New clsKouzaCheck
        Dim iRet As Integer
        Dim clsPrnt As New KFJP041
        Dim KDBinfo(4) As String    '名義人、移管支店、移管口座番号、エラーコード、エラーメッセージ

        Dim bRet As Boolean = True

        Dim tourokuDate As String = ""
        Dim FuriDate As String = ""

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)開始", "成功", "")

            btnAction.Enabled = False
            btnClose.Enabled = False

            sql = New StringBuilder(128)

            '*****************************
            ' 一括か個別かで分岐
            ' 現時点で口座振替のみ2009.10.02
            '*****************************
            If RadMode1.Checked = True Then '一括

                If check_input(1) = False Then Return

                tourokuDate = txtTorokuDateY.Text & txtTorokuDateM.Text & txtTorokuDateD.Text

                Select Case GCom.GetComboBox(cmbSyubetu)
                    Case 91

                        sql.Append(" SELECT ")
                        sql.Append(" TORIS_CODE_K ")
                        sql.Append(" ,TORIF_CODE_K ")
                        sql.Append(" ,FURI_DATE_K ")
                        sql.Append(" ,KEIYAKU_KIN_K ")
                        sql.Append(" ,KEIYAKU_SIT_K ")
                        sql.Append(" ,KEIYAKU_KAMOKU_K ")
                        sql.Append(" ,KEIYAKU_KOUZA_K ")
                        sql.Append(" ,KEIYAKU_KNAME_K ")
                        sql.Append(" ,FURI_CODE_K ")
                        sql.Append(" ,RECORD_NO_K ")
                        sql.Append(" ,KIGYO_CODE_K ")
                        sql.Append(" ,JYUYOUKA_NO_K ")
                        sql.Append(" ,ITAKU_NNAME_T ")
                        sql.Append(" ,TORIMATOME_SIT_T ")
                        sql.Append(" ,SYUBETU_T ")
                        sql.Append(" ,JIFURICHK_KBN_T ")        '自振契約作成区分
                        sql.Append(" FROM ")
                        sql.Append(" TORIMAST ")
                        sql.Append(" ,SCHMAST ")
                        sql.Append(" ,MEIMAST ")
                        sql.Append(" WHERE ")
                        sql.Append(" TORIS_CODE_T = TORIS_CODE_S ")
                        sql.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
                        sql.Append(" AND FURI_DATE_S = FURI_DATE_K ")
                        sql.Append(" AND TORIS_CODE_S = TORIS_CODE_K ")
                        sql.Append(" AND TORIF_CODE_S = TORIF_CODE_K ")
                        sql.Append(" AND TOUROKU_FLG_S = '1'")
                        sql.Append(" AND TYUUDAN_FLG_S = '0'")
                        sql.Append(" AND DATA_KBN_K in ('2','3')")  '国税も
                        sql.Append(" AND TOUROKU_DATE_S = '" & tourokuDate & "'")
                        sql.Append(" ORDER BY TORIS_CODE_K,TORIF_CODE_K,FURI_DATE_K,KEIYAKU_KIN_K,KEIYAKU_SIT_K,RECORD_NO_K")

                    Case 21
                        '総振　未対応
                        Return
                    Case 11
                        '給与　未対応
                        Return
                    Case 12
                        '賞与　未対応
                        Return
                End Select

            Else                            '個別

                If check_input(2) = False Then Return

                FuriDate = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text

                Select Case GCom.GetComboBox(cmbSyubetu_KOBETU)
                    Case 91
                        sql.Append(" SELECT ")
                        sql.Append(" TORIS_CODE_K ")
                        sql.Append(" ,TORIF_CODE_K ")
                        sql.Append(" ,FURI_DATE_K ")
                        sql.Append(" ,KEIYAKU_KIN_K ")
                        sql.Append(" ,KEIYAKU_SIT_K ")
                        sql.Append(" ,KEIYAKU_KAMOKU_K ")
                        sql.Append(" ,KEIYAKU_KOUZA_K ")
                        sql.Append(" ,KEIYAKU_KNAME_K ")
                        sql.Append(" ,FURI_CODE_K ")
                        sql.Append(" ,RECORD_NO_K ")
                        sql.Append(" ,KIGYO_CODE_K ")
                        sql.Append(" ,JYUYOUKA_NO_K ")
                        sql.Append(" ,ITAKU_NNAME_T ")
                        sql.Append(" ,TORIMATOME_SIT_T ")
                        sql.Append(" ,SYUBETU_T ")
                        sql.Append(" ,JIFURICHK_KBN_T ")        '自振契約作成区分
                        sql.Append(" FROM ")
                        sql.Append(" TORIMAST ")
                        sql.Append(" ,SCHMAST ")
                        sql.Append(" ,MEIMAST ")
                        sql.Append(" WHERE ")
                        sql.Append(" TORIS_CODE_T = TORIS_CODE_S ")
                        sql.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
                        sql.Append(" AND FURI_DATE_S = FURI_DATE_K ")
                        sql.Append(" AND TORIS_CODE_S = TORIS_CODE_K ")
                        sql.Append(" AND TORIF_CODE_S = TORIF_CODE_K ")
                        sql.Append(" AND TOUROKU_FLG_S = '1'")
                        sql.Append(" AND TYUUDAN_FLG_S = '0'")
                        sql.Append(" AND DATA_KBN_K in ('2','3')")  '国税も
                        sql.Append(" AND FURI_DATE_S = '" & FuriDate & "'")
                        'sql.Append(" AND MOTIKOMI_SEQ_S = '" & txtMotikomiSEQ.Text & "'")  2010.02.04 自振ははずす
                        sql.Append(" AND TORIS_CODE_T = '" & txtTorisCode.Text & "'")
                        sql.Append(" AND TORIF_CODE_T = '" & txtTorifCode.Text & "'")
                        sql.Append(" ORDER BY TORIS_CODE_K,TORIF_CODE_K,FURI_DATE_K,KEIYAKU_KIN_K,KEIYAKU_SIT_K,RECORD_NO_K")

                    Case 21
                        '総振　未対応
                        Return
                    Case 11
                        '給与　未対応
                        Return
                    Case 12
                        '賞与　未対応
                        Return
                End Select

            End If

            If MessageBox.Show(MSG0015I.Replace("{0}", "口座チェック"), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then Return

            oraReader = New CASTCommon.MyOracleReader(MainDB)

            clsPrnt.CreateCsvFile()
            Dim strCSVFileName As String = clsPrnt.FileName
            Dim PrintFlg As Boolean = False

            If oraReader.DataReader(sql) = True Then
                Do Until oraReader.EOF

                    KDBinfo = New String() {"", "", "", "", ""} '名義人、移管支店、移管口座番号、エラーコード、エラーメッセージ

                    'ログ情報
                    LW.ToriCode = String.Concat(oraReader.GetString("TORIS_CODE_K"), oraReader.GetString("TORIF_CODE_K"))

                    '自行だったらチェック
                    If ini_info.JIKINKO_CODE = oraReader.GetString("KEIYAKU_KIN_K") Then

                        iRet = clsKCheck.KouzaChk(oraReader.GetString("KEIYAKU_SIT_K"), _
                                               oraReader.GetString("KEIYAKU_KAMOKU_K"), _
                                               oraReader.GetString("KEIYAKU_KOUZA_K"), _
                                               oraReader.GetString("FURI_CODE_K"), _
                                               oraReader.GetString("KIGYO_CODE_K"), _
                                               oraReader.GetString("SYUBETU_T"), _
                                               oraReader.GetString("KEIYAKU_KNAME_K"), _
                                               KDBinfo, MainDB)

                        '結果を受けて自振契約対象をテーブルに登録
                        '戻り値 2 4 12 14 自振契約作成対象

                        Dim jikei_sit As String
                        Dim jikei_kamoku As String
                        Dim jikei_kouza As String

                        Select Case iRet
                            Case 2, 4, 12, 14

                                '自振契約作成対象の委託者であれば作成
                                If oraReader.GetString("JIFURICHK_KBN_T") = "1" Then

                                    If iRet = 2 OrElse iRet = 4 Then
                                        jikei_sit = oraReader.GetString("KEIYAKU_SIT_K")
                                        jikei_kamoku = oraReader.GetString("KEIYAKU_KAMOKU_K")
                                        jikei_kouza = oraReader.GetString("KEIYAKU_KOUZA_K")
                                    Else
                                        jikei_sit = KDBinfo(1)
                                        jikei_kamoku = oraReader.GetString("KEIYAKU_KAMOKU_K")
                                        jikei_kouza = KDBinfo(2)
                                    End If

                                    sql = New StringBuilder(512)
                                    sql.Append("")

                                    sql.Append("MERGE INTO STORE_JIFKEIYAKU")
                                    sql.Append(" USING (SELECT")
                                    sql.Append(" '" & jikei_sit & "' KEIYAKU_SIT")
                                    sql.Append(", '" & jikei_kamoku & "' KEIYAKU_KAMOKU")
                                    sql.Append(", '" & jikei_kouza & "' KEIYAKU_KOUZA")
                                    sql.Append(",'" & oraReader.GetString("FURI_CODE_K") & "' FURI_CODE")
                                    sql.Append(", '" & oraReader.GetString("KIGYO_CODE_K") & "' KIGYO_CODE")
                                    sql.Append(" FROM DUAL) WORK ")
                                    sql.Append(" ON (")
                                    sql.Append("     STORE_JIFKEIYAKU.KEIYAKU_SIT_JK = WORK.KEIYAKU_SIT ")
                                    sql.Append(" AND STORE_JIFKEIYAKU.KEIYAKU_KAMOKU_JK = WORK.KEIYAKU_KAMOKU ")
                                    sql.Append(" AND STORE_JIFKEIYAKU.KEIYAKU_KOUZA_JK = WORK.KEIYAKU_KOUZA ")
                                    sql.Append(" AND STORE_JIFKEIYAKU.FURI_CODE_JK = WORK.FURI_CODE ")
                                    sql.Append(" AND STORE_JIFKEIYAKU.KIGYO_CODE_JK = WORK.KIGYO_CODE ")
                                    sql.Append(" )")
                                    'UPDATE句
                                    sql.Append(" WHEN MATCHED THEN")
                                    sql.Append(" UPDATE SET")
                                    sql.Append(" TORIS_CODE_JK = '" & oraReader.GetString("TORIS_CODE_K") & "'")
                                    sql.Append(" ,TORIF_CODE_JK = '" & oraReader.GetString("TORIF_CODE_K") & "'")
                                    sql.Append(" ,KEIYAKU_KNAME_JK = '" & oraReader.GetString("KEIYAKU_KNAME_K") & "'")
                                    sql.Append(" ,FURI_DATE_JK = '" & oraReader.GetString("FURI_DATE_K") & "'")
                                    sql.Append(" ,RECORD_NO_JK = " & oraReader.GetInt64("RECORD_NO_K"))
                                    sql.Append(" ,KOUSIN_DATE_JK = '" & DateTime.Today.ToString("yyyyMMdd") & "'")
                                    'INSERT句
                                    sql.Append(" WHEN NOT MATCHED THEN")
                                    sql.Append(" INSERT ")
                                    sql.Append(" (")
                                    sql.Append("  KEIYAKU_SIT_JK")
                                    sql.Append(" ,KEIYAKU_KAMOKU_JK")
                                    sql.Append(" ,KEIYAKU_KOUZA_JK")
                                    sql.Append(" ,KEIYAKU_KNAME_JK")
                                    sql.Append(" ,FURI_CODE_JK")
                                    sql.Append(" ,KIGYO_CODE_JK")
                                    sql.Append(" ,TORIS_CODE_JK")
                                    sql.Append(" ,TORIF_CODE_JK")
                                    sql.Append(" ,FURI_DATE_JK")
                                    sql.Append(" ,RECORD_NO_JK")
                                    sql.Append(" ,SAKUSEI_DATE_JK")
                                    sql.Append(" ,KOUSIN_DATE_JK")
                                    sql.Append(" )")
                                    sql.Append(" VALUES(")
                                    sql.Append("'" & jikei_sit & "',")
                                    sql.Append("'" & jikei_kamoku & "',")
                                    sql.Append("'" & jikei_kouza & "',")
                                    sql.Append("'" & oraReader.GetString("KEIYAKU_KNAME_K") & "',")
                                    sql.Append("'" & oraReader.GetString("FURI_CODE_K") & "',")
                                    sql.Append("'" & oraReader.GetString("KIGYO_CODE_K") & "',")
                                    sql.Append("'" & oraReader.GetString("TORIS_CODE_K") & "',")
                                    sql.Append("'" & oraReader.GetString("TORIF_CODE_K") & "',")
                                    sql.Append("'" & oraReader.GetString("FURI_DATE_K") & "',")
                                    sql.Append(oraReader.GetInt64("RECORD_NO_K") & ",")
                                    sql.Append("'" & DateTime.Today.ToString("yyyyMMdd") & "',")
                                    sql.Append("'" & DateTime.Today.ToString("yyyyMMdd") & "'")
                                    sql.Append(")")

                                    Dim nRet As Integer = MainDB.ExecuteNonQuery(sql)
                                    If nRet <= 0 Then
                                        bRet = False
                                        MessageBox.Show(String.Format(MSG0027E, "自振契約テーブル", "更新"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)自振契約テーブル登録", "失敗", "支店:" & jikei_sit & "科目:" & jikei_kamoku & "口座番号:" & jikei_kouza)
                                        Exit Do
                                        'エラー処理
                                    End If
                                Else
                                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)スキップ", "成功", "自振契約作成対象外設定")
                                End If

                            Case -1
                                bRet = False
                                Exit Do
                                'エラー処理
                        End Select


                        '結果を受けて口座チェック確認表出力
                        '0:異常なし 1:口座なし 2:自振契約なし  3:名義人名(カナ)相違  4:自振契約無-名義人名(カナ)相違  5:解約済 -1:異常
                        '(移管後) 10:移管済 12:自振契約なし 13:名義人名(カナ)相違 14:自振契約無-名義人名(カナ)相違 15:解約済

                        If iRet <> 0 Then

                            clsPrnt.OutputCsvData(oraReader.GetString("FURI_DATE_K"), True)         '振替日
                            clsPrnt.OutputCsvData(oraReader.GetString("KEIYAKU_SIT_K"), True)       '支店コード
                            clsPrnt.OutputCsvData(GCom.GetBKBRName(ini_info.JIKINKO_CODE, oraReader.GetString("KEIYAKU_SIT_K")), True)   '支店名
                            clsPrnt.OutputCsvData(oraReader.GetString("TORIS_CODE_K"), True)        '取引先主コード
                            clsPrnt.OutputCsvData(oraReader.GetString("TORIF_CODE_K"), True)        '取引先副コード
                            clsPrnt.OutputCsvData(oraReader.GetString("ITAKU_NNAME_T"), True)       '取引先名（漢字）
                            clsPrnt.OutputCsvData(oraReader.GetString("SYUBETU_T"), True)           '種別
                            clsPrnt.OutputCsvData(oraReader.GetString("FURI_CODE_K"), True)         '振替コード
                            clsPrnt.OutputCsvData(oraReader.GetString("KIGYO_CODE_K"), True)        '企業コード
                            clsPrnt.OutputCsvData(oraReader.GetString("KEIYAKU_KAMOKU_K"), True)    '科目
                            clsPrnt.OutputCsvData(oraReader.GetString("KEIYAKU_KOUZA_K"), True)     '口座番号
                            clsPrnt.OutputCsvData(oraReader.GetString("JYUYOUKA_NO_K"), True)       '需要家番号
                            clsPrnt.OutputCsvData(oraReader.GetString("KEIYAKU_KNAME_K"), True)     '契約者名カナ
                            clsPrnt.OutputCsvData(KDBinfo(0), True)                                 'KDB名義人
                            clsPrnt.OutputCsvData(KDBinfo(1), True)                                 '新支店コード
                            clsPrnt.OutputCsvData(KDBinfo(2), True)                                 '新口座番号
                            clsPrnt.OutputCsvData(KDBinfo(4), True)                                 'エラー理由
                            clsPrnt.OutputCsvData("", True, True)                                   '備考

                            PrintFlg = True

                        End If

                    Else
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)スキップ", "成功", "他行データ:" & oraReader.GetString("KEIYAKU_KIN_K"))
                    End If

                    oraReader.NextRead()

                Loop

            Else
                MessageBox.Show(MSG0112W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                If RadMode1.Checked = True Then '一括
                    txtTorokuDateY.Focus()
                Else                            '個別
                    txtFuriDateY.Focus()
                End If
                Return
            End If

            clsPrnt.CloseCsv()

            '口座チェック確認表の印刷
            If bRet = True AndAlso PrintFlg = True Then
                ''印刷バッチ呼び出し
                Dim ExeRepo As New CAstReports.ClsExecute
                Dim param As String
                ExeRepo.SetOwner = Me
                Dim errMsg As String = ""

                'パラメータ設定：ログイン名、ＣＳＶファイル名、取引先コード
                param = GCom.GetUserID & "," & strCSVFileName
                iRet = ExeRepo.ExecReport("KFJP041.EXE", param)

                If iRet <> 0 Then

                    '印刷失敗：戻り値に対応したエラーメッセージを表示する
                    Select Case iRet
                        Case -1
                            MessageBox.Show(String.Format(MSG0226W, "口座チェック確認表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座チェック確認表印刷", "失敗", "エラーコード：" & iRet)
                        Case Else
                            MessageBox.Show(String.Format(MSG0004E, "口座チェック確認表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座チェック確認表印刷", "失敗", "エラーコード：" & iRet)
                    End Select

                    bRet = False
                End If
            End If

            If bRet = True Then
                MessageBox.Show(MSG0016I.Replace("{0}", "口座チェック"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                MainDB.Commit()
            Else
                MessageBox.Show(MSG0236W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                MainDB.Rollback()
            End If

        Catch ex As Exception

            If Not MainDB Is Nothing Then MainDB.Rollback()
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)例外エラー", "失敗", ex.Message)
        Finally

            btnAction.Enabled = True
            btnClose.Enabled = True

            If Not MainDB Is Nothing Then MainDB.Close()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(実行)終了", "成功", "")
        End Try

    End Sub

    Private Function check_input(ByVal kbn As Integer) As Boolean

        Select Case kbn
            Case 1  '一括

                '年必須チェック
                If txtTorokuDateY.Text.Trim = "" Then
                    MessageBox.Show(MSG0018W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorokuDateY.Focus()
                    Return False
                End If
                '月必須チェック
                If txtTorokuDateM.Text.Trim = "" Then
                    MessageBox.Show(MSG0020W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorokuDateM.Focus()
                    Return False
                End If
                '月範囲チェック
                If GCom.NzInt(txtTorokuDateM.Text.Trim) < 1 OrElse GCom.NzInt(txtTorokuDateM.Text.Trim) > 12 Then
                    MessageBox.Show(MSG0022W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorokuDateM.Focus()
                    Return False
                End If

                '日付必須チェック
                If txtTorokuDateD.Text.Trim = "" Then
                    MessageBox.Show(MSG0023W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorokuDateD.Focus()
                    Return False
                End If
                '日付範囲チェック
                If GCom.NzInt(txtTorokuDateD.Text.Trim) < 1 OrElse GCom.NzInt(txtTorokuDateD.Text.Trim) > 31 Then
                    MessageBox.Show(MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorokuDateD.Focus()
                    Return False
                End If
                '日付整合性チェック
                Dim WORK_DATE As String = txtTorokuDateY.Text & "/" & txtTorokuDateM.Text & "/" & txtTorokuDateD.Text
                If Information.IsDate(WORK_DATE) = False Then
                    MessageBox.Show(MSG0027W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtTorokuDateY.Focus()
                    Return False
                End If


            Case 2  '個別

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

                '持込ＳＥＱ必須チェック 91以外
                Select Case GCom.GetComboBox(cmbSyubetu_KOBETU)
                    Case "91"
                    Case Else
                        If txtMotikomiSEQ.Text.Trim = "" Then
                            MessageBox.Show(MSG0115W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtMotikomiSEQ.Focus()
                            Return False
                        End If
                End Select
        End Select

        Return True

    End Function

    Private Sub FURI_DATE_S_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
Handles txtTorisCode.Validating, txtTorifCode.Validating, txtFuriDateY.Validating, _
txtFuriDateM.Validating, txtFuriDateD.Validating, txtTorokuDateY.Validating, txtTorokuDateM.Validating, txtTorokuDateD.Validating

        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力後処置)例外エラー", "失敗", ex.Message)
        End Try


    End Sub

End Class
