Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports CASTCommon

''' <summary>
''' ＳＳＳ他行データ作成クラス
''' </summary>
''' <remarks>2017/01/16 saitou 東春信金(RSV2) added for スリーエス対応</remarks>
Public Class ClsSSS

#Region "クラス変数"

    'ログ
    Private LOG As New CASTCommon.BatchLOG("KFJ020", "他行データ作成(SSS)")

    '設定ファイル
    Private Structure strcIniInfo
        Dim DATBK As String
        Dim DEN As String
        Dim FTR As String
        Dim FTRANP As String
        Dim KINKOCD As String
        Dim SSS_SOUFU As String
        Dim SSS_SOUFU_TEL As String
        Dim SSS_SOUFU_FAX As String
        '2017/01/19 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
        Dim FUNOU_SSS_1 As String
        Dim FUNOU_SSS_2 As String
        '2017/01/19 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END
        '2017/02/27 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
        Dim SSS_ITAKUCODE_PATN As String
        '2017/02/27 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END
    End Structure
    Private IniInfo As strcIniInfo

    '処理日時
    Private ReadOnly strDate As String = CASTCommon.Calendar.Now.ToString("yyyyMMdd")
    Private ReadOnly strTime As String = CASTCommon.Calendar.Now.ToString("HHmmss")

    Private MainDB As CASTCommon.MyOracle

    Private strOUT_FILE As String
    Private strSOUSIN_FILE As String
    Private strJIFURI_SEQ As String

    '振替日
    Public Property FURI_DATE As String
        Get
            Return strFURI_DATE
        End Get
        Set(value As String)
            strFURI_DATE = value
        End Set
    End Property
    Private strFURI_DATE As String

    Private bDataExistsFlg As Boolean = False

    '2017/01/19 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
    Private FmtComm As New CAstFormat.CFormat
    '2017/01/19 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

#End Region

#Region "パブリックメソッド"

    ''' <summary>
    ''' ＳＳＳ他行データ作成メイン処理
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Public Function fn_CREATE_DATA_MAIN() As Boolean
        LOG.ToriCode = m_TAKOU.strTORI_CODE
        LOG.FuriDate = m_TAKOU.strFURI_DATE
        LOG.JobTuuban = CASTCommon.CAInt32(m_TAKOU.strTUUBAN)
        LOG.Write(LOG.ToriCode, LOG.FuriDate, "(SSSデータ作成メイン処理)開始", "成功")

        '------------------------------------------------
        '設定ファイル読み込み
        '------------------------------------------------
        If fn_INI_READ() = False Then
            Return False
        End If

        '------------------------------------------------
        'ＤＢ接続 トランザクションの開始
        '------------------------------------------------
        MainDB = New CASTCommon.MyOracle
        MainDB.BeginTrans()
        '2017/01/19 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
        FmtComm.Oracle = MainDB
        '2017/01/19 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

        Try
            '------------------------------------------------
            '送信ファイルの作成(ワークファイル)
            '------------------------------------------------
            '処理前にワークファイル削除
            For i As Integer = 1 To 2
                If File.Exists(Path.Combine(IniInfo.DATBK, "SYUKIN_DAIKOU" & i.ToString & ".DAT")) = True Then
                    File.Delete(Path.Combine(IniInfo.DATBK, "SYUKIN_DAIKOU" & i.ToString & ".DAT"))
                End If
            Next

            If fn_CREATE_FILE() = False Then
                MainDB.Rollback()
                MainDB.Close()
                Return False
            End If

            '------------------------------------------------
            '送信ファイルのコード変換
            '------------------------------------------------
            '提携内と提携外とでファイルが異なるため、コード変換を2回行う
            For i As Integer = 1 To 2
                Me.strOUT_FILE = Path.Combine(IniInfo.DATBK, "SYUKIN_DAIKOU" & i.ToString & ".DAT")
                If File.Exists(Me.strOUT_FILE) = True Then
                    Me.strSOUSIN_FILE = Path.Combine(IniInfo.DEN, "SYUKIN_DAIKOU" & i.ToString & ".DAT")
                    If File.Exists(Me.strSOUSIN_FILE) = True Then
                        File.Delete(Me.strSOUSIN_FILE)
                    End If

                    Dim strPFilePath As String = Path.Combine(IniInfo.FTR, "120.P")
                    Dim intKEKKA As Integer = ConvertFileFtranP("PUTRAND", strOUT_FILE, strSOUSIN_FILE, strPFilePath)
                    Select Case intKEKKA
                        Case 0
                            LOG.Write("(コード変換)", "成功")
                        Case 100
                            LOG.Write("(コード変換)", "失敗")
                            LOG.UpdateJOBMASTbyErr("コード変換失敗")
                            Return False
                    End Select
                End If
            Next

            '------------------------------------------------
            '送付票の印刷
            '------------------------------------------------
            If bDataExistsFlg = True Then
                If fn_PRINT_SOUFUHYOU() = False Then
                    LOG.UpdateJOBMASTbyErr("送付票出力失敗")
                    Return False
                End If
            Else
                LOG.Write("(送付票印刷)", "成功", "印刷対象なし")
            End If

            '------------------------------------------------
            'ＤＢの開放
            '------------------------------------------------
            MainDB.Commit()
            MainDB = Nothing

            Return True

        Catch ex As Exception
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "(SSSデータ作成メイン処理)", "失敗", ex.Message)
            LOG.UpdateJOBMASTbyErr("SSSデータ作成　例外")
            MainDB.Rollback()
            Return False
        Finally
            '最後まで残っていたらロールバック
            If Not MainDB Is Nothing Then
                MainDB.Rollback()
                MainDB.Close()
                MainDB = Nothing
            End If
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "(SSSデータ作成メイン処理)終了", "成功")
        End Try
    End Function

    ''' <summary>
    ''' 配信待ちのフラグを全て元に戻す
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReturnFlg() As Integer
        Dim SQL As New StringBuilder
        Dim Ret As Integer = 0
        Dim OraReader As CASTCommon.MyOracleReader = Nothing
        Try
            '念のため、一端クローズ
            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If
            Me.MainDB = New CASTCommon.MyOracle
            OraReader = New CASTCommon.MyOracleReader(Me.MainDB)
            If OraReader.DataReader(Me.GetSSSFileCreateSQL) = True Then
                While OraReader.EOF = False
                    SQL.Length = 0
                    SQL.Append("UPDATE SCHMAST SET")
                    '2017/01/17 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                    '標準版(スリーエス決済無し)で実装
                    SQL.Append(" TAKOU_FLG_S = '0'")
                    'If OraReader.GetString("SORTKEY") = "1" Then
                    '    SQL.Append(" HAISIN_T1FLG_S = '0'")
                    'Else
                    '    SQL.Append(" HAISIN_T2FLG_S = '0'")
                    'End If
                    '2017/01/17 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                    SQL.Append(" WHERE TORIS_CODE_S = " & SQ(OraReader.GetString("TORIS_CODE_S")))
                    SQL.Append(" AND TORIF_CODE_S = " & SQ(OraReader.GetString("TORIF_CODE_S")))
                    SQL.Append(" AND FURI_DATE_S = " & SQ(OraReader.GetString("FURI_DATE_S")))

                    Ret = Me.MainDB.ExecuteNonQuery(SQL)
                    LOG.Write("配信待ち取消", "成功", Ret & "件")

                    OraReader.NextRead()
                End While
            End If

            Me.MainDB.Commit()
            Return 0

        Catch ex As Exception
            MainDB.Rollback()
            LOG.Write("配信待ち取消", "失敗", ex.ToString)
            Return -1
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
            If Not Me.MainDB Is Nothing Then
                Me.MainDB.Close()
                Me.MainDB = Nothing
            End If
        End Try
    End Function

#End Region

#Region "プライベートメソッド"

    ''' <summary>
    ''' SSSワークファイルを作成します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_CREATE_FILE() As Boolean

        Dim oraSchReader As CASTCommon.MyOracleReader = Nothing
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        Dim ZenStream As StreamWriter = Nothing
        Dim ZenFmt As New CAstFormat.CFormatZengin

        Dim dblFURI_KEN As Double
        Dim dblFURI_KIN As Double
        Dim bDataRecordExistsFlg As Boolean = False
        Dim bMeisaiExistsFlg As Boolean = False

        Dim OldSortKey As String = ""
        Dim bTeikeinaiOpenFlg As Boolean = False
        Dim bTeikeigaiOpenFlg As Boolean = False

        Try
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "(SSSデータ作成)開始", "成功")

            '------------------------------------------------
            '対象スケジュールの検索
            '------------------------------------------------
            oraSchReader = New CASTCommon.MyOracleReader(MainDB)
            If oraSchReader.DataReader(GetSSSFileCreateSQL) = True Then

                '明細用のリーダーオープン
                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)

                While oraSchReader.EOF = False

                    'データレコード存在フラグ初期化
                    bDataRecordExistsFlg = False

                    'ヘッダレコードの取得
                    If oraMeiReader.DataReader(GetMeimastSQL("1", oraSchReader)) = True Then
                        '2017/02/27 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                        'ヘッダの委託者コードのパターンを設定する。
                        Select Case IniInfo.SSS_ITAKUCODE_PATN
                            Case "0"
                                '0:振替依頼データ委託者コードをそのまま使用
                                ZenFmt.ZENGIN_REC1.Data = oraMeiReader.GetItem("FURI_DATA_K")
                            Case "1"
                                '1:委託者コードの下1桁に提携種別を設定する
                                ZenFmt.ZENGIN_REC1.Data = oraMeiReader.GetItem("FURI_DATA_K")
                                ZenFmt.ZENGIN_REC1.ZG4 = ZenFmt.ZENGIN_REC1.ZG4.Substring(0, 9) & oraSchReader.GetString("SORTKEY")
                            Case "2"
                                '2:特殊("03" + 自金庫コード下3桁 + 委託者コード下4桁 + 提携種別)
                                ZenFmt.ZENGIN_REC1.Data = oraMeiReader.GetItem("FURI_DATA_K")
                                ZenFmt.ZENGIN_REC1.ZG4 = "03" & IniInfo.KINKOCD.Substring(1, 3) & ZenFmt.ZENGIN_REC1.ZG4.Substring(6, 4) & oraSchReader.GetString("SORTKEY")
                            Case Else
                                '上記以外は振替データを使用する
                                ZenFmt.ZENGIN_REC1.Data = oraMeiReader.GetItem("FURI_DATA_K")
                        End Select
                        'ZenFmt.ZENGIN_REC1.Data = oraMeiReader.GetItem("FURI_DATA_K")
                        '2017/02/27 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                    Else
                        LOG.Write("(SSSデータ作成)", "失敗", "ヘッダレコード取得失敗")
                        LOG.UpdateJOBMASTbyErr("SSSデータ作成失敗 ヘッダレコード取得失敗")
                        Return False
                    End If

                    oraMeiReader.Close()

                    'シーケンス番号の取得
                    strJIFURI_SEQ = fn_SELECT_SEQ(oraSchReader.GetString("FURI_DATE_S"))

                    'データレコードの書き込み
                    If oraMeiReader.DataReader(GetMeimastSQL("2", oraSchReader)) = True Then
                        '件数と金額初期化
                        dblFURI_KEN = 0
                        dblFURI_KIN = 0
                        'データレコード存在フラグ更新
                        bDataRecordExistsFlg = True
                        '明細存在フラグ更新
                        bMeisaiExistsFlg = True

                        If oraSchReader.GetString("SORTKEY") = "1" AndAlso bTeikeinaiOpenFlg = False Then
                            '提携内のデータ
                            Dim strTeikeinaiWorkFileName As String = Path.Combine(IniInfo.DATBK, "SYUKIN_DAIKOU1.DAT")
                            If File.Exists(strTeikeinaiWorkFileName) = True Then
                                File.Delete(strTeikeinaiWorkFileName)
                            End If

                            ZenStream = New StreamWriter(strTeikeinaiWorkFileName, False, Encoding.GetEncoding("SHIFT-JIS"))
                            bTeikeinaiOpenFlg = True
                        ElseIf oraSchReader.GetString("SORTKEY") = "2" AndAlso bTeikeigaiOpenFlg = False Then
                            '提携外のデータ
                            If bTeikeinaiOpenFlg = True Then
                                'すでに提携内のデータを作成している場合はエンドレコード書き込んでクローズ
                                ZenFmt.ZENGIN_REC9.ZG1 = "9"
                                ZenStream.Write(ZenFmt.ZENGIN_REC9.Data)
                                If Not ZenStream Is Nothing Then
                                    ZenStream.Close()
                                    ZenStream = Nothing
                                End If
                            End If

                            Dim strTeikeigaiWorkFileName As String = Path.Combine(IniInfo.DATBK, "SYUKIN_DAIKOU2.DAT")
                            If File.Exists(strTeikeigaiWorkFileName) = True Then
                                File.Delete(strTeikeigaiWorkFileName)
                            End If

                            ZenStream = New StreamWriter(strTeikeigaiWorkFileName, False, Encoding.GetEncoding("SHIFT-JIS"))
                            bTeikeigaiOpenFlg = True
                        End If

                        'ヘッダレコードの書き込み
                        ZenStream.Write(ZenFmt.ZENGIN_REC1.Data)


                        While oraMeiReader.EOF = False
                            dblFURI_KEN += 1
                            dblFURI_KIN += oraMeiReader.GetInt64("FURIKIN_K")
                            ZenFmt.ZENGIN_REC2.Data = oraMeiReader.GetItem("FURI_DATA_K")

                            If oraMeiReader.GetString("TEKIYO_KBN_K") <> "1" Then
                                If oraMeiReader.GetString("KTEKIYO_K") = String.Empty Then
                                    '店舗名は空
                                    ZenFmt.ZENGIN_REC2.ZG5 = String.Empty
                                Else
                                    '店舗名にカナ摘要
                                    ZenFmt.ZENGIN_REC2.ZG5 = oraMeiReader.GetString("KTEKIYO_K").PadRight(15, " "c)
                                End If
                            Else
                                '店舗名は空
                                ZenFmt.ZENGIN_REC2.ZG5 = String.Empty
                            End If

                            '顧客番号
                            Dim strKokyakuNo As String = String.Concat(New String() {oraSchReader.GetString("ITAKU_CODE_T").Substring(5, 4), "0", IniInfo.KINKOCD, "00000000000"})
                            ZenFmt.ZENGIN_REC2.ZG12 = strKokyakuNo.Substring(0, 10)
                            ZenFmt.ZENGIN_REC2.ZG13 = strKokyakuNo.Substring(10, 10)

                            'ダミー部(シーケンス)
                            ZenFmt.ZENGIN_REC2.ZG15 = strJIFURI_SEQ

                            ZenStream.Write(ZenFmt.ZENGIN_REC2.Data)


                            '------------------------------------------------
                            '明細マスタのシーケンス番号の更新
                            '------------------------------------------------
                            If UpdateMeimastJifuriSEQ(oraMeiReader) = False Then
                                Return False
                            End If

                            strJIFURI_SEQ = CStr(CLng(strJIFURI_SEQ) + 1)

                            oraMeiReader.NextRead()
                        End While
                    End If

                    oraMeiReader.Close()


                    'トレーラレコードの書き込み
                    If bDataRecordExistsFlg = True Then
                        ZenFmt.ZENGIN_REC8.ZG1 = "8"
                        ZenFmt.ZENGIN_REC8.ZG2 = dblFURI_KEN.ToString.PadLeft(6, "0"c)
                        ZenFmt.ZENGIN_REC8.ZG3 = dblFURI_KIN.ToString.PadLeft(12, "0"c)
                        ZenFmt.ZENGIN_REC8.ZG4 = "".PadLeft(6, "0"c)
                        ZenFmt.ZENGIN_REC8.ZG5 = "".PadLeft(12, "0"c)
                        ZenFmt.ZENGIN_REC8.ZG6 = "".PadLeft(6, "0"c)
                        ZenFmt.ZENGIN_REC8.ZG7 = "".PadLeft(12, "0"c)

                        ZenStream.Write(ZenFmt.ZENGIN_REC8.Data)
                    End If

                    '他行スケジュールマスタ作成
                    If Me.fn_INSERT_TAKOSCHMAST(oraSchReader) = False Then
                        Return False
                    End If

                    '------------------------------------------------
                    'スケジュールマスタ更新
                    '※他行分データを作成していなくても更新
                    '------------------------------------------------
                    If fn_UPDATE_SCHMAST(oraSchReader) = False Then
                        Return False
                    End If

                    oraSchReader.NextRead()
                End While

            Else
                'スケジュールなし
                LOG.Write("(SSSデータ作成)", "失敗", "対象スケジュールなし")
                LOG.UpdateJOBMASTbyErr("SSSデータ作成失敗 対象スケジュールなし")
                Return False
            End If

            If bMeisaiExistsFlg = True Then
                'エンドレコードの書き込み
                ZenFmt.ZENGIN_REC9.ZG1 = "9"
                ZenStream.Write(ZenFmt.ZENGIN_REC9.Data)

                bDataExistsFlg = True
            End If

            If Not ZenStream Is Nothing Then ZenStream.Close()

            Return True

        Catch ex As Exception
            LOG.Write("(SSSデータ作成)", "失敗", ex.Message)
            LOG.UpdateJOBMASTbyErr("SSSデータ作成失敗 例外発生")
            Return False
        Finally
            If Not ZenStream Is Nothing Then ZenStream.Close()
            LOG.Write(LOG.ToriCode, LOG.FuriDate, "(SSSデータ作成)終了", "成功")
        End Try
    End Function

    ''' <summary>
    ''' シーケンス番号の検索を行います。
    ''' </summary>
    ''' <param name="strFuriDate">振替日</param>
    ''' <returns>MAXシーケンス番号</returns>
    ''' <remarks></remarks>
    Private Function fn_SELECT_SEQ(ByVal strFuriDate As String) As String
        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        SQL.Append("SELECT NVL(MAX(KIGYO_SEQ_K), 69999999) AS JIFURI_SEQ_MAX, COUNT(KIGYO_SEQ_K) AS JIFURI_SEQ_CNT")
        SQL.Append(" FROM MEIMAST")
        SQL.Append(" WHERE FURI_DATE_K = " & SQ(strFuriDate))
        SQL.Append(" AND KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
        SQL.Append(" AND KIGYO_SEQ_K BETWEEN '70000000' AND '79999999'")

        Try
            If oraReader.DataReader(SQL) = True Then
                Return (oraReader.GetInt64("JIFURI_SEQ_MAX") + 1).ToString
            Else
                Return "70000000"
            End If
        Catch ex As Exception
            Return "70000000"
        Finally
            oraReader.Close()
        End Try

    End Function

    ''' <summary>
    ''' スケジュールマスタを更新します。
    ''' </summary>
    ''' <param name="oraSchReader">スケジュールオラクルリーダー</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_UPDATE_SCHMAST(ByVal oraSchReader As CASTCommon.MyOracleReader) As Boolean
        Dim SQL As New StringBuilder
        With SQL
            .Append("update SCHMAST set ")
            '2017/01/17 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
            '標準版(スリーエス決済無し)で実装
            .Append(" TAKOU_FLG_S = '1'")
            .Append(",YOBI1_S = " & SQ(strDate & strTime))
            'If oraSchReader.GetString("SORTKEY") = "1" Then
            '    .Append(" HAISIN_T1DATE_S = " & SQ(strDate))
            '    .Append(",HAISIN_T1FLG_S = '1'")
            '    .Append(",JIFURI_T1TIME_STAMP_S = " & SQ(strDate & strTime))
            'Else
            '    .Append(" HAISIN_T2DATE_S = " & SQ(strDate))
            '    .Append(",HAISIN_T2FLG_S = '1'")
            '    .Append(",JIFURI_T2TIME_STAMP_S = " & SQ(strDate & strTime))
            'End If
            '2017/01/17 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
            .Append(" where TORIS_CODE_S = " & SQ(oraSchReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_S = " & SQ(oraSchReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_S = " & SQ(oraSchReader.GetString("FURI_DATE_S")))
        End With

        Try
            Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If nRet < 1 Then
                LOG.Write("(スケジュールマスタ更新)", "失敗", _
                          "取引先コード：" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIS_CODE_S") & _
                          "振替日：" & oraSchReader.GetString("FURI_DATE_S"))
                LOG.UpdateJOBMASTbyErr("スケジュールマスタ更新失敗 取引先コード：" & oraSchReader.GetString("TORIS_CODE_S") & "-" & oraSchReader.GetString("TORIS_CODE_S") & _
                          "振替日：" & oraSchReader.GetString("FURI_DATE_S"))
                Return False
            End If

            Return True
        Catch ex As Exception
            LOG.Write("(スケジュールマスタ更新)", "失敗", ex.Message)
            LOG.UpdateJOBMASTbyErr("スケジュールマスタ更新失敗 例外発生")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 設定ファイルを読み込みます。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_INI_READ() As Boolean
        Try
            IniInfo.DATBK = CASTCommon.GetFSKJIni("COMMON", "DATBK")
            If IniInfo.DATBK.ToUpper.Equals("ERR") = True OrElse IniInfo.DATBK = Nothing Then
                LOG.Write("(設定ファイル読み込み)", "失敗", "[COMMON].DATBK 設定なし")
                LOG.UpdateJOBMASTbyErr("設定ファイル読み込み [COMMON].DATBK 設定なし")
                Return False
            End If

            IniInfo.DEN = CASTCommon.GetFSKJIni("COMMON", "DEN")
            If IniInfo.DEN.ToUpper.Equals("ERR") = True OrElse IniInfo.DEN = Nothing Then
                LOG.Write("(設定ファイル読み込み)", "失敗", "[COMMON].DEN 設定なし")
                LOG.UpdateJOBMASTbyErr("設定ファイル読み込み [COMMON].DEN 設定なし")
                Return False
            End If

            IniInfo.FTR = CASTCommon.GetFSKJIni("COMMON", "FTR")
            If IniInfo.FTR.ToUpper.Equals("ERR") = True OrElse IniInfo.FTR = Nothing Then
                LOG.Write("(設定ファイル読み込み)", "失敗", "[COMMON].FTR 設定なし")
                LOG.UpdateJOBMASTbyErr("設定ファイル読み込み [COMMON].FTR 設定なし")
                Return False
            End If

            IniInfo.FTRANP = CASTCommon.GetFSKJIni("COMMON", "FTRANP")
            If IniInfo.FTRANP.ToUpper.Equals("ERR") = True OrElse IniInfo.FTRANP = Nothing Then
                LOG.Write("(設定ファイル読み込み)", "失敗", "[COMMON].FTRANP 設定なし")
                LOG.UpdateJOBMASTbyErr("設定ファイル読み込み [COMMON].FTRANP 設定なし")
                Return False
            End If

            IniInfo.KINKOCD = CASTCommon.GetFSKJIni("COMMON", "KINKOCD")
            If IniInfo.KINKOCD.ToUpper.Equals("ERR") = True OrElse IniInfo.KINKOCD = Nothing Then
                LOG.Write("(設定ファイル読み込み)", "失敗", "[COMMON].KINKOCD 設定なし")
                LOG.UpdateJOBMASTbyErr("設定ファイル読み込み [COMMON].KINKOCD 設定なし")
                Return False
            End If

            IniInfo.SSS_SOUFU = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SSS_SOUFU")
            If IniInfo.SSS_SOUFU.ToUpper.Equals("ERR") = True OrElse IniInfo.SSS_SOUFU = Nothing Then
                LOG.Write("(設定ファイル読み込み)", "失敗", "[RSV2_V1.0.0].SSS_SOUFU 設定なし")
                LOG.UpdateJOBMASTbyErr("設定ファイル読み込み [RSV2_V1.0.0].SSS_SOUFU 設定なし")
                Return False
            End If

            IniInfo.SSS_SOUFU_TEL = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SSS_SOUFU_TEL")
            If IniInfo.SSS_SOUFU_TEL.ToUpper.Equals("ERR") = True OrElse IniInfo.SSS_SOUFU_TEL = Nothing Then
                LOG.Write("(設定ファイル読み込み)", "失敗", "[RSV2_V1.0.0].SSS_SOUFU_TEL 設定なし")
                LOG.UpdateJOBMASTbyErr("設定ファイル読み込み [RSV2_V1.0.0].SSS_SOUFU_TEL 設定なし")
                Return False
            End If

            IniInfo.SSS_SOUFU_FAX = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SSS_SOUFU_FAX")
            If IniInfo.SSS_SOUFU_FAX.ToUpper.Equals("ERR") = True OrElse IniInfo.SSS_SOUFU_FAX = Nothing Then
                LOG.Write("(設定ファイル読み込み)", "失敗", "[RSV2_V1.0.0].SSS_SOUFU_FAX 設定なし")
                LOG.UpdateJOBMASTbyErr("設定ファイル読み込み [RSV2_V1.0.0].SSS_SOUFU_FAX 設定なし")
                Return False
            End If

            '2017/01/19 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
            IniInfo.FUNOU_SSS_1 = CASTCommon.GetFSKJIni("JIFURI", "FUNOU_SSS_1")
            If IniInfo.FUNOU_SSS_1.ToUpper.Equals("ERR") = True OrElse IniInfo.FUNOU_SSS_1 = Nothing Then
                LOG.Write("(設定ファイル読み込み)", "失敗", "[JIFURI].FUNOU_SSS_1 設定なし")
                LOG.UpdateJOBMASTbyErr("設定ファイル読み込み [JIFURI].FUNOU_SSS_1 設定なし")
                Return False
            End If

            IniInfo.FUNOU_SSS_2 = CASTCommon.GetFSKJIni("JIFURI", "FUNOU_SSS_2")
            If IniInfo.FUNOU_SSS_2.ToUpper.Equals("ERR") = True OrElse IniInfo.FUNOU_SSS_2 = Nothing Then
                LOG.Write("(設定ファイル読み込み)", "失敗", "[JIFURI].FUNOU_SSS_2 設定なし")
                LOG.UpdateJOBMASTbyErr("設定ファイル読み込み [JIFURI].FUNOU_SSS_2 設定なし")
                Return False
            End If
            '2017/01/19 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

            '2017/02/27 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
            IniInfo.SSS_ITAKUCODE_PATN = CASTCommon.GetRSKJIni("RSV2_V1.0.0", "SSS_ITAKUCODE_PATN")
            If IniInfo.SSS_ITAKUCODE_PATN.ToUpper.Equals("ERR") = True OrElse IniInfo.SSS_ITAKUCODE_PATN = Nothing Then
                LOG.Write("(設定ファイル読み込み)", "失敗", "[RSV2_V1.0.0].SSS_ITAKUCODE_PATN 設定なし")
                LOG.UpdateJOBMASTbyErr("設定ファイル読み込み [RSV2_V1.0.0].SSS_ITAKUCODE_PATN 設定なし")
                Return False
            End If
            '2017/02/27 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

            Return True
        Catch ex As Exception
            LOG.Write("(設定ファイル読み込み)", "失敗", ex.Message)
            LOG.UpdateJOBMASTbyErr("設定ファイル読み込み 例外発生")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' SSSファイル作成対象のスケジュールを検索するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function GetSSSFileCreateSQL() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            '提携内
            .Append("SELECT ")
            .Append(" '1' SORTKEY")
            .Append(",TORIS_CODE_S KEY1")
            .Append(",TORIF_CODE_S KEY2")
            .Append(",SCHMAST.*")
            .Append(",TORIMAST.*")
            .Append(" FROM SCHMAST,TORIMAST")
            .Append(" WHERE ")
            '2017/01/17 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
            '標準版(スリーエス決済無し)で実装
            .Append("     FURI_DATE_S = " & SQ(FURI_DATE))
            .Append(" AND TAKOU_FLG_S = '2'")
            '.Append("     HAISIN_T1FLG_S = '2'")
            '.Append(" AND HAISIN_T1YDATE_S = " & SQ(HAISIN_YDATE))
            '2017/01/17 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
            .Append(" AND TOUROKU_FLG_S = '1'")
            .Append(" AND TYUUDAN_FLG_S = '0'")
            .Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            .Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            .Append(" AND FMT_KBN_T IN ('20', '21')")
            .Append(" AND MOTIKOMI_KBN_T = '0'")
            '2017/01/17 saitou 東春信金(RSV2標準) DEL スリーエス対応 ---------------------------------------- START
            '標準版(スリーエス決済無し)で実装
            '.Append(" UNION ALL ")
            ''提携外
            '.Append("SELECT ")
            '.Append(" '2' SORTKEY")
            '.Append(",TORIS_CODE_S KEY1")
            '.Append(",TORIF_CODE_S KEY2")
            '.Append(",SCHMAST.*")
            '.Append(",TORIMAST.*")
            '.Append(" FROM SCHMAST,TORIMAST")
            '.Append(" WHERE ")
            '.Append("     HAISIN_T2FLG_S = '2'")
            '.Append(" AND HAISIN_T2YDATE_S = " & SQ(HAISIN_YDATE))
            '.Append(" AND TOUROKU_FLG_S = '1'")
            '.Append(" AND TYUUDAN_FLG_S = '0'")
            '.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            '.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            '.Append(" AND FMT_KBN_T = '21'")
            '.Append(" AND MOTIKOMI_KBN_T = '0'")
            '2017/01/17 saitou 東春信金(RSV2標準) DEL ------------------------------------------------------- END
            .Append(" ORDER BY SORTKEY ASC, KEY1 ASC, KEY2 ASC")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' SSSファイル作成対象の明細を取得するSQLを作成します。
    ''' </summary>
    ''' <param name="strDataKbn">データ区分</param>
    ''' <param name="oraSchReader">スケジュールオラクルリーダー</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function GetMeimastSQL(ByVal strDataKbn As String, _
                                   ByVal oraSchReader As CASTCommon.MyOracleReader) As StringBuilder
        Dim SQL As New StringBuilder
        SQL.Append("select * from MEIMAST")
        SQL.Append(" where TORIS_CODE_K = " & SQ(oraSchReader.GetString("TORIS_CODE_S")))
        SQL.Append(" and TORIF_CODE_K = " & SQ(oraSchReader.GetString("TORIF_CODE_S")))
        SQL.Append(" and FURI_DATE_K = " & SQ(oraSchReader.GetString("FURI_DATE_S")))
        Select Case strDataKbn
            Case "1"
                SQL.Append(" and DATA_KBN_K = '1'")
            Case "2"
                SQL.Append(" and DATA_KBN_K = '2'")
                SQL.Append(" and KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
                SQL.Append(" and FURIKETU_CODE_K = '0'")
                SQL.Append(" and exists (")
                SQL.Append(" select TEIKEI_KBN_N")
                SQL.Append("    from TENMAST")
                SQL.Append("    where KIN_NO_N = KEIYAKU_KIN_K")
                If oraSchReader.GetString("SORTKEY") = "1" Then
                    '提携内
                    SQL.Append("    and TEIKEI_KBN_N = '1'")
                Else
                    '提携外
                    SQL.Append("    and TEIKEI_KBN_N = '0'")
                End If
                SQL.Append(" )")
                'ソート順は大規模に合わせる
                SQL.Append(" order by KEIYAKU_KIN_K, KEIYAKU_SIT_K")
                SQL.Append(",decode(KEIYAKU_KAMOKU_K,'02',1,'01',2,'05',3,'37',4,9)")
                SQL.Append(",KEIYAKU_KOUZA_K, RECORD_NO_K")
            Case "8"
            Case "9"
        End Select
        Return SQL
    End Function

    ''' <summary>
    ''' 明細マスタの自振シーケンスを更新します。
    ''' </summary>
    ''' <param name="oraMeiReader">明細オラクルリーダー</param>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function UpdateMeimastJifuriSEQ(ByVal oraMeiReader As CASTCommon.MyOracleReader) As Boolean
        Dim SQL As New StringBuilder
        With SQL
            .Append("update MEIMAST set ")
            .Append(" KIGYO_SEQ_K = " & SQ(strJIFURI_SEQ))
            .Append(" where TORIS_CODE_K = " & SQ(oraMeiReader.GetString("TORIS_CODE_K")))
            .Append(" and TORIF_CODE_K = " & SQ(oraMeiReader.GetString("TORIF_CODE_K")))
            .Append(" and FURI_DATE_K = " & SQ(oraMeiReader.GetString("FURI_DATE_K")))
            .Append(" and RECORD_NO_K = " & oraMeiReader.GetInt64("RECORD_NO_K"))
        End With

        Try
            Dim nRet As Integer = MainDB.ExecuteNonQuery(SQL)
            If nRet < 1 Then
                LOG.Write("(明細マスタ更新)", "失敗", _
                          "取引先コード：" & oraMeiReader.GetString("TORIS_CODE_K") & "-" & oraMeiReader.GetString("TORIF_CODE_K") & _
                          " 振替日：" & oraMeiReader.GetString("FURI_DATE_K") & " レコード番号：" & oraMeiReader.GetInt64("RECORD_NO_K"))
                LOG.UpdateJOBMASTbyErr("明細マスタ更新失敗 取引先コード：" & oraMeiReader.GetString("TORIS_CODE_K") & "-" & oraMeiReader.GetString("TORIF_CODE_K") & _
                          " 振替日：" & oraMeiReader.GetString("FURI_DATE_K") & " レコード番号：" & oraMeiReader.GetInt64("RECORD_NO_K"))
                Return False
            End If

            Return True
        Catch ex As Exception
            LOG.Write("(明細マスタ更新)", "失敗", ex.Message)
            LOG.UpdateJOBMASTbyErr("明細マスタ更新失敗 例外発生")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' FTRANPを用いてコード変換を行います。
    ''' </summary>
    ''' <param name="strGetOrPut">GETRAND,GETDATA,PUTRAND,PUTDATA</param>
    ''' <param name="strInFileName">入力ファイルパス</param>
    ''' <param name="strOutFileName">出力ファイルパス</param>
    ''' <param name="strPFileName">FTRANPパラメータファイル名</param>
    ''' <returns>0:正常 100:異常</returns>
    ''' <remarks></remarks>
    Private Function ConvertFileFtranP(ByVal strGetOrPut As String, _
                                       ByVal strInFileName As String, _
                                       ByVal strOutFileName As String, _
                                       ByVal strPFileName As String) As Integer
        Try
            '変換コマンド組み立て
            Dim Command As New StringBuilder
            With Command
                .Append(" /nwd/ cload ")
                .Append("""" & IniInfo.FTR & "FUSION" & """")
                .Append(" ; kanji 83_jis")
                .Append(" " & strGetOrPut & " ")
                .Append("""" & strInFileName & """" & " ")
                .Append("""" & strOutFileName & """" & " ")
                .Append(" ++" & """" & strPFileName & """")
            End With

            Dim Proc As New Process
            Dim ProcInfo As New ProcessStartInfo
            ProcInfo.FileName = Path.Combine(IniInfo.FTRANP, "FP.EXE")
            ProcInfo.WorkingDirectory = IniInfo.FTRANP
            ProcInfo.Arguments = Command.ToString
            Proc = Process.Start(ProcInfo)
            Proc.WaitForExit()
            If Proc.ExitCode = 0 Then
                Return 0
            Else
                LOG.Write("(FTRANPコード変換)", "失敗", "終了コード：" & Proc.ExitCode)
                Return 100
            End If
        Catch ex As Exception
            LOG.Write("(FTRANPコード変換)", "失敗", ex.Message)
            Return 100
        End Try
    End Function

    ''' <summary>
    ''' 送付票を出力します。
    ''' </summary>
    ''' <returns>True or False</returns>
    ''' <remarks></remarks>
    Private Function fn_PRINT_SOUFUHYOU() As Boolean

        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)
        Dim oraMeiReader As CASTCommon.MyOracleReader = Nothing

        Try
            LOG.Write("(送付票出力)開始", "成功")

            Dim List As New KF3SP005
            Dim Name As String = List.CreateCsvFile()

            '印刷対象のスケジュールを検索
            If oraReader.DataReader(GetSchPrintSOUFUHYOU) = True Then

                oraMeiReader = New CASTCommon.MyOracleReader(MainDB)

                While oraReader.EOF = False
                    If oraMeiReader.DataReader(GetMeiPrintSOUFUHYOU(oraReader)) = True Then
                        If oraMeiReader.GetInt("FURIKEN") = 0 Then
                            '件数が0件はデータでも作成していないため、排除
                        Else
                            List.OutputCsvData(oraReader.GetString("FURI_DATE_S"), True)
                            List.OutputCsvData(IniInfo.SSS_SOUFU, True)
                            List.OutputCsvData(IniInfo.SSS_SOUFU_TEL, True)
                            List.OutputCsvData(IniInfo.SSS_SOUFU_FAX, True)
                            '2017/02/27 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
                            Dim strItakuCode As String = String.Empty
                            Select Case IniInfo.SSS_ITAKUCODE_PATN
                                Case "0"
                                    strItakuCode = oraReader.GetString("ITAKU_CODE_T")
                                Case "1"
                                    strItakuCode = oraReader.GetString("ITAKU_CODE_T").Substring(0, 9) & oraReader.GetString("SORTKEY")
                                Case "2"
                                    strItakuCode = "03" & IniInfo.KINKOCD.Substring(1, 3) & oraReader.GetString("ITAKU_CODE_T").Substring(6, 4) & oraReader.GetString("SORTKEY")
                                Case Else
                                    strItakuCode = oraReader.GetString("ITAKU_CODE_T")
                            End Select
                            ''委託者コードはSSS仕様
                            'Dim strItakuCode As String
                            'strItakuCode = "03" & IniInfo.KINKOCD.Substring(1, 3) & oraReader.GetString("ITAKU_CODE_T").Substring(5, 4) & "1"
                            '2017/02/27 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
                            List.OutputCsvData(strItakuCode, True)
                            List.OutputCsvData(oraReader.GetString("ITAKU_NNAME_T"), True)
                            List.OutputCsvData(oraMeiReader.GetInt("FURIKEN").ToString)
                            List.OutputCsvData(oraMeiReader.GetInt64("FURIKIN").ToString)
                            If oraReader.GetString("SORTKEY") = "1" Then
                                List.OutputCsvData("1", True, True)
                            Else
                                List.OutputCsvData("2", True, True)
                            End If
                        End If
                    End If

                    oraMeiReader.Close()


                    oraReader.NextRead()
                End While

            Else
                LOG.Write("(送付票出力)", "失敗", "スケジュールなし")
                Return False
            End If

            List.CloseCsv()
            If List.ReportExecute = True Then
                LOG.Write("(送付票出力)", "成功")
            Else
                LOG.Write("(送付票出力)", "失敗", List.ReportMessage)
                Return False
            End If

            Return True
        Catch ex As Exception
            LOG.Write("(送付票出力)", "失敗", ex.Message)
            Return False
        Finally
            LOG.Write("(送付票出力)終了", "成功")
            If Not oraReader Is Nothing Then oraReader.Close()
            If Not oraMeiReader Is Nothing Then oraMeiReader.Close()
        End Try
    End Function

    ''' <summary>
    ''' 送付票印刷対象のスケジュールを取得するSQLを作成します。
    ''' </summary>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function GetSchPrintSOUFUHYOU() As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            '提携内
            .Append("SELECT ")
            .Append(" '1' SORTKEY")
            .Append(",TORIS_CODE_S KEY1")
            .Append(",TORIF_CODE_S KEY2")
            .Append(",SCHMAST.*")
            .Append(",TORIMAST.*")
            .Append(" FROM SCHMAST,TORIMAST")
            .Append(" WHERE")
            '2017/01/17 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
            '標準版(スリーエス決済無し)で実装
            .Append("     TAKOU_FLG_S = '1'")
            .Append(" AND YOBI1_S = " & SQ(strDate & strTime))
            '.Append("     HAISIN_T1FLG_S = '1'")
            '.Append(" AND HAISIN_T1DATE_S = " & SQ(strDate))
            '.Append(" AND JIFURI_T1TIME_STAMP_S = " & SQ(strDate & strTime))
            '2017/01/17 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END
            .Append(" AND TOUROKU_FLG_S = '1'")
            .Append(" AND TYUUDAN_FLG_S = '0'")
            .Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            .Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            .Append(" AND FMT_KBN_T IN ('20', '21')")
            .Append(" AND MOTIKOMI_KBN_T = '0'")
            '2017/01/17 saitou 東春信金(RSV2標準) DEL スリーエス対応 ---------------------------------------- START
            '標準版(スリーエス決済無し)で実装
            '.Append(" UNION ALL ")
            ''提携外
            '.Append("SELECT ")
            '.Append(" '2' SORTKEY")
            '.Append(",TORIS_CODE_S KEY1")
            '.Append(",TORIF_CODE_S KEY2")
            '.Append(",SCHMAST.*")
            '.Append(",TORIMAST.*")
            '.Append(" FROM SCHMAST,TORIMAST")
            '.Append(" WHERE HAISIN_T2FLG_S = '1'")
            '.Append(" AND HAISIN_T2DATE_S = " & SQ(strDate))
            '.Append(" AND JIFURI_T2TIME_STAMP_S = " & SQ(strDate & strTime))
            '.Append(" AND TOUROKU_FLG_S = '1'")
            '.Append(" AND TYUUDAN_FLG_S = '0'")
            '.Append(" AND TORIS_CODE_S = TORIS_CODE_T")
            '.Append(" AND TORIF_CODE_S = TORIF_CODE_T")
            '.Append(" AND FMT_KBN_T = '21'")
            '.Append(" AND MOTIKOMI_KBN_T = '0'")
            '2017/01/17 saitou 東春信金(RSV2標準) DEL ------------------------------------------------------- END
            .Append(" ORDER BY SORTKEY ASC, KEY1 ASC, KEY2 ASC")
        End With
        Return SQL
    End Function

    ''' <summary>
    ''' 送付票印刷対象の明細を取得するSQLを作成します。
    ''' </summary>
    ''' <param name="oraSchReader">スケジュールオラクルリーダー</param>
    ''' <returns>作成SQL</returns>
    ''' <remarks></remarks>
    Private Function GetMeiPrintSOUFUHYOU(ByVal oraSchReader As CASTCommon.MyOracleReader) As StringBuilder
        Dim SQL As New StringBuilder
        With SQL
            .Append("select ")
            .Append("  count(FURIKIN_K) as FURIKEN")
            .Append(", sum(FURIKIN_K) as FURIKIN")
            .Append(" from MEIMAST")
            .Append(" where TORIS_CODE_K = " & SQ(oraSchReader.GetString("TORIS_CODE_S")))
            .Append(" and TORIF_CODE_K = " & SQ(oraSchReader.GetString("TORIF_CODE_S")))
            .Append(" and FURI_DATE_K = " & SQ(oraSchReader.GetString("FURI_DATE_S")))
            .Append(" and KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
            .Append(" and DATA_KBN_K = '2'")
            .Append(" and FURIKETU_CODE_K = 0")
            .Append(" and exists (")
            .Append(" select TEIKEI_KBN_N from TENMAST")
            .Append(" where KIN_NO_N = KEIYAKU_KIN_K")
            If oraSchReader.GetString("SORTKEY") = "1" Then
                '提携内
                .Append(" and TEIKEI_KBN_N = '1'")
            Else
                '提携外
                .Append(" and TEIKEI_KBN_N = '0'")
            End If
            .Append(" )")
        End With
        Return SQL
    End Function


    ''' <summary>
    ''' 他行スケジュールマスタを作成します。
    ''' </summary>
    ''' <param name="OraSchReader"></param>
    ''' <returns>True or False</returns>
    ''' <remarks>
    ''' 2017/01/19 saitou 東春信金(RSV2標準) added for スリーエス対応
    ''' 標準版(スリーエス決済無し)は今まで他行スケジュールマスタを作っていなかったが、
    ''' 標準組み込みで他行スケジュールを作るようにしてみる。
    ''' </remarks>
    Private Function fn_INSERT_TAKOSCHMAST(ByVal OraSchReader As CASTCommon.MyOracleReader) As Boolean
        Dim SQL As New StringBuilder
        Dim oraReader As New CASTCommon.MyOracleReader(MainDB)

        Try
            '2017/01/19 saitou 東春信金(RSV2標準) ADD スリーエス対応 ---------------------------------------- START
            '不能結果更新予定日を算出
            '標準版(RSV2スリーエス決済無し)なので、提携内だけでOK
            Dim FUNOU_T1YDATE As String = String.Empty
            FUNOU_T1YDATE = CASTCommon.GetEigyobi(CASTCommon.ConvertDate(OraSchReader.GetString("FURI_DATE_S")), _
                                                  CInt(Me.IniInfo.FUNOU_SSS_1), _
                                                  FmtComm.HolidayList).ToString("yyyyMMdd")
            '2017/01/19 saitou 東春信金(RSV2標準) ADD ------------------------------------------------------- END

            '--------------------------------------------------
            'エラー件数、金額の抽出
            '--------------------------------------------------
            Dim inputErrCnt As Long = 0
            Dim inputErrKin As Long = 0

            With SQL
                .Length = 0
                .Append("SELECT COUNT(FURIKIN_K) AS CNT,NVL(SUM(FURIKIN_K),0) AS KIN FROM MEIMAST ")
                .Append(" WHERE TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                .Append(" AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                .Append(" AND FURI_DATE_K = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                .Append(" AND DATA_KBN_K = '2'")
                .Append(" AND KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
                .Append(" AND FURIKETU_CODE_K <> 0")
                .Append(" AND EXISTS (")
                .Append(" SELECT TEIKEI_KBN_N FROM TENMAST")
                .Append(" WHERE KIN_NO_N = KEIYAKU_KIN_K")
                If OraSchReader.GetString("SORTKEY") = "1" Then
                    .Append(" AND TEIKEI_KBN_N = '1'")
                Else
                    .Append(" AND TEIKEI_KBN_N = '0'")
                End If
                .Append(" )")
            End With

            If oraReader.DataReader(SQL) = True Then
                inputErrCnt = oraReader.GetInt64("CNT")
                inputErrKin = oraReader.GetInt64("KIN")
            End If

            oraReader.Close()

            '--------------------------------------------------
            'エラーでない件数、金額の抽出
            '--------------------------------------------------
            Dim normalCnt As Long = 0
            Dim normalKin As Long = 0

            With SQL
                .Length = 0
                .Append("SELECT COUNT(FURIKIN_K) AS CNT,NVL(SUM(FURIKIN_K),0) AS KIN FROM MEIMAST ")
                .Append(" WHERE TORIS_CODE_K = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                .Append(" AND TORIF_CODE_K = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                .Append(" AND FURI_DATE_K = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
                .Append(" AND DATA_KBN_K = '2'")
                .Append(" AND KEIYAKU_KIN_K <> " & SQ(IniInfo.KINKOCD))
                .Append(" AND FURIKETU_CODE_K = 0")
                .Append(" AND EXISTS (")
                .Append(" SELECT TEIKEI_KBN_N FROM TENMAST")
                .Append(" WHERE KIN_NO_N = KEIYAKU_KIN_K")
                If OraSchReader.GetString("SORTKEY") = "1" Then
                    .Append(" AND TEIKEI_KBN_N = '1'")
                Else
                    .Append(" AND TEIKEI_KBN_N = '0'")
                End If
                .Append(" )")
            End With

            If oraReader.DataReader(SQL) = True Then
                normalCnt = oraReader.GetInt64("CNT")
                normalKin = oraReader.GetInt64("KIN")
            End If

            oraReader.Close()

            '--------------------------------------------------
            '他行スケジュールマスタ作成
            '--------------------------------------------------
            '2017/01/19 saitou 東春信金(RSV2標準) UPD スリーエス対応 ---------------------------------------- START
            '標準版(スリーエス決済無し)用に変更
            With SQL
                .Length = 0
                .Append("INSERT INTO TAKOSCHMAST (")
                .Append(" TORIS_CODE_U")
                .Append(",TORIF_CODE_U")
                .Append(",FURI_DATE_U")
                .Append(",FUNOU_YDATE_U")
                .Append(",FMT_KBN_U")
                .Append(",BAITAI_CODE_U")
                .Append(",LABEL_CODE_U")
                .Append(",CODE_KBN_U")
                .Append(",TKIN_NO_U")
                .Append(",FUNOU_FLG_U")
                .Append(",SYORI_KEN_U")
                .Append(",SYORI_KIN_U")
                .Append(",FURI_KEN_U")
                .Append(",FURI_KIN_U")
                .Append(",FUNOU_KEN_U")
                .Append(",FUNOU_KIN_U")
                .Append(",SAKUSEI_DATE_U")
                .Append(") VALUES (")
                .Append(" " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
                .Append("," & SQ(OraSchReader.GetString("TORIF_CODE_S")))
                .Append("," & SQ(OraSchReader.GetString("FURI_DATE_S")))
                .Append("," & SQ(FUNOU_T1YDATE))
                .Append("," & SQ(OraSchReader.GetString("FMT_KBN_T")))
                .Append("," & SQ("00"))
                .Append("," & SQ(OraSchReader.GetString("LABEL_KBN_T")))
                .Append("," & SQ("4"))
                .Append("," & SQ(IniInfo.KINKOCD))
                .Append("," & SQ("0"))
                .Append("," & normalCnt.ToString)
                .Append("," & normalKin.ToString)
                .Append(",0")
                .Append(",0")
                .Append(",0")
                .Append(",0")
                .Append("," & SQ(strDate))
                .Append(")")
            End With
            'With SQL
            '    .Length = 0
            '    .Append("INSERT INTO TAKOSCHMAST (")
            '    .Append(" TORIS_CODE_U")
            '    .Append(",TORIF_CODE_U")
            '    .Append(",FURI_DATE_U")
            '    .Append(",FUNOU_YDATE_U")
            '    .Append(",FMT_KBN_U")
            '    .Append(",BAITAI_CODE_U")
            '    .Append(",LABEL_CODE_U")
            '    .Append(",CODE_KBN_U")
            '    .Append(",TKIN_NO_U")
            '    .Append(",TEIKEI_KBN_U")
            '    .Append(",FUNOU_FLG_U")
            '    .Append(",SYORI_KEN_U")
            '    .Append(",SYORI_KIN_U")
            '    .Append(",FURI_KEN_U")
            '    .Append(",FURI_KIN_U")
            '    .Append(",FUNOU_KEN_U")
            '    .Append(",FUNOU_KIN_U")
            '    .Append(",SAKUSEI_DATE_U")
            '    .Append(",HAISIN_YDATE_U")
            '    .Append(",HAISIN_DATE_U")
            '    .Append(",KESSAI_YDATE_U")
            '    .Append(",KESSAI_DATE_U")
            '    .Append(",TESUU_YDATE_U")
            '    .Append(",TESUU_DATE_U")
            '    .Append(",HENKAN_YDATE_U")
            '    .Append(",HENKAN_DATE_U")
            '    .Append(",TESUUKEI_FLG_U")
            '    .Append(",TESUUTYO_FLG_U")
            '    .Append(",KESSAI_FLG_U")
            '    .Append(",HENKAN_FLG_U")
            '    .Append(",TESUU_KIN_U")
            '    .Append(",TESUU_KIN1_U")
            '    .Append(",TESUU_KIN2_U")
            '    .Append(",TESUU_KIN3_U")
            '    .Append(",FUNOU_DATE_U")
            '    .Append(",ERR_KEN_U")
            '    .Append(",ERR_KIN_U")
            '    .Append(",SEND_KEN_U")
            '    .Append(",SEND_KIN_U")
            '    .Append(",JIFURI_TIME_STAMP_U")
            '    .Append(",KESSAI_TIME_STAMP_U")
            '    .Append(",TESUU_TIME_STAMP_U")
            '    .Append(") VALUES (")
            '    .Append(" " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
            '    .Append("," & SQ(OraSchReader.GetString("TORIF_CODE_S")))
            '    .Append("," & SQ(OraSchReader.GetString("FURI_DATE_S")))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ(OraSchReader.GetString("FUNOU_T1YDATE_S")))
            '    Else
            '        .Append("," & SQ(OraSchReader.GetString("FUNOU_T2YDATE_S")))
            '    End If
            '    .Append("," & SQ(OraSchReader.GetString("FMT_KBN_T")))
            '    .Append("," & SQ("00"))
            '    .Append("," & SQ(OraSchReader.GetString("LABEL_KBN_T")))
            '    .Append("," & SQ("3"))
            '    .Append("," & SQ(IniInfo.KINKOCD))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ("1"))
            '    Else
            '        .Append("," & SQ("2"))
            '    End If
            '    .Append("," & SQ("0"))
            '    .Append("," & (normalCnt + inputErrCnt).ToString)
            '    .Append("," & (normalKin + inputErrKin).ToString)
            '    .Append(",0")
            '    .Append(",0")
            '    .Append(",0")
            '    .Append(",0")
            '    .Append("," & SQ(strDate))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ(OraSchReader.GetString("HAISIN_T1YDATE_S")))
            '    Else
            '        .Append("," & SQ(OraSchReader.GetString("HAISIN_T2YDATE_S")))
            '    End If
            '    .Append("," & SQ(strDate))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ(OraSchReader.GetString("KESSAI_T1YDATE_S")))
            '    Else
            '        .Append("," & SQ(OraSchReader.GetString("KESSAI_T2YDATE_S")))
            '    End If
            '    .Append("," & SQ(New String("0"c, 8)))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ(OraSchReader.GetString("TESUU_T1YDATE_S")))
            '    Else
            '        .Append("," & SQ(OraSchReader.GetString("TESUU_T2YDATE_S")))
            '    End If
            '    .Append("," & SQ(New String("0"c, 8)))
            '    If OraSchReader.GetString("SORTKEY") = "1" Then
            '        .Append("," & SQ(OraSchReader.GetString("HENKAN_T1YDATE_S")))
            '    Else
            '        .Append("," & SQ(OraSchReader.GetString("HENKAN_T2YDATE_S")))
            '    End If
            '    .Append("," & SQ(New String("0"c, 8)))
            '    .Append("," & SQ("0"))
            '    .Append("," & SQ("0"))
            '    .Append("," & SQ("0"))
            '    .Append("," & SQ("0"))
            '    .Append(",0")
            '    .Append(",0")
            '    .Append(",0")
            '    .Append(",0")
            '    .Append("," & SQ(New String("0"c, 8)))
            '    .Append("," & inputErrCnt.ToString)
            '    .Append("," & inputErrKin.ToString)
            '    .Append("," & normalCnt.ToString)
            '    .Append("," & normalKin.ToString)
            '    .Append("," & SQ(strDate & strTime))
            '    .Append("," & SQ(New String("0"c, 14)))
            '    .Append("," & SQ(New String("0"c, 14)))
            '    .Append(")")
            'End With
            '2017/01/19 saitou 東春信金(RSV2標準) UPD ------------------------------------------------------- END

            Dim SQLDel As New StringBuilder
            SQLDel.Append("DELETE FROM TAKOSCHMAST")
            SQLDel.Append(" WHERE TORIS_CODE_U = " & SQ(OraSchReader.GetString("TORIS_CODE_S")))
            SQLDel.Append(" AND TORIF_CODE_U = " & SQ(OraSchReader.GetString("TORIF_CODE_S")))
            SQLDel.Append(" AND FURI_DATE_U = " & SQ(OraSchReader.GetString("FURI_DATE_S")))
            SQLDel.Append(" AND TKIN_NO_U = " & SQ(IniInfo.KINKOCD))
            '2017/01/19 saitou 東春信金(RSV2標準) DEL スリーエス対応 ---------------------------------------- START
            '標準版(スリーエス決済無し)は提携内のみの扱いなので、他行スケジュールマスタに提携区分は持たない。
            'If OraSchReader.GetString("SORTKEY") = "1" Then
            '    SQLDel.Append(" AND TEIKEI_KBN_U = '1'")
            'Else
            '    SQLDel.Append(" AND TEIKEI_KBN_U = '2'")
            'End If
            '2017/01/19 saitou 東春信金(RSV2標準) DEL ------------------------------------------------------- END

            Dim nRet As Integer = MainDB.ExecuteNonQuery(SQLDel)
            If nRet >= 0 Then
                LOG.Write("他行スケジュール作成", "成功", "先行レコードを削除")
            End If

            nRet = MainDB.ExecuteNonQuery(SQL)
            If nRet > 0 Then
                LOG.Write("他行スケジュール作成", "成功", "")
            End If

        Catch ex As Exception
            LOG.Write("他行スケジュール作成", "失敗", ex.Message)
            Return False
        Finally
            If Not oraReader Is Nothing Then
                oraReader.Close()
                oraReader = Nothing
            End If
        End Try

        Return True

    End Function

#End Region

#Region "プライベートクラス"

    ''' <summary>
    ''' 送付票印刷クラス
    ''' </summary>
    ''' <remarks></remarks>
    Private Class KF3SP005
        Inherits CAstReports.ClsReportBase
        Public Sub New()
            'CSVファイルセット
            InfoReport.ReportName = "KF3SP005"
            '定義体名セット
            ReportBaseName = "KF3SP005_SSS送付票.rpd"
        End Sub

        Public Overrides Function CreateCsvFile() As String
            Return MyBase.CreateCsvFile()
        End Function

        Public Sub OutputCsvData(ByVal data As String, Optional ByVal dq As Boolean = False, Optional ByVal crlf As Boolean = False)
            CSVObject.Output(data, dq, crlf)
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

    End Class

#End Region

End Class
