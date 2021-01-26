Option Strict Off '旧VB互換用

Imports System.Windows.Forms
Imports System.IO
Imports CASTCommon

Public Class clsMain

    Function fn_CHECK_NUM_MSG(ByVal objOBJ As String, ByVal strJNAME As String, ByVal gstrTITLE As String) As Boolean
        '============================================================================
        'NAME           :fn_CHECK_NUM_MSG
        'Parameter      :objOBJ：チェック対象オブジェクト／strJNAME：オブジェクト名称
        '               :gstrTITLE：タイトル
        'Description    :数値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/05/28
        'Update         :
        '============================================================================
        fn_CHECK_NUM_MSG = False
        If Trim(objOBJ).Length = 0 Then
            MessageBox.Show(String.Format(MSG0285W, strJNAME), gstrTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            fn_CHECK_NUM_MSG = False
            Exit Function
        End If
        Dim i As Integer
        For i = 0 To objOBJ.Length - 1       '小数点/符号ﾁｪｯｸ
            If Char.IsDigit(objOBJ.Chars(i)) = False Then
                MessageBox.Show(String.Format(MSG0344W, strJNAME), gstrTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                fn_CHECK_NUM_MSG = False
                Exit Function
            End If
        Next i
        fn_CHECK_NUM_MSG = True
    End Function
    Function fn_CHECK_NUM(ByVal objOBJ As String) As Boolean
        '============================================================================
        'NAME           :fn_CHECK_NUM
        'Parameter      :objOBJ：チェック対象オブジェクト
        'Description    :数値チェック
        'Return         :True=OK,False=NG
        'Create         :2004/05/28
        'Update         :
        '============================================================================
        fn_CHECK_NUM = False
        Dim i As Integer
        'For i = 0 To Trim(objOBJ).Length - 1       '小数点/符号ﾁｪｯｸ
        For i = 0 To objOBJ.Length - 1       '小数点/符号ﾁｪｯｸ
            If objOBJ.Chars(i) = " " Then
                fn_CHECK_NUM = False
                Exit Function
            ElseIf Char.IsDigit(objOBJ.Chars(i)) = False Then
                fn_CHECK_NUM = False
                Exit Function
            End If
        Next i
        fn_CHECK_NUM = True
    End Function
    Function fn_Select_TENMAST(ByVal KIN_NO As String, ByVal SIT_NO As String, ByRef KIN_NNAME As String, ByRef SIT_NNAME As String, ByRef KIN_KNAME As String, ByRef SIT_KNAME As String) As Boolean
        '=====================================================================================
        'NAME           :fn_Select_TENMAST
        'Parameter      :KIN_NO：金融機関コード／SIT_NO：支店コード／KIN_NNAME:金融機関漢字名
        '               :SIT_NNAME:支店漢字名／KIN_KNAME：金融機関カナ名／SIT_KNAME：支店カナ名
        'Description    :金融機関マスタ検索
        'Return         :True=OK(検索ヒット),False=NG（検索失敗）
        'Create         :2004/05/28
        'Update         :
        '=====================================================================================
        fn_Select_TENMAST = False
start:
        Try
            '***** 2009/10/02 kakiwaki *******
            '   gstrSSQL = "SELECT * FROM TENMAST WHERE KIN_NO_N = '" & Trim(KIN_NO) & "' AND SIT_NO_N = '" & Trim(SIT_NO) & "' AND EDA_N = '01'"
            gstrSSQL = "SELECT * FROM TENMAST"
            gstrSSQL = gstrSSQL & " WHERE KIN_NO_N = '" & Trim(KIN_NO) & "'"
            gstrSSQL = gstrSSQL & " AND SIT_NO_N = '" & Trim(SIT_NO) & "'"
            '**************** 2009/10/01 *****
            gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
            gdbcCONNECT.Open()

            gdbCOMMAND = New OracleClient.OracleCommand
            gdbCOMMAND.CommandText = gstrSSQL
            gdbCOMMAND.Connection = gdbcCONNECT

            'gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ
            gdbrREADER = gdbCOMMAND.ExecuteReader


            KIN_NNAME = ""
            SIT_NNAME = ""
            KIN_KNAME = ""
            SIT_KNAME = ""

            '読込のみ
            If gdbrREADER.Read = False Then
                fn_Select_TENMAST = False
                gdbcCONNECT.Close()
                Exit Function
            Else
                KIN_NNAME = gdbrREADER.Item("KIN_NNAME_N")
                SIT_NNAME = gdbrREADER.Item("SIT_NNAME_N")
                KIN_KNAME = gdbrREADER.Item("KIN_KNAME_N")
                SIT_KNAME = gdbrREADER.Item("SIT_KNAME_N")
                fn_Select_TENMAST = True
            End If
            gdbcCONNECT.Close()
        Catch ex As Exception
            If Err.Number = 5 Then
                '-----------------------------------------
                '１秒停止
                '-----------------------------------------
                Dim Start As Double
                Start = CDbl(System.DateTime.Now.ToString("yyyyMMddHHmmss"))           ' 中断の開始時刻を設定します。
                Do While CDbl(System.DateTime.Now.ToString("yyyyMMddHHmmss")) < Start + 1
                    Application.DoEvents()
                Loop
                Err.Clear()
                GoTo start
            End If
            If Err.Number <> 0 And Err.Number <> 5 Then
                Exit Function
            End If

        End Try

    End Function
    Function fn_change_kana_SPACE(ByVal strTXT As String, ByRef strRETURN As String) As Boolean
        '============================================================================
        'NAME           :fn_change_kana_SPACE
        'Parameter      :strTXT：チェック対象文字列／strRETURN：戻り値
        'Description    :規定カナ文字チェック  規定外文字が入っていた場合はメッセージを表示
        'Return         :True=OK,False=NG
        'Create         :2004/06/02
        'Update         :
        '============================================================================
        fn_change_kana_SPACE = False
        Dim i As Integer
        Dim sbRETURN As New System.Text.StringBuilder("", strTXT.Length)

        For i = 0 To strTXT.Length - 1
            Select Case strTXT.Chars(i)
                Case "ｧ"
                    sbRETURN.Append("ｱ")
                Case "ｨ"
                    sbRETURN.Append("ｲ")
                Case "ｩ"
                    sbRETURN.Append("ｳ")
                Case "ｪ"
                    sbRETURN.Append("ｴ")
                Case "ｫ"
                    sbRETURN.Append("ｵ")
                Case "ｬ"
                    sbRETURN.Append("ﾔ")
                Case "ｭ"
                    sbRETURN.Append("ﾕ")
                Case "ｮ"
                    sbRETURN.Append("ﾖ")
                Case "ｯ"
                    sbRETURN.Append("ﾂ")
                Case "ｰ"
                    sbRETURN.Append("-")
                Case "A" To "Z", "ｱ" To "ﾝ", "0" To "9"
                    sbRETURN.Append(strTXT.Chars(i))
                Case "ﾞ", "ﾟ"
                    sbRETURN.Append(strTXT.Chars(i))
                    '*&$は規定外文字
                Case "\", ",", ".", "｢", "｣", "-", "/", "(", ")"
                    sbRETURN.Append(strTXT.Chars(i))
                Case " "
                    sbRETURN.Append(strTXT.Chars(i))
                Case Else
                    sbRETURN.Append(" ")
            End Select
        Next i
        strRETURN = sbRETURN.ToString
        fn_change_kana_SPACE = True

    End Function
    Function fn_INSERT_JOBMAST(ByVal strJOBID As String, ByVal strUSERID As String, ByVal strPARAMETA As String) As Boolean
        '=====================================================================================
        'NAME           :fn_INSERT_JOBMAST
        'Parameter      :strJOBID：起動するジョブＩＤ／strUSERID：ログインユーザ
        '　　　　　　　　:strPARAMETA：パラメータ
        'Description    :ジョブマスタにジョブを登録する
        'Return         :True=OK(成功),False=NG（失敗）
        'Create         :2004/07/14
        'Update         :
        '=====================================================================================

        '*** Str Add 2015/12/01 SO)荒木 for ログ強化 ***
        Dim MainLog As BatchLOG = New BatchLOG("clsFUSION", "clsMain")

        Dim sw As System.Diagnostics.Stopwatch
        sw = MainLog.Write_Enter3("clsFUSION.fn_INSERT_JOBMAST")
        '*** End Add 2015/12/01 SO)荒木 for ログ強化 ***

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
        Dim OraMain As CASTCommon.MyOracle = Nothing
        Dim dblock As CASTCommon.CDBLock = New CASTCommon.CDBLock

        Dim LockWaitTime As Integer = 30
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 30
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

        fn_INSERT_JOBMAST = False
        Dim JOB(12) As String
        JOB(0) = " "
        JOB(1) = 1                                              'TUUBAN_J
        JOB(2) = System.DateTime.Today.ToString("yyyyMMdd")      'TOUROKU_DATE_J
        JOB(3) = System.DateTime.Now.ToString("HHmmss")          'TOUROKU_TIME_J
        JOB(4) = "00000000"                                     'STA_DATE_J
        JOB(5) = "000000"                                       'STA_TIME_J
        JOB(6) = "00000000"                                     'END_DATE_J
        JOB(7) = "000000"                                       'END_TIME_J
        JOB(8) = strJOBID                                       'JOBID_J
        JOB(9) = "0"                                            'STATUS_J
        JOB(10) = strUSERID                                     'USERID_J
        JOB(11) = strPARAMETA                                   'PARAMETA_J
        JOB(12) = " "                                           'ERRMSG_J

        gstrSSQL = "INSERT INTO JOBMAST"
        gstrSSQL &= "("
        gstrSSQL &= " TUUBAN_J      "
        gstrSSQL &= ",TOUROKU_DATE_J"
        gstrSSQL &= ",TOUROKU_TIME_J"
        gstrSSQL &= ",STA_DATE_J    "
        gstrSSQL &= ",STA_TIME_J    "
        gstrSSQL &= ",END_DATE_J    "
        gstrSSQL &= ",END_TIME_J    "
        gstrSSQL &= ",JOBID_J       "
        gstrSSQL &= ",STATUS_J      "
        gstrSSQL &= ",USERID_J      "
        gstrSSQL &= ",PARAMETA_J    "
        gstrSSQL &= ",ERRMSG_J      "
        gstrSSQL &= ")"
        gstrSSQL = gstrSSQL & " VALUES ('" & JOB(1) & "',TO_CHAR(SYSDATE,'YYYYMMDD'),'" & JOB(3) & "','"
        gstrSSQL = gstrSSQL & JOB(4) & "','" & JOB(5) & "','" & JOB(6) & "','" & JOB(7) & "','"
        gstrSSQL = gstrSSQL & JOB(8) & "','" & JOB(9) & "','" & JOB(10) & "','" & JOB(11) & "','"
        gstrSSQL = gstrSSQL & JOB(12) & "')"

        '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
'        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
'        gdbcCONNECT.Open()
'
'        gdbCOMMAND = New OracleClient.OracleCommand
'        gdbCOMMAND.CommandText = gstrSSQL
'        gdbCOMMAND.Connection = gdbcCONNECT
'        gdbTRANS = gdbcCONNECT.BeginTransaction
'        gdbCOMMAND.Transaction = gdbTRANS
'
'        Try
'            gdbCOMMAND.ExecuteNonQuery()
'            gdbTRANS.Commit()
'        Catch ex As Exception
'            gdbTRANS.Rollback()
'            gdbcCONNECT.Close()
'            fn_INSERT_JOBMAST = False
'            'MessageBox.Show("ジョブマスタの登録に失敗しました：" & ex.Message, "ジョブマスタ登録", MessageBoxButtons.OK, MessageBoxIcon.Warning)
'            Exit Function
'        End Try
'        gdbcCONNECT.Close()

        Try
            OraMain = New CASTCommon.MyOracle

            ' ジョブ登録ロック
            If dblock.InsertJOBMAST_Lock(OraMain, LockWaitTime) = False Then
                MainLog.Write_Err("ジョブマスタ登録", "失敗", "タイムアウト")

                ' ロールバック
                OraMain.Rollback()
                OraMain.Close()

                fn_INSERT_JOBMAST = False
                Exit Function
            End If

            OraMain.ExecuteNonQuery(gstrSSQL)

            ' ジョブ登録アンロック
            dblock.InsertJOBMAST_UnLock(OraMain)

            OraMain.Commit()
            OraMain.Close()

            MainLog.Write_Exit3(sw, "clsFUSION.fn_INSERT_JOBMAST")

        Catch ex As Exception
             MainLog.Write_Err("ジョブマスタ登録", "失敗", ex)

            ' ジョブ登録アンロック
            dblock.InsertJOBMAST_UnLock(OraMain)

            If Not OraMain Is Nothing Then
                ' ロールバック
                OraMain.Rollback()
                OraMain.Close()
            End If

            fn_INSERT_JOBMAST = False
            Exit Function
        End Try
        '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

        fn_INSERT_JOBMAST = True
        Exit Function

    End Function
    Function fn_DEN_CPYTO_DISK(ByVal strTORI_CODE As String, ByVal strIN_FILE_NAME As String, ByVal strOUT_FILE_NAME As String, ByVal intREC_LENGTH As Integer, ByVal strCODE_KBN As String, ByVal strP_FILE As String, ByVal msgTitle As String) As Integer
        '=====================================================================================
        'NAME           :fn_DEN_CPYTO_DISK
        'Parameter      :strTORI_CODE：取引先コード／strIN_FILE_NAME：入力ファイル名／
        '               :strOUT_FILE_NAME：出力ファイル／intREC_LENGTH：レコード長／
        '               :strCODE_KBN：コード区分／strP_FILE：FTRAN+定義ファイル ／ msgTitle:メッセージタイトル
        'Description    :伝送ファイルをコピーする
        'Return         :0=成功、50=ファイルなし、100=コード変換失敗、200=コード区分異常（JIS改行あり）、
        '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
        'Create         :2004/07/21
        'Update         :
        '=====================================================================================
        fn_DEN_CPYTO_DISK = 100
        Dim strDIR As String
        strDIR = CurDir()

        '2013/10/24 saitou 標準修正 UPD -------------------------------------------------->>>>
        'INIファイルの取得方法改修
        gstrFTR_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTR")
        If gstrFTR_OPENDIR.Equals("err") = True OrElse gstrFTR_OPENDIR = String.Empty Then
            Exit Function
        End If

        gstrFTRANP_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
        If gstrFTRANP_OPENDIR.Equals("err") = True OrElse gstrFTRANP_OPENDIR = String.Empty Then
            Exit Function
        End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTR"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTR_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTRANP"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTRANP_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If
        '2013/10/24 saitou 標準修正 UPD --------------------------------------------------<<<<

        '-----------------------------------------------------------
        'ファイルの存在チェック
        '-----------------------------------------------------------
        If Dir(strIN_FILE_NAME) = "" Then
            MessageBox.Show(String.Format(MSG0274W, strIN_FILE_NAME), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            fn_DEN_CPYTO_DISK = 50
            Exit Function
        End If

        ' まずローカルへコピーする
        Dim sDestFileName As String = ""
        For nCounter As Integer = 1 To 100
            sDestFileName = Path.GetDirectoryName(strOUT_FILE_NAME) & "\" & Path.GetFileName(strIN_FILE_NAME)
            sDestFileName &= "." & Now.ToString("yyyyMMddHHmmssfffffff")
            Try
                File.Copy(strIN_FILE_NAME, sDestFileName, False)
                Exit For
            Catch ex As FileNotFoundException
                Return 400                      '出力ファイル作成失敗
            Catch ex As IOException
                ' ファイルがコピーできるまで繰り返す
            Catch ex As Exception
                Return 400                      '出力ファイル作成失敗
            End Try
        Next nCounter

        '-----------------------------------------------------------
        'コード変換処理
        'コード変換区分が 2 の場合はIBM形式 EBCDIC コードとみなし
        'JIS8コードへのコード変換を行う
        '-----------------------------------------------------------
        Select Case strCODE_KBN
            'Case "1", "3"     'EBCDIC
            Case "4"            'EBCDIC コード区分変更 2009/09/30 kakiwaki
                If Dir(strOUT_FILE_NAME) <> "" Then
                    Kill(strOUT_FILE_NAME)
                End If
                Dim strCMD As String
                Dim strTEIGI_FIEL As String

                ChDir(gstrFTRANP_OPENDIR)
                strTEIGI_FIEL = gstrFTR_OPENDIR & strP_FILE

                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                'strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & sDestFileName & " " & strOUT_FILE_NAME & " ++" & strTEIGI_FIEL
                strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & """" & sDestFileName & """" & " " & """" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                'strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & strIN_FILE_NAME & " " & strOUT_FILE_NAME & " ++" & strTEIGI_FIEL

                'intIDPROCESS = Shell(strCMD, , True, 100000)
                ''intIDPROCESS = Shell(strCMD, , True)

                ''lngProcess = OpenProcess(lngPROCESS_QUERY_INFOMATION, 1, intIDPROCESS)
                ''Do
                ''    lngRET = GetExitCodeProcess(lngProcess, lngEXITCODE)
                ''    System.Windows.Forms.Application.DoEvents()
                ''Loop While (lngEXITCODE = lngSTILL_ACTIVE)

                ''lngRET = CloseHandle(lngProcess)
                Dim ProcFT As New Process
                Dim ProcInfo As New ProcessStartInfo
                ProcInfo.FileName = gstrFTRANP_OPENDIR & "FP.EXE"
                ProcInfo.Arguments = strCMD.Substring(3)
                ProcInfo.WorkingDirectory = gstrFTRANP_OPENDIR
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                    fn_DEN_CPYTO_DISK = 0
                Else
                    fn_DEN_CPYTO_DISK = 100         'コード変換失敗
                    Exit Function
                End If
            Case Else        'JIS,JIS改
                If Dir(strOUT_FILE_NAME) <> "" Then
                    Kill(strOUT_FILE_NAME)
                End If

                '--------------------------------------------
                'ファイルを読み込み改行がついているか判定
                '--------------------------------------------
                Dim strFILE_DATA As String
                Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
                intFILE_NO_1 = FreeFile()
                FileOpen(intFILE_NO_1, strIN_FILE_NAME, OpenMode.Binary, OpenAccess.Read, OpenShare.Default, 1)
                strFILE_DATA = InputString(intFILE_NO_1, intREC_LENGTH + 1)
                '2013/10/24 saitou 標準修正 UPD -------------------------------------------------->>>>
                'ファイルに全角文字が入っていると改行コードの判定に失敗するため処理ロジック変更
                Dim btFileData As Byte() = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes(strFILE_DATA)
                Dim strCheck As String = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(btFileData, intREC_LENGTH, 1)
                If strCheck.Equals(Chr(13)) = True Then
                    Select Case strCODE_KBN
                        Case "0", "4"   'ＪＩＳ(改行なし)、ＥＢＣＤＩＣ
                            fn_DEN_CPYTO_DISK = 200     'コード区分異常（JIS改行あり）
                            '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ START
                            'ファイルを閉じていないため処理追加
                            FileClose(intFILE_NO_1)
                            '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ END
                            Exit Function
                    End Select
                Else
                    If strCODE_KBN <> "0" Then
                        fn_DEN_CPYTO_DISK = 300       'コード区分異常（JIS改行なし）
                        '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ START
                        'ファイルを閉じていないため処理追加
                        FileClose(intFILE_NO_1)
                        '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ END
                        Exit Function
                    End If
                End If

                'If strFILE_DATA.Substring(intREC_LENGTH, 1) = Chr(13) Then  '改行コード

                '    '2009/09/18.Sakon　コード区分変更に伴う修正 +++++++++++++++++++++++++
                '    Select Case strCODE_KBN
                '        Case "0", "4"   'ＪＩＳ(改行なし)、ＥＢＣＤＩＣ
                '            fn_DEN_CPYTO_DISK = 200     'コード区分異常（JIS改行あり）
                '            Exit Function
                '    End Select
                '    'If strCODE_KBN <> 2 Then
                '    '    fn_DEN_CPYTO_DISK = 200       'コード区分異常（JIS改行あり）
                '    '    Exit Function
                '    'End If
                '    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                'Else
                '    If strCODE_KBN <> "0" Then
                '        fn_DEN_CPYTO_DISK = 300       'コード区分異常（JIS改行なし）
                '        Exit Function
                '    End If
                'End If
                '2013/10/24 saitou 標準修正 UPD --------------------------------------------------<<<<

                FileClose(intFILE_NO_1)

                FileOpen(intFILE_NO_1, strIN_FILE_NAME, OpenMode.Input, , , intREC_LENGTH)   '入力ファイル
                intFILE_NO_2 = FreeFile()

                '2009/10/10.Sakon　119改,118改対応 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                Select Case strCODE_KBN
                    Case "2"
                        FileOpen(intFILE_NO_2, strOUT_FILE_NAME, OpenMode.Output, , , 120)              '出力ファイル
                        Do Until EOF(intFILE_NO_1)
                            strFILE_DATA = LineInput(intFILE_NO_1)
                            Print(intFILE_NO_2, strFILE_DATA & Space(1))
                        Loop
                    Case "3"
                        FileOpen(intFILE_NO_2, strOUT_FILE_NAME, OpenMode.Output, , , 120)              '出力ファイル
                        Do Until EOF(intFILE_NO_1)
                            strFILE_DATA = LineInput(intFILE_NO_1)
                            Print(intFILE_NO_2, strFILE_DATA & Space(2))
                        Loop
                    Case Else
                        FileOpen(intFILE_NO_2, strOUT_FILE_NAME, OpenMode.Output, , , intREC_LENGTH)    '出力ファイル
                        Do Until EOF(intFILE_NO_1)
                            strFILE_DATA = LineInput(intFILE_NO_1)
                            Print(intFILE_NO_2, strFILE_DATA)
                        Loop
                End Select
                'FileOpen(intFILE_NO_2, strOUT_FILE_NAME, OpenMode.Output, , , intREC_LENGTH)   '出力ファイル
                'Do Until EOF(intFILE_NO_1)
                '    strFILE_DATA = LineInput(intFILE_NO_1)
                '    Print(intFILE_NO_2, strFILE_DATA)
                'Loop
                '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                FileClose(intFILE_NO_1)
                FileClose(intFILE_NO_2)
                If Err.Number <> 0 Then
                    fn_DEN_CPYTO_DISK = 400    '出力ファイル作成失敗
                End If
        End Select
        ChDir(strDIR)
        fn_DEN_CPYTO_DISK = 0

    End Function
    Function fn_FD_CPYTO_DISK(ByVal astrTORI_CODE As String, ByRef astrIN_FILE_NAME As String, ByVal astrOUT_FILE_NAME As String, ByVal aintREC_LENGTH As Integer, ByVal astrCODE_KBN As String, ByVal astrP_FILE As String, ByVal msgTitle As String) As Integer
        '=====================================================================================
        'NAME           :fn_FD_CPYTO_DISK
        'Parameter      :astrTORI_CODE：取引先コード／astrIN_FILE_NAME：入力ファイル名／
        '               :astrOUT_FILE_NAME：出力ファイル／aintREC_LENGTH：レコード長／
        '               :astrCODE_KBN：コード区分／astrP_FILE：FTRAN+定義ファイル　／ msgTitle:メッセージタイトル
        'Description    :伝送ファイルをコピーする
        'Return         :0=成功、100=ファイル読み込み失敗、200=コード区分異常（JIS改行あり）、
        '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
        '               :50=ユーザキャンセル
        'Create         :2004/07/21
        'Update         :
        '=====================================================================================
        fn_FD_CPYTO_DISK = 100

        Dim strDIR As String
        strDIR = CurDir()

        '2013/10/24 saitou 標準修正 UPD -------------------------------------------------->>>>
        'INIファイルの取得方法改修
        gstrFTR_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTR")
        If gstrFTR_OPENDIR.Equals("err") = True OrElse gstrFTR_OPENDIR = String.Empty Then
            Exit Function
        End If

        gstrFTRAN2000_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTRAN2000")
        If gstrFTRAN2000_OPENDIR.Equals("err") = True OrElse gstrFTRAN2000_OPENDIR = String.Empty Then
            Exit Function
        End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTR"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTR_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTRAN2000"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTRAN2000_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If
        '2013/10/24 saitou 標準修正 UPD --------------------------------------------------<<<<

        '-----------------------------------------------------------
        'コード変換処理
        'コード変換区分が 2 の場合はIBM形式 EBCDIC コードとみなし
        'JIS8コードへのコード変換を行う
        '-----------------------------------------------------------
        Select Case astrCODE_KBN
            'Case "1", "3"     'EBCDIC
            Case "4"            'EBCDIC コード区分変更 2009/09/30 kakiwaki

                '       '2006/08/18　メッセージボックス表示の際、キャンセルボタンを押下時に動作を終了する
                '       'MessageBox.Show("ＦＤ(ＩＢＭ形式)をセットしてください。" & vbCrLf & " 取引先コード：" & astrTORI_CODE.Substring(0, 7) & "-" & astrTORI_CODE.Substring(7, 2), "ＦＤ読み込み", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                '   If MessageBox.Show("ＦＤ(ＩＢＭ形式)をセットしてください。" & vbCrLf & " 取引先コード：" & astrTORI_CODE.Substring(0, 7) & "-" & astrTORI_CODE.Substring(7, 2), "ＦＤ読み込み", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                '       Exit Function
                '   End If
                ' 取引先コード表示桁変更 2009/10/04 kakiwaki
                If MessageBox.Show(String.Format(MSG0073I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    Exit Function
                End If
                '
                If Dir(astrOUT_FILE_NAME) <> "" Then
                    Kill(astrOUT_FILE_NAME)
                End If
                Dim strCMD As String
                Dim strTEIGI_FIEL As String

                ChDir(gstrFTRAN2000_OPENDIR)
                strTEIGI_FIEL = gstrFTR_OPENDIR & astrP_FILE

                'ＩＢＭフロッピーディスクがセットされているか判定
FD_EXIST_IBM:
                '2010/02/02 ================
                'strCMD = "FT /wc/ ILIST A:"
                strCMD = "FT /nwd/ ILIST A:"
                '===========================

                'intIDPROCESS = Shell(strCMD, , True, 100000)
                ''intIDPROCESS = Shell(strCMD, , True)

                ''lngProcess = OpenProcess(lngPROCESS_QUERY_INFOMATION, 1, intIDPROCESS)
                ''Do
                ''    lngRET = GetExitCodeProcess(lngProcess, lngEXITCODE)
                ''    System.Windows.Forms.Application.DoEvents()
                ''Loop While (lngEXITCODE = lngSTILL_ACTIVE)

                ''lngRET = CloseHandle(lngProcess)
                Dim ProcFT As New Process
                Dim ProcInfo As New ProcessStartInfo(gstrFTRAN2000_OPENDIR & "FT", strCMD.Substring(3))
                ProcInfo.CreateNoWindow = True
                ProcInfo.WorkingDirectory = gstrFTRAN2000_OPENDIR
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                Else
                    ' 取引先コード表示桁変更 2009/10/04 kakiwaki
                    '   MessageBox.Show("ＦＤ(ＩＢＭ形式)をセットしてください。" & vbCrLf & " 取引先コード：" & astrTORI_CODE.Substring(0, 7) & "-" & astrTORI_CODE.Substring(7, 2), "ＦＤ読み込み", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                    If MessageBox.Show(String.Format(MSG0073I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                        ChDir(strDIR)
                        fn_FD_CPYTO_DISK = 100         'コード変換失敗
                        Exit Function
                    End If
                    GoTo FD_EXIST_IBM
                End If

                ChDir(gstrFTRAN2000_OPENDIR)
                '2010/02/02 ==================
                'strCMD = "FT /WC/ GETRAND " & "A:" & astrIN_FILE_NAME & Space(1) & astrOUT_FILE_NAME & " ++" & strTEIGI_FIEL
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                'strCMD = "FT /nwd/ GETRAND " & "A:" & astrIN_FILE_NAME & Space(1) & astrOUT_FILE_NAME & " ++" & strTEIGI_FIEL
                strCMD = "FT /nwd/ GETRAND " & """A:" & astrIN_FILE_NAME & """" & Space(1) & """" & astrOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                '=============================

                'intIDPROCESS = Shell(strCMD, , True, 500000)
                ''intIDPROCESS = Shell(strCMD, , True)

                ''lngProcess = OpenProcess(lngPROCESS_QUERY_INFOMATION, 1, intIDPROCESS)
                ''Do
                ''    lngRET = GetExitCodeProcess(lngProcess, lngEXITCODE)
                ''    System.Windows.Forms.Application.DoEvents()
                ''Loop While (lngEXITCODE = lngSTILL_ACTIVE)

                ''lngRET = CloseHandle(lngProcess)
                ProcFT = New Process
                ProcInfo = New ProcessStartInfo(gstrFTRAN2000_OPENDIR & "FT", strCMD.Substring(3))
                ProcInfo.CreateNoWindow = True
                ProcInfo.WorkingDirectory = gstrFTRAN2000_OPENDIR
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                    fn_FD_CPYTO_DISK = 0
                Else
                    ChDir(strDIR)
                    fn_FD_CPYTO_DISK = 100         'コード変換失敗
                    Exit Function
                End If
            Case Else        'JIS,JIS改

                '       '2006/08/18　メッセージボックス表示の際、キャンセルボタンを押下時に動作を終了する
                '       'MessageBox.Show("ＦＤ(ＤＯＳ形式)をセットしてください。" & vbCrLf & " 取引先コード：" & astrTORI_CODE.Substring(0, 7) & "-" & astrTORI_CODE.Substring(7, 2), "ＦＤ読み込み", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                '   If MessageBox.Show("ＦＤ(ＤＯＳ形式)をセットしてください。" & vbCrLf & " 取引先コード：" & astrTORI_CODE.Substring(0, 7) & "-" & astrTORI_CODE.Substring(7, 2), "ＦＤ読み込み", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                '       Exit Function
                '   End If
                ' 取引先コード表示桁変更 2009/10/04 kakiwaki
                If MessageBox.Show(String.Format(MSG0064I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    '2018/03/06 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ START
                    'キャンセル時のリターンコードを返していないので追加する
                    fn_FD_CPYTO_DISK = 50
                    '2018/03/06 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ END
                    Exit Function
                End If

                Dim strFOLDER As String
                Dim strFILE As String
                If Dir(astrOUT_FILE_NAME) <> "" Then
                    Kill(astrOUT_FILE_NAME)
                End If
                strFOLDER = "A:\"
                strFILE = strFOLDER & astrIN_FILE_NAME.Trim
                '2005/04/11
                If astrIN_FILE_NAME = "" Then
                    MessageBox.Show(MSG0346W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    fn_FD_CPYTO_DISK = 100
                    Exit Function
                End If

FD_EXIST:
                Try
                    Dir(strFOLDER)
                    '-----------------------------
                    'ＤＯＳ形式か確認
                    '-----------------------------
                    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'FileOpen(1, Dir(strFOLDER), OpenMode.Binary)
                    'FileClose(1)
                    If Directory.GetFiles(strFOLDER).Length = 0 Then
                        Throw New Exception
                    End If
                    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                Catch EX As Exception      'FDがセットされていなかった場合
                    ' 取引先コード表示桁変更 2009/10/04 kakiwaki 
                    '   If MessageBox.Show("ＦＤ(ＤＯＳ形式)をセットしてください。" & vbCrLf & _
                    '       "取引先コード：" & astrTORI_CODE.Substring(0, 7) & "-" & astrTORI_CODE.Substring(7, 2), _
                    If MessageBox.Show(String.Format(MSG0064I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) _
                       = DialogResult.OK Then
                        GoTo FD_EXIST
                    Else
                        ChDir(strDIR)
                        '   fn_FD_CPYTO_DISK = 100         'ファイル読み込み失敗 2009/09/30 kakiwaki
                        fn_FD_CPYTO_DISK = 200         'ファイル読み込み失敗
                        Exit Function
                    End If
                End Try
                If Dir(strFILE) = "" Then 'FD内にファイルが存在しなかったら
                    Dim OPENFILEDIALOG1 As New OpenFileDialog
                    OPENFILEDIALOG1.InitialDirectory = strFOLDER
                    OPENFILEDIALOG1.Multiselect = False
                    OPENFILEDIALOG1.CheckFileExists = True
                    OPENFILEDIALOG1.FileName = strFILE
                    OPENFILEDIALOG1.AddExtension = True
                    OPENFILEDIALOG1.CheckFileExists = True
                    Dim dlgRESULT As DialogResult
                    dlgRESULT = OPENFILEDIALOG1.ShowDialog()
                    If dlgRESULT = DialogResult.Cancel Then    'キャンセルボタンが押されたら
                        ChDir(strDIR)
                        '   fn_FD_CPYTO_DISK = 100         'ファイル読み込み失敗 2009/09/30 kakiwaki
                        fn_FD_CPYTO_DISK = 200         'ファイル読み込み失敗
                        Exit Function
                    End If
                    strFILE = OPENFILEDIALOG1.FileName
                    astrIN_FILE_NAME = ""
                    Dim j As Integer
                    For j = 1 To strFILE.Length
                        If strFILE.Chars(strFILE.Length - j) = "\" Then
                            Exit For
                        End If
                        astrIN_FILE_NAME = strFILE.Chars(strFILE.Length - j) & astrIN_FILE_NAME
                    Next
                End If

                '--------------------------------------------
                'ファイルを読み込み改行がついているか判定
                '--------------------------------------------
                Dim strFILE_DATA As String
                Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
                intFILE_NO_1 = FreeFile()
                FileOpen(intFILE_NO_1, strFILE, OpenMode.Binary, OpenAccess.Read, OpenShare.Default, 1)
                strFILE_DATA = InputString(intFILE_NO_1, aintREC_LENGTH + 1)
                '2013/10/24 saitou 標準修正 UPD -------------------------------------------------->>>>
                'ファイルに全角文字が入っていると改行コードの判定に失敗するため処理ロジック変更
                Dim btFileData As Byte() = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes(strFILE_DATA)
                Dim strCheck As String = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(btFileData, aintREC_LENGTH, 1)
                If strCheck.Equals(Chr(13)) = True Then
                    Select Case astrCODE_KBN
                        Case "0", "4" 'ＪＩＳ(改行なし)、ＥＢＣＤＩＣ
                            ChDir(strDIR)
                            fn_FD_CPYTO_DISK = 200     'コード区分異常（JIS改行あり）
                            '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ START
                            'ファイルを閉じていないため処理追加
                            FileClose(intFILE_NO_1)
                            '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ END
                            Exit Function
                    End Select
                Else
                    If astrCODE_KBN <> "0" Then
                        ChDir(strDIR)
                        fn_FD_CPYTO_DISK = 300       'コード区分異常（JIS改行なし）
                        '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ START
                        'ファイルを閉じていないため処理追加
                        FileClose(intFILE_NO_1)
                        '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ END
                        Exit Function
                    End If
                End If

                'If strFILE_DATA.Substring(aintREC_LENGTH, 1) = Chr(13) Then  '改行コード

                '    '2009/09/18.Sakon　コード区分変更に伴う修正 +++++++++++++++++++++++++
                '    Select Case astrCODE_KBN
                '        Case "0", "4" 'ＪＩＳ(改行なし)、ＥＢＣＤＩＣ
                '            ChDir(strDIR)
                '            fn_FD_CPYTO_DISK = 200     'コード区分異常（JIS改行あり）
                '            Exit Function
                '    End Select
                '    'If astrCODE_KBN <> 2 Then
                '    '    ChDir(strDIR)
                '    '    fn_FD_CPYTO_DISK = 200       'コード区分異常（JIS改行あり）
                '    '    Exit Function
                '    'End If
                '    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                'Else
                '    If astrCODE_KBN <> "0" Then
                '        ChDir(strDIR)
                '        fn_FD_CPYTO_DISK = 300       'コード区分異常（JIS改行なし）
                '        Exit Function
                '    End If
                'End If
                '2013/10/24 saitou 標準修正 UPD --------------------------------------------------<<<<

                FileClose(intFILE_NO_1)
                FileOpen(intFILE_NO_1, strFILE, OpenMode.Input, , , aintREC_LENGTH)   '入力ファイル
                intFILE_NO_2 = FreeFile()
                FileOpen(intFILE_NO_2, astrOUT_FILE_NAME, OpenMode.Output, , , aintREC_LENGTH)   '出力ファイル
                Do Until EOF(intFILE_NO_1)
                    strFILE_DATA = LineInput(intFILE_NO_1)
                    Select Case astrCODE_KBN
                        Case "2"    'JIS119改行
                            strFILE_DATA &= Space(1)
                        Case "3"    'JIS118改行
                            strFILE_DATA &= Space(2)
                    End Select
                    Print(intFILE_NO_2, strFILE_DATA)
                Loop
                FileClose(intFILE_NO_1)
                FileClose(intFILE_NO_2)
                If Err.Number <> 0 Then
                    fn_FD_CPYTO_DISK = 400    '出力ファイル作成失敗
                End If
        End Select
        ChDir(strDIR)
        fn_FD_CPYTO_DISK = 0

    End Function

    '**********20120704 mubuchi DVD対応 START****************************************************************************************************************************************************************************************************************************
    Function fn_DVD_CPYTO_DISK(ByVal astrTORI_CODE As String, ByRef astrIN_FILE_NAME As String, ByVal astrOUT_FILE_NAME As String, _
                               ByVal aintREC_LENGTH As Integer, ByVal astrCODE_KBN As String, ByVal astrP_FILE As String, ByVal msgTitle As String, _
                               ByVal astrBAITAI_CODE As String) As Integer
        '=====================================================================================
        'NAME           :fn_DVD_CPYTO_DISK
        'Parameter      :astrTORI_CODE：取引先コード／astrIN_FILE_NAME：入力ファイル名／
        '               :astrOUT_FILE_NAME：出力ファイル／aintREC_LENGTH：レコード長／
        '               :astrCODE_KBN：コード区分／astrP_FILE：FTRAN+定義ファイル　／ msgTitle:メッセージタイトル
        'Description    :DVDからファイルをコピーする
        'Return         :0=成功、100=ファイル読み込み失敗、200=コード区分異常（JIS改行あり）、
        '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
        'Create         :2004/07/21
        'Update         :2013/12/24 saitou 標準版 外部媒体対応
        '=====================================================================================
        fn_DVD_CPYTO_DISK = 100

        Dim strDIR As String
        strDIR = CurDir()

        '2013/10/24 saitou 標準修正 UPD -------------------------------------------------->>>>
        'INIファイルの取得方法改修
        gstrFTR_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTR")
        If gstrFTR_OPENDIR.Equals("err") = True OrElse gstrFTR_OPENDIR = String.Empty Then
            Exit Function
        End If

        gstrFTRAN2000_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTRAN2000")
        If gstrFTRAN2000_OPENDIR.Equals("err") = True OrElse gstrFTRAN2000_OPENDIR = String.Empty Then
            Exit Function
        End If

        '2013/12/24 saitou 標準版 外部媒体対応 DEL -------------------------------------------------->>>>
        'DVDは媒体1として扱うことにする
        'DVD_DRIVE = CASTCommon.GetFSKJIni("DVD", "DRIVE")
        'If DVD_DRIVE.Equals("err") = True OrElse DVD_DRIVE = String.Empty Then
        '    Exit Function
        'End If
        '2013/12/24 saitou 標準版 外部媒体対応 DEL --------------------------------------------------<<<<
        '2013/12/24 saitou 標準版 外部媒体対応 ADD -------------------------------------------------->>>>
        ' 2016/01/27 タスク）綾部 CHG 【PG】UI_B-14-99(RSV2対応(USB)) -------------------- START
        'Dim BAITAI_1 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_1")
        'Dim BAITAI_2 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_2")
        'Dim BAITAI_3 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_3")
        'Dim BAITAI_4 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_4")
        'Dim BAITAI_5 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_5")
        'Dim strUseBaitai As String = String.Empty       '使う媒体
        'Dim strUseBaitaiName As String = String.Empty   '使う媒体名
        'Select Case astrBAITAI_CODE
        '    Case "11"   'DVD
        '        strUseBaitai = BAITAI_1
        '        strUseBaitaiName = "DVD-RAM"
        '    Case "12"
        '        strUseBaitai = BAITAI_2
        '        strUseBaitaiName = ""
        '    Case "13"
        '        strUseBaitai = BAITAI_3
        '        strUseBaitaiName = ""
        '    Case "14"
        '        strUseBaitai = BAITAI_4
        '        strUseBaitaiName = ""
        '    Case "15"
        '        strUseBaitai = BAITAI_5
        '        strUseBaitaiName = ""
        'End Select
        Dim strUseBaitai As String = String.Empty       '使う媒体
        Dim strUseBaitaiName As String = String.Empty   '使う媒体名
        Select Case astrBAITAI_CODE
            Case "11"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_1")
            Case "12"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_2")
            Case "13"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_3")
            Case "14"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_4")
            Case "15"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_5")
        End Select
        strUseBaitaiName = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"), astrBAITAI_CODE)
        ' 2016/01/27 タスク）綾部 CHG 【PG】UI_B-14-99(RSV2対応(USB)) -------------------- END
        '2013/12/24 saitou 標準版 外部媒体対応 ADD --------------------------------------------------<<<<

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTR"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTR_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTRAN2000"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTRAN2000_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "DVD"
        'gstrIKeyName = "DRIVE"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    DVD_DRIVE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If
        '2013/10/24 saitou 標準修正 UPD --------------------------------------------------<<<<

        '-----------------------------------------------------------
        'コード変換処理
        'コード変換区分が 2 の場合はIBM形式 EBCDIC コードとみなし
        'JIS8コードへのコード変換を行う
        '-----------------------------------------------------------
        Select Case astrCODE_KBN
            Case "4"            'EBCDIC 
                '2012/7/9　メッセージボックス表示の際、キャンセルボタンを押下時に動作を終了する
                '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                '媒体名を設定できるように変更
                If MessageBox.Show(String.Format(MSG0075I, strUseBaitaiName, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), _
                                   msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    Exit Function
                End If
                'If MessageBox.Show(String.Format(MSG0076I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                '    Exit Function
                'End If
                '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<

                Dim strFOLDER As String
                Dim strFILE As String
                If Dir(astrOUT_FILE_NAME) <> "" Then
                    Kill(astrOUT_FILE_NAME)
                End If
                '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                strFOLDER = strUseBaitai
                'strFOLDER = DVD_DRIVE
                '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                strFILE = strFOLDER & astrIN_FILE_NAME.Trim
                '2005/04/11
                If astrIN_FILE_NAME = "" Then
                    MessageBox.Show(MSG0346W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    fn_DVD_CPYTO_DISK = 100
                    Exit Function
                End If

                '2017/04/21 タスク）西野 ADD 標準版修正（進級戻し処理対応）------------------------------------ START
DVD_EBC_EXIST:
                Try
                    '-----------------------------
                    'ＤＯＳ形式か確認
                    '-----------------------------
                    If Directory.GetFiles(strFOLDER).Length = 0 Then
                        Throw New Exception
                    End If
                Catch EX As Exception      'FDがセットされていなかった場合
                    '媒体名を設定できるように変更
                    If MessageBox.Show(String.Format(MSG0076I, strUseBaitaiName, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), _
                                       msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.OK Then
                        GoTo DVD_EBC_EXIST
                    Else
                        ChDir(strDIR)
                        fn_DVD_CPYTO_DISK = 200         'ファイル読み込み失敗
                        Exit Function
                    End If
                End Try
                If Dir(strFILE) = "" Then 'FD内にファイルが存在しなかったら
                    Dim OPENFILEDIALOG1 As New OpenFileDialog
                    OPENFILEDIALOG1.InitialDirectory = strFOLDER
                    OPENFILEDIALOG1.Multiselect = False
                    OPENFILEDIALOG1.CheckFileExists = True
                    OPENFILEDIALOG1.FileName = strFILE
                    OPENFILEDIALOG1.AddExtension = True
                    OPENFILEDIALOG1.CheckFileExists = True
                    Dim dlgRESULT As DialogResult
                    dlgRESULT = OPENFILEDIALOG1.ShowDialog()
                    If dlgRESULT = DialogResult.Cancel Then    'キャンセルボタンが押されたら
                        ChDir(strDIR)
                        '   fn_FD_CPYTO_DISK = 100         'ファイル読み込み失敗 2009/09/30 kakiwaki
                        fn_DVD_CPYTO_DISK = 200         'ファイル読み込み失敗
                        Exit Function
                    End If
                    strFILE = OPENFILEDIALOG1.FileName
                    astrIN_FILE_NAME = Path.GetFileName(strFILE)
                End If
                '2017/04/21 タスク）西野 ADD 標準版修正（進級戻し処理対応）------------------------------------ END

                Dim strCMD As String
                Dim strTEIGI_FIEL As String

                strTEIGI_FIEL = gstrFTR_OPENDIR & astrP_FILE

                'FTRAN+のコマンドを使ってコード変換する
                '2013/12/24 saitou 標準版 外部媒体対応 MODIFY -------------------------------------------------->>>>
                'ドライブ固定やめる
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                'strCMD = "/nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & strFILE & " " & astrOUT_FILE_NAME & " ++" & strTEIGI_FIEL
                strCMD = "/nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & """" & strFILE & """" & " " & """" & astrOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                'strCMD = "/nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & "E:" & astrIN_FILE_NAME & " " & astrOUT_FILE_NAME & " ++" & strTEIGI_FIEL
                '2013/12/24 saitou 標準版 外部媒体対応 MODIFY --------------------------------------------------<<<<

                Dim ProcFT As Process
                Dim ProcInfo As New ProcessStartInfo
                ProcInfo.FileName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "FTRANP"), "FP.EXE")
                ProcInfo.Arguments = strCMD
                ProcInfo.WorkingDirectory = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                    fn_DVD_CPYTO_DISK = 0
                Else
                    ChDir(strDIR)
                    fn_DVD_CPYTO_DISK = 100         'コード変換失敗
                    Exit Function
                End If

            Case Else        'JIS,JIS改

                '2012/7/9　メッセージボックス表示の際、キャンセルボタンを押下時に動作を終了する
                '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                '媒体名を設定できるように変更
                If MessageBox.Show(String.Format(MSG0075I, strUseBaitaiName, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), _
                                   msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    Exit Function
                End If
                'If MessageBox.Show(String.Format(MSG0076I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                '    Exit Function
                'End If
                '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<

                Dim strFOLDER As String
                Dim strFILE As String
                If Dir(astrOUT_FILE_NAME) <> "" Then
                    Kill(astrOUT_FILE_NAME)
                End If
                '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                strFOLDER = strUseBaitai
                'strFOLDER = DVD_DRIVE
                '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                strFILE = strFOLDER & astrIN_FILE_NAME.Trim
                '2005/04/11
                If astrIN_FILE_NAME = "" Then
                    MessageBox.Show(MSG0346W, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    fn_DVD_CPYTO_DISK = 100
                    Exit Function
                End If

DVD_EXIST:
                Try
                    Dir(strFOLDER)
                    '-----------------------------
                    'ＤＯＳ形式か確認
                    '-----------------------------
                    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'FileOpen(1, Dir(strFOLDER), OpenMode.Binary)
                    'FileClose(1)
                    If Directory.GetFiles(strFOLDER).Length = 0 Then
                        Throw New Exception
                    End If
                    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                Catch EX As Exception      'FDがセットされていなかった場合
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                    '媒体名を設定できるように変更
                    If MessageBox.Show(String.Format(MSG0076I, strUseBaitaiName, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), _
                                       msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.OK Then
                        GoTo DVD_EXIST
                    Else
                        ChDir(strDIR)
                        fn_DVD_CPYTO_DISK = 200         'ファイル読み込み失敗
                        Exit Function
                    End If

                    'If MessageBox.Show(String.Format(MSG0076I, astrTORI_CODE.Substring(0, 10) & "-" & astrTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) _
                    '                       = DialogResult.OK Then
                    '    GoTo DVD_EXIST
                    'Else
                    '    ChDir(strDIR)
                    '    fn_DVD_CPYTO_DISK = 200         'ファイル読み込み失敗
                    '    Exit Function
                    'End If
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                End Try
                If Dir(strFILE) = "" Then 'FD内にファイルが存在しなかったら
                    Dim OPENFILEDIALOG1 As New OpenFileDialog
                    OPENFILEDIALOG1.InitialDirectory = strFOLDER
                    OPENFILEDIALOG1.Multiselect = False
                    OPENFILEDIALOG1.CheckFileExists = True
                    OPENFILEDIALOG1.FileName = strFILE
                    OPENFILEDIALOG1.AddExtension = True
                    OPENFILEDIALOG1.CheckFileExists = True
                    Dim dlgRESULT As DialogResult
                    dlgRESULT = OPENFILEDIALOG1.ShowDialog()
                    If dlgRESULT = DialogResult.Cancel Then    'キャンセルボタンが押されたら
                        ChDir(strDIR)
                        '   fn_FD_CPYTO_DISK = 100         'ファイル読み込み失敗 2009/09/30 kakiwaki
                        fn_DVD_CPYTO_DISK = 200         'ファイル読み込み失敗
                        Exit Function
                    End If
                    strFILE = OPENFILEDIALOG1.FileName
                    astrIN_FILE_NAME = ""
                    Dim j As Integer
                    For j = 1 To strFILE.Length
                        If strFILE.Chars(strFILE.Length - j) = "\" Then
                            Exit For
                        End If
                        astrIN_FILE_NAME = strFILE.Chars(strFILE.Length - j) & astrIN_FILE_NAME
                    Next
                End If

                '--------------------------------------------
                'ファイルを読み込み改行がついているか判定
                '--------------------------------------------
                Dim strFILE_DATA As String
                Dim intFILE_NO_1 As Integer, intFILE_NO_2 As Integer
                intFILE_NO_1 = FreeFile()
                FileOpen(intFILE_NO_1, strFILE, OpenMode.Binary, OpenAccess.Read, OpenShare.Default, 1)
                strFILE_DATA = InputString(intFILE_NO_1, aintREC_LENGTH + 1)
                '2013/10/24 saitou 標準修正 UPD -------------------------------------------------->>>>
                'ファイルに全角文字が入っていると改行コードの判定に失敗するため処理ロジック変更
                Dim btFileData As Byte() = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetBytes(strFILE_DATA)
                Dim strCheck As String = System.Text.Encoding.GetEncoding("SHIFT-JIS").GetString(btFileData, aintREC_LENGTH, 1)
                If strCheck.Equals(Chr(13)) = True Then
                    Select Case astrCODE_KBN
                        Case "0", "4" 'ＪＩＳ(改行なし)、ＥＢＣＤＩＣ
                            ChDir(strDIR)
                            fn_DVD_CPYTO_DISK = 200     'コード区分異常（JIS改行あり）
                            '2017/03/02 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ START
                            'ファイルを閉じていないため処理追加
                            FileClose(intFILE_NO_1)
                            '2017/03/02 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ END
                            Exit Function
                    End Select
                Else
                    If astrCODE_KBN <> "0" Then
                        ChDir(strDIR)
                        fn_DVD_CPYTO_DISK = 300       'コード区分異常（JIS改行なし）
                        '2017/03/02 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ START
                        'ファイルを閉じていないため処理追加
                        FileClose(intFILE_NO_1)
                        '2017/03/02 タスク）西野 ADD 標準版修正（潜在バグ修正）------------------------------------ END
                        Exit Function
                    End If
                End If

                'If strFILE_DATA.Substring(aintREC_LENGTH, 1) = Chr(13) Then  '改行コード

                '    '2009/09/18.Sakon　コード区分変更に伴う修正 +++++++++++++++++++++++++
                '    Select Case astrCODE_KBN
                '        Case "0", "4" 'ＪＩＳ(改行なし)、ＥＢＣＤＩＣ
                '            ChDir(strDIR)
                '            fn_DVD_CPYTO_DISK = 200     'コード区分異常（JIS改行あり）
                '            Exit Function
                '    End Select
                '    'If astrCODE_KBN <> 2 Then
                '    '    ChDir(strDIR)
                '    '    fn_FD_CPYTO_DISK = 200       'コード区分異常（JIS改行あり）
                '    '    Exit Function
                '    'End If
                '    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                'Else
                '    If astrCODE_KBN <> "0" Then
                '        ChDir(strDIR)
                '        fn_DVD_CPYTO_DISK = 300       'コード区分異常（JIS改行なし）
                '        Exit Function
                '    End If
                'End If
                '2013/10/24 saitou 標準修正 UPD --------------------------------------------------<<<<

                FileClose(intFILE_NO_1)
                FileOpen(intFILE_NO_1, strFILE, OpenMode.Input, , , aintREC_LENGTH)   '入力ファイル
                intFILE_NO_2 = FreeFile()
                FileOpen(intFILE_NO_2, astrOUT_FILE_NAME, OpenMode.Output, , , aintREC_LENGTH)   '出力ファイル
                Do Until EOF(intFILE_NO_1)
                    strFILE_DATA = LineInput(intFILE_NO_1)
                    Select Case astrCODE_KBN
                        Case "2"    'JIS119改行
                            strFILE_DATA &= Space(1)
                        Case "3"    'JIS118改行
                            strFILE_DATA &= Space(2)
                    End Select
                    Print(intFILE_NO_2, strFILE_DATA)
                Loop
                FileClose(intFILE_NO_1)
                FileClose(intFILE_NO_2)
                If Err.Number <> 0 Then
                    fn_DVD_CPYTO_DISK = 400    '出力ファイル作成失敗
                End If
        End Select
        ChDir(strDIR)
        fn_DVD_CPYTO_DISK = 0

    End Function
    '**********20120704 mubuchi DVD対応 END****************************************************************************************************************************************************************************************************************************

    Function fn_JOBMAST_TOUROKU_CHECK(ByVal strJOBID As String, ByVal strUSERID As String, ByVal strPARAMETA As String) As Boolean
        '=====================================================================================
        'NAME           :fn_JOBMAST_TOUROKU_CHECK
        'Parameter      :strJOBID：起動するジョブＩＤ／strUSERID：ログインユーザ
        '　　　　　　　　:strPARAMETA：パラメータ
        'Description    :ジョブマスタにジョブを登録する前に既に登録されていないか検索
        'Return         :True=OK(未登録),False=NG（登録済）
        'Create         :2004/08/19
        'Update         :
        '=====================================================================================
        fn_JOBMAST_TOUROKU_CHECK = False

        gstrSSQL = "SELECT * FROM JOBMAST "
        gstrSSQL = gstrSSQL & " WHERE JOBID_J = '" & Trim(strJOBID) & "' AND PARAMETA_J = '" & Trim(strPARAMETA) & "' AND STATUS_J IN('0','1')"

        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = gstrSSQL
        gdbCOMMAND.Connection = gdbcCONNECT

        gdbrREADER = gdbCOMMAND.ExecuteReader   '読込のみ
        Dim COUNT As Integer
        COUNT = 0
        While (gdbrREADER.Read)
            COUNT += 1
            fn_JOBMAST_TOUROKU_CHECK = False
        End While
        If Err.Number <> 0 Then
            Exit Function
        End If
        If COUNT = 0 Then
            fn_JOBMAST_TOUROKU_CHECK = True
        Else
            'MessageBox.Show("バッチジョブ管理マスタに登録済です")
        End If
        gdbcCONNECT.Close()

    End Function
    Function fn_Select_TAKOUMAST(ByVal aTORIS_CODE As String, ByVal aTORIF_CODE As String, ByVal aKIN_NO As String, _
     ByRef aITAKU_CODE As String, ByRef aITAKU_KIN As String, ByRef aITAKU_SIT As String, ByRef aITAKU_KAMOKU As String, _
          ByRef aITAKU_KOUZA As String, ByRef aBAITAI_CODE As String, ByRef aS_FILE_NAME As String, ByRef aR_FILE_NAME As String, ByRef aCODE_KBN As String) As Boolean
        '=====================================================================================
        'NAME           :fn_Select_TAKOUMAST
        'Parameter      :aTORIS_CODE：取引先主コード／aTORIF_CODE：取引先副コード／aKIN_NO:金融機関コード
        '               :aITAKU_CODE:委託者コード／aITAKU_KIN：委託金融機関コード／aITAKU_SIT：委託支店コード
        '               :aITAKU_KAMOKU:委託科目／aITAKU_KOUZA：委託口座／aBAITAI_CODE：媒体コード
        '               :aS_FILE_NAME:送信ファイル名／aR_FILE_NAME:受信ファイル名／aCODE_KBN：コード区分
        'Description    :他行マスタ検索
        'Return         :True=OK(検索ヒット),False=NG（検索失敗）
        'Create         :2004/05/28
        'Update         :
        '=====================================================================================
        fn_Select_TAKOUMAST = False

        gstrSSQL = "SELECT * FROM TAKOUMAST"
        gstrSSQL = gstrSSQL & " WHERE TORIS_CODE_V = '" & Trim(aTORIS_CODE) & _
                               "' AND TORIF_CODE_V = '" & Trim(aTORIF_CODE) & _
                               "' AND TKIN_NO_V = '" & Trim(aKIN_NO) & "'"

        gdbcCONNECT.ConnectionString = CASTCommon.DB.CONNECT
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = gstrSSQL
        gdbCOMMAND.Connection = gdbcCONNECT

        gdbrREADER = gdbCOMMAND.ExecuteReader

        '読込のみ
        If gdbrREADER.Read = False Then
            fn_Select_TAKOUMAST = False
            gdbcCONNECT.Close()
            Exit Function
        Else
            aITAKU_CODE = gdbrREADER.Item("ITAKU_CODE_V")
            aITAKU_KIN = gdbrREADER.Item("TKIN_NO_V")
            aITAKU_SIT = gdbrREADER.Item("TSIT_NO_V")
            aITAKU_KAMOKU = gdbrREADER.Item("KAMOKU_V")
            aITAKU_KOUZA = gdbrREADER.Item("KOUZA_V")
            aBAITAI_CODE = gdbrREADER.Item("BAITAI_CODE_V")
            aS_FILE_NAME = fn_chenge_null_value(gdbrREADER.Item("SFILE_NAME_V"))
            aR_FILE_NAME = fn_chenge_null_value(gdbrREADER.Item("RFILE_NAME_V"))
            aCODE_KBN = gdbrREADER.Item("CODE_KBN_V")

            fn_Select_TAKOUMAST = True
        End If
        gdbcCONNECT.Close()
    End Function
    Function fn_DISK_CPYTO_DEN(ByVal strTORI_CODE As String, ByVal strIN_FILE_NAME As String, ByVal strOUT_FILE_NAME As String, ByVal intREC_LENGTH As Integer, ByVal strCODE_KBN As String, ByVal strP_FILE As String) As Integer
        '=====================================================================================
        'NAME           :fn_DISK_CPYTO_DEN
        'Parameter      :strTORI_CODE：取引先コード／strIN_FILE_NAME：入力ファイル名／
        '               :strOUT_FILE_NAME：出力ファイル／intREC_LENGTH：レコード長／
        '               :strCODE_KBN：コード区分／strP_FILE：FTRAN+の定義ファイル
        'Description    :ファイルを伝送ファイルにコピーする
        'Return         :0=成功、100=コード変換失敗、
        'Create         :2004/08/20
        'Update         :
        '=====================================================================================
        fn_DISK_CPYTO_DEN = 100
        Dim strDIR As String
        strDIR = CurDir()

        '2013/10/24 saitou 標準修正 UPD -------------------------------------------------->>>>
        'INIファイルの取得方法改修
        gstrFTR_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTR")
        If gstrFTR_OPENDIR.Equals("err") = True OrElse gstrFTR_OPENDIR = String.Empty Then
            Exit Function
        End If

        gstrFTRANP_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
        If gstrFTRANP_OPENDIR.Equals("err") = True OrElse gstrFTRANP_OPENDIR = String.Empty Then
            Exit Function
        End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTR"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTR_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTRANP"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTRANP_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If
        '2013/10/24 saitou 標準修正 UPD --------------------------------------------------<<<<

        '-----------------------------------------------------------
        'コード変換処理
        'コード変換区分が 2 の場合はIBM形式 EBCDIC コードとみなし
        'JIS8コードへのコード変換を行う
        '-----------------------------------------------------------
        Dim strCMD As String = ""
        Dim strTEIGI_FIEL As String
        If Dir(strOUT_FILE_NAME) <> "" Then
            Kill(strOUT_FILE_NAME)
        End If
        ChDir(gstrFTRANP_OPENDIR)

        strTEIGI_FIEL = gstrFTR_OPENDIR & strP_FILE
        Select Case strCODE_KBN
            Case "0"          'JIS  ホスト→WINファイル変換
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                'strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & strIN_FILE_NAME & " """ & strOUT_FILE_NAME & """ ++" & strTEIGI_FIEL
                strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & """" & strIN_FILE_NAME & """" & " " & """" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                'Case "1", "3"     'EBCDIC　　WIN→ホストファイル変換
                '   strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis putrand " & strIN_FILE_NAME & " """ & strOUT_FILE_NAME & """ ++" & strTEIGI_FIEL
            Case "4"            'EBCDIC コード区分変更 2009/09/30 kakiwaki
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                'strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis putrand " & strIN_FILE_NAME & " """ & strOUT_FILE_NAME & """ ++" & strTEIGI_FIEL
                strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis putrand " & """" & strIN_FILE_NAME & """" & " " & """" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

            Case "1", "2", "3"
                '   Case "2"          'JIS改あり  ホスト→WINファイル変換
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                'strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getdata " & strIN_FILE_NAME & " """ & strOUT_FILE_NAME & """ ++" & strTEIGI_FIEL
                strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getdata " & """" & strIN_FILE_NAME & """" & " " & """" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

        End Select

        'intIDPROCESS = Shell(strCMD, , True, 100000)
        'intIDPROCESS = Shell(strCMD, , True)

        'lngProcess = OpenProcess(lngPROCESS_QUERY_INFOMATION, 1, intIDPROCESS)
        'Do
        '    lngRET = GetExitCodeProcess(lngProcess, lngEXITCODE)
        '    System.Windows.Forms.Application.DoEvents()
        'Loop While (lngEXITCODE = lngSTILL_ACTIVE)

        'lngRET = CloseHandle(lngProcess)
        Dim ProcFT As New Process
        Dim ProcInfo As New ProcessStartInfo(gstrFTRANP_OPENDIR & "FP", strCMD.Substring(3))
        ProcInfo.CreateNoWindow = True
        ProcInfo.WorkingDirectory = gstrFTRANP_OPENDIR
        ProcFT = Process.Start(ProcInfo)
        ProcFT.WaitForExit()
        lngEXITCODE = ProcFT.ExitCode

        If lngEXITCODE = 0 Then
            fn_DISK_CPYTO_DEN = 0
        Else
            fn_DISK_CPYTO_DEN = 100         'コード変換失敗
            Exit Function
        End If

        ChDir(strDIR)

    End Function
    Function fn_DISK_CPYTO_FD(ByVal strTORI_CODE As String, ByVal strIN_FILE_NAME As String, ByVal strOUT_FILE_NAME As String, ByVal intREC_LENGTH As Integer, ByVal strCODE_KBN As String, ByVal strP_FILE As String, ByVal bloMSG As Boolean, ByVal msgTitle As String) As Integer
        '=====================================================================================
        'NAME           :fn_DISK_CPYTO_FD
        'Parameter      :strTORI_CODE：取引先コード／strIN_FILE_NAME：入力ファイル名／
        '               :strOUT_FILE_NAME：出力ファイル／intREC_LENGTH：レコード長／
        '               :strCODE_KBN：コード区分／bloMSG：確認メッセージの有無
        'Description    :ファイルをＦＤ３．５にコピーする
        'Return         :0=成功、100=ＦＤ書込み失敗（IBM形式）、200=ＦＤ書込み失敗（DOS形式）
        'Create         :2004/08/20
        'Update         :
        '=====================================================================================
        fn_DISK_CPYTO_FD = 100
        Dim strDIR As String
        strDIR = CurDir()

        '2013/10/24 saitou 標準修正 UPD -------------------------------------------------->>>>
        'INIファイルの取得方法改修
        gstrFTR_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTR")
        If gstrFTR_OPENDIR.Equals("err") = True OrElse gstrFTR_OPENDIR = String.Empty Then
            Exit Function
        End If

        gstrFTRAN2000_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTRAN2000")
        If gstrFTRAN2000_OPENDIR.Equals("err") = True OrElse gstrFTRAN2000_OPENDIR = String.Empty Then
            Exit Function
        End If

        gstrFTRANP_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
        If gstrFTRANP_OPENDIR.Equals("err") = True OrElse gstrFTRANP_OPENDIR = String.Empty Then
            Exit Function
        End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTR"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTR_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTRAN2000"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTRAN2000_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTRANP"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTRANP_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If
        '2013/10/24 saitou 標準修正 UPD --------------------------------------------------<<<<
        '-----------------------------------------------------------
        'コード変換処理
        'コード変換区分が 2 の場合はIBM形式 EBCDIC コードとみなし
        'JIS8コードへのコード変換を行う
        '-----------------------------------------------------------
        Dim strCMD As String = ""
        Dim strTEIGI_FIEL As String
        strTEIGI_FIEL = gstrFTR_OPENDIR & strP_FILE

        Select Case strCODE_KBN
            '2009/09/18.Sakon　コード区分変更に伴う変更 +++
            'Case "1", "3"     'EBCDIC
            Case "4"
                '++++++++++++++++++++++++++++++++++++++++++
                If bloMSG = True Then
                    ' 取引先コード表示桁変更 2009/10/04 kakiwaki
                    '   If MessageBox.Show("ＦＤ(ＩＢＭ形式)をセットしてください。" & vbCrLf & " 取引先コード：" & strTORI_CODE.Substring(0, 7) & "-" & strTORI_CODE.Substring(7, 2), "ＦＤ書き込み", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    If MessageBox.Show(String.Format(MSG0073I, strTORI_CODE.Substring(0, 10) & "-" & strTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                        fn_DISK_CPYTO_FD = 100
                        Exit Function
                    End If
                End If

                '----------------------------------------------
                'FTRAN2000を使用してＦＤ３．５に書き込む
                '----------------------------------------------
                'ＩＢＭフロッピーディスクがセットされているか判定
FD_EXIST_IBM:
                ChDir(gstrFTRAN2000_OPENDIR)
                '2010/06/01 Window非表示
                strCMD = "FT /nwd/ ILIST A:"
                'strCMD = "FT /wc/ ILIST A:"

                'intIDPROCESS = Shell(strCMD, , True, 100000)
                ''intIDPROCESS = Shell(strCMD, , True)

                ''lngProcess = OpenProcess(lngPROCESS_QUERY_INFOMATION, 1, intIDPROCESS)
                ''Do
                ''    lngRET = GetExitCodeProcess(lngProcess, lngEXITCODE)
                ''    System.Windows.Forms.Application.DoEvents()
                ''Loop While (lngEXITCODE = lngSTILL_ACTIVE)

                ''lngRET = CloseHandle(lngProcess)
                Dim ProcFT As New Process
                'Dim ProcInfo As New ProcessStartInfo(gstrFTRAN2000_OPENDIR & "FT", "/wc/ ILIST A:")
                Dim ProcInfo As New ProcessStartInfo(gstrFTRAN2000_OPENDIR & "FT", "/nwd/ ILIST A:")
                ProcInfo.CreateNoWindow = True
                ProcInfo.WorkingDirectory = gstrFTRAN2000_OPENDIR
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                Else
                    ' 取引先コード表示桁変更 2009/10/04 kakiwaki 
                    '   MessageBox.Show("ＦＤ(ＩＢＭ形式)をセットしてください。" & vbCrLf & " 取引先コード：" & strTORI_CODE.Substring(0, 7) & "-" & strTORI_CODE.Substring(7, 2), "ＦＤ書き込み", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)
                    If MessageBox.Show(String.Format(MSG0073I, strTORI_CODE.Substring(0, 10) & "-" & strTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                        fn_DISK_CPYTO_FD = 100
                        Exit Function
                    End If
                    GoTo FD_EXIST_IBM
                End If


                '------------------------------------------------
                '書き込み
                '------------------------------------------------
                '2010/02/18 メッセージ追加
                If MessageBox.Show(String.Format(MSG0063I, strTORI_CODE.Substring(0, 10) & "-" & strTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    fn_DISK_CPYTO_FD = 100
                    Exit Function
                End If

                ChDir(gstrFTRAN2000_OPENDIR)

                '2010/02/18 修正
                'strCMD = "FT /WC/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis PUTRAND " & strIN_FILE_NAME & " A:" & strOUT_FILE_NAME & "/REP ++" & strTEIGI_FIEL
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                'strCMD = "FT /nwd/ cload " & gstrFTR_OPENDIR & "FUSION2007 ; ank ebcdic ; kanji 83_jis PUTRAND " & strIN_FILE_NAME & " A:" & strOUT_FILE_NAME & "/REP ++" & strTEIGI_FIEL
                strCMD = "FT /nwd/ cload " & gstrFTR_OPENDIR & "FUSION2007 ; ank ebcdic ; kanji 83_jis PUTRAND " & """" & strIN_FILE_NAME & """" & " " & """A:" & strOUT_FILE_NAME & """" & "/REP ++" & """" & strTEIGI_FIEL & """"
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                'intIDPROCESS = Shell(strCMD, , True, 100000)
                ''intIDPROCESS = Shell(strCMD, , True)

                ''lngProcess = OpenProcess(lngPROCESS_QUERY_INFOMATION, 1, intIDPROCESS)
                ''Do
                ''    lngRET = GetExitCodeProcess(lngProcess, lngEXITCODE)
                ''    System.Windows.Forms.Application.DoEvents()
                ''Loop While (lngEXITCODE = lngSTILL_ACTIVE)

                ''lngRET = CloseHandle(lngProcess)
                ProcFT = New Process
                ProcInfo = New ProcessStartInfo(gstrFTRAN2000_OPENDIR & "FT", strCMD.Substring(3))
                ProcInfo.CreateNoWindow = True
                ProcInfo.WorkingDirectory = gstrFTRAN2000_OPENDIR
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                    fn_DISK_CPYTO_FD = 0
                Else
                    fn_DISK_CPYTO_FD = 100         'コード変換失敗
                    ChDir(strDIR)
                    Exit Function
                End If
            Case Else        'JIS,JIS改
                If bloMSG = True Then
                    ' 取引先コード表示桁変更 2009/10/04 kakiwaki 
                    '   If MessageBox.Show("ＦＤ(ＤＯＳ形式)をセットしてください。" & vbCrLf & " 取引先コード：" & strTORI_CODE.Substring(0, 7) & "-" & strTORI_CODE.Substring(7, 2), "ＦＤ書き込み", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    If MessageBox.Show(String.Format(MSG0064I, strTORI_CODE.Substring(0, 10) & "-" & strTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                        fn_DISK_CPYTO_FD = 100
                        Exit Function
                    End If
                End If
                Dim strFOLDER As String

                strFOLDER = "A:\"
                'strIN_FILE_NAME = strFOLDER & strIN_FILE_NAME.Trim
                strIN_FILE_NAME = strIN_FILE_NAME.Trim
FD_EXIST:
                Try
                    Dir(strFOLDER)
                    '-------------------------------------
                    'ＤＯＳ形式か確認(プログラム追加予定)
                    '-------------------------------------
                    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'FileCopy(strIN_FILE_NAME, strFOLDER & strOUT_FILE_NAME)
                    ''FileOpen(1, Dir(strFOLDER), OpenMode.Binary)
                    ''FileClose(1)
                    If Directory.GetFiles(strFOLDER).Length = 0 Then
                        Throw New Exception
                    End If
                    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                Catch EX As Exception      'FDがセットされていなかった場合
                    ' 取引先コード表示桁変更 2009/10/04 kakiwaki
                    '   If MessageBox.Show("ＦＤ(ＤＯＳ形式)をセットしてください。" & vbCrLf & _
                    '      "取引先コード：" & strTORI_CODE.Substring(0, 7) & "-" & strTORI_CODE.Substring(7, 2), _

                    If MessageBox.Show(String.Format(MSG0064I, strTORI_CODE.Substring(0, 10) & "-" & strTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) _
                       = DialogResult.OK Then
                        GoTo FD_EXIST
                    Else
                        '   fn_DISK_CPYTO_FD = 100         'ファイル読み込み失敗
                        fn_DISK_CPYTO_FD = 200         'ファイル読み込み失敗        2009/09/30 kaiwaki
                        ChDir(strDIR)
                        Exit Function
                    End If
                End Try
                '----------------------------------------------
                'FTRAN+を使用してＦＤ３．５に書き込む
                '----------------------------------------------
                ChDir(gstrFTRANP_OPENDIR)
                '2017/05/26 タスク）西野 CHG 標準版修正（JIS118,119改対応）-------------------------- START
                Select Case strCODE_KBN
                    Case "0", "2", "3"      'JIS,JIS改(119),JIS改(118)
                        strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & """" & strIN_FILE_NAME & """" & " " & """A:" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                    Case "1"                'JIS改(120)
                        strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getdata " & """" & strIN_FILE_NAME & """" & " " & """A:" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                End Select
                'Select Case strCODE_KBN
                '    Case "0"          'JIS  ホスト→WINファイル変換
                '        ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                '        'strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & strIN_FILE_NAME & " A:" & strOUT_FILE_NAME & " ++" & strTEIGI_FIEL
                '        strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & """" & strIN_FILE_NAME & """" & " " & """A:" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                '        ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                '        '2009/09/18.Sakon　コード区分変更に伴う変更 ++++++++++++++++++

                '    Case "1", "2", "3"          'JIS改あり  ホスト→WINファイル変換
                '        'Case "2"          'JIS改あり  ホスト→WINファイル変換
                '        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                '        ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                '        'strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getdata " & strIN_FILE_NAME & " A:" & strOUT_FILE_NAME & " ++" & strTEIGI_FIEL
                '        strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getdata " & """" & strIN_FILE_NAME & """" & " " & """A:" & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                '        ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                'End Select
                '2017/05/26 タスク）西野 CHG 標準版修正（JIS118,119改対応）-------------------------- END

                'intIDPROCESS = Shell(strCMD, , True, 100000)
                ''intIDPROCESS = Shell(strCMD, , True)

                ''lngProcess = OpenProcess(lngPROCESS_QUERY_INFOMATION, 1, intIDPROCESS)
                ''Do
                ''    lngRET = GetExitCodeProcess(lngProcess, lngEXITCODE)
                ''    System.Windows.Forms.Application.DoEvents()
                ''Loop While (lngEXITCODE = lngSTILL_ACTIVE)

                ''lngRET = CloseHandle(lngProcess)
                Dim ProcFT As New Process
                Dim ProcInfo As New ProcessStartInfo(gstrFTRANP_OPENDIR & "FP", strCMD.Substring(3))
                ProcInfo.CreateNoWindow = True
                ProcInfo.WorkingDirectory = gstrFTRANP_OPENDIR
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                    fn_DISK_CPYTO_FD = 0
                Else
                    '   fn_DISK_CPYTO_FD = 100         'コード変換失敗
                    fn_DISK_CPYTO_FD = 200         'コード変換失敗      2009/09/30
                    ChDir(strDIR)
                    Exit Function
                End If
        End Select
        ChDir(strDIR)
        fn_DISK_CPYTO_FD = 0

    End Function

    '******20120705 mubuchi DVD追加対応 START******************************************************************************************************************************************************************************************************************************************>>>>
    Function fn_DISK_CPYTO_DVD(ByVal strTORI_CODE As String, ByVal strIN_FILE_NAME As String, ByVal strOUT_FILE_NAME As String, _
                               ByVal intREC_LENGTH As Integer, ByVal strCODE_KBN As String, ByVal strP_FILE As String, _
                               ByVal bloMSG As Boolean, ByVal msgTitle As String, _
                               ByVal strBAITAI_CODE As String) As Integer
        '=====================================================================================
        'NAME           :fn_DISK_CPYTO_DVD
        'Parameter      :strTORI_CODE：取引先コード／strIN_FILE_NAME：入力ファイル名／
        '               :strOUT_FILE_NAME：出力ファイル／intREC_LENGTH：レコード長／
        '               :strCODE_KBN：コード区分／bloMSG：確認メッセージの有無
        'Description    :ファイルをDVDにコピーする
        'Return         :0=成功、100=DVD書込み失敗（EBCDIC）、200=DVD書込み失敗（JIS）
        'Create         :2004/08/20
        'Update         :2013/12/24 saitou 標準版 外部媒体対応
        '=====================================================================================
        fn_DISK_CPYTO_DVD = 100
        Dim strDIR As String
        strDIR = CurDir()

        '2013/10/24 saitou 標準修正 UPD -------------------------------------------------->>>>
        'INIファイルの取得方法改修
        gstrFTR_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTR")
        If gstrFTR_OPENDIR.Equals("err") = True OrElse gstrFTR_OPENDIR = String.Empty Then
            Exit Function
        End If

        gstrFTRAN2000_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTRAN2000")
        If gstrFTRAN2000_OPENDIR.Equals("err") = True OrElse gstrFTRAN2000_OPENDIR = String.Empty Then
            Exit Function
        End If

        gstrFTRANP_OPENDIR = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
        If gstrFTRANP_OPENDIR.Equals("err") = True OrElse gstrFTRANP_OPENDIR = String.Empty Then
            Exit Function
        End If

        '2013/12/24 saitou 標準版 外部媒体対応 DEL -------------------------------------------------->>>>
        'DVDは媒体1として扱うことにする
        'DVD_DRIVE = CASTCommon.GetFSKJIni("DVD", "DRIVE")
        'If DVD_DRIVE.Equals("err") = True OrElse DVD_DRIVE = String.Empty Then
        '    Exit Function
        'End If
        '2013/12/24 saitou 標準版 外部媒体対応 DEL --------------------------------------------------<<<<
        '2013/12/24 saitou 標準版 外部媒体対応 ADD -------------------------------------------------->>>>
        ' 2016/01/27 タスク）綾部 CHG 【PG】UI_B-14-99(RSV2対応(USB)) -------------------- START
        'Dim BAITAI_1 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_1")
        'Dim BAITAI_2 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_2")
        'Dim BAITAI_3 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_3")
        'Dim BAITAI_4 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_4")
        'Dim BAITAI_5 As String = CASTCommon.GetFSKJIni("COMMON", "BAITAI_5")
        'Dim strUseBaitai As String = String.Empty       '使う媒体
        'Dim strUseBaitaiName As String = String.Empty   '使う媒体名
        'Select Case strBAITAI_CODE
        '    Case "11"   'DVD
        '        strUseBaitai = BAITAI_1
        '        strUseBaitaiName = "DVD-RAM"
        '    Case "12"
        '        strUseBaitai = BAITAI_2
        '        strUseBaitaiName = ""
        '    Case "13"
        '        strUseBaitai = BAITAI_3
        '        strUseBaitaiName = ""
        '    Case "14"
        '        strUseBaitai = BAITAI_4
        '        strUseBaitaiName = ""
        '    Case "15"
        '        strUseBaitai = BAITAI_5
        '        strUseBaitaiName = ""
        'End Select
        Dim strUseBaitai As String = String.Empty       '使う媒体
        Dim strUseBaitaiName As String = String.Empty   '使う媒体名
        Select Case strBAITAI_CODE
            Case "11"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_1")
            Case "12"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_2")
            Case "13"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_3")
            Case "14"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_4")
            Case "15"
                strUseBaitai = CASTCommon.GetFSKJIni("COMMON", "BAITAI_5")
        End Select
        strUseBaitaiName = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"), strBAITAI_CODE)
        ' 2016/01/27 タスク）綾部 CHG 【PG】UI_B-14-99(RSV2対応(USB)) -------------------- END
        '2013/12/24 saitou 標準版 外部媒体対応 ADD --------------------------------------------------<<<<

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTR"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTR_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTRAN2000"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTRAN2000_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If

        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "COMMON"
        'gstrIKeyName = "FTRANP"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    gstrFTRANP_OPENDIR = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If
        'gstrIFileName = CurDir()     'カレントディレクトリの取得
        'gstrIFileName = gstrIFileName & "\FSKJ.INI"
        'gstrIAppName = "DVD"
        'gstrIKeyName = "DRIVE"
        'gstrIDefault = "err"
        'gintTEMP_LEN = 0
        'gstrTEMP = Space(100)

        'gintTEMP_LEN = GetPrivateProfileString(gstrIAppName, gstrIKeyName, gstrIDefault, gstrTEMP, gstrTEMP.Length, gstrIFileName)
        'If Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1) = gstrIDefault Then
        '    Exit Function
        'Else
        '    '引数 gstrTEMPには末尾にNullが入るので取得後にNullを削除
        '    DVD_DRIVE = Microsoft.VisualBasic.Left(gstrTEMP, InStr(gstrTEMP, Chr(0)) - 1)
        'End If
        '2013/10/24 saitou 標準修正 UPD --------------------------------------------------<<<<
        '-----------------------------------------------------------
        'コード変換処理
        'コード変換区分が 4 の場合はIBM形式 EBCDIC コードとみなし
        'JIS8コードへのコード変換を行う
        '-----------------------------------------------------------
        Dim strCMD As String = ""
        Dim strTEIGI_FIEL As String
        strTEIGI_FIEL = gstrFTR_OPENDIR & strP_FILE

        Select Case strCODE_KBN
            Case "4"        'EBCDIC
                If bloMSG = True Then
                    'DVDがセットされていなければ、DVDセットのメッセージを表示する。
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                    '媒体名を設定できるように変更
                    If MessageBox.Show(String.Format(MSG0075I, strUseBaitaiName, strTORI_CODE.Substring(0, 10) & "-" & strTORI_CODE.Substring(10, 2)).Replace("読込", "書込"), _
                                       msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                        fn_DISK_CPYTO_DVD = 100
                        Exit Function
                    End If
                    'If MessageBox.Show(String.Format(MSG0076I, strTORI_CODE.Substring(0, 10) & "-" & strTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    '    fn_DISK_CPYTO_DVD = 100
                    '    Exit Function
                    'End If
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                End If
                Dim strFOLDER As String

                '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                strFOLDER = strUseBaitai
                'strFOLDER = DVD_DRIVE
                '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                strIN_FILE_NAME = strIN_FILE_NAME.Trim

                '----------------------------------------------
                'FTRAN+を使用してDVDに書き込む
                '----------------------------------------------
                ChDir(gstrFTRANP_OPENDIR)

                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                'strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis PUTRAND " & strIN_FILE_NAME & " " & strFOLDER & strOUT_FILE_NAME & " ++" & strTEIGI_FIEL
                strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis PUTRAND " & """" & strIN_FILE_NAME & """" & " " & """" & strFOLDER & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                Dim ProcFT As New Process
                Dim ProcInfo As New ProcessStartInfo(gstrFTRANP_OPENDIR & "FP", strCMD.Substring(3))
                ProcInfo.CreateNoWindow = True
                ProcInfo.WorkingDirectory = gstrFTRANP_OPENDIR
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                    fn_DISK_CPYTO_DVD = 0
                Else
                    fn_DISK_CPYTO_DVD = 200         'コード変換失敗      2009/09/30
                    ChDir(strDIR)
                    Exit Function
                End If


            Case Else        'JIS,JIS改
                If bloMSG = True Then
                    ' 取引先コード表示桁変更 2009/10/04 kakiwaki 
                    '   If MessageBox.Show("ＦＤ(ＤＯＳ形式)をセットしてください。" & vbCrLf & " 取引先コード：" & strTORI_CODE.Substring(0, 7) & "-" & strTORI_CODE.Substring(7, 2), "ＦＤ書き込み", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                    '媒体名を設定できるように変更
                    If MessageBox.Show(String.Format(MSG0075I, strUseBaitaiName, strTORI_CODE.Substring(0, 10) & "-" & strTORI_CODE.Substring(10, 2)).Replace("読込", "書込"), _
                                       msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                        fn_DISK_CPYTO_DVD = 100
                        Exit Function
                    End If
                    'If MessageBox.Show(String.Format(MSG0076I, strTORI_CODE.Substring(0, 10) & "-" & strTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.Cancel Then
                    '    fn_DISK_CPYTO_DVD = 100
                    '    Exit Function
                    'End If
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                End If
                Dim strFOLDER As String

                '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                strFOLDER = strUseBaitai
                'strFOLDER = DVD_DRIVE
                '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                strIN_FILE_NAME = strIN_FILE_NAME.Trim
DVD_EXIST:
                Try
                    Dir(strFOLDER)
                    '-------------------------------------
                    'ＤＯＳ形式か確認(プログラム追加予定)
                    '-------------------------------------
                    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                    'FileCopy(strIN_FILE_NAME, strFOLDER & strOUT_FILE_NAME)
                    'If Directory.GetFiles(strFOLDER).Length = 0 Then
                    '    Throw New Exception
                    'End If
                    If Directory.GetFiles(strFOLDER).Length = 0 Then
                        File.Create(strFOLDER & strOUT_FILE_NAME).Close()
                        If File.Exists(strFOLDER & strOUT_FILE_NAME) = False Then
                            Throw New Exception
                        Else
                            File.Delete(strFOLDER & strOUT_FILE_NAME)
                        End If
                    End If
                    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                Catch EX As Exception      'DVDがセットされていなかった場合
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                    '媒体名を設定できるように変更
                    If MessageBox.Show(String.Format(MSG0076I, strUseBaitaiName, strTORI_CODE.Substring(0, 10) & "-" & strTORI_CODE.Substring(10, 2)), _
                                       msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) = DialogResult.OK Then
                        GoTo DVD_EXIST
                    Else
                        ChDir(strDIR)
                        fn_DISK_CPYTO_DVD = 200         'ファイル読み込み失敗
                        Exit Function
                    End If
                    'If MessageBox.Show(String.Format(MSG0076I, strTORI_CODE.Substring(0, 10) & "-" & strTORI_CODE.Substring(10, 2)), msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly) _
                    '   = DialogResult.OK Then
                    '    GoTo DVD_EXIST
                    'Else
                    '    '   fn_DISK_CPYTO_FD = 100         'ファイル読み込み失敗
                    '    fn_DISK_CPYTO_DVD = 200         'ファイル読み込み失敗        2009/09/30 kaiwaki
                    '    ChDir(strDIR)
                    '    Exit Function
                    'End If
                    '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<
                End Try
                '----------------------------------------------
                'FTRAN+を使用してDVDに書き込む
                '----------------------------------------------
                ChDir(gstrFTRANP_OPENDIR)
                Select Case strCODE_KBN
                    '2017/05/26 タスク）西野 CHG 標準版修正（JIS118,119改対応）-------------------------- START
                    Case "0", "2", "3"      'JIS,JIS改(119),JIS改(118)
                        strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & """" & strIN_FILE_NAME & """" & " " & """" & strFOLDER & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"

                    Case "1"                'JIS改(120)
                        strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getdata " & """" & strIN_FILE_NAME & """" & " " & """" & strFOLDER & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                        'Case "0"          'JIS  ホスト→WINファイル変換
                        '    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                        '    'strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & strIN_FILE_NAME & " " & strFOLDER & strOUT_FILE_NAME & " ++" & strTEIGI_FIEL
                        '    strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getrand " & """" & strIN_FILE_NAME & """" & " " & """" & strFOLDER & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                        '    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END

                        'Case "1", "2", "3"          'JIS改あり  ホスト→WINファイル変換
                        '    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- START
                        '    'strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getdata " & strIN_FILE_NAME & " " & strFOLDER & strOUT_FILE_NAME & " ++" & strTEIGI_FIEL
                        '    strCMD = "FP /nwd/ cload " & gstrFTR_OPENDIR & "FUSION ; ank ebcdic ; kanji 83_jis getdata " & """" & strIN_FILE_NAME & """" & " " & """" & strFOLDER & strOUT_FILE_NAME & """" & " ++" & """" & strTEIGI_FIEL & """"
                        '    ' 2016/04/25 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応(標準バグ修正)) -------------------- END
                        '2017/05/26 タスク）西野 CHG 標準版修正（JIS118,119改対応）-------------------------- END

                End Select

                Dim ProcFT As New Process
                Dim ProcInfo As New ProcessStartInfo(gstrFTRANP_OPENDIR & "FP", strCMD.Substring(3))
                ProcInfo.CreateNoWindow = True
                ProcInfo.WorkingDirectory = gstrFTRANP_OPENDIR
                ProcFT = Process.Start(ProcInfo)
                ProcFT.WaitForExit()
                lngEXITCODE = ProcFT.ExitCode

                If lngEXITCODE = 0 Then
                    fn_DISK_CPYTO_DVD = 0
                Else
                    fn_DISK_CPYTO_DVD = 200         'コード変換失敗
                    ChDir(strDIR)
                    Exit Function
                End If
        End Select
        ChDir(strDIR)
        fn_DISK_CPYTO_DVD = 0

    End Function
    '******20120705 mubuchi DVD追加対応 END******************************************************************************************************************************************************************************************************************************************<<<<

    Function fn_POWER_SORT(ByVal aDisposalNumber As Integer, ByVal aFieldDefinition As Integer, ByVal aKeyCmdStr As String, ByVal aInputFiles As String, ByVal aInputFileType As Integer, ByVal aOutputFile As String, ByVal aOutputFileType As Integer, ByVal aMaxRecordLength As Integer) As Boolean
        '=====================================================================================
        'NAME           :fn_POWER_SORT
        'Parameter      :aTORIS_CODE：取引先主コード／aTORIF_CODE：取引先副コード／aKIN_NO:金融機関コード
        '               :aITAKU_CODE:委託者コード／aITAKU_KIN：委託金融機関コード／aITAKU_SIT：委託支店コード
        '               :aITAKU_KAMOKU:委託科目／aITAKU_KOUZA：委託口座／aBAITAI_CODE：媒体コード
        '               :aS_FILE_NAME:送信ファイル名／aR_FILE_NAME:受信ファイル名／aCODE_KBN：コード区分
        'Description    :他行マスタ検索
        'Return         :True=OK(検索ヒット),False=NG（検索失敗）
        'Create         :2004/08/25
        'Update         :
        '=====================================================================================
        fn_POWER_SORT = False
        '2017/12/11 saitou 広島信金(RSV2標準) UPD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START
        Dim PowerSort As New clsPowerSORT
        PowerSort.DisposalNumber = aDisposalNumber
        PowerSort.FieldDefinition = aFieldDefinition
        PowerSort.KeyCmdStr = aKeyCmdStr
        PowerSort.InputFiles = aInputFiles
        PowerSort.InputFileType = aInputFileType
        PowerSort.OutputFile = aOutputFile
        PowerSort.OutputFileType = aOutputFileType
        PowerSort.MaxRecordLength = aMaxRecordLength
        PowerSort.Action()

        'Dim frmREPORT As New clsFUSION.frmREPORT
        'Dim PowerSort As AxPowerSORT_Lib.AxPowerSORT = frmREPORT.objAxPowerSORT
        'PowerSort.DispMessage = False
        'PowerSort.DisposalNumber = aDisposalNumber
        'PowerSort.FieldDefinition = aFieldDefinition
        'PowerSort.KeyCmdStr = aKeyCmdStr
        'PowerSort.InputFiles = aInputFiles
        'PowerSort.InputFileType = aInputFileType
        'PowerSort.OutputFile = aOutputFile
        'PowerSort.OutputFileType = aOutputFileType
        'PowerSort.MaxRecordLength = aMaxRecordLength
        'PowerSort.Action()
        '2017/12/11 saitou 広島信金(RSV2標準) ADD ------------------------------------------------------------ END

        If PowerSort.ErrorCode <> 0 Then
            Exit Function
        End If
        fn_POWER_SORT = True
    End Function
    Function fn_CHECK_DIR(ByVal astrSEEK_DIR As String, ByVal astrIN_DIR As String) As Boolean
        '============================================================================
        'NAME           :fn_CHECK_DIR
        'Parameter      :SEEK_DIR:被検索ﾃﾞｨﾚｸﾄﾘ/IN_DIR:検索ﾃﾞｨﾚｸﾄﾘ
        'Description    :ﾃﾞｨﾚｸﾄﾘ検索ｻﾌﾞﾙｰﾁﾝ
        'Return         :True=あり(成功),False=なし（失敗）
        'Create         :2004/08/31
        'Update         :
        '============================================================================
        Dim W_DIR As String
        W_DIR = Dir(astrSEEK_DIR, vbDirectory)   ' 最初のディレクトリ名を返します。
        Do While W_DIR <> ""   ' ループを開始します。
            ' 現在のディレクトリと親ディレクトリは無視します。
            If W_DIR <> "." And W_DIR <> ".." Then
                ' ビット単位の比較を行い、W_DIR がディレクトリかどうかを調べます。
                If (GetAttr(astrSEEK_DIR & W_DIR) And vbDirectory) = vbDirectory Then
                    'MsgBox W_DIR
                    If W_DIR = astrIN_DIR Then      'ﾃﾞｨﾚｸﾄﾘ名が一致した場合
                        fn_CHECK_DIR = True
                        Exit Function
                    End If
                End If
            End If
            W_DIR = Dir()  ' 次のディレクトリ名を返します。
        Loop
        fn_CHECK_DIR = False

    End Function
     Function fn_chenge_null_value(ByVal strVALUE As Object) As String
        '============================================================================
        'NAME           :fn_chenge_null_value
        'Parameter      :strVALUE：チェック対象文字列
        'Description    :Null文字チェック　NULLの場合は空白を返す
        'Return         :string
        'Create         :2004/07/16
        'Update         :
        '============================================================================
        fn_chenge_null_value = ""
        If strVALUE Is DBNull.Value Then
            fn_chenge_null_value = ""
        Else
            fn_chenge_null_value = strVALUE.ToString
        End If
    End Function
    Function fn_chenge_null(ByVal strVALUE1 As Object, ByVal strVALUE2 As Object) As String
        '============================================================================
        'NAME           :fn_chenge_null
        'Parameter      :strVALUE1：チェック対象文字列,strVALUE2:strVALUE1がNULLだった場合strVALUE2をセット
        'Description    :Null文字チェック　NULLの場合はstrVALUE2を返す
        'Return         :string
        'Create         :2004/07/28
        'Update         :
        '============================================================================
        fn_chenge_null = ""
        If strVALUE1 Is DBNull.Value Then
            fn_chenge_null = strVALUE2
        Else
            fn_chenge_null = strVALUE1.ToString
        End If
    End Function
    Function fn_CHECK_NUM(ByVal objOBJ As String, ByVal strJNAME As String, ByVal gstrTITLE As String) As Boolean
        '============================================================================
        'NAME           :fn_CHECK_NUM
        'Parameter      :objOBJ：チェック対象オブジェクト
        'Description    :数値チェック(必須入力ではない項目の数字チェック)
        'Return         :True=OK,False=NG
        'Create         :2004/11/19
        'Update         :
        '============================================================================
        fn_CHECK_NUM = False
        Dim i As Integer

        For i = 0 To objOBJ.Length - 1       '小数点/符号ﾁｪｯｸ
            If objOBJ.Chars(i) = " " Then
                fn_CHECK_NUM = False
                MessageBox.Show(String.Format(MSG0285W, strJNAME), gstrTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            ElseIf Char.IsDigit(objOBJ.Chars(i)) = False Then
                fn_CHECK_NUM = False
                MessageBox.Show(String.Format(MSG0344W, strJNAME), gstrTITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        Next i
        fn_CHECK_NUM = True
    End Function
End Class

'2017/12/11 saitou 広島信金(RSV2標準) ADD サーバー印刷対応(RepoAgent64ビット対応) -------------------- START

''' <summary>
''' パワーソートクラス
''' </summary>
''' <remarks>2017/12/11 saitou 広島信金(RSV2標準) added for サーバー印刷対応</remarks>
Public Class clsPowerSORT
    Protected iDisposalNumber As Integer
    Protected iFieldDefinition As Integer
    Protected mFieldDelimiter As String
    Protected mKeyCmdStr As String
    Protected mInputFilesSkiprec As String
    Protected mSelCmdStr As String
    Protected mInputFiles As String
    Protected iInputFileType As Integer
    Protected mOutputFile As String
    Protected iOutputFileType As Integer
    Protected iMaxRecordLength As Integer
    Protected iErrorCode As Integer
    Protected mErrorDetail As String

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        iDisposalNumber = 0
        iFieldDefinition = 0
        mFieldDelimiter = ""
        mKeyCmdStr = ""
        mInputFilesSkiprec = ""
        mSelCmdStr = ""
        mInputFiles = ""
        iInputFileType = 0
        mOutputFile = ""
        iOutputFileType = 0
        iMaxRecordLength = 0
        iErrorCode = 0
        mErrorDetail = ""
    End Sub

#Region "プロパティ"

    Public Property DisposalNumber As Integer
        Get
            Return iDisposalNumber
        End Get
        Set(value As Integer)
            iDisposalNumber = value
        End Set
    End Property

    Public Property FieldDefinition As Integer
        Get
            Return iFieldDefinition
        End Get
        Set(value As Integer)
            iFieldDefinition = value
        End Set
    End Property

    Public Property FieldDelimiter As String
        Get
            Return mFieldDelimiter
        End Get
        Set(value As String)
            mFieldDelimiter = value
        End Set
    End Property

    Public Property KeyCmdStr As String
        Get
            Return mKeyCmdStr
        End Get
        Set(value As String)
            mKeyCmdStr = value
        End Set
    End Property

    Public Property InputFilesSkiprec As String
        Get
            Return mInputFilesSkiprec
        End Get
        Set(value As String)
            mInputFilesSkiprec = value
        End Set
    End Property

    Public Property SelCmdStr As String
        Get
            Return mSelCmdStr
        End Get
        Set(value As String)
            mSelCmdStr = value
        End Set
    End Property

    Public Property InputFiles As String
        Get
            Return mInputFiles
        End Get
        Set(value As String)
            mInputFiles = value
        End Set
    End Property

    Public Property InputFileType As Integer
        Get
            Return iInputFileType
        End Get
        Set(value As Integer)
            iInputFileType = value
        End Set
    End Property

    Public Property OutputFile As String
        Get
            Return mOutputFile
        End Get
        Set(value As String)
            mOutputFile = value
        End Set
    End Property

    Public Property OutputFileType As Integer
        Get
            Return iOutputFileType
        End Get
        Set(value As Integer)
            iOutputFileType = value
        End Set
    End Property

    Public Property MaxRecordLength As Integer
        Get
            Return iMaxRecordLength
        End Get
        Set(value As Integer)
            iMaxRecordLength = value
        End Set
    End Property

    Public Property ErrorCode As Integer
        Get
            Return iErrorCode
        End Get
        Set(value As Integer)
            iErrorCode = value
        End Set
    End Property

    Public Property ErrorDetail As String
        Get
            Return mErrorDetail
        End Get
        Set(value As String)
            mErrorDetail = value
        End Set
    End Property

#End Region

#Region "メソッド"

    Public Sub Action()
        Try
            Dim ProcSort As Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(GetRSKJIni("RSV2_V1.0.0", "POWERSORT_PATH"), "bsort.exe")
            If File.Exists(ProcInfo.FileName) = False Then
                ' パワーソートがないので、ソートせずに処理を続行する
                Return
            End If

            '入出力ファイル名チェック
            If InputFiles.Trim = "" OrElse OutputFile.Trim = "" Then
                Return
            End If

            '処理区分
            Dim SyoriKbn As String = String.Empty
            Select Case DisposalNumber
                Case 0 : SyoriKbn = "-s"    'ソート
                Case 1 : SyoriKbn = "-m"    'マージ
                Case 2 : SyoriKbn = "-c"    'コピー
                Case Else : Return
            End Select

            'テキストファイルオプション
            Dim TxtOpt As String = String.Empty
            Select Case FieldDefinition
                Case 0 : TxtOpt = "-Tflt"   '浮動
                Case 1 : TxtOpt = "-Tfix"   '固定
                Case Else : Return
            End Select

            'フィールド分離文字列
            Dim FieldSep As String = String.Empty
            If FieldDefinition = 0 AndAlso FieldDelimiter.Trim <> "" Then
                FieldSep = "-t " & FieldDelimiter
            End If

            '最大レコードサイズ
            Dim RecSize As String = String.Empty
            If MaxRecordLength <> 0 Then
                RecSize = "-z" & MaxRecordLength.ToString
            End If

            'スキップレコード番号
            Dim SkipRec As String = String.Empty
            If InputFilesSkiprec.Trim <> "" Then
                SkipRec = "-R " & InputFilesSkiprec
            End If

            '選択フィールドオプション
            Dim SelCmd As String = String.Empty
            If SelCmdStr.Trim <> "" Then
                SelCmd = "-p " & SelCmdStr
            End If

            ProcInfo.Arguments = String.Format("{0} {1} -""{2}"" {3} {4} {5} {6} -o {7} {8}", SyoriKbn, RecSize, KeyCmdStr, TxtOpt, FieldSep, SkipRec, SelCmd, OutputFile, InputFiles)

            ProcInfo.WorkingDirectory = Path.GetDirectoryName(ProcInfo.FileName)
            ProcInfo.UseShellExecute = False
            ProcInfo.RedirectStandardOutput = True
            ProcSort = Process.Start(ProcInfo)
            ProcSort.WaitForExit()

            'エラーコード設定
            ErrorCode = ProcSort.ExitCode

            If ProcSort.ExitCode <> 0 Then
                ErrorDetail = ProcSort.StandardOutput.ReadToEnd()
                Return
            End If

        Catch ex As Exception
            Return
        End Try
    End Sub

#End Region

End Class
'2017/12/11 saitou 広島信金(RSV2標準) ADD ------------------------------------------------------------ END
