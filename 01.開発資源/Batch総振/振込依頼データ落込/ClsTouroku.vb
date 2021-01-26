Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CAstSystem
Imports CASTCommon
Imports CAstFormat
'*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

' 個別落し込み処理
Public Class ClsTouroku

    '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
    Private vbDLL As New CMTCTRL.ClsFSKJ
    'Private vbDLL As New FSKJDLL.ClsFSKJ
    '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

    ' 個別起動パラメータ 構造体
    Structure TourokuParam
        Dim CP As CAstBatch.CommData.stPARAMETER

        Public WriteOnly Property Data() As String      '固定長データ処理用プロパティ
            Set(ByVal value As String)
                Dim para() As String = value.Split(","c)
                CP.TORI_CODE = para(0)                      '取引先コード
                CP.FURI_DATE = para(1)                      '振込日
                CP.CODE_KBN = para(2)                       'コード区分
                CP.FMT_KBN = para(3).PadLeft(2, "0"c)       'フォーマット区分
                CP.BAITAI_CODE = para(4).PadLeft(2, "0"c)   '媒体コード
                CP.LABEL_KBN = para(5)                      'ラベル区分

                Select Case para.Length
                    Case 7
                        CP.RENKEI_FILENAME = ""                  ' 連携ファイル名
                        CP.ENC_KBN = ""                          ' 暗号化処理区分
                        CP.ENC_KEY1 = ""                         ' 暗号化キー１
                        CP.ENC_KEY2 = ""                         ' 暗号化キー２
                        CP.ENC_OPT1 = ""                         ' ＡＥＳオプション
                        CP.CYCLENO = ""                          ' サイクル№
                        CP.JOBTUUBAN = Integer.Parse(para(6))    'ジョブ通番
                    Case 8
                        CP.RENKEI_FILENAME = para(6).TrimEnd     ' 連携ファイル名
                        CP.ENC_KBN = ""                          ' 暗号化処理区分
                        CP.ENC_KEY1 = ""                         ' 暗号化キー１
                        CP.ENC_KEY2 = ""                         ' 暗号化キー２
                        CP.ENC_OPT1 = ""                         ' ＡＥＳオプション
                        CP.CYCLENO = ""                          ' サイクル№
                        CP.JOBTUUBAN = Integer.Parse(para(7))    ' ジョブ通番
                    Case 9
                        CP.RENKEI_FILENAME = ""                  ' 連携ファイル名
                        CP.ENC_KBN = ""                          ' 暗号化処理区分
                        CP.ENC_KEY1 = ""                         ' 暗号化キー１
                        CP.ENC_KEY2 = ""                         ' 暗号化キー２
                        CP.ENC_OPT1 = ""                         ' ＡＥＳオプション
                        CP.CYCLENO = ""                          ' サイクル№
                        CP.JOBTUUBAN = Integer.Parse(para(8))    ' ジョブ通番
                    Case 13
                        CP.RENKEI_FILENAME = para(6)             ' 連携ファイル名
                        CP.ENC_KBN = para(7)                     ' 暗号化処理区分
                        CP.ENC_KEY1 = para(8)                    ' 暗号化キー１
                        CP.ENC_KEY2 = para(9)                    ' 暗号化キー２
                        CP.ENC_OPT1 = para(10)                   ' ＡＥＳオプション
                        CP.CYCLENO = para(11)                    ' サイクル№
                        CP.JOBTUUBAN = Integer.Parse(para(12))   ' ジョブ通番
                End Select
            End Set
        End Property
    End Structure
    Private mKoParam As TourokuParam

    Dim mUserID As String = ""                      'ユーザＩＤ
    Dim mJobMessage As String = ""                  'ジョブメッセージ
    Private mDataFileName As String                 '依頼データファイル名
    Private mArgumentData As CommData               '起動パラメータ 共通情報
    Public MainDB As CASTCommon.MyOracle            'パブリックＤＢ
    Private mErrMessage As String = ""              'エラーメッセージ(処理結果確認表印刷用)
    Private ArrayTenbetu As New ArrayList           '店別集計表出力対象 格納リスト

    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Private INI_RSV2_EDITION As String = ""
    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

    ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- START
    Dim INI_RSV2_S_SYORIKEKKA_TOUROKU As String     ' 処理結果確認表印刷要否
    ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- END

    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
    '持込期日チェック(0:チェックしない、1:チェックする)
    Protected INI_MOTIKOMI_KIJITU_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MOTIKOMI_KIJITU_CHK")
    '受付時間チェック(0:チェックしない、1:チェックする)
    Protected INI_UKETUKE_JIKAN_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_JIKAN_CHK")
    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END

    ' 2016/02/08 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
    Private dblock As CASTCommon.CDBLock = Nothing
    Private LockWaitTime As Integer = 600
    ' 2016/02/08 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END

    ' New
    Public Sub New()
    End Sub

    '=================================================================
    ' 機能　 ： 落込主処理　メイン
    ' 引数　 ： command - 起動パラメータ
    ' 戻り値 ： True - 正常，False - 異常
    ' 備考　 ： 
    ' 作成日 ： 2009/09/07
    ' 更新日 ： 
    '=================================================================
    Function Main(ByVal command As String) As Integer

        Try
            '--------------------------------------------
            ' パラメータ取得
            '--------------------------------------------
            'メイン引数設定
            mKoParam.Data = command

            'ジョブ通番設定
            BatchLog.Write("", "0000000000-00", "00000000", "(パラメータ取得)開始", "成功", "")
            BatchLog.JobTuuban = mKoParam.CP.JOBTUUBAN
            BatchLog.ToriCode = mKoParam.CP.TORI_CODE
            BatchLog.FuriDate = mKoParam.CP.FURI_DATE

        Catch ex As Exception
            BatchLog.Write("(パラメータ取得)", "失敗", "パラメータ取得失敗[" & command & "]：" & ex.Message)
            Return 1
        End Try

        Try
            ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
            '--------------------------------------------
            ' INIﾌｧｲﾙ情報設定
            '--------------------------------------------
            INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

            ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- START
            INI_RSV2_S_SYORIKEKKA_TOUROKU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "S_SYORIKEKKA_TOUROKU")
            ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- END

            '--------------------------------------------
            'パラメータ情報設定
            '--------------------------------------------
            Dim bRet As Boolean                         '処理判定
            Dim InfoParam As New CommData.stPARAMETER   'パラメータ情報

            'オラクル
            MainDB = New CASTCommon.MyOracle

            '起動パラメータ共通情報
            mArgumentData = New CommData(MainDB)

            'パラメータ情報を設定
            InfoParam.TORI_CODE = mKoParam.CP.TORI_CODE
            InfoParam.BAITAI_CODE = mKoParam.CP.BAITAI_CODE
            InfoParam.FMT_KBN = mKoParam.CP.FMT_KBN
            InfoParam.FURI_DATE = mKoParam.CP.FURI_DATE
            InfoParam.CODE_KBN = mKoParam.CP.CODE_KBN
            InfoParam.RENKEI_FILENAME = mKoParam.CP.RENKEI_FILENAME
            InfoParam.ENC_KBN = mKoParam.CP.ENC_KBN
            InfoParam.ENC_KEY1 = mKoParam.CP.ENC_KEY1
            InfoParam.ENC_KEY2 = mKoParam.CP.ENC_KEY2
            InfoParam.ENC_OPT1 = mKoParam.CP.ENC_OPT1
            InfoParam.CYCLENO = mKoParam.CP.CYCLENO
            InfoParam.JOBTUUBAN = mKoParam.CP.JOBTUUBAN
            InfoParam.TIME_STAMP = DateTime.Now.ToString("HHmmss")
            mArgumentData.INFOParameter = InfoParam

            '--------------------------------------------
            '落込処理実行
            '--------------------------------------------
            ' 2016/10/17 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
            If INI_RSV2_EDITION = "2" Then
                If dblock Is Nothing Then
                    dblock = New CASTCommon.CDBLock

                    Dim sWorkTime As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
                    If IsNumeric(sWorkTime) Then
                        LockWaitTime = CInt(sWorkTime)
                        If LockWaitTime <= 0 Then
                            LockWaitTime = 600
                        End If
                    End If

                    If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                        BatchLog.Write_Err("主処理", "失敗", "振込依頼データ落込処理で実行待ちタイムアウト")
                        BatchLog.UpdateJOBMASTbyErr("振込依頼データ落込処理で実行待ちタイムアウト")
                        Return -1
                    End If
                End If
            End If
            ' 2016/10/17 タスク）綾部 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END

            bRet = TourokuMain()

            '--------------------------------------------
            '処理判定・帳票出力
            '--------------------------------------------
            If bRet = False Then
                ' 2016/02/08 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                If dblock Is Nothing Then
                    dblock = New CASTCommon.CDBLock

                    Dim sWorkTime As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
                    If IsNumeric(sWorkTime) Then
                        LockWaitTime = CInt(sWorkTime)
                        If LockWaitTime <= 0 Then
                            LockWaitTime = 600
                        End If
                    End If

                    If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                        BatchLog.Write_Err("主処理", "失敗", "振込依頼データ落込処理で実行待ちタイムアウト")
                        BatchLog.UpdateJOBMASTbyErr("振込依頼データ落込処理で実行待ちタイムアウト")
                        Return -1
                    End If
                End If
                ' 2016/02/08 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END
                '処理結果確認表出力（システムエラー)
                Dim oRenkei As New ClsRenkei(mArgumentData)

                Dim Prn As New ClsPrnSyorikekkaKakuninhyo
                If Prn.OutputCSVKekkaSysError(mKoParam.CP.FSYORI_KBN, _
                        mKoParam.CP.TORIS_CODE, mKoParam.CP.TORIF_CODE, _
                        mKoParam.CP.JOBTUUBAN, Path.GetFileName(mKoParam.CP.RENKEI_FILENAME), mErrMessage, MainDB) = False Then
                    BatchLog.Write("処理結果確認表", "失敗", "ファイル名:" & Prn.FileName & " メッセージ:" & Prn.ReportMessage)
                End If

                Prn = Nothing

            Else
                '処理正常終了
                Dim DestFile As String = ""
                '2017/05/08 タスク）西野 ADD 標準版修正（依頼データの退避設定）---------------------- START
                Dim IRAI_BK_MODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "IRAI_BKUP_MODE_S")
                '2017/05/08 タスク）西野 ADD 標準版修正（依頼データの退避設定）---------------------- END
                Try
                    Select Case InfoParam.BAITAI_CODE
                        ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- START
                        'Case "00"
                        Case "00", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                            ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- END
                            '他システム連携から起動された場合を考慮する
                            Dim R_FILENAME As String = Path.Combine(Path.GetDirectoryName(mKoParam.CP.RENKEI_FILENAME), InfoParam.RENKEI_FILENAME)

                            If File.Exists(R_FILENAME) = True Then
                                '他システム連携の場合のファイル処理
                                DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(R_FILENAME)

                                '前回ファイルを削除
                                If File.Exists(DestFile) = True Then
                                    File.Delete(DestFile)
                                End If

                                '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- START
                                If IRAI_BK_MODE = "1" Then
                                    File.Copy(R_FILENAME, DestFile, True)
                                    File.Delete(R_FILENAME)
                                Else
                                    File.Move(R_FILENAME, DestFile)
                                End If
                                'File.Move(R_FILENAME, DestFile)
                                '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- END

                            Else
                                '通常の場合のファイル処理
                                DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                                '前回ファイルを削除
                                If File.Exists(DestFile) = True Then
                                    File.Delete(DestFile)
                                End If

                                '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- START
                                If IRAI_BK_MODE = "1" Then
                                    File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                    File.Delete(mKoParam.CP.RENKEI_FILENAME)
                                Else
                                    File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                End If
                                'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- END

                            End If

                            '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                        Case "01", "11", "12", "13", "14", "15"
                            'Case "01"
                            '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<

                            DestFile = CASTCommon.GetFSKJIni("COMMON", "DATBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                            '前回ファイルを削除
                            If File.Exists(DestFile) = True Then
                                File.Delete(DestFile)
                            End If

                            '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- START
                            If IRAI_BK_MODE = "1" Then
                                File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                File.Delete(mKoParam.CP.RENKEI_FILENAME)
                            Else
                                File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                            End If
                            'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                            '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- END

                        Case "04", "09" '(依頼書・伝票を追加)
                            ' 2016/01/27 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                            'DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & "E" & mKoParam.CP.TORI_CODE & ".DAT"
                            ''前回ファイルを削除
                            'If File.Exists(DestFile) = True Then
                            '    File.Delete(DestFile)
                            'End If
                            'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                            ''2009/12/09 追加 ================================
                            '前回ファイルを削除
                            Select Case INI_RSV2_EDITION
                                Case "2"
                                    DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)
                                    If File.Exists(DestFile) = True Then
                                        File.Delete(DestFile)
                                    End If
                                    '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- START
                                    If IRAI_BK_MODE = "1" Then
                                        File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                        File.Delete(mKoParam.CP.RENKEI_FILENAME)
                                    Else
                                        File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                    End If
                                    'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                    '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- END
                                Case Else
                                    DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & "E" & mKoParam.CP.TORI_CODE & ".DAT"
                                    If File.Exists(DestFile) = True Then
                                        File.Delete(DestFile)
                                    End If
                                    '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- START
                                    If IRAI_BK_MODE = "1" Then
                                        File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                        File.Delete(mKoParam.CP.RENKEI_FILENAME)
                                    Else
                                        File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                    End If
                                    'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                    '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- END
                            End Select
                            ' 2016/01/27 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END
                        Case "05"
                            If CASTCommon.GetFSKJIni("COMMON", "MT") = "1" Then '未接続
                                DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)
                                '前回ファイルを削除
                                If File.Exists(DestFile) = True Then
                                    File.Delete(DestFile)
                                End If
                                '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- START
                                If IRAI_BK_MODE = "1" Then
                                    File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                    File.Delete(mKoParam.CP.RENKEI_FILENAME)
                                Else
                                    File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                End If
                                'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- END
                            End If
                        Case "06"
                            If CASTCommon.GetFSKJIni("COMMON", "CMT") = "1" Then '未接続
                                DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)
                                '前回ファイルを削除
                                If File.Exists(DestFile) = True Then
                                    File.Delete(DestFile)
                                End If
                                '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- START
                                If IRAI_BK_MODE = "1" Then
                                    File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                    File.Delete(mKoParam.CP.RENKEI_FILENAME)
                                Else
                                    File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                End If
                                'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                                '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- END
                            End If
                            '===============================================
                            '2012/06/30 標準版　WEB伝送対応
                        Case "10"
                            DestFile = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV_BK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                            '前回ファイルを削除
                            If File.Exists(DestFile) = True Then
                                File.Delete(DestFile)
                            End If

                            '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- START
                            If IRAI_BK_MODE = "1" Then
                                File.Copy(mKoParam.CP.RENKEI_FILENAME, DestFile, True)
                                File.Delete(mKoParam.CP.RENKEI_FILENAME)
                            Else
                                File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                            End If
                            'File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)
                            '2017/05/08 タスク）西野 CHG 標準版修正（依頼データの退避設定）---------------------- END
                    End Select
                Catch ex As Exception
                    BatchLog.Write("正常フォルダ移動", "失敗", mKoParam.CP.RENKEI_FILENAME & " -> " & DestFile)
                End Try
            End If

            MainDB.Close()

            If bRet = True Then
                '総振標準で店別集計表は未対応
                '総合振込店別集計表
                'Dim ExeRepo As New CAstReports.ClsExecute 'レポエージェント印刷
                'For i As Integer = 0 To ArrayTenbetu.Count - 1
                '    Dim prnTenbetu() As String = CStr(ArrayTenbetu(i)).Split(","c)
                '    Dim Param As String = prnTenbetu(1) & prnTenbetu(2) & "," & mArgumentData.INFOParameter.FURI_DATE & ",0"
                '    Dim nRet As Integer = ExeRepo.ExecReport("KFSPxxx.EXE", Param)
                '    If nRet <> 0 Then
                '        BatchLog.Write("総合振込店別集計表出力", "失敗", "復帰値：" & nRet)
                '    End If
                'Next
                ' 2016/01/28 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                Dim readFmt As CAstFormat.CFormat = CAstFormat.CFormat.GetFormat(mArgumentData.INFOParameter)
                Dim ToriCode(ArrayTenbetu.Count - 1) As String
                For i As Integer = 0 To ArrayTenbetu.Count - 1
                    Dim prnTenbetu() As String = CStr(ArrayTenbetu(i)).Split(","c)
                    ToriCode(i) = prnTenbetu(1) & prnTenbetu(2)
                Next

                If readFmt.CallTourokuExit(ToriCode, mArgumentData.INFOParameter.FURI_DATE) = False Then
                    BatchLog.Write("落し込み用登録出口メソッド処理", "失敗", "")
                End If
                readFmt.Close()
                ' 2016/01/28 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END
            End If
            BatchLog.Write("終了", "成功", bRet.ToString)

            If bRet = False Then
                Return 2
            End If

            Return 0

        Catch ex As Exception
            BatchLog.Write("(落し込みメイン処理)", "失敗", ex.Message)
        Finally
            ' 2016/02/08 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
            ' ジョブ実行アンロック
            System.Threading.Thread.Sleep(100)
            dblock.Job_UnLock(MainDB)
            ' 2016/02/08 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END
        End Try
    End Function

    '=================================================================
    ' 機能　 ： 登録メイン処理
    ' 引数　 ： なし
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    ' 備考　 ： 
    ' 作成日 ： 2009/09/08
    ' 更新日 ：
    '=================================================================
    Private Function TourokuMain() As Boolean
        Try
            BatchLog.Write("(登録メイン処理)開始", "成功")

            '--------------------------------------------
            '取引先情報取得
            '--------------------------------------------
            Try
                '取引先情報取得
                Call mArgumentData.SelectTORIMAST("3", mKoParam.CP.TORIS_CODE, mKoParam.CP.TORIF_CODE)

                If mArgumentData.INFOToriMast.EOF = True Then
                    Dim line1 As String = ""

                    Try
                        Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData)
                        Dim readFmt As CAstFormat.CFormat = CAstFormat.CFormat.GetFormat(mArgumentData.INFOParameter)

                        '各フォーマットごとに値を取得／設定
                        If readFmt.FirstRead(oRenkei.InFileName) = 1 Then
                            readFmt.GetFileData()
                            line1 = readFmt.RecordData
                            readFmt.Close()
                        Else
                            BatchLog.Write(readFmt.Message, "失敗", oRenkei.InFileName)

                            readFmt.Close()

                            Dim ReadF As New FileStream(oRenkei.InFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                            Dim Arr(100) As Byte

                            ReadF.Read(Arr, 0, 100)
                            line1 = Encoding.GetEncoding("SHIFT-JIS").GetString(Arr)
                            ReadF.Close()
                        End If

                    Catch ex As Exception
                        line1 = ex.Message
                    End Try
                    BatchLog.Write("(登録メイン処理)", "失敗", "取引先情報取得：" & line1)
                    BatchLog.UpdateJOBMASTbyErr("取引先情報取得失敗 " & line1)
                    mErrMessage = "取引先情報取得失敗"
                    Return False
                End If

            Catch ex As Exception
                BatchLog.Write("(登録メイン処理)", "失敗", ex.Message)
                BatchLog.UpdateJOBMASTbyErr("取引先情報取得失敗")
                mErrMessage = "取引先情報取得失敗"
                Return False
            End Try

            '--------------------------------------------
            ' パラメータ情報を設定
            '--------------------------------------------
            Dim InfoParam As CommData.stPARAMETER = mArgumentData.INFOParameter

            InfoParam.FSYORI_KBN = mArgumentData.INFOToriMast.FSYORI_KBN_T
            mArgumentData.INFOParameter = InfoParam

            'パラメータのチェック（年金フォーマット以外）
            If mKoParam.CP.FMT_KBN <> "03" Then
                If CheckParameter() = False Then
                    Return False
                End If
            End If

            '--------------------------------------------
            '媒体読み込み（フォーマット　）
            '--------------------------------------------
            Dim oReadFMT As CAstFormat.CFormat
            Dim sReadFile As String = ""

            Try
                'フォーマット区分から，フォーマットを特定する
                oReadFMT = CFormat.GetFormat(mArgumentData.INFOParameter)

                If oReadFMT Is Nothing Then
                    BatchLog.Write("(登録メイン処理)", "失敗", "フォーマット取得（規定外フォーマット）")
                    '*** Str Add 2016/3/5 sys)mori for メッセージ修正 ***
                    BatchLog.UpdateJOBMASTbyErr("フォーマット取得失敗（規定外フォーマット）")
                    'BatchLog.UpdateJOBMASTbyErr("フォーマット取得（規定外フォーマット）")
                    '*** end Add 2016/3/5 sys)mori for メッセージ修正 ***
                    mErrMessage = "フォーマット取得失敗"
                    Return False
                End If

            Catch ex As Exception
                BatchLog.Write("(登録メイン処理)", "失敗", "フォーマット取得：" & ex.Message)
                '*** Str Add 2016/3/5 sys)mori for メッセージ修正 ***
                BatchLog.UpdateJOBMASTbyErr("フォーマット取得失敗")
                'BatchLog.UpdateJOBMASTbyErr("フォーマット取得")
                '*** end Add 2016/3/5 sys)mori for メッセージ修正 ***
                mErrMessage = "フォーマット取得失敗"
                Return False
            End Try

            'SJIS 改行なしファイルに変換して読み込む(媒体読込の実行)
            sReadFile = ReadMediaData(oReadFMT)
            ' 2016/05/16 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応_岐阜機能調査漏れ) -------------------- START
            'If sReadFile = "" Then
            '    oReadFMT.Close()
            '    BatchLog.Write("(登録メイン処理)", "失敗", "媒体読み込み：" & oReadFMT.Message)
            '    Return False
            'End If
            If sReadFile = "" Then
                oReadFMT.Close()
                BatchLog.Write("(登録メイン処理)", "失敗", "媒体読み込み：" & oReadFMT.Message)
                Return False
            Else
                '--------------------------------------------
                ' 振込依頼データ(中間ファイル)の退避
                '--------------------------------------------
                Dim BKUP_CHECK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_S_CHECK_IN")
                If BKUP_CHECK <> "err" Then
                    If BKUP_CHECK = "1" Then
                        Dim BKUP_BAITAI As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_S_BAITAI_IN")

                        If BKUP_BAITAI.IndexOf(mKoParam.CP.BAITAI_CODE) >= 0 Then
                            Dim BKUP_FOLDER As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_S_FOLDER_IN")
                            Dim BKUP_FILENAME As String = BKUP_FOLDER & Path.GetFileNameWithoutExtension(sReadFile) & "_" & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & ".DAT"

                            Try
                                File.Copy(sReadFile, BKUP_FILENAME, True)
                            Catch ex As Exception
                                BatchLog.Write("(登録メイン処理)", "失敗", "振込依頼データ退避：" & ex.Message)
                            End Try
                        End If
                    End If
                End If
            End If
            ' 2016/05/16 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応_岐阜機能調査漏れ) -------------------- END

            '--------------------------------------------
            'データチェック
            '--------------------------------------------
            '伝送，ＦＤ，ＭＴ，ＣＭＴ
            '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
            Select Case mKoParam.CP.BAITAI_CODE
                ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- START
                'Case "00", "01", "05", "06", "04", "09", "10", "SA", "11", "12", "13", "14", "15"
                Case "00", "01", "05", "06", "04", "09", "10", "SA", "11", "12", "13", "14", "15", _
                     "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                    ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- END
                    Dim nPos As Long = -1
                    Dim nLine As Long = 0
                    Dim nErrorCount As Long = 0
                    Dim FirstError As String = ""

                    '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ START
                    If mKoParam.CP.CODE_KBN = "2" OrElse mKoParam.CP.CODE_KBN = "3" Then
                        '中間ファイルのレコード長が120バイトなので改行なしのフォーマットで開きなおす
                        oReadFMT = New CAstFormat.CFormatFurikomi
                    End If
                    '2018/03/05 タスク）西野 ADD 標準版修正：広島信金対応（不具合修正）------------------------ END

                    oReadFMT.FirstRead(sReadFile)

                    Do Until oReadFMT.EOF
                        nLine += 1

                        '1レコード　データ取得
                        If oReadFMT.GetFileData() = 0 Then
                        End If

                        '*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                        Try
                            '*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

                            '規定文字チェック
                            nPos = oReadFMT.CheckRegularString()

                            '*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                        Catch ex As Exception
                            oReadFMT.Close()
                            BatchLog.Write_Err("(登録メイン処理)", "失敗", ex)
                            BatchLog.UpdateJOBMASTbyErr("フォーマット変換失敗")
                            mErrMessage = "フォーマット変換失敗"
                            Return False
                        End Try
                        '*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

                        If nPos >= 0 Then
                            nErrorCount += 1
                            BatchLog.Write("(登録メイン処理)", "成功", "規定外文字あり：" & _
                                                nLine.ToString & "行目の" & (nPos + 1).ToString & "バイト目に不正な文字が入っています")

                            If FirstError Is Nothing Then
                                FirstError = nLine.ToString & "行目の" & (nPos + 1).ToString & "バイト目に不正な文字が入っています"
                            End If

                            If nErrorCount >= 10 Then
                                Exit Do
                            End If
                        End If
                    Loop

                    oReadFMT.Close()

                Case Else

            End Select

            'If mKoParam.CP.BAITAI_CODE = "00" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "01" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "05" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "06" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "04" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "09" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "10" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "SA" Then '媒体"04","09","SA"追加
            '    '2012/06/30 標準版　WEB伝送対応　媒体"10"追加

            '    Dim nPos As Long = -1
            '    Dim nLine As Long = 0
            '    Dim nErrorCount As Long = 0
            '    Dim FirstError As String = ""

            '    oReadFMT.FirstRead(sReadFile)

            '    Do Until oReadFMT.EOF
            '        nLine += 1

            '        '1レコード　データ取得
            '        If oReadFMT.GetFileData() = 0 Then
            '        End If

            '        '規定文字チェック
            '        nPos = oReadFMT.CheckRegularString()

            '        If nPos >= 0 Then
            '            nErrorCount += 1
            '            BatchLog.Write("(登録メイン処理)", "成功", "規定外文字あり：" & _
            '                                nLine.ToString & "行目の" & (nPos + 1).ToString & "バイト目に不正な文字が入っています")

            '            If FirstError Is Nothing Then
            '                FirstError = nLine.ToString & "行目の" & (nPos + 1).ToString & "バイト目に不正な文字が入っています"
            '            End If

            '            If nErrorCount >= 10 Then
            '                Exit Do
            '            End If
            '        End If
            '    Loop

            '    oReadFMT.Close()

            'End If
            '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<

            '--------------------------------------------
            ' 明細マスタ登録処理
            '--------------------------------------------
            If MakingMeiMast(oReadFMT) = False Then
                oReadFMT.Close()

                If File.Exists(sReadFile) = True Then
                    File.Delete(sReadFile)
                End If

                ' ロールバック
                MainDB.Rollback()
                Return False
            End If

            '2010/02/18 依頼書の場合、対応する取引先の依頼書の新規コードを1から0に更新する
            If mKoParam.CP.BAITAI_CODE = "04" Then
                If UpdateENTMAST() = False Then
                    oReadFMT.Close()
                    ' ロールバック
                    MainDB.Rollback()
                    Return False
                End If
            End If

            oReadFMT.Close()
            If File.Exists(sReadFile) = True Then
                File.Delete(sReadFile)
            End If

            ' JOBマスタ更新
            If BatchLog.UpdateJOBMASTbyOK(mJobMessage) = False Then
                ' ロールバック
                MainDB.Rollback()
                Return False
            End If

            ' ＤＢコミット
            MainDB.Commit()

            Return True

        Catch ex As Exception
            BatchLog.Write("(登録メイン処理)", "失敗", ex.Message)
            Return False
        Finally
            BatchLog.Write("(登録メイン処理)終了", "成功")
        End Try
    End Function

    '=================================================================
    ' 機能　 ： パラメータチェック
    ' 引数　 ： なし
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    ' 備考　 ： 
    ' 作成日 ： 2009/09/08
    ' 更新日 ： 
    '=================================================================
    Function CheckParameter() As Boolean
        Dim SQL As StringBuilder = New StringBuilder(256)

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
        Dim LockWaitTime As Integer = 600
        Dim sWork As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
        If IsNumeric(sWork) Then
            LockWaitTime = CInt(sWork)
            If LockWaitTime <= 0 Then
                LockWaitTime = 600
            End If
        End If
        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

        Try
            BatchLog.Write("(パラメータチェック)開始", "成功")

            '---------------------------------------------------------
            'フォーマット区分のチェック
            '---------------------------------------------------------
            If mKoParam.CP.FMT_KBN <> mArgumentData.INFOToriMast.FMT_KBN_T Then
                BatchLog.Write("(パラメータチェック)", "失敗", "フォーマット区分異常")
                Call BatchLog.UpdateJOBMASTbyErr("取引先マスタ登録異常：フォーマット区分")
                mErrMessage = "フォーマット区分異常"
                Return False
            End If

            '---------------------------------------------------------
            '媒体コードのチェック
            '---------------------------------------------------------
            '再振の場合はチェックしない 2009/10/21 追加
            If mKoParam.CP.BAITAI_CODE <> mArgumentData.INFOToriMast.BAITAI_CODE_T _
                 AndAlso mKoParam.CP.BAITAI_CODE <> "SA" Then
                BatchLog.Write("(パラメータチェック)", "失敗", "媒体コード異常")
                Call BatchLog.UpdateJOBMASTbyErr("取引先マスタ登録異常：媒体コード")
                mErrMessage = "媒体コード異常"
                Return False
            End If

            '---------------------------------------------------------
            'コード区分のチェック
            '---------------------------------------------------------
            If mKoParam.CP.CODE_KBN <> mArgumentData.INFOToriMast.CODE_KBN_T Then
                BatchLog.Write("(パラメータチェック)", "失敗", "コード区分異常")
                Call BatchLog.UpdateJOBMASTbyErr("取引先マスタ登録異常：コード区分")
                mErrMessage = "コード区分異常"
                Return False
            End If

            ' 依頼書・伝票でない場合、複数回持込あり かつ 期日管理しない場合はスケジュールの存在チェックを行わない
            If mArgumentData.INFOToriMast.BAITAI_CODE_T <> "04" AndAlso mArgumentData.INFOToriMast.BAITAI_CODE_T <> "09" Then
                If mArgumentData.INFOToriMast.CYCLE_T = "1" AndAlso mArgumentData.INFOToriMast.KIJITU_KANRI_T = "0" Then
                    Return True
                End If
            End If

            '---------------------------------------------------------
            'スケジュールの有無のチェック／落し込み処理未処理の確認
            '---------------------------------------------------------
            Dim oReader As CASTCommon.MyOracleReader = Nothing

            SQL.Append("SELECT ")
            SQL.Append(" TYUUDAN_FLG_S")
            SQL.Append(",TOUROKU_FLG_S")
            SQL.Append(" FROM S_SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(mKoParam.CP.TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_S = " & SQ(mKoParam.CP.TORIF_CODE))
            SQL.Append(" AND FURI_DATE_S  = " & SQ(mKoParam.CP.FURI_DATE))

            Try
                oReader = New CASTCommon.MyOracleReader(MainDB)

                If oReader.DataReader(SQL) = False Then
                    ' 複数回持込なし かつ 期日管理しない場合
                    If mArgumentData.INFOToriMast.CYCLE_T = "0" AndAlso mArgumentData.INFOToriMast.KIJITU_KANRI_T = "0" Then
                        Return True
                    End If
                    BatchLog.Write("スケジュールなし", "失敗", " 振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                    Call BatchLog.UpdateJOBMASTbyErr("スケジュールなし 振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                    mErrMessage = "スケジュールなし"
                    Return False
                Else
                    If mArgumentData.INFOToriMast.CYCLE_T = "0" AndAlso mArgumentData.INFOToriMast.KIJITU_KANRI_T = "0" Then
                        ' 期日管理なしの場合で，複数回持ち込みもない場合，
                        ' 既存スケジュールが存在する場合エラー
                        Do While oReader.EOF = False
                            '登録フラグ／中断フラグの確認
                            If oReader.GetString("TOUROKU_FLG_S") <> "0" AndAlso oReader.GetString("TYUUDAN_FLG_S") = "0" Then
                                BatchLog.Write("スケジュール", "失敗", "落し込み処理済 振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                                Call BatchLog.UpdateJOBMASTbyErr("スケジュール:落し込み処理済 振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                                mErrMessage = "落し込み処理済"
                            End If
                            oReader.NextRead()
                        Loop

                        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                        'Return True
                        Return SCHMAST_Lock(oReader, LockWaitTime)
                        '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                    End If
                End If

                If oReader.GetString("TYUUDAN_FLG_S") <> "0" Then
                    BatchLog.Write("スケジュール", "失敗", "中断フラグ設定済 振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"))
                    Call BatchLog.UpdateJOBMASTbyErr("スケジュール:中断フラグ設定済 振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"))
                    mErrMessage = "中断フラグ設定済み"
                    Return False
                End If

                If oReader.GetString("TOUROKU_FLG_S") <> "0" Then
                    BatchLog.Write("スケジュール", "失敗", "落し込み処理済 振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & "取引先コード：" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    Call BatchLog.UpdateJOBMASTbyErr("スケジュール:落し込み処理済 振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    mErrMessage = "落し込み処理済"
                    Return False
                End If

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                'Return True
                Return SCHMAST_Lock(oReader, LockWaitTime)
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            Catch ex As Exception
                BatchLog.Write("スケジュール検索", "失敗", "振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"))
                Call BatchLog.UpdateJOBMASTbyErr("スケジュール検索失敗 振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"))
                mErrMessage = "スケジュール検索失敗"
                Return False

            Finally
                If Not oReader Is Nothing Then
                    oReader.Close()
                End If
            End Try

            Return True

        Catch ex As Exception
            Call BatchLog.UpdateJOBMASTbyErr("パラメータチェック：システムエラー（ログ参照）")
            BatchLog.Write("(パラメータチェック)", "失敗", ex.Message)
            mErrMessage = "パラメータチェック失敗"
            Return False
        Finally
            BatchLog.Write("(パラメータチェック)終了", "成功")
        End Try
    End Function


    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
    Private Function SCHMAST_Lock(ByRef oReader As CASTCommon.MyOracleReader, ByVal LockWaitTime As Integer) As Boolean

        Dim SQL As StringBuilder = New StringBuilder(256)

        ' 代表委託者コード取得
        SQL.Append("SELECT ")
        SQL.Append(" ITAKU_KANRI_CODE_T")
        SQL.Append(" FROM S_TORIMAST")
        SQL.Append(" WHERE TORIS_CODE_T = " & SQ(mKoParam.CP.TORIS_CODE))
        SQL.Append(" AND TORIF_CODE_T = " & SQ(mKoParam.CP.TORIF_CODE))
        SQL.Append(" AND MULTI_KBN_T  = '1'")

        If oReader.DataReader(SQL) = True Then
            Dim itaku_kanri_code As String = oReader.GetString("ITAKU_KANRI_CODE_T")

            oReader.Close()

            ' マルチ区分時のスケジュールロック
            SQL = New StringBuilder(256)
            SQL.Append("SELECT FURI_DATE_S")
            SQL.Append(" FROM S_SCHMAST , S_TORIMAST")
            SQL.Append(" WHERE ITAKU_KANRI_CODE_T = " & SQ(itaku_kanri_code))
            SQL.Append(" AND TORIS_CODE_T       = TORIS_CODE_S")
            SQL.Append(" AND TORIF_CODE_T       = TORIF_CODE_S")
            SQL.Append(" AND FURI_DATE_S        = " & SQ(mKoParam.CP.FURI_DATE))
            SQL.Append(" AND TOUROKU_FLG_S      = '0'")
            SQL.Append(" AND TYUUDAN_FLG_S      = '0'")
            SQL.Append(" FOR UPDATE OF S_SCHMAST.TORIS_CODE_S WAIT " & LockWaitTime)
            If oReader.DataReader(SQL) = False Then
                If oReader.Message <> "" Then
                    Dim errmsg As String
                    If oReader.Message.StartsWith("ORA-30006") Then
                        errmsg = "振込依頼データ落込処理で実行待ちタイムアウト"
                    Else
                        errmsg = "振込依頼データ落込処理ロック異常"
                    End If

                    BatchLog.Write_Err("落し込み処理", "失敗", errmsg & " 振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"))
                    Call BatchLog.UpdateJOBMASTbyErr(errmsg & " 振込日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"))
                    mErrMessage = errmsg
                    Return False
                End If
            End If
        End If

        Return True

    End Function
    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***


    '=================================================================
    ' 機能　 ： 依頼データ読込処理
    ' 引数　 ： readfmt - フォーマットクラス
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    ' 備考　 ： 
    ' 作成日 ： 2009/09/08
    ' 更新日 ： 
    '=================================================================
    Public Function ReadMediaData(ByVal readfmt As CAstFormat.CFormat) As String
        Dim nRetRead As Integer = 0     '媒体読み込み結果
        Dim FTranName As String         'F*TRANファイル名
        Dim strKekka As String
        Dim MediaRead As New clsMediaRead

        '2010/04/26 パラメータファイル名を考慮する
        Dim ParamInFileName As String = Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

        Try
            BatchLog.Write("(依頼データ読込)開始", "成功")

            '依頼データ読込／フォーマットチェック実行
            Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData) '連携

            '------------------------------------------------------------
            '媒体読み
            '------------------------------------------------------------
            With mKoParam.CP

                '中間ファイル作成先指定
                oRenkei.OutFileName = CASTCommon.GetFSKJIni("COMMON", "DATBK") & "i" & .TORI_CODE & ".DAT"

                'F*TRANファイル名を取得
                If .CODE_KBN = "4" And .BAITAI_CODE = "01" Then
                    'FTranName = readfmt.FTRANIBMP
                    FTranName = readfmt.FTRANIBMBINARYP
                Else
                    FTranName = readfmt.FTRANP
                End If

                '------------------------------------------------------------
                '媒体別　ファイル取込実行
                '------------------------------------------------------------
                '2009/12/09 CMT/MT 未接続対応
                Dim Baitai_Code As String
                Select Case mKoParam.CP.BAITAI_CODE
                    Case "05"
                        If CASTCommon.GetFSKJIni("COMMON", "MT") = "1" Then 'MT未接続
                            Baitai_Code = "MT"  '代用媒体コード
                        Else
                            Baitai_Code = mKoParam.CP.BAITAI_CODE
                        End If
                    Case "06"
                        If CASTCommon.GetFSKJIni("COMMON", "CMT") = "1" Then 'CMT未接続
                            Baitai_Code = "CMT"  '代用媒体コード
                        Else
                            Baitai_Code = mKoParam.CP.BAITAI_CODE
                        End If
                    Case Else
                        Baitai_Code = mKoParam.CP.BAITAI_CODE
                End Select
                'Select mKoParam.CP.BAITAI_CODE
                Select Case Baitai_Code
                    '===================================
                    Case "01"       '３．５ＦＤ

                        Dim REC_LENGTH As Integer
                        Select Case .CODE_KBN
                            Case "2"
                                '119バイトＪＩＳ改行あり
                                REC_LENGTH = 119
                            Case "3"
                                '118バイトＪＩＳ改行あり
                                REC_LENGTH = 118
                            Case Else
                                'その他
                                REC_LENGTH = readfmt.RecordLen
                        End Select

                        'ＦＤファイル読込処理実行

                        ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                        'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", .TORI_CODE))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                        'Else
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        'End If

                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                        ''2010/04/26 パラメータファイル名を考慮する
                        ''nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                        'If ParamInFileName = "" Then
                        '    nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                        'Else
                        '    nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, ParamInFileName, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                        'End If

                        ''2009/11/30 コード区分チェック追加===
                        'If nRetRead = 0 Then
                        '    Dim ErrMessage As String = ""
                        '    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                        '        Case 0  '異常なし
                        '        Case 10 '10：Shift-jis
                        '            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                        '            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                        '            mErrMessage = "データファイル コード区分異常(JIS)"
                        '            Return ""
                        '        Case 20 ' 20:EBCDIC異常
                        '            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                        '            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                        '            mErrMessage = "データファイル コード区分異常(EBCDIC)"
                        '            Return ""
                        '        Case -1 '処理失敗
                        '            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：システムエラー（ログ参照）")
                        '            BatchLog.Write("(依頼データ読込)", "失敗", ErrMessage)
                        '            mErrMessage = "システムエラー(ログ参照)"
                        '            Return ""
                        '    End Select
                        'End If
                        ''===================================
                        Select Case INI_RSV2_EDITION
                            Case "2"
                                Dim FileName_Edition2 As String = ""
                                If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                    BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：ファイル名構築失敗:")
                                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル名構築失敗:")
                                    mErrMessage = "ファイル名構築失敗"
                                    Return ""
                                End If
                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "BAITAIREAD"), FileName_Edition2)


                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                '依頼ファイルをコピーする前に、依頼ファイルの読み取り専用属性を解除する
                                If File.Exists(mKoParam.CP.RENKEI_FILENAME) = True Then
                                    Dim fiCopyFile As New System.IO.FileInfo(mKoParam.CP.RENKEI_FILENAME)
                                    If (fiCopyFile.Attributes And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                                        fiCopyFile.Attributes = FileAttributes.Normal
                                    End If
                                End If

                                nRetRead = oRenkei.CopyToDisk(readfmt)
                                If nRetRead = 0 And Baitai_Code <> "SA" Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '異常なし
                                        Case 10 '10：Shift-jis
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC異常
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(EBCDIC)"
                                            Return ""
                                        Case -1 '処理失敗
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：システムエラー（ログ参照）")
                                            BatchLog.Write("(依頼データ読込)", "失敗", ErrMessage)
                                            mErrMessage = "システムエラー(ログ参照)"
                                            Return ""
                                    End Select
                                End If
                            Case Else
                                If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                                Else
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                                End If

                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                If ParamInFileName = "" Then
                                    nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                                Else
                                    nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, ParamInFileName, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                                End If

                                If nRetRead = 0 Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '異常なし
                                        Case 10 '10：Shift-jis
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC異常
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(EBCDIC)"
                                            Return ""
                                        Case -1 '処理失敗
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：システムエラー（ログ参照）")
                                            BatchLog.Write("(依頼データ読込)", "失敗", ErrMessage)
                                            mErrMessage = "システムエラー(ログ参照)"
                                            Return ""
                                    End Select
                                End If
                        End Select
                        ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

                        '------------------------------------------------------------
                        '元の文字コードのデータを再変換して処理に乗せる
                        '------------------------------------------------------------
                        If nRetRead = 0 Then
                            nRetRead = oRenkei.CopyToDisk(readfmt)
                        End If

                    Case "05"       'ＭＴ

                        '2018/10/05 saitou 広島信金(RSV2標準) UPD ---------------------------------------- START
                        Select Case INI_RSV2_EDITION
                            Case "2"
                                Dim FileName_Edition2 As String = ""
                                If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                    BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：ファイル名構築失敗")
                                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル名構築失敗:" & Baitai_Code & "-" & FileName_Edition2)
                                    mErrMessage = "ファイル名構築失敗"
                                    Return ""
                                End If
                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "BAITAIREAD"), FileName_Edition2)

                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                nRetRead = oRenkei.CopyToDisk(readfmt)
                                If nRetRead = 0 And Baitai_Code <> "SA" Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '異常なし
                                        Case 10 '10：Shift-jis
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC異常
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(EBCDIC)"
                                            Return ""
                                        Case -1 '処理失敗
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：システムエラー（ログ参照）")
                                            BatchLog.Write("(依頼データ読込)", "失敗", ErrMessage)
                                            mErrMessage = "システムエラー(ログ参照)"
                                            Return ""
                                    End Select
                                End If

                            Case Else
                                'ＭＴファイル読込処理実行
                                strKekka = vbDLL.cmtCPYtoDISK(readfmt.BLOCKSIZE, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)

                                '媒体読込失敗：処理終了
                                If strKekka <> "" Then
                                    BatchLog.Write("(依頼データ読込)", "失敗", "ＭＴ読込：" & oRenkei.Message)
                                    BatchLog.UpdateJOBMASTbyErr("ＭＴ読込失敗、ファイル名：" & oRenkei.InFileName)
                                    Return ""
                                End If

                                '------------------------------------------------------------
                                '元の文字コードのデータを再変換して処理に乗せる
                                '------------------------------------------------------------
                                If nRetRead = 0 Then
                                    nRetRead = oRenkei.CopyToDisk(readfmt)
                                End If
                        End Select

                        ''****ばいなり未対応 2009.10.23

                        ''2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                        ''ＭＴファイル読込処理実行
                        'strKekka = vbDLL.cmtCPYtoDISK(readfmt.BLOCKSIZE, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        ''Dim intblk_size As Short = CShort(readfmt.BLOCKSIZE)
                        ''Dim shtREC_LENGTH As Short = CShort(readfmt.RecordLen)
                        ''Dim shtLABEL_KBN As Short = CShort(.LABEL_KBN)

                        ''ＭＴファイル読込処理実行
                        ''strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        ''2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                        ''媒体読込失敗：処理終了
                        'If strKekka <> "" Then
                        '    BatchLog.Write("(依頼データ読込)", "失敗", "ＭＴ読込：" & oRenkei.Message)
                        '    BatchLog.UpdateJOBMASTbyErr("ＭＴ読込失敗、ファイル名：" & oRenkei.InFileName)
                        '    Return ""
                        'End If

                        ''**************************
                        ''------------------------------------------------------------
                        ''元の文字コードのデータを再変換して処理に乗せる
                        ''------------------------------------------------------------
                        'If nRetRead = 0 Then
                        '    nRetRead = oRenkei.CopyToDisk(readfmt)
                        'End If
                        ''**************************
                        '2018/10/05 saitou 広島信金(RSV2標準) UPD ---------------------------------------- END

                    Case "06"       'ＣＭＴ

                        '2018/10/05 saitou 広島信金(RSV2標準) UPD ---------------------------------------- START
                        Select Case INI_RSV2_EDITION
                            Case "2"
                                Dim FileName_Edition2 As String = ""
                                If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                    BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：ファイル名構築失敗")
                                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル名構築失敗:" & Baitai_Code & "-" & FileName_Edition2)
                                    mErrMessage = "ファイル名構築失敗"
                                    Return ""
                                End If
                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "BAITAIREAD"), FileName_Edition2)

                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                nRetRead = oRenkei.CopyToDisk(readfmt)
                                If nRetRead = 0 And Baitai_Code <> "SA" Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '異常なし
                                        Case 10 '10：Shift-jis
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC異常
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(EBCDIC)"
                                            Return ""
                                        Case -1 '処理失敗
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：システムエラー（ログ参照）")
                                            BatchLog.Write("(依頼データ読込)", "失敗", ErrMessage)
                                            mErrMessage = "システムエラー(ログ参照)"
                                            Return ""
                                    End Select
                                End If

                            Case Else

                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))

                                'ＣＭＴファイルコピー先設定
                                strKekka = vbDLL.cmtCPYtoDISK(readfmt.BLOCKSIZE, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)

                                '媒体読込失敗：処理終了
                                If strKekka <> "" Then
                                    BatchLog.Write("(依頼データ読込)", "失敗", "ＣＭＴ読込：" & oRenkei.Message)
                                    BatchLog.UpdateJOBMASTbyErr("ＣＭＴ読込失敗、ファイル名：" & oRenkei.InFileName)
                                    Return ""
                                End If

                                '------------------------------------------------------------
                                '元の文字コードのデータを再変換して処理に乗せる
                                '------------------------------------------------------------
                                If nRetRead = 0 Then
                                    nRetRead = oRenkei.CopyToDisk(readfmt)
                                End If
                        End Select

                        ''****ばいなり未対応 2009.10.23

                        ''2018/02/19 saitou 広島信金(RSV2標準) DEL サーバー処理対応(64ビット対応) -------------------- START
                        ''Dim intblk_size As Short = CShort(readfmt.BLOCKSIZE)
                        ''Dim shtREC_LENGTH As Short = CShort(readfmt.RecordLen)
                        ''Dim shtLABEL_KBN As Short = CShort(.LABEL_KBN)
                        ''2018/02/19 saitou 広島信金(RSV2標準) DEL --------------------------------------------------- END

                        ''***************************
                        'mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                        ''***************************

                        ''ＣＭＴファイルコピー先設定
                        ''2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                        'strKekka = vbDLL.cmtCPYtoDISK(readfmt.BLOCKSIZE, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        ''strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        ' ''strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        ''2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                        ''媒体読込失敗：処理終了
                        'If strKekka <> "" Then
                        '    BatchLog.Write("(依頼データ読込)", "失敗", "ＣＭＴ読込：" & oRenkei.Message)
                        '    BatchLog.UpdateJOBMASTbyErr("ＣＭＴ読込失敗、ファイル名：" & oRenkei.InFileName)
                        '    Return ""
                        'End If

                        ''**************************
                        ''------------------------------------------------------------
                        ''元の文字コードのデータを再変換して処理に乗せる
                        ''------------------------------------------------------------
                        'If nRetRead = 0 Then
                        '    nRetRead = oRenkei.CopyToDisk(readfmt)
                        'End If
                        ''**************************
                        '2018/10/05 saitou 広島信金(RSV2標準) UPD ---------------------------------------- END

                    Case "04", "09" '依頼書/伝票

                        Dim REC_LENGTH As Integer = 120
                        '依頼ファイルコピー先設定
                        ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                        'mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "ETC") & "E" & .TORI_CODE & ".DAT"
                        Select Case INI_RSV2_EDITION
                            Case "2"
                                Dim FileName_Edition2 As String = ""
                                If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                    BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：ファイル名構築失敗")
                                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル名構築失敗:" & Baitai_Code & "-" & FileName_Edition2)
                                    mErrMessage = "ファイル名構築失敗"
                                    Return ""
                                End If
                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "ETC"), FileName_Edition2)
                            Case Else
                                mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "ETC") & "E" & .TORI_CODE & ".DAT"
                        End Select
                        ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END
                        oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
                        nRetRead = oRenkei.CopyToDisk(readfmt)
                        'nRetRead = clsFusion.fn_DEN_CPYTO_DISK(.TORI_CODE, mKoParam.CP.RENKEI_FILENAME, oRenkei.OutFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)

                        '2013/12/24 saitou 標準版 外部媒体対応 ADD -------------------------------------------------->>>>
                        '外部媒体(11～15)追加
                    Case "11", "12", "13", "14", "15"

                        Dim REC_LENGTH As Integer
                        Select Case .CODE_KBN
                            Case "2"
                                '119バイトＪＩＳ改行あり
                                REC_LENGTH = 119
                            Case "3"
                                '118バイトＪＩＳ改行あり
                                REC_LENGTH = 118
                            Case Else
                                'その他
                                REC_LENGTH = readfmt.RecordLen
                        End Select

                        ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                        'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                        'Else
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        'End If

                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                        ''コード区分チェック用のワークファイルパス用
                        'Dim CodeCHKFileName As String = Path.Combine(GetFSKJIni("COMMON", "DATBK"), mArgumentData.INFOToriMast.FILE_NAME_T)

                        'nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, _
                        '                                       .CODE_KBN, FTranName, msgTitle, readfmt, Baitai_Code)

                        ''2009/11/30 コード区分チェック追加===
                        'If nRetRead = 0 Then
                        '    Dim ErrMessage As String = ""
                        '    Select Case oRenkei.CheckCode(CodeCHKFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                        '        Case 0  '異常なし
                        '        Case 10 '10：Shift-jis
                        '            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                        '            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                        '            mErrMessage = "データファイル コード区分異常(JIS)"
                        '            Return ""
                        '        Case 20 ' 20:EBCDIC異常
                        '            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                        '            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                        '            mErrMessage = "データファイル コード区分異常(EBCDIC)"
                        '            Return ""
                        '        Case -1 '処理失敗
                        '            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：システムエラー（ログ参照）")
                        '            BatchLog.Write("(依頼データ読込)", "失敗", ErrMessage)
                        '            mErrMessage = "システムエラー(ログ参照)"
                        '            Return ""
                        '    End Select
                        'End If
                        ''===================================
                        Select Case INI_RSV2_EDITION
                            Case "2"
                                Dim FileName_Edition2 As String = ""
                                If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                    BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：ファイル名構築失敗")
                                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル名構築失敗:" & Baitai_Code & "-" & FileName_Edition2)
                                    mErrMessage = "ファイル名構築失敗"
                                    Return ""
                                End If
                                mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "BAITAIREAD"), FileName_Edition2)

                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                '依頼ファイルをコピーする前に、依頼ファイルの読み取り専用属性を解除する
                                If File.Exists(mKoParam.CP.RENKEI_FILENAME) = True Then
                                    Dim fiCopyFile As New System.IO.FileInfo(mKoParam.CP.RENKEI_FILENAME)
                                    If (fiCopyFile.Attributes And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                                        fiCopyFile.Attributes = FileAttributes.Normal
                                    End If
                                End If

                                nRetRead = oRenkei.CopyToDisk(readfmt)
                                If nRetRead = 0 And Baitai_Code <> "SA" Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '異常なし
                                        Case 10 '10：Shift-jis
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC異常
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(EBCDIC)"
                                            Return ""
                                        Case -1 '処理失敗
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：システムエラー（ログ参照）")
                                            BatchLog.Write("(依頼データ読込)", "失敗", ErrMessage)
                                            mErrMessage = "システムエラー(ログ参照)"
                                            Return ""
                                    End Select
                                End If
                            Case Else
                                If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                                Else
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                                End If

                                oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                                'コード区分チェック用のワークファイルパス用
                                Dim CodeCHKFileName As String = Path.Combine(GetFSKJIni("COMMON", "DATBK"), mArgumentData.INFOToriMast.FILE_NAME_T)

                                nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, _
                                                                       .CODE_KBN, FTranName, msgTitle, readfmt, Baitai_Code)

                                If nRetRead = 0 Then
                                    Dim ErrMessage As String = ""
                                    Select Case oRenkei.CheckCode(CodeCHKFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                        Case 0  '異常なし
                                        Case 10 '10：Shift-jis
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(JIS)"
                                            Return ""
                                        Case 20 ' 20:EBCDIC異常
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                            mErrMessage = "データファイル コード区分異常(EBCDIC)"
                                            Return ""
                                        Case -1 '処理失敗
                                            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：システムエラー（ログ参照）")
                                            BatchLog.Write("(依頼データ読込)", "失敗", ErrMessage)
                                            mErrMessage = "システムエラー(ログ参照)"
                                            Return ""
                                    End Select
                                End If
                        End Select
                        ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

                        '------------------------------------------------------------
                        '元の文字コードのデータを再変換して処理に乗せる
                        '------------------------------------------------------------
                        If nRetRead = 0 Then
                            nRetRead = oRenkei.CopyToDisk(readfmt)
                        End If
                        '2013/12/24 saitou 標準版 外部媒体対応 ADD --------------------------------------------------<<<<

                    Case Else       'その他(伝送 etc)

                        Dim REC_LENGTH As Integer
                        Select Case .CODE_KBN
                            Case "2"
                                '119バイトＪＩＳ改行あり
                                REC_LENGTH = 119
                            Case "3"
                                '118バイトＪＩＳ改行あり
                                REC_LENGTH = 118
                            Case Else
                                'その他
                                REC_LENGTH = readfmt.RecordLen
                        End Select

                        '依頼ファイルコピー先設定
                        If mKoParam.CP.BAITAI_CODE = "SA" Then
                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "ETC") & "S" & .TORI_CODE & ".DAT"
                            '2012/06/30 標準版　WEB伝送対応-------------------------------------------------------------
                        ElseIf mKoParam.CP.BAITAI_CODE = "10" Then
                            ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                            'If mArgumentData.INFOToriMast.FILE_NAME_T.Trim <> "" Then
                            '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), mArgumentData.INFOToriMast.FILE_NAME_T)
                            'Else
                            '    If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & .TORI_CODE & ".DAT"
                            '    Else
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                            '    End If
                            'End If
                            ''--------------------------------------------------------------------------------------------
                            Select Case INI_RSV2_EDITION
                                Case "2"
                                    Dim FileName_Edition2 As String = ""
                                    If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                        BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：ファイル名構築失敗")
                                        BatchLog.Write("(依頼データ読込)", "失敗", "ファイル名構築失敗:" & Baitai_Code & "-" & FileName_Edition2)
                                        mErrMessage = "ファイル名構築失敗"
                                        Return ""
                                    End If
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), FileName_Edition2)
                                Case Else
                                    If mArgumentData.INFOToriMast.FILE_NAME_T.Trim <> "" Then
                                        mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), mArgumentData.INFOToriMast.FILE_NAME_T)
                                    Else
                                        If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & .TORI_CODE & ".DAT"
                                        Else
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                                        End If
                                    End If
                            End Select
                            ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END
                        Else
                            '2010/09/08.Sakon　取引先マスタのファイル名を優先して使用する ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                            'If mArgumentData.INFOToriMast.FILE_NAME_T.Trim <> "" Then
                            '    'TORIMAST.FILE_NAME_Tが空白でなければTORIMAST.FILE_NAME_Tをファイルとして設定する。
                            '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DEN"), mArgumentData.INFOToriMast.FILE_NAME_T)
                            'Else
                            '    'TORIMAST.FILE_NAME_Tが空白であれば従来のファイル名を設定する。
                            '    If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                            '    Else
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                            '    End If
                            'End If
                            Select Case INI_RSV2_EDITION
                                Case "2"
                                    Dim FileName_Edition2 As String = ""
                                    If GetFileName_Edition2(Baitai_Code, FileName_Edition2) = False Then
                                        BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：ファイル名構築失敗")
                                        BatchLog.Write("(依頼データ読込)", "失敗", "ファイル名構築失敗:" & Baitai_Code & "-" & FileName_Edition2)
                                        mErrMessage = "ファイル名構築失敗"
                                        Return ""
                                    End If
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DEN"), FileName_Edition2)
                                Case Else
                                    If mArgumentData.INFOToriMast.FILE_NAME_T.Trim <> "" Then
                                        mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DEN"), mArgumentData.INFOToriMast.FILE_NAME_T)
                                    Else
                                        If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                                        Else
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                                        End If
                                    End If
                            End Select
                            ' 2016/01/12 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END
                            'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                            'Else
                            '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                            'End If
                            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        End If

                        '2010/04/26 パラメータファイル名を考慮する
                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
                        If ParamInFileName = "" Then
                            oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
                        Else
                            oRenkei.InFileName = Path.Combine(Path.GetDirectoryName(mKoParam.CP.RENKEI_FILENAME), ParamInFileName)
                        End If

                        '2012/01/13 saitou 標準修正 ADD ---------------------------------------->>>>
                        '依頼ファイルをコピーする前に、依頼ファイルの読み取り専用属性を解除する
                        If File.Exists(mKoParam.CP.RENKEI_FILENAME) = True Then
                            Dim fiCopyFile As New System.IO.FileInfo(mKoParam.CP.RENKEI_FILENAME)
                            If (fiCopyFile.Attributes And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                                fiCopyFile.Attributes = FileAttributes.Normal
                            End If
                        End If
                        '2012/01/13 saitou 標準修正 ADD ----------------------------------------<<<<

                        nRetRead = oRenkei.CopyToDisk(readfmt)
                        '2009/11/30 コード区分チェック追加===
                        If nRetRead = 0 Then
                            Dim ErrMessage As String = ""
                            Select Case oRenkei.CheckCode(oRenkei.InFileName, mKoParam.CP.CODE_KBN, ErrMessage)
                                Case 0  '異常なし
                                Case 10 '10：Shift-jis
                                    BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                    BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(JIS) ファイル名:" & oRenkei.InFileName)
                                    mErrMessage = "データファイル コード区分異常(JIS)"
                                    Return ""
                                Case 20 ' 20:EBCDIC異常
                                    BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                    BatchLog.Write("(依頼データ読込)", "失敗", "コード区分異常(EBCDIC) ファイル名:" & oRenkei.InFileName)
                                    mErrMessage = "データファイル コード区分異常(EBCDIC)"
                                    Return ""
                                Case -1 '処理失敗
                                    BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：システムエラー（ログ参照）")
                                    BatchLog.Write("(依頼データ読込)", "失敗", ErrMessage)
                                    mErrMessage = "システムエラー(ログ参照)"
                                    Return ""
                            End Select
                        End If
                        '===================================

                End Select

            End With

            '----------------------------------------------------------------------------------------------------
            'Return         :0=成功、50=ファイルなし、100=コード変換失敗、200=コード区分異常（JIS改行あり）、
            '               :300=コード区分異常（JIS改行なし）、400=出力ファイル作成失敗
            '----------------------------------------------------------------------------------------------------
            Select Case nRetRead
                Case 0
                    Return oRenkei.OutFileName
                Case 10
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル取込：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル形式異常、ファイル名：" & oRenkei.InFileName)
                    mErrMessage = "ﾌｧｲﾙ形式異常→" & oRenkei.InFileName
                    Return ""
                Case 50
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル検索：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイルなし、ファイル名：" & oRenkei.InFileName)
                    mErrMessage = "ﾌｧｲﾙなし→" & oRenkei.InFileName
                    Return ""
                Case 100
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル取込(コード変換)：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル取込失敗（コード変換）")
                    mErrMessage = "ﾌｧｲﾙ取込失敗（ｺｰﾄﾞ変換）"
                    Return ""
                Case 200
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル取込(コード区分異常(JIS改行あり))：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル取込失敗（コード区分異常（JIS改行あり））")
                    mErrMessage = "ﾌｧｲﾙ取込失敗(ｺｰﾄﾞ区分異常(JIS改行あり))"
                    Return ""
                Case 300
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル取込(コード区分異常(JIS改行なし))：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル取込失敗(コード区分異常(JIS改行なし))")
                    mErrMessage = "ﾌｧｲﾙ取込失敗(ｺｰﾄﾞ区分異常(JIS改行なし))"
                    Return ""
                Case 400
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル取込(出力ファイル作成)：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル取込失敗（出力ファイル作成）")
                    mErrMessage = "ﾌｧｲﾙ取込失敗(出力ﾌｧｲﾙ作成)"
                    Return ""
                Case 999
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル取込(ユーザキャンセル)：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル取込失敗（ユーザキャンセル）")
                    mErrMessage = "ﾌｧｲﾙ取込失敗(ﾕｰｻﾞｷｬﾝｾﾙ)"
                    Return ""
                Case Else
                    BatchLog.Write("(依頼データ読込)", "失敗", "不明な戻り値：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル取込失敗（ログ参照）")
                    mErrMessage = "ﾌｧｲﾙ取込失敗(ﾛｸﾞ参照)"
                    Return ""
            End Select

        Catch ex As Exception
            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：システムエラー（ログ参照）")
            BatchLog.Write("(依頼データ読込)", "失敗", ex.Message)
            mErrMessage = "システムエラー(ログ参照)"
            Return ""
        Finally
            BatchLog.Write("(依頼データ読込)終了", "成功")
        End Try
    End Function

    '=================================================================
    ' 機能　 ： 明細マスタ登録処理
    ' 引数　 ： readfmt - フォーマットクラス / aReadFile - 読込ファイル名
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    ' 備考　 ： 
    ' 作成日 ： 2009/09/08
    ' 更新日 ：
    '=================================================================
    Private Function MakingMeiMast(ByVal aReadFmt As CAstFormat.CFormat) As Boolean
        Dim nRecordCount As Integer = 0                         'ファイル全体のレコードカウント
        Dim nRecordNumber As Integer = 0                        'ヘッダ単位のレコードカウント
        Dim sCheckRet As String = ""                            'チェック処理結果
        Dim bRet As Boolean                                     '処理結果
        Dim PrnInErrList As ClsPrnInputError = Nothing          'インプットエラーリストクラス
        Dim PrnSyoKekka As ClsPrnSyorikekkaKakuninhyo = Nothing '処理結果確認表
        Dim PrnFlag As Boolean = False
        Dim PrnMeisai As ClsPrnUketukeMeisai = Nothing          '受付明細表
        Dim ArrayPrnMeisai As New ArrayList(128)
        Dim ArrayPrnInErrList As New ArrayList(128)
        Dim PrnSitenYomikae As New ClsPrnSitenYomikae           '支店読替明細表
        Dim bSitenYomikaePrint As Boolean = False               '支店読替出力明細有無
        Dim DaihyouItakuCode As String = ""                     '代表委託者コード
        ' 2016/03/03 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
        Dim IOfileStream As FileStream = Nothing
        ' 2016/03/03 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
        Dim MotikomiOver As Boolean = True  '持込期日フラグ(True:持込期日内)
        Dim DateOver As Boolean = True      '受付時間外フラグ(True:受付時間内)
        '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END

        Try
            BatchLog.Write("(明細マスタ登録)開始", "成功")

            '取引先の情報をフォーマットクラスへ渡す
            aReadFmt.Oracle = MainDB
            aReadFmt.LOG = BatchLog

            '起動パラメータ情報を渡す
            aReadFmt.ToriData = mArgumentData

            Dim ArrayMeiData As New ArrayList           '重複チェックカウンタ
            Dim EndFlag As Boolean = False              'エンドレコード存在フラグ
            Dim ErrFlag As Boolean = False              'インプットエラー異常処理フラグ
            Dim ErrText As String = ""
            Dim SitenYomikae As String                  '支店読替印刷区分(1:印刷対象 / その他:印刷非対象)
            SitenYomikae = CASTCommon.GetFSKJIni("YOMIKAE", "TENPO")

            '--------------------------------------------
            '全データチェック
            '--------------------------------------------
            Try
                If aReadFmt.FirstRead() = 0 Then
                    BatchLog.Write("ファイルオープン", "失敗", aReadFmt.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイルオープン失敗")
                    mErrMessage = "ファイルオープン失敗"
                    Return False
                End If

                'バインド変数対応クエリ生成
                If SetInsertMEIMAST(aReadFmt) = False Then
                    BatchLog.UpdateJOBMASTbyErr("MEIMASTのインサート用クエリ生成失敗")
                    mErrMessage = "明細登録失敗"
                    Return False
                End If

                Do Until aReadFmt.EOF
                    nRecordCount += 1
                    nRecordNumber += 1

                    '*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                    Try
                        '*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

                        ' データを読み込んで，フォーマットチェックを行う
                        sCheckRet = aReadFmt.CheckDataFormat()

                        '*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                    Catch ex As Exception
                        BatchLog.Write_Err("(登録メイン処理)", "失敗", ex)
                        BatchLog.UpdateJOBMASTbyErr("フォーマット変換失敗")
                        mErrMessage = "フォーマット変換失敗"
                        Return False
                    End Try
                    '*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

                    Select Case sCheckRet

                        Case "ERR"
                            'フォーマットエラー
                            Dim nPos As Long
                            If aReadFmt.RecordData.Length > 0 Then nPos = aReadFmt.CheckRegularString

                            If nPos > 0 Then
                                BatchLog.Write("フォーマットエラー", "失敗", nRecordCount.ToString & "行目，" & (nPos + 1).ToString & "バイト目 " & aReadFmt.Message)
                                BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目，" & (nPos + 1).ToString & "バイト目 " & aReadFmt.Message)
                            Else
                                BatchLog.Write("フォーマットエラー", "失敗", nRecordCount.ToString & "行目 " & aReadFmt.Message)
                                BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目 " & aReadFmt.Message)
                            End If

                            mErrMessage = nRecordCount.ToString & "行目 " & aReadFmt.Message

                            bRet = False
                            Exit Do

                        Case "IJO"
                            '処理をとめない 2009.10.17
                            ''金額異常 フォーマットエラー 
                            'If ErrFlag = False AndAlso aReadFmt.InErrorArray.Count > 0 Then
                            '    For i As Integer = 0 To aReadFmt.InErrorArray.Count - 1

                            '        Dim InError As CAstFormat.CFormat.INPUTERROR
                            '        InError = CType(aReadFmt.InErrorArray(i), CAstFormat.CFormat.INPUTERROR)

                            '        '自行店番異常以外／他行店番異常以外／口座番号異常以外のエラー
                            '        If InError.ERRINFO <> CAstFormat.CFormat.Err.Name(CAstFormat.CFormat.Err.InputErrorType.JikouTenban) AndAlso _
                            '           InError.ERRINFO <> CAstFormat.CFormat.Err.Name(CAstFormat.CFormat.Err.InputErrorType.TakouTenban) AndAlso _
                            '           InError.ERRINFO <> CAstFormat.CFormat.Err.Name(CAstFormat.CFormat.Err.InputErrorType.Kouza) Then

                            '            '異常処理を行う
                            '            ErrFlag = True
                            '            ErrText = nRecordCount.ToString & "行目 " & aReadFmt.Message
                            '            Exit For
                            '        End If
                            '    Next i
                            'End If

                            If PrnInErrList Is Nothing Then
                                '最初のファイル作成
                                '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- START
                                PrnInErrList = New ClsPrnInputError(INI_RSV2_EDITION)        ' インプットエラーリストクラス
                                PrnInErrList.CreateMemFile()
                                'PrnInErrList = New ClsPrnInputError        ' インプットエラーリストクラス
                                'PrnInErrList.CreateCsvFile()
                                '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- END
                            End If

                            '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- START
                            Call PrnInErrList.OutputMemData(aReadFmt, aReadFmt.ToriData)
                            'Call PrnInErrList.OutputData(aReadFmt, aReadFmt.ToriData)
                            '2017/02/27 タスク）西野 CHG 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- END

                        Case "H"
                            EndFlag = False

                        Case "D"

                        Case "T"

                        Case "E"
                            EndFlag = True

                        Case "99"
                            'エンドレコードが複数あってもそのまま通す
                            EndFlag = True

                        Case "1A" '2010.01.19 1A 対応
                            EndFlag = True

                        Case ""
                            Exit Do

                    End Select

                    'ヘッダレコード
                    If aReadFmt.IsHeaderRecord() = True Then

                        'マルチチェック
                        If DaihyouItakuCode = "" Then
                            '１　週　目：代表委託者コードを保持
                            DaihyouItakuCode = aReadFmt.ToriData.INFOToriMast.ITAKU_KANRI_CODE_T
                        Else
                            '２週目以降：マルチデータチェック
                            Select Case True    'True = エラー発生

                                Case aReadFmt.ToriData.INFOToriMast.MULTI_KBN_T = "0"
                                    'マルチ区分チェック
                                    BatchLog.Write("明細マスタ登録処理", "失敗", nRecordCount.ToString & "行目 マルチ区分異常 : 0")
                                    BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目 マルチ区分異常 : 0")
                                    mKoParam.CP.TORIS_CODE = aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                    mKoParam.CP.TORIF_CODE = aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                    mErrMessage = "マルチ区分異常"
                                    Return False

                                Case DaihyouItakuCode <> aReadFmt.ToriData.INFOToriMast.ITAKU_KANRI_CODE_T
                                    '代表委託者コードチェック
                                    BatchLog.Write("明細マスタ登録処理", "失敗", nRecordCount.ToString & "行目 代表委託者コード不一致" & _
                                                   " : " & DaihyouItakuCode & " - " & aReadFmt.ToriData.INFOToriMast.ITAKU_KANRI_CODE_T)
                                    BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目 代表委託者コード不一致" & _
                                                   " : " & DaihyouItakuCode & " - " & aReadFmt.ToriData.INFOToriMast.ITAKU_KANRI_CODE_T)
                                    mKoParam.CP.TORIS_CODE = aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                    mKoParam.CP.TORIF_CODE = aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                    mErrMessage = "代表委託者コード不一致"
                                    Return False
                            End Select
                        End If

                        'カウンタ初期化
                        nRecordNumber = 1
                        aReadFmt.InfoMeisaiMast.FILE_SEQ += 1
                        PrnFlag = True

                        '0:非対象,1:店番ソート,2:非ソート,3:エラー分のみ 4以降拡張用
                        Dim ret As Integer = ".123456789".IndexOf(CType(aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T, Char))
                        If ret >= 1 Then
                            If aReadFmt.ToriData.INFOParameter.FMT_KBN <> "03" Then
                                '年金以外の出力対象
                                PrnMeisai = New ClsPrnUketukeMeisai
                                PrnMeisai.OraDB = MainDB
                                PrnMeisai.CreateCsvFile()
                            End If
                        End If

                        ' 二重
                        ArrayMeiData.Clear()
                    End If

                    '明細マスタ項目設定 レコード番号
                    '複数エンドのレコードは明細マスタにINSERTしない
                    If sCheckRet <> "99" Then
                        aReadFmt.InfoMeisaiMast.RECORD_NO = nRecordNumber
                    End If

                    ' データレコード
                    If aReadFmt.IsDataRecord = True Then

                        '支店読替ＣＳＶ出力
                        If SitenYomikae = "1" Then  'INIファイルの出力対象が「1」の場合のみ
                            If aReadFmt.InfoMeisaiMast.KEIYAKU_KIN <> aReadFmt.InfoMeisaiMast.OLD_KIN_NO OrElse _
                               aReadFmt.InfoMeisaiMast.KEIYAKU_SIT <> aReadFmt.InfoMeisaiMast.OLD_SIT_NO OrElse _
                               aReadFmt.InfoMeisaiMast.KEIYAKU_KOUZA <> aReadFmt.InfoMeisaiMast.OLD_KOUZA Then

                                '初回処理(印刷フラグを立てる／ＣＳＶファイルを作成する)
                                If bSitenYomikaePrint = False Then
                                    bSitenYomikaePrint = True
                                    PrnSitenYomikae.CreateCsvFile()
                                End If

                                'データ書き出し
                                PrnSitenYomikae.OutputCsvData(aReadFmt)
                            End If
                        End If

                        '受付明細ＣＳＶ出力
                        If Not PrnMeisai Is Nothing Then
                            ' 0:非対象,1:店番ソート,2:非ソート,3:エラー分のみ
                            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- START
                            ' 修正した意図が不明すぎるため戻しついでに、アップグレード
                            'Select Case aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T
                            'Select Case "1"
                            '    Case "1", "2"
                            '        PrnMeisai.OutputCsvData(aReadFmt)
                            '    Case "3"
                            '        If sCheckRet = "IJO" Then
                            '            PrnMeisai.OutputCsvData(aReadFmt)
                            '        End If
                            'End Select
                            Select Case aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T
                                Case "0"
                                Case Else
                                    Dim UketukeErrOut As String = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFSMAST010_受付明細表出力区分.TXT"), _
                                                                                                    aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T, _
                                                                                                    2)
                                    Select Case UketukeErrOut.Trim
                                        Case "E"
                                            If sCheckRet = "IJO" Then
                                                PrnMeisai.OutputCsvData(aReadFmt)
                                            End If
                                        Case Else
                                            PrnMeisai.OutputCsvData(aReadFmt)
                                    End Select
                            End Select
                            ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- END
                        End If
                    End If

                    ' 二重持込チェック
                    If aReadFmt.ToriData.INFOToriMast.CYCLE_T = "1" Then
                        If nRecordNumber <= 6 Then
                            ' 最初の６件
                            ArrayMeiData.Add(aReadFmt.RecordData)
                        ElseIf sCheckRet = "T" Then
                            ' トレーラレコード
                            ArrayMeiData.Add(aReadFmt.RecordData)
                        End If
                    End If

                    '複数エンドのレコードは明細マスタにINSERTしない
                    '2010.01.19 1A 対応
                    '明細マスタ登録
                    If sCheckRet <> "99" And sCheckRet <> "1A" Then
                        ' 明細マスタ登録
                        bRet = InsertMEIMAST(aReadFmt)
                        If bRet = False Then
                            BatchLog.Write("フォーマットエラー", "失敗", nRecordCount.ToString & "行目 ")
                            BatchLog.UpdateJOBMASTbyErr("MEIMASTのインサート失敗 " & nRecordCount.ToString & "行目 ")
                            mErrMessage = "明細ﾏｽﾀ登録失敗 " & nRecordCount.ToString & "行目"
                            Exit Do
                        End If
                    End If

                    ' トレーラーレコード
                    If aReadFmt.IsTrailerRecord() = True Or aReadFmt.EOF = True Then
                        If aReadFmt.EOF = True AndAlso EndFlag = False Then
                            BatchLog.Write("フォーマットエラー", "失敗", (nRecordCount + 1).ToString & "行目 エンドレコードがありません")
                            BatchLog.UpdateJOBMASTbyErr((nRecordCount + 1).ToString & "行目 エンドレコードがありません")
                            mErrMessage = (nRecordCount + 1).ToString & "行目 ｴﾝﾄﾞﾚｺｰﾄﾞなし"
                            bRet = False
                            Exit Do
                        End If

                        If PrnFlag = True Then

                            ' 明細マスタを検索し，２重持込の検査を行う
                            Dim bDupicate As Boolean = False
                            '2017/12/06 タスク）西野 ADD 標準版修正：広島信金対応（重複レコードチェック）-------------- START
                            Dim blnChofuku As Boolean = False
                            '2017/12/06 タスク）西野 ADD 標準版修正：広島信金対応（重複レコードチェック）-------------- END
                            bDupicate = aReadFmt.CheckDuplicate(ArrayMeiData)
                            If bDupicate = True Then
                                mJobMessage = "二重持ち込み"
                                '2017/12/06 タスク）西野 ADD 標準版修正：広島信金対応（重複レコードチェック）-------------- START
                            Else
                                If GetRSKJIni("RSV2_V1.0.0", "CHOFUKU_CHK") = "1" Then
                                    '同一依頼データが存在する場合、重複レコードチェックエラーとする。
                                    If aReadFmt.fn_Meisai_FukusuuIrai_Check(ArrayMeiData) = True Then
                                        mJobMessage = "重複レコードあり"
                                        blnChofuku = True
                                    End If
                                End If
                                '2017/12/06 タスク）西野 ADD 標準版修正：広島信金対応（重複レコードチェック）-------------- END
                            End If

                            '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
                            If INI_MOTIKOMI_KIJITU_CHK = "1" Then
                                '持込期日チェック
                                MotikomiOver = aReadFmt.CheckMotikomiDate()
                                If MotikomiOver = False Then
                                    mJobMessage = "持込期日超過"
                                    mArgumentData.Syorikekka_Bikou = mJobMessage    '処理結果確認表の備考欄に出力
                                    BatchLog.Write("持込期日チェック処理", "成功", mJobMessage)
                                End If
                            End If

                            If INI_UKETUKE_JIKAN_CHK = "1" Then
                                '持込期日エラーの場合にチェックする
                                If MotikomiOver = False Then
                                    DateOver = aReadFmt.CheckInDatetime(aReadFmt.ToriData.INFOParameter.FURI_DATE)
                                    If DateOver = False Then
                                        mJobMessage = "受付時間超過"
                                        mArgumentData.Syorikekka_Bikou = mJobMessage    '処理結果確認表の備考欄に出力
                                        BatchLog.Write("受付時間外チェック処理", "成功", mJobMessage)
                                    End If
                                End If
                            End If
                            '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- END

                            If bDupicate = False Then
                                ' ２重登録時は，インプットエラーを出力しない
                                ' インプットエラーＣＳＶを出力する
                                If Not PrnInErrList Is Nothing Then
                                    '2017/02/27 タスク）西野 ADD 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- START
                                    ' CSV出力
                                    Call PrnInErrList.OutputCsvData(aReadFmt)

                                    If INI_RSV2_EDITION = "2" Then
                                        PrnInErrList.HostCsvName = "KFSP035_"
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                        PrnInErrList.HostCsvName &= aReadFmt.InfoMeisaiMast.MOTIKOMI_SEQ.ToString("0000") & ".CSV"
                                    End If
                                    '2017/02/27 タスク）西野 ADD 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- END

                                    PrnInErrList.CloseCsv()

                                    ' 正常処理のあとに印刷するため，リストに保存
                                    ArrayPrnInErrList.Add(PrnInErrList)
                                    PrnInErrList = Nothing
                                End If
                            End If

                            ' 処理結果確認表ＣＳＶファイル作成(正常終了時にも出力する)
                            If PrnSyoKekka Is Nothing Then
                                PrnSyoKekka = New ClsPrnSyorikekkaKakuninhyo
                                PrnSyoKekka.CreateCsvFile()
                            End If

                            If Not PrnSyoKekka Is Nothing Then
                                Call PrnSyoKekka.OutputCSVKekka(aReadFmt, mArgumentData)
                            End If

                            If bDupicate = False Then
                                ' ２重登録時は，受付明細表を出力しない
                                ' 受付明細表を印刷する
                                If Not PrnMeisai Is Nothing Then
                                    PrnMeisai.CloseCsv()

                                    ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- START
                                    '' 0:非対象,1:店番ソート,2:非ソート,3:エラー分のみ
                                    'If aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T = "1" Then
                                    '    ' 店番ソート
                                    '    Call PrnMeisai.SortData()
                                    'End If
                                    Dim SortParam As String = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFSMAST010_受付明細表出力区分.TXT"), _
                                                                                            aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T, _
                                                                                            3)
                                    If SortParam.Trim <> "" Then
                                        Call PrnMeisai.SortData(SortParam.Trim)
                                    End If
                                    ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- END

                                    ' 後で印刷
                                    ' 2016/03/03 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                                    'CSVファイルが0バイトの場合、印刷対象に乗せない
                                    IOfileStream = New FileStream(PrnMeisai.FileName, FileMode.Open, FileAccess.Read, FileShare.None)
                                    If IOfileStream.Length <> 0 Then
                                        '2011/06/16 標準版修正 受付明細表印刷部数により印刷 ------------------START
                                        Dim Print_CNT As Integer = 0
                                        Dim OraReader As CASTCommon.MyOracleReader = Nothing
                                        Dim SQL As StringBuilder = New StringBuilder(256)
                                        Try
                                            'デバッグ用
                                            'BatchLog.Write("印刷部数取得開始", "成功", "取引先コード" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "-" & _
                                            'aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T)
                                            OraReader = New CASTCommon.MyOracleReader(MainDB)

                                            SQL.Append("SELECT PRTNUM_T FROM S_TORIMAST ")
                                            SQL.Append(" WHERE TORIS_CODE_T = '" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "'")
                                            SQL.Append(" AND TORIF_CODE_T = '" & aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T & "'")
                                            If OraReader.DataReader(SQL) = True Then
                                                Print_CNT = OraReader.GetInt("PRTNUM_T")
                                            Else
                                                Print_CNT = 1
                                            End If
                                            For intPrintNumber As Integer = 1 To Print_CNT
                                                PrnMeisai.HostCsvName = "KFSP002_"
                                                PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                                PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                                PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                                PrnMeisai.HostCsvName &= aReadFmt.InfoMeisaiMast.MOTIKOMI_SEQ.ToString("0000") & ".CSV"
                                                ArrayPrnMeisai.Add(PrnMeisai)
                                            Next
                                        Catch ex As Exception
                                            BatchLog.Write("印刷部数取得", "失敗", "取引先コード" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "-" & _
                                                           aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T & " " & ex.Message)

                                        Finally
                                            OraReader.Close()
                                        End Try
                                        'ArrayPrnMeisai.Add(PrnMeisai)
                                        '2011/06/16 標準版修正 受付明細表印刷部数により印刷 ------------------END
                                    End If
                                    IOfileStream.Close()
                                    IOfileStream = Nothing
                                    ''2011/06/16 標準版修正 受付明細表印刷部数により印刷 ------------------START
                                    'Dim Print_CNT As Integer = 0
                                    'Dim OraReader As CASTCommon.MyOracleReader = Nothing
                                    'Dim SQL As StringBuilder = New StringBuilder(256)
                                    'Try
                                    '    'デバッグ用
                                    '    'BatchLog.Write("印刷部数取得開始", "成功", "取引先コード" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "-" & _
                                    '    'aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T)
                                    '    OraReader = New CASTCommon.MyOracleReader(MainDB)

                                    '    SQL.Append("SELECT PRTNUM_T FROM S_TORIMAST ")
                                    '    SQL.Append(" WHERE TORIS_CODE_T = '" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "'")
                                    '    SQL.Append(" AND TORIF_CODE_T = '" & aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T & "'")
                                    '    If OraReader.DataReader(SQL) = True Then
                                    '        Print_CNT = OraReader.GetInt("PRTNUM_T")
                                    '    Else
                                    '        Print_CNT = 1
                                    '    End If
                                    '    For intPrintNumber As Integer = 1 To Print_CNT
                                    '        ArrayPrnMeisai.Add(PrnMeisai)
                                    '    Next
                                    'Catch ex As Exception
                                    '    BatchLog.Write("印刷部数取得", "失敗", "取引先コード" & aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "-" & _
                                    '                   aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T & " " & ex.Message)

                                    'Finally
                                    '    OraReader.Close()
                                    'End Try
                                    ''ArrayPrnMeisai.Add(PrnMeisai)
                                    ''2011/06/16 標準版修正 受付明細表印刷部数により印刷 ------------------END
                                    ' 2016/03/03 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END

                                    PrnMeisai = Nothing



                                End If
                            End If

                            '店別集計表印刷用リストに格納(印刷区分,取引先主コード,取引先副コード)
                            ArrayTenbetu.Add(mArgumentData.INFOToriMast.FUNOU_MEISAI_KBN_T & "," & _
                                             aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "," & _
                                             aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T & "," & _
                                             aReadFmt.InfoMeisaiMast.MOTIKOMI_SEQ)

                            ' スケジュールマスタを更新する
                            bRet = aReadFmt.UpdateSCHMAST
                            If bRet = False Then
                                BatchLog.Write("スケジュール更新", "失敗")
                                BatchLog.UpdateJOBMASTbyErr("スケジュール更新失敗")
                                mErrMessage = "スケジュール更新失敗"
                                Exit Do
                            End If
                        End If

                        PrnFlag = False
                    End If
                Loop

                ' 2016/02/08 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
                If dblock Is Nothing Then
                    dblock = New CASTCommon.CDBLock

                    Dim sWorkTime As String = CASTCommon.GetFSKJIni("COMMON", "LOCKTIME3")
                    If IsNumeric(sWorkTime) Then
                        LockWaitTime = CInt(sWorkTime)
                        If LockWaitTime <= 0 Then
                            LockWaitTime = 600
                        End If
                    End If

                    If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                        BatchLog.Write_Err("主処理", "失敗", "振込依頼データ落込処理で実行待ちタイムアウト")
                        BatchLog.UpdateJOBMASTbyErr("振込依頼データ落込処理で実行待ちタイムアウト")
                        Return False
                    End If
                End If
                ' 2016/02/08 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END

                If bRet = False Then

                    If aReadFmt.IsTrailerRecord() = True Then
                        ' インプットエラーリストを印刷する
                        If Not PrnInErrList Is Nothing Then
                            '2017/02/27 タスク）西野 ADD 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- START
                            ' CSV出力
                            Call PrnInErrList.OutputCsvData(aReadFmt)

                            If INI_RSV2_EDITION = "2" Then
                                PrnInErrList.HostCsvName = "KFSP035_"
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                PrnInErrList.HostCsvName &= aReadFmt.InfoMeisaiMast.MOTIKOMI_SEQ.ToString("0000") & ".CSV"
                            End If
                            '2017/02/27 タスク）西野 ADD 飯田信金 カスタマイズ対応(インプットエラーリスト修正) -------------------- END

                            PrnInErrList.CloseCsv()

                            ' 正常処理のあとに印刷するため，リストに保存
                            ArrayPrnInErrList.Add(PrnInErrList)
                            PrnInErrList = Nothing
                        End If

                        For i As Integer = 0 To ArrayPrnInErrList.Count - 1

                            PrnInErrList = CType(ArrayPrnInErrList(i), ClsPrnInputError)
                            If PrnInErrList.ReportExecute() = False Then
                                BatchLog.Write("インプットエラーリスト出力", "失敗", "ファイル名:" & PrnInErrList.FileName & " メッセージ:" & PrnInErrList.ReportMessage)
                            End If


                            '2017/04/12 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ START
                            '大規模版のみコピーする
                            If INI_RSV2_EDITION = "2" Then
                                Try
                                    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                                    DestName &= PrnInErrList.HostCsvName
                                    File.Copy(PrnInErrList.FileName, DestName, True)
                                    BatchLog.Write("インプットエラーＣＳＶ出力処理", "成功", DestName)
                                    PrnInErrList = Nothing
                                Catch ex As Exception
                                    BatchLog.Write("インプットエラーＣＳＶ出力処理", "失敗", ex.Message)
                                End Try
                            End If
                            'Try
                            '    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                            '    DestName &= PrnInErrList.HostCsvName
                            '    File.Copy(PrnInErrList.FileName, DestName, True)
                            '    BatchLog.Write("インプットエラーＣＳＶ出力処理", "成功", DestName)
                            '    PrnInErrList = Nothing
                            'Catch ex As Exception
                            '    BatchLog.Write("インプットエラーＣＳＶ出力処理", "失敗", ex.Message)
                            'End Try
                            '2017/04/12 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ END

                            PrnInErrList = Nothing
                        Next i

                        'インプットエラー出力時はパトライト点灯
                        If ArrayPrnInErrList.Count > 0 Then
                            Try
                                Dim ELog As New CASTCommon.ClsEventLOG
                                ELog.Write("インプットエラーリスト出力あり", EventLogEntryType.Error)
                            Catch ex As Exception
                            End Try
                        End If
                    End If

                    'インプットエラー出力時に異常終了とする場合はコメント化しない +++
                    'mErrMessage = "インプットエラーリスト出力あり"
                    Return False
                    '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                End If

                '2010.01.19 1A 対応
                If sCheckRet <> "E" AndAlso sCheckRet <> "T" AndAlso sCheckRet <> "" AndAlso sCheckRet <> "99" AndAlso sCheckRet <> "1A" Then
                    BatchLog.Write("フォーマットエラー", "失敗", nRecordCount.ToString & "行目 " & aReadFmt.Message)
                    BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目 ファイルレコード（エンド区分）異常")
                    mErrMessage = "エンド区分異常"
                    Return False
                End If

                '支店読替明細表を印刷する
                If bSitenYomikaePrint = True Then
                    If PrnSitenYomikae.ReportExecute() = False Then
                        BatchLog.Write("支店読替明細表", "失敗", "ファイル名:" & PrnSitenYomikae.FileName & " メッセージ:" & PrnSitenYomikae.ReportMessage)
                    End If
                End If

                ' 受付明細表を印刷する
                For i As Integer = 0 To ArrayPrnMeisai.Count - 1
                    PrnMeisai = CType(ArrayPrnMeisai(i), ClsPrnUketukeMeisai)

                    If PrnMeisai.ReportExecute() = False Then
                        BatchLog.Write("受付明細表出力", "失敗", "ファイル名:" & PrnMeisai.FileName & " メッセージ:" & PrnMeisai.ReportMessage)
                    End If

                    '2017/04/12 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ START
                    '大規模版のみコピーする
                    If INI_RSV2_EDITION = "2" Then
                        Try
                            Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                            DestName &= PrnMeisai.HostCsvName
                            File.Copy(PrnMeisai.FileName, DestName, True)
                            BatchLog.Write("受付明細表ＣＳＶ出力処理", "成功", DestName)
                            PrnMeisai = Nothing
                        Catch ex As Exception
                            BatchLog.Write("受付明細表ＣＳＶ出力処理", "失敗", ex.Message)
                        End Try
                    End If
                    'Try
                    '    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    '    DestName &= PrnMeisai.HostCsvName
                    '    File.Copy(PrnMeisai.FileName, DestName, True)
                    '    BatchLog.Write("受付明細表ＣＳＶ出力処理", "成功", DestName)
                    '    PrnMeisai = Nothing
                    'Catch ex As Exception
                    '    BatchLog.Write("受付明細表ＣＳＶ出力処理", "失敗", ex.Message)
                    'End Try
                    '2017/04/12 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ END

                Next i

                ' インプットエラーリストを印刷する
                For i As Integer = 0 To ArrayPrnInErrList.Count - 1

                    PrnInErrList = CType(ArrayPrnInErrList(i), ClsPrnInputError)

                    If PrnInErrList.ReportExecute() = False Then
                        BatchLog.Write("インプットエラーリスト出力", "失敗", "ファイル名:" & PrnInErrList.FileName & " メッセージ:" & PrnInErrList.ReportMessage)
                    End If

                    '2017/04/12 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ START
                    '大規模版のみコピーする
                    If INI_RSV2_EDITION = "2" Then
                        Try
                            Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                            DestName &= PrnInErrList.HostCsvName
                            File.Copy(PrnInErrList.FileName, DestName, True)
                            BatchLog.Write("インプットエラーＣＳＶ出力処理", "成功", DestName)
                            PrnInErrList = Nothing
                        Catch ex As Exception
                            BatchLog.Write("インプットエラーＣＳＶ出力処理", "失敗", ex.Message)
                        End Try
                    End If
                    'Try
                    '    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    '    DestName &= PrnInErrList.HostCsvName
                    '    File.Copy(PrnInErrList.FileName, DestName, True)
                    '    BatchLog.Write("インプットエラーＣＳＶ出力処理", "成功", DestName)
                    '    PrnInErrList = Nothing
                    'Catch ex As Exception
                    '    BatchLog.Write("インプットエラーＣＳＶ出力処理", "失敗", ex.Message)
                    'End Try
                    '2017/04/12 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ END

                Next i

                'インプットエラー出力時はパトライト点灯
                If ArrayPrnInErrList.Count > 0 Then
                    Try
                        Dim ELog As New CASTCommon.ClsEventLOG
                        ELog.Write("インプットエラーリスト出力あり", EventLogEntryType.Error)
                    Catch ex As Exception

                    End Try
                End If

                '落し込みチェック
                If ErrFlag = True Then
                    BatchLog.Write("インプットチェック", "失敗", ErrText)
                    BatchLog.UpdateJOBMASTbyErr("インプットチェックエラー " & ErrText)
                    mErrMessage = ErrText
                    Return False
                End If

                '処理結果確認表を印刷する
                ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- START
                'If Not PrnSyoKekka Is Nothing Then
                '    If PrnSyoKekka.ReportExecute() = False Then
                '        BatchLog.Write("処理結果確認表出力", "失敗", "ファイル名:" & PrnSyoKekka.FileName & " メッセージ:" & PrnSyoKekka.ReportMessage)
                '    End If
                'End If
                '2017/12/07 タスク）西野 CHG 標準版修正：広島信金対応（持込期日対応）---------------------- START
                '持込期日/受付時間外チェックエラー時は出力する（各チェック機能が有効の場合）
                If INI_RSV2_S_SYORIKEKKA_TOUROKU = "1" OrElse MotikomiOver = False OrElse DateOver = False Then
                    'If INI_RSV2_S_SYORIKEKKA_TOUROKU = "1" Then
                    '2017/12/07 タスク）西野 CHG 標準版修正：広島信金対応（持込期日対応）---------------------- END
                    If Not PrnSyoKekka Is Nothing Then
                        If PrnSyoKekka.ReportExecute() = False Then
                            BatchLog.Write("処理結果確認表出力", "失敗", "ファイル名:" & PrnSyoKekka.FileName & " メッセージ:" & PrnSyoKekka.ReportMessage)
                        End If
                    End If
                End If
                ' 2016/03/15 タスク）綾部 ADD 【ST】UI_B-14-99(RSV2対応) -------------------- END

            Catch ex As Exception
                BatchLog.Write("明細マスタ登録処理", "失敗", ex.Message & ":" & ex.StackTrace)
                BatchLog.UpdateJOBMASTbyErr("システムエラー（ログ参照）")
                mErrMessage = "システムエラー（ログ参照）"
                Return False
            End Try

            Return True

        Catch ex As Exception
            BatchLog.Write("明細マスタ登録処理", "失敗", ex.Message & ":" & ex.StackTrace)
            BatchLog.UpdateJOBMASTbyErr("システムエラー（ログ参照）")
            mErrMessage = "システムエラー（ログ参照）"
            Return False
        Finally
            BatchLog.Write("(明細マスタ登録)終了", "成功")
        End Try
    End Function

    ' 機能　 ： 明細マスタ INSERTクエリセット
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function SetInsertMEIMAST(ByRef aReadFmt As CAstFormat.CFormat) As Boolean
        Dim SQL As StringBuilder        ' ＳＱＬ

        Dim stTori As CAstBatch.CommData.stTORIMAST ' 取引先情報
        stTori = aReadFmt.ToriData.INFOToriMast

        Try
            SQL = New System.Text.StringBuilder("INSERT INTO ", 2048)
            SQL.AppendLine(" S_MEIMAST(")
            SQL.AppendLine(" FSYORI_KBN_K")
            SQL.AppendLine(",TORIS_CODE_K")
            SQL.AppendLine(",TORIF_CODE_K")
            SQL.AppendLine(",FURI_DATE_K")
            SQL.AppendLine(",MOTIKOMI_SEQ_K")
            SQL.AppendLine(",SYUBETU_K")
            SQL.AppendLine(",FURI_CODE_K")
            SQL.AppendLine(",KIGYO_CODE_K")
            SQL.AppendLine(",ITAKU_KIN_K")
            SQL.AppendLine(",ITAKU_SIT_K")
            SQL.AppendLine(",ITAKU_KAMOKU_K")
            SQL.AppendLine(",ITAKU_KOUZA_K")
            SQL.AppendLine(",KEIYAKU_KIN_K")
            SQL.AppendLine(",KEIYAKU_SIT_K")
            SQL.AppendLine(",KEIYAKU_NO_K")
            SQL.AppendLine(",KEIYAKU_KNAME_K")
            SQL.AppendLine(",KEIYAKU_KAMOKU_K")
            SQL.AppendLine(",KEIYAKU_KOUZA_K")
            SQL.AppendLine(",FURIKIN_K")
            SQL.AppendLine(",TESUU_KIN_K")
            SQL.AppendLine(",FURIKETU_CODE_K")
            SQL.AppendLine(",FURIKETU_CENTERCODE_K")
            SQL.AppendLine(",SINKI_CODE_K")
            SQL.AppendLine(",NS_KBN_K")
            SQL.AppendLine(",TEKIYO_KBN_K")
            SQL.AppendLine(",KTEKIYO_K")
            SQL.AppendLine(",NTEKIYO_K")
            SQL.AppendLine(",JYUYOUKA_NO_K")
            SQL.AppendLine(",TEISEI_SIT_K")
            SQL.AppendLine(",TEISEI_KAMOKU_K")
            SQL.AppendLine(",TEISEI_KOUZA_K")
            SQL.AppendLine(",TEISEI_AKAMOKU_K")
            SQL.AppendLine(",TEISEI_AKOUZA_K")
            SQL.AppendLine(",DATA_KBN_K")
            SQL.AppendLine(",FURI_DATA_K")
            SQL.AppendLine(",RECORD_NO_K")
            SQL.AppendLine(",SORT_KEY_K")
            SQL.AppendLine(",SAKUSEI_DATE_K")
            SQL.AppendLine(",KOUSIN_DATE_K")
            SQL.AppendLine(",YOBI1_K")
            SQL.AppendLine(",YOBI2_K")
            SQL.AppendLine(",YOBI3_K")
            SQL.AppendLine(",YOBI4_K")
            SQL.AppendLine(",YOBI5_K")
            SQL.AppendLine(",YOBI6_K")
            SQL.AppendLine(",YOBI7_K")
            SQL.AppendLine(",YOBI8_K")
            SQL.AppendLine(",YOBI9_K")
            SQL.AppendLine(",YOBI10_K")

            If aReadFmt.BinMode Then
                SQL.AppendLine(",BIN_DATA_K")
            End If

            SQL.AppendLine(")")
            SQL.AppendLine(" VALUES(")
            SQL.AppendLine(" :FSYORI_KBN")
            SQL.AppendLine(",:TORIS_CODE")                              'TORIS_CODE_K             
            SQL.AppendLine(",:TORIF_CODE")                              'TORIF_CODE_K
            SQL.AppendLine(",:FURI_DATE")                               'FURI_DATE_K
            SQL.AppendLine(",:MOTIKOMI_SEQ")                            'MOTIKOMI_SEQ_K
            SQL.AppendLine(",:SYUBETU")                                 'SYUBETU_K
            SQL.AppendLine(",:FURI_CODE")                               'FURI_CODE_K
            SQL.AppendLine(",:KIGYO_CODE")                              'KIGYO_CODE_K
            SQL.AppendLine(",:ITAKU_KIN")                               'ITAKU_KIN_K
            SQL.AppendLine(",:ITAKU_SIT")                               'ITAKU_SIT_K
            SQL.AppendLine(",:ITAKU_KAMOKU")                            'ITAKU_KAMOKU_K
            SQL.AppendLine(",:ITAKU_KOUZA")                             'ITAKU_KOUZA_K
            SQL.AppendLine(",:KEIYAKU_KIN")                             'KEIYAKU_KIN_K
            SQL.AppendLine(",:KEIYAKU_SIT")                             'KEIYAKU_SIT_K
            SQL.AppendLine(",:KEIYAKU_NO")                              'KEIYAKU_NO_K
            SQL.AppendLine(",:KEIYAKU_KNAME")                           'KEIYAKU_KNAME_K
            SQL.AppendLine(",:KEIYAKU_KAMOKU")                          'KEIYAKU_KAMOKU_K
            SQL.AppendLine(",:KEIYAKU_KOUZA")                           'KEIYAKU_KOUZA_K
            SQL.AppendLine(",:FURIKIN")                                 'FURIKIN_K
            SQL.AppendLine(",:TESUU_KIN")                               'TESUU_KIN_K
            SQL.AppendLine(",:FURIKETU_CODE")                           'FURIKETU_CODE_K 
            SQL.AppendLine(",:FURIKETU_CENTERCODE")                     'FURIKETU_SENTERCODE_K
            SQL.AppendLine(",:SINKI_CODE")                              'SINKI_CODE_K
            SQL.AppendLine(",:NS_KBN")                                  'NS_KBN_K
            SQL.AppendLine(",:TEKIYOU_KBN")                             'TEKIYO_KBN_K
            SQL.AppendLine(",:KTEKIYO")                                 'KTEKIYO_K
            SQL.AppendLine(",:NTEKIYOU")                                'NTEKIYO_K
            SQL.AppendLine(",:JYUYOKA_NO")                              'JYUYOUKA_NO_K
            SQL.AppendLine(",:TEISEI_SIT")                              'TEISEI_SIT_K
            SQL.AppendLine(",:TEISEI_KAMOKU")                           'TEISEI_KAMOKU_K
            SQL.AppendLine(",:TEISEI_KOUZA")                            'TEISEI_KOUZA_K
            SQL.AppendLine(",'00'")                                     'TEISEI_AKAMOKU_K
            SQL.AppendLine(",'0000000'")                                'TEISEI_AKOUZA_K
            SQL.AppendLine(",:DATA_KBN")                                'DATA_KBN_K
            SQL.AppendLine(",:FURI_DATA")                               'FURI_DATA_K
            SQL.AppendLine(",:RECORD_NO")                               'RECORD_NO_K
            SQL.AppendLine(",' '")                                      'SORT_KEY_K
            SQL.AppendLine(",:SAKUSEI_DATE")                            'SAKUSEI_DATE_K
            SQL.AppendLine(",'00000000'")                               'KOUSIN_DATE_K
            SQL.AppendLine(",:YOBI1")                                   'YOBI1_K
            SQL.AppendLine(",:YOBI2")                                   'YOBI2_K
            SQL.AppendLine(",:YOBI3")                                   'YOBI3_K
            '2010/01/19 需要家番号を変換したものを格納
            SQL.AppendLine(",:YOBI4")                                   'YOBI4_K
            '=========================================
            '*** Str Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
            'SQL.AppendLine(",' '")                                      'YOBI5_K
            'SQL.AppendLine(",' '")                                      'YOBI6_K
            'SQL.AppendLine(",' '")                                      'YOBI7_K
            'SQL.AppendLine(",' '")                                      'YOBI8_K
            'SQL.AppendLine(",' '")                                      'YOBI9_K
            'SQL.AppendLine(",' '")                                      'YOBI10_K
            SQL.AppendLine(",:YOBI5")                                   'YOBI5_K
            SQL.AppendLine(",:YOBI6")                                   'YOBI6_K
            SQL.AppendLine(",:YOBI7")                                   'YOBI7_K
            SQL.AppendLine(",:YOBI8")                                   'YOBI8_K
            SQL.AppendLine(",:YOBI9")                                   'YOBI9_K
            SQL.AppendLine(",:YOBI10")                                  'YOBI10_K
            '*** End Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

            If aReadFmt.BinMode Then
                'EBCDICの場合
                SQL.AppendLine(",:BIN")                                 'BIN_DATA_K
                SQL.AppendLine(")")
            Else
                SQL.AppendLine(")")
            End If

            MainDB.CommandText = SQL.ToString

        Catch ex As Exception
            BatchLog.Write("明細マスタ登録クエリ生成", "失敗", ex.Message)
            Return False
        End Try

        Return True
    End Function

    ' 機能　 ： 明細マスタ INSERT
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Function InsertMEIMAST(ByRef aReadFmt As CAstFormat.CFormat) As Boolean
        Dim stTori As CAstBatch.CommData.stTORIMAST = aReadFmt.ToriData.INFOToriMast ' 取引先情報
        Dim stMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast     ' 明細マスタ情報

        Try
            With MainDB
                .Item("FSYORI_KBN") = stTori.FSYORI_KBN_T
                .Item("TORIS_CODE") = stTori.TORIS_CODE_T
                .Item("TORIF_CODE") = stTori.TORIF_CODE_T
                .Item("FURI_DATE") = stMei.FURIKAE_DATE

                If aReadFmt.IsEndRecord = True AndAlso stMei.SCH_UPDATE_FLG = True Then
                    ' エンドレコードの場合は，１つ前の持込SEQを使用する
                    .Item("MOTIKOMI_SEQ") = stMei.MOTIKOMI_SEQ - 1
                Else
                    .Item("MOTIKOMI_SEQ") = stMei.MOTIKOMI_SEQ
                End If

                .Item("SYUBETU") = stTori.SYUBETU_T
                .Item("FURI_CODE") = stTori.FURI_CODE_T
                .Item("KIGYO_CODE") = stTori.KIGYO_CODE_T
                .Item("ITAKU_KIN") = stMei.ITAKU_KIN
                .Item("ITAKU_SIT") = stMei.ITAKU_SIT
                .Item("ITAKU_KAMOKU") = stMei.ITAKU_KAMOKU
                .Item("ITAKU_KOUZA") = stMei.ITAKU_KOUZA
                .Item("KEIYAKU_KIN") = stMei.KEIYAKU_KIN
                .Item("KEIYAKU_SIT") = stMei.KEIYAKU_SIT
                .Item("KEIYAKU_NO") = stMei.KEIYAKU_NO
                .Item("KEIYAKU_KNAME") = stMei.KEIYAKU_KNAME
                '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------START
                .Item("KEIYAKU_KAMOKU") = CASTCommon.ConvertKamoku1TO2_K(stMei.KEIYAKU_KAMOKU)
                '.Item("KEIYAKU_KAMOKU") = CASTCommon.onvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)
                '2011/06/28 標準版修正 総振の場合、科目その他は、09に変更 ------------------END
                .Item("KEIYAKU_KOUZA") = stMei.KEIYAKU_KOUZA
                .Item("FURIKIN") = stMei.FURIKIN
                .Item("TESUU_KIN") = stMei.TESUU_KIN
                .Item("FURIKETU_CODE") = stMei.FURIKETU_CODE
                .Item("FURIKETU_CENTERCODE") = stMei.FURIKETU_CENTERCODE
                .Item("SINKI_CODE") = stMei.SINKI_CODE
                .Item("NS_KBN") = stTori.NS_KBN_T
                .Item("TEKIYOU_KBN") = stTori.TEKIYOU_KBN_T
                .Item("KTEKIYO") = stMei.KTEKIYO
                .Item("NTEKIYOU") = stTori.NTEKIYOU_T
                .Item("JYUYOKA_NO") = stMei.JYUYOKA_NO
                .Item("TEISEI_SIT") = stMei.TEISEI_SIT
                .Item("TEISEI_KAMOKU") = stMei.TEISEI_KAMOKU
                .Item("TEISEI_KOUZA") = stMei.TEISEI_KOUZA
                .Item("DATA_KBN") = stMei.DATA_KBN
                .Item("FURI_DATA") = aReadFmt.RecordDataNoChange
                .Item("RECORD_NO") = stMei.RECORD_NO.ToString
                .Item("SAKUSEI_DATE") = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
                .Item("YOBI1") = stMei.YOBI1
                .Item("YOBI2") = stMei.YOBI2
                .Item("YOBI3") = stMei.YOBI3
                '2010/01/19 需要家番号を変換して入力
                Dim wJYUYOUKA As New StringBuilder(24)
                For Each c As Char In stMei.JYUYOKA_NO.PadRight(24)
                    Select Case c
                        '0-9、A-Z、(、)、SPACE
                        Case "0"c To "9"c, "A"c To "Z"c, "("c To ")"c, " "c
                            wJYUYOUKA.Append(c)
                        Case Else
                            wJYUYOUKA.Append("0"c)
                    End Select
                Next
                .Item("YOBI4") = wJYUYOUKA.ToString
                '*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                .Item("YOBI5") = stMei.YOBI5
                .Item("YOBI6") = stMei.YOBI6
                .Item("YOBI7") = stMei.YOBI7
                .Item("YOBI8") = stMei.YOBI8
                .Item("YOBI9") = stMei.YOBI9
                .Item("YOBI10") = stMei.YOBI10
                '*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                '===================================

                If aReadFmt.BinMode Then
                    .Item("BIN") = aReadFmt.RecordDataBin
                End If

                If MainDB.ExecuteNonQuery() <= 0 Then
                    BatchLog.Write("明細マスタ登録", "失敗", MainDB.Message)
                    Return False
                End If
            End With

        Catch ex As Exception
            BatchLog.Write("明細マスタ登録", "失敗", ex.Message)
            mErrMessage = "明細マスタ登録失敗"
            Return False
        End Try

        Return True
    End Function

    '2010/02/18 対応する依頼書マスタの新規コードを更新する
    ' 機能　 ： 新規コード UPDATE
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 依頼書マスタ新規コード更新用
    '
    Private Function UpdateENTMAST() As Boolean

        Try
            For I As Integer = 1 To 2
                Dim SQL As StringBuilder = New System.Text.StringBuilder(512)
                If I = 1 Then
                    SQL.Append("UPDATE S_ENTMAST SET")
                Else
                    SQL.Append("UPDATE S_FUKU_ENTMAST SET")
                End If

                SQL.Append(" SINKI_CODE_E = '0'")
                SQL.Append(" WHERE TORIS_CODE_E = " & SQ(mKoParam.CP.TORIS_CODE))
                SQL.Append(" AND TORIF_CODE_E = " & SQ(mKoParam.CP.TORIF_CODE))
                SQL.Append(" AND FURI_DATE_E = " & SQ(mKoParam.CP.FURI_DATE))
                SQL.Append(" AND SINKI_CODE_E = '1'")
                SQL.Append(" AND KAISI_DATE_E <= " & SQ(mKoParam.CP.FURI_DATE)) '開始年月日を比較

                If MainDB.ExecuteNonQuery(SQL) < 0 Then
                    BatchLog.Write("依頼書マスタ（新規コード）更新", "失敗", MainDB.Message)
                    Return False
                End If
            Next

        Catch ex As Exception
            BatchLog.Write("依頼書マスタ（新規コード）更新", "失敗", ex.Message)
            Return False
        End Try
        Return True
    End Function

    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    '================================
    ' ファイル名構築
    '================================
    Private Function GetFileName_Edition2(ByVal BaitaiCode As String, ByRef FileName As String) As Boolean

        Dim CheckFileName As String = ""

        Try
            BatchLog.Write("", "0000000000-00", "00000000", "ファイル名構築", "開始")

            FileName = ""

            '--------------------------------
            ' 業務
            '--------------------------------
            If mKoParam.CP.FSYORI_KBN = "1" Then
                CheckFileName = "J_"
            Else
                CheckFileName = "S_"
            End If

            '--------------------------------
            ' 媒体
            '--------------------------------
            CheckFileName &= GetTextFileInfo(CASTCommon.GetFSKJIni("COMMON", "TXT") & "媒体命名規約.txt", BaitaiCode) & "_"

            '--------------------------------
            ' マルチ区分
            '--------------------------------
            If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                CheckFileName &= "S_" & mKoParam.CP.TORI_CODE & "_"
            Else
                '2016/02/05 タスク）斎藤 RSV2対応 UPD ---------------------------------------- START
                'マルチでも媒体が依頼書、伝票の場合はシングル扱いとする
                Select Case BaitaiCode
                    Case "04", "09"
                        CheckFileName &= "S_" & mKoParam.CP.TORI_CODE & "_"
                    Case Else
                        CheckFileName &= "M_" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & "00" & "_"
                End Select
                'CheckFileName &= "M_" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & "00" & "_"
                '2016/02/05 タスク）斎藤 RSV2対応 UPD ---------------------------------------- END
            End If

            '--------------------------------
            ' フォーマット区分
            '--------------------------------
            CheckFileName &= mArgumentData.INFOToriMast.FMT_KBN_T & "_"

            '--------------------------------
            ' 指定日(振替日または振込日)
            '--------------------------------
            CheckFileName &= mKoParam.CP.FURI_DATE

            Dim FileList() As String = Nothing
            Select Case BaitaiCode
                '2017/04/10 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ START
                ' 伝送系媒体追加(30～39)
                Case "00", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                'Case "00"
                '2017/04/10 タスク）西野 CHG 標準版修正（飯田信金分反映）------------------------------------ END
                    FileList = Directory.GetFiles(GetFSKJIni("COMMON", "DEN"))
                Case "01", "05", "06", "11", "12", "13", "14", "15"
                    FileList = Directory.GetFiles(GetFSKJIni("COMMON", "BAITAIREAD"))
                Case "04", "09"
                    FileList = Directory.GetFiles(GetFSKJIni("COMMON", "ETC"))
                Case "10"
                    FileList = Directory.GetFiles(GetFSKJIni("WEB_DEN", "WEB_REV"))
            End Select

            BatchLog.Write("", "0000000000-00", "00000000", "ファイル名構築", "成功", "チェックファイル名:" & CheckFileName)
            For i As Integer = 0 To FileList.Length - 1 Step 1
                If Path.GetFileName(FileList(i)).IndexOf(CheckFileName) = 0 Then
                    FileName = Path.GetFileName(FileList(i))
                    BatchLog.Write("", "0000000000-00", "00000000", "ファイル名構築", "成功", "取得ファイル名(" & i & "):" & FileName)
                    Exit For
                End If
            Next

            If FileName = "" Then
                BatchLog.Write("", "0000000000-00", "00000000", "ファイル名構築", "失敗", "取得ファイル名なし")
                Return False
            Else
                Return True
            End If

        Catch ex As Exception
            BatchLog.Write("", "0000000000-00", "00000000", "ファイル名構築", "", ex.Message)
            Return False
        Finally
            BatchLog.Write("", "0000000000-00", "00000000", "ファイル名構築", "終了")
        End Try

    End Function

    '================================
    ' テキストファイル情報取得
    '================================
    Private Function GetTextFileInfo(ByVal TextFileName As String, ByVal KeyInfo As String) As String

        Dim sr As StreamReader = Nothing

        Try
            BatchLog.Write("", "0000000000-00", "00000000", "テキストファイル読込", "開始", TextFileName)

            sr = New StreamReader(TextFileName, Encoding.GetEncoding("SHIFT-JIS"))
            While sr.Peek > -1
                Dim strLineData() As String = sr.ReadLine().Split(","c)
                If strLineData(0) = KeyInfo Then
                    Return strLineData(1).Trim
                End If
            End While

            BatchLog.Write("", "0000000000-00", "00000000", "テキストファイル読込", "", "該当なし")
            Return "NON"

        Catch ex As Exception
            BatchLog.Write("", "0000000000-00", "00000000", "テキストファイル読込", "", ex.Message)
            Return "ERR"
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
            BatchLog.Write("", "0000000000-00", "00000000", "テキストファイル読込", "終了", "")
        End Try

    End Function
    ' 2016/01/12 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

End Class
