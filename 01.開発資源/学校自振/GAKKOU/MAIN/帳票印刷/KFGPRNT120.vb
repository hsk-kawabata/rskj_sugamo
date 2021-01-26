Option Explicit On 
Option Strict Off
Imports CASTCommon
Imports System.Text
Imports Microsoft.VisualBasic
Public Class KFGPRNT120
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '
    ' 口座チェックリスト印刷
    '
    '
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
#Region " 共通変数定義 "
    Private STR学校名 As String
    Dim strKIGYO_CODE As String
    Dim strFURI_CODE As String
    Dim strGAKKOU_CODE As String
    Dim strKIN_NO As String
    Dim strSIT_NO As String
    Dim strKAMOKU As String
    Dim strKOUZA As String
    Dim strKNAME As String
    Dim strReKNAME As String
    Dim intKEKKA As Integer
    Dim STR帳票ソート順 As String
    Dim Str_Kubn As String
    Dim STR学校コード As String
    Private MainLOG As New CASTCommon.BatchLOG("KFGPRNT120", "学校自振口座チェックリスト印刷画面")
    Private Const msgTitle As String = "学校自振口座チェックリスト印刷画面(KFGPRNT120)"
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
    Private Sub KFGPRNT120_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
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
                MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

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

            If Trim(txtGAKKOU_CODE.Text) = "9999999999" Then
                lab学校名.Text = "全学校対象"
            End If

            LW.ToriCode = Trim(txtGAKKOU_CODE.Text)
            '--------------------------------
            'PR_KOUZAMASTのデータを削除する
            '--------------------------------
            MainDB.BeginTrans()
            If fn_DELETE_PR_KOUZAMAST() = False Then
                MainDB.Rollback()
                Exit Sub
            End If

            '--------------------------------------------------------------------------
            '口座チェックを実行し、チェックに引っかかった明細はPR_KOUZAMASTにインサートする
            '--------------------------------------------------------------------------
            If fn_KOUZA_CHK_MAIN() = False Then
                MainDB.Rollback()
                Return
            End If
            MainDB.Commit()
            OraReader = New MyOracleReader(MainDB)
            SQL.Append(" SELECT DISTINCT GAKKOU_CODE_P")
            SQL.Append(" FROM PR_KOUZAMAST")
            If txtGAKKOU_CODE.Text <> "9999999999" Then
                SQL.Append(" WHERE GAKKOU_CODE_P = " & SQ(txtGAKKOU_CODE.Text))
            End If
            SQL.Append(" ORDER BY GAKKOU_CODE_P")

            '印刷対象存在チェック
            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show("対象データが存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Exit Sub
            End If

            '印刷前確認メッセージ
            If MessageBox.Show(String.Format(MSG0013I, "学校自振口座チェックリスト"), _
                               msgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Return
            End If

            While OraReader.EOF = False
                STR学校コード = OraReader.GetString("GAKKOU_CODE_P")
                '帳票ソート順の取得
                If PFUNC_GAKNAME_GET(False) = False Then
                    STR帳票ソート順 = "0"
                End If

                'ログインID,学校コード,帳票ソート順
                Param = GCom.GetUserID & "," & STR学校コード & "," & STR帳票ソート順
                nRet = ExeRepo.ExecReport("KFGP031.EXE", Param)
                '戻り値に対応したメッセージを表示する
                Select Case nRet
                    Case 0
                    Case Else
                        '印刷失敗メッセージ
                        MessageBox.Show(String.Format(MSG0004E, "学校自振口座チェックリスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                End Select
                OraReader.NextRead()
            End While
            txtGAKKOU_CODE.Focus()
            MessageBox.Show(String.Format(MSG0014I, "学校自振口座チェックリスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(印刷)", "失敗", ex.ToString)
            MainDB.Rollback()
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

    Function fn_KOUZA_CHK_MAIN() As Boolean
        '-------------------------------------------
        '対象の学校の企業コード、振替コードを取得
        '-------------------------------------------
        fn_KOUZA_CHK_MAIN = False
        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Dim OraReader2 As MyOracleReader = Nothing
        Try
            OraReader = New MyOracleReader(MainDB)
            OraReader2 = New MyOracleReader(MainDB)

            SQL.Append("SELECT KIGYO_CODE_T,FURI_CODE_T,GAKKOU_CODE_T")
            SQL.Append(" FROM GAKMAST2")
            If txtGAKKOU_CODE.Text = "9999999999" Then
                SQL.Append(" ORDER BY GAKKOU_CODE_T")
            Else
                SQL.Append(" WHERE GAKKOU_CODE_T = " & SQ(txtGAKKOU_CODE.Text))
            End If

            If OraReader.DataReader(SQL) = False Then
                MessageBox.Show(String.Format(MSG0002E, "検索"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Function
            End If

            While OraReader.EOF = False
                strKIGYO_CODE = Trim(OraReader.GetString("KIGYO_CODE_T"))
                strFURI_CODE = Trim(OraReader.GetString("FURI_CODE_T"))
                strGAKKOU_CODE = OraReader.GetString("GAKKOU_CODE_T")
                '-------------------------------------------
                '口座チェックを実行
                '-------------------------------------------
                SQL = New StringBuilder
                SQL.Append(" SELECT *")
                SQL.Append(" FROM SEITOMAST")
                SQL.Append(" WHERE GAKKOU_CODE_O = " & SQ(strGAKKOU_CODE))
                SQL.Append(" AND  TUKI_NO_O = '01'")
                SQL.Append(" ORDER BY NENDO_O,TUUBAN_O")

                If OraReader2.DataReader(SQL) = True Then
                    While OraReader2.EOF = False
                        strKIN_NO = OraReader2.GetString("TKIN_NO_O")
                        strSIT_NO = OraReader2.GetString("TSIT_NO_O")
                        strKAMOKU = OraReader2.GetString("KAMOKU_O")
                        strKOUZA = OraReader2.GetString("KOUZA_O")
                        strKNAME = Trim(OraReader2.GetString("MEIGI_KNAME_O"))

                        '自金庫コードの場合のみ口座チェック実行
                        If strKIN_NO = STR_JIKINKO_CODE Then
                            If fn_KOUZA_CHK(strKIGYO_CODE, strFURI_CODE, strKIN_NO, strSIT_NO, strKAMOKU, strKOUZA, _
                                            strKNAME, strReKNAME, intKEKKA) = False Then
                                Return False
                            End If
                            '-------------------------------------------------------
                            '口座異常のデータは口座チェックリストマスタにインサートする
                            '-------------------------------------------------------
                            If intKEKKA <> 0 Then
                                If fn_INSERT_PR_KOUZAMAST(OraReader2) = False Then
                                    Return False
                                End If
                            End If
                        End If
                        OraReader2.NextRead()
                    End While
                Else '明細がない場合は次の学校を処理する
                End If
                OraReader.NextRead()
            End While
            fn_KOUZA_CHK_MAIN = True
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(学校検索)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
            If Not OraReader2 Is Nothing Then OraReader2.Close()
        End Try

    End Function

    Function fn_KOUZA_CHK(ByVal astrKIGYO_CODE As String, ByVal astrFURI_CODE As String, ByVal astrKIN_NO As String, ByVal astrSIT_NO As String, _
                          ByVal astrKAMOKU As String, ByVal astrKOUZA As String, ByVal astrKNAME As String, ByRef astrReKNAME As String, _
                          ByRef aintKEKKA As Integer) As Boolean
        '============================================================================
        'NAME           :fn_KOUZA_CHK
        'Parameter      :astrKIGYO_CODE：企業コード／astrFURI_CODE：振替コード／astrKIN_NO：金融機関コード
        '               :astrSIT_NO：支店コード／astrKAMOKU：科目コード／astrKOUZA：口座番号
        '　　　　　　　　:astrKNAME: カナ氏名／astrReKNAME：元帳カナ氏名（一致しなかった場合値を返す）
        '　　　　　　　　:aintKEKKA: 口座チェックの結果(0=自振契約ありカナ一致,1=契約ありカナ不一致,
        '　　　　　　　　:　　　　　　　　　　　　　　　2=契約なしカナ一致,3=契約なしカナ不一致,4=口座なし)
        'Description    :口座チェックの実行
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2005/04/14
        'UPDATE         :
        '============================================================================
        fn_KOUZA_CHK = False
        aintKEKKA = 0
        astrReKNAME = ""

        ' 2017/05/09 タスク）綾部 ADD 【OM】(RSV2対応 機能追加) -------------------- START
        Dim RSKJ_G_KNAMECHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_KNAMECHK")
        Dim RSKJ_G_JIFURICHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "G_JIFURICHK")
        ' 2017/05/09 タスク）綾部 ADD 【OM】(RSV2対応 機能追加) -------------------- END

        Dim SQL As New StringBuilder
        Dim OraReader As MyOracleReader = Nothing
        Try
            ' 2017/05/09 タスク）綾部 CHG 【OM】(RSV2対応 機能追加) -------------------- START
            'OraReader = New MyOracleReader(MainDB)
            ''企業コード、振替コードが一致するものがあるかどうか検索
            'SQL.Append(" SELECT * FROM KDBMAST")
            'SQL.Append(" WHERE TSIT_NO_D =" & SQ((Trim(astrSIT_NO))))
            'SQL.Append(" AND KAMOKU_D =" & SQ(astrKAMOKU))
            'SQL.Append(" AND KOUZA_D =" & SQ(Trim(astrKOUZA)))
            'SQL.Append(" AND KIGYOU_CODE_D =" & SQ(astrKIGYO_CODE))
            'SQL.Append(" AND FURI_CODE_D =" & SQ(astrFURI_CODE))

            'If OraReader.DataReader(SQL) = True Then
            '    If Trim(astrKNAME) = Trim(OraReader.GetString("KOKYAKU_KNAME_D")) Then
            '        aintKEKKA = 0
            '    Else
            '        aintKEKKA = 1
            '        astrReKNAME = Trim(OraReader.GetString("KOKYAKU_KNAME_D"))
            '    End If
            '    Return True
            'End If

            ''企業コード、振替コードが一致するものがなかったとき、口座が一致するものがあるか検索
            'SQL = New StringBuilder
            'SQL.Append(" SELECT * FROM KDBMAST")
            'SQL.Append(" WHERE TSIT_NO_D =" & SQ(Trim(astrSIT_NO)))
            'SQL.Append(" AND KAMOKU_D =" & SQ(astrKAMOKU))
            'SQL.Append(" AND KOUZA_D =" & SQ(Trim(astrKOUZA)))

            'If OraReader.DataReader(SQL) = True Then
            '    If Mid(Trim(astrKNAME), 1, 40) = Mid(Trim(OraReader.GetString("KOKYAKU_KNAME_D")), 1, 40) Then
            '        aintKEKKA = 2
            '    Else
            '        aintKEKKA = 3
            '        astrReKNAME = Mid(Trim(OraReader.GetString("KOKYAKU_KNAME_D")), 1, 40)
            '    End If
            'Else
            '    aintKEKKA = 4
            'End If
            'fn_KOUZA_CHK = True
            '-------------------------------------------------------------------------
            ' 口座が存在するかどうか確認し、各情報を取得する
            '-------------------------------------------------------------------------
            OraReader = New MyOracleReader(MainDB)
            SQL.Length = 0
            SQL.Append(" SELECT * FROM KDBMAST")
            SQL.Append(" WHERE ")
            SQL.Append("     TSIT_NO_D =" & SQ(Trim(astrSIT_NO)))
            SQL.Append(" AND KAMOKU_D  =" & SQ(astrKAMOKU))
            SQL.Append(" AND KOUZA_D   =" & SQ(Trim(astrKOUZA)))
            '2017/06/23 saitou ADD 標準版修正 ---------------------------------------- START
            '口座解約済みが出力されないため活口座フラグでソートする
            SQL.Append(" ORDER BY KATU_KOUZA_D DESC")
            '2017/06/23 saitou ADD 標準版修正 ---------------------------------------- END

            '-------------------------------------------------------------
            ' 初期値設定[0:自振契約あり(※帳票出力なし)]
            '-------------------------------------------------------------
            aintKEKKA = 0

            If OraReader.DataReader(SQL) = True Then
                '2017/06/23 saitou UPD 標準版修正 ---------------------------------------- START
                '口座解約済みのチェック漏れと口座があってもループしていないので、
                '全て自振契約なしになってしまうのを修正。
                Dim KigyoCode As String = ""
                Dim FuriCode As String = ""
                Dim KDBKName As String = ""

                If OraReader.GetString("KATU_KOUZA_D") = "0" Then
                    ' [8:口座解約済み]
                    aintKEKKA = 8
                Else
                    While OraReader.EOF = False
                        KigyoCode = OraReader.GetString("KIGYOU_CODE_D")
                        FuriCode = OraReader.GetString("FURI_CODE_D")
                        KDBKName = OraReader.GetString("KOKYAKU_KNAME_D")

                        If RSKJ_G_JIFURICHK = "YES" Then
                            '-------------------------------------------------------------
                            ' 自振契約（企業コード・振替コード）のチェックを行う
                            '-------------------------------------------------------------
                            If astrKIGYO_CODE = KigyoCode And _
                                       astrFURI_CODE = FuriCode Then
                                aintKEKKA = 0
                                Exit While
                            End If

                            ' [1:自振契約なし]
                            aintKEKKA = 1
                        Else
                            Exit While
                        End If

                        OraReader.NextRead()
                    End While

                    If RSKJ_G_KNAMECHK = "YES" Then
                        '-----------------------------------------------------
                        ' カナ名チェックを行う
                        '-----------------------------------------------------
                        If Trim(astrKNAME) <> Trim(KDBKName) Then
                            Select Case aintKEKKA
                                Case 1
                                    ' [2:自振契約なしカナ不一致]
                                    aintKEKKA = 2
                                    astrReKNAME = Trim(KDBKName)
                                Case Else
                                    ' [3:自振契約ありカナ不一致]
                                    aintKEKKA = 3
                                    astrReKNAME = Trim(KDBKName)
                            End Select
                        End If
                    End If
                End If

                'Select Case RSKJ_G_JIFURICHK
                '    Case "YES"
                '        '-------------------------------------------------------------
                '        ' 自振契約（企業コード・振替コード）のチェックを行う
                '        '-------------------------------------------------------------
                '        Dim KigyoCode As String = OraReader.GetString("KIGYOU_CODE_D")
                '        Dim FuriCode As String = OraReader.GetString("FURI_CODE_D")
                '        If astrKIGYO_CODE = KigyoCode And _
                '                   astrFURI_CODE = FuriCode Then
                '        Else
                '            ' [1:自振契約なし]
                '            aintKEKKA = 1
                '        End If

                '        Select Case RSKJ_G_KNAMECHK
                '            Case "YES"
                '                '-----------------------------------------------------
                '                ' カナ名チェックを行う
                '                '-----------------------------------------------------
                '                Dim KDBKName As String = OraReader.GetString("KOKYAKU_KNAME_D")
                '                If Trim(astrKNAME) <> Trim(KDBKName) Then
                '                    Select Case aintKEKKA
                '                        Case 1
                '                            ' [2:自振契約なしカナ不一致]
                '                            aintKEKKA = 2
                '                            astrReKNAME = Trim(KDBKName)
                '                        Case Else
                '                            ' [3:自振契約ありカナ不一致]
                '                            aintKEKKA = 3
                '                            astrReKNAME = Trim(KDBKName)
                '                    End Select
                '                End If
                '        End Select
                '    Case Else
                '        '-------------------------------------------------------------
                '        ' 自振契約（企業コード・振替コード）のチェックを行わない
                '        '-------------------------------------------------------------
                '        Select Case RSKJ_G_KNAMECHK
                '            Case "YES"
                '                '-----------------------------------------------------
                '                ' カナ名チェックを行う
                '                '-----------------------------------------------------
                '                Dim KDBKName As String = OraReader.GetString("KOKYAKU_KNAME_D")
                '                If Trim(astrKNAME) <> Trim(KDBKName) Then
                '                    ' [4:カナ不一致]
                '                    aintKEKKA = 4
                '                    astrReKNAME = Trim(KDBKName)
                '                End If
                '        End Select
                'End Select
                '2017/06/23 saitou UPD 標準版修正 ---------------------------------------- END
            Else
                '-----------------------------------------------------
                ' 口座該当なしの場合は、9(口座なし)
                '-----------------------------------------------------
                aintKEKKA = 9
            End If

            Return True
            ' 2017/05/09 タスク）綾部 CHG 【OM】(RSV2対応 機能追加) -------------------- END

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(口座チェック)", "失敗", ex.ToString)
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function
    Function fn_INSERT_PR_KOUZAMAST(ByVal WorkReader As MyOracleReader) As Boolean
        '============================================================================
        'NAME           :fn_INSERT_PR_KOUZAMAST
        'Parameter      :
        'Description    :口座チェックリストマスタへのインサート
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2005/04/14
        'UPDATE         :
        '============================================================================
        fn_INSERT_PR_KOUZAMAST = False

        Dim SQL As New StringBuilder
        Try
            SQL.Append(" INSERT INTO PR_KOUZAMAST VALUES(")
            SQL.Append(SQ(strGAKKOU_CODE))                              '学校コード
            SQL.Append("," & WorkReader.GetString("NENDO_O"))           '入学年度
            SQL.Append("," & WorkReader.GetString("TUUBAN_O"))          '通番
            SQL.Append("," & WorkReader.GetString("GAKUNEN_CODE_O"))    '学年
            SQL.Append("," & WorkReader.GetString("CLASS_CODE_O"))      'クラス
            SQL.Append("," & SQ(WorkReader.GetString("SEITO_NO_O")))    '生徒番号
            SQL.Append("," & SQ(strKIN_NO))                             '金融機関コード
            SQL.Append("," & SQ(strSIT_NO))                             '支店コード
            SQL.Append("," & SQ(strKAMOKU))                             '科目コード
            SQL.Append("," & SQ(strKOUZA))                              '口座番号
            SQL.Append("," & SQ(strKNAME))                              '名義人カナ氏名
            SQL.Append("," & SQ(strReKNAME))                            '元帳カナ氏名
            SQL.Append("," & SQ(strKIGYO_CODE))                         '企業コード
            SQL.Append("," & SQ(strFURI_CODE))                          '振替コード
            SQL.Append("," & SQ(intKEKKA))                              '結果
            SQL.Append(")")

            If MainDB.ExecuteNonQuery(SQL) <> 1 Then
                Throw New Exception("ワークマスタ挿入失敗")
            End If
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ワークマスタ作成)", "失敗", ex.ToString)
            Return False
        End Try

        fn_INSERT_PR_KOUZAMAST = True
    End Function
    Function fn_DELETE_PR_KOUZAMAST() As Boolean
        '============================================================================
        'NAME           :fn_DELETE_PR_KOUZAMAST
        'Parameter      :
        'Description    :口座チェックリストマスタの全データを削除する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2005/04/14
        'UPDATE         :
        '============================================================================
        fn_DELETE_PR_KOUZAMAST = False
        Try
            Dim SQL As New StringBuilder
            SQL.Append("DELETE FROM PR_KOUZAMAST")
            MainDB.ExecuteNonQuery(SQL)
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ワークマスタ削除)", "失敗", ex.ToString)
            Return False
        End Try
        fn_DELETE_PR_KOUZAMAST = True
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
