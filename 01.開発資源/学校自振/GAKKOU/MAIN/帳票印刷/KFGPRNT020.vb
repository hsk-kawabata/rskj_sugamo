Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Public Class KFGPRNT020

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 口座振替予定一覧表印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Dim Str_Report_Path As String
    Dim STR請求月 As String
    Dim STR１学年 As String
    Dim STR２学年 As String
    Dim STR３学年 As String
    Dim STR４学年 As String
    Dim STR５学年 As String
    Dim STR６学年 As String
    Dim STR７学年 As String
    Dim STR８学年 As String
    Dim STR９学年 As String
    Dim STR学校コード As String
    Dim STR帳票ソート順 As String
    Dim STR帳票ソート区分 As String

    Private STR学校名 As String

    Private Str_Sch_Kbn As String
    Private STR_FURI_DATE_SAIFURI As String

    Dim str請求年月度 As String
    Dim LNG振替金額合計 As Long
    Dim str前月_振替日 As String
    Dim LNG不能件数 As Long

    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT020", "口座振替予定一覧表印刷画面")
    Private Const msgTitle As String = "口座振替予定一覧表印刷画面(KFGPRNT020)"
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
    Private Sub KFGPRNT020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
                MessageBox.Show(String.Format(MSG0013E, "学校検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            'テキストファイルからコンボボックス設定
            Dim MSG As String
            Select Case GCom.SetComboBox(cmbFURIKUBUN, STR_FURI_SFURI_KBN_TXT, True)
                Case 1  'ファイルなし
                    MSG = String.Format(MSG0025E, "振替区分", STR_FURI_SFURI_KBN_TXT)
                    MessageBox.Show(MSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MSG = "振替区分" & "設定ファイルなし。ファイル:" & STR_FURI_SFURI_KBN_TXT
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", MSG)
                    Return
                Case 2  '異常
                    MSG = String.Format(MSG0026E, "振替区分")
                    MessageBox.Show(MSG.ToString, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    MSG = "コンボボックス設定失敗 コンボボックス名:" & "振替区分"
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", MSG)
                    Return
            End Select

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
    '****************************
    'Button Click
    '****************************
    Private Sub btnPrnt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrnt.Click
        Try
            Dim Param As String
            '印刷ボタン
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)開始", "成功", "")
            LNG不能件数 = 0
            MainDB = New MyOracle
            '入力値チェック
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If

            Str_Sch_Kbn = ""

            STR１学年 = ""
            STR２学年 = ""
            STR３学年 = ""
            STR４学年 = ""
            STR５学年 = ""
            STR６学年 = ""
            STR７学年 = ""
            STR８学年 = ""
            STR９学年 = ""

            STR_COMMAND = "印刷"


            LW.ToriCode = txtGAKKOU_CODE.Text
            LW.FuriDate = STR_FURIKAE_DATE(1)


            MainDB.BeginTrans()

            'ワークマスタ削除
            If PFUNC_PRTWORK_DEL() = False Then
                Exit Sub
            End If

            MainDB.Commit()

            If Trim(txtGAKKOU_CODE.Text) <> "9999999999" Then
                STR学校コード = Trim(txtGAKKOU_CODE.Text)

                '再振時、初振の処理が終了しているかのチェック
                If PFUNC_SCHMAST_CHECK() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                '再振時、初振日を取得
                Call PSUB_GETSYOFURI()


                '処理対象学年の取得
                If PFUNC_SCHMAST_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                '帳票ソート指示の取得
                If PFUNC_GAKMAST2_GET() = False Then
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                '印刷前確認メッセージ
                If MessageBox.Show(String.Format(MSG0013I, "口座振替予定一覧表"), _
                                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                Dim ExeRepo As New CAstReports.ClsExecute
                ExeRepo.SetOwner = Me
                Dim nRet As Integer
                If Trim(STR_FURI_DATE_SAIFURI) <> "" Then
                    '再振用帳票
                    '口座振替一覧表(口座データ),口座振替予定一覧表(口座データ0件用)印刷 
                    'パラメータ設定：ログイン名,学校コード,振替日,再振日,請求月,帳票ソート順,不能件数
                    Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURI_DATE_SAIFURI & "," & STR_FURIKAE_DATE(1) & "," & _
                            STR請求月 & "," & STR帳票ソート順 & "," & LNG不能件数

                    nRet = ExeRepo.ExecReport("KFGP014.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                            '2011/06/16 標準版修正 レコード件数0件の場合、メッセージ変更------------------START
                        Case -1
                            '印刷対象なしメッセージ
                            MessageBox.Show(String.Format("印刷対象0件です。", "口座振替予定一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                            '2011/06/16 標準版修正 レコード件数0件の場合、メッセージ変更------------------END
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "口座振替予定一覧表(口座データ)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                    End Select
                Else
                    'G_PRTWORKにデータをインサートし、G_PRTWORKの値を印刷する
                    MainDB.BeginTrans()
                    If PFUNC_PRTWORK_SEITOINS() = False Then
                        MainDB.Rollback()
                        Exit Sub
                    End If
                    MainDB.Commit()
                    '依頼用帳票
                    '口座振替一覧表印刷 
                    'パラメータ設定：ログイン名,学校コード,振替日,再振日,請求月,帳票ソート順,不能件数
                    Param = GCom.GetUserID & "," & STR学校コード & "," & STR帳票ソート順

                    nRet = ExeRepo.ExecReport("KFGP013.EXE", Param)
                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                            '2011/06/16 標準版修正 レコード件数0件の場合、メッセージ変更------------------START
                        Case -1
                            '印刷対象なしメッセージ
                            MessageBox.Show(String.Format("印刷対象0件です。", "口座振替予定一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                            '2011/06/16 標準版修正 レコード件数0件の場合、メッセージ変更------------------END
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "口座振替予定一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                    End Select
                End If

                MessageBox.Show(String.Format(MSG0014I, "口座振替予定一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                '全学校コード対象
                Dim SQL As New StringBuilder
                Dim OraReader As New MyOracleReader(MainDB)
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'")
                SQL.Append(" AND FURI_KBN_S ='" & GCom.GetComboBox(cmbFURIKUBUN) & "'")
                '2006/10/26　不能フラグチェックをはずす
                'If GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURI_SFURI_KBN_TXT, cmbFURIKUBUN) = 1 Then
                '    STR_SQL += " AND"
                '    STR_SQL += " FUNOU_FLG_S ='1' "
                'End If
                SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")


                Dim intCOUNT As Integer = 0
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Exit Sub
                End If

                '印刷前確認メッセージ
                If MessageBox.Show(String.Format(MSG0013I, "口座振替予定一覧表"), _
                                   msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                    Return
                End If

                While OraReader.EOF = False
                    STR学校コード = OraReader.GetString("GAKKOU_CODE_S")

                    '再振時、初振の処理が終了しているかのチェック
                    If PFUNC_SCHMAST_CHECK() = False Then
                        GoTo NEXT_DATA
                    End If
                    '再振時、初振日を取得
                    Call PSUB_GETSYOFURI()


                    '対象学年の取得
                    If PFUNC_SCHMAST_GET() = False Then
                        Exit Sub
                    End If

                    '帳票ソート順の取得
                    If PFUNC_GAKMAST2_GET() = False Then
                        '    Exit Sub
                    End If

                    intCOUNT += 1

                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me
                    Dim nRet As Integer
                    If Trim(STR_FURI_DATE_SAIFURI) <> "" Then
                        '再振用帳票
                        '口座振替一覧表(口座データ),口座振替予定一覧表(口座データ0件用)印刷
                        'パラメータ設定：ログイン名,学校コード,振替日,再振日,請求月,帳票ソート順,不能件数
                        Param = GCom.GetUserID & "," & STR学校コード & "," & STR_FURI_DATE_SAIFURI & "," & STR_FURIKAE_DATE(1) & "," & _
                                STR請求月 & "," & STR帳票ソート順 & "," & LNG不能件数

                        nRet = ExeRepo.ExecReport("KFGP014.EXE", Param)
                        '戻り値に対応したメッセージを表示する
                        Select Case nRet
                            Case 0
                                '2011/06/16 標準版修正 レコード件数0件の場合、メッセージ変更------------------START
                            Case -1
                                '2011/06/16 標準版修正 レコード件数0件の場合、メッセージ変更------------------END
                            Case Else
                                '印刷失敗メッセージ
                                MessageBox.Show(String.Format(MSG0004E, "口座振替予定一覧表(口座データ)"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Select
                    Else
                        'G_PRTWORKにデータをインサートし、G_PRTWORKの値を印刷する
                        MainDB.BeginTrans()
                        If PFUNC_PRTWORK_SEITOINS() = False Then
                            MainDB.Rollback()
                        Else
                            MainDB.Commit()
                            '依頼用帳票
                            '口座振替一覧表印刷 
                            'パラメータ設定：ログイン名,学校コード,振替日,再振日,請求月,帳票ソート順,不能件数
                            Param = GCom.GetUserID & "," & STR学校コード & "," & STR帳票ソート順

                            nRet = ExeRepo.ExecReport("KFGP013.EXE", Param)
                            '戻り値に対応したメッセージを表示する
                            Select Case nRet
                                Case 0
                                    '2011/06/16 標準版修正 レコード件数0件の場合、メッセージ変更------------------START
                                Case -1
                                    '2011/06/16 標準版修正 レコード件数0件の場合、メッセージ変更------------------END
                                Case Else
                                    '印刷失敗メッセージ
                                    MessageBox.Show(String.Format(MSG0004E, "口座振替予定一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Exit Sub
                            End Select
                        End If

                    End If
NEXT_DATA:
                    OraReader.NextRead()
                End While

                If intCOUNT = 0 And GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_FURI_SFURI_KBN_TXT, cmbFURIKUBUN) = "1" Then
                    MessageBox.Show(G_MSG0024W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    MessageBox.Show(String.Format(MSG0014I, "口座振替予定一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If

            txtGAKKOU_CODE.Focus()

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

        'COMBOBOX選択時学校名,学校コード設定
        lab学校名.Text = cmbGakkouName.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex)

        '学年テキストボックスにFOCUS
        txtGAKKOU_CODE.Focus()

    End Sub
    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating
        Call GCom.NzNumberString(CType(sender, TextBox), True)
        With CType(sender, TextBox)
            If .Text.Trim <> "" Then
                .Text = .Text.Trim.PadLeft(.MaxLength, "0")
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
#Region " Private Function "
    '****************************
    'Private Function
    '****************************
    Private Function PFUNC_GAKNAME_GET() As Boolean
        '学校名の設定
        PFUNC_GAKNAME_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Dim OraDB As MyOracle = Nothing
        Try
            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab学校名.Text = ""
            Else
                If OraDB Is Nothing Then OraDB = New MyOracle
                OraReader = New MyOracleReader(OraDB)
                SQL.Append(" SELECT * FROM GAKMAST1")
                SQL.Append(" WHERE GAKKOU_CODE_G =" & SQ(txtGAKKOU_CODE.Text))

                '学校マスタ１存在チェック
                If OraReader.DataReader(SQL) = False Then
                    MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    lab学校名.Text = ""
                    txtGAKKOU_CODE.Focus()
                    Exit Function
                End If

                lab学校名.Text = OraReader.GetString("GAKKOU_NNAME_G")

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraDB IsNot Nothing Then OraDB.Close()
        End Try
        PFUNC_GAKNAME_GET = True

    End Function
    Private Function PFUNC_GAKMAST2_GET() As Boolean
        '学校マスタ２の取得

        PFUNC_GAKMAST2_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM GAKMAST2")
            SQL.Append(" WHERE")
            SQL.Append(" GAKKOU_CODE_T =" & SQ(STR学校コード))

            If OraReader.DataReader(SQL) = False Then
                STR帳票ソート順 = "0"
            Else
                STR帳票ソート順 = OraReader.GetString("MEISAI_OUT_T")
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

        PFUNC_GAKMAST2_GET = True

    End Function
    Private Function PFUNC_SCHMAST_GET() As Boolean

        PFUNC_SCHMAST_GET = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT * FROM G_SCHMAST")
            SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(STR学校コード))
            SQL.Append(" AND FURI_DATE_S = " & SQ(STR_FURIKAE_DATE(1)))
            SQL.Append(" AND FURI_KBN_S =" & SQ(GCom.GetComboBox(cmbFURIKUBUN)))

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '2017/03/14 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ START
            '特別振替日対応
            STR１学年 = ""
            STR２学年 = ""
            STR３学年 = ""
            STR４学年 = ""
            STR５学年 = ""
            STR６学年 = ""
            STR７学年 = ""
            STR８学年 = ""
            STR９学年 = ""
            '2017/03/14 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ END

            While OraReader.EOF = False
                With OraReader
                    If Trim(Str_Sch_Kbn) = "" Then
                        Str_Sch_Kbn = .GetString("SCH_KBN_S")
                    Else
                        Str_Sch_Kbn = 9
                    End If

                    'If Trim(Str_Sch_Kbn) <> "" AND Str_Sch_Kbn <> .GetString("SCH_KBN_S") Then
                    '    Str_Sch_Kbn = 9
                    'Else
                    '    Str_Sch_Kbn = .GetString("SCH_KBN_S")
                    'End If

                    STR請求月 = Mid(.GetString("NENGETUDO_S"), 5, 2)
                    str請求年月度 = .GetString("NENGETUDO_S")
                    If .GetString("GAKUNEN1_FLG_S") = "1" Then
                        STR１学年 = "1"
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
                        '特別振替日対応
                        'Else
                        '    If STR１学年 <> "" Then
                        '        STR１学年 = ""
                        '    End If
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
                    End If
                    If .GetString("GAKUNEN2_FLG_S") = "1" Then
                        STR２学年 = "1"
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
                        '特別振替日対応
                        'Else
                        '    If STR２学年 <> "" Then
                        '        STR２学年 = ""
                        '    End If
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
                    End If
                    If .GetString("GAKUNEN3_FLG_S") = "1" Then
                        STR３学年 = "1"
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
                        '特別振替日対応
                        'Else
                        '    If STR３学年 <> "" Then
                        '        STR３学年 = ""
                        '    End If
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
                    End If
                    If .GetString("GAKUNEN4_FLG_S") = "1" Then
                        STR４学年 = "1"
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
                        '特別振替日対応
                        'Else
                        '    If STR４学年 <> "" Then
                        '        STR４学年 = ""
                        '    End If
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
                    End If
                    If .GetString("GAKUNEN5_FLG_S") = "1" Then
                        STR５学年 = "1"
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
                        '特別振替日対応
                        'Else
                        '    If STR５学年 <> "" Then
                        '        STR５学年 = ""
                        '    End If
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
                    End If
                    If .GetString("GAKUNEN6_FLG_S") = "1" Then
                        STR６学年 = "1"
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
                        '特別振替日対応
                        'Else
                        '    If STR６学年 <> "" Then
                        '        STR６学年 = ""
                        '    End If
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
                    End If
                    If .GetString("GAKUNEN7_FLG_S") = "1" Then
                        STR７学年 = "1"
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
                        '特別振替日対応
                        'Else
                        '    If STR７学年 <> "" Then
                        '        STR７学年 = ""
                        '    End If
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
                    End If
                    If .GetString("GAKUNEN8_FLG_S") = "1" Then
                        STR８学年 = "1"
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
                        '特別振替日対応
                        'Else
                        '    If STR８学年 <> "" Then
                        '        STR８学年 = ""
                        '    End If
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
                    End If
                    If .GetString("GAKUNEN9_FLG_S") = "1" Then
                        STR９学年 = "1"
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ START
                        '特別振替日対応
                        'Else
                        '    If STR９学年 <> "" Then
                        '        STR９学年 = ""
                        '    End If
                        '2017/03/14 タスク）西野 DEL 標準版修正（潜在バグ修正）------------------------------------ END
                    End If
                End With
                OraReader.NextRead()
            End While

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校マスタ検索)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

        PFUNC_SCHMAST_GET = True

    End Function
    Private Function PFUNC_SCHMAST_CHECK() As Boolean

        PFUNC_SCHMAST_CHECK = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            '選択した振替区分が再振の場合のみ
            If GCom.GetComboBox(cmbFURIKUBUN) = 1 Then
                OraReader = New MyOracleReader(MainDB)

                '指定した日付を再振日に持つ初振のスケジュールが不能結果更新済みであるかどうかのチェック
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_S = " & SQ(STR学校コード))
                SQL.Append(" AND SFURI_DATE_S =" & SQ(STR_FURIKAE_DATE(1)))
                SQL.Append(" AND FURI_KBN_S ='0'")

                '初振、再振
                If OraReader.DataReader(SQL) Then
                    If OraReader.GetInt64("FUNOU_FLG_S") = 0 Then
                        MessageBox.Show(G_MSG0025W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                    End If
                Else
                    MessageBox.Show(MSG0056W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(スケジュールチェック)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

        PFUNC_SCHMAST_CHECK = True

    End Function
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False
        Try
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

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(入力チェック)", "失敗", ex.ToString)
            Return False
        End Try

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Sub PSUB_GETSYOFURI()

        STR_FURI_DATE_SAIFURI = ""
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        Try
            '選択した振替区分が再振の場合のみ
            If GCom.GetComboBox(cmbFURIKUBUN) = 1 Then
                OraReader = New MyOracleReader(MainDB)
                '画面で入力した日付を再振日に持つ初振のスケジュールを取得
                SQL.Append(" SELECT * FROM G_SCHMAST")
                SQL.Append(" WHERE GAKKOU_CODE_S ='" & STR学校コード & "'")
                SQL.Append(" AND FURI_KBN_S ='0'")
                SQL.Append(" AND SFURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'")


                If OraReader.DataReader(SQL) = False Then
                    Exit Sub
                Else
                    STR_FURI_DATE_SAIFURI = OraReader.GetString("FURI_DATE_S")
                    LNG不能件数 = OraReader.GetString("FUNOU_KEN_S")
                End If
            Else

            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "初振日取得", "失敗", ex.ToString)
            Return
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try

    End Sub
    Private Function PFUNC_PRTWORK_SEITOINS() As Boolean

        PFUNC_PRTWORK_SEITOINS = False
        Dim OraReader As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)
        '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------START
        Dim Funou_FLG As Boolean = False
        '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------END

        ' 2017/05/25 タスク）綾部 ADD 【OT】(飯田信金 再振対象コードIniファイル化) -------------------- START
        Dim SFuriCode As String = String.Empty
        ' 2017/05/25 タスク）綾部 ADD 【OT】(飯田信金 再振対象コードIniファイル化) -------------------- END

        Try
            ' 2017/05/25 タスク）綾部 ADD 【OT】(飯田信金 再振対象コードIniファイル化) -------------------- START
            SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_SFURI_CODE")
            If SFuriCode = "err" OrElse SFuriCode = "" Then
                SFuriCode = "1,2,3,4,5,6,7,8,9"
            End If
            ' 2017/05/25 タスク）綾部 ADD 【OT】(飯田信金 再振対象コードIniファイル化) -------------------- END

            OraReader = New MyOracleReader(MainDB)
            '-----------------------
            '持越ありか判定
            '-----------------------
            SQL.Append(" SELECT * FROM GAKMAST2")
            SQL.Append(" WHERE")
            SQL.Append(" GAKKOU_CODE_T =" & SQ(STR学校コード))

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(G_MSG0002W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Function
            End If

            Dim strSFURI_SYUBETU As String = ""

            strSFURI_SYUBETU = OraReader.GetString("SFURI_SYUBETU_T")

            OraReader.Close()

            '持越しありのとき
            If strSFURI_SYUBETU = "2" Or strSFURI_SYUBETU = "3" Then
                Select Case (STR請求月)
                    Case "04"  '4月は持越しがないため飛ばす
                    Case Else
                        Dim strNEGETU As String, strNEN As String
                        If STR請求月 = "01" Then
                            strNEGETU = "12"
                            strNEN = CStr(Val(str請求年月度.Substring(0, 4)) - 1)
                        Else
                            strNEGETU = CStr(Val(STR請求月) - 1).PadLeft(2, "0")
                            strNEN = str請求年月度.Substring(0, 4)
                        End If
                        SQL = New StringBuilder(128)
                        SQL.Append(" SELECT * FROM G_SCHMAST")
                        SQL.Append(" WHERE GAKKOU_CODE_S =" & SQ(STR学校コード))
                        SQL.Append(" AND FURI_DATE_S < " & SQ(STR_FURIKAE_DATE(1))) '入力振替日以前のもので不能のものを探す
                        '  SQL.Append(" AND NENGETUDO_S ='" & strNEN & strNEGETU & "'")
                        SQL.Append(" AND (FURI_KBN_S =  '0' OR FURI_KBN_S = '1')")
                        '  SQL.Append(" FURI_KBN_S ='0'")
                        If Str_Sch_Kbn = "9" Then
                            SQL.Append(" AND SCH_KBN_S ='0'")
                        End If
                        SQL.Append(" AND FUNOU_FLG_S =  '1'") '入力振替日以前で不能フラグがたっているスケジュール
                        SQL.Append(" ORDER BY")
                        SQL.Append(" FURI_DATE_S desc")
                        '2010/10/21 持越しが複数スケジュール合った場合の対応追加-------------------------------------
                        'OraReader = New MyOracleReader(MainDB)
                        Dim G_SchReader As MyOracleReader = Nothing
                        G_SchReader = New MyOracleReader(MainDB)

                        'If OraReader.DataReader(SQL) = False Then
                        '    OraReader.Close()
                        '    Exit Select
                        'End If
                        If G_SchReader.DataReader(SQL) = False Then
                            G_SchReader.Close()
                            Exit Select
                        End If
                        '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------START
                        Dim Furi_Date_BK As String
                        Furi_Date_BK = G_SchReader.GetString("FURI_DATE_S")
                        '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------END
                        While G_SchReader.EOF = False

                            Dim dblFUNOU_KEN As Double = 0

                            'str前月_振替日 = OraReader.GetString("FURI_DATE_S")
                            'dblFUNOU_KEN = OraReader.GetString("FUNOU_KEN_S")
                            str前月_振替日 = G_SchReader.GetString("FURI_DATE_S")
                            dblFUNOU_KEN = G_SchReader.GetString("FUNOU_KEN_S")
                            '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------START
                            If Furi_Date_BK <> str前月_振替日 Then
                                OraReader.Close()
                                G_SchReader.Close()
                                Exit Select
                            End If
                            '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------END
                            'OraReader.Close() '2010/10/21 コメントアウト
                            '---------------------------------------------------------------------------------------
                            If dblFUNOU_KEN = 0 Then
                                '2010/10/21 Exit Selectしない ここから
                                'Exit Select
                                'End If
                            Else
                                '2010/10/21 Exit Selectしない ここまで

                                '持越しありで、先月の不能件数が0件以上だった場合、先月不能分もインサート
                                SQL = New StringBuilder(128)
                                SQL.Append(" SELECT * FROM G_MEIMAST")
                                SQL.Append(" WHERE")
                                SQL.Append(" GAKKOU_CODE_M = " & SQ(STR学校コード))
                                SQL.Append(" AND FURI_DATE_M = " & SQ(str前月_振替日))
                                SQL.Append(" AND (FURI_KBN_M =  '0' OR FURI_KBN_M = '1')")
                                '  SQL.Append( " FURI_KBN_M ='0'")
                                ''  SQL.Append(" AND SEIKYU_TUKI_M ='" & strNEN & strNEGETU & "'")

                                ' 2017/05/25 タスク）綾部 CHG 【OT】(飯田信金 再振対象コードIniファイル化) -------------------- START
                                'SQL.Append(" AND FURIKETU_CODE_M <> '0'")
                                SQL.Append(" AND FURIKETU_CODE_M IN (" & SFuriCode & ")")
                                ' 2017/05/25 タスク）綾部 CHG 【OT】(飯田信金 再振対象コードIniファイル化) -------------------- END

                                '2011/06/16 標準版修正 前年度分は持ち越さない（抽出しない）------------------START                                '2011/05/30 修正
                                If CInt(STR請求月) >= 4 And CInt(STR請求月) <= 12 Then                  '振替日の「月」が４～１２月だったらその年の４月以降のみ抽出する
                                    SQL.Append(" AND FURI_DATE_M >= '" & str請求年月度.Substring(0, 4) & "0401'  ")
                                ElseIf CInt(STR請求月) >= 1 And CInt(STR請求月) <= 3 Then               '振替日の「月」が１～３月だったらその前の年の４月以降のみ抽出する
                                    SQL.Append(" AND FURI_DATE_M >= '" & CStr(Val(str請求年月度.Substring(0, 4)) - 1) & "0401'  ")
                                End If
                                '2011/06/16 標準版修正 前年度分は持ち越さない（抽出しない）------------------END

                                OraReader = New MyOracleReader(MainDB)

                                If OraReader.DataReader(SQL) = False Then
                                    OraReader.Close()
                                    '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------START
                                    G_SchReader.Close()
                                    '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------END
                                    Exit Select
                                End If

                                While OraReader.EOF = False

                                    LNG振替金額合計 = OraReader.GetString("SEIKYU_KIN_M")

                                    If INSERT_PRTWORK_MEIMAST(OraReader) = False Then
                                        OraReader.Close()
                                        '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------START
                                        G_SchReader.Close()
                                        '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------END
                                        Exit Function
                                    End If
                                    '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------START
                                    Funou_FLG = True
                                    '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------END
                                    OraReader.NextRead()
                                End While
                                OraReader.Close()
                            End If '2010/10/21 Exit Selectしない End If追加
                            '2010/10/21 持越しが複数スケジュール合った場合の対応追加
                            '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------START
                            Furi_Date_BK = str前月_振替日
                            '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------END
                            G_SchReader.NextRead()
                        End While
                        G_SchReader.Close()
                        '------------------------------------------------------------
                End Select
            End If
            '今月の対象データを生徒マスタビューから抽出
            SQL = New StringBuilder
            SQL.Append("SELECT * FROM SEITOMASTVIEW WHERE ")
            SQL.Append(" SEITOMASTVIEW.GAKKOU_CODE_O  =" & SQ(STR学校コード))
            SQL.Append(" AND　(")

            '対象学年はスケジュールの学年フラグがONのもの
            If STR１学年 = "1" Then
                SQL.Append(" SEITOMASTVIEW.GAKUNEN_CODE_O = 1")
                If STR２学年 = "1" Or STR３学年 = "1" Or STR４学年 = "1" Or STR５学年 = "1" Or STR６学年 = "1" Or STR７学年 = "1" Or STR８学年 = "1" Or STR９学年 = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR２学年 = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 2")
                If STR３学年 = "1" Or STR４学年 = "1" Or STR５学年 = "1" Or STR６学年 = "1" Or STR７学年 = "1" Or STR８学年 = "1" Or STR９学年 = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR３学年 = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 3")
                If STR４学年 = "1" Or STR５学年 = "1" Or STR６学年 = "1" Or STR７学年 = "1" Or STR８学年 = "1" Or STR９学年 = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR４学年 = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 4")
                If STR５学年 = "1" Or STR６学年 = "1" Or STR７学年 = "1" Or STR８学年 = "1" Or STR９学年 = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR５学年 = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 5")
                If STR６学年 = "1" Or STR７学年 = "1" Or STR８学年 = "1" Or STR９学年 = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR６学年 = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 6")
                If STR７学年 = "1" Or STR８学年 = "1" Or STR９学年 = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR７学年 = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 7")
                If STR８学年 = "1" Or STR９学年 = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR８学年 = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 8")
                If STR９学年 = "1" Then
                    SQL.Append(" OR")
                Else
                    SQL.Append(" )")
                End If
            End If
            If STR９学年 = "1" Then
                SQL.Append("  SEITOMASTVIEW.GAKUNEN_CODE_O = 9)")
            End If
            'スケジュールの請求月と同じ生徒マスタビュ
            SQL.Append(" AND SEITOMASTVIEW.TUKI_NO_O =" & SQ(STR請求月))
            '振替方法（0:口座振替）
            SQL.Append(" AND SEITOMASTVIEW.FURIKAE_O = '0'")
            '解約区分=0 （通常）
            SQL.Append(" AND SEITOMASTVIEW.KAIYAKU_FLG_O  =" & "'0'")
            '2006/10/16　振替金額が0円のデータは印字しないように修正
            '2013/01/23 saitou 大垣信金 DEL -------------------------------------------------->>>>
            '振替金額が0円も対象とする。
            'SQL.Append(" AND (SEITOMASTVIEW.KINGAKU01_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU02_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU03_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU04_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU05_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU06_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU07_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU08_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU09_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU10_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU11_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU12_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU13_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU14_O  <> 0 ")
            'SQL.Append(" OR SEITOMASTVIEW.KINGAKU15_O  <> 0) ")
            '2013/01/23 saitou 大垣信金 DEL --------------------------------------------------<<<<

            OraReader = New MyOracleReader(MainDB)

            If OraReader.DataReader(SQL) = False Then
                '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------START
                If Funou_FLG = False Then
                    If txtGAKKOU_CODE.Text <> "9999999999" Then
                        MessageBox.Show(MSG0106W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    OraReader.Close()
                    Exit Function
                End If
                'If txtGAKKOU_CODE.Text <> "9999999999" Then
                '    MessageBox.Show("印刷対象データが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'End If
                'OraReader.Close()
                'Exit Function
                '2011/06/16 標準版修正 持ち越しは直近のみ含める------------------END
            End If

            While OraReader.EOF = False

                '振替金額合計計算
                LNG振替金額合計 = OraReader.GetInt64("KINGAKU01_O") _
                                + OraReader.GetInt64("KINGAKU02_O") _
                                + OraReader.GetInt64("KINGAKU03_O") _
                                + OraReader.GetInt64("KINGAKU04_O") _
                                + OraReader.GetInt64("KINGAKU05_O") _
                                + OraReader.GetInt64("KINGAKU06_O") _
                                + OraReader.GetInt64("KINGAKU07_O") _
                                + OraReader.GetInt64("KINGAKU08_O") _
                                + OraReader.GetInt64("KINGAKU09_O") _
                                + OraReader.GetInt64("KINGAKU10_O") _
                                + OraReader.GetInt64("KINGAKU11_O") _
                                + OraReader.GetInt64("KINGAKU12_O") _
                                + OraReader.GetInt64("KINGAKU13_O") _
                                + OraReader.GetInt64("KINGAKU14_O") _
                                + OraReader.GetInt64("KINGAKU15_O")


                If INSERT_PRTWORK_SEITOMASTVIEW(OraReader) = False Then
                    OraReader.Close()
                    Exit Function
                End If
                OraReader.NextRead()
            End While

            OraReader.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ワークテーブル作成)", "失敗", ex.ToString)
            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
        End Try


        PFUNC_PRTWORK_SEITOINS = True

    End Function
    Function INSERT_PRTWORK_SEITOMASTVIEW(ByVal OraReader As MyOracleReader) As Boolean
        INSERT_PRTWORK_SEITOMASTVIEW = False

        Dim SQL As New StringBuilder
        Try
            SQL.Append("INSERT INTO KZFMAST.G_PRTWORK2 ")
            SQL.Append(" (GAKKOU_CODE_P ")
            SQL.Append(", NENDO_P ")
            SQL.Append(", TUUBAN_P ")
            SQL.Append(", SEIKYU_TUKI_P ")
            SQL.Append(", FURI_DATE_P ")
            SQL.Append(", GAKUNEN_CODE_P ")
            SQL.Append(", CLASS_CODE_P ")
            SQL.Append(", SEITO_NO_P ")
            SQL.Append(", SEITO_KNAME_P ")
            SQL.Append(", SEITO_NNAME_P ")
            SQL.Append(", TKIN_NO_P ")
            SQL.Append(", TSIT_NO_P ")
            SQL.Append(", KAMOKU_P ")
            SQL.Append(", KOUZA_P ")
            SQL.Append(", MEIGI_KNAME_P ")
            SQL.Append(", HIMOKU_ID_P ")
            SQL.Append(", KINGAKU_P ")
            SQL.Append(", KINGAKU01_P ")
            SQL.Append(", KINGAKU02_P ")
            SQL.Append(", KINGAKU03_P ")
            SQL.Append(", KINGAKU04_P ")
            SQL.Append(", KINGAKU05_P ")
            SQL.Append(", KINGAKU06_P ")
            SQL.Append(", KINGAKU07_P ")
            SQL.Append(", KINGAKU08_P ")
            SQL.Append(", KINGAKU09_P ")
            SQL.Append(", KINGAKU10_P ")
            SQL.Append(", KINGAKU11_P ")
            SQL.Append(", KINGAKU12_P ")
            SQL.Append(", KINGAKU13_P ")
            SQL.Append(", KINGAKU14_P ")
            SQL.Append(", KINGAKU15_P ")
            SQL.Append(", YOBI01_P ")
            SQL.Append(", YOBI02_P ")
            SQL.Append(", YOBI03_P ")
            SQL.Append(", YOBI04_P ")
            SQL.Append(", YOBI05_P )")
            SQL.Append(" VALUES ( ")
            '学校コード
            SQL.Append(SQ(STR学校コード))
            '入学年度
            SQL.Append("," & SQ(OraReader.GetString("NENDO_O")))
            '通番
            SQL.Append("," & OraReader.GetInt("TUUBAN_O"))
            '請求月
            SQL.Append("," & SQ(str請求年月度.Substring(4, 2)))
            '振替日
            SQL.Append("," & SQ(STR_FURIKAE_DATE(1)))
            '学年コード
            SQL.Append("," & OraReader.GetInt("GAKUNEN_CODE_O"))
            'クラスコード
            SQL.Append("," & OraReader.GetInt("CLASS_CODE_O"))
            '生徒番号
            SQL.Append("," & "'" & OraReader.GetString("SEITO_NO_O") & "'")
            '生徒名（カナ）
            SQL.Append("," & "'" & OraReader.GetString("SEITO_KNAME_O") & "'")
            '生徒名（漢字）
            SQL.Append("," & SQ(IIf(OraReader.GetString("SEITO_NNAME_O") = "", Space(50), OraReader.GetString("SEITO_NNAME_O"))))
            '取扱金融機関
            SQL.Append("," & "'" & OraReader.GetString("TKIN_NO_O") & "'")
            '取扱支店コード
            SQL.Append("," & "'" & OraReader.GetString("TSIT_NO_O") & "'")
            '科目
            SQL.Append("," & "'" & OraReader.GetString("KAMOKU_O") & "'")
            '口座番号
            SQL.Append("," & "'" & OraReader.GetString("KOUZA_O") & "'")
            '口座名義人カナ
            SQL.Append("," & "'" & OraReader.GetString("MEIGI_KNAME_O") & "'")
            '費目ＩＤ
            SQL.Append("," & "'" & OraReader.GetString("HIMOKU_ID_O") & "'")
            '振替金額（合計）
            SQL.Append("," & LNG振替金額合計)
            '振替金額（費目１）
            SQL.Append("," & OraReader.GetInt64("KINGAKU01_O"))
            '振替金額（費目２）
            SQL.Append("," & OraReader.GetInt64("KINGAKU02_O"))
            '振替金額（費目３）
            SQL.Append("," & OraReader.GetInt64("KINGAKU03_O"))
            '振替金額（費目４）
            SQL.Append("," & OraReader.GetInt64("KINGAKU04_O"))
            '振替金額（費目５）
            SQL.Append("," & OraReader.GetInt64("KINGAKU05_O"))
            '振替金額（費目６）
            SQL.Append("," & OraReader.GetInt64("KINGAKU06_O"))
            '振替金額（費目７）
            SQL.Append("," & OraReader.GetInt64("KINGAKU07_O"))
            '振替金額（費目８）
            SQL.Append("," & OraReader.GetInt64("KINGAKU08_O"))
            '振替金額（費目９）
            SQL.Append("," & OraReader.GetInt64("KINGAKU09_O"))
            '振替金額（費目１０）
            SQL.Append("," & OraReader.GetInt64("KINGAKU10_O"))
            '振替金額（費目１１）
            SQL.Append("," & OraReader.GetInt64("KINGAKU11_O"))
            '振替金額（費目１２）
            SQL.Append("," & OraReader.GetInt64("KINGAKU12_O"))
            '振替金額（費目１３）
            SQL.Append("," & OraReader.GetInt64("KINGAKU13_O"))
            '振替金額（費目１４）
            SQL.Append("," & OraReader.GetInt64("KINGAKU14_O"))
            '振替金額（費目１５）
            SQL.Append("," & OraReader.GetInt64("KINGAKU15_O"))

            '予備1
            SQL.Append("," & "'" & Space(20) & "'")
            '予備2
            SQL.Append("," & "'" & Space(20) & "'")
            '予備3
            SQL.Append("," & "'" & Space(20) & "'")
            '予備4
            SQL.Append("," & "'" & Space(20) & "'")
            '予備5
            SQL.Append("," & "'" & Space(20) & "')")

            MainDB.ExecuteNonQuery(SQL)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        End Try


        INSERT_PRTWORK_SEITOMASTVIEW = True
    End Function
    Function INSERT_PRTWORK_MEIMAST(ByVal OraReader As MyOracleReader) As Boolean
        INSERT_PRTWORK_MEIMAST = False
        Dim SQL As New StringBuilder
        Try
            SQL.Append("INSERT INTO KZFMAST.G_PRTWORK2 ")
            SQL.Append(" (GAKKOU_CODE_P ")
            SQL.Append(", NENDO_P ")
            SQL.Append(", TUUBAN_P ")
            SQL.Append(", SEIKYU_TUKI_P ")
            SQL.Append(", FURI_DATE_P ")
            SQL.Append(", GAKUNEN_CODE_P ")
            SQL.Append(", CLASS_CODE_P ")
            SQL.Append(", SEITO_NO_P ")
            SQL.Append(", SEITO_KNAME_P ")
            SQL.Append(", SEITO_NNAME_P ")
            SQL.Append(", TKIN_NO_P ")
            SQL.Append(", TSIT_NO_P ")
            SQL.Append(", KAMOKU_P ")
            SQL.Append(", KOUZA_P ")
            SQL.Append(", MEIGI_KNAME_P ")
            SQL.Append(", HIMOKU_ID_P ")
            SQL.Append(", KINGAKU_P ")
            SQL.Append(", KINGAKU01_P ")
            SQL.Append(", KINGAKU02_P ")
            SQL.Append(", KINGAKU03_P ")
            SQL.Append(", KINGAKU04_P ")
            SQL.Append(", KINGAKU05_P ")
            SQL.Append(", KINGAKU06_P ")
            SQL.Append(", KINGAKU07_P ")
            SQL.Append(", KINGAKU08_P ")
            SQL.Append(", KINGAKU09_P ")
            SQL.Append(", KINGAKU10_P ")
            SQL.Append(", KINGAKU11_P ")
            SQL.Append(", KINGAKU12_P ")
            SQL.Append(", KINGAKU13_P ")
            SQL.Append(", KINGAKU14_P ")
            SQL.Append(", KINGAKU15_P ")
            SQL.Append(", YOBI01_P ")
            SQL.Append(", YOBI02_P ")
            SQL.Append(", YOBI03_P ")
            SQL.Append(", YOBI04_P ")
            SQL.Append(", YOBI05_P )")
            SQL.Append(" VALUES ( ")
            '学校コード
            SQL.Append("'" & STR学校コード & "'")
            '入学年度
            SQL.Append("," & "'" & OraReader.GetString("NENDO_M") & "'")
            '通番
            SQL.Append("," & CInt(OraReader.GetString("TUUBAN_M")))
            '請求月
            Dim strTUKI As String
            strTUKI = OraReader.GetString("SEIKYU_TUKI_M")
            strTUKI = strTUKI.Substring(4, 2)
            SQL.Append(",'" & strTUKI)
            '振替日
            SQL.Append("'," & "'" & STR_FURIKAE_DATE(1) & "'")
            '学年コード
            SQL.Append("," & OraReader.GetInt("GAKUNEN_CODE_M"))
            'クラスコード
            SQL.Append("," & OraReader.GetInt("CLASS_CODE_M"))
            '生徒番号
            SQL.Append("," & "'" & OraReader.GetString("SEITO_NO_M") & "'")
            '生徒名（カナ）
            SQL.Append("," & "'" & Space(1) & "'")
            '生徒名（漢字）
            SQL.Append("," & "'" & Space(1) & "'")
            '取扱金融機関
            SQL.Append("," & "'" & OraReader.GetString("TKIN_NO_M") & "'")
            '取扱支店コード
            SQL.Append("," & "'" & OraReader.GetString("TSIT_NO_M") & "'")
            '科目
            SQL.Append("," & "'" & OraReader.GetString("TKAMOKU_M") & "'")
            '口座番号
            SQL.Append("," & "'" & OraReader.GetString("TKOUZA_M") & "'")
            '口座名義人カナ
            SQL.Append("," & "'" & OraReader.GetString("TMEIGI_KNM_M") & "'")
            '費目ＩＤ
            SQL.Append("," & "'" & OraReader.GetString("HIMOKU_ID_M") & "'")
            '振替金額（合計）
            SQL.Append("," & LNG振替金額合計)
            '振替金額（費目１）
            SQL.Append("," & OraReader.GetInt64("HIMOKU1_KIN_M"))
            '振替金額（費目２）
            SQL.Append("," & OraReader.GetInt64("HIMOKU2_KIN_M"))
            '振替金額（費目３）
            SQL.Append("," & OraReader.GetInt64("HIMOKU3_KIN_M"))
            '振替金額（費目４）
            SQL.Append("," & OraReader.GetInt64("HIMOKU4_KIN_M"))
            '振替金額（費目５）
            SQL.Append("," & OraReader.GetInt64("HIMOKU5_KIN_M"))
            '振替金額（費目６）
            SQL.Append("," & OraReader.GetInt64("HIMOKU6_KIN_M"))
            '振替金額（費目７）
            SQL.Append("," & OraReader.GetInt64("HIMOKU7_KIN_M"))
            '振替金額（費目８）
            SQL.Append("," & OraReader.GetInt64("HIMOKU8_KIN_M"))
            '振替金額（費目９）
            SQL.Append("," & OraReader.GetInt64("HIMOKU9_KIN_M"))
            '振替金額（費目１０）
            SQL.Append("," & OraReader.GetInt64("HIMOKU10_KIN_M"))
            '振替金額（費目１１）
            SQL.Append("," & OraReader.GetInt64("HIMOKU11_KIN_M"))
            '振替金額（費目１２）
            SQL.Append("," & OraReader.GetInt64("HIMOKU12_KIN_M"))
            '振替金額（費目１３）
            SQL.Append("," & OraReader.GetInt64("HIMOKU13_KIN_M"))
            '振替金額（費目１４）
            SQL.Append("," & OraReader.GetInt64("HIMOKU14_KIN_M"))
            '振替金額（費目１５）
            SQL.Append("," & OraReader.GetInt64("HIMOKU15_KIN_M"))
            '予備1
            SQL.Append("," & "'" & Space(20) & "'")
            '予備2
            SQL.Append("," & "'" & Space(20) & "'")
            '予備3
            SQL.Append("," & "'" & Space(20) & "'")
            '予備4
            SQL.Append("," & "'" & Space(20) & "'")
            '予備5
            SQL.Append("," & "'" & Space(20) & "')")

            '2010/10/21 初振・再振ともに不能の場合は一意制約違反になるので無視する
            'MainDB.ExecuteNonQuery(SQL)
            MainDB.ExecuteNonQuery(SQL.ToString, True)

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(テーブルチェック)", "失敗", ex.ToString)
            Return False
        End Try

        INSERT_PRTWORK_MEIMAST = True
    End Function
    Private Function PFUNC_PRTWORK_DEL() As Boolean
        Try
            PFUNC_PRTWORK_DEL = False

            Dim SQL As New StringBuilder
            SQL.Append("DELETE  FROM G_PRTWORK2")

            MainDB.ExecuteNonQuery(SQL)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ワークマスタ削除)", "失敗", ex.ToString)
            Return False
        End Try

        PFUNC_PRTWORK_DEL = True
    End Function
#End Region

End Class
