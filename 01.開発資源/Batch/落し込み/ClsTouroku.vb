Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CAstSystem
Imports CASTCommon
Imports CASTCommon.ModPublic
Imports Microsoft.VisualBasic

' 個別落し込み処理
Public Class ClsTouroku

    '2013/12/24 saitou 標準版 未使用 DEL -------------------------------------------------->>>>
    'Private clsFusion As New clsFUSION.clsMain
    '2013/12/24 saitou 標準版 未使用 DEL --------------------------------------------------<<<<
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
                CP.FURI_DATE = para(1)                      '振替日
                CP.CODE_KBN = para(2)                       'コード区分
                CP.FMT_KBN = para(3).PadLeft(2, "0"c)       'フォーマット区分
                CP.BAITAI_CODE = para(4).PadLeft(2, "0"c)   '媒体コード
                CP.LABEL_KBN = para(5)                      'ラベル区分

                ' 2017/04/12 タスク）綾部 CHG 【ME】(RSV2標準機能対応) -------------------- START
                'CP.RENKEI_FILENAME = ""                     '連携ファイル名
                'CP.ENC_KBN = ""                             '暗号化処理区分
                'CP.ENC_KEY1 = ""                            '暗号化キー１
                'CP.ENC_KEY2 = ""                            '暗号化キー２
                'CP.ENC_OPT1 = ""                            'ＡＥＳオプション
                'CP.CYCLENO = ""                             'サイクル№
                'CP.JOBTUUBAN = Integer.Parse(para(6))       'ジョブ通番
                Select Case para.Length
                    Case 7
                        CP.RENKEI_FILENAME = ""                     '連携ファイル名
                        CP.ENC_KBN = ""                             '暗号化処理区分
                        CP.ENC_KEY1 = ""                            '暗号化キー１
                        CP.ENC_KEY2 = ""                            '暗号化キー２
                        CP.ENC_OPT1 = ""                            'ＡＥＳオプション
                        CP.CYCLENO = ""                             'サイクル№
                        CP.JOBTUUBAN = Integer.Parse(para(6))       'ジョブ通番
                    Case 8
                        CP.RENKEI_FILENAME = para(6).TrimEnd        '連携ファイル名
                        CP.ENC_KBN = ""                             '暗号化処理区分
                        CP.ENC_KEY1 = ""                            '暗号化キー１
                        CP.ENC_KEY2 = ""                            '暗号化キー２
                        CP.ENC_OPT1 = ""                            'ＡＥＳオプション
                        CP.CYCLENO = ""                             'サイクル№
                        CP.JOBTUUBAN = Integer.Parse(para(7))       'ジョブ通番
                End Select
                ' 2017/04/12 タスク）綾部 CHG 【ME】(RSV2標準機能対応) -------------------- END

            End Set
        End Property
    End Structure
    Private mKoParam As TourokuParam

    Dim mUserID As String = ""                      'ユーザＩＤ
    Dim mJobMessage As String = ""                  'ジョブメッセージ
    Private mDataFileName As String                 '依頼データファイル名
    Private mArgumentData As CommData               '起動パラメータ 共通情報
    Public MainDB As CASTCommon.MyOracle            'パブリックＤＢ
    Private ArrayPrnNenkin As New ArrayList(128)    '年金 受取人別振込明細表 出力リスト
    Private mErrMessage As String = ""              'エラーメッセージ(処理結果確認表印刷用)
    Private ArrayTenbetu As New ArrayList           '店別集計表出力対象 格納リスト

    ' 2015/12/11 タスク）綾部 ADD 【PG】UI_B-14-03(RSV2対応) -------------------- START
    Private INI_RSV2_TENBETUSYUKEI As String = ""
    ' 2015/12/11 タスク）綾部 ADD 【PG】UI_B-14-03(RSV2対応) -------------------- END

    ' 2016/01/11 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
    Private INI_RSV2_EDITION As String = ""
    Private INI_RSV2_SYORIKEKKA_TOUROKU As String = ""
    ' 2016/01/11 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END
    '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
    '受付期日チェック(0:チェックしない、1:チェックする)
    Private INI_UKETUKE_KIJITU_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "UKETUKE_KIJITU_CHK")
    '持込期日チェック(0:チェックしない、1:チェックする)
    Private INI_MOTIKOMI_KIJITU_CHK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "MOTIKOMI_KIJITU_CHK")
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

            ' 2015/12/11 タスク）綾部 ADD 【PG】UI_B-14-03(RSV2対応) -------------------- START
            '--------------------------------------------
            ' INIﾌｧｲﾙ情報設定
            '--------------------------------------------
            INI_RSV2_TENBETUSYUKEI = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "TENBETUSYUKEI")
            ' 2015/12/11 タスク）綾部 ADD 【PG】UI_B-14-03(RSV2対応) -------------------- END
            ' 2016/01/11 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
            INI_RSV2_EDITION = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
            INI_RSV2_SYORIKEKKA_TOUROKU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SYORIKEKKA_TOUROKU")
            ' 2016/01/11 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

            '--------------------------------------------
            'パラメータ情報設定
            '--------------------------------------------
            Dim bRet As Boolean                         '処理判定
            Dim InfoParam As New CommData.stPARAMETER   'パラメータ情報
            'Dim ErrFileName As String               'ファイル名

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
                        BatchLog.Write_Err("主処理", "失敗", "口振依頼データ落込処理で実行待ちタイムアウト")
                        BatchLog.UpdateJOBMASTbyErr("口振依頼データ落込処理で実行待ちタイムアウト")
                        Return -1
                    End If
                End If
                ' 2016/02/08 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END
                '処理結果確認表出力（システムエラー)
                Dim oRenkei As New ClsRenkei(mArgumentData)

                Dim Prn As New ClsPrnSyorikekkaKakuninhyo
                If Prn.OutputCSVKekkaSysError(mKoParam.CP.FSYORI_KBN,
                        mKoParam.CP.TORIS_CODE, mKoParam.CP.TORIF_CODE,
                        mKoParam.CP.JOBTUUBAN, Path.GetFileName(mKoParam.CP.RENKEI_FILENAME), mErrMessage, MainDB) = False Then
                    BatchLog.Write("処理結果確認表", "失敗", "ファイル名:" & Prn.FileName & " メッセージ:" & Prn.ReportMessage)
                End If

                Prn = Nothing

            Else
                '処理正常終了
                Dim DestFile As String = ""
                '2017/05/08 タスク）西野 ADD 標準版修正（依頼データの退避設定）---------------------- START
                Dim IRAI_BK_MODE As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "IRAI_BKUP_MODE")
                '2017/05/08 タスク）西野 ADD 標準版修正（依頼データの退避設定）---------------------- END
                Try

                    Select Case InfoParam.BAITAI_CODE
                        ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- START
                        'Case "00"                       '2010.02.04 学校はDATに
                        Case "00", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                            ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- END

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
                            '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                        Case "01", "07", "11", "12", "13", "14", "15"
                            'Case "01", "07", "11", "12", "13", "14"                 '2010.02.04 学校はDATに       ''2012/7/3  mubuchi "11"(DVD)追加
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
                            Select Case INI_RSV2_EDITION
                                Case "2"
                                    DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)
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
                                Case Else
                                    DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & "E" & mKoParam.CP.TORI_CODE & ".DAT"
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
                            ' 2016/01/27 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END
                        Case "SA"   '再振用特殊媒体区分
                            DestFile = CASTCommon.GetFSKJIni("COMMON", "ETCBK") & "S" & mKoParam.CP.TORI_CODE & ".DAT"
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
                            '2009/12/09 追加 ================================
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

            ' 2015/12/11 タスク）綾部 CHG 【PG】UI_B-14-03(RSV2対応) -------------------- START
            'If bRet = True Then
            '    '口座振替店別集計表（出力条件として不能明細区分を参照する） →　2009.11.11 参照しない
            '    Dim ExeRepo As New CAstReports.ClsExecute 'レポエージェント印刷
            '    For i As Integer = 0 To ArrayTenbetu.Count - 1
            '        Dim prnTenbetu() As String = CStr(ArrayTenbetu(i)).Split(","c)
            '        'Select Case prnTenbetu(0)
            '        '    Case "1", "2", "3"
            '        Dim Param As String = prnTenbetu(1) & prnTenbetu(2) & "," & mArgumentData.INFOParameter.FURI_DATE & ",0"
            '        Dim nRet As Integer = ExeRepo.ExecReport("KFJP019.EXE", Param)
            '        If nRet <> 0 Then
            '            BatchLog.Write("口座振替店別集計表出力", "失敗", "復帰値：" & nRet)
            '        End If
            '        'End Select
            '    Next
            'End If
            If INI_RSV2_TENBETUSYUKEI = "1" Then
                '--------------------------------------------
                ' 口座振替店別集計表印刷
                '--------------------------------------------
                If bRet = True Then
                    Dim ExeRepo As New CAstReports.ClsExecute
                    For i As Integer = 0 To ArrayTenbetu.Count - 1
                        Dim prnTenbetu() As String = CStr(ArrayTenbetu(i)).Split(","c)
                        Dim Param As String = prnTenbetu(1) & prnTenbetu(2) & "," & mArgumentData.INFOParameter.FURI_DATE & ",0"
                        Dim nRet As Integer = ExeRepo.ExecReport("KFJP019.EXE", Param)
                        If nRet <> 0 Then
                            BatchLog.Write("口座振替店別集計表出力", "失敗", "復帰値：" & nRet)
                        End If
                    Next
                End If
            End If
            ' 2015/12/11 タスク）綾部 CHG 【PG】UI_B-14-03(RSV2対応) -------------------- END

            ' 2016/01/28 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- START
            If bRet = True Then
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
            End If
            ' 2016/01/28 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END

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
                Call mArgumentData.GetTORIMAST(mKoParam.CP.TORIS_CODE, mKoParam.CP.TORIF_CODE)

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
            'If mKoParam.CP.FMT_KBN <> "04" And mKoParam.CP.FMT_KBN <> "05" Then
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
                'oReadFMT = oReadFMT.GetFormat(mArgumentData.INFOParameter)
                '*** Str Add 2015/12/26 sys)mori for エラーのため、修正 ***
                oReadFMT = CAstFormat.CFormat.GetFormat(mArgumentData.INFOParameter)
                'oReadFMT = CFormat.GetFormat(mArgumentData.INFOParameter)
                '*** end Add 2015/12/26 sys)mori for LOG強化対応 ***

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
            '    'mErrMessage = "媒体読み込み失敗"　関数内で設定
            '    Return False
            'End If
            If sReadFile = "" Then
                oReadFMT.Close()
                BatchLog.Write("(登録メイン処理)", "失敗", "媒体読み込み：" & oReadFMT.Message)
                Return False
            Else
                '--------------------------------------------
                ' 振替依頼データ(中間ファイル)の退避
                '--------------------------------------------
                Dim BKUP_CHECK As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_CHECK_IN")
                If BKUP_CHECK <> "err" Then
                    If BKUP_CHECK = "1" Then
                        Dim BKUP_BAITAI As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_BAITAI_IN")

                        If BKUP_BAITAI.IndexOf(mKoParam.CP.BAITAI_CODE) >= 0 Then
                            Dim BKUP_FOLDER As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "BKUP_FOLDER_IN")
                            Dim BKUP_FILENAME As String = BKUP_FOLDER & Path.GetFileNameWithoutExtension(sReadFile) & "_" & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & ".DAT"

                            Try
                                File.Copy(sReadFile, BKUP_FILENAME, True)
                            Catch ex As Exception
                                BatchLog.Write("(登録メイン処理)", "失敗", "振替依頼データ退避：" & ex.Message)
                            End Try
                        End If
                    End If
                End If
            End If
            ' 2016/05/16 タスク）綾部 CHG 【OM】UI_B-99-99(RSV2対応_岐阜機能調査漏れ) -------------------- END

            '--------------------------------------------
            'データチェック
            '--------------------------------------------
            '伝送，ＦＤ，ＭＴ，ＣＭＴ,ＤＶＤ
            '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
            'ソース改善
            Select Case mKoParam.CP.BAITAI_CODE
                ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- START
                'Case "00", "01", "05", "06", "04", "09", "07", "10", "11", "12", "13", "14", "15", "SA"
                Case "00", "01", "05", "06", "04", "09", "07", "10", "11", "12", "13", "14", "15", "SA",
                     "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                    ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- END

                    Dim nPos As Long = -1
                    Dim nLine As Long = 0
                    Dim nErrorCount As Long = 0
                    Dim FirstError As String = ""

                    '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- START
                    If mKoParam.CP.CODE_KBN = "2" OrElse mKoParam.CP.CODE_KBN = "3" Then
                        'レコード長が変わるため再設定する(120改もかな...)
                        oReadFMT = New CAstFormat.CFormatZengin
                    End If
                    '2017/05/24 タスク）西野 ADD 標準版修正（JIS118,119改対応）-------------------------- END

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
                            BatchLog.Write("(登録メイン処理)", "成功", "規定外文字あり：" &
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

            End Select


            'If mKoParam.CP.BAITAI_CODE = "00" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "01" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "05" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "06" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "04" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "09" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "07" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "10" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "11" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "12" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "13" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "14" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "SA" Then '媒体"04","09","SA","07"追加     '20120618 mubuchi "11"(DVD)追加
            '    '2012/06/30 標準版　WEB伝送対応　媒体"10"追加
            '    '20130606 maeda その他媒体追加


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
            '=====================================================

            '2012/06/30 標準版　WEB伝送連携-------------------------->
            If mKoParam.CP.BAITAI_CODE = "10" Then
                If UpdateWEB_RIREKIMAST() = False Then
                    BatchLog.Write("WEB履歴マスタ更新", "失敗", "")

                    oReadFMT.Close()
                    'ロールバック
                    MainDB.Rollback()
                    Return False
                End If
            End If
            '--------------------------------------------------------<

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

            '---------------------------------------------------------
            'スケジュールの有無のチェック／落し込み処理未処理の確認
            '---------------------------------------------------------
            Dim oReader As CASTCommon.MyOracleReader = Nothing

            SQL.Append("SELECT ")
            SQL.Append(" TYUUDAN_FLG_S")
            SQL.Append(",TOUROKU_FLG_S")
            SQL.Append(" FROM SCHMAST")
            SQL.Append(" WHERE TORIS_CODE_S = " & SQ(mKoParam.CP.TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_S = " & SQ(mKoParam.CP.TORIF_CODE))
            SQL.Append(" AND FURI_DATE_S  = " & SQ(mKoParam.CP.FURI_DATE))
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            SQL.Append(" FOR UPDATE WAIT " & LockWaitTime)
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            Try
                oReader = New CASTCommon.MyOracleReader(MainDB)

                If oReader.DataReader(SQL) = False Then
                    '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                    If oReader.Message <> "" Then
                        Dim errmsg As String
                        If oReader.Message.StartsWith("ORA-30006") Then
                            errmsg = "落し込み処理で実行待ちタイムアウト"
                        Else
                            errmsg = "落し込み処理ロック異常"
                        End If

                        BatchLog.Write_Err("落し込み処理", "失敗", errmsg & " 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                        Call BatchLog.UpdateJOBMASTbyErr(errmsg & " 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                        mErrMessage = errmsg
                        Return False
                    End If
                    '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                    BatchLog.Write("スケジュールなし", "失敗", " 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                    Call BatchLog.UpdateJOBMASTbyErr("スケジュールなし 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                    mErrMessage = "スケジュールなし"
                    Return False
                Else
                    '登録フラグ／中断フラグの確認
                    'Do While oReader.EOF = False
                    If oReader.GetString("TOUROKU_FLG_S") <> "0" AndAlso oReader.GetString("TYUUDAN_FLG_S") = "0" Then
                        BatchLog.Write("スケジュール", "失敗", "落し込み処理済 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                        Call BatchLog.UpdateJOBMASTbyErr("スケジュール:落し込み処理済 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                        mErrMessage = "落し込み処理済"
                    End If
                    'oReader.NextRead()
                    'Loop
                End If

                If oReader.GetString("TYUUDAN_FLG_S") <> "0" Then
                    BatchLog.Write("スケジュール", "失敗", "中断フラグ設定済 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"))
                    Call BatchLog.UpdateJOBMASTbyErr("スケジュール:中断フラグ設定済 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"))
                    mErrMessage = "中断フラグ設定済み"
                    Return False
                End If

                If oReader.GetString("TOUROKU_FLG_S") <> "0" Then
                    BatchLog.Write("スケジュール", "失敗", "落し込み処理済 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & "取引先コード：" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    Call BatchLog.UpdateJOBMASTbyErr("スケジュール:落し込み処理済 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    mErrMessage = "落し込み処理済"
                    Return False
                End If

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                ' マルチ区分の場合
                ' 同一の取引先主副コードかつマルチ区分=1の取引先マスタを検索し、取引先マスタの「代表委託者コード(ITAKU_KANRI_CODE_T)」
                ' と同じ振替日の取引先のスケジュールをロックする。（対象が0件の場合は、そのまま後続処理続行。）
                If mArgumentData.INFOToriMast.MULTI_KBN_T = "1" Then
                    oReader.Close()

                    ' 代表委託者コード取得
                    SQL = New StringBuilder(256)
                    SQL.Append("SELECT ")
                    SQL.Append(" ITAKU_KANRI_CODE_T")
                    SQL.Append(" FROM TORIMAST")
                    SQL.Append(" WHERE TORIS_CODE_T = " & SQ(mKoParam.CP.TORIS_CODE))
                    SQL.Append(" AND TORIF_CODE_T = " & SQ(mKoParam.CP.TORIF_CODE))
                    SQL.Append(" AND MULTI_KBN_T  = '1'")

                    If oReader.DataReader(SQL) = True Then
                        Dim itaku_kanri_code As String = oReader.GetString("ITAKU_KANRI_CODE_T")

                        oReader.Close()

                        ' マルチ区分時のスケジュールロック
                        SQL = New StringBuilder(256)
                        SQL.Append("SELECT FURI_DATE_S")
                        SQL.Append(" FROM SCHMAST , TORIMAST")
                        SQL.Append(" WHERE ITAKU_KANRI_CODE_T = " & SQ(itaku_kanri_code))
                        SQL.Append(" AND TORIS_CODE_T       = TORIS_CODE_S")
                        SQL.Append(" AND TORIF_CODE_T       = TORIF_CODE_S")
                        SQL.Append(" AND FURI_DATE_S        = " & SQ(mKoParam.CP.FURI_DATE))
                        SQL.Append(" FOR UPDATE OF SCHMAST.TORIS_CODE_S WAIT " & LockWaitTime)
                        If oReader.DataReader(SQL) = False Then
                            If oReader.Message <> "" Then
                                Dim errmsg As String
                                If oReader.Message.StartsWith("ORA-30006") Then
                                    errmsg = "落し込み処理で実行待ちタイムアウト"
                                Else
                                    errmsg = "落し込み処理ロック異常"
                                End If

                                BatchLog.Write_Err("落し込み処理", "失敗", errmsg & " 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                                Call BatchLog.UpdateJOBMASTbyErr(errmsg & " 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
                                mErrMessage = errmsg
                                Return False
                            End If
                        End If
                    End If
                End If
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***


                Return True

            Catch ex As Exception
                BatchLog.Write("スケジュール検索", "失敗", "振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"))
                Call BatchLog.UpdateJOBMASTbyErr("スケジュール検索失敗 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"))
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

                        ' 2016/01/11 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                        'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", .TORI_CODE))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                        'Else
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        'End If

                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                        'nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
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

                                nRetRead = MediaRead.fn_FD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
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
                        ' 2016/01/11 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

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

                        ' ''ＭＴファイル読込処理実行
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
                        ' 2016/01/11 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                        'mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "ETC") & "E" & .TORI_CODE & ".DAT"
                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
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
                        ' 2016/01/11 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END
                        oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
                        nRetRead = oRenkei.CopyToDisk(readfmt)
                        'nRetRead = clsFusion.fn_DEN_CPYTO_DISK(.TORI_CODE, mKoParam.CP.RENKEI_FILENAME, oRenkei.OutFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)

                    Case "07"       '2010.02.04 追加

                        Dim REC_LENGTH As Integer = 120
                        '依頼ファイルコピー先設定
                        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DAT") & "D" & .TORI_CODE & ".DAT"
                        oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME
                        nRetRead = oRenkei.CopyToDisk(readfmt)

                        '******20120618 mubuchi DVD対応*********************************************
                        '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                    Case "11", "12", "13", "14", "15"
                        'Case "11", "12", "13", "14"       'DVD
                        '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<

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


                        'DVD読込処理実行

                        ' 2016/01/11 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                        'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", .TORI_CODE))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", .TORI_CODE))
                        'Else
                        '    'oRenkei.InFileName = Path.Combine(GetFSKJIni("TOUROKU", "PATH"), String.Format("KD{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("COMMON", "DAT"), String.Format("F{0}.DAT", mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T))
                        'End If

                        'oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                        ''コード区分チェック用のワークファイルパス用
                        'Dim CodeCHKFileName As String = Path.Combine(GetFSKJIni("COMMON", "DATBK"), mArgumentData.INFOToriMast.FILE_NAME_T)


                        ''2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                        ''媒体コードを引数に追加し、媒体コードによって対象パスを変更するように処理を変更
                        'nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, _
                        '                                       .CODE_KBN, FTranName, msgTitle, readfmt, Baitai_Code)
                        ''nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle, readfmt)
                        ''2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<

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


                                '2013/12/24 saitou 標準版 外部媒体対応 UPD -------------------------------------------------->>>>
                                '媒体コードを引数に追加し、媒体コードによって対象パスを変更するように処理を変更
                                nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH,
                                                                       .CODE_KBN, FTranName, msgTitle, readfmt, Baitai_Code)
                                'nRetRead = MediaRead.fn_DVD_CPYTO_DISK(.TORI_CODE, mArgumentData.INFOToriMast.FILE_NAME_T, oRenkei.InFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle, readfmt)
                                '2013/12/24 saitou 標準版 外部媒体対応 UPD --------------------------------------------------<<<<

                                '2009/11/30 コード区分チェック追加===
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
                                '===================================
                        End Select
                        ' 2016/01/11 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

                        '------------------------------------------------------------
                        '元の文字コードのデータを再変換して処理に乗せる
                        '------------------------------------------------------------
                        If nRetRead = 0 Then
                            nRetRead = oRenkei.CopyToDisk(readfmt)
                        End If
                        '******20120618 mubuchi DVD対応 END*********************************************

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

                            ' 2016/01/11 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                            'If Trim(mArgumentData.INFOToriMast.FILE_NAME_T) <> "" Then
                            '    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), mArgumentData.INFOToriMast.FILE_NAME_T)
                            'Else
                            '    If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & .TORI_CODE & ".DAT"
                            '    Else
                            '        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
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
                                    mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), FileName_Edition2)
                                Case Else
                                    If Trim(mArgumentData.INFOToriMast.FILE_NAME_T) <> "" Then
                                        mKoParam.CP.RENKEI_FILENAME = Path.Combine(GetFSKJIni("WEB_DEN", "WEB_REV"), mArgumentData.INFOToriMast.FILE_NAME_T)
                                    Else
                                        If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & .TORI_CODE & ".DAT"
                                        Else
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("WEB_DEN", "WEB_REV") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                                        End If
                                    End If
                            End Select
                            ' 2016/01/11 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

                        Else
                            ' 2016/01/11 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                            '2014/05/21 saitou 標準版 MODIFY ----------------------------------------------->>>>
                            ''伝送の場合、取引先マスタのファイル名は使用しない（元に戻す）
                            'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                            'Else
                            '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
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
                                    ' 2017/04/12 タスク）綾部 CHG 【ME】(RSV2標準機能対応) -------------------- START
                                    'If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                    '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                                    'Else
                                    '    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                                    'End If
                                    If mKoParam.CP.RENKEI_FILENAME.Trim = "" Then
                                        If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                                        Else
                                            mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                                        End If
                                    End If
                                    ' 2017/04/12 タスク）綾部 CHG 【ME】(RSV2標準機能対応) -------------------- END
                            End Select
                            ' 2016/01/11 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

                            ''2010/09/08.Sakon　取引先マスタのファイル名を優先して使用する ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            'If Trim(mArgumentData.INFOToriMast.FILE_NAME_T) <> "" Then
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
                            ''If mArgumentData.INFOToriMast.MULTI_KBN_T = "0" Then
                            ''    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                            ''Else
                            ''    mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & mArgumentData.INFOToriMast.ITAKU_KANRI_CODE_T & ".DAT"
                            ''End If
                            ''+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            ''mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "D" & .TORI_CODE & ".DAT"
                            '2014/05/21 saitou 標準版 MODIFY -----------------------------------------------<<<<
                        End If

                        oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

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
                        '2011/06/15 標準版修正 再振の場合,コード区分をチェックをしない-----------START
                        '2009/11/30 コード区分チェック追加===
                        'If nRetRead = 0 Then
                        If nRetRead = 0 And Baitai_Code <> "SA" Then
                            '2011/06/15 標準版修正 再振の場合,コード区分をチェックをしない-----------END
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
                '2010/10/07.Sakon　インプットエラーを配信するか(結果コードに9をセットするか) +++++
                'インプットエラー配信可否フラグチェック
                If aReadFmt.SouInputErr = "ERR" Then
                    BatchLog.Write("インプットエラー配信可否判定", "失敗", aReadFmt.Message)
                    BatchLog.UpdateJOBMASTbyErr("インプットエラー配信可否判定失敗")
                    mErrMessage = "インプットエラー配信可否判定失敗"
                    Return False
                End If
                '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

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

                            ''金額異常 フォーマットエラー

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
                                    BatchLog.Write("明細マスタ登録処理", "失敗", nRecordCount.ToString & "行目 代表委託者コード不一致" &
                                                   " : " & DaihyouItakuCode & " - " & aReadFmt.ToriData.INFOToriMast.ITAKU_KANRI_CODE_T)
                                    BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目 代表委託者コード不一致" &
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

                        '0:非対象,1:店番ソート,2:非ソート,3:エラー分のみ,4以降拡張用
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
                            If aReadFmt.InfoMeisaiMast.KEIYAKU_KIN <> aReadFmt.InfoMeisaiMast.OLD_KIN_NO OrElse
                               aReadFmt.InfoMeisaiMast.KEIYAKU_SIT <> aReadFmt.InfoMeisaiMast.OLD_SIT_NO OrElse
                               aReadFmt.InfoMeisaiMast.KEIYAKU_KOUZA <> aReadFmt.InfoMeisaiMast.OLD_KOUZA Then
                                '2010/02/04 国税データの場合はデータ区分3のみ出力対象にする
                                If mKoParam.CP.FMT_KBN = "02" Then
                                    If aReadFmt.RecordData.Substring(0, 1) = "3" Then
                                        '初回処理(印刷フラグを立てる／ＣＳＶファイルを作成する)
                                        If bSitenYomikaePrint = False Then
                                            bSitenYomikaePrint = True
                                            PrnSitenYomikae.CreateCsvFile()
                                        End If
                                        'データ書き出し
                                        PrnSitenYomikae.OutputCsvData(aReadFmt)
                                    End If
                                Else
                                    '初回処理(印刷フラグを立てる／ＣＳＶファイルを作成する)
                                    If bSitenYomikaePrint = False Then
                                        bSitenYomikaePrint = True
                                        PrnSitenYomikae.CreateCsvFile()
                                    End If
                                    'データ書き出し
                                    PrnSitenYomikae.OutputCsvData(aReadFmt)
                                End If
                            End If
                        End If

                        '国税でない。またはデータ区分が３（国税も出る）
                        If aReadFmt.ToriData.INFOToriMast.FMT_KBN_T <> "02" OrElse aReadFmt.InfoMeisaiMast.DATA_KBN = "3" Then

                            '受付明細ＣＳＶ出力
                            If Not PrnMeisai Is Nothing Then
                                ' 0:非対象,1:店番ソート,2:非ソート,3:エラー分のみ
                                ' 2016/06/10 タスク）綾部 CHG 【PG】UI-03-01(RSV2対応<小浜信金>) -------------------- START
                                'Select Case aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T
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
                                        Dim UketukeErrOut As String = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFJMAST010_受付明細表出力区分.TXT"),
                                                                                                    aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T,
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
                    End If

                    ' 二重持込チェック(サイクル考慮を行わない。総振には必要になるかも？)
                    'If aReadFmt.ToriData.INFOToriMast.CYCLE_T = "1" Then
                    If nRecordNumber <= 6 Then
                        ' 最初の６件
                        ArrayMeiData.Add(aReadFmt.RecordData)
                    ElseIf sCheckRet = "T" Then
                        ' トレーラレコード
                        ArrayMeiData.Add(aReadFmt.RecordData)
                    End If
                    'End If


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
                            '直接持込フォーマット以外
                            'エンドレコードが存在しない場合があるのでチェックしない
                            If aReadFmt.ToriData.INFOParameter.FMT_KBN <> "TO" Then
                                BatchLog.Write("フォーマットエラー", "失敗", (nRecordCount + 1).ToString & "行目 エンドレコードがありません")
                                BatchLog.UpdateJOBMASTbyErr((nRecordCount + 1).ToString & "行目 エンドレコードがありません")
                                mErrMessage = (nRecordCount + 1).ToString & "行目 ｴﾝﾄﾞﾚｺｰﾄﾞなし"
                                bRet = False
                                Exit Do
                            End If
                        End If

                        If PrnFlag = True Then

                            ' 明細マスタを検索し，２重持込の検査を行う
                            Dim bDupicate As Boolean = False
                            'bDupicate = aReadFmt.CheckDuplicate(ArrayMeiData) '総振用
                            If bDupicate = True Then
                                mJobMessage = "二重持ち込み"
                            End If

                            '2017/12/07 タスク）西野 ADD 標準版修正：広島信金対応（持込期日対応）---------------------- START
                            If INI_UKETUKE_KIJITU_CHK = "1" Then
                                '受付期日チェック
                                MotikomiOver = aReadFmt.CheckKaisiDate(aReadFmt.ToriData.INFOParameter.FURI_DATE)
                                If MotikomiOver = False Then
                                    mJobMessage = "受付開始前"
                                    mArgumentData.Syorikekka_Bikou = mJobMessage    '処理結果確認表の備考欄に出力
                                    BatchLog.Write("受付期日チェック処理", "成功", mJobMessage)
                                End If
                            End If

                            If INI_MOTIKOMI_KIJITU_CHK = "1" Then
                                '受付期日チェックがエラーでなければチェックする
                                If MotikomiOver = True Then
                                    '持込期日チェック
                                    MotikomiOver = aReadFmt.CheckMotikomiDate()
                                    If MotikomiOver = False Then
                                        mJobMessage = "持込期日超過"
                                        mArgumentData.Syorikekka_Bikou = mJobMessage    '処理結果確認表の備考欄に出力
                                        BatchLog.Write("持込期日チェック処理", "成功", mJobMessage)
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
                                        PrnInErrList.HostCsvName = "KFJP061_"
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                        PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                        PrnInErrList.HostCsvName &= "0000.CSV"
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
                                    Dim SortParam As String = CASTCommon.GetText_CodeToName(Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "KFJMAST010_受付明細表出力区分.TXT"),
                                                                                            aReadFmt.ToriData.INFOToriMast.UMEISAI_KBN_T,
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
                                        PrnMeisai.HostCsvName = "KFJP003_"
                                        PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                        PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                        PrnMeisai.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                        PrnMeisai.HostCsvName &= "0000.CSV"
                                        ArrayPrnMeisai.Add(PrnMeisai)
                                        PrnMeisai = Nothing
                                    End If
                                    IOfileStream.Close()
                                    IOfileStream = Nothing
                                    'ArrayPrnMeisai.Add(PrnMeisai)
                                    'PrnMeisai = Nothing
                                    ' 2016/03/03 タスク）岩城 ADD 【PG】UI_B-14-99(RSV2対応) -------------------- END
                                End If
                            End If

                            '店別集計表印刷用リストに格納(印刷区分,取引先主コード,取引先副コード)
                            ArrayTenbetu.Add(mArgumentData.INFOToriMast.FUNOU_MEISAI_KBN_T & "," &
                                             aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "," &
                                             aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T)

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
                        BatchLog.Write_Err("主処理", "失敗", "口振依頼データ落込処理で実行待ちタイムアウト")
                        BatchLog.UpdateJOBMASTbyErr("口振依頼データ落込処理で実行待ちタイムアウト")
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
                                PrnInErrList.HostCsvName = "KFJP061_"
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T
                                PrnInErrList.HostCsvName &= aReadFmt.ToriData.INFOParameter.FURI_DATE
                                PrnInErrList.HostCsvName &= "0000.CSV"
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
                                    BatchLog.Write("インプットエラーリストＣＳＶ出力処理", "失敗", ex.Message)
                                End Try
                            End If
                            'Try
                            '    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                            '    DestName &= PrnInErrList.HostCsvName
                            '    File.Copy(PrnInErrList.FileName, DestName, True)
                            '    BatchLog.Write("インプットエラーＣＳＶ出力処理", "成功", DestName)
                            '    PrnInErrList = Nothing
                            'Catch ex As Exception
                            '    BatchLog.Write("インプットエラーリストＣＳＶ出力処理", "失敗", ex.Message)
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

                '複数エンドレコードを通す
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

                    'Try
                    '    Dim DestName As String = CASTCommon.GetFSKJIni("COMMON", "HOST-PRT")
                    '    DestName &= PrnMeisai.HostCsvName

                    '    If File.Exists(PrnMeisai.FileName) = True Then
                    '        File.Copy(PrnMeisai.FileName, DestName, True)
                    '    Else
                    '        BatchLog.Write("受付明細表出力 明細０件", "成功", "出力帳票なし")
                    '    End If

                    '    PrnMeisai = Nothing
                    'Catch ex As Exception
                    '    BatchLog.Write("受付明細表ＣＳＶ出力処理", "失敗", ex.Message)
                    'End Try

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
                    '処理結果確認表を印刷する
                    BatchLog.Write("インプットチェック", "失敗", ErrText)
                    BatchLog.UpdateJOBMASTbyErr("インプットチェックエラー " & ErrText)
                    mErrMessage = ErrText
                    'BatchLog.JobMastMessage = ErrText
                    Return False
                End If

                '処理結果確認表を印刷する
                ' 2016/01/13 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- START
                'If Not PrnSyoKekka Is Nothing Then
                '    If PrnSyoKekka.ReportExecute() = False Then
                '        BatchLog.Write("処理結果確認表出力", "失敗", "ファイル名:" & PrnSyoKekka.FileName & " メッセージ:" & PrnSyoKekka.ReportMessage)
                '    End If
                'End If
                '2017/12/07 タスク）西野 CHG 標準版修正：広島信金対応（持込期日対応）---------------------- START
                '受付期日／持込期日チェックエラーの場合は出力する（各チェック機能が有効の場合）
                If INI_RSV2_SYORIKEKKA_TOUROKU = "1" OrElse MotikomiOver = False Then
                    'If INI_RSV2_SYORIKEKKA_TOUROKU = "1" Then
                    '2017/12/07 タスク）西野 CHG 標準版修正：広島信金対応（持込期日対応）---------------------- END
                    If Not PrnSyoKekka Is Nothing Then
                        If PrnSyoKekka.ReportExecute() = False Then
                            BatchLog.Write("処理結果確認表出力", "失敗", "ファイル名:" & PrnSyoKekka.FileName & " メッセージ:" & PrnSyoKekka.ReportMessage)
                        End If
                    End If
                End If
                ' 2016/01/13 タスク）綾部 CHG 【PG】UI_B-14-01(RSV2対応) -------------------- END

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
            BatchLog.Write("0000000000-00", "00000000", "ログイン(開始)", "成功")

            SQL = New System.Text.StringBuilder("INSERT INTO ", 2048)
            SQL.Append(" MEIMAST(")
            SQL.Append(" FSYORI_KBN_K")
            SQL.Append(",TORIS_CODE_K")
            SQL.Append(",TORIF_CODE_K")
            SQL.Append(",FURI_DATE_K")
            SQL.Append(",FURI_CODE_K")
            SQL.Append(",KIGYO_CODE_K")
            SQL.Append(",KIGYO_SEQ_K")
            SQL.Append(",ITAKU_KIN_K")
            SQL.Append(",ITAKU_SIT_K")
            SQL.Append(",ITAKU_KAMOKU_K")
            SQL.Append(",ITAKU_KOUZA_K")
            SQL.Append(",KEIYAKU_KIN_K")
            SQL.Append(",KEIYAKU_SIT_K")
            SQL.Append(",KEIYAKU_NO_K")
            SQL.Append(",KEIYAKU_KNAME_K")
            SQL.Append(",KEIYAKU_KAMOKU_K")
            SQL.Append(",KEIYAKU_KOUZA_K")
            SQL.Append(",FURIKIN_K")
            SQL.Append(",FURIKETU_CODE_K")
            SQL.Append(",FURIKETU_CENTERCODE_K")
            SQL.Append(",SINKI_CODE_K")
            SQL.Append(",NS_KBN_K")
            SQL.Append(",TEKIYO_KBN_K")
            SQL.Append(",KTEKIYO_K")
            SQL.Append(",NTEKIYO_K")
            SQL.Append(",JYUYOUKA_NO_K")
            SQL.Append(",MINASI_K")
            SQL.Append(",TEISEI_SIT_K")
            SQL.Append(",TEISEI_KAMOKU_K")
            SQL.Append(",TEISEI_KOUZA_K")
            SQL.Append(",TEISEI_AKAMOKU_K")
            SQL.Append(",TEISEI_AKOUZA_K")
            SQL.Append(",DATA_KBN_K")
            SQL.Append(",FURI_DATA_K")
            SQL.Append(",RECORD_NO_K")
            SQL.Append(",SORT_KEY_K")
            SQL.Append(",SAKUSEI_DATE_K")
            SQL.Append(",KOUSIN_DATE_K")
            SQL.Append(",YOBI1_K")
            SQL.Append(",YOBI2_K")
            SQL.Append(",YOBI3_K")
            SQL.Append(",YOBI4_K")
            SQL.Append(",YOBI5_K")
            SQL.Append(",YOBI6_K")
            SQL.Append(",YOBI7_K")
            SQL.Append(",YOBI8_K")
            SQL.Append(",YOBI9_K")
            SQL.Append(",YOBI10_K")

            If stTori.FSYORI_KBN_T = "3" Then
                SQL.Append(",CYCLE_NO_K")
            ElseIf aReadFmt.BinMode Then
                SQL.Append(",BIN_DATA_K")
            End If

            SQL.Append(")")
            SQL.Append(" VALUES(")
            SQL.Append(" :FSYORI_KBN")
            SQL.Append(",:TORIS_CODE")                              'TORIS_CODE_K             
            SQL.Append(",:TORIF_CODE")                              'TORIF_CODE_K
            SQL.Append(",:FURI_DATE")                               'FURI_DATE_K
            SQL.Append(",:FURI_CODE")                               'FURI_CODE_K
            SQL.Append(",:KIGYO_CODE")                              'KIGYO_CODE_K
            SQL.Append(",:KIGYO_SEQ")                               'KIGYO_SEQ_K
            SQL.Append(",:ITAKU_KIN")                               'ITAKU_KIN_K
            SQL.Append(",:ITAKU_SIT")                               'ITAKU_SIT_K
            SQL.Append(",:ITAKU_KAMOKU")                            'ITAKU_KAMOKU_K
            SQL.Append(",:ITAKU_KOUZA")                             'ITAKU_KOUZA_K
            SQL.Append(",:KEIYAKU_KIN")                             'KEIYAKU_KIN_K
            SQL.Append(",:KEIYAKU_SIT")                             'KEIYAKU_SIT_K
            SQL.Append(",:KEIYAKU_NO")                              'KEIYAKU_NO_K
            SQL.Append(",:KEIYAKU_KNAME")                           'KEIYAKU_KNAME_K
            SQL.Append(",:KEIYAKU_KAMOKU")                          'KEIYAKU_KAMOKU_K
            SQL.Append(",:KEIYAKU_KOUZA")                           'KEIYAKU_KOUZA_K
            SQL.Append(",:FURIKIN")                                 'FURIKIN_K
            SQL.Append(",:FURIKETU_CODE")                           'FURIKETU_CODE_K 
            SQL.Append(",:FURIKETU_CENTERCODE")                     'FURIKETU_SENTERCODE_K
            SQL.Append(",:SINKI_CODE")                              'SINKI_CODE_K
            SQL.Append(",:NS_KBN")                                  'NS_KBN_K
            SQL.Append(",:TEKIYOU_KBN")                             'TEKIYO_KBN_K
            SQL.Append(",:KTEKIYO")                                 'KTEKIYO_K
            SQL.Append(",:NTEKIYOU")                                'NTEKIYO_K
            SQL.Append(",:JYUYOKA_NO")                              'JYUYOUKA_NO_K
            SQL.Append(",'0'")                                      'MINASI_K
            SQL.Append(",:TEISEI_SIT")                              'TEISEI_SIT_K
            SQL.Append(",:TEISEI_KAMOKU")                           'TEISEI_KAMOKU_K
            SQL.Append(",:TEISEI_KOUZA")                            'TEISEI_KOUZA_K
            SQL.Append(",'00'")                                     'TEISEI_AKAMOKU_K
            SQL.Append(",'0000000'")                                'TEISEI_AKOUZA_K
            SQL.Append(",:DATA_KBN")                                'DATA_KBN_K
            SQL.Append(",:FURI_DATA")                               'FURI_DATA_K
            SQL.Append(",:RECORD_NO")                               'RECORD_NO_K
            SQL.Append(",' '")                                      'SORT_KEY_K
            SQL.Append(",:SAKUSEI_DATE")                            'SAKUSEI_DATE_K
            SQL.Append(",'00000000'")                               'KOUSIN_DATE_K
            SQL.Append(",:YOBI1")                                   'YOBI1_K
            SQL.Append(",:YOBI2")                                   'YOBI2_K
            SQL.Append(",:YOBI3")                                   'YOBI3_K
            '2010/01/19 需要家番号を変換したものを格納
            'SQL.Append(",' '")                                      'YOBI4_K    
            SQL.Append(",:YOBI4")                                   'YOBI4_K    
            '=========================================

            '2010/09/21.Sakon　予備５にはエラーメッセージを格納 ++++++++++++++++++++++++++++++++++++++++++++++++
            SQL.Append(",:YOBI5")                                   'YOBI5_K
            'SQL.Append(",' '")                                      'YOBI5_K
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            '*** Str Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
            'SQL.Append(",' '")                                      'YOBI6_K
            'SQL.Append(",' '")                                      'YOBI7_K
            'SQL.Append(",' '")                                      'YOBI8_K
            'SQL.Append(",' '")                                      'YOBI9_K
            'SQL.Append(",' '")                                      'YOBI10_K
            SQL.Append(",:YOBI6")                                   'YOBI6_K
            SQL.Append(",:YOBI7")                                   'YOBI7_K
            SQL.Append(",:YOBI8")                                   'YOBI8_K
            SQL.Append(",:YOBI9")                                   'YOBI9_K
            SQL.Append(",:YOBI10")                                  'YOBI10_K
            '*** End Upd 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

            If aReadFmt.BinMode Then
                '口振かつEBCDICの場合
                SQL.Append(",:BIN")                                 'BIN_DATA_K
                SQL.Append(")")
            Else
                '口振の場合
                SQL.Append(")")
            End If

            MainDB.CommandText = SQL.ToString

        Catch ex As Exception
            BatchLog.Write("明細マスタ登録クエリ生成", "失敗", ex.Message)
            Return False
        Finally
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
                .Item("FURI_CODE") = stTori.FURI_CODE_T
                .Item("KIGYO_CODE") = stTori.KIGYO_CODE_T
                .Item("KIGYO_SEQ") = stMei.KIGYO_SEQ
                .Item("ITAKU_KIN") = stMei.ITAKU_KIN
                .Item("ITAKU_SIT") = stMei.ITAKU_SIT
                .Item("ITAKU_KAMOKU") = stMei.ITAKU_KAMOKU
                .Item("ITAKU_KOUZA") = stMei.ITAKU_KOUZA
                .Item("KEIYAKU_KIN") = stMei.KEIYAKU_KIN
                .Item("KEIYAKU_SIT") = stMei.KEIYAKU_SIT
                .Item("KEIYAKU_NO") = stMei.KEIYAKU_NO
                .Item("KEIYAKU_KNAME") = stMei.KEIYAKU_KNAME
                .Item("KEIYAKU_KAMOKU") = CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)
                .Item("KEIYAKU_KOUZA") = stMei.KEIYAKU_KOUZA
                .Item("FURIKIN") = stMei.FURIKIN
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
                Dim W_JYUYOUKA As String = ""
                Dim IN_JYUYOUKA As String = stMei.JYUYOKA_NO.PadRight(24)
                For LOO As Integer = 1 To 24
                    Select Case Asc(Mid(IN_JYUYOUKA, LOO, 1))
                        Case 48 To 57, 65 To 90, 40 To 41, 32
                            W_JYUYOUKA &= Mid(IN_JYUYOUKA, LOO, 1)
                        Case Else
                            W_JYUYOUKA &= "0"
                    End Select
                Next
                .Item("YOBI4") = W_JYUYOUKA
                '===================================

                '2010/09/21.Sakon　受付明細随時出力対応：予備５にエラー情報を格納 ++++++++++++++++++++++++++++++
                If aReadFmt.InErrorArray.Count = 0 OrElse stMei.DATA_KBN <> "2" Then
                    .Item("YOBI5") = ""
                Else
                    .Item("YOBI5") = CType(aReadFmt.InErrorArray(0), CAstFormat.CFormat.INPUTERROR).ERRINFO
                End If
                '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                '*** Str Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***
                .Item("YOBI6") = stMei.YOBI6
                .Item("YOBI7") = stMei.YOBI7
                .Item("YOBI8") = stMei.YOBI8
                .Item("YOBI9") = stMei.YOBI9
                .Item("YOBI10") = stMei.YOBI10
                '*** End Add 2015/12/01 SO)荒木 for XMLフォーマット変換対応 ***

                If stTori.FSYORI_KBN_T = "1" Then

                    ' 口振の場合
                    If aReadFmt.BinMode Then
                        .Item("BIN") = aReadFmt.RecordDataBin
                    End If

                End If

                If MainDB.ExecuteNonQuery() <= 0 Then
                    BatchLog.Write("明細マスタ登録", "失敗", MainDB.Message)
                    Return False
                End If

                '2013/12/24 saitou 標準版 DEL -------------------------------------------------->>>>
                '年金はセンター直接持込に変更
                ''年金の場合は，NENKINMAST へもINSERT する
                'If aReadFmt.ToriData.INFOParameter.FMT_KBN = "03" Then
                '    If InsertNENKINMAST(aReadFmt) = False Then
                '        Return False
                '    End If
                'End If
                '2013/12/24 saitou 標準版 DEL --------------------------------------------------<<<<

            End With

        Catch ex As Exception
            BatchLog.Write("明細マスタ登録", "失敗", ex.Message)
            mErrMessage = "明細マスタ登録失敗"
            Return False
        Finally
        End Try

        Return True
    End Function

    '2013/12/24 saitou 標準版 DEL -------------------------------------------------->>>>
    '年金はセンター直接持込に変更
    '' 機能　 ： 年金マスタ INSERT
    ''
    '' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    ''
    '' 備考　 ： 旧InsertMEIMASTとほぼ同一
    ''
    'Private Function InsertNENKINMAST(ByRef aReadFmt As CAstFormat.CFormat) As Boolean
    '    Dim SQL As StringBuilder         ' ＳＱＬ

    '    Dim stTori As CAstBatch.CommData.stTORIMAST = aReadFmt.ToriData.INFOToriMast ' 取引先情報
    '    Dim stMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast     ' 明細マスタ情報

    '    Try
    '        SQL = New System.Text.StringBuilder("INSERT INTO ", 2048)
    '        SQL.Append(" NENKINMAST(")
    '        SQL.Append(" FSYORI_KBN_K")
    '        SQL.Append(",TORIS_CODE_K")
    '        SQL.Append(",TORIF_CODE_K")
    '        SQL.Append(",FURI_DATE_K")
    '        SQL.Append(",KIGYO_CODE_K")
    '        SQL.Append(",KIGYO_SEQ_K")
    '        SQL.Append(",ITAKU_KIN_K")
    '        SQL.Append(",ITAKU_SIT_K")
    '        SQL.Append(",ITAKU_KAMOKU_K")
    '        SQL.Append(",ITAKU_KOUZA_K")
    '        SQL.Append(",KEIYAKU_KIN_K")
    '        SQL.Append(",KEIYAKU_SIT_K")
    '        SQL.Append(",KEIYAKU_NO_K")
    '        SQL.Append(",KEIYAKU_KNAME_K")
    '        SQL.Append(",KEIYAKU_KAMOKU_K")
    '        SQL.Append(",KEIYAKU_KOUZA_K")
    '        SQL.Append(",FURIKIN_K")
    '        SQL.Append(",FURIKETU_CODE_K")
    '        SQL.Append(",FURIKETU_CENTERCODE_K")
    '        SQL.Append(",SINKI_CODE_K")
    '        SQL.Append(",NS_KBN_K")
    '        SQL.Append(",TEKIYO_KBN_K")
    '        SQL.Append(",KTEKIYO_K")
    '        SQL.Append(",NTEKIYO_K")
    '        SQL.Append(",JYUYOUKA_NO_K")
    '        SQL.Append(",MINASI_K")
    '        SQL.Append(",TEISEI_SIT_K")
    '        SQL.Append(",TEISEI_KAMOKU_K")
    '        SQL.Append(",TEISEI_KOUZA_K")
    '        SQL.Append(",TEISEI_AKAMOKU_K")
    '        SQL.Append(",TEISEI_AKOUZA_K")
    '        SQL.Append(",DATA_KBN_K")
    '        SQL.Append(",FURI_DATA_K")
    '        SQL.Append(",RECORD_NO_K")
    '        SQL.Append(",SORT_KEY_K")
    '        SQL.Append(",SAKUSEI_DATE_K")
    '        SQL.Append(",KOUSIN_DATE_K")
    '        SQL.Append(",YOBI1_K")
    '        SQL.Append(",YOBI2_K")
    '        SQL.Append(",YOBI3_K")
    '        SQL.Append(",YOBI4_K")
    '        SQL.Append(",YOBI5_K")
    '        SQL.Append(",YOBI6_K")
    '        SQL.Append(",YOBI7_K")
    '        SQL.Append(",YOBI8_K")
    '        SQL.Append(",YOBI9_K")
    '        SQL.Append(",YOBI10_K")

    '        SQL.Append(")")
    '        SQL.Append(" VALUES(")
    '        SQL.Append(" " & SQ(stTori.FSYORI_KBN_T))                               ' FSYORI_KBN_K
    '        SQL.Append("," & SQ(stTori.TORIS_CODE_T))                               ' TORIS_CODE_K             
    '        SQL.Append("," & SQ(stTori.TORIF_CODE_T))                               ' TORIF_CODE_K
    '        SQL.Append("," & SQ(stMei.FURIKAE_DATE))                                ' FURI_DATE_K
    '        SQL.Append("," & SQ(stTori.KIGYO_CODE_T))                               ' KIGYO_CODE_K
    '        SQL.Append("," & SQ(stMei.KIGYO_SEQ))                                   ' KIGYO_SEQ_K
    '        SQL.Append("," & SQ(stMei.ITAKU_KIN))                                   ' ITAKU_KIN_K
    '        SQL.Append("," & SQ(stMei.ITAKU_SIT))                                   ' ITAKU_SIT_K
    '        SQL.Append("," & SQ(stMei.ITAKU_KAMOKU))                                ' ITAKU_KAMOKU_K
    '        SQL.Append("," & SQ(stMei.ITAKU_KOUZA))                                 ' ITAKU_KOUZA_K
    '        SQL.Append("," & SQ(stMei.KEIYAKU_KIN))                                 ' KEIYAKU_KIN_K
    '        SQL.Append("," & SQ(stMei.KEIYAKU_SIT))                                 ' KEIYAKU_SIT_K
    '        SQL.Append("," & SQ(stMei.KEIYAKU_NO))                                  ' KEIYAKU_NO_K
    '        SQL.Append("," & SQ(stMei.KEIYAKU_KNAME))                               ' KEIYAKU_KNAME_K
    '        SQL.Append("," & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU))) ' KEIYAKU_KAMOKU_K
    '        SQL.Append("," & SQ(stMei.KEIYAKU_KOUZA))                               ' KEIYAKU_KOUZA_K
    '        SQL.Append("," & stMei.FURIKIN)                                         ' FURIKIN_K
    '        SQL.Append("," & stMei.FURIKETU_CODE)                                   ' FURIKETU_CODE_K 
    '        SQL.Append("," & stMei.FURIKETU_CENTERCODE)                             ' FURIKETU_SENTERCODE_K
    '        SQL.Append("," & SQ(stMei.SINKI_CODE))                                  ' SINKI_CODE_K
    '        SQL.Append("," & SQ(stTori.NS_KBN_T))                                   ' NS_KBN_K
    '        SQL.Append("," & SQ(stTori.TEKIYOU_KBN_T))                              ' TEKIYO_KBN_K
    '        SQL.Append("," & SQ(stMei.KTEKIYO))                                     ' KTEKIYO_K
    '        SQL.Append("," & SQ(stTori.NTEKIYOU_T))                                 ' NTEKIYO_K
    '        SQL.Append("," & SQ(stMei.JYUYOKA_NO))                                  ' JYUYOUKA_NO_K
    '        SQL.Append(",'0'")                                                      ' MINASI_K
    '        SQL.Append("," & SQ(stMei.TEISEI_SIT))                                  ' TEISEI_SIT_K
    '        SQL.Append("," & SQ(stMei.TEISEI_KAMOKU))                               ' TEISEI_KAMOKU_K
    '        SQL.Append("," & SQ(stMei.TEISEI_KOUZA))                                ' TEISEI_KOUZA_K
    '        SQL.Append(",'00'")                                                     ' TEISEI_AKAMOKU_K
    '        SQL.Append(",'0000000'")                                                ' TEISEI_AKOUZA_K
    '        SQL.Append("," & SQ(stMei.DATA_KBN))                                    ' DATA_KBN_K
    '        SQL.Append("," & SQ(aReadFmt.RecordDataNoChange))                       ' FURI_DATA_K
    '        SQL.Append("," & stMei.RECORD_NO.ToString)                              ' RECORD_NO_K
    '        SQL.Append(",' '")                                                      ' SORT_KEY_K
    '        SQL.Append("," & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))      ' SAKUSEI_DATE_K
    '        SQL.Append(",'00000000'")                                               ' KOUSIN_DATE_K
    '        SQL.Append("," & SQ(stMei.YOBI1))                                       ' YOBI1_K
    '        SQL.Append("," & SQ(stMei.YOBI2))                                       ' YOBI2_K
    '        SQL.Append("," & SQ(stMei.YOBI3))                                       ' YOBI3_K
    '        SQL.Append(",' '")                                                      ' YOBI4_K
    '        SQL.Append(",' '")                                                      ' YOBI5_K
    '        SQL.Append(",' '")                                                      ' YOBI6_K
    '        SQL.Append(",' '")                                                      ' YOBI7_K
    '        SQL.Append(",' '")                                                      ' YOBI8_K
    '        SQL.Append(",' '")                                                      ' YOBI9_K
    '        SQL.Append(",' '")                                                      ' YOBI10_K
    '        SQL.Append(")")

    '        If MainDB.ExecuteNonQuery(SQL) <= 0 Then
    '            BatchLog.Write("年金マスタ登録", "失敗", MainDB.Message)
    '            Return False
    '        End If

    '    Catch ex As Exception
    '        BatchLog.Write("年金マスタ登録", "失敗", ex.Message)
    '        Return False
    '    End Try

    '    Return True
    'End Function
    '2013/12/24 saitou 標準版 DEL --------------------------------------------------<<<<

    ' 機能　 ： 明細マスタ企業シーケンス UPDATE
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： センター直接持込フォーマット国税データ用
    '
    Private Function UpdateKigyoSEQ(ByRef aReadFmt As CAstFormat.CFormat) As Boolean
        Dim stTori As CAstBatch.CommData.stTORIMAST = aReadFmt.ToriData.INFOToriMast ' 取引先情報
        Dim stMei As CAstFormat.CFormat.MEISAI = aReadFmt.InfoMeisaiMast     ' 明細マスタ情報

        'データレコードのみ更新する
        If stMei.DATA_KBN <> "2" Then
            Return True
        End If

        Try

            Dim SQL As StringBuilder = New System.Text.StringBuilder(512)
            SQL.Append("UPDATE MEIMAST SET")
            SQL.Append(" KIGYO_SEQ_K = " & SQ(stMei.KIGYO_SEQ))
            SQL.Append(", KOUSIN_DATE_K = " & SQ(CASTCommon.Calendar.Now.ToString("yyyyMMdd")))
            SQL.Append(" WHERE FSYORI_KBN_K = '1'")
            SQL.Append(" AND TORIS_CODE_K = " & SQ(stTori.TORIS_CODE_T))
            SQL.Append(" AND TORIF_CODE_K = " & SQ(stTori.TORIF_CODE_T))
            SQL.Append(" AND FURI_DATE_K = " & SQ(stMei.FURIKAE_DATE))
            SQL.Append(" AND DATA_KBN_K = '3'")
            SQL.Append(" AND KEIYAKU_KIN_K = " & SQ(stMei.KEIYAKU_KIN))
            SQL.Append(" AND KEIYAKU_SIT_K = " & SQ(stMei.KEIYAKU_SIT))
            SQL.Append(" AND KEIYAKU_KAMOKU_K = " & SQ(CASTCommon.ConvertKamoku1TO2(stMei.KEIYAKU_KAMOKU)))
            SQL.Append(" AND KEIYAKU_KOUZA_K = " & SQ(stMei.KEIYAKU_KOUZA))
            SQL.Append(" AND FURIKIN_K = " & stMei.FURIKIN)
            '全角文字対応
            SQL.Append(" AND SUBSTRB(FURI_DATA_K, 4, 5) = " & SQ(stMei.JYUYOKA_NO.Substring(0, 5))) '局署番号
            SQL.Append(" AND SUBSTRB(FURI_DATA_K, 53, 8) = " & SQ(stMei.JYUYOKA_NO.Substring(5, 8))) ' 整理番号

            '一意の明細を更新出来なければエラーとする
            If MainDB.ExecuteNonQuery(SQL) <> 1 Then
                BatchLog.Write("明細マスタ（企業シーケンス）更新", "失敗", "明細更新件数が一意でない " & MainDB.Message)
                Return False
            End If

        Catch ex As Exception
            BatchLog.Write("明細マスタ（企業シーケンス）更新", "失敗", ex.Message)
            Return False
        Finally
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
            BatchLog.Write("0000000000-00", "00000000", "ログイン(開始)", "成功")

            For I As Integer = 1 To 2
                Dim SQL As StringBuilder = New System.Text.StringBuilder(512)
                If I = 1 Then
                    SQL.Append("UPDATE ENTMAST SET")
                Else
                    SQL.Append("UPDATE FUKU_ENTMAST SET")
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
        Finally
        End Try
        Return True
    End Function
    '' 機能　 ： 国税　取引先取得
    ''
    '' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    ''
    '' 備考　 ： 
    ''
    'Private Function GetKokuzeiTORIMAST() As String
    '    Dim SQL As New StringBuilder(128)
    '    Dim OraReader As CASTCommon.MyOracleReader

    '    Dim KokuzeiFmt As New CAstFormat.CFormatKokuzei
    '    Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData)
    '    Call KokuzeiFmt.FirstRead(oRenkei.InFileName)
    '    KokuzeiFmt.GetFileData(KokuzeiFmt.KOKUZEI_REC1.Data)
    '    Dim Kamoku As String = KokuzeiFmt.KOKUZEI_REC1.KZ4
    '    KokuzeiFmt.Close()
    '    oRenkei = Nothing

    '    Dim ToriCode As String
    '    '科目コード　020:申告所得税, 300:消費税及地方消費税
    '    ToriCode = CASTCommon.GetFSKJIni("TOUROKU", "KOKUZEI" & Kamoku)
    '    If ToriCode <> "err" Then
    '        Return ToriCode
    '    End If

    '    '基本的には，上記のＩＮＩファイルから取得となるが，ＩＮＩが見つからない場合ＤＢから検索
    '    SQL.Append("SELECT ")
    '    SQL.Append(" TORIS_CODE_T")
    '    SQL.Append(",TORIF_CODE_T")
    '    SQL.Append(" FROM TORIMAST")

    '    '2009/09/08.暫定対応　連携区分とりのぞく +++++++
    '    'SQL.Append(" WHERE RENKEI_KBN_T = " & SQ(mKoParam.CP.RENKEI_KBN))
    '    'SQL.Append("   AND FMT_KBN_T = '02'")
    '    SQL.Append(" WHERE FMT_KBN_T = '02'")
    '    '+++++++++++++++++++++++++++++++++++++++++++++++

    '    SQL.Append("   AND " & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & " BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")
    '    SQL.Append(" ORDER BY TORIS_CODE_T ASC, TORIF_CODE_T ASC")
    '    Try
    '        OraReader = New CASTCommon.MyOracleReader(MainDB)
    '        If OraReader.DataReader(SQL) = True Then
    '            If Kamoku <> "020" Then
    '                OraReader.NextRead()
    '                If OraReader.EOF = True Then
    '                    OraReader.DataReader(SQL)
    '                End If
    '            End If
    '            ToriCode = OraReader.GetString("TORIS_CODE_T") & OraReader.GetString("TORIF_CODE_T")
    '            Return ToriCode.PadRight(9, " "c)
    '        End If
    '    Catch ex As Exception

    '    Finally
    '        If Not OraReader Is Nothing Then
    '            OraReader.Close()
    '        End If
    '        OraReader = Nothing
    '    End Try

    '    Return New String(" "c, 9)
    'End Function

    '2013/12/24 saitou 標準版 DEL -------------------------------------------------->>>>
    '年金はセンター直接持込に変更
    '' 機能　 ： 年金　取引先取得
    ''
    '' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    ''
    '' 備考　 ： 
    ''
    'Private Function GetNenkinTORIMAST() As String
    '    Dim SQL As New StringBuilder(128)
    '    Dim NenkinFmt As New CAstFormat.CFormatNenkin
    '    Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData)
    '    Dim Kamoku As String = ""
    '    Dim ToriCode As String

    '    Call NenkinFmt.FirstRead(oRenkei.InFileName)
    '    NenkinFmt.GetFileData(NenkinFmt.NENKIN_REC1.Data)

    '    ' 年金種別から判断 61:旧厚生年金,62:旧船員年金,63:旧国民年金,64:労災年金,65:新国民年金・厚生年金,66:新船員年金,67:旧国民年金短期
    '    Kamoku = NenkinFmt.NENKIN_REC1.NK2
    '    NenkinFmt.Close()
    '    oRenkei = Nothing

    '    ' 科目コード　
    '    If Kamoku.Trim <> "" Then
    '        ToriCode = CASTCommon.GetFSKJIni("TOUROKU", "NENKIN" & Kamoku)
    '        If ToriCode <> "err" Then
    '            Return ToriCode
    '        End If
    '    End If

    '    Return New String(" "c, 9)
    'End Function
    '2013/12/24 saitou 標準版 DEL --------------------------------------------------<<<<

    '2013/12/24 saitou 標準版 DEL -------------------------------------------------->>>>
    '金庫持込の落し込み処理なので、ここでセンター直接持込分の取引先を参照することはないためコメントアウト
    '' 機能　 ： センター直接持ち込み　取引先取得
    ''
    '' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    ''
    '' 備考　 ： 
    ''
    'Private Sub GetCenterTORIMAST(ByVal furiCode As String, ByVal kigyoCode As String)
    '    Dim SQL As New StringBuilder(128)
    '    Dim OraReader As CASTCommon.MyOracleReader = Nothing

    '    SQL.Append("SELECT ")
    '    SQL.Append(" TORIS_CODE_T")
    '    SQL.Append(",TORIF_CODE_T")
    '    SQL.Append(" FROM TORIMAST")
    '    SQL.Append(" WHERE FURI_CODE_T = " & SQ(furiCode))
    '    SQL.Append("   AND KIGYO_CODE_T = " & SQ(kigyoCode))
    '    SQL.Append("   AND MOTIKOMI_KBN_T = '1'")
    '    SQL.Append("   AND " & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & " BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")

    '    Try
    '        OraReader = New CASTCommon.MyOracleReader(MainDB)
    '        If OraReader.DataReader(SQL) = True Then
    '            mKoParam.CP.TORIS_CODE = OraReader.GetString("TORIS_CODE_T")
    '            mKoParam.CP.TORIF_CODE = OraReader.GetString("TORIF_CODE_T")
    '            mKoParam.CP.TORI_CODE = mKoParam.CP.TORIS_CODE & mKoParam.CP.TORIF_CODE
    '        End If
    '    Catch ex As Exception

    '    Finally
    '        If Not OraReader Is Nothing Then
    '            OraReader.Close()
    '        End If
    '        OraReader = Nothing
    '    End Try
    'End Sub
    '2013/12/24 saitou 標準版 DEL --------------------------------------------------<<<<

    '2013/12/24 saitou 標準版 DEL -------------------------------------------------->>>>
    'きっと大規模版の機能なのでコメントアウト
    '' 機能　 ： 個別前処理
    ''
    '' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    ''
    '' 備考　 ： 
    ''
    'Private Sub KobetuExecute(ByVal fileName As String, ByVal key As CommData)
    '    Dim StrReader As StreamReader = Nothing
    '    Try
    '        Dim MaeSyoriKey As String
    '        Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
    '        Dim SQL As New StringBuilder(128)
    '        Dim TxtName As String

    '        SQL.Append("SELECT MAE_SYORI_T FROM TORIMAST WHERE")
    '        SQL.Append("   　TORIS_CODE_T = " & SQ(key.INFOToriMast.TORIS_CODE_T))
    '        SQL.Append(" AND TORIF_CODE_T = " & SQ(key.INFOToriMast.TORIF_CODE_T))

    '        '見つからない場合
    '        If OraReader.DataReader(SQL) = False Then
    '            BatchLog.Write("個別前処理 取引先マスタ登録なし", "成功", "取引先コード：" & _
    '                       key.INFOToriMast.TORIS_CODE_T & "-" & key.INFOToriMast.TORIF_CODE_T)
    '            Return
    '        End If

    '        '登録していない場合
    '        If OraReader.GetItem("MAE_SYORI_T").Trim = "" Then
    '            BatchLog.Write("個別前処理 登録なし", "成功", "")
    '            Return
    '        End If

    '        '個別前処理番号取得
    '        MaeSyoriKey = OraReader.GetString("MAE_SYORI_T")
    '        OraReader.Close()

    '        TxtName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "134_MAE_SYORI_T.TXT")
    '        StrReader = New StreamReader(TxtName, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

    '        Dim Line As String = StrReader.ReadLine()

    '        Do Until Line = Nothing
    '            Dim pos As Integer = Line.IndexOf(","c)

    '            If pos >= 0 Then
    '                If CAInt32(Line.Substring(0, pos).Trim).ToString("00") = MaeSyoriKey Then
    '                    ' 一致する番号を見つける

    '                    If pos >= 0 AndAlso Line.Length >= pos Then
    '                        pos = Line.IndexOf(","c, pos + 1)

    '                        Dim ParaPos As Integer = Line.ToUpper.IndexOf(".EXE", pos + 1)
    '                        If ParaPos < 0 Then
    '                            BatchLog.Write("個別前処理 ＥＸＥ登録なし", "成功", "")
    '                            Exit Sub
    '                        End If

    '                        Dim Exe As String = Line.Substring(pos + 1, ParaPos - pos + 3).Trim
    '                        Exe = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "EXE"), Exe)

    '                        Dim Para As String = ""
    '                        If ParaPos >= 0 AndAlso Line.Length >= ParaPos Then
    '                            Para = Line.Substring(ParaPos + 4).Trim
    '                        End If

    '                        ' 個別前処理 起動
    '                        Dim ProcInfo As New ProcessStartInfo
    '                        ProcInfo.FileName = Exe

    '                        If File.Exists(ProcInfo.FileName) = True Then

    '                            Dim OutPath As String = CASTCommon.GetFSKJIni("OTHERSYS", "MAESYORI")
    '                            If OutPath <> "err" Then
    '                                If Directory.Exists(OutPath) = False Then
    '                                    Directory.CreateDirectory(OutPath)
    '                                End If
    '                            End If

    '                            If Directory.Exists(OutPath) = False Then
    '                                BatchLog.Write("個別前処理 連携フォルダなし", "失敗", OutPath)
    '                                Return
    '                            End If

    '                            Dim OutName As String = Path.GetFileName(Exe) & "_" & Para & "."
    '                            OutName = Path.Combine(OutPath, OutName)

    '                            For i As Integer = 1 To 999
    '                                Try
    '                                    ' ファイルが作成できるまで繰り返す
    '                                    If File.Exists(OutName & i.ToString("000")) = False Then
    '                                        File.Copy(fileName, OutName & i.ToString("000"), False)
    '                                        Exit For
    '                                    Else
    '                                        Threading.Thread.Sleep(100)
    '                                    End If
    '                                Catch ex As Exception

    '                                End Try
    '                            Next i

    '                            ProcInfo.WorkingDirectory = Path.GetDirectoryName(ProcInfo.FileName)
    '                            ProcInfo.Arguments = Para
    '                            Process.Start(ProcInfo)
    '                            BatchLog.Write("個別前処理 実行", "成功")
    '                        Else
    '                            BatchLog.Write("個別前処理 実行モジュールなし", "失敗", Exe)
    '                        End If

    '                        ' 個別処理を実行したでの抜ける
    '                        Return
    '                    End If

    '                    BatchLog.Write("個別前処理 実行モジュールの指定なし", "失敗")
    '                    Exit Do
    '                End If

    '                Line = StrReader.ReadLine()
    '            End If
    '        Loop

    '        BatchLog.Write("個別前処理 TXT一致データなし", "成功", MaeSyoriKey)

    '        StrReader.Close()
    '        StrReader = Nothing
    '    Catch ex As Exception
    '        BatchLog.Write("個別前処理", "失敗", ex.Message)
    '    Finally
    '        If Not StrReader Is Nothing Then
    '            StrReader.Close()
    '        End If
    '    End Try
    'End Sub
    '2013/12/24 saitou 標準版 DEL --------------------------------------------------<<<<

    '2012/06/30 標準版　WEB伝送連携
    ' 機能　 ： WEB履歴マスタ UPDATE
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： WEB履歴マスタ更新用
    '
    Private Function UpdateWEB_RIREKIMAST() As Boolean

        Try
            BatchLog.Write("0000000000-00", "00000000", "ログイン(開始)", "成功")

            Dim SQL As New StringBuilder(128)
            SQL.Append("UPDATE WEB_RIREKIMAST SET")
            SQL.Append(" STATUS_KBN_W = '1' ")
            SQL.Append(" WHERE FSYORI_KBN_W = '1'")
            SQL.Append(" AND TORIS_CODE_W = " & SQ(mKoParam.CP.TORIS_CODE))
            SQL.Append(" AND TORIF_CODE_W = " & SQ(mKoParam.CP.TORIF_CODE))
            SQL.Append(" AND FURI_DATE_W = " & SQ(mKoParam.CP.FURI_DATE))
            SQL.Append(" AND SEQ_NO_W = (SELECT MAX(SEQ_NO_W) FROM WEB_RIREKIMAST ")
            SQL.Append(" WHERE TORIS_CODE_W = " & SQ(mKoParam.CP.TORIS_CODE))
            SQL.Append(" AND   TORIF_CODE_W = " & SQ(mKoParam.CP.TORIF_CODE))
            SQL.Append(" AND   FURI_DATE_W = " & SQ(mKoParam.CP.FURI_DATE))
            SQL.Append(" AND   FSYORI_KBN_W = '1'")
            SQL.Append(" )")


            '2017/04/19 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ START
            'WEB履歴マスタにレコードがない場合にエラーとなるように修正
            If MainDB.ExecuteNonQuery(SQL) < 1 Then
                'If MainDB.ExecuteNonQuery(SQL) < 0 Then
                '2017/04/19 タスク）西野 CHG 標準版修正（潜在バグ修正）------------------------------------ END
                BatchLog.Write("WEB履歴マスタ更新", "失敗", MainDB.Message)
                Return False
            End If

        Catch ex As Exception
            BatchLog.Write("WEB履歴マスタ更新", "失敗", ex.Message)
            Return False
        Finally
        End Try

        Return True
    End Function

    ' 2016/01/11 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- START
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
            If mArgumentData.INFOToriMast.FSYORI_KBN_T = "1" Then
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

            For i As Integer = 0 To FileList.Length - 1 Step 1
                If Path.GetFileName(FileList(i)).IndexOf(CheckFileName) = 0 Then
                    FileName = Path.GetFileName(FileList(i))
                    Exit For
                End If
            Next

            If FileName = "" Then
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
            BatchLog.Write("", "0000000000-00", "00000000", "テキストファイル読込", "", ex)
            Return "ERR"
        Finally
            If Not sr Is Nothing Then
                sr.Close()
                sr = Nothing
            End If
            BatchLog.Write("", "0000000000-00", "00000000", "テキストファイル読込", "終了", "")
        End Try

    End Function
    ' 2016/01/11 タスク）綾部 ADD 【PG】UI_B-14-01(RSV2対応) -------------------- END

End Class
