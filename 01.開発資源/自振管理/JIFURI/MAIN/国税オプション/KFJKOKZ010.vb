Imports clsFUSION.clsMain
Imports System
Imports System.IO
Imports CASTCommon
Imports System.Text
Imports System.Data.OracleClient
Imports System.Drawing.Printing
Public Class KFJKOKZ010

    Inherits System.Windows.Forms.Form
    Private MainLOG As New CASTCommon.BatchLOG("KFJKOKZ010", "国税特殊帳票印刷画面")
    Private Const msgTitle As String = "国税特殊帳票印刷画面(KFJKOKZ010)"

    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
    Private DenFolder As String
    Private MainDB As CASTCommon.MyOracle   'パブリックデーターベース

    Private strFURI_DATE As String
    Private strTORIS_CODE As String
    Private strTORIF_CODE As String
    Private CAST As New CASTCommon.Events
    Private CASTx01 As New CASTCommon.Events("0-9", CASTCommon.Events.KeyMode.GOOD)

    '印刷用項目
    Private PrnKbn As String         '領収証書印刷区分
    Private PrinterKFJP045 As String 'プリンタ(領収証書用)
    Private PrinterKFJP048 As String 'プリンタ(領収控用)
    Private PrinterKFJP049 As String 'プリンタ(口座振替用納付書送付書用)
    Private PrinterKFJP050 As String 'プリンタ(口座振替処理結果件数表用)
#Region " ロード"
    Private Sub KFJKOKZ010_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
            Call GCom.SetMonitorTopArea(Label2, Label4, lblUser, lblDate)

            '休日マスタ取り込み
            Dim bRet As Boolean = GCom.CheckDateModule(Nothing, 1)

            If bRet = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(休日情報取得)終了", "失敗", "")
                MessageBox.Show(MSG0003E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '------------------------------------
            'INIファイルの読み込み
            '------------------------------------
            If fn_INI_Read() = False Then
                Return
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '画面表示時に振替日に前営業日を表示する
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            Dim strSysDate As String
            Dim strGetdate As String = ""

            strSysDate = String.Format("{0:yyyyMMdd}", GCom.GetSysDate)

            bRet = GCom.CheckDateModule(strSysDate, strGetdate, 1, 1)

            txtFuriDateY.Text = strGetdate.Substring(0, 4)
            txtFuriDateM.Text = strGetdate.Substring(4, 2)
            txtFuriDateD.Text = strGetdate.Substring(6, 2)


            '取引先コンボボックス設定
            Dim Jyoken As String = " AND FMT_KBN_T = '02'"   '国税フォーマット
            If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
                MessageBox.Show(MSG0230W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
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
    Public Function fn_INI_Read() As Boolean
        '============================================================================
        'NAME           :fn_INI_Read
        'Parameter      :
        'Description    :FSKJ.INIファイルの読み込み
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2009/10/30
        'Update         :
        '============================================================================
        fn_INI_Read = False

        '領収証書設定
        PrnKbn = CASTCommon.GetFSKJIni("KOKUZEI", "RYOUSYUSYO")
        If PrnKbn.ToUpper = "ERR" OrElse PrnKbn = "" Then
            MessageBox.Show(String.Format(MSG0001E, "領収証書印刷区", "KOKUZEI", "RYOUSYUSYO"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        '領収証書用プリンター
        PrinterKFJP045 = CASTCommon.GetFSKJIni("KOKUZEI", "PRINTER_RYOUSYU")
        If PrinterKFJP045.ToUpper = "ERR" OrElse PrinterKFJP045 = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "領収証書用プリンター", "KOKUZEI", "PRINTER_RYOUSYU"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        PrinterKFJP045 = """" & PrinterKFJP045 & """"

        '領収控用プリンター
        PrinterKFJP048 = CASTCommon.GetFSKJIni("KOKUZEI", "PRINTER_HIKAE")
        If PrinterKFJP048.ToUpper = "ERR" OrElse PrinterKFJP048 = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "領収控用プリンター", "KOKUZEI", "PRINTER_HIKAE"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        PrinterKFJP048 = """" & PrinterKFJP048 & """"

        '口座振替用納付書送付書用プリンター
        PrinterKFJP049 = CASTCommon.GetFSKJIni("KOKUZEI", "PRINTER_NOUFUSYO")
        If PrinterKFJP049.ToUpper = "ERR" OrElse PrinterKFJP049 = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "口座振替用納付書送付書用プリンター", "KOKUZEI", "PRINTER_NOUFUSYO"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        PrinterKFJP049 = """" & PrinterKFJP049 & """"

        '口座振替処理結果件数表用プリンター
        PrinterKFJP050 = CASTCommon.GetFSKJIni("KOKUZEI", "PRINTER_KENSUUHYO")
        If PrinterKFJP050.ToUpper = "ERR" OrElse PrinterKFJP050 = Nothing Then
            MessageBox.Show(String.Format(MSG0001E, "口座振替処理結果件数表用プリンター", "KOKUZEI", "PRINTER_KENSUUHYO"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        PrinterKFJP050 = """" & PrinterKFJP050 & """"

        fn_INI_Read = True
    End Function
    Function fn_check_text() As Boolean
        '============================================================================
        'NAME           :fn_check_text
        'Parameter      :
        'Description    :更新ボタンを押下時に必須項目のテキストボックスの入力値チェック
        'Return         :True=OK,False=NG
        'Create         :2009/09/09
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
        'Description    :更新ボタンを押下時にマスタの相関チェックを実行
        'Return         :True=OK,False=NG
        'Create         :2009/09/10
        'Update         :
        '============================================================================
        fn_check_Table = False
        MainDB = New CASTCommon.MyOracle
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)
        Dim strFMT_KBN As String
        Try
            '取引先情報取得
            SQL.Append("SELECT * FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))
            If OraReader.DataReader(SQL) = True Then
                strFMT_KBN = GCom.NzStr(OraReader.Reader.Item("FMT_KBN_T"))
                OraReader.Close()
            Else
                '取引先なし
                MessageBox.Show(MSG0063W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                OraReader.Close()
                Return False
            End If

            'フォーマット区分チェック
            If strFMT_KBN <> "02" Then '国税フォーマット
                MessageBox.Show(MSG0114W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtTorisCode.Focus()
                Return False
            End If

            'スケジュール情報取得
            SQL = New StringBuilder(128)
            SQL.Append(" SELECT COUNT(*) COUNTER,MAX(HENKAN_FLG_S) HENKAN_FLG_S")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(strTORIS_CODE))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(strTORIF_CODE))
            SQL.Append(" AND TORIS_CODE_T = TORIS_CODE_S ")
            SQL.Append(" AND TORIF_CODE_T = TORIF_CODE_S ")
            SQL.Append(" AND FURI_DATE_S = " & SQ(strFURI_DATE))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")

            Dim Count As Integer
            Dim HENAKN As String
            If OraReader.DataReader(SQL) = True Then
                Count = GCom.NzInt(OraReader.Reader.Item("COUNTER"))
                HENAKN = GCom.NzStr(OraReader.Reader.Item("HENKAN_FLG_S"))
                OraReader.Close()
            Else
                'スケジュール検索失敗
                MessageBox.Show(MSG0002E.Replace("{0}", "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                OraReader.Close()
                Return False
            End If

            If Count = 0 Then
                'スケジュールなし
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            If HENAKN = "0" Then
                '返還未処理
                MessageBox.Show(MSG0090W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If MainDB IsNot Nothing Then MainDB.Close()
        End Try
        fn_check_Table = True
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
                Dim Jyoken As String = " AND FMT_KBN_T = '02'"   '国税フォーマット
                If GCom.SelectItakuName("", cmbToriName, txtTorisCode, txtTorifCode, "1", Jyoken) = -1 Then
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
            '取引先コンボボックス設定(Msg????W)
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
             Handles txtFuriDateY.Validating, txtFuriDateM.Validating, txtFuriDateD.Validating, txtTorisCode.Validating, txtTorifCode.Validating
        Try
            Call GCom.NzNumberString(CType(sender, TextBox), True)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ゼロパディング)", "失敗", ex.ToString)
        End Try
    End Sub
#End Region
#Region " 領収控"
    Private Sub btnPrint1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint1.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(領収控)開始", "成功", "")

            'パラメータ設定：取引先主コード,取引先副コード,振替日,プリンター名,用紙名
            'Dim PrinterName As String = """Microsoft XPS Document Writer"""
            Dim PrinterName As String = PrinterKFJP048
            Const PaperName As String = "領収済通知書・領収控" '幅33.20cm × 高さ28.00cm

            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            strTORIS_CODE = txtTorisCode.Text
            strTORIF_CODE = txtTorifCode.Text

            LW.ToriCode = strTORIS_CODE & strTORIF_CODE
            LW.FuriDate = strFURI_DATE

            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            ''使わないかもしれないが、一応用紙設定存在チェック
            'Dim pdo As New System.Drawing.Printing.PrintDocument
            'Dim PageSetupDialog1 As New PageSetupDialog
            'Dim intItem_Index As Integer
            'PageSetupDialog1.Document = pdo
            'PageSetupDialog1.Document.DefaultPageSettings.Color = False
            'Dim pkPeperSize As PaperSize
            'For intItem_Index = 0 To pdo.PrinterSettings.PaperSizes.Count - 1
            '    pkPeperSize = pdo.PrinterSettings.PaperSizes.Item(intItem_Index)
            '    If pkPeperSize.PaperName = PaperName Then
            '        Exit For
            '    End If
            'Next
            ''領収済通知書・領収控がなかった場合、メッセージを表示して終了
            'If intItem_Index = pdo.PrinterSettings.PaperSizes.Count Then
            '    MessageBox.Show(String.Format(MSG0352W, PaperName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If

            '印刷前確認メッセージ
            If MessageBox.Show(MSG0013I.Replace("{0}", "領収控"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim param As String
            Dim nRet As Integer

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            param = strTORIS_CODE & "," & strTORIF_CODE & "," & strFURI_DATE & "," & PrinterName & "," & PaperName

            nRet = ExeRepo.ExecReport("KFJP048.EXE", param)

            '印刷失敗：戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(MSG0014I.Replace("{0}", "領収控"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0226W.Replace("{0}", "領収控"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(MSG0004E.Replace("{0}", "領収控"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select


        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "領収控", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(領収控)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region

#Region " 領収証書"
    Private Sub btnPrint2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint2.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(領収証書)開始", "成功", "")

            'Dim PrinterName As String = """Microsoft XPS Document Writer"""
            Dim PrinterName As String = PrinterKFJP045
            Dim PaperName As String = ""
            Select Case PrnKbn
                Case "1"    '2つ折
                    PaperName = "A4"
                Case "2"    '3つ折
                    PaperName = "領収証書"
                Case "3"    '保留中
                    PaperName = ""
            End Select
            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            strTORIS_CODE = txtTorisCode.Text
            strTORIF_CODE = txtTorifCode.Text

            LW.ToriCode = strTORIS_CODE & strTORIF_CODE
            LW.FuriDate = strFURI_DATE

            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            ''使わないかもしれないが、一応用紙設定存在チェック
            'Dim pdo As New System.Drawing.Printing.PrintDocument
            'Dim PageSetupDialog1 As New PageSetupDialog
            'Dim intItem_Index As Integer
            'PageSetupDialog1.Document = pdo
            'PageSetupDialog1.Document.DefaultPageSettings.Color = False
            'Dim pkPeperSize As PaperSize
            'For intItem_Index = 0 To pdo.PrinterSettings.PaperSizes.Count - 1
            '    pkPeperSize = pdo.PrinterSettings.PaperSizes.Item(intItem_Index)
            '    If pkPeperSize.PaperName = PaperName Then
            '        Exit For
            '    End If
            'Next
            ''領収証書がなかった場合、メッセージを表示して終了
            'If intItem_Index = pdo.PrinterSettings.PaperSizes.Count Then
            '    MessageBox.Show(String.Format(MSG0352W, PaperName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If

            '印刷前確認メッセージ
            If MessageBox.Show(MSG0013I.Replace("{0}", "領収証書"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim param As String
            Dim nRet As Integer

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：取引先主コード,取引先副コード,振替日,プリンター名,用紙名
            param = strTORIS_CODE & "," & strTORIF_CODE & "," & strFURI_DATE & "," & PrinterName & "," & PaperName

            nRet = ExeRepo.ExecReport("KFJP045.EXE", param)

            '印刷失敗：戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(MSG0014I.Replace("{0}", "領収証書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0226W.Replace("{0}", "領収証書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(MSG0004E.Replace("{0}", "領収証書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select


        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "領収証書", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(領収証書)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region

#Region " 口座振替用納付書送付書"
    Private Sub btnPrint3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint3.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座振替用納付書送付書)開始", "成功", "")
            'Dim PrinterName As String = """Microsoft XPS Document Writer"""
            Dim PrinterName As String = PrinterKFJP049
            Const PaperName As String = "口座振替納付書送付書" '23.30cm × 高さ8.90cm

            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            strTORIS_CODE = txtTorisCode.Text
            strTORIF_CODE = txtTorifCode.Text

            LW.ToriCode = strTORIS_CODE & strTORIF_CODE
            LW.FuriDate = strFURI_DATE

            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            ''使わないかもしれないが、一応用紙設定存在チェック
            'Dim pdo As New System.Drawing.Printing.PrintDocument
            'Dim PageSetupDialog1 As New PageSetupDialog
            'Dim intItem_Index As Integer
            'PageSetupDialog1.Document = pdo
            'PageSetupDialog1.Document.DefaultPageSettings.Color = False
            'Dim pkPeperSize As PaperSize
            'For intItem_Index = 0 To pdo.PrinterSettings.PaperSizes.Count - 1
            '    pkPeperSize = pdo.PrinterSettings.PaperSizes.Item(intItem_Index)
            '    If pkPeperSize.PaperName = PaperName Then
            '        Exit For
            '    End If
            'Next
            ''口座振替納付書送付書がなかった場合、メッセージを表示して終了
            'If intItem_Index = pdo.PrinterSettings.PaperSizes.Count Then
            '    MessageBox.Show(String.Format(MSG0352W, PaperName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If

            '印刷前確認メッセージ
            If MessageBox.Show(MSG0013I.Replace("{0}", "口座振替用納付書送付書"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim param As String
            Dim nRet As Integer

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：取引先主コード,取引先副コード,振替日,プリンター名,用紙名
            param = strTORIS_CODE & "," & strTORIF_CODE & "," & strFURI_DATE & "," & PrinterName & "," & PaperName

            nRet = ExeRepo.ExecReport("KFJP049.EXE", param)

            '印刷失敗：戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(MSG0014I.Replace("{0}", "口座振替用納付書送付書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0226W.Replace("{0}", "口座振替用納付書送付書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(MSG0004E.Replace("{0}", "口座振替用納付書送付書"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select


        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座振替用納付書送付書", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座振替用納付書送付書)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region

#Region " 口座振替処理結果件数表"
    Private Sub btnPrint4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint4.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座振替処理結果件数表)開始", "成功", "")
            'Dim PrinterName As String = """Microsoft XPS Document Writer"""
            Dim PrinterName As String = PrinterKFJP050
            Dim PaperName As String = "B4"

            '--------------------------------
            'テキストボックスの入力チェック
            '--------------------------------
            If fn_check_text() = False Then
                Exit Sub
            End If
            strFURI_DATE = txtFuriDateY.Text & txtFuriDateM.Text & txtFuriDateD.Text
            strTORIS_CODE = txtTorisCode.Text
            strTORIF_CODE = txtTorifCode.Text

            LW.ToriCode = strTORIS_CODE & strTORIF_CODE
            LW.FuriDate = strFURI_DATE

            '--------------------------------
            'マスタチェック
            '--------------------------------
            If fn_check_Table() = False Then
                Exit Sub
            End If

            ''使わないかもしれないが、一応用紙設定存在チェック
            'Dim pdo As New System.Drawing.Printing.PrintDocument
            'Dim PageSetupDialog1 As New PageSetupDialog
            'Dim intItem_Index As Integer
            'PageSetupDialog1.Document = pdo
            'PageSetupDialog1.Document.DefaultPageSettings.Color = False
            'Dim pkPeperSize As PaperSize
            'For intItem_Index = 0 To pdo.PrinterSettings.PaperSizes.Count - 1
            '    pkPeperSize = pdo.PrinterSettings.PaperSizes.Item(intItem_Index)
            '    If pkPeperSize.PaperName = PaperName Then
            '        Exit For
            '    End If
            'Next
            ''口座振替納付書送付書がなかった場合、メッセージを表示して終了
            'If intItem_Index = pdo.PrinterSettings.PaperSizes.Count Then
            '    MessageBox.Show(String.Format(MSG0352W, PaperName), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            '    Exit Sub
            'End If

            '印刷前確認メッセージ
            If MessageBox.Show(MSG0013I.Replace("{0}", "口座振替処理結果件数表"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            Dim param As String
            Dim nRet As Integer

            '印刷バッチ呼び出し
            Dim ExeRepo As New CAstReports.ClsExecute
            ExeRepo.SetOwner = Me

            'パラメータ設定：取引先主コード,取引先副コード,振替日,プリンター名,用紙名
            param = strTORIS_CODE & "," & strTORIF_CODE & "," & strFURI_DATE & "," & PrinterName & "," & PaperName

            nRet = ExeRepo.ExecReport("KFJP050.EXE", param)

            '印刷失敗：戻り値に対応したメッセージを表示する
            Select Case nRet
                Case 0
                    '印刷後確認メッセージ
                    MessageBox.Show(MSG0014I.Replace("{0}", "口座振替処理結果件数表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case -1
                    '印刷対象なしメッセージ
                    MessageBox.Show(MSG0226W.Replace("{0}", "口座振替処理結果件数表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Case Else
                    '印刷失敗メッセージ
                    MessageBox.Show(MSG0004E.Replace("{0}", "口座振替処理結果件数表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select


        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "口座振替処理結果件数表", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座振替処理結果件数表)終了", "成功", "")
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"
        End Try
    End Sub
#End Region

End Class