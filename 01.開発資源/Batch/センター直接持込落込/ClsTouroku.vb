Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports System.Diagnostics
Imports CAstBatch
Imports CAstSystem
Imports CASTCommon
Imports CASTCommon.MyOracle
'*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
Imports Microsoft.VisualBasic
'*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

' 個別落し込み処理
Public Class ClsTouroku

    Private clsFusion As New clsFUSION.clsMain
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
                CP.TORI_CODE = ""                           '取引先コード
                CP.FURI_DATE = para(0)                      '振替日
                CP.CODE_KBN = "4"                           'コード区分
                CP.FMT_KBN = "TO"                           'フォーマット区分
                CP.BAITAI_CODE = para(1).PadLeft(2, "0"c)   '媒体コード
                '2010/01/27 add
                'CP.LABEL_KBN = ""                           'ラベル区分
                CP.LABEL_KBN = "2"                           'ラベル区分
                CP.RENKEI_FILENAME = ""                     '連携ファイル名
                CP.ENC_KBN = ""                             '暗号化処理区分
                CP.ENC_KEY1 = ""                            '暗号化キー１
                CP.ENC_KEY2 = ""                            '暗号化キー２
                CP.ENC_OPT1 = ""                            'ＡＥＳオプション
                CP.CYCLENO = ""                             'サイクル№
                CP.JOBTUUBAN = Integer.Parse(para(2))       'ジョブ通番
            End Set
        End Property
    End Structure
    Private mKoParam As TourokuParam

    ' ＣＯＭＰＬＯＣＫ暗号化情報
    Private Structure stCOMPLOCK
        Dim AES As String
        Dim KEY As String
        Dim IVKEY As String
        Dim RECLEN As String
    End Structure

    Dim Complock As stCOMPLOCK

    Dim mUserID As String = ""                      'ユーザＩＤ
    Dim mJobMessage As String = ""                  'ジョブメッセージ
    Private mDataFileName As String                 '依頼データファイル名
    Private mArgumentData As CommData               '起動パラメータ 共通情報
    Public MainDB As CASTCommon.MyOracle            'パブリックＤＢ
    Private mErrMessage As String = ""              'エラーメッセージ(処理結果確認表印刷用)
    Private ArrayTenbetu As New ArrayList           '店別集計表出力対象 格納リスト

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
            BatchLog.Write("0000000000-00", "00000000", "(パラメータ取得)開始", "成功")
            BatchLog.JobTuuban = mKoParam.CP.JOBTUUBAN
            BatchLog.ToriCode = mKoParam.CP.TORI_CODE
            BatchLog.FuriDate = mKoParam.CP.FURI_DATE
        Catch ex As Exception
            BatchLog.Write("(パラメータ取得)", "失敗", "パラメータ取得失敗[" & command & "]：" & ex.Message)

            Return 1
        End Try

        Try
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
                Try
                    '伝送・学校の場合は依頼ファイルを退避
                    ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- START
                    'Select Case InfoParam.BAITAI_CODE
                    '    Case "00", "07"
                    '        DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                    '        '前回ファイルを削除
                    '        If File.Exists(DestFile) = True Then
                    '            File.Delete(DestFile)
                    '        End If

                    '        File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)

                    '    Case Else

                    '        DestFile = CASTCommon.GetFSKJIni("COMMON", "DATBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                    '        '前回ファイルを削除
                    '        If File.Exists(DestFile) = True Then
                    '            File.Delete(DestFile)
                    '        End If

                    '        File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)

                    'End Select
                    Select Case InfoParam.BAITAI_CODE
                        Case "00", "07", _
                             "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                            DestFile = CASTCommon.GetFSKJIni("COMMON", "DENBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                            '前回ファイルを削除
                            If File.Exists(DestFile) = True Then
                                File.Delete(DestFile)
                            End If

                            File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)

                        Case Else

                            DestFile = CASTCommon.GetFSKJIni("COMMON", "DATBK") & Path.GetFileName(mKoParam.CP.RENKEI_FILENAME)

                            '前回ファイルを削除
                            If File.Exists(DestFile) = True Then
                                File.Delete(DestFile)
                            End If

                            File.Move(mKoParam.CP.RENKEI_FILENAME, DestFile)

                    End Select
                    ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- END

                Catch ex As Exception
                    BatchLog.Write("正常フォルダ移動", "失敗", mKoParam.CP.RENKEI_FILENAME & " -> " & DestFile)
                End Try
            End If

            MainDB.Close()

            'RV-A45 更新完了位置変更
            ' JOBマスタ更新
            If Not bRet = False Then
                If BatchLog.UpdateJOBMASTbyOK(mJobMessage) = False Then
                    bRet = False
                End If
            End If
            '========================
            '---------------------------------------------------------------------------------

            BatchLog.Write("終了", "成功", bRet.ToString)


            If bRet = False Then
                Return 2
            End If

            Return 0

        Catch ex As Exception
            BatchLog.Write("(落し込みメイン処理)", "失敗", ex.Message)

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

        '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
        Dim dblock As CASTCommon.CDBLock = Nothing

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
            BatchLog.Write("(登録メイン処理)開始", "成功")


            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行ロック
            dblock = New CASTCommon.CDBLock
            If dblock.Job_Lock(MainDB, LockWaitTime) = False Then
                BatchLog.Write_Err("(登録メイン処理)", "失敗", "センター直接持込落込処理で実行待ちタイムアウト")
                BatchLog.UpdateJOBMASTbyErr("センター直接持込落込処理で実行待ちタイムアウト")
                mErrMessage = "実行待ちタイムアウト"
                Return False
            End If
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            '--------------------------------------------
            '媒体読み込み（フォーマット　）
            '--------------------------------------------
            Dim oReadFMT As CAstFormat.CFormat
            Dim sReadFile As String = ""

            Try
                'フォーマット区分から，フォーマットを特定する
                'oReadFMT = oReadFMT.GetFormat(mArgumentData.INFOParameter)
                oReadFMT = CFormat.GetFormat(mArgumentData.INFOParameter)

                If oReadFMT Is Nothing Then
                    BatchLog.Write("(登録メイン処理)", "失敗", "フォーマット取得（規定外フォーマット）")

                    BatchLog.UpdateJOBMASTbyErr("フォーマット取得（規定外フォーマット）")
                    mErrMessage = "フォーマット取得失敗"
                    Return False
                End If

            Catch ex As Exception
                BatchLog.Write("(登録メイン処理)", "失敗", "フォーマット取得：" & ex.Message)
                BatchLog.UpdateJOBMASTbyErr("フォーマット取得")
                mErrMessage = "フォーマット取得失敗"
                Return False
            End Try

            'SJIS 改行なしファイルに変換して読み込む(媒体読込の実行)
            sReadFile = ReadMediaData(oReadFMT)
            If sReadFile = "" Then
                oReadFMT.Close()
                ' 2016/02/08 タスク）綾部 CHG 【IT】UI_B-14-99(RSV2対応(影響調査漏れ)) -------------------- START
                'BatchLog.UpdateJOBMASTbyErr("フォーマット取得失敗")
                Select Case mErrMessage
                    Case ""
                        BatchLog.UpdateJOBMASTbyErr("フォーマット取得失敗")
                    Case Else
                        BatchLog.UpdateJOBMASTbyErr(mErrMessage)
                End Select
                ' 2016/02/08 タスク）綾部 CHG 【IT】UI_B-14-99(RSV2対応(影響調査漏れ)) -------------------- END
                BatchLog.Write("(登録メイン処理)", "失敗", "媒体読み込み：" & oReadFMT.Message)
                mErrMessage = "フォーマット取得失敗"
                Return False
            End If

            '--------------------------------------------
            'データチェック
            '--------------------------------------------
            '*****20120704 mubuchi  "11"(DVD)追加***********>>>>
            ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- START
            'If mKoParam.CP.BAITAI_CODE = "00" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "01" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "05" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "11" OrElse _
            '   mKoParam.CP.BAITAI_CODE = "06" Then
            '    '*****20120704 mubuchi  "11"(DVD)追加***********<<<<
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
            '                                                        nLine.ToString & "行目の" & (nPos + 1).ToString & "バイト目に不正な文字が入っています")

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
            Select Case mKoParam.CP.BAITAI_CODE
                Case "00", "01", "05", "06", _
                     "11", "12", "13", "14", "15", _
                     "30", "31", "32", "33", "34", "35", "36", "37", "38", "39"
                    Dim nPos As Long = -1
                    Dim nLine As Long = 0
                    Dim nErrorCount As Long = 0
                    Dim FirstError As String = ""

                    oReadFMT.FirstRead(sReadFile)

                    Do Until oReadFMT.EOF
                        nLine += 1

                        '1レコード　データ取得
                        If oReadFMT.GetFileData() = 0 Then
                        End If

                        '規定文字チェック
                        nPos = oReadFMT.CheckRegularString()

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
            End Select
            ' 2016/11/21 タスク）綾部 CHG 【PG】UI_99-99(飯田信金 伝送系媒体追加(30～39) -------------------- END

            '--------------------------------------------
            ' 明細マスタ登録処理
            '--------------------------------------------
            If MakingMeiMast(oReadFMT) = False Then
                oReadFMT.Close()

                If File.Exists(sReadFile) = True Then
                    File.Delete(sReadFile)
                End If

                '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
                ' ジョブ実行アンロック
                dblock.Job_UnLock(MainDB)
                '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

                ' ロールバック
                MainDB.Rollback()
                Return False
            End If

            oReadFMT.Close()
            If File.Exists(sReadFile) = True Then
                File.Delete(sReadFile)
            End If

            'RV-A45 更新完了位置変更
            ' JOBマスタ更新
            'If BatchLog.UpdateJOBMASTbyOK(mJobMessage) = False Then
            '    ' ロールバック
            '    MainDB.Rollback()
            '    Return False
            'End If
            '========================

            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            ' ＤＢコミット
            MainDB.Commit()

            Return True

        Catch ex As Exception
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***

            'RV-A45 ロールバックを追加
            MainDB.Rollback()
            '=================
            BatchLog.Write("(登録メイン処理)", "失敗", ex.Message)
            Return False

        Finally
            '*** Str Add 2015/12/01 SO)荒木 for 多重実行対応 ***
            ' ジョブ実行アンロック
            dblock.Job_UnLock(MainDB)

            ' ロールバック
            MainDB.Rollback()
            '*** End Add 2015/12/01 SO)荒木 for 多重実行対応 ***
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
            If mKoParam.CP.BAITAI_CODE <> mArgumentData.INFOToriMast.BAITAI_CODE_T Then
                BatchLog.Write("(パラメータチェック)", "失敗", "媒体コード異常")
                Call BatchLog.UpdateJOBMASTbyErr("取引先マスタ登録異常：媒体コード")
                mErrMessage = "媒体コード異常"
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

            Try
                oReader = New CASTCommon.MyOracleReader(MainDB)

                If oReader.DataReader(SQL) = False Then
                    BatchLog.Write("スケジュールなし", "失敗", "振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mKoParam.CP.TORIS_CODE & "-" & mKoParam.CP.TORIF_CODE)
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
                    BatchLog.Write("スケジュール", "失敗", "落し込み処理済 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"), "中断", "取引先コード：" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    Call BatchLog.UpdateJOBMASTbyErr("スケジュール:落し込み処理済 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日") & " 取引先コード：" & mArgumentData.INFOToriMast.TORIS_CODE_T & "-" & mArgumentData.INFOToriMast.TORIF_CODE_T)
                    mErrMessage = "落し込み処理済"
                    Return False
                End If

                Return True

            Catch ex As Exception
                BatchLog.Write("スケジュール検索", "失敗", " 振替日：" & CASTCommon.ConvertDate(mKoParam.CP.FURI_DATE, "yyyy年MM月dd日"))
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

        Try
            BatchLog.Write("(依頼データ読込)開始", "成功")

            '依頼データ読込／フォーマットチェック実行
            Dim oRenkei As ClsRenkei = New ClsRenkei(mArgumentData) '連携

            '------------------------------------------------------------
            '媒体読み
            '------------------------------------------------------------
            With mKoParam.CP

                '中間ファイル作成先指定
                oRenkei.OutFileName = CASTCommon.GetFSKJIni("COMMON", "DATBK") & "iKINKO_WATASHI.DAT"

                'F*TRANファイル名を取得
                If .CODE_KBN = "4" And .BAITAI_CODE = "01" Then
                    FTranName = readfmt.FTRANIBMP
                    'FTranName = readfmt.FTRANIBMBINARYP
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
                        BatchLog.Write("(依頼データ読込)", "失敗", "媒体コード不正")

                        BatchLog.UpdateJOBMASTbyErr("媒体コード不正")
                        Return ""

                    Case "05"       'ＭＴ

                        '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                        'ＭＴファイル読込処理実行
                        strKekka = vbDLL.cmtCPYtoDISK(0, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)

                        'Dim intblk_size As Short
                        'Dim shtREC_LENGTH As Short = CShort(readfmt.RecordLen)
                        'Dim shtLABEL_KBN As Short = CShort(.LABEL_KBN)

                        ''ＭＴファイル読込処理実行
                        'strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                        '媒体読込失敗：処理終了
                        If strKekka <> "" Then
                            BatchLog.Write("(依頼データ読込)", "失敗", "ＭＴ読込：" & oRenkei.Message)
                            BatchLog.UpdateJOBMASTbyErr("ＭＴ読込失敗、ファイル名：" & oRenkei.InFileName)
                            Return ""
                        End If

                    Case "06"       'ＣＭＴ

                        '2018/02/19 saitou 広島信金(RSV2標準) UPD サーバー処理対応(64ビット対応) -------------------- START
                        'ＣＭＴファイルコピー先設定
                        strKekka = vbDLL.cmtCPYtoDISK(0, readfmt.RecordLen, CInt(.LABEL_KBN), "SLMT", 1, 0, " ", oRenkei.InFileName, 0, 0)

                        'Dim intblk_size As Short
                        'Dim shtREC_LENGTH As Short = CShort(readfmt.RecordLen)
                        'Dim shtLABEL_KBN As Short = CShort(.LABEL_KBN)

                        ''ＣＭＴファイルコピー先設定
                        ''strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 4, " ", oRenkei.InFileName, 0, 0)
                        'strKekka = vbDLL.cmtCPYtoDISK(intblk_size, shtREC_LENGTH, shtLABEL_KBN, "SLMT", 1, 0, " ", oRenkei.InFileName, 0, 0)
                        '2018/02/19 saitou 広島信金(RSV2標準) UPD --------------------------------------------------- END

                        '媒体読込失敗：処理終了
                        If strKekka <> "" Then
                            BatchLog.Write("(依頼データ読込)", "失敗", "ＣＭＴ読込：" & oRenkei.Message)
                            BatchLog.UpdateJOBMASTbyErr("ＣＭＴ読込失敗、ファイル名：" & oRenkei.InFileName)
                            Return ""
                        End If
                        '2010/04/26 三島信金対応 バイナリで読み込んで変換する
                        Dim REC_LENGTH As Integer
                        REC_LENGTH = readfmt.RecordLen
                        nRetRead = clsFusion.fn_DEN_CPYTO_DISK(.TORI_CODE, oRenkei.InFileName, oRenkei.OutFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)
                        '======================================================
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
                        mKoParam.CP.RENKEI_FILENAME = CASTCommon.GetFSKJIni("COMMON", "DEN") & "KINKO_WATASHI.DAT"

                        '2017/01/23 saitou 東春信金(RSV2標準) ADD 大容量デリバリ対応 ---------------------------------------- START
                        Dim CENTER_DELIVERY As String = String.Empty
                        CENTER_DELIVERY = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "CENTER_DELIVERY")
                        If CENTER_DELIVERY = "1" Then
                            '大容量デリバリ
                            If Not Me.fn_CopyBatchSVToDen(mKoParam.CP.RENKEI_FILENAME) Then
                                Return ""
                            End If
                        Else
                            '伝送
                            If CENTER_DELIVERY = "err" OrElse CENTER_DELIVERY = Nothing Then
                                '設定値ミス（通常の伝送として処理）
                                BatchLog.Write("(依頼データ読込)", "失敗", "設定ファイル取得失敗 [RSV2_V1.0.0]CENTER_DELIVERY 設定なしのため伝送として処理")
                            End If
                        End If
                        '2017/01/23 saitou 東春信金(RSV2標準) ADD ----------------------------------------------------------- END

                        If GetFSKJIni("CENTER", "COMPLOCK") = "1" Then

                            'CompLock
                            Dim strOfileName As String = CASTCommon.GetFSKJIni("COMMON", "DEN") & "AKINKO_WATASHI.DAT"

                            If GetKeyInfo() = False Then
                                BatchLog.Write("復号化情報取得", "失敗", mErrMessage)
                                mErrMessage = "復号化情報取得失敗"
                                Return ""
                            End If

                            Dim cmpErr As Long
                            cmpErr = EncodeComplock(Complock, mKoParam.CP.RENKEI_FILENAME, strOfileName)
                            If cmpErr <> 0 Then   '戻り値暫定
                                BatchLog.Write("復号化処理", "失敗", "エラーコード:" & cmpErr & " " & mErrMessage)
                                mErrMessage = "復号化処理失敗"
                                Return ""
                            End If

                            mKoParam.CP.RENKEI_FILENAME = strOfileName

                        End If

                        oRenkei.InFileName = mKoParam.CP.RENKEI_FILENAME

                        ' 2016/02/01 タスク）綾部 CHG 【IT】UI_B-14-99(RSV2対応(影響調査漏れ)) -------------------- START
                        Select Case CASTCommon.GetRSKJIni("RSV2_V1.0.0", "EDITION")
                            Case "2"
                                If File.Exists(oRenkei.InFileName) = False Then
                                    BatchLog.Write("(依頼データ読込)", "失敗", "入力ファイルなし : " & oRenkei.InFileName)
                                    mErrMessage = "入力ファイルなし : " & oRenkei.InFileName
                                    Return ""
                                End If
                            Case Else
                                ' NOP
                        End Select
                        ' 2016/02/01 タスク）綾部 CHG 【IT】UI_B-14-99(RSV2対応(影響調査漏れ)) -------------------- END

                        'nRetRead = oRenkei.CopyToDisk(readfmt)
                        nRetRead = clsFusion.fn_DEN_CPYTO_DISK(.TORI_CODE, mKoParam.CP.RENKEI_FILENAME, oRenkei.OutFileName, REC_LENGTH, .CODE_KBN, FTranName, msgTitle)

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
                    Return ""
                Case 50
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル検索：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイルなし、ファイル名：" & oRenkei.InFileName)
                    Return ""
                Case 100
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル取込(コード変換)：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル取込失敗（コード変換）")
                    Return ""
                Case 200
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル取込(コード区分異常(JIS改行あり))：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル取込失敗（コード区分異常（JIS改行あり））")
                    Return ""
                Case 300
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル取込(コード区分異常(JIS改行なし))：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル取込失敗(コード区分異常(JIS改行なし))")
                    Return ""
                Case 400
                    BatchLog.Write("(依頼データ読込)", "失敗", "ファイル取込(出力ファイル作成)：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル取込失敗（出力ファイル作成）")
                    Return ""
                Case Else
                    BatchLog.Write("(依頼データ読込)", "失敗", "不明な戻り値：" & oRenkei.Message)
                    BatchLog.UpdateJOBMASTbyErr("ファイル取込失敗（ログ参照）")
                    Return ""
            End Select

        Catch ex As Exception
            BatchLog.UpdateJOBMASTbyErr("依頼データ読込処理：システムエラー（ログ参照）")
            BatchLog.Write("(依頼データ読込)", "失敗", ex.Message)

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
        Dim PrnSchErrList As ClsPrnSchError = Nothing          'スケジュールエラーリストクラス
        Dim PrnToriErrList As ClsPrnToriError = Nothing          'インプットエラーリストクラス
        'Dim PrnMeisai As ClsPrnUketukeMeisai = Nothing          '受付明細表
        'Dim ArrayPrnMeisai As New ArrayList(128)
        Dim ArrayPrnInErrList As New ArrayList(128)
        'Dim DaihyouItakuCode As String = ""                     '代表委託者コード

        Dim SyoriFlg As Boolean = True      'エラーリストのために処理続行フラグ

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
            'Dim SitenYomikae As String                  '支店読替印刷区分(1:印刷対象 / その他:印刷非対象)
            'SitenYomikae = CASTCommon.GetFSKJIni("YOMIKAE", "TENPO")

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

                    ' データを読み込んで，フォーマットチェックを行う
                    sCheckRet = aReadFmt.CheckDataFormat()

                    Select Case sCheckRet

                        Case "ERR"
                            'フォーマットエラー
                            Dim nPos As Long
                            nPos = aReadFmt.CheckRegularString

                            If nPos > 0 Then
                                BatchLog.Write("フォーマットエラー", "失敗", nRecordCount.ToString & "行目，" & (nPos + 1).ToString & "バイト目 " & aReadFmt.Message)
                                BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目，" & (nPos + 1).ToString & "バイト目 " & aReadFmt.Message)
                            Else
                                BatchLog.Write("フォーマットエラー", "失敗", nRecordCount.ToString & "行目 " & aReadFmt.Message)
                                BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目 " & aReadFmt.Message)
                            End If

                            mErrMessage = aReadFmt.Message

                            bRet = False
                            Exit Do

                        Case "IJO"
                            'インプットエラー

                            If PrnInErrList Is Nothing Then
                                '最初のファイル作成
                                PrnInErrList = New ClsPrnInputError        ' インプットエラーリストクラス
                                PrnInErrList.CreateCsvFile()
                            End If

                            Call PrnInErrList.OutputData(aReadFmt, aReadFmt.ToriData)

                        Case "NS"   'スケジュールなし
                            BatchLog.Write("スケジュールエラー", "失敗", nRecordCount.ToString & "行目 " & aReadFmt.Message)
                            BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目 " & aReadFmt.Message)

                            If PrnSchErrList Is Nothing Then
                                '最初のファイル作成
                                PrnSchErrList = New ClsPrnSchError
                                PrnSchErrList.CreateCsvFile()
                            End If

                            Call PrnSchErrList.OutputData(aReadFmt.ToriData, aReadFmt, 1)

                            SyoriFlg = False

                        Case "TS"   'スケジュール中断
                            BatchLog.Write("スケジュールエラー", "失敗", nRecordCount.ToString & "行目 " & aReadFmt.Message)
                            BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目 " & aReadFmt.Message)

                            If PrnSchErrList Is Nothing Then
                                '最初のファイル作成
                                PrnSchErrList = New ClsPrnSchError
                                PrnSchErrList.CreateCsvFile()
                            End If

                            Call PrnSchErrList.OutputData(aReadFmt.ToriData, aReadFmt, 2)

                            SyoriFlg = False

                        Case "SS"   'スケジュール落し込み済
                            BatchLog.Write("スケジュールエラー", "失敗", nRecordCount.ToString & "行目 " & aReadFmt.Message)
                            BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目 " & aReadFmt.Message)

                            If PrnSchErrList Is Nothing Then
                                '最初のファイル作成
                                PrnSchErrList = New ClsPrnSchError
                                PrnSchErrList.CreateCsvFile()
                            End If

                            Call PrnSchErrList.OutputData(aReadFmt.ToriData, aReadFmt, 3)

                            SyoriFlg = False

                        Case "NT"   '取引先情報なし
                            BatchLog.Write("取引先エラー", "失敗", nRecordCount.ToString & "行目 " & aReadFmt.Message)
                            BatchLog.UpdateJOBMASTbyErr(nRecordCount.ToString & "行目 " & aReadFmt.Message)

                            If PrnToriErrList Is Nothing Then
                                '最初のファイル作成
                                PrnToriErrList = New ClsPrnToriError
                                PrnToriErrList.CreateCsvFile()
                            End If

                            Call PrnToriErrList.OutputData(aReadFmt)

                            SyoriFlg = False

                        Case "H"
                            EndFlag = False

                        Case "D"

                        Case "T"

                        Case "E"
                            EndFlag = True

                        Case ""
                            Exit Do

                    End Select

                    If SyoriFlg = True Then

                        'ヘッダレコード
                        If aReadFmt.IsHeaderRecord() = True Then

                            'カウンタ初期化
                            nRecordNumber = 1
                            aReadFmt.InfoMeisaiMast.FILE_SEQ += 1
                            PrnFlag = True

                            ' 二重
                            ArrayMeiData.Clear()
                        End If


                        '明細マスタ項目設定 レコード番号
                        '複数エンドのレコードは明細マスタにINSERTしない
                        If sCheckRet <> "99" Then
                            aReadFmt.InfoMeisaiMast.RECORD_NO = nRecordNumber
                        End If

                        '複数エンドのレコードは明細マスタにINSERTしない
                        '明細マスタ登録
                        If sCheckRet <> "99" Then
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

                            If PrnFlag = True Then

                                ' 明細マスタを検索し，２重持込の検査を行う
                                Dim bDupicate As Boolean = False
                                'bDupicate = aReadFmt.CheckDuplicate(ArrayMeiData) '総振用
                                If bDupicate = True Then
                                    mJobMessage = "二重持ち込み"
                                End If

                                If bDupicate = False Then
                                    ' ２重登録時は，インプットエラーを出力しない
                                    ' インプットエラーＣＳＶを出力する
                                    If Not PrnInErrList Is Nothing Then
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

                                '店別集計表印刷用リストに格納(印刷区分,取引先主コード,取引先副コード)
                                ArrayTenbetu.Add(mArgumentData.INFOToriMast.FUNOU_MEISAI_KBN_T & "," & _
                                                 aReadFmt.ToriData.INFOToriMast.TORIS_CODE_T & "," & _
                                                 aReadFmt.ToriData.INFOToriMast.TORIF_CODE_T)

                                ' スケジュールマスタを更新する
                                bRet = aReadFmt.UpdateSCHMAST
                                If bRet = False Then
                                    BatchLog.Write("スケジュール更新", "失敗")
                                    BatchLog.UpdateJOBMASTbyErr("スケジュール更新失敗")
                                    Exit Do
                                End If
                            End If

                            PrnFlag = False
                        End If

                    End If

                Loop


                If Not PrnSchErrList Is Nothing Then
                    If PrnSchErrList.ReportExecute() = False Then
                        BatchLog.Write("スケジュールエラーリスト", "失敗", "ファイル名:" & PrnSchErrList.FileName & " メッセージ:" & PrnSchErrList.ReportMessage)
                    End If
                    mErrMessage = "スケジュールエラー委託者あり"
                End If

                If Not PrnToriErrList Is Nothing Then
                    If PrnToriErrList.ReportExecute() = False Then
                        BatchLog.Write("取引先マスタエラーリスト", "失敗", "ファイル名:" & PrnToriErrList.FileName & " メッセージ:" & PrnToriErrList.ReportMessage)
                    End If
                    mErrMessage = "取引先エラー委託者あり"
                End If

                If SyoriFlg = True Then

                    If bRet = False Then

                        If aReadFmt.IsTrailerRecord() = True Then
                            ' インプットエラーリストを印刷する
                            If Not PrnInErrList Is Nothing Then
                                PrnInErrList.CloseCsv()

                                ' 正常処理のあとに印刷するため，リストに保存
                                ArrayPrnInErrList.Add(PrnInErrList)
                                PrnInErrList = Nothing
                            End If

                            For i As Integer = 0 To ArrayPrnInErrList.Count - 1
                                PrnInErrList = CType(ArrayPrnInErrList(i), ClsPrnInputError)
                                BatchLog.Write("インプットエラーリスト出力", "対象", PrnInErrList.FileName)

                                If PrnInErrList.ReportExecute() = False Then
                                    BatchLog.Write("インプットエラーリスト出力", "失敗", "ファイル名:" & PrnInErrList.FileName & " メッセージ:" & PrnInErrList.ReportMessage)
                                End If
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

                    ' インプットエラーリストを印刷する
                    For i As Integer = 0 To ArrayPrnInErrList.Count - 1

                        PrnInErrList = CType(ArrayPrnInErrList(i), ClsPrnInputError)
                        BatchLog.Write("インプットエラーリスト出力", "対象", PrnInErrList.FileName)

                        If PrnInErrList.ReportExecute() = False Then
                            BatchLog.Write("インプットエラーリスト出力", "失敗", "ファイル名:" & PrnInErrList.FileName & " メッセージ:" & PrnInErrList.ReportMessage)
                        End If

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
                    If Not PrnSyoKekka Is Nothing Then
                        If PrnSyoKekka.ReportExecute() = False Then
                            BatchLog.Write("処理結果確認表出力", "失敗", "ファイル名:" & PrnSyoKekka.FileName & " メッセージ:" & PrnSyoKekka.ReportMessage)
                        End If
                    End If

                Else

                    'スケジュールエラーリストもしくは取引先エラーリストがでたのだからエラー
                    Return False

                End If

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
            SQL.Append(",' '")                                      'YOBI4_K
            SQL.Append(",' '")                                      'YOBI5_K
            SQL.Append(",' '")                                      'YOBI6_K
            SQL.Append(",' '")                                      'YOBI7_K
            SQL.Append(",' '")                                      'YOBI8_K
            SQL.Append(",' '")                                      'YOBI9_K
            SQL.Append(",' '")                                      'YOBI10_K

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
                '2010.06.08 oni センター直接持ち込みは変換後(NULL対策)をセット start
                .Item("FURI_DATA") = aReadFmt.RecordData
                '.Item("FURI_DATA") = aReadFmt.RecordDataNoChange
                '2010.06.08 oni センター直接持ち込みは変換後(NULL対策)をセット end
                .Item("RECORD_NO") = stMei.RECORD_NO.ToString
                .Item("SAKUSEI_DATE") = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
                .Item("YOBI1") = stMei.YOBI1
                .Item("YOBI2") = stMei.YOBI2
                .Item("YOBI3") = stMei.YOBI3

                If MainDB.ExecuteNonQuery() <= 0 Then
                    BatchLog.Write("明細マスタ登録", "失敗", MainDB.Message)
                    Return False
                End If

                '年金の場合は，NENKINMAST へもINSERT する
                'If aReadFmt.ToriData.INFOParameter.FMT_KBN = "03" Then
                '    If InsertNENKINMAST(aReadFmt) = False Then
                '        Return False
                '    End If
                'End If

            End With

        Catch ex As Exception
            BatchLog.Write("明細マスタ登録", "失敗", ex.Message)
            Return False
        End Try

        Return True
    End Function

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
        End Try

        Return True
    End Function


    ' 機能　 ： センター直接持ち込み　取引先取得
    '
    ' 戻り値 ： 0 - 正常 ， 0 以外 - 異常
    '
    ' 備考　 ： 
    '
    Private Sub GetCenterTORIMAST(ByVal furiCode As String, ByVal kigyoCode As String)
        Dim SQL As New StringBuilder(128)
        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        SQL.Append("SELECT ")
        SQL.Append(" TORIS_CODE_T")
        SQL.Append(",TORIF_CODE_T")
        SQL.Append(" FROM TORIMAST")
        SQL.Append(" WHERE FURI_CODE_T = " & SQ(furiCode))
        SQL.Append("   AND KIGYO_CODE_T = " & SQ(kigyoCode))
        SQL.Append("   AND MOTIKOMI_KBN_T = '1'")
        SQL.Append("   AND " & CASTCommon.Calendar.Now.ToString("yyyyMMdd") & " BETWEEN KAISI_DATE_T AND SYURYOU_DATE_T")

        Try
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            If OraReader.DataReader(SQL) = True Then
                mKoParam.CP.TORIS_CODE = OraReader.GetString("TORIS_CODE_T")
                mKoParam.CP.TORIF_CODE = OraReader.GetString("TORIF_CODE_T")
                mKoParam.CP.TORI_CODE = mKoParam.CP.TORIS_CODE & mKoParam.CP.TORIF_CODE
            End If
        Catch ex As Exception

        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            OraReader = Nothing
        End Try
    End Sub

    ' 機能　 ： 個別前処理
    '
    ' 戻り値 ： TRUE - 正常 ， FALSE - 異常
    '
    ' 備考　 ： 
    '
    Private Sub KobetuExecute(ByVal fileName As String, ByVal key As CommData)
        Dim StrReader As StreamReader = Nothing

        '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

        Try
            Dim MaeSyoriKey As String
            '*** Str Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            'Dim OraReader As New CASTCommon.MyOracleReader(MainDB)
            OraReader = New CASTCommon.MyOracleReader(MainDB)
            '*** End Upd 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            Dim SQL As New StringBuilder(128)
            Dim TxtName As String

            SQL.Append("SELECT MAE_SYORI_T FROM TORIMAST WHERE")
            SQL.Append("   　TORIS_CODE_T = " & SQ(key.INFOToriMast.TORIS_CODE_T))
            SQL.Append(" AND TORIF_CODE_T = " & SQ(key.INFOToriMast.TORIF_CODE_T))

            '見つからない場合
            If OraReader.DataReader(SQL) = False Then
                BatchLog.Write("個別前処理 取引先マスタ登録なし", "成功", "取引先コード：" & _
                           key.INFOToriMast.TORIS_CODE_T & "-" & key.INFOToriMast.TORIF_CODE_T)
                
                Return
            End If

            '登録していない場合
            If OraReader.GetItem("MAE_SYORI_T").Trim = "" Then
                BatchLog.Write("個別前処理 登録なし", "成功", "")
                Return
            End If

            '個別前処理番号取得
            MaeSyoriKey = OraReader.GetString("MAE_SYORI_T")
            OraReader.Close()

            TxtName = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "TXT"), "134_MAE_SYORI_T.TXT")
            StrReader = New StreamReader(TxtName, System.Text.Encoding.GetEncoding("SHIFT-JIS"))

            Dim Line As String = StrReader.ReadLine()

            Do Until Line = Nothing
                Dim pos As Integer = Line.IndexOf(","c)

                If pos >= 0 Then
                    If CAInt32(Line.Substring(0, pos).Trim).ToString("00") = MaeSyoriKey Then
                        ' 一致する番号を見つける

                        If pos >= 0 AndAlso Line.Length >= pos Then
                            pos = Line.IndexOf(","c, pos + 1)

                            Dim ParaPos As Integer = Line.ToUpper.IndexOf(".EXE", pos + 1)
                            If ParaPos < 0 Then
                                BatchLog.Write("個別前処理 ＥＸＥ登録なし", "成功", "")
                                Exit Sub
                            End If

                            Dim Exe As String = Line.Substring(pos + 1, ParaPos - pos + 3).Trim
                            Exe = Path.Combine(CASTCommon.GetFSKJIni("COMMON", "EXE"), Exe)

                            Dim Para As String = ""
                            If ParaPos >= 0 AndAlso Line.Length >= ParaPos Then
                                Para = Line.Substring(ParaPos + 4).Trim
                            End If

                            ' 個別前処理 起動
                            Dim ProcInfo As New ProcessStartInfo
                            ProcInfo.FileName = Exe

                            If File.Exists(ProcInfo.FileName) = True Then

                                Dim OutPath As String = CASTCommon.GetFSKJIni("OTHERSYS", "MAESYORI")
                                If OutPath <> "err" Then
                                    If Directory.Exists(OutPath) = False Then
                                        Directory.CreateDirectory(OutPath)
                                    End If
                                End If

                                If Directory.Exists(OutPath) = False Then
                                    BatchLog.Write("個別前処理 連携フォルダなし", "失敗", OutPath)
                                    Return
                                End If

                                Dim OutName As String = Path.GetFileName(Exe) & "_" & Para & "."
                                OutName = Path.Combine(OutPath, OutName)

                                For i As Integer = 1 To 999
                                    Try
                                        ' ファイルが作成できるまで繰り返す
                                        If File.Exists(OutName & i.ToString("000")) = False Then
                                            File.Copy(fileName, OutName & i.ToString("000"), False)
                                            Exit For
                                        Else
                                            Threading.Thread.Sleep(100)
                                        End If
                                    Catch ex As Exception

                                    End Try
                                Next i

                                ProcInfo.WorkingDirectory = Path.GetDirectoryName(ProcInfo.FileName)
                                ProcInfo.Arguments = Para
                                Process.Start(ProcInfo)
                                BatchLog.Write("個別前処理 実行", "成功")
                            Else
                                BatchLog.Write("個別前処理 実行モジュールなし", "失敗", Exe)

                            End If

                            ' 個別処理を実行したでの抜ける
                            Return
                        End If
                        BatchLog.Write("個別前処理 実行モジュールの指定なし", "失敗")

                        Exit Do
                    End If

                    Line = StrReader.ReadLine()
                End If
            Loop
            BatchLog.Write("個別前処理 TXT一致データなし", "成功", MaeSyoriKey)


            StrReader.Close()
            StrReader = Nothing
        Catch ex As Exception
            BatchLog.Write("個別前処理", "失敗", ex.Message)
        Finally
            '*** Str Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***
            If Not OraReader Is Nothing Then
                OraReader.Close()
            End If
            '*** End Add 2015/12/01 SO)荒木 for MyOracleReaderクローズ忘れを修正（潜在） ***

            If Not StrReader Is Nothing Then
                StrReader.Close()
            End If
        End Try
    End Sub

    ' 機能　 ： ＣＯＭＰＬＯＣＫを使用してファイルを暗号化
    '
    ' 備考　 ： 
    '
    Private Function EncodeComplock(ByVal complock As stCOMPLOCK, ByVal infile As String, ByVal outfile As String) As Long
        Dim Arg As String
        Dim intIDPROCESS As Integer = Nothing

        Dim sComplockPath As String = CASTCommon.GetFSKJIni("COMMON", "COMPLOCK")
        If File.Exists(sComplockPath) = True Then
            mErrMessage = "COMPLOCKプログラムが見つかりません"
            Return -100
        End If

        ' 引数組み立て
        Dim DQ As String = """"
        Arg = "-I "
        Arg &= DQ & infile & DQ
        'Arg &= " -l " & DQ & complock.RECLEN & DQ
        Arg &= " -O " & DQ & outfile & DQ ' 出力先
        'If complock.AES = "0" Then
        '    ' ＡＥＳなし
        '    Arg &= " -a 5"
        '    Arg &= " -n 256"
        'Else
        '    ' ＡＥＳ
        '    Arg &= " -a  8"             ' -rl を指定しない場合は, -a 6 となる
        '    Arg &= " -m  1 "
        '    Arg &= " -ak 1 "
        '    Arg &= " -p  1"
        'End If
        Arg &= " -k " & DQ & complock.KEY & DQ                      ' 鍵
        Arg &= " -v " & DQ & complock.IVKEY & DQ                    ' IV
        Arg &= " -rl " & DQ & complock.RECLEN & DQ
        Arg &= " -lf 0"
        'Arg &= " -t 0"
        'Arg &= " -g 1"

        ' 複合化実行
        Dim ProcFT As New Process
        Try
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(sComplockPath, "decode.exe")
            ProcInfo.Arguments = Arg
            ProcInfo.WorkingDirectory = sComplockPath
            ProcInfo.UseShellExecute = False
            ProcInfo.RedirectStandardOutput = True
            ProcFT = Process.Start(ProcInfo)
            ProcFT.WaitForExit()
            'If ProcFT.ExitCode <> 0 Then
            '    Return ProcFT.ExitCode
            'End If
        Catch ex As Exception
            mErrMessage = ex.Message
            Return -1
        End Try

        Return ProcFT.ExitCode

    End Function

    Private Function GetKeyInfo() As Boolean

        Complock.AES = GetFSKJIni("CENTER", "AES")
        Complock.KEY = GetFSKJIni("CENTER", "KEY")
        Complock.IVKEY = GetFSKJIni("CENTER", "IVKEY")
        Complock.RECLEN = GetFSKJIni("CENTER", "RECLEN")

    End Function

    ''' <summary>
    ''' 金庫渡しファイルをバッチSVからDENフォルダにコピーする
    ''' </summary>
    ''' <param name="CMTFileName"></param>
    ''' <returns>True or False</returns>
    ''' <remarks>2017/01/23 saitou 東春信金(RSV2標準) added for 大容量デリバリ対応</remarks>
    Private Function fn_CopyBatchSVToDen(ByVal CMTFileName As String) As Boolean

        Dim ret As Boolean = False

        Try
            'コピー元ファイル名
            Dim SrcFileName As String = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "CENTER_FILE")
            If SrcFileName.ToUpper = "ERR" Then
                BatchLog.UpdateJOBMASTbyErr("金庫渡しファイル取得 失敗 ")
                BatchLog.Write("金庫渡しフォルダ取得", "失敗", "[RSV2_V1.0.0].CENTER_FILE")
                mErrMessage = "金庫渡しファイル取得 失敗 [RSV2_V1.0.0].CENTER_FILE"
                Exit Try
            End If

            'ファイル名に金庫コード、登録日が入る場合の考慮
            SrcFileName = Format(SrcFileName.Replace("{0}", CASTCommon.GetFSKJIni("COMMON", "KINKOCD")))
            SrcFileName = Format(SrcFileName.Replace("{1}", mKoParam.CP.FURI_DATE))

            If Not File.Exists(SrcFileName) Then
                BatchLog.UpdateJOBMASTbyErr("金庫渡しファイル取得 失敗 ファイル無し：" & SrcFileName)
                BatchLog.Write("金庫渡しファイル取得", "失敗", "ファイル無し：" & SrcFileName)
                mErrMessage = "金庫渡しファイル取得 失敗 ファイル無し：" & SrcFileName
                Exit Try
            End If

            Dim FpExePath As String = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
            If FpExePath.ToUpper = "ERR" Then
                BatchLog.UpdateJOBMASTbyErr("金庫渡しファイル取得 失敗 ")
                BatchLog.Write("金庫渡しファイル取得", "失敗", "[COMMON].FTRANP")
                mErrMessage = "金庫渡しファイル取得 失敗 [COMMON].FTRANP"
                Exit Try
            End If

            'コピー先ファイル名
            Dim DestFileName As String = CMTFileName

            'バッチサーバからDENフォルダにコピー
            Dim Proc As Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(FpExePath, "FP")
            'パラメータ
            ProcInfo.Arguments = " /nwd/ putrand " & SrcFileName & " " & DestFileName
            ProcInfo.Arguments &= " /size 644 /isize 640 /map @4 binary 640:640 "

            ProcInfo.WorkingDirectory = FpExePath

            ProcInfo.UseShellExecute = False
            ProcInfo.RedirectStandardOutput = True
            Proc = Process.Start(ProcInfo)
            Proc.WaitForExit()

            If Proc.ExitCode = 0 Then
                ret = True
            End If

        Catch ex As Exception
            BatchLog.UpdateJOBMASTbyErr("金庫渡しファイル取得 失敗")
            BatchLog.Write("金庫渡しファイル取得", "失敗", ex.Message)
            mErrMessage = "金庫渡しファイル取得 失敗 "
            Exit Try
        End Try

        Return ret

    End Function

End Class
