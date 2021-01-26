Option Explicit On 
Option Strict Off

Imports System.IO
Imports System.Text

Public Class KFGMAIN020

#Region " 共通変数定義 "

    Private Str_Select_Nyusyutu_Code As String
    Private Str_Zengin_FileName As String
    Private Str_Seikyu_Nentuki As String

    Private Str_Gakkou_Code As String
    Private Str_Sort_Kbn As String

    Private Str_Meisai_kbn As String

    Private Str_Syori_Date(1) As String

    Private Str_Jikkou_Date As String
    Private Str_Saifuri_Flg As String
    Private Str_Sch_Flg(1) As String
    Private Str_Funou_YDate(1) As String
    Private Str_Baitai_Code As String

    Private Str_Gakkou_Name As String
    Private Str_Gakkou_Name_MSG As String = "" '更新ダイアログ表記用学校名 2006/10/12

    Private Str_Syori_Ginko(,) As String

    Private Lng_RecordNo As Long

    Private Lng_Ijyo_Count As Long
    Private Lng_Err_Count As Long

    Private Lng_Trw_Count As Long

    Private Str_Ginko(1) As String

    Private Str_Tako(2) As String

    Private Str_WHERE As String

    'エラーリスト作成時受渡用パラメータ
    Private Int_Err_Gakunen_Code As Integer
    Private Int_Err_Class_Code As Integer
    Private Str_Err_Seito_No As String
    Private Int_Err_Tuuban As Integer
    Private Str_Err_Itaku_Name As String
    Private Str_Err_Tkin_No As String
    Private Str_Err_Tsit_No As String
    Private Str_Err_Kamoku As String
    Private Str_Err_Kouza As String
    Private Str_Err_Keiyaku_No As String
    Private Str_Err_Keiyaku_Name As String
    Private Lng_Err_Furikae_Kingaku As Long
    Private Str_Err_Msg As String

    '追加 2006/10/05
    Private lngGAK_SYORI_KEN(10) As Long
    Private dblGAK_SYORI_KIN(10) As Double

    Private iGakunen_Flag() As Integer
    Private lngTAKOU_SYORISAKI As Long = 0 '他行データ(振替ﾃﾞｰﾀファイル)を作成した数 2006/10/06

#End Region

#Region " Form Load "
    Private Sub KFGMAIN020_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim str_Format_Furikae_Date As String

        With Me
            .WindowState = FormWindowState.Normal
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .ControlBox = True
        End With

        STR_SYORI_NAME = "他行分データ作成"
        STR_COMMAND = "Form_Load"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = ""

        Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
        MainLog = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

        '学校コンボ設定（全学校）
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME, True) = False Then
            Call GSUB_LOG(0, "コンボボックス設定(cmbGAKKOUNAME)")

            Call GSUB_MESSAGE_WARNING("学校名コンボボックス設定でエラーが発生しました")

            Exit Sub
        End If

        str_Format_Furikae_Date = fn_GetEigyoubi(Format(Now, "yyyyMMdd"), STR_JIFURI_HAISIN, "+")

        txtFuriDateY.Text = Mid(str_Format_Furikae_Date, 1, 4)
        txtFuriDateM.Text = Mid(str_Format_Furikae_Date, 5, 2)
        txtFuriDateD.Text = Mid(str_Format_Furikae_Date, 7, 2)

        cmbNyusyutu.SelectedIndex = 0

        'Oracle 接続(Read専用)
        OBJ_CONNECTION_DREAD = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read専用)
        OBJ_CONNECTION_DREAD.Open()

        'Oracle 接続(Read専用)
        OBJ_CONNECTION_DREAD2 = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read専用)
        OBJ_CONNECTION_DREAD2.Open()

        'Oracle 接続(Read専用)
        OBJ_CONNECTION_DREAD3 = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read専用)
        OBJ_CONNECTION_DREAD3.Open()
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click

        Cursor.Current = Cursors.WaitCursor()
        Dim strDIR As String
        strDIR = CurDir()

        Dim lCount As Long

        Str_Syori_Date(0) = Format(Now, "yyyyMMdd")
        Str_Syori_Date(1) = Format(Now, "yyyyMMddHHmmss")

        '入力チェック
        If PFUNC_Nyuryoku_Check() = False Then
            ChDir(strDIR)
            Exit Sub
        End If

        STR_COMMAND = "実行"
        STR_LOG_GAKKOU_CODE = txtGAKKOU_CODE.Text
        STR_LOG_FURI_DATE = STR_FURIKAE_DATE(1)

        Lng_Ijyo_Count = 0

        '処理他行情報取得
        If PFUNC_Get_Takou_Ginko(Str_Syori_Ginko) = False Then
            txtGAKKOU_CODE.Focus()
            txtGAKKOU_CODE.SelectionStart = 0
            txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length
            ChDir(strDIR)
            Exit Sub
        End If

        '確認メッセージ
        If GFUNC_MESSAGE_QUESTION("作成しますか？") <> vbOK Then
            ChDir(strDIR)
            Exit Sub
        End If

        '異常リストマスタ削除
        STR_SQL = " DELETE  FROM G_IJYOLIST"

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Sub
        End If
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Sub
        End If
        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Sub
        End If

        Str_Jikkou_Date = Format(Now, "yyyyMMddHHmmss")

        ReDim lngGAK_SYORI_KEN(10)
        ReDim dblGAK_SYORI_KIN(10)
        lngTAKOU_SYORISAKI = 0
        '支店コードを渡さない形に修正
        Select Case CInt(Str_Select_Nyusyutu_Code)
            Case 0
                '初振
                '口座振替明細マスタ削除(初振のみ)
                If PFUNC_Delete_Meisai(Str_Gakkou_Code, 0) = False Then
                    ChDir(strDIR)

                    Exit Sub
                End If

                '存在する他行数分全銀ﾌｧｲﾙ作成(明細マスタ登録)処理を実行
                For lCount = 1 To UBound(Str_Syori_Ginko, 2)

                    Call PSUB_Insert_Meisai1(Str_Syori_Ginko(0, lCount), _
                                             Str_Syori_Ginko(2, lCount), _
                                             Format(CInt(Str_Syori_Ginko(3, lCount)), "0"), _
                                             Str_Syori_Ginko(4, lCount), _
                                             Str_Syori_Ginko(5, lCount), _
                                             Str_Syori_Ginko(6, lCount))

                    Lng_Ijyo_Count += Lng_Err_Count
                Next lCount
            Case 1
                '再振
                '口座振替明細マスタ削除(再振のみ)
                If PFUNC_Delete_Meisai(Str_Gakkou_Code, 1) = False Then
                    ChDir(strDIR)

                    Exit Sub
                End If

                '存在する他行数分全銀ﾌｧｲﾙ作成(明細マスタ登録)処理を実行
                For lCount = 1 To UBound(Str_Syori_Ginko, 2)
                    Call PSUB_Insert_Meisai2(Str_Syori_Ginko(0, lCount), _
                           Str_Syori_Ginko(2, lCount), _
                           Format(CInt(Str_Syori_Ginko(3, lCount)), "0"), _
                           Str_Syori_Ginko(4, lCount), _
                           Str_Syori_Ginko(5, lCount), _
                           Str_Syori_Ginko(6, lCount))
                    Lng_Ijyo_Count += Lng_Err_Count
                Next lCount
        End Select

        If Lng_Ijyo_Count = 0 Or Lng_Ijyo_Count = Lng_Trw_Count Then
            ''完了メッセージ
            Call GSUB_LOG(1, "他行データ作成")
            If lngTAKOU_SYORISAKI = 0 Then
                Call GSUB_MESSAGE_INFOMATION("他行データ作成対象先がありません")
            Else
                Call GSUB_MESSAGE_INFOMATION("作成しました" & vbCrLf & "作成他行数" & lngTAKOU_SYORISAKI & " 先")

            End If

        Else
            '異常リスト印刷
            Call GSUB_PRINT_CRYSTALREPORT("IJYO.RPT", "")

            Call GSUB_LOG(0, "他行データ作成")
            If lngTAKOU_SYORISAKI = 0 Then
                Call GSUB_MESSAGE_INFOMATION("他行データ作成対象先がありません")
            Else
                Call GSUB_MESSAGE_INFOMATION("作成しました" & vbCrLf & "作成他行数" & lngTAKOU_SYORISAKI & " 先")
            End If
        End If
        ChDir(strDIR)

        btnEnd.Focus()

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        'Oracle CLOSE
        OBJ_CONNECTION_DREAD.Close()
        OBJ_CONNECTION_DREAD = Nothing

        'Oracle CLOSE
        OBJ_CONNECTION_DREAD2.Close()
        OBJ_CONNECTION_DREAD2 = Nothing

        'Oracle CLOSE
        OBJ_CONNECTION_DREAD3.Close()
        OBJ_CONNECTION_DREAD3 = Nothing

        Me.Close()
    End Sub
#End Region

#Region " Private Sub "
    Private Sub PSUB_Insert_Meisai1(ByVal pGinko_Code As String, ByVal pItaku_Code As String, ByVal pKamoku As String, ByVal pKouza As String, ByVal pFileName As String, ByVal pCodeKbn As String)

        Dim lRecordCount As Long
        Dim lFurikae_Kingaku As Long
        Dim lTotal_Kensuu As Long
        Dim lTotal_Kingaku As Long

        Dim iLcount As Integer
        Dim iFileNo As Integer

        Dim bFlg As Boolean
        Dim bLoopFlg As Boolean

        Dim sJyuyouka_No As String
        Dim sBuff As String
        Dim sZenginFile As String

        Dim lSyukei(1) As Long

        Dim lThrrowCount As Long


        'スケジュールが年間、特別から
        Select Case PFUNC_Get_Gakunen(Str_Gakkou_Code, iGakunen_Flag)
            Case -1
                'エラー
                Call GSUB_LOG(0, "指定学年取得でエラーが発生しました")

                Exit Sub
            Case -2
                'エラー
                Call GSUB_LOG(0, Str_Gakkou_Code & ":該当学年が0件です")
                Call GSUB_MESSAGE_WARNING(Str_Gakkou_Code & ":指定した学校のスケジュールに作成対象データは存在しません")
                txtFuriDateY.Focus()
                txtFuriDateY.SelectionStart = 0
                txtFuriDateY.SelectionLength = txtFuriDateY.Text.Length
                Exit Sub

            Case 0
                '全学年が対象
                bFlg = False
            Case Else
                '特定学年のみが対象
                bFlg = True
        End Select

        '全銀ファイル作成
        sZenginFile = Str_Zengin_FileName & Str_Gakkou_Code & pGinko_Code & ".dat"

        If Dir$(sZenginFile) <> "" Then Kill(sZenginFile)

        'コピー先ファイル名
        If Dir(pFileName) <> "" Then Kill(pFileName)

        iFileNo = FreeFile()
        Err.Number = 0

        FileOpen(iFileNo, sZenginFile, OpenMode.Random, , , 120)    'ワークファイル

        If Err.Number <> 0 Then
            Call GSUB_LOG(0, "全銀ﾌｧｲﾙOPENエラー")

            Exit Sub
        End If

        '銀行名取得
        Call PSUB_GET_TAKONAME(pGinko_Code)

        'ヘッダデータ作成
        With gZENGIN_REC1
            .ZG1 = "1"
            .ZG2 = "91"
            If pCodeKbn = "0" Then 'JIS指定
                .ZG3 = "0"
            Else 'EBCDIC
                .ZG3 = "1"
            End If
            .ZG4 = pItaku_Code
            .ZG5 = Str_Gakkou_Name
            .ZG6 = Mid(STR_FURIKAE_DATE(1), 5, 4)
            .ZG7 = pGinko_Code
            .ZG8 = Str_Tako(0)
            .ZG9 = Str_Tako(1)
            .ZG10 = Str_Tako(2)
            .ZG11 = pKamoku
            .ZG12 = pKouza
            .ZG13 = Space(17)
        End With

        Call PSUB_DAT_ZENGIN_WRITE(iFileNo, 1)

        '生徒マスタ取得
        STR_SQL = " SELECT "
        STR_SQL += " SEITOMASTVIEW.*"
        STR_SQL += ", ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V , BAITAI_CODE_V"
        STR_SQL += ", TKIN_NO_T , TSIT_NO_T  , KAMOKU_T , KOUZA_T "
        STR_SQL += " FROM SEITOMASTVIEW , GAKMAST2 , G_TAKOUMAST"
        STR_SQL += " WHERE GAKKOU_CODE_O = GAKKOU_CODE_T"
        STR_SQL += " AND GAKKOU_CODE_O = GAKKOU_CODE_V"
        STR_SQL += " AND TKIN_NO_O = TKIN_NO_V"
        STR_SQL += " AND GAKKOU_CODE_O = '" & Str_Gakkou_Code & "'"
        STR_SQL += " AND TUKI_NO_O = '" & Mid(Str_Seikyu_Nentuki, 5, 2) & "'"
        STR_SQL += " AND TKIN_NO_O = '" & pGinko_Code & "'"
        STR_SQL += " AND FURIKAE_O = '0'"
        STR_SQL += " AND KAIYAKU_FLG_O <> '9'"
        If bFlg = True Then
            STR_SQL += " AND ("
            For iLcount = 1 To 9
                If iGakunen_Flag(iLcount) = 1 Then
                    If bLoopFlg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_O=" & iLcount
                    bLoopFlg = True
                End If
            Next iLcount
            STR_SQL += " )"
        End If
        '振替ﾃﾞｰﾀは金庫・店番・科目・口座番号・学年(降順)・請求月 2006/10/04
        STR_SQL += " ORDER BY TKIN_NO_O ASC, TSIT_NO_O ASC, KAMOKU_O ASC, KOUZA_O ASC, GAKUNEN_CODE_O DESC"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Sub
        End If

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Sub
        End If

        lRecordCount = 1
        lTotal_Kensuu = 0
        lTotal_Kingaku = 0
        Lng_Err_Count = 0

        lThrrowCount = 0

        While (OBJ_DATAREADER_DREAD.Read = True)
            '媒体コード取得 2006/10/11
            Str_Baitai_Code = OBJ_DATAREADER_DREAD.Item("BAITAI_CODE_V")


            '再振種別が繰越ありの場合、不能分のデータも当月の初振分として追加する（処理中の金融機関の生徒のみ）
            Select Case CInt(Str_Saifuri_Flg)
                Case 2, 3
                    '振替不能　口座振替明細データ作成
                    STR_SQL = " SELECT "
                    STR_SQL += " G_MEIMAST.*"
                    STR_SQL += ", ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V"
                    STR_SQL += " FROM G_MEIMAST, G_TAKOUMAST"
                    STR_SQL += " WHERE GAKKOU_CODE_M = '" & Str_Gakkou_Code & "'"
                    STR_SQL += " AND GAKKOU_CODE_M = GAKKOU_CODE_V"
                    STR_SQL += " AND FURIKETU_CODE_M <> 0"
                    STR_SQL += " AND SAIFURI_SUMI_M = '0'"
                    STR_SQL += " AND TKIN_NO_M = '" & OBJ_DATAREADER_DREAD.Item("TKIN_NO_O") & "'"
                    STR_SQL += " AND SEIKYU_KIN_M > 0"
                    STR_SQL += " AND TKIN_NO_M = TKIN_NO_V"
                    STR_SQL += " AND NENDO_M = '" & OBJ_DATAREADER_DREAD.Item("NENDO_O") & "'"
                    STR_SQL += " AND TUUBAN_M = " & OBJ_DATAREADER_DREAD.Item("TUUBAN_O")
                    '振替ﾃﾞｰﾀは金庫・店番・科目・口座番号・学年(降順)・請求月 2006/10/04
                    STR_SQL += " ORDER BY TKIN_NO_M ASC, TSIT_NO_M ASC, TKAMOKU_M ASC, TKOUZA_M ASC, SEIKYU_TUKI_M ASC,GAKUNEN_CODE_M DESC"

                    If GFUNC_SELECT_SQL4(STR_SQL, 0) = False Then
                        Exit Sub
                    End If

                    While (OBJ_DATAREADER_DREAD2.Read = True)
                        With OBJ_DATAREADER_DREAD2
                            Int_Err_Gakunen_Code = .Item("GAKUNEN_CODE_M")
                            Int_Err_Class_Code = .Item("CLASS_CODE_M")
                            Str_Err_Seito_No = .Item("SEITO_NO_M")
                            Int_Err_Tuuban = .Item("TUUBAN_M")
                            Str_Err_Itaku_Name = ""
                            Str_Err_Tkin_No = .Item("TKIN_NO_M")
                            Str_Err_Tsit_No = .Item("TSIT_NO_M")
                            Str_Err_Kamoku = .Item("TKAMOKU_M")
                            Str_Err_Kouza = .Item("TKOUZA_M")
                            Str_Err_Keiyaku_No = ""
                            Str_Err_Keiyaku_Name = .Item("TMEIGI_KNM_M")
                            Lng_Err_Furikae_Kingaku = .Item("SEIKYU_KIN_M")

                            '前回振替全銀データ
                            sBuff = CStr(.Item("FURI_DATA_M"))

                            '需要家番号
                            sJyuyouka_No = .Item("NENDO_M") & Format(CInt(.Item("TUUBAN_M")), "0000") & Mid(.Item("SEIKYU_TUKI_M"), 5, 2)

                            '全銀データ作成(明細)
                            'データ区分(=2)
                            '被仕向銀行番号
                            '被仕向銀行名　
                            '被仕向支店番号
                            '被仕向支店名
                            '手形交換所番号
                            '預金種目
                            '口座番号
                            '受取人
                            '振込金額
                            '新規コード
                            '顧客コード１
                            '顧客コード２
                            '振込指定区分
                            'ダミー
                            With gZENGIN_REC2
                                .ZG1 = Mid(sBuff, 1, 1)
                                .ZG2 = Mid(sBuff, 2, 4)
                                .ZG3 = Mid(sBuff, 6, 15)
                                .ZG4 = Mid(sBuff, 21, 3)
                                .ZG5 = Mid(sBuff, 24, 15)
                                .ZG6 = Mid(sBuff, 39, 4)
                                .ZG7 = Mid(sBuff, 43, 1)
                                .ZG8 = Mid(sBuff, 44, 7)
                                .ZG9 = Mid(sBuff, 51, 30)
                                .ZG10 = Mid(sBuff, 81, 10)
                                .ZG11 = Mid(sBuff, 91, 1)
                                .ZG12 = sJyuyouka_No
                                .ZG13 = "0" & Space(9)      'スケジュール区分を設定
                                .ZG14 = Mid(sBuff, 112, 1)
                                .ZG15 = Mid(sBuff, 113, 8)
                            End With

                            Call PSUB_DAT_ZENGIN_WRITE(iFileNo, 2)

                            '口座振替明細データ作成
                            STR_SQL = " INSERT INTO G_MEIMAST"
                            STR_SQL += " values("
                            STR_SQL += "'" & Str_Gakkou_Code & "'"
                            STR_SQL += ",'" & .Item("NENDO_M") & "'"
                            STR_SQL += ",'" & STR_FURIKAE_DATE(1) & "'"
                            STR_SQL += "," & .Item("GAKUNEN_CODE_M")
                            STR_SQL += "," & .Item("CLASS_CODE_M")
                            STR_SQL += ",'" & .Item("SEITO_NO_M") & "'"
                            STR_SQL += "," & .Item("TUUBAN_M")
                            STR_SQL += ",'" & .Item("ITAKU_KIN_M") & "'"
                            STR_SQL += ",'" & .Item("ITAKU_SIT_M") & "'"
                            STR_SQL += ",'" & .Item("ITAKU_KAMOKU_M") & "'"
                            STR_SQL += ",'" & .Item("ITAKU_KOUZA_M") & "'"
                            STR_SQL += ",'" & .Item("TKIN_NO_M") & "'"
                            STR_SQL += ",'" & .Item("TSIT_NO_M") & "'"
                            STR_SQL += ",'" & .Item("TKAMOKU_M") & "'"
                            STR_SQL += ",'" & .Item("TKOUZA_M") & "'"
                            STR_SQL += ",'" & .Item("TMEIGI_KNM_M") & "'"
                            'STR_SQL += ",'" & Mid(Str_Seikyu_Nentuki, 5, 2) & "ｶﾞﾂﾄﾞ'"
                            STR_SQL += ",'" & Mid(.Item("SEIKYU_TUKI_M"), 5, 2) & "ｶﾞﾂﾄﾞ'"
                            STR_SQL += ",'" & sJyuyouka_No & "0" & Space(9) & "'" '区分追加 2006/12/25
                            STR_SQL += ",'" & sBuff & "'"
                            STR_SQL += "," & lRecordCount
                            STR_SQL += ",'" & .Item("SEIKYU_TUKI_M") & "'"
                            STR_SQL += ",'" & Str_Seikyu_Nentuki & "'"
                            STR_SQL += ",'" & .Item("HIMOKU_ID_M") & "'"
                            STR_SQL += "," & .Item("SEIKYU_KIN_M")
                            For iLcount = 1 To 15
                                STR_SQL += "," & .Item("HIMOKU" & iLcount & "_KIN_M")
                            Next iLcount
                            STR_SQL += ",0"
                            '再振作成済フラグを初期登録時は未入力だったのを0:未作成で
                            '作成する
                            'STR_SQL += ",' '"
                            STR_SQL += ",'0'"
                            STR_SQL += ",'0'"
                            STR_SQL += ",'" & Str_Syori_Date(1) & "'"
                            STR_SQL += ",' '"
                            STR_SQL += ",' '"
                            STR_SQL += ")"

                            Select Case PFUNC_Check_Meisai()
                                Case -1
                                    'エラー有(異常リストに追加 , 明細に登録なし , スケジュール更新なし)
                                    Lng_Err_Count += 1

                                    Call PSUB_Insert_IjyoList()
                                Case 0
                                    '正常終了
                                    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                                        '異常リスト追加
                                        Lng_Err_Count += 1
                                        Str_Err_Msg = "データベースエラーです"

                                        Call PSUB_Insert_IjyoList()
                                    Else
                                        lTotal_Kingaku += .Item("SEIKYU_KIN_M")
                                        lTotal_Kensuu += 1

                                        lRecordCount += 1

                                        '学年毎の件数取得 2006/10/05
                                        lngGAK_SYORI_KEN(CInt(.Item("GAKUNEN_CODE_M"))) += 1
                                        dblGAK_SYORI_KIN(CInt(.Item("GAKUNEN_CODE_M"))) += .Item("SEIKYU_KIN_M")

                                    End If
                                Case Else
                                    'エラー有(異常リストに追加 , 明細に登録あり , スケジュール更新あり)
                                    lThrrowCount += 1
                                    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                                        '異常リスト追加
                                        Lng_Err_Count += 1
                                        Str_Err_Msg = "データベースエラーです"

                                        Call PSUB_Insert_IjyoList()
                                    Else
                                        lTotal_Kingaku += .Item("SEIKYU_KIN_M")
                                        lTotal_Kensuu += 1

                                        lRecordCount += 1
                                    End If

                                    Lng_Err_Count += 1

                                    Call PSUB_Insert_IjyoList()
                            End Select

                        End With
                    End While

                    If GFUNC_SELECT_SQL4(STR_SQL, 1) = False Then
                        Exit Sub
                    End If
            End Select

            With OBJ_DATAREADER_DREAD

                lFurikae_Kingaku = 0
                '請求金額
                For iLcount = 1 To 15                                                                             '振替金額合計
                    If Not IsDBNull(.Item("KINGAKU" & Format(iLcount, "00") & "_O")) Then
                        lFurikae_Kingaku += .Item("KINGAKU" & Format(iLcount, "00") & "_O")
                    End If
                Next iLcount

                If lFurikae_Kingaku = 0 Then '振替金額0円の生徒はデータ作成しない 2006/10/05
                    GoTo NEXT_SEITO
                End If

                Int_Err_Gakunen_Code = .Item("GAKUNEN_CODE_O")
                Int_Err_Class_Code = .Item("CLASS_CODE_O")
                Str_Err_Seito_No = .Item("SEITO_NO_O")
                Int_Err_Tuuban = .Item("TUUBAN_O")
                Str_Err_Itaku_Name = ""
                Str_Err_Tkin_No = .Item("TKIN_NO_O")
                Str_Err_Tsit_No = .Item("TSIT_NO_O")
                Str_Err_Kamoku = .Item("KAMOKU_O")
                Str_Err_Kouza = .Item("KOUZA_O")
                Str_Err_Keiyaku_No = ""
                Str_Err_Keiyaku_Name = Mid(OBJ_DATAREADER_DREAD.Item("MEIGI_KNAME_O"), 1, 30)

                Lng_Err_Furikae_Kingaku = lFurikae_Kingaku

                '需要家番号
                sJyuyouka_No = .Item("NENDO_O") & Format(CInt(.Item("TUUBAN_O")), "0000") & Mid(Str_Seikyu_Nentuki, 5, 2)

                '銀行名取得
                Call PSUB_GET_GINKONAME(OBJ_DATAREADER_DREAD.Item("TKIN_NO_O"), OBJ_DATAREADER_DREAD.Item("TSIT_NO_O"))

                '全銀データ作成(明細)

                'データ区分(=2)
                '被仕向銀行番号
                '被仕向銀行名　
                '被仕向支店番号
                '被仕向支店名
                '手形交換所番号
                '預金種目
                '口座番号
                '受取人
                '振込金額
                '新規コード
                '顧客コード１
                '顧客コード２
                '振込指定区分
                'ダミー
                With gZENGIN_REC2
                    .ZG1 = "2"
                    .ZG2 = Format(CInt(OBJ_DATAREADER_DREAD.Item("TKIN_NO_O")), "0000")
                    .ZG3 = Str_Ginko(0)
                    .ZG4 = Format(CInt(OBJ_DATAREADER_DREAD.Item("TSIT_NO_O")), "000")
                    .ZG5 = Str_Ginko(1)
                    .ZG6 = Space(4)
                    .ZG7 = Format(CInt(OBJ_DATAREADER_DREAD.Item("KAMOKU_O")), "0")
                    .ZG8 = Format(CInt(OBJ_DATAREADER_DREAD.Item("KOUZA_O")), "0000000")
                    .ZG9 = Mid(OBJ_DATAREADER_DREAD.Item("MEIGI_KNAME_O"), 1, 30)
                    .ZG10 = Format(CInt(lFurikae_Kingaku), "0000000000")
                    .ZG11 = "0"
                    .ZG12 = sJyuyouka_No
                    .ZG13 = "0" & Space(9)
                    .ZG14 = "0"
                    .ZG15 = Space(8)
                End With

                sBuff = PFUNC_GET_ZENGIN_LINE(2)
                Call PSUB_DAT_ZENGIN_WRITE(iFileNo, 2)

                '口座振替明細作成
                STR_SQL = " INSERT INTO G_MEIMAST"
                STR_SQL += " values("
                STR_SQL += "'" & Str_Gakkou_Code & "'"
                STR_SQL += ",'" & .Item("NENDO_O") & "'"
                STR_SQL += ",'" & STR_FURIKAE_DATE(1) & "'"
                STR_SQL += "," & .Item("GAKUNEN_CODE_O")
                STR_SQL += "," & .Item("CLASS_CODE_O")
                STR_SQL += ",'" & .Item("SEITO_NO_O") & "'"
                STR_SQL += "," & .Item("TUUBAN_O")
                STR_SQL += ",'" & .Item("TKIN_NO_T") & "'" '学校マスタの情報 2006/10/24
                STR_SQL += ",'" & .Item("TSIT_NO_T") & "'" '学校マスタの情報 2006/10/24
                STR_SQL += ",'" & .Item("KAMOKU_T") & "'" '学校マスタの情報 2006/10/24
                STR_SQL += ",'" & .Item("KOUZA_T") & "'" '学校マスタの情報 2006/10/24
                STR_SQL += ",'" & .Item("TKIN_NO_O") & "'"
                STR_SQL += ",'" & .Item("TSIT_NO_O") & "'"
                STR_SQL += ",'" & .Item("KAMOKU_O") & "'"
                STR_SQL += ",'" & .Item("KOUZA_O") & "'"
                STR_SQL += ",'" & .Item("MEIGI_KNAME_O") & "'"
                STR_SQL += ",'" & Mid(Str_Seikyu_Nentuki, 5, 2) & "ｶﾞﾂﾄﾞ'"
                STR_SQL += ",'" & sJyuyouka_No & "0" & Space(9) & "'" '区分追加 2006/12/25
                STR_SQL += ",'" & sBuff & "'"
                STR_SQL += "," & lRecordCount
                STR_SQL += ",'" & Str_Seikyu_Nentuki & "'"
                STR_SQL += ",'" & Str_Seikyu_Nentuki & "'"
                STR_SQL += ",'" & .Item("HIMOKU_ID_O") & "'"
                STR_SQL += "," & lFurikae_Kingaku
                For iLcount = 1 To 15
                    STR_SQL += "," & .Item("KINGAKU" & Format(iLcount, "00") & "_O")
                Next iLcount
                STR_SQL += ",0"
                '再振作成済フラグを初期登録時は未入力だったのを0:未作成で
                '作成する
                'STR_SQL += ",' '"
                STR_SQL += ",'0'"
                STR_SQL += ",'0'"
                STR_SQL += ",'" & Str_Syori_Date(1) & "'"
                STR_SQL += ",' '"
                STR_SQL += ",' '"
                STR_SQL += ")"
            End With

            Select Case PFUNC_Check_Meisai()
                Case -1
                    'エラー有(異常リストに追加 , 明細に登録なし , スケジュール更新なし)
                    Lng_Err_Count += 1

                    Call PSUB_Insert_IjyoList()
                Case 0
                    '正常終了
                    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                        '異常リスト追加
                        Lng_Err_Count += 1
                        Str_Err_Msg = "データベースエラーです"

                        Call PSUB_Insert_IjyoList()
                    Else
                        lTotal_Kingaku += lFurikae_Kingaku
                        lTotal_Kensuu += 1

                        lRecordCount += 1

                        '学年毎の件数取得 2006/10/05
                        lngGAK_SYORI_KEN(Int_Err_Gakunen_Code) += 1
                        dblGAK_SYORI_KIN(Int_Err_Gakunen_Code) += lFurikae_Kingaku

                    End If
                Case Else
                    'エラー有(異常リストに追加 , 明細に登録あり , スケジュール更新あり)
                    lThrrowCount += 1
                    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                        '異常リスト追加
                        Lng_Err_Count += 1
                        Str_Err_Msg = "データベースエラーです"

                        Call PSUB_Insert_IjyoList()
                    Else
                        lTotal_Kingaku += lFurikae_Kingaku
                        lTotal_Kensuu += 1

                        lRecordCount += 1

                    End If

                    Lng_Err_Count += 1

                    Call PSUB_Insert_IjyoList()
            End Select
NEXT_SEITO:

        End While

        If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
            Exit Sub
        End If

        '該当データが０件の場合終了 2006/10/06
        If lTotal_Kensuu = 0 Then
            Err.Number = 0
            FileClose(iFileNo)
            If Dir$(sZenginFile) <> "" Then Kill(sZenginFile)

            If OBJ_TRANSACTION.Connection Is Nothing Then
            Else
                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then 'Commit処理
                    Exit Sub
                End If
            End If

            Exit Sub
        Else
            lngTAKOU_SYORISAKI += 1
        End If

        '全銀データ作成(トレーラー行｢集計結果｣)
        '作成した件数と金額を設定
        '前月の不能分は再振種別が0以外の場合のみ集計結果に加算される
        '(全銀データ作成(明細)時も同様)
        With gZENGIN_REC8
            .ZG1 = "8"
            .ZG2 = Format(lTotal_Kensuu, "000000")
            .ZG3 = Format(lTotal_Kingaku, "000000000000")
            .ZG4 = "000000"
            .ZG5 = "000000000000"
            .ZG6 = "000000"
            .ZG7 = "000000000000"
            .ZG8 = Space(65)
        End With

        Call PSUB_DAT_ZENGIN_WRITE(iFileNo, 8)

        '全銀データ作成(終了行)
        'データ区分
        'ダミー
        With gZENGIN_REC9
            .ZG1 = "9"
            .ZG2 = Space(119)
        End With

        Call PSUB_DAT_ZENGIN_WRITE(iFileNo, 9)

        Err.Number = 0

        FileClose(iFileNo)

        If Err.Number <> 0 Then
            Call GSUB_LOG(0, "全銀ﾌｧｲﾙCLOSEエラー")

            Exit Sub
        End If

        '同一金融機関・学年ごとで他行スケジュール作成 2006/10/05
        For i As Integer = 1 To 9
            If lngGAK_SYORI_KEN(i) = 0 Then
                GoTo NEXT_FOR
            End If
            '他行ｽｹｼﾞｭｰﾙ作成
            '同金融機関の学年ごとで他行ｽｹｼﾞｭｰﾙを作成
            If PFUNC_DelIns_TakouSchedule(Str_Gakkou_Code, i, 0, pGinko_Code) = False Then

                Exit Sub
            End If
NEXT_FOR:
        Next

        If OBJ_TRANSACTION.Connection Is Nothing Then
        Else
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
                Exit Sub
            End If
        End If

        'ダイアログ出力 2006/10/05
        If Lng_Err_Count = 0 Or Lng_Err_Count = lThrrowCount Then
            'FD保存
            Select Case GFUNC_FD_Copy(Me, Str_Gakkou_Name_MSG, sZenginFile, Trim(pFileName), Str_Baitai_Code, pGinko_Code)
                Case 0
                    '正常終了
                    Call GSUB_LOG(1, "他行分データ作成")
                Case 1
                    'キャンセル
                    lngTAKOU_SYORISAKI -= 1
                    Exit Sub
                Case Else
                    'エラー
                    lngTAKOU_SYORISAKI -= 1

                    Call GSUB_LOG(0, "全銀ﾌｧｲﾙFD保存エラー")

                    Exit Sub
            End Select

            '送付表は必ず出す 2006/10/20
            '印刷
            Str_WHERE = "{G_MEIMAST.GAKKOU_CODE_M} = '" & Trim(txtGAKKOU_CODE.Text) & "'"
            Str_WHERE += "and {G_MEIMAST.TKIN_NO_M} = '" & Trim(pGinko_Code) & "'"
            Str_WHERE += "and {G_MEIMAST.FURI_DATE_M} = '" & Trim(STR_FURIKAE_DATE(1)) & "'"

            '2007/06/28　送付票 表示項目変更（ＴＥＬ、ＦＡＸ）
            'Call GSUB_PRINT_CRYSTALREPORT("口座振替データ送付票.rpt", Str_Where)
            Call Sub_PRINT_SOUFU(Str_WHERE)

            Select Case CInt(Str_Meisai_kbn)
                Case 1, 2

                    '不能分の元レコードを処理最後に更新している為
                    Select Case CInt(Str_Meisai_kbn)
                        Case 1
                            Call GSUB_PRINT_CRYSTALREPORT2("口座振替一覧表(口座データ).rpt", PFUNC_Query_String(pGinko_Code, 1))
                        Case 2
                            '店番ソートあり
                            Call GSUB_PRINT_CRYSTALREPORT2("口座振替一覧表(店番ソート).rpt", PFUNC_Query_String(pGinko_Code, 2))
                    End Select
            End Select

            '↑最終金融機関集計行＆FD保存処理
        Else '異常があった場合
            If Dir$(sZenginFile) <> "" Then Kill(sZenginFile)
            lngTAKOU_SYORISAKI -= 1
        End If

        If Lng_Err_Count = 0 Or Lng_Err_Count = lThrrowCount Then
            If CInt(Str_Saifuri_Flg) = 2 Or CInt(Str_Saifuri_Flg) = 3 Then
                '不能分の元レコードの再振作成済区分を一括更新
                If PFUNC_Update_Sakuseizumi(Str_Gakkou_Code, Str_Seikyu_Nentuki, STR_FURIKAE_DATE(1), CInt(Str_Saifuri_Flg), 0, pGinko_Code) = False Then
                    Call GSUB_LOG(0, "作成済みフラグ更新失敗")

                    Exit Sub
                End If
            End If

        End If

        Lng_Trw_Count += lThrrowCount

        ReDim lngGAK_SYORI_KEN(10)
        ReDim dblGAK_SYORI_KIN(10)


    End Sub
    Private Sub PSUB_Insert_Meisai2(ByVal pGinko_Code As String, ByVal pItaku_Code As String, ByVal pKamoku As String, ByVal pKouza As String, ByVal pFileName As String, ByVal pCodeKbn As String)

        Dim lRecordCount As Long
        Dim lTotal_Kensuu As Long
        Dim lTotal_Kingaku As Long

        Dim iLoopCount As Integer
        Dim iFileNo As Integer

        Dim sSyofuri_Date As String
        Dim sBuff As String
        Dim sZenginFile As String

        Dim lSyukei(1) As Long

        Dim lThrrowCount As Long
        Dim iLcount As Integer
        Dim bFlg As Boolean = False
        Dim bLoopFlg As Boolean = False

        Select Case CInt(Str_Saifuri_Flg)
            Case 0, 3
                '処理対象外

                Exit Sub
            Case Else
                sSyofuri_Date = ""

                STR_SQL = " SELECT "
                STR_SQL += " FURI_DATE_S,GAKUNEN1_FLG_S,GAKUNEN2_FLG_S,GAKUNEN3_FLG_S"
                STR_SQL += " ,GAKUNEN4_FLG_S,GAKUNEN5_FLG_S,GAKUNEN6_FLG_S,GAKUNEN7_FLG_S"
                STR_SQL += " ,GAKUNEN8_FLG_S,GAKUNEN9_FLG_S"
                STR_SQL += " FROM G_SCHMAST"
                STR_SQL += " WHERE GAKKOU_CODE_S = '" & Str_Gakkou_Code & "'"
                STR_SQL += " AND NENGETUDO_S = '" & Str_Seikyu_Nentuki & "'"
                STR_SQL += " AND FURI_KBN_S = '0'"
                STR_SQL += " AND SCH_KBN_S <> '2'"
                STR_SQL += " AND SFURI_DATE_S = '" & STR_FURIKAE_DATE(1) & "'"

                '2006/12/20　複数件対応
                'sSyofuri_Date = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "FURI_DATE_S")

                'For iLoopCount = 1 To 9
                '    iGakunen_Flag(iLoopCount) = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "GAKUNEN" & iLoopCount & "_FLG_S")
                'Next
                If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
                    Exit Sub
                End If
                While (OBJ_DATAREADER_DREAD.Read = True)
                    sSyofuri_Date = OBJ_DATAREADER_DREAD.Item("FURI_DATE_S")
                    For iLoopCount = 1 To 9
                        If iGakunen_Flag(iLoopCount) = 0 Then
                            iGakunen_Flag(iLoopCount) = OBJ_DATAREADER_DREAD.Item("GAKUNEN" & iLoopCount & "_FLG_S")
                        End If
                    Next
                End While
                If GFUNC_SELECT_SQL3("", 1) = False Then
                    Exit Sub
                End If

        End Select

        '全銀ファイル作成
        sZenginFile = Str_Zengin_FileName & Str_Gakkou_Code & pGinko_Code & ".dat"

        If Dir$(sZenginFile) <> "" Then Kill(sZenginFile)

        'コピー先ファイル名
        If Dir(pFileName) <> "" Then Kill(pFileName)

        iFileNo = FreeFile()
        Err.Number = 0

        FileOpen(iFileNo, sZenginFile, OpenMode.Random, , , 120)    'ワークファイル

        If Err.Number <> 0 Then
            Call GSUB_LOG(0, "全銀ﾌｧｲﾙOPENエラー")

            Exit Sub
        End If

        '銀行名取得
        Call PSUB_GET_TAKONAME(pGinko_Code)

        'ヘッダデータ作成
        With gZENGIN_REC1
            .ZG1 = "1"
            .ZG2 = "91"
            If pCodeKbn = "0" Then 'JIS指定
                .ZG3 = "0"
            Else 'EBCDIC
                .ZG3 = "1"
            End If
            .ZG4 = pItaku_Code
            .ZG5 = Str_Gakkou_Name
            .ZG6 = Mid(STR_FURIKAE_DATE(1), 5, 4)
            .ZG7 = pGinko_Code
            .ZG8 = Str_Tako(0)
            .ZG9 = Str_Tako(1)
            .ZG10 = Str_Tako(2)
            .ZG11 = pKamoku
            .ZG12 = pKouza
            .ZG13 = Space(17)
        End With

        Call PSUB_DAT_ZENGIN_WRITE(iFileNo, 1)

        '不能振替データの取得
        STR_SQL = " SELECT "
        STR_SQL += " G_MEIMAST.*"
        STR_SQL += ", SEITO_KNAME_O , TKIN_NO_O , TSIT_NO_O"
        STR_SQL += ", ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V, BAITAI_CODE_V"
        STR_SQL += " FROM "
        STR_SQL += " G_MEIMAST"
        STR_SQL += ",SEITOMASTVIEW"
        STR_SQL += ",G_TAKOUMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M = GAKKOU_CODE_O"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_M = GAKKOU_CODE_V"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_O = TKIN_NO_V"
        STR_SQL += " AND"
        STR_SQL += " NENDO_M = NENDO_O"
        STR_SQL += " AND"
        STR_SQL += " TUUBAN_M = TUUBAN_O"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_M = '" & Str_Gakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_M = '" & pGinko_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " FURIKETU_CODE_M <> 0"
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_KIN_M > 0"
        STR_SQL += " AND"
        STR_SQL += " KAIYAKU_FLG_O <> '9'"
        STR_SQL += " AND"
        STR_SQL += " FURIKAE_O = '0'"
        '年度で抽出しても
        '生徒マスタは１生徒に月数（１２）分レコードが存在する為
        '１明細が１２明細まで増加するのを防ぐために追加
        '↓
        STR_SQL += " AND"
        STR_SQL += " TUKI_NO_O = '" & Str_Seikyu_Nentuki.Substring(4, 2) & "'"
        '↓2006/10/26
        STR_SQL += " AND ("
        For iLcount = 1 To 9
            If iGakunen_Flag(iLcount) = 1 Then
                If bLoopFlg = True Then
                    STR_SQL += " OR "
                End If
                STR_SQL += " GAKUNEN_CODE_M=" & iLcount
                bLoopFlg = True
            End If
        Next iLcount
        STR_SQL += " )"

        Select Case CInt(Str_Saifuri_Flg)
            Case 1
                '明細マスタより当月で不能となった初振データを取得する
                '明細マスタの振替日は取得した初振日を使用する
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_M = '" & sSyofuri_Date & "'"
                STR_SQL += " AND"
                STR_SQL += " SEIKYU_TUKI_M ='" & Str_Seikyu_Nentuki & "'"
            Case 2
                '再振種別＝２
                '過去に不能となった全振替データを取得する
        End Select

        '振替ﾃﾞｰﾀは金庫・店番・科目・口座番号・学年(降順)・請求月 2006/10/04
        STR_SQL += " ORDER BY TKIN_NO_O ASC, TSIT_NO_O ASC, KAMOKU_O ASC, KOUZA_O ASC, GAKUNEN_CODE_O DESC"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Sub
        End If

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Sub
        End If

        lRecordCount = 0
        lSyukei(0) = 0
        lSyukei(1) = 0
        lTotal_Kensuu = 0
        lTotal_Kingaku = 0
        Lng_Err_Count = 0

        lThrrowCount = 0

        While (OBJ_DATAREADER_DREAD.Read = True)
            '媒体コード取得 2006/10/11
            Str_Baitai_Code = OBJ_DATAREADER_DREAD.Item("BAITAI_CODE_V")

            With OBJ_DATAREADER_DREAD
                Int_Err_Gakunen_Code = .Item("GAKUNEN_CODE_M")
                Int_Err_Class_Code = .Item("CLASS_CODE_M")
                Str_Err_Seito_No = .Item("SEITO_NO_M")
                Int_Err_Tuuban = .Item("TUUBAN_M")
                Str_Err_Itaku_Name = ""
                Str_Err_Tkin_No = .Item("TKIN_NO_M")
                Str_Err_Tsit_No = .Item("TSIT_NO_M")
                Str_Err_Kamoku = .Item("TKAMOKU_M")
                Str_Err_Kouza = .Item("TKOUZA_M")
                Str_Err_Keiyaku_No = ""
                Str_Err_Keiyaku_Name = .Item("TMEIGI_KNM_M")
                Lng_Err_Furikae_Kingaku = .Item("SEIKYU_KIN_M")

                sBuff = CStr(.Item("FURI_DATA_M"))                               '前回振替全銀データ

                With gZENGIN_REC2
                    .ZG1 = Mid(sBuff, 1, 1)
                    .ZG2 = Mid(sBuff, 2, 4)
                    .ZG3 = Mid(sBuff, 6, 15)
                    .ZG4 = Mid(sBuff, 21, 3)
                    .ZG5 = Mid(sBuff, 24, 15)
                    .ZG6 = Mid(sBuff, 39, 4)
                    .ZG7 = Mid(sBuff, 43, 1)
                    .ZG8 = Mid(sBuff, 44, 7)
                    .ZG9 = Mid(sBuff, 51, 30)
                    .ZG10 = Mid(sBuff, 81, 10)
                    .ZG11 = Mid(sBuff, 91, 1)
                    .ZG12 = Mid(sBuff, 92, 10)
                    .ZG13 = "1" & Space(9)
                    .ZG14 = Mid(sBuff, 112, 1)
                    .ZG15 = Mid(sBuff, 113, 8)
                End With

                Call PSUB_DAT_ZENGIN_WRITE(iFileNo, 2)

                '口座振替明細データ作成
                STR_SQL = " insert into"
                STR_SQL += " G_MEIMAST"
                STR_SQL += " values("
                STR_SQL += " '" & Str_Gakkou_Code & "'"
                STR_SQL += ",'" & .Item("NENDO_M") & "'"
                STR_SQL += ",'" & STR_FURIKAE_DATE(1) & "'"
                STR_SQL += "," & .Item("GAKUNEN_CODE_M")
                STR_SQL += "," & .Item("CLASS_CODE_M")
                STR_SQL += ",'" & .Item("SEITO_NO_M") & "'"
                STR_SQL += "," & .Item("TUUBAN_M")
                STR_SQL += ",'" & .Item("ITAKU_KIN_M") & "'"
                STR_SQL += ",'" & .Item("ITAKU_SIT_M") & "'"
                STR_SQL += ",'" & .Item("ITAKU_KAMOKU_M") & "'"
                STR_SQL += ",'" & .Item("ITAKU_KOUZA_M") & "'"
                STR_SQL += ",'" & .Item("TKIN_NO_M") & "'"
                STR_SQL += ",'" & .Item("TSIT_NO_M") & "'"
                STR_SQL += ",'" & .Item("TKAMOKU_M") & "'"
                STR_SQL += ",'" & .Item("TKOUZA_M") & "'"
                STR_SQL += ",'" & .Item("TMEIGI_KNM_M") & "'"
                STR_SQL += ",'" & .Item("KTEKIYO_M") & "'"
                STR_SQL += ",'" & Trim(.Item("JUYOUKA_NO_M")).Substring(0, 10) & "1" & Space(9) & "'" '区分追加 2006/12/25
                STR_SQL += ",'" & sBuff & "'"
                STR_SQL += "," & lRecordCount
                Select Case CInt(Str_Saifuri_Flg)
                    Case 1
                        STR_SQL += ",'" & Str_Seikyu_Nentuki & "'"
                    Case 2
                        STR_SQL += ",'" & .Item("SEIKYU_TAISYOU_M") & "'"
                End Select
                STR_SQL += ",'" & Str_Seikyu_Nentuki & "'"
                STR_SQL += ",'" & .Item("HIMOKU_ID_M") & "'"
                STR_SQL += "," & .Item("SEIKYU_KIN_M")
                For iLoopCount = 1 To 15
                    STR_SQL += "," & .Item("HIMOKU" & iLoopCount & "_KIN_M")
                Next iLoopCount
                STR_SQL += ",0"
                '再振作成済フラグを初期登録時は未入力だったのを0:未作成で
                '作成する
                'STR_SQL += ",' '"
                STR_SQL += ",'0'"
                STR_SQL += ",'1'"
                STR_SQL += ",'" & Str_Syori_Date(1) & "'"
                STR_SQL += ",' '"
                STR_SQL += ",' '"
                STR_SQL += ")"

                Select Case PFUNC_Check_Meisai()
                    Case -1
                        'エラー有(異常リストに追加 , 明細に登録なし , スケジュール更新なし)
                        Lng_Err_Count += 1

                        Call PSUB_Insert_IjyoList()
                    Case 0
                        '正常終了
                        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                            '異常リスト追加
                            Lng_Err_Count += 1
                            Str_Err_Msg = "データベースエラーです"

                            Call PSUB_Insert_IjyoList()
                        Else
                            lTotal_Kingaku += .Item("SEIKYU_KIN_M")
                            lTotal_Kensuu += 1

                            lRecordCount += 1

                            '学年毎の件数取得 2006/10/05
                            lngGAK_SYORI_KEN(CInt(.Item("GAKUNEN_CODE_M"))) += 1
                            dblGAK_SYORI_KIN(CInt(.Item("GAKUNEN_CODE_M"))) += .Item("SEIKYU_KIN_M")

                        End If
                    Case Else
                        'エラー有(異常リストに追加 , 明細に登録あり , スケジュール更新あり)
                        lThrrowCount += 1
                        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                            '異常リスト追加
                            Lng_Err_Count += 1
                            Str_Err_Msg = "データベースエラーです"

                            Call PSUB_Insert_IjyoList()
                        Else
                            lTotal_Kingaku += .Item("SEIKYU_KIN_M")
                            lTotal_Kensuu += 1

                            lRecordCount += 1
                        End If

                        Lng_Err_Count += 1

                        Call PSUB_Insert_IjyoList()
                End Select
            End With

        End While

        If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
            Exit Sub
        End If

        '該当データが０件の場合終了 2006/10/06
        If lTotal_Kensuu = 0 Then
            Err.Number = 0
            FileClose(iFileNo)
            If Dir$(sZenginFile) <> "" Then Kill(sZenginFile)

            If OBJ_TRANSACTION.Connection Is Nothing Then
            Else
                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then 'Commit処理
                    Exit Sub
                End If
            End If

            Exit Sub
        Else
            lngTAKOU_SYORISAKI += 1
        End If

        '全銀データ作成(トレーラー行｢集計結果｣)
        '作成した件数と金額を設定
        '前月の不能分は再振種別が0以外の場合のみ集計結果に加算される
        '(全銀データ作成(明細)時も同様)
        With gZENGIN_REC8
            .ZG1 = "8"
            .ZG2 = Format(lTotal_Kensuu, "000000")
            .ZG3 = Format(lTotal_Kingaku, "000000000000")
            .ZG4 = "000000"
            .ZG5 = "000000000000"
            .ZG6 = "000000"
            .ZG7 = "000000000000"
            .ZG8 = Space(65)
        End With

        Call PSUB_DAT_ZENGIN_WRITE(iFileNo, 8)

        '全銀データ作成(終了行)
        'データ区分
        'ダミー
        With gZENGIN_REC9
            .ZG1 = "9"
            .ZG2 = Space(119)
        End With

        Call PSUB_DAT_ZENGIN_WRITE(iFileNo, 9)

        Err.Number = 0

        FileClose(iFileNo)

        If Err.Number <> 0 Then
            Call GSUB_LOG(0, "全銀ﾌｧｲﾙCLOSEエラー")

            Exit Sub
        End If

        '同一金融機関・学年ごとで他行スケジュール作成 2006/10/05
        For i As Integer = 1 To 9
            If lngGAK_SYORI_KEN(i) = 0 Then
                GoTo NEXT_FOR
            End If
            '他行ｽｹｼﾞｭｰﾙ作成
            '同金融機関の学年ごとで他行ｽｹｼﾞｭｰﾙを作成
            If PFUNC_DelIns_TakouSchedule(Str_Gakkou_Code, i, 0, pGinko_Code) = False Then

                Exit Sub
            End If
NEXT_FOR:
        Next

        If OBJ_TRANSACTION.Connection Is Nothing Then
        Else
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
                Exit Sub
            End If
        End If


        'ダイアログ出力 2006/10/05
        If Lng_Err_Count = 0 Or Lng_Err_Count = lThrrowCount Then
            'FD保存
            Select Case GFUNC_FD_Copy(Me, Str_Gakkou_Name_MSG, sZenginFile, Trim(pFileName), Str_Baitai_Code, pGinko_Code)
                Case 0
                    '正常終了
                    Call GSUB_LOG(1, "他行分データ作成")
                Case 1
                    lngTAKOU_SYORISAKI -= 1
                    'キャンセル
                    Exit Sub
                Case Else
                    lngTAKOU_SYORISAKI -= 1
                    'エラー
                    Call GSUB_LOG(0, "全銀ﾌｧｲﾙFD保存エラー")
                    Exit Sub
            End Select

            '送付表は必ず出す 2006/10/20
            '印刷
            Str_WHERE = "{G_MEIMAST.GAKKOU_CODE_M} = '" & Trim(Str_Gakkou_Code) & "'"
            Str_WHERE += "and {G_MEIMAST.TKIN_NO_M} = '" & Trim(pGinko_Code) & "'"
            Str_WHERE += "and {G_MEIMAST.FURI_DATE_M} = '" & Trim(STR_FURIKAE_DATE(1)) & "'"

            '2007/06/28　送付票 表示項目変更（ＴＥＬ、ＦＡＸ）
            'Call GSUB_PRINT_CRYSTALREPORT("口座振替データ送付票.rpt", Str_Where)
            Call Sub_PRINT_SOUFU(Str_WHERE)

            Select Case CInt(Str_Meisai_kbn)
                Case 1, 2

                    '不能分の元レコードを処理最後に更新している為
                    Select Case CInt(Str_Meisai_kbn)
                        Case 1
                            Call GSUB_PRINT_CRYSTALREPORT2("口座振替一覧表(口座データ).rpt", PFUNC_Query_String(pGinko_Code, 1))
                        Case 2
                            '店番ソートあり
                            Call GSUB_PRINT_CRYSTALREPORT2("口座振替一覧表(店番ソート).rpt", PFUNC_Query_String(pGinko_Code, 2))
                    End Select
            End Select

            '↑最終金融機関集計行＆FD保存処理
        Else '異常があった場合
            If Dir$(sZenginFile) <> "" Then Kill(sZenginFile)
            lngTAKOU_SYORISAKI -= 1
        End If

        If Lng_Err_Count = 0 Or Lng_Err_Count = lThrrowCount Then
            '不能分の元レコードの再振作成済区分を一括更新
            If PFUNC_Update_Sakuseizumi(Str_Gakkou_Code, Str_Seikyu_Nentuki, sSyofuri_Date, CInt(Str_Saifuri_Flg), 1, pGinko_Code) = False Then
                Call GSUB_LOG(0, "作成済みフラグ更新失敗")

                Exit Sub
            End If
        End If

        Lng_Trw_Count += lThrrowCount

        ReDim lngGAK_SYORI_KEN(10)
        ReDim dblGAK_SYORI_KIN(10)

    End Sub
    Private Sub PSUB_DAT_ZENGIN_WRITE(ByVal pFileNo As Integer, ByVal pZengin_kbn As Integer)

        Select Case pZengin_kbn
            Case 1
                Lng_RecordNo = 1
                FilePut(pFileNo, gZENGIN_REC1, Lng_RecordNo)
            Case 2
                Lng_RecordNo += 1
                FilePut(pFileNo, gZENGIN_REC2, Lng_RecordNo)
            Case 8
                Lng_RecordNo += 1
                FilePut(pFileNo, gZENGIN_REC8, Lng_RecordNo)
            Case 9
                Lng_RecordNo += 1
                FilePut(pFileNo, gZENGIN_REC9, Lng_RecordNo)
        End Select

    End Sub
    Private Sub PSUB_GET_GINKONAME(ByVal pGGinko_Code As String, ByVal pSiten_Code As String)

        '金融機関コードと支店コードから金融機関名、支店名を抽出

        Str_Ginko(0) = ""
        Str_Ginko(1) = ""

        If Trim(pGGinko_Code) = "" Or Trim(pSiten_Code) = "" Then
            Exit Sub
        End If

        Dim SQL As String = ""

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            SQL = "SELECT KIN_KNAME_N , SIT_KNAME_N  FROM TENMAST "
            SQL &= " WHERE KIN_NO_N = '" & pGGinko_Code & "'"
            SQL &= " AND SIT_NO_N = '" & pSiten_Code & "'"

            OraReader = New CASTCommon.MyOracleReader

            If OraReader.DataReader(SQL) Then
                Str_Ginko(0) = OraReader.GetItem("KIN_KNAME_N")
                Str_Ginko(1) = OraReader.GetItem("SIT_KNAME_N")
            End If

            OraReader.Close()
            OraReader = Nothing

        Catch ex As Exception
            Throw New Exception("TENMAST取得失敗", ex)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

    End Sub
    Private Sub PSUB_GET_TAKONAME(ByVal pTGGinko_Code As String)

        '金融機関コードと支店コードから金融機関名、支店名を抽出
        If Trim(pTGGinko_Code) = "" Then
            Exit Sub
        End If

        Str_Tako(0) = ""
        Str_Tako(1) = ""
        Str_Tako(2) = ""

        Dim OraDB As CASTCommon.MyOracle = Nothing
        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            OraDB = New CASTCommon.MyOracle
            Orareader = New CASTCommon.MyOracleReader(OraDB)

            Dim SQL As String = ""

            SQL = "SELECT TSIT_NO_V  FROM G_TAKOUMAST "
            SQL &= " WHERE GAKKOU_CODE_V = '" & Str_Gakkou_Code & "'"
            SQL &= " AND TKIN_NO_V = '" & pTGGinko_Code & "'"

            If Orareader.DataReader(SQL) Then
                Str_Tako(1) = Orareader.GetItem("TSIT_NO_V")
            Else
                Exit Try
            End If

            Orareader.Close()
            Orareader = Nothing

            SQL = "SELECT KIN_KNAME_N , SIT_KNAME_N  FROM TENMAST "
            SQL &= " WHERE KIN_NO_N = '" & pTGGinko_Code & "'"
            SQL &= " AND SIT_NO_N = '" & Str_Tako(1) & "'"

            Orareader = New CASTCommon.MyOracleReader(OraDB)

            If Orareader.DataReader(SQL) Then
                Str_Tako(0) = Orareader.GetItem("KIN_KNAME_N")
                Str_Tako(2) = Orareader.GetItem("SIT_KNAME_N")
            Else
                Exit Try
            End If

            Orareader.Close()
            Orareader = Nothing
            OraDB.Close()
            OraDB = Nothing

        Catch ex As Exception
            Throw New Exception("TENMAST取得失敗", ex)
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
                Orareader = Nothing
            End If
            If Not OraDB Is Nothing Then
                OraDB.Close()
                OraDB = Nothing
            End If
        End Try

    End Sub
    Private Sub PSUB_Insert_IjyoList()

        On Error Resume Next

        STR_SQL = " INSERT INTO G_IJYOLIST"
        STR_SQL += " VALUES ("
        STR_SQL += "'" & Str_Syori_Date(0) & "'"
        STR_SQL += "," & Int_Err_Gakunen_Code
        STR_SQL += "," & Int_Err_Class_Code
        STR_SQL += ",'" & Str_Err_Seito_No & "'"
        STR_SQL += "," & Int_Err_Tuuban
        STR_SQL += "," & Lng_Err_Count
        STR_SQL += ",'" & Str_Gakkou_Code & "'"
        STR_SQL += ",'" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += ",'" & Str_Err_Itaku_Name & "'"
        STR_SQL += "," & Lng_RecordNo
        STR_SQL += ",'" & Str_Err_Tkin_No & "'"
        STR_SQL += ",'" & Str_Err_Tsit_No & "'"
        STR_SQL += ",'" & Str_Err_Kamoku & "'"
        STR_SQL += ",'" & Str_Err_Kouza & "'"
        STR_SQL += ",'" & Str_Err_Keiyaku_No & "'"
        STR_SQL += ",'" & Str_Err_Keiyaku_Name.PadRight(40).Substring(0, 30) & "'"
        STR_SQL += "," & Lng_Err_Furikae_Kingaku
        STR_SQL += ",'" & Str_Err_Msg & "'"
        STR_SQL += ",'" & Format(Now, "yyyyMMddHHmmss") & "'" 'タイムスタンプ 2006/11/21
        STR_SQL += ",'" & Space(14) & "'"
        STR_SQL += ",'" & Space(14) & "'"
        STR_SQL += " )"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Sub
        End If

    End Sub



#End Region

#Region " Private Function "
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        Dim sSch_Check_Flg As String
        Dim sSch_Data_Flg As String
        Dim sSch_Entri_Flg As String
        Dim sSch_Tyudan_Flg As String

        Dim sTakou_Kbn As String

        PFUNC_Nyuryoku_Check = False

        Str_Gakkou_Code = ""

        '学校コード
        If Trim(txtGAKKOU_CODE.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("学校コードが未入力です")
            txtGAKKOU_CODE.Focus()

            Exit Function
        Else
            '学校マスタ存在チェック
            STR_SQL = "SELECT GAKKOU_NNAME_G , GAKKOU_KNAME_G , GAKMAST2.* FROM GAKMAST1,GAKMAST2 "
            STR_SQL += " WHERE GAKKOU_CODE_G = GAKKOU_CODE_T"
            STR_SQL += " AND GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Function
            End If

            OBJ_DATAREADER.Read()

            If OBJ_DATAREADER.HasRows = False Then
                If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                    Exit Function
                End If

                Call GSUB_MESSAGE_WARNING("入力された学校コードが存在しません")
                txtGAKKOU_CODE.Focus()

                Exit Function
            End If

            'Str_Baitai_Code = OBJ_DATAREADER.Item("BAITAI_CODE_T")

            Str_Meisai_kbn = OBJ_DATAREADER.Item("MEISAI_KBN_T")
            sTakou_Kbn = OBJ_DATAREADER.Item("TAKO_KBN_T")
            Str_Sort_Kbn = OBJ_DATAREADER.Item("MEISAI_OUT_T")
            Str_Saifuri_Flg = OBJ_DATAREADER.Item("SFURI_SYUBETU_T")
            Str_Gakkou_Name = OBJ_DATAREADER.Item("GAKKOU_KNAME_G")
            If Not IsDBNull(OBJ_DATAREADER.Item("GAKKOU_NNAME_G")) Then
                Str_Gakkou_Name_MSG = CStr(OBJ_DATAREADER.Item("GAKKOU_NNAME_G")).Trim
            Else
                Str_Gakkou_Name_MSG = CStr(OBJ_DATAREADER.Item("GAKKOU_KNAME_G")).Trim
            End If

            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Function
            End If

            Select Case CInt(sTakou_Kbn)
                Case Is <> 1
                    Call GSUB_MESSAGE_WARNING("入力学校コードは処理対象外の学校です")
                    txtGAKKOU_CODE.Focus()

                    Exit Function
            End Select
        End If

        Str_Gakkou_Code = Trim(txtGAKKOU_CODE.Text)

        '振替日
        If Trim(txtFuriDateY.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("振替日(年)が未入力です")
            txtFuriDateY.Focus()

            Exit Function
        End If

        If Trim(txtFuriDateM.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("振替日(月)が未入力です")
            txtFuriDateM.Focus()

            Exit Function
        Else
            Select Case CInt(txtFuriDateM.Text)
                Case 1 To 12
                Case Else
                    Call GSUB_MESSAGE_WARNING("請求月は１月～１２月を設定してください")
                    txtFuriDateM.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtFuriDateD.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("振替日(日)が未入力です")
            txtFuriDateD.Focus()

            Exit Function
        End If

        STR_FURIKAE_DATE(0) = Trim(txtFuriDateY.Text) & "/" & Trim(txtFuriDateM.Text) & "/" & Trim(txtFuriDateD.Text)
        STR_FURIKAE_DATE(1) = Trim(txtFuriDateY.Text) & Format(CInt(txtFuriDateM.Text), "00") & Format(CInt(txtFuriDateD.Text), "00")

        If Not IsDate(STR_FURIKAE_DATE(0)) Then
            Call GSUB_MESSAGE_WARNING("振替年月日が正しくありません")

            txtFuriDateY.Focus()

            Exit Function
        End If

        Str_Select_Nyusyutu_Code = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbNyusyutu)

        'スケジュールチェック
        STR_SQL = " SELECT "
        STR_SQL += " CHECK_FLG_S , DATA_FLG_S , ENTRI_FLG_S , TYUUDAN_FLG_S , NENGETUDO_S , SCH_KBN_S , FUNOU_YDATE_S"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " WHERE GAKKOU_CODE_S = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        STR_SQL += " AND FURI_DATE_S = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND FURI_KBN_S = '" & Str_Select_Nyusyutu_Code & "'"
        'ｽｹｼﾞｭｰﾙ区分2(随時)は処理に含まない為
        STR_SQL += " AND SCH_KBN_S <> '2'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Function
            End If

            Call GSUB_MESSAGE_WARNING("スケジュールが存在しません")

            Exit Function
        End If

        Str_Sch_Flg(0) = ""
        Str_Sch_Flg(1) = ""

        Str_Funou_YDate(0) = ""
        Str_Funou_YDate(1) = ""

        While (OBJ_DATAREADER.Read = True)
            Str_Seikyu_Nentuki = OBJ_DATAREADER.Item("NENGETUDO_S")

            sSch_Check_Flg = OBJ_DATAREADER.Item("CHECK_FLG_S")
            sSch_Data_Flg = OBJ_DATAREADER.Item("DATA_FLG_S")
            sSch_Entri_Flg = OBJ_DATAREADER.Item("ENTRI_FLG_S")
            sSch_Tyudan_Flg = OBJ_DATAREADER.Item("TYUUDAN_FLG_S")

            Select Case CInt(OBJ_DATAREADER.Item("SCH_KBN_S"))
                Case 0
                    Str_Sch_Flg(0) = OBJ_DATAREADER.Item("SCH_KBN_S")
                    Str_Funou_YDate(0) = OBJ_DATAREADER.Item("FUNOU_YDATE_S")
                Case 1
                    Str_Sch_Flg(1) = OBJ_DATAREADER.Item("SCH_KBN_S")
                    Str_Funou_YDate(1) = OBJ_DATAREADER.Item("FUNOU_YDATE_S")
            End Select

            If CInt(sSch_Tyudan_Flg) <> 0 Then
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If

                Call GSUB_MESSAGE_WARNING("このスケジュールは中断中です")

                Exit Function
            End If

            Select Case CInt(Str_Select_Nyusyutu_Code)
                Case 0, 1
                    '初振
                    '再振
                    If CInt(sSch_Check_Flg) <> 1 Then
                        If GFUNC_SELECT_SQL2("", 1) = False Then
                            Exit Function
                        End If
                        Call GSUB_MESSAGE_WARNING("請求金額確認の処理が終わっていません")

                        Exit Function
                    End If
                Case 2, 3
                    '入金
                    '出金
                    If CInt(sSch_Entri_Flg) <> 1 Then
                        If GFUNC_SELECT_SQL2("", 1) = False Then
                            Exit Function
                        End If

                        Call GSUB_MESSAGE_WARNING("生徒明細作成の処理が終わっていません")

                        Exit Function
                    End If
                    If CInt(sSch_Check_Flg) <> 1 Then
                        If GFUNC_SELECT_SQL2("", 1) = False Then
                            Exit Function
                        End If

                        Call GSUB_MESSAGE_WARNING("生徒明細入力の処理が終わっていません")

                        Exit Function
                    End If
            End Select

            If CInt(sSch_Data_Flg) <> 0 Then

                Call GSUB_MESSAGE_WARNING("振替データ作成済みです")
                If GFUNC_SELECT_SQL2("", 1) = False Then
                    Exit Function
                End If
                txtGAKKOU_CODE.Focus()
                txtGAKKOU_CODE.SelectionStart = 0
                txtGAKKOU_CODE.SelectionLength = txtGAKKOU_CODE.Text.Length
                Exit Function
            End If
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        '他行スケジュールチェック
        STR_SQL = " SELECT "
        STR_SQL += " GAKKOU_CODE_U"
        STR_SQL += " FROM G_TAKOUSCHMAST"
        STR_SQL += " WHERE GAKKOU_CODE_U = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"
        STR_SQL += " AND FURI_KBN_U = '" & Str_Select_Nyusyutu_Code & "'"
        STR_SQL += " AND FURI_DATE_U = '" & STR_FURIKAE_DATE(1) & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        OBJ_DATAREADER.Read()

        If OBJ_DATAREADER.HasRows = True Then
            If GFUNC_MESSAGE_QUESTION("他行データ作成処理済です") <> vbOK Then
                If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                    Exit Function
                End If
                Exit Function
            End If

            If GFUNC_MESSAGE_QUESTION("前回分のデータを削除して再作成しますか？") <> vbOK Then
                If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                    Exit Function
                End If
                Exit Function
            End If
        End If

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        Select Case CInt(Str_Select_Nyusyutu_Code)
            Case 0, 3
                Str_Zengin_FileName = STR_DAT_PATH & "D"
            Case 1
                Str_Zengin_FileName = STR_DAT_PATH & "D"
            Case 2
                Str_Zengin_FileName = STR_DAT_PATH & "D"
        End Select

        PFUNC_Nyuryoku_Check = True

    End Function
    Private Function PFUNC_Get_Gakunen(ByVal pGakkou_Code As String, ByRef pSiyou_gakunen() As Integer) As Integer

        Dim iLoopCount As Integer
        Dim iMaxGakunen As Integer

        ReDim pSiyou_gakunen(9)

        PFUNC_Get_Gakunen = -1

        '選択された学校の指定振替日で抽出
        '(全スケジュール区分が対象)
        STR_SQL = " SELECT "
        STR_SQL += " SCH_KBN_S"
        For iLoopCount = 1 To 9
            STR_SQL += ", GAKUNEN" & iLoopCount & "_FLG_S"
            pSiyou_gakunen(iLoopCount) = 0
        Next iLoopCount
        STR_SQL += ", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " left join GAKMAST2 on "
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " WHERE GAKKOU_CODE_S ='" & pGakkou_Code & "'"
        STR_SQL += " AND FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND CHECK_FLG_S ='1'"
        STR_SQL += " AND TYUUDAN_FLG_S ='0'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER.Read)
            With OBJ_DATAREADER
                iMaxGakunen = CInt(.Item("SIYOU_GAKUNEN_T"))
                For iLoopCount = 1 To iMaxGakunen
                    Select Case CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S"))
                        Case 1
                            pSiyou_gakunen(iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                    End Select
                Next iLoopCount
            End With
        End While

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        '追加 2006/12/2526
        '使用学年すべてにフラグがない場合は該当なしとする 2006/12/21
        For iLoopCount = 1 To iMaxGakunen
            Select Case pSiyou_gakunen(iLoopCount)
                Case 1
                    Exit For
                Case Else 'すべて指定がない場合
                    If iLoopCount = iMaxGakunen Then '追加 2006/12/21
                        PFUNC_Get_Gakunen = -2
                        Exit Function
                    End If
            End Select
        Next iLoopCount

        '使用学年全てに学年フラグがある場合は全学年対象として扱う
        '学年
        For iLoopCount = 1 To iMaxGakunen
            Select Case pSiyou_gakunen(iLoopCount)
                Case Is <> 1
                    PFUNC_Get_Gakunen = iMaxGakunen

                    Exit Function
            End Select
        Next iLoopCount

        PFUNC_Get_Gakunen = 0

    End Function

    Private Function PFUNC_DelIns_TakouSchedule(ByVal pTGakkou_Code As String, _
                                                  ByVal pTGakunen_Code As String, _
                                                  ByVal pTFurikae_kbn As String, _
                                                  ByVal pTGinko_Code As String) As Boolean

        Dim iG_Flg(1, 9) As Integer

        PFUNC_DelIns_TakouSchedule = False

        If OBJ_TRANSACTION.Connection Is Nothing Then
        Else
            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
                Exit Function
            End If
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Function
        End If

        '他行スケジュールマスタ作成
        STR_SQL = " INSERT INTO G_TAKOUSCHMAST "
        STR_SQL += " values ("
        STR_SQL += "'" & pTGakkou_Code & "'"
        STR_SQL += "," & pTGakunen_Code
        STR_SQL += ",'" & Str_Seikyu_Nentuki & "'"
        STR_SQL += ",'" & Str_Sch_Flg(0) & "'"
        STR_SQL += ",'" & Str_Select_Nyusyutu_Code & "'"
        STR_SQL += ",'" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += ",'" & Str_Funou_YDate(0) & "'"
        STR_SQL += ",'" & Str_Baitai_Code & "'"
        STR_SQL += ",'" & pTGinko_Code & "'"
        STR_SQL += ",'0'"
        STR_SQL += "," & lngGAK_SYORI_KEN(pTGakunen_Code)
        STR_SQL += "," & dblGAK_SYORI_KIN(pTGakunen_Code)
        STR_SQL += ",0"
        STR_SQL += ",0"
        STR_SQL += ",0"
        STR_SQL += ",0"
        STR_SQL += ",'" & Format(Now, "yyyyMMdd") & "'"
        STR_SQL += ")"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        PFUNC_DelIns_TakouSchedule = True

    End Function

    Private Function PFUNC_Update_Sakuseizumi(ByVal pGakkou_Code As String, ByVal pNengetu As String, ByVal pFuri_Date As String, ByVal pSyubetu As Integer, ByVal pFuri_Kbn As Integer, ByVal pTAKOU_KIN As String) As Boolean
        '前回振替で不能になったデータを再振or持越したというフラグ(再振済フラグ)を立たせる

        Dim bFlg As Boolean
        Dim bLoopFlg As Boolean
        Dim iLcount As Integer

        PFUNC_Update_Sakuseizumi = False

        'スケジュールが年間、特別から
        Select Case PFUNC_Get_Gakunen(pGakkou_Code, iGakunen_Flag)
            Case -1
                'エラー
                Call GSUB_LOG(0, "指定学年取得でエラーが発生しました")

                Exit Function
            Case -2
                'エラー
                Call GSUB_LOG(0, Str_Gakkou_Code & ":該当学年が0件です")

                Exit Function
            Case 0
                '全学年が対象
                bFlg = False
            Case Else
                '特定学年のみが対象
                bFlg = True
        End Select


        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Function
        End If

        '----------------------
        '振替作成済設定
        '----------------------
        STR_SQL = " UPDATE  G_MEIMAST SET "
        STR_SQL += " SAIFURI_SUMI_M ='1'"
        STR_SQL += " WHERE"
        STR_SQL += " (GAKKOU_CODE_M , NENDO_M , TUUBAN_M)"
        STR_SQL += " = ("
        STR_SQL += " SELECT "
        STR_SQL += " GAKKOU_CODE_O , NENDO_O , TUUBAN_O"
        STR_SQL += " FROM SEITOMASTVIEW"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M = GAKKOU_CODE_O"
        STR_SQL += " AND"
        STR_SQL += " NENDO_M = NENDO_O"
        STR_SQL += " AND"
        STR_SQL += " TUUBAN_M = TUUBAN_O"
        STR_SQL += " AND"
        STR_SQL += " KAIYAKU_FLG_O <> '9'"
        STR_SQL += " AND"
        STR_SQL += " FURIKAE_O = '0'"
        '年度で抽出しても
        '生徒マスタは１生徒に月数（１２）分レコードが存在する為
        '１明細が１２明細まで増加するのを防ぐために追加
        '↓
        STR_SQL += " AND"
        STR_SQL += " TUKI_NO_O = '" & pNengetu.Substring(4, 2) & "'"
        'STR_SQL += " TUKI_NO_O = '04'"
        '↑
        STR_SQL += " )"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_M = '" & pGakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " FURIKETU_CODE_M <> 0"
        STR_SQL += " AND"
        STR_SQL += " SAIFURI_SUMI_M = '0'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_M = '" & pTAKOU_KIN & "'" '指定金融機関分（他行)
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_KIN_M > 0"
        STR_SQL += " AND"
        If pFuri_Kbn = 0 Then '初振の場合
            STR_SQL += " FURI_DATE_M < '" & pFuri_Date & "'"
        Else
            STR_SQL += " FURI_DATE_M = '" & pFuri_Date & "'"

        End If
        Select Case pSyubetu
            Case 1
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_M = '0'"
                STR_SQL += " AND"
                STR_SQL += " SEIKYU_TUKI_M = '" & pNengetu & "'"
        End Select
        '指定学年追加 2006/10/05
        If bFlg = True Then
            STR_SQL += " AND ("
            For iLcount = 1 To 9
                If iGakunen_Flag(iLcount) = 1 Then
                    If bLoopFlg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_M=" & iLcount
                    bLoopFlg = True
                End If
            Next iLcount
            STR_SQL += " )"
        End If


        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_Update_Sakuseizumi = True

    End Function
    Private Function PFUNC_Delete_Meisai(ByVal pGakkou_Code As String, ByVal pFuri_Kbn As Integer) As Boolean

        PFUNC_Delete_Meisai = False

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        '---------------------
        '口座振替明細マスタ削除(他行分のみ)
        '----------------------
        STR_SQL = " DELETE  FROM G_MEIMAST"
        STR_SQL += " WHERE GAKKOU_CODE_M = '" & pGakkou_Code & "'"
        STR_SQL += " AND SEIKYU_TAISYOU_M = '" & Str_Seikyu_Nentuki & "'"
        STR_SQL += " AND FURI_DATE_M = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND FURI_KBN_M = '" & pFuri_Kbn & "'"
        STR_SQL += " AND TKIN_NO_M <> '" & STR_JIKINKO_CODE & "'"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Call GSUB_LOG(0, "口座振替明細データ削除")

            Exit Function
        End If

        '他行スケジュールマスタ削除
        '※前仕様では１回の処理で１レコードしか作成されなかった為
        '　現行仕様では1回の処理で複数レコード存在する為ここで削除しておく
        STR_SQL = " DELETE  G_TAKOUSCHMAST"
        STR_SQL += " WHERE GAKKOU_CODE_U = '" & pGakkou_Code & "'"
        STR_SQL += " AND FURI_KBN_U = '" & pFuri_Kbn & "'"
        STR_SQL += " AND FURI_DATE_U = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND TKIN_NO_U <> '" & STR_JIKINKO_CODE & "'"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Call GSUB_LOG(0, "他行スケジュール削除")

            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        '前回振替分の再振済フラグを"0"に戻す 2006/10/06
        Select Case Str_Saifuri_Flg
            Case "1", "2", "3"   '再振あり、繰越あり
                If PFUNC_ZENMEISAI_UPD(pFuri_Kbn) = False Then
                    Call GSUB_LOG(0, "前回振替明細データの更新")

                    Exit Function
                End If
        End Select

        PFUNC_Delete_Meisai = True

    End Function
    Private Function PFUNC_GET_ZENGIN_LINE(ByVal pIndex As Integer) As String

        Dim sLine As String

        PFUNC_GET_ZENGIN_LINE = ""
        sLine = ""

        Select Case pIndex
            Case 1
                With gZENGIN_REC1
                    sLine = .ZG1 & .ZG2 & .ZG3 & .ZG4 & .ZG5 & .ZG6 & .ZG7 & .ZG8 & .ZG9 & .ZG10 & .ZG11 & .ZG12 & .ZG13
                End With
            Case 2
                With gZENGIN_REC2
                    sLine = .ZG1 & .ZG2 & .ZG3 & .ZG4 & .ZG5 & .ZG6 & .ZG7 & .ZG8 & .ZG9 & .ZG10 & .ZG11 & .ZG12 & .ZG13 & .ZG14 & .ZG15
                End With
            Case 8
                With gZENGIN_REC8
                    sLine = .ZG1 & .ZG2 & .ZG3 & .ZG4 & .ZG5 & .ZG6 & .ZG7 & .ZG8
                End With
            Case 9
                With gZENGIN_REC9
                    sLine = .ZG1 & .ZG2
                End With
        End Select

        PFUNC_GET_ZENGIN_LINE = sLine

    End Function
    Private Function PFUNC_Get_Takou_Ginko(ByRef pTakou_Ginko(,) As String) As Boolean

        Dim lCount As Long

        PFUNC_Get_Takou_Ginko = False

        '処理対象となる他行一覧を取得
        STR_SQL = " SELECT "
        STR_SQL += " GAKKOU_CODE_V ,TKIN_NO_O"
        STR_SQL += ", ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V , CODE_KBN_V"
        STR_SQL += " FROM "
        STR_SQL += " SEITOMASTVIEW , GAKMAST2 , G_TAKOUMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_O = GAKKOU_CODE_T"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_O = GAKKOU_CODE_V"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_O = TKIN_NO_V"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_O = '" & Str_Gakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " TUKI_NO_O = '" & Mid(Str_Seikyu_Nentuki, 5, 2) & "'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_O <> '" & STR_JIKINKO_CODE & "'"
        STR_SQL += " AND"
        STR_SQL += " FURIKAE_O = '0'"
        STR_SQL += " AND"
        STR_SQL += " KAIYAKU_FLG_O <> '9'"
        STR_SQL += " group by GAKKOU_CODE_V,TKIN_NO_O , ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V, CODE_KBN_V"
        STR_SQL += " ORDER BY GAKKOU_CODE_V,TKIN_NO_O , ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V, CODE_KBN_V"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            GSUB_MESSAGE_WARNING("他行情報取得失敗")
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If
            GSUB_MESSAGE_WARNING("他行分データ作成対象の生徒が0件です")
            Exit Function
        End If

        lCount = 1

        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                ReDim Preserve pTakou_Ginko(6, lCount)

                pTakou_Ginko(0, lCount) = .Item("TKIN_NO_O")
                pTakou_Ginko(1, lCount) = "" '.Item("TSIT_NO_O")
                pTakou_Ginko(2, lCount) = .Item("ITAKU_CODE_V")
                pTakou_Ginko(3, lCount) = .Item("KAMOKU_V")
                pTakou_Ginko(4, lCount) = .Item("KOUZA_V")

                If IsDBNull(.Item("SFILE_NAME_V")) = True Then
                    'S+学校コード＋金融機関コード+.dat
                    pTakou_Ginko(5, lCount) = "S" & CStr(.Item("GAKKOU_CODE_V")).Trim & CStr(.Item("TKIN_NO_O")).Trim & ".dat"
                Else
                    If CStr(.Item("SFILE_NAME_V")).Trim = "" Then
                        pTakou_Ginko(5, lCount) = "S" & CStr(.Item("GAKKOU_CODE_V")).Trim & CStr(.Item("TKIN_NO_O")).Trim & ".dat"
                    Else
                        pTakou_Ginko(5, lCount) = CStr(.Item("SFILE_NAME_V")).Trim
                    End If
                End If

                'コード区分追加 2006/10/04
                pTakou_Ginko(6, lCount) = .Item("CODE_KBN_V")

                lCount += 1
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Takou_Ginko = True

    End Function
    Private Function PFUNC_Check_Meisai() As Integer

        Dim Chk_Sql As String

        PFUNC_Check_Meisai = -1

        '金融機関未入力チェック
        If Trim(Str_Err_Tkin_No) = "" Or Trim(Str_Err_Tsit_No) = "" Then
            Str_Err_Msg = "委託金融機関が未入力です。"

            Exit Function
        Else
            '存在チェック
            Chk_Sql = " SELECT * FROM TENMAST"
            Chk_Sql += " WHERE"
            Chk_Sql += " KIN_NO_N = '" & Str_Err_Tkin_No & "'"
            Chk_Sql += " AND"
            Chk_Sql += " SIT_NO_N = '" & Str_Err_Tsit_No & "'"

            If GFUNC_SELECT_SQL5(Chk_Sql, 0) = False Then
                Exit Function
            End If

            If OBJ_DATAREADER_DREAD3.HasRows = False Then
                If GFUNC_SELECT_SQL5("", 1) = False Then
                    Exit Function
                End If

                PFUNC_Check_Meisai = 0

                Str_Err_Msg = "金融機関マスタに登録されていません。"

                Exit Function
            End If

            If GFUNC_SELECT_SQL5("", 1) = False Then
                Exit Function
            End If

        End If

        '他行金融機関存在チェック

        '口座番号規定桁チェック
        Select Case Len(Trim(Str_Err_Kouza))
            Case Is <> 7
                Str_Err_Msg = "口座番号の桁が７桁以外です"

                Exit Function
        End Select

        '口座番号桁数ALLZEROチェック
        If Trim(Str_Err_Kouza) = "0000000" Then
            Str_Err_Msg = "口座番号にALLZERO値は設定できません"

            Exit Function
        End If

        PFUNC_Check_Meisai = 0

    End Function

    Private Function PFUNC_Get_Gakunen2(ByVal pGakkou_Code As String, ByRef pSiyou_gakunen(,) As Integer) As Boolean

        Dim iMaxGakunen As Integer

        ReDim pSiyou_gakunen(1, 9)

        PFUNC_Get_Gakunen2 = False

        '選択された学校の指定振替日で抽出
        '(全スケジュール区分が対象)
        STR_SQL = " SELECT "
        STR_SQL += " SCH_KBN_S"
        For iLoopCount As Integer = 1 To 9
            STR_SQL += ", GAKUNEN" & iLoopCount & "_FLG_S"
            pSiyou_gakunen(0, iLoopCount) = 0
            pSiyou_gakunen(1, iLoopCount) = 0
        Next iLoopCount
        STR_SQL += ", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " left join GAKMAST2 on "
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S ='" & pGakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND"
        STR_SQL += " CHECK_FLG_S ='1'"
        STR_SQL += " AND"
        STR_SQL += " TYUUDAN_FLG_S ='0'"
        'ｽｹｼﾞｭｰﾙ区分2(随時)は処理に含まない為
        STR_SQL += " AND SCH_KBN_S <> '2'"


        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                Exit Function
            End If

            Exit Function
        End If

        While (OBJ_DATAREADER.Read)
            With OBJ_DATAREADER
                iMaxGakunen = CInt(.Item("SIYOU_GAKUNEN_T"))
                Select Case CInt(.Item("SCH_KBN_S"))
                    Case 0
                        For iLoopCount As Integer = 1 To iMaxGakunen
                            Select Case CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S"))
                                Case 1
                                    pSiyou_gakunen(0, iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                            End Select
                        Next iLoopCount
                    Case 1
                        For iLoopCount As Integer = 1 To iMaxGakunen
                            Select Case CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S"))
                                Case 1
                                    pSiyou_gakunen(1, iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                            End Select
                        Next iLoopCount
                End Select
            End With
        End While

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Gakunen2 = True

    End Function

    Private Function PFUNC_Query_String(ByVal pGinko_Code As String, ByVal pSortKbn As Integer) As String

        Dim sQuery As String

        PFUNC_Query_String = ""

        sQuery = "SELECT TENMAST.SIT_NO_N, GAKMAST1.GAKKOU_CODE_G, GAKMAST1.GAKKOU_NNAME_G, HIMOMAST.HIMOKU_NAME01_H, HIMOMAST.HIMOKU_NAME02_H, HIMOMAST.HIMOKU_NAME04_H, HIMOMAST.HIMOKU_NAME06_H, HIMOMAST.HIMOKU_NAME07_H, HIMOMAST.HIMOKU_NAME08_H, HIMOMAST.HIMOKU_NAME09_H, HIMOMAST.HIMOKU_NAME10_H, HIMOMAST.HIMOKU_NAME11_H, HIMOMAST.HIMOKU_NAME12_H, HIMOMAST.HIMOKU_NAME13_H, HIMOMAST.HIMOKU_NAME14_H, HIMOMAST.HIMOKU_NAME15_H, HIMOMAST.HIMOKU_NAME03_H, HIMOMAST.HIMOKU_NAME05_H, TENMAST.KIN_NO_N, G_SCHMAST.FURI_DATE_S, SEITOMASTVIEW.GAKUNEN_CODE_O, SEITOMASTVIEW.SEITO_NO_O, SEITOMASTVIEW.SEITO_KNAME_O, SEITOMASTVIEW.SEITO_NNAME_O, SEITOMASTVIEW.MEIGI_KNAME_O, TENMAST.SIT_NNAME_N, SEITOMASTVIEW.TYOUSI_FLG_O, G_SCHMAST.FUNOU_FLG_S, G_MEIMAST.TKAMOKU_M, G_MEIMAST.GAKKOU_CODE_M, G_MEIMAST.GAKUNEN_CODE_M, G_MEIMAST.HIMOKU1_KIN_M, G_MEIMAST.HIMOKU2_KIN_M, G_MEIMAST.HIMOKU3_KIN_M, G_MEIMAST.HIMOKU4_KIN_M, G_MEIMAST.HIMOKU5_KIN_M, G_MEIMAST.HIMOKU6_KIN_M, G_MEIMAST.HIMOKU7_KIN_M, G_MEIMAST.HIMOKU8_KIN_M, G_MEIMAST.HIMOKU9_KIN_M, G_MEIMAST.HIMOKU10_KIN_M, G_MEIMAST.HIMOKU11_KIN_M, G_MEIMAST.HIMOKU12_KIN_M, G_MEIMAST.HIMOKU13_KIN_M, G_MEIMAST.HIMOKU14_KIN_M, G_MEIMAST.HIMOKU15_KIN_M, G_MEIMAST.TUUBAN_M, G_MEIMAST.NENDO_M, G_MEIMAST.CLASS_CODE_M, G_MEIMAST.SEITO_NO_M, G_MEIMAST.TKOUZA_M FROM   KZFMAST.G_MEIMAST G_MEIMAST, KZFMAST.SEITOMASTVIEW SEITOMASTVIEW, KZFMAST.G_SCHMAST G_SCHMAST, KZFMAST.HIMOMAST HIMOMAST, KZFMAST.GAKMAST1 GAKMAST1, KZFMAST.TENMAST TENMAST WHERE  ((((((G_MEIMAST.GAKKOU_CODE_M=SEITOMASTVIEW.GAKKOU_CODE_O) AND (G_MEIMAST.NENDO_M=SEITOMASTVIEW.NENDO_O)) AND (G_MEIMAST.GAKUNEN_CODE_M=SEITOMASTVIEW.GAKUNEN_CODE_O)) AND (G_MEIMAST.CLASS_CODE_M=SEITOMASTVIEW.CLASS_CODE_O)) AND (G_MEIMAST.SEITO_NO_M=SEITOMASTVIEW.SEITO_NO_O)) AND (G_MEIMAST.TUUBAN_M=SEITOMASTVIEW.TUUBAN_O)) AND (((G_MEIMAST.GAKKOU_CODE_M=G_SCHMAST.GAKKOU_CODE_S) AND (G_MEIMAST.FURI_DATE_M=G_SCHMAST.FURI_DATE_S)) AND (G_MEIMAST.FURI_KBN_M=G_SCHMAST.FURI_KBN_S)) AND ((((SEITOMASTVIEW.GAKKOU_CODE_O=HIMOMAST.GAKKOU_CODE_H (+)) AND (SEITOMASTVIEW.GAKUNEN_CODE_O=HIMOMAST.GAKUNEN_CODE_H (+))) AND (SEITOMASTVIEW.HIMOKU_ID_O=HIMOMAST.HIMOKU_ID_H (+))) AND (SEITOMASTVIEW.TUKI_NO_O=HIMOMAST.TUKI_NO_H (+))) AND ((SEITOMASTVIEW.GAKKOU_CODE_O=GAKMAST1.GAKKOU_CODE_G (+)) AND (SEITOMASTVIEW.GAKUNEN_CODE_O=GAKMAST1.GAKUNEN_CODE_G (+))) AND ((SEITOMASTVIEW.TKIN_NO_O=TENMAST.KIN_NO_N (+)) AND (SEITOMASTVIEW.TSIT_NO_O=TENMAST.SIT_NO_N (+))) "

        sQuery += " AND"
        sQuery += " G_MEIMAST.YOBI1_M = '" & Str_Syori_Date(1) & "'"
        sQuery += " AND"
        sQuery += " SEITOMASTVIEW.TUKI_NO_O = '" & Mid(Trim(STR_FURIKAE_DATE(1)), 5, 2) & "'"

        sQuery += " AND"
        sQuery += " G_MEIMAST.TKIN_NO_M ='" & pGinko_Code & "'"

        sQuery += " AND"
        sQuery += " ((G_MEIMAST.GAKUNEN_CODE_M=1 AND G_SCHMAST.GAKUNEN1_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=2 AND G_SCHMAST.GAKUNEN2_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=3 AND G_SCHMAST.GAKUNEN3_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=4 AND G_SCHMAST.GAKUNEN4_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=5 AND G_SCHMAST.GAKUNEN5_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=6 AND G_SCHMAST.GAKUNEN6_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=7 AND G_SCHMAST.GAKUNEN7_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=8 AND G_SCHMAST.GAKUNEN8_FLG_S='1') OR"
        sQuery += " (G_MEIMAST.GAKUNEN_CODE_M=9 AND G_SCHMAST.GAKUNEN9_FLG_S='1'))"



        sQuery += " ORDER BY"
        If pSortKbn = 2 Then
            sQuery += " G_MEIMAST.TKIN_NO_M , G_MEIMAST.TSIT_NO_M ,"
        End If
        Select Case CInt(Str_Sort_Kbn)
            Case 0
                '学年,クラス,生徒番号
                sQuery += "  G_MEIMAST.GAKUNEN_CODE_M ASC, G_MEIMAST.CLASS_CODE_M ASC, G_MEIMAST.SEITO_NO_M ASC,G_MEIMAST.NENDO_M ASC, G_MEIMAST.TUUBAN_M ASC"
            Case 1
                '入学年度,通番
                sQuery += "  G_MEIMAST.GAKUNEN_CODE_M ASC,G_MEIMAST.NENDO_M ASC, G_MEIMAST.TUUBAN_M ASC"
            Case 2
                'あいうえお(生徒名(ｶﾅ))
                sQuery += "  G_MEIMAST.GAKUNEN_CODE_M ASC,SEITOMASTVIEW.SEITO_KNAME_O ASC,G_MEIMAST.NENDO_M ASC, G_MEIMAST.TUUBAN_M ASC"
        End Select

        PFUNC_Query_String = sQuery

    End Function
    Private Function PFUNC_ZENMEISAI_UPD(ByVal aintFURI_KBN As Integer) As Boolean
        '前回振替分で不能となっているものの再振済フラグを"0"に戻す
        Dim bFlg As Boolean
        Dim bLoopFlg As Boolean = False '指定学年を条件に追加したかチェック
        Dim iLcount As Integer '指定学年ループ数

        Dim strZENKAI_FURI_DATE As String '前回振替日

        PFUNC_ZENMEISAI_UPD = False

        'スケジュールが年間、特別から
        Select Case PFUNC_Get_Gakunen(txtGAKKOU_CODE.Text, iGakunen_Flag)
            Case -1
                'エラー
                Call GSUB_LOG(0, "指定学年取得でエラーが発生しました")

                Exit Function
            Case -2
                'エラー
                Call GSUB_LOG(0, Str_Gakkou_Code & ":該当学年が0件です")

                Exit Function
            Case 0
                '全学年が対象
                bFlg = False
            Case Else
                '特定学年のみが対象
                bFlg = True
        End Select


        '前回明細マスタ更新（再振済フラグ）
        '検索キーは、学校コード、振替区分、振替結果コード、再振済フラグで、振替日の降順

        STR_SQL = " SELECT * FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        'STR_SQL += " AND"
        'STR_SQL += " FURI_KBN_M ='" & GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU_TXT, cmbFURIKUBUN) & "'"
        STR_SQL += " AND"
        STR_SQL += " FURIKETU_CODE_M <> 0"
        'STR_SQL += " AND"
        'STR_SQL += " SAIFURI_SUMI_M ='1'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_M <> '" & STR_JIKINKO_CODE & "'" '他行分のみ抽出
        STR_SQL += " ORDER BY FURI_DATE_M desc"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.Read() = True Then

            '初振の取消
            Select Case aintFURI_KBN
                Case "0" '初振
                    Select Case Str_Saifuri_Flg
                        Case "1", "2"
                            '再振ありの場合で取得した明細が初振の明細の場合
                            '前回の再振で振込が成功しているので明細マスタの更新は行わない
                            If OBJ_DATAREADER_DREAD.Item("FURI_KBN_M") = 0 Then
                                If GFUNC_SELECT_SQL3("", 1) = False Then
                                    Exit Function
                                End If
                                PFUNC_ZENMEISAI_UPD = True

                                Exit Function
                            End If
                    End Select
            End Select

            strZENKAI_FURI_DATE = OBJ_DATAREADER_DREAD.Item("FURI_DATE_M")
        Else
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Function
            End If
            PFUNC_ZENMEISAI_UPD = True

            Exit Function
        End If

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            '更新処理エラー
            Call GSUB_MESSAGE_WARNING("明細マスタの更新でエラーが発生しました")
            Exit Function
        End If

        '前回振替日と同一日付の明細マスタを更新する
        STR_SQL = " UPDATE  G_MEIMAST SET "
        '再振済フラグ
        STR_SQL += " SAIFURI_SUMI_M ='0'"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M ='" & txtGAKKOU_CODE.Text & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M ='" & strZENKAI_FURI_DATE & "' "
        STR_SQL += " AND"
        STR_SQL += " FURIKETU_CODE_M <> 0"
        STR_SQL += " AND"
        STR_SQL += " SAIFURI_SUMI_M ='1'"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_M <> '" & STR_JIKINKO_CODE & "'" '他行分のみ抽出

        'If strSCH_KBN <> "0" Then '通常以外のスケジュール時は学年指定 2005/12/09
        STR_SQL += " AND ("
        For iLcount = 1 To 9
            If iGakunen_Flag(iLcount) = 1 Then
                If bLoopFlg = True Then
                    STR_SQL += " OR "
                End If
                STR_SQL += " GAKUNEN_CODE_M = " & iLcount
                bLoopFlg = True
            End If
        Next iLcount
        STR_SQL += " )"
        'End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            '更新処理エラー
            Call GSUB_MESSAGE_WARNING("明細マスタの更新でエラーが発生しました")
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            '更新処理エラー
            Call GSUB_MESSAGE_WARNING("明細マスタの更新でエラーが発生しました")
            Exit Function
        End If

        PFUNC_ZENMEISAI_UPD = True

    End Function

#End Region

    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged
        '********************************************
        '学校カナ絞込みコンボ
        '********************************************
        If cmbKana.Text = "" Then
            Exit Sub
        End If

        '学校名コンボボックス設定
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGAKKOUNAME, True) = True Then
            cmbGAKKOUNAME.Focus()
        End If

    End Sub
    Private Sub cmbGAKKOUNAME_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGAKKOUNAME.SelectedIndexChanged

        'COMBOBOX選択時学校名,学校コード設定
        lblGAKKOU_NAME.Text = cmbGAKKOUNAME.Text
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGAKKOUNAME.SelectedIndex)

        '学年テキストボックスにFOCUS
        txtGAKKOU_CODE.Focus()

    End Sub

    Private Sub TextBox_Validating(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.Validating

        With CType(sender, TextBox)
            If .Text.Trim.Length <> 0 Then
                '学校名検索
                STR_SQL = "SELECT GAKKOU_NNAME_G FROM GAKMAST1,GAKMAST2 "
                STR_SQL += " WHERE GAKKOU_CODE_G = GAKKOU_CODE_T"
                STR_SQL += " AND TAKO_KBN_T ='1'"
                STR_SQL += " AND GAKKOU_CODE_G = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'"

                If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                    Exit Sub
                End If

                OBJ_DATAREADER.Read()
                If OBJ_DATAREADER.HasRows = True Then
                    lblGAKKOU_NAME.Text = CStr(OBJ_DATAREADER.Item("GAKKOU_NNAME_G"))
                Else
                    lblGAKKOU_NAME.Text = ""
                End If

                If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
                    Exit Sub
                End If
            End If
        End With
    End Sub

    Public Sub Sub_PRINT_SOUFU(ByVal pJyoken As String)
        '============================================================================
        'NAME      :PSUB_PRINT_SOUFU
        'Parameter   :pJyoken：出力条件（ＳＱＬ文）
        'Description  :送付票出力
        '        :（ｉｎｉファイルから電話番号とＦＡＸ番号を取得、表示する）
        'Return     :True=OK,False=NG
        'Create     :2007/06/28
        'UPDATE     :
        '============================================================================

        'ｉｎｉファイルから電話番号とＦＡＸ番号を取得
        Dim strSoufuTEL As String '電話番号
        Dim strSoufuFAX As String 'ＦＡＸ番号

        strSoufuTEL = GFUNC_INI_READ("COMMON", "SOUFU_TEL") '電話番号の取得
        strSoufuFAX = GFUNC_INI_READ("COMMON", "SOUFU_FAX") 'ＦＡＸ番号の取得

        'クリスタルレポートの設定
        'Dim CRXApplication As New CRAXDDRT.Application
        'Dim CRXReport As CRAXDDRT.Report
        'Dim CPProperty As CRAXDDRT.ConnectionProperty
        'Dim DBTable As CRAXDDRT.DatabaseTable
        'Dim CRX_FORMULA As CRAXDDRT.FormulaFieldDefinition
        'Dim strFORMULA_NAME As String

        'CRXReport = CRXApplication.OpenReport(STR_LST_PATH & "口座振替データ送付票.rpt", 1)

        'DBTable = CRXReport.Database.Tables(1)

        'CPProperty = DBTable.ConnectionProperties("Password")
        'CPProperty.Value = "KZFMAST"
        'CRXReport.RecordSelectionFormula = pJyoken

        ''出力帳票に変数を設定
        'For i As Integer = 1 To CRXReport.FormulaFields.Count
        '    CRX_FORMULA = CRXReport.FormulaFields.Item(i)
        '    strFORMULA_NAME = CRX_FORMULA.FormulaFieldName

        '    Select Case strFORMULA_NAME
        '        Case "担当部署電話番号"
        '            CRX_FORMULA.Text = "'" & strSoufuTEL & "'"
        '        Case "担当部署ＦＡＸ番号"
        '            CRX_FORMULA.Text = "'" & strSoufuFAX & "'"
        '    End Select
        'Next

        'CRXReport.PrintOut(False, 1) '出力

    End Sub

End Class
