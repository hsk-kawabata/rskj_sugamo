Imports System.IO
Imports System.Text
Imports CASTCommon
Imports CAstFormat

Module ModMain
    ' ログ処理クラス
    Private ELog As New CASTCommon.ClsEventLOG
    Private MainLOG As New CASTCommon.BatchLOG("KFJ080", "再振データ作成")

    ' パブリックＤＢ
    '*** Str Upd 2015/12/01 SO)荒木 for 不要MyOracle削除 ***
    'Private MainDB As New CASTCommon.MyOracle
    Private MainDB As CASTCommon.MyOracle
    '*** Str Upd 2015/12/01 SO)荒木 for 不要MyOracle削除 ***
    Private OraReader As CASTCommon.MyOracleReader

    ' システム連携
    Private clsFUSION As New clsFUSION.clsMain
    Private Renkei As New CAstSystem.ClsRenkei

    ' パラメータ
    Private TorisCode As String = ""
    Private TorifCode As String = ""
    Private FuriDate As String = ""
    Private SFuriDate As String = ""
    Private JobTuuban As Integer = 0
    Private JobMessage As String = ""
    Private FileKbn As String 
    'INIファイルより
    Private ETC_DIR, DENBK_DIR, DAT_DIR, DATBK_DIR, NTC, NIPPOU_HONBU, CSV_DIR As String
    '*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
    Private sfuriformat As String
    '*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
    Private mLockWaitTime As Integer = 60
    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

    '2016/10/12 ayabe RSV2 ADD --------------------------------------------------------------- START
    Private SFuriCode As String = ""
    '2016/10/12 ayabe RSV2 ADD --------------------------------------------------------------- END

    '共通変数
    Private MOTIKOMI_KBN As String = ""
    Private MOTIKOMI_SEQ As Integer
    Private FMT_KBN As String
    Private BAITAI_CODE As String
    Private MULTI_KBN As String
    Private CODE_KBN As String
    Private ITAKU_KANRI_CODE As String
    Private SYORI_KEN As Long = 0
    Private SYORI_KIN As Long = 0
    Private FURI_KEN As Long = 0
    Private FURI_KIN As Long = 0
    Private FUNOU_KEN As Long = 0
    Private FUNOU_KIN As Long = 0
    Private RECORD_COUNT As Integer = 0
    Private IN_FILE_NAME As String
    Private OUT_FILE_NAME As String
    Private strSFURI_FLG, strSFURI_FCODE As String
    '処理結果確認表印刷項目
    Public Structure Syorikekka
        Public TORIS_CODE As String
        Public TORIF_CODE As String
        Public ITAKU_NNAME As String
        Public ITAKU_CODE As String
        Public FURI_DATE As String
        Public BAITAI_CODE As String
        Public SYORI_KEN As Long
        Public SYORI_KIN As Long
        Public BIKO As String
        '2010/10/05.Sakon　再振替日追加 +++++
        Public SFURI_DATE As String
        '++++++++++++++++++++++++++++++++++++
    End Structure
    Private KekkaData As Syorikekka

    Private KFJP021 As clsKFJP021
    Public Function Main(ByVal CmdArgs() As String) As Integer
        Try
            ELog.Write("開始")

            '初期処理
            If SaifuriInit(CmdArgs) = False Then
                Return -1
            End If

            '主処理
            MainDB = New MyOracle
            Dim ret As Boolean = SaifuriMain()
            If ret = True Then
                MainDB.Commit()
            Else
                MainDB.Rollback()
            End If
            '終了処理
            If SaifuriEnd(ret) = False Then
                Return -1
            Else
                Return 0
            End If

        Catch ex As Exception
            ELog.Write(ex.Message)
            Return -1
        Finally
            '*** Str add 2016/01/07 SO)荒木 for MyOracleクローズ忘れを修正（潜在） ***
            If Not MainDB Is Nothing Then
                MainDB.close()
            End If
            '*** End add 2016/01/07 SO)荒木 for MyOracleクローズ忘れを修正（潜在） ***

            ELog.Write("終了")
        End Try
    End Function

    Private Function SaifuriInit(ByVal CmdArgs() As String) As Boolean
        Try
            MainLOG.Write("(初期処理)開始", "成功")

            'パラメータチェック
            If CmdArgs.Length = 0 Then
                MainLOG.Write("パラメータチェック", "失敗", "コマンドライン引数異常")
                Return False
            End If

            Dim param() As String = CmdArgs(0).Split(","c)
            If param.Length = 4 Then
                TorisCode = Mid(param(0), 1, 10)
                TorifCode = Mid(param(0), 11, 2)
                FuriDate = param(1)
                SFuriDate = param(2)
                JobTuuban = CInt(param(3))

                'ログの初期設定
                MainLOG.JobTuuban = JobTuuban
                MainLOG.ToriCode = param(0)
                If MainLOG.UserID.Trim = "" Then
                    MainLOG.Write("ログイン名取得", "失敗", "")
                    MainLOG.UpdateJOBMASTbyErr("ログイン名取得失敗")
                    Return False
                End If
                MainLOG.FuriDate = FuriDate
            Else
                MainLOG.Write("パラメータチェック", "失敗", "コマンドライン引数異常：" & CmdArgs(0))

                Return False
            End If

        Catch ex As Exception
            MainLOG.Write("初期処理", "失敗", ex.Message)

            Return False
        Finally
            MainLOG.Write("(初期処理)終了", "成功")
        End Try

        Return True
    End Function

    Private Function SaifuriMain() As Boolean
        Try
            MainLOG.Write("(主処理)開始", "成功")
            

            If IniRead() = False Then
                Return False
            End If

            '再振スケジュールチェック
            If SaifuriCheck() = False Then
                Return False
            End If

            '作成ファイル名の設定
            '*** Str Upd 2016/03/07 SYS)森 for 排他制御追加 ***
            IN_FILE_NAME = DAT_DIR & "S" & TorisCode & TorifCode & ".DAT"
            'IN_FILE_NAME = DAT_DIR & "S" & TorisCode & ".DAT"
            '*** End Upd 2016/03/07 SYS)森 for 排他制御追加 ***
            OUT_FILE_NAME = ETC_DIR & "S" & TorisCode & strSFURI_FCODE & ".DAT"
            Dim PrintData As New List(Of Syorikekka)

            '再振データ作成処理
            Select Case FMT_KBN
                Case "00", "04", "05"  '全銀データ
                    If MakeSaifuriData_ZENGIN(PrintData) = False Then
                        MainDB.Rollback()
                        Return False
                    End If
                    '*** Str Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                Case Else
                    If sfuriformat <> "err" And sfuriformat <> "" Then  ' iniファイル定義あり
                        Dim format() As String = sfuriformat.Split(","c)
                        For Each value As String In format
                            If value.Trim = FMT_KBN Then
                                If MakeSaifuriData_ZENGIN(PrintData) = False Then
                                    MainDB.Rollback()
                                    Return False
                                End If

                                Exit For
                            End If
                        Next
                    End If
                    '*** End Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
            End Select

            'CSVデータ作成
            KFJP021 = New clsKFJP021
            Dim CSVFile As String = KFJP021.CreateCsvFile

            '印刷データ設定
            If OutPutData(PrintData) = False Then
                Return False
            End If
            KFJP021.CloseCsv()
            '再振処理結果確認表
            ' 2016/01/07 タスク）綾部 CHG 【OT】UI_99-99(RSV2対応) -------------------- START
            'Dim ExeRepo As New CAstReports.ClsExecute
            'Dim ret As Integer = ExeRepo.ExecReport("KFJP021", MainLOG.UserID & "," & CSVFile)
            'If ret <> 0 Then
            '    MainLOG.Write("処理結果確認表(再振データ作成)印刷", "失敗", "リターンコード:" & ret)
            '    JobMessage = "処理結果確認表(再振データ作成)印刷に失敗しました。"
            '    Return False
            'Else
            '    MainLOG.Write("処理結果確認表(再振データ作成)印刷", "成功")
            'End If
            Select Case CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SYORIKEKKA_SFURI")
                Case "0"
                    ' 指定が0の場合は印刷を行わない
                Case Else
                    Dim ExeRepo As New CAstReports.ClsExecute
                    Dim ret As Integer = ExeRepo.ExecReport("KFJP021", MainLOG.UserID & "," & CSVFile)
                    If ret <> 0 Then
                        MainLOG.Write("処理結果確認表(再振データ作成)印刷", "失敗", "リターンコード:" & ret)
                        JobMessage = "処理結果確認表(再振データ作成)印刷に失敗しました。"
                        Return False
                    Else
                        MainLOG.Write("処理結果確認表(再振データ作成)印刷", "成功")
                    End If
            End Select
            ' 2016/01/07 タスク）綾部 CHG 【OT】UI_99-99(RSV2対応) -------------------- START

            'ファイルのコピーを行う(ETCフォルダ)
            If File.Exists(OUT_FILE_NAME) Then
                File.Delete(OUT_FILE_NAME)
            End If
            File.Copy(IN_FILE_NAME, OUT_FILE_NAME)

            '不要なファイルを削除する
            If File.Exists(IN_FILE_NAME) Then
                File.Delete(IN_FILE_NAME)
            End If

            '落とし込みのジョブ登録を行う
            Dim okFlg As Boolean = True
            Dim jobid As String
            jobid = "J010"      '落とし込み
            Dim para As String
            '媒体はSA(再振)として処理する
            para = TorisCode & strSFURI_FCODE & "," & SFuriDate & "," & CODE_KBN & "," & FMT_KBN _
                                   & "," & "SA" & "," & "0"
            'job検索
            Dim iRet As Integer
            iRet = MainLOG.SearchJOBMAST(jobid, para, MainDB)
            If iRet = 1 Then    'ジョブ登録済
                MainLOG.Write("落とし込みジョブ登録", "失敗", "登録済")

                JobMessage = "落とし込みジョブ登録に失敗しました。"
                Return False
            ElseIf iRet = -1 Then 'ジョブ検索失敗
                MainLOG.Write("落とし込みジョブ登録", "失敗", "検索失敗")
                JobMessage = "落とし込みジョブ登録に失敗しました。"
                Return False
            End If

            'job登録
            If MainLOG.InsertJOBMAST(jobid, MainLOG.UserID, para, MainDB) = False Then
                MainLOG.Write("落とし込みジョブ登録", "失敗", "検索失敗")
                JobMessage = "落とし込みジョブ登録に失敗しました。"
                Return False
            Else

                MainLOG.Write("落とし込みジョブ登録", "成功")
            End If

        Catch ex As Exception
            MainLOG.Write("主処理", "失敗", ex.Message)
            
            JobMessage = ex.Message
            Return False
        Finally
            MainLOG.Write("(主処理)終了", "成功")
        End Try

        Return True
    End Function
    Private Function OutPutData(ByVal List As List(Of Syorikekka)) As Boolean
        Try
            Dim Today As String = Now.ToString("yyyyMMdd")
            Dim Time As String = Now.ToString("HHmmss")
            For No As Integer = 0 To List.Count - 1
                With KFJP021
                    .OutputCsvData(Today, True)                         '処理日
                    .OutputCsvData(Time, True)                          'タイムスタンプ
                    .OutputCsvData(List(No).FURI_DATE, True)            '振替日
                    .OutputCsvData(List(No).TORIS_CODE, True)           '取引先主コード
                    .OutputCsvData(List(No).TORIF_CODE, True)           '取引先副コード
                    .OutputCsvData(List(No).ITAKU_NNAME, True)          '取引先名
                    .OutputCsvData(List(No).ITAKU_CODE, True)           '委託者コード
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                    '媒体名をテキストから取得する
                    .OutputCsvData(CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "Common_媒体コード.TXT"), _
                                                                 List(No).BAITAI_CODE), True)
                    'Select Case List(No).BAITAI_CODE                    '媒体
                    '    Case "00"
                    '        .OutputCsvData("伝送", True)
                    '    Case "01"
                    '        .OutputCsvData("FD3.5", True)
                    '    Case "04"
                    '        .OutputCsvData("依頼書", True)
                    '    Case "05"
                    '        .OutputCsvData("MT", True)
                    '    Case "06"
                    '        .OutputCsvData("CMT", True)
                    '    Case "07"
                    '        .OutputCsvData("学校自振", True)
                    '    Case "09"
                    '        .OutputCsvData("伝票", True)
                    '        '2012/06/30 標準版 WEB伝送対応
                    '    Case "10"
                    '        .OutputCsvData("WEB伝送", True)
                    '    Case "11"
                    '        .OutputCsvData("DVD-RAM", True)
                    '    Case "12"
                    '        .OutputCsvData("その他", True)
                    '    Case "13"
                    '        .OutputCsvData("その他", True)
                    '    Case "14"
                    '        .OutputCsvData("その他", True)
                    '    Case "15"
                    '        .OutputCsvData("その他", True)
                    '    Case Else
                    '        .OutputCsvData("その他", True)
                    'End Select
                    ' 2016/01/23 タスク）斎藤 UPD 【PG】UI_B-14-99(RSV2対応) -------------------- END
                    .OutputCsvData(List(No).SYORI_KEN.ToString, True)   '処理件数
                    .OutputCsvData(List(No).SYORI_KIN.ToString, True)   '処理金額

                    '2010/10/05.Sakon　再振替日追加 ++++++++++++++++++++++++++++++
                    .OutputCsvData(List(No).BIKO, True)                 '備考
                    .OutputCsvData(List(No).SFURI_DATE, True, True)     '再振替日
                    '.OutputCsvData(List(No).BIKO, True, True) '備考
                    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                End With
            Next
            Return True
        Catch ex As Exception
            MainLOG.Write("処理結果確認表印刷データ作成", "失敗", ex.ToString)
            JobMessage = "処理結果確認表のデータ作成に失敗しました。"
        End Try
    End Function

    Private Function SaifuriEnd(ByVal ret As Boolean) As Boolean
        Try
            MainLOG.Write("(終了処理)開始", "成功")


            If ret = False Then
                MainLOG.UpdateJOBMASTbyErr(JobMessage)
                Return False
            Else
                MainLOG.UpdateJOBMASTbyOK("")
            End If

        Catch ex As Exception
            MainLOG.Write("終了処理", "失敗", ex.Message)
            Return False
        Finally
            MainLOG.Write("(終了処理)終了", "成功")

        End Try

        Return True
    End Function

    Private Function IniRead() As Boolean
        'DENフォルダチェック
        ETC_DIR = CASTCommon.GetFSKJIni("COMMON", "ETC")
        If ETC_DIR = "err" OrElse ETC_DIR = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:ETCフォルダ 分類:COMMON 項目:ETC")
            JobMessage = "設定ファイル取得失敗 項目名:ETCフォルダ 分類:COMMON 項目:ETC"
            Return False
        End If

        'DENBKフォルダチェック
        DENBK_DIR = CASTCommon.GetFSKJIni("COMMON", "DENBK")
        If DENBK_DIR = "err" OrElse DENBK_DIR = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DENBKフォルダ 分類:COMMON 項目:DENBK")
            JobMessage = "設定ファイル取得失敗 項目名:DENBKフォルダ 分類:COMMON 項目:DENBK"
            Return False
        End If

        'DATフォルダチェック
        DAT_DIR = CASTCommon.GetFSKJIni("COMMON", "DAT")
        If DAT_DIR = "err" OrElse DAT_DIR = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT")
            JobMessage = "設定ファイル取得失敗 項目名:DATフォルダ 分類:COMMON 項目:DAT"
            Return False
        End If

        'DATBKフォルダチェック
        DATBK_DIR = CASTCommon.GetFSKJIni("COMMON", "DATBK")
        If DATBK_DIR = "err" OrElse DATBK_DIR = "" Then
            MainLOG.Write("INIファイル取得", "失敗", "設定ファイル取得失敗 項目名:DATBKフォルダ 分類:COMMON 項目:DATBK")
            JobMessage = "設定ファイル取得失敗 項目名:DATBKフォルダ 分類:COMMON 項目:DATBK"
            Return False
        End If

        '*** Str Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
        sfuriformat = CASTCommon.GetFSKJIni("COMMON", "SFURI_FORMAT")
        sfuriformat = sfuriformat.Replace(" ", "")
        sfuriformat = sfuriformat.Replace(vbTab, "")
        '*** End Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME1")
        If IsNumeric(sWork) Then
            mLockWaitTime = CInt(sWork)
            If mLockWaitTime <= 0 Then
                mLockWaitTime = 60
            End If
        End If

        '2016/10/12 ayabe RSV2 ADD --------------------------------------------------------------- START
        ' 再振対象振替結果コード(<RSKJ.INI> 分類:RSV2_V1.0.0 項目:SFURI_CODE)
        SFuriCode = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SFURI_CODE")
        If SFuriCode = "err" OrElse SFuriCode = "" Then
            SFuriCode = ""
        End If
        '2016/10/12 ayabe RSV2 ADD --------------------------------------------------------------- END

        Return True
    End Function
    Private Function SaifuriCheck() As Boolean

        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim OraReader2 As MyOracleReader = Nothing
        Dim SQL As New StringBuilder(128)

        Try

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '取引先マスタ検索(再振指定の登録がされているかチェック)
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            SQL.Append("SELECT * FROM TORIMAST")
            SQL.Append(" WHERE TORIS_CODE_T = " & SQ(TorisCode))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(TorifCode))
            If OraReader.DataReader(SQL) = True Then
                strSFURI_FLG = OraReader.GetString("SFURI_FLG_T")
                strSFURI_FCODE = OraReader.GetString("SFURI_FCODE_T")
                ITAKU_KANRI_CODE = OraReader.GetString("ITAKU_KANRI_CODE_T")
                strSFURI_FLG = OraReader.GetString("SFURI_FLG_T")
                strSFURI_FCODE = OraReader.GetString("SFURI_FCODE_T")
                ITAKU_KANRI_CODE = OraReader.GetString("ITAKU_KANRI_CODE_T")
                FMT_KBN = OraReader.GetString("FMT_KBN_T")
                BAITAI_CODE = OraReader.GetString("BAITAI_CODE_T")
                MULTI_KBN = OraReader.GetString("MULTI_KBN_T")
                CODE_KBN = OraReader.GetString("CODE_KBN_T")
                OraReader.Close()
            Else
                '取引先なし
                MainLOG.Write("取引先検索", "失敗", "")

                JobMessage = "取引先の検索に失敗しました。"
                OraReader.Close()
                Return False
            End If

            If strSFURI_FLG <> "1" Then
                '再振非対象
                MainLOG.Write("再振フラグチェック", "失敗", "契約なし:" & TorisCode & "-" & TorifCode)
                JobMessage = "取引先に再振契約がありません。取引先コード:" & TorisCode & "-" & TorifCode
                OraReader.Close()
                Return False
            ElseIf strSFURI_FCODE = "" Then
                '再振副コード未設定
                MainLOG.Write("再振副コードチェック", "失敗", "再振副コードなし:" & TorisCode & "-" & TorifCode)
                JobMessage = "再振副コードが登録されていません。取引先コード:" & TorisCode & "-" & TorifCode
                OraReader.Close()
                Return False
            Else
                Select Case FMT_KBN
                    Case "00", "04", "05"
                    Case Else
                        '*** Str Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                        'フォーマット異常
                        'MainLOG.Write("フォーマット区分チェック", "失敗", "再振非対応フォーマット:" & FMT_KBN)
                        'JobMessage = "フォーマット区分が再振非対応です。取引先コード:" & TorisCode & "-" & TorifCode & " フォーマット区分:" & FMT_KBN
                        'OraReader.Close()
                        'Return False
                        Dim errflg As Boolean = True
                        If sfuriformat <> "err" And sfuriformat <> "" then  ' iniファイル定義あり
                            Dim format() As String = sfuriformat.Split(","c)
                            For Each value As String In format
                                If value.Trim = FMT_KBN Then
                                    errflg = False
                                    Exit For
                                End IF
                            Next
                        End If

                        If errflg = True Then
                            'フォーマット異常
                            MainLOG.Write("フォーマット区分チェック", "失敗", "再振非対応フォーマット:" & FMT_KBN)
                            JobMessage = "フォーマット区分が再振非対応です。取引先コード:" & TorisCode & "-" & TorifCode & " フォーマット区分:" & FMT_KBN
                            OraReader.Close()
                            Return False
                        End If
                        '*** End Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                End Select
                Select Case BAITAI_CODE
                    Case "07"
                        '媒体コード異常
                        MainLOG.Write("媒体コードチェック", "失敗", "再振非対応媒体:" & BAITAI_CODE)
                        JobMessage = "媒体コードが再振非対応です。取引先コード:" & TorisCode & "-" & TorifCode & " 媒体コード:" & BAITAI_CODE
                        OraReader.Close()
                        Return False
                End Select
            End If

            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            'スケジュールの検索（初振のスケジュール）
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            SQL = New StringBuilder(128)
            SQL.Append("SELECT *")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(TorisCode))
            SQL.Append(" AND TORIF_CODE_S = " & SQ(TorifCode))
            SQL.Append(" AND FURI_DATE_S = " & SQ(FuriDate))
            SQL.Append(" AND KSAIFURI_DATE_S = " & SQ(SFuriDate))
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            SQL.Append(" FOR UPDATE WAIT " & mLockWaitTime)
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            If OraReader.DataReader(SQL) = True Then

                MOTIKOMI_SEQ = OraReader.GetInt("MOTIKOMI_SEQ_S")
                If OraReader.GetString("HENKAN_FLG_S") <> "1" Then
                    '返還未完了
                    MainLOG.Write("スケジュール検索", "失敗", "返還未完了 取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate)
                    JobMessage = "返還処理が行われていません。　取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate
                    OraReader.Close()
                    Return False
                End If
                If MULTI_KBN = "0" Then
                    If OraReader.GetInt64("FUNOU_KEN_S") = 0 Then
                        '不能対象無し
                        MainLOG.Write("不能件数チェック(シングル)", "失敗", "不能0件  取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate)
                        JobMessage = "不能件数が0件です。　取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate
                        OraReader.Close()
                        Return False
                    End If
                End If
                OraReader.Close()
            Else
                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                If OraReader.Message <> "" Then
                    Dim errmsg As String
                    If OraReader.Message.StartsWith("ORA-30006") Then
                        errmsg = "再振データ作成処理で実行待ちタイムアウト"
                    Else
                        errmsg = "再振データ作成処理ロック異常"
                    End If

                    MainLOG.Write_Err("再振データ作成処理", "失敗", errmsg & " 取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate)
                    JobMessage = errmsg & "　取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate
                    OraReader.Close()
                    Return False
                End If
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                'スケジュールなし
                MainLOG.Write("スケジュール検索", "失敗", "スケジュールなし 取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate)
                JobMessage = "対象のスケジュールが存在しません。　取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate
                OraReader.Close()
                Return False
            End If

            If MULTI_KBN = "1" Then  'マルチ指定の場合、同じ持ち込みシーケンスのスケジュール全て検索
                Dim lngFUNOU_KEN As Long = 0
                SQL = New StringBuilder(128)
                SQL.Append(" SELECT * ")
                SQL.Append(" FROM TORIMAST,SCHMAST")
                SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
                SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                SQL.Append(" AND FURI_DATE_S = " & SQ(FuriDate))
                SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(ITAKU_KANRI_CODE))
                SQL.Append(" AND MOTIKOMI_SEQ_S = " & MOTIKOMI_SEQ)
                SQL.Append(" AND TYUUDAN_FLG_S = '0'")
                SQL.Append(" ORDER BY TORIS_CODE_T,TORIF_CODE_T")
                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                SQL.Append(" FOR UPDATE OF SCHMAST.TORIS_CODE_S WAIT " & mLockWaitTime)
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                If OraReader.DataReader(SQL) = True Then
                    While OraReader.EOF = False
                        If OraReader.GetString("HENKAN_FLG_S") <> "1" Then
                            '返還未完了
                            MainLOG.Write("スケジュール検索", "失敗", "返還未完了 取引先コード:" & OraReader.GetString("TORIS_CODE_T") & "-" & OraReader.GetString("TORIF_CODE_T") & " 振替日:" & FuriDate)
                            JobMessage = "返還処理が行われていません。　取引先コード:" & OraReader.GetString("TORIS_CODE_T") & "-" & OraReader.GetString("TORIF_CODE_T") & " 振替日:" & FuriDate
                            OraReader.Close()
                            Return False
                        End If
                        lngFUNOU_KEN += OraReader.GetInt64("FUNOU_KEN_S")
                        OraReader.NextRead()
                    End While
                    OraReader.Close()
                Else
                    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                    If OraReader.Message <> "" Then
                        Dim errmsg As String
                        If OraReader.Message.StartsWith("ORA-30006") Then
                            errmsg = "再振データ作成処理で実行待ちタイムアウト"
                        Else
                            errmsg = "再振データ作成処理ロック異常"
                        End If
                        MainLOG.Write_Err("再振データ作成処理", "失敗", errmsg & " 取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate)
                        JobMessage = errmsg & "　取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate
                        OraReader.Close()
                        Return False
                    End If
                    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                    'スケジュール検索失敗
                    MainLOG.Write("スケジュール検索", "失敗", "取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate)
                    JobMessage = "スケジュールの検索に失敗しました。　取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate
                    OraReader.Close()
                    Return False
                End If

                If lngFUNOU_KEN = 0 Then
                    '不能対象無し
                    MainLOG.Write("不能件数チェック(マルチ)", "失敗", "不能0件  代表委託者コード:" & ITAKU_KANRI_CODE & " 振替日:" & FuriDate & " 持込SEQ:" & MOTIKOMI_SEQ)
                    JobMessage = "不能件数が0件です。　代表委託者コード:" & ITAKU_KANRI_CODE & " 振替日:" & FuriDate & " 持込SEQ:" & MOTIKOMI_SEQ
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
            SQL.Append(" AND FURI_DATE_S = " & SQ(FuriDate))
            SQL.Append(" AND KSAIFURI_DATE_S = " & SQ(SFuriDate))
            SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(ITAKU_KANRI_CODE))
            SQL.Append(" AND MOTIKOMI_SEQ_S = " & MOTIKOMI_SEQ)
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" ORDER BY TORIS_CODE_T,TORIF_CODE_T")
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            SQL.Append(" FOR UPDATE OF SCHMAST.TORIS_CODE_S WAIT " & mLockWaitTime)
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            If OraReader.DataReader(SQL) = True Then
                While OraReader.EOF = False
                    SQL = New StringBuilder(128)
                    SQL.Append("SELECT * FROM SCHMAST,TORIMAST")
                    SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
                    SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
                    SQL.Append(" AND TORIS_CODE_S = " & SQ(OraReader.GetString("TORIS_CODE_T")))
                    SQL.Append(" AND TORIF_CODE_S = " & SQ(OraReader.GetString("SFURI_FCODE_T")))
                    SQL.Append(" AND FURI_DATE_S = " & SQ(SFuriDate))
                    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                    SQL.Append(" FOR UPDATE OF SCHMAST.TORIS_CODE_S WAIT " & mLockWaitTime)
                    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                    If OraReader2.DataReader(SQL) = False Then
                        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                        If OraReader2.Message <> "" Then
                            Dim errmsg As String
                            If OraReader2.Message.StartsWith("ORA-30006") Then
                                errmsg = "再振データ作成処理で実行待ちタイムアウト"
                            Else
                                errmsg = "再振データ作成処理ロック異常"
                            End If

                            MainLOG.Write_Err("再振データ作成処理", "失敗", errmsg & " 取引先コード:" & OraReader2.GetString("TORIS_CODE_T") & "-" & OraReader2.GetString("TORIF_CODE_T") & " 振替日:" & SFuriDate)
                            JobMessage = errmsg & "　取引先コード:" & OraReader2.GetString("TORIS_CODE_T") & "-" & OraReader2.GetString("TORIF_CODE_T") & " 振替日:" & SFuriDate
                            OraReader.Close()
                            Return False
                        End If
                        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                        '再振スケジュールなし
                        MainLOG.Write("再振スケジュール検索", "失敗", "再振スケジュールなし 取引先コード:" & OraReader2.GetString("TORIS_CODE_T") & "-" & OraReader2.GetString("TORIF_CODE_T") & " 振替日:" & SFuriDate)
                        JobMessage = "再振スケジュールが存在しません。 取引先コード:" & OraReader2.GetString("TORIS_CODE_T") & "-" & OraReader2.GetString("TORIF_CODE_T") & " 振替日:" & SFuriDate
                        OraReader2.Close()
                        Return False
                    Else
                        If OraReader2.GetString("TOUROKU_FLG_S") = "1" Then
                            '登録処理済
                            MainLOG.Write("再振スケジュールフラグチェック", "失敗", "返還未完了 取引先コード:" & OraReader2.GetString("TORIS_CODE_T") & "-" & OraReader2.GetString("TORIF_CODE_T") & " 振替日:" & FuriDate)
                            JobMessage = "再振登録済です。 　取引先コード:" & OraReader2.GetString("TORIS_CODE_T") & "-" & OraReader2.GetString("TORIF_CODE_T") & " 振替日:" & SFuriDate
                            OraReader2.Close()
                            Return False
                        End If
                        If FMT_KBN <> OraReader2.GetString("FMT_KBN_T") Then
                            'フォーマット区分一致チェック
                            MainLOG.Write("再振フォーマットチェック", "失敗", "返還未完了 取引先コード:" & OraReader2.GetString("TORIS_CODE_T") & "-" & OraReader2.GetString("TORIF_CODE_T") & " 振替日:" & FuriDate)
                            JobMessage = "再振フォーマット不一致 　取引先コード:" & OraReader2.GetString("TORIS_CODE_T") & "-" & OraReader2.GetString("TORIF_CODE_T") & " 振替日:" & SFuriDate
                            OraReader2.Close()
                            Return False
                            OraReader2.Close()
                            Return False
                        End If
                    End If
                    OraReader.NextRead()
                End While
                OraReader.Close()
            Else
                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                If OraReader.Message <> "" Then
                    Dim errmsg As String
                    If OraReader.Message.StartsWith("ORA-30006") Then
                        errmsg = "再振データ作成処理で実行待ちタイムアウト"
                    Else
                        errmsg = "再振データ作成処理ロック異常"
                    End If

                    MainLOG.Write_Err("再振データ作成処理", "失敗", errmsg & " 代表委託者コード:" & ITAKU_KANRI_CODE & " 振替日:" & FuriDate & " 持込SEQ:" & MOTIKOMI_SEQ)
                    JobMessage = errmsg & "　取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate
                    OraReader.Close()
                    Return False
                End If
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                'スケジュール検索失敗(MSG0002E)
                MainLOG.Write("スケジュール検索", "失敗", "代表委託者コード:" & ITAKU_KANRI_CODE & " 振替日:" & FuriDate & " 持込SEQ:" & MOTIKOMI_SEQ)
                JobMessage = "スケジュールの検索に失敗しました。　取引先コード:" & TorisCode & "-" & TorifCode & " 振替日:" & FuriDate
                OraReader.Close()
                Return False
            End If
            MainLOG.Write("マスタチェック", "成功")

            Return True
        Catch ex As Exception
            JobMessage = "マスタ検索中にエラーが発生しました。"
            MainLOG.Write("(再振作成チェック)", "失敗", ex.ToString)

            Return False
        Finally
            If OraReader IsNot Nothing Then OraReader.Close()
            If OraReader2 IsNot Nothing Then OraReader2.Close()
        End Try
    End Function
    Public Structure KeyInfo
        Dim TORIS_CODE As String            ' 取引先主コード
        Dim TORIF_CODE As String            ' 取引先副コード
        Dim DENSO_CNT_CODE As String        ' 伝送相手センター確認コード
        Dim TOHO_CNT_CODE As String         ' 伝送当方センター確認コード
        Dim FURI_DATE As String             ' 振替日
        Dim TORIMATOME_SIT_NO As String     ' 取りまとめ店
        Dim FUNOU_MEISAI_KBN As String      ' 不能結果明細表出力
        Dim CODE_KBN As String              ' コード区分
        Dim BAITAI_CODE As String           ' 媒体コード
        Dim FMT_KBN As String               ' フォーマット区分
        Dim RENKEI_KBN As String            ' 連携区分
        Dim ENC_KBN_T As String             ' 暗号化区分（取引先マスタ）
        Dim ENC_KBN_S As String             ' 暗号化区分（スケジュールマスタ）
        Dim ITAKU_CODE As String            '委託者コード
        Dim ITAKU_KNAME As String           '委託者名
        Dim ITAKU_NNAME As String           '委託者名
        Dim MULTI_KBN As String
        Dim SYORI_KEN As Double
        Dim SYORI_KIN As Double
        Dim FURI_KEN As Double
        Dim FURI_KIN As Double
        Dim FUNOU_KEN As Double
        Dim FUNOU_KIN As Double
        Dim MESSAGE As String
        Dim ErrorDetail As String           ' エラー詳細
        ' 初期化
        Public Sub Init()
            TORIS_CODE = ""
            TORIF_CODE = ""
            DENSO_CNT_CODE = ""
            TOHO_CNT_CODE = ""
            FURI_DATE = ""
            TORIMATOME_SIT_NO = ""
            FUNOU_MEISAI_KBN = ""
            CODE_KBN = ""
            FMT_KBN = ""
            RENKEI_KBN = ""
            ENC_KBN_S = ""
            ENC_KBN_T = ""
            ITAKU_CODE = ""
            ITAKU_KNAME = ""
            MULTI_KBN = ""
            SYORI_KEN = 0
            SYORI_KIN = 0
            FURI_KEN = 0
            FURI_KIN = 0
            FUNOU_KEN = 0
            FUNOU_KIN = 0
            MESSAGE = ""
            ErrorDetail = ""
        End Sub
        ' DBからの値を設定
        Friend Sub SetOracleData(ByVal OraReader As OracleClient.OracleDataReader)
            Dim GCOM As New MenteCommon.clsCommon

            FURI_DATE = GCOM.NzStr(OraReader.Item("FURI_DATE_S")).PadRight(8)              '振替日
            TORIS_CODE = GCOM.NzStr(OraReader.Item("TORIS_CODE_S")).PadRight(7)            '取引先主コード
            TORIF_CODE = GCOM.NzStr(OraReader.Item("TORIF_CODE_S")).PadRight(2)            '取引先副コード
            ITAKU_KNAME = GCOM.NzStr(OraReader.Item("ITAKU_KNAME_T"))
            ITAKU_NNAME = GCOM.NzStr(OraReader.Item("ITAKU_NNAME_T"))
            ITAKU_CODE = GCOM.NzStr(OraReader.Item("ITAKU_CODE_T"))
            BAITAI_CODE = GCOM.NzStr(OraReader.Item("BAITAI_CODE_T"))
            SYORI_KEN = GCOM.NzLong(OraReader.Item("SYORI_KEN_S"))
            SYORI_KIN = GCOM.NzLong(OraReader.Item("SYORI_KIN_S"))
            FURI_KEN = GCOM.NzLong(OraReader.Item("FURI_KEN_S"))
            FURI_KIN = GCOM.NzLong(OraReader.Item("FURI_KIN_S"))
            FUNOU_KEN = GCOM.NzLong(OraReader.Item("FUNOU_KEN_S"))
            FUNOU_KIN = GCOM.NzLong(OraReader.Item("FUNOU_KIN_S"))
            FUNOU_MEISAI_KBN = GCOM.NzStr(OraReader.Item("FUNOU_MEISAI_KBN_T"))

            CODE_KBN = GCOM.NzStr(OraReader.Item("CODE_KBN_T"))
            FMT_KBN = GCOM.NzStr(OraReader.Item("FMT_KBN_T"))
            MULTI_KBN = GCOM.NzStr(OraReader.Item("MULTI_KBN_T"))
            ENC_KBN_T = GCOM.NzStr(OraReader.Item("ENC_KBN_T"))
            MESSAGE = ""
            ErrorDetail = ""
        End Sub

        ' ＤＢからの値を設定（伝送用）
        Friend Sub SetOraData(ByVal oraReader As CASTCommon.MyOracleReader)
            TORIS_CODE = oraReader.GetString("TORIS_CODE_S").PadRight(7)
            TORIF_CODE = oraReader.GetString("TORIF_CODE_S").PadRight(2)
            FURI_DATE = oraReader.GetString("FURI_DATE_S").PadRight(8)
            TORIMATOME_SIT_NO = oraReader.GetString("TORIMATOME_SIT_NO_T").PadRight(14)
            FUNOU_MEISAI_KBN = oraReader.GetString("FUNOU_MEISAI_KBN_T")
            CODE_KBN = oraReader.GetString("CODE_KBN_T")
            FMT_KBN = oraReader.GetString("FMT_KBN_T")
            RENKEI_KBN = oraReader.GetString("RENKEI_KBN_T")
            ENC_KBN_S = oraReader.GetString("ENC_KBN_S")
            ENC_KBN_T = oraReader.GetString("ENC_KBN_T")
            MESSAGE = ""
            ErrorDetail = ""
        End Sub
    End Structure


    ' 機能　 ： 再振データ作成（全銀）
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 

    Private Function MakeSaifuriData_ZENGIN(ByVal PrintData As List(Of Syorikekka)) As Boolean
        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim SQL As New StringBuilder(128)

        'JISファイルを作成し、落とし込む(媒体変換は不要とする)

        Dim gZENGIN_REC1 As CAstFormat.CFormatZengin.ZGRECORD1 = Nothing
        Dim gZENGIN_REC2 As CAstFormat.CFormatZengin.ZGRECORD2 = Nothing
        Dim gZENGIN_REC8 As CAstFormat.CFormatZengin.ZGRECORD8 = Nothing
        Dim gZENGIN_REC9 As CAstFormat.CFormatZengin.ZGRECORD9 = Nothing
        Try
            Dim Key As New KeyInfo
            Dim SaifuriFMT As New CAstFormat.CFormat
            Dim para As New CAstBatch.CommData.stPARAMETER
            para.FSYORI_KBN = "1"
            para.FMT_KBN = FMT_KBN
            SaifuriFMT = CAstFormat.CFormat.GetFormat(para)

            '*** Str Upd 2015/12/01 SO)荒木 for 不要MyOracle削除 ***
            'Dim Comm As New CAstBatch.CommData(New MyOracle)
            Dim Comm As New CAstBatch.CommData(MainDB)
            '*** End Upd 2015/12/01 SO)荒木 for 不要MyOracle削除 ***
            SaifuriFMT.Oracle = MainDB
            Dim SaifuriStream As FileStream = Nothing
            SaifuriStream = New FileStream(IN_FILE_NAME, FileMode.Create, FileAccess.Write)

            Dim EndRecord As String = ""

            SQL.Append(" SELECT * ")
            SQL.Append(" FROM TORIMAST,SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_S = TORIS_CODE_T")
            SQL.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            SQL.Append(" AND FURI_DATE_S = " & SQ(FuriDate))
            SQL.Append(" AND KSAIFURI_DATE_S = " & SQ(SFuriDate))
            SQL.Append(" AND ITAKU_KANRI_CODE_T = " & SQ(ITAKU_KANRI_CODE))
            SQL.Append(" AND MOTIKOMI_SEQ_S = " & MOTIKOMI_SEQ)
            SQL.Append(" AND TYUUDAN_FLG_S = '0'")
            SQL.Append(" ORDER BY FILE_SEQ_S")  'ファイルシーケンス順に出力
            If OraReader.DataReader(SQL) = True Then
                'マスタ検索関数
                While OraReader.EOF = False

                    para.TORIS_CODE = OraReader.GetString("TORIS_CODE_S")
                    para.TORIF_CODE = OraReader.GetString("TORIF_CODE_S")
                    para.FURI_DATE = OraReader.GetString("FURI_DATE_S")

                    Comm.INFOParameter = para

                    ' 取引先マスタ取得
                    Call Comm.GetTORIMAST(para.TORIS_CODE, _
                                          para.TORIF_CODE)
                    SaifuriFMT.ToriData = Comm
                    If SaifuriFMT.FirstReadMast() = 0 Then
                        MainLOG.Write("スケジュール読込", "失敗", "取引先コード:" & para.TORIS_CODE & "-" & para.TORIF_CODE & " 振替日:" & para.FURI_DATE)
                        JobMessage = "スケジュールの読込に失敗しました。　取引先コード:" & para.TORIS_CODE & "-" & para.TORIF_CODE & " 振替日:" & para.FURI_DATE
                        Return False
                    End If
                    Dim sRet As String = ""
                    Dim stTori As CAstBatch.CommData.stTORIMAST ' 取引先情報
                    stTori = Comm.INFOToriMast
                    Do Until SaifuriFMT.EOF(1)
                        Key.Init()
                        Call Key.SetOracleData(OraReader.Reader)
                        sRet = SaifuriFMT.CheckSaifuriFormat()
                        'ヘッダ
                        If SaifuriFMT.IsHeaderRecord Then
                            Call SaifuriFMT.GetSaifuriHeaderRecord(Mid(SFuriDate, 5))
                            SYORI_KEN = 0
                            SYORI_KIN = 0
                        ElseIf SaifuriFMT.IsTrailerRecord Then
                            'Falseの場合、自動計算された不能の合計を書き込む
                            'Trueの場合は設定した処理件数･金額を書き込む
                            Call SaifuriFMT.GetSaifuriTrailerRecord(SYORI_KEN, SYORI_KIN, True)
                            If fn_SCHMAST_UPDATE(Key) = False Then
                                SaifuriStream.Close()
                                SaifuriStream = Nothing
                                OraReader.Close()
                                Exit Function
                            End If

                            '処理結果確認表印刷データ設定
                            With KekkaData
                                .TORIS_CODE = Key.TORIS_CODE
                                .TORIF_CODE = Key.TORIF_CODE
                                .ITAKU_CODE = Key.ITAKU_CODE
                                .ITAKU_NNAME = Key.ITAKU_NNAME
                                .FURI_DATE = FuriDate
                                .BAITAI_CODE = Key.BAITAI_CODE
                                .SYORI_KEN = SYORI_KEN
                                .SYORI_KIN = SYORI_KIN
                                '2010/10/05.Sakon　再振替日を追加 +++++
                                .SFURI_DATE = SFuriDate
                                '++++++++++++++++++++++++++++++++++++++
                            End With
                            PrintData.Add(KekkaData)

                        ElseIf SaifuriFMT.IsEndRecord Then
                            ' エンド
                            EndRecord = SaifuriFMT.RecordData

                        ElseIf SaifuriFMT.IsDataRecord Then
                            'データレコード
                            Call SaifuriFMT.GetSaifuriDataRecord()
                            '不能明細のみ出力
                            gZENGIN_REC2.Data = SaifuriFMT.RecordData

                            Select Case SaifuriFMT.InfoMeisaiMast.FURIKETU_CODE
                                Case 0
                                    SaifuriFMT.RecordData = ""
                                Case Else
                                    '2016/10/12 ayabe RSV2 CHG --------------------------------------------------------------- START
                                    'SYORI_KEN += 1                                  '処理件数合計
                                    'SYORI_KIN += Long.Parse(gZENGIN_REC2.ZG10)      '処理金額合計
                                    If SFuriCode <> "" Then
                                        '----------------------------------------
                                        ' <RSKJ.INI> 記載あり(RSV2機能使用)
                                        '----------------------------------------
                                        If SFuriCode.IndexOf(SaifuriFMT.InfoMeisaiMast.FURIKETU_CODE.ToString) >= 0 Then
                                            SYORI_KEN += 1                                  '処理件数合計
                                            SYORI_KIN += Long.Parse(gZENGIN_REC2.ZG10)      '処理金額合計
                                        Else
                                            SaifuriFMT.RecordData = ""
                                        End If
                                    Else
                                        '----------------------------------------
                                        ' <RSKJ.INI> 記載なし(RSV1既存機能)
                                        '----------------------------------------
                                        SYORI_KEN += 1                                  '処理件数合計
                                        SYORI_KIN += Long.Parse(gZENGIN_REC2.ZG10)      '処理金額合計
                                    End If
                                    '2016/10/12 ayabe RSV2 CHG --------------------------------------------------------------- END
                            End Select
                        End If

                        If SaifuriFMT.RecordData.Trim <> "" Then
                            ' 書き込み
                            If CODE_KBN = "4" AndAlso Not SaifuriFMT.RecordDataBin Is Nothing Then  'EBCDICのときはバイナリを書き込む
                                SaifuriStream.Write(SaifuriFMT.RecordDataBin, 0, SaifuriFMT.RecordLen)
                                SaifuriFMT.ReadByteBin = Nothing
                            ElseIf CODE_KBN = "1" Then  'JIS120改行あり
                                SaifuriStream.Write(SaifuriFMT.RecordDataToBytes, 0, SaifuriFMT.RecordLen)
                                SaifuriStream.Write(New Byte() {13, 10}, 0, 2)
                            ElseIf CODE_KBN = "2" Then  'JIS119改行あり
                                SaifuriStream.Write(SaifuriFMT.RecordDataToBytes, 0, SaifuriFMT.RecordLen - 1)
                                SaifuriStream.Write(New Byte() {13, 10}, 0, 2)
                            ElseIf CODE_KBN = "3" Then  'JIS118改行あり
                                SaifuriStream.Write(SaifuriFMT.RecordDataToBytes, 0, SaifuriFMT.RecordLen - 2)
                                SaifuriStream.Write(New Byte() {13, 10}, 0, 2)
                            Else 'JIS
                                SaifuriStream.Write(SaifuriFMT.RecordDataToBytes, 0, SaifuriFMT.RecordLen)
                            End If
                        End If
                    Loop
                    OraReader.NextRead()
                End While
            Else
                Return False
            End If
            SaifuriStream.Close()
            SaifuriStream = Nothing

            Return True

        Catch ex As Exception
            '*** Str Add 2016/01/07 SO)荒木 for XMLフォーマット変換対応 ***
            If IsNumeric(FMT_KBN) Then
                Dim nFmtKbn As Integer = CInt(FMT_KBN)
                If nFmtKbn >= 50 AndAlso nFmtKbn <= 99 Then
                    MainLOG.Write_Err("再振データ作成（XMLフォーマット）処理", "失敗", ex)
                    MainLOG.UpdateJOBMASTbyErr("再振データ作成（XMLフォーマット）処理 失敗")
                    JobMessage = "再振データ作成（XMLフォーマット）処理 失敗"
                    Return False
                End If
            End If

            JobMessage = "再振データ作成（全銀）処理 失敗"
            '*** End Add 2016/01/07 SO)荒木 for XMLフォーマット変換対応 ***

            MainLOG.Write("再振データ作成（全銀）処理", "失敗", ex.Message)
            MainLOG.Write("トレース", "成功", CASTCommon.MakeLogTrace)
            MainLOG.UpdateJOBMASTbyErr("再振データ作成（全銀）処理 失敗")
            Return False
        Finally
            If Not OraReader Is Nothing Then OraReader.Close()
        End Try
    End Function

    '初振スケジュールの更新
    Private Function fn_SCHMAST_UPDATE(ByVal Key As KeyInfo) As Boolean
        Dim SQL As New StringBuilder(128)
        Try
            Dim Ret As Integer = 0
            SQL.Append(" UPDATE SCHMAST SET ")
            SQL.Append(" SAIFURI_DATE_S = " & SQ(SFuriDate))
            SQL.Append(" ,SAIFURI_FLG_S = '1'")
            SQL.Append("  WHERE TORIS_CODE_S = " & SQ(Key.TORIS_CODE))
            SQL.Append("  AND TORIF_CODE_S = " & SQ(Key.TORIF_CODE))
            SQL.Append("  AND FURI_DATE_S = " & SQ(FuriDate))

            Ret = MainDB.ExecuteNonQuery(SQL)
            If Not Ret = 1 Then
                MainLOG.Write("スケジュール更新", "失敗", "取引先コード:" & Key.TORIS_CODE & "-" & Key.TORIF_CODE & " 振替日:" & FuriDate)
                JobMessage = "スケジュールの更新に失敗しました。　取引先コード:" & Key.TORIS_CODE & "-" & Key.TORIF_CODE & " 振替日:" & FuriDate
                Return False
            End If

            Return True
        Catch ex As Exception
            MainLOG.Write("スケジュール更新", "失敗", "取引先コード:" & Key.TORIS_CODE & "-" & Key.TORIF_CODE & " エラー内容:" & ex.ToString)
            JobMessage = "スケジュールの更新に失敗しました。　取引先コード:" & Key.TORIS_CODE & "-" & Key.TORIF_CODE & " 振替日:" & FuriDate
            Return False
        End Try
    End Function
End Module
