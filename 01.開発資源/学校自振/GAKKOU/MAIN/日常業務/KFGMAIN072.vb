Option Explicit On 
Option Strict Off

Imports CASTCommon

Public Class KFGMAIN072

    'KFJMAIN0100Gマスタ落し込み用
    Private gastrTORI_CODE_MAIN0100 As String
    Private gastrFURI_DATE_MAIN0100 As String
    Private gstrBAITAI_CODE_MAIN0100 As String
    Private gstrFMT_KBN_MAIN0100 As String
    Private gstrCODE_KBN_MAIN0100 As String
    Private gstrLABEL_KBN_MAIN0100 As String

    'コモンダイアログ用
    Public Const STR_DLG_FILTER As String = "*.dat"
    Public Const STR_DLG_FILTER_NAME As String = "全銀ファイル"
    Public Const STR_DEF_FILE_KBN As String = "DAT"

#Region " 共通変数定義 "
    Private INTCNT01 As Integer
    Private Bln_Gakunen_Flg As Boolean
    Private Bln_Ginko_Flg(1) As Boolean

    Private Str_ZenginFile As String
    Private Str_Syori_Date(1) As String
    Private Str_Ginko(3) As String
    Private Str_Gakunen_Flg() As String
    Private Str_Baitai_Code As String
    Private Str_WHERE As String

    Private Lng_Ijyo_Count As Long
    Private Lng_Err_Count As Long

    Private Lng_RecordNo As Long

    Private Lng_Trw_Count As Long

    Private Str_Syori_Ginko(,) As String

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
    Public lngGAK_SYORI_KEN(10) As Long
    Public dblGAK_SYORI_KIN(10) As Double

    Public lngTAKOU_SYORISAKI As Long = 0 '他行データ(振替ﾃﾞｰﾀファイル)を作成した数 2006/10/06
    Public Str_Seikyu_Nentuki As String = ""
    Public strFUNOU_YDATE As String = ""
    Public strGAKKOU_KNAME As String = ""
    Public flgNEXT_DATA_MAKE As Boolean = True '処理途中でキャンセルするかどうか True:続行 False:中断
    Public intPRNT_SORT As Integer = 0 '帳票ソート順 2006/10/18
    Public strTKIN_NO_GAK As String = "" '学校の取扱金融機関コード 2006/10/23
    Public strTSIT_NO_GAK As String = "" '学校の取扱支店コード 2006/10/23
    Public strKAMOKU_GAK As String = "" '学校の科目コード 2006/10/23
    Public strKOUZA_GAK As String = "" '学校の口座番号 2006/10/23
    Private Const msgTitle As String = "随時データ作成画面(KFGMAIN072)"
    Private MainLOG As New CASTCommon.BatchLOG("KFGMAIN072", "随時データ作成画面")
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite
#End Region

#Region " Form Load "
    Private Sub KFGMAIN072_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '#####################################
        'ログの書込に必要な情報の取得
        LW.UserID = GCom.GetUserID
        LW.ToriCode = "000000000000"
        LW.FuriDate = "00000000"
        '#####################################
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)開始", "成功", "")

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            LW.ToriCode = STR_生徒明細学校コード
            LW.FuriDate = STR_生徒明細振替日

            '学校コード
            lab学校コード.Text = STR_生徒明細学校コード

            lab学校名.Text = STR_生徒明細学校名

            lab振替日.Text = Mid(STR_生徒明細振替日, 1, 4) & "/" & Mid(STR_生徒明細振替日, 5, 2) & "/" & Mid(STR_生徒明細振替日, 7, 2)

            Select Case (STR_生徒明細入出区分)
                Case "2"
                    lab入出金区分.Text = "入金"
                Case "3"
                    lab入出金区分.Text = "出金"
            End Select

            STR_SQL = "SELECT * FROM GAKMAST2"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_T ='" & STR_生徒明細学校コード & "'"

            Str_Baitai_Code = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "BAITAI_CODE_T")
            intPRNT_SORT = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "MEISAI_OUT_T") '2006/10/18
            strTKIN_NO_GAK = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "TKIN_NO_T")  '学校の取扱金融機関コード 2006/10/23
            strTSIT_NO_GAK = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "TSIT_NO_T")  '学校の取扱支店コード 2006/10/23
            strKAMOKU_GAK = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "KAMOKU_T") '学校の科目コード 2006/10/23
            strKOUZA_GAK = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "KOUZA_T")  '学校の口座番号 2006/10/23

            '学校名カナ取得
            STR_SQL = "SELECT GAKKOU_KNAME_G FROM GAKMAST1"
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_G ='" & STR_生徒明細学校コード & "'"
            STR_SQL += " group by GAKKOU_KNAME_G"

            strGAKKOU_KNAME = GFUNC_GET_SELECTSQL_ITEM(STR_SQL, "GAKKOU_KNAME_G")


            'スプレッド一覧の設定
            If PFUNC_Spread_Set() = False Then
                MainLOG.Write("ロード", "失敗", "生徒明細取得エラー")
                Exit Sub
            End If

            'スケジュールが随時から
            If PFUNC_Get_Gakunen() = False Then
                MainLOG.Write("ロード", "失敗", "スケジュール取得エラー")
                Exit Sub
            End If

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

            btnCreate.Focus()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(ロード)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub btnCreate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreate.Click

        Cursor.Current = Cursors.WaitCursor()
        Dim strDIR As String
        strDIR = CurDir()
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(データ作成)開始", "成功", "")
            STR_COMMAND = "随時データ作成"

            If MessageBox.Show(String.Format(MSG0015I, "随時データ作成"), _
                               msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> Windows.Forms.DialogResult.OK Then
                Return
            End If
            '処理日の取得
            Str_Syori_Date(0) = Format(Now, "yyyyMMdd")
            Str_Syori_Date(1) = Format(Now, "yyyyMMddHHmmss")

            Lng_Ijyo_Count = 0

            'エラーリストの初期化
            Call PSUB_Delete_IjyoList()

            flgNEXT_DATA_MAKE = True '処理中断フラグ初期化

            'If Bln_Ginko_Flg(0) = True Then'自金庫かの判定はなし
            '蒲郡信金向け 自行・他行一つのファイルにまとめる 2007/09/06
            '自行全銀作成
            Call PSUB_Insert_Meisai()

            Lng_Ijyo_Count += Lng_Err_Count
            'End If

            '自行分帳票印刷 2006/10/17
            If Lng_Ijyo_Count = 0 Or Lng_Ijyo_Count = Lng_Trw_Count Then
                Select Case (CInt(STR_生徒明細印刷区分))
                    Case 1, 2

                        Dim nRet As Integer
                        Dim Param As String
                        '印刷バッチ呼び出し
                        Dim ExeRepo As New CAstReports.ClsExecute
                        ExeRepo.SetOwner = Me

                        'パラメータ設定：ログイン名、学校コード、処理日
                        Param = GCom.GetUserID & "," & STR_生徒明細学校コード & "," & Str_Syori_Date(1)

                        nRet = ExeRepo.ExecReport("KFGP005.EXE", Param)

                        '戻り値に対応したメッセージを表示する
                        Select Case nRet
                            Case 0
                            Case Else
                                '印刷失敗メッセージ
                                MessageBox.Show(String.Format(MSG0004E, "口座振替予定集計表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Select


                        '口座振替予定一覧表
                        ExeRepo = New CAstReports.ClsExecute
                        ExeRepo.SetOwner = Me
                        'パラメータ設定：ログイン名、学校コード、処理日、振替日、帳票印刷区分(1:店番ソートなし 2:店番ソートあり)、帳票ソート順
                        Param = GCom.GetUserID & "," & STR_生徒明細学校コード & "," _
                                 & Str_Syori_Date(1) & "," & STR_生徒明細振替日 & "," & STR_生徒明細印刷区分 & "," _
                                 & intPRNT_SORT

                        nRet = ExeRepo.ExecReport("KFGP003.EXE", Param)

                        '戻り値に対応したメッセージを表示する
                        Select Case nRet
                            Case 0
                            Case Else
                                '印刷失敗メッセージ
                                MessageBox.Show(String.Format(MSG0004E, "口座振替予定一覧表"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Select
                End Select
            End If

            flgNEXT_DATA_MAKE = True '処理中断フラグ初期化

            'エラー件数が１件以上の場合はエラーリスト印刷
            Select Case (Lng_Ijyo_Count)
                Case 0, Lng_Trw_Count
                    '完了メッセージ
                    MessageBox.Show(String.Format(MSG0016I, "随時データ作成"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case Else
                    'エラーリスト印刷
                    Dim nRet As Integer
                    Dim Param2 As String
                    '印刷バッチ呼び出し
                    Dim ExeRepo As New CAstReports.ClsExecute
                    ExeRepo.SetOwner = Me

                    'パラメータ設定：ログイン名
                    Param2 = GCom.GetUserID

                    nRet = ExeRepo.ExecReport("KFGP001.EXE", Param2)

                    '戻り値に対応したメッセージを表示する
                    Select Case nRet
                        Case 0
                        Case Else
                            '印刷失敗メッセージ
                            MessageBox.Show(String.Format(MSG0004E, "インプットエラーリスト"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Select

            End Select

            btnEnd.Focus()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(データ作成)", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(データ作成)終了", "成功", "")
            ChDir(strDIR)
        End Try
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)開始", "成功", "")
            If OBJ_CONNECTION_DREAD Is Nothing Then
            Else
                'Oracle CLOSE
                OBJ_CONNECTION_DREAD.Close()
                OBJ_CONNECTION_DREAD = Nothing
            End If

            If OBJ_CONNECTION_DREAD2 Is Nothing Then
            Else
                'Oracle CLOSE
                OBJ_CONNECTION_DREAD2.Close()
                OBJ_CONNECTION_DREAD2 = Nothing
            End If

            If OBJ_CONNECTION_DREAD3 Is Nothing Then
            Else
                'Oracle CLOSE
                OBJ_CONNECTION_DREAD3.Close()
                OBJ_CONNECTION_DREAD3 = Nothing
            End If

            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "例外", "失敗", ex.ToString)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(クローズ)終了", "成功", "")
        End Try
    End Sub
#End Region

#Region " Private Sub "
    Private Sub PSUB_Delete_IjyoList()

        Lng_Ijyo_Count = 0

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Sub
        End If

        STR_SQL = " DELETE  FROM G_IJYOLIST"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Sub
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Sub
        End If

    End Sub
    Private Sub PSUB_Insert_IjyoList()

        On Error Resume Next

        STR_SQL = " INSERT INTO G_IJYOLIST"
        STR_SQL += " values("
        STR_SQL += "'" & Str_Syori_Date(0) & "'"
        STR_SQL += "," & Int_Err_Gakunen_Code
        STR_SQL += "," & Int_Err_Class_Code
        STR_SQL += ",'" & Str_Err_Seito_No & "'"
        STR_SQL += "," & Int_Err_Tuuban
        STR_SQL += "," & Lng_Ijyo_Count
        STR_SQL += ",'" & STR_生徒明細学校コード & "'"
        STR_SQL += ",'" & STR_生徒明細振替日 & "'"
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
        STR_SQL += ",'" & Format(Now, "yyyyMMddHHmmss") & "'" 'タイムスタンプ 2006/12/25
        STR_SQL += ",'" & Space(14) & "'"
        STR_SQL += ",'" & Space(14) & "'"
        STR_SQL += " )"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Sub
        End If

    End Sub
    Private Sub PSUB_Insert_Meisai()

        Dim bLoopFlg As Boolean

        Dim iLcount As Integer
        Dim iNo As Integer
        Dim iFileNo As Integer

        Dim sJyuyouka_No As String
        Dim sBuff As String
        Dim sZenginFile As String
        Dim sBaitai_Code As String
        Dim sFile_Name As String

        Dim lThrrowCount As Long

        Dim lRecordCount As Long
        Dim lFurikae_Kingaku As Long
        Dim lTotal_Kingaku As Long
        Dim lTotal_Kensuu As Long

        Dim MainDB As New MyOracle

        On Error Resume Next

        '口座振替明細マスタ削除(自行のみ)
        If PFUNC_Delete_Meisai(0) = False Then
            MainLOG.Write("明細作成", "失敗", "口座振替明細マスタ削除エラー")
            Exit Sub
        End If

        '全銀ファイル作成
        Select Case (CInt(STR_生徒明細入出区分))
            Case 2
                Str_ZenginFile = STR_DAT_PATH & "D" & STR_生徒明細学校コード & "03.dat"
            Case 3
                Str_ZenginFile = STR_DAT_PATH & "D" & STR_生徒明細学校コード & "04.dat"
        End Select

        If Dir$(Str_ZenginFile) <> "" Then Kill(Str_ZenginFile)

        iFileNo = FreeFile()
        Err.Number = 0

        FileOpen(iFileNo, Str_ZenginFile, OpenMode.Random, , , 120)    'ワークファイル

        If Err.Number <> 0 Then
            MainLOG.Write("明細作成", "失敗", "全銀ﾌｧｲﾙOPENエラー")
            Exit Sub
        End If

        STR_SQL = " SELECT "
        STR_SQL += " ITAKU_CODE_T , KAMOKU_T , KOUZA_T , FILE_NAME_T , BAITAI_CODE_T"
        STR_SQL += ", GAKKOU_KNAME_G"
        STR_SQL += ", KIN_NO_N , KIN_KNAME_N , SIT_NO_N , SIT_KNAME_N"
        STR_SQL += " FROM GAKMAST1 , GAKMAST2 , TENMAST , G_SCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_G = GAKKOU_CODE_T"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_T = KIN_NO_N"
        STR_SQL += " AND"
        STR_SQL += " TSIT_NO_T = SIT_NO_N"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_G = GAKKOU_CODE_S"
        STR_SQL += " AND"
        STR_SQL += " NENGETUDO_S = '" & Mid(STR_生徒明細振替日, 1, 6) & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S = '" & STR_生徒明細入出区分 & "'"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_G = '" & STR_生徒明細学校コード & "'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S = '2'"

        'データベースOPEN
        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            MainLOG.Write("明細作成", "失敗", "ﾃﾞｰﾀﾍﾞｰｽOPENエラー")
            Exit Sub
        End If

        'データレコードチェック
        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Sub
            End If

            MainLOG.Write("明細作成", "失敗", "明細データ０件")
            Exit Sub
        End If

        OBJ_DATAREADER.Read()

        '全銀データ作成(ヘッダ)
        'データ区分(=1) 
        '種別コード(21 OR 91)
        'コード区分
        '振込依頼人コード
        '振込依頼人名
        '取扱日
        '仕向銀行ｺｰﾄﾞ
        '仕向銀行名
        '仕向支店ｺｰﾄﾞ
        '仕向支店名
        '預金種目
        '口座番号
        'ダミー
        With gZENGIN_REC1
            .ZG1 = "1"
            Select Case (CInt(STR_生徒明細入出区分))
                Case 2
                    '入金
                    .ZG2 = "21"
                    iNo = 1
                Case 3
                    '随時出金
                    .ZG2 = "91"
                    iNo = 2
            End Select
            .ZG3 = "0" 'JIS形式のみなので"1"⇒"0"に修正 2006/10/18
            .ZG4 = OBJ_DATAREADER.Item("ITAKU_CODE_T")
            .ZG5 = OBJ_DATAREADER.Item("GAKKOU_KNAME_G")
            .ZG6 = Mid(STR_生徒明細振替日, 5, 4)
            .ZG7 = OBJ_DATAREADER.Item("KIN_NO_N")
            .ZG8 = OBJ_DATAREADER.Item("KIN_KNAME_N")
            .ZG9 = OBJ_DATAREADER.Item("SIT_NO_N")
            .ZG10 = OBJ_DATAREADER.Item("SIT_KNAME_N")
            .ZG11 = Format(CInt(OBJ_DATAREADER.Item("KAMOKU_T")), "0")
            .ZG12 = Format(CInt(OBJ_DATAREADER.Item("KOUZA_T")), "0000000")
            .ZG13 = Space(17)
        End With
        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 1) = False Then
            MainLOG.Write("明細作成", "失敗", "全銀ﾌｧｲﾙﾍｯﾀﾞ部書込みエラー")
            Exit Sub
        End If

        With OBJ_DATAREADER
            sBaitai_Code = Trim(.Item("BAITAI_CODE_T"))
            sFile_Name = Trim(.Item("FILE_NAME_T"))
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Sub
        End If

        bLoopFlg = False

        lRecordCount = 1
        lTotal_Kingaku = 0
        lTotal_Kensuu = 0
        lFurikae_Kingaku = 0
        Lng_Err_Count = 0

        lThrrowCount = 0

        'エントリマスタ取得(自行のみ)
        STR_SQL = " SELECT "
        STR_SQL += " G_ENTMAST" & iNo & ".*"
        STR_SQL += ", TKIN_NO_T , TSIT_NO_T , KAMOKU_T , KOUZA_T"
        STR_SQL += " FROM G_ENTMAST" & iNo & " , GAKMAST2"
        STR_SQL += " WHERE GAKKOU_CODE_E = GAKKOU_CODE_T"
        '蒲郡信金向け 自行・他行一つのファイルにまとめる 2007/09/06
        'STR_SQL += " AND"
        'STR_SQL += " TKIN_NO_E ='" & Str_Jikou_Ginko_Code & "'"
        STR_SQL += " AND FURI_DATE_E ='" & STR_生徒明細振替日 & "'"
        STR_SQL += " AND FURIKIN_E > 0"
        STR_SQL += " AND GAKKOU_CODE_E = '" & STR_生徒明細学校コード & "'"
        If Bln_Gakunen_Flg = False Then
            STR_SQL += " AND ("
            For iLcount = 1 To UBound(Str_Gakunen_Flg)
                If bLoopFlg = True Then
                    STR_SQL += " OR "
                End If
                STR_SQL += " GAKUNEN_CODE_E=" & Str_Gakunen_Flg(iLcount)
                bLoopFlg = True
            Next iLcount
            STR_SQL += " )"
        End If
        '振替ﾃﾞｰﾀは金庫・店番・科目・口座番号・学年(降順)・請求月 2006/10/17
        STR_SQL += " ORDER BY TKIN_NO_E ASC, TSIT_NO_E ASC, KAMOKU_E ASC, KOUZA_E ASC, GAKUNEN_CODE_E DESC"

        'データベースOPEN
        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            MainLOG.Write("明細作成", "失敗", "データベースOPENエラー")
            Exit Sub
        End If

        'データレコードチェック
        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Sub
            End If

            MainLOG.Write("明細作成", "失敗", "明細データ０件")
            Exit Sub
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            If GFUNC_SELECT_SQL3("", 1) = False Then
                Exit Sub
            End If

            Exit Sub
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                Int_Err_Gakunen_Code = .Item("GAKUNEN_CODE_E")
                Int_Err_Class_Code = .Item("CLASS_CODE_E")
                Str_Err_Seito_No = .Item("SEITO_NO_E")
                Int_Err_Tuuban = .Item("TUUBAN_E")
                Str_Err_Itaku_Name = ""
                Str_Err_Tkin_No = .Item("TKIN_NO_E")
                Str_Err_Tsit_No = .Item("TSIT_NO_E")
                Str_Err_Kamoku = .Item("KAMOKU_E")
                Str_Err_Kouza = .Item("KOUZA_E")
                Str_Err_Keiyaku_No = .Item("KEIYAKU_NO_E")
                Str_Err_Keiyaku_Name = .Item("KEIYAKU_KNAME_E")
                Lng_Err_Furikae_Kingaku = .Item("FURIKIN_E")

                '振替金額
                lFurikae_Kingaku = .Item("FURIKIN_E")

                '需要家番号
                sJyuyouka_No = .Item("NENDO_E") & Format(.Item("TUUBAN_E"), "0000") & Str_Seikyu_Nentuki.Substring(4, 2)
            End With

            Call PSUB_GET_GINKONAME(OBJ_DATAREADER_DREAD.Item("TKIN_NO_E"), OBJ_DATAREADER_DREAD.Item("TSIT_NO_E"), MainDB)

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
                .ZG2 = Format(CLng(OBJ_DATAREADER_DREAD.Item("TKIN_NO_E")), "0000")
                .ZG3 = Str_Ginko(0)
                .ZG4 = Format(CLng(OBJ_DATAREADER_DREAD.Item("TSIT_NO_E")), "000")
                .ZG5 = Str_Ginko(2)
                .ZG6 = Space(4)
                .ZG7 = CASTCommon.ConvertKamoku2TO1(OBJ_DATAREADER_DREAD.Item("KAMOKU_E"))
                .ZG8 = Format(CLng(OBJ_DATAREADER_DREAD.Item("KOUZA_E")), "0000000")
                .ZG9 = Mid(OBJ_DATAREADER_DREAD.Item("KEIYAKU_KNAME_E"), 1, 30)
                .ZG10 = Format(lFurikae_Kingaku, "0000000000")
                .ZG11 = "0"
                .ZG12 = sJyuyouka_No
                .ZG13 = CInt(STR_生徒明細入出区分) & Space(9)      'スケジュール区分を設定→振替区分に変更 2006/12/22
                .ZG14 = "0"
                .ZG15 = Space(8)
            End With
            If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 2) = False Then
                If GFUNC_SELECT_SQL3("", 1) = False Then
                    Exit Sub
                End If

                Exit Sub
            End If

            '明細登録用振替データ(全銀フォーマットのデータレコード１２０BYTE)
            '2017/05/15 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
            '金融機関名カナ／支店名カナの後ろ空白がトリムされている
            sBuff = gZENGIN_REC2.Data
            'sBuff = PFUNC_GET_ZENGIN_LINE(2)
            '2017/05/15 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END

            With OBJ_DATAREADER_DREAD
                '口座振替明細作成
                STR_SQL = " INSERT INTO G_MEIMAST"
                STR_SQL += " values("
                STR_SQL += "'" & STR_生徒明細学校コード & "'"
                STR_SQL += ",'" & .Item("NENDO_E") & "'"
                STR_SQL += ",'" & STR_生徒明細振替日 & "'"
                STR_SQL += "," & .Item("GAKUNEN_CODE_E")
                STR_SQL += "," & .Item("CLASS_CODE_E")
                STR_SQL += ",'" & .Item("SEITO_NO_E") & "'"
                STR_SQL += "," & .Item("TUUBAN_E")
                STR_SQL += ",'" & .Item("TKIN_NO_T") & "'"
                STR_SQL += ",'" & .Item("TSIT_NO_T") & "'"
                STR_SQL += ",'" & .Item("KAMOKU_T") & "'"
                STR_SQL += ",'" & .Item("KOUZA_T") & "'"
                STR_SQL += ",'" & .Item("TKIN_NO_E") & "'"
                STR_SQL += ",'" & .Item("TSIT_NO_E") & "'"
                STR_SQL += ",'" & .Item("KAMOKU_E") & "'"
                STR_SQL += ",'" & .Item("KOUZA_E") & "'"
                STR_SQL += ",'" & .Item("KEIYAKU_KNAME_E") & "'"
                STR_SQL += ",'" & Str_Seikyu_Nentuki.Substring(4, 2) & "ｶﾞﾂﾄﾞ'"
                STR_SQL += ",'" & sJyuyouka_No & CInt(STR_生徒明細入出区分) & Space(9) & "'" '振替区分に変更 2006/12/22
                STR_SQL += ",'" & sBuff & "'"
                STR_SQL += "," & lRecordCount
                STR_SQL += ",'" & Mid(STR_生徒明細振替日, 1, 6) & "'"
                STR_SQL += ",'" & Mid(STR_生徒明細振替日, 1, 6) & "'"
                STR_SQL += ",'000'"
                STR_SQL += "," & lFurikae_Kingaku
                For iLcount = 1 To 15
                    STR_SQL += ",0"
                Next iLcount
                STR_SQL += ",0"
                STR_SQL += ",'0'"
                '2006/10/20 入金と出金で振替区分を変更する
                'STR_SQL += ",'2'"
                Select Case (CInt(STR_生徒明細入出区分))
                    Case 2
                        '入金
                        STR_SQL += ",'2'"
                    Case 3
                        '随時出金
                        STR_SQL += ",'3'"
                End Select

                STR_SQL += ",'" & Str_Syori_Date(1) & "'"   '予備１
                STR_SQL += ",' '"                           '予備２
                STR_SQL += ",' '"                           '予備３
                STR_SQL += ",' '"                           '予備４
                STR_SQL += ",' '"                           '予備５
                STR_SQL += ",' '"                           '予備６
                STR_SQL += ",' '"                           '予備７
                STR_SQL += ",' '"                           '予備８
                STR_SQL += ",' '"                           '予備９
                STR_SQL += ",' '"                           '予備１０
                STR_SQL += ")"
            End With

            '明細マスタ登録
            Select Case (PFUNC_Chk_Meisai())
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
        End While

        MainDB.Close()

        'データベースCLOSE
        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Sub
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Str_Err_Msg = ""

            Exit Sub
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

        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 8) = False Then
            MainLOG.Write("明細作成", "失敗", "明細データ０件")

            Exit Sub
        End If

        '全銀データ作成(終了行)
        'データ区分
        'ダミー
        With gZENGIN_REC9
            .ZG1 = "9"
            .ZG2 = Space(119)
        End With

        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 9) = False Then
            Exit Sub
        End If

        Err.Number = 0

        FileClose(iFileNo)

        If Err.Number <> 0 Then
            MainLOG.Write("明細作成", "失敗", "全銀ﾌｧｲﾙCLOSEエラー")

            Exit Sub
        End If

        '例外として、金融機関に存在しない金融機関の登録時はエラーもスケジュールの更新も両方行う
        If Lng_Err_Count = 0 Or Lng_Err_Count = lThrrowCount Then
            'スケジュール更新
            If PFUNC_Update_Schedule(iNo) = False Then

                MainLOG.Write("明細作成", "失敗", "スケジュール更新エラー")

                Exit Sub
            End If

            'FDコピー
            'Select case GFUNC_FD_Copy2(Me, STR_生徒明細学校名, Str_ZenginFile, sFile_Name, Str_Baitai_Code)
            '2005/06/27確認メッセージを表示しない
            Select Case (GFUNC_FD_Copy3(Me, STR_生徒明細学校名, Str_ZenginFile, sFile_Name, Str_Baitai_Code))
                Case 0
                    '正常終了
                    MainLOG.Write("随時データ作成", "成功", "")

                Case 1
                    'キャンセル
                    flgNEXT_DATA_MAKE = False '中断
                    Exit Sub
                Case Else
                    'エラー
                    MainLOG.Write("明細作成", "失敗", "全銀ﾌｧｲﾙFD保存エラー")

                    Exit Sub
            End Select

            Dim JobID As String = ""
            Dim Param As String = ""
            '2005/06/27
            'ジョブ監視にパラメータ追加
            '------------------------------------------------
            'ジョブマスタに登録
            '------------------------------------------------
            JobID = "J010"
            Select Case (STR_生徒明細入出区分)
                Case "2"
                    gastrTORI_CODE_MAIN0100 = STR_生徒明細学校コード & "03"
                Case "3"
                    gastrTORI_CODE_MAIN0100 = STR_生徒明細学校コード & "04"
            End Select

            gastrFURI_DATE_MAIN0100 = STR_生徒明細振替日
            gstrCODE_KBN_MAIN0100 = "0"
            gstrFMT_KBN_MAIN0100 = "00"
            gstrBAITAI_CODE_MAIN0100 = "7"
            gstrLABEL_KBN_MAIN0100 = "0"
            Param = gastrTORI_CODE_MAIN0100 & "," & gastrFURI_DATE_MAIN0100 & "," & gstrCODE_KBN_MAIN0100 & "," & gstrFMT_KBN_MAIN0100 _
                            & "," & gstrBAITAI_CODE_MAIN0100 & "," & gstrLABEL_KBN_MAIN0100

            If fn_JOBMAST_TOUROKU_CHECK(JobID, GCom.GetUserID, Param) = False Then
                gdbcCONNECT.Close()
                Me.Close()
                Exit Sub
            End If
            If fn_INSERT_JOBMAST(JobID, GCom.GetUserID, Param) = False Then
                MainLOG.Write("パラメータ登録", "失敗", JobID & ":" & Param)
            Else
                'MessageBox.Show("起動パラメタを登録しました", gstrSYORI_R)
                MainLOG.Write("パラメータ登録", "成功", JobID & ":" & Param)
            End If
        End If

        Lng_Trw_Count += lThrrowCount

    End Sub
    '    Private Sub PSUB_Insert_Meisai_Takou(ByVal pGinko_Code As String, _
    '                                         ByVal pSiten_Code As String, _
    '                                         ByVal pItaku_Code As String, _
    '                                         ByVal pKamoku As String, _
    '                                         ByVal pKouza As String, _
    '                                         ByVal pFileName As String, _
    '                                         ByVal pCodeKbn As String)

    '        Dim bLoopFlg As Boolean

    '        Dim iLcount As Integer
    '        Dim iNo As Integer
    '        Dim iFileNo As Integer

    '        Dim lThrrowCount As Long

    '        Dim sJyuyouka_No As String
    '        Dim sBuff As String

    '        Dim lRecordCount As Long
    '        Dim lFurikae_Kingaku As Long
    '        Dim lTotal_Kingaku As Long
    '        Dim lTotal_Kensuu As Long
    '        Dim lSyukei(1) As Long

    '        ReDim lngGAK_SYORI_KEN(10)
    '        ReDim dblGAK_SYORI_KIN(10)

    '        Select Case (CInt(STR_生徒明細入出区分))
    '            Case 2
    '                Str_ZenginFile = STR_DAT_PATH & "D" & STR_生徒明細学校コード & pGinko_Code & "03.dat"
    '            Case 3
    '                Str_ZenginFile = STR_DAT_PATH & "D" & STR_生徒明細学校コード & pGinko_Code & "01.dat"
    '        End Select

    '        If Dir$(Str_ZenginFile) <> "" Then Kill(Str_ZenginFile)

    '        iFileNo = FreeFile()
    '        Err.Number = 0

    '        FileOpen(iFileNo, Str_ZenginFile, OpenMode.Random, , , 120)    'ワークファイル

    '        If Err.Number <> 0 Then
    '            If GFUNC_SELECT_SQL3("", 1) = False Then
    '                Exit Sub
    '            End If

    '            Call GSUB_LOG(0, "全銀ﾌｧｲﾙOPENエラー")

    '            Exit Sub
    '        End If

    '        '銀行名取得
    '        Call PSUB_GET_GINKONAME(pGinko_Code, pSiten_Code)

    '        '全銀データ作成(ヘッダ)
    '        'データ区分(=1) 
    '        '種別コード(21 OR 91)
    '        'コード区分
    '        '振込依頼人コード
    '        '振込依頼人名
    '        '取扱日
    '        '仕向銀行ｺｰﾄﾞ
    '        '仕向銀行名
    '        '仕向支店ｺｰﾄﾞ
    '        '仕向支店名
    '        '預金種目
    '        '口座番号
    '        'ダミー
    '        With gZENGIN_REC1
    '            .ZG1 = "1"
    '            Select Case (CInt(STR_生徒明細入出区分))
    '                Case 2
    '                    '入金
    '                    .ZG2 = "21"
    '                    iNo = 1
    '                Case 3
    '                    '随時出金
    '                    .ZG2 = "91"
    '                    iNo = 2
    '            End Select
    '            .ZG3 = "0" 'JIS形式のみなので"1"⇒"0"に修正 2006/10/18
    '            .ZG4 = pItaku_Code
    '            .ZG5 = strGAKKOU_KNAME.Trim
    '            .ZG6 = Mid(STR_生徒明細振替日, 5, 4)
    '            .ZG7 = pGinko_Code
    '            .ZG8 = Str_Ginko(0)
    '            .ZG9 = pSiten_Code
    '            .ZG10 = Str_Ginko(2)
    '            .ZG11 = pKamoku
    '            .ZG12 = pKouza
    '            .ZG13 = Space(17)
    '        End With

    '        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 1) = False Then
    '            Call GSUB_LOG(0, "全銀ﾌｧｲﾙﾍｯﾀﾞ部書込みエラー")

    '            Exit Sub
    '        End If



    '        bLoopFlg = False

    '        lRecordCount = 1
    '        lTotal_Kingaku = 0
    '        lTotal_Kensuu = 0
    '        lFurikae_Kingaku = 0
    '        Lng_Err_Count = 0

    '        'エントリマスタ取得(他行のみ)
    '        STR_SQL = " SELECT "
    '        STR_SQL += " G_ENTMAST" & iNo & ".*"
    '        STR_SQL += ", ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V,BAITAI_CODE_V"
    '        STR_SQL += " FROM "
    '        STR_SQL += " G_ENTMAST" & iNo & " , G_TAKOUMAST "
    '        STR_SQL += " WHERE"
    '        STR_SQL += " GAKKOU_CODE_E = GAKKOU_CODE_V"
    '        STR_SQL += " AND"
    '        STR_SQL += " TKIN_NO_E = TKIN_NO_V"
    '        STR_SQL += " AND"
    '        STR_SQL += " TKIN_NO_E = '" & pGinko_Code & "'"
    '        STR_SQL += " AND"
    '        STR_SQL += " FURIKIN_E > 0"
    '        STR_SQL += " AND"
    '        STR_SQL += " GAKKOU_CODE_E = '" & STR_生徒明細学校コード & "'"
    '        '2006/10/20 振替日を条件に追加
    '        STR_SQL += " AND"
    '        STR_SQL += " FURI_DATE_E ='" & STR_生徒明細振替日 & "'"
    '        If Bln_Gakunen_Flg = False Then
    '            STR_SQL += " AND ("
    '            For iLcount = 1 To UBound(Str_Gakunen_Flg)
    '                If bLoopFlg = True Then
    '                    STR_SQL += " OR "
    '                End If
    '                STR_SQL += " GAKUNEN_CODE_E=" & Str_Gakunen_Flg(iLcount)
    '                bLoopFlg = True
    '            Next iLcount
    '            STR_SQL += " )"
    '        End If
    '        '振替ﾃﾞｰﾀは金庫・店番・科目・口座番号・学年(降順)・請求月 2006/10/17
    '        STR_SQL += " ORDER BY TKIN_NO_E ASC, TSIT_NO_E ASC, KAMOKU_E ASC, KOUZA_E ASC, GAKUNEN_CODE_E DESC"

    '        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
    '            Exit Sub
    '        End If

    '        'データレコードチェック
    '        If OBJ_DATAREADER_DREAD.HasRows = False Then
    '            If GFUNC_SELECT_SQL3("", 1) = False Then
    '                Exit Sub
    '            End If

    '            Call GSUB_LOG(0, "明細データ０件")

    '            Exit Sub
    '        End If

    '        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
    '            If GFUNC_SELECT_SQL3("", 1) = False Then
    '                Exit Sub
    '            End If

    '            Exit Sub
    '        End If

    '        While (OBJ_DATAREADER_DREAD.Read = True)

    '            With OBJ_DATAREADER_DREAD
    '                '媒体コード取得 2006/10/11
    '                Str_Baitai_Code = .Item("BAITAI_CODE_V")

    '                Int_Err_Gakunen_Code = .Item("GAKUNEN_CODE_E")
    '                Int_Err_Class_Code = .Item("CLASS_CODE_E")
    '                Str_Err_Seito_No = .Item("SEITO_NO_E")
    '                Int_Err_Tuuban = .Item("TUUBAN_E")
    '                Str_Err_Itaku_Name = ""
    '                Str_Err_Tkin_No = .Item("TKIN_NO_E")
    '                Str_Err_Tsit_No = .Item("TSIT_NO_E")
    '                Str_Err_Kamoku = .Item("KAMOKU_E")
    '                Str_Err_Kouza = .Item("KOUZA_E")
    '                Str_Err_Keiyaku_No = .Item("KEIYAKU_NO_E")
    '                Str_Err_Keiyaku_Name = .Item("KEIYAKU_KNAME_E")
    '                Lng_Err_Furikae_Kingaku = .Item("FURIKIN_E")

    '                '振替金額
    '                lFurikae_Kingaku = .Item("FURIKIN_E")

    '                '需要家番号
    '                sJyuyouka_No = .Item("NENDO_E") & Format(.Item("TUUBAN_E"), "0000") & Str_Seikyu_Nentuki.Substring(4, 2)
    '            End With

    '            Call PSUB_GET_GINKONAME(OBJ_DATAREADER_DREAD.Item("TKIN_NO_E"), OBJ_DATAREADER_DREAD.Item("TSIT_NO_E"))

    '            If lFurikae_Kingaku = 0 Then '振替金額0円の生徒はデータ作成しない 2006/10/05
    '                GoTo NEXT_SEITO
    '            End If

    '            '全銀データ作成(明細)
    '            'データ区分(=2)
    '            '被仕向銀行番号
    '            '被仕向銀行名　
    '            '被仕向支店番号
    '            '被仕向支店名
    '            '手形交換所番号
    '            '預金種目
    '            '口座番号
    '            '受取人
    '            '振込金額
    '            '新規コード
    '            '顧客コード１
    '            '顧客コード２
    '            '振込指定区分
    '            'ダミー
    '            With gZENGIN_REC2
    '                .ZG1 = "2"
    '                .ZG2 = Format(CLng(OBJ_DATAREADER_DREAD.Item("TKIN_NO_E")), "0000")
    '                .ZG3 = Str_Ginko(0)
    '                .ZG4 = Format(CLng(OBJ_DATAREADER_DREAD.Item("TSIT_NO_E")), "000")
    '                .ZG5 = Str_Ginko(2)
    '                .ZG6 = Space(4)
    '                .ZG7 = Format(CInt(OBJ_DATAREADER_DREAD.Item("KAMOKU_E")), "0")
    '                .ZG8 = Format(CLng(OBJ_DATAREADER_DREAD.Item("KOUZA_E")), "0000000")
    '                .ZG9 = CStr(OBJ_DATAREADER_DREAD.Item("KEIYAKU_KNAME_E")).Trim
    '                .ZG10 = Format(lFurikae_Kingaku, "0000000000")
    '                .ZG11 = "0"
    '                .ZG12 = sJyuyouka_No
    '                .ZG13 = CInt(STR_生徒明細入出区分) & Space(9)      'スケジュール区分を設定→振替区分に変更 2006/12/22
    '                .ZG14 = "0"
    '                .ZG15 = Space(8)
    '            End With

    '            If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 2) = False Then
    '                If GFUNC_SELECT_SQL3("", 1) = False Then
    '                    Exit Sub
    '                End If

    '                Exit Sub
    '            End If

    '            '明細登録用振替データ(全銀フォーマットのデータレコード１２０BYTE)
    '            sBuff = PFUNC_GET_ZENGIN_LINE(2)

    '            With OBJ_DATAREADER_DREAD
    '                '口座振替明細作成
    '                STR_SQL = " INSERT INTO G_MEIMAST"
    '                STR_SQL += " values("
    '                STR_SQL += "'" & STR_生徒明細学校コード & "'"
    '                STR_SQL += ",'" & .Item("NENDO_E") & "'"
    '                STR_SQL += ",'" & STR_生徒明細振替日 & "'"
    '                STR_SQL += "," & .Item("GAKUNEN_CODE_E")
    '                STR_SQL += "," & .Item("CLASS_CODE_E")
    '                STR_SQL += ",'" & .Item("SEITO_NO_E") & "'"
    '                STR_SQL += "," & .Item("TUUBAN_E")
    '                STR_SQL += ",'" & strTKIN_NO_GAK & "'"
    '                STR_SQL += ",'" & strTSIT_NO_GAK & "'"
    '                STR_SQL += ",'" & strKAMOKU_GAK & "'"
    '                STR_SQL += ",'" & strKOUZA_GAK & "'"
    '                STR_SQL += ",'" & .Item("TKIN_NO_E") & "'"
    '                STR_SQL += ",'" & .Item("TSIT_NO_E") & "'"
    '                STR_SQL += ",'" & .Item("KAMOKU_E") & "'"
    '                STR_SQL += ",'" & .Item("KOUZA_E") & "'"
    '                STR_SQL += ",'" & .Item("KEIYAKU_KNAME_E") & "'"
    '                STR_SQL += ",'" & Str_Seikyu_Nentuki.Substring(4, 2) & "ｶﾞﾂﾄﾞ'"
    '                STR_SQL += ",'" & sJyuyouka_No & CInt(STR_生徒明細入出区分) & Space(9) & "'" '振替区分に変更 2006/12/22
    '                STR_SQL += ",'" & sBuff & "'"
    '                STR_SQL += "," & lRecordCount
    '                STR_SQL += ",'" & Mid(STR_生徒明細振替日, 1, 6) & "'"
    '                STR_SQL += ",'" & Mid(STR_生徒明細振替日, 1, 6) & "'"
    '                STR_SQL += ",'000'"
    '                STR_SQL += "," & lFurikae_Kingaku
    '                For iLcount = 1 To 15
    '                    STR_SQL += ",0"
    '                Next iLcount
    '                STR_SQL += ",0"
    '                STR_SQL += ",'0'"
    '                '2006/10/20 入金と出金で振替区分を変更する
    '                'STR_SQL += ",'2'"
    '                Select Case (CInt(STR_生徒明細入出区分))
    '                    Case 2
    '                        '入金
    '                        STR_SQL += ",'2'"
    '                    Case 3
    '                        '随時出金
    '                        STR_SQL += ",'3'"
    '                End Select
    '                STR_SQL += ",'" & Str_Syori_Date(1) & "'"
    '                STR_SQL += ",' '"
    '                STR_SQL += ",' '"
    '                STR_SQL += ")"

    '                '明細マスタ登録
    '                Select Case (PFUNC_Chk_Meisai())
    '                    Case -1
    '                        'エラー有(異常リストに追加 , 明細に登録なし , スケジュール更新なし)
    '                        Lng_Err_Count += 1

    '                        Call PSUB_Insert_IjyoList()
    '                    Case 0
    '                        '正常終了
    '                        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
    '                            '異常リスト追加
    '                            Lng_Err_Count += 1
    '                            Str_Err_Msg = "データベースエラーです"

    '                            Call PSUB_Insert_IjyoList()
    '                        Else
    '                            lTotal_Kingaku += lFurikae_Kingaku
    '                            lTotal_Kensuu += 1

    '                            lRecordCount += 1

    '                            '学年毎の件数取得 2006/10/05
    '                            lngGAK_SYORI_KEN(CInt(.Item("GAKUNEN_CODE_E"))) += 1
    '                            dblGAK_SYORI_KIN(CInt(.Item("GAKUNEN_CODE_E"))) += lFurikae_Kingaku
    '                        End If
    '                    Case Else
    '                        'エラー有(異常リストに追加 , 明細に登録あり , スケジュール更新あり)
    '                        lThrrowCount += 1
    '                        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
    '                            '異常リスト追加
    '                            Lng_Err_Count += 1
    '                            Str_Err_Msg = "データベースエラーです"

    '                            Call PSUB_Insert_IjyoList()
    '                        Else
    '                            lTotal_Kingaku += lFurikae_Kingaku
    '                            lTotal_Kensuu += 1

    '                            lRecordCount += 1
    '                        End If

    '                        Lng_Err_Count += 1

    '                        Call PSUB_Insert_IjyoList()
    '                End Select
    '            End With

    'NEXT_SEITO:
    '        End While

    '        If GFUNC_SELECT_SQL3("", 1) = False Then
    '            If OBJ_TRANSACTION.Connection Is Nothing Then
    '            Else
    '                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then 'Commit処理
    '                    Exit Sub
    '                End If
    '            End If

    '            Exit Sub
    '        End If

    '        '該当データが０件の場合終了 2006/10/06
    '        If lTotal_Kensuu = 0 Then
    '            Err.Number = 0
    '            FileClose(iFileNo)
    '            If Dir$(Str_ZenginFile) <> "" Then Kill(Str_ZenginFile)

    '            If OBJ_TRANSACTION.Connection Is Nothing Then
    '            Else
    '                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then 'Commit処理
    '                    Exit Sub
    '                End If
    '            End If

    '            Exit Sub
    '        Else
    '            lngTAKOU_SYORISAKI += 1
    '        End If

    '        '全銀データ作成(トレーラー行｢集計結果｣)
    '        '作成した件数と金額を設定
    '        '前月の不能分は再振種別が0以外の場合のみ集計結果に加算される
    '        '(全銀データ作成(明細)時も同様)
    '        With gZENGIN_REC8
    '            .ZG1 = "8"
    '            .ZG2 = Format(lTotal_Kensuu, "000000")
    '            .ZG3 = Format(lTotal_Kingaku, "000000000000")
    '            .ZG4 = "000000"
    '            .ZG5 = "000000000000"
    '            .ZG6 = "000000"
    '            .ZG7 = "000000000000"
    '            .ZG8 = Space(65)
    '        End With

    '        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 8) = False Then
    '            Call GSUB_LOG(0, "全銀ﾌｧｲﾙトレーラレコード書き込みエラー")
    '            If OBJ_TRANSACTION.Connection Is Nothing Then
    '            Else
    '                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
    '                    Exit Sub
    '                End If
    '            End If
    '            Exit Sub
    '        End If

    '        '全銀データ作成(終了行)
    '        'データ区分
    '        'ダミー
    '        With gZENGIN_REC9
    '            .ZG1 = "9"
    '            .ZG2 = Space(119)
    '        End With

    '        If PFUNC_DAT_ZENGIN_WRITE(iFileNo, 9) = False Then
    '            Call GSUB_LOG(0, "全銀ﾌｧｲﾙエンドレコード書き込みエラー")
    '            If OBJ_TRANSACTION.Connection Is Nothing Then
    '            Else
    '                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
    '                    Exit Sub
    '                End If
    '            End If
    '            Exit Sub
    '        End If

    '        Err.Number = 0

    '        FileClose(iFileNo)

    '        If Err.Number <> 0 Then
    '            Call GSUB_LOG(0, "全銀ﾌｧｲﾙCLOSEエラー")
    '            If OBJ_TRANSACTION.Connection Is Nothing Then
    '            Else
    '                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
    '                    Exit Sub
    '                End If
    '            End If
    '            Exit Sub
    '        End If

    '        ''現行の処理だと最後の金融機関の集計行からの全銀ファイルと他行ｽｹｼﾞｭｰﾙの作成が
    '        ''されないのでここで最後の金融機関の全銀ファイルと他行ｽｹｼﾞｭｰﾙの作成を行う
    '        ''↓最終金融機関集計行＆FD保存処理
    '        ''他行ｽｹｼﾞｭｰﾙ作成
    '        ''同金融機関の学年ごとで他行ｽｹｼﾞｭｰﾙを作成
    '        'If PFUNC_DelIns_TakouSchedule(sEsc_Gakunen, pGinko_Code, lSyukei(0), lSyukei(1)) = False Then
    '        '    Call GSUB_LOG(0, "他行ｽｹｼﾞｭｰﾙ作成エラー")

    '        '    Exit Sub
    '        'End If


    '        '同一金融機関・学年ごとで他行スケジュール作成 2006/10/05
    '        For i As Integer = 1 To 9
    '            If lngGAK_SYORI_KEN(i) = 0 Then
    '                GoTo NEXT_FOR
    '            End If
    '            '他行ｽｹｼﾞｭｰﾙ作成
    '            '同金融機関の学年ごとで他行ｽｹｼﾞｭｰﾙを作成
    '            If PFUNC_DelIns_TakouSchedule(STR_生徒明細学校コード, i, 0, pGinko_Code) = False Then
    '                If OBJ_TRANSACTION.Connection Is Nothing Then
    '                Else
    '                    If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
    '                        Exit Sub
    '                    End If
    '                End If
    '                Exit Sub
    '            End If
    'NEXT_FOR:
    '        Next


    '        If OBJ_TRANSACTION.Connection Is Nothing Then
    '        Else
    '            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
    '                Exit Sub
    '            End If
    '        End If


    '        If Lng_Err_Count = 0 Or Lng_Err_Count = lThrrowCount Then

    '            'FD保存
    '            Select Case (GFUNC_FD_Copy(Me, STR_生徒明細学校名, Str_ZenginFile, Trim(pFileName), Str_Baitai_Code, pGinko_Code))
    '                Case 0
    '                    '正常終了
    '                    Call GSUB_LOG(1, "随時データ作成")
    '                Case 1
    '                    'キャンセル
    '                    flgNEXT_DATA_MAKE = False '中断
    '                    Exit Sub
    '                Case Else
    '                    'エラー
    '                    Call GSUB_LOG(0, "全銀ﾌｧｲﾙFD保存エラー")

    '                    Exit Sub
    '            End Select
    '            '↑最終金融機関集計行＆FD保存処理
    '        End If

    '        Lng_Trw_Count += lThrrowCount

    '    End Sub
    Private Sub PSUB_GET_GINKONAME(ByVal pGinko_Code As String, ByVal pSiten_Code As String, ByVal db As MyOracle)

        '金融機関コードと支店コードから金融機関名、支店名を抽出

        Str_Ginko(0) = ""
        Str_Ginko(1) = ""
        Str_Ginko(2) = ""
        Str_Ginko(3) = ""

        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            If pGinko_Code.Trim = "" OrElse pSiten_Code.Trim = "" Then
                Exit Sub
            End If

            Dim SQL As New System.Text.StringBuilder(128)

            SQL.Append(" SELECT ")
            SQL.Append(" KIN_KNAME_N ")
            SQL.Append(",KIN_NNAME_N ")
            SQL.Append(",SIT_KNAME_N ")
            SQL.Append(",SIT_NNAME_N ")
            SQL.Append(" FROM TENMAST ")
            SQL.Append(" WHERE KIN_NO_N = '" & pGinko_Code & "'")
            SQL.Append(" AND SIT_NO_N = '" & pSiten_Code & "'")

            Orareader = New CASTCommon.MyOracleReader(db)

            If Orareader.DataReader(SQL) Then
                Str_Ginko(0) = Orareader.GetItem("KIN_KNAME_N")
                Str_Ginko(1) = Orareader.GetItem("KIN_NNAME_N")
                Str_Ginko(2) = Orareader.GetItem("SIT_KNAME_N")
                Str_Ginko(3) = Orareader.GetItem("SIT_NNAME_N")
            Else
                Exit Sub
            End If

            Orareader.Close()
            Orareader = Nothing

        Catch ex As Exception
            Throw New Exception("TENMAST取得失敗", ex)
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
                Orareader = Nothing
            End If
        End Try

    End Sub
#End Region

#Region " Private Function "

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
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S ='" & pGakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND"
        STR_SQL += " ENTRI_FLG_S ='1'"
        STR_SQL += " AND"
        STR_SQL += " TYUUDAN_FLG_S ='0'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S ='2'" '随時のみ 2006/10/16
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S ='2'" '随時のみ 2006/10/16


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
                    Select Case (CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S")))
                        Case 1
                            pSiyou_gakunen(iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                    End Select
                Next iLoopCount
            End With
        End While

        If GFUNC_SELECT_SQL2(STR_SQL, 1) = False Then
            Exit Function
        End If

        '使用学年全てに学年フラグがある場合は全学年対象として扱う
        '学年
        For iLoopCount = 1 To iMaxGakunen
            Select Case (pSiyou_gakunen(iLoopCount))
                Case Is <> 1
                    PFUNC_Get_Gakunen = iMaxGakunen

                    Exit Function
            End Select
        Next iLoopCount

        PFUNC_Get_Gakunen = 0

    End Function

    Private Function PFUNC_Spread_Set() As Boolean

        Dim iNo As Integer

        Dim MainDB As New MyOracle

        PFUNC_Spread_Set = False

        Try

            Select Case (STR_生徒明細入出区分)
                Case "2"
                    iNo = 1
                Case "3"
                    iNo = 2
            End Select

            'エントリマスタを検索する

            'スプレッドヘッダの編集
            Select Case (iNo)
                Case 1
                    DataGridView.Columns(17).HeaderText = "入金金額"
                Case 2
                    DataGridView.Columns(17).HeaderText = "出金金額"
            End Select

            'エントリマスタ検索のSQL文作成
            STR_SQL = " SELECT "
            STR_SQL += " G_ENTMAST" & iNo & ".*"
            STR_SQL += " FROM "
            STR_SQL += " G_ENTMAST" & iNo
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_E ='" & STR_生徒明細学校コード & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_E ='" & STR_生徒明細振替日 & "'"
            STR_SQL += " ORDER BY "
            Select Case (STR_生徒明細ソート順)
                Case "1"
                    '学年、クラス、生徒番号
                    STR_SQL += " GAKUNEN_CODE_E ASC, CLASS_CODE_E ASC, SEITO_NO_E ASC"
                Case "2"
                    '入学年度、通番
                    STR_SQL += " GAKUNEN_CODE_E ASC, NENDO_E ASC, TUUBAN_E ASC"
                Case "3"
                    '生徒名のアイウエオ順
                    STR_SQL += " GAKUNEN_CODE_E ASC, SEITO_KNAME_E ASC"
            End Select

            'エントリマスタ存在チェック
            If GFUNC_ISEXIST(STR_SQL) = False Then
                MessageBox.Show(G_MSG0007W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Function
            End If

            INTCNT01 = 0
            Bln_Ginko_Flg(0) = False
            Bln_Ginko_Flg(1) = False

            If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
                Exit Function
            End If

            With DataGridView
                Select Case (STR_生徒明細ソート順)
                    Case "1"
                        '学年、クラス、生徒番号
                        .Columns(0).Visible = False
                        .Columns(1).Visible = False
                        .Columns(2).Visible = True
                        .Columns(3).Visible = True
                        .Columns(4).Visible = True
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = False
                        .Columns(18).Visible = False
                    Case "2"
                        '入学年度、通番
                        .Columns(0).Visible = True
                        .Columns(1).Visible = True
                        .Columns(2).Visible = False
                        .Columns(3).Visible = False
                        .Columns(4).Visible = False
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = False
                        .Columns(18).Visible = False
                    Case "3"
                        '生徒名のアイウエオ順
                        .Columns(0).Visible = False
                        .Columns(1).Visible = False
                        .Columns(2).Visible = False
                        .Columns(3).Visible = False
                        .Columns(4).Visible = False
                        .Columns(5).Visible = False
                        .Columns(6).Visible = False
                        .Columns(7).Visible = True
                        .Columns(18).Visible = False
                End Select
            End With

            While (OBJ_DATAREADER.Read = True)
                With DataGridView
                    '行数の設定
                    Dim RowItem As New DataGridViewRow
                    RowItem.CreateCells(DataGridView)

                    '入学年度
                    RowItem.Cells(0).Value = OBJ_DATAREADER.Item("NENDO_E")
                    '通番
                    RowItem.Cells(1).Value = OBJ_DATAREADER.Item("TUUBAN_E")
                    '学年
                    RowItem.Cells(2).Value = OBJ_DATAREADER.Item("GAKUNEN_CODE_E")
                    'クラス
                    RowItem.Cells(3).Value = OBJ_DATAREADER.Item("CLASS_CODE_E")
                    '生徒番号
                    RowItem.Cells(4).Value = OBJ_DATAREADER.Item("SEITO_NO_E")
                    '生徒名カナ
                    RowItem.Cells(5).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                    '生徒名漢字 2007/02/10
                    If IsDBNull(OBJ_DATAREADER.Item("SEITO_NNAME_E")) = True Then
                        RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                    Else
                        If Trim(OBJ_DATAREADER.Item("SEITO_NNAME_E")) = "" Then 'スペースの場合カナ表示
                            RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_KNAME_E")
                        Else
                            RowItem.Cells(6).Value = OBJ_DATAREADER.Item("SEITO_NNAME_E")
                        End If
                    End If

                    '表示用生徒名
                    If RowItem.Cells(6).Value = "" Then
                        RowItem.Cells(7).Value = RowItem.Cells(5).Value
                    Else
                        RowItem.Cells(7).Value = RowItem.Cells(6).Value
                    End If

                    Select Case (OBJ_DATAREADER.Item("TKIN_NO_E"))
                        Case STR_JIKINKO_CODE
                            Bln_Ginko_Flg(0) = True
                        Case Else
                            Bln_Ginko_Flg(1) = True
                    End Select

                    Call PSUB_GET_GINKONAME(OBJ_DATAREADER.Item("TKIN_NO_E"), OBJ_DATAREADER.Item("TSIT_NO_E"), MainDB)

                    '金融機関コードの格納
                    RowItem.Cells(8).Value = OBJ_DATAREADER.Item("TKIN_NO_E")
                    RowItem.Cells(9).Value = Str_Ginko(1)

                    '支店コードの格納
                    RowItem.Cells(10).Value = OBJ_DATAREADER.Item("TSIT_NO_E")
                    RowItem.Cells(11).Value = Str_Ginko(3)

                    '科目コードの格納（２桁から１桁に変換）
                    Select Case (OBJ_DATAREADER.Item("KAMOKU_E"))
                        Case "01"
                            RowItem.Cells(12).Value = "2"
                        Case "02"
                            RowItem.Cells(12).Value = "1"
                        Case "05"
                            RowItem.Cells(12).Value = "3"
                        Case "37"
                            RowItem.Cells(12).Value = "4"
                        Case "04"
                            RowItem.Cells(12).Value = "9"
                        Case Else
                            RowItem.Cells(12).Value = "2"
                    End Select
                    '科目名の変換、格納
                    Select Case (OBJ_DATAREADER.Item("KAMOKU_E"))
                        '2011/06/16 標準版修正 科目が01の場合当座 ------------------START
                        'Case "02"
                        Case "01"
                            '2011/06/16 標準版修正 科目が01の場合当座 ------------------END
                            '当座
                            RowItem.Cells(13).Value = "当"
                        Case "03"
                            '納税
                            RowItem.Cells(13).Value = "納"
                        Case "04"
                            '職員
                            RowItem.Cells(13).Value = "職"
                        Case Else
                            '普通
                            'その他
                            RowItem.Cells(13).Value = "普"
                    End Select
                    '口座番号の格納
                    RowItem.Cells(14).Value = OBJ_DATAREADER.Item("KOUZA_E")

                    '契約者名の格納
                    '2006/12/08　データベースにはスペースが入っているため、IsDBNullでは空白判定できない
                    If IsDBNull(OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")) = True Then
                        RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_KNAME_E")
                    Else
                        If Trim(OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")) = "" Then 'スペースの場合カナ表示
                            RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_KNAME_E")
                        Else
                            RowItem.Cells(15).Value = OBJ_DATAREADER.Item("KEIYAKU_NNAME_E")
                        End If
                    End If

                    '契約者番号の格納
                    RowItem.Cells(16).Value = OBJ_DATAREADER.Item("KEIYAKU_NO_E")
                    '金額の格納
                    RowItem.Cells(17).ReadOnly = True
                    RowItem.Cells(17).Value = Format(CDbl(OBJ_DATAREADER.Item("FURIKIN_E")), "#,##0")

                    '手数料の格納
                    RowItem.Cells(18).Value = Format(CDbl(OBJ_DATAREADER.Item("TESUU_E")), "#,##0")

                    For Cnt As Integer = 0 To RowItem.Cells.Count - 1
                        RowItem.Cells(Cnt).Style.BackColor = Color.Yellow
                    Next

                    .Rows.Add(RowItem)

                    INTCNT01 += 1
                End With
            End While

            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If

            lab件数.Text = Format(CDbl(INTCNT01), "#,##0")

            With DataGridView
                Dim SumKin As Decimal = 0
                For cnt As Integer = 0 To INTCNT01 - 1
                    SumKin += CDec(.Rows(cnt).Cells(17).Value)
                Next
                '行数の設定
                .RowCount = INTCNT01 + 1
                '合計金額表示行
                .Rows(INTCNT01).Cells(17).ReadOnly = True
                .Rows(INTCNT01).Cells(17).Value = Format(SumKin, "#,##0")

                txt入力合計金額.Text = Format(CDbl(.Rows(INTCNT01).Cells(17).Value), "#,##0")
            End With


            PFUNC_Spread_Set = True

        Catch ex As Exception
            MainLog.Write("", "失敗", ex.Message)
            Return False
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

    End Function
    Private Function PFUNC_Delete_Meisai(ByVal pIndex As Integer) As Boolean

        PFUNC_Delete_Meisai = False

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        '---------------------
        '口座振替明細マスタ削除(自行／他行も含む)
        '----------------------
        STR_SQL = " DELETE  FROM G_MEIMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_M = '" & STR_生徒明細学校コード & "'"
        STR_SQL += " AND"
        STR_SQL += " SEIKYU_TAISYOU_M = '" & Str_Seikyu_Nentuki & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_M = '" & STR_生徒明細振替日 & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_M = '" & STR_生徒明細入出区分 & "'"
        '蒲郡信金向け　自行・他行一緒のファイル 2007/09/06
        'Select case pIndex
        '    Case 0
        '        '自行
        '        STR_SQL += " AND"
        '        STR_SQL += " TKIN_NO_M = '" & Str_Jikou_Ginko_Code & "'"
        '    Case 1
        '        '他行
        '        STR_SQL += " AND"
        '        STR_SQL += " TKIN_NO_M <> '" & Str_Jikou_Ginko_Code & "'"
        'End Select

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            MainLOG.Write("明細削除", "失敗", "他行スケジュール明細削除")
            If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                Exit Function
            End If
            Exit Function
        End If

        If pIndex = 1 Then '他行の場合 2006/10/17
            '他行スケジュールマスタ削除
            '※前仕様では１回の処理で１レコードしか作成されなかった為
            '　現行仕様では1回の処理で複数レコード存在する為ここで削除しておく
            STR_SQL = " DELETE  G_TAKOUSCHMAST"
            STR_SQL += " WHERE GAKKOU_CODE_U = '" & STR_生徒明細学校コード & "'"
            STR_SQL += " AND FURI_KBN_U = '" & STR_生徒明細入出区分 & "'"
            STR_SQL += " AND FURI_DATE_U = '" & STR_生徒明細振替日 & "'"
            STR_SQL += " AND TKIN_NO_U <> '" & STR_JIKINKO_CODE & "'"

            If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                MainLOG.Write("学校他行マスタ削除", "失敗", "他行スケジュール削除")

                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Exit Function
                End If
                Exit Function
            End If

        End If

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        PFUNC_Delete_Meisai = True

    End Function
    Private Function PFUNC_Get_Gakunen() As Boolean

        '年間スケジュールから随時処理のものを検索する
        Dim iLoopCount As Integer
        Dim iGakunenCount As Integer

        PFUNC_Get_Gakunen = False

        STR_SQL = " SELECT "
        STR_SQL += " G_SCHMAST.* "
        STR_SQL += ", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T"
        STR_SQL += " FROM G_SCHMAST , GAKMAST2"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_S ='" & STR_生徒明細学校コード & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & STR_生徒明細振替日 & "'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S = '2'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S ='" & STR_生徒明細入出区分 & "'"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            Exit Function
        End If

        'スケジュールマスタ存在チェック
        If OBJ_DATAREADER.HasRows = False Then
            MessageBox.Show(G_MSG0008W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        OBJ_DATAREADER.Read()

        iGakunenCount = 1

        With OBJ_DATAREADER

            For iLoopCount = 1 To CInt(.Item("SIYOU_GAKUNEN_T"))
                If .Item("GAKUNEN" & iLoopCount & "_FLG_S") = "1" Then
                    ReDim Preserve Str_Gakunen_Flg(iGakunenCount)
                    Str_Gakunen_Flg(iGakunenCount) = iLoopCount
                    iGakunenCount += 1
                End If
            Next

            '使用学年数が学校マスタで設定されている使用学年数と一致する場合は
            '全学年が抽出対象
            If CInt(.Item("SIYOU_GAKUNEN_T")) = (UBound(Str_Gakunen_Flg) - 1) Then
                Bln_Gakunen_Flg = False
            Else
                Bln_Gakunen_Flg = True
            End If
            '追加 2006/10/16
            Str_Seikyu_Nentuki = .Item("NENGETUDO_S")
            strFUNOU_YDATE = .Item("FUNOU_YDATE_S")
        End With

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Gakunen = True

    End Function
    Private Function PFUNC_Update_Schedule(ByVal pNo As Integer) As Boolean

        On Error Resume Next

        Dim iG_Flg(9) As Integer

        Dim bG_Flg As Boolean

        PFUNC_Update_Schedule = False

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then
            Exit Function
        End If

        STR_SQL = " SELECT "
        STR_SQL += " SFURI_DATE_S"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " FURI_KBN_S ='" & STR_生徒明細入出区分 & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & STR_生徒明細振替日 & "'"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_S = '" & STR_生徒明細学校コード & "'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S ='2'"
        STR_SQL += " group by SFURI_DATE_S"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            '取得したプライマリ情報で単一のスケジュールを取得
            '※集計するスケジュールの対象学年を取得する為
            If PFUNC_Get_Gakunen2(OBJ_DATAREADER_DREAD.Item("SFURI_DATE_S"), iG_Flg) = False Then
                GoTo NextWhile
            End If

            bG_Flg = False

            '取得したプライマリ情報と対象学年情報で集計をかける
            '※これによってスケジュールに更新すべき集計値が取得できたことになる
            STR_SQL = " SELECT "
            STR_SQL += " sum(1) as KENSUU"
            STR_SQL += ",sum(FURIKIN_E) as KINGAKU"
            STR_SQL += " FROM "
            STR_SQL += " G_ENTMAST" & pNo & ""
            STR_SQL += " WHERE"
            STR_SQL += " GAKKOU_CODE_E = '" & STR_生徒明細学校コード & "'"
            STR_SQL += " AND"
            STR_SQL += " FURI_DATE_E ='" & STR_生徒明細振替日 & "'"
            '2007/10/05　追加：振替金額が0円の明細はカウントしない---------
            STR_SQL += " AND"
            STR_SQL += " FURIKIN_E <> 0"
            '--------------------------------------------------------------
            STR_SQL += " AND ("
            For i As Integer = 1 To 9
                If iG_Flg(i) = 1 Then
                    If bG_Flg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_E=" & i
                    bG_Flg = True
                End If
            Next i
            STR_SQL += " )"

            If GFUNC_SELECT_SQL4(STR_SQL, 0) = False Then
                If GFUNC_SELECT_SQL3("", 1) = False Then
                    Exit Function
                End If

                Exit Function
            End If

            If OBJ_DATAREADER_DREAD2.HasRows = True Then
                OBJ_DATAREADER_DREAD2.Read()

                'スケジュールマスタ振替データ作成部分更新
                STR_SQL = " UPDATE  G_SCHMAST SET "
                STR_SQL += " DATA_DATE_S='" & Str_Syori_Date(0) & "'"
                STR_SQL += ",DATA_FLG_S='1'"
                STR_SQL += ",TIME_STAMP_S='" & Str_Syori_Date(1) & "'"
                STR_SQL += ",SYORI_KEN_S=" & CLng(OBJ_DATAREADER_DREAD2.Item("KENSUU"))
                STR_SQL += ",SYORI_KIN_S=" & CDbl(OBJ_DATAREADER_DREAD2.Item("KINGAKU"))
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_S = '" & STR_生徒明細学校コード & "'"
                STR_SQL += " AND"
                STR_SQL += " NENGETUDO_S = '" & Mid(STR_生徒明細振替日, 1, 6) & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_S = '" & STR_生徒明細振替日 & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_S = '" & STR_生徒明細入出区分 & "'"
                STR_SQL += " AND"
                STR_SQL += " SFURI_DATE_S ='" & OBJ_DATAREADER_DREAD.Item("SFURI_DATE_S") & "'"
                STR_SQL += " AND"
                STR_SQL += " SCH_KBN_S = '2'"

                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                    If GFUNC_SELECT_SQL3("", 1) = False Then
                        Exit Function
                    End If

                    Exit Function
                End If
            End If

            If GFUNC_SELECT_SQL4("", 1) = False Then
                If GFUNC_SELECT_SQL3("", 1) = False Then
                    Exit Function
                End If

                Exit Function
            End If
NextWhile:
        End While

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_Update_Schedule = True

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
        STR_SQL += ",'2'"
        STR_SQL += ",'" & STR_生徒明細入出区分 & "'"
        STR_SQL += ",'" & STR_生徒明細振替日 & "'"
        STR_SQL += ",'" & strFUNOU_YDATE & "'"
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

    Private Function PFUNC_DAT_ZENGIN_WRITE(ByVal pFileNo As Integer, ByVal pZengin_kbn As Integer) As Boolean

        On Error Resume Next

        PFUNC_DAT_ZENGIN_WRITE = False

        Err.Number = 0

        Select Case (pZengin_kbn)
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

        If Err.Number <> 0 Then

            Str_Err_Msg = Err.Description

            Exit Function
        End If

        PFUNC_DAT_ZENGIN_WRITE = True

    End Function
    Private Function PFUNC_GET_ZENGIN_LINE(ByVal pIndex As Integer) As String

        Dim sLine As String

        PFUNC_GET_ZENGIN_LINE = ""
        sLine = ""

        Select Case (pIndex)
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
        Dim sEntriName As String = ""

        PFUNC_Get_Takou_Ginko = False

        Select Case (STR_生徒明細入出区分)
            Case "2"
                sEntriName = "G_ENTMAST1"
            Case "3"
                sEntriName = "G_ENTMAST2"
        End Select

        '処理対象となる他行一覧を取得
        'エントリマスタ取得(自行のみ)
        STR_SQL = " SELECT "
        STR_SQL += " GAKKOU_CODE_V,TKIN_NO_E "
        STR_SQL += ", ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V,CODE_KBN_V"
        STR_SQL += " FROM "
        STR_SQL += " " & sEntriName & " , G_TAKOUMAST"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_E = GAKKOU_CODE_V"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_E = TKIN_NO_V"
        STR_SQL += " AND"
        STR_SQL += " TKIN_NO_E <> '" & STR_JIKINKO_CODE & "'"
        STR_SQL += " AND"
        STR_SQL += " FURIKIN_E > 0"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_E = '" & STR_生徒明細学校コード & "'"
        '2007/02/14 条件に振替日追加
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_E = '" & STR_生徒明細振替日 & "'"
        STR_SQL += " group by GAKKOU_CODE_V,TKIN_NO_E ,  ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V,CODE_KBN_V"
        STR_SQL += " ORDER BY GAKKOU_CODE_V,TKIN_NO_E,  ITAKU_CODE_V , KAMOKU_V  , KOUZA_V , SFILE_NAME_V,CODE_KBN_V"

        If GFUNC_SELECT_SQL2(STR_SQL, 0) = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If
            MessageBox.Show("他行情報の検索に失敗しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        If OBJ_DATAREADER.HasRows = False Then
            If GFUNC_SELECT_SQL2("", 1) = False Then
                Exit Function
            End If
            MessageBox.Show("随時データ作成対象の生徒が存在しません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        lCount = 1

        While (OBJ_DATAREADER.Read = True)
            With OBJ_DATAREADER
                ReDim Preserve pTakou_Ginko(6, lCount)

                pTakou_Ginko(0, lCount) = .Item("TKIN_NO_E")
                pTakou_Ginko(1, lCount) = "" '.Item("TSIT_NO_E")
                pTakou_Ginko(2, lCount) = .Item("ITAKU_CODE_V")
                pTakou_Ginko(3, lCount) = .Item("KAMOKU_V")
                pTakou_Ginko(4, lCount) = .Item("KOUZA_V")
                If IsDBNull(.Item("SFILE_NAME_V")) = True Then
                    'S+学校コード＋金融機関コード+.dat
                    pTakou_Ginko(5, lCount) = "S" & CStr(.Item("GAKKOU_CODE_V")).Trim & CStr(.Item("TKIN_NO_E")).Trim & ".dat"
                Else
                    If CStr(.Item("SFILE_NAME_V")).Trim = "" Then
                        pTakou_Ginko(5, lCount) = "S" & CStr(.Item("GAKKOU_CODE_V")).Trim & CStr(.Item("TKIN_NO_E")).Trim & ".dat"
                    Else
                        pTakou_Ginko(5, lCount) = CStr(.Item("SFILE_NAME_V")).Trim
                    End If
                End If
                'コード区分追加 2006/10/16
                pTakou_Ginko(6, lCount) = .Item("CODE_KBN_V")

                lCount += 1
            End With
        End While

        If GFUNC_SELECT_SQL2("", 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Takou_Ginko = True

    End Function
    Private Function PFUNC_Chk_Meisai() As Integer

        Dim Chk_Sql As String

        PFUNC_Chk_Meisai = -1

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

                PFUNC_Chk_Meisai = 1

                Str_Err_Msg = "金融機関マスタに登録されていません。"

                Exit Function
            End If

            If GFUNC_SELECT_SQL5("", 1) = False Then
                Exit Function
            End If
        End If

        '口座番号規定桁チェック
        Select Case (Len(Trim(Str_Err_Kouza)))
            Case Is <> 7
                Str_Err_Msg = "口座番号の桁が７桁以外です"

                Exit Function
        End Select

        '口座番号桁数ALLZERO , ALL9 チェック
        '
        Select Case (Trim(Str_Err_Kouza))
            Case "0000000"
                Str_Err_Msg = "口座番号にALLZERO値は設定できません"

                Exit Function
            Case "9999999999"
                Str_Err_Msg = "口座番号にALL9値は設定できません"

                Exit Function
        End Select

        PFUNC_Chk_Meisai = 0

    End Function

    Private Function PFUNC_Get_Gakunen2(ByVal pSFuri_Date As String, _
                                        ByRef pSiyou_gakunen() As Integer) As Boolean

        Dim iMaxGakunen As Integer

        ReDim pSiyou_gakunen(9)

        PFUNC_Get_Gakunen2 = False

        '選択された学校に存在するスケジュールより
        '再振替日までを抽出条件とし、
        'スケジュール毎に集計をかける学年を取得する
        STR_SQL = " SELECT "
        STR_SQL += " SCH_KBN_S"
        For iLoopCount As Integer = 1 To 9
            STR_SQL += ", GAKUNEN" & iLoopCount & "_FLG_S"
            pSiyou_gakunen(iLoopCount) = 0
        Next iLoopCount
        STR_SQL += ", SIYOU_GAKUNEN_T , SAIKOU_GAKUNEN_T"
        STR_SQL += " FROM G_SCHMAST"
        STR_SQL += " left join GAKMAST2 on "
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S ='" & STR_生徒明細学校コード & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_DATE_S ='" & STR_生徒明細振替日 & "'"
        STR_SQL += " AND"
        STR_SQL += " SFURI_DATE_S ='" & pSFuri_Date & "'"
        STR_SQL += " AND"
        STR_SQL += " CHECK_FLG_S ='1'"
        STR_SQL += " AND"
        STR_SQL += " TYUUDAN_FLG_S ='0'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S = '2'"

        If GFUNC_SELECT_SQL4(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD2.HasRows = False Then
            If GFUNC_SELECT_SQL4("", 1) = False Then
                Exit Function
            End If

            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD2.Read)
            With OBJ_DATAREADER_DREAD2
                iMaxGakunen = CInt(.Item("SIYOU_GAKUNEN_T"))
                For iLoopCount As Integer = 1 To iMaxGakunen
                    Select Case (CInt(.Item("GAKUNEN" & iLoopCount & "_FLG_S")))
                        Case 1
                            pSiyou_gakunen(iLoopCount) = .Item("GAKUNEN" & iLoopCount & "_FLG_S")
                    End Select
                Next iLoopCount
            End With
        End While

        If GFUNC_SELECT_SQL4("", 1) = False Then
            Exit Function
        End If

        PFUNC_Get_Gakunen2 = True

    End Function

    Private Function PFUNC_Query_String(ByVal astrKINKO_CD As String) As String

        Dim sQuery As String

        PFUNC_Query_String = ""

        sQuery = "SELECT TENMAST.SIT_NO_N, GAKMAST1.GAKKOU_CODE_G, GAKMAST1.GAKKOU_NNAME_G, HIMOMAST.HIMOKU_NAME01_H, HIMOMAST.HIMOKU_NAME02_H, HIMOMAST.HIMOKU_NAME04_H, HIMOMAST.HIMOKU_NAME06_H, HIMOMAST.HIMOKU_NAME07_H, HIMOMAST.HIMOKU_NAME08_H, HIMOMAST.HIMOKU_NAME09_H, HIMOMAST.HIMOKU_NAME10_H, HIMOMAST.HIMOKU_NAME11_H, HIMOMAST.HIMOKU_NAME12_H, HIMOMAST.HIMOKU_NAME13_H, HIMOMAST.HIMOKU_NAME14_H, HIMOMAST.HIMOKU_NAME15_H, HIMOMAST.HIMOKU_NAME03_H, HIMOMAST.HIMOKU_NAME05_H, TENMAST.KIN_NO_N, G_SCHMAST.FURI_DATE_S, SEITOMASTVIEW.GAKUNEN_CODE_O, SEITOMASTVIEW.SEITO_NO_O, SEITOMASTVIEW.SEITO_KNAME_O, SEITOMASTVIEW.SEITO_NNAME_O, SEITOMASTVIEW.MEIGI_KNAME_O, TENMAST.SIT_NNAME_N, SEITOMASTVIEW.TYOUSI_FLG_O, G_SCHMAST.FUNOU_FLG_S, G_MEIMAST.TKAMOKU_M, G_MEIMAST.GAKKOU_CODE_M, G_MEIMAST.GAKUNEN_CODE_M, G_MEIMAST.HIMOKU1_KIN_M, G_MEIMAST.HIMOKU2_KIN_M, G_MEIMAST.HIMOKU3_KIN_M, G_MEIMAST.HIMOKU4_KIN_M, G_MEIMAST.HIMOKU5_KIN_M, G_MEIMAST.HIMOKU6_KIN_M, G_MEIMAST.HIMOKU7_KIN_M, G_MEIMAST.HIMOKU8_KIN_M, G_MEIMAST.HIMOKU9_KIN_M, G_MEIMAST.HIMOKU10_KIN_M, G_MEIMAST.HIMOKU11_KIN_M, G_MEIMAST.HIMOKU12_KIN_M, G_MEIMAST.HIMOKU13_KIN_M, G_MEIMAST.HIMOKU14_KIN_M, G_MEIMAST.HIMOKU15_KIN_M, G_MEIMAST.TUUBAN_M, G_MEIMAST.NENDO_M, G_MEIMAST.CLASS_CODE_M, G_MEIMAST.SEITO_NO_M, G_MEIMAST.TKOUZA_M FROM   KZFMAST.G_MEIMAST G_MEIMAST, KZFMAST.SEITOMASTVIEW SEITOMASTVIEW, KZFMAST.G_SCHMAST G_SCHMAST, KZFMAST.HIMOMAST HIMOMAST, KZFMAST.GAKMAST1 GAKMAST1, KZFMAST.TENMAST TENMAST "

        sQuery += " WHERE"
        sQuery += " ((((((G_MEIMAST.GAKKOU_CODE_M=SEITOMASTVIEW.GAKKOU_CODE_O) AND (G_MEIMAST.NENDO_M=SEITOMASTVIEW.NENDO_O)) AND (G_MEIMAST.GAKUNEN_CODE_M=SEITOMASTVIEW.GAKUNEN_CODE_O)) AND (G_MEIMAST.CLASS_CODE_M=SEITOMASTVIEW.CLASS_CODE_O)) AND (G_MEIMAST.SEITO_NO_M=SEITOMASTVIEW.SEITO_NO_O)) AND (G_MEIMAST.TUUBAN_M=SEITOMASTVIEW.TUUBAN_O)) AND (((G_MEIMAST.GAKKOU_CODE_M=G_SCHMAST.GAKKOU_CODE_S) AND (G_MEIMAST.FURI_DATE_M=G_SCHMAST.FURI_DATE_S)) AND (G_MEIMAST.FURI_KBN_M=G_SCHMAST.FURI_KBN_S)) AND ((((SEITOMASTVIEW.GAKKOU_CODE_O=HIMOMAST.GAKKOU_CODE_H (+)) AND (SEITOMASTVIEW.GAKUNEN_CODE_O=HIMOMAST.GAKUNEN_CODE_H (+))) AND (SEITOMASTVIEW.HIMOKU_ID_O=HIMOMAST.HIMOKU_ID_H (+))) AND (SEITOMASTVIEW.TUKI_NO_O=HIMOMAST.TUKI_NO_H (+))) AND ((SEITOMASTVIEW.GAKKOU_CODE_O=GAKMAST1.GAKKOU_CODE_G (+)) AND (SEITOMASTVIEW.GAKUNEN_CODE_O=GAKMAST1.GAKUNEN_CODE_G (+))) AND ((SEITOMASTVIEW.TKIN_NO_O=TENMAST.KIN_NO_N (+)) AND (SEITOMASTVIEW.TSIT_NO_O=TENMAST.SIT_NO_N (+))) "

        sQuery += " AND"
        sQuery += " G_MEIMAST.GAKKOU_CODE_M = '" & STR_生徒明細学校コード & "'"
        sQuery += " AND"
        sQuery += " G_MEIMAST.YOBI1_M = '" & Str_Syori_Date(1) & "'"
        sQuery += " AND"
        sQuery += " SEITOMASTVIEW.TUKI_NO_O = '" & Mid(Trim(STR_生徒明細振替日), 5, 2) & "'"
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
        ''金庫ごとに出力2006/10/17→蒲郡は自行・他行一緒に出力
        'sQuery += " AND"
        'sQuery += " G_MEIMAST.TKIN_NO_M = '" & astrKINKO_CD & "'"

        sQuery += " ORDER BY"
        If STR_生徒明細印刷区分 = "2" Then
            sQuery += " G_MEIMAST.TKIN_NO_M , G_MEIMAST.TSIT_NO_M , "
        End If

        Select Case (intPRNT_SORT) '学校マスタに登録された「帳票ソート順」で指定 2006/10/18
            Case 0
                '学年,クラス,生徒番号
                sQuery += " G_MEIMAST.GAKUNEN_CODE_M ASC, G_MEIMAST.CLASS_CODE_M ASC, G_MEIMAST.SEITO_NO_M ASC, G_MEIMAST.NENDO_M ASC, G_MEIMAST.TUUBAN_M ASC"
            Case 1
                '入学年度,通番
                sQuery += " G_MEIMAST.GAKUNEN_CODE_M ASC, G_MEIMAST.NENDO_M ASC, G_MEIMAST.TUUBAN_M ASC"
            Case 2
                'あいうえお(生徒名(ｶﾅ))
                sQuery += " G_MEIMAST.GAKUNEN_CODE_M ASC, SEITOMASTVIEW.SEITO_KNAME_O ASC, G_MEIMAST.NENDO_M ASC, G_MEIMAST.TUUBAN_M ASC"
        End Select

        PFUNC_Query_String = sQuery

    End Function


    Public Function GFUNC_FD_Copy3(ByVal pForm As Form, _
                              ByVal pTitleName As String, _
                              ByVal pSouceFilePath As String, _
                              ByVal pInitialFileName As String, _
                              ByVal pBaitai As String) As Integer
        '2005/03/14
        Dim sPath As String = ""
        Dim sBuff As String = ""

        Dim oDlg As New SaveFileDialog

        On Error Resume Next

        GFUNC_FD_Copy3 = -1

        '--------------------
        'FD保存
        '--------------------
        '--------------------
        '振替データ保存先パス
        '--------------------
        ' 2017/06/07 タスク）綾部 CHG (RSV2標準対応 媒体コード調整) -------------------- START
        'Select Case (pBaitai)
        '    Case "0"
        '        sPath = STR_IFL_PATH
        '    Case "1"
        '        sPath = "A:\"
        'End Select
        Select Case pBaitai
            Case "1"
                sPath = "A:\"
            Case Else
                sPath = STR_IFL_PATH
        End Select
        ' 2017/06/07 タスク）綾部 CHG (RSV2標準対応 媒体コード調整) -------------------- END

        If sPath = "" Then
            'iniファイルにTAKIRAIFL格納先情報が設定されていません
            MessageBox.Show(String.Format(MSG0001E, "依頼ファイル格納先", "GCOMMON", "IRAIFL"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End If

        Select Case (StrConv(Mid(sPath, 1, 1), vbProperCase))
            Case "A", "B"
                'ＦＤ読み取り処理
                '確認メッセージ表示
                If MessageBox.Show(G_MSG0002I, msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> Windows.Forms.DialogResult.OK Then
                    GFUNC_FD_Copy3 = 1
                    Exit Function
                End If
        End Select
        Select Case (StrConv(Mid(sPath, 1, 1), vbProperCase))
            Case "A", "B"

                With oDlg
                    .Filter = STR_DLG_FILTER_NAME & " (" & STR_DLG_FILTER & ")|" & STR_DLG_FILTER


                    .FilterIndex = 1
                    .InitialDirectory = sPath

                    .DefaultExt = STR_DEF_FILE_KBN
                    If Trim(pInitialFileName) <> "" Then
                        .FileName = pInitialFileName
                    End If
                    .Title = "[" & pTitleName & "]振替データ保存"
                    .ShowDialog()
                    sBuff = .FileName
                End With


                If sBuff = pInitialFileName Or Err.Number <> 0 Then
                    'キャンセルのときは、セットしたファイル名のみ返すため
                    GFUNC_FD_Copy3 = 1
                    Exit Function
                End If

                If Dir(sBuff, vbNormal) <> "" Then Kill(sBuff)

                FileCopy(pSouceFilePath, sBuff)

                If Err.Number <> 0 Then
                    'ファイル保存失敗
                    Exit Function
                End If
            Case Else

                Select Case (CInt(STR_生徒明細入出区分))
                    Case 2 '入金
                        sBuff = sPath & "G" & STR_生徒明細学校コード & "03.dat"
                    Case 3 '臨時出金
                        sBuff = sPath & "G" & STR_生徒明細学校コード & "04.dat"
                End Select

                FileCopy(pSouceFilePath, sBuff)
                If Err.Number <> 0 Then
                    'ファイル保存失敗
                    Exit Function
                End If
        End Select

        GFUNC_FD_Copy3 = 0

    End Function

#End Region

    Private colNo, rowNo As Integer
    Private TextEditCtrl As DataGridViewTextBoxEditingControl

    Private Sub CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        colNo = e.ColumnIndex
        rowNo = e.RowIndex

        CType(sender, DataGridView).ImeMode = ImeMode.Disable
    End Sub
    Private Sub CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

        'スプレット内項目前ZERO詰め
        With CType(sender, DataGridView)
            Select Case colNo
                Case 17
                    Dim str_Value As String
                    str_Value = .Rows(e.RowIndex).Cells(e.ColumnIndex).Value
                    If Not str_Value Is Nothing Then
                        If IsNumeric(str_Value) Then
                            .Rows(e.RowIndex).Cells(e.ColumnIndex).Value = Format(CDec(str_Value), "#,##0")
                        End If
                    End If
            End Select
        End With

    End Sub
    Private Sub EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs)
        TextEditCtrl = CType(e.Control, DataGridViewTextBoxEditingControl)

        Select Case colNo
            Case 17
                AddHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                AddHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
        End Select
    End Sub
    Private Sub CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        Select Case colNo
            Case 17
                RemoveHandler TextEditCtrl.GotFocus, AddressOf CASTx01.GotFocusMoney
                RemoveHandler TextEditCtrl.KeyPress, AddressOf CASTx01.KeyPressMoney
        End Select

        Call CellLeave(sender, e)
    End Sub

    Private Sub CustomDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As DataGridViewRowPostPaintEventArgs) Handles DataGridView.RowPostPaint
        Dim dgv As DataGridView = CType(sender, DataGridView)

        ' 行ヘッダのセル領域を、行番号を描画する長方形とする
        ' （ただし右端に4ドットのすき間を空ける）
        Dim rect As New Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, dgv.Rows(e.RowIndex).Height)

        ' 上記の長方形内に行番号を縦方向中央＆右詰で描画する
        ' フォントや色は行ヘッダの既定値を使用する
        TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgv.RowHeadersDefaultCellStyle.Font, _
                              rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter _
                              Or TextFormatFlags.Right)

    End Sub

End Class
