Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports CASTCommon
Imports Microsoft.VisualBasic
Imports System.Diagnostics

' WEB伝送連携処理
Public Class ClsDensouRenkei

    ' パブリックＤＢ
    Private MainDB As CASTCommon.MyOracle

    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***
    ' サブＤＢ
    Private SubDB As CASTCommon.MyOracle
    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***

    ' メッセージ
    Public Message As String = ""

    Public JobTuuban As Integer = 0

    Private FilePath As String = ""
    Private FileName As String = ""

    Private clsFUSION As New clsFUSION.clsMain
    Public S_DATFileArray As New System.Collections.Specialized.StringCollection

    Private MOJICode As Encoding

    'ファイル名
    Private RENKEI_FILENAME As String = ""

    '一時ファイル
    Private TmpFileName As String = ""

    Private DelFlg As Integer = 0

    Private Koufuri_kbn As Integer = 1      '1：口振　3：総振

    Private File_Byte As Integer = 120       '120バイト

    'マルチ区分
    Private Multi_Kbn As String = "0"

    '2012/12/05 saitou WEB伝送 UPD -------------------------------------------------->>>>
    Protected Friend END_KEN As String = String.Empty       'ENDファイル内の件数(パラメータより)
    Protected Friend END_KIN As String = String.Empty       'ENDファイル内の金額(パラメータより)
    '2012/12/05 saitou WEB伝送 UPD --------------------------------------------------<<<<

    ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Friend Structure INI_PARAM
        Dim RSV2_EDITION As String                          ' RSV2機能設定
    End Structure
    Private Ini_Info As INI_PARAM

    Private TimeStamp As String
    ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

    ' 機能　 ： メイン
    '
    ' 戻り値 ： 0 - 正常 ， 0以外 - 異常
    '
    ' 備考　 ： 
    Public Function Main(ByVal command As String) As Integer

        MainLOG.ToriCode = "0000000000-00"
        MainLOG.FuriDate = "00000000"
        MainLOG.UserID = "Densou"

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***
        Dim dblock As CASTCommon.CDBLock = Nothing

        Dim LockWaitTime As Integer = 30
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 30
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***


        Try
            FilePath = Path.GetDirectoryName(command)
            FileName = Path.GetFileName(command)

            ' オラクル
            MainDB = New CASTCommon.MyOracle
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***
            SubDB = New CASTCommon.MyOracle
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***

            'フォルダ名が無ければWEB伝送フォルダとする
            If FilePath = "" Then
                RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), FileName)
            Else
                RENKEI_FILENAME = Path.Combine(FilePath, FileName)
            End If

            'パラメータ保存用
            Dim bRet As Boolean

            ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
            If GetIniInfo() = False Then
                Message = "設定ファイル取得失敗 ログ参照"
                bRet = False
            End If

            TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

            If File.Exists(RENKEI_FILENAME) = False Then
                MainLOG.Write("WEB伝送ファイル取得", "失敗", "WEB伝送データファイルなし：" & RENKEI_FILENAME)
                Message = "WEB伝送データファイルなし：" & RENKEI_FILENAME

                bRet = False
            Else
                'ファイル分割

                If fn_FileSplit(RENKEI_FILENAME) = False Then
                    bRet = False
                Else

                    For i As Integer = 0 To S_DATFileArray.Count - 1
                        Dim JobParam As String = ""

                        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***
                        ' トラン開始
                        SubDB.BeginTrans()

                        ' WEB_RIREKIMAST登録ロック
                        dblock = New CASTCommon.CDBLock
                        If dblock.InsertWEB_RIREKIMAST_Lock(MainDB, LockWaitTime) = False Then
                            ' ロールバック
                            SubDB.Rollback()
                            MainLOG.Write_Err("WEB_RIREKIMAST登録", "失敗", "InsertWEB_RIREKIMAST処理でタイムアウト")
                            Message = "InsertWEB_RIREKIMAST処理でタイムアウト"
                            bRet = False
                            Exit For
                        End If
                        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***

                        bRet = TourokuFile(S_DATFileArray.Item(i), JobParam)

                        If bRet = True Then

                            bRet = InsertJobMast(JobParam)

                        End If

                        If bRet = False Then
                            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***
                            ' WEB_RIREKIMAST登録アンロック
                            dblock.InsertWEB_RIREKIMAST_UnLock(MainDB)

                            ' ロールバック
                            SubDB.Rollback()
                            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***

                            For j As Integer = 0 To S_DATFileArray.Count - 1
                                File.Delete(S_DATFileArray.Item(j))
                            Next

                            Exit For
                        End If

                        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***
                        ' WEB_RIREKIMAST登録アンロック
                        dblock.InsertWEB_RIREKIMAST_UnLock(MainDB)

                        ' コミット
                        SubDB.Commit()
                        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***
                    Next
                End If


                '一時ファイルを削除
                Try
                    If File.Exists(TmpFileName) = True Then
                        File.Delete(TmpFileName)
                    End If

                Catch ex As Exception
                    MainLOG.Write("一時ファイル削除", "失敗", TmpFileName & "　" & ex.Message)
                End Try

            End If

            'エラーがある場合
            If bRet = False Then
                'ジョブ監視に登録する
                If JobTuuban = 0 Then
                    InsertJOBMASTbyError(Message)
                Else
                    MainLOG.UpdateJOBMASTbyErr(Message)
                End If
                'ロールバック
                MainDB.Rollback()


                Return 2
            End If

            '正常終了時コミット
            If JobTuuban > 0 Then
                MainLOG.UpdateJOBMASTbyOK(Message)
            End If
            MainDB.Commit()

            'ファイルをWEB_REV_BKへ移動
            If S_DATFileArray.Count <> 1 Then

                Dim DestFile As String = ""
                Try

                    DestFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV_BK"), FileName)

                    '前回ファイルを削除
                    If File.Exists(DestFile) = True Then
                        File.Delete(DestFile)
                    End If

                    File.Move(RENKEI_FILENAME, DestFile)

                Catch ex As Exception
                    MainLOG.Write("正常フォルダ移動", "失敗", RENKEI_FILENAME & " -> " & DestFile)
                    
                End Try

                Return 0
                'フラグが1の場合、シングルファイルをリネームしたとみなす
            ElseIf S_DATFileArray.Count = 1 And DelFlg = 1 Then
                Dim DestFile As String = ""
                Try

                    DestFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV_BK"), FileName)

                    '前回ファイルを削除
                    If File.Exists(DestFile) = True Then
                        File.Delete(DestFile)
                    End If

                    File.Move(RENKEI_FILENAME, DestFile)

                Catch ex As Exception
                    MainLOG.Write("正常フォルダ移動", "失敗", RENKEI_FILENAME & " -> " & DestFile)
                    
                End Try

                Return 0
            Else
                Return 0
            End If

        Catch
            Throw
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***
            If Not SubDB Is Nothing Then
                ' WEB_RIREKIMAST登録アンロック
                dblock.InsertWEB_RIREKIMAST_UnLock(MainDB)

                SubDB.Rollback()
                SubDB.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（WEB_RIREKIMAST登録時の一意性保証） ***

            If Not MainDB Is Nothing Then MainDB.Close()
        End Try

        Return 0

    End Function

    Private Function TourokuFile(ByVal filename As String, ByRef jobparam As String) As Boolean
        Dim ParamFile As String = ""
        'エンドレコード追加
        If fn_AddEndRecord(filename) = False Then
            Return False
        End If

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraReader As MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        '委託者コード解析メイン処理
        Try
            Using fs As FileStream = New FileStream(filename, FileMode.Open, FileAccess.Read)
                Dim Rec(0) As Byte              'レコード区分格納用
                Dim Hed As Byte                 'ヘッダレコードのデータ区分
                Dim Enc As System.Text.Encoding 'エンコーディング

                'ファイル先頭1バイトを読み取りエンコード判定
                fs.Read(Rec, 0, 1)

                Select Case Rec(0)
                    Case 49 'SJIS.GetBytes("1"c)(0)に該当
                        Hed = 49
                        Enc = Encoding.GetEncoding("SHIFT-JIS")

                    Case 241 'EncdE.GetBytes("1"c)(0)に該当
                        Hed = 241
                        Enc = Encoding.GetEncoding("IBM290")

                    Case Else
                        MainLOG.Write("エンコード判定", "失敗", "ファイル名：" & filename)
                        Message = "エンコード判定失敗 ファイル名：" & filename
                        Return False
                End Select

                'ファイル先頭までシーク
                fs.Seek(0, SeekOrigin.Begin)

                'ヘッダレコード読込
                'Dim Header(119) As Byte
                Dim Header(121) As Byte
                'fs.Read(Header, 0, 120)
                fs.Read(Header, 0, File_Byte)

                Dim Syubetu As String = Enc.GetString(Header, 1, 2)     '種別
                Dim CodeKbn As String = Enc.GetString(Header, 3, 1)     'コード区分
                Dim ItakuCode As String = Enc.GetString(Header, 4, 10)  '委託者コード
                Dim FuriDate As String = Enc.GetString(Header, 54, 4)   '振込日
                Dim KinCode As String = Enc.GetString(Header, 58, 4)      '金融機関コード
                Dim SitCode As String = Enc.GetString(Header, 77, 3)      '支店コード
                Dim Kamoku As String = Enc.GetString(Header, 95, 1)       '科目
                Dim Kouza As String = Enc.GetString(Header, 96, 7)        '口座番号

                Dim CheckFlg As String = GetFSKJIni("WEB_DEN", "HED_CHECK")     'ヘッダの店/科目/口座を0:チェックする,1:チェックしない

                If Syubetu = "91" Then
                    Koufuri_kbn = 1
                Else
                    Koufuri_kbn = 3
                End If

                'ヘッダ情報チェック
                '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                'Dim OraReader As MyOracleReader = New MyOracleReader(MainDB)
                OraReader = New MyOracleReader(MainDB)
                '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                Dim SQL As StringBuilder = New StringBuilder

                '*** Str Del 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                'OraReader = New CASTCommon.MyOracleReader
                '*** End Del 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                SQL = New StringBuilder(128)
                SQL.AppendLine("SELECT")
                SQL.AppendLine(" TORIS_CODE_T")
                SQL.AppendLine(",TORIF_CODE_T")
                SQL.AppendLine(",BAITAI_CODE_T")
                SQL.AppendLine(",FMT_KBN_T")
                SQL.AppendLine(",CODE_KBN_T")
                SQL.AppendLine(",LABEL_KBN_T")
                SQL.AppendLine(",FILE_NAME_T")
                SQL.AppendLine(",MULTI_KBN_T")
                SQL.AppendLine(",ITAKU_KANRI_CODE_T")
                SQL.AppendLine(",SFURI_FLG_T")
                SQL.AppendLine(" FROM TORIMAST , SCHMAST ")
                '条件追加 start
                SQL.AppendLine(" WHERE TORIS_CODE_T = TORIS_CODE_S")
                SQL.AppendLine(" AND TORIF_CODE_T = TORIF_CODE_S")
                SQL.AppendLine(" AND BAITAI_CODE_T = '10'  ")
                SQL.AppendLine(" AND FURI_DATE_S = '" & ConvertDate(FuriDate, "yyyyMMdd") & "'")
                'end
                SQL.AppendLine(" AND ITAKU_CODE_T = '" & ItakuCode & "'")
                SQL.AppendLine(" AND SYUBETU_T = '" & Syubetu & "'")
                SQL.AppendLine(" ORDER BY SFURI_FLG_T DESC")

                If OraReader.DataReader(SQL) Then

                    If OraReader.GetString("SFURI_FLG_T") <> "1" Then

                        'パラメータ組み立て
                        Dim param As StringBuilder = New StringBuilder
                        param.Append(OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T"))
                        param.Append("," & ConvertDate(FuriDate, "yyyyMMdd"))
                        param.Append("," & OraReader.GetString("CODE_KBN_T"))
                        param.Append("," & OraReader.GetString("FMT_KBN_T"))
                        param.Append("," & OraReader.GetString("BAITAI_CODE_T"))
                        param.Append("," & OraReader.GetString("LABEL_KBN_T"))
                        '落とし込みファイルを作る
                        ' 2015/12/28 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                        'If OraReader.GetString("FILE_NAME_T") = "" Then
                        '    If OraReader.GetString("MULTI_KBN_T") = "0" Then
                        '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                        '                                 "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                        '    Else
                        '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                        '                                 "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                        '    End If
                        'Else
                        '    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), OraReader.GetString("FILE_NAME_T"))
                        'End If
                        '=========================================================
                        ' ファイル名設定（再振なし）
                        '=========================================================
                        Select Case Ini_Info.RSV2_EDITION
                            Case "2"
                                '-------------------------------------------------
                                ' 大規模設定構築
                                '-------------------------------------------------
                                If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "J_WEB_S_" & _
                                                             OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & "_" & _
                                                             OraReader.GetString("FMT_KBN_T") & "_" & _
                                                             ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                             TimeStamp & "_" & _
                                                             Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                             "000" & _
                                                             ".DAT")
                                Else
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "J_WEB_M_" & _
                                                             OraReader.GetString("ITAKU_KANRI_CODE_T") & "00" & "_" & _
                                                             OraReader.GetString("FMT_KBN_T") & "_" & _
                                                             ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                             TimeStamp & "_" & _
                                                             Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                             "000" & _
                                                             ".DAT")
                                End If
                            Case Else
                                '-------------------------------------------------
                                ' 標準設定構築
                                '-------------------------------------------------
                                If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                                Else
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                                End If
                        End Select
                        ' 2015/12/28 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

                        'フラグが1の場合、シングルファイルをリネームしたとみなす
                        If RENKEI_FILENAME.ToUpper <> ParamFile.ToUpper Then
                            DelFlg = 1
                        End If

                        'WEB伝送履歴マスタ追加
                        InsertWEB_RIREKIMAST(OraReader.GetString("TORIS_CODE_T"), OraReader.GetString("TORIF_CODE_T"), ConvertDate(FuriDate, "yyyyMMdd"), OraReader.GetString("ITAKU_KANRI_CODE_T"))

                        jobparam = param.ToString
                        OraReader.Close()
                    Else
                        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                        OraReader.Close()
                        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                        '再振有の場合は、同じ委託者コードが複数存在するため、スケジュール検索
                        '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                        'OraReader = New CASTCommon.MyOracleReader
                        OraReader = New CASTCommon.MyOracleReader(MainDB)
                        '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                        SQL = New StringBuilder(128)
                        SQL.AppendLine("SELECT")
                        SQL.AppendLine(" TORIS_CODE_T")
                        SQL.AppendLine(",TORIF_CODE_T")
                        SQL.AppendLine(",BAITAI_CODE_T")
                        SQL.AppendLine(",FMT_KBN_T")
                        SQL.AppendLine(",CODE_KBN_T")
                        SQL.AppendLine(",LABEL_KBN_T")
                        SQL.AppendLine(",FILE_NAME_T")
                        SQL.AppendLine(",MULTI_KBN_T")
                        SQL.AppendLine(",ITAKU_KANRI_CODE_T")
                        SQL.AppendLine(" FROM TORIMAST,SCHMAST")
                        SQL.AppendLine(" WHERE ITAKU_CODE_T = '" & ItakuCode & "'")
                        SQL.AppendLine(" AND SYUBETU_T = '" & Syubetu & "'")
                        SQL.AppendLine(" AND FURI_DATE_S = '" & ConvertDate(FuriDate, "yyyyMMdd") & "'")
                        SQL.AppendLine(" AND TORIS_CODE_T = TORIS_CODE_S")
                        SQL.AppendLine(" AND TORIF_CODE_T = TORIF_CODE_S")
                        '2014/05/01 saitou 標準修正 ADD -------------------------------------------------->>>>
                        '条件追加
                        SQL.AppendLine(" AND BAITAI_CODE_T = '10'")
                        '2014/05/01 saitou 標準修正 ADD --------------------------------------------------<<<<

                        If OraReader.DataReader(SQL) Then
                            'パラメータ組み立て
                            Dim param As StringBuilder = New StringBuilder
                            param.Append(OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T"))
                            param.Append("," & ConvertDate(FuriDate, "yyyyMMdd"))
                            param.Append("," & OraReader.GetString("CODE_KBN_T"))
                            param.Append("," & OraReader.GetString("FMT_KBN_T"))
                            param.Append("," & OraReader.GetString("BAITAI_CODE_T"))
                            param.Append("," & OraReader.GetString("LABEL_KBN_T"))
                            '落とし込みファイルを作る
                            ' 2015/12/28 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                            'If OraReader.GetString("FILE_NAME_T") = "" Then
                            '    If OraReader.GetString("MULTI_KBN_T") = "0" Then
                            '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                            '                                 "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                            '    Else
                            '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                            '                                 "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                            '    End If
                            'Else
                            '    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), OraReader.GetString("FILE_NAME_T"))
                            'End If
                            '=========================================================
                            ' ファイル名設定（再振あり）
                            '=========================================================
                            Select Case Ini_Info.RSV2_EDITION
                                Case "2"
                                    '-------------------------------------------------
                                    ' 大規模設定構築
                                    '-------------------------------------------------
                                    If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                                 "J_WEB_S_" & _
                                                                 OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & "_" & _
                                                                 OraReader.GetString("FMT_KBN_T") & "_" & _
                                                                 ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                                 TimeStamp & "_" & _
                                                                 Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                                 "000" & _
                                                                 ".DAT")
                                    Else
                                        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                                 "J_WEB_M_" & _
                                                                 OraReader.GetString("ITAKU_KANRI_CODE_T") & "00" & "_" & _
                                                                 OraReader.GetString("FMT_KBN_T") & "_" & _
                                                                 ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                                 TimeStamp & "_" & _
                                                                 Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                                 "000" & _
                                                                 ".DAT")
                                    End If
                                Case Else
                                    '-------------------------------------------------
                                    ' 標準設定構築
                                    '-------------------------------------------------
                                    If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                                 "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                                    Else
                                        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                                 "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                                    End If
                            End Select
                            ' 2015/12/28 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

                            'フラグが1の場合、シングルファイルをリネームしたとみなす
                            If RENKEI_FILENAME.ToUpper <> ParamFile.ToUpper Then
                                DelFlg = 1
                            End If

                            'WEB伝送履歴マスタ追加
                            InsertWEB_RIREKIMAST(OraReader.GetString("TORIS_CODE_T"), OraReader.GetString("TORIF_CODE_T"), ConvertDate(FuriDate, "yyyyMMdd"), OraReader.GetString("ITAKU_KANRI_CODE_T"))

                            jobparam = param.ToString
                            OraReader.Close()
                        Else
                            Message = "スケジュール情報取得失敗 委託者コード：" & ItakuCode & " 種別コード：" & Syubetu & " ファイル名：" & RENKEI_FILENAME & " 振替日:" & FuriDate
                            MainLOG.Write("スケジュール情報取得", "失敗", "スケジュール情報取得失敗 委託者コード：" & ItakuCode & " 種別コード：" & Syubetu & " ファイル名：" & RENKEI_FILENAME & " 振替日:" & FuriDate)
                            
                            OraReader.Close()

                            Return False
                        End If
                    End If

                ElseIf GetFSKJIni("OPTION", "SOUFURI") = "1" Then   '総振オプションを使用しているか？
                    '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                    OraReader.Close()
                    '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

                    '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                    'OraReader = New CASTCommon.MyOracleReader
                    OraReader = New CASTCommon.MyOracleReader(MainDB)
                    '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                    SQL = New StringBuilder(128)
                    '口振でない場合、総振側で確認
                    SQL.AppendLine("SELECT")
                    SQL.AppendLine(" TORIS_CODE_T")
                    SQL.AppendLine(",TORIF_CODE_T")
                    SQL.AppendLine(",BAITAI_CODE_T")
                    SQL.AppendLine(",FMT_KBN_T")
                    SQL.AppendLine(",CODE_KBN_T")
                    SQL.AppendLine(",LABEL_KBN_T")
                    SQL.AppendLine(",FILE_NAME_T")
                    SQL.AppendLine(",MULTI_KBN_T")
                    SQL.AppendLine(",ITAKU_KANRI_CODE_T")
                    SQL.AppendLine(" FROM S_TORIMAST")
                    SQL.AppendLine(" WHERE ITAKU_CODE_T = '" & ItakuCode & "'")
                    SQL.AppendLine(" AND SYUBETU_T = '" & Syubetu & "'")
                    '2014/05/01 saitou 標準修正 ADD -------------------------------------------------->>>>
                    '条件追加
                    SQL.AppendLine(" AND BAITAI_CODE_T = '10'")
                    '2014/05/01 saitou 標準修正 ADD --------------------------------------------------<<<<
                    If CheckFlg = "1" Then
                        SQL.AppendLine(" AND TKIN_NO_T = '" & KinCode & "'")
                        SQL.AppendLine(" AND TSIT_NO_T = '" & SitCode & "'")
                        SQL.AppendLine(" AND KAMOKU_T = '" & CASTCommon.ConvertKamoku1TO2(Kamoku) & "'")
                        SQL.AppendLine(" AND KOUZA_T = '" & Kouza & "'")
                    End If
                    SQL.AppendLine(" AND ROWNUM = 1")

                    If OraReader.DataReader(SQL) Then
                        'パラメータ組み立て
                        Dim param As StringBuilder = New StringBuilder
                        param.Append(OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T"))
                        param.Append("," & ConvertDate(FuriDate, "yyyyMMdd"))
                        param.Append("," & OraReader.GetString("CODE_KBN_T"))
                        param.Append("," & OraReader.GetString("FMT_KBN_T"))
                        param.Append("," & OraReader.GetString("BAITAI_CODE_T"))
                        param.Append("," & OraReader.GetString("LABEL_KBN_T"))
                        '落とし込みファイルを作る
                        ' 2015/12/28 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                        'If OraReader.GetString("FILE_NAME_T") = "" Then
                        '    If OraReader.GetString("MULTI_KBN_T") = "0" Then
                        '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                        '                                 "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                        '    Else
                        '        ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                        '                                 "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                        '    End If
                        'Else
                        '    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), OraReader.GetString("FILE_NAME_T"))
                        'End If
                        '=========================================================
                        ' ファイル名設定（総合振込）
                        '=========================================================
                        Select Case Ini_Info.RSV2_EDITION
                            Case "2"
                                '-------------------------------------------------
                                ' 大規模設定構築
                                '-------------------------------------------------
                                If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "S_WEB_S_" & _
                                                             OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & "_" & _
                                                             OraReader.GetString("FMT_KBN_T") & "_" & _
                                                             ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                             TimeStamp & "_" & _
                                                             Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                             "000" & _
                                                             ".DAT")
                                Else
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "S_WEB_M_" & _
                                                             OraReader.GetString("ITAKU_KANRI_CODE_T") & "00" & "_" & _
                                                             OraReader.GetString("FMT_KBN_T") & "_" & _
                                                             ConvertDate(FuriDate, "yyyyMMdd") & "_" & _
                                                             TimeStamp & "_" & _
                                                             Format(Process.GetCurrentProcess.Id, "0000") & "_" & _
                                                             "000" & _
                                                             ".DAT")
                                End If
                            Case Else
                                '-------------------------------------------------
                                ' 標準設定構築
                                '-------------------------------------------------
                                If OraReader.GetString("MULTI_KBN_T") = "0" Then
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "D" & OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T") & ".DAT")
                                Else
                                    ParamFile = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), _
                                                             "D" & OraReader.GetString("ITAKU_KANRI_CODE_T") & ".DAT")
                                End If
                        End Select
                        ' 2015/12/28 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

                        'フラグが1の場合、シングルファイルをリネームしたとみなす
                        If RENKEI_FILENAME.ToUpper <> ParamFile.ToUpper Then
                            DelFlg = 1
                        End If

                        'WEB伝送履歴マスタ追加
                        InsertWEB_RIREKIMAST(OraReader.GetString("TORIS_CODE_T"), OraReader.GetString("TORIF_CODE_T"), ConvertDate(FuriDate, "yyyyMMdd"), OraReader.GetString("ITAKU_KANRI_CODE_T"))

                        jobparam = param.ToString
                        OraReader.Close()

                    Else
                        ' 取引先情報取得失敗

                        Message = "取引先情報取得失敗 委託者コード：" & ItakuCode & " 種別コード：" & Syubetu & " ファイル名：" & RENKEI_FILENAME
                        MainLOG.Write("取引先情報取得", "失敗", "取引先情報取得失敗 委託者コード：" & ItakuCode & " 種別コード：" & Syubetu & " ファイル名：" & RENKEI_FILENAME)
                        OraReader.Close()

                        Return False
                    End If
                Else
                    ' 取引先情報取得失敗

                    Message = "取引先情報取得失敗 委託者コード：" & ItakuCode & " 種別コード：" & Syubetu & " ファイル名：" & RENKEI_FILENAME
                    MainLOG.Write("取引先情報取得", "失敗", "取引先情報取得失敗 委託者コード：" & ItakuCode & " 種別コード：" & Syubetu & " ファイル名：" & RENKEI_FILENAME)
                    
                    OraReader.Close()

                    Return False
                End If
            End Using
            '同一委託者の落込完了するまで待機する
            If DelFlg <> 0 Then
                For cnt As Integer = 1 To 300 Step 1
                    Try
                        If File.Exists(ParamFile) = True Then
                            If cnt = 300 Then
                                Message = "二重ﾌｧｲﾙ送信ｴﾗｰ:" & ParamFile
                                MainLOG.Write("ファイルコピー", "失敗", "ファイル名：" & ParamFile)
                                Return False
                            End If
                        Else
                            File.Copy(filename, ParamFile, True)
                            '一時ファイル消す
                            File.Delete(filename)
                            Exit For
                        End If
                    Catch
                    End Try
                    System.Threading.Thread.Sleep(100)
                Next
            Else
                File.Copy(filename, ParamFile, True)
                '一時ファイル消す
                File.Delete(filename)
            End If

        Catch ex As Exception
            MainLOG.Write("取引先情報取得", "失敗", "ファイル名：" & RENKEI_FILENAME & " " & ex.Message)
            
            Message = "取引先情報取得失敗 ファイル名：" & RENKEI_FILENAME
            Return False

            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        End Try

        Return True
    End Function

    Private Function InsertJobMast(ByVal param As String) As Boolean
        '------------------------------------------------
        'ジョブマスタに登録
        '------------------------------------------------
        Try

            System.Threading.Thread.Sleep(500)

            If Koufuri_kbn <> 1 Then
                '個別落込(総振)
                '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                'If MainLOG.SearchJOBMAST("S010", param, MainDB) = -1 Then
                If MainLOG.SearchJOBMAST("S010", param, SubDB) = -1 Then
                '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                    Message = "対象となるジョブが登録済みです"
                    Return False
                End If

                System.Threading.Thread.Sleep(500)

                '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                'If MainLOG.InsertJOBMAST("S010", MainLOG.UserID, param, MainDB) = False Then
                If MainLOG.InsertJOBMAST("S010", MainLOG.UserID, param, SubDB) = False Then
                '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                    Message = "ジョブマスタの登録に失敗しました"
                    Return False
                End If
            Else
                '個別落込(口振)

                '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                'If MainLOG.SearchJOBMAST("J010", param, MainDB) = -1 Then
                If MainLOG.SearchJOBMAST("J010", param, SubDB) = -1 Then
                '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                    Message = "対象となるジョブが登録済みです"
                    Return False
                End If

                System.Threading.Thread.Sleep(500)

                '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                'If MainLOG.InsertJOBMAST("J010", MainLOG.UserID, param, MainDB) = False Then
                If MainLOG.InsertJOBMAST("J010", MainLOG.UserID, param, SubDB) = False Then
                '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
                    Message = "ジョブマスタの登録に失敗しました"
                    Return False
                End If
                MainLOG.Write("f", "")
            End If

        Catch
            Throw
        End Try
        MainLOG.Write("ジョブ登録", "成功", "パラメータ：" & param)
        Return True
    End Function

    Public Function InsertJOBMASTbyError(ByVal Message As String) As Boolean
        Dim SQL As New StringBuilder(128)

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
        Dim DB As CASTCommon.MyOracle = Nothing
        Dim dblock As CASTCommon.CDBLock = New CASTCommon.CDBLock

        Dim LockWaitTime As Integer = 30
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME4")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 30
            End If
        End If

        Try
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

            SQL.Append("INSERT INTO JOBMAST(")
            SQL.Append(" TOUROKU_DATE_J")
            SQL.Append(",TOUROKU_TIME_J")
            SQL.Append(",STA_DATE_J")
            SQL.Append(",STA_TIME_J")
            SQL.Append(",END_DATE_J")
            SQL.Append(",END_TIME_J")
            SQL.Append(",JOBID_J")
            SQL.Append(",STATUS_J")
            SQL.Append(",USERID_J")
            SQL.Append(",PARAMETA_J")
            SQL.Append(",ERRMSG_J")
            SQL.Append(") VALUES (")
            SQL.Append(" TO_CHAR(SYSDATE,'YYYYMMDD')")                          ' TOUROKU_DATE_J
            SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' TOUROKU_TIME_J
            SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' STA_DATE_J
            SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' STA_TIME_J
            SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' END_DATE_J
            SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' END_TIME_J
            SQL.Append(",'W010'")                                               ' JOBID_J
            SQL.Append(",'7'")          ' 異常終了                              ' STATUS_J
            SQL.Append("," & SQ(MainLOG.UserID))                                    ' USERID_J
            SQL.Append("," & SQ(Path.GetFileName(RENKEI_FILENAME)))              ' PARAMETA_J
            SQL.Append("," & SQ(Message))                                       ' ERRMSG_J
            SQL.Append(")")

            '*** Str Chg 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
            'Dim DB As New CASTCommon.MyOracle
            DB = New CASTCommon.MyOracle

            ' ジョブ登録ロック
            If dblock.InsertJOBMAST_Lock(DB, LockWaitTime) = False Then
                MainLog.Write_Err("ジョブマスタ登録", "失敗", "タイムアウト")

                Dim ELog As New CASTCommon.ClsEventLOG
                ELog.Write("WEB伝送連携　失敗　" & Message, Diagnostics.EventLogEntryType.Error)
                Return False
            End If
            '*** End Chg 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

            If DB.ExecuteNonQuery(SQL) <= 0 Then
                Dim ELog As New CASTCommon.ClsEventLOG
                ELog.Write("WEB伝送連携　失敗　" & Message, Diagnostics.EventLogEntryType.Error)
                Return False
            End If

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
            ' ジョブ登録アンロック
            dblock.InsertJOBMAST_UnLock(DB)
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

            DB.Commit()
            DB.Close()

            Return True

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***
        Finally
            If Not DB Is Nothing Then
                ' ジョブ登録アンロック
                dblock.InsertJOBMAST_UnLock(DB)

                DB.Rollback()
                DB.Close()
            End If
        End Try
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応（ジョブ登録時の一意性保証） ***

    End Function

    Public Function InsertWEB_RIREKIMAST(ByVal TORIS_CODE As String, ByVal TORIF_CODE As String, ByVal FURIDATE As String, ByVal ITAKU_KANRI_CODE As String) As Boolean
        Dim SQL As New StringBuilder(128)
        '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
        'Dim OraReader As MyOracleReader = New MyOracleReader(MainDB)
        Dim OraReader As MyOracleReader = New MyOracleReader(SubDB)
        '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
        Dim SEQ As Integer

        If Strings.Right(FileName, 4).ToUpper = ".DAT" Then
            'ファイル名が"*.DAT*"の場合、一度、リネーム処理が終わったとみなして、インサート処理はしない
            Return False
        End If

        'FileArray(0) = ユーザ名
        'FileArray(1) = 元ファイル名
        'FileArray(2) = 配信日
        'FileArray(3) = 配信時間
        Dim FileArray() As String = FileName.Split("_"c)

        SQL.Append(" SELECT SEQ_NO_W ")
        SQL.Append(" FROM WEB_RIREKIMAST ")
        SQL.Append(" WHERE TORIS_CODE_W = '" & TORIS_CODE & "'")
        SQL.Append(" AND TORIF_CODE_W = '" & TORIF_CODE & "'")
        SQL.Append(" AND FURI_DATE_W = '" & FURIDATE & "'")
        SQL.Append(" ORDER BY SEQ_NO_W DESC")

        '複数回持ち込み確認
        If OraReader.DataReader(SQL) Then
            SEQ = OraReader.GetInt("SEQ_NO_W") + 1
        Else
            SEQ = 1
        End If

        OraReader.Close()

        SQL = New StringBuilder(128)
        SQL.Append("INSERT INTO WEB_RIREKIMAST(")
        SQL.Append(" FSYORI_KBN_W")
        SQL.Append(",TORIS_CODE_W")
        SQL.Append(",TORIF_CODE_W")
        SQL.Append(",FURI_DATE_W")
        SQL.Append(",SEQ_NO_W")
        SQL.Append(",ITAKU_KANRI_CODE_W")
        SQL.Append(",USER_ID_W")
        SQL.Append(",FILE_NAME_W")
        SQL.Append(",SAKUSEI_DATE_W")
        SQL.Append(",SAKUSEI_TIME_W")
        SQL.Append(",STATUS_KBN_W")
        '2012/12/05 saitou WEB伝送 UPD -------------------------------------------------->>>>
        '件数、金額、予備項目追加
        SQL.Append(",END_KEN_W")
        SQL.Append(",END_KIN_W")
        SQL.Append(",YOBI1_W")
        SQL.Append(",YOBI2_W")
        SQL.Append(",YOBI3_W")
        SQL.Append(",YOBI4_W")
        SQL.Append(",YOBI5_W")
        SQL.Append(",YOBI6_W")
        SQL.Append(",YOBI7_W")
        SQL.Append(",YOBI8_W")
        SQL.Append(",YOBI9_W")
        SQL.Append(",YOBI10_W")
        '2012/12/05 saitou WEB伝送 UPD --------------------------------------------------<<<<
        SQL.Append(") VALUES (")
        SQL.Append("'" & Koufuri_kbn & "'")                                 ' FSYORI_KBN_W
        SQL.Append("," & SQ(TORIS_CODE))                                    ' TORIS_CODE_W
        SQL.Append("," & SQ(TORIF_CODE))                                    ' TORIF_CODE_W
        SQL.Append("," & SQ(FURIDATE))                                      ' FURI_DATE_W
        SQL.Append("," & SQ(SEQ))                                           ' SEQ_NO_W
        SQL.Append("," & SQ(ITAKU_KANRI_CODE))                              ' ITAKU_KANRI_CODE_W
        SQL.Append("," & SQ(FileArray(0)))                                  ' USER_ID_W
        SQL.Append("," & SQ(FileArray(1)))                                  ' FILE_NAME_W
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))  ' SAKUSEI_DATE_W
        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("HHmmss")))    ' SAKUSEI_TIME_W
        SQL.Append("," & SQ("0"))                                           ' STATUS_KBN_W (0:受付済)
        '2012/12/05 saitou WEB伝送 UPD -------------------------------------------------->>>>
        '件数、金額、予備項目追加
        SQL.Append("," & CInt(END_KEN))                                     ' END_KEN_W
        SQL.Append("," & CInt(END_KIN))                                     ' END_KIN_W
        SQL.Append("," & SQ(""))                                            ' YOBI1_W
        SQL.Append("," & SQ(""))                                            ' YOBI2_W
        SQL.Append("," & SQ(""))                                            ' YOBI3_W
        SQL.Append("," & SQ(""))                                            ' YOBI4_W
        SQL.Append("," & SQ(""))                                            ' YOBI5_W
        SQL.Append("," & SQ(""))                                            ' YOBI6_W
        SQL.Append("," & SQ(""))                                            ' YOBI7_W
        SQL.Append("," & SQ(""))                                            ' YOBI8_W
        SQL.Append("," & SQ(""))                                            ' YOBI9_W
        SQL.Append("," & SQ(""))                                            ' YOBI10_W
        '2012/12/05 saitou WEB伝送 UPD --------------------------------------------------<<<<
        SQL.Append(")")
        '*** Str Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***
        'Dim DB As New CASTCommon.MyOracle
        'If DB.ExecuteNonQuery(SQL) <= 0 Then
        '    Dim ELog As New CASTCommon.ClsEventLOG
        '    ELog.Write("WEB伝送連携　失敗　" & Message, Diagnostics.EventLogEntryType.Error)
        '    Return False
        'End If
        'DB.Commit()
        'DB.Close()
        If SubDB.ExecuteNonQuery(SQL) <= 0 Then
            Dim ELog As New CASTCommon.ClsEventLOG
            ELog.Write("WEB伝送連携　失敗　" & Message, Diagnostics.EventLogEntryType.Error)
            Return False
        End If
        '*** End Upd 2015/12/01 SO)荒木 for 多重実行対応（DBコネクションの一本化） ***

        Return True
    End Function

    Private Function fn_FileSplit(ByVal FileName As String) As Boolean

        Dim SFileInfo As New clsSplitFileInfo.SplitFilePara

        Dim fs As FileStream = Nothing
        Dim br As BinaryReader = Nothing

        '読み込んだファイルレコード
        Dim key1 As String = ""

        '振込日
        Dim FuriDate As String = ""
        Dim FuriDate_bk As String = ""

        '委託者コード
        Dim ItakuCode As String = ""
        Dim ItakuCode_bk As String = ""

        '読込ファイルの位置情報
        Dim _cnt As Long = 0
        Dim _cntbk As Long = 0

        'レコード区分(エンド)
        Dim E_RecKbn As String = ""

        '分割フラグ
        Dim flgSplit As Boolean = False

        Dim OraReader As MyOracleReader = New MyOracleReader(MainDB)
        Dim SQL As StringBuilder = New StringBuilder

        Try
            '中間ファイル名を設定
            TmpFileName = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV_BK"), DateTime.Now.ToString("yyyyMMddHHmmssfff") & "_" & Path.GetFileName(FileName))
            File.Copy(FileName, TmpFileName, True)

            '*********************************************************************
            '文字コード判定
            '*********************************************************************
            Try
                fs = New FileStream(TmpFileName, FileMode.Open, FileAccess.Read)
                br = New BinaryReader(fs)

                '空ファイルの場合
                If fs.Length = 0 Then
                    MainLOG.Write("ファイルデータ読込", "失敗", "空ファイルのため処理出来ません。：" & FileName)
                    Message = "空ファイルのため処理出来ません：" & FileName
                    Return False
                End If

                br.BaseStream.Seek(0, SeekOrigin.Begin)
                br.BaseStream.Position = 0

                '先頭１バイトを読み込み文字コードを判定する
                Select Case br.ReadByte()
                    Case 49
                        '文字コードをJISに設定
                        MOJICode = Encoding.GetEncoding("SHIFT-JIS")

                    Case 241
                        '文字コードをEBCDICに設定
                        MOJICode = Encoding.GetEncoding("IBM290")
                    Case Else
                        MainLOG.Write("伝送ファイルコード判定", "失敗", "先頭文字コード異常：" & FileName)
                        
                        Message = "先頭文字コード異常：" & FileName
                        Return False
                End Select

            Catch ex As Exception
                MainLOG.Write("伝送ファイルコード判定", "失敗", "ファイル名：" & FileName & "　" & ex.Message)
                
                Message = "ファイル名：" & FileName & "　" & ex.Message
                Return False
            End Try

            '*********************************************************************
            'ファイル分割
            '*********************************************************************
            Dim num As Integer = 1
            Dim _FileLen As Long = fs.Length
            Dim TwoByte(1) As Byte

            _cnt = 0

            br.BaseStream.Seek(0, SeekOrigin.Begin)
            br.BaseStream.Position = 0

            If _FileLen Mod 120 = 0 Then
                br.BaseStream.Seek(120, SeekOrigin.Begin)
                TwoByte = br.ReadBytes(2)
                Select Case TwoByte(0)
                    Case &HD    '0D
                        Select Case TwoByte(1)
                            Case &HA, &H25  '0A,25
                                File_Byte = 122
                            Case Else
                                File_Byte = 121
                        End Select
                    Case &HA    '0A
                        File_Byte = 121
                    Case &HF1   'F1
                        File_Byte = 121
                    Case Else
                        File_Byte = 120
                End Select
            Else
                br.BaseStream.Seek(120, SeekOrigin.Begin)
                TwoByte = br.ReadBytes(2)
                Select Case TwoByte(0)
                    Case &HA    '0A
                        File_Byte = 121
                    Case Else
                        File_Byte = 122
                End Select
            End If

            br.BaseStream.Seek(0, SeekOrigin.Begin)
            br.BaseStream.Position = 0

            Do
                '=================================================================================
                'ファイル分割条件判定処理
                '=================================================================================

                'Dim bt_RecordData(119) As Byte
                Dim bt_RecordData(121) As Byte

                'ファイル末尾でなければ分割条件判定を行う
                If _cnt = _FileLen Then
                    flgSplit = True
                    '_cnt += 120
                    _cnt += File_Byte
                Else
                    '振込日情報取得
                    If _cnt = 0 Then
                        '1行目ヘッダー情報取得
                        br.BaseStream.Seek(0, SeekOrigin.Begin)
                        'bt_RecordData = br.ReadBytes(120)
                        bt_RecordData = br.ReadBytes(File_Byte)

                        If SFileInfo.fn_GetFirstRecord(bt_RecordData, MOJICode) = True Then
                            '委託者コード取得
                            ItakuCode_bk = SFileInfo.ZG04
                            '振込日取得
                            FuriDate_bk = SFileInfo.ZG06
                        End If
                    End If

                    'ファイルを120バイトづつ読み込む
                    br.BaseStream.Seek(_cnt, SeekOrigin.Begin)
                    'bt_RecordData = br.ReadBytes(120)
                    bt_RecordData = br.ReadBytes(File_Byte)

                    'ヘッダーレコードかどうか判定
                    If SFileInfo.fn_GetFirstRecord(bt_RecordData, MOJICode) = True Then

                        '委託者コード取得
                        ItakuCode = SFileInfo.ZG04
                        '振込日情報取得
                        FuriDate = SFileInfo.ZG06

                        'ヘッダ情報チェック

                        '*** Str Del 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                        'OraReader = New CASTCommon.MyOracleReader
                        '*** End Del 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
                        SQL = New StringBuilder(128)

                        SQL.Append(" SELECT MULTI_KBN_T ")
                        SQL.Append(" FROM TORIMAST ")
                        SQL.Append(" WHERE ITAKU_CODE_T = '" & ItakuCode & "'")
                        SQL.Append(" AND   SYUBETU_T = '" & SFileInfo.ZG02 & "'")

                        If OraReader.DataReader(SQL) Then
                            Multi_Kbn = OraReader.GetString("MULTI_KBN_T")
                        Else
                            '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在）＆ 多重実行対応（DBコネクションの一本化）***
                            'OraReader = New CASTCommon.MyOracleReader
                            OraReader.Close()
                            OraReader = New CASTCommon.MyOracleReader(MainDB)
                            '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在）＆ 多重実行対応（DBコネクションの一本化）***
                            SQL = New StringBuilder(128)

                            SQL.Append(" SELECT MULTI_KBN_T")
                            SQL.Append(" FROM S_TORIMAST")
                            SQL.Append(" WHERE ITAKU_CODE_T = '" & ItakuCode & "'")
                            SQL.Append(" AND SYUBETU_T = '" & SFileInfo.ZG02 & "'")

                            If OraReader.DataReader(SQL) Then
                                Multi_Kbn = OraReader.GetString("MULTI_KBN_T")
                            End If
                        End If

                        OraReader.Close()

                        '前レコードを読み込む
                        'If _cnt >= 120 Then
                        If _cnt >= File_Byte Then
                            'br.BaseStream.Seek(_cnt - 120, SeekOrigin.Begin)
                            br.BaseStream.Seek(_cnt - File_Byte, SeekOrigin.Begin)
                            E_RecKbn = MOJICode.GetString(br.ReadBytes(1))
                        End If

                        'ファイル分割条件設定
                        '以下の3条件に当てはまる場合は分割対象とする
                        '=================================================================
                        '1前レコードがエンドレコードまたはトレーラレコード
                        '2振込日不一致
                        '3委託者コード一致かつ、振込日一致
                        '=================================================================

                        If E_RecKbn <> Nothing And E_RecKbn <> "" Then
                            '条件1:前レコードがエンドレコード
                            If E_RecKbn.Substring(0, 1) = "9" Or E_RecKbn.Substring(0, 1) = "8" Then
                                'If E_RecKbn.Substring(0, 1) = "9" Then
                                flgSplit = True
                            End If
                        End If

                        If FuriDate <> FuriDate_bk Then
                            '条件2:振込日不一致
                            flgSplit = True
                        End If

                        If ItakuCode = ItakuCode_bk AndAlso FuriDate = FuriDate_bk Then
                            '条件3:委託者コード一致かつ、振込日一致
                            If _cnt <> 0 Then
                                flgSplit = True
                            End If
                        End If

                        '委託者コードを退避
                        ItakuCode_bk = ItakuCode
                        '振込日情報を退避
                        FuriDate_bk = FuriDate

                        '取引先情報がマルチファイルの場合、分割しない
                        If Multi_Kbn <> "0" Then
                            flgSplit = False
                        End If

                    End If
                End If

                '=================================================================================
                'ファイル分割処理
                '=================================================================================
                '分割フラグがtrueなら分割する
                If flgSplit = True Then

                    '分割ファイルをフォルダに書込み
                    Dim _FileName As String = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), Path.GetFileName(FileName) & "." & num.ToString("000"))
                    For i As Integer = num To 999
                        If File.Exists(_FileName) = False Then
                            Exit For
                        End If
                        'ファイルが存在する場合はカウントアップする
                        num += 1
                        _FileName = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), Path.GetFileName(FileName) & "." & num.ToString("000"))
                    Next
                   
                    num += 1

                    If _cnt <> 0 Then
                        Try
                            '分割ファイル書込み
                            br.BaseStream.Seek(_cntbk, SeekOrigin.Begin)

                            Dim sw As New StreamWriter(_FileName, True)
                            Dim bw As New BinaryWriter(sw.BaseStream)
                            bw.Write(br.ReadBytes(CInt(_cnt - _cntbk)))
                            sw.Close()
                            bw.Close()

                            '分割フラグをfalseに戻す
                            flgSplit = False

                        Catch ex As Exception
                            'MainLOG.Write("分割ファイル書込", "失敗", "ファイル名：" & _FileName & "　" & ex.Message)
                            Message = "ファイル名：" & _FileName & "　" & ex.Message
                            Return False
                        End Try

                        '分割ファイル名を配列に追加
                        S_DATFileArray.Add(_FileName)

                        _cntbk = _cnt
                    End If
                End If

                '_cnt += 120
                _cnt += File_Byte

            Loop Until _cnt > _FileLen

            Return True

        Catch ex As Exception
            MainLOG.Write("分割ファイル書込", "失敗", "ファイル名：" & FileName & "　" & ex.Message)
            
            Message = "ファイル名：" & FileName & "　" & ex.Message
            Return False
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

            If Not fs Is Nothing Then fs.Close()
            If Not br Is Nothing Then br.Close()
        End Try

    End Function

    ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Private Function GetIniInfo() As Boolean

        Try
            MainLOG.Write(MainLOG.UserID, MainLOG.ToriCode, MainLOG.FuriDate, "設定ファイル取得", "")

            '==================================================================
            '　RSV2機能設定
            '==================================================================
            Ini_Info.RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            If Ini_Info.RSV2_EDITION.ToUpper = "ERR" OrElse Ini_Info.RSV2_EDITION = Nothing Then
                MainLOG.Write(MainLOG.UserID, MainLOG.ToriCode, MainLOG.FuriDate, "設定ファイル取得", "失敗", "項目名:RSV2機能設定 分類:RSV2_V1.0.0 項目:EDITION")
                Return False
            End If

            Return True

        Catch ex As Exception
            MainLOG.Write(MainLOG.ToriCode, MainLOG.FuriDate, "設定ファイル取得","失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write(MainLOG.UserID, MainLOG.ToriCode, MainLOG.FuriDate, "設定ファイル取得", "")
        End Try

    End Function
    ' 2015/12/28 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

    Public Class clsSplitFileInfo
        '分割ファイル情報格納用構造体
        Structure SplitFilePara

            '***ヘッダレコード情報***
            <VBFixedString(1)> Public ZG01 As String    'データ区分(=1)
            <VBFixedString(2)> Public ZG02 As String    '種別コード
            <VBFixedString(1)> Public ZG03 As String    'コード区分
            <VBFixedString(10)> Public ZG04 As String   '振込依頼人コード
            <VBFixedString(40)> Public ZG05 As String   '振込依頼人名
            <VBFixedString(4)> Public ZG06 As String    '取扱日
            <VBFixedString(4)> Public ZG07 As String    '仕向銀行ｺｰﾄﾞ
            <VBFixedString(15)> Public ZG08 As String   '仕向銀行名
            <VBFixedString(3)> Public ZG09 As String    '仕向支店ｺｰﾄﾞ
            <VBFixedString(15)> Public ZG10 As String   '仕向支店名
            <VBFixedString(1)> Public ZG11 As String    '預金種目
            <VBFixedString(7)> Public ZG12 As String    '口座番号
            <VBFixedString(17)> Public ZG13 As String   'ダミー

            Public Sub init()
                ZG01 = ""
                ZG02 = ""
                ZG03 = ""
                ZG04 = ""
                ZG05 = ""
                ZG06 = ""
                ZG07 = ""
                ZG08 = ""
                ZG09 = ""
                ZG10 = ""
                ZG11 = ""
                ZG12 = ""
                ZG13 = ""
            End Sub

            Public Function fn_GetFirstRecord(ByVal recordbyte() As Byte, ByVal code As Encoding) As Boolean

                Try
                    '読み込んだ文字列が120バイト以下
                    If recordbyte.Length < 120 Then
                        Return False
                    End If

                    'データセット
                    ZG01 = code.GetString(recordbyte, 0, 1) 'データ区分(=1)
                    ZG02 = code.GetString(recordbyte, 1, 2) '種別コード
                    ZG03 = code.GetString(recordbyte, 3, 1)    'コード区分
                    ZG04 = code.GetString(recordbyte, 4, 10)   '振込依頼人コード
                    ZG05 = code.GetString(recordbyte, 14, 40)  '振込依頼人名
                    ZG06 = code.GetString(recordbyte, 54, 4)   '取扱日
                    ZG07 = code.GetString(recordbyte, 58, 4)   '仕向銀行ｺｰﾄﾞ
                    ZG08 = code.GetString(recordbyte, 62, 15) '仕向銀行名
                    ZG09 = code.GetString(recordbyte, 77, 3)  '仕向支店ｺｰﾄﾞ
                    ZG10 = code.GetString(recordbyte, 80, 15) '仕向支店名
                    ZG11 = code.GetString(recordbyte, 95, 1)  '預金種目
                    ZG12 = code.GetString(recordbyte, 96, 7)  '口座番号
                    ZG13 = code.GetString(recordbyte, 103, 17) 'ダミー

                    'データ区分(=1)
                    If ZG01 <> "1" Then
                        Return False
                    End If

                    Return True

                Catch
                    Throw
                End Try

            End Function

        End Structure
        Public SplitFile_Para As SplitFilePara = Nothing

    End Class

    Private Function fn_AddEndRecord(ByVal DatFilePath As String) As Boolean
        Try
            Dim fs As FileStream = Nothing
            Dim br As BinaryReader = Nothing
            'Dim bt(119) As Byte
            Dim bt(121) As Byte
            Dim EndRecord As String

            '分割ファイル存在チェック
            If Not File.Exists(DatFilePath) Then
                Return False
            End If

            Try
                '開始レコードと最終レコード読込
                fs = New FileStream(DatFilePath, FileMode.Open, FileAccess.ReadWrite)
                br = New BinaryReader(fs, MOJICode)
                '開始レコード読込
                'bt = br.ReadBytes(120)
                bt = br.ReadBytes(File_Byte)
                '最終レコード読込
                'fs.Seek(-120, SeekOrigin.End)
                fs.Seek(-File_Byte, SeekOrigin.End)
                EndRecord = MOJICode.GetString(br.ReadBytes(1))

                '最終レコードがトレーラのときエンドレコード追加
                If EndRecord = "8" Then
                    'Dim b() As Byte = MOJICode.GetBytes("9".PadRight(120))
                    Dim b() As Byte = MOJICode.GetBytes("9".PadRight(File_Byte))
                    fs.Seek(0, SeekOrigin.End)
                    fs.Write(b, 0, 120)
                    'fs.Write(b, 0, File_Byte)
                End If

            Catch
                Throw
            Finally
                If Not fs Is Nothing Then fs.Close()
                If Not br Is Nothing Then br.Close()
            End Try

        Catch ex As Exception
            MainLOG.Write("エンドレコード追加", "失敗", "ファイル名：" & FileName & "　" & ex.Message)
            Message = "エンドレコード追加失敗：" & FileName & "　" & ex.Message
            Return False
        End Try

        Return True
    End Function

End Class
